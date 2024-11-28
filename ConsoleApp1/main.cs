using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ConsoleApp1.Controllers;
using ConsoleApp1.Commands;
using ConsoleApp1.Commands.Bike;
using ConsoleApp1.Commands.User;

namespace ConsoleApp1
{
    class Program
    {
        private static readonly string BaseApiUrl = "https://localhost:7087";
        private static readonly BikeApiClient _bikeClient;
        private static readonly ShuttleApiClient _shuttleClient;
        private static readonly SharedVehicleApiClient _sharedVehicleClient;
        private static readonly UserApiClient _userClient;
        private static int _currentUserId;
        private static List<ICommand> _commands;

        static Program()
        {
            _bikeClient = new BikeApiClient(BaseApiUrl);
            _shuttleClient = new ShuttleApiClient(BaseApiUrl);
            _sharedVehicleClient = new SharedVehicleApiClient(BaseApiUrl);
            _userClient = new UserApiClient(BaseApiUrl);
        }

        static async Task Main(string[] args)
        {
            if (!await InitializeUserSession())
            {
                return;
            }

            while (true)
            {
                InitializeCommands();
                DisplayMenu();

                string choice = Console.ReadLine();

                try
                {
                    if (choice == "0")
                    {
                        Console.WriteLine("Au revoir !");
                        return;
                    }

                    if (choice == "8")
                    {
                        if (!await InitializeUserSession())
                        {
                            return;
                        }
                        continue;
                    }

                    if (int.TryParse(choice, out int commandIndex) && commandIndex > 0 && commandIndex <= _commands.Count)
                    {
                        await _commands[commandIndex - 1].ExecuteAsync();
                    }
                    else
                    {
                        Console.WriteLine("Option invalide");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"{ex.Message}");
                }

                Console.WriteLine("\nAppuyez sur une touche pour continuer...");
                Console.ReadKey();
            }
        }

        private static void InitializeCommands()
        {
            _commands = new List<ICommand>
            {
                new RentBikeCommand(_bikeClient, _currentUserId),
                new EndBikeRentalCommand(_bikeClient, _currentUserId),
                new BoardShuttleCommand(_shuttleClient, _currentUserId),
                new CreateSharedVehicleTripCommand(_sharedVehicleClient, _currentUserId),
                new JoinSharedVehicleCommand(_sharedVehicleClient, _currentUserId),
                new EndSharedVehicleTripCommand(_sharedVehicleClient, _currentUserId),
                new TransactionCommand(_userClient, _currentUserId),
            };
        }

        private static void DisplayMenu()
        {
            Console.Clear();
            Console.WriteLine($"=== Application de Transport - Utilisateur ID: {_currentUserId} ===");

            for (int i = 0; i < _commands.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {_commands[i].Name}");
            }

            Console.WriteLine("8. Changer d'utilisateur");
            Console.WriteLine("0. Quitter");
            Console.WriteLine("\nChoisissez une option :");
        }

        private static async Task<bool> InitializeUserSession()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("=== Identification Utilisateur ===");
                Console.WriteLine("(Entrez 0 pour quitter)");
                Console.Write("Entrez votre ID utilisateur : ");

                if (!int.TryParse(Console.ReadLine(), out int userId))
                {
                    Console.WriteLine("ID invalide. Veuillez entrer un nombre.");
                    Console.WriteLine("Appuyez sur une touche pour réessayer...");
                    Console.ReadKey();
                    continue;
                }

                if (userId == 0)
                {
                    Console.WriteLine("Au revoir !");
                    return false;
                }

                try
                {
                    _currentUserId = userId;
                    return true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"\nErreur : {ex.Message}");
                    Console.WriteLine("Appuyez sur une touche pour réessayer...");
                    Console.ReadKey();
                }
            }
        }
    }
}