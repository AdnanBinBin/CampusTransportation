using Microsoft.EntityFrameworkCore;
using DAL.DB.Model;

namespace DAL.DB
{
    public class Context : DbContext
    {
        // DbSets
        public DbSet<TransportationTransaction> TransportationTransactions { get; set; }
        public DbSet<PaymentTransaction> PaymentTransactions { get; set; }
        public DbSet<Bike> Bikes { get; set; }
        public DbSet<SharedVehicule> SharedVehicules { get; set; }
        public DbSet<Shuttle> Shuttles { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Card> Cards { get; set; }

        // Données de test pour les cartes
        private static Card[] _cards = new[]
        {
            new Card {
                Id = 1,
                PaymentMethod = PaymentMethod.CreditCard
            },
            new Card {
                Id = 2,
                PaymentMethod = PaymentMethod.BankTransfer
            },
            new Card {
                Id = 3,
                PaymentMethod = PaymentMethod.CreditCard
            },
            new Card {
                Id = 4,
                PaymentMethod = PaymentMethod.BankTransfer
            }
        };

        // Données de test pour les utilisateurs
        private static User[] _users = new[]
        {
            new User {
                Id = 1,
                Name = "Jean Martin",
                IsDisabled = false,
                IsStateFunded = true,
                CardId = 1
            },
            new User {
                Id = 2,
                Name = "Marie Dubois",
                IsDisabled = false,
                IsStateFunded = false,
                CardId = 2
            },
            new User {
                Id = 3,
                Name = "Paul Bernard",
                IsDisabled = true,
                IsStateFunded = true,
                CardId = 3
            },
            new User {
                Id = 4,
                Name = "Sophie Lambert",
                IsDisabled = false,
                IsStateFunded = true,
                CardId = 4
            }
        };

        // Données de test pour les vélos
        private static Bike[] _bikes = new[]
        {
            new Bike {
                Id = "BIKE001",
                Name = "Vélo Classique 1",
                IsAvailable = true,
                Price = 2.50m
            },
            new Bike {
                Id = "BIKE002",
                Name = "Vélo Électrique 1",
                IsAvailable = true,
                Price = 4.00m
            }
        };

        // Données de test pour les véhicules partagés
        private static SharedVehicule[] _sharedVehicules = new[]
        {
            new SharedVehicule {
                Id = "SHARE001",
                Name = "Covoiturage Campus 1",
                IsAvailable = true,
                Price = 3.00m,
                Capacity = 4,
                DriverId = 1
            },
            new SharedVehicule {
                Id = "SHARE002",
                Name = "Covoiturage Campus 2",
                IsAvailable = true,
                Price = 3.00m,
                Capacity = 6,
                DriverId = 2
            }
        };

        // Données de test pour les navettes
        private static Shuttle[] _shuttles = new[]
        {
            new Shuttle {
                Id = "SHUT001",
                Name = "Navette Campus-Centre",
                IsAvailable = true,
                Price = 1.50m
            },
            new Shuttle {
                Id = "SHUT002",
                Name = "Navette Campus-Gare",
                IsAvailable = true,
                Price = 2.00m
            }
        };

        public Context(DbContextOptions options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=DbCampusV7");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuration des clés étrangères et relations
            modelBuilder.Entity<User>()
                .Property(u => u.CardId)
                .IsRequired();

            modelBuilder.Entity<User>()
                .HasIndex(u => u.CardId)
                .IsUnique();

            // Configuration des transactions
            modelBuilder.Entity<TransportationTransaction>()
                .Property(t => t.ShuttleId)
                .HasMaxLength(50)
                .IsRequired(false);

            modelBuilder.Entity<TransportationTransaction>()
                .Property(t => t.BikeId)
                .HasMaxLength(50)
                .IsRequired(false);

            modelBuilder.Entity<TransportationTransaction>()
                .Property(t => t.SharedVehiculeId)
                .HasMaxLength(50)
                .IsRequired(false);

            modelBuilder.Entity<PaymentTransaction>()
                .Property(p => p.Amount)
                .IsRequired();

            // Configuration des entités Bike
            modelBuilder.Entity<Bike>()
                .Property(b => b.Id)
                .HasMaxLength(50);

            modelBuilder.Entity<Bike>()
                .Property(b => b.Name)
                .HasMaxLength(100)
                .IsRequired();

            modelBuilder.Entity<Bike>()
                .Property(b => b.Price)
                .IsRequired();

            // Configuration des entités SharedVehicule
            modelBuilder.Entity<SharedVehicule>()
                .Property(v => v.Id)
                .HasMaxLength(50);

            modelBuilder.Entity<SharedVehicule>()
                .Property(v => v.Name)
                .HasMaxLength(100)
                .IsRequired();

            modelBuilder.Entity<SharedVehicule>()
                .Property(v => v.Price)
                .IsRequired();

            modelBuilder.Entity<SharedVehicule>()
                .Property(v => v.Capacity)
                .IsRequired();

            // Configuration des entités Shuttle
            modelBuilder.Entity<Shuttle>()
                .Property(s => s.Id)
                .HasMaxLength(50);

            modelBuilder.Entity<Shuttle>()
                .Property(s => s.Name)
                .HasMaxLength(100)
                .IsRequired();

            modelBuilder.Entity<Shuttle>()
                .Property(s => s.Price)
                .IsRequired();

            // Configuration des entités User
            modelBuilder.Entity<User>()
                .Property(u => u.Name)
                .HasMaxLength(100)
                .IsRequired();

            modelBuilder.Entity<User>()
                .Property(u => u.IsDisabled)
                .IsRequired();

            modelBuilder.Entity<User>()
                .Property(u => u.IsStateFunded)
                .IsRequired();

            // Seeding des données
            modelBuilder.Entity<Card>().HasData(_cards);
            modelBuilder.Entity<User>().HasData(_users);
            modelBuilder.Entity<Bike>().HasData(_bikes);
            modelBuilder.Entity<SharedVehicule>().HasData(_sharedVehicules);
            modelBuilder.Entity<Shuttle>().HasData(_shuttles);
        }
    }
}