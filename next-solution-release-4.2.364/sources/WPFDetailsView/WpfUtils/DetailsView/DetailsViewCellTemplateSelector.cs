using System.Windows.Controls;
using System.Windows.Data;
using System.Windows;
using System;
using System.Collections;


namespace Monogram.WpfUtils
{
  public class DetailsViewCellTemplateSelector : DataTemplateSelector
  {
    #region constants

    public const string CELL_TEMPLATE = "DV_FieldTemplate";
    public const string TEPLATE_PATTERN = "DV_{0}FieldTemplate";
    public const string ENUMERABLE_CELL_TEMPLATE = "DV_EnumerableFieldTemplate";
    public const string ENUM_CELL_TEMPLATE = "DV_EnumFieldTemplate";
    public const string EDIT_POSTFIX = "_Edit";

    #endregion


    public static DetailsViewCellTemplateSelector Instance
    {
      get
      {
        if (_Instance == null) _Instance = new DetailsViewCellTemplateSelector();
        return _Instance;
      }
    }
    private static DetailsViewCellTemplateSelector _Instance;

    public FrameworkElement VisualTreeHack;

    public override System.Windows.DataTemplate SelectTemplate(object item, DependencyObject container)
    {
      var field = (DetailsViewRow)item;
      var dv = (DetailsView)container;

      if (field.CellTemplate == null)
      {
        if (field.CellTemplateSelector != null && field.CellTemplateSelector.GetType() != this.GetType())
        {
          // if field.CellTemplateSelector is different from DetailsView.CellTemplateSelector then use field.CellTemplateSelector
          field.CellTemplate = field.CellTemplateSelector.SelectTemplate(field, dv);
        }
        else
        {
          if (field.DisplayMemberBinding == null) return null;

          var value = DataBinder.Eval(container, field.DisplayMemberBinding);
          //var value = new BindingEvaluator(field.DisplayMemberBinding, dv.DataContext, VisualTreeHack).Value;

          string resourceKey = value == null ? CELL_TEMPLATE : string.Format(TEPLATE_PATTERN, value.GetType().Name);
          var postFix = dv.AllowEdit && field.DisplayMemberBinding.Mode != BindingMode.OneWay ? EDIT_POSTFIX : string.Empty;
          object template = null;

          while (template == null)
          {
            template = dv.TryFindResource(resourceKey + postFix);
            if (template == null) dv.TryFindResource(resourceKey);
            if (template == null)
            {
              if (value is string) template = dv.FindResource(CELL_TEMPLATE + postFix); //string is also ienumebrable, so this is exception
              else if (value is Enum) template = dv.FindResource(ENUM_CELL_TEMPLATE + postFix);
              else if (value is IEnumerable) template = dv.TryFindResource(ENUMERABLE_CELL_TEMPLATE + postFix);
              if (template == null) template = dv.FindResource(CELL_TEMPLATE + postFix); //todo: check id type is convertible to string (CELL_TEMPLATE is textbox)
            }
            if (template == null) postFix = string.Empty;
          }
          field.CellTemplate = (DataTemplate)template;
        }
      }

      return field.CellTemplate;
    }
  }
}
