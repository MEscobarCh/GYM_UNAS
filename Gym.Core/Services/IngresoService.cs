using System;
using System.Threading.Tasks;
using Gym.Core.Entities;
using Gym.Core.Interfaces;

namespace Gym.Core.Services;

public class IngresoService
{
    private readonly IIngresoRepository _ingresoRepository;
    private readonly ClasificacionService _clasificacionService;
    private readonly IPersonaRepository _personaRepository;

    public IngresoService(IIngresoRepository ingresoRepository, ClasificacionService clasificacionService, IPersonaRepository personaRepository)
    {
        _ingresoRepository = ingresoRepository;
        _clasificacionService = clasificacionService;
        _personaRepository = personaRepository;
    }

    public async Task RegistrarIngresoAlumnoAsync(string dni, int idCiclo, string validacion, int idUsuario)
    {
        string tipo = await _clasificacionService.DeterminarTipoPersonaAsync(dni, idCiclo);
        
        var persona = await _personaRepository.ObtenerPorDniAsync(dni);
        int idPersona = persona?.Id ?? 0;
        
        var ingreso = new Ingreso
        {
            IdPersona = idPersona,
            IdCiclo = idCiclo,
            Tipo = tipo, // "ALUMNO" o "EXTERNO"
            Validacion = validacion, // "DNI" o "CARNET"
            FechaHora = DateTime.Now,
            IdUsuario = idUsuario
        };

        await _ingresoRepository.CrearAsync(ingreso);
    }
}
