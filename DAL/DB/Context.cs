using Microsoft.EntityFrameworkCore;
using DAL.DB.Model;

namespace DAL.DB
{
    public class Context : DbContext
    {
        // Définition des DbSet pour les tables
        public DbSet<TransportationTransaction> TransportationTransactions { get; set; }
        public DbSet<PaymentTransaction> PaymentTransactions { get; set; }
        public DbSet<Bike> Bikes { get; set; } // Ajout du DbSet pour les vélos
        public DbSet<SharedVehicule> SharedVehicules { get; set; } // Ajout du DbSet pour les véhicules partagés
        public DbSet<Shuttle> Shuttles { get; set; } // Ajout du DbSet pour les navettes
        public DbSet<User> Users { get; set; } // Ajout du DbSet pour les utilisateurs
        public DbSet<Card> Cards { get; set; } // Ajout du DbSet pour les cartes

        // Exemple de données statiques (si nécessaire)
        private static TransportationTransaction[] _transportationTransactions = new[]
        {
            new TransportationTransaction { Id = 1, UserId = 1, ShuttleId = "Shuttle1", BikeId = "Bike1", Date = DateTime.Now, RentalStartTime = DateTime.Now.AddHours(-1), RentalEndTime = DateTime.Now }
        };

        private static PaymentTransaction[] _paymentTransactions = new[]
        {
            new PaymentTransaction { Id = 1, UserId = 1, Amount = 100.00m, Date = DateTime.Now, Method = PaymentMethod.CreditCard, IsRefund = false }
        };

        private static User[] _users = new[]
        {
            new User { Id = 1, Name = "John Doe", IsDisabled = false, IsStateFunded = true } // Update to include IsStateFunded
        };

        private static Card[] _cards = new[]
        {
            new Card { Id = 1, PaymentMethod = PaymentMethod.BankTransfer, UserId = 1 }
        };

        // Constructeur
        public Context(DbContextOptions options) : base(options)
        {
        }

        // Configuration des options (comme la chaîne de connexion) si nécessaire
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=DbCampus");
        }

        // Configuration des modèles
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Données initiales pour TransportationTransaction (optionnel)
            modelBuilder.Entity<TransportationTransaction>().HasData(_transportationTransactions);

            // Données initiales pour PaymentTransaction (optionnel)
            modelBuilder.Entity<PaymentTransaction>().HasData(_paymentTransactions);

            // Données initiales pour User (optionnel)
            modelBuilder.Entity<User>().HasData(_users);

            // Données initiales pour Card (optionnel)
            modelBuilder.Entity<Card>().HasData(_cards);

            // Configuration des propriétés spécifiques pour PaymentTransaction
            modelBuilder.Entity<PaymentTransaction>()
                .Property(p => p.Amount);

            // Configuration des propriétés spécifiques pour TransportationTransaction (facultatif)
            modelBuilder.Entity<TransportationTransaction>()
                .Property(t => t.ShuttleId)
                .HasMaxLength(50);  // Limitation de longueur pour ShuttleId

            modelBuilder.Entity<TransportationTransaction>()
                .Property(t => t.BikeId)
                .HasMaxLength(50);  // Limitation de longueur pour BikeId

            // Configuration des propriétés pour Bike
            modelBuilder.Entity<Bike>()
                .Property(b => b.Name)
                .HasMaxLength(100); // Limite de longueur pour le nom du vélo

            // Configuration des propriétés pour SharedVehicule
            modelBuilder.Entity<SharedVehicule>()
                .Property(v => v.Name)
                .HasMaxLength(100); // Limite de longueur pour le nom du véhicule partagé

            // Configuration des propriétés pour Shuttle
            modelBuilder.Entity<Shuttle>()
                .Property(s => s.Name)
                .HasMaxLength(100); // Limite de longueur pour le nom de la navette

            // Configuration des propriétés pour User
            modelBuilder.Entity<User>()
                .Property(u => u.Name)
                .HasMaxLength(100); // Limite de longueur pour le nom de l'utilisateur

            modelBuilder.Entity<User>()
                .Property(u => u.IsDisabled)
                .IsRequired(); // IsDisabled est requis

            modelBuilder.Entity<User>()
                .Property(u => u.IsStateFunded)
                .IsRequired(); // IsStateFunded est requis

            // Configuration des propriétés pour Card
            modelBuilder.Entity<Card>()
                .Property(c => c.UserId)
                .IsRequired(); // UserId est requis
        }
    }
}
