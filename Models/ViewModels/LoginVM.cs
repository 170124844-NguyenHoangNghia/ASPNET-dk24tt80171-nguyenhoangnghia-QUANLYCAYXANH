using System.ComponentModel.DataAnnotations;

namespace QUANLYCAYXANH.Models.ViewModels
{
    public class LoginVM
    {
        [Required]
        public string? TenDangNhap { get; set; }

        [Required]
        public string? MatKhau { get; set; }
    }
}