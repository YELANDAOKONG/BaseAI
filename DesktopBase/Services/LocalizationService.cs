using System;
using System.Globalization;
using System.IO;
using System.Text.Json;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace DesktopBase.Services;

public class LocalizationService : INotifyPropertyChanged
{
    private Dictionary<string, Dictionary<string, object>> _resources = new();
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
            Directory.CreateDirectory("Assets/Localization");
            
            if (!File.Exists("Assets/Localization/en.json"))
            {
                var defaultEnglish = CreateDefaultEnglishResource();
                File.WriteAllText("Assets/Localization/en.json", defaultEnglish);
            }
            
            if (!File.Exists("Assets/Localization/zh-CN.json"))
            {
                var defaultChinese = CreateDefaultChineseResource();
                File.WriteAllText("Assets/Localization/zh-CN.json", defaultChinese);
            }
            
            var enJson = await File.ReadAllTextAsync("Assets/Localization/en.json");
            _resources["en"] = JsonSerializer.Deserialize<Dictionary<string, object>>(enJson) ?? new();
            
            var zhJson = await File.ReadAllTextAsync("Assets/Localization/zh-CN.json");
            _resources["zh-CN"] = JsonSerializer.Deserialize<Dictionary<string, object>>(zhJson) ?? new();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading localization files: {ex.Message}");
            _resources["en"] = JsonSerializer.Deserialize<Dictionary<string, object>>(CreateDefaultEnglishResource()) ?? new();
            _resources["zh-CN"] = JsonSerializer.Deserialize<Dictionary<string, object>>(CreateDefaultChineseResource()) ?? new();
        }
    }
    
    private string CreateDefaultEnglishResource()
    {
        return @"{
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
            ""AIMessage"": ""AI""
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
    }
    
    private string CreateDefaultChineseResource()
    {
        return @"{
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
            ""AIMessage"": ""AI""
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
