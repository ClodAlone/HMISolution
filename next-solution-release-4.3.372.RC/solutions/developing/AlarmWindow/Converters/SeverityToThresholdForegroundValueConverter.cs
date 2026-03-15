using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media;

namespace AlarmWindow.Converters
{
    public class SeverityToThresholdForegroundValueConverter : IMultiValueConverter
    {

        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            try
            {
                if (values[0] == DependencyProperty.UnsetValue || values[1] == DependencyProperty.UnsetValue)
                    return values[2] == DependencyProperty.UnsetValue ? new SolidColorBrush(Colors.White) : (Brush)values[2];

                ObservableCollection<ThresholdSettings> _list = (ObservableCollection<ThresholdSettings>)values[1];

                if (values[0] == null)
                    return values[2] == DependencyProperty.UnsetValue ? new SolidColorBrush(Colors.White) : (Brush)values[2];

                var _value = double.Parse(values[0].ToString());

                if (_list.Count == 0)
                    return values[2] == DependencyProperty.UnsetValue ? new SolidColorBrush(Colors.White) : (Brush)values[2];

                var tv = _list.Where(x => x.ThresholdValue <= _value).Select(x => x).LastOrDefault();
                if (tv == null)
                    tv = _list.Where(x => x.ThresholdValue > _value).Select(x => x).FirstOrDefault();
                if (tv != null)
                {
                    if (values.Count() > 3 && values[3] != DependencyProperty.UnsetValue)
                    {
                        var needAcknowledge = Boolean.Parse(values[3].ToString());
                        if (values[4] != DependencyProperty.UnsetValue)
                        {
                            bool isInactive = values[4].ToString().Contains("Inactive");
                            if (needAcknowledge)
                            {
                                if (isInactive)
                                    return new SolidColorBrush(tv.ThresholdOffForeColor);
                                else
                                    return new SolidColorBrush(tv.ThresholdForeColor);
                            }
                            else
                            {
                                if (isInactive)
                                    return new SolidColorBrush(tv.ThresholdOffAckForeColor);
                                else
                                    return new SolidColorBrush(tv.ThresholdAckForeColor);
                            }
                        }
                        else if (needAcknowledge)
                            return new SolidColorBrush(tv.ThresholdForeColor);
                        else
                            return new SolidColorBrush(tv.ThresholdAckForeColor);
                    }
                    else
                        return new SolidColorBrush(tv.ThresholdForeColor);
                }

                return new SolidColorBrush(Colors.Transparent);
            }
            catch (Exception)
            {
                return new SolidColorBrush(Colors.White);
            }
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
