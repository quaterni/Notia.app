namespace Np.UsersService.Core.Exceptions;

public class ConcurrencyException : Exception
{
    public ConcurrencyException() : base("Concurrent exception was thrown")
    {
    }

    public ConcurrencyException(Exception innerException) : base("Concurrent exception was thrown", innerException)
    {
    }
}
