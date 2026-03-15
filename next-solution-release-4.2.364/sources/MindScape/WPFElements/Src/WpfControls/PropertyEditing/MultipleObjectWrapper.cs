using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Windows;

namespace Mindscape.WpfElements.PropertyEditing
{
  /// <summary>
  /// Represents multiple objects which are to be displayed in a combined view.
  /// </summary>
  public sealed class MultipleObjectWrapper : ICustomTypeDescriptor, INotifyPropertyChanged, INotifyPropertyChanging, IDisposable, IWeakEventListener
  {
    private readonly IList _objects;

    /// <summary>
    /// Initializes a new instance of the <see cref="MultipleObjectWrapper"/> class.
    /// </summary>
    /// <param name="wrapped">The objects to be wrapped.</param>
    public MultipleObjectWrapper(params object[] wrapped)
    {
      Invariant.ArgumentNotNull(wrapped, "wrapped");

      _objects = wrapped;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MultipleObjectWrapper"/> class.
    /// </summary>
    /// <param name="wrapped">The objects to be wrapped.</param>
    public MultipleObjectWrapper(IList wrapped)
    {
      Invariant.ArgumentNotNull(wrapped, "wrapped");

      _objects = wrapped;
      HookCollectionChanged();
    }

    internal bool OwnedByGrid { get; set; }  // If the grid created it, the grid can dispose it; if the user created it, the user has to dispose it

    private void HookCollectionChanged()
    {
      INotifyCollectionChanged notifier = _objects as INotifyCollectionChanged;
      if (notifier != null)
      {
        notifier.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(HandleWrappedCollectionChanged);
      }
    }

    private void UnhookCollectionChanged()
    {
      INotifyCollectionChanged notifier = _objects as INotifyCollectionChanged;
      if (notifier != null)
      {
        notifier.CollectionChanged -= new System.Collections.Specialized.NotifyCollectionChangedEventHandler(HandleWrappedCollectionChanged);
      }
    }

    void HandleWrappedCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
      foreach (Many many in _wrappers.Values)
      {
        many.PropertyChanging -= OnManyValueChanging;
        many.DetachPropertyChangeListeners();
      }
      _wrappers.Clear();
      OnObjectCollectionChanged();
    }

    /// <summary>
    /// Gets the wrapped objects.
    /// </summary>
    public IEnumerable Objects
    {
      get { return _objects; }
    }

    /// <summary>
    /// Releases all resources used by the object.
    /// </summary>
    public void Dispose()
    {
      Dispose(true);
    }

    private void Dispose(bool disposing)
    {
      if (disposing)
      {
        UnhookCollectionChanged();
        foreach (Many many in _wrappers.Values)
        {
          many.PropertyChanging -= OnManyValueChanging;
          many.DetachPropertyChangeListeners();
        }
        _wrappers.Clear();
        _objects.Clear();
      }
    }

    #region ICustomTypeDescriptor Members

    AttributeCollection ICustomTypeDescriptor.GetAttributes()
    {
      throw new NotImplementedException();
    }

    string ICustomTypeDescriptor.GetClassName()
    {
      throw new NotImplementedException();
    }

    string ICustomTypeDescriptor.GetComponentName()
    {
      throw new NotImplementedException();
    }

    TypeConverter ICustomTypeDescriptor.GetConverter()
    {
      throw new NotImplementedException();
    }

    EventDescriptor ICustomTypeDescriptor.GetDefaultEvent()
    {
      throw new NotImplementedException();
    }

    PropertyDescriptor ICustomTypeDescriptor.GetDefaultProperty()
    {
      throw new NotImplementedException();
    }

    object ICustomTypeDescriptor.GetEditor(Type editorBaseType)
    {
      throw new NotImplementedException();
    }

    EventDescriptorCollection ICustomTypeDescriptor.GetEvents(Attribute[] attributes)
    {
      throw new NotImplementedException();
    }

    EventDescriptorCollection ICustomTypeDescriptor.GetEvents()
    {
      throw new NotImplementedException();
    }

    PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties(Attribute[] attributes)
    {
      if (_objects == null || _objects.Count == 0)
      {
        return PropertyDescriptorCollection.Empty;
      }

      List<PropertyDescriptorCollection> pdcs = new List<PropertyDescriptorCollection>();
      foreach (object obj in _objects)
      {
        pdcs.Add(ReflectionUtilities.GetProperties(obj, attributes));
      }

      return CommonSubset(pdcs);
    }

    PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties()
    {
      if (_objects == null || _objects.Count == 0)
      {
        return PropertyDescriptorCollection.Empty;
      }

      List<PropertyDescriptorCollection> pdcs = new List<PropertyDescriptorCollection>();
      foreach (object obj in _objects)
      {
        pdcs.Add(ReflectionUtilities.GetProperties(obj));
      }

      return CommonSubset(pdcs);
    }

    private static PropertyDescriptorCollection CommonSubset(List<PropertyDescriptorCollection> pdcs)
    {
      List<PropertyDescriptor> pds = new List<PropertyDescriptor>();

      PropertyDescriptorCollection pdcPrimary = pdcs[0];

      foreach (PropertyDescriptor pd in pdcPrimary)
      {
        Type propertyType = pd.PropertyType;

        bool hasMatchingPropertiesInAllCollections = (pdcs.Count <= 1 || IsMergeable(pd));
        bool isReadOnlyInAnyCollection = pd.IsReadOnly;
        bool isBrowsableInAllCollections = pd.IsBrowsable;

        foreach (PropertyDescriptorCollection pdc in pdcs)
        {
          if (pdc != pdcPrimary)
          {
            PropertyDescriptor pdOther = ReflectionUtilities.Find(pdc, pd);
            if (pdOther == null
              || pdOther.PropertyType != propertyType
              || (pdcs.Count > 1 && !IsMergeable(pdOther)))
            {
              hasMatchingPropertiesInAllCollections = false;
              break;
            }

            if (pdOther.IsReadOnly)
            {
              isReadOnlyInAnyCollection = true;
            }
            if (!pdOther.IsBrowsable)
            {
              isBrowsableInAllCollections = false;
            }
          }
        }

        if (hasMatchingPropertiesInAllCollections)
        {
          MultipleObjectPropertyDescriptor mopd = new MultipleObjectPropertyDescriptor(pd, isReadOnlyInAnyCollection, isBrowsableInAllCollections);
          pds.Add(mopd);
        }
      }

      return new PropertyDescriptorCollection(pds.ToArray(), true);
    }

    private static bool IsMergeable(PropertyDescriptor pd)
    {
      MergablePropertyAttribute attr = (MergablePropertyAttribute)(pd.Attributes[typeof(MergablePropertyAttribute)]);
      return attr.AllowMerge;
    }

    object ICustomTypeDescriptor.GetPropertyOwner(PropertyDescriptor pd)
    {
      return this;
    }

    #endregion

    private IDictionary<MultipleObjectPropertyDescriptor, Many> _wrappers = new Dictionary<MultipleObjectPropertyDescriptor, Many>();

    internal object GetValue(MultipleObjectPropertyDescriptor property)
    {
      Many value;
      if (!_wrappers.TryGetValue(property, out value))
      {
        List<PropertyWrapper> pws = new List<PropertyWrapper>();

        Type type = property.UnderlyingProperty.PropertyType;

        foreach (object obj in _objects)
        {
          PropertyDescriptor descriptor = ReflectionUtilities.Find(ReflectionUtilities.GetProperties(obj), property.UnderlyingProperty);
          if (descriptor != null && type.IsAssignableFrom(descriptor.PropertyType))
          {
            PropertyWrapper pw = new PropertyWrapper(obj, descriptor);
            pws.Add(pw);
          }
        }

        value = Many.GetMany(type, pws, property);
        _wrappers[property] = value;

        PropertyChangedEventManager.AddListener(value, this, "Value");
        PropertyChangedEventManager.AddListener(value, this, "IsConsistent");
        value.PropertyChanging += OnManyValueChanging;  // TODO: turn this into a weak event if it still causes problems
      }
      return value;
    }

    internal void SetValue(MultipleObjectPropertyDescriptor property, object value)
    {
      Many many = (Many)GetValue(property);
      many.SetValue(value);
    }

    private void OnManyValueChanging(object sender, PropertyChangingEventArgs e)
    {
      Many many = (Many)sender;
      if (e.PropertyName == "Value" || e.PropertyName == "IsConsistent")
      {
        OnPropertyChanging(many.Descriptor.Name);
      }
    }

    /// <summary>
    /// Occurs when a property value changes.
    /// </summary>
    public event PropertyChangedEventHandler PropertyChanged;

    private void OnPropertyChanged(string propertyName)
    {
      if (PropertyChanged != null)
      {
        PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
      }
    }

    /// <summary>
    /// Occurs when a property value is changing.
    /// </summary>
    public event PropertyChangingEventHandler PropertyChanging;

    private void OnPropertyChanging(string propertyName)
    {
      if (PropertyChanging != null)
      {
        PropertyChanging(this, new PropertyChangingEventArgs(propertyName));
      }
    }

    internal event EventHandler ObjectCollectionChanged;

    private void OnObjectCollectionChanged()
    {
      if (ObjectCollectionChanged != null)
      {
        ObjectCollectionChanged(this, EventArgs.Empty);
      }
    }

    #region IWeakEventListener Members

    bool IWeakEventListener.ReceiveWeakEvent(Type managerType, object sender, EventArgs e)
    {
      PropertyChangedEventArgs pcea = (PropertyChangedEventArgs)e;
      if (pcea.PropertyName == "Value" || pcea.PropertyName == "IsConsistent")
      {
        Many many = (Many)sender;
        OnPropertyChanged(many.Descriptor.Name);
        return true;
      }
      return false;
    }

    #endregion
  }
}
