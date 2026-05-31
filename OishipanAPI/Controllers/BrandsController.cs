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
    }
}