using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Data;
using DocumentManager.ComponentService;
using StringManager.ComponentService;

namespace StringManager.Converters
{
    /// <summary>
    /// A Value converter
    /// </summary>
    [ValueConversion(typeof(String), typeof(String))]
    public class StringIdConverter : IValueConverter
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
            
            if (value != null && !string.IsNullOrEmpty(value.ToString()))
            {
                if (value is String)
                {
                    //return value as String;
                    if (StringEditorManagerComponent.stringEditorManagerComponent.Workspace != null)
                    {
                        IDocument parent = StringEditorManagerComponent.stringEditorManagerComponent.Workspace.ContextDocument;
                        if (parent != null)
                        {
                            var actCult = StringEditorManagerComponent.stringEditorManagerComponent.GetActiveCulture(parent);
                            if (actCult != null)
                            {
                                var mapstring = StringEditorManagerComponent.stringEditorManagerComponent.GetListStringForCulture(parent, actCult);
                                if (mapstring.ContainsKey(value as string))
                                {
                                    return mapstring[value as string];
                                }
                            }
                        }
                    }
                    return value as string;
                }
            }

            return String.Empty;
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
            return value as String;
        }

        #endregion
    }
}
