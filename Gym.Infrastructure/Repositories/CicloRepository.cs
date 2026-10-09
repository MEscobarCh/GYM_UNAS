using Gym.Core.Entities;
using Gym.Core.Interfaces;
using MySqlConnector;

namespace Gym.Infrastructure.Repositories;

public class CicloRepository : ICicloRepository
{
    private readonly string _connectionString;

    public CicloRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task<IEnumerable<Ciclo>> ObtenerTodosAsync()
    {
        var ciclos = new List<Ciclo>();

        using var connection = new MySqlConnection(_connectionString);
        await connection.OpenAsync();

        // Se usa la columna codigo_ocda (asumiendo que en la BD también se actualizó el nombre)
        const string query = "SELECT id, nombre, estado, fecha_inicio, fecha_fin, codigo_ocda FROM ciclo";
        
        using var command = new MySqlCommand(query, connection);
        using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            ciclos.Add(new Ciclo
            {
                Id = reader.GetInt32("id"),
                Nombre = reader.IsDBNull(reader.GetOrdinal("nombre")) ? null : reader.GetString("nombre"),
                Estado = reader.IsDBNull(reader.GetOrdinal("estado")) ? null : reader.GetInt32("estado"),
                
                // Mapeo seguro de fecha a DateOnly
                FechaInicio = reader.IsDBNull(reader.GetOrdinal("fecha_inicio")) 
                    ? null 
                    : DateOnly.FromDateTime(reader.GetDateTime("fecha_inicio")),
                    
                FechaFin = reader.IsDBNull(reader.GetOrdinal("fecha_fin")) 
                    ? null 
                    : DateOnly.FromDateTime(reader.GetDateTime("fecha_fin")),
                    
                CodigoOcda = reader.IsDBNull(reader.GetOrdinal("codigo_ocda")) ? null : reader.GetString("codigo_ocda")
            });
        }

        return ciclos;
    }

    public async Task CrearAsync(Ciclo ciclo)
    {
        using var connection = new MySqlConnection(_connectionString);
        await connection.OpenAsync();

        const string query = @"
            INSERT INTO ciclo (nombre, estado, fecha_inicio, fecha_fin, codigo_ocda) 
            VALUES (@nombre, @estado, @fecha_inicio, @fecha_fin, @codigo_ocda)";

        using var command = new MySqlCommand(query, connection);
        
        command.Parameters.AddWithValue("@nombre", ciclo.Nombre ?? (object)DBNull.Value);
        command.Parameters.AddWithValue("@estado", ciclo.Estado ?? (object)DBNull.Value);
        
        // Convertimos el DateOnly a string 'yyyy-MM-dd' para MySQL tal como solicitaste
        command.Parameters.AddWithValue("@fecha_inicio", ciclo.FechaInicio?.ToString("yyyy-MM-dd") ?? (object)DBNull.Value);
        command.Parameters.AddWithValue("@fecha_fin", ciclo.FechaFin?.ToString("yyyy-MM-dd") ?? (object)DBNull.Value);
        
        command.Parameters.AddWithValue("@codigo_ocda", ciclo.CodigoOcda ?? (object)DBNull.Value);

        await command.ExecuteNonQueryAsync();
    }
}
