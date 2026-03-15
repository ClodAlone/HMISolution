using Opc.Ua;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Data;
using TranslationHelpers;

namespace DataloggerViewerControl.Converters
{
    public class QualityIdentifierToStringConverter : IValueConverter
    {
        #region Public Props
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public IDictionary<string, string> CurrentStringList { get; set; }
        #endregion

        #region IValueConverter Members
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
                return value;
            if (uint.TryParse(value.ToString(), out uint valCode))
            {
                var ret = StatusCodes.GetBrowseName(valCode);
                if (!string.IsNullOrEmpty(ret) && CurrentStringList != null)
                    ret = TranslationHelper.TranlslateText(ret, CurrentStringList, ret);
                return ret;
            }
            return value;
        }
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value;
        }
        #endregion
    }
}
