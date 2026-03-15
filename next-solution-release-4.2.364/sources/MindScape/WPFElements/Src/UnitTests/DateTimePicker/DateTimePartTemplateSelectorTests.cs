using NUnit.Framework;
using System.Windows;
using System.Globalization;

namespace Mindscape.WpfElements.UnitTests
{
  [TestFixture]
  public class DateTimePartTemplateSelectorTests
  {
    [Test]
    public void SelectTemplate()
    {
      DateTimePartTemplateSelector selector = new DateTimePartTemplateSelector();
      selector.NumericElementTemplate = new DataTemplate();
      selector.ReadOnlyElementTemplate = new DataTemplate();
      selector.SelectElementTemplate = new DataTemplate();

      Token token;
      DateTimeDisplayElement element;
      DataTemplate selectedTemplate;

      token = new Token("d", false);
      element = new DateTimeNumericDisplayElement(token, CultureInfo.InvariantCulture);
      selectedTemplate = selector.SelectTemplate(element, null);
      Assert.AreEqual(selector.NumericElementTemplate, selectedTemplate);

      token = new Token("x", false);
      element = new DateTimeReadOnlyDisplayElement(token, CultureInfo.InvariantCulture);
      selectedTemplate = selector.SelectTemplate(element, null);
      Assert.AreEqual(selector.ReadOnlyElementTemplate, selectedTemplate);

      token = new Token("MMM", false);
      element = new DateTimeSelectDisplayElement(token, CultureInfo.InvariantCulture);
      selectedTemplate = selector.SelectTemplate(element, null);
      Assert.AreEqual(selector.SelectElementTemplate, selectedTemplate);
    }
  }
}
