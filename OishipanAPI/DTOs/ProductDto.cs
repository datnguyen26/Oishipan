namespace OishipanAPI.DTOs
{
    public class CreateProductDto
    {
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public int CategoryId { get; set; }
        public int BrandId { get; set; }
        public string Description { get; set; }
    }

    public class UpdateProductDto
    {
        public int ProductId { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public int CategoryId { get; set; }
        public int BrandId { get; set; }
        public string Description { get; set; }
    }

    public class ProductDto
    {
        public int ProductId { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string Image { get; set; }
        public int Quantity { get; set; }
        public int CategoryId { get; set; }
        public int BrandId { get; set; }
        public string Description { get; set; }
        public CategoryDto Category { get; set; }
        public BrandDto Brand { get; set; }
    }

    public class CategoryDto
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public string Description { get; set; }
    }

    public class CreateCategoryDto
    {
        public string CategoryName { get; set; }
        public string Description { get; set; }
    }

    public class BrandDto
    {
        public int BrandId { get; set; }
        public string BrandName { get; set; }
        public string Website { get; set; }
    }

    public class CreateBrandDto
    {
        public string BrandName { get; set; }
        public string Website { get; set; }
    }
}
