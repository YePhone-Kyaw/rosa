using backend.Data;
using backend.DTOs;
using backend.Models;
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

    private async Task<(UserService userService, User user)> CreateTestData()
    {
        var db = GetInMemoryDb();
        var userService = new UserService(db);
        var user = await userService.Register(new RegisterDto { Name = "Test", Email = "test@gmail.com", Password = "password123" });

        return (userService, user);
    }

    [Fact]
    public async Task Register_CreatesUserWithHashedPassword()
    {
        var testData = await CreateTestData();

        Assert.Equal("Test", testData.user.Name);
        Assert.Equal("test@gmail.com", testData.user.Email);
        Assert.Equal("email", testData.user.AuthProvider);
    }

    [Fact]
    public async Task GetUserByEmail_ReturnUser_WhenExists()
    {
        var testData = await CreateTestData();
        var user = await testData.userService.GetUserByEmail("test@gmail.com");

        Assert.NotNull(user);
        Assert.Equal("test@gmail.com", user.Email);
    }

    [Fact]
    public async Task GetUserByEmail_ReturnNull_WhenNotExists()
    {
        var testData = await CreateTestData();
        var user = await testData.userService.GetUserByEmail("nobody@gmail.com");

        Assert.Null(user);
    }

    [Fact]
    public async Task GetUserById_ReturnUserData_WhenExists()
    {
        var testData = await CreateTestData();
        var user = await testData.userService.GetUserById(testData.user.UserId);

        Assert.NotNull(user);
        Assert.Equal("Test", user.Name);
        Assert.Equal("test@gmail.com", user.Email);
    }

    [Fact]
    public async Task GetUserById_ReturnNull_WhenNotExists()
    {
        var testData = await CreateTestData();
        var user = await testData.userService.GetUserById(111);

        Assert.Null(user);
    }

    [Fact]
    public async Task VerifyPassword_ReturnTrue_WhenCorrect()
    {
        var testData = await CreateTestData();
        var result = testData.userService.VerifyPassword("password123", testData.user.Password!);

        Assert.True(result);
    }

    [Fact]
    public async Task VerifyPassword_ReturnFalse_WhenIncorrect()
    {
        var testData = await CreateTestData();
        var result = testData.userService.VerifyPassword("wrongPassword", testData.user.Password!);

        Assert.False(result);
    }

    [Fact]
    public async Task UpdatePassword_ReturnTrue_WhenCorrect()
    {
        var testData = await CreateTestData();
        var result = await testData.userService.UpdatePassword(
            testData.user.UserId,
            new UpdatePasswordDto { CurrentPassword = "password123", NewPassword = "newPassword" }
        );

        Assert.True(result);

    }

    [Fact]
    public async Task UpdatePassword_ReturnFalse_WhenIncorrect()
    {
        var testData = await CreateTestData();
        var result = await testData.userService.UpdatePassword(
            testData.user.UserId,
            new UpdatePasswordDto { CurrentPassword = "wrongPassword", NewPassword = "newPassword" }
        );

        Assert.False(result);
    }

    [Fact]
    public async Task UpdatePassword_ReturnFalse_SocialUser()
    {
        var db = GetInMemoryDb();
        var userService = new UserService(db);
        var user = await userService.RegisterSocialUser("Google user", "google@gmail.com", null, "google");

        var result = await userService.UpdatePassword(
            user.UserId,
            new UpdatePasswordDto { CurrentPassword = "anything", NewPassword = "newPassword" }
        );

        Assert.False(result);
    }

    [Fact]
    public async Task RegisterSocialUser_CreateUserWithoutPassword()
    {
        var db = GetInMemoryDb();
        var userService = new UserService(db);
        var user = await userService.RegisterSocialUser("Google user", "google@gmail.com", null, "google");

        Assert.NotNull(user);
        Assert.Equal("Google user", user.Name);
        Assert.Equal("google@gmail.com", user.Email);
        Assert.Null(user.Password);
        Assert.Equal("google", user.AuthProvider);
    }

    [Fact]
    public async Task UpdateProfile_ReturnUserById()
    {
        var testData = await CreateTestData();
        var updatedProfile = await testData.userService.UpdateProfile(
            testData.user.UserId,
            new UpdateProfileDto { Name = "New name", Email = "newemail@gmail.com", ProfilePicture = null }
        );
        
        Assert.NotNull(updatedProfile);
        Assert.Equal("New name", updatedProfile.Name);
        Assert.Equal("newemail@gmail.com", updatedProfile.Email);
    }

    [Fact]
    public async Task UpdateProfilePicture()
    {
        var testData = await CreateTestData();
        await testData.userService.UpdateProfilePicture(testData.user.UserId, "testingPicture.png");

        var profile = await testData.userService.GetUserById(testData.user.UserId);

        Assert.NotNull(profile);
        Assert.Equal("testingPicture.png", profile.ProfilePicture);
    }
}
