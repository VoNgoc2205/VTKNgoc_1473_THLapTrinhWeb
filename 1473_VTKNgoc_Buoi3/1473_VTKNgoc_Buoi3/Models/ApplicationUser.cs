using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace _1473_VTKNgoc_Buoi3.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        public string FullName { get; set; } = "";

        public string? Address { get; set; }

        public DateTime? BirthDate { get; set; }

        public string? Gender { get; set; }

        public string? AvatarUrl { get; set; }
    }
}