using System;
using System.Globalization;
using System.Windows.Data;
using DocumentManager.ComponentService;
using UFInterfaces;

namespace CommonControls.Converters
{
    public class ConnectionStringConverter : IMultiValueConverter
    {
        #region Declarations
        IDocument contextDocument;
        #endregion

        #region IMultiValueConverter
        
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            contextDocument = null;
            if (values[0] == null)
                return null;

            if (values != null && values.Length > 1)
            {
                var value = values[0] as string;
                if (value != null)
                {
                    if (values[1] is IWorkspace)
                        contextDocument = (values[1] as IWorkspace).ContextDocument;
                    else if (values[1] is IDocument)
                        contextDocument = values[1] as IDocument;
                    return XpoHelpers.XpoHelper.NormalizeConnectionString(value, contextDocument?.rootBase);
                }
            }

            return values[0];
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            if (value is string)
                return new object[] { XpoHelpers.XpoHelper.AddPlaceholderToConnectionString(value as string, contextDocument?.rootBase) };

            return new object[] { value };
        }
        #endregion
    }
}
