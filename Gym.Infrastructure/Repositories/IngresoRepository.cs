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
}
