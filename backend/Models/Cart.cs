namespace backend.Models;

public class Cart
{
    public int CartId { get; set; }
    public int UserId { get; set; }
    public User User { get; set; } = null!;
    public List<CartItem> CartItems { get; set; } = new();
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}