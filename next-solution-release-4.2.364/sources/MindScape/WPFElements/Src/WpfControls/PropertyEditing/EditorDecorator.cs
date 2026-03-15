using System.Windows.Controls;
using System.Windows;

namespace Mindscape.WpfElements.PropertyEditing
{
  /// <summary>
  /// A control which acts a placeholder for wrapping a decorating DataTemplate
  /// (represented by the ContentTemplate) around a "base" template known only at run time.
  /// </summary>
  public class EditorDecorator : ContentControl
  {
    /// <summary>
    /// Gets or sets the template around which the decoration will be placed.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="DecoratedTemplateProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public DataTemplate DecoratedTemplate
    {
      get { return (DataTemplate)GetValue(DecoratedTemplateProperty); }
      set { SetValue(DecoratedTemplateProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="DecoratedTemplate"/> property.
    /// </summary>
    public static readonly DependencyProperty DecoratedTemplateProperty =
        DependencyProperty.Register("DecoratedTemplate", typeof(DataTemplate), typeof(EditorDecorator));

    /// <summary>
    /// Gets or sets the <see cref="Node"/> that represents the data item being edited.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="DecoratedNodeProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public Node DecoratedNode
    {
      get { return (Node)GetValue(DecoratedNodeProperty); }
      set { SetValue(DecoratedNodeProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="DecoratedNode"/> property.
    /// </summary>
    public static readonly DependencyProperty DecoratedNodeProperty =
        DependencyProperty.Register("DecoratedNode", typeof(Node), typeof(EditorDecorator));
  }
}
