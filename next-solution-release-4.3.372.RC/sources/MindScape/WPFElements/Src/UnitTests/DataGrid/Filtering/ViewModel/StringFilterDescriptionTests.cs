using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mindscape.WpfElements.WpfDataGrid;
using NUnit.Framework;

namespace Mindscape.WpfElements.UnitTests
{
  [TestFixture]
  public class StringFilterDescriptionTests
  {
    private StringFilterDescription _description;

    [SetUp]
    public void SetUp()
    {
      _description = new StringFilterDescription();
    }

    [Test]
    public void FirstValueProperty_DefaultValue()
    {
      Assert.AreEqual("", _description.FirstValue);
    }

    [Test]
    public void FirstValueProperty()
    {
      _description.FirstValue = "Pear";
      Assert.AreEqual("Pear", _description.FirstValue);
    }

    [Test]
    public void FirstValueProperty_UpdatesFilter()
    {
      _description.FirstValue = "P";
      StartsWithFilter filter = _description.Filter as StartsWithFilter;
      Assert.IsNotNull(filter);
      Assert.AreEqual("P", filter.Value);
    }

    [Test]
    public void FirstValueProperty_RaisesPropertyChanged()
    {
      bool raised = false;
      _description.PropertyChanged += (o, e) => { if ("FirstValue".Equals(e.PropertyName)) { raised = true; } };
      _description.FirstValue = "R";
      Assert.IsTrue(raised);
    }

    [Test]
    public void SecondValueProperty_DefaultValue()
    {
      Assert.AreEqual("", _description.SecondValue);
    }

    [Test]
    public void SecondValueProperty()
    {
      _description.SecondValue = "Pear";
      Assert.AreEqual("Pear", _description.SecondValue);
    }

    [Test]
    public void SecondValueProperty_UpdatesFilter()
    {
      _description.FirstValue = "P"; // Need to at least set FirstValue so filter is not null
      _description.SecondValue = "Q";
      StartsWithFilter filter = _description.Filter as StartsWithFilter;
      Assert.IsNotNull(filter);
      // TODO: Check that the second value was applied when we have a 2-value string filter builder such as range filter.
    }

    [Test]
    public void SecondValueProperty_RaisesPropertyChanged()
    {
      bool raised = false;
      _description.PropertyChanged += (o, e) => { if ("SecondValue".Equals(e.PropertyName)) { raised = true; } };
      _description.SecondValue = "R";
      Assert.IsTrue(raised);
    }

    [Test]
    public void MatchCaseProperty_DefaultValue()
    {
      Assert.IsFalse(_description.MatchCase);
    }

    [Test]
    public void MatchCaseProperty()
    {
      _description.MatchCase = true;
      Assert.IsTrue(_description.MatchCase);
    }

    [Test]
    public void MatchCaseProperty_UpdatesFilter()
    {
      _description.FirstValue = "P"; // Need to at least set FirstValue so filter is not null
      _description.MatchCase = true;
      StartsWithFilter filter = _description.Filter as StartsWithFilter;
      Assert.IsNotNull(filter);
      Assert.IsTrue(filter.MatchCase);
    }

    [Test]
    public void MatchCaseProperty_RaisesPropertyChanged()
    {
      bool raised = false;
      _description.PropertyChanged += (o, e) => { if ("MatchCase".Equals(e.PropertyName)) { raised = true; } };
      _description.MatchCase = true;
      Assert.IsTrue(raised);
    }

    [Test]
    public void BuildProperty_DefaultValue()
    {
      StartsWithFilterBuilder builder = _description.Builder as StartsWithFilterBuilder;
      Assert.IsNotNull(builder);
    }

    [Test]
    public void BuilderProperty()
    {
      EndsWithFilterBuilder builder = new EndsWithFilterBuilder();
      _description.Builder = builder;
      Assert.AreSame(builder, _description.Builder);
    }

    [Test]
    public void BuilderProperty_UpdatesFilter()
    {
      _description.FirstValue = "P"; // Need to at least set FirstValue so filter is not null
      _description.Builder = new EndsWithFilterBuilder();
      EndsWithFilter filter = _description.Filter as EndsWithFilter;
      Assert.IsNotNull(filter);
    }

    [Test]
    public void BuilderProperty_RaisesPropertyChanged()
    {
      bool raised = false;
      _description.PropertyChanged += (o, e) => { if ("Builder".Equals(e.PropertyName)) { raised = true; } };
      _description.Builder = new EndsWithFilterBuilder();
      Assert.IsTrue(raised);
    }

    [Test]
    public void NullBuilderMeansNullFilter()
    {
      _description.FirstValue = "P"; // Need to at least set FirstValue
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
      _description.FirstValue = "R";
      Assert.IsTrue(raised);

      raised = false;
      _description.SecondValue = "Q";
      Assert.IsTrue(raised);

      raised = false;
      _description.MatchCase = true;
      Assert.IsTrue(raised);

      raised = false;
      _description.Builder = null;
      Assert.IsTrue(raised);
    }

    // TODO: test that only one event is raised when calling SetAs.

    [Test]
    public void SetAsNull()
    {
      // Set some non-default values
      _description.Builder = new EndsWithFilterBuilder();
      _description.FirstValue = "P";
      _description.SecondValue = "Q";
      _description.MatchCase = true;

      _description.SetAs(null);
      // The builder is a StartsWithFilterBuilder (the default)
      Assert.IsNotNull(_description.Builder);
      Assert.IsInstanceOf<StartsWithFilterBuilder>(_description.Builder);
      // The default values are restored
      Assert.AreEqual("", _description.FirstValue);
      Assert.AreEqual("", _description.SecondValue);
      Assert.IsFalse(_description.MatchCase);

      // The resulting filter is null
      Assert.IsNull(_description.Filter);
    }

    [Test]
    public void SetAsUnsupportedFilter()
    {
      // Set some non-default values
      _description.Builder = new EndsWithFilterBuilder();
      _description.FirstValue = "P";
      _description.SecondValue = "Q";
      _description.MatchCase = true;

      FalseFilter filter = new FalseFilter();
      _description.SetAs(filter);

      // The builder is a StartsWithFilterBuilder (the default)
      Assert.IsNotNull(_description.Builder);
      Assert.IsInstanceOf<StartsWithFilterBuilder>(_description.Builder);
      // The default values are restored
      Assert.AreEqual("", _description.FirstValue);
      Assert.AreEqual("", _description.SecondValue);
      Assert.IsFalse(_description.MatchCase);

      // The resulting filter is null
      Assert.IsNull(_description.Filter);
    }

    [Test]
    public void SetAsStartsWithFilter()
    {
      // Set some opposite expected values
      _description.Builder = new EndsWithFilterBuilder();
      _description.FirstValue = "P";
      _description.SecondValue = "Q";
      _description.MatchCase = false;

      StartsWithFilter filter = new StartsWithFilter("S", true);
      _description.SetAs(filter);
      //The builder is a StartsWithFilterBuilder
      Assert.IsNotNull(_description.Builder);
      Assert.IsInstanceOf<StartsWithFilterBuilder>(_description.Builder);
      // The values are copied
      Assert.AreEqual("S", _description.FirstValue);
      Assert.IsTrue(_description.MatchCase);
      // The second value is reverted to default
      Assert.AreEqual("", _description.SecondValue);

      // The resulting filter is mimicked
      StartsWithFilter f = _description.Filter as StartsWithFilter;
      Assert.IsNotNull(f);
      Assert.AreEqual("S", f.Value);
      Assert.IsTrue(f.MatchCase);
    }

    [Test]
    public void SetAsEndsWithFilter()
    {
      // Set some opposite expected values
      _description.Builder = new ContainsFilterBuilder();
      _description.FirstValue = "P";
      _description.SecondValue = "Q";
      _description.MatchCase = false;

      EndsWithFilter filter = new EndsWithFilter("S", true);
      _description.SetAs(filter);
      //The builder is a EndsWithFilterBuilder
      Assert.IsNotNull(_description.Builder);
      Assert.IsInstanceOf<EndsWithFilterBuilder>(_description.Builder);
      // The values are copied
      Assert.AreEqual("S", _description.FirstValue);
      Assert.IsTrue(_description.MatchCase);
      // The second value is reverted to default
      Assert.AreEqual("", _description.SecondValue);

      // The resulting filter is mimicked
      EndsWithFilter f = _description.Filter as EndsWithFilter;
      Assert.IsNotNull(f);
      Assert.AreEqual("S", f.Value);
      Assert.IsTrue(f.MatchCase);
    }

    [Test]
    public void SetAsContainsFilter()
    {
      // Set some opposite expected values
      _description.Builder = new EndsWithFilterBuilder();
      _description.FirstValue = "P";
      _description.SecondValue = "Q";
      _description.MatchCase = false;

      ContainsFilter filter = new ContainsFilter("S", true);
      _description.SetAs(filter);
      //The builder is a ContainsFilterBuilder
      Assert.IsNotNull(_description.Builder);
      Assert.IsInstanceOf<ContainsFilterBuilder>(_description.Builder);
      // The values are copied
      Assert.AreEqual("S", _description.FirstValue);
      Assert.IsTrue(_description.MatchCase);
      // The second value is reverted to default
      Assert.AreEqual("", _description.SecondValue);

      // The resulting filter is mimicked
      ContainsFilter f = _description.Filter as ContainsFilter;
      Assert.IsNotNull(f);
      Assert.AreEqual("S", f.Value);
      Assert.IsTrue(f.MatchCase);
    }
  }
}
