using System.Threading.Tasks;
using Gym.Core.Entities;
using Gym.Core.Interfaces;

namespace Gym.Core.Services;

public class TarifaService
{
    private readonly ITarifaRepository _tarifaRepository;

    public TarifaService(ITarifaRepository tarifaRepository)
    {
        _tarifaRepository = tarifaRepository;
    }

    public async Task<Tarifa?> ObtenerTarifaVigenteAsync(string tipo)
    {
        return await _tarifaRepository.ObtenerTarifaVigenteAsync(tipo);
    }
}
