using Xunit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using BLL.Services;
using DAL.DB;
using DAL.Repositories;
using DAL.DB.Repositories;
using DAL.DB.Repositories.Interfaces;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using DAL.DB.Model;

namespace Web_Api.Tests
{
    public class ProgramTests
    {
        [Fact]
        public void Program_ConfigureServices_RegistersAllDependencies()
        {
            // Arrange
            var builder = WebApplication.CreateBuilder();

            // Act
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

            builder.Services.AddDbContext<Context>(options =>
                options.UseInMemoryDatabase("TestDb"));

            builder.Services.AddScoped<IExtendedShuttleRepository, ShuttleRepository>();
            builder.Services.AddScoped<ShuttleRepository>();
            builder.Services.AddScoped<IExtendedBikeRepository, BikeRepository>();
            builder.Services.AddScoped<BikeRepository>();
            builder.Services.AddScoped<IExtendedTransportationTransactionRepository, TransportationTransactionRepository>();
            builder.Services.AddScoped<TransportationTransactionRepository>();
            builder.Services.AddScoped<IExtendedPaymentTransactionRepository, PaymentTransactionRepository>();
            builder.Services.AddScoped<PaymentTransactionRepository>();
            builder.Services.AddScoped<IExtendedSharedVehiculeRepository, SharedVehiculeRepository>();
            builder.Services.AddScoped<SharedVehiculeRepository>();
            builder.Services.AddScoped<IRepositoryInt<User>, UserRepository>();
            builder.Services.AddScoped<UserRepository>();
            builder.Services.AddScoped<IRepositoryInt<Card>, CardRepository>();
            builder.Services.AddScoped<CardRepository>();

            builder.Services.AddScoped<ITransportationService, TransportationService>();
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // Assert
            var app = builder.Build();

            var serviceProvider = app.Services;
            Assert.NotNull(serviceProvider.GetService<IExtendedShuttleRepository>());
            Assert.NotNull(serviceProvider.GetService<ShuttleRepository>());
            Assert.NotNull(serviceProvider.GetService<IExtendedBikeRepository>());
            Assert.NotNull(serviceProvider.GetService<BikeRepository>());
            Assert.NotNull(serviceProvider.GetService<IExtendedTransportationTransactionRepository>());
            Assert.NotNull(serviceProvider.GetService<TransportationTransactionRepository>());
            Assert.NotNull(serviceProvider.GetService<IExtendedPaymentTransactionRepository>());
            Assert.NotNull(serviceProvider.GetService<PaymentTransactionRepository>());
            Assert.NotNull(serviceProvider.GetService<IExtendedSharedVehiculeRepository>());
            Assert.NotNull(serviceProvider.GetService<SharedVehiculeRepository>());
            Assert.NotNull(serviceProvider.GetService<IRepositoryInt<User>>());
            Assert.NotNull(serviceProvider.GetService<UserRepository>());
            Assert.NotNull(serviceProvider.GetService<IRepositoryInt<Card>>());
            Assert.NotNull(serviceProvider.GetService<CardRepository>());
            Assert.NotNull(serviceProvider.GetService<ITransportationService>());
        }

        [Fact]
        public void Program_Configure_ConfiguresMiddleware()
        {
            // Arrange
            var builder = WebApplication.CreateBuilder();
            ConfigureServices(builder.Services);

            // Act
            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseAuthorization();
            app.MapControllers();

            // Assert
            Assert.NotNull(app);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void Program_Configure_HandlesEnvironmentCorrectly(bool isDevelopment)
        {
            // Arrange
            var builder = WebApplication.CreateBuilder();
            ConfigureServices(builder.Services);
            if (isDevelopment)
            {
                builder.Environment.EnvironmentName = Environments.Development;
            }
            else
            {
                builder.Environment.EnvironmentName = Environments.Production;
            }

            // Act
            var app = builder.Build();
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            // Assert
            var swaggerEndpoint = app.Services.GetService<Swashbuckle.AspNetCore.Swagger.ISwaggerProvider>();
            if (isDevelopment)
            {
                Assert.NotNull(swaggerEndpoint);
            }
        }

        private void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<Context>(options =>
                options.UseInMemoryDatabase("TestDb"));

            services.AddScoped<IExtendedShuttleRepository, ShuttleRepository>();
            services.AddScoped<ShuttleRepository>();
            services.AddScoped<IExtendedBikeRepository, BikeRepository>();
            services.AddScoped<BikeRepository>();
            services.AddScoped<IExtendedTransportationTransactionRepository, TransportationTransactionRepository>();
            services.AddScoped<TransportationTransactionRepository>();
            services.AddScoped<IExtendedPaymentTransactionRepository, PaymentTransactionRepository>();
            services.AddScoped<PaymentTransactionRepository>();
            services.AddScoped<IExtendedSharedVehiculeRepository, SharedVehiculeRepository>();
            services.AddScoped<SharedVehiculeRepository>();
            services.AddScoped<IRepositoryInt<User>, UserRepository>();
            services.AddScoped<UserRepository>();
            services.AddScoped<IRepositoryInt<Card>, CardRepository>();
            services.AddScoped<CardRepository>();

            services.AddScoped<ITransportationService, TransportationService>();
            services.AddControllers();
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();
        }
    }
}