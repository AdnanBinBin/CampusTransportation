using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using ConsoleApp1.DTOs.Responses;

namespace ConsoleApp1.Controllers
{
    public class ShuttleApiClient
    {
        private readonly HttpClient _httpClient;

        public ShuttleApiClient(string baseUrl)
        {
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri(baseUrl)
            };
        }

        public async Task<BoardShuttleResponseDto> BoardShuttleAsync(int userId, string shuttleId)
        {
            try
            {
                var response = await _httpClient.PostAsync($"/api/shuttle/board?userId={userId}&shuttleId={shuttleId}", null);
                response.EnsureSuccessStatusCode();
                var result = await response.Content.ReadFromJsonAsync<BoardShuttleResponseDto>();

                if (result == null)
                {
                    throw new Exception("La réponse de l'API est vide");
                }

                return result;
            }
            catch (HttpRequestException ex)
            {
                throw new Exception($"Erreur lors de l'embarquement dans la navette: {ex.Message}", ex);
            }
        }
    }
}