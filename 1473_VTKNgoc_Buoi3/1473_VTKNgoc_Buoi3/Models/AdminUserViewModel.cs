namespace _1473_VTKNgoc_Buoi3.Models
{
    public class AdminUserViewModel
    {
        public string Id { get; set; } = "";

        public string FullName { get; set; } = "";

        public string Email { get; set; } = "";

        public string? PhoneNumber { get; set; }

        public string Roles { get; set; } = "";

        public bool IsLocked { get; set; }

        public DateTimeOffset? LockoutEnd { get; set; }
    }
}
