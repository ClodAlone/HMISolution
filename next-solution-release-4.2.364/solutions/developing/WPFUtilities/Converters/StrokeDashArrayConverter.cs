using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace WPFUtilities.Converters
{
    public class StrokeDashArrayConverter : IValueConverter
    {
        #region Members

        static Double[] dashStrokeArray = new Double[] { 4.0, 2.0 };
        static Double[] dotStrokeArray = new Double[] { 1.0, 2.0 };
        static Double[] dashDotStrokeArray = new Double[] { 4.0, 2.0, 1.0, 2.0 };
        static Double[] dashDotDotStrokeArray = new Double[] { 4.0, 2.0, 1.0, 2.0, 1.0, 2.0 };

        static System.Globalization.NumberFormatInfo formatInfo = new System.Globalization.NumberFormatInfo()
        {
            NumberDecimalSeparator = ".",
            NumberGroupSeparator = ","
        };

        DoubleCollection lastStrokeArrayEnum;
        String lastTextBoxValue;

        #endregion

        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (targetType != typeof(Int32) && targetType != typeof(String))
                throw new InvalidOperationException("The target must be a Int32 or String");

            var strokeArray = value as DoubleCollection;
            if (targetType == typeof(Int32))
            {
                if (strokeArray == null)
                    return (int)-1; // Custom
                if (strokeArray.Count == 0)
                    return (int)0; // Solid
                if (dashStrokeArray.SequenceEqual(strokeArray.ToArray()))
                    return (int)1; // Dash
                else if (dotStrokeArray.SequenceEqual(strokeArray.ToArray()))
                    return (int)2; // Dot
                else if (dashDotStrokeArray.SequenceEqual(strokeArray.ToArray()))
                    return (int)3; // Dash-Dot
                else if (dashDotDotStrokeArray.SequenceEqual(strokeArray.ToArray()))
                    return (int)4; // Dash-Dot-Dot
                else
                    return (int)-1; // Custom
            }
            else
            {
                if (lastStrokeArrayEnum == null && strokeArray != null && strokeArray.Count > 0)
                    lastTextBoxValue = String.Join(",", strokeArray);
                
                return lastTextBoxValue;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (targetType != typeof(DoubleCollection))
                throw new InvalidOperationException("The target must be a DoubleCollection");

            if (value.GetType() != typeof(Int32) && value.GetType() != typeof(String))
                throw new InvalidOperationException("The source must be a Int32 or String");

            if (value.GetType() == typeof(Int32))
            {
                var index = (int)value;
                switch (index)
                {
                    // Solid
                    case 0: lastStrokeArrayEnum = new DoubleCollection(); break;
                    // Dash
                    case 1: lastStrokeArrayEnum = new DoubleCollection(dashStrokeArray); break;
                    // Dot
                    case 2: lastStrokeArrayEnum = new DoubleCollection(dotStrokeArray); break;
                    // Dash-Dot
                    case 3: lastStrokeArrayEnum = new DoubleCollection(dashDotStrokeArray); break;
                    // Dash-Dot-Dot
                    case 4: lastStrokeArrayEnum = new DoubleCollection(dashDotDotStrokeArray); break;
                    // Custom
                    default: lastStrokeArrayEnum = null; break;
                }
            }
            else
            {
                lastTextBoxValue = value as String;
                if (lastStrokeArrayEnum == null)
                {
                    var ret = new DoubleCollection();
                    if (!String.IsNullOrEmpty(lastTextBoxValue))
                    {
                        var array = lastTextBoxValue.Split(',');
                        foreach (var s in array)
                        {
                            try
                            {
                                ret.Add(System.Convert.ToDouble(s, formatInfo));
                            }
                            catch
                            {
                                continue;
                            }
                        }
                    }
                    
                    return ret;
                }
            }

            return lastStrokeArrayEnum;
        }

        #endregion
        
    }
}
