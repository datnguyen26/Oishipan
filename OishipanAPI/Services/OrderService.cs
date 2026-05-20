using OishipanAPI.DTOs;
using Microsoft.EntityFrameworkCore;
using Oishipan.Models;

namespace OishipanAPI.Services
{
    public class OrderService : IOrderService
    {
        private readonly OishipanContext _context;

        public OrderService(OishipanContext context)
        {
            _context = context;
        }

        public async Task<OrderDto> CreateOrderAsync(CreateOrderDto dto)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    decimal totalAmount = 0;

                    // Validate and calculate total amount
                    foreach (var item in dto.OrderDetails)
                    {
                        var product = await _context.Products.FindAsync(item.ProductId);
                        if (product == null || product.Quantity < item.Quantity)
                        {
                            await transaction.RollbackAsync();
                            return null;
                        }
                        totalAmount += product.Price * item.Quantity;
                    }

                    // Create order
                    var order = new Order
                    {
                        UserId = dto.UserId,
                        OrderDate = DateTime.Now,
                        TotalAmount = totalAmount,
                        Status = "Mới",
                        PaymentMethod = dto.PaymentMethod
                    };

                    _context.Orders.Add(order);
                    await _context.SaveChangesAsync();

                    // Create order details and update product quantities
                    foreach (var item in dto.OrderDetails)
                    {
                        var product = await _context.Products.FindAsync(item.ProductId);

                        var orderDetail = new OrderDetail
                        {
                            OrderId = order.OrderId,
                            ProductId = item.ProductId,
                            Price = product.Price,
                            Quantity = item.Quantity
                        };

                        product.Quantity -= item.Quantity;
                        _context.OrderDetails.Add(orderDetail);
                    }

                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();

                    return await GetOrderByIdAsync(order.OrderId);
                }
                catch
                {
                    await transaction.RollbackAsync();
                    return null;
                }
            }
        }

        public async Task<OrderDto> GetOrderByIdAsync(int orderId)
        {
            var order = await _context.Orders
                .Include(o => o.OrderDetails)
                .ThenInclude(od => od.Product)
                .ThenInclude(p => p.Category)
                .FirstOrDefaultAsync(o => o.OrderId == orderId);

            if (order == null)
                return null;

            return MapOrderToDto(order);
        }

        public async Task<List<OrderDto>> GetOrdersByUserAsync(int userId)
        {
            var orders = await _context.Orders
                .Where(o => o.UserId == userId)
                .Include(o => o.OrderDetails)
                .ThenInclude(od => od.Product)
                .ThenInclude(p => p.Category)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();

            return orders.Select(MapOrderToDto).ToList();
        }

        public async Task<List<OrderDto>> GetAllOrdersAsync()
        {
            var orders = await _context.Orders
                .Include(o => o.OrderDetails)
                .ThenInclude(od => od.Product)
                .ThenInclude(p => p.Category)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();

            return orders.Select(MapOrderToDto).ToList();
        }

        public async Task<bool> UpdateOrderStatusAsync(UpdateOrderStatusDto dto)
        {
            var order = await _context.Orders.FindAsync(dto.OrderId);

            if (order == null)
                return false;

            order.Status = dto.Status;
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> CancelOrderAsync(int orderId)
        {
            var order = await _context.Orders
                .Include(o => o.OrderDetails)
                .FirstOrDefaultAsync(o => o.OrderId == orderId);

            if (order == null || order.Status != "Mới")
                return false;

            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    // Restore product quantities
                    foreach (var detail in order.OrderDetails)
                    {
                        var product = await _context.Products.FindAsync(detail.ProductId);
                        product.Quantity += detail.Quantity;
                    }

                    order.Status = "Hủy";
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();

                    return true;
                }
                catch
                {
                    await transaction.RollbackAsync();
                    return false;
                }
            }
        }

        private OrderDto MapOrderToDto(Order order)
        {
            return new OrderDto
            {
                OrderId = order.OrderId,
                UserId = order.UserId,
                OrderDate = order.OrderDate,
                TotalAmount = order.TotalAmount,
                Status = order.Status,
                PaymentMethod = order.PaymentMethod,
                OrderDetails = order.OrderDetails.Select(od => new OrderDetailDto
                {
                    OrderDetailId = od.OrderDetailId,
                    OrderId = od.OrderId,
                    ProductId = od.ProductId,
                    Price = od.Price,
                    Quantity = od.Quantity,
                    Product = new ProductDto
                    {
                        ProductId = od.Product.ProductId,
                        Name = od.Product.Name,
                        Price = od.Product.Price,
                        Image = od.Product.Image,
                        Quantity = od.Product.Quantity,
                        CategoryId = od.Product.CategoryId,
                        BrandId = od.Product.BrandId,
                        Description = od.Product.Description
                    }
                }).ToList()
            };
        }
    }
}
