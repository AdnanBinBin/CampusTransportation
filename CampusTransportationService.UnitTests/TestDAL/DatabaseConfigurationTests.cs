using Xunit;
using Microsoft.EntityFrameworkCore;
using DAL.DB;
using System.Linq;

namespace DAL.Tests.DB
{
    public class DatabaseConfigurationTests
    {
        [Fact]
        public void OnConfiguring_WhenNotConfigured_UseSqlServerWithCorrectConnectionString()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<Context>()
                .Options;

            // Act
            using (var context = new Context(options))
            {
                // Assert
                Assert.True(context.Database.IsSqlServer());
                Assert.Equal(@"Server=(localdb)\mssqllocaldb;Database=DbCampusV7",
                    context.Database.GetConnectionString());
            }
        }

        [Fact]
        public void OnConfiguring_WhenAlreadyConfigured_DoesNotModifyConfiguration()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<Context>()
                .UseInMemoryDatabase("TestDatabase")
                .Options;

            // Act
            using (var context = new Context(options))
            {
                // Assert
                Assert.True(context.Database.IsInMemory());
                Assert.False(context.Database.IsSqlServer());
            }
        }
    }
}