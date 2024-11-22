
using Microsoft.Extensions.DependencyInjection;
using Notia.Desctop.Exceptions;
using Notia.Desctop.ViewModels.Pages;
using ReactiveUI;
using System;

namespace Notia.Desctop.Routing;

public class AppRouter : ReactiveObject
{
    private readonly IServiceProvider _serviceProvider;

    public AppRouter(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
       CurrentPage = _serviceProvider.GetRequiredService<LoadingPageViewModel>();
    }

    public PageViewModel CurrentPage { get; private set; }

    public void Navigate<TPage>() where TPage : PageViewModel
    {
        try
        {
            CurrentPage = _serviceProvider.GetRequiredService<TPage>();
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
