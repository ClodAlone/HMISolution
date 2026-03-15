using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using System.Globalization;
using System.Threading;

namespace Mindscape.WpfElements.UnitTests
{
  [TestFixture]
  public class CurrencyTextBoxModelTests
  {
    [Test]
    public void CanInsert()
    {
      CurrencyTextBoxModel ct = new CurrencyTextBoxModel();
      ct.Culture = new CultureInfo("en-NZ");

      ct.Text = String.Empty;

      Assert.IsTrue(ct.CanInsert(new PlainCaret(0), "123"));
      Assert.IsFalse(ct.CanInsert(new PlainCaret(0), "123.45.67"));
      Assert.IsTrue(ct.CanInsert(new PlainCaret(0), "123456789.00"));
      Assert.IsFalse(ct.CanInsert(new PlainCaret(0), "12abc"));
      Assert.IsTrue(ct.CanInsert(new PlainCaret(0), "$123,456.78"));
      Assert.IsTrue(ct.CanInsert(new PlainCaret(0), "-$123,456.78"));
      Assert.IsFalse(ct.CanInsert(new PlainCaret(0), "123-456"));
      Assert.IsTrue(ct.CanInsert(new PlainCaret(0), "($123)"));

      ct.Text = "123.45";

      Assert.IsTrue(ct.CanInsert(new PlainCaret(0), "$789"));
      Assert.IsFalse(ct.CanInsert(new PlainCaret(0), "789.00"));
      Assert.IsTrue(ct.CanInsert(new PlainCaret(3), ",456"));
      Assert.IsFalse(ct.CanInsert(new PlainCaret(3), "$456"));
    }

    [Test]
    public void PartialParentheses()
    {
      CurrencyTextBoxModel ct = new CurrencyTextBoxModel();
      ct.Text = "$123.45";
      ct.Culture = new CultureInfo("en-NZ");

      Assert.IsTrue(ct.CanInsert(new PlainCaret(0), "("));
      Assert.IsFalse(ct.CanInsert(new PlainCaret(3), "("));
      Assert.IsFalse(ct.CanInsert(new PlainCaret(7), "("));

      Assert.IsFalse(ct.CanInsert(new PlainCaret(0), ")"));
      Assert.IsFalse(ct.CanInsert(new PlainCaret(3), ")"));
      Assert.IsTrue(ct.CanInsert(new PlainCaret(7), ")"));

      Assert.IsTrue(ct.CanInsert(new PlainRange(0, 2), "("));
      Assert.IsTrue(ct.CanInsert(new PlainRange(5, 2), ")"));

      Assert.IsFalse(ct.CanInsert(new PlainCaret(0), "(("));
      Assert.IsFalse(ct.CanInsert(new PlainCaret(7), "))"));

      ct.Text = "($123.45)";
      Assert.IsFalse(ct.CanInsert(new PlainCaret(0), "("));
      Assert.IsFalse(ct.CanInsert(new PlainCaret(9), ")"));
    }

    [Test]
    public void CanReplace()
    {
      CurrencyTextBoxModel ct = new CurrencyTextBoxModel();
      ct.Culture = new CultureInfo("fr-CH");
      ct.EnforceDecimalPlaces = false;

      ct.Text = "123456.789";
      Assert.AreEqual("123456.789", ct.Text);

      Assert.IsTrue(ct.CanInsert(new PlainRange(2, 1), "789"));
      Assert.IsTrue(ct.CanInsert(new PlainRange(3, 6), "789"));
      Assert.IsFalse(ct.CanInsert(new PlainRange(2, 1), "7.89"));
      Assert.IsTrue(ct.CanInsert(new PlainRange(2, 6), "7.89"));
      Assert.IsTrue(ct.CanInsert(new PlainRange(0, 4), "fr."));
      Assert.IsFalse(ct.CanInsert(new PlainRange(1, 4), "fr."));
    }

    [Test]
    public void RationaliseGroupSeparators()
    {
      CurrencyTextBoxModel ct = new CurrencyTextBoxModel();
      ct.Culture = new CultureInfo("en-NZ");

      ct.Text = "12345.67";
      ct.RationaliseGroupSeparators(false);
      Assert.AreEqual("12345.67", ct.Text);
      ct.RationaliseGroupSeparators(true);
      Assert.AreEqual("12,345.67", ct.Text);

      ct.Text = "12,345.67";
      ct.RationaliseGroupSeparators(false);
      Assert.AreEqual("12,345.67", ct.Text);

      ct.Text = "12,34,5.67";
      ct.RationaliseGroupSeparators(false);
      Assert.AreEqual("12,345.67", ct.Text);

      ct.Text = "12,34,56,78,90";
      ct.RationaliseGroupSeparators(false);
      Assert.AreEqual("1,234,567,890", ct.Text);

      // Ensure irregular groupings handled correctly

      ct.Culture = new CultureInfo("en-BZ");

      ct.Text = "12,34,56,78,90";
      ct.RationaliseGroupSeparators(false);
      Assert.AreEqual("1234567,890", ct.Text);

      ct.Culture = new CultureInfo("sa-IN");

      ct.Text = "12,34,56,78,90";
      ct.RationaliseGroupSeparators(false);
      Assert.AreEqual("1,23,45,67,890", ct.Text);
    }


    [Test]
    public void Insert()
    {
      CurrencyTextBoxModel ct = new CurrencyTextBoxModel();
      ct.Culture = new CultureInfo("en-NZ");

      int newPos;

      ct.Value = 1567.89m;
      Assert.AreEqual("$1,567.89", ct.Text);

      newPos = ct.InsertText(new PlainCaret(2), "2");
      Assert.AreEqual("$12,567.89", ct.Text);
      Assert.AreEqual(3, newPos);

      newPos = ct.InsertText(new PlainCaret(newPos), "3");
      Assert.AreEqual("$123,567.89", ct.Text);
      Assert.AreEqual(4, newPos);

      newPos = ct.InsertText(new PlainCaret(newPos), "4");
      Assert.AreEqual("$1,234,567.89", ct.Text);
      Assert.AreEqual(6, newPos);

      ct.Value = 1567.89m;
      Assert.AreEqual("$1,567.89", ct.Text);

      newPos = ct.InsertText(new PlainCaret(2), "234");
      Assert.AreEqual("$1,234,567.89", ct.Text);
      Assert.AreEqual(6, newPos);

      ct.Value = 1567.89m;
      Assert.AreEqual("$1,567.89", ct.Text);

      newPos = ct.InsertText(new PlainCaret(2), "23,4");
      Assert.AreEqual("$1,234,567.89", ct.Text);
      Assert.AreEqual(6, newPos);

      ct.Value = 127m;
      Assert.AreEqual("$127.00", ct.Text);

      newPos = ct.InsertText(new PlainCaret(3), "3456");
      Assert.AreEqual("$1,234,567.00", ct.Text);
      Assert.AreEqual(9, newPos);
    }

    [Test]
    public void Replace()
    {
      CurrencyTextBoxModel ct = new CurrencyTextBoxModel();
      ct.Culture = new CultureInfo("en-NZ");

      int newPos;

      ct.Value = 14325.67m;
      Assert.AreEqual("$14,325.67", ct.Text);

      newPos = ct.InsertText(new PlainRange(2, 4), "234");
      Assert.AreEqual("$12,345.67", ct.Text);
      Assert.AreEqual(6, newPos);

      newPos = ct.InsertText(new PlainRange(2, 4), "9");
      Assert.AreEqual("$195.67", ct.Text);
      Assert.AreEqual(3, newPos);

      newPos = ct.InsertText(new PlainRange(2, 1), "888888");
      Assert.AreEqual("$18,888,885.67", ct.Text);
      Assert.AreEqual(10, newPos);

      newPos = ct.InsertText(new PlainRange(4, 3), "77,77,77");
      Assert.AreEqual("$18,777,777,885.67", ct.Text);
      Assert.AreEqual(11, newPos);

      newPos = ct.InsertText(new PlainRange(4, 7), "0");
      Assert.AreEqual("$180,885.67", ct.Text);
      Assert.AreEqual(4, newPos);

      newPos = ct.InsertText(new PlainRange(7, 3), "00.0");
      Assert.AreEqual("$1,808,800.07", ct.Text);
      Assert.AreEqual(12, newPos);

      newPos = ct.InsertText(new PlainRange(9, 4), "1");
      Assert.AreEqual("$1,808,801", ct.Text);
      Assert.AreEqual(10, newPos);

      newPos = ct.InsertText(new PlainRange(9, 1), "2");
      Assert.AreEqual("$1,808,802", ct.Text);
      Assert.AreEqual(10, newPos);
    }

    [Test]
    public void Insert_PartialParentheses()
    {
      CurrencyTextBoxModel ct = new CurrencyTextBoxModel();
      ct.Culture = new CultureInfo("en-NZ");

      int newPos;

      ct.Value = 12345.67m;
      Assert.AreEqual("$12,345.67", ct.Text);

      newPos = ct.InsertText(new PlainCaret(0), "(");
      Assert.AreEqual("($12,345.67", ct.Text);
      Assert.AreEqual(1, newPos);

      ct.Value = 12345.67m;
      Assert.AreEqual("$12,345.67", ct.Text);

      newPos = ct.InsertText(new PlainCaret(10), ")");
      Assert.AreEqual("$12,345.67)", ct.Text);
      Assert.AreEqual(11, newPos);
    }

    [Test]
    public void Delete_AtCaret()
    {
      CurrencyTextBoxModel ct = new CurrencyTextBoxModel();
      ct.Culture = new CultureInfo("en-NZ");

      int newPos;

      ct.Value = 1234.56m;
      Assert.AreEqual("$1,234.56", ct.Text);

      newPos = ct.DeleteText(new PlainCaret(9), DeleteDirection.Backward);
      Assert.AreEqual("$1,234.5", ct.Text);
      Assert.AreEqual(8, newPos);

      ct.Value = 123405.67m;
      Assert.AreEqual("$123,405.67", ct.Text);

      newPos = ct.DeleteText(new PlainCaret(6), DeleteDirection.Forward);
      Assert.AreEqual("$12,345.67", ct.Text);
      Assert.AreEqual(6, newPos);

      ct.Value = 123405.67m;
      Assert.AreEqual("$123,405.67", ct.Text);

      newPos = ct.DeleteText(new PlainCaret(7), DeleteDirection.Backward);
      Assert.AreEqual("$12,345.67", ct.Text);
      Assert.AreEqual(6, newPos);

      ct.Value = 12345.67m;
      Assert.AreEqual("$12,345.67", ct.Text);

      newPos = ct.DeleteText(new PlainCaret(4), DeleteDirection.Backward);
      Assert.AreEqual("$12,345.67", ct.Text);
      Assert.AreEqual(3, newPos);

      newPos = ct.DeleteText(new PlainCaret(3), DeleteDirection.Forward);
      Assert.AreEqual("$12,345.67", ct.Text);
      Assert.AreEqual(4, newPos);

      ct.Value = 12345.67m;
      Assert.AreEqual("$12,345.67", ct.Text);

      newPos = ct.DeleteText(new PlainCaret(7), DeleteDirection.Forward);
      Assert.AreEqual("$1,234,567", ct.Text);
      Assert.AreEqual(8, newPos);

      ct.Value = 1234567m;
      Assert.AreEqual("$1,234,567.00", ct.Text);

      newPos = ct.DeleteText(new PlainCaret(12), DeleteDirection.Backward);
      Assert.AreEqual("$1,234,567.0", ct.Text);
      Assert.AreEqual(11, newPos);

      newPos = ct.DeleteText(new PlainCaret(11), DeleteDirection.Forward);
      Assert.AreEqual("$1,234,567.", ct.Text);
      Assert.AreEqual(11, newPos);

      ct.Value = 12345.67m;
      Assert.AreEqual("$12,345.67", ct.Text);

      newPos = ct.DeleteText(new PlainCaret(8), DeleteDirection.Forward);
      Assert.AreEqual("$12,345.7", ct.Text);
      Assert.AreEqual(8, newPos);
    }

    [Test]
    public void Delete_Range()
    {
      CurrencyTextBoxModel ct = new CurrencyTextBoxModel();
      ct.Culture = new CultureInfo("en-NZ");

      int newPos;

      ct.Value = 123000.45m;
      Assert.AreEqual("$123,000.45", ct.Text);

      newPos = ct.DeleteText(new PlainRange(5, 3), DeleteDirection.Forward);
      Assert.AreEqual("$123.45", ct.Text);
      Assert.AreEqual(4, newPos);

      ct.Value = 123000.45m;
      Assert.AreEqual("$123,000.45", ct.Text);

      newPos = ct.DeleteText(new PlainRange(5, 4), DeleteDirection.Forward);
      Assert.AreEqual("$12,345", ct.Text);
      Assert.AreEqual(5, newPos);

      ct.Value = 123456.78m;
      Assert.AreEqual("$123,456.78", ct.Text);

      newPos = ct.DeleteText(new PlainRange(1, 3), DeleteDirection.Forward);
      Assert.AreEqual("$456.78", ct.Text);
      Assert.AreEqual(1, newPos);
    }

    [Test]
    public void Delete_AtStart()
    {
      CurrencyTextBoxModel ct = new CurrencyTextBoxModel();
      ct.Text = "123";

      int newPos;

      newPos = ct.DeleteText(new PlainCaret(0), DeleteDirection.Backward);
      Assert.AreEqual("123", ct.Text);
      Assert.AreEqual(0, newPos);

      newPos = ct.DeleteText(new PlainCaret(3), DeleteDirection.Forward);
      Assert.AreEqual("123", ct.Text);
      Assert.AreEqual(3, newPos);
    }

    [Test]
    public void NoSeparators()
    {
      CurrencyTextBoxModel ct = new CurrencyTextBoxModel();
      ct.Culture = new CultureInfo("en-NZ");
      ct.ShowSeparators = false;
      ct.Value = 1234567;

      Assert.AreEqual("$1234567.00", ct.Text);

      int newPos = ct.InsertText(new PlainCaret(4), "99");
      Assert.AreEqual("$123994567.00", ct.Text);
      Assert.AreEqual(6, newPos);
    }

    [Test]
    public void CultureChangesReflectedIfValueSet()
    {
      CurrencyTextBoxModel ct = new CurrencyTextBoxModel();
      ct.Culture = new CultureInfo("en-NZ");
      ct.Value = 12345.67m;
      Assert.AreEqual("$12,345.67", ct.Text);

      ct.Culture = new CultureInfo("fr-CH");
      Assert.AreEqual("fr. 12'345.67", ct.Text);
    }

    [Test]
    public void LastSetOfTextAndValueIsMaster()
    {
      CurrencyTextBoxModel ct = new CurrencyTextBoxModel();
      ct.Culture = new CultureInfo("en-NZ");

      ct.Value = 12345.67m;
      Assert.AreEqual(12345.67m, ct.Value);
      Assert.AreEqual("$12,345.67", ct.Text);

      ct.Text = "123.4";
      Assert.AreEqual("123.4", ct.Text);
      Assert.AreEqual(123.4m, ct.Value);

      ct.Text = "not a number";
      Assert.AreEqual("not a number", ct.Text);
      Assert.AreEqual(123.4m, ct.Value);

      ct.Value = 432.1m;
      Assert.AreEqual("$432.10", ct.Text);
      Assert.AreEqual(432.1m, ct.Value);
    }

    [Test]
    public void InsertAndDeleteUpdateValueWherePossible()
    {
      CurrencyTextBoxModel ct = new CurrencyTextBoxModel();
      ct.Culture = new CultureInfo("en-NZ");

      ct.Text = "125.6";
      Assert.AreEqual(125.6m, ct.Value);

      ct.InsertText(new PlainCaret(2), "34");
      Assert.AreEqual(12345.6m, ct.Value);

      ct.DeleteText(new PlainRange(5, 2), DeleteDirection.Forward);
      Assert.AreEqual(12346m, ct.Value);
    }

    [Test]
    public void NullTextConvertedToEmptyString()
    {
      CultureInfo threadCulture = CultureInfo.CurrentCulture;
      try
      {
        Thread.CurrentThread.CurrentCulture = TestCultures.EnglishNZ;

        CurrencyTextBoxModel ct = new CurrencyTextBoxModel();
        Assert.AreEqual("$0.00", ct.Text);

        ct.Text = null;
        Assert.AreEqual(String.Empty, ct.Text);
      }
      finally
      {
        Thread.CurrentThread.CurrentCulture = threadCulture;
      }
    }

    [Test]
    public void GroupSeparatorsNotIncorrectlyRemoved()
    {
      CurrencyTextBoxModel ct = new CurrencyTextBoxModel();
      ct.Culture = new CultureInfo("mk-MK");
      ct.Value = 1234.56m;

      Assert.AreEqual("1.234,56 ден.", ct.Text);  // note currency symbol includes same char as group separator

      ct.Culture = TestCultures.EnglishNZ;

      Assert.AreEqual(1234.56m, ct.Value);
      Assert.AreEqual("$1,234.56", ct.Text);
    }

    [Test]
    public void DefaultDecimalPlaces()
    {
      CurrencyTextBoxModel ct = new CurrencyTextBoxModel();

      Assert.AreEqual(-1, ct.DecimalPlaces);
    }

    [Test]
    public void DecimalPlacesProperty()
    {
      CurrencyTextBoxModel ct = new CurrencyTextBoxModel();

      ct.DecimalPlaces = 3;
      Assert.AreEqual(3, ct.DecimalPlaces);
    }

    [Test]
    [ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void DecimalPlacesCanNotBeLessThanNegativeOne()
    {
      CurrencyTextBoxModel ct = new CurrencyTextBoxModel();
      ct.DecimalPlaces = -2;
    }

    [Test]
    public void DecimalPlaces()
    {
      CurrencyTextBoxModel ct = new CurrencyTextBoxModel();
      ct.Culture = TestCultures.EnglishNZ;

      ct.Value = (decimal)42.947;

      Assert.AreEqual("$42.95", ct.Text);
      ct.DecimalPlaces = 3;
      Assert.AreEqual("$42.947", ct.Text);
    }

    [Test]
    public void DecimalPlaces_SettingValue()
    {
      CurrencyTextBoxModel ct = new CurrencyTextBoxModel();
      ct.Culture = TestCultures.EnglishNZ;
      ct.DecimalPlaces = 3;

      ct.Value = (decimal)42.9;
      Assert.AreEqual("$42.900", ct.Text);
    }
  }
}
