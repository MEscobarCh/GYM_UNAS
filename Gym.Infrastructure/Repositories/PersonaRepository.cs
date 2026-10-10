using Gym.Core.Entities;
using Gym.Core.Interfaces;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gym.Infrastructure.Repositories;

public class PersonaRepository : IPersonaRepository
{
    private readonly string _connectionString;

    public PersonaRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task<Persona?> ObtenerPorDniAsync(string dni)
    {
        using var connection = new MySqlConnection(_connectionString);
        await connection.OpenAsync();

        const string query = "SELECT id, dni, nombres, sexo, telefono, correo, fecha_registro, estado, huella FROM persona WHERE dni = @dni LIMIT 1";
        
        using var command = new MySqlCommand(query, connection);
        command.Parameters.AddWithValue("@dni", dni);

        using var reader = await command.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            return MapearPersona(reader);
        }
        return null;
    }

    public async Task<Persona?> ObtenerPorIdAsync(int id)
    {
        using var connection = new MySqlConnection(_connectionString);
        await connection.OpenAsync();

        const string query = "SELECT id, dni, nombres, sexo, telefono, correo, fecha_registro, estado, huella FROM persona WHERE id = @id LIMIT 1";
        
        using var command = new MySqlCommand(query, connection);
        command.Parameters.AddWithValue("@id", id);

        using var reader = await command.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            return MapearPersona(reader);
        }
        return null;
    }

    public async Task<Dictionary<string, Persona>> ObtenerPorDnisAsync(IEnumerable<string> dnis)
    {
        var result = new Dictionary<string, Persona>();
        var dniList = dnis.ToList();
        if (!dniList.Any()) return result;

        using var connection = new MySqlConnection(_connectionString);
        await connection.OpenAsync();

        var inClause = string.Join(",", dniList.Select((_, i) => $"@p{i}"));
        var query = $"SELECT id, dni, nombres, sexo, telefono, correo, fecha_registro, estado, huella FROM persona WHERE dni IN ({inClause})";

        using var command = new MySqlCommand(query, connection);
        for (int i = 0; i < dniList.Count; i++)
        {
            command.Parameters.AddWithValue($"@p{i}", dniList[i]);
        }

        using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            var persona = MapearPersona(reader);
            if (!string.IsNullOrEmpty(persona.Dni))
            {
                result[persona.Dni] = persona;
            }
        }
        return result;
    }

    public async Task CrearAsync(Persona persona)
    {
        using var connection = new MySqlConnection(_connectionString);
        await connection.OpenAsync();

        const string query = @"
            INSERT INTO persona (id, dni, nombres, sexo, telefono, correo, fecha_registro, estado, huella) 
            VALUES (@id, @dni, @nombres, @sexo, @telefono, @correo, @fecha_registro, @estado, @huella)";

        using var command = new MySqlCommand(query, connection);
        command.Parameters.AddWithValue("@id", persona.Id);
        command.Parameters.AddWithValue("@dni", persona.Dni ?? (object)DBNull.Value);
        command.Parameters.AddWithValue("@nombres", persona.Nombres ?? (object)DBNull.Value);
        command.Parameters.AddWithValue("@sexo", persona.Sexo ?? (object)DBNull.Value);
        command.Parameters.AddWithValue("@telefono", persona.Telefono ?? (object)DBNull.Value);
        command.Parameters.AddWithValue("@correo", persona.Correo ?? (object)DBNull.Value);
        command.Parameters.AddWithValue("@fecha_registro", persona.FechaRegistro ?? (object)DBNull.Value);
        command.Parameters.AddWithValue("@estado", persona.Estado ?? (object)DBNull.Value);
        command.Parameters.AddWithValue("@huella", persona.Huella ?? (object)DBNull.Value);

        await command.ExecuteNonQueryAsync();
    }

    public async Task<int> CrearRetornandoIdAsync(Persona persona)
    {
        using var connection = new MySqlConnection(_connectionString);
        await connection.OpenAsync();

        const string query = @"
            INSERT INTO persona (dni, nombres, sexo, telefono, correo, fecha_registro, estado, huella) 
            VALUES (@dni, @nombres, @sexo, @telefono, @correo, @fecha_registro, @estado, @huella);
            SELECT LAST_INSERT_ID();";

        using var command = new MySqlCommand(query, connection);
        command.Parameters.AddWithValue("@dni", persona.Dni ?? (object)DBNull.Value);
        command.Parameters.AddWithValue("@nombres", persona.Nombres ?? (object)DBNull.Value);
        command.Parameters.AddWithValue("@sexo", persona.Sexo ?? (object)DBNull.Value);
        command.Parameters.AddWithValue("@telefono", persona.Telefono ?? (object)DBNull.Value);
        command.Parameters.AddWithValue("@correo", persona.Correo ?? (object)DBNull.Value);
        command.Parameters.AddWithValue("@fecha_registro", persona.FechaRegistro ?? (object)DBNull.Value);
        command.Parameters.AddWithValue("@estado", persona.Estado ?? (object)DBNull.Value);
        command.Parameters.AddWithValue("@huella", persona.Huella ?? (object)DBNull.Value);

        var result = await command.ExecuteScalarAsync();
        return Convert.ToInt32(result);
    }

    public async Task ActualizarHuellaAsync(int idPersona, string Huella)
    {
        using var connection = new MySqlConnection(_connectionString);
        await connection.OpenAsync();

        const string query = "UPDATE persona SET huella = @huella WHERE id = @id";
        using var command = new MySqlCommand(query, connection);
        command.Parameters.AddWithValue("@id", idPersona);
        command.Parameters.AddWithValue("@huella", Huella);

        await command.ExecuteNonQueryAsync();
    }

    public async Task<IEnumerable<Persona>> ObtenerTodasConHuellaAsync()
    {
        var result = new List<Persona>();
        using var connection = new MySqlConnection(_connectionString);
        await connection.OpenAsync();

        const string query = "SELECT id, dni, nombres, sexo, telefono, correo, fecha_registro, estado, huella FROM persona WHERE huella IS NOT NULL AND huella != ''";
        
        using var command = new MySqlCommand(query, connection);
        using var reader = await command.ExecuteReaderAsync();
        
        while (await reader.ReadAsync())
        {
            result.Add(MapearPersona(reader));
        }
        return result;
    }

    private static Persona MapearPersona(MySqlDataReader reader)
    {
        return new Persona
        {
            Id = reader.GetInt32("id"),
            Dni = reader.IsDBNull(reader.GetOrdinal("dni")) ? null : reader.GetString("dni"),
            Nombres = reader.IsDBNull(reader.GetOrdinal("nombres")) ? null : reader.GetString("nombres"),
            Sexo = reader.IsDBNull(reader.GetOrdinal("sexo")) ? null : reader.GetString("sexo"),
            Telefono = reader.IsDBNull(reader.GetOrdinal("telefono")) ? null : reader.GetString("telefono"),
            Correo = reader.IsDBNull(reader.GetOrdinal("correo")) ? null : reader.GetString("correo"),
            FechaRegistro = reader.IsDBNull(reader.GetOrdinal("fecha_registro")) ? null : reader.GetDateTime("fecha_registro"),
            Estado = reader.IsDBNull(reader.GetOrdinal("estado")) ? null : reader.GetInt32("estado"),
            Huella = reader.IsDBNull(reader.GetOrdinal("huella")) ? null : reader.GetString("huella")
        };
    }
}


