using Moq;
using Xunit;
using DAL.DB.Model;
using System;
using DAL.DB.Repositories.Interfaces;
using DAL.DB.Repositories;

namespace BLL.Services.Tests
{
    public class TransportationServiceTests
    {
        private readonly Mock<IExtendedShuttleRepository> _mockShuttleRepo;
        private readonly Mock<IExtendedBikeRepository> _mockBikeRepo;
        private readonly Mock<IExtendedTransportationTransactionRepository> _mockTransportRepo;
        private readonly Mock<IExtendedPaymentTransactionRepository> _mockPaymentRepo;
        private readonly Mock<IExtendedSharedVehiculeRepository> _mockSharedVehiculeRepo;
        private readonly Mock<IRepositoryInt<User>> _mockUserRepo;
        private readonly Mock<IRepositoryInt<Card>> _mockCardRepo;
        private readonly ITransportationService _service;

        public TransportationServiceTests()
        {
            _mockShuttleRepo = new Mock<IExtendedShuttleRepository>();
            _mockBikeRepo = new Mock<IExtendedBikeRepository>();
            _mockTransportRepo = new Mock<IExtendedTransportationTransactionRepository>();
            _mockPaymentRepo = new Mock<IExtendedPaymentTransactionRepository>();
            _mockSharedVehiculeRepo = new Mock<IExtendedSharedVehiculeRepository>();
            _mockUserRepo = new Mock<IRepositoryInt<User>>();
            _mockCardRepo = new Mock<IRepositoryInt<Card>>();

            _service = new TransportationService(
                _mockShuttleRepo.Object,
                _mockBikeRepo.Object,
                _mockTransportRepo.Object,
                _mockPaymentRepo.Object,
                _mockSharedVehiculeRepo.Object,
                _mockUserRepo.Object,
                _mockCardRepo.Object
            );
        }


        //Test for BoardShuttle
        [Fact]
        public void BoardShuttle_ValidUserAndAvailableShuttle_ReturnsTrue()
        {
            // Arrange
            int userId = 1;
            string shuttleId = "shuttle1";
            decimal shuttlePrice = 10.0m;

            var user = new User { Id = userId, CardId = 1, IsDisabled = false };
            var shuttle = new Shuttle { Id = shuttleId, IsAvailable = true, Price = shuttlePrice };
            var card = new Card { Id = 1, PaymentMethod = PaymentMethod.CreditCard };

            _mockUserRepo.Setup(r => r.GetById(userId)).Returns(user);
            _mockShuttleRepo.Setup(r => r.GetById(shuttleId)).Returns(shuttle);
            _mockCardRepo.Setup(r => r.GetById(1)).Returns(card);
            _mockTransportRepo.Setup(r => r.GetActiveTransactionByUserId(userId))
                             .Returns((TransportationTransaction)null);

            // Act
            var result = _service.BoardShuttle(userId, shuttleId);

            // Assert
            Assert.True(result);
            _mockTransportRepo.Verify(r => r.Insert(It.IsAny<TransportationTransaction>()), Times.Once);
            _mockPaymentRepo.Verify(r => r.Insert(It.IsAny<PaymentTransaction>()), Times.Once);
        }
        [Fact]
        public void BoardShuttle_DisabledUser_ReturnsFalse()
        {
            // Arrange
            int userId = 1;
            string shuttleId = "shuttle1";
            var user = new User { Id = userId, IsDisabled = true };

            _mockUserRepo.Setup(r => r.GetById(userId)).Returns(user);

            // Act
            var result = _service.BoardShuttle(userId, shuttleId);

            // Assert
            Assert.False(result);
            _mockTransportRepo.Verify(r => r.Insert(It.IsAny<TransportationTransaction>()), Times.Never);
            _mockPaymentRepo.Verify(r => r.Insert(It.IsAny<PaymentTransaction>()), Times.Never);
        }

        [Fact]
        public void BoardShuttle_UserHasActiveTransaction_ThrowsException()
        {
            // Arrange
            int userId = 1;
            string shuttleId = "shuttle1";
            var user = new User { Id = userId, IsDisabled = false };
            var activeTransaction = new TransportationTransaction { UserId = userId };

            _mockUserRepo.Setup(r => r.GetById(userId)).Returns(user);
            _mockTransportRepo.Setup(r => r.GetActiveTransactionByUserId(userId))
                             .Returns(activeTransaction);

            // Act & Assert
            var exception = Assert.Throws<Exception>(() => _service.BoardShuttle(userId, shuttleId));
            Assert.Equal("Erreur lors de l'embarquement dans la navette : L'utilisateur doit terminer sa location actuelle avant de monter dans une navette.", exception.Message);
            Assert.IsType<InvalidOperationException>(exception.InnerException);
        }

        [Fact]
        public void BoardShuttle_UnavailableShuttle_ThrowsException()
        {
            // Arrange
            int userId = 1;
            string shuttleId = "shuttle1";
            var user = new User { Id = userId, IsDisabled = false };
            var shuttle = new Shuttle { Id = shuttleId, IsAvailable = false };

            _mockUserRepo.Setup(r => r.GetById(userId)).Returns(user);
            _mockShuttleRepo.Setup(r => r.GetById(shuttleId)).Returns(shuttle);
            _mockTransportRepo.Setup(r => r.GetActiveTransactionByUserId(userId))
                             .Returns((TransportationTransaction)null);

            // Act & Assert
            var exception = Assert.Throws<Exception>(() => _service.BoardShuttle(userId, shuttleId));
            Assert.Equal("Erreur lors de l'embarquement dans la navette : La navette n'est pas disponible.", exception.Message);
            Assert.IsType<InvalidOperationException>(exception.InnerException);
        }

        [Fact]
        public void BoardShuttle_NoCard_ThrowsException()
        {
            // Arrange
            int userId = 1;
            string shuttleId = "shuttle1";
            var user = new User { Id = userId, CardId = 1, IsDisabled = false };
            var shuttle = new Shuttle { Id = shuttleId, IsAvailable = true };

            _mockUserRepo.Setup(r => r.GetById(userId)).Returns(user);
            _mockShuttleRepo.Setup(r => r.GetById(shuttleId)).Returns(shuttle);
            _mockCardRepo.Setup(r => r.GetById(1)).Returns((Card)null);
            _mockTransportRepo.Setup(r => r.GetActiveTransactionByUserId(userId))
                             .Returns((TransportationTransaction)null);

            // Act & Assert
            var exception = Assert.Throws<Exception>(() => _service.BoardShuttle(userId, shuttleId));
            Assert.Equal("Erreur lors de l'embarquement dans la navette : L'utilisateur n'a pas de carte associée.", exception.Message);
            Assert.IsType<InvalidOperationException>(exception.InnerException);
            Assert.Equal("L'utilisateur n'a pas de carte associée.", exception.InnerException.Message);
        }





        //Tests for method EndBikeRental
        [Fact]
        public void EndBikeRental_Success_ReturnsTrue()
        {
            // Arrange
            int userId = 1;
            string bikeId = "bike1";
            decimal bikePrice = 10.0m;
            DateTime startTime = DateTime.Now;
            DateTime endTime = startTime.AddHours(2);

            var user = new User { Id = userId, IsDisabled = false };
            var bike = new Bike { Id = bikeId, Price = bikePrice };
            var activeTransaction = new TransportationTransaction
            {
                UserId = userId,
                RentalStartTime = startTime,
                RentalEndTime = null
            };
            var paymentTransaction = new PaymentTransaction { UserId = userId };

            _mockUserRepo.Setup(r => r.GetById(userId)).Returns(user);
            _mockBikeRepo.Setup(r => r.GetById(bikeId)).Returns(bike);
            _mockTransportRepo.Setup(r => r.GetLatestTransactionForBike(bikeId)).Returns(activeTransaction);
            _mockPaymentRepo.Setup(r => r.GetLatestPaymentTransactionByUserId(userId)).Returns(paymentTransaction);

            // Act
            var result = _service.EndBikeRental(userId, bikeId, endTime);

            // Assert
            Assert.True(result);
            _mockTransportRepo.Verify(r => r.Update(It.Is<TransportationTransaction>(t => t.RentalEndTime == endTime)), Times.Once);
            _mockPaymentRepo.Verify(r => r.Update(It.Is<PaymentTransaction>(p => p.Amount == bikePrice * 2)), Times.Once);
            _mockBikeRepo.Verify(r => r.Update(It.Is<Bike>(b => b.IsAvailable)), Times.Once);
        }

        [Fact]
        public void EndBikeRental_DisabledUser_ReturnsFalse()
        {
            // Arrange
            int userId = 1;
            string bikeId = "bike1";
            var user = new User { Id = userId, IsDisabled = true };

            _mockUserRepo.Setup(r => r.GetById(userId)).Returns(user);

            // Act
            var result = _service.EndBikeRental(userId, bikeId, DateTime.Now);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void EndBikeRental_BikeNotFound_ThrowsException()
        {
            // Arrange
            int userId = 1;
            string bikeId = "bike1";
            var user = new User { Id = userId, IsDisabled = false };

            _mockUserRepo.Setup(r => r.GetById(userId)).Returns(user);
            _mockBikeRepo.Setup(r => r.GetById(bikeId)).Returns((Bike)null);

            // Act & Assert
            var exception = Assert.Throws<Exception>(() => _service.EndBikeRental(userId, bikeId, DateTime.Now));
            Assert.Equal("Erreur lors de la fin de la location du vélo : Le vélo n'existe pas.", exception.Message);
            Assert.IsType<InvalidOperationException>(exception.InnerException);
        }

        [Fact]
        public void EndBikeRental_NoActiveTransaction_ThrowsException()
        {
            // Arrange
            int userId = 1;
            string bikeId = "bike1";
            var user = new User { Id = userId, IsDisabled = false };
            var bike = new Bike { Id = bikeId };

            _mockUserRepo.Setup(r => r.GetById(userId)).Returns(user);
            _mockBikeRepo.Setup(r => r.GetById(bikeId)).Returns(bike);
            _mockTransportRepo.Setup(r => r.GetLatestTransactionForBike(bikeId))
                .Returns((TransportationTransaction)null);

            // Act & Assert
            var exception = Assert.Throws<Exception>(() => _service.EndBikeRental(userId, bikeId, DateTime.Now));
            Assert.Equal("Erreur lors de la fin de la location du vélo : Aucune location active trouvée pour cet utilisateur et ce vélo.", exception.Message);
        }

        [Fact]
        public void EndBikeRental_InvalidEndTime_ThrowsException()
        {
            // Arrange
            int userId = 1;
            string bikeId = "bike1";
            DateTime startTime = DateTime.Now;
            DateTime endTime = startTime.AddHours(-1);

            var user = new User { Id = userId, IsDisabled = false };
            var bike = new Bike { Id = bikeId };
            var activeTransaction = new TransportationTransaction
            {
                UserId = userId,
                RentalStartTime = startTime,
                RentalEndTime = null
            };

            _mockUserRepo.Setup(r => r.GetById(userId)).Returns(user);
            _mockBikeRepo.Setup(r => r.GetById(bikeId)).Returns(bike);
            _mockTransportRepo.Setup(r => r.GetLatestTransactionForBike(bikeId)).Returns(activeTransaction);

            // Act & Assert
            var exception = Assert.Throws<Exception>(() => _service.EndBikeRental(userId, bikeId, endTime));
            Assert.Equal("Erreur lors de la fin de la location du vélo : L'heure de fin de location doit être postérieure à l'heure de début.", exception.Message);
        }


        //Tests for method EndSharedVehiculeRental
        [Fact]
        public void IsBikeRented_BikeAlreadyRented_ThrowsException()
        {
            // Arrange
            int userId = 1;
            string bikeId = "bike1";
            DateTime startTime;
            var user = new User { Id = userId, IsDisabled = false, CardId = 1 };
            var bike = new Bike { Id = bikeId, IsAvailable = true };
            var activeTransaction = new TransportationTransaction
            {
                BikeId = bikeId,
                RentalEndTime = null
            };

            _mockUserRepo.Setup(r => r.GetById(userId)).Returns(user);
            _mockBikeRepo.Setup(r => r.GetById(bikeId)).Returns(bike);
            _mockTransportRepo.Setup(r => r.GetLatestTransactionForBike(bikeId))
                .Returns(activeTransaction);

            // Act & Assert
            var exception = Assert.Throws<Exception>(() => _service.RentBike(userId, bikeId, out startTime));
            Assert.Equal("Erreur lors de la location du vélo : Le vélo n'est pas disponible ou est déjà loué.", exception.Message);
        }

        //Tests for method RentBike
        [Fact]
        public void RentBike_ValidUserAndAvailableBike_ReturnsTrue()
        {
            // Arrange
            int userId = 1;
            string bikeId = "bike1";
            DateTime startTime;
            var user = new User { Id = userId, CardId = 1, IsDisabled = false };
            var bike = new Bike { Id = bikeId, IsAvailable = true };
            var card = new Card { Id = 1, PaymentMethod = PaymentMethod.CreditCard };

            _mockUserRepo.Setup(r => r.GetById(userId)).Returns(user);
            _mockBikeRepo.Setup(r => r.GetById(bikeId)).Returns(bike);
            _mockCardRepo.Setup(r => r.GetById(1)).Returns(card);
            _mockTransportRepo.Setup(r => r.GetActiveTransactionByUserId(userId))
                             .Returns((TransportationTransaction)null);
            _mockTransportRepo.Setup(r => r.GetLatestTransactionForBike(bikeId))
                             .Returns((TransportationTransaction)null);

            // Act
            var result = _service.RentBike(userId, bikeId, out startTime);

            // Assert
            Assert.True(result);
            Assert.NotEqual(DateTime.MinValue, startTime);
            _mockTransportRepo.Verify(r => r.Insert(It.IsAny<TransportationTransaction>()), Times.Once);
            _mockPaymentRepo.Verify(r => r.Insert(It.IsAny<PaymentTransaction>()), Times.Once);
            _mockBikeRepo.Verify(r => r.Update(It.Is<Bike>(b => !b.IsAvailable)), Times.Once);
        }

        [Fact]
        public void RentBike_UserWithActiveTransaction_ThrowsException()
        {
            // Arrange
            int userId = 1;
            string bikeId = "bike1";
            DateTime startTime;
            var user = new User { Id = userId, IsDisabled = false };
            var activeTransaction = new TransportationTransaction { UserId = userId };

            _mockUserRepo.Setup(r => r.GetById(userId)).Returns(user);
            _mockTransportRepo.Setup(r => r.GetActiveTransactionByUserId(userId))
                             .Returns(activeTransaction);

            // Act & Assert
            var exception = Assert.Throws<Exception>(() => _service.RentBike(userId, bikeId, out startTime));
            Assert.Equal("Erreur lors de la location du vélo : L'utilisateur doit terminer sa location actuelle avant de commencer une nouvelle.", exception.Message);
        }

        [Fact]
        public void RentBike_DisabledUser_ReturnsFalse()
        {
            // Arrange
            int userId = 1;
            string bikeId = "bike1";
            DateTime startTime;
            var user = new User { Id = userId, IsDisabled = true };

            _mockUserRepo.Setup(r => r.GetById(userId)).Returns(user);

            // Act
            var result = _service.RentBike(userId, bikeId, out startTime);

            // Assert
            Assert.False(result);
            Assert.Equal(DateTime.MinValue, startTime);
        }

        [Fact]
        public void RentBike_NoAssociatedCard_ThrowsException()
        {
            // Arrange
            int userId = 1;
            string bikeId = "bike1";
            DateTime startTime;
            var user = new User { Id = userId, CardId = 1, IsDisabled = false };
            var bike = new Bike { Id = bikeId, IsAvailable = true };

            _mockUserRepo.Setup(r => r.GetById(userId)).Returns(user);
            _mockBikeRepo.Setup(r => r.GetById(bikeId)).Returns(bike);
            _mockCardRepo.Setup(r => r.GetById(1)).Returns((Card)null);
            _mockTransportRepo.Setup(r => r.GetActiveTransactionByUserId(userId))
                             .Returns((TransportationTransaction)null);

            // Act & Assert
            var exception = Assert.Throws<Exception>(() => _service.RentBike(userId, bikeId, out startTime));
            Assert.Equal("Erreur lors de la location du vélo : L'utilisateur n'a pas de carte associée.", exception.Message);
        }

        [Fact]
        public void RentBike_BikeNotFound_ThrowsException()
        {
            // Arrange
            int userId = 1;
            string bikeId = "bike1";
            DateTime startTime;
            var user = new User { Id = userId, CardId = 1, IsDisabled = false };

            _mockUserRepo.Setup(r => r.GetById(userId)).Returns(user);
            _mockBikeRepo.Setup(r => r.GetById(bikeId)).Returns((Bike)null);
            _mockTransportRepo.Setup(r => r.GetActiveTransactionByUserId(userId))
                             .Returns((TransportationTransaction)null);

            // Act & Assert
            var exception = Assert.Throws<Exception>(() => _service.RentBike(userId, bikeId, out startTime));
            Assert.Equal("Erreur lors de la location du vélo : Le vélo n'est pas disponible ou est déjà loué.", exception.Message);
        }




        //Tests for method IsUserDisabled
        

        [Fact]
        public void BoardShuttle_UserNotFound_ThrowsException()
        {
            // Arrange
            int userId = 1;
            string shuttleId = "shuttle1";
            _mockUserRepo.Setup(r => r.GetById(userId)).Returns((User)null);

            // Act & Assert
            var exception = Assert.Throws<Exception>(() => _service.BoardShuttle(userId, shuttleId));
            Assert.Equal("Erreur lors de l'embarquement dans la navette : Utilisateur introuvable.", exception.Message);
        }

        //Tests for method CreateSharedVehiculeTrip
        [Fact]
        public void CreateSharedVehicleTrip_ValidDriverAndVehicle_ReturnsTrue()
        {
            // Arrange
            int driverId = 1;
            string vehicleId = "vehicle1";
            DateTime startTime = DateTime.Now;

            var driver = new User { Id = driverId, IsDisabled = false };
            var vehicle = new SharedVehicule { Id = vehicleId, IsAvailable = true };

            _mockUserRepo.Setup(r => r.GetById(driverId)).Returns(driver);
            _mockSharedVehiculeRepo.Setup(r => r.GetById(vehicleId)).Returns(vehicle);
            _mockTransportRepo.Setup(r => r.GetActiveTransactionByUserId(driverId))
                             .Returns((TransportationTransaction)null);

            // Act
            var result = _service.CreateSharedVehicleTrip(driverId, vehicleId, startTime);

            // Assert
            Assert.True(result);
            _mockTransportRepo.Verify(r => r.Insert(It.IsAny<TransportationTransaction>()), Times.Once);
            _mockSharedVehiculeRepo.Verify(r => r.Update(It.Is<SharedVehicule>(v =>
                v.DriverId == driverId &&
                !v.IsAvailable)),
                Times.Once);
        }

        [Fact]
        public void CreateSharedVehicleTrip_DisabledDriver_ReturnsFalse()
        {
            // Arrange
            int driverId = 1;
            string vehicleId = "vehicle1";
            var driver = new User { Id = driverId, IsDisabled = true };

            _mockUserRepo.Setup(r => r.GetById(driverId)).Returns(driver);

            // Act
            var result = _service.CreateSharedVehicleTrip(driverId, vehicleId, DateTime.Now);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void CreateSharedVehicleTrip_DriverHasActiveTransaction_ThrowsException()
        {
            // Arrange
            int driverId = 1;
            string vehicleId = "vehicle1";
            var driver = new User { Id = driverId, IsDisabled = false };
            var activeTransaction = new TransportationTransaction { UserId = driverId };

            _mockUserRepo.Setup(r => r.GetById(driverId)).Returns(driver);
            _mockTransportRepo.Setup(r => r.GetActiveTransactionByUserId(driverId))
                             .Returns(activeTransaction);

            // Act & Assert
            var exception = Assert.Throws<Exception>(() =>
                _service.CreateSharedVehicleTrip(driverId, vehicleId, DateTime.Now));
            Assert.Equal("Erreur lors de la création du trajet en véhicule partagé : Le conducteur doit terminer sa course actuelle avant d'en commencer une nouvelle.",
                exception.Message);
        }

        [Fact]
        public void CreateSharedVehicleTrip_VehicleNotFound_ThrowsException()
        {
            // Arrange
            int driverId = 1;
            string vehicleId = "vehicle1";
            var driver = new User { Id = driverId, IsDisabled = false };

            _mockUserRepo.Setup(r => r.GetById(driverId)).Returns(driver);
            _mockTransportRepo.Setup(r => r.GetActiveTransactionByUserId(driverId))
                             .Returns((TransportationTransaction)null);
            _mockSharedVehiculeRepo.Setup(r => r.GetById(vehicleId))
                                  .Returns((SharedVehicule)null);

            // Act & Assert
            var exception = Assert.Throws<Exception>(() =>
                _service.CreateSharedVehicleTrip(driverId, vehicleId, DateTime.Now));
            Assert.Equal("Erreur lors de la création du trajet en véhicule partagé : Le véhicule partagé n'existe pas.",
                exception.Message);
        }

        [Fact]
        public void CreateSharedVehicleTrip_UnavailableVehicle_ThrowsException()
        {
            // Arrange
            int driverId = 1;
            string vehicleId = "vehicle1";
            var driver = new User { Id = driverId, IsDisabled = false };
            var vehicle = new SharedVehicule { Id = vehicleId, IsAvailable = false };

            _mockUserRepo.Setup(r => r.GetById(driverId)).Returns(driver);
            _mockTransportRepo.Setup(r => r.GetActiveTransactionByUserId(driverId))
                             .Returns((TransportationTransaction)null);
            _mockSharedVehiculeRepo.Setup(r => r.GetById(vehicleId))
                                  .Returns(vehicle);

            // Act & Assert
            var exception = Assert.Throws<Exception>(() =>
                _service.CreateSharedVehicleTrip(driverId, vehicleId, DateTime.Now));
            Assert.Equal("Erreur lors de la création du trajet en véhicule partagé : Le véhicule n'est pas disponible.",
                exception.Message);
        }



        //Tests for method RentSharedVehicule
        [Fact]
        public void RentSharedVehicle_ValidUserAndVehicle_ReturnsTrue()
        {
            // Arrange
            int userId = 1, driverId = 2;
            string vehicleId = "vehicle1";
            var user = new User { Id = userId, CardId = 1, IsDisabled = false };
            var vehicle = new SharedVehicule { Id = vehicleId, IsAvailable = true, Capacity = 4, Price = 10 };
            var card = new Card { Id = 1, PaymentMethod = PaymentMethod.CreditCard };

            _mockUserRepo.Setup(r => r.GetById(userId)).Returns(user);
            _mockSharedVehiculeRepo.Setup(r => r.GetById(vehicleId)).Returns(vehicle);
            _mockCardRepo.Setup(r => r.GetById(1)).Returns(card);
            _mockTransportRepo.Setup(r => r.GetActiveTransactionByUserId(userId))
                             .Returns((TransportationTransaction)null);

            // Act
            var result = _service.RentSharedVehicle(userId, vehicleId, out DateTime startTime, driverId);

            // Assert
            Assert.True(result);
            Assert.NotEqual(DateTime.MinValue, startTime);
            _mockTransportRepo.Verify(r => r.Insert(It.IsAny<TransportationTransaction>()), Times.Once);
            _mockPaymentRepo.Verify(r => r.Insert(It.IsAny<PaymentTransaction>()), Times.Once);
            _mockSharedVehiculeRepo.Verify(r => r.Update(It.Is<SharedVehicule>(v =>
                v.DriverId == driverId &&
                v.Users.Contains(user))), Times.Once);
        }

        [Fact]
        public void RentSharedVehicle_DisabledUser_ReturnsFalse()
        {
            // Arrange
            int userId = 1, driverId = 2;
            string vehicleId = "vehicle1";
            var user = new User { Id = userId, IsDisabled = true };

            _mockUserRepo.Setup(r => r.GetById(userId)).Returns(user);

            // Act
            var result = _service.RentSharedVehicle(userId, vehicleId, out DateTime startTime, driverId);

            // Assert
            Assert.False(result);
            Assert.Equal(DateTime.Now.Date, startTime.Date);
        }

        [Fact]
        public void RentSharedVehicle_ActiveTransaction_ThrowsException()
        {
            // Arrange
            int userId = 1, driverId = 2;
            string vehicleId = "vehicle1";
            var user = new User { Id = userId, IsDisabled = false };
            var activeTransaction = new TransportationTransaction { UserId = userId };

            _mockUserRepo.Setup(r => r.GetById(userId)).Returns(user);
            _mockTransportRepo.Setup(r => r.GetActiveTransactionByUserId(userId))
                             .Returns(activeTransaction);

            // Act & Assert
            var exception = Assert.Throws<Exception>(() =>
                _service.RentSharedVehicle(userId, vehicleId, out DateTime startTime, driverId));
            Assert.Equal("Erreur lors de la location du véhicule partagé : L'utilisateur doit terminer sa location actuelle avant de louer un véhicule partagé.",
                exception.Message);
        }

        [Fact]
        public void RentSharedVehicle_VehicleNotFound_ThrowsException()
        {
            // Arrange
            int userId = 1, driverId = 2;
            string vehicleId = "vehicle1";
            var user = new User { Id = userId, IsDisabled = false };

            _mockUserRepo.Setup(r => r.GetById(userId)).Returns(user);
            _mockTransportRepo.Setup(r => r.GetActiveTransactionByUserId(userId))
                             .Returns((TransportationTransaction)null);
            _mockSharedVehiculeRepo.Setup(r => r.GetById(vehicleId))
                                  .Returns((SharedVehicule)null);

            // Act & Assert
            var exception = Assert.Throws<Exception>(() =>
                _service.RentSharedVehicle(userId, vehicleId, out DateTime startTime, driverId));
            Assert.Equal("Erreur lors de la location du véhicule partagé : Le véhicule partagé n'existe pas.",
                exception.Message);
        }

        [Fact]
        public void RentSharedVehicle_VehicleAtCapacity_ThrowsException()
        {
            // Arrange
            int userId = 1, driverId = 2;
            string vehicleId = "vehicle1";
            var user = new User { Id = userId, IsDisabled = false };
            var vehicle = new SharedVehicule
            {
                Id = vehicleId,
                IsAvailable = true,
                Capacity = 2,
                Users = new List<User> { new User(), new User() }
            };

            _mockUserRepo.Setup(r => r.GetById(userId)).Returns(user);
            _mockTransportRepo.Setup(r => r.GetActiveTransactionByUserId(userId))
                             .Returns((TransportationTransaction)null);
            _mockSharedVehiculeRepo.Setup(r => r.GetById(vehicleId))
                                  .Returns(vehicle);

            // Act & Assert
            var exception = Assert.Throws<Exception>(() =>
                _service.RentSharedVehicle(userId, vehicleId, out DateTime startTime, driverId));
            Assert.Equal("Erreur lors de la location du véhicule partagé : Le véhicule n'est pas disponible ou a atteint sa capacité maximale.",
                exception.Message);
        }

        [Fact]
        public void RentSharedVehicle_NoCard_ThrowsException()
        {
            // Arrange
            int userId = 1, driverId = 2;
            string vehicleId = "vehicle1";
            var user = new User { Id = userId, CardId = 1, IsDisabled = false };
            var vehicle = new SharedVehicule { Id = vehicleId, IsAvailable = true, Capacity = 4 };

            _mockUserRepo.Setup(r => r.GetById(userId)).Returns(user);
            _mockTransportRepo.Setup(r => r.GetActiveTransactionByUserId(userId))
                             .Returns((TransportationTransaction)null);
            _mockSharedVehiculeRepo.Setup(r => r.GetById(vehicleId))
                                  .Returns(vehicle);
            _mockCardRepo.Setup(r => r.GetById(1))
                        .Returns((Card)null);

            // Act & Assert
            var exception = Assert.Throws<Exception>(() =>
                _service.RentSharedVehicle(userId, vehicleId, out DateTime startTime, driverId));
            Assert.Equal("Erreur lors de la location du véhicule partagé : L'utilisateur n'a pas de carte associée.",
                exception.Message);
        }


        //Tests for method EndSharedVehiculeRental
        [Fact]
        public void EndSharedVehicleRental_ValidRequest_ReturnsTrue()
        {
            // Arrange
            string vehicleId = "vehicle1";
            int driverId = 1;
            var vehicle = new SharedVehicule { Id = vehicleId, DriverId = driverId };
            var transactions = new List<TransportationTransaction> {
        new TransportationTransaction { SharedVehiculeId = vehicleId, RentalEndTime = null }
    };

            _mockSharedVehiculeRepo.Setup(r => r.GetById(vehicleId)).Returns(vehicle);
            _mockTransportRepo.Setup(r => r.GetAllTransactionsForSharedVehicule(vehicleId))
                             .Returns(transactions);

            // Act
            var result = _service.EndSharedVehicleRental(vehicleId, driverId);

            // Assert
            Assert.True(result);
            _mockTransportRepo.Verify(r => r.Update(It.Is<TransportationTransaction>(t =>
                t.RentalEndTime != null)), Times.Once);
            _mockSharedVehiculeRepo.Verify(r => r.Update(It.Is<SharedVehicule>(v =>
                v.IsAvailable && v.DriverId == 0)), Times.Once);
        }

        [Fact]
        public void EndSharedVehicleRental_VehicleNotFound_ThrowsException()
        {
            // Arrange
            string vehicleId = "vehicle1";
            int driverId = 1;
            _mockSharedVehiculeRepo.Setup(r => r.GetById(vehicleId))
                                  .Returns((SharedVehicule)null);

            // Act & Assert
            var exception = Assert.Throws<Exception>(() =>
                _service.EndSharedVehicleRental(vehicleId, driverId));
            Assert.Equal("Erreur lors de la fin du trajet en véhicule partagé : Le véhicule partagé n'existe pas.",
                exception.Message);
        }

        [Fact]
        public void EndSharedVehicleRental_WrongDriver_ThrowsException()
        {
            // Arrange
            string vehicleId = "vehicle1";
            int driverId = 1;
            var vehicle = new SharedVehicule { Id = vehicleId, DriverId = 2 };
            _mockSharedVehiculeRepo.Setup(r => r.GetById(vehicleId)).Returns(vehicle);

            // Act & Assert
            var exception = Assert.Throws<Exception>(() =>
                _service.EndSharedVehicleRental(vehicleId, driverId));
            Assert.Equal("Erreur lors de la fin du trajet en véhicule partagé : Seul le conducteur désigné peut terminer le trajet.",
                exception.Message);
        }

        [Fact]
        public void EndSharedVehicleRental_NoActiveTransactions_ThrowsException()
        {
            // Arrange
            string vehicleId = "vehicle1";
            int driverId = 1;
            var vehicle = new SharedVehicule { Id = vehicleId, DriverId = driverId };
            var transactions = new List<TransportationTransaction> {
        new TransportationTransaction { SharedVehiculeId = vehicleId, RentalEndTime = DateTime.Now }
    };

            _mockSharedVehiculeRepo.Setup(r => r.GetById(vehicleId)).Returns(vehicle);
            _mockTransportRepo.Setup(r => r.GetAllTransactionsForSharedVehicule(vehicleId))
                             .Returns(transactions);

            // Act & Assert
            var exception = Assert.Throws<Exception>(() =>
                _service.EndSharedVehicleRental(vehicleId, driverId));
            Assert.Equal("Erreur lors de la fin du trajet en véhicule partagé : Aucune course active n'est en cours.",
                exception.Message);
        }

        [Fact]
         public void GetUserTransportationTransactions_UserExistsAndHasTransactions_ReturnsAllUserTransactions()
        {
            // Arrange
            int userId = 1;
            var user = new User { Id = userId, IsDisabled = false };
            var transactions = new List<TransportationTransaction>
    {
        new TransportationTransaction { UserId = userId, Date = DateTime.Now },
        new TransportationTransaction { UserId = userId, Date = DateTime.Now.AddDays(-1) }
    };

            _mockUserRepo.Setup(r => r.GetById(userId)).Returns(user);
            _mockTransportRepo.Setup(r => r.GetAll()).Returns(transactions);

            // Act
            var result = _service.GetUserTransportationTransactions(userId);

            // Assert
            Assert.Equal(2, result.Count);
            Assert.All(result, t => Assert.Equal(userId, t.UserId));
        }

        [Fact]
        public void GetUserTransportationTransactions_DisabledUser_ThrowsException()
        {
            // Arrange
            int userId = 1;
            var user = new User { Id = userId, IsDisabled = true };
            _mockUserRepo.Setup(r => r.GetById(userId)).Returns(user);

            // Act & Assert
            var exception = Assert.Throws<Exception>(() =>
                _service.GetUserTransportationTransactions(userId));
            Assert.Equal("Erreur lors de la récupération des transactions de transport : L'utilisateur est désactivé ou introuvable.",
                exception.Message);
        }

        [Fact]
        public void GetUserPaymentTransactions_UserExistsAndHasTransactions_ReturnsAllUserTransactions()

        {
            // Arrange
            int userId = 1;
            var user = new User { Id = userId, IsDisabled = false };
            var transactions = new List<PaymentTransaction>
    {
        new PaymentTransaction { UserId = userId, Date = DateTime.Now },
        new PaymentTransaction { UserId = userId, Date = DateTime.Now.AddDays(-1) }
    };

            _mockUserRepo.Setup(r => r.GetById(userId)).Returns(user);
            _mockPaymentRepo.Setup(r => r.GetAll()).Returns(transactions);

            // Act
            var result = _service.GetUserPaymentTransactions(userId);

            // Assert
            Assert.Equal(2, result.Count);
            Assert.All(result, t => Assert.Equal(userId, t.UserId));
        }

        [Fact]
        public void GetUserPaymentTransactions_DisabledUser_ThrowsException()
        {
            // Arrange
            int userId = 1;
            var user = new User { Id = userId, IsDisabled = true };
            _mockUserRepo.Setup(r => r.GetById(userId)).Returns(user);

            // Act & Assert
            var exception = Assert.Throws<Exception>(() =>
                _service.GetUserPaymentTransactions(userId));
            Assert.Equal("Erreur lors de la récupération des transactions de paiement : L'utilisateur est désactivé ou introuvable.",
                exception.Message);
        }

        [Fact]
        public void GetUserPaymentTransactions_UserNotFound_ThrowsException()
        {
            // Arrange
            int userId = 1;
            _mockUserRepo.Setup(r => r.GetById(userId)).Returns((User)null);

            // Act & Assert
            var exception = Assert.Throws<Exception>(() =>
                _service.GetUserPaymentTransactions(userId));
            Assert.Equal("Erreur lors de la récupération des transactions de paiement : Utilisateur introuvable.",
                exception.Message);
        }

        [Fact]
        public void GetUserPaymentTransactions_NoTransactions_ReturnsEmptyList()
        {
            // Arrange
            int userId = 1;
            var user = new User { Id = userId, IsDisabled = false };
            var transactions = new List<PaymentTransaction>();

            _mockUserRepo.Setup(r => r.GetById(userId)).Returns(user);
            _mockPaymentRepo.Setup(r => r.GetAll()).Returns(transactions);

            // Act
            var result = _service.GetUserPaymentTransactions(userId);

            // Assert
            Assert.Empty(result);
        }
    }



}