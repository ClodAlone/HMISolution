using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using System.ComponentModel;
using System.Collections.ObjectModel;
using Mindscape.WpfElements.WpfPropertyGrid.UnitTests;
using Mindscape.WpfElements.WpfPropertyGrid;

namespace Mindscape.WpfElements.PropertyEditing.UnitTests
{
  [TestFixture]
  public class MultipleSelectedObjectsTests
  {
    private static Many GetMany(MultipleObjectWrapper wrapper, string propertyName)
    {
      PropertyDescriptor pd = TypeDescriptor.GetProperties(wrapper)[propertyName];
      return (Many)(pd.GetValue(wrapper));
    }

    private static void AssertPropertyValue(MultipleObjectWrapper wrapper, string propertyName, object expectedValue)
    {
      Many many = GetMany(wrapper, propertyName);
      Assert.IsTrue(many.IsConsistent);
      object value = many.RawValue;
      Assert.AreEqual(expectedValue, value);
    }

    private static object GetConsistentPropertyValue(MultipleObjectWrapper wrapper, string propertyName)
    {
      Many many = GetMany(wrapper, propertyName);
      Assert.IsTrue(many.IsConsistent);
      return many.RawValue;
    }

    private static void AssertPropertyInconsistent(MultipleObjectWrapper wrapper, string propertyName)
    {
      Many many = GetMany(wrapper, propertyName);
      Assert.IsFalse(many.IsConsistent);
    }

    private static void AssertPropertyConsistent(MultipleObjectWrapper wrapper, string propertyName)
    {
      Many many = GetMany(wrapper, propertyName);
      Assert.IsTrue(many.IsConsistent);
    }

    private static void AssertPropertyCount(MultipleObjectWrapper wrapper, int expectedCount)
    {
      PropertyDescriptorCollection pdc = TypeDescriptor.GetProperties(wrapper);
      Assert.AreEqual(expectedCount, pdc.Count);
    }

    private static void AssertPropertyType(MultipleObjectWrapper wrapper, string propertyName, Type expectedType)
    {
      PropertyDescriptor pd = TypeDescriptor.GetProperties(wrapper)[propertyName];
      Type type = pd.PropertyType;
      Assert.IsTrue(typeof(Many).IsAssignableFrom(type));
      Many many = (Many)(pd.GetValue(wrapper));
      Assert.AreEqual(expectedType, many.PropertyType);
    }

    private static void AssertNoProperty(MultipleObjectWrapper wrapper, string propertyName)
    {
      PropertyDescriptor pd = TypeDescriptor.GetProperties(wrapper)[propertyName];
      Assert.IsNull(pd);
    }

    private static void AssertPropertyReadOnly(MultipleObjectWrapper wrapper, string propertyName, bool expectedIsReadOnly)
    {
      PropertyDescriptor pd = TypeDescriptor.GetProperties(wrapper)[propertyName];
      Assert.AreEqual(expectedIsReadOnly, pd.IsReadOnly);
    }

    private static void AssertPropertyBrowsable(MultipleObjectWrapper wrapper, string propertyName, bool expectedBrowsable)
    {
      PropertyDescriptor pd = TypeDescriptor.GetProperties(wrapper)[propertyName];
      Assert.AreEqual(expectedBrowsable, pd.IsBrowsable);
    }

    [Test]
    public void HomogeneousObjectsRetainAllProperties()
    {
      MultipleObjectWrapper wrapper = new MultipleObjectWrapper(Animal.Fang, Animal.Kiki);

      AssertPropertyCount(wrapper, 4);

      AssertPropertyValue(wrapper, "Id", 0);
      AssertPropertyValue(wrapper, "Species", "Cat");
      AssertPropertyValue(wrapper, "LegCount", 4);
      AssertPropertyValue(wrapper, "BirthDate", new DateTime(2000, 1, 1));
    }

    [Test]
    public void DifferentValuesReportSpecialValue()
    {
      MultipleObjectWrapper wrapper = new MultipleObjectWrapper(Animal.Fang, Animal.Butch);

      AssertPropertyCount(wrapper, 4);

      AssertPropertyValue(wrapper, "Id", 0);
      AssertPropertyInconsistent(wrapper, "Species");
      AssertPropertyValue(wrapper, "LegCount", 4);
      AssertPropertyValue(wrapper, "BirthDate", new DateTime(2000, 1, 1));
    }

    [Test]
    public void OnlyPropertiesOccurringOnAllObjects()
    {
      MultipleObjectWrapper wrapper = new MultipleObjectWrapper(Animal.Fang, Animal.Kiki, Plant.ZogTheDevourer);

      AssertPropertyCount(wrapper, 2);
      AssertNoProperty(wrapper, "IsFlowering");
      AssertNoProperty(wrapper, "LegCount");

      AssertPropertyValue(wrapper, "Id", 0);
      AssertPropertyInconsistent(wrapper, "Species");
    }

    [Test]
    public void PropertiesWithNonMergeableAttributeAreExcluded_AcrossType()
    {
      MultipleObjectWrapper wrapper = new MultipleObjectWrapper(new TheAntiMergeOTron1(), new TheAntiMergeOTron2());

      AssertPropertyCount(wrapper, 2);
      AssertNoProperty(wrapper, "MergableOnlyIn1");

      wrapper = new MultipleObjectWrapper(new TheAntiMergeOTron2(), new TheAntiMergeOTron1());

      AssertPropertyCount(wrapper, 2);
      AssertNoProperty(wrapper, "MergableOnlyIn1");
    }

    [Test]
    public void PropertiesWithNonMergeableAttributeAreExcluded_SameType()
    {
      MultipleObjectWrapper wrapper = new MultipleObjectWrapper(new TheAntiMergeOTron2(), new TheAntiMergeOTron2());

      AssertPropertyCount(wrapper, 2);
      AssertNoProperty(wrapper, "MergableOnlyIn1");
    }

    [Test]
    public void PropertiesWithNonMergeableAttributeAreIncludedWhenOnlyOneItemInSelection()
    {
      MultipleObjectWrapper wrapper = new MultipleObjectWrapper(new TheAntiMergeOTron2());

      AssertPropertyCount(wrapper, 3);
    }

    [Test]
    public void DifferentTypesDoNotCountAsSameProperty()
    {
      MultipleObjectWrapper wrapper = new MultipleObjectWrapper(Animal.Fang, Alien.Bill);

      AssertPropertyCount(wrapper, 3);
      AssertNoProperty(wrapper, "LegCount");

      AssertPropertyValue(wrapper, "Id", 0);
      AssertPropertyInconsistent(wrapper, "Species");
      AssertPropertyValue(wrapper, "BirthDate", new DateTime(2000, 1, 1));
    }

    [Test]
    public void PropertyTypeIsManyOfUnderlyingProperty()
    {
      MultipleObjectWrapper wrapper = new MultipleObjectWrapper(Animal.Fang, Animal.Fido);

      AssertPropertyType(wrapper, "Id", typeof(int));
      AssertPropertyType(wrapper, "Species", typeof(string));
      AssertPropertyType(wrapper, "LegCount", typeof(int));
      AssertPropertyType(wrapper, "BirthDate", typeof(DateTime));
    }

    [Test]
    public void ReadOnlyPropertiesHaveReadOnlyWrappers()
    {
      MultipleObjectWrapper wrapper = new MultipleObjectWrapper(Animal.Fang, Animal.Fido);

      AssertPropertyReadOnly(wrapper, "Species", false);
      AssertPropertyReadOnly(wrapper, "BirthDate", true);
    }

    [Test]
    public void PropertyIsReadOnlyIfReadOnlyOnAnyObject()
    {
      MultipleObjectWrapper wrapper = new MultipleObjectWrapper(Animal.Fang, Alien.Bill);

      AssertPropertyReadOnly(wrapper, "Species", false);
      AssertPropertyReadOnly(wrapper, "BirthDate", true);
    }

    [Test]
    public void PropertyIsNonBrowsableIfNonBrowsableOnAnyObject()
    {
      Herbivore goat = new Herbivore { Species = "goat" };

      MultipleObjectWrapper wrapper = new MultipleObjectWrapper(goat, Animal.Fang);
      AssertPropertyBrowsable(wrapper, "Species", false);
 
      // Check no ordering dependency
      wrapper = new MultipleObjectWrapper(Animal.Fang, goat);
      AssertPropertyBrowsable(wrapper, "Species", false);

      wrapper = new MultipleObjectWrapper(Animal.Fang, Alien.Anastasia);
      AssertPropertyBrowsable(wrapper, "Species", true);
    }

    [Test]
    public void ReadOnlyPropertiesAreReadOnlyOnMany()
    {
      MultipleObjectWrapper wrapper = new MultipleObjectWrapper(Animal.Fang, Alien.Bill);

      Many speciesMany = GetMany(wrapper, "Species");
      Assert.IsFalse(speciesMany.IsReadOnly);
      Many birthDateMany = GetMany(wrapper, "BirthDate");
      Assert.IsTrue(birthDateMany.IsReadOnly);
    }

    [Test]
    public void ResettingValueTypesResultsInDefault()
    {
      Alien alice = new Alien { Species = "Dalek", LegCount = 0 };
      Alien bob = new Alien { Species = "Vogon", LegCount = 2 };
      MultipleObjectWrapper aliens = new MultipleObjectWrapper(alice, bob);

      ResetValue(aliens, "LegCount");

      AssertPropertyValue(aliens, "LegCount", 0);
    }

    [Test]
    public void DateTimeResetsToCustomDefault()
    {
      Alien alice = new Alien { Species = "Dalek", LegCount = 0 };
      Alien bob = new Alien { Species = "Vogon", LegCount = 2 };
      MultipleObjectWrapper aliens = new MultipleObjectWrapper(alice, bob);

      ResetValue(aliens, "BirthDate");

      DateTime resetTo = (DateTime)GetConsistentPropertyValue(aliens, "BirthDate");
      Assert.IsTrue(DateTime.Now.Subtract(resetTo).TotalMilliseconds < 1000);
    }

    [Test]
    public void ResettingStringsResultsInDefault()
    {
      Alien alice = new Alien { Species = "Dalek", LegCount = 0 };
      Alien bob = new Alien { Species = "Vogon", LegCount = 2 };
      MultipleObjectWrapper aliens = new MultipleObjectWrapper(alice, bob);

      ResetValue(aliens, "Species");

      AssertPropertyValue(aliens, "Species", null);
    }

    [Test]
    public void ResettingReferenceTypesResultsInNewInstance()
    {
      Plant triffid = new Plant { Species = "Triffid", FavouriteFood = new Animal { Species = "Human" } };
      Plant sundew = new Plant { Species = "Sundew", FavouriteFood = new Animal { Species = "Fly" } };
      MultipleObjectWrapper carnivores = new MultipleObjectWrapper(triffid, sundew);

      ResetValue(carnivores, "FavouriteFood");

      Animal animal = (Animal)GetConsistentPropertyValue(carnivores, "FavouriteFood");
      Assert.AreEqual(null, animal.Species);
      Assert.AreEqual(0, animal.LegCount);
    }

    [Test]
    public void ResetRespectsTypeConverters()
    {
      Herbivore cow = new Herbivore { FavouriteFood = new Plant { Species = "Grass" } };
      Herbivore goat = new Herbivore { FavouriteFood = new Plant { Species = "Gorse, Trousers, Anything" } };
      MultipleObjectWrapper herbivores = new MultipleObjectWrapper(cow, goat);

      ResetValue(herbivores, "FavouriteFood");

      Plant plant = (Plant)GetConsistentPropertyValue(herbivores, "FavouriteFood");
      Assert.AreEqual("Triffid", plant.Species);
      Assert.IsTrue(plant.IsFlowering);
    }

    [Test]
    public void ResettingRaisesPropertyChangedForIsConsistent()
    {
      Alien alice = new Alien { Species = "Dalek", LegCount = 0 };
      Alien bob = new Alien { Species = "Vogon", LegCount = 2 };
      MultipleObjectWrapper aliens = new MultipleObjectWrapper(alice, bob);

      PropertyDescriptor pd = TypeDescriptor.GetProperties(aliens)["Species"];
      Many many = (Many)(pd.GetValue(aliens));

      bool gotEvent = false;

      many.PropertyChanged += delegate(object sender, PropertyChangedEventArgs e)
      {
        if (e.PropertyName == "IsConsistent")
        {
          gotEvent = true;
        }
      };

      ResetValue(aliens, "LegCount");

      Assert.IsFalse(gotEvent);

      ResetValue(aliens, "Species");

      Assert.IsTrue(gotEvent);
    }

    [Test]
    public void RessetingRespectsDefaultValue()
    {
      DependencyObjectModel obj1 = new DependencyObjectModel() { Quantity = 18.0 };
      DependencyObjectModel obj2 = new DependencyObjectModel() { Quantity = 16.0 };
      MultipleObjectWrapper objects = new MultipleObjectWrapper(obj1, obj2);

      ResetValue(objects, "Quantity");

      Assert.AreEqual(13.0, obj1.Quantity);
      Assert.AreEqual(13.0, obj2.Quantity);
    }

    private static void ResetValue(MultipleObjectWrapper wrapper, string propertyName)
    {
      PropertyDescriptor pd = TypeDescriptor.GetProperties(wrapper)[propertyName];
      Many many = (Many)(pd.GetValue(wrapper));
      many.Reset();
    }

    private static void SetValue(MultipleObjectWrapper wrapper, string propertyName, object value)
    {
      PropertyDescriptor pd = TypeDescriptor.GetProperties(wrapper)[propertyName];
      pd.SetValue(wrapper, value);
    }

    private static void SetValueUsingMany(MultipleObjectWrapper wrapper, string propertyName, object value)
    {
      PropertyDescriptor pd = TypeDescriptor.GetProperties(wrapper)[propertyName];
      Many many = (Many)(pd.GetValue(wrapper));
      many.GetType().GetProperty("Value").SetValue(many, value, null);
    }

    // TODO: deduplicate these tests

    [Test]
    public void SettingValuePropagatesToAllWrappedObjects()
    {
      Alien alice = new Alien { Species = "Dalek", LegCount = 0 };
      Alien bob = new Alien { Species = "Vogon", LegCount = 2 };
      MultipleObjectWrapper aliens = new MultipleObjectWrapper(alice, bob);

      SetValue(aliens, "Species", "Dravidian Mega-Caterpillar");

      AssertPropertyValue(aliens, "Species", "Dravidian Mega-Caterpillar");
      AssertPropertyInconsistent(aliens, "LegCount");
    }

    [Test]
    public void SettingValueUsingManyPropagatesToAllWrappedObjects()
    {
      Alien alice = new Alien { Species = "Dalek", LegCount = 0 };
      Alien bob = new Alien { Species = "Vogon", LegCount = 2 };
      MultipleObjectWrapper aliens = new MultipleObjectWrapper(alice, bob);

      SetValueUsingMany(aliens, "Species", "Dravidian Mega-Caterpillar");

      AssertPropertyValue(aliens, "Species", "Dravidian Mega-Caterpillar");
      AssertPropertyInconsistent(aliens, "LegCount");
    }

    [Test]
    public void SettingValueRaisesPropertyChangeNotifications()
    {
      bool gotSpeciesPropertyChanging = false;
      bool gotLegCountPropertyChanging = false;
      bool gotSpeciesPropertyChanged = false;
      bool gotLegCountPropertyChanged = false;

      Alien alice = new Alien { Species = "Dalek", LegCount = 0 };
      Alien bob = new Alien { Species = "Vogon", LegCount = 2 };
      MultipleObjectWrapper aliens = new MultipleObjectWrapper(alice, bob);

      aliens.PropertyChanging += delegate(object sender, PropertyChangingEventArgs e)
      {
        AssertPropertyInconsistent(aliens, e.PropertyName);
        switch (e.PropertyName)
        {
          case "Species": gotSpeciesPropertyChanging = true; break;
          case "LegCount": gotLegCountPropertyChanging = true; break;
        }
      };

      aliens.PropertyChanged += delegate(object sender, PropertyChangedEventArgs e)
      {
        AssertPropertyConsistent(aliens, e.PropertyName);
        switch (e.PropertyName)
        {
          case "Species": gotSpeciesPropertyChanged = true; break;
          case "LegCount": gotLegCountPropertyChanged = true; break;
        }
      };

      SetValue(aliens, "Species", "Dravidian Mega-Caterpillar");

      Assert.IsTrue(gotSpeciesPropertyChanging);
      Assert.IsFalse(gotLegCountPropertyChanging);
      Assert.IsTrue(gotSpeciesPropertyChanged);
      Assert.IsFalse(gotLegCountPropertyChanged);
    }

    [Test]
    public void ResettingInconsistentValueRaisesPropertyChangeNotificationOnWrapper()
    {
      bool gotSpeciesPropertyChanging = false;
      bool gotLegCountPropertyChanging = false;
      bool gotSpeciesPropertyChanged = false;
      bool gotLegCountPropertyChanged = false;

      Alien alice = new Alien { Species = "Dalek", LegCount = 0 };
      Alien bob = new Alien { Species = "Vogon", LegCount = 2 };
      MultipleObjectWrapper aliens = new MultipleObjectWrapper(alice, bob);

      aliens.PropertyChanging += delegate(object sender, PropertyChangingEventArgs e)
      {
        AssertPropertyInconsistent(aliens, e.PropertyName);
        switch (e.PropertyName)
        {
          case "Species": gotSpeciesPropertyChanging = true; break;
          case "LegCount": gotLegCountPropertyChanging = true; break;
        }
      };

      aliens.PropertyChanged += delegate(object sender, PropertyChangedEventArgs e)
      {
        AssertPropertyConsistent(aliens, e.PropertyName);
        switch (e.PropertyName)
        {
          case "Species": gotSpeciesPropertyChanged = true; break;
          case "LegCount": gotLegCountPropertyChanged = true; break;
        }
      };

      Many species = GetMany(aliens, "Species");
      species.Reset();

      Assert.IsTrue(gotSpeciesPropertyChanging);
      Assert.IsFalse(gotLegCountPropertyChanging);
      Assert.IsTrue(gotSpeciesPropertyChanged);
      Assert.IsFalse(gotLegCountPropertyChanged);
    }

    [Test]
    public void SettingValueUsingManyRaisesPropertyChangeNotifications()
    {
      bool gotSpeciesPropertyChanging = false;
      bool gotLegCountPropertyChanging = false;
      bool gotSpeciesPropertyChanged = false;
      bool gotLegCountPropertyChanged = false;

      Alien alice = new Alien { Species = "Dalek", LegCount = 0 };
      Alien bob = new Alien { Species = "Vogon", LegCount = 2 };
      MultipleObjectWrapper aliens = new MultipleObjectWrapper(alice, bob);

      aliens.PropertyChanging += delegate(object sender, PropertyChangingEventArgs e)
      {
        AssertPropertyInconsistent(aliens, e.PropertyName);
        switch (e.PropertyName)
        {
          case "Species": gotSpeciesPropertyChanging = true; break;
          case "LegCount": gotLegCountPropertyChanging = true; break;
        }
      };

      aliens.PropertyChanged += delegate(object sender, PropertyChangedEventArgs e)
      {
        AssertPropertyConsistent(aliens, e.PropertyName);
        switch (e.PropertyName)
        {
          case "Species": gotSpeciesPropertyChanged = true; break;
          case "LegCount": gotLegCountPropertyChanged = true; break;
        }
      };

      SetValueUsingMany(aliens, "Species", "Dravidian Mega-Caterpillar");

      Assert.IsTrue(gotSpeciesPropertyChanging);
      Assert.IsFalse(gotLegCountPropertyChanging);
      Assert.IsTrue(gotSpeciesPropertyChanged);
      Assert.IsFalse(gotLegCountPropertyChanged);
    }

    [Test]
    public void NullValues()
    {
      Alien alice = new Alien { Species = null, LegCount = 0 };
      Alien bob = new Alien { Species = null, LegCount = 2 };
      MultipleObjectWrapper aliens = new MultipleObjectWrapper(alice, bob);

      AssertPropertyValue(aliens, "Species", null);
    }

    [Test]
    public void ModifyingWrappedObjectCausesChangeNotifications()
    {
      bool gotConsistencyChangeNotification = false;
      bool gotValueChangeNotification = false;

      Alien alice = new Alien { Species = "Dalek", LegCount = 0 };
      Alien bob = new Alien { Species = "Vogon", LegCount = 2 };
      MultipleObjectWrapper aliens = new MultipleObjectWrapper(alice, bob);
      Many many = GetMany(aliens, "Species");

      many.PropertyChanged += delegate(object sender, PropertyChangedEventArgs e)
      {
        if (e.PropertyName == "IsConsistent")
        {
          gotConsistencyChangeNotification = true;
        }
        if (e.PropertyName == "Value")
        {
          gotValueChangeNotification = true;
        }
      };

      Assert.IsFalse(many.IsConsistent);

      alice.Species = "Vogon";

      Assert.IsTrue(gotConsistencyChangeNotification);
      Assert.IsTrue(gotValueChangeNotification);
      Assert.IsTrue(many.IsConsistent);

      gotConsistencyChangeNotification = false;
      gotValueChangeNotification = false;

      alice.Species = "Space Weevil";

      Assert.IsTrue(gotConsistencyChangeNotification);
      Assert.IsTrue(gotValueChangeNotification);
      Assert.IsFalse(many.IsConsistent);
    }

    [Test]
    public void ModifyingObjectCollectionUpdatesPropertyCollection()
    {
      ObservableCollection<object> selection = new ObservableCollection<object>();
      selection.Add(Animal.Fang);
      selection.Add(Animal.Kiki);
      MultipleObjectWrapper wrapper = new MultipleObjectWrapper(selection);

      AssertPropertyCount(wrapper, 4);

      selection.Add(Plant.ZogTheDevourer);

      AssertPropertyCount(wrapper, 2);

      selection.Remove(Plant.ZogTheDevourer);

      AssertPropertyCount(wrapper, 4);
    }

    [Test]
    public void PropertyValuesReportedCorrectlyAfterModifyingCollection()
    {
      ObservableCollection<object> selection = new ObservableCollection<object>();
      selection.Add(Animal.Fang);
      selection.Add(Animal.Kiki);
      MultipleObjectWrapper wrapper = new MultipleObjectWrapper(selection);

      AssertPropertyValue(wrapper, "Species", "Cat");

      selection.Add(Plant.ZogTheDevourer);
      AssertPropertyInconsistent(wrapper, "Species");

      selection.Remove(Plant.ZogTheDevourer);
      AssertPropertyValue(wrapper, "Species", "Cat");
    }

    [Test]
    public void PropertyChangeNotificationsStillWorkAfterModifyingCollection()
    {
      Plant sundew = new Plant { Species = "Sundew" };

      ObservableCollection<object> selection = new ObservableCollection<object>();
      selection.Add(Animal.Fang);
      selection.Add(Animal.Kiki);
      MultipleObjectWrapper wrapper = new MultipleObjectWrapper(selection);

      AssertPropertyValue(wrapper, "Species", "Cat");

      selection.Add(sundew);
      AssertPropertyInconsistent(wrapper, "Species");

      sundew.Species = "Cat";
      AssertPropertyValue(wrapper, "Species", "Cat");
    }


    [Test]
    public void SettingValueStillWorksAfterModifyingCollection()
    {
      Alien alice = new Alien { Species = "Dalek", LegCount = 0 };
      Alien bob = new Alien { Species = "Vogon", LegCount = 2 };
      Alien carol = new Alien { Species = "Space Squid", LegCount = 1000000 };
      ObservableCollection<object> selection = new ObservableCollection<object>();
      selection.Add(alice);
      selection.Add(bob);

      MultipleObjectWrapper aliens = new MultipleObjectWrapper(selection);

      SetValue(aliens, "Species", "Dravidian Mega-Caterpillar");
      Assert.AreEqual("Dravidian Mega-Caterpillar", alice.Species);
      Assert.AreEqual("Dravidian Mega-Caterpillar", bob.Species);

      selection.Add(carol);

      Assert.AreEqual("Space Squid", carol.Species);
      SetValue(aliens, "Species", "Dravidian Mega-Caterpillar");

      Assert.AreEqual("Dravidian Mega-Caterpillar", carol.Species);

      AssertPropertyValue(aliens, "Species", "Dravidian Mega-Caterpillar");
      AssertPropertyInconsistent(aliens, "LegCount");
    }

    [Test]
    public void SettingValueUsingManyStillWorksAfterModifyingCollection()
    {
      Alien alice = new Alien { Species = "Dalek", LegCount = 0 };
      Alien bob = new Alien { Species = "Vogon", LegCount = 2 };
      Alien carol = new Alien { Species = "Space Squid", LegCount = 1000000 };
      ObservableCollection<object> selection = new ObservableCollection<object>();
      selection.Add(alice);
      selection.Add(bob);

      MultipleObjectWrapper aliens = new MultipleObjectWrapper(selection);

      SetValue(aliens, "Species", "Dravidian Mega-Caterpillar");
      Assert.AreEqual("Dravidian Mega-Caterpillar", alice.Species);
      Assert.AreEqual("Dravidian Mega-Caterpillar", bob.Species);

      selection.Add(carol);

      Assert.AreEqual("Space Squid", carol.Species);
      SetValueUsingMany(aliens, "Species", "Dravidian Mega-Caterpillar");

      Assert.AreEqual("Dravidian Mega-Caterpillar", carol.Species);

      AssertPropertyValue(aliens, "Species", "Dravidian Mega-Caterpillar");
      AssertPropertyInconsistent(aliens, "LegCount");
    }

    [Test]
    public void RemovedItemsAreNotAffectedBySettingValue()
    {
      Alien alice = new Alien { Species = "Dalek", LegCount = 0 };
      Alien bob = new Alien { Species = "Vogon", LegCount = 2 };
      Alien carol = new Alien { Species = "Space Squid", LegCount = 1000000 };
      ObservableCollection<object> selection = new ObservableCollection<object>();
      selection.Add(alice);
      selection.Add(bob);
      selection.Add(carol);

      MultipleObjectWrapper aliens = new MultipleObjectWrapper(selection);

      SetValue(aliens, "Species", "Dravidian Mega-Caterpillar");
      Assert.AreEqual("Dravidian Mega-Caterpillar", alice.Species);
      Assert.AreEqual("Dravidian Mega-Caterpillar", bob.Species);
      Assert.AreEqual("Dravidian Mega-Caterpillar", carol.Species);
      AssertPropertyInconsistent(aliens, "LegCount");

      selection.Remove(carol);

      SetValue(aliens, "LegCount", 4L);
      Assert.AreEqual(4, alice.LegCount);
      Assert.AreEqual(4, bob.LegCount);
      Assert.AreEqual(1000000, carol.LegCount);
    }

    [Test]
    public void RemovedItemsAreNotAffectedBySettingValueUsingMany()
    {
      Alien alice = new Alien { Species = "Dalek", LegCount = 0 };
      Alien bob = new Alien { Species = "Vogon", LegCount = 2 };
      Alien carol = new Alien { Species = "Space Squid", LegCount = 1000000 };
      ObservableCollection<object> selection = new ObservableCollection<object>();
      selection.Add(alice);
      selection.Add(bob);
      selection.Add(carol);

      MultipleObjectWrapper aliens = new MultipleObjectWrapper(selection);

      SetValueUsingMany(aliens, "Species", "Dravidian Mega-Caterpillar");
      Assert.AreEqual("Dravidian Mega-Caterpillar", alice.Species);
      Assert.AreEqual("Dravidian Mega-Caterpillar", bob.Species);
      Assert.AreEqual("Dravidian Mega-Caterpillar", carol.Species);
      AssertPropertyInconsistent(aliens, "LegCount");

      selection.Remove(carol);

      SetValueUsingMany(aliens, "LegCount", 4);
      Assert.AreEqual(4, alice.LegCount);
      Assert.AreEqual(4, bob.LegCount);
      Assert.AreEqual(1000000, carol.LegCount);
    }

    // TODO: deduplicate across these and other tests

    [Test]
    public void SettingValueStillRaisesPropertyChangeNotificationsAfterModifyingCollection()
    {
      bool gotSpeciesPropertyChanging = false;
      bool gotLegCountPropertyChanging = false;
      bool gotSpeciesPropertyChanged = false;
      bool gotLegCountPropertyChanged = false;

      Alien alice = new Alien { Species = "Dalek", LegCount = 0 };
      Alien bob = new Alien { Species = "Vogon", LegCount = 2 };
      Alien carol = new Alien { Species = "Space Squid", LegCount = 1000000 };
      ObservableCollection<object> selection = new ObservableCollection<object>();
      selection.Add(alice);
      selection.Add(bob);
      MultipleObjectWrapper aliens = new MultipleObjectWrapper(selection);

      aliens.PropertyChanging += delegate(object sender, PropertyChangingEventArgs e)
      {
        switch (e.PropertyName)
        {
          case "Species": gotSpeciesPropertyChanging = true; break;
          case "LegCount": gotLegCountPropertyChanging = true; break;
        }
      };

      aliens.PropertyChanged += delegate(object sender, PropertyChangedEventArgs e)
      {
        AssertPropertyConsistent(aliens, e.PropertyName);
        switch (e.PropertyName)
        {
          case "Species": gotSpeciesPropertyChanged = true; break;
          case "LegCount": gotLegCountPropertyChanged = true; break;
        }
      };

      SetValue(aliens, "Species", "Dravidian Mega-Caterpillar");

      gotSpeciesPropertyChanging = false;
      gotLegCountPropertyChanging = false;
      gotSpeciesPropertyChanged = false;
      gotLegCountPropertyChanged = false;

      selection.Add(carol);

      SetValue(aliens, "Species", "Arcturan Mega-Donkey");

      Assert.IsTrue(gotSpeciesPropertyChanging);
      Assert.IsFalse(gotLegCountPropertyChanging);
      Assert.IsTrue(gotSpeciesPropertyChanged);
      Assert.IsFalse(gotLegCountPropertyChanged);

      gotSpeciesPropertyChanging = false;
      gotLegCountPropertyChanging = false;
      gotSpeciesPropertyChanged = false;
      gotLegCountPropertyChanged = false;

      selection.Remove(carol);

      SetValue(aliens, "Species", "Fie");

      Assert.IsTrue(gotSpeciesPropertyChanging);
      Assert.IsFalse(gotLegCountPropertyChanging);
      Assert.IsTrue(gotSpeciesPropertyChanged);
      Assert.IsFalse(gotLegCountPropertyChanged);
    }

    [Test]
    public void SettingValueUsingManyStillRaisesPropertyChangeNotificationsAfterModifyingCollection()
    {
      bool gotSpeciesPropertyChanging = false;
      bool gotLegCountPropertyChanging = false;
      bool gotSpeciesPropertyChanged = false;
      bool gotLegCountPropertyChanged = false;

      Alien alice = new Alien { Species = "Dalek", LegCount = 0 };
      Alien bob = new Alien { Species = "Vogon", LegCount = 2 };
      Alien carol = new Alien { Species = "Space Squid", LegCount = 1000000 };
      ObservableCollection<object> selection = new ObservableCollection<object>();
      selection.Add(alice);
      selection.Add(bob);
      MultipleObjectWrapper aliens = new MultipleObjectWrapper(selection);

      aliens.PropertyChanging += delegate(object sender, PropertyChangingEventArgs e)
      {
        switch (e.PropertyName)
        {
          case "Species": gotSpeciesPropertyChanging = true; break;
          case "LegCount": gotLegCountPropertyChanging = true; break;
        }
      };

      aliens.PropertyChanged += delegate(object sender, PropertyChangedEventArgs e)
      {
        AssertPropertyConsistent(aliens, e.PropertyName);
        switch (e.PropertyName)
        {
          case "Species": gotSpeciesPropertyChanged = true; break;
          case "LegCount": gotLegCountPropertyChanged = true; break;
        }
      };

      SetValueUsingMany(aliens, "Species", "Dravidian Mega-Caterpillar");

      gotSpeciesPropertyChanging = false;
      gotLegCountPropertyChanging = false;
      gotSpeciesPropertyChanged = false;
      gotLegCountPropertyChanged = false;

      selection.Add(carol);

      SetValueUsingMany(aliens, "Species", "Arcturan Mega-Donkey");

      Assert.IsTrue(gotSpeciesPropertyChanging);
      Assert.IsFalse(gotLegCountPropertyChanging);
      Assert.IsTrue(gotSpeciesPropertyChanged);
      Assert.IsFalse(gotLegCountPropertyChanged);

      gotSpeciesPropertyChanging = false;
      gotLegCountPropertyChanging = false;
      gotSpeciesPropertyChanged = false;
      gotLegCountPropertyChanged = false;

      selection.Remove(carol);

      SetValueUsingMany(aliens, "Species", "Fie");

      Assert.IsTrue(gotSpeciesPropertyChanging);
      Assert.IsFalse(gotLegCountPropertyChanging);
      Assert.IsTrue(gotSpeciesPropertyChanged);
      Assert.IsFalse(gotLegCountPropertyChanged);
    }

    [Test]
    [STAThread]
    public void CollectionMembersAreSupportedIfReferenceEqual()
    {
      PropertyGrid grid = new PropertyGrid();
      grid.SelectedObjects = new object[] { Person.Alice };

      Node collectionNode = null;
      foreach (Node node in grid.Nodes)
      {
        if (node.Name == "Friends")
        {
          collectionNode = node;
          break;
        }
      }
      Assert.IsNotNull(collectionNode);

      Assert.IsInstanceOf<CollectionElement>(collectionNode.Children[0]);
      Assert.AreEqual(4, collectionNode.Children.Count);
    }

    [Test]
    public void ManyValuePropertyDescriptorPropagatesPropertyLevelTypeConverterWhenConsistent()
    {
      MultipleObjectWrapper wrapper = new MultipleObjectWrapper(Person.Alice, Person.Bob);
      Many isAlive = GetMany(wrapper, "Alive");
      PropertyNode node = ManyToNodeConverter.CreateNodeFromMany(isAlive, null);

      Assert.IsInstanceOf<YesNoConverter>(node.Property.Converter);
    }

    [Test]
    public void ManyValuePropertyDescriptorPropagatesCategoryWhenSameSource()
    {
      MultipleObjectWrapper wrapper = new MultipleObjectWrapper(Person.Alice, Person.Bob);
      Many firstName = GetMany(wrapper, "FirstName");
      PropertyNode node = ManyToNodeConverter.CreateNodeFromMany(firstName, null);

      Assert.AreEqual("Identity", node.Property.Category);
    }

    [Test]
    public void ManyValuePropertyDescriptorPropagatesCategoryWhenSameOnAllSources()
    {
      MultipleObjectWrapper wrapper = new MultipleObjectWrapper(Person.Alice, Plant.ZogTheDevourer);
      Many firstName = GetMany(wrapper, "FirstName");
      PropertyNode node = ManyToNodeConverter.CreateNodeFromMany(firstName, null);

      Assert.AreEqual("Identity", node.Property.Category);
    }

    [Test]
    public void ManyValuePropertyDescriptorDoesNotPropagatesCategoryWhenNotSame()
    {
      MultipleObjectWrapper wrapper = new MultipleObjectWrapper(Person.Alice, Alien.Anastasia);
      Many firstName = GetMany(wrapper, "FirstName");
      PropertyNode node = ManyToNodeConverter.CreateNodeFromMany(firstName, null);

      Assert.AreEqual(System.ComponentModel.CategoryAttribute.Default.Category, node.Property.Category);
    }
  }
}
