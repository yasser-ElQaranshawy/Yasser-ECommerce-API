using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Yasser.Services;

namespace Yasser.Controllers
{
    [Route("api/cart")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;

        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }

        // دالة مساعدة لجلب الـ UserId أو الـ Guest للتسهيل حالياً
        private string GetUserId()
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                userId = "guest_1"; // مؤقتم لحين اكتمال الـ Authentication مع بسملة
            }
            return userId;
        }

        // 1. جلب محتويات السلة
        [HttpGet]
        public async Task<IActionResult> GetCart()
        {
            var userId = GetUserId();
            var cart = await _cartService.GetCartAsync(userId);
            return Ok(cart);
        }

        // 2. إضافة منتج للسلة (باستخدام الـ ProductId فقط والمخزون من داتابيز محمد)
        [HttpPost("items")]
        public async Task<IActionResult> AddItem([FromBody] AddToCartDto dto)
        {
            try
            {
                var userId = GetUserId();
                await _cartService.AddItemAsync(userId, dto.ProductId, dto.Quantity);
                return Ok(new { message = "Item added to cart successfully from catalog!" });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { error = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        // 3. تعديل كمية منتج في السلة
        [HttpPut("items/{cartItemId}")]
        public async Task<IActionResult> UpdateQuantity(int cartItemId, [FromBody] UpdateQuantityDto dto)
        {
            var userId = GetUserId();
            await _cartService.UpdateQuantityAsync(userId, cartItemId, dto.Quantity);
            return Ok(new { message = "Item quantity updated successfully" });
        }

        // 4. حذف منتج من السلة
        [HttpDelete("items/{cartItemId}")]
        public async Task<IActionResult> RemoveItem(int cartItemId)
        {
            var userId = GetUserId();
            await _cartService.RemoveItemAsync(userId, cartItemId);
            return Ok(new { message = "Item removed from cart successfully" });
        }
    }

    // الـ DTOs الخاصة بالـ Cart
    public record AddToCartDto(int ProductId, int Quantity);
    public record UpdateQuantityDto(int Quantity);
}