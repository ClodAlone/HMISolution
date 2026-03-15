using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Mindscape.WpfElements.WpfDataGrid;

namespace Mindscape.WpfElements.UnitTests
{
  [TestFixture]
  public class ContainsFilterTests
  {
    [Test]
    public void ValueProperty()
    {
      ContainsFilter filter = new ContainsFilter("R", false);
      Assert.AreEqual("R", filter.Value);
      // Check that casing is not changed
      filter = new ContainsFilter("r", false);
      Assert.AreEqual("r", filter.Value);
    }

    [Test]
    public void MatchCaseProperty()
    {
      ContainsFilter filter = new ContainsFilter("R", true);
      Assert.IsTrue(filter.MatchCase);

      filter = new ContainsFilter("R", false);
      Assert.IsFalse(filter.MatchCase);
    }

    [Test]
    public void NullInputReturnsFalse()
    {
      // string matched with null input
      ContainsFilter filter = new ContainsFilter("R", false);
      Assert.IsFalse(filter.IsMatch(null));
      // empty string matched with null input
      filter = new ContainsFilter("", false);
      Assert.IsFalse(filter.IsMatch(null));
    }

    [Test]
    public void NullValueReturnsFalse()
    {
      // null comparison value matched with string
      ContainsFilter filter = new ContainsFilter(null, false);
      Assert.IsFalse(filter.IsMatch("String"));
      // null comparison matched with null
      filter = new ContainsFilter(null, false);
      Assert.IsFalse(filter.IsMatch(null));
      // null comparison value matched with empty string
      filter = new ContainsFilter(null, false);
      Assert.IsFalse(filter.IsMatch(""));
    }

    [Test]
    public void MatchString_IgnoreCase()
    {
      ContainsFilter filter = new ContainsFilter("R", false);
      Assert.IsTrue(filter.IsMatch("String"));

      filter = new ContainsFilter("z", false);
      Assert.IsFalse(filter.IsMatch("String"));

      filter = new ContainsFilter("sTrInG", false);
      Assert.IsTrue(filter.IsMatch("String"));
    }

    [Test]
    public void MatchString_MatchCase()
    {
      ContainsFilter filter = new ContainsFilter("R", true);
      Assert.IsFalse(filter.IsMatch("String"));

      filter = new ContainsFilter("q", true);
      Assert.IsFalse(filter.IsMatch("String"));

      filter = new ContainsFilter("String", true);
      Assert.IsTrue(filter.IsMatch("String"));
    }

    [Test]
    public void Whitespace()
    {
      ContainsFilter filter = new ContainsFilter(" ", false);
      Assert.IsTrue(filter.IsMatch("Contains whitspace"));
      // Space at end
      Assert.IsTrue(filter.IsMatch("SpaceAtEnd "));
      // Space at start
      Assert.IsTrue(filter.IsMatch(" SpaceAtStart"));
    }

    [Test]
    public void AlwaysContainsEmptyString()
    {
      ContainsFilter filter = new ContainsFilter("", false);
      Assert.IsTrue(filter.IsMatch("Arbitrary value"));
    }

    [Test]
    public void CanMatchObjects()
    {
      ContainsFilter filter = new ContainsFilter("R", false);
      Assert.IsTrue(filter.IsMatch(true)); // The input gets converted into a string "true"

      filter = new ContainsFilter("R", true);
      Assert.IsFalse(filter.IsMatch(true));
    }
  }
}
