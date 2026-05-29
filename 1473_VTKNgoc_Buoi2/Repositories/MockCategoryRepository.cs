using _1473_VTKNgoc_Buoi2.Models;

namespace _1473_VTKNgoc_Buoi2.Repositories
{
    public class MockCategoryRepository : ICategoryRepository
    {
        private static List<Category> _categoryList = new List<Category>
        {
            new Category { Id = 1, Name = "Thức ăn" },
            new Category { Id = 2, Name = "Phụ kiện" },
            new Category { Id = 3, Name = "Dụng cụ ăn" },
            new Category { Id = 4, Name = "Các loại hạt" }
        };

        public IEnumerable<Category> GetAllCategories()
        {
            return _categoryList;
        }

        public Category GetById(int id)
        {
            return _categoryList.FirstOrDefault(c => c.Id == id);
        }

        public void Add(Category category)
        {
            category.Id = _categoryList.Any()
                ? _categoryList.Max(c => c.Id) + 1
                : 1;

            _categoryList.Add(category);
        }

        public void Update(Category category)
        {
            var existing = _categoryList.FirstOrDefault(c => c.Id == category.Id);

            if (existing != null)
            {
                existing.Name = category.Name;
            }
        }

        public void Delete(int id)
        {
            var category = _categoryList.FirstOrDefault(c => c.Id == id);

            if (category != null)
            {
                _categoryList.Remove(category);
            }
        }
    }
}