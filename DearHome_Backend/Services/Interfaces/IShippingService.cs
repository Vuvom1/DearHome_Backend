using System;
using DearHome_Backend.DTOs.ShippingDtos;

namespace DearHome_Backend.Services.Interfaces;

public interface IShippingService
{
    Task<string> GetValidationTokenAsync();
    Task<object> CalculateShippingCostAsync(Guid addressToId, Guid addressFromId, string cod, string weight, string length, string width, string height);
    Task<bool> ValidateShippingAddressAsync(Guid addressId);
    Task<string> GetShippingCarrierAsync(Guid addressId);
    Task<GoshipCreateShipmentResponse> CreateShipmentAsync(string shippingRate, Guid addressFromId, Guid addressToId, string cod, string weight, string length, string width, string height);
    Task<string> TrackShipmentAsync(string trackingNumber);
    Task<bool> UpdateShippingStatusAsync(Guid orderId, string status);
    Task<object> GetCitiesAsync();
    Task<object> GetCitiesByCodeAsync(string code);
    Task<object> GetDistrictsByCityIdAsync(string cityId);
    Task<object> GetWardsByDistrictIdAsync(string districtId);
    Task<object> GetFormatedAddressAsync(Guid addressId);
}
