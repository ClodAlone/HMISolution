using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows;
using System.Collections.Specialized;
using System.Collections;
using System.ComponentModel;
using Infralution.Licensing;
using System.Windows.Threading;
using System.Reflection;

namespace Mindscape.WpfElements
{
  /// <summary>
  /// A control which allows a user to select between multiple content panes using a set
  /// of buttons, which can be collapsed to small icons depending on the amount of space
  /// available for the button stack.
  /// </summary>
  [TemplatePart(Name = ExpandedItemsPanelPartName, Type = typeof(CountLimitedPanel))]
  [TemplatePart(Name = CollapsedItemsPanelPartName, Type = typeof(SizeLimitedPanel))]
  [TemplatePart(Name = ThumbPartName, Type = typeof(Thumb))]
  [TemplatePart(Name = ContentHostPartName, Type = typeof(ContentPresenter))]
  [LicenseProvider(typeof(PublicEncryptedLicenseProvider))]
  public class OutlookBar : ItemsControl
  {
    private const string ExpandedItemsPanelPartName = "PART_ExpandedItemsPanel";
    private const string CollapsedItemsPanelPartName = "PART_CollapsedItemsPanel";
    private const string ThumbPartName = "PART_Thumb";
    private const string ContentHostPartName = "PART_ContentHost";

    private Thumb _thumb;
    private ContentPresenter _contentHost;
    private bool _loading;

    static OutlookBar()
    {
      DefaultStyleKeyProperty.OverrideMetadata(typeof(OutlookBar),
        new FrameworkPropertyMetadata(typeof(OutlookBar)));
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="OutlookBar"/> control.
    /// </summary>
    public OutlookBar()
    {
      // Licensing
      //new WpfElementsCore(Assembly.GetCallingAssembly());
      //LicenseHelper.Attach(this, Assembly.GetCallingAssembly());
      // End licensing

      _loading = true;
      Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(AfterLoaded));
    }

    private void AfterLoaded()
    {
      _loading = false;
    }

    /// <summary>
    /// Called by the framework when the control template is applied.
    /// </summary>
    public override void OnApplyTemplate()
    {
      base.OnApplyTemplate();

      _thumb = GetTemplateChild(ThumbPartName) as Thumb;
      if (_thumb != null)
      {
        _thumb.DragDelta += new DragDeltaEventHandler(Thumb_DragDelta);
      }
      _contentHost = GetTemplateChild(ContentHostPartName) as ContentPresenter;

      //SetupItems(Items);
      Dispatcher.BeginInvoke(new Action(SetupItems));

      Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(OnExpandedItemCountChanged));
    }

    private void SetupItems()
    {
      SetupItems(Items);
    }

    private void SetupItems(IList list)
    {
      foreach (object o in list)
      {
        OutlookBarItem item = o as OutlookBarItem;
        if (item != null)
        {
          AttachEventHandlers(item);
          if (item.IsSelected)
          {
            SetSelectedItem(item);
          }
          if (ItemTemplate != null)
          {
            item.HeaderTemplate = ItemTemplate;
          }
          if (ItemTemplateSelector != null)
          {
            item.HeaderTemplateSelector = ItemTemplateSelector;
          }
          if (CollapsedItemTemplate != null)
          {
            item.CollapsedHeaderTemplate = CollapsedItemTemplate;
          }
          if (CollapsedItemTemplateSelector != null)
          {
            item.CollapsedHeaderTemplateSelector = CollapsedItemTemplateSelector;
          }
          item.OnPropertyChanged("Content");
        }
      }
    }

    /// <summary>
    /// Called when the contents of the Items collection has changed.
    /// </summary>
    /// <param name="e">A <see cref="NotifyCollectionChangedEventArgs"/> containing the event data.</param>
    protected override void OnItemsChanged(NotifyCollectionChangedEventArgs e)
    {
      base.OnItemsChanged(e);

      if (e.NewItems != null)
      {
        SetupItems(e.NewItems);
      }
      if (e.OldItems != null)
      {
        foreach (object o in e.OldItems)
        {
          OutlookBarItem item = o as OutlookBarItem;
          if (item != null)
          {
            RemoveEventHandlers(item);
            item.IsSelected = false;
            if (item.Equals(SelectedItem))
            {
              SelectedItem = null; // This will allow the next item to become selected when ResetSelectedItem is called at the end of this method.
            }
          }
        }
      }

      Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(ResetSelectedItem));
    }

    private void RemoveEventHandlers(OutlookBarItem item)
    {
      item.IsSelectedChanged -= new RoutedEventHandler(Item_IsSelectedChanged);
      item.VisibilityChanged -= new EventHandler(Item_VisibilityChanged);
    }

    private void AttachEventHandlers(OutlookBarItem item)
    {
      item.IsSelectedChanged += new RoutedEventHandler(Item_IsSelectedChanged);
      item.VisibilityChanged += new EventHandler(Item_VisibilityChanged);
    }

    private void Item_VisibilityChanged(object sender, EventArgs e)
    {
      OutlookBarItem item = sender as OutlookBarItem;
      if (item.Visibility != Visibility.Visible)
      {
        item.IsSelectedChanged -= new RoutedEventHandler(Item_IsSelectedChanged);
        if (item == SelectedItem)
        {
          item.IsSelected = false;
          SelectedItem = null;
        }
      }
      else
      {
        if (SelectedItem != null)
        {
          item.IsSelected = false;
        }
        item.IsSelectedChanged += new RoutedEventHandler(Item_IsSelectedChanged);
      }
      ResetSelectedItem();
    }

    private void Item_IsSelectedChanged(object sender, RoutedEventArgs e)
    {
      OutlookBarItem item = sender as OutlookBarItem;
      if (item != null && item.IsSelected && item != SelectedItem)
      {
        SetSelectedItem(item);
      }
      else if (item != null && item == SelectedItem)
      {
        SelectedItem.IsSelected = true;
      }
    }

    private void ResetSelectedItem()
    {
      if (SelectedItem != null)
      {
        SetSelectedItem(SelectedItem);
      }
      else if (Items.Count > 0)
      {
        foreach (object o in Items)
        {
          OutlookBarItem item = o as OutlookBarItem;
          if (item != null && item.Visibility == Visibility.Visible)
          {
            SetSelectedItem(item);
            break;
          }
        }
      }
      if (SelectedItem == null)
      {
        if (_contentHost != null)
        {
          _contentHost.Content = null;
        }
        OnSelectedItemChanged();
      }
    }

    private void SetSelectedItem(OutlookBarItem item)
    {
      if (item != null)
      {
        OutlookBarItem oldSelectedItem = SelectedItem;
        SelectedItem = item;
        if (oldSelectedItem != null && oldSelectedItem != SelectedItem)
        {
          oldSelectedItem.IsSelected = false;
        }
        SelectedItem.IsSelected = true;
        if (_contentHost != null)
        {
          _contentHost.Content = SelectedItem.Content;
        }
      }
    }

    #region SelectedIndex Property

    /// <summary>
    /// Gets or sets the index of the selected item.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="SelectedIndexProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public int SelectedIndex
    {
      get { return (int)GetValue(SelectedIndexProperty); }
      set { SetValue(SelectedIndexProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="SelectedIndex"/> property.
    /// </summary>
    public static readonly DependencyProperty SelectedIndexProperty =
      DependencyProperty.Register("SelectedIndex", typeof(int), typeof(OutlookBar),
      new FrameworkPropertyMetadata(OnSelectedIndexChanged));

    private static void OnSelectedIndexChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((OutlookBar)d).OnSelectedIndexChanged();
    }

    private void OnSelectedIndexChanged()
    {
      if (SelectedIndex >= 0 && SelectedIndex < Items.Count)
      {
        SelectedItem = Items[SelectedIndex] as OutlookBarItem;
      }
      else
      {
        SelectedItem = null;
      }
    }

    #endregion // SelectedIndex Property

    #region SelectedItem Property

    /// <summary>
    /// Gets the selected <see cref="OutlookBarItem"/>.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="SelectedItemProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public OutlookBarItem SelectedItem
    {
      get { return (OutlookBarItem)GetValue(SelectedItemProperty); }
      private set
      {
        if (SelectedItem != value)
        {
          SetValue(SelectedItemPropertyKey, value);

          SelectedIndex = Items.IndexOf(SelectedItem);
          OnSelectedItemChanged();
        }
      }
    }

    private static readonly DependencyPropertyKey SelectedItemPropertyKey =
        DependencyProperty.RegisterReadOnly("SelectedItem", typeof(OutlookBarItem), typeof(OutlookBar), new UIPropertyMetadata(null));

    /// <summary>
    /// Identifies the <see cref="SelectedItem"/> property.
    /// </summary>
    public static readonly DependencyProperty SelectedItemProperty =
        SelectedItemPropertyKey.DependencyProperty;

    /// <summary>
    /// Raised when the <see cref="SelectedItem"/> property changes.
    /// </summary>
    public event RoutedEventHandler SelectedItemChanged;

    private void OnSelectedItemChanged()
    {
      RoutedEventHandler handler = SelectedItemChanged;
      if (handler != null)
      {
        handler(this, new RoutedEventArgs());
      }
    }

    #endregion // SelectedItem Property

    #region CollapsedItemTemplate Property

    /// <summary>
    /// Gets or sets the <see cref="DataTemplate"/> used to render the header of collapsed items.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="CollapsedItemTemplateProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public DataTemplate CollapsedItemTemplate
    {
      get { return (DataTemplate)GetValue(CollapsedItemTemplateProperty); }
      set { SetValue(CollapsedItemTemplateProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="CollapsedItemTemplate"/> property.
    /// </summary>
    public static readonly DependencyProperty CollapsedItemTemplateProperty =
      DependencyProperty.Register("CollapsedItemTemplate", typeof(DataTemplate), typeof(OutlookBar),
      new FrameworkPropertyMetadata(OnCollapsedItemTemplateChanged));

    private static void OnCollapsedItemTemplateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((OutlookBar)d).OnCollapsedItemTemplateChanged();
    }

    private void OnCollapsedItemTemplateChanged()
    {
    }

    #endregion // CollapsedItemTemplate Property

    #region CollapsedItemTemplateSelector Property

    /// <summary>
    /// Gets or sets the <see cref="DataTemplateSelector"/> used to render the header of collapsed items.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="CollapsedItemTemplateSelectorProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public DataTemplateSelector CollapsedItemTemplateSelector
    {
      get { return (DataTemplateSelector)GetValue(CollapsedItemTemplateSelectorProperty); }
      set { SetValue(CollapsedItemTemplateSelectorProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="CollapsedItemTemplateSelector"/> property.
    /// </summary>
    public static readonly DependencyProperty CollapsedItemTemplateSelectorProperty =
      DependencyProperty.Register("CollapsedItemTemplateSelector", typeof(DataTemplateSelector), typeof(OutlookBar),
      new FrameworkPropertyMetadata(OnCollapsedItemTemplateSelectorChanged));

    private static void OnCollapsedItemTemplateSelectorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((OutlookBar)d).OnCollapsedItemTemplateSelectorChanged();
    }

    private void OnCollapsedItemTemplateSelectorChanged()
    {
    }

    #endregion // CollapsedItemTemplateSelector Property

    private void Thumb_DragDelta(object sender, DragDeltaEventArgs e)
    {
      CountLimitedPanel panel = VisualTreeUtils.GetChild<CountLimitedPanel>(this);
      if (panel != null && panel.Name.Equals(ExpandedItemsPanelPartName))
      {
        int count = 0;
        UIElement lastElement = null;
        foreach (UIElement element in panel.Children)
        {
          if (count == ExpandedItemCount - (e.VerticalChange > 0 ? 1 : 0))
          {
            lastElement = element;
          }
          count++;
        }
        if (lastElement != null)
        {
          if (e.VerticalChange > 0 && e.VerticalChange > lastElement.DesiredSize.Height)
          {
            ExpandedItemCount = Math.Max(0, ExpandedItemCount - 1);
          }
          else if (e.VerticalChange < 0 && -e.VerticalChange > lastElement.DesiredSize.Height)
          {
            ExpandedItemCount = Math.Min(Items.Count, ExpandedItemCount + 1);
          }
        }
      }
    }

    #region ExpandedItemCount property

    /// <summary>
    /// Gets or sets the number of items to display as buttons.  The remaining items are displayed
    /// as small icons.
    /// This is a dependency property.
    /// </summary>
    public int ExpandedItemCount
    {
      get { return (int)GetValue(ExpandedItemCountProperty); }
      set { SetValue(ExpandedItemCountProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="ExpandedItemCount"/> property.
    /// </summary>
    public static readonly DependencyProperty ExpandedItemCountProperty =
      DependencyProperty.Register("ExpandedItemCount", typeof(int), typeof(OutlookBar),
      new PropertyMetadata(0, new PropertyChangedCallback(OnExpandedItemCountChanged)));

    private static void OnExpandedItemCountChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((OutlookBar)d).OnExpandedItemCountChanged();
    }

    private void OnExpandedItemCountChanged()
    {
      if (!_loading)
      {
        if (ExpandedItemCount < 0)
        {
          ExpandedItemCount = 0;
        }
        else if (ExpandedItemCount > Items.Count)
        {
          ExpandedItemCount = Items.Count;
        }
        else
        {
          CountLimitedPanel panel = VisualTreeUtils.GetChild<CountLimitedPanel>(this);
          if (panel != null && panel.Name.Equals(ExpandedItemsPanelPartName))
          {
            panel.VisibleItemCount = ExpandedItemCount;
          }
          SizeLimitedPanel collapsedPanel = VisualTreeUtils.GetChild<SizeLimitedPanel>(this);
          if (collapsedPanel != null && collapsedPanel.Name.Equals(CollapsedItemsPanelPartName))
          {
            collapsedPanel.StartIndex = ExpandedItemCount;
          }
        }
      }
    }

    #endregion // ExpandedItemCount property
  }
}
