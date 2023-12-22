using DataAccess.Data;
using DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Identity;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repository
{
    public class UserRepository : Repository<User>, IUserRepository
    {
        private ApplicationDbContext _db;
        //private readonly UserManager<User> _userManager;
        public UserRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
            //_userManager = userManager;
        }

        void IUserRepository.Save() { 
            _db.SaveChanges();
        }
        void IUserRepository.Update(User user) {
            _db.Users.Update(user);
        }

        void IUserRepository.Enable(string userId)
        {
            User user = _db.Users.Find(userId);
            if (user != null)
            {
                user.Enable = (user.Enable == true ? false : true);
            }
            _db.Users.Update(user);
            _db.SaveChanges();
        }

        public List<User> GetByRole(string role)
        {
            List<User> users = _db.Users.Where(u => u.Role.Equals(role)).ToList();
            return users;
        }
    }
}
