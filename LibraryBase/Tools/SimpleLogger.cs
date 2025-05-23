﻿using System.Text;
using LibraryBase.Tools.Interfaces;
using Spectre.Console;

namespace LibraryBase.Tools;

public class SimpleLogger : ILogger
{

    public string Title = "APP";
    public bool Colorful = true;
    public readonly object ConsoleLock = new();
    
    public SimpleLogger() { }
    
    public SimpleLogger(string title, bool colorful = true)
    {
        Title = title;
        Colorful = colorful;
    }

    public void Log(string level, string[] messages)
    {
        if (Colorful)
        {
            LogColorful(level, messages, null);
        }
        else
        {
            LogNormal(level, messages, null);
        }
    }
    
    public void LogAuto(string level, string[] messages, string? levelColor = null, string? messageColor = null)
    {
        if (Colorful)
        {
            LogColorful(level, messages, levelColor, messageColor);
        }
        else
        {
            LogNormal(level, messages, levelColor, messageColor);
        }
    }

    
    private void LogNormal(string level, string[] messages, string? levelColor = null, string? messageColor = null)
    {
        StringBuilder builder = new StringBuilder();
        // Time
        builder.Append($"[{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}]");
        builder.Append(' ');
        // Level
        builder.Append($"[{level}]");
        builder.Append(' ');
        // Title
        builder.Append($"({Title})");
        builder.Append(' ');
        // Messages
        foreach (string message in messages)
        {
            builder.Append($"{message}");
            builder.Append(' ');
        }

        lock (ConsoleLock)
        {
            Console.WriteLine(builder.ToString());
        }
    }
    
    private void LogColorful(string level, string[] messages, string? levelColor = null, string? messageColor = null)
    {
        StringBuilder builder = new StringBuilder();
        // Time
        builder.Append($"[grey][[{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}]][/]");
        builder.Append(' ');
        // Level
        builder.Append($"[{levelColor}][[{Markup.Escape(level)}]][/]");
        builder.Append(' ');
        // Title
        builder.Append($"[{messageColor}]({Markup.Escape(Title)})[/]");
        builder.Append(' ');
        // Messages
        foreach (string message in messages)
        {
            builder.Append($"[{messageColor}]{Markup.Escape(message)}[/]");
            builder.Append(' ');
        }

        lock (ConsoleLock)
        {
            AnsiConsole.MarkupLine(builder.ToString());
        }
    }

    public void All(params string[] messages)
    {
        LogAuto("A", messages, "grey", "grey");
    }
    
    public void Trace(params string[] messages)
    {
        LogAuto("T", messages, "purple", "purple");
    }

    public void Debug(params string[] messages)
    {
        LogAuto("D", messages, "blue", "blue");
    }

    public void Info(params string[] messages)
    {
        LogAuto("I", messages, "white", "white");
    }

    public void Warn(params string[] messages)
    {
        LogAuto("W", messages, "yellow", "yellow");
    }

    public void Error(params string[] messages)
    {
        LogAuto("E", messages, "red", "red");
    }

    public void Fatal(params string[] messages)
    {
        LogAuto("F", messages, "red", "red");
    }

    public void Off(params string[] messages)
    {
        return;
    }
}