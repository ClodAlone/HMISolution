using OPCUABrowser;
using OPCUAViewModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;
using Utilities;
using ViewModelLib;

namespace OPCUAClientStatus.Converters
{
    public class CustomControlConverter : IValueConverter
    {
        #region IValueConverter Members

        /// <summary>
        /// Modifies the source data before passing it to the target for display in the UI. 
        /// </summary>
        /// <param name="value">The source data being passed to the target </param>
        /// <param name="targetType">The Type of data expected by the target dependency property.</param>
        /// <param name="parameter">An optional parameter to be used in the converter logic.</param>
        /// <param name="culture">The culture of the conversion.</param>
        /// <returns>The value to be passed to the target dependency property. </returns>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            //if (targetType != typeof(String))
            //    throw new InvalidOperationException("The target must be a String");
            try
            {
                TreeViewItemViewModel dvm = (value as WPFUtilities.TreeItemControl).TreeItemInnerObject as TreeViewItemViewModel;
                if (dvm == null)
                    return null;
                if (dvm is SessionViewModel)
                {
                    SessionItemControl control = new SessionItemControl(null)
                    {
                        DataContext = dvm
                    };
                    return control;
                }
                else if (dvm is RealTimeConnectionManagerViewModel)
                {
                    PendingItemControl control = new PendingItemControl()
                    {
                        DataContext = dvm
                    };
                    return control;
                }
                else
                {
                    TextBlock text = new TextBlock();
                    text.Text = dvm.Title;
                    return text;
                }
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Modifies the target data before passing it to the source object. This method is called only in TwoWay bindings. 
        /// </summary>
        /// <param name="value">The target data being passed to the source.</param>
        /// <param name="targetType">The Type of data expected by the source object.</param>
        /// <param name="parameter">An optional parameter to be used in the converter logic. </param>
        /// <param name="culture">The culture of the conversion.</param>
        /// <returns>The value to be passed to the source object.</returns>
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
