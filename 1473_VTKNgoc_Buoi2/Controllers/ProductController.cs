using _1473_VTKNgoc_Buoi2.Models;
using _1473_VTKNgoc_Buoi2.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace _1473_VTKNgoc_Buoi2.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;

        public ProductController(
            IProductRepository productRepository,
            ICategoryRepository categoryRepository)
        {
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
        }

        public IActionResult Index()
        {
            var products = _productRepository.GetAll();
            return View(products);
        }

        public IActionResult Display(int id)
        {
            var product = _productRepository.GetById(id);

            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        public IActionResult Add()
        {
            LoadCategories();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Add(
            Product product,
            IFormFile imageUrl,
            List<IFormFile> imageUrls)
        {
            if (ModelState.IsValid)
            {
                if (imageUrl != null)
                {
                    product.ImageUrl = await SaveImage(imageUrl);
                }

                if (imageUrls != null && imageUrls.Count > 0)
                {
                    product.ImageUrls = new List<string>();

                    foreach (var file in imageUrls)
                    {
                        product.ImageUrls.Add(await SaveImage(file));
                    }
                }

                _productRepository.Add(product);
                return RedirectToAction("Index");
            }

            LoadCategories();
            return View(product);
        }

        public IActionResult Update(int id)
        {
            var product = _productRepository.GetById(id);

            if (product == null)
            {
                return NotFound();
            }

            LoadCategories(product.CategoryId);
            return View(product);
        }

        [HttpPost]
        public async Task<IActionResult> Update(
            Product product,
            IFormFile imageUrlFile)
        {
            if (ModelState.IsValid)
            {
                var oldProduct = _productRepository.GetById(product.Id);

                if (oldProduct == null)
                {
                    return NotFound();
                }

                if (imageUrlFile != null)
                {
                    product.ImageUrl = await SaveImage(imageUrlFile);
                }
                else
                {
                    product.ImageUrl = oldProduct.ImageUrl;
                }

                _productRepository.Update(product);
                return RedirectToAction("Index");
            }

            LoadCategories(product.CategoryId);
            return View(product);
        }

        public IActionResult Delete(int id)
        {
            var product = _productRepository.GetById(id);

            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        [HttpPost, ActionName("DeleteConfirmed")]
        public IActionResult DeleteConfirmed(int id)
        {
            _productRepository.Delete(id);
            return RedirectToAction("Index");
        }

        private void LoadCategories(int? selectedId = null)
        {
            var categories = _categoryRepository.GetAllCategories();
            ViewBag.Categories = new SelectList(categories, "Id", "Name", selectedId);
        }

        private async Task<string> SaveImage(IFormFile image)
        {
            var fileName = Guid.NewGuid().ToString()
                           + Path.GetExtension(image.FileName);

            var folderPath = Path.Combine(
                Directory.GetCurrentDirectory(),
                "wwwroot",
                "images"
            );

            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            var savePath = Path.Combine(folderPath, fileName);

            using (var fileStream = new FileStream(savePath, FileMode.Create))
            {
                await image.CopyToAsync(fileStream);
            }

            return "/images/" + fileName;
        }
    }
}