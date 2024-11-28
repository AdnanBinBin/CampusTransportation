using System;
using System.Threading.Tasks;
using ConsoleApp1.Controllers;
using ConsoleApp1.DTOs.Responses;
using System.Linq;

namespace ConsoleApp1.Commands.User
{
    public class TransactionCommand : ICommand
    {
        private readonly UserApiClient _userClient;
        private readonly int _userId;

        public string Name => "Voir mes transactions";
        public string Description => "Affiche l'historique des transactions de transport et de paiement";

        public TransactionCommand(UserApiClient userClient, int userId)
        {
            _userClient = userClient;
            _userId = userId;
        }

        public async Task ExecuteAsync()
        {
            Console.WriteLine($"\n=== {Name} ===");

            try
            {
                var transportResponse = await _userClient.GetUserTransportationsAsync(_userId);
                var paymentResponse = await _userClient.GetUserPaymentsAsync(_userId);

                if (transportResponse?.Transactions == null || paymentResponse?.Transactions == null)
                {
                    Console.WriteLine("Aucune transaction trouvée.");
                    return;
                }

                // Combiner et trier les transactions par date
                var combinedTransactions = transportResponse.Transactions
                    .Select(t => new
                    {
                        Date = t.Date,
                        TransportInfo = t,
                        PaymentInfo = paymentResponse.Transactions
                            .FirstOrDefault(p => p.Date.Date == t.Date.Date && p.UserId == t.UserId)
                    })
                    .OrderByDescending(x => x.Date);

                Console.WriteLine("\nHistorique des transactions :");
                Console.WriteLine(new string('-', 80));

                foreach (var transaction in combinedTransactions)
                {
                    Console.WriteLine($"Date: {transaction.Date:dd/MM/yyyy HH:mm}");

                    // Afficher les informations de transport
                    string transportType = DetermineTransportType(transaction.TransportInfo);
                    Console.WriteLine($"Type de transport: {transportType}");

                    if (transaction.TransportInfo.RentalStartTime.HasValue)
                    {
                        Console.WriteLine($"Début: {transaction.TransportInfo.RentalStartTime.Value:HH:mm}");
                        if (transaction.TransportInfo.RentalEndTime.HasValue)
                        {
                            Console.WriteLine($"Fin: {transaction.TransportInfo.RentalEndTime.Value:HH:mm}");
                            TimeSpan duration = transaction.TransportInfo.RentalEndTime.Value - transaction.TransportInfo.RentalStartTime.Value;
                            Console.WriteLine($"Durée: {FormatDuration(duration)}");
                        }
                        else
                        {
                            Console.WriteLine($"En cours");
                        }
                    }

                    // Afficher les informations de paiement
                    if (transaction.PaymentInfo != null)
                    {
                        Console.WriteLine($"Montant: {transaction.PaymentInfo.Amount:C2}");
                        Console.WriteLine($"Mode de paiement: {transaction.PaymentInfo.PaymentMethod}");
                        if (transaction.PaymentInfo.IsRefund)
                        {
                            Console.WriteLine("(Remboursement)");
                        }
                    }

                    Console.WriteLine(new string('-', 80));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors de la récupération des transactions : {ex.Message}");
            }
        }

        private string DetermineTransportType(TransportationTransactionDto transaction)
        {
            if (!string.IsNullOrEmpty(transaction.BikeId))
                return $"Vélo (ID: {transaction.BikeId})";
            if (!string.IsNullOrEmpty(transaction.ShuttleId))
                return $"Navette (ID: {transaction.ShuttleId})";
            if (!string.IsNullOrEmpty(transaction.SharedVehiculeId))
                return $"Véhicule partagé (ID: {transaction.SharedVehiculeId})";
            return "Type inconnu";
        }

        private string FormatDuration(TimeSpan duration)
        {
            if (duration.TotalHours >= 1)
                return $"{Math.Floor(duration.TotalHours)}h {duration.Minutes}min";
            return $"{duration.Minutes}min";
        }
    }
}