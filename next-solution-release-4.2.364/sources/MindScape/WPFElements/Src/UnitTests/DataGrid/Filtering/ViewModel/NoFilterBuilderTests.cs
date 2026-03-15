using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Mindscape.WpfElements.WpfDataGrid;

namespace Mindscape.WpfElements.UnitTests
{
  [TestFixture]
  public class NoFilterBuilderTests
  {
    private NoFilterBuilder _builder;

    [SetUp]
    public void SetUp()
    {
      _builder = new NoFilterBuilder();
    }

    [Test]
    public void DefaultName()
    {
      Assert.AreEqual("", _builder.Name);
    }

    [Test]
    public void AlwaysReturnsNull()
    {
      IFilter filter = _builder.Build(1, 2);
      Assert.IsNull(filter);
    }
  }
}
