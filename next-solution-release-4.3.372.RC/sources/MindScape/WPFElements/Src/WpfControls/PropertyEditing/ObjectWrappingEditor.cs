using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Input;

namespace Mindscape.WpfElements.PropertyEditing
{
  /// <summary>
  /// Provides value editing services for values accessed as properties on another object.
  /// </summary>
  public abstract class ObjectWrappingEditor : Editor
  {
    /// <summary>
    /// Gets or sets an object which is made available to the editor template
    /// through the EditContext binding.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="EditContextProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public object EditContext
    {
      get { return GetValue(EditContextProperty); }
      set { SetValue(EditContextProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="EditContext"/> property.
    /// </summary>
    public static readonly DependencyProperty EditContextProperty =
        DependencyProperty.Register("EditContext", typeof(object), 
        typeof(ObjectWrappingEditor), new PropertyMetadata(null));

    /// <summary>
    /// Constructs a <see cref="DataTemplate"/> which can be bound to the specified node
    /// to edit its value.
    /// </summary>
    /// <param name="node">The node for which an editor is required.</param>
    /// <returns>A data template for editing the node value.</returns>
    public override DataTemplate BuildTemplate(Node node)
    {
      bool editable = node.CanWrite && CanEdit(node);

      // TODO: this DefaultMargin binding is required for the property grid. But it causes binding expression warnings in the data grid.
      Binding defaultMarginBinding = new Binding(PropertyGrid.DefaultMarginProperty.Name);
      defaultMarginBinding.RelativeSource = new RelativeSource(RelativeSourceMode.FindAncestor, typeof(IProvideEditorHosting), 1);

      FrameworkElementFactory factory = new FrameworkElementFactory(typeof(ContentControl));
      SetContentTemplate(factory, node);

      ObjectWrapper editWrapper = ObjectWrapperFactory.CreateWrapper(node, editable, EditContext);

      factory.SetValue(ContentControl.ContentProperty, editWrapper);

      factory.SetValue(Control.PaddingProperty, new Thickness(0));
      factory.SetBinding(Control.MarginProperty, defaultMarginBinding);
      factory.SetValue(ContentControl.BackgroundProperty, Brushes.Transparent);
      factory.SetValue(ContentControl.IsTabStopProperty, false);
      factory.SetValue(KeyboardNavigation.TabNavigationProperty, KeyboardNavigationMode.Local);

      OnCustomizeTemplate(factory, node);

      DataTemplate template = new DataTemplate();
      template.VisualTree = factory;
      return template;
    }

    /// <summary>
    /// When overridden in a derived class, sets the <see cref="ContentControl.ContentTemplate"/>
    /// property of the specified <see cref="FrameworkElementFactory"/> to the required data template.
    /// </summary>
    /// <param name="factory">The FrameworkElementFactory on which the ContentTemplateProperty
    /// must be set.</param>
    /// <param name="node">The node which the template will edit.</param>
    protected abstract void SetContentTemplate(FrameworkElementFactory factory, Node node);

    /// <summary>
    /// Allows derived classes to customize the visual tree of the template constructed in 
    /// <see cref="BuildTemplate"/>, for example by setting up additional properties.
    /// </summary>
    /// <param name="factory">The FrameworkElementFactory that defines the template visual tree.</param>
    /// <param name="node">The node which the template will edit.</param>
    protected virtual void OnCustomizeTemplate(FrameworkElementFactory factory, Node node)
    {
      // Do nothing by default
    }
  }
}
