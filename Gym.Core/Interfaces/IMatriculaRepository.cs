using Gym.Core.Entities;

namespace Gym.Core.Interfaces;

public interface IMatriculaRepository
{
    Task<Matricula?> ObtenerPorPersonaYCicloAsync(int idPersona, int idCiclo);
    Task CrearAsync(Matricula matricula);
    Task ActualizarAsync(Matricula matricula);
}
