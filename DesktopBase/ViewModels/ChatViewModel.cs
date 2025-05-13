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
    
    [ObservableProperty]
    private bool _isStreamingResponse;
    
    [ObservableProperty] 
    private bool _showThinking;
    
    // I18n Text Properties
    [ObservableProperty] private string _modelSettingsText = "Model Settings";
    [ObservableProperty] private string _systemPromptLabelText = "System Prompt";
    [ObservableProperty] private string _systemPromptPlaceholderText = "You are a helpful assistant...";
    [ObservableProperty] private string _temperatureText = "Temperature";
    [ObservableProperty] private string _clearChatText = "Clear Chat";
    [ObservableProperty] private string _inputPlaceholderText = "Type your message here...";
    [ObservableProperty] private string _sendText = "Send";
    [ObservableProperty] private string _streamingModeText = "Streaming Mode";
    [ObservableProperty] private string _showThinkingText = "Show Thinking";
    
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
        
        _localizationService.PropertyChanged += (s, e) => 
        {
            if (e.PropertyName == nameof(LocalizationService.CurrentCulture))
            {
                UpdateLocalizedTexts();
            }
        };
        
        // Initial localization
        UpdateLocalizedTexts();
    }
    
    private void UpdateLocalizedTexts()
    {
        ModelSettingsText = _localizationService.GetString("Chat.ModelSettings");
        SystemPromptLabelText = _localizationService.GetString("Chat.SystemPromptLabel");
        SystemPromptPlaceholderText = _localizationService.GetString("Chat.SystemPromptPlaceholder");
        TemperatureText = _localizationService.GetString("Chat.Temperature");
        ClearChatText = _localizationService.GetString("Chat.ClearChat");
        InputPlaceholderText = _localizationService.GetString("Chat.InputPlaceholder");
        SendText = _localizationService.GetString("Chat.Send");
        StreamingModeText = _localizationService.GetString("Chat.StreamingMode");
        ShowThinkingText = _localizationService.GetString("Chat.ShowThinking");
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
            if (IsStreamingResponse)
            {
                // Add an AI message that will be updated with streaming content
                var aiMessage = new ChatMessage("", MessageType.AI);
                Messages.Add(aiMessage);
                
                // If show thinking is enabled, first add a thinking message
                ChatMessage? thinkingMessage = null;
                if (ShowThinking)
                {
                    thinkingMessage = new ChatMessage("Thinking...", MessageType.System);
                    thinkingMessage.IsThinking = true;
                    Messages.Add(thinkingMessage);
                }
                
                // Stream the response
                await foreach (var chunk in _aiService.StreamMessageAsync(userInput, SystemPrompt))
                {
                    // If this is a thinking chunk, update the thinking message
                    if (chunk.IsThinking && thinkingMessage != null)
                    {
                        thinkingMessage.Text = chunk.Text;
                    }
                    // Otherwise update the AI message
                    else if (!chunk.IsThinking)
                    {
                        aiMessage.Text += chunk.Text;
                    }
                }
                
                // If there was thinking but no content, add a placeholder
                if (thinkingMessage != null && string.IsNullOrEmpty(thinkingMessage.Text))
                {
                    thinkingMessage.Text = "(No thinking provided)";
                }
            }
            else
            {
                // Standard non-streaming response
                var response = await _aiService.SendMessageAsync(userInput, SystemPrompt);
                Messages.Add(new ChatMessage(response, MessageType.AI));
            }
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
    
    [RelayCommand]
    private void ToggleThinking()
    {
        ShowThinking = !ShowThinking;
    }
    
    // Update AI settings when they change
    public void UpdateSettings()
    {
        var settings = _aiService.GetSettings();
        settings.SystemPrompt = SystemPrompt;
        settings.Temperature = Temperature;
        _aiService.UpdateSettings(settings);
    }
}
