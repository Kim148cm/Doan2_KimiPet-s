using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace A25082.Models
{
    public class CartItem
    {
        public SanPhamKemChongNang SanPhamKemChongNang { get; set; }
        public int Quantity { get; set; }
        public string SelectedColor { get; set; }
        public string SelectedSize { get; set; }
        public decimal Price { get; set; }
        public decimal DiscountPrice { get; set; }
        public decimal TotalPrice => Quantity * (DiscountPrice > 0 ? DiscountPrice : Price);
    }
}