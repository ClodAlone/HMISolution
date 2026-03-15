using Opc.Ua;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Text;
using System.Windows.Data;
using TranslationHelpers;

namespace DataloggerViewerControl.Converters
{
    public class EventStateConverter : IMultiValueConverter
    {
        #region IValueConverter Members
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (values == null || values.Length == 0 || values[0] as String == null)
                return String.Empty;

            if (values.Length < 2 || values[1] == null || !(values[1] is IDictionary<string, string>))
                return values[0];

            var stringlist = values[1] as IDictionary<string, string>;

            if (string.IsNullOrEmpty((values[0] as String)))
                return string.Empty;
            else if ((values[0] as String).Contains("|"))
            {
                var states = (values[0] as String).Split('|');
                StringBuilder sbuilder = new StringBuilder();
                foreach (string state in states)
                {
                    string tstate = state.Trim();
                    string resourceManagerString = GetResourceString(tstate);
                    string newState = TranslationHelper.TranlslateText($"_{HistoricalEventControl.Properties.Settings.Default.HistoricalEventsPlaceHolder}_{tstate}", stringlist, resourceManagerString);
                    sbuilder.Append(newState);
                    sbuilder.Append(" | ");
                }
                sbuilder.Remove(sbuilder.Length - 3, 3);
                return sbuilder.ToString();
            }
            else
            {
                string newState = TranslationHelper.TranlslateText($"_{HistoricalEventControl.Properties.Settings.Default.HistoricalEventsPlaceHolder}_{(values[0] as String)}", stringlist, (values[0] as String));
                return newState;
            }
        }

        private string GetResourceString(string tstate)
        {
            if (tstate == Opc.Ua.ConditionStateNames.Acknowledged)
                return HistoricalEventControl.Properties.Resources.AcknowledgedState;
            else if (tstate == Opc.Ua.ConditionStateNames.Unacknowledged)
                return HistoricalEventControl.Properties.Resources.UnacknowledgedState;
            else if (tstate == Opc.Ua.ConditionStateNames.Inactive)
                return HistoricalEventControl.Properties.Resources.InactiveState;
            else if (tstate == Opc.Ua.ConditionStateNames.Active)
                return HistoricalEventControl.Properties.Resources.ActiveState;
            else if (tstate == Opc.Ua.ConditionStateNames.Unconfirmed)
                return HistoricalEventControl.Properties.Resources.UnconfirmedState;
            else
                return tstate;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return new object[] { value };
        }
        #endregion
    }
}