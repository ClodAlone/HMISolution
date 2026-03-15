using System;
using System.Reflection;

using NUnit.Framework;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Mindscape.WpfElements.WpfPropertyGrid.UnitTests;

namespace Mindscape.WpfElements.PropertyEditing.UnitTests
{
  [TestFixture]
  public class PropertyNodeTests
  {
    private readonly PropertyInfo SampleProperty = typeof(string).GetProperty("Length");

    [Test]
    public void Properties()
    {
      PropertyNode property = new PropertyNode("Bob", SampleProperty, null);
      Assert.AreEqual("Bob", property.Source);
      Assert.AreEqual(SampleProperty, property.PropertyInfo);
      Assert.AreEqual(0, property.Children.Count);
      Assert.AreEqual("Length", property.Name);
      Assert.AreEqual("Length", property.HumanName);
      Assert.IsFalse(property.HasOwnInPlaceEditor);
      Assert.IsNull(property.InPlaceEditor);
      Assert.IsNull(property.IndexedPropertyArguments);
      Assert.AreEqual(typeof(int), property.PropertyType);
      Assert.AreEqual(3, property.Value);
      Assert.AreEqual(typeof(string), property.DeclaringType);
      Assert.IsFalse(property.CanWrite);
    }

    [Test]
    [ExpectedException(typeof(ArgumentNullException))]
    public void Constructor_SourceMustBeNonNull()
    {
      new PropertyNode(null, SampleProperty, null);
    }

    [Test]
    [ExpectedException(typeof(ArgumentNullException))]
    public void Constructor_PropertyInfoMustBeNonNull()
    {
      new PropertyNode("Bob", (PropertyInfo)null, null);
    }

    [Test]
    public void CustomCaption()
    {
      PropertyNode property = new PropertyNode("Bob", "StringLength", SampleProperty, null);
      Assert.AreEqual("Length", property.Name);
      Assert.AreEqual("StringLength", property.HumanName);
    }

    [Test]
    public void CaptionUsesDisplayNameAttribute()
    {
      PropertyNode property = new PropertyNode(Person.Alice, typeof(Person).GetProperty("NullableProperty"), null);
      Assert.AreEqual("Nullable Integer", property.HumanName);
      Assert.AreEqual(property.HumanName, property.Property.DisplayName);
    }

    [Test]
    public void CategoryUsesCategoryAttribute()
    {
      PropertyNode firstName = new PropertyNode(Person.Alice, typeof(Person).GetProperty("FirstName"), null);
      Assert.AreEqual("Identity", firstName.Property.Category);

      PropertyNode age = new PropertyNode(Person.Alice, typeof(Person).GetProperty("Age"), null);
      Assert.IsNull(age.Property.Category);
    }

    [Test]
    public void DescriptionUsesDescriptionAttribute()
    {
      PropertyNode firstName = new PropertyNode(Person.Alice, typeof(Person).GetProperty("FirstName"), null);
      Assert.AreEqual("The person's first name", firstName.Property.Description);

      PropertyNode age = new PropertyNode(Person.Alice, typeof(Person).GetProperty("Age"), null);
      Assert.IsNull(age.Property.Description);
    }

    [Test]
    public void SelfEditing()
    {
      NodeEditor editor = new StaticNodeEditor();
      PropertyNode property = new PropertyNode("Bob", "StringLength", SampleProperty, null, editor);
      Assert.IsTrue(property.HasOwnInPlaceEditor);
      Assert.AreEqual(editor, property.InPlaceEditor);
    }

    [Test]
    public void Children()
    {
      PropertyNode property = new PropertyNode(Person.Alice, typeof(Person).GetProperty("Address"), null);
      Assert.AreEqual(5, property.Children.Count);

      PropertyNode selfEditingProperty = new PropertyNode(Person.Alice, "test", typeof(Person).GetProperty("Address"), null, new StaticNodeEditor());
      Assert.AreEqual(0, selfEditingProperty.Children.Count);
    }

    [Test]
    public void NonBrowsableChildrenOmitted()
    {
      Person person = new Person();
      person.Puppy = new Puppy(null);
      PropertyNode puppyProperty = new PropertyNode(person, typeof(Person).GetProperty("Puppy"), null);
      ObservableCollection<Node> children = puppyProperty.Children;

      Assert.AreEqual(typeof(Puppy).GetProperties().Length - 2 /* local Owner and inherited Id */, children.Count);
      foreach (Node child in children)
      {
        Assert.IsFalse(child.Name == "Owner");
      }
    }

    [Test]
    public void CollectionsDoNotEmitPropertiesAsChildren()
    {
      Node friends = new PropertyNode(Person.Alice, typeof(Person).GetProperty("Friends"), null);
      ObservableCollection<Node> children = friends.Children;

      Assert.AreEqual(children.Count, Person.Alice.Friends.Count);
      foreach (Node child in children)
      {
        Assert.IsInstanceOf<CollectionElement>(child);
      }
    }

    [Test]
    [STAThread]
    public void Descriptor()
    {
      DynamicPerson person = new DynamicPerson(
        "Alice", "Liddell", "English", 
        Person.Alice.Address, new ObservableCollection<int>(), true);

      PropertyGrid grid = new PropertyGrid();
      grid.SelectedObject = person;
      Assert.AreEqual(9, grid.Nodes.Count);
      Assert.AreEqual("FirstName", grid.Nodes[0].Name);
      Assert.AreEqual("Alice", grid.Nodes[0].Value);

      PropertyInfo info = grid.Nodes[0].PropertyInfo;
      Assert.IsNotNull(info);

      Assert.IsTrue(info.CanWrite);

      info.SetValue(person, "Bob", null);
      Assert.AreEqual("Bob", info.GetValue(person, null));

      Assert.AreEqual(typeof(string), info.PropertyType);
      Assert.AreEqual(typeof(DynamicPerson), info.DeclaringType);
      Assert.AreEqual(typeof(DynamicPerson), info.ReflectedType);

      Assert.AreEqual(0, info.GetCustomAttributes(true).Length);

      PropertyInfo addressInfo = grid.Nodes[3].PropertyInfo;

      Assert.AreEqual(2, addressInfo.GetCustomAttributes(true).Length);
      Assert.AreEqual(1, addressInfo.GetCustomAttributes(typeof(System.ComponentModel.DescriptionAttribute), true).Length);
      Assert.AreEqual(1, addressInfo.GetCustomAttributes(typeof(System.ComponentModel.CategoryAttribute), true).Length);
      Assert.AreEqual(0, addressInfo.GetCustomAttributes(typeof(BrowsableAttribute), true).Length);
      Assert.IsTrue(addressInfo.IsDefined(typeof(System.ComponentModel.CategoryAttribute), true));
      Assert.IsFalse(addressInfo.IsDefined(typeof(BrowsableAttribute), true));

      PropertyInfo favouritePalInfo = grid.Nodes[8].PropertyInfo;
      Assert.AreEqual("FavouritePal", favouritePalInfo.Name);

      bool gotTargetParameterCountException = false;

      try
      {
        info.SetValue(person, "Carol", new object[] { 2 });
      }
      catch (TargetParameterCountException)
      {
        gotTargetParameterCountException = true;
      }

      Assert.IsTrue(gotTargetParameterCountException);

      gotTargetParameterCountException = false;

      try
      {
        info.GetValue(person, new object[] { 2 });
      }
      catch (TargetParameterCountException)
      {
        gotTargetParameterCountException = true;
      }

      Assert.IsTrue(gotTargetParameterCountException);
    }

    private static Node FindNode(PropertyGrid grid, string propertyName)
    {
      foreach (Node node in grid.Nodes)
      {
        if (node.Name == propertyName)
        {
          return node;
        }
      }

      return null;
    }

    [Test]
    [STAThread]
    public void DescriptorGetsDisplayOverrides()
    {
      DynamicPerson person = new DynamicPerson(
        "Alice", "Liddell", "English",
        Person.Alice.Address, new ObservableCollection<int>(), true);

      PropertyGrid grid = new PropertyGrid();
      grid.SelectedObject = person;

      Node favoriteColorNode = FindNode(grid, "FavoriteColor");
      Assert.IsNotNull(favoriteColorNode);

      IPropertyInfo property = favoriteColorNode.Property;
      Assert.AreEqual(0, property.GetCustomAttributes(true).Length);
      Assert.AreEqual("CategoryFromPropertyDescriptor", property.Category);
      Assert.AreEqual("DescriptionFromPropertyDescriptor", property.Description);
      Assert.AreEqual("DisplayNameFromPropertyDescriptor", property.DisplayName);
    }

    [Test]
    [STAThread]
    public void DescriptorGetsIsBrowsableOverride()
    {
      DynamicPerson person = new DynamicPerson(
        "Alice", "Liddell", "English",
        Person.Alice.Address, new ObservableCollection<int>(), true);

      PropertyGrid grid = new PropertyGrid();
      grid.SelectedObject = person;

      Node secretNode = FindNode(grid, "AlsoSecretMessage");
      Assert.IsNull(secretNode);
    }
  }
}
