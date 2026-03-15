using System;
using System.Globalization;
#if !NET_STANDARD
using System.Windows.Data;
#endif
using Utilities.Enums;

namespace Utilities.Converters
{
    public class AlarmMaskConverter : IValueConverter
    {
        AlarmMask target;

        public AlarmMaskConverter()
        {
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                target = AlarmMask.Default;
                if (Nullable.GetUnderlyingType(targetType) != null)
                    return null;
                else
                    return false;
            }

            AlarmMask mask = (AlarmMask)parameter;
            target = (AlarmMask)Enum.ToObject(typeof(AlarmMask), value);
            return ((mask & target) != 0);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null && Nullable.GetUnderlyingType(targetType) != null)
                return null;

            AlarmMask mask = (AlarmMask)parameter;
            bool bValue = System.Convert.ToBoolean(value);
            return GetNewVisibilityLevel(mask, target, bValue);
        }

        static public uint GetNewVisibilityLevel(AlarmMask mask, AlarmMask target, bool bAdd)
        {
            if (bAdd)
                return (uint)(mask | target);
            else
                return (uint)(mask ^ target);
        }
    }
}
