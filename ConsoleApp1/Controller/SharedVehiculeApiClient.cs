﻿using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using ConsoleApp1.DTOs.Responses;

namespace ConsoleApp1.Controllers
{
    public class SharedVehicleApiClient
    {
        private readonly HttpClient _httpClient;

        public SharedVehicleApiClient(string baseUrl)
        {
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri(baseUrl)
            };
        }

        public async Task<SharedVehicleResponseDto> CreateTripAsync(int driverId, string sharedVehicleId)
        {
            try
            {
                var response = await _httpClient.PostAsync(
                    $"/api/sharedvehicle/createTrip?driverId={driverId}&sharedVehicleId={sharedVehicleId}",
                    null);
                response.EnsureSuccessStatusCode();
                var result = await response.Content.ReadFromJsonAsync<SharedVehicleResponseDto>();

                if (result == null)
                {
                    throw new Exception("La réponse de l'API est vide");
                }

                return result;
            }
            catch (HttpRequestException ex)
            {
                throw new Exception($"Erreur lors de la création du trajet: {ex.Message}", ex);
            }
        }

        public async Task<SharedVehicleResponseDto> RentSharedVehicleAsync(int userId, string sharedVehicleId, int driverId)
        {
            try
            {
                var response = await _httpClient.PostAsync(
                    $"/api/sharedvehicle/rent?userId={userId}&sharedVehicleId={sharedVehicleId}&driverId={driverId}",
                    null);
                response.EnsureSuccessStatusCode();
                var result = await response.Content.ReadFromJsonAsync<SharedVehicleResponseDto>();

                if (result == null)
                {
                    throw new Exception("La réponse de l'API est vide");
                }

                return result;
            }
            catch (HttpRequestException ex)
            {
                throw new Exception($"Erreur lors de la location du véhicule partagé: {ex.Message}", ex);
            }
        }

        public async Task<SharedVehicleResponseDto> EndRentalAsync(string sharedVehicleId, int driverId)
        {
            try
            {
                var response = await _httpClient.PostAsync(
                    $"/api/sharedvehicle/end?sharedVehicleId={sharedVehicleId}&driverId={driverId}",
                    null);
                response.EnsureSuccessStatusCode();
                var result = await response.Content.ReadFromJsonAsync<SharedVehicleResponseDto>();

                if (result == null)
                {
                    throw new Exception("La réponse de l'API est vide");
                }

                return result;
            }
            catch (HttpRequestException ex)
            {
                throw new Exception($"Erreur lors de la fin de location: {ex.Message}", ex);
            }
        }
    }
}