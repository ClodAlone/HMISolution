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
using NUnit.Framework;

namespace Mindscape.WpfElements.UnitTests
{
  [TestFixture]
  public class MonthAxisValueConverterTests
  {
    private MonthAxisValueConverter _converter = new MonthAxisValueConverter();

    [Test]
    public void TheBeginningOfTimeConvertsToZero()
    {
      double value = _converter.GetAxisPlotPosition(new DateTime());
      Assert.AreEqual(0, value);
    }

    [Test]
    public void ZeroConvertsToTheBeginningOfTime()
    {
      DateTime dt = (DateTime)_converter.GetDataObjectAt(0);
      Assert.AreEqual(new DateTime(), dt);
    }

    [Test]
    public void ConvertingBothWaysIsTheSame()
    {
      DateTime t = DateTime.Now;
      double value = _converter.GetAxisPlotPosition(t);
      DateTime dateTime = (DateTime)_converter.GetDataObjectAt(value);
      // The month converter only deals with the Month and Year component of a DateTime.
      // Other components will usually not match. This is correct behavior.
      Assert.AreEqual(t.Month, dateTime.Month);
      Assert.AreEqual(t.Year, dateTime.Year);
    }

    [Test]
    public void NonDateTimeReturnsZero()
    {
      double value = _converter.GetAxisPlotPosition(new Point());
      Assert.AreEqual(0, value);
      value = _converter.GetAxisPlotPosition("string");
      Assert.AreEqual(0, value);
    }
  }
}
