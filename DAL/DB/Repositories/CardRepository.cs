using DAL.DB;
using DAL.DB.Model;
using System.Collections.Generic;
using System.Linq;

namespace DAL.Repositories
{
    public class CardRepository : IRepository<Card>
    {
        private readonly Context _context;

        public CardRepository(Context context)
        {
            _context = context;
        }

        public void Insert(Card entity)
        {
            _context.Cards.Add(entity);
            SaveData();
        }

        public void Update(Card entity)
        {
            _context.Cards.Update(entity);
            SaveData();
        }

        public void Delete(Card entity)
        {
            _context.Cards.Remove(entity);
            SaveData();
        }

        public IEnumerable<Card> GetAll()
        {
            return _context.Cards.ToList();
        }

        public Card GetById(int id)
        {
            return _context.Cards.Find(id);
        }

        public void SaveData()
        {
            _context.SaveChanges();
        }
    }
}
