using A25082.Models;
using System;
using System.Linq;
using System.Web.Mvc;
using System.Security.Cryptography;
using System.Text;
using System.Data.Entity;
namespace A25082.Admin.Controllers
{
    public class AdminUserController : Controller
    {
        private AppDbContext db = new AppDbContext();

        public ActionResult Index()
        {
            var users = db.NguoiDungs.ToList();
            var roles = db.VaiTros.ToList();
            ViewBag.Roles = roles;
            return View(users);
        }

        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var bytes = Encoding.UTF8.GetBytes(password);
                var hash = sha256.ComputeHash(bytes);
                return Convert.ToBase64String(hash);
            }
        }

        [HttpPost]
        public JsonResult SaveUser(NguoiDungViewModel model)
        {
            if (!ModelState.IsValid)
            {
                // Lấy lỗi cụ thể để gửi về client (nếu muốn)
                var errors = ModelState.Values.SelectMany(v => v.Errors)
                              .Select(e => e.ErrorMessage).ToList();
                return Json(new { success = false, message = "Dữ liệu không hợp lệ: " + string.Join("; ", errors) });
            }

            try
            {
                if (model.MaNguoiDung == 0)
                {
                    // Thêm mới, mật khẩu bắt buộc
                    if (string.IsNullOrEmpty(model.MatKhau))
                        return Json(new { success = false, message = "Mật khẩu không được để trống." });

                    var user = new NguoiDung
                    {
                        TenDangNhap = model.TenDangNhap,
                        Email = model.Email,
                        SoDienThoai = model.SoDienThoai,
                        MaVaiTro = model.MaVaiTro,
                        NgayTao = DateTime.Now,
                        MatKhauHash = HashPassword(model.MatKhau)
                    };

                    db.NguoiDungs.Add(user);
                }
                else
                {
                    // Sửa thông tin user
                    var existingUser = db.NguoiDungs.Find(model.MaNguoiDung);
                    if (existingUser == null)
                        return Json(new { success = false, message = "Người dùng không tồn tại" });

                    existingUser.TenDangNhap = model.TenDangNhap;
                    existingUser.Email = model.Email;
                    existingUser.SoDienThoai = model.SoDienThoai;
                    existingUser.MaVaiTro = model.MaVaiTro;
                    existingUser.NgayCapNhat = DateTime.Now;

                    // Nếu có mật khẩu mới, hash và cập nhật
                    if (!string.IsNullOrEmpty(model.MatKhau))
                    {
                        existingUser.MatKhauHash = HashPassword(model.MatKhau);
                    }
                }

                db.SaveChanges();
                return Json(new { success = true, message = "Lưu thành công" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Có lỗi xảy ra: " + ex.Message });
            }
        }

        [HttpPost]
        public JsonResult DeleteUser(int id)
        {
            var user = db.NguoiDungs.Find(id);
            if (user == null)
                return Json(new { success = false, message = "Người dùng không tồn tại" });

            db.NguoiDungs.Remove(user);
            db.SaveChanges();

            return Json(new { success = true, message = "Xóa thành công" });
        }



    }
}
