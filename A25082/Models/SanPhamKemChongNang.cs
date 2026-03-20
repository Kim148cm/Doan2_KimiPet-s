using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace A25082.Models
{
    public class SanPhamKemChongNang
    {
        [Key]
        public int MaKem { get; set; }
        public string TenKem { get; set; }
        public string MoTa { get; set; }
        public decimal GiaGoc { get; set; }
        public decimal GiaGiam { get; set; }
        public int SoLuongTon { get; set; }
        public int MaLoai { get; set; }
        public int? MaLoaiAnh { get; set; }

        public LoaiKemChongNang LoaiKemChongNang { get; set; }
        public LoaiAnh LoaiAnh { get; set; }

        public ICollection<AnhKemChongNang> AnhKemChongNangs { get; set; }

    }
}