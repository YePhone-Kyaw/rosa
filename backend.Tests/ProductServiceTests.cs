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

    [Fact]
    public async Task GetProductById_ReturnNull_WhenNotExists()
    {
        var testData = await CreateTestData();
        var product = await testData.productService.GetProductById(12345);

        Assert.Null(product);
    }

    [Fact]
    public async Task GetAllProducts_ReturnProductsData_WhenExist()
    {
        var testData = await CreateTestData();
        var products = await testData.productService.GetAllProducts();
        
        Assert.Equal(1, products.TotalItems);
        Assert.Equal(1, products.TotalPages);
        Assert.Single(products.Products);
    }

    [Fact]
    public async Task GetAllProducts_ReturnEmptyList_WhenNoProducts()
    {
        var db = GetInMemoryDb();
        var category = new Category { CategoryName = "Electronics", CategoryImageUrl = "electronics.png" };
        db.Categories.Add(category);
        await db.SaveChangesAsync();

        var productService = new ProductService(db);
        var products = await productService.GetAllProducts();

        Assert.Equal(0, products.TotalItems);
        Assert.Empty(products.Products);
    }

    [Fact]
    public async Task CreateProduct_ReturnProductData_WhenSuccessful()
    {
        var db = GetInMemoryDb();
        var category = new Category { CategoryName = "Books", CategoryImageUrl = "books.png" };
        db.Categories.Add(category);
        await db.SaveChangesAsync();

        var productService = new ProductService(db);
        var product = await productService.CreateProduct(new CreateProductDto
        {
            ProductName = "C# in Depth",
            Description = "A great book",
            Price = 49.99m,
            Stock = 20,
            ImageUrl = "csharp.png",
            CategoryId = category.CategoryId
        });

        Assert.NotNull(product);
        Assert.Equal("C# in Depth", product.ProductName);
        Assert.Equal(49.99m, product.Price);
        Assert.Equal(20, product.Stock);
        Assert.Equal("Books", product.CategoryName);
    }

    [Fact]
    public async Task UpdateProduct_ReturnUpdatedData_WhenExists()
    {
        var testData = await CreateTestData();
        var product = await testData.productService.UpdateProduct(
            testData.product.ProductId,
            new UpdateProductDto { ProductName = "Gaming Laptop", Price = 1499.99m }
        );

        Assert.NotNull(product);
        Assert.Equal("Gaming Laptop", product.ProductName);
        Assert.Equal(1499.99m, product.Price);
        Assert.Equal(5, product.Stock);
    }

    [Fact]
    public async Task UpdateProduct_ReturnNull_WhenNotExists()
    {
        var testData = await CreateTestData();
        var product = await testData.productService.UpdateProduct(
            999,
            new UpdateProductDto { ProductName = "Nothing" }
        );

        Assert.Null(product);
    }

    [Fact]
    public async Task DeleteProduct_ReturnTrue_WhenExists()
    {
        var testData = await CreateTestData();
        var result = await testData.productService.DeleteProduct(testData.product.ProductId);

        Assert.True(result);
    }

    [Fact]
    public async Task DeleteProduct_ReturnFalse_WhenNotExists()
    {
        var testData = await CreateTestData();
        var result = await testData.productService.DeleteProduct(999);

        Assert.False(result);
    }
}
