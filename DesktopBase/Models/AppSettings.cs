namespace DesktopBase.Models;

public class AppSettings
{
    public ThemeType Theme { get; set; } = ThemeType.Light;
    public LanguageType Language { get; set; } = LanguageType.English;
    public AIModelSettings ModelSettings { get; set; } = new();
}