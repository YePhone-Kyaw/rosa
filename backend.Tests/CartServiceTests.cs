using backend.Data;
using backend.DTOs;
using backend.Models;
using backend.Services;
using Microsoft.EntityFrameworkCore;

namespace backend.Tests;

public class CartServiceTests
{
    private AppDbContext GetInMemoryDb()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        return new AppDbContext(options);
    }

    private async Task<(CartService cartService, User user, Product product)> CreateTestData()
    {
        var db = GetInMemoryDb();

        var user = new User { Name = "Test", Email = "test@gmail.com", Password = "password", AuthProvider = "email" };
        db.Users.Add(user);

        var category = new Category { CategoryName = "Electronics", CategoryImageUrl = "electronics.png" };
        db.Categories.Add(category);
        await db.SaveChangesAsync();

        var product = new Product
        {
            ProductName = "Laptop",
            Description = "A good laptop",
            Price = 999.99m,
            Stock = 5,
            ImageUrl = "laptop.png",
            CategoryId = category.CategoryId
        };
        db.Products.Add(product);
        await db.SaveChangesAsync();

        var cartService = new CartService(db);
        return (cartService, user, product);
    }

    [Fact]
    public async Task GetCartByUserId_ReturnNull_WhenNoCart()
    {
        var testData = await CreateTestData();
        var cart = await testData.cartService.GetCartByUserId(testData.user.UserId);

        Assert.Null(cart);
    }

    [Fact]
    public async Task AddToCart_CreatesCartAndAddsItem()
    {
        var testData = await CreateTestData();
        var cart = await testData.cartService.AddToCart(
            testData.user.UserId,
            new AddToCartDto { ProductId = testData.product.ProductId, Quantity = 2 }
        );

        Assert.NotNull(cart);
        Assert.Single(cart.CartItems);
        Assert.Equal("Laptop", cart.CartItems[0].ProductName);
        Assert.Equal(2, cart.CartItems[0].Quantity);
        Assert.Equal(1999.98m, cart.TotalAmount);
    }

    [Fact]
    public async Task AddToCart_IncrementsQuantity_WhenProductAlreadyInCart()
    {
        var testData = await CreateTestData();

        await testData.cartService.AddToCart(
            testData.user.UserId,
            new AddToCartDto { ProductId = testData.product.ProductId, Quantity = 1 }
        );

        var cart = await testData.cartService.AddToCart(
            testData.user.UserId,
            new AddToCartDto { ProductId = testData.product.ProductId, Quantity = 3 }
        );

        Assert.Single(cart.CartItems);
        Assert.Equal(4, cart.CartItems[0].Quantity);
    }

    [Fact]
    public async Task UpdateCartItem_UpdatesQuantity()
    {
        var testData = await CreateTestData();
        var cart = await testData.cartService.AddToCart(
            testData.user.UserId,
            new AddToCartDto { ProductId = testData.product.ProductId, Quantity = 2 }
        );

        var updated = await testData.cartService.UpdateCartItem(
            cart.CartItems[0].CartItemId,
            testData.user.UserId,
            new UpdateCartDto { Quantity = 5 }
        );

        Assert.NotNull(updated);
        Assert.Equal(5, updated.CartItems[0].Quantity);
    }

    [Fact]
    public async Task UpdateCartItem_RemovesItem_WhenQuantityIsZero()
    {
        var testData = await CreateTestData();
        var cart = await testData.cartService.AddToCart(
            testData.user.UserId,
            new AddToCartDto { ProductId = testData.product.ProductId, Quantity = 2 }
        );

        var updated = await testData.cartService.UpdateCartItem(
            cart.CartItems[0].CartItemId,
            testData.user.UserId,
            new UpdateCartDto { Quantity = 0 }
        );

        Assert.NotNull(updated);
        Assert.Empty(updated.CartItems);
    }

    [Fact]
    public async Task UpdateCartItem_ReturnNull_WhenItemNotExists()
    {
        var testData = await CreateTestData();
        var result = await testData.cartService.UpdateCartItem(
            999,
            testData.user.UserId,
            new UpdateCartDto { Quantity = 5 }
        );

        Assert.Null(result);
    }

    [Fact]
    public async Task RemoveFromCart_RemovesItem()
    {
        var testData = await CreateTestData();
        var cart = await testData.cartService.AddToCart(
            testData.user.UserId,
            new AddToCartDto { ProductId = testData.product.ProductId, Quantity = 2 }
        );

        var updated = await testData.cartService.RemoveFromCart(
            cart.CartItems[0].CartItemId,
            testData.user.UserId
        );

        Assert.NotNull(updated);
        Assert.Empty(updated.CartItems);
    }

    [Fact]
    public async Task RemoveFromCart_ReturnNull_WhenItemNotExists()
    {
        var testData = await CreateTestData();
        var result = await testData.cartService.RemoveFromCart(999, testData.user.UserId);

        Assert.Null(result);
    }

    [Fact]
    public async Task CleanCart_RemovesAllItems()
    {
        var testData = await CreateTestData();
        await testData.cartService.AddToCart(
            testData.user.UserId,
            new AddToCartDto { ProductId = testData.product.ProductId, Quantity = 2 }
        );

        var cleaned = await testData.cartService.CleanCart(testData.user.UserId);

        Assert.NotNull(cleaned);
        Assert.Empty(cleaned.CartItems);
        Assert.Equal(0, cleaned.TotalAmount);
    }

    [Fact]
    public async Task CleanCart_ReturnNull_WhenNoCart()
    {
        var testData = await CreateTestData();
        var result = await testData.cartService.CleanCart(testData.user.UserId);

        Assert.Null(result);
    }
}
