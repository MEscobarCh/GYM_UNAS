using System;

namespace Gym.Core.Entities;

public class Suscripcion
{
    public int Id { get; set; }
    public int IdPersona { get; set; }
    public int IdPago { get; set; }
    public string Tipo { get; set; } = string.Empty; // "DIA" o "MES"
    public DateOnly FechaInicio { get; set; }
    public DateOnly FechaFin { get; set; }
}
