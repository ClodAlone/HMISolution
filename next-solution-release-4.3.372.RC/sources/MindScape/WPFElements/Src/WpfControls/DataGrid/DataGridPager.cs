using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Threading;

namespace Mindscape.WpfElements.WpfDataGrid
{
  /// <summary>
  /// A control for allowing the user to navigate through pages of data in a <see cref="DataGrid"/>.
  /// </summary>
  public class DataGridPager : Control
  {
    private bool _unloaded;

    static DataGridPager()
    {
      DefaultStyleKeyProperty.OverrideMetadata(typeof(DataGridPager), new FrameworkPropertyMetadata(typeof(DataGridPager)));
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DataGridPager"/> class.
    /// </summary>
    public DataGridPager()
    {
      Loaded += new RoutedEventHandler(DataGridPager_Loaded);
      Unloaded += new RoutedEventHandler(DataGridPager_Unloaded);
    }

    private void DataGridPager_Loaded(object sender, RoutedEventArgs e)
    {
      if (_unloaded)
      {
        if (DataGridItemsSource != null)
        {
          DataGridItemsSource.PageIndexChanged += new EventHandler(DataGridItemsSource_PageIndexChanged);
          DataGridItemsSource.PropertyChanged += new PropertyChangedEventHandler(DataGridItemsSource_PropertyChanged);
        }
        _unloaded = false;
      }
    }

    private void DataGridPager_Unloaded(object sender, RoutedEventArgs e)
    {
      if (DataGridItemsSource != null)
      {
        DataGridItemsSource.PageIndexChanged -= new EventHandler(DataGridItemsSource_PageIndexChanged);
        DataGridItemsSource.PropertyChanged -= new PropertyChangedEventHandler(DataGridItemsSource_PropertyChanged);
      }
      _unloaded = true;
    }

    #region DataGridItemsSource Property

    /// <summary>
    /// Gets or sets the <see cref="DataGridItemsSource"/> of the <see cref="DataGrid"/>.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="DataGridItemsSourceProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public DataGridItemsSource DataGridItemsSource
    {
      get { return (DataGridItemsSource)GetValue(DataGridItemsSourceProperty); }
      set { SetValue(DataGridItemsSourceProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="DataGridItemsSource"/> property.
    /// </summary>
    public static readonly DependencyProperty DataGridItemsSourceProperty =
      DependencyProperty.Register("DataGridItemsSource", typeof(DataGridItemsSource), typeof(DataGridPager),
      new FrameworkPropertyMetadata(OnDataGridItemsSourceChanged));

    private static void OnDataGridItemsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((DataGridPager)d).OnDataGridItemsSourceChanged(e);
    }

    private void OnDataGridItemsSourceChanged(DependencyPropertyChangedEventArgs e)
    {
      DataGridItemsSource oldSource = e.OldValue as DataGridItemsSource;
      if (oldSource != null)
      {
        oldSource.PageIndexChanged -= new EventHandler(DataGridItemsSource_PageIndexChanged);
        oldSource.PropertyChanged -= new PropertyChangedEventHandler(DataGridItemsSource_PropertyChanged);
      }
      if (DataGridItemsSource != null)
      {
        DataGridItemsSource.PageIndexChanged += new EventHandler(DataGridItemsSource_PageIndexChanged);
        DataGridItemsSource.PropertyChanged += new PropertyChangedEventHandler(DataGridItemsSource_PropertyChanged);
        PageIndex = DataGridItemsSource.PageIndex;
        UpdatePagerButtons();
      }
    }

    private void DataGridItemsSource_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      if ("PageCount".Equals(e.PropertyName))
      {
        UpdatePagerButtons();
      }
    }

    private void DataGridItemsSource_PageIndexChanged(object sender, EventArgs e)
    {
      PageIndex = DataGridItemsSource.PageIndex;
    }

    #endregion // DataGridItemsSource Property

    #region PageIndex Property

    /// <summary>
    /// Gets or sets the index of the currently displayed page.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="PageIndexProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public int PageIndex
    {
      get { return (int)GetValue(PageIndexProperty); }
      set { SetValue(PageIndexProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="PageIndex"/> property.
    /// </summary>
    public static readonly DependencyProperty PageIndexProperty =
      DependencyProperty.Register("PageIndex", typeof(int), typeof(DataGridPager),
      new FrameworkPropertyMetadata(OnPageIndexChanged));

    private static void OnPageIndexChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((DataGridPager)d).OnPageIndexChanged();
    }

    private void OnPageIndexChanged()
    {
      if (DataGridItemsSource != null)
      {
        DataGridItemsSource.PageIndex = PageIndex;
        PageIndex = DataGridItemsSource.PageIndex;

        UpdatePagerButtons();
      }
    }

    #endregion // PageIndex Property

    #region MaxPagerButtonCount Property

    /// <summary>
    /// Gets or sets the maximum number of pager buttons to display.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="MaxPagerButtonCountProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public int MaxPagerButtonCount
    {
      get { return (int)GetValue(MaxPagerButtonCountProperty); }
      set { SetValue(MaxPagerButtonCountProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="MaxPagerButtonCount"/> property.
    /// </summary>
    public static readonly DependencyProperty MaxPagerButtonCountProperty =
      DependencyProperty.Register("MaxPagerButtonCount", typeof(int), typeof(DataGridPager),
      new FrameworkPropertyMetadata(9, OnMaxPagerButtonCountChanged));

    private static void OnMaxPagerButtonCountChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((DataGridPager)d).OnMaxPagerButtonCountChanged();
    }

    private void OnMaxPagerButtonCountChanged()
    {
      UpdatePagerButtons();
    }

    #endregion // MaxPagerButtonCount Property

    #region EllipsisMode Property

    /// <summary>
    /// Gets or sets the <see cref="EllipsisMode"/> of the pager buttons.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="EllipsisModeProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public EllipsisMode EllipsisMode
    {
      get { return (EllipsisMode)GetValue(EllipsisModeProperty); }
      set { SetValue(EllipsisModeProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="EllipsisMode"/> property.
    /// </summary>
    public static readonly DependencyProperty EllipsisModeProperty =
      DependencyProperty.Register("EllipsisMode", typeof(EllipsisMode), typeof(DataGridPager),
      new FrameworkPropertyMetadata(EllipsisMode.Both, OnEllipsisModeChanged));

    private static void OnEllipsisModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((DataGridPager)d).OnEllipsisModeChanged();
    }

    private void OnEllipsisModeChanged()
    {
      UpdatePagerButtons();
    }

    #endregion // EllipsisMode Property

    private bool _ignoreSelectionUpdates;

    private void UpdatePagerButtons()
    {
      int buttonCount = DataGridItemsSource == null ? 0 : Math.Min(MaxPagerButtonCount, DataGridItemsSource.PageCount);
      int pageIndex = DataGridItemsSource == null ? 0 : Math.Max(0, Math.Min(PageIndex - (buttonCount / 2), DataGridItemsSource.PageCount - buttonCount));
      _ignoreSelectionUpdates = true;
      RemoveUnusedButtons(0);
      for (int i = 0; i < buttonCount; i++)
      {
        PagerButtonModel model = GetButtonModel(i);

        if (i == 0 && pageIndex != 0 && (EllipsisMode == EllipsisMode.Before || EllipsisMode == EllipsisMode.Both))
        {
          model.Content = "...";
        }
        else if (DataGridItemsSource != null && i == buttonCount - 1 && pageIndex != DataGridItemsSource.PageCount - 1 && (EllipsisMode == EllipsisMode.After || EllipsisMode == EllipsisMode.Both))
        {
          model.Content = "...";
        }
        else
        {
          model.Content = pageIndex + 1;
        }
        model.IsSelected = pageIndex == PageIndex;
        if (model.IsSelected)
        {
          _selectedButton = model;
        }
        model.PageIndex = pageIndex;
        pageIndex++;
      }
      _ignoreSelectionUpdates = false;
      RemoveUnusedButtons(buttonCount);

      if (_selectedButton != null)
      {
        // This kludge is to fix some strange behavior in RadioButton:
        // May be able to use normal buttons instead of radio buttons later.
        Dispatcher.BeginInvoke(new Action(EnsureCorrectSelectedButton));
      }
    }

    private PagerButtonModel _selectedButton;

    private void EnsureCorrectSelectedButton()
    {
      _ignoreSelectionUpdates = true;
      _selectedButton.IsSelected = true;
      _ignoreSelectionUpdates = false;
    }

    private void RemoveUnusedButtons(int buttonCount)
    {
      while (buttonCount < _pagerButtons.Count)
      {
        PagerButtonModel model = _pagerButtons[_pagerButtons.Count - 1];
        model.PropertyChanged -= new PropertyChangedEventHandler(PagerButtonModel_PropertyChanged);
        _pagerButtons.RemoveAt(_pagerButtons.Count - 1);
      }
    }

    private PagerButtonModel GetButtonModel(int buttonIndex)
    {
      if (buttonIndex < _pagerButtons.Count)
      {
        return _pagerButtons[buttonIndex];
      }
      PagerButtonModel model = new PagerButtonModel();
      model.PropertyChanged += new PropertyChangedEventHandler(PagerButtonModel_PropertyChanged);
      _pagerButtons.Add(model);
      return model;
    }

    private void PagerButtonModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      if (!_ignoreSelectionUpdates && "IsSelected".Equals(e.PropertyName))
      {
        PagerButtonModel model = sender as PagerButtonModel;
        if (model.IsSelected)
        {
          PageIndex = model.PageIndex;
        }
      }
    }

    // TODO: throw an error if someone tries to modify this collection? Or change this to be a readonly collection?
    // This is an observable collection so that the ui can automatically be updated, and I think this can help improve performance compared to creating a read only collection every time.

    private ObservableCollection<PagerButtonModel> _pagerButtons = new ObservableCollection<PagerButtonModel>();

    /// <summary>
    /// Gets the collection of pager buttons.
    /// </summary>
    public ObservableCollection<PagerButtonModel> PagerButtons
    {
      get { return _pagerButtons; }
    }
  }
}
