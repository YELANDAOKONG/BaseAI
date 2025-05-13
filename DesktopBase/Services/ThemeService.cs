using DesktopBase.Models;
using System;

namespace DesktopBase.Services;

public class ThemeService
{
    public ThemeType CurrentTheme { get; private set; } = ThemeType.Light;
    
    public event EventHandler<ThemeType>? ThemeChanged;
    
    public void SetTheme(ThemeType theme)
    {
        CurrentTheme = theme;
        ThemeChanged?.Invoke(this, theme);
    }
    
    public string GetThemeClass() => CurrentTheme.ToString().ToLowerInvariant();
}