using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Data;

namespace EditDisplay.Converters
{
    public class DateTimeStringConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            String format = null;
            if (parameter != null && parameter is String)
                format = parameter as String;

            try
            {
                var dvalue = System.Convert.ToDouble(value);
                if (!String.IsNullOrEmpty(format))
                {
                    var utcNow = DateTime.UtcNow;
                    int year = utcNow.Year;
                    int month = utcNow.Month;
                    int day = utcNow.Day;
                    int hours = utcNow.Hour;
                    int minute = utcNow.Minute;
                    int second = utcNow.Second;
                    int millisecond = utcNow.Millisecond;

                    var matches = Regex.Matches(format, @"yyyy|yy|MM|dd|hh|HH|mm|ss|fffffff|ffffff|fffff|ffff|fff|ff|f");
                    
                    var newformat = new StringBuilder();
                    foreach (Match match in matches)
                        newformat.Append(match.Value);
                    
                    var doubleformat = Regex.Replace(newformat.ToString(), @"[\s\S]", "0");
                    var sValue = dvalue.ToString(doubleformat);

                    int index = 0;
                    foreach (Match match in matches)
                    {
                        int datepart = System.Convert.ToInt32(sValue.Substring(index, match.Value.Length));
                        index += match.Value.Length;
                        if (match.Value == "yyyy" || 
                            match.Value == "yy")
                            year = datepart;
                        else if (match.Value == "MM")
                            month = datepart;
                        else if (match.Value == "dd")
                            day = datepart;
                        else if (match.Value == "hh" || 
                            match.Value == "HH")
                            hours = datepart;
                        else if (match.Value == "mm")
                            minute = datepart;
                        else if (match.Value == "ss")
                            second = datepart;
                        else if (match.Value == "fffffff" || 
                            match.Value == "ffffff" || 
                            match.Value == "fffff" || 
                            match.Value == "ffff" ||
                            match.Value == "fff" || 
                            match.Value == "ff" || 
                            match.Value == "f")
                            millisecond = datepart;
                    }

                    return new DateTime(year, month, day, hours, minute, second, millisecond);
                }
            }
            catch 
            { }
                
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            String format = null;
            if (parameter != null && parameter is String)
                format = parameter as String;

            if (value is DateTime)
            {
                var dateTime = (DateTime)value;
                if (!String.IsNullOrEmpty(format))
                {
                    var matches = Regex.Matches(format, @"yyyy|yy|MM|dd|hh|HH|mm|ss|fffffff|ffffff|fffff|ffff|fff|ff|f");

                    var newformat = new StringBuilder();
                    foreach (Match match in matches)
                        newformat.Append(match.Value);

                    var dateString = dateTime.ToString(newformat.ToString());
                    return Regex.Replace(dateString, @"[^0-9]", "");
                }
                else
                    return Regex.Replace(dateTime.ToString(), @"[^0-9]", "");
            }
            
            return value;
        }

        #endregion
    }
}
