using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mindscape.WpfElements.PropertyEditing;
using System.Windows.Data;
using System.Reflection;
using System.Windows.Controls;
using System.Windows;
using System.Globalization;
using System.ComponentModel;
using System.Collections.ObjectModel;

namespace Mindscape.WpfElements.WpfDataGrid
{
  internal class BindingPropertyInfoAdapter : IPropertyInfo
  {
    private readonly Binding _binding;
    private readonly Extractor _extractor = new Extractor();
    private readonly Type _startingType;
    private readonly Type _propertyType;
    private bool _canWrite;

    private bool _isBindingInitialized = false;

    public BindingPropertyInfoAdapter(Binding binding, Type startingType)
    {
      Invariant.ArgumentNotNull(binding, "binding");

      _binding = binding;
      _extractor.Adapter = this;
      _extractor.SetBinding(Extractor.ValueProperty, _binding);
      _extractor.DataContext = null;
      //_extractor.DataContext = new Extractor();
      //object v = _extractor.Value; // Magical hackery to kick start the binding system in certain scenarios.
      //_extractor.SetBinding(Extractor.ValueProperty, _binding);
      _startingType = startingType;
      _propertyType = GetSourceType();
      CalculateCanWrite();
    }

    public Binding Binding { get { return _binding; } }

    public System.Reflection.PropertyAttributes Attributes
    {
      get { return System.Reflection.PropertyAttributes.None; }
    }

    public bool CanRead
    {
      get { return true; }
    }

    public bool CanWrite
    {
      get { return _canWrite; }
    }

    private void CalculateCanWrite()
    {
      PropertyInfo info = GetSourceProperty();
      /*if (info != null)
      {
        Type collectionType = info.PropertyType;
        if (CollectionUtilities.IsEnumerable(collectionType))
        {
          return true; // This should really check to see if the collection is readonly. But how to do that just using the type?
        }
      }*/
      _canWrite = (info == null || _binding.Mode != BindingMode.TwoWay) ? false : info.CanWrite;
    }

    public ParameterInfo[] GetIndexParameters()
    {
      return new ParameterInfo[0];
    }

    public object GetValue(object obj, BindingFlags invokeAttr, Binder binder, object[] index, CultureInfo culture)
    {
      object value = null;
      if (obj != null)
      {
        object invalidValue = null;
        _validationCache.TryGetValue(obj, out invalidValue);
        if (invalidValue != null)
        {
          value = invalidValue;
        }
        else
        {
          if (_extractor.DataContext != obj)
          {
            _extractor.loseFocus();
          }
          if (!_isBindingInitialized)
          {
            _extractor.SetBinding(Extractor.ValueProperty, _binding);
          }
          _extractor.DataContext = obj;
          value = _extractor.Value;
          if (!_isBindingInitialized)
          {
            _extractor.SetBinding(Extractor.ValueProperty, _binding);
            _isBindingInitialized = true;
          }
          //value = _extractor.Value;
        }
      }
      /*if (value != null && Binding.StringFormat != null)
      {
        value = String.Format("{0:" + Binding.StringFormat + "}", value);
      }*/
      return value;
    }

    internal Dictionary<object, object> ValidationCache { get { return _validationCache; } }

    public Type PropertyType
    {
      get { return _propertyType; }
    }

    private Type GetSourceType()
    {
      PropertyInfo info = GetSourceProperty();
      if (info != null)
      {
        Type collectionType = CollectionUtilities.GetValueType(info.PropertyType);
        if (collectionType != null)
        {
          return collectionType;
        }
      }
      return info == null ? typeof(string) : info.PropertyType;
    }

    private PropertyInfo GetSourceProperty()
    {
      if (_binding.Path != null && _binding.Path.Path != null && _startingType != null)
      {
        Type currentType = _startingType;
        PropertyInfo currentInfo = null;
        string[] parts = _binding.Path.Path.Split('.');
        foreach (string part in parts)
        {
          string[] subParts = part.Split('[', ']');
          string propertyName = part;

          if (subParts.Length > 1)
          {
            propertyName = subParts[0];
          }

          currentInfo = currentType.GetProperty(propertyName);
          if (currentInfo != null)
          {
            if (subParts.Length > 1)
            {
              currentType = CollectionUtilities.GetValueType(currentInfo.PropertyType);
            }
            else
            {
              currentType = currentInfo.PropertyType;
            }
          }
          else
          {
            return null;
          }
          if(currentType == null)
          {
            return null;
          }
        }
        return currentInfo;
      }
      return null;
    }

    public void SetValue(object obj, object value, BindingFlags invokeAttr, Binder binder, object[] index, CultureInfo culture)
    {
      if (_extractor.DataContext != obj)
      {
        _extractor.loseFocus();
      }
      _extractor.DataContext = obj;
      _extractor.Value = value;
      bool hasError = Validation.GetHasError(_extractor);
      if (hasError)
      {
        _validationCache[obj] = value;
      }
      else
      {
        _validationCache.Remove(obj);
      }
      if (Binding != null && Binding.UpdateSourceTrigger != UpdateSourceTrigger.LostFocus)
      {
        OnValueChanged();
      }
    }

    internal void OnValueChanged()
    {
      EventHandler handler = ValueChanged;
      if (handler != null)
      {
        handler(this, EventArgs.Empty);
      }
    }

    internal event EventHandler ValueChanged;

    private readonly Dictionary<object, object> _validationCache = new Dictionary<object,object>();

    internal Extractor Extractor
    {
      get { return _extractor; }
    }

    public Type DeclaringType
    {
      get { return _startingType; }
    }

    public object[] GetCustomAttributes(bool inherit)
    {
      return new Attribute[0];
    }

    public object[] GetCustomAttributes(Type attributeType, bool inherit)
    {
      List<Attribute> attrsOfType = new List<Attribute>();
      return attrsOfType.ToArray();
    }

    public bool IsDefined(Type attributeType, bool inherit)
    {
      return false;
    }

    public string Name
    {
      get { return _binding.Path.Path; }
    }

    public TypeConverter Converter
    {
      get { return null; }
    }

    private PropertyInfo _asPropertyInfo;

    public PropertyInfo AsPropertyInfo
    {
      get
      {
        EnsurePropertyInfoInitialised();
        return _asPropertyInfo;
      }
    }

    private void EnsurePropertyInfoInitialised()
    {
      if (_asPropertyInfo == null)
      {
        _asPropertyInfo = new DelegatingPropertyInfo(this);
      }
    }

    public PropertyDescriptor AsPropertyDescriptor
    {
      get
      {
        throw new NotSupportedException();
      }
    }

    public string DisplayName
    {
      get
      {
        PropertyInfo info = GetSourceProperty();
        if (info != null)
        {
          object[] attributes = info.GetCustomAttributes(typeof(DisplayNameAttribute), false);
          if (attributes.Length > 0)
          {
            DisplayNameAttribute item = attributes[0] as DisplayNameAttribute;
            if (item != null && !item.IsDefaultAttribute())
            {
              return item.DisplayName;
            }
          }
        }
        if (_binding.Path != null && _binding.Path.Path != null)
        {
          string[] parts = _binding.Path.Path.Split('.');
          if (parts.Length > 0)
          {
            return parts[parts.Length - 1];
          }
        }
        return "";
      }
    }

    public string Category
    {
      get { throw new NotSupportedException(); }
    }

    public string Description
    {
      get { throw new NotSupportedException(); }
    }

    public bool IsBrowsable
    {
      get { return true; }
    }
  }

  internal class Extractor : Control
  {
    /*public Extractor()
    {
      SourceUpdated += new EventHandler<DataTransferEventArgs>(Extractor_SourceUpdated);
    }

    private void Extractor_SourceUpdated(object sender, DataTransferEventArgs e)
    {
      
    }*/

    #region Value Property

    public object Value
    {
      get { return GetValue(ValueProperty); }
      set { SetValue(ValueProperty, value); }
    }

    public static readonly DependencyProperty ValueProperty =
      DependencyProperty.Register("Value", typeof(object), typeof(Extractor));

    #endregion // Value Property

    internal BindingPropertyInfoAdapter Adapter { get; set; }

    internal void loseFocus()
    {
      OnLostFocus(new RoutedEventArgs(Control.LostFocusEvent));
      if (DataContext != null)
      {
        bool hasError = Validation.GetHasError(this);
        if (hasError)
        {
          Adapter.ValidationCache[DataContext] = Value;
        }
        else
        {
          Adapter.ValidationCache.Remove(DataContext);
        }
      }
      Adapter.OnValueChanged();
    }
  }
}
