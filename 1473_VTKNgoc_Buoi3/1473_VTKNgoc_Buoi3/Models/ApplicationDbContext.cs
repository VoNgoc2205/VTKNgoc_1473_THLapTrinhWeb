using Microsoft.EntityFrameworkCore;

namespace _1473_VTKNgoc_Buoi3.Models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<ProductImage> ProductImages { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Thức ăn thú cưng" },
                new Category { Id = 2, Name = "Phụ kiện" },
                new Category { Id = 3, Name = "Chăm sóc sức khỏe" },
                new Category { Id = 4, Name = "Dịch vụ chăm sóc" }
            );

            modelBuilder.Entity<Product>().HasData(
                new Product
                {
                    Id = 1,
                    Name = "Hạt dinh dưỡng cho mèo",
                    Price = 185000,
                    Description = "Hạt giàu protein, bổ sung vitamin giúp mèo khỏe mạnh và lông mượt.",
                    ImageUrl = "/images/Hat.jpg",
                    CategoryId = 1
                },
                new Product
                {
                    Id = 2,
                    Name = "Gối ngủ mềm cho thú cưng",
                    Price = 249000,
                    Description = "Gối ngủ êm ái, giúp thú cưng nghỉ ngơi thoải mái trong mọi góc nhà.",
                    ImageUrl = "/images/GoiNgu.jpg",
                    CategoryId = 2
                },
                new Product
                {
                    Id = 3,
                    Name = "Nhà cây cho mèo",
                    Price = 390000,
                    Description = "Không gian leo trèo và nghỉ ngơi gọn đẹp, phù hợp cho mèo năng động.",
                    ImageUrl = "/images/Cat.jpg",
                    CategoryId = 3
                },
                new Product
                {
                    Id = 4,
                    Name = "Spa & Grooming cao cấp",
                    Price = 200000,
                    Description = "Dịch vụ tắm, sấy và cắt tỉa lông giúp thú cưng sạch sẽ, thơm tho và thoải mái.",
                    ImageUrl = "/images/pet-banner.jpg",
                    CategoryId = 4
                },
                new Product
                {
                    Id = 5,
                    Name = "Khách sạn cho thú cưng",
                    Price = 350000,
                    Description = "Không gian lưu trú sạch sẽ, có khu vui chơi và nhân viên theo dõi thú cưng trong ngày.",
                    ImageUrl = "/images/GoiNgu.jpg",
                    CategoryId = 4
                },
                new Product
                {
                    Id = 6,
                    Name = "Tiêm phòng & khám tổng quát",
                    Price = 150000,
                    Description = "Gói kiểm tra sức khỏe định kỳ và tư vấn tiêm phòng cần thiết cho chó mèo.",
                    ImageUrl = "/images/Cat.jpg",
                    CategoryId = 4
                }
            );
        }
    }
}
