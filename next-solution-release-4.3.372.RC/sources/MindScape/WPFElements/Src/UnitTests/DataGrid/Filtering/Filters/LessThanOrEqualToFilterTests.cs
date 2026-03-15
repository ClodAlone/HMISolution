using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Mindscape.WpfElements.WpfDataGrid;

namespace Mindscape.WpfElements.UnitTests
{
  [TestFixture]
  public class LessThanOrEqualToFilterTests
  {
    [Test]
    public void ValueProperty()
    {
      LessThanOrEqualToFilter filter = new LessThanOrEqualToFilter(40.3);
      Assert.AreEqual(40.3, filter.Value);
      // strings
      filter = new LessThanOrEqualToFilter("C");
      Assert.AreEqual("C", filter.Value);
      // DateTime
      DateTime date = DateTime.Now;
      filter = new LessThanOrEqualToFilter(date);
      Assert.AreEqual(date, filter.Value);
    }

    [Test]
    public void LessThanOrEqualToNumbers()
    {
      // Integer
      LessThanOrEqualToFilter filter = new LessThanOrEqualToFilter(7);
      Assert.IsTrue(filter.IsMatch(3));
      Assert.IsTrue(filter.IsMatch(7));
      Assert.IsFalse(filter.IsMatch(70));

      // Double
      filter = new LessThanOrEqualToFilter(-5.64);
      Assert.IsTrue(filter.IsMatch(-5.65));
      Assert.IsTrue(filter.IsMatch(-5.64));
      Assert.IsFalse(filter.IsMatch(-5.63));
    }

    [Test]
    public void LessThanOrEqualToStrings()
    {
      LessThanOrEqualToFilter filter = new LessThanOrEqualToFilter("F");
      Assert.IsTrue(filter.IsMatch("E"));
      Assert.IsTrue(filter.IsMatch("f"));
      Assert.IsTrue(filter.IsMatch("F"));
      Assert.IsFalse(filter.IsMatch("G"));
    }

    [Test]
    public void LessThanOrEqualToDateTime()
    {
      DateTime date = DateTime.Now;
      LessThanOrEqualToFilter filter = new LessThanOrEqualToFilter(date);
      Assert.IsTrue(filter.IsMatch(date.AddDays(-1)));
      Assert.IsTrue(filter.IsMatch(date));
      Assert.IsFalse(filter.IsMatch(date.AddHours(1)));
    }

    [Test]
    public void NullIsNotLessThanOrEqualToAnything()
    {
      LessThanOrEqualToFilter filter = new LessThanOrEqualToFilter(0);
      Assert.IsFalse(filter.IsMatch(null));
    }

    [Test]
    public void NonComparablesAreNotLessThanOrEqualToAnything()
    {
      LessThanOrEqualToFilter filter = new LessThanOrEqualToFilter(0);
      Assert.IsFalse(filter.IsMatch(new Object()));
    }
  }
}
