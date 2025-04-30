using DearHome_Backend.DTOs.PaymentDto;
using DearHome_Backend.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DearHome_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;

        public PaymentController(IPaymentService paymentService)
        {
            _paymentService = paymentService ?? throw new ArgumentNullException(nameof(paymentService));
        }

        [HttpGet("get-payment-link-infomation")]
        public async Task<IActionResult> GetPaymentLinkInformation([FromQuery] long id)
        {
            var paymentInfo = await _paymentService.GetPaymentLinkInformationAsync(id);
            return Ok(paymentInfo);
        }

        [HttpPost("verify-webhook")]
        public IActionResult VerifyWebhook([FromQuery] VerifyPaymentDto verifyPaymentDto)
        {
            if (verifyPaymentDto == null)
            {
                return BadRequest("Webhook data cannot be null.");
            }

            // Return 200 OK to acknowledge receipt of webhook
            return Ok();
        }

        [HttpPost("verify-payment")]
        public async Task<IActionResult> VerifyPayment([FromBody] VerifyPaymentDto verifyPaymentDto)
        {
                
            // Process the webhook data
            await _paymentService.VerifyPaymentAsync(verifyPaymentDto);
                
            // Return 200 OK to acknowledge receipt of webhook
            return Ok();
        }
        
    }
}
