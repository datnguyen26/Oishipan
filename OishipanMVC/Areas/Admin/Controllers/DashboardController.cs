using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using OishipanMVC.Areas.Admin.Models;
using OishipanMVC.Models;
using OishipanMVC.Services;
using System.Linq;

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
            var orders = new List<AdminOrderDto>();
            var errors = new List<string>();

            try
            {
                products = await _apiClient.GetAsync<List<ProductViewModel>>("/api/products");
            }
            catch
            {
                errors.Add("Không thể tải sản phẩm từ cơ sở dữ liệu.");
            }

            try
            {
                orders = await _apiClient.GetAsync<List<AdminOrderDto>>("/api/orders");
            }
            catch
            {
                errors.Add("Không thể tải đơn hàng từ cơ sở dữ liệu.");
            }

            ViewBag.Error = errors.Any() ? string.Join(" ", errors) : null;
            ViewBag.ProductCount = products.Count;
            ViewBag.OrderCount = orders.Count;
            ViewBag.CustomerCount = orders.Select(o => o.UserId).Distinct().Count();
            ViewBag.TotalRevenue = orders.Sum(o => o.TotalAmount);
            ViewBag.RecentOrders = orders.OrderByDescending(o => o.OrderDate).Take(5).ToList();

            return View(products);
        }
    }
}