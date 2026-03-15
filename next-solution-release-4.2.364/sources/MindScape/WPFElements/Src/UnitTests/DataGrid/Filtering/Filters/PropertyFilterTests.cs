using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Mindscape.WpfElements.WpfDataGrid;
using System.Windows;
using System.Reflection;

namespace Mindscape.WpfElements.UnitTests
{
  [TestFixture]
  public class PropertyFilterTests
  {
    private PropertyInfo _propertyInfo;

    [SetUp]
    public void SetUp()
    {
      _propertyInfo = typeof(Point).GetProperty("X");
      Assert.IsNotNull(_propertyInfo);
      Assert.AreEqual("X", _propertyInfo.Name);
    }

    [Test]
    public void PropertyInfoProperty()
    {
      PropertyFilter filter = new PropertyFilter(_propertyInfo, new EqualsFilter(42));
      Assert.AreSame(_propertyInfo, filter.PropertyInfo);
    }

    [Test]
    public void PropertyInfoAcceptsNull()
    {
      PropertyFilter filter = new PropertyFilter(null, new EqualsFilter(42));
      Assert.IsNull(filter.PropertyInfo);
    }

    [Test]
    public void FilterProperty()
    {
      IFilter f = new EqualsFilter(42);
      PropertyFilter filter = new PropertyFilter(_propertyInfo, f);
      Assert.AreSame(f, filter.Filter);
    }

    [Test]
    public void FilterAcceptsNull()
    {
      PropertyFilter filter = new PropertyFilter(_propertyInfo, null);
      Assert.IsNull(filter.Filter);
    }

    [Test]
    public void FilterIsUsed()
    {
      PropertyFilter filter = new PropertyFilter(_propertyInfo, new TrueFilter());
      Assert.IsTrue(filter.IsMatch(new Point()));
      filter = new PropertyFilter(_propertyInfo, new FalseFilter());
      Assert.IsFalse(filter.IsMatch(new Point()));

      filter = new PropertyFilter(_propertyInfo, new EqualsFilter(42.0)); // TODO: seems to be an anomoly with comparing 2 different numeric types that are actually the same value.
      Assert.IsTrue(filter.IsMatch(new Point(42, 1)));
      Assert.IsFalse(filter.IsMatch(new Point(1, 42)));
    }

    [Test]
    public void HandleTypeMismatch()
    {
      PropertyFilter filter = new PropertyFilter(_propertyInfo, new TrueFilter());
      Assert.IsFalse(filter.IsMatch("Arbitrary value")); // The PropertyInfo is for Point, but check against string, so return false and don't crash.
    }

    [Test]
    public void NullReturnsFalse() // Because null has no properties
    {
      PropertyFilter filter = new PropertyFilter(_propertyInfo, new TrueFilter());
      Assert.IsFalse(filter.IsMatch(null));
    }

    [Test]
    public void NullPropertyInfoReturnsTrue() // A null PropertyInfo denotes an unfinished filter, so it returns true to be ignored.
    {
      PropertyFilter filter = new PropertyFilter(null, new FalseFilter());
      Assert.IsTrue(filter.IsMatch("Arbitrary value"));
      filter = new PropertyFilter(null, new FalseFilter());
      Assert.IsTrue(filter.IsMatch(null));
    }

    [Test]
    public void NullFilterReturnsFalse() // A null Filter denotes an unfinished filter, so it returns true to be ignored.
    {
      PropertyFilter filter = new PropertyFilter(_propertyInfo, null);
      Assert.IsTrue(filter.IsMatch(new Point()));
      filter = new PropertyFilter(_propertyInfo, null);
      Assert.IsTrue(filter.IsMatch(null));
    }
  }
}
