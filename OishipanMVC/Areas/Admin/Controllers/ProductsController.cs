using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using OishipanMVC.Models;
using OishipanMVC.Services;
using System.Text.Json;

namespace OishipanMVC.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("admin/products")]
    [Authorize(Roles = "Admin,Staff")]
    public class ProductsController : Controller
    {
        private readonly IApiClient _apiClient;

        public ProductsController(IApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        [HttpGet("")]
        public async Task<IActionResult> Index()
        {
            try
            {
                var products = await _apiClient.GetAsync<List<ProductViewModel>>("/api/products");
                return View(products);
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Không thể tải sản phẩm: " + ex.Message;
                return View(new List<ProductViewModel>());
            }
        }

        private async Task LoadCategoryBrandLists()
        {
            try
            {
                ViewBag.Categories = await _apiClient.GetAsync<List<CategoryViewModel>>("/api/categories");
            }
            catch
            {
                ViewBag.Categories = new List<CategoryViewModel>();
            }

            try
            {
                ViewBag.Brands = await _apiClient.GetAsync<List<BrandViewModel>>("/api/brands");
            }
            catch
            {
                ViewBag.Brands = new List<BrandViewModel>();
            }
        }

        [HttpGet("create")]
        public async Task<IActionResult> Create()
        {
            await LoadCategoryBrandLists();
            return View(new ProductViewModel());
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create(IFormFile imageFile, string name, decimal price, int quantity, int categoryId, int brandId, string description, string variantsJson)
        {
            var model = new ProductViewModel
            {
                Name = name ?? string.Empty,
                Price = price,
                Quantity = quantity,
                CategoryId = categoryId,
                BrandId = brandId,
                Description = description ?? string.Empty
            };

            try
            {
                if (string.IsNullOrWhiteSpace(name) || price <= 0 || quantity <= 0 || categoryId <= 0 || brandId <= 0 || imageFile == null || imageFile.Length == 0)
                {
                    ViewBag.Error = "Vui lòng điền đầy đủ các trường bắt buộc và chọn ảnh sản phẩm.";
                    await LoadCategoryBrandLists();
                    return View(model);
                }

                var createRequest = new
                {
                    name,
                    price,
                    quantity,
                    categoryId,
                    brandId,
                    description,
                    variantsJson
                };

                var product = await _apiClient.PostAsync<ProductViewModel>("/api/products", createRequest);
                if (product == null)
                {
                    ViewBag.Error = "Không thể tạo sản phẩm.";
                    await LoadCategoryBrandLists();
                    return View(model);
                }

                var formContent = new MultipartFormDataContent();
                formContent.Add(new StreamContent(imageFile.OpenReadStream()), "file", imageFile.FileName);

                try
                {
                    var uploadResult = await _apiClient.PostFormAsync<JsonElement>($"/api/products/{product.ProductId}/upload-image", formContent);
                    if (!uploadResult.TryGetProperty("url", out var urlProperty) || string.IsNullOrWhiteSpace(urlProperty.GetString()))
                    {
                        await _apiClient.DeleteAsync($"/api/products/{product.ProductId}");
                        ViewBag.Error = "Không thể tải lên ảnh cho sản phẩm.";
                        await LoadCategoryBrandLists();
                        return View(model);
                    }
                }
                catch (Exception ex)
                {
                    await _apiClient.DeleteAsync($"/api/products/{product.ProductId}");
                    ViewBag.Error = "Không thể tải lên ảnh: " + ex.Message;
                    await LoadCategoryBrandLists();
                    return View(model);
                }

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Lỗi: " + ex.Message;
                await LoadCategoryBrandLists();
            }

            return View(model);
        }

        [HttpGet("edit/{id}")]
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var product = await _apiClient.GetAsync<ProductViewModel>($"/api/products/{id}");
                if (product == null)
                    return NotFound();

                await LoadCategoryBrandLists();
                return View(product);
            }
            catch
            {
                return NotFound();
            }
        }

        [HttpPost("edit/{id}")]
        public async Task<IActionResult> Edit(int id, IFormFile? imageFile, string name, decimal price, int quantity, int categoryId, int brandId, string description, string variantsJson)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(name) || price <= 0)
                {
                    ViewBag.Error = "Vui lòng điền đầy đủ các trường bắt buộc";
                    await LoadCategoryBrandLists();
                    return View();
                }

                var updateRequest = new
                {
                    productId = id,
                    name,
                    price,
                    quantity,
                    categoryId,
                    brandId,
                    description,
                    variantsJson
                };

                var result = await _apiClient.PutAsync<ProductViewModel>($"/api/products/{id}", updateRequest);
                if (result == null)
                {
                    ViewBag.Error = "Không thể cập nhật sản phẩm";
                    await LoadCategoryBrandLists();
                    return View();
                }

                if (imageFile != null && imageFile.Length > 0)
                {
                    var formContent = new MultipartFormDataContent();
                    formContent.Add(new StreamContent(imageFile.OpenReadStream()), "file", imageFile.FileName);

                    try
                    {
                        var uploadResult = await _apiClient.PostFormAsync<JsonElement>($"/api/products/{id}/upload-image", formContent);
                        if (!uploadResult.TryGetProperty("url", out var urlProperty) || string.IsNullOrWhiteSpace(urlProperty.GetString()))
                        {
                            ViewBag.Error = "Cập nhật sản phẩm thành công nhưng không thể tải ảnh mới.";
                            await LoadCategoryBrandLists();
                            return View(result);
                        }
                    }
                    catch (Exception ex)
                    {
                        ViewBag.Error = "Cập nhật sản phẩm thành công nhưng không thể tải ảnh mới: " + ex.Message;
                        await LoadCategoryBrandLists();
                        return View(result);
                    }
                }

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Lỗi: " + ex.Message;
                await LoadCategoryBrandLists();
            }

            return View();
        }

        [HttpPost("delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _apiClient.DeleteAsync($"/api/products/{id}");
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