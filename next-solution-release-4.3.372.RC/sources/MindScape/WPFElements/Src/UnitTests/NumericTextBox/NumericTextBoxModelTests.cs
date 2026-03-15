using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using System.Globalization;

namespace Mindscape.WpfElements.UnitTests
{
  [TestFixture]
  public class NumericTextBoxModelTests
  {
    [Test]
    public void CanInsert()
    {
      NumericTextBoxModel model = new NumericTextBoxModel();
      model.Culture = new CultureInfo("en-NZ");

      model.Text = String.Empty;

      Assert.IsTrue(model.CanInsert(new PlainCaret(0), "123"));
      Assert.IsFalse(model.CanInsert(new PlainCaret(0), "123.45.67"));
      Assert.IsTrue(model.CanInsert(new PlainCaret(0), "123456789.00"));
      Assert.IsFalse(model.CanInsert(new PlainCaret(0), "12abc"));
      Assert.IsFalse(model.CanInsert(new PlainCaret(0), "$123,456.78"));
      Assert.IsTrue(model.CanInsert(new PlainCaret(0), "-123,456.78"));
      Assert.IsFalse(model.CanInsert(new PlainCaret(0), "123-456"));
      Assert.IsTrue(model.CanInsert(new PlainCaret(0), "(123)"));

      model.Text = "123.45";

      Assert.IsTrue(model.CanInsert(new PlainCaret(0), "789"));
      Assert.IsFalse(model.CanInsert(new PlainCaret(0), "789.00"));
      Assert.IsTrue(model.CanInsert(new PlainCaret(3), ",456"));
      Assert.IsFalse(model.CanInsert(new PlainCaret(3), ".456"));
    }

    [Test]
    public void PartialParentheses()
    {
      NumericTextBoxModel model = new NumericTextBoxModel();
      model.Text = "123.45";
      model.Culture = new CultureInfo("en-NZ");

      Assert.IsTrue(model.CanInsert(new PlainCaret(0), "("));
      Assert.IsFalse(model.CanInsert(new PlainCaret(3), "("));
      Assert.IsFalse(model.CanInsert(new PlainCaret(6), "("));

      Assert.IsFalse(model.CanInsert(new PlainCaret(0), ")"));
      Assert.IsFalse(model.CanInsert(new PlainCaret(3), ")"));
      Assert.IsTrue(model.CanInsert(new PlainCaret(6), ")"));

      Assert.IsTrue(model.CanInsert(new PlainRange(0, 2), "("));
      Assert.IsTrue(model.CanInsert(new PlainRange(4, 2), ")"));

      Assert.IsFalse(model.CanInsert(new PlainCaret(0), "(("));
      Assert.IsFalse(model.CanInsert(new PlainCaret(6), "))"));

      model.Text = "(123.45)";
      Assert.IsFalse(model.CanInsert(new PlainCaret(0), "("));
      Assert.IsFalse(model.CanInsert(new PlainCaret(8), ")"));
    }

    [Test]
    public void CanReplace()
    {
      NumericTextBoxModel model = new NumericTextBoxModel();
      model.Culture = new CultureInfo("fr-CH");

      model.Text = "123456.789";
      Assert.AreEqual("123456.789", model.Text);

      Assert.IsTrue(model.CanInsert(new PlainRange(2, 1), "789"));
      Assert.IsTrue(model.CanInsert(new PlainRange(3, 6), "789"));
      Assert.IsFalse(model.CanInsert(new PlainRange(2, 1), "7.89"));
      Assert.IsTrue(model.CanInsert(new PlainRange(2, 6), "7.89"));
      Assert.IsFalse(model.CanInsert(new PlainRange(0, 4), "SFr."));
      Assert.IsFalse(model.CanInsert(new PlainRange(1, 4), "SFr."));
    }

    [Test]
    public void CultureChangesReflectedIfValueSet()
    {
      NumericTextBoxModel model = new NumericTextBoxModel();
      model.Culture = new CultureInfo("en-NZ");
      model.Value = 12345.67m;
      Assert.AreEqual("12,345.67", model.Text);

      model.Culture = new CultureInfo("fr-CH");
      Assert.AreEqual("12'345.67", model.Text);
    }

    [Test]
    public void RationaliseGroupSeparators()
    {
      NumericTextBoxModel model = new NumericTextBoxModel();
      model.Culture = new CultureInfo("en-NZ");

      model.Text = "12345.67";
      model.RationaliseGroupSeparators(false);
      Assert.AreEqual("12345.67", model.Text);
      model.RationaliseGroupSeparators(true);
      Assert.AreEqual("12,345.67", model.Text);

      model.Text = "12,345.67";
      model.RationaliseGroupSeparators(false);
      Assert.AreEqual("12,345.67", model.Text);

      model.Text = "12,34,5.67";
      model.RationaliseGroupSeparators(false);
      Assert.AreEqual("12,345.67", model.Text);

      model.Text = "12,34,56,78,90";
      model.RationaliseGroupSeparators(false);
      Assert.AreEqual("1,234,567,890", model.Text);

      // Ensure picking up numeric grouping in cultures where
      // numeric and currency differ

      model.Culture = new CultureInfo("en-BZ");

      model.Text = "12,34,56,78,90";
      model.RationaliseGroupSeparators(false);
      Assert.AreEqual("1,234,567,890", model.Text);

      // Ensure irregular groupings handled correctly

      model.Culture = new CultureInfo("sa-IN");

      model.Text = "12,34,56,78,90";
      model.RationaliseGroupSeparators(false);
      Assert.AreEqual("1,23,45,67,890", model.Text);
    }

    [Test]
    public void Range()
    {
      NumericTextBoxModel model = new NumericTextBoxModel();

      Assert.AreEqual(Decimal.MinValue, model.Minimum);
      Assert.AreEqual(Decimal.MaxValue, model.Maximum);

      model.Minimum = 0;
      Assert.AreEqual(0m, model.Minimum);

      model.Maximum = 100;
      Assert.AreEqual(0m, model.Minimum);
      Assert.AreEqual(100m, model.Maximum);

      model.Minimum = 200m;
      Assert.AreEqual(200m, model.Minimum);
      Assert.AreEqual(200m, model.Maximum);

      model.Minimum = 0m;
      model.Maximum = 1000m;

      model.Value = 500m;
      Assert.AreEqual(500m, model.Value);

      model.Maximum = 200m;
      Assert.AreEqual(200m, model.Value);

      model.Maximum = 1000m;
      model.Minimum = 750m;
      Assert.AreEqual(750m, model.Value);

      model.Text = "800";
      Assert.AreEqual(800m, model.Value);

      model.Text = "1200";
      Assert.AreEqual(800m, model.Value);
      Assert.AreEqual("1200", model.Text);
    }

    [Test]
    public void DecimalPlaces()
    {
      NumericTextBoxModel model = new NumericTextBoxModel();
      model.Culture = TestCultures.EnglishNZ;

      model.Value = 12345.6789m;

      Assert.AreEqual("12,345.68", model.Text);
      model.DecimalPlaces = 3;
      Assert.AreEqual("12,345.679", model.Text);
    }

    [Test]
    public void DecimalPlaces_SettingValue()
    {
      NumericTextBoxModel model = new NumericTextBoxModel();
      model.Culture = TestCultures.EnglishNZ;
      model.DecimalPlaces = 3;

      model.Value = 12345.6789m;
      Assert.AreEqual("12,345.679", model.Text);
    }

    /*[Test]
    [ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void Range_OutOfRangeValueSetThrowsException()
    {
      NumericTextBoxModel model = new NumericTextBoxModel();
      model.Minimum = 0;
      model.Maximum = 1000m;

      model.Value = -1m;
    }*/

    [Test]
    public void CanNotSetValueLessThanMinimum()
    {
      NumericTextBoxModel model = new NumericTextBoxModel();
      model.Minimum = 0;
      model.Maximum = 1000m;

      model.Value = 50m;
      model.Value = -1m;
      Assert.AreEqual(0, model.Value);
    }

    [Test]
    public void CanNotSetValueGreaterThanMaximum()
    {
      NumericTextBoxModel model = new NumericTextBoxModel();
      model.Minimum = 0;
      model.Maximum = 1000m;

      model.Value = 2000m;
      Assert.AreEqual(1000m, model.Value);
    }

    [Test]
    public void AllowsNoValueIsFalseByDefault()
    {
      NumericTextBoxModel model = new NumericTextBoxModel();
      Assert.IsFalse(model.AllowsNoValue);
    }

    [Test]
    public void HasValueIsTrueByDefault()
    {
      NumericTextBoxModel model = new NumericTextBoxModel();
      Assert.IsTrue(model.HasValue);
    }

    [Test]
    public void AllowsNoValueProperty()
    {
      NumericTextBoxModel model = new NumericTextBoxModel();
      model.AllowsNoValue = true;
      Assert.IsTrue(model.AllowsNoValue);
      model.AllowsNoValue = false;
      Assert.IsFalse(model.AllowsNoValue);
    }

    [Test]
    public void HasValueProperty()
    {
      NumericTextBoxModel model = new NumericTextBoxModel();
      model.HasValue = false;
      Assert.IsFalse(model.HasValue);
      model.HasValue = true;
      Assert.IsTrue(model.HasValue);
    }

    [Test]
    public void SettingAllowsNoValueStillHasValue()
    {
      NumericTextBoxModel model = new NumericTextBoxModel();
      model.AllowsNoValue = true;

      Assert.IsTrue(model.HasValue);
    }

    [Test]
    [Ignore("TODO: in HasValue setter, check if the value is zero?")]
    public void SettingHasValueToFalseWhenThereIsValueRevertsToTrue()
    {
      NumericTextBoxModel model = new NumericTextBoxModel();
      model.AllowsNoValue = true;

      model.Value = 42;
      model.HasValue = false;
      Assert.IsTrue(model.HasValue);
    }

    [Test]
    public void SettingEmptyTextSetsHasValueToFalse()
    {
      NumericTextBoxModel model = new NumericTextBoxModel();
      model.AllowsNoValue = true;

      model.Text = "";
      Assert.IsFalse(model.HasValue);
    }

    [Test]
    public void SettingNullTextSetsHasValueToFalse()
    {
      NumericTextBoxModel model = new NumericTextBoxModel();
      model.AllowsNoValue = true;

      model.Text = null;
      Assert.IsFalse(model.HasValue);
    }

    [Test]
    public void SettingTextSetsHasValueToTrue()
    {
      NumericTextBoxModel model = new NumericTextBoxModel();
      model.AllowsNoValue = true;

      model.HasValue = false;
      Assert.IsFalse(model.HasValue);
      model.Text = "67";
      Assert.IsTrue(model.HasValue);
    }

    [Test]
    public void SettingHasValueToFalseSetsTextToEmpty()
    {
      NumericTextBoxModel model = new NumericTextBoxModel();
      model.AllowsNoValue = true;

      model.HasValue = false;
      Assert.AreEqual("", model.Text);
    }

    [Test]
    public void SettingNonZeroValueSetsHasValueToTrue()
    {
      NumericTextBoxModel model = new NumericTextBoxModel();
      model.AllowsNoValue = true;

      model.HasValue = false;
      Assert.IsFalse(model.HasValue);
      model.Value = 42;
      Assert.IsTrue(model.HasValue);
    }
  }
}
