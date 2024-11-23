
using Avalonia.Threading;
using Microsoft.Extensions.DependencyInjection;
using Notia.Desctop.Routing;
using Notia.Desctop.Services.Abstractions;
using Notia.Desctop.Services.Accounts.Abstractions;
using Notia.Desctop.Services.DateTime.Abstractions;
using Notia.Desctop.Services.Sessions.Abstractions;
using Notia.Desctop.ViewModels.Pages;
using ReactiveUI;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Notia.Desctop.Stores;

internal class SessionStore : ReactiveObject
{
    private readonly IServiceProvider _serviceProvider;
    private readonly AppRouter _appRouter;
    private Session? _currentSession;

    public SessionStore(
        IServiceProvider serviceProvider, 
        AppRouter appRouter)
    {
        _serviceProvider = serviceProvider;
        _appRouter = appRouter;
        OnInitialization(serviceProvider);
    }

    public Session? CurrentSession 
    {
        get => _currentSession;
        private set => this.RaiseAndSetIfChanged(ref _currentSession, value);
    }

    public bool HasSession => _currentSession is not null;

    public async Task<Result> LoginAsync(string username, string password, CancellationToken cancellationToken = default)
    {
        using var scope = _serviceProvider.CreateScope();
        var accountService = scope.ServiceProvider.GetRequiredService<IAccountService>();

        var resultToken = await accountService.Login(username, password, cancellationToken);
        if (resultToken.IsFailed)
        {
            return resultToken;
        }
        var token = resultToken.Value;
        var dateTimeProvider = scope.ServiceProvider.GetRequiredService<IDateTimeProvider>();

        var sessionResult = Session.Create(token.AccessToken, token.RefreshToken, dateTimeProvider); 
        var session = sessionResult.Value; 
        var sessionService = scope.ServiceProvider.GetRequiredService<ISessionService>();

        await sessionService.AddSession(session);

        CurrentSession = session;
        _appRouter.Navigate<TestingLoginPageViewModel>();
        return Result.Success();
    }

    public async Task<Result> UpdateToken()
    {
        using var scope = _serviceProvider.CreateScope();
        var accountService = scope.ServiceProvider.GetRequiredService<IAccountService>();

        if (_currentSession is null)
        {
            throw new InvalidOperationException("Trying update token when SessionStore has not session");
        }

        var resultToken = await accountService.UpdateToken(_currentSession.RefreshToken);
        if (resultToken.IsFailed && resultToken.Error.Equals(AccountServiceErrors.Unauthorized))
        {
            await CloseSession();
            return resultToken;
        }

        var token = resultToken.Value;
        var dateTimeProvider = scope.ServiceProvider.GetRequiredService<IDateTimeProvider>();

        Dispatcher.UIThread.Post(async () => 
        {
            _currentSession.RefreshToken = token.RefreshToken;
            _currentSession.AccessToken = token.AccessToken;
            _currentSession.LastAccess = dateTimeProvider.CurrentTime();

            var sessionService = scope.ServiceProvider.GetRequiredService<ISessionService>();
            await sessionService.UpdateSession(_currentSession);
        });

        return Result.Success();
    }
    public async Task CloseSession()
    {
        if (_currentSession is null)
        {
            return;
        }
        using var scope = _serviceProvider.CreateScope();
        var sessionService = scope.ServiceProvider.GetRequiredService<ISessionService>();
        await sessionService.RemoveSession(_currentSession.SessionId);

        CurrentSession = null!;
    }

    private async void OnInitialization(IServiceProvider serviceProvider)
    {
        using var scope = _serviceProvider.CreateScope();
        var sessionService = scope.ServiceProvider.GetRequiredService<ISessionService>();
        var dateTimeProvider = scope.ServiceProvider.GetRequiredService<IDateTimeProvider>();

        var resultSession = sessionService.GetLastAccessedSession().Result;
        if (resultSession.IsFailed)
        {
            CurrentSession = null!;
            return;
        }
        var session = resultSession.Value;
        session.LastAccess = dateTimeProvider.CurrentTime();

        await sessionService.UpdateSession(session);

        CurrentSession = session;
    }
}
