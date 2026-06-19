using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace WebAPI.Models
{
    public class Category
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tên danh mục")]
        [StringLength(50)]
        public string Name { get; set; } = string.Empty;

        [JsonIgnore]
        public List<Product>? Products { get; set; }
    }
}