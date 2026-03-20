using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace A25082.Models
{
    public class GioHang
    {
        [Key]
        public int MaGioHang { get; set; }
        public int MaKem { get; set; }
        public string TenKem { get; set; }
        public string ImageUrl { get; set; }
        public decimal GiaGoc { get; set; }
        public decimal GiaGiam { get; set; }
        public decimal TongTien => SoLuong * (GiaGoc > 0 ? GiaGoc : GiaGiam);
        public int SoLuong { get; set; }
        public decimal TongThanhToan { get; set; }

        public SanPhamKemChongNang SanPhamKemChongNang { get; set; }

    }
}