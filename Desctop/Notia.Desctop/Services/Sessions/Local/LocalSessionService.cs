
using Notia.Desctop.Services.Abstractions;
using Notia.Desctop.Services.Sessions.Abstractions;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Notia.Desctop.Services.Sessions.Local;

internal class LocalSessionService : ISessionService
{
    public Task<Result> AddSession(Session session, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(Result.Failure(SessionServiceErrors.Unavailable));
    }

    public Task<Result<Session>> GetLastAccessedSession(CancellationToken cancellationToken = default)
    {
        return Task.FromResult(Result.Failure<Session>(SessionServiceErrors.Unavailable));
    }

    public Task<Result> RemoveSession(Guid sessionId, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(Result.Failure(SessionServiceErrors.Unavailable));
    }

    public Task<Result> UpdateSession(Session session, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(Result.Failure(SessionServiceErrors.Unavailable));
    }
}
