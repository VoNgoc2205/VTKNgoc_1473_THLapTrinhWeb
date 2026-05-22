using _1473_VTKNgoc_Buoi2.Models;

namespace _1473_VTKNgoc_Buoi2.Repositories
{
    public interface ICategoryRepository
    {
        IEnumerable<Category> GetAllCategories();

        Category GetById(int id);

        void Add(Category category);

        void Update(Category category);

        void Delete(int id);
    }
}