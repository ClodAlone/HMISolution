using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace Mindscape.WpfElements.Charting
{
  /// <summary>
  /// Represents an axis label displayed by a <see cref="ChartAxis"/>.
  /// </summary>
  public class AxisLabel
  {
    private readonly object _label;
    private readonly string _formattedLabel;

    internal AxisLabel(object label, string formattedLabel)
    {
      _label = label;
      _formattedLabel = formattedLabel;
    }

    /// <summary>
    /// Gets the content of the <see cref="AxisLabel"/>.
    /// </summary>
    public object Label
    {
      get { return _label; }
    }

    /// <summary>
    /// Gets the string formatted content of the <see cref="AxisLabel"/>.
    /// </summary>
    public string FormattedLabel
    {
      get { return _formattedLabel; }
    }

    /// <summary>
    /// Returns a string representation of the <see cref="AxisLabel"/>.
    /// </summary>
    /// <returns>A string representation of the <see cref="AxisLabel"/>.</returns>
    public override string ToString()
    {
      return FormattedLabel;
    }
  }
}
