using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using System.Windows.Data;

namespace Mindscape.WpfElements.UnitTests
{
  [TestFixture]
  public class SignClassifierTests
  {
    [Test]
    public void Classify()
    {
      IValueConverter converter = new SignConverter();
      Assert.AreEqual(Sign.Negative, converter.Convert(Decimal.MinValue, null, null, null));
      Assert.AreEqual(Sign.Negative, converter.Convert(Decimal.MinusOne, null, null, null));
      Assert.AreEqual(Sign.Zero, converter.Convert(Decimal.Zero, null, null, null));
      Assert.AreEqual(Sign.Positive, converter.Convert(Decimal.One, null, null, null));
      Assert.AreEqual(Sign.Positive, converter.Convert(Decimal.MaxValue, null, null, null));
    }

    [Test]
    [ExpectedException(typeof(NotImplementedException))]
    public void ConvertBack()
    {
      IValueConverter converter = new SignConverter();
      converter.ConvertBack(Sign.Positive, null, null, null);
    }
  }
}
