/*
 *  Copyright © Northwoods Software Corporation, 1998-2016. All Rights Reserved.
 *
 *  Restricted Rights: Use, duplication, or disclosure by the U.S.
 *  Government is subject to restrictions as set forth in subparagraph
 *  (c) (1) (ii) of DFARS 252.227-7013, or in FAR 52.227-19, or in FAR
 *  52.227-14 Alt. III, as applicable.
 *
 *  This software is proprietary to and embodies the confidential
 *  technology of Northwoods Software Corporation. Possession, use, or
 *  copying of this software and media is authorized only pursuant to a
 *  valid written license from Northwoods or an authorized sublicensor.
 */

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Reflection;
#if !WINDOWS_UWP
using System.Windows;
using System.Windows.Data;
#else
using System.Collections;
#endif

namespace Northwoods.GoXam.Model {

  // Because thrown exceptions are often caught silently,
  // never just "throw new ...Exception(...)",
  // but call ModelHelper.Error(...) instead.
  // These Error methods first write out the error message to Trace listeners,
  // before throwing an InvalidOperationException.

  // But there is no System.Diagnostics.Trace in Silverlight,
  // so those messages go to System.Diagnostics.Debug instead
  // and might be lost on that platform.

  internal static class ModelHelper {

#if WINDOWS_UWP  // System.Diagnostics.Trace

        public static void Trace(String msg) {
      System.Diagnostics.Debug.WriteLine(msg);
    }

    public static void Trace(int indent, String msg) {
      String s = "";
      for (int i = 0; i < indent; i++) s += "  ";
      System.Diagnostics.Debug.WriteLine(s + msg);
    }

    public static void Trace(IDiagramModel model, String msg) {
      System.Diagnostics.Debug.WriteLine(model.Name + "! " + msg);
    }

    public static void Error(String msg) {
      System.Diagnostics.Debug.WriteLine(msg);
      throw new InvalidOperationException(msg);
    }

    public static void Error(IDiagramModel model, String msg) {
      System.Diagnostics.Debug.WriteLine("Error in model " + model.Name);
      System.Diagnostics.Debug.WriteLine(msg);
      throw new InvalidOperationException(msg);
    }

    public static void Error(IDiagramModel model, String msg, Exception ex) {
      System.Diagnostics.Debug.WriteLine("Error in model " + model.Name);
      System.Diagnostics.Debug.WriteLine(msg);
      System.Diagnostics.Debug.WriteLine(ex);
      throw new InvalidOperationException(msg, ex);
    }

#else

    public static void Trace(String msg) {
      System.Diagnostics.Trace.WriteLine(msg);
    }

    public static void Trace(int indent, String msg) {
      String s = "";
      for (int i = 0; i < indent; i++) s += "  ";
      System.Diagnostics.Trace.WriteLine(s + msg);
    }

    public static void Trace(IDiagramModel model, String msg) {
      System.Diagnostics.Trace.WriteLine(model.Name + "! " + msg);
    }

    public static void Error(String msg) {
      System.Diagnostics.Trace.WriteLine(msg);
      throw new InvalidOperationException(msg);
    }

    public static void Error(IDiagramModel model, String msg) {
      System.Diagnostics.Trace.WriteLine("Error in model " + model.Name);
      System.Diagnostics.Trace.WriteLine(msg);
      throw new InvalidOperationException(msg);
    }

    public static void Error(IDiagramModel model, String msg, Exception ex) {
      System.Diagnostics.Trace.WriteLine("Error in model " + model.Name);
      System.Diagnostics.Trace.WriteLine(msg);
      System.Diagnostics.Trace.WriteLine(ex);
      throw new InvalidOperationException(msg, ex);
    }

    public static T CopyByBinarySerialization<T>(T x) {
      System.IO.MemoryStream memstream = new System.IO.MemoryStream();  //??? slow and unreliable
      System.Runtime.Serialization.IFormatter oformatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
      oformatter.Serialize(memstream, x);
      memstream.Position = 0;
      System.Runtime.Serialization.IFormatter iformatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
      T result = (T)iformatter.Deserialize(memstream);
      memstream.Dispose();
      return result;
    }

#endif

    //public static T CopyByXmlSerialization<T>(T x) {
    //  System.IO.StringWriter memwriter = new System.IO.StringWriter(System.Globalization.CultureInfo.InvariantCulture);  //??? slow and unreliable
    //  System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(typeof(T));
    //  serializer.Serialize(memwriter, x);
    //  System.IO.StringReader memreader = new System.IO.StringReader(memwriter.ToString());
    //  return (T)serializer.Deserialize(memreader);
    //}

    public static readonly IEnumerable<Object> NoObjects = new List<Object>().AsReadOnly();

    public static bool SetProperty(String pname, Object obj, Object val) {
      if (pname == null || pname.Length == 0 || obj == null) return false;
      PropertyInfo pi = obj.GetType().GetProperty(pname, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
#if !WINDOWS_UWP
            while (pi != null && !pi.CanWrite) {
        Type parenttype = pi.DeclaringType.BaseType;
        pi = parenttype.GetProperty(pname, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
      }
#endif
      if (pi != null && pi.CanWrite) {
        try {
          pi.SetValue(obj, val, null);
          return true;
        } catch (Exception) {
        }
      }
#if !WINDOWS_UWP  // PropertyDescriptor
      ICustomTypeDescriptor custom = obj as ICustomTypeDescriptor;
      if (custom != null) {
        foreach (PropertyDescriptor desc in custom.GetProperties()) {
          if (desc.Name == pname) {
            desc.SetValue(custom, val);
            return true;
          }
        }
      }
#endif
            return false;
    }


    private static bool ComputeStringKey(String key, out String prefix, out int suffix) {
      try {
        int len = key.Length-1;
        int i = len;
        while (i >= 0 && Char.IsDigit(key[i])) i--;
        if (i < len) {
          prefix = key.Substring(0, i+1);
          String digits = key.Substring(i+1);
          suffix = Int32.Parse(digits, System.Globalization.NumberFormatInfo.InvariantInfo);
          return false;
        }
      } catch (Exception) {
      }
      prefix = key;
      suffix = -1;
      return true;
    }

    public static bool MakeNodeKeyUnique<K,V>(Type valtype, Object key, String nodekeypath, Object nodedata, Dictionary<K,V> dictionary, String separator, int start) {
      if (key == null) {
        if (valtype.IsAssignableFrom(typeof(String))) {
          key = "";
        } else if (valtype.IsAssignableFrom(typeof(int))) {
          key = 0;
        } else if (valtype.IsAssignableFrom(typeof(Guid))) {
          key = Guid.Empty;
        } else {
          return false;
        }
      }
      if (key is String) {
        String k = (String)key;
        String prefix;
        int suffix;
        if (ModelHelper.ComputeStringKey(k, out prefix, out suffix)) {
          prefix = (k == "") ? "" : k + separator;  // don't start with separator
          suffix = start;
        }
        String newk = prefix + suffix.ToString(System.Globalization.NumberFormatInfo.InvariantInfo);
        while (dictionary.ContainsKey((K)((Object)newk))) {
          suffix++;
          newk = prefix + suffix.ToString(System.Globalization.NumberFormatInfo.InvariantInfo);
        }
        ModelHelper.SetProperty(nodekeypath, nodedata, newk);
        return true;
      } else if (key is int) {
        int i = (int)((Object)key);
        i++;
        while (dictionary.ContainsKey((K)((Object)i))) {
          i++;
        }
        ModelHelper.SetProperty(nodekeypath, nodedata, i);
        return true;
      } else if (key is Guid) {
        Guid newg = Guid.NewGuid();
        // don't need to check dictionary to see if it's unique -- assume it is!
        ModelHelper.SetProperty(nodekeypath, nodedata, newg);
        return true;
      }
      //??? handle other node key data types
      return false;
    }

  }


  // A convenient way to get the value of a property path for a given source object
  internal sealed class PropPathInfo<K>
#if !WINDOWS_UWP
    : FrameworkElement
#endif
  {
    public PropPathInfo() { }
    public PropPathInfo(String s) { this.Path = s; }

    // The property path, a string, that has the same syntax as used in Bindings.
    // The empty string will refer to the whole source object itself.
    public String Path {
      get { return _Path; }
      set {
        if (_Path != value /* && value != null */) {
          _Path = value;
          _IsSimpleProperty = (_Path != null && _Path.Length > 0 && _Path.IndexOfAny(complexSyntaxChars) < 0);
          _LastType = null;
          _LastPropInfo = null;
        }
      }
    }
    private String _Path;
    private bool _IsSimpleProperty;
    private Type _LastType;
    private PropertyInfo _LastPropInfo;
#if !WINDOWS_UWP  // PropertyDescriptor
    private PropertyDescriptor _LastPropDesc;
#endif
        private static readonly char[] complexSyntaxChars = new char[] { '.', '(', ')', '[', ']', ',', ':' };


    // Evaluate the property path (this.Path) on the given object.
    // When the Path is an empty string, this will just return the source object.
    // A null Path value just returns the default value for the type K.
    // If the source is null, this just returns the default value for the type K.
    public K EvalFor(Object source) {
      if (source == null) return default(K);
      String path = this.Path;
      if (path == null) return default(K);
      if (path.Length == 0) {
        K result = default(K);
        try {
          result = (K)source;
        } catch (Exception ex) {
          ModelHelper.Trace("Could not convert data: " + source.ToString() + " to Type: " + typeof(K).Name + ";\n  Perhaps the property path should not be an empty string?\n  Exception: " + ex.ToString());
        }
        return result;
      }
      if (_IsSimpleProperty) {
        PropertyInfo fni = null;
#if !WINDOWS_UWP  // PropertyDescriptor
        PropertyDescriptor desc = null;
#endif
                lock (this) {
          Type stype = source.GetType();
          if (stype != _LastType) {
            _LastType = stype;
            _LastPropInfo = stype.GetProperty(path, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
#if !WINDOWS_UWP  // PropertyDescriptor
            ICustomTypeDescriptor custom = source as ICustomTypeDescriptor;
            if (custom != null) {
              foreach (PropertyDescriptor d in custom.GetProperties()) {
                if (d.Name == path) {
                  _LastPropDesc = d;
                  break;
                }
              }
            }
#endif
                    }
                    fni = _LastPropInfo;
#if !WINDOWS_UWP  // PropertyDescriptor
          desc = _LastPropDesc;
#endif
                }
                if (fni != null) {
          return (K)fni.GetValue(source, null);
        }
#if !WINDOWS_UWP  // PropertyDescriptor
        else if (desc != null) {
          Object val = desc.GetValue(source);
          if (val == DBNull.Value)
            return default(K);
          else
            return (K)val;
        }
#endif
            }
#if !WINDOWS_UWP
            Binding binding = new Binding(path);
      binding.Source = source;
      SetBinding(DummyProperty, binding);
      K v = (K)GetValue(DummyProperty);
      ClearValue(DummyProperty);
      return v;
#else
            return default(K);
#endif
    }

    public void SetFor(Object source, K newval) {
      if (source == null) return;
      String path = this.Path;
      if (path == null) return;
      if (path.Length == 0) return;
      if (_IsSimpleProperty) {
        PropertyInfo fni = null;
#if !WINDOWS_UWP  // PropertyDescriptor
        PropertyDescriptor desc = null;
#endif
                lock (this) {
          Type stype = source.GetType();
          if (stype != _LastType) {
            _LastType = stype;
            _LastPropInfo = stype.GetProperty(path, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
#if !WINDOWS_UWP  // PropertyDescriptor
            ICustomTypeDescriptor custom = source as ICustomTypeDescriptor;
            if (custom != null) {
              foreach (PropertyDescriptor d in custom.GetProperties()) {
                if (d.Name == path) {
                  _LastPropDesc = d;
                  break;
                }
              }
            }
#endif
                    }
                    fni = _LastPropInfo;
#if !WINDOWS_UWP  // PropertyDescriptor
          desc = _LastPropDesc;
#endif
                }
                if (fni != null) {
          fni.SetValue(source, newval, null);
          return;
        }
#if !WINDOWS_UWP  // PropertyDescriptor
        else if (desc != null) {
          desc.SetValue(source, newval);
          return;
        }
#endif
            }
#if !WINDOWS_UWP
            Binding binding = new Binding(path);
      binding.Source = source;
      SetBinding(DummyProperty, binding);
      SetValue(DummyProperty, newval);
      ClearValue(DummyProperty);
#endif
        }

#if !WINDOWS_UWP
        private static readonly DependencyProperty DummyProperty = DependencyProperty.Register("Dummy", typeof(K), typeof(PropPathInfo<K>), new FrameworkPropertyMetadata(default(K)));
#endif
    }  // end of internal PropPathInfo class


#if WINDOWS_UWP  // Serialization attributes

    [System.Diagnostics.Conditional("UNUSED")]
    internal class SerializableAttribute : Attribute { }

    [System.Diagnostics.Conditional("UNUSED")]
    internal class NonSerializedAttribute : Attribute { }

#endif


#if WINDOWS_UWP

    internal static class SR {
    public static String Get(String s) { return s; }
    public static String Get(String s, object x) { return s; }
  }

  public enum NotifyCollectionChangedAction {
    Add,
    Remove,
    Replace,
    Move,
    Reset
  }

  public class NotifyCollectionChangedEventArgs : EventArgs {
    // Fields
    private NotifyCollectionChangedAction _action;
    private IList _newItems;
    private int _newStartingIndex;
    private IList _oldItems;
    private int _oldStartingIndex;

    // Methods
    public NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction action) {
      this._newStartingIndex = -1;
      this._oldStartingIndex = -1;
      if (action != NotifyCollectionChangedAction.Reset) {
        throw new ArgumentException(SR.Get("WrongActionForCtor", new object[] { NotifyCollectionChangedAction.Reset }), "action");
      }
      this.InitializeAdd(action, null, -1);
    }

    public NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction action, IList changedItems) {
      this._newStartingIndex = -1;
      this._oldStartingIndex = -1;
      if (((action != NotifyCollectionChangedAction.Add) && (action != NotifyCollectionChangedAction.Remove)) && (action != NotifyCollectionChangedAction.Reset)) {
        throw new ArgumentException(SR.Get("MustBeResetAddOrRemoveActionForCtor"), "action");
      }
      if (action == NotifyCollectionChangedAction.Reset) {
        if (changedItems != null) {
          throw new ArgumentException(SR.Get("ResetActionRequiresNullItem"), "action");
        }
        this.InitializeAdd(action, null, -1);
      } else {
        if (changedItems == null) {
          throw new ArgumentNullException("changedItems");
        }
        this.InitializeAddOrRemove(action, changedItems, -1);
      }
    }

    public NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction action, object changedItem) {
      this._newStartingIndex = -1;
      this._oldStartingIndex = -1;
      if (((action != NotifyCollectionChangedAction.Add) && (action != NotifyCollectionChangedAction.Remove)) && (action != NotifyCollectionChangedAction.Reset)) {
        throw new ArgumentException(SR.Get("MustBeResetAddOrRemoveActionForCtor"), "action");
      }
      if (action == NotifyCollectionChangedAction.Reset) {
        if (changedItem != null) {
          throw new ArgumentException(SR.Get("ResetActionRequiresNullItem"), "action");
        }
        this.InitializeAdd(action, null, -1);
      } else {
        this.InitializeAddOrRemove(action, new object[] { changedItem }, -1);
      }
    }

    public NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction action, IList newItems, IList oldItems) {
      this._newStartingIndex = -1;
      this._oldStartingIndex = -1;
      if (action != NotifyCollectionChangedAction.Replace) {
        throw new ArgumentException(SR.Get("WrongActionForCtor", new object[] { NotifyCollectionChangedAction.Replace }), "action");
      }
      if (newItems == null) {
        throw new ArgumentNullException("newItems");
      }
      if (oldItems == null) {
        throw new ArgumentNullException("oldItems");
      }
      this.InitializeMoveOrReplace(action, newItems, oldItems, -1, -1);
    }

    public NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction action, IList changedItems, int startingIndex) {
      this._newStartingIndex = -1;
      this._oldStartingIndex = -1;
      if (((action != NotifyCollectionChangedAction.Add) && (action != NotifyCollectionChangedAction.Remove)) && (action != NotifyCollectionChangedAction.Reset)) {
        throw new ArgumentException(SR.Get("MustBeResetAddOrRemoveActionForCtor"), "action");
      }
      if (action == NotifyCollectionChangedAction.Reset) {
        if (changedItems != null) {
          throw new ArgumentException(SR.Get("ResetActionRequiresNullItem"), "action");
        }
        if (startingIndex != -1) {
          throw new ArgumentException(SR.Get("ResetActionRequiresIndexMinus1"), "action");
        }
        this.InitializeAdd(action, null, -1);
      } else {
        if (changedItems == null) {
          throw new ArgumentNullException("changedItems");
        }
        if (startingIndex < -1) {
          throw new ArgumentException(SR.Get("IndexCannotBeNegative"), "startingIndex");
        }
        this.InitializeAddOrRemove(action, changedItems, startingIndex);
      }
    }

    public NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction action, object changedItem, int index) {
      this._newStartingIndex = -1;
      this._oldStartingIndex = -1;
      if (((action != NotifyCollectionChangedAction.Add) && (action != NotifyCollectionChangedAction.Remove)) && (action != NotifyCollectionChangedAction.Reset)) {
        throw new ArgumentException(SR.Get("MustBeResetAddOrRemoveActionForCtor"), "action");
      }
      if (action == NotifyCollectionChangedAction.Reset) {
        if (changedItem != null) {
          throw new ArgumentException(SR.Get("ResetActionRequiresNullItem"), "action");
        }
        if (index != -1) {
          throw new ArgumentException(SR.Get("ResetActionRequiresIndexMinus1"), "action");
        }
        this.InitializeAdd(action, null, -1);
      } else {
        this.InitializeAddOrRemove(action, new object[] { changedItem }, index);
      }
    }

    public NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction action, object newItem, object oldItem) {
      this._newStartingIndex = -1;
      this._oldStartingIndex = -1;
      if (action != NotifyCollectionChangedAction.Replace) {
        throw new ArgumentException(SR.Get("WrongActionForCtor", new object[] { NotifyCollectionChangedAction.Replace }), "action");
      }
      this.InitializeMoveOrReplace(action, new object[] { newItem }, new object[] { oldItem }, -1, -1);
    }

    public NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction action, IList newItems, IList oldItems, int startingIndex) {
      this._newStartingIndex = -1;
      this._oldStartingIndex = -1;
      if (action != NotifyCollectionChangedAction.Replace) {
        throw new ArgumentException(SR.Get("WrongActionForCtor", new object[] { NotifyCollectionChangedAction.Replace }), "action");
      }
      if (newItems == null) {
        throw new ArgumentNullException("newItems");
      }
      if (oldItems == null) {
        throw new ArgumentNullException("oldItems");
      }
      this.InitializeMoveOrReplace(action, newItems, oldItems, startingIndex, startingIndex);
    }

    public NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction action, IList changedItems, int index, int oldIndex) {
      this._newStartingIndex = -1;
      this._oldStartingIndex = -1;
      if (action != NotifyCollectionChangedAction.Move) {
        throw new ArgumentException(SR.Get("WrongActionForCtor", new object[] { NotifyCollectionChangedAction.Move }), "action");
      }
      if (index < 0) {
        throw new ArgumentException(SR.Get("IndexCannotBeNegative"), "index");
      }
      this.InitializeMoveOrReplace(action, changedItems, changedItems, index, oldIndex);
    }

    public NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction action, object changedItem, int index, int oldIndex) {
      this._newStartingIndex = -1;
      this._oldStartingIndex = -1;
      if (action != NotifyCollectionChangedAction.Move) {
        throw new ArgumentException(SR.Get("WrongActionForCtor", new object[] { NotifyCollectionChangedAction.Move }), "action");
      }
      if (index < 0) {
        throw new ArgumentException(SR.Get("IndexCannotBeNegative"), "index");
      }
      object[] newItems = new object[] { changedItem };
      this.InitializeMoveOrReplace(action, newItems, newItems, index, oldIndex);
    }

    public NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction action, object newItem, object oldItem, int index) {
      this._newStartingIndex = -1;
      this._oldStartingIndex = -1;
      if (action != NotifyCollectionChangedAction.Replace) {
        throw new ArgumentException(SR.Get("WrongActionForCtor", new object[] { NotifyCollectionChangedAction.Replace }), "action");
      }
      this.InitializeMoveOrReplace(action, new object[] { newItem }, new object[] { oldItem }, index, index);
    }

    private void InitializeAdd(NotifyCollectionChangedAction action, IList newItems, int newStartingIndex) {
      this._action = action;
      this._newItems = (newItems == null) ? null : ArrayList.ReadOnly(newItems);
      this._newStartingIndex = newStartingIndex;
    }

    private void InitializeAddOrRemove(NotifyCollectionChangedAction action, IList changedItems, int startingIndex) {
      if (action == NotifyCollectionChangedAction.Add) {
        this.InitializeAdd(action, changedItems, startingIndex);
      } else if (action == NotifyCollectionChangedAction.Remove) {
        this.InitializeRemove(action, changedItems, startingIndex);
      } else {
        //?? Invariant.Assert(false, "Unsupported action: {0}", action.ToString());
      }
    }

    private void InitializeMoveOrReplace(NotifyCollectionChangedAction action, IList newItems, IList oldItems, int startingIndex, int oldStartingIndex) {
      this.InitializeAdd(action, newItems, startingIndex);
      this.InitializeRemove(action, oldItems, oldStartingIndex);
    }

    private void InitializeRemove(NotifyCollectionChangedAction action, IList oldItems, int oldStartingIndex) {
      this._action = action;
      this._oldItems = (oldItems == null) ? null : ArrayList.ReadOnly(oldItems);
      this._oldStartingIndex = oldStartingIndex;
    }

    // Properties
    public NotifyCollectionChangedAction Action {
      get {
        return this._action;
      }
    }

    public IList NewItems {
      get {
        return this._newItems;
      }
    }

    public int NewStartingIndex {
      get {
        return this._newStartingIndex;
      }
    }

    public IList OldItems {
      get {
        return this._oldItems;
      }
    }

    public int OldStartingIndex {
      get {
        return this._oldStartingIndex;
      }
    }
  }


  public delegate void NotifyCollectionChangedEventHandler(Object sender, NotifyCollectionChangedEventArgs e);

  public interface INotifyCollectionChanged {
    event NotifyCollectionChangedEventHandler CollectionChanged;
  }

  [Serializable]
  public class ObservableCollection<T> : Collection<T>, INotifyCollectionChanged, INotifyPropertyChanged {
    // Fields
    private SimpleMonitor<T> _monitor;
    private const string CountString = "Count";
    private const string IndexerName = "Item[]";

    // Events
    [field: NonSerialized]
    public event NotifyCollectionChangedEventHandler CollectionChanged;

    [field: NonSerialized]
    protected event PropertyChangedEventHandler PropertyChanged;

    event PropertyChangedEventHandler INotifyPropertyChanged.PropertyChanged {
      add { this.PropertyChanged += value; }
      remove { this.PropertyChanged -= value; }
    }

    // Methods
    public ObservableCollection() {
      this._monitor = new SimpleMonitor<T>();
    }

    public ObservableCollection(IEnumerable<T> collection) {
      this._monitor = new SimpleMonitor<T>();
      if (collection == null) {
        throw new ArgumentNullException("collection");
      }
      this.CopyFrom(collection);
    }

    public ObservableCollection(List<T> list)
      : base((list != null) ? new List<T>(list.Count) : list) {
      this._monitor = new SimpleMonitor<T>();
      this.CopyFrom(list);
    }

    protected IDisposable BlockReentrancy() {
      this._monitor.Enter();
      return this._monitor;
    }

    protected void CheckReentrancy() {
      if ((this._monitor.Busy && (this.CollectionChanged != null)) && (this.CollectionChanged.GetInvocationList().Length > 1)) {
        throw new InvalidOperationException(SR.Get("ObservableCollectionReentrancyNotAllowed"));
      }
    }

    protected override void ClearItems() {
      this.CheckReentrancy();
      base.ClearItems();
      this.OnPropertyChanged(CountString);
      this.OnPropertyChanged(IndexerName);
      this.OnCollectionReset();
    }

    private void CopyFrom(IEnumerable<T> collection) {
      IList<T> items = base.Items;
      if ((collection != null) && (items != null)) {
        using (IEnumerator<T> enumerator = collection.GetEnumerator()) {
          while (enumerator.MoveNext()) {
            items.Add(enumerator.Current);
          }
        }
      }
    }

    protected override void InsertItem(int index, T item) {
      this.CheckReentrancy();
      base.InsertItem(index, item);
      this.OnPropertyChanged(CountString);
      this.OnPropertyChanged(IndexerName);
      this.OnCollectionChanged(NotifyCollectionChangedAction.Add, item, index);
    }

    public void Move(int oldIndex, int newIndex) {
      this.MoveItem(oldIndex, newIndex);
    }

    protected virtual void MoveItem(int oldIndex, int newIndex) {
      this.CheckReentrancy();
      T item = base[oldIndex];
      base.RemoveItem(oldIndex);
      base.InsertItem(newIndex, item);
      this.OnPropertyChanged(IndexerName);
      this.OnCollectionChanged(NotifyCollectionChangedAction.Move, item, newIndex, oldIndex);
    }

    protected virtual void OnCollectionChanged(NotifyCollectionChangedEventArgs e) {
      if (this.CollectionChanged != null) {
        using (this.BlockReentrancy()) {
          this.CollectionChanged(this, e);
        }
      }
    }

    private void OnCollectionChanged(NotifyCollectionChangedAction action, object item, int index) {
      this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(action, item, index));
    }

    private void OnCollectionChanged(NotifyCollectionChangedAction action, object item, int index, int oldIndex) {
      this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(action, item, index, oldIndex));
    }

    private void OnCollectionChanged(NotifyCollectionChangedAction action, object oldItem, object newItem, int index) {
      this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(action, newItem, oldItem, index));
    }

    private void OnCollectionReset() {
      this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
    }

    protected virtual void OnPropertyChanged(PropertyChangedEventArgs e) {
      if (this.PropertyChanged != null) {
        this.PropertyChanged(this, e);
      }
    }

    private void OnPropertyChanged(string propertyName) {
      this.OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
    }

    protected override void RemoveItem(int index) {
      this.CheckReentrancy();
      T item = base[index];
      base.RemoveItem(index);
      this.OnPropertyChanged(CountString);
      this.OnPropertyChanged(IndexerName);
      this.OnCollectionChanged(NotifyCollectionChangedAction.Remove, item, index);
    }

    protected override void SetItem(int index, T item) {
      this.CheckReentrancy();
      T oldItem = base[index];
      base.SetItem(index, item);
      this.OnPropertyChanged(IndexerName);
      this.OnCollectionChanged(NotifyCollectionChangedAction.Replace, oldItem, item, index);
    }

    // Nested Types
    [Serializable]
    private class SimpleMonitor<S> : IDisposable {
      // Fields
      private int _busyCount;

      // Methods
      public void Dispose() {
        this._busyCount--;
      }

      public void Enter() {
        this._busyCount++;
      }

      // Properties
      public bool Busy {
        get {
          return (this._busyCount > 0);
        }
      }
    }
  }
#endif  // WINFORMS compatibility
}
