using EXAMPORTAL.DTOs;
using EXAMPORTAL.Repositery.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EXAMPORTAL.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentsController : ControllerBase
    {
        private readonly IPaymentService _svc;
        public PaymentsController(IPaymentService svc) { _svc = svc; }

        [Authorize]
        [HttpPost("create")]
        public async Task<IActionResult> CreatePayment(PaymentCreateDto dto)
        {
            var result = await _svc.CreatePaymentAsync(dto);
            return Ok(result);
        }

        // Stripe webhook endpoint
        [HttpPost("stripe/webhook")]
        public async Task<IActionResult> StripeWebhook()
        {
            var json = await new StreamReader(Request.Body).ReadToEndAsync();
            var sigHeader = Request.Headers["Stripe-Signature"].FirstOrDefault();
            try
            {
                await _svc.HandleStripeWebhookAsync(json, sigHeader);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [Authorize]
        [HttpGet("{paymentId}/receipt")]
        public async Task<IActionResult> GetReceipt(int paymentId)
        {
            var ms = new MemoryStream();
            await _svc.GenerateReceiptPdfAsync(paymentId, ms);
            ms.Position = 0;
            return File(ms, "application/pdf", $"receipt_{paymentId}.pdf");
        }
    }
}
