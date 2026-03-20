using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace A25082.Models
{
    public class ProductDetails
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public decimal Disprice { get; set; }
        public string PrimaryImageUrl { get; set; }
        public string HoverImageUrl { get; set; }
        public List<string> Colors { get; set; }
    }
}