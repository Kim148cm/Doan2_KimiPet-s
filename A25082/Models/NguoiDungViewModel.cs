using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace A25082.Models
{
    public class NguoiDungViewModel
    {
        public int MaNguoiDung { get; set; }

        [Required(ErrorMessage = "Tên đăng nhập là bắt buộc")]
        public string TenDangNhap { get; set; }

        [Required(ErrorMessage = "Email là bắt buộc")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        public string Email { get; set; }

        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
        public string SoDienThoai { get; set; }

        [Required(ErrorMessage = "Vai trò là bắt buộc")]
        public int MaVaiTro { get; set; }

        // Mật khẩu thô, có thể trống khi sửa
        public string MatKhau { get; set; }
    }
}