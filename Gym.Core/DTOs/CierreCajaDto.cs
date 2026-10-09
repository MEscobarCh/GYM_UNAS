using System;

namespace Gym.Core.DTOs;

public class CierreCajaDto
{
    public DateTime FechaInicio { get; set; }
    public DateTime FechaFin { get; set; }
    public decimal TotalEfectivo { get; set; }
    public decimal TotalYape { get; set; }
    public decimal TotalGeneral { get; set; }
    public int CantidadPagos { get; set; }
}
