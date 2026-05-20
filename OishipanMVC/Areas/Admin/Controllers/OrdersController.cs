using Microsoft.AspNetCore.Mvc;
using OishipanMVC.Areas.Admin.Models;

namespace OishipanMVC.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("admin/orders")]
    public class OrdersController : Controller
    {
        [HttpGet("")]
        public IActionResult Index()
        {
            var orders = new List<AdminOrderViewModel>
            {
                new AdminOrderViewModel { OrderId = 1023, CustomerName = "Nguyễn Văn A", TotalAmount = 850000, Status = "Đang xử lý", CreatedAt = DateTime.Now.AddDays(-1) },
                new AdminOrderViewModel { OrderId = 1024, CustomerName = "Trần Thị B", TotalAmount = 950000, Status = "Đã giao", CreatedAt = DateTime.Now.AddDays(-2) },
                new AdminOrderViewModel { OrderId = 1025, CustomerName = "Lê Văn C", TotalAmount = 780000, Status = "Chờ thanh toán", CreatedAt = DateTime.Now.AddHours(-5) },
            };

            return View(orders);
        }
    }
}