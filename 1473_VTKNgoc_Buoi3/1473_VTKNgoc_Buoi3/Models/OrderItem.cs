using System.ComponentModel.DataAnnotations.Schema;

namespace _1473_VTKNgoc_Buoi3.Models
{
    public class OrderItem
    {
        public int Id { get; set; }

        public int OrderId { get; set; }

        public Order? Order { get; set; }

        public int ProductId { get; set; }

        public string ProductName { get; set; } = "";

        public string? ImageUrl { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        public int Quantity { get; set; }

        public bool IsService { get; set; }

        public string ItemStatus { get; set; } = "Chờ xử lý";

        public DateTime? AppointmentAt { get; set; }

        public string? PetName { get; set; }

        public string? PetType { get; set; }

        public string? PetAge { get; set; }

        public string? PetBreed { get; set; }

        public string? CustomerNote { get; set; }

        [NotMapped]
        public decimal Total => Price * Quantity;
    }
}
