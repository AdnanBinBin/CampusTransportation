using BLL.Services;
using DAL.DB;
using DAL.DB.Model;
using DAL.DB.Repositories;
using DAL.DB.Repositories.Interfaces;
using DAL.Repositories;
using Microsoft.EntityFrameworkCore;

[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
public partial class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

// Ajouter les services au conteneur.

// Configuration de la chaîne de connexion (modifiez-la avec vos propres paramètres)
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// Ajouter le contexte de la base de données (si vous utilisez Entity Framework Core)
builder.Services.AddDbContext<Context>(options =>
    options.UseSqlServer(connectionString));

// Enregistrer les dépôts (repositories)
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
 

// Enregistrer les services
builder.Services.AddScoped<ITransportationService, TransportationService>();

builder.Services.AddControllers();

// Configuration de Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configurer le pipeline de requêtes HTTP
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
    }
}