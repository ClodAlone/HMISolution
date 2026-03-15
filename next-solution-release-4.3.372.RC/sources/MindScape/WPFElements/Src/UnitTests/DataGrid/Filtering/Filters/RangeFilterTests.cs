using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Mindscape.WpfElements.WpfDataGrid;

namespace Mindscape.WpfElements.UnitTests
{
  [TestFixture]
  public class RangeFilterTests
  {
    [Test]
    public void StartProperty()
    {
      RangeFilter filter = new RangeFilter(40.3, 70.0);
      Assert.AreEqual(40.3, filter.Start);
      // strings
      filter = new RangeFilter("C", "E");
      Assert.AreEqual("C", filter.Start);
      // DateTime
      DateTime date = DateTime.Now;
      filter = new RangeFilter(date, date.AddHours(1));
      Assert.AreEqual(date, filter.Start);
    }

    [Test]
    public void EndProperty()
    {
      RangeFilter filter = new RangeFilter(40.3, 70.0);
      Assert.AreEqual(70.0, filter.End);
      // strings
      filter = new RangeFilter("C", "E");
      Assert.AreEqual("E", filter.End);
      // DateTime
      DateTime date = DateTime.Now;
      filter = new RangeFilter(date, date.AddHours(1));
      Assert.AreEqual(date.AddHours(1), filter.End);
    }

    [Test]
    public void SwapPropertiesIfNecessary()
    {
      RangeFilter filter = new RangeFilter(70.0, 40.3);
      Assert.AreEqual(40.3, filter.Start);
      Assert.AreEqual(70.0, filter.End);
      // strings
      filter = new RangeFilter("E", "C");
      Assert.AreEqual("C", filter.Start);
      Assert.AreEqual("E", filter.End);
      // DateTime
      DateTime date = DateTime.Now;
      filter = new RangeFilter(date.AddHours(1), date);
      Assert.AreEqual(date, filter.Start);
      Assert.AreEqual(date.AddHours(1), filter.End);
    }

    [Test]
    public void AcceptsSameStartAndEnd()
    {
      RangeFilter filter = new RangeFilter(2.3, 2.3);
      Assert.AreEqual(2.3, filter.Start);
      Assert.AreEqual(2.3, filter.End);

      filter = new RangeFilter("c", "c");
      Assert.AreEqual("c", filter.Start);
      Assert.AreEqual("c", filter.End);

      DateTime date = DateTime.Now;
      filter = new RangeFilter(date, date);
      Assert.AreEqual(date, filter.Start);
      Assert.AreEqual(date, filter.End);
    }

    [Test]
    public void Numbers()
    {
      // Integer
      RangeFilter filter = new RangeFilter(7, 14);
      Assert.IsTrue(filter.IsMatch(10));
      Assert.IsTrue(filter.IsMatch(7));
      Assert.IsTrue(filter.IsMatch(14));
      Assert.IsFalse(filter.IsMatch(4));
      Assert.IsFalse(filter.IsMatch(70));
      // Double
      filter = new RangeFilter(-5.64, 9.78);
      Assert.IsTrue(filter.IsMatch(0.3));
      Assert.IsTrue(filter.IsMatch(-5.64));
      Assert.IsTrue(filter.IsMatch(9.78));
      Assert.IsFalse(filter.IsMatch(-5.65));
      Assert.IsFalse(filter.IsMatch(9.79));
      // Same start and end
      filter = new RangeFilter(2.3, 2.3);
      Assert.IsTrue(filter.IsMatch(2.3));
      Assert.IsFalse(filter.IsMatch(2.0));
      Assert.IsFalse(filter.IsMatch(3.0));
    }

    [Test]
    public void Strings()
    {
      RangeFilter filter = new RangeFilter("b", "f");
      Assert.IsTrue(filter.IsMatch("d"));
      Assert.IsTrue(filter.IsMatch("b"));
      Assert.IsTrue(filter.IsMatch("f"));
      Assert.IsTrue(filter.IsMatch("D"));
      Assert.IsFalse(filter.IsMatch("a"));
      Assert.IsFalse(filter.IsMatch("h"));
      // same start and end
      filter = new RangeFilter("c", "c");
      Assert.IsTrue(filter.IsMatch("c"));
      Assert.IsFalse(filter.IsMatch("a"));
      Assert.IsFalse(filter.IsMatch("f"));
    }

    [Test]
    public void DateTimes()
    {
      DateTime date = DateTime.Now;
      RangeFilter filter = new RangeFilter(date, date.AddDays(1));
      Assert.IsTrue(filter.IsMatch(date.AddHours(1)));
      Assert.IsTrue(filter.IsMatch(date));
      Assert.IsTrue(filter.IsMatch(date.AddDays(1)));
      Assert.IsFalse(filter.IsMatch(date.AddHours(-1)));
      Assert.IsFalse(filter.IsMatch(date.AddDays(2)));
      // same start and end
      filter = new RangeFilter(date, date);
      Assert.IsTrue(filter.IsMatch(date));
      Assert.IsFalse(filter.IsMatch(date.AddHours(-1)));
      Assert.IsFalse(filter.IsMatch(date.AddHours(1)));
    }

    [Test]
    public void NullIsNotBetweenAnything()
    {
      RangeFilter filter = new RangeFilter(0, 1);
      Assert.IsFalse(filter.IsMatch(null));
    }

    [Test]
    public void NonComparablesAreNotBetweenAnything()
    {
      RangeFilter filter = new RangeFilter(0, 1);
      Assert.IsFalse(filter.IsMatch(new Object()));
    }
  }
}
