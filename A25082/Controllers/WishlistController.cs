using A25082.Models;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Data.Entity;

namespace A25082.Controllers
{
    public class WishlistController : Controller
    {
        private AppDbContext db = new AppDbContext();
        private const string KEY = "Wishlist";

        private List<int> GetList()
        {
            var l = Session[KEY] as List<int>;
            if (l == null) { l = new List<int>(); Session[KEY] = l; }
            return l;
        }

        // GET: /Wishlist
        public ActionResult Index()
        {
            var ids = GetList();
            var products = ids.Any()
                ? db.SanPhamKemChongNangs
                      .Include(p => p.AnhKemChongNangs)
                      .Include(p => p.LoaiKemChongNang)
                      .Where(p => ids.Contains(p.MaKem))
                      .ToList()
                : new List<SanPhamKemChongNang>();

            ViewBag.WishlistIds = ids;
            return View(products);
        }

        // POST: /Wishlist/Toggle
        [HttpPost]
        public JsonResult Toggle(int productId)
        {
            var list = GetList();
            bool added = !list.Contains(productId);
            if (added) list.Add(productId); else list.Remove(productId);
            Session[KEY] = list;
            return Json(new
            {
                success = true,
                added,
                count = list.Count,
                message = added ? "Đã thêm vào yêu thích!" : "Đã xoá khỏi yêu thích!"
            });
        }

        // POST: /Wishlist/Remove
        [HttpPost]
        public JsonResult Remove(int productId)
        {
            var list = GetList();
            list.Remove(productId);
            Session[KEY] = list;
            return Json(new { success = true, count = list.Count });
        }

        // POST: /Wishlist/ClearAll
        [HttpPost]
        public JsonResult ClearAll()
        {
            Session[KEY] = new List<int>();
            return Json(new { success = true });
        }

        // GET: /Wishlist/Count
        public JsonResult Count()
        {
            return Json(new { count = GetList().Count }, JsonRequestBehavior.AllowGet);
        }

        // GET: /Wishlist/GetIds  (dùng để sync UI khi load trang)
        public JsonResult GetIds()
        {
            return Json(GetList(), JsonRequestBehavior.AllowGet);
        }
    }
}