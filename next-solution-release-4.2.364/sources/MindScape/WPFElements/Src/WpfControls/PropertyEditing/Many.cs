using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Collections;
using System.Diagnostics;
using System.Windows.Data;
using System.Reflection;
using System.Windows.Input;
using System.Windows.Media;
using System.Collections.Specialized;

[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Scope = "member", Target = "Mindscape.WpfElements.WpfPropertyGrid.Many.#System.ComponentModel.ICustomTypeDescriptor.GetAttributes()")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Scope = "member", Target = "Mindscape.WpfElements.WpfPropertyGrid.Many.#System.ComponentModel.ICustomTypeDescriptor.GetClassName()")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Scope = "member", Target = "Mindscape.WpfElements.WpfPropertyGrid.Many.#System.ComponentModel.ICustomTypeDescriptor.GetComponentName()")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Scope = "member", Target = "Mindscape.WpfElements.WpfPropertyGrid.Many.#System.ComponentModel.ICustomTypeDescriptor.GetConverter()")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Scope = "member", Target = "Mindscape.WpfElements.WpfPropertyGrid.Many.#System.ComponentModel.ICustomTypeDescriptor.GetDefaultEvent()")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Scope = "member", Target = "Mindscape.WpfElements.WpfPropertyGrid.Many.#System.ComponentModel.ICustomTypeDescriptor.GetDefaultProperty()")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Scope = "member", Target = "Mindscape.WpfElements.WpfPropertyGrid.Many.#System.ComponentModel.ICustomTypeDescriptor.GetEditor(System.Type)")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Scope = "member", Target = "Mindscape.WpfElements.WpfPropertyGrid.Many.#System.ComponentModel.ICustomTypeDescriptor.GetEvents()")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Scope = "member", Target = "Mindscape.WpfElements.WpfPropertyGrid.Many.#System.ComponentModel.ICustomTypeDescriptor.GetEvents(System.Attribute[])")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Scope = "member", Target = "Mindscape.WpfElements.WpfPropertyGrid.Many.#System.ComponentModel.ICustomTypeDescriptor.GetProperties()")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Scope = "member", Target = "Mindscape.WpfElements.WpfPropertyGrid.Many.#System.ComponentModel.ICustomTypeDescriptor.GetProperties(System.Attribute[])")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Scope = "member", Target = "Mindscape.WpfElements.WpfPropertyGrid.Many.#System.ComponentModel.ICustomTypeDescriptor.GetPropertyOwner(System.ComponentModel.PropertyDescriptor)")]

namespace Mindscape.WpfElements.PropertyEditing
{
  /// <summary>
  /// Represents a property that may have different values across the selected objects.
  /// </summary>
  public abstract class Many : INotifyPropertyChanged, INotifyPropertyChanging, ICustomTypeDescriptor
  {
    private IList<PropertyWrapper> _properties;
    private MultipleObjectPropertyDescriptor _descriptor;

    private static Many GetMany(Type type)
    {
      if (typeof(INotifyCollectionChanged).IsAssignableFrom(type))
      {
        return (Many)(Activator.CreateInstance(typeof(CollectionMany<>).MakeGenericType(type)));
      }
      return (Many)(Activator.CreateInstance(typeof(Many<>).MakeGenericType(type)));
    }

    internal static Many GetMany(Type type, IList<PropertyWrapper> properties, MultipleObjectPropertyDescriptor descriptor)
    {
      Many many = GetMany(type);
      many._properties = properties;
      many._descriptor = descriptor;

      many.AttachPropertyChangeListeners();

      return many;
    }

    /// <summary>
    /// Gets the (short) name of the data type of the property.
    /// </summary>
    /// <remarks>This property is equivalent to <see cref="PropertyType"/>.Name and is provided
    /// as a helper for partial trust scenarios.</remarks>
    public string PropertyTypeName
    {
      get { return PropertyType.Name; }
    }

    /// <summary>
    /// Gets the data type of the property.
    /// </summary>
    public abstract Type PropertyType { get; }

    /// <summary>
    /// Gets the values across the selected objects.
    /// </summary>
    public IEnumerable Values
    {
      get
      {
        foreach (PropertyWrapper pw in _properties)
        {
          yield return pw.Value;
        }
      }
    }

    /// <summary>
    /// Gets the name of the property that the values came from.
    /// </summary>
    public string PropertyName
    {
      get { return _descriptor == null ? null : _descriptor.Name; }
    }

    public IList<PropertyWrapper> Properties
    {
      get { return _properties; }
    }

    internal MultipleObjectPropertyDescriptor Descriptor
    {
      get { return _descriptor; }
    }

    internal PropertyDescriptor ValuePropertyDescriptor
    {
      get { return new ManyValuePropertyDescriptor(GetMergedAttributes(), PropertyType, IsReadOnly, _descriptor.UnderlyingProperty); }
    }

    private Attribute[] GetMergedAttributes()
    {
      AttributeCollection attrs = GetMergedAttributesCollection();
      Attribute[] result = new Attribute[attrs.Count];
      attrs.CopyTo(result, 0);
      return result;
    }

    private AttributeCollection GetMergedAttributesCollection()
    {
      if (AllPropertyInstancesFromSameProperty)
      {
        return _properties[0].PropertyDescriptor.Attributes;
      }

      List<Attribute> attrs = new List<Attribute>();
      AttributeCollection attrsBase = _properties[0].PropertyDescriptor.Attributes;
      foreach (Attribute attr in attrsBase)
      {
        bool isSameOnAllObjects = true;
        foreach (PropertyWrapper pw in _properties)
        {
          if (!pw.PropertyDescriptor.Attributes.Contains(attr))
          {
            isSameOnAllObjects = false;
            break;
          }
        }
        if (isSameOnAllObjects)
        {
          attrs.Add(attr);
        }
      }
      return new AttributeCollection(attrs.ToArray());
    }

    internal bool AllPropertyInstancesFromSameProperty
    {
      get
      {
        if (_properties.Count == 0)
        {
          return false;
        }

        PropertyDescriptor metadata = _properties[0].PropertyDescriptor;
        foreach (PropertyWrapper pw in _properties)
        {
          if (pw.PropertyDescriptor != metadata)
          {
            return false;
          }
        }

        return true;
      }
    }

    /// <summary>
    /// Gets the value (if available).  This value is meaningful only if
    /// <see cref="IsConsistent"/> is true.
    /// </summary>
    public abstract object RawValue { get; }

    /// <summary>
    /// Gets a value indicating whether the <see cref="Many"/> contains
    /// consistent values.
    /// </summary>
    public abstract bool IsConsistent { get; }

    /// <summary>
    /// Gets Type for consistent <see cref="Many"/> objects.
    /// </summary>
    public abstract Type ConsistentType { get; }

    /// <summary>
    /// Resets the property to a default value across all objects.
    /// </summary>
    public abstract void Reset();

    /// <summary>
    /// Gets whether the property value is read-only.
    /// </summary>
    public bool IsReadOnly
    {
      get { return _descriptor.IsReadOnly; }
    }

    /// <summary>
    /// Occurs when a property value changes.
    /// </summary>
    public event PropertyChangedEventHandler PropertyChanged;

    /// <summary>
    /// Raises the <see cref="PropertyChanged"/> event.
    /// </summary>
    /// <param name="propertyName">The property which is changing.</param>
    protected virtual void OnPropertyChanged(string propertyName)
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

    /// <summary>
    /// Raises the <see cref="PropertyChanging"/> event.
    /// </summary>
    /// <param name="propertyName">The property which is changing.</param>
    protected void OnPropertyChanging(string propertyName)
    {
      if (PropertyChanging != null)
      {
        PropertyChanging(this, new PropertyChangingEventArgs(propertyName));
      }
    }

    internal abstract void AttachPropertyChangeListeners();
    internal abstract void DetachPropertyChangeListeners();

    internal abstract void SetValue(object value);

    #region ICustomTypeDescriptor Members

    AttributeCollection ICustomTypeDescriptor.GetAttributes()
    {
      return TypeDescriptor.GetAttributes(PropertyType);
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
      return TypeDescriptor.GetConverter(PropertyType);
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
      return TypeDescriptor.GetProperties(PropertyType, attributes);
    }

    PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties()
    {
      return TypeDescriptor.GetProperties(PropertyType);
    }

    object ICustomTypeDescriptor.GetPropertyOwner(PropertyDescriptor pd)
    {
      return this; // _properties[0].Value;  // nominal value; needs to be implemented, but has no effect
    }

    #endregion

    private class ManyValuePropertyDescriptor : PropertyDescriptor, IPropertyDescriptorWrapper
    {
      private readonly Type _propertyType;
      private readonly bool _isReadOnly;
      private readonly PropertyDescriptor _underlyingDescriptor;

      public ManyValuePropertyDescriptor(Attribute[] attrs, Type propertyType, bool isReadOnly, PropertyDescriptor underlyingDescriptor)
        : base("Value", attrs)
      {
        _propertyType = propertyType;
        _isReadOnly = isReadOnly;
        _underlyingDescriptor = underlyingDescriptor;
      }

      public override bool CanResetValue(object component)
      {
        return false;
      }

      public override Type ComponentType
      {
        get { return typeof(Many); }
      }

      public override object GetValue(object component)
      {
        return ((Many)component).RawValue;
      }

      public override bool IsReadOnly
      {
        get { return _isReadOnly; }
      }

      public override Type PropertyType
      {
        get { return _propertyType; }
      }

      public override void ResetValue(object component)
      {
        throw new NotImplementedException();
      }

      public override void SetValue(object component, object value)
      {
        ((Many)component).SetValue(value);
      }

      public override bool ShouldSerializeValue(object component)
      {
        throw new NotImplementedException();
      }

      public override TypeConverter Converter
      {
        get
        {
          return _underlyingDescriptor.Converter;
        }
      }

      #region IPropertyDescriptorWrapper Members

      PropertyDescriptor IPropertyDescriptorWrapper.PropertyDescriptor
      {
        get { return _underlyingDescriptor; }
      }

      #endregion
    }
  }

  /// <summary>
  /// Represents a property that may have different values across the selected objects.
  /// </summary>
  /// <typeparam name="T">The static type of the property.</typeparam>
  public class Many<T> : Many
  {
    private bool _inValueChange = false;

    private void BeginValueChange()
    {
      _inValueChange = true;
    }

    private void EndValueChange()
    {
      _inValueChange = false;
    }

    private void WrappedObjectPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      if (!_inValueChange && e.PropertyName == Descriptor.Name)
      {
        // If we're in the midst of set_Value, it will take care of raising
        // the required PropertyChanged events when it's finished setting
        // everything.

        /*
        OnPropertyChanged("IsConsistent");
        OnPropertyChanged("Value");
        OnPropertyChanged("RawValue");
        */
      }
    }

    /// <summary>
    /// Gets the value (if available).  This value is meaningful only if
    /// <see cref="IsConsistent"/> is true.
    /// </summary>
    public override object RawValue
    {
      get { return Value; }
    }

    /// <summary>
    /// Resets the property to a default value across all objects.
    /// </summary>
    public override void Reset()
    {
      T value = default(T);

      if (Properties != null && Properties.Count > 0)
      {
        PropertyWrapper wrapper = Properties[0];
        if (wrapper.PropertyDescriptor.CanResetValue(wrapper.Wrapped))
        {
          wrapper.PropertyDescriptor.ResetValue(wrapper.Wrapped);
          value = (T)wrapper.Value;
          SetValue(value);
          return;
        }
      }

      Type underlyingType = Nullable.GetUnderlyingType(typeof(T));
      if (underlyingType != null && underlyingType != typeof(T))
      {
        value = (T)Activator.CreateInstance(underlyingType);
      }

      TypeConverter converter = TypeDescriptor.GetConverter(typeof(T));
      if (AllPropertyInstancesFromSameProperty)
      {
        converter = Properties[0].PropertyDescriptor.Converter;
      }
      if (converter != null && converter.GetCreateInstanceSupported())
      {
        value = (T)(converter.CreateInstance(new Hashtable()));
      }
      if (value == null)
      {
        try
        {
          value = (T)Activator.CreateInstance(typeof(T));
        }
        catch (MissingMethodException)
        {
        }
      }

      OverrideDefault(ref value);

      SetValue(value);
    }

    // Hook for cases where the CLR default is ugly or not very useful
    private static void OverrideDefault(ref T value)
    {
      if (typeof(T) == typeof(DateTime))
      {
        value = (T)(object)DateTime.Now;  // better than DateTime.MinValue
      }
      if (typeof(T) == typeof(Color))
      {
        value = (T)(object)Colors.Black;  // better than #00000000
      }
    }

    internal override void AttachPropertyChangeListeners()
    {
      bool isConsistent;
      T value;
      GetValue(out value, out isConsistent);

      if (isConsistent)
      {
        INotifyPropertyChanged notifier = value as INotifyPropertyChanged;
        if (notifier != null)
        {
          notifier.PropertyChanged += ObjectSubpropertyChanged;
        }
      }

      foreach (PropertyWrapper pw in Properties)
      {
        INotifyPropertyChanged notifier = pw.Wrapped as INotifyPropertyChanged;
        if (notifier != null)
        {
          notifier.PropertyChanged += new PropertyChangedEventHandler(WrappedObjectPropertyChanged);
        }
      }
    }

    internal override void DetachPropertyChangeListeners()
    {
      bool isConsistent;
      T value;
      GetValue(out value, out isConsistent);

      if (isConsistent)
      {
        INotifyPropertyChanged notifier = value as INotifyPropertyChanged;
        if (notifier != null)
        {
          notifier.PropertyChanged -= ObjectSubpropertyChanged;
        }
      }

      foreach (PropertyWrapper pw in Properties)
      {
        INotifyPropertyChanged notifier = pw.Wrapped as INotifyPropertyChanged;
        if (notifier != null)
        {
          notifier.PropertyChanged -= new PropertyChangedEventHandler(WrappedObjectPropertyChanged);
        }
      }
    }

    private bool _isConsistent;

    /// <summary>
    /// Gets a value indicating whether the <see cref="Many"/> contains
    /// consistent values.
    /// </summary>
    public override bool IsConsistent
    {
      get
      {
        bool isConsistent;
        T value;
        _consistentType = GetValue(out value, out isConsistent);
        _isConsistent = isConsistent;
        return isConsistent;
      }
    }

    private Type _consistentType;

    /// <summary>
    /// Gets Type for consistent <see cref="Many"/> objects.
    /// </summary>
    public override Type ConsistentType
    {
        get
        {
            return IsConsistent ? _consistentType : null;
        }
    }

    /// <summary>
    /// Gets or sets the value of the property.  Getting the value is meaningful
    /// only when <see cref="IsConsistent"/> is true.  Setting the value sets the
    /// property value on all objects in the <see cref="Many"/>.
    /// </summary>
    public T Value
    {
      get
      {
        bool isConsistent;
        T value;
        GetValue(out value, out isConsistent);
        return value;
      }
      set
      {
        bool wasConsistent;
        T oldValue;
        GetValue(out oldValue, out wasConsistent);
        wasConsistent = _isConsistent;

        bool valueChanging = (oldValue == null ? value != null : !oldValue.Equals(value));

        if (valueChanging)
        {
          OnPropertyChanging("Value");
        }

        if (wasConsistent)
        {
          INotifyPropertyChanged oldNotifier = oldValue as INotifyPropertyChanged;
          if (oldNotifier != null)
          {
            oldNotifier.PropertyChanged -= ObjectSubpropertyChanged;
          }
        }
        else
        {
          OnPropertyChanging("IsConsistent");
        }

        try
        {
          BeginValueChange();
          foreach (PropertyWrapper pw in Properties)
          {
            pw.Value = value;
          }
        }
        finally
        {
          EndValueChange();
        }

        INotifyPropertyChanged notifier = value as INotifyPropertyChanged;
        if (notifier != null)
        {
          notifier.PropertyChanged += ObjectSubpropertyChanged;
        }

        if (valueChanging)
        {
          OnPropertyChanged("Value");
          OnPropertyChanged("RawValue");
        }
        if (!wasConsistent)
        {
          OnPropertyChanged("IsConsistent");
        }
      }
    }

    private void ObjectSubpropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      OnPropertyChanged(e.PropertyName);
    }

    /// <summary>
    /// Gets the data type of the property.
    /// </summary>
    public override Type PropertyType
    {
      get { return typeof(T); }
    }

    private Type GetValue(out T value, out bool isConsistent)
    {
      isConsistent = false;

      List<T> values = new List<T>();
      foreach (PropertyWrapper pw in Properties)
      {
        values.Add((T)(pw.Value));
      }

      if (values.Count == 0)
      {
        value = default(T);
        return null;
      }

      T proposedValue = values[0];
      foreach (T otherValue in values)
      {
        if (!AreEqual(otherValue, proposedValue))
        {
          value = default(T);
          return null;
        }
      }

      isConsistent = true;
      value = proposedValue;
      Type consistentType = null;
      if (Properties != null && Properties.Count > 0)
        consistentType = Properties[0].Wrapped.GetType();
      return consistentType;
    }

    internal override void SetValue(object value)
    {
      Value = (T)value;
    }

    private static bool AreEqual(T first, T second)
    {
      if (first == null)
      {
        return second == null;
      }
      return first.Equals(second);
    }
  }

  internal class CollectionMany<T> : Many<T>, INotifyCollectionChanged
  {
    protected virtual void OnNotifyCollectionChanged(NotifyCollectionChangedEventArgs e)
    {
      NotifyCollectionChangedEventHandler handler = CollectionChanged;
      if (handler != null)
      {
        handler(this, e);
      }
    }

    public event NotifyCollectionChangedEventHandler CollectionChanged;

    internal override void SetValue(object value)
    {
      if (IsConsistent)
      {
        INotifyCollectionChanged oldNotifier = Value as INotifyCollectionChanged;
        if (oldNotifier != null)
        {
          oldNotifier.CollectionChanged -= PropagateCollectionChangeFromWrappedValue;
        }
      }
      base.SetValue(value);
      INotifyCollectionChanged newNotifier = Value as INotifyCollectionChanged;
      if (newNotifier != null)
      {
        newNotifier.CollectionChanged += PropagateCollectionChangeFromWrappedValue;
      }
    }

    private void PropagateCollectionChangeFromWrappedValue(object sender, NotifyCollectionChangedEventArgs e)
    {
      OnNotifyCollectionChanged(e);
    }
  }

  public class PropertyWrapper
  {
    private readonly object _wrapped;
    private readonly PropertyDescriptor _propertyDescriptor;

    public object Wrapped { get { return _wrapped; } }
    public PropertyDescriptor PropertyDescriptor { get { return _propertyDescriptor; } }

    public PropertyWrapper(object wrapped, PropertyDescriptor propertyDescriptor)
    {
      Invariant.ArgumentNotNull(propertyDescriptor, "propertyDescriptor");

      if (wrapped is ICustomTypeDescriptor) // TODO: write tests for this
      {
        _wrapped = (wrapped as ICustomTypeDescriptor).GetPropertyOwner(propertyDescriptor);
      }
      else
      {
        _wrapped = wrapped;
      }

      _propertyDescriptor = propertyDescriptor;
    }

    public object Value
    {
      get
      {
        object obj = Wrapped;
        if (obj is ICustomTypeDescriptor) // TODO: write tests for this
        {
          obj = (obj as ICustomTypeDescriptor).GetPropertyOwner(PropertyDescriptor);
        }
        return PropertyDescriptor.GetValue(obj);
      }
      set { PropertyDescriptor.SetValue(Wrapped, value); }
    }
  }
}
