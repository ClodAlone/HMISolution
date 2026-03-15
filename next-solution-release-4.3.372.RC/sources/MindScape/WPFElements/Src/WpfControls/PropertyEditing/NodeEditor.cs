using System.Windows;
using System.Windows.Controls;

namespace Mindscape.WpfElements.PropertyEditing
{
  /// <summary>
  /// Provides value editing services for a single node instance.
  /// </summary>
  /// <remarks>Node editors are usually constructed for you by the <see cref="PropertyGrid"/>;
  /// you should not need to use this class in your code.</remarks>
  public abstract class NodeEditor : ObjectWrappingEditor
  { 
    /// <summary>
    /// Indicates whether the editor can edit the value of the specified node.
    /// </summary>
    /// <param name="node">The node.</param>
    /// <returns>true if the editor can edit the value of this node; otherwise false.</returns>
    public override bool CanEdit(Node node)
    {
      return true;
    }
  }

  /// <summary>
  /// Provides value editing services for a single node instance, using a static 
  /// <see cref="DataTemplate"/> defined in the <see cref="Editor.EditorTemplate"/> property.
  /// </summary>
  /// <remarks>Node editors are usually constructed for you by the <see cref="PropertyGrid"/>;
  /// you should not need to use this class in your code.</remarks>
  public class StaticNodeEditor : NodeEditor
  {
    /// <summary>
    /// Attaches the editor data template to the <see cref="Editor.EditorTemplate"/>.
    /// </summary>
    /// <param name="factory">The FrameworkElementFactory representing the data template under construction.</param>
    /// <param name="node">The node which the template will edit.</param>
    protected override void SetContentTemplate(FrameworkElementFactory factory, Node node)
    {
      factory.SetValue(ContentControl.ContentTemplateProperty, EditorTemplate);
    }
  }

  /// <summary>
  /// Provides value editing services for a single node instance, using an external
  /// <see cref="DataTemplate"/> referenced via a resource key.
  /// </summary>
  /// <remarks>Node editors are usually constructed for you by the <see cref="PropertyGrid"/>;
  /// you should not need to use this class in your code.</remarks>
  public class DynamicNodeEditor : NodeEditor
  {
    /// <summary>
    /// Gets or sets the resource key of the data template.  This is a dependency property.
    /// </summary>
    public object EditorTemplateKey
    {
      get { return GetValue(EditorTemplateKeyProperty); }
      set { SetValue(EditorTemplateKeyProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="EditorTemplateKey"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty EditorTemplateKeyProperty =
        DependencyProperty.Register("EditorTemplateKey", typeof(object), typeof(DynamicNodeEditor));

    /// <summary>
    /// Attaches the editor data template to the data template identified by the <see cref="EditorTemplateKey"/>.
    /// </summary>
    /// <param name="factory">The FrameworkElementFactory representing the data template under construction.</param>
    /// <param name="node">The node which the template will edit.</param>
    protected override void SetContentTemplate(FrameworkElementFactory factory, Node node)
    {
      factory.SetResourceReference(ContentControl.ContentTemplateProperty, EditorTemplateKey);
    }
  }
}
