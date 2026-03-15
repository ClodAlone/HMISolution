using System;
using System.Windows.Controls;
using System.Windows.Input;
using System.ComponentModel;
using System.Windows.Data;
using System.Windows;
using System.Windows.Controls.Primitives;
using WpfTextBox = System.Windows.Controls.TextBox;
using Mindscape.WpfElements.PropertyEditing;

namespace Mindscape.WpfElements.WpfPropertyGrid
{
  /// <summary>
  /// A control which encapsulates common <see cref="PropertyGridBindingView"/> view manipulation code.
  /// </summary>
  [TemplatePart(Name=FilterTextPartName, Type=typeof(TextBox))]
  public class ViewCommandsToolBar : Control
  {
    private WpfTextBox _filterEditor;

    static ViewCommandsToolBar()
    {
      DefaultStyleKeyProperty.OverrideMetadata(typeof(ViewCommandsToolBar),
        new FrameworkPropertyMetadata(typeof(ViewCommandsToolBar)));
    }

    private const string FilterTextPartName = "PART_FilterText";

    private PropertyGroupDescription _categoryGrouper = new PropertyGroupDescription("Node", new NodeToCategoryConverter(), StringComparison.CurrentCultureIgnoreCase);
    private SortDescription _nameSorter = new SortDescription("Node.HumanName", ListSortDirection.Ascending);

    /// <summary>
    /// Called by the framework when a template is applied to the control.
    /// </summary>
    public override void OnApplyTemplate()
    {
      base.OnApplyTemplate();

      _filterEditor = GetTemplateChild(FilterTextPartName) as WpfTextBox;
      if (_filterEditor != null)
      {
        _filterEditor.TextChanged += new TextChangedEventHandler(FilterEditor_TextChanged);
      }
    }

    void FilterEditor_TextChanged(object sender, TextChangedEventArgs e)
    {
      if (Target != null)
      {
        string filter = (sender as WpfTextBox).Text;
        if (String.IsNullOrEmpty(filter))
        {
          Target.DefaultView.Filter = null;
        }
        else
        {
          Target.DefaultView.Filter = delegate(object obj)
          {
            Node node = ((PropertyGridRow)obj).Node;
            return node.Name.IndexOf(filter, StringComparison.CurrentCultureIgnoreCase) >= 0 ||
              node.HumanName.IndexOf(filter, StringComparison.CurrentCultureIgnoreCase) >= 0;
          };
        }
      }
    }

    /// <summary>
    /// A command for clearing the filter in a <see cref="ViewCommandsToolBar"/>.
    /// </summary>
    /// <remarks>The default toolbar does not provide a UI for this command.</remarks>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "Type is immutable")]
    public static readonly ICommand ClearFilter = new RoutedCommand("ClearFilter", typeof(ViewCommandsToolBar));

    /// <summary>
    /// A command for removing all sorting from a <see cref="PropertyGridBindingView"/>.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "Type is immutable")]
    public static readonly ICommand ShowUnsorted = new RoutedCommand("ShowUnsorted", typeof(ViewCommandsToolBar));

    /// <summary>
    /// A command for sorting a <see cref="PropertyGridBindingView"/> alphabetically by property name.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "Type is immutable")]
    public static readonly ICommand ShowAlphaSorted = new RoutedCommand("ShowAlphaSorted", typeof(ViewCommandsToolBar));

    /// <summary>
    /// A command for sorting a <see cref="PropertyGridBindingView"/> by category.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "Type is immutable")]
    public static readonly ICommand ShowCategorized = new RoutedCommand("ShowCategorized", typeof(ViewCommandsToolBar));

    /// <summary>
    /// Gets or sets the <see cref="PropertyGridBindingView"/> whose view is controlled by the toolbar.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="TargetProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification="Dependency property pattern")]
    public PropertyGridBindingView Target
    {
      get { return (PropertyGridBindingView)GetValue(TargetProperty); }
      set { SetValue(TargetProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="Target"/> property.
    /// </summary>
    public static readonly DependencyProperty TargetProperty =
        DependencyProperty.Register("Target", typeof(PropertyGridBindingView), typeof(ViewCommandsToolBar));

    /// <summary>
    /// Initialises a new instance of the <see cref="ViewCommandsToolBar"/> class.
    /// </summary>
    public ViewCommandsToolBar()
    {
      CommandBindings.Add(new CommandBinding(ShowUnsorted, ShowUnsorted_Executed, ShowUnsorted_CanExecute));
      CommandBindings.Add(new CommandBinding(ShowAlphaSorted, ShowAlphaSorted_Executed, ShowAlphaSorted_CanExecute));
      CommandBindings.Add(new CommandBinding(ShowCategorized, ShowCategorized_Executed, ShowCategorized_CanExecute));
      CommandBindings.Add(new CommandBinding(ClearFilter, ClearFilter_Executed, ClearFilter_CanExecute));
    }

    private void ShowUnsorted_CanExecute(object sender, CanExecuteRoutedEventArgs e)
    {
      e.CanExecute = (Target != null);
    }

    private void ShowAlphaSorted_CanExecute(object sender, CanExecuteRoutedEventArgs e)
    {
      e.CanExecute = (Target != null && Target.DefaultView.CanSort);
    }

    private void ShowCategorized_CanExecute(object sender, CanExecuteRoutedEventArgs e)
    {
      e.CanExecute = (Target != null && Target.DefaultView.CanGroup);
    }

    private void ClearFilter_CanExecute(object sender, CanExecuteRoutedEventArgs e)
    {
      e.CanExecute = (_filterEditor != null && !String.IsNullOrEmpty(_filterEditor.Text));
    }

    private void ClearAllViewDescriptions()
    {
      System.Diagnostics.Debug.Assert(Target != null);
      Target.DefaultView.SortDescriptions.Clear();
      Target.DefaultView.GroupDescriptions.Clear();
    }

    private void ShowUnsorted_Executed(object sender, ExecutedRoutedEventArgs e)
    {
      if (Target != null)
      {
        ClearAllViewDescriptions();
      }
    }

    private void ShowAlphaSorted_Executed(object sender, ExecutedRoutedEventArgs e)
    {
      if (Target != null)
      {
        ClearAllViewDescriptions();
        Target.DefaultView.SortDescriptions.Add(_nameSorter);
      }
    }

    private void ShowCategorized_Executed(object sender, ExecutedRoutedEventArgs e)
    {
      if (Target != null)
      {
        ClearAllViewDescriptions();
        Target.DefaultView.GroupDescriptions.Add(_categoryGrouper);
      }
    }

    private void ClearFilter_Executed(object sender, ExecutedRoutedEventArgs e)
    {
      _filterEditor.Text = String.Empty;
    }

    /// <summary>
    /// Gets the <see cref="ResourceKey"/> for the content of the "Search" label.
    /// </summary>
    public static ComponentResourceKey SearchLabelContentKey
    {
      get { return new ComponentResourceKey(typeof(ViewCommandsToolBar), "SearchLabelContent"); }
    }
  }
}
