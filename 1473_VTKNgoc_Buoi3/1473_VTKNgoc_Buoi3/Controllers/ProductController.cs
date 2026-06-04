using _1473_VTKNgoc_Buoi3.Models;
using _1473_VTKNgoc_Buoi3.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace _1473_VTKNgoc_Buoi3.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IWebHostEnvironment _environment;

        public ProductController(
            IProductRepository productRepository,
            ICategoryRepository categoryRepository,
            IWebHostEnvironment environment)
        {
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
            _environment = environment;
        }

        public async Task<IActionResult> Index(string? q, int? categoryId, decimal? minPrice, decimal? maxPrice, string? sort)
        {
            var products = await _productRepository.GetAllAsync();
            ViewBag.Context = "Product";
            await LoadFilterDataAsync(q, categoryId, minPrice, maxPrice, sort);
            return View(FilterProducts(products.Where(p => !IsServiceProduct(p)), q, categoryId, minPrice, maxPrice, sort));
        }

        public async Task<IActionResult> Services(string? q, int? categoryId, decimal? minPrice, decimal? maxPrice, string? sort)
        {
            var products = await _productRepository.GetAllAsync();
            ViewBag.Context = "Service";
            await LoadFilterDataAsync(q, categoryId, minPrice, maxPrice, sort);
            return View("Index", FilterProducts(products.Where(IsServiceProduct), q, categoryId, minPrice, maxPrice, sort));
        }

        public async Task<IActionResult> Display(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);

            if (product == null)
            {
                return NotFound();
            }

            ViewBag.Context = Request.Query["type"] == "service" ? "Service" : "Product";
            return View(product);
        }

        public async Task<IActionResult> Add()
        {
            var isService = Request.Query["type"] == "service";
            await LoadCategoriesAsync(isService ? 4 : null);
            ViewBag.Context = isService ? "Service" : "Product";
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(Product product, IFormFile? imageUrl, List<IFormFile>? imageUrls)
        {
            ModelState.Remove("ImageUrl");
            ModelState.Remove("Category");
            ModelState.Remove("Images");

            if (ModelState.IsValid)
            {
                if (imageUrl != null)
                {
                    product.ImageUrl = await SaveImageAsync(imageUrl);
                }

                if (imageUrls != null && imageUrls.Count > 0)
                {
                    product.Images = new List<ProductImage>();

                    foreach (var file in imageUrls)
                    {
                        product.Images.Add(new ProductImage { Url = await SaveImageAsync(file) });
                    }
                }

                await _productRepository.AddAsync(product);
                return RedirectToContextIndex();
            }

            await LoadCategoriesAsync(product.CategoryId);
            ViewBag.Context = Request.Query["type"] == "service" ? "Service" : "Product";
            return View(product);
        }

        public async Task<IActionResult> Update(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);

            if (product == null)
            {
                return NotFound();
            }

            await LoadCategoriesAsync(product.CategoryId);
            ViewBag.Context = Request.Query["type"] == "service" ? "Service" : "Product";
            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int id, Product product, IFormFile? imageUrl)
        {
            ModelState.Remove("ImageUrl");
            ModelState.Remove("Category");
            ModelState.Remove("Images");

            if (id != product.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var existingProduct = await _productRepository.GetByIdAsync(id);

                if (existingProduct == null)
                {
                    return NotFound();
                }

                existingProduct.Name = product.Name;
                existingProduct.Price = product.Price;
                existingProduct.Description = product.Description;
                existingProduct.CategoryId = product.CategoryId;

                if (imageUrl != null)
                {
                    existingProduct.ImageUrl = await SaveImageAsync(imageUrl);
                }

                await _productRepository.UpdateAsync(existingProduct);
                return RedirectToContextIndex();
            }

            await LoadCategoriesAsync(product.CategoryId);
            ViewBag.Context = Request.Query["type"] == "service" ? "Service" : "Product";
            return View(product);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);

            if (product == null)
            {
                return NotFound();
            }

            ViewBag.Context = Request.Query["type"] == "service" ? "Service" : "Product";
            return View(product);
        }

        [HttpPost, ActionName("DeleteConfirmed")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _productRepository.DeleteAsync(id);
            return RedirectToContextIndex();
        }

        private IActionResult RedirectToContextIndex()
        {
            return Request.Query["type"] == "service"
                ? RedirectToAction(nameof(Services))
                : RedirectToAction(nameof(Index));
        }

        private static bool IsServiceProduct(Product product)
        {
            return product.CategoryId == 4
                || product.Category?.Name.Contains("Dich vu", StringComparison.OrdinalIgnoreCase) == true
                || product.Category?.Name.Contains("Dịch vụ", StringComparison.OrdinalIgnoreCase) == true;
        }

        private async Task LoadCategoriesAsync(int? selectedId = null)
        {
            var categories = await _categoryRepository.GetAllAsync();
            ViewBag.Categories = new SelectList(categories, "Id", "Name", selectedId);
        }

        private async Task LoadFilterDataAsync(string? q, int? categoryId, decimal? minPrice, decimal? maxPrice, string? sort)
        {
            await LoadCategoriesAsync(categoryId);
            ViewBag.Search = q;
            ViewBag.CategoryId = categoryId;
            ViewBag.MinPrice = minPrice;
            ViewBag.MaxPrice = maxPrice;
            ViewBag.Sort = sort;
        }

        private static IEnumerable<Product> FilterProducts(
            IEnumerable<Product> products,
            string? q,
            int? categoryId,
            decimal? minPrice,
            decimal? maxPrice,
            string? sort)
        {
            if (!string.IsNullOrWhiteSpace(q))
            {
                products = products.Where(p =>
                    p.Name.Contains(q, StringComparison.OrdinalIgnoreCase)
                    || p.Description.Contains(q, StringComparison.OrdinalIgnoreCase)
                    || (p.Category?.Name.Contains(q, StringComparison.OrdinalIgnoreCase) ?? false));
            }

            if (categoryId.HasValue && categoryId.Value > 0)
            {
                products = products.Where(p => p.CategoryId == categoryId.Value);
            }

            if (minPrice.HasValue)
            {
                products = products.Where(p => p.Price >= minPrice.Value);
            }

            if (maxPrice.HasValue)
            {
                products = products.Where(p => p.Price <= maxPrice.Value);
            }

            return sort switch
            {
                "price-asc" => products.OrderBy(p => p.Price),
                "price-desc" => products.OrderByDescending(p => p.Price),
                "name" => products.OrderBy(p => p.Name),
                _ => products.OrderByDescending(p => p.Id)
            };
        }

        private async Task<string> SaveImageAsync(IFormFile image)
        {
            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(image.FileName)}";
            var folderPath = Path.Combine(_environment.WebRootPath, "images");

            Directory.CreateDirectory(folderPath);

            var savePath = Path.Combine(folderPath, fileName);

            using (var fileStream = new FileStream(savePath, FileMode.Create))
            {
                await image.CopyToAsync(fileStream);
            }

            return $"/images/{fileName}";
        }
    }
}
