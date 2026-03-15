using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Mindscape.WpfElements.WpfDataGrid;

namespace Mindscape.WpfElements.UnitTests
{
  [TestFixture]
  public class EqualsFilterBuilderTests
  {
    private EqualsFilterBuilder _builder;

    [SetUp]
    public void SetUp()
    {
      _builder = new EqualsFilterBuilder();
    }

    [Test]
    public void DefaultName()
    {
      Assert.AreEqual("Equals", _builder.Name);
    }

    [Test]
    public void BuildFilter()
    {
      EqualsFilter filter = _builder.Build("Value", "Not used") as EqualsFilter;
      Assert.IsNotNull(filter);
      Assert.AreEqual("Value", filter.Value);
    }
  }
}
