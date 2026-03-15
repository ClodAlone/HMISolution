using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using System.Globalization;

namespace Mindscape.WpfElements.UnitTests
{
  [TestFixture]
  public class IntegerTextBoxModelTests
  {
    [Test]
    public void CanInsert()
    {
      IntegerTextBoxModel model = new IntegerTextBoxModel();
      model.Culture = new CultureInfo("en-NZ");

      model.Text = String.Empty;

      Assert.IsTrue(model.CanInsert(new PlainCaret(0), "123"));
      Assert.IsFalse(model.CanInsert(new PlainCaret(0), "123.45.67"));
      Assert.IsFalse(model.CanInsert(new PlainCaret(0), "1234567.89"));
      Assert.IsFalse(model.CanInsert(new PlainCaret(0), "12abc"));
      Assert.IsFalse(model.CanInsert(new PlainCaret(0), "$123,456.78"));
      Assert.IsFalse(model.CanInsert(new PlainCaret(0), "-123,456.78"));
      Assert.IsTrue(model.CanInsert(new PlainCaret(0), "-123"));
      Assert.IsFalse(model.CanInsert(new PlainCaret(0), "123-456"));
      Assert.IsFalse(model.CanInsert(new PlainCaret(0), "(123)"));

      model.Text = "123";

      Assert.IsTrue(model.CanInsert(new PlainCaret(0), "789"));
      Assert.IsFalse(model.CanInsert(new PlainCaret(0), "789.00"));
      Assert.IsTrue(model.CanInsert(new PlainCaret(3), ",456"));
      Assert.IsFalse(model.CanInsert(new PlainCaret(3), ".456"));
    }

    [Test]
    public void PartialParentheses()
    {
      IntegerTextBoxModel model = new IntegerTextBoxModel();
      model.Text = "123";
      model.Culture = new CultureInfo("en-NZ");

      Assert.IsTrue(model.CanInsert(new PlainCaret(0), "("));
      Assert.IsFalse(model.CanInsert(new PlainCaret(1), "("));
      Assert.IsFalse(model.CanInsert(new PlainCaret(3), "("));

      Assert.IsFalse(model.CanInsert(new PlainCaret(0), ")"));
      Assert.IsFalse(model.CanInsert(new PlainCaret(1), ")"));
      Assert.IsTrue(model.CanInsert(new PlainCaret(3), ")"));

      Assert.IsTrue(model.CanInsert(new PlainRange(0, 2), "("));
      Assert.IsTrue(model.CanInsert(new PlainRange(1, 2), ")"));

      Assert.IsFalse(model.CanInsert(new PlainCaret(0), "(("));
      Assert.IsFalse(model.CanInsert(new PlainCaret(3), "))"));

      model.Text = "(123)";
      Assert.IsFalse(model.CanInsert(new PlainCaret(0), "("));
      Assert.IsFalse(model.CanInsert(new PlainCaret(5), ")"));
    }

    [Test]
    public void CanReplace()
    {
      IntegerTextBoxModel model = new IntegerTextBoxModel();
      model.Culture = new CultureInfo("fr-CH");

      model.Text = "123456";
      Assert.AreEqual("123456", model.Text);

      Assert.IsTrue(model.CanInsert(new PlainRange(2, 1), "789"));
      Assert.IsTrue(model.CanInsert(new PlainRange(2, 4), "789"));
      Assert.IsFalse(model.CanInsert(new PlainRange(2, 1), "7.89"));
      Assert.IsFalse(model.CanInsert(new PlainRange(0, 4), "SFr."));
      Assert.IsFalse(model.CanInsert(new PlainRange(1, 4), "SFr."));
    }

    [Test]
    public void GroupSeparatorsNotShown()
    {
      IntegerTextBoxModel model = new IntegerTextBoxModel();
      model.ShowSeparators = false;  // in real environment this is set by the control
      model.Culture = new CultureInfo("en-NZ");
      model.Value = 12345;
      Assert.AreEqual("12345", model.Text);

      model.Culture = new CultureInfo("fr-CH");
      Assert.AreEqual("12345", model.Text);

      model.RationaliseGroupSeparators(true);
      Assert.AreEqual("12345", model.Text);
    }

    [Test]
    public void Precision()
    {
      IntegerTextBoxModel model = new IntegerTextBoxModel();
      model.Culture = new CultureInfo("en-NZ");
      model.Value = 12345;
      model.ShowSeparators = false;  // in real environment this is set by the control
      Assert.AreEqual("12345", model.Text);

      model.Precision = 8;
      Assert.AreEqual("00012345", model.Text);

      model.Precision = 4;
      Assert.AreEqual("12345", model.Text);

      model.Precision = 0;
      Assert.AreEqual("12345", model.Text);
    }

    [Test]
    [ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void PrecisionIsNonNegative()
    {
      IntegerTextBoxModel model = new IntegerTextBoxModel();
      model.Culture = new CultureInfo("en-NZ");
      model.Value = 12345;
      model.Precision = -1;
    }
  }
}
