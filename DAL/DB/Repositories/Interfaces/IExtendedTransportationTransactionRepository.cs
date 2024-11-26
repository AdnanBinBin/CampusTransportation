using DAL.DB.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.DB.Repositories.Interfaces
{
    public interface IExtendedTransportationTransactionRepository : IRepositoryInt<TransportationTransaction>
    {
        TransportationTransaction GetActiveTransactionByUserId(int userId);
        TransportationTransaction GetLatestTransactionForBike(string bikeId);
        IEnumerable<TransportationTransaction> GetAllTransactionsForSharedVehicule(string sharedVehiculeId);
    }
}
