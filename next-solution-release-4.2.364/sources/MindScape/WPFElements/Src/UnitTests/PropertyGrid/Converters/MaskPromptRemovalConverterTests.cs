using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;

namespace Mindscape.WpfElements.WpfPropertyGrid.UnitTests
{
  [TestFixture]
  public class MaskPromptRemovalConverterTests
  {
    [Test]
    public void RemovePromptChars()
    {
      MaskPromptRemovalConverter converter = new MaskPromptRemovalConverter();
      object converted = converter.ConvertBack("__123__", typeof(int), null, null);
      Assert.AreEqual(123, (int)converted);
    }

    [Test]
    public void HandleNonDefaultPromptChar()
    {
      MaskPromptRemovalConverter converter = new MaskPromptRemovalConverter();
      converter.PromptChar = '*';
      object converted = converter.ConvertBack("**123**", typeof(int), null, null);
      Assert.AreEqual(123, (int)converted);
    }

    [Test]
    public void PromptCharInMiddleIsNotRemoved()
    {
      MaskPromptRemovalConverter converter = new MaskPromptRemovalConverter();
      object converted = converter.ConvertBack("123_45", typeof(int), null, null);
      Assert.AreEqual("123_45", converted);
    }

    [Test]
    public void ConvertIsPassthrough()
    {
      MaskPromptRemovalConverter converter = new MaskPromptRemovalConverter();
      object converted = converter.Convert("__123__", null, null, null);
      Assert.AreEqual("__123__", converted);
    }
  }
}
