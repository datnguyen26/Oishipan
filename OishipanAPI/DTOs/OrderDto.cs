namespace OishipanAPI.DTOs
{
    public class CreateOrderDto
    {
        public int UserId { get; set; }
        public List<OrderDetailItemDto> OrderDetails { get; set; }
        public string PaymentMethod { get; set; }
    }

    public class OrderDetailItemDto
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }

    public class OrderDto
    {
        public int OrderId { get; set; }
        public int UserId { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; }
        public string PaymentMethod { get; set; }
        public List<OrderDetailDto> OrderDetails { get; set; }
    }

    public class OrderDetailDto
    {
        public int OrderDetailId { get; set; }
        public int OrderId { get; set; }
        public int ProductId { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public ProductDto Product { get; set; }
    }

    public class UpdateOrderStatusDto
    {
        public int OrderId { get; set; }
        public string Status { get; set; }
    }

    public class PaymentDto
    {
        public int PaymentId { get; set; }
        public int OrderId { get; set; }
        public string Vnp_TransactionNo { get; set; }
        public string Vnp_ResponseCode { get; set; }
        public DateTime PaymentDate { get; set; }
    }

    public class VoucherDto
    {
        public int VoucherId { get; set; }
        public string Code { get; set; }
        public decimal DiscountValue { get; set; }
        public DateTime ExpiryDate { get; set; }
    }

    public class CreateVoucherDto
    {
        public string Code { get; set; }
        public decimal DiscountValue { get; set; }
        public DateTime ExpiryDate { get; set; }
    }
}
