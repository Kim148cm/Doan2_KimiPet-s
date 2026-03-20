using System.Linq;
using System.Web.Mvc;
using System.Data.Entity;
using A25082.Models;
using System;

namespace A25082.Admin.Controllers
{
    public class AdminDanhGiaController : Controller
    {
        private AppDbContext db = new AppDbContext();

        // Hiển thị danh sách đánh giá
        public ActionResult Index()
        {
            var danhGias = db.DanhGias
                .Include(d => d.NguoiDung)
                .Include(d => d.SanPhamKemChongNang)
                .OrderByDescending(d => d.NgayDanhGia)
                .ToList();

            return View(danhGias);
        }

        // Xóa đánh giá
        [HttpPost]
        public JsonResult Delete(int id)
        {
            var dg = db.DanhGias.Find(id);
            if (dg == null)
                return Json(new { success = false, message = "Đánh giá không tồn tại" });

            db.DanhGias.Remove(dg);
            db.SaveChanges();

            return Json(new { success = true, message = "Xóa đánh giá thành công" });
        }
    }
}
