
using Microsoft.EntityFrameworkCore;
using Notia.Desctop.Data;
using Notia.Desctop.Services.Abstractions;
using Notia.Desctop.Services.Sessions.Abstractions;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Notia.Desctop.Services.Sessions.Local;

internal class LocalSessionService : ISessionService
{
    private readonly ApplicationDbContext _dbContext;

    public LocalSessionService(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result> AddSession(Session session, CancellationToken cancellationToken = default)
    {
        _dbContext.Add(session);
        await _dbContext.SaveChangesAsync();

        return Result.Success();
    }

    public async Task<Result<Session>> GetLastAccessedSession(CancellationToken cancellationToken = default)
    {
        var session = await _dbContext
            .Set<Session>()
            .OrderByDescending(s => s.LastAccess)
            .FirstOrDefaultAsync();

        if (session is null) 
        {
            return Result.Failure<Session>(Error.NullValue);
        }
        return session;
    }

    public async Task<Result> RemoveSession(Guid sessionId, CancellationToken cancellationToken = default)
    {
        await _dbContext.Set<Session>().Where(s=> s.SessionId ==sessionId).ExecuteDeleteAsync();

        return Result.Success();
    }

    public async Task<Result> UpdateSession(Session session, CancellationToken cancellationToken = default)
    {
        _dbContext.Update(session);
        await _dbContext.SaveChangesAsync();

        return Result.Success();
    }
}
