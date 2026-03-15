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
using System.Collections;
using System.Collections.Generic;
using Mindscape.WpfElements.Charting;
using NUnit.Framework;

namespace Mindscape.WpfElements.UnitTests
{
  [TestFixture]
  public class AxisValueConverterUsageTests
  {
    private SimpleAxisValueConverter _converter = new SimpleAxisValueConverter();

    [Test]
    [STAThread]
    public void ChartAxis_GetLogicalPositionMethodUsesConverter()
    {
      ChartAxis axis = new ChartAxis();
      axis.Minimum = 0;
      axis.Maximum = 10;
      axis.ValueConverter = _converter; // Set the axis to use a SimpleAxisValueConverter

      double value = axis.GetLogicalPosition(0.0);

      // The GetLogicalPosition method should use the converter. This will simply add 5 to the given value.
      Assert.AreEqual(5, value);
    }

    [Test]
    [STAThread]
    public void DataSeriesUsesConverterOnPointData()
    {
      Chart chart = new Chart();
      ChartAxis xAxis = new ChartAxis() { Minimum = 0, Maximum = 10, ValueConverter = _converter };
      ChartAxis yAxis = new ChartAxis() { Minimum = 0, Maximum = 10, ValueConverter = _converter };
      chart.XAxis = xAxis;
      chart.YAxis = yAxis;
      SimpleDataSeries series = new SimpleDataSeries();
      chart.Series.Add(series);

      Point point = new Point(2.1, 4.3);
      SimpleDataPoint dp = new SimpleDataPoint();
      dp.DataContext = point;

      Point logicalPoint = series.GetLogicalPoint(dp);
      Assert.AreEqual(new Point(7.1, 9.3), logicalPoint);
    }
  }
}
