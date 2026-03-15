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
using Mindscape.WpfElements.Charting;
using NUnit.Framework;

namespace Mindscape.WpfElements.UnitTests
{
  [TestFixture]
  public class DataPointTests
  {
    // NOTE: The IsSelected property of a DataPoint can behave differently depending on if it was created by a DataSeries or not.
    // If it is created by a data series, then the DataSeries.SelectionMode property will determine if the IsSelected property
    // can be changed or not.
    // If the data point is created by using its constructor, then there is no constraint on setting the IsSelected property.
    // Because of this, I have included tests for both situations. This is just to make sure that they both work.

    private bool _eventRaised;

    [Test]
    [Ignore("This test needs to be revised")]
    public void SelectionProperty()
    {
      Chart chart = new Chart();
      chart.XAxis = new ChartAxis();
      chart.YAxis = new ChartAxis();
      BarSeries series = new BarSeries();
      // arbitrary data:
      IList data = new List<object>();
      data.Add(new Point(0, 2));
      data.Add(new Point(1, -4));
      chart.Series.Add(series);
      series.ItemsSource = data;

      series.SelectionMode = DataPointSelectionMode.Single;

      int count = 0;
      foreach (UIElement element in series.Canvas.Children)
      {
        DataPoint dp = element as DataPoint;
        if (dp != null)
        {
          dp.IsSelected = true;
          Assert.AreEqual(dp.IsSelected, true);
          dp.IsSelected = false;
          Assert.AreEqual(dp.IsSelected, false);
          count++;
        }
      }
      Assert.AreEqual(count, data.Count); // Make sure we tested the same number of data points as the size of the data collection.
    }

    [Test]
    [STAThread]
    public void SelectionPropertyWorksWhenNotCreatedBySeries()
    {
      SimpleDataPoint dp = new SimpleDataPoint();
      dp.IsSelected = true;
      Assert.IsTrue(dp.IsSelected);
      dp.IsSelected = false;
      Assert.IsFalse(dp.IsSelected);
    }

    [Test]
    [Ignore("This test needs to be revised")]
    public void SelectionChangedEventIsRaised()
    {
      Chart chart = new Chart();
      chart.XAxis = new ChartAxis();
      chart.YAxis = new ChartAxis();
      BarSeries series = new BarSeries();
      // arbitrary data:
      IList data = new List<object>();
      data.Add(new Point(0, 2));
      data.Add(new Point(1, -4));
      chart.Series.Add(series);
      series.ItemsSource = data;

      series.SelectionMode = DataPointSelectionMode.Single;

      DataPoint dp = null;
      foreach (UIElement element in series.Canvas.Children)
      {
        dp = element as DataPoint;
        if (dp != null)
        {
          break;
        }
      }
      Assert.IsNotNull(dp);
      dp.IsSelectedChanged += new EventHandler(DataPoint_IsSelectedChanged);

      _eventRaised = false;
      dp.IsSelected = true;
      Assert.IsTrue(_eventRaised);

      _eventRaised = false;
      dp.IsSelected = false;
      Assert.IsTrue(_eventRaised);
    }

    [Test]
    [STAThread]
    public void SelectionChangedEventRaisedWhenNotCreatedBySeries()
    {
      SimpleDataPoint dp = new SimpleDataPoint();
      dp.IsSelectedChanged += new EventHandler(DataPoint_IsSelectedChanged);

      _eventRaised = false;
      dp.IsSelected = true;
      Assert.IsTrue(_eventRaised);

      _eventRaised = false;
      dp.IsSelected = false;
      Assert.IsTrue(_eventRaised);
    }

    [Test]
    [Ignore("This test needs to be revised")]
    public void SelectionChangedEventIsNotRaisedWhenSelectionModeIsNone()
    {
      Chart chart = new Chart();
      chart.XAxis = new ChartAxis();
      chart.YAxis = new ChartAxis();
      BarSeries series = new BarSeries();
      // arbitrary data:
      IList data = new List<object>();
      data.Add(new Point(0, 2));
      data.Add(new Point(1, -4));
      chart.Series.Add(series);
      series.ItemsSource = data;

      series.SelectionMode = DataPointSelectionMode.None;

      DataPoint dp = null;
      foreach (UIElement element in series.Canvas.Children)
      {
        dp = element as DataPoint;
        if (dp != null)
        {
          break;
        }
      }
      Assert.IsNotNull(dp);
      dp.IsSelectedChanged += new EventHandler(DataPoint_IsSelectedChanged);

      _eventRaised = false;
      dp.IsSelected = true;
      Assert.IsFalse(dp.IsSelected);
      Assert.IsFalse(_eventRaised);
    }

    [Test]
    [Ignore("This test needs to be revised")]
    public void SelectionChangedEventIsNotRaisedWhenSelectionIsNotChanged()
    {
      Chart chart = new Chart();
      chart.XAxis = new ChartAxis();
      chart.YAxis = new ChartAxis();
      BarSeries series = new BarSeries();
      // arbitrary data:
      IList data = new List<object>();
      data.Add(new Point(0, 2));
      data.Add(new Point(1, -4));
      chart.Series.Add(series);
      series.ItemsSource = data;

      series.SelectionMode = DataPointSelectionMode.Single;

      DataPoint dp = null;
      foreach (UIElement element in series.Canvas.Children)
      {
        dp = element as DataPoint;
        if (dp != null)
        {
          break;
        }
      }
      Assert.IsNotNull(dp);
      dp.IsSelectedChanged += new EventHandler(DataPoint_IsSelectedChanged);

      _eventRaised = false;
      Assert.IsFalse(dp.IsSelected); // data point should not be selected
      dp.IsSelected = false; // Set the data point to not be selected
      Assert.IsFalse(_eventRaised); // Event should not raise because IsSelected value is not changed

      dp.IsSelected = true;
      _eventRaised = false;
      Assert.IsTrue(dp.IsSelected); // data point should now be selected
      dp.IsSelected = true; // Set the data point to be selected
      Assert.IsFalse(_eventRaised); // Event should not raise because IsSelected value is not changed
    }

    private void DataPoint_IsSelectedChanged(object sender, EventArgs e)
    {
      _eventRaised = true;
    }
  }
}
