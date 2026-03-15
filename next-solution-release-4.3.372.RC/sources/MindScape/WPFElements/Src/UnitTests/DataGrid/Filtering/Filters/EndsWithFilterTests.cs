using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Mindscape.WpfElements.WpfDataGrid;

namespace Mindscape.WpfElements.UnitTests
{
  [TestFixture]
  public class EndsWithFilterTests
  {
    [Test]
    public void ValueProperty()
    {
      EndsWithFilter filter = new EndsWithFilter("G", false);
      Assert.AreEqual("G", filter.Value);
      // Check casing is not changed
      filter = new EndsWithFilter("g", false);
      Assert.AreEqual("g", filter.Value);
      // Check more than one character
      filter = new EndsWithFilter("ing", false);
      Assert.AreEqual("ing", filter.Value);
      // Does not strip whitespace
      filter = new EndsWithFilter(" ", false);
      Assert.AreEqual(" ", filter.Value);
    }

    [Test]
    public void MatchCaseProperty()
    {
      EndsWithFilter filter = new EndsWithFilter("G", true);
      Assert.IsTrue(filter.MatchCase);

      filter = new EndsWithFilter("G", false);
      Assert.IsFalse(filter.MatchCase);
    }

    [Test]
    public void NullInputReturnsFalse()
    {
      // string matched with null input
      EndsWithFilter filter = new EndsWithFilter("G", false);
      Assert.IsFalse(filter.IsMatch(null));
      // empty string matched with null input
      filter = new EndsWithFilter("", false);
      Assert.IsFalse(filter.IsMatch(null));
    }

    [Test]
    public void NullValueReturnsFalse()
    {
      // null comparison value matched with string
      EndsWithFilter filter = new EndsWithFilter(null, false);
      Assert.IsFalse(filter.IsMatch("String"));
      // null comparison matched with null
      filter = new EndsWithFilter(null, false);
      Assert.IsFalse(filter.IsMatch(null));
      // null comparison value matched with empty string
      filter = new EndsWithFilter(null, false);
      Assert.IsFalse(filter.IsMatch(""));
    }

    [Test]
    public void MatchString_IgnoreCase()
    {
      EndsWithFilter filter = new EndsWithFilter("g", false);
      Assert.IsTrue(filter.IsMatch("String"));
      // Match more than one character
      filter = new EndsWithFilter("iNg", false);
      Assert.IsTrue(filter.IsMatch("String"));
      //Match entire string
      filter = new EndsWithFilter("StRiNg", false);
      Assert.IsTrue(filter.IsMatch("String"));
      // Does not match
      filter = new EndsWithFilter("qz", false);
      Assert.IsFalse(filter.IsMatch("String"));
    }

    [Test]
    public void MatchString_MatchCase()
    {
      EndsWithFilter filter = new EndsWithFilter("g", true);
      Assert.IsTrue(filter.IsMatch("String"));
      // Does not match case
      filter = new EndsWithFilter("G", true);
      Assert.IsFalse(filter.IsMatch("String"));
      // Does not match string
      filter = new EndsWithFilter("z", true);
      Assert.IsFalse(filter.IsMatch("String"));
      // match entire string
      filter = new EndsWithFilter("String", true);
      Assert.IsTrue(filter.IsMatch("String"));
      // Does not match case of entire string
      filter = new EndsWithFilter("sTrInG", true);
      Assert.IsFalse(filter.IsMatch("String"));
    }

    [Test]
    public void Whitespace()
    {
      EndsWithFilter filter = new EndsWithFilter(" ", true);
      Assert.IsTrue(filter.IsMatch("String "));
      // does not contain whitespace
      Assert.IsFalse(filter.IsMatch("String"));
      // contains whitespace, but not at end
      Assert.IsFalse(filter.IsMatch("Arbitrary value"));
    }

    [Test]
    public void AlwaysEndsWithEmptyString()
    {
      EndsWithFilter filter = new EndsWithFilter("", true);
      Assert.IsTrue(filter.IsMatch("Arbitrary value"));
    }

    [Test]
    public void CanMatchObjects()
    {
      EndsWithFilter filter = new EndsWithFilter("E", false);
      Assert.IsTrue(filter.IsMatch(false)); // The object gets converted to string "false"
      // case sensitivity
      filter = new EndsWithFilter("E", true);
      Assert.IsFalse(filter.IsMatch(true));
    }
  }
}
