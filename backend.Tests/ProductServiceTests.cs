using backend.Data;
using backend.DTOs;
using backend.Models;
using backend.Services;
using Microsoft.EntityFrameworkCore;

namespace backend.Tests;

public class ProductServiceTests
{
    private AppDbContext GetInMemoryDb()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        return new AppDbContext(options);
    }

    private async Task<(ProductService productService, Product product, Category category)> CreateTestData()
    {
        var db = GetInMemoryDb();
        var category = new Category { CategoryName = "Electronics", CategoryImageUrl = "electronics.png" };
        db.Categories.Add(category);
        await db.SaveChangesAsync();

        var productService = new ProductService(db);
        var result = await productService.CreateProduct(new CreateProductDto
        {
            ProductName = "Laptop",
            Description = "A good laptop",
            Price = 999.99m,
            Stock = 5,
            ImageUrl = "laptop.png",
            CategoryId = category.CategoryId
        });

        var product = await db.Products.FirstAsync();

        return (productService, product, category);
    }

    [Fact]
    public async Task GetProductById_ReturnProductDetails_WhenExists()
    {
        var testData = await CreateTestData();
        var product = await testData.productService.GetProductById(testData.product.ProductId);

        Assert.NotNull(product);
        Assert.Equal("Laptop", product.ProductName);
        Assert.Equal("laptop.png", product.ImageUrl);
        Assert.Equal("A good laptop", product.Description);
        Assert.Equal(5, product.Stock);
        Assert.Equal(999.99m, product.Price);
    }
}
