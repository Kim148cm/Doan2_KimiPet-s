using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace A25082.Models
{
    public class DanhGiaViewModel
    {
        public int MaKem { get; set; }
        public string TenKem { get; set; }

        [Range(1, 5, ErrorMessage = "Vui lòng chọn số sao từ 1 đến 5")]
        public int Diem { get; set; }

        [StringLength(1000, ErrorMessage = "Bình luận tối đa 1000 ký tự")]
        public string BinhLuan { get; set; }
    }
}