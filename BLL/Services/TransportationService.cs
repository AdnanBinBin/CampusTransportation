using DAL.DB.Model;
using DAL.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Services
{
    public class TransportationService : ITransportationService
    {

        private readonly ShuttleRepository _shuttleRepository;
        private readonly BikeRepository _bikeRepository;
        private readonly TransportationTransactionRepository _transportationTransactionRepository;
        private readonly PaymentTransactionRepository _paymentRepository;
        private readonly SharedVehiculeRepository _sharedVehiculeRepository;
        private readonly UserRepository _userRepository;
        public bool BoardShuttle(int userId, string shuttleId)
        {
            try
            {
                if (IsUserDisabled(userId))
                {
                    return false;
                }

                var shuttle = _shuttleRepository.GetByName(shuttleId);
                if (shuttle == null || !shuttle.IsAvailable)
                {
                    return false;
                }

                var user = _userRepository.GetById(userId);
                

                if (user.Card == null)
                {
                    throw new Exception("User does not have an associated card.");
                }

                bool IsStudentRefunded = IsUserStateFunded(userId);
                CreateTransportationTransaction(userId, shuttleId, null, DateTime.Now, DateTime.Now, DateTime.Now.AddHours(1));
                CreatePaymentTransaction(userId, shuttle.Price, user.Card.PaymentMethod, IsStudentRefunded);

                return true;
            }
            catch (DbUpdateException dbEx)
            {
                // Handle database update exceptions
                Console.WriteLine($"Database error occurred: {dbEx.Message}");
                return false;
            }
            catch (NullReferenceException nullEx)
            {
                // Handle null reference exceptions
                Console.WriteLine($"Null reference error: {nullEx.Message}");
                return false;
            }
            catch (Exception ex)
            {
                // Handle other exceptions
                Console.WriteLine($"An unexpected error occurred: {ex.Message}");
                return false;
            }
        }



        public bool EndBikeRental(int userId, string bikeId, DateTime rentalEndTime)
        {
            throw new NotImplementedException();
        }

        public List<TransportationTransaction> GetUserTransportationTransactions(int userId)
        {
            throw new NotImplementedException();
        }

        public bool RentBike(int userId, string bikeId, out DateTime rentalStartTime)
        {
            throw new NotImplementedException();
        }

        public bool IsUserDisabled(int userId)
        {
            var user = _userRepository.GetById(userId);

            if (user == null)
            {
                throw new Exception("User not found.");
            }

            return user.IsDisabled;


        }

        public bool IsUserStateFunded(int userId)
        {
            var user = _userRepository.GetById(userId);
            return user.IsStateFunded;
        }

        public void CreateTransportationTransaction(int userId, string shuttleId, string bikeId, DateTime date, DateTime rentalStartTime, DateTime rentalEndTime )
        {

            var transaction = new TransportationTransaction
            {
                UserId = userId,
                ShuttleId = shuttleId,
                BikeId = bikeId,
                Date = date,
                RentalStartTime = rentalStartTime,
                RentalEndTime = rentalEndTime // Example duration, you can customize this
            };

            // Save the transaction to the repository
            _transportationTransactionRepository.Insert(transaction);
        }

        public void CreatePaymentTransaction(int userId, decimal amount, PaymentMethod method, bool isRefund)
        {
            var transaction = new PaymentTransaction
            {
                UserId = userId,
                Amount = amount,
                Date = DateTime.Now,
                Method = method,
                IsRefund = isRefund
            };

            // Save the transaction to the repository
            _paymentRepository.Insert(transaction);
        }



    }
}
