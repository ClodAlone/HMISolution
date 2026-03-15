using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Diagnostics;
using Mindscape.WpfElements.PropertyEditing;
using System.ComponentModel;
using System.Windows.Media;

namespace Mindscape.WpfElements.WpfDataGrid
{
  /// <summary>
  /// Represents a column in a <see cref="DataGrid"/>.
  /// </summary>
  public class DataGridColumn : DependencyObject
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="DataGridColumn"/> class.
    /// </summary>
    public DataGridColumn()
    {
      IsAutoWidthDirty = true;
    }

    /// <summary>
    /// Gets or set the name of the property that the cells in this column are bound to.
    /// </summary>
    public string PropertyName { get; set; }

    /// <summary>
    /// Gets or sets a <see cref="Binding"/> that can be used to display data in this column.
    /// This is an alternative to setting PropertyName for more advanced cases.
    /// </summary>
    public Binding DisplayMemberBinding { get; set; }

    #region Filter Property

    /// <summary>
    /// Gets or sets the <see cref="IFilter"/> that provides filtering logic for this <see cref="DataGridColumn"/>.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="FilterProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public IFilter Filter
    {
      get { return (IFilter)GetValue(FilterProperty); }
      set { SetValue(FilterProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="Filter"/> property.
    /// </summary>
    public static readonly DependencyProperty FilterProperty =
      DependencyProperty.Register("Filter", typeof(IFilter), typeof(DataGridColumn),
      new FrameworkPropertyMetadata(OnFilterChanged));

    private static void OnFilterChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((DataGridColumn)d).OnFilterChanged();
    }

    private void OnFilterChanged()
    {
      EventHandler handler = FilterChanged;
      if (handler != null)
      {
        handler(this, EventArgs.Empty);
      }
    }

    #endregion // Filter Property

    internal event EventHandler FilterChanged;

    #region Width Property

    // TODO: should width be an integer instead? A double width value can cause pixel snapping issues.

    /// <summary>
    /// Gets or sets the width of this <see cref="DataGridColumn"/>.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="WidthProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public GridLength Width
    {
      get { return (GridLength)GetValue(WidthProperty); }
      set { SetValue(WidthProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="Width"/> property.
    /// </summary>
    public static readonly DependencyProperty WidthProperty =
      DependencyProperty.Register("Width", typeof(GridLength), typeof(DataGridColumn),
      new FrameworkPropertyMetadata(DataGrid.DefaultColumnWidth, OnWidthChanged));

    private static void OnWidthChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((DataGridColumn)d).OnWidthChanged(e);
    }

    private bool _widthChangeLock;
    private double? _actualOldValue;

    private bool _isLoaded = false;

    internal bool IsLoaded
    {
      get { return _isLoaded; }
      set
      {
        _isLoaded = value;
        if (Width.Value < MinWidth && IsLoaded && Width.IsAbsolute)
        {
          _widthChangeLock = true;
          Width = new GridLength(MinWidth);
        }
      }
    }

    private void OnWidthChanged(DependencyPropertyChangedEventArgs e)
    {
      //if (AllowResize)
      {
        if (Width.Value < MinWidth && IsLoaded && Width.IsAbsolute)
        {
          _actualOldValue = ((GridLength)e.OldValue).Value;
          _widthChangeLock = ((GridLength)e.OldValue).Value == MinWidth;
          Width = new GridLength(MinWidth);
        }
        else if (!_widthChangeLock)
        {
          if (!Width.IsAuto && !IsStarSizing)
          {
            SetValue(ActualWidthPropertyKey, Width.Value);
          }
          OnWidthChanged(new ColumnWidthChangedEventArgs(_actualOldValue == null ? ((GridLength)e.OldValue).Value : _actualOldValue.Value, ((GridLength)e.NewValue).Value));
          _actualOldValue = null;
        }
      }
      /*else if (!_widthChangeLock)
      {
        _widthChangeLock = true;
        Width = (double)e.OldValue;
      }*/
      _widthChangeLock = false;
    }

    /// <summary>
    /// Raised when the width of this <see cref="DataGridColumn"/> changes.
    /// </summary>
    public event EventHandler<ColumnWidthChangedEventArgs> WidthChanged;

    /// <summary>
    /// Raises the <see cref="WidthChanged"/> event.
    /// </summary>
    /// <param name="e">The event data.</param>
    protected virtual void OnWidthChanged(ColumnWidthChangedEventArgs e)
    {
      EventHandler<ColumnWidthChangedEventArgs> handler = WidthChanged;
      if (handler != null)
      {
        handler(this, e);
      }
    }

    #endregion // Width Property

    private double _autoWidth;

    internal double AutoWidth
    {
      get { return _autoWidth; }
      set
      {
        _autoWidth = Math.Min(MaxAutoWidth, value);
      }
    }

    internal bool IsAutoWidthDirty { get; set; }

    internal bool IsStarSizing
    {
      get { return Width.IsStar; }
    }

    internal double FillWeight
    {
      get { return IsStarSizing ? Width.Value : 0; }
    }

    #region ActualWidth Property

    /// <summary>
    /// Gets the actual rendered width of the column.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="ActualWidthProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public double ActualWidth
    {
      get { return (double)GetValue(ActualWidthProperty); }
    }

    private static readonly DependencyPropertyKey ActualWidthPropertyKey =
        DependencyProperty.RegisterReadOnly("ActualWidth", typeof(double), typeof(DataGridColumn),
        new UIPropertyMetadata(DataGrid.DefaultColumnWidth.Value, OnActualWidthChanged));

    /// <summary>
    /// Identifies the <see cref="ActualWidth"/> property.
    /// </summary>
    public static readonly DependencyProperty ActualWidthProperty = ActualWidthPropertyKey.DependencyProperty;

    internal void SetActualWidth(double width)
    {
      if (IsVisible)
      {
        SetValue(ActualWidthPropertyKey, width);
      }
      else
      {
        SetValue(ActualWidthPropertyKey, 0.0);
      }
    }

    private static void OnActualWidthChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((DataGridColumn)d).OnActualWidthChanged(e);
    }

    private void OnActualWidthChanged(DependencyPropertyChangedEventArgs e)
    {
      OnActualWidthChanged(new ColumnWidthChangedEventArgs((double)e.OldValue, (double)e.NewValue));
    }

    internal event EventHandler<ColumnWidthChangedEventArgs> ActualWidthChanged;

    private void OnActualWidthChanged(ColumnWidthChangedEventArgs e)
    {
      EventHandler<ColumnWidthChangedEventArgs> handler = ActualWidthChanged;
      if (handler != null)
      {
        handler(this, e);
      }
    }

    #endregion // ActualWidth Property

    #region MinWidth Property

    /// <summary>
    /// Gets or sets the minimum allowed width of this <see cref="DataGridColumn"/>.
    /// The default is 10 pixels.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="MinWidthProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public double MinWidth
    {
      get { return (double)GetValue(MinWidthProperty); }
      set { SetValue(MinWidthProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="MinWidth"/> property.
    /// </summary>
    public static readonly DependencyProperty MinWidthProperty =
      DependencyProperty.Register("MinWidth", typeof(double), typeof(DataGridColumn),
      new FrameworkPropertyMetadata(10.0, OnMinWidthChanged));

    private static void OnMinWidthChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((DataGridColumn)d).OnMinWidthChanged();
    }

    private void OnMinWidthChanged()
    {
      if (MinWidth < 0)
      {
        throw new InvalidOperationException("The minimum width of a column can not be negative.");
      }
      if (ActualWidth < MinWidth)
      {
        if (!Width.IsAuto)
        {
          Width = new GridLength(MinWidth);
        }
        if (Width.Value < MinWidth)
        {
          MinWidth = Width.Value;
        }
        SetValue(ActualWidthPropertyKey, Math.Max(MinWidth, ActualWidth));
      }
    }

    #endregion // MinWidth Property

    #region MaxAutoWidth Property

    /// <summary>
    /// Gets or sets the maximum auto width of this <see cref="DataGridColumn"/>.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="MaxAutoWidthProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public double MaxAutoWidth
    {
      get { return (double)GetValue(MaxAutoWidthProperty); }
      set { SetValue(MaxAutoWidthProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="MaxAutoWidth"/> property.
    /// </summary>
    public static readonly DependencyProperty MaxAutoWidthProperty =
      DependencyProperty.Register("MaxAutoWidth", typeof(double), typeof(DataGridColumn),
      new FrameworkPropertyMetadata(Double.MaxValue, OnMaxAutoWidthChanged));

    private static void OnMaxAutoWidthChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((DataGridColumn)d).OnMaxAutoWidthChanged();
    }

    private void OnMaxAutoWidthChanged()
    {
    }

    #endregion // MaxAutoWidth Property

    #region Header Property

    /// <summary>
    /// Gets or sets the object displayed in the column header.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="HeaderProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public object Header
    {
      get { return GetValue(HeaderProperty); }
      set { SetValue(HeaderProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="Header"/> property.
    /// </summary>
    public static readonly DependencyProperty HeaderProperty =
      DependencyProperty.Register("Header", typeof(object), typeof(DataGridColumn));

    #endregion // Header Property

    #region IsVisible Property

    /// <summary>
    /// Gets or sets whether or not this column is visible in the data grid.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="IsVisibleProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public bool IsVisible
    {
      get { return (bool)GetValue(IsVisibleProperty); }
      set { SetValue(IsVisibleProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="IsVisible"/> property.
    /// </summary>
    public static readonly DependencyProperty IsVisibleProperty =
      DependencyProperty.Register("IsVisible", typeof(bool), typeof(DataGridColumn),
      new FrameworkPropertyMetadata(true, OnIsVisibleChanged));

    private static void OnIsVisibleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((DataGridColumn)d).OnIsVisibleChanged();
    }

    private double _oldWidth;

    private void OnIsVisibleChanged()
    {
      if (!IsVisible)
      {
        _oldWidth = ActualWidth;
        SetValue(ActualWidthPropertyKey, 0.0);
      }
      else if (Width.IsAbsolute)
      {
        SetValue(ActualWidthPropertyKey, Width.Value);
      }
      else
      {
        // TODO: should rerun the auto or star sizing logic here instead.
        SetValue(ActualWidthPropertyKey, _oldWidth);
      }
      EventHandler handler = IsVisibleChanged;
      if (handler != null)
      {
        handler(this, EventArgs.Empty);
      }
    }

    internal event EventHandler IsVisibleChanged;

    #endregion // IsVisible Property

    internal bool IsHeaderAutoSet { get; set; }

    // TODO: sorting tests and sorting property tests.

    #region AllowSort Property

    /// <summary>
    /// Gets or sets the whether or not the user can sort this column.
    /// The default is true.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="AllowSortProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public bool AllowSort
    {
      get { return (bool)GetValue(AllowSortProperty); }
      set { SetValue(AllowSortProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="AllowSort"/> property.
    /// </summary>
    public static readonly DependencyProperty AllowSortProperty =
      DependencyProperty.Register("AllowSort", typeof(bool), typeof(DataGridColumn),
      new FrameworkPropertyMetadata(true, OnAllowSortChanged));

    private static void OnAllowSortChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((DataGridColumn)d).OnAllowSortChanged();
    }

    private void OnAllowSortChanged()
    {
    }

    #endregion // AllowSort Property

    #region AllowResize Property

    /// <summary>
    /// Gets or sets whether this column can be resized.
    /// The default is true.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="AllowResizeProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public bool AllowResize
    {
      get { return (bool)GetValue(AllowResizeProperty); }
      set { SetValue(AllowResizeProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="AllowResize"/> property.
    /// </summary>
    public static readonly DependencyProperty AllowResizeProperty =
      DependencyProperty.Register("AllowResize", typeof(bool), typeof(DataGridColumn),
      new FrameworkPropertyMetadata(true));

    #endregion // AllowResize Property

    #region AllowEditing Property

    /// <summary>
    /// Gets or sets whether the cells within this column can be edited.
    /// The default is true.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="AllowEditingProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public bool AllowEditing
    {
      get { return (bool)GetValue(AllowEditingProperty); }
      set { SetValue(AllowEditingProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="AllowEditing"/> property.
    /// </summary>
    public static readonly DependencyProperty AllowEditingProperty =
      DependencyProperty.Register("AllowEditing", typeof(bool), typeof(DataGridColumn),
      new FrameworkPropertyMetadata(true, OnAllowEditingChanged));

    private static void OnAllowEditingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((DataGridColumn)d).OnAllowEditingChanged();
    }

    /// <summary>
    /// Raised when the AllowEditing property changes.
    /// </summary>
    public event EventHandler AllowEditingChanged;

    private void OnAllowEditingChanged()
    {
      EventHandler handler = AllowEditingChanged;
      if (handler != null)
      {
        handler(this, EventArgs.Empty);
      }
    }

    #endregion // AllowEditing Property

    #region AllowGrouping Property

    /// <summary>
    /// Gets or sets the AllowGrouping.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="AllowGroupingProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public bool AllowGrouping
    {
      get { return (bool)GetValue(AllowGroupingProperty); }
      set { SetValue(AllowGroupingProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="AllowGrouping"/> property.
    /// </summary>
    public static readonly DependencyProperty AllowGroupingProperty =
      DependencyProperty.Register("AllowGrouping", typeof(bool), typeof(DataGridColumn),
      new FrameworkPropertyMetadata(true, OnAllowGroupingChanged));

    private static void OnAllowGroupingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((DataGridColumn)d).OnAllowGroupingChanged();
    }

    private void OnAllowGroupingChanged()
    {
      // TODO: if this column is currently grouped, and this property changes to false, it should probably be ungrouped.
    }

    #endregion // AllowGrouping Property

    #region AllowFiltering Property

    /// <summary>
    /// Gets or sets whether or not the <see cref="DataGridColumn"/> supports filtering. The default is true.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="AllowFilteringProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public bool AllowFiltering
    {
      get { return (bool)GetValue(AllowFilteringProperty); }
      set { SetValue(AllowFilteringProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="AllowFiltering"/> property.
    /// </summary>
    public static readonly DependencyProperty AllowFilteringProperty =
      DependencyProperty.Register("AllowFiltering", typeof(bool), typeof(DataGridColumn),
      new FrameworkPropertyMetadata(true, OnAllowFilteringChanged));

    private static void OnAllowFilteringChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((DataGridColumn)d).OnAllowFilteringChanged();
    }

    private void OnAllowFilteringChanged()
    {
    }

    #endregion // AllowFiltering Property

    #region FooterTemplate Property

    /// <summary>
    /// Gets or sets the <see cref="DataTemplate"/> used to customize the footer display for the <see cref="DataGridColumn"/>.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="FooterTemplateProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public DataTemplate FooterTemplate
    {
      get { return (DataTemplate)GetValue(FooterTemplateProperty); }
      set { SetValue(FooterTemplateProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="FooterTemplate"/> property.
    /// </summary>
    public static readonly DependencyProperty FooterTemplateProperty =
      DependencyProperty.Register("FooterTemplate", typeof(DataTemplate), typeof(DataGridColumn),
      new FrameworkPropertyMetadata(null, OnFooterTemplateChanged));

    private static void OnFooterTemplateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((DataGridColumn)d).OnFooterTemplateChanged();
    }

    private void OnFooterTemplateChanged()
    {
    }

    #endregion // FooterTemplate Property

    #region GroupHeaderCellTemplate Property

    /// <summary>
    /// Gets or sets the GroupHeaderCellTemplate.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="GroupHeaderCellTemplateProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public DataTemplate GroupHeaderCellTemplate
    {
      get { return (DataTemplate)GetValue(GroupHeaderCellTemplateProperty); }
      set { SetValue(GroupHeaderCellTemplateProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="GroupHeaderCellTemplate"/> property.
    /// </summary>
    public static readonly DependencyProperty GroupHeaderCellTemplateProperty =
      DependencyProperty.Register("GroupHeaderCellTemplate", typeof(DataTemplate), typeof(DataGridColumn),
      new FrameworkPropertyMetadata(null, OnGroupHeaderCellTemplateChanged));

    private static void OnGroupHeaderCellTemplateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((DataGridColumn)d).OnGroupHeaderCellTemplateChanged();
    }

    private void OnGroupHeaderCellTemplateChanged()
    {
    }

    #endregion // GroupHeaderCellTemplate Property

    #region FooterAggregate Property

    /// <summary>
    /// Gets or sets the <see cref="IAggregate"/> primarily used for displaying in the footer of the <see cref="DataGridColumn"/>.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="FooterAggregateProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public IAggregate FooterAggregate
    {
      get { return (IAggregate)GetValue(FooterAggregateProperty); }
      set { SetValue(FooterAggregateProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="FooterAggregate"/> property.
    /// </summary>
    public static readonly DependencyProperty FooterAggregateProperty =
      DependencyProperty.Register("FooterAggregate", typeof(IAggregate), typeof(DataGridColumn),
      new FrameworkPropertyMetadata(OnFooterAggregateChanged));

    private static void OnFooterAggregateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((DataGridColumn)d).OnFooterAggregateChanged();
    }

    private void OnFooterAggregateChanged()
    {
      // TODO
    }

    #endregion // FooterAggregate Property

    #region GroupAggregate Property

    /// <summary>
    /// Gets or sets the <see cref="IAggregate"/> used for displaying in the group headers.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="GroupAggregateProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public IAggregate GroupAggregate
    {
      get { return (IAggregate)GetValue(GroupAggregateProperty); }
      set { SetValue(GroupAggregateProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="GroupAggregate"/> property.
    /// </summary>
    public static readonly DependencyProperty GroupAggregateProperty =
      DependencyProperty.Register("GroupAggregate", typeof(IAggregate), typeof(DataGridColumn),
      new FrameworkPropertyMetadata(OnGroupAggregateChanged));

    private static void OnGroupAggregateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((DataGridColumn)d).OnGroupAggregateChanged();
    }

    private void OnGroupAggregateChanged()
    {
      // TODO
    }

    #endregion // GroupAggregate Property

    #region DisplayTemplate Property

    /// <summary>
    /// Gets or sets the <see cref="DataTemplate"/> used to display cell values.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="DisplayTemplateProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public DataTemplate DisplayTemplate
    {
      get { return (DataTemplate)GetValue(DisplayTemplateProperty); }
      set { SetValue(DisplayTemplateProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="DisplayTemplate"/> property.
    /// </summary>
    public static readonly DependencyProperty DisplayTemplateProperty =
      DependencyProperty.Register("DisplayTemplate", typeof(DataTemplate), typeof(DataGridColumn),
      new FrameworkPropertyMetadata(null, OnDisplayTemplateChanged));

    private static void OnDisplayTemplateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((DataGridColumn)d).OnDisplayTemplateChanged();
    }

    private void OnDisplayTemplateChanged()
    {
      OnTemplateChanged();
    }

    #endregion // DisplayTemplate Property

    #region DisplayTemplateSelector Property

    /// <summary>
    /// Gets or sets the <see cref="DataTemplateSelector"/> used to display cell values.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="DisplayTemplateSelectorProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public DataTemplateSelector DisplayTemplateSelector
    {
      get { return (DataTemplateSelector)GetValue(DisplayTemplateSelectorProperty); }
      set { SetValue(DisplayTemplateSelectorProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="DisplayTemplateSelector"/> property.
    /// </summary>
    public static readonly DependencyProperty DisplayTemplateSelectorProperty =
      DependencyProperty.Register("DisplayTemplateSelector", typeof(DataTemplateSelector), typeof(DataGridColumn),
      new FrameworkPropertyMetadata(null, OnDisplayTemplateSelectorChanged));

    private static void OnDisplayTemplateSelectorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((DataGridColumn)d).OnDisplayTemplateSelectorChanged();
    }

    private void OnDisplayTemplateSelectorChanged()
    {
      OnTemplateChanged();
    }

    #endregion // DisplayTemplateSelector Property

    #region EditorTemplate Property

    /// <summary>
    /// Gets or sets the <see cref="DataTemplate"/> used to edit cell values.
    /// This is a dependency property.
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
    /// Identifies the <see cref="EditorTemplate"/> property.
    /// </summary>
    public static readonly DependencyProperty EditorTemplateProperty =
      DependencyProperty.Register("EditorTemplate", typeof(DataTemplate), typeof(DataGridColumn),
      new FrameworkPropertyMetadata(null, OnEditorTemplateChanged));

    private static void OnEditorTemplateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((DataGridColumn)d).OnEditorTemplateChanged();
    }

    private void OnEditorTemplateChanged()
    {
      OnTemplateChanged();
    }

    #endregion // EditorTemplate Property

    #region EditorTemplateSelector Property

    /// <summary>
    /// Gets or sets the <see cref="DataTemplateSelector"/> used to edit cell values.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="EditorTemplateSelectorProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public DataTemplateSelector EditorTemplateSelector
    {
      get { return (DataTemplateSelector)GetValue(EditorTemplateSelectorProperty); }
      set { SetValue(EditorTemplateSelectorProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="EditorTemplateSelector"/> property.
    /// </summary>
    public static readonly DependencyProperty EditorTemplateSelectorProperty =
      DependencyProperty.Register("EditorTemplateSelector", typeof(DataTemplateSelector), typeof(DataGridColumn),
      new FrameworkPropertyMetadata(null, OnEditorTemplateSelectorChanged));

    private static void OnEditorTemplateSelectorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((DataGridColumn)d).OnEditorTemplateSelectorChanged();
    }

    private void OnEditorTemplateSelectorChanged()
    {
      OnTemplateChanged();
    }

    #endregion // EditorTemplateSelector Property

    #region GroupRowHeaderTemplate Property

    /// <summary>
    /// Gets or sets the <see cref="DataTemplate"/> used to display the group header of a <see cref="DataGridGroupingRow"/> generated by grouping this column.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="GroupRowHeaderTemplateProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public DataTemplate GroupRowHeaderTemplate
    {
      get { return (DataTemplate)GetValue(GroupRowHeaderTemplateProperty); }
      set { SetValue(GroupRowHeaderTemplateProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="GroupRowHeaderTemplate"/> property.
    /// </summary>
    public static readonly DependencyProperty GroupRowHeaderTemplateProperty =
      DependencyProperty.Register("GroupRowHeaderTemplate", typeof(DataTemplate), typeof(DataGridColumn));

    #endregion // GroupRowHeaderTemplate Property

    #region GroupRowHeaderTemplateSelector Property

    /// <summary>
    /// Gets or sets the <see cref="DataTemplateSelector"/> used to display the group header of a <see cref="DataGridGroupingRow"/> generated by grouping this column.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="GroupRowHeaderTemplateSelectorProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public DataTemplateSelector GroupRowHeaderTemplateSelector
    {
      get { return (DataTemplateSelector)GetValue(GroupRowHeaderTemplateSelectorProperty); }
      set { SetValue(GroupRowHeaderTemplateSelectorProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="GroupRowHeaderTemplateSelector"/> property.
    /// </summary>
    public static readonly DependencyProperty GroupRowHeaderTemplateSelectorProperty =
      DependencyProperty.Register("GroupRowHeaderTemplateSelector", typeof(DataTemplateSelector), typeof(DataGridColumn));

    #endregion // GroupRowHeaderTemplateSelector Property

    #region SortDirection Property

    /// <summary>
    /// Gets or sets the <see cref="SortDirection"/> of this column.
    /// The default is SortDirection.None.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="SortDirectionProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public SortDirection SortDirection
    {
      get { return (SortDirection)GetValue(SortDirectionProperty); }
      set { SetValue(SortDirectionProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="SortDirection"/> property.
    /// </summary>
    public static readonly DependencyProperty SortDirectionProperty =
      DependencyProperty.Register("SortDirection", typeof(SortDirection), typeof(DataGridColumn),
      new FrameworkPropertyMetadata(SortDirection.None, OnSortDirectionChanged));

    private static void OnSortDirectionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((DataGridColumn)d).OnSortDirectionChanged();
    }

    private void OnSortDirectionChanged()
    {
      EventHandler handler = SortDirectionChanged;
      if (handler != null)
      {
        handler(this, EventArgs.Empty);
      }
    }

    /// <summary>
    /// Raised when the sort direction of this column changes.
    /// </summary>
    public event EventHandler SortDirectionChanged;

    #endregion // SortDirection Property

    #region SortComparer property

    /// <summary>
    /// Gets or sets a custom comparer to be used when sorting on this column.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// The custom comparer is applied between the objects in the data source,
    /// not between the property values in this column.
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="SortComparerProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public IComparer<object> SortComparer
    {
      get { return (IComparer<object>)GetValue(SortComparerProperty); }
      set { SetValue(SortComparerProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="SortComparer"/> property.
    /// </summary>
    public static readonly DependencyProperty SortComparerProperty =
      DependencyProperty.Register("SortComparer", typeof(IComparer<object>), typeof(DataGridColumn),
      new FrameworkPropertyMetadata(OnSortComparerChanged));

    private static void OnSortComparerChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((DataGridColumn)d).OnSortComparerChanged(e);
    }

    private void OnSortComparerChanged(DependencyPropertyChangedEventArgs e)
    {
      EventHandler handler = SortDirectionChanged;
      if (handler != null)
      {
        handler(this, EventArgs.Empty);
      }
    }

    #endregion

    #region Tag Property

    /// <summary>
    /// Gets or sets the object that contains data to associate with the <see cref="DataGridColumn"/>.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="TagProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public object Tag
    {
      get { return GetValue(TagProperty); }
      set { SetValue(TagProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="Tag"/> property.
    /// </summary>
    public static readonly DependencyProperty TagProperty =
      DependencyProperty.Register("Tag", typeof(object), typeof(DataGridColumn));

    #endregion // Tag Property

    #region Background Property

    /// <summary>
    /// Gets or sets the background brush of all the cells in this column.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="BackgroundProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public Brush Background
    {
      get { return (Brush)GetValue(BackgroundProperty); }
      set { SetValue(BackgroundProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="Background"/> property.
    /// </summary>
    public static readonly DependencyProperty BackgroundProperty =
      DependencyProperty.Register("Background", typeof(Brush), typeof(DataGridColumn),
      new FrameworkPropertyMetadata(Brushes.Transparent, OnBackgroundChanged));

    private static void OnBackgroundChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((DataGridColumn)d).OnBackgroundChanged();
    }

    private void OnBackgroundChanged()
    {
      EventHandler handler = BackgroundChanged;
      if (handler != null)
      {
        handler(this, EventArgs.Empty);
      }
    }

    internal event EventHandler BackgroundChanged;

    #endregion // Background Property

    #region Foreground Property

    /// <summary>
    /// Gets or sets the foreground brush of all the cells in this column.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="ForegroundProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public Brush Foreground
    {
      get { return (Brush)GetValue(ForegroundProperty); }
      set { SetValue(ForegroundProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="Foreground"/> property.
    /// </summary>
    public static readonly DependencyProperty ForegroundProperty =
      DependencyProperty.Register("Foreground", typeof(Brush), typeof(DataGridColumn),
      new FrameworkPropertyMetadata(null, OnForegroundChanged));

    private static void OnForegroundChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((DataGridColumn)d).OnForegroundChanged();
    }

    private void OnForegroundChanged()
    {
      EventHandler handler = ForegroundChanged;
      if (handler != null)
      {
        handler(this, EventArgs.Empty);
      }
    }

    internal event EventHandler ForegroundChanged;

    #endregion // Foreground Property

    #region IsAlwaysInEditMode Property

    /// <summary>
    /// Gets or sets whether or not the cells in this column are always in edit mode.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="IsAlwaysInEditModeProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public bool IsAlwaysInEditMode
    {
      get { return (bool)GetValue(IsAlwaysInEditModeProperty); }
      set { SetValue(IsAlwaysInEditModeProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="IsAlwaysInEditMode"/> property.
    /// </summary>
    public static readonly DependencyProperty IsAlwaysInEditModeProperty =
      DependencyProperty.Register("IsAlwaysInEditMode", typeof(bool), typeof(DataGridColumn),
      new FrameworkPropertyMetadata(false, OnIsAlwaysInEditModeChanged));

    private static void OnIsAlwaysInEditModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((DataGridColumn)d).OnIsAlwaysInEditModeChanged();
    }

    private void OnIsAlwaysInEditModeChanged()
    {
      
    }

    #endregion // IsAlwaysInEditMode Property

    internal event EventHandler TemplateChanged;

    private void OnTemplateChanged()
    {
      EventHandler handler = TemplateChanged;
      if (handler != null)
      {
        handler(this, EventArgs.Empty);
      }
    }

    /// <summary>
    /// Gets the property info of the <see cref="DataGridColumn"/>.
    /// </summary>
    public IPropertyInfo PropertyInfo { get; internal set; }

    private bool _useDisplayTemplate = true;
    private bool _useEditorTemplateSelector = true;

    internal bool UseDisplayTemplate
    {
      get { return _useDisplayTemplate; }
      set { _useDisplayTemplate = value; }
    }

    internal bool UseEditorTemplateSelector
    {
      get { return _useEditorTemplateSelector; }
      set { _useEditorTemplateSelector = value; }
    }
  }

  internal class DataGridColumnInfo
  {
    public DataGridColumn Column { get; set; }
    public IPropertyInfo PropertyInfo { get; set; }

    public string SafePropertyDisplayName
    {
      get
      {
        if (PropertyInfo != null)
        {
          return PropertyInfo.DisplayName;
        }
        if (Column != null)
        {
          return Column.PropertyName;
        }
        return String.Empty;
      }
    }
  }

  internal static class GridViewColumnExtensions
  {
    private class DefaultingConverter : IValueConverter
    {
      public object DefaultValue { get; set; }

      public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
      {
        return value ?? DefaultValue;
      }

      public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
      {
        throw new NotImplementedException();
      }
    }

    internal static void Bind(this GridViewColumn target, DependencyProperty targetProperty, DataGridColumn source, DependencyProperty sourceProperty, object defaultValue)
    {
      if (source == null)
      {
        target.SetValue(targetProperty, defaultValue);
      }
      else
      {
        BindingOperations.SetBinding(target,
          targetProperty,
          new Binding { Source = source, Path = new PropertyPath(sourceProperty), Converter = new DefaultingConverter { DefaultValue = defaultValue } });
      }
    }
  }
}
