using backend.Data;
using backend.DTOs;
using backend.Models;
using Microsoft.EntityFrameworkCore;

namespace backend.Services;

public class ProductService
{
    private readonly AppDbContext _db;
    private readonly IFileStorageService _fileStorage;
    public ProductService(AppDbContext db, IFileStorageService fileStorage)
    {
        _db = db;
        _fileStorage = fileStorage;
    }

    public async Task<ProductListResponseDto> GetAllProducts(
        string? search = null,
        int? categoryId = null,
        decimal? minPrice = null,
        decimal? maxPrice = null,
        string? sortBy = null,
        string? order = null,
        int page = 1,
        int pageSize = 28)
    {
        var query = _db.Products
            .Include((product) => product.Category)
            .Include((product) => product.Images)
            .AsQueryable();

        if (!string.IsNullOrEmpty(search))
            query = query.Where((product) => product.ProductName.Contains(search) ||
                product.Description.Contains(search));

        if (categoryId.HasValue)
            query = query.Where((product) => product.CategoryId == categoryId.Value);

        if (minPrice.HasValue)
            query = query.Where((product) => product.Price >= minPrice.Value);

        if (maxPrice.HasValue)
            query = query.Where((product) => product.Price <= maxPrice.Value);

        query = ApplySorting(query, sortBy, order);

        var totalItems = await query.CountAsync();
        var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

        var products = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select((product) => new ProductResponseDto
            {
                ProductId = product.ProductId,
                ProductName = product.ProductName,
                Description = product.Description,
                Price = product.Price,
                Stock = product.Stock,
                CategoryName = product.Category.CategoryName,
                CreatedAt = product.CreatedAt,
                Images = product.Images.OrderBy((image) => image.DisplayOrder)
                    .Select((image) => new ProductImageDto
                    {
                        ProductImageId = image.ProductImageId,
                        ProductImageUrl = image.ProductImageUrl,
                        DisplayOrder = image.DisplayOrder
                    }).ToList()
            })
            .ToListAsync();

        return new ProductListResponseDto
        {
            Products = products,
            Page = page,
            PageSize = pageSize,
            TotalItems = totalItems,
            TotalPages = totalPages
        };
    }

    public async Task<ProductResponseDto?> GetProductById(int id)
    {
        return await _db.Products
            .Include((product) => product.Category)
            .Where((product) => product.ProductId == id)
            .Select((product) => new ProductResponseDto
            {
                ProductId = product.ProductId,
                ProductName = product.ProductName,
                Description = product.Description,
                Price = product.Price,
                Stock = product.Stock,
                CategoryName = product.Category.CategoryName,
                CreatedAt = product.CreatedAt,
                Images = product.Images.OrderBy((image) => image.DisplayOrder)
                    .Select((image) => new ProductImageDto
                    {
                        ProductImageId = image.ProductImageId,
                        ProductImageUrl = image.ProductImageUrl,
                        DisplayOrder = image.DisplayOrder
                    }).ToList()
            })
            .FirstOrDefaultAsync();
    }

    public async Task<ProductResponseDto> CreateProduct(CreateProductDto dto)
    {
        var product = new Product
        {
            ProductName = dto.ProductName,
            Description = dto.Description,
            Price = dto.Price,
            Stock = dto.Stock,
            CategoryId = dto.CategoryId
        };

        _db.Products.Add(product);
        await _db.SaveChangesAsync();

        if (dto.Images != null)
        {
            for (int i = 0; i < dto.Images.Count; i++)
            {
                var url = await _fileStorage.UploadFileAsync(dto.Images[i], "products");
                _db.ProductImages.Add(new ProductImage
                {
                    ProductId = product.ProductId,
                    ProductImageUrl = url,
                    DisplayOrder = i
                });
            }
            await _db.SaveChangesAsync();
        }

        return await GetProductById(product.ProductId) ?? throw new Exception("Product not found after creation");
    }

    public async Task<ProductResponseDto?> UpdateProduct(int id, UpdateProductDto dto)
    {
        var product = await _db.Products
            .Include((product) => product.Images)
            .FirstOrDefaultAsync((product) => product.ProductId == id);
        if (product == null) return null;

        if (dto.ProductName != null) product.ProductName = dto.ProductName;
        if (dto.Description != null) product.Description = dto.Description;
        if (dto.Price != null) product.Price = dto.Price.Value;
        if (dto.Stock != null) product.Stock = dto.Stock.Value;
        if (dto.CategoryId != null) product.CategoryId = dto.CategoryId.Value;

        if (dto.Images != null)
        {
            var nextImageOrder = product.Images.Count > 0
                ? product.Images.Max((image) => image.DisplayOrder) + 1 : 0;
            
            for (int i = 0; i < dto.Images.Count; i++)
            {
                var url = await _fileStorage.UploadFileAsync(dto.Images[i], "products");
                _db.ProductImages.Add(new ProductImage
                {
                    ProductId = product.ProductId,
                    ProductImageUrl = url,
                    DisplayOrder = nextImageOrder + i
                });
            }
        }

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

    public async Task<bool> DeleteProductImage(int productId, int imageId)
    {
        var image = await _db.ProductImages.FindAsync(imageId);
        if (image == null || image.ProductId != productId) return false;

        await _fileStorage.DeleteFileAsync(image.ProductImageUrl);
        _db.ProductImages.Remove(image);
        await _db.SaveChangesAsync();

        return true;
    }
    private IQueryable<Product> ApplySorting(
        IQueryable<Product> query,
        string? sortBy,
        string? order)
    {
        switch (sortBy)
        {
            case "price":
                return order == "desc" ?
                    query.OrderByDescending((product) => product.Price) :
                    query.OrderBy((product) => product.Price);
            case "name":
                return order == "desc" ?
                    query.OrderByDescending((product) => product.ProductName) :
                    query.OrderBy((product) => product.ProductName);
            default:
                return query.OrderByDescending((product) => product.CreatedAt);
        }
    }
}
