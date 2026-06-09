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

        public IActionResult Index()
        {
            var cart = HttpContext.Session.GetObject<List<CartItem>>(CartKey)
                       ?? new List<CartItem>();

            return View(cart);
        }

        public async Task<IActionResult> AddToCart(int id)
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
                    Quantity = 1,
                    IsService = false
                });
            }
            else
            {
                item.Quantity++;
            }

            HttpContext.Session.SetObject(CartKey, cart);
            return RedirectToAction(nameof(Index));
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
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Checkout()
        {
            var cart = HttpContext.Session.GetObject<List<CartItem>>(CartKey)
                       ?? new List<CartItem>();

            if (!cart.Any())
            {
                return RedirectToAction(nameof(Index));
            }

            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return Redirect("/Identity/Account/Login");
            }

            var order = new Order
            {
                UserId = user.Id,
                CustomerName = string.IsNullOrWhiteSpace(user.FullName) ? user.Email ?? "" : user.FullName,
                CustomerEmail = user.Email ?? "",
                CustomerPhone = user.PhoneNumber,
                CreatedAt = DateTime.Now,
                TotalAmount = cart.Sum(x => x.Total),
                Status = cart.Any(x => !x.IsService) ? "Chờ xác nhận" : "Chờ xác nhận lịch",
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

            HttpContext.Session.Remove(CartKey);
            TempData["Success"] = $"Đặt hàng thành công. Mã đơn hàng #{order.Id}.";

            return RedirectToAction(nameof(Index));
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
    }
}
