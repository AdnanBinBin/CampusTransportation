using DAL.DB.Model;

namespace BLL.Services
{
    public interface ITransportationService
    {
        bool BoardShuttle(int userId, string shuttleId);
        bool RentBike(int userId, string bikeId, out DateTime rentalStartTime);
        bool EndBikeRental(int userId, string bikeId, DateTime rentalEndTime);
        List<TransportationTransaction> GetUserTransportationTransactions(int userId);
    }

}
