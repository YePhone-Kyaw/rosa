using backend.DTOs;
using backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers;

[ApiController]
[Route("products")]
public class ProductController : ControllerBase
{
    private readonly ProductService _productService;
    public ProductController(ProductService productService)
    {
        _productService = productService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllProducts(
        [FromQuery] string? search = null,
        [FromQuery] int? categoryId = null,
        [FromQuery] decimal? minPrice = null,
        [FromQuery] decimal? maxPrice = null,
        [FromQuery] string? sortBy = null,
        [FromQuery] string? order = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 28)
    {
        var products = await _productService.GetAllProducts(
            search, categoryId, minPrice, maxPrice, sortBy, order, page, pageSize);
        return Ok(products);
    }

    [HttpGet("{productId}")]
    public async Task<IActionResult> GetProductById(int productId)
    {
        var product = await _productService.GetProductById(productId);
        if (product == null) return NotFound(new { message = "Product not found" });
        return Ok(product);
    }

    [HttpPost]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> CreateProduct([FromForm] CreateProductDto dto)
    {
        var product = await _productService.CreateProduct(dto);
        return CreatedAtAction(nameof(GetProductById), new { productId = product.ProductId }, product);
    }

    [HttpPut("{productId}")]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult?> UpdateProduct(int productId, [FromForm] UpdateProductDto dto)
    {
        var product = await _productService.UpdateProduct(productId, dto);
        if (product == null) return NotFound(new { message = "Product not found" });
        return Ok(product);
    }

    [HttpDelete("{productId}")]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> DeleteProduct(int productId)
    {
        var productToDelete = await _productService.DeleteProduct(productId);
        if (!productToDelete) return NotFound(new { message = "Product not found" });
        return Ok(new { message = "Product deleted successfully" });
    }

    [HttpDelete("{productId}/images/{imageId}")]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> DeleteProductImage(int productId, int imageId)
    {
        var imageToDelete = await _productService.DeleteProductImage(productId, imageId);
        if (!imageToDelete) return NotFound(new { message = "Image not found" });
        return Ok(new { message = "Image deleted" });
    }
}
