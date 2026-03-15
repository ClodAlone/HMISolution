using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.ComponentModel;
using Mindscape.WpfElements.PropertyEditing;

namespace Mindscape.WpfElements.WpfDataGrid
{
  /// <summary>
  /// Describes how to build a filter from the valid values of an object.
  /// </summary>
  public class ObjectFilterDescription : ViewModelBase, IFilterDescription
  {
    private readonly BindingList<SelectableObject> _values;
    private bool? _isAllSelected = false;

    private IFilter _filter;

    private bool _lock;

    /// <summary>
    /// Initializes a new instance of the <see cref="ObjectFilterDescription"/> class.
    /// </summary>
    /// <param name="propertyInfo">Used to provide a type converter if any.</param>
    /// <param name="values">The valid selectable values.</param>
    internal ObjectFilterDescription(IPropertyInfo propertyInfo, IList values)
    {
      _values = new BindingList<SelectableObject>();
      TypeConverter tc = propertyInfo.Converter;
      if(!tc.CanConvertTo(typeof(string)))
      {
        // TODO: test this code path
        tc = null;
      }
      foreach (object o in values)
      {
        string displayValue = null;
        
        if (tc != null && !"".Equals(o))
        {
          displayValue = tc.ConvertToString(o);
        }
        else if (o != null)
        {
          // TODO: test this code path
          displayValue = o.ToString();
        }
        _values.Add(new SelectableObject(o, displayValue));
      }
      _values.ListChanged += new ListChangedEventHandler(Values_ListChanged);
    }

    private void Values_ListChanged(object sender, ListChangedEventArgs e)
    {
      if (e.ListChangedType == ListChangedType.ItemChanged)
      {
        if (!_lock)
        {
          _lock = true;
          UpdateFilter();
          _lock = false;
        }
      }
    }

    /// <summary>
    /// Gets or sets whether or not all the values are selected.
    /// This property is null if at least one, but not all items are selected.
    /// </summary>
    public bool? IsAllSelected
    {
      get { return _isAllSelected; }
      set
      {
        if (_isAllSelected != value)
        {
          _isAllSelected = value;
          OnPropertyChanged("IsAllSelected");

          if (!_lock)
          {
            _lock = true;
            
            if (_isAllSelected != null)
            {
              foreach (SelectableObject o in _values)
              {
                o.IsSelected = _isAllSelected.Value;
              }
            }

            UpdateFilter();
            _lock = false;
          }
        }
      }
    }

    /// <summary>
    /// Gets a list of selectable values.
    /// </summary>
    public IList Values
    {
      get { return _values; }
    }

    /// <summary>
    /// Gets the current filter created by this filter description.
    /// </summary>
    public IFilter Filter
    {
      get { return _filter; }
      private set
      {
        _filter = value;
        OnFilterChanged();
      }
    }

    /// <summary>
    /// Raised when the <see cref="Filter"/> changes.
    /// </summary>
    public event EventHandler FilterChanged;

    private void OnFilterChanged()
    {
      EventHandler handler = FilterChanged;
      if (handler != null)
      {
        handler(this, EventArgs.Empty);
      }
    }

    private void UpdateFilter()
    {
      OrFilter rootFilter = new OrFilter();
      IFilter lastFilter = null;
      int count = 0;
      foreach (SelectableObject o in _values)
      {
        if (o.IsSelected)
        {
          EqualsFilter filter = new EqualsFilter(o.ActualValue);
          lastFilter = filter;
          rootFilter.Add(filter);
          count++;
        }
      }
      IsAllSelected = count == 0 ? new Nullable<bool>(false) : count == _values.Count ? new Nullable<bool>(true) : null;
      Filter = count == 0 ? null : count == 1 ? lastFilter : rootFilter;
    }

    /// <summary>
    /// Sets the filter description to best match the given <see cref="IFilter"/>.
    /// </summary>
    /// <param name="filter">The <see cref="IFilter"/> to mimic.</param>
    public void SetAs(IFilter filter)
    {
      _lock = true;
      OrFilter orFilter = filter as OrFilter;
      if (orFilter != null) // TODO Test
      {
        // Or filter containing multiple equals filters:
        foreach (SelectableObject o in _values)
        {
          o.IsSelected = false;
        }
        foreach (IFilter subFilter in orFilter.Filters)
        {
          EqualsFilter equalsFilter = subFilter as EqualsFilter;
          if (equalsFilter != null)
          {
            foreach (SelectableObject o in _values)
            {
              if (o.ActualValue.Equals(equalsFilter.Value))
              {
                o.IsSelected = true;
              }
            }
          }
        }
      }
      else
      {
        // A single equals filter:
        EqualsFilter equalsFilter = filter as EqualsFilter;
        if (equalsFilter != null)
        {
          foreach (SelectableObject o in _values)
          {
            if (o.ActualValue.Equals(equalsFilter.Value))
            {
              o.IsSelected = true;
            }
            else
            {
              o.IsSelected = false;
            }
          }
        }
        else
        {
           // null or unsupported filter:
          foreach (SelectableObject o in _values)
          {
            o.IsSelected = false;
          }
        }
      }
      UpdateFilter();
      _lock = false;
    }
  }

  /// <summary>
  /// An object that knows its selection state.
  /// </summary>
  public class SelectableObject : ViewModelBase
  {
    private bool _isSelected;
    private readonly object _value;
    private readonly string _displayValue;

    /// <summary>
    /// Initializes a new instance of the <see cref="SelectableObject"/> class.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="displayValue">The display value.</param>
    internal SelectableObject(object value, string displayValue)
    {
      _value = value;
      _displayValue = displayValue;
    }

    internal object ActualValue
    {
      get { return _value; }
    }

    /// <summary>
    /// Gets the value.
    /// </summary>
    public object Value
    {
      get { return _displayValue; }
    }

    /// <summary>
    /// Gets or sets the selection state of the object.
    /// </summary>
    public bool IsSelected
    {
      get { return _isSelected; }
      set
      {
        if (_isSelected != value)
        {
          Set<bool>(ref _isSelected, value, "IsSelected");
        }
      }
    }
  }
}
