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
  public class ChartAxisTests
  {
    // This test no longer works due to the charting performance enhancements that involve using a dispatcher.
    // The axis.Labels property is now null.
    // This test isn't super important, but will need a new way of performing it.
    /*[Test]
    public void LabelLevelCountIsRespected()
    {
      ChartAxis axis = new ChartAxis();
      Assert.AreEqual(axis.LabelLevelCount, 1);
      axis.LabelLevelCount = 2;
      Assert.AreEqual(axis.LabelLevelCount, 2);
      axis.Width = 100;
      axis.Orientation = Orientation.Horizontal;
      axis.Minimum = 0;
      axis.Maximum = 10;
      double firstLevelOffset = axis.Labels[0].LevelOffset;
      double secondLevelOffset = axis.Labels[1].LevelOffset;
      double thirdLevelOffset = axis.Labels[2].LevelOffset;
      double fourthLevelOffset = axis.Labels[3].LevelOffset;
      // every second level offset should be the same. Adjacent labels should have different level offsets.
      Assert.AreNotEqual(firstLevelOffset, secondLevelOffset);
      Assert.AreEqual(firstLevelOffset, thirdLevelOffset);
      Assert.AreNotEqual(thirdLevelOffset, fourthLevelOffset);
      Assert.AreEqual(secondLevelOffset, fourthLevelOffset);
    }*/

    [Test]
    [STAThread]
    public void LabelLevelCountCanNotBeLessThanOne()
    {
      ChartAxis axis = new ChartAxis();
      Assert.AreEqual(1, axis.LabelLevelCount);
      axis.LabelLevelCount = 0;
      Assert.AreEqual(1, axis.LabelLevelCount);
      axis.LabelLevelCount = -2;
      Assert.AreEqual(1, axis.LabelLevelCount);
    }

    [Test]
    [Ignore("This no longer works due to performance enhancements, this test needs to be revised")]
    public void XAxisRangeIsSetAutomatically()
    {
      Chart chart = new Chart();
      IList data = new List<object>();
      data.Add(new Point(0, 3));
      data.Add(new Point(1, 7));
      data.Add(new Point(2, 4));
      data.Add(new Point(3, -3.4));
      data.Add(new Point(4, 6));
      BarSeries series = new BarSeries();
      chart.Series.Add(series);
      series.ItemsSource = data;

      Assert.AreEqual(0, chart.XAxis.Minimum);
      Assert.AreEqual(4, chart.XAxis.Maximum);
    }

    [Test]
    [Ignore("This no longer works due to performance enhancements, this test needs to be revised")]
    public void XAxisRangeIsSetAutomatically_StartingAboveZero()
    {
      Chart chart = new Chart();
      IList data = new List<object>();
      data.Add(new Point(2, 4));
      data.Add(new Point(3, -3.4));
      data.Add(new Point(4, 6));
      data.Add(new Point(5, 3));
      BarSeries series = new BarSeries();
      chart.Series.Add(series);
      series.ItemsSource = data;

      Assert.AreEqual(2, chart.XAxis.Minimum);
      Assert.AreEqual(5, chart.XAxis.Maximum);
    }

    [Test]
    [Ignore("This no longer works due to performance enhancements, this test needs to be revised")]
    public void XAxisRangeIsSetAutomatically_StartingNegative()
    {
      Chart chart = new Chart();
      IList data = new List<object>();
      data.Add(new Point(-3, 4));
      data.Add(new Point(0, -3.4));
      data.Add(new Point(3, 6));
      data.Add(new Point(6, 3));
      BarSeries series = new BarSeries();
      chart.Series.Add(series);
      series.ItemsSource = data;

      Assert.AreEqual(-3, chart.XAxis.Minimum);
      Assert.AreEqual(6, chart.XAxis.Maximum);
    }

    [Test]
    [Ignore("This no longer works due to performance enhancements, this test needs to be revised")]
    public void XAxisRangeIsSetAutomatically_MultipleSeries()
    {
      Chart chart = new Chart();

      IList data1 = new List<object>();
      data1.Add(new Point(2, 3));
      data1.Add(new Point(3, 3));
      data1.Add(new Point(4, 3));
      data1.Add(new Point(5, 3)); // This will be the maximum X value
      BarSeries series1 = new BarSeries();
      chart.Series.Add(series1);
      series1.ItemsSource = data1;

      IList data2 = new List<object>();
      data2.Add(new Point(1, 3)); // This will be the minimum X value
      data2.Add(new Point(2, 3));
      data2.Add(new Point(3, 3));
      data2.Add(new Point(4, 3));
      BarSeries series2 = new BarSeries();
      chart.Series.Add(series2);
      series2.ItemsSource = data2;

      Assert.AreEqual(1, chart.XAxis.Minimum);
      Assert.AreEqual(5, chart.XAxis.Maximum);
    }

    [Test]
    [Ignore("This no longer works due to performance enhancements, this test needs to be revised")]
    public void YAxisRangeIsSetAutomatically()
    {
      Chart chart = new Chart();
      IList data = new List<object>();
      data.Add(new Point(0, 4));
      data.Add(new Point(1, 2)); // Lowest Y value is 2. The Minimum Y axis value however will automatically be set to zero
      data.Add(new Point(2, 7));
      data.Add(new Point(3, 3.8));
      data.Add(new Point(4, 6));
      BarSeries series = new BarSeries();
      chart.Series.Add(series);
      series.ItemsSource = data;

      Assert.AreEqual(0, chart.YAxis.Minimum); // The minimum is zero, even though the Y data starts at 2
      Assert.AreEqual(8, chart.YAxis.Maximum); // The maximum is the highest Y value from the data plus 1
    }

    [Test]
    [Ignore("This no longer works due to performance enhancements, this test needs to be revised")]
    public void YAxisRangeIsSetAutomatically_StartingNegative()
    {
      Chart chart = new Chart();
      IList data = new List<object>();
      data.Add(new Point(0, 4));
      data.Add(new Point(1, -4.3)); // Smallest Y value. This will be rounded to -5
      data.Add(new Point(2, 7));
      data.Add(new Point(3, 12.8)); // The highest Y value. This will be rounded to 13, and then added with 1 to give 14
      data.Add(new Point(4, 6));
      BarSeries series = new BarSeries();
      chart.Series.Add(series);
      series.ItemsSource = data;

      Assert.AreEqual(-6, chart.YAxis.Minimum); // The minimum is the Math.Floor of the smallest Y value in the data
      Assert.AreEqual(14, chart.YAxis.Maximum); // The maximum is the highest rounded Y value from the data, plus 1
    }

    [Test]
    [Ignore("This no longer works due to performance enhancements, this test needs to be revised")]
    public void YAxisRangeIsSetAutomatically_MultipleSeries()
    {
      Chart chart = new Chart();

      IList data1 = new List<object>();
      data1.Add(new Point(0, 3));
      data1.Add(new Point(1, 3));
      data1.Add(new Point(2, -5)); // This will be the minimum Y value
      data1.Add(new Point(3, 3));
      BarSeries series1 = new BarSeries();
      chart.Series.Add(series1);
      series1.ItemsSource = data1;

      IList data2 = new List<object>();
      data2.Add(new Point(0, 3));
      data2.Add(new Point(1, 3));
      data2.Add(new Point(2, 6)); // This will be the maximum Y value
      data2.Add(new Point(3, 3));
      BarSeries series2 = new BarSeries();
      chart.Series.Add(series2);
      series2.ItemsSource = data2;

      Assert.AreEqual(-6, chart.YAxis.Minimum);
      Assert.AreEqual(7, chart.YAxis.Maximum);
    }

    [Test]
    [STAThread]
    public void Pan()
    {
      ChartAxis axis = new ChartAxis();
      axis.Minimum = 0;
      axis.Maximum = 10;
      axis.ActualMinimum = 1;
      axis.ActualMaximum = 5;
      double actualPan = axis.Pan(3.4);

      Assert.AreEqual(4.4, axis.ActualMinimum);
      Assert.AreEqual(8.4, axis.ActualMaximum);
      Assert.AreEqual(3.4, actualPan);
    }

    [Test]
    [STAThread]
    public void Pan_NegativeDelta()
    {
      ChartAxis axis = new ChartAxis();
      axis.Minimum = 0;
      axis.Maximum = 10;
      axis.ActualMinimum = 5;
      axis.ActualMaximum = 9;
      double actualPan = axis.Pan(-3.4);

      Assert.AreEqual(1.6, axis.ActualMinimum);
      Assert.AreEqual(5.6, axis.ActualMaximum);
      Assert.AreEqual(-3.4, actualPan);
    }

    [Test]
    [STAThread]
    public void PanDoesNotOverflowMaximum()
    {
      ChartAxis axis = new ChartAxis();
      axis.Minimum = 0;
      axis.Maximum = 10;
      axis.ActualMinimum = 1;
      axis.ActualMaximum = 5;
      // If 6 is added to the actual maximum, then the actual maximum will be larger than the true maximum.
      // The pan method should notice this and change the delta value to 5 to prevent this overflow.
      // The returned actual pan will be 5, and the actual min/max values should both have only been incremented by 5.
      double actualPan = axis.Pan(6);

      Assert.AreEqual(6, axis.ActualMinimum);
      Assert.AreEqual(10, axis.ActualMaximum);
      Assert.AreEqual(5, actualPan);
    }

    [Test]
    [STAThread]
    public void PanDoesNotOverflowMinimum()
    {
      ChartAxis axis = new ChartAxis();
      axis.Minimum = 0;
      axis.Maximum = 10;
      axis.ActualMinimum = 1;
      axis.ActualMaximum = 5;
      // If -3 is added to the actual minimum, then the actual minimum will be smaller than the true minimum.
      // The pan method should notice this and change the delta value to -1 to prevent this overflow.
      // The returned actual pan will be -1, and the actual min/max values should both have only been changed by -1.
      double actualPan = axis.Pan(-3);

      Assert.AreEqual(0, axis.ActualMinimum);
      Assert.AreEqual(4, axis.ActualMaximum);
      Assert.AreEqual(-1, actualPan);
    }

    [Test]
    [Ignore("Min/Max constraints are now applied after the axis has loaded to resolve binding issues. Need to revise this")]
    public void MinimumCanNotBeHigherThanMaximum()
    {
      ChartAxis axis = new ChartAxis();
      axis.Minimum = 0;
      axis.Maximum = 10;

      axis.Minimum = 13; // Set the minimum to be higher than the maximum.

      Assert.AreEqual(13, axis.Minimum);
      Assert.AreEqual(13, axis.Maximum); // The maximum should be adjusted to keep the constraint.
    }

    [Test]
    [Ignore("Min/Max constraints are now applied after the axis has loaded to resolve binding issues. Need to revise this")]
    public void MaximumCanNotBeLowerThanMinimum()
    {
      ChartAxis axis = new ChartAxis();
      axis.Minimum = 0;
      axis.Maximum = 10;

      axis.Maximum = -7; // Set the maximum to be lower than the minimum.

      Assert.AreEqual(-7, axis.Minimum); // The minimum should be adjusted to keep the constraint.
      Assert.AreEqual(-7, axis.Maximum);
    }

    [Test]
    [STAThread]
    public void SettingMinimumWillSetActualMinimum()
    {
      ChartAxis axis = new ChartAxis();
      //Assert.AreEqual(axis.Minimum, axis.ActualMinimum);
      axis.Minimum = -4;
      Assert.AreEqual(-4, axis.Minimum);
      Assert.AreEqual(axis.Minimum, axis.ActualMinimum);
    }

    [Test]
    [STAThread]
    public void SettingMaximumWillSetActualMaximum()
    {
      ChartAxis axis = new ChartAxis();
      //Assert.AreEqual(axis.Maximum, axis.ActualMaximum);
      axis.Maximum = 7;
      Assert.AreEqual(7, axis.Maximum);
      Assert.AreEqual(axis.Maximum, axis.ActualMaximum);
    }
    
    [Test]
    [STAThread]
    public void ActualMinimumCanNotBeSetLowerThanMinimum()
    {
      ChartAxis axis = new ChartAxis();
      axis.Minimum = -5;
      axis.Maximum = 5;

      axis.ActualMinimum = -6;

      Assert.AreEqual(-5, axis.ActualMinimum);
      Assert.AreEqual(-5, axis.Minimum);
    }

    [Test]
    [STAThread]
    public void ActualMinimumCanNotBeHigherThanActualMaximum()
    {
      ChartAxis axis = new ChartAxis();
      axis.Minimum = 0;
      axis.Maximum = 10;
      axis.ActualMaximum = 5;

      axis.ActualMinimum = 6; // Set the actual minimum to be higher than the actual maximum

      Assert.AreEqual(6, axis.ActualMinimum);
      Assert.AreEqual(6, axis.ActualMaximum); // The actual maximum should be adjusted to hold the constraint.
    }

    [Test]
    [STAThread]
    public void ActualMinimumCanNotBeSetHigherThanMaximum()
    {
      ChartAxis axis = new ChartAxis();
      axis.Minimum = -5;
      axis.Maximum = 5;

      axis.ActualMinimum = 6; // Set the actual minimum to be higher than the maximum

      Assert.AreEqual(5, axis.ActualMinimum); // The actual minimum should be adjusted to be equal to the maximum
      Assert.AreEqual(5, axis.ActualMaximum);
      Assert.AreEqual(5, axis.Maximum); // just checking that the maximum is not modified.
    }

    [Test]
    [STAThread]
    public void ActualMaximumCanNotBeSetHigherThanMaximum()
    {
      ChartAxis axis = new ChartAxis();
      axis.Minimum = -5;
      axis.Maximum = 5;

      axis.ActualMaximum = 6;

      Assert.AreEqual(5, axis.ActualMaximum);
      Assert.AreEqual(5, axis.Maximum);
    }

    [Test]
    [STAThread]
    public void ActualMaximumCanNotBeLowerThanActualMinimum()
    {
      ChartAxis axis = new ChartAxis();
      axis.Minimum = 0;
      axis.Maximum = 10;
      axis.ActualMinimum = 5;

      axis.ActualMaximum = 3; // Set the actual maximum to be lower than the actual minimum

      Assert.AreEqual(3, axis.ActualMinimum); // The actual minimum should be adjusted to hold the constraint.
      Assert.AreEqual(3, axis.ActualMaximum);
    }

    [Test]
    [STAThread]
    public void ActualMaximumCanNotBeSetLowerThanMinimum()
    {
      ChartAxis axis = new ChartAxis();
      axis.Minimum = -5;
      axis.Maximum = 5;

      axis.ActualMaximum = -6; // Set the actual maximum to be lower than the minimum

      Assert.AreEqual(-5, axis.ActualMinimum);
      Assert.AreEqual(-5, axis.ActualMaximum); // The actual maximum should be adjusted to be equal to the minimum
      Assert.AreEqual(-5, axis.Minimum); // just checking that the minimum is not modified.
    }
  }
}
