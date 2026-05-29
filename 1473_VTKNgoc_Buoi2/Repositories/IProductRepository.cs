using _1473_VTKNgoc_Buoi2.Models;

namespace _1473_VTKNgoc_Buoi2.Repositories
{
	public interface IProductRepository
	{
		IEnumerable<Product> GetAll();
		Product GetById(int id);
		void Add(Product product);
		void Update(Product product);
		void Delete(int id);
	}
}
