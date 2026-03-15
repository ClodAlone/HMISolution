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
  /// Plots data using a logarithmic scale.
  /// </summary>
  public class LogarithmicAxisValueConverter : IAxisValueConverter
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="LogarithmicAxisValueConverter"/> class.
    /// </summary>
    public LogarithmicAxisValueConverter()
    {
      Base = 10;
      Minimum = 1;
    }

    /// <summary>
    ///  Gets or sets the logarithmic base. The default is 10.
    /// </summary>
    public double Base { get; set; }

    /// <summary>
    /// Gets or sets the minimum value displayed on the axis. This is typically a value greater than 0 and less than or equal to 1.
    /// The default is 1.
    /// </summary>
    public double Minimum { get; set; }

    /// <summary>
    /// Gets or sets whether or not the logarithmic scale includes zero.
    /// The default is false.
    /// </summary>
    public bool StartsFromZero { get; set; }

    /// <summary>
    /// Converts the given value into an axis plot position.
    /// </summary>
    /// <param name="o">A numerical data value.</param>
    /// <returns>The axis plot position</returns>
    public double GetAxisPlotPosition(object o)
    {
      double offset = 0;
      if (Minimum > 0)
      {
        offset = Math.Log(Minimum, Base);
      }
      double plotPosition = 0;
      double? value = NumericalUtils.ConvertToDouble(o);
      if (value != null)
      {
        if (value > 0)
        {
          plotPosition = Math.Log(value.Value, Base) - offset;
          if (StartsFromZero)
          {
            plotPosition++;
          }
        }
      }
      return plotPosition;
    }

    /// <summary>
    /// Gets the data object corresponding to a logical axis position.
    /// </summary>
    /// <param name="axisPosition">The logical axis position.</param>
    /// <returns>The object represented on the axis at that position.</returns>
    public object GetDataObjectAt(double axisPosition)
    {
      double offset = 0;
      if (Minimum > 0)
      {
        offset = Math.Log(Minimum, Base);
      }
      if (StartsFromZero)
      {
        if (axisPosition >= 1)
        {
          axisPosition--;
        }
        else if (axisPosition > 0)
        {
          return Math.Pow(10, axisPosition + offset) / 10.0;
          //double ratio = 1 / Minimum;
          //return axisPosition * ratio;
        }
        else
        {
          return 0.0;
        }
      }
      double value = Math.Pow(Base, axisPosition + offset);
      return value;
    }
  }
}
