using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Mindscape.WpfElements.WpfDataGrid;
using System.Collections.ObjectModel;

namespace Mindscape.WpfElements.UnitTests.DataGridTests
{
  [TestFixture]
  public class DataGridItemsSourceTests
  {
    [Test]
    public void DefaultsToInputOrder()
    {
      DataGridItemsSource items = new DataGridItemsSource(new string[] { "Alice", "Bob", "Engelbert", "Zack" });
      var result = items.ToList();
      Assert.AreEqual("Alice", result[0]);
      Assert.AreEqual("Bob", result[1]);
      Assert.AreEqual("Engelbert", result[2]);
      Assert.AreEqual("Zack", result[3]);
    }

    [Test]
    public void CanSortByProperty()
    {
      DataGridItemsSource items = new DataGridItemsSource(new string[] { "Alice", "Bob", "Engelbert", "Zack" });
      items.Sort(typeof(string).GetProperty("Length"), SortDirection.Ascending);
      var result = items.ToList();
      Assert.AreEqual("Bob", result[0]);
      Assert.AreEqual("Zack", result[1]);
      Assert.AreEqual("Alice", result[2]);
      Assert.AreEqual("Engelbert", result[3]);
    }

    [Test]
    public void CanSortByProperty_Descending()
    {
      DataGridItemsSource items = new DataGridItemsSource(new string[] { "Alice", "Bob", "Engelbert", "Zack" });
      items.Sort(typeof(string).GetProperty("Length"), SortDirection.Descending);
      var result = items.ToList();
      Assert.AreEqual("Engelbert", result[0]);
      Assert.AreEqual("Alice", result[1]);
      Assert.AreEqual("Zack", result[2]);
      Assert.AreEqual("Bob", result[3]);
    }

    [Test]
    public void CanChangeSortOrder_DescendingToAscending()
    {
      DataGridItemsSource items = new DataGridItemsSource(new string[] { "Alice", "Bob", "Engelbert", "Zack" });
      items.Sort(typeof(string).GetProperty("Length"), SortDirection.Descending);
      items.Sort(typeof(string).GetProperty("Length"), SortDirection.Ascending);
      var result = items.ToList();
      Assert.AreEqual("Bob", result[0]);
      Assert.AreEqual("Zack", result[1]);
      Assert.AreEqual("Alice", result[2]);
      Assert.AreEqual("Engelbert", result[3]);
    }

    [Test]
    public void CanChangeSortOrder_AscendingToDescending()
    {
      DataGridItemsSource items = new DataGridItemsSource(new string[] { "Alice", "Bob", "Engelbert", "Zack" });
      items.Sort(typeof(string).GetProperty("Length"), SortDirection.Ascending);
      items.Sort(typeof(string).GetProperty("Length"), SortDirection.Descending);
      var result = items.ToList();
      Assert.AreEqual("Engelbert", result[0]);
      Assert.AreEqual("Alice", result[1]);
      Assert.AreEqual("Zack", result[2]);
      Assert.AreEqual("Bob", result[3]);
    }

    [Test]
    public void CanChangeSortOrder_AscendingToDescendingAndBack()
    {
      DataGridItemsSource items = new DataGridItemsSource(new string[] { "Alice", "Bob", "Engelbert", "Zack" });
      items.Sort(typeof(string).GetProperty("Length"), SortDirection.Ascending);
      items.Sort(typeof(string).GetProperty("Length"), SortDirection.Descending);
      items.Sort(typeof(string).GetProperty("Length"), SortDirection.Ascending);
      var result = items.ToList();
      Assert.AreEqual("Bob", result[0]);
      Assert.AreEqual("Zack", result[1]);
      Assert.AreEqual("Alice", result[2]);
      Assert.AreEqual("Engelbert", result[3]);
    }

    [Test]
    public void CanChangeSortOrder_ToNone()
    {
      DataGridItemsSource items = new DataGridItemsSource(new string[] { "Alice", "Bob", "Engelbert", "Zack" });
      items.Sort(typeof(string).GetProperty("Length"), SortDirection.Ascending);
      items.Sort(typeof(string).GetProperty("Length"), SortDirection.None);
      var result = items.ToList();
      Assert.AreEqual("Alice", result[0]);
      Assert.AreEqual("Bob", result[1]);
      Assert.AreEqual("Engelbert", result[2]);
      Assert.AreEqual("Zack", result[3]);
    }

    [Test]
    public void CanSortByCustomComparer()
    {
      DataGridItemsSource items = new DataGridItemsSource(new string[] { "Alice", "Bob", "Engelbert", "Zack" });
      items.Sort(new BySecondLetterComparer(), SortDirection.Ascending);
      var result = items.ToList();
      Assert.AreEqual("Zack", result[0]);
      Assert.AreEqual("Alice", result[1]);
      Assert.AreEqual("Engelbert", result[2]);
      Assert.AreEqual("Bob", result[3]);
    }

    [Test]
    public void CanSortByCustomComparer_Descending()
    {
      DataGridItemsSource items = new DataGridItemsSource(new string[] { "Alice", "Bob", "Engelbert", "Zack" });
      items.Sort(new BySecondLetterComparer(), SortDirection.Descending);
      var result = items.ToList();
      Assert.AreEqual("Bob", result[0]);
      Assert.AreEqual("Engelbert", result[1]);
      Assert.AreEqual("Alice", result[2]);
      Assert.AreEqual("Zack", result[3]);
    }

    [Test]
    public void CanChangeComparer_PropertyToCustom()
    {
      DataGridItemsSource items = new DataGridItemsSource(new string[] { "Alice", "Bob", "Engelbert", "Zack" });
      items.Sort(typeof(string).GetProperty("Length"), SortDirection.Ascending);
      items.Sort(new BySecondLetterComparer(), SortDirection.Ascending);
      var result = items.ToList();
      Assert.AreEqual("Zack", result[0]);
      Assert.AreEqual("Alice", result[1]);
      Assert.AreEqual("Engelbert", result[2]);
      Assert.AreEqual("Bob", result[3]);
    }

    [Test]
    public void CanChangeComparer_CustomToProperty()
    {
      DataGridItemsSource items = new DataGridItemsSource(new string[] { "Alice", "Bob", "Engelbert", "Zack" });
      items.Sort(new BySecondLetterComparer(), SortDirection.Ascending);
      items.Sort(typeof(string).GetProperty("Length"), SortDirection.Descending);
      var result = items.ToList();
      Assert.AreEqual("Engelbert", result[0]);
      Assert.AreEqual("Alice", result[1]);
      Assert.AreEqual("Zack", result[2]);
      Assert.AreEqual("Bob", result[3]);
    }

    [Test]
    [Ignore("The DisplayedItemsSource no longer listens to collection changes. The DataGrid passes the events to it from the OnItemsChanged method")]
    public void RespondsToChangesInSource_ButLosesSortOrder()  // Is this really the desired behavior?
    {
      var source = new ObservableCollection<string> { "Alice", "Bob", "Engelbert", "Zack" };
      DataGridItemsSource items = new DataGridItemsSource(source);
      items.Sort(new BySecondLetterComparer(), SortDirection.Ascending);
      source.Add("Ibsen");
      var result = items.ToList();
      Assert.AreEqual("Alice", result[0]);
      Assert.AreEqual("Bob", result[1]);
      Assert.AreEqual("Engelbert", result[2]);
      Assert.AreEqual("Zack", result[3]);
      Assert.AreEqual("Ibsen", result[4]);
    }

    [Test]
    public void CanResortAfterSourceCollectionChanges()
    {
      var source = new ObservableCollection<string> { "Alice", "Bob", "Engelbert", "Zack" };
      DataGridItemsSource items = new DataGridItemsSource(source);
      items.Sort(new BySecondLetterComparer(), SortDirection.Descending);
      source.Add("Ibsen");
      items.Sort(new BySecondLetterComparer(), SortDirection.Descending);
      var result = items.ToList();
      Assert.AreEqual("Bob", result[0]);
      Assert.AreEqual("Engelbert", result[1]);
      Assert.AreEqual("Alice", result[2]);
      Assert.AreEqual("Ibsen", result[3]);
      Assert.AreEqual("Zack", result[4]);
    }

    private class BySecondLetterComparer : IComparer<object>
    {
      public int Compare(object x, object y)
      {
        return ((string)x)[1].CompareTo(((string)y)[1]);
      }
    }
  }
}
