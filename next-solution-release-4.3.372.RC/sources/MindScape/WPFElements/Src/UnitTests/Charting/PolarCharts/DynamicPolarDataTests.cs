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
using Mindscape.WpfElements.Charting;
using System.Windows.Data;
using NUnit.Framework;

namespace Mindscape.WpfElements.UnitTests
{
  [TestFixture]
  public class DynamicPolarDataTests
  {
    // NOTES:
    // The automatic minimum and maximum calculations for a theta axis works differently to other axes.
    // As the data is being analysed, the difference between each consecutive theta value is calculated.
    // If this theta difference is consistant throughout the whole data set, this difference will be added to the maximum theta value to calculate the maximum theta axis value.
    // This is because the theta axis is circular and the minimum and maximum are both pointing due north.
    // So adding this extra spacing prevents overlaps for the first and last data points, and makes the data well spread.
    // If however the theta difference is inconsistant (such as in a scatter series or buble series),
    // then the maximum theta axis value will just be the maximum theta data value. This occurs in at least one of these tests.

    [Test]
    [STAThread]
    public void AddingDataToItemsSourceUpdatesPolarChart()
    {
      PolarChart chart = new PolarChart();
      SimplePolarDataSeries series = new SimplePolarDataSeries();
      chart.Series.Add(series);

      // Abitry data
      IList data = new ObservableCollection<PolarPoint>();
      data.Add(new PolarPoint(0, 3));
      data.Add(new PolarPoint(1, -4));
      data.Add(new PolarPoint(2, -2.3));
      data.Add(new PolarPoint(3, 34));
      data.Add(new PolarPoint(4, 12.4));

      series.ItemsSource = data;

      Assert.AreEqual(5, series.DataPoints.Count);
      // Add another piece of data to the collection:
      data.Add(new PolarPoint(5, 25));
      // This should increase the number of DataPoint controls generated:
      Assert.AreEqual(6, series.DataPoints.Count);
      // And also the number of items in the chart canvas:
      Assert.AreEqual(6, series.Canvas.Children.Count);
    }

    [Test]
    [STAThread]
    public void AddingDataToItemsSourceUpdatesThetaAxis()
    {
      PolarChart chart = new PolarChart();
      SimplePolarDataSeries series = new SimplePolarDataSeries();
      chart.Series.Add(series);

      // Abitry data
      IList data = new ObservableCollection<PolarPoint>();
      data.Add(new PolarPoint(0, 3));
      data.Add(new PolarPoint(1, -4));
      data.Add(new PolarPoint(2, -2.3));
      data.Add(new PolarPoint(3, 34));
      data.Add(new PolarPoint(4, 12.4));

      series.ItemsSource = data;

      Assert.AreEqual(5, series.DataPoints.Count);
      Assert.AreEqual(5, chart.ThetaAxis.Maximum);
      // Add another piece of data to the collection:
      data.Add(new PolarPoint(5, 25));
      // This should update the maximum ThetaAxis value:
      Assert.AreEqual(6, chart.ThetaAxis.Maximum);

      Assert.AreEqual(0, chart.ThetaAxis.Minimum);
      // Add another piece of data with a negative ThetaAxis value:
      data.Insert(0, new PolarPoint(-2, 3));
      // This should update the minimum ThetaAxis value:
      Assert.AreEqual(-2, chart.ThetaAxis.Minimum);
    }

    [Test]
    [STAThread]
    public void AddingDataToItemsSourceUpdatesRhoAXis()
    {
      PolarChart chart = new PolarChart();
      SimplePolarDataSeries series = new SimplePolarDataSeries();
      chart.Series.Add(series);

      // Abitry data
      IList data = new ObservableCollection<PolarPoint>();
      data.Add(new PolarPoint(0, 3));
      data.Add(new PolarPoint(1, -4));
      data.Add(new PolarPoint(2, -2.3));
      data.Add(new PolarPoint(3, 34));
      data.Add(new PolarPoint(4, 12.4));

      series.ItemsSource = data;

      Assert.AreEqual(5, series.DataPoints.Count);
      Assert.AreEqual(35, chart.RhoAxis.Maximum);
      // Add another piece of data to the collection:
      data.Add(new PolarPoint(5, 47));
      // This should update the maximum RhoAxis value:
      Assert.AreEqual(48, chart.RhoAxis.Maximum);

      Assert.AreEqual(-5, chart.RhoAxis.Minimum);
      // Add another piece of data with a negative RhoAxis value:
      data.Add(new PolarPoint(6, -8));
      // This should update the minimum YAXis value.
      Assert.AreEqual(-9, chart.RhoAxis.Minimum);
    }

    [Test]
    [STAThread]
    public void AddingDataToItemsSourceDoesNotModifyOtherPolarSeries()
    {
      PolarChart chart = new PolarChart();
      SimplePolarDataSeries series1 = new SimplePolarDataSeries();
      SimplePolarDataSeries series2 = new SimplePolarDataSeries();
      chart.Series.Add(series1);
      chart.Series.Add(series2);

      // Abitry data
      IList data1 = new ObservableCollection<PolarPoint>();
      data1.Add(new PolarPoint(0, 3));
      data1.Add(new PolarPoint(1, -4));
      data1.Add(new PolarPoint(2, -2.3));
      data1.Add(new PolarPoint(3, 34));
      data1.Add(new PolarPoint(4, 12.4));

      // Abitry data
      IList data2 = new ObservableCollection<PolarPoint>();
      data2.Add(new PolarPoint(0, -6));
      data2.Add(new PolarPoint(1, 4));
      data2.Add(new PolarPoint(2, 17.5));
      data2.Add(new PolarPoint(3, -9.6));
      data2.Add(new PolarPoint(4, -32.9));

      series1.ItemsSource = data1;
      series2.ItemsSource = data2;

      Assert.AreEqual(5, series1.DataPoints.Count);
      Assert.AreEqual(5, series2.DataPoints.Count);
      // Add another piece of data to the first data series:
      data1.Add(new PolarPoint(5, 17));
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
    public void ChangingPropertyOfDataObjectUpdatesPolarChart()
    {
      PolarChart chart = new PolarChart();
      SimplePolarDataSeries series = new SimplePolarDataSeries();
      series.ThetaBinding = new Binding("String");
      series.RhoBinding = new Binding("Double");
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

      PolarDataPoint point = series.DataPoints[4];
      Assert.AreEqual(9, point.LogicalPoint.Rho);
    }

    [Test]
    [STAThread]
    public void ChangingPropertyOfDataObjectUpdatesThetaAxis()
    {
      PolarChart chart = new PolarChart();
      SimplePolarDataSeries series = new SimplePolarDataSeries();
      // This time the doubles are mapped along the theta axis:
      series.ThetaBinding = new Binding("Double");
      series.RhoBinding = new Binding("String");
      chart.Series.Add(series);

      // Abitry data
      IList data = new ObservableCollection<StringDouble>();
      data.Add(new StringDouble("Jan", 0));
      data.Add(new StringDouble("Feb", 5));
      data.Add(new StringDouble("Mar", 20));
      data.Add(new StringDouble("Apr", 15));
      data.Add(new StringDouble("May", 10));

      series.ItemsSource = data;

      Assert.AreEqual(5, series.DataPoints.Count);
      // The current maximum theta axis value should be 20:
      Assert.AreEqual(20, chart.ThetaAxis.Maximum);
      // Change the Double property of one of the StringDouble objects:
      StringDouble sd = data[2] as StringDouble;
      sd.Double = 13;
      // The maximum theta axis value should now be 15 instead of 20:
      Assert.AreEqual(15, chart.ThetaAxis.Maximum);

      Assert.AreEqual(0, chart.ThetaAxis.Minimum);
      // Change the Double property of one of the StringDouble objects:
      sd = data[0] as StringDouble;
      sd.Double = -13;
      // The minimum theta axis value should now be -13 instead of 0:
      Assert.AreEqual(-13, chart.ThetaAxis.Minimum);
    }

    [Test]
    [STAThread]
    public void ChangingPropertyOfDataObjectUpdatesRhoAxis()
    {
      PolarChart chart = new PolarChart();
      SimplePolarDataSeries series = new SimplePolarDataSeries();
      series.ThetaBinding = new Binding("String");
      series.RhoBinding = new Binding("Double");
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
      // The current maximum tho axis value should be 35:
      Assert.AreEqual(35, chart.RhoAxis.Maximum);
      // Change the Double property of one of the StringDouble objects:
      StringDouble sd = data[3] as StringDouble;
      sd.Double = 20;
      // The maximum rho axis value should now be 21 instead of 35:
      Assert.AreEqual(21, chart.RhoAxis.Maximum);

      Assert.AreEqual(-5, chart.RhoAxis.Minimum);
      // Change the Double property of one of the StringDouble objects:
      sd = data[0] as StringDouble;
      sd.Double = -13;
      // The minimum rho axis value should now be -14 instead of -5:
      Assert.AreEqual(-14, chart.RhoAxis.Minimum);
    }

    [Test]
    [STAThread]
    public void ChangingPropertyOfDataObjectDoesNotModifyOtherPolarSeries()
    {
      PolarChart chart = new PolarChart();
      SimplePolarDataSeries series1 = new SimplePolarDataSeries();
      series1.ThetaBinding = new Binding("String");
      series1.RhoBinding = new Binding("Double");
      SimplePolarDataSeries series2 = new SimplePolarDataSeries();
      series2.ThetaBinding = new Binding("String");
      series2.RhoBinding = new Binding("Double");
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
    public void ReplacingItemsSourceUpdatesPolarChart()
    {
      PolarChart chart = new PolarChart();
      SimplePolarDataSeries series = new SimplePolarDataSeries();
      chart.Series.Add(series);

      // Abitry data
      IList data = new ObservableCollection<PolarPoint>();
      data.Add(new PolarPoint(0, 3));
      data.Add(new PolarPoint(1, -4));
      data.Add(new PolarPoint(2, -2.3));
      data.Add(new PolarPoint(3, 34));
      data.Add(new PolarPoint(4, 12.4));

      // Abitry data
      IList replacedData = new ObservableCollection<PolarPoint>();
      replacedData.Add(new PolarPoint(0, -4));
      replacedData.Add(new PolarPoint(1, 12));
      replacedData.Add(new PolarPoint(2, 6.5));
      replacedData.Add(new PolarPoint(3, 13.1));
      replacedData.Add(new PolarPoint(4, 4.72));

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
      foreach (PolarDataPoint point in series.DataPoints)
      {
        PolarPoint p = (PolarPoint)series.ItemsSource[index];
        Assert.AreEqual(p.Theta, point.LogicalPoint.Theta);
        Assert.AreEqual(p.Rho, point.LogicalPoint.Rho);
        index++;
      }
    }

    [Test]
    [STAThread]
    public void ReplacingItemsSourceUpdatesThetaAxis()
    {
      PolarChart chart = new PolarChart();
      SimplePolarDataSeries series = new SimplePolarDataSeries();
      chart.Series.Add(series);

      // Abitry data
      IList data = new ObservableCollection<PolarPoint>();
      data.Add(new PolarPoint(0, 3));
      data.Add(new PolarPoint(1, -4));
      data.Add(new PolarPoint(2, -2.3));
      data.Add(new PolarPoint(3, 34));
      data.Add(new PolarPoint(4, 12.4));

      // Abitry data
      IList replacedData = new ObservableCollection<PolarPoint>();
      replacedData.Add(new PolarPoint(-2, -4));
      replacedData.Add(new PolarPoint(0, 12));
      replacedData.Add(new PolarPoint(2, 6.5));
      replacedData.Add(new PolarPoint(4, 13.1));
      replacedData.Add(new PolarPoint(6, 4.72));

      series.ItemsSource = data;

      Assert.AreEqual(5, series.DataPoints.Count);
      Assert.AreEqual(5, chart.ThetaAxis.Maximum);
      Assert.AreEqual(0, chart.ThetaAxis.Minimum);
      // Replace the items source of the data series:
      series.ItemsSource = replacedData;
      // The minimum and maximum theta axis values should be updated based on the replaced data:
      Assert.AreEqual(8, chart.ThetaAxis.Maximum);
      Assert.AreEqual(-2, chart.ThetaAxis.Minimum);
    }

    [Test]
    [STAThread]
    public void ReplacingItemsSourceUpdatesRhoAxis()
    {
      PolarChart chart = new PolarChart();
      SimplePolarDataSeries series = new SimplePolarDataSeries();
      chart.Series.Add(series);

      // Abitry data
      IList data = new ObservableCollection<PolarPoint>();
      data.Add(new PolarPoint(0, 3));
      data.Add(new PolarPoint(1, -4));
      data.Add(new PolarPoint(2, -2.3));
      data.Add(new PolarPoint(3, 34));
      data.Add(new PolarPoint(4, 12.4));

      // Abitry data
      IList replacedData = new ObservableCollection<PolarPoint>();
      replacedData.Add(new PolarPoint(0, -4));
      replacedData.Add(new PolarPoint(1, 12));
      replacedData.Add(new PolarPoint(2, 21));
      replacedData.Add(new PolarPoint(3, 13.1));
      replacedData.Add(new PolarPoint(4, -18));

      series.ItemsSource = data;

      Assert.AreEqual(5, series.DataPoints.Count);
      Assert.AreEqual(35, chart.RhoAxis.Maximum);
      Assert.AreEqual(-5, chart.RhoAxis.Minimum);
      // Replace the items source of the data series:
      series.ItemsSource = replacedData;
      // The minimum and maximum rho axis values should be updated based on the replaced data:
      Assert.AreEqual(22, chart.RhoAxis.Maximum);
      Assert.AreEqual(-19, chart.RhoAxis.Minimum);
    }

    [Test]
    [STAThread]
    public void ReplacingItemsSourceDoesNotModifyOtherPolarSeries()
    {
      PolarChart chart = new PolarChart();
      SimplePolarDataSeries series1 = new SimplePolarDataSeries();
      SimplePolarDataSeries series2 = new SimplePolarDataSeries();
      chart.Series.Add(series1);
      chart.Series.Add(series2);

      // Abitry data
      IList data1 = new ObservableCollection<PolarPoint>();
      data1.Add(new PolarPoint(0, 3));
      data1.Add(new PolarPoint(1, -4));
      data1.Add(new PolarPoint(2, -2.3));
      data1.Add(new PolarPoint(3, 34));
      data1.Add(new PolarPoint(4, 12.4));

      // Abitry data
      IList data2 = new ObservableCollection<PolarPoint>();
      data2.Add(new PolarPoint(0, -6));
      data2.Add(new PolarPoint(1, 4));
      data2.Add(new PolarPoint(2, 17.5));
      data2.Add(new PolarPoint(3, -9.6));
      data2.Add(new PolarPoint(4, -32.9));

      // Abitry data
      IList replacedData = new ObservableCollection<PolarPoint>();
      replacedData.Add(new PolarPoint(0, -4));
      replacedData.Add(new PolarPoint(1, 12));
      replacedData.Add(new PolarPoint(2, 6.5));
      replacedData.Add(new PolarPoint(3, 13.1));
      replacedData.Add(new PolarPoint(4, 4.72));

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
  }
}
