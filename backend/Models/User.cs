namespace backend.Models;
public class User
{
    public int UserId { get; set; }
    public required string Name { get; set; }
    public required string Email { get; set; }
    public string? Password { get; set; }
    public string Role { get; set; } = "customer";
    public string? ProfilePicture { get; set; }
    public bool IsEmailVerified { get; set; } = false;
    public string? EmailVerificationToken { get; set; }
    public string AuthProvider { get; set; } = "email";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}