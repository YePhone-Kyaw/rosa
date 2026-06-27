using backend.Data;
using backend.DTOs;
using backend.Models;
using backend.Services;
using Microsoft.EntityFrameworkCore;

namespace backend.Tests;

public class CategoryServiceTests
{
    private AppDbContext GetInMemoryDb()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        return new AppDbContext(options);
    }

    private async Task<(CategoryService categoryService, Category category)> CreateTestData()
    {
        var db = GetInMemoryDb();
        var categoryService = new CategoryService(db);
        var result = await categoryService.CreateCategory(
            new CreateCategoryDto { CategoryName = "Electronics" },
            "electronic.png"
        );
        var category = await db.Categories.FirstAsync();
        
        return (categoryService, category);
    }

    [Fact]
    public async Task GetAllCategories_ReturnEmptyList_WhenNoCategories()
    {
        var db = GetInMemoryDb();
        var categoryService = new CategoryService(db);

        var categories = await categoryService.GetAllCategories();

        Assert.Empty(categories);
    }

    [Fact]
    public async Task GetAllCategories_ReturnCategories_WhenExist()
    {
        var testData = await CreateTestData();
        var categories = await testData.categoryService.GetAllCategories();

        Assert.Single(categories);
        Assert.Equal("Electronics", categories[0].CategoryName);
        Assert.Equal("electronic.png", categories[0].CategoryImageUrl);
    }
}
