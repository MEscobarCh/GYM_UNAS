using System;

namespace Gym.Core.Entities;

public class Auditoria
{
    public int Id { get; set; }
    public int? IdUsuario { get; set; }
    public string? Accion { get; set; }
    public string? Detalle { get; set; }
    public DateTime? FechaHora { get; set; }
}
