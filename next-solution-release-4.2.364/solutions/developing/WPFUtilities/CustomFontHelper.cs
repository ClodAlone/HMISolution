using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Utilities.WPF;

namespace WPFUtilities
{
    public static class CustomFontHelper
    {
        public static FontFamily GetCustomFontFamily()
        {
            return Application.Current.MainWindow.FontFamily;
        }
        public static double GetCustomFontSize()
        {
            return Application.Current.MainWindow.FontSize;
        }
        public static bool CanApplyCustomFont()
        {
            return Application.Current != null && Application.Current.Dispatcher.CheckAccess() && Application.Current.MainWindow != null;
        }
    }
}
