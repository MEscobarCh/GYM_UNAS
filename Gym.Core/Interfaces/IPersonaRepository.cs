using Gym.Core.Entities;

namespace Gym.Core.Interfaces;

public interface IPersonaRepository
{
    Task<Persona?> ObtenerPorDniAsync(string dni);
    Task<Persona?> ObtenerPorIdAsync(int id);
    Task<Dictionary<string, Persona>> ObtenerPorDnisAsync(IEnumerable<string> dnis);
    Task CrearAsync(Persona persona);
    Task<int> CrearRetornandoIdAsync(Persona persona);

    Task ActualizarHuellaAsync(int idPersona, string huella);
    Task<System.Collections.Generic.IEnumerable<Persona>> ObtenerTodasConHuellaAsync();
}


