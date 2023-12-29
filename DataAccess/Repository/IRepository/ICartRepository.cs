using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repository.IRepository
{
    public interface ICartRepository : IRepository<Cart>
    {
        IEnumerable<Cart> FindCartOfUser(string userId);
        void Update(Cart cart);
        void Save();
        double Total(List<Cart> carts);
    }
}
