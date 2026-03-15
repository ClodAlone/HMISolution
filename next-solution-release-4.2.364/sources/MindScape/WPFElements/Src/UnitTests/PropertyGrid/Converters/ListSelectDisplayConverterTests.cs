using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using Mindscape.WpfElements.WpfPropertyGrid.UnitTests;

namespace Mindscape.WpfElements.PropertyEditing.UnitTests.Converters
{
  [TestFixture]
  public class ListSelectDisplayConverterTests
  {
    [Test]
    public void BooleansHandledCorrectly()
    {
      IPropertyInfo propertyInfo = new PropertyNode(Person.Alice, typeof(Person).GetProperty("Tall"), null).Property;

      ListSelectDisplayConverter converter = new ListSelectDisplayConverter();
      object[] args = new object[] { true, propertyInfo };
      object result = converter.Convert(args, typeof(string), null, null);

      Assert.AreEqual("True", result);
    }

    [Test]
    public void EnumsHandledCorrectly()
    {
      IPropertyInfo propertyInfo = new PropertyNode(Person.Alice, typeof(Person).GetProperty("Status"), null).Property;

      ListSelectDisplayConverter converter = new ListSelectDisplayConverter();
      object[] args = new object[] { CitizenshipStatus.Alien, propertyInfo };
      object result = converter.Convert(args, typeof(string), null, null);

      Assert.AreEqual("Alien", result);
    }

    [Test]
    public void TypeConverterTypesHandledCorrectly()
    {
      IPropertyInfo propertyInfo = new PropertyNode(Person.Alice.Department, typeof(Department).GetProperty("Location"), null).Property;

      ListSelectDisplayConverter converter = new ListSelectDisplayConverter();
      object[] args = new object[] { Location.Wellington, propertyInfo };
      object result = converter.Convert(args, typeof(string), null, null);

      Assert.AreEqual("Wellington", result);
    }

    [Test]
    public void TypeConverterPropertiesOfNonTypeConverterTypesHandledCorrectly()
    {
      IPropertyInfo propertyInfo = new PropertyNode(Person.Alice.Department, typeof(Department).GetProperty("Mascot"), null).Property;

      ListSelectDisplayConverter converter = new ListSelectDisplayConverter();
      object[] args = new object[] { DepartmentalMascotConverter.Butch, propertyInfo };
      object result = converter.Convert(args, typeof(string), null, null);

      Assert.AreEqual("Butch", result);
    }
  }
}
