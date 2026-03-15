using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ScreenDependencies
{
    public static class SkinStorage
    {
        public static readonly DependencyProperty OverrideVisualStyleProperty = DependencyProperty.RegisterAttached(
           "OverrideVisualStyle",
           typeof(bool),
           typeof(SkinStorage),
           new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.None));

        public static bool GetOverrideVisualStyle(DependencyObject obj)
        {
            return (bool)obj.GetValue(OverrideVisualStyleProperty);
        }

        public static void SetOverrideVisualStyle(DependencyObject obj, bool value)
        {
            obj.SetValue(OverrideVisualStyleProperty, value);
        }
    }
}
