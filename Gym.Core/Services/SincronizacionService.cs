using System;
using System.Linq;
using System.Threading.Tasks;
using Gym.Core.Entities;
using Gym.Core.Interfaces;

namespace Gym.Core.Services;

public class SincronizacionService : ISincronizacionService
{
    private readonly IOcdaClient _ocdaClient;
    private readonly IPersonaRepository _personaRepository;
    private readonly IMatriculaRepository _matriculaRepository;
    private readonly IAuditoriaRepository _auditoriaRepository;

    public SincronizacionService(
        IOcdaClient ocdaClient,
        IPersonaRepository personaRepository,
        IMatriculaRepository matriculaRepository,
        IAuditoriaRepository auditoriaRepository)
    {
        _ocdaClient = ocdaClient;
        _personaRepository = personaRepository;
        _matriculaRepository = matriculaRepository;
        _auditoriaRepository = auditoriaRepository;
    }

    public async Task EjecutarReclasificacionMasivaAsync(int idCiclo, int idUsuarioAdmin)
    {
        var alumnosOcda = await _ocdaClient.ObtenerAlumnosAsync(idCiclo);
        var dnisOcda = alumnosOcda.Where(x => !string.IsNullOrEmpty(x.Dni)).Select(x => x.Dni!).Distinct().ToList();

        if (!dnisOcda.Any()) return;

        var personasLocales = await _personaRepository.ObtenerPorDnisAsync(dnisOcda);

        foreach (var alumnoOcda in alumnosOcda)
        {
            if (string.IsNullOrEmpty(alumnoOcda.Dni) || !personasLocales.TryGetValue(alumnoOcda.Dni, out var personaLocal))
            {
                continue; 
            }

            var matricula = await _matriculaRepository.ObtenerPorPersonaYCicloAsync(personaLocal.Id, idCiclo);
            bool eraExterno = matricula == null || matricula.FechaRetiro != null;
            bool vieneConRetiro = alumnoOcda.FechaRetiro != null;

            if (eraExterno && !vieneConRetiro)
            {
                if (matricula == null)
                {
                    matricula = new Matricula
                    {
                        IdPersona = personaLocal.Id,
                        IdCiclo = idCiclo,
                        FechaMatricula = DateTime.Now,
                        FechaRetiro = null
                    };
                    await _matriculaRepository.CrearAsync(matricula);
                }
                else
                {
                    matricula.FechaRetiro = null;
                    await _matriculaRepository.ActualizarAsync(matricula);
                }

                await _auditoriaRepository.CrearAsync(new Auditoria
                {
                    IdUsuario = idUsuarioAdmin,
                    Accion = "RECLASIFICAR",
                    Detalle = $"DNI {alumnoOcda.Dni} pasó a ALUMNO en ciclo {idCiclo}",
                    FechaHora = DateTime.Now
                });
            }
            else if (!eraExterno && vieneConRetiro)
            {
                matricula!.FechaRetiro = alumnoOcda.FechaRetiro;
                await _matriculaRepository.ActualizarAsync(matricula);

                await _auditoriaRepository.CrearAsync(new Auditoria
                {
                    IdUsuario = idUsuarioAdmin,
                    Accion = "RECLASIFICAR",
                    Detalle = $"DNI {alumnoOcda.Dni} pasó a EXTERNO por retiro en ciclo {idCiclo}",
                    FechaHora = DateTime.Now
                });
            }
        }
    }
}
