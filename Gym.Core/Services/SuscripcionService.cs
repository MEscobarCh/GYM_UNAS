using System;
using System.Threading.Tasks;
using Gym.Core.Entities;
using Gym.Core.Interfaces;

namespace Gym.Core.Services;

public class SuscripcionService
{
    private readonly ISuscripcionRepository _suscripcionRepository;

    public SuscripcionService(ISuscripcionRepository suscripcionRepository)
    {
        _suscripcionRepository = suscripcionRepository;
    }

    public DateOnly CalcularVencimiento(DateOnly fechaInicio, string tipo)
    {
        if (tipo.ToUpper() == "DIARIO")
        {
            return fechaInicio; // Vence el mismo da
        }
        else if (tipo.ToUpper() == "MENSUAL")
        {
            // AddMonths ajusta automticamente al ltimo da del mes si el da original no existe 
            // (ej: 31 de enero -> 28/29 de febrero)
            return fechaInicio.AddMonths(1);
        }

        throw new ArgumentException("Tipo de suscripción no válido. Debe ser 'DIA' o 'MES'.");
    }

    public async Task<Suscripcion?> ObtenerSuscripcionVigenteAsync(int idPersona)
    {
        return await _suscripcionRepository.ObtenerSuscripcionVigenteAsync(idPersona);
    }
}
