using A25082.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
namespace A25082.Controllers
{
    public class SearchController : Controller
    {
        // GET: Search
        private AppDbContext db = new AppDbContext();
        public ActionResult Index()
        {
            return View();
        }

        // Load thông tin sản phẩm khi gõ từ khóa tìm kiếm ---------------------------------------------------------------------
        public ActionResult LoadProductSearch(string sortOrder, string query)
        {
            if (string.IsNullOrEmpty(query))
            {
                return PartialView("_ProductListSearch", new List<SanPhamKemChongNang>());
            }

            var products = db.SanPhamKemChongNangs
                 .Include(m => m.LoaiAnh)
                   .Include(m => m.AnhKemChongNangs)
                .Where(p => p.TenKem.Contains(query))
                .ToList();

            ViewBag.SearchQuery = query;

            switch (sortOrder)
            {
                case "PriceAsc":
                    products = products.OrderBy(p => p.GiaGoc).ToList();
                    break;
                case "PriceDesc":
                    products = products.OrderByDescending(p => p.GiaGoc).ToList();
                    break;
            }

            return PartialView("_ProductListSearch", products);
        }

        // ------------------------------------------------------------------------------------------------------------------
        public JsonResult SuggestProducts(string query)
        {
            if (string.IsNullOrEmpty(query))
            {
                return Json(new List<object>(), JsonRequestBehavior.AllowGet); // Trả về list rỗng thay vì object message
            }

            var products = db.SanPhamKemChongNangs
                             .Where(p => p.TenKem.Contains(query))
                             .Select(p => new
                             {
                                 MaKem = p.MaKem,
                                 TenKem = p.TenKem,
                                 GiaGoc = p.GiaGoc,
                                 ImageUrl = p.AnhKemChongNangs.Select(a => a.ImageUrl).FirstOrDefault() ?? "/images/no-image.png"
                             })
                             .Take(5)
                             .ToList();

            return Json(products, JsonRequestBehavior.AllowGet);
        }



        // ------------------------------------------------------------------------------------------------------------------


    }
}