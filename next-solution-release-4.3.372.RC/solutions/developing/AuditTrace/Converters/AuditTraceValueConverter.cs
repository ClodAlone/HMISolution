using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace AuditTrace.Converters
{
    public class AuditTraceValueConverter : IValueConverter
    {
        #region Declarations
        readonly AuditTraceViewModel auditTraceViewModel;

        bool inExecution;
        object lastValue;
        #endregion

        #region Constructors
        public AuditTraceValueConverter(AuditTraceViewModel auditTraceViewModel)
        {
            this.auditTraceViewModel = auditTraceViewModel;
        }
        #endregion

        #region Properties
        public IValueConverter Converter { get; set; }
        #endregion

        #region IValueConverter implementation
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            lastValue = value;
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!auditTraceViewModel.IsAuditTraceEnabled)
                return value;

            if (inExecution)
                throw new InvalidOperationException("Cannot convert back the value when an audit comment is pending.");

            try
            {
                inExecution = true;
                object newValue = null;
                if (Converter != null)
                    newValue = Converter.ConvertBack(value, targetType, parameter, culture);

                if (newValue == null || !auditTraceViewModel.SetValue(newValue.ToString()))
                    return lastValue;

                return value;
            }
            finally
            {
                inExecution = false;
            }
        }
        #endregion
    }
}
