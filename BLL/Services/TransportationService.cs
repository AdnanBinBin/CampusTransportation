using DAL.DB.Model;
using DAL.DB.Repositories.Interfaces;
using DAL.DB.Repositories;
using DAL.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BLL.Services
{
    public class TransportationService : ITransportationService
    {
        private readonly IExtendedShuttleRepository _shuttleRepository;
        private readonly IExtendedBikeRepository _bikeRepository;
        private readonly IExtendedTransportationTransactionRepository _transportationTransactionRepository;
        private readonly IExtendedPaymentTransactionRepository _paymentRepository;
        private readonly IExtendedSharedVehiculeRepository _sharedVehiculeRepository;
        private readonly IRepositoryInt<User> _userRepository;
        private readonly IRepositoryInt<Card> _cardRepository;

        public TransportationService(
        IExtendedShuttleRepository shuttleRepository,
        IExtendedBikeRepository bikeRepository,
        IExtendedTransportationTransactionRepository transportationTransactionRepository,
        IExtendedPaymentTransactionRepository paymentRepository,
        IExtendedSharedVehiculeRepository sharedVehiculeRepository,
        IRepositoryInt<User> userRepository,
        IRepositoryInt<Card> cardRepository)
        {
            _shuttleRepository = shuttleRepository ?? throw new ArgumentNullException(nameof(shuttleRepository));
            _bikeRepository = bikeRepository ?? throw new ArgumentNullException(nameof(bikeRepository));
            _transportationTransactionRepository = transportationTransactionRepository ?? throw new ArgumentNullException(nameof(transportationTransactionRepository));
            _paymentRepository = paymentRepository ?? throw new ArgumentNullException(nameof(paymentRepository));
            _sharedVehiculeRepository = sharedVehiculeRepository ?? throw new ArgumentNullException(nameof(sharedVehiculeRepository));
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _cardRepository = cardRepository ?? throw new ArgumentNullException(nameof(cardRepository));
        }



        
        public bool BoardShuttle(int userId, string shuttleId)
        {
            try
            {
                if (IsUserDisabled(userId))
                {
                    return false;  // L'utilisateur est désactivé ou introuvable
                }

                // Vérifier si l'utilisateur a déjà une location active
                var activeTransaction = _transportationTransactionRepository.GetActiveTransactionByUserId(userId);
                if (activeTransaction != null)
                {
                    throw new InvalidOperationException("L'utilisateur doit terminer sa location actuelle avant de monter dans une navette.");
                }

                var shuttle = _shuttleRepository.GetById(shuttleId);
                if (shuttle == null || !shuttle.IsAvailable)
                {
                    throw new InvalidOperationException("La navette n'est pas disponible.");
                }

                var user = _userRepository.GetById(userId);
                var card = _cardRepository.GetById(user.CardId);


                if (card == null)
                {
                    throw new InvalidOperationException("L'utilisateur n'a pas de carte associée.");
                }

                CreateTransportationTransaction(userId, shuttleId, null, null, DateTime.Now, DateTime.Now, DateTime.Now.AddHours(1));
                CreatePaymentTransaction(userId, shuttle.Price, card.PaymentMethod);

                return true;
            }
            catch (Exception ex)
            {
                // Gérer l'exception ou la relancer
                throw new Exception($"Erreur lors de l'embarquement dans la navette : {ex.Message}", ex);
            }
        }

        public bool EndBikeRental(int userId, string bikeId, DateTime rentalEndTime)
        {
            try
            {
                var user = _userRepository.GetById(userId);

                if (IsUserDisabled(userId))
                {
                    return false;  // Retourne false si l'utilisateur est désactivé ou introuvable
                }

                var bike = _bikeRepository.GetById(bikeId);
                if (bike == null)
                {
                    throw new InvalidOperationException("Le vélo n'existe pas.");
                }

                // Récupérer la transaction de location en cours pour ce vélo
                var activeTransaction = _transportationTransactionRepository.GetLatestTransactionForBike(bikeId);

                // Vérifier si une transaction active existe (location en cours) et si elle est liée à cet utilisateur
                if (activeTransaction == null || activeTransaction.RentalEndTime != null || activeTransaction.UserId != userId)
                {
                    throw new InvalidOperationException("Aucune location active trouvée pour cet utilisateur et ce vélo.");
                }

                // Calculer la durée de location
                var rentalDuration = rentalEndTime - activeTransaction.RentalStartTime.Value;
                if (rentalDuration.TotalHours <= 0)
                {
                    throw new InvalidOperationException("L'heure de fin de location doit être postérieure à l'heure de début.");
                }

                var rentalHours = Math.Ceiling(rentalDuration.TotalHours);

                // Mettre à jour la transaction avec l'heure de fin de location
                activeTransaction.RentalEndTime = rentalEndTime;
                _transportationTransactionRepository.Update(activeTransaction);

                // Calculer le nouveau montant basé sur le prix du vélo et la durée de location
                decimal newAmount = bike.Price * (decimal)rentalHours;

                // Récupérer la dernière transaction de paiement associée à cet utilisateur
                var paymentTransaction = _paymentRepository.GetLatestPaymentTransactionByUserId(userId);
                if (paymentTransaction != null)
                {
                    // Mettre à jour le montant de la transaction de paiement
                    paymentTransaction.Amount = newAmount;
                    _paymentRepository.Update(paymentTransaction);
                }

                // Marquer le vélo comme disponible à nouveau
                bike.IsAvailable = true;
                _bikeRepository.Update(bike);

                return true;  // Location terminée avec succès
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur lors de la fin de la location du vélo : {ex.Message}", ex);
            }
        }

        internal bool IsBikeRented(string bikeId)
        {
            var lastTransaction = _transportationTransactionRepository.GetLatestTransactionForBike(bikeId);

            // Si la dernière transaction n'a pas d'heure de fin, le vélo est encore en location
            return lastTransaction != null && lastTransaction.RentalEndTime == null;
        }

        public List<TransportationTransaction> GetUserTransportationTransactions(int userId)
        {
            try
            {
                if (IsUserDisabled(userId))
                {
                    throw new InvalidOperationException("L'utilisateur est désactivé ou introuvable.");
                }

                // Récupère toutes les transactions pour cet utilisateur
                var transactions = _transportationTransactionRepository
                    .GetAll()
                    .Where(t => t.UserId == userId)
                    .OrderByDescending(t => t.Date)
                    .ToList();

                return transactions;
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur lors de la récupération des transactions de transport : {ex.Message}", ex);
            }
        }

        public bool RentBike(int userId, string bikeId, out DateTime rentalStartTime)
        {
            rentalStartTime = DateTime.MinValue;

            try
            {
                var user = _userRepository.GetById(userId);

                if (IsUserDisabled(userId))
                {
                    return false;  // L'utilisateur est désactivé ou introuvable
                }

                // Vérifier si l'utilisateur a déjà une location active (vélo ou autre)
                var activeTransaction = _transportationTransactionRepository.GetActiveTransactionByUserId(userId);
                if (activeTransaction != null)
                {
                    throw new InvalidOperationException("L'utilisateur doit terminer sa location actuelle avant de commencer une nouvelle.");
                }

                var bike = _bikeRepository.GetById(bikeId);
                if (bike == null || !bike.IsAvailable || IsBikeRented(bikeId))
                {
                    throw new InvalidOperationException("Le vélo n'est pas disponible ou est déjà loué.");
                }

                var card = _cardRepository.GetById(user.CardId);

                if (card == null)
                {
                    throw new InvalidOperationException("L'utilisateur n'a pas de carte associée.");
                }

                rentalStartTime = DateTime.Now;

                // Créer et sauvegarder la transaction de transport
                CreateTransportationTransaction(userId, null, null, bikeId, DateTime.Now, rentalStartTime, null);
                // Créer une transaction de paiement
                CreatePaymentTransaction(userId, bike.Price, card.PaymentMethod);

                // Marquer le vélo comme non disponible
                bike.IsAvailable = false;
                _bikeRepository.Update(bike);

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur lors de la location du vélo : {ex.Message}", ex);
            }
        }

        public bool IsUserDisabled(int userId)
        {
            var user = _userRepository.GetById(userId);

            if (user == null)
            {
                throw new InvalidOperationException("Utilisateur introuvable.");
            }

            return user.IsDisabled;
        }

        public bool IsUserStateFunded(int userId)
        {
            var user = _userRepository.GetById(userId);
            return user.IsStateFunded;
        }

        public void CreateTransportationTransaction(int userId, string shuttleId, string sharedVehiculeId, string bikeId, DateTime date, DateTime rentalStartTime, DateTime? rentalEndTime)
        {
            var transaction = new TransportationTransaction
            {
                UserId = userId,
                ShuttleId = shuttleId,
                BikeId = bikeId,
                SharedVehiculeId = sharedVehiculeId,
                Date = date,
                RentalStartTime = rentalStartTime,
                RentalEndTime = rentalEndTime
            };

            // Enregistrer la transaction
            _transportationTransactionRepository.Insert(transaction);
        }

        public void CreatePaymentTransaction(int userId, decimal amount, PaymentMethod method)
        {
            bool isStateFunded = IsUserStateFunded(userId);

            var transaction = new PaymentTransaction
            {
                UserId = userId,
                Amount = amount,
                Date = DateTime.Now,
                Method = method,
                IsRefund = isStateFunded
            };

            // Enregistrer la transaction
            _paymentRepository.Insert(transaction);
        }

        public bool CreateSharedVehicleTrip(int driverId, string sharedVehicleId, DateTime rentalStartTime)
        {
            try
            {
                // Vérifier si le conducteur est désactivé
                if (IsUserDisabled(driverId))
                {
                    return false;
                }

                // Vérifier si le conducteur a déjà une location active
                var activeTransaction = _transportationTransactionRepository.GetActiveTransactionByUserId(driverId);
                if (activeTransaction != null)
                {
                    throw new InvalidOperationException("Le conducteur doit terminer sa course actuelle avant d'en commencer une nouvelle.");
                }

                // Récupérer le véhicule partagé
                var sharedVehicle = _sharedVehiculeRepository.GetById(sharedVehicleId);
                if (sharedVehicle == null)
                {
                    throw new InvalidOperationException("Le véhicule partagé n'existe pas.");
                }

                // Vérifier si le véhicule est disponible
                if (!sharedVehicle.IsAvailable)
                {
                    throw new InvalidOperationException("Le véhicule n'est pas disponible.");
                }

                // Assigner le DriverId et mettre à jour l'état du véhicule
                sharedVehicle.DriverId = driverId;
                sharedVehicle.IsAvailable = false;
                var user = _userRepository.GetById(driverId);
                sharedVehicle.Users.Add(user);

                // Créer une transaction de transport pour le conducteur
                CreateTransportationTransaction(driverId, null, sharedVehicleId, null, DateTime.Now, rentalStartTime, null);

                // Sauvegarder les modifications
                _sharedVehiculeRepository.Update(sharedVehicle);

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur lors de la création du trajet en véhicule partagé : {ex.Message}", ex);
            }
        }




        public bool RentSharedVehicle(int userId, string sharedVehicleId, out DateTime rentalStartTime, int driverId)
        {
            rentalStartTime = DateTime.Now;

            try
            {
                // Vérifier si l'utilisateur est désactivé
                if (IsUserDisabled(userId))
                {
                    return false;
                }

                // Vérifier si l'utilisateur a déjà une location active
                var activeTransaction = _transportationTransactionRepository.GetActiveTransactionByUserId(userId);
                if (activeTransaction != null)
                {
                    throw new InvalidOperationException("L'utilisateur doit terminer sa location actuelle avant de louer un véhicule partagé.");
                }

                // Récupérer le véhicule partagé
                var sharedVehicle = _sharedVehiculeRepository.GetById(sharedVehicleId);
                if (sharedVehicle == null)
                {
                    throw new InvalidOperationException("Le véhicule partagé n'existe pas.");
                }

                // Vérifier si le véhicule est disponible et si la capacité est suffisante
                if (sharedVehicle.Users.Count >= sharedVehicle.Capacity && sharedVehicle.IsAvailable)
                {
                    throw new InvalidOperationException("Le véhicule n'est pas disponible ou a atteint sa capacité maximale.");
                }

                // Ajouter l'utilisateur et le conducteur à la liste des participants
                var user = _userRepository.GetById(userId);
                var card = _cardRepository.GetById(user.CardId);
                if (user == null || card == null)
                {
                    throw new InvalidOperationException("L'utilisateur n'a pas de carte associée.");
                }

                // Assigner le DriverId au véhicule partagé
                sharedVehicle.DriverId = driverId;

                // Créer et sauvegarder la transaction de transport avec rentalEndTime nul
                CreateTransportationTransaction(userId, null, sharedVehicleId, null, DateTime.Now, DateTime.Now, null);

                // Créer une transaction de paiement
                CreatePaymentTransaction(userId, sharedVehicle.Price, card.PaymentMethod);

                // Ajouter l'utilisateur à la liste des participants et sauvegarder l'état du véhicule partagé
                sharedVehicle.Users.Add(user);
                _sharedVehiculeRepository.Update(sharedVehicle);

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur lors de la location du véhicule partagé : {ex.Message}", ex);
            }
        }

        public bool EndSharedVehicleRental(string sharedVehicleId, int driverId)
        {
            try
            {
                // Récupérer le véhicule partagé
                var sharedVehicle = _sharedVehiculeRepository.GetById(sharedVehicleId);
                if (sharedVehicle == null)
                {
                    throw new InvalidOperationException("Le véhicule partagé n'existe pas.");
                }

                // Vérifier si le driverId correspond au conducteur actuel du véhicule
                if (sharedVehicle.DriverId != driverId)
                {
                    throw new InvalidOperationException("Seul le conducteur désigné peut terminer le trajet.");
                }

                // Récupérer toutes les transactions de transport associées à ce véhicule partagé
                var transactions = _transportationTransactionRepository.GetAllTransactionsForSharedVehicule(sharedVehicleId);

                // Vérifier si des transactions existent et si elles sont toujours actives (sans date de fin)
                if (transactions == null || !transactions.Any(t => t.RentalEndTime == null))
                {
                    throw new InvalidOperationException("Aucune course active n'est en cours.");
                }

                // Terminer la course pour tous les passagers et le conducteur
                foreach (var transaction in transactions.Where(t => t.RentalEndTime == null))
                {
                    transaction.RentalEndTime = DateTime.Now;
                    _transportationTransactionRepository.Update(transaction);
                }

                // Le trajet est terminé, vider la liste des utilisateurs
                sharedVehicle.Users.Clear();
                sharedVehicle.DriverId = 0;  // Réinitialiser le conducteur à 0
                sharedVehicle.IsAvailable = true;  // Marquer le véhicule comme disponible

                // Sauvegarder les modifications dans le dépôt
                _sharedVehiculeRepository.Update(sharedVehicle);

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur lors de la fin du trajet en véhicule partagé : {ex.Message}", ex);
            }
        }

        public List<PaymentTransaction> GetUserPaymentTransactions(int userId)
        {
            try
            {
                if (IsUserDisabled(userId))
                {
                    throw new InvalidOperationException("L'utilisateur est désactivé ou introuvable.");
                }

                // Récupérer toutes les transactions de paiement et filtrer par userId
                return _paymentRepository
                    .GetAll()
                    .Where(t => t.UserId == userId)
                    .OrderByDescending(t => t.Date)
                    .ToList();
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur lors de la récupération des transactions de paiement : {ex.Message}", ex);
            }
        }
    }


}
