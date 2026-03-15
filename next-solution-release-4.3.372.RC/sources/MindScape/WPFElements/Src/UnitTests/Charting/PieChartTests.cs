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
using System.Collections.ObjectModel;
using Mindscape.WpfElements.Charting;
using NUnit.Framework;

namespace Mindscape.WpfElements.UnitTests
{
  [TestFixture]
  public class PieChartTests
  {
    [Test]
    [STAThread]
    public void NumberOfLegendItemsIsSameAsItemsSourceCount()
    {
      PieChart chart = new PieChart();
      PieSeries series = new PieSeries();
      chart.Series.Add(series);
      IList data = new List<object>();
      for (int i = 0; i < 5; i++)
      {
        data.Add(3);
      }
      series.ItemsSource = data;

      Assert.AreEqual(5, series.ItemsSource.Count);
      Assert.AreEqual(chart.LegendItems.Count, series.ItemsSource.Count);
    }

    [Test]
    [STAThread]
    public void DataPointsWithSameNameShareLegendItem()
    {
      PieChart chart = new PieChart();
      PieSeries series1 = new PieSeries();
      series1.DataBinding = new Binding("Double");
      series1.TitleBinding = new Binding("String");
      PieSeries series2 = new PieSeries();
      series2.DataBinding = new Binding("Double");
      series2.TitleBinding = new Binding("String");
      chart.Series.Add(series1);
      chart.Series.Add(series2);

      IList data1 = new List<object>();
      data1.Add(new StringDouble("Jan", 3));
      data1.Add(new StringDouble("Feb", 3));
      data1.Add(new StringDouble("Mar", 3));
      data1.Add(new StringDouble("SharedName", 3));

      IList data2 = new List<object>();
      data2.Add(new StringDouble("Apr", 3));
      data2.Add(new StringDouble("May", 3));
      data2.Add(new StringDouble("Jun", 3));
      data2.Add(new StringDouble("SharedName", 3));

      series1.ItemsSource = data1;
      series2.ItemsSource = data2;

      // There are 8 data points, but only 7 legend items should be created because 2 of the data points share the same name.
      Assert.AreEqual(7, chart.LegendItems.Count);
    }

    [Test]
    [STAThread]
    public void NumberOfDataPointsIsSameAsItemsSourceCount()
    {
      PieChart chart = new PieChart();
      PieSeries series = new PieSeries();
      chart.Series.Add(series);
      IList data = new List<object>();
      for (int i = 0; i < 7; i++)
      {
        data.Add(5);
      }
      series.ItemsSource = data;

      Assert.AreEqual(7, series.ItemsSource.Count);
      Assert.AreEqual(series.DataPoints.Count, series.ItemsSource.Count);
    }

    [Test]
    [STAThread]
    public void BrushesAreAppliedSequentially()
    {
      PieChart chart = new PieChart();
      PieSeries series = new PieSeries();
      chart.Series.Add(series);
      series.Brushes.Clear();
      series.Brushes.Add(new SolidColorBrush() { Color = Colors.Red });
      series.Brushes.Add(new SolidColorBrush() { Color = Colors.Green });
      series.Brushes.Add(new SolidColorBrush() { Color = Colors.Blue });
      IList data = new List<object>();
      data.Add(5);
      data.Add(5);
      data.Add(5);
      series.ItemsSource = data;

      Assert.AreEqual(series.DataPoints.Count, series.ItemsSource.Count);
      int index = 0;
      foreach (PieSlice slice in series.DataPoints)
      {
        Assert.AreEqual(series.Brushes[index], slice.Background);
        index++;
      }
    }

    [Test]
    [STAThread]
    public void BrushesAreWrappedIfTooManyDataPoints()
    {
      PieChart chart = new PieChart();
      PieSeries series = new PieSeries();
      chart.Series.Add(series);
      series.Brushes.Clear();
      series.Brushes.Add(new SolidColorBrush() { Color = Colors.Red });
      series.Brushes.Add(new SolidColorBrush() { Color = Colors.Green });
      series.Brushes.Add(new SolidColorBrush() { Color = Colors.Blue });
      IList data = new List<object>();
      data.Add(5);
      data.Add(5);
      data.Add(5);
      data.Add(5); // 4 data points, but only 3 brushes.
      series.ItemsSource = data;

      Assert.AreEqual(series.DataPoints.Count, series.ItemsSource.Count);
      int index = 0;
      foreach (PieSlice slice in series.DataPoints)
      {
        Assert.AreEqual(series.Brushes[index], slice.Background);
        index = ++index % series.Brushes.Count; // wrap the index based on number
      }
    }

    [Test]
    [STAThread]
    public void RadiusBinding()
    {
      PieChart chart = new PieChart();
      PieSeries series = new PieSeries();
      chart.Series.Add(series);
      series.DataBinding = new Binding("X");
      series.RadiusBinding = new Binding("Y");
      IList data = new List<object>();
      data.Add(new Point(5, 0.3));
      data.Add(new Point(5, 0.5));
      data.Add(new Point(5, 0.4));
      data.Add(new Point(5, 0.8));
      series.ItemsSource = data;

      Assert.AreEqual(series.DataPoints.Count, series.ItemsSource.Count);
      int index = 0;
      foreach (PieSlice slice in series.DataPoints)
      {
        Point point = (Point)data[index];
        Assert.AreEqual(point.Y, slice.RadiusFactor);
        index++;
      }
    }

    [Test]
    [STAThread]
    public void DataBinding()
    {
      PieChart chart = new PieChart();
      PieSeries series = new PieSeries();
      chart.Series.Add(series);
      series.DataBinding = new Binding("Double");
      IList data = new List<object>();
      data.Add(new StringDouble("Jan", 1.2));
      data.Add(new StringDouble("Feb", 3.4));
      data.Add(new StringDouble("Mar", 5.6));
      data.Add(new StringDouble("Apr", 7.8));
      series.ItemsSource = data;

      Assert.AreEqual(series.DataPoints.Count, series.ItemsSource.Count);
      int index = 0;
      foreach (PieSlice slice in series.DataPoints)
      {
        StringDouble sd = (StringDouble)data[index];
        Assert.AreEqual(sd.Double, slice.DataValue);
        index++;
      }
    }

    [Test]
    [STAThread]
    public void TitleBinding()
    {
      PieChart chart = new PieChart();
      PieSeries series = new PieSeries();
      chart.Series.Add(series);
      series.TitleBinding = new Binding("String");
      IList data = new List<object>();
      data.Add(new StringDouble("Jan", 1.2));
      data.Add(new StringDouble("Feb", 3.4));
      data.Add(new StringDouble("Mar", 5.6));
      data.Add(new StringDouble("Apr", 7.8));
      series.ItemsSource = data;

      Assert.AreEqual(series.DataPoints.Count, series.ItemsSource.Count);
      int index = 0;
      foreach (PieSlice slice in series.DataPoints)
      {
        StringDouble sd = (StringDouble)data[index];
        Assert.AreEqual(sd.String, slice.Title);
        index++;
      }
    }

    [Test]
    [STAThread]
    public void TitleBindingAffectsLegendTitles()
    {
      PieChart chart = new PieChart();
      PieSeries series = new PieSeries();
      chart.Series.Add(series);
      series.TitleBinding = new Binding("String");
      IList data = new List<object>();
      data.Add(new StringDouble("Jan", 1.2));
      data.Add(new StringDouble("Feb", 3.4));
      data.Add(new StringDouble("Mar", 5.6));
      data.Add(new StringDouble("Apr", 7.8));
      series.ItemsSource = data;

      Assert.AreEqual(chart.LegendItems.Count, series.ItemsSource.Count);
      int index = 0;
      foreach (LegendItem item in chart.LegendItems)
      {
        StringDouble sd = data[index] as StringDouble;
        Assert.AreEqual(sd.String, item.LegendLabel);
        index++;
      }
    }

    [Test]
    [STAThread]
    public void AutomaticLegendTitles()
    {
      PieChart chart = new PieChart();
      PieSeries series = new PieSeries();
      chart.Series.Add(series);
      IList data = new List<object>();
      data.Add(1.2);
      data.Add(3.4);
      data.Add(5.6);
      data.Add(7.8);
      series.ItemsSource = data;

      Assert.AreEqual(chart.LegendItems.Count, series.ItemsSource.Count);
      int index = 0;
      foreach (LegendItem item in chart.LegendItems)
      {
        Assert.AreEqual("Item " + (index + 1), item.LegendLabel);
        index++;
      }
    }

    [Test]
    [STAThread]
    public void PercentageProperty()
    {
      PieChart chart = new PieChart();
      PieSeries series = new PieSeries();
      chart.Series.Add(series);
      IList data = new List<object>();
      data.Add(20.0);
      data.Add(10.4);
      data.Add(3.1);
      data.Add(16.5);
      series.ItemsSource = data;

      Assert.AreEqual(series.DataPoints.Count, series.ItemsSource.Count);
      int index = 0;
      foreach (PieSlice slice in series.DataPoints)
      {
        double value = (double)data[index];
        Assert.AreEqual(value * 2, Math.Round(slice.Percentage, 2));
        index++;
      }
    }

    [Test]
    [STAThread]
    public void DoughnutScaleIsPassedToPieSlices()
    {
      PieChart chart = new PieChart();
      PieSeries series = new PieSeries();
      series.DoughnutScale = 0.4;
      chart.Series.Add(series);
      IList data = new List<object>();
      for (int i = 0; i < 5; i++)
      {
        data.Add(10); // abitrary data
      }
      series.ItemsSource = data;

      Assert.AreEqual(series.DataPoints.Count, series.ItemsSource.Count);
      foreach (PieSlice slice in series.DataPoints)
      {
        Assert.AreEqual(series.DoughnutScale, slice.DoughnutScale);
      }
    }

    [Test]
    [STAThread]
    public void DiameterIsTwiceRadius()
    {
      PieChart chart = new PieChart();
      chart.Width = 400;
      chart.Height = 400;
      PieSeries series = new PieSeries();
      chart.Series.Add(series);
      series.DataBinding = new Binding("X");
      series.RadiusBinding = new Binding("Y");
      IList data = new List<object>();
      data.Add(new Point(5, 0.3));
      data.Add(new Point(5, 0.5));
      data.Add(new Point(5, 0.4));
      data.Add(new Point(5, 0.8));
      series.ItemsSource = data;

      Assert.AreEqual(series.DataPoints.Count, series.ItemsSource.Count);
      foreach (PieSlice slice in series.DataPoints)
      {
        Assert.AreEqual(slice.Radius * 2, slice.Diameter);
      }
    }

    #region DataPointSelectionMode tests

    [Test]
    [STAThread]
    public void CanNotSelectIfSelectionModeIsNone()
    {
      PieChart chart = new PieChart();
      PieSeries series = new PieSeries();
      // arbitrary data:
      IList data = new List<object>();
      data.Add(2);
      data.Add(4);
      data.Add(13);
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
    [STAThread]
    public void CanOnlySelectOneDataPointIfSelectionModeIsSingle()
    {
      PieChart chart = new PieChart();
      PieSeries series = new PieSeries();
      // arbitrary data:
      IList data = new List<object>();
      data.Add(2);
      data.Add(4);
      data.Add(13);
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
    [STAThread]
    public void AllDataPointsAreSelectedIfSelectionModeIsAll()
    {
      PieChart chart = new PieChart();
      PieSeries series = new PieSeries();
      // arbitrary data:
      IList data = new List<object>();
      data.Add(2);
      data.Add(4);
      data.Add(13);
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
    [STAThread]
    public void ChangingSelectionModeToNoneWillDeselectAllDataPoints()
    {
      PieChart chart = new PieChart();
      PieSeries series = new PieSeries();
      // arbitrary data:
      IList data = new List<object>();
      data.Add(2);
      data.Add(4);
      data.Add(13);
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
    [STAThread]
    public void ChangingSelectionModeToSingleWillRespectSelectionMode()
    {
      PieChart chart = new PieChart();
      PieSeries series = new PieSeries();
      // arbitrary data:
      IList data = new List<object>();
      data.Add(2);
      data.Add(4);
      data.Add(13);
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
    [STAThread]
    public void SelectedDataPointPropertyRemainsNullWhenSelectionModeIsNone()
    {
      PieChart chart = new PieChart();
      PieSeries series = new PieSeries();
      // arbitrary data:
      IList data = new List<object>();
      data.Add(2);
      data.Add(4);
      data.Add(13);
      chart.Series.Add(series);
      series.ItemsSource = data;

      series.SelectionMode = DataPointSelectionMode.None;

      Assert.AreEqual(3, series.DataPoints.Count);
      // Attempt to select a data point. The SelectedDataPoint should remain null because the selection mode is none.
      series.DataPoints[1].IsSelected = true;
      Assert.IsNull(series.SelectedDataPoint);
    }

    [Test]
    [STAThread]
    public void SelectedDataPointPropertyIsSetWhenSelectionModeIsSingle()
    {
      PieChart chart = new PieChart();
      PieSeries series = new PieSeries();
      // arbitrary data:
      IList data = new List<object>();
      data.Add(2);
      data.Add(4);
      data.Add(13);
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
    [STAThread]
    public void SelectedDataPointPropertyIsNullWhenDataPointIsDeselected_SingleSelectionMode()
    {
      PieChart chart = new PieChart();
      PieSeries series = new PieSeries();
      // arbitrary data:
      IList data = new List<object>();
      data.Add(2);
      data.Add(4);
      data.Add(13);
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
    [STAThread]
    public void SelectedDataPointPropertyIsSetWhenSelectionModeIsAll()
    {
      PieChart chart = new PieChart();
      PieSeries series = new PieSeries();
      // arbitrary data:
      IList data = new List<object>();
      data.Add(2);
      data.Add(4);
      data.Add(13);
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
    [STAThread]
    public void SelectedDataPointPropertyIsNullWhenDataPointIsDeselected_AllSelectionMode()
    {
      PieChart chart = new PieChart();
      PieSeries series = new PieSeries();
      // arbitrary data:
      IList data = new List<object>();
      data.Add(2);
      data.Add(4);
      data.Add(13);
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

    #endregion // DataPointSelectionMode tests

    #region DataSeriesSelectionMode tests

    [Test]
    [STAThread]
    public void OnlyOneSeriesCanHaveSelectionIfSelectionModeIsSingle()
    {
      PieChart chart = new PieChart();
      chart.SelectionMode = DataSeriesSelectionMode.Single;
      PieSeries series1 = new PieSeries();
      // arbitrary data:
      IList data1 = new List<object>();
      data1.Add(new Point(0, 2));
      data1.Add(new Point(1, -4));
      data1.Add(new Point(2, 13));
      chart.Series.Add(series1);
      series1.ItemsSource = data1;

      PieSeries series2 = new PieSeries();
      // arbitrary data:
      IList data2 = new List<object>();
      data2.Add(new Point(0, 4));
      data2.Add(new Point(2, 1));
      data2.Add(new Point(4, -12));
      chart.Series.Add(series2);
      series2.ItemsSource = data2;

      // Find all the data points from the first series.
      ReadOnlyCollection<PieSlice> points1 = series1.DataPoints;
      Assert.AreEqual(points1.Count, data1.Count);

      // Find all the data points from the second series
      ReadOnlyCollection<PieSlice> points2 = series2.DataPoints;
      Assert.AreEqual(points2.Count, data2.Count);

      // Select a data point from the first series and check that it is the only selected data point.
      points1[1].IsSelected = true;
      Assert.IsFalse(points1[0].IsSelected);
      Assert.IsTrue(points1[1].IsSelected);
      Assert.IsFalse(points1[2].IsSelected);

      Assert.IsFalse(points2[0].IsSelected);
      Assert.IsFalse(points2[1].IsSelected);
      Assert.IsFalse(points2[2].IsSelected);

      // Select a data point from the second series. Check that it is selected, and the previous data point has been deselected
      points2[0].IsSelected = true;
      Assert.IsFalse(points1[0].IsSelected);
      Assert.IsFalse(points1[1].IsSelected);
      Assert.IsFalse(points1[2].IsSelected);

      Assert.IsTrue(points2[0].IsSelected);
      Assert.IsFalse(points2[1].IsSelected);
      Assert.IsFalse(points2[2].IsSelected);
    }

    [Test]
    [STAThread]
    public void MultipleDataSeriesCanHaveSelectionIfSelectionModeIsMultiple()
    {
      PieChart chart = new PieChart();
      chart.SelectionMode = DataSeriesSelectionMode.Multiple;
      PieSeries series1 = new PieSeries();
      // arbitrary data:
      IList data1 = new List<object>();
      data1.Add(new Point(0, 2));
      data1.Add(new Point(1, -4));
      data1.Add(new Point(2, 13));
      chart.Series.Add(series1);
      series1.ItemsSource = data1;

      PieSeries series2 = new PieSeries();
      // arbitrary data:
      IList data2 = new List<object>();
      data2.Add(new Point(0, 4));
      data2.Add(new Point(2, 1));
      data2.Add(new Point(4, -12));
      chart.Series.Add(series2);
      series2.ItemsSource = data2;

      // Find all the data points from the first series.
      ReadOnlyCollection<PieSlice> points1 = series1.DataPoints;
      Assert.AreEqual(points1.Count, data1.Count);

      // Find all the data points from the second series
      ReadOnlyCollection<PieSlice> points2 = series2.DataPoints;
      Assert.AreEqual(points2.Count, data2.Count);

      // Select a data point from the first series and check that it is the only selected data point.
      points1[1].IsSelected = true;
      Assert.IsFalse(points1[0].IsSelected);
      Assert.IsTrue(points1[1].IsSelected);
      Assert.IsFalse(points1[2].IsSelected);

      Assert.IsFalse(points2[0].IsSelected);
      Assert.IsFalse(points2[1].IsSelected);
      Assert.IsFalse(points2[2].IsSelected);

      // Select a data point from the second series. Check that it is selected, and the previous data point has not been deselected
      points2[0].IsSelected = true;
      Assert.IsFalse(points1[0].IsSelected);
      Assert.IsTrue(points1[1].IsSelected);
      Assert.IsFalse(points1[2].IsSelected);

      Assert.IsTrue(points2[0].IsSelected);
      Assert.IsFalse(points2[1].IsSelected);
      Assert.IsFalse(points2[2].IsSelected);
    }

    [Test]
    [STAThread]
    public void ChangingToSingleSelectionModeIsRespected()
    {
      PieChart chart = new PieChart();
      chart.SelectionMode = DataSeriesSelectionMode.Multiple;
      PieSeries series1 = new PieSeries();
      // arbitrary data:
      IList data1 = new List<object>();
      data1.Add(new Point(0, 2));
      data1.Add(new Point(1, -4));
      data1.Add(new Point(2, 13));
      chart.Series.Add(series1);
      series1.ItemsSource = data1;

      PieSeries series2 = new PieSeries();
      // arbitrary data:
      IList data2 = new List<object>();
      data2.Add(new Point(0, 4));
      data2.Add(new Point(2, 1));
      data2.Add(new Point(4, -12));
      chart.Series.Add(series2);
      series2.ItemsSource = data2;

      // Find all the data points from the first series.
      ReadOnlyCollection<PieSlice> points1 = series1.DataPoints;
      Assert.AreEqual(points1.Count, data1.Count);

      // Find all the data points from the second series
      ReadOnlyCollection<PieSlice> points2 = series2.DataPoints;
      Assert.AreEqual(points2.Count, data2.Count);

      // Select a data point from both series
      points1[1].IsSelected = true;
      points2[0].IsSelected = true;

      // Change the selection mode to Single. All data points should be deselected.
      chart.SelectionMode = DataSeriesSelectionMode.Single;
      Assert.IsFalse(points1[0].IsSelected);
      Assert.IsFalse(points1[1].IsSelected);
      Assert.IsFalse(points1[2].IsSelected);

      Assert.IsFalse(points2[0].IsSelected);
      Assert.IsFalse(points2[1].IsSelected);
      Assert.IsFalse(points2[2].IsSelected);
    }

    [Test]
    [STAThread]
    public void SingleSelectionModeWorksWhenEachSeriesHasDifferentSelectionModes()
    {
      PieChart chart = new PieChart();
      chart.SelectionMode = DataSeriesSelectionMode.Single;
      PieSeries series1 = new PieSeries();
      series1.SelectionMode = DataPointSelectionMode.Single;
      PieSeries series2 = new PieSeries();
      series2.SelectionMode = DataPointSelectionMode.All;

      // arbitrary data:
      IList data1 = new List<object>();
      data1.Add(new Point(0, 2));
      data1.Add(new Point(1, -4));
      data1.Add(new Point(2, 13));
      chart.Series.Add(series1);
      series1.ItemsSource = data1;

      // arbitrary data:
      IList data2 = new List<object>();
      data2.Add(new Point(0, 4));
      data2.Add(new Point(2, 1));
      data2.Add(new Point(4, -12));
      chart.Series.Add(series2);
      series2.ItemsSource = data2;

      // Find all the data points from the first series.
      ReadOnlyCollection<PieSlice> points1 = series1.DataPoints;
      Assert.AreEqual(points1.Count, data1.Count);

      // Find all the data points from the second series
      ReadOnlyCollection<PieSlice> points2 = series2.DataPoints;
      Assert.AreEqual(points2.Count, data2.Count);

      points1[1].IsSelected = true; // Select a point from the first series and check that it is the only selected point
      Assert.IsFalse(points1[0].IsSelected);
      Assert.IsTrue(points1[1].IsSelected);
      Assert.IsFalse(points1[2].IsSelected);

      Assert.IsFalse(points2[0].IsSelected);
      Assert.IsFalse(points2[1].IsSelected);
      Assert.IsFalse(points2[2].IsSelected);

      // Select a point from the second series. The previous point should be deselected (due to chart.SelectionMode = single).
      // And all the points in the second series should be selected (due to series2.SelectionMode = all).
      points2[1].IsSelected = true;
      Assert.IsFalse(points1[0].IsSelected);
      Assert.IsFalse(points1[1].IsSelected);
      Assert.IsFalse(points1[2].IsSelected);

      Assert.IsTrue(points2[0].IsSelected);
      Assert.IsTrue(points2[1].IsSelected);
      Assert.IsTrue(points2[2].IsSelected);

      // And select a data point from the first series again to test the reverse scenario.
      points1[0].IsSelected = true;
      Assert.IsTrue(points1[0].IsSelected);
      Assert.IsFalse(points1[1].IsSelected);
      Assert.IsFalse(points1[2].IsSelected);

      Assert.IsFalse(points2[0].IsSelected);
      Assert.IsFalse(points2[1].IsSelected);
      Assert.IsFalse(points2[2].IsSelected);
    }

    #endregion // DataSeriesSelectionMode tests
  }
}
