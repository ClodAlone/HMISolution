using System;
using System.Reflection;

using NUnit.Framework;
using Mindscape.WpfElements.PropertyEditing;

namespace Mindscape.WpfElements.WpfPropertyGrid.UnitTests.Model
{
  [TestFixture]
  public class PropertyGridRowTests
  {
    private readonly PropertyInfo SampleProperty = typeof(string).GetProperty("Length");

    [Test]
    public void Properties()
    {
      Node property = new PropertyNode("Bob", SampleProperty, null);
      PropertyGridRow row = new PropertyGridRow(property);

      Assert.AreEqual(property, row.Node);
      Assert.IsTrue(row.IsLeaf);

      property.Children.Add(new PropertyNode("Fred", SampleProperty, null));

      Assert.IsFalse(row.IsLeaf);
    }

    [Test]
    [ExpectedException(typeof(ArgumentNullException))]
    public void Constructor_PropertyMustBeNonNull()
    {
      new PropertyGridRow(null);
    }

    [Test]
    public void Children()
    {
      Node property = new PropertyNode(Person.Alice, typeof(Person).GetProperty("Address"), null);
      PropertyGridRow row = new PropertyGridRow(property);
      PropertyGridBindingView children = row.Children;
      Assert.IsNotNull(children);
      Assert.IsTrue(children.Count > 0);
    }
  }
}
