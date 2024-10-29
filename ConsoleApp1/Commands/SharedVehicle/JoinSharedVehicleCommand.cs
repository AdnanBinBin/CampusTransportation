using System;
using System.Threading.Tasks;
using ConsoleApp1.Controllers;
using ConsoleApp1.DTOs.Responses;

namespace ConsoleApp1.Commands
{
    // Commande pour rejoindre un véhicule partagé
    public class JoinSharedVehicleCommand : ICommand
    {
        private readonly SharedVehicleApiClient _sharedVehicleClient;
        private readonly int _userId;

        public string Name => "Rejoindre un véhicule partagé";
        public string Description => "Permet de rejoindre un trajet en tant que passager";

        public JoinSharedVehicleCommand(SharedVehicleApiClient sharedVehicleClient, int userId)
        {
            _sharedVehicleClient = sharedVehicleClient;
            _userId = userId;
        }

        public async Task ExecuteAsync()
        {
            Console.WriteLine($"\n=== {Name} (mode passager) ===");
            Console.Write("ID véhicule : ");
            string vehicleId = Console.ReadLine();
            Console.Write("ID conducteur : ");
            int driverId = int.Parse(Console.ReadLine());

            var result = await _sharedVehicleClient.RentSharedVehicleAsync(_userId, vehicleId, driverId);
            Console.WriteLine($"Statut: {result.Message}");
        }
    }
}
