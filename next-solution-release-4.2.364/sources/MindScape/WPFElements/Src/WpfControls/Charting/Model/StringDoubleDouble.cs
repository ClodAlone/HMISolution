using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mindscape.WpfElements.Charting
{
  /// <summary>
  /// Represents a data object containing a string value and 2 double values.
  /// Useful for plotting data in a <see cref="BubbleSeries"/>.
  /// </summary>
  public class StringDoubleDouble : ViewModelBase
  {
    private string _string;
    private double _double;
    private double _size;

    /// <summary>
    /// Initializes a new instance of the <see cref="StringDoubleDouble"/> class.
    /// </summary>
    /// <param name="str">The string value, generally plotted along the X axis.</param>
    /// <param name="d">The double value, generally plotted along the Y axis.</param>
    /// <param name="size">The size value, used for the size of the bubbles.</param>
    public StringDoubleDouble(string str, double d, double size)
    {
      String = str;
      Double = d;
      Size = size;
    }

    /// <summary>
    /// Gets or sets the string value.
    /// </summary>
    public string String
    {
      get { return _string; }
      set { Set<string>(ref _string, value, "String"); }
    }

    /// <summary>
    /// Gets or sets the double value.
    /// </summary>
    public double Double
    {
      get { return _double; }
      set { Set<double>(ref _double, value, "Double"); }
    }

    /// <summary>
    /// Gets or sets the size value.
    /// </summary>
    public double Size
    {
      get { return _size; }
      set { Set<double>(ref _size, value, "Size"); }
    }
  }
}
