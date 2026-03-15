using NUnit.Framework;
using System.Collections.Specialized;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Reflection;
using Mindscape.WpfElements.WpfPropertyGrid.UnitTests;

namespace Mindscape.WpfElements.PropertyEditing.UnitTests
{
  [TestFixture]
  public class NodeTests
  {
    [Test]
    public void HandlesNotifyingCollections()
    {
      int changeCount = 0;

      Person p = new Person();
      p.Friends.Add("Fie");

      PropertyNode node = new PropertyNode(p, typeof(Person).GetProperty("Friends"), null);

      node.Children.CollectionChanged += delegate(object sender, NotifyCollectionChangedEventArgs e)
        {
          ++changeCount;
        };

      Assert.AreEqual(0, changeCount);
      Assert.AreEqual(1, node.Children.Count);

      p.Friends.Add("Tchah");

      Assert.AreEqual(3, changeCount);  // 1 remove, 2 adds
      Assert.AreEqual(2, node.Children.Count);

      ((ObservableCollection<string>)(p.Friends))[0] = "Faugh";
      Assert.AreEqual(3, changeCount);  // the replace operation doesn't affect the children collection
      Assert.AreEqual(2, node.Children.Count);
    }

    [Test]
    public void UnreadablePropertiesAreNotReturned()
    {
      Person p = new Person();
      p.Partner = new Person();
      Node node = new PropertyNode(p, typeof(Person).GetProperty("Partner"), null);
      ICollection<Node> personChildrenNodes = node.Children;

      PropertyInfo settableOnlyProperty = typeof(Person).GetProperty("EvilNonGettableProperty");
      foreach (Node child in personChildrenNodes)
      {
        Assert.IsTrue(child.PropertyInfo.CanRead);
        Assert.AreNotEqual(child.PropertyInfo.Name, settableOnlyProperty.Name);
      }
    }

    [Test]
    public void CanDisplayReadOnlyPropertiesSetToFalse_PropertyWithNoSetter()
    {
      Person p = new Person();
      p.Partner = new Person();
      Node node = new PropertyNode(p, typeof(Person).GetProperty("Partner"), null);
      node.CanDisplayReadOnlyProperties = false;
      ICollection<Node> personChildrenNodes = node.Children;

      foreach (Node child in personChildrenNodes)
      {
        Assert.IsTrue(child.PropertyInfo.CanWrite);
        Assert.AreNotEqual(child.PropertyInfo.Name, "Gender");
      }
    }

    [Test]
    public void CanDisplayReadOnlyPropertiesSetToFalse_PropertyWithPrivateSetter()
    {
      Wrapper wrapper = new Wrapper();
      ObjectWithPrivatePropertySetter o = new ObjectWithPrivatePropertySetter();
      wrapper.Value = o;
      Node node = new PropertyNode(wrapper, typeof(Wrapper).GetProperty("Value"), null);
      node.CanDisplayReadOnlyProperties = false;
      ICollection<Node> childrenNodes = node.Children;

      foreach (Node child in childrenNodes)
      {
        Assert.IsTrue(child.PropertyInfo.CanWrite);
        Assert.AreNotEqual(child.PropertyInfo.Name, "PrivateSetter");
        Assert.AreNotEqual(child.PropertyInfo.Name, "InternalSetter");
      }
    }

    [Test]
    public void ChildrenAreRefreshedWhenExpandablePropertyTypeChanges()
    {
      ObjectWithPolymorphicChild o = new ObjectWithPolymorphicChild();
      Node node = new PropertyNode(o, typeof(ObjectWithPolymorphicChild).GetProperty("Child"), null);

      Assert.AreEqual(2, node.Children.Count);
      Assert.AreEqual("Fie", node.Children[0].Name);
      Assert.AreEqual("Zounds", node.Children[1].Name);

      o.ChildKind = ChildKind.Techie;

      Assert.AreEqual(3, node.Children.Count);
      Assert.AreEqual("Foo", node.Children[0].Name);
      Assert.AreEqual("Bar", node.Children[1].Name);
      Assert.AreEqual("Baz", node.Children[2].Name);
    }

    public class ObjectWithPolymorphicChild : Entity
    {
      private ChildKind _childKind = ChildKind.Mediaevalist;

      public ChildKind ChildKind
      {
        get { return _childKind; }
        set
        {
          Set(ref _childKind, value, "ChildKind");
          if (_childKind == ChildKind.Mediaevalist)
          {
            Child = new MediaevalistChild();
          }
          else
          {
            Child = new TechieChild();
          }
        }
      }

      private object _child = new MediaevalistChild();

      public object Child
      {
        get { return _child; }
        set { Set(ref _child, value, "Child"); }
      }
    }

    public enum ChildKind
    {
      Mediaevalist,
      Techie
    }

    public class MediaevalistChild
    {
      public int Fie { get; set; }
      public int Zounds { get; set; }
    }

    public class TechieChild
    {
      public int Foo { get; set; }
      public int Bar { get; set; }
      public int Baz { get; set; }
    }

    public class Wrapper
    {
      public object Value { get; set; }
    }

    public class ObjectWithPrivatePropertySetter
    {
      public int PublicSetter { get; set; }
      public int PrivateSetter { get; private set; }
      public int InternalSetter { get; internal set; }
    }
  }
}
