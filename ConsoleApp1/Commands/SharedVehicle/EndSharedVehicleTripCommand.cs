using System;
using System.Threading.Tasks;
using ConsoleApp1.Controllers;
using ConsoleApp1.DTOs.Responses;

namespace ConsoleApp1.Commands
{
    // Commande pour terminer un trajet en véhicule partagé
    public class EndSharedVehicleTripCommand : ICommand
    {
        private readonly SharedVehicleApiClient _sharedVehicleClient;
        private readonly int _userId;

        public string Name => "Terminer un trajet en véhicule partagé";
        public string Description => "Permet de terminer un trajet en véhicule partagé";

        public EndSharedVehicleTripCommand(SharedVehicleApiClient sharedVehicleClient, int userId)
        {
            _sharedVehicleClient = sharedVehicleClient;
            _userId = userId;
        }

        public async Task ExecuteAsync()
        {
            Console.WriteLine($"\n=== {Name} ===");
            Console.Write("ID véhicule : ");
            string vehicleId = Console.ReadLine();

            var result = await _sharedVehicleClient.EndRentalAsync(vehicleId, _userId);
            Console.WriteLine($"Statut: {result.Message}");
        }
    }
}
