using System;

namespace Gym.Core.DTOs;

public class IngresoDto
{
    public int Id { get; set; }
    public DateTime FechaHora { get; set; }
    public string Dni { get; set; } = string.Empty;
    public string Nombres { get; set; } = string.Empty;
    public string Tipo { get; set; } = string.Empty;
    public string Validacion { get; set; } = string.Empty;
}
