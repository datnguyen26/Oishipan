using OishipanAPI.DTOs;

namespace OishipanAPI.Services
{
    public interface IAuthService
    {
        Task<LoginResponse> LoginAsync(LoginRequest request);
        Task<RegisterResponse> RegisterAsync(RegisterRequest request);
        Task<UserDto> GetUserByIdAsync(int userId);
        Task<bool> UpdateUserAsync(int userId, string fullName, string phoneNumber, string address);
    }

    public interface IProductService
    {
        Task<List<ProductDto>> GetAllProductsAsync();
        Task<ProductDto> GetProductByIdAsync(int productId);
        Task<List<ProductDto>> GetProductsByCategoryAsync(int categoryId);
        Task<List<ProductDto>> GetProductsByBrandAsync(int brandId);
        Task<ProductDto> CreateProductAsync(CreateProductDto dto);
        Task<ProductDto> UpdateProductAsync(UpdateProductDto dto);
        Task<bool> DeleteProductAsync(int productId);
        Task<bool> UpdateProductImageAsync(int productId, string imageUrl);
    }

    public interface IOrderService
    {
        Task<OrderDto> CreateOrderAsync(CreateOrderDto dto);
        Task<OrderDto> GetOrderByIdAsync(int orderId);
        Task<List<OrderDto>> GetOrdersByUserAsync(int userId);
        Task<List<OrderDto>> GetAllOrdersAsync();
        Task<bool> UpdateOrderStatusAsync(UpdateOrderStatusDto dto);
        Task<bool> CancelOrderAsync(int orderId);
    }

    public interface ICloudinaryService
    {
        Task<string> UploadImageAsync(IFormFile file, string folderName = "oishipan");
        Task<bool> DeleteImageAsync(string publicId);
    }

    public interface IVoucherService
    {
        Task<VoucherDto> GetVoucherByCodeAsync(string code);
        Task<List<VoucherDto>> GetAllVouchersAsync();
        Task<VoucherDto> CreateVoucherAsync(CreateVoucherDto dto);
        Task<bool> DeleteVoucherAsync(int voucherId);
        Task<bool> IsVoucherValidAsync(string code);
    }
}
