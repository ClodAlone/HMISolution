using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using System.Windows.Media;

namespace Mindscape.WpfElements.WpfPropertyGrid.UnitTests
{
  [TestFixture]
  public class ColorToNameConverterTests
  {
    [Test]
    public void Convert()
    {
      Color named = Colors.Red;
      Color nameless = Color.FromArgb(0x12, 0x34, 0x56, 0x78);
      Color namedViaValues = Color.FromArgb(0xFF, 0xFF, 0x00, 0x00);

      ColorToNameConverter converter = new ColorToNameConverter();
      string result;

      result = (string)(converter.Convert(named, typeof(string), null, null));
      Assert.AreEqual("Red", result);

      result = (string)(converter.Convert(nameless, typeof(string), null, null));
      Assert.AreEqual("#12345678", result);

      result = (string)(converter.Convert(namedViaValues, typeof(string), null, null));
      Assert.AreEqual("Red", result);
    }

    [Test]
    [ExpectedException(typeof(NotImplementedException))]
    public void ConvertBack()
    {
      (new ColorToNameConverter()).ConvertBack("Red", typeof(Color), null, null);
    }
  }
}
