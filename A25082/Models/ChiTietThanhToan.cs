using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace A25082.Models
{
    public class ChiTietThanhToan
    {
        [Key]
        public int MaChiTiet { get; set; }

        // Khóa ngoại tới ThanhToan
        [Required]
        public int MaThanhToan { get; set; }

        [ForeignKey("MaThanhToan")]
        public virtual ThanhToan ThanhToan { get; set; }

        // Khóa ngoại tới SanPhamKemChongNang
        [Required]
        [ForeignKey(nameof(SanPhamKemChongNang))]
        public int MaKem { get; set; }

        public virtual SanPhamKemChongNang SanPhamKemChongNang { get; set; }

        [Required, StringLength(255)]
        public string TenKem { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Số lượng phải lớn hơn 0")]
        public int SoLuong { get; set; }

        [Required]

        public decimal DonGia { get; set; }

        // Thuộc tính không lưu trong DB (computed column trong SQL)
        [NotMapped]
        public decimal ThanhTien => SoLuong * DonGia;
    }
}