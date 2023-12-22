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
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        private ApplicationDbContext _db;
        public ProductRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        List<Product> IProductRepository.GetForCategory(string category)
        {
            List<Product> products = _db.Products.Where(u => u.Category.Equals(category)).ToList();
            return products;
        }

        void IProductRepository.Save()
        {
            _db.SaveChanges();
        }

        void IProductRepository.Update(Product product)
        {
            _db.Products.Update(product);
        }
        
        void IProductRepository.Enable(int productId)
        {
            Product product = _db.Products.Find(productId); 
            if (product != null) {
                product.Enable = (product.Enable == true ? false : true);
            }
            _db.Products.Update(product);
            _db.SaveChanges();
        }
        int IProductRepository.newId()
        {
            List<Product> products = _db.Products.ToList();
            for (int i = 0; i < products.Count - 1; i++)
            {
                if (products[i + 1].Id - products[i].Id == 1)
                {
                    continue;
                }
                else
                {
                    return products[i].Id + 1;
                }
            }
            return products.Count + 1;
        }
    }
}
