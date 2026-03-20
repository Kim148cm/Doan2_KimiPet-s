using A25082.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace A25082.Admin.Controllers
{
    public class AdminProductController : Controller
    {
        private AppDbContext db = new AppDbContext();

        public ActionResult Index()
        {
            var products = db.SanPhamKemChongNangs.Include("LoaiKemChongNang").ToList();
            ViewBag.Categories = db.LoaiKemChongNangs?.ToList() ?? new List<LoaiKemChongNang>();
            return View(products);
        }

  
        [HttpGet]
        public JsonResult GetProduct(int id)
        {
            try
            {
                var product = db.SanPhamKemChongNangs.FirstOrDefault(p => p.MaKem == id);
                if (product == null)
                {
                    return Json(new { success = false, message = "Sản phẩm không tồn tại!" }, JsonRequestBehavior.AllowGet);
                }

                return Json(new
                {
                    success = true,
                    data = new
                    {
                        id = product.MaKem,
                        name = product.TenKem,
                        description = product.MoTa ?? "",
                        price = product.GiaGoc,
                        disprice = product.GiaGiam,
                        stock = product.SoLuongTon,
                        categoryId = product.MaLoai
                    }
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Lỗi hệ thống: " + ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult SaveProduct(int Id, string Name, string Description, decimal Price, decimal Disprice, int Stock, int CategoryId)
        {
            try
            {
                // Validate input
                if (string.IsNullOrWhiteSpace(Name))
                {
                    return Json(new { success = false, message = "Tên sản phẩm không được để trống!" });
                }

                if (Price <= 0)
                {
                    return Json(new { success = false, message = "Giá sản phẩm phải lớn hơn 0!" });
                }

                if (Stock < 0)
                {
                    return Json(new { success = false, message = "Số lượng không được âm!" });
                }

                if (CategoryId <= 0)
                {
                    return Json(new { success = false, message = "Vui lòng chọn danh mục!" });
                }

                var categoryExists = db.LoaiKemChongNangs.Any(c => c.MaLoai == CategoryId);
                if (!categoryExists)
                {
                    return Json(new { success = false, message = "Danh mục không tồn tại!" });
                }

                if (Id == 0)
                {
                    var newProduct = new SanPhamKemChongNang
                    {
                        TenKem = Name.Trim(),
                        MoTa = Description?.Trim() ?? "",
                        GiaGoc = Price,
                        GiaGiam = Disprice,
                        SoLuongTon = Stock,
                        MaLoai = CategoryId
                    };
                    db.SanPhamKemChongNangs.Add(newProduct);
                    db.SaveChanges();
                    return Json(new { success = true, message = "Thêm sản phẩm thành công!" });
                }
                else
                {
                    var existingProduct = db.SanPhamKemChongNangs.FirstOrDefault(p => p.MaKem == Id);
                    if (existingProduct == null)
                    {
                        return Json(new { success = false, message = "Sản phẩm không tồn tại!" });
                    }

                    existingProduct.TenKem = Name.Trim();
                    existingProduct.MoTa = Description?.Trim() ?? "";
                    existingProduct.GiaGoc = Price;
                    existingProduct.GiaGiam = Disprice;
                    existingProduct.SoLuongTon = Stock;
                    existingProduct.MaLoai = CategoryId;

                    db.SaveChanges();
                    return Json(new { success = true, message = "Cập nhật sản phẩm thành công!" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Lỗi hệ thống: " + ex.Message });
            }
        }

        [HttpPost]
        public JsonResult DeleteProduct(int id)
        {
            try
            {
                var product = db.SanPhamKemChongNangs.Find(id);
                if (product == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy sản phẩm!" });
                }

                db.SanPhamKemChongNangs.Remove(product);
                db.SaveChanges();

                return Json(new { success = true, message = "Xóa sản phẩm thành công!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Lỗi hệ thống: " + ex.Message });
            }
        }
    }
}