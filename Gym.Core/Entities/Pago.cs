using System;

namespace Gym.Core.Entities;

public class Pago
{
    public int Id { get; set; }
    public int IdPersona { get; set; }
    public decimal Monto { get; set; }
    public string Metodo { get; set; } = string.Empty; // "EFECTIVO" o "YAPE"
    public string? NroOperacion { get; set; }
    public DateTime FechaHora { get; set; }
    public int IdUsuario { get; set; }
    public int ComprobanteEnviado { get; set; }
    public int Estado { get; set; }
}
