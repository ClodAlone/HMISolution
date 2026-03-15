using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;

namespace Mindscape.WpfElements.UnitTests
{
  [TestFixture]
  public class TextUtilsTests
  {
    [Test]
    public void MoveCaretToTextOffset()
    {
      RichCaret c = new RichCaret(5);
      Assert.AreEqual(5, TextUtils.GetOffsetOfPointer(c.CaretPosition));

      TextUtils.MoveCaretToTextOffset(c, 7);
      Assert.AreEqual(7, TextUtils.GetOffsetOfPointer(c.CaretPosition));
    }

    [Test]
    public void SetSelectionUsingTextOffset()
    {
      RichRange r = new RichRange(8, 16);
      Assert.AreEqual(8, TextUtils.GetOffsetOfPointer(r.Selection.Start));
      Assert.AreEqual(16, TextUtils.GetOffsetOfPointer(r.Selection.End));

      TextUtils.SetSelectionUsingTextOffset(r, 3, 12);
      Assert.AreEqual(3, TextUtils.GetOffsetOfPointer(r.Selection.Start));
      Assert.AreEqual(12, TextUtils.GetOffsetOfPointer(r.Selection.End));
    }
  }
}
