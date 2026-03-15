using System;
using NUnit.Framework;
using System.Globalization;

namespace Mindscape.WpfElements.WpfPropertyGrid.UnitTests
{
  [TestFixture]
  public class MonthDisplayNameConverterTests
  {
    [Test]
    public void Convert()
    {
      DateTime date = new DateTime(2007, 12, 1);
      MonthDisplayNameConverter converter = new MonthDisplayNameConverter();

      string nz = (string)converter.Convert(date, typeof(string), null, new CultureInfo("en-NZ"));
      Assert.AreEqual("December 2007", nz);
      string fr = (string)converter.Convert(date, typeof(string), null, new CultureInfo("fr-FR"));
      Assert.AreEqual("décembre 2007", fr);
    }

    [Test]
    [ExpectedException(typeof(NotImplementedException))]
    public void ConvertBack()
    {
      MonthDisplayNameConverter converter = new MonthDisplayNameConverter();
      converter.ConvertBack(null, null, null, null);
    }
  }
}
