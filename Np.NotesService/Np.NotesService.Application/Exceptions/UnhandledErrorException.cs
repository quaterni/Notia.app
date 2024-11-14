
namespace Np.NotesService.Application.Exceptions;

public class UnhandledErrorException : Exception
{
    public UnhandledErrorException(string errorMessage) : base("Error from result was unhandled")
    {
        ErrorMessage = errorMessage;
    }

    public string ErrorMessage { get; }
}
