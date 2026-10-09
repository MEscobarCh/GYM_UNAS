using Gym.Core.Interfaces;

namespace Gym.Infrastructure.Security;

public class BcryptPasswordHasher : IPasswordHasher
{
    public bool VerificarPassword(string passwordTextoPlano, string hash)
    {
        if (string.IsNullOrEmpty(hash))
        {
            return false;
        }

        try
        {
            return BCrypt.Net.BCrypt.Verify(passwordTextoPlano, hash);
        }
        catch (BCrypt.Net.SaltParseException)
        {
            // Manejo seguro por si la base de datos contiene hashes corruptos o inválidos
            return false; 
        }
    }
}
