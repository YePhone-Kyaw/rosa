namespace backend.DTOs;

public class RegisterDto
{
    public required string Name { get; set; }
    public required string Email { get; set; }
    public required string Password { get; set; }
}

public class LoginDto
{
    public required string Email { get; set; }
    public required string Password { get; set; }
}

public class GoogleLoginDto
{
    public required string IdToken { get; set; }
}

public class FacebookLoginDto
{
    public required string AccessToken { get; set; }
}

public class FacebookUserResponse
{
    public string id { get; set; } = string.Empty;
    public string name { get; set; } = string.Empty;
    public string email { get; set; } = string.Empty;
    public FacebookPicture? picture { get; set; }
}

public class FacebookPicture
{
    public FacebookPictureData? data { get; set; }
}

public class FacebookPictureData
{
    public string? url { get; set; }
}

public class UserResponseDto
{
    public int UserId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
}
