using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebCafe.Models
{
    [Table("DichVuSpa")]
    public class DichVuSpa
    {
        [Key]
        public int MaDichVu { get; set; }

        [Required]
        [StringLength(255)]
        public string TenDichVu { get; set; }

        [StringLength(1000)]
        public string MoTa { get; set; }

        public decimal GiaTien { get; set; }

        /// <summary>Thời gian thực hiện (phút)</summary>
        public int ThoiGian { get; set; }

        public string HinhAnh { get; set; }

        public bool TrangThai { get; set; } = true;

        // Navigation
        public virtual ICollection<DatLichSpa> DatLichSpas { get; set; }
    }
}