using System;
using NUnit.Framework;

namespace Mindscape.WpfElements.WpfPropertyGrid.UnitTests
{
  [TestFixture]
  public class ScalingConverterTests
  {
    [Test]
    public void Convert()
    {
      ScalingConverter converter = new ScalingConverter();

      double result = (double)(converter.Convert(123.45d, typeof(double), null, null));
      Assert.AreEqual(123.45d, result);

      converter.ScaleFactor = 2;
      result = (double)(converter.Convert(123.45d, typeof(double), null, null));
      Assert.AreEqual(246.90d, result);

      converter.ForceToInteger = true;
      result = (double)(converter.Convert(123.45d, typeof(double), null, null));
      Assert.AreEqual(247d, result);
    }

    [Test]
    [ExpectedException(typeof(NotImplementedException))]
    public void ConvertBack()
    {
      ScalingConverter converter = new ScalingConverter();
      converter.ConvertBack(null, null, null, null);
    }
  }
}
