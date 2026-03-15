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
  public class DateTimeAxisValueConverterTests
  {
    private static DateTimeAxisValueConverter _converter = new DateTimeAxisValueConverter();

    [Test]
    public void TheBeginningOfTimeConvertsToZero()
    {
      double value = _converter.GetAxisPlotPosition(new DateTime());
      Assert.AreEqual(0, value);
    }

    [Test]
    public void ZeroConvertsToTheBeginningOfTime()
    {
      DateTime value = (DateTime)_converter.GetDataObjectAt(0);
      Assert.AreEqual(new DateTime(), value);
    }

    [Test]
    public void ConvertingBothWaysIsTheSame()
    {
      DateTime t = DateTime.Now;
      // Milliseconds do not always round correctly. They can be off by 0.00001.
      // So here we remove the milliseconds from the DateTime.
      t = new DateTime(t.Year, t.Month, t.Day, t.Hour, t.Minute, t.Second);
      double value = _converter.GetAxisPlotPosition(t);
      DateTime dateTime = (DateTime)_converter.GetDataObjectAt(value);
      Assert.AreEqual(t, dateTime);
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
