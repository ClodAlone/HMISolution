using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using System.Globalization;

namespace Mindscape.WpfElements.UnitTests
{
  [TestFixture]
  public class DateTimeDisplayElementTests
  {
    [Test]
    public void GetDisplayElements()
    {
      DateTimePickerModel model = new DateTimePickerModel();
      model.Culture = TestCultures.EnglishNZ;
      model.Format = DateTimePickerFormat.ShortDate;
      model.Value = new DateTime(2008, 3, 7, 3, 4, 5);

      List<DateTimeDisplayElement> elements = new List<DateTimeDisplayElement>(model.GetDisplayElements());

      Assert.AreEqual(5, elements.Count);

      Assert.AreEqual("7", elements[0].TextCore);
      Assert.AreEqual("/", elements[1].TextCore);
      Assert.AreEqual("03", elements[2].TextCore);
      Assert.AreEqual("/", elements[3].TextCore);
      Assert.AreEqual("2008", elements[4].TextCore);

      Assert.AreEqual(DateTimeDisplayElementType.Numeric, elements[0].ElementType);
      Assert.AreEqual(DateTimeDisplayElementType.ReadOnly, elements[1].ElementType);
      Assert.AreEqual(DateTimeDisplayElementType.Numeric, elements[2].ElementType);
      Assert.AreEqual(DateTimeDisplayElementType.ReadOnly, elements[3].ElementType);
      Assert.AreEqual(DateTimeDisplayElementType.Numeric, elements[4].ElementType);

      Assert.AreEqual("d", elements[0].FormatStringFragment);
      Assert.AreEqual("/", elements[1].FormatStringFragment);
      Assert.AreEqual("MM", elements[2].FormatStringFragment);
      Assert.AreEqual("/", elements[3].FormatStringFragment);
      Assert.AreEqual("yyyy", elements[4].FormatStringFragment);

      Assert.AreEqual(7, ((DateTimeNumericDisplayElement)(elements[0])).Value);

      model.Culture = TestCultures.QuechuaEcuador;
      model.Format = DateTimePickerFormat.LongDate;

      elements = new List<DateTimeDisplayElement>(model.GetDisplayElements());

      Assert.AreEqual(7, elements.Count);

      // Don't tell me this isn't highly educational...
      Assert.AreEqual("Illapachaw", elements[0].TextCore);
      Assert.AreEqual(", ", elements[1].TextCore);
      Assert.AreEqual("07", elements[2].TextCore);
      Assert.AreEqual(" de ", elements[3].TextCore);
      Assert.AreEqual("Pauqar waray", elements[4].TextCore);
      Assert.AreEqual(" de ", elements[5].TextCore);
      Assert.AreEqual("2008", elements[6].TextCore);

      Assert.AreEqual(DateTimeDisplayElementType.ReadOnly, elements[0].ElementType);
      Assert.AreEqual(DateTimeDisplayElementType.ReadOnly, elements[1].ElementType);
      Assert.AreEqual(DateTimeDisplayElementType.Numeric, elements[2].ElementType);
      Assert.AreEqual(DateTimeDisplayElementType.ReadOnly, elements[3].ElementType);
      Assert.AreEqual(DateTimeDisplayElementType.Select, elements[4].ElementType);
      Assert.AreEqual(DateTimeDisplayElementType.ReadOnly, elements[5].ElementType);
      Assert.AreEqual(DateTimeDisplayElementType.Numeric, elements[6].ElementType);

      Assert.AreEqual(7, ((DateTimeNumericDisplayElement)(elements[2])).Value);
      Assert.AreEqual(2008, ((DateTimeNumericDisplayElement)(elements[6])).Value);
    }

    [Test]
    public void DecomposeAndRecompose_Simple()
    {
      DateTime sample = new DateTime(1900, 1, 2);

      DateTimePickerModel model = new DateTimePickerModel();
      model.Culture = TestCultures.EnglishNZ;
      model.Format = DateTimePickerFormat.ShortDate;
      model.Value = sample;

      IEnumerable<DateTimeDisplayElement> elements = model.GetDisplayElements();

      DateTime dt = Composer.Compose(elements, sample, model.Culture);
      Assert.AreEqual(sample, dt);
    }

    [Test]
    public void DecomposeAndRecompose_TimeIsPreservedWhenEditingOnlyDate()
    {
      DateTime sample = new DateTime(1900, 1, 2, 13, 14, 15);

      DateTimePickerModel model = new DateTimePickerModel();
      model.Culture = TestCultures.EnglishNZ;
      model.Format = DateTimePickerFormat.ShortDate;
      model.Value = sample;

      IEnumerable<DateTimeDisplayElement> elements = model.GetDisplayElements();

      DateTime dt = Composer.Compose(elements, sample, model.Culture);
      Assert.AreEqual(sample, dt);
    }

    [Test]
    public void DecomposeAndRecompose_DateIsPreservedWhenEditingOnlyTime()
    {
      DateTime sample = new DateTime(1900, 1, 2, 13, 14, 15);

      DateTimePickerModel model = new DateTimePickerModel();
      model.Culture = TestCultures.EnglishNZ;
      model.Format = DateTimePickerFormat.ShortTime;
      model.Value = sample;

      IEnumerable<DateTimeDisplayElement> elements = model.GetDisplayElements();

      DateTime dt = Composer.Compose(elements, sample, model.Culture);
      Assert.AreEqual(sample, dt);
    }

    [Test]
    public void DecomposeAndRecompose_CustomFormat()
    {
      DateTime sample = new DateTime(1900, 1, 2, 3, 4, 5);

      DateTimePickerModel model = new DateTimePickerModel();
      model.Culture = TestCultures.EnglishNZ;
      model.Format = DateTimePickerFormat.Custom;
      model.CustomFormat = "hh:mm:ss' on 'dddd, \\t\\he dd\\t\\h\" day of \"MMM yyyy";
      model.Value = sample;

      IEnumerable<DateTimeDisplayElement> elements = model.Text;

      DateTime dt = Composer.Compose(elements, sample, model.Culture);
      Assert.AreEqual(sample, dt);
    }

    [Test]
    public void DecomposeAndRecompose_TwoDigitYear()
    {
      DateTime sample = new DateTime(2012, 1, 2, 3, 4, 5);

      DateTimePickerModel model = new DateTimePickerModel();
      model.Culture = TestCultures.EnglishNZ;
      model.Format = DateTimePickerFormat.Custom;
      model.CustomFormat = "dd/MM/yy hh:mm:ss";
      model.Value = sample;

      IEnumerable<DateTimeDisplayElement> elements = model.Text;

      DateTime dt = Composer.Compose(elements, sample, model.Culture);
      Assert.AreEqual(sample, dt);
    }

    [Test]
    public void DecomposeAndRecompose_SingleCharacterTokens()
    {
      DateTime sample = new DateTime(2012, 1, 2, 3, 4, 5);

      DateTimePickerModel model = new DateTimePickerModel();
      model.Culture = TestCultures.EnglishNZ;
      model.Format = DateTimePickerFormat.Custom;
      model.CustomFormat = "d/M/y";
      model.Value = sample;

      IEnumerable<DateTimeDisplayElement> elements = model.Text;

      DateTime dt = Composer.Compose(elements, sample, model.Culture);
      Assert.AreEqual(sample, dt);
    }

    [Test]
    public void ModifyingDisplayElementsModifiesValue_Numeric()
    {
      DateTime sample = new DateTime(2008, 3, 11);

      DateTimePickerModel model = new DateTimePickerModel();
      model.Culture = TestCultures.EnglishNZ;
      model.Format = DateTimePickerFormat.ShortDate;
      model.Value = sample;

      Assert.AreEqual(sample, model.Value);

      model.Text[0].TextCore = "7";

      Assert.AreEqual(new DateTime(2008, 3, 7), model.Value);

      ((DateTimeNumericDisplayElement)(model.Text[0])).Value = 8;

      Assert.AreEqual(new DateTime(2008, 3, 8), model.Value);
    }

    [Test]
    public void ModifyingDisplayElementsModifiesValue_Select()
    {
      DateTime sample = new DateTime(2008, 3, 11);

      DateTimePickerModel model = new DateTimePickerModel();
      model.Culture = TestCultures.EnglishNZ;
      model.Format = DateTimePickerFormat.LongDate;
      model.Value = sample;

      Assert.AreEqual(sample, model.Value);

      model.Text[4].TextCore = "August";

      Assert.AreEqual(new DateTime(2008, 8, 11), model.Value);

      ((DateTimeSelectDisplayElement)(model.Text[4])).Text = "November";

      Assert.AreEqual(new DateTime(2008, 11, 11), model.Value);
    }

    public void ModifyingDisplayElementsModifiesValue_PropagatingChanges()
    {
      DateTime sample = new DateTime(2008, 3, 11);

      DateTimePickerModel model = new DateTimePickerModel();
      model.Culture = TestCultures.EnglishNZ;
      model.Format = DateTimePickerFormat.LongDate;
      model.Value = sample;

      Assert.AreEqual(sample, model.Value);
      Assert.AreEqual("Tuesday", model.Text[0].TextCore);

      model.Text[2].TextCore = "7";

      Assert.AreEqual("Friday", model.Text[0].TextCore);
      Assert.AreEqual(new DateTime(2008, 3, 7), model.Value);
    }

    public void ModifyingDisplayElementsModifiesValue_CustomFormat()
    {
      DateTime sample = new DateTime(2008, 3, 11);

      DateTimePickerModel model = new DateTimePickerModel();
      model.Culture = TestCultures.EnglishNZ;
      model.Format = DateTimePickerFormat.Custom;
      model.CustomFormat = "d' of 'M, yyyy";
      model.Value = sample;

      Assert.AreEqual(sample, model.Value);

      model.Text[0].TextCore = "1";
      model.Text[2].TextCore = "4";
      model.Text[4].TextCore = "2012";

      Assert.AreEqual(new DateTime(2012, 4, 1), model.Value);
    }

    public void ModifyingDisplayElementsModifiesValue_TwoDigitYears()
    {
      DateTime sample = new DateTime(2008, 3, 11);

      DateTimePickerModel model = new DateTimePickerModel();
      model.Culture = TestCultures.EnglishNZ;
      model.Format = DateTimePickerFormat.Custom;
      model.CustomFormat = "dd/MM/yy";
      model.Value = sample;

      Assert.AreEqual(sample, model.Value);

      model.Text[0].TextCore = "01";
      model.Text[2].TextCore = "4";
      model.Text[4].TextCore = "12";

      Assert.AreEqual(new DateTime(2012, 4, 1), model.Value);
    }

    [Test]
    public void ModifyingDisplayElementModifiesValue_LeadingZeroesAreInferred()
    {
      DateTime sample = new DateTime(2008, 3, 11);

      DateTimePickerModel model = new DateTimePickerModel();
      model.Culture = TestCultures.EnglishNZ;
      model.Format = DateTimePickerFormat.ShortDate;
      model.Value = sample;

      Assert.AreEqual(sample, model.Value);

      model.Text[2].TextCore = "4";

      Assert.AreEqual(new DateTime(2008, 4, 11), model.Value);
    }

    [Test]
    public void RaisesValueChangedEvent()
    {
      DateTime sample = new DateTime(2008, 3, 11);

      DateTimePickerModel model = new DateTimePickerModel();
      model.Culture = TestCultures.EnglishNZ;
      model.Format = DateTimePickerFormat.ShortDate;
      model.Value = sample;

      int changeCount = 0;

      model.ValueChanged += delegate(object sender, EventArgs e)
      {
        ++changeCount;
      };

      model.Text[0].TextCore = "7";
      Assert.AreEqual(1, changeCount);

      model.Text[2].TextCore = "03";
      Assert.AreEqual(1, changeCount);  // no event if no change

      model.Value = new DateTime(1900, 1, 2, 3, 4, 5);
      Assert.AreEqual(2, changeCount);

      model.Value = new DateTime(1900, 1, 2, 3, 4, 5);
      Assert.AreEqual(2, changeCount);  // no event if no change
    
      model.Format = DateTimePickerFormat.LongDate;
      model.Value = sample;
      changeCount = 0;

      model.Text[4].TextCore = "August";
      Assert.AreEqual(1, changeCount);

      model.Text[4].TextCore = "August";
      Assert.AreEqual(1, changeCount);  // no event if no change
    }

    [Test]
    public void PermittedValues()
    {
      Token token = new Token("MMM", false);
      DateTimeSelectDisplayElement element = 
        DateTimeDisplayElement.Create(token, TestCultures.EnglishNZ) as DateTimeSelectDisplayElement;
      element.Bind(new DateTime(1900, 1, 2, 3, 4, 5));

      Assert.AreEqual("Jan", element.Text);
      Assert.AreEqual(12, new List<string>(element.PermittedValues).Count);
    }

    [Test]
    public void RebindingSelectElementPreservesText()
    {
      Token token = new Token("MMM", false);
      DateTimeSelectDisplayElement element = 
        DateTimeDisplayElement.Create(token, TestCultures.EnglishNZ) as DateTimeSelectDisplayElement;

      element.Bind(new DateTime(1900, 5, 4, 3, 2, 1));
      Assert.AreEqual("May", element.Text);

      element.Bind(new DateTime(1900, 6, 4, 3, 2, 1));
      Assert.AreEqual("Jun", element.Text);
    }

    [Test]
    public void NumericElementValue()
    {
      Token token = new Token("d", false);
      DateTimeNumericDisplayElement element = 
        DateTimeDisplayElement.Create(token, TestCultures.EnglishNZ) as DateTimeNumericDisplayElement;
      element.Bind(new DateTime(1900, 5, 4, 3, 2, 1));

      Assert.AreEqual("4", element.TextCore);
      Assert.AreEqual(4, element.Value);

      element.Value = 7;

      Assert.AreEqual("7", element.TextCore);
    }

    [Test]
    public void NumericElementRange()
    {
      Token token = new Token("d", false);
      DateTimeNumericDisplayElement element =
        DateTimeDisplayElement.Create(token, TestCultures.EnglishNZ) as DateTimeNumericDisplayElement;
      
      element.Bind(new DateTime(1900, 5, 4, 3, 2, 1));
      Assert.AreEqual(1, element.Minimum);
      Assert.AreEqual(31, element.Maximum);

      element.Bind(new DateTime(1900, 2, 4, 3, 2, 1));
      Assert.AreEqual(1, element.Minimum);
      Assert.AreEqual(28, element.Maximum);

      element.Bind(new DateTime(1904, 2, 4, 3, 2, 1));
      Assert.AreEqual(1, element.Minimum);
      Assert.AreEqual(29, element.Maximum);
    }

    [Test]
    public void ParseableText()
    {
      Token token;
      DateTimeNumericDisplayElement element;

      token = new Token("d", false);
      element = DateTimeDisplayElement.Create(token, TestCultures.EnglishNZ) as DateTimeNumericDisplayElement;
      element.Bind(new DateTime(2005, 5, 14, 3, 2, 1));
      Assert.AreEqual("14", element.ParseableText);

      token = new Token("d", false);
      element = DateTimeDisplayElement.Create(token, TestCultures.EnglishNZ) as DateTimeNumericDisplayElement;
      element.Bind(new DateTime(2005, 5, 4, 3, 2, 1));
      Assert.AreEqual("4", element.ParseableText);

      token = new Token("dd", false);
      element = DateTimeDisplayElement.Create(token, TestCultures.EnglishNZ) as DateTimeNumericDisplayElement;
      element.Bind(new DateTime(2005, 5, 14, 3, 2, 1));
      Assert.AreEqual("14", element.ParseableText);

      token = new Token("dd", false);
      element = DateTimeDisplayElement.Create(token, TestCultures.EnglishNZ) as DateTimeNumericDisplayElement;
      element.Bind(new DateTime(2005, 5, 4, 3, 2, 1));
      Assert.AreEqual("04", element.ParseableText);

      token = new Token("y", false);
      element = DateTimeDisplayElement.Create(token, TestCultures.EnglishNZ) as DateTimeNumericDisplayElement;
      element.Bind(new DateTime(2005, 5, 4, 3, 2, 1));
      Assert.AreEqual("5", element.ParseableText);

      token = new Token("yy", false);
      element = DateTimeDisplayElement.Create(token, TestCultures.EnglishNZ) as DateTimeNumericDisplayElement;
      element.Bind(new DateTime(2005, 5, 4, 3, 2, 1));
      Assert.AreEqual("05", element.ParseableText);

      token = new Token("yyy", false);
      element = DateTimeDisplayElement.Create(token, TestCultures.EnglishNZ) as DateTimeNumericDisplayElement;
      element.Bind(new DateTime(2005, 5, 4, 3, 2, 1));
      Assert.AreEqual("2005", element.ParseableText);

      token = new Token("yyyy", false);
      element = DateTimeDisplayElement.Create(token, TestCultures.EnglishNZ) as DateTimeNumericDisplayElement;
      element.Bind(new DateTime(2005, 5, 4, 3, 2, 1));
      Assert.AreEqual("2005", element.ParseableText);

      token = new Token("yyyyy", false);
      element = DateTimeDisplayElement.Create(token, TestCultures.EnglishNZ) as DateTimeNumericDisplayElement;
      element.Bind(new DateTime(2005, 5, 4, 3, 2, 1));
      Assert.AreEqual("02005", element.ParseableText);

      token = new Token("h", false);
      element = DateTimeDisplayElement.Create(token, TestCultures.EnglishNZ) as DateTimeNumericDisplayElement;
      element.Bind(new DateTime(2005, 5, 4, 3, 2, 1));
      Assert.AreEqual("3", element.ParseableText);

      token = new Token("hh", false);
      element = DateTimeDisplayElement.Create(token, TestCultures.EnglishNZ) as DateTimeNumericDisplayElement;
      element.Bind(new DateTime(2005, 5, 4, 3, 2, 1));
      Assert.AreEqual("03", element.ParseableText);

      token = new Token("hhh", false);
      element = DateTimeDisplayElement.Create(token, TestCultures.EnglishNZ) as DateTimeNumericDisplayElement;
      element.Bind(new DateTime(2005, 5, 4, 3, 2, 1));
      Assert.AreEqual("03", element.ParseableText);

      token = new Token("H", false);
      element = DateTimeDisplayElement.Create(token, TestCultures.EnglishNZ) as DateTimeNumericDisplayElement;
      element.Bind(new DateTime(2005, 5, 4, 3, 2, 1));
      Assert.AreEqual("3", element.ParseableText);

      token = new Token("HH", false);
      element = DateTimeDisplayElement.Create(token, TestCultures.EnglishNZ) as DateTimeNumericDisplayElement;
      element.Bind(new DateTime(2005, 5, 4, 3, 2, 1));
      Assert.AreEqual("03", element.ParseableText);

      token = new Token("HHH", false);
      element = DateTimeDisplayElement.Create(token, TestCultures.EnglishNZ) as DateTimeNumericDisplayElement;
      element.Bind(new DateTime(2005, 5, 4, 3, 2, 1));
      Assert.AreEqual("03", element.ParseableText);

      token = new Token("m", false);
      element = DateTimeDisplayElement.Create(token, TestCultures.EnglishNZ) as DateTimeNumericDisplayElement;
      element.Bind(new DateTime(2005, 5, 4, 3, 2, 1));
      Assert.AreEqual("2", element.ParseableText);

      token = new Token("mm", false);
      element = DateTimeDisplayElement.Create(token, TestCultures.EnglishNZ) as DateTimeNumericDisplayElement;
      element.Bind(new DateTime(2005, 5, 4, 3, 2, 1));
      Assert.AreEqual("02", element.ParseableText);

      token = new Token("mmm", false);
      element = DateTimeDisplayElement.Create(token, TestCultures.EnglishNZ) as DateTimeNumericDisplayElement;
      element.Bind(new DateTime(2005, 5, 4, 3, 2, 1));
      Assert.AreEqual("02", element.ParseableText);

      token = new Token("s", false);
      element = DateTimeDisplayElement.Create(token, TestCultures.EnglishNZ) as DateTimeNumericDisplayElement;
      element.Bind(new DateTime(2005, 5, 4, 3, 2, 1));
      Assert.AreEqual("1", element.ParseableText);

      token = new Token("ss", false);
      element = DateTimeDisplayElement.Create(token, TestCultures.EnglishNZ) as DateTimeNumericDisplayElement;
      element.Bind(new DateTime(2005, 5, 4, 3, 2, 1));
      Assert.AreEqual("01", element.ParseableText);

      token = new Token("sss", false);
      element = DateTimeDisplayElement.Create(token, TestCultures.EnglishNZ) as DateTimeNumericDisplayElement;
      element.Bind(new DateTime(2005, 5, 4, 3, 2, 1));
      Assert.AreEqual("01", element.ParseableText);
    }

    [Test]
    public void EstimatedProportions()
    {
      Token token;
      DateTimeDisplayElement element;
      DateTime sampleDate = new DateTime(2008, 1, 1, 1, 1, 1);

      // TODO: We would prefer not to have to bind before getting the proportion,
      // but for non-Gregorian calendars the set of available options may
      // depend on the date (e.g. leap months only occur in certain years, and
      // the name of the leap month might affect the sizing).  [Then again we would
      // actually prefer to leave room for the leap month right at the get-go,
      // and avoid jiggy-jig as the year spins over.]

      token = new Token("xxxxx", false);
      element = DateTimeDisplayElement.Create(token, TestCultures.EnglishNZ);
      element.Bind(sampleDate);
      Assert.AreEqual(50, element.EstimatedProportion);

      token = new Token("dd", false);
      element = DateTimeDisplayElement.Create(token, TestCultures.EnglishNZ);
      element.Bind(sampleDate);
      Assert.AreEqual(25, element.EstimatedProportion);

      token = new Token("ddd", false);
      element = DateTimeDisplayElement.Create(token, TestCultures.FrenchFrance);
      element.Bind(sampleDate);
      Assert.AreEqual(40, element.EstimatedProportion);  // dim., lun., etc.

      token = new Token("dddd", false);
      element = DateTimeDisplayElement.Create(token, TestCultures.FrenchFrance);
      element.Bind(sampleDate);
      Assert.AreEqual(80, element.EstimatedProportion);  // mercredi, vendredi

      token = new Token("MMMM", false);
      element = DateTimeDisplayElement.Create(token, TestCultures.FrenchFrance);
      element.Bind(sampleDate);
      Assert.AreEqual(90, element.EstimatedProportion);  // septembre
    }

    [Test]
    public void Precision()
    {
      Token token;
      DateTimeNumericDisplayElement element;
      DateTime sampleDate = new DateTime(1234, 5, 6, 7, 8, 9);

      token = new Token("d", false);
      element = DateTimeDisplayElement.Create(token, TestCultures.EnglishNZ) as DateTimeNumericDisplayElement;
      element.Bind(sampleDate);
      Assert.AreEqual(1, element.Precision);

      token = new Token("yyy", false);
      element = DateTimeDisplayElement.Create(token, TestCultures.EnglishNZ) as DateTimeNumericDisplayElement;
      element.Bind(sampleDate);
      Assert.AreEqual(3, element.Precision);

      token = new Token("hh", false);
      element = DateTimeDisplayElement.Create(token, TestCultures.EnglishNZ) as DateTimeNumericDisplayElement;
      element.Bind(sampleDate);
      Assert.AreEqual(2, element.Precision);
    }
  }
}
