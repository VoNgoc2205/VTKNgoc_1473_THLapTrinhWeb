namespace _1473_VTKNgoc_Buoi3.Models
{
    public static class VoucherStore
    {
        private static readonly List<Voucher> Vouchers = new()
        {
            new Voucher { Code = "PET10", DiscountType = "Percent", DiscountValue = 10, MinimumOrderAmount = 0, Quantity = 100, ExpiresAt = DateTime.Today.AddMonths(1) },
            new Voucher { Code = "PET50", DiscountType = "Amount", DiscountValue = 50000, MinimumOrderAmount = 300000, Quantity = 50, ExpiresAt = DateTime.Today.AddMonths(1) },
            new Voucher { Code = "FREESHIP", DiscountType = "FreeShip", DiscountValue = 0, MinimumOrderAmount = 0, Quantity = 80, ExpiresAt = DateTime.Today.AddMonths(1) }
        };

        public static IReadOnlyList<Voucher> GetAll()
        {
            lock (Vouchers)
            {
                return Vouchers
                    .OrderByDescending(v => v.IsAvailable)
                    .ThenBy(v => v.IsExpired)
                    .ThenBy(v => v.Code)
                    .ToList();
            }
        }

        public static Voucher? Find(string? code)
        {
            if (string.IsNullOrWhiteSpace(code))
            {
                return null;
            }

            lock (Vouchers)
            {
                return Vouchers.FirstOrDefault(v =>
                    v.Code.Equals(code.Trim(), StringComparison.OrdinalIgnoreCase));
            }
        }

        public static void Save(Voucher voucher)
        {
            voucher.Code = voucher.Code.Trim().ToUpperInvariant();

            lock (Vouchers)
            {
                var existing = Vouchers.FirstOrDefault(v =>
                    v.Code.Equals(voucher.Code, StringComparison.OrdinalIgnoreCase));

                if (existing == null)
                {
                    Vouchers.Add(voucher);
                    return;
                }

                existing.DiscountType = voucher.DiscountType;
                existing.DiscountValue = voucher.DiscountValue;
                existing.MinimumOrderAmount = voucher.MinimumOrderAmount;
                existing.Quantity = voucher.Quantity;
                existing.ExpiresAt = voucher.ExpiresAt;
                existing.IsActive = voucher.IsActive;
            }
        }

        public static bool CanUse(string? code, decimal subtotal)
        {
            var voucher = Find(code);
            return voucher is { IsAvailable: true } && subtotal >= voucher.MinimumOrderAmount;
        }

        public static void MarkUsed(string? code)
        {
            var voucher = Find(code);

            if (voucher is not { IsAvailable: true })
            {
                return;
            }

            lock (Vouchers)
            {
                if (voucher.Quantity > 0)
                {
                    voucher.Quantity--;
                }
            }
        }

        public static decimal CalculateDiscount(string? code, decimal subtotal, decimal shippingFee)
        {
            var voucher = Find(code);

            if (voucher == null || !voucher.IsAvailable || subtotal < voucher.MinimumOrderAmount)
            {
                return 0;
            }

            return voucher.DiscountType switch
            {
                "Percent" => Math.Round(subtotal * voucher.DiscountValue / 100, 0),
                "Amount" => Math.Min(voucher.DiscountValue, subtotal),
                "FreeShip" => shippingFee,
                _ => 0
            };
        }
    }
}
