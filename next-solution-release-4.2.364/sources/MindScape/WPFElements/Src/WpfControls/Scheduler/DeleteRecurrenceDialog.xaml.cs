using System;
using System.Collections.Generic;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Diagnostics;

#if SILVERLIGHT
namespace Mindscape.SilverlightElements
#else
namespace Mindscape.WpfElements
#endif
{
  /// <summary>
  /// A dialog box to ask the user if they want to delete an entire recurrence pattern or just one of its schedule items.
  /// </summary>
  public partial class DeleteRecurrenceDialog :
#if SILVERLIGHT
    ChildWindow
#else
    UserControl
#endif
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="DeleteRecurrenceDialog"/> class.
    /// </summary>
    /// <param name="item">The <see cref="ScheduleItem"/> to be deleted. This must be an instance of a recurring item.</param>
    public DeleteRecurrenceDialog(ScheduleItem item)
    {
      InitializeComponent();

      BindCommands();

      if (!item.IsInstanceOfRecurringItem)
      {
        throw new InvalidOperationException("The DeleteRecurrenceDialog can only accept a ScheduleItem that is an instance of a recurring item");
      }
      ScheduleItem = item;
      DeleteSeries = false;

      Loaded += new RoutedEventHandler(DeleteRecurrenceDialog_Loaded);

      DataContext = this;
    }

    private void DeleteRecurrenceDialog_Loaded(object sender, RoutedEventArgs e)
    {
      Window wnd = Parent as Window;
      if (wnd != null)
      {
        wnd.Title = Title;
      }
    }

    /// <summary>
    /// Gets the <see cref="ScheduleItem"/> to be deleted.
    /// </summary>
    public ScheduleItem ScheduleItem { get; private set; }

    private void BindCommands()
    {
      CommandBindings.Add(new CommandBinding(SchedulerCommands.OkCommand, Ok_Executed));
      CommandBindings.Add(new CommandBinding(SchedulerCommands.CancelCommand, Cancel_Executed));
    }

    private void Ok_Executed(object sender, ExecutedRoutedEventArgs e)
    {
      this.DialogResult = true;
    }

    private void Cancel_Executed(object sender, ExecutedRoutedEventArgs e)
    {
      this.DialogResult = false;
    }

    #region DeleteSeries property

    /// <summary>
    /// Gets or sets whether or not to delete the entire recurrence pattern.
    /// This is a dependency property.
    /// </summary>
    public bool DeleteSeries
    {
      get { return (bool)GetValue(DeleteSeriesProperty); }
      set { SetValue(DeleteSeriesProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="DeleteSeries"/> property.
    /// </summary>
    public static readonly DependencyProperty DeleteSeriesProperty =
      DependencyProperty.Register("DeleteSeries", typeof(bool), typeof(DeleteRecurrenceDialog),
      new PropertyMetadata(new PropertyChangedCallback(OnDeleteSeriesChanged)));

    private static void OnDeleteSeriesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((DeleteRecurrenceDialog)d).OnDeleteSeriesChanged();
    }

    private void OnDeleteSeriesChanged()
    {
      if (DeleteSeries)
      {
        DeleteOccurrence = false;
      }
    }

    #endregion // DeleteSeries property

    #region DeleteOccurrence Property

    /// <summary>
    /// Gets or sets the DeleteOccurrence.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="DeleteOccurrenceProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public bool DeleteOccurrence
    {
      get { return (bool)GetValue(DeleteOccurrenceProperty); }
      set { SetValue(DeleteOccurrenceProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="DeleteOccurrence"/> property.
    /// </summary>
    public static readonly DependencyProperty DeleteOccurrenceProperty =
      DependencyProperty.Register("DeleteOccurrence", typeof(bool), typeof(DeleteRecurrenceDialog),
      new FrameworkPropertyMetadata(true, OnDeleteOccurrenceChanged));

    private static void OnDeleteOccurrenceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((DeleteRecurrenceDialog)d).OnDeleteOccurrenceChanged();
    }

    private void OnDeleteOccurrenceChanged()
    {
      if (DeleteOccurrence)
      {
        DeleteSeries = false;
      }
    }

    #endregion // DeleteOccurrence Property

    #region Title Property

    /// <summary>
    /// Gets or sets the Title.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="TitleProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public string Title
    {
      get { return (string)GetValue(TitleProperty); }
      set { SetValue(TitleProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="Title"/> property.
    /// </summary>
    public static readonly DependencyProperty TitleProperty =
      DependencyProperty.Register("Title", typeof(string), typeof(DeleteRecurrenceDialog),
      new FrameworkPropertyMetadata("Confirm Delete", OnTitleChanged));

    private static void OnTitleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((DeleteRecurrenceDialog)d).OnTitleChanged();
    }

    private void OnTitleChanged()
    {
      Window wnd = Parent as Window;
      if (wnd != null)
      {
        wnd.Title = Title;
      }
    }

    #endregion // Title Property

#if !SILVERLIGHT
    private bool _dialogResult = false;

    /// <summary>
    /// Gets the dialog result.
    /// </summary>
    public bool DialogResult
    {
      get { return _dialogResult; }
      private set
      {
        _dialogResult = value;
        Window wnd = Parent as Window;// VisualTreeUtils.FindAncestor<Window>(this);
        if (wnd != null)
        {
          wnd.DialogResult = DialogResult;
        }
        OnClosed();
      }
    }

    /// <summary>
    /// Raised when this <see cref="DeleteRecurrenceDialog"/> is closed.
    /// </summary>
    public event RoutedEventHandler Closed;

    internal void OnClosed()
    {
      RoutedEventHandler handler = Closed;
      if (handler != null)
      {
        handler(this, new RoutedEventArgs());
      }
    }
#endif
  }
}

