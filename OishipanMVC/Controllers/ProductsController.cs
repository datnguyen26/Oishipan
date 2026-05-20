using Microsoft.AspNetCore.Mvc;
using OishipanMVC.Models;
using OishipanMVC.Services;

namespace OishipanMVC.Controllers
{
    [Route("san-pham")]
    public class ProductsController : Controller
    {
        private readonly IApiClient _apiClient;

        public ProductsController(IApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        [HttpGet("")]
        public async Task<IActionResult> Index()
        {
            try
            {
                var products = await _apiClient.GetAsync<List<ProductViewModel>>("/api/products");
                return View(products);
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Không thể tải sản phẩm: " + ex.Message;
                return View(new List<ProductViewModel>());
            }
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var product = await _apiClient.GetAsync<ProductViewModel>($"/api/products/{id}");
                return View(product);
            }
            catch (Exception)
            {
                TempData["Error"] = "Sản phẩm không tồn tại.";
                return RedirectToAction("Index");
            }
        }

        [HttpGet("danh-muc/{categoryId:int}")]
        public async Task<IActionResult> Category(int categoryId)
        {
            try
            {
                var products = await _apiClient.GetAsync<List<ProductViewModel>>($"/api/products/category/{categoryId}");
                ViewBag.SectionTitle = $"Danh mục: {categoryId}";
                return View("Index", products);
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Không thể tải sản phẩm: " + ex.Message;
                return View("Index", new List<ProductViewModel>());
            }
        }

        [HttpGet("thuong-hieu/{brandId:int}")]
        public async Task<IActionResult> Brand(int brandId)
        {
            try
            {
                var products = await _apiClient.GetAsync<List<ProductViewModel>>($"/api/products/brand/{brandId}");
                ViewBag.SectionTitle = $"Thương hiệu: {brandId}";
                return View("Index", products);
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Không thể tải sản phẩm: " + ex.Message;
                return View("Index", new List<ProductViewModel>());
            }
        }
    }
}
