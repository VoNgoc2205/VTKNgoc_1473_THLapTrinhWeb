using _1473_VTKNgoc_Buoi3.Models;
using _1473_VTKNgoc_Buoi3.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace _1473_VTKNgoc_Buoi3.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ICategoryRepository _categoryRepository;

        public CategoryController(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        public async Task<IActionResult> Index(string? q, string? sort)
        {
            var categories = await _categoryRepository.GetAllAsync();
            if (!string.IsNullOrWhiteSpace(q))
            {
                categories = categories.Where(c => c.Name.Contains(q, StringComparison.OrdinalIgnoreCase));
            }

            categories = sort switch
            {
                "name" => categories.OrderBy(c => c.Name),
                "count-desc" => categories.OrderByDescending(c => c.Products?.Count ?? 0),
                _ => categories.OrderBy(c => c.Id)
            };

            ViewBag.Search = q;
            ViewBag.Sort = sort;
            return View(categories);
        }

        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(Category category)
        {
            if (ModelState.IsValid)
            {
                await _categoryRepository.AddAsync(category);
                return RedirectToAction(nameof(Index));
            }

            return View(category);
        }

        public async Task<IActionResult> Update(int id)
        {
            var category = await _categoryRepository.GetByIdAsync(id);

            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int id, Category category)
        {
            if (id != category.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                await _categoryRepository.UpdateAsync(category);
                return RedirectToAction(nameof(Index));
            }

            return View(category);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var category = await _categoryRepository.GetByIdAsync(id);

            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        [HttpPost, ActionName("DeleteConfirmed")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _categoryRepository.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
