using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace A25082.Models
{
    public class DanhGia
    {
        [Key]
        public int MaDanhGia { get; set; }

        // FK tới NguoiDung
        [Required]
        [ForeignKey("NguoiDung")]
        public int MaNguoiDung { get; set; }

        // FK tới SanPhamKemChongNang
        [Required]
        [ForeignKey("SanPhamKemChongNang")]
        public int MaKem { get; set; }

        [Range(1, 5)]
        public int Diem { get; set; }

        public string BinhLuan { get; set; }

        public DateTime NgayDanhGia { get; set; } = DateTime.Now;

        // Navigation properties
        public virtual NguoiDung NguoiDung { get; set; }
        public virtual SanPhamKemChongNang SanPhamKemChongNang { get; set; }
    }
}