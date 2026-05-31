using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OishipanMVC.Models;
using OishipanMVC.Services;

namespace OishipanMVC.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("admin/brands")]
    [Authorize(Roles = "Admin,Staff")]
    public class BrandsController : Controller
    {
        private readonly IApiClient _apiClient;

        public BrandsController(IApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        [HttpGet("")]
        public async Task<IActionResult> Index()
        {
            try
            {
                var brands = await _apiClient.GetAsync<List<BrandViewModel>>("/api/brands");
                return View(brands ?? new List<BrandViewModel>());
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Không thể tải danh sách thương hiệu: " + ex.Message;
                return View(new List<BrandViewModel>());
            }
        }

        [HttpGet("create")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create(string brandName, string description)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(brandName))
                {
                    ViewBag.Error = "Vui lòng nhập tên thương hiệu.";
                    return View();
                }

                var request = new
                {
                    brandName,
                    description
                };

                await _apiClient.PostAsync<BrandViewModel>("/api/brands", request);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Lỗi khi tạo thương hiệu: " + ex.Message;
                return View();
            }
        }

        [HttpGet("edit/{id}")]
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var brand = await _apiClient.GetAsync<BrandViewModel>($"/api/brands/{id}");
                return View(brand);
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Không thể tải thương hiệu: " + ex.Message;
                return RedirectToAction("Index");
            }
        }

        [HttpPost("edit/{id}")]
        public async Task<IActionResult> Edit(int id, string brandName, string description)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(brandName))
                {
                    ViewBag.Error = "Vui lòng nhập tên thương hiệu.";
                    var brand = new BrandViewModel
                    {
                        BrandId = id,
                        BrandName = brandName,
                        Description = description
                    };
                    return View(brand);
                }

                var request = new
                {
                    brandName,
                    description
                };

                await _apiClient.PutAsync<BrandViewModel>($"/api/brands/{id}", request);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Lỗi khi cập nhật thương hiệu: " + ex.Message;
                var brand = new BrandViewModel
                {
                    BrandId = id,
                    BrandName = brandName,
                    Description = description
                };
                return View(brand);
            }
        }

        [HttpPost("delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _apiClient.DeleteAsync($"/api/brands/{id}");
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Lỗi khi xóa thương hiệu: " + ex.Message;
                return RedirectToAction("Index");
            }
        }
    }
}