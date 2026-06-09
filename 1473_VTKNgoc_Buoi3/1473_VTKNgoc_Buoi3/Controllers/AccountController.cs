using _1473_VTKNgoc_Buoi3.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace _1473_VTKNgoc_Buoi3.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWebHostEnvironment _environment;
        private readonly ApplicationDbContext _context;

        public AccountController(
            UserManager<ApplicationUser> userManager,
            IWebHostEnvironment environment,
            ApplicationDbContext context)
        {
            _userManager = userManager;
            _environment = environment;
            _context = context;
        }

        public async Task<IActionResult> Profile()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return Redirect("/Identity/Account/Login");
            }

            return View(user);
        }

        public async Task<IActionResult> OrderHistory()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return Redirect("/Identity/Account/Login");
            }

            var orders = await _context.Orders
                .Include(o => o.Items)
                .Where(o => o.UserId == user.Id && o.Items.Any(i => !i.IsService))
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();

            return View(orders);
        }

        public async Task<IActionResult> ServiceHistory()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return Redirect("/Identity/Account/Login");
            }

            var orders = await _context.Orders
                .Include(o => o.Items)
                .Where(o => o.UserId == user.Id && o.Items.Any(i => i.IsService))
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();

            return View(orders);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CancelOrder(int id)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return Redirect("/Identity/Account/Login");
            }

            var order = await _context.Orders
                .Include(o => o.Items)
                .FirstOrDefaultAsync(o => o.Id == id && o.UserId == user.Id && o.Items.Any(i => !i.IsService));

            if (order == null)
            {
                return NotFound();
            }

            var cancelableStatuses = new[] { "Chờ xác nhận", "Đang xử lý" };

            if (!cancelableStatuses.Contains(order.Status))
            {
                TempData["Error"] = "Đơn hàng này không thể hủy ở trạng thái hiện tại.";
                return RedirectToAction(nameof(OrderHistory));
            }

            order.Status = "Đã hủy";
            await _context.SaveChangesAsync();

            TempData["Success"] = $"Đã hủy đơn hàng #{order.Id}.";
            return RedirectToAction(nameof(OrderHistory));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CancelService(int id)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return Redirect("/Identity/Account/Login");
            }

            var item = await _context.OrderItems
                .Include(i => i.Order)
                .FirstOrDefaultAsync(i => i.Id == id && i.IsService && i.Order != null && i.Order.UserId == user.Id);

            if (item == null)
            {
                return NotFound();
            }

            var cancelableStatuses = new[] { "Chờ xác nhận lịch", "Đã xác nhận" };

            if (!cancelableStatuses.Contains(item.ItemStatus))
            {
                TempData["Error"] = "Lịch hẹn này không thể hủy ở trạng thái hiện tại.";
                return RedirectToAction(nameof(ServiceHistory));
            }

            item.ItemStatus = "Đã hủy";
            await _context.SaveChangesAsync();

            TempData["Success"] = $"Đã hủy lịch hẹn #{item.Id}.";
            return RedirectToAction(nameof(ServiceHistory));
        }

        public IActionResult ChangePassword()
        {
            return View(new ChangePasswordViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return Redirect("/Identity/Account/Login");
            }

            var result = await _userManager.ChangePasswordAsync(
                user,
                model.CurrentPassword,
                model.NewPassword
            );

            if (result.Succeeded)
            {
                TempData["Success"] = "Đổi mật khẩu thành công.";
                return RedirectToAction(nameof(Profile));
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Profile(ApplicationUser model, IFormFile? AvatarFile, bool removeAvatar = false)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return Redirect("/Identity/Account/Login");
            }

            if (string.IsNullOrWhiteSpace(model.FullName))
            {
                TempData["Error"] = "Vui lòng nhập họ và tên.";
                return RedirectToAction(nameof(Profile));
            }

            user.FullName = model.FullName;
            user.PhoneNumber = model.PhoneNumber;
            user.Address = model.Address;
            user.Gender = model.Gender;
            user.BirthDate = model.BirthDate;

            if (removeAvatar)
            {
                DeleteLocalAvatar(user.AvatarUrl);
                user.AvatarUrl = null;
            }
            else if (AvatarFile != null && AvatarFile.Length > 0)
            {
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
                var extension = Path.GetExtension(AvatarFile.FileName).ToLower();

                if (!allowedExtensions.Contains(extension))
                {
                    TempData["Error"] = "Chỉ được tải ảnh JPG, PNG, GIF hoặc WEBP.";
                    return RedirectToAction(nameof(Profile));
                }

                var uploadsFolder = Path.Combine(_environment.WebRootPath, "images", "avatars");

                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                var fileName = $"{Guid.NewGuid()}{extension}";
                var filePath = Path.Combine(uploadsFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await AvatarFile.CopyToAsync(stream);
                }

                DeleteLocalAvatar(user.AvatarUrl);
                user.AvatarUrl = $"/images/avatars/{fileName}";
            }

            var result = await _userManager.UpdateAsync(user);

            TempData[result.Succeeded ? "Success" : "Error"] =
                result.Succeeded ? "Cập nhật thông tin thành công!" : "Cập nhật thất bại.";

            return RedirectToAction(nameof(Profile));
        }

        private void DeleteLocalAvatar(string? avatarUrl)
        {
            if (string.IsNullOrWhiteSpace(avatarUrl) ||
                !avatarUrl.StartsWith("/images/avatars/", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            var fileName = Path.GetFileName(avatarUrl);
            var filePath = Path.Combine(_environment.WebRootPath, "images", "avatars", fileName);

            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }
        }
    }
}
