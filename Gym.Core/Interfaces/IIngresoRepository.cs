using Gym.Core.Entities;

namespace Gym.Core.Interfaces;

public interface IIngresoRepository
{
    Task CrearAsync(Ingreso ingreso);
}
