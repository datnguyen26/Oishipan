using Microsoft.AspNetCore.Mvc;
using OishipanMVC.Models;
using OishipanMVC.Services;

namespace OishipanMVC.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("admin/products")]
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
    }
}