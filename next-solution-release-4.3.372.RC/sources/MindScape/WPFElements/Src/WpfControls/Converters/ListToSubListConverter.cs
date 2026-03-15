using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Windows.Data;

namespace Mindscape.WpfElements
{
  /// <summary>
  /// Extracts a sub-list from a master list.
  /// </summary>
  public class ListToSubListConverter : IValueConverter
  {
    /// <summary>
    /// The index within the source list at which to start extracting the sub-list.
    /// </summary>
    public int StartIndex { get; set; }

    /// <summary>
    /// The length of the sub-list.
    /// </summary>
    public int Length { get; set; }

    /// <summary>
    /// Extracts a sub-list from a master list.  The sub-list begins
    /// at <see cref="StartIndex"/>, and has <see cref="Length"/> elements.
    /// </summary>
    /// <param name="value">The <see cref="IList"/> value produced by the binding source.</param>
    /// <param name="targetType">The type of the binding target property.</param>
    /// <param name="parameter">The converter parameter to use.</param>
    /// <param name="culture">The culture to use in the conversion.</param>
    /// <returns>The sub-list extracted from the master list.</returns>
    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      IList list = value as IList;
      List<Object> sub = new List<Object>();
      if (list != null)
      {
        int startIndex = Math.Max(0, Math.Min(StartIndex, list.Count));
        int endIndex = Math.Max(0, Math.Min(StartIndex + Length, list.Count));
        for (int i = startIndex; i < endIndex; i++)
        {
          sub.Add(list[i]);
        }
      }
      return sub.AsReadOnly();
    }

    /// <summary>
    /// Converts a value from a binding target for writing to multiple binding source.
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
