using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;

namespace Mindscape.WpfElements.UnitTests
{
  [TestFixture]
  public class PlainTextUndoInfoTests
  {
    [Test]
    public void PlainTextUndoInfoFactory_Caret()
    {
      PlainTextUndoInfo info = PlainTextUndoInfo.Create("1234567890", new PlainCaret(5));

      Assert.IsInstanceOf<CaretPlainTextUndoInfo>(info);
      Assert.AreEqual("1234567890", info.Text);

      PlainCaret newTextBox = new PlainCaret(7);
      info.SetCursorPosition(newTextBox);

      Assert.AreEqual(5, newTextBox.CaretPosition);
    }

    [Test]
    public void PlainTextUndoInfoFactory_Range()
    {
      PlainTextUndoInfo info = PlainTextUndoInfo.Create("1234567890", new PlainRange(5, 10));

      Assert.IsInstanceOf<RangePlainTextUndoInfo>(info);
      Assert.AreEqual("1234567890", info.Text);

      PlainRange newTextBox = new PlainRange(7, 9);
      info.SetCursorPosition(newTextBox);

      Assert.AreEqual(5, newTextBox.SelectionStart);
      Assert.AreEqual(10, newTextBox.SelectionLength);
    }

    [Test]
    [ExpectedException(typeof(ArgumentNullException))]
    public void PlainTextUndoInfoFactory_RejectsNull()
    {
      PlainTextUndoInfo.Create("1234567890", null);
    }
  }
}
