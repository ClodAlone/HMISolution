using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Mindscape.WpfElements.WpfDataGrid;

namespace Mindscape.WpfElements.UnitTests
{
  [TestFixture]
  public class LessThanOrEqualToFilterBuilderTests
  {
    private LessThanOrEqualToFilterBuilder _builder;

    [SetUp]
    public void SetUp()
    {
      _builder = new LessThanOrEqualToFilterBuilder();
    }

    [Test]
    public void DefaultName()
    {
      Assert.AreEqual("Less than or equal to", _builder.Name);
    }

    [Test]
    public void BuildFilter()
    {
      LessThanOrEqualToFilter filter = _builder.Build("Value", "Not used") as LessThanOrEqualToFilter;
      Assert.IsNotNull(filter);
      Assert.AreEqual("Value", filter.Value);
    }
  }
}
