using Notia.Desctop.Routing;
using ReactiveUI;
using System;

namespace Notia.Desctop.ViewModels;

public class MainViewModel : ViewModelBase
{
    private readonly AppRouter _router;
    private ViewModelBase _currentViewModel;
    private readonly IDisposable _routerChangePageSubscription;

    public MainViewModel(AppRouter router)
    {
        _router = router;
        _currentViewModel = router.CurrentPageViewModel;

        _routerChangePageSubscription =  router
            .WhenAnyValue(r=> r.CurrentPageViewModel)
            .Subscribe(newPageViewModel => 
            { 
                CurrentViewModel = newPageViewModel; 
            });
    }

    public ViewModelBase CurrentViewModel
    {
        get => _currentViewModel;
        private set => this.RaiseAndSetIfChanged(ref _currentViewModel, value);
    }

    public override void Dispose()
    {
        _routerChangePageSubscription.Dispose();
        base.Dispose();
    }
}
