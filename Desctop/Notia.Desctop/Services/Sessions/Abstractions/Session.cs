
using Notia.Desctop.Services.Abstractions;
using Notia.Desctop.Services.DateTime.Abstractions;
using System;

namespace Notia.Desctop.Services.Sessions.Abstractions;

internal class Session
{
    private Session()
    {
    }

    protected Session(
        string accessToken, 
        string refreshToken,
        Guid sessionId,
        DateTimeOffset lastAccess)
    {
        AccessToken = accessToken;
        RefreshToken = refreshToken;
        SessionId = sessionId;
        LastAccess = lastAccess;
    }

    public string AccessToken { get; set; }

    public string RefreshToken { get; set; }

    public Guid SessionId { get; set; }

    public DateTimeOffset LastAccess { get; set; }

    public static Result<Session> Create(string accessToken, string refreshToken, IDateTimeProvider dateTimeProvider)
    {
        var session = new Session(
            accessToken,
            refreshToken,
            Guid.NewGuid(),
            dateTimeProvider.CurrentTime());

        return session;
    }
}
