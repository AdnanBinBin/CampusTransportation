using DAL.DB;
using DAL.DB.Model;
using System.Collections.Generic;
using System.Linq;

namespace DAL.Repositories
{
    public class SharedVehiculeRepository : IRepository<SharedVehicule>
    {
        private readonly Context _context;

        public SharedVehiculeRepository(Context context)
        {
            _context = context;
        }

        public void Insert(SharedVehicule sharedVehicule)
        {
            _context.SharedVehicules.Add(sharedVehicule);
            SaveData();
        }

        public void Update(SharedVehicule sharedVehicule)
        {
            var existingSharedVehicule = _context.SharedVehicules.Find(sharedVehicule.Id);
            if (existingSharedVehicule != null)
            {
                _context.Entry(existingSharedVehicule).CurrentValues.SetValues(sharedVehicule);
                SaveData();
            }
        }

        public void Delete(SharedVehicule sharedVehicule)
        {
            _context.SharedVehicules.Remove(sharedVehicule);
            SaveData();
        }

        public IEnumerable<SharedVehicule> GetAll()
        {
            return _context.SharedVehicules.ToList();
        }

        public SharedVehicule GetById(int id)
        {
            return _context.SharedVehicules.Find(id);
        }

        public SharedVehicule GetByName(string name)
        {
            return _context.SharedVehicules.FirstOrDefault(v => v.Name == name);
        }

        public void SaveData()
        {
            _context.SaveChanges();
        }
    }
}
