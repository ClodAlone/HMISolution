using System.Windows;
using System.Windows.Media;

namespace WpfApp2
{
    public enum Theme
    {
        Light,
        Dark
    }

    public class ThemeManager
    {
        private static ThemeManager? _instance;
        public static ThemeManager Instance => _instance ??= new ThemeManager();

        private Theme _currentTheme = Theme.Dark;

        public Theme CurrentTheme
        {
            get => _currentTheme;
            set => SetTheme(value);
        }

        public void SetTheme(Theme theme)
        {
            _currentTheme = theme;

            if (theme == Theme.Dark)
            {
                SetBrush("BackgroundColor", Color.FromRgb(45, 45, 48));
                SetBrush("ForegroundColor", Color.FromRgb(230, 230, 230));
                SetBrush("AccentColor", Color.FromRgb(0, 122, 204));
                SetBrush("PanelBackgroundColor", Color.FromRgb(37, 37, 38));
                SetBrush("PanelBorderColor", Color.FromRgb(51, 51, 55));
            }
            else
            {
                SetBrush("BackgroundColor", Color.FromRgb(255, 255, 255));
                SetBrush("ForegroundColor", Color.FromRgb(51, 51, 51));
                SetBrush("AccentColor", Color.FromRgb(0, 120, 215));
                SetBrush("PanelBackgroundColor", Color.FromRgb(245, 245, 245));
                SetBrush("PanelBorderColor", Color.FromRgb(204, 204, 204));
            }
        }

        private void SetBrush(string resourceKey, Color color)
        {
            // Create a new brush instead of modifying existing one (which is frozen)
            var newBrush = new SolidColorBrush(color);
            newBrush.Freeze(); // Freeze for performance
            Application.Current.Resources[resourceKey] = newBrush;
        }
    }
}
