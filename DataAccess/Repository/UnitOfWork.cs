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
    public class UnitOfWork : IUnitOfWork
    {
        private ApplicationDbContext _db;
        public IUserRepository User {  get;private set; }
        public IProductRepository Product { get;private set; }

        public ICartRepository Cart { get; private set; }
        public IOrderRepository Order {get; private set;}
        public IOrderDetailRepository OrderDetail {get; private set;}
        public UnitOfWork (ApplicationDbContext db)
        {
            _db = db;
            Cart = new CartRepository(_db);
            User = new UserRepository(_db);
            Product = new ProductRepository(_db);
            Order = new OrderRepository(_db);
            OrderDetail = new OrderDetailRepository(_db);
        }

        public void Save()
        {
            _db.SaveChanges();
        }
    }
}
