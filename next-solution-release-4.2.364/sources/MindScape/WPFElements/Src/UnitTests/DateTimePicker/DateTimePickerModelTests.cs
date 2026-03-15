using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using System.Globalization;
using System.ComponentModel;

namespace Mindscape.WpfElements.UnitTests
{
  [TestFixture]
  public class DateTimePickerModelTests
  {
    [Test]
    public void FormatString()
    {
      DateTimePickerModel model = new DateTimePickerModel();
      model.CustomFormat = "hh:mm on d, M yyyy";

      model.Culture = TestCultures.EnglishNZ;

      model.Format = DateTimePickerFormat.ShortDate;
      Assert.AreEqual("d/MM/yyyy", model.GetFormatString());

      model.Format = DateTimePickerFormat.LongDate;
      Assert.AreEqual("dddd, d MMMM yyyy", model.GetFormatString());

      model.Format = DateTimePickerFormat.ShortTime;
      Assert.AreEqual("h:mm tt", model.GetFormatString());

      model.Format = DateTimePickerFormat.LongTime;
      Assert.AreEqual("h:mm:ss tt", model.GetFormatString());

      model.Format = DateTimePickerFormat.Custom;
      Assert.AreEqual("hh:mm on d, M yyyy", model.GetFormatString());

      model.Culture = TestCultures.QuechuaEcuador;

      model.Format = DateTimePickerFormat.LongDate;
      Assert.AreEqual("dddd, dd' de 'MMMM' de 'yyyy", model.GetFormatString());
    }

    [Test]
    public void Tokeniser()
    {
      DateTimePickerModel model = new DateTimePickerModel();
      List<Token> tokens;

      model.Culture = TestCultures.EnglishNZ;
      model.Format = DateTimePickerFormat.ShortDate;

      tokens = new List<Token>(model.GetFormatStringTokens());

      Assert.AreEqual(5, tokens.Count);

      Assert.AreEqual("d", tokens[0].FormatString);
      Assert.AreEqual("/", tokens[1].FormatString);
      Assert.AreEqual("MM", tokens[2].FormatString);
      Assert.AreEqual("/", tokens[3].FormatString);
      Assert.AreEqual("yyyy", tokens[4].FormatString);

      Assert.AreEqual("d", tokens[0].OriginalText);
      Assert.AreEqual("/", tokens[1].OriginalText);
      Assert.AreEqual("MM", tokens[2].OriginalText);
      Assert.AreEqual("/", tokens[3].OriginalText);
      Assert.AreEqual("yyyy", tokens[4].OriginalText);

      model.Culture = TestCultures.QuechuaEcuador;
      model.Format = DateTimePickerFormat.LongDate;

      tokens = new List<Token>(model.GetFormatStringTokens());

      Assert.AreEqual(7, tokens.Count);

      Assert.AreEqual("dddd", tokens[0].FormatString);
      Assert.AreEqual(", ", tokens[1].FormatString);
      Assert.AreEqual("dd", tokens[2].FormatString);
      Assert.AreEqual(" de ", tokens[3].FormatString);
      Assert.AreEqual("MMMM", tokens[4].FormatString);
      Assert.AreEqual(" de ", tokens[5].FormatString);
      Assert.AreEqual("yyyy", tokens[6].FormatString);

      Assert.AreEqual("dddd", tokens[0].OriginalText);
      Assert.AreEqual(", ", tokens[1].OriginalText);
      Assert.AreEqual("dd", tokens[2].OriginalText);
      Assert.AreEqual("' de '", tokens[3].OriginalText);
      Assert.AreEqual("MMMM", tokens[4].OriginalText);
      Assert.AreEqual("' de '", tokens[5].OriginalText);
      Assert.AreEqual("yyyy", tokens[6].OriginalText);

      Assert.IsFalse(tokens[2].ForceLiteral);
      Assert.IsTrue(tokens[3].ForceLiteral);
      Assert.IsTrue(tokens[5].ForceLiteral);

      model.Format = DateTimePickerFormat.Custom;
      model.CustomFormat = "d\" of \"MM";

      tokens = new List<Token>(model.GetFormatStringTokens());

      Assert.AreEqual(3, tokens.Count);

      Assert.AreEqual("d", tokens[0].FormatString);
      Assert.AreEqual(" of ", tokens[1].FormatString);
      Assert.AreEqual("MM", tokens[2].FormatString);

      Assert.AreEqual("d", tokens[0].OriginalText);
      Assert.AreEqual("\" of \"", tokens[1].OriginalText);
      Assert.AreEqual("MM", tokens[2].OriginalText);

      model.Format = DateTimePickerFormat.Custom;
      model.CustomFormat = @"d\da\y MM\mon\t\h";

      tokens = new List<Token>(model.GetFormatStringTokens());

      Assert.AreEqual(4, tokens.Count);

      Assert.AreEqual("d", tokens[0].FormatString);
      Assert.AreEqual("day ", tokens[1].FormatString);
      Assert.AreEqual("MM", tokens[2].FormatString);
      Assert.AreEqual("month", tokens[3].FormatString);

      Assert.AreEqual("d", tokens[0].OriginalText);
      Assert.AreEqual(@"\da\y ", tokens[1].OriginalText);
      Assert.AreEqual("MM", tokens[2].OriginalText);
      Assert.AreEqual(@"\mon\t\h", tokens[3].OriginalText);
    }

    [Test]
    public void Tokeniser_InitialQuoteHandledCorrectly()
    {
      DateTimePickerModel model = new DateTimePickerModel();
      model.Culture = new CultureInfo("sv-FI", false);
      model.Format = DateTimePickerFormat.LongDate;

      Assert.AreEqual("'den 'd MMMM yyyy", model.GetFormatString());

      List<Token> tokens = new List<Token>(model.GetFormatStringTokens());

      Assert.AreEqual(6, tokens.Count);
      Assert.AreEqual("'den '", tokens[0].OriginalText);
      Assert.AreEqual("d", tokens[1].OriginalText);
      Assert.AreEqual(" ", tokens[2].OriginalText);
      Assert.AreEqual("MMMM", tokens[3].OriginalText);
      Assert.AreEqual(" ", tokens[4].OriginalText);
      Assert.AreEqual("yyyy", tokens[5].OriginalText);
    }

    [Test]
    public void Tokeniser_TerminalQuoteHandledCorrectly()
    {
      DateTimePickerModel model = new DateTimePickerModel();
      model.Culture = TestCultures.EnglishNZ;
      model.Format = DateTimePickerFormat.Custom;
      model.CustomFormat = "'the 'yyyy\"th year\"";

      List<Token> tokens = new List<Token>(model.GetFormatStringTokens());

      Assert.AreEqual(3, tokens.Count);
      Assert.AreEqual("'the '", tokens[0].OriginalText);
      Assert.AreEqual("the ", tokens[0].FormatString);
      Assert.AreEqual("yyyy", tokens[1].OriginalText);
      Assert.AreEqual("\"th year\"", tokens[2].OriginalText);
      Assert.AreEqual("th year", tokens[2].FormatString);
    }

    [Test]
    public void Formatting()
    {
      DateTimePickerModel model = new DateTimePickerModel();
      model.Culture = TestCultures.FrenchFrance;
      model.Format = DateTimePickerFormat.Custom;
      model.CustomFormat = "HH:mm:ss dd MM yyyy gg";

      model.Value = new DateTime(2008, 3, 4, 15, 6, 7);

      Assert.AreEqual(13, model.Text.Count);
      Assert.AreEqual("15", model.Text[0].TextCore);
      Assert.AreEqual(":", model.Text[1].TextCore);
      Assert.AreEqual("06", model.Text[2].TextCore);
      Assert.AreEqual(":", model.Text[3].TextCore);
      Assert.AreEqual("07", model.Text[4].TextCore);
      Assert.AreEqual(" ", model.Text[5].TextCore);
      Assert.AreEqual("04", model.Text[6].TextCore);
      Assert.AreEqual(" ", model.Text[7].TextCore);
      Assert.AreEqual("03", model.Text[8].TextCore);
      Assert.AreEqual(" ", model.Text[9].TextCore);
      Assert.AreEqual("2008", model.Text[10].TextCore);
      Assert.AreEqual(" ", model.Text[11].TextCore);
      Assert.AreEqual("ap. J.-C.", model.Text[12].TextCore);

      model.Culture = TestCultures.EnglishNZ;
      model.CustomFormat = "h:mmm:s tt dddd MMM yy g";

      Assert.AreEqual(15, model.Text.Count);
      Assert.AreEqual("3", model.Text[0].TextCore);
      Assert.AreEqual(":", model.Text[1].TextCore);
      Assert.AreEqual("06", model.Text[2].TextCore);
      Assert.AreEqual(":", model.Text[3].TextCore);
      Assert.AreEqual("7", model.Text[4].TextCore);
      Assert.AreEqual(" ", model.Text[5].TextCore);
      Assert.AreEqual("p.m.", model.Text[6].TextCore);
      Assert.AreEqual(" ", model.Text[7].TextCore);
      Assert.AreEqual("Tuesday", model.Text[8].TextCore);
      Assert.AreEqual(" ", model.Text[9].TextCore);
      Assert.AreEqual("Mar", model.Text[10].TextCore);
      Assert.AreEqual(" ", model.Text[11].TextCore);
      Assert.AreEqual("08", model.Text[12].TextCore);
      Assert.AreEqual(" ", model.Text[13].TextCore);
      Assert.AreEqual("A.D.", model.Text[14].TextCore);
    }
  }
}
