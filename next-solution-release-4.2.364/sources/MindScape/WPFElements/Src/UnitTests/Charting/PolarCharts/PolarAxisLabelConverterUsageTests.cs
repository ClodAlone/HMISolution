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
using NUnit.Framework;

namespace Mindscape.WpfElements.UnitTests
{
  [TestFixture]
  public class PolarAxisLabelConverterUsageTests
  {
    private SimpleAxisValueConverter _converter = new SimpleAxisValueConverter();

    [Test]
    [STAThread]
    public void ThetaAxis_GetLogicalPositionMethodUsesConverter()
    {
      ThetaAxis axis = new ThetaAxis();
      axis.Minimum = 0;
      axis.Maximum = 10;
      axis.ValueConverter = _converter; // Set the axis to use a SimpleAxisValueConverter

      double value = axis.GetLogicalPosition(0.0);

      // The GetLogicalPosition method should use the converter. This will simply add 5 to the given value.
      Assert.AreEqual(5, value);
    }

    [Test]
    [STAThread]
    public void RhoAxis_GetLogicalPositionMethodUsesConverter()
    {
      RhoAxis axis = new RhoAxis();
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
      PolarChart chart = new PolarChart();
      ThetaAxis thetaAxis = new ThetaAxis() { Minimum = 0, Maximum = 10, ValueConverter = _converter };
      RhoAxis rhoAxis = new RhoAxis() { Minimum = 0, Maximum = 10, ValueConverter = _converter };
      chart.ThetaAxis = thetaAxis;
      chart.RhoAxis = rhoAxis;
      SimplePolarDataSeries series = new SimplePolarDataSeries();
      chart.Series.Add(series);

      PolarPoint point = new PolarPoint(2.1, 4.3);
      SimplePolarDataPoint dp = new SimplePolarDataPoint();
      dp.DataContext = point;

      PolarPoint logicalPoint = series.GetLogicalPoint(dp);
      Assert.AreEqual(new PolarPoint(7.1, 9.3), logicalPoint);
    }
  }
}
