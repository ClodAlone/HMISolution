using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using Mindscape.WpfElements.WpfPropertyGrid.UnitTests;

namespace Mindscape.WpfElements.PropertyEditing.UnitTests
{
  [TestFixture]
  public class NodeToCategoryConverterTests
  {
    [Test]
    public void Convert()
    {
      PropertyNode identityProperty = new PropertyNode(Person.Alice, typeof(Person).GetProperty("FirstName"), null);
      PropertyNode uncategorisedProperty = new PropertyNode(Person.Alice, typeof(Person).GetProperty("Age"), null);

      NodeToCategoryConverter converter = new NodeToCategoryConverter();

      Assert.AreEqual("Identity", converter.Convert(identityProperty, typeof(string), null, null) as string);
      Assert.AreEqual("Miscellaneous", converter.Convert(uncategorisedProperty, typeof(string), null, null) as string);

      converter.DefaultCategory = "Fie!";
      Assert.AreEqual("Fie!", converter.DefaultCategory);
      Assert.AreEqual("Fie!", converter.Convert(uncategorisedProperty, typeof(string), null, null) as string);
    }

    [Test]
    [ExpectedException(typeof(NotImplementedException))]
    public void ConvertBack()
    {
      (new NodeToCategoryConverter()).ConvertBack("Identity", typeof(Node), null, null);
    }
  }
}
