using System;
using System.ComponentModel;
using System.Globalization;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using System.Windows.Media;
using Microsoft.WindowsAPICodePack.Taskbar;

namespace Utilities.ProgressDialog
{
  /// <summary>
  /// A simple progress dialog that invokes clients via
  /// a synchronous event which is called on a worker thread.
  /// </summary>
  /// <example>
  /// This example creates a new dialog instance, registers an
  /// event handler for the worker thread, and displays the dialog
  /// by invoking the <see cref="RunWorkerThread(DoWorkEventHandler)"/>
  /// or <see cref="RunWorkerThread(object,DoWorkEventHandler)"/> methods:
  ///  <code>
  /// //declare background worker method
  /// DoWorkEventHandler handler = delegate
  /// {
  ///   SaveProject();
  /// }
  /// 
  /// //init progress dialog
  /// ProgressDialog dlg = new ProgressDialog("Saving project...");
  /// dlg.AutoIncrementInterval = 200;
  /// dlg.Owner = Application.Current.MainWindow;
  /// 
  /// //run work
  /// dlg.RunWorkerThread(handler);
  /// 
  /// if (dlg.Error != null)
  /// {
  ///   Console.Out.Writeline("An error occurred: " + dlg.Error.Message);
  /// }
  /// </code>
  /// </example>
  public partial class ProgressDialog : Window
  {
    #region fields

      bool bClosed;

      ManualResetEvent workTerminated = new ManualResetEvent(false);
    /// <summary>
    /// The background worker which handles asynchronous invocation
    /// of the worker method.
    /// </summary>
    private readonly BackgroundWorker worker;

    /// <summary>
    /// The timer to be used for automatic progress bar updated.
    /// </summary>
    private readonly DispatcherTimer progressTimer;

    /// <summary>
    /// The UI culture of the thread that invokes the dialog.
    /// </summary>
    private CultureInfo uiCulture;

    /// <summary>
    /// If set, the interval in which the progress bar
    /// gets incremented automatically.
    /// </summary>
    private int? autoIncrementInterval = null;

    /// <summary>
    /// If set, the interval in which the progress bar
    /// gets incremented automatically.
    /// </summary>
    private int autoShowDelay = 1000;

    /// <summary>
    /// Whether background processing was cancelled by the user.
    /// </summary>
    private bool cancelled = false;

    /// <summary>
    /// Defines the size of a single increment of the progress bar.
    /// Defaults to 5.
    /// </summary>
    private int progressBarIncrement = 5;

    /// <summary>
    /// Provides an exception that occurred during the asynchronous
    /// operation on the worker thread. Defaults to null, which
    /// indicates that no exception occurred at all.
    /// </summary>
    private Exception error = null;

    /// <summary>
    /// The result, if assigned to the <see cref="DoWorkEventArgs.Result"/>
    /// property by the worker method.
    /// </summary>
    private object result = null;

    private static TaskbarManager windowsTaskbar = TaskbarManager.Instance;

    /// <summary>
    /// The 
    /// </summary>
    private DoWorkEventHandler workerCallback;

    #endregion


    #region properties

    /// <summary>
    /// Gets or sets the dialog text.
    /// </summary>
    public string DialogText
    {
      get { return txtDialogMessage.Text; }
      set { txtDialogMessage.Text = value; }
    }


    /// <summary>
    /// Whether to enable cancelling the process. This basically
    /// shows or hides the Cancel button. Defaults to false.
    /// </summary>
    public bool IsCancellingEnabled
    {
      get { return btnCancel.IsVisible; }
      set { btnCancel.Visibility = value ? Visibility.Visible : Visibility.Collapsed; }
    }


    /// <summary>
    /// Whether the process was cancelled by the user.
    /// </summary>
    public bool Cancelled
    {
      get { return cancelled; }
    }

    /// <summary>
    /// If set, the interval in which the progress bar
    /// gets incremented automatically.
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException">If the interval
    /// is lower than 100 ms.</exception>
    public int? AutoIncrementInterval
    {
      get { return autoIncrementInterval; }
      set
      {
        if (value.HasValue && value < 100) throw new ArgumentOutOfRangeException("value");
        autoIncrementInterval = value;
      }
    }

    /// <summary>
    /// Defines the size of a single increment of the progress bar.
    /// The default value is 5, with a progress bar range of 0 - 100.
    /// </summary>
    public int ProgressBarIncrement
    {
      get { return progressBarIncrement; }
      set { progressBarIncrement = value; }
    }

    public double ProgressBarMaximum
    {
        get { return progressBar.Maximum; }
        set { progressBar.Maximum = value; }
    }

    public double ProgressBarMinimum
    {
        get { return progressBar.Minimum; }
        set { progressBar.Minimum = value; }
    }

    public int AutoShowDelay
    {
        get { return autoShowDelay; }
        set { autoShowDelay = value; }
    }
    /// <summary>
    /// Provides an exception that occurred during the asynchronous
    /// operation on the worker thread. Defaults to null, which
    /// indicates that no exception occurred at all.
    /// </summary>
    public Exception Error
    {
      get { return error; }
    }

    /// <summary>
    /// The result, if assigned to the <see cref="DoWorkEventArgs.Result"/>
    /// property by the worker method. Defaults to null.
    /// </summary>
    public object Result
    {
      get { return result; }
    }


    /// <summary>
    /// Shows or hides the progressbar control. Defaults to
    /// true.
    /// </summary>
    public bool ShowProgressBar
    {
      get { return progressBar.Visibility == Visibility.Visible; }
        set { progressBar.Visibility = value ? Visibility.Visible : Visibility.Collapsed; }
    }

    public bool ProgressBarIndeterminate
    {
        get { return progressBar.IsIndeterminate; }
        set { progressBar.IsIndeterminate = value; }
    }
    
      private bool bDispatchEnabled;
      private bool? dialogResult;
    #endregion


    /// <summary>
    /// Inits the dialog with a given dialog text.
    /// </summary>
    public ProgressDialog(string dialogText) : this()
    {
      DialogText = dialogText;
    }


    /// <summary>
    /// Inits the dialog without displaying it.
    /// </summary>
    public ProgressDialog()
    {
      InitializeComponent();

      //init the timer
      progressTimer = new DispatcherTimer(DispatcherPriority.SystemIdle, Dispatcher);
      progressTimer.Tick += OnProgressTimer_Tick;

      //init background worker
      worker = new BackgroundWorker() { WorkerReportsProgress = true, WorkerSupportsCancellation = true };

      worker.DoWork += worker_DoWork;
      worker.ProgressChanged += worker_ProgressChanged;
      worker.RunWorkerCompleted += worker_RunWorkerCompleted;
    }

    protected override void OnClosed(EventArgs e)
    {
        bClosed = true;

        base.OnClosed(e);
        try
        {
            workTerminated.Set();
            workTerminated.Dispose();
        }
        catch (Exception ex)
        {
            
        }
    }

    #region run worker thread

    /// <summary>
    /// Launches a worker thread which is intendet to perform
    /// work while progress is indicated.
    /// </summary>
    /// <param name="workHandler">A callback method which is
    /// being invoked on a background thread in order to perform
    /// the work to be performed.</param>
    public bool RunWorkerThread(DoWorkEventHandler workHandler)
    {
      return RunWorkerThread(null, workHandler);
    }


    /// <summary>
    /// Launches a worker thread which is intended to perform
    /// work while progress is indicated, and displays the dialog
    /// modally in order to block the calling thread.
    /// </summary>
    /// <param name="argument">A custom object which will be
    /// submitted in the <see cref="DoWorkEventArgs.Argument"/>
    /// property <paramref name="workHandler"/> callback method.</param>
    /// <param name="workHandler">A callback method which is
    /// being invoked on a background thread in order to perform
    /// the work to be performed.</param>
    public bool RunWorkerThread(object argument, DoWorkEventHandler workHandler)
    {
        if (TaskbarManager.IsPlatformSupported)
            windowsTaskbar.SetProgressState(ProgressBarIndeterminate ? TaskbarProgressBarState.Indeterminate : TaskbarProgressBarState.Normal);

      if (autoIncrementInterval.HasValue)
      {
        //run timer to increment progress bar
        progressTimer.Interval = TimeSpan.FromMilliseconds(autoIncrementInterval.Value);
        progressTimer.Start();
      }

      //store the UI culture
      uiCulture = CultureInfo.CurrentUICulture;

      //store reference to callback handler and launch worker thread
      workerCallback = workHandler;
      worker.RunWorkerAsync(argument);

      //display modal dialog (blocks caller)
      if (autoShowDelay != 0)
      {
          // Thread.Sleep(autoShowDelay);
          using (new WaitCursor())
          {
              var now = DateTime.Now + TimeSpan.FromMilliseconds(autoShowDelay);
              try
              {
                  while (!workTerminated.WaitOne(0, false) && (now > DateTime.Now))
                      WaitForPriority.DoEventsSync();
              }
              catch (Exception ex)
              {
                  
              }
              //if (dialogResult != null)
              //    return dialogResult.Value;
          }
      }
      try
      {
          if (!worker.IsBusy || workTerminated.WaitOne(0, false))
          {
              //if (dialogResult != null)
              //    return dialogResult.Value;
              workTerminated.Dispose();
              return false;
          }
      }
      catch (Exception ex)
      {
          return false;
      }
      if (bClosed)
          return false;

      bDispatchEnabled = true;
            try
            {
                return ShowDialog() ?? false;
            }
            catch
            {
                return false;
            }
    }

    #endregion


    #region event handlers

    /// <summary>
    /// Worker method that gets called from a worker thread.
    /// Synchronously calls event listeners that may handle
    /// the work load.
    /// </summary>
    private void worker_DoWork(object sender, DoWorkEventArgs e)
    {
      try
      {
        //make sure the UI culture is properly set on the worker thread
        Thread.CurrentThread.CurrentUICulture = uiCulture;

        //invoke the callback method with the designated argument
        workerCallback(sender, e);
        workTerminated.Set();
      }
      catch (Exception)
      {
        //disable cancelling and rethrow the exception
          if (bDispatchEnabled)
        Dispatcher.BeginInvoke(DispatcherPriority.Send,
                               (SendOrPostCallback) delegate { btnCancel.SetValue(Button.IsEnabledProperty, false); },
                               null);

        throw;
      }
    }


    /// <summary>
    /// Cancels the background worker's progress.
    /// </summary>
    private void btnCancel_Click(object sender, RoutedEventArgs e)
    {
      btnCancel.IsEnabled = false;
      worker.CancelAsync();
      cancelled = true;
    }


    /// <summary>
    /// Visually indicates the progress of the background operation by
    /// updating the dialog's progress bar.
    /// </summary>
    private void worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
    {
        if (!bDispatchEnabled)
            return;

      if (!Dispatcher.CheckAccess())
      {
        //run on UI thread
        ProgressChangedEventHandler handler = worker_ProgressChanged;
        Dispatcher.Invoke(DispatcherPriority.Send, handler, new object[] {sender, e}, null);
        return;
      }

      if (e.ProgressPercentage != int.MinValue)
      {
        progressBar.Value = e.ProgressPercentage;

        if (TaskbarManager.IsPlatformSupported)
            windowsTaskbar.SetProgressValue((int)progressBar.Value, (int)ProgressBarMaximum);
      }

      lblStatus.Content = e.UserState;
    }


    /// <summary>
    /// Updates the user interface once an operation has been completed and
    /// sets the dialog's <see cref="Window.DialogResult"/> depending on the value
    /// of the <see cref="AsyncCompletedEventArgs.Cancelled"/> property.
    /// </summary>
    private void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
    {
        if (!bDispatchEnabled)
        {
            if (e.Error != null)
            {
                error = e.Error;
            }
            else if (!e.Cancelled)
            {
                //assign result if there was neither exception nor cancel
                result = e.Result;
            }

            //update UI in case closing the dialog takes a moment
            progressTimer.Stop();

            //set the dialog result, which closes the dialog
            try
            {
                dialogResult = error == null && !e.Cancelled;
            }
            catch
            {

            }
            return;
        }

      if (!Dispatcher.CheckAccess())
      {
        //run on UI thread
        RunWorkerCompletedEventHandler handler = worker_RunWorkerCompleted;
        Dispatcher.Invoke(DispatcherPriority.SystemIdle, handler, new object[] {sender, e}, null);
        return;
      }

      if (e.Error != null)
      {
        error = e.Error;
      }
      else if (!e.Cancelled)
      {
        //assign result if there was neither exception nor cancel
        result = e.Result;
      }

      //update UI in case closing the dialog takes a moment
      progressTimer.Stop();
      progressBar.Value = progressBar.Maximum;
      btnCancel.IsEnabled = false;

      //set the dialog result, which closes the dialog
      try
      {
          DialogResult = error == null && !e.Cancelled;
      }
      catch
      {

      }
      if (TaskbarManager.IsPlatformSupported)
          windowsTaskbar.SetProgressState(TaskbarProgressBarState.NoProgress);
    }


    /// <summary>
    /// Periodically increments the value of the progress bar.
    /// </summary>
    private void OnProgressTimer_Tick(object sender, EventArgs e)
    {
        if (!bDispatchEnabled)
            return;
        int threshold = 100 + progressBarIncrement;
      progressBar.Value = ((progressBar.Value + progressBarIncrement)%threshold);
    }

    #endregion


    #region update progress bar / status label

    /// <summary>
    /// Directly updates the value of the underlying
    /// progress bar. This method can be invoked from a worker thread.
    /// </summary>
    /// <param name="progress"></param>
    /// <exception cref="ArgumentOutOfRangeException">If the
    /// value is not between 0 and 100.</exception>
    public void UpdateProgress(int progress)
    {
        if (!bDispatchEnabled)
            return;
        if (!Dispatcher.CheckAccess())
      {
        //switch to UI thread
        Dispatcher.BeginInvoke(DispatcherPriority.Send,
                                     (SendOrPostCallback)
                                     delegate { UpdateProgress(progress); }, null);
        return;  
      }


      //validate range
      if (progress < progressBar.Minimum || progress > progressBar.Maximum)
      {
        string msg = "Only values between {0} and {1} can be assigned to the progress bar.";
        msg = String.Format(msg, progressBar.Minimum, progressBar.Maximum);
        throw new ArgumentOutOfRangeException("progress", progress, msg);
      }

      //set the progress bar's value
      progressBar.Value = progress;

      if (TaskbarManager.IsPlatformSupported)
          windowsTaskbar.SetProgressValue((int)progressBar.Value, (int)ProgressBarMaximum);
    }


    /// <summary>
    /// Sets the content of the status label to a given value. This method
    /// can be invoked from a worker thread.
    /// </summary>
    /// <param name="status">The status to be displayed.</param>
    public void UpdateStatus(object status)
    {
        if (!bDispatchEnabled)
            return;
        Dispatcher.BeginInvoke(DispatcherPriority.Send,
                             (SendOrPostCallback) delegate { lblStatus.SetValue(ContentProperty, status); }, null);
    }

    #endregion


    #region invoke methods on UI thread

    /// <summary>
    /// Asynchronously invokes a given method on the thread
    /// of the dialog's dispatcher.
    /// </summary>
    /// <param name="method">The method to be invoked.</param>
    /// <param name="priority">The priority of the operation.</param>
    /// <returns>The result of the
    /// <see cref="Dispatcher.BeginInvoke(DispatcherPriority,Delegate)"/>
    /// method.</returns>
    public DispatcherOperation BeginInvoke(Delegate method, DispatcherPriority priority)
    {
        if (!bDispatchEnabled)
            return null;
        return Dispatcher.BeginInvoke(priority, method);
    }


    /// <summary>
    /// Synchronously invokes a given method on the thread
    /// of the dialog's dispatcher.
    /// </summary>
    /// <param name="method">The method to be invoked.</param>
    /// <param name="priority">The priority of the operation.</param>
    /// <returns>The result of the
    /// <see cref="Dispatcher.Invoke(DispatcherPriority,Delegate)"/>
    /// method.</returns>
    public object Invoke(Delegate method, DispatcherPriority priority)
    {
        if (!bDispatchEnabled)
            return null;
        return Dispatcher.Invoke(priority, method);
    }

    #endregion

  }
}