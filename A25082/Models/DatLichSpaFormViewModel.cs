using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebCafe.Models
{
    public class DatLichSpaFormViewModel
    {
        public DatLichSpa DatLich { get; set; }
        public List<DichVuSpa> DanhSachDichVu { get; set; }
        public List<LoaiThuCung> DanhSachLoaiThu { get; set; }
    }
}