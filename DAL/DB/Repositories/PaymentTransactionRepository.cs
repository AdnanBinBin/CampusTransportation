using DAL.DB;
using DAL.DB.Model;
using DAL.DB.Repositories;
using DAL.DB.Repositories.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace DAL.Repositories
{
    public class PaymentTransactionRepository : IExtendedPaymentTransactionRepository
    {
        private readonly Context _context;

        public PaymentTransactionRepository(Context context)
        {
            _context = context;
        }

        public void Insert(PaymentTransaction paymentTransaction)
        {
            _context.PaymentTransactions.Add(paymentTransaction);
            SaveData();
        }

        public void Update(PaymentTransaction paymentTransaction)
        {
            var existingPaymentTransaction = _context.PaymentTransactions.Find(paymentTransaction.Id);
            if (existingPaymentTransaction != null)
            {
                _context.Entry(existingPaymentTransaction).CurrentValues.SetValues(paymentTransaction);
                SaveData();
            }
        }

        public PaymentTransaction GetLatestPaymentTransactionByUserId(int userId)
        {
            return _context.PaymentTransactions
                           .Where(pt => pt.UserId == userId)
                           .OrderByDescending(pt => pt.Date)
                           .FirstOrDefault();
        }



        public void Delete(PaymentTransaction paymentTransaction)
        {
            _context.PaymentTransactions.Remove(paymentTransaction);
            SaveData();
        }

        public IEnumerable<PaymentTransaction> GetAll()
        {
            return _context.PaymentTransactions.ToList();
        }

        public PaymentTransaction GetById(int id)
        {
            return _context.PaymentTransactions.Find(id);
        }

        public void SaveData()
        {
            _context.SaveChanges();
        }
    }
}
