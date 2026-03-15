using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;

namespace Mindscape.WpfElements.UnitTests
{
  [TestFixture]
  public class MaskedTextUndoInfoTests
  {
    [Test]
    public void MaskedTextUndoInfoFactory_Caret()
    {
      MaskedTextUndoInfo info = MaskedTextUndoInfo.Create("1234567890", new RichCaret(5));

      Assert.IsInstanceOf<CaretMaskedTextUndoInfo>(info);
      Assert.AreEqual("1234567890", info.Text);

      RichCaret newDocument = new RichCaret(7);
      info.SetCursorPosition(newDocument);

      Assert.AreEqual(5, TextUtils.GetOffsetOfPointer(newDocument.CaretPosition));
    }

    [Test]
    public void MaskedTextUndoInfoFactory_Range()
    {
      MaskedTextUndoInfo info = MaskedTextUndoInfo.Create("1234567890", new RichRange(5, 10));

      Assert.IsInstanceOf<RangeMaskedTextUndoInfo>(info);
      Assert.AreEqual("1234567890", info.Text);

      RichRange newDocument = new RichRange(7, 9);
      info.SetCursorPosition(newDocument);

      Assert.AreEqual(5, TextUtils.GetOffsetOfPointer(newDocument.Selection.Start));
      Assert.AreEqual(10, TextUtils.GetOffsetOfPointer(newDocument.Selection.End));
    }

    [Test]
    [ExpectedException(typeof(ArgumentNullException))]
    public void MaskedTextUndoInfoFactory_RejectsNull()
    {
      MaskedTextUndoInfo.Create("1234567890", null);
    }
  }
}
