using Gym.Core.Entities;
using Gym.Core.Interfaces;
using MySqlConnector;
using System;
using System.Threading.Tasks;

namespace Gym.Infrastructure.Repositories;

public class MatriculaRepository : IMatriculaRepository
{
    private readonly string _connectionString;

    public MatriculaRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task<Matricula?> ObtenerPorPersonaYCicloAsync(int idPersona, int idCiclo)
    {
        using var connection = new MySqlConnection(_connectionString);
        await connection.OpenAsync();

        const string query = "SELECT id, id_persona, id_ciclo, fecha_matricula, fecha_retiro FROM matricula WHERE id_persona = @idPersona AND id_ciclo = @idCiclo LIMIT 1";
        
        using var command = new MySqlCommand(query, connection);
        command.Parameters.AddWithValue("@idPersona", idPersona);
        command.Parameters.AddWithValue("@idCiclo", idCiclo);

        using var reader = await command.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            return new Matricula
            {
                Id = reader.GetInt32("id"),
                IdPersona = reader.GetInt32("id_persona"),
                IdCiclo = reader.GetInt32("id_ciclo"),
                FechaMatricula = reader.GetDateTime("fecha_matricula"),
                FechaRetiro = reader.IsDBNull(reader.GetOrdinal("fecha_retiro")) ? null : DateOnly.FromDateTime(reader.GetDateTime("fecha_retiro"))
            };
        }
        return null;
    }

    public async Task CrearAsync(Matricula matricula)
    {
        using var connection = new MySqlConnection(_connectionString);
        await connection.OpenAsync();

        const string query = @"
            INSERT INTO matricula (id, id_persona, id_ciclo, fecha_matricula, fecha_retiro) 
            VALUES (@id, @id_persona, @id_ciclo, @fecha_matricula, @fecha_retiro)";

        using var command = new MySqlCommand(query, connection);
        command.Parameters.AddWithValue("@id", matricula.Id);
        command.Parameters.AddWithValue("@id_persona", matricula.IdPersona);
        command.Parameters.AddWithValue("@id_ciclo", matricula.IdCiclo);
        command.Parameters.AddWithValue("@fecha_matricula", matricula.FechaMatricula);
        command.Parameters.AddWithValue("@fecha_retiro", matricula.FechaRetiro?.ToString("yyyy-MM-dd") ?? (object)DBNull.Value);

        await command.ExecuteNonQueryAsync();
    }

    public async Task ActualizarAsync(Matricula matricula)
    {
        using var connection = new MySqlConnection(_connectionString);
        await connection.OpenAsync();

        const string query = @"
            UPDATE matricula 
            SET fecha_retiro = @fecha_retiro
            WHERE id = @id";

        using var command = new MySqlCommand(query, connection);
        command.Parameters.AddWithValue("@id", matricula.Id);
        command.Parameters.AddWithValue("@fecha_retiro", matricula.FechaRetiro?.ToString("yyyy-MM-dd") ?? (object)DBNull.Value);

        await command.ExecuteNonQueryAsync();
    }
}
