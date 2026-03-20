using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebCafe.Models
{
    [Table("LoaiThuCung")]
    public class LoaiThuCung
    {
        [Key]
        public int MaLoaiThu { get; set; }

        [Required]
        [StringLength(100)]
        public string TenLoaiThu { get; set; }

        // Navigation
        public virtual ICollection<DatLichSpa> DatLichSpas { get; set; }
    }
}