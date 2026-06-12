namespace _1473_VTKNgoc_Buoi3.Models
{
    public class OrderDetailViewModel
    {
        public Order Order { get; set; } = new();

        public int UserOrderNumber { get; set; }

        public string[] TimelineSteps { get; set; } =
        {
            "Đã xác nhận",
            "Đã gửi hàng",
            "Đang vận chuyển",
            "Đã giao"
        };

        public int CurrentStepIndex { get; set; }
    }
}
