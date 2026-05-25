namespace backend.Models;

public class Order
{
    public int OrderId { get; set; }
    public int UserId { get; set; }
    public User User { get; set; } = null!;
    public List<OrderItem> OrderItems { get; set; } = new();
    public string Status { get; set; } = "pending";
    public decimal TotalAmount { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}