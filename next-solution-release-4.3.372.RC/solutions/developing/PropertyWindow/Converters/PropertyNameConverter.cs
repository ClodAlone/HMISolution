using Mindscape.WpfElements.WpfPropertyGrid;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using PropertyControl.Localization;
using System.Windows;

namespace PropertyControl
{
    /// <summary>
    /// Converts the level of a <see cref="PropertyGridRow"/> to a horizontal offset.
    /// </summary>
    public sealed class PropertyNameConverter : IMultiValueConverter
    {
        
        /// <summary>
        /// Converts a value from a binding source for use by a binding target.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object[] value, Type targetType, object parameter, CultureInfo culture)
        {
            var tagNames = (string)value[0];
            int numberOfelements = tagNames.Split('|').Length - 1;

            string humanName = "";
            if (numberOfelements >= System.Convert.ToInt32(parameter))
                humanName = tagNames.Split('|')[System.Convert.ToInt32(parameter)];
            else
                humanName = tagNames.Split('|')[0];

            try
            {
                bool isLocalizable = (bool)value[1];

                if (isLocalizable)
                    return humanName;
                else
                {
                    string resourceFileName = String.Empty;
                    if (value[2] != null)
                        resourceFileName = (value[2] as Type).FullName;

                    var resourceManager = new PropertyResourceManager(resourceFileName);

                    string newDisplayName = resourceManager.GetString(humanName);
                    if (String.IsNullOrEmpty(newDisplayName))
                        return humanName;
                    else
                        return newDisplayName;
                }
            }
            catch(Exception ex)
            {
                return humanName;
            }
        }

        /// <summary>
        /// Converts a value from a binding target for writing to a binding source.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object[] ConvertBack(object value, Type[] targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }

    public sealed class MultiplePropertiesVisibilityConverter : IValueConverter
    {
        /// <summary>
        /// Converts a value from a binding source for use by a binding target.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string humanName = (string)value;
            var numberOfWords = humanName.Split('|').Length;

            if (System.Convert.ToInt32(parameter) > numberOfWords)
                return Visibility.Collapsed;
            else
                return Visibility.Visible;
        }

        /// <summary>
        /// Converts a value from a binding target for writing to a binding source.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
