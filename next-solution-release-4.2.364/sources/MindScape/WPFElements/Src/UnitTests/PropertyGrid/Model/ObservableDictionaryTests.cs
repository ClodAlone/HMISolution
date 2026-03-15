using System;
using System.Collections.Generic;
using NUnit.Framework;
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Mindscape.WpfElements.WpfPropertyGrid.UnitTests
{
  [TestFixture]
  public class ObservableDictionaryTests
  {
    [Test]
    public void Add()
    {
      ObservableDictionary<string, string> dict = new ObservableDictionary<string, string>();
      Assert.AreEqual(0, dict.Count);
      dict.Add("key1", "value1");
      dict.Add("key2", "value2");

      Assert.AreEqual(2, dict.Count);
      Assert.IsTrue(dict.ContainsKey("key1"));
      Assert.IsTrue(dict.ContainsKey("key2"));
      Assert.AreEqual("value1", dict["key1"]);
      Assert.AreEqual("value2", dict["key2"]);

      string value;

      Assert.IsTrue(dict.TryGetValue("key1", out value));
      Assert.AreEqual("value1", value);
      Assert.IsTrue(dict.TryGetValue("key2", out value));
      Assert.AreEqual("value2", value);
    }

    [Test]
    [ExpectedException(typeof(ArgumentException))]
    public void AddDuplicate()
    {
      ObservableDictionary<string, string> dict = new ObservableDictionary<string, string>();
      dict.Add("key1", "value1");
      dict.Add("key1", "value2");
    }

    [Test]
    [ExpectedException(typeof(ArgumentNullException))]
    public void AddNullKey()
    {
      ObservableDictionary<string, string> dict = new ObservableDictionary<string, string>();
      dict.Add(null, "value1");
    }

    [Test]
    public void Remove()
    {
      ObservableDictionary<string, string> dict = new ObservableDictionary<string, string>();
      dict.Add("key1", "value1");
      dict.Add("key2", "value2");

      Assert.IsTrue(dict.Remove("key1"));
      Assert.AreEqual(1, dict.Count);
      Assert.IsFalse(dict.ContainsKey("key1"));

      string dummy;
      Assert.IsFalse(dict.TryGetValue("key1", out dummy));
    }

    [Test]
    public void RemoveNonExistent()
    {
      ObservableDictionary<string, string> dict = new ObservableDictionary<string, string>();
      dict.Add("key1", "value1");
      dict.Add("key2", "value2");

      Assert.IsFalse(dict.Remove("key3"));
    }

    [Test]
    public void Events()
    {
      List<NotifyCollectionChangedEventArgs> collectionChanges = new List<NotifyCollectionChangedEventArgs>();
      List<string> changedProperties = new List<string>();

      ObservableDictionary<string, string> dict = new ObservableDictionary<string, string>();
      ICollection<KeyValuePair<string, string>> coll = dict;
      dict.CollectionChanged += delegate(object sender, NotifyCollectionChangedEventArgs e)
        {
          Assert.AreEqual(dict, sender);
          collectionChanges.Add(e);
        };
      dict.PropertyChanged += delegate(object sender, PropertyChangedEventArgs e)
        {
          Assert.AreEqual(dict, sender);
          changedProperties.Add(e.PropertyName);
        };

      dict.Add("key1", "value1");

      Assert.AreEqual(2, changedProperties.Count);
      Assert.IsTrue(changedProperties.Contains("Count"));
      Assert.IsTrue(changedProperties.Contains("Item[]"));

      Assert.AreEqual(1, collectionChanges.Count);
      Assert.AreEqual(NotifyCollectionChangedAction.Add, collectionChanges[0].Action);
      Assert.AreEqual(0, collectionChanges[0].NewStartingIndex);
      Assert.AreEqual(1, collectionChanges[0].NewItems.Count);
      Assert.IsNull(collectionChanges[0].OldItems);

      collectionChanges.Clear();
      changedProperties.Clear();

      dict.Add("key2", "value2");

      Assert.AreEqual(1, collectionChanges.Count);
      Assert.AreEqual(NotifyCollectionChangedAction.Add, collectionChanges[0].Action);
      Assert.AreEqual(1, collectionChanges[0].NewStartingIndex);
      Assert.AreEqual(1, collectionChanges[0].NewItems.Count);
      Assert.IsNull(collectionChanges[0].OldItems);

      collectionChanges.Clear();
      changedProperties.Clear();

      coll.Add(new KeyValuePair<string, string>("key3", "value3"));

      Assert.AreEqual(2, changedProperties.Count);
      
      Assert.AreEqual(1, collectionChanges.Count);
      Assert.AreEqual(NotifyCollectionChangedAction.Add, collectionChanges[0].Action);
      Assert.AreEqual(2, collectionChanges[0].NewStartingIndex);
      Assert.AreEqual(1, collectionChanges[0].NewItems.Count);
      Assert.IsNull(collectionChanges[0].OldItems);

      collectionChanges.Clear();
      changedProperties.Clear();

      dict["key0"] = "value0";

      Assert.AreEqual(1, collectionChanges.Count);
      Assert.AreEqual(NotifyCollectionChangedAction.Add, collectionChanges[0].Action);
      Assert.AreEqual(0, collectionChanges[0].NewStartingIndex);
      Assert.AreEqual(1, collectionChanges[0].NewItems.Count);
      Assert.IsNull(collectionChanges[0].OldItems);

      collectionChanges.Clear();
      changedProperties.Clear();

      dict["key1"] = "newValue";

      Assert.AreEqual(1, changedProperties.Count);
      Assert.IsTrue(changedProperties.Contains("Item[]"));

      Assert.AreEqual(1, collectionChanges.Count);
      Assert.AreEqual(NotifyCollectionChangedAction.Replace, collectionChanges[0].Action);
      Assert.AreEqual(1, collectionChanges[0].NewStartingIndex);
      Assert.AreEqual(1, collectionChanges[0].NewItems.Count);
      Assert.AreEqual(1, collectionChanges[0].OldStartingIndex);
      Assert.AreEqual(1, collectionChanges[0].OldItems.Count);

      collectionChanges.Clear();
      changedProperties.Clear();

      dict["key1"] = "newValue";

      Assert.AreEqual(0, changedProperties.Count);
      Assert.AreEqual(0, collectionChanges.Count);

      collectionChanges.Clear();
      changedProperties.Clear();

      dict.Remove("key1");

      Assert.AreEqual(2, changedProperties.Count);
      Assert.IsTrue(changedProperties.Contains("Count"));
      Assert.IsTrue(changedProperties.Contains("Item[]"));

      Assert.AreEqual(1, collectionChanges.Count);
      Assert.AreEqual(NotifyCollectionChangedAction.Remove, collectionChanges[0].Action);
      Assert.IsNull(collectionChanges[0].NewItems);
      Assert.AreEqual(1, collectionChanges[0].OldStartingIndex);
      Assert.AreEqual(1, collectionChanges[0].OldItems.Count);

      collectionChanges.Clear();
      changedProperties.Clear();

      coll.Remove(new KeyValuePair<string,string>("key3", "wrongValue"));

      Assert.AreEqual(0, changedProperties.Count);
      Assert.AreEqual(0, collectionChanges.Count);

      collectionChanges.Clear();
      changedProperties.Clear();

      coll.Remove(new KeyValuePair<string, string>("key3", "value3"));

      Assert.AreEqual(2, changedProperties.Count);

      Assert.AreEqual(1, collectionChanges.Count);
      Assert.AreEqual(NotifyCollectionChangedAction.Remove, collectionChanges[0].Action);
      Assert.IsNull(collectionChanges[0].NewItems);
      Assert.AreEqual(2, collectionChanges[0].OldStartingIndex);
      Assert.AreEqual(1, collectionChanges[0].OldItems.Count);

      collectionChanges.Clear();
      changedProperties.Clear();

      dict.Clear();

      Assert.AreEqual(2, changedProperties.Count);

      Assert.AreEqual(1, collectionChanges.Count);
      Assert.AreEqual(NotifyCollectionChangedAction.Reset, collectionChanges[0].Action);

      collectionChanges.Clear();
      changedProperties.Clear();

      dict.Clear();

      Assert.AreEqual(2, changedProperties.Count);  // ensure always raise property changed events, even if collection is already empty, for compatibility with ObservableCollection

      Assert.AreEqual(1, collectionChanges.Count);
      Assert.AreEqual(NotifyCollectionChangedAction.Reset, collectionChanges[0].Action);
    }

    [Test]
    public void KeysAndValues()
    {
      ObservableDictionary<string, string> dict = new ObservableDictionary<string, string>();
      dict.Add("key1", "value1");
      dict.Add("key2", "value2");

      Assert.AreEqual(2, dict.Keys.Count);
      Assert.IsTrue(dict.Keys.Contains("key1"));
      Assert.IsTrue(dict.Keys.Contains("key2"));

      Assert.AreEqual(2, dict.Values.Count);
      Assert.IsTrue(dict.Values.Contains("value1"));
      Assert.IsTrue(dict.Values.Contains("value2"));
    }

    [Test]
    public void Indexer()
    {
      ObservableDictionary<string, string> dict = new ObservableDictionary<string, string>();
      dict["key1"] = "value1";
      dict["key2"] = "value2";

      Assert.AreEqual(2, dict.Count);
      Assert.AreEqual("value1", dict["key1"]);
      Assert.AreEqual("value2", dict["key2"]);

      dict["key2"] = "newValue";

      Assert.AreEqual(2, dict.Count);
      Assert.AreEqual("value1", dict["key1"]);
      Assert.AreEqual("newValue", dict["key2"]);
    }

    [Test]
    public void Clear()
    {
      ObservableDictionary<string, string> dict = new ObservableDictionary<string, string>();
      dict.Add("key1", "value1");
      dict.Add("key2", "value2");

      Assert.AreEqual(2, dict.Count);

      dict.Clear();

      Assert.AreEqual(0, dict.Count);
      Assert.IsFalse(dict.ContainsKey("key1"));
    }

    [Test]
    public void KeyValuePairMethods()
    {
      ObservableDictionary<string, string> dict = new ObservableDictionary<string, string>();
      ICollection<KeyValuePair<string, string>> coll = dict;
      dict.Add("key1", "value1");
      coll.Add(new KeyValuePair<string,string>("key2", "value2"));

      Assert.AreEqual(2, dict.Count);
      Assert.IsTrue(dict.ContainsKey("key2"));
      Assert.IsTrue(coll.Contains(new KeyValuePair<string,string>("key1", "value1")));
      Assert.IsFalse(coll.Contains(new KeyValuePair<string,string>("key1", "value2")));

      KeyValuePair<string, string>[] kvps = new KeyValuePair<string, string>[4];
      dict.CopyTo(kvps, 1);
      Assert.AreEqual(null, kvps[0].Key);
      Assert.AreEqual("key1", kvps[1].Key);
      Assert.AreEqual("key2", kvps[2].Key);
      Assert.AreEqual(null, kvps[3].Key);

      Assert.IsTrue(coll.Remove(new KeyValuePair<string, string>("key1", "value1")));
      Assert.AreEqual(1, dict.Count);
      Assert.IsFalse(coll.Remove(new KeyValuePair<string,string>("key2", "wrongValue")));
      Assert.AreEqual(1, dict.Count);
    }

    [Test]
    public void Enumerators()
    {
      ObservableDictionary<string, string> dict = new ObservableDictionary<string, string>();
      dict.Add("key1", "value1");
      dict.Add("key2", "value2");

      IEnumerator<KeyValuePair<string, string>> enumerator1 = dict.GetEnumerator();
      int count1 = 0;
      while (enumerator1.MoveNext())
      {
        ++count1;
      }
      Assert.AreEqual(2, count1);

      IEnumerator enumerator2 = ((IEnumerable)dict).GetEnumerator();
      int count2 = 0;
      while (enumerator2.MoveNext())
      {
        Assert.IsInstanceOf<KeyValuePair<string, string>>(enumerator2.Current);
        ++count2;
      }
      Assert.AreEqual(2, count2);
    }

    [Test]
    public void ReadOnly()
    {
      ObservableDictionary<string, string> dict = new ObservableDictionary<string, string>();
      Assert.IsFalse(dict.IsReadOnly);
      Assert.IsFalse(dict.IsFixedSize);
    }

    [Test]
    public void Sync()
    {
      ObservableDictionary<string, string> dict = new ObservableDictionary<string, string>();
      Assert.IsFalse(dict.IsSynchronized);
      Assert.IsNotNull(dict.SyncRoot);
    }

    [Test]
    public void AsIDictionary()
    {
      IDictionary dict = new ObservableDictionary<string, string>();
      dict.Add("fie", "bah");
      Assert.IsTrue(dict.Contains("fie"));
      Assert.IsNotNull(dict.GetEnumerator());
      Assert.AreEqual(1, dict.Keys.Count);
      Assert.AreEqual(1, dict.Values.Count);
      Assert.AreEqual("bah", dict["fie"]);
      dict["fie"] = "tchah";
      Assert.AreEqual("tchah", dict["fie"]);
      object[] array = new object[1];
      dict.CopyTo(array, 0);
      Assert.IsNotNull(array[0]);
      dict.Remove("fie");
      Assert.IsFalse(dict.Contains("fie"));
    }

    [Test]
    [ExpectedException(typeof(InvalidCastException))]
    public void AsIDictionary_BadAdd_Key()
    {
      IDictionary dict = new ObservableDictionary<string, string>();
      dict.Add(123, "fie");      
    }

    [Test]
    [ExpectedException(typeof(InvalidCastException))]
    public void AsIDictionary_BadAdd_Value()
    {
      IDictionary dict = new ObservableDictionary<string, string>();
      dict.Add("fie", 123);
    }

    [Test]
    [ExpectedException(typeof(InvalidCastException))]
    public void AsIDictionary_BadSet_Key()
    {
      IDictionary dict = new ObservableDictionary<string, string>();
      dict[123] = "fie";
    }

    [Test]
    [ExpectedException(typeof(InvalidCastException))]
    public void AsIDictionary_BadSet_Value()
    {
      IDictionary dict = new ObservableDictionary<string, string>();
      dict["fie"] = 123;
    }

    [Test]
    [ExpectedException(typeof(InvalidCastException))]
    public void AsIDictionary_BadRemove_Key()
    {
      IDictionary dict = new ObservableDictionary<string, string>();
      dict.Remove(123);
    }

    [Test]
    public void Serializable()
    {
      ObservableDictionary<int, string> strings = new ObservableDictionary<int, string>();
      strings[1] = "One";
      strings[2] = "Two";

      MemoryStream stm = new MemoryStream();
      BinaryFormatter formatter = new BinaryFormatter();
      formatter.Serialize(stm, strings);

      stm.Seek(0, SeekOrigin.Begin);
      object deserialised = formatter.Deserialize(stm);

      stm.Dispose();

      Assert.IsInstanceOf<ObservableDictionary<int, string>>(deserialised);
      ObservableDictionary<int, string> deserialisedStrings = (ObservableDictionary<int, string>)deserialised;
      Assert.AreEqual(2, deserialisedStrings.Count);
      Assert.AreEqual("One", deserialisedStrings[1]);
      Assert.AreEqual("Two", deserialisedStrings[2]);
    }
  }
}