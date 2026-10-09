namespace Gym.Core.Entities;

public class Matricula
{
    public int Id { get; set; }
    public int IdPersona { get; set; }
    public int IdCiclo { get; set; }
    public DateTime FechaMatricula { get; set; }
    public DateOnly? FechaRetiro { get; set; }
}
