using System.ComponentModel.DataAnnotations;

namespace _1473_VTKNgoc_Buoi3.Models
{
    public class ServiceBookingViewModel
    {
        public int ProductId { get; set; }

        public string ServiceName { get; set; } = "";

        public string? ImageUrl { get; set; }

        public decimal Price { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tên thú cưng.")]
        public string PetName { get; set; } = "";

        [Required(ErrorMessage = "Vui lòng nhập loại thú cưng.")]
        public string PetType { get; set; } = "";

        [Required(ErrorMessage = "Vui lòng nhập tuổi thú cưng.")]
        public string PetAge { get; set; } = "";

        [Required(ErrorMessage = "Vui lòng nhập giống loài.")]
        public string PetBreed { get; set; } = "";

        [Required(ErrorMessage = "Vui lòng chọn thời gian hẹn.")]
        public DateTime? AppointmentAt { get; set; }

        public string? CustomerNote { get; set; }
    }
}
