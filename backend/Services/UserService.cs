using backend.Data;
using backend.DTOs;
using backend.Models;
using Microsoft.EntityFrameworkCore;

namespace backend.Services;

public class UserService
{
    private readonly AppDbContext _db;
    public UserService(AppDbContext db) 
    {
        _db = db;
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
            Password = BCrypt.Net.BCrypt.HashPassword(dto.Password)
        };

        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        return user;
    }

    public bool VerifyPassword(string password, string hashedPassword)
    {
        return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
    }
}