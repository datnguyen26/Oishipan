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
                    VariantsJson = p.VariantsJson,
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
                        Description = p.Brand.Description
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
                    VariantsJson = product.VariantsJson,
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
                    Description = product.Brand.Description
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
                    VariantsJson = p.VariantsJson,
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
                        Description = p.Brand.Description
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
                    VariantsJson = p.VariantsJson,
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
                        Description = p.Brand.Description
                    }
                })
                .ToListAsync();
        }

        public async Task<ProductDto> CreateProductAsync(CreateProductDto dto)
        {
            // Validate required fields
            if (dto == null)
                return null;

            if (string.IsNullOrWhiteSpace(dto.Name))
                return null;

            if (dto.Price <= 0)
                return null;

            if (dto.Quantity < 0)
                return null;

            if (dto.CategoryId <= 0)
                return null;

            if (dto.BrandId <= 0)
                return null;

            // Verify that Category and Brand exist
            var categoryExists = await _context.Categories.AnyAsync(c => c.CategoryId == dto.CategoryId);
            if (!categoryExists)
                return null;

            var brandExists = await _context.Brands.AnyAsync(b => b.BrandId == dto.BrandId);
            if (!brandExists)
                return null;

            var product = new Product
            {
                Name = dto.Name.Trim(),
                Price = dto.Price,
                Quantity = dto.Quantity,
                CategoryId = dto.CategoryId,
                BrandId = dto.BrandId,
                Description = string.IsNullOrWhiteSpace(dto.Description) ? string.Empty : dto.Description.Trim(),
                VariantsJson = string.IsNullOrWhiteSpace(dto.VariantsJson) ? string.Empty : dto.VariantsJson,
                Image = string.Empty
            };

            _context.Products.Add(product);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                // Log the exception details for debugging
                System.Diagnostics.Debug.WriteLine($"DbUpdateException: {ex.InnerException?.Message}");
                return null;
            }
            catch (Exception ex)
            {
                // Log any other exception
                System.Diagnostics.Debug.WriteLine($"Exception: {ex.Message}");
                return null;
            }

            return await GetProductByIdAsync(product.ProductId);
        }

        public async Task<ProductDto> UpdateProductAsync(UpdateProductDto dto)
        {
            var product = await _context.Products.FindAsync(dto.ProductId);

            if (product == null)
                return null;

            // Update only non-null/non-zero fields
            if (!string.IsNullOrWhiteSpace(dto.Name))
                product.Name = dto.Name.Trim();

            if (dto.Price > 0)
                product.Price = dto.Price;

            if (dto.Quantity >= 0)
                product.Quantity = dto.Quantity;

            if (dto.CategoryId > 0)
            {
                var categoryExists = await _context.Categories.AnyAsync(c => c.CategoryId == dto.CategoryId);
                if (!categoryExists)
                    return null;
                product.CategoryId = dto.CategoryId;
            }

            if (dto.BrandId > 0)
            {
                var brandExists = await _context.Brands.AnyAsync(b => b.BrandId == dto.BrandId);
                if (!brandExists)
                    return null;
                product.BrandId = dto.BrandId;
            }

            if (!string.IsNullOrWhiteSpace(dto.Description))
                product.Description = dto.Description.Trim();

            if (!string.IsNullOrWhiteSpace(dto.VariantsJson))
                product.VariantsJson = dto.VariantsJson;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                System.Diagnostics.Debug.WriteLine($"DbUpdateException: {ex.InnerException?.Message}");
                return null;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Exception: {ex.Message}");
                return null;
            }

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
