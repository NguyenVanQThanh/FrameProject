using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Models.ViewModels
{
    public class CheckoutViewModel
    {
        public User? user {get; set;}
        public double total;
    }
}