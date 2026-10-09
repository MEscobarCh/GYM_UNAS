using System.Threading.Tasks;
using Gym.Core.Entities;

namespace Gym.Core.Interfaces;

public interface IReniecClient
{
    Task<Persona?> ConsultarDniAsync(string dni);
}
