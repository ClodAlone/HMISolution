using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Utilities;
using Utilities.WPF;
using System.ComponentModel;
using Mindscape.WpfElements.WpfPropertyGrid;
using System.Collections.Specialized;
using System.Globalization;
using Mindscape.WpfElements.PropertyEditing;
using System.Reflection;
using DevExpress.Xpf.Editors;

namespace WPFUtilities.Converters
{
    public class BrushToNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return Properties.Resources.DefaultBrushLabel;

            var ret = Properties.Resources.NamedBrushLabel;
            bool bCustomRet = value is LinearGradientBrush || value is RadialGradientBrush || value is DrawingBrush || value is VisualBrush;
            if (value is VisualBrush)
            {
                try
                {
                    ret = System.IO.Path.GetFileName((((VisualBrush)value).Visual as Image).Source.ToString());
                }
                catch (Exception) { }
            }
            else if (value is SolidColorBrush)
            {
                var color = (value as SolidColorBrush).Color;
                if (color == ColorEdit.EmptyColor)
                    return Properties.Resources.DefaultBrushLabel;
                var colorName = color.GetColorName();
                if (colorName != null)
                    return colorName;
            }
            else if (value is Color)
            {
                var color = (Color)value;
                if (color == ColorEdit.EmptyColor)
                    return Properties.Resources.DefaultBrushLabel;
                var colorName = color.GetColorName();
                if (colorName != null)
                    return colorName;
            }

            if (bCustomRet)
                return ret;
            else
                return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    static class ColorHelpers
    {

        public static string GetColorName(this Color color)
        {
            return _knownColors
                .Where(kvp => kvp.Value.Equals(color))
                .Select(kvp => kvp.Key)
                .FirstOrDefault();
        }

        static readonly Dictionary<string, Color> _knownColors = GetKnownColors();

        static Dictionary<string, Color> GetKnownColors()
        {
            var colorProperties = typeof(Colors).GetProperties(BindingFlags.Static | BindingFlags.Public);
            return colorProperties
                .ToDictionary(
                    p => p.Name,
                    p => (Color)p.GetValue(null, null));
        }
    }
}
