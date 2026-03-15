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
  public class RoseSeriesTests
  {
    [Test]
    [STAThread]
    public void NumberOfDataPointsIsSameAsItemsSourceCount()
    {
      PolarChart chart = new PolarChart();
      RoseSeries series = new RoseSeries();
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
    public void BarStyleIsApplied()
    {
      PolarChart chart = new PolarChart();
      chart.ThetaAxis = new ThetaAxis();
      chart.RhoAxis = new RhoAxis();
      RoseSeries series = new RoseSeries();

      Style style = new Style(typeof(PolarBar));
      style.Setters.Add(new Setter(PolarBar.BorderThicknessProperty, new Thickness(0)));
      style.Setters.Add(new Setter(PolarBar.OpacityProperty, 0.5));
      series.BarStyle = style;

      chart.Series.Add(series);
      IList data = new List<object>();
      data.Add(new Point(0, 5));
      data.Add(new Point(1, 5));
      data.Add(new Point(2, 5));
      data.Add(new Point(3, 5));
      series.ItemsSource = data;

      Assert.AreEqual(series.DataPoints.Count, series.ItemsSource.Count);
      foreach (PolarBar dp in series.DataPoints)
      {
        Assert.AreEqual(style, dp.Style);
      }
    }

    [Test]
    [STAThread]
    public void OneLegendItemForEachRoseSeriesByDefault()
    {
      PolarChart chart = new PolarChart();
      chart.ThetaAxis = new ThetaAxis();
      chart.RhoAxis = new RhoAxis();
      RoseSeries series1 = new RoseSeries() { Title = "First Rose Series" };
      RoseSeries series2 = new RoseSeries() { Title = "Second Rose Series" };
      chart.Series.Add(series1);
      chart.Series.Add(series2);

      Assert.AreEqual(2, chart.LegendItems.Count);
      Assert.AreEqual(series1.Title, chart.LegendItems[0].LegendLabel);
      Assert.AreEqual(series2.Title, chart.LegendItems[1].LegendLabel);
    }

    // TODO: implement ShowAllLegendItems property for RoseSeries?
    /*[Test]
    public void AutomaticLegendTitles()
    {
      PolarChart chart = new PolarChart();
      chart.ThetaAxis = new ThetaAxis();
      chart.RhoAxis = new RhoAxis();
      RoseSeries series = new RoseSeries();
      series.ShowAllLegendItems = true; // So that each data point has a legend item
      chart.Series.Add(series);
      IList data = new List<object>();
      data.Add(new Point(0, 5));
      data.Add(new Point(1, 5));
      data.Add(new Point(2, 5));
      data.Add(new Point(3, 5));
      series.ItemsSource = data;

      Assert.AreEqual(chart.LegendItems.Count, series.ItemsSource.Count);
      int index = 0;
      foreach (LegendItem item in chart.LegendItems)
      {
        Assert.AreEqual("Item " + (index + 1), item.LegendLabel);
        index++;
      }
    }*/
  }
}
