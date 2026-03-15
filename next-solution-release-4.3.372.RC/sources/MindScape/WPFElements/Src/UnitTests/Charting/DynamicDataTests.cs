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
using System.Collections.ObjectModel;
using System.Windows.Data;
using Mindscape.WpfElements.Charting;
using NUnit.Framework;

namespace Mindscape.WpfElements.UnitTests
{
  [TestFixture]
  public class DynamicDataTests
  {
    #region Chart tests

    [Test]
    [Ignore("Performance improvements prevents this from working in the test environment")]
    public void AddingDataToItemsSourceUpdatesChart()
    {
      Chart chart = new Chart();
      SimpleDataSeries series = new SimpleDataSeries();
      chart.Series.Add(series);

      // Abitry data
      IList data = new ObservableCollection<Point>();
      data.Add(new Point(0, 3));
      data.Add(new Point(1, -4));
      data.Add(new Point(2, -2.3));
      data.Add(new Point(3, 34));
      data.Add(new Point(4, 12.4));

      series.ItemsSource = data;

      Assert.AreEqual(5, series.DataPoints.Count);
      // Add another piece of data to the collection:
      data.Add(new Point(5, 25));
      // This should increase the number of DataPoint controls generated:
      Assert.AreEqual(6, series.DataPoints.Count);
      // And also the number of items in the chart canvas: (does not get updated in a test environment)
      //Assert.AreEqual(6, series.Canvas.Children.Count);
    }

    [Test]
    [Ignore("This test needs to be revised")]
    public void AddingDataToItemsSourceUpdatesXAxis()
    {
      Chart chart = new Chart();
      SimpleDataSeries series = new SimpleDataSeries();
      chart.Series.Add(series);

      // Abitry data
      IList data = new ObservableCollection<Point>();
      data.Add(new Point(0, 3));
      data.Add(new Point(1, -4));
      data.Add(new Point(2, -2.3));
      data.Add(new Point(3, 34));
      data.Add(new Point(4, 12.4));

      series.ItemsSource = data;

      Assert.AreEqual(5, series.DataPoints.Count);
      Assert.AreEqual(4, chart.XAxis.Maximum);
      // Add another piece of data to the collection:
      data.Add(new Point(5, 25));
      // This should update the maximum XAxis value:
      Assert.AreEqual(5, chart.XAxis.Maximum);

      Assert.AreEqual(0, chart.XAxis.Minimum);
      // Add another piece of data with a negative XAxis value:
      data.Insert(0, new Point(-2, 3));
      // This should update the minimum XAxis value:
      Assert.AreEqual(-2, chart.XAxis.Minimum);
    }

    [Test]
    [Ignore("This test needs to be revised")]
    public void AddingDataToItemsSourceUpdatesYAXis()
    {
      Chart chart = new Chart();
      SimpleDataSeries series = new SimpleDataSeries();
      chart.Series.Add(series);

      // Abitry data
      IList data = new ObservableCollection<Point>();
      data.Add(new Point(0, 3));
      data.Add(new Point(1, -4));
      data.Add(new Point(2, -2.3));
      data.Add(new Point(3, 34));
      data.Add(new Point(4, 12.4));

      series.ItemsSource = data;

      Assert.AreEqual(5, series.DataPoints.Count);
      Assert.AreEqual(35, chart.YAxis.Maximum);
      // Add another piece of data to the collection:
      data.Add(new Point(5, 47));
      // This should update the maximum YAxis value:
      Assert.AreEqual(48, chart.YAxis.Maximum);

      Assert.AreEqual(-5, chart.YAxis.Minimum);
      // Add another piece of data with a negative YAxis value:
      data.Add(new Point(6, -8));
      // This should update the minimum YAXis value.
      Assert.AreEqual(-9, chart.YAxis.Minimum);
    }

    [Test]
    [Ignore("Performance improvements prevents this from working in the test environment")]
    public void AddingDataToItemsSourceDoesNotModifyOtherSeries()
    {
      Chart chart = new Chart();
      SimpleDataSeries series1 = new SimpleDataSeries();
      SimpleDataSeries series2 = new SimpleDataSeries();
      chart.Series.Add(series1);
      chart.Series.Add(series2);

      // Abitry data
      IList data1 = new ObservableCollection<Point>();
      data1.Add(new Point(0, 3));
      data1.Add(new Point(1, -4));
      data1.Add(new Point(2, -2.3));
      data1.Add(new Point(3, 34));
      data1.Add(new Point(4, 12.4));

      // Abitry data
      IList data2 = new ObservableCollection<Point>();
      data2.Add(new Point(0, -6));
      data2.Add(new Point(1, 4));
      data2.Add(new Point(2, 17.5));
      data2.Add(new Point(3, -9.6));
      data2.Add(new Point(4, -32.9));

      series1.ItemsSource = data1;
      series2.ItemsSource = data2;

      Assert.AreEqual(5, series1.DataPoints.Count);
      Assert.AreEqual(5, series2.DataPoints.Count);
      // Add another piece of data to the first data series:
      data1.Add(new Point(5, 17));
      // The first data series should now have 6 data points:
      Assert.AreEqual(6, series1.DataPoints.Count);
      // The second data series should still have 5 data points:
      Assert.AreEqual(5, series2.DataPoints.Count);
      // And the important thing is that the canvas has 11 children.
      // This shows that by updating the first data series, the second data series is not cleared from the canvas: (does not get updated in a test environment)
      //Assert.AreEqual(11, series1.Canvas.Children.Count);
    }

    // TODO: This no longer works due to performance optimizations:
    /*[Test]
    public void ChangingPropertyOfDataObjectUpdatesChart()
    {
      Chart chart = new Chart();
      SimpleDataSeries series = new SimpleDataSeries();
      series.XBinding = new Binding("String");
      series.YBinding = new Binding("Double");
      chart.Series.Add(series);

      // Abitry data
      IList data = new ObservableCollection<StringDouble>();
      data.Add(new StringDouble("Jan", 3));
      data.Add(new StringDouble("Feb", -4));
      data.Add(new StringDouble("Mar", -2.3));
      data.Add(new StringDouble("Apr", 34));
      data.Add(new StringDouble("May", 12.4));

      series.ItemsSource = data;

      Assert.AreEqual(5, series.DataPoints.Count);
      // Change the Double property of the last StringDouble:
      StringDouble sd = data[4] as StringDouble;
      sd.Double = 9;
      // The data series should still have 5 data points:
      Assert.AreEqual(5, series.DataPoints.Count);

      CartesianDataPoint point = series.DataPoints[4];
      Assert.AreEqual(9, point.LogicalPoint.Y);
    }*/

    [Test]
    [Ignore("This test needs to be revised")]
    public void ChangingPropertyOfDataObjectUpdatesXAxis()
    {
      Chart chart = new Chart();
      SimpleDataSeries series = new SimpleDataSeries();
      // This time the doubles are mapped along the X axis:
      series.XBinding = new Binding("Double");
      series.YBinding = new Binding("String");
      chart.Series.Add(series);

      // Abitry data
      IList data = new ObservableCollection<StringDouble>();
      data.Add(new StringDouble("Jan", 3));
      data.Add(new StringDouble("Feb", -4));
      data.Add(new StringDouble("Mar", -2.3));
      data.Add(new StringDouble("Apr", 34));
      data.Add(new StringDouble("May", 12.4));

      series.ItemsSource = data;

      Assert.AreEqual(5, series.DataPoints.Count);
      // The current maximum X axis value should be 34:
      Assert.AreEqual(34, chart.XAxis.Maximum);
      // Change the Double property of one of the StringDouble objects:
      StringDouble sd = data[3] as StringDouble;
      sd.Double = 20;
      // The maximum X axis value should now be 20 instead of 34:
      Assert.AreEqual(20, chart.XAxis.Maximum);

      Assert.AreEqual(-4, chart.XAxis.Minimum);
      // Change the Double property of one of the StringDouble objects:
      sd = data[0] as StringDouble;
      sd.Double = -13;
      // The minimum X axis value should now be -13 instead of -4:
      Assert.AreEqual(-13, chart.XAxis.Minimum);
    }

    [Test]
    [Ignore("This test needs to be revised")]
    public void ChangingPropertyOfDataObjectUpdatesYAxis()
    {
      Chart chart = new Chart();
      SimpleDataSeries series = new SimpleDataSeries();
      series.XBinding = new Binding("String");
      series.YBinding = new Binding("Double");
      chart.Series.Add(series);

      // Abitry data
      IList data = new ObservableCollection<StringDouble>();
      data.Add(new StringDouble("Jan", 3));
      data.Add(new StringDouble("Feb", -4));
      data.Add(new StringDouble("Mar", -2.3));
      data.Add(new StringDouble("Apr", 34));
      data.Add(new StringDouble("May", 12.4));

      series.ItemsSource = data;

      Assert.AreEqual(5, series.DataPoints.Count);
      // The current maximum Y axis value should be 35:
      Assert.AreEqual(35, chart.YAxis.Maximum);
      // Change the Double property of one of the StringDouble objects:
      StringDouble sd = data[3] as StringDouble;
      sd.Double = 20;
      // The maximum Y axis value should now be 21 instead of 35:
      Assert.AreEqual(21, chart.YAxis.Maximum);

      Assert.AreEqual(-5, chart.YAxis.Minimum);
      // Change the Double property of one of the StringDouble objects:
      sd = data[0] as StringDouble;
      sd.Double = -13;
      // The minimum Y axis value should now be -14 instead of -5:
      Assert.AreEqual(-14, chart.YAxis.Minimum);
    }

    [Test]
    [Ignore("This test needs to be revised")]
    public void ChangingPropertyOfDataObjectDoesNotModifyOtherSeries()
    {
      Chart chart = new Chart();
      SimpleDataSeries series1 = new SimpleDataSeries();
      series1.XBinding = new Binding("String");
      series1.YBinding = new Binding("Double");
      SimpleDataSeries series2 = new SimpleDataSeries();
      series2.XBinding = new Binding("String");
      series2.YBinding = new Binding("Double");
      chart.Series.Add(series1);
      chart.Series.Add(series2);

      // Abitry data
      IList data1 = new ObservableCollection<StringDouble>();
      data1.Add(new StringDouble("Jan", 3));
      data1.Add(new StringDouble("Feb", -4));
      data1.Add(new StringDouble("Mar", -2.3));
      data1.Add(new StringDouble("Apr", 34));
      data1.Add(new StringDouble("May", 12.4));

      // Abitry data
      IList data2 = new ObservableCollection<StringDouble>();
      data2.Add(new StringDouble("Jan", -6));
      data2.Add(new StringDouble("Feb", 4));
      data2.Add(new StringDouble("Mar", 17.5));
      data2.Add(new StringDouble("Apr", -9.6));
      data2.Add(new StringDouble("May", -32.9));

      series1.ItemsSource = data1;
      series2.ItemsSource = data2;

      Assert.AreEqual(5, series1.DataPoints.Count);
      Assert.AreEqual(5, series2.DataPoints.Count);
      // Change the Double property of one of the StringDouble objects in the first data series:
      StringDouble sd = data1[4] as StringDouble;
      sd.Double = 9;

      // Both data series should still have 5 data point objects:
      Assert.AreEqual(5, series1.DataPoints.Count);
      Assert.AreEqual(5, series2.DataPoints.Count);
      // And the important thing is that the canvas still has 10 children.
      // This shows that by updating the first data series, the second data series is not cleared from the canvas:
      Assert.AreEqual(10, series1.Canvas.Children.Count);
    }

    // TODO: this no longer works due to performance optimizations:
    /*[Test]
    public void ReplacingItemsSourceUpdatesChart()
    {
      Chart chart = new Chart();
      SimpleDataSeries series = new SimpleDataSeries();
      chart.Series.Add(series);

      // Abitry data
      IList data = new ObservableCollection<Point>();
      data.Add(new Point(0, 3));
      data.Add(new Point(1, -4));
      data.Add(new Point(2, -2.3));
      data.Add(new Point(3, 34));
      data.Add(new Point(4, 12.4));

      // Abitry data
      IList replacedData = new ObservableCollection<Point>();
      replacedData.Add(new Point(0, -4));
      replacedData.Add(new Point(1, 12));
      replacedData.Add(new Point(2, 6.5));
      replacedData.Add(new Point(3, 13.1));
      replacedData.Add(new Point(4, 4.72));

      series.ItemsSource = data;

      Assert.AreEqual(5, series.DataPoints.Count);
      // Replace the items source of the data series:
      series.ItemsSource = replacedData;
      // There should be 5 data point objects generated:
      Assert.AreEqual(5, series.DataPoints.Count);
      // And also the canvas should have 5 children:
      Assert.AreEqual(5, series.Canvas.Children.Count);

      // The data points generated by the series should now be updated based on the replaced data:
      int index = 0;
      foreach (CartesianDataPoint point in series.DataPoints)
      {
        Point p = (Point)series.ItemsSource[index];
        Assert.AreEqual(p.X, point.LogicalPoint.X);
        Assert.AreEqual(p.Y, point.LogicalPoint.Y);
        index++;
      }
    }*/

    [Test]
    [Ignore("This test needs to be revised")]
    public void ReplacingItemsSourceUpdatesXAxis()
    {
      Chart chart = new Chart();
      SimpleDataSeries series = new SimpleDataSeries();
      chart.Series.Add(series);

      // Abitry data
      IList data = new ObservableCollection<Point>();
      data.Add(new Point(0, 3));
      data.Add(new Point(1, -4));
      data.Add(new Point(2, -2.3));
      data.Add(new Point(3, 34));
      data.Add(new Point(4, 12.4));

      // Abitry data
      IList replacedData = new ObservableCollection<Point>();
      replacedData.Add(new Point(-2, -4));
      replacedData.Add(new Point(0, 12));
      replacedData.Add(new Point(2, 6.5));
      replacedData.Add(new Point(4, 13.1));
      replacedData.Add(new Point(6, 4.72));

      series.ItemsSource = data;

      Assert.AreEqual(5, series.DataPoints.Count);
      Assert.AreEqual(4, chart.XAxis.Maximum);
      Assert.AreEqual(0, chart.XAxis.Minimum);
      // Replace the items source of the data series:
      series.ItemsSource = replacedData;
      // The minimum and maximum X axis values should be updated based on the replaced data:
      Assert.AreEqual(6, chart.XAxis.Maximum);
      Assert.AreEqual(-2, chart.XAxis.Minimum);
    }

    [Test]
    [Ignore("This test needs to be revised")]
    public void ReplacingItemsSourceUpdatesYAxis()
    {
      Chart chart = new Chart();
      SimpleDataSeries series = new SimpleDataSeries();
      chart.Series.Add(series);

      // Abitry data
      IList data = new ObservableCollection<Point>();
      data.Add(new Point(0, 3));
      data.Add(new Point(1, -4));
      data.Add(new Point(2, -2.3));
      data.Add(new Point(3, 34));
      data.Add(new Point(4, 12.4));

      // Abitry data
      IList replacedData = new ObservableCollection<Point>();
      replacedData.Add(new Point(0, -4));
      replacedData.Add(new Point(1, 12));
      replacedData.Add(new Point(2, 21));
      replacedData.Add(new Point(3, 13.1));
      replacedData.Add(new Point(4, -18));

      series.ItemsSource = data;

      Assert.AreEqual(5, series.DataPoints.Count);
      Assert.AreEqual(35, chart.YAxis.Maximum);
      Assert.AreEqual(-5, chart.YAxis.Minimum);
      // Replace the items source of the data series:
      series.ItemsSource = replacedData;
      // The minimum and maximum Y axis values should be updated based on the replaced data:
      Assert.AreEqual(22, chart.YAxis.Maximum);
      Assert.AreEqual(-19, chart.YAxis.Minimum);
    }

    [Test]
    [Ignore("This test needs to be revised")]
    public void ReplacingItemsSourceDoesNotModifyOtherSeries()
    {
      Chart chart = new Chart();
      SimpleDataSeries series1 = new SimpleDataSeries();
      SimpleDataSeries series2 = new SimpleDataSeries();
      chart.Series.Add(series1);
      chart.Series.Add(series2);

      // Abitry data
      IList data1 = new ObservableCollection<Point>();
      data1.Add(new Point(0, 3));
      data1.Add(new Point(1, -4));
      data1.Add(new Point(2, -2.3));
      data1.Add(new Point(3, 34));
      data1.Add(new Point(4, 12.4));

      // Abitry data
      IList data2 = new ObservableCollection<Point>();
      data2.Add(new Point(0, -6));
      data2.Add(new Point(1, 4));
      data2.Add(new Point(2, 17.5));
      data2.Add(new Point(3, -9.6));
      data2.Add(new Point(4, -32.9));

      // Abitry data
      IList replacedData = new ObservableCollection<Point>();
      replacedData.Add(new Point(0, -4));
      replacedData.Add(new Point(1, 12));
      replacedData.Add(new Point(2, 6.5));
      replacedData.Add(new Point(3, 13.1));
      replacedData.Add(new Point(4, 4.72));

      series1.ItemsSource = data1;
      series2.ItemsSource = data2;

      Assert.AreEqual(5, series1.DataPoints.Count);
      // Replace the items source of the data series:
      series1.ItemsSource = replacedData;
      // Both series should still have 5 data points:
      Assert.AreEqual(5, series1.DataPoints.Count);
      Assert.AreEqual(5, series2.DataPoints.Count);
      // And the important thing is that the canvas still has 10 children.
      // This shows that by updating the first data series, the second data series is not cleared from the canvas:
      Assert.AreEqual(10, series1.Canvas.Children.Count);
    }

    #endregion // Chart tests

    #region PieChart tests

    [Test]
    [STAThread]
    public void AddingDataToItemsSourceUpdatesChart_PieChart()
    {
      PieChart chart = new PieChart();
      PieSeries series = new PieSeries();
      chart.Series.Add(series);

      // Abitry data
      IList data = new ObservableCollection<object>();
      data.Add(3);
      data.Add(-4);
      data.Add(-2.3);
      data.Add(34);
      data.Add(12.4);

      series.ItemsSource = data;

      Assert.AreEqual(5, series.DataPoints.Count);
      // Add another piece of data to the collection:
      data.Add(25);
      // This should increase the number of DataPoint controls generated:
      Assert.AreEqual(6, series.DataPoints.Count);
      // And also the number of items in the chart canvas:
      Assert.AreEqual(6, series.Canvas.Children.Count);
    }

    [Test]
    [STAThread]
    public void AddingDataToItemsSourceDoesNotModifyOtherSeries_PieChart()
    {
      PieChart chart = new PieChart();
      PieSeries series1 = new PieSeries();
      PieSeries series2 = new PieSeries();
      chart.Series.Add(series1);
      chart.Series.Add(series2);

      // Abitry data
      IList data1 = new ObservableCollection<object>();
      data1.Add(3);
      data1.Add(-4);
      data1.Add(-2.3);
      data1.Add(34);
      data1.Add(12.4);

      // Abitry data
      IList data2 = new ObservableCollection<object>();
      data2.Add(-6);
      data2.Add(4);
      data2.Add(17.5);
      data2.Add(-9.6);
      data2.Add(-32.9);

      series1.ItemsSource = data1;
      series2.ItemsSource = data2;

      Assert.AreEqual(5, series1.DataPoints.Count);
      Assert.AreEqual(5, series2.DataPoints.Count);
      // Add another piece of data to the first data series:
      data1.Add(17);
      // The first data series should now have 6 data points:
      Assert.AreEqual(6, series1.DataPoints.Count);
      // The second data series should still have 5 data points:
      Assert.AreEqual(5, series2.DataPoints.Count);
      // And the important thing is that the canvas has 11 children.
      // This shows that by updating the first data series, the second data series is not cleared from the canvas:
      Assert.AreEqual(11, series1.Canvas.Children.Count);
    }

    [Test]
    [STAThread]
    public void ChangingPropertyOfDataObjectUpdatesChart_PieChart()
    {
      PieChart chart = new PieChart();
      PieSeries series = new PieSeries();
      series.TitleBinding = new Binding("String");
      series.DataBinding = new Binding("Double");
      chart.Series.Add(series);

      // Abitry data
      IList data = new ObservableCollection<StringDouble>();
      data.Add(new StringDouble("Jan", 3));
      data.Add(new StringDouble("Feb", -4));
      data.Add(new StringDouble("Mar", -2.3));
      data.Add(new StringDouble("Apr", 34));
      data.Add(new StringDouble("May", 12.4));

      series.ItemsSource = data;

      Assert.AreEqual(5, series.DataPoints.Count);
      // Change the Double property of the last StringDouble:
      StringDouble sd = data[4] as StringDouble;
      sd.Double = 9;
      // The data series should still have 5 data points:
      Assert.AreEqual(5, series.DataPoints.Count);

      PieSlice point = series.DataPoints[4];
      Assert.AreEqual(9, point.DataValue);
    }

    [Test]
    [STAThread]
    public void ChangingPropertyOfDataObjectDoesNotModifyOtherSeries_PieChart()
    {
      PieChart chart = new PieChart();
      PieSeries series1 = new PieSeries();
      series1.TitleBinding = new Binding("String");
      series1.DataBinding = new Binding("Double");
      PieSeries series2 = new PieSeries();
      series2.TitleBinding = new Binding("String");
      series2.DataBinding = new Binding("Double");
      chart.Series.Add(series1);
      chart.Series.Add(series2);

      // Abitry data
      IList data1 = new ObservableCollection<StringDouble>();
      data1.Add(new StringDouble("Jan", 3));
      data1.Add(new StringDouble("Feb", -4));
      data1.Add(new StringDouble("Mar", -2.3));
      data1.Add(new StringDouble("Apr", 34));
      data1.Add(new StringDouble("May", 12.4));

      // Abitry data
      IList data2 = new ObservableCollection<StringDouble>();
      data2.Add(new StringDouble("Jan", -6));
      data2.Add(new StringDouble("Feb", 4));
      data2.Add(new StringDouble("Mar", 17.5));
      data2.Add(new StringDouble("Apr", -9.6));
      data2.Add(new StringDouble("May", -32.9));

      series1.ItemsSource = data1;
      series2.ItemsSource = data2;

      Assert.AreEqual(5, series1.DataPoints.Count);
      Assert.AreEqual(5, series2.DataPoints.Count);
      // Change the Double property of one of the StringDouble objects in the first data series:
      StringDouble sd = data1[4] as StringDouble;
      sd.Double = 9;

      // Both data series should still have 5 data point objects:
      Assert.AreEqual(5, series1.DataPoints.Count);
      Assert.AreEqual(5, series2.DataPoints.Count);
      // And the important thing is that the canvas still has 10 children.
      // This shows that by updating the first data series, the second data series is not cleared from the canvas:
      Assert.AreEqual(10, series1.Canvas.Children.Count);
    }

    [Test]
    [STAThread]
    public void ReplacingItemsSourceUpdatesChart_PieChart()
    {
      PieChart chart = new PieChart();
      PieSeries series = new PieSeries();
      chart.Series.Add(series);

      // Abitry data
      IList data = new ObservableCollection<object>();
      data.Add(3.0);
      data.Add(-4.0);
      data.Add(-2.3);
      data.Add(34.0);
      data.Add(12.4);

      // Abitry data
      IList replacedData = new ObservableCollection<object>();
      replacedData.Add(-4.0);
      replacedData.Add(12.0);
      replacedData.Add(6.5);
      replacedData.Add(13.1);
      replacedData.Add(4.72);

      series.ItemsSource = data;

      Assert.AreEqual(5, series.DataPoints.Count);
      // Replace the items source of the data series:
      series.ItemsSource = replacedData;
      // There should be 5 data point objects generated:
      Assert.AreEqual(5, series.DataPoints.Count);
      // And also the canvas should have 5 children:
      Assert.AreEqual(5, series.Canvas.Children.Count);

      // The data points generated by the series should now be updated based on the replaced data:
      int index = 0;
      foreach (PieSlice point in series.DataPoints)
      {
        double d = (double)series.ItemsSource[index];
        Assert.AreEqual(d, point.DataValue);
        Assert.AreEqual(d, point.DataValue);
        index++;
      }
    }

    [Test]
    [STAThread]
    public void ReplacingItemsSourceDoesNotModifyOtherSeries_PieChart()
    {
      PieChart chart = new PieChart();
      PieSeries series1 = new PieSeries();
      PieSeries series2 = new PieSeries();
      chart.Series.Add(series1);
      chart.Series.Add(series2);

      // Abitry data
      IList data1 = new ObservableCollection<object>();
      data1.Add(3);
      data1.Add(-4);
      data1.Add(-2.3);
      data1.Add(34);
      data1.Add(12.4);

      // Abitry data
      IList data2 = new ObservableCollection<object>();
      data2.Add(-6);
      data2.Add(4);
      data2.Add(17.5);
      data2.Add(-9.6);
      data2.Add(-32.9);

      // Abitry data
      IList replacedData = new ObservableCollection<object>();
      replacedData.Add(-4);
      replacedData.Add(12);
      replacedData.Add(6.5);
      replacedData.Add(13.1);
      replacedData.Add(4.72);

      series1.ItemsSource = data1;
      series2.ItemsSource = data2;

      Assert.AreEqual(5, series1.DataPoints.Count);
      // Replace the items source of the data series:
      series1.ItemsSource = replacedData;
      // Both series should still have 5 data points:
      Assert.AreEqual(5, series1.DataPoints.Count);
      Assert.AreEqual(5, series2.DataPoints.Count);
      // And the important thing is that the canvas still has 10 children.
      // This shows that by updating the first data series, the second data series is not cleared from the canvas:
      Assert.AreEqual(10, series1.Canvas.Children.Count);
    }

    #endregion // PieChart tests
  }
}
