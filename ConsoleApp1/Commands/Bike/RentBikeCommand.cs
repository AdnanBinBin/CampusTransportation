using System;
using System.Threading.Tasks;
using ConsoleApp1.Controllers;
using ConsoleApp1.DTOs.Responses;

namespace ConsoleApp1.Commands.Bike
{
    // Commande de location de vélo
    public class RentBikeCommand : ICommand
    {
        private readonly BikeApiClient _bikeClient;
        private readonly int _userId;

        public string Name => "Louer un vélo";
        public string Description => "Permet de louer un vélo disponible";

        public RentBikeCommand(BikeApiClient bikeClient, int userId)
        {
            _bikeClient = bikeClient;
            _userId = userId;
        }

        public async Task ExecuteAsync()
        {
            Console.WriteLine($"\n=== {Name} ===");
            Console.Write("ID vélo : ");
            string bikeId = Console.ReadLine();

            var result = await _bikeClient.RentBikeAsync(_userId, bikeId);
            Console.WriteLine($"Statut: {result.Message}");
        }
    }
}
