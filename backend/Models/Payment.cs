namespace backend.Models;

public class Payment
{
    public int PaymentId { get; set; }
    public int OrderId { get; set; }
    public Order Order { get; set; } = null!;
    public string PaymentIntentId { get; set; } = string.Empty;
    public string Status { get; set; } = "pending";
    public decimal Amount { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
