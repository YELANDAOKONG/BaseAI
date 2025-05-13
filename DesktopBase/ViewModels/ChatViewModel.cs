using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using DesktopBase.Models;
using DesktopBase.Services;
using System;

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
        
        _themeService.PropertyChanged += (s, e) => 
        {
            if (e.PropertyName == nameof(ThemeService.CurrentTheme))
            {
                OnPropertyChanged(nameof(ThemeClass));
            }
        };
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
        catch (Exception ex)
        {
            Messages.Add(new ChatMessage($"An error occurred: {ex.Message}", MessageType.System));
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
    
    public string GetLocalizedString(string key) => _localizationService.GetString(key);

    #region Localization

    public string ModelSettingsText => GetLocalizedString("Chat.ModelSettings");
    public string SystemPromptLabelText => GetLocalizedString("Chat.SystemPromptLabel");
    public string SystemPromptPlaceholderText => GetLocalizedString("Chat.SystemPromptPlaceholder");
    public string TemperatureText => GetLocalizedString("Chat.Temperature");
    public string ClearChatText => GetLocalizedString("Chat.ClearChat");
    public string InputPlaceholderText => GetLocalizedString("Chat.InputPlaceholder");
    public string SendText => GetLocalizedString("Chat.Send");

    #endregion
    
}
