using Gym.Core.Entities;

namespace Gym.Core.Interfaces;

public interface ICicloRepository
{
    Task<IEnumerable<Ciclo>> ObtenerTodosAsync();
    Task CrearAsync(Ciclo ciclo);
}
