namespace OishipanMVC.Models
{
    public class ProductViewModel
    {
        public int ProductId { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string Image { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public string Description { get; set; } = string.Empty;
        public CategoryViewModel? Category { get; set; }
        public BrandViewModel? Brand { get; set; }

        public string CategoryName => Category?.CategoryName ?? "";
        public string BrandName => Brand?.BrandName ?? "";
        public string ImageUrl => string.IsNullOrEmpty(Image) ? "https://via.placeholder.com/600x400?text=No+Image" : Image;
    }

    public class CategoryViewModel
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }

    public class BrandViewModel
    {
        public int BrandId { get; set; }
        public string BrandName { get; set; } = string.Empty;
        public string Website { get; set; } = string.Empty;
    }
}
