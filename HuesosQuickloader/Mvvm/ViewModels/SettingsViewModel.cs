using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HuesosQuickloader.Models;
using HuesosQuickloader.Services.Config;
using System.Windows;

namespace HuesosQuickloader.Mvvm.ViewModels;

public sealed partial class SettingsViewModel : ObservableObject
{
    private readonly IConfigService _configService;

    public SettingsViewModel(IConfigService configService)
    {
        _configService = configService;

        CurrentConfig = _configService.Read();
    }

    public AppConfig CurrentConfig { get; private set; }

    [RelayCommand]
    private void OnSaveAndExit(Window window)
    {
        _configService.Write(CurrentConfig);
        window.Close();
    }

    [RelayCommand]
    private void OnCancel(Window window)
    {
        window.Close();
    }
}
