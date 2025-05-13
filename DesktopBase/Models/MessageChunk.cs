namespace DesktopBase.Models;

public class MessageChunk
{
    public string Text { get; set; } = string.Empty;
    public bool IsThinking { get; set; }
    
    public MessageChunk(string text, bool isThinking = false)
    {
        Text = text;
        IsThinking = isThinking;
    }
}