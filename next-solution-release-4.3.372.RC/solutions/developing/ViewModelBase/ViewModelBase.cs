using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Diagnostics;
using System.Windows.Input;
using System.Runtime.Serialization;
using System.Threading;
using System.Collections.Generic;
using System.Reflection;
using System.Windows;
using System.Collections.Concurrent;
using System.Threading.Tasks;
#if !NET_STANDARD
#if WINDOWS_UWP
using Windows.UI.Core;
using Windows.UI.Xaml;
#else
using Utilities;
using System.Windows.Threading;
using System.Windows.Controls;
using ViewModelLib.Performances;
#endif
#endif

namespace ViewModelLib
{





    /// <summary>
    /// Base class for all ViewModel classes in the application.
    /// It provides support for property change notifications 
    /// and has a DisplayName property.  This class is abstract.
    /// </summary>
    [DataContract(Name = "ViewModelBase", Namespace = UFInterfaces.Constants.Namespaces.UriProgea)]
    public abstract class ViewModelBase : INotifyPropertyChanged, IDisposable, IDataErrorInfo
#if !WINDOWS_UWP && !NET_STANDARD
        , IParentablePropertyExposer
#endif
    {
        #region Members

        public object lockObject { get; protected set; } = new object();
        Object lockObjectThread = new Object();
        //Object lockObjectTimer = new Object();
        Object operationLockObject = new Object();
#if !WINDOWS_UWP && !NET_STANDARD
        static Mediator _mediator;
        volatile BackgroundWorker backgroundWorker;
        volatile DispatcherOperation IdleExecutionPending;
        protected volatile Dispatcher m_dispatcher;
#endif
#endregion

#region Constructor

        protected ViewModelBase()
        {
            Initialize();

            BackgroundWorkProcessed = false;
#if !WINDOWS_UWP && !NET_STANDARD
            RepromoteBackgroundProcess = false;
#endif
        }

        private void Initialize()
        {
            lockObject = new object();
            lockObjectThread = new Object();
            //lockObjectTimer = new Object();
            operationLockObject = new Object();
        }

        [OnDeserializing]
        private void Initialize(StreamingContext context)
        {
            Initialize();
        }

#endregion // Constructor

#region DisplayName

        /// <summary>
        /// Returns the user-friendly name of this object.
        /// Child classes can set this property to a new value,
        /// or override it to determine the value on-demand.
        /// </summary>

        String _title;
#if !WINDOWS_UWP
        [Browsable(false)]
#endif
        public virtual string Title 
        { 
            get
            {
                return _title;
            }
            set
            {
                if (value != _title)
                {
                    _title = value;
                    OnPropertyChanged("Title");
                }
            }
        }

#if !WINDOWS_UWP
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
#endif
        public virtual bool BackgroundWorkProcessed { get; protected set; }

#if !WINDOWS_UWP
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
#endif
        public virtual bool RepromoteBackgroundProcess { get; protected set; }

        private Boolean isBusy = false;
#if !WINDOWS_UWP
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
#endif
        public Boolean IsBusy
        {
            get { return isBusy; }
            set
            {
                isBusy = value;
                OnPropertyChanged("IsBusy");
            }
        }

#endregion // DisplayName

#if !WINDOWS_UWP && !NET_STANDARD
#region Debugging Aides

        /// <summary>
        /// Warns the developer if this object does not have
        /// a public property with the specified name. This 
        /// method does not exist in a Release build.
        /// </summary>
        [Conditional("DEBUG")]
        [DebuggerStepThrough]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void VerifyPropertyName(string propertyName)
        {
            // Verify that the property name matches a real,  
            // public, instance property on this object.
            if (TypeDescriptor.GetProperties(this)[propertyName] == null)
            {
                string msg = String.Format("Invalid property name: {0}", propertyName);

                if (ThrowOnInvalidPropertyName)
                    throw new Exception(msg);
                else
                    Debug.WriteLine(msg);
            }
        }

        /// <summary>
        /// Returns whether an exception is thrown, or if a Debug.Fail() is used
        /// when an invalid property name is passed to the VerifyPropertyName method.
        /// The default value is false, but subclasses used by unit tests might 
        /// override this property's getter to return true.
        /// </summary>
        protected virtual bool ThrowOnInvalidPropertyName { get; private set; }

#endregion // Debugging Aides

#region MonitorQueue
        private long _operationsQueueCount = 0;
        private List<DispatcherOperation> _operations;
        private Dictionary<Dispatcher, List<String>> _pendingOperations;
        private IDisposable observableTimer;
        private static readonly Object staticLockObject = new Object();
        private static readonly LatencyRecorder latencyRecorder = new LatencyRecorder();
        private static readonly Dictionary<DispatcherOperation, Latency> _pendingLatencies = new Dictionary<DispatcherOperation, Latency>();
        private static readonly ProcessorMonitor processorMonitor = new ProcessorMonitor();
        private static readonly TimeSpan StatsFrequency = TimeSpan.FromSeconds(1);
        private static StatisticData statisticData;

        public static TimeSpan StatisticDataTimeSpan
        {
            get
            {
                return StatsFrequency;
            }
        }

        public static void RefreshStatisticData()
        {
            lock (staticLockObject)
            {
                if (statisticData == null)
                    return;

                var stats = latencyRecorder.CalculateAndReset();
                if (stats == null)
                    return;

                statisticData.PendingCount = _pendingLatencies.Count;
                statisticData.UiLatency = stats.UiLatencyMax;
                statisticData.UiUpdates = stats.RenderedCount;
                statisticData.TicksReceived = stats.ReceivedCount;

                statisticData.Histogram = stats.Histogram;

                statisticData.ServerClientLatency = stats.ServerLatencyMax + "ms";
                statisticData.TotalLatency = stats.TotalLatencyMax + "ms";

                if (processorMonitor.IsAvailable)
                {
                    var cpuTime = processorMonitor.CalculateProcessingAndReset();
                    statisticData.CpuTime = Math.Round(cpuTime.TotalMilliseconds, 0).ToString();
                    statisticData.CpuPercent = Math.Round(cpuTime.TotalMilliseconds / (Environment.ProcessorCount * StatsFrequency.TotalMilliseconds) * 100, 0).ToString();
                }


                var threads = System.Diagnostics.Process.GetCurrentProcess().Threads;
                statisticData.ThreadCount = threads.Count;
            }
        }

        public static StatisticData StatisticData
        {
            get
            {
                lock (staticLockObject)
                {
                    if (statisticData == null)
                    {
                        statisticData = new StatisticData();
                    }
                    return statisticData;
                }
            }
        }

        private void MonitorDispatcherQueue(long l)
        {
            if (_operationsQueueCount != 0)
                Debug.WriteLine(String.Format("Dispatcher Operations In Queue {0}, ", _operationsQueueCount));

            if (_operationsQueueCount > 10)
            {
                Debug.WriteLine("Pushing all Dispatcher operations");
                DispatcherOperation disp = null;
                lock (operationLockObject)
                {
                    disp = _operations.First();
                }

                if (disp == null || disp.Dispatcher.Thread == null || !disp.Dispatcher.Thread.IsAlive)
                    // disp.Dispatcher.DoEvents();
                // else
                {
                    lock (operationLockObject)
                    {
                        var currentqueue = Interlocked.Decrement(ref _operationsQueueCount);

                        if (currentqueue == 0 && observableTimer != null)
                        {
                            observableTimer.Dispose();
                            observableTimer = null;
                        }

                        _operations.Remove(disp);
                    }
                }
                // Application.Current.DoEvents();
                // _operations.Clear();
                // Interlocked.Exchange(ref _operationsQueueCount, 0);
            }
        }
#endregion
#endif

#region INotifyPropertyChanged Members

        protected virtual string GetPropertyName<TResult>
          (Expression<Func<TResult>> property)
        {
            // Convert expression to a property name
            string propertyName = ((MemberExpression)property.
                Body).Member.Name;

            return propertyName;
        }

        /// <summary>
        /// Raised when a property on this object has a new value.
        /// </summary>
        public virtual event PropertyChangedEventHandler PropertyChanged;
        public static bool bDoNotCheckDispatcherObjects = false;
        
        /// <summary>
        /// Raises this object's PropertyChanged event.
        /// </summary>
        /// <param name="propertyName">The property that has a new value.</param>
        /// 
        protected virtual void OnPropertyChanged(string propertyName)
        {
            NotifyPropertyChanged(propertyName
#if !WINDOWS_UWP && !NET_STANDARD
                , null
#endif
                );
        }

        // static List<DispatcherOperation> listOperations = new List<DispatcherOperation>();
#if WINDOWS_UWP
        async
#endif
        void NotifyPropertyChanged(string propertyName
#if !WINDOWS_UWP && !NET_STANDARD
            , Dispatcher dispatcher
#endif
            )
        {
            if (bDisposed)
                return;

            // VerifyPropertyName(propertyName);

            PropertyChangedEventHandler hndler = PropertyChanged;
            if (hndler != null)
            {
                var e = new PropertyChangedEventArgs(propertyName);

                // Walk thru invocation list.
                Delegate[] delegates = hndler.GetInvocationList();

                //var disp = (from PropertyChangedEventHandler nh in delegates
                //                            let dpo = nh.Target as DispatcherObject
                //                            where dpo != null
                //                            select dpo.Dispatcher).FirstOrDefault();

                //if (disp == null)
                //    disp = dispatcher;

                foreach (PropertyChangedEventHandler handler in delegates)
                {
#if !NET_STANDARD
#if !WINDOWS_UWP
                    var dispobj = handler.Target as DependencyObject;
                    if (dispatcher != null)
                    {
                        if (dispobj != null && dispobj.Dispatcher != null && dispobj.Dispatcher == dispatcher)
                            handler.Invoke(this, e);
                        continue;
                    }

                    if (!bDoNotCheckDispatcherObjects && dispobj != null && dispobj.Dispatcher != null &&
                        !dispobj.Dispatcher.HasShutdownStarted && !dispobj.Dispatcher.HasShutdownFinished &&
                        dispobj.Dispatcher.Thread.IsAlive && !dispobj.Dispatcher.CheckAccess())
#else
                    if (!Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.HasThreadAccess)
#endif
                    {
#if !WINDOWS_UWP
                        var latency = new Latency();
                        DispatcherOperation operation = null;
                        lock(staticLockObject)
                        {
                            latencyRecorder.OnReceived(latency);
                        }

                        lock (operationLockObject)
                        {
                            if (_pendingOperations == null)
                                _pendingOperations = new Dictionary<Dispatcher, List<String>>();
                            if (_pendingOperations.ContainsKey(dispobj.Dispatcher) && 
                                _pendingOperations[dispobj.Dispatcher].Contains(propertyName))
                                continue;
                            if (!_pendingOperations.ContainsKey(dispobj.Dispatcher))
                            {
                                _pendingOperations.Add(dispobj.Dispatcher, new List<String>());
                                dispobj.Dispatcher.ShutdownFinished += Dispatcher_ShutdownFinished;
                            }

                            operation = dispobj.Dispatcher.BeginInvoke(DispatcherPriority.Background,
                                (Action)(() => NotifyPropertyChanged(propertyName, dispobj.Dispatcher)));
                            operation.Completed += (s, o) =>
                            {
                                var s1 = (DispatcherOperation)s;
                                OperationCompletedOrAborted(s1, latency, propertyName);
                            };
                            operation.Aborted += (s, o) =>
                            {
                                var s1 = (DispatcherOperation)s;
                                OperationCompletedOrAborted(s1, latency, propertyName);
                            };
                            if (operation.Status != DispatcherOperationStatus.Completed &&
                                operation.Status != DispatcherOperationStatus.Aborted)
                            {
                                var current = Interlocked.Increment(ref _operationsQueueCount);
                                if (current == 1)
                                {
                                    if (_operations == null)
                                        _operations = new List<DispatcherOperation>();
                                    //if (observableTimer == null)
                                    //    observableTimer = System.Linq.Observable.Interval(TimeSpan.FromMilliseconds(2000)).Subscribe(MonitorDispatcherQueue);
                                }

                                _operations.Add(operation);
                                _pendingOperations[dispobj.Dispatcher].Add(propertyName);
                            }
                        }

                        lock (staticLockObject)
                        {
                            if (operation.Status != DispatcherOperationStatus.Completed &&
                                operation.Status != DispatcherOperationStatus.Aborted)
                                _pendingLatencies.Add(operation, latency);
                            else
                                latencyRecorder.OnRendered(latency);
                        }
#else
                        Utilities.RunOnUIThread.Run(() => NotifyPropertyChanged(propertyName));
#endif
                    }
                    else
#endif
                    {
                        handler.Invoke(this, e);
                    }
                }
            }
                //DependencyObject dispatcherObject = handler.Target as DependencyObject;

                //// If the subscriber is a DispatcherObject and different thread
                //if (dispatcherObject != null && dispatcherObject.CheckAccess() == false && dispatcherObject.Dispatcher.Thread.IsAlive)
                //{
                //    /*var disp = */
                //    dispatcherObject.Dispatcher.BeginInvoke(DispatcherPriority.DataBind, handler, this, e);

                //    //dispatcherObject.Dispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle, (Action)(() =>
                //    //    {
                //    //        // Dirty the commands registered with CommandManager,
                //    //        // such as our Save command, so that they are queried
                //    //        // to see if they can execute now.
                //    //        System.Windows.Input.CommandManager.InvalidateRequerySuggested();
                //    //    }));
                //    //lock (lockObjectThread)
                //    //{
                //    //    listOperations.Add(disp);
                //    //}

                //    //disp.Aborted += (o, ev) =>
                //    //    {
                //    //        handler(this, e);
                //    //        lock (lockObjectThread)
                //    //        {
                //    //            if (listOperations.Contains(disp))
                //    //                listOperations.Remove(disp);
                //    //        }
                //    //    };
                //    //disp.Completed += (o, ev) =>
                //    //    {
                //    //        lock (lockObjectThread)
                //    //        {
                //    //            if (listOperations.Contains(disp))
                //    //                listOperations.Remove(disp);
                //    //        }
                //    //    };

                //}
                //else // Execute handler as is
                //{
                //    handler(this, e);

                //    // Dirty the commands registered with CommandManager,
                //    // such as our Save command, so that they are queried
                //    // to see if they can execute now.
                //    // System.Windows.Input.CommandManager.InvalidateRequerySuggested();
                //}
            // }
            // Debug.WriteLine("Pending operation {0} on {1}", listOperations.Count, Title);
        }

#if !NET_STANDARD
        void Dispatcher_ShutdownFinished(object sender, EventArgs e)
        {
            var dispatcher = (Dispatcher)sender;
            dispatcher.ShutdownFinished -= Dispatcher_ShutdownFinished;

            lock (operationLockObject)
            {
                _pendingOperations.Remove(dispatcher);
            }
        }

        void OperationCompletedOrAborted(DispatcherOperation s1, Latency latency, string propertyName)
        {
            lock (staticLockObject)
            {
                latencyRecorder.OnRendered(latency);
                _pendingLatencies.Remove(s1);
            }

            lock (operationLockObject)
            {
                if (_operations != null && _operations.Remove(s1))
                {
                    var currentqueue = Interlocked.Decrement(ref _operationsQueueCount);
                    if (currentqueue == 0 && observableTimer != null)
                    {
                        observableTimer.Dispose();
                        observableTimer = null;
                    }

	                if (_pendingOperations.ContainsKey(s1.Dispatcher))
	                    _pendingOperations[s1.Dispatcher].Remove(propertyName);
                }
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public void NotifyOfPropertyChange<TProperty>(Expression<Func<TProperty>> property)
        {
            var lambda = (LambdaExpression)property;

            MemberExpression memberExpression;
            if (lambda.Body is UnaryExpression)
            {
                var unaryExpression = (UnaryExpression)lambda.Body;
                memberExpression = (MemberExpression)unaryExpression.Operand;
            }
            else memberExpression = (MemberExpression)lambda.Body;

            OnPropertyChanged(memberExpression.Member.Name);
        }

        protected void NotifyPropertyChanged(PropertyChangedEventArgs args)
        {
            PropertyChangedEventHandler handler = PropertyChanged;

            if (handler != null)
            {
                handler(this, args);
            }
        }
#endif
#endregion // INotifyPropertyChanged Members

#region IParentablePropertyExposer
#if !WINDOWS_UWP && !NET_STANDARD
        /// <summary>
        /// Returns the list of delegates that are currently subscribed for the
        /// <see cref="System.ComponentModel.INotifyPropertyChanged">INotifyPropertyChanged</see>
        /// PropertyChanged event
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public Delegate[] GetINPCSubscribers()
        {
            return PropertyChanged == null ? null : PropertyChanged.GetInvocationList();
        }
#endif
#endregion

#region IDataErrorInfo Members
#if !WINDOWS_UWP
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool IsValid
        {
            get 
            {
                if (bDisposed)
                    return false;
                if (!string.IsNullOrEmpty(Error))
                    return false;

                PropertyInfo[] properties = GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
                foreach (PropertyInfo property in properties)
                {
                    if (!String.IsNullOrEmpty(PerformValidation(property.Name)))
                        return false;
                }

                return true;
            }
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual string Error 
        {
            get
            {
                var context = new ValidationContext(this, null, null);
                var results = new List<System.ComponentModel.DataAnnotations.ValidationResult>();

                return !Validator.TryValidateObject(this, context, results)
                    ? string.Join(Environment.NewLine, results.Select(x => x.ErrorMessage))
                    : null;
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual string this[string propertyName] 
        {
            get
            {
                String s = PerformValidation(propertyName);
                if (!String.IsNullOrEmpty(s))
                    return s;
                var context = new ValidationContext(this, null, null)
                {
                    MemberName = propertyName
                };

                var results = new List<System.ComponentModel.DataAnnotations.ValidationResult>();
                var propertyInfo = GetType().GetProperty(propertyName);
                if (propertyInfo != null)
                {
                    var value = propertyInfo.GetValue(this, null);

                    return !Validator.TryValidateProperty(value, context, results)
                        ? string.Join(Environment.NewLine, results.Select(x => x.ErrorMessage))
                        : null;
                }

                return null;
            }
        }

        protected virtual String PerformValidation(String propertyName)
        {
            // Dirty the commands registered with CommandManager,
            // such as our Save command, so that they are queried
            // to see if they can execute now.
            // CommandManager.InvalidateRequerySuggested();

            return null;
        }
#endif
#endregion

#region ProcessAsync
#if !WINDOWS_UWP && !NET_STANDARD
        protected static void ProcessAsync(Action method)
        {
            ThreadPool.QueueUserWorkItem((_) => method());
        }

        protected static void ProcessAsync<T>(Action<T> method, T parameter)
        {
            ThreadPool.QueueUserWorkItem((_) => method(parameter));
        }

        protected void ProcessOnDispatcherThread(Action method)
        {
            m_dispatcher.BeginInvoke(method, null);
        }

        protected void ProcessOnDispatcherThread(Action method, DispatcherPriority priority)
        {
            m_dispatcher.BeginInvoke(priority, method);
        }

        protected void ProcessOnDispatcherThread<T>(Action<T> method, T parameter)
        {
            m_dispatcher.BeginInvoke(method, parameter);
        }

        protected void ProcessOnDispatcherThread<T1, T2>(Action<T1, T2> method, T1 parameter, T2 parameter2)
        {
            m_dispatcher.BeginInvoke(method, parameter, parameter2);
        }

        protected void ProcessOnDispatcherThread<T>(Action<T> method, T parameter, DispatcherPriority priority)
        {
            m_dispatcher.BeginInvoke(method, priority, parameter);
        }

        protected static void WithUpdateProgress(Func<Window> createUpdateWindow, Action longRunningOperation)
        {
            var dispatcherStart = new object();
            Window updateWindow = null;
            var thread = new Thread(() =>
            {
                lock (dispatcherStart)
                {
                    updateWindow = createUpdateWindow();
                    updateWindow.Closed += (sender, e) => updateWindow.Dispatcher.InvokeShutdown();
                    updateWindow.Show();
                }
                Dispatcher.Run();
            });

            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
            try
            {
                longRunningOperation();
            }
            finally
            {
                lock (dispatcherStart)
                {
                    updateWindow.Dispatcher.BeginInvoke((Action)(() => updateWindow.Close()));
                }
            }
        }

        void UnsubscribeDispatcher()
        {
            lock (operationLockObject)
            {
                if (_pendingOperations != null)
                {
                    foreach (var dispatcher in _pendingOperations.Keys)
                        dispatcher.ShutdownFinished -= Dispatcher_ShutdownFinished;
                }
            }
        }
#endif
#endregion

#region IDisposable Members

        /// <summary>
        /// Invoked when this object is being removed from the application
        /// and will be subject to garbage collection.
        /// </summary>
        /// 
        bool bDisposed;
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void Dispose()
        {
            if (bDisposed)
                return;
            
            OnDispose();
            bDisposed = true;
        }

        /// <summary>
        /// Child classes can override this method to perform 
        /// clean-up logic, such as removing event handlers.
        /// </summary>
        /// 
        protected virtual void OnDispose()
        {
            if (bDisposed)
                return;
            
            bDisposed = true;

#if !WINDOWS_UWP && !NET_STANDARD
            UnsubscribeDispatcher();
#endif

            CancelPendingIdleExecution();

#if !WINDOWS_UWP && !NET_STANDARD
            PropertyChanged.CheckEventHasNoSubscribers();
            
            if (observableTimer != null)
            {
                observableTimer.Dispose();
                observableTimer = null;
            }
#endif
        }

        /*
#if DEBUG
        /// <summary>
        /// Useful for ensuring that ViewModel objects are properly garbage collected.
        /// </summary>
        ~ViewModelBase()
        {
            try
            {
                string msg = string.Format("{0} ({1}) ({2}) Finalized", GetType().Name, Title, GetHashCode());
                System.Diagnostics.Debug.WriteLine(msg);
            }
            catch (Exception ex)
            {
                
            }
        }
#endif
        */
#endregion // IDisposable Members

#region Idle Execution

        static Timer staticIdleExecutionTimer;
        static bool staticExecuting;
        static List<ViewModelBase> listPendingExecution = new List<ViewModelBase>();
        static int staticLastDelay;
        static DateTime staticTimeDelay;

        Timer IdleExecutionTimer;
        bool bExecuting;
        bool bRepromote;
        int lastDelay;
        DateTime timeDelay;
#if DEBUG
        static int staticExecutionCounter;
#endif
        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual void PromoteIdleExecution(int nDelay, bool bStatic = false)
        {
            if (bDisposed)
                return;

            if (bStatic)
            {
                lock (listPendingExecution)
                {
                    if (!listPendingExecution.Contains(this))
                        listPendingExecution.Add(this);

                    if (staticIdleExecutionTimer != null)
                    {
                        if (nDelay >= staticLastDelay || (DateTime.UtcNow - staticTimeDelay).TotalMilliseconds < nDelay && nDelay != staticLastDelay)
                            return;
                        staticLastDelay = nDelay;
                        staticIdleExecutionTimer.Change(nDelay, Timeout.Infinite);
                    }
                    if (staticExecuting)
                    {
                        staticLastDelay = nDelay;
                        return;
                    }

#if WINDOWS_UWP || NET_STANDARD
                    IsBusy = true;
                    BackgroundWorkProcessed = false;
#endif
                    staticExecuting = true;
                    staticLastDelay = nDelay;
                    staticTimeDelay = DateTime.UtcNow;
                    staticIdleExecutionTimer = new Timer((o) =>
                    {
                        lock (listPendingExecution)
                        {
                            if (staticIdleExecutionTimer != null)
                            {
                                staticIdleExecutionTimer.Dispose();
                                staticIdleExecutionTimer = null;
                            }
                        }
#if DEBUG
                        var ret = Interlocked.Increment(ref staticExecutionCounter);
                        if (ret > 1)
                            System.Diagnostics.Debugger.Launch();
#endif
#if !WINDOWS_UWP
                        var oldPriority = Thread.CurrentThread.Priority;
#endif
                        try
                        {
#if !WINDOWS_UWP
                            try
                            {
                                Thread.CurrentThread.Priority = ThreadPriority.BelowNormal;
                            }
                            catch { }
#endif
                            var list = new List<ViewModelBase>();
                            lock (listPendingExecution)
                            {
                                if (listPendingExecution.Count > 0)
                                {
                                    list.AddRange(listPendingExecution);
                                    listPendingExecution.Clear();
                                }
                            }

                            foreach (var v in list)
                            {
//#if WINDOWS_UWP
//                                Task.Delay(1).Wait();
//#else
//                                Thread.Sleep(1);
//#endif
                                v.IdleExecution();
                            }
                        }
                        finally
                        {
#if !WINDOWS_UWP
                            try
                            {
                                Thread.CurrentThread.Priority = oldPriority;
                            }
                            catch { }
#endif
#if DEBUG
                            Interlocked.Decrement(ref staticExecutionCounter);
#endif
                            lock (listPendingExecution)
                            {
#if WINDOWS_UWP || NET_STANDARD
                                IsBusy = false;
                                BackgroundWorkProcessed = true;
#endif
                                staticExecuting = false;
                                if (listPendingExecution.Count > 0)
                                {
                                    var viewModel = listPendingExecution[0];
                                    viewModel.PromoteIdleExecution(staticLastDelay, true);
                                }
                            }
                        }
                    }, this, nDelay, Timeout.Infinite);
                }
            }
            else
            {
                lock (lockObjectThread)
                {
                    if (IdleExecutionTimer != null)
                    {
                        if (nDelay >= lastDelay || (DateTime.UtcNow - timeDelay).TotalMilliseconds < nDelay && nDelay != lastDelay)
                            return;
                        IdleExecutionTimer.Dispose();
                        IdleExecutionTimer = null;
                    }
                    if (bExecuting)
                    {
                        lastDelay = nDelay;
                        bRepromote = true;
                        return;
                    }

#if WINDOWS_UWP || NET_STANDARD
                    IsBusy = true;
                    BackgroundWorkProcessed = false;
#endif
                    lastDelay = nDelay;
                    timeDelay = DateTime.UtcNow;
                    IdleExecutionTimer = new Timer((o) =>
                        {
                            bExecuting = true;

                            lock (lockObjectThread)
                            {
                                if (IdleExecutionTimer != null)
                                {
                                    IdleExecutionTimer.Dispose();
                                    IdleExecutionTimer = null;
                                }
                            }

#if !WINDOWS_UWP
                            var oldPriority = Thread.CurrentThread.Priority;
#endif
                            try
                            {
#if !WINDOWS_UWP
                                try
                                {
                                    Thread.CurrentThread.Priority = ThreadPriority.BelowNormal;
                                }
                                catch { }

                                IdleExecution();
                                if (SleepingTime > 0)
                                {
                                    Thread.Sleep(SleepingTime);
#else
                                    Task.Delay(SleepingTime).Wait();
#endif
                                }
                            }
                            finally
                            {
#if !WINDOWS_UWP
                                try
                                {
                                    Thread.CurrentThread.Priority = oldPriority;
                                }
                                catch { }
#endif
                                lock (lockObjectThread)
                                {
#if WINDOWS_UWP || NET_STANDARD
                                    IsBusy = false;
                                    BackgroundWorkProcessed = true;
#endif
                                    bExecuting = false;
                                    if (bRepromote)
                                    {
                                        bRepromote = false;
                                        PromoteIdleExecution(lastDelay);
                                    }
                                }
                            }
                        }, this, nDelay, Timeout.Infinite);
                }
            }
        }

#if !WINDOWS_UWP && !NET_STANDARD
        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual void PromoteIdleExecution(DispatcherPriority priority)
        {
            if (bDisposed)
                return;

            lock (lockObjectThread)
            {
                if (priority != DispatcherPriority.Invalid)
                {
                    if (m_dispatcher == null)
                        m_dispatcher = Dispatcher.FromThread(Thread.CurrentThread);
                    if (m_dispatcher != null)
                    {
                        if (IdleExecutionPending == null)
                        {
                            IsBusy = true;
                            BackgroundWorkProcessed = false;
                            Action action = () => IdleExecution();
                            IdleExecutionPending = m_dispatcher.BeginInvoke(action, priority);
                            IdleExecutionPending.Completed += (sender, e) =>
                            {
                                try
                                {
                                    lock (lockObjectThread)
                                    {
                                        IdleExecutionPending = null;

                                        if (RepromoteBackgroundProcess)
                                        {
                                            RepromoteBackgroundProcess = false;
                                            PromoteIdleExecution(priority);
                                        }
                                    }
                                }
                                finally
                                {
                                    BackgroundWorkProcessed = true;
                                    IsBusy = false;
                                }
                            };
                        }
                        else
                            RepromoteBackgroundProcess = true;
                    }
                    else
                        RepromoteBackgroundProcess = true;
                }
                else 
                {
                    if (IsBusy)
                    {
                        RepromoteBackgroundProcess = true;
                        return;
                    }
                    BackgroundWorkProcessed = false;
                    IsBusy = true;

                    /*
                    ThreadPool.QueueUserWorkItem((o) =>
                        {
                            var oldPriority = Thread.CurrentThread.Priority;
                            try
                            {
                                Thread.CurrentThread.Priority = ThreadPriority.Lowest;
                                Thread.Sleep(SleepingTime);
                                IdleExecution();
                            }
                            finally
                            {
                                lock (lockObjectThread)
                                {
                                    if (RepromoteBackgroundProcess)
                                    {
                                        RepromoteBackgroundProcess = false;
                                        PromoteIdleExecution(DispatcherPriority.Invalid);
                                    }
                                    IsBusy = false;
                                }
                                BackgroundWorkProcessed = true;
                                Thread.CurrentThread.Priority = oldPriority;
                            }
                        });
                    */
                    
                    if (backgroundWorker == null)
                    {
                        IsBusy = true;
                        BackgroundWorkProcessed = false;
                        backgroundWorker = new BackgroundWorker();
                        backgroundWorker.DoWork +=
                            (o, e) =>
                            {
                                var oldPriority = Thread.CurrentThread.Priority;
                                try
                                {
                                    try
                                    {
                                        Thread.CurrentThread.Priority = ThreadPriority.BelowNormal;
                                    }
                                    catch { }

                                    Thread.Sleep(SleepingTime);
                                    IdleExecution();
                                }
                                finally
                                {
                                    try
                                    {
                                        Thread.CurrentThread.Priority = oldPriority;
                                    }
                                    catch { }
                                }
                            };
                        backgroundWorker.RunWorkerCompleted +=
                            (o, e) =>
                            {
                                try
                                {
                                    lock (lockObjectThread)
                                    {
                                        backgroundWorker.Dispose();
                                        backgroundWorker = null;

                                        if (RepromoteBackgroundProcess)
                                        {
                                            RepromoteBackgroundProcess = false;
                                            PromoteIdleExecution(DispatcherPriority.Invalid);
                                        }
                                    }
                                }
                                finally
                                {
                                    BackgroundWorkProcessed = true;
                                    IsBusy = false;
                                }
                            };
                        backgroundWorker.WorkerSupportsCancellation = true;
                        backgroundWorker.RunWorkerAsync();
                    }
                    else
                        RepromoteBackgroundProcess = true;
                }
            }
        }
#endif

        protected void CancelPendingIdleExecution()
        {
            lock (lockObjectThread)
            {
                RepromoteBackgroundProcess = false;
#if !WINDOWS_UWP && !NET_STANDARD
                if (backgroundWorker != null && backgroundWorker.WorkerSupportsCancellation)
                    backgroundWorker.CancelAsync();
#endif
                if (IdleExecutionTimer != null)
                {
                    IdleExecutionTimer.Dispose();
                    IdleExecutionTimer = null;
                }
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool IsIdleExecutionBusy()
        {
#if !WINDOWS_UWP && !NET_STANDARD
            lock (lockObjectThread)
            {
                if (backgroundWorker != null)
                    return backgroundWorker.IsBusy;
                else if (IdleExecutionPending != null)
                    return IdleExecutionPending.Status == DispatcherOperationStatus.Executing ||
                           IdleExecutionPending.Status == DispatcherOperationStatus.Pending;
            }
#endif
            return false;
        }

#if !WINDOWS_UWP && !NET_STANDARD
        protected void ReportProgress(int nProgress)
        {
            lock (lockObjectThread)
            {
                if (backgroundWorker != null && backgroundWorker.WorkerReportsProgress)
                    backgroundWorker.ReportProgress(nProgress);
            }
        }
#endif

        protected bool IsIdleExecutionCancelled()
        {
#if !WINDOWS_UWP && !NET_STANDARD
            lock (lockObjectThread)
            {
                if (backgroundWorker != null && backgroundWorker.WorkerSupportsCancellation)
                    return backgroundWorker.CancellationPending;
                else if (IdleExecutionPending != null)
                    return IdleExecutionPending.Status == DispatcherOperationStatus.Aborted;
            }
#endif
            return false;
        }

        protected virtual void IdleExecution()
        {
            // throw new NotImplementedException();
        }
#endregion

#region Common Properties

        int sleepingTime = 10;
#if !WINDOWS_UWP
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
#endif
        public int SleepingTime
        {
            get
            {
                return sleepingTime;
            }
            set
            {
                if (sleepingTime == value)
                    return;
                sleepingTime = value;
                OnPropertyChanged("SleepingTime");
            }
        }

        /*
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public Mediator Mediator 
        {
            get
            {
                lock (lockObject)
                {
                    if (_mediator == null)
                    {
                        _mediator = new Mediator();
                        Mediator.Register(this);
                    }
                }
                return _mediator;
            }
        }
        */

        String _lastMessage;
#if !WINDOWS_UWP
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
#endif
        public String LastMessage
        {
            get
            {
                return _lastMessage;
            }
            set
            {
                if (value == _lastMessage)
                    return;

                _lastMessage = value;
                OnPropertyChanged("LastMessage");
            }
        }

#endregion
    }
}
