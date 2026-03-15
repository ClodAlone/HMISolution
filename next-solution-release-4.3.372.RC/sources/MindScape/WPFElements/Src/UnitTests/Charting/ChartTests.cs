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
using System.Collections.ObjectModel;
using Mindscape.WpfElements.Charting;
using NUnit.Framework;

namespace Mindscape.WpfElements.UnitTests
{
  [TestFixture]
  public class ChartTests
  {
    [Test]
    [Ignore("This test needs to be revised")]
    public void OnlyOneSeriesCanHaveSelectionIfSelectionModeIsSingle()
    {
      Chart chart = new Chart();
      chart.SelectionMode = DataSeriesSelectionMode.Single;
      chart.XAxis = new ChartAxis();
      chart.YAxis = new ChartAxis();
      BarSeries series1 = new BarSeries();
      // arbitrary data:
      IList data1 = new List<object>();
      data1.Add(new Point(0, 2));
      data1.Add(new Point(1, -4));
      data1.Add(new Point(2, 13));
      chart.Series.Add(series1);
      series1.ItemsSource = data1;

      BarSeries series2 = new BarSeries();
      // arbitrary data:
      IList data2 = new List<object>();
      data2.Add(new Point(0, 4));
      data2.Add(new Point(2, 1));
      data2.Add(new Point(4, -12));
      chart.Series.Add(series2);
      series2.ItemsSource = data2;

      // Find all the data points from the first series.
      ReadOnlyCollection<CartesianDataPoint> points1 = series1.DataPoints;
      Assert.AreEqual(points1.Count, data1.Count);

      // Find all the data points from the second series
      ReadOnlyCollection<CartesianDataPoint> points2 = series2.DataPoints;
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
    [Ignore("This test needs to be revised")]
    public void MultipleDataSeriesCanHaveSelectionIfSelectionModeIsMultiple()
    {
      Chart chart = new Chart();
      chart.SelectionMode = DataSeriesSelectionMode.Multiple;
      chart.XAxis = new ChartAxis();
      chart.YAxis = new ChartAxis();
      BarSeries series1 = new BarSeries();
      // arbitrary data:
      IList data1 = new List<object>();
      data1.Add(new Point(0, 2));
      data1.Add(new Point(1, -4));
      data1.Add(new Point(2, 13));
      chart.Series.Add(series1);
      series1.ItemsSource = data1;

      BarSeries series2 = new BarSeries();
      // arbitrary data:
      IList data2 = new List<object>();
      data2.Add(new Point(0, 4));
      data2.Add(new Point(2, 1));
      data2.Add(new Point(4, -12));
      chart.Series.Add(series2);
      series2.ItemsSource = data2;

      // Find all the data points from the first series.
      ReadOnlyCollection<CartesianDataPoint> points1 = series1.DataPoints;
      Assert.AreEqual(points1.Count, data1.Count);

      // Find all the data points from the second series
      ReadOnlyCollection<CartesianDataPoint> points2 = series2.DataPoints;
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
    [Ignore("This test needs to be revised")]
    public void ChangingToSingleSelectionModeIsRespected()
    {
      Chart chart = new Chart();
      chart.SelectionMode = DataSeriesSelectionMode.Multiple;
      chart.XAxis = new ChartAxis();
      chart.YAxis = new ChartAxis();
      BarSeries series1 = new BarSeries();
      // arbitrary data:
      IList data1 = new List<object>();
      data1.Add(new Point(0, 2));
      data1.Add(new Point(1, -4));
      data1.Add(new Point(2, 13));
      chart.Series.Add(series1);
      series1.ItemsSource = data1;

      BarSeries series2 = new BarSeries();
      // arbitrary data:
      IList data2 = new List<object>();
      data2.Add(new Point(0, 4));
      data2.Add(new Point(2, 1));
      data2.Add(new Point(4, -12));
      chart.Series.Add(series2);
      series2.ItemsSource = data2;

      // Find all the data points from the first series.
      ReadOnlyCollection<CartesianDataPoint> points1 = series1.DataPoints;
      Assert.AreEqual(points1.Count, data1.Count);

      // Find all the data points from the second series
      ReadOnlyCollection<CartesianDataPoint> points2 = series2.DataPoints;
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
    [Ignore("This test needs to be revised")]
    public void SingleSelectionModeWorksWhenEachSeriesHasDifferentSelectionModes()
    {
      Chart chart = new Chart();
      chart.SelectionMode = DataSeriesSelectionMode.Single;
      chart.XAxis = new ChartAxis();
      chart.YAxis = new ChartAxis();
      SimpleDataSeries series1 = new SimpleDataSeries();
      series1.SelectionMode = DataPointSelectionMode.Single;
      SimpleDataSeries series2 = new SimpleDataSeries();
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
      ReadOnlyCollection<CartesianDataPoint> points1 = series1.DataPoints;
      Assert.AreEqual(points1.Count, data1.Count);

      // Find all the data points from the second series
      ReadOnlyCollection<CartesianDataPoint> points2 = series2.DataPoints;
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

    [Test]
    [Ignore("The series names can no longer be set until the chart is loaded so that bindings will work. So this can not be tested currently")]
    public void AutomaticLegendTitles()
    {
      Chart chart = new Chart();
      chart.XAxis = new ChartAxis();
      chart.YAxis = new ChartAxis();
      SimpleDataSeries series1 = new SimpleDataSeries();
      SimpleDataSeries series2 = new SimpleDataSeries();
      SimpleDataSeries series3 = new SimpleDataSeries();
      chart.Series.Add(series1);
      chart.Series.Add(series2);
      chart.Series.Add(series3);

      Assert.AreEqual(3, chart.LegendItems.Count);
      Assert.AreEqual(chart.Series.Count, chart.LegendItems.Count);
      int index = 0;
      foreach (LegendItem item in chart.LegendItems)
      {
        Assert.AreEqual("Series " + (index + 1), item.LegendLabel);
        index++;
      }
    }
  }
}
