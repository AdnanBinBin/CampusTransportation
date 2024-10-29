using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using ConsoleApp1.DTOs.Responses;

namespace ConsoleApp1.Controllers
{
    public class BikeApiClient
    {
        private readonly HttpClient _httpClient;

        public BikeApiClient(string baseUrl)
        {
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri(baseUrl)
            };
        }

        public async Task<RentBikeResponseDto> RentBikeAsync(int userId, string bikeId)
        {
            try
            {
                var response = await _httpClient.PostAsync($"/api/bike/rent?userId={userId}&bikeId={bikeId}", null);
                response.EnsureSuccessStatusCode();
                var result = await response.Content.ReadFromJsonAsync<RentBikeResponseDto>();

                if (result == null)
                {
                    throw new Exception("La réponse de l'API est vide");
                }

                return result;
            }
            catch (HttpRequestException ex)
            {
                throw new Exception($"Erreur lors de la location du vélo: {ex.Message}", ex);
            }
        }

        public async Task<EndBikeRentalResponseDto> EndBikeRentalAsync(int userId, string bikeId)
        {
            try
            {
                var response = await _httpClient.PostAsync($"/api/bike/end?userId={userId}&bikeId={bikeId}", null);
                response.EnsureSuccessStatusCode();
                var result = await response.Content.ReadFromJsonAsync<EndBikeRentalResponseDto>();

                if (result == null)
                {
                    throw new Exception("La réponse de l'API est vide");
                }

                return result;
            }
            catch (HttpRequestException ex)
            {
                throw new Exception($"Erreur lors de la fin de location du vélo: {ex.Message}", ex);
            }
        }
    }
}