using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace A25082.Models
{
    public class LoaiAnh
    {
        [Key]
        public int MaLoaiAnh { get; set; }
        public string TenLoaiAnh { get; set; }
       
        public ICollection<Slider> Sliders { get; set; }
        public ICollection<AnhKemChongNang> AnhKemChongNangs { get; set; }
    }
}