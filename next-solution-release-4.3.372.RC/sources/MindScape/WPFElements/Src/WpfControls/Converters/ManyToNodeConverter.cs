using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Data;
using System.ComponentModel;
using Mindscape.WpfElements.PropertyEditing;

namespace Mindscape.WpfElements.WpfPropertyGrid
{
  /// <summary>
  /// Converts an <see cref="ObjectWrapper"/> for a <see cref="Many"/> to a 
  /// node of singular type suitable for editor selection.
  /// </summary>
  /// <remarks>This class supports the multiple selection infrastructure and is 
  /// intended for use only as part of a <see cref="ManyEditor"/> control template.</remarks>
  [ValueConversion(typeof(ObjectWrapper), typeof(Node))]
  public class ManyToNodeConverter : IValueConverter
  {
    /// <summary>
    /// Converts an <see cref="ObjectWrapper"/> containing a consistent <see cref="Many"/>
    /// to a <see cref="Node"/> carrying metadata for selecting an editor appropriate to the
    /// Many type.
    /// </summary>
    /// <param name="value">An ObjectWrapper containing a Many.</param>
    /// <param name="targetType">The type of the binding target property.</param>
    /// <param name="parameter">The converter parameter to use.</param>
    /// <param name="culture">The culture to use in the conversion.</param>
    /// <returns>A value suitable for use by the binding target.</returns>
    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      ObjectWrapper ow = (ObjectWrapper)value;
      if (ow == null || ow.Property == null)
      {
        return null;
      }

      Many many = (Many)(ow.Property.Value);
      if (many == null)
      {
        return null;
      }

      PropertyNode node = CreateNodeFromMany(many, ow.Property.ChildFilter);

      return node;
    }

    internal static PropertyNode CreateNodeFromMany(Many many, Predicate<Node> childFilter)
    {
      PropertyNode node = new PropertyNode(many, many.ValuePropertyDescriptor, childFilter);
      bool isReadOnly = many.Descriptor.IsReadOnly;
      node.SetCanWrite(!isReadOnly);
      return node;
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
