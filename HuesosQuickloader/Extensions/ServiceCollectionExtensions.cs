using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;

namespace HuesosQuickloader.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddView<TView, TViewModel>(this IServiceCollection serviceCollection)
        where TView : FrameworkElement, new()
        where TViewModel : ObservableObject
    {
        serviceCollection.AddSingleton<TView>(p => new() { DataContext = p.GetRequiredService<TViewModel>() });
        serviceCollection.AddSingleton<TViewModel>();
    }

    public static void AddTransientView<TView, TViewModel>(this IServiceCollection serviceCollection)
        where TView : FrameworkElement, new()
        where TViewModel : ObservableObject
    {
        serviceCollection.AddTransient<TView>(p => new() { DataContext = p.GetRequiredService<TViewModel>() });
        serviceCollection.AddTransient<TViewModel>();
    }
}
