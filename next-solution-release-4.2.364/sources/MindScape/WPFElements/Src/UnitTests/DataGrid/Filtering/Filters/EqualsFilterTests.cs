using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Mindscape.WpfElements.WpfDataGrid;
using System.Windows;

namespace Mindscape.WpfElements.UnitTests
{
  [TestFixture]
  public class EqualsFilterTests
  {
    [Test]
    public void ValueProperty()
    {
      EqualsFilter filter = new EqualsFilter("String");
      Assert.AreEqual("String", filter.Value);
      // Does not strip whitespace from strings
      filter = new EqualsFilter("String  ");
      Assert.AreEqual("String  ", filter.Value);
      // numbers
      filter = new EqualsFilter(3.52);
      Assert.AreEqual(3.52, filter.Value);
    }

    [Test]
    public void AcceptsNullValue()
    {
      EqualsFilter filter = new EqualsFilter(null);
      Assert.IsNull(filter.Value);
    }

    [Test]
    public void EqualTo()
    {
      EqualsFilter filter = new EqualsFilter("Arbitrary value");
      Assert.IsTrue(filter.IsMatch("Arbitrary value"));
      // numbers
      filter = new EqualsFilter(6.49);
      Assert.IsTrue(filter.IsMatch(6.49));
      // enums
      filter = new EqualsFilter(Visibility.Hidden);
      Assert.IsTrue(filter.IsMatch(Visibility.Hidden));
      // Objects
      filter = new EqualsFilter(new Point(3.6, 65.7));
      Assert.IsTrue(filter.IsMatch(new Point(3.6, 65.7)));
    }

    [Test]
    public void DoesNotEqual()
    {
      EqualsFilter filter = new EqualsFilter("Arbitrary value");
      Assert.IsFalse(filter.IsMatch("ArbitraryValue"));
      // numbers
      filter = new EqualsFilter(6.49);
      Assert.IsFalse(filter.IsMatch(94.6));
      // enums
      filter = new EqualsFilter(Visibility.Hidden);
      Assert.IsFalse(filter.IsMatch(Visibility.Collapsed));
      // Objects
      filter = new EqualsFilter(new Point(3.6, 65.7));
      Assert.IsFalse(filter.IsMatch(new Point()));
    }

    [Test]
    public void NullEqualsNull()
    {
      EqualsFilter filter = new EqualsFilter(null);
      Assert.IsTrue(filter.IsMatch(null));
    }

    [Test]
    public void NullInputBehavesNicely()
    {
      EqualsFilter filter = new EqualsFilter(new Point());
      Assert.IsFalse(filter.IsMatch(null));
    }

    [Test]
    public void NullValueBehavesNicely()
    {
      EqualsFilter filter = new EqualsFilter(null);
      Assert.IsFalse(filter.IsMatch(14.6));
    }
  }
}
