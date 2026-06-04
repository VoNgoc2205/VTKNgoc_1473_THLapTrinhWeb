using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace _1473_VTKNgoc_Buoi3.Models
{
    public class Product
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tên")]
        [StringLength(100)]
        [Display(Name = "Tên")]
        public string Name { get; set; } = string.Empty;

        [Range(1000, 100000000, ErrorMessage = "Giá phải lớn hơn 1.000 VNĐ")]
        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Giá")]
        public decimal Price { get; set; }

        [Display(Name = "Mô tả")]
        public string Description { get; set; } = string.Empty;

        public string? ImageUrl { get; set; }

        public List<ProductImage>? Images { get; set; }

        [Display(Name = "Danh mục")]
        public int CategoryId { get; set; }

        public Category? Category { get; set; }
    }
}
