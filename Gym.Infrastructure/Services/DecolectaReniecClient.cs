using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using Gym.Core.Entities;
using Gym.Core.Interfaces;

namespace Gym.Infrastructure.Services;

public class DecolectaReniecClient : IReniecClient
{
    private readonly IConfigRepository _configRepository;
    private readonly IHttpClientFactory _httpClientFactory;

    public DecolectaReniecClient(IConfigRepository configRepository, IHttpClientFactory httpClientFactory)
    {
        _configRepository = configRepository;
        _httpClientFactory = httpClientFactory;
    }

    public async Task<Persona?> ConsultarDniAsync(string dni)
    {
        try
        {
            // a) Obtén el token
            var config = await _configRepository.ObtenerPorClaveAsync("token_api_reniec");
            string? token = config?.Valor;
            if (string.IsNullOrWhiteSpace(token))
                return null;

            // b) Petición GET
            var client = _httpClientFactory.CreateClient("DecolectaClient");
            var request = new HttpRequestMessage(HttpMethod.Get, $"https://api.decolecta.com/v1/reniec/dni?numero={dni}");

            // c) Header Authorization
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await client.SendAsync(request);

            // d) Si 200, deserializa el JSON y mapea full_name a Nombres
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                
                // Usamos JsonDocument para extraer full_name fácilmente sin crear un DTO extra si no queremos
                using var document = JsonDocument.Parse(content);
                if (document.RootElement.TryGetProperty("full_name", out JsonElement fullNameElement))
                {
                    return new Persona
                    {
                        Dni = dni,
                        Nombres = fullNameElement.GetString()
                    };
                }
            }

            // Si es 400/401 u otro error, se ignora y retorna null (e)
            return null;
        }
        catch
        {
            // e) Falla silenciosa (Network error, etc.)
            return null;
        }
    }
}
