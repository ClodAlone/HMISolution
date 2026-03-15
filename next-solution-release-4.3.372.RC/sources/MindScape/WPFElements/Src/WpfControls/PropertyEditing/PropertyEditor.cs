using System;
using System.Windows;
using System.Windows.Controls;

namespace Mindscape.WpfElements.PropertyEditing
{
  /// <summary>
  /// Provides value editing services for a named property of a specified type.
  /// </summary>
  public class PropertyEditor : ObjectWrappingEditor
  {
    /// <summary>
    /// Gets or sets the type on which the property is declared.  This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="DeclaringTypeProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public Type DeclaringType
    {
      get { return (Type)GetValue(DeclaringTypeProperty); }
      set { SetValue(DeclaringTypeProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="DeclaringType"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty DeclaringTypeProperty =
        DependencyProperty.Register("DeclaringType", typeof(Type), typeof(PropertyEditor));

    /// <summary>
    /// Gets or sets the property handled by this editor.  This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="PropertyNameProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public string PropertyName
    {
      get { return (string)GetValue(PropertyNameProperty); }
      set { SetValue(PropertyNameProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="PropertyName"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty PropertyNameProperty =
        DependencyProperty.Register("PropertyName", typeof(string), typeof(PropertyEditor));

    /// <summary>
    /// Indicates whether the editor can edit the value of the specified node.
    /// </summary>
    /// <param name="node">The node.</param>
    /// <returns>true if the editor can edit the value of this node; otherwise false.</returns>
    public override bool CanEdit(Node node)
    {
      Many many = node.Source as Many;
      if (many != null)
      {
        bool matchesName = (PropertyName == many.Descriptor.Name || PropertyName == many.Descriptor.DisplayName);
        return many.IsConsistent && matchesName;
      }

      return (DeclaringType == null || DeclaringType.IsAssignableFrom(node.Source.GetType())) &&
        (PropertyName == node.Name || PropertyName == node.HumanName);
    }

    /// <summary>
    /// Gets or sets the editor style.  This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="FrameworkElement.StyleProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public Style Style
    {
      get { return (Style)GetValue(FrameworkElement.StyleProperty); }
      set { SetValue(FrameworkElement.StyleProperty, value); }
    }

    /// <summary>
    /// Attaches the editor data template to the <see cref="Editor.EditorTemplate"/>.
    /// </summary>
    /// <param name="factory">The FrameworkElementFactory representing the data template under construction.</param>
    /// <param name="node">The node which the template will edit.</param>
    protected override void SetContentTemplate(FrameworkElementFactory factory, Node node)
    {
      factory.SetValue(ContentControl.ContentTemplateProperty, EditorTemplate);
    }

    /// <summary>
    /// Propagates the <see cref="Style"/> of the <see cref="PropertyEditor"/> object to the
    /// data template visual tree.
    /// </summary>
    /// <param name="factory">The FrameworkElementFactory representing the data template under construction.</param>
    /// <param name="node">The node which the template will edit.</param>
    protected override void OnCustomizeTemplate(FrameworkElementFactory factory, Node node)
    {
      if (Style == null && ContainingGrid != null)
      {
        foreach (BuiltInEditorStyle editorStyle in ContainingGrid.BuiltInEditorStyles)
        {
          if (ContainingGrid.FindResource(editorStyle.EditorKey) == EditorTemplate)
          {
            factory.SetValue(HostStyleProperty, editorStyle.Style);
            break;
          }
        }
      }
      else if (Style != null)
      {
        factory.SetValue(HostStyleProperty, Style);
      }
    }
  }
}
