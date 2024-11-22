
namespace Np.NotesService.Application.Exceptions;

public class ServiceRequestException : Exception
{
    public ServiceRequestException(string? message) : base(message)
    {
    }

    public ServiceRequestException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}