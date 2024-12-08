using APIwebmoi.Models;

namespace ApiWebManagement.DTO
{
    public class NguoiDungDTO
    {
        public string IdUser { get; set; }
        public string? Email { get; set; }    // Nullable
        public string? Pass { get; set; }     // Nullable
        public string? Username { get; set; } // Nullable
        public string? Avatar { get; set; }   // Nullable
        public int? IdRole { get; set; }      // Nullable int
        public bool? IsActive { get; set; }   // Nullable bool
        public string? Fullname { get; set; } // Nullable
    }

}
