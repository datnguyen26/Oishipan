using OishipanAPI.DTOs;
using OishipanAPI.Utilities;
using Microsoft.EntityFrameworkCore;
using Oishipan.Models;

namespace OishipanAPI.Services
{
    public class UserService : IUserService
    {
        private readonly OishipanContext _context;

        public UserService(OishipanContext context)
        {
            _context = context;
        }

        public async Task<UserListResponse> GetAllUsersAsync(string role = null, bool? status = null, string searchTerm = null, int page = 1, int pageSize = 20)
        {
            var query = _context.Accounts.AsQueryable();

            if (!string.IsNullOrWhiteSpace(role))
                query = query.Where(u => u.UserRole.ToString() == role);

            if (status.HasValue)
                query = query.Where(u => u.Status == status.Value);

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(u => u.FullName.Contains(searchTerm) || u.Email.Contains(searchTerm) || u.PhoneNumber.Contains(searchTerm));
            }

            var total = await query.CountAsync();
            var users = await query
                .OrderByDescending(u => u.UserId)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(u => new UserDto
                {
                    UserId = u.UserId,
                    FullName = u.FullName,
                    Email = u.Email,
                    PhoneNumber = u.PhoneNumber,
                    Role = u.UserRole.ToString(),
                    Address = u.Address,
                    Status = u.Status
                })
                .ToListAsync();

            return new UserListResponse { Total = total, Users = users };
        }

        public async Task<UserDto> GetUserByIdAsync(int userId)
        {
            var user = await _context.Accounts.FirstOrDefaultAsync(u => u.UserId == userId);
            if (user == null)
                return null;

            return new UserDto
            {
                UserId = user.UserId,
                FullName = user.FullName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Role = user.UserRole.ToString(),
                Address = user.Address,
                Status = user.Status
            };
        }

        public async Task<UserDto> UpdateUserAsync(int userId, UpdateUserRequest request)
        {
            var user = await _context.Accounts.FirstOrDefaultAsync(u => u.UserId == userId);
            if (user == null)
                return null;

            // Check if new email already exists (if changed)
            if (user.Email != request.Email && await _context.Accounts.AnyAsync(u => u.Email == request.Email))
                throw new InvalidOperationException("Email này đã được sử dụng");

            user.FullName = request.FullName;
            user.Email = request.Email;
            user.PhoneNumber = request.PhoneNumber;
            user.Address = request.Address;
            user.UserRole = Enum.Parse<Role>(request.Role);
            user.Status = request.Status;

            _context.Accounts.Update(user);
            await _context.SaveChangesAsync();

            return new UserDto
            {
                UserId = user.UserId,
                FullName = user.FullName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Role = user.UserRole.ToString(),
                Address = user.Address,
                Status = user.Status
            };
        }

        public async Task<bool> DeleteUserAsync(int userId)
        {
            var user = await _context.Accounts.FirstOrDefaultAsync(u => u.UserId == userId);
            if (user == null)
                return false;

            _context.Accounts.Remove(user);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<UserDto> ToggleUserStatusAsync(int userId)
        {
            var user = await _context.Accounts.FirstOrDefaultAsync(u => u.UserId == userId);
            if (user == null)
                return null;

            user.Status = !user.Status;
            _context.Accounts.Update(user);
            await _context.SaveChangesAsync();

            return new UserDto
            {
                UserId = user.UserId,
                FullName = user.FullName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Role = user.UserRole.ToString(),
                Address = user.Address,
                Status = user.Status
            };
        }

        public async Task<List<UserDto>> SearchUsersAsync(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return new List<UserDto>();

            var users = await _context.Accounts
                .Where(u => u.FullName.Contains(searchTerm) || u.Email.Contains(searchTerm) || u.PhoneNumber.Contains(searchTerm))
                .Take(50)
                .Select(u => new UserDto
                {
                    UserId = u.UserId,
                    FullName = u.FullName,
                    Email = u.Email,
                    PhoneNumber = u.PhoneNumber,
                    Role = u.UserRole.ToString(),
                    Address = u.Address,
                    Status = u.Status
                })
                .ToListAsync();

            return users;
        }

        public async Task<UserDto> CreateUserAsync(CreateUserRequest request)
        {
            // Check if email already exists
            if (await _context.Accounts.AnyAsync(u => u.Email == request.Email))
                throw new InvalidOperationException("Email này đã được đăng ký");

            var user = new Account
            {
                FullName = request.FullName,
                Email = request.Email,
                PhoneNumber = request.PhoneNumber,
                Password = PasswordHelper.HashPassword(request.Password),
                UserRole = Enum.Parse<Role>(request.Role),
                Address = request.Address,
                Status = true
            };

            _context.Accounts.Add(user);
            await _context.SaveChangesAsync();

            return new UserDto
            {
                UserId = user.UserId,
                FullName = user.FullName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Role = user.UserRole.ToString(),
                Address = user.Address,
                Status = user.Status
            };
        }

        public async Task<bool> ChangePasswordAsync(int userId, string newPassword)
        {
            if (string.IsNullOrWhiteSpace(newPassword) || newPassword.Length < 6)
                throw new InvalidOperationException("Mật khẩu phải có ít nhất 6 ký tự");

            var user = await _context.Accounts.FirstOrDefaultAsync(u => u.UserId == userId);
            if (user == null)
                return false;

            user.Password = PasswordHelper.HashPassword(newPassword);
            _context.Accounts.Update(user);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
