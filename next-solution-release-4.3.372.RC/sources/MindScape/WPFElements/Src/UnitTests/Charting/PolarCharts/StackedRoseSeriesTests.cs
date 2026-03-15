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
using System.Windows.Data;
using NUnit.Framework;

namespace Mindscape.WpfElements.UnitTests
{
  [TestFixture]
  public class StackedRoseSeriesTests
  {
    [Test]
    [STAThread]
    public void NumberOfDataPointsIsSameAsItemsSourceCount()
    {
      PolarChart chart = new PolarChart();
      StackedRoseSeries series = new StackedRoseSeries();
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
    public void SeriesBrush()
    {
      PolarChart chart = new PolarChart();
      chart.ThetaAxis = new ThetaAxis();
      chart.RhoAxis = new RhoAxis();
      StackedRoseSeries series = new StackedRoseSeries();
      Brush defaultBrush = new SolidColorBrush(Colors.Brown);
      series.SeriesBrush = defaultBrush;
      chart.Series.Add(series);
      IList data = new List<object>();
      data.Add(new PolarPoint(0, 5));
      data.Add(new PolarPoint(1, 5));
      data.Add(new PolarPoint(2, 5));
      data.Add(new PolarPoint(3, 5));
      series.ItemsSource = data;

      Assert.AreEqual(defaultBrush, series.SeriesBrush);
      Assert.AreEqual(series.DataPoints.Count, series.ItemsSource.Count);
      foreach (DataPoint dp in series.DataPoints)
      {
        Assert.AreEqual(defaultBrush, dp.Background);
      }
    }

    [Test]
    [STAThread]
    public void BarStyleIsApplied()
    {
      PolarChart chart = new PolarChart();
      chart.ThetaAxis = new ThetaAxis();
      chart.RhoAxis = new RhoAxis();
      StackedRoseSeries series = new StackedRoseSeries();

      Style style = new Style(typeof(PolarBar));
      style.Setters.Add(new Setter(PolarBar.BorderThicknessProperty, new Thickness(0)));
      style.Setters.Add(new Setter(PolarBar.OpacityProperty, 0.5));
      series.BarStyle = style;

      chart.Series.Add(series);
      IList data = new List<object>();
      data.Add(new PolarPoint(0, 5));
      data.Add(new PolarPoint(1, 5));
      data.Add(new PolarPoint(2, 5));
      data.Add(new PolarPoint(3, 5));
      series.ItemsSource = data;

      Assert.AreEqual(series.DataPoints.Count, series.ItemsSource.Count);
      foreach (DataPoint dp in series.DataPoints)
      {
        Assert.AreEqual(style, dp.Style);
      }
    }

    [Test]
    [STAThread]
    public void OneLegendItemForEachBarSeriesByDefault()
    {
      PolarChart chart = new PolarChart();
      chart.ThetaAxis = new ThetaAxis();
      chart.RhoAxis = new RhoAxis();
      StackedRoseSeries series1 = new StackedRoseSeries() { Title = "First Bar Series" };
      StackedRoseSeries series2 = new StackedRoseSeries() { Title = "Second Bar Series" };
      chart.Series.Add(series1);
      chart.Series.Add(series2);

      Assert.AreEqual(2, chart.LegendItems.Count);
      Assert.AreEqual(series1.Title, chart.LegendItems[0].LegendLabel);
      Assert.AreEqual(series2.Title, chart.LegendItems[1].LegendLabel);
    }
  }
}
