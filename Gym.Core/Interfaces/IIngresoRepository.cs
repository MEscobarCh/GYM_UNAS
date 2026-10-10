using Gym.Core.Entities;

namespace Gym.Core.Interfaces;

public interface IIngresoRepository
{
    Task CrearAsync(Ingreso ingreso);
    Task<System.Collections.Generic.IEnumerable<Gym.Core.DTOs.IngresoDto>> ObtenerHistorialAsync(DateTime fechaInicio, DateTime fechaFin, string filtroTexto);
}
