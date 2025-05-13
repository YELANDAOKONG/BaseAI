using DesktopBase.Models;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace DesktopBase.Services;

public class ThemeService : INotifyPropertyChanged
{
    private ThemeType _currentTheme = ThemeType.Light;
    
    public ThemeType CurrentTheme 
    { 
        get => _currentTheme;
        private set
        {
            if (_currentTheme != value)
            {
                _currentTheme = value;
                OnPropertyChanged();
                ThemeChanged?.Invoke(this, value);
            }
        }
    }
    
    public event EventHandler<ThemeType>? ThemeChanged;
    public event PropertyChangedEventHandler? PropertyChanged;
    
    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
    
    public void SetTheme(ThemeType theme)
    {
        CurrentTheme = theme;
    }
    
    public string GetThemeClass() => CurrentTheme.ToString().ToLowerInvariant();
}