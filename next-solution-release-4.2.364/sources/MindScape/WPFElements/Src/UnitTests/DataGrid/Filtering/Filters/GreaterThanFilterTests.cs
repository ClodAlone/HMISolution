using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Mindscape.WpfElements.WpfDataGrid;

namespace Mindscape.WpfElements.UnitTests
{
  [TestFixture]
  public class GreaterThanFilterTests
  {
    [Test]
    public void ValueProperty()
    {
      GreaterThanFilter filter = new GreaterThanFilter(40.3);
      Assert.AreEqual(40.3, filter.Value);
      // strings
      filter = new GreaterThanFilter("C");
      Assert.AreEqual("C", filter.Value);
      // DateTime
      DateTime date = DateTime.Now;
      filter = new GreaterThanFilter(date);
      Assert.AreEqual(date, filter.Value);
    }

    [Test]
    public void GreaterThanNumbers()
    {
      // Integer
      GreaterThanFilter filter = new GreaterThanFilter(7);
      Assert.IsTrue(filter.IsMatch(70));
      Assert.IsFalse(filter.IsMatch(7));
      Assert.IsFalse(filter.IsMatch(3));
      // Double
      filter = new GreaterThanFilter(-5.64);
      Assert.IsTrue(filter.IsMatch(-5.63));
      Assert.IsFalse(filter.IsMatch(-5.64));
      Assert.IsFalse(filter.IsMatch(-5.65));
    }

    [Test]
    public void GreaterThanStrings()
    {
      GreaterThanFilter filter = new GreaterThanFilter("f");
      Assert.IsTrue(filter.IsMatch("g"));
      Assert.IsTrue(filter.IsMatch("F"));
      Assert.IsFalse(filter.IsMatch("f"));
      Assert.IsFalse(filter.IsMatch("e"));
    }

    [Test]
    public void GreaterThanDateTime()
    {
      DateTime date = DateTime.Now;
      GreaterThanFilter filter = new GreaterThanFilter(date);
      Assert.IsTrue(filter.IsMatch(date.AddDays(1)));
      Assert.IsFalse(filter.IsMatch(date));
      Assert.IsFalse(filter.IsMatch(date.AddHours(-1)));
    }

    [Test]
    public void NullIsNotGreaterThanAnything()
    {
      GreaterThanFilter filter = new GreaterThanFilter(0);
      Assert.IsFalse(filter.IsMatch(null));
    }

    [Test]
    public void NonComparablesAreNotGreaterThanAnything()
    {
      GreaterThanFilter filter = new GreaterThanFilter(0);
      Assert.IsFalse(filter.IsMatch(new Object()));
    }
  }
}
