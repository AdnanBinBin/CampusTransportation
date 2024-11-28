using ConsoleApp1.DTOs.Responses;
using System.Net.Http.Json;

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
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<BoardShuttleResponseDto>();
                    if (result == null)
                    {
                        throw new Exception("La réponse de l'API est vide");
                    }
                    return result;
                }

                var errorContent = await response.Content.ReadFromJsonAsync<ApiErrorResponse>();
                throw new Exception(errorContent.Error?? "Une erreur s'est produite lors de l'embarquement dans la navette");
            }
            catch (HttpRequestException ex)
            {
                throw new Exception("Erreur de connexion au serveur", ex);
            }
            catch (Exception ex) when (ex.Message != null)
            {
                throw;
            }
        }
    }
}