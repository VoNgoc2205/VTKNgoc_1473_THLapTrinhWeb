using _1473_VTKNgoc_Buoi3.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace _1473_VTKNgoc_Buoi3.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWebHostEnvironment _environment;

        public AccountController(
            UserManager<ApplicationUser> userManager,
            IWebHostEnvironment environment)
        {
            _userManager = userManager;
            _environment = environment;
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Profile(ApplicationUser model, IFormFile? AvatarFile)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return Redirect("/Identity/Account/Login");
            }

            user.FullName = model.FullName;
            user.PhoneNumber = model.PhoneNumber;
            user.Address = model.Address;
            user.Gender = model.Gender;
            user.BirthDate = model.BirthDate;

            if (AvatarFile != null && AvatarFile.Length > 0)
            {
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
                var extension = Path.GetExtension(AvatarFile.FileName).ToLower();

                if (!allowedExtensions.Contains(extension))
                {
                    TempData["Error"] = "Chỉ được tải ảnh JPG, PNG, GIF hoặc WEBP.";
                    return RedirectToAction("Profile");
                }

                var uploadsFolder = Path.Combine(
                    _environment.WebRootPath,
                    "images",
                    "avatars"
                );

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

                user.AvatarUrl = $"/images/avatars/{fileName}";
            }

            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                TempData["Success"] = "Cập nhật thông tin thành công!";
            }
            else
            {
                TempData["Error"] = "Cập nhật thất bại.";
            }

            return RedirectToAction("Profile");
        }
    }
}