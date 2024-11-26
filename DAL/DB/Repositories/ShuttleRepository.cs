using DAL.DB;
using DAL.DB.Model;
using DAL.DB.Repositories.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace DAL.Repositories
{
    public class ShuttleRepository : IExtendedShuttleRepository
    {
        private readonly Context _context;

        public ShuttleRepository(Context context)
        {
            _context = context;
        }

        public void Insert(Shuttle shuttle)
        {
            _context.Shuttles.Add(shuttle);
            SaveData();
        }

        public void Update(Shuttle shuttle)
        {
            var existingShuttle = _context.Shuttles.Find(shuttle.Id);
            if (existingShuttle != null)
            {
                _context.Entry(existingShuttle).CurrentValues.SetValues(shuttle);
                SaveData();
            }
        }

        public void Delete(Shuttle shuttle)
        {
            _context.Shuttles.Remove(shuttle);
            SaveData();
        }

        public IEnumerable<Shuttle> GetAll()
        {
            return _context.Shuttles.ToList();
        }

        public Shuttle GetById(string id)
        {
            return _context.Shuttles.Find(id);
        }

        public Shuttle GetByName(string name)
        {
            return _context.Shuttles.FirstOrDefault(s => s.Name == name);
        }

        public void SaveData()
        {
            _context.SaveChanges();
        }
    }
}
