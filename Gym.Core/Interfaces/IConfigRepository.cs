using Gym.Core.Entities;

namespace Gym.Core.Interfaces;

public interface IConfigRepository
{
    Task<Config?> ObtenerPorClaveAsync(string clave);
    Task GuardarAsync(Config config);
    Task<bool> ProbarConexionAsync();
    Task<bool> ProbarConexionAsync(string connectionString);
}
