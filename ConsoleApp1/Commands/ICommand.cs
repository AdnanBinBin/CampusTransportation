using System.Threading.Tasks;

namespace ConsoleApp1.Commands
{
    // Interface de base pour les commandes
    public interface ICommand
    {
        string Name { get; }
        string Description { get; }
        Task ExecuteAsync();
    }
}
