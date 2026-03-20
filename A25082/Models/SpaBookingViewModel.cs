using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebCafe.Models
{
    public class SpaBookingViewModel
    {
        public List<DatLichSpa> DanhSachLich { get; set; }
        public int TongLich { get; set; }
        public int ChoXacNhan { get; set; }
        public int DaXacNhan { get; set; }
        public int HoanThanh { get; set; }
        public int DaHuy { get; set; }
        public string FilterStatus { get; set; }
        public string FilterDate { get; set; }
    }
}