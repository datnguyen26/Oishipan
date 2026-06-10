using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OishipanMVC.Models;
using OishipanMVC.Services;

namespace OishipanMVC.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("admin/categories")]
    [Authorize(Roles = "Admin,Staff")]
    public class CategoriesController : Controller
    {
        private readonly IApiClient _apiClient;

        public CategoriesController(IApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        [HttpGet("")]
        public async Task<IActionResult> Index()
        {
            try
            {
                var categories = await _apiClient.GetAsync<List<CategoryViewModel>>("/api/categories");
                return View(categories);
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Không thể tải danh sách danh mục: " + ex.Message;
                return View(new List<CategoryViewModel>());
            }
        }

        [HttpGet("create")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create(string categoryName, string description)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(categoryName))
                {
                    ViewBag.Error = "Vui lòng nhập tên danh mục.";
                    return View();
                }

                var request = new
                {
                    categoryName,
                    description
                };

                await _apiClient.PostAsync<CategoryViewModel>("/api/categories", request);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Lỗi khi tạo danh mục: " + ex.Message;
                return View();
            }
        }

        [HttpGet("edit/{id}")]
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var category = await _apiClient.GetAsync<CategoryViewModel>($"/api/categories/{id}");
                return View(category);
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Không thể tải danh mục: " + ex.Message;
                return RedirectToAction("Index");
            }
        }

        [HttpPost("edit/{id}")]
        public async Task<IActionResult> Edit(int id, string categoryName, string description)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(categoryName))
                {
                    ViewBag.Error = "Vui lòng nhập tên danh mục.";
                    var category = new CategoryViewModel
                    {
                        CategoryId = id,
                        CategoryName = categoryName,
                        Description = description
                    };
                    return View(category);
                }

                var request = new
                {
                    categoryName,
                    description
                };

                await _apiClient.PutAsync<CategoryViewModel>($"/api/categories/{id}", request);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Lỗi khi cập nhật danh mục: " + ex.Message;
                var category = new CategoryViewModel
                {
                    CategoryId = id,
                    CategoryName = categoryName,
                    Description = description
                };
                return View(category);
            }
        }

        [HttpPost("delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var success = await _apiClient.DeleteAsync($"/api/categories/{id}");
                if (!success)
                {
                    TempData["Error"] = "Không thể xóa danh mục này. Danh mục có chứa sản phẩm.";
                    return RedirectToAction("Index");
                }
                TempData["Success"] = "Xóa danh mục thành công";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Lỗi khi xóa danh mục: " + ex.Message;
                return RedirectToAction("Index");
            }
        }
    }
}