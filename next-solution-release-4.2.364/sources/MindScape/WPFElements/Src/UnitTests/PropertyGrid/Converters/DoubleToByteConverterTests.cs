using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;

namespace Mindscape.WpfElements.WpfPropertyGrid.UnitTests
{
  [TestFixture]
  public class DoubleToByteConverterTests
  {
    [Test]
    public void Convert()
    {
      DoubleToByteConverter dbc = new DoubleToByteConverter();

      double d = 123;
      byte b = (byte)(dbc.Convert(d, typeof(byte), null, null));
      Assert.AreEqual(123, b);

      d = 12.3;
      b = (byte)(dbc.Convert(d, typeof(byte), null, null));
      Assert.AreEqual(12, b);
    }

    [Test]
    [ExpectedException(typeof(OverflowException))]
    public void Convert_Overflow()
    {
      DoubleToByteConverter dbc = new DoubleToByteConverter();

      double d = 256;
      dbc.Convert(d, typeof(byte), null, null);
    }

    [Test]
    public void ConvertBack()
    {
      byte b = 47;
      double d = (double)(new DoubleToByteConverter().ConvertBack(b, typeof(double), null, null));
      Assert.AreEqual(47, d);
    }
  }
}
