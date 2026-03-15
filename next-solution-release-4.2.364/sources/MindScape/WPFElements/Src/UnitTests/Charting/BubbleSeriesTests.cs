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
using System.Windows.Data;
using System.Collections;
using System.Collections.Generic;
using Mindscape.WpfElements.Charting;
using NUnit.Framework;

namespace Mindscape.WpfElements.UnitTests
{
  [TestFixture]
  public class BubbleSeriesTests
  {
    [Test]
    [Ignore("This test needs to be revised")]
    public void NumberOfDataPointsIsSameAsItemsSourceCount()
    {
      Chart chart = new Chart();
      BubbleSeries series = new BubbleSeries();
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
    public void SizeBinding()
    {
      Chart chart = new Chart();
      BubbleSeries series = new BubbleSeries();
      series.SizeBinding = new Binding("Z");
      chart.Series.Add(series);
      IList data = new List<object>();
      data.Add(new Point3(1.2, 2.3, 3.4));
      data.Add(new Point3(10.2, 20.3, 30.4));
      data.Add(new Point3(5.7, 2.0, 9.6));
      data.Add(new Point3(2.1, 9.8, -1.6));
      series.ItemsSource = data;

      Assert.AreEqual(4, series.ItemsSource.Count);
      Assert.AreEqual(series.DataPoints.Count, series.ItemsSource.Count);
      int index = 0;
      foreach (Bubble bubble in series.DataPoints)
      {
        Point3 sd = (Point3)data[index];
        Assert.AreEqual(sd.Z, bubble.Size);
        index++;
      }
    }

    [Test]
    [Ignore("This test needs to be revised")]
    public void IsNegative()
    {
      Chart chart = new Chart();
      BubbleSeries series = new BubbleSeries();
      series.SizeBinding = new Binding("Z");
      chart.Series.Add(series);
      IList data = new List<object>();
      data.Add(new Point3(1.2, 2.3, 3.4));
      data.Add(new Point3(10.2, 20.3, -30.4));
      data.Add(new Point3(5.7, 2.0, 9.6));
      data.Add(new Point3(2.1, 9.8, -1.6));
      series.ItemsSource = data;

      Assert.AreEqual(4, series.ItemsSource.Count);
      Assert.AreEqual(series.DataPoints.Count, series.ItemsSource.Count);
      int index = 0;
      foreach (Bubble bubble in series.DataPoints)
      {
        Point3 sd = (Point3)data[index];
        Assert.AreEqual(sd.Z < 0, bubble.IsNegative);
        index++;
      }
    }

    [Test]
    [Ignore("This test needs to be revised")]
    public void SeriesBrushIsApplied()
    {
      Chart chart = new Chart();
      BubbleSeries series = new BubbleSeries();
      SolidColorBrush brush = new SolidColorBrush(Colors.Cyan);
      series.SeriesBrush = brush;
      series.SizeBinding = new Binding("Z");
      chart.Series.Add(series);
      // Abitrary data
      IList data = new List<object>();
      data.Add(new Point3(1.2, 2.3, 3.4));
      data.Add(new Point3(10.2, 20.3, -30.4));
      data.Add(new Point3(5.7, 2.0, 9.6));
      data.Add(new Point3(2.1, 9.8, -1.6));
      series.ItemsSource = data;

      Assert.AreEqual(4, series.ItemsSource.Count);
      Assert.AreEqual(series.DataPoints.Count, series.ItemsSource.Count);
      foreach (Bubble bubble in series.DataPoints)
      {
        Assert.AreEqual(brush, bubble.Background);
      }
    }

    [Test]
    [Ignore("This test needs to be revised")]
    public void BubbleStyleIsApplied()
    {
      Chart chart = new Chart();
      BubbleSeries series = new BubbleSeries();

      Style style = new Style(typeof(Bubble));
      style.Setters.Add(new Setter(Bubble.BorderBrushProperty, new SolidColorBrush(Colors.Magenta)));
      style.Setters.Add(new Setter(Bubble.BorderThicknessProperty, new Thickness(2)));
      series.BubbleStyle = style;

      series.SizeBinding = new Binding("Z");
      chart.Series.Add(series);
      // Abitrary data
      IList data = new List<object>();
      data.Add(new Point3(1.2, 2.3, 3.4));
      data.Add(new Point3(10.2, 20.3, -30.4));
      data.Add(new Point3(5.7, 2.0, 9.6));
      data.Add(new Point3(2.1, 9.8, -1.6));
      series.ItemsSource = data;

      Assert.AreEqual(4, series.ItemsSource.Count);
      Assert.AreEqual(series.DataPoints.Count, series.ItemsSource.Count);
      foreach (Bubble bubble in series.DataPoints)
      {
        Assert.AreEqual(style, bubble.Style);
      }
    }
  }
}
