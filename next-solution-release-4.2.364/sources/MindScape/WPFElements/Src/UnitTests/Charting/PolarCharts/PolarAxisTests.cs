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
using NUnit.Framework;

namespace Mindscape.WpfElements.UnitTests
{
  [TestFixture]
  public class PolarAxisTests
  {
    [Test]
    [STAThread]
    public void ThetaAxisRangeIsSetAutomatically()
    {
      PolarChart chart = new PolarChart();
      IList data = new List<object>();
      data.Add(new PolarPoint(0, 3));
      data.Add(new PolarPoint(1, 7));
      data.Add(new PolarPoint(2, 4));
      data.Add(new PolarPoint(3, -3.4));
      data.Add(new PolarPoint(4, 6));
      SimplePolarDataSeries series = new SimplePolarDataSeries();
      chart.Series.Add(series);
      series.ItemsSource = data;

      Assert.AreEqual(0, chart.ThetaAxis.Minimum);
      Assert.AreEqual(5, chart.ThetaAxis.Maximum);
    }

    [Test]
    [STAThread]
    public void ThetaAxisRangeIsSetAutomatically_StartingAboveZero()
    {
      PolarChart chart = new PolarChart();
      IList data = new List<object>();
      data.Add(new PolarPoint(2, 4));
      data.Add(new PolarPoint(3, -3.4));
      data.Add(new PolarPoint(4, 6));
      data.Add(new PolarPoint(5, 3));
      SimplePolarDataSeries series = new SimplePolarDataSeries();
      chart.Series.Add(series);
      series.ItemsSource = data;

      Assert.AreEqual(2, chart.ThetaAxis.Minimum);
      Assert.AreEqual(6, chart.ThetaAxis.Maximum);
    }

    [Test]
    [STAThread]
    public void ThetaAxisRangeIsSetAutomatically_StartingNegative()
    {
      PolarChart chart = new PolarChart();
      IList data = new List<object>();
      data.Add(new PolarPoint(-3, 4));
      data.Add(new PolarPoint(0, -3.4));
      data.Add(new PolarPoint(3, 6));
      data.Add(new PolarPoint(6, 3));
      SimplePolarDataSeries series = new SimplePolarDataSeries();
      chart.Series.Add(series);
      series.ItemsSource = data;

      Assert.AreEqual(-3, chart.ThetaAxis.Minimum);
      Assert.AreEqual(9, chart.ThetaAxis.Maximum);
    }

    [Test]
    [STAThread]
    public void ThetaAxisRangeIsSetAutomatically_MultipleSeries()
    {
      PolarChart chart = new PolarChart();

      IList data1 = new List<object>();
      data1.Add(new PolarPoint(2, 3));
      data1.Add(new PolarPoint(3, 3));
      data1.Add(new PolarPoint(4, 3));
      data1.Add(new PolarPoint(5, 3)); // This will be the maximum theta value
      SimplePolarDataSeries series1 = new SimplePolarDataSeries();
      chart.Series.Add(series1);
      series1.ItemsSource = data1;

      IList data2 = new List<object>();
      data2.Add(new PolarPoint(1, 3)); // This will be the minimum theta value
      data2.Add(new PolarPoint(2, 3));
      data2.Add(new PolarPoint(3, 3));
      data2.Add(new PolarPoint(4, 3));
      SimplePolarDataSeries series2 = new SimplePolarDataSeries();
      chart.Series.Add(series2);
      series2.ItemsSource = data2;

      Assert.AreEqual(1, chart.ThetaAxis.Minimum);
      Assert.AreEqual(6, chart.ThetaAxis.Maximum);
    }

    [Test]
    [STAThread]
    public void RhoAxisRangeIsSetAutomatically()
    {
      PolarChart chart = new PolarChart();
      IList data = new List<object>();
      data.Add(new PolarPoint(0, 4));
      data.Add(new PolarPoint(1, 2)); // Lowest Rho value is 2. The Minimum rho axis value however will automatically be set to zero
      data.Add(new PolarPoint(2, 7));
      data.Add(new PolarPoint(3, 3.8));
      data.Add(new PolarPoint(4, 6));
      SimplePolarDataSeries series = new SimplePolarDataSeries();
      chart.Series.Add(series);
      series.ItemsSource = data;

      Assert.AreEqual(0, chart.RhoAxis.Minimum); // The minimum is zero, even though the rho data starts at 2
      Assert.AreEqual(8, chart.RhoAxis.Maximum); // The maximum is the highest rho value from the data plus 1
    }

    [Test]
    [STAThread]
    public void RhoAxisRangeIsSetAutomatically_StartingNegative()
    {
      PolarChart chart = new PolarChart();
      IList data = new List<object>();
      data.Add(new PolarPoint(0, 4));
      data.Add(new PolarPoint(1, -4.3)); // Smallest rho value. This will be rounded to -5
      data.Add(new PolarPoint(2, 7));
      data.Add(new PolarPoint(3, 12.8)); // The highest rho value. This will be rounded to 13, and then added with 1 to give 14
      data.Add(new PolarPoint(4, 6));
      SimplePolarDataSeries series = new SimplePolarDataSeries();
      chart.Series.Add(series);
      series.ItemsSource = data;

      Assert.AreEqual(-6, chart.RhoAxis.Minimum); // The minimum is the Math.Floor of the smallest rho value in the data
      Assert.AreEqual(14, chart.RhoAxis.Maximum); // The maximum is the highest rounded rho value from the data, plus 1
    }

    [Test]
    [STAThread]
    public void RhoAxisRangeIsSetAutomatically_MultipleSeries()
    {
      PolarChart chart = new PolarChart();

      IList data1 = new List<object>();
      data1.Add(new PolarPoint(0, 3));
      data1.Add(new PolarPoint(1, 3));
      data1.Add(new PolarPoint(2, -5)); // This will be the minimum rho value
      data1.Add(new PolarPoint(3, 3));
      SimplePolarDataSeries series1 = new SimplePolarDataSeries();
      chart.Series.Add(series1);
      series1.ItemsSource = data1;

      IList data2 = new List<object>();
      data2.Add(new PolarPoint(0, 3));
      data2.Add(new PolarPoint(1, 3));
      data2.Add(new PolarPoint(2, 6)); // This will be the maximum rho value
      data2.Add(new PolarPoint(3, 3));
      SimplePolarDataSeries series2 = new SimplePolarDataSeries();
      chart.Series.Add(series2);
      series2.ItemsSource = data2;

      Assert.AreEqual(-6, chart.RhoAxis.Minimum);
      Assert.AreEqual(7, chart.RhoAxis.Maximum);
    }

    [Test]
    [STAThread]
    public void ThetaMinimumCanNotBeHigherThanMaximum()
    {
      ThetaAxis axis = new ThetaAxis();
      axis.Minimum = 0;
      axis.Maximum = 10;

      axis.Minimum = 13; // Set the minimum to be higher than the maximum.

      Assert.AreEqual(13, axis.Minimum);
      Assert.AreEqual(13, axis.Maximum); // The maximum should be adjusted to keep the constraint.
    }

    [Test]
    [STAThread]
    public void ThetaMaximumCanNotBeLowerThanMinimum()
    {
      ThetaAxis axis = new ThetaAxis();
      axis.Minimum = 0;
      axis.Maximum = 10;

      axis.Maximum = -7; // Set the maximum to be lower than the minimum.

      Assert.AreEqual(-7, axis.Minimum); // The minimum should be adjusted to keep the constraint.
      Assert.AreEqual(-7, axis.Maximum);
    }

    [Test]
    [STAThread]
    public void RhoMinimumCanNotBeHigherThanMaximum()
    {
      RhoAxis axis = new RhoAxis();
      axis.Minimum = 0;
      axis.Maximum = 10;

      axis.Minimum = 13; // Set the minimum to be higher than the maximum.

      Assert.AreEqual(13, axis.Minimum);
      Assert.AreEqual(13, axis.Maximum); // The maximum should be adjusted to keep the constraint.
    }

    [Test]
    [STAThread]
    public void RhoMaximumCanNotBeLowerThanMinimum()
    {
      RhoAxis axis = new RhoAxis();
      axis.Minimum = 0;
      axis.Maximum = 10;

      axis.Maximum = -7; // Set the maximum to be lower than the minimum.

      Assert.AreEqual(-7, axis.Minimum); // The minimum should be adjusted to keep the constraint.
      Assert.AreEqual(-7, axis.Maximum);
    }
  }
}
