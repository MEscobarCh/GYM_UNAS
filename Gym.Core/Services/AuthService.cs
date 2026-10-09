using Gym.Core.Entities;
using Gym.Core.Exceptions;
using Gym.Core.Interfaces;

namespace Gym.Core.Services;

public class AuthService
{
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IPasswordHasher _passwordHasher;

    public AuthService(IUsuarioRepository usuarioRepository, IPasswordHasher passwordHasher)
    {
        _usuarioRepository = usuarioRepository;
        _passwordHasher = passwordHasher;
    }

    public async Task<Usuario> LoginAsync(string usuario, string password)
    {
        var user = await _usuarioRepository.ObtenerPorUsuarioYPasswordAsync(usuario, password);

        if (user == null)
        {
            throw new CredencialesInvalidasException();
        }

        return user;
    }
}
