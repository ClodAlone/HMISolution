using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Mindscape.WpfElements.WpfDataGrid;

namespace Mindscape.WpfElements.UnitTests
{
  [TestFixture]
  public class StartsWithFilterTests
  {
    [Test]
    public void ValueProperty()
    {
      StartsWithFilter filter = new StartsWithFilter("S", false);
      Assert.AreEqual("S", filter.Value);
      // Check casing is not changed
      filter = new StartsWithFilter("s", false);
      Assert.AreEqual("s", filter.Value);
      // Check more than one character
      filter = new StartsWithFilter("Str", false);
      Assert.AreEqual("Str", filter.Value);
      // Does not strip whitespace
      filter = new StartsWithFilter(" ", false);
      Assert.AreEqual(" ", filter.Value);
    }

    [Test]
    public void MatchCaseProperty()
    {
      StartsWithFilter filter = new StartsWithFilter("S", true);
      Assert.IsTrue(filter.MatchCase);

      filter = new StartsWithFilter("S", false);
      Assert.IsFalse(filter.MatchCase);
    }

    [Test]
    public void NullInputReturnsFalse()
    {
      // string matched with null input
      StartsWithFilter filter = new StartsWithFilter("S", false);
      Assert.IsFalse(filter.IsMatch(null));
      // empty string matched with null input
      filter = new StartsWithFilter("", false);
      Assert.IsFalse(filter.IsMatch(null));
    }

    [Test]
    public void NullValueReturnsFalse()
    {
      // null comparison value matched with string
      StartsWithFilter filter = new StartsWithFilter(null, false);
      Assert.IsFalse(filter.IsMatch("String"));
      // null comparison matched with null
      filter = new StartsWithFilter(null, false);
      Assert.IsFalse(filter.IsMatch(null));
      // null comparison value matched with empty string
      filter = new StartsWithFilter(null, false);
      Assert.IsFalse(filter.IsMatch(""));
    }

    [Test]
    public void MatchString_IgnoreCase()
    {
      StartsWithFilter filter = new StartsWithFilter("s", false);
      Assert.IsTrue(filter.IsMatch("String"));
      // Match more than one character
      filter = new StartsWithFilter("sTr", false);
      Assert.IsTrue(filter.IsMatch("String"));
      //Match entire string
      filter = new StartsWithFilter("StRiNg", false);
      Assert.IsTrue(filter.IsMatch("String"));
      // Does not match
      filter = new StartsWithFilter("qz", false);
      Assert.IsFalse(filter.IsMatch("String"));
    }

    [Test]
    public void MatchString_MatchCase()
    {
      StartsWithFilter filter = new StartsWithFilter("S", true);
      Assert.IsTrue(filter.IsMatch("String"));
      // Does not match case
      filter = new StartsWithFilter("s", true);
      Assert.IsFalse(filter.IsMatch("String"));
      // Does not match string
      filter = new StartsWithFilter("z", true);
      Assert.IsFalse(filter.IsMatch("String"));
      // match entire string
      filter = new StartsWithFilter("String", true);
      Assert.IsTrue(filter.IsMatch("String"));
      // Does not match case of entire string
      filter = new StartsWithFilter("sTrInG", true);
      Assert.IsFalse(filter.IsMatch("String"));
    }

    [Test]
    public void Whitespace()
    {
      StartsWithFilter filter = new StartsWithFilter(" ", true);
      Assert.IsTrue(filter.IsMatch(" String"));
      // does not contain whitespace
      Assert.IsFalse(filter.IsMatch("String"));
      // contains whitespace, but not at end
      Assert.IsFalse(filter.IsMatch("Arbitrary value"));
    }

    [Test]
    public void AlwaysStartsWithEmptyString()
    {
      StartsWithFilter filter = new StartsWithFilter("", true);
      Assert.IsTrue(filter.IsMatch("Arbitrary value"));
    }

    [Test]
    public void CanMatchObjects()
    {
      StartsWithFilter filter = new StartsWithFilter("F", false);
      Assert.IsTrue(filter.IsMatch(false)); // The object gets converted to string "false"
      // case sensitivity
      filter = new StartsWithFilter("t", true);
      Assert.IsFalse(filter.IsMatch(true));
    }
  }
}
