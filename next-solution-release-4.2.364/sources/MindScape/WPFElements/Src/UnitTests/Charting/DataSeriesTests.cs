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
using System.Windows.Data;
using Mindscape.WpfElements.Charting;
using NUnit.Framework;
using System.Collections.ObjectModel;

namespace Mindscape.WpfElements.UnitTests
{
  [TestFixture]
  public class DataSeriesTests
  {
    [Test]
    [Ignore("This test needs to be revised")]
    public void CanNotSelectIfSelectionModeIsNone()
    {
      Chart chart = new Chart();
      chart.XAxis = new ChartAxis();
      chart.YAxis = new ChartAxis();
      BarSeries series = new BarSeries();
      // arbitrary data:
      IList data = new List<object>();
      data.Add(new Point(0, 2));
      data.Add(new Point(1, -4));
      data.Add(new Point(2, 13));
      chart.Series.Add(series);
      series.ItemsSource = data;

      series.SelectionMode = DataPointSelectionMode.None;

      int count = 0;
      foreach (UIElement element in series.Canvas.Children)
      {
        DataPoint dp = element as DataPoint;
        if (dp != null)
        {
          dp.IsSelected = true;
          Assert.AreEqual(dp.IsSelected, false); // The data point should fail to be selected.
          count++;
        }
      }
      Assert.AreEqual(count, data.Count); // Make sure we tested the same number of data points as the size of the data collection.
    }

    [Test]
    [Ignore("This test needs to be revised")]
    public void CanOnlySelectOneDataPointIfSelectionModeIsSingle()
    {
      Chart chart = new Chart();
      chart.XAxis = new ChartAxis();
      chart.YAxis = new ChartAxis();
      BarSeries series = new BarSeries();
      // arbitrary data:
      IList data = new List<object>();
      data.Add(new Point(0, 2));
      data.Add(new Point(1, -4));
      data.Add(new Point(2, 13));
      chart.Series.Add(series);
      series.ItemsSource = data;

      series.SelectionMode = DataPointSelectionMode.Single;

      // Get all the generated data points:
      List<DataPoint> dataPoints = new List<DataPoint>();
      foreach (UIElement element in series.Canvas.Children)
      {
        DataPoint dp = element as DataPoint;
        if (dp != null)
        {
          dataPoints.Add(dp);
        }
      }
      Assert.AreEqual(dataPoints.Count, data.Count); // Make sure we found the same number of data points as the size of the data collection.

      // By selecting a data point, only the selected data point should become selected:
      dataPoints[0].IsSelected = true;
      Assert.IsTrue(dataPoints[0].IsSelected);
      Assert.IsFalse(dataPoints[1].IsSelected);
      Assert.IsFalse(dataPoints[2].IsSelected);

      // By selecting a different data point, it should become selected, and the previous one should be deselected.
      dataPoints[2].IsSelected = true;
      Assert.IsFalse(dataPoints[0].IsSelected);
      Assert.IsFalse(dataPoints[1].IsSelected);
      Assert.IsTrue(dataPoints[2].IsSelected);
    }

    [Test]
    [Ignore("This test needs to be revised")]
    public void AllDataPointsAreSelectedIfSelectionModeIsAll()
    {
      Chart chart = new Chart();
      chart.XAxis = new ChartAxis();
      chart.YAxis = new ChartAxis();
      BarSeries series = new BarSeries();
      // arbitrary data:
      IList data = new List<object>();
      data.Add(new Point(0, 2));
      data.Add(new Point(1, -4));
      data.Add(new Point(2, 13));
      chart.Series.Add(series);
      series.ItemsSource = data;

      series.SelectionMode = DataPointSelectionMode.All;

      // Get all the generated data points:
      List<DataPoint> dataPoints = new List<DataPoint>();
      foreach (UIElement element in series.Canvas.Children)
      {
        DataPoint dp = element as DataPoint;
        if (dp != null)
        {
          dataPoints.Add(dp);
        }
      }
      Assert.AreEqual(dataPoints.Count, data.Count); // Make sure we found the same number of data points as the size of the data collection.

      // Selecting a data point will cause all data points to be selected.
      dataPoints[1].IsSelected = true;
      Assert.IsTrue(dataPoints[0].IsSelected);
      Assert.IsTrue(dataPoints[1].IsSelected);
      Assert.IsTrue(dataPoints[2].IsSelected);

      // Deselecting any data point will cause all data points to be deselected.
      dataPoints[2].IsSelected = false;
      Assert.IsFalse(dataPoints[0].IsSelected);
      Assert.IsFalse(dataPoints[1].IsSelected);
      Assert.IsFalse(dataPoints[2].IsSelected);
    }

    [Test]
    [Ignore("This test needs to be revised")]
    public void ChangingSelectionModeToNoneWillDeselectAllDataPoints()
    {
      Chart chart = new Chart();
      chart.XAxis = new ChartAxis();
      chart.YAxis = new ChartAxis();
      BarSeries series = new BarSeries();
      // arbitrary data:
      IList data = new List<object>();
      data.Add(new Point(0, 2));
      data.Add(new Point(1, -4));
      data.Add(new Point(2, 13));
      chart.Series.Add(series);
      series.ItemsSource = data;

      // Get all the generated data points:
      List<DataPoint> dataPoints = new List<DataPoint>();
      foreach (UIElement element in series.Canvas.Children)
      {
        DataPoint dp = element as DataPoint;
        if (dp != null)
        {
          dataPoints.Add(dp);
        }
      }
      Assert.AreEqual(dataPoints.Count, data.Count); // Make sure we found the same number of data points as the size of the data collection.

      // Set selection mode to Single and select a data point.
      series.SelectionMode = DataPointSelectionMode.Single;
      dataPoints[1].IsSelected = true;

      // Change selection mode to None. All data points should be deselected.
      series.SelectionMode = DataPointSelectionMode.None;
      Assert.IsFalse(dataPoints[0].IsSelected);
      Assert.IsFalse(dataPoints[1].IsSelected);
      Assert.IsFalse(dataPoints[2].IsSelected);

      // Set selection mode to All and select a data point (causeing all data points to be selected).
      series.SelectionMode = DataPointSelectionMode.All;
      dataPoints[1].IsSelected = true;

      // Change selection mode to None. All data points should be deselected.
      series.SelectionMode = DataPointSelectionMode.None;
      Assert.IsFalse(dataPoints[0].IsSelected);
      Assert.IsFalse(dataPoints[1].IsSelected);
      Assert.IsFalse(dataPoints[2].IsSelected);
    }

    [Test]
    [Ignore("This test needs to be revised")]
    public void ChangingSelectionModeToSingleWillRespectSelectionMode()
    {
      Chart chart = new Chart();
      chart.XAxis = new ChartAxis();
      chart.YAxis = new ChartAxis();
      BarSeries series = new BarSeries();
      // arbitrary data:
      IList data = new List<object>();
      data.Add(new Point(0, 2));
      data.Add(new Point(1, -4));
      data.Add(new Point(2, 13));
      chart.Series.Add(series);
      series.ItemsSource = data;

      // Get all the generated data points:
      List<DataPoint> dataPoints = new List<DataPoint>();
      foreach (UIElement element in series.Canvas.Children)
      {
        DataPoint dp = element as DataPoint;
        if (dp != null)
        {
          dataPoints.Add(dp);
        }
      }
      Assert.AreEqual(dataPoints.Count, data.Count); // Make sure we found the same number of data points as the size of the data collection.

      // Set selection mode to All and select a data point (causeing all data points to be selected).
      series.SelectionMode = DataPointSelectionMode.All;
      dataPoints[1].IsSelected = true;

      // Change selection mode to None. All data points should be deselected.
      series.SelectionMode = DataPointSelectionMode.None;
      Assert.IsFalse(dataPoints[0].IsSelected);
      Assert.IsFalse(dataPoints[1].IsSelected);
      Assert.IsFalse(dataPoints[2].IsSelected);
    }

    [Test]
    [Ignore("This test needs to be revised")]
    public void SelectedDataPointPropertyRemainsNullWhenSelectionModeIsNone()
    {
      Chart chart = new Chart();
      chart.XAxis = new ChartAxis();
      chart.YAxis = new ChartAxis();
      SimpleDataSeries series = new SimpleDataSeries();
      // arbitrary data:
      IList data = new List<object>();
      data.Add(new Point(0, 2));
      data.Add(new Point(1, -4));
      data.Add(new Point(2, 13));
      chart.Series.Add(series);
      series.ItemsSource = data;

      series.SelectionMode = DataPointSelectionMode.None;

      Assert.AreEqual(3, series.DataPoints.Count);
      // Attempt to select a data point. The SelectedDataPoint should remain null because the selection mode is none.
      series.DataPoints[1].IsSelected = true;
      Assert.IsNull(series.SelectedDataPoint);
    }

    [Test]
    [Ignore("This test needs to be revised")]
    public void SelectedDataPointPropertyIsSetWhenSelectionModeIsSingle()
    {
      Chart chart = new Chart();
      chart.XAxis = new ChartAxis();
      chart.YAxis = new ChartAxis();
      SimpleDataSeries series = new SimpleDataSeries();
      // arbitrary data:
      IList data = new List<object>();
      data.Add(new Point(0, 2));
      data.Add(new Point(1, -4));
      data.Add(new Point(2, 13));
      chart.Series.Add(series);
      series.ItemsSource = data;

      series.SelectionMode = DataPointSelectionMode.Single;

      Assert.AreEqual(3, series.DataPoints.Count);
      series.DataPoints[1].IsSelected = true;
      Assert.AreEqual(series.DataPoints[1], series.SelectedDataPoint);
      // select a different data point.
      series.DataPoints[0].IsSelected = true;
      // The SelectedDataPoint should now be the newly selected data point.
      Assert.AreEqual(series.DataPoints[0], series.SelectedDataPoint);
    }

    [Test]
    [Ignore("This test needs to be revised")]
    public void SelectedDataPointPropertyIsNullWhenDataPointIsDeselected_SingleSelectionMode()
    {
      Chart chart = new Chart();
      chart.XAxis = new ChartAxis();
      chart.YAxis = new ChartAxis();
      SimpleDataSeries series = new SimpleDataSeries();
      // arbitrary data:
      IList data = new List<object>();
      data.Add(new Point(0, 2));
      data.Add(new Point(1, -4));
      data.Add(new Point(2, 13));
      chart.Series.Add(series);
      series.ItemsSource = data;

      series.SelectionMode = DataPointSelectionMode.Single;

      Assert.AreEqual(3, series.DataPoints.Count);
      series.DataPoints[1].IsSelected = true;
      Assert.AreEqual(series.DataPoints[1], series.SelectedDataPoint);
      // deselect the currently selected data point.
      series.DataPoints[1].IsSelected = false;
      // This should cause the SelectedDataPoint property to be set to null.
      Assert.IsNull(series.SelectedDataPoint);
    }

    [Test]
    [Ignore("This test needs to be revised")]
    public void SelectedDataPointPropertyIsSetWhenSelectionModeIsAll()
    {
      Chart chart = new Chart();
      chart.XAxis = new ChartAxis();
      chart.YAxis = new ChartAxis();
      SimpleDataSeries series = new SimpleDataSeries();
      // arbitrary data:
      IList data = new List<object>();
      data.Add(new Point(0, 2));
      data.Add(new Point(1, -4));
      data.Add(new Point(2, 13));
      chart.Series.Add(series);
      series.ItemsSource = data;

      series.SelectionMode = DataPointSelectionMode.All;

      Assert.AreEqual(3, series.DataPoints.Count);
      series.DataPoints[1].IsSelected = true;
      Assert.AreEqual(series.DataPoints[1], series.SelectedDataPoint);

      // TODO: All data points are now selected.
      // So by setting the IsSelected property of a different data point to be true, no change in the property value occurs.
      // This means that the DataSeries.SelectedDataPoint property is not going to change... but should it??

      /*// select a different data point.
      series.DataPoints[0].IsSelected = true;
      // The SelectedDataPoint should now be the newly selected data point.
      Assert.AreEqual(series.DataPoints[0], series.SelectedDataPoint);*/
    }

    [Test]
    [Ignore("This test needs to be revised")]
    public void SelectedDataPointPropertyIsNullWhenDataPointIsDeselected_AllSelectionMode()
    {
      Chart chart = new Chart();
      chart.XAxis = new ChartAxis();
      chart.YAxis = new ChartAxis();
      SimpleDataSeries series = new SimpleDataSeries();
      // arbitrary data:
      IList data = new List<object>();
      data.Add(new Point(0, 2));
      data.Add(new Point(1, -4));
      data.Add(new Point(2, 13));
      chart.Series.Add(series);
      series.ItemsSource = data;

      series.SelectionMode = DataPointSelectionMode.All;

      Assert.AreEqual(3, series.DataPoints.Count);
      series.DataPoints[1].IsSelected = true;
      Assert.AreEqual(series.DataPoints[1], series.SelectedDataPoint);
      // deselect the currently selected data point.
      series.DataPoints[1].IsSelected = false;
      // This should cause the SelectedDataPoint property to be set to null.
      Assert.IsNull(series.SelectedDataPoint);
    }

    [Test]
    [Ignore("This test needs to be revised")]
    public void SelectedDataPointPropertyChangesToNullWhenItemsSourceChanges()
    {
      Chart chart = new Chart();
      chart.XAxis = new ChartAxis();
      chart.YAxis = new ChartAxis();
      SimpleDataSeries series = new SimpleDataSeries();
      // arbitrary data:
      IList data = new List<object>();
      data.Add(new Point(0, 4));
      data.Add(new Point(1, 7));
      data.Add(new Point(2, 6));
      chart.Series.Add(series);
      series.ItemsSource = data;

      series.SelectionMode = DataPointSelectionMode.Single;

      Assert.AreEqual(3, series.DataPoints.Count);
      series.DataPoints[1].IsSelected = true;
      Assert.AreEqual(series.DataPoints[1], series.SelectedDataPoint);

      // change the items source:
      IList data2 = new List<object>();
      data2.Add(new Point(0, 9));
      data2.Add(new Point(1, 1));
      data2.Add(new Point(2, 15));
      series.ItemsSource = data2;

      // This should cause the SelectedDataPoint property to be set to null.
      Assert.IsNull(series.SelectedDataPoint);
    }

    [Test]
    [Ignore("Performance improvements prevents this from working in the test environment")]
    public void SelectedDataPointPropertyChangesToNullWhenDataIsRemoved()
    {
      Chart chart = new Chart();
      chart.XAxis = new ChartAxis();
      chart.YAxis = new ChartAxis();
      SimpleDataSeries series = new SimpleDataSeries();
      // arbitrary data:
      ObservableCollection<object> data = new ObservableCollection<object>();
      data.Add(new Point(0, 456));
      data.Add(new Point(1, 93));
      data.Add(new Point(2, 87));
      chart.Series.Add(series);
      series.ItemsSource = data;

      series.SelectionMode = DataPointSelectionMode.Single;

      Assert.AreEqual(3, series.DataPoints.Count);
      series.DataPoints[1].IsSelected = true;
      Assert.AreEqual(series.DataPoints[1], series.SelectedDataPoint);

      // Remove the selected data object corresponding to the selected cartesian data point.
      data.RemoveAt(1);
      Assert.AreEqual(2, series.DataPoints.Count);

      // This should cause the SelectedDataPoint property to be set to null.
      Assert.IsNull(series.SelectedDataPoint);
    }

    [Test]
    [Ignore("Performance improvements prevents this from working in the test environment")]
    public void SelectedDataPointPropertyChangesToNullWhenItemsSourceIsCleared()
    {
      Chart chart = new Chart();
      chart.XAxis = new ChartAxis();
      chart.YAxis = new ChartAxis();
      SimpleDataSeries series = new SimpleDataSeries();
      // arbitrary data:
      ObservableCollection<object> data = new ObservableCollection<object>();
      data.Add(new Point(0, 90));
      data.Add(new Point(1, 57));
      data.Add(new Point(2, 62));
      chart.Series.Add(series);
      series.ItemsSource = data;

      series.SelectionMode = DataPointSelectionMode.Single;

      Assert.AreEqual(3, series.DataPoints.Count);
      series.DataPoints[1].IsSelected = true;
      Assert.AreEqual(series.DataPoints[1], series.SelectedDataPoint);

      data.Clear();
      Assert.AreEqual(0, series.DataPoints.Count);

      // This should cause the SelectedDataPoint property to be set to null.
      Assert.IsNull(series.SelectedDataPoint);
    }

    [Test]
    [STAThread]
    public void YAxisTitleProperty()
    {
      Chart chart = new Chart();
      chart.XAxis = new ChartAxis();
      chart.YAxis = new ChartAxis();
      ChartAxis alternateAxis1 = new ChartAxis() { Title = "Secondary" };
      ChartAxis alternateAxis2 = new ChartAxis() { Title = "Tertiary" };
      chart.AlternativeYAxes.Add(alternateAxis1);
      chart.AlternativeYAxes.Add(alternateAxis2);

      SimpleDataSeries series = new SimpleDataSeries();
      series.YAxisTitle = "Secondary";
      chart.Series.Add(series);

      Assert.AreEqual(alternateAxis1, series.YAxis);
    }

    [Test]
    [STAThread]
    public void UseDefaultAxisIfAlternateCanNotBeFound()
    {
      Chart chart = new Chart();
      chart.XAxis = new ChartAxis();
      chart.YAxis = new ChartAxis();
      ChartAxis alternateAxis2 = new ChartAxis() { Title = "Tertiary" };
      chart.AlternativeYAxes.Add(alternateAxis2);

      SimpleDataSeries series = new SimpleDataSeries();
      series.YAxisTitle = "Secondary"; // This title does not match any of the alternate Y axes...
      chart.Series.Add(series);

      Assert.AreEqual(chart.YAxis, series.YAxis);// ...so the series should automatically use the normal Y axis
    }

    [Test]
    [STAThread]
    public void ChangingYAxisTitleChangesYAxis()
    {
      Chart chart = new Chart();
      chart.XAxis = new ChartAxis();
      chart.YAxis = new ChartAxis();
      ChartAxis alternateAxis1 = new ChartAxis() { Title = "Secondary" };
      ChartAxis alternateAxis2 = new ChartAxis() { Title = "Tertiary" };
      chart.AlternativeYAxes.Add(alternateAxis1);
      chart.AlternativeYAxes.Add(alternateAxis2);

      SimpleDataSeries series = new SimpleDataSeries();
      series.YAxisTitle = "Secondary";
      chart.Series.Add(series);

      Assert.AreEqual(alternateAxis1, series.YAxis);

      series.YAxisTitle = "Tertiary"; // Changing the YAxisTitle property should cause the YAxis of the data series to be changed.
      Assert.AreEqual(alternateAxis2, series.YAxis);
    }

    [Test]
    [Ignore("This test needs to be revised")]
    public void XBinding()
    {
      Chart chart = new Chart();
      chart.XAxis = new ChartAxis();
      chart.YAxis = new ChartAxis();
      SimpleDataSeries series = new SimpleDataSeries();
      series.XBinding = new Binding("String");
      chart.Series.Add(series);
      IList data = new List<object>();
      data.Add(new StringDouble("Jan", 1.2));
      data.Add(new StringDouble("Feb", 3.4));
      data.Add(new StringDouble("Mar", 5.6));
      data.Add(new StringDouble("Apr", 7.8));
      series.ItemsSource = data;

      Assert.AreEqual(series.DataPoints.Count, series.ItemsSource.Count);
      int index = 0;
      foreach (CartesianDataPoint dp in series.DataPoints)
      {
        StringDouble sd = data[index] as StringDouble;
        Assert.AreEqual(sd.String, dp.XObject);
        index++;
      }
    }

    [Test]
    [Ignore("This test needs to be revised")]
    public void YBinding()
    {
      Chart chart = new Chart();
      chart.XAxis = new ChartAxis();
      chart.YAxis = new ChartAxis();
      SimpleDataSeries series = new SimpleDataSeries();
      series.YBinding = new Binding("Double");
      chart.Series.Add(series);
      IList data = new List<object>();
      data.Add(new StringDouble("Jan", 1.2));
      data.Add(new StringDouble("Feb", 3.4));
      data.Add(new StringDouble("Mar", 5.6));
      data.Add(new StringDouble("Apr", 7.8));
      series.ItemsSource = data;

      Assert.AreEqual(series.DataPoints.Count, series.ItemsSource.Count);
      int index = 0;
      foreach (CartesianDataPoint dp in series.DataPoints)
      {
        StringDouble sd = data[index] as StringDouble;
        Assert.AreEqual(sd.Double, dp.YObject);
        index++;
      }
    }

    [Test]
    [STAThread]
    public void TitleIsDisplayedInLegend()
    {
      Chart chart = new Chart();
      chart.XAxis = new ChartAxis();
      chart.YAxis = new ChartAxis();
      SimpleDataSeries series = new SimpleDataSeries();
      series.Title = "Elite Series";
      chart.Series.Add(series);

      Assert.AreEqual(1, chart.LegendItems.Count);
      Assert.AreEqual("Elite Series", chart.LegendItems[0].LegendLabel);
    }

    [Test]
    [Ignore("This test needs to be revised")]
    public void ListOfPoint()
    {
      Chart chart = new Chart();
      chart.XAxis = new ChartAxis();
      chart.YAxis = new ChartAxis();
      SimpleDataSeries series = new SimpleDataSeries();
      chart.Series.Add(series);

      IList data = new List<object>();
      data.Add(new Point(0.0, 1.2));
      data.Add(new Point(0.0, 3.4));
      data.Add(new Point(0.0, 5.6));
      data.Add(new Point(0.0, 7.8));
      series.ItemsSource = data;

      Assert.AreEqual(4, series.DataPoints.Count);
      Assert.AreEqual(series.DataPoints.Count, series.ItemsSource.Count);
      int index = 0;
      foreach (CartesianDataPoint dp in series.DataPoints)
      {
        Point point = (Point)data[index];
        Assert.AreEqual(point.X, dp.XObject);
        Assert.AreEqual(point.Y, dp.YObject);
        Assert.AreEqual(point, dp.LogicalPoint);
        index++;
      }
    }

    [Test]
    [Ignore("This test needs to be revised")]
    public void ListOfInt()
    {
      Chart chart = new Chart();
      chart.XAxis = new ChartAxis();
      chart.YAxis = new ChartAxis();
      SimpleDataSeries series = new SimpleDataSeries();
      chart.Series.Add(series);

      IList data = new List<object>();
      data.Add(1);
      data.Add(7);
      data.Add(5);
      data.Add(6);
      series.ItemsSource = data;

      Assert.AreEqual(4, series.DataPoints.Count);
      Assert.AreEqual(series.DataPoints.Count, series.ItemsSource.Count);
      int index = 0;
      foreach (CartesianDataPoint dp in series.DataPoints)
      {
        int value = (int)data[index];
        Assert.AreEqual((double)index, dp.XObject);
        Assert.AreEqual((double)value, dp.YObject);
        Assert.AreEqual(new Point(index, value), dp.LogicalPoint);
        index++;
      }
    }

    [Test]
    [Ignore("This test needs to be revised")]
    public void ListOfDouble()
    {
      Chart chart = new Chart();
      chart.XAxis = new ChartAxis();
      chart.YAxis = new ChartAxis();
      SimpleDataSeries series = new SimpleDataSeries();
      chart.Series.Add(series);

      IList data = new List<object>();
      data.Add(1.4);
      data.Add(7.2);
      data.Add(5.8);
      data.Add(6.5);
      series.ItemsSource = data;

      Assert.AreEqual(4, series.DataPoints.Count);
      Assert.AreEqual(series.DataPoints.Count, series.ItemsSource.Count);
      int index = 0;
      foreach (CartesianDataPoint dp in series.DataPoints)
      {
        double value = (double)data[index];
        Assert.AreEqual((double)index, dp.XObject);
        Assert.AreEqual(value, dp.YObject);
        Assert.AreEqual(new Point(index, value), dp.LogicalPoint);
        index++;
      }
    }

    [Test]
    [Ignore("This test needs to be revised")]
    public void ListOfDecimal()
    {
      Chart chart = new Chart();
      chart.XAxis = new ChartAxis();
      chart.YAxis = new ChartAxis();
      SimpleDataSeries series = new SimpleDataSeries();
      chart.Series.Add(series);

      IList data = new List<object>();
      data.Add((decimal)1.4);
      data.Add((decimal)7.2);
      data.Add((decimal)5.8);
      data.Add((decimal)6.5);
      series.ItemsSource = data;

      Assert.AreEqual(4, series.DataPoints.Count);
      Assert.AreEqual(series.DataPoints.Count, series.ItemsSource.Count);
      int index = 0;
      foreach (CartesianDataPoint dp in series.DataPoints)
      {
        decimal value = (decimal)data[index];
        Assert.AreEqual((double)index, dp.XObject);
        Assert.AreEqual((double)value, dp.YObject);
        Assert.AreEqual(new Point(index, (double)value), dp.LogicalPoint);
        index++;
      }
    }

    [Test]
    [Ignore("This test needs to be revised")]
    public void ListOfLong()
    {
      Chart chart = new Chart();
      chart.XAxis = new ChartAxis();
      chart.YAxis = new ChartAxis();
      SimpleDataSeries series = new SimpleDataSeries();
      chart.Series.Add(series);

      IList data = new List<object>();
      data.Add((long)1028364);
      data.Add((long)4730194);
      data.Add((long)1934732);
      data.Add((long)8234760);
      series.ItemsSource = data;

      Assert.AreEqual(4, series.DataPoints.Count);
      Assert.AreEqual(series.DataPoints.Count, series.ItemsSource.Count);
      int index = 0;
      foreach (CartesianDataPoint dp in series.DataPoints)
      {
        long value = (long)data[index];
        Assert.AreEqual((double)index, dp.XObject);
        Assert.AreEqual((double)value, dp.YObject);
        Assert.AreEqual(new Point(index, value), dp.LogicalPoint);
        index++;
      }
    }

    [Test]
    [Ignore("This test needs to be revised")]
    public void ListOfFloat()
    {
      Chart chart = new Chart();
      chart.XAxis = new ChartAxis();
      chart.YAxis = new ChartAxis();
      SimpleDataSeries series = new SimpleDataSeries();
      chart.Series.Add(series);

      IList data = new List<object>();
      data.Add((float)3.4);
      data.Add((float)9.1);
      data.Add((float)8.6);
      data.Add((float)6.3);
      series.ItemsSource = data;

      Assert.AreEqual(4, series.DataPoints.Count);
      Assert.AreEqual(series.DataPoints.Count, series.ItemsSource.Count);
      int index = 0;
      foreach (CartesianDataPoint dp in series.DataPoints)
      {
        float value = (float)data[index];
        Assert.AreEqual((double)index, dp.XObject);
        Assert.AreEqual(Double.Parse(value.ToString()), dp.YObject);
        Assert.AreEqual(new Point(index, Double.Parse(value.ToString())), dp.LogicalPoint);
        index++;
      }
    }

    [Test]
    [Ignore("This test needs to be revised")]
    public void ListOfObject()
    {
      Chart chart = new Chart();
      chart.XAxis = new ChartAxis();
      chart.YAxis = new ChartAxis();
      SimpleDataSeries series = new SimpleDataSeries();
      chart.Series.Add(series);

      IList data = new List<object>();
      data.Add("Jan");
      data.Add("Feb");
      data.Add("Mar");
      data.Add("Apr");
      series.ItemsSource = data;

      Assert.AreEqual(4, series.DataPoints.Count);
      Assert.AreEqual(series.DataPoints.Count, series.ItemsSource.Count);
      int index = 0;
      foreach (CartesianDataPoint dp in series.DataPoints)
      {
        string value = (string)data[index];
        Assert.AreEqual((double)index, dp.XObject);
        Assert.AreEqual(value, dp.YObject);
        index++;
      }
    }

    [Test]
    [Ignore("This test needs to be revised")]
    public void HorizontalListOfInt()
    {
      Chart chart = new Chart();
      chart.XAxis = new ChartAxis();
      chart.YAxis = new ChartAxis();
      SimpleDataSeries series = new SimpleDataSeries();
      series.Orientation = Orientation.Horizontal;
      chart.Series.Add(series);

      IList data = new List<object>();
      data.Add(1);
      data.Add(7);
      data.Add(5);
      data.Add(6);
      series.ItemsSource = data;

      Assert.AreEqual(4, series.DataPoints.Count);
      Assert.AreEqual(series.DataPoints.Count, series.ItemsSource.Count);
      int index = 0;
      foreach (CartesianDataPoint dp in series.DataPoints)
      {
        int value = (int)data[index];
        Assert.AreEqual((double)index, dp.YObject); // The index is now mapped along the Y axis.
        Assert.AreEqual((double)value, dp.XObject); // The data object itself is now mapped along the X axis
        Assert.AreEqual(new Point(value, index), dp.LogicalPoint);
        index++;
      }
    }

    [Test]
    [Ignore("This test needs to be revised")]
    public void HorizontalListOfObject()
    {
      Chart chart = new Chart();
      chart.XAxis = new ChartAxis();
      chart.YAxis = new ChartAxis();
      SimpleDataSeries series = new SimpleDataSeries();
      series.Orientation = Orientation.Horizontal;
      chart.Series.Add(series);

      IList data = new List<object>();
      data.Add("Jan");
      data.Add("Feb");
      data.Add("Mar");
      data.Add("Apr");
      series.ItemsSource = data;

      Assert.AreEqual(4, series.DataPoints.Count);
      Assert.AreEqual(series.DataPoints.Count, series.ItemsSource.Count);
      int index = 0;
      foreach (CartesianDataPoint dp in series.DataPoints)
      {
        string value = (string)data[index];
        Assert.AreEqual((double)index, dp.YObject); // The index is now mapped along the Y axis.
        Assert.AreEqual(value, dp.XObject); // The data object itself is now mapped along the X axis
        index++;
      }
    }
  }
}
