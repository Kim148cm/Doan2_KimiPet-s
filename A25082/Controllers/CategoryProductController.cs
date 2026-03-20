using A25082.Models;
using System.Linq;
using System.Web.Mvc;
using System.Data.Entity;

namespace A25082.Controllers
{
    public class CategoryProductController : Controller
    {
        private AppDbContext db = new AppDbContext();

        // Lấy sản phẩm theo danh mục (dùng ajax)
        public ActionResult ProductsByCategory(int? categoryId)
        {
            var products = (!categoryId.HasValue || categoryId.Value == 0)
                ? db.SanPhamKemChongNangs.Include(p => p.AnhKemChongNangs).ToList()
                : db.SanPhamKemChongNangs
                    .Include(p => p.AnhKemChongNangs)
                    .Where(p => p.MaLoai == categoryId.Value)
                    .ToList();

            return PartialView("_ProductPartial", products);
        }
    }
}
