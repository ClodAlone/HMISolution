using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mindscape.WpfElements.WpfDataGrid;
using NUnit.Framework;

namespace Mindscape.WpfElements.UnitTests
{
  [TestFixture]
  public class ComparableFilterDescriptionTests
  {
    // TODO what happens when FirstValue is set to a different type of IComparable than the ComparableType?

    private ComparableFilterDescription _description;

    [SetUp]
    public void SetUp()
    {
      _description = new ComparableFilterDescription(typeof(int));
    }

    [Test]
    public void ComparableTypeProperty()
    {
      Assert.AreEqual(typeof(int), _description.ComparableType);

      _description = new ComparableFilterDescription(typeof(string));
      Assert.AreEqual(typeof(string), _description.ComparableType);

      _description = new ComparableFilterDescription(typeof(double));
      Assert.AreEqual(typeof(double), _description.ComparableType);

      _description = new ComparableFilterDescription(typeof(DateTime));
      Assert.AreEqual(typeof(DateTime), _description.ComparableType);
    }

    [Test]
    public void DefaultValues()
    {
      Assert.AreEqual(0, _description.FirstValue);
      Assert.AreEqual(0, _description.SecondValue);

      _description = new ComparableFilterDescription(typeof(string));
      Assert.AreEqual(null, _description.FirstValue);
      Assert.AreEqual(null, _description.SecondValue);

      _description = new ComparableFilterDescription(typeof(double));
      Assert.AreEqual(0.0, _description.FirstValue);
      Assert.AreEqual(0.0, _description.SecondValue);

      _description = new ComparableFilterDescription(typeof(DateTime));
      Assert.IsTrue(_description.FirstValue is DateTime);
      Assert.AreEqual(_description.FirstValue, _description.SecondValue);
      DateTime dateTime = (DateTime)_description.FirstValue;
      TimeSpan diff = DateTime.Now.Date - dateTime;
      Assert.Less(diff.TotalDays, 2); // This is the best we can do to check the default date is today in a test environment.
    }

    [Test]
    public void FirstValueProperty()
    {
      _description.FirstValue = 1;
      Assert.AreEqual(1, _description.FirstValue);
    }

    [Test]
    public void FirstValueProperty_UpdatesFilter()
    {
      _description.Builder = new LessThanFilterBuilder(); // Need to at least set the builder so the filter is not null.
      _description.FirstValue = 2;
      LessThanFilter filter = _description.Filter as LessThanFilter;
      Assert.IsNotNull(filter);
      Assert.AreEqual(2, filter.Value);
    }

    [Test]
    public void FirstValueProperty_RaisesPropertyChanged()
    {
      bool raised = false;
      _description.PropertyChanged += (o, e) => { if ("FirstValue".Equals(e.PropertyName)) { raised = true; } };
      _description.FirstValue = 3;
      Assert.IsTrue(raised);
    }

    [Test]
    public void SecondValueProperty()
    {
      _description.SecondValue = 1;
      Assert.AreEqual(1, _description.SecondValue);
    }

    [Test]
    public void SecondValueProperty_UpdatesFilter()
    {
      _description.Builder = new LessThanFilterBuilder(); // Need to at least set the builder so the filter is not null.
      _description.SecondValue = 2;
      LessThanFilter filter = _description.Filter as LessThanFilter;
      Assert.IsNotNull(filter);
      // TODO: check that the second value is applied when we have a builder for something like the range filter.
    }

    [Test]
    public void SecondValueProperty_RaisesPropertyChanged()
    {
      bool raised = false;
      _description.PropertyChanged += (o, e) => { if ("SecondValue".Equals(e.PropertyName)) { raised = true; } };
      _description.SecondValue = 3;
      Assert.IsTrue(raised);
    }

    [Test]
    public void BuilderProperty()
    {
      LessThanOrEqualToFilterBuilder builder = new LessThanOrEqualToFilterBuilder();
      _description.Builder = builder;
      Assert.AreSame(builder, _description.Builder);
    }

    [Test]
    public void BuilderProperty_UpdatesFilter()
    {
      _description.FirstValue = 42; // Need to at least set FirstValue so filter is not null
      _description.Builder = new GreaterThanFilterBuilder();
      GreaterThanFilter filter = _description.Filter as GreaterThanFilter;
      Assert.IsNotNull(filter);
    }

    [Test]
    public void BuilderProperty_RaisesPropertyChanged()
    {
      bool raised = false;
      _description.PropertyChanged += (o, e) => { if ("Builder".Equals(e.PropertyName)) { raised = true; } };
      _description.Builder = new EqualsFilterBuilder();
      Assert.IsTrue(raised);
    }

    [Test]
    public void NullBuilderMeansNullFilter()
    {
      _description.FirstValue = 9; // Need to at least set FirstValue
      _description.Builder = null;
      Assert.IsNull(_description.Filter);
    }

    [Test]
    public void FilterProperty_DefaultValue()
    {
      Assert.IsNull(_description.Filter);
    }

    [Test]
    public void UpdatingFilter_RaisesEvent()
    {
      bool raised = false;
      _description.FilterChanged += (o, e) => { raised = true; };
      _description.FirstValue = 1;
      Assert.IsTrue(raised);

      raised = false;
      _description.SecondValue = 2;
      Assert.IsTrue(raised);

      raised = false;
      _description.Builder = null;
      Assert.IsTrue(raised);
    }

    // TODO: test that only one event is raised when calling SetAs.
    // TODO: test other comparable types? Or at least test the SetDefaultValue method (currently private).

    [Test]
    public void SetAsNull()
    {
      // Set some non-default values
      _description.Builder = new LessThanFilterBuilder();
      _description.FirstValue = 1;
      _description.SecondValue = 2;

      _description.SetAs(null);
      // The builder is a NoFilterBuilder
      Assert.IsNotNull(_description.Builder);
      Assert.IsInstanceOf<NoFilterBuilder>(_description.Builder);
      // Default values are restored
      Assert.AreEqual(0, _description.FirstValue);
      Assert.AreEqual(0, _description.SecondValue);
      // The resulting filter is null
      Assert.IsNull(_description.Filter);
    }

    [Test]
    public void SetAsUnsupportedFilter()
    {
      // Set some non-default values
      _description.Builder = new LessThanFilterBuilder();
      _description.FirstValue = 1;
      _description.SecondValue = 2;

      FalseFilter filter = new FalseFilter();
      _description.SetAs(filter);

      // The builder is a NoFilterBuilder
      Assert.IsNotNull(_description.Builder);
      Assert.IsInstanceOf<NoFilterBuilder>(_description.Builder);
      // Default values are restored
      Assert.AreEqual(0, _description.FirstValue);
      Assert.AreEqual(0, _description.SecondValue);
      // The resulting filter is null
      Assert.IsNull(_description.Filter);
    }

    [Test]
    public void SetAsGreaterThanFilter()
    {
      // Set some non-default values
      _description.FirstValue = 1;
      _description.SecondValue = 2;

      GreaterThanFilter filter = new GreaterThanFilter(3);
      _description.SetAs(filter);
      // The builder is a GreaterThanFilterBuilder
      Assert.IsNotNull(_description.Builder);
      Assert.IsInstanceOf<GreaterThanFilterBuilder>(_description.Builder);
      // The first value is copied
      Assert.AreEqual(3, _description.FirstValue);
      // The second value is reverted to default
      Assert.AreEqual(0, _description.SecondValue);

      // The resulting filter is mimicked
      GreaterThanFilter f = _description.Filter as GreaterThanFilter;
      Assert.IsNotNull(f);
      Assert.AreEqual(3, f.Value);
    }

    [Test]
    public void SetAsGreaterThanOrEqualToFilter()
    {
      // Set some non-default values
      _description.FirstValue = 1;
      _description.SecondValue = 2;

      GreaterThanOrEqualToFilter filter = new GreaterThanOrEqualToFilter(3);
      _description.SetAs(filter);
      // The builder is a GreaterThanOrEqualToFilterBuilder
      Assert.IsNotNull(_description.Builder);
      Assert.IsInstanceOf<GreaterThanOrEqualToFilterBuilder>(_description.Builder);
      // The first value is copied
      Assert.AreEqual(3, _description.FirstValue);
      // The second value is reverted to default
      Assert.AreEqual(0, _description.SecondValue);

      // The resulting filter is mimicked
      GreaterThanOrEqualToFilter f = _description.Filter as GreaterThanOrEqualToFilter;
      Assert.IsNotNull(f);
      Assert.AreEqual(3, f.Value);
    }

    [Test]
    public void SetAsLessThanFilter()
    {
      // Set some non-default values
      _description.FirstValue = 1;
      _description.SecondValue = 2;

      LessThanFilter filter = new LessThanFilter(3);
      _description.SetAs(filter);
      // The builder is a LessThanFilterBuilder
      Assert.IsNotNull(_description.Builder);
      Assert.IsInstanceOf<LessThanFilterBuilder>(_description.Builder);
      // The first value is copied
      Assert.AreEqual(3, _description.FirstValue);
      // The second value is reverted to default
      Assert.AreEqual(0, _description.SecondValue);

      // The resulting filter is mimicked
      LessThanFilter f = _description.Filter as LessThanFilter;
      Assert.IsNotNull(f);
      Assert.AreEqual(3, f.Value);
    }

    [Test]
    public void SetAsLessThanOrEqualToFilter()
    {
      // Set some non-default values
      _description.FirstValue = 1;
      _description.SecondValue = 2;

      LessThanOrEqualToFilter filter = new LessThanOrEqualToFilter(3);
      _description.SetAs(filter);
      // The builder is a LessThanOrEqualToFilterBuilder
      Assert.IsNotNull(_description.Builder);
      Assert.IsInstanceOf<LessThanOrEqualToFilterBuilder>(_description.Builder);
      // The first value is copied
      Assert.AreEqual(3, _description.FirstValue);
      // The second value is reverted to default
      Assert.AreEqual(0, _description.SecondValue);

      // The resulting filter is mimicked
      LessThanOrEqualToFilter f = _description.Filter as LessThanOrEqualToFilter;
      Assert.IsNotNull(f);
      Assert.AreEqual(3, f.Value);
    }

    [Test]
    public void SetAsEqualsFilter()
    {
      // Set some non-default values
      _description.FirstValue = 1;
      _description.SecondValue = 2;

      EqualsFilter filter = new EqualsFilter(3);
      _description.SetAs(filter);
      // The builder is a EqualsFilterBuilder
      Assert.IsNotNull(_description.Builder);
      Assert.IsInstanceOf<EqualsFilterBuilder>(_description.Builder);
      // The first value is copied
      Assert.AreEqual(3, _description.FirstValue);
      // The second value is reverted to default
      Assert.AreEqual(0, _description.SecondValue);

      // The resulting filter is mimicked
      EqualsFilter f = _description.Filter as EqualsFilter;
      Assert.IsNotNull(f);
      Assert.AreEqual(3, f.Value);
    }

    [Test]
    public void SetAsNotEqualFilter()
    {
      // Set some non-default values
      _description.FirstValue = 1;
      _description.SecondValue = 2;

      NotEqualFilter filter = new NotEqualFilter(3);
      _description.SetAs(filter);
      // The builder is a NotEqualFilterBuilder
      Assert.IsNotNull(_description.Builder);
      Assert.IsInstanceOf<NotEqualFilterBuilder>(_description.Builder);
      // The first value is copied
      Assert.AreEqual(3, _description.FirstValue);
      // The second value is reverted to default
      Assert.AreEqual(0, _description.SecondValue);

      // The resulting filter is mimicked
      NotEqualFilter f = _description.Filter as NotEqualFilter;
      Assert.IsNotNull(f);
      Assert.AreEqual(3, f.Value);
    }
  }
}
