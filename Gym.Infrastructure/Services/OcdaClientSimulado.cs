using System.Text.Json;
using Gym.Core.Entities;
using Gym.Core.DTOs;
using Gym.Core.Interfaces;

namespace Gym.Infrastructure.Services;

public class OcdaClientSimulado : IOcdaClient
{
    private readonly string _filePath;

    public OcdaClientSimulado()
    {
        // Se asume que el archivo estará en el mismo directorio de ejecución
        _filePath = Path.Combine(AppContext.BaseDirectory, "alumnos_simulados.json");
    }

    public async Task<IEnumerable<Ciclo>> ObtenerCiclosAsync()
    {
        // Simulando un retardo de red
        await Task.Delay(500);

        var ciclos = new List<Ciclo>
        {
            new Ciclo
            {
                Id = 1,
                Nombre = "2026-I",
                Estado = 1,
                FechaInicio = new DateOnly(2026, 3, 1),
                FechaFin = new DateOnly(2026, 7, 31),
                CodigoOcda = "OCDA-2026-1"
            },
            new Ciclo
            {
                Id = 2,
                Nombre = "2026-II",
                Estado = 1,
                FechaInicio = new DateOnly(2026, 8, 15),
                FechaFin = new DateOnly(2026, 12, 15),
                CodigoOcda = "OCDA-2026-2"
            }
        };

        return ciclos;
    }

    public async Task<IEnumerable<AlumnoOcdaDto>> ObtenerAlumnosAsync(int idCiclo)
    {
        if (!File.Exists(_filePath))
        {
            return Enumerable.Empty<AlumnoOcdaDto>();
        }

        using var stream = new FileStream(_filePath, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, useAsync: true);
        
        var options = new JsonSerializerOptions 
        { 
            PropertyNameCaseInsensitive = true 
        };

        var personas = await JsonSerializer.DeserializeAsync<List<AlumnoOcdaDto>>(stream, options);

        return personas ?? Enumerable.Empty<AlumnoOcdaDto>();
    }

    public async Task<bool> TestearConexionAsync(string url)
    {
        await Task.Delay(300); // Latencia simulada
        return !string.IsNullOrWhiteSpace(url) && (url.StartsWith("http://") || url.StartsWith("https://"));
    }
}
