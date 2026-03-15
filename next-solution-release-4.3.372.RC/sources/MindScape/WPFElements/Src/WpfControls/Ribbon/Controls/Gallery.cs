using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls.Primitives;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Collections;
using System.Windows.Data;

namespace Mindscape.WpfElements
{
  /// <summary>
  /// A control for displaying a categorized list of items that the user can select from.
  /// </summary>
  public class Gallery : ItemsControl
  {
    private GalleryPanel _panel;
    private GalleryItem _selectedGalleryItem;

    static Gallery()
    {
      DefaultStyleKeyProperty.OverrideMetadata(typeof(Gallery),
        new FrameworkPropertyMetadata(typeof(Gallery)));
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Gallery"/> class.
    /// </summary>
    public Gallery()
    {
      Loaded += new RoutedEventHandler(Gallery_Loaded);
      BindCommands();
    }

    private void Gallery_Loaded(object sender, RoutedEventArgs e)
    {
      _panel = VisualTreeUtils.GetChild<GalleryPanel>(this);
      SetValue(GalleryItemsPropertyKey, new GalleryEnumerator(Items));
      UpdateIsWithinPopup();
    }

    private void BindCommands()
    {
      CommandBindings.Add(new CommandBinding(GalleryCommands.NavigateUpCommand, NavigateUp_Execute, NavigateUp_CanExecute));
      CommandBindings.Add(new CommandBinding(GalleryCommands.NavigateDownCommand, NavigateDown_Execute, NavigateDown_CanExecute));
    }

    private void NavigateUp_CanExecute(object sender, CanExecuteRoutedEventArgs e)
    {
      if (_panel != null)
      {
        e.CanExecute = _panel.CanNavigateUp;
      }
    }

    private void NavigateUp_Execute(object sender, ExecutedRoutedEventArgs e)
    {
      if (_panel != null)
      {
        _panel.NavigateUp();
      }
    }

    private void NavigateDown_CanExecute(object sender, CanExecuteRoutedEventArgs e)
    {
      if (_panel != null)
      {
        e.CanExecute = _panel.CanNavigateDown;
      }
    }

    private void NavigateDown_Execute(object sender, ExecutedRoutedEventArgs e)
    {
      if (_panel != null)
      {
        _panel.NavigateDown();
      }
    }

    /// <summary>
    /// Called when the mouse wheel moves over this control.
    /// </summary>
    /// <param name="e">The event data.</param>
    protected override void OnMouseWheel(MouseWheelEventArgs e)
    {
      base.OnMouseWheel(e);

      if (!IsWithinPopup && _panel != null)
      {
        if (e.Delta > 0)
        {
          _panel.NavigateUp();
        }
        else
        {
          _panel.NavigateDown();
        }
        CommandManager.InvalidateRequerySuggested();
      }
    }

    /// <summary>
    /// Called when the ItemsSource property changes.
    /// </summary>
    /// <param name="oldValue">The old items source.</param>
    /// <param name="newValue">The new items source.</param>
    protected override void OnItemsSourceChanged(IEnumerable oldValue, IEnumerable newValue)
    {
      base.OnItemsSourceChanged(oldValue, newValue);
      SetValue(GalleryItemsPropertyKey, new GalleryEnumerator(Items));
    }

    /// <summary>
    /// Creates or identifies the element that is used to display the given item.
    /// </summary>
    /// <returns>The element that is used to display the given item.</returns>
    protected override DependencyObject GetContainerForItemOverride()
    {
      return new GalleryItem();
    }

    /// <summary>
    /// Determines if the specified item is (or is eligible to be) its own container.
    /// </summary>
    /// <param name="item">The item to check.</param>
    /// <returns>true if the item is (or is eligible to be) its own container; otherwise, false.</returns>
    protected override bool IsItemItsOwnContainerOverride(object item)
    {
      return item is GalleryItem || item is GalleryGroup;
    }

    internal void GalleryItem_IsSelectedChanged(GalleryItem item)
    {
      if (item.IsSelected)
      {
        if (_selectedGalleryItem != null)
        {
          _selectedGalleryItem.IsSelected = false;
        }
        _selectedGalleryItem = item;
        SelectedValue = item.DataContext;
      }
    }

    private bool _isUpdatingSelectionBinding;

    private void UpdateIsWithinPopup()
    {
      Popup popup = VisualTreeUtils.FindPopup(this);
      PopupMenu popupMenu = VisualTreeUtils.FindAncestor<PopupMenu>(this);
      if (popup != null && popupMenu != null)
      {
        SetValue(IsWithinPopupPropertyKey, true);
        Gallery host = VisualTreeUtils.FindAncestor<Gallery>(popup);
        if (host != null)
        {
          ItemsSource = host.Items; // TODO: should probably use bindings here.
          ItemWidth = host.ItemWidth;
          ItemHeight = host.ItemHeight;
          _isUpdatingSelectionBinding = true;
          Binding selectedValueBinding = new Binding("SelectedValue") { Source = host, Mode = BindingMode.TwoWay };
          BindingOperations.SetBinding(this, Gallery.SelectedValueProperty, selectedValueBinding);
          _isUpdatingSelectionBinding = false;
        }
      }
    }

    internal void UpdateActualItemSize(GalleryItem galleryItem)
    {
      bool canMeasure = false;
      if (GalleryItems != null && galleryItem != null)
      {
        foreach (object o in GalleryItems)
        {
          if (o != null && o.Equals(galleryItem.DataContext))
          {
            canMeasure = true;
          }
          if (o != null)
          {
            break; // We only want to check the first item, so break here at the first iteration.
          }
        }
      }

      if (canMeasure)
      {
        double itemWidth = ItemWidth;
        double itemHeight = ItemHeight;
        if (Double.IsNaN(itemWidth) || Double.IsNaN(itemHeight))
        {
          galleryItem.Measure(new Size(Double.PositiveInfinity, Double.PositiveInfinity));
          if (Double.IsNaN(itemWidth))
          {
            itemWidth = Math.Round(galleryItem.DesiredSize.Width);
          }
          if (Double.IsNaN(itemHeight))
          {
            itemHeight = Math.Round(galleryItem.DesiredSize.Height);
          }
          double padding = GetPadding(itemWidth, itemHeight);
          itemWidth += 2 * padding;
          itemHeight += 2 * padding;
        }
        SetActualItemWidth(itemWidth);
        SetActualItemHeight(itemHeight);
      }

      double p = GetPadding(ActualItemWidth, ActualItemHeight);
      galleryItem.Padding = new Thickness(p);
    }

    private double GetPadding(double currentWidth, double currentHeight)
    {
      if (currentHeight >= 40 || currentWidth >= 40)
      {
        return 4;
      }
      else
      {
        return 2;
      }
    }

    #region ActualItemWidth Property

    /// <summary>
    /// Gets the actual width of items displayed in this <see cref="Gallery"/>.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="ActualItemWidthProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public double ActualItemWidth
    {
      get { return (double)GetValue(ActualItemWidthProperty); }
    }

    private static readonly DependencyPropertyKey ActualItemWidthPropertyKey =
        DependencyProperty.RegisterReadOnly("ActualItemWidth", typeof(double), typeof(Gallery), new UIPropertyMetadata(0.0));

    /// <summary>
    /// Identifies the <see cref="ActualItemWidth"/> property.
    /// </summary>
    public static readonly DependencyProperty ActualItemWidthProperty =
        ActualItemWidthPropertyKey.DependencyProperty;

    internal void SetActualItemWidth(double actualItemWidth)
    {
      SetValue(ActualItemWidthPropertyKey, actualItemWidth);
    }

    #endregion // ActualItemWidth Property

    #region ActualItemHeight Property

    /// <summary>
    /// Gets the actual height of items displayed in this <see cref="Gallery"/>.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="ActualItemHeightProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public double ActualItemHeight
    {
      get { return (double)GetValue(ActualItemHeightProperty); }
    }

    private static readonly DependencyPropertyKey ActualItemHeightPropertyKey =
        DependencyProperty.RegisterReadOnly("ActualItemHeight", typeof(double), typeof(Gallery), new UIPropertyMetadata(0.0));

    /// <summary>
    /// Identifies the <see cref="ActualItemHeight"/> property.
    /// </summary>
    public static readonly DependencyProperty ActualItemHeightProperty =
        ActualItemHeightPropertyKey.DependencyProperty;

    internal void SetActualItemHeight(double actualItemHeight)
    {
      SetValue(ActualItemHeightPropertyKey, actualItemHeight);
    }

    #endregion // ActualItemHeight Property

    #region ItemWidth Property

    /// <summary>
    /// Gets or sets the desired width of items displayed in this <see cref="Gallery"/>.
    /// A value of Double.NaN will cause the <see cref="Gallery"/> to calculate the best item width.
    /// The default is Double.NaN.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="ItemWidthProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public double ItemWidth
    {
      get { return (double)GetValue(ItemWidthProperty); }
      set { SetValue(ItemWidthProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="ItemWidth"/> property.
    /// </summary>
    public static readonly DependencyProperty ItemWidthProperty =
      DependencyProperty.Register("ItemWidth", typeof(double), typeof(Gallery),
      new FrameworkPropertyMetadata(Double.NaN, OnItemWidthChanged));

    private static void OnItemWidthChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((Gallery)d).OnItemWidthChanged();
    }

    private void OnItemWidthChanged()
    {
    }

    #endregion // ItemWidth Property

    #region ItemHeight Property

    /// <summary>
    /// Gets or sets the desired height of items displayed in this <see cref="Gallery"/>.
    /// A value of Double.NaN will cause the <see cref="Gallery"/> to calculate the best item height.
    /// The default is Double.NaN.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="ItemHeightProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public double ItemHeight
    {
      get { return (double)GetValue(ItemHeightProperty); }
      set { SetValue(ItemHeightProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="ItemHeight"/> property.
    /// </summary>
    public static readonly DependencyProperty ItemHeightProperty =
      DependencyProperty.Register("ItemHeight", typeof(double), typeof(Gallery),
      new FrameworkPropertyMetadata(Double.NaN, OnItemHeightChanged));

    private static void OnItemHeightChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((Gallery)d).OnItemHeightChanged();
    }

    private void OnItemHeightChanged()
    {
    }

    #endregion // ItemHeight Property

    #region MinColumnCount Property

    /// <summary>
    /// Gets or sets the minimum number of columns this <see cref="Gallery"/> will use to arrange the items.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="MinColumnCountProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public int MinColumnCount
    {
      get { return (int)GetValue(MinColumnCountProperty); }
      set { SetValue(MinColumnCountProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="MinColumnCount"/> property.
    /// </summary>
    public static readonly DependencyProperty MinColumnCountProperty =
      DependencyProperty.Register("MinColumnCount", typeof(int), typeof(Gallery),
      new FrameworkPropertyMetadata(1, OnMinColumnCountChanged));

    private static void OnMinColumnCountChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((Gallery)d).OnMinColumnCountChanged();
    }

    private void OnMinColumnCountChanged()
    {
    }

    #endregion // MinColumnCount Property

    #region MaxColumnCount Property

    /// <summary>
    /// Gets or sets the maximum number of columns this <see cref="Gallery"/> will use to arrange the items.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="MaxColumnCountProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public int MaxColumnCount
    {
      get { return (int)GetValue(MaxColumnCountProperty); }
      set { SetValue(MaxColumnCountProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="MaxColumnCount"/> property.
    /// </summary>
    public static readonly DependencyProperty MaxColumnCountProperty =
      DependencyProperty.Register("MaxColumnCount", typeof(int), typeof(Gallery),
      new FrameworkPropertyMetadata(int.MaxValue, OnMaxColumnCountChanged));

    private static void OnMaxColumnCountChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((Gallery)d).OnMaxColumnCountChanged();
    }

    private void OnMaxColumnCountChanged()
    {
    }

    #endregion // MaxColumnCount Property

    #region IsWithinPopup Property

    /// <summary>
    /// Gets whether or not this <see cref="Gallery"/> is within a <see cref="Popup"/>.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="IsWithinPopupProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public bool IsWithinPopup
    {
      get { return (bool)GetValue(IsWithinPopupProperty); }
    }

    private static readonly DependencyPropertyKey IsWithinPopupPropertyKey =
        DependencyProperty.RegisterReadOnly("IsWithinPopup", typeof(bool), typeof(Gallery), new UIPropertyMetadata(false));

    /// <summary>
    /// Identifies the <see cref="IsWithinPopup"/> property.
    /// </summary>
    public static readonly DependencyProperty IsWithinPopupProperty =
        IsWithinPopupPropertyKey.DependencyProperty;

    #endregion // IsWithinPopup Property

    #region GalleryItems Property

    /// <summary>
    /// Gets the gallery items.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="GalleryItemsProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public IEnumerable GalleryItems
    {
      get { return (IEnumerable)GetValue(GalleryItemsProperty); }
    }

    private static readonly DependencyPropertyKey GalleryItemsPropertyKey =
        DependencyProperty.RegisterReadOnly("GalleryItems", typeof(IEnumerable), typeof(Gallery), new UIPropertyMetadata(null));

    /// <summary>
    /// Identifies the <see cref="GalleryItems"/> property.
    /// </summary>
    public static readonly DependencyProperty GalleryItemsProperty =
        GalleryItemsPropertyKey.DependencyProperty;

    #endregion // GalleryItems Property

    #region SelectedValue Property

    /// <summary>
    /// Gets or sets the selected item value.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="SelectedValueProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public object SelectedValue
    {
      get { return GetValue(SelectedValueProperty); }
      set { SetValue(SelectedValueProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="SelectedValue"/> property.
    /// </summary>
    public static readonly DependencyProperty SelectedValueProperty =
      DependencyProperty.Register("SelectedValue", typeof(object), typeof(Gallery),
      new FrameworkPropertyMetadata(OnSelectedValueChanged));

    private static void OnSelectedValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((Gallery)d).OnSelectedValueChanged(e);
    }

    private void OnSelectedValueChanged(DependencyPropertyChangedEventArgs e)
    {
      if (SelectedValue == null && _selectedGalleryItem != null)
      {
        _selectedGalleryItem.IsSelected = false;
        _selectedGalleryItem = null;
      }
      else
      {
        GalleryItem item = GetGalleryItem(SelectedValue);
        if (item != null)
        {
          item.IsSelected = true;
        }
      }

      if (_panel != null)
      {
        _panel.BringIntoView(SelectedValue);
      }

      if (!_isUpdatingSelectionBinding)
      {
        if (e.NewValue == null || !e.NewValue.Equals(e.OldValue))
        {
          EventHandler handler = SelectedValueChanged;
          if (handler != null)
          {
            handler(this, EventArgs.Empty);
          }
        }
      }
    }

    internal event EventHandler SelectedValueChanged;

    #endregion // SelectedValue Property

    private GalleryItem GetGalleryItem(object content)
    {
      if (_panel != null)
      {
        foreach (object o in _panel.Children)
        {
          GalleryItem galleryItem = o as GalleryItem;
          if (galleryItem != null && galleryItem.DataContext != null && (galleryItem.DataContext == content || galleryItem.DataContext.Equals(content)))
          {
            return galleryItem;
          }
        }
      }
      GalleryItem item = ItemContainerGenerator.ContainerFromItem(content) as GalleryItem;
      if (item == null)
      {
        foreach (object o in Items)
        {
          GalleryGroup group = o as GalleryGroup;
          if (group != null)
          {
            item = group.ItemContainerGenerator.ContainerFromItem(content) as GalleryItem;
            if (item != null)
            {
              break;
            }
          }
        }
      }
      return item;
    }
  }
}
