using System;
using NUnit.Framework;
using System.Globalization;

namespace Mindscape.WpfElements.WpfPropertyGrid.UnitTests
{
  [TestFixture]
  public class LocalDateFormatConverterTests
  {
    [Test]
    public void Convert()
    {
      DateTime date = new DateTime(2007, 12, 1);
      LocalDateFormatConverter converter = new LocalDateFormatConverter();

      converter.LongFormat = false;
      string nzshort = (string)converter.Convert(date, typeof(string), null, new CultureInfo("en-NZ"));
      Assert.AreEqual("1/12/2007", nzshort);
      string usshort = (string)converter.Convert(date, typeof(string), null, new CultureInfo("en-US"));
      Assert.AreEqual("12/1/2007", usshort);

      converter.LongFormat = true;
      string nzlong = (string)converter.Convert(date, typeof(string), null, new CultureInfo("en-NZ"));
      Assert.AreEqual("Saturday, 1 December 2007", nzlong);
      string frlong = (string)converter.Convert(date, typeof(string), null, new CultureInfo("fr-FR"));
      Assert.AreEqual("samedi 1 décembre 2007", frlong);
    }

    [Test]
    public void ConvertBack()
    {
      LocalDateFormatConverter converter = new LocalDateFormatConverter();

      DateTime nz = (DateTime)converter.ConvertBack("1/12/2007", typeof(DateTime), null, new CultureInfo("en-NZ"));
      Assert.AreEqual(new DateTime(2007, 12, 1), nz);
      DateTime us = (DateTime)converter.ConvertBack("1/12/2007", typeof(DateTime), null, new CultureInfo("en-US"));
      Assert.AreEqual(new DateTime(2007, 1, 12), us);
      DateTime fr = (DateTime)converter.ConvertBack("samedi 1 décembre 2007", typeof(DateTime), null, new CultureInfo("fr-FR"));
      Assert.AreEqual(new DateTime(2007, 12, 1), fr);
    }
  }
}
