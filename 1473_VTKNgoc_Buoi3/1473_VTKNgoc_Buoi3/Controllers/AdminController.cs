using System.Text;
using System.Text.Json;
using _1473_VTKNgoc_Buoi3.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace _1473_VTKNgoc_Buoi3.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;

        public AdminController(
            UserManager<ApplicationUser> userManager,
            ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            await LoadRevenueDashboardAsync();
            return View();
        }

        public async Task<IActionResult> Users()
        {
            var users = await _userManager.Users
                .OrderBy(u => u.Email)
                .ToListAsync();

            var model = new List<AdminUserViewModel>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);

                model.Add(new AdminUserViewModel
                {
                    Id = user.Id,
                    FullName = user.FullName,
                    Email = user.Email ?? "",
                    PhoneNumber = user.PhoneNumber,
                    Roles = roles.Any() ? string.Join(", ", roles) : "User",
                    IsLocked = user.LockoutEnd.HasValue && user.LockoutEnd.Value > DateTimeOffset.Now,
                    LockoutEnd = user.LockoutEnd
                });
            }

            return View(model);
        }

        public IActionResult CreateAdmin()
        {
            return View(new CreateAdminViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateAdmin(CreateAdminViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var existingUser = await _userManager.FindByEmailAsync(model.Email);

            if (existingUser != null)
            {
                ModelState.AddModelError(nameof(model.Email), "Email này đã được sử dụng.");
                return View(model);
            }

            var admin = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,
                FullName = model.FullName,
                PhoneNumber = model.PhoneNumber,
                EmailConfirmed = true,
                AvatarUrl = "/images/avatar-default.jpg"
            };

            var result = await _userManager.CreateAsync(admin, model.Password);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }

                return View(model);
            }

            await _userManager.AddToRoleAsync(admin, "Admin");
            TempData["Success"] = $"Đã tạo tài khoản Admin {admin.Email}.";

            return RedirectToAction(nameof(Users));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleUserLock(string id)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            if (await _userManager.IsInRoleAsync(user, "Admin"))
            {
                TempData["Error"] = "Không thể khóa tài khoản Admin.";
                return RedirectToAction(nameof(Users));
            }

            var isLocked = user.LockoutEnd.HasValue && user.LockoutEnd.Value > DateTimeOffset.Now;

            if (isLocked)
            {
                await _userManager.SetLockoutEndDateAsync(user, null);
                TempData["Success"] = $"Đã mở khóa tài khoản {user.Email}.";
            }
            else
            {
                await _userManager.SetLockoutEnabledAsync(user, true);
                await _userManager.SetLockoutEndDateAsync(user, DateTimeOffset.Now.AddYears(100));
                TempData["Success"] = $"Đã khóa tài khoản {user.Email}.";
            }

            return RedirectToAction(nameof(Users));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LockUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            if (await _userManager.IsInRoleAsync(user, "Admin"))
            {
                TempData["Error"] = "Không thể khóa tài khoản Admin.";
                return RedirectToAction(nameof(Users));
            }

            await _userManager.SetLockoutEnabledAsync(user, true);
            await _userManager.SetLockoutEndDateAsync(user, DateTimeOffset.Now.AddYears(100));
            TempData["Success"] = $"Đã khóa tài khoản {user.Email}.";

            return RedirectToAction(nameof(Users));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UnlockUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            await _userManager.SetLockoutEndDateAsync(user, null);
            TempData["Success"] = $"Đã mở khóa tài khoản {user.Email}.";

            return RedirectToAction(nameof(Users));
        }

        public async Task<IActionResult> Orders(string status = "all")
        {
            var orders = await BuildOrdersQuery(status).ToListAsync();
            var allOrders = await _context.Orders.Include(o => o.Items).ToListAsync();
            var completedOrders = allOrders
                .Where(o => o.Status == OrderStatusOptions.Delivered || o.Status == "Đã nhận")
                .ToList();

            ViewBag.Status = status;
            ViewBag.TotalRevenue = completedOrders.Sum(o => o.TotalAmount);
            ViewBag.TotalOrders = allOrders.Count;
            ViewBag.Aov = completedOrders.Any() ? completedOrders.Average(o => o.TotalAmount) : 0;
            ViewBag.ConversionRate = allOrders.Any()
                ? Math.Round(completedOrders.Count * 100m / allOrders.Count, 1)
                : 0;
            ViewBag.ProductRevenue = completedOrders
                .SelectMany(o => o.Items)
                .Where(i => !i.IsService)
                .Sum(i => i.Total);
            ViewBag.ServiceRevenue = completedOrders
                .SelectMany(o => o.Items)
                .Where(i => i.IsService)
                .Sum(i => i.Total);

            var revenueTrend = completedOrders
                .GroupBy(o => o.CreatedAt.Date)
                .OrderBy(g => g.Key)
                .Select(g => new
                {
                    label = g.Key.ToString("dd/MM"),
                    revenue = g.Sum(o => o.TotalAmount)
                })
                .ToList();

            var topProducts = allOrders
                .SelectMany(o => o.Items)
                .Where(i => !i.IsService)
                .GroupBy(i => i.ProductName)
                .Select(g => new TopProductViewModel
                {
                    Name = g.Key,
                    Quantity = g.Sum(i => i.Quantity),
                    Revenue = g.Sum(i => i.Total)
                })
                .OrderByDescending(x => x.Quantity)
                .Take(5)
                .ToList();

            ViewBag.RevenueChartLabels = JsonSerializer.Serialize(revenueTrend.Select(x => x.label));
            ViewBag.RevenueChartValues = JsonSerializer.Serialize(revenueTrend.Select(x => x.revenue));
            ViewBag.TopProducts = topProducts;

            return View(orders);
        }

        public async Task<IActionResult> ExportOrdersCsv(string status = "all")
        {
            var orders = await BuildOrdersQuery(status).ToListAsync();
            var csv = new StringBuilder();
            csv.AppendLine("Ma don,Ngay dat,Khach hang,Email,So dien thoai,Dia chi,Tong tien,Trang thai");

            foreach (var order in orders)
            {
                csv.AppendLine(string.Join(",", new[]
                {
                    EscapeCsv($"#{order.Id}"),
                    EscapeCsv(order.CreatedAt.ToString("dd/MM/yyyy HH:mm")),
                    EscapeCsv(order.CustomerName),
                    EscapeCsv(order.CustomerEmail),
                    EscapeCsv(order.CustomerPhone ?? ""),
                    EscapeCsv(order.CustomerAddress ?? ""),
                    EscapeCsv(order.TotalAmount.ToString("N0")),
                    EscapeCsv(order.Status)
                }));
            }

            return File(Encoding.UTF8.GetBytes(csv.ToString()), "text/csv", $"petlounge-orders-{DateTime.Now:yyyyMMddHHmm}.csv");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateOrderStatus(int id, string status)
        {
            if (!OrderStatusOptions.Statuses.Contains(status))
            {
                TempData["Error"] = "Trạng thái đơn hàng không hợp lệ.";
                return RedirectToAction(nameof(Orders));
            }

            var order = await _context.Orders.FindAsync(id);

            if (order == null)
            {
                return NotFound();
            }

            order.Status = status;
            await _context.SaveChangesAsync();
            TempData["Success"] = $"Đã cập nhật trạng thái đơn hàng #{order.Id}.";

            return RedirectToAction(nameof(Orders));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateServiceStatus(int id, string status)
        {
            var allowedStatuses = new[] { "Chờ xác nhận lịch", "Đã xác nhận", "Đang phục vụ", "Hoàn thành", "Đã hủy" };

            if (!allowedStatuses.Contains(status))
            {
                TempData["Error"] = "Trạng thái lịch dịch vụ không hợp lệ.";
                return RedirectToAction(nameof(Orders));
            }

            var item = await _context.OrderItems.FirstOrDefaultAsync(i => i.Id == id && i.IsService);

            if (item == null)
            {
                return NotFound();
            }

            item.ItemStatus = status;
            await _context.SaveChangesAsync();
            TempData["Success"] = $"Đã cập nhật lịch dịch vụ #{item.Id}.";

            return RedirectToAction(nameof(Orders));
        }

        public IActionResult Vouchers()
        {
            ViewBag.Vouchers = VoucherStore.GetAll();
            return View(new Voucher());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Vouchers(Voucher model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Vouchers = VoucherStore.GetAll();
                return View(model);
            }

            VoucherStore.Save(model);
            TempData["Success"] = $"Đã lưu voucher {model.Code.ToUpperInvariant()}.";
            return RedirectToAction(nameof(Vouchers));
        }

        private IQueryable<Order> BuildOrdersQuery(string status)
        {
            var ordersQuery = _context.Orders
                .Include(o => o.Items)
                .OrderByDescending(o => o.CreatedAt)
                .AsQueryable();

            return status switch
            {
                "pending" => ordersQuery.Where(o => o.Status == OrderStatusOptions.Pending || o.Status == "Đã xác nhận"),
                "processing" => ordersQuery.Where(o => o.Status == OrderStatusOptions.Processing),
                "shipped" => ordersQuery.Where(o => o.Status == OrderStatusOptions.Shipped || o.Status == "Đã gửi hàng"),
                "shipping" => ordersQuery.Where(o => o.Status == OrderStatusOptions.Shipping || o.Status == "Đang giao"),
                "delivered" => ordersQuery.Where(o => o.Status == OrderStatusOptions.Delivered || o.Status == "Đã nhận"),
                "cancelled" => ordersQuery.Where(o => o.Status == OrderStatusOptions.Cancelled),
                _ => ordersQuery
            };
        }

        private async Task LoadRevenueDashboardAsync()
        {
            var allOrders = await _context.Orders.Include(o => o.Items).ToListAsync();
            var completedOrders = allOrders
                .Where(o => OrderStatusOptions.IsFinal(o.Status) && !OrderStatusOptions.IsCancelled(o.Status))
                .ToList();

            var revenueTrend = completedOrders
                .GroupBy(o => o.CreatedAt.Date)
                .OrderBy(g => g.Key)
                .Select(g => new
                {
                    label = g.Key.ToString("dd/MM"),
                    revenue = g.Sum(o => o.TotalAmount)
                })
                .ToList();

            var topProducts = allOrders
                .SelectMany(o => o.Items)
                .Where(i => !i.IsService)
                .GroupBy(i => i.ProductName)
                .Select(g => new TopProductViewModel
                {
                    Name = g.Key,
                    Quantity = g.Sum(i => i.Quantity),
                    Revenue = g.Sum(i => i.Total)
                })
                .OrderByDescending(x => x.Quantity)
                .ThenByDescending(x => x.Revenue)
                .Take(6)
                .ToList();

            ViewBag.TotalRevenue = completedOrders.Sum(o => o.TotalAmount);
            ViewBag.TotalOrders = allOrders.Count;
            ViewBag.CompletedOrders = completedOrders.Count;
            ViewBag.PendingOrders = allOrders.Count(o => !OrderStatusOptions.IsFinal(o.Status));
            ViewBag.CancelledOrders = allOrders.Count(o => OrderStatusOptions.IsCancelled(o.Status));
            ViewBag.Aov = completedOrders.Any() ? completedOrders.Average(o => o.TotalAmount) : 0;
            ViewBag.ConversionRate = allOrders.Any()
                ? Math.Round(completedOrders.Count * 100m / allOrders.Count, 1)
                : 0;
            ViewBag.ProductRevenue = completedOrders
                .SelectMany(o => o.Items)
                .Where(i => !i.IsService)
                .Sum(i => i.Total);
            ViewBag.ServiceRevenue = completedOrders
                .SelectMany(o => o.Items)
                .Where(i => i.IsService)
                .Sum(i => i.Total);
            ViewBag.RecentOrders = allOrders
                .OrderByDescending(o => o.CreatedAt)
                .Take(5)
                .ToList();
            ViewBag.RevenueChartLabels = JsonSerializer.Serialize(revenueTrend.Select(x => x.label));
            ViewBag.RevenueChartValues = JsonSerializer.Serialize(revenueTrend.Select(x => x.revenue));
            ViewBag.TopProducts = topProducts;
        }

        private static string EscapeCsv(string value)
        {
            return $"\"{value.Replace("\"", "\"\"")}\"";
        }
    }
}
