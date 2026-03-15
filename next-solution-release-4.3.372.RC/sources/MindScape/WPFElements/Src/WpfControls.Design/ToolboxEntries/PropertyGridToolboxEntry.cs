using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using Mindscape.WpfElements.PropertyEditing;
using Mindscape.WpfElements.WpfPropertyGrid;

namespace Mindscape.WpfElements.Design
{
  internal partial class ToolboxEntries
  {
    internal static readonly ToolboxEntry PropertyGridToolboxEntry = new ToolboxEntry(

      typeof(PropertyGrid),
      "A control for browsing and setting the properties of an object",

      new DependencyProperty[] {
        PropertyGrid.BindingViewProperty,
        Editor.HostStyleProperty,
        PropertyGrid.NodesProperty,
        TreeListView.IsFillColumnProperty,
        TreeListView.IsResizeColumnProperty,
        PropertyGrid.SelectedGridItemProperty,
        PropertyGrid.PropertyNameTemplateProperty,
        PropertyGrid.SelectedObjectsProperty,
      },

      new PropertyCategory[] {
        new PropertyCategory("Property Editing", new object[] {
          "Editors",
          PropertyGrid.BuiltInEditorStylesProperty,
          PropertyGrid.AllowModifyCollectionsProperty,
          PropertyGrid.ItemsSourceProperty,
          PropertyGrid.SelectedObjectProperty
        }),
        new PropertyCategory("Appearance", new DependencyProperty[] {
          PropertyGrid.IsToolBarVisibleProperty,
          PropertyGrid.DefaultMarginProperty
        }),
        new PropertyCategory("Behavior", new DependencyProperty[] {
          PropertyGrid.CursorKeyModeProperty,
          PropertyGrid.GroupingProperty,
          PropertyGrid.SortingProperty,
          PropertyGrid.SortSubpropertiesProperty
        })
      }

      );
  }
}
