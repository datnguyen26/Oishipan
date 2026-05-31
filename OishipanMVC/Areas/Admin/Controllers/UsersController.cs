using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using OishipanMVC.Services;
using OishipanMVC.Models;
using System.Text.Json;

namespace OishipanMVC.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("admin/users")]
    [Authorize(Roles = "Admin,Staff")]
    public class UsersController : Controller
    {
        private readonly IApiClient _apiClient;

        public UsersController(IApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        [HttpGet("")]
        public async Task<IActionResult> Index(string search = null, int page = 1)
        {
            try
            {
                ViewBag.SearchTerm = search;
                ViewBag.CurrentPage = page;

                var query = $"/api/users?search={System.Net.WebUtility.UrlEncode(search ?? string.Empty)}&page={page}&pageSize=20";
                var resp = await _apiClient.GetAsync<dynamic>(query);
                var users = new List<UserViewModel>();

                if (resp != null)
                {
                    try
                    {
                        if (resp is JsonElement json)
                        {
                            if (json.TryGetProperty("users", out var usersArray))
                            {
                                users = System.Text.Json.JsonSerializer.Deserialize<List<UserViewModel>>(usersArray.GetRawText()) ?? new List<UserViewModel>();
                            }
                        }
                    }
                    catch { }
                }

                return View(users);
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Không thể tải danh sách người dùng: " + ex.Message;
                return View(new List<UserViewModel>());
            }
        }

        [HttpGet("create")]
        public IActionResult Create()
        {
            return View(new UserViewModel());
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create(string fullName, string email, string phoneNumber, string password, string address, string role = "User")
        {
            try
            {
                if (string.IsNullOrWhiteSpace(fullName) || string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
                {
                    ViewBag.Error = "Vui lòng điền đầy đủ: Họ tên, Email, Mật khẩu";
                    return View();
                }

                var request = new
                {
                    fullName,
                    email,
                    phoneNumber,
                    password,
                    address,
                    role
                };

                var result = await _apiClient.PostAsync<UserViewModel>("/api/users", request);
                if (result != null)
                    return RedirectToAction("Index");

                ViewBag.Error = "Không thể tạo người dùng";
                return View();
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Lỗi: " + ex.Message;
                return View();
            }
        }

        [HttpGet("edit/{id}")]
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var user = await _apiClient.GetAsync<UserViewModel>($"/api/users/{id}");
                if (user == null)
                    return NotFound();

                return View(user);
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Không thể tải người dùng: " + ex.Message;
                return RedirectToAction("Index");
            }
        }

        [HttpPost("edit/{id}")]
        public async Task<IActionResult> Edit(int id, string fullName, string email, string phoneNumber, string address, string role)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(fullName) || string.IsNullOrWhiteSpace(email))
                {
                    ViewBag.Error = "Vui lòng điền đầy đủ: Họ tên, Email";
                    return View();
                }

                var request = new
                {
                    fullName,
                    email,
                    phoneNumber,
                    address,
                    role
                };

                var result = await _apiClient.PutAsync<UserViewModel>($"/api/users/{id}", request);
                if (result != null)
                    return RedirectToAction("Index");

                ViewBag.Error = "Không thể cập nhật người dùng";
                return View();
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Lỗi: " + ex.Message;
                return View();
            }
        }

        [HttpPost("delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _apiClient.DeleteAsync($"/api/users/{id}");
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Lỗi: " + ex.Message;
                return RedirectToAction("Index");
            }
        }

        [HttpPost("toggle-status/{id}")]
        public async Task<IActionResult> ToggleStatus(int id)
        {
            try
            {
                await _apiClient.PostAsync<dynamic>($"/api/users/{id}/toggle-status", new { });
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Lỗi: " + ex.Message;
                return RedirectToAction("Index");
            }
        }
    }
}
