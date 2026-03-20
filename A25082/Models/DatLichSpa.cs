using A25082.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebCafe.Models
{
    [Table("DatLichSpa")]
    public class DatLichSpa
    {
        [Key]
        public int MaLich { get; set; }

        public int? MaNguoiDung { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập họ tên")]
        [StringLength(100)]
        [Display(Name = "Họ và tên")]
        public string HoTen { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập email")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        [StringLength(255)]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập số điện thoại")]
        [StringLength(10, MinimumLength = 10, ErrorMessage = "Số điện thoại phải đủ 10 số")]
        [RegularExpression(@"^[0-9]{10}$", ErrorMessage = "Số điện thoại không hợp lệ")]
        [Display(Name = "Số điện thoại")]
        public string SoDienThoai { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tên thú cưng")]
        [StringLength(100)]
        [Display(Name = "Tên thú cưng")]
        public string TenThuCung { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn loại thú cưng")]
        [Display(Name = "Loại thú cưng")]
        public int MaLoaiThu { get; set; }

        [StringLength(100)]
        [Display(Name = "Giống")]
        public string Giong { get; set; }

        [StringLength(50)]
        [Display(Name = "Tuổi")]
        public string TuoiThuCung { get; set; }

        [StringLength(50)]
        [Display(Name = "Cân nặng")]
        public string CanNang { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn dịch vụ")]
        [Display(Name = "Dịch vụ")]
        public int MaDichVu { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn ngày hẹn")]
        [Display(Name = "Ngày hẹn")]
        [DataType(DataType.Date)]
        public DateTime NgayHen { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn giờ hẹn")]
        [Display(Name = "Giờ hẹn")]
        [DataType(DataType.Time)]
        public TimeSpan GioHen { get; set; }

        [StringLength(1000)]
        [Display(Name = "Ghi chú")]
        public string GhiChu { get; set; }

        [StringLength(50)]
        public string TrangThai { get; set; } = "Chờ xác nhận";

        [StringLength(500)]
        public string LyDoHuy { get; set; }

        public DateTime NgayTao { get; set; } = DateTime.Now;

        public DateTime? NgayCapNhat { get; set; }

        [StringLength(500)]
        public string GhiChuAdmin { get; set; }

        // Navigation properties
        [ForeignKey("MaNguoiDung")]
        public virtual NguoiDung NguoiDung { get; set; }

        [ForeignKey("MaDichVu")]
        public virtual DichVuSpa DichVuSpa { get; set; }

        [ForeignKey("MaLoaiThu")]
        public virtual LoaiThuCung LoaiThuCung { get; set; }
    }
}