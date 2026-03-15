using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using Mindscape.WpfElements.PropertyEditing;
using Mindscape.WpfElements.WpfDataGrid;
using Mindscape.WpfElements.WpfPropertyGrid.UnitTests;
using NUnit.Framework;

namespace Mindscape.WpfElements.UnitTests
{
  [TestFixture]
  public class FilterTemplateSelectorTests
  {
    private FilterTemplateSelector _blankSelector;
    private FilterTemplateSelector _selector;
    private DataTemplate _stringTemplate;
    private DataTemplate _numericTemplate;
    private DataTemplate _enumTemplate;
    private DataTemplate _noTemplate;

    private DataGridColumn _column;
    private readonly IPropertyInfo _stringPropertyInfo = new PassthroughPropertyInfoAdapter(typeof(Person).GetProperty("FirstName"));
    private readonly IPropertyInfo _enumPropertyInfo = new PassthroughPropertyInfoAdapter(typeof(Person).GetProperty("Status"));
    private readonly IPropertyInfo _boolPropertyInfo = new PassthroughPropertyInfoAdapter(typeof(Person).GetProperty("Tall"));
    private readonly IPropertyInfo _doublePropertyInfo = new PassthroughPropertyInfoAdapter(typeof(Person).GetProperty("Age"));
    // TODO: make a new test class that has properties for all the types that we support. i.e. int, decimal, byte, single, UInt32, etc
    // TODO: also test eums and booleans that use a custom type converter.

    [SetUp]
    public void SetUp()
    {
      _blankSelector = new FilterTemplateSelector();

      _selector = new FilterTemplateSelector();
      _stringTemplate = new DataTemplate();
      _numericTemplate = new DataTemplate();
      _enumTemplate = new DataTemplate();
      _noTemplate = new DataTemplate();
      _selector.StringFilterTemplate = _stringTemplate;
      _selector.NumericFilterTemplate = _numericTemplate;
      _selector.EnumFilterTemplate = _enumTemplate;
      _selector.NoFilterTemplate = _noTemplate;

      _column = new DataGridColumn();
    }

    [Test]
    public void StringFilterTemplate_DefaultValue()
    {
      Assert.IsNull(_blankSelector.StringFilterTemplate);
    }

    [Test]
    public void StringFilterTemplateProperty()
    {
      _blankSelector.StringFilterTemplate = _stringTemplate;
      Assert.AreSame(_stringTemplate, _blankSelector.StringFilterTemplate);
    }

    [Test]
    public void NumericFilterTemplate_DefautValue()
    {
      Assert.IsNull(_blankSelector.NumericFilterTemplate);
    }

    [Test]
    public void NumericFilterTemplateProperty()
    {
      _blankSelector.NumericFilterTemplate = _numericTemplate;
      Assert.AreSame(_numericTemplate, _blankSelector.NumericFilterTemplate);
    }

    [Test]
    public void EnumFilterTemplate_DefaultValue()
    {
      Assert.IsNull(_blankSelector.EnumFilterTemplate);
    }

    [Test]
    public void EnumFilterTemplateProperty()
    {
      _blankSelector.EnumFilterTemplate = _enumTemplate;
      Assert.AreSame(_enumTemplate, _blankSelector.EnumFilterTemplate);
    }

    [Test]
    public void NoFilterTemplate_DefaultValue()
    {
      Assert.IsNull(_blankSelector.NoFilterTemplate);
    }

    [Test]
    public void NoFilterTemplateProperty()
    {
      _blankSelector.NoFilterTemplate = _noTemplate;
      Assert.AreSame(_noTemplate, _blankSelector.NoFilterTemplate);
    }

    [Test]
    public void SelectTemplate_Null()
    {
      Assert.AreSame(_noTemplate, _selector.SelectTemplate(null, null));
    }

    [Test]
    public void SelectTemplate_NullPropertyInfo()
    {
      _column.PropertyInfo = null;
      Assert.AreSame(_noTemplate, _selector.SelectTemplate(_column, null));
    }

    [Test]
    public void SelectTemplate_UnsupportedType()
    {
      // Department is a custom object, so the NoFilterTemplate should be selected.
      IPropertyInfo customPropertyInfo = new PassthroughPropertyInfoAdapter(typeof(Person).GetProperty("Department"));
      _column.PropertyInfo = customPropertyInfo;
      Assert.AreSame(_noTemplate, _selector.SelectTemplate(_column, null));
    }

    [Test]
    public void SelectTemplate_String()
    {
      _column.PropertyInfo = _stringPropertyInfo;
      Assert.AreSame(_stringTemplate, _selector.SelectTemplate(_column, null));
    }

    [Test]
    public void SelectTemplate_Double()
    {
      _column.PropertyInfo = _doublePropertyInfo;
      Assert.AreSame(_numericTemplate, _selector.SelectTemplate(_column, null));
    }

    [Test]
    public void SelectTemplate_Enum()
    {
      _column.PropertyInfo = _enumPropertyInfo;
      Assert.AreSame(_enumTemplate, _selector.SelectTemplate(_column, null));
    }

    [Test]
    public void SelectTemplate_Bool()
    {
      _column.PropertyInfo = _boolPropertyInfo;
      Assert.AreSame(_enumTemplate, _selector.SelectTemplate(_column, null));
    }
  }
}
