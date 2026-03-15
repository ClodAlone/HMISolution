using System.Collections.ObjectModel;
using System.Windows;
using System.Collections.Specialized;

namespace Mindscape.WpfElements.PropertyEditing
{
  /// <summary>
  /// Provides value editing services to a <see cref="PropertyGrid"/>.
  /// </summary>
  public abstract class Editor : DependencyObject
  {
    /// <summary>
    /// When overridden in a derived class, indicates whether the editor can edit the
    /// value of the specified node.
    /// </summary>
    /// <param name="node">The node.</param>
    /// <returns>true if the editor can edit the value of this node; otherwise false.</returns>
    public abstract bool CanEdit(Node node);

    /// <summary>
    /// Gets or sets the <see cref="DataTemplate"/> which the editor uses to present or edit
    /// node values.  This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="EditorTemplateProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public DataTemplate EditorTemplate
    {
      get { return (DataTemplate)GetValue(EditorTemplateProperty); }
      set { SetValue(EditorTemplateProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="EditorTemplate"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty EditorTemplateProperty =
        DependencyProperty.Register("EditorTemplate", typeof(DataTemplate), typeof(Editor));

    /// <summary>
    /// Gets or sets whether the user should be allowed to expand a node edited by this.
    /// editor.  This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <para>If false, the user cannot expand a node that is handled by this editor: all
    /// editing is done via the editor.  If true, the user can expand the node if it has
    /// subproperties: this allows the user to edit a record "one bit at a time" via the
    /// expansion or to edit it "as a whole" via this editor.  This is also useful for displaying
    /// a read-only summary even if all modifications must be done via subproperties.
    /// The default is false.</para>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="AllowExpandProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public bool AllowExpand
    {
      get { return (bool)GetValue(AllowExpandProperty); }
      set { SetValue(AllowExpandProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="AllowExpand"/> property.
    /// </summary>
    public static readonly DependencyProperty AllowExpandProperty =
        DependencyProperty.Register("AllowExpand", typeof(bool), typeof(Editor), new UIPropertyMetadata(false));

    /// <summary>
    /// Identifies the HostStyle attached property.
    /// </summary>
    public static readonly DependencyProperty HostStyleProperty =
        DependencyProperty.RegisterAttached("HostStyle", typeof(Style), typeof(Editor));

    /// <summary>
    /// Sets the value of the HostStyle attached property for a given <see cref="DependencyObject"/>.
    /// </summary>
    /// <param name="obj">The element on which to set the attached property.</param>
    /// <param name="value">The property value to set.</param>
    /// <remarks>The HostStyle property is used by internal elements of the editor template.  User code
    /// should not set the HostStyle property directly.</remarks>
    public static void SetHostStyle(DependencyObject obj, Style value)
    {
      obj.SetValue(HostStyleProperty, value);
    }

    /// <summary>
    /// Gets the value of the HostStyle attached property for a given <see cref="DependencyObject"/>.
    /// </summary>
    /// <param name="obj">The element from which to read the property value.</param>
    /// <returns>The value of the HostStyle attached property.</returns>
    public static Style GetHostStyle(DependencyObject obj)
    {
      return (Style)(obj.GetValue(HostStyleProperty));
    }
    
    /// <summary>
    /// When overridden in a derived class, constructs a <see cref="DataTemplate"/> which can
    /// be bound to the specified node in order to edit its value.
    /// </summary>
    /// <param name="node">The node to be edited.</param>
    /// <returns>A data template suitable for editing this node.</returns>
    public abstract DataTemplate BuildTemplate(Node node);

    internal IProvideEditorHosting ContainingGrid { get; set; }
  }

  /// <summary>
  /// A collection of <see cref="Editor"/> objects.
  /// </summary>
  public class EditorCollection : ObservableCollection<Editor>
  {
    private readonly IProvideEditorHosting _propertyGrid;

    /// <summary>
    /// Initializes a new instance of the <see cref="EditorCollection"/> class.
    /// </summary>
    public EditorCollection() { }

    internal EditorCollection(IProvideEditorHosting propertyGrid)
    {
      _propertyGrid = propertyGrid;
    }

    /// <summary>
    /// Inserts an item into the collection at the specified index.
    /// </summary>
    /// <param name="index">The zero-based index at which item should be inserted.</param>
    /// <param name="item">The object to insert.</param>
    protected override void InsertItem(int index, Editor item)
    {
      item.ContainingGrid = _propertyGrid;
      base.InsertItem(index, item);
    }
  }
}
