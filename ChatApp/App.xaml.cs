using ChatApp.ViewModels;
using ChatApp.ViewModels.Interfaces;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Windows;
using Toolkit.Wpf.Mvvm.Messaging;
using Toolkit.Wpf.Mvvm.Messaging.Interfaces;

namespace ChatApp;

public partial class App : Application
{
    private IHost _host;

    public App()
    {
        _host = Host.CreateDefaultBuilder()
            .ConfigureServices((context, services) =>
            {
                ConfigureServices(services);
            })
            .Build();
    }

    protected override async void OnStartup(StartupEventArgs e)
    {
        await _host.StartAsync();

        var mainWindow = _host.Services.GetRequiredService<MainWindow>();
        mainWindow.DataContext = _host.Services.GetRequiredService<IMainWindowViewModel>();
        mainWindow.Show();

        base.OnStartup(e);
    }

    protected override async void OnExit(ExitEventArgs e)
    {
        using (_host)
        {
            await _host.StopAsync();
        }

        base.OnExit(e);
    }

    private void ConfigureServices(IServiceCollection services)
    {
        services.AddSingleton<IEventAggregator, EventAggregator>();

        services.AddSingleton<IStatusThumbsViewModel, StatusThumbsViewModel>();
        services.AddSingleton<IChatListViewModel, ChatListViewModel>();
        services.AddSingleton<IConversationViewModel, ConversationViewModel>();
        services.AddSingleton<IMainWindowViewModel, MainWindowViewModel>();

        services.AddSingleton<MainWindow>();
    }
}