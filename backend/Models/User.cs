namespace backend.Models;
public class User
{
    public int UserId { get; set; }
    public required string Name { get; set; }
    public required string Email { get; set; }
    public required string Password { get; set; }
    public string Role { get; set; } = "customer";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}