using A25082.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;

namespace A25082.Admin.Controllers
{
    public class AdminImageProductController : Controller
    {
        private AppDbContext db = new AppDbContext();

        public ActionResult Index()
        {
            var productImages = db.AnhKemChongNangs
             .Include(a => a.SanPhamKemChongNang)
             .Include(a => a.LoaiAnh)
             .ToList();

            ViewBag.Products = db.SanPhamKemChongNangs
                .Select(p => new SelectListItem { Value = p.MaKem.ToString(), Text = p.TenKem })
                .ToList();

            ViewBag.ImageCategories = db.LoaiAnhs
                .Select(c => new SelectListItem { Value = c.MaLoaiAnh.ToString(), Text = c.TenLoaiAnh })
                .ToList();


            return View(productImages);
        }

        [HttpPost]
        public JsonResult SaveProductImage(int Id, int ProductId, HttpPostedFileBase ImageFile, int ImageCategoryId, int? ProductColorId)
        {
            string imageUrl = null;

            if (ImageFile != null && ImageFile.ContentLength > 0)
            {
                string fileName = Path.GetFileName(ImageFile.FileName);
                string filePath = Path.Combine(Server.MapPath("~/Assets/image/"), fileName);
                ImageFile.SaveAs(filePath);
                imageUrl = "/Assets/image/" + fileName;
            }

            if (Id == 0)
            {
                if (string.IsNullOrEmpty(imageUrl))
                    return Json(new { success = false, message = "Vui lòng chọn hình ảnh!" });

                var newImage = new AnhKemChongNang
                {
                    MaKem = ProductId,
                    ImageUrl = imageUrl,
                    MaLoaiAnh = ImageCategoryId
                };
                db.AnhKemChongNangs.Add(newImage);
            }
            else
            {
                var existingImage = db.AnhKemChongNangs.Find(Id);
                if (existingImage == null)
                    return Json(new { success = false, message = "Hình ảnh không tồn tại!" });

                existingImage.MaKem = ProductId;
                existingImage.MaLoaiAnh = ImageCategoryId;
                if (!string.IsNullOrEmpty(imageUrl))
                    existingImage.ImageUrl = imageUrl;
            }

            db.SaveChanges();
            return Json(new { success = true });
        }


        [HttpPost]
        public JsonResult DeleteProductImage(int id)
        {
            var productImage = db.AnhKemChongNangs.Find(id);

            if (productImage == null)
            {
                return Json(new { success = false, message = "Hình ảnh không tồn tại!" });
            }

            string filePath = Server.MapPath(productImage.ImageUrl);
            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }

            db.AnhKemChongNangs.Remove(productImage);
            db.SaveChanges();
            return Json(new { success = true });
        }
    }
}
