using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Collections.ObjectModel;
using System.Threading;
using System.Windows.Threading;

namespace Mindscape.WpfElements.WpfDataGrid
{
  /// <summary>
  /// Maps an editor key to a <see cref="DataTemplate"/> used for displaying a data grid cell value.
  /// </summary>
  /// <remarks>This is used to override editor templates when creating themes for the grid; for
  /// mapping a type or property to an editor, use a TypeEditor or PropertyEditor declaration.</remarks>
  public class BuiltInDisplayTemplate
  {
    /// <summary>
    /// Gets or sets the editor key.
    /// </summary>
    public object EditorKey { get; set; }

    /// <summary>
    /// Gets or sets the <see cref="DataTemplate"/>.
    /// </summary>
    public DataTemplate Template { get; set; }

    /*#region EditorKey Property

    /// <summary>
    /// Gets or sets the editor key.
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
      DependencyProperty.Register("EditorKey", typeof(object), typeof(BuiltInDisplayTemplate));

    #endregion // EditorKey Property

    #region Template Property

    /// <summary>
    /// Gets or sets the <see cref="DataTemplate"/>.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="TemplateProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public DataTemplate Template
    {
      get { return (DataTemplate)GetValue(TemplateProperty); }
      set { SetValue(TemplateProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="Template"/> property.
    /// </summary>
    public static readonly DependencyProperty TemplateProperty =
      DependencyProperty.Register("Template", typeof(DataTemplate), typeof(BuiltInDisplayTemplate));

    #endregion // Template Property*/
  }

  /// <summary>
  /// A collection for mapping editor keys to display templates.
  /// </summary>
  public class BuiltInDisplayTemplateCollection : ObservableCollection<BuiltInDisplayTemplate>
  {
    /// <summary>
    /// Returns the display template for the given editor key.
    /// </summary>
    /// <param name="editorKey">The editor key.</param>
    /// <returns>The display template mapped to the given editor key.</returns>
    public DataTemplate FindTemplate(object editorKey)
    {
      DataTemplate template = null;
      foreach (BuiltInDisplayTemplate displayTemplate in this)
      {
        if (Object.Equals(displayTemplate.EditorKey, editorKey))
        {
          template = displayTemplate.Template;
          break;
        }
      }
      return template;
    }
  }
}
