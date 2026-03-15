using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Markup;
using System;

namespace Mindscape.WpfElements.PropertyEditing
{
  /// <summary>
  /// Supports styling of built-in editors.
  /// </summary>
  [ContentProperty("Style")]
  public class BuiltInEditorStyle
  {
    /// <summary>
    /// Gets or sets the key of the built-in editor to the style will be applied.
    /// </summary>
    public object EditorKey { get; set; }

    /// <summary>
    /// Gets or sets the style to be applied to the built-in editor.
    /// </summary>
    public Style Style { get; set; }

    /*/// <summary>
    /// Gets or sets the key of the built-in editor to the style will be applied.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="EditorKeyProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public object EditorKey
    {
      get { return GetValue(EditorKeyProperty); }
      set { SetValue(EditorKeyProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="EditorKey"/> property.
    /// </summary>
    public static readonly DependencyProperty EditorKeyProperty =
        DependencyProperty.Register("EditorKey", typeof(object), typeof(BuiltInEditorStyle));

    /// <summary>
    /// Gets or sets the style to be applied to the built-in editor.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="StyleProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public Style Style
    {
      get { return (Style)GetValue(StyleProperty); }
      set { SetValue(StyleProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="Style"/> property.
    /// </summary>
    public static readonly DependencyProperty StyleProperty =
        FrameworkElement.StyleProperty.AddOwner(typeof(BuiltInEditorStyle));*/
  }

  /// <summary>
  /// Represents a collection of <see cref="BuiltInEditorStyle"/> objects.
  /// </summary>
  public class BuiltInEditorStyleCollection : ObservableCollection<BuiltInEditorStyle>
  {
    /// <summary>
    /// Gets a <see cref="BuiltInEditorStyleCollection"/> containing additional styles for this collection to use.
    /// </summary>
    public BuiltInEditorStyleCollection Include { get; set; }

    /// <summary>
    /// Finds the <see cref="Style"/> associated with the specified editor.
    /// </summary>
    /// <param name="editorKey">The key identifying the editor to search for.</param>
    /// <returns>The Style associated with the editor key in this collection if there is one;
    /// otherwise null.</returns>
    public Style FindStyle(object editorKey)
    {
      foreach (BuiltInEditorStyle editorStyle in this)
      {
        if (Object.Equals(editorStyle.EditorKey, editorKey))
        {
          return editorStyle.Style;
        }
      }
      if (Include != null)
      {
        return Include.FindStyle(editorKey);
      }

      return null;
    }
  }
}
