using backend.DTOs;
using backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers;

[ApiController]
[Route("[controller]")]
public class CartController : ControllerBase
{
    private readonly CartService _cartService;
    public CartController(CartService cartService)
    {
        _cartService = cartService;
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetCartByUserId()
    {
        var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);
        var cart = await _cartService.GetCartByUserId(userId);
        return Ok(cart);
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> AddToCart(AddToCartDto dto)
    {
        var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);
        var cart = await _cartService.AddToCart(userId, dto);
        return CreatedAtAction(nameof(GetCartByUserId), new { id = cart.CartId }, cart);
    }

    [HttpPut("{cartItemId}")]
    [Authorize]
    public async Task<IActionResult> UpdateCartItem(int cartItemId, UpdateCartDto dto)
    {
        var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);
        var cart = await _cartService.UpdateCartItem(cartItemId, userId, dto);
        if (cart == null) return NotFound(new { message = "Cart item not found" });
        return Ok(cart);
    }

    [HttpDelete("{cartItemId}")]
    [Authorize]
    public async Task<IActionResult> RemoveFromCart(int cartItemId)
    {
        var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);
        var cart = await _cartService.RemoveFromCart(cartItemId, userId);
        if (cart == null) return NotFound(new { message = "Cart item not found" });
        return Ok(cart);
    }

    [HttpDelete]
    [Authorize]
    public async Task<IActionResult> ClearCart()
    {
        var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);
        var cart = await _cartService.CleanCart(userId);
        return Ok(new { message = "Cart cleared successfully" });
    }
}
