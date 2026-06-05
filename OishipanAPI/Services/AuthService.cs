using OishipanAPI.DTOs;
using Microsoft.EntityFrameworkCore;
using Oishipan.Models;
using OishipanAPI.Utilities;

namespace OishipanAPI.Services
{
    public class AuthService : IAuthService
    {
        private readonly OishipanContext _context;
        private readonly JwtTokenGenerator _jwtGenerator;
        private readonly ICloudinaryService _cloudinaryService;

        public AuthService(OishipanContext context, JwtTokenGenerator jwtGenerator, ICloudinaryService cloudinaryService = null)
        {
            _context = context;
            _jwtGenerator = jwtGenerator;
            _cloudinaryService = cloudinaryService;
        }

        public async Task<LoginResponse> LoginAsync(LoginRequest request)
        {
            if (string.IsNullOrWhiteSpace(request?.Email) || string.IsNullOrWhiteSpace(request?.Password))
            {
                return new LoginResponse
                {
                    Success = false,
                    Message = "Email và mật khẩu không được để trống"
                };
            }

            var normalizedEmail = request.Email.Trim().ToLowerInvariant();
            var user = await _context.Accounts.FirstOrDefaultAsync(a => a.Email.ToLower() == normalizedEmail);

            if (user == null || !user.Status)
            {
                return new LoginResponse
                {
                    Success = false,
                    Message = "Email hoặc mật khẩu không chính xác hoặc tài khoản bị khóa"
                };
            }

            var verified = PasswordHelper.VerifyPassword(request.Password, user.Password);

            if (!verified)
            {
                try
                {
                    var prefix = user.Password != null && user.Password.Length >= 4 ? user.Password.Substring(0, 4) : user.Password;
                    Console.WriteLine($"[Auth] Password verification failed for '{user.Email}'. HashPrefix={prefix}, HashLength={(user.Password?.Length ?? 0)}");
                }
                catch { }

                return new LoginResponse
                {
                    Success = false,
                    Message = "Email hoặc mật khẩu không chính xác hoặc tài khoản bị khóa"
                };
            }

            // If password verified but stored hash is a legacy variant ($2y$/$2x$), re-hash with the current standard and save
            try
            {
                if (!string.IsNullOrEmpty(user.Password) && (user.Password.StartsWith("$2y$") || user.Password.StartsWith("$2x$")))
                {
                    user.Password = PasswordHelper.HashPassword(request.Password);
                    _context.Accounts.Update(user);
                    await _context.SaveChangesAsync();
                    Console.WriteLine($"[Auth] Re-hashed password for '{user.Email}' with current bcrypt variant.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Auth] Failed to re-hash password for '{user.Email}': {ex.Message}");
            }

            return new LoginResponse
            {
                Success = true,
                Message = "Đăng nhập thành công",
                User = new UserDto
                {
                    UserId = user.UserId,
                    FullName = user.FullName,
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber,
                    Role = user.UserRole.ToString(),
                    Address = user.Address,
                    Status = user.Status
                }
                ,
                Token = _jwtGenerator.GenerateToken(user.UserId, user.Email, user.UserRole.ToString())
            };
        }

        public async Task<RegisterResponse> RegisterAsync(RegisterRequest request)
        {
            // Validate required fields
            if (string.IsNullOrWhiteSpace(request?.FullName) || string.IsNullOrWhiteSpace(request?.Email) 
                || string.IsNullOrWhiteSpace(request?.PhoneNumber) || string.IsNullOrWhiteSpace(request?.Password))
            {
                return new RegisterResponse
                {
                    Success = false,
                    Message = "Vui lòng điền đầy đủ các trường bắt buộc"
                };
            }

            // Check if email already exists
            if (await _context.Accounts.AnyAsync(a => a.Email == request.Email))
            {
                return new RegisterResponse
                {
                    Success = false,
                    Message = "Email này đã được đăng ký"
                };
            }

            // Check if phone number already exists
            if (await _context.Accounts.AnyAsync(a => a.PhoneNumber == request.PhoneNumber))
            {
                return new RegisterResponse
                {
                    Success = false,
                    Message = "Số điện thoại này đã được đăng ký"
                };
            }

            var account = new Account
            {
                FullName = request.FullName,
                Email = request.Email,
                PhoneNumber = request.PhoneNumber,
                Password = PasswordHelper.HashPassword(request.Password),
                UserRole = Role.User,
                Address = request.Address ?? string.Empty,
                Status = true
            };

            _context.Accounts.Add(account);
            await _context.SaveChangesAsync();

            return new RegisterResponse
            {
                Success = true,
                Message = "Đăng ký thành công. Vui lòng đăng nhập"
            };
        }

        public async Task<UserDto> GetUserByIdAsync(int userId)
        {
            if (userId <= 0)
                return null;

            var user = await _context.Accounts.FirstOrDefaultAsync(u => u.UserId == userId && u.Status);

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

        public async Task<bool> UpdateUserAsync(int userId, string fullName, string phoneNumber, string address)
        {
            var user = await _context.Accounts.FindAsync(userId);

            if (user == null)
                return false;

            user.FullName = fullName ?? user.FullName;
            user.PhoneNumber = phoneNumber ?? user.PhoneNumber;
            user.Address = address ?? user.Address;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UploadProfileImageAsync(int userId, IFormFile file)
        {
            var user = await _context.Accounts.FindAsync(userId);

            if (user == null)
                return false;

            if (_cloudinaryService == null)
                throw new InvalidOperationException("Cloudinary service not configured");

            try
            {
                var imageUrl = await _cloudinaryService.UploadImageAsync(file, "oishipan/profiles");

                if (imageUrl == null)
                    throw new Exception("Failed to upload image to Cloudinary");

                user.Image = imageUrl;
                _context.Accounts.Update(user);
                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error uploading profile image: {ex.Message}", ex);
            }
        }
    }
}
