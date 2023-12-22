using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repository.IRepository
{
    public interface IProductRepository : IRepository<Product>
    {
        List<Product> GetForCategory(string category);
        void Update(Product product);

        void Save();
        void Enable(int productId);
        int newId();
    }
}
