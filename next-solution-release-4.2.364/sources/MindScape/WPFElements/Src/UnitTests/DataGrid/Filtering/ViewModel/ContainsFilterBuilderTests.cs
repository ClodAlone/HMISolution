using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Mindscape.WpfElements.WpfDataGrid;

namespace Mindscape.WpfElements.UnitTests
{
  [TestFixture]
  public class ContainsFilterBuilderTests
  {
    private ContainsFilterBuilder _builder;

    [SetUp]
    public void SetUp()
    {
      _builder = new ContainsFilterBuilder();
    }

    [Test]
    public void DefaultName()
    {
      Assert.AreEqual("Contains", _builder.Name);
    }

    [Test]
    public void BuildFilter()
    {
      ContainsFilter filter = _builder.Build("Value", "Not used", true) as ContainsFilter;
      Assert.IsNotNull(filter);
      Assert.AreEqual("Value", filter.Value);
      Assert.IsTrue(filter.MatchCase);

      filter = _builder.Build("Different value", "Not used", false) as ContainsFilter;
      Assert.IsNotNull(filter);
      Assert.AreEqual("Different value", filter.Value);
      Assert.IsFalse(filter.MatchCase);
    }

    [Test]
    public void BuildFilter_Null()
    {
      ContainsFilter filter = _builder.Build(null, "Not used", true) as ContainsFilter;
      Assert.IsNull(filter);
    }

    [Test]
    public void BuildFilter_EmptyString()
    {
      ContainsFilter filter = _builder.Build("", "Not used", true) as ContainsFilter;
      Assert.IsNull(filter);
    }

    [Test]
    public void BuilderFilter_Whitespace()
    {
      ContainsFilter filter = _builder.Build(" ", "Not used", true) as ContainsFilter;
      Assert.IsNotNull(filter);
      Assert.AreEqual(" ", filter.Value);
    }
  }
}
