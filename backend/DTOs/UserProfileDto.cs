namespace backend.DTOs;

public class UpdateProfileDto
{
    public string? Name { get; set; }
    public string? Email { get; set; }
    public string? ProfilePicture { get; set; }
}

public class UpdatePasswordDto
{
    public required string CurrentPassword { get; set; }
    public required string NewPassword { get; set; }
}

public class ProfileResponseDto
{
    public int UserId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public string? ProfilePicture { get; set; }
    public DateTime CreatedAt { get; set; }
}