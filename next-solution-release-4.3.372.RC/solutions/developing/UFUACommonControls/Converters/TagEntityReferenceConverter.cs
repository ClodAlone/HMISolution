using OPCUAViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using UFUAModel;

namespace UFUACommonControls.Converters
{
    public class TagEntityReferenceConverter : IValueConverter
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
            if(value != null && value is TagEntityReference)
            {
                TagEntityReference uri = value as TagEntityReference;
                //if (uri.Name != null && uri.Name.Contains("&"))
                //    return string.Format("{0} ({1})", (uri.Name).Replace('&', '\\'), uri.AppName);
                //else
                return uri.StringRepresentationWithProject;
            }
            else if (value as String != null)
            {
                OPCUAXMLEntityReference newValue = new OPCUAXMLEntityReference() { TagReferenceXml = value.ToString() };
                var tagReference = newValue.TagReference;
                if (tagReference != null)
                if (tagReference.ReadablePath != null && tagReference.ReadablePath.Contains("&"))
                    return string.Format("{0} ({1})", (tagReference.ReadablePath).Replace('&', '\\'), tagReference.AppName);
                else
                    return tagReference.StringRepresentationWithProject;
            }
            return string.Empty;
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
