using System.Threading.Tasks;
using Gym.Core.Entities;

namespace Gym.Core.Interfaces;

public interface IPagoRepository
{
    Task<bool> ExisteNroOperacionAsync(string nroOperacion);
    
    // Mtodo transaccional que inserta Pago y luego Suscripcion (usando el LAST_INSERT_ID)
    Task<int> CrearPagoYSuscripcionAsync(Pago pago, Suscripcion suscripcion);

    Task<System.Collections.Generic.List<Gym.Core.DTOs.PagoHistorialDto>> ObtenerHistorialPagosAsync(System.DateTime fechaInicio, System.DateTime fechaFin);
    Task<Gym.Core.DTOs.CierreCajaDto> ObtenerCierreCajaAsync(System.DateTime fechaInicio, System.DateTime fechaFin);
}
