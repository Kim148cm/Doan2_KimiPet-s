using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace A25082.Models
{
    public class ProductDiscount
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal MinPrice { get; set; }
        public decimal MaxPrice { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}