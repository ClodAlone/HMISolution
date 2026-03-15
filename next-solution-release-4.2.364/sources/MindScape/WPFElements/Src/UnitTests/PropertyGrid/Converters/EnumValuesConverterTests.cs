using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using System.Collections;
using System.Linq;
using Mindscape.WpfElements.PropertyEditing;

namespace Mindscape.WpfElements.WpfPropertyGrid.UnitTests
{
  [TestFixture]
  public class EnumValuesConverterTests
  {
    [Test]
    public void ConvertEnumToPossibleValuesList()
    {
      EnumValuesConverter converter = new EnumValuesConverter();
      object converted = converter.Convert(CitizenshipStatus.Alien, typeof(IEnumerable), null, null);
      Assert.IsInstanceOf<IEnumerable>(converted);

      int count = 0;
      foreach (object obj in (IEnumerable)converted)
      {
        Assert.IsInstanceOf<CitizenshipStatus>(obj);
        ++count;
      }

      Assert.AreEqual(4, count);
    }

    [Test]
    public void ConvertEnumTypeToPossibleValuesList()
    {
      EnumValuesConverter converter = new EnumValuesConverter();
      object converted = converter.Convert(typeof(CitizenshipStatus), typeof(IEnumerable), null, null);
      Assert.IsInstanceOf<IEnumerable>(converted);

      int count = 0;
      foreach (object obj in (IEnumerable)converted)
      {
        Assert.IsInstanceOf<CitizenshipStatus>(obj);
        ++count;
      }

      Assert.AreEqual(4, count);
    }

    [Test]
    public void ConvertEnumPropertyToPossibleValuesList()
    {
      IPropertyInfo property = new PropertyNode(Person.Alice, typeof(Person).GetProperty("Status"), null).Property;

      EnumValuesConverter converter = new EnumValuesConverter();
      object converted = converter.Convert(property, typeof(IEnumerable), null, null);
      Assert.IsInstanceOf<IEnumerable>(converted);

      int count = 0;
      foreach (object obj in (IEnumerable)converted)
      {
        Assert.IsInstanceOf<CitizenshipStatus>(obj);
        ++count;
      }

      Assert.AreEqual(4, count);
    }

    [Test]
    public void ConvertEnumPropertyToPossibleValuesList_Node()
    {
      Node property = new PropertyNode(Person.Alice, typeof(Person).GetProperty("Status"), null);

      EnumValuesConverter converter = new EnumValuesConverter();
      object converted = converter.Convert(property, typeof(IEnumerable), null, null);
      Assert.IsInstanceOf<IEnumerable>(converted);

      int count = 0;
      foreach (object obj in (IEnumerable)converted)
      {
        Assert.IsInstanceOf<CitizenshipStatus>(obj);
        ++count;
      }

      Assert.AreEqual(4, count);
    }

    [Test]
    public void ConvertEnumPropertyToPossibleValuesList_Dictionary()
    {
      ObservableDictionary<string, object> dict = new ObservableDictionary<string, object>();
      dict["Enum"] = CitizenshipStatus.Alien;

      Node property = CollectionElement.FromDictionary(dict, null).First();

      EnumValuesConverter converter = new EnumValuesConverter();
      object converted = converter.Convert(property, typeof(IEnumerable), null, null);
      Assert.IsInstanceOf<IEnumerable>(converted);

      int count = 0;
      foreach (object obj in (IEnumerable)converted)
      {
        Assert.IsInstanceOf<CitizenshipStatus>(obj);
        ++count;
      }

      Assert.AreEqual(4, count);
    }

    [Test]
    public void ConvertBooleanToPossibleValuesList()
    {
      EnumValuesConverter converter = new EnumValuesConverter();
      object converted = converter.Convert(true, typeof(IEnumerable), null, null);
      Assert.IsInstanceOf<IEnumerable>(converted);

      int count = 0;
      foreach (object obj in (IEnumerable)converted)
      {
        Assert.IsInstanceOf<bool>(obj);
        ++count;
      }

      Assert.AreEqual(2, count);
    }

    [Test]
    public void ConvertBooleanTypeToPossibleValuesList()
    {
      EnumValuesConverter converter = new EnumValuesConverter();
      object converted = converter.Convert(typeof(bool), typeof(IEnumerable), null, null);
      Assert.IsInstanceOf<IEnumerable>(converted);

      int count = 0;
      foreach (object obj in (IEnumerable)converted)
      {
        Assert.IsInstanceOf<bool>(obj);
        ++count;
      }

      Assert.AreEqual(2, count);
    }

    [Test]
    public void ConvertBooleanPropertyToPossibleValuesList()
    {
      IPropertyInfo property = new PropertyNode(Person.Alice, typeof(Person).GetProperty("Tall"), null).Property;

      EnumValuesConverter converter = new EnumValuesConverter();
      object converted = converter.Convert(property, typeof(IEnumerable), null, null);
      Assert.IsInstanceOf<IEnumerable>(converted);

      int count = 0;
      foreach (object obj in (IEnumerable)converted)
      {
        Assert.IsInstanceOf<bool>(obj);
        ++count;
      }

      Assert.AreEqual(2, count);
    }

    [Test]
    public void TypeConverterStandardValuesDetected()
    {
      EnumValuesConverter converter = new EnumValuesConverter();
      object converted = converter.Convert(Location.Auckland, typeof(IEnumerable), null, null);
      Assert.IsInstanceOf<IEnumerable>(converted);

      int count = 0;
      foreach (object obj in (IEnumerable)converted)
      {
        Assert.IsInstanceOf<Location>(obj);
        ++count;
      }

      Assert.AreEqual(3, count);
    }

    [Test]
    public void TypeConverterTypeStandardValuesDetected()
    {
      EnumValuesConverter converter = new EnumValuesConverter();
      object converted = converter.Convert(typeof(Location), typeof(IEnumerable), null, null);
      Assert.IsInstanceOf<IEnumerable>(converted);

      int count = 0;
      foreach (object obj in (IEnumerable)converted)
      {
        Assert.IsInstanceOf<Location>(obj);
        ++count;
      }

      Assert.AreEqual(3, count);
    }

    [Test]
    public void TypeConverterPropertyStandardValuesDetected()
    {
      IPropertyInfo property = new PropertyNode(new Department(), typeof(Department).GetProperty("Mascot"), null).Property;

      EnumValuesConverter converter = new EnumValuesConverter();
      object converted = converter.Convert(property, typeof(IEnumerable), null, null);
      Assert.IsInstanceOf<IEnumerable>(converted);

      int count = 0;
      foreach (object obj in (IEnumerable)converted)
      {
        Assert.IsInstanceOf<Puppy>(obj);
        ++count;
      }

      Assert.AreEqual(2, count);
    }

    [Test]
    public void TypeConverterPropertyStandardValuesDetected_Node()
    {
      Node property = new PropertyNode(new Department(), typeof(Department).GetProperty("Mascot"), null);

      EnumValuesConverter converter = new EnumValuesConverter();
      object converted = converter.Convert(property, typeof(IEnumerable), null, null);
      Assert.IsInstanceOf<IEnumerable>(converted);

      int count = 0;
      foreach (object obj in (IEnumerable)converted)
      {
        Assert.IsInstanceOf<Puppy>(obj);
        ++count;
      }

      Assert.AreEqual(2, count);
    }

    [Test]
    [ExpectedException(typeof(NotImplementedException))]
    public void ConvertBack()
    {
      // Mainly here so that if anybody adds a ConvertBack they'll be reminded to add tests.
      (new EnumValuesConverter()).ConvertBack(null, null, null, null);
    }
  }
}
