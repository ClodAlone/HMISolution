using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using Opc.Ua;
using System.Windows;
using Sliders.Enums;

namespace Sliders.Converters
{
    public class ThumbStyleValueConverter : IMultiValueConverter
    {
        #region IMultiValueConverter Members
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            try
            {
                if (values[0] == DependencyProperty.UnsetValue || values[1] == DependencyProperty.UnsetValue)
                    return null;

                SliderStyleEnum sliderStyle = (SliderStyleEnum)values[0];
                SliderControl slider = values[1] as SliderControl;
                var style = slider.TryFindResource($"{sliderStyle.ToString()}") as Style;
                return style;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    #endregion
}
