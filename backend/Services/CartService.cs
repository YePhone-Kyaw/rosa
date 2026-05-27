using backend.Data;
using backend.DTOs;
using backend.Models;
using Microsoft.EntityFrameworkCore;

namespace backend.Services;

public class CartService
{
    private readonly AppDbContext _db;
    public CartService (AppDbContext db)
    {
        _db = db;
    }

    public async Task<CartResponseDto?> GetCartByUserId(int userId)
    {
        return await _db.Carts
            .Where((cart) => cart.UserId == userId)
            .Include((cart) => cart.CartItems)
                .ThenInclude((cart) => cart.Product)
            .Select((cart) => new CartResponseDto
            {
                CartId = cart.CartId,
                TotalAmount = cart.CartItems.Sum((cartItem) => cartItem.Quantity * cartItem.Product.Price),
                CartItems = cart.CartItems.Select((cartItem) => new CartItemResponseDto
                {
                    ProductId = cartItem.ProductId,
                    ProductName = cartItem.Product.Name,
                    Quantity = cartItem.Quantity,
                    UnitPrice = cartItem.Product.Price,
                    Subtotal = cartItem.Quantity * cartItem.Product.Price
                }).ToList()
            })
            .FirstOrDefaultAsync();
    }

    public async Task<CartResponseDto> AddToCart(int userId, AddToCartDto dto)
    {
        // Find existing cart or create new one
        var cart = await _db.Carts.FirstOrDefaultAsync((cart) => cart.UserId == userId);
        if (cart == null)
        {
            cart = new Cart { UserId = userId };
            _db.Carts.Add(cart);
            await _db.SaveChangesAsync();
        }

        // Check if product already exists in cart
        var existingItem = await _db.CartItems
            .FirstOrDefaultAsync((cartItem) => cartItem.CartId == cart.CartId
                && cartItem.ProductId == dto.ProductId);
        
        if (existingItem != null)
        {
            existingItem.Quantity += dto.Quantity;
        } else
        {
            // Add new cart item if the product doesn't exist in the cart
            var cartItem = new CartItem
            {
                CartId = cart.CartId,
                ProductId = dto.ProductId,
                Quantity = dto.Quantity
            };
            _db.CartItems.Add(cartItem);
        }
        await _db.SaveChangesAsync();

        // Return update cart
        return await GetCartByUserId(userId) ?? throw new Exception("Cart not found.");
    }

    public async Task<CartResponseDto?> UpdateCartItem(int cartItemId, int userId, UpdateCartDto dto)
    {
        // Find cart item by cartItemId
        var cartItem = await _db.CartItems.FirstOrDefaultAsync((cartItem) => cartItem.CartItemId == cartItemId);
        if (cartItem == null) return null;

        // Remove the cart item if the quantity is zero
        if (dto.Quantity == 0)
        {
            _db.CartItems.Remove(cartItem);
            await _db.SaveChangesAsync();
        } else
        {            
            // Update the quantity with new value
            cartItem.Quantity = dto.Quantity;
            await _db.SaveChangesAsync();
        }
        return await GetCartByUserId(userId);
    }

    public async Task<CartResponseDto?> RemoveFromCart(int cartItemId, int userId)
    {
        var cartItem = await _db.CartItems.FirstOrDefaultAsync((cartItem) => cartItem.CartItemId == cartItemId);
        if (cartItem == null) return null;

        _db.CartItems.Remove(cartItem);
        await _db.SaveChangesAsync();

        return await GetCartByUserId(userId);
    }

    public async Task<CartResponseDto?> CleanCart(int userId)
    {
        var cart = await _db.Carts
            .Include((cart) => cart.CartItems)
            .FirstOrDefaultAsync((cart) => cart.UserId == userId);
        
        if (cart == null) return null;
        
        _db.CartItems.RemoveRange(cart.CartItems);
        await _db.SaveChangesAsync();

        return await GetCartByUserId(userId);
    }
}