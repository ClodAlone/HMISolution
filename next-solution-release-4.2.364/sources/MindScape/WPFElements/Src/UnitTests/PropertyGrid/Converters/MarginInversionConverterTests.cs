using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using System.Windows;
using System.Globalization;

namespace Mindscape.WpfElements.WpfPropertyGrid.UnitTests.Converters
{
  [TestFixture]
  public class MarginInversionConverterTests
  {
    [Test]
    public void InvertMargin()
    {
      Thickness t1 = new Thickness(1, 2, 3, 4);
      Thickness t2 = (Thickness)(new MarginInversionConverter().Convert(t1, typeof(Thickness), null, CultureInfo.CurrentCulture));

      Assert.AreEqual(new Thickness(-1, 2, -3, 4), t2);
    }

    [Test]
    [ExpectedException(typeof(NotImplementedException))]
    public void ConvertBack()
    {
      (new MarginInversionConverter()).ConvertBack(new Thickness(1, 2, 3, 4), typeof(Thickness), null, CultureInfo.CurrentCulture);
    }
  }
}
