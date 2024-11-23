
using Notia.Desctop.Services.Abstractions;

namespace Notia.Desctop.Services.Sessions.Abstractions;

internal static class SessionServiceErrors
{
    public static Error Unavailable => new("SessionService:Unavailable", "Session service is unavailable");
}
