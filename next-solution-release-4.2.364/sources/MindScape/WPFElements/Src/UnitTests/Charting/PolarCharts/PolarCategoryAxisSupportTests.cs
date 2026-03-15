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
  public class PolarCategoryAxisSupportTests
  {
    [Test]
    [Ignore("Labels property needs to be revised")]
    public void ThetaAxisCategorySupport()
    {
      /*PolarChart chart = new PolarChart();
      chart.ThetaAxis = new ThetaAxis() { Minimum = 0, Maximum = 4, MajorTickSpacing = 1 }; // We shouldn't really need to set the MajorTickSpacing here...
      chart.RhoAxis = new RhoAxis();
      SimplePolarDataSeries series = new SimplePolarDataSeries();
      series.ThetaBinding = new Binding("String");
      chart.Series.Add(series);
      IList data = new List<StringDouble>();
      data.Add(new StringDouble("Jan", 3));
      data.Add(new StringDouble("Feb", 8));
      data.Add(new StringDouble("Mar", 2.4));
      data.Add(new StringDouble("Apr", 5));
      series.ItemsSource = data;

      Assert.AreEqual(chart.ThetaAxis.Labels.Count, data.Count);
      int index = 0;
      foreach (AxisLabel label in chart.ThetaAxis.Labels)
      {
        StringDouble sd = data[index] as StringDouble;
        Assert.AreEqual(label.Label, sd.String);
        index++;
      }*/
    }

    [Test]
    [Ignore("Labels property needs to be revised")]
    public void RhoAxisCategorySupport()
    {
      /*PolarChart chart = new PolarChart();
      chart.ThetaAxis = new ThetaAxis();
      chart.RhoAxis = new RhoAxis() { Minimum = 0, Maximum = 3, MajorTickSpacing = 1 }; // We shouldn't really need to set the MajorTickSpacing here...
      SimplePolarDataSeries series = new SimplePolarDataSeries();
      series.RhoBinding = new Binding("String");
      chart.Series.Add(series);
      IList data = new List<StringDouble>();
      data.Add(new StringDouble("Jan", 3));
      data.Add(new StringDouble("Feb", 8));
      data.Add(new StringDouble("Mar", 2.4));
      data.Add(new StringDouble("Apr", 5));
      series.ItemsSource = data;

      Assert.AreEqual(chart.RhoAxis.Labels.Count, data.Count);
      int index = 0;
      foreach (AxisLabel label in chart.RhoAxis.Labels)
      {
        StringDouble sd = data[index] as StringDouble;
        Assert.AreEqual(label.Label, sd.String);
        index++;
      }*/
    }
  }
}
