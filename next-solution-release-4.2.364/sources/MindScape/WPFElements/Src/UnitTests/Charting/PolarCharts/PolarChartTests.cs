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
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using NUnit.Framework;

namespace Mindscape.WpfElements.UnitTests
{
  [TestFixture]
  public class PolarChartTests
  {
    [Test]
    [STAThread]
    public void OnlyOneSeriesCanHaveSelectionIfSelectionModeIsSingle()
    {
      PolarChart chart = new PolarChart();
      chart.SelectionMode = DataSeriesSelectionMode.Single;
      chart.ThetaAxis = new ThetaAxis();
      chart.RhoAxis = new RhoAxis();
      SimplePolarDataSeries series1 = new SimplePolarDataSeries();
      // arbitrary data:
      IList data1 = new List<object>();
      data1.Add(new Point(0, 2));
      data1.Add(new Point(1, -4));
      data1.Add(new Point(2, 13));
      chart.Series.Add(series1);
      series1.ItemsSource = data1;

      SimplePolarDataSeries series2 = new SimplePolarDataSeries();
      // arbitrary data:
      IList data2 = new List<object>();
      data2.Add(new Point(0, 4));
      data2.Add(new Point(2, 1));
      data2.Add(new Point(4, -12));
      chart.Series.Add(series2);
      series2.ItemsSource = data2;

      // Find all the data points from the first series.
      ReadOnlyCollection<PolarDataPoint> points1 = series1.DataPoints;
      Assert.AreEqual(points1.Count, data1.Count);

      // Find all the data points from the second series
      ReadOnlyCollection<PolarDataPoint> points2 = series2.DataPoints;
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
      PolarChart chart = new PolarChart();
      chart.SelectionMode = DataSeriesSelectionMode.Multiple;
      chart.ThetaAxis = new ThetaAxis();
      chart.RhoAxis = new RhoAxis();
      SimplePolarDataSeries series1 = new SimplePolarDataSeries();
      // arbitrary data:
      IList data1 = new List<object>();
      data1.Add(new Point(0, 2));
      data1.Add(new Point(1, -4));
      data1.Add(new Point(2, 13));
      chart.Series.Add(series1);
      series1.ItemsSource = data1;

      SimplePolarDataSeries series2 = new SimplePolarDataSeries();
      // arbitrary data:
      IList data2 = new List<object>();
      data2.Add(new Point(0, 4));
      data2.Add(new Point(2, 1));
      data2.Add(new Point(4, -12));
      chart.Series.Add(series2);
      series2.ItemsSource = data2;

      // Find all the data points from the first series.
      ReadOnlyCollection<PolarDataPoint> points1 = series1.DataPoints;
      Assert.AreEqual(points1.Count, data1.Count);

      // Find all the data points from the second series
      ReadOnlyCollection<PolarDataPoint> points2 = series2.DataPoints;
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
      PolarChart chart = new PolarChart();
      chart.SelectionMode = DataSeriesSelectionMode.Multiple;
      chart.ThetaAxis = new ThetaAxis();
      chart.RhoAxis = new RhoAxis();
      SimplePolarDataSeries series1 = new SimplePolarDataSeries();
      // arbitrary data:
      IList data1 = new List<object>();
      data1.Add(new Point(0, 2));
      data1.Add(new Point(1, -4));
      data1.Add(new Point(2, 13));
      chart.Series.Add(series1);
      series1.ItemsSource = data1;

      SimplePolarDataSeries series2 = new SimplePolarDataSeries();
      // arbitrary data:
      IList data2 = new List<object>();
      data2.Add(new Point(0, 4));
      data2.Add(new Point(2, 1));
      data2.Add(new Point(4, -12));
      chart.Series.Add(series2);
      series2.ItemsSource = data2;

      // Find all the data points from the first series.
      ReadOnlyCollection<PolarDataPoint> points1 = series1.DataPoints;
      Assert.AreEqual(points1.Count, data1.Count);

      // Find all the data points from the second series
      ReadOnlyCollection<PolarDataPoint> points2 = series2.DataPoints;
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
      PolarChart chart = new PolarChart();
      chart.SelectionMode = DataSeriesSelectionMode.Single;
      chart.ThetaAxis = new ThetaAxis();
      chart.RhoAxis = new RhoAxis();
      SimplePolarDataSeries series1 = new SimplePolarDataSeries();
      series1.SelectionMode = DataPointSelectionMode.Single;
      SimplePolarDataSeries series2 = new SimplePolarDataSeries();
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
      ReadOnlyCollection<PolarDataPoint> points1 = series1.DataPoints;
      Assert.AreEqual(points1.Count, data1.Count);

      // Find all the data points from the second series
      ReadOnlyCollection<PolarDataPoint> points2 = series2.DataPoints;
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
    [STAThread]
    public void AutomaticLegendTitles()
    {
      PolarChart chart = new PolarChart();
      chart.ThetaAxis = new ThetaAxis();
      chart.RhoAxis = new RhoAxis();
      SimplePolarDataSeries series1 = new SimplePolarDataSeries();
      SimplePolarDataSeries series2 = new SimplePolarDataSeries();
      SimplePolarDataSeries series3 = new SimplePolarDataSeries();
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
