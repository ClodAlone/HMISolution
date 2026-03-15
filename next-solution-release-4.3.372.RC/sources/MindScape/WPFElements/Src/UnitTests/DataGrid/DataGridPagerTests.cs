using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using System.Collections;
using Mindscape.WpfElements.WpfDataGrid;

namespace Mindscape.WpfElements.UnitTests.DataGridTests
{
  // TODO: write tests for first page, previous page, next page and last page commands. Also methods and properties when they exist.
  [TestFixture]
  public class DataGridPagerTests
  {
    private DataGridItemsSource _itemsSource;
    private IList _data;
    private DataGridPager _pager;

    private DataGridPager _emptyPager;

    public DataGridPagerTests()
    {
      _data = new List<object>();
      for (int i = 0; i < 100; i++)
      {
        _data.Add(new Mindscape.WpfElements.UnitTests.DataGridTests.TestClass1() { Property1 = i, Property2 = i.ToString() });
      }
    }

    [SetUp]
    public void SetUp()
    {
      _itemsSource = new DataGridItemsSource(_data);
      _pager = new DataGridPager();
      _pager.DataGridItemsSource = _itemsSource;

      _emptyPager = new DataGridPager();
    }

    #region Empty DataGridPager Tests

    [Test]
    [STAThread]
    public void DefaultPageIndex()
    {
      Assert.AreEqual(0, _emptyPager.PageIndex);
    }

    [Test]
    [STAThread]
    public void DefaultMaxPagerButtonCount()
    {
      Assert.AreEqual(9, _emptyPager.MaxPagerButtonCount);
    }

    [Test]
    [STAThread]
    public void DefaultEllipsisMode()
    {
      Assert.AreEqual(EllipsisMode.Both, _emptyPager.EllipsisMode);
    }

    [Test]
    [STAThread]
    public void NoPagerButtons()
    {
      Assert.IsEmpty(_emptyPager.PagerButtons);
    }

    [Test]
    [STAThread]
    public void SettingSourceUpdatesPageIndexIfNeeded()
    {
      _itemsSource.PageSize = 10;
      _itemsSource.PageIndex = 3;
      _emptyPager.DataGridItemsSource = _itemsSource;
      Assert.AreEqual(3, _emptyPager.PageIndex);
    }

    #endregion // Empty DataGridPager Tests

    #region Synchronized PageIndex Tests

    [Test]
    [STAThread]
    public void SettingPagerPageIndexUpdatesSourcePageIndex()
    {
      _itemsSource.PageSize = 10;
      _pager.PageIndex = 5; // Changed PageIndex of the pager to abitrary page.
      Assert.AreEqual(5, _itemsSource.PageIndex); // The PageIndex of the source automatically updates as well.
    }

    [Test]
    [STAThread]
    public void SettingSourcePageIndexUpdatesPagerPageIndex()
    {
      _itemsSource.PageSize = 10;
      _itemsSource.PageIndex = 4; // Changed PageIndex of the items source to abitrary page.
      Assert.AreEqual(4, _pager.PageIndex); // The PageIndex of the pager automatically updates as well.
    }

    [Test]
    [STAThread]
    public void PagerPageIndexUsesSourceConstraints()
    {
      _itemsSource.PageSize = 10;

      _pager.PageIndex = -9;
      Assert.AreEqual(0, _pager.PageIndex);

      _pager.PageIndex = 13;
      Assert.AreEqual(9, _pager.PageIndex);
    }

    #endregion // Synchronized PageIndex Tests

    #region Pager button tests

    [Test]
    [STAThread]
    public void InitiallyOnePagerButton()
    {
      // The data grid source currently has a single page, so there will be a single pager button as follows:
      Assert.AreEqual(1, _pager.PagerButtons.Count);

      PagerButtonModel model = _pager.PagerButtons[0];
      Assert.AreEqual(0, model.PageIndex);
      Assert.AreEqual(1, model.Content);
      Assert.IsTrue(model.IsSelected);
    }

    [Test]
    [STAThread]
    public void ChangingPageSizeUpdatesPagerButtonsCorrectly()
    {
      _itemsSource.PageSize = 10;
      Assert.AreEqual(9, _pager.PagerButtons.Count);

      Assert.IsTrue(_pager.PagerButtons[0].IsSelected);

      int index = 0;
      foreach (PagerButtonModel model in _pager.PagerButtons)
      {
        Assert.AreEqual(index, model.PageIndex);
        if (index == 8)
        {
          Assert.AreEqual("...", model.Content);
        }
        else
        {
          Assert.AreEqual(index + 1, model.Content);
        }
        if (index != 0)
        {
          Assert.IsFalse(model.IsSelected);
        }
        index++;
      }
    }

    private void CheckAllPagerButtons()
    {
      int buttonCount = _pager.PagerButtons.Count;
      int index = Math.Max(0, Math.Min(_pager.PageIndex - (buttonCount / 2), _itemsSource.PageCount - buttonCount));
      int buttonIndex = 0;
      foreach (PagerButtonModel model in _pager.PagerButtons)
      {
        // Correct page index:
        Assert.AreEqual(index, model.PageIndex);

        if (buttonIndex == 0 && index != 0 && (_pager.EllipsisMode == EllipsisMode.Before || _pager.EllipsisMode == EllipsisMode.Both))
        {
          // First ellipsis:
          Assert.AreEqual("...", model.Content);
        }
        else if (buttonIndex == buttonCount - 1 && index != _itemsSource.PageCount - 1 && (_pager.EllipsisMode == EllipsisMode.After || _pager.EllipsisMode == EllipsisMode.Both))
        {
          // Last ellipsis:
          Assert.AreEqual("...", model.Content);
        }
        else
        {
          // Otherwise the content should be the page number plus one:
          Assert.AreEqual(index + 1, model.Content);
        }

        // The pager button is selected if and only if the buttons' page index matches the DataGridPagers' page index:
        if (model.PageIndex != _pager.PageIndex)
        {
          Assert.IsFalse(model.IsSelected);
        }
        else
        {
          Assert.IsTrue(model.IsSelected);
        }
        index++;
        buttonIndex++;
      }
    }

    [Test]
    [STAThread]
    public void ChangingPageIndexUpdatesPagerButtonsCorrectly()
    {
      _itemsSource.PageSize = 10;
      _pager.PageIndex = 1;

      Assert.AreEqual(9, _pager.PagerButtons.Count);
      Assert.IsTrue(_pager.PagerButtons[1].IsSelected);
      Assert.AreEqual(1, _pager.PagerButtons[1].PageIndex);

      CheckAllPagerButtons();
    }

    [Test]
    [STAThread]
    public void ChangingPageIndexUpdatesPagerButtonsCorrectly_MiddlePage()
    {
      _itemsSource.PageSize = 5; // 5 items per page - 20 pages
      _pager.PageIndex = 10; // some page in the middle

      Assert.AreEqual(9, _pager.PagerButtons.Count);
      // The middle button will be the selected button:
      Assert.IsTrue(_pager.PagerButtons[4].IsSelected);
      Assert.AreEqual(10, _pager.PagerButtons[4].PageIndex);

      CheckAllPagerButtons();
    }

    [Test]
    [STAThread]
    public void ChangingPageIndexUpdatesPagerButtonsCorrectly_MiddlePage_EvenNumberOfPagerButtons()
    {
      _itemsSource.PageSize = 5; // 5 items per page - 20 pages
      _pager.MaxPagerButtonCount = 8; // even number of pager buttons
      _pager.PageIndex = 10; // some page in the middle

      Assert.AreEqual(8, _pager.PagerButtons.Count); // now there are 8 pager buttons
      // The middle button will be the selected button:
      Assert.IsTrue(_pager.PagerButtons[4].IsSelected);
      Assert.AreEqual(10, _pager.PagerButtons[4].PageIndex);

      CheckAllPagerButtons();
    }

    [Test]
    [STAThread]
    public void ChangingPageIndexUpdatesPagerButtonsCorrectly_LastPage()
    {
      _itemsSource.PageSize = 5; // 5 items per page - 20 pages
      _pager.PageIndex = 19; // last page

      Assert.AreEqual(9, _pager.PagerButtons.Count);
      // The last button will be the selected button:
      Assert.IsTrue(_pager.PagerButtons[8].IsSelected);
      Assert.AreEqual(19, _pager.PagerButtons[8].PageIndex);

      CheckAllPagerButtons();
    }

    [Test]
    [STAThread]
    public void PagerButtonsAreCorrectWithSmallPageCount()
    {
      _itemsSource.PageSize = 49; // 49 items per page will create 3 pages

      // Max button count is 8, but there are only 3 pages, so only create 3 buttons:
      Assert.AreEqual(3, _pager.PagerButtons.Count);

      CheckAllPagerButtons();
    }

    // I dought this would ever change, but this will make things easier if it does:
    private static string _ellipsisContent = "...";

    [Test]
    [STAThread]
    public void NoneEllipsisModeDoesNotDisplayEllipsis()
    {
      _pager.EllipsisMode = EllipsisMode.None;
      _itemsSource.PageSize = 5; // 5 items per page - 20 pages

      // page index 0
      Assert.AreNotEqual(_ellipsisContent, _pager.PagerButtons[0].Content);
      Assert.AreNotEqual(_ellipsisContent, _pager.PagerButtons[8].Content);
      CheckAllPagerButtons();

      // middle page
      _itemsSource.PageIndex = 10;
      Assert.AreNotEqual(_ellipsisContent, _pager.PagerButtons[0].Content);
      Assert.AreNotEqual(_ellipsisContent, _pager.PagerButtons[8].Content);
      CheckAllPagerButtons();

      // last page
      _itemsSource.PageIndex = 19;
      Assert.AreNotEqual(_ellipsisContent, _pager.PagerButtons[0].Content);
      Assert.AreNotEqual(_ellipsisContent, _pager.PagerButtons[8].Content);
      CheckAllPagerButtons();
    }

    [Test]
    [STAThread]
    public void BeforeEllipsisModeDisplaysEllipsis()
    {
      _pager.EllipsisMode = EllipsisMode.Before;
      _itemsSource.PageSize = 5; // 5 items per page - 20 pages

      // Some where in the middle, first button has ellipsis, last button does not.
      _itemsSource.PageIndex = 10;
      Assert.AreEqual(_ellipsisContent, _pager.PagerButtons[0].Content);
      Assert.AreNotEqual(_ellipsisContent, _pager.PagerButtons[8].Content);
      CheckAllPagerButtons();

      // Last page, first button has ellipsis, last button does not.
      _itemsSource.PageIndex = 19;
      Assert.AreEqual(_ellipsisContent, _pager.PagerButtons[0].Content);
      Assert.AreNotEqual(_ellipsisContent, _pager.PagerButtons[8].Content);
      CheckAllPagerButtons();
    }

    [Test]
    [STAThread]
    public void BeforeEllipsisModeOnlyDisplaysEllipsisWhenNeeded()
    {
      _pager.EllipsisMode = EllipsisMode.Before;
      _itemsSource.PageSize = 5; // 5 items per page - 20 pages

      // First page, don't need to display ellipsis anywhere
      Assert.AreNotEqual(_ellipsisContent, _pager.PagerButtons[0].Content);
      Assert.AreNotEqual(_ellipsisContent, _pager.PagerButtons[8].Content);
      CheckAllPagerButtons();

      // The selected button is not the first page, but first page button will still be visible, so don't use ellipsis
      _itemsSource.PageIndex = 3;
      Assert.AreNotEqual(_ellipsisContent, _pager.PagerButtons[0].Content);
      Assert.AreNotEqual(_ellipsisContent, _pager.PagerButtons[8].Content);
      CheckAllPagerButtons();
    }

    [Test]
    [STAThread]
    public void AfterEllipsisModeDisplaysEllipsis()
    {
      _pager.EllipsisMode = EllipsisMode.After;
      _itemsSource.PageSize = 5; // 5 items per page - 20 pages

      // First page, last button has ellipsis, first button does not.
      Assert.AreNotEqual(_ellipsisContent, _pager.PagerButtons[0].Content);
      Assert.AreEqual(_ellipsisContent, _pager.PagerButtons[8].Content);
      CheckAllPagerButtons();

      // Some where in the middle, last button has ellipsis, first button does not.
      _itemsSource.PageIndex = 10;
      Assert.AreNotEqual(_ellipsisContent, _pager.PagerButtons[0].Content);
      Assert.AreEqual(_ellipsisContent, _pager.PagerButtons[8].Content);
      CheckAllPagerButtons();
    }

    [Test]
    [STAThread]
    public void AfterEllipsisModeOnlyDisplaysEllipsisWhenNeeded()
    {
      _pager.EllipsisMode = EllipsisMode.After;
      _itemsSource.PageSize = 5; // 5 items per page - 20 pages

      // Last page, don't need to display ellipsis anywhere
      _pager.PageIndex = 19;
      Assert.AreNotEqual(_ellipsisContent, _pager.PagerButtons[0].Content);
      Assert.AreNotEqual(_ellipsisContent, _pager.PagerButtons[8].Content);
      CheckAllPagerButtons();

      // The selected button is not the last page, but last page button will still be visible, so don't use ellipsis
      _itemsSource.PageIndex = 16;
      Assert.AreNotEqual(_ellipsisContent, _pager.PagerButtons[0].Content);
      Assert.AreNotEqual(_ellipsisContent, _pager.PagerButtons[8].Content);
      CheckAllPagerButtons();
    }

    [Test]
    [STAThread]
    public void BothEllipsisModeDisplaysEllipsis()
    {
      _pager.EllipsisMode = EllipsisMode.Both;
      _itemsSource.PageSize = 5; // 5 items per page - 20 pages

      // Some where in the middle, first and last buttons both have ellipsis
      _itemsSource.PageIndex = 10;
      Assert.AreEqual(_ellipsisContent, _pager.PagerButtons[0].Content);
      Assert.AreEqual(_ellipsisContent, _pager.PagerButtons[8].Content);
      CheckAllPagerButtons();
    }

    [Test]
    [STAThread]
    public void BothEllipsisModeDoesNotNeedToDisplayFirstEllipsis()
    {
      _pager.EllipsisMode = EllipsisMode.Both;
      _itemsSource.PageSize = 5; // 5 items per page - 20 pages

      // First page, don't need to display first ellipsis, but still display last one
      Assert.AreNotEqual(_ellipsisContent, _pager.PagerButtons[0].Content);
      Assert.AreEqual(_ellipsisContent, _pager.PagerButtons[8].Content);
      CheckAllPagerButtons();

      // The selected button is not the first page, but first page button will still be visible, so don't use first ellipsis
      _itemsSource.PageIndex = 3;
      Assert.AreNotEqual(_ellipsisContent, _pager.PagerButtons[0].Content);
      Assert.AreEqual(_ellipsisContent, _pager.PagerButtons[8].Content);
      CheckAllPagerButtons();
    }

    [Test]
    [STAThread]
    public void BothEllipsisModeDoesNotNeedToDisplayLastEllipsis()
    {
      _pager.EllipsisMode = EllipsisMode.Both;
      _itemsSource.PageSize = 5; // 5 items per page - 20 pages

      // Last page, don't need to display last ellipsis, but still display first one
      _pager.PageIndex = 19;
      Assert.AreEqual(_ellipsisContent, _pager.PagerButtons[0].Content);
      Assert.AreNotEqual(_ellipsisContent, _pager.PagerButtons[8].Content);
      CheckAllPagerButtons();

      // The selected button is not the last page, but last page button will still be visible, so don't use last ellipsis
      _itemsSource.PageIndex = 16;
      Assert.AreEqual(_ellipsisContent, _pager.PagerButtons[0].Content);
      Assert.AreNotEqual(_ellipsisContent, _pager.PagerButtons[8].Content);
      CheckAllPagerButtons();
    }

    [Test]
    [STAThread]
    public void BothEllipsisModeDoesNotDisplayEllipsisWithSmallPageCount()
    {
      _pager.EllipsisMode = EllipsisMode.Both;
      _itemsSource.PageSize = 20; // 20 items per page - 5 pages

      // There are only 5 pages, so all possible pager buttons are visible, so don't use ellipsis anywhere
      Assert.AreEqual(5, _pager.PagerButtons.Count);
      Assert.AreNotEqual(_ellipsisContent, _pager.PagerButtons[0].Content);
      Assert.AreNotEqual(_ellipsisContent, _pager.PagerButtons[4].Content);

      CheckAllPagerButtons();
    }

    [Test]
    [STAThread]
    public void SelectingPagerButtonUpdatesPageIndex()
    {
      _itemsSource.PageSize = 5; // 5 items per page - 20 pages

      // Check defaults:
      Assert.AreEqual(9, _pager.PagerButtons.Count);
      Assert.AreEqual(0, _pager.PageIndex);
      Assert.IsFalse(_pager.PagerButtons[6].IsSelected);

      // Programatically select button for page 7 (index 6)
      _pager.PagerButtons[6].IsSelected = true;
      // The PageIndex of the pager has updated correctly:
      Assert.AreEqual(6, _pager.PageIndex);
      // The PageIndex of the DataGridItemsSource has also been updated:
      Assert.AreEqual(6, _itemsSource.PageIndex);
    }

    [Test]
    [STAThread]
    public void SelectingPagerButtonUpdatesPagerButtonsCorrectly()
    {
      _itemsSource.PageSize = 5; // 5 items per page - 20 pages

      // Programatically select button for page 7 (index 6)
      _pager.PagerButtons[6].IsSelected = true;
      // The center button (at button index 4) will now be selected and represent page 7 (page index 6)
      Assert.IsTrue(_pager.PagerButtons[4].IsSelected);
      Assert.AreEqual(6, _pager.PagerButtons[4].PageIndex);

      CheckAllPagerButtons();
    }

    [Test]
    [STAThread]
    public void DecreasingMaxPagerButtonCountUpdatesPagerButtonsCorrectly()
    {
      _itemsSource.PageSize = 5; // 5 items per page - 20 pages

      // Currently has 9 pager buttons:
      Assert.AreEqual(9, _pager.MaxPagerButtonCount);
      Assert.AreEqual(9, _pager.PagerButtons.Count);

      // Decrease max pager button count to 5:
      _pager.MaxPagerButtonCount = 5;
      // Now there are only 5 buttons displayed:
      Assert.AreEqual(5, _pager.PagerButtons.Count);

      CheckAllPagerButtons();
    }

    [Test]
    [STAThread]
    public void IncreasingMaxPagerButtonCountUpdatesPagerButtonsCorrectly()
    {
      _itemsSource.PageSize = 5; // 5 items per page - 20 pages

      // Currently has 9 pager buttons:
      Assert.AreEqual(9, _pager.MaxPagerButtonCount);
      Assert.AreEqual(9, _pager.PagerButtons.Count);

      // Increase max pager button count to 18:
      _pager.MaxPagerButtonCount = 18;
      // Now there are only 5 buttons displayed:
      Assert.AreEqual(18, _pager.PagerButtons.Count);

      CheckAllPagerButtons();
    }

    [Test]
    [STAThread]
    public void IncreasingPageSizeUpdatesPagerButtonsCorrectly()
    {
      _itemsSource.PageSize = 5; // 5 items per page - 20 pages

      // Currently has 9 pager buttons:
      Assert.AreEqual(9, _pager.PagerButtons.Count);

      // Increase page size from 5 to 24. Now there are only 5 pages
      _itemsSource.PageSize = 24;
      // Now only 5 pager buttons:
      Assert.AreEqual(5, _pager.PagerButtons.Count);

      CheckAllPagerButtons();
    }

    [Test]
    [STAThread]
    public void DecreasingPageSizeUpdatesPagerButtonsCorrectly()
    {
      _itemsSource.PageSize = 25; // 25 items per page - 4 pages

      // Currently has 4 pager buttons:
      Assert.AreEqual(4, _pager.PagerButtons.Count);

      // Decrease page size from 25 to 19. Now there are 6 pages
      _itemsSource.PageSize = 19;
      // Now displays 6 pager buttons:
      Assert.AreEqual(6, _pager.PagerButtons.Count);

      CheckAllPagerButtons();
    }

    // TODO: write test about dynamically changing ellipsis mode
    // TODO: maybe write test about ellipsis behavior with small button count

    #endregion // Pager button tests
  }
}
