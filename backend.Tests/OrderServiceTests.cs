using backend.Data;
using backend.DTOs;
using backend.Models;
using backend.Services;
using Microsoft.EntityFrameworkCore;

namespace backend.Tests;

public class OrderServiceTests
{
    private AppDbContext GetInMemoryDb()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        return new AppDbContext(options);
    }

    private async Task<(OrderService orderService, User user, Product product, AppDbContext db)> CreateTestData()
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
            Stock = 10,
            ImageUrl = "laptop.png",
            CategoryId = category.CategoryId
        };
        db.Products.Add(product);
        await db.SaveChangesAsync();

        var orderService = new OrderService(db);
        return (orderService, user, product, db);
    }

    private async Task<OrderResponseDto> CreateTestOrder(
        OrderService orderService, int userId, int productId, int quantity = 2)
    {
        return await orderService.CreateOrder(userId, new CreateOrderDto
        {
            Items = new List<CreateOrderItemDto>
            {
                new CreateOrderItemDto { ProductId = productId, Quantity = quantity }
            }
        });
    }

    [Fact]
    public async Task GetOrdersByUserId_ReturnEmptyList_WhenNoOrders()
    {
        var testData = await CreateTestData();
        var orders = await testData.orderService.GetOrdersByUserId(testData.user.UserId);

        Assert.Empty(orders);
    }

    [Fact]
    public async Task GetOrdersByUserId_ReturnOrders_WhenExist()
    {
        var testData = await CreateTestData();
        await CreateTestOrder(testData.orderService, testData.user.UserId, testData.product.ProductId);

        var orders = await testData.orderService.GetOrdersByUserId(testData.user.UserId);

        Assert.Single(orders);
        Assert.Equal("pending", orders[0].Status);
    }

    [Fact]
    public async Task GetOrderById_ReturnOrderDetails_WhenExists()
    {
        var testData = await CreateTestData();
        var created = await CreateTestOrder(testData.orderService, testData.user.UserId, testData.product.ProductId);

        var order = await testData.orderService.GetOrderById(created.OrderId);

        Assert.NotNull(order);
        Assert.Equal("pending", order.Status);
        Assert.Single(order.Items);
        Assert.Equal("Laptop", order.Items[0].ProductName);
        Assert.Equal(2, order.Items[0].Quantity);
        Assert.Equal(999.99m, order.Items[0].UnitPrice);
        Assert.Equal(1999.98m, order.TotalAmount);
    }

    [Fact]
    public async Task GetOrderById_ReturnNull_WhenNotExists()
    {
        var testData = await CreateTestData();
        var order = await testData.orderService.GetOrderById(999);

        Assert.Null(order);
    }

    [Fact]
    public async Task CreateOrder_CreatesOrderWithCorrectTotal()
    {
        var testData = await CreateTestData();

        var order = await CreateTestOrder(testData.orderService, testData.user.UserId, testData.product.ProductId, 3);

        Assert.NotNull(order);
        Assert.Equal("pending", order.Status);
        Assert.Equal(2999.97m, order.TotalAmount);  // 999.99 * 3
        Assert.Single(order.Items);
        Assert.Equal(3, order.Items[0].Quantity);
    }

    [Fact]
    public async Task CreateOrder_ReducesProductStock()
    {
        var testData = await CreateTestData();

        await CreateTestOrder(testData.orderService, testData.user.UserId, testData.product.ProductId, 3);

        // Check stock was reduced
        var product = await testData.db.Products.FindAsync(testData.product.ProductId);
        Assert.NotNull(product);
        Assert.Equal(7, product.Stock);  // 10 - 3
    }

    [Fact]
    public async Task UpdateOrderStatus_ReturnUpdatedOrder_WhenExists()
    {
        var testData = await CreateTestData();
        var created = await CreateTestOrder(testData.orderService, testData.user.UserId, testData.product.ProductId);

        var updated = await testData.orderService.UpdateOrderStatus(created.OrderId, "shipped");

        Assert.NotNull(updated);
        Assert.Equal("shipped", updated.Status);
    }

    [Fact]
    public async Task UpdateOrderStatus_ReturnNull_WhenNotExists()
    {
        var testData = await CreateTestData();
        var result = await testData.orderService.UpdateOrderStatus(999, "shipped");

        Assert.Null(result);
    }
}
