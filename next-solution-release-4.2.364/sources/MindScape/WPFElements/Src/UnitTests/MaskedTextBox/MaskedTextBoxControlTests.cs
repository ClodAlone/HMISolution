using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;

namespace Mindscape.WpfElements.UnitTests
{
  [TestFixture]
  public class MaskedTextBoxControlTests
  {
    [Test]
    [STAThread]
    public void Instantiate()
    {
      MaskedTextBox control = new MaskedTextBox();
      Assert.IsNotNull(control);
      Assert.IsInstanceOf<MaskedTextBoxModel>(control.Model);
    }

    [Test]
    [STAThread]
    public void PresenterModel()
    {
      RichTextBox rtb = new RichTextBox();
      rtb.Document = new FlowDocument(new Paragraph(new Run("The quick brown fox jumped over the lazy dog")));

      ITextPresenter presenter = new RichTextBoxTextPresenter(rtb);

      Assert.AreEqual(rtb.CaretPosition, presenter.CaretPosition);
      Assert.AreEqual(rtb.Selection, presenter.Selection);

      presenter.CaretPosition = rtb.CaretPosition.DocumentStart.GetPositionAtOffset(15);

      Assert.AreEqual(rtb.CaretPosition, presenter.CaretPosition);
      Assert.AreEqual(rtb.Selection, presenter.Selection);

      presenter.Selection.Select(presenter.CaretPosition, presenter.CaretPosition.DocumentEnd.GetPositionAtOffset(-15));

      Assert.AreEqual(rtb.CaretPosition, presenter.CaretPosition);
      Assert.AreEqual(rtb.Selection, presenter.Selection);
      Assert.AreEqual("wn fox jumped over", rtb.Selection.Text);  // <15 letters removed because of element start/end TextPointers
    }

    [Test]
    [STAThread]
    public void Undo()
    {
      MaskedTextBox control = new MaskedTextBox();
      control.Mask = "0L0L0";

      control.Text = "1A2";
      control.Text = "1A2B3";

      Assert.AreEqual("1A2B3", control.Text);
      Assert.IsTrue(ApplicationCommands.Undo.CanExecute(null, control));
      ApplicationCommands.Undo.Execute(null, control);
      Assert.AreEqual("1A2", control.Text);

      ApplicationCommands.Undo.Execute(null, control);
      Assert.IsFalse(ApplicationCommands.Undo.CanExecute(null, control));
    }
  }
}
