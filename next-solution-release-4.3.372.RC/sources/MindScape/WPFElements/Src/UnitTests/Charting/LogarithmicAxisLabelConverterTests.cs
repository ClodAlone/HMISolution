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
  public class LogarithmicAxisValueConverterTests
  {
    private static LogarithmicAxisValueConverter _converter_10 = new LogarithmicAxisValueConverter();
    private static LogarithmicAxisValueConverter _converter_2 = new LogarithmicAxisValueConverter() { Base = 2 };

    [Test]
    public void ZeroAxisPlotPositionConvertsToOne()
    {
      Assert.AreEqual(1.0, _converter_10.GetDataObjectAt(0));
    }

    [Test]
    public void OneConvertsToZeroAxisPlotPosition()
    {
      Assert.AreEqual(0, _converter_10.GetAxisPlotPosition(1));
    }

    [Test]
    public void ConvertBothWaysIsTheSame()
    {
      Random random = new Random();
      for (int i = 0; i < 9; i++)
      {
        double dataValue = random.NextDouble() * Math.Pow(10, i + 1) + Math.Pow(10, i);
        double plotPosition = _converter_10.GetAxisPlotPosition(dataValue);
        double ConvertedDataValue = (double)_converter_10.GetDataObjectAt(plotPosition);
        Assert.AreEqual(Math.Round(dataValue, 3), Math.Round(ConvertedDataValue, 3));
      }
    }

    [Test]
    public void GetAxisPlotPosition()
    {
      Assert.AreEqual(Math.Log(123.4, 10), _converter_10.GetAxisPlotPosition(123.4));
      Assert.AreEqual(Math.Log(123897, 10), _converter_10.GetAxisPlotPosition(123897));
      Assert.AreEqual(Math.Log(12310918, 10), _converter_10.GetAxisPlotPosition(12310918));
      Assert.AreEqual(Math.Log(123892304.412, 10), _converter_10.GetAxisPlotPosition(123892304.412));
    }

    [Test]
    public void GetDataObjectAt()
    {
      Assert.AreEqual(Math.Pow(10, 4), _converter_10.GetDataObjectAt(4));
      Assert.AreEqual(Math.Pow(10, 10.5), _converter_10.GetDataObjectAt(10.5));
      Assert.AreEqual(Math.Pow(10, 0.01), _converter_10.GetDataObjectAt(0.01));
      Assert.AreEqual(Math.Pow(10, 6.78), _converter_10.GetDataObjectAt(6.78));
      Assert.AreEqual(Math.Pow(10, -3.65), _converter_10.GetDataObjectAt(-3.65));
    }

    [Test]
    public void GetAxisPlotPosition_BaseTwo()
    {
      Assert.AreEqual(Math.Log(123.4, 2), _converter_2.GetAxisPlotPosition(123.4));
      Assert.AreEqual(Math.Log(123897, 2), _converter_2.GetAxisPlotPosition(123897));
      Assert.AreEqual(Math.Log(12310918, 2), _converter_2.GetAxisPlotPosition(12310918));
      Assert.AreEqual(Math.Log(123892304.412, 2), _converter_2.GetAxisPlotPosition(123892304.412));
    }

    [Test]
    public void GetDataObjectAt_BaseTwo()
    {
      Assert.AreEqual(Math.Pow(2, 4), _converter_2.GetDataObjectAt(4));
      Assert.AreEqual(Math.Pow(2, 10.5), _converter_2.GetDataObjectAt(10.5));
      Assert.AreEqual(Math.Pow(2, 0.01), _converter_2.GetDataObjectAt(0.01));
      Assert.AreEqual(Math.Pow(2, 6.78), _converter_2.GetDataObjectAt(6.78));
      Assert.AreEqual(Math.Pow(2, -3.65), _converter_2.GetDataObjectAt(-3.65));
    }

    [Test]
    public void GetAxisPlotPosition_ZeroOrLess()
    {
      Assert.AreEqual(0, _converter_10.GetAxisPlotPosition(0));
      Assert.AreEqual(0, _converter_10.GetAxisPlotPosition(-4));
      Assert.AreEqual(0, _converter_2.GetAxisPlotPosition(0));
      Assert.AreEqual(0, _converter_2.GetAxisPlotPosition(-4));
    }

    [Test]
    [STAThread]
    public void LogarithmicScaleWithPointData()
    {
      Chart chart = new Chart();
      chart.YAxis = new ChartAxis() { ValueConverter = _converter_10 };
      SimpleDataSeries series = new SimpleDataSeries();
      chart.Series.Add(series);
      IList data = new List<object>();
      data.Add(new Point(0, 654332));
      data.Add(new Point(1, 23));
      data.Add(new Point(2, 4567.123));
      data.Add(new Point(3, 98123135234242));
      series.ItemsSource = data;

      int index = 0;
      foreach (CartesianDataPoint dataPoint in series.DataPoints)
      {
        Point point = (Point)data[index];
        Assert.AreEqual(index, dataPoint.LogicalPoint.X);
        Assert.AreEqual((double)index, dataPoint.XObject);
        Assert.AreEqual(_converter_10.GetAxisPlotPosition(point.Y), dataPoint.LogicalPoint.Y);
        Assert.AreEqual(point.Y, dataPoint.YObject);
        index++;
      }
    }

    [Test]
    [STAThread]
    public void LogarithmicScaleWithListOfIntegerData()
    {
      Chart chart = new Chart();
      chart.YAxis = new ChartAxis() { ValueConverter = _converter_10 };
      SimpleDataSeries series = new SimpleDataSeries();
      chart.Series.Add(series);
      IList data = new List<object>();
      data.Add(654332);
      data.Add(23);
      data.Add(567);
      data.Add(981237242);
      series.ItemsSource = data;

      int index = 0;
      foreach (CartesianDataPoint dataPoint in series.DataPoints)
      {
        int i = (int)data[index];
        Assert.AreEqual(index, dataPoint.LogicalPoint.X);
        Assert.AreEqual((double)index, dataPoint.XObject);
        Assert.AreEqual(_converter_10.GetAxisPlotPosition(i), dataPoint.LogicalPoint.Y);
        Assert.AreEqual((double)i, dataPoint.YObject);
        index++;
      }
    }

    [Test]
    [Ignore("This no longer works due to performance enhancements, this test needs to be revised")]
    public void LogarithmicScaleWithPointData_CorrectAutomaticAxisRange()
    {
      Chart chart = new Chart();
      chart.YAxis.ValueConverter = _converter_10;
      SimpleDataSeries series = new SimpleDataSeries();
      chart.Series.Add(series);
      IList data = new List<object>();
      data.Add(new Point(0, 654332));
      data.Add(new Point(1, 23));
      data.Add(new Point(2, 4567.123));
      data.Add(new Point(3, 98123135234242));
      series.ItemsSource = data;

      Assert.AreEqual(0, chart.YAxis.Minimum);
      // 15 is the value that will fit the largest value once it has been converted with the logarithmic scale.
      Assert.AreEqual(15, chart.YAxis.Maximum);
    }
  }
}
