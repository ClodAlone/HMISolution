using System;
using System.Collections.Generic;
using NUnit.Framework;
using System.Collections.ObjectModel;

namespace Mindscape.WpfElements.WpfPropertyGrid.UnitTests
{
  [TestFixture]
  public class CollectionUtilsTests
  {
    [Test]
    public void Append()
    {
      List<int> list = new List<int>(new int[] { 1, 2, 3 });
      IEnumerable<int> enumeration = new int[] { 4, 5 };
      CollectionUtilities.Append(list, enumeration);
      Assert.AreEqual(5, list.Count);
      Assert.AreEqual(4, list[3]);
    }

    [Test]
    public void AddDefaultValueEntry()
    {
      Int32Collection int32Collection = new Int32Collection();
      Assert.AreEqual(0, int32Collection.Count);
      CollectionUtilities.AddDefaultValueEntry(int32Collection);
      Assert.AreEqual(1, int32Collection.Count);
      Assert.AreEqual(default(Int32), int32Collection[0]);

      List<string> stringCollection = new List<string>();
      stringCollection.Add("Fie");
      Assert.AreEqual(1, stringCollection.Count);
      CollectionUtilities.AddDefaultValueEntry(stringCollection);
      Assert.AreEqual(2, stringCollection.Count);
      Assert.AreEqual(default(string), stringCollection[1]);

      List<Person> personCollection = new List<Person>();
      CollectionUtilities.AddDefaultValueEntry(personCollection);
      Assert.IsNotNull(personCollection[0]);
    }

    [Test]
    public void CanAddToCollection()
    {
      ReadOnlyCollection<string> roc = new ReadOnlyCollection<string>(new List<string>());
      Assert.IsFalse(CollectionUtilities.CanAddToCollection(roc));

      List<string> strs = new List<string>();
      Assert.IsTrue(CollectionUtilities.CanAddToCollection(strs));

      string[] array = new string[17];
      Assert.IsFalse(CollectionUtilities.CanAddToCollection(array));

      List<string> nullList = null;
      Assert.IsFalse(CollectionUtilities.CanAddToCollection(nullList));
    }

    [Test]
    public void CannotAddToArray()
    {
      string[] array = new string[17];
      Assert.IsFalse(CollectionUtilities.CanAddToCollection(array));
    }

    [Test]
    public void CannotRemoveFromArray()
    {
      string[] array = new string[17];
      Assert.IsFalse(CollectionUtilities.CanRemoveFromCollection(array));
    }
  }
}
