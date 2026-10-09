using System;

namespace Gym.Core.Entities;

public class Tarifa
{
    public int Id { get; set; }
    public string Tipo { get; set; } = string.Empty; // "DIA" o "MES"
    public decimal Monto { get; set; }
    public DateTime VigenteDesde { get; set; }
    public int? IdUsuario { get; set; }
}
