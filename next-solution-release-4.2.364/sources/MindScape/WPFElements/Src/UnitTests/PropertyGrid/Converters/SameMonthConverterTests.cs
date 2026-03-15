using System;
using NUnit.Framework;

namespace Mindscape.WpfElements.WpfPropertyGrid.UnitTests
{
  [TestFixture]
  public class SameMonthConverterTests
  {
    [Test]
    public void Convert()
    {
      SameMonthConverter converter = new SameMonthConverter();

      object[] datesInSameMonth = new object[] { new DateTime(2007, 10, 1), new DateTime(2007, 10, 20) };
      object[] datesInDifferentMonth = new object[] { new DateTime(2007, 10, 1), new DateTime(2007, 11, 1) };
      object[] datesInSameMonthButDifferentYear = new object[] { new DateTime(2006, 10, 1), new DateTime(2007, 10, 20) };

      bool datesInSameMonthResult = (bool)converter.Convert(datesInSameMonth, typeof(bool), null, null);
      Assert.IsTrue(datesInSameMonthResult);

      bool datesInDifferentMonthResult = (bool)converter.Convert(datesInDifferentMonth, typeof(bool), null, null);
      Assert.IsFalse(datesInDifferentMonthResult);

      bool datesInSameMonthButDifferentYearResult = (bool)converter.Convert(datesInSameMonthButDifferentYear, typeof(bool), null, null);
      Assert.IsFalse(datesInSameMonthButDifferentYearResult);
    }

    [Test]
    [ExpectedException(typeof(NotImplementedException))]
    public void ConvertBack()
    {
      SameMonthConverter converter = new SameMonthConverter();
      converter.ConvertBack(null, null, null, null);
    }
  }
}
