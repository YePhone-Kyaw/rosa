using backend.Models;

namespace backend.DTOs;

public class CreateCategoryDto
{
    public required string CategoryName { get; set; }
}

public class UpdateCategoryDto
{
    public string CategoryName { get; set; } = string.Empty;
}

public class CategoryResponseDto
{
    public int CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public int ProductCount { get; set; }
}
