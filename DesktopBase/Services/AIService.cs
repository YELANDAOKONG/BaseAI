using DesktopBase.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DesktopBase.Services;

public interface IAIService
{
    Task<string> SendMessageAsync(string message, string? systemPrompt = null);
    AIModelSettings GetSettings();
    void UpdateSettings(AIModelSettings settings);
}

// 模拟实现，实际项目中需要连接真实的AI API
public class MockAIService : IAIService
{
    private AIModelSettings _settings = new();
    
    public async Task<string> SendMessageAsync(string message, string? systemPrompt = null)
    {
        // TODO...
        await Task.Delay(1000); // Demo
        return $"This is a mock AI response to: {message}";
    }
    
    public AIModelSettings GetSettings() => _settings;
    
    public void UpdateSettings(AIModelSettings settings)
    {
        _settings = settings;
    }
}