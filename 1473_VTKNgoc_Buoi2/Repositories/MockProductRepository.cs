using _1473_VTKNgoc_Buoi2.Models;

namespace _1473_VTKNgoc_Buoi2.Repositories
{
	public class MockProductRepository : IProductRepository
	{
		private readonly List<Product> _products;
		public MockProductRepository()
		{
			// Tạo một số dữ liệu mẫu
			_products = new List<Product>
			{
            new Product{Id = 1,Name = "Thức ăn hạt cho mào",Price = 250000,Description = "Thức ăn dinh dưỡng dành cho mèo",ImageUrl = "/images/6.JPG"},

			new Product{Id = 2,Name = "Cát vệ sinh cho mèo",Price = 120000,Description = "Cát khử mùi hiệu quả.",ImageUrl = "/images/Cat.jpg"},

			new Product{Id = 3,Name = "Vòng cổ thú cưng",Price = 80000,Description = "Vòng cổ điều chỉnh kích thước.",ImageUrl = "/images/4.JPG"},

			new Product{Id = 4,Name = "Bát ăn đôi cho thú cưng",Price = 150000,Description = "Bộ bát ăn bằng inox.",ImageUrl = "/images/2.JPG"},

			new Product{Id = 5,Name = "Đồ chơi bóng cho chó mèo",Price = 60000,Description = "Đồ chơi vận động cho thú cưng.",ImageUrl = "/images/1.jpg"}
            };
		}		
		public IEnumerable<Product> GetAll()
		{
			return _products;
		}
		public Product GetById(int id)
		{
			return _products.FirstOrDefault(p => p.Id == id);
		}
		public void Add(Product product)
		{
			product.Id = _products.Max(p => p.Id) + 1;
			_products.Add(product);
		}
		public void Update(Product product)
		{
			var index = _products.FindIndex(p => p.Id == product.Id);
			if (index != -1)
			{
				_products[index] = product;
			}
		}
		public void Delete(int id)
		{
			var product = _products.FirstOrDefault(p => p.Id == id);
			if (product != null)
			{
				_products.Remove(product);
			}
		}
	}
}