using backend.DTOs;
using backend.Models;
using backend.Services;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers;

[ApiController]
[Route("auth")]
public class AuthController : ControllerBase
{
    private readonly UserService _userService;
    private readonly TokenService _tokenService;
    private readonly IConfiguration _config;

    public AuthController(UserService userService, TokenService tokenService, IConfiguration config)
    {
        _userService = userService;
        _tokenService = tokenService;
        _config = config;
    }

    private CookieOptions GetCookieOptions()
    {
        return new CookieOptions
        {
            HttpOnly = true,
            Secure = false,
            SameSite = SameSiteMode.Lax,
            Expires = DateTimeOffset.UtcNow.AddMinutes(1440)
        };
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterDto dto)
    {
        var existingUser = await _userService.GetUserByEmail(dto.Email);
        if (existingUser != null)
        {
            return BadRequest(new { message = "Email already in use" });
        }

        var user = await _userService.Register(dto);

        var token = _tokenService.GenerateToken(user);
        Response.Cookies.Append("token", token, GetCookieOptions());

        return Ok(new UserResponseDto
        {
            UserId = user.UserId,
            Name = user.Name,
            Email = user.Email,
            Role = user.Role
        }
        );
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto dto)
    {
        var user = await _userService.GetUserByEmail(dto.Email);
        if (user == null)
        {
            return Unauthorized(new { message = "Invalid email" });
        }

        if (user.AuthProvider != "email" || user.Password == null)
        {
            return Unauthorized(new { message = "Please login with your social account" });
        }

        var isValid = _userService.VerifyPassword(dto.Password, user.Password);

        if (!isValid)
        {
            return Unauthorized(new { message = "Incorrect password" });
        }

        var token = _tokenService.GenerateToken(user);
        Response.Cookies.Append("token", token, GetCookieOptions());

        return Ok(new UserResponseDto
        {
            UserId = user.UserId,
            Name = user.Name,
            Email = user.Email,
            Role = user.Role
        });
    }

    [HttpPost("google")]
    public async Task<IActionResult> GoogleLogin(GoogleLoginDto dto)
    {
        try
        {
            var payload = await GoogleJsonWebSignature.ValidateAsync(dto.IdToken, new GoogleJsonWebSignature.ValidationSettings
            {
                Audience = new[] { _config["Google:ClientId"] }
            });

            var user = await _userService.GetUserByEmail(payload.Email);
            if (user == null)
            {
                user = await _userService.RegisterSocialUser(payload.Name, payload.Email, payload.Picture, "google");
            }

            var token = _tokenService.GenerateToken(user);
            Response.Cookies.Append("token", token, GetCookieOptions());

            return Ok(new UserResponseDto
            {
                UserId = user.UserId,
                Name = user.Name,
                Email = user.Email,
                Role = user.Role
            });
        }
        catch (InvalidJwtException)
        {
            return BadRequest(new { message = "Invalid Google Token" });
        }
    }

    [HttpPost("facebook")]
    public async Task<IActionResult> FacebookLogin(FacebookLoginDto dto)
    {
        try
        {
            var httpClient = new HttpClient();
            var response = await httpClient.GetAsync($"https://graph.facebook.com/me?fields=id,name,email,picture&access_token={dto.AccessToken}");

            if (!response.IsSuccessStatusCode)
            {
                return BadRequest(new { message = "Invalid Facebook token" });
            }

            var content = await response.Content.ReadAsStringAsync();
            var fbUser = System.Text.Json.JsonSerializer.Deserialize<FacebookUserResponse>(content);

            if (fbUser == null || string.IsNullOrEmpty(fbUser.email))
            {
                return BadRequest(new { message = "Could not get email from Facebook" });
            }

            var user = await _userService.GetUserByEmail(fbUser.email);
            if (user == null)
            {
                user = await _userService.RegisterSocialUser(fbUser.name, fbUser.email, fbUser.picture?.data?.url, "facebook");
            }

            var token = _tokenService.GenerateToken(user);
            Response.Cookies.Append("token", token, GetCookieOptions());

            return Ok(new UserResponseDto
            {
                UserId = user.UserId,
                Name = user.Name,
                Email = user.Email,
                Role = user.Role
            });
        }
        catch (Exception)
        {
            return BadRequest(new { message = "Facebook login failed" });
        }
    }

    [HttpPost("logout")]
    public IActionResult Logout()
    {
        Response.Cookies.Delete("token");
        return Ok(new { message = "Logged out successfully" });
    }
}
