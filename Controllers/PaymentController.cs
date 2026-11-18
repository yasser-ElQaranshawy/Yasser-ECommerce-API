using Microsoft.AspNetCore.Mvc;
using Yasser.Services;

namespace Yasser.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;

        public PaymentController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        // تنفيذ عملية الدفع وإتمام الطلب (Checkout)
        [HttpPost("checkout")]
        public async Task<IActionResult> Checkout([FromBody] CheckoutDto dto)
        {
            try
            {
                // تم التعديل لاستخدام UserId بدلاً من Identifier
                var payment = await _paymentService.ProcessCheckoutAsync(dto.UserId, dto.PaymentMethod);
                return Ok(new
                {
                    message = "Checkout and payment completed successfully!",
                    transactionId = payment.TransactionId,
                    amountPaid = payment.Amount,
                    paymentMethod = payment.PaymentMethod,
                    status = payment.Status.ToString(),
                    paymentDate = payment.CreatedAt
                });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "An internal error occurred during checkout.", details = ex.Message });
            }
        }
    }

    // DTO لبيانات الدفع - تم تعديله ليطابق طلب الـ UserId
    public record CheckoutDto(string UserId, string PaymentMethod);
}