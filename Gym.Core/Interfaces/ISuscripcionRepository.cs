using System.Threading.Tasks;
using Gym.Core.Entities;

namespace Gym.Core.Interfaces;

public interface ISuscripcionRepository
{
    // Solo para consultas si es necesario, pero la creacin principal est en IPagoRepository para ser atmica
    Task<Suscripcion?> ObtenerPorIdAsync(int id);
    Task<Suscripcion?> ObtenerSuscripcionVigenteAsync(int idPersona);
}
