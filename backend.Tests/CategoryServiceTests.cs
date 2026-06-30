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
    
    [Fact]
    public async Task GetCategoryById_ReturnCategoryData_WhenExists()
    {
        var testData = await CreateTestData();
        var category = await testData.categoryService.GetCategoryById(testData.category.CategoryId);

        Assert.NotNull(category);
        Assert.Equal("Electronics", category.CategoryName);
        Assert.Equal("electronic.png", category.CategoryImageUrl);
    }

    [Fact]
    public async Task GetCategoryById_ReturnNull_WhenNotExists()
    {
        var testData = await CreateTestData();
        var category = await testData.categoryService.GetCategoryById(1234);

        Assert.Null(category);
    }
}
