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
  /// Represents a thata- and rho-coordinate pair in polar space.
  /// </summary>
  public struct PolarPoint
  {
    // TODO: make this immutable.

    double _theta;
    double _rho;

    /// <summary>
    /// Initializes a new instance of the <see cref="PolarPoint"/> struct.
    /// </summary>
    /// <param name="theta">The theta coordinate.</param>
    /// <param name="rho">The rho coordinate.</param>
    public PolarPoint(double theta, double rho)
    {
      _theta = theta;
      _rho = rho;
    }

    /// <summary>
    /// Gets the theta coordinate value of this <see cref="PolarPoint"/>.
    /// </summary>
    public double Theta
    {
      get { return _theta; }
      internal set { _theta = value; }
    }

    /// <summary>
    /// Gets the rho coordinate of this <see cref="PolarPoint"/>.
    /// </summary>
    public double Rho
    {
      get { return _rho; }
      internal set { _rho = value; }
    }
  }
}
