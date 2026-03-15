using System.Windows;

namespace Mindscape.WpfElements.Design
{
  internal partial class ToolboxEntries
  {
    internal static readonly ToolboxEntry MulticolumnTreeViewToolboxEntry = new ToolboxEntry(

      typeof(MulticolumnTreeView),
      "A control with the expand/collapse functionality of a TreeView, but supporting multiple columns like a ListView.",

      new DependencyProperty[] {
        MulticolumnTreeView.WrappedColumnsProperty,
      },

      new PropertyCategory[] {
        new PropertyCategory("Appearance", new DependencyProperty[] {
          MulticolumnTreeView.ColumnHeaderContainerStyleProperty,
          MulticolumnTreeView.ColumnHeaderTemplateProperty,
          MulticolumnTreeView.ColumnHeaderTemplateSelectorProperty,
          MulticolumnTreeView.ColumnsProperty,
          MulticolumnTreeView.ExpandingDecoratorProperty,
        })
      }

      );
  }
}
