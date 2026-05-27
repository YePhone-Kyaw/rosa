using backend.DTOs;
using backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers;

[ApiController]
[Route("[controller]")]
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

    [HttpGet("{id}")]
    public async Task<IActionResult> GetCategoryById(int id)
    {
        var category = await _categoryService.GetCategoryById(id);
        if (category == null) return NotFound(new { message = "Category not found" });
        return Ok(category);
    }

    [HttpPost]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> CreateCategory(CreateCategoryDto dto)
    {
        var category = await _categoryService.CreateCategory(dto);
        return CreatedAtAction(nameof(GetCategoryById), new { id = category.CategoryId }, category);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> UpdateCategory(int id, UpdateCategoryDto dto)
    {
        var category = await _categoryService.UpdateCategory(id, dto);
        if (category == null) return NotFound(new { message = "Category not found" });
        return Ok(category);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> DeleteCategory(int id)
    {
        var isValid = await _categoryService.DeleteCategory(id);
        if (!isValid) return NotFound(new { message = "Category not found" });
        return Ok(new { message = "Category deleted successfully" });
    }
}