using backend.Data;
using backend.DTOs;
using backend.Models;
using Microsoft.EntityFrameworkCore;

namespace backend.Services;

public class CategoryService
{
    private readonly AppDbContext _db;
    public CategoryService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<List<CategoryResponseDto>> GetAllCategories()
    {
        return await _db.Categories
            .Select((category) => new CategoryResponseDto
            {
                CategoryId = category.CategoryId,
                Name = category.Name,
                ProductCount = category.Products.Count
            })
            .ToListAsync();
    }

    public async Task<CategoryResponseDto?> GetCategoryById(int id)
    {
        return await _db.Categories
            .Where((category) => category.CategoryId == id)
            .Select((category) => new CategoryResponseDto
            {
                CategoryId = category.CategoryId,
                Name = category.Name,
                ProductCount = category.Products.Count
            })
            .FirstOrDefaultAsync();
    }

    public async Task<CategoryResponseDto> CreateCategory(CreateCategoryDto dto)
    {
        var category = new Category
        {
            Name = dto.Name
        };

        _db.Categories.Add(category);
        await _db.SaveChangesAsync();
        
        return await GetCategoryById(category.CategoryId) ?? throw new Exception("Category not found after creation");
    }

    public async Task<CategoryResponseDto?> UpdateCategory(int id, UpdateCategoryDto dto)
    {
        var category = await _db.Categories.FindAsync(id);
        if (category == null) return null;

        if (dto.Name != null) category.Name = dto.Name;

        await _db.SaveChangesAsync();
        
        return await GetCategoryById(id);
    }

    public async Task<bool> DeleteCategory(int id)
    {
        var category = await _db.Categories.FindAsync(id);
        
        if (category == null) return false;

        _db.Categories.Remove(category);
        await _db.SaveChangesAsync();

        return true; 
    }
}
