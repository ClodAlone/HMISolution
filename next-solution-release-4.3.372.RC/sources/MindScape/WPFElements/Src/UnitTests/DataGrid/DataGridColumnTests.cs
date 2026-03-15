using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Mindscape.WpfElements.WpfDataGrid;
using System.Windows;

namespace Mindscape.WpfElements.WpfDataGrid.UnitTests.DataGridTests
{
  [TestFixture]
  public class DataGridColumnTests
  {
    private DataGridColumn _column;

    [SetUp]
    public void SetUp()
    {
      _column = new DataGridColumn();
    }

    #region Width and MinWidth tests

    [Test]
    public void DefaultColumnWidth()
    {
      Assert.AreEqual(DataGrid.DefaultColumnWidth, _column.Width);
    }

    [Test]
    public void DefaultMinWidth()
    {
      Assert.AreEqual(10.0, _column.MinWidth);
    }

    [Test]
    public void WidthProperty()
    {
      _column.Width = new GridLength(257);
      Assert.AreEqual(257, _column.Width.Value);
      _column.Width = new GridLength(43);
      Assert.AreEqual(43, _column.Width.Value);
    }

    [Test]
    public void WidthCanNotBeSetLessThanMinWidth()
    {
      Assert.Less(5, _column.MinWidth);
      _column.IsLoaded = true; // Kludge
      _column.Width = new GridLength(5);
      Assert.AreEqual(_column.MinWidth, _column.Width.Value);
    }

    [Test]
    public void WidthCanNotBeSetNegative()
    {
      // This is due to the min width never being negative.
      _column.IsLoaded = true; // Kludge
      _column.Width = new GridLength(-42);
      Assert.AreEqual(_column.MinWidth, _column.Width.Value);
    }

    [Test]
    public void MinWidthProperty()
    {
      _column.MinWidth = 89;
      Assert.AreEqual(89, _column.MinWidth);
      _column.MinWidth = 7;
      Assert.AreEqual(7, _column.MinWidth);
    }

    [Test]
    [ExpectedException(typeof(InvalidOperationException))]
    public void MinWidthCanNotBeNegative()
    {
      _column.MinWidth = -77;
    }

    [Test]
    public void SettingMinWidthWillSetWidthIfNeccessary()
    {
      Assert.Greater(200, DataGrid.DefaultColumnWidth.Value);
      _column.MinWidth = 200; // This is larger than the current width.
      Assert.AreEqual(200, _column.MinWidth);
      Assert.AreEqual(200, _column.Width.Value); // So the Width property is updated to respect the MinWidth property.
    }

    [Test]
    public void WidthChangedEventIsRaised()
    {
      int eventCount = 0;
      _column.WidthChanged += delegate(object sender, ColumnWidthChangedEventArgs e)
      {
        eventCount++;
      };
      _column.Width = new GridLength(489);
      Assert.AreEqual(1, eventCount);
    }

    [Test]
    public void WidthChangedEventArgsAreCorrect()
    {
      _column.Width = new GridLength(376);
      int eventCount = 0;
      _column.WidthChanged += delegate(object sender, ColumnWidthChangedEventArgs e)
      {
        eventCount++;
        Assert.AreEqual(376, e.OldWidth);
        Assert.AreEqual(489, e.NewWidth);
      };
      _column.Width = new GridLength(489);
      Assert.AreEqual(1, eventCount);
    }

    [Test]
    public void WidthChangedEventIsOnlyRaisedOnceWhenSettingBelowMinWidth()
    {
      int eventCount = 0;
      _column.WidthChanged += delegate(object sender, ColumnWidthChangedEventArgs e)
      {
        eventCount++;
      };
      Assert.Less(5, _column.MinWidth);
      _column.Width = new GridLength(5);
      Assert.AreEqual(1, eventCount);
    }

    [Test]
    public void WidthChangedEventArgsAreCorrectWhenSettingBelowMinimum()
    {
      _column.IsLoaded = true; // Kludge
      _column.Width = new GridLength(193);
      int eventCount = 0;
      _column.WidthChanged += delegate(object sender, ColumnWidthChangedEventArgs e)
      {
        eventCount++;
        Assert.AreEqual(193, e.OldWidth);
        Assert.AreEqual(_column.MinWidth, e.NewWidth);
      };
      Assert.Less(3, _column.MinWidth);
      _column.Width = new GridLength(3);
      Assert.AreEqual(1, eventCount);
    }

    [Test]
    public void WidthChangedEventIsNotRaisedWhenSettingBelowMinimumIfNoChangeOccured()
    {
      _column.IsLoaded = true; // Kludge
      // Set the width to be the minimum width.
      _column.Width = new GridLength(_column.MinWidth);
      int eventCount = 0;
      _column.WidthChanged += delegate(object sender, ColumnWidthChangedEventArgs e)
      {
        eventCount++;
      };
      Assert.Less(5, _column.MinWidth);
      // At this point the width is the minimum width.
      // Setting the width to be lower than the minimum width will result in the Width property being set to be the minimum width - as seen in previous tests.
      // Since the resulting width property will be the same as before we set it, the WidthChanged event should not fire.
      _column.Width = new GridLength(5);
      Assert.AreEqual(0, eventCount);
    }

    #endregion // Width and MinWidth tests

    #region AllowResize tests

    [Test]
    public void AllowResizeByDefault()
    {
      Assert.IsTrue(_column.AllowResize);
    }

    [Test]
    public void AllowResizeProperty()
    {
      _column.AllowResize = false;
      Assert.IsFalse(_column.AllowResize);
      _column.AllowResize = true;
      Assert.IsTrue(_column.AllowResize);
    }

    [Test]
    [Ignore]
    public void WidthPropertyIsNotChangedIfNotAllowResize()
    {
      _column.AllowResize = false;
      _column.Width = new GridLength(666);
      Assert.AreEqual(DataGrid.DefaultColumnWidth, _column.Width);
    }

    [Test]
    [Ignore]
    public void WidthChangedIsNotRaisedIfNotAllowResize()
    {
      int eventCount = 0;
      _column.WidthChanged += delegate(object sender, ColumnWidthChangedEventArgs e)
      {
        eventCount++;
      };
      _column.AllowResize = false;
      _column.Width = new GridLength(89);
      Assert.AreEqual(0, eventCount);
    }

    [Test]
    [ExpectedException(typeof(InvalidOperationException))]
    public void MinWidthCanNotBeNegativeIfNotAllowResize()
    {
      _column.AllowResize = false;
      _column.MinWidth = -97;
    }

    [Test]
    public void MinWidthCanBeSetIfNotAllowResize()
    {
      _column.AllowResize = false;
      _column.MinWidth = 67;
      Assert.AreEqual(67, _column.MinWidth);
      Assert.AreEqual(DataGrid.DefaultColumnWidth, _column.Width);
    }

    [Test]
    [Ignore]
    public void MinWidthCanNotBeSetGreaterThanWidthIfNotAllowResize()
    {
      _column.AllowResize = false;
      Assert.Greater(273, DataGrid.DefaultColumnWidth.Value);
      _column.MinWidth = 273;
      Assert.AreEqual(DataGrid.DefaultColumnWidth, _column.MinWidth);
      Assert.AreEqual(DataGrid.DefaultColumnWidth, _column.Width);
    }

    #endregion // AllowResize tests
  }
}
