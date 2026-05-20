using OishipanAPI.DTOs;
using Microsoft.EntityFrameworkCore;
using Oishipan.Models;

namespace OishipanAPI.Services
{
    public class ProductService : IProductService
    {
        private readonly OishipanContext _context;

        public ProductService(OishipanContext context)
        {
            _context = context;
        }

        public async Task<List<ProductDto>> GetAllProductsAsync()
        {
            return await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Brand)
                .Select(p => new ProductDto
                {
                    ProductId = p.ProductId,
                    Name = p.Name,
                    Price = p.Price,
                    Image = p.Image,
                    Quantity = p.Quantity,
                    CategoryId = p.CategoryId,
                    BrandId = p.BrandId,
                    Description = p.Description,
                    Category = new CategoryDto
                    {
                        CategoryId = p.Category.CategoryId,
                        CategoryName = p.Category.CategoryName,
                        Description = p.Category.Description
                    },
                    Brand = new BrandDto
                    {
                        BrandId = p.Brand.BrandId,
                        BrandName = p.Brand.BrandName,
                        Website = p.Brand.Website
                    }
                })
                .ToListAsync();
        }

        public async Task<ProductDto> GetProductByIdAsync(int productId)
        {
            var product = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Brand)
                .FirstOrDefaultAsync(p => p.ProductId == productId);

            if (product == null)
                return null;

            return new ProductDto
            {
                ProductId = product.ProductId,
                Name = product.Name,
                Price = product.Price,
                Image = product.Image,
                Quantity = product.Quantity,
                CategoryId = product.CategoryId,
                BrandId = product.BrandId,
                Description = product.Description,
                Category = new CategoryDto
                {
                    CategoryId = product.Category.CategoryId,
                    CategoryName = product.Category.CategoryName,
                    Description = product.Category.Description
                },
                Brand = new BrandDto
                {
                    BrandId = product.Brand.BrandId,
                    BrandName = product.Brand.BrandName,
                    Website = product.Brand.Website
                }
            };
        }

        public async Task<List<ProductDto>> GetProductsByCategoryAsync(int categoryId)
        {
            return await _context.Products
                .Where(p => p.CategoryId == categoryId)
                .Include(p => p.Category)
                .Include(p => p.Brand)
                .Select(p => new ProductDto
                {
                    ProductId = p.ProductId,
                    Name = p.Name,
                    Price = p.Price,
                    Image = p.Image,
                    Quantity = p.Quantity,
                    CategoryId = p.CategoryId,
                    BrandId = p.BrandId,
                    Description = p.Description,
                    Category = new CategoryDto
                    {
                        CategoryId = p.Category.CategoryId,
                        CategoryName = p.Category.CategoryName,
                        Description = p.Category.Description
                    },
                    Brand = new BrandDto
                    {
                        BrandId = p.Brand.BrandId,
                        BrandName = p.Brand.BrandName,
                        Website = p.Brand.Website
                    }
                })
                .ToListAsync();
        }

        public async Task<List<ProductDto>> GetProductsByBrandAsync(int brandId)
        {
            return await _context.Products
                .Where(p => p.BrandId == brandId)
                .Include(p => p.Category)
                .Include(p => p.Brand)
                .Select(p => new ProductDto
                {
                    ProductId = p.ProductId,
                    Name = p.Name,
                    Price = p.Price,
                    Image = p.Image,
                    Quantity = p.Quantity,
                    CategoryId = p.CategoryId,
                    BrandId = p.BrandId,
                    Description = p.Description,
                    Category = new CategoryDto
                    {
                        CategoryId = p.Category.CategoryId,
                        CategoryName = p.Category.CategoryName,
                        Description = p.Category.Description
                    },
                    Brand = new BrandDto
                    {
                        BrandId = p.Brand.BrandId,
                        BrandName = p.Brand.BrandName,
                        Website = p.Brand.Website
                    }
                })
                .ToListAsync();
        }

        public async Task<ProductDto> CreateProductAsync(CreateProductDto dto)
        {
            var product = new Product
            {
                Name = dto.Name,
                Price = dto.Price,
                Quantity = dto.Quantity,
                CategoryId = dto.CategoryId,
                BrandId = dto.BrandId,
                Description = dto.Description
            };

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            return await GetProductByIdAsync(product.ProductId);
        }

        public async Task<ProductDto> UpdateProductAsync(UpdateProductDto dto)
        {
            var product = await _context.Products.FindAsync(dto.ProductId);

            if (product == null)
                return null;

            product.Name = dto.Name ?? product.Name;
            product.Price = dto.Price > 0 ? dto.Price : product.Price;
            product.Quantity = dto.Quantity > 0 ? dto.Quantity : product.Quantity;
            product.CategoryId = dto.CategoryId > 0 ? dto.CategoryId : product.CategoryId;
            product.BrandId = dto.BrandId > 0 ? dto.BrandId : product.BrandId;
            product.Description = dto.Description ?? product.Description;

            await _context.SaveChangesAsync();

            return await GetProductByIdAsync(product.ProductId);
        }

        public async Task<bool> DeleteProductAsync(int productId)
        {
            var product = await _context.Products.FindAsync(productId);

            if (product == null)
                return false;

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> UpdateProductImageAsync(int productId, string imageUrl)
        {
            var product = await _context.Products.FindAsync(productId);

            if (product == null)
                return false;

            product.Image = imageUrl;
            await _context.SaveChangesAsync();

            return true;
        }
    }
}
