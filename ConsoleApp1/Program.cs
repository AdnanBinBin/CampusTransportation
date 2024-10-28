using DAL.DB;
using DAL.Repositories;
using Microsoft.EntityFrameworkCore;

namespace ConsoleApp 
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var options = new DbContextOptionsBuilder<Context>()
                .UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=DbCampusV7")
                .Options;

            using (var context = new Context(options))
            {
                context.Database.EnsureCreated();

                TestRepositories(context);
            }

            Console.WriteLine("La base de données a été créée et les tests des repositories ont été effectués.");
        }

        private static void TestRepositories(Context context)
        {
            var paymentRepo = new PaymentTransactionRepository(context);
            var transportationRepo = new TransportationTransactionRepository(context);

            // Récupérer toutes les transactions de paiement
            var allPayments = paymentRepo.GetAll();
            Console.WriteLine("Transactions de paiement :");
            foreach (var payment in allPayments)
            {
                Console.WriteLine($"Id: {payment.Id}, Montant: {payment.Amount}, Date: {payment.Date}, Méthode: {payment.Method}");
            }

            // Récupérer toutes les transactions de transport
            var allTransportations = transportationRepo.GetAll();
            Console.WriteLine("Transactions de transport :");
            foreach (var transportation in allTransportations)
            {
                Console.WriteLine($"Id: {transportation.Id}, ShuttleId: {transportation.ShuttleId}, BikeId: {transportation.BikeId}, Date: {transportation.Date}");
            }
        }
    }
}
