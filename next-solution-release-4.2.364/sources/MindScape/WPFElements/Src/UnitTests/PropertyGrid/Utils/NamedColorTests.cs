using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using System.Windows.Media;

namespace Mindscape.WpfElements.WpfPropertyGrid.UnitTests
{
  [TestFixture]
  public class NamedColorTests
  {
    [Test]
    public void SimpleProperties()
    {
      NamedColor nc = new NamedColor("Incarnadine", Colors.DarkGoldenrod);
      Assert.AreEqual("Incarnadine", nc.Name);
      Assert.AreEqual(Colors.DarkGoldenrod, nc.Color);
    }

    [Test]
    [ExpectedException(typeof(ArgumentNullException))]
    public void Constructor_Name_NotNull()
    {
      new NamedColor(null, Colors.DarkGoldenrod);
    }

    [Test]
    [ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void Constructor_Name_NotEmpty()
    {
      new NamedColor(String.Empty, Colors.DarkGoldenrod);
    }

    [Test]
    public void NamedColors()
    {
      Assert.IsTrue(NamedColor.NamedColors.Count > 0);

      ColorConverter converter = new ColorConverter();

      foreach (NamedColor nc in NamedColor.NamedColors)
      {
        Color color = (Color)(converter.ConvertFromInvariantString(nc.Name));
        Assert.AreEqual(color, nc.Color);
      }
    }

    [Test]
    public void ColorNames()
    {
      Assert.IsTrue(NamedColor.ColorNames.Count > 0);

      ColorConverter converter = new ColorConverter();

      foreach (Color color in NamedColor.ColorNames.Keys)
      {
        string name = NamedColor.ColorNames[color];
        Color colorFromName = (Color)(converter.ConvertFromInvariantString(name));
        Assert.AreEqual(color, colorFromName);
      }
    }
  }
}
