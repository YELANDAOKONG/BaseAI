namespace DesktopBase.Models;

public class AIModelSettings
{
    public string ApiKey { get; set; } = string.Empty;
    public string ModelType { get; set; } = "gpt-4";
    public string SystemPrompt { get; set; } = "You are a helpful assistant.";
    public double Temperature { get; set; } = 0.7;
    public int MaxTokens { get; set; } = 2048;
}