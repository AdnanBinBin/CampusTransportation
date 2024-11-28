using DAL.DB.Model;
using System;
using System.Collections.Generic;

namespace BLL.Services
{
    public interface ITransportationService
    {
        bool BoardShuttle(int userId, string shuttleId);
        bool EndBikeRental(int userId, string bikeId, DateTime rentalEndTime);
        List<TransportationTransaction> GetUserTransportationTransactions(int userId);
        List<PaymentTransaction> GetUserPaymentTransactions(int userId);

        bool RentBike(int userId, string bikeId, out DateTime rentalStartTime);
        bool CreateSharedVehicleTrip(int driverId, string sharedVehicleId, DateTime rentalStartTime);
        bool RentSharedVehicle(int userId, string sharedVehicleId, out DateTime rentalStartTime, int driverId);
        bool EndSharedVehicleRental(string sharedVehicleId, int driverId);
    }
}
