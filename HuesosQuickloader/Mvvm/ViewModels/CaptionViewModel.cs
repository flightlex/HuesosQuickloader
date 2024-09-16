using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FontAwesome.Sharp;
using HuesosQuickloader.Mvvm.Views;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;

namespace HuesosQuickloader.Mvvm.ViewModels;

public sealed partial class CaptionViewModel : ObservableObject
{
    [ObservableProperty]
    private IconChar _currentResizeIcon = IconChar.SquarePlus;

    [RelayCommand]
    private void OnClose(Window window)
    {
        window.Close();
    }

    [RelayCommand]
    private void OnResize(Window window)
    {
        window.WindowState = window.WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
    }

    [RelayCommand]
    private void OnWindowStateChanged(Window window)
    {
        CurrentResizeIcon = window.WindowState == WindowState.Maximized ? IconChar.SquareMinus : IconChar.SquarePlus;
    }

    [RelayCommand]
    private void OnOpenSettings()
    {
        var settings = App.Current.ServiceProvider.GetRequiredService<SettingsWindow>();

        settings.Owner = App.Current.MainWindow;
        settings.ShowDialog();
    }
}
