using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

using System.Windows;
using System.Windows.Data;
using UFUAEditor.ComponentService;
using DocumentManager.ComponentService;

namespace DataAnalisysControl.Converters
{
    public class AggregatedTablesConverter : IMultiValueConverter
    {
        public IDocument Document { get; set; }
        public IUFUAEditorManager UFUAEditorManager { get; set; }

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values[0] == DependencyProperty.UnsetValue || values[1] == DependencyProperty.UnsetValue)
                return false;
            if (Document == null || UFUAEditorManager == null
                || values[1] == null || string.IsNullOrEmpty(values[1].ToString()))
                    return false;

            try
            {
                string historical = values[1].ToString();
                bool dlrSource = bool.Parse(values[0].ToString());
                return dlrSource && UFUAEditorManager.UsesAggreagatedTables(Document, historical);
            }
            catch
            {
                return false;
            }
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
