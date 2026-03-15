using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows;
using System.Linq;
using Mindscape.WpfElements.WpfPropertyGrid;

namespace Mindscape.WpfElements.PropertyEditing
{
  /// <summary>
  /// Represents an object which extends the repertoire of editors available to
  /// a <see cref="DataTemplateSelector"/>.
  /// </summary>
  /// <seealso cref="Editor"/>
  public interface IExtendInPlaceEditors
  {
    /// <summary>
    /// Gets the list of additional editors available for selection.
    /// </summary>
    EditorCollection Editors { get; }

    /// <summary>
    /// Gets the list of additional editor decorations available for selection.
    /// </summary>
    EditorDecorationCollection EditorDecorations { get; }

    /// <summary>
    /// Gets the list of styles available for customising built-in editors.
    /// </summary>
    BuiltInEditorStyleCollection BuiltInEditorStyles { get; }
  }

  /// <summary>
  /// Selects an editor for a value.
  /// </summary>
  public sealed class EditorSelector : DataTemplateSelector
  {
    private readonly IExtendInPlaceEditors _extensions;

    /// <summary>
    /// Initialises a new instance of the <see cref="EditorSelector"/> class.
    /// </summary>
    /// <param name="extensions">The <see cref="IExtendInPlaceEditors"/> which supplies additional
    /// editors for specific values or types.</param>
    public EditorSelector(IExtendInPlaceEditors extensions)
    {
      _extensions = extensions;
    }

    /// <summary>
    /// Iterates the list of additional editors.
    /// </summary>
    public IEnumerable<Editor> ExtendingEditors
    {
      get
      {
        if (_extensions == null || _extensions.Editors == null)
        {
          yield break;
        }
        foreach (Editor editor in _extensions.Editors)
        {
          yield return editor;
        }
      }
    }

    private BuiltInEditorStyleCollection ExtendingStyles
    {
      get
      {
        if (_extensions == null || _extensions.BuiltInEditorStyles == null)
        {
          return new BuiltInEditorStyleCollection();
        }
        return _extensions.BuiltInEditorStyles;
      }
    }

    private IEnumerable<EditorDecoration> ExtendingDecorations
    {
      get { return _extensions == null ? Enumerable.Empty<EditorDecoration>() : _extensions.EditorDecorations; }
    }

    /// <summary>
    /// Returns a <see cref="DataTemplate"/> suitable for editing the specified item.
    /// </summary>
    /// <param name="item">The data object for which to select the template.</param>
    /// <param name="container">The data-bound object.</param>
    /// <returns>The <see cref="DataTemplate"/> to be used to present and edit the item, if one
    /// was found; otherwise null.</returns>
    public override DataTemplate SelectTemplate(object item, DependencyObject container)
    {
      Node node = item as Node;

      if (node == null)
      {
        PropertyGridRow propertyGridRow = item as PropertyGridRow;
        if (propertyGridRow != null)
        {
          node = propertyGridRow.Node;
        }
      }

      if (node == null)
      {
        return null;
      }

      Editor editor = GetEditor(node);
      DataTemplate baseTemplate = editor.BuildTemplate(node);
      return EditorDecorations.Decorate(baseTemplate, node, item, ExtendingDecorations);
    }
    
    /// <summary>
    /// Gets the editing capabilities available to the <see cref="EditorSelector"/> for the 
    /// specified node.
    /// </summary>
    /// <param name="node">The node for which editing information is being requested.</param>
    /// <returns>An <see cref="InPlaceEditing"/> describing the edit capabilities.</returns>
    public InPlaceEditing GetEditSettings(Node node)
    {
      if (node.HasOwnInPlaceEditor)
      {
        return new InPlaceEditing(true, node.InPlaceEditor.AllowExpand);
      }

      foreach (Editor editor in ExtendingEditors)
      {
        if (editor.CanEdit(node))
        {
          return new InPlaceEditing(true, editor.AllowExpand);
        }
      }

      return BuiltInEditor.GetEditSettings(node);
    }

    /// <summary>
    /// Gets an <see cref="Editor"/> for the specified node.  This is used to determine 
    /// a <see cref="DataTemplate"/> for presentation and editing of the node value.
    /// </summary>
    /// <param name="node">The node for which an editor is being requested.</param>
    /// <returns>An editor capable of providing a data template for the node value.</returns>
    /// <remarks>If no in-place editor is available, this method returns an "editor" which will 
    /// render the value using a read-only text block.</remarks>
    public Editor GetEditor(Node node)
    {
      if (node.HasOwnInPlaceEditor)
      {
        return node.InPlaceEditor;
      }

      foreach (Editor editor in ExtendingEditors)
      {
        if (editor.CanEdit(node))
        {
          return editor;
        }
      }

      return new BuiltInEditor(ExtendingStyles);
    }
  }

  // TODO: Should this remain in the PropertyEditing namespace? or be moved back to the WpfPropertyGrid namespace.

  /// <summary>
  /// Describes the in-place editing capability available in a <see cref="PropertyGrid"/> for a node.
  /// </summary>
  public class InPlaceEditing
  {
    /// <summary>
    /// Initialises a new instance of the <see cref="InPlaceEditing"/> class.
    /// </summary>
    /// <param name="canEditInPlace">Indicates whether an in-place editor is available.</param>
    /// <param name="allowExpand">Indicates whether the grid should show the node as expandable.</param>
    public InPlaceEditing(bool canEditInPlace, bool allowExpand)
    {
      _canEditInPlace = canEditInPlace;
      _allowExpand = allowExpand;
    }

    private readonly bool _canEditInPlace;
    private readonly bool _allowExpand;

    /// <summary>
    /// Gets whether an in-place editor is available.
    /// </summary>
    public bool CanEditInPlace
    {
      get { return _canEditInPlace; }
    }

    /// <summary>
    /// Gets whether the grid should show the node as expandable.
    /// </summary>
    public bool AllowExpand
    {
      get { return _allowExpand; }
    }
  }
}