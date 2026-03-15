using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Reflection;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Windows.Data;


namespace Monogram.WpfUtils
{
  public class FieldGenerator : IFieldGenerator
  {
    #region IFieldGenerator members

    public virtual IEnumerable<DetailsViewRow> GenerateFields(object dataItem, DetailsView container)
    {
      var result = new List<DetailsViewRow>();

      if (dataItem == null) return result;

      Type dataType = dataItem.GetType();
      PropertyInfo[] properties = dataType.GetProperties();

      BindableAttribute bindable = ((BindableAttribute[])dataType.GetCustomAttributes(typeof(System.ComponentModel.BindableAttribute), true)).FirstOrDefault();
      if (bindable == null) bindable = new BindableAttribute(true, BindingDirection.TwoWay);
      else if (!bindable.Bindable) return result; //if context object is not bindable do nothing

      int row = 0;
      foreach (PropertyInfo property in properties)
      {
        // Check if Readable
        if (!property.CanRead) continue;
        
        DetailsViewRow field = CreateField(property, bindable, row, container);
        if (field != null) result.Add(field);

        row++;
      }
      return result.OrderBy(f => f.OrderNumber);
    }

    #endregion


    #region overridable helper methods

   /// <summary>
    /// can return null
    /// </summary> 
    public virtual DetailsViewRow CreateField(PropertyInfo property, BindableAttribute objectBindable, int index, DetailsView container)
    {
      DisplayAttribute display = GetDisplayAttribute(property, index);
      if (display.GetAutoGenerateField() == false) return null;

      DisplayFormatAttribute format = (DisplayFormatAttribute)property.GetCustomAttributes(typeof(DisplayFormatAttribute), false).FirstOrDefault();

      DetailsViewRow field = new DetailsViewRow();
      field.Header = display.Name;
      field.OrderNumber = display.Order;

      var bindableAttr = CreateBindable(property); //the way of binding deterbined by attribute a property's flags.
      if (!bindableAttr.Bindable) return null;

      Binding binding = new Binding(property.Name);
      binding.Mode = (objectBindable.Direction == BindingDirection.OneWay || bindableAttr.Direction == BindingDirection.OneWay || !container.AllowEdit)
                     ? BindingMode.OneWay : BindingMode.TwoWay;
      binding.ValidatesOnDataErrors = true;
      binding.ValidatesOnExceptions = true;
      binding.NotifyOnValidationError = true;
      binding.NotifyOnTargetUpdated = true;
      binding.NotifyOnSourceUpdated = true;
      binding.IsAsync = true;

      if (format != null) 
      {
        binding.StringFormat = format.DataFormatString;
        binding.FallbackValue = format.NullDisplayText;
      }

      field.DisplayMemberBinding = binding;

      return field;
    }

    public virtual DisplayAttribute GetDisplayAttribute(PropertyInfo property, int index)
    {
      DisplayAttribute display =
        ((DisplayAttribute[])property.GetCustomAttributes(typeof(DisplayAttribute), false)).FirstOrDefault()
          ??
        new DisplayAttribute
        {
          AutoGenerateField = true,
          Prompt = null,
          GroupName = null,
          Description = null
        };

      if (display.GetName() == null) display.Name = property.Name;
      if (display.GetShortName() == null) display.ShortName = property.Name;
      if (!display.GetOrder().HasValue) display.Order = index +1;
      return display;
    }


    /// <summary>
    /// checks if property is bindable and editable and return appropriate Binding (Binding mode is determined by bindingdirection)
    /// </summary>
    public virtual BindableAttribute CreateBindable(PropertyInfo property)
    {
      BindableAttribute propBindable = ((BindableAttribute[])property.GetCustomAttributes(typeof(System.ComponentModel.BindableAttribute), true)).FirstOrDefault();

      if (propBindable != null)
      {
        if (!propBindable.Bindable || propBindable.Direction == BindingDirection.OneWay)
          return propBindable; //if not bindable, direction doesn't matter. if direction is OneWay we dont care if property is editable
      }

      //if property is not editable direction must be OneWay
      EditableAttribute propEditable = ((EditableAttribute[])property.GetCustomAttributes(typeof(EditableAttribute), false)).FirstOrDefault();
      if (propEditable == null) propEditable = new EditableAttribute(allowEdit: true);

      if (!property.CanWrite || !propEditable.AllowEdit || !property.GetSetMethod(true).IsPublic)
        return new BindableAttribute(true, BindingDirection.OneWay);

      if (propBindable != null) return propBindable;
      else return new BindableAttribute(true, BindingDirection.TwoWay);

    }

    #endregion
  }
}