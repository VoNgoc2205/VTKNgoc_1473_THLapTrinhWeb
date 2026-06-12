using System.Globalization;
using System.Text;

namespace _1473_VTKNgoc_Buoi3.Models
{
    public static class OrderStatusOptions
    {
        public const string Pending = "Chờ xác nhận";
        public const string Processing = "Đang xử lý";
        public const string Shipped = "Đã gửi";
        public const string Shipping = "Đang vận chuyển";
        public const string Delivered = "Đã giao";
        public const string Cancelled = "Đã hủy";

        public static readonly string[] Statuses =
        {
            Pending,
            Processing,
            Shipped,
            Shipping,
            Delivered,
            Cancelled
        };

        public static readonly (string Value, string Label)[] Filters =
        {
            ("all", "Tất cả"),
            ("pending", Pending),
            ("processing", Processing),
            ("shipped", Shipped),
            ("shipping", Shipping),
            ("delivered", Delivered),
            ("cancelled", Cancelled)
        };

        public static bool IsFinal(string? status)
        {
            if (string.IsNullOrWhiteSpace(status))
            {
                return false;
            }

            var normalized = Normalize(status);

            return normalized.Contains("da giao")
                || normalized.Contains("da nhan")
                || normalized.Contains("hoan thanh")
                || normalized.Contains("huy");
        }

        public static bool IsCancelled(string? status)
        {
            return !string.IsNullOrWhiteSpace(status) && Normalize(status).Contains("huy");
        }

        public static bool IsCancelable(string? status)
        {
            if (IsFinal(status) || string.IsNullOrWhiteSpace(status))
            {
                return false;
            }

            var normalized = Normalize(status);

            return normalized.Contains("cho xac nhan")
                || normalized.Contains("dang xu ly");
        }

        private static string Normalize(string value)
        {
            var normalized = value.Trim().ToLowerInvariant().Normalize(NormalizationForm.FormD);
            var builder = new StringBuilder(normalized.Length);

            foreach (var character in normalized)
            {
                if (CharUnicodeInfo.GetUnicodeCategory(character) != UnicodeCategory.NonSpacingMark)
                {
                    builder.Append(character);
                }
            }

            return builder
                .ToString()
                .Replace('đ', 'd')
                .Replace('Đ', 'd')
                .Normalize(NormalizationForm.FormC);
        }
    }
}
