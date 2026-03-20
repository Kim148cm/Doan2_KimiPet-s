using A25082.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace A25082.Admin.Controllers
{
    public class AdminImageSliderController : Controller
    {
        private AppDbContext db = new AppDbContext();

        public ActionResult Index()
        {
            var sliders = db.Sliders.ToList();
            ViewBag.ImageCategories = db.LoaiAnhs.ToList();
            return View(sliders);
        }


        // CODE ADD NEW DATA --------------------------------

        [HttpPost]
        public JsonResult SaveSlider(int Id, HttpPostedFileBase ImageFile, string ImageUrl, int ImageCategoryId)
        {
            if (!db.LoaiAnhs.Any(l => l.MaLoaiAnh == ImageCategoryId))
                return Json(new { success = false, message = "Danh mục hình ảnh không tồn tại!" });

            string imageUrl = ImageUrl;

            if (ImageFile != null && ImageFile.ContentLength > 0)
            {
                string fileName = Path.GetFileName(ImageFile.FileName);
                string path = Path.Combine(Server.MapPath("~/Assets/image/"), fileName);
                ImageFile.SaveAs(path);
                imageUrl = "/Assets/image/" + fileName;
            }

            if (string.IsNullOrEmpty(imageUrl))
                return Json(new { success = false, message = "Vui lòng chọn hình ảnh!" });

            if (Id == 0)
            {
                // Kiểm tra trùng URL để tránh thêm ảnh giống nhau
                if (db.Sliders.Any(s => s.ImageUrl == imageUrl && s.MaLoaiAnh == ImageCategoryId))
                    return Json(new { success = false, message = "Hình ảnh này đã tồn tại!" });

                var newSlider = new Slider
                {
                    ImageUrl = imageUrl,
                    MaLoaiAnh = ImageCategoryId
                };
                db.Sliders.Add(newSlider);
            }
            else
            {
                var existingSlider = db.Sliders.Find(Id);
                if (existingSlider == null)
                    return Json(new { success = false, message = "Hình ảnh không tồn tại!" });

                existingSlider.ImageUrl = imageUrl;
                existingSlider.MaLoaiAnh = ImageCategoryId;
            }

            db.SaveChanges();
            return Json(new { success = true, imageUrl });
        }


        // CODE DELETE DATA ---------------------------------------------

        [HttpPost]
        public JsonResult DeleteSlider(int id)
        {
            var slider = db.Sliders.Find(id);
            if (slider == null)
                return Json(new { success = false, message = "Hình ảnh không tồn tại!" });

            db.Sliders.Remove(slider);
            db.SaveChanges();
            return Json(new { success = true });
        }


        // -------------------------------------------------------------------
    }
}
