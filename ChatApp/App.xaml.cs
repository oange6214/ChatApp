using ChatApp.Core.Interfaces;
using ChatApp.Core.Services;
using ChatApp.Infrastructure.Data;
using ChatApp.Infrastructure.Data.Repositories;
using ChatApp.Infrastructure.Mapping;
using ChatApp.ViewModels;
using ChatApp.ViewModels.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using System.IO;
using System.Windows;

namespace ChatApp;

public partial class App : Application
{
    private IHost _host;

    public App()
    {
        _host = CreateHostBuilder().Build();
    }

    public static IHostBuilder CreateHostBuilder(string[] args = null) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration((hostingContext, config) =>
            {
                config.SetBasePath(AppDomain.CurrentDomain.BaseDirectory);
                config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
                config.AddJsonFile($"appsettings.{hostingContext.HostingEnvironment.EnvironmentName}.json", optional: true, reloadOnChange: true);
                config.AddEnvironmentVariables();
            })
            .UseSerilog((context, services, loggerConfiguration) =>
            {
                loggerConfiguration
                    .ReadFrom.Configuration(context.Configuration)
                    .Enrich.FromLogContext()
                    .Enrich.WithEnvironmentName()
                    .Enrich.WithMachineName()
                    .WriteTo.File(
                        Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs", "chatapp-.log"),
                        rollingInterval: RollingInterval.Day,
                        retainedFileCountLimit: 31)
                    .WriteTo.Debug();

                if (context.HostingEnvironment.IsDevelopment())
                {
                    loggerConfiguration.WriteTo.Console();
                }
            })
            .ConfigureServices((context, services) =>
            {
                ConfigureServices(services, context.Configuration);
            });

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
            Log.CloseAndFlush();
        }

        base.OnExit(e);
    }

    private static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<IDbConnectionFactory>(sp =>
            new DbConnectionFactory(configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped<IChatRepository, ChatRepository>();
        services.AddScoped<IChatService, ChatService>();
        services.AddSingleton<IMainWindowViewModel, MainWindowViewModel>();
        services.AddSingleton(AutoMapperConfig.Initialize());

        services.AddSingleton<MainWindow>();
    }
}