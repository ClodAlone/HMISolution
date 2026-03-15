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
  public class NotEqualFilterTests
  {
    [Test]
    public void ValueProperty()
    {
      NotEqualFilter filter = new NotEqualFilter("String");
      Assert.AreEqual("String", filter.Value);
      // Does not strip whitespace from strings
      filter = new NotEqualFilter("String  ");
      Assert.AreEqual("String  ", filter.Value);
      // numbers
      filter = new NotEqualFilter(3.52);
      Assert.AreEqual(3.52, filter.Value);
    }

    [Test]
    public void AcceptsNullValue()
    {
      NotEqualFilter filter = new NotEqualFilter(null);
      Assert.IsNull(filter.Value);
    }

    [Test]
    public void NotEqualValuesReturnTrue()
    {
      NotEqualFilter filter = new NotEqualFilter("Arbitrary value");
      Assert.IsTrue(filter.IsMatch("ArbitraryValue"));
      // numbers
      filter = new NotEqualFilter(6.49);
      Assert.IsTrue(filter.IsMatch(94.6));
      // enums
      filter = new NotEqualFilter(Visibility.Hidden);
      Assert.IsTrue(filter.IsMatch(Visibility.Collapsed));
      // Objects
      filter = new NotEqualFilter(new Point(3.6, 65.7));
      Assert.IsTrue(filter.IsMatch(new Point()));
    }

    [Test]
    public void EqualValuesReturnFalse()
    {
      NotEqualFilter filter = new NotEqualFilter("Arbitrary value");
      Assert.IsFalse(filter.IsMatch("Arbitrary value"));
      // numbers
      filter = new NotEqualFilter(6.49);
      Assert.IsFalse(filter.IsMatch(6.49));
      // enums
      filter = new NotEqualFilter(Visibility.Hidden);
      Assert.IsFalse(filter.IsMatch(Visibility.Hidden));
      // Objects
      filter = new NotEqualFilter(new Point(3.6, 65.7));
      Assert.IsFalse(filter.IsMatch(new Point(3.6, 65.7)));
    }

    [Test]
    public void TwoNullsReturnFalse()
    {
      NotEqualFilter filter = new NotEqualFilter(null);
      Assert.IsFalse(filter.IsMatch(null));
    }

    [Test]
    public void NullInputBehavesNicely()
    {
      NotEqualFilter filter = new NotEqualFilter(new Point());
      Assert.IsTrue(filter.IsMatch(null));
    }

    [Test]
    public void NullValueBehavesNicely()
    {
      NotEqualFilter filter = new NotEqualFilter(null);
      Assert.IsTrue(filter.IsMatch(14.6));
    }
  }
}
