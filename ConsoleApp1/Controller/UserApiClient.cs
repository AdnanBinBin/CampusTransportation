using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using ConsoleApp1.DTOs.Responses;

namespace ConsoleApp1.Controllers
{
    public class UserApiClient
    {
        private readonly HttpClient _httpClient;

        public UserApiClient(string baseUrl)
        {
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri(baseUrl)
            };
        }

        public async Task<UserTransportationResponseDto> GetUserTransportationsAsync(int userId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"/api/User/transportation/{userId}");
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<UserTransportationResponseDto>();
                    if (result == null)
                    {
                        throw new Exception("La réponse de l'API est vide");
                    }
                    return result;
                }
                var errorContent = await response.Content.ReadFromJsonAsync<ApiErrorResponse>();
                throw new Exception(errorContent?.Error ?? "Une erreur s'est produite lors de la récupération des transactions de transport");
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

        public async Task<UserPaymentResponseDto> GetUserPaymentsAsync(int userId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"/api/User/payment/{userId}");
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<UserPaymentResponseDto>();
                    if (result == null)
                    {
                        throw new Exception("La réponse de l'API est vide");
                    }
                    return result;
                }
                var errorContent = await response.Content.ReadFromJsonAsync<ApiErrorResponse>();
                throw new Exception(errorContent?.Error ?? "Une erreur s'est produite lors de la récupération des transactions de paiement");
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