using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace Mindscape.WpfElements
{
  /// <summary>
  /// A converter for splitting a string into 1 or 2 parts for display in a collapsed dropdown header within a <see cref="Ribbon"/> control.
  /// </summary>
  public class DropDownContentConverter : IValueConverter
  {
    /// <summary>
    /// Gets or sets whether this converter instance returns the top content or not.
    /// </summary>
    public bool IsTop { get; set; }

    /// <summary>
    /// Gets or sets whether a space is appended to the bottom content.
    /// </summary>
    public bool IncludeFinalSpace { get; set; }

    /// <summary>
    /// Converts a string into either the top or bottom string to be displayed in a collapsed dropdown header.
    /// </summary>
    /// <param name="value">The string value produced by the binding source.</param>
    /// <param name="targetType">The type of the binding target property.</param>
    /// <param name="parameter">The converter parameter to use.</param>
    /// <param name="culture">The culture to use in the conversion.</param>
    /// <returns>The converted string.</returns>
    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      if (IsTop)
      {
        if (value is string)
        {
          string str = (string)value;
          string top;
          string bottom;
          SplitString(str, out top, out bottom);
          return top;
        }
        return value;
      }
      else
      {
        if (value is string)
        {
          string str = (string)value;
          string top;
          string bottom;
          SplitString(str, out top, out bottom);
          if (IncludeFinalSpace && bottom.Length > 0)
          {
            bottom += " ";
          }
          return bottom;
        }
        return null;
      }
    }

    private static void SplitString(string str, out string top, out string bottom)
    {
      int middleIndex = (str.Length + 2) / 2;
      int index = middleIndex;
      top = "";
      int count = 0;
      int previousSpaceIndex = 0;
      foreach (char ch in str)
      {
        if (count < index)
        {
          top += ch;
          if (ch.Equals(' '))
          {
            previousSpaceIndex = count;
          }
        }
        else if (count > middleIndex)
        {
          break;
        }
        else
        {
          if (previousSpaceIndex == count - 1)
          {
            break;
          }
          int spaceIndex = str.IndexOf(' ', count);
          if (spaceIndex == -1)
          {
            spaceIndex = str.Length;
          }

          if (spaceIndex - count < count - previousSpaceIndex)
          {
            top += ch;
            index = spaceIndex; // Allows the loop to continue to the next space.
          }
          else if (previousSpaceIndex != -1)
          {
            top = top.Substring(0, Math.Max(0, previousSpaceIndex)); // Revert back to the previous space
            break;
          }
          else
          {
            top += ch;
            index = spaceIndex; // Allows the loop to continue to the next space.
          }
        }
        count++;
      }
      bottom = str.Substring(top.Length);
      top.Trim();
      bottom.Trim();
    }

    /// <summary>
    /// This conversion direction is not implemented by this converter.
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
