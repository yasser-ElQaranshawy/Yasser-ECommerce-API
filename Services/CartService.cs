
using Microsoft.EntityFrameworkCore;
using Yasser.Data;
using Yasser.Models;

namespace Yasser.Services
{
    public interface ICartService
    {
        Task<ShoppingCart> GetCartAsync(string userId);
        Task AddItemAsync(string userId, int productId, int quantity);
        Task UpdateQuantityAsync(string userId, int cartItemId, int quantity);
        Task RemoveItemAsync(string userId, int cartItemId);
        Task ClearCartAsync(string userId);
    }

    public class CartService : ICartService
    {
        private readonly AppDbContext _context;

        public CartService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<ShoppingCart> GetCartAsync(string userId)
        {
            var cart = await _context.ShoppingCarts
                .Include(c => c.Items)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null)
            {
                cart = new ShoppingCart
                {
                    UserId = userId,
                    Items = new List<CartItem>() // تهيئة القائمة لتجنب الـ NullReferenceException
                };
                _context.ShoppingCarts.Add(cart);
                await _context.SaveChangesAsync();
            }
            else if (cart.Items == null)
            {
                cart.Items = new List<CartItem>();
            }

            return cart;
        }

        public async Task AddItemAsync(string userId, int productId, int quantity)
        {
            // 1. جلب المنتج من داتابيز المنتجات للتأكد من السعر والمخزون
            var product = await _context.Products.FindAsync(productId);
            if (product == null)
            {
                throw new KeyNotFoundException("Product not found in the catalog.");
            }

            if (product.Stock < quantity)
            {
                throw new InvalidOperationException("Requested quantity is not available in stock.");
            }

            // 2. جلب سلة المستخدم والتأكد من تهيئة العناصر
            var cart = await GetCartAsync(userId);
            if (cart.Items == null)
            {
                cart.Items = new List<CartItem>();
            }

            var existingItem = cart.Items.FirstOrDefault(i => i.ProductId == productId);

            if (existingItem != null)
            {
                existingItem.Quantity += quantity;
            }
            else
            {
                cart.Items.Add(new CartItem
                {
                    ShoppingCartId = cart.Id,
                    ProductId = product.Id,
                    ProductName = product.Name,
                    ProductImageUrl = product.ImageUrl,
                    UnitPrice = product.Price, // السعر الحقيقي من الداتابيز
                    Quantity = quantity
                });
            }

            cart.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }

        public async Task UpdateQuantityAsync(string userId, int cartItemId, int quantity)
        {
            var cart = await GetCartAsync(userId);
            if (cart.Items == null) return;

            var item = cart.Items.FirstOrDefault(i => i.Id == cartItemId);

            if (item != null)
            {
                if (quantity <= 0)
                {
                    cart.Items.Remove(item);
                }
                else
                {
                    item.Quantity = quantity;
                }

                cart.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }
        }

        public async Task RemoveItemAsync(string userId, int cartItemId)
        {
            var cart = await GetCartAsync(userId);
            if (cart.Items == null) return;

            var item = cart.Items.FirstOrDefault(i => i.Id == cartItemId);

            if (item != null)
            {
                cart.Items.Remove(item);
                cart.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }
        }

        public async Task ClearCartAsync(string userId)
        {
            var cart = await GetCartAsync(userId);
            if (cart != null && cart.Items != null && cart.Items.Any())
            {
                _context.CartItems.RemoveRange(cart.Items);
                await _context.SaveChangesAsync();
            }
        }
    }
}