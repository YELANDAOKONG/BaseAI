using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Threading.Tasks;

namespace DesktopBase.Services;

public class LocalizationService : INotifyPropertyChanged
{
    private readonly Dictionary<string, Dictionary<string, object>> _resources = new();
    private string _currentCulture = "en";
    
    public string CurrentCulture
    {
        get => _currentCulture;
        private set
        {
            if (_currentCulture != value)
            {
                _currentCulture = value;
                OnPropertyChanged();
            }
        }
    }
    
    public event PropertyChangedEventHandler? PropertyChanged;
    
    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
    
    public async Task InitializeAsync()
    {
        try
        {
            // Create default resources in memory
            _resources["en"] = JsonSerializer.Deserialize<Dictionary<string, object>>(DefaultResources.English) ?? new();
            _resources["zh-CN"] = JsonSerializer.Deserialize<Dictionary<string, object>>(DefaultResources.Chinese) ?? new();
            
            // Try to load from files if they exist
            var localizationDir = Path.Combine(AppContext.BaseDirectory, "Assets", "Localization");
            
            if (Directory.Exists(localizationDir))
            {
                var enFile = Path.Combine(localizationDir, "en.json");
                var zhFile = Path.Combine(localizationDir, "zh-CN.json");
                
                if (File.Exists(enFile))
                {
                    var enJson = await File.ReadAllTextAsync(enFile);
                    _resources["en"] = JsonSerializer.Deserialize<Dictionary<string, object>>(enJson) ?? _resources["en"];
                }
                
                if (File.Exists(zhFile))
                {
                    var zhJson = await File.ReadAllTextAsync(zhFile);
                    _resources["zh-CN"] = JsonSerializer.Deserialize<Dictionary<string, object>>(zhJson) ?? _resources["zh-CN"];
                }
            }
        }
        catch (Exception ex)
        {
            // Log exception but don't crash
            Console.WriteLine($"Error loading localization resources: {ex.Message}");
            // Resources already loaded from embedded defaults
        }
    }
    
    public void SetCulture(string culture)
    {
        if (_resources.ContainsKey(culture))
        {
            CurrentCulture = culture;
            CultureInfo.CurrentUICulture = new CultureInfo(culture);
        }
    }
    
    public string GetString(string key)
    {
        try
        {
            var parts = key.Split('.');
            if (parts.Length != 2) return key;
            
            var section = parts[0];
            var name = parts[1];
            
            if (_resources.TryGetValue(_currentCulture, out var resource) &&
                resource.TryGetValue(section, out var sectionObj))
            {
                var sectionDict = JsonSerializer.Deserialize<Dictionary<string, string>>(
                    JsonSerializer.Serialize(sectionObj));
                    
                if (sectionDict != null && sectionDict.TryGetValue(name, out var value))
                {
                    return value;
                }
            }
            
            return key;
        }
        catch
        {
            return key;
        }
    }
}

// Default resources embedded in code to ensure the app always works
internal static class DefaultResources
{
    public static string English => @"{
      ""App"": {
        ""Title"": ""AI Assistant"",
        ""Settings"": ""Settings"",
        ""Chat"": ""Chat""
      },
      ""Chat"": {
        ""InputPlaceholder"": ""Type your message here..."",
        ""Send"": ""Send"",
        ""SystemPromptLabel"": ""System Prompt"",
        ""SystemPromptPlaceholder"": ""You are a helpful assistant..."",
        ""ModelSettings"": ""Model Settings"",
        ""Temperature"": ""Temperature"",
        ""ClearChat"": ""Clear Chat"",
        ""UserMessage"": ""You"",
        ""AIMessage"": ""AI"",
        ""SystemMessage"": ""System"",
        ""StreamingMode"": ""Streaming Mode"",
        ""ShowThinking"": ""Show Thinking"",
        ""ThinkingMessage"": ""Thinking...""
      },
      ""Settings"": {
        ""Language"": ""Language"",
        ""English"": ""English"",
        ""Chinese"": ""Chinese"",
        ""Theme"": ""Theme"",
        ""Light"": ""Light"",
        ""Dark"": ""Dark"",
        ""APISettings"": ""API Settings"",
        ""ApiKey"": ""API Key"",
        ""ApiKeyPlaceholder"": ""Enter your API key"",
        ""ModelType"": ""Model Type"",
        ""MaxTokens"": ""Max Tokens"",
        ""Save"": ""Save""
      }
    }";

    public static string Chinese => @"{
      ""App"": {
        ""Title"": ""AI 助手"",
        ""Settings"": ""设置"",
        ""Chat"": ""对话""
      },
      ""Chat"": {
        ""InputPlaceholder"": ""在此输入您的消息..."",
        ""Send"": ""发送"",
        ""SystemPromptLabel"": ""系统提示词"",
        ""SystemPromptPlaceholder"": ""你是一个有帮助的助手..."",
        ""ModelSettings"": ""模型设置"",
        ""Temperature"": ""温度"",
        ""ClearChat"": ""清空对话"",
        ""UserMessage"": ""你"",
        ""AIMessage"": ""AI"",
        ""SystemMessage"": ""系统"",
        ""StreamingMode"": ""流式输出"",
        ""ShowThinking"": ""显示思考过程"",
        ""ThinkingMessage"": ""思考中...""
      },
      ""Settings"": {
        ""Language"": ""语言"",
        ""English"": ""英文"",
        ""Chinese"": ""中文"",
        ""Theme"": ""主题"",
        ""Light"": ""明亮"",
        ""Dark"": ""暗黑"",
        ""APISettings"": ""API 设置"",
        ""ApiKey"": ""API 密钥"",
        ""ApiKeyPlaceholder"": ""输入您的 API 密钥"",
        ""ModelType"": ""模型类型"",
        ""MaxTokens"": ""最大令牌数"",
        ""Save"": ""保存""
      }
    }";
}
