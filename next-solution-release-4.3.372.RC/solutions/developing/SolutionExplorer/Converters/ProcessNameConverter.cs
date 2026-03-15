using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Data;

namespace UFProjectManager.Converters
{
    public class ProcessNameConverter : IMultiValueConverter
    {
        #region IMultiValueConverter Members

        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (values == null || values.Length < 1)
                return String.Empty;
            if (values.Length < 2)
                return values[0];
            return GetFriendlyProcessName(values[0] as string, values[1] as string);
        }

        public object[] ConvertBack(object value, Type[] targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
        string GetFriendlyProcessName(string name, string docTitle)
        {
            var ret = name;
            try
            {
                var testName = name;
                if (name.IndexOf("_") > 0)
                    testName = name.Substring(0, name.IndexOf("_"));
                ret = String.Format("{0} ({1})", processNameMap.ContainsKey(testName) ? processNameMap[testName] : name, docTitle);
            }
            catch { }
            return ret;
        }

        Dictionary<string, string> processNameMap = new Dictionary<string, string>()
        {
            { Properties.Settings.Default.SchedulerServiceNamePrefix, UFInterfaces.Properties.Resources.MSServer_ServiceDisplayName },
            { Properties.Settings.Default.ServerServiceNamePrefix, UFInterfaces.Properties.Resources.UFUAServer_ServiceDisplayName },
            { Properties.Settings.Default.LogicsServiceNamePrefix, UFInterfaces.Properties.Resources.LogicService_ServiceDisplayName },
            { Properties.Settings.Default.RecipesServiceNamePrefix, UFInterfaces.Properties.Resources.RecipeService_ServiceDisplayName },
            { Properties.Settings.Default.ClientWebHMIRootFolder, UFInterfaces.Properties.Resources.WebHMI_ServiceDisplayName }
        };
    }
}
