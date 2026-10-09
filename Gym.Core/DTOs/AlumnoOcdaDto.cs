using System;

namespace Gym.Core.DTOs;

public class AlumnoOcdaDto
{
    public string? Dni { get; set; }
    public string? Nombres { get; set; }
    public string? Apellidos { get; set; }
    public string? CodigoUniversitario { get; set; }
    public string? Facultad { get; set; }
    public string? Correo { get; set; }
    public string? Telefono { get; set; }
    public DateOnly? FechaRetiro { get; set; }
}
