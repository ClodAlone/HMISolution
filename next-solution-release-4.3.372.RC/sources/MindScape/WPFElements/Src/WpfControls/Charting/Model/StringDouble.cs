using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mindscape.WpfElements.Charting
{
  /// <summary>
  /// Represents a data object containing a string and a double.
  /// </summary>
  public class StringDouble : ViewModelBase
  {
    private string _s;
    private double _d;

    /// <summary>
    /// Initializes a new instance of the <see cref="StringDouble"/> class.
    /// </summary>
    /// <param name="s">The string value.</param>
    /// <param name="d">The double value.</param>
    public StringDouble(string s, double d)
    {
      String = s;
      Double = d;
    }

    /// <summary>
    /// Gets or sets the string value.
    /// </summary>
    public string String
    {
      get { return _s; }
      set { Set<string>(ref _s, value, "String"); }
    }

    /// <summary>
    /// Gets or sets the double value.
    /// </summary>
    public double Double
    {
      get { return _d; }
      set { Set<double>(ref _d, value, "Double"); }
    }
  }
}
