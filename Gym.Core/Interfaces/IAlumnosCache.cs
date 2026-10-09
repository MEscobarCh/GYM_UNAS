using Gym.Core.Entities;

namespace Gym.Core.Interfaces;

public interface IAlumnosCache
{
    Task GuardarCacheAsync(int idCiclo, IEnumerable<Persona> alumnos);
    Task<IEnumerable<Persona>> ObtenerCacheAsync(int idCiclo);
}
