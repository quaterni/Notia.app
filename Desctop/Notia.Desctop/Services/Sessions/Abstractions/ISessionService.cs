
using Notia.Desctop.Services.Abstractions;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Notia.Desctop.Services.Sessions.Abstractions;

internal interface ISessionService
{
    Task<Result<Session>> GetLastAccessedSession(CancellationToken cancellationToken = default);

    Task<Result> AddSession(Session session, CancellationToken cancellationToken = default);

    Task<Result> UpdateSession(Session session, CancellationToken cancellationToken = default);

    Task<Result> RemoveSession(Guid sessionId, CancellationToken cancellationToken = default);
}
