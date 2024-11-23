
using Notia.Desctop.Routing;
using Notia.Desctop.Stores;
using ReactiveUI;
using System.Reactive;
using System.Threading.Tasks;

namespace Notia.Desctop.ViewModels.Pages;

internal class TestingLoginPageViewModel : PageViewModel
{
    private readonly SessionStore _sessionStore;

    public TestingLoginPageViewModel(SessionStore sessionStore)
    {
        _sessionStore = sessionStore;

        LogoutCommand = ReactiveCommand.Create(() =>
        {
            Task.Run(() => _sessionStore.CloseSession().Wait());
        });
    }

    public ReactiveCommand<Unit, Unit> LogoutCommand { get; }
}
