using System.Threading.Tasks;
using Gym.Core.Entities;

namespace Gym.Core.Interfaces;

public interface ITarifaRepository
{
    Task<Tarifa?> ObtenerTarifaVigenteAsync(string tipo);
}
