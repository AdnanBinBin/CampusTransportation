using BLL.Services;
using DAL.DB;
using DAL.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Ajouter les services au conteneur.

// Configuration de la cha�ne de connexion (modifiez-la avec vos propres param�tres)
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// Ajouter le contexte de la base de donn�es (si vous utilisez Entity Framework Core)
builder.Services.AddDbContext<Context>(options =>
    options.UseSqlServer(connectionString));

// Enregistrer les d�p�ts (repositories)
builder.Services.AddScoped<ShuttleRepository>();
builder.Services.AddScoped<BikeRepository>();
builder.Services.AddScoped<TransportationTransactionRepository>();
builder.Services.AddScoped<PaymentTransactionRepository>();
builder.Services.AddScoped<SharedVehiculeRepository>();
builder.Services.AddScoped<UserRepository>();
builder.Services.AddScoped<CardRepository>();

// Enregistrer les services
builder.Services.AddScoped<ITransportationService, TransportationService>();

builder.Services.AddControllers();

// Configuration de Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configurer le pipeline de requ�tes HTTP
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
