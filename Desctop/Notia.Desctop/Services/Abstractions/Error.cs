
namespace Notia.Desctop.Services.Abstractions;

public record Error(string Name, string Description)
{
    public static Error NullValue => new("NullValue", "Value was null");
}
