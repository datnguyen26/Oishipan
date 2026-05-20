using OishipanAPI.DTOs;
using OishipanAPI.Utilities;
using Microsoft.EntityFrameworkCore;
using Oishipan.Models;

namespace OishipanAPI.Services
{
    public class AuthService : IAuthService
    {
        private readonly OishipanContext _context;
        private readonly JwtTokenGenerator _tokenGenerator;

        public AuthService(OishipanContext context, JwtTokenGenerator tokenGenerator)
        {
            _context = context;
            _tokenGenerator = tokenGenerator;
        }

        public async Task<LoginResponse> LoginAsync(LoginRequest request)
        {
            var user = await _context.Accounts.FirstOrDefaultAsync(a => a.Email == request.Email);

            if (user == null || !user.Status || !PasswordHelper.VerifyPassword(request.Password, user.Password))
            {
                return new LoginResponse
                {
                    Success = false,
                    Message = "Invalid email or password"
                };
            }

            var token = _tokenGenerator.GenerateToken(user.UserId, user.Email, user.Role);

            return new LoginResponse
            {
                Success = true,
                Message = "Login successful",
                Token = token,
                User = new UserDto
                {
                    UserId = user.UserId,
                    FullName = user.FullName,
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber,
                    Role = user.Role,
                    Address = user.Address,
                    Status = user.Status
                }
            };
        }

        public async Task<RegisterResponse> RegisterAsync(RegisterRequest request)
        {
            // Check if email already exists
            if (await _context.Accounts.AnyAsync(a => a.Email == request.Email))
            {
                return new RegisterResponse
                {
                    Success = false,
                    Message = "Email already exists"
                };
            }

            // Check if phone number already exists
            if (await _context.Accounts.AnyAsync(a => a.PhoneNumber == request.PhoneNumber))
            {
                return new RegisterResponse
                {
                    Success = false,
                    Message = "Phone number already exists"
                };
            }

            var account = new Account
            {
                FullName = request.FullName,
                Email = request.Email,
                PhoneNumber = request.PhoneNumber,
                Password = PasswordHelper.HashPassword(request.Password),
                Role = "User",
                Address = request.Address,
                Status = true
            };

            _context.Accounts.Add(account);
            await _context.SaveChangesAsync();

            return new RegisterResponse
            {
                Success = true,
                Message = "Registration successful"
            };
        }

        public async Task<UserDto> GetUserByIdAsync(int userId)
        {
            var user = await _context.Accounts.FindAsync(userId);

            if (user == null)
                return null;

            return new UserDto
            {
                UserId = user.UserId,
                FullName = user.FullName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Role = user.Role,
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
    }
}
