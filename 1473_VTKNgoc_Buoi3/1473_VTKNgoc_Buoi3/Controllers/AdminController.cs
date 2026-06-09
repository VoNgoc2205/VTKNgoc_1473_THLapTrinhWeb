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

        public async Task<IActionResult> Orders()
        {
            var orders = await _context.Orders
                .Include(o => o.Items)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();

            ViewBag.TotalRevenue = orders.Sum(o => o.TotalAmount);
            ViewBag.ProductRevenue = orders
                .SelectMany(o => o.Items)
                .Where(i => !i.IsService)
                .Sum(i => i.Total);
            ViewBag.ServiceRevenue = orders
                .SelectMany(o => o.Items)
                .Where(i => i.IsService)
                .Sum(i => i.Total);

            return View(orders);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateOrderStatus(int id, string status)
        {
            var allowedStatuses = new[] { "Chờ xác nhận", "Đang xử lý", "Đang giao", "Đã nhận", "Đã hủy" };

            if (!allowedStatuses.Contains(status))
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
    }
}
