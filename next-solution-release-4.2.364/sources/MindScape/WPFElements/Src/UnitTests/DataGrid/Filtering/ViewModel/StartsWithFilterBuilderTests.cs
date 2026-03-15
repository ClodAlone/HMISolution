using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Mindscape.WpfElements.WpfDataGrid;

namespace Mindscape.WpfElements.UnitTests
{
  [TestFixture]
  public class StartsWithFilterBuilderTests
  {
    private StartsWithFilterBuilder _builder;

    [SetUp]
    public void SetUp()
    {
      _builder = new StartsWithFilterBuilder();
    }

    [Test]
    public void DefaultName()
    {
      Assert.AreEqual("Starts with", _builder.Name);
    }

    [Test]
    public void BuildFilter()
    {
      StartsWithFilter filter = _builder.Build("Value", "Not used", true) as StartsWithFilter;
      Assert.IsNotNull(filter);
      Assert.AreEqual("Value", filter.Value);
      Assert.IsTrue(filter.MatchCase);

      filter = _builder.Build("Different value", "Not used", false) as StartsWithFilter;
      Assert.IsNotNull(filter);
      Assert.AreEqual("Different value", filter.Value);
      Assert.IsFalse(filter.MatchCase);
    }

    [Test]
    public void BuildFilter_Null()
    {
      StartsWithFilter filter = _builder.Build(null, "Not used", true) as StartsWithFilter;
      Assert.IsNull(filter);
    }

    [Test]
    public void BuildFilter_EmptyString()
    {
      StartsWithFilter filter = _builder.Build("", "Not used", true) as StartsWithFilter;
      Assert.IsNull(filter);
    }

    [Test]
    public void BuilderFilter_Whitespace()
    {
      StartsWithFilter filter = _builder.Build(" ", "Not used", true) as StartsWithFilter;
      Assert.IsNotNull(filter);
      Assert.AreEqual(" ", filter.Value);
    }
  }
}
