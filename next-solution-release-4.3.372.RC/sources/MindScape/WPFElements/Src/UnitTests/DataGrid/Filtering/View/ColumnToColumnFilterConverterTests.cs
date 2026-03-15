using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Mindscape.WpfElements.PropertyEditing;
using Mindscape.WpfElements.WpfDataGrid;
using Mindscape.WpfElements.WpfPropertyGrid.UnitTests;
using NUnit.Framework;

namespace Mindscape.WpfElements.UnitTests
{
  [TestFixture]
  public class ColumnToColumnFilterConverterTests
  {
    private readonly IPropertyInfo _stringPropertyInfo = new PassthroughPropertyInfoAdapter(typeof(Person).GetProperty("FirstName"));
    private ColumnToColumnFilterConverter _converter;

    [SetUp]
    public void SetUp()
    {
      _converter = new ColumnToColumnFilterConverter();
    }

    [Test]
    public void UseBooleanFilter_DefaultValue()
    {
      Assert.IsTrue(_converter.UseBooleanFilter);
    }

    [Test]
    public void UseBooleanFilterProperty()
    {
      _converter.UseBooleanFilter = false;
      Assert.IsFalse(_converter.UseBooleanFilter);
      _converter.UseBooleanFilter = true;
      Assert.IsTrue(_converter.UseBooleanFilter);
    }

    [Test]
    public void Convert_UseBooleanFilter_True()
    {
      DataGridColumn column = new DataGridColumn() { PropertyInfo = _stringPropertyInfo };
      _converter.UseBooleanFilter = true;
      ColumnFilter filter = _converter.Convert(column, typeof(ColumnFilter), null, CultureInfo.CurrentUICulture) as ColumnFilter;

      Assert.IsNotNull(filter);
      Assert.AreSame(column, filter.Column);
      Assert.IsNotNull(filter.Description);
      Assert.IsInstanceOf<BooleanFilterDescription>(filter.Description);
    }

    [Test]
    public void Convert_UseBooleanFilter_False()
    {
      DataGridColumn column = new DataGridColumn() { PropertyInfo = _stringPropertyInfo };
      _converter.UseBooleanFilter = false;
      ColumnFilter filter = _converter.Convert(column, typeof(ColumnFilter), null, CultureInfo.CurrentUICulture) as ColumnFilter;

      Assert.IsNotNull(filter);
      Assert.AreSame(column, filter.Column);
      Assert.IsNotNull(filter.Description);
      Assert.IsNotInstanceOf<BooleanFilterDescription>(filter.Description);
    }
  }
}
