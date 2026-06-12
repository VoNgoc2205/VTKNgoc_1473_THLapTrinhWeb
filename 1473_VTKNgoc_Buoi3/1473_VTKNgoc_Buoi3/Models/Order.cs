using System.ComponentModel.DataAnnotations.Schema;

namespace _1473_VTKNgoc_Buoi3.Models
{
    public class Order
    {
        public int Id { get; set; }

        public string UserId { get; set; } = "";

        public string CustomerName { get; set; } = "";

        public string CustomerEmail { get; set; } = "";

        public string? CustomerPhone { get; set; }

        public string? CustomerAddress { get; set; }

        public string? PaymentMethod { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public string Status { get; set; } = "Chờ xác nhận";

        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalAmount { get; set; }

        public List<OrderItem> Items { get; set; } = new();
    }
}
