using System.ComponentModel.DataAnnotations;

namespace OishipanAPI.DTOs
{
    public class UserDto
    {
        public int UserId { get; set; }

        [StringLength(100)]
        public string FullName { get; set; }

        [EmailAddress]
        public string Email { get; set; }

        [Phone]
        public string PhoneNumber { get; set; }

        public string Role { get; set; }

        public string Address { get; set; }

        public bool Status { get; set; }

        public DateTime? CreatedAt { get; set; }
    }

    public class UpdateUserRequest
    {
        [Required(ErrorMessage = "Họ và tên không được để trống")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Họ và tên phải từ 3 đến 100 ký tự")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Email không được để trống")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Số điện thoại không được để trống")]
        [RegularExpression(@"^0\d{9}$", ErrorMessage = "Số điện thoại phải bắt đầu bằng 0 và có 10 chữ số")]
        public string PhoneNumber { get; set; }

        [StringLength(200, ErrorMessage = "Địa chỉ không được vượt quá 200 ký tự")]
        public string Address { get; set; }

        [Required(ErrorMessage = "Role không được để trống")]
        [RegularExpression("^(Admin|Staff|User)$", ErrorMessage = "Role phải là Admin, Staff hoặc User")]
        public string Role { get; set; }

        public bool Status { get; set; }
    }

    public class CreateUserRequest
    {
        [Required(ErrorMessage = "Họ và tên không được để trống")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Họ và tên phải từ 3 đến 100 ký tự")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Email không được để trống")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Số điện thoại không được để trống")]
        [RegularExpression(@"^0\d{9}$", ErrorMessage = "Số điện thoại phải bắt đầu bằng 0 và có 10 chữ số")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Mật khẩu không được để trống")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Mật khẩu phải từ 6 ký tự trở lên")]
        public string Password { get; set; }

        [StringLength(200, ErrorMessage = "Địa chỉ không được vượt quá 200 ký tự")]
        public string Address { get; set; }

        [Required(ErrorMessage = "Role không được để trống")]
        [RegularExpression("^(Admin|Staff|User)$", ErrorMessage = "Role phải là Admin, Staff hoặc User")]
        public string Role { get; set; }
    }

    public class UserListResponse
    {
        public int Total { get; set; }
        public List<UserDto> Users { get; set; } = new();
    }
}
