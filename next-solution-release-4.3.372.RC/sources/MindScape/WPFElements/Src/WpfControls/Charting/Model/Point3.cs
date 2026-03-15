using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mindscape.WpfElements.Charting
{
  /// <summary>
  /// A data object made of 3 double values.
  /// </summary>
  public class Point3 : ViewModelBase
  {
    private double _x;
    private double _y;
    private double _z;

    /// <summary>
    /// Initializes a new instance of the <see cref="Point3"/> class.
    /// </summary>
    /// <param name="x">The x value.</param>
    /// <param name="y">The y value.</param>
    /// <param name="z">The z value.</param>
    public Point3(double x, double y, double z)
    {
      X = x;
      Y = y;
      Z = z;
    }

    /// <summary>
    /// Gets or sets the x value.
    /// </summary>
    public double X
    {
      get { return _x; }
      set { Set<double>(ref _x, value, "X"); }
    }

    /// <summary>
    /// Gets or sets the y value.
    /// </summary>
    public double Y
    {
      get { return _y; }
      set { Set<double>(ref _y, value, "Y"); }
    }

    /// <summary>
    /// Gets or sets the z value.
    /// </summary>
    public double Z
    {
      get { return _z; }
      set { Set<double>(ref _z, value, "Z"); }
    }
  }
}
