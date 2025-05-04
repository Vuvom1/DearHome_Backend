using System;
using DearHome_Backend.Services.Interfaces;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
namespace DearHome_Backend.Services.Implementations;

using System.Collections.Generic;
using System.Net.Http.Headers;
using DearHome_Backend.DTOs.ShippingDtos;
using DearHome_Backend.Repositories.Interfaces;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;

public class ShippingService : IShippingService
{
    private readonly IVariantRepository _variantRepository;
    private readonly IAddressRepository _addressRepository;
    private readonly IConfiguration _configuration;
    private readonly HttpClient _httpClient;
    private readonly IDistributedCache _cache;
    private readonly ILogger<ShippingService> _logger;
    private readonly TelemetryClient _telemetryClient;
    private const string TokenCacheKey = "GoShip_AccessToken";
    private const string ExpiryTimeCacheKey = "GoShip_TokenExpiry";
    public ShippingService(
        IVariantRepository variantRepository,
        IAddressRepository addressRepository,
        IConfiguration configuration,
        HttpClient httpClient,
        IDistributedCache cache,
        ILogger<ShippingService> logger,
        TelemetryClient telemetryClient)
    {
        _variantRepository = variantRepository ?? throw new ArgumentNullException(nameof(variantRepository));
        _addressRepository = addressRepository ?? throw new ArgumentNullException(nameof(addressRepository));
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _cache = cache ?? throw new ArgumentNullException(nameof(cache));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _telemetryClient = telemetryClient ?? throw new ArgumentNullException(nameof(telemetryClient));

        // Set the base address for the HttpClient
        _httpClient.BaseAddress = new Uri(_configuration["GoShip:BaseUrl"]);
    }
    public async Task<string> GetValidationTokenAsync()
    {
        using var operation = _telemetryClient.StartOperation<DependencyTelemetry>("GetValidToken");
        try
        {
            string accessToken = null;
            string expiryTimeString = null;
            bool cacheOperationSucceeded = true;

            // Add retry policy for Redis operations
            int maxRetries = 3;
            int currentRetry = 0;
            TimeSpan delay = TimeSpan.FromSeconds(1);

            while (currentRetry <= maxRetries)
            {
                try
                {
                    // Try to get the token from cache with retry logic
                    accessToken = await _cache.GetStringAsync(TokenCacheKey);
                    expiryTimeString = await _cache.GetStringAsync(ExpiryTimeCacheKey);
                    break; // Success, exit retry loop
                }
                catch (Exception ex) when (ex is StackExchange.Redis.RedisConnectionException ||
                                          ex is StackExchange.Redis.RedisTimeoutException ||
                                          ex is System.Net.Sockets.SocketException)
                {
                    // Only retry on connection-related exceptions
                    if (currentRetry == maxRetries)
                    {
                        _logger.LogError(ex, "Redis cache access failed after {RetryCount} retries", maxRetries);
                        _telemetryClient.TrackEvent("RedisCacheFailure", new Dictionary<string, string>
                    {
                        { "Operation", "GetToken" },
                        { "Error", ex.Message }
                    });
                        cacheOperationSucceeded = false;
                        break;
                    }

                    currentRetry++;
                    _logger.LogWarning(ex, "Redis cache access failed (attempt {RetryCount}/{MaxRetries}). Retrying...",
                        currentRetry, maxRetries);

                    // Track dependency failure with Application Insights
                    _telemetryClient.TrackDependency(
                        "Redis",
                        "GetToken",
                        $"{TokenCacheKey}/{ExpiryTimeCacheKey}",
                        DateTimeOffset.UtcNow,
                        TimeSpan.FromMilliseconds(500),
                        false);

                    await Task.Delay(delay);
                    delay = TimeSpan.FromSeconds(Math.Min(30, Math.Pow(2, currentRetry))); // Exponential backoff up to 30 seconds
                }
            }

            bool tokenValid = false;

            if (!string.IsNullOrEmpty(accessToken) && !string.IsNullOrEmpty(expiryTimeString))
            {
                if (DateTime.TryParse(expiryTimeString, out DateTime expiryTime))
                {
                    // Check if token is still valid (with 5-minute buffer)
                    tokenValid = expiryTime > DateTime.UtcNow.AddMinutes(5);

                    // Add telemetry for token validation
                    _telemetryClient.TrackMetric("TokenTimeRemaining", (expiryTime - DateTime.UtcNow).TotalMinutes);
                }
            }

            if (!tokenValid)
            {
                _logger.LogInformation("GoShip token not found in cache or expired. Requesting new token.");

                // Get fresh token
                var (newToken, expiresIn) = await RequestNewTokenAsync();

                // Calculate expiry time
                var expiryTime = DateTime.UtcNow.AddSeconds(expiresIn);

                // Create cache options
                var cacheOptions = new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(expiresIn - 60) // 1 minute before actual expiry
                };

                // Only try to cache if the previous cache operations succeeded
                if (cacheOperationSucceeded)
                {
                    currentRetry = 0;
                    delay = TimeSpan.FromSeconds(1);

                    while (currentRetry <= maxRetries)
                    {
                        try
                        {
                            // Use multiple Set operations in a more resilient way
                            await Task.WhenAll(
                                _cache.SetStringAsync(TokenCacheKey, newToken, cacheOptions),
                                _cache.SetStringAsync(ExpiryTimeCacheKey, expiryTime.ToString("o"), cacheOptions)
                            );

                            _logger.LogInformation("Successfully cached GoShip token. Expires at {ExpiryTime}", expiryTime);
                            break; // Success, exit retry loop
                        }
                        catch (Exception ex) when (ex is StackExchange.Redis.RedisConnectionException ||
                                                 ex is StackExchange.Redis.RedisTimeoutException ||
                                                 ex is System.Net.Sockets.SocketException)
                        {
                            if (currentRetry == maxRetries)
                            {
                                _logger.LogError(ex, "Failed to cache GoShip token after {RetryCount} retries", maxRetries);
                                _telemetryClient.TrackEvent("RedisCacheSetFailure", new Dictionary<string, string>
                            {
                                { "Operation", "SetToken" },
                                { "Error", ex.Message }
                            });
                                break;
                            }

                            currentRetry++;
                            _logger.LogWarning(ex, "Redis cache write failed (attempt {RetryCount}/{MaxRetries}). Retrying...",
                                currentRetry, maxRetries);

                            await Task.Delay(delay);
                            delay = TimeSpan.FromSeconds(Math.Min(30, Math.Pow(2, currentRetry))); // Exponential backoff
                        }
                    }
                }
                else
                {
                    _logger.LogWarning("Skipping token caching due to previous Redis failures");
                }

                accessToken = newToken;
            }

            return accessToken;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while getting valid GoShip token");
            _telemetryClient.TrackException(ex, new Dictionary<string, string> {
            { "Method", "GetValidationTokenAsync" }
        });

            // Try direct token acquisition as fallback
            try
            {
                _logger.LogWarning("Attempting emergency direct token acquisition");
                var (emergencyToken, _) = await RequestNewTokenAsync();
                return emergencyToken;
            }
            catch (Exception fallbackEx)
            {
                _logger.LogError(fallbackEx, "Emergency token acquisition failed");
                throw; // Re-throw the original exception
            }
        }
    }

    private async Task<(string Token, int ExpiresIn)> RequestNewTokenAsync()
    {
        using var operation = _telemetryClient.StartOperation<Microsoft.ApplicationInsights.DataContracts.DependencyTelemetry>("RequestNewGoShipToken");
        try
        {
            var goshipConfig = _configuration.GetSection("Goship");
            if (goshipConfig == null)
            {
                throw new InvalidOperationException("GoShip configuration is missing in application settings");
            }

            var apiSecretConfig = goshipConfig.GetSection("ApiSecret");
            if (apiSecretConfig == null)
            {
                throw new InvalidOperationException("GoShip API Secret configuration is missing in application settings");
            }

            var username = apiSecretConfig["username"];
            var password = apiSecretConfig["password"];
            var clientId = apiSecretConfig["client_id"];
            var clientSecret = apiSecretConfig["client_secret"];

            if (string.IsNullOrEmpty(clientId) || string.IsNullOrEmpty(clientSecret) ||
                string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                throw new InvalidOperationException("GoShip configuration is incomplete in application settings");
            }

            var authRequest = new
            {
                grant_type = "password",
                client_id = clientId,
                client_secret = clientSecret,
                username = username,
                password = password
            };

            // Make authentication request to GoShip API
            var response = await _httpClient.PostAsJsonAsync("login", authRequest);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogError("Failed to authenticate with GoShip API. Status: {StatusCode}, Error: {Error}",
                    response.StatusCode, errorContent);
                throw new Exception($"Failed to authenticate with GoShip API: {response.StatusCode}");
            }

            var authResult = await response.Content.ReadFromJsonAsync<GoshipAuthResponse>();

            if (authResult == null || string.IsNullOrEmpty(authResult.access_token))
            {
                throw new Exception("Invalid authentication response from GoShip API");
            }

            return (authResult.access_token, authResult.expires_in);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while requesting new GoShip token");
            _telemetryClient.TrackException(ex);
            throw;
        }
    }

    public Task<bool> ValidateShippingAddressAsync(Guid addressId)
    {
        // Implementation for validating shipping address
        throw new NotImplementedException();
    }

    public Task<string> GetShippingCarrierAsync(Guid addressId)
    {
        // Implementation for getting shipping carrier
        throw new NotImplementedException();
    }

    public Task<string> TrackShipmentAsync(string trackingNumber)
    {
        // Implementation for tracking shipment
        throw new NotImplementedException();
    }

    public Task<bool> UpdateShippingStatusAsync(Guid orderId, string status)
    {
        // Implementation for updating shipping status
        throw new NotImplementedException();
    }

    public async Task<object> CalculateShippingCostAsync(Guid addressId, Guid addressFromId, string cod, string weight, string length, string width, string height)
    {

        //Send request to GoShip API for shipping cost calculation
        var address = await _addressRepository.GetByIdAsync(addressId);
        if (address == null)
        {
            throw new Exception("Address not found");
        }
        var cityId = address.City;
        var districtId = address.District;

        var request = new
        {
            shipment = new
            {
                address_from = new
                {
                    city = "700000",
                    district = "700700",
                },
                address_to = new
                {
                    city = cityId,
                    district = districtId,
                },
                parcel = new
                {
                    code = cod,
                    width = width,
                    length = length,
                    weight = weight,
                    height = height,
                },
            }
        };

        var token = await GetValidationTokenAsync();
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await _httpClient.PostAsJsonAsync("rates", request);
        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            _logger.LogError("Failed to calculate shipping cost. Status: {StatusCode}, Error: {Error}",
                response.StatusCode, errorContent);
            throw new Exception($"Failed to calculate shipping cost: {errorContent}");
        }

        var shippingCostResult = await response.Content.ReadFromJsonAsync<GoshipShippingCostResponse>();

        if (shippingCostResult == null)
        {
            throw new Exception("Invalid shipping cost response from GoShip API");
        }

        return shippingCostResult.data;
    }

    public async Task<object> GetDistrictsByCityIdAsync(string cityId)
    {
        var token = GetValidationTokenAsync();
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.Result);
        var response = await _httpClient.GetAsync($"cities/{cityId}/districts");

        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            _logger.LogError("Failed to get districts. Status: {StatusCode}, Error: {Error}",
                response.StatusCode, errorContent);
            throw new Exception($"Failed to get districts: {errorContent}");
        }

        var districtsResult = await response.Content.ReadFromJsonAsync<GoshipResponse>();
        if (districtsResult == null)
        {
            throw new Exception("Invalid districts response from GoShip API");
        }
        return districtsResult.data;
    }

    public async Task<object> GetCitiesAsync()
    {
        var token = GetValidationTokenAsync();
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.Result);
        var response = await _httpClient.GetAsync("cities");

        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            _logger.LogError("Failed to get cities. Status: {StatusCode}, Error: {Error}",
                response.StatusCode, errorContent);
            throw new Exception($"Failed to get cities: {errorContent}");
        }
        var citiesResult = await response.Content.ReadFromJsonAsync<GoshipResponse>();
        if (citiesResult == null)
        {
            throw new Exception("Invalid cities response from GoShip API");
        }
        return citiesResult.data;
    }

    

    public async Task<GoshipCreateShipmentResponse> CreateShipmentAsync(string shippingRate, Guid addressFromId, Guid addressToId, string cod, string weight, string length, string width, string height)
    {
        var token = GetValidationTokenAsync();
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.Result);

        var address = await _addressRepository.GetByIdAsync(addressToId);
        if (address == null)
        {
            throw new Exception("Address not found");
        }
        var cityId = address.City;
        var districtId = address.District;

        var request = new
        {
            shipment = new
            {
                rate = shippingRate,
                address_from = new
                {
                    name = "Dear Home",
                    phone = "123456789",
                    street = "123 Main St",
                    ward = "9233",
                    city = "700000",
                    district = "700700",

                },
                address_to = new
                {
                    name = "Customer Name",
                    phone = "123456789",
                    street = address.Street,
                    ward = "9020",
                    city = cityId,
                    district = districtId,
                },
                parcel = new
                {
                    cod = cod,
                    width = width,
                    length = length,
                    weight = weight,
                    height = height,
                    metadata = "metadata",
                },
            }
        };

        var response = await _httpClient.PostAsJsonAsync("shipments", request);
        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            _logger.LogError("Failed to create shipment. Status: {StatusCode}, Error: {Error}",
                response.StatusCode, errorContent);
            throw new Exception($"Failed to create shipment: {errorContent}");
        }
        var shipmentResult = await response.Content.ReadFromJsonAsync<GoshipCreateShipmentResponse>();
        if (shipmentResult == null)
        {
            throw new Exception("Invalid shipment response from GoShip API");
        }
        return shipmentResult;
    }

    public async Task<object> GetWardsByDistrictIdAsync(string districtId)
    {
        var token = GetValidationTokenAsync();
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.Result);
        var response = await _httpClient.GetAsync($"districts/{districtId}/wards");

        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            _logger.LogError("Failed to get wards. Status: {StatusCode}, Error: {Error}",
                response.StatusCode, errorContent);
            throw new Exception($"Failed to get wards: {errorContent}");
        }

        var wardsResult = await response.Content.ReadFromJsonAsync<GoshipResponse>();
        if (wardsResult == null)
        {
            throw new Exception("Invalid wards response from GoShip API");
        }
        return wardsResult.data;
    }

    public async Task<object> GetCitiesByCodeAsync(string code)
    {
        object cities = await GetCitiesAsync();
        
        // Handle JsonElement properly
        var citiesElement = (System.Text.Json.JsonElement)cities;
        System.Text.Json.JsonElement? foundCity = null;
        
        if (citiesElement.ValueKind == System.Text.Json.JsonValueKind.Array)
        {
            foreach (var city in citiesElement.EnumerateArray())
            {
                if (city.TryGetProperty("id", out var idProperty) && 
                    idProperty.GetString() == code)
                {
                    foundCity = city;
                    break;
                }
            }
        }
        
        if (foundCity == null)
        {
            throw new Exception("City not found");
        }
        
        return foundCity.Value;
    }

    public async Task<object> GetFormatedAddressAsync(Guid addressId)
    {
        var address = await _addressRepository.GetByIdAsync(addressId);
        if (address == null)
        {
            throw new Exception("Address not found");
        }
        var city = await GetCitiesByCodeAsync(address.City!);

        var districts = await GetDistrictsByCityIdAsync(address.City!);
        var district = ((System.Text.Json.JsonElement)districts).EnumerateArray()
            .FirstOrDefault(d => d.GetProperty("id").GetString() == address.District);

        var wards = await GetWardsByDistrictIdAsync(address.District!);
        var ward = ((System.Text.Json.JsonElement)wards).EnumerateArray()
            .FirstOrDefault(w => w.GetProperty("id").ValueKind == System.Text.Json.JsonValueKind.Number 
                ? w.GetProperty("id").GetInt64().ToString() == address.Ward
                : w.GetProperty("id").GetString() == address.Ward);

        var formatedAddress = new
        {
            id = address.Id,
            address.Street,
            city,
            district,
            ward, 
            address.PostalCode,   
            address.Country,
        };
        return formatedAddress;
    }
}


