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
        
        // Register services
        services.AddSingleton<ThemeService>();
        services.AddSingleton<LocalizationService>();
        services.AddSingleton<IAIService, MockAIService>();
        
        // Register view models
        services.AddTransient<MainWindowViewModel>();
        
        _serviceProvider = services.BuildServiceProvider();
    }
    
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            // Initialize localization service
            var localizationService = _serviceProvider.GetRequiredService<LocalizationService>();
            
            // Create main view model and window
            var mainViewModel = _serviceProvider.GetRequiredService<MainWindowViewModel>();
            var mainWindow = new MainWindow { DataContext = mainViewModel };
            desktop.MainWindow = mainWindow;
            
            // Start initialization in background
            Task.Run(async () =>
            {
                try
                {
                    await localizationService.InitializeAsync();
                    // Dispatch UI updates back to UI thread
                    await Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(() =>
                    {
                        mainViewModel.Title = localizationService.GetString("App.Title");
                    });
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error initializing application: {ex.Message}");
                    // TODO: Add proper error logging
                }
            });
        }

        base.OnFrameworkInitializationCompleted();
    }
}
