using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mindscape.WpfElements.WpfDataGrid
{
  /// <summary>
  /// Represents the mode (most occurring value) of a collection and includes the percantage of occurrence.
  /// </summary>
  public class Mode
  {
    private readonly object _value;
    private readonly double _percentage;

    /// <summary>
    /// Initializes a new instance of the <see cref="Mode"/> class.
    /// </summary>
    /// <param name="value">The mode value.</param>
    /// <param name="percentage">The percentage of occurrence.</param>
    public Mode(object value, double percentage)
    {
      _value = value;
      _percentage = percentage;
    }

    /// <summary>
    /// Gets the mode value.
    /// </summary>
    public object Value
    {
      get { return _value; }
    }

    /// <summary>
    /// Gets the percentage of occurrence.
    /// </summary>
    public double Percentage
    {
      get { return _percentage; }
    }
  }
}
