using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using DesktopBase.ViewModels;
using DesktopBase.Views;
using DesktopBase.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace DesktopBase;

public partial class App : Application
{
    private ServiceProvider _serviceProvider;

    public App()
    {
        var services = new ServiceCollection();
        
        services.AddSingleton<ThemeService>();
        services.AddSingleton<LocalizationService>();
        services.AddSingleton<IAIService, MockAIService>();
        
        services.AddTransient<MainWindowViewModel>();
        
        _serviceProvider = services.BuildServiceProvider();
    }
    
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override async void OnFrameworkInitializationCompleted()
    {
        var localizationService = _serviceProvider.GetRequiredService<LocalizationService>();
        await localizationService.InitializeAsync();
        
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var mainViewModel = _serviceProvider.GetRequiredService<MainWindowViewModel>();
            desktop.MainWindow = new MainWindow
            {
                DataContext = mainViewModel
            };
        }

        base.OnFrameworkInitializationCompleted();
    }
}