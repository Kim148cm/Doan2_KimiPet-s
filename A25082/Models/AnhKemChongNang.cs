using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
namespace A25082.Models
{
    public class AnhKemChongNang
    {
        [Key]
        public int MaAnhKem { get; set; }
        public int MaKem { get; set; }
        public string ImageUrl { get; set; }
        public int MaLoaiAnh { get; set; }

        // LIÊN KẾT KHÓA ===
        public SanPhamKemChongNang SanPhamKemChongNang { get; set; }
        public LoaiAnh LoaiAnh { get; set; }
    }
}