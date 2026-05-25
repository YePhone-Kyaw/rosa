using backend.Data;
using backend.DTOs;
using backend.Models;
using Microsoft.EntityFrameworkCore;

namespace backend.Services;

public class ProductService
{
    private readonly AppDbContext _db;
    public ProductService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<List<ProductResponseDto>> GetAllProducts()
    {
        return await _db.Products
            .Include((product) => product.Category)
            .Select((product) => new ProductResponseDto
            {
                ProductId = product.ProductId,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                Stock = product.Stock,
                ImageUrl = product.ImageUrl,
                CategoryName = product.Category.Name,
                CreatedAt = product.CreatedAt
            })
            .ToListAsync();
    }

    public async Task<ProductResponseDto?> GetProductById(int id)
    {
        return await _db.Products
            .Include((product) => product.Category)
            .Where((product) => product.ProductId == id)
            .Select((product) => new ProductResponseDto
            {
                ProductId = product.ProductId,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                Stock = product.Stock,
                ImageUrl = product.ImageUrl,
                CategoryName = product.Category.Name,
                CreatedAt = product.CreatedAt
            })
            .FirstOrDefaultAsync();
    }

    public async Task<ProductResponseDto> CreateProduct(CreateProductDto dto)
    {
        var product = new Product
        {
            Name = dto.Name,
            Description = dto.Description,
            Price = dto.Price,
            Stock = dto.Stock,
            ImageUrl = dto.ImageUrl,
            CategoryId = dto.CategoryId
        };

        _db.Products.Add(product);
        await _db.SaveChangesAsync();

        return await GetProductById(product.ProductId) ?? throw new Exception("Product not found after creation");
    }

    public async Task<ProductResponseDto?> UpdateProduct(int id, UpdateProductDto dto)
    {
        var product = await _db.Products.FindAsync(id);
        if (product == null) return null;

        if (dto.Name != null) product.Name = dto.Name;
        if (dto.Description != null) product.Description = dto.Description;
        if (dto.Price != null) product.Price = dto.Price.Value;
        if (dto.Stock != null) product.Stock = dto.Stock.Value;
        if (dto.ImageUrl != null) product.ImageUrl = dto.ImageUrl;
        if (dto.CategoryId != null) product.CategoryId = dto.CategoryId.Value;

        await _db.SaveChangesAsync();

        return await GetProductById(id);
    }

    public async Task<bool> DeleteProduct(int id)
    {
        var product = await _db.Products.FindAsync(id);

        if (product == null) return false;

        _db.Products.Remove(product);
        await _db.SaveChangesAsync();

        return true;
    }
}