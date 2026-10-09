using System;
using System.Threading.Tasks;
using Gym.Core.Entities;
using Gym.Core.Interfaces;
using MySqlConnector;

namespace Gym.Infrastructure.Repositories;

public class TarifaRepository : ITarifaRepository
{
    private readonly string _connectionString;

    public TarifaRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task<Tarifa?> ObtenerTarifaVigenteAsync(string tipo)
    {
        using var connection = new MySqlConnection(_connectionString);
        await connection.OpenAsync();

        // Adaptado a la estructura de la tabla 'tarifa' (id, nombre, monto, tipo)
        const string query = @"
            SELECT id, tipo, monto 
            FROM tarifa 
            WHERE tipo = @tipo 
            ORDER BY id DESC LIMIT 1";

        using var command = new MySqlCommand(query, connection);
        command.Parameters.AddWithValue("@tipo", tipo);

        using var reader = await command.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            return new Tarifa
            {
                Id = reader.GetInt32("id"),
                Tipo = reader.GetString("tipo"),
                Monto = reader.GetDecimal("monto"),
                VigenteDesde = DateTime.Now, // La BD no guarda esta columna
                IdUsuario = 1 // La BD no guarda esta columna
            };
        }

        return null;
    }
}
