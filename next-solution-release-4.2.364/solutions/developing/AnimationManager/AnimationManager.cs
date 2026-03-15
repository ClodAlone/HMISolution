using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Windows;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Windows.Input;
#if !NET_STANDARD
using WPFUtilities.Converters;
#endif
#if !WINDOWS_UWP
#if !NET_STANDARD
using System.Windows.Controls;
using System.Windows.Threading;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Media3D;
using Converters;
using UFInterfaces.PropertyControl;
#endif
#else
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml;
using Windows.UI.Core;
#endif
using System.ComponentModel;
using System.Diagnostics;
using ViewModelLib;
using System.IO;
using Utilities;
using Opc.Ua;
using OPCUAViewModel;
using UFInterfaces;
using DocumentManager.ComponentService;
using Utilities.Converters;
using System.Xml.Linq;
using System.Dynamic;
using ExpressionManager;

namespace AnimationManager
{
    [TypeConverter(typeof(LocalizedEnumConverter))]
    public enum AnimationBehavior
    {
        Trigger,
        Proportional,
        Absolute
    }

    [DataContract(Name = "AnimationManager")]

#if !WINDOWS_UWP && !NET_STANDARD
    [KnownType(typeof(EasingFunctionBase))]
    [KnownType(typeof(BackEase))]
    [KnownType(typeof(BounceEase))]
    [KnownType(typeof(CircleEase))]
    [KnownType(typeof(CubicEase))]
    [KnownType(typeof(ElasticEase))]
    [KnownType(typeof(ExponentialEase))]
    [KnownType(typeof(PowerEase))]
    [KnownType(typeof(QuadraticEase))]
    [KnownType(typeof(QuarticEase))]
    [KnownType(typeof(QuinticEase))]
    [KnownType(typeof(SineEase))]
#endif
    public abstract class AnimationManager
#if !WINDOWS_UWP && !NET_STANDARD
 : INotifyPropertyChanged, INotifyPropertyVisibilityChanged, IDataErrorInfo
#endif
    {
#if !NET_STANDARD
        public static readonly DependencyProperty IsAccessDeniedProperty = DependencyProperty.RegisterAttached("IsAccessDenied", typeof(Boolean), typeof(CommandManager),
            new PropertyMetadata(false));
        public static void SetIsAccessDenied(UIElement element, Boolean value)
        {
            element.SetValue(IsAccessDeniedProperty, value);
        }
        public static Boolean GetIsAccessDenied(UIElement element)
        {
            return (Boolean)element.GetValue(IsAccessDeniedProperty);
        }
#endif

        #region Constant
        const string rootNode = "AnimationOrder";
        #endregion

        #region Properties

#if !WINDOWS_UWP
        [Browsable(false)]
#endif
        public String SessionName { get; protected set; }

#if !WINDOWS_UWP
        [Browsable(false)]
#endif
        protected IDocument Parent { get; set; }

#if !WINDOWS_UWP
        [Browsable(false)]
#endif
        public IDictionary<String, Object> mapDynamics;
        public Dictionary<String, String> mapCurrentParameteItems;

        Guid id;
#if !WINDOWS_UWP
        [Browsable(false)]
#endif
        [DataMember]
        public Guid ID
        {
            get
            {
                if (id == null || id == Guid.Empty)
                    id = Guid.NewGuid();
                return id;
            }
            set
            {
                if (value == id)
                    return;
                id = value;
            }
        }

        int? sVGReferenceId;
        [DataMember]
        public int? SVGReferenceId
        {
            get { return sVGReferenceId; }
            set
            {
                if (value == sVGReferenceId)
                    return;
                sVGReferenceId = value;
#if !WINDOWS_UWP && !NET_STANDARD
                OnPropertyChanged("SVGReferenceId");
#endif
            }
        }
        String sVGItemId;
        [DataMember]
        public String SVGItemId
        {
            get { return sVGItemId; }
            set
            {
                if (value == sVGItemId)
                    return;
                sVGItemId = value;
#if !WINDOWS_UWP && !NET_STANDARD
                OnPropertyChanged("SVGItemId");
#endif
            }
        }

        int animationTime = 1000;
        [DataMember]
        public int AnimationTime
        {
            get { return animationTime; }
            set
            {
                if (value == animationTime)
                    return;
                animationTime = value;
#if !WINDOWS_UWP && !NET_STANDARD
                OnPropertyChanged("AnimationTime");
                Reexecute();
#endif
            }
        }

        String sExpression;
        [DataMember]
        public String Expression
        {
            get { return sExpression; }
            set
            {
                if (value == sExpression)
                    return;
                sExpression = value;
#if !WINDOWS_UWP && !NET_STANDARD
                OnPropertyChanged("Expression");
                Reexecute();
#endif
            }
        }

        bool repeatable = false;
        [DataMember]
        public bool Repeatable
        {
            get { return repeatable; }
            set
            {
                if (value == repeatable)
                    return;
                repeatable = value;
#if !WINDOWS_UWP && !NET_STANDARD
                OnPropertyChanged("Repeatable");
                Reexecute();
#endif
            }
        }

        bool autoreverse = false;
        [DataMember]
        public bool Autoreverse
        {
            get { return autoreverse; }
            set
            {
                if (value == autoreverse)
                    return;
                autoreverse = value;
#if !WINDOWS_UWP && !NET_STANDARD
                OnPropertyChanged("Autoreverse");
                Reexecute();
#endif
            }
        }

        AnimationBehavior animationBehavior = AnimationBehavior.Absolute;
        [DataMember]
        public AnimationBehavior AnimationBehavior
        {
            get { return animationBehavior; }
            set
            {
                if (value == AnimationBehavior)
                    return;
                animationBehavior = value;
#if !WINDOWS_UWP && !NET_STANDARD
                OnPropertyChanged("AnimationBehavior");
                OnPropertyVisiblityChanged("AnimationBehavior");
                Reexecute();
#endif
            }
        }

#if !NET_STANDARD
        EasingFunctionBase animationEquation = new SineEase() { EasingMode = EasingMode.EaseOut };
        // [DataMember] we must not set a UI object serializable directly
        public EasingFunctionBase AnimationEquation
        {
            get { return animationEquation; }
            set
            {
                if (value == animationEquation)
                    return;
                animationEquation = value;
#if !WINDOWS_UWP
                OnPropertyChanged("AnimationEquation");
                Reexecute();
#endif
            }
        }
#endif

        [DataMember]
        OPCUAEntityReference opcuaEntityReference;
#if !WINDOWS_UWP
        [DisplayName("Tag")]
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
                OnPropertyChanged("AnimationSummary");
                Reexecute();
#endif
            }
        }

#if !WINDOWS_UWP
        Range range = new Range(0, 100);
#else
            Range range;
#endif
        [Browsable(false)]
        [DataMember]
        public Range Range
        {
            get
            {
                return range;
            }
            set
            {
                if (range == value)
                    return;
                range = value;
                RangeChanged();
            }
        }

        void RangeChanged()
        {
#if !NET_STANDARD
#if !WINDOWS_UWP
            OnPropertyChanged("Range");
            var control = Control;
            if (control != null)
                control.Dispatcher.BeginInvokeIfRequired(() =>
                {
                    Reexecute();
                });
#else
                if (control != null)
                    Utilities.RunOnUIThread.RunIfRequired(() => Reexecute());
#endif
#endif

        }
        public double TagMinValue
        {
            get { return range.Low; }
            set
            {
                if (range.Low == value)
                    return;
                range.Low = value;
                RangeChanged();
            }
        }

        public double TagMaxValue
        {
            get { return range.High; }
            set
            {
                if (range.High == value)
                    return;
                range.High = value;
                RangeChanged();
            }
        }
#if !NET_STANDARD
#if !WINDOWS_UWP
        [Browsable(false)]
#endif
        public bool Executed
        {
            get
            {
                return bExecuted;
            }
        }

#if !WINDOWS_UWP
        [Browsable(false)]
#endif
        public DataValue LastData
        {
            get
            {
                return lastdata;
            }
        }

        DataValue lastDataNotConverted;
#if !WINDOWS_UWP
        [Browsable(false)]
#endif
        public DataValue LastDataNotConverted
        {
            get
            {
                return lastDataNotConverted;
            }
            set
            {
                lastDataNotConverted = value;
            }
        }

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
        IEntityReference Entity { get; set; }
#endregion

#region Declarations
#if !NET_STANDARD
        PropertyObserver<OPCUAEntityReference> observer;
        PropertyObserver<MonitoredItemViewModel> datavalueobserver;
        ExpressionEntity expressionEntity;
        protected DataValue lastdata;
        double? lastValue;
        protected bool bExecuted;
        protected bool bDemoMode;
        bool bInit;
        protected double dTargetValue;
        protected bool bRangeLimit;
#endif
#endregion

#region Animation Type collection

        static ObservableCollection<Type> animationTypes = new ObservableCollection<Type>();
        static Dictionary<String, Type> animationTypesMap = new Dictionary<string, Type>();
        static ObservableCollection<String> animationTypeNames = new ObservableCollection<String>();

        public static AnimationManager CreateFrom(String type)
        {
            if (!animationTypesMap.ContainsKey(type))
                return null;
            return Activator.CreateInstance(animationTypesMap[type]) as AnimationManager;
        }

        public static Type GetAnimationType(String type)
        {
            if (!animationTypesMap.ContainsKey(type))
                return null;
            return animationTypesMap[type];
        }

        public static String GetAnimationTypeName(String aname)
        {
            if (animationTypesMap.ContainsKey(aname))
                return animationTypesMap[aname].Name;
            else
                return aname;
        }

        public static ObservableCollection<String> LoadAnimationTypes()
        {
            if (animationTypeNames.Count > 0)
                return animationTypeNames;

            ObservableCollection<String> ret = new ObservableCollection<string>();

#if WINDOWS_UWP
            var list = LoadAnimationTypes("AnimationManager");
            foreach (var i in list)
                ret.Add(i);
#else
            var curAssembly = Assembly.GetExecutingAssembly();

            var basefolder = String.Format("{0}\\{1}", Path.GetDirectoryName(curAssembly.Location), "Animators");
            if (Directory.Exists(basefolder))
            {
                String[] listFiles = Directory.GetFiles(basefolder, "*.dll");

                Array.ForEach(listFiles, file =>
                {
                    var list = LoadAnimationTypes(file);
                    foreach (var i in list)
                        ret.Add(i);
                });
            }
            var v = LoadAnimationTypes(Assembly.GetAssembly(typeof(AnimationManager)));
            foreach (var i in v)
                ret.Add(i);
#endif

            return ret;
        }

        public static ObservableCollection<String> LoadAnimationTypes(Assembly assembly)
        {
            var orderlist = AnimationManager.LoadAnimationOrder();
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
                    if (animationTypes.Contains(anType)) { continue; }

                    // Must not be abstract.
                    if ((typeof(AnimationManager).IsAssignableFrom(anType)) &&
#if !WINDOWS_UWP
                (!anType.IsAbstract)
#else
                anType != typeof(AnimationManager)
#endif
                )
                    {
                        animationTypes.Add(anType);
                        AnimationManager am = Activator.CreateInstance(anType) as AnimationManager;
                        animationTypeNames.Add(am.Name);
                        animationTypesMap.Add(am.Name, anType);
                    }
                }
            }

            foreach (Type type in assembly.GetTypes())
            {
                // Must not already exist
                if (animationTypes.Contains(type)) { continue; }

                // Must not be abstract.
                if ((typeof(AnimationManager).IsAssignableFrom(type)) &&
#if !WINDOWS_UWP
                    (!type.IsAbstract)
#else
                    type != typeof(AnimationManager)
#endif
                    )
                {
                    animationTypes.Add(type);
                    AnimationManager am = Activator.CreateInstance(type) as AnimationManager;
                    animationTypeNames.Add(am.Name);
                    animationTypesMap.Add(am.Name, type);
                }
            }

            return animationTypeNames;
        }

        static List<string> LoadAnimationOrder()
        {
            List<string> listOrder = new List<string>();
            var me = Assembly.GetEntryAssembly();
            if (me == null)
                return listOrder;

            string filepath = string.Format("{0}{1}{2}", Path.GetDirectoryName(me.Location), Path.DirectorySeparatorChar, Properties.Settings.Default.AnimationOrderFileName);
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
        public static ObservableCollection<String> LoadAnimationTypes(string assemblyPath)
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
            return LoadAnimationTypes(assembly);
        }
#endregion

#region Methods
#if !NET_STANDARD
        void ExpressionEntity_ParserError(object sender, EventArgs e)
        {
            if (OnExpressionParserError != null)
                OnExpressionParserError(sender, e);
        }

        void ExpressionEntity_ExecutionError(object sender, EventArgs e)
        {
            if (OnExpressionExecutionError != null)
                OnExpressionExecutionError(sender, e);
        }
#endif

        [OnDeserialized]
        private void PostInitialize(StreamingContext context)
        {
            if (range == null)
                range = new Range(0, 100);
        }

#if !NET_STANDARD
        public virtual void PreInit(IEntityReference entity)
        { }

#if !WINDOWS_UWP
        //DispatcherOperation d1;
        DispatcherOperation d2;
        DispatcherOperation d3;
        //DispatcherOperation d4;
#endif
        /// <summary>
        /// Initialize the animation to be executed.
        /// </summary>
        /// <param name="entity">
        /// The UFInterfaces.IEntityReference that contains the animation to initialize.
        /// </param>
        /// <param name="parent">
        /// The DocumentManager.ComponentService.IDocument that contains the animation to initialize.
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
        /// true if the animation is successfully initialized; otherwise, false.
        /// </returns>
        public void Init(IEntityReference entity, IDocument parent, String sessionname, EventHandler expressionParserError, EventHandler expressionExecutionError)
        {
            OnExpressionParserError = expressionParserError;
            OnExpressionExecutionError = expressionExecutionError;
            Init(entity, parent, sessionname);
        }

        /// <summary>
        /// Initialize the animation to be executed.
        /// </summary>
        /// <param name="entity">
        /// The UFInterfaces.IEntityReference that contains the animation to initialize.
        /// </param>
        /// <param name="parent">
        /// The DocumentManager.ComponentService.IDocument that contains the animation to initialize.
        /// </param>
        /// <param name="sessionname">
        /// The session name to use for connecting the tags.
        /// </param>
        /// <returns></returns>
        public virtual void Init(IEntityReference entity, IDocument parent, String sessionname)
        {
            bInit = true;
            bRangeLimit = false;
            Entity = entity;
            Parent = parent;
            SessionName = sessionname;

            if (OpcuaEntityReference != null)
            {
                observer = new PropertyObserver<OPCUAEntityReference>(OpcuaEntityReference)
                    .RegisterHandler(n => n.MonitoredItemViewModel, n =>
                    {
                        // observer.UnregisterHandler(p => p.MonitoredItemViewModel);
                        if (expressionEntity != null)
                        {
                            expressionEntity.ParserError -= ExpressionEntity_ParserError;
                            expressionEntity.ExecutionError -= ExpressionEntity_ExecutionError;
                            expressionEntity.Dispose();
                            expressionEntity = null;
                        }

                        var control = Control;
                        var value = String.Empty;
                        if (n.MonitoredItemViewModel != null &&
                            control != null)
                        {
                            /*
                            // pump read attributes outside UI thread
                            try
                            {
                                var name = n.NodeIdViewModel.BrowseName.Name;
                                var path = n.NodeIdViewModel.CompletePath;
                            }
                            catch { }

                            try
                            {
                                if (n.MonitoredItemViewModel.HasRange)
                                    range = n.MonitoredItemViewModel.Range;
                            }
                            catch { }

#if !WINDOWS_UWP
                            d4 = control.Dispatcher.BeginInvokeAsynchronously(() =>
#else
                            RunOnUIThread.RunIfRequired(() =>
#endif
                            {
                                try
                                {
                                    if (n.MonitoredItemViewModel.Value != null)
                                        value = n.MonitoredItemViewModel.Value;
                                    if (n.NodeIdViewModel != null)
                                    {
                                        mapDynamics[n.NodeIdViewModel.BrowseName.Name] = value;
                                        mapDynamics[n.NodeIdViewModel.CompletePath] = value;
                                    }
                                    else
                                        mapDynamics[n.RelativePath] = value;
                                    mapDynamics[OpcuaEntityReference.HumanReadableNoProject] = value;
                                }
                                catch (Exception ex)
                                {

                                }
                            });
                            */

                            var monitoredItemViewModel = n.MonitoredItemViewModel;
                            if (!String.IsNullOrEmpty(Expression))
                            {
                                expressionEntity = ExpressionBucket.GetInstance(parent).AddExpression(monitoredItemViewModel, Expression, null, mapCurrentParameteItems);
                                expressionEntity.ParserError += ExpressionEntity_ParserError;
                                expressionEntity.ExecutionError += ExpressionEntity_ExecutionError;
                                monitoredItemViewModel = expressionEntity.TempVariable;
                            }

                            if (datavalueobserver != null)
                                datavalueobserver.Dispose();
                            if (d2 != null && d2.Status != DispatcherOperationStatus.Aborted &&
                                d2.Status != DispatcherOperationStatus.Completed)
                                d2.Abort();
                            if (d3 != null && d3.Status != DispatcherOperationStatus.Aborted &&
                                d3.Status != DispatcherOperationStatus.Completed)
                                d3.Abort();
                            if (monitoredItemViewModel.DataValue != null)
                                LastDataNotConverted = monitoredItemViewModel.DataValue;
                            datavalueobserver = new PropertyObserver<MonitoredItemViewModel>(monitoredItemViewModel)
                            .RegisterHandler(d => d.DataValue, d =>
                            {
                                var controlTemp = Control;
                                /*
                                if (controlTemp != null && d.DataValue != null && d.DataValue.Value != null)
                                {
                                    try
                                    {
#if !WINDOWS_UWP
                                    if (d1 == null || d1.Status == DispatcherOperationStatus.Completed ||
                                            d1.Status == DispatcherOperationStatus.Aborted)
#endif
                                    {
#if !WINDOWS_UWP
                                    // pump read attributes outside UI thread
                                    try
                                    {
                                        var name = n.NodeIdViewModel.BrowseName.Name;
                                        var path = n.NodeIdViewModel.CompletePath;
                                    }
                                    catch { }

                                    d1 = controlTemp.Dispatcher.BeginInvokeAsynchronously(IsFreezable ? controlTemp as FrameworkElement : null, () =>
#else
                                    RunOnUIThread.RunIfRequired(() =>
#endif
                                        {
                                            try
                                            {
                                                if (d.Value != null)
                                                    value = d.Value;
                                                if (n.NodeIdViewModel != null)
                                                {
                                                    mapDynamics[n.NodeIdViewModel.BrowseName.Name] = value;
                                                    mapDynamics[n.NodeIdViewModel.CompletePath] = value;
                                                }
                                                else
                                                    mapDynamics[n.RelativePath] = value;
                                                mapDynamics[OpcuaEntityReference.HumanReadableNoProject] = value;
                                            }
                                            catch (Exception ex)
                                            {

                                            }

                                            LastDataNotConverted = d.DataValue;

                                            OnDataChanged();
                                        });
                                        /*
#if !WINDOWS_UWP
                                        if (d1 != null)
                                        {
                                            if (d1.Status != DispatcherOperationStatus.Completed)
                                                d1.Completed += (o, e) => { d1 = null; };
                                            else
                                                d1 = null;
                                        }
#endif
                                        /*
                                        }
                                    }
                                    catch (Exception ex)
                                    {

                                    }
                                }
                                */

                                if (controlTemp != null)
                                {
#if !WINDOWS_UWP
                                    if (d2 == null || d2.Status == DispatcherOperationStatus.Completed ||
                                        d2.Status == DispatcherOperationStatus.Aborted)
#endif
                                {
#if !WINDOWS_UWP
                                    d2 = controlTemp.Dispatcher.BeginInvokeAsynchronously(IsFreezable ? controlTemp as FrameworkElement : null, () =>
#else
                                    RunOnUIThread.RunIfRequired(() =>
#endif
                                    {
                                        try
                                        {
                                            InternalExecute(d.DataValue);
                                        }
                                        catch { }
                                    });
                                    /*
#if !WINDOWS_UWP
                                    if (d2 != null)
                                    {
                                        if (d2.Status != DispatcherOperationStatus.Completed)
                                            d2.Completed += (o, e) => { d2 = null; };
                                        else
                                            d2 = null;
                                    }
#endif
                                    */
                                    }
                                }
                            });

                            if (control != null)
                            {
#if !WINDOWS_UWP
                                if (d3 == null || d3.Status == DispatcherOperationStatus.Completed
                                    || d3.Status == DispatcherOperationStatus.Aborted)
#endif
                                {
#if !WINDOWS_UWP
                                    d3 = control.Dispatcher.BeginInvokeAsynchronously(IsFreezable ? control as FrameworkElement : null, () =>
#else
                                    RunOnUIThread.RunIfRequired(() =>
#endif
                                    {
                                        try
                                        {
                                            InternalExecute(monitoredItemViewModel.DataValue);
                                        }
                                        catch { }
                                    });
                                    /*
#if !WINDOWS_UWP
                                    if (d3 != null)
                                    {
                                        if (d3.Status != DispatcherOperationStatus.Completed)
                                            d3.Completed += (o, e) => { d3 = null; };
                                        else
                                            d3 = null;
                                    }
#endif
                                    */
                                }
                            }
                        }
                    });


                OpcuaEntityReference.Resolve(SessionName, parent);
                OpcuaEntityReference.SetInUse(Entity, true);
            }
        }

        public virtual void Terminate()
        {
#if !WINDOWS_UWP
            if (IdleExecutionPending != null && IdleExecutionPending.Status != DispatcherOperationStatus.Aborted &&
                IdleExecutionPending.Status != DispatcherOperationStatus.Completed)
            {
                IdleExecutionPending.Abort();
                // IdleExecutionPending = null;
            }
#endif

            bInit = false;
            if (OpcuaEntityReference != null)
            {
                OpcuaEntityReference.SetInUse(Entity, false);

                if (observer != null)
                    observer.Dispose();
                if (datavalueobserver != null)
                    datavalueobserver.Dispose();
                observer = null;
                datavalueobserver = null;

                if (expressionEntity != null)
                {
                    expressionEntity.ParserError -= ExpressionEntity_ParserError;
                    expressionEntity.ExecutionError -= ExpressionEntity_ExecutionError;
                    expressionEntity.Dispose();
                    expressionEntity = null;
                }
            }
#if !WINDOWS_UWP
            //if (d1 != null && d1.Status != DispatcherOperationStatus.Aborted &&
            //    d1.Status != DispatcherOperationStatus.Completed)
            //    d1.Abort();
            if (d2 != null && d2.Status != DispatcherOperationStatus.Aborted &&
                d2.Status != DispatcherOperationStatus.Completed)
                d2.Abort();
            if (d3 != null && d3.Status != DispatcherOperationStatus.Aborted &&
                d3.Status != DispatcherOperationStatus.Completed)
                d3.Abort();
            //if (d4 != null && d4.Status != DispatcherOperationStatus.Aborted &&
            //    d4.Status != DispatcherOperationStatus.Completed)
            //    d4.Abort();
#endif
            Stop();
            Control = null;
#if !WINDOWS_UWP
            Control3D = null;
#endif
            DataChanged = null;
            lastdata = null;
            lastValue = null;
            lastDataNotConverted = null;
        }

        public bool IsInitialized()
        {
            return bInit;
        }

        public void ExecuteWithData(DataValue data)
        {
            if (OpcuaEntityReference != null && OpcuaEntityReference.MonitoredItemViewModel != null)
                data = OpcuaEntityReference.MonitoredItemViewModel.DataValue;

#if !WINDOWS_UWP
            var control = Control;
            if (control != null)
                control.Dispatcher.BeginInvokeIfRequired(() =>
#else
            RunOnUIThread.RunIfRequired(() =>
#endif
            {
                InternalExecute(data);
            });
        }

        public virtual void ExecuteChangeLanguage(bool bPreparing = false)
        {
        }

        internal virtual bool ExecuteWithValue(double dValue)
        {
            return false;
        }

        static bool isDataGood(DataValue data)
        {
            if (data == null)
                return false;

            return StatusCode.IsGood(data.StatusCode) || data.StatusCode == Opc.Ua.StatusCodes.UncertainLastUsableValue;
        }

        bool bNotEnabled;
        protected void InternalExecute(DataValue data, bool bCompareLastValue = true)
        {
            if (data != null && !isDataGood(data))
                return;

            double value1 = GetDataValue(data);
            if (IsInvalidDouble(value1))
            {
                value1 = 0;
                if (bCompareLastValue && data == lastdata)
                    return;
            }
            else if (bCompareLastValue && lastValue.HasValue && lastValue == value1)
                return;

            lastValue = value1;
            lastdata = data;
            if (bNotEnabled)
                return;

            if (ExecuteWithValue(value1))
                return;

            switch (AnimationBehavior)
            {
                case AnimationBehavior.Trigger:
                    {
                        if (value1 != 0)
                        {
                            if (!bExecuted)
                            {
                                bExecuted = true;
                                dTargetValue = CommonTarget;
                                Execute();
                            }
                        }
                        else if (bExecuted)
                        {
                            bExecuted = false;
                            Stop();
                        }
                        break;
                    }
                case AnimationBehavior.Absolute:
                    {
                        dTargetValue = value1;
                        if (bRangeLimit)
                        {
                            dTargetValue = Math.Max(dTargetValue, Range.Low);
                            dTargetValue = Math.Min(dTargetValue, Range.High);
                        }
                        if (dTargetValue == Range.Low && (Autoreverse || Repeatable))
                        {
                            bExecuted = false;
                            Stop();
                            break;
                        }
                        bExecuted = true;
                        Execute();
                        break;
                    }
                case AnimationBehavior.Proportional:
                    {
                        dTargetValue = (value1 - Range.Low) / (Range.High - Range.Low) * CommonTarget;
                        if (bRangeLimit)
                        {
                            dTargetValue = Math.Max(dTargetValue, Range.Low);
                            dTargetValue = Math.Min(dTargetValue, Range.High);
                        }
                        if (IsInvalidDouble(dTargetValue))
                            dTargetValue = 0;
                        if (dTargetValue == 0 && (Autoreverse || Repeatable))
                        {
                            bExecuted = false;
                            Stop();
                            break;
                        }
                        bExecuted = true;
                        Execute();
                        break;
                    }
            }
        }

#if !WINDOWS_UWP
        DispatcherOperation IdleExecutionPending;
#endif
        protected virtual void Reexecute()
        {
            if (!bExecuted)
                return;

#if !WINDOWS_UWP
            if (IdleExecutionPending != null && IdleExecutionPending.Status != DispatcherOperationStatus.Completed &&
                IdleExecutionPending.Status != DispatcherOperationStatus.Aborted)
                return;

            Action action = () =>
                {
                    Stop();
                    if (bInit)
                        Execute();
                    else
                        Demo();
                };
            var control = Control;
            var control3D = Control3D;
            if (control != null)
            {
                IdleExecutionPending = control.Dispatcher.BeginInvoke(action, DispatcherPriority.Background);
                // IdleExecutionPending.Completed += (sender, e) => IdleExecutionPending = null;
            }
            else if (control3D != null)
            {
                IdleExecutionPending = control3D.Dispatcher.BeginInvoke(action, DispatcherPriority.Background);
                // IdleExecutionPending.Completed += (sender, e) => IdleExecutionPending = null;
            }
#else
            Stop();
            if (bInit)
                Execute();
#endif
        }

        protected static bool IsInvalidDouble(double value)
        {
            return Double.IsNaN(value) || Double.IsInfinity(value) ||
                            Double.IsNegativeInfinity(value) || Double.IsPositiveInfinity(value);
        }

        static System.Globalization.CultureInfo culture = new System.Globalization.CultureInfo("en-US");
        protected double GetDataValue(DataValue dv)
        {
            if (dv == null)
                return Double.NaN;

            object modelDataValueGetValue = dv.GetValue(null);
            if (modelDataValueGetValue == null || 
                !isDataGood(dv))
                return Double.NaN;

            try
            {
                if (modelDataValueGetValue is Double)
                    return (Double)modelDataValueGetValue;
                else if (modelDataValueGetValue is float)
                {
                    var sVal = Convert.ToString(modelDataValueGetValue, culture);
                    return Convert.ToDouble(sVal, culture);
                }
                return Convert.ToDouble(modelDataValueGetValue, culture);
            }
            catch (Exception ex)
            {
            }

            return Double.NaN;
        }
#endif
#endregion

#region Abstracts

        public virtual void RefreshAllEntityReferences()
        {
            if (OpcuaEntityReference != null)
            {
                var xml = OpcuaEntityReference.ToXml();
                OpcuaEntityReference = xml.FromXml<OPCUAEntityReference>();
            }
        }

#if !NET_STANDARD
#if !WINDOWS_UWP
        public virtual void Demo()
        {
            bExecuted = true;
            bDemoMode = true;
        }
#endif
        public abstract void Execute();

        public virtual void Enable(bool bEnable)
        {
            if (bNotEnabled != bEnable)
                return;

            bNotEnabled = !bEnable;
            if (bEnable)
            {
                if (lastdata != null)
                    InternalExecute(lastdata, false);
            }
            else
                Stop();
        }

        public virtual void Stop()
        {
            bExecuted = false;
            bDemoMode = false;
        }
#endif
        public abstract String Name { get; }

#if !WINDOWS_UWP && !NET_STANDARD
        [Browsable(false)]
        public virtual UserControl Editor
        {
            get
            {
                return new UserControls.CommonAnimationPropertyEditor();
            }
        }

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
        public virtual double CommonTarget
        {
            get
            {
                return Double.NaN;
            }
        }

#if !WINDOWS_UWP
        [Browsable(false)]
#endif
        public virtual bool Is3D
        {
            get
            {
                return false;
            }
        }

#if !WINDOWS_UWP
        [Browsable(false)]
#endif
        public virtual bool Is2D
        {
            get
            {
                return false;
            }
        }

#if !WINDOWS_UWP
        [Browsable(false)]
#endif
        public virtual bool IsFreezable
        {
            get
            {
                return true;
            }
        }

#if !WINDOWS_UWP
        public virtual String AnimationSummary
        {
            get
            {
                //return String.Empty;
                if (OpcuaEntityReference == null
#if !NET_STANDARD
                    || !OpcuaEntityReference.IsValid
#endif
                    )
                    return String.Empty;
                if (OpcuaEntityReference.ReadablePath == null || OpcuaEntityReference.AppName == null)
                    return OpcuaEntityReference.HumanReadable;
                //return OpcuaEntityReference.HumanReadable;
                Opc.Ua.NamespaceTable n = new Opc.Ua.NamespaceTable();
                UInt16 ns = (UInt16)(n.Count + 2 - 1);
                string oldChars = string.Format("{0}:", ns);
                return string.Format("{0} ({1})", (OpcuaEntityReference.ReadablePath).Replace(oldChars, ""), OpcuaEntityReference.AppName);
            }
        }
#endif
        public virtual Type[] ExpectingControl()
        {
            return null;
        }
#if !NET_STANDARD
        public virtual bool NotSupportedControl(UIElement element)
        {
            return false;
        }
#endif
#endregion

#region Commands
#if !WINDOWS_UWP && !NET_STANDARD
        RelayCommand _executeCommand;
        [Browsable(false)]
        public ICommand ExecuteCommand
        {
            get
            {
                if (_executeCommand == null)
                {
                    _executeCommand = new RelayCommand(
                        param => Execute(),
                        param => Control != null
                        );
                }
                return _executeCommand;
            }
        }

        RelayCommand _stopCommand;
        [Browsable(false)]
        public ICommand StopCommand
        {
            get
            {
                if (_stopCommand == null)
                {
                    _stopCommand = new RelayCommand(
                        param => Stop(),
                        param => Control != null
                        );
                }
                return _stopCommand;
            }
        }
#endif
#endregion

#region Dropping
#if !WINDOWS_UWP && !NET_STANDARD

        public bool OnDropReference(Object reference)
        {
            if (reference is ReferenceDescriptionViewModel)
            {
                OpcuaEntityReference = (reference as ReferenceDescriptionViewModel).CreateEntityReference(null);
                return true;
            }

            return false;
        }
#endif
#endregion

#region DataChange Event
#if !NET_STANDARD
        public event EventHandler DataChanged;

        void OnDataChanged()
        {
            var t = DataChanged;
            if (t != null)
                t(this, EventArgs.Empty);
        }

        protected EventHandler OnExpressionParserError;

        protected EventHandler OnExpressionExecutionError;
#endif
#endregion

#if !WINDOWS_UWP && !NET_STANDARD
#region INotifyPropertyChanged Members

        /// <summary>
        /// Raised when a property on this object has a new value.
        /// </summary>
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
#endregion // INotifyPropertyChanged Members

#region INotifyPropertyVisibilityChanged Members

        /// <summary>
        /// Gets the visibility state for the property with the given name.
        /// </summary>
        /// <param name="propertyName">The property name that you want konw the current visibility state.</param>
        /// <returns></returns>
        public virtual bool this[string propertyName]
        {
            get
            {
                if (propertyName == "SVGReferenceId" || propertyName == "SVGItemId")
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
#endregion

#region Validations
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
            if (propertyName == "Range.Low")
            {
                if (Range.Low >= Range.High)
                    return Properties.Resources.InvalidRangeValues;
            }
            else if (propertyName == "Range.High")
            {
                if (Range.Low >= Range.High)
                    return Properties.Resources.InvalidRangeValues;
            }
            if (propertyName == "AnimationTime")
            {
                if (AnimationTime < 0)
                    return Properties.Resources.InvalidAnimationTime;
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
#endregion
#endif

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
