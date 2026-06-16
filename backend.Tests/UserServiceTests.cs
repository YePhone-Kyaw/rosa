using backend.Data;
using backend.DTOs;
using backend.Services;
using Microsoft.EntityFrameworkCore;

namespace backend.Tests;

public class UserServiceTests
{
    private AppDbContext GetInMemoryDb()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        return new AppDbContext(options);
    }

    [Fact]
    public async Task Register_CreatesUserWithHashedPassword()
    {
        var db = GetInMemoryDb();
        var service = new UserService(db);
        var dto = new RegisterDto { Name = "Test", Email = "test@gmail.com", Password = "password123" };

        var user = await service.Register(dto);

        Assert.Equal("Test", user.Name);
        Assert.Equal("test@gmail.com", user.Email);
    }
}
