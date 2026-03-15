using System;
using System.Collections.Generic;
using NUnit.Framework;
using System.Reflection;
using System.ComponentModel;
using System.Collections.ObjectModel;
using Mindscape.WpfElements.WpfPropertyGrid.UnitTests;

namespace Mindscape.WpfElements.PropertyEditing.UnitTests
{
  [TestFixture]
  public class ObjectWrapperTests
  {
    private static Person _subject;  // some tests modify their subject, so we don't want to use a static object for these
    private static int _propertyChangeCount;
    private static string _lastPropertyChanged;

    [SetUp]
    public void SetUp()
    {
      _subject = new Person();
      _subject.Surname = "Newman";

      _propertyChangeCount = 0;
      _lastPropertyChanged = null;
    }

    private const BindingFlags StdFlags = BindingFlags.Public | BindingFlags.Instance;

    [Test]
    public void ExplicitInterfaceImplementation()
    {
      object obj = Person.Alice.Friends;

      Type declaredType = typeof(Person).GetProperty("Friends").PropertyType;
      PropertyInfo explicitlyImplementedPropertyInfo = declaredType.GetProperty("IsReadOnly");
      Node explicitlyImplementedProperty = new PropertyNode(obj, explicitlyImplementedPropertyInfo, null);

      ObjectWrapper wrapper = ObjectWrapperFactory.CreateWrapper(explicitlyImplementedProperty, true);

      Assert.IsNotNull(wrapper);
      Assert.AreEqual(obj, wrapper.UnderlyingObject);
      Assert.AreEqual("IsReadOnly", wrapper.PropertyName);

      object value = wrapper.GetType().InvokeMember("Value", StdFlags | BindingFlags.GetProperty, 
        null, wrapper, null);

      Assert.IsInstanceOf<bool>(value);
      Assert.AreEqual(Person.Alice.Friends.IsReadOnly, (bool)value);
    }

    private ObjectWrapper CreateSubjectSurnamePropertyWrapper()
    {
      Person obj = _subject;
      PropertyNode property = new PropertyNode(obj, typeof(Person).GetProperty("Surname"), null);
      return ObjectWrapperFactory.CreateWrapper(property, true);
    }

    private ObjectWrapper CreatePuppyAlivePropertyWrapper()
    {
      Puppy puppy = new Puppy(null);
      PropertyNode property = new PropertyNode(puppy, typeof(Puppy).GetProperty("Alive"), null);
      return ObjectWrapperFactory.CreateWrapper(property, true);
    }

    [Test]
    public void Properties()
    {
      ObjectWrapper wrapper = CreateSubjectSurnamePropertyWrapper();

      Assert.IsTrue(wrapper.Editable);
      Assert.AreEqual("Surname", wrapper.PropertyName);
      Assert.AreEqual(_subject, wrapper.UnderlyingObject);

      Assert.IsInstanceOf<ObjectWrapper<string>>(wrapper);
      Assert.AreEqual(typeof(string), ((ObjectWrapper<string>)wrapper).DataType);
    }

    [Test]
    public void GetSetValue()
    {
      ObjectWrapper wrapper = CreateSubjectSurnamePropertyWrapper();

      object value = wrapper.GetType().InvokeMember("Value", StdFlags | BindingFlags.GetProperty, 
        null, wrapper, null);
      Assert.AreEqual("Newman", value);

      Assert.AreEqual(0, _propertyChangeCount);

      wrapper.GetType().InvokeMember("Value", StdFlags | BindingFlags.SetProperty, 
        null, wrapper, new object[] { "Flowers" });
      Assert.AreEqual(_subject.Surname, "Flowers");
    }

    [Test]
    public void WrapperReflectsChangesInSourceAndPropagatesNotifications()
    {
      ObjectWrapper wrapper = CreateSubjectSurnamePropertyWrapper();
      wrapper.PropertyChanged += LogPropertyChange; // we want to ensure that setting values on the wrapper fires notifications on the real object

      Assert.AreEqual(0, _propertyChangeCount);

      _subject.Surname = "Flowers";

      Assert.AreEqual(1, _propertyChangeCount);
      Assert.AreEqual("Value", _lastPropertyChanged);

      object value = wrapper.GetType().InvokeMember("Value", StdFlags | BindingFlags.GetProperty,
          null, wrapper, null);
      Assert.AreEqual("Flowers", value);
    }

    private void LogPropertyChange(object sender, PropertyChangedEventArgs e)
    {
      ++_propertyChangeCount;
      _lastPropertyChanged = e.PropertyName;
    }

    [Test]
    public void ActualWrapperIsStronglyTyped()
    {
      ObjectWrapper wrapper = CreateSubjectSurnamePropertyWrapper();
      PropertyInfo valueProperty = wrapper.GetType().GetProperty("Value");
      Assert.AreEqual(typeof(string), valueProperty.PropertyType);
    }

    [Test]
    public void IndexedProperties()
    {
      List<int> list = new List<int>(new int[] { 1, 2, 3 });
      CollectionElement property = new CollectionElement(list, new CollectionTypeOperations(typeof(List<int>), "Item", "RemoveAt"), 1, false, null);
      ObjectWrapper wrapper = ObjectWrapperFactory.CreateWrapper(property, true);

      object value = wrapper.GetType().InvokeMember("Value", StdFlags | BindingFlags.GetProperty,
        null, wrapper, null);

      Assert.IsInstanceOf<int>(value);
      Assert.AreEqual(2, (int)value);

      wrapper.GetType().InvokeMember("Value", StdFlags | BindingFlags.SetProperty,
        null, wrapper, new object[] { 999 });

      Assert.AreEqual(1, list[0]);
      Assert.AreEqual(999, list[1]);
      Assert.AreEqual(3, list[2]);
    }

    [Test]
    public void ReportsExceptionsViaIDataErrorInfo()
    {
      ObjectWrapper wrapper = CreateSubjectSurnamePropertyWrapper();
      IDataErrorInfo errorInfo = wrapper;

      Assert.IsNull(errorInfo.Error);
      Assert.IsNull(errorInfo["Value"]);

      wrapper.GetType().InvokeMember("Value", StdFlags | BindingFlags.SetProperty,
        null, wrapper, new object[] { "This surname is so long it causes an exception" });

      Assert.IsNotNull(errorInfo.Error);
      Assert.IsNotNull(errorInfo["Value"]);
      Assert.AreEqual("surname too long", errorInfo.Error);
      Assert.AreEqual("surname too long", errorInfo["Value"]);

      wrapper.GetType().InvokeMember("Value", StdFlags | BindingFlags.SetProperty,
        null, wrapper, new object[] { "Safe name" });

      Assert.IsNull(errorInfo.Error);
      Assert.IsNull(errorInfo["Value"]);
    }

    [Test]
    public void PropagatesIDataErrorInfoErrors()
    {
      ObjectWrapper wrapper = CreatePuppyAlivePropertyWrapper();
      IDataErrorInfo errorInfo = wrapper;

      Assert.AreEqual("puppy is dead", errorInfo.Error);
      Assert.AreEqual("puppy is dead", errorInfo["Value"]);

      wrapper.GetType().InvokeMember("Value", StdFlags | BindingFlags.SetProperty,
        null, wrapper, new object[] { true });

      Assert.IsNull(errorInfo.Error);
      Assert.IsNull(errorInfo["Value"]);
    }

    [Test]
    public void ObservableCollectionChangesArePropagated()
    {
      ObservableCollection<string> strs = new ObservableCollection<string>();
      strs.Add("1");
      strs.Add("2");

      List<CollectionElement> ces = new List<CollectionElement>(CollectionElement.GetCollectionElements(strs, null));

      int changeCount = 0;
      ObjectWrapper wrapper = ObjectWrapperFactory.CreateWrapper(ces[0], true);
      wrapper.PropertyChanged += delegate(object sender, PropertyChangedEventArgs e)
        {
          if (e.PropertyName == "Value")
          {
            ++changeCount;
          }
        };

      wrapper.GetType().InvokeMember("Value", StdFlags | BindingFlags.SetProperty,
        null, wrapper, new object[] { "Hello" });

      Assert.AreEqual(1, changeCount);
    }

    [Test]
    public void EditContextIsPropagated()
    {
      object editContext = new object();
      PropertyNode property = new PropertyNode(Person.Alice, typeof(Person).GetProperty("Surname"), null);
      ObjectWrapper wrapper = ObjectWrapperFactory.CreateWrapper(property, true, editContext);
      Assert.AreEqual(editContext, wrapper.EditContext);
    }
  }
}
