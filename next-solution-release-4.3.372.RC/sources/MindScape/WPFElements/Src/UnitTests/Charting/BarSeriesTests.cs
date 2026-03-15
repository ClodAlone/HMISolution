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

namespace Mindscape.WpfElements.UnitTests
{
  [TestFixture]
  public class BarSeriesTests
  {
    [Test]
    [Ignore("Performance improvements prevents this from working in the test environment")]
    public void NumberOfDataPointsIsSameAsItemsSourceCount()
    {
      Chart chart = new Chart();
      BarSeries series = new BarSeries();
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
    [Ignore("This test needs to be revised")]
    public void Bar_IsNegativePropertyIsCorrect_Vertical()
    {
      Chart chart = new Chart();
      chart.XAxis = new ChartAxis();
      chart.YAxis = new ChartAxis();
      BarSeries series = new BarSeries();
      IList data = new List<object>();
      data.Add(new Point(0, 5));
      data.Add(new Point(1, -3));
      chart.Series.Add(series);
      series.ItemsSource = data; // This line causes the series to be graphically built. There is a test for this elsewhere.

      int count = 0;
      // Look through the canvas for the generated Bar objects.
      foreach(UIElement element in series.Canvas.Children)
      {
        Bar bar = element as Bar;
        if (bar != null)
        {
          count++;
          // The Bar.IsNegative property should be true if the LogicalPoint.Y value is less than 0.
          Assert.AreEqual(bar.LogicalPoint.Y < 0, bar.IsNegative);
        }
      }
      // Make sure that we tested the same number of Bar objects as the size of the data collection.
      Assert.AreEqual(count, data.Count);
    }

    [Test]
    [Ignore("This test needs to be revised")]
    public void Bar_IsNegativePropertyIsCorrect_Horizontal()
    {
      Chart chart = new Chart();
      chart.XAxis = new ChartAxis();
      chart.YAxis = new ChartAxis();
      BarSeries series = new BarSeries();
      series.Orientation = Orientation.Horizontal;
      IList data = new List<object>();
      data.Add(new Point(5, 0));
      data.Add(new Point(-3, 1));
      chart.Series.Add(series);
      series.ItemsSource = data; // This line causes the series to be graphically built. There is a test for this elsewhere.

      int count = 0;
      // Look through the canvas for the generated Bar objects.
      foreach (UIElement element in series.Canvas.Children)
      {
        Bar bar = element as Bar;
        if (bar != null)
        {
          count++;
          // The Bar.IsNegative property should be true if the LogicalPoint.X value is less than 0.
          Assert.AreEqual(bar.LogicalPoint.X < 0, bar.IsNegative);
        }
      }
      // Make sure that we tested the same number of Bar objects as the size of the data collection.
      Assert.AreEqual(count, data.Count);
    }

    [Test]
    [Ignore("This test needs to be revised")]
    public void BarOrientationIsSeriesOrientation_Vertical()
    {
      Chart chart = new Chart();
      chart.XAxis = new ChartAxis();
      chart.YAxis = new ChartAxis();
      BarSeries series = new BarSeries();
      series.Orientation = Orientation.Vertical;
      IList data = new List<object>();
      data.Add(new Point(0, 3));
      data.Add(new Point(1, -4));
      chart.Series.Add(series);
      series.ItemsSource = data; // This line causes the series to be graphically built. There is a test for this elsewhere.

      int count = 0;
      // Look through the canvas for the generated Bar objects.
      foreach (UIElement element in series.Canvas.Children)
      {
        Bar bar = element as Bar;
        if (bar != null)
        {
          count++;
          Assert.AreEqual(bar.Orientation, Orientation.Vertical);
        }
      }
      // Make sure that we tested the same number of Bar objects as the size of the data collection.
      Assert.AreEqual(count, data.Count);
    }

    [Test]
    [Ignore("This test needs to be revised")]
    public void BarOrientationIsSeriesOrientation_Horizontal()
    {
      Chart chart = new Chart();
      chart.XAxis = new ChartAxis();
      chart.YAxis = new ChartAxis();
      BarSeries series = new BarSeries();
      series.Orientation = Orientation.Horizontal;
      IList data = new List<object>();
      data.Add(new Point(3, 0));
      data.Add(new Point(-4, 1));
      chart.Series.Add(series);
      series.ItemsSource = data; // This line causes the series to be graphically built. There is a test for this elsewhere.

      int count = 0;
      // Look through the canvas for the generated Bar objects.
      foreach (UIElement element in series.Canvas.Children)
      {
        Bar bar = element as Bar;
        if (bar != null)
        {
          count++;
          Assert.AreEqual(bar.Orientation, Orientation.Horizontal);
        }
      }
      // Make sure that we tested the same number of Bar objects as the size of the data collection.
      Assert.AreEqual(count, data.Count);
    }

    [Test]
    [Ignore("This test needs to be revised")]
    public void BrushesAreAppliedSequentially()
    {
      Chart chart = new Chart();
      chart.XAxis = new ChartAxis();
      chart.YAxis = new ChartAxis();
      BarSeries series = new BarSeries();
      chart.Series.Add(series);
      series.Brushes.Clear();
      series.Brushes.Add(new SolidColorBrush() { Color = Colors.Red });
      series.Brushes.Add(new SolidColorBrush() { Color = Colors.Green });
      series.Brushes.Add(new SolidColorBrush() { Color = Colors.Blue });
      IList data = new List<object>();
      data.Add(new Point(0, 5));
      data.Add(new Point(1, 5));
      data.Add(new Point(2, 5));
      series.ItemsSource = data;

      Assert.AreEqual(series.DataPoints.Count, series.ItemsSource.Count);
      int index = 0;
      foreach (DataPoint dp in series.DataPoints)
      {
        Assert.AreEqual(series.Brushes[index], dp.Background);
        index++;
      }
    }

    [Test]
    [Ignore("Now that bar charts can use data sampling, the DataPoints collection is not fully populated in a testing environment")]
    public void BrushesAreWrappedIfTooManyDataPoints()
    {
      Chart chart = new Chart();
      chart.XAxis = new ChartAxis();
      chart.YAxis = new ChartAxis();
      BarSeries series = new BarSeries();
      chart.Series.Add(series);
      series.Brushes.Clear();
      series.Brushes.Add(new SolidColorBrush() { Color = Colors.Red });
      series.Brushes.Add(new SolidColorBrush() { Color = Colors.Green });
      series.Brushes.Add(new SolidColorBrush() { Color = Colors.Blue });
      IList data = new List<object>();
      data.Add(new Point(0, 5));
      data.Add(new Point(1, 5));
      data.Add(new Point(2, 5));
      data.Add(new Point(3, 5)); // 4 data points, but only 3 brushes.
      series.ItemsSource = data;

      Assert.AreEqual(series.DataPoints.Count, series.ItemsSource.Count);
      int index = 0;
      foreach (DataPoint dp in series.DataPoints)
      {
        Assert.AreEqual(series.Brushes[index], dp.Background);
        index = ++index % series.Brushes.Count; // wrap the index based on number
      }
    }

    [Test]
    [Ignore("Now that bar charts can use data sampling, the DataPoints collection is not fully populated in a testing environment")]
    public void SeriesBrushIsUsedIfBrushesIsEmpty()
    {
      Chart chart = new Chart();
      chart.XAxis = new ChartAxis();
      chart.YAxis = new ChartAxis();
      BarSeries series = new BarSeries();
      Brush defaultBrush = new SolidColorBrush(Colors.Brown);
      series.SeriesBrush = defaultBrush;
      chart.Series.Add(series);
      IList data = new List<object>();
      data.Add(new Point(0, 5));
      data.Add(new Point(1, 5));
      data.Add(new Point(2, 5));
      data.Add(new Point(3, 5));
      series.ItemsSource = data;

      Assert.AreEqual(0, series.Brushes.Count);
      Assert.AreEqual(defaultBrush, series.SeriesBrush);
      Assert.AreEqual(series.DataPoints.Count, series.ItemsSource.Count);
      foreach (DataPoint dp in series.DataPoints)
      {
        Assert.AreEqual(defaultBrush, dp.Background);
      }
    }

    [Test]
    [Ignore("Now that bar charts can use data sampling, the DataPoints collection is not fully populated in a testing environment")]
    public void BarStyleIsApplied()
    {
      Chart chart = new Chart();
      chart.XAxis = new ChartAxis();
      chart.YAxis = new ChartAxis();
      BarSeries series = new BarSeries();

      Style style = new Style(typeof(Bar));
      style.Setters.Add(new Setter(Bar.BorderThicknessProperty, new Thickness(0)));
      style.Setters.Add(new Setter(Bar.OpacityProperty, 0.5));
      series.BarStyle = style;

      chart.Series.Add(series);
      IList data = new List<object>();
      data.Add(new Point(0, 5));
      data.Add(new Point(1, 5));
      data.Add(new Point(2, 5));
      data.Add(new Point(3, 5));
      series.ItemsSource = data;

      Assert.AreEqual(series.DataPoints.Count, series.ItemsSource.Count);
      foreach (DataPoint dp in series.DataPoints)
      {
        Assert.AreEqual(style, dp.Style);
      }
    }

    [Test]
    [Ignore("Now that bar charts can use data sampling, the DataPoints collection is not fully populated in a testing environment")]
    public void TitleBinding()
    {
      Chart chart = new Chart();
      chart.XAxis = new ChartAxis();
      chart.YAxis = new ChartAxis();
      BarSeries series = new BarSeries();
      series.XBinding = new Binding("Double");
      series.YBinding = new Binding("Double");
      series.TitleBinding = new Binding("String");
      chart.Series.Add(series);
      IList data = new List<object>();
      data.Add(new StringDouble("Jan", 0));
      data.Add(new StringDouble("Feb", 1));
      data.Add(new StringDouble("Mar", 1));
      data.Add(new StringDouble("Apr", 1));
      series.ItemsSource = data;

      Assert.AreEqual(series.DataPoints.Count, series.ItemsSource.Count);
      int index = 0;
      foreach (Bar bar in series.DataPoints)
      {
        StringDouble sd = data[index] as StringDouble;
        Assert.AreEqual(sd.String, bar.Title);
        index++;
      }
    }

    [Test]
    [STAThread]
    public void TitleBindingAffectsLegendItems()
    {
      Chart chart = new Chart();
      chart.XAxis = new ChartAxis();
      chart.YAxis = new ChartAxis();
      BarSeries series = new BarSeries();
      series.ShowAllLegendItems = true; // So that each data point has a legend item
      series.XBinding = new Binding("Double");
      series.YBinding = new Binding("Double");
      series.TitleBinding = new Binding("String");
      chart.Series.Add(series);
      IList data = new List<object>();
      data.Add(new StringDouble("Jan", 0));
      data.Add(new StringDouble("Feb", 1));
      data.Add(new StringDouble("Mar", 1));
      data.Add(new StringDouble("Apr", 1));
      series.ItemsSource = data;

      Assert.AreEqual(chart.LegendItems.Count, series.ItemsSource.Count);
      int index = 0;
      foreach (LegendItem item in chart.LegendItems)
      {
        StringDouble sd = data[index] as StringDouble;
        Assert.AreEqual(item.LegendLabel, sd.String);
        index++;
      }
    }

    [Test]
    [STAThread]
    public void OneLegendItemForEachBarSeriesByDefault()
    {
      Chart chart = new Chart();
      chart.XAxis = new ChartAxis();
      chart.YAxis = new ChartAxis();
      BarSeries series1 = new BarSeries() { Title = "First Bar Series" };
      BarSeries series2 = new BarSeries() { Title = "Second Bar Series" };
      chart.Series.Add(series1);
      chart.Series.Add(series2);

      Assert.AreEqual(2, chart.LegendItems.Count);
      Assert.AreEqual(series1.Title, chart.LegendItems[0].LegendLabel);
      Assert.AreEqual(series2.Title, chart.LegendItems[1].LegendLabel);
    }

    [Test]
    [STAThread]
    public void AutomaticLegendTitles()
    {
      Chart chart = new Chart();
      chart.XAxis = new ChartAxis();
      chart.YAxis = new ChartAxis();
      BarSeries series = new BarSeries();
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
    }
  }
}
