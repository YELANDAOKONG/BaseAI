using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DesktopBase.Models;
using DesktopBase.Services;

namespace DesktopBase.ViewModels;

public partial class SettingsViewModel : ViewModelBase
{
    private readonly ThemeService _themeService;
    private readonly LocalizationService _localizationService;
    private readonly IAIService _aiService;
    
    [ObservableProperty]
    private ThemeType _selectedTheme;
    
    [ObservableProperty]
    private LanguageType _selectedLanguage;
    
    [ObservableProperty]
    private string _apiKey = string.Empty;
    
    [ObservableProperty]
    private string _modelType = "gpt-4";
    
    [ObservableProperty]
    private int _maxTokens = 2048;
    
    public string ThemeClass => _themeService.GetThemeClass();
    
    public SettingsViewModel(ThemeService themeService, LocalizationService localizationService, IAIService aiService)
    {
        _themeService = themeService;
        _localizationService = localizationService;
        _aiService = aiService;
        
        // Initialize with current settings
        _selectedTheme = _themeService.CurrentTheme;
        _selectedLanguage = _localizationService.CurrentCulture == "en" ? LanguageType.English : LanguageType.Chinese;
        
        var settings = _aiService.GetSettings();
        _apiKey = settings.ApiKey;
        _modelType = settings.ModelType;
        _maxTokens = settings.MaxTokens;
        
        _themeService.ThemeChanged += (s, e) => OnPropertyChanged(nameof(ThemeClass));
    }
    
    [RelayCommand]
    private void SaveSettings()
    {
        // Update theme
        _themeService.SetTheme(SelectedTheme);
        
        // Update language
        var culture = SelectedLanguage == LanguageType.English ? "en" : "zh-CN";
        _localizationService.SetCulture(culture);
        
        // Update AI settings
        var settings = _aiService.GetSettings();
        settings.ApiKey = ApiKey;
        settings.ModelType = ModelType;
        settings.MaxTokens = MaxTokens;
        _aiService.UpdateSettings(settings);
    }
    
    // 获取本地化字符串的辅助方法
    public string Loc(string key) => _localizationService.GetString(key);
}
