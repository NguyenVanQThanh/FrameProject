using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataAccess.Data;
using DataAccess.Repository.IRepository;
using Models;

namespace DataAccess.Repository
{
    public class OrderRepository : Repository<Order>, IOrderRepository
    {
        private ApplicationDbContext _db;
        public OrderRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void SaveAndClear(int orderId, List<Cart> carts)
        {
            // _db.Orders.Add(order);
            // _db.SaveChanges();
            foreach(var cart in carts){
                OrderDetail orderDetail = new OrderDetail{
                    IdOrder = orderId,
                    IdProduct = cart.ProductId,
                    Quantity = cart.Quantity,
                    Total = (decimal) (cart.Quantity * cart.Product.Price)
                };
                _db.OrderDetails.Add(orderDetail);
                _db.Carts.Remove(cart);
                _db.SaveChanges();
            }
            _db.SaveChanges();
        }

        public decimal totalOrderOfUser(string UserId)
        {
            throw new NotImplementedException();
        }
    }
}