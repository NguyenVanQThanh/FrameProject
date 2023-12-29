using DataAccess.Data;
using DataAccess.Repository.IRepository;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repository
{
    public class CartRepository : Repository<Cart> , ICartRepository
    {
        private ApplicationDbContext _db;
        public CartRepository(ApplicationDbContext db) : base(db) 
        {
            _db = db;
        }

        public IEnumerable<Cart> FindCartOfUser(string userId)
        {
            List<Cart> carts =  _db.Carts.Where(u=> u.UserId == userId).ToList();
            return carts;
            
        }

        public void Save()
        {
           _db.SaveChanges();
        }

        public double Total(List<Cart> carts)
        {
            double result = 0;
            foreach (Cart c in carts)
            {
                if (c.Product!=null){
                result += c.Quantity * c.Product.Price;
                }else {
                    Product product = _db.Products.Find(c.ProductId);
                    c.Product = product;
                    result+=c.Quantity*product.Price;
                }
            }
            return result;
        }

        public double totalCart(List<Cart> carts)
        {
            double total = 0;
            foreach(var obj in carts){
                total += obj.Quantity * obj.Product.Price;
            }
            return total;
        }

        public void Update(Cart cart)
        {
            _db.Carts.Update(cart);
        }
    }
}
