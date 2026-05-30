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

    [HttpGet("{id}")]
    public async Task<IActionResult> GetProductById(int id)
    {
        var product = await _productService.GetProductById(id);
        if (product == null) return NotFound(new { message = "Product not found" });
        return Ok(product);
    }

    [HttpPost]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> CreateProduct(CreateProductDto dto)
    {
        var product = await _productService.CreateProduct(dto);
        return CreatedAtAction(nameof(GetProductById), new { id = product.ProductId }, product);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult?> UpdateProduct(int id, UpdateProductDto dto)
    {
        var product = await _productService.UpdateProduct(id, dto);
        if (product == null) return NotFound(new { message = "Product not found" });
        return Ok(product);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> DeleteProduct(int id)
    {
        var isValid = await _productService.DeleteProduct(id);
        if (!isValid) return NotFound(new { message = "Product not found" });
        return Ok(new { message = "Product deleted successfully" });
    }
}
