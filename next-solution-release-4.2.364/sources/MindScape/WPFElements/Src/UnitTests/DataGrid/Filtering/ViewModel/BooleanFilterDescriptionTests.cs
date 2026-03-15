using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mindscape.WpfElements.WpfDataGrid;
using NUnit.Framework;

namespace Mindscape.WpfElements.UnitTests
{
  [TestFixture]
  public class BooleanFilterDescriptionTests
  {
    private FakeFilterDescription _fakeDescription1;
    private FakeFilterDescription _fakeDescription2;
    private BooleanFilterDescription _description;

    [SetUp]
    public void SetUp()
    {
      _fakeDescription1 = new FakeFilterDescription();
      _fakeDescription2 = new FakeFilterDescription();
      _description = new BooleanFilterDescription(_fakeDescription1, _fakeDescription2);
    }

    [Test]
    public void FirstFilterProperty()
    {
      Assert.AreSame(_fakeDescription1, _description.FirstFilter);
    }

    [Test]
    public void FirstFilterBuilderIsUsed()
    {
      OrFilterBuilder builder = new OrFilterBuilder();
      _description.Builder = builder;
      OrFilter filter = _description.Filter as OrFilter; // We know this works from the BuilderProperty_UpdatesFilter test below
      FalseFilter falseFilter = filter.Filters[0] as FalseFilter; // We know this works from the OrFilterBuilderTests

      Assert.IsNotNull(falseFilter); // FalseFilterBuilder is the default of the FakeFilterDescription
    }

    [Test]
    public void FirstFilter_UpdatesFilter()
    {
      OrFilterBuilder builder = new OrFilterBuilder();
      _description.Builder = builder;
      _fakeDescription1.SetFilter(new TrueFilter());

      OrFilter filter = _description.Filter as OrFilter;
      TrueFilter trueFilter = filter.Filters[0] as TrueFilter;
      Assert.IsNotNull(trueFilter);
    }

    [Test]
    public void SecondFilterProperty()
    {
      Assert.AreSame(_fakeDescription2, _description.SecondFilter);
    }

    [Test]
    public void SecondFilterBuilderIsUsed()
    {
      OrFilterBuilder builder = new OrFilterBuilder();
      _description.Builder = builder;
      OrFilter filter = _description.Filter as OrFilter; // We know this works from the BuilderProperty_UpdatesFilter test below
      FalseFilter falseFilter = filter.Filters[1] as FalseFilter; // We know this works from the OrFilterBuilderTests

      Assert.IsNotNull(falseFilter); // FalseFilterBuilder is the default of the FakeFilterDescription
    }

    [Test]
    public void SecondFilter_UpdatesFilter()
    {
      OrFilterBuilder builder = new OrFilterBuilder();
      _description.Builder = builder;
      _fakeDescription2.SetFilter(new TrueFilter());

      OrFilter filter = _description.Filter as OrFilter;
      TrueFilter trueFilter = filter.Filters[1] as TrueFilter;
      Assert.IsNotNull(trueFilter);
    }

    [Test]
    public void BuilderProperty_DefaultValue()
    {
      Assert.IsNotNull(_description.Builder);
      Assert.IsInstanceOf<AndFilterBuilder>(_description.Builder);
    }

    [Test]
    public void BuilderProperty()
    {
      OrFilterBuilder builder = new OrFilterBuilder();
      _description.Builder = builder;
      Assert.AreSame(builder, _description.Builder);
    }

    [Test]
    public void BuilderProperty_UpdatesFilter()
    {
      _description.Builder = new OrFilterBuilder();
      OrFilter filter = _description.Filter as OrFilter;
      Assert.IsNotNull(filter);
    }

    [Test]
    public void BuilderProperty_RaisesPropertyChanged()
    {
      bool raised = false;
      _description.PropertyChanged += (o, e) => { if ("Builder".Equals(e.PropertyName)) { raised = true; } };
      _description.Builder = new OrFilterBuilder();
      Assert.IsTrue(raised);
    }

    [Test]
    public void NullBuilderMeansNullFilter()
    {
      _description.Builder = new AndFilterBuilder();
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
      _description.Builder = new OrFilterBuilder();
      Assert.IsTrue(raised);
    }

    [Test]
    public void UpdatingFirstFilter_RaisesEvent()
    {
      _description.Builder = new OrFilterBuilder();
      bool raised = false;
      _description.FilterChanged += (o, e) => { raised = true; };
      Assert.IsFalse(raised);
      _fakeDescription1.SetFilter(new TrueFilter());
      Assert.IsTrue(raised);
    }

    [Test]
    public void UpdatingSecondFilter_RaisesEvent()
    {
      _description.Builder = new OrFilterBuilder();
      bool raised = false;
      _description.FilterChanged += (o, e) => { raised = true; };
      Assert.IsFalse(raised);
      _fakeDescription2.SetFilter(new TrueFilter());
      Assert.IsTrue(raised);
    }

    // TODO: test that only one event is raised when calling SetAs.

    [Test]
    public void SetAsNull()
    {
      // Set some non-default values:
      _description.Builder = new OrFilterBuilder();
      _fakeDescription1.SetFilter(new TrueFilter());
      _fakeDescription2.SetFilter(new TrueFilter());

      _description.SetAs(null);
      // The default builder is reset:
      Assert.IsNotNull(_description.Builder);
      Assert.IsInstanceOf<AndFilterBuilder>(_description.Builder);
      // The sub filters are affected:
      Assert.IsNull(_fakeDescription1.Filter);
      Assert.IsNull(_fakeDescription2.Filter);

      // The resulting filter is null:
      Assert.IsNull(_description.Filter);
    }

    [Test]
    public void SetAsSingleFilter()
    {
      // Set some non-default values:
      _description.Builder = new OrFilterBuilder();
      _fakeDescription1.SetFilter(new TrueFilter());
      _fakeDescription2.SetFilter(new TrueFilter());

      _description.SetAs(new FalseFilter());
      // The default builder untouched:
      Assert.IsNotNull(_description.Builder);
      Assert.IsInstanceOf<OrFilterBuilder>(_description.Builder);
      // The first filter is affected:
      Assert.IsNotNull(_fakeDescription1.Filter);
      Assert.IsInstanceOf<FalseFilter>(_fakeDescription1.Filter);
      // The second sub filter is nullified:
      Assert.IsNull(_fakeDescription2.Filter);

      // The resulting filter mimics the SetAs filter:
      Assert.IsNotNull(_description.Filter);
      Assert.IsInstanceOf<FalseFilter>(_description.Filter);
    }

    [Test]
    public void SetAsOrFilter()
    {
      // Set some opposite expected values:
      _description.Builder = new AndFilterBuilder();
      _fakeDescription1.SetFilter(new FalseFilter());
      _fakeDescription2.SetFilter(new FalseFilter());

      OrFilter filter = new OrFilter();
      filter.Add(new TrueFilter());
      filter.Add(new TrueFilter());

      _description.SetAs(filter);
      // The builder is an OrFilterBuilder:
      Assert.IsNotNull(_description.Builder);
      Assert.IsInstanceOf<OrFilterBuilder>(_description.Builder);
      // The sub filters are affected:
      Assert.IsNotNull(_fakeDescription1.Filter);
      Assert.IsInstanceOf<TrueFilter>(_fakeDescription1.Filter);
      Assert.IsNotNull(_fakeDescription2.Filter);
      Assert.IsInstanceOf<TrueFilter>(_fakeDescription2.Filter);

      // The resulting filter mimics the SetAs filter:
      OrFilter f = _description.Filter as OrFilter;
      Assert.IsNotNull(f);
      Assert.AreEqual(2, f.Filters.Count);
      TrueFilter f1 = f.Filters[0] as TrueFilter;
      Assert.IsNotNull(f1);
      TrueFilter f2 = f.Filters[1] as TrueFilter;
      Assert.IsNotNull(f2);
    }

    [Test]
    public void SetAsEmptyOrFilter()
    {
      // Set some opposite expected values:
      _description.Builder = new AndFilterBuilder();
      _fakeDescription1.SetFilter(new FalseFilter());
      _fakeDescription2.SetFilter(new FalseFilter());

      OrFilter filter = new OrFilter();

      _description.SetAs(filter);
      // The builder is an OrFilterBuilder:
      Assert.IsNotNull(_description.Builder);
      Assert.IsInstanceOf<OrFilterBuilder>(_description.Builder);
      // The sub filters are affected:
      Assert.IsNull(_fakeDescription1.Filter);
      Assert.IsNull(_fakeDescription2.Filter);

      // The resulting filter is null:
      Assert.IsNull(_description.Filter);
    }

    [Test]
    public void SetAsSingleOrFilter()
    {
      // Set some opposite expected values:
      _description.Builder = new AndFilterBuilder();
      _fakeDescription1.SetFilter(new FalseFilter());
      _fakeDescription2.SetFilter(new FalseFilter());

      OrFilter filter = new OrFilter();
      filter.Add(new TrueFilter());

      _description.SetAs(filter);
      // The builder is an OrFilterBuilder:
      Assert.IsNotNull(_description.Builder);
      Assert.IsInstanceOf<OrFilterBuilder>(_description.Builder);
      // The first sub filter is mimiced:
      Assert.IsNotNull(_fakeDescription1.Filter);
      Assert.IsInstanceOf<TrueFilter>(_fakeDescription1.Filter);
      // The second sub filter is nullified:
      Assert.IsNull(_fakeDescription2.Filter);

      // The resulting filter mimics the lone filter:
      Assert.IsNotNull(_description.Filter);
      Assert.IsInstanceOf<TrueFilter>(_description.Filter);
    }

    [Test]
    public void SetAsAndFilter()
    {
      // Set some opposite expected values:
      _description.Builder = new OrFilterBuilder();
      _fakeDescription1.SetFilter(new FalseFilter());
      _fakeDescription2.SetFilter(new FalseFilter());

      AndFilter filter = new AndFilter();
      filter.Add(new TrueFilter());
      filter.Add(new TrueFilter());

      _description.SetAs(filter);
      // The builder is an AndFilterBuilder:
      Assert.IsNotNull(_description.Builder);
      Assert.IsInstanceOf<AndFilterBuilder>(_description.Builder);
      // The sub filters are affected:
      Assert.IsNotNull(_fakeDescription1.Filter);
      Assert.IsInstanceOf<TrueFilter>(_fakeDescription1.Filter);
      Assert.IsNotNull(_fakeDescription2.Filter);
      Assert.IsInstanceOf<TrueFilter>(_fakeDescription2.Filter);

      // The resulting filter mimics the SetAs filter:
      AndFilter f = _description.Filter as AndFilter;
      Assert.IsNotNull(f);
      Assert.AreEqual(2, f.Filters.Count);
      TrueFilter f1 = f.Filters[0] as TrueFilter;
      Assert.IsNotNull(f1);
      TrueFilter f2 = f.Filters[1] as TrueFilter;
      Assert.IsNotNull(f2);
    }

    [Test]
    public void SetAsEmptyAndFilter()
    {
      // Set some opposite expected values:
      _description.Builder = new OrFilterBuilder();
      _fakeDescription1.SetFilter(new FalseFilter());
      _fakeDescription2.SetFilter(new FalseFilter());

      AndFilter filter = new AndFilter();

      _description.SetAs(filter);
      // The builder is an AndFilterBuilder:
      Assert.IsNotNull(_description.Builder);
      Assert.IsInstanceOf<AndFilterBuilder>(_description.Builder);
      // The sub filters are affected:
      Assert.IsNull(_fakeDescription1.Filter);
      Assert.IsNull(_fakeDescription2.Filter);

      // The resulting filter is null:
      Assert.IsNull(_description.Filter);
    }

    [Test]
    public void SetAsSingleAndFilter()
    {
      // Set some opposite expected values:
      _description.Builder = new OrFilterBuilder();
      _fakeDescription1.SetFilter(new FalseFilter());
      _fakeDescription2.SetFilter(new FalseFilter());

      AndFilter filter = new AndFilter();
      filter.Add(new TrueFilter());

      _description.SetAs(filter);
      // The builder is an AndFilterBuilder:
      Assert.IsNotNull(_description.Builder);
      Assert.IsInstanceOf<AndFilterBuilder>(_description.Builder);
      // The first sub filter is mimiced:
      Assert.IsNotNull(_fakeDescription1.Filter);
      Assert.IsInstanceOf<TrueFilter>(_fakeDescription1.Filter);
      // The second sub filter is nullified:
      Assert.IsNull(_fakeDescription2.Filter);

      // The resulting filter mimics the lone filter:
      Assert.IsNotNull(_description.Filter);
      Assert.IsInstanceOf<TrueFilter>(_description.Filter);
    }
  }
}
