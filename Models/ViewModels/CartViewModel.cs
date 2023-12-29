using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Models.ViewModels
{
    public class CartViewModel
    {
        public List<Cart> cartsList {set; get;}
        public List<Product>? productList {set; get;}
        public double total {set; get;}
    }
}