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
  public class CategoryAxisSupportTests
  {
    [Test]
    [Ignore("This test needs to be revised")]
    public void XAxisCategorySupport()
    {
      /*Chart chart = new Chart();
      chart.XAxis = new ChartAxis() { Minimum = 0, Maximum = 3, LabelStep = 1 }; // Need to set LabelStep here, otherwise it automatically gets set to undesired value due to ActualWidth being 0 in test environment.
      chart.YAxis = new ChartAxis();
      BarSeries series = new BarSeries();
      series.XBinding = new Binding("String");
      chart.Series.Add(series);
      IList data = new List<StringDouble>();
      data.Add(new StringDouble("Jan", 3));
      data.Add(new StringDouble("Feb", 8));
      data.Add(new StringDouble("Mar", 2.4));
      data.Add(new StringDouble("Apr", 5));
      series.ItemsSource = data;

      Assert.AreEqual(chart.XAxis.Labels.Count, data.Count);
      int index = 0;
      foreach (AxisLabel label in chart.XAxis.Labels)
      {
        StringDouble sd = data[index] as StringDouble;
        Assert.AreEqual(label.Label, sd.String);
        index++;
      }*/
    }

    [Test]
    [Ignore("A performance enhancement prevents this from working in a test environment")]
    public void YAxisCategorySupport()
    {
      /*Chart chart = new Chart();
      chart.XAxis = new ChartAxis();
      chart.YAxis = new ChartAxis() { Minimum = 0, Maximum = 3 };
      SimpleDataSeries series = new SimpleDataSeries();
      series.YBinding = new Binding("String");
      series.XBinding = new Binding("Double");
      chart.Series.Add(series);
      IList data = new List<StringDouble>();
      data.Add(new StringDouble("Jan", 3));
      data.Add(new StringDouble("Feb", 8));
      data.Add(new StringDouble("Mar", 2.4));
      data.Add(new StringDouble("Apr", 5));
      series.ItemsSource = data;

      Assert.AreEqual(chart.YAxis.Labels.Count, data.Count);
      int index = 0;
      foreach (AxisLabel label in chart.YAxis.Labels)
      {
        StringDouble sd = data[index] as StringDouble;
        Assert.AreEqual(label.Label, sd.String);
        index++;
      }*/
    }
  }
}
