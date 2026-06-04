using System.ComponentModel.DataAnnotations;

namespace _1473_VTKNgoc_Buoi3.Models
{
    public class Category
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tên danh mục")]
        [StringLength(50)]
        [Display(Name = "Tên danh mục")]
        public string Name { get; set; } = string.Empty;

        public List<Product>? Products { get; set; }
    }
}
