using Gym.Core.Entities;

namespace Gym.Core.Interfaces;

public interface IUsuarioRepository
{
    Task<Usuario?> ObtenerPorUsuarioYPasswordAsync(string username, string password);
}
