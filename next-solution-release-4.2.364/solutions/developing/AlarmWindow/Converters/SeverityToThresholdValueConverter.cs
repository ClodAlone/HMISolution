using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace AlarmWindow.Converters
{
    public class SeverityToThresholdValueConverter : IMultiValueConverter
    {

        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            try
            {
                if (values[0] == DependencyProperty.UnsetValue || values[1] == DependencyProperty.UnsetValue)
                    return new SolidColorBrush(Colors.Transparent);

                ObservableCollection<ThresholdSettings> _list = (ObservableCollection<ThresholdSettings>)values[1];

                if (values[0] == null)
                    return new SolidColorBrush(Colors.Transparent);

                var _value = double.Parse(values[0].ToString());

                if(_list.Count == 0)
                    return new SolidColorBrush(Colors.Transparent);

                var tv = _list.Where(x => x.ThresholdValue <= _value).Select(x => x).LastOrDefault();
                if(tv == null)
                    tv = _list.Where(x => x.ThresholdValue > _value).Select(x => x).FirstOrDefault();
                if (tv != null)
                {
                    if (values[2] != DependencyProperty.UnsetValue)
                    {
                        var needAcknowledge = Boolean.Parse(values[2].ToString());
                        if (values[3] != DependencyProperty.UnsetValue)
                        {
                            bool isInactive = values[3].ToString().Contains("Inactive");
                            if (needAcknowledge)
                            {
                                if(isInactive)
                                    return new SolidColorBrush(tv.ThresholdOffColor);
                                else
                                    return new SolidColorBrush(tv.ThresholdColor);
                            }
                            else
                            {
                                if (isInactive)
                                    return new SolidColorBrush(tv.ThresholdOffAckColor);
                                else
                                    return new SolidColorBrush(tv.ThresholdAckColor);
                            }
                        }
                        else if (needAcknowledge)
                            return new SolidColorBrush(tv.ThresholdColor);
                        else
                            return new SolidColorBrush(tv.ThresholdAckColor);
                    }
                    else
                        return new SolidColorBrush(tv.ThresholdColor);
                }

                return new SolidColorBrush(Colors.Transparent);
            }
            catch (Exception)
            {
                return new SolidColorBrush(Colors.Transparent);
            }
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
