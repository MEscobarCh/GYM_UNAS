using Gym.Core.Entities;
using Gym.Core.Interfaces;

namespace Gym.Core.Services;

public class ConfigService
{
    private readonly IConfigRepository _configRepository;
    private readonly IOcdaClient _ocdaClient;
    private const string ClaveUrlApi = "url_api_ocda";

    public ConfigService(IConfigRepository configRepository, IOcdaClient ocdaClient)
    {
        _configRepository = configRepository;
        _ocdaClient = ocdaClient;
    }

    public async Task GuardarUrlApiAsync(string url)
    {
        var config = new Config { Clave = ClaveUrlApi, Valor = url };
        await _configRepository.GuardarAsync(config);
    }

    public async Task<string> ObtenerUrlApiAsync()
    {
        var config = await _configRepository.ObtenerPorClaveAsync(ClaveUrlApi);
        return config?.Valor ?? string.Empty;
    }

    public async Task<bool> TestearConexionBdAsync()
    {
        return await _configRepository.ProbarConexionAsync();
    }

    public async Task<bool> TestearConexionBdAsync(string connectionString)
    {
        return await _configRepository.ProbarConexionAsync(connectionString);
    }

    public async Task<bool> TestearConexionApiAsync()
    {
        string url = await ObtenerUrlApiAsync();
        return await TestearConexionApiAsync(url);
    }

    public async Task<bool> TestearConexionApiAsync(string url)
    {
        if (string.IsNullOrWhiteSpace(url)) 
            return false;
        
        return await _ocdaClient.TestearConexionAsync(url);
    }
}
