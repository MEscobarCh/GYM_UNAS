using System.Text.Json;
using Gym.Core.Entities;
using Gym.Core.Interfaces;

namespace Gym.Core.Services;

public class CicloService
{
    private readonly ICicloRepository _cicloRepository;
    private readonly IOcdaClient _ocdaClient;

    public CicloService(ICicloRepository cicloRepository, IOcdaClient ocdaClient)
    {
        _cicloRepository = cicloRepository;
        _ocdaClient = ocdaClient;
    }

    public async Task<IEnumerable<Ciclo>> SincronizarCiclosAsync()
    {
        var ciclosBd = await _cicloRepository.ObtenerTodosAsync();
        var ciclosOcda = await _ocdaClient.ObtenerCiclosAsync();

        var codigosBd = ciclosBd.Select(c => c.CodigoOcda).ToHashSet();

        foreach (var cicloOcda in ciclosOcda)
        {
            if (!string.IsNullOrEmpty(cicloOcda.CodigoOcda) && !codigosBd.Contains(cicloOcda.CodigoOcda))
            {
                await _cicloRepository.CrearAsync(cicloOcda);
            }
        }

        return await _cicloRepository.ObtenerTodosAsync();
    }

    public async Task ActualizarCacheAlumnosAsync(int idCiclo)
    {
        var alumnos = await _ocdaClient.ObtenerAlumnosAsync(idCiclo);
        
        if (alumnos != null && alumnos.Any())
        {
            string path = Path.Combine(AppContext.BaseDirectory, $"cache_alumnos_{idCiclo}.json");
            var options = new JsonSerializerOptions { WriteIndented = true };
            
            using var stream = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None, 4096, useAsync: true);
            await JsonSerializer.SerializeAsync(stream, alumnos, options);
        }
    }
}
