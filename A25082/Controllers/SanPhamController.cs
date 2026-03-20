using A25082.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace A25082.Controllers
{
    public class SanPhamController : Controller
    {
        private AppDbContext db = new AppDbContext();

        // GROQ API Key - thay bằng key thật của bạn
        private const string GROQ_API_KEY = "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";
        private const string GROQ_API_URL = "https://api.groq.com/openai/v1/chat/completions";
        private const string GROQ_MODEL = "llama-3.3-70b-versatile"; 

        public ActionResult Index()
        {
            return View();
        }

        //------------------------------------------------------------------------------------------------------
        public ActionResult LoadProductList(string sortOrder, decimal? minPrice, decimal? maxPrice)
        {
            var products = db.SanPhamKemChongNangs
                .Include(m => m.AnhKemChongNangs)
                .AsQueryable();

            if (minPrice.HasValue)
                products = products.Where(p => (p.GiaGiam > 0 ? p.GiaGiam : p.GiaGoc) >= minPrice.Value);

            if (maxPrice.HasValue)
                products = products.Where(p => (p.GiaGiam > 0 ? p.GiaGiam : p.GiaGoc) <= maxPrice.Value);

            switch (sortOrder)
            {
                case "PriceAsc":
                    products = products.OrderBy(p => p.GiaGiam > 0 ? p.GiaGiam : p.GiaGoc);
                    break;
                case "PriceDesc":
                    products = products.OrderByDescending(p => p.GiaGiam > 0 ? p.GiaGiam : p.GiaGoc);
                    break;
                default:
                    products = products.OrderByDescending(p => p.MaKem);
                    break;
            }

            return PartialView("_ProductList", products.ToList());
        }

        //------------------------------------------------------------------------------------------------------
        public ActionResult ProductHot()
        {
            var products = db.SanPhamKemChongNangs
                  .Include(m => m.LoaiAnh)
                  .Include(m => m.AnhKemChongNangs)
                  .ToList();
            return PartialView(products);
        }

        //------------------------------------------------------------------------------------------------------
        public ActionResult AllProduct()
        {
            var products = db.SanPhamKemChongNangs
                            .Include(m => m.LoaiAnh)
                            .ToList();
            return PartialView(products);
        }

        //------------------------------------------------------------------------------------------------------
        public ActionResult PageDeTailsProduct(int id)
        {
            var PagedetailProduct = db.SanPhamKemChongNangs
                            .Include(m => m.LoaiAnh)
                             .Include(m => m.AnhKemChongNangs)
                            .Where(p => p.MaKem == id)
                            .FirstOrDefault();

            if (PagedetailProduct == null) return null;

            return View(PagedetailProduct);
        }

        //------------------------------------------------------------------------------------------------------
        public ActionResult AllProductNew()
        {
            var Product_new = db.SanPhamKemChongNangs
                 .Include(m => m.LoaiAnh)
                .Where(p => p.MaLoai == 4)
                .ToList();
            return PartialView(Product_new);
        }

        //------------------------------------------------------------------------------------------------------
        public ActionResult AllClothers(string sortBy)
        {
            return View();
        }

        //------------------------------------------------------------------------------------------------------
        public ActionResult AllProductsByCategory(int categoryId)
        {
            var category = db.LoaiKemChongNangs.FirstOrDefault(c => c.MaLoai == categoryId);
            if (category == null) return HttpNotFound();

            var products = db.SanPhamKemChongNangs
                .Include(m => m.AnhKemChongNangs)
                .Where(p => p.MaLoai == categoryId)
                .ToList();

            ViewBag.CategoryName = category.TenLoai;
            return View(products);
        }

        public ActionResult CategoryProductLike()
        {
            var products = db.SanPhamKemChongNangs
               .Include(m => m.AnhKemChongNangs)
                .ToList();
            return PartialView(products);
        }

        //------------------------------------------------------------------------------------------------------
        public ActionResult AllDiscountProducts(string sortOrder)
        {
            var products = db.SanPhamKemChongNangs
                .Include(m => m.LoaiAnh)
                .Include(m => m.AnhKemChongNangs)
                .Where(p => p.GiaGiam > 0 && p.GiaGiam < p.GiaGoc)
                .AsQueryable();

            switch (sortOrder)
            {
                case "PriceAsc":
                    products = products.OrderBy(p => p.GiaGiam);
                    break;
                case "PriceDesc":
                    products = products.OrderByDescending(p => p.GiaGiam);
                    break;
                default:
                    products = products.OrderByDescending(p => (p.GiaGoc - p.GiaGiam));
                    break;
            }

            return View(products.ToList());
        }

        // =====================================================================
        // CHATBOT - Tích hợp Groq AI + Gợi ý sản phẩm
        // =====================================================================
        [HttpPost]
        public async Task<JsonResult> ChatBot(string message)
        {
            if (string.IsNullOrWhiteSpace(message))
                return Json(new { reply = "Bạn chưa nhập tin nhắn!" });

            string trimmed = message.Trim().ToLower();

            // --- 1. Tìm sản phẩm phù hợp trong DB ---
            var matchedProducts = db.SanPhamKemChongNangs
                .Where(p => p.TenKem.ToLower().Contains(trimmed))
                .Select(p => new
                {
                    p.MaKem,
                    p.TenKem,
                    Gia = p.GiaGiam > 0 ? p.GiaGiam : p.GiaGoc,
                    GiaGoc = p.GiaGoc,
                    GiaGiam = p.GiaGiam,
                    Anh = p.AnhKemChongNangs.FirstOrDefault().ImageUrl
                })
                .Take(5)
                .ToList();

            if (matchedProducts.Any())
            {
                string productCards = string.Join("", matchedProducts.Select(p =>
                    $@"<a href='/SanPham/PageDeTailsProduct/{p.MaKem}' target='_blank' class='chat-product-card'>
                        <div class='chat-product-img-wrap'>
                            {(string.IsNullOrEmpty(p.Anh) ? "<div class='no-img'>🐾</div>" : $"<img src='{p.Anh}' alt='{p.TenKem}' />")}
                            {(p.GiaGiam > 0 && p.GiaGiam < p.GiaGoc ? "<span class='chat-badge'>Sale</span>" : "")}
                        </div>
                        <div class='chat-product-info'>
                            <div class='chat-product-name'>{p.TenKem}</div>
                            <div class='chat-product-price'>
                                {(p.GiaGiam > 0 && p.GiaGiam < p.GiaGoc
                                    ? $"<span class='price-sale'>{p.GiaGiam:N0}đ</span> <span class='price-old'>{p.GiaGoc:N0}đ</span>"
                                    : $"<span class='price-main'>{p.GiaGoc:N0}đ</span>")}
                            </div>
                        </div>
                    </a>"
                ));

                string introText = await CallGroqForIntro(message, matchedProducts.Select(p => p.TenKem).ToList());
                return Json(new { reply = $"<p class='ai-intro'>{introText}</p><div class='chat-products-grid'>{productCards}</div>" });
            }

            // --- 2. Gọi Groq AI để trả lời tự nhiên ---
            string aiReply = await CallGroqAI(message);
            return Json(new { reply = aiReply });
        }

        // Gọi Groq AI - Trả lời tự nhiên
        private async Task<string> CallGroqAI(string userMessage)
        {
            try
            {
                // Lấy danh sách sản phẩm để làm context
                var allProducts = db.SanPhamKemChongNangs
                    .Select(p => new { p.TenKem, Gia = p.GiaGiam > 0 ? p.GiaGiam : p.GiaGoc })
                    .Take(20).ToList();

                string productContext = string.Join(", ", allProducts.Select(p => $"{p.TenKem} ({p.Gia:N0}đ)"));

                string systemPrompt = $@"Bạn là trợ lý AI thân thiện của cửa hàng thú cưng Kimipet's, chuyên bán thức ăn và phụ kiện cho thú cưng.
                Danh sách sản phẩm hiện có: {productContext}.
                Hãy trả lời ngắn gọn, thân thiện bằng tiếng Việt. Nếu khách hỏi sản phẩm cụ thể, hãy gợi ý từ danh sách trên.
                Không trả lời các chủ đề không liên quan đến thú cưng hoặc cửa hàng.";

                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Add("Authorization", $"Bearer {GROQ_API_KEY}");

                    var requestBody = new
                    {
                        model = GROQ_MODEL,
                        messages = new[]
                        {
                            new { role = "system", content = systemPrompt },
                            new { role = "user", content = userMessage }
                        },
                        max_tokens = 300,
                        temperature = 0.7
                    };

                    var json = JsonConvert.SerializeObject(requestBody);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");

                    var response = await client.PostAsync(GROQ_API_URL, content);
                    var responseString = await response.Content.ReadAsStringAsync();

                    dynamic result = JsonConvert.DeserializeObject(responseString);
                    return result?.choices?[0]?.message?.content?.ToString()
                           ?? "Xin lỗi, mình chưa hiểu câu hỏi. Bạn có thể hỏi về sản phẩm hoặc dịch vụ của shop nhé!";
                }
            }
            catch (Exception ex)
            {
                return "Xin lỗi, mình đang gặp sự cố kết nối. Vui lòng thử lại sau!";
            }
        }

        // Gọi Groq AI - Sinh câu giới thiệu sản phẩm
        private async Task<string> CallGroqForIntro(string query, List<string> productNames)
        {
            try
            {
                string names = string.Join(", ", productNames);
                string prompt = $"Viết 1 câu ngắn gọn, thân thiện giới thiệu những sản phẩm sau cho khách hàng tìm kiếm '{query}': {names}. Chỉ trả lời 1 câu duy nhất bằng tiếng Việt.";

                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Add("Authorization", $"Bearer {GROQ_API_KEY}");

                    var requestBody = new
                    {
                        model = GROQ_MODEL,
                        messages = new[]
                        {
                            new { role = "user", content = prompt }
                        },
                        max_tokens = 80,
                        temperature = 0.6
                    };

                    var json = JsonConvert.SerializeObject(requestBody);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");

                    var response = await client.PostAsync(GROQ_API_URL, content);
                    var responseString = await response.Content.ReadAsStringAsync();

                    dynamic result = JsonConvert.DeserializeObject(responseString);
                    return result?.choices?[0]?.message?.content?.ToString()
                           ?? $"Mình tìm thấy {productNames.Count} sản phẩm phù hợp cho bạn:";
                }
            }
            catch
            {
                return $"Mình tìm thấy {productNames.Count} sản phẩm phù hợp cho bạn:";
            }
        }
        //=======

        public ActionResult AllProductsByCategoryPET(int categoryId, string sortOrder = "",
    decimal? minPrice = null, decimal? maxPrice = null)
        {
            var category = db.LoaiKemChongNangs.FirstOrDefault(c => c.MaLoai == categoryId);
            if (category == null) return HttpNotFound();

            // Query sản phẩm
            var query = db.SanPhamKemChongNangs
                .Include(m => m.AnhKemChongNangs)
                .Include(m => m.LoaiKemChongNang)
                .Where(p => p.MaLoai == categoryId)
                .AsQueryable();

            // Lọc giá
            if (minPrice.HasValue)
                query = query.Where(p => (p.GiaGiam > 0 ? p.GiaGiam : p.GiaGoc) >= minPrice.Value);
            if (maxPrice.HasValue)
                query = query.Where(p => (p.GiaGiam > 0 ? p.GiaGiam : p.GiaGoc) <= maxPrice.Value);

            // Sắp xếp
            switch (sortOrder)
            {
                case "PriceAsc":
                    query = query.OrderBy(p => p.GiaGiam > 0 ? p.GiaGiam : p.GiaGoc);
                    break;
                case "PriceDesc":
                    query = query.OrderByDescending(p => p.GiaGiam > 0 ? p.GiaGiam : p.GiaGoc);
                    break;
                default:
                    query = query.OrderByDescending(p => p.MaKem);
                    break;
            }

            // Số lượng sản phẩm theo từng loại (cho sidebar)
            var productCounts = db.SanPhamKemChongNangs
                .GroupBy(p => p.MaLoai)
                .Select(g => new { MaLoai = g.Key, Count = g.Count() })
                .ToList();

            var allCategories = db.LoaiKemChongNangs
                .ToList()
                .Select(c => new LoaiKemChongNang
                {
                    MaLoai = c.MaLoai,
                    TenLoai = c.TenLoai
                   
                })
                .ToList();

            ViewBag.CategoryName = category.TenLoai;
            ViewBag.CurrentCategoryId = categoryId;
            ViewBag.SortOrder = sortOrder;
            ViewBag.MinPrice = minPrice;
            ViewBag.MaxPrice = maxPrice;
            ViewBag.AllCategories = allCategories;

            return View(query.ToList());
        }









        //======
    }
}