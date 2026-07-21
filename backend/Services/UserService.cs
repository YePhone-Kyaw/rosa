using backend.Data;
using backend.DTOs;
using backend.Models;
using Microsoft.EntityFrameworkCore;

namespace backend.Services;

public class UserService
{
    private readonly AppDbContext _db;
    private readonly IFileStorageService _fileStorage;
    public UserService(AppDbContext db, IFileStorageService fileStorage)
    {
        _db = db;
        _fileStorage = fileStorage;
    }

    public async Task<User?> GetUserByEmail(string email)
    {
        return await _db.Users.FirstOrDefaultAsync((user) => user.Email == email);
    }

    public async Task<User> Register(RegisterDto dto)
    {
        var user = new User
        {
            Name = dto.Name,
            Email = dto.Email,
            Password = BCrypt.Net.BCrypt.HashPassword(dto.Password),
            IsEmailVerified = true,
            AuthProvider = "email"
        };

        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        return user;
    }

    public bool VerifyPassword(string password, string hashedPassword)
    {
        return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
    }

    public async Task<ProfileResponseDto?> GetUserById(int userId)
    {
        return await _db.Users
            .Where((user) => user.UserId == userId)
            .Select((user) => new ProfileResponseDto
            {
                UserId = user.UserId,
                Name = user.Name,
                Email = user.Email ?? string.Empty,
                Role = user.Role,
                ProfilePicture = user.ProfilePicture,
                AuthProvider = user.AuthProvider,
                CreatedAt = user.CreatedAt
            })
            .FirstOrDefaultAsync();

    }

    public async Task<ProfileResponseDto?> UpdateProfile(int userId, UpdateProfileDto dto)
    {
        var user = await _db.Users.FindAsync(userId);
        if (user == null) return null;

        if (dto.Name != null) user.Name = dto.Name;
        if (dto.Email != null) user.Email = dto.Email;
        await _db.SaveChangesAsync();

        return await GetUserById(userId);
    }

    public async Task<bool> UpdatePassword(int userId, UpdatePasswordDto dto)
    {
        var user = await _db.Users.FindAsync(userId);
        if (user == null) return false;

        if (user.Password == null) return false;

        var isValid = BCrypt.Net.BCrypt.Verify(dto.CurrentPassword, user.Password);
        if (!isValid) return false;

        user.Password = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);
        await _db.SaveChangesAsync();

        return true;
    }

    public async Task<string?> UpdateProfilePicture(int userId, IFormFile profileImage)
    {
        var user = await _db.Users.FindAsync(userId);
        if (user == null) return null;

        if (!string.IsNullOrEmpty(user.ProfilePicture))
        {
            await _fileStorage.DeleteFileAsync(user.ProfilePicture);
        }

        var url = await _fileStorage.UploadFileAsync(profileImage, "profile-picture");

        user.ProfilePicture = url;
        await _db.SaveChangesAsync();

        return url;
    }

    public async Task<bool> DeleteProfilePicture(int userId)
    {
        var user = await _db.Users.FindAsync(userId);
        if (user == null) return false;

        if (!string.IsNullOrEmpty(user.ProfilePicture))
        {
            await _fileStorage.DeleteFileAsync(user.ProfilePicture);
        }

        user.ProfilePicture = null;
        await _db.SaveChangesAsync();

        return true;
    }

    public async Task<User> RegisterSocialUser(string name, string email, string? picture, string authProvider)
    {
        var user = new User
        {
            Name = name,
            Email = email,
            Password = null,
            ProfilePicture = picture,
            IsEmailVerified = true,
            AuthProvider = authProvider
        };

        _db.Users.Add(user);
        await _db.SaveChangesAsync();
        return user;
    }
}
