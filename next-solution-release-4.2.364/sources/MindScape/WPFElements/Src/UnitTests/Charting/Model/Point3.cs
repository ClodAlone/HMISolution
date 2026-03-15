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

namespace Mindscape.WpfElements.UnitTests
{
  public struct Point3
  {
    private double _x, _y, _z;

    public Point3(double x, double y, double z)
    {
      _x = x;
      _y = y;
      _z = z;
    }

    public double X
    {
      get { return _x; }
      set { _x = value; }
    }

    public double Y
    {
      get { return _y; }
      set { _y = value; }
    }

    public double Z
    {
      get { return _z; }
      set { _z = value; }
    }
  }
}
