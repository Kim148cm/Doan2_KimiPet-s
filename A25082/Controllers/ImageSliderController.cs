using A25082.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace A25082.Controllers
{
    public class ImageSliderController : Controller
    {
        // GET: ImageSlider
        private AppDbContext db = new AppDbContext();
        public ActionResult Index()
        {
            var sliderImage = db.Sliders.ToList();
            return PartialView(sliderImage);
        }
    }
}