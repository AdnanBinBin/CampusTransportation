using Xunit;
using Moq;
using Microsoft.EntityFrameworkCore;
using DAL.DB;
using DAL.DB.Model;
using DAL.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Diagnostics.CodeAnalysis;

namespace CampusTransportationService.UnitTests.TestDAL
{
    public class TransportationTransactionRepositoryTests
    {
        private readonly Mock<DbSet<TransportationTransaction>> _mockSet;
        private readonly Context _context;
        private readonly TransportationTransactionRepository _repository;
        private readonly List<TransportationTransaction> _data;

        public TransportationTransactionRepositoryTests()
        {
            // Préparer les données de test
            var now = DateTime.Now;
            _data = new List<TransportationTransaction>
            {
                new TransportationTransaction
                {
                    Id = 1,
                    UserId = 1,
                    BikeId = "BIKE1",
                    Date = now.AddDays(-1),
                    RentalStartTime = now.AddDays(-1),
                    RentalEndTime = null
                },
                new TransportationTransaction
                {
                    Id = 2,
                    UserId = 1,
                    BikeId = "BIKE2",
                    Date = now,
                    RentalStartTime = now,
                    RentalEndTime = now.AddHours(2)
                },
                new TransportationTransaction
                {
                    Id = 3,
                    UserId = 2,
                    SharedVehiculeId = "VEHICLE1",
                    Date = now,
                    RentalStartTime = now,
                    RentalEndTime = null
                },
                new TransportationTransaction
                {
                    Id = 4,
                    UserId = 3,
                    SharedVehiculeId = "VEHICLE1",
                    Date = now.AddDays(-1),
                    RentalStartTime = now.AddDays(-1),
                    RentalEndTime = null
                }
            };

            var queryableData = _data.AsQueryable();

            _mockSet = new Mock<DbSet<TransportationTransaction>>();

            // Setup pour IQueryable
            _mockSet.As<IQueryable<TransportationTransaction>>()
                   .Setup(m => m.Provider)
                   .Returns(new TestAsyncQueryProvider<TransportationTransaction>(queryableData.Provider));

            _mockSet.As<IQueryable<TransportationTransaction>>()
                   .Setup(m => m.Expression)
                   .Returns(queryableData.Expression);

            _mockSet.As<IQueryable<TransportationTransaction>>()
                   .Setup(m => m.ElementType)
                   .Returns(queryableData.ElementType);

            _mockSet.As<IQueryable<TransportationTransaction>>()
                   .Setup(m => m.GetEnumerator())
                   .Returns(() => queryableData.GetEnumerator());

            var options = new DbContextOptionsBuilder<Context>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new Context(options);

            var property = _context.GetType().GetProperty("TransportationTransactions");
            property.SetValue(_context, _mockSet.Object);

            _repository = new TransportationTransactionRepository(_context);
        }


        [ExcludeFromCodeCoverage]
        // Classes helper pour les tests asynchrones
        internal class TestAsyncQueryProvider<TEntity> : IQueryProvider
        {
            private readonly IQueryProvider _inner;

            internal TestAsyncQueryProvider(IQueryProvider inner)
            {
                _inner = inner;
            }

            public IQueryable CreateQuery(Expression expression)
            {
                return new TestAsyncEnumerable<TEntity>(expression);
            }

            public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
            {
                return new TestAsyncEnumerable<TElement>(expression);
            }

            public object Execute(Expression expression)
            {
                return _inner.Execute(expression);
            }

            public TResult Execute<TResult>(Expression expression)
            {
                return _inner.Execute<TResult>(expression);
            }
        }


        [ExcludeFromCodeCoverage]
        internal class TestAsyncEnumerable<T> : EnumerableQuery<T>, IAsyncEnumerable<T>, IQueryable<T>
        {
            public TestAsyncEnumerable(IEnumerable<T> enumerable)
                : base(enumerable)
            { }

            public TestAsyncEnumerable(Expression expression)
                : base(expression)
            { }

            IQueryProvider IQueryable.Provider => new TestAsyncQueryProvider<T>(this);

            public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken token = default)
            {
                return new TestAsyncEnumerator<T>(this.AsEnumerable().GetEnumerator());
            }
        }

        [ExcludeFromCodeCoverage]
        internal class TestAsyncEnumerator<T> : IAsyncEnumerator<T>
        {
            private readonly IEnumerator<T> _inner;

            public TestAsyncEnumerator(IEnumerator<T> inner)
            {
                _inner = inner;
            }

            public ValueTask<bool> MoveNextAsync()
            {
                return new ValueTask<bool>(_inner.MoveNext());
            }

            public T Current => _inner.Current;

            public ValueTask DisposeAsync()
            {
                _inner.Dispose();
                return new ValueTask();
            }
        }

        [Fact]
        public void GetActiveTransactionByUserId_UserWithActiveRental_ReturnsTransaction()
        {
            // Act
            var result = _repository.GetActiveTransactionByUserId(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("BIKE1", result.BikeId);
            Assert.Null(result.RentalEndTime);
        }

        [Fact]
        public void GetLatestTransactionForBike_BikeWithTransactions_ReturnsMostRecent()
        {
            // Act
            var result = _repository.GetLatestTransactionForBike("BIKE2");

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Id);
        }

        [Fact]
        public void GetAllTransactionsForSharedVehicule_VehiculeHasTransactions_ReturnsAllTransactions()
        {
            // Act
            var result = _repository.GetAllTransactionsForSharedVehicule("VEHICLE1").ToList();

            // Assert
            Assert.Equal(2, result.Count);
            Assert.All(result, t => Assert.Equal("VEHICLE1", t.SharedVehiculeId));
        }

        [Fact]
        public void Insert_ValidTransaction_AddedToDatabase()
        {
            // Arrange
            var transaction = new TransportationTransaction
            {
                UserId = 4,
                BikeId = "BIKE3",
                Date = DateTime.Now,
                RentalStartTime = DateTime.Now
            };

            // Act
            _repository.Insert(transaction);

            // Assert
            _mockSet.Verify(m => m.Add(It.Is<TransportationTransaction>(t =>
                t.UserId == transaction.UserId &&
                t.BikeId == transaction.BikeId)),
                Times.Once());
        }

        [Fact]
        public void Update_ExistingTransaction_UpdatesDatabase()
        {
            // Arrange
            var transaction = _data[0];
            transaction.RentalEndTime = DateTime.Now;

            _mockSet.Setup(m => m.Find(It.IsAny<object[]>()))
                   .Returns(transaction);

            // Act
            _repository.Update(transaction);

            // Assert
            _mockSet.Verify(m => m.Find(It.Is<object[]>(o =>
                Convert.ToInt32(o[0]) == transaction.Id)), Times.Once());
        }

        [Fact]
        public void GetById_ExistingTransactionId_ReturnsTransaction()
        {
            // Arrange
            var expectedTransaction = _data[0];
            _mockSet.Setup(m => m.Find(It.IsAny<object[]>()))
                   .Returns(expectedTransaction);

            // Act
            var result = _repository.GetById(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedTransaction.Id, result.Id);
            Assert.Equal(expectedTransaction.UserId, result.UserId);
        }

        [Fact]
        public void GetAll_DataExists_ReturnsAllTransactions()
        {
            // Act
            var result = _repository.GetAll().ToList();

            // Assert
            Assert.Equal(_data.Count, result.Count);
        }

        [Fact]
        public void GetLatestTransactionForShuttle_ShuttleWithTransactions_ReturnsMostRecent()
        {
            // Arrange
            var now = DateTime.Now;
            var data = _data.ToList(); // Clone existing data
            data.Add(new TransportationTransaction
            {
                Id = 5,
                ShuttleId = "SHUTTLE1",
                Date = now.AddDays(-1),
                UserId = 1
            });
            data.Add(new TransportationTransaction
            {
                Id = 6,
                ShuttleId = "SHUTTLE1",
                Date = now, // More recent
                UserId = 2
            });

            var queryableData = data.AsQueryable();
            _mockSet.As<IQueryable<TransportationTransaction>>()
                   .Setup(m => m.Provider)
                   .Returns(new TestAsyncQueryProvider<TransportationTransaction>(queryableData.Provider));
            _mockSet.As<IQueryable<TransportationTransaction>>()
                   .Setup(m => m.Expression)
                   .Returns(queryableData.Expression);
            _mockSet.As<IQueryable<TransportationTransaction>>()
                   .Setup(m => m.ElementType)
                   .Returns(queryableData.ElementType);
            _mockSet.As<IQueryable<TransportationTransaction>>()
                   .Setup(m => m.GetEnumerator())
                   .Returns(() => queryableData.GetEnumerator());

            // Act
            var result = _repository.GetLatestTransactionForShuttle("SHUTTLE1");

            // Assert
            Assert.NotNull(result);
            Assert.Equal(6, result.Id);
            Assert.Equal("SHUTTLE1", result.ShuttleId);
        }

        [Fact]
        public void GetLatestTransactionForShuttle_NoTransactions_ReturnsNull()
        {
            // Act
            var result = _repository.GetLatestTransactionForShuttle("NONEXISTENT_SHUTTLE");

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void GetLatestTransactionForSharedVehicule_VehiculeWithTransactions_ReturnsMostRecent()
        {
            // The data is already setup in the constructor with VEHICLE1 transactions
            // Act
            var result = _repository.GetLatestTransactionForSharedVehicule("VEHICLE1");

            // Assert
            Assert.NotNull(result);
            Assert.Equal(3, result.Id); // ID 3 has the most recent date
            Assert.Equal("VEHICLE1", result.SharedVehiculeId);
        }

        [Fact]
        public void GetLatestTransactionForSharedVehicule_NoTransactions_ReturnsNull()
        {
            // Act
            var result = _repository.GetLatestTransactionForSharedVehicule("NONEXISTENT_VEHICLE");

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void Delete_ExistingTransaction_RemovesFromDatabase()
        {
            // Arrange
            var transaction = new TransportationTransaction
            {
                Id = 1,
                UserId = 1,
                BikeId = "BIKE1",
                Date = DateTime.Now
            };

            // Act
            _repository.Delete(transaction);

            // Assert
            _mockSet.Verify(m => m.Remove(It.Is<TransportationTransaction>(t =>
                t.Id == transaction.Id &&
                t.UserId == transaction.UserId &&
                t.BikeId == transaction.BikeId)), Times.Once());
        }
    }
}