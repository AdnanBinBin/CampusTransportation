using DAL.DB.Model;
using DAL.DB;
using System.Collections.Generic;
using System.Linq;

namespace DAL.Repositories
{
    public class TransportationTransactionRepository : IRepository<TransportationTransaction>
    {
        private readonly Context _context;

        public TransportationTransactionRepository(Context context)
        {
            _context = context;
        }

        public void Insert(TransportationTransaction transaction)
        {
            _context.TransportationTransactions.Add(transaction);
            SaveData();
        }

        public void Update(TransportationTransaction transaction)
        {
            var existingTransaction = _context.TransportationTransactions.Find(transaction.Id);
            if (existingTransaction != null)
            {
                _context.Entry(existingTransaction).CurrentValues.SetValues(transaction);
                SaveData();
            }
        }

        public void Delete(TransportationTransaction transaction)
        {
            _context.TransportationTransactions.Remove(transaction);
            SaveData();
        }

        public void SaveData()
        {
            _context.SaveChanges();
        }

        public IEnumerable<TransportationTransaction> GetAll()
        {
            return _context.TransportationTransactions.ToList();
        }

        // Implémentation de GetById
        public TransportationTransaction GetById(int id)
        {
            return _context.TransportationTransactions.Find(id);
        }
    }
}
