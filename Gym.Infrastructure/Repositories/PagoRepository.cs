using System;
using System.Threading.Tasks;
using Gym.Core.Entities;
using Gym.Core.Interfaces;
using MySqlConnector;

namespace Gym.Infrastructure.Repositories;

public class PagoRepository : IPagoRepository
{
    private readonly string _connectionString;

    public PagoRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task<bool> ExisteNroOperacionAsync(string nroOperacion)
    {
        // La tabla pago en la BD actual no tiene columna nro_operacion, así que siempre retorna false
        return await Task.FromResult(false);
    }

    public async Task<int> CrearPagoYSuscripcionAsync(Pago pago, Suscripcion suscripcion)
    {
        using var connection = new MySqlConnection(_connectionString);
        await connection.OpenAsync();

        using var transaction = await connection.BeginTransactionAsync();

        try
        {
            // 0. Obtener id_tarifa basado en el tipo de suscripción
            const string queryTarifa = "SELECT id FROM tarifa WHERE tipo = @tipo LIMIT 1";
            using var cmdTarifa = new MySqlCommand(queryTarifa, connection, transaction);
            cmdTarifa.Parameters.AddWithValue("@tipo", suscripcion.Tipo);
            var resultTarifa = await cmdTarifa.ExecuteScalarAsync();
            int idTarifa = resultTarifa != null ? Convert.ToInt32(resultTarifa) : 1; // Default a 1 si no encuentra

            // 1. Insertar en pago (que en esta BD almacena también los datos de la suscripción)
            const string queryPago = @"
                INSERT INTO pago (id_persona, id_tarifa, monto, fecha_inicio, fecha_fin, fecha_pago, metodo, estado) 
                VALUES (@id_persona, @id_tarifa, @monto, @fecha_inicio, @fecha_fin, @fecha_pago, @metodo, @estado);
                SELECT LAST_INSERT_ID();";

            using var commandPago = new MySqlCommand(queryPago, connection, transaction);
            commandPago.Parameters.AddWithValue("@id_persona", pago.IdPersona);
            commandPago.Parameters.AddWithValue("@id_tarifa", idTarifa);
            commandPago.Parameters.AddWithValue("@monto", pago.Monto);
            commandPago.Parameters.AddWithValue("@fecha_inicio", suscripcion.FechaInicio.ToString("yyyy-MM-dd"));
            commandPago.Parameters.AddWithValue("@fecha_fin", suscripcion.FechaFin.ToString("yyyy-MM-dd"));
            commandPago.Parameters.AddWithValue("@fecha_pago", pago.FechaHora);
            commandPago.Parameters.AddWithValue("@metodo", pago.Metodo);
            commandPago.Parameters.AddWithValue("@estado", pago.Estado);

            var result = await commandPago.ExecuteScalarAsync();
            int idPago = Convert.ToInt32(result);

            // No intentamos insertar en 'suscripcion' porque esa tabla no existe en esta arquitectura de BD.
            
            await transaction.CommitAsync();

            return idPago;
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<System.Collections.Generic.List<Gym.Core.DTOs.PagoHistorialDto>> ObtenerHistorialPagosAsync(DateTime fechaInicio, DateTime fechaFin)
    {
        var result = new System.Collections.Generic.List<Gym.Core.DTOs.PagoHistorialDto>();
        using var connection = new MySqlConnection(_connectionString);
        await connection.OpenAsync();

        const string query = @"
            SELECT p.id as IdPago, p.id_persona as IdPersona, pe.dni as Dni, pe.nombres as Nombres, 
                   p.monto as Monto, p.metodo as Metodo, NULL as NroOperacion, 
                   p.fecha_pago as FechaHora, 1 as IdUsuario 
            FROM pago p
            INNER JOIN persona pe ON p.id_persona = pe.id
            WHERE p.fecha_pago >= @fecha_inicio AND p.fecha_pago <= @fecha_fin
            ORDER BY p.fecha_pago DESC";

        using var command = new MySqlCommand(query, connection);
        command.Parameters.AddWithValue("@fecha_inicio", fechaInicio);
        command.Parameters.AddWithValue("@fecha_fin", fechaFin);

        using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            result.Add(new Gym.Core.DTOs.PagoHistorialDto
            {
                IdPago = reader.GetInt32("IdPago"),
                IdPersona = reader.GetInt32("IdPersona"),
                Dni = reader.IsDBNull(reader.GetOrdinal("Dni")) ? string.Empty : reader.GetString("Dni"),
                Nombres = reader.IsDBNull(reader.GetOrdinal("Nombres")) ? string.Empty : reader.GetString("Nombres"),
                Monto = reader.GetDecimal("Monto"),
                Metodo = reader.GetString("Metodo"),
                NroOperacion = null, // La BD no lo guarda
                FechaHora = reader.GetDateTime("FechaHora"),
                IdUsuario = 1 // Dummy
            });
        }
        return result;
    }

    public async Task<Gym.Core.DTOs.CierreCajaDto> ObtenerCierreCajaAsync(DateTime fechaInicio, DateTime fechaFin)
    {
        using var connection = new MySqlConnection(_connectionString);
        await connection.OpenAsync();

        const string query = @"
            SELECT 
                SUM(CASE WHEN metodo = 'EFECTIVO' THEN monto ELSE 0 END) AS TotalEfectivo,
                SUM(CASE WHEN metodo = 'YAPE' THEN monto ELSE 0 END) AS TotalYape,
                SUM(monto) AS TotalGeneral,
                COUNT(id) AS CantidadPagos
            FROM pago
            WHERE fecha_pago >= @fecha_inicio AND fecha_pago <= @fecha_fin";

        using var command = new MySqlCommand(query, connection);
        command.Parameters.AddWithValue("@fecha_inicio", fechaInicio);
        command.Parameters.AddWithValue("@fecha_fin", fechaFin);

        using var reader = await command.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            return new Gym.Core.DTOs.CierreCajaDto
            {
                FechaInicio = fechaInicio,
                FechaFin = fechaFin,
                TotalEfectivo = reader.IsDBNull(reader.GetOrdinal("TotalEfectivo")) ? 0 : reader.GetDecimal("TotalEfectivo"),
                TotalYape = reader.IsDBNull(reader.GetOrdinal("TotalYape")) ? 0 : reader.GetDecimal("TotalYape"),
                TotalGeneral = reader.IsDBNull(reader.GetOrdinal("TotalGeneral")) ? 0 : reader.GetDecimal("TotalGeneral"),
                CantidadPagos = reader.GetInt32("CantidadPagos")
            };
        }

        return new Gym.Core.DTOs.CierreCajaDto { FechaInicio = fechaInicio, FechaFin = fechaFin };
    }
}
