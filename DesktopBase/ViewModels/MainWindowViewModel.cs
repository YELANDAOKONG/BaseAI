using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DesktopBase.Services;
using DesktopBase.Models;
using System;

namespace DesktopBase.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    private readonly ThemeService _themeService;
    private readonly LocalizationService _localizationService;
    
    [ObservableProperty]
    private ViewModelBase _currentPage;
    
    [ObservableProperty]
    private string _title = "AI Assistant";
    
    public string ChatTabText => _localizationService.GetString("App.Chat");
    public string SettingsTabText => _localizationService.GetString("App.Settings");
    
    public ChatViewModel ChatViewModel { get; }
    public SettingsViewModel SettingsViewModel { get; }
    
    public MainWindowViewModel(ThemeService themeService, LocalizationService localizationService, IAIService aiService)
    {
        _themeService = themeService;
        _localizationService = localizationService;
        
        ChatViewModel = new ChatViewModel(aiService, localizationService, themeService);
        SettingsViewModel = new SettingsViewModel(themeService, localizationService, aiService);
        
        // Default to chat page
        _currentPage = ChatViewModel;
        
        // Update the title and tab text when language changes
        _localizationService.PropertyChanged += (s, e) => 
        {
            if (e.PropertyName == nameof(LocalizationService.CurrentCulture))
            {
                Title = _localizationService.GetString("App.Title");
                OnPropertyChanged(nameof(ChatTabText));
                OnPropertyChanged(nameof(SettingsTabText));
            }
        };
    }
    
    [RelayCommand]
    private void OpenChat()
    {
        CurrentPage = ChatViewModel;
    }
    
    [RelayCommand]
    private void OpenSettings()
    {
        CurrentPage = SettingsViewModel;
    }
}