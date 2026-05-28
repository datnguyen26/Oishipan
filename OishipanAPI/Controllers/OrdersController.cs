using Microsoft.AspNetCore.Mvc;
using OishipanAPI.DTOs;
using OishipanAPI.Services;
using System.ComponentModel.DataAnnotations;

namespace OishipanAPI.Controllers
{
    /// <summary>
    /// API cho quản lý đơn hàng
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrdersController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        /// <summary>
        /// Tạo đơn hàng mới
        /// </summary>
        /// <param name="dto">Thông tin đơn hàng</param>
        /// <returns>Đơn hàng vừa tạo</returns>
        /// <response code="201">Tạo thành công</response>
        /// <response code="400">Dữ liệu không hợp lệ</response>
        [HttpPost]
        [ProducesResponseType(typeof(OrderDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var order = await _orderService.CreateOrderAsync(dto);

            if (order == null)
                return BadRequest(new { message = "Failed to create order" });

            return CreatedAtAction(nameof(GetOrderById), new { id = order.OrderId }, order);
        }

        /// <summary>
        /// Lấy chi tiết đơn hàng theo ID
        /// </summary>
        /// <param name="id">ID đơn hàng</param>
        /// <returns>Thông tin chi tiết đơn hàng</returns>
        /// <response code="200">Lấy thông tin thành công</response>
        /// <response code="404">Đơn hàng không tồn tại</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(OrderDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetOrderById(int id)
        {
            var order = await _orderService.GetOrderByIdAsync(id);

            if (order == null)
                return NotFound(new { message = "Order not found" });

            return Ok(order);
        }

        /// <summary>
        /// Lấy danh sách đơn hàng của người dùng
        /// </summary>
        /// <param name="userId">ID người dùng</param>
        /// <returns>Danh sách đơn hàng</returns>
        /// <response code="200">Lấy danh sách thành công</response>
        [HttpGet("user/{userId}/my-orders")]
        [ProducesResponseType(typeof(IEnumerable<OrderDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetMyOrders(int userId)
        {
            var orders = await _orderService.GetOrdersByUserAsync(userId);
            return Ok(orders);
        }

        /// <summary>
        /// Lấy danh sách tất cả đơn hàng
        /// </summary>
        /// <returns>Danh sách đơn hàng</returns>
        /// <response code="200">Lấy danh sách thành công</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<OrderDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllOrders()
        {
            var orders = await _orderService.GetAllOrdersAsync();
            return Ok(orders);
        }

        /// <summary>
        /// Cập nhật trạng thái đơn hàng
        /// </summary>
        /// <param name="id">ID đơn hàng</param>
        /// <param name="dto">Thông tin cập nhật trạng thái</param>
        /// <returns>Thông báo cập nhật thành công</returns>
        /// <response code="200">Cập nhật thành công</response>
        /// <response code="404">Đơn hàng không tồn tại</response>
        [HttpPut("{id}/status")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateOrderStatus(int id, [FromBody] UpdateOrderStatusDto dto)
        {
            dto.OrderId = id;
            var success = await _orderService.UpdateOrderStatusAsync(dto);

            if (!success)
                return NotFound(new { message = "Order not found" });

            return Ok(new { message = "Order status updated successfully" });
        }

        /// <summary>
        /// Hủy đơn hàng
        /// </summary>
        /// <param name="id">ID đơn hàng</param>
        /// <returns>Thông báo hủy thành công</returns>
        /// <response code="200">Hủy thành công</response>
        /// <response code="400">Không thể hủy đơn hàng này</response>
        [HttpDelete("{id}/cancel")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CancelOrder(int id)
        {
            var success = await _orderService.CancelOrderAsync(id);

            if (!success)
                return BadRequest(new { message = "Cannot cancel this order" });

            return Ok(new { message = "Order cancelled successfully" });
        }
    }
}
