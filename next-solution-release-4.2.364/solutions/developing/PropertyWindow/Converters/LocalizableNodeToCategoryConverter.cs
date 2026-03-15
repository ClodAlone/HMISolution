using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Data;
using System.ComponentModel;
using Mindscape.WpfElements.PropertyEditing;

namespace PropertyControl
{
    /// <summary>
    /// Converts a <see cref="Node"/> to its category.
    /// </summary>
    [ValueConversion(typeof(Node), typeof(string))]
    public class LocalizableNodeToCategoryConverter : IValueConverter
    {
        /// <summary>
        /// Converts a <see cref="Node"/> representing a property to the category of that
        /// property, as specified using the <see cref="CategoryAttribute"/>.
        /// </summary>
        /// <param name="value">The node whose category is required.</param>
        /// <param name="targetType">The type of the binding target property.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the conversion.</param>
        /// <returns>The category of the node property, or the default category if none is specified.</returns>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            Node node = value as Node;
            if (node == null)
            {
                return Properties.Resources.MiscellaneousCategoryDisplayName;
            }

            return node.Property.Category ?? Properties.Resources.MiscellaneousCategoryDisplayName;
        }

        /// <summary>
        /// Converts a value from a binding target for writing to a binding source.
        /// </summary>
        /// <param name="value">The value produced by the binding target.</param>
        /// <param name="targetType">The type to convert to.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the conversion.</param>
        /// <returns>This conversion direction is not implemented by this converter.</returns>
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
