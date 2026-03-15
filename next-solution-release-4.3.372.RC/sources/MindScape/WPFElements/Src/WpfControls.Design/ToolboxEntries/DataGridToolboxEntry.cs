using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Mindscape.WpfElements.Design
{
  internal partial class ToolboxEntries
  {
    internal static readonly ToolboxEntry DataGridToolboxEntry = new ToolboxEntry(

      typeof(DataGrid),
      "A control for editing a collection of objects in a rectangular grid",

      new DependencyProperty[] {
      },

      new PropertyCategory[] {
        new PropertyCategory("Behavior", new DependencyProperty[] {
          DataGrid.AllowColumnReorderProperty,
          DataGrid.AutoGenerateColumnsProperty,
          DataGrid.SelectionModeProperty,
          DataGrid.SelectionTypeProperty
        }),
        new PropertyCategory("Appearance", new DependencyProperty[] {
          DataGrid.ColumnsProperty,
          DataGrid.RowHeaderWidthProperty,
          DataGrid.IsFrozenLineVisibleProperty,
          DataGrid.IsPagerVisibleProperty,
          DataGrid.ShowColumnHeadersProperty,
          DataGrid.ShowFirstFrozenLineProperty,
          DataGrid.ShowRowHeadersProperty
        })
      }

      );
  }
}
