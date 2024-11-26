using DAL.DB.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.DB.Repositories.Interfaces
{
    public interface IExtendedPaymentTransactionRepository : IRepositoryInt<PaymentTransaction>
    {
        PaymentTransaction GetLatestPaymentTransactionByUserId(int userId);
    }
}
