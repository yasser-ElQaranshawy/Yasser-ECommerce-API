using Microsoft.EntityFrameworkCore;
using Yasser.Data;
using Yasser.Models;

namespace Yasser.Services
{
    public interface IPaymentService
    {
        // تغيير المعامل من identifier إلى userId ليتطابق مع سلة المستخدم
        Task<Payment> ProcessCheckoutAsync(string userId, string paymentMethod);
    }

    public class PaymentService : IPaymentService
    {
        private readonly AppDbContext _context;
        private readonly ICartService _cartService;

        public PaymentService(AppDbContext context, ICartService cartService)
        {
            _context = context;
            _cartService = cartService;
        }

        public async Task<Payment> ProcessCheckoutAsync(string userId, string paymentMethod)
        {
            // 1. جلب السلة الخاصة بالمستخدم الصحيح (guest_1) بالمنتجات بتاعتها
            var cart = await _cartService.GetCartAsync(userId);

            if (cart.Items == null || !cart.Items.Any())
            {
                throw new InvalidOperationException("Cart is empty. Cannot proceed with checkout.");
            }

            // 2. حساب الإجمالي الكلي للمنتجات في السلة
            decimal totalAmount = cart.Items.Sum(item => item.UnitPrice * item.Quantity);

            // 3. إنشاء سجل الدفع (Payment Record)
            var payment = new Payment
            {
                ShoppingCartId = cart.Id,
                Amount = totalAmount,
                PaymentMethod = paymentMethod,
                Status = PaymentStatus.Completed, // افتراض نجاح الدفع كمحاكاة
                TransactionId = "TXN_" + Guid.NewGuid().ToString().Substring(0, 8).ToUpper(),
                CreatedAt = DateTime.UtcNow
            };

            _context.Payments.Add(payment);

            // 4. تفريغ السلة بعد نجاح الدفع وإتمام الأوردر باستخدام الـ userId
            await _cartService.ClearCartAsync(userId);

            await _context.SaveChangesAsync();

            return payment;
        }
    }
}