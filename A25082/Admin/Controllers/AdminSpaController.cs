using System;
using System.Linq;
using System.Web.Mvc;
using System.Data.Entity;
using System.Net.Mail;
using System.Net;
using A25082.Models;
using WebCafe.Models;

namespace A25082.Controllers
{
    public class AdminSpaController : Controller
    {
        private AppDbContext db = new AppDbContext();

        // ── Credentials gửi mail (giống PayMentController) ──────
        private const string MAIL_FROM = "thaithienkim365@gmail.com";
        private const string MAIL_PASSWORD = "vbfxtnjnlurdcuzm";
        private const string MAIL_NAME = "Kimipet's Spa";

        private bool IsAdmin()
        {
            var user = Session["User"] as NguoiDung;
            return user != null && user.MaVaiTro == 1;
        }

        // ── Hàm gửi mail nội bộ ──────────────────────────────────
        private bool GuiMailSpa(string toEmail, string toName, DatLichSpa lich, string trangThai)
        {
            try
            {
                string subject, body;

                // ── FONT IMPORT ───────────────────────────────────
                string fontImport = @"<style>@import url('https://fonts.googleapis.com/css2?family=Cormorant+Garamond:wght@400;600;700&family=DM+Sans:wght@300;400;500&display=swap');</style>";

                if (trangThai == "Đã xác nhận")
                {
                    subject = "[Kimipet's Spa] Lịch hẹn #" + lich.MaLich + " đã được xác nhận!";
                    body = fontImport + $@"
<div style='margin:0;padding:0;background-color:#f2f0eb;font-family:""DM Sans"",Helvetica,Arial,sans-serif;'>
<table width='100%' cellpadding='0' cellspacing='0' border='0' style='background-color:#f2f0eb;padding:40px 16px;'>
  <tr><td align='center'>
  <table width='600' cellpadding='0' cellspacing='0' border='0' style='max-width:600px;width:100%;'>

    <!-- HEADER BAR -->
    <tr>
      <td style='background-color:#0d3320;padding:10px 40px;'>
        <table width='100%' cellpadding='0' cellspacing='0' border='0'>
          <tr>
            <td style='font-family:""DM Sans"",Helvetica,sans-serif;font-size:11px;font-weight:500;letter-spacing:3px;color:#7ec99a;text-transform:uppercase;'>KIMIPET&apos;S SPA &nbsp;&mdash;&nbsp; CẦN THƠ</td>
            <td align='right' style='font-family:""DM Sans"",Helvetica,sans-serif;font-size:11px;color:#4a7a5e;letter-spacing:1px;'>039 4627 246</td>
          </tr>
        </table>
      </td>
    </tr>

    <!-- HERO -->
    <tr>
      <td style='background:linear-gradient(160deg,#0f4029 0%,#1a6b42 55%,#2d9e63 100%);padding:52px 40px 44px;text-align:center;'>
        <div style='width:40px;height:2px;background:#7ec99a;margin:0 auto 24px;'></div>
        <p style='margin:0 0 12px;font-family:""DM Sans"",Helvetica,sans-serif;font-size:11px;font-weight:500;letter-spacing:4px;color:#7ec99a;text-transform:uppercase;'>Xác nhận lịch hẹn</p>
        <h1 style='margin:0 0 8px;font-family:""Cormorant Garamond"",Georgia,serif;font-size:42px;font-weight:600;color:#ffffff;line-height:1.1;letter-spacing:-0.5px;'>Lịch hẹn của bạn<br/>đã được xác nhận</h1>
        <p style='margin:20px 0 0;font-family:""DM Sans"",Helvetica,sans-serif;font-size:14px;color:rgba(255,255,255,0.6);font-weight:300;'>Cảm ơn bạn đã tin tưởng dịch vụ chăm sóc thú cưng của chúng tôi</p>
        <div style='width:40px;height:2px;background:rgba(126,201,154,0.4);margin:28px auto 0;'></div>
      </td>
    </tr>

    <!-- GREETING -->
    <tr>
      <td style='background-color:#ffffff;padding:40px 40px 0;'>
        <p style='margin:0;font-family:""DM Sans"",Helvetica,sans-serif;font-size:15px;color:#333;line-height:1.7;'>Xin chào <strong style='color:#0d3320;font-weight:500;'>{toName}</strong>,</p>
        <p style='margin:10px 0 0;font-family:""DM Sans"",Helvetica,sans-serif;font-size:14px;color:#666;line-height:1.7;font-weight:300;'>Lịch spa cho bé <strong style='color:#1a6b42;font-weight:500;'>{lich.TenThuCung}</strong> đã được xác nhận thành công. Vui lòng đến đúng giờ để nhận được dịch vụ tốt nhất.</p>
      </td>
    </tr>

    <!-- DIVIDER -->
    <tr><td style='background-color:#ffffff;padding:28px 40px 0;'><div style='height:1px;background:linear-gradient(to right,transparent,#e0ddd5,transparent);'></div></td></tr>

    <!-- BOOKING DETAILS -->
    <tr>
      <td style='background-color:#ffffff;padding:24px 40px 0;'>
        <p style='margin:0 0 20px;font-family:""DM Sans"",Helvetica,sans-serif;font-size:10px;font-weight:500;letter-spacing:3px;color:#999;text-transform:uppercase;'>Chi tiết lịch hẹn</p>
        <table width='100%' cellpadding='0' cellspacing='0' border='0'>
          <tr>
            <td style='padding:14px 0;border-bottom:1px solid #f0ede8;width:42%;'><span style='font-family:""DM Sans"",Helvetica,sans-serif;font-size:12px;font-weight:500;letter-spacing:1.5px;text-transform:uppercase;color:#aaa;'>Mã lịch hẹn</span></td>
            <td style='padding:14px 0;border-bottom:1px solid #f0ede8;'><span style='font-family:""Cormorant Garamond"",Georgia,serif;font-size:18px;font-weight:600;color:#0d3320;letter-spacing:1px;'>#{lich.MaLich}</span></td>
          </tr>
          <tr>
            <td style='padding:14px 0;border-bottom:1px solid #f0ede8;'><span style='font-family:""DM Sans"",Helvetica,sans-serif;font-size:12px;font-weight:500;letter-spacing:1.5px;text-transform:uppercase;color:#aaa;'>Thú cưng</span></td>
            <td style='padding:14px 0;border-bottom:1px solid #f0ede8;'><span style='font-family:""DM Sans"",Helvetica,sans-serif;font-size:15px;font-weight:500;color:#1a1a1a;'>{lich.TenThuCung}</span></td>
          </tr>
          <tr>
            <td style='padding:14px 0;border-bottom:1px solid #f0ede8;'><span style='font-family:""DM Sans"",Helvetica,sans-serif;font-size:12px;font-weight:500;letter-spacing:1.5px;text-transform:uppercase;color:#aaa;'>Dịch vụ</span></td>
            <td style='padding:14px 0;border-bottom:1px solid #f0ede8;'><span style='font-family:""DM Sans"",Helvetica,sans-serif;font-size:15px;color:#333;'>{(lich.DichVuSpa != null ? lich.DichVuSpa.TenDichVu : "")}</span></td>
          </tr>
          <tr>
            <td style='padding:14px 0;border-bottom:1px solid #f0ede8;'><span style='font-family:""DM Sans"",Helvetica,sans-serif;font-size:12px;font-weight:500;letter-spacing:1.5px;text-transform:uppercase;color:#aaa;'>Ngày hẹn</span></td>
            <td style='padding:14px 0;border-bottom:1px solid #f0ede8;'>
              <span style='font-family:""Cormorant Garamond"",Georgia,serif;font-size:18px;font-weight:600;color:#1a6b42;'>{lich.NgayHen:dd/MM/yyyy}</span>
              <span style='font-family:""DM Sans"",Helvetica,sans-serif;font-size:13px;color:#888;margin-left:8px;'>lúc {lich.GioHen:hh\:mm}</span>
            </td>
          </tr>
          {(!string.IsNullOrEmpty(lich.GhiChuAdmin) ? $@"
          <tr>
            <td style='padding:14px 0;'><span style='font-family:""DM Sans"",Helvetica,sans-serif;font-size:12px;font-weight:500;letter-spacing:1.5px;text-transform:uppercase;color:#aaa;'>Ghi chú</span></td>
            <td style='padding:14px 0;'><span style='font-family:""DM Sans"",Helvetica,sans-serif;font-size:14px;color:#555;font-style:italic;'>{lich.GhiChuAdmin}</span></td>
          </tr>" : "")}
        </table>
      </td>
    </tr>

    <!-- REMINDER -->
    <tr>
      <td style='background-color:#ffffff;padding:28px 40px 0;'>
        <table width='100%' cellpadding='0' cellspacing='0' border='0'>
          <tr>
            <td style='background-color:#f7f5f0;border-left:3px solid #1a6b42;padding:18px 22px;'>
              <p style='margin:0 0 6px;font-family:""DM Sans"",Helvetica,sans-serif;font-size:11px;font-weight:500;letter-spacing:2px;color:#1a6b42;text-transform:uppercase;'>Lưu ý quan trọng</p>
              <p style='margin:0;font-family:""DM Sans"",Helvetica,sans-serif;font-size:13px;color:#666;line-height:1.7;font-weight:300;'>Mang theo giấy tiêm phòng (nếu có). Vui lòng đến trước <strong style='color:#333;font-weight:500;'>5 phút</strong> để hoàn tất thủ tục.</p>
            </td>
          </tr>
        </table>
      </td>
    </tr>

    <!-- CONTACT -->
    <tr>
      <td style='background-color:#ffffff;padding:28px 40px 40px;'>
        <table width='100%' cellpadding='0' cellspacing='0' border='0'>
          <tr>
            <td style='border-top:1px solid #f0ede8;padding-top:24px;'>
              <p style='margin:0;font-family:""DM Sans"",Helvetica,sans-serif;font-size:13px;color:#999;font-weight:300;'>Cần hỗ trợ? Liên hệ trực tiếp với chúng tôi:</p>
              <p style='margin:6px 0 0;font-family:""Cormorant Garamond"",Georgia,serif;font-size:20px;font-weight:600;color:#0d3320;letter-spacing:1px;'>039 4627 246</p>
            </td>
          </tr>
        </table>
      </td>
    </tr>

    <!-- FOOTER -->
    <tr>
      <td style='background-color:#0d3320;padding:28px 40px;'>
        <table width='100%' cellpadding='0' cellspacing='0' border='0'>
          <tr>
            <td>
              <p style='margin:0 0 4px;font-family:""Cormorant Garamond"",Georgia,serif;font-size:18px;font-weight:600;color:#ffffff;'>Kimipet&apos;s Spa</p>
              <p style='margin:0;font-family:""DM Sans"",Helvetica,sans-serif;font-size:12px;color:#4a7a5e;font-weight:300;letter-spacing:1px;'>Cần Thơ, Việt Nam</p>
            </td>
            <td align='right'>
              <p style='margin:0;font-family:""DM Sans"",Helvetica,sans-serif;font-size:11px;color:#4a7a5e;font-weight:300;line-height:1.6;'>Chăm sóc thú cưng<br/>tận tâm &amp; chuyên nghiệp</p>
            </td>
          </tr>
        </table>
      </td>
    </tr>
    <tr><td style='background-color:#08200f;padding:10px 40px;text-align:center;'><p style='margin:0;font-family:""DM Sans"",Helvetica,sans-serif;font-size:10px;color:#2d5c3e;letter-spacing:2px;'>EMAIL NÀY ĐƯỢC GỬI TỰ ĐỘNG — VUI LÒNG KHÔNG PHẢN HỒI</p></td></tr>

  </table>
  </td></tr>
</table>
</div>";
                }
                else if (trangThai == "Đã hủy")
                {
                    subject = "[Kimipet's Spa] Lịch hẹn #" + lich.MaLich + " đã bị hủy";
                    body = fontImport + $@"
<div style='margin:0;padding:0;background-color:#f2f0eb;font-family:""DM Sans"",Helvetica,Arial,sans-serif;'>
<table width='100%' cellpadding='0' cellspacing='0' border='0' style='background-color:#f2f0eb;padding:40px 16px;'>
  <tr><td align='center'>
  <table width='600' cellpadding='0' cellspacing='0' border='0' style='max-width:600px;width:100%;'>

    <!-- HEADER BAR -->
    <tr>
      <td style='background-color:#2c0a0a;padding:10px 40px;'>
        <table width='100%' cellpadding='0' cellspacing='0' border='0'>
          <tr>
            <td style='font-family:""DM Sans"",Helvetica,sans-serif;font-size:11px;font-weight:500;letter-spacing:3px;color:#e07070;text-transform:uppercase;'>KIMIPET&apos;S SPA &nbsp;&mdash;&nbsp; CẦN THƠ</td>
            <td align='right' style='font-family:""DM Sans"",Helvetica,sans-serif;font-size:11px;color:#8a4a4a;letter-spacing:1px;'>039 4627 246</td>
          </tr>
        </table>
      </td>
    </tr>

    <!-- HERO -->
    <tr>
      <td style='background:linear-gradient(160deg,#5c1010 0%,#9b2020 55%,#c0392b 100%);padding:52px 40px 44px;text-align:center;'>
        <div style='width:40px;height:2px;background:#e07070;margin:0 auto 24px;'></div>
        <p style='margin:0 0 12px;font-family:""DM Sans"",Helvetica,sans-serif;font-size:11px;font-weight:500;letter-spacing:4px;color:#e07070;text-transform:uppercase;'>Thông báo hủy lịch</p>
        <h1 style='margin:0 0 8px;font-family:""Cormorant Garamond"",Georgia,serif;font-size:42px;font-weight:600;color:#ffffff;line-height:1.1;letter-spacing:-0.5px;'>Lịch hẹn của bạn<br/>đã bị hủy</h1>
        <p style='margin:20px 0 0;font-family:""DM Sans"",Helvetica,sans-serif;font-size:14px;color:rgba(255,255,255,0.6);font-weight:300;'>Chúng tôi rất tiếc vì sự bất tiện này</p>
        <div style='width:40px;height:2px;background:rgba(224,112,112,0.4);margin:28px auto 0;'></div>
      </td>
    </tr>

    <!-- CONTENT -->
    <tr>
      <td style='background-color:#ffffff;padding:40px 40px 0;'>
        <p style='margin:0;font-family:""DM Sans"",Helvetica,sans-serif;font-size:15px;color:#333;line-height:1.7;'>Xin chào <strong style='color:#5c1010;font-weight:500;'>{toName}</strong>,</p>
        <p style='margin:10px 0 0;font-family:""DM Sans"",Helvetica,sans-serif;font-size:14px;color:#666;line-height:1.7;font-weight:300;'>Rất tiếc, lịch hẹn <strong style='color:#9b2020;'>#{lich.MaLich}</strong> của bạn đã bị hủy.</p>
      </td>
    </tr>

    <tr><td style='background-color:#ffffff;padding:20px 40px 0;'><div style='height:1px;background:linear-gradient(to right,transparent,#e0ddd5,transparent);'></div></td></tr>

    <!-- LY DO -->
    <tr>
      <td style='background-color:#ffffff;padding:24px 40px 0;'>
        <table width='100%' cellpadding='0' cellspacing='0' border='0'>
          <tr>
            <td style='background-color:#fff5f5;border-left:3px solid #c0392b;padding:18px 22px;'>
              <p style='margin:0 0 6px;font-family:""DM Sans"",Helvetica,sans-serif;font-size:11px;font-weight:500;letter-spacing:2px;color:#9b2020;text-transform:uppercase;'>Lý do hủy</p>
              <p style='margin:0;font-family:""DM Sans"",Helvetica,sans-serif;font-size:14px;color:#555;line-height:1.7;font-style:italic;'>{(lich.LyDoHuy ?? "Không có thông tin")}</p>
            </td>
          </tr>
        </table>
      </td>
    </tr>

    <!-- CONTACT -->
    <tr>
      <td style='background-color:#ffffff;padding:28px 40px 40px;'>
        <table width='100%' cellpadding='0' cellspacing='0' border='0'>
          <tr>
            <td style='border-top:1px solid #f0ede8;padding-top:24px;'>
              <p style='margin:0;font-family:""DM Sans"",Helvetica,sans-serif;font-size:13px;color:#999;font-weight:300;'>Muốn đặt lại lịch hoặc cần hỗ trợ?</p>
              <p style='margin:6px 0 0;font-family:""Cormorant Garamond"",Georgia,serif;font-size:20px;font-weight:600;color:#0d3320;letter-spacing:1px;'>039 4627 246</p>
            </td>
          </tr>
        </table>
      </td>
    </tr>

    <!-- FOOTER -->
    <tr>
      <td style='background-color:#0d3320;padding:28px 40px;'>
        <table width='100%' cellpadding='0' cellspacing='0' border='0'>
          <tr>
            <td>
              <p style='margin:0 0 4px;font-family:""Cormorant Garamond"",Georgia,serif;font-size:18px;font-weight:600;color:#ffffff;'>Kimipet&apos;s Spa</p>
              <p style='margin:0;font-family:""DM Sans"",Helvetica,sans-serif;font-size:12px;color:#4a7a5e;font-weight:300;letter-spacing:1px;'>Cần Thơ, Việt Nam</p>
            </td>
            <td align='right'>
              <p style='margin:0;font-family:""DM Sans"",Helvetica,sans-serif;font-size:11px;color:#4a7a5e;font-weight:300;line-height:1.6;'>Chăm sóc thú cưng<br/>tận tâm &amp; chuyên nghiệp</p>
            </td>
          </tr>
        </table>
      </td>
    </tr>
    <tr><td style='background-color:#08200f;padding:10px 40px;text-align:center;'><p style='margin:0;font-family:""DM Sans"",Helvetica,sans-serif;font-size:10px;color:#2d5c3e;letter-spacing:2px;'>EMAIL NÀY ĐƯỢC GỬI TỰ ĐỘNG — VUI LÒNG KHÔNG PHẢN HỒI</p></td></tr>

  </table>
  </td></tr>
</table>
</div>";
                }
                else if (trangThai == "Hoàn thành")
                {
                    subject = "[Kimipet's Spa] Dịch vụ #" + lich.MaLich + " hoàn thành - Cảm ơn bạn!";
                    body = fontImport + $@"
<div style='margin:0;padding:0;background-color:#f2f0eb;font-family:""DM Sans"",Helvetica,Arial,sans-serif;'>
<table width='100%' cellpadding='0' cellspacing='0' border='0' style='background-color:#f2f0eb;padding:40px 16px;'>
  <tr><td align='center'>
  <table width='600' cellpadding='0' cellspacing='0' border='0' style='max-width:600px;width:100%;'>

    <!-- HEADER BAR -->
    <tr>
      <td style='background-color:#2a1f00;padding:10px 40px;'>
        <table width='100%' cellpadding='0' cellspacing='0' border='0'>
          <tr>
            <td style='font-family:""DM Sans"",Helvetica,sans-serif;font-size:11px;font-weight:500;letter-spacing:3px;color:#c9a84c;text-transform:uppercase;'>KIMIPET&apos;S SPA &nbsp;&mdash;&nbsp; CẦN THƠ</td>
            <td align='right' style='font-family:""DM Sans"",Helvetica,sans-serif;font-size:11px;color:#7a6020;letter-spacing:1px;'>039 4627 246</td>
          </tr>
        </table>
      </td>
    </tr>

    <!-- HERO -->
    <tr>
      <td style='background:linear-gradient(160deg,#3d2a00 0%,#7a5500 55%,#c09020 100%);padding:52px 40px 44px;text-align:center;'>
        <div style='width:40px;height:2px;background:#c9a84c;margin:0 auto 24px;'></div>
        <p style='margin:0 0 12px;font-family:""DM Sans"",Helvetica,sans-serif;font-size:11px;font-weight:500;letter-spacing:4px;color:#c9a84c;text-transform:uppercase;'>Dịch vụ hoàn thành</p>
        <h1 style='margin:0 0 8px;font-family:""Cormorant Garamond"",Georgia,serif;font-size:42px;font-weight:600;color:#ffffff;line-height:1.1;letter-spacing:-0.5px;'>Cảm ơn bạn<br/>đã tin tưởng chúng tôi</h1>
        <p style='margin:20px 0 0;font-family:""DM Sans"",Helvetica,sans-serif;font-size:14px;color:rgba(255,255,255,0.6);font-weight:300;'>Rất vui được chăm sóc cho bé yêu của bạn</p>
        <div style='width:40px;height:2px;background:rgba(201,168,76,0.4);margin:28px auto 0;'></div>
      </td>
    </tr>

    <!-- CONTENT -->
    <tr>
      <td style='background-color:#ffffff;padding:40px 40px 0;'>
        <p style='margin:0;font-family:""DM Sans"",Helvetica,sans-serif;font-size:15px;color:#333;line-height:1.7;'>Xin chào <strong style='color:#3d2a00;font-weight:500;'>{toName}</strong>,</p>
        <p style='margin:10px 0 0;font-family:""DM Sans"",Helvetica,sans-serif;font-size:14px;color:#666;line-height:1.7;font-weight:300;'>Bé <strong style='color:#7a5500;font-weight:500;'>{lich.TenThuCung}</strong> đã hoàn thành dịch vụ tại Kimipet's Spa. Chúng tôi hy vọng bé có một trải nghiệm tuyệt vời!</p>
      </td>
    </tr>

    <tr><td style='background-color:#ffffff;padding:28px 40px 0;'><div style='height:1px;background:linear-gradient(to right,transparent,#e0ddd5,transparent);'></div></td></tr>

    <!-- FEEDBACK NUDGE -->
    <tr>
      <td style='background-color:#ffffff;padding:24px 40px 0;'>
        <table width='100%' cellpadding='0' cellspacing='0' border='0'>
          <tr>
            <td style='background-color:#fdfaf2;border-left:3px solid #c09020;padding:18px 22px;'>
              <p style='margin:0 0 6px;font-family:""DM Sans"",Helvetica,sans-serif;font-size:11px;font-weight:500;letter-spacing:2px;color:#7a5500;text-transform:uppercase;'>Đánh giá của bạn</p>
              <p style='margin:0;font-family:""DM Sans"",Helvetica,sans-serif;font-size:13px;color:#666;line-height:1.7;font-weight:300;'>Hãy để lại đánh giá để giúp chúng tôi cải thiện dịch vụ và phục vụ bé tốt hơn trong những lần tiếp theo.</p>
            </td>
          </tr>
        </table>
      </td>
    </tr>

    <!-- CONTACT -->
    <tr>
      <td style='background-color:#ffffff;padding:28px 40px 40px;'>
        <table width='100%' cellpadding='0' cellspacing='0' border='0'>
          <tr>
            <td style='border-top:1px solid #f0ede8;padding-top:24px;'>
              <p style='margin:0;font-family:""DM Sans"",Helvetica,sans-serif;font-size:13px;color:#999;font-weight:300;'>Hẹn gặp lại! Liên hệ đặt lịch lần tiếp:</p>
              <p style='margin:6px 0 0;font-family:""Cormorant Garamond"",Georgia,serif;font-size:20px;font-weight:600;color:#0d3320;letter-spacing:1px;'>039 4627 246</p>
            </td>
          </tr>
        </table>
      </td>
    </tr>

    <!-- FOOTER -->
    <tr>
      <td style='background-color:#0d3320;padding:28px 40px;'>
        <table width='100%' cellpadding='0' cellspacing='0' border='0'>
          <tr>
            <td>
              <p style='margin:0 0 4px;font-family:""Cormorant Garamond"",Georgia,serif;font-size:18px;font-weight:600;color:#ffffff;'>Kimipet&apos;s Spa</p>
              <p style='margin:0;font-family:""DM Sans"",Helvetica,sans-serif;font-size:12px;color:#4a7a5e;font-weight:300;letter-spacing:1px;'>Cần Thơ, Việt Nam</p>
            </td>
            <td align='right'>
              <p style='margin:0;font-family:""DM Sans"",Helvetica,sans-serif;font-size:11px;color:#4a7a5e;font-weight:300;line-height:1.6;'>Chăm sóc thú cưng<br/>tận tâm &amp; chuyên nghiệp</p>
            </td>
          </tr>
        </table>
      </td>
    </tr>
    <tr><td style='background-color:#08200f;padding:10px 40px;text-align:center;'><p style='margin:0;font-family:""DM Sans"",Helvetica,sans-serif;font-size:10px;color:#2d5c3e;letter-spacing:2px;'>EMAIL NÀY ĐƯỢC GỬI TỰ ĐỘNG — VUI LÒNG KHÔNG PHẢN HỒI</p></td></tr>

  </table>
  </td></tr>
</table>
</div>";
                }
                else
                {
                    // Chờ xác nhận hoặc trạng thái khác
                    subject = "[Kimipet's Spa] Đặt lịch #" + lich.MaLich + " thành công - Chờ xác nhận";
                    body = fontImport + $@"
<div style='margin:0;padding:0;background-color:#f2f0eb;font-family:""DM Sans"",Helvetica,Arial,sans-serif;'>
<table width='100%' cellpadding='0' cellspacing='0' border='0' style='background-color:#f2f0eb;padding:40px 16px;'>
  <tr><td align='center'>
  <table width='600' cellpadding='0' cellspacing='0' border='0' style='max-width:600px;width:100%;'>

    <!-- HEADER BAR -->
    <tr>
      <td style='background-color:#0d3320;padding:10px 40px;'>
        <table width='100%' cellpadding='0' cellspacing='0' border='0'>
          <tr>
            <td style='font-family:""DM Sans"",Helvetica,sans-serif;font-size:11px;font-weight:500;letter-spacing:3px;color:#7ec99a;text-transform:uppercase;'>KIMIPET&apos;S SPA &nbsp;&mdash;&nbsp; CẦN THƠ</td>
            <td align='right' style='font-family:""DM Sans"",Helvetica,sans-serif;font-size:11px;color:#4a7a5e;letter-spacing:1px;'>039 4627 246</td>
          </tr>
        </table>
      </td>
    </tr>

    <!-- HERO -->
    <tr>
      <td style='background:linear-gradient(160deg,#0f4029 0%,#1a6b42 55%,#2d9e63 100%);padding:52px 40px 44px;text-align:center;'>
        <div style='width:40px;height:2px;background:#7ec99a;margin:0 auto 24px;'></div>
        <p style='margin:0 0 12px;font-family:""DM Sans"",Helvetica,sans-serif;font-size:11px;font-weight:500;letter-spacing:4px;color:#7ec99a;text-transform:uppercase;'>Đặt lịch thành công</p>
        <h1 style='margin:0 0 8px;font-family:""Cormorant Garamond"",Georgia,serif;font-size:42px;font-weight:600;color:#ffffff;line-height:1.1;letter-spacing:-0.5px;'>Chúng tôi đã nhận<br/>lịch hẹn của bạn</h1>
        <p style='margin:20px 0 0;font-family:""DM Sans"",Helvetica,sans-serif;font-size:14px;color:rgba(255,255,255,0.6);font-weight:300;'>Nhân viên sẽ xác nhận trong vòng <strong style='color:rgba(255,255,255,0.85);'>30 phút</strong></p>
        <div style='width:40px;height:2px;background:rgba(126,201,154,0.4);margin:28px auto 0;'></div>
      </td>
    </tr>

    <!-- GREETING -->
    <tr>
      <td style='background-color:#ffffff;padding:40px 40px 0;'>
        <p style='margin:0;font-family:""DM Sans"",Helvetica,sans-serif;font-size:15px;color:#333;line-height:1.7;'>Xin chào <strong style='color:#0d3320;font-weight:500;'>{toName}</strong>,</p>
        <p style='margin:10px 0 0;font-family:""DM Sans"",Helvetica,sans-serif;font-size:14px;color:#666;line-height:1.7;font-weight:300;'>Chúng tôi đã nhận lịch spa cho bé <strong style='color:#1a6b42;font-weight:500;'>{lich.TenThuCung}</strong>. Vui lòng chờ xác nhận từ nhân viên của chúng tôi.</p>
      </td>
    </tr>

    <tr><td style='background-color:#ffffff;padding:28px 40px 0;'><div style='height:1px;background:linear-gradient(to right,transparent,#e0ddd5,transparent);'></div></td></tr>

    <!-- BOOKING DETAILS -->
    <tr>
      <td style='background-color:#ffffff;padding:24px 40px 0;'>
        <p style='margin:0 0 20px;font-family:""DM Sans"",Helvetica,sans-serif;font-size:10px;font-weight:500;letter-spacing:3px;color:#999;text-transform:uppercase;'>Chi tiết lịch hẹn</p>
        <table width='100%' cellpadding='0' cellspacing='0' border='0'>
          <tr>
            <td style='padding:14px 0;border-bottom:1px solid #f0ede8;width:42%;'><span style='font-family:""DM Sans"",Helvetica,sans-serif;font-size:12px;font-weight:500;letter-spacing:1.5px;text-transform:uppercase;color:#aaa;'>Mã lịch hẹn</span></td>
            <td style='padding:14px 0;border-bottom:1px solid #f0ede8;'><span style='font-family:""Cormorant Garamond"",Georgia,serif;font-size:18px;font-weight:600;color:#0d3320;letter-spacing:1px;'>#{lich.MaLich}</span></td>
          </tr>
          <tr>
            <td style='padding:14px 0;border-bottom:1px solid #f0ede8;'><span style='font-family:""DM Sans"",Helvetica,sans-serif;font-size:12px;font-weight:500;letter-spacing:1.5px;text-transform:uppercase;color:#aaa;'>Thú cưng</span></td>
            <td style='padding:14px 0;border-bottom:1px solid #f0ede8;'><span style='font-family:""DM Sans"",Helvetica,sans-serif;font-size:15px;font-weight:500;color:#1a1a1a;'>{lich.TenThuCung}</span></td>
          </tr>
          <tr>
            <td style='padding:14px 0;border-bottom:1px solid #f0ede8;'><span style='font-family:""DM Sans"",Helvetica,sans-serif;font-size:12px;font-weight:500;letter-spacing:1.5px;text-transform:uppercase;color:#aaa;'>Dịch vụ</span></td>
            <td style='padding:14px 0;border-bottom:1px solid #f0ede8;'><span style='font-family:""DM Sans"",Helvetica,sans-serif;font-size:15px;color:#333;'>{(lich.DichVuSpa != null ? lich.DichVuSpa.TenDichVu : "")}</span></td>
          </tr>
          <tr>
            <td style='padding:14px 0;'><span style='font-family:""DM Sans"",Helvetica,sans-serif;font-size:12px;font-weight:500;letter-spacing:1.5px;text-transform:uppercase;color:#aaa;'>Ngày hẹn</span></td>
            <td style='padding:14px 0;'>
              <span style='font-family:""Cormorant Garamond"",Georgia,serif;font-size:18px;font-weight:600;color:#1a6b42;'>{lich.NgayHen:dd/MM/yyyy}</span>
              <span style='font-family:""DM Sans"",Helvetica,sans-serif;font-size:13px;color:#888;margin-left:8px;'>lúc {lich.GioHen:hh\:mm}</span>
            </td>
          </tr>
        </table>
      </td>
    </tr>

    <!-- CONTACT -->
    <tr>
      <td style='background-color:#ffffff;padding:28px 40px 40px;'>
        <table width='100%' cellpadding='0' cellspacing='0' border='0'>
          <tr>
            <td style='border-top:1px solid #f0ede8;padding-top:24px;'>
              <p style='margin:0;font-family:""DM Sans"",Helvetica,sans-serif;font-size:13px;color:#999;font-weight:300;'>Cần hỗ trợ? Liên hệ trực tiếp với chúng tôi:</p>
              <p style='margin:6px 0 0;font-family:""Cormorant Garamond"",Georgia,serif;font-size:20px;font-weight:600;color:#0d3320;letter-spacing:1px;'>039 4627 246</p>
            </td>
          </tr>
        </table>
      </td>
    </tr>

    <!-- FOOTER -->
    <tr>
      <td style='background-color:#0d3320;padding:28px 40px;'>
        <table width='100%' cellpadding='0' cellspacing='0' border='0'>
          <tr>
            <td>
              <p style='margin:0 0 4px;font-family:""Cormorant Garamond"",Georgia,serif;font-size:18px;font-weight:600;color:#ffffff;'>Kimipet&apos;s Spa</p>
              <p style='margin:0;font-family:""DM Sans"",Helvetica,sans-serif;font-size:12px;color:#4a7a5e;font-weight:300;letter-spacing:1px;'>Cần Thơ, Việt Nam</p>
            </td>
            <td align='right'>
              <p style='margin:0;font-family:""DM Sans"",Helvetica,sans-serif;font-size:11px;color:#4a7a5e;font-weight:300;line-height:1.6;'>Chăm sóc thú cưng<br/>tận tâm &amp; chuyên nghiệp</p>
            </td>
          </tr>
        </table>
      </td>
    </tr>
    <tr><td style='background-color:#08200f;padding:10px 40px;text-align:center;'><p style='margin:0;font-family:""DM Sans"",Helvetica,sans-serif;font-size:10px;color:#2d5c3e;letter-spacing:2px;'>EMAIL NÀY ĐƯỢC GỬI TỰ ĐỘNG — VUI LÒNG KHÔNG PHẢN HỒI</p></td></tr>

  </table>
  </td></tr>
</table>
</div>";
                }

                // Gửi mail
                MailMessage mail = new MailMessage();
                mail.From = new MailAddress(MAIL_FROM, MAIL_NAME);
                mail.To.Add(new MailAddress(toEmail));
                mail.Subject = subject;
                mail.Body = body;
                mail.IsBodyHtml = true;

                using (SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587))
                {
                    smtp.Credentials = new NetworkCredential(MAIL_FROM, MAIL_PASSWORD);
                    smtp.EnableSsl = true;
                    smtp.Send(mail);
                }

                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Loi gui email spa: " + ex.Message);
                return false;
            }
        }

        // ============================================================
        // GET: /AdminSpa/Index
        // ============================================================
        public ActionResult Index(string status = "", string date = "", string keyword = "")
        {
            if (!IsAdmin()) return RedirectToAction("Index", "DangNhapNguoiDung");

            var query = db.DatLichSpas
                          .Include(l => l.DichVuSpa)
                          .Include(l => l.LoaiThuCung)
                          .Include(l => l.NguoiDung)
                          .AsQueryable();

            if (!string.IsNullOrEmpty(status))
                query = query.Where(l => l.TrangThai == status);

            if (!string.IsNullOrEmpty(date) && DateTime.TryParse(date, out DateTime filterDate))
                query = query.Where(l => l.NgayHen == filterDate);

            if (!string.IsNullOrEmpty(keyword))
                query = query.Where(l => l.HoTen.Contains(keyword)
                                      || l.SoDienThoai.Contains(keyword)
                                      || l.TenThuCung.Contains(keyword));

            var all = db.DatLichSpas.ToList();

            var vm = new SpaBookingViewModel
            {
                DanhSachLich = query.OrderByDescending(l => l.NgayTao).ToList(),
                TongLich = all.Count,
                ChoXacNhan = all.Count(l => l.TrangThai == "Chờ xác nhận"),
                DaXacNhan = all.Count(l => l.TrangThai == "Đã xác nhận"),
                HoanThanh = all.Count(l => l.TrangThai == "Hoàn thành"),
                DaHuy = all.Count(l => l.TrangThai == "Đã hủy"),
                FilterStatus = status,
                FilterDate = date
            };

            ViewBag.Keyword = keyword;
            return View(vm);
        }

        // ============================================================
        // POST: /AdminSpa/CapNhatTrangThai
        // ============================================================
        [HttpPost]
        public JsonResult CapNhatTrangThai(int id, string trangThai, string ghiChu = "", string lyDoHuy = "")
        {
            if (!IsAdmin())
                return Json(new { success = false, message = "Khong co quyen." });

            var lich = db.DatLichSpas
                         .Include(l => l.DichVuSpa)
                         .FirstOrDefault(l => l.MaLich == id);

            if (lich == null)
                return Json(new { success = false, message = "Khong tim thay lich." });

            string oldStatus = lich.TrangThai;
            lich.TrangThai = trangThai;
            lich.NgayCapNhat = DateTime.Now;

            if (!string.IsNullOrEmpty(ghiChu))
                lich.GhiChuAdmin = ghiChu;

            if (trangThai == "Đã hủy" && !string.IsNullOrEmpty(lyDoHuy))
                lich.LyDoHuy = lyDoHuy;

            db.SaveChanges();

            // Gửi email nếu trạng thái thay đổi
            if (oldStatus != trangThai && !string.IsNullOrEmpty(lich.Email))
                GuiMailSpa(lich.Email, lich.HoTen, lich, trangThai);

            return Json(new
            {
                success = true,
                message = "Da cap nhat thanh \"" + trangThai + "\".",
                newStatus = trangThai
            });
        }

        // ============================================================
        // POST: /AdminSpa/GuiEmail
        // ============================================================
        [HttpPost]
        public JsonResult GuiEmail(int id, string trangThai, string ghiChu = "")
        {
            if (!IsAdmin())
                return Json(new { success = false, message = "Khong co quyen." });

            var lich = db.DatLichSpas
                         .Include(l => l.DichVuSpa)
                         .Include(l => l.LoaiThuCung)
                         .FirstOrDefault(l => l.MaLich == id);

            if (lich == null)
                return Json(new { success = false, message = "Khong tim thay lich hen." });

            if (string.IsNullOrEmpty(lich.Email))
                return Json(new { success = false, message = "Lich hen nay khong co dia chi email." });

            if (!string.IsNullOrEmpty(ghiChu))
            {
                lich.GhiChuAdmin = ghiChu;
                db.SaveChanges();
            }

            string trangThaiGui = string.IsNullOrEmpty(trangThai) ? lich.TrangThai : trangThai;
            bool ok = GuiMailSpa(lich.Email, lich.HoTen, lich, trangThaiGui);

            if (ok)
                return Json(new { success = true, message = "Da gui email den " + lich.Email });
            else
                return Json(new { success = false, message = "Gui email that bai. Kiem tra lai App Password Gmail." });
        }

        // ============================================================
        // GET: /AdminSpa/DichVu
        // ============================================================
        public ActionResult DichVu()
        {
            if (!IsAdmin()) return RedirectToAction("Index", "DangNhapNguoiDung");
            var list = db.DichVuSpas.OrderBy(d => d.GiaTien).ToList();
            return View(list);
        }

        [HttpPost]
        public JsonResult ThemDichVu(DichVuSpa model)
        {
            if (!IsAdmin()) return Json(new { success = false });
            if (string.IsNullOrEmpty(model.TenDichVu))
                return Json(new { success = false, message = "Ten dich vu khong duoc de trong." });

            db.DichVuSpas.Add(model);
            db.SaveChanges();
            return Json(new { success = true, message = "Them thanh cong!", id = model.MaDichVu });
        }

        [HttpPost]
        public JsonResult HienDichVu(int id)
        {
            if (!IsAdmin()) return Json(new { success = false });
            var dv = db.DichVuSpas.Find(id);
            if (dv == null) return Json(new { success = false });
            dv.TrangThai = true;
            db.SaveChanges();
            return Json(new { success = true });
        }

        [HttpPost]
        public JsonResult SuaDichVu(DichVuSpa model)
        {
            if (!IsAdmin()) return Json(new { success = false });
            var dv = db.DichVuSpas.Find(model.MaDichVu);
            if (dv == null) return Json(new { success = false, message = "Khong tim thay." });

            dv.TenDichVu = model.TenDichVu;
            dv.MoTa = model.MoTa;
            dv.GiaTien = model.GiaTien;
            dv.ThoiGian = model.ThoiGian;
            dv.TrangThai = model.TrangThai;
            db.SaveChanges();
            return Json(new { success = true, message = "Cap nhat thanh cong!" });
        }

        [HttpPost]
        public JsonResult XoaDichVu(int id)
        {
            if (!IsAdmin()) return Json(new { success = false });
            var dv = db.DichVuSpas.Find(id);
            if (dv == null) return Json(new { success = false });
            dv.TrangThai = false;
            db.SaveChanges();
            return Json(new { success = true });
        }

        public JsonResult ThongKe()
        {
            if (!IsAdmin()) return Json(null, JsonRequestBehavior.AllowGet);

            var data = db.DatLichSpas
                .GroupBy(l => new { l.NgayHen.Month, l.NgayHen.Year })
                .Select(g => new
                {
                    Thang = g.Key.Month,
                    Nam = g.Key.Year,
                    SoLuong = g.Count(),
                    DoanhThu = g.Sum(l => (decimal?)l.DichVuSpa.GiaTien) ?? 0
                })
                .OrderBy(x => x.Nam).ThenBy(x => x.Thang)
                .ToList();

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) db.Dispose();
            base.Dispose(disposing);
        }
    }
}