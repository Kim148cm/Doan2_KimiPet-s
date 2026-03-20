using A25082.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;

namespace A25082.Controllers
{
    public class GioHangController : Controller
    {
        private AppDbContext db = new AppDbContext();

        // ------------------------------------------------------------------
        public ActionResult Index()
        {
            return View(GetCart());
        }
        // ------------------------------------------------------------------

        public List<GioHang> GetCart()
        {
            var cart = Session["Cart"] as List<GioHang>;

            if (cart == null)
            {
                cart = new List<GioHang>();
                Session["Cart"] = cart;
                //  Session.Timeout = 30;
            }


            return cart;
        }
        public ActionResult GetCartSidebar()
        {
            var cart = Session["Cart"] as List<GioHang> ?? new List<GioHang>();

            int totalItems = cart.Sum(c => c.SoLuong);
            decimal totalPrice = cart.Sum(c => c.TongThanhToan);

            foreach (var item in cart)
            {
                var product = db.SanPhamKemChongNangs
                    .Include(p => p.AnhKemChongNangs)
                    .FirstOrDefault(p => p.MaLoaiAnh == item.MaKem);

                if (product != null)
                {
                    item.ImageUrl = item.ImageUrl ?? product.AnhKemChongNangs?.FirstOrDefault()?.ImageUrl ?? "/images/default.jpg";
                }
            }

            ViewBag.TotalItems = totalItems;
            ViewBag.TotalPrice = totalPrice;

            return PartialView("_CartSidebarPartial", cart);
        }

        // Thêm vào giỏ hàng và lưu tron Session ------------------------------------------------------------------

        [HttpPost]
        public JsonResult AddToCart(int productId, string color, string size, int quantity)
        {
            var cart = GetCart();

            var product = db.SanPhamKemChongNangs.Find(productId); // tìm sản phẩm trong DB nếu có sẽ thêm thành công 

            if (product == null)
            {
                return Json(new { success = false, message = "Sản phẩm không tồn tại!" }); // không có sẽ thêm Fail 
            }


            // Kiểm tra sản phẩm đã có trong giỏ hàng hay chưa ? 
            var existingItem = cart.FirstOrDefault(c => c.MaKem == productId);

            if (existingItem != null)
            {
                existingItem.SoLuong += quantity;
                existingItem.TongThanhToan = existingItem.GiaGoc * existingItem.SoLuong;
            }
            else // khác thì thêm mới vào Cart 
            {
                cart.Add(new GioHang
                {
                    MaGioHang = cart.Count + 1,
                    MaKem = productId,
                    TenKem = product.TenKem,
                    ImageUrl = product.AnhKemChongNangs?.FirstOrDefault()?.ImageUrl ?? "/images/default.jpg",
                    GiaGoc = product.GiaGoc,
                    GiaGiam = product.GiaGiam,
                    SoLuong = quantity,
                    TongThanhToan = product.GiaGiam * quantity,
                });
            }

            Session["Cart"] = cart; // lưu lại vào Session 
            // Session.Timeout = 30;

            int totalItems = cart.Sum(c => c.SoLuong);
            decimal totalPrice = cart.Sum(c => c.TongThanhToan);



            return Json(new
            {
                success = true,
                message = "Thêm vào giỏ hàng thành công!",
                totalItems,
                totalPrice = totalPrice.ToString("N0") + "đ"
            });


        }


        //-------------------------------------------------------------------------------------------------------------------------- 
        private string RenderRazorViewToString(string viewName, object model)
        {
            ViewData.Model = model;
            using (var sw = new System.IO.StringWriter())
            {
                var viewResult = ViewEngines.Engines.FindPartialView(ControllerContext, viewName);
                var viewContext = new ViewContext(ControllerContext, viewResult.View, ViewData, TempData, sw);
                viewResult.View.Render(viewContext, sw);
                return sw.GetStringBuilder().ToString();
            }
        }


        // Xóa sản phẩm khỏi Sidebar khi thêm giỏ hàng --------------------------------------------------------------------------------------- 

        [HttpPost]
        public JsonResult RemoveFromCartSideBar(int productId, string color, string size)
        {
            var cart = GetCart();

            var itemToRemove = cart.FirstOrDefault(c => c.MaKem == productId);

            if (itemToRemove != null)
            {
                cart.Remove(itemToRemove);
                Session["Cart"] = cart;
            }

            // Tính lại tổng số lượng và tổng tiền 
            int totalItems = cart.Sum(c => c.SoLuong);
            decimal totalPrice = cart.Sum(c => c.TongThanhToan);

            string cartHtml = RenderRazorViewToString("_CartSidebarPartial", cart);

            return Json(new
            {
                success = true,
                message = "Xóa sản phẩm khỏi giỏ hàng thành công!",
                cartHtml,
                totalItems,
                totalPrice = totalPrice.ToString("N0") + "đ"
            });
        }
        //------------------------------------------------------------------------------------------------------------------------------------

        // Code thay đổi thông tin sản phẩm trong giỏ hàng --------------------------------------------------------------------------------------- 
        [HttpGet]
        public JsonResult GetProductOptions(int productId)
        {


            var product = db.SanPhamKemChongNangs.FirstOrDefault(p => p.MaKem == productId);
            var price = product?.GiaGoc ?? 0;

            var nameProduct = product?.TenKem ?? "không có tên sản phẩm";

            var disPrice = product?.GiaGiam ?? 0;

            // Lấy ra hình ảnh đầu tiên 
            var imageUrls = db.AnhKemChongNangs
                .Where(i => i.MaKem == productId)
                .Select(i => i.ImageUrl)
                .ToList();

            return Json(new { nameProduct = nameProduct, imageUrl = imageUrls, price, disPrice }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult UpdateCartItem(int productId, string color, string size)
        {
            var cart = GetCart();
            var item = cart.FirstOrDefault(c => c.MaKem == productId);


            Session["Cart"] = cart;
            return Json(new { success = true });
        }

        // ------------------------------------------------------------------------------------------------------------------------------------

        // Trang giỏ hàng + hiển thị hình ảnh ------------------------------------------------------------------

        public ActionResult ViewCart()
        {
            var cart = GetCart();

            foreach (var item in cart)
            {
                var product = db.SanPhamKemChongNangs
                    .Include(p => p.AnhKemChongNangs)
                    .FirstOrDefault(p => p.MaKem == item.MaKem);  // Sửa điều kiện ở đây

                if (product != null && product.AnhKemChongNangs != null && product.AnhKemChongNangs.Any())
                {
                    item.ImageUrl = product.AnhKemChongNangs.First().ImageUrl ?? "/images/default.jpg";
                }
                else
                {
                    item.ImageUrl = "/images/default.jpg"; // Ảnh mặc định nếu không có
                }
            }

            return View(cart);
        }

        // Xóa sản phẩm ------------------------------------------------------------------

        [HttpPost]
        public JsonResult RemoveFromCart(int productId, string color, string size)
        {
            var cart = GetCart();
            var itemToRemove = cart.FirstOrDefault(c => c.MaKem == productId);

            if (itemToRemove != null)
            {
                cart.Remove(itemToRemove);
                Session["Cart"] = cart;
                return Json(new { success = true, message = "Xóa sản phẩm khỏi giỏ hàng thành công!" });
            }

            return Json(new { success = false, message = "Sản phẩm không tồn tại trong giỏ hàng!" });
        }



        // Code cập nhật số lượng sản phẩm trong giỏ hàng --------------------------------------------------------------------------------------- 
        [HttpPost]
        public ActionResult UpdateQuantity(int productId, string color, string size, bool increase)
        {
            var cart = Session["Cart"] as List<GioHang>;
            if (cart != null)
            {
                var item = cart.FirstOrDefault(x => x.MaKem == productId);
                if (item != null)
                {
                    if (increase)
                    {
                        item.SoLuong++;
                    }
                    else
                    {
                        if (item.SoLuong > 1)
                        {
                            item.SoLuong--;
                        }
                        else
                        {
                            return Json(new { success = false, message = "Số lượng không thể nhỏ hơn 1." });
                        }
                    }

                    Session["Cart"] = cart;
                    return Json(new { success = true });
                }
            }
            return Json(new { success = false, message = "Sản phẩm không tồn tại trong giỏ hàng." });
        }

        //--------------------------------------------------------------------------------------------





    }
}