using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using System.Windows;
using Mindscape.WpfElements.Charting;
using System.Windows.Data;
using System.Windows.Controls;

namespace Mindscape.WpfElements.UnitTests.Charting
{
  // TODO: test missing data point scenarios.
  // TODO: test an incorrect binding.
  // TODO: test duplicate values in category axis support
  // TODO: test IChartDataExtractor.

  [TestFixture]
  public class DataSeries_GetPointTests
  {
    private SimpleDataSeries _simpleSeries;
    private ChartAxis _xAxisWithConverter;
    private ChartAxis _yAxisWithConverter;
    private ChartAxis _axisWithDateTimeConverter;

    [SetUp]
    public void SetUp()
    {
      _simpleSeries = new SimpleDataSeries();

      _xAxisWithConverter = new ChartAxis();
      _xAxisWithConverter.ValueConverter = new SimpleAxisValueConverter();

      _yAxisWithConverter = new ChartAxis();
      _yAxisWithConverter.ValueConverter = new SimpleAxisValueConverter();

      _axisWithDateTimeConverter = new ChartAxis();
      _axisWithDateTimeConverter.ValueConverter = new DateTimeAxisValueConverter();
    }

    [Test]
    [Ignore("Ambiguous method call")]
    public void NullReturnsNaN()
    {
      Assert.AreEqual(new Point(Double.NaN, Double.NaN), _simpleSeries.GetPoint(null, 0));
    }

    #region Point data tests

    #region Standard tests

    [Test]
    [STAThread]
    public void PointData()
    {
      Assert.AreEqual(new Point(3, 17), _simpleSeries.GetPoint(new Point(3, 17), 0));
    }

    [Test]
    [STAThread]
    public void PointData_XValueConverter()
    {
      _simpleSeries.XAxis = _xAxisWithConverter;
      Assert.AreEqual(new Point(8, 17), _simpleSeries.GetPoint(new Point(3, 17), 0));
    }

    [Test]
    [STAThread]
    public void PointData_YValueConverter()
    {
      _simpleSeries.YAxis = _yAxisWithConverter;
      Assert.AreEqual(new Point(3, 22), _simpleSeries.GetPoint(new Point(3, 17), 0));
    }

    [Test]
    [STAThread]
    public void PointData_XAndYValueConverters()
    {
      _simpleSeries.XAxis = _xAxisWithConverter;
      _simpleSeries.YAxis = _yAxisWithConverter;
      Assert.AreEqual(new Point(8, 22), _simpleSeries.GetPoint(new Point(3, 17), 0));
    }

    [Test]
    [STAThread]
    public void PointData_XAndYBindings()
    {
      _simpleSeries.XBinding = new Binding("Y");
      _simpleSeries.YBinding = new Binding("X");
      Assert.AreEqual(new Point(17, 3), _simpleSeries.GetPoint(new Point(3, 17), 0));
    }

    [Test]
    [STAThread]
    public void PointData_InvalidBinding()
    {
      _simpleSeries.XAxis = new ChartAxis();
      _simpleSeries.YAxis = new ChartAxis();
      _simpleSeries.XBinding = new Binding("TheX"); // TODO: should this default back to Point.X etc, or should it result in NaN as it does at the moment?
      _simpleSeries.YBinding = new Binding("Y");
      Assert.AreEqual(new Point(Double.NaN, 17), _simpleSeries.GetPoint(new Point(3, 17), 0));
    }

    [Test]
    [STAThread]
    public void PointData_XAndYValueConvertersAndBindings()
    {
      _simpleSeries.XBinding = new Binding("Y");
      _simpleSeries.YBinding = new Binding("X");
      _simpleSeries.XAxis = _xAxisWithConverter;
      _simpleSeries.YAxis = _yAxisWithConverter;
      Assert.AreEqual(new Point(22, 8), _simpleSeries.GetPoint(new Point(3, 17), 0));
    }

    #endregion // Standard tests

    #region Reverse axis tests

    // Reverse axis Point data tests produce the same results as standard test. This is because X is X and Y is Y.

    [Test]
    [STAThread]
    public void Horizontal_PointData()
    {
      _simpleSeries.Orientation = Orientation.Horizontal;
      Assert.AreEqual(new Point(3, 17), _simpleSeries.GetPoint(new Point(3, 17), 0));
    }

    [Test]
    [STAThread]
    public void Horizontal_PointData_XValueConverter()
    {
      _simpleSeries.Orientation = Orientation.Horizontal;
      _simpleSeries.XAxis = _xAxisWithConverter;
      Assert.AreEqual(new Point(8, 17), _simpleSeries.GetPoint(new Point(3, 17), 0));
    }

    [Test]
    [STAThread]
    public void Horizontal_PointData_YValueConverter()
    {
      _simpleSeries.Orientation = Orientation.Horizontal;
      _simpleSeries.YAxis = _yAxisWithConverter;
      Assert.AreEqual(new Point(3, 22), _simpleSeries.GetPoint(new Point(3, 17), 0));
    }

    [Test]
    [STAThread]
    public void Horizontal_PointData_XAndYValueConverters()
    {
      _simpleSeries.Orientation = Orientation.Horizontal;
      _simpleSeries.XAxis = _xAxisWithConverter;
      _simpleSeries.YAxis = _yAxisWithConverter;
      Assert.AreEqual(new Point(8, 22), _simpleSeries.GetPoint(new Point(3, 17), 0));
    }

    [Test]
    [STAThread]
    public void Horizontal_PointData_XAndYBindings()
    {
      _simpleSeries.Orientation = Orientation.Horizontal;
      _simpleSeries.XBinding = new Binding("Y");
      _simpleSeries.YBinding = new Binding("X");
      Assert.AreEqual(new Point(17, 3), _simpleSeries.GetPoint(new Point(3, 17), 0));
    }

    [Test]
    [STAThread]
    public void Horizontal_PointData_XAndYValueConvertersAndBindings()
    {
      _simpleSeries.Orientation = Orientation.Horizontal;
      _simpleSeries.XBinding = new Binding("Y");
      _simpleSeries.YBinding = new Binding("X");
      _simpleSeries.XAxis = _xAxisWithConverter;
      _simpleSeries.YAxis = _yAxisWithConverter;
      Assert.AreEqual(new Point(22, 8), _simpleSeries.GetPoint(new Point(3, 17), 0));
    }

    #endregion // Reverse axis tests

    #endregion // Point data tests

    #region StringDouble tests

    #region Standard tests

    [Test]
    [STAThread]
    public void StringDouble()
    {
      // Set the axis so it can keep tracking of category mapping.
      _simpleSeries.XAxis = new ChartAxis();
      // String simply gets mapped along the x axis. First string is at x = 0.
      Assert.AreEqual(new Point(0, 17), _simpleSeries.GetPoint(new Mindscape.WpfElements.Charting.StringDouble("String", 17), 0));
      // Second string is at x = 1.
      Assert.AreEqual(new Point(1, 42), _simpleSeries.GetPoint(new Mindscape.WpfElements.Charting.StringDouble("String 2", 42), 1));
    }

    // No point testing X axis value converter

    [Test]
    [STAThread]
    public void StringDouble_YValueConverter()
    {
      // Set the axis so it can keep track of category mapping.
      _simpleSeries.XAxis = new ChartAxis();
      _simpleSeries.YAxis = _yAxisWithConverter;
      Assert.AreEqual(new Point(0, 22), _simpleSeries.GetPoint(new Mindscape.WpfElements.Charting.StringDouble("String", 17), 0));
      Assert.AreEqual(new Point(1, 47), _simpleSeries.GetPoint(new Mindscape.WpfElements.Charting.StringDouble("String 2", 42), 1));
    }

    [Test]
    [STAThread]
    public void StringDouble_XAndYBindings()
    {
      // Set the axis so it can keep track of category mapping.
      _simpleSeries.YAxis = new ChartAxis();
      _simpleSeries.XBinding = new Binding("Double");
      _simpleSeries.YBinding = new Binding("String");
      // This time strings are mapped along the Y axis because of the bindings
      Assert.AreEqual(new Point(17, 0), _simpleSeries.GetPoint(new Mindscape.WpfElements.Charting.StringDouble("String", 17), 0));
      Assert.AreEqual(new Point(42, 1), _simpleSeries.GetPoint(new Mindscape.WpfElements.Charting.StringDouble("String 2", 42), 1));
    }

    [Test]
    [STAThread]
    public void StringDouble_XAndYBindingsAndXValueConverter()
    {
      // Set the axis so it can keep track of category mapping.
      _simpleSeries.YAxis = new ChartAxis();
      _simpleSeries.XBinding = new Binding("Double");
      _simpleSeries.YBinding = new Binding("String");
      _simpleSeries.XAxis = _xAxisWithConverter;
      Assert.AreEqual(new Point(22, 0), _simpleSeries.GetPoint(new Mindscape.WpfElements.Charting.StringDouble("String", 17), 0));
      Assert.AreEqual(new Point(47, 1), _simpleSeries.GetPoint(new Mindscape.WpfElements.Charting.StringDouble("String 2", 42), 1));
    }

    #endregion // Standard tests

    #region Reverse axis tests

    // This time strings automatically get mapped along the Y axis unless otherwise specified.

    [Test]
    [STAThread]
    public void Horizontal_StringDouble()
    {
      _simpleSeries.Orientation = Orientation.Horizontal;
      // Set the axis so it can keep track of category mapping.
      _simpleSeries.YAxis = new ChartAxis();
      // String simply gets mapped along the y axis. First string is at y = 0.
      Assert.AreEqual(new Point(17, 0), _simpleSeries.GetPoint(new Mindscape.WpfElements.Charting.StringDouble("String", 17), 0));
      // Second string is at y = 1.
      Assert.AreEqual(new Point(42, 1), _simpleSeries.GetPoint(new Mindscape.WpfElements.Charting.StringDouble("String 2", 42), 1));
    }

    [Test]
    [STAThread]
    public void Horizontal_StringDouble_XValueConverter()
    {
      _simpleSeries.Orientation = Orientation.Horizontal;
      // Set the axis so it can keep track of category mapping.
      _simpleSeries.YAxis = new ChartAxis();
      _simpleSeries.XAxis = _xAxisWithConverter;
      Assert.AreEqual(new Point(22, 0), _simpleSeries.GetPoint(new Mindscape.WpfElements.Charting.StringDouble("String", 17), 0));
      Assert.AreEqual(new Point(47, 1), _simpleSeries.GetPoint(new Mindscape.WpfElements.Charting.StringDouble("String 2", 42), 1));
    }

    [Test]
    [STAThread]
    public void Horizontal_StringDouble_XAndYBindings()
    {
      _simpleSeries.Orientation = Orientation.Horizontal;
      // Set the axis so it can keep track of category mapping.
      _simpleSeries.XAxis = new ChartAxis();
      _simpleSeries.XBinding = new Binding("String");
      _simpleSeries.YBinding = new Binding("Double");
      Assert.AreEqual(new Point(0, 17), _simpleSeries.GetPoint(new Mindscape.WpfElements.Charting.StringDouble("String", 17), 0));
      Assert.AreEqual(new Point(1, 42), _simpleSeries.GetPoint(new Mindscape.WpfElements.Charting.StringDouble("String 2", 42), 1));
    }

    [Test]
    [STAThread]
    public void Horizontal_StringDouble_XAndYBindingsAndYValueConverter()
    {
      _simpleSeries.Orientation = Orientation.Horizontal;
      // Set the axis so it can keep track of category mapping.
      _simpleSeries.XAxis = new ChartAxis();
      _simpleSeries.XBinding = new Binding("String");
      _simpleSeries.YBinding = new Binding("Double");
      _simpleSeries.YAxis = _yAxisWithConverter;
      Assert.AreEqual(new Point(0, 22), _simpleSeries.GetPoint(new Mindscape.WpfElements.Charting.StringDouble("String", 17), 0));
      Assert.AreEqual(new Point(1, 47), _simpleSeries.GetPoint(new Mindscape.WpfElements.Charting.StringDouble("String 2", 42), 1));
    }

    #endregion // Reverse axis tests

    #endregion // StringDouble tests

    #region DateTimeDouble tests

    #region Standard tests

    [Test]
    [STAThread]
    public void DateTimeDouble()
    {
      // Set the axis so it can keep track of category mapping.
      _simpleSeries.XAxis = new ChartAxis();
      // DateTime simply gets mapped along the X axis. This is because we haven't specified an axis value converter, so the axis uses category support.
      // First DateTime gets plotted at x = 0.
      Assert.AreEqual(new Point(0, 17), _simpleSeries.GetPoint(new DateTimeDouble(new DateTime(2012, 1, 1), 17), 0));
      // Second DateTime gets plotted at x = 1.
      Assert.AreEqual(new Point(1, 42), _simpleSeries.GetPoint(new DateTimeDouble(new DateTime(2012, 1, 2), 42), 1));
    }

    [Test]
    [STAThread]
    public void DateTimeDouble_XValueConverter()
    {
      _simpleSeries.XAxis = _axisWithDateTimeConverter;
      DateTime dateTime1 = new DateTime(2012, 1, 1);
      Assert.AreEqual(new Point(dateTime1.Ticks, 17), _simpleSeries.GetPoint(new DateTimeDouble(dateTime1, 17), 0));
      DateTime dateTime2 = new DateTime(2012, 1, 2);
      Assert.AreEqual(new Point(dateTime2.Ticks, 42), _simpleSeries.GetPoint(new DateTimeDouble(dateTime2, 42), 1));
    }

    [Test]
    [STAThread]
    public void DateTimeDouble_YValueConverter()
    {
      _simpleSeries.YAxis = _yAxisWithConverter;
      // Set the X axis so it can keep tracking of category mapping.
      _simpleSeries.XAxis = new ChartAxis();
      Assert.AreEqual(new Point(0, 22), _simpleSeries.GetPoint(new DateTimeDouble(new DateTime(2012, 1, 1), 17), 0));
      Assert.AreEqual(new Point(1, 47), _simpleSeries.GetPoint(new DateTimeDouble(new DateTime(2012, 1, 2), 42), 1));
    }

    [Test]
    [STAThread]
    public void DateTimeDouble_XAndYValueConverters()
    {
      _simpleSeries.YAxis = _yAxisWithConverter;
      _simpleSeries.XAxis = _axisWithDateTimeConverter;
      DateTime dateTime1 = new DateTime(2012, 1, 1);
      Assert.AreEqual(new Point(dateTime1.Ticks, 22), _simpleSeries.GetPoint(new DateTimeDouble(dateTime1, 17), 0));
      DateTime dateTime2 = new DateTime(2012, 1, 2);
      Assert.AreEqual(new Point(dateTime2.Ticks, 47), _simpleSeries.GetPoint(new DateTimeDouble(dateTime2, 42), 1));
    }

    [Test]
    [STAThread]
    public void DateTimeDouble_XAndYBindings()
    {
      _simpleSeries.XBinding = new Binding("Double");
      _simpleSeries.YBinding = new Binding("DateTime");
      // Set the Y axis so it can keep track of category mapping.
      _simpleSeries.YAxis = new ChartAxis();
      Assert.AreEqual(new Point(17, 0), _simpleSeries.GetPoint(new DateTimeDouble(new DateTime(2012, 1, 1), 17), 0));
      Assert.AreEqual(new Point(42, 1), _simpleSeries.GetPoint(new DateTimeDouble(new DateTime(2012, 1, 2), 42), 1));
    }

    [Test]
    [STAThread]
    public void DateTimeDouble_XAndYValueConvertersAndBindings()
    {
      _simpleSeries.XBinding = new Binding("Double");
      _simpleSeries.YBinding = new Binding("DateTime");
      _simpleSeries.XAxis = _xAxisWithConverter;
      _simpleSeries.YAxis = _axisWithDateTimeConverter;
      DateTime dateTime1 = new DateTime(2012, 1, 1);
      Assert.AreEqual(new Point(22, dateTime1.Ticks), _simpleSeries.GetPoint(new DateTimeDouble(dateTime1, 17), 0));
      DateTime dateTime2 = new DateTime(2012, 1, 2);
      Assert.AreEqual(new Point(47, dateTime2.Ticks), _simpleSeries.GetPoint(new DateTimeDouble(dateTime2, 42), 1));
    }

    #endregion // Standard tests

    #region Reverse axis tests

    [Test]
    [STAThread]
    public void Horizontal_DateTimeDouble()
    {
      _simpleSeries.Orientation = Orientation.Horizontal;
      // Set the axis so it can keep track of category mapping.
      _simpleSeries.YAxis = new ChartAxis();
      // DateTime simply gets mapped along the Y axis. This is because we haven't specified an axis value converter, so the axis uses category support.
      // First DateTime gets plotted at y = 0.
      Assert.AreEqual(new Point(17, 0), _simpleSeries.GetPoint(new DateTimeDouble(new DateTime(2012, 1, 1), 17), 0));
      // Second DateTime gets plotted at y = 1.
      Assert.AreEqual(new Point(42, 1), _simpleSeries.GetPoint(new DateTimeDouble(new DateTime(2012, 1, 2), 42), 1));
    }

    [Test]
    [STAThread]
    public void Horizontal_DateTimeDouble_XValueConverter()
    {
      _simpleSeries.Orientation = Orientation.Horizontal;
      _simpleSeries.XAxis = _xAxisWithConverter;
      // Set the Y axis so it can keep track of category mapping.
      _simpleSeries.YAxis = new ChartAxis();
      Assert.AreEqual(new Point(22, 0), _simpleSeries.GetPoint(new DateTimeDouble(new DateTime(2012, 1, 1), 17), 0));
      Assert.AreEqual(new Point(47, 1), _simpleSeries.GetPoint(new DateTimeDouble(new DateTime(2012, 1, 2), 42), 1));
    }

    [Test]
    [STAThread]
    public void Horizontal_DateTimeDouble_YValueConverter()
    {
      _simpleSeries.Orientation = Orientation.Horizontal;
      _simpleSeries.YAxis = _axisWithDateTimeConverter;
      DateTime dateTime1 = new DateTime(2012, 1, 1);
      Assert.AreEqual(new Point(17, dateTime1.Ticks), _simpleSeries.GetPoint(new DateTimeDouble(dateTime1, 17), 0));
      DateTime dateTime2 = new DateTime(2012, 1, 2);
      Assert.AreEqual(new Point(42, dateTime2.Ticks), _simpleSeries.GetPoint(new DateTimeDouble(dateTime2, 42), 1));
    }

    [Test]
    [STAThread]
    public void Horizontal_DateTimeDouble_XAndYValueConverters()
    {
      _simpleSeries.Orientation = Orientation.Horizontal;
      _simpleSeries.YAxis = _axisWithDateTimeConverter;
      _simpleSeries.XAxis = _xAxisWithConverter;
      DateTime dateTime1 = new DateTime(2012, 1, 1);
      Assert.AreEqual(new Point(22, dateTime1.Ticks), _simpleSeries.GetPoint(new DateTimeDouble(dateTime1, 17), 0));
      DateTime dateTime2 = new DateTime(2012, 1, 2);
      Assert.AreEqual(new Point(47, dateTime2.Ticks), _simpleSeries.GetPoint(new DateTimeDouble(dateTime2, 42), 1));
    }

    [Test]
    [STAThread]
    public void Horizontal_DateTimeDouble_XAndYBindings()
    {
      _simpleSeries.Orientation = Orientation.Horizontal;
      _simpleSeries.XBinding = new Binding("DateTime");
      _simpleSeries.YBinding = new Binding("Double");
      // Set the X axis so it can keep tracking of category mapping.
      _simpleSeries.XAxis = new ChartAxis();
      Assert.AreEqual(new Point(0, 17), _simpleSeries.GetPoint(new DateTimeDouble(new DateTime(2012, 1, 1), 17), 0));
      Assert.AreEqual(new Point(1, 42), _simpleSeries.GetPoint(new DateTimeDouble(new DateTime(2012, 1, 2), 42), 1));
    }

    [Test]
    [STAThread]
    public void Horizontal_DateTimeDouble_XAndYValueConvertersAndBindings()
    {
      _simpleSeries.Orientation = Orientation.Horizontal;
      _simpleSeries.XBinding = new Binding("DateTime");
      _simpleSeries.YBinding = new Binding("Double");
      _simpleSeries.XAxis = _axisWithDateTimeConverter;
      _simpleSeries.YAxis = _yAxisWithConverter;
      DateTime dateTime1 = new DateTime(2012, 1, 1);
      Assert.AreEqual(new Point(dateTime1.Ticks, 22), _simpleSeries.GetPoint(new DateTimeDouble(dateTime1, 17), 0));
      DateTime dateTime2 = new DateTime(2012, 1, 2);
      Assert.AreEqual(new Point(dateTime2.Ticks, 47), _simpleSeries.GetPoint(new DateTimeDouble(dateTime2, 42), 1));
    }

    #endregion // Reverse axis tests

    #endregion // DateTimeDouble tests

    // TODO
    #region Point3 tests

    #region Standard tests

    #endregion // Standard tests

    #region Reverse axis tests

    #endregion // Reverse axis tests

    #endregion // Point3 tests

    // TODO
    #region StringDoubleDouble tests

    #region Standard tests

    #endregion // Standard tests

    #region Reverse axis tests

    #endregion // Reverse axis tests

    #endregion // StringDoubleDouble tests

    // TODO
    #region StockDataPoint tests

    #region Standard tests

    #endregion // Standard tests

    #region Reverse axis tests

    #endregion // Reverse axis tests

    #endregion // StockDataPoint tests

    // TODO
    #region Custom data tests

    #region Standard tests

    #endregion // Standard tests

    #region Reverse axis tests

    #endregion // Reverse axis tests

    #endregion // Custom data tests

    #region Primitive numerical tests

    #region Standard tests

    [Test]
    [STAThread]
    public void PrimitiveNumericalData()
    {
      // At the moment setting the axes is required for primitive numerical data.
      _simpleSeries.YAxis = new ChartAxis();
      _simpleSeries.XAxis = new ChartAxis();
      Assert.AreEqual(new Point(0, 17), _simpleSeries.GetPoint(17, 0));
      Assert.AreEqual(new Point(1, 42), _simpleSeries.GetPoint(42, 1));
    }

    [Test]
    [STAThread]
    public void PrimitiveNumericalData_YAxisValueConverter()
    {
      _simpleSeries.YAxis = _yAxisWithConverter;
      _simpleSeries.XAxis = new ChartAxis();
      Assert.AreEqual(new Point(0, 22), _simpleSeries.GetPoint(17, 0));
      Assert.AreEqual(new Point(1, 47), _simpleSeries.GetPoint(42, 1));
    }

    #endregion // Standard tests

    #region Reverse axis tests

    [Test]
    [STAThread]
    public void Horizontal_PrimitiveNumericalData()
    {
      _simpleSeries.Orientation = Orientation.Horizontal;
      // At the moment setting the axes is required for primitive numerical data.
      _simpleSeries.YAxis = new ChartAxis();
      _simpleSeries.XAxis = new ChartAxis();
      Assert.AreEqual(new Point(17, 0), _simpleSeries.GetPoint(17, 0));
      Assert.AreEqual(new Point(42, 1), _simpleSeries.GetPoint(42, 1));
    }

    [Test]
    [STAThread]
    public void Horizontal_PrimitiveNumericalData_YAxisValueConverter()
    {
      _simpleSeries.Orientation = Orientation.Horizontal;
      _simpleSeries.YAxis = new ChartAxis();
      _simpleSeries.XAxis = _xAxisWithConverter;
      Assert.AreEqual(new Point(22, 0), _simpleSeries.GetPoint(17, 0));
      Assert.AreEqual(new Point(47, 1), _simpleSeries.GetPoint(42, 1));
    }

    #endregion // Reverse axis tests

    #endregion // Primitive numerical tests

    #region String tests

    #region Standard tests

    [Test]
    [STAThread]
    public void StringData()
    {
      _simpleSeries.YAxis = new ChartAxis();
      _simpleSeries.XAxis = new ChartAxis();
      Assert.AreEqual(new Point(0, 0), _simpleSeries.GetPoint("String 1", 0));
      Assert.AreEqual(new Point(1, 1), _simpleSeries.GetPoint("String 2", 1));
    }

    // TODO: make a string axis value converter to test this
    /*[Test]
    public void StringData_YAxisValueConverter()
    {
      _simpleSeries.YAxis = ;
      _simpleSeries.XAxis = new ChartAxis();
      Assert.AreEqual(new Point(0, ), _simpleSeries.GetPoint("String 1", 0));
      Assert.AreEqual(new Point(1, ), _simpleSeries.GetPoint("String 2", 1));
    }*/

    #endregion // Standard tests

    #region Reverse axis tests

    [Test]
    [STAThread]
    public void Horizontal_StringData()
    {
      _simpleSeries.YAxis = new ChartAxis();
      _simpleSeries.XAxis = new ChartAxis();
      Assert.AreEqual(new Point(0, 0), _simpleSeries.GetPoint("String 1", 0));
      Assert.AreEqual(new Point(1, 1), _simpleSeries.GetPoint("String 2", 1));
    }

    // TODO: make a string axis value converter to test this
    /*[Test]
    public void Horizontal_StringData_YAxisValueConverter()
    {
      _simpleSeries.Orientation = Orientation.Horizontal;
      _simpleSeries.YAxis = new ChartAxis();
      _simpleSeries.XAxis = ;
      Assert.AreEqual(new Point( ,0), _simpleSeries.GetPoint("String 1", 0));
      Assert.AreEqual(new Point( ,1), _simpleSeries.GetPoint("String 2", 1));
    }*/

    #endregion // Reverse axis tests

    #endregion // String tests
  }
}
