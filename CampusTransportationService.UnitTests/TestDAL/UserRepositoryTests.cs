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
    public class UserRepositoryTests
    {
        private readonly Mock<DbSet<User>> _mockSet;
        private readonly Context _context;
        private readonly UserRepository _repository;
        private readonly List<User> _data;

        public UserRepositoryTests()
        {
            // Préparer les données de test
            _data = new List<User>
            {
                new User
                {
                    Id = 1,
                    CardId = 1,
                    IsDisabled = false,
                    IsStateFunded = false
                },
                new User
                {
                    Id = 2,
                    CardId = 2,
                    IsDisabled = true,
                    IsStateFunded = true
                }
            };

            var queryableData = _data.AsQueryable();

            _mockSet = new Mock<DbSet<User>>();

            // Setup pour IQueryable
            _mockSet.As<IQueryable<User>>()
                   .Setup(m => m.Provider)
                   .Returns(new TestAsyncQueryProvider<User>(queryableData.Provider));

            _mockSet.As<IQueryable<User>>()
                   .Setup(m => m.Expression)
                   .Returns(queryableData.Expression);

            _mockSet.As<IQueryable<User>>()
                   .Setup(m => m.ElementType)
                   .Returns(queryableData.ElementType);

            _mockSet.As<IQueryable<User>>()
                   .Setup(m => m.GetEnumerator())
                   .Returns(() => queryableData.GetEnumerator());

            var options = new DbContextOptionsBuilder<Context>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new Context(options);

            var property = _context.GetType().GetProperty("Users");
            property.SetValue(_context, _mockSet.Object);

            _repository = new UserRepository(_context);
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
        public void GetById_ExistingUserId_ReturnsUser()
        {
            // Arrange
            var expectedUser = _data[0];
            _mockSet.Setup(m => m.Find(It.IsAny<object[]>()))
                   .Returns(expectedUser);

            // Act
            var result = _repository.GetById(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedUser.Id, result.Id);
            Assert.Equal(expectedUser.CardId, result.CardId);
            Assert.Equal(expectedUser.IsDisabled, result.IsDisabled);
        }

        [Fact]
        public void GetById_NonExistingUserId_ReturnsNull()
        {
            // Arrange
            _mockSet.Setup(m => m.Find(It.IsAny<object[]>()))
                   .Returns((User)null);

            // Act
            var result = _repository.GetById(999);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void Insert_ValidUser_AddedToDatabase()
        {
            // Arrange
            var user = new User
            {
                Id = 3,
                CardId = 3,
                IsDisabled = false,
                IsStateFunded = false
            };

            // Act
            _repository.Insert(user);

            // Assert
            _mockSet.Verify(m => m.Add(It.Is<User>(u =>
                u.Id == user.Id &&
                u.CardId == user.CardId &&
                u.IsDisabled == user.IsDisabled)),
                Times.Once());
        }

        [Fact]
        public void Update_ExistingUser_UpdatesDatabase()
        {
            // Arrange
            var user = new User
            {
                Id = 1,
                CardId = 1,
                IsDisabled = true,
                IsStateFunded = true
            };

            // Act
            _repository.Update(user);

            // Assert
            _mockSet.Verify(m => m.Update(It.Is<User>(u =>
                u.Id == user.Id &&
                u.IsDisabled == user.IsDisabled &&
                u.IsStateFunded == user.IsStateFunded)),
                Times.Once());
        }

        [Fact]
        public void Delete_ExistingUser_RemovedFromDatabase()
        {
            // Arrange
            var user = _data[0];

            // Act
            _repository.Delete(user);

            // Assert
            _mockSet.Verify(m => m.Remove(It.Is<User>(u =>
                u.Id == user.Id)), Times.Once());
        }

        [Fact]
        public void GetAll_DataExists_ReturnsAllUsers()
        {
            // Act
            var result = _repository.GetAll().ToList();

            // Assert
            Assert.Equal(_data.Count, result.Count);
            Assert.Contains(result, u => u.Id == 1);
            Assert.Contains(result, u => u.Id == 2);
        }

        [Fact]
        public void Insert_ValidUser_SavesChanges()
        {
            // Arrange
            var user = new User { Id = 3 };

            // Act
            _repository.Insert(user);

            // Assert
            _mockSet.Verify(m => m.Add(It.IsAny<User>()), Times.Once());
        }

        [Fact]
        public void Update_ExistingUser_SavesChanges()
        {
            // Arrange
            var user = new User { Id = 1 };

            // Act
            _repository.Update(user);

            // Assert
            _mockSet.Verify(m => m.Update(It.IsAny<User>()), Times.Once());
        }

        [Fact]
        public void Delete_ExistingUser_SavesChanges()
        {
            // Arrange
            var user = _data[0];

            // Act
            _repository.Delete(user);

            // Assert
            _mockSet.Verify(m => m.Remove(It.IsAny<User>()), Times.Once());
        }
    }
}