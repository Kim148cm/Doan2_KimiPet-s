using A25082.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using System.IO;
using System.Threading.Tasks;
using A25082.Admin.Service;

namespace A25082.Admin.Controllers
{
    public class AdminPayMentOrderController : Controller
    {
        private AppDbContext db = new AppDbContext();
        private GHNService ghnService = new GHNService();

        // GET: AdminPayMentOrder
        public ActionResult Index()
        {
            var payments = db.ThanhToans
                .Include(t => t.ChiTietThanhToans.Select(c => c.SanPhamKemChongNang))
                .OrderByDescending(t => t.NgayTao)
                .ToList();

            return View(payments);
        }

        // ==================== XEM QUÁ TRÌNH VẬN CHUYỂN ====================
        public ActionResult Tracking(int id)
        {
            var payment = db.ThanhToans
                .Include(t => t.ChiTietThanhToans.Select(c => c.SanPhamKemChongNang))
                .FirstOrDefault(t => t.MaThanhToan == id);

            if (payment == null)
            {
                return HttpNotFound();
            }

            // Tạo view model chứa dữ liệu vận chuyển
            var trackingModel = new TrackingViewModel
            {
                MaThanhToan = payment.MaThanhToan,
                HoTen = payment.HoTen,
                SoDienThoai = payment.SoDienThoai,
                DiaChi = $"{payment.DiaChi}, {payment.Phuong}, {payment.Huyen}, {payment.Tinh}",
                SoTien = payment.SoTienThanhToan,
                Products = string.Join(", ", payment.ChiTietThanhToans.Select(x => $"{x.SanPhamKemChongNang.TenKem} (x{x.SoLuong})")),

                // Fake dữ liệu vận chuyển
                MaVanDonGHN = $"GHN{payment.MaThanhToan}",
                ShipperName = "Nguyễn Văn A",
                ShipperPhone = "0987654321",
                ShipperBike = "79-A1234",
                Distance = "8.5 km",
                EstimatedTime = "25 phút",

                // Tọa độ fake
                SenderLat = 10.7769,
                SenderLng = 106.7009,
                SenderAddress = "Kho hàng Quận 1, TP HCM",
                RecipientLat = 10.7597,
                RecipientLng = 106.6848,
                RecipientAddress = payment.DiaChi,
                ShipperLat = 10.7680,
                ShipperLng = 106.6920,

                // Timeline dữ liệu
                TimelineStatus = GetTrackingTimeline(payment.MaThanhToan)
            };

            return View(trackingModel);
        }

        // Hàm tạo timeline vận chuyển fake
        private List<TrackingStatus> GetTrackingTimeline(int paymentId)
        {
            return new List<TrackingStatus>
            {
                new TrackingStatus
                {
                    Status = "Đơn hàng được tạo",
                    Time = DateTime.Now.AddHours(-2).ToString("HH:mm - dd/MM/yyyy"),
                    IsCompleted = true,
                    IsCurrent = false
                },
                new TrackingStatus
                {
                    Status = "Shipper đang đến lấy hàng",
                    Time = DateTime.Now.AddHours(-1.5).ToString("HH:mm - dd/MM/yyyy"),
                    IsCompleted = true,
                    IsCurrent = false
                },
                new TrackingStatus
                {
                    Status = "Đã nhận hàng từ cửa hàng",
                    Time = DateTime.Now.AddHours(-1).ToString("HH:mm - dd/MM/yyyy"),
                    IsCompleted = true,
                    IsCurrent = false
                },
                new TrackingStatus
                {
                    Status = "Hàng đang được vận chuyển",
                    Time = DateTime.Now.ToString("HH:mm - dd/MM/yyyy"),
                    IsCompleted = false,
                    IsCurrent = true
                },
                new TrackingStatus
                {
                    Status = "Sắp đến nơi",
                    Time = DateTime.Now.AddMinutes(25).ToString("HH:mm - dd/MM/yyyy"),
                    IsCompleted = false,
                    IsCurrent = false
                },
                new TrackingStatus
                {
                    Status = "Hoàn thành giao hàng",
                    Time = DateTime.Now.AddMinutes(30).ToString("HH:mm - dd/MM/yyyy"),
                    IsCompleted = false,
                    IsCurrent = false
                }
            };
        }

        // ==================== GỌI SHIPPER ====================
        [HttpPost]
        public JsonResult CallShipper(int id)
        {
            try
            {
                var payment = db.ThanhToans.Find(id);
                if (payment == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy đơn hàng!" });
                }

                // Thực hiện gọi shipper (có thể tích hợp với API GHN hoặc dịch vụ SMS)
                return Json(new
                {
                    success = true,
                    message = "Đang gọi shipper...",
                    shipperPhone = "0987654321"
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Lỗi: " + ex.Message });
            }
        }

        // ==================== GỬI TIN NHẮN SHIPPER ====================
        [HttpPost]
        public JsonResult SendMessageShipper(int id, string message)
        {
            try
            {
                var payment = db.ThanhToans.Find(id);
                if (payment == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy đơn hàng!" });
                }

                // Lưu tin nhắn hoặc gửi SMS/API thực tế
                return Json(new
                {
                    success = true,
                    message = "Tin nhắn đã được gửi cho shipper!"
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Lỗi: " + ex.Message });
            }
        }

        [HttpPost]
        public JsonResult DeletePayment(int id)
        {
            try
            {
                var payment = db.ThanhToans.Find(id);
                if (payment == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy thanh toán!" });
                }

                db.ThanhToans.Remove(payment);
                db.SaveChanges();
                return Json(new { success = true, message = "Xóa thành công!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Lỗi hệ thống: " + ex.Message });
            }
        }

        // ==================== XÁC NHẬN VÀ TẠO ĐƠN GHN ====================
        [HttpPost]
        public async Task<JsonResult> ConfirmPayment(int id)
        {
            try
            {
                var payment = db.ThanhToans
                    .Include(t => t.ChiTietThanhToans.Select(c => c.SanPhamKemChongNang))
                    .FirstOrDefault(t => t.MaThanhToan == id);

                if (payment == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy thanh toán!" });
                }

                // Cập nhật trạng thái đơn hàng
                payment.TrangThai = "Đã xác nhận";
                db.SaveChanges();

                // ===== TẠO ĐƠN HÀNG TRÊN GHN =====
                var ghnResult = await ghnService.CreateOrderAsync(payment);

                if (ghnResult.Code == 200 && ghnResult.Data != null)
                {
                    // Lưu mã vận đơn GHN vào database (nếu bảng ThanhToan có cột MaVanDonGHN)
                    // payment.MaVanDonGHN = ghnResult.Data.OrderCode;
                    // db.SaveChanges();

                    return Json(new
                    {
                        success = true,
                        message = $"Xác nhận thành công! Mã vận đơn GHN: {ghnResult.Data.OrderCode}",
                        ghnOrderCode = ghnResult.Data.OrderCode,
                        expectedDelivery = ghnResult.Data.ExpectedDeliveryTime?.ToString("dd/MM/yyyy"),
                        shippingFee = ghnResult.Data.TotalFee
                    });
                }
                else
                {
                    // Vẫn xác nhận đơn nhưng báo lỗi GHN
                    return Json(new
                    {
                        success = true,
                        message = $"Đơn hàng đã xác nhận thành công",
                        ghnError = true
                    });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Lỗi hệ thống: " + ex.Message });
            }
        }

        [HttpPost]
        public JsonResult CancelConfirmation(int id)
        {
            try
            {
                var payment = db.ThanhToans.Find(id);
                if (payment == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy thanh toán!" });
                }

                payment.TrangThai = "Chờ xác nhận";
                db.SaveChanges();

                // LƯU Ý: Nếu đã tạo đơn GHN thì cần gọi API hủy đơn GHN
                // await ghnService.CancelOrderAsync(payment.MaVanDonGHN);

                return Json(new { success = true, message = "Hủy xác nhận thành công!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Lỗi hệ thống: " + ex.Message });
            }
        }

        [HttpGet]
        public JsonResult GetOrderDetail(int id)
        {
            try
            {
                var order = db.ThanhToans
                    .Include(o => o.ChiTietThanhToans)
                    .FirstOrDefault(o => o.MaThanhToan == id);

                if (order == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy đơn hàng" }, JsonRequestBehavior.AllowGet);
                }

                return Json(new
                {
                    success = true,
                    order = new
                    {
                        order.MaThanhToan,
                        order.HoTen,
                        order.Email,
                        order.SoDienThoai,
                        order.DiaChi,
                        order.Phuong,
                        order.Huyen,
                        order.Tinh,
                        order.PhuongThucThanhToan,
                        order.TrangThai,
                        order.SoTienThanhToan,
                        NgayTao = order.NgayTao.ToUniversalTime().ToString("O"),
                        ChiTietThanhToans = order.ChiTietThanhToans.Select(ct => new
                        {
                            ct.TenKem,
                            ct.SoLuong,
                            ct.DonGia,
                            ThanhTien = ct.SoLuong * ct.DonGia
                        })
                    }
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Có lỗi xảy ra: " + ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        // ==================== XUẤT FILE EXCEL ====================
        public ActionResult ExportConfirmedPayments()
        {
            var confirmedPayments = db.ThanhToans
                .Include(t => t.ChiTietThanhToans.Select(c => c.SanPhamKemChongNang))
                .Where(t => t.TrangThai == "Đã xác nhận")
                .OrderByDescending(t => t.NgayTao)
                .ToList();

            if (!confirmedPayments.Any())
            {
                return Json(new { success = false, message = "Chưa có đơn hàng nào được xác nhận!" }, JsonRequestBehavior.AllowGet);
            }

            var sb = new System.Text.StringBuilder();

            sb.AppendLine("<table border='1' style='font-family:Times New Roman; border-collapse: collapse;'>");
            sb.AppendLine("<tr style='font-weight:bold; background-color:#D3D3D3;'>");
            sb.AppendLine("<th>Mã TT</th>");
            sb.AppendLine("<th>Khách Hàng</th>");
            sb.AppendLine("<th>Email</th>");
            sb.AppendLine("<th>SĐT</th>");
            sb.AppendLine("<th>Địa chỉ</th>");
            sb.AppendLine("<th>Sản phẩm</th>");
            sb.AppendLine("<th>Tổng số lượng</th>");
            sb.AppendLine("<th>Số tiền (VNĐ)</th>");
            sb.AppendLine("<th>Ngày tạo</th>");
            sb.AppendLine("</tr>");

            foreach (var p in confirmedPayments)
            {
                string products = string.Join(", ",
                    p.ChiTietThanhToans.Select(c => $"{c.SanPhamKemChongNang.TenKem}({c.SoLuong})"));
                int totalQuantity = p.ChiTietThanhToans.Sum(c => c.SoLuong);
                string formattedMoney = string.Format("{0:N0} VNĐ", p.SoTienThanhToan);
                string fullAddress = $"{p.DiaChi}, {p.Phuong}, {p.Huyen}, {p.Tinh}";

                sb.AppendLine("<tr>");
                sb.AppendLine($"<td>{p.MaThanhToan}</td>");
                sb.AppendLine($"<td>{p.HoTen}</td>");
                sb.AppendLine($"<td>{p.Email}</td>");
                sb.AppendLine($"<td>{p.SoDienThoai}</td>");
                sb.AppendLine($"<td>{fullAddress}</td>");
                sb.AppendLine($"<td>{products}</td>");
                sb.AppendLine($"<td>{totalQuantity}</td>");
                sb.AppendLine($"<td>{formattedMoney}</td>");
                sb.AppendLine($"<td>{p.NgayTao:dd/MM/yyyy HH:mm}</td>");
                sb.AppendLine("</tr>");
            }

            sb.AppendLine("</table>");

            byte[] fileBytes = System.Text.Encoding.UTF8.GetBytes(sb.ToString());
            string fileName = $"DonHangXacNhan_{DateTime.Now:yyyyMMddHHmmss}.xls";

            return File(fileBytes, "application/vnd.ms-excel", fileName);
        }
    }


    // ==================== VIEW MODELS ====================
    public class TrackingViewModel
    {
        public int MaThanhToan { get; set; }
        public string HoTen { get; set; }
        public string SoDienThoai { get; set; }
        public string DiaChi { get; set; }
        public decimal SoTien { get; set; }
        public string Products { get; set; }

        public string MaVanDonGHN { get; set; }
        public string ShipperName { get; set; }
        public string ShipperPhone { get; set; }
        public string ShipperBike { get; set; }
        public string Distance { get; set; }
        public string EstimatedTime { get; set; }

        public double SenderLat { get; set; }
        public double SenderLng { get; set; }
        public string SenderAddress { get; set; }
        public double RecipientLat { get; set; }
        public double RecipientLng { get; set; }
        public string RecipientAddress { get; set; }
        public double ShipperLat { get; set; }
        public double ShipperLng { get; set; }

        public List<TrackingStatus> TimelineStatus { get; set; }
    }

    public class TrackingStatus
    {
        public string Status { get; set; }
        public string Time { get; set; }
        public bool IsCompleted { get; set; }
        public bool IsCurrent { get; set; }
    }
}