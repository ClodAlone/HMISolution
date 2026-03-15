using System;
using System.Windows;
using System.Windows.Data;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Mindscape.WpfElements.PropertyEditing
{
  /// <summary>
  /// Provides a value editing service for all values of a given type.
  /// </summary>
  public class TypeEditor : Editor
  {
    /// <summary>
    /// The type of value that this editor can edit.  This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <para>The object to which the <see cref="Editor.EditorTemplate"/> will be bound is of
    /// this type; therefore the template can reference member properties of this type without
    /// qualification.</para>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="EditedTypeProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public Type EditedType
    {
      get { return (Type)GetValue(EditedTypeProperty); }
      set { SetValue(EditedTypeProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="EditedType"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty EditedTypeProperty =
        DependencyProperty.Register("EditedType", typeof(Type), typeof(TypeEditor));


    /// <summary>
    /// Gets or sets the way in which the <see cref="Editor.EditorTemplate"/> is bound to the
    /// property value.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="TemplateBindingModeProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public TypeEditorTemplateBindingMode TemplateBindingMode
    {
      get { return (TypeEditorTemplateBindingMode)GetValue(TemplateBindingModeProperty); }
      set { SetValue(TemplateBindingModeProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="TemplateBindingMode"/> property.
    /// </summary>
    public static readonly DependencyProperty TemplateBindingModeProperty =
        DependencyProperty.Register("TemplateBindingMode", typeof(TypeEditorTemplateBindingMode), 
        typeof(TypeEditor), new UIPropertyMetadata(TypeEditorTemplateBindingMode.Default));

    /// <summary>
    /// Indicates whether the editor can edit the value of the specified node.
    /// </summary>
    /// <param name="node">The node.</param>
    /// <returns>true if the editor can edit the value of this node; otherwise false.</returns>
    /// <remarks>A <see cref="TypeEditor"/> will attempt to edit any node whose declared 
    /// <see cref="Node.PropertyType"/> is compatible with its <see cref="EditedType"/>.  To prevent
    /// a TypeEditor from claiming derived types, specify a more specific TypeEditor earlier in the
    /// <see cref="PropertyGrid.Editors"/> collection.</remarks>
    public override bool CanEdit(Node node)
    {
      return EditedType.IsAssignableFrom(node.PropertyType);
    }

    private object GetBindableValue(Node node)
    {
      switch (TemplateBindingMode)
      {
        case TypeEditorTemplateBindingMode.Default:
          if (node.PropertyType.IsValueType || node.PropertyType.IsInterface)
          {
            return ObjectWrapperFactory.CreateWrapper(node, node.CanWrite);
          }
          else
          {
            return node.Value;
          }
        case TypeEditorTemplateBindingMode.Reference:
          return node.Value;
        case TypeEditorTemplateBindingMode.WrappedValue:
          return ObjectWrapperFactory.CreateWrapper(node, node.CanWrite);
        default:
          throw new InvalidOperationException("Unexpected TemplateBindingMode");
      }
    }

    /// <summary>
    /// Constructs a <see cref="DataTemplate"/> which can be bound to the specified node
    /// to edit its value.
    /// </summary>
    /// <param name="node">The node for which an editor is required.</param>
    /// <returns>A data template for editing the node value.</returns>
    public override DataTemplate BuildTemplate(Node node)
    {
      // TODO: This now looks a lot more like the ObjectWrappingEditor implementation than
      // it did before.  Clear scope for convergence, but can it be done without making
      // breaking changes?

      Binding defaultMarginBinding = new Binding(PropertyGrid.DefaultMarginProperty.Name);
      defaultMarginBinding.RelativeSource = new RelativeSource(RelativeSourceMode.FindAncestor, typeof(PropertyGrid), 1);

      FrameworkElementFactory factory = new FrameworkElementFactory(typeof(ContentControl));
      factory.SetValue(ContentControl.ContentTemplateProperty, EditorTemplate);
      factory.SetValue(ContentControl.ContentProperty, GetBindableValue(node));

      factory.SetValue(Control.PaddingProperty, new Thickness(0));
      factory.SetBinding(Control.MarginProperty, defaultMarginBinding);
      factory.SetValue(ContentControl.BackgroundProperty, Brushes.Transparent);
      factory.SetValue(ContentControl.IsTabStopProperty, false);
      factory.SetValue(KeyboardNavigation.TabNavigationProperty, KeyboardNavigationMode.Local);

      if (ContainingGrid != null)
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

      DataTemplate template = new DataTemplate();
      template.VisualTree = factory;
      return template;
    }
  }

  /// <summary>
  /// Specifies whether a <see cref="TypeEditor"/> acts on a property reference (appropriate
  /// for reference types) or on a value (appropriate for value types).  This is normally
  /// inferred by the editor according to the edited type, but may be overridden if required.
  /// </summary>
  public enum TypeEditorTemplateBindingMode
  {
    /// <summary>
    /// The TypeEditor infers how to act on the property depending on whether it is a
    /// reference or a value type.
    /// </summary>
    Default,

    /// <summary>
    /// The TypeEditor considers properties of the edited type to be references.  The
    /// template binds directly to properties of the edited property value.
    /// </summary>
    Reference,

    /// <summary>
    /// The TypeEditor considers properties of the edited type to be values.  The
    /// template binds to a pseudo-property named Value.
    /// </summary>
    WrappedValue
  }
}
