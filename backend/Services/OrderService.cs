using backend.Data;
using backend.DTOs;
using backend.Models;
using Microsoft.EntityFrameworkCore;

namespace backend.Services;

public class OrderService
{
    private readonly AppDbContext _db;
    public OrderService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<List<OrderResponseDto>> GetOrdersByUserId(int userId)
    {
        return await _db.Orders
            .Where((order) => order.UserId == userId)
            .Include((order) => order.OrderItems)
                .ThenInclude((orderItem) => orderItem.Product)
            .Select((order) => new OrderResponseDto
            {
                OrderId = order.OrderId,
                Status = order.Status,
                TotalAmount = order.TotalAmount,
                CreatedAt = order.CreatedAt,
                Items = order.OrderItems.Select((orderItem) => new OrderItemResponseDto
                {
                    ProductId = orderItem.ProductId,
                    ProductName = orderItem.Product.ProductName,
                    Quantity = orderItem.Quantity,
                    UnitPrice = orderItem.Product.Price,
                    Subtotal = orderItem.Quantity * orderItem.UnitPrice
                }).ToList()
            })
            .ToListAsync();
    }

    public async Task<OrderResponseDto?> GetOrderById(int id)
    {
        return await _db.Orders
            .Where((order) => order.OrderId == id)
            .Include((order) => order.OrderItems)
                .ThenInclude((orderItem) => orderItem.Product)
            .Select((order) => new OrderResponseDto
            {
                OrderId = order.OrderId,
                Status = order.Status,
                TotalAmount = order.TotalAmount,
                CreatedAt = order.CreatedAt,
                Items = order.OrderItems.Select((orderItem) => new OrderItemResponseDto
                {
                    ProductId = orderItem.ProductId,
                    ProductName = orderItem.Product.ProductName,
                    Quantity = orderItem.Quantity,
                    UnitPrice = orderItem.UnitPrice,
                    Subtotal = orderItem.Quantity * orderItem.UnitPrice
                }).ToList()
            })
            .FirstOrDefaultAsync();
    }

    public async Task<OrderResponseDto> CreateOrder(int userId, CreateOrderDto dto)
    {   
        // Fetch each product that customer chooses to order
        var productIds = dto.Items.Select((item) => item.ProductId).ToList();

        var products = await _db.Products
            .Where((product) => productIds.Contains(product.ProductId))
            .ToListAsync();
        
        // Calculate the total amount
        var totalAmount = dto.Items.Sum((item) =>
        {
            var product = products.First((product) => product.ProductId == item.ProductId);
            return product.Price * item.Quantity;
        });

        // Create the order
        var order = new Order
        {
            UserId = userId,
            TotalAmount = totalAmount,
            Status = "pending"
        };

        _db.Orders.Add(order);
        await _db.SaveChangesAsync();

        // Creating order items
        var orderItems = dto.Items.Select((item) =>
        {
            var product = products.First((product) => product.ProductId == item.ProductId);
            return new OrderItem
            {
                OrderId = order.OrderId,
                ProductId = item.ProductId,
                Quantity = item.Quantity,
                UnitPrice = product.Price
            };
        }).ToList();

        _db.OrderItems.AddRange(orderItems);
        await _db.SaveChangesAsync();

        // Update the stock for each product
        foreach (var item in dto.Items)
        {
            var product = products.First((product) => product.ProductId == item.ProductId);
            product.Stock -= item.Quantity;
        }

        await _db.SaveChangesAsync();

        return await GetOrderById(order.OrderId) ?? throw new Exception("Order not found after creation");
    }

    public async Task<OrderResponseDto?> UpdateOrderStatus(int id, string status)
    {
        var order = await _db.Orders.FindAsync(id);
        if (order == null) return null;

        order.Status = status;
        await _db.SaveChangesAsync();

        return await GetOrderById(id);
    }
}
