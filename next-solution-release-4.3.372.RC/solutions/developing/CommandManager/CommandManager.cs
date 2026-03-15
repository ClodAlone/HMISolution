using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using ViewModelLib;
using UFInterfaces.Constants;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Reflection;
using System.IO;
using System.Diagnostics;
#if !WINDOWS_UWP
#if !NET_STANDARD
using System.Windows.Threading;
using System.Windows.Controls;
using System.Windows.Media;
using UFInterfaces.PropertyControl;
using System.Windows.Media.Media3D;
using CommandManager.ComponentService;
using UIMsgBoxAlertService.ComponentService;
#endif
#else
using Windows.UI.Xaml;
#endif
using UFInterfaces;
using DocumentManager.ComponentService;
using System.Windows;
using OPCUAViewModel;
using log4net;
using Utilities.Logger;
using Utilities;
using Utilities.Converters;
using System.Xml.Linq;
using System.Dynamic;
using ExpressionManager;

namespace CommandManager
{
    public class RemoteExecute
    {
        public Uri uri { get; set; }
        public ExecutionMode executionMode { get; set; }
        public Exception ex { get; set; }
        public IDocument Parent { get; set; }
    }

    [DataContract(Name = "CommandManager", Namespace = Namespaces.UriProgea)]
    public abstract class CommandManager
#if !WINDOWS_UWP && !NET_STANDARD
         : INotifyPropertyChanged, INotifyPropertyVisibilityChanged, IDataErrorInfo
#endif
    {
        protected static readonly ILog logCommands = Logger.GetDestinationLog(LoggerDestination.CommandManager);
#if !NET_STANDARD
        static bool bUICommandsNotAllExecuted;

#if !WINDOWS_UWP
        public static readonly DependencyProperty IsOnManipulationProperty = DependencyProperty.RegisterAttached(
          "IsOnManipulation",
          typeof(Boolean),
          typeof(CommandManager),
          new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.None)
        );
        public static void SetIsOnManipulation(UIElement element, Boolean value)
        {
            element.SetValue(IsOnManipulationProperty, value);
        }
        public static Boolean GetIsOnManipulation(UIElement element)
        {
            return (Boolean)element.GetValue(IsOnManipulationProperty);
        }
        static bool bPressAndHoldEnabled = RegistryKeysHelper.PressAndHoldEnabled();

        static Dictionary<UIElement, CommandManager> lastTouchCommand = new Dictionary<UIElement, CommandManager>();
#endif

        public static readonly DependencyProperty IsAccessDeniedProperty = DependencyProperty.RegisterAttached("IsAccessDenied", typeof(Boolean), typeof(CommandManager),
#if !WINDOWS_UWP
            new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.None));
#else
            new PropertyMetadata(false));
#endif
        public static void SetIsAccessDenied(UIElement element, Boolean value)
        {
            if (element != null)
                element.SetValue(IsAccessDeniedProperty, value);
        }
        public static Boolean GetIsAccessDenied(UIElement element)
        {
            if (element != null)
                return (Boolean)element.GetValue(IsAccessDeniedProperty);
            return false;
        }
#endif
        #region Constant
        const string rootNode = "CommandOrder";
#endregion

#region Persistance

        [DataMember]
        OPCUAEntityReference opcuaEntityReference;
#if !WINDOWS_UWP
        [DisplayName("Tag")]
#endif
#if !NETSTANDARD
        [DisplayNameExtension]
#endif
        public OPCUAEntityReference OpcuaEntityReference
        {
            get { return opcuaEntityReference; }
            set
            {
                if (value == opcuaEntityReference)
                    return;
                opcuaEntityReference = value;
#if !WINDOWS_UWP && !NET_STANDARD
                OnPropertyChanged("OpcuaEntityReference");
                OnPropertyChanged("CommandSummary");
                OnPropertyVisiblityChanged("OpcuaEntityReference");
#endif
            }
        }

        [DataMember]
        int delayCommandSecs;
        public int DelayCommandSecs
        {
            get
            {
                var c = this as ValueCommand;
                if (c != null && (c.Type == ValueCommandType.Impulsive || c.Type == ValueCommandType.ImpulsiveLatch)) //Retroactively not supported
                    delayCommandSecs = 0;
                return delayCommandSecs;
            }
            set
            {
                if (value == delayCommandSecs)
                    return;
                delayCommandSecs = value;
#if !WINDOWS_UWP && !NET_STANDARD
                OnPropertyChanged("DelayCommandSecs");
                OnPropertyChanged("CommandSummary");
#endif
            }
        }

        [DataMember(EmitDefaultValue = false)]
        [Browsable(false)]
        public int? SVGReferenceId { get; set; }
        [DataMember(EmitDefaultValue = false)]
        [Browsable(false)]
        public String SVGItemId { get; set; }

        [DataMember]
        String expression;
        public String Expression
        {
            get { return expression; }
            set
            {
                if (value == expression)
                    return;
                expression = value;
#if !WINDOWS_UWP && !NET_STANDARD
                OnPropertyChanged("Expression");
#endif
            }
        }
#endregion

#region Command Type collection

        static ObservableCollection<Type> commandTypes = new ObservableCollection<Type>();
        static Dictionary<String, Type> commandTypesMap = new Dictionary<string, Type>();
        static ObservableCollection<String> commandTypeNames = new ObservableCollection<String>();

        public static CommandManager CreateFrom(String type)
        {
            if (!commandTypesMap.ContainsKey(type))
                return null;
            return Activator.CreateInstance(commandTypesMap[type]) as CommandManager;
        }

        public static Type GetCommandType(String type)
        {
            if (!commandTypesMap.ContainsKey(type))
                return null;
            return commandTypesMap[type];
        }

        public static String GetCommandTypeName(String aname)
        {
            if (commandTypesMap.ContainsKey(aname))
                return commandTypesMap[aname].Name;
            else
                return aname;
        }

        public static ObservableCollection<String> LoadCommandTypes()
        {
            if (commandTypeNames.Count > 0)
                return commandTypeNames;

            ObservableCollection<String> ret = new ObservableCollection<string>();
#if WINDOWS_UWP
            var list = LoadCommandTypes("CommandManager");
            foreach (var i in list)
                ret.Add(i);
#else
            var curAssembly = Assembly.GetExecutingAssembly();

            var basefolder = String.Format("{0}\\{1}", Path.GetDirectoryName(curAssembly.Location), "Commanders");
            if (Directory.Exists(basefolder))
            {
                String[] listFiles = Directory.GetFiles(basefolder, "*.dll");

                Array.ForEach(listFiles, file =>
                {
                    var list = LoadCommandTypes(file);
                    foreach (var i in list)
                        ret.Add(i);
                });
            }

            var v = LoadCommandTypes(Assembly.GetAssembly(typeof(CommandManager)));
            foreach (var i in v)
                ret.Add(i);
#endif
            return ret;
        }

        public static ObservableCollection<String> LoadCommandTypes(Assembly assembly)
        {
            List<CommandManager> list = new List<CommandManager>();
            var orderlist = CommandManager.LoadCommandOrder();

            var assemblyname = assembly.GetName().Name;
            Type anType;
            while (orderlist.Count > 0)
            {
                var at = orderlist[0];
                orderlist.Remove(at);
                try
                {
                    anType = assembly.GetType(string.Format("{0}.{1}", assemblyname, at), false);
                }
                catch
                {
                    continue;
                }
                if (anType != null)
                {
                    // Must not already exist
                    if (commandTypes.Contains(anType)) { continue; }

                    // Must not be abstract.
                    if ((typeof(CommandManager).IsAssignableFrom(anType)) &&
#if !WINDOWS_UWP
                (!anType.IsAbstract)
#else
                anType != typeof(CommandManager)
#endif
                )
                    {
                        commandTypes.Add(anType);
                        CommandManager am = Activator.CreateInstance(anType) as CommandManager;
                        commandTypeNames.Add(am.Name);
                        commandTypesMap.Add(am.Name, anType);
                    }
                }
            }

            foreach (Type type in assembly.GetTypes())
            {
                // Must not already exist
                if (commandTypes.Contains(type)) { continue; }

                // Must not be abstract.
                if ((typeof(CommandManager).IsAssignableFrom(type)) &&
#if !WINDOWS_UWP
                    (!type.IsAbstract)
#else
                    type != typeof(CommandManager)
#endif
                    )
                {
                    commandTypes.Add(type);
                    CommandManager am = Activator.CreateInstance(type) as CommandManager;
                    commandTypeNames.Add(am.Name);
                    commandTypesMap.Add(am.Name, type);
                }
            }

            return commandTypeNames;
        }

        static List<string> LoadCommandOrder()
        {
            List<string> listOrder = new List<string>();
            var me = Assembly.GetEntryAssembly();
            if (me == null)
                return listOrder;

            string filepath = string.Format("{0}{1}{2}", Path.GetDirectoryName(me.Location), Path.DirectorySeparatorChar, Properties.Settings.Default.CommandOrderFileName);
            if (File.Exists(filepath))
            {
                try
                {
                    XDocument doc = new XDocument();
                    doc = XDocument.Parse(File.ReadAllText(filepath));

                    var animationlist = doc.Descendants(rootNode).ToList();
                    if (animationlist.Count > 0)
                    {
                        var element = animationlist[0];

                        foreach (var child in element.Descendants())
                        {
                            if (child.Name.Namespace == "")
                                listOrder.Add(child.Value);
                        }
                    }
                }
                catch (Exception e)
                {

                }
            }
            return listOrder;
        }

        public static ObservableCollection<String> LoadCommandTypes(string assemblyPath)
        {
            // Load the assembly
#if WINDOWS_UWP
            var asName = new AssemblyName();
            asName.Name = assemblyPath;
            Assembly assembly = Assembly.Load(asName);
#else
            Assembly assembly = Assembly.LoadFrom(assemblyPath);
#endif

            // Load transitions from the assembly
            return LoadCommandTypes(assembly);
        }

#endregion

#region Abstracts
#if !NET_STANDARD
        protected Dispatcher currentDispatcher;
#endif
        protected bool bInitialized;
        /// <summary>
        /// Initialize the command to be executed.
        /// </summary>
        /// <param name="entity">
        /// The UFInterfaces.IEntityReference that contains the command to initialize.
        /// </param>
        /// <param name="parent">
        /// The DocumentManager.ComponentService.IDocument that contains the command to initialize.
        /// </param>
        /// <param name="sessionname">
        /// The session name to use for connecting the tags.
        /// </param>
        /// <param name="expressionParserError">
        /// The method to call in case of error on parsing the expression.
        /// </param>
        /// <param name="expressionExecutionError">
        /// The method to call in case of error on calculating the expression.
        /// </param>
        /// <returns>
        /// true if the command is successfully initialized; otherwise, false.
        /// </returns>
        public bool Init(IEntityReference entity, IDocument parent, String sessionname, EventHandler expressionParserError, EventHandler expressionExecutionError)
        {
            OnExpressionParserError = expressionParserError;
            OnExpressionExecutionError = expressionParserError;
            return Init(entity, parent, sessionname);
        }

        /// <summary>
        /// Initialize the command to be executed.
        /// </summary>
        /// <param name="entity">
        /// The UFInterfaces.IEntityReference that contains the command to initialize.
        /// </param>
        /// <param name="parent">
        /// The DocumentManager.ComponentService.IDocument that contains the command to initialize.
        /// </param>
        /// <param name="sessionname">
        /// The session name to use for connecting the tags.
        /// </param>
        /// <returns></returns>
        public virtual bool Init(IEntityReference entity, IDocument parent, String sessionname)
        {
            Entity = entity;
            Parent = parent;
            SessionName = sessionname;
#if !NET_STANDARD
            currentDispatcher = Dispatcher.FromThread(System.Threading.Thread.CurrentThread);
#endif
            bInitialized = true;

#if !NET_STANDARD
#if !WINDOWS_UWP
            if (Control != null && Control3D == null)
            {
                lock (lastTouchCommand)
                {
                    lastTouchCommand[Control] = this;
                }
                Control.TouchDown += Control_TouchDown;
                Control.TouchUp += Control_TouchUp;
                Control.TouchLeave += Control_TouchLeave;

                if (DelayCommandSecs > 0)
                {
                    Control.PreviewMouseDown += Control_MouseDown;
                    Control.PreviewMouseUp += Control_MouseUp;
                    Control.MouseLeave += Control_MouseLeave;
                }
            }
#else
            if (Control != null && DelayCommandSecs > 0)
            {
                Control.PointerPressed += Control_MouseDown;
                Control.PointerReleased += Control_MouseUp;
                Control.PointerCanceled += Control_MouseLeave;
            }
#endif
#endif
            return true;
        }

        public virtual void RefreshAllEntityReferences()
        {
            if (OpcuaEntityReference != null)
            {
                var xml = OpcuaEntityReference.ToXml();
                OpcuaEntityReference = xml.FromXml<OPCUAEntityReference>();
            }
        }

#if !NET_STANDARD
        DispatcherTimer delayCommandTimer;
        protected bool executeDelayCommand;
        static Dictionary<String, bool> mapExecutedDelayCommand = new Dictionary<String, bool>();

        String GetControlId()
        {
            var fe = Control as FrameworkElement;
            if (fe != null)
            {
                if (!String.IsNullOrEmpty(fe.Name))
                    return fe.Name;
#if !WINDOWS_UWP
                var name = fe.Uid as String;
                if (!String.IsNullOrEmpty(name))
                    return name;
#endif
            }

            return "GenericControl";
        }

        void StartDelayCommand()
        {
            if (DelayCommandSecs <= 0)
                return;

            if (delayCommandTimer == null)
            {
                delayCommandTimer = new DispatcherTimer();
                delayCommandTimer.Interval = TimeSpan.FromSeconds(DelayCommandSecs);
                delayCommandTimer.Tick += (o, e) =>
                {
                    delayCommandTimer.Stop();
                    bool mouseIsDown = System.Windows.Input.Mouse.LeftButton == System.Windows.Input.MouseButtonState.Pressed || isControlPressed;
                    if (mouseIsDown)
                    {
                        // Control.IsEnabled = false;
                        executeDelayCommand = true;
                        lock (mapExecutedDelayCommand)
                        {
                            mapExecutedDelayCommand[GetControlId()] = true;
                        }
                        try
                        {
                            Execute();
                        }
                        finally
                        {
                            executeDelayCommand = false;
                        }
                    }
                };
            }
            delayCommandTimer.Start();
        }

        protected bool CanExecuteDelayCommand()
        {
            if (DelayCommandSecs <= 0)
                return true;

            return executeDelayCommand;
        }

        void StopDelayCommand()
        {
            if (delayCommandTimer != null)
                delayCommandTimer.Stop();
        }

        private void Control_MouseLeave(object sender,
#if !WINDOWS_UWP
            System.Windows.Input.MouseEventArgs e)
#else
            Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
#endif
        {
            InvalidateExecutedDelayCommand();
            StopDelayCommand();
            // Control.IsEnabled = true;
        }

        private void Control_MouseUp(object sender,
#if !WINDOWS_UWP
            System.Windows.Input.MouseButtonEventArgs e)
#else
            Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
#endif
        {
            InvalidateExecutedDelayCommand();
            StopDelayCommand();
            // Control.IsEnabled = true;
        }

        private void Control_MouseDown(object sender,
#if !WINDOWS_UWP
            System.Windows.Input.MouseButtonEventArgs e)
#else
            Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
#endif
        {
#if !WINDOWS_UWP
            if (GetIsOnManipulation(Control))
                return;
#endif
            StartDelayCommand();
        }

#if !WINDOWS_UWP
        DateTime lastTouchExecuted;
        bool isControlPressed;
        protected bool isTouchDownHandled;
        void Control_TouchDown(object sender, System.Windows.Input.TouchEventArgs e)
        {
            lock (lastTouchCommand)
            {
                e.Handled = lastTouchCommand.ContainsKey(Control) && lastTouchCommand[Control] == this;
            }

            if (isTouchDownHandled)
            {
                isTouchDownHandled = false;
                return;
            }

            if (Environment.OSVersion.Version < new Version(6, 2))
                isControlPressed = true;
            if (GetIsOnManipulation(Control))
                return;

            if (DelayCommandSecs > 0)
                StartDelayCommand();
            else if (Environment.OSVersion.Version < new Version(6, 2) || !bPressAndHoldEnabled)
            {
                lastTouchExecuted = DateTime.MinValue;
                Execute();
            }
            else if (ExecuteOnTouchDown())
            {
                Execute();
                lastTouchExecuted = DateTime.UtcNow;
            }
        }

        protected bool isTouchUpHandled;
        void Control_TouchUp(object sender, System.Windows.Input.TouchEventArgs e)
        {
            if (isTouchUpHandled)
            {
                isTouchUpHandled = false;
                return;
            }

            if (Environment.OSVersion.Version < new Version(6, 2))
                isControlPressed = false;
            InvalidateExecutedDelayCommand();
            StopDelayCommand();

            if (ExecuteOnTouchDown())
                InvalidateTouchExecuted();
            //else
            //{
            //    Execute();
            //    e.Handled = true;
            //}
            // Control.IsEnabled = true;
        }

        void Control_TouchLeave(object sender, System.Windows.Input.TouchEventArgs e)
        {
            InvalidateExecutedDelayCommand();
            StopDelayCommand();

            if (ExecuteOnTouchDown())
                InvalidateTouchExecuted();
            // Control.IsEnabled = true;
        }
#endif
        DispatcherTimer refreshTimerExecutedDelayCommand;
        void InvalidateExecutedDelayCommand()
        {
            var name = GetControlId();
            lock (mapExecutedDelayCommand)
            {
                if (mapExecutedDelayCommand.ContainsKey(name) && mapExecutedDelayCommand[name] == true)
                {
                    if (refreshTimerExecutedDelayCommand == null)
                    {
                        refreshTimerExecutedDelayCommand = new DispatcherTimer();
                        refreshTimerExecutedDelayCommand.Interval = TimeSpan.FromMilliseconds(20);
                        refreshTimerExecutedDelayCommand.Tick += (o, e) =>
                        {
                            refreshTimerExecutedDelayCommand.Stop();
                            lock (mapExecutedDelayCommand)
                            {
                                mapExecutedDelayCommand[name] = false;
                            }
#if !WINDOWS_UWP
                        System.Windows.Input.CommandManager.InvalidateRequerySuggested();
#endif
                    };
                    }
                    refreshTimerExecutedDelayCommand.Start();
                }
            }
        }

#if !WINDOWS_UWP
        DispatcherTimer refreshTimer;
        void InvalidateTouchExecuted()
        {
            if (lastTouchExecuted != null && lastTouchExecuted != DateTime.MinValue)
            {
                if (refreshTimer == null)
                {
                    refreshTimer = new DispatcherTimer();
                    refreshTimer.Interval = TimeSpan.FromMilliseconds(20);
                    refreshTimer.Tick += (o, e) =>
                    {
                        refreshTimer.Stop();
                        lastTouchExecuted = DateTime.MinValue;
                        System.Windows.Input.CommandManager.InvalidateRequerySuggested();
                    };
                }
                refreshTimer.Start();
            }
        }
#endif
#endif
        public virtual void Terminate()
        {
        	bInitialized = false;
#if !NET_STANDARD
            if (Control != null
#if !WINDOWS_UWP
                && Control3D == null
#endif
                )
            {
#if !WINDOWS_UWP
                Control.TouchDown -= Control_TouchDown;
                Control.TouchUp -= Control_TouchUp;
                Control.TouchLeave -= Control_TouchLeave;
                lock (lastTouchCommand)
                {
                    lastTouchCommand.Remove(Control);
                }

                if (DelayCommandSecs > 0)
                {
                    Control.PreviewMouseDown -= Control_MouseDown;
                    Control.PreviewMouseUp -= Control_MouseUp;
                    Control.MouseLeave -= Control_MouseLeave;
                }
#else
                if (Control != null && DelayCommandSecs > 0)
                {
                    Control.PointerPressed -= Control_MouseDown;
                    Control.PointerReleased -= Control_MouseUp;
                    Control.PointerCanceled -= Control_MouseLeave;
                }
#endif
            }

#if !WINDOWS_UWP
            if (refreshTimer != null)
                refreshTimer.Stop();
#endif
            if (refreshTimerExecutedDelayCommand != null)
                refreshTimerExecutedDelayCommand.Stop();
            if (delayCommandTimer != null)
                delayCommandTimer.Stop();
#endif
        }

        public virtual bool CanExecute()
        {
#if !NET_STANDARD
            var name = GetControlId();
            lock (mapExecutedDelayCommand)
            {
                if (mapExecutedDelayCommand.ContainsKey(name) && mapExecutedDelayCommand[name] == true)
                {
                    return DelayCommandSecs > 0;
                }
            }
#endif
#if !WINDOWS_UWP && !NET_STANDARD
            return lastTouchExecuted == null || lastTouchExecuted == DateTime.MinValue;
#else
            return true;
#endif
        }

#if !NET_STANDARD
        protected bool IsAccessDenied()
        {
            return GetIsAccessDenied(Control);
        }
#endif

        protected EventHandler OnExpressionParserError;

        protected EventHandler OnExpressionExecutionError;

#if !NET_STANDARD
#if !WINDOWS_UWP
        public virtual bool EditDataCommandSetting()
        {
            return true;
        }

        public virtual bool IsDataCommandSettingAvailable()
        {
            return false;
        }
#endif
        public virtual bool ExecuteOnTouchDown()
        {
            return false;
        }
#endif

        public virtual void BlindExecute()
        {
            Execute();
        }

        public virtual bool IsUICommand()
        {
            return false;
        }

#if !NET_STANDARD
        bool bPendingExecution;
        protected void ExecuteOnUserInterface(Action action = null)
        {
            if (currentDispatcher == null)
                return;

            if (bPendingExecution)
                throw new Exception(Properties.Resources.PendingExecutionException);
            bPendingExecution = true;

            currentDispatcher.BeginInvokeIfRequired(() =>
            {
                try
                {
                    if (!bInitialized)
                        return;

                    if (action != null)
                        action();
                    else
                        Execute();
                }
                catch (Exception ex)
                {
                    logCommands.ErrorFormat(Properties.Resources.ErrorExecutingCommand, Name, ex.Message);
                    if (!bUICommandsNotAllExecuted)
                    {
                        bUICommandsNotAllExecuted = true;
                        currentDispatcher.BeginInvokeAsynchronouslyInBackground(() =>
                        {
                            try
                            {
                                if (Parent != null)
                                {
                                    var ui = Parent.GetService(typeof(IUIMsgBoxAlertService)) as IUIMsgBoxAlertService;
                                    if (ui != null)
                                        ui.ShowError(Properties.Resources.CommandsNotAllExecuted);
                                }
                                else
                                    System.Windows.MessageBox.Show(Properties.Resources.CommandsNotAllExecuted, Parent?.Title);
                            }
                            finally
                            {
                                bUICommandsNotAllExecuted = false;
                            }
                        });
                    }
                }
                finally
                {
                    bPendingExecution = false;
                }
            });
        }
#endif

#if !WINDOWS_UWP
        public abstract RemoteExecute RemoteExecute();
#endif
        public abstract void Execute();

        public abstract String Name { get; }

#if !WINDOWS_UWP
        [Browsable(false)]
#endif
        public String SessionName { get; protected set; }

#if !NET_STANDARD
#if !WINDOWS_UWP
        [Browsable(false)]
#endif
        public UIElement Control { get; set; }
#if !WINDOWS_UWP
        [Browsable(false)]
        public Model3D Control3D { get; set; }
#endif
#endif
#if !WINDOWS_UWP
        [Browsable(false)]
#endif
        public String sExpression;
#if !WINDOWS_UWP
        [Browsable(false)]
#endif
        public String sReverseExpression;
#if !WINDOWS_UWP
        [Browsable(false)]
#endif
        public IDictionary<String, Object> mapDynamics;
        public Dictionary<String, String> mapCurrentParameteItems;

#if !WINDOWS_UWP && !NET_STANDARD
        [Browsable(false)]
        public virtual UserControl Editor
        {
            get
            {
                return new UserControls.CommonCommandPropertyEditor();
            }
        }
#endif

#if !WINDOWS_UWP
        [Browsable(false)]
#endif
        public virtual List<OPCUAEntityReference> ListTags
        {
            get
            {
                var ret = new List<OPCUAEntityReference>();
                if (OpcuaEntityReference != null)
                    ret.Add(OpcuaEntityReference);
                return ret;
            }
        }

#if !WINDOWS_UWP
        [Browsable(false)]
#endif
        public virtual void UpdateTags(OPCUAEntityReference source, OPCUAEntityReference dest)
        {
            if (OpcuaEntityReference != null && OpcuaEntityReference.HumanReadableNoProject == source?.HumanReadableNoProject && dest != null)
                OpcuaEntityReference = dest;
        }

#if !WINDOWS_UWP
        [Browsable(false)]
#endif
        public virtual Dictionary<OPCUAEntityReference, String> TagsMap
        {
            get
            {
                var ret = new Dictionary<OPCUAEntityReference, String>();
                if (OpcuaEntityReference != null)
                    ret.Add(OpcuaEntityReference, Name);
                return ret;
            }
        }

#if !WINDOWS_UWP && !NET_STANDARD
        [Browsable(false)]
        public virtual ImageSource Image
        {
            get
            {
                return null;
            }
        }
#endif
#if !WINDOWS_UWP
        [Browsable(false)]
#endif
        protected IEntityReference Entity { get; set; }
#if !WINDOWS_UWP
        [Browsable(false)]
#endif
        protected IDocument Parent { get; set; }

#if !WINDOWS_UWP && !NET_STANDARD
        [EditorBrowsable(EditorBrowsableState.Never)]
        public CommandManager CommandSettings
        {
            get { return this; }
        }
#endif
#if !WINDOWS_UWP
        [Browsable(false)]
        public virtual String CommandSummary
        {
            get
            {
                return String.Empty;
            }
        }
#endif
#if !WINDOWS_UWP && !NET_STANDARD
        public void ForceRefreshCommandSummary()
        {
            OnPropertyChanged("CommandSummary");
        }
#endif
        #endregion

        #region INotifyPropertyChanged Members

        /// <summary>
        /// Raised when a property on this object has a new value.
        /// </summary>
#if !WINDOWS_UWP && !NET_STANDARD
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Raises this object's PropertyChanged event.
        /// </summary>
        /// <param name="propertyName">The property that has a new value.</param>
        protected virtual void OnPropertyChanged(string propertyName)
        {
            // VerifyPropertyName(propertyName);

            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                DispatcherObject dispatcherObject = handler.Target as DispatcherObject;

                var e = new PropertyChangedEventArgs(propertyName);
                // If the subscriber is a DispatcherObject and different thread
                if (dispatcherObject != null && dispatcherObject.CheckAccess() == false)
                {
                    // Invoke handler in the target dispatcher's thread
                    dispatcherObject.Dispatcher.BeginInvoke(DispatcherPriority.DataBind, handler, this, e);
                }
                else // Execute handler as is
                    handler(this, e);
            }
        }
#endif
        #endregion // INotifyPropertyChanged Members

        #region Validations
#if !WINDOWS_UWP && !NET_STANDARD
        [EditorBrowsable(EditorBrowsableState.Never)]
        string IDataErrorInfo.Error
        {
            get
            {
                return null;
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        string IDataErrorInfo.this[string propertyName]
        {
            get
            {
                return PerformValidation(propertyName);
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        protected virtual String PerformValidation(String propertyName)
        {
            if (propertyName == "DelayCommandSecs")
            {
                if (DelayCommandSecs < 0)
                    return Properties.Resources.InvalidDelayCommandSecs;
            }
            if (propertyName == "Expression")
            {
                var expression = Expression;
                if (!String.IsNullOrEmpty(expression) && AliasHelper.GetAliasCount(expression) == 0)
                {
                    var error = ExpressionBucket.CheckExpression(expression, null);
                    if (!String.IsNullOrEmpty(error))
                        return error;
                }
            }
            return null;
        }
#endif
        #endregion

        #region INotifyPropertyVisibilityChanged Members

#if !WINDOWS_UWP && !NET_STANDARD
        /// <summary>
        /// Gets the visibility state for the property with the given name.
        /// </summary>
        /// <param name="propertyName">The property name that you want konw the current visibility state.</param>
        /// <returns></returns>
        public virtual bool this[string propertyName]
        {
            get
            {
                if (propertyName == "CommandSettings" || propertyName == "SVGReferenceId" || propertyName == "SVGItemId")
                {
                    return false;
                }

                return true;
            }
        }

        /// <summary>
        /// Raised when a property visibility state on this object has a new value.
        /// </summary>
        public event PropertyChangedEventHandler PropertyVisiblityChanged;

        /// <summary>
        /// Raises this object's PropertyVisiblityChanged event.
        /// </summary>
        /// <param name="propertyName">The property that has changed his value and has triggered the change of visibility.</param>
        protected virtual void OnPropertyVisiblityChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyVisiblityChanged;
            if (handler != null)
            {
                DispatcherObject dispatcherObject = handler.Target as DispatcherObject;

                var e = new PropertyChangedEventArgs(propertyName);
                // If the subscriber is a DispatcherObject and different thread
                if (dispatcherObject != null && dispatcherObject.CheckAccess() == false)
                {
                    // Invoke handler in the target dispatcher's thread
                    dispatcherObject.Dispatcher.BeginInvoke(DispatcherPriority.DataBind, handler, this, e);
                }
                else // Execute handler as is
                    handler(this, e);
            }
        }
#endif
        #endregion

        #region Debugging Aides

#if !WINDOWS_UWP && !NET_STANDARD
        /// <summary>
        /// Warns the developer if this object does not have
        /// a public property with the specified name. This 
        /// method does not exist in a Release build.
        /// </summary>
        [Conditional("DEBUG")]
        [DebuggerStepThrough]
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
#endif
        #endregion // Debugging Aides
    }
    public class LocalizedEnumConverter : ResourceEnumConverter
    {
        public LocalizedEnumConverter(Type type)
            : base(type, Properties.Resources.ResourceManager)
        {

        }
    }
}
