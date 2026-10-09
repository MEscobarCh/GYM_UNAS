using System;

namespace Gym.Core.DTOs;

public class PagoHistorialDto
{
    public int IdPago { get; set; }
    public int IdPersona { get; set; }
    public string Dni { get; set; } = string.Empty;
    public string Nombres { get; set; } = string.Empty;
    public decimal Monto { get; set; }
    public string Metodo { get; set; } = string.Empty;
    public string? NroOperacion { get; set; }
    public DateTime FechaHora { get; set; }
    public int IdUsuario { get; set; }
}
