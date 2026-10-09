using System.Threading.Tasks;
using Gym.Core.Interfaces;

namespace Gym.Core.Services;

public class ClasificacionService
{
    private readonly IPersonaRepository _personaRepository;
    private readonly IMatriculaRepository _matriculaRepository;

    public ClasificacionService(IPersonaRepository personaRepository, IMatriculaRepository matriculaRepository)
    {
        _personaRepository = personaRepository;
        _matriculaRepository = matriculaRepository;
    }

    public async Task<string> DeterminarTipoPersonaAsync(string dni, int idCiclo)
    {
        var persona = await _personaRepository.ObtenerPorDniAsync(dni);
        
        if (persona == null)
            return "EXTERNO";

        var matricula = await _matriculaRepository.ObtenerPorPersonaYCicloAsync(persona.Id, idCiclo);
        
        // Regla: "Alumno = DNI en la matrícula del ciclo activo y sin fecha_retiro. Todo lo demás es externo"
        if (matricula == null || matricula.FechaRetiro.HasValue)
            return "EXTERNO";

        return "ALUMNO";
    }
}
