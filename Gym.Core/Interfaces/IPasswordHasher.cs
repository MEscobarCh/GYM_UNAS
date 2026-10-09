namespace Gym.Core.Interfaces;

public interface IPasswordHasher
{
    bool VerificarPassword(string passwordTextoPlano, string hash);
}
