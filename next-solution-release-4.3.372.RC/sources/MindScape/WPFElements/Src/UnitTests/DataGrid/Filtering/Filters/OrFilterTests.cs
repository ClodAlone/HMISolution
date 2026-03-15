using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Mindscape.WpfElements.WpfDataGrid;

namespace Mindscape.WpfElements.UnitTests
{
  [TestFixture]
  public class OrFilterTests
  {
    [Test]
    public void NewAndFilterIsEmpty()
    {
      OrFilter filter = new OrFilter();
      Assert.AreEqual(0, filter.Filters.Count);
    }

    [Test]
    public void EmptyOrFilter()
    {
      OrFilter filter = new OrFilter();

      Assert.IsTrue(filter.IsMatch("Arbitrary value"));
    }

    [Test]
    public void EmptyOrFilterAcceptsNull()
    {
      OrFilter filter = new OrFilter();

      Assert.IsTrue(filter.IsMatch(null));
    }

    [Test]
    public void SingleTrueFilter()
    {
      OrFilter filter = new OrFilter();
      filter.Add(new TrueFilter());

      Assert.IsTrue(filter.IsMatch("Arbitrary value"));
    }

    [Test]
    public void SingleFalseFilter()
    {
      OrFilter filter = new OrFilter();
      filter.Add(new FalseFilter());

      Assert.IsFalse(filter.IsMatch("Arbitrary value"));
    }

    [Test]
    public void TwoTrueFilters()
    {
      OrFilter filter = new OrFilter();
      filter.Add(new TrueFilter());
      filter.Add(new TrueFilter());

      Assert.IsTrue(filter.IsMatch("Arbitrary value"));
    }

    [Test]
    public void TwoFalseFilters()
    {
      OrFilter filter = new OrFilter();
      filter.Add(new FalseFilter());
      filter.Add(new FalseFilter());

      Assert.IsFalse(filter.IsMatch("Arbitrary value"));
    }

    [Test]
    public void TrueAndFalseFilters()
    {
      OrFilter filter = new OrFilter();
      filter.Add(new TrueFilter());
      filter.Add(new FalseFilter());

      Assert.IsTrue(filter.IsMatch("Arbitrary value"));
    }

    [Test]
    public void FalseAndTrueFilters()
    {
      OrFilter filter = new OrFilter();
      filter.Add(new FalseFilter());
      filter.Add(new TrueFilter());

      Assert.IsTrue(filter.IsMatch("Arbitrary value"));
    }

    [Test]
    public void AddFilter()
    {
      OrFilter filter = new OrFilter();
      IFilter firstFilter = new TrueFilter();
      IFilter secondFilter = new TrueFilter();
      IFilter thirdFilter = new TrueFilter();

      filter.Add(firstFilter);
      Assert.AreEqual(1, filter.Filters.Count);
      Assert.AreSame(firstFilter, filter.Filters[0]);
      // 2 filters
      filter.Add(secondFilter);
      Assert.AreEqual(2, filter.Filters.Count);
      Assert.AreSame(firstFilter, filter.Filters[0]);
      Assert.AreSame(secondFilter, filter.Filters[1]);
      // multiple filters
      filter.Add(thirdFilter);
      Assert.AreEqual(3, filter.Filters.Count);
      Assert.AreSame(firstFilter, filter.Filters[0]);
      Assert.AreSame(secondFilter, filter.Filters[1]);
      Assert.AreSame(thirdFilter, filter.Filters[2]);
    }

    [Test]
    public void CanNotAddNullFilter()
    {
      OrFilter filter = new OrFilter();
      filter.Add(null);
      Assert.AreEqual(0, filter.Filters.Count);
    }

    [Test]
    public void CanNotAddDuplicateFilter()
    {
      OrFilter filter = new OrFilter();
      IFilter subFilter = new FalseFilter();
      filter.Add(subFilter);
      filter.Add(subFilter);
      Assert.AreEqual(1, filter.Filters.Count); // Added the same filter twice, so only 1 filter is added.
      Assert.AreSame(subFilter, filter.Filters[0]);
    }
  }
}
