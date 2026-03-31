using A25082.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using System.Net.Mail;
using System.Net;

namespace A25082.Controllers
{
    public class PayMentController : Controller
    {
        private AppDbContext db = new AppDbContext();

        // lệnh chạy ngrok : ngrok http https://localhost:44379 --host-header=rewrite
        // VNPay Configuration Constants
        private const string VNP_URL = "https://sandbox.vnpayment.vn/paymentv2/vpcpay.html";
        private const string VNP_RETURNURL = "https://26e09830921f.ngrok-free.app/PayMent/VNPayReturn";
        private const string VNP_TMNCODE = "VMQP903J";
        private const string VNP_HASHSECRET = "RHNI1DJKA8DJE4SMXW3556VNXM4ZM56P";

        //----------------------------------------------------------------------------------------------------------------------

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult IndexHistory()
        {
            return View();
        }

        // GET: PayMent
        public ActionResult PayMentCart()
        {
            var cart = Session["Cart"] as List<GioHang>;
            if (cart == null || !cart.Any())
            {
                return RedirectToAction("ViewCart");
            }

            var productIdsInCart = cart.Select(c => c.MaKem).ToList();
            var productList = db.SanPhamKemChongNangs.Where(p => productIdsInCart.Contains(p.MaKem)).ToList();

            var outOfStockProducts = cart
               .Where(item =>
               {
                   var product = productList.FirstOrDefault(p => p.MaKem == item.MaKem);
                   return product != null && item.SoLuong > product.SoLuongTon;
               })
               .Select(item => item.TenKem)
               .ToList();

            ViewBag.OutOfStockProducts = outOfStockProducts;
            var voucherCode = Session["VoucherCode"] as string;
            var voucherDiscount = Session["VoucherDiscount"] as decimal? ?? 0;

            decimal cartTotal = cart.Sum(c => c.TongThanhToan);
            decimal finalTotal = cartTotal - voucherDiscount;

            ViewBag.VoucherCode = voucherCode;
            ViewBag.VoucherDiscount = voucherDiscount;
            ViewBag.TotalAmount = finalTotal;

            return View(cart);
        }

        //----------------------------------------------------------------------------------------------------------------------
        [HttpPost]
        public ActionResult ConfirmOrder(string FullName, string Email, string PhoneNumber, string Address,
            string Province, string District, string Ward, string PaymentMethod)
        {
            var cart = Session["Cart"] as List<GioHang>;
            if (cart == null || !cart.Any())
            {
                TempData["ErrorMessage"] = "Giỏ hàng của bạn đang trống.";
                return RedirectToAction("ViewCart", "GioHang");
            }

            var productIdsInCart = cart.Select(item => item.MaKem).ToList();

            var validProductIds = db.SanPhamKemChongNangs
                .Where(p => productIdsInCart.Contains(p.MaKem))
                .Select(p => p.MaKem)
                .ToList();

            var invalidProducts = cart.Where(item => !validProductIds.Contains(item.MaKem)).ToList();

            if (invalidProducts.Any())
            {
                string invalidProductIds = string.Join(", ", invalidProducts.Select(p => p.MaKem));
                TempData["ErrorMessage"] = $"Sản phẩm không hợp lệ (ID: {invalidProductIds})!";
                return RedirectToAction("ViewCart", "GioHang");
            }

            var productList = db.SanPhamKemChongNangs
                .Where(p => productIdsInCart.Contains(p.MaKem))
                .ToList();

            foreach (var item in cart)
            {
                var product = productList.FirstOrDefault(p => p.MaKem == item.MaKem);
                if (product == null || product.SoLuongTon < item.SoLuong)
                {
                    TempData["ErrorMessage"] = $"Sản phẩm {product?.TenKem ?? "Không xác định"} không đủ hàng!";
                    return RedirectToAction("Index", "Home");
                }
            }

            string productNames = string.Join(", ", cart.Select(item =>
            {
                var product = productList.FirstOrDefault(p => p.MaKem == item.MaKem);
                return product != null ? product.TenKem : "Sản phẩm không xác định";
            }));

            decimal totalAmount = cart.Sum(item => item.SoLuong * item.GiaGoc);

            decimal discountApplied = Session["VoucherDiscount"] as decimal? ?? 0;
            string voucherName = Session["VoucherCode"] as string ?? "Không có voucher";

            decimal finalAmount = totalAmount - discountApplied;
            if (finalAmount < 0) finalAmount = 0;

            int totalQuantity = cart.Sum(item => (int)item.SoLuong);

            // VNPay Payment
            if (PaymentMethod == "bank")
            {
                var userVnPay = Session["UserId"] as int?;

                var paymentPay = new ThanhToan
                {
                    MaNguoiDung = userVnPay,
                    SoLuong = totalQuantity,
                    HoTen = FullName,
                    Email = Email,
                    SoDienThoai = PhoneNumber,
                    DiaChi = Address,
                    Tinh = Province,
                    Huyen = District,
                    Phuong = Ward,
                    PhuongThucThanhToan = "VNPay",
                    TrangThai = "Chờ thanh toán VNPay",
                    SoTienThanhToan = finalAmount,
                    NgayTao = DateTime.Now
                };
                db.ThanhToans.Add(paymentPay);
                db.SaveChanges();

                //  LƯU CHI TIẾT SẢN PHẨM NGAY KHI TẠO ĐỢN HÀNG
                foreach (var item in cart)
                {
                    var product = db.SanPhamKemChongNangs.FirstOrDefault(p => p.MaKem == item.MaKem);
                    if (product != null)
                    {
                        var orderDetail = new ChiTietThanhToan
                        {
                            MaThanhToan = paymentPay.MaThanhToan,
                            MaKem = product.MaKem,
                            TenKem = product.TenKem,
                            SoLuong = item.SoLuong,
                            DonGia = product.GiaGoc
                        };
                        db.ChiTietThanhToans.Add(orderDetail);
                    }
                }
                db.SaveChanges(); //  Lưu chi tiết sản phẩm

                var pay = new VnPayLibrary();
                pay.AddRequestData("vnp_Version", "2.1.0");
                pay.AddRequestData("vnp_Command", "pay");
                pay.AddRequestData("vnp_TmnCode", VNP_TMNCODE);
                pay.AddRequestData("vnp_Amount", ((long)(finalAmount * 100)).ToString());
                pay.AddRequestData("vnp_CreateDate", DateTime.Now.ToString("yyyyMMddHHmmss"));
                pay.AddRequestData("vnp_CurrCode", "VND");
                pay.AddRequestData("vnp_IpAddr", GetClientIpAddress());
                pay.AddRequestData("vnp_Locale", "vn");
                pay.AddRequestData("vnp_OrderInfo", "Thanh toan don hang #" + paymentPay.MaThanhToan);
                pay.AddRequestData("vnp_OrderType", "other");
                pay.AddRequestData("vnp_ReturnUrl", VNP_RETURNURL);
                pay.AddRequestData("vnp_TxnRef", paymentPay.MaThanhToan.ToString());

                string paymentUrl = pay.CreateRequestUrl(VNP_URL, VNP_HASHSECRET);
                return Redirect(paymentUrl);
            }

            if (PaymentMethod == "vietqr")
            {
                return RedirectToAction("VietQR", "PayMent");
            }
            // COD hoặc phương thức khác
            var userId = Session["UserId"] as int?;
            var payment = new ThanhToan
            {
                MaNguoiDung = userId,
                HoTen = FullName,
                Email = Email,
                SoDienThoai = PhoneNumber,
                DiaChi = Address,
                Tinh = Province,
                Huyen = District,
                Phuong = Ward,
                PhuongThucThanhToan = PaymentMethod,
                TrangThai = "Chờ xác nhận",
                SoTienThanhToan = finalAmount,
                NgayTao = DateTime.Now,
                SoLuong = totalQuantity
            };

            db.ThanhToans.Add(payment);

            foreach (var item in cart)
            {
                var product = db.SanPhamKemChongNangs.FirstOrDefault(p => p.MaKem == item.MaKem);
                if (product != null)
                {
                    var orderDetail = new ChiTietThanhToan
                    {
                        MaThanhToan = payment.MaThanhToan,
                        MaKem = product.MaKem,
                        TenKem = product.TenKem,
                        SoLuong = item.SoLuong,
                        DonGia = product.GiaGoc
                    };
                    db.ChiTietThanhToans.Add(orderDetail);

                    product.SoLuongTon -= item.SoLuong;
                }
            }

            db.SaveChanges();

            // GỬI EMAIL XÁC NHẬN
            try
            {
                string fromEmail = "thaithienkim365@gmail.com";
                string fromPassword = "vbfxtnjnlurdcuzm";

                string subject = "Xác nhận đơn hàng - Kimipet's";

                string body = $@"
                <div style='font-family: Arial, sans-serif; padding: 20px; background-color:#f8f9fa;'>
                    <h2 style='color: #28a745; text-align:center;'>Kimipet's - Xác nhận đơn hàng</h2>
                    <p>Xin chào <b>{FullName}</b>,</p>
                    <p>Cảm ơn bạn đã tin tưởng và đặt hàng tại <b>Kimipet's</b>! Dưới đây là thông tin chi tiết đơn hàng của bạn:</p>

                    <table style='width:100%; border-collapse: collapse; margin-top: 15px;'>
                        <tr style='background-color: #28a745; color: white;'>
                            <th style='padding: 10px; border: 1px solid #ddd;'>Mã đơn</th>
                            <th style='padding: 10px; border: 1px solid #ddd;'>Sản phẩm</th>
                            <th style='padding: 10px; border: 1px solid #ddd;'>Tổng SL</th>                         
                            <th style='padding: 10px; border: 1px solid #ddd;'>Thành tiền</th>
                            <th style='padding: 10px; border: 1px solid #ddd;'>Thanh toán</th>
                        </tr>
                        <tr style='background-color: #fff; text-align:center;'>
                            <td style='padding: 10px; border: 1px solid #ddd;'>{payment.MaThanhToan}</td>
                            <td style='padding: 10px; border: 1px solid #ddd;'>{string.Join("<br/>", cart.Select(c => c.TenKem + " x" + c.SoLuong))}</td>
                            <td style='padding: 10px; border: 1px solid #ddd;'>{totalQuantity}</td>                        
                            <td style='padding: 10px; border: 1px solid #ddd; color:#dc3545; font-weight:bold;'>{finalAmount:#,##0}đ</td>
                            <td style='padding: 10px; border: 1px solid #ddd;'>{PaymentMethod}</td>
                        </tr>
                    </table>

                    <p style='margin-top:20px;'>📍 <b>Địa chỉ giao hàng:</b><br/>
                    {Address}, {Ward}, {District}, {Province}</p>

                    <p style='margin-top:20px; text-align:center;'>
                        <span style='font-size:16px;'>💚 Chúng tôi sẽ sớm liên hệ và giao hàng cho bạn.</span>
                    </p>

                    <hr style='margin:20px 0;'/>
                    <p style='text-align:center; font-size:14px; color:#6c757d;'>
                        © 2026 Kimipet's | Cần Thơ<br/>
                        Hân hạnh phục vụ quý khách!
                    </p>
                </div>";

                MailMessage mail = new MailMessage();
                mail.From = new MailAddress(fromEmail, "Kimipet's");
                mail.To.Add(new MailAddress(Email));
                mail.Subject = subject;
                mail.Body = body;
                mail.IsBodyHtml = true;

                using (SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587))
                {
                    smtp.Credentials = new NetworkCredential(fromEmail, fromPassword);
                    smtp.EnableSsl = true;
                    smtp.Send(mail);
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Đặt hàng thành công nhưng gửi email thất bại: " + ex.Message;
            }

            Session["Cart"] = null;
            Session["VoucherDiscount"] = null;
            Session["VoucherCode"] = null;

            TempData["OrderSuccess"] = "Đơn hàng của bạn đã được xác nhận thành công!";
            return RedirectToAction("IndexHistory");
        }

        // ==================== FIX VNPAY RETURN ====================
        public ActionResult VNPayReturn()
        {
            var vnpData = new VnPayLibrary();

            foreach (string key in Request.QueryString)
            {
                if (!string.IsNullOrEmpty(Request.QueryString[key]) &&
                    key != "vnp_SecureHash" && key != "vnp_SecureHashType")
                {
                    vnpData.AddResponseData(key, Request.QueryString[key]);
                }
            }

            string vnp_SecureHash = Request.QueryString["vnp_SecureHash"];
            bool checkSignature = vnpData.ValidateSignature(vnp_SecureHash, VNP_HASHSECRET);

            if (!checkSignature)
            {
                TempData["ErrorMessage"] = "Chữ ký không hợp lệ!";
                return RedirectToAction("Index", "Home");
            }

            string vnp_ResponseCode = Request.QueryString["vnp_ResponseCode"];
            string txnRef = Request.QueryString["vnp_TxnRef"];

            var order = db.ThanhToans
                .Include(o => o.ChiTietThanhToans) // ✅ Load chi tiết luôn
                .FirstOrDefault(p => p.MaThanhToan.ToString() == txnRef);

            if (order == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy đơn hàng!";
                return RedirectToAction("PayMentCart");
            }

            if (vnp_ResponseCode == "00")
            {
                // ✅ CHỈ CẬP NHẬT TRẠNG THÁI VÀ TRỪ TỒN KHO
                order.TrangThai = "Đã xác nhận";

                // Trừ tồn kho
                foreach (var detail in order.ChiTietThanhToans)
                {
                    var product = db.SanPhamKemChongNangs.FirstOrDefault(p => p.MaKem == detail.MaKem);
                    if (product != null)
                    {
                        product.SoLuongTon -= detail.SoLuong;
                    }
                }

                db.SaveChanges();

                // ✅ GỬI EMAIL
                try
                {
                    string fromEmail = "thaithienkim365@gmail.com";
                    string fromPassword = "vbfxtnjnlurdcuzm";
                    string subject = "✅ Thanh toán thành công - Kimipet's";

                    string productList = string.Join("",
                        order.ChiTietThanhToans.Select(d => $"<li>{d.TenKem} x{d.SoLuong}</li>"));

                    string body = $@"
            <div style='font-family: Arial, sans-serif; padding: 20px; background-color:#f8f9fa;'>
                <h2 style='color: #28a745; text-align:center;'>Kimipet's - Thanh toán thành công!</h2>
                <p>Xin chào <b>{order.HoTen}</b>,</p>
                <p>Thanh toán VNPay của bạn đã được xác nhận thành công! 🎉</p>

                <h3 style='color: #28a745;'>Thông tin đơn hàng:</h3>
                <table style='width:100%; border-collapse: collapse;'>
                    <tr style='background-color: #28a745; color: white;'>
                        <th style='padding: 10px; border: 1px solid #ddd;'>Mã đơn</th>
                        <th style='padding: 10px; border: 1px solid #ddd;'>Sản phẩm</th>
                        <th style='padding: 10px; border: 1px solid #ddd;'>Thành tiền</th>
                    </tr>
                    <tr style='background-color: #fff;'>
                        <td style='padding: 10px; border: 1px solid #ddd;'>{order.MaThanhToan}</td>
                        <td style='padding: 10px; border: 1px solid #ddd;'><ul style='margin:0;'>{productList}</ul></td>
                        <td style='padding: 10px; border: 1px solid #ddd; color:#dc3545; font-weight:bold;'>{order.SoTienThanhToan:#,##0}đ</td>
                    </tr>
                </table>

                <p style='margin-top:20px;'>📍 <b>Địa chỉ giao hàng:</b><br/>{order.DiaChi}, {order.Phuong}, {order.Huyen}, {order.Tinh}</p>

                <p style='margin-top:20px; text-align:center; color:#28a745;'>
                    <b>Cảm ơn bạn đã tin tưởng Kimipet's! Chúng tôi sẽ giao hàng sớm nhất.</b>
                </p>

                <hr style='margin:20px 0;'/>
                <p style='text-align:center; font-size:14px; color:#6c757d;'>
                    © 2026 Kimipet's | Cần Thơ
                </p>
            </div>";

                    MailMessage mail = new MailMessage();
                    mail.From = new MailAddress(fromEmail, "Kimipet's");
                    mail.To.Add(new MailAddress(order.Email));
                    mail.Subject = subject;
                    mail.Body = body;
                    mail.IsBodyHtml = true;

                    using (SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587))
                    {
                        smtp.Credentials = new NetworkCredential(fromEmail, fromPassword);
                        smtp.EnableSsl = true;
                        smtp.Send(mail);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Lỗi gửi email: " + ex.Message);
                }

                TempData["OrderSuccess"] = "Thanh toán VNPay thành công!";
                Session["Cart"] = null;
                Session["VoucherDiscount"] = null;
                Session["VoucherCode"] = null;

                return RedirectToAction("IndexHistory");
            }
            else
            {
                order.TrangThai = "Thanh toán thất bại";
                db.SaveChanges();
                TempData["ErrorMessage"] = "Thanh toán VNPay thất bại. Mã lỗi: " + vnp_ResponseCode;
                return RedirectToAction("PayMentCart");
            }
        }
        public ActionResult VietQR()
        {
            var cart = Session["Cart"] as List<GioHang>;
            if (cart == null || !cart.Any())
                return RedirectToAction("Index", "GioHang");

            var voucherDiscount = Session["VoucherDiscount"] as decimal? ?? 0m;
            var voucherCode = Session["VoucherCode"] as string ?? "";

            decimal subtotal = cart.Sum(c => c.SoLuong * (c.GiaGoc > 0 ? c.GiaGoc : c.GiaGiam));
            decimal total = subtotal - voucherDiscount;
            if (total < 0) total = 0;

            ViewBag.TotalAmount = total;
            ViewBag.VoucherDiscount = voucherDiscount;
            ViewBag.VoucherCode = voucherCode;

            return View("VietQR", cart);   // Views/PayMent/VietQR.cshtml
        }
        public ActionResult OrderSuccess()
        {
            var lastOrder = db.ThanhToans.OrderByDescending(p => p.NgayTao).FirstOrDefault();

            if (lastOrder == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy đơn hàng!";
                return RedirectToAction("ViewCart", "GioHang");
            }

            return View(lastOrder);
        }

        public ActionResult OrderHistory(string email)
        {
            int? userId = Session["UserId"] as int?;

            if (userId.HasValue)
            {
                var userOrders = db.ThanhToans
                    .Include(p => p.ChiTietThanhToans)
                    .Where(p => p.MaNguoiDung == userId)
                    .OrderByDescending(p => p.NgayTao)
                    .ToList();

                return View(userOrders);
            }
            else
            {
                var guestOrders = db.ThanhToans
                    .Include(p => p.ChiTietThanhToans)
                    .Where(p => p.MaNguoiDung == null)
                    .OrderByDescending(p => p.NgayTao)
                    .ToList();

                return View(guestOrders);
            }
        }

        private string GetClientIpAddress()
        {
            if (!string.IsNullOrEmpty(Request.ServerVariables["HTTP_CF_CONNECTING_IP"]))
                return Request.ServerVariables["HTTP_CF_CONNECTING_IP"];

            if (!string.IsNullOrEmpty(Request.ServerVariables["HTTP_X_FORWARDED_FOR"]))
                return Request.ServerVariables["HTTP_X_FORWARDED_FOR"];

            if (!string.IsNullOrEmpty(Request.ServerVariables["HTTP_X_FORWARDED"]))
                return Request.ServerVariables["HTTP_X_FORWARDED"];

            if (!string.IsNullOrEmpty(Request.ServerVariables["HTTP_FORWARDED_FOR"]))
                return Request.ServerVariables["HTTP_FORWARDED_FOR"];

            if (!string.IsNullOrEmpty(Request.ServerVariables["HTTP_FORWARDED"]))
                return Request.ServerVariables["HTTP_FORWARDED"];

            if (!string.IsNullOrEmpty(Request.UserHostAddress))
                return Request.UserHostAddress;

            return "0.0.0.0";
        }
    }
}
