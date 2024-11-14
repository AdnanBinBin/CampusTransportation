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

namespace CampusTransportationService.UnitTests.TestDAL
{
    public class PaymentTransactionRepositoryTests
    {
        private readonly Mock<DbSet<PaymentTransaction>> _mockSet;
        private readonly Context _context;
        private readonly PaymentTransactionRepository _repository;
        private readonly List<PaymentTransaction> _data;

        public PaymentTransactionRepositoryTests()
        {
            // Préparer les données de test
            var now = DateTime.Now;
            _data = new List<PaymentTransaction>
            {
                new PaymentTransaction
                {
                    Id = 1,
                    UserId = 1,
                    Amount = 100,
                    Date = now,
                    Method = PaymentMethod.CreditCard,
                    IsRefund = false
                },
                new PaymentTransaction
                {
                    Id = 2,
                    UserId = 1,
                    Amount = 50,
                    Date = now.AddDays(-1),
                    Method = PaymentMethod.DebitCard,
                    IsRefund = false
                }
            };

            var queryableData = _data.AsQueryable();

            _mockSet = new Mock<DbSet<PaymentTransaction>>();

            // Setup pour IQueryable
            _mockSet.As<IQueryable<PaymentTransaction>>()
                   .Setup(m => m.Provider)
                   .Returns(new TestAsyncQueryProvider<PaymentTransaction>(queryableData.Provider));

            _mockSet.As<IQueryable<PaymentTransaction>>()
                   .Setup(m => m.Expression)
                   .Returns(queryableData.Expression);

            _mockSet.As<IQueryable<PaymentTransaction>>()
                   .Setup(m => m.ElementType)
                   .Returns(queryableData.ElementType);

            _mockSet.As<IQueryable<PaymentTransaction>>()
                   .Setup(m => m.GetEnumerator())
                   .Returns(() => queryableData.GetEnumerator());

            var options = new DbContextOptionsBuilder<Context>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new Context(options);

            var property = _context.GetType().GetProperty("PaymentTransactions");
            property.SetValue(_context, _mockSet.Object);

            _repository = new PaymentTransactionRepository(_context);
        }

        #region Helper Classes
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
        #endregion

        [Fact]
        public void GetLatestPaymentTransactionByUserId_UserWithTransactions_ReturnsMostRecent()
        {
            // Act
            var result = _repository.GetLatestPaymentTransactionByUserId(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
            Assert.Equal(100, result.Amount);
        }

        [Fact]
        public void Insert_ValidTransaction_AddedToDatabase()
        {
            // Arrange
            var transaction = new PaymentTransaction
            {
                Id = 3,
                UserId = 2,
                Amount = 75,
                Date = DateTime.Now,
                Method = PaymentMethod.CreditCard,
                IsRefund = false
            };

            // Act
            _repository.Insert(transaction);

            // Assert
            _mockSet.Verify(m => m.Add(It.Is<PaymentTransaction>(t =>
                t.Id == transaction.Id &&
                t.Amount == transaction.Amount)),
                Times.Once());
        }

        [Fact]
        public void Update_ExistingTransaction_UpdatesDatabase()
        {
            // Arrange
            var transaction = _data[0];
            transaction.Amount = 150;

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
            Assert.Equal(expectedTransaction.Amount, result.Amount);
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
        public void Delete_ExistingTransaction_RemovedFromDatabase()
        {
            // Arrange
            var transaction = _data[0];

            // Act
            _repository.Delete(transaction);

            // Assert
            _mockSet.Verify(m => m.Remove(It.Is<PaymentTransaction>(t =>
                t.Id == transaction.Id)), Times.Once());
        }
    }
}