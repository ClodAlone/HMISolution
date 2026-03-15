using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Mindscape.WpfElements.WpfDataGrid;

namespace Mindscape.WpfElements.UnitTests
{
  [TestFixture]
  public class LessThanFilterBuilderTests
  {
    private LessThanFilterBuilder _builder;

    [SetUp]
    public void SetUp()
    {
      _builder = new LessThanFilterBuilder();
    }

    [Test]
    public void DefaultName()
    {
      Assert.AreEqual("Less than", _builder.Name);
    }

    [Test]
    public void BuildFilter()
    {
      LessThanFilter filter = _builder.Build("b", "Not used") as LessThanFilter;
      Assert.IsNotNull(filter);
      Assert.AreEqual("b", filter.Value);
    }
  }
}
