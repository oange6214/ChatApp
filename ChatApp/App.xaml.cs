﻿using ChatApp.Data;
using ChatApp.Data.Interfaces;
using ChatApp.Data.Repositories;
using ChatApp.Services.Implementation;
using ChatApp.Services.Interfaces;
using ChatApp.ViewModels;
using ChatApp.ViewModels.Interfaces;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Windows;
using Toolkit.Mvvm.Messaging;
using Toolkit.Mvvm.Messaging.Interfaces;

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
        services.AddSingleton<IDbConnectionFactory>(sp =>
        new DbConnectionFactory(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=D:\Workspaces\Devolops\WPFs\ChatApp\ChatApp\Database\Database1.mdf;Integrated Security=True"));
        services.AddScoped<IChatRepository, ChatRepository>();
        services.AddScoped<IChatService, ChatService>();

        services.AddSingleton<IMainWindowViewModel, MainWindowViewModel>();
        services.AddSingleton(AutoMapperConfig.Initialize());

        services.AddSingleton<MainWindow>();
    }
}