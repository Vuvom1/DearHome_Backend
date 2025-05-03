using DearHome_Backend.DTOs.ShippingDtos;
using DearHome_Backend.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DearHome_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShippingController : ControllerBase
    {
        private readonly IShippingService _shippingService;

        public ShippingController(IShippingService shippingService)
        {
            _shippingService = shippingService ?? throw new ArgumentNullException(nameof(shippingService));
        }

        [HttpGet("validate-token")]
        public async Task<IActionResult> ValidateToken()
        {
            var token = await _shippingService.GetValidationTokenAsync();
            return Ok(token);
        }

        [HttpPost("calculate-shipping-cost")]
        public async Task<IActionResult> CalculateShippingCost([FromBody] ShippingCostDto shippingCostDto)
        {
            var cost = await _shippingService.CalculateShippingCostAsync(
                shippingCostDto.AddressToId,
                shippingCostDto.AddressToId,
                "0",
                "0",
                "0",   
                "0",
                "0"
            );
            return Ok(cost);
        }

        [HttpGet("get-cities")]
        public async Task<IActionResult> GetCities()
        {
            var cities = await _shippingService.GetCitiesAsync();
            return Ok(cities);
        }

        [HttpGet("get-districts/{cityId}")]
        public async Task<IActionResult> GetDistrictsByCityId(string cityId)
        {
            var districts = await _shippingService.GetDistrictsByCityIdAsync(cityId);
            return Ok(districts);
        }

        [HttpGet("get-wards/{districtId}")]        
        public async Task<IActionResult> GetWardsByDistrictId(string districtId)
        {
            var wards = await _shippingService.GetWardsByDistrictIdAsync(districtId);
            return Ok(wards);
        }

        [HttpGet("get-formatted-address/{addressId}")]
        public async Task<IActionResult> GetFormatedAddress(Guid addressId)
        {
            var formatedAddress = await _shippingService.GetFormatedAddressAsync(addressId);
            return Ok(formatedAddress);
        }

        [HttpGet("get-citys-by-code/{code}")]
        public async Task<IActionResult> GetCitiesByCode(string code)
        {
            var cities = await _shippingService.GetCitiesByCodeAsync(code);
            return Ok(cities);
        }
    }
}
