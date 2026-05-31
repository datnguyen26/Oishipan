using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace OishipanAPI.DTOs
{
    public class CreateProductDto
    {
        [Required(ErrorMessage = "Tên sản phẩm không được để trống")]
        [StringLength(200, ErrorMessage = "Tên sản phẩm không được vượt quá 200 ký tự")]
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [Range(0.01, double.MaxValue, ErrorMessage = "Giá sản phẩm phải lớn hơn 0")]
        [JsonPropertyName("price")]
        public decimal Price { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Số lượng sản phẩm phải là số nguyên dương")]
        [JsonPropertyName("quantity")]
        public int Quantity { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Danh mục không hợp lệ")]
        [JsonPropertyName("categoryId")]
        public int CategoryId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Thương hiệu không hợp lệ")]
        [JsonPropertyName("brandId")]
        public int BrandId { get; set; }

        [StringLength(1000, ErrorMessage = "Mô tả không được vượt quá 1000 ký tự")]
        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("variantsJson")]
        public string VariantsJson { get; set; }
    }

    public class UpdateProductDto
    {
        [JsonPropertyName("productId")]
        public int ProductId { get; set; }

        [StringLength(200, ErrorMessage = "Tên sản phẩm không được vượt quá 200 ký tự")]
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [Range(0.01, double.MaxValue, ErrorMessage = "Giá sản phẩm phải lớn hơn 0")]
        [JsonPropertyName("price")]
        public decimal Price { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Số lượng sản phẩm phải là số nguyên dương")]
        [JsonPropertyName("quantity")]
        public int Quantity { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Danh mục không hợp lệ")]
        [JsonPropertyName("categoryId")]
        public int CategoryId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Thương hiệu không hợp lệ")]
        [JsonPropertyName("brandId")]
        public int BrandId { get; set; }

        [StringLength(1000, ErrorMessage = "Mô tả không được vượt quá 1000 ký tự")]
        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("variantsJson")]
        public string VariantsJson { get; set; }
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
        public string VariantsJson { get; set; }
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

    public class UpdateCategoryDto
    {
        public string CategoryName { get; set; }
        public string Description { get; set; }
    }

    public class BrandDto
    {
        public int BrandId { get; set; }
        public string BrandName { get; set; }
        public string Description { get; set; }
    }

    public class CreateBrandDto
    {
        public string BrandName { get; set; }
        public string Description { get; set; }
    }
}
