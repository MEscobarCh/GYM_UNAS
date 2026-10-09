namespace Gym.Core.Exceptions;

public class CredencialesInvalidasException : Exception
{
    public CredencialesInvalidasException() 
        : base("Usuario o contraseña incorrectos.")
    {
    }

    public CredencialesInvalidasException(string message) 
        : base(message)
    {
    }

    public CredencialesInvalidasException(string message, Exception innerException) 
        : base(message, innerException)
    {
    }
}
