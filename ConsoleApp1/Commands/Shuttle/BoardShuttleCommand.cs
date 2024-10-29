using System;
using System.Threading.Tasks;
using ConsoleApp1.Controllers;
using ConsoleApp1.DTOs.Responses;

namespace ConsoleApp1.Commands
{
    // Commande d'embarquement dans une navette
    public class BoardShuttleCommand : ICommand
    {
        private readonly ShuttleApiClient _shuttleClient;
        private readonly int _userId;

        public string Name => "Monter dans une navette";
        public string Description => "Permet de monter dans une navette disponible";

        public BoardShuttleCommand(ShuttleApiClient shuttleClient, int userId)
        {
            _shuttleClient = shuttleClient;
            _userId = userId;
        }

        public async Task ExecuteAsync()
        {
            Console.WriteLine($"\n=== {Name} ===");
            Console.Write("ID navette : ");
            string shuttleId = Console.ReadLine();

            var result = await _shuttleClient.BoardShuttleAsync(_userId, shuttleId);
            Console.WriteLine($"Statut: {result.Message}");
        }
    }
}
