namespace backend.Models;

public class Category
{
    public int CategoryId { get; set; }
    public required string Name { get; set; }
    public List<Product> Products { get; set; } = new();
}