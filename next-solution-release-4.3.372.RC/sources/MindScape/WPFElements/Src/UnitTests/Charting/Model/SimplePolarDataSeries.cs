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
  public class SimplePolarDataSeries : PolarSeries
  {
    public PolarPoint GetLogicalPoint(PolarDataPoint dataPoint)
    {
      return GetPoint(dataPoint);
    }

    protected override void BuildChartCore()
    {
      int index = 0;
      foreach (object o in ItemsSource)
      {
        SimplePolarDataPoint dp = GetSimpleDataPoint(o, index);
        GetPoint(dp);
        PrepareDataPoint(dp, index);
        Canvas.Children.Add(dp);
        index++;
      }
    }

    private SimplePolarDataPoint GetSimpleDataPoint(object o, int index)
    {
      SimplePolarDataPoint dp = GetDataPoint(o, index) as SimplePolarDataPoint;
      if (dp == null)
      {
        dp = new SimplePolarDataPoint();
        dp.DataContext = o;
      }
      return dp;
    }
  }
}
