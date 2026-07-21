using backend.DTOs;
using backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers;

[ApiController]
[Route("categories")]
public class CategoryController : ControllerBase
{
    private readonly CategoryService _categoryService;
    public CategoryController(CategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllCategories()
    {
        var categories = await _categoryService.GetAllCategories();
        return Ok(categories);
    }

    [HttpGet("{categoryId}")]
    public async Task<IActionResult> GetCategoryById(int categoryId)
    {
        var category = await _categoryService.GetCategoryById(categoryId);
        if (category == null) return NotFound(new { message = "Category not found" });
        return Ok(category);
    }

    [HttpPost]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> CreateCategory([FromForm] CreateCategoryDto dto)
    {
        var category = await _categoryService.CreateCategory(dto);
        return CreatedAtAction(nameof(GetCategoryById), new { categoryId = category.CategoryId }, category);
    }

    [HttpPut("{categoryId}")]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> UpdateCategory(int categoryId, [FromForm] UpdateCategoryDto dto)
    {
        var category = await _categoryService.UpdateCategory(categoryId, dto);
        if (category == null) return NotFound(new { message = "Category not found" });
        return Ok(category);
    }

    [HttpDelete("{categoryId}")]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> DeleteCategory(int categoryId)
    {
        var categoryToDelete = await _categoryService.DeleteCategory(categoryId);
        if (!categoryToDelete) return NotFound(new { message = "Category not found" });
        return Ok(new { message = "Category deleted successfully" });
    }
}
