using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Mindscape.WpfElements.WpfDataGrid;

namespace Mindscape.WpfElements.UnitTests
{
  [TestFixture]
  public class GreaterThanOrEqualToFilterBuilderTests
  {
    private GreaterThanOrEqualToFilterBuilder _builder;

    [SetUp]
    public void SetUp()
    {
      _builder = new GreaterThanOrEqualToFilterBuilder();
    }

    [Test]
    public void DefautName()
    {
      Assert.AreEqual("Greater than or equal to", _builder.Name);
    }

    [Test]
    public void BuildFilter()
    {
      GreaterThanOrEqualToFilter filter = _builder.Build("Value", "Not used") as GreaterThanOrEqualToFilter;
      Assert.IsNotNull(filter);
      Assert.AreEqual("Value", filter.Value);
    }
  }
}
