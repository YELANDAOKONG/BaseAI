using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using DesktopBase.Models;
using DesktopBase.Services;

namespace DesktopBase.ViewModels;

public partial class ChatViewModel : ViewModelBase
{
    private readonly IAIService _aiService;
    private readonly LocalizationService _localizationService;
    private readonly ThemeService _themeService;
    
    [ObservableProperty]
    private string _inputText = string.Empty;
    
    [ObservableProperty]
    private string _systemPrompt = string.Empty;
    
    [ObservableProperty]
    private double _temperature = 0.7;
    
    [ObservableProperty]
    private bool _isLoading;
    
    public ObservableCollection<ChatMessage> Messages { get; } = new();
    
    public string ThemeClass => _themeService.GetThemeClass();
    
    public ChatViewModel(IAIService aiService, LocalizationService localizationService, ThemeService themeService)
    {
        _aiService = aiService;
        _localizationService = localizationService;
        _themeService = themeService;
        
        // Load settings
        var settings = _aiService.GetSettings();
        SystemPrompt = settings.SystemPrompt;
        Temperature = settings.Temperature;
        
        _themeService.ThemeChanged += (s, e) => OnPropertyChanged(nameof(ThemeClass));
    }
    
    [RelayCommand]
    private async Task SendMessageAsync()
    {
        if (string.IsNullOrWhiteSpace(InputText))
            return;
            
        var userMessage = new ChatMessage(InputText, MessageType.User);
        Messages.Add(userMessage);
        
        string userInput = InputText;
        InputText = string.Empty;
        
        IsLoading = true;
        
        try
        {
            var response = await _aiService.SendMessageAsync(userInput, SystemPrompt);
            Messages.Add(new ChatMessage(response, MessageType.AI));
        }
        finally
        {
            IsLoading = false;
        }
    }
    
    [RelayCommand]
    private void ClearChat()
    {
        Messages.Clear();
    }
    
    // Update AI settings when they change
    public void UpdateSettings()
    {
        var settings = _aiService.GetSettings();
        settings.SystemPrompt = SystemPrompt;
        settings.Temperature = Temperature;
        _aiService.UpdateSettings(settings);
    }
    
    public string Loc(string key) => _localizationService.GetString(key);
}
