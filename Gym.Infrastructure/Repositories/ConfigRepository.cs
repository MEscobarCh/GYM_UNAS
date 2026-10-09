using Gym.Core.Entities;
using Gym.Core.Interfaces;
using MySqlConnector;

namespace Gym.Infrastructure.Repositories;

public class ConfigRepository : IConfigRepository
{
    private readonly string _connectionString;

    public ConfigRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task<Config?> ObtenerPorClaveAsync(string clave)
    {
        using var connection = new MySqlConnection(_connectionString);
        await connection.OpenAsync();

        const string query = "SELECT clave, valor FROM config WHERE clave = @clave LIMIT 1";
        
        using var command = new MySqlCommand(query, connection);
        command.Parameters.AddWithValue("@clave", clave);

        using var reader = await command.ExecuteReaderAsync();
        
        if (await reader.ReadAsync())
        {
            return new Config
            {
                Clave = reader.GetString("clave"),
                Valor = reader.IsDBNull(reader.GetOrdinal("valor")) ? null : reader.GetString("valor")
            };
        }
        
        return null;
    }

    public async Task GuardarAsync(Config config)
    {
        using var connection = new MySqlConnection(_connectionString);
        await connection.OpenAsync();

        // Si la clave ya existe, se actualiza su valor. Si no, se inserta.
        const string query = @"
            INSERT INTO config (clave, valor) 
            VALUES (@clave, @valor) 
            ON DUPLICATE KEY UPDATE valor = @valor";

        using var command = new MySqlCommand(query, connection);
        command.Parameters.AddWithValue("@clave", config.Clave ?? string.Empty);
        command.Parameters.AddWithValue("@valor", config.Valor ?? (object)DBNull.Value);

        await command.ExecuteNonQueryAsync();
    }

    public async Task<bool> ProbarConexionAsync()
    {
        return await ProbarConexionAsync(_connectionString);
    }

    public async Task<bool> ProbarConexionAsync(string connectionString)
    {
        try
        {
            using var connection = new MySqlConnection(connectionString);
            await connection.OpenAsync();
            return connection.State == System.Data.ConnectionState.Open;
        }
        catch
        {
            return false;
        }
    }
}
