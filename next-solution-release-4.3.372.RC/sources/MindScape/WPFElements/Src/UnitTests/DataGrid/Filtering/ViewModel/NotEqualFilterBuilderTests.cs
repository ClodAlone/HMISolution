using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Mindscape.WpfElements.WpfDataGrid;

namespace Mindscape.WpfElements.UnitTests
{
  [TestFixture]
  public class NotEqualFilterBuilderTests
  {
    private NotEqualFilterBuilder _builder;

    [SetUp]
    public void SetUp()
    {
      _builder = new NotEqualFilterBuilder();
    }

    [Test]
    public void DefaultName()
    {
      Assert.AreEqual("Does not equal", _builder.Name);
    }

    [Test]
    public void BuildFilter()
    {
      NotEqualFilter filter = _builder.Build("Value", "Not used") as NotEqualFilter;
      Assert.IsNotNull(filter);
      Assert.AreEqual("Value", filter.Value);
    }
  }
}
