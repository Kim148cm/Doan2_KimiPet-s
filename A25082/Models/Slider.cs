using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace A25082.Models
{
    public class Slider
    {
        [Key]
        public int MaSlider { get; set; }
        public string ImageUrl { get; set; }
        public int MaLoaiAnh { get; set; }


        public LoaiAnh LoaiAnh { get; set; }
    }
}