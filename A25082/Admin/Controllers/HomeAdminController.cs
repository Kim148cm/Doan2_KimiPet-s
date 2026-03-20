using A25082.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace A25082.Admin.Controllers
{
    public class HomeAdminController : Controller
    {
        private AppDbContext db = new AppDbContext();

        public ActionResult ManHinhChinhAdmin()
        {
            return View();
        }

        public ActionResult DanhMucAdmin()
        {
            var categories = db.LoaiKemChongNangs.ToList();
            return View(categories);
        }

        // ==================== LỌC DỮ LIỆU TÌM KIẾM ====================
        [HttpGet]
        public JsonResult SearchCategory(string keyword)
        {
            var categories = db.LoaiKemChongNangs
                              .Where(c => string.IsNullOrEmpty(keyword) || c.TenLoai.ToLower().Contains(keyword.ToLower()))
                              .Select(c => new
                              {
                                  c.MaLoai,
                                  c.TenLoai
                              })
                              .ToList();

            return Json(categories, JsonRequestBehavior.AllowGet);
        }

        // ==================== LƯU DANH MỤC ====================
        [HttpPost]
        public JsonResult SaveCategory(int Id, string Name)
        {
            try
            {
                if (string.IsNullOrEmpty(Name))
                {
                    return Json(new { success = false, message = "Tên danh mục không được để trống!" });
                }

                if (Id == 0)
                {
                    var newCategory = new LoaiKemChongNang
                    {
                        TenLoai = Name
                    };
                    db.LoaiKemChongNangs.Add(newCategory);
                }
                else
                {
                    var existingCategory = db.LoaiKemChongNangs.Find(Id);
                    if (existingCategory == null)
                        return Json(new { success = false, message = "Danh mục không tồn tại!" });

                    existingCategory.TenLoai = Name;
                }

                db.SaveChanges();
                return Json(new { success = true, message = "Lưu thành công!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Lỗi: " + ex.Message });
            }
        }

        // ==================== XÓA DANH MỤC ====================
        [HttpPost]
        public JsonResult DeleteCategory(int id)
        {
            try
            {
                var category = db.LoaiKemChongNangs.Find(id);
                if (category == null)
                {
                    return Json(new { success = false, message = "Danh mục không tồn tại!" });
                }

                db.LoaiKemChongNangs.Remove(category);
                db.SaveChanges();
                return Json(new { success = true, message = "Xóa thành công!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Lỗi: " + ex.Message });
            }
        }

        // ==================== TẢI DANH SÁCH ĐƠN CHỜ XÁC NHẬN ====================
        [HttpGet]
        public JsonResult GetPendingOrders()
        {
            try
            {
                var pendingOrders = db.ThanhToans
                    .Where(p => p.TrangThai == "Chờ xác nhận")
                    .OrderByDescending(p => p.NgayTao)
                    .ToList()
                    .Select(p => new
                    {
                        PaymentId = p.MaThanhToan,
                        FullName = p.HoTen,
                        PhoneNumber = p.SoDienThoai,
                        TotalAmount = p.SoTienThanhToan,
                        CreatedAt = p.NgayTao.ToString("yyyy-MM-ddTHH:mm:ss"),
                        Status = p.TrangThai,
                        ProductNames = string.Join(", ",
                            db.ChiTietThanhToans
                              .Where(ct => ct.MaThanhToan == p.MaThanhToan)
                              .Select(ct => ct.TenKem + " x" + ct.SoLuong))
                    })
                    .ToList();

                return Json(new { success = true, data = pendingOrders }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        // ==================== TẢI BẢNG HÓA ĐƠN ĐÃ XÁC NHẬN HÔM NAY ====================
        [HttpGet]
        public JsonResult GetConfirmedPaymentsToday()
        {
            try
            {
                var today = DateTime.Today;
                var todayEnd = today.AddDays(1).AddSeconds(-1);

                var confirmedPayments = db.ThanhToans
                    .Where(p => p.TrangThai == "Đã xác nhận" && p.NgayTao >= today && p.NgayTao <= todayEnd)
                    .OrderByDescending(p => p.NgayTao)
                    .ToList()
                    .Select(p => new
                    {
                        PaymentId = p.MaThanhToan,
                        FullName = p.HoTen,
                        Email = p.Email,
                        PhoneNumber = p.SoDienThoai,
                        TotalAmount = p.SoTienThanhToan,
                        PaymentMethod = p.PhuongThucThanhToan,
                        CreatedAt = p.NgayTao.ToString("yyyy-MM-ddTHH:mm:ss"),
                        ProductNames = string.Join(", ",
                            db.ChiTietThanhToans
                              .Where(ct => ct.MaThanhToan == p.MaThanhToan)
                              .Select(ct => ct.TenKem + " x" + ct.SoLuong))
                    })
                    .ToList();

                return Json(new { success = true, data = confirmedPayments }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        // ==================== TỔNG DOANH THU HÔM NAY ====================
        [HttpGet]
        public JsonResult GetTodayRevenue()
        {
            try
            {
                DateTime todayStart = DateTime.Today;
                DateTime todayEnd = todayStart.AddDays(1).AddSeconds(-1);

                var totalRevenue = db.ThanhToans
                    .Where(p => p.TrangThai == "Đã xác nhận" && p.NgayTao >= todayStart && p.NgayTao <= todayEnd)
                    .Sum(p => (decimal?)p.SoTienThanhToan) ?? 0;

                return Json(new { success = true, totalRevenue = totalRevenue }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        // ==================== TỔNG NGƯỜI MUA HÔM NAY ====================
        [HttpGet]
        public JsonResult GetTodayBuyersCount()
        {
            try
            {
                DateTime todayStart = DateTime.Today;
                DateTime todayEnd = todayStart.AddDays(1).AddSeconds(-1);

                int buyerCount = db.ThanhToans
                    .Where(p => p.TrangThai == "Đã xác nhận" && p.NgayTao >= todayStart && p.NgayTao <= todayEnd)
                    .Count();

                return Json(new { success = true, totalBuyers = buyerCount }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        // ==================== TỔNG KHÁCH HÀNG MỚI TRONG TUẦN ====================
        [HttpGet]
        public JsonResult GetNewUsersCount()
        {
            try
            {
                DateTime startOfWeek = DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek);
                DateTime endOfWeek = startOfWeek.AddDays(7).AddSeconds(-1);

                int newUsersCount = db.NguoiDungs
                    .Where(u => u.NgayTao >= startOfWeek && u.NgayTao <= endOfWeek)
                    .Count();

                return Json(new { success = true, newUsers = newUsersCount }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        // ==================== TỔNG SẢN PHẨM GIẢM GIÁ ====================
        [HttpGet]
        public JsonResult GetDiscountedProductsCount()
        {
            try
            {
                int discountedProducts = db.SanPhamKemChongNangs
                    .Where(p => p.GiaGiam > 0 && p.GiaGiam < p.GiaGoc)
                    .Count();

                return Json(new { success = true, discounted = discountedProducts }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        // ==================== DOANH THU THEO THÁNG ====================
        [HttpGet]
        public JsonResult GetMonthlyRevenue()
        {
            try
            {
                var year = DateTime.Now.Year;

                var data = db.ThanhToans
                    .Where(p => p.TrangThai == "Đã xác nhận" && p.NgayTao.Year == year)
                    .GroupBy(p => p.NgayTao.Month)
                    .Select(g => new
                    {
                        Month = g.Key,
                        TotalRevenue = g.Sum(x => x.SoTienThanhToan)
                    })
                    .ToList();

                var monthlyData = Enumerable.Range(1, 12).Select(m => new
                {
                    Month = m,
                    TotalRevenue = data.FirstOrDefault(d => d.Month == m)?.TotalRevenue ?? 0
                });

                return Json(new { success = true, data = monthlyData }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}