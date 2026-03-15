using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Mindscape.WpfElements.PropertyEditing;
using Mindscape.WpfElements.WpfDataGrid;
using Mindscape.WpfElements.WpfPropertyGrid.UnitTests;
using NUnit.Framework;

namespace Mindscape.WpfElements.UnitTests
{
  [TestFixture]
  public class ColumnFilterTests
  {
    private readonly IPropertyInfo _stringPropertyInfo = new PassthroughPropertyInfoAdapter(typeof(Person).GetProperty("FirstName"));
    private readonly IPropertyInfo _enumPropertyInfo = new PassthroughPropertyInfoAdapter(typeof(Person).GetProperty("Status"));
    private readonly IPropertyInfo _boolPropertyInfo = new PassthroughPropertyInfoAdapter(typeof(Person).GetProperty("Tall"));
    //private readonly IPropertyInfo _intPropertyInfo = new PassthroughPropertyInfoAdapter(typeof(Person).GetProperty("")); // TODO
    //private readonly IPropertyInfo _decimalPropertyInfo = new PassthroughPropertyInfoAdapter(typeof(Person).GetProperty("")); // TODO
    private readonly IPropertyInfo _doublePropertyInfo = new PassthroughPropertyInfoAdapter(typeof(Person).GetProperty("Age"));
    private readonly IPropertyInfo _dateTimePropertyInfo = new PassthroughPropertyInfoAdapter(typeof(Person).GetProperty("DateOfBirth"));
    // TODO: also test enums/booleans with type converters

    // TODO: test that if the column already has a filter, it gets mimicked into the generated description.

    [Test]
    public void ColumnProperty()
    {
      DataGridColumn column = new DataGridColumn() { PropertyInfo = _stringPropertyInfo };
      ColumnFilter cf = new ColumnFilter(column, false);

      Assert.AreSame(column, cf.Column);
    }

    // TODO: test these 2 change/update methods for:
    //  -- useBooleanFilter
    //  -- ObjectFilterDescription (because this has a different code path)

    [Test]
    public void ChangingColumnFilterUpdatesDescription()
    {
      DataGridColumn column = new DataGridColumn() { PropertyInfo = _stringPropertyInfo };
      ColumnFilter cf = new ColumnFilter(column, false);

      EndsWithFilter filter = new EndsWithFilter("Q", true);
      column.Filter = filter;

      StringFilterDescription description = cf.Description as StringFilterDescription;
      Assert.IsNotNull(description.Builder);
      Assert.IsInstanceOf<EndsWithFilterBuilder>(description.Builder);
      Assert.AreEqual("Q", description.FirstValue);
      Assert.IsTrue(description.MatchCase);
    }

    [Test]
    public void UpdatingDescriptionSetsColumnFilter()
    {
      DataGridColumn column = new DataGridColumn() { PropertyInfo = _stringPropertyInfo };
      ColumnFilter cf = new ColumnFilter(column, false);

      StringFilterDescription description = cf.Description as StringFilterDescription;
      description.FirstValue = "P";
      description.MatchCase = true;
      description.Builder = new EndsWithFilterBuilder();

      EndsWithFilter filter = column.Filter as EndsWithFilter;
      Assert.IsNotNull(filter);
      Assert.AreEqual("P", filter.Value);
      Assert.IsTrue(filter.MatchCase);
    }

    [Test]
    public void NullPropertyInfo()
    {
      DataGridColumn column = new DataGridColumn();
      ColumnFilter cf = new ColumnFilter(column, false);

      Assert.IsNull(cf.Description);
    }

    [Test]
    public void NullPropertyInfo_UseBooleanFilter()
    {
      DataGridColumn column = new DataGridColumn();
      ColumnFilter cf = new ColumnFilter(column, true);

      Assert.IsNull(cf.Description);
    }

    [Test]
    public void StringPropertyInfo()
    {
      DataGridColumn column = new DataGridColumn() { PropertyInfo = _stringPropertyInfo };
      ColumnFilter cf = new ColumnFilter(column, false);

      StringFilterDescription description = cf.Description as StringFilterDescription;
      Assert.IsNotNull(description);
    }

    [Test]
    public void StringPropertyInfo_UseBooleanFilter()
    {
      DataGridColumn column = new DataGridColumn() { PropertyInfo = _stringPropertyInfo };
      ColumnFilter cf = new ColumnFilter(column, true);

      BooleanFilterDescription description = cf.Description as BooleanFilterDescription;
      Assert.IsNotNull(description);

      StringFilterDescription d1 = description.FirstFilter as StringFilterDescription;
      Assert.IsNotNull(d1);
      StringFilterDescription d2 = description.SecondFilter as StringFilterDescription;
      Assert.IsNotNull(d2);
    }

    [Test]
    public void EnumPropertyInfo()
    {
      DataGridColumn column = new DataGridColumn() { PropertyInfo = _enumPropertyInfo };
      ColumnFilter cf = new ColumnFilter(column, false);

      ObjectFilterDescription description = cf.Description as ObjectFilterDescription;
      Assert.IsNotNull(description);
      Assert.AreEqual(4, description.Values.Count);
      Assert.AreEqual(CitizenshipStatus.Citizen, ((SelectableObject)description.Values[0]).ActualValue);
      Assert.AreEqual(CitizenshipStatus.Resident, ((SelectableObject)description.Values[1]).ActualValue);
      Assert.AreEqual(CitizenshipStatus.WorkerVisa, ((SelectableObject)description.Values[2]).ActualValue);
      Assert.AreEqual(CitizenshipStatus.Alien, ((SelectableObject)description.Values[3]).ActualValue);
    }

    [Test]
    public void EnumPropertyInfo_UseBooleanFilter()
    {
      DataGridColumn column = new DataGridColumn() { PropertyInfo = _enumPropertyInfo };
      ColumnFilter cf = new ColumnFilter(column, true);

      // No point using a boolean filter description for objects, so just return a single ObjectFilterDescription
      ObjectFilterDescription description = cf.Description as ObjectFilterDescription;
      Assert.IsNotNull(description);
      Assert.AreEqual(4, description.Values.Count);
      Assert.AreEqual(CitizenshipStatus.Citizen, ((SelectableObject)description.Values[0]).ActualValue);
      Assert.AreEqual(CitizenshipStatus.Resident, ((SelectableObject)description.Values[1]).ActualValue);
      Assert.AreEqual(CitizenshipStatus.WorkerVisa, ((SelectableObject)description.Values[2]).ActualValue);
      Assert.AreEqual(CitizenshipStatus.Alien, ((SelectableObject)description.Values[3]).ActualValue);
    }

    [Test]
    public void BooleanPropertyInfo()
    {
      DataGridColumn column = new DataGridColumn() { PropertyInfo = _boolPropertyInfo };
      ColumnFilter cf = new ColumnFilter(column, false);

      ObjectFilterDescription description = cf.Description as ObjectFilterDescription;
      Assert.IsNotNull(description);
      Assert.AreEqual(2, description.Values.Count);
      Assert.AreEqual(true, ((SelectableObject)description.Values[0]).ActualValue);
      Assert.AreEqual(false, ((SelectableObject)description.Values[1]).ActualValue);
    }

    [Test]
    public void BooleanPropertyInfo_UseBooleanFilter()
    {
      DataGridColumn column = new DataGridColumn() { PropertyInfo = _boolPropertyInfo };
      ColumnFilter cf = new ColumnFilter(column, true);

      // No point using a boolean filter description for objects, so just return a single ObjectFilterDescription
      ObjectFilterDescription description = cf.Description as ObjectFilterDescription;
      Assert.IsNotNull(description);
      Assert.AreEqual(2, description.Values.Count);
      Assert.AreEqual(true, ((SelectableObject)description.Values[0]).ActualValue);
      Assert.AreEqual(false, ((SelectableObject)description.Values[1]).ActualValue);
    }

    [Test]
    public void DoublePropertyInfo()
    {
      DataGridColumn column = new DataGridColumn() { PropertyInfo = _doublePropertyInfo };
      ColumnFilter cf = new ColumnFilter(column, false);

      ComparableFilterDescription description = cf.Description as ComparableFilterDescription;
      Assert.IsNotNull(description);
      Assert.AreEqual(typeof(double), description.ComparableType);
    }

    [Test]
    public void DoublePropertyInfo_UseBooleanFilter()
    {
      DataGridColumn column = new DataGridColumn() { PropertyInfo = _doublePropertyInfo };
      ColumnFilter cf = new ColumnFilter(column, true);

      BooleanFilterDescription description = cf.Description as BooleanFilterDescription;
      Assert.IsNotNull(description);

      ComparableFilterDescription d1 = description.FirstFilter as ComparableFilterDescription;
      Assert.IsNotNull(d1);
      Assert.AreEqual(typeof(double), d1.ComparableType);

      ComparableFilterDescription d2 = description.SecondFilter as ComparableFilterDescription;
      Assert.IsNotNull(d2);
      Assert.AreEqual(typeof(double), d2.ComparableType);
    }

    [Test]
    public void DateTimePropertyInfo()
    {
      DataGridColumn column = new DataGridColumn() { PropertyInfo = _dateTimePropertyInfo };
      ColumnFilter cf = new ColumnFilter(column, false);

      ComparableFilterDescription description = cf.Description as ComparableFilterDescription;
      Assert.IsNotNull(description);
      Assert.AreEqual(typeof(DateTime), description.ComparableType);
    }

    [Test]
    public void DateTimePropertyInfo_UseBooleanFilter()
    {
      DataGridColumn column = new DataGridColumn() { PropertyInfo = _dateTimePropertyInfo };
      ColumnFilter cf = new ColumnFilter(column, true);

      BooleanFilterDescription description = cf.Description as BooleanFilterDescription;
      Assert.IsNotNull(description);

      ComparableFilterDescription d1 = description.FirstFilter as ComparableFilterDescription;
      Assert.IsNotNull(d1);
      Assert.AreEqual(typeof(DateTime), d1.ComparableType);

      ComparableFilterDescription d2 = description.SecondFilter as ComparableFilterDescription;
      Assert.IsNotNull(d2);
      Assert.AreEqual(typeof(DateTime), d2.ComparableType);
    }
  }
}
