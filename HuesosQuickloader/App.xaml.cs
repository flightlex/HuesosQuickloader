using HuesosQuickloader.Extensions;
using HuesosQuickloader.Mvvm.ViewModels;
using HuesosQuickloader.Mvvm.Views;
using HuesosQuickloader.Services.Config;
using HuesosQuickloader.Services.Dialog;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Windows;

namespace HuesosQuickloader;

public sealed partial class App : Application
{
    public static new App Current => (App)Application.Current;

    public IServiceProvider ServiceProvider { get; private set; } = null!;

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        ConfigureServices();

        var config = ServiceProvider.GetRequiredService<IConfigService>();
        config.Initialize();

        MainWindow = ServiceProvider.GetRequiredService<MainWindow>();
        MainWindow.Show();
    }

    private void ConfigureServices()
    {
        IServiceCollection serviceCollection = new ServiceCollection();

        serviceCollection.AddView<MainWindow, MainViewModel>();
        serviceCollection.AddTransientView<SettingsWindow, SettingsViewModel>();

        serviceCollection.AddSingleton<CaptionViewModel>();

        serviceCollection.AddSingleton<IDialogService, DialogService>();
        serviceCollection.AddSingleton<IConfigService, ConfigService>();

        ServiceProvider = serviceCollection.BuildServiceProvider();
    }
}
