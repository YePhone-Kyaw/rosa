namespace backend.Models;

public class ProductImage
{
    public int ProductImageId { get; set; }
    public int ProductId { get; set; }
    public string ProductImageUrl { get; set; } = string.Empty;
    public int DisplayOrder { get; set; } = 0;
    public Product Product { get; set; } = null!;
}
