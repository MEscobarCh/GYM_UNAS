using System.Collections.Generic;
using System.Threading.Tasks;
using Gym.Core.Entities;

namespace Gym.Core.Interfaces;

public interface IBiometricoClient
{
    void InicializarLector();
    void DetenerLector();
    Task<string?> IniciarCapturaAsync();
    
    Task<Persona?> IdentificarPersonaAsync(IEnumerable<Persona> personasConHuella);
}
