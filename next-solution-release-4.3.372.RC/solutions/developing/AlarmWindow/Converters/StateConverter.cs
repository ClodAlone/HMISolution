using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;

namespace AlarmWindow.Converters
{
    public class StateConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null || parameter == null || parameter == DependencyProperty.UnsetValue)
                return value;

            try
            {
                int index = int.Parse(parameter.ToString());
                string state = value.ToString();
                var states = state.Split('|');
                if (index >= states.Length)
                    return value;

                if(index == 0)
                {
                    string statevalue = states[index].Trim();
                    string ret = GetTextValue(statevalue);
                    return ret;
                }
                else
                {
                    StringBuilder ret = new StringBuilder();
                    for (int i = 1; i < states.Length; i++)
                    {
                        ret.Append($"{GetTextValue(states[i].Trim())} | ");
                    }

                    ret.Remove(ret.Length - 2, 2);
                    return ret;
                }
            }
            catch (Exception)
            {
                return value;
            }

        }
        string GetTextValue(string statevalue)
        {
            switch (statevalue)
            {
                case "Active":
                    return Properties.Resources.StateON;
                case "High":
                    return Properties.Resources.StateONHigh;
                case "HighHigh":
                    return Properties.Resources.StateONHighHigh;
                case "Low":
                    return Properties.Resources.StateONLow;
                case "LowLow":
                    return Properties.Resources.StateONLowLow;
                case "HighActive":
                    return Properties.Resources.StateONHighActive;
                case "HighHighActive":
                    return Properties.Resources.StateONHighHighActive;
                case "LowActive":
                    return Properties.Resources.StateONLowActive;
                case "LowLowActive":
                    return Properties.Resources.StateONLowLowActive;
                case "Inactive":
                    return Properties.Resources.StateOFF;
                case "Unacknowledged":
                    return Properties.Resources.StateUnacknowledged;
                case "Unconfirmed":
                    return Properties.Resources.StateUnconfirmed;
                case "Suppressed":
                    return Properties.Resources.StateSuppressed;
                case "TimedShelved":
                    return Properties.Resources.StateTimedShelved;
                default:
                    return statevalue;
            }

        }
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
