using System.Windows.Controls;
using System.Windows;
using System.Windows.Data;
using System.ComponentModel;

namespace Mindscape.WpfElements
{
  /// <summary>
  /// Represents a column in a <see cref="MulticolumnTreeView"/>.
  /// </summary>
  /// <remarks>You do not need to create instances of this type directly.
  /// You should use the normal <see cref="GridViewColumn"/>; the MulticolumnTreeView
  /// internally creates wrapper objects as required.</remarks>
  public class MulticolumnTreeViewColumn : GridViewColumn
  {
    internal MulticolumnTreeViewColumn() { }

    /// <summary>
    /// Gets or sets the cell template to be used for this column within
    /// any decorator applied by the <see cref="MulticolumnTreeView"/>.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="OriginalCellTemplateProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public DataTemplate OriginalCellTemplate
    {
      get { return (DataTemplate)GetValue(OriginalCellTemplateProperty); }
      set { SetValue(OriginalCellTemplateProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="OriginalCellTemplate"/> property.
    /// </summary>
    public static readonly DependencyProperty OriginalCellTemplateProperty =
        DependencyProperty.Register("OriginalCellTemplate", typeof(DataTemplate), 
        typeof(MulticolumnTreeViewColumn));


    /// <summary>
    /// Gets or sets the cell template selector to be used for this column
    /// within any decorator applied by the <see cref="MulticolumnTreeView"/>.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="OriginalCellTemplateSelectorProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public DataTemplateSelector OriginalCellTemplateSelector
    {
      get { return (DataTemplateSelector)GetValue(OriginalCellTemplateSelectorProperty); }
      set { SetValue(OriginalCellTemplateSelectorProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="OriginalCellTemplateSelector"/> property.
    /// </summary>
    public static readonly DependencyProperty OriginalCellTemplateSelectorProperty =
        DependencyProperty.Register("OriginalCellTemplateSelector", typeof(DataTemplateSelector), 
        typeof(MulticolumnTreeViewColumn));



    internal static GridViewColumn CreateFrom(GridViewColumn source, bool applyExpander, DataTemplate expander)
    {
      GridViewColumn wrapper = CreateBaseColumn(source, applyExpander, expander);

      BindWrapperPropertyToSourceProperty(wrapper, source, GridViewColumn.HeaderProperty);
      BindWrapperPropertyToSourceProperty(wrapper, source, GridViewColumn.HeaderContainerStyleProperty);
      BindWrapperPropertyToSourceProperty(wrapper, source, GridViewColumn.HeaderTemplateProperty);
      BindWrapperPropertyToSourceProperty(wrapper, source, GridViewColumn.HeaderTemplateSelectorProperty);
      BindWrapperPropertyToSourceProperty(wrapper, source, GridViewColumn.WidthProperty);

      return wrapper;
    }

    private static void BindWrapperPropertyToSourceProperty(
      GridViewColumn wrapper, GridViewColumn source, DependencyProperty property)
    {
      BindWrapperPropertyToSourceProperty(wrapper, source, property, property);
    }

    private static void BindWrapperPropertyToSourceProperty(
      GridViewColumn wrapper, GridViewColumn source, 
      DependencyProperty wrapperProperty, DependencyProperty sourceProperty)
    {
      Binding binding = new Binding(sourceProperty.Name);
      binding.Source = source;
      BindingOperations.SetBinding(wrapper, wrapperProperty, binding);
    }

    internal static GridViewColumnCollection CreateFrom(GridViewColumnCollection source, DataTemplate expander)
    {
      int index = 0;

      GridViewColumnCollection result = new GridViewColumnCollection();
      foreach (GridViewColumn column in source)
      {
        GridViewColumn resultColumn = CreateFrom(column, (index == 0), expander);
        ++index;
        result.Add(resultColumn);
      }

      return result;
    }

    private static GridViewColumn CreateBaseColumn(GridViewColumn source, bool applyExpander, DataTemplate expander)
    {
      if (applyExpander)
      {
        MulticolumnTreeViewColumn wrapper = new MulticolumnTreeViewColumn();
        wrapper.CellTemplate = expander;
        if (source.DisplayMemberBinding == null)
        {
          BindWrapperPropertyToSourceProperty(wrapper, source, 
            MulticolumnTreeViewColumn.OriginalCellTemplateProperty, GridViewColumn.CellTemplateProperty);
          BindWrapperPropertyToSourceProperty(wrapper, source,
            MulticolumnTreeViewColumn.OriginalCellTemplateSelectorProperty, GridViewColumn.CellTemplateSelectorProperty);
        }
        else
        {
          FrameworkElementFactory fef = new FrameworkElementFactory(typeof(TextBlock));
          fef.SetBinding(TextBlock.TextProperty, source.DisplayMemberBinding);
          wrapper.OriginalCellTemplate = new DataTemplate();
          wrapper.OriginalCellTemplate.VisualTree = fef;
        }
        wrapper.DisplayMemberBinding = null;
        ((INotifyPropertyChanged)source).PropertyChanged += delegate(object sender, PropertyChangedEventArgs e)
        {
          if (e.PropertyName == "DisplayMemberBinding")
          {
            if (source.DisplayMemberBinding == null)
            {
              BindWrapperPropertyToSourceProperty(wrapper, source,
                MulticolumnTreeViewColumn.OriginalCellTemplateProperty, GridViewColumn.CellTemplateProperty);
              BindWrapperPropertyToSourceProperty(wrapper, source,
                MulticolumnTreeViewColumn.OriginalCellTemplateSelectorProperty, GridViewColumn.CellTemplateSelectorProperty);
            }
            else
            {
              FrameworkElementFactory fef = new FrameworkElementFactory(typeof(TextBlock));
              fef.SetBinding(TextBlock.TextProperty, source.DisplayMemberBinding);
              wrapper.OriginalCellTemplate = new DataTemplate();
              wrapper.OriginalCellTemplate.VisualTree = fef;
            }
          }
        };
        return wrapper;
      }
      else
      {
        GridViewColumn wrapper = new GridViewColumn();
        BindWrapperPropertyToSourceProperty(wrapper, source, GridViewColumn.CellTemplateProperty);
        BindWrapperPropertyToSourceProperty(wrapper, source, GridViewColumn.CellTemplateSelectorProperty);
        wrapper.DisplayMemberBinding = source.DisplayMemberBinding;
        ((INotifyPropertyChanged)source).PropertyChanged += delegate(object sender, PropertyChangedEventArgs e)
        {
          if (e.PropertyName == "DisplayMemberBinding")
          {
            wrapper.DisplayMemberBinding = source.DisplayMemberBinding;
          }
        };
        return wrapper;
      }
    }
  }
}
