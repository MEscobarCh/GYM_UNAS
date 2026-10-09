using Gym.Core.Entities;

namespace Gym.Core.Interfaces;

public interface IAuditoriaRepository
{
    Task CrearAsync(Auditoria auditoria);
}
