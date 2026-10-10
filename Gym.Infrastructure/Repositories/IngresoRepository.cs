using Gym.Core.Entities;
using Gym.Core.Interfaces;
using MySqlConnector;
using System;
using System.Threading.Tasks;

namespace Gym.Infrastructure.Repositories;

public class IngresoRepository : IIngresoRepository
{
    private readonly string _connectionString;

    public IngresoRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task CrearAsync(Ingreso ingreso)
    {
        using var connection = new MySqlConnection(_connectionString);
        await connection.OpenAsync();

        // Corregido el nombre de la tabla a "ingreso" y las columnas mapeadas directamente a la entidad
        const string query = @"
            INSERT INTO ingreso (id_persona, tipo, fecha_hora) 
            VALUES (@id_persona, @tipo, @fecha_hora)";

        using var command = new MySqlCommand(query, connection);
        command.Parameters.AddWithValue("@id_persona", ingreso.IdPersona);
        command.Parameters.AddWithValue("@tipo", ingreso.Tipo ?? (object)DBNull.Value);
        command.Parameters.AddWithValue("@fecha_hora", ingreso.FechaHora ?? (object)DBNull.Value);

        await command.ExecuteNonQueryAsync();
    }

    public async Task<System.Collections.Generic.IEnumerable<Gym.Core.DTOs.IngresoDto>> ObtenerHistorialAsync(DateTime fechaInicio, DateTime fechaFin, string filtroTexto)
    {
        using var connection = new MySqlConnection(_connectionString);
        await connection.OpenAsync();

        string query = @"
            SELECT i.id as Id, i.fecha_hora as FechaHora, p.dni as Dni, p.nombres as Nombres, i.tipo as Tipo
            FROM ingreso i
            INNER JOIN persona p ON i.id_persona = p.id
            WHERE i.fecha_hora >= @inicio AND i.fecha_hora <= @fin";

        if (!string.IsNullOrWhiteSpace(filtroTexto))
        {
            query += " AND (p.dni LIKE @filtro OR p.nombres LIKE @filtro)";
        }

        query += " ORDER BY i.fecha_hora DESC LIMIT 500"; // Top 500 para evitar cargar demasiada data de golpe

        using var command = new MySqlCommand(query, connection);
        command.Parameters.AddWithValue("@inicio", fechaInicio);
        // Al final del da para que incluya las horas de la fechaFin
        command.Parameters.AddWithValue("@fin", fechaFin.Date.AddDays(1).AddSeconds(-1));
        
        if (!string.IsNullOrWhiteSpace(filtroTexto))
        {
            command.Parameters.AddWithValue("@filtro", "%" + filtroTexto + "%");
        }

        var list = new System.Collections.Generic.List<Gym.Core.DTOs.IngresoDto>();
        using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            list.Add(new Gym.Core.DTOs.IngresoDto
            {
                Id = reader.GetInt32("Id"),
                FechaHora = reader.GetDateTime("FechaHora"),
                Dni = reader.IsDBNull(reader.GetOrdinal("Dni")) ? "" : reader.GetString("Dni"),
                Nombres = reader.IsDBNull(reader.GetOrdinal("Nombres")) ? "" : reader.GetString("Nombres"),
                Tipo = reader.IsDBNull(reader.GetOrdinal("Tipo")) ? "" : reader.GetString("Tipo"),
                Validacion = ""
            });
        }
        return list;
    }
}
