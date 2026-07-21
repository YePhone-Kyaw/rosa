using backend.DTOs;
using backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers;

[ApiController]
[Route("user")]
public class UserController : ControllerBase
{
    private readonly UserService _userService;
    public UserController(UserService userService)
    {
        _userService = userService;
    }

    [HttpGet("profile")]
    [Authorize]
    public async Task<IActionResult> GetProfile()
    {
        var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);
        var user = await _userService.GetUserById(userId);
        return Ok(user);
    }

    [HttpPut("profile")]
    [Authorize]
    public async Task<IActionResult> UpdateProfile(UpdateProfileDto dto)
    {
        var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);
        var profile = await _userService.UpdateProfile(userId, dto);
        if (profile == null) return NotFound(new { message = "User not found" });
        return Ok(profile);
    }

    [HttpPut("password")]
    [Authorize]
    public async Task<IActionResult> UpdatePassword(UpdatePasswordDto dto)
    {
        var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);
        var updatedPassword = await _userService.UpdatePassword(userId, dto);
        if (!updatedPassword) return BadRequest(new { message = "Current password is incorrect" });
        return Ok(new { message = "Password updated successfully" });
    }

    [HttpPost("profile/picture")]
    [Authorize]
    public async Task<IActionResult> UploadProfilePicture([FromForm] IFormFile profileImage)
    {
        var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);
        var user = await _userService.GetUserById(userId);
        if (user == null) return NotFound(new { message = "User not found" });

        var url = await _userService.UpdateProfilePicture(userId, profileImage);
        
        return Ok(new { profilePicture = url });
    }

    [HttpDelete("profile/picture")]
    [Authorize]
    public async Task<IActionResult> DeleteProfilePicture()
    {
        var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);
        var user = await _userService.GetUserById(userId);
        if (user == null) return NotFound(new { message = "User not found" });

        var profilePictureToDelete = await _userService.DeleteProfilePicture(userId);
        if (!profilePictureToDelete) return NotFound(new { message = "Profile picture not found to delete" });

        return Ok(new { message = "Profile picture removed" });
    }
}
