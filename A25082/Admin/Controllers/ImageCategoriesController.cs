using A25082.Models;
using System;
using System.Linq;
using System.Web.Mvc;

namespace A25082.Admin.Controllers
{
    public class ImageCategoriesController : Controller
    {
        private readonly AppDbContext db = new AppDbContext();

        // 📌 Lấy danh sách danh mục hình ảnh
        public ActionResult Index()
        {
            var categories = db.LoaiAnhs.ToList();
            return View(categories);
        }

        // 📌 Thêm hoặc cập nhật danh mục hình ảnh
        [HttpPost]
        public JsonResult SaveCategory(int MaLoaiAnh, string TenLoaiAnh)
        {
            if (string.IsNullOrWhiteSpace(TenLoaiAnh))
            {
                return Json(new { success = false, message = "Tên danh mục không được để trống!" });
            }

            if (MaLoaiAnh == 0)
            {
                var newCategory = new LoaiAnh { TenLoaiAnh = TenLoaiAnh };
                db.LoaiAnhs.Add(newCategory);
            }
            else
            {
                var category = db.LoaiAnhs.FirstOrDefault(c => c.MaLoaiAnh == MaLoaiAnh);
                if (category == null)
                    return Json(new { success = false, message = "Danh mục không tồn tại!" });

                category.TenLoaiAnh = TenLoaiAnh;
            }

            db.SaveChanges();
            return Json(new { success = true });
        }


        // 📌 Xóa danh mục hình ảnh
        [HttpPost]
        public JsonResult DeleteCategory(int MaLoaiAnh)
        {
            var category = db.LoaiAnhs.FirstOrDefault(c => c.MaLoaiAnh == MaLoaiAnh);
            if (category == null)
            {
                return Json(new { success = false, message = "Danh mục không tồn tại!" });
            }

            db.LoaiAnhs.Remove(category);
            db.SaveChanges();
            return Json(new { success = true });
        }

    }
}
