using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Models.ViewModels
{
    public class DetailOrderViewModel
    {
        public List<OrderDetail> orderDetails {set; get;}
        public Order order {set; get;}
        public decimal total {set; get;}
        public User? user {set; get;}
        public List<string> listStatus = new List<string>{
            "Đã đặt hàng",
            "Đang chuẩn bị",
            "Đang giao hàng",
            "Đợi xác nhận hủy",
            "Đã hủy",
            "Thành công"
        };
    }
}