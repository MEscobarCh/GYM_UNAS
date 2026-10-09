using System.Threading.Tasks;

namespace Gym.Core.Interfaces;

public interface ISincronizacionService
{
    Task EjecutarReclasificacionMasivaAsync(int idCiclo, int idUsuarioAdmin);
}
