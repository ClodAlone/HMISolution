using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using System.Windows.Media;
using System.Collections;

namespace Mindscape.WpfElements.WpfPropertyGrid.UnitTests
{
  public sealed class DynamicPerson : ICustomTypeDescriptor, INotifyPropertyChanged
  {
    public static readonly DynamicPerson Alice = new DynamicPerson("Alice", "Liddell", "English", 
      Address.AndrewsAddress, new int[] { 7, 13, 42 }, true);

    private Dictionary<string, object> _props = new Dictionary<string, object>();

    public DynamicPerson(string firstName, string surname, string nationality, 
      Address address, ObservableCollection<int> luckyNumbers, bool recurse)
    {
      _props["FirstName"] = firstName;
      _props["Surname"] = surname;
      _props["Nationality"] = nationality;
      _props["Address"] = address;
      _props["LuckyNumbers"] = luckyNumbers;
      _props["FavoriteColor"] = Colors.Orange;
      _props["IsAlive"] = true;
      _props["Status"] = CitizenshipStatus.Alien;
      _props["SecretMessage"] = "shouldn't appear";
      _props["AlsoSecretMessage"] = "also shouldn't appear";

      if (recurse)
      {
        _props["BestMate"] = new CollectionBasedDynamicPerson(
          "Calvin", "O'Hobbes", "American",
          Person.Alice.Address,
          new ObservableCollection<int>(new List<int>(new int[] { 5, 7, 9 })));
      }
    }

    public DynamicPerson(string firstName, string surname, string nationality,
      Address address, IEnumerable<int> luckyNumbers, bool recurse)
      : this(firstName, surname, nationality, address, 
        new ObservableCollection<int>(new List<int>(luckyNumbers)), recurse) { }

    AttributeCollection ICustomTypeDescriptor.GetAttributes()
    {
      return TypeDescriptor.GetAttributes(this, true);
    }

    string ICustomTypeDescriptor.GetClassName()
    {
      return TypeDescriptor.GetClassName(this, true);
    }

    string ICustomTypeDescriptor.GetComponentName()
    {
      return TypeDescriptor.GetComponentName(this, true);
    }

    TypeConverter ICustomTypeDescriptor.GetConverter()
    {
      return TypeDescriptor.GetConverter(this, true);
    }

    EventDescriptor ICustomTypeDescriptor.GetDefaultEvent()
    {
      return TypeDescriptor.GetDefaultEvent(this, true);
    }

    PropertyDescriptor ICustomTypeDescriptor.GetDefaultProperty()
    {
      return TypeDescriptor.GetDefaultProperty(this, true);
    }

    object ICustomTypeDescriptor.GetEditor(Type editorBaseType)
    {
      return TypeDescriptor.GetEditor(this, editorBaseType, true);
    }

    EventDescriptorCollection ICustomTypeDescriptor.GetEvents(Attribute[] attributes)
    {
      return TypeDescriptor.GetEvents(this, attributes, true);
    }

    EventDescriptorCollection ICustomTypeDescriptor.GetEvents()
    {
      return TypeDescriptor.GetEvents(this, true);
    }

    PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties()
    {
      return ((ICustomTypeDescriptor)this).GetProperties(new Attribute[0]);
    }

    PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties(Attribute[] attributes)
    {
      List<PropertyDescriptor> descriptors = new List<PropertyDescriptor>();

      foreach (string key in _props.Keys)
      {
        string propertyName = key;

        List<Attribute> attrs = new List<Attribute>();
        switch (key)
        {
          case "Address":
            attrs.Add(new CategoryAttribute("Location"));
            attrs.Add(new DescriptionAttribute("Where they live"));
            break;
          case "SecretMessage":
            attrs.Add(new BrowsableAttribute(false));
            break;
          case "BestMate":
            propertyName = "FavouritePal";
            break;
        }
        PropertyDescriptor descriptor = new TestPropertyDescriptor(propertyName, key, _props[key].GetType(), attrs.ToArray());
        descriptors.Add(descriptor);
      }

      return new PropertyDescriptorCollection(descriptors.ToArray(), true);
    }

    object ICustomTypeDescriptor.GetPropertyOwner(PropertyDescriptor pd)
    {
      return this;
    }

    private class TestPropertyDescriptor : PropertyDescriptor
    {
      private Type _type;
      private string _key;

      public TestPropertyDescriptor(string name, string key, Type type, Attribute[] attrs)
        : base(name, attrs)
      {
        _key = key;
        _type = type;
      }

      public override bool CanResetValue(object component)
      {
        return false;
      }

      public override Type ComponentType
      {
        get { return typeof(DynamicPerson); }
      }

      public override object GetValue(object component)
      {
        return ((DynamicPerson)component)._props[_key];
      }

      public override bool IsReadOnly
      {
        get { return false; }
      }

      public override Type PropertyType
      {
        get { return _type; }
      }

      public override void ResetValue(object component)
      {
        throw new NotImplementedException();
      }

      public override void SetValue(object component, object value)
      {
        DynamicPerson person = (DynamicPerson)component;
        if (!Equals(person._props[_key], value))
        {
          person._props[_key] = value;
          person.OnPropertyChanged(Name);
        }
      }

      public override bool ShouldSerializeValue(object component)
      {
        throw new NotImplementedException();
      }

      public override string Category
      {
        get
        {
          if (_key == "FavoriteColor")
          {
            return "CategoryFromPropertyDescriptor";
          }
          return base.Category;
        }
      }

      public override string Description
      {
        get
        {
          if (_key == "FavoriteColor")
          {
            return "DescriptionFromPropertyDescriptor";
          }
          return base.Description;
        }
      }

      public override string DisplayName
      {
        get
        {
          if (_key == "FavoriteColor")
          {
            return "DisplayNameFromPropertyDescriptor";
          }
          return base.DisplayName;
        }
      }

      public override bool IsBrowsable
      {
        get
        {
          if (_key == "AlsoSecretMessage")
          {
            return false;
          }
          return base.IsBrowsable;
        }
      }
    }

    private void OnPropertyChanged(string propertyName)
    {
      if (PropertyChanged != null)
      {
        PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
      }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    public override string ToString()
    {
      return "DynamicPerson";
    }
  }

  public sealed class CollectionBasedDynamicPerson : DictionaryBase, ICustomTypeDescriptor, INotifyPropertyChanged
  {
    public CollectionBasedDynamicPerson(string firstName, string surname, string nationality, 
      Address address, ObservableCollection<int> luckyNumbers)
    {
      Dictionary["FirstName"] = firstName;
      Dictionary["Surname"] = surname;
      Dictionary["Nationality"] = nationality;
      Dictionary["Address"] = address;
      Dictionary["LuckyNumbers"] = luckyNumbers;
      Dictionary["FavoriteColor"] = Colors.Orange;
      Dictionary["IsAlive"] = true;
      Dictionary["Status"] = CitizenshipStatus.Alien;
      Dictionary["SecretMessage"] = "shouldn't appear";
      Dictionary["BestMate"] = new DynamicPerson(
        "Hobbes", "O'Calvin", "Polish", 
        Person.Alice.Address, new int[] { 456 }, false);
    }

    public CollectionBasedDynamicPerson(string firstName, string surname, string nationality,
      Address address, IEnumerable<int> luckyNumbers)
      : this(firstName, surname, nationality, address, 
        new ObservableCollection<int>(new List<int>(luckyNumbers))) { }

    AttributeCollection ICustomTypeDescriptor.GetAttributes()
    {
      return TypeDescriptor.GetAttributes(this, true);
    }

    string ICustomTypeDescriptor.GetClassName()
    {
      return TypeDescriptor.GetClassName(this, true);
    }

    string ICustomTypeDescriptor.GetComponentName()
    {
      return TypeDescriptor.GetComponentName(this, true);
    }

    TypeConverter ICustomTypeDescriptor.GetConverter()
    {
      return TypeDescriptor.GetConverter(this, true);
    }

    EventDescriptor ICustomTypeDescriptor.GetDefaultEvent()
    {
      return TypeDescriptor.GetDefaultEvent(this, true);
    }

    PropertyDescriptor ICustomTypeDescriptor.GetDefaultProperty()
    {
      return TypeDescriptor.GetDefaultProperty(this, true);
    }

    object ICustomTypeDescriptor.GetEditor(Type editorBaseType)
    {
      return TypeDescriptor.GetEditor(this, editorBaseType, true);
    }

    EventDescriptorCollection ICustomTypeDescriptor.GetEvents(Attribute[] attributes)
    {
      return TypeDescriptor.GetEvents(this, attributes, true);
    }

    EventDescriptorCollection ICustomTypeDescriptor.GetEvents()
    {
      return TypeDescriptor.GetEvents(this, true);
    }

    PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties()
    {
      return ((ICustomTypeDescriptor)this).GetProperties(new Attribute[0]);
    }

    PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties(Attribute[] attributes)
    {
      List<PropertyDescriptor> descriptors = new List<PropertyDescriptor>();

      foreach (object key in Dictionary.Keys)
      {
        string propertyName = key.ToString();

        List<Attribute> attrs = new List<Attribute>();
        switch (key.ToString())
        {
          case "Address":
            attrs.Add(new CategoryAttribute("Location"));
            attrs.Add(new DescriptionAttribute("Where they live"));
            break;
          case "SecretMessage":
            attrs.Add(new BrowsableAttribute(false));
            break;
          case "BestMate":
            propertyName = "FavouritePal";
            break;
        }
        PropertyDescriptor descriptor = new TestPropertyDescriptor(propertyName, key, Dictionary[key].GetType(), attrs.ToArray());
        descriptors.Add(descriptor);
      }

      return new PropertyDescriptorCollection(descriptors.ToArray(), true);
    }

    object ICustomTypeDescriptor.GetPropertyOwner(PropertyDescriptor pd)
    {
      return this;
    }

    private class TestPropertyDescriptor : PropertyDescriptor
    {
      private Type _type;
      private object _key;

      public TestPropertyDescriptor(string name, object key, Type type, Attribute[] attrs)
        : base(name, attrs)
      {
        _type = type;
        _key = key;
      }

      public override bool CanResetValue(object component)
      {
        return false;
      }

      public override Type ComponentType
      {
        get { return typeof(DynamicPerson); }
      }

      public override object GetValue(object component)
      {
        return ((CollectionBasedDynamicPerson)component).Dictionary[_key];
      }

      public override bool IsReadOnly
      {
        get { return false; }
      }

      public override Type PropertyType
      {
        get { return _type; }
      }

      public override void ResetValue(object component)
      {
        throw new NotImplementedException();
      }

      public override void SetValue(object component, object value)
      {
        CollectionBasedDynamicPerson person = (CollectionBasedDynamicPerson)component;
        if (!Equals(person.Dictionary[_key], value))
        {
          person.Dictionary[_key] = value;
          person.OnPropertyChanged(Name);
        }
      }

      public override bool ShouldSerializeValue(object component)
      {
        throw new NotImplementedException();
      }
    }

    private void OnPropertyChanged(string propertyName)
    {
      if (PropertyChanged != null)
      {
        PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
      }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    public override string ToString()
    {
      return "CollectionBasedDynamicPerson";
    }
  }
}
