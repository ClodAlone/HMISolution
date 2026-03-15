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
using System.Windows.Data;
using System.Collections.Generic;
using NUnit.Framework;

namespace Mindscape.WpfElements.UnitTests
{
  [TestFixture]
  public class PolarBubbleSeriesTests
  {
    [Test]
    [STAThread]
    public void NumberOfDataPointsIsSameAsItemsSourceCount()
    {
      PolarChart chart = new PolarChart();
      PolarBubbleSeries series = new PolarBubbleSeries();
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
    public void SizeBinding()
    {
      PolarChart chart = new PolarChart();
      PolarBubbleSeries series = new PolarBubbleSeries();
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
      foreach (PolarBubble bubble in series.DataPoints)
      {
        Point3 sd = (Point3)data[index];
        Assert.AreEqual(sd.Z, bubble.Size);
        index++;
      }
    }

    [Test]
    [STAThread]
    public void IsNegative()
    {
      PolarChart chart = new PolarChart();
      PolarBubbleSeries series = new PolarBubbleSeries();
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
      foreach (PolarBubble bubble in series.DataPoints)
      {
        Point3 sd = (Point3)data[index];
        Assert.AreEqual(sd.Z < 0, bubble.IsNegative);
        index++;
      }
    }

    [Test]
    [STAThread]
    public void SeriesBrushIsApplied()
    {
      PolarChart chart = new PolarChart();
      PolarBubbleSeries series = new PolarBubbleSeries();
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
      foreach (PolarBubble bubble in series.DataPoints)
      {
        Assert.AreEqual(brush, bubble.Background);
      }
    }

    [Test]
    [STAThread]
    public void BubbleStyleIsApplied()
    {
      PolarChart chart = new PolarChart();
      PolarBubbleSeries series = new PolarBubbleSeries();

      Style style = new Style(typeof(PolarBubble));
      style.Setters.Add(new Setter(PolarBubble.BorderBrushProperty, new SolidColorBrush(Colors.Magenta)));
      style.Setters.Add(new Setter(PolarBubble.BorderThicknessProperty, new Thickness(2)));
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
      foreach (PolarBubble bubble in series.DataPoints)
      {
        Assert.AreEqual(style, bubble.Style);
      }
    }
  }
}
