

using System.Net;
using System.Net.Mail;
using WebCafe.Models;


namespace A25082.Helpers
{
    public static class EmailHelper
    {
        private const string FromEmail = "thaithienkim365@gmail.com";  // mail chủ gửi <---
        private const string FromName = "Kimipet's Spa";
        private const string SmtpPassword = "vbfxtnjnlurdcuzm";   // Mã pass tài khoản <---
        private const string SmtpHost = "smtp.gmail.com";
        private const int SmtpPort = 587;

       
        public static bool SendBookingConfirmation(
            string toEmail,
            string toName,
            WebCafe.Models.DatLichSpa lich,  
            string trangThai)
        {
            try
            {
                string subject, body;

                switch (trangThai)
                {
                    case "Đã xác nhận":
                        subject = $"✅ [Kimipet's Spa] Lịch hẹn #{lich.MaLich} đã được xác nhận!";
                        body = BuildConfirmedEmail(toName, lich);
                        break;
                    case "Đã hủy":
                        subject = $"❌ [Kimipet's Spa] Lịch hẹn #{lich.MaLich} đã bị hủy";
                        body = BuildCancelledEmail(toName, lich);
                        break;
                    case "Hoàn thành":
                        subject = $"🎉 [Kimipet's Spa] Dịch vụ #{lich.MaLich} hoàn thành - Cảm ơn bạn!";
                        body = BuildCompletedEmail(toName, lich);
                        break;
                    default:
                        subject = $"📋 [Kimipet's Spa] Đặt lịch #{lich.MaLich} thành công - Chờ xác nhận";
                        body = BuildPendingEmail(toName, lich);
                        break;
                }

                return Send(toEmail, subject, body);
            }
            catch { return false; }
        }

        /// <summary>Thông báo Admin khi có lịch mới</summary>
        // ✅ ĐÚNG
        public static bool SendAdminNotification(WebCafe.Models.DatLichSpa lich)
        {
            try
            {
                string subject = $"🐾 Lịch Spa mới #{lich.MaLich} - {lich.HoTen} - {lich.NgayHen:dd/MM/yyyy}";
                string body = $@"
<div style='font-family:Arial,sans-serif;max-width:600px;margin:0 auto;'>
  <div style='background:#1a7a3c;color:#fff;padding:20px;border-radius:8px 8px 0 0;'>
    <h2 style='margin:0;'>🐾 Lịch Spa Mới — Cần Xác Nhận</h2>
  </div>
  <div style='padding:24px;background:#fff;border:1px solid #e5e5e5;border-radius:0 0 8px 8px;'>
    <table style='width:100%;border-collapse:collapse;font-size:14px;'>
      <tr><td style='padding:8px;color:#666;width:40%;'>Mã lịch</td>
          <td style='padding:8px;font-weight:bold;'>#{lich.MaLich}</td></tr>
      <tr style='background:#f9f9f9;'>
          <td style='padding:8px;color:#666;'>Khách hàng</td>
          <td style='padding:8px;font-weight:bold;'>{lich.HoTen}</td></tr>
      <tr><td style='padding:8px;color:#666;'>Email</td>
          <td style='padding:8px;'>{lich.Email}</td></tr>
      <tr style='background:#f9f9f9;'>
          <td style='padding:8px;color:#666;'>SĐT</td>
          <td style='padding:8px;'>{lich.SoDienThoai}</td></tr>
      <tr><td style='padding:8px;color:#666;'>Thú cưng</td>
          <td style='padding:8px;font-weight:bold;'>{lich.TenThuCung}</td></tr>
      <tr style='background:#f9f9f9;'>
          <td style='padding:8px;color:#666;'>Dịch vụ</td>
          <td style='padding:8px;'>{lich.DichVuSpa?.TenDichVu}</td></tr>
      <tr><td style='padding:8px;color:#666;'>Ngày hẹn</td>
          <td style='padding:8px;font-weight:bold;color:#1a7a3c;'>{lich.NgayHen:dd/MM/yyyy} lúc {lich.GioHen:hh\\:mm}</td></tr>
      <tr style='background:#f9f9f9;'>
          <td style='padding:8px;color:#666;'>Ghi chú</td>
          <td style='padding:8px;'>{lich.GhiChu ?? "Không có"}</td></tr>
    </table>
    <div style='margin-top:20px;text-align:center;'>
      <a href='http://yoursite.com/AdminSpa/Index'
         style='background:#1a7a3c;color:#fff;padding:12px 28px;border-radius:6px;text-decoration:none;font-weight:bold;'>
        Vào trang quản lý →
      </a>
    </div>
  </div>
</div>";
                return Send(FromEmail, subject, body);
            }
            catch { return false; }
        }

        // ── Private helpers ──────────────────────────────────────

        private static bool Send(string toEmail, string subject, string body)
        {
            using (var mail = new MailMessage())
            {
                mail.From = new MailAddress(FromEmail, FromName);
                mail.To.Add(toEmail);
                mail.Subject = subject;
                mail.Body = body;
                mail.IsBodyHtml = true;

                using (var smtp = new SmtpClient(SmtpHost, SmtpPort))
                {
                    smtp.EnableSsl = true;
                    smtp.Credentials = new NetworkCredential(FromEmail, SmtpPassword);
                    smtp.Send(mail);
                }
            }
            return true;
        }

        private static string BuildPendingEmail(string name, DatLichSpa l) => $@"
<div style='font-family:""Nunito"",Arial,sans-serif;max-width:600px;margin:0 auto;background:#f7fdf9;'>
  <div style='background:linear-gradient(135deg,#1a7a3c,#34c769);padding:32px 24px;text-align:center;border-radius:12px 12px 0 0;'>
    <div style='font-size:48px;'>🐾</div>
    <h1 style='color:#fff;margin:8px 0 4px;font-size:24px;'>Đặt lịch thành công!</h1>
    <p style='color:rgba(255,255,255,0.85);margin:0;'>Cảm ơn bạn đã tin tưởng Kimipet's Spa</p>
  </div>
  <div style='background:#fff;padding:28px 24px;border:1px solid #e8f5e9;border-radius:0 0 12px 12px;'>
    <p>Xin chào <strong>{name}</strong>,</p>
    <p style='color:#555;font-size:14px;line-height:1.6;'>Chúng tôi đã nhận lịch spa cho bé <strong>{l.TenThuCung}</strong>. Nhân viên sẽ xác nhận trong vòng <strong>30 phút</strong>.</p>
    <div style='background:#f0faf4;border-left:4px solid #1a7a3c;border-radius:8px;padding:16px;margin:20px 0;'>
      <p style='margin:4px 0;font-size:14px;'><strong>📋 Mã lịch:</strong> #{l.MaLich}</p>
      <p style='margin:4px 0;font-size:14px;'><strong>🐶 Thú cưng:</strong> {l.TenThuCung}</p>
      <p style='margin:4px 0;font-size:14px;'><strong>✂️ Dịch vụ:</strong> {l.DichVuSpa?.TenDichVu}</p>
      <p style='margin:4px 0;font-size:14px;color:#1a7a3c;'><strong>📅 Ngày hẹn:</strong> {l.NgayHen:dd/MM/yyyy} lúc {l.GioHen:hh\\:mm}</p>
    </div>
    <p style='color:#888;font-size:13px;'>Cần hỗ trợ? Gọi: <strong style='color:#1a7a3c;'>039 4627 246</strong></p>
  </div>
</div>";

        private static string BuildConfirmedEmail(string name, DatLichSpa l) => $@"
<div style='font-family:""Nunito"",Arial,sans-serif;max-width:600px;margin:0 auto;background:#f7fdf9;'>
  <div style='background:linear-gradient(135deg,#1a7a3c,#34c769);padding:32px 24px;text-align:center;border-radius:12px 12px 0 0;'>
    <div style='font-size:48px;'>✅</div>
    <h1 style='color:#fff;margin:8px 0 4px;font-size:24px;'>Lịch hẹn đã được xác nhận!</h1>
  </div>
  <div style='background:#fff;padding:28px 24px;border:1px solid #e8f5e9;border-radius:0 0 12px 12px;'>
    <p>Xin chào <strong>{name}</strong>,</p>
    <p style='color:#555;font-size:14px;'>Lịch spa cho bé <strong>{l.TenThuCung}</strong> đã được xác nhận. Vui lòng đến đúng giờ!</p>
    <div style='background:#f0faf4;border-left:4px solid #1a7a3c;border-radius:8px;padding:16px;margin:20px 0;'>
      <p style='margin:4px 0;font-size:14px;'><strong>📋 Mã lịch:</strong> #{l.MaLich}</p>
      <p style='margin:4px 0;font-size:14px;color:#1a7a3c;'><strong>📅 Ngày hẹn:</strong> {l.NgayHen:dd/MM/yyyy} lúc {l.GioHen:hh\\:mm}</p>
      {(l.GhiChuAdmin != null ? $"<p style='margin:4px 0;font-size:14px;'><strong>💬 Ghi chú:</strong> {l.GhiChuAdmin}</p>" : "")}
    </div>
    <div style='background:#fff3cd;border-radius:8px;padding:14px;font-size:13px;color:#856404;'>
      ⚠️ Mang theo giấy tiêm phòng (nếu có). Đến trước 5 phút để làm thủ tục.
    </div>
  </div>
</div>";

        private static string BuildCancelledEmail(string name, DatLichSpa l) => $@"
<div style='font-family:""Nunito"",Arial,sans-serif;max-width:600px;margin:0 auto;'>
  <div style='background:#e53935;padding:32px 24px;text-align:center;border-radius:12px 12px 0 0;'>
    <div style='font-size:48px;'>❌</div>
    <h1 style='color:#fff;margin:8px 0 4px;font-size:24px;'>Lịch hẹn đã bị hủy</h1>
  </div>
  <div style='background:#fff;padding:28px 24px;border:1px solid #fce8e8;border-radius:0 0 12px 12px;'>
    <p>Xin chào <strong>{name}</strong>,</p>
    <p style='color:#555;font-size:14px;'>Rất tiếc, lịch <strong>#{l.MaLich}</strong> đã bị hủy.</p>
    <p style='font-size:14px;'><strong>Lý do:</strong> {l.LyDoHuy ?? "Không có"}</p>
    <p style='color:#555;font-size:14px;'>Vui lòng đặt lại hoặc liên hệ: <strong style='color:#1a7a3c;'>039 4627 246</strong></p>
  </div>
</div>";

        private static string BuildCompletedEmail(string name, DatLichSpa l) => $@"
<div style='font-family:""Nunito"",Arial,sans-serif;max-width:600px;margin:0 auto;background:#f7fdf9;'>
  <div style='background:linear-gradient(135deg,#f5c518,#ff8c00);padding:32px 24px;text-align:center;border-radius:12px 12px 0 0;'>
    <div style='font-size:48px;'>🎉</div>
    <h1 style='color:#fff;margin:8px 0 4px;font-size:24px;'>Dịch vụ hoàn thành!</h1>
  </div>
  <div style='background:#fff;padding:28px 24px;border:1px solid #e8f5e9;border-radius:0 0 12px 12px;'>
    <p>Xin chào <strong>{name}</strong>,</p>
    <p style='color:#555;font-size:14px;'>Bé <strong>{l.TenThuCung}</strong> đã hoàn thành dịch vụ. Hẹn gặp lại! 🐾</p>
    <p style='color:#888;font-size:13px;text-align:center;margin-top:20px;'>Hãy để lại đánh giá trên website để giúp chúng tôi cải thiện dịch vụ nhé!</p>
  </div>
</div>";
    }
}