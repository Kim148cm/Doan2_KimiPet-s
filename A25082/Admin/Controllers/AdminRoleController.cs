using A25082.Models;
using System.Linq;
using System.Web.Mvc;
using System.Data.Entity;
namespace A25082.Admin.Controllers
{
    public class AdminRoleController : Controller
    {
        private AppDbContext db = new AppDbContext();

        // Hiển thị danh sách Roles
        public ActionResult Index()
        {
            var roles = db.VaiTros.ToList();
            return View(roles);
        }

        // Thêm hoặc Cập nhật Role (AJAX)
        [HttpPost]
        public JsonResult SaveRole(int? MaVaiTro, string TenVaiTro)
        {
            if (string.IsNullOrEmpty(TenVaiTro))
            {
                return Json(new { success = false, message = "Tên vai trò không được để trống!" });
            }

            if (MaVaiTro == null || MaVaiTro == 0) // Thêm mới
            {
                if (db.VaiTros.Any(r => r.TenVaiTro == TenVaiTro))
                {
                    return Json(new { success = false, message = "Vai trò này đã tồn tại!" });
                }

                VaiTro newRole = new VaiTro { TenVaiTro = TenVaiTro };
                db.VaiTros.Add(newRole);
            }
            else // Cập nhật
            {
                var existingRole = db.VaiTros.Find(MaVaiTro);
                if (existingRole == null)
                {
                    return Json(new { success = false, message = "Vai trò không tồn tại!" });
                }
                existingRole.TenVaiTro = TenVaiTro;
            }

            db.SaveChanges();
            return Json(new { success = true, message = "Lưu vai trò thành công!" });
        }


        // Xóa Role (AJAX)
        [HttpPost]
        public JsonResult DeleteRole(int id)
        {
            var role = db.VaiTros.Include(r => r.NguoiDungs).FirstOrDefault(r => r.MaVaiTro == id);
            if (role == null)
            {
                return Json(new { success = false, message = "Vai trò không tồn tại!" });
            }
            if (role.NguoiDungs.Any())
            {
                return Json(new { success = false, message = "Vai trò đang được sử dụng, không thể xóa!" });
            }
            db.VaiTros.Remove(role);

            db.SaveChanges();

            return Json(new { success = true, message = "Đã xóa vai trò thành công!" });
        }
    }
}
