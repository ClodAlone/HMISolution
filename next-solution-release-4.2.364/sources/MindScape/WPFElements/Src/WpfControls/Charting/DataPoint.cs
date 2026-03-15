using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.ComponentModel;

namespace Mindscape.WpfElements.Charting
{
  /// <summary>
  /// Represents a single piece of data to be plotted on a chart.
  /// </summary>
  public abstract class DataPoint : Control
  {
    // _isMouseOver and _isHighlighted mean the same thing. They are managed by different features and so have been split
    // into 2 variables so that they don't interfere with each other.
    private bool _isMouseOver;
    private bool _isHighlighted;
    private bool _canHighlight = true;

    /// <summary>
    /// Initializes a new instance of the <see cref="DataPoint"/> class.
    /// </summary>
    protected DataPoint()
    {
    }

    /// <summary>
    /// Called when the mouse enters the <see cref="DataPoint"/>
    /// </summary>
    /// <param name="e">The event data.</param>
    protected override void OnMouseEnter(MouseEventArgs e)
    {
      base.OnMouseEnter(e);

      _isMouseOver = true;
      UpdateIsMouseOver();
    }

    /// <summary>
    /// Called when the mouse leaves the <see cref="DataPoint"/>.
    /// </summary>
    /// <param name="e">The event data.</param>
    protected override void OnMouseLeave(MouseEventArgs e)
    {
      base.OnMouseLeave(e);

      _isMouseOver = false;
      UpdateIsMouseOver();
    }

    /// <summary>
    /// Called when the left mouse button is pressed over the <see cref="DataPoint"/>.
    /// </summary>
    /// <param name="e">The event data.</param>
    protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
    {
      base.OnMouseLeftButtonDown(e);

      _isMouseOver = false;
      IsSelected = !IsSelected;
      UpdateIsMouseOver();
    }

    /// <summary>
    /// Called when the right mouse button is pressed over the <see cref="DataPoint"/>.
    /// </summary>
    /// <param name="e">The event data.</param>
    protected override void OnMouseRightButtonDown(MouseButtonEventArgs e)
    {
      base.OnMouseLeftButtonDown(e);

      _isMouseOver = false;
      IsSelected = !IsSelected;
      UpdateIsMouseOver();
    }

    /// <summary>
    /// Gets whether the mouse is over the <see cref="DataPoint"/>.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="IsMouseOverProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    new public bool IsMouseOver
    {
      get { return (bool)GetValue(IsMouseOverProperty); }
    }

    private static readonly DependencyPropertyKey IsMouseOverPropertyKey =
        DependencyProperty.RegisterReadOnly("IsMouseOver", typeof(bool), typeof(DataPoint), new UIPropertyMetadata(false));

    /// <summary>
    /// Identifies the <see cref="IsMouseOver"/> property.
    /// </summary>
    new public static readonly DependencyProperty IsMouseOverProperty =
        IsMouseOverPropertyKey.DependencyProperty;

    #region LabelContent Property

    /// <summary>
    /// Gets or sets custom data to be displayed in the data label for this <see cref="DataPoint"/>.
    /// This property can be set using the DataSeries.LabelBinding property.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="LabelContentProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public object LabelContent
    {
      get { return GetValue(LabelContentProperty); }
      set { SetValue(LabelContentProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="LabelContent"/> property.
    /// </summary>
    public static readonly DependencyProperty LabelContentProperty =
      DependencyProperty.Register("LabelContent", typeof(object), typeof(DataPoint),
      new FrameworkPropertyMetadata(OnLabelContentChanged));

    private static void OnLabelContentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((DataPoint)d).OnLabelContentChanged();
    }

    private void OnLabelContentChanged()
    {
    }

    #endregion // LabelContent Property

    /// <summary>
    /// Gets or sets whether the <see cref="DataPoint"/> is selected.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="IsSelectedProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public bool IsSelected
    {
      get { return (bool)GetValue(IsSelectedProperty); }
      set { SetValue(IsSelectedProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="IsSelected"/> property.
    /// </summary>
    public static readonly DependencyProperty IsSelectedProperty =
      DependencyProperty.Register("IsSelected", typeof(bool), typeof(DataPoint),
      new FrameworkPropertyMetadata(OnIsSelectedChanged));

    private static void OnIsSelectedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((DataPoint)d).OnIsSelectedChanged();
    }

    private bool _settingIsSelectedInternal;

    private void OnIsSelectedChanged()
    {
      EventHandler<SelectionChangeRequestArgs> handler = IsSelectedChangeRequested;
      if (handler != null && !_settingIsSelectedInternal)
      {
        SelectionChangeRequestArgs args = new SelectionChangeRequestArgs(IsSelected);
        handler(this, args);
        _settingIsSelectedInternal = true;
        if (IsSelected == args.ActualSelection)
        {
          _settingIsSelectedInternal = false;
          RaiseSelectedChangedEvent();
        }
        else
        {
          IsSelected = args.ActualSelection;
          _settingIsSelectedInternal = false;
        }
      }
      else if (handler == null)
      {
        RaiseSelectedChangedEvent();
      }
    }

    internal void SetIsSelected(bool isSelected)
    {
      _settingIsSelectedInternal = true;
      IsSelected = isSelected;
      _settingIsSelectedInternal = false;
    }

    internal event EventHandler<SelectionChangeRequestArgs> IsSelectedChangeRequested;

    internal bool CanHighlight
    {
      get { return _canHighlight; }
      set
      {
        if (_canHighlight != value)
        {
          _canHighlight = value;
          UpdateIsMouseOver();
        }
      }
    }

    internal bool IsHighlighted
    {
      get { return _isHighlighted; }
      set
      {
        if (_isHighlighted != value)
        {
          _isHighlighted = value;
          UpdateIsMouseOver();
        }
      }
    }

    /// <summary>
    /// Raised when the IsSelected property changes.
    /// </summary>
    public event EventHandler IsSelectedChanged;

    /// <summary>
    /// Identifies the IsSelectedChanged routed event.
    /// </summary>
    public static readonly RoutedEvent IsSelectedChangedEvent = EventManager.RegisterRoutedEvent("IsSelectedChanged", RoutingStrategy.Bubble,
        typeof(EventHandler), typeof(DataPoint));

    private void RaiseSelectedChangedEvent()
    {
      EventHandler handler = IsSelectedChanged;
      if (handler != null)
      {
        handler(this, new EventArgs());
      }

      RaiseEvent(new RoutedEventArgs(IsSelectedChangedEvent, this));
    }

    private void UpdateIsMouseOver()
    {
      if ((_isMouseOver || _isHighlighted) && !IsSelected && _canHighlight)
      {
        SetValue(IsMouseOverPropertyKey, true);
      }
      else
      {
        SetValue(IsMouseOverPropertyKey, false);
      }
    }

    internal class SelectionChangeRequestArgs : EventArgs
    {
      public SelectionChangeRequestArgs(bool requestedSelection)
      {
        RequestedSelection = requestedSelection;
        ActualSelection = requestedSelection;
      }

      public bool RequestedSelection { get; private set; }

      public bool ActualSelection { get; set; }
    }
  }
}
