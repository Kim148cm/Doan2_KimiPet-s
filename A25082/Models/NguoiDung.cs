using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace A25082.Models
{
    public class NguoiDung
    {
        [Key]
        public int MaNguoiDung { get; set; }

        [Required, StringLength(100)]
        public string TenDangNhap { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; }

        [Required]
        public string MatKhauHash { get; set; }

        [Phone]
        public string SoDienThoai { get; set; }

        public DateTime NgayTao { get; set; } = DateTime.Now;

        public DateTime? NgayCapNhat { get; set; }

        [Required]
        public int MaVaiTro { get; set; }

        public VaiTro VaiTro { get; set; }
    }
}