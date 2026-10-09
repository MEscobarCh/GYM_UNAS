using System;
using System.Threading.Tasks;
using Gym.Core.Entities;
using Gym.Core.Interfaces;

namespace Gym.Core.Services;

public class PagoService
{
    private readonly IPagoRepository _pagoRepository;
    private readonly SuscripcionService _suscripcionService;

    public PagoService(IPagoRepository pagoRepository, SuscripcionService suscripcionService)
    {
        _pagoRepository = pagoRepository;
        _suscripcionService = suscripcionService;
    }

    public async Task<int> ProcesarPagoYSuscripcionAsync(
        int idPersona,
        decimal monto,
        string metodo,
        string? nroOperacion,
        int idUsuario,
        string tipoSuscripcion)
    {
        // Regla a) Fail fast: Monto > 0
        if (monto <= 0)
        {
            throw new ArgumentException("El monto debe ser mayor a 0.");
        }

        string metodoUpper = metodo.ToUpper();

        // Regla b) YAPE requiere nro_operacion nico y obligatorio
        if (metodoUpper == "YAPE")
        {
            if (string.IsNullOrWhiteSpace(nroOperacion))
            {
                throw new ArgumentException("El nmero de operacin es obligatorio para pagos con YAPE.");
            }

            bool existe = await _pagoRepository.ExisteNroOperacionAsync(nroOperacion);
            if (existe)
            {
                throw new InvalidOperationException("El nmero de operacin ingresado ya ha sido registrado.");
            }
        }
        else if (metodoUpper == "EFECTIVO")
        {
            nroOperacion = null; // Forzamos nulo si es efectivo para mantener consistencia
        }
        else
        {
            throw new ArgumentException("Mtodo de pago no vlido. Use 'EFECTIVO' o 'YAPE'.");
        }

        var fechaActual = DateTime.Now;
        var fechaInicioSuscripcion = DateOnly.FromDateTime(fechaActual);
        var fechaFinSuscripcion = _suscripcionService.CalcularVencimiento(fechaInicioSuscripcion, tipoSuscripcion);

        // Crear entidades
        var pago = new Pago
        {
            IdPersona = idPersona,
            Monto = monto,
            Metodo = metodoUpper,
            NroOperacion = nroOperacion,
            FechaHora = fechaActual,
            IdUsuario = idUsuario,
            ComprobanteEnviado = 0, // Por defecto 0
            Estado = 1 // Asumimos 1 como activo/completado
        };

        var suscripcion = new Suscripcion
        {
            IdPersona = idPersona,
            // IdPago se asignará dentro de la transacción
            Tipo = tipoSuscripcion,
            FechaInicio = fechaInicioSuscripcion,
            FechaFin = fechaFinSuscripcion
        };

        // Regla c) Guardar todo de manera transaccional
        return await _pagoRepository.CrearPagoYSuscripcionAsync(pago, suscripcion);
    }

    public async Task<System.Collections.Generic.List<Gym.Core.DTOs.PagoHistorialDto>> ObtenerHistorialPagosAsync(DateTime fechaInicio, DateTime fechaFin)
    {
        return await _pagoRepository.ObtenerHistorialPagosAsync(fechaInicio, fechaFin);
    }

    public async Task<Gym.Core.DTOs.CierreCajaDto> ObtenerCierreCajaAsync(DateTime fechaInicio, DateTime fechaFin)
    {
        return await _pagoRepository.ObtenerCierreCajaAsync(fechaInicio, fechaFin);
    }
}
