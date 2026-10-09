using System;
using System.Threading.Tasks;
using Gym.Core.Entities;
using Gym.Core.Interfaces;
using MySqlConnector;

namespace Gym.Infrastructure.Repositories;

public class SuscripcionRepository : ISuscripcionRepository
{
    private readonly string _connectionString;

    public SuscripcionRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task<Suscripcion?> ObtenerPorIdAsync(int id)
    {
        using var connection = new MySqlConnection(_connectionString);
        await connection.OpenAsync();

        const string query = @"
            SELECT p.id as id, p.id_persona, p.id as id_pago, t.tipo as tipo, p.fecha_inicio, p.fecha_fin 
            FROM pago p
            INNER JOIN tarifa t ON p.id_tarifa = t.id
            WHERE p.id = @id";

        using var command = new MySqlCommand(query, connection);
        command.Parameters.AddWithValue("@id", id);

        using var reader = await command.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            return new Suscripcion
            {
                Id = reader.GetInt32("id"),
                IdPersona = reader.GetInt32("id_persona"),
                IdPago = reader.GetInt32("id_pago"),
                Tipo = reader.GetString("tipo"),
                FechaInicio = DateOnly.FromDateTime(reader.GetDateTime("fecha_inicio")),
                FechaFin = DateOnly.FromDateTime(reader.GetDateTime("fecha_fin"))
            };
        }

        return null;
    }

    public async Task<Suscripcion?> ObtenerSuscripcionVigenteAsync(int idPersona)
    {
        using var connection = new MySqlConnection(_connectionString);
        await connection.OpenAsync();

        // fecha_inicio <= hoy <= fecha_fin AND pago.estado = 1
        const string query = @"
            SELECT p.id as id, p.id_persona, p.id as id_pago, t.tipo as tipo, p.fecha_inicio, p.fecha_fin 
            FROM pago p
            INNER JOIN tarifa t ON p.id_tarifa = t.id
            WHERE p.id_persona = @id_persona 
            AND p.fecha_inicio <= CURDATE() 
            AND p.fecha_fin >= CURDATE()
            AND p.estado = 1
            ORDER BY p.fecha_fin DESC
            LIMIT 1";

        using var command = new MySqlCommand(query, connection);
        command.Parameters.AddWithValue("@id_persona", idPersona);

        using var reader = await command.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            return new Suscripcion
            {
                Id = reader.GetInt32("id"),
                IdPersona = reader.GetInt32("id_persona"),
                IdPago = reader.GetInt32("id_pago"),
                Tipo = reader.GetString("tipo"),
                FechaInicio = DateOnly.FromDateTime(reader.GetDateTime("fecha_inicio")),
                FechaFin = DateOnly.FromDateTime(reader.GetDateTime("fecha_fin"))
            };
        }

        return null;
    }
}
