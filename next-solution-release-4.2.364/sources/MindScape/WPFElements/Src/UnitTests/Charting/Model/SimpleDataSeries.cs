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
using Mindscape.WpfElements.Charting;

namespace Mindscape.WpfElements.UnitTests
{
  public class SimpleDataSeries : DataSeries
  {
    private Orientation _orientation = Orientation.Vertical;

    public Point GetLogicalPoint(CartesianDataPoint dataPoint)
    {
      return GetPoint(dataPoint, 0);
    }

    protected override void BuildChartCore()
    {
      int index = 0;
      foreach (object o in ItemsSource)
      {
        SimpleDataPoint dp = GetSimpleDataPoint(o, index);
        GetPoint(dp, index);
        PrepareDataPoint(dp, index);
        Canvas.Children.Add(dp);
        index++;
      }
    }

    public Orientation Orientation
    {
      get { return _orientation; }
      set { _orientation = value; }
    }

    internal override bool ReverseAxes
    {
      get
      {
        return Orientation == Orientation.Horizontal;
      }
    }

    private SimpleDataPoint GetSimpleDataPoint(object o, int index)
    {
      SimpleDataPoint dp = GetDataPoint(o, index) as SimpleDataPoint;
      if (dp == null)
      {
        dp = new SimpleDataPoint();
        dp.DataContext = o;
      }
      return dp;
    }
  }
}
