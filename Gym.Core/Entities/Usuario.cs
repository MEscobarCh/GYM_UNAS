namespace Gym.Core.Entities;

public class Usuario
{
    public int Id { get; set; }
    public string? NombreUsuario { get; set; }
    public string? PasswordHash { get; set; }
    public string? Rol { get; set; }
    public int? Estado { get; set; }
}
