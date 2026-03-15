using DevExpress.Xpf.Bars.Themes;
using System.Windows;
#if !WINDOWS_UWP
using System.Windows.Controls;
using System.Windows.Data;
#endif

namespace WPFUtilities
{
    public static class ToolBarControlHelper
    {
#if !WINDOWS_UWP
        static BarControlThemeKeyExtension toolbarStyleKey;
        public static BarControlThemeKeyExtension GetToolBarStyleKey()
        {
            if(toolbarStyleKey == null)
                toolbarStyleKey = new BarControlThemeKeyExtension() { IsThemeIndependent = true, ResourceKey = BarControlThemeKeys.BarContentStyle };
            return toolbarStyleKey;
        }
        public static Style GetToolBarStyle()
        {
            var style = new Style(typeof(ContentControl));
            style.Setters.Add(new Setter(Control.FontFamilyProperty, new Binding("FontFamily") { RelativeSource = new RelativeSource(RelativeSourceMode.FindAncestor, typeof(UserControl), 1) }));
            style.Setters.Add(new Setter(Control.FontSizeProperty, new Binding("FontSize") { RelativeSource = new RelativeSource(RelativeSourceMode.FindAncestor, typeof(UserControl), 1) }));
            style.Setters.Add(new Setter(Control.FontWeightProperty, new Binding("FontWeight") { RelativeSource = new RelativeSource(RelativeSourceMode.FindAncestor, typeof(UserControl), 1) }));
            style.Setters.Add(new Setter(Control.FontStyleProperty, new Binding("FontStyle") { RelativeSource = new RelativeSource(RelativeSourceMode.FindAncestor, typeof(UserControl), 1) }));
            return style;
        }

        public static void AddToolBarStyleResource(this FrameworkElement obj)
        {
            var key = ToolBarControlHelper.GetToolBarStyleKey();
            if (!obj.Resources.Contains(key))
                obj.Resources.Add(key,
                ToolBarControlHelper.GetToolBarStyle());
        }
#endif
    }
}
