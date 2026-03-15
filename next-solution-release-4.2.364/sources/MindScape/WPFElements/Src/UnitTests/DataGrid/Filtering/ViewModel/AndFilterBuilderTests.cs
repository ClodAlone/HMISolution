using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Mindscape.WpfElements.WpfDataGrid;

namespace Mindscape.WpfElements.UnitTests
{
  [TestFixture]
  public class AndFilterBuilderTests
  {
    private AndFilterBuilder _builder;
    private IFilter _firstFilter;
    private IFilter _secondFilter;

    [SetUp]
    public void SetUp()
    {
      _builder = new AndFilterBuilder();
      _firstFilter = new TrueFilter();
      _secondFilter = new FalseFilter();
    }

    [Test]
    public void DefaultName()
    {
      Assert.AreEqual("And", _builder.Name);
    }

    [Test]
    public void TwoNullsMakeANull()
    {
      IFilter filter = _builder.Build(null, null);
      Assert.IsNull(filter);
    }

    [Test]
    public void FirstNullReturnsSecondFilter()
    {
      IFilter filter = _builder.Build(null, _secondFilter);
      Assert.AreSame(_secondFilter, filter);
    }

    [Test]
    public void SecondNullReturnsFirstFilter()
    {
      IFilter filter = _builder.Build(_firstFilter, null);
      Assert.AreSame(_firstFilter, filter);
    }

    [Test]
    public void BuildAndFilter()
    {
      AndFilter filter = _builder.Build(_firstFilter, _secondFilter) as AndFilter;
      Assert.IsNotNull(filter);
      Assert.AreEqual(2, filter.Filters.Count);
      Assert.AreSame(_firstFilter, filter.Filters[0]);
      Assert.AreSame(_secondFilter, filter.Filters[1]);
    }
  }
}
