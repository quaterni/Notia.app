using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Notia.Desctop.BackgroundWorkers;
using Notia.Desctop.Data;
using Notia.Desctop.Routing;
using Notia.Desctop.Services.Accounts.Abstractions;
using Notia.Desctop.Services.Accounts.Http;
using Notia.Desctop.Services.DateTime;
using Notia.Desctop.Services.DateTime.Abstractions;
using Notia.Desctop.Services.Sessions.Abstractions;
using Notia.Desctop.Services.Sessions.Local;
using Notia.Desctop.Stores;
using Notia.Desctop.ViewModels;
using Notia.Desctop.ViewModels.Pages;
using Notia.Desctop.Views;
using System;
using System.Diagnostics;
using System.IO;

namespace Notia.Desctop;

public partial class App : Application
{
    private IHost? _host;

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        var builder = CreateApplicaitonBuilder();
        _host = builder.Build();


        var sp = _host.Services;

        _host.Start();

        var router = sp.GetRequiredService<AppRouter>();
        var sessionStore = sp.GetRequiredService<SessionStore>();

        if (sessionStore.HasSession)
        {
            router.Navigate<TestingLoginPageViewModel>();

        }
        else
        {
            router.Navigate<LoginPageViewModel>();
        }

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow
            {
                DataContext = sp.GetRequiredService<MainViewModel>()
            };

            desktop.Exit += OnAppExit;

        }
        else if (ApplicationLifetime is ISingleViewApplicationLifetime singleViewPlatform)
        {
            singleViewPlatform.MainView = new MainView
            {
                DataContext = sp.GetRequiredService<MainViewModel>()
            };
        }

        base.OnFrameworkInitializationCompleted();
    }

    private void OnAppExit(object? sender, ControlledApplicationLifetimeExitEventArgs e)
    {
        _host?.StopAsync().Wait();
        _host?.Dispose();
    }

    private static HostApplicationBuilder CreateApplicaitonBuilder()
    {
        HostApplicationBuilder builder = Host.CreateApplicationBuilder();

        builder.Services.AddDbContext<ApplicationDbContext>(
            opt => opt.UseSqlite(builder.Configuration.GetConnectionString("Database")));

        builder.Services.AddSingleton<AppRouter>();
        builder.Services.AddSingleton<SessionStore>();
        builder.Services.AddScoped<ISessionService, LocalSessionService>();
        builder.Services.AddTransient<IDateTimeProvider, DateTimeProvider>();

        builder.Services.AddTransient<LoadingPageViewModel>();
        builder.Services.AddTransient<TestingLoginPageViewModel>();
        builder.Services.AddSingleton<MainViewModel>();

        builder.Services.AddHostedService<SessionObserver>();


        AddLoginPage(builder.Services, builder.Configuration);

        return builder;
    }



    private static void AddLoginPage(IServiceCollection services, IConfiguration configuration)
    {
        services.AddTransient<LoginPageViewModel>();
        services.AddHttpClient<HttpAccountService>();
        services.Configure<HttpAccountServiceOptions>(configuration.GetSection("HttpAccountService"));
        services.AddSingleton<IAccountService, HttpAccountService>();
    }

    private static void MigrateDatabase(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        Debug.WriteLine("Current Working Directory: " + Directory.GetCurrentDirectory());
        dbContext.Database.Migrate();
    }
}
