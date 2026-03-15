using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;

namespace Mindscape.WpfElements.UnitTests
{
  [TestFixture]
  public class CurrencyTextBoxControlTests
  {
    [Test]
    [STAThread]
    public void TextAndValueAreSynchronised()
    {
      CurrencyTextBox control = new CurrencyTextBox();
      control.Culture = TestCultures.EnglishNZ;

      control.Value = 123;
      Assert.AreEqual("$123.00", control.Text);

      control.Text = "-123";
      Assert.AreEqual(-123m, control.Value);
    }

    [Test]
    [STAThread]
    public void InvalidTextDoesNotAffectValue()
    {
      CurrencyTextBox control = new CurrencyTextBox();
      control.Culture = TestCultures.EnglishNZ;

      control.Value = 123;
      Assert.AreEqual("$123.00", control.Text);
      Assert.AreEqual(123m, control.Value);

      control.Text = "1X2X3";
      Assert.AreEqual("1X2X3", control.Text);
      Assert.AreEqual(123m, control.Value);
    }

    [Test]
    [STAThread]
    public void Instantiate()
    {
      CurrencyTextBox control = new CurrencyTextBox();
      Assert.IsNotNull(control);
    }

    [Test]
    [STAThread]
    public void DefaultDecimalPlaces()
    {
      CurrencyTextBox ct = new CurrencyTextBox();

      Assert.AreEqual(-1, ct.DecimalPlaces);
    }

    [Test]
    [STAThread]
    public void DecimalPlacesProperty()
    {
      CurrencyTextBox ct = new CurrencyTextBox();

      ct.DecimalPlaces = 3;
      Assert.AreEqual(3, ct.DecimalPlaces);
    }

    [Test]
    [STAThread]
    [ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void DecimalPlacesCanNotBeLessThanNegativeOne()
    {
      CurrencyTextBox ct = new CurrencyTextBox();
      ct.DecimalPlaces = -2;
    }

    [Test]
    [Ignore("Does not work unless control is initialized")]
    public void DecimalPlaces()
    {
      CurrencyTextBox ct = new CurrencyTextBox();
      ct.Culture = TestCultures.EnglishNZ;

      ct.Value = (decimal)42.947;

      Assert.AreEqual("$42.95", ct.Text);
      ct.DecimalPlaces = 3;
      Assert.AreEqual("$42.947", ct.Text);
    }

    [Test]
    [STAThread]
    public void DecimalPlaces_SettingValue()
    {
      CurrencyTextBox ct = new CurrencyTextBox();
      ct.Culture = TestCultures.EnglishNZ;
      ct.DecimalPlaces = 3;

      ct.Value = (decimal)42.9;
      Assert.AreEqual("$42.900", ct.Text);
    }
  }
}
