namespace Gym.Core.Entities;

public class Ciclo
{
    public int Id { get; set; }
    public string? Nombre { get; set; }
    public int? Estado { get; set; }
    public DateOnly? FechaInicio { get; set; }
    public DateOnly? FechaFin { get; set; }
    public string? CodigoOcda { get; set; }
}
