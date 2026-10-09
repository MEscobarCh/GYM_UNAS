using System.Data;
using Gym.Core.Entities;
using Gym.Core.Interfaces;
using MySqlConnector;

namespace Gym.Infrastructure.Repositories;

public class UsuarioRepository : IUsuarioRepository
{
    private readonly string _connectionString;

    public UsuarioRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task<Usuario?> ObtenerPorUsuarioYPasswordAsync(string username, string password)
    {
        using var connection = new MySqlConnection(_connectionString);
        await connection.OpenAsync();

        // Proyección exacta para la tabla "usuario" y los campos que componen la entidad
        const string query = @"
            SELECT id, username, password, rol, estado 
            FROM usuario 
            WHERE username = @username AND password = @password AND estado = 1 
            LIMIT 1";

        using var command = new MySqlCommand(query, connection);
        command.Parameters.AddWithValue("@username", username);
        command.Parameters.AddWithValue("@password", password);

        using var reader = await command.ExecuteReaderAsync();

        if (await reader.ReadAsync())
        {
            return new Usuario
            {
                Id = reader.GetInt32(reader.GetOrdinal("id")),
                NombreUsuario = reader.IsDBNull(reader.GetOrdinal("username")) ? null : reader.GetString(reader.GetOrdinal("username")),
                PasswordHash = reader.IsDBNull(reader.GetOrdinal("password")) ? null : reader.GetString(reader.GetOrdinal("password")),
                Rol = reader.IsDBNull(reader.GetOrdinal("rol")) ? null : reader.GetString(reader.GetOrdinal("rol")),
                Estado = reader.IsDBNull(reader.GetOrdinal("estado")) ? null : reader.GetInt32(reader.GetOrdinal("estado"))
            };
        }

        return null;
    }
}
