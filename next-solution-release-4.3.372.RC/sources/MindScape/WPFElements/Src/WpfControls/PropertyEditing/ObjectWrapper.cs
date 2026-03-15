using System.ComponentModel;
using System.Reflection;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Collections;
using System.Diagnostics;
using System.Windows;

[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Scope = "member", Target = "Mindscape.WpfElements.WpfPropertyGrid.ObjectWrapper.#System.ComponentModel.IDataErrorInfo.Error")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Scope = "member", Target = "Mindscape.WpfElements.WpfPropertyGrid.ObjectWrapper.#System.ComponentModel.IDataErrorInfo.Item[System.String]")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Scope = "member", Target = "Mindscape.WpfElements.WpfPropertyGrid.ObjectWrapper.#System.Windows.IWeakEventListener.ReceiveWeakEvent(System.Type,System.Object,System.EventArgs)")]

namespace Mindscape.WpfElements.PropertyEditing
{
  /// <summary>
  /// Creates strongly-typed <see cref="ObjectWrapper"/> objects.
  /// </summary>
  public static class ObjectWrapperFactory
  {
    /// <summary>
    /// Creates a strongly-typed <see cref="ObjectWrapper"/>.
    /// </summary>
    /// <param name="property">The property whose value is to be accessed through the wrapper.</param>
    /// <param name="editable">Whether the value can be modified.</param>
    /// <returns>A wrapper which can be used to get or set the property value in a
    /// property-agnostic way.</returns>
    public static ObjectWrapper CreateWrapper(Node property, bool editable)
    {
      return CreateWrapper(property, editable, null);
    }

    /// <summary>
    /// Creates a strongly-typed <see cref="ObjectWrapper"/>.
    /// </summary>
    /// <param name="property">The property whose value is to be accessed through the wrapper.</param>
    /// <param name="editable">Whether the value can be modified.</param>
    /// <param name="editContext">Additional user-specified information to be made available to 
    /// consumers of the ObjectWrapper.</param>
    /// <returns>A wrapper which can be used to get or set the property value in a
    /// property-agnostic way.</returns>
    public static ObjectWrapper CreateWrapper(Node property, bool editable, object editContext)
    {
      Type valueType = property.PropertyType;
      if (valueType == null)
      {
        valueType = typeof(object);
      }
      Type wrapperType = typeof(ObjectWrapper<>);
      Type specificWrapperType = wrapperType.MakeGenericType(valueType);
      return Activator.CreateInstance(specificWrapperType, property, editable, editContext) as ObjectWrapper;
    }
  }

  /// <summary>
  /// A wrapper class which allows the data binding infrastructure to get or set property values
  /// without knowing the name of the property being accessed.
  /// </summary>
  /// <remarks>This class supports the WPF data binding infrastructure and is not intended for
  /// use from user code.</remarks>
  public abstract class ObjectWrapper : INotifyPropertyChanged, IDataErrorInfo, IWeakEventListener
  {
    private Node _property;
    private readonly bool _editable;
    private readonly object _editContext;

    /// <summary>
    /// Initialises a new instance of the <see cref="ObjectWrapper"/> class.
    /// </summary>
    /// <param name="property">The property whose value is to be accessed through the wrapper.</param>
    /// <param name="editable">Whether the value can be modified.</param>
    /// <param name="editContext">Additional user-specified information to be made available to
    /// consumers of the ObjectWrapper.</param>
    protected ObjectWrapper(Node property, bool editable, object editContext)
    {
      _property = property;
      _editable = editable;
      _editContext = editContext;

      HookChangeNotifications();
    }

    #region IWeakEventListener Members

    bool IWeakEventListener.ReceiveWeakEvent(Type managerType, object sender, EventArgs e)
    {
      PropertyChangedEventArgs pcea = e as PropertyChangedEventArgs;
      if (pcea != null)
      {
        if (sender is ObjectWrapper)
        {
          OnSourceReferenceChanged(sender, pcea);
        }
        else
        {          
          OnSourcePropertyChanged(sender, pcea);
        }
        return true;
      }
      return false;
    }

    #endregion

    private void HookChangeNotifications()
    {
      INotifyPropertyChanged notifier = _property.Source as INotifyPropertyChanged;
      if (notifier != null)
      {
        PropertyChangedEventManager.AddListener(notifier, this, _property.Name);
        PropertyChangedEventManager.AddListener(notifier, this, _property.Name + "[]");
        //notifier.PropertyChanged += OnSourcePropertyChanged;
      }

      if (IsMemberOfValueType && Property.SourceReference != null)
      {
        PropertyChangedEventManager.AddListener(Property.SourceReference, this, "Value");
        //Property.SourceReference.PropertyChanged += OnSourceReferenceChanged;
      }
    }

    private void OnSourcePropertyChanged(object sender, PropertyChangedEventArgs e)
    {      
      string changingProperty = e.PropertyName;
      if (changingProperty != null && changingProperty.EndsWith("[]", StringComparison.Ordinal))  // ObservableCollection sends questionable name in its PropertyChanged event
      {
        changingProperty = changingProperty.Substring(0, changingProperty.Length - 2);
      }
      if (String.IsNullOrEmpty(changingProperty) || changingProperty == _property.Name)
      {
        FireValuePropertyChanged();
      }

      if (_property != null && !(_property is CollectionElement))
      {
        _property.UpdateChildren();
      }
    }

    private void OnSourceReferenceChanged(object sender, PropertyChangedEventArgs e)
    {
      if ("Value".Equals(e.PropertyName, StringComparison.OrdinalIgnoreCase))
      {
        UnhookChangeNotifications();
        _property = _property.SourceReference.Property.FindChild(_property.Name);
        HookChangeNotifications();
        FireValuePropertyChanged();
      }
    }

    private void UnhookChangeNotifications()
    {
      INotifyPropertyChanged notifier = _property.Source as INotifyPropertyChanged;
      if (notifier != null)
      {
        PropertyChangedEventManager.RemoveListener(notifier, this, _property.Name);
        PropertyChangedEventManager.RemoveListener(notifier, this, _property.Name + "[]");
        //notifier.PropertyChanged -= OnSourcePropertyChanged;
      }

      if (IsMemberOfValueType && Property.SourceReference != null)
      {
        PropertyChangedEventManager.RemoveListener(Property.SourceReference, this, "Value");
        //Property.SourceReference.PropertyChanged -= OnSourceReferenceChanged;
      }
    }

    private void FireValuePropertyChanged()
    {
      if (PropertyChanged != null)
      {
        PropertyChanged(this, new PropertyChangedEventArgs("Value"));
      }
    }

    /// <summary>
    /// Initialises a new instance of the <see cref="ObjectWrapper"/> class.
    /// </summary>
    /// <param name="property">The property whose value is to be accessed through the wrapper.</param>
    /// <param name="editable">Whether the value can be modified.</param>
    protected ObjectWrapper(Node property, bool editable)
      : this(property, editable, null)
    {
    }

    /// <summary>
    /// Gets whether the property can be modified.
    /// </summary>
    public bool Editable
    {
      get { return _editable; }
    }

    /// <summary>
    /// Gets optional additional user-specified information specified in an editor declaration.
    /// </summary>
    public object EditContext
    {
      get { return _editContext; }
    }

    /// <summary>
    /// Gets the wrapped object.
    /// </summary>
    public object UnderlyingObject
    {
      get { return _property.Source; }
    }

    /// <summary>
    /// Gets the wrapped property.
    /// </summary>
    public string PropertyName
    {
      get { return _property.Name; }
    }

    /// <summary>
    /// Gets the wrapped property.
    /// </summary>
    public Node Property
    {
      get { return _property; }
    }

    private bool IsMemberOfValueType
    {
      get
      {
        return _property.DeclaringType.IsValueType;
      }
    }

    /// <summary>
    /// Occurs when a property value changes.
    /// </summary>
    public event PropertyChangedEventHandler PropertyChanged;

    internal object RawValue
    {
      get
      {
        object value = Property.Property.GetValue(UnderlyingObject, BindingFlags.Default, null, IndexedPropertyArguments, null);
        return value;
      }
      set
      {
        if (Object.Equals(value, RawValue))
        {
          return;
        }

        try
        {
          _error = null;
          if (IsMemberOfValueType && Property.SourceReference != null)
          {
            RecreateParentValueInstanceWithNewMemberValue(value);
          }
          else
          {
            Property.Property.SetValue(UnderlyingObject, value, BindingFlags.Default, null, IndexedPropertyArguments, null);
          }
        }
        catch (TargetInvocationException e)
        {
          // Throwing the inner exception doesn't help: data binding still ends up displaying the
          // unhelpful TargetInvocationException message.  So we'll use IDataErrorInfo instead.
          _error = e.InnerException.Message;
        }
      }
    }

    private void RecreateParentValueInstanceWithNewMemberValue(object value)
    {
      ObjectWrapper parent = Property.SourceReference;
      TypeConverter converter = TypeDescriptor.GetConverter(Property.Source);
      if (converter.GetCreateInstanceSupported())
      {
        Hashtable properties = GetCurrentPropertyDictionary();
        properties[Property.Name] = value;
        object newValue = converter.CreateInstance(properties);
        parent.RawValue = newValue;
      }
      else
      {
        object newValue = CloneCurrentValue();
        Property.Property.SetValue(newValue, value, BindingFlags.Default, null, IndexedPropertyArguments, null);
        parent.RawValue = newValue;
      }
      parent.Property.RefreshSourceReference();
      _property = parent.Property.FindChild(Property.Name);
      FireValuePropertyChanged();
    }

    private object[] IndexedPropertyArguments
    {
      get
      {
        object[] setterArgs = null;
        if (Property.IndexedPropertyArguments != null)
        {
          setterArgs = new List<object>(Property.IndexedPropertyArguments).ToArray();
        }
        return setterArgs;
      }
    }

    private object CloneCurrentValue()
    {
      object newValue = Activator.CreateInstance(Property.Source.GetType());
      foreach (PropertyDescriptor pd in TypeDescriptor.GetProperties(Property.Source))
      {
        if (!pd.IsReadOnly)
        {
          if (Property.SourceReference.RawValue is Many)
          {
            var many = Property.SourceReference.RawValue as Many;
            foreach (var v in many.Values)
            {
              object existingValue = pd.GetValue(v);
              pd.SetValue(newValue, existingValue);
              break;
            }
          }
          else
          {
            object existingValue = pd.GetValue(Property.SourceReference.RawValue);
            pd.SetValue(newValue, existingValue);
          }
        }
      }
      return newValue;
    }

    private Hashtable GetCurrentPropertyDictionary()
    {
      Hashtable properties = new Hashtable();
      foreach (PropertyDescriptor pd in TypeDescriptor.GetProperties(Property.Source))
      {
        properties[pd.Name] = pd.GetValue(Property.Source);
      }
      return properties;
    }

    #region IDataErrorInfo Members

    private string _error;

    string IDataErrorInfo.Error
    {
      get { return GetErrorString(); }
    }

    string IDataErrorInfo.this[string columnName]
    {
      get { return GetErrorString(); }
    }

    private string GetErrorString()
    {
      IDataErrorInfo errorInfo = UnderlyingObject as IDataErrorInfo;
      if (errorInfo == null)
      {
          if (Property.Parent != null)
          {
              errorInfo = Property.Parent.Source as IDataErrorInfo;
              if (errorInfo != null)
              {
                  var propName = String.Format("{0}.{1}", Property.Parent.Name, PropertyName);
                  string errorString = errorInfo[propName];
                  return errorString ?? _error;
              }
          }
          return _error;
      }
      else
      {
        string errorString = errorInfo[PropertyName];
        return errorString ?? _error;
      }
    }

    #endregion

    /// <summary>
    /// Gets the declared type of the value.
    /// </summary>
    public abstract Type DataType { get; }

  }

  /// <summary>
  /// A wrapper class which allows the data binding infrastructure to get or set property values
  /// without knowing the name of the property being accessed.
  /// </summary>
  /// <remarks>This class supports the WPF data binding infrastructure and is not intended for
  /// use from user code.</remarks>
  public sealed class ObjectWrapper<T> : ObjectWrapper
  {
    /// <summary>
    /// Initialises a new instance of the <see cref="ObjectWrapper{T}"/> class.
    /// </summary>
    /// <param name="property">The property whose value is to be accessed through the wrapper.</param>
    /// <param name="editable">Whether the value can be modified.</param>
    public ObjectWrapper(Node property, bool editable)
      : base(property, editable) { }

    /// <summary>
    /// Initialises a new instance of the <see cref="ObjectWrapper{T}"/> class.
    /// </summary>
    /// <param name="property">The property whose value is to be accessed through the wrapper.</param>
    /// <param name="editable">Whether the value can be modified.</param>
    /// <param name="editContext">Additional user-specified information to be made available to
    /// consumers of the ObjectWrapper.</param>
    public ObjectWrapper(Node property, bool editable, object editContext)
      : base(property, editable, editContext) { }

    /// <summary>
    /// Gets or sets the value of the wrapped property.
    /// </summary>
    public T Value
    {
      get
      {
        return (T)RawValue;
      }
      set
      {
        RawValue = value;
      }
    }

    /// <summary>
    /// Gets the declared type of the value.
    /// </summary>
    public override Type DataType
    {
      get { return typeof(T); }
    }
  }
}
