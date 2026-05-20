using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using System.Text.Json;
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
                var loginRequest = new { email, password };
                var result = await _apiClient.PostAsync<JsonElement>("/api/auth/login", loginRequest);

                if (result.GetProperty("success").GetBoolean())
                {
                    var token = result.GetProperty("token").GetString() ?? string.Empty;
                    var user = result.GetProperty("user");

                    HttpContext.Session.SetString("JwtToken", token);
                    _apiClient.SetAuthToken(token);

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

                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ViewBag.Error = result.GetProperty("message").GetString();
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
                var registerRequest = new { fullName, email, phoneNumber, password, address };
                var result = await _apiClient.PostAsync<JsonElement>("/api/auth/register", registerRequest);

                if (result.GetProperty("success").GetBoolean())
                {
                    return RedirectToAction("Login");
                }
                else
                {
                    ViewBag.Error = result.GetProperty("message").GetString();
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
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            HttpContext.Session.Remove("JwtToken");
            _apiClient.ClearAuthToken();

            return RedirectToAction("Index", "Home");
        }

        [HttpGet("truy-cap-bi-cam")]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
