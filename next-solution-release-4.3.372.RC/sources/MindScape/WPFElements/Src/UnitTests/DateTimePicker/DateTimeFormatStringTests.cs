using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using System.Globalization;

namespace Mindscape.WpfElements.UnitTests
{
  [TestFixture]
  public class DateTimeFormatStringTests
  {
    [Test]
    public void IsSelectString()
    {
      Assert.IsFalse(DateTimeFormatString.IsSelect("d"));
      Assert.IsFalse(DateTimeFormatString.IsSelect("dd"));
      Assert.IsFalse(DateTimeFormatString.IsSelect("ddd"));
      Assert.IsFalse(DateTimeFormatString.IsSelect("dddd"));

      Assert.IsFalse(DateTimeFormatString.IsSelect("M"));
      Assert.IsFalse(DateTimeFormatString.IsSelect("MM"));
      Assert.IsTrue(DateTimeFormatString.IsSelect("MMM"));
      Assert.IsTrue(DateTimeFormatString.IsSelect("MMMM"));
      Assert.IsTrue(DateTimeFormatString.IsSelect("MMMMM"));

      Assert.IsFalse(DateTimeFormatString.IsSelect("m"));
      Assert.IsFalse(DateTimeFormatString.IsSelect("mm"));
      Assert.IsFalse(DateTimeFormatString.IsSelect("mmm"));
      Assert.IsFalse(DateTimeFormatString.IsSelect("mmm"));

      Assert.IsTrue(DateTimeFormatString.IsSelect("t"));
      Assert.IsTrue(DateTimeFormatString.IsSelect("tt"));
      Assert.IsTrue(DateTimeFormatString.IsSelect("ttt"));
    }

    // At present we always use the default calendar for a culture, which
    // for all Windows cultures is Gregorian (hence 12 months and 1 era).
    // These tests will need to be extended if we support optional calendars.
    // (Note that DateTimeFormatInfo.Calendar is settable.)

    [Test]
    public void PermittedValues_ShortMonth()
    {
      List<string> values;
      DateTime referenceDate = new DateTime(2008, 1, 1);
      
      values = new List<string>(DateTimeFormatString.GetPermittedValues("MMM",
        TestCultures.EnglishNZ, referenceDate));

      Assert.AreEqual(12, values.Count);
      Assert.AreEqual("Jan", values[0]);
      Assert.AreEqual("Dec", values[11]);

      values = new List<string>(DateTimeFormatString.GetPermittedValues("MMM",
        TestCultures.QuechuaEcuador, referenceDate));

      Assert.AreEqual(12, values.Count);
      Assert.AreEqual("Qul", values[0]);
      Assert.AreEqual("Kap", values[11]);
    }

    [Test]
    public void PermittedValues_LongMonth()
    {
      List<string> values;
      DateTime referenceDate = new DateTime(2008, 1, 1);

      values = new List<string>(DateTimeFormatString.GetPermittedValues("MMMM",
        TestCultures.EnglishNZ, referenceDate));

      Assert.AreEqual(12, values.Count);
      Assert.AreEqual("January", values[0]);
      Assert.AreEqual("December", values[11]);

      values = new List<string>(DateTimeFormatString.GetPermittedValues("MMMMMM",
        TestCultures.FrenchFrance, referenceDate));

      Assert.AreEqual(12, values.Count);
      Assert.AreEqual("janvier", values[0]);
      Assert.AreEqual("décembre", values[11]);
    }

    [Test]
    public void PermittedValues_ShortDayOfWeek()
    {
      List<string> values;
      DateTime referenceDate = new DateTime(2008, 1, 1);

      values = new List<string>(DateTimeFormatString.GetPermittedValues("ddd",
        TestCultures.EnglishNZ, referenceDate));

      Assert.AreEqual(7, values.Count);
      Assert.AreEqual("Sun", values[0]);
      Assert.AreEqual("Sat", values[6]);

      values = new List<string>(DateTimeFormatString.GetPermittedValues("ddd",
        TestCultures.FrenchFrance, referenceDate));

      Assert.AreEqual(7, values.Count);
      Assert.AreEqual("dim.", values[0]);
      Assert.AreEqual("sam.", values[6]);
    }

    [Test]
    public void PermittedValues_LongDayOfWeek()
    {
      List<string> values;
      DateTime referenceDate = new DateTime(2008, 1, 1);

      values = new List<string>(DateTimeFormatString.GetPermittedValues("dddd",
        TestCultures.EnglishNZ, referenceDate));

      Assert.AreEqual(7, values.Count);
      Assert.AreEqual("Sunday", values[0]);
      Assert.AreEqual("Saturday", values[6]);

      values = new List<string>(DateTimeFormatString.GetPermittedValues("ddddd",
        TestCultures.FrenchFrance, referenceDate));

      Assert.AreEqual(7, values.Count);
      Assert.AreEqual("dimanche", values[0]);
      Assert.AreEqual("samedi", values[6]);
    }

    [Test]
    public void PermittedValues_ShortDesignator()
    {
      List<string> values;
      DateTime referenceDate = new DateTime(2008, 1, 1);

      values = new List<string>(DateTimeFormatString.GetPermittedValues("t",
        TestCultures.EnglishNZ, referenceDate));

      Assert.AreEqual(2, values.Count);
      Assert.AreEqual("a", values[0]);
      Assert.AreEqual("p", values[1]);

      values = new List<string>(DateTimeFormatString.GetPermittedValues("t",
        TestCultures.EstonianEstonia, referenceDate));

      Assert.AreEqual(2, values.Count);
      Assert.AreEqual("E", values[0]);
      Assert.AreEqual("P", values[1]);
    }

    [Test]
    public void PermittedValues_LongDesignator()
    {
      List<string> values;
      DateTime referenceDate = new DateTime(2008, 1, 1);

      values = new List<string>(DateTimeFormatString.GetPermittedValues("tt",
        TestCultures.EnglishNZ, referenceDate));

      Assert.AreEqual(2, values.Count);
      Assert.AreEqual("a.m.", values[0]);
      Assert.AreEqual("p.m.", values[1]);

      values = new List<string>(DateTimeFormatString.GetPermittedValues("ttttt",
        TestCultures.EstonianEstonia, referenceDate));

      Assert.AreEqual(2, values.Count);
      Assert.AreEqual("EL", values[0]);
      Assert.AreEqual("PL", values[1]);
    }

    [Test]
    public void Ranges_Day()
    {
      DateTime referenceDate;
      CultureInfo culture = TestCultures.EnglishNZ;
      Range<int> range;

      referenceDate = new DateTime(2008, 1, 1);
      range = DateTimeFormatString.GetValueRange("d", culture, referenceDate);
      Assert.AreEqual(1, range.Minimum);
      Assert.AreEqual(31, range.Maximum);

      referenceDate = new DateTime(2008, 2, 1);
      range = DateTimeFormatString.GetValueRange("dd", culture, referenceDate);
      Assert.AreEqual(1, range.Minimum);
      Assert.AreEqual(29, range.Maximum);

      referenceDate = new DateTime(2100, 2, 1);
      range = DateTimeFormatString.GetValueRange("d", culture, referenceDate);
      Assert.AreEqual(1, range.Minimum);
      Assert.AreEqual(28, range.Maximum);
    }

    [Test]
    public void Ranges_Month()
    {
      DateTime referenceDate;
      CultureInfo culture = TestCultures.EnglishNZ;
      Range<int> range;

      referenceDate = new DateTime(2008, 1, 1);
      range = DateTimeFormatString.GetValueRange("M", culture, referenceDate);
      Assert.AreEqual(1, range.Minimum);
      Assert.AreEqual(12, range.Maximum);

      referenceDate = new DateTime(2008, 2, 1);
      range = DateTimeFormatString.GetValueRange("MM", culture, referenceDate);
      Assert.AreEqual(1, range.Minimum);
      Assert.AreEqual(12, range.Maximum);
    }

    [Test]
    public void Ranges_Year()
    {
      DateTime referenceDate;
      CultureInfo culture = TestCultures.EnglishNZ;
      Range<int> range;

      // TODO: More smarts around 2-digit years?

      referenceDate = new DateTime(2008, 1, 1);
      range = DateTimeFormatString.GetValueRange("yyyy", culture, referenceDate);
      Assert.AreEqual(DateTime.MinValue.Year, range.Minimum);
      Assert.AreEqual(DateTime.MaxValue.Year, range.Maximum);
    }

    [Test]
    public void Ranges_Hour()
    {
      DateTime referenceDate;
      CultureInfo culture = TestCultures.EnglishNZ;
      Range<int> range;

      referenceDate = new DateTime(2008, 1, 1);
      range = DateTimeFormatString.GetValueRange("h", culture, referenceDate);
      Assert.AreEqual(0, range.Minimum);
      Assert.AreEqual(12, range.Maximum);

      referenceDate = new DateTime(2008, 2, 1);
      range = DateTimeFormatString.GetValueRange("hhh", culture, referenceDate);
      Assert.AreEqual(0, range.Minimum);
      Assert.AreEqual(12, range.Maximum);

      range = DateTimeFormatString.GetValueRange("H", culture, referenceDate);
      Assert.AreEqual(0, range.Minimum);
      Assert.AreEqual(23, range.Maximum);

      range = DateTimeFormatString.GetValueRange("HHH", culture, referenceDate);
      Assert.AreEqual(0, range.Minimum);
      Assert.AreEqual(23, range.Maximum);
    }

    [Test]
    public void Ranges_Minute()
    {
      DateTime referenceDate;
      CultureInfo culture = TestCultures.EnglishNZ;
      Range<int> range;

      referenceDate = new DateTime(2008, 1, 1);
      range = DateTimeFormatString.GetValueRange("m", culture, referenceDate);
      Assert.AreEqual(0, range.Minimum);
      Assert.AreEqual(59, range.Maximum);

      range = DateTimeFormatString.GetValueRange("mmm", culture, referenceDate);
      Assert.AreEqual(0, range.Minimum);
      Assert.AreEqual(59, range.Maximum);
    }

    [Test]
    public void Ranges_Second()
    {
      DateTime referenceDate;
      CultureInfo culture = TestCultures.EnglishNZ;
      Range<int> range;

      referenceDate = new DateTime(2008, 1, 1);
      range = DateTimeFormatString.GetValueRange("s", culture, referenceDate);
      Assert.AreEqual(0, range.Minimum);
      Assert.AreEqual(59, range.Maximum);

      range = DateTimeFormatString.GetValueRange("sss", culture, referenceDate);
      Assert.AreEqual(0, range.Minimum);
      Assert.AreEqual(59, range.Maximum);
    }
  }
}
