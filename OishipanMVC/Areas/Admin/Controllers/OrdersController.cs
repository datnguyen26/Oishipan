using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using OishipanMVC.Areas.Admin.Models;
using OishipanMVC.Services;
using System.Linq;

namespace OishipanMVC.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("admin/orders")]
    public class OrdersController : Controller
    {
        private readonly IApiClient _apiClient;

        public OrdersController(IApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        [HttpGet("")]
        public async Task<IActionResult> Index()
        {
            var orders = new List<AdminOrderViewModel>();
            try
            {
                var apiOrders = await _apiClient.GetAsync<List<AdminOrderDto>>("/api/orders");
                if (apiOrders != null)
                {
                    orders = apiOrders.Select(o => new AdminOrderViewModel
                    {
                        OrderId = o.OrderId,
                        CustomerName = $"Khách #{o.UserId}",
                        TotalAmount = o.TotalAmount,
                        Status = string.IsNullOrEmpty(o.Status) ? "Chờ xử lý" : o.Status,
                        CreatedAt = o.OrderDate
                    }).OrderByDescending(o => o.CreatedAt).ToList();
                }
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Không thể tải đơn hàng từ cơ sở dữ liệu: " + ex.Message;
            }

            return View(orders);
        }
    }
}