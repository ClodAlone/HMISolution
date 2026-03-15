using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using System.Collections;
using Mindscape.WpfElements.WpfPropertyGrid.UnitTests;
using System.ComponentModel;
using Mindscape.WpfElements.WpfDataGrid;

namespace Mindscape.WpfElements.UnitTests.DataGridTests
{
  // TODO: write tests for sorting and paging working together.
  // TODO: might move these into a DataGridItemsSourceTests class.
  [TestFixture]
  public class PagingTests
  {
    private DataGridItemsSource _itemsSource;
    private IList _data;
    private DataGridPager _pager;

    public PagingTests()
    {
      _data = new List<object>();
      for (int i = 0; i < 100; i++)
      {
        _data.Add(new TestClass1() { Property1 = i, Property2 = i.ToString() });
      }
    }

    [SetUp]
    public void SetUp()
    {
      _itemsSource = new DataGridItemsSource(_data);
      _pager = new DataGridPager();
      _pager.DataGridItemsSource = _itemsSource;
    }

    [Test]
    [STAThread]
    public void DefaultPageSize()
    {
      Assert.AreEqual(0, _itemsSource.PageSize);
    }

    [Test]
    [STAThread]
    public void PageSizeProperty()
    {
      _itemsSource.PageSize = 1;
      Assert.AreEqual(1, _itemsSource.PageSize);
      _itemsSource.PageSize = 2;
      Assert.AreEqual(2, _itemsSource.PageSize);

      _itemsSource.PageSize = 37;
      Assert.AreEqual(37, _itemsSource.PageSize);
    }

    [Test]
    [Ignore("This doesn't seem like a good idea anymore because then you can not set the page size before the items are loaded.")]
    public void PageSizeCanNotBeSetLargerThanItemsCount()
    {
      _itemsSource.PageSize = 200;
      Assert.AreEqual(100, _itemsSource.PageSize);
    }

    [Test]
    [STAThread]
    public void PageCountIsOneWhenPageSizeIsZero()
    {
      _itemsSource.PageSize = 0; // This is the default, but set it anyway.
      Assert.AreEqual(1, _itemsSource.PageCount);
    }

    [Test]
    [STAThread]
    public void PageCountIsCorrect()
    {
      _itemsSource.PageSize = 1;
      Assert.AreEqual(100, _itemsSource.PageCount);
      _itemsSource.PageSize = 2;
      Assert.AreEqual(50, _itemsSource.PageCount);
      _itemsSource.PageSize = 3;
      Assert.AreEqual(34, _itemsSource.PageCount);
      _itemsSource.PageSize = 4;
      Assert.AreEqual(25, _itemsSource.PageCount);

      _itemsSource.PageSize = 42;
      Assert.AreEqual(3, _itemsSource.PageCount);

      _itemsSource.PageSize = 100;
      Assert.AreEqual(1, _itemsSource.PageCount);
    }

    [Test]
    [STAThread]
    public void DefaultPageIndex()
    {
      Assert.AreEqual(0, _itemsSource.PageIndex);
    }

    [Test]
    [STAThread]
    public void PageIndexCanNotBeSetLessThanZero()
    {
      _itemsSource.PageIndex = -29; // less than zero
      Assert.AreEqual(0, _itemsSource.PageIndex);

      _itemsSource.PageSize = 25;
      _itemsSource.PageIndex = -13; // less than zero
      Assert.AreEqual(0, _itemsSource.PageIndex);
    }

    [Test]
    [STAThread]
    public void PageIndexCanNotBeSetGreaterThanPageCount()
    {
      _itemsSource.PageIndex = 3; // greater than page count (0)
      Assert.AreEqual(0, _itemsSource.PageIndex);

      _itemsSource.PageSize = 25;
      _itemsSource.PageIndex = 53; // greater than page count (4);
      Assert.AreEqual(4, _itemsSource.PageCount);
      Assert.AreEqual(3, _itemsSource.PageIndex);
    }

    [Test]
    [STAThread]
    public void AllItemsDisplayedByDefault()
    {
      int index = 0;
      foreach (object o in _itemsSource)
      {
        Assert.AreSame(o, _data[index]);
        index++;
      }
      Assert.AreEqual(100, index);
    }

    [Test]
    [STAThread]
    public void ChangingPageSizeDisplaysFirstPage()
    {
      _itemsSource.PageSize = 10;
      int index = 0;
      foreach (object o in _itemsSource)
      {
        Assert.AreSame(o, _data[index]);
        index++;
      }
      Assert.AreEqual(10, index);
    }

    [Test]
    [STAThread]
    public void ChangingPageIndexDisplaysCorrectItems()
    {
      _itemsSource.PageSize = 10;
      _itemsSource.PageIndex = 3; // page 4
      int index = 0;
      foreach (object o in _itemsSource)
      {
        Assert.AreSame(o, _data[index + 30]); // +30 skips first 3 pages
        index++;
      }
      Assert.AreEqual(10, index);

      _itemsSource.PageIndex = 9; // last page - page 10
      index = 0;
      foreach (object o in _itemsSource)
      {
        Assert.AreSame(o, _data[index + 90]); // skip first 9 pages
        index++;
      }
      Assert.AreEqual(10, index);
    }

    [Test]
    [STAThread]
    public void LastPageSmallerThanPageSizeIsCorrect()
    {
      _itemsSource.PageSize = 49;
      _itemsSource.PageIndex = 2; // last page - page 3
      int index = 0;
      foreach (object o in _itemsSource)
      {
        Assert.AreSame(o, _data[index + 98]); // skip first 2 pages
        index++;
      }
      Assert.AreEqual(2, index); // page size is 49, but last page only has the 2 remaining items.
    }

    [Test]
    [STAThread]
    public void DecreasingPageCountAdjustsPageIndexIfNeeded()
    {
      _itemsSource.PageSize = 1; // 1 item per page - 100 pages.
      _itemsSource.PageIndex = 98;
      Assert.AreEqual(100, _itemsSource.PageCount);
      Assert.AreEqual(98, _itemsSource.PageIndex);

      _itemsSource.PageSize = 2; // 2 items per page - 50 pages => index 98 is now incorrect
      Assert.AreEqual(50, _itemsSource.PageCount);
      Assert.AreEqual(49, _itemsSource.PageIndex); // PageIndex has automatically been adjusted due to the decrease in page count.
    }

    [Test]
    [STAThread]
    public void RaisePageCountChanged()
    {
      IList changedProperties = new List<string>();
      _itemsSource.PropertyChanged += delegate(object sender, PropertyChangedEventArgs e)
      {
        changedProperties.Add(e.PropertyName);
      };
      _itemsSource.PageSize = 10;
      Assert.Contains("PageCount", changedProperties);
    }
  }
}
