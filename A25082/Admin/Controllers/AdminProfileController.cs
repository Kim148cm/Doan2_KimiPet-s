using A25082.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace A25082.Admin.Controllers
{
    public class AdminProfileController : Controller
    {
        // GET: AdminProfile
        private AppDbContext db = new AppDbContext();


        [HttpGet]
        public ActionResult Index()
        {
            int userId = (int)Session["UserId"];

            var user = db.NguoiDungs.FirstOrDefault(u => u.MaNguoiDung == userId);
            if (user == null)
            {
                return RedirectToAction("Index", "DangNhapNguoiDung");
            }

            // Lưu thông tin user vào Session để sử dụng trong View
            Session["User"] = user;


            return View(user);
        }

        [HttpPost]
        public JsonResult DangXuat()
        {
            try
            {
                Session.Clear(); // Xóa toàn bộ session
                return Json(new { success = true });
            }
            catch
            {
                return Json(new { success = false });
            }
        }

    }
}