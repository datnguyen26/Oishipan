using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Oishipan.Models;
using OishipanAPI.DTOs;

namespace OishipanAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class BrandsController : ControllerBase
    {
        private readonly OishipanContext _context;

        public BrandsController(OishipanContext context)
        {
            _context = context;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<BrandDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllBrands()
        {
            var brands = await _context.Brands
                .OrderBy(b => b.BrandName)
                .Select(b => new BrandDto
                {
                    BrandId = b.BrandId,
                    BrandName = b.BrandName,
                    Description = b.Description ?? string.Empty
                })
                .ToListAsync();

            return Ok(brands);
        }

        [Authorize(Roles = "Admin,Staff")]
        [HttpPost]
        [ProducesResponseType(typeof(BrandDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateBrand([FromBody] CreateBrandDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

                var brand = new Brand
                {
                    BrandName = dto.BrandName,
                    Description = dto.Description
                };

            _context.Brands.Add(brand);
            await _context.SaveChangesAsync();

            var result = new BrandDto
            {
                BrandId = brand.BrandId,
                BrandName = brand.BrandName,
                Description = brand.Description ?? string.Empty
            };

            return CreatedAtAction(nameof(GetAllBrands), new { id = brand.BrandId }, result);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(BrandDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetBrandById(int id)
        {
            var brand = await _context.Brands
                .Where(b => b.BrandId == id)
                .Select(b => new BrandDto
                {
                    BrandId = b.BrandId,
                    BrandName = b.BrandName,
                    Description = b.Description ?? string.Empty
                })
                .FirstOrDefaultAsync();

            if (brand == null)
                return NotFound(new { message = "Brand not found" });

            return Ok(brand);
        }

        [Authorize(Roles = "Admin,Staff")]
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(BrandDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateBrand(int id, [FromBody] CreateBrandDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var brand = await _context.Brands.FindAsync(id);
            if (brand == null)
                return NotFound(new { message = "Brand not found" });

            brand.BrandName = dto.BrandName ?? brand.BrandName;
            brand.Description = dto.Description;

            await _context.SaveChangesAsync();

            var result = new BrandDto
            {
                BrandId = brand.BrandId,
                BrandName = brand.BrandName,
                Description = brand.Description ?? string.Empty
            };

            return Ok(result);
        }

        [Authorize(Roles = "Admin,Staff")]
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> DeleteBrand(int id)
        {
            var brand = await _context.Brands.Include(b => b.Products).FirstOrDefaultAsync(b => b.BrandId == id);
            if (brand == null)
                return NotFound(new { message = "Brand not found" });

            if (brand.Products.Any())
                return BadRequest(new { message = "Cannot delete brand that has products." });

            _context.Brands.Remove(brand);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Brand deleted successfully" });
        }
    }
}