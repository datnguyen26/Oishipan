using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using OishipanMVC.Models;
using OishipanMVC.Services;

namespace OishipanMVC.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("admin")]
    [Authorize(Roles = "Admin,Staff")]
    public class DashboardController : Controller
    {
        private readonly IApiClient _apiClient;

        public DashboardController(IApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        [HttpGet("")]
        public async Task<IActionResult> Index()
        {
            var products = new List<ProductViewModel>();
            try
            {
                products = await _apiClient.GetAsync<List<ProductViewModel>>("/api/products");
            }
            catch
            {
                // Nếu API không phản hồi, vẫn hiển thị giao diện quản lý với dữ liệu mẫu.
            }

            ViewBag.ProductCount = products.Count;
            ViewBag.OrderCount = 14;
            ViewBag.CustomerCount = 22;

            return View(products);
        }
    }
}