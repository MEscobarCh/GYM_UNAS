using System;

namespace Gym.Core.Entities;

public class Ingreso
{
    public int Id { get; set; }
    public int IdPersona { get; set; }
    public int IdCiclo { get; set; }
    public string? Tipo { get; set; }
    public string? Validacion { get; set; }
    public int? IdSuscripcion { get; set; }
    public DateTime? FechaHora { get; set; }
    public int? IdUsuario { get; set; }
}
