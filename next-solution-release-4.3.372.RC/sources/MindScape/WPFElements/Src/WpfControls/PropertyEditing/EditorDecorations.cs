using System.Windows;
using System;
using System.Linq;
using System.Collections.ObjectModel;
using System.Collections.Generic;

namespace Mindscape.WpfElements.PropertyEditing
{
  // TODO: I don't know much about editor decorations. Should the DataGrid support them?
  // The data grid needs to know about them because of the IExtendInPlaceEditors interface.

  internal static class EditorDecorations
  {
    private static EditorDecoration[] StandardDecorations = new EditorDecoration[] {
      new CollectionElementRemovalDecoration()
    };

    internal static DataTemplate Decorate(DataTemplate baseTemplate, Node node, object item, IEnumerable<EditorDecoration> customDecorations)
    {
      DataTemplate actualTemplate = baseTemplate;

      foreach (EditorDecoration decoration in (customDecorations ?? Enumerable.Empty<EditorDecoration>()).Concat(StandardDecorations))
      {
        if (decoration.AppliesTo(node))
        {
          actualTemplate = decoration.ApplyTo(actualTemplate, node, item);
        }
      }

      return actualTemplate;
    }
  }

  /// <summary>
  /// Represents a decoration that can be applied to an editor to provide additional
  /// services independent of the value being edited.  For example, collection element
  /// editors are decorated with a button to delete the item from the collection.
  /// </summary>
  public abstract class EditorDecoration
  {
    /// <summary>
    /// When overridden in a derived class, gets whether the decorator should be applied
    /// to a node.
    /// </summary>
    /// <param name="node">The node for which to check whether the decorator applies.</param>
    /// <returns>true if the decorator should be applied to the node editor; otherwise false.</returns>
    public abstract bool AppliesTo(Node node);

    /// <summary>
    /// When overridden in a derived class, populates the decorator with a DataTemplate.
    /// </summary>
    /// <param name="decorator">The decorator whose template is to be set.</param>
    protected abstract void ApplyDecoratorTemplate(FrameworkElementFactory decorator);

    internal DataTemplate ApplyTo(DataTemplate baseTemplate, Node node, object item)
    {
      FrameworkElementFactory decorator = new FrameworkElementFactory(typeof(EditorDecorator));
      decorator.SetValue(EditorDecorator.ContentProperty, item);
      ApplyDecoratorTemplate(decorator);
      decorator.SetValue(EditorDecorator.DecoratedTemplateProperty, baseTemplate);
      decorator.SetValue(EditorDecorator.DecoratedNodeProperty, node);

      DataTemplate template = new DataTemplate();
      template.VisualTree = decorator;
      return template;
    }
  }

  /// <summary>
  /// Represents an <see cref="EditorDecoration"/> where the decoration is specified
  /// as a DataTemplate
  /// </summary>
  public abstract class TemplateEditorDecoration : EditorDecoration
  {
    /// <summary>
    /// Gets or sets the <see cref="DataTemplate"/> of the decoration.
    /// </summary>
    public DataTemplate DecoratorTemplate { get; set; }

    /// <summary>
    /// Populates the decorator with the <see cref="DecoratorTemplate"/>.
    /// </summary>
    /// <param name="decorator">The decorator whose template is to be set.</param>
    protected sealed override void ApplyDecoratorTemplate(FrameworkElementFactory decorator)
    {
      decorator.SetValue(EditorDecorator.ContentTemplateProperty, DecoratorTemplate);
    }
  }

  internal abstract class DynamicResourceEditorDecoration : EditorDecoration
  {
    protected abstract object DecoratorResourceKey { get; }

    protected sealed override void ApplyDecoratorTemplate(FrameworkElementFactory decorator)
    {
      decorator.SetResourceReference(EditorDecorator.ContentTemplateProperty, DecoratorResourceKey);
    }
  }

  internal class CollectionElementRemovalDecoration : DynamicResourceEditorDecoration
  {
    public override bool AppliesTo(Node node)
    {
      return node is CollectionElement;
    }

    protected override object DecoratorResourceKey
    {
      get { return PropertyGrid.CollectionElementEditorKey; }
    }
  }

  /// <summary>
  /// A collection of <see cref="EditorDecoration"/> objects.
  /// </summary>
  public class EditorDecorationCollection : ObservableCollection<EditorDecoration>
  {
    //private readonly PropertyGrid _propertyGrid;

    ///// <summary>
    ///// Initializes a new instance of the <see cref="EditorCollection"/> class.
    ///// </summary>
    //public EditorDecorationCollection() { }

    //internal EditorDecorationCollection(PropertyGrid propertyGrid)
    //{
    //  _propertyGrid = propertyGrid;
    //}

    ///// <summary>
    ///// Inserts an item into the collection at the specified index.
    ///// </summary>
    ///// <param name="index">The zero-based index at which item should be inserted.</param>
    ///// <param name="item">The object to insert.</param>
    //protected override void InsertItem(int index, EditorDecoration item)
    //{
    //  item.ContainingGrid = _propertyGrid;
    //  base.InsertItem(index, item);
    //}
  }
}
