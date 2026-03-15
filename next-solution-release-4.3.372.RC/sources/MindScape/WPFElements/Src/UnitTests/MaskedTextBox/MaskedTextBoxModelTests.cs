using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using System.Windows.Documents;
using System.Windows;
using System.ComponentModel;
using System.Windows.Controls;
using System.Globalization;
using System.Threading;

namespace Mindscape.WpfElements.UnitTests
{
  [TestFixture]
  public class MaskedTextBoxModelTests
  {
    [Test]
    public void NoMask()
    {
      string sampleText = "The quick brown fox jumped over the lazy dog";
      MaskedTextBoxModel mt = new MaskedTextBoxModel();
      mt.Text = sampleText;
      Assert.AreEqual(sampleText, mt.Text);
      Assert.AreEqual(sampleText, mt.DisplayText);

      int i = 0;
      foreach (MaskedTextDisplayElement displayElement in mt.GetDisplayChars())
      {
        Assert.IsInstanceOf<InputDisplayElement>(displayElement);
        InputDisplayElement mch = (InputDisplayElement)displayElement;
        Assert.AreEqual(mt.Text[i], mch.Char);
        Assert.AreEqual(DisplayElementType.Input, mch.ElementType);
        ++i;
      }

      mt.InsertAtDisplayPos("/", 10);
      Assert.IsTrue(mt.LastOperationSucceeded);

      mt.InsertAtDisplayPos("/qw*&1", 20);
      Assert.IsTrue(mt.LastOperationSucceeded);

      mt.RemoveAtDisplayPos(5);
      Assert.IsTrue(mt.LastOperationSucceeded);

      mt.RemoveAtDisplayPosRange(5, 15);
      Assert.IsTrue(mt.LastOperationSucceeded);

      mt.ReplaceAtDisplayPosRange("woo woo", 4, 6);
      Assert.IsTrue(mt.LastOperationSucceeded);

      Assert.IsNotNull(mt.CreateDisplayDocument(new TestStyleProvider()));

      Assert.IsTrue(mt.IsMaskCompleted);
      Assert.IsFalse(mt.IsMaskFull);
    }

    [Test]
    public void Literals()
    {
      string sampleText = "+64 (21) 494 279";
      string mask = "+00 (00) 000 000";
      MaskedTextBoxModel mt = new MaskedTextBoxModel();
      mt.Mask = mask;
      mt.Text = sampleText;

      Assert.AreEqual(sampleText, mt.Text);

      int i = 0;
      foreach (MaskedTextDisplayElement displayElement in mt.GetDisplayChars())
      {
        Assert.IsInstanceOf<CharacterDisplayElement>(displayElement);
        CharacterDisplayElement mch = (CharacterDisplayElement)displayElement;
        Assert.AreEqual(sampleText[i], mch.Char);
        if (mask[i] == '0')
        {
          Assert.AreEqual(DisplayElementType.Input, mch.ElementType);
        }
        else
        {
          Assert.AreEqual(DisplayElementType.Literal, mch.ElementType);
        }
        ++i;
      }
    }

    [Test]
    public void Literals_EscapedPrompts()
    {
      MaskedTextBoxModel mt = new MaskedTextBoxModel();
      mt.Mask = @"\A0K0\L0\00\<0\\0\$0";

      string expectedLiterals = @"AKL0<\$";

      List<MaskedTextDisplayElement> mcs = new List<MaskedTextDisplayElement>(mt.GetDisplayChars());

      Assert.AreEqual(14, mcs.Count);
      for (int i = 0; i < mcs.Count; i += 2)
      {
        Assert.AreEqual(DisplayElementType.Literal, mcs[i].ElementType);
        Assert.AreEqual(expectedLiterals[i / 2], ((LiteralDisplayElement)(mcs[i])).Char);
      }
      for (int i = 1; i < mcs.Count; i += 2)
      {
        Assert.AreEqual(DisplayElementType.Prompt, mcs[i].ElementType);
        Assert.AreEqual(ExpectedInputType.Digit, ((IPrompt)(mcs[i])).ExpectedInputType);
      }
    }

    [Test]
    public void Prompts()
    {
      string sampleText = "+64 (21) ";
      string mask = "+00 (00) 000 000";
      MaskedTextBoxModel mt = new MaskedTextBoxModel();
      mt.Mask = mask;
      mt.Text = sampleText;
      mt.PromptChar = '&';

      //Assert.AreEqual(sampleText, mt.Text);

      int i = 0;
      foreach (MaskedTextDisplayElement mch in mt.GetDisplayChars())
      {
        if (i < sampleText.Length)
        {
          Assert.AreEqual(sampleText[i], ((CharacterDisplayElement)mch).Char);
        }
        if (mask[i] == '0' && i <= 8)
        {
          Assert.AreEqual(DisplayElementType.Input, mch.ElementType);
        }
        else if (mask[i] == '0' && i > 8)
        {
          Assert.AreEqual(DisplayElementType.Prompt, mch.ElementType);
        }
        else
        {
          Assert.AreEqual(DisplayElementType.Literal, mch.ElementType);
        }
        ++i;
      }
    }

    [Test]
    public void InsertionsAndPrompts()
    {
      MaskedTextBoxModel mt = new MaskedTextBoxModel();
      mt.Text = "123";
      mt.Mask = "000???";
      mt.InsertAtDisplayPos("X", 4);

      Assert.AreEqual("123 X", mt.Text);

      List<MaskedTextDisplayElement> mcs = new List<MaskedTextDisplayElement>(mt.GetDisplayChars());

      Assert.AreEqual(DisplayElementType.Prompt, mcs[3].ElementType);
      Assert.AreEqual(DisplayElementType.Input, mcs[4].ElementType);
      Assert.AreEqual(DisplayElementType.Prompt, mcs[5].ElementType);
    }

    [Test]
    public void MaskCompletion()
    {
      MaskedTextBoxModel mt = new MaskedTextBoxModel();
      mt.Mask = "(00)00";
      mt.Text = "(12)";
      Assert.IsFalse(mt.IsMaskCompleted);
      Assert.IsFalse(mt.IsMaskFull);

      mt.InsertAtDisplayPos("34", 4);
      Assert.IsTrue(mt.IsMaskCompleted);
      Assert.IsTrue(mt.IsMaskFull);
    }

    [Test]
    public void MaskCompletion_OptionalChars()
    {
      MaskedTextBoxModel mt = new MaskedTextBoxModel();
      mt.Mask = "(0#)0#";
      mt.Text = "(1 )";
      Assert.IsFalse(mt.IsMaskCompleted);
      Assert.IsFalse(mt.IsMaskFull);

      mt.InsertAtDisplayPos("3", 4);
      Assert.IsTrue(mt.IsMaskCompleted);
      Assert.IsFalse(mt.IsMaskFull);

      mt.InsertAtDisplayPos("2", 2);
      mt.InsertAtDisplayPos("4", 5);
      Assert.IsTrue(mt.IsMaskCompleted);
      Assert.IsTrue(mt.IsMaskFull);
    }

    [Test]
    public void ChangeMask()
    {
      MaskedTextBoxModel mt = new MaskedTextBoxModel();
      mt.Mask = "00AALL&&";
      mt.Text = "12AA";

      Assert.AreEqual("12AA", mt.Text);

      mt.Mask = String.Empty;

      Assert.AreEqual("12AA", mt.Text);

      mt.Mask = "00AA";

      Assert.AreEqual("12AA", mt.Text);

      mt.Mask = "LL&&";

      Assert.AreEqual(String.Empty, mt.Text);
    }

    [Test]
    public void AllowedInput()
    {
      string mask = "+00 (00) 000 000";
      MaskedTextBoxModel mt = new MaskedTextBoxModel();
      mt.Mask = mask;

      mt.InsertAtDisplayPos("+", 0);
      Assert.IsTrue(mt.LastOperationSucceeded);

      mt.InsertAtDisplayPos("1", 0);
      Assert.IsFalse(mt.LastOperationSucceeded);

      mt.InsertAtDisplayPos("1", 1);
      Assert.IsTrue(mt.LastOperationSucceeded);

      mt.InsertAtDisplayPos("x", 1);
      Assert.IsFalse(mt.LastOperationSucceeded);

      mt.InsertAtDisplayPos(" ", 3);
      Assert.IsTrue(mt.LastOperationSucceeded);

      mt.InsertAtDisplayPos("2", 3);
      Assert.IsFalse(mt.LastOperationSucceeded);

      mt.InsertAtDisplayPos(" ", 4);
      Assert.IsFalse(mt.LastOperationSucceeded);

      mt.Text = "+64 (21) 279";

      mt.InsertAtDisplayPos("494", 9);
      Assert.IsTrue(mt.LastOperationSucceeded);
      Assert.AreEqual("+64 (21) 494 279", mt.Text);

      mt.Text = "+64 (21) 279";

      mt.InsertAtDisplayPos("4 94", 11);
      Assert.IsTrue(mt.LastOperationSucceeded);
      Assert.AreEqual("+64 (21) 274 949", mt.Text);

      mt.Text = "+64 (21) 279";

      mt.InsertAtDisplayPos("XYZ", 11);
      Assert.IsFalse(mt.LastOperationSucceeded);
      Assert.AreEqual("+64 (21) 279 ", mt.Text);
    }

    [Test]
    public void AllowedRemoval()
    {
      string mask = "+00 (00) 000 000";
      MaskedTextBoxModel mt = new MaskedTextBoxModel();
      mt.Mask = mask;
      mt.Text = "+64 (21) 494 279";

      mt.RemoveAtDisplayPos(0);
      Assert.IsFalse(mt.LastOperationSucceeded);

      mt.RemoveAtDisplayPos(1);
      Assert.IsTrue(mt.LastOperationSucceeded);

      mt.RemoveAtDisplayPos(2);
      Assert.IsTrue(mt.LastOperationSucceeded);

      mt.RemoveAtDisplayPos(3);
      Assert.IsFalse(mt.LastOperationSucceeded);

      mt.RemoveAtDisplayPos(16);
      Assert.IsFalse(mt.LastOperationSucceeded);

      mt.Text = "+64 (21) 494 279";

      mt.RemoveAtDisplayPosRange(13, 15);
      Assert.IsTrue(mt.LastOperationSucceeded);
      Assert.AreEqual("+64 (21) 494 ", mt.Text);

      mt.RemoveAtDisplayPosRange(4, 6);
      Assert.IsTrue(mt.LastOperationSucceeded);
      Assert.AreEqual("+64 (49) 4   ", mt.Text);
    }

    [Test]
    public void InsertText_AtCaret()
    {
      MaskedTextBoxModel mt = new MaskedTextBoxModel();
      mt.Mask = "000000LLLL";
      mt.Text = "145";

      RichCaret tsp = new RichCaret(1);

      int newPos = mt.InsertText(tsp, "2A3");
      Assert.IsFalse(mt.LastOperationSucceeded);
      Assert.AreEqual(MaskedTextResultHint.DigitExpected, mt.LastOperationResult);
      Assert.AreEqual("145", mt.Text);

      tsp = new RichCaret(1);
      newPos = mt.InsertText(tsp, "23");
      Assert.IsTrue(mt.LastOperationSucceeded);
      Assert.AreEqual(3, newPos);
      Assert.AreEqual("12345", mt.Text);

      tsp = new RichCaret(5);
      newPos = mt.InsertText(tsp, "6AB");
      Assert.IsTrue(mt.LastOperationSucceeded);
      Assert.AreEqual(8, newPos);
      Assert.AreEqual("123456AB", mt.Text);
    }

    [Test]
    public void InsertText_AtCaret_SkipOverLiterals()
    {
      MaskedTextBoxModel mt = new MaskedTextBoxModel();
      mt.Mask = "+00 00";
      mt.Text = String.Empty;

      RichCaret caret = new RichCaret(0);
      int newPos = mt.InsertText(caret, "1");
      Assert.IsFalse(mt.LastOperationSucceeded);

      mt.AutoSkipLiterals = true;

      newPos = mt.InsertText(caret, "1");
      Assert.IsTrue(mt.LastOperationSucceeded);
      Assert.AreEqual(2, newPos);
      Assert.AreEqual("+1  ", mt.Text);

      newPos = mt.InsertText(new RichCaret(2), "2");
      Assert.IsTrue(mt.LastOperationSucceeded);
      Assert.AreEqual(3, newPos);
      Assert.AreEqual("+12 ", mt.Text);

      newPos = mt.InsertText(new RichCaret(3), "3");
      Assert.IsTrue(mt.LastOperationSucceeded);
      Assert.AreEqual(5, newPos);
      Assert.AreEqual("+12 3", mt.Text);
    }

    [Test]
    public void InsertText_OptionalCharacters()
    {
      string sampleText = "+12   (34 ) 567";

      MaskedTextBoxModel mt = new MaskedTextBoxModel();

      mt.Mask = "+0### (0##) 000";
      mt.Text = sampleText;
      Assert.AreEqual(sampleText, mt.Text);

      mt = new MaskedTextBoxModel();
      mt.Mask = "+0aaa (0Ca) 000";
      mt.Text = sampleText;
      Assert.AreEqual(sampleText, mt.Text);

      mt.InsertAtDisplayPos("3X", 3);
      Assert.AreEqual("+123X (34 ) 567", mt.Text);
    }

    [Test]
    public void InsertText_ReplacingSelection()
    {
      MaskedTextBoxModel mt = new MaskedTextBoxModel();
      mt.Mask = "000000LLLL";
      mt.Text = "145";

      RichRange tsp = new RichRange(1, 3);

      int newPos = mt.InsertText(tsp, "2A3");
      Assert.IsFalse(mt.LastOperationSucceeded);
      Assert.AreEqual(MaskedTextResultHint.DigitExpected, mt.LastOperationResult);
      Assert.AreEqual("145", mt.Text);

      tsp = new RichRange(1, 3);
      newPos = mt.InsertText(tsp, "23");
      Assert.IsTrue(mt.LastOperationSucceeded);
      Assert.AreEqual(3, newPos);
      Assert.AreEqual("123", mt.Text);

      tsp = new RichRange(3, 10);
      newPos = mt.InsertText(tsp, "456WXYZ");
      Assert.IsTrue(mt.LastOperationSucceeded);
      Assert.AreEqual(10, newPos);
      Assert.AreEqual("123456WXYZ", mt.Text);
    }

    [Test]
    public void DeleteText_AtCaret()
    {
      MaskedTextBoxModel mt = new MaskedTextBoxModel();
      mt.Mask = "0000LLLL0000";
      mt.Text = "1234AB";

      RichCaret tsp = new RichCaret(1);

      int newPos = mt.DeleteText(tsp, DeleteDirection.Forward);
      Assert.IsFalse(mt.LastOperationSucceeded);
      Assert.AreEqual(MaskedTextResultHint.DigitExpected, mt.LastOperationResult);

      tsp = new RichCaret(5);
      newPos = mt.DeleteText(tsp, DeleteDirection.Backward);
      Assert.IsTrue(mt.LastOperationSucceeded);
      Assert.AreEqual(4, newPos);
      Assert.AreEqual("1234B", mt.Text);

      tsp = new RichCaret(4);
      newPos = mt.DeleteText(tsp, DeleteDirection.Forward);
      Assert.IsTrue(mt.LastOperationSucceeded);
      Assert.AreEqual(4, newPos);
      Assert.AreEqual("1234", mt.Text);
    }

    [Test]
    public void DeleteText_AtCaret_SkipLiterals()
    {
      MaskedTextBoxModel mt = new MaskedTextBoxModel();
      mt.Mask = "(00) 00";
      mt.Text = "(12) 34";

      int newPos;

      newPos = mt.DeleteText(new RichCaret(4), DeleteDirection.Backward);
      Assert.IsFalse(mt.LastOperationSucceeded);

      newPos = mt.DeleteText(new RichCaret(4), DeleteDirection.Forward);
      Assert.IsFalse(mt.LastOperationSucceeded);

      mt.AutoSkipLiterals = true;

      newPos = mt.DeleteText(new RichCaret(4), DeleteDirection.Backward);
      Assert.IsTrue(mt.LastOperationSucceeded);
      Assert.AreEqual(2, newPos);
      Assert.AreEqual("(13) 4", mt.Text);

      newPos = mt.DeleteText(new RichCaret(4), DeleteDirection.Forward);
      Assert.IsTrue(mt.LastOperationSucceeded);
      Assert.AreEqual(5, newPos);
      Assert.AreEqual("(13) ", mt.Text);
    }

    [Test]
    public void DeleteText_DeletingSelection()
    {
      MaskedTextBoxModel mt = new MaskedTextBoxModel();
      mt.Mask = "0000LLLL0000";
      mt.Text = "1234AB";

      RichRange tsp = new RichRange(1, 2);

      int newPos = mt.DeleteText(tsp, DeleteDirection.Forward);
      Assert.IsFalse(mt.LastOperationSucceeded);
      Assert.AreEqual(MaskedTextResultHint.DigitExpected, mt.LastOperationResult);

      tsp = new RichRange(4, 6);
      newPos = mt.DeleteText(tsp, DeleteDirection.Backward);
      Assert.IsTrue(mt.LastOperationSucceeded);
      Assert.AreEqual(4, newPos);
      Assert.AreEqual("1234", mt.Text);

      tsp = new RichRange(1, 3);
      newPos = mt.DeleteText(tsp, DeleteDirection.Forward);
      Assert.IsTrue(mt.LastOperationSucceeded);
      Assert.AreEqual(1, newPos);
      Assert.AreEqual("14", mt.Text);
    }

    [Test]
    public void AllowedReplace()
    {
      string mask = "+00 (00) 000 000";
      MaskedTextBoxModel mt = new MaskedTextBoxModel();
      mt.Mask = mask;
      mt.Text = "+64 (21) 494 279";

      mt.ReplaceAtDisplayPosRange("+99", 0, 2);
      Assert.IsTrue(mt.LastOperationSucceeded);
      Assert.AreEqual("+99 (21) 494 279", mt.Text);

      mt.ReplaceAtDisplayPosRange("+99", 4, 6);
      Assert.IsFalse(mt.LastOperationSucceeded);
      Assert.AreEqual("+99 (21) 494 279", mt.Text);
    }

    [Test]
    public void GetDisplayChars()
    {
      MaskedTextBoxModel mt = new MaskedTextBoxModel();
      mt.Mask = "(000)";
      mt.Text = "(12";

      List<MaskedTextDisplayElement> mcs = new List<MaskedTextDisplayElement>(mt.GetDisplayChars());

      Assert.AreEqual(5, mcs.Count);
      Assert.IsInstanceOf<LiteralDisplayElement>(mcs[0]);
      Assert.IsInstanceOf<InputDisplayElement>(mcs[1]);
      Assert.IsInstanceOf<InputDisplayElement>(mcs[2]);
      Assert.IsInstanceOf<PromptDisplayElement>(mcs[3]);
      Assert.IsInstanceOf<LiteralDisplayElement>(mcs[4]);
    }

    private class TestStyleProvider : IDisplayElementStyleProvider
    {
      public Style InputStyle { get; set; }
      public Style LiteralStyle { get; set; }
      public Style PromptStyle { get; set; }
      public IPromptCharDisplaySelector PromptCharDisplaySelector { get; set; }
    }

    [Test]
    public void CreateDocument()
    {
      MaskedTextBoxModel mt = new MaskedTextBoxModel();
      mt.Mask = "(000)";
      mt.Text = "(12";

      TestStyleProvider styleProvider = new TestStyleProvider();
      styleProvider.InputStyle = new Style();
      styleProvider.LiteralStyle = new Style();
      styleProvider.PromptStyle = null;
      styleProvider.PromptCharDisplaySelector = new DefaultPromptCharDisplaySelector();

      Style[] expectedStyles = new Style[] {
        styleProvider.LiteralStyle,
        styleProvider.InputStyle,
        styleProvider.InputStyle,
        styleProvider.PromptStyle,
        styleProvider.LiteralStyle
      };

      List<MaskedTextDisplayElement> mcs = new List<MaskedTextDisplayElement>(mt.GetDisplayChars());

      FlowDocument fd = mt.CreateDisplayDocument(styleProvider);

      Assert.AreEqual(1, fd.Blocks.Count);
      Assert.IsInstanceOf<Paragraph>(fd.Blocks.FirstBlock);

      Paragraph p = fd.Blocks.FirstBlock as Paragraph;

      Assert.AreEqual(5, p.Inlines.Count);

      int i = 0;
      foreach (Inline inline in p.Inlines)
      {
        Assert.IsInstanceOf<Run>(inline);
        Assert.AreEqual(expectedStyles[i], inline.Style);

        string expectedText = mt.PromptChar.ToString();
        if (mcs[i] is CharacterDisplayElement)
        {
          expectedText = ((CharacterDisplayElement)(mcs[i])).Char.ToString();
        }
        string actualText = ((Run)inline).Text;
        Assert.AreEqual(expectedText, actualText);

        ++i;
      }
    }

    [Test]
    public void ZeroWidthElements()
    {
      MaskedTextBoxModel mt = new MaskedTextBoxModel();
      mt.Mask = "L>L|L<L|L|L";
      mt.Text = "abcD";

      Assert.AreEqual("aBcd", mt.Text);
      Assert.AreEqual("aBcd__", mt.DisplayText);

      List<MaskedTextDisplayElement> mcs = new List<MaskedTextDisplayElement>(mt.GetDisplayChars());

      Assert.AreEqual('a', ((InputDisplayElement)mcs[0]).Char);
      Assert.AreEqual('B', ((InputDisplayElement)mcs[1]).Char);
      Assert.AreEqual('c', ((InputDisplayElement)mcs[2]).Char);
      Assert.AreEqual('d', ((InputDisplayElement)mcs[3]).Char);
      Assert.AreEqual(DisplayElementType.Prompt, mcs[4].ElementType);
      Assert.AreEqual(DisplayElementType.Prompt, mcs[5].ElementType);

      Assert.AreEqual(6, mcs.Count);

      mt.InsertAtDisplayPos("E", 4);
      Assert.AreEqual("aBcdE", mt.Text);
      Assert.AreEqual("aBcdE_", mt.DisplayText);

      mt.RemoveAtDisplayPos(1);
      Assert.AreEqual("aCde", mt.Text);
      Assert.AreEqual("aCde__", mt.DisplayText);
    }

    [Test]
    public void PromptsAreCorrectlyInterpreted()
    {
      MaskedTextBoxModel mt = new MaskedTextBoxModel();
      mt.Mask = "09#L?&CAa";

      List<IPrompt> mcs = new List<MaskedTextDisplayElement>(mt.GetDisplayChars())
        .ConvertAll(delegate(MaskedTextDisplayElement e) { return (IPrompt)e; });

      Assert.AreEqual(9, mcs.Count);

      Assert.AreEqual(ExpectedInputType.Digit, mcs[0].ExpectedInputType);
      Assert.AreEqual(ExpectedInputType.Digit, mcs[1].ExpectedInputType);
      Assert.AreEqual(ExpectedInputType.Digit, mcs[2].ExpectedInputType);
      Assert.AreEqual(ExpectedInputType.Letter, mcs[3].ExpectedInputType);
      Assert.AreEqual(ExpectedInputType.Letter, mcs[4].ExpectedInputType);
      Assert.AreEqual(ExpectedInputType.Any, mcs[5].ExpectedInputType);
      Assert.AreEqual(ExpectedInputType.Any, mcs[6].ExpectedInputType);
      Assert.AreEqual(ExpectedInputType.Alphanumeric, mcs[7].ExpectedInputType);
      Assert.AreEqual(ExpectedInputType.Alphanumeric, mcs[8].ExpectedInputType);

      Assert.IsFalse(mcs[0].IsOptional);
      Assert.IsTrue(mcs[1].IsOptional);
      Assert.IsTrue(mcs[2].IsOptional);
      Assert.IsFalse(mcs[3].IsOptional);
      Assert.IsTrue(mcs[4].IsOptional);
      Assert.IsFalse(mcs[5].IsOptional);
      Assert.IsTrue(mcs[6].IsOptional);
      Assert.IsFalse(mcs[7].IsOptional);
      Assert.IsTrue(mcs[8].IsOptional);
    }

    private class TestPromptCharDisplaySelector : IPromptCharDisplaySelector
    {
      public bool OverridesRepresentation(IPrompt prompt)
      {
        return (prompt.ExpectedInputType != ExpectedInputType.Any);
      }

      public Inline CreateRepresentation(IPrompt prompt)
      {
        switch (prompt.ExpectedInputType)
        {
          case ExpectedInputType.Alphanumeric:
            return new Run(prompt.DisplayContext.PromptChar.ToString());
          case ExpectedInputType.Digit:
            return new Run("D");
          case ExpectedInputType.Letter:
            return new InlineUIContainer(new Border());
          default:
            throw new InvalidOperationException();
        }
      }
    }

    [Test]
    [STAThread]
    public void PromptRepresentationsCanBeOverridden()
    {
      MaskedTextBoxModel mt = new MaskedTextBoxModel();
      mt.Mask = "09#L?&CAa";

      TestStyleProvider styleProvider = new TestStyleProvider();
      styleProvider.PromptCharDisplaySelector = new TestPromptCharDisplaySelector();

      FlowDocument document = mt.CreateDisplayDocument(styleProvider);

      List<Inline> inlines = new List<Inline>((((Paragraph)(document.Blocks.FirstBlock)).Inlines));

      AssertTextRun("D", inlines[0]);
      AssertTextRun("D", inlines[1]);
      AssertTextRun("D", inlines[2]);
      AssertUIElement(typeof(Border), inlines[3]);
      AssertUIElement(typeof(Border), inlines[4]);
      AssertTextRun("_", inlines[5]);
      AssertTextRun("_", inlines[6]);
      AssertTextRun("_", inlines[7]);
      AssertTextRun("_", inlines[8]);
    }

    private static void AssertTextRun(string expectedText, Inline actualInline)
    {
      Assert.IsInstanceOf<Run>(actualInline);
      Assert.AreEqual(expectedText, ((Run)actualInline).Text);
    }

    private static void AssertUIElement(Type expectedElementType, Inline actualInline)
    {
      Assert.IsInstanceOf<InlineUIContainer>(actualInline);
      Assert.IsInstanceOf(expectedElementType, ((InlineUIContainer)actualInline).Child);
    }

    private class BadPromptCharDisplaySelector : IPromptCharDisplaySelector
    {
      public bool OverridesRepresentation(IPrompt prompt)
      {
        return true;
      }

      public Inline CreateRepresentation(IPrompt prompt)
      {
        switch (prompt.ExpectedInputType)
        {
          case ExpectedInputType.Letter:
            return new Run("[abc]");
          case ExpectedInputType.Digit:
            return new Bold(new Italic(new Run("[123]")));
          case ExpectedInputType.Alphanumeric:
            return new LineBreak();
          default:
            Span span = new Span();
            span.Inlines.Add(new InlineUIContainer(new Border()));
            span.Inlines.Add(new InlineUIContainer(new Border()));
            return span;
        }
      }
    }

    [Test]
    [ExpectedException(typeof(MaskedTextDisplayException))]
    public void BadPromptRepresentationsAreDetected_MultiCharString()
    {
      MaskedTextBoxModel mt = new MaskedTextBoxModel();
      mt.Mask = "L";
      TestStyleProvider styleProvider = new TestStyleProvider();
      styleProvider.PromptCharDisplaySelector = new BadPromptCharDisplaySelector();
      mt.CreateDisplayDocument(styleProvider);
    }

    [Test]
    [ExpectedException(typeof(MaskedTextDisplayException))]
    public void BadPromptRepresentationsAreDetected_MultiCharString_Nested()
    {
      MaskedTextBoxModel mt = new MaskedTextBoxModel();
      mt.Mask = "0";
      TestStyleProvider styleProvider = new TestStyleProvider();
      styleProvider.PromptCharDisplaySelector = new BadPromptCharDisplaySelector();
      mt.CreateDisplayDocument(styleProvider);
    }

    [Test]
    [ExpectedException(typeof(MaskedTextDisplayException))]
    public void BadPromptRepresentationsAreDetected_InvalidInlineType()
    {
      MaskedTextBoxModel mt = new MaskedTextBoxModel();
      mt.Mask = "A";
      TestStyleProvider styleProvider = new TestStyleProvider();
      styleProvider.PromptCharDisplaySelector = new BadPromptCharDisplaySelector();
      mt.CreateDisplayDocument(styleProvider);
    }

    [Test]
    [STAThread]
    [ExpectedException(typeof(MaskedTextDisplayException))]
    public void BadPromptRepresentationsAreDetected_MultipleUIElements()
    {
      MaskedTextBoxModel mt = new MaskedTextBoxModel();
      mt.Mask = "&";
      TestStyleProvider styleProvider = new TestStyleProvider();
      styleProvider.PromptCharDisplaySelector = new BadPromptCharDisplaySelector();
      mt.CreateDisplayDocument(styleProvider);
    }

    [Test]
    public void MulticharacterPromptSubstitutions()
    {
      MaskedTextBoxModel mt = new MaskedTextBoxModel();

      // Switzerland has multi-character currency symbol "SFr."

      mt.Culture = new CultureInfo("fr-CH");
      mt.Mask = "$00.00";
      Assert.AreEqual("fr.  .", mt.Text);
      mt.InsertAtDisplayPos("12", 3);
      Assert.AreEqual("fr.12.", mt.Text);

      List<MaskedTextDisplayElement> mcs = new List<MaskedTextDisplayElement>(mt.GetDisplayChars());
      Assert.AreEqual(8, mcs.Count);
      Assert.AreEqual(DisplayElementType.Literal, mcs[0].ElementType);
      Assert.AreEqual(DisplayElementType.Literal, mcs[1].ElementType);
      Assert.AreEqual(DisplayElementType.Literal, mcs[2].ElementType);
      Assert.AreEqual(DisplayElementType.Input, mcs[3].ElementType);
      Assert.AreEqual(DisplayElementType.Input, mcs[4].ElementType);
      Assert.AreEqual(DisplayElementType.Literal, mcs[5].ElementType);
      Assert.AreEqual(DisplayElementType.Prompt, mcs[6].ElementType);
      Assert.AreEqual(DisplayElementType.Prompt, mcs[7].ElementType);

      // Slovakia has multi-character date separator ". "

      mt.Culture = new CultureInfo("sk-SK");
      mt.Mask = "00/00";
      Assert.AreEqual("  . ", mt.Text);
      mt.InsertAtDisplayPos("12", 0);
      Assert.AreEqual("12. ", mt.Text);
      mt.InsertAtDisplayPos("01", 4);
      Assert.AreEqual("12. 01", mt.Text);

      mcs = new List<MaskedTextDisplayElement>(mt.GetDisplayChars());
      Assert.AreEqual(6, mcs.Count);
      Assert.AreEqual(DisplayElementType.Input, mcs[0].ElementType);
      Assert.AreEqual(DisplayElementType.Input, mcs[1].ElementType);
      Assert.AreEqual(DisplayElementType.Literal, mcs[2].ElementType);
      Assert.AreEqual(DisplayElementType.Literal, mcs[3].ElementType);
      Assert.AreEqual(DisplayElementType.Input, mcs[4].ElementType);
      Assert.AreEqual(DisplayElementType.Input, mcs[5].ElementType);
    }

    [Test]
    public void MapDisplayIndexToMaskIndex()
    {
      MaskedTextBoxModel mt = new MaskedTextBoxModel();
      int[] map;

      mt.Mask = @"00";
      map = mt.CreateDisplayCharScanner().CalculateDisplayIndexToMaskIndexMap();
      Assert.AreEqual(2, map.Length);
      Assert.AreEqual(0, map[0]);
      Assert.AreEqual(1, map[1]);

      mt.Mask = @"0<0";
      map = mt.CreateDisplayCharScanner().CalculateDisplayIndexToMaskIndexMap();
      Assert.AreEqual(2, map.Length);
      Assert.AreEqual(0, map[0]);
      Assert.AreEqual(2, map[1]);

      mt.Mask = @"0<><>|0";
      map = mt.CreateDisplayCharScanner().CalculateDisplayIndexToMaskIndexMap();
      Assert.AreEqual(2, map.Length);
      Assert.AreEqual(0, map[0]);
      Assert.AreEqual(6, map[1]);

      mt.Mask = @"0\00";
      map = mt.CreateDisplayCharScanner().CalculateDisplayIndexToMaskIndexMap();
      Assert.AreEqual(3, map.Length);
      Assert.AreEqual(0, map[0]);
      Assert.AreEqual(2, map[1]);
      Assert.AreEqual(3, map[2]);

      mt.Mask = @"0\\\<0";
      map = mt.CreateDisplayCharScanner().CalculateDisplayIndexToMaskIndexMap();
      Assert.AreEqual(4, map.Length);
      Assert.AreEqual(0, map[0]);
      Assert.AreEqual(2, map[1]);
      Assert.AreEqual(4, map[2]);
      Assert.AreEqual(5, map[3]);

      mt.Culture = new CultureInfo("fr-CH");  // Switzerland: multi-char currency symbol "SFr."

      mt.Mask = @"$0,0.0";
      map = mt.CreateDisplayCharScanner().CalculateDisplayIndexToMaskIndexMap();
      Assert.AreEqual(8, map.Length);
      Assert.AreEqual(0, map[0]);
      Assert.AreEqual(0, map[1]);
      Assert.AreEqual(0, map[2]);
      Assert.AreEqual(1, map[3]);
      Assert.AreEqual(2, map[4]);
      Assert.AreEqual(3, map[5]);
      Assert.AreEqual(4, map[6]);
      Assert.AreEqual(5, map[7]);

      mt.Culture = new CultureInfo("sk-SK");  // Slovakia: multi-char date separator ". "

      mt.Mask = @"0/0:0";
      map = mt.CreateDisplayCharScanner().CalculateDisplayIndexToMaskIndexMap();
      Assert.AreEqual(6, map.Length);
      Assert.AreEqual(0, map[0]);
      Assert.AreEqual(1, map[1]);
      Assert.AreEqual(1, map[2]);
      Assert.AreEqual(2, map[3]);
      Assert.AreEqual(3, map[4]);
      Assert.AreEqual(4, map[5]);
    }
  }
}
