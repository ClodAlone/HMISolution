using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using System.Collections.ObjectModel;
using Mindscape.WpfElements.WpfPropertyGrid.UnitTests;

namespace Mindscape.WpfElements.PropertyEditing.UnitTests
{
  [TestFixture]
  public class CollectionElementTests
  {
    [Test]
    public void FromDictionary()
    {
      Dictionary<string, object> dictionary = new Dictionary<string, object>();
      dictionary.Add("int", 27);
      dictionary.Add("address", Address.AndrewsAddress);

      List<CollectionElement> elements = new List<CollectionElement>(CollectionElement.GetCollectionElements(dictionary, null));

      Assert.AreEqual(2, elements.Count);

      CollectionElement intElement = elements.Find(delegate(CollectionElement e) { return e.HumanName == "int"; });
      Assert.IsNotNull(intElement);
      Assert.AreEqual(27, intElement.Value);
      Assert.AreEqual(typeof(int), intElement.PropertyType);
      Assert.AreEqual(0, intElement.Children.Count);
      Assert.AreEqual(1, intElement.IndexedPropertyArguments.Count);
      Assert.AreEqual("int", intElement.IndexedPropertyArguments[0]);

      CollectionElement addressElement = elements.Find(delegate(CollectionElement e) { return e.HumanName == "address"; });
      Assert.IsNotNull(addressElement);
      Assert.AreEqual(Address.AndrewsAddress, addressElement.Value);
      Assert.AreEqual(typeof(Address), addressElement.PropertyType);
      Assert.AreEqual(5, addressElement.Children.Count);
      Assert.AreEqual(1, addressElement.IndexedPropertyArguments.Count);
      Assert.AreEqual("address", addressElement.IndexedPropertyArguments[0]);

      Assert.IsTrue(intElement.CanWrite);

      dictionary["int"] = 456;
      Assert.AreEqual(456, intElement.Value);
    }

    [Test]
    public void FromList()
    {
      List<object> list = new List<object>(new object[] { 1, 2, 3, Address.AndrewsAddress });

      List<CollectionElement> elements = new List<CollectionElement>(CollectionElement.GetCollectionElements(list, null));

      Assert.AreEqual(4, elements.Count);
      Assert.AreEqual("Item[0]", elements[0].HumanName);
      Assert.AreEqual(1, elements[0].Value);
      Assert.AreEqual(3, elements[2].Value);
      Assert.AreEqual(typeof(int), elements[2].PropertyType);
      Assert.AreEqual(1, elements[1].IndexedPropertyArguments.Count);
      Assert.AreEqual(1, elements[1].IndexedPropertyArguments[0]);

      Assert.AreEqual("Item[3]", elements[3].HumanName);
      Assert.AreEqual(Address.AndrewsAddress, elements[3].Value);
      Assert.AreEqual(typeof(Address), elements[3].PropertyType);
      Assert.AreEqual(5, elements[3].Children.Count);
    }

    [Test]
    public void Scalars()
    {
      int scalar = 27;

      List<CollectionElement> elementsFromScalar = new List<CollectionElement>(CollectionElement.GetCollectionElements(scalar, null));
      Assert.IsNotNull(elementsFromScalar);
      Assert.AreEqual(0, elementsFromScalar.Count);

      List<CollectionElement> elementsFromNull = new List<CollectionElement>(CollectionElement.GetCollectionElements(null, null));
      Assert.IsNotNull(elementsFromNull);
      Assert.AreEqual(0, elementsFromNull.Count);
    }

    [Test]
    public void PropertyType()
    {
      Cat c = new Cat();
      c.Kittens.Add(new Cat());
      c.Kittens.Add(null);

      List<CollectionElement> kittenNodes = new List<CollectionElement>(CollectionElement.GetCollectionElements(c.Kittens, null));

      Assert.IsNotNull(kittenNodes[0].Value);
      Assert.AreEqual(typeof(Cat), kittenNodes[0].PropertyType);
      Assert.IsNull(kittenNodes[1].Value);
      Assert.AreEqual(typeof(Cat), kittenNodes[1].PropertyType);
    }

    [Test]
    public void RemoveFromParentCollection()
    {
      List<int> ints = new List<int>(new int[] { 0, 1, 2 });
      List<CollectionElement> nodes = new List<CollectionElement>(CollectionElement.GetCollectionElements(ints, null));

      Assert.AreEqual(3, ints.Count);
      nodes[1].RemoveFromParentCollection();
      Assert.AreEqual(2, ints.Count);
      Assert.IsFalse(ints.Contains(1));
    }

    [Test]
    public void ElementsOfReadWriteCollectionAreReadWrite()
    {
      List<string> stringsList = new List<string> { "Fie", "Zounds", "Ods Bodikins" };
      List<CollectionElement> nodes = new List<CollectionElement>(CollectionElement.GetCollectionElements(stringsList, null));

      Assert.AreEqual(3, nodes.Count);
      Assert.IsTrue(nodes[0].CanWrite);

      Dictionary<string, string> stringsDictionary = new Dictionary<string,string> { { "Fie", "Zounds" }, { "Ods Bodikins", "Gramercy" } };
      nodes = new List<CollectionElement>(CollectionElement.GetCollectionElements(stringsDictionary, null));

      Assert.AreEqual(2, nodes.Count);
      Assert.IsTrue(nodes[0].CanWrite);
    }

    [Test]
    public void ElementsOfReadOnlyCollectionAreReadOnly()
    {
      ReadOnlyCollection<string> strings = new List<string> { "Fie", "Zounds", "Ods Bodikins" }.AsReadOnly();
      List<CollectionElement> nodes = new List<CollectionElement>(CollectionElement.GetCollectionElements(strings, null));

      Assert.AreEqual(3, nodes.Count);
      Assert.IsFalse(nodes[0].CanWrite);
    }

    [Test]
    public void CollectionElementDescriptorsReportCorrectPropertyType_FromList()
    {
      List<CitizenshipStatus> statuses = new List<CitizenshipStatus> { CitizenshipStatus.Resident, CitizenshipStatus.Alien };
      var statusNodes = new List<CollectionElement>(CollectionElement.FromList(statuses, null));

      Assert.AreEqual(2, statusNodes.Count);
      Assert.AreEqual(typeof(CitizenshipStatus), statusNodes[0].Property.PropertyType);
    }

    [Test]
    public void CollectionElementDescriptorsReportCorrectPropertyType_FromDictionary()
    {
      Dictionary<string, CitizenshipStatus> statuses = new Dictionary<string, CitizenshipStatus> { { "Bob", CitizenshipStatus.Resident }, { "Alice", CitizenshipStatus.Alien } };
      var statusNodes = new List<CollectionElement>(CollectionElement.FromDictionary(statuses, null));

      Assert.AreEqual(2, statusNodes.Count);
      Assert.AreEqual(typeof(CitizenshipStatus), statusNodes[0].Property.PropertyType);
    }
  }

  [TestFixture]
  public class CollectionTypeOperationsTests
  {
    [Test]
    public void GetItem()
    {
      List<int> ints = new List<int>(new int[] { 0, 1, 4, 9, 16 });
      CollectionTypeOperations typeOps = new CollectionTypeOperations(ints.GetType(), "Item", "RemoveAt");

      int item2 = (int)(typeOps.ItemProperty.GetValue(ints, new object[] { 2 }));
      Assert.AreEqual(ints[2], item2);

      typeOps.RemovalMethod.Invoke(ints, new object[] { 3 });
      Assert.AreEqual(4, ints.Count);
      Assert.IsFalse(ints.Contains(9));
    }
  }

  public class CatCollection : List<Cat> { }

  public class Cat
  {
    private readonly CatCollection _kittens = new CatCollection();

    public CatCollection Kittens
    {
      get { return _kittens; }
    }
  }
}
