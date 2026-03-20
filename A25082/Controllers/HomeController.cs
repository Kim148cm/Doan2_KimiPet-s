using A25082.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace A25082.Controllers
{
    public class HomeController : Controller
    {
        private AppDbContext db = new AppDbContext();

        public class HomeViewModel
        {
            public IEnumerable<LoaiKemChongNang> LoaiKemChongNangs { get; set; }
            public IEnumerable<Slider> Sliders { get; set; }
        }

        public ActionResult Index()
        {
            var viewModel = new HomeViewModel
            {
                LoaiKemChongNangs = db.LoaiKemChongNangs.ToList(),
                Sliders = db.Sliders.ToList()
            };
            return View(viewModel);
        }
    }
}