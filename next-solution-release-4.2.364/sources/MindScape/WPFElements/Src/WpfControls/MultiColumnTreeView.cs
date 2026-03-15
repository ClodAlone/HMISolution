using System.Windows;
using System.Windows.Controls;
using System.Collections.Specialized;
using System.ComponentModel;
using Infralution.Licensing;
using System.Windows.Media;
using System.Collections.Generic;
using System.Reflection;

namespace Mindscape.WpfElements
{
  /// <summary>
  /// A control with the expand/collapse functionality of a TreeView, but supporting
  /// multiple columns like a ListView.
  /// </summary>
  [StyleTypedProperty(Property = "ItemContainerStyle", StyleTargetType = typeof(MulticolumnTreeViewItem))]
  [StyleTypedPropertyAttribute(Property = "ColumnHeaderContainerStyle", StyleTargetType = typeof(GridViewColumnHeader))]
  [LicenseProvider(typeof(PublicEncryptedLicenseProvider))]
  public class MulticolumnTreeView : TreeView
  {
    static MulticolumnTreeView()
    {
      DefaultStyleKeyProperty.OverrideMetadata(typeof(MulticolumnTreeView),
        new FrameworkPropertyMetadata(typeof(MulticolumnTreeView)));
    }

    /// <summary>
    /// Initialises a new instance of the <see cref="MulticolumnTreeView"/> class.
    /// </summary>
    public MulticolumnTreeView()
    {
      // Licensing
      //new WpfElementsCore(Assembly.GetCallingAssembly());
      //LicenseHelper.Attach(this, Assembly.GetCallingAssembly());
      // End licensing
      
      Columns = new GridViewColumnCollection();
    }

    #region Dependency property declarations

    /// <summary>
    /// Gets or sets the columns in the <see cref="MulticolumnTreeView"/>.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="ColumnsProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>NotDataBindable</td></tr>
    /// </table>
    /// </remarks>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "Following the normal pattern for dependency properties")]
    public GridViewColumnCollection Columns
    {
      get { return (GridViewColumnCollection)GetValue(ColumnsProperty); }
      set { SetValue(ColumnsProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="Columns"/> property.
    /// </summary>
    public static readonly DependencyProperty ColumnsProperty =
        DependencyProperty.Register("Columns", typeof(GridViewColumnCollection),
        typeof(MulticolumnTreeView),
        new FrameworkPropertyMetadata(
          null,
          FrameworkPropertyMetadataOptions.NotDataBindable,
          OnColumnsChanged));


    /// <summary>
    /// Gets or sets the column header container style.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="ColumnHeaderContainerStyleProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public Style ColumnHeaderContainerStyle
    {
      get { return (Style)GetValue(ColumnHeaderContainerStyleProperty); }
      set { SetValue(ColumnHeaderContainerStyleProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="ColumnHeaderContainerStyle"/> property.
    /// </summary>
    public static readonly DependencyProperty ColumnHeaderContainerStyleProperty =
      GridView.ColumnHeaderContainerStyleProperty.AddOwner(typeof(MulticolumnTreeView));

    /// <summary>
    /// Gets or sets the column header template.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="ColumnHeaderTemplateProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public DataTemplate ColumnHeaderTemplate
    {
      get { return (DataTemplate)GetValue(ColumnHeaderTemplateProperty); }
      set { SetValue(ColumnHeaderTemplateProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="ColumnHeaderTemplate"/> property.
    /// </summary>
    public static readonly DependencyProperty ColumnHeaderTemplateProperty =
      GridView.ColumnHeaderTemplateProperty.AddOwner(typeof(MulticolumnTreeView));

    /// <summary>
    /// Gets or sets the column header template selector.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="ColumnHeaderTemplateSelectorProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public DataTemplateSelector ColumnHeaderTemplateSelector
    {
      get { return (DataTemplateSelector)GetValue(ColumnHeaderTemplateSelectorProperty); }
      set { SetValue(ColumnHeaderTemplateSelectorProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="ColumnHeaderTemplateSelector"/> property.
    /// </summary>
    public static readonly DependencyProperty ColumnHeaderTemplateSelectorProperty =
      GridView.ColumnHeaderTemplateSelectorProperty.AddOwner(typeof(MulticolumnTreeView));


    /// <summary>
    /// Gets or sets the a context menu for the grid column headers.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="ColumnHeaderContextMenuProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public ContextMenu ColumnHeaderContextMenu
    {
      get { return (ContextMenu)GetValue(ColumnHeaderContextMenuProperty); }
      set { SetValue(ColumnHeaderContextMenuProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="ColumnHeaderContextMenu"/> property.
    /// </summary>
    public static readonly DependencyProperty ColumnHeaderContextMenuProperty =
      DependencyProperty.Register("ColumnHeaderContextMenu", typeof(ContextMenu), typeof(MulticolumnTreeView));

    /// <summary>
    /// Gets or sets the decorator applied to cells used for expanding the tree.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="ExpandingDecoratorProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>NotDataBindable</td></tr>
    /// </table>
    /// </remarks>
    public DataTemplate ExpandingDecorator
    {
      get { return (DataTemplate)GetValue(ExpandingDecoratorProperty); }
      set { SetValue(ExpandingDecoratorProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="ExpandingDecorator"/> property.
    /// </summary>
    public static readonly DependencyProperty ExpandingDecoratorProperty =
        DependencyProperty.Register("ExpandingDecorator", typeof(DataTemplate), typeof(MulticolumnTreeView),
        new FrameworkPropertyMetadata(
          null,
          FrameworkPropertyMetadataOptions.NotDataBindable,
          OnExpandingDecoratorChanged));

    /// <summary>
    /// Gets the grid columns, wrapped to provide a tree expansion UI.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="WrappedColumnsProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public GridViewColumnCollection WrappedColumns
    {
      get { return (GridViewColumnCollection)GetValue(WrappedColumnsProperty); }
    }

    private static readonly DependencyPropertyKey WrappedColumnsPropertyKey =
      DependencyProperty.RegisterReadOnly("WrappedColumns", typeof(GridViewColumnCollection),
      typeof(MulticolumnTreeView), new FrameworkPropertyMetadata());

    /// <summary>
    /// Identifies the <see cref="WrappedColumns"/> property.
    /// </summary>
    public static readonly DependencyProperty WrappedColumnsProperty =
      WrappedColumnsPropertyKey.DependencyProperty;

    /// <summary>
    /// Gets or sets a value that indicates whether columns can change
    /// positions.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="AllowsColumnReorderProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public bool AllowsColumnReorder
    {
      get { return (bool)GetValue(AllowsColumnReorderProperty); }
      set { SetValue(AllowsColumnReorderProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="AllowsColumnReorder"/> property.
    /// </summary>
    public static readonly DependencyProperty AllowsColumnReorderProperty =
      DependencyProperty.Register("AllowsColumnReorder", typeof(bool), typeof(MulticolumnTreeView),
      new FrameworkPropertyMetadata(false));

    #endregion

    #region Dependency property change notification dispatchers

    private static void OnColumnsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((MulticolumnTreeView)d).OnColumnsChanged();
    }

    private static void OnExpandingDecoratorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((MulticolumnTreeView)d).OnExpandingDecoratorChanged();
    }

    #endregion

    #region Dependency property change notification handlers

    private void OnColumnsChanged()
    {
      Columns.CollectionChanged += new NotifyCollectionChangedEventHandler(Columns_CollectionChanged);
      WrapColumn0WithExpander();
    }

    private void OnExpandingDecoratorChanged()
    {
      WrapColumn0WithExpander();
    }

    #endregion

    #region Expander column management

    private void Columns_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
      WrapColumn0WithExpander();
    }

    private void WrapColumn0WithExpander()
    {
      GridViewColumnCollection cc = MulticolumnTreeViewColumn.CreateFrom(Columns, ExpandingDecorator);
      SetValue(WrappedColumnsPropertyKey, cc);
      cc.CollectionChanged += new NotifyCollectionChangedEventHandler(WrapperColumnsCollectionChanged);
    }

    private void WrapperColumnsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
      WrappedColumns.CollectionChanged -= new NotifyCollectionChangedEventHandler(WrapperColumnsCollectionChanged);

      Columns.Move(e.OldStartingIndex, e.NewStartingIndex);
    }

    #endregion

    #region Resource keys

    /// <summary>
    /// Gets the <see cref="ResourceKey"/> for the default <see cref="Style"/> of an
    /// expand/collapse toggle button.
    /// </summary>
    public static object ExpandCollapseToggleStyleKey
    {
      get { return new ComponentResourceKey(typeof(MulticolumnTreeView), "ExpandCollapseToggleStyle"); }
    }

    /// <summary>
    /// Gets the <see cref="ResourceKey"/> for the default <see cref="Style"/> of the ScrollViewer
    /// used to scroll the view.
    /// </summary>
    public static object ScrollViewerStyleKey
    {
      get { return new ComponentResourceKey(typeof(MulticolumnTreeView), "ScrollViewerStyle"); }
    }

    /// <summary>
    /// Gets the <see cref="ResourceKey"/> for the default <see cref="DataTemplate"/> of an
    /// cell in a column containing expand/collapse toggle buttons.
    /// </summary>
    public static object ExpandingCellTemplateKey
    {
      get { return new ComponentResourceKey(typeof(MulticolumnTreeView), "ExpandingCellTemplate"); }
    }

    #endregion

    #region Override item type

    /// <summary>
    /// Creates or identifies the element that is used to display the given item.
    /// </summary>
    /// <returns>The element that is used to display the given item.</returns>
    protected override DependencyObject GetContainerForItemOverride()
    {
      return new MulticolumnTreeViewItem();
    }

    /// <summary>
    /// Determines if the specified item is (or is eligible to be) its own container.
    /// </summary>
    /// <param name="item">The item to check.</param>
    /// <returns>true if the item is (or is eligible to be) its own container; otherwise, false.</returns>
    protected override bool IsItemItsOwnContainerOverride(object item)
    {
      return item is MulticolumnTreeViewItem;
    }

    #endregion
  }
}