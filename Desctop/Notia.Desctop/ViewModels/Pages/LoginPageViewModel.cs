
using Avalonia.Threading;
using Notia.Desctop.Routing;
using Notia.Desctop.Stores;
using ReactiveUI;
using System;
using System.Reactive;
using System.Threading.Tasks;

namespace Notia.Desctop.ViewModels.Pages;

internal class LoginPageViewModel : PageViewModel
{
    private readonly AppRouter _appRouter;
    private readonly SessionStore _sessionStore;
    private string _username = string.Empty;

    private string _password = string.Empty;

    private bool _isLoggingFailed;

    public LoginPageViewModel(AppRouter appRouter, SessionStore sessionStore)
	{
		LoginCommand = ReactiveCommand.Create(() => Task.Run(Login));
        _appRouter = appRouter;
        _sessionStore = sessionStore;
    }

	public string Username
    {
		get => _username;
		set => this.RaiseAndSetIfChanged(ref _username, value); 
	}

    public string Password
    {
        get => _password;
        set => this.RaiseAndSetIfChanged(ref _password, value);
    }

    public bool IsLoggingFailed
    {
        get => _isLoggingFailed;
        set => this.RaiseAndSetIfChanged(ref _isLoggingFailed, value);
    }

	public ReactiveCommand<Unit,Task>  LoginCommand { get; set; }

    public async Task Login()
    {
        try
        {
            var result = await _sessionStore.LoginAsync(Username, Password);
            if (result.IsFailed)
            {
                Dispatcher.UIThread.Post(() => { IsLoggingFailed = true; });
                return;
            }
        }
        catch(Exception ex)
        {
            Dispatcher.UIThread.Post(() => throw ex);
        }
    }
}
