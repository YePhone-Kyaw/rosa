namespace backend.DTOs;

public class AddToCartDto
{
    public int ProductId { get; set; }
    public int Quantity { get; set; }
}

public class UpdateCartDto
{
    public int Quantity { get; set; }
}

public class CartItemResponseDto
{
    public int CartItemId { get; set; }
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string? ProductImageUrl { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal Subtotal { get; set; }
}

public class CartResponseDto
{
    public int CartId { get; set; }
    public List<CartItemResponseDto> CartItems { get; set; } = new();
    public decimal TotalAmount { get; set; }
}
