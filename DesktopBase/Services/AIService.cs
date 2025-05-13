using DesktopBase.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DesktopBase.Services;

public interface IAIService
{
    Task<string> SendMessageAsync(string message, string? systemPrompt = null);
    IAsyncEnumerable<MessageChunk> StreamMessageAsync(string message, string? systemPrompt = null);
    AIModelSettings GetSettings();
    void UpdateSettings(AIModelSettings settings);
}

public class MockAIService : IAIService
{
    private AIModelSettings _settings = new();
    
    public async Task<string> SendMessageAsync(string message, string? systemPrompt = null)
    {
        // TODO...
        await Task.Delay(1000); // Demo delay
        return $"This is a mock AI response to: {message}";
    }
    
    public async IAsyncEnumerable<MessageChunk> StreamMessageAsync(string message, string? systemPrompt = null)
    {
        // TODO...
        yield return new MessageChunk("Let me think about this...", true);
        await Task.Delay(500);
        yield return new MessageChunk(" Analyzing the query...", true);
        await Task.Delay(500);
        yield return new MessageChunk(" Considering possible responses...", true);
        await Task.Delay(500);
        
        string response = $"This is a mock AI response to: {message}";
        
        for (int i = 0; i < response.Length; i++)
        {
            await Task.Delay(50); // Demo delay
            yield return new MessageChunk(response[i].ToString());
        }
    }
    
    public AIModelSettings GetSettings() => _settings;
    
    public void UpdateSettings(AIModelSettings settings)
    {
        _settings = settings;
    }
}