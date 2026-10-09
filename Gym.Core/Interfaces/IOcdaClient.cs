using Gym.Core.Entities;
using Gym.Core.DTOs;

namespace Gym.Core.Interfaces;

public interface IOcdaClient
{
    Task<IEnumerable<Ciclo>> ObtenerCiclosAsync();
    Task<IEnumerable<AlumnoOcdaDto>> ObtenerAlumnosAsync(int idCiclo);
    Task<bool> TestearConexionAsync(string url);
}
