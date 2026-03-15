using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Mindscape.WpfElements.WpfDataGrid;

namespace Mindscape.WpfElements.UnitTests
{
  [TestFixture]
  public class LessThanFilterTests
  {
    [Test]
    public void ValueProperty()
    {
      LessThanFilter filter = new LessThanFilter(40.3);
      Assert.AreEqual(40.3, filter.Value);
      // strings
      filter = new LessThanFilter("C");
      Assert.AreEqual("C", filter.Value);
      // DateTime
      DateTime date = DateTime.Now;
      filter = new LessThanFilter(date);
      Assert.AreEqual(date, filter.Value);
    }

    [Test]
    public void LessThanNumbers()
    {
      // Integer
      LessThanFilter filter = new LessThanFilter(7);
      Assert.IsTrue(filter.IsMatch(3));
      Assert.IsFalse(filter.IsMatch(7));
      Assert.IsFalse(filter.IsMatch(70));
      
      // Double
      filter = new LessThanFilter(-5.64);
      Assert.IsTrue(filter.IsMatch(-5.65));
      Assert.IsFalse(filter.IsMatch(-5.64));
      Assert.IsFalse(filter.IsMatch(-5.63));
    }

    [Test]
    public void LessThanStrings()
    {
      LessThanFilter filter = new LessThanFilter("F");
      Assert.IsTrue(filter.IsMatch("E"));
      Assert.IsTrue(filter.IsMatch("f"));
      Assert.IsFalse(filter.IsMatch("F"));
      Assert.IsFalse(filter.IsMatch("G"));
    }

    [Test]
    public void LessThanDateTime()
    {
      DateTime date = DateTime.Now;
      LessThanFilter filter = new LessThanFilter(date);
      Assert.IsTrue(filter.IsMatch(date.AddDays(-1)));
      Assert.IsFalse(filter.IsMatch(date));
      Assert.IsFalse(filter.IsMatch(date.AddHours(1)));
    }

    [Test]
    public void NullIsNotLessThanAnything()
    {
      LessThanFilter filter = new LessThanFilter(0);
      Assert.IsFalse(filter.IsMatch(null));
    }

    [Test]
    public void NonComparablesAreNotLessThanAnything()
    {
      LessThanFilter filter = new LessThanFilter(0);
      Assert.IsFalse(filter.IsMatch(new Object()));
    }
  }
}
