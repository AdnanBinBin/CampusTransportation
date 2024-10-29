using System;
using System.Threading.Tasks;
using ConsoleApp1.Controllers;
using ConsoleApp1.DTOs.Responses;

namespace ConsoleApp1.Commands
{
    // Commande de création de trajet en véhicule partagé
    public class CreateSharedVehicleTripCommand : ICommand
    {
        private readonly SharedVehicleApiClient _sharedVehicleClient;
        private readonly int _userId;

        public string Name => "Créer un trajet en véhicule partagé";
        public string Description => "Permet de créer un nouveau trajet en tant que conducteur";

        public CreateSharedVehicleTripCommand(SharedVehicleApiClient sharedVehicleClient, int userId)
        {
            _sharedVehicleClient = sharedVehicleClient;
            _userId = userId;
        }

        public async Task ExecuteAsync()
        {
            Console.WriteLine($"\n=== {Name} (mode conducteur) ===");
            Console.Write("ID véhicule : ");
            string vehicleId = Console.ReadLine();

            var result = await _sharedVehicleClient.CreateTripAsync(_userId, vehicleId);
            Console.WriteLine($"Statut: {result.Message}");
        }
    }
}
