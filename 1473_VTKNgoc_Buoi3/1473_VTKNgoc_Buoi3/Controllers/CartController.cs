using _1473_VTKNgoc_Buoi3.Extensions;
using _1473_VTKNgoc_Buoi3.Models;
using _1473_VTKNgoc_Buoi3.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace _1473_VTKNgoc_Buoi3.Controllers
{
    [Authorize]
    public class CartController : Controller
    {
        private const string CartKey = "Cart";
        private const string BuyNowKey = "BuyNowCart";
        private const string CouponKey = "CartCoupon";
        private const string BuyNowCouponKey = "BuyNowCoupon";
        private readonly IProductRepository _productRepository;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public CartController(
            IProductRepository productRepository,
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager)
        {
            _productRepository = productRepository;
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var cart = HttpContext.Session.GetObject<List<CartItem>>(CartKey)
                       ?? new List<CartItem>();

            var products = await _productRepository.GetAllAsync();
            var cartIds = cart.Select(x => x.ProductId).ToHashSet();
            ViewBag.Suggestions = products
                .Where(p => !cartIds.Contains(p.Id) && !IsServiceProduct(p))
                .Take(3)
                .ToList();

            return View(cart);
        }

        public async Task<IActionResult> AddToCart(int id, int quantity = 1)
        {
            var product = await _productRepository.GetByIdAsync(id);

            if (product == null)
            {
                return NotFound();
            }

            if (IsServiceProduct(product))
            {
                return RedirectToAction(nameof(BookService), new { id });
            }

            quantity = Math.Clamp(quantity, 1, 99);

            var cart = HttpContext.Session.GetObject<List<CartItem>>(CartKey)
                       ?? new List<CartItem>();

            var item = cart.FirstOrDefault(x => x.ProductId == id && !x.IsService);

            if (item == null)
            {
                cart.Add(new CartItem
                {
                    ProductId = product.Id,
                    ProductName = product.Name,
                    ImageUrl = product.ImageUrl,
                    Price = product.Price,
                    Quantity = quantity,
                    IsService = false
                });
            }
            else
            {
                item.Quantity += quantity;
            }

            HttpContext.Session.SetObject(CartKey, cart);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> BuyNow(int id, int quantity = 1)
        {
            var product = await _productRepository.GetByIdAsync(id);

            if (product == null)
            {
                return NotFound();
            }

            if (IsServiceProduct(product))
            {
                return RedirectToAction(nameof(BookService), new { id });
            }

            quantity = Math.Clamp(quantity, 1, 99);

            var items = new List<CartItem>
            {
                new()
                {
                    ProductId = product.Id,
                    ProductName = product.Name,
                    ImageUrl = product.ImageUrl,
                    Price = product.Price,
                    Quantity = quantity,
                    IsService = false
                }
            };

            HttpContext.Session.SetObject(BuyNowKey, items);
            return RedirectToAction(nameof(Checkout), new { source = "buy-now" });
        }

        public async Task<IActionResult> BookService(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);

            if (product == null)
            {
                return NotFound();
            }

            if (!IsServiceProduct(product))
            {
                return RedirectToAction(nameof(AddToCart), new { id });
            }

            return View(new ServiceBookingViewModel
            {
                ProductId = product.Id,
                ServiceName = product.Name,
                ImageUrl = product.ImageUrl,
                Price = product.Price,
                AppointmentAt = DateTime.Now.AddDays(1).Date.AddHours(9)
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BookService(ServiceBookingViewModel model)
        {
            var product = await _productRepository.GetByIdAsync(model.ProductId);

            if (product == null)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                model.ServiceName = product.Name;
                model.ImageUrl = product.ImageUrl;
                model.Price = product.Price;
                return View(model);
            }

            var cart = HttpContext.Session.GetObject<List<CartItem>>(CartKey)
                       ?? new List<CartItem>();

            cart.Add(new CartItem
            {
                ProductId = product.Id,
                ProductName = product.Name,
                ImageUrl = product.ImageUrl,
                Price = product.Price,
                Quantity = 1,
                IsService = true,
                AppointmentAt = model.AppointmentAt,
                PetName = model.PetName,
                PetType = model.PetType,
                PetAge = model.PetAge,
                PetBreed = model.PetBreed,
                CustomerNote = model.CustomerNote
            });

            HttpContext.Session.SetObject(CartKey, cart);
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Increase(int id)
        {
            var cart = HttpContext.Session.GetObject<List<CartItem>>(CartKey)
                       ?? new List<CartItem>();

            var item = cart.FirstOrDefault(x => x.ProductId == id && !x.IsService);

            if (item != null)
            {
                item.Quantity++;
            }

            HttpContext.Session.SetObject(CartKey, cart);
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Decrease(int id)
        {
            var cart = HttpContext.Session.GetObject<List<CartItem>>(CartKey)
                       ?? new List<CartItem>();

            var item = cart.FirstOrDefault(x => x.ProductId == id && !x.IsService);

            if (item != null)
            {
                item.Quantity--;

                if (item.Quantity <= 0)
                {
                    cart.Remove(item);
                }
            }

            HttpContext.Session.SetObject(CartKey, cart);
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Remove(int id)
        {
            var cart = HttpContext.Session.GetObject<List<CartItem>>(CartKey)
                       ?? new List<CartItem>();

            var item = cart.FirstOrDefault(x => x.ProductId == id);

            if (item != null)
            {
                cart.Remove(item);
            }

            HttpContext.Session.SetObject(CartKey, cart);
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Clear()
        {
            HttpContext.Session.Remove(CartKey);
            HttpContext.Session.Remove(CouponKey);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ApplyCoupon(string? couponCode, string source = "cart", List<int>? selectedProductIds = null)
        {
            couponCode = couponCode?.Trim().ToUpperInvariant();

            if (string.IsNullOrWhiteSpace(couponCode))
            {
                HttpContext.Session.Remove(GetCouponSessionKey(source));
                TempData["Error"] = "Vui lòng nhập mã khuyến mãi.";
                return RedirectToAction(nameof(Checkout), BuildCheckoutRouteValues(source, selectedProductIds));
            }

            if (!IsValidCoupon(couponCode))
            {
                HttpContext.Session.Remove(GetCouponSessionKey(source));
                TempData["Error"] = "Mã khuyến mãi không hợp lệ.";
                return RedirectToAction(nameof(Checkout), BuildCheckoutRouteValues(source, selectedProductIds));
            }

            HttpContext.Session.SetString(GetCouponSessionKey(source), couponCode);
            TempData["Success"] = $"Đã áp dụng mã {couponCode}.";
            return RedirectToAction(nameof(Checkout), BuildCheckoutRouteValues(source, selectedProductIds));
        }

        public IActionResult RemoveCoupon(string source = "cart", List<int>? selectedProductIds = null)
        {
            HttpContext.Session.Remove(GetCouponSessionKey(source));
            return RedirectToAction(nameof(Checkout), BuildCheckoutRouteValues(source, selectedProductIds));
        }

        public async Task<IActionResult> Checkout(string source = "cart", List<int>? selectedProductIds = null)
        {
            var allItems = GetCheckoutItems(source);
            var cart = FilterCheckoutItems(allItems, source, selectedProductIds);

            if (!cart.Any())
            {
                TempData["Error"] = "Vui lòng chọn ít nhất một sản phẩm để thanh toán.";
                return RedirectToAction(nameof(Index));
            }

            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return Redirect("/Identity/Account/Login");
            }

            var model = new CheckoutViewModel
            {
                CustomerName = string.IsNullOrWhiteSpace(user.FullName) ? user.Email ?? "" : user.FullName,
                CustomerEmail = user.Email ?? "",
                CustomerPhone = user.PhoneNumber ?? "",
                CustomerAddress = user.Address ?? "",
                Source = source,
                SelectedProductIds = source == "buy-now"
                    ? cart.Select(x => x.ProductId).ToList()
                    : selectedProductIds ?? new List<int>(),
                Items = cart,
                CouponCode = GetCouponCode(source),
                PaymentMethod = "COD"
            };

            SetCheckoutAmounts(model);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Checkout(CheckoutViewModel model)
        {
            var allItems = GetCheckoutItems(model.Source);
            var cart = FilterCheckoutItems(allItems, model.Source, model.SelectedProductIds);

            if (!cart.Any())
            {
                return RedirectToAction(nameof(Index));
            }

            model.Items = cart;
            model.CouponCode = GetCouponCode(model.Source);
            SetCheckoutAmounts(model);

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return Redirect("/Identity/Account/Login");
            }

            var order = new Order
            {
                UserId = user.Id,
                CustomerName = model.CustomerName,
                CustomerEmail = model.CustomerEmail,
                CustomerPhone = model.CustomerPhone,
                CustomerAddress = BuildFullAddress(model),
                PaymentMethod = GetPaymentMethodLabel(model.PaymentMethod),
                CreatedAt = DateTime.Now,
                TotalAmount = model.Total,
                Status = cart.Any(x => !x.IsService) ? OrderStatusOptions.Pending : "Chờ xác nhận lịch",
                Items = cart.Select(x => new OrderItem
                {
                    ProductId = x.ProductId,
                    ProductName = x.ProductName,
                    ImageUrl = x.ImageUrl,
                    Price = x.Price,
                    Quantity = x.Quantity,
                    IsService = x.IsService,
                    ItemStatus = x.IsService ? "Chờ xác nhận lịch" : "Chờ xử lý",
                    AppointmentAt = x.AppointmentAt,
                    PetName = x.PetName,
                    PetType = x.PetType,
                    PetAge = x.PetAge,
                    PetBreed = x.PetBreed,
                    CustomerNote = x.CustomerNote
                }).ToList()
            };

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            VoucherStore.MarkUsed(model.CouponCode);

            if (model.Source == "buy-now")
            {
                HttpContext.Session.Remove(BuyNowKey);
                HttpContext.Session.Remove(BuyNowCouponKey);
            }
            else
            {
                var remainingItems = allItems
                    .Where(x => !model.SelectedProductIds.Contains(x.ProductId))
                    .ToList();

                if (remainingItems.Any())
                {
                    HttpContext.Session.SetObject(CartKey, remainingItems);
                }
                else
                {
                    HttpContext.Session.Remove(CartKey);
                    HttpContext.Session.Remove(CouponKey);
                }
            }

            TempData["Success"] = $"Đặt hàng thành công. Mã đơn hàng #{order.Id}.";
            return RedirectToAction("OrderHistory", "Account");
        }

        public IActionResult Count()
        {
            var cart = HttpContext.Session.GetObject<List<CartItem>>(CartKey)
                       ?? new List<CartItem>();

            return Json(cart.Sum(x => x.Quantity));
        }

        private static bool IsServiceProduct(Product product)
        {
            return product.CategoryId == 4
                || product.Category?.Name.Contains("Dich vu", StringComparison.OrdinalIgnoreCase) == true
                || product.Category?.Name.Contains("Dịch vụ", StringComparison.OrdinalIgnoreCase) == true;
        }

        private List<CartItem> GetCheckoutItems(string source)
        {
            var key = source == "buy-now" ? BuyNowKey : CartKey;
            return HttpContext.Session.GetObject<List<CartItem>>(key) ?? new List<CartItem>();
        }

        private static List<CartItem> FilterCheckoutItems(List<CartItem> items, string source, List<int>? selectedProductIds)
        {
            if (source == "buy-now")
            {
                return items;
            }

            if (selectedProductIds == null || !selectedProductIds.Any())
            {
                return new List<CartItem>();
            }

            return items
                .Where(x => selectedProductIds.Contains(x.ProductId))
                .ToList();
        }

        private static RouteValueDictionary BuildCheckoutRouteValues(string source, List<int>? selectedProductIds)
        {
            var routeValues = new RouteValueDictionary
            {
                ["source"] = source
            };

            for (var i = 0; i < selectedProductIds?.Count; i++)
            {
                routeValues[$"selectedProductIds[{i}]"] = selectedProductIds[i];
            }

            return routeValues;
        }

        private string? GetCouponCode(string source)
        {
            return HttpContext.Session.GetString(GetCouponSessionKey(source));
        }

        private static string GetCouponSessionKey(string source)
        {
            return source == "buy-now" ? BuyNowCouponKey : CouponKey;
        }

        private static bool IsValidCoupon(string couponCode)
        {
            var voucher = VoucherStore.Find(couponCode);
            return voucher is { IsAvailable: true };
        }

        private static decimal GetShippingFee(decimal subtotal, string? province)
        {
            if (subtotal <= 0)
            {
                return 0;
            }

            if (string.IsNullOrWhiteSpace(province))
            {
                return 30000;
            }

            var normalized = province.Trim().ToLowerInvariant();

            if (normalized.Contains("hồ chí minh") || normalized.Contains("há»“ chĂ­ minh") || normalized.Contains("ho chi minh"))
            {
                return 30000;
            }

            if (normalized.Contains("bình dương") || normalized.Contains("bĂ¬nh dÆ°Æ¡ng")
                || normalized.Contains("đồng nai") || normalized.Contains("Ä‘á»“ng nai")
                || normalized.Contains("long an"))
            {
                return 40000;
            }

            if (normalized.Contains("bà rịa") || normalized.Contains("bĂ  rá»‹a")
                || normalized.Contains("tây ninh") || normalized.Contains("tĂ¢y ninh")
                || normalized.Contains("tiền giang") || normalized.Contains("tiá»n giang"))
            {
                return 50000;
            }

            if (normalized.Contains("hà nội") || normalized.Contains("hĂ  ná»™i")
                || normalized.Contains("đà nẵng") || normalized.Contains("Ä‘Ă  náºµng")
                || normalized.Contains("cần thơ") || normalized.Contains("cáº§n thÆ¡")
                || normalized.Contains("hải phòng") || normalized.Contains("háº£i phĂ²ng")
                || normalized.Contains("huế") || normalized.Contains("huáº¿"))
            {
                return 70000;
            }

            return 90000;
        }

        private static decimal GetDiscount(decimal subtotal, decimal shippingFee, string? couponCode)
        {
            return VoucherStore.CalculateDiscount(couponCode, subtotal, shippingFee);
        }

        private void SetCheckoutAmounts(List<CartItem> cart, string? couponCode)
        {
            var subtotal = cart.Sum(x => x.Total);
            var shippingFee = GetShippingFee(subtotal, null);

            ViewBag.Subtotal = subtotal;
            ViewBag.ShippingFee = shippingFee;
            ViewBag.Discount = GetDiscount(subtotal, shippingFee, couponCode);
        }

        private static void SetCheckoutAmounts(CheckoutViewModel model)
        {
            model.Subtotal = model.Items.Sum(x => x.Total);
            model.ShippingFee = GetShippingFee(model.Subtotal, model.Province);
            model.Discount = GetDiscount(model.Subtotal, model.ShippingFee, model.CouponCode);
        }

        private static string BuildFullAddress(CheckoutViewModel model)
        {
            var streetAddress = model.CustomerAddress?.Trim() ?? "";

            if (streetAddress.Contains("@"))
            {
                streetAddress = "";
            }

            return string.Join(", ", new[]
            {
                streetAddress,
                model.Ward,
                model.District,
                model.Province
            }.Where(x => !string.IsNullOrWhiteSpace(x)));
        }

        private static string GetPaymentMethodLabel(string paymentMethod)
        {
            return paymentMethod switch
            {
                "CreditCard" => "Thẻ tín dụng/Thẻ ghi nợ",
                "EWallet" => "Ví điện tử",
                "BankTransfer" => "Chuyển khoản ngân hàng",
                _ => "Thanh toán khi nhận hàng"
            };
        }
    }
}
