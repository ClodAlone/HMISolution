using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using System.Windows.Media;

namespace Mindscape.WpfElements.WpfPropertyGrid.UnitTests
{
  [TestFixture]
  public class ColorToBrushConverterTests
  {
    [Test]
    public void Convert()
    {
      Color color = Color.FromArgb(12, 34, 56, 78);
      SolidColorBrush brush = (SolidColorBrush)(new ColorToBrushConverter().Convert(color, typeof(Brush), null, null));
      Assert.AreEqual(color, brush.Color);
    }

    [Test]
    [ExpectedException(typeof(NotImplementedException))]
    public void ConvertBack()
    {
      SolidColorBrush brush = new SolidColorBrush(Colors.Red);
      new ColorToBrushConverter().ConvertBack(brush, typeof(Color), null, null);
    }
  }
}
