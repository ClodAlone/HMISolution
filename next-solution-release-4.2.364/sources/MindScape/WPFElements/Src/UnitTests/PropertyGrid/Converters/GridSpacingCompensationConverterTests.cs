using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using System.Windows;

namespace Mindscape.WpfElements.WpfPropertyGrid.UnitTests
{
  [TestFixture]
  public class GridSpacingCompensationConverterTests
  {
    [Test]
    public void Convert_Padding()
    {
      GridSpacingCompensationConverter converter = new GridSpacingCompensationConverter();

      converter.Usage = ThicknessUsage.Padding;

      Thickness result = (Thickness)(converter.Convert(new Thickness(-4, 4, -4, 4), typeof(Thickness), null, null));
      Assert.AreEqual(new Thickness(8, 4, 6, 4), result);
    }

    [Test]
    public void Convert_Margin()
    {
      GridSpacingCompensationConverter converter = new GridSpacingCompensationConverter();

      converter.Usage = ThicknessUsage.Margin;

      Thickness result = (Thickness)(converter.Convert(new Thickness(-4, 4, -4, 4), typeof(Thickness), null, null));
      Assert.AreEqual(new Thickness(-8, -4, -6, -4), result);
    }

    [Test]
    [ExpectedException(typeof(NotImplementedException))]
    public void ConvertBack()
    {
      (new GridSpacingCompensationConverter()).ConvertBack(new Thickness(123), typeof(Thickness), null, null);
    }
  }
}
