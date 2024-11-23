
using Microsoft.Extensions.Hosting;
using Notia.Desctop.Routing;
using Notia.Desctop.Services.Sessions.Abstractions;
using Notia.Desctop.Stores;
using Notia.Desctop.ViewModels.Pages;
using ReactiveUI;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Notia.Desctop.BackgroundWorkers;

internal class SessionObserver : BackgroundService
{
    private readonly AppRouter _appRouter;
    private readonly SessionStore _sessionStore;
    private IDisposable? _observableSubscription;

    public SessionObserver(
        AppRouter appRouter,
        SessionStore sessionStore)
    {
        _appRouter = appRouter;
        _sessionStore = sessionStore;
    }

    public override Task StartAsync(CancellationToken cancellationToken)
    {
        _observableSubscription = _sessionStore
            .WhenAnyValue(s => s.CurrentSession)
            .Subscribe(MoveToLoginIfNull);
        return base.StartAsync(cancellationToken);
    }

    public override Task StopAsync(CancellationToken cancellationToken)
    {
        _observableSubscription?.Dispose();

        return base.StopAsync(cancellationToken);
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        return Task.CompletedTask;
    }

    private void MoveToLoginIfNull(Session? session)
    {
        if (session == null)
        {
            _appRouter.Navigate<LoginPageViewModel>();
        }
    }
}