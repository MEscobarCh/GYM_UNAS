using System.Text.Json;
using Gym.Core.Entities;
using Gym.Core.Interfaces;

namespace Gym.Infrastructure.Services;

public class LocalFileAlumnosCache : IAlumnosCache
{
    private string ObtenerRutaArchivo(int idCiclo) 
        => Path.Combine(AppContext.BaseDirectory, $"cache_alumnos_ciclo_{idCiclo}.json");

    public async Task GuardarCacheAsync(int idCiclo, IEnumerable<Persona> alumnos)
    {
        var options = new JsonSerializerOptions { WriteIndented = true };
        
        using var stream = new FileStream(
            ObtenerRutaArchivo(idCiclo), 
            FileMode.Create, 
            FileAccess.Write, 
            FileShare.None, 
            4096, 
            useAsync: true);
            
        await JsonSerializer.SerializeAsync(stream, alumnos, options);
    }

    public async Task<IEnumerable<Persona>> ObtenerCacheAsync(int idCiclo)
    {
        var ruta = ObtenerRutaArchivo(idCiclo);
        
        if (!File.Exists(ruta)) 
        {
            return Enumerable.Empty<Persona>();
        }

        using var stream = new FileStream(
            ruta, 
            FileMode.Open, 
            FileAccess.Read, 
            FileShare.Read, 
            4096, 
            useAsync: true);
            
        var alumnos = await JsonSerializer.DeserializeAsync<IEnumerable<Persona>>(stream);
        return alumnos ?? Enumerable.Empty<Persona>();
    }
}
