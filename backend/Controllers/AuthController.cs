using backend.DTOs;
using backend.Models;
using backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers;

[ApiController]
[Route("auth")]
public class AuthController : ControllerBase
{
    private readonly UserService _userService;
    private readonly TokenService _tokenService;

    public AuthController(UserService userService, TokenService tokenService)
    {
        _userService = userService;
        _tokenService = tokenService;
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
        Response.Cookies.Append("token", token, new CookieOptions
        {
            HttpOnly = true,
            Secure = false, // Will change to true for production (HTTPS)
            SameSite = SameSiteMode.Lax,
            Expires = DateTimeOffset.UtcNow.AddMinutes(1440)
        });

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
        Response.Cookies.Append("token", token, new CookieOptions
        {
            HttpOnly = true,
            Secure = false, // Will change to true for production (HTTPS)
            SameSite = SameSiteMode.Lax,
            Expires = DateTimeOffset.UtcNow.AddMinutes(1440)
        });

        return Ok(new UserResponseDto
        {
            UserId = user.UserId,
            Name = user.Name,
            Email = user.Email,
            Role = user.Role
        });
    }

    [HttpPost("logout")]
    public IActionResult Logout()
    {
        Response.Cookies.Delete("token");
        return Ok(new { message = "Logged out successfully" });
    }
}
