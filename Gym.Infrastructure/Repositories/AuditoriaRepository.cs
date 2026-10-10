using Gym.Core.Entities;
using Gym.Core.Interfaces;
using MySqlConnector;
using System;
using System.Threading.Tasks;

namespace Gym.Infrastructure.Repositories;

public class AuditoriaRepository : IAuditoriaRepository
{
    private readonly string _connectionString;

    public AuditoriaRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task CrearAsync(Auditoria auditoria)
    {
        using var connection = new MySqlConnection(_connectionString);
        await connection.OpenAsync();

        const string query = @"
            INSERT INTO auditoria (id_usuario_admin, accion, detalle, fecha) 
            VALUES (@id_usuario, @accion, @detalle, @fecha_hora)";

        using var command = new MySqlCommand(query, connection);
        command.Parameters.AddWithValue("@id_usuario", auditoria.IdUsuario ?? (object)DBNull.Value);
        command.Parameters.AddWithValue("@accion", auditoria.Accion ?? (object)DBNull.Value);
        command.Parameters.AddWithValue("@detalle", auditoria.Detalle ?? (object)DBNull.Value);
        command.Parameters.AddWithValue("@fecha_hora", auditoria.FechaHora ?? (object)DBNull.Value);

        await command.ExecuteNonQueryAsync();
    }
}
