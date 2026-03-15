using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Mindscape.WpfElements.WpfDataGrid;

namespace Mindscape.WpfElements.UnitTests
{
  [TestFixture]
  public class EndsWithFilterBuilderTests
  {
    private EndsWithFilterBuilder _builder;

    [SetUp]
    public void SetUp()
    {
      _builder = new EndsWithFilterBuilder();
    }

    [Test]
    public void DefaultName()
    {
      Assert.AreEqual("Ends with", _builder.Name);
    }

    [Test]
    public void BuildFilter()
    {
      EndsWithFilter filter = _builder.Build("Value", "Not used", true) as EndsWithFilter;
      Assert.IsNotNull(filter);
      Assert.AreEqual("Value", filter.Value);
      Assert.IsTrue(filter.MatchCase);

      filter = _builder.Build("Different value", "Not used", false) as EndsWithFilter;
      Assert.IsNotNull(filter);
      Assert.AreEqual("Different value", filter.Value);
      Assert.IsFalse(filter.MatchCase);
    }

    [Test]
    public void BuildFilter_Null()
    {
      EndsWithFilter filter = _builder.Build(null, "Not used", true) as EndsWithFilter;
      Assert.IsNull(filter);
    }

    [Test]
    public void BuildFilter_EmptyString()
    {
      EndsWithFilter filter = _builder.Build("", "Not used", true) as EndsWithFilter;
      Assert.IsNull(filter);
    }

    [Test]
    public void BuilderFilter_Whitespace()
    {
      EndsWithFilter filter = _builder.Build(" ", "Not used", true) as EndsWithFilter;
      Assert.IsNotNull(filter);
      Assert.AreEqual(" ", filter.Value);
    }
  }
}
