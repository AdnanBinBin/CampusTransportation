using System;
using System.Threading.Tasks;
using ConsoleApp1.Controllers;
using ConsoleApp1.DTOs.Responses;

namespace ConsoleApp1.Commands
{
    // Commande de fin de location de vélo
    public class EndBikeRentalCommand : ICommand
    {
        private readonly BikeApiClient _bikeClient;
        private readonly int _userId;

        public string Name => "Terminer la location d'un vélo";
        public string Description => "Permet de terminer la location d'un vélo";

        public EndBikeRentalCommand(BikeApiClient bikeClient, int userId)
        {
            _bikeClient = bikeClient;
            _userId = userId;
        }

        public async Task ExecuteAsync()
        {
            Console.WriteLine($"\n=== {Name} ===");
            Console.Write("ID vélo : ");
            string bikeId = Console.ReadLine();

            var result = await _bikeClient.EndBikeRentalAsync(_userId, bikeId);
            Console.WriteLine($"Statut: {result.Message}");
        }
    }
}
