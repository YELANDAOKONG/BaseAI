using System;

namespace DesktopBase.Models;

public class ChatMessage
{
    public string Text { get; set; } = string.Empty;
    public MessageType Type { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.Now;
    public bool IsThinking { get; set; }
    public bool IsCollapsed { get; set; }
    
    public ChatMessage(string text, MessageType type)
    {
        Text = text;
        Type = type;
    }
}