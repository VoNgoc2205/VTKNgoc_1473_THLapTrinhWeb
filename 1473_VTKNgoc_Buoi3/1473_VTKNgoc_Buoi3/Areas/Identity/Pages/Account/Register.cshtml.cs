using _1473_VTKNgoc_Buoi3.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace _1473_VTKNgoc_Buoi3.Areas.Identity.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public RegisterModel(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [BindProperty]
        public InputModel Input { get; set; } = new();

        public string ErrorMessage { get; set; } = "";

        public class InputModel
        {
            [Required(ErrorMessage = "Vui lòng nhập họ và tên")]
            public string FullName { get; set; } = "";

            [Required(ErrorMessage = "Vui lòng nhập email")]
            [EmailAddress(ErrorMessage = "Email không hợp lệ")]
            public string Email { get; set; } = "";

            [Required(ErrorMessage = "Vui lòng nhập số điện thoại")]
            public string PhoneNumber { get; set; } = "";

            public string? Address { get; set; }

            public string? Gender { get; set; }

            public IFormFile? AvatarFile { get; set; }
            public DateTime? BirthDate { get; set; }

            [Required(ErrorMessage = "Vui lòng nhập mật khẩu")]
            [DataType(DataType.Password)]
            public string Password { get; set; } = "";

            [Required(ErrorMessage = "Vui lòng xác nhận mật khẩu")]
            [DataType(DataType.Password)]
            public string ConfirmPassword { get; set; } = "";
        }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            if (Input.Password != Input.ConfirmPassword)
            {
                ErrorMessage = "Mật khẩu xác nhận không khớp.";
                return Page();
            }

            var avatarUrl = "/images/avatar-default.jpg";

            if (Input.AvatarFile != null && Input.AvatarFile.Length > 0)
            {
                var uploadsFolder = Path.Combine(
                    Directory.GetCurrentDirectory(),
                    "wwwroot",
                    "images",
                    "avatars"
                );

                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(Input.AvatarFile.FileName);
                var filePath = Path.Combine(uploadsFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await Input.AvatarFile.CopyToAsync(stream);
                }

                avatarUrl = "/images/avatars/" + fileName;
            }

            var user = new ApplicationUser
            {
                UserName = Input.Email,
                Email = Input.Email,
                PhoneNumber = Input.PhoneNumber,
                EmailConfirmed = true,

                FullName = Input.FullName,
                Address = Input.Address,
                Gender = Input.Gender,
                BirthDate = Input.BirthDate,
                AvatarUrl = avatarUrl
            };

            var result = await _userManager.CreateAsync(user, Input.Password);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "User");
                await _signInManager.SignInAsync(user, isPersistent: false);
                return LocalRedirect("~/");
            }

            ErrorMessage = string.Join(" ", result.Errors.Select(e => e.Description));
            return Page();
        }
    }
}