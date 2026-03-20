using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace A25082.Models
{
    public class LoaiKemChongNang
    {
        [Key]
        public int MaLoai { get; set; }
        public string TenLoai { get; set; }

        public ICollection<SanPhamKemChongNang> SanPhamKemChongNangs { get; set; }
    }
}