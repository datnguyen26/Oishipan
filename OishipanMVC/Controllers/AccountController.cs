using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.Text.Json;
using System.Text.RegularExpressions;
using OishipanMVC.Services;

namespace OishipanMVC.Controllers
{
    public class AccountController : Controller
    {
        private readonly IApiClient _apiClient;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AccountController(IApiClient apiClient, IHttpContextAccessor httpContextAccessor)
        {
            _apiClient = apiClient;
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpGet("dang-nhap")]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost("dang-nhap")]
        public async Task<IActionResult> Login(string email, string password)
        {
            try
            {
                // Validate input
                if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
                {
                    ViewBag.Error = "Vui lòng nhập email và mật khẩu";
                    return View();
                }

                // Validate email format
                var emailRegex = new Regex(@"^[^\s@]+@[^\s@]+\.[^\s@]+$");
                if (!emailRegex.IsMatch(email))
                {
                    ViewBag.Error = "Email không hợp lệ";
                    return View();
                }

                // Validate password length
                if (password.Length < 6)
                {
                    ViewBag.Error = "Mật khẩu phải có ít nhất 6 ký tự";
                    return View();
                }

                var loginRequest = new { email, password };
                var result = await _apiClient.PostAsync<JsonElement>("/api/auth/login", loginRequest);

                if (result.GetProperty("success").GetBoolean())
                {
                    var user = result.GetProperty("user");

                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.NameIdentifier, user.GetProperty("userId").GetInt32().ToString()),
                        new Claim(ClaimTypes.Email, user.GetProperty("email").GetString() ?? string.Empty),
                        new Claim(ClaimTypes.Name, user.GetProperty("fullName").GetString() ?? string.Empty),
                        new Claim(ClaimTypes.Role, user.GetProperty("role").GetString() ?? string.Empty)
                    };

                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var authProperties = new AuthenticationProperties
                    {
                        IsPersistent = true,
                        ExpiresUtc = DateTimeOffset.UtcNow.AddHours(1)
                    };

                    await HttpContext.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(claimsIdentity),
                        authProperties);

                    var token = result.GetProperty("token").GetString();
                    if (!string.IsNullOrWhiteSpace(token))
                    {
                        HttpContext.Session.SetString("ApiToken", token);
                        _apiClient.SetAuthToken(token);
                    }

                    var role = user.GetProperty("role").GetString() ?? string.Empty;
                    if (role.Equals("Admin", StringComparison.OrdinalIgnoreCase) ||
                        role.Equals("Staff", StringComparison.OrdinalIgnoreCase))
                    {
                        return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
                    }

                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ViewBag.Error = result.GetProperty("message").GetString() ?? "Đăng nhập thất bại";
                }
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Đăng nhập thất bại: " + ex.Message;
            }

            return View();
        }

        [HttpGet("dang-ky")]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost("dang-ky")]
        public async Task<IActionResult> Register(string fullName, string email, string phoneNumber, string password, string address)
        {
            try
            {
                // Validate required fields
                if (string.IsNullOrWhiteSpace(fullName) || string.IsNullOrWhiteSpace(email) 
                    || string.IsNullOrWhiteSpace(phoneNumber) || string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(address))
                {
                    ViewBag.Error = "Vui lòng điền đầy đủ các trường bắt buộc";
                    return View();
                }

                // Validate Full Name
                if (fullName.Length < 3 || fullName.Length > 100)
                {
                    ViewBag.Error = "Họ tên phải từ 3 đến 100 ký tự";
                    return View();
                }

                // Validate Email format
                var emailRegex = new Regex(@"^[^\s@]+@[^\s@]+\.[^\s@]+$");
                if (!emailRegex.IsMatch(email))
                {
                    ViewBag.Error = "Email không hợp lệ";
                    return View();
                }

                // Validate Phone Number
                var phoneRegex = new Regex(@"^0[0-9]{9}$");
                if (!phoneRegex.IsMatch(phoneNumber))
                {
                    ViewBag.Error = "Số điện thoại phải bắt đầu bằng 0 và có 10 chữ số";
                    return View();
                }

                // Validate Password
                if (password.Length < 6 || password.Length > 100)
                {
                    ViewBag.Error = "Mật khẩu phải từ 6 đến 100 ký tự";
                    return View();
                }

                // Validate Address
                if (address.Length > 200)
                {
                    ViewBag.Error = "Địa chỉ không được vượt quá 200 ký tự";
                    return View();
                }

                var registerRequest = new { fullName, email, phoneNumber, password, address };
                var result = await _apiClient.PostAsync<JsonElement>("/api/auth/register", registerRequest);

                if (result.GetProperty("success").GetBoolean())
                {
                    ViewBag.Success = "Đăng ký thành công! Vui lòng đăng nhập.";
                    return RedirectToAction("Login");
                }
                else
                {
                    ViewBag.Error = result.GetProperty("message").GetString() ?? "Đăng ký thất bại";
                }
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Đăng ký thất bại: " + ex.Message;
            }

            return View();
        }

        [HttpGet("dang-xuat")]
        public async Task<IActionResult> Logout()
        {
            HttpContext.Session.Remove("ApiToken");
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return RedirectToAction("Index", "Home");
        }

        [Authorize]
        [HttpGet("ho-so")]
        public async Task<IActionResult> Profile()
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return RedirectToAction("Login");
                }

                var user = await _apiClient.GetAsync<dynamic>($"/api/auth/profile/{userId}");
                return View(user);
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Không thể tải hồ sơ: " + ex.Message;
                return View();
            }
        }

        [Authorize]
        [HttpPost("cap-nhat-ho-so")]
        public async Task<IActionResult> UpdateProfile(string fullName, string phoneNumber, string address)
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return RedirectToAction("Login");
                }

                // Validate
                if (string.IsNullOrWhiteSpace(fullName) || string.IsNullOrWhiteSpace(phoneNumber))
                {
                    ViewBag.Error = "Vui lòng điền đầy đủ thông tin";
                    return await Profile();
                }

                if (fullName.Length < 3 || fullName.Length > 100)
                {
                    ViewBag.Error = "Họ tên phải từ 3 đến 100 ký tự";
                    return await Profile();
                }

                var phoneRegex = new Regex(@"^0[0-9]{9}$");
                if (!phoneRegex.IsMatch(phoneNumber))
                {
                    ViewBag.Error = "Số điện thoại không hợp lệ";
                    return await Profile();
                }

                var updateRequest = new { fullName, phoneNumber, address };
                var result = await _apiClient.PutAsync<JsonElement>($"/api/auth/update-profile/{userId}", updateRequest);

                ViewBag.Success = "Cập nhật hồ sơ thành công";
                return await Profile();
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Cập nhật thất bại: " + ex.Message;
                return await Profile();
            }
        }

        [HttpGet("truy-cap-bi-cam")]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
