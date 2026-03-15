using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows;

namespace Mindscape.WpfElements.UnitTests
{
  [TestFixture]
  public class NumericTextBoxControlTests
  {
    [Test]
    [STAThread]
    public void Instantiate()
    {
      NumericTextBox control1 = new NumericTextBox();
      Assert.IsNotNull(control1);
      Assert.IsInstanceOf<NumericTextBoxModel>(control1.NumericModel);

      IntegerTextBox control2 = new IntegerTextBox();
      Assert.IsNotNull(control2);
      Assert.IsInstanceOf<IntegerTextBoxModel>(control2.NumericModel);
    }

    [Test]
    [STAThread]
    public void Precision()
    {
      IntegerTextBox control = new IntegerTextBox();
      control.BeginInit();
      control.Value = 123;
      control.EndInit();

      control.Precision = 0;
      Assert.AreEqual("123", control.Text);

      control.Precision = 5;
      Assert.AreEqual("00123", control.Text);

      Assert.AreEqual(5, control.Precision);
    }

    [Test]
    [STAThread]
    public void PresenterModel()
    {
      TextBox tb = new TextBox();
      tb.Text = "The quick brown fox jumped over the lazy dog";
      tb.Select(5, 10);

      IPlainTextPresenter presenter = new TextBoxTextPresenter(tb);

      Assert.AreEqual(5, presenter.SelectionStart);
      Assert.AreEqual(10, presenter.SelectionLength);
      Assert.AreEqual(5, presenter.CaretPosition);

      presenter.CaretPosition = 5;

      Assert.AreEqual(5, tb.CaretIndex);
      Assert.AreEqual(5, tb.SelectionStart);
      Assert.AreEqual(0, tb.SelectionLength);

      presenter.Select(10, 5);

      Assert.AreEqual(10, tb.SelectionStart);
      Assert.AreEqual(5, tb.SelectionLength);
      Assert.AreEqual(10, tb.CaretIndex);

      Assert.AreEqual("brown", tb.SelectedText);
    }

    [Test]
    [STAThread]
    public void Undo()
    {
      IntegerTextBox control = new IntegerTextBox();

      control.Text = "123";
      control.Text = "1234";

      Assert.AreEqual("1234", control.Text);
      Assert.AreEqual(1234, control.Value);
      Assert.IsTrue(ApplicationCommands.Undo.CanExecute(null, control));
      ApplicationCommands.Undo.Execute(null, control);
      Assert.AreEqual("123", control.Text);
      Assert.AreEqual(123, control.Value);

      ApplicationCommands.Undo.Execute(null, control);
      Assert.IsFalse(ApplicationCommands.Undo.CanExecute(null, control));
    }

    [Test]
    [STAThread]
    public void Paste()
    {
      IntegerTextBox control = new IntegerTextBox();
      control.ShowSeparators = false;  // legit because the style sets this

      Clipboard.SetText("123");
      ApplicationCommands.Paste.Execute(null, control);
      Assert.AreEqual("1230", control.Text);

      Clipboard.SetText("Fie!");
      ApplicationCommands.Paste.Execute(null, control);
      Assert.AreEqual("1230", control.Text);

      Clipboard.SetText("456", TextDataFormat.UnicodeText);
      ApplicationCommands.Paste.Execute(null, control);
      Assert.AreEqual("4561230", control.Text);
      Assert.AreEqual(4561230, control.Value);
    }

    [Test]
    [STAThread]
    public void MaximumMinimum()
    {
      IntegerTextBox control = new IntegerTextBox();
      control.BeginInit();
      control.Value = 123;
      control.EndInit();

      Assert.AreEqual("123", control.Text);

      control.Minimum = 250;
      Assert.AreEqual(250, control.Minimum);
      Assert.AreEqual(250, control.Value);
      Assert.AreEqual("250", control.Text);
      control.Minimum = 0;

      control.Maximum = 150;
      Assert.AreEqual(150, control.Maximum);
      Assert.AreEqual(150, control.Value);
      Assert.AreEqual("150", control.Text);
    }
  }
}
