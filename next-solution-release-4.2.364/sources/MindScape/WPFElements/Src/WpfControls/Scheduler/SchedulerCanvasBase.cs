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
using System.Windows.Threading;
using System.Collections.Generic;
using System.Diagnostics;
#if SILVERLIGHT
using Mindscape.SilverlightElements.Internal;
#else
using Mindscape.WpfElements.Internal;
#endif
using System.ComponentModel;

#if SILVERLIGHT
namespace Mindscape.SilverlightElements
#else
namespace Mindscape.WpfElements
#endif
{
  /// <summary>
  /// Represents a canvas on which schedule items can be laid out.
  /// </summary>
  public abstract class SchedulerCanvasBase : Canvas, ITimeUIElement
#if SILVERLIGHT
    , IMouseWheelObserver
#endif
  {
    private readonly DispatcherTimer _createHereButtonTimer;
    private readonly Button _createHereButton;
    private SchedulerElement _createHereSchedulerElement;
    private SchedulerElement _SilentElement;
    private TextBox _input;

    /// <summary>
    /// Initializes a new instance of the <see cref="SchedulerCanvasBase"/> class.
    /// </summary>
    protected SchedulerCanvasBase()
    {
      _createHereButton = new Button();
      _createHereButton.Content = CreateHereHintContent;
      _createHereButton.Click += new RoutedEventHandler(CreateHereButton_Click);
      
      CreateHereDayIndex = -1;

#if SILVERLIGHT
      _createHereButtonTimer = new DispatcherTimer { Interval = new TimeSpan(0, 0, 1) };
      _createHereButtonTimer.Tick += new EventHandler(OnCreateHereTimerExpired);
#else
      _createHereButtonTimer = new DispatcherTimer { Interval = new TimeSpan(0, 0, 3) };
      Loaded += new RoutedEventHandler(SchedulerCanvasBase_Loaded);
      Unloaded += new RoutedEventHandler(SchedulerCanvasBase_Unloaded);
#endif

      // Non-Controls don't have OnMouseXxx methods, so we have to subscribe to our
      // own events.  Silverlight FAIL.
      MouseLeftButtonDown += new MouseButtonEventHandler(SchedulerCanvas_MouseLeftButtonDown);
      MouseLeftButtonUp += new MouseButtonEventHandler(SchedulerCanvas_MouseLeftButtonUp);
      
#if SILVERLIGHT
      Dispatcher.BeginInvoke(ObtainFormatter);
#else
      Dispatcher.BeginInvoke(new Action(ObtainFormatter));
#endif

      _input = new TextBox();
      _input.BorderThickness = new Thickness(0);
      _input.Height = 0;
      _input.TextChanged += new TextChangedEventHandler(Input_TextChanged);
      Children.Add(_input);
    }

#if !SILVERLIGHT
    private void SchedulerCanvasBase_Loaded(object sender, RoutedEventArgs e)
    {
      _createHereButtonTimer.Tick += new EventHandler(OnCreateHereTimerExpired);
    }

    private void SchedulerCanvasBase_Unloaded(object sender, RoutedEventArgs e)
    {
      _createHereButtonTimer.Tick -= new EventHandler(OnCreateHereTimerExpired);
    }
#endif

    /// <summary>
    /// Clears the Children collection and performs some base preperations.
    /// </summary>
    protected void ClearChildren()
    {
      Children.Clear();
      Children.Add(_input);
    }

    private void Input_TextChanged(object sender, TextChangedEventArgs e)
    {
      if (_input.Text.Length > 0)
      {
        Scheduler scheduler = VisualTreeUtils.FindAncestor<Scheduler>(this);
        if (scheduler != null && scheduler.IsReadOnly)
        {
          _input.Text = "";
          return;
        }
        IScheduleProvider view = VisualTreeUtils.FindAncestor<IScheduleProvider>(this);
        if (view != null)
        {
          Schedule schedule = view.Schedule;
          if (schedule != null && schedule.SelectedItem == null)
          {
            //ScheduleView view = VisualTreeUtils.FindAncestor<ScheduleView>(this);
            if (view != null)
            {
              ScheduleItem item = new ScheduleItem() { Name = _input.Text };
              DateRange range = view.SelectedDateRange;
              item.StartTime = range.StartDate;
              item.EndTime = range.EndDate;
              item.Name = _input.Text;
              item.IsSilent = true;
              schedule.SilentlyAddItem(item);
              if (item != null)
              {
#if SILVERLIGHT
              item.IsSelected = true;
#endif
                IsMouseDown = false;
                MonthViewDayElement._dragging = false; // So bad. find a way to avoid this !!
                _SilentElement = view.FindSchedulerElement(item);
                if (_SilentElement != null)
                {
                  _SilentElement.ScheduleItem.PropertyChanged += new PropertyChangedEventHandler(SilentElement_PropertyChanged);
#if SILVERLIGHT
                Dispatcher.BeginInvoke(new EnsureFocusDelegate(EnsureFocus), _SilentElement, 1);
#else
                  // When there is no more room in month view on a SmallSchedulerCanvas, overflowing items are collapsed.
                  // In this case, the OnApplyTemplate will never get called, and so the the EditableTextBlock will
                  // never be found and given focus. The following code cancels the type-to-add feature and
                  // makes sure that the silently added item is then silently removed to avoid glitches.
                  if (_SilentElement.Visibility == Visibility.Collapsed)
                  {
                    schedule.SilentlyRemoveItem(_SilentElement.ScheduleItem);
                    _SilentElement.PropertyChanged -= new PropertyChangedEventHandler(SilentElement_PropertyChanged);
                    _SilentElement = null;
                    _input.Text = "";
                    return;
                  }
                  else
                  {
                    item.IsSelected = true;
                    _SilentElement.TemplateApplied += new RoutedEventHandler(SilentElement_TemplateApplied);
                  }
#endif
                }
              }
            }
          }
        }
        _input.TextChanged -= Input_TextChanged;
        _input.Text = "";
      }
    }

#if !SILVERLIGHT
    private void SilentElement_TemplateApplied(object sender, RoutedEventArgs e)
    {
      _SilentElement.TemplateApplied -= new RoutedEventHandler(SilentElement_TemplateApplied);
      // Here we perform 2 attempts to ensuring the focus.
      // The second attempt occurs after the template has been applied to the content of the element which
      // is what needs to happen to find the EditableTextBlock.
      EnsureFocus(_SilentElement, 2);
    }
#endif

    private void SilentElement_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      if (e.PropertyName.Equals("IsSelected"))
      {
        if (_SilentElement != null && !_SilentElement.IsSelected)
        {
          /*_SilentElement.ScheduleItem.PropertyChanged -= new PropertyChangedEventHandler(SilentElement_PropertyChanged);
          _input.TextChanged += Input_TextChanged;
          Scheduler scheduler = VisualTreeUtils.FindAncestor<Scheduler>(this);
          if (scheduler != null)
          {
            bool cancel, show;
            scheduler.OnItemAdded(_SilentElement.ScheduleItem, out cancel, out show);
          }
          _SilentElement = null;*/
          EditableTextBlock block = VisualTreeUtils.GetChild<EditableTextBlock>(_SilentElement);
          if (block != null)
          {
            Block_LostFocus(block, new RoutedEventArgs());
          }
        }
      }
    }

    private delegate void EnsureFocusDelegate(SchedulerElement element, int attempts);

    private void EnsureFocus(SchedulerElement element, int attempts)
    {
      EditableTextBlock block = VisualTreeUtils.GetChild<EditableTextBlock>(element);
      if (block != null)
      {
        block.IsInEditMode = true;
        block.Focus();
        block.SelectionStart = block.Text.Length;
        block.SelectionLength = 0;
        block.LostFocus -= new RoutedEventHandler(Block_LostFocus);
        block.LostFocus += new RoutedEventHandler(Block_LostFocus);
      }
      if (attempts > 1)
      {
        // This improves the ensurance that the EditableTextBlock is given focus.
        Dispatcher.BeginInvoke(new EnsureFocusDelegate(EnsureFocus), element, attempts - 1);
      }
    }

    private void Block_LostFocus(object sender, RoutedEventArgs e)
    {
      _input.Text = "";
      _input.TextChanged += Input_TextChanged;
      Scheduler scheduler = VisualTreeUtils.FindAncestor<Scheduler>(this);
      IScheduleProvider provider = VisualTreeUtils.FindAncestor<IScheduleProvider>(this);
      Schedule schedule = provider.Schedule;
      if (provider != null)
      {
        EditableTextBlock block = sender as EditableTextBlock;
        block.LostFocus -= new RoutedEventHandler(Block_LostFocus);
        if (_SilentElement != null)
        {
#if !SILVERLIGHT
          _SilentElement.Focus();
#endif
          if (block.Text.Length == 0)
          {
            schedule.SilentlyRemoveItem(_SilentElement.ScheduleItem);
          }
          else
          {
            ScheduleItem item = _SilentElement.ScheduleItem;
            bool addDefault = true;
            if (schedule.ScheduleItemBuilder != null)
            {
              CreateScheduleItemArgs args = new CreateScheduleItemArgs(schedule, ScheduleItemCreationType.TextInput, item.StartTime, item.EndTime, item.Name);
              CreateScheduleItemResult result = schedule.ScheduleItemBuilder.CreateScheduleItem(args);
              addDefault = result.AddDefaultItem && result.Item == null;
            }
            if (addDefault)
            {
              item.IsSilent = false;
              bool cancel, show;
              if (scheduler != null)
              {
                scheduler.OnItemAdded(item, out cancel, out show);
              }
            }
            else
            {
              schedule.SilentlyRemoveItem(item);
            }
          }
          _SilentElement.PropertyChanged -= new PropertyChangedEventHandler(SilentElement_PropertyChanged);
          _SilentElement.ScheduleItem.IsSelected = false;
          _SilentElement = null;
        }
      }
    }

    private void EnsureInputFocus()
    {
      _input.Focus();
    }

    private void ObtainFormatter()
    {
      Scheduler scheduler = VisualTreeUtils.FindAncestor<Scheduler>(this);
      if (scheduler != null)
      {
        Formatter = scheduler.Formatter;
      }
    }

    internal abstract SchedulerElement FindSchedulerElement(ScheduleItem item);

    private void SchedulerCanvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
      IsMouseDown = false;
    }

    private void SchedulerCanvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
      IsMouseDown = true;
      //Scheduler scheduler = VisualTreeUtils.FindAncestor<Scheduler>(this);
      //if (scheduler != null && scheduler.SelectedItem == null)
      //{
#if SILVERLIGHT
        Dispatcher.BeginInvoke(EnsureInputFocus);
#else
        Dispatcher.BeginInvoke(new Action(EnsureInputFocus));
#endif
      //}
    }

    DateTime ITimeUIElement.GetDateTime(Point point, TimeEnd timeEnd)
    {
      return GetDateTime(point.X, point.Y, timeEnd);
    }

    /// <summary>
    /// When overridden in a derived class, gets the time corresponding to a location
    /// on the canvas.
    /// </summary>
    /// <param name="x">The x-coordinate of the location.</param>
    /// <param name="y">The y-coordinate of the location.</param>
    /// <param name="timeEnd">Whether to adjust the time for the start or end of a <see cref="ScheduleItem"/>.</param>
    /// <returns>The date and time corresponding to x and y.</returns>
    protected abstract DateTime GetDateTime(double x, double y, TimeEnd timeEnd);

    /// <summary>
    /// Gets or sets the per-day data to be represented on the canvas.
    /// This member supports the <see cref="Scheduler"/> control and is not intended for use
    /// in your code, but may be referenced in advanced styling and templating scenarios.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="DaysProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public IList<DayModel> Days
    {
      get { return (IList<DayModel>)GetValue(DaysProperty); }
      set { SetValue(DaysProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="Days"/> property.
    /// </summary>
    public static readonly DependencyProperty DaysProperty = DependencyProperty.Register(
      "Days", typeof(IList<DayModel>),
      typeof(SchedulerCanvasBase), new PropertyMetadata(OnDaysChanged));

    private static void OnDaysChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
    {
      ((SchedulerCanvasBase)sender).OnDaysChanged(e);
    }

    /// <summary>
    /// When overridden in a derived class, handles changes to the
    /// <see cref="Days"/> property.
    /// </summary>
    /// <param name="e">Contains information about changes to the Days property.</param>
    protected virtual void OnDaysChanged(DependencyPropertyChangedEventArgs e)
    {
      // no-op in base class
    }

    #region Formatter property

    /// <summary>
    /// Gets or sets the Formatter used to provide information about styling various parts of a <see cref="Scheduler"/>.
    /// This is a dependency property.
    /// </summary>
    public SchedulerFormatter Formatter
    {
      get { return (SchedulerFormatter)GetValue(FormatterProperty); }
      set { SetValue(FormatterProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="Formatter"/> property.
    /// </summary>
    public static readonly DependencyProperty FormatterProperty =
      DependencyProperty.Register("Formatter", typeof(SchedulerFormatter), typeof(SchedulerCanvasBase),
      new PropertyMetadata(new PropertyChangedCallback(OnFormatterChanged)));

    private static void OnFormatterChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((SchedulerCanvasBase)d).OnFormatterChanged(e);
    }

    /// <summary>
    /// Classes derived from <see cref="SchedulerCanvasBase"/> can override this method to provide additional logic when
    /// the formatter is changed.
    /// </summary>
    /// <param name="e">Event args.</param>
    protected abstract void OnFormatterChangedCore(DependencyPropertyChangedEventArgs e);

    private void OnFormatterChanged(DependencyPropertyChangedEventArgs e)
    {
      OnFormatterChangedCore(e);
    }

    #endregion // Formatter property

    /// <summary>
    /// Selects a style for the given <see cref="SchedulerElement"/> based on the given <see cref="StyleSelector"/>. If the
    /// selector is null, or if it fails to select a style, then the given default style will be returned.
    /// </summary>
    /// <param name="element">The <see cref="SchedulerElement"/> to select a style for.</param>
    /// <param name="selector">The <see cref="StyleSelector"/> for selecting the style.</param>
    /// <param name="defaultStyle">The default <see cref="Style"/> if the selector fails to select one.</param>
    /// <returns>The <see cref="Style"/> to be applied to the given <see cref="SchedulerElement"/>.</returns>
    protected Style SelectStyle(SchedulerElement element, StyleSelector selector, Style defaultStyle)
    {
      Style style = null;
      if (selector != null)
      {
        style = selector.SelectStyle(element.ScheduleItem, element);
      }
      return style ?? defaultStyle;
    }

    /// <summary>
    /// Selects a template for the given <see cref="SchedulerElement"/> based on the given <see cref="DataTemplateSelector"/>.
    /// If the selector is null, or if it fails to select a template, then the given default template will be returned.
    /// </summary>
    /// <param name="element">The <see cref="SchedulerElement"/> to select a template for.</param>
    /// <param name="selector">The <see cref="DataTemplateSelector"/> for selecting the template.</param>
    /// <param name="defaultTemplate">The default <see cref="DataTemplate"/> if the selector fails to select one.</param>
    /// <returns>The <see cref="DataTemplate"/> to be applied to the given <see cref="SchedulerElement"/>.</returns>
    protected DataTemplate SelectTemplate(SchedulerElement element, DataTemplateSelector selector, DataTemplate defaultTemplate)
    {
      DataTemplate template = null;
      if (selector != null)
      {
        template = selector.SelectTemplate(element.ScheduleItem, element);
      }
      return template ?? defaultTemplate;
    }

    /// <summary>
    /// Gets or sets the content displayed on the "create here" button
    /// that is displayed when the user hovers over the control.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="CreateHereHintContentProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public object CreateHereHintContent
    {
      get { return GetValue(CreateHereHintContentProperty); }
      set { SetValue(CreateHereHintContentProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="CreateHereHintContent"/> property.
    /// </summary>
    public static readonly DependencyProperty CreateHereHintContentProperty =
      DependencyProperty.Register("CreateHereHintContent", typeof(object), typeof(SchedulerCanvasBase),
      new PropertyMetadata("Click to add item"));

    //private static void OnCreateHereHintContentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    //{
    //  ((SchedulerCanvasBase)d).OnCreateHereHintContentChanged(e);
    //}

    //private void OnCreateHereHintContentChanged(DependencyPropertyChangedEventArgs e)
    //{
    //  _createHereButton.Content = CreateHereHintContent;
    //}

    /// <summary>
    /// Gets or sets the style for the "create here" button
    /// that is displayed when the user hovers over the control.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="CreateHereButtonStyleProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public Style CreateHereButtonStyle
    {
      get { return (Style)GetValue(CreateHereButtonStyleProperty); }
      set { SetValue(CreateHereButtonStyleProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="CreateHereButtonStyle"/> property.
    /// </summary>
    public static readonly DependencyProperty CreateHereButtonStyleProperty =
      DependencyProperty.Register("CreateHereButtonStyle", typeof(Style), typeof(SchedulerCanvasBase),
      new PropertyMetadata(new PropertyChangedCallback(OnCreateHereButtonStyleChanged)));

    private static void OnCreateHereButtonStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((SchedulerCanvasBase)d).OnCreateHereButtonStyleChanged();
    }

    private void OnCreateHereButtonStyleChanged()
    {
      Debug.Assert(_createHereButton != null);
      _createHereButton.Style = CreateHereButtonStyle;
    }

    /// <summary>
    /// Gets or sets the style for the "new appointment" element that
    /// appears when the user clicks the "create here" button.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="CreateHereElementStyleProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public Style CreateHereElementStyle
    {
      get { return (Style)GetValue(CreateHereElementStyleProperty); }
      set { SetValue(CreateHereElementStyleProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="CreateHereElementStyle"/> property.
    /// </summary>
    public static readonly DependencyProperty CreateHereElementStyleProperty = DependencyProperty.Register(
      "CreateHereElementStyle", typeof(Style),
      typeof(SchedulerCanvasBase), new PropertyMetadata(null));
    
    /// <summary>
    /// Gets or sets the duration in minutes for a schedule item that is added useing the "Create here" button.
    /// This is a dependency property.
    /// </summary>
    public int CreateHereElementDuration
    {
      get { return (int)GetValue(CreateHereElementDurationProperty); }
      set { SetValue(CreateHereElementDurationProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="CreateHereElementDuration"/> property.
    /// </summary>
    public static readonly DependencyProperty CreateHereElementDurationProperty =
      DependencyProperty.Register("CreateHereElementDuration", typeof(int), typeof(SchedulerCanvasBase),
      new PropertyMetadata(30));

    /// <summary>
    /// Removes the "create here" button.
    /// </summary>
    protected void RemoveCreateHereButton()  // Fie
    {
      Children.Remove(_createHereButton);
      CreateHereDayIndex = -1;
    }

    private void ResetCreateHereButtonTimer()
    {
      _createHereButtonTimer.Start();
    }

    /// <summary>
    /// Gets the day index on which to display the "create here" button.
    /// </summary>
    protected int CreateHereDayIndex { get; private set; }

    /// <summary>
    /// Gets the time at whose location to display the "create here" button.
    /// </summary>
    protected DateTime CreateHereTime { get; private set; }

    /// <summary>
    /// Performs layout of the "create here" button when a setting or selection changes.
    /// </summary>
    /// <param name="dayIndex">The index of the day in which to display the "create here" button.</param>
    /// <param name="timeSlotIndex">The index of the time slot in which to display the "create here" button.</param>
    /// <param name="time">The time at whose location to display the "create here" button.</param>
    protected void RelayoutCreateHereButton(int dayIndex, int timeSlotIndex, DateTime time)
    {
      RemoveCreateHereButton();
      CreateHereDayIndex = dayIndex;
      CreateHereTime = time;

      ButtonLayout layout = GetCreateHereButtonLayout(dayIndex, timeSlotIndex);
      layout.ApplyTo(_createHereButton);

      ResetCreateHereButtonTimer();
    }

    internal abstract ButtonLayout GetCreateHereButtonLayout(int dayIndex, int timeSlotIndex);

    /// <summary>
    /// Gets whether the mouse button is down over the <see cref="SchedulerCanvasBase"/>.
    /// </summary>
    protected bool IsMouseDown { get; set; }

    private void OnCreateHereTimerExpired(object sender, EventArgs e)
    {
      Scheduler scheduler = VisualTreeUtils.FindAncestor<Scheduler>(this);
      if (scheduler == null || !scheduler.IsReadOnly)
      {
        if (!IsMouseDown && !ScheduleItemMover.IsDragging && !ScheduleItemTimeChanger.IsDragging && _createHereSchedulerElement == null &&
          CreateHereDayIndex != -1 && !Children.Contains(_createHereButton))
        {
          Children.Add(_createHereButton);
        }
      }
    }

    private void CreateHereButton_Click(object sender, RoutedEventArgs e)
    {
      try
      {
        _createHereSchedulerElement = new SchedulerElement(new ScheduleItem());
        _createHereSchedulerElement.ScheduleItem.StartTime = CreateHereTime;
        _createHereSchedulerElement.ScheduleItem.EndTime = CreateHereTime.AddMinutes(CreateHereElementDuration);
        _createHereSchedulerElement.FirstVisibleDay = Days[CreateHereDayIndex];
        _createHereSchedulerElement.Style = CreateHereElementStyle;
        _createHereSchedulerElement.Margin = _createHereButton.Margin;
        _createHereSchedulerElement.Width = (int)(ActualWidth) / Days.Count;
        //GetCreateHereButtonLayout(CreateHereDayIndex

        Children.Add(_createHereSchedulerElement);
        Children.Remove(_createHereButton);
#if SILVERLIGHT
        Dispatcher.BeginInvoke(SetupTemporaryTextBox);
#else
        _createHereSchedulerElement.TemplateApplied += new RoutedEventHandler(CreateHereSchedulerElement_TemplateApplied);
#endif

        DeselectEverything();
      }
      catch (Exception)
      {
        return;
      }
    }

#if !SILVERLIGHT
    private void CreateHereSchedulerElement_TemplateApplied(object sender, RoutedEventArgs e)
    {
      _createHereSchedulerElement.TemplateApplied -= new RoutedEventHandler(CreateHereSchedulerElement_TemplateApplied);
      SetupTemporaryTextBox();
    }
#endif

    private void DeselectEverything()
    {
      foreach (DayModel day in Days)
      {
        day.DeselectAll();
        day.IsSelected = false;
        foreach (ScheduleItem item in day.ScheduleItems)
        {
          item.IsSelected = false;
        }
      }
    }

    private void SetupTemporaryTextBox()
    {
      try
      {
        TextBox box = VisualTreeUtils.GetChild<TextBox>(_createHereSchedulerElement);
        if (box != null)
        {
          box.Focus();
          box.LostFocus += TemporaryTextBox_LostFocus;
          box.KeyDown += new KeyEventHandler(TemporaryTextBox_KeyDown);
        }
        else
        {
          Children.Remove(_createHereSchedulerElement);
          _createHereSchedulerElement = null;
        }
      }
      catch (Exception)
      {
        return;
      }
    }

    private void TemporaryTextBox_KeyDown(object sender, KeyEventArgs e)
    {
      if (e.Key == Key.Enter)
      {
        TextBox box = sender as TextBox;
        DisposeTemporaryTextBox(box);
        CreateHereDayIndex = -1;
      }
    }

    private void TemporaryTextBox_LostFocus(object sender, RoutedEventArgs e)
    {
      TextBox box = sender as TextBox;
      DisposeTemporaryTextBox(box); 
    }

    private void DisposeTemporaryTextBox(TextBox box)
    {
      try
      {
        box.LostFocus -= TemporaryTextBox_LostFocus;
        box.KeyDown -= new KeyEventHandler(TemporaryTextBox_KeyDown);
        if (box.Text.Length != 0)
        {
          _createHereSchedulerElement.IsSelected = false;
          _createHereSchedulerElement.ScheduleItem.IsSelected = false;
          _createHereSchedulerElement.ScheduleItem.Name = box.Text;
          bool addDefaultItem = true;
          //Scheduler scheduler = VisualTreeUtils.FindContaining<Scheduler>(this);
          IScheduleProvider view = VisualTreeUtils.FindContaining<IScheduleProvider>(this);
          if (view != null)
          {
            Schedule schedule = view.Schedule;
            ScheduleItem item = _createHereSchedulerElement.ScheduleItem;
            if (schedule.ScheduleItemBuilder != null)
            {
              CreateScheduleItemArgs args = new CreateScheduleItemArgs(schedule, ScheduleItemCreationType.CreateHereButton, item.StartTime, item.EndTime, item.Name);
              CreateScheduleItemResult result = schedule.ScheduleItemBuilder.CreateScheduleItem(args);
              addDefaultItem = result.AddDefaultItem && result.Item == null;
            }
            if (addDefaultItem)
            {
              schedule.AddItem(item);
            }
          }
        }
        Children.Remove(_createHereSchedulerElement);
        _createHereSchedulerElement = null;
      }
      catch (Exception)
      {
        return;
      }
    }

#if SILVERLIGHT
    #region IMouseWheelObserver Members

    /// <summary>
    /// Gets the amount to scroll the <see cref="SchedulerCanvasBase"/> in response to the mouse wheel.
    /// </summary>
    protected abstract double MouseWheelScrollFactor { get; }

    void IMouseWheelObserver.OnMouseWheel(MouseWheelEventArgs args)
    {
      double delta = args.Delta;
      ScrollViewer viewer = FindContainingScrollViewer();
      if (viewer == null)
      {
        return;
      }
      double newOffset = viewer.VerticalOffset + delta * MouseWheelScrollFactor; //TODO: later need to be able to change how big a time slot covers.
      viewer.ScrollToVerticalOffset(newOffset);
      RemoveCreateHereButton();
    }

    #endregion

    private ScrollViewer FindContainingScrollViewer()
    {
      return VisualTreeUtils.FindContaining<ScrollViewer>(this);
    }
#endif
  }
}
