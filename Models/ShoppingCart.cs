namespace Yasser.Models
{
    public class ShoppingCart
    {
        public int Id { get; set; }
        public string? UserId { get; set; } // للمستخدم المسجل
        public string? GuestId { get; set; } // للزائر المؤقت
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<CartItem> Items { get; set; } = new List<CartItem>();
    }
}