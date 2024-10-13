using DAL.DB;
using DAL.DB.Model;
using System.Collections.Generic;
using System.Linq;

namespace DAL.Repositories
{
    public class BikeRepository : IRepository<Bike>
    {
        private readonly Context _context;

        public BikeRepository(Context context)
        {
            _context = context;
        }

        public void Insert(Bike bike)
        {
            _context.Bikes.Add(bike);
            SaveData();
        }

        public void Update(Bike bike)
        {
            var existingBike = _context.Bikes.Find(bike.Id);
            if (existingBike != null)
            {
                _context.Entry(existingBike).CurrentValues.SetValues(bike);
                SaveData();
            }
        }

        public void Delete(Bike bike)
        {
            _context.Bikes.Remove(bike);
            SaveData();
        }

        public IEnumerable<Bike> GetAll()
        {
            return _context.Bikes.ToList();
        }

        public Bike GetById(int id)
        {
            return _context.Bikes.Find(id);
        }

        public Bike GetByName(string name)
        {
            return _context.Bikes.FirstOrDefault(b => b.Name == name);
        }

        public void SaveData()
        {
            _context.SaveChanges();
        }
    }
}
