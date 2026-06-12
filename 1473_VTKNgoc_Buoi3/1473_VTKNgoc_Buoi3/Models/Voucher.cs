using System.ComponentModel.DataAnnotations;

namespace _1473_VTKNgoc_Buoi3.Models
{
    public class Voucher
    {
        [Required(ErrorMessage = "Vui lòng nhập mã voucher.")]
        public string Code { get; set; } = "";

        [Required(ErrorMessage = "Vui lòng chọn loại giảm giá.")]
        public string DiscountType { get; set; } = "Percent";

        [Range(0, 100000000, ErrorMessage = "Giá trị giảm không hợp lệ.")]
        public decimal DiscountValue { get; set; }

        [Range(0, 100000000, ErrorMessage = "Đơn tối thiểu không hợp lệ.")]
        public decimal MinimumOrderAmount { get; set; }

        [Range(0, 100000, ErrorMessage = "Số lượng voucher không hợp lệ.")]
        public int Quantity { get; set; } = 100;

        [DataType(DataType.Date)]
        public DateTime? ExpiresAt { get; set; } = DateTime.Today.AddMonths(1);

        public bool IsActive { get; set; } = true;

        public bool IsExpired => ExpiresAt.HasValue && ExpiresAt.Value.Date < DateTime.Today;

        public bool IsAvailable => IsActive && !IsExpired && Quantity > 0;

        public string StatusText
        {
            get
            {
                if (IsExpired)
                {
                    return "Hết hạn sử dụng";
                }

                if (Quantity <= 0)
                {
                    return "Hết số lượng";
                }

                if (!IsActive)
                {
                    return "Đã tắt";
                }

                return "Còn hạn sử dụng";
            }
        }

        public string Description =>
            DiscountType == "FreeShip"
                ? "Miễn phí vận chuyển"
                : DiscountType == "Percent"
                    ? $"Giảm {DiscountValue:N0}%"
                    : $"Giảm {DiscountValue:N0} VNĐ";
    }
}
