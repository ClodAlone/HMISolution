using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Mindscape.WpfElements.WpfDataGrid;
using System.Collections;
using System.Collections.ObjectModel;
using System.Windows.Data;

namespace Mindscape.WpfElements.UnitTests.DataGridTests
{
  [TestFixture]
  public class GroupingTests
  {
    // TODO: dynamically add item to collection when grouping is active (single and multiple property grouping.
    // TODO: test with both objects and descriptors.
    // TODO: test expanding/collapsing events.
    // TODO: test that group event handlers are removed when ungrouping.
    // TODO: test header names and other information (aggregates when they are implented - but put tests in a different class).
    // TODO: test that DataGridItemsSource.GetItemAt returns null for group headers.
    // TODO: group an enum property.
    // TODO: test that group name respects a TypeConverter that changes the names of enum values.
    // TODO: group an ICustomTypeDescriptor.
    // TODO: group by a property that is a PropertyDescriptor.
    // TODO: group by a property that is valid for some of the objects, but not valid for others.
    // TODO: group converters. (Alphabetical, numerical, date-time, color, custom).
    // TODO: Test swapping the order of group descriptors.

    // TODO: if we implement some fancy code that builds parts of the list on demand, then we'll also need tests for getting items in a different order.

    private ObservableCollection<TestClass1> _data;
    private DataGridItemsSource _dataGridItemsSource;

    // The test objects in the order they are added to the items source:
    private readonly TestClass1 _pear5 = new TestClass1() { Property1 = 5, Property2 = "Pear" };
    private readonly TestClass1 _orange3A = new TestClass1() { Property1 = 3, Property2 = "Orange" };
    private readonly TestClass1 _apple7 = new TestClass1() { Property1 = 7, Property2 = "Apple" };
    private readonly TestClass1 _pear3 = new TestClass1() { Property1 = 3, Property2 = "Pear" };
    private readonly TestClass1 _apple5 = new TestClass1() { Property1 = 5, Property2 = "Apple" };
    private readonly TestClass1 _orange5 = new TestClass1() { Property1 = 5, Property2 = "Orange" };
    private readonly TestClass1 _pear7 = new TestClass1() { Property1 = 7, Property2 = "Pear" };
    private readonly TestClass1 _orange3B = new TestClass1() { Property1 = 3, Property2 = "Orange" };

    [SetUp]
    public void SetUp()
    {
      _data = new ObservableCollection<TestClass1>();
      _data.Add(_pear5);
      _data.Add(_orange3A);
      _data.Add(_apple7);
      _data.Add(_pear3);
      _data.Add(_apple5);
      _data.Add(_orange5);
      _data.Add(_pear7);
      _data.Add(_orange3B);
      _dataGridItemsSource = new DataGridItemsSource(_data);
    }

    [Test]
    public void NoGrouping() // Control test
    {
      Assert.AreEqual(8, _dataGridItemsSource.Count);
      Assert.AreEqual(8, _dataGridItemsSource.CountOnCurrentPage);

      // Check that all the items exist in the same order as the original collection.
      int index = 0;
      foreach (object o in _dataGridItemsSource)
      {
        Assert.AreSame(o, _data[index]);
        index++;
      }
    }

    // This test covers GroupName, InternalChildren.Count, Level, The order of the items/groups as well as the existence of the group headers.
    [Test]
    public void SinglePropertyGrouping()
    {
      Group("Property1");

      //Expected:

      // 3
      //   3, Orange
      //   3, Pear
      //   3, Orange
      // 5
      //   5, Pear
      //   5, Apple
      //   5, Orange
      // 7
      //   7, Apple
      //   7, Pear

      // Collection now includes group headers
      Assert.AreEqual(8, _dataGridItemsSource.Count); // TODO: this is only 8, is this correct?
      Assert.AreEqual(11, _dataGridItemsSource.CountOnCurrentPage);

      // Note that the groups are sorted, and the items in each group are in their original order.

      // Group 3

      DataGridGroup group = _dataGridItemsSource.GetItemOnCurrentPageAt(0) as DataGridGroup;
      Assert.IsNotNull(group);
      Assert.AreEqual("3", group.GroupName);
      Assert.AreEqual(0, group.Level);
      Assert.AreEqual(0, group.Groups.Count);

      Assert.AreSame(_orange3A, _dataGridItemsSource.GetItemOnCurrentPageAt(1));
      Assert.AreSame(_pear3, _dataGridItemsSource.GetItemOnCurrentPageAt(2));
      Assert.AreSame(_orange3B, _dataGridItemsSource.GetItemOnCurrentPageAt(3));

      Assert.AreEqual(3, group.InternalChildren.Count);
      Assert.AreSame(_orange3A, group.InternalChildren[0]);
      Assert.AreSame(_pear3, group.InternalChildren[1]);
      Assert.AreSame(_orange3B, group.InternalChildren[2]);
      Assert.AreEqual(3, group.Children.Count);
      Assert.AreSame(_orange3A, group.Children[0]);
      Assert.AreSame(_pear3, group.Children[1]);
      Assert.AreSame(_orange3B, group.Children[2]);

      // Group 5

      group = _dataGridItemsSource.GetItemOnCurrentPageAt(4) as DataGridGroup;
      Assert.IsNotNull(group);
      Assert.AreEqual("5", group.GroupName);
      Assert.AreEqual(0, group.Level);
      Assert.AreEqual(0, group.Groups.Count);

      Assert.AreSame(_pear5, _dataGridItemsSource.GetItemOnCurrentPageAt(5));
      Assert.AreSame(_apple5, _dataGridItemsSource.GetItemOnCurrentPageAt(6));
      Assert.AreSame(_orange5, _dataGridItemsSource.GetItemOnCurrentPageAt(7));

      Assert.AreEqual(3, group.InternalChildren.Count);
      Assert.AreSame(_pear5, group.InternalChildren[0]);
      Assert.AreSame(_apple5, group.InternalChildren[1]);
      Assert.AreSame(_orange5, group.InternalChildren[2]);
      Assert.AreEqual(3, group.Children.Count);
      Assert.AreSame(_pear5, group.Children[0]);
      Assert.AreSame(_apple5, group.Children[1]);
      Assert.AreSame(_orange5, group.Children[2]);

      // Group 7

      group = _dataGridItemsSource.GetItemOnCurrentPageAt(8) as DataGridGroup;
      Assert.IsNotNull(group);
      Assert.AreEqual("7", group.GroupName);
      Assert.AreEqual(0, group.Level);
      Assert.AreEqual(0, group.Groups.Count);

      Assert.AreSame(_apple7, _dataGridItemsSource.GetItemOnCurrentPageAt(9));
      Assert.AreSame(_pear7, _dataGridItemsSource.GetItemOnCurrentPageAt(10));

      Assert.AreEqual(2, group.InternalChildren.Count);
      Assert.AreSame(_apple7, group.InternalChildren[0]);
      Assert.AreSame(_pear7, group.InternalChildren[1]);
      Assert.AreEqual(2, group.Children.Count);
      Assert.AreSame(_apple7, group.Children[0]);
      Assert.AreSame(_pear7, group.Children[1]);
    }

    [Test]
    public void SinglePropertyUngrouping()
    {
      Group("Property1");
      Group();

      Assert.AreEqual(8, _dataGridItemsSource.Count);
      Assert.AreEqual(8, _dataGridItemsSource.CountOnCurrentPage);

      // Check that there are no groups and that the items are back in their original order.
      int index = 0;
      foreach (object o in _dataGridItemsSource)
      {
        Assert.AreSame(o, _data[index]);
        index++;
      }
    }

    [Test]
    public void GroupTwoProperties()
    {
      Group("Property2", "Property1");

      // Expected:

      // Apple
      //   5
      //     5, Apple
      //   7
      //     7, Apple
      // Orange
      //   3
      //     3, Orange
      //     3, Orange
      //   5
      //     5, Orange
      // Pear
      //   3
      //     3, Pear
      //   5
      //     5, Pear
      //   7
      //     7, Pear

      // Collection now includes group headers
      Assert.AreEqual(8, _dataGridItemsSource.Count); // TODO: this is only 8, is this correct?
      Assert.AreEqual(18, _dataGridItemsSource.CountOnCurrentPage);

      // Note that the groups are sorted.

      // Group Apple

      DataGridGroup group = _dataGridItemsSource.GetItemOnCurrentPageAt(0) as DataGridGroup;
      Assert.IsNotNull(group);
      Assert.AreEqual("Apple", group.GroupName);
      Assert.AreEqual(0, group.Level);

      DataGridGroup subGroup1 = _dataGridItemsSource.GetItemOnCurrentPageAt(1) as DataGridGroup;
      Assert.IsNotNull(subGroup1);
      Assert.AreEqual("5", subGroup1.GroupName);
      Assert.AreEqual(1, subGroup1.Level);
      Assert.AreEqual(0, subGroup1.Groups.Count);

      Assert.AreSame(_apple5, _dataGridItemsSource.GetItemOnCurrentPageAt(2));
      Assert.AreEqual(1, subGroup1.InternalChildren.Count);
      Assert.AreSame(_apple5, subGroup1.InternalChildren[0]);
      Assert.AreEqual(1, subGroup1.Children.Count);
      Assert.AreSame(_apple5, subGroup1.Children[0]);

      DataGridGroup subGroup2 = _dataGridItemsSource.GetItemOnCurrentPageAt(3) as DataGridGroup;
      Assert.IsNotNull(subGroup2);
      Assert.AreEqual("7", subGroup2.GroupName);
      Assert.AreEqual(1, subGroup2.Level);
      Assert.AreEqual(0, subGroup2.Groups.Count);

      Assert.AreSame(_apple7, _dataGridItemsSource.GetItemOnCurrentPageAt(4));
      Assert.AreEqual(1, subGroup2.InternalChildren.Count);
      Assert.AreSame(_apple7, subGroup2.InternalChildren[0]);
      Assert.AreEqual(1, subGroup2.Children.Count);
      Assert.AreSame(_apple7, subGroup2.Children[0]);

      // Group Apple contains sub groups, but no children
      Assert.AreEqual(0, group.InternalChildren.Count);
      Assert.AreEqual(2, group.Groups.Count);
      Assert.AreSame(subGroup1, group.Groups["5"]);
      Assert.AreSame(subGroup2, group.Groups["7"]);
      // Children gets the leaf nodes of the group.
      Assert.AreEqual(2, group.Children.Count);
      Assert.AreSame(_apple5, group.Children[0]);
      Assert.AreSame(_apple7, group.Children[1]);

      // Group Orange

      group = _dataGridItemsSource.GetItemOnCurrentPageAt(5) as DataGridGroup;
      Assert.IsNotNull(group);
      Assert.AreEqual("Orange", group.GroupName);
      Assert.AreEqual(0, group.Level);

      subGroup1 = _dataGridItemsSource.GetItemOnCurrentPageAt(6) as DataGridGroup;
      Assert.IsNotNull(subGroup1);
      Assert.AreEqual("3", subGroup1.GroupName);
      Assert.AreEqual(1, subGroup1.Level);
      Assert.AreEqual(0, subGroup1.Groups.Count);

      Assert.AreSame(_orange3A, _dataGridItemsSource.GetItemOnCurrentPageAt(7));
      Assert.AreSame(_orange3B, _dataGridItemsSource.GetItemOnCurrentPageAt(8));

      Assert.AreEqual(2, subGroup1.InternalChildren.Count);
      Assert.AreSame(_orange3A, subGroup1.InternalChildren[0]);
      Assert.AreSame(_orange3B, subGroup1.InternalChildren[1]);
      Assert.AreEqual(2, subGroup1.Children.Count);
      Assert.AreSame(_orange3A, subGroup1.Children[0]);
      Assert.AreSame(_orange3B, subGroup1.Children[1]);

      subGroup2 = _dataGridItemsSource.GetItemOnCurrentPageAt(9) as DataGridGroup;
      Assert.IsNotNull(subGroup2);
      Assert.AreEqual("5", subGroup2.GroupName);
      Assert.AreEqual(1, subGroup2.Level);
      Assert.AreEqual(0, subGroup2.Groups.Count);

      Assert.AreSame(_orange5, _dataGridItemsSource.GetItemOnCurrentPageAt(10));
      Assert.AreEqual(1, subGroup2.InternalChildren.Count);
      Assert.AreSame(_orange5, subGroup2.InternalChildren[0]);
      Assert.AreEqual(1, subGroup2.Children.Count);
      Assert.AreSame(_orange5, subGroup2.Children[0]);

      // Group Orange contains sub groups, but no children
      Assert.AreEqual(0, group.InternalChildren.Count);
      Assert.AreEqual(2, group.Groups.Count);
      Assert.AreSame(subGroup1, group.Groups["3"]);
      Assert.AreSame(subGroup2, group.Groups["5"]);
      // Children gets the leaf nodes of the group.
      Assert.AreEqual(3, group.Children.Count);
      Assert.AreSame(_orange3A, group.Children[0]);
      Assert.AreSame(_orange3B, group.Children[1]);
      Assert.AreSame(_orange5, group.Children[2]);

      // Group Pear

      group = _dataGridItemsSource.GetItemOnCurrentPageAt(11) as DataGridGroup;
      Assert.IsNotNull(group);
      Assert.AreEqual("Pear", group.GroupName);
      Assert.AreEqual(0, group.Level);

      subGroup1 = _dataGridItemsSource.GetItemOnCurrentPageAt(12) as DataGridGroup;
      Assert.IsNotNull(subGroup1);
      Assert.AreEqual("3", subGroup1.GroupName);
      Assert.AreEqual(1, subGroup1.Level);
      Assert.AreEqual(0, subGroup1.Groups.Count);

      Assert.AreSame(_pear3, _dataGridItemsSource.GetItemOnCurrentPageAt(13));
      Assert.AreEqual(1, subGroup1.InternalChildren.Count);
      Assert.AreSame(_pear3, subGroup1.InternalChildren[0]);
      Assert.AreEqual(1, subGroup1.Children.Count);
      Assert.AreSame(_pear3, subGroup1.Children[0]);

      subGroup2 = _dataGridItemsSource.GetItemOnCurrentPageAt(14) as DataGridGroup;
      Assert.IsNotNull(subGroup2);
      Assert.AreEqual("5", subGroup2.GroupName);
      Assert.AreEqual(1, subGroup2.Level);
      Assert.AreEqual(0, subGroup2.Groups.Count);

      Assert.AreSame(_pear5, _dataGridItemsSource.GetItemOnCurrentPageAt(15));
      Assert.AreEqual(1, subGroup2.InternalChildren.Count);
      Assert.AreSame(_pear5, subGroup2.InternalChildren[0]);
      Assert.AreEqual(1, subGroup2.Children.Count);
      Assert.AreSame(_pear5, subGroup2.Children[0]);

      DataGridGroup subGroup3 = _dataGridItemsSource.GetItemOnCurrentPageAt(16) as DataGridGroup;
      Assert.IsNotNull(subGroup3);
      Assert.AreEqual("7", subGroup3.GroupName);
      Assert.AreEqual(1, subGroup3.Level);
      Assert.AreEqual(0, subGroup3.Groups.Count);

      Assert.AreSame(_pear7, _dataGridItemsSource.GetItemOnCurrentPageAt(17));
      Assert.AreEqual(1, subGroup3.InternalChildren.Count);
      Assert.AreSame(_pear7, subGroup3.InternalChildren[0]);
      Assert.AreEqual(1, subGroup3.Children.Count);
      Assert.AreSame(_pear7, subGroup3.Children[0]);

      // Group Pear contains sub groups, but no children
      Assert.AreEqual(0, group.InternalChildren.Count);
      Assert.AreEqual(3, group.Groups.Count);
      Assert.AreSame(subGroup1, group.Groups["3"]);
      Assert.AreSame(subGroup2, group.Groups["5"]);
      Assert.AreSame(subGroup3, group.Groups["7"]);
      // Children gets the leaf nodes of the group.
      Assert.AreEqual(3, group.Children.Count);
      Assert.AreSame(_pear3, group.Children[0]);
      Assert.AreSame(_pear5, group.Children[1]);
      Assert.AreSame(_pear7, group.Children[2]);
    }

    [Test]
    public void GroupTwoProperties_ThenUngroupFirstProperty()
    {
      // Group by property2 then property1
      Group("Property2", "Property1");
      // Ungroup property2 (the first grouped property)
      Group("Property1");

      // Expected:

      // 3
      //   3, Orange
      //   3, Pear
      //   3, Orange
      // 5
      //   5, Pear
      //   5, Apple
      //   5, Orange
      // 7
      //   7, Apple
      //   7, Pear

      // Collection now includes group headers
      Assert.AreEqual(8, _dataGridItemsSource.Count); // TODO: this is only 8, is this correct?
      Assert.AreEqual(11, _dataGridItemsSource.CountOnCurrentPage);

      // Note that the groups are sorted, and the items in each group are in their original order.

      // Group 3

      DataGridGroup group = _dataGridItemsSource.GetItemOnCurrentPageAt(0) as DataGridGroup;
      Assert.IsNotNull(group);
      Assert.AreEqual("3", group.GroupName);
      Assert.AreEqual(0, group.Level);
      Assert.AreEqual(0, group.Groups.Count);

      Assert.AreSame(_orange3A, _dataGridItemsSource.GetItemOnCurrentPageAt(1));
      Assert.AreSame(_pear3, _dataGridItemsSource.GetItemOnCurrentPageAt(2));
      Assert.AreSame(_orange3B, _dataGridItemsSource.GetItemOnCurrentPageAt(3));

      Assert.AreEqual(3, group.InternalChildren.Count);
      Assert.AreSame(_orange3A, group.InternalChildren[0]);
      Assert.AreSame(_pear3, group.InternalChildren[1]);
      Assert.AreSame(_orange3B, group.InternalChildren[2]);
      Assert.AreEqual(3, group.Children.Count);
      Assert.AreSame(_orange3A, group.Children[0]);
      Assert.AreSame(_pear3, group.Children[1]);
      Assert.AreSame(_orange3B, group.Children[2]);

      // Group 5

      group = _dataGridItemsSource.GetItemOnCurrentPageAt(4) as DataGridGroup;
      Assert.IsNotNull(group);
      Assert.AreEqual("5", group.GroupName);
      Assert.AreEqual(0, group.Level);
      Assert.AreEqual(0, group.Groups.Count);

      Assert.AreSame(_pear5, _dataGridItemsSource.GetItemOnCurrentPageAt(5));
      Assert.AreSame(_apple5, _dataGridItemsSource.GetItemOnCurrentPageAt(6));
      Assert.AreSame(_orange5, _dataGridItemsSource.GetItemOnCurrentPageAt(7));

      Assert.AreEqual(3, group.InternalChildren.Count);
      Assert.AreSame(_pear5, group.InternalChildren[0]);
      Assert.AreSame(_apple5, group.InternalChildren[1]);
      Assert.AreSame(_orange5, group.InternalChildren[2]);
      Assert.AreEqual(3, group.Children.Count);
      Assert.AreSame(_pear5, group.Children[0]);
      Assert.AreSame(_apple5, group.Children[1]);
      Assert.AreSame(_orange5, group.Children[2]);

      // Group 7

      group = _dataGridItemsSource.GetItemOnCurrentPageAt(8) as DataGridGroup;
      Assert.IsNotNull(group);
      Assert.AreEqual("7", group.GroupName);
      Assert.AreEqual(0, group.Level);
      Assert.AreEqual(0, group.Groups.Count);

      Assert.AreSame(_apple7, _dataGridItemsSource.GetItemOnCurrentPageAt(9));
      Assert.AreSame(_pear7, _dataGridItemsSource.GetItemOnCurrentPageAt(10));

      Assert.AreEqual(2, group.InternalChildren.Count);
      Assert.AreSame(_apple7, group.InternalChildren[0]);
      Assert.AreSame(_pear7, group.InternalChildren[1]);
      Assert.AreEqual(2, group.Children.Count);
      Assert.AreSame(_apple7, group.Children[0]);
      Assert.AreSame(_pear7, group.Children[1]);
    }

    [Test]
    public void GroupTwoProperties_ThenUngroupSecondProperty()
    {
      // Group by property2 then property1
      Group("Property2", "Property1");
      // Ungroup property1 (the second grouped property)
      Group("Property2");

      // Expected:

      // Apple
      //   7, Apple
      //   5, Apple
      // Orange
      //   3, Orange
      //   5, Orange
      //   3, Orange
      // Pear
      //   5, Pear
      //   3, Pear
      //   7, Pear

      // Collection now includes group headers
      Assert.AreEqual(8, _dataGridItemsSource.Count); // TODO: this is only 8, is this correct?
      Assert.AreEqual(11, _dataGridItemsSource.CountOnCurrentPage);

      // Note that the groups are sorted, and the items in each group are in their original order.

      // Group Apple

      DataGridGroup group = _dataGridItemsSource.GetItemOnCurrentPageAt(0) as DataGridGroup;
      Assert.IsNotNull(group);
      Assert.AreEqual("Apple", group.GroupName);
      Assert.AreEqual(0, group.Level);
      Assert.AreEqual(0, group.Groups.Count);

      Assert.AreSame(_apple7, _dataGridItemsSource.GetItemOnCurrentPageAt(1));
      Assert.AreSame(_apple5, _dataGridItemsSource.GetItemOnCurrentPageAt(2));

      Assert.AreEqual(2, group.InternalChildren.Count);
      Assert.AreSame(_apple7, group.InternalChildren[0]);
      Assert.AreSame(_apple5, group.InternalChildren[1]);
      Assert.AreEqual(2, group.Children.Count);
      Assert.AreSame(_apple7, group.Children[0]);
      Assert.AreSame(_apple5, group.Children[1]);

      // Group Orange

      group = _dataGridItemsSource.GetItemOnCurrentPageAt(3) as DataGridGroup;
      Assert.IsNotNull(group);
      Assert.AreEqual("Orange", group.GroupName);
      Assert.AreEqual(0, group.Level);
      Assert.AreEqual(0, group.Groups.Count);

      Assert.AreSame(_orange3A, _dataGridItemsSource.GetItemOnCurrentPageAt(4));
      Assert.AreSame(_orange5, _dataGridItemsSource.GetItemOnCurrentPageAt(5));
      Assert.AreSame(_orange3B, _dataGridItemsSource.GetItemOnCurrentPageAt(6));

      Assert.AreEqual(3, group.InternalChildren.Count);
      Assert.AreSame(_orange3A, group.InternalChildren[0]);
      Assert.AreSame(_orange5, group.InternalChildren[1]);
      Assert.AreSame(_orange3B, group.InternalChildren[2]);
      Assert.AreEqual(3, group.Children.Count);
      Assert.AreSame(_orange3A, group.Children[0]);
      Assert.AreSame(_orange5, group.Children[1]);
      Assert.AreSame(_orange3B, group.Children[2]);

      // Group Pear

      group = _dataGridItemsSource.GetItemOnCurrentPageAt(7) as DataGridGroup;
      Assert.IsNotNull(group);
      Assert.AreEqual("Pear", group.GroupName);
      Assert.AreEqual(0, group.Level);
      Assert.AreEqual(0, group.Groups.Count);

      Assert.AreSame(_pear5, _dataGridItemsSource.GetItemOnCurrentPageAt(8));
      Assert.AreSame(_pear3, _dataGridItemsSource.GetItemOnCurrentPageAt(9));
      Assert.AreSame(_pear7, _dataGridItemsSource.GetItemOnCurrentPageAt(10));

      Assert.AreEqual(3, group.InternalChildren.Count);
      Assert.AreSame(_pear5, group.InternalChildren[0]);
      Assert.AreSame(_pear3, group.InternalChildren[1]);
      Assert.AreSame(_pear7, group.InternalChildren[2]);
      Assert.AreEqual(3, group.Children.Count);
      Assert.AreSame(_pear5, group.Children[0]);
      Assert.AreSame(_pear3, group.Children[1]);
      Assert.AreSame(_pear7, group.Children[2]);
    }

    [Test]
    public void GroupEmptyList()
    {
      ObservableCollection<TestClass1> data = new ObservableCollection<TestClass1>();
      DataGridItemsSource itemsSource = new DataGridItemsSource(data);

      Group(itemsSource, "Property1");

      Assert.AreEqual(0, itemsSource.Count);
      Assert.AreEqual(0, itemsSource.CountOnCurrentPage);
    }

    [Test]
    public void GroupByNonExistingProperty()
    {
      Group("NonExistingProperty");

      // DataGridItemsSource will display ungrouped original items

      Assert.AreEqual(8, _dataGridItemsSource.Count);
      Assert.AreEqual(8, _dataGridItemsSource.CountOnCurrentPage);

      int index = 0;
      foreach (object o in _dataGridItemsSource)
      {
        Assert.AreSame(o, _data[index]);
        index++;
      }
    }

    [Test]
    public void NullPropertyValue()
    {
      ObservableCollection<TestClass1> data = new ObservableCollection<TestClass1>();
      data.Add(new TestClass1() { Property1 = 42, Property2 = null });
      data.Add(new TestClass1() { Property1 = 9, Property2 = "Apple" });
      DataGridItemsSource itemsSource = new DataGridItemsSource(data);

      Group(itemsSource, "Property2");

      // Expected:

      // Apple
      //   9, Apple
      // NULL
      //   42, null

      Assert.AreEqual(2, itemsSource.Count);
      Assert.AreEqual(4, itemsSource.CountOnCurrentPage);

      DataGridGroup group = itemsSource.GetItemOnCurrentPageAt(0) as DataGridGroup;
      Assert.IsNotNull(group);
      Assert.AreEqual("Apple", group.GroupName);
      Assert.AreEqual(0, group.Level);
      Assert.AreEqual(0, group.Groups.Count);

      TestClass1 item = itemsSource.GetItemOnCurrentPageAt(1) as TestClass1;
      Assert.IsNotNull(item);
      Assert.AreEqual(9, item.Property1);
      Assert.AreEqual("Apple", item.Property2);

      Assert.AreEqual(1, group.InternalChildren.Count);
      Assert.AreSame(item, group.InternalChildren[0]);
      Assert.AreEqual(1, group.Children.Count);
      Assert.AreSame(item, group.Children[0]);

      group = itemsSource.GetItemOnCurrentPageAt(2) as DataGridGroup;
      Assert.IsNotNull(group);
      Assert.AreEqual("NULL", group.GroupName);
      Assert.AreEqual(0, group.Level);
      Assert.AreEqual(0, group.Groups.Count);

      item = itemsSource.GetItemOnCurrentPageAt(3) as TestClass1;
      Assert.IsNotNull(item);
      Assert.AreEqual(42, item.Property1);
      Assert.IsNull(item.Property2);

      Assert.AreEqual(1, group.InternalChildren.Count);
      Assert.AreSame(item, group.InternalChildren[0]);
      Assert.AreEqual(1, group.Children.Count);
      Assert.AreSame(item, group.Children[0]);
    }

    #region Collapse/Expand tests

    [Test]
    public void CollapsingGroupHidesItems()
    {
      Group("Property1");
      DataGridGroup group = _dataGridItemsSource.GetItemOnCurrentPageAt(4) as DataGridGroup;
      group.IsExpanded = false;

      //Expected:

      // 3
      //   3, Orange
      //   3, Pear
      //   3, Orange
      // 5
      // 7
      //   7, Apple
      //   7, Pear

      Assert.AreEqual(8, _dataGridItemsSource.Count); // TODO: The Count property currently returns the count of the sorted collection before the grouping is applied. This means expanded group children are included in the count. This needs to be revised.
      Assert.AreEqual(8, _dataGridItemsSource.CountOnCurrentPage);

      // Group 3

      group = _dataGridItemsSource.GetItemOnCurrentPageAt(0) as DataGridGroup;
      Assert.IsNotNull(group);
      Assert.AreEqual("3", group.GroupName);
      Assert.AreEqual(0, group.Level);
      Assert.AreEqual(0, group.Groups.Count);

      Assert.AreSame(_orange3A, _dataGridItemsSource.GetItemOnCurrentPageAt(1));
      Assert.AreSame(_pear3, _dataGridItemsSource.GetItemOnCurrentPageAt(2));
      Assert.AreSame(_orange3B, _dataGridItemsSource.GetItemOnCurrentPageAt(3));

      Assert.AreEqual(3, group.InternalChildren.Count);
      Assert.AreSame(_orange3A, group.InternalChildren[0]);
      Assert.AreSame(_pear3, group.InternalChildren[1]);
      Assert.AreSame(_orange3B, group.InternalChildren[2]);
      Assert.AreEqual(3, group.Children.Count);
      Assert.AreSame(_orange3A, group.Children[0]);
      Assert.AreSame(_pear3, group.Children[1]);
      Assert.AreSame(_orange3B, group.Children[2]);

      // Group 5

      group = _dataGridItemsSource.GetItemOnCurrentPageAt(4) as DataGridGroup;
      Assert.IsNotNull(group);
      Assert.AreEqual("5", group.GroupName);
      Assert.AreEqual(0, group.Level);
      Assert.AreEqual(0, group.Groups.Count);
      Assert.AreEqual(3, group.InternalChildren.Count);
      Assert.AreEqual(3, group.Children.Count);

      // Group 7

      group = _dataGridItemsSource.GetItemOnCurrentPageAt(5) as DataGridGroup;
      Assert.IsNotNull(group);
      Assert.AreEqual("7", group.GroupName);
      Assert.AreEqual(0, group.Level);
      Assert.AreEqual(0, group.Groups.Count);

      Assert.AreSame(_apple7, _dataGridItemsSource.GetItemOnCurrentPageAt(6));
      Assert.AreSame(_pear7, _dataGridItemsSource.GetItemOnCurrentPageAt(7));

      Assert.AreEqual(2, group.InternalChildren.Count);
      Assert.AreSame(_apple7, group.InternalChildren[0]);
      Assert.AreSame(_pear7, group.InternalChildren[1]);
      Assert.AreEqual(2, group.Children.Count);
      Assert.AreSame(_apple7, group.Children[0]);
      Assert.AreSame(_pear7, group.Children[1]);
    }

    [Test]
    public void ExpandingGroupDisplaysItems()
    {
      Group("Property1");
      DataGridGroup group = _dataGridItemsSource.GetItemOnCurrentPageAt(4) as DataGridGroup;
      group.IsExpanded = false;
      group.IsExpanded = true;

      //Expected:

      // 3
      //   3, Orange
      //   3, Pear
      //   3, Orange
      // 5
      //   5, Pear
      //   5, Apple
      //   5, Orange
      // 7
      //   7, Apple
      //   7, Pear

      Assert.AreEqual(8, _dataGridItemsSource.Count);
      Assert.AreEqual(11, _dataGridItemsSource.CountOnCurrentPage);

      // Group 3

      group = _dataGridItemsSource.GetItemOnCurrentPageAt(0) as DataGridGroup;
      Assert.IsNotNull(group);
      Assert.AreEqual("3", group.GroupName);
      Assert.AreEqual(0, group.Level);
      Assert.AreEqual(0, group.Groups.Count);

      Assert.AreSame(_orange3A, _dataGridItemsSource.GetItemOnCurrentPageAt(1));
      Assert.AreSame(_pear3, _dataGridItemsSource.GetItemOnCurrentPageAt(2));
      Assert.AreSame(_orange3B, _dataGridItemsSource.GetItemOnCurrentPageAt(3));

      Assert.AreEqual(3, group.InternalChildren.Count);
      Assert.AreSame(_orange3A, group.InternalChildren[0]);
      Assert.AreSame(_pear3, group.InternalChildren[1]);
      Assert.AreSame(_orange3B, group.InternalChildren[2]);
      Assert.AreEqual(3, group.Children.Count);
      Assert.AreSame(_orange3A, group.Children[0]);
      Assert.AreSame(_pear3, group.Children[1]);
      Assert.AreSame(_orange3B, group.Children[2]);

      // Group 5

      group = _dataGridItemsSource.GetItemOnCurrentPageAt(4) as DataGridGroup;
      Assert.IsNotNull(group);
      Assert.AreEqual("5", group.GroupName);
      Assert.AreEqual(0, group.Level);
      Assert.AreEqual(0, group.Groups.Count);

      Assert.AreSame(_pear5, _dataGridItemsSource.GetItemOnCurrentPageAt(5));
      Assert.AreSame(_apple5, _dataGridItemsSource.GetItemOnCurrentPageAt(6));
      Assert.AreSame(_orange5, _dataGridItemsSource.GetItemOnCurrentPageAt(7));

      Assert.AreEqual(3, group.InternalChildren.Count);
      Assert.AreSame(_pear5, group.InternalChildren[0]);
      Assert.AreSame(_apple5, group.InternalChildren[1]);
      Assert.AreSame(_orange5, group.InternalChildren[2]);
      Assert.AreEqual(3, group.Children.Count);
      Assert.AreSame(_pear5, group.Children[0]);
      Assert.AreSame(_apple5, group.Children[1]);
      Assert.AreSame(_orange5, group.Children[2]);

      // Group 7

      group = _dataGridItemsSource.GetItemOnCurrentPageAt(8) as DataGridGroup;
      Assert.IsNotNull(group);
      Assert.AreEqual("7", group.GroupName);
      Assert.AreEqual(0, group.Level);
      Assert.AreEqual(0, group.Groups.Count);

      Assert.AreSame(_apple7, _dataGridItemsSource.GetItemOnCurrentPageAt(9));
      Assert.AreSame(_pear7, _dataGridItemsSource.GetItemOnCurrentPageAt(10));

      Assert.AreEqual(2, group.InternalChildren.Count);
      Assert.AreSame(_apple7, group.InternalChildren[0]);
      Assert.AreSame(_pear7, group.InternalChildren[1]);
      Assert.AreEqual(2, group.Children.Count);
      Assert.AreSame(_apple7, group.Children[0]);
      Assert.AreSame(_pear7, group.Children[1]);
    }

    [Test]
    public void CollapseNestedGroupd()
    {
      Group("Property2", "Property1");

      DataGridGroup group = _dataGridItemsSource.GetItemOnCurrentPageAt(6) as DataGridGroup;
      group.IsExpanded = false; // Collapse group Orange/3

      // Expected:

      // Apple
      //   5
      //     5, Apple
      //   7
      //     7, Apple
      // Orange
      //   3
      //   5
      //     5, Orange
      // Pear
      //   3
      //     3, Pear
      //   5
      //     5, Pear
      //   7
      //     7, Pear

      Assert.AreEqual(8, _dataGridItemsSource.Count);
      Assert.AreEqual(16, _dataGridItemsSource.CountOnCurrentPage);

      // Group Apple

      group = _dataGridItemsSource.GetItemOnCurrentPageAt(0) as DataGridGroup;
      Assert.IsNotNull(group);
      Assert.AreEqual("Apple", group.GroupName);

      DataGridGroup subGroup1 = _dataGridItemsSource.GetItemOnCurrentPageAt(1) as DataGridGroup;
      Assert.IsNotNull(subGroup1);
      Assert.AreEqual("5", subGroup1.GroupName);

      Assert.AreSame(_apple5, _dataGridItemsSource.GetItemOnCurrentPageAt(2));

      DataGridGroup subGroup2 = _dataGridItemsSource.GetItemOnCurrentPageAt(3) as DataGridGroup;
      Assert.IsNotNull(subGroup2);
      Assert.AreEqual("7", subGroup2.GroupName);

      Assert.AreSame(_apple7, _dataGridItemsSource.GetItemOnCurrentPageAt(4));

      // Group Orange

      group = _dataGridItemsSource.GetItemOnCurrentPageAt(5) as DataGridGroup;
      Assert.IsNotNull(group);
      Assert.AreEqual("Orange", group.GroupName);

      subGroup1 = _dataGridItemsSource.GetItemOnCurrentPageAt(6) as DataGridGroup;
      Assert.IsNotNull(subGroup1);
      Assert.AreEqual("3", subGroup1.GroupName);
      Assert.AreEqual(1, subGroup1.Level);

      // Notice that there are no items between these subgroups because Orange/3 is collapsed.

      subGroup2 = _dataGridItemsSource.GetItemOnCurrentPageAt(7) as DataGridGroup;
      Assert.IsNotNull(subGroup2);
      Assert.AreEqual("5", subGroup2.GroupName);

      Assert.AreSame(_orange5, _dataGridItemsSource.GetItemOnCurrentPageAt(8));

      // Group Pear

      group = _dataGridItemsSource.GetItemOnCurrentPageAt(9) as DataGridGroup;
      Assert.IsNotNull(group);
      Assert.AreEqual("Pear", group.GroupName);

      subGroup1 = _dataGridItemsSource.GetItemOnCurrentPageAt(10) as DataGridGroup;
      Assert.IsNotNull(subGroup1);
      Assert.AreEqual("3", subGroup1.GroupName);

      Assert.AreSame(_pear3, _dataGridItemsSource.GetItemOnCurrentPageAt(11));

      subGroup2 = _dataGridItemsSource.GetItemOnCurrentPageAt(12) as DataGridGroup;
      Assert.IsNotNull(subGroup2);
      Assert.AreEqual("5", subGroup2.GroupName);

      Assert.AreSame(_pear5, _dataGridItemsSource.GetItemOnCurrentPageAt(13));

      DataGridGroup subGroup3 = _dataGridItemsSource.GetItemOnCurrentPageAt(14) as DataGridGroup;
      Assert.IsNotNull(subGroup3);
      Assert.AreEqual("7", subGroup3.GroupName);

      Assert.AreSame(_pear7, _dataGridItemsSource.GetItemOnCurrentPageAt(15));
    }

    [Test]
    public void ExpandNestedGroup()
    {
      Group("Property2", "Property1");

      DataGridGroup group = _dataGridItemsSource.GetItemOnCurrentPageAt(6) as DataGridGroup;
      group.IsExpanded = false; // Collapse group Orange/3
      group.IsExpanded = true; // Expand again.

      // Expected:

      // Apple
      //   5
      //     5, Apple
      //   7
      //     7, Apple
      // Orange
      //   3
      //     3, Orange
      //     3, Orange
      //   5
      //     5, Orange
      // Pear
      //   3
      //     3, Pear
      //   5
      //     5, Pear
      //   7
      //     7, Pear

      Assert.AreEqual(8, _dataGridItemsSource.Count);
      Assert.AreEqual(18, _dataGridItemsSource.CountOnCurrentPage);

      // Group Apple

      group = _dataGridItemsSource.GetItemOnCurrentPageAt(0) as DataGridGroup;
      Assert.IsNotNull(group);
      Assert.AreEqual("Apple", group.GroupName);

      DataGridGroup subGroup1 = _dataGridItemsSource.GetItemOnCurrentPageAt(1) as DataGridGroup;
      Assert.IsNotNull(subGroup1);
      Assert.AreEqual("5", subGroup1.GroupName);

      Assert.AreSame(_apple5, _dataGridItemsSource.GetItemOnCurrentPageAt(2));

      DataGridGroup subGroup2 = _dataGridItemsSource.GetItemOnCurrentPageAt(3) as DataGridGroup;
      Assert.IsNotNull(subGroup2);
      Assert.AreEqual("7", subGroup2.GroupName);

      Assert.AreSame(_apple7, _dataGridItemsSource.GetItemOnCurrentPageAt(4));

      // Group Orange

      group = _dataGridItemsSource.GetItemOnCurrentPageAt(5) as DataGridGroup;
      Assert.IsNotNull(group);
      Assert.AreEqual("Orange", group.GroupName);

      subGroup1 = _dataGridItemsSource.GetItemOnCurrentPageAt(6) as DataGridGroup;
      Assert.IsNotNull(subGroup1);
      Assert.AreEqual("3", subGroup1.GroupName);
      Assert.AreEqual(1, subGroup1.Level);

      // Orange/3 items are now displayed again.

      Assert.AreSame(_orange3A, _dataGridItemsSource.GetItemOnCurrentPageAt(7));
      Assert.AreSame(_orange3B, _dataGridItemsSource.GetItemOnCurrentPageAt(8));

      subGroup2 = _dataGridItemsSource.GetItemOnCurrentPageAt(9) as DataGridGroup;
      Assert.IsNotNull(subGroup2);
      Assert.AreEqual("5", subGroup2.GroupName);

      Assert.AreSame(_orange5, _dataGridItemsSource.GetItemOnCurrentPageAt(10));

      // Group Pear

      group = _dataGridItemsSource.GetItemOnCurrentPageAt(11) as DataGridGroup;
      Assert.IsNotNull(group);
      Assert.AreEqual("Pear", group.GroupName);

      subGroup1 = _dataGridItemsSource.GetItemOnCurrentPageAt(12) as DataGridGroup;
      Assert.IsNotNull(subGroup1);
      Assert.AreEqual("3", subGroup1.GroupName);

      Assert.AreSame(_pear3, _dataGridItemsSource.GetItemOnCurrentPageAt(13));

      subGroup2 = _dataGridItemsSource.GetItemOnCurrentPageAt(14) as DataGridGroup;
      Assert.IsNotNull(subGroup2);
      Assert.AreEqual("5", subGroup2.GroupName);

      Assert.AreSame(_pear5, _dataGridItemsSource.GetItemOnCurrentPageAt(15));

      DataGridGroup subGroup3 = _dataGridItemsSource.GetItemOnCurrentPageAt(16) as DataGridGroup;
      Assert.IsNotNull(subGroup3);
      Assert.AreEqual("7", subGroup3.GroupName);

      Assert.AreSame(_pear7, _dataGridItemsSource.GetItemOnCurrentPageAt(17));
    }

    [Test]
    public void CollapseParentGroup()
    {
      Group("Property2", "Property1");

      DataGridGroup group = _dataGridItemsSource.GetItemOnCurrentPageAt(5) as DataGridGroup;
      group.IsExpanded = false; // Collapse Orange parent group

      // Expected:

      // Apple
      //   5
      //     5, Apple
      //   7
      //     7, Apple
      // Orange
      // Pear
      //   3
      //     3, Pear
      //   5
      //     5, Pear
      //   7
      //     7, Pear

      Assert.AreEqual(8, _dataGridItemsSource.Count);
      Assert.AreEqual(13, _dataGridItemsSource.CountOnCurrentPage);

      // Group Apple

      group = _dataGridItemsSource.GetItemOnCurrentPageAt(0) as DataGridGroup;
      Assert.IsNotNull(group);
      Assert.AreEqual("Apple", group.GroupName);

      DataGridGroup subGroup1 = _dataGridItemsSource.GetItemOnCurrentPageAt(1) as DataGridGroup;
      Assert.IsNotNull(subGroup1);
      Assert.AreEqual("5", subGroup1.GroupName);

      Assert.AreSame(_apple5, _dataGridItemsSource.GetItemOnCurrentPageAt(2));

      DataGridGroup subGroup2 = _dataGridItemsSource.GetItemOnCurrentPageAt(3) as DataGridGroup;
      Assert.IsNotNull(subGroup2);
      Assert.AreEqual("7", subGroup2.GroupName);

      Assert.AreSame(_apple7, _dataGridItemsSource.GetItemOnCurrentPageAt(4));

      // Group Orange

      group = _dataGridItemsSource.GetItemOnCurrentPageAt(5) as DataGridGroup;
      Assert.IsNotNull(group);
      Assert.AreEqual("Orange", group.GroupName);

      // None of the Orange subitems are displayed now.
      // Check that the expansion state of the hidden subgroups are still expanded:
      Assert.IsTrue(group.Groups["3"].IsExpanded);
      Assert.IsTrue(group.Groups["5"].IsExpanded);

      // Group Pear

      group = _dataGridItemsSource.GetItemOnCurrentPageAt(6) as DataGridGroup;
      Assert.IsNotNull(group);
      Assert.AreEqual("Pear", group.GroupName);

      subGroup1 = _dataGridItemsSource.GetItemOnCurrentPageAt(7) as DataGridGroup;
      Assert.IsNotNull(subGroup1);
      Assert.AreEqual("3", subGroup1.GroupName);

      Assert.AreSame(_pear3, _dataGridItemsSource.GetItemOnCurrentPageAt(8));

      subGroup2 = _dataGridItemsSource.GetItemOnCurrentPageAt(9) as DataGridGroup;
      Assert.IsNotNull(subGroup2);
      Assert.AreEqual("5", subGroup2.GroupName);

      Assert.AreSame(_pear5, _dataGridItemsSource.GetItemOnCurrentPageAt(10));

      DataGridGroup subGroup3 = _dataGridItemsSource.GetItemOnCurrentPageAt(11) as DataGridGroup;
      Assert.IsNotNull(subGroup3);
      Assert.AreEqual("7", subGroup3.GroupName);

      Assert.AreSame(_pear7, _dataGridItemsSource.GetItemOnCurrentPageAt(12));
    }

    [Test]
    public void ExpandingParentGroupDoesNotCollapseNestedGroup()
    {
      Group("Property2", "Property1");

      DataGridGroup group = _dataGridItemsSource.GetItemOnCurrentPageAt(5) as DataGridGroup;
      group.IsExpanded = false; // Collapse Orange parent group
      group.IsExpanded = true; // Expand again.

      // Expected:

      // Apple
      //   5
      //     5, Apple
      //   7
      //     7, Apple
      // Orange
      //   3
      //     3, Orange
      //     3, Orange
      //   5
      //     5, Orange
      // Pear
      //   3
      //     3, Pear
      //   5
      //     5, Pear
      //   7
      //     7, Pear

      Assert.AreEqual(8, _dataGridItemsSource.Count);
      Assert.AreEqual(18, _dataGridItemsSource.CountOnCurrentPage);

      // Group Apple

      group = _dataGridItemsSource.GetItemOnCurrentPageAt(0) as DataGridGroup;
      Assert.IsNotNull(group);
      Assert.AreEqual("Apple", group.GroupName);

      DataGridGroup subGroup1 = _dataGridItemsSource.GetItemOnCurrentPageAt(1) as DataGridGroup;
      Assert.IsNotNull(subGroup1);
      Assert.AreEqual("5", subGroup1.GroupName);

      Assert.AreSame(_apple5, _dataGridItemsSource.GetItemOnCurrentPageAt(2));

      DataGridGroup subGroup2 = _dataGridItemsSource.GetItemOnCurrentPageAt(3) as DataGridGroup;
      Assert.IsNotNull(subGroup2);
      Assert.AreEqual("7", subGroup2.GroupName);

      Assert.AreSame(_apple7, _dataGridItemsSource.GetItemOnCurrentPageAt(4));

      // Group Orange

      group = _dataGridItemsSource.GetItemOnCurrentPageAt(5) as DataGridGroup;
      Assert.IsNotNull(group);
      Assert.AreEqual("Orange", group.GroupName);

      // All sub groups and items are displayed. No subgroups were inadvertently collapsed:

      subGroup1 = _dataGridItemsSource.GetItemOnCurrentPageAt(6) as DataGridGroup;
      Assert.IsNotNull(subGroup1);
      Assert.AreEqual("3", subGroup1.GroupName);
      Assert.AreEqual(1, subGroup1.Level);

      Assert.AreSame(_orange3A, _dataGridItemsSource.GetItemOnCurrentPageAt(7));
      Assert.AreSame(_orange3B, _dataGridItemsSource.GetItemOnCurrentPageAt(8));

      subGroup2 = _dataGridItemsSource.GetItemOnCurrentPageAt(9) as DataGridGroup;
      Assert.IsNotNull(subGroup2);
      Assert.AreEqual("5", subGroup2.GroupName);

      Assert.AreSame(_orange5, _dataGridItemsSource.GetItemOnCurrentPageAt(10));

      // Group Pear

      group = _dataGridItemsSource.GetItemOnCurrentPageAt(11) as DataGridGroup;
      Assert.IsNotNull(group);
      Assert.AreEqual("Pear", group.GroupName);

      subGroup1 = _dataGridItemsSource.GetItemOnCurrentPageAt(12) as DataGridGroup;
      Assert.IsNotNull(subGroup1);
      Assert.AreEqual("3", subGroup1.GroupName);

      Assert.AreSame(_pear3, _dataGridItemsSource.GetItemOnCurrentPageAt(13));

      subGroup2 = _dataGridItemsSource.GetItemOnCurrentPageAt(14) as DataGridGroup;
      Assert.IsNotNull(subGroup2);
      Assert.AreEqual("5", subGroup2.GroupName);

      Assert.AreSame(_pear5, _dataGridItemsSource.GetItemOnCurrentPageAt(15));

      DataGridGroup subGroup3 = _dataGridItemsSource.GetItemOnCurrentPageAt(16) as DataGridGroup;
      Assert.IsNotNull(subGroup3);
      Assert.AreEqual("7", subGroup3.GroupName);

      Assert.AreSame(_pear7, _dataGridItemsSource.GetItemOnCurrentPageAt(17));
    }

    [Test]
    public void ExpandingParentGroupDoesNotExpandNestedGroup()
    {
      Group("Property2", "Property1");

      DataGridGroup group = _dataGridItemsSource.GetItemOnCurrentPageAt(6) as DataGridGroup;
      group.IsExpanded = false; // Collapse group Orange/3
      group = _dataGridItemsSource.GetItemOnCurrentPageAt(5) as DataGridGroup;
      group.IsExpanded = false; // Collapse Orange parent group.
      group.IsExpanded = true; // Expand parent again - test that Orange/3 is still collapsed.

      // Expected:

      // Apple
      //   5
      //     5, Apple
      //   7
      //     7, Apple
      // Orange
      //   3
      //   5
      //     5, Orange
      // Pear
      //   3
      //     3, Pear
      //   5
      //     5, Pear
      //   7
      //     7, Pear

      Assert.AreEqual(8, _dataGridItemsSource.Count);
      Assert.AreEqual(16, _dataGridItemsSource.CountOnCurrentPage);

      // Group Apple

      group = _dataGridItemsSource.GetItemOnCurrentPageAt(0) as DataGridGroup;
      Assert.IsNotNull(group);
      Assert.AreEqual("Apple", group.GroupName);

      DataGridGroup subGroup1 = _dataGridItemsSource.GetItemOnCurrentPageAt(1) as DataGridGroup;
      Assert.IsNotNull(subGroup1);
      Assert.AreEqual("5", subGroup1.GroupName);

      Assert.AreSame(_apple5, _dataGridItemsSource.GetItemOnCurrentPageAt(2));

      DataGridGroup subGroup2 = _dataGridItemsSource.GetItemOnCurrentPageAt(3) as DataGridGroup;
      Assert.IsNotNull(subGroup2);
      Assert.AreEqual("7", subGroup2.GroupName);

      Assert.AreSame(_apple7, _dataGridItemsSource.GetItemOnCurrentPageAt(4));

      // Group Orange

      group = _dataGridItemsSource.GetItemOnCurrentPageAt(5) as DataGridGroup;
      Assert.IsNotNull(group);
      Assert.AreEqual("Orange", group.GroupName);

      subGroup1 = _dataGridItemsSource.GetItemOnCurrentPageAt(6) as DataGridGroup;
      Assert.IsNotNull(subGroup1);
      Assert.AreEqual("3", subGroup1.GroupName);
      Assert.AreEqual(1, subGroup1.Level);

      // Orange/3 items are not displayed as the sub group is still collapsed.

      subGroup2 = _dataGridItemsSource.GetItemOnCurrentPageAt(7) as DataGridGroup;
      Assert.IsNotNull(subGroup2);
      Assert.AreEqual("5", subGroup2.GroupName);

      Assert.AreSame(_orange5, _dataGridItemsSource.GetItemOnCurrentPageAt(8));

      // Group Pear

      group = _dataGridItemsSource.GetItemOnCurrentPageAt(9) as DataGridGroup;
      Assert.IsNotNull(group);
      Assert.AreEqual("Pear", group.GroupName);

      subGroup1 = _dataGridItemsSource.GetItemOnCurrentPageAt(10) as DataGridGroup;
      Assert.IsNotNull(subGroup1);
      Assert.AreEqual("3", subGroup1.GroupName);

      Assert.AreSame(_pear3, _dataGridItemsSource.GetItemOnCurrentPageAt(11));

      subGroup2 = _dataGridItemsSource.GetItemOnCurrentPageAt(12) as DataGridGroup;
      Assert.IsNotNull(subGroup2);
      Assert.AreEqual("5", subGroup2.GroupName);

      Assert.AreSame(_pear5, _dataGridItemsSource.GetItemOnCurrentPageAt(13));

      DataGridGroup subGroup3 = _dataGridItemsSource.GetItemOnCurrentPageAt(14) as DataGridGroup;
      Assert.IsNotNull(subGroup3);
      Assert.AreEqual("7", subGroup3.GroupName);

      Assert.AreSame(_pear7, _dataGridItemsSource.GetItemOnCurrentPageAt(15));
    }

    [Test]
    public void CollapseGroupWithinCollapsedParent()
    {
      Group("Property2", "Property1");

      DataGridGroup group = _dataGridItemsSource.GetItemOnCurrentPageAt(5) as DataGridGroup;
      group.IsExpanded = false; // Collapse Orange parent group
      group.Groups["3"].IsExpanded = false; // Collapse group Orange/3 which is inside collapsed parent group.

      // Expected:

      // Apple
      //   5
      //     5, Apple
      //   7
      //     7, Apple
      // Orange
      // Pear
      //   3
      //     3, Pear
      //   5
      //     5, Pear
      //   7
      //     7, Pear

      Assert.AreEqual(8, _dataGridItemsSource.Count);
      Assert.AreEqual(13, _dataGridItemsSource.CountOnCurrentPage);

      // Group Apple

      group = _dataGridItemsSource.GetItemOnCurrentPageAt(0) as DataGridGroup;
      Assert.IsNotNull(group);
      Assert.AreEqual("Apple", group.GroupName);

      DataGridGroup subGroup1 = _dataGridItemsSource.GetItemOnCurrentPageAt(1) as DataGridGroup;
      Assert.IsNotNull(subGroup1);
      Assert.AreEqual("5", subGroup1.GroupName);

      Assert.AreSame(_apple5, _dataGridItemsSource.GetItemOnCurrentPageAt(2));

      DataGridGroup subGroup2 = _dataGridItemsSource.GetItemOnCurrentPageAt(3) as DataGridGroup;
      Assert.IsNotNull(subGroup2);
      Assert.AreEqual("7", subGroup2.GroupName);

      Assert.AreSame(_apple7, _dataGridItemsSource.GetItemOnCurrentPageAt(4));

      // Group Orange

      group = _dataGridItemsSource.GetItemOnCurrentPageAt(5) as DataGridGroup;
      Assert.IsNotNull(group);
      Assert.AreEqual("Orange", group.GroupName);

      // None of the Orange items are displayed because the Orange parent group is collapsed. And also:
      Assert.IsFalse(group.Groups["3"].IsExpanded); // sub group 3 is correctly collapsed.
      Assert.IsTrue(group.Groups["5"].IsExpanded); // sub group 5 is still expanded as it was not touched.

      // Group Pear

      group = _dataGridItemsSource.GetItemOnCurrentPageAt(6) as DataGridGroup;
      Assert.IsNotNull(group);
      Assert.AreEqual("Pear", group.GroupName);

      subGroup1 = _dataGridItemsSource.GetItemOnCurrentPageAt(7) as DataGridGroup;
      Assert.IsNotNull(subGroup1);
      Assert.AreEqual("3", subGroup1.GroupName);

      Assert.AreSame(_pear3, _dataGridItemsSource.GetItemOnCurrentPageAt(8));

      subGroup2 = _dataGridItemsSource.GetItemOnCurrentPageAt(9) as DataGridGroup;
      Assert.IsNotNull(subGroup2);
      Assert.AreEqual("5", subGroup2.GroupName);

      Assert.AreSame(_pear5, _dataGridItemsSource.GetItemOnCurrentPageAt(10));

      DataGridGroup subGroup3 = _dataGridItemsSource.GetItemOnCurrentPageAt(11) as DataGridGroup;
      Assert.IsNotNull(subGroup3);
      Assert.AreEqual("7", subGroup3.GroupName);

      Assert.AreSame(_pear7, _dataGridItemsSource.GetItemOnCurrentPageAt(12));
    }

    [Test]
    public void CollapseGroupWithinCollapsedParent_ThenExpandParent()
    {
      Group("Property2", "Property1");

      DataGridGroup group = _dataGridItemsSource.GetItemOnCurrentPageAt(5) as DataGridGroup;
      group.IsExpanded = false; // Collapse Orange parent group
      group.Groups["3"].IsExpanded = false; // Collapse sub group Orange/3
      group.IsExpanded = true; // Expand Orange parent group again - test that Orange/3 is still collapsed.

      // Expected:

      // Apple
      //   5
      //     5, Apple
      //   7
      //     7, Apple
      // Orange
      //   3
      //   5
      //     5, Orange
      // Pear
      //   3
      //     3, Pear
      //   5
      //     5, Pear
      //   7
      //     7, Pear

      Assert.AreEqual(8, _dataGridItemsSource.Count);
      Assert.AreEqual(16, _dataGridItemsSource.CountOnCurrentPage);

      // Group Apple

      group = _dataGridItemsSource.GetItemOnCurrentPageAt(0) as DataGridGroup;
      Assert.IsNotNull(group);
      Assert.AreEqual("Apple", group.GroupName);

      DataGridGroup subGroup1 = _dataGridItemsSource.GetItemOnCurrentPageAt(1) as DataGridGroup;
      Assert.IsNotNull(subGroup1);
      Assert.AreEqual("5", subGroup1.GroupName);

      Assert.AreSame(_apple5, _dataGridItemsSource.GetItemOnCurrentPageAt(2));

      DataGridGroup subGroup2 = _dataGridItemsSource.GetItemOnCurrentPageAt(3) as DataGridGroup;
      Assert.IsNotNull(subGroup2);
      Assert.AreEqual("7", subGroup2.GroupName);

      Assert.AreSame(_apple7, _dataGridItemsSource.GetItemOnCurrentPageAt(4));

      // Group Orange

      group = _dataGridItemsSource.GetItemOnCurrentPageAt(5) as DataGridGroup;
      Assert.IsNotNull(group);
      Assert.AreEqual("Orange", group.GroupName);

      subGroup1 = _dataGridItemsSource.GetItemOnCurrentPageAt(6) as DataGridGroup;
      Assert.IsNotNull(subGroup1);
      Assert.AreEqual("3", subGroup1.GroupName);
      Assert.AreEqual(1, subGroup1.Level);

      // Orange/3 items are not displayed as the sub group is still collapsed.

      subGroup2 = _dataGridItemsSource.GetItemOnCurrentPageAt(7) as DataGridGroup;
      Assert.IsNotNull(subGroup2);
      Assert.AreEqual("5", subGroup2.GroupName);

      Assert.AreSame(_orange5, _dataGridItemsSource.GetItemOnCurrentPageAt(8));

      // Group Pear

      group = _dataGridItemsSource.GetItemOnCurrentPageAt(9) as DataGridGroup;
      Assert.IsNotNull(group);
      Assert.AreEqual("Pear", group.GroupName);

      subGroup1 = _dataGridItemsSource.GetItemOnCurrentPageAt(10) as DataGridGroup;
      Assert.IsNotNull(subGroup1);
      Assert.AreEqual("3", subGroup1.GroupName);

      Assert.AreSame(_pear3, _dataGridItemsSource.GetItemOnCurrentPageAt(11));

      subGroup2 = _dataGridItemsSource.GetItemOnCurrentPageAt(12) as DataGridGroup;
      Assert.IsNotNull(subGroup2);
      Assert.AreEqual("5", subGroup2.GroupName);

      Assert.AreSame(_pear5, _dataGridItemsSource.GetItemOnCurrentPageAt(13));

      DataGridGroup subGroup3 = _dataGridItemsSource.GetItemOnCurrentPageAt(14) as DataGridGroup;
      Assert.IsNotNull(subGroup3);
      Assert.AreEqual("7", subGroup3.GroupName);

      Assert.AreSame(_pear7, _dataGridItemsSource.GetItemOnCurrentPageAt(15));
    }

    [Test]
    public void ExpandGroupWithinCollapsedParent()
    {
      Group("Property2", "Property1");

      DataGridGroup group = _dataGridItemsSource.GetItemOnCurrentPageAt(6) as DataGridGroup;
      group.IsExpanded = false; // Collapse group Orange/3
      group = _dataGridItemsSource.GetItemOnCurrentPageAt(5) as DataGridGroup;
      group.IsExpanded = false; // Collapse Orange parent group.
      group.Groups["3"].IsExpanded = true; // Expand group Orange/3 which is inside collapsed parent group.

      // Expected:

      // Apple
      //   5
      //     5, Apple
      //   7
      //     7, Apple
      // Orange
      // Pear
      //   3
      //     3, Pear
      //   5
      //     5, Pear
      //   7
      //     7, Pear

      Assert.AreEqual(8, _dataGridItemsSource.Count);
      Assert.AreEqual(13, _dataGridItemsSource.CountOnCurrentPage);

      // Group Apple

      group = _dataGridItemsSource.GetItemOnCurrentPageAt(0) as DataGridGroup;
      Assert.IsNotNull(group);
      Assert.AreEqual("Apple", group.GroupName);

      DataGridGroup subGroup1 = _dataGridItemsSource.GetItemOnCurrentPageAt(1) as DataGridGroup;
      Assert.IsNotNull(subGroup1);
      Assert.AreEqual("5", subGroup1.GroupName);

      Assert.AreSame(_apple5, _dataGridItemsSource.GetItemOnCurrentPageAt(2));

      DataGridGroup subGroup2 = _dataGridItemsSource.GetItemOnCurrentPageAt(3) as DataGridGroup;
      Assert.IsNotNull(subGroup2);
      Assert.AreEqual("7", subGroup2.GroupName);

      Assert.AreSame(_apple7, _dataGridItemsSource.GetItemOnCurrentPageAt(4));

      // Group Orange

      group = _dataGridItemsSource.GetItemOnCurrentPageAt(5) as DataGridGroup;
      Assert.IsNotNull(group);
      Assert.AreEqual("Orange", group.GroupName);

      // None of the Orange items are displayed because the Orange parent group is collapsed.
      // Both sub groups however are correctly expanded:
      Assert.IsTrue(group.Groups["3"].IsExpanded);
      Assert.IsTrue(group.Groups["5"].IsExpanded);

      // Group Pear

      group = _dataGridItemsSource.GetItemOnCurrentPageAt(6) as DataGridGroup;
      Assert.IsNotNull(group);
      Assert.AreEqual("Pear", group.GroupName);

      subGroup1 = _dataGridItemsSource.GetItemOnCurrentPageAt(7) as DataGridGroup;
      Assert.IsNotNull(subGroup1);
      Assert.AreEqual("3", subGroup1.GroupName);

      Assert.AreSame(_pear3, _dataGridItemsSource.GetItemOnCurrentPageAt(8));

      subGroup2 = _dataGridItemsSource.GetItemOnCurrentPageAt(9) as DataGridGroup;
      Assert.IsNotNull(subGroup2);
      Assert.AreEqual("5", subGroup2.GroupName);

      Assert.AreSame(_pear5, _dataGridItemsSource.GetItemOnCurrentPageAt(10));

      DataGridGroup subGroup3 = _dataGridItemsSource.GetItemOnCurrentPageAt(11) as DataGridGroup;
      Assert.IsNotNull(subGroup3);
      Assert.AreEqual("7", subGroup3.GroupName);

      Assert.AreSame(_pear7, _dataGridItemsSource.GetItemOnCurrentPageAt(12));
    }

    [Test]
    public void ExpandGroupWithinCollapsedParent_ThenExpandParent()
    {
      Group("Property2", "Property1");

      DataGridGroup group = _dataGridItemsSource.GetItemOnCurrentPageAt(6) as DataGridGroup;
      group.IsExpanded = false; // Collapse group Orange/3
      group = _dataGridItemsSource.GetItemOnCurrentPageAt(5) as DataGridGroup;
      group.IsExpanded = false; // Collapse Orange parent group.
      group.Groups["3"].IsExpanded = true; // Expand group Orange/3 which is inside collapsed parent group.
      group.IsExpanded = true; // re-expand Orange parent group - test that all groups and subgroups are expanded again.

      // Expected:

      // Apple
      //   5
      //     5, Apple
      //   7
      //     7, Apple
      // Orange
      //   3
      //     3, Orange
      //     3, Orange
      //   5
      //     5, Orange
      // Pear
      //   3
      //     3, Pear
      //   5
      //     5, Pear
      //   7
      //     7, Pear

      Assert.AreEqual(8, _dataGridItemsSource.Count);
      Assert.AreEqual(18, _dataGridItemsSource.CountOnCurrentPage);

      // Group Apple

      group = _dataGridItemsSource.GetItemOnCurrentPageAt(0) as DataGridGroup;
      Assert.IsNotNull(group);
      Assert.AreEqual("Apple", group.GroupName);

      DataGridGroup subGroup1 = _dataGridItemsSource.GetItemOnCurrentPageAt(1) as DataGridGroup;
      Assert.IsNotNull(subGroup1);
      Assert.AreEqual("5", subGroup1.GroupName);

      Assert.AreSame(_apple5, _dataGridItemsSource.GetItemOnCurrentPageAt(2));

      DataGridGroup subGroup2 = _dataGridItemsSource.GetItemOnCurrentPageAt(3) as DataGridGroup;
      Assert.IsNotNull(subGroup2);
      Assert.AreEqual("7", subGroup2.GroupName);

      Assert.AreSame(_apple7, _dataGridItemsSource.GetItemOnCurrentPageAt(4));

      // Group Orange

      group = _dataGridItemsSource.GetItemOnCurrentPageAt(5) as DataGridGroup;
      Assert.IsNotNull(group);
      Assert.AreEqual("Orange", group.GroupName);

      // All sub groups and items are displayed again.

      subGroup1 = _dataGridItemsSource.GetItemOnCurrentPageAt(6) as DataGridGroup;
      Assert.IsNotNull(subGroup1);
      Assert.AreEqual("3", subGroup1.GroupName);
      Assert.AreEqual(1, subGroup1.Level);

      Assert.AreSame(_orange3A, _dataGridItemsSource.GetItemOnCurrentPageAt(7));
      Assert.AreSame(_orange3B, _dataGridItemsSource.GetItemOnCurrentPageAt(8));

      subGroup2 = _dataGridItemsSource.GetItemOnCurrentPageAt(9) as DataGridGroup;
      Assert.IsNotNull(subGroup2);
      Assert.AreEqual("5", subGroup2.GroupName);

      Assert.AreSame(_orange5, _dataGridItemsSource.GetItemOnCurrentPageAt(10));

      // Group Pear

      group = _dataGridItemsSource.GetItemOnCurrentPageAt(11) as DataGridGroup;
      Assert.IsNotNull(group);
      Assert.AreEqual("Pear", group.GroupName);

      subGroup1 = _dataGridItemsSource.GetItemOnCurrentPageAt(12) as DataGridGroup;
      Assert.IsNotNull(subGroup1);
      Assert.AreEqual("3", subGroup1.GroupName);

      Assert.AreSame(_pear3, _dataGridItemsSource.GetItemOnCurrentPageAt(13));

      subGroup2 = _dataGridItemsSource.GetItemOnCurrentPageAt(14) as DataGridGroup;
      Assert.IsNotNull(subGroup2);
      Assert.AreEqual("5", subGroup2.GroupName);

      Assert.AreSame(_pear5, _dataGridItemsSource.GetItemOnCurrentPageAt(15));

      DataGridGroup subGroup3 = _dataGridItemsSource.GetItemOnCurrentPageAt(16) as DataGridGroup;
      Assert.IsNotNull(subGroup3);
      Assert.AreEqual("7", subGroup3.GroupName);

      Assert.AreSame(_pear7, _dataGridItemsSource.GetItemOnCurrentPageAt(17));
    }

    #endregion // Collapse/Expand tests

    private void Group(params string[] groups)
    {
      Group(_dataGridItemsSource, groups);
    }

    private void Group(DataGridItemsSource itemsSource, params string[] groups)
    {
      itemsSource.Begin();
      itemsSource.GroupDescriptions.Clear();
      if (groups != null)
      {
        foreach (string group in groups)
        {
          itemsSource.GroupDescriptions.Add(new PropertyGroupDescription(group));
        }
      }
      itemsSource.End();
    }
  }
}
