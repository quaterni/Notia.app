
using Microsoft.Extensions.DependencyInjection;
using Notia.Desctop.Exceptions;
using Notia.Desctop.ViewModels.Pages;
using ReactiveUI;
using System;

namespace Notia.Desctop.Routing;

public class AppRouter : ReactiveObject
{
    private readonly IServiceProvider _serviceProvider;

    private PageViewModel _currentPageViewModel;

    public AppRouter(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        _currentPageViewModel = _serviceProvider.GetRequiredService<LoadingPageViewModel>();
    }

    public PageViewModel CurrentPageViewModel
    {
        get => _currentPageViewModel;
        set => this.RaiseAndSetIfChanged(ref _currentPageViewModel, value);
    }

    public void Navigate<TPage>() where TPage : PageViewModel
    {
        try
        {
            CurrentPageViewModel = _serviceProvider.GetRequiredService<TPage>();
        }
        catch 
        {
            throw new NotRegisteredPage(typeof(TPage));
        }
    }

    public void OnLoading()
    {
        Navigate<LoadingPageViewModel>();
    }
}
