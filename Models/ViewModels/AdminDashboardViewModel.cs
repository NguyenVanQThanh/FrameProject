using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Models.ViewModels
{
    public class AdminDashboardViewModel
    {
        public List<decimal> totalInYear {set; get;}
        public int numberOrder {set; get;}

        public int numberOrderSuccess {set; get;}
        public int numberUser {set; get;}
        public int numberProduct {set; get;}
    }
}