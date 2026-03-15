    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
    using System.Windows.Media;
    using System.Globalization;

namespace ScreenManager
{
    /// <summary>
    /// Used in MyUserControl.zaml to converts a scale value to a percentage.
    /// It is used to display the 50%, 100%, etc that appears underneath the zoom and pan control.
    /// </summary>
    public class BarItemMinWidthConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            double defDim = (double)Properties.Resources.InnerParameterTextMenu.Length;
            string candidate = Properties.Resources.InnerParameterTextMenu;
            if (Properties.Resources.InnerScreenTextMenu.Length > defDim)
            {
                defDim = (double)Properties.Resources.InnerScreenTextMenu.Length;
                candidate = Properties.Resources.InnerScreenTextMenu;
            }
            try
            {
                UserControl control = values[0] as UserControl;
                var formattedText = new FormattedText(candidate,CultureInfo.CurrentCulture,FlowDirection.LeftToRight,new Typeface(control.FontFamily, control.FontStyle,
                                                      control.FontWeight, control.FontStretch),control.FontSize,Brushes.Black);
                defDim = formattedText.Width + Properties.Resources.InnerParameterTextMenu.Length;
                return defDim;
            }
            catch (Exception ex)
            {
                return System.Math.Max(defDim,100);
            }
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return new object[] { value };
        }
    }
}

