namespace OishipanMVC.Models
{
    public class UserViewModel
    {
        public int UserId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public bool Status { get; set; }
        public DateTime? CreatedAt { get; set; }
    }

    public class UserListResponse
    {
        public int Total { get; set; }
        public List<UserViewModel> Users { get; set; } = new();
    }
}
