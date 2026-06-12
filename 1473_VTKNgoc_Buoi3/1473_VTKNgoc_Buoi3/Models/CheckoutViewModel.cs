using System.ComponentModel.DataAnnotations;

namespace _1473_VTKNgoc_Buoi3.Models
{
    public class CheckoutViewModel
    {
        [Required(ErrorMessage = "Vui lòng nhập tên người đặt.")]
        public string CustomerName { get; set; } = "";

        [Required(ErrorMessage = "Vui lòng nhập email.")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ.")]
        public string CustomerEmail { get; set; } = "";

        [Required(ErrorMessage = "Vui lòng nhập số điện thoại.")]
        public string CustomerPhone { get; set; } = "";

        [Required(ErrorMessage = "Vui lòng nhập địa chỉ giao hàng.")]
        public string CustomerAddress { get; set; } = "";

        [Required(ErrorMessage = "Vui lòng chọn tỉnh/thành phố.")]
        public string Province { get; set; } = "";

        [Required(ErrorMessage = "Vui lòng chọn quận/huyện.")]
        public string District { get; set; } = "";

        [Required(ErrorMessage = "Vui lòng chọn phường/xã.")]
        public string Ward { get; set; } = "";

        [Required(ErrorMessage = "Vui lòng chọn phương thức thanh toán.")]
        public string PaymentMethod { get; set; } = "COD";

        public string Source { get; set; } = "cart";

        public List<int> SelectedProductIds { get; set; } = new();

        public string? CouponCode { get; set; }

        public List<CartItem> Items { get; set; } = new();

        public decimal Subtotal { get; set; }

        public decimal ShippingFee { get; set; }

        public decimal Discount { get; set; }

        public decimal Total => Math.Max(0, Subtotal + ShippingFee - Discount);
    }
}
