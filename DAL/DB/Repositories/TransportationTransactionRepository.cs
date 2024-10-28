using DAL.DB.Model;
using DAL.DB;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using DAL.DB.Repositories;

namespace DAL.Repositories
{
    public class TransportationTransactionRepository : IRepositoryInt<TransportationTransaction>
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
        public TransportationTransaction GetLatestTransactionForBike(string bikeId)
        {
            return _context.TransportationTransactions
                             .Where(t => t.BikeId == bikeId)  // Filtrer par bikeId
                             .OrderByDescending(t => t.Date)  // Trier par date décroissante
                             .FirstOrDefault();              // Prendre la transaction la plus récente
        }

        public TransportationTransaction GetLatestTransactionForShuttle(string shuttleId)
        {
            return _context.TransportationTransactions
                             .Where(t => t.ShuttleId == shuttleId)  // Filtrer par shuttleId
                             .OrderByDescending(t => t.Date)  // Trier par date décroissante
                             .FirstOrDefault();              // Prendre la transaction la plus récente
        }

        public TransportationTransaction GetLatestTransactionForSharedVehicule(string sharedVehiculeId)
        {
            return _context.TransportationTransactions
                             .Where(t => t.SharedVehiculeId == sharedVehiculeId)  // Filtrer par sharedVehiculeId
                             .OrderByDescending(t => t.Date)  // Trier par date décroissante
                             .FirstOrDefault();              // Prendre la transaction la plus récente
        }

        public IEnumerable<TransportationTransaction> GetAllTransactionsForSharedVehicule(string sharedVehicleId)
        {
            return _context.TransportationTransactions
                .Where(t => t.SharedVehiculeId == sharedVehicleId)
                .ToList();
        }


        public TransportationTransaction GetActiveTransactionByUserId(int userId)
        {
            return _context.TransportationTransactions
                           .Where(t => t.UserId == userId && t.RentalEndTime == null)
                           .FirstOrDefault();
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
