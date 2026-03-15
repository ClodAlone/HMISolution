using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Mindscape.WpfElements.WpfDataGrid;

namespace Mindscape.WpfElements.UnitTests
{
  [TestFixture]
  public class GreaterThanFilterBuilderTests
  {
    private GreaterThanFilterBuilder _builder;

    [SetUp]
    public void SetUp()
    {
      _builder = new GreaterThanFilterBuilder();
    }

    [Test]
    public void DefaultName()
    {
      Assert.AreEqual("Greater than", _builder.Name);
    }

    [Test]
    public void BuildFilter()
    {
      GreaterThanFilter filter = _builder.Build(23.4, 0) as GreaterThanFilter;
      Assert.IsNotNull(filter);
      Assert.AreEqual(23.4, filter.Value);
    }
  }
}
