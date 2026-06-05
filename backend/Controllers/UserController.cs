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
    private readonly S3Service _s3;
    public UserController(UserService userService, S3Service s3)
    {
        _userService = userService;
        _s3 = s3;
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
    public async Task<IActionResult> UploadProfilePicture([FromForm]IFormFile file)
    {
        if (file == null)
        {
            return BadRequest(new { message = "No file uploaded" });
        }

        var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);
        var user = await _userService.GetUserById(userId);
        if (user == null) return NotFound();

        if (!string.IsNullOrEmpty(user.ProfilePicture))
        {
            await _s3.DeleteFileAsync(user.ProfilePicture);
        }

        var url = await _s3.UploadFileAsync(file, "profile-pictures");
        await _userService.UpdateProfilePicture(userId, url);

        return Ok(new { profilePicture = url });
    }

    [HttpDelete("profile/picture")]
    [Authorize]
    public async Task<IActionResult> DeleteProfilePicture()
    {
        var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);
        var user = await _userService.GetUserById(userId);
        if (user == null) return NotFound();

        if (!string.IsNullOrEmpty(user.ProfilePicture))
        {
            await _s3.DeleteFileAsync(user.ProfilePicture);
        }
        await _userService.UpdateProfilePicture(userId, null);

        return Ok(new { message = "Profile picture removed" });
    }
}
