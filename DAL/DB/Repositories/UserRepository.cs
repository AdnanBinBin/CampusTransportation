using DAL.DB;
using DAL.DB.Model;
using DAL.DB.Repositories;
using System.Collections.Generic;
using System.Linq;

namespace DAL.Repositories
{
    public class UserRepository : IRepositoryInt<User>
    {
        private readonly Context _context;

        public UserRepository(Context context)
        {
            _context = context;
        }

        public void Insert(User entity)
        {
            _context.Users.Add(entity);
            SaveData();
        }

        public void Update(User entity)
        {
            _context.Users.Update(entity);
            SaveData();
        }

        public void Delete(User entity)
        {
            _context.Users.Remove(entity);
            SaveData();
        }

        public IEnumerable<User> GetAll()
        {
            return _context.Users.ToList();
        }

        public User GetById(int id)
        {
            return _context.Users.Find(id);
        }

        public void SaveData()
        {
            _context.SaveChanges();
        }
    }
}
