using System;
using System.IO;
using System.Windows;
using System.Windows.Media;

namespace WPFUtilities
{
    public static class DeployHelper
    {
        static int delta = Properties.Settings.Default.DeployThemeColorDeltaContrast;
        public static string DefaultDeployTheme { get { return Properties.Settings.Default.DefaultDeployTheme; } }
        public static T GetBorderResources<T>(FrameworkElement control, string resourceName, string theme) where T : class
        {
            if (control == null)
                return null;

            var element = control.TryFindResource(resourceName);
            if (element == null)
            {
                ResourceDictionary dict = GetDeployResourceDictionary();
                control.Resources.MergedDictionaries.Add(dict);
                element = control.TryFindResource(resourceName);
                if (theme == "None")
                    theme = Properties.Settings.Default.DefaultDeployTheme;
                WPFUtilities.ThemeHelper.SetTheme(element as DependencyObject, theme);
            }

            return (T)element;
        }

        static ResourceDictionary GetDeployResourceDictionary()
        {
            Stream stream = Utilities.ExtractFileFromResource.Extract(System.Reflection.Assembly.GetExecutingAssembly(),
                                                      $"{typeof(DeployHelper).Namespace}.Resources.DeployResources.xaml");
            if (stream == null)
                return null;

            return System.Windows.Markup.XamlReader.Load(stream) as ResourceDictionary;
        }
        public static Color GetColorInContrast(Color color, string theme)
        {
            try
            {
                return Color.FromArgb(color.A, GetColorInContrast(color.R, theme), GetColorInContrast(color.G, theme), GetColorInContrast(color.B, theme));
            }
            catch (Exception)
            {
                return color;
            }
        }
        static byte GetColorInContrast(byte color, string theme)
        {
            try
            {
                if(ThemeHelper.IsLightWeightThemeBrush(theme))
                    return (byte)(color + delta);
                else
                    return (byte)(color - delta);
            }
            catch (Exception)
            {
                return color;
            }
        }
    }
}
