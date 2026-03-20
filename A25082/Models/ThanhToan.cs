using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace A25082.Models
{
    public class ThanhToan
    {
        [Key]
        public int MaThanhToan { get; set; }
     
        public int SoLuong { get; set; }    
        public string HoTen { get; set; }
        public string Email { get; set; }
        public string SoDienThoai { get; set; }
        public string DiaChi { get; set; }
        public string Tinh { get; set; }
        public string Huyen { get; set; }
        public string Phuong { get; set; }
        public string PhuongThucThanhToan { get; set; }
        public string TrangThai { get; set; } = "Chờ xác nhận"; // Cài trạng thái mặt định 
        public decimal SoTienThanhToan { get; set; }

        public int? MaNguoiDung { get; set; }



        public DateTime NgayTao { get; set; } = DateTime.Now; // Ngày tạo đơn hàng, mặc định là ngày hiện tại

        public virtual ICollection<ChiTietThanhToan> ChiTietThanhToans { get; set; }
    }
}