namespace UmbracoBridge.Infraestructure.Exceptions;

public class UmbracoCmsUnauthorizedException : Exception
{
    public UmbracoCmsUnauthorizedException(string message, Exception? inner = null) : base(message, inner) { }
}
