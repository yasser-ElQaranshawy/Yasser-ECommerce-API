namespace Yasser.Models
{
    public enum PaymentStatus
    {
        Pending,
        Completed,
        Failed
    }

    public class Payment
    {
        public int Id { get; set; }
        public int ShoppingCartId { get; set; }
        public ShoppingCart? ShoppingCart { get; set; }

        public decimal Amount { get; set; }
        public string PaymentMethod { get; set; } = string.Empty;
        public PaymentStatus Status { get; set; }
        public string TransactionId { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}