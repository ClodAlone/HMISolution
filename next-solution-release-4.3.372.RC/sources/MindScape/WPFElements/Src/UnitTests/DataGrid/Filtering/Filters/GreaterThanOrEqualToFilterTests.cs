using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Mindscape.WpfElements.WpfDataGrid;

namespace Mindscape.WpfElements.UnitTests
{
  [TestFixture]
  public class GreaterThanOrEqualToFilterTests
  {
    [Test]
    public void ValueProperty()
    {
      GreaterThanOrEqualToFilter filter = new GreaterThanOrEqualToFilter(40.3);
      Assert.AreEqual(40.3, filter.Value);
      // strings
      filter = new GreaterThanOrEqualToFilter("C");
      Assert.AreEqual("C", filter.Value);
      // DateTime
      DateTime date = DateTime.Now;
      filter = new GreaterThanOrEqualToFilter(date);
      Assert.AreEqual(date, filter.Value);
    }

    [Test]
    public void GreaterThanOrEqualToNumbers()
    {
      // Integer
      GreaterThanOrEqualToFilter filter = new GreaterThanOrEqualToFilter(7);
      Assert.IsTrue(filter.IsMatch(70));
      Assert.IsTrue(filter.IsMatch(7));
      Assert.IsFalse(filter.IsMatch(3));
      // Double
      filter = new GreaterThanOrEqualToFilter(-5.64);
      Assert.IsTrue(filter.IsMatch(-5.63));
      Assert.IsTrue(filter.IsMatch(-5.64));
      Assert.IsFalse(filter.IsMatch(-5.65));
    }

    [Test]
    public void GreaterThanOrEqualToStrings()
    {
      GreaterThanOrEqualToFilter filter = new GreaterThanOrEqualToFilter("f");
      Assert.IsTrue(filter.IsMatch("g"));
      Assert.IsTrue(filter.IsMatch("F"));
      Assert.IsTrue(filter.IsMatch("f"));
      Assert.IsFalse(filter.IsMatch("e"));
    }

    [Test]
    public void GreaterThanOrEqualToDateTime()
    {
      DateTime date = DateTime.Now;
      GreaterThanOrEqualToFilter filter = new GreaterThanOrEqualToFilter(date);
      Assert.IsTrue(filter.IsMatch(date.AddDays(1)));
      Assert.IsTrue(filter.IsMatch(date));
      Assert.IsFalse(filter.IsMatch(date.AddHours(-1)));
    }

    [Test]
    public void NullIsNotGreaterThanOrEqualToAnything()
    {
      GreaterThanOrEqualToFilter filter = new GreaterThanOrEqualToFilter(0);
      Assert.IsFalse(filter.IsMatch(null));
    }

    [Test]
    public void NonComparablesAreNotGreaterThanOrEqualToAnything()
    {
      GreaterThanOrEqualToFilter filter = new GreaterThanOrEqualToFilter(0);
      Assert.IsFalse(filter.IsMatch(new Object()));
    }
  }
}
