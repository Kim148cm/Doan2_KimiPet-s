using A25082.Models;
using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web.Mvc;
using System.Data.Entity;
using Microsoft.Owin.Security;
using System.Security.Claims;
using System.Web;
using Microsoft.AspNet.Identity;

namespace A25082.Controllers

{
    public class DangNhapNguoiDungController : Controller
    {
        private AppDbContext db = new AppDbContext();
        private IAuthenticationManager AuthenticationManager => HttpContext.GetOwinContext().Authentication;
        // Đăng nhập
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult DangKyNguoiDung()
        {
            return View();
        }
        // đăng nhập google ----------------------------------------


        // Gọi Google Login
        public ActionResult GoogleLogin()
        {
            var properties = new AuthenticationProperties();  // Để trống hoặc set nếu bạn muốn redirect sau login đến trang cụ thể (ví dụ /Home/Index)

            AuthenticationManager.Challenge(properties, "Google");
            return new HttpUnauthorizedResult();
        }
        public ActionResult GoogleLoginCallback()
        {
            var authManager = HttpContext.GetOwinContext().Authentication;
            var authResult = authManager.AuthenticateAsync(DefaultAuthenticationTypes.ExternalCookie).Result;

            if (authResult == null || authResult.Identity == null)
            {
                TempData["LoginError"] = "Không thể xác thực từ Google. Vui lòng thử lại!";
                return RedirectToAction("Index");
            }

            // Lấy thông tin từ Google
            var claims = authResult.Identity.Claims.ToList();
            string email = claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            string name = claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value ?? "User";

            if (string.IsNullOrEmpty(email))
            {
                TempData["LoginError"] = "Không lấy được email từ Google!";
                return RedirectToAction("Index");
            }

            // Tìm hoặc tạo user
            var user = db.NguoiDungs.FirstOrDefault(u => u.Email == email);
            if (user == null)
            {
                user = new NguoiDung
                {
                    Email = email,
                    TenDangNhap = name,
                    MatKhauHash = Guid.NewGuid().ToString(), // dummy vì dùng external login
                    NgayTao = DateTime.Now,
                    NgayCapNhat = DateTime.Now,
                    MaVaiTro = 2 // user thường
                };
                db.NguoiDungs.Add(user);
                db.SaveChanges();
            }

            // Tạo identity cho OWIN cookie chính (ApplicationCookie)
            var identity = new ClaimsIdentity(DefaultAuthenticationTypes.ApplicationCookie);
            identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, user.MaNguoiDung.ToString()));
            identity.AddClaim(new Claim(ClaimTypes.Name, user.TenDangNhap));
            identity.AddClaim(new Claim(ClaimTypes.Email, user.Email));
            // Thêm claim role nếu cần
            identity.AddClaim(new Claim(ClaimTypes.Role, user.MaVaiTro == 1 ? "Admin" : "User"));

            authManager.SignOut(DefaultAuthenticationTypes.ExternalCookie);           // Xóa tạm
            authManager.SignIn(new AuthenticationProperties { IsPersistent = true }, identity);

            // Session (nếu bạn vẫn dùng session song song)
            Session["User"] = user;
            Session["UserId"] = user.MaNguoiDung;
            Session["UserName"] = user.TenDangNhap;
            Session["UserEmail"] = user.Email;
            Session["UserRole"] = user.MaVaiTro;

            TempData["SuccessMessage"] = "Đăng nhập bằng Google thành công!";

            return user.MaVaiTro == 1
                ? RedirectToAction("ManHinhChinhAdmin", "HomeAdmin")
                : RedirectToAction("Index", "Home");
        }
        // Đăng ký người dùng -----------------------------------------------------------------------------------------------------------------
        [HttpPost]
        public ActionResult DangKyNguoiDung(string Email, string Password, string PhoneNumber, string Address)
        {
            if (db.NguoiDungs.Any(u => u.Email == Email))
            {
                ViewBag.ErrorMessage = "Email đã tồn tại!";
                return View();
            }

            var newUser = new NguoiDung
            {
                Email = Email,
                TenDangNhap = Email.Split('@')[0],  // Lấy phần trước @ làm tên người dùng
                MatKhauHash = HashPassword(Password),
                SoDienThoai = PhoneNumber,
                NgayTao = DateTime.Now,
                NgayCapNhat = DateTime.Now,
                MaVaiTro = 2
            };

            db.NguoiDungs.Add(newUser);
            db.SaveChanges();

            TempData["SuccessMessageRegister"] = "Bạn đã đăng ký thành công !";

            return RedirectToAction("Index", "DangNhapNguoiDung");
        }


        // Hàm mã hóa mật khẩu -----------------------------------------------------------------------------------------------------------------
        private string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(hashedBytes);
            }
        }

        // Hàm đăng nhập thông tin tài khoản -----------------------------------------------------------------------------------------------------------------


        [HttpPost]
        public ActionResult DangNhap(string Email, string Password)
        {
            var user = db.NguoiDungs.FirstOrDefault(u => u.Email == Email);

            if (user == null)
            {
                TempData["LoginError"] = "Email hoặc mật khẩu không chính xác!";
                return RedirectToAction("Index", "DangNhapNguoiDung");
            }

            // So sánh mật khẩu đã hash
            string hashedPassword = HashPassword(Password);
            if (user.MatKhauHash != hashedPassword)
            {
                TempData["LoginError"] = "Email hoặc mật khẩu không chính xác!";
                return RedirectToAction("Index", "DangNhapNguoiDung");
            }

            // Lưu thông tin đăng nhập vào session

            Session["User"] = user;
            Session["UserId"] = user.MaNguoiDung;
            Session["UserName"] = user.TenDangNhap;
            Session["UserEmail"] = user.Email;
            Session["UserRole"] = user.MaVaiTro;

            TempData["SuccessMessage"] = "Bạn đã đăng nhập thành công!";

            // Điều hướng theo quyền ----------------------
            if (user.MaVaiTro == 1)
            {
                return RedirectToAction("ManHinhChinhAdmin", "HomeAdmin");
            }
            else if (user.MaVaiTro == 2)
            {
                return RedirectToAction("Index", "Home");
            }


            TempData["LoginError"] = "Vai trò không hợp lệ!";

            return RedirectToAction("Index");
        }

        // Hàm đăng xuất thông tin tài khoản -----------------------------------------------------------------------------------------------------------------

        public ActionResult DangXuat()
        {
            Session.Clear(); // Xóa toàn bộ session
            return RedirectToAction("Index", "DangNhapNguoiDung"); // Quay lại trang chính
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        // ===================== CHỨC NĂNG ĐÁNH GIÁ ==========================
        [HttpGet]
        public ActionResult DanhGia(int? MaKem) // cho phép null
        {
            // Bắt buộc đăng nhập
            if (Session["UserId"] == null)
            {
                TempData["SweetAlertError"] = "Vui lòng đăng nhập để đánh giá !";
                return RedirectToAction("Index", "DangNhapNguoiDung");
            }

            // Nếu chưa chọn món → trả về danh sách món để chọn
            if (MaKem == null)
            {
                var danhSachMon = db.SanPhamKemChongNangs
              .Include(m => m.LoaiAnh)
              .Include(m => m.AnhKemChongNangs) 
              .ToList();
                return View("ChonMonDanhGia", danhSachMon);
              
            }

           
            var mon = db.SanPhamKemChongNangs
        .Include(m => m.AnhKemChongNangs)
        .FirstOrDefault(m => m.MaKem == MaKem);
            if (mon == null) return HttpNotFound();

            var model = new DanhGiaViewModel
            {
                MaKem = mon.MaKem,
                TenKem = mon.TenKem
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DanhGia(DanhGiaViewModel model)
        {
            if (Session["UserId"] == null)
            {
                TempData["SweetAlertError"] = "Vui lòng đăng nhập để đánh giá!";
                return RedirectToAction("Index", "DangNhapNguoiDung");
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            int maNguoiDung = Convert.ToInt32(Session["UserId"]);

            var danhGia = new DanhGia
            {
                MaNguoiDung = maNguoiDung,
                MaKem = model.MaKem,
                Diem = model.Diem,
                BinhLuan = model.BinhLuan,
                NgayDanhGia = DateTime.Now
            };

            db.DanhGias.Add(danhGia);
            db.SaveChanges();


            TempData["SuccessMessage"] = "Cảm ơn bạn đã gửi đánh giá!";
            return RedirectToAction("DanhGia", new { MaKem = model.MaKem });
        }
        public ActionResult ChonMonDanhGia()
        {
            var SanPhamKemChongNangs = db.SanPhamKemChongNangs
                .Include(m => m.LoaiAnh)
                .Include(m => m.AnhKemChongNangs) 
                .ToList();

            return View(SanPhamKemChongNangs);
        }
        // ===================== CHỨC NĂNG ĐỔI MẬT KHẨU ==========================
        [HttpGet]
        public ActionResult DoiMatKhau()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DoiMatKhau(string Email, string OldPassword, string NewPassword, string ConfirmPassword)
        {
            var user = db.NguoiDungs.FirstOrDefault(u => u.Email == Email);
            if (user == null)
            {
                TempData["SweetAlertError"] = "Không tìm thấy tài khoản với email này!";
                return RedirectToAction("DoiMatKhau");
            }

            // Nếu yêu cầu xác thực mật khẩu cũ
            string hashedOldPassword = HashPassword(OldPassword);
           

            if (NewPassword != ConfirmPassword)
            {
                TempData["SweetAlertError"] = "Mật khẩu mới và xác nhận không khớp!";
                return RedirectToAction("DoiMatKhau");
            }

            // Cập nhật mật khẩu
            user.MatKhauHash = HashPassword(NewPassword);
            user.NgayCapNhat = DateTime.Now;
            db.Entry(user).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();

            TempData["SuccessMessageChange"] = "Đổi mật khẩu thành công !";
            return RedirectToAction("Index", "DangNhapNguoiDung");
        }



        //-------------------------------------------------------------------------------------------------------------------------
    }
}
