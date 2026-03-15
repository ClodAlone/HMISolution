using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Threading;
using System.Threading.Tasks;
using Utilities;
using Utilities.WPF;
using System.Xml;
using System.Windows.Controls;
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using System.Windows.Input;
using System.Windows.Shapes;
using System.Windows.Automation.Peers;
using System.Windows.Automation;
using System.Windows.Automation.Provider;
using ScreenSettings;
using VFS;
using OPCUAViewModel;
using log4net;
using WPFUtilities;
using ScreenSettings.Entities;
using DocumentManager.ComponentService;
using System.Globalization;
using UFUAEditor.ComponentService;
using UFUserEditor.ComponentService;
using System.IO.IsolatedStorage;
using System.Reflection;
using System.Runtime.Serialization;
using Opc.Ua;
using StringManager.ComponentService;
using UnitConverterManager.ComponentService;
using System.Security.AccessControl;
using System.Security.Principal;
using UFInterfaces.RemotelyCommandable;
using DevExpress.Xpf.Core;

namespace WPFScreenSink
{
    public class ScreenSink : IDisposable
    {
        #region Declarations
        string _Culture;
        String _Converter;
        string _Role;
        int _AccessMask;
        int _AccessLevel;
        Thread thread;
        static long currentSessions = 0;
        bool licenseAcquired;

        Window dispatcher;
        DispatcherTimer timer;
        Grid gridContainer;

        List<String> listOpenedUri = new List<String>();

        readonly Object lockObject = new Object();
        static readonly Object lockStaticObject = new Object();
        static Semaphore lockSemaphoreRenderingPipeLine;
        readonly Dictionary<Uri, ScreenDocument> mapUriToDocuments = new Dictionary<Uri, ScreenDocument>();
        readonly Dictionary<Uri, List<String>> mapZoomVisibilityEntities = new Dictionary<Uri, List<String>>();

        readonly Dictionary<ScreenDocument, Canvas> mapDocumentsToBeDisposed = new Dictionary<ScreenDocument, Canvas>();

        readonly Dictionary<Uri, Canvas> mapUriToCanvas = new Dictionary<Uri, Canvas>();
        readonly Dictionary<Uri, Brush> mapUriToCanvasBackground = new Dictionary<Uri, Brush>();
        readonly Dictionary<Uri, List<ScreenManager.SpecialObjects.EmbeddedTabScreen>> mapUriToEmbeddeds = new Dictionary<Uri, List<ScreenManager.SpecialObjects.EmbeddedTabScreen>>();

        readonly Dictionary<Uri, String> mapUriToBackground = new Dictionary<Uri, String>();
        readonly Dictionary<Uri, List<UIElement>> mapUriToBackgroundChildren = new Dictionary<Uri, List<UIElement>>();

        readonly Dictionary<Guid, UIElement> mapGuidToElementUpdateOnly = new Dictionary<Guid, UIElement>();
        readonly Dictionary<Guid, UIElement> mapGuidToElement = new Dictionary<Guid, UIElement>();
        readonly Dictionary<UIElement, Guid> mapElementToGuid = new Dictionary<UIElement, Guid>();
        readonly Dictionary<UIElement, String> mapElementCommandToName = new Dictionary<UIElement, String>();
        readonly Dictionary<UIElement, ScreenDocument> mapElementCommandToDoc = new Dictionary<UIElement, ScreenDocument>();
        readonly Dictionary<FrameworkElement, Guid> mapDataContextElementToGuid = new Dictionary<FrameworkElement, Guid>();
        readonly Dictionary<UIElement, Rect> mapOriginalBound = new Dictionary<UIElement, Rect>();
        readonly Dictionary<Guid, String> mapGuidToBase64Element = new Dictionary<Guid, String>();
        readonly Dictionary<Guid, Rect> mapGuidToElementPos = new Dictionary<Guid, Rect>();
        readonly Dictionary<Guid, Status> mapGuidToElementStatus = new Dictionary<Guid, Status>();
        readonly Dictionary<Guid, MonitoredItemViewModel> mapGuidToDataContextItem = new Dictionary<Guid, MonitoredItemViewModel>();
        readonly Dictionary<Guid, Canvas> mapGuidToParentCanvas = new Dictionary<Guid, Canvas>();
        readonly List<Guid> listChanged = new List<Guid>();
        readonly List<Guid> listChangedPending = new List<Guid>();
        readonly List<Guid> listStatusChanged = new List<Guid>();
        readonly List<Guid> listStatusChangedPending = new List<Guid>();
        // readonly List<Guid> listUpdatingImages = new List<Guid>();

        private static readonly ILog log = LogManager.GetLogger(Properties.Resources.WebClientHTML5Log);
        private static readonly ILog logLicense = LogManager.GetLogger(Properties.Resources.LicenseManager);

        readonly String Theme;
        readonly String ClientId;
        readonly bool DisableStaticOptimization;
        readonly ServerType ServerType;

        DateTime lastTimeUsed;
        bool bThreadTerminated;

        static Dictionary<String, ScreenSink> mapIdSinks = new Dictionary<String, ScreenSink>();
        static List<String> listToDispose = new List<String>();

        static String commonFolder;

        readonly static WebSessions.WebSessions activeSessions;
        #endregion

        #region Ctor

        static ScreenSink()
        {
            activeSessions = new WebSessions.WebSessions();
            commonFolder = String.Format("{0}\\{1}\\{2}\\Resources\\",
                                 Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
                                 Properties.Settings.Default.CompanyName,
                                 Properties.Settings.Default.CommonApplicationFolder);
        }

        public ScreenSink(String theme, String clientId, ServerType serverType, bool disableStaticOptimization, bool bPopup = false)
        {
            isPopup = bPopup;
            Theme = theme;
            ClientId = clientId;
            DisableStaticOptimization = disableStaticOptimization;
            ServerType = serverType;
            lock (mapIdSinks)
            {
                if (!mapIdSinks.ContainsKey(ClientId))
                    mapIdSinks.Add(ClientId, this);
            }
        }

        //#region OnRecycling
        ///// <summary>
        ///// Triggers the Recycling event.
        ///// </summary>
        //public event EventHandler Recycling;
        //public virtual void OnRecycling()
        //{
        //    var e = Recycling;
        //    if (e != null)
        //        e(this, EventArgs.Empty);
        //}
        //#endregion

        static public void DisposeClientId(String clientId)
        {
            lock (mapIdSinks)
            {
                if (mapIdSinks.ContainsKey(clientId) && !listToDispose.Contains(clientId))
                    listToDispose.Add(clientId);
            }
        }

        #endregion

        #region Methods
        bool isPopup = false;
        void EnsureThread()
        {
            lock (lockObject)
            {
                if (bDisposed)
                    return;

                if (thread != null)
                    return;

                if (!isPopup)
                {
                	licenseAcquired = activeSessions.Acquire();
                    if (!licenseAcquired)
                    {
                        logLicense.WarnFormat(Properties.Resources.LicenseClientExceeded, activeSessions.MaxSessions);
                        return;
                    }
	                else
	                {
	                    logLicense.InfoFormat(Properties.Resources.LicenseClientEntered, Interlocked.Increment(ref currentSessions));
	                }
                }

                try
                {
                    using (ManualResetEvent eventStarted = new ManualResetEvent(false))
                    {
                        thread = new Thread(() =>
                        {
                            try
                            {
                                //Timeline.DesiredFrameRateProperty.OverrideMetadata(
                                //   typeof(Timeline),
                                //   new FrameworkPropertyMetadata { DefaultValue = 5 }
                                //   );

                                lock (lockStaticObject)
                                {
                                    //log.Info(Properties.Resources.CreatingNewSession);

                                    dispatcher = new ThemedWindow() { Visibility = Visibility.Hidden };
                                    //#if DEBUG
                                    //dispatcher.Visibility = Visibility.Visible;
                                    //#endif
                                    dispatcher.Closing += dispatcher_Closing;
                                    dispatcher.Show();
                                    // }
                                    gridContainer = new Grid();
                                    dispatcher.Content = gridContainer;

                                    ThemeHelper.SetTheme(dispatcher, Theme);
                                    var commonFolderRoot = String.Format("{0}\\{1}\\{2}",
                                                            Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
                                                            Properties.Settings.Default.CompanyName,
                                                            Properties.Settings.Default.CommonApplicationFolder);
                                    var commonFolder = String.Format("{0}\\{1}\\", commonFolderRoot, "Resources");
                                    ResourceDictionaryExtensions.AddCommonResources(dispatcher, commonFolder, commonFolderRoot);
                                    timer = new DispatcherTimer(DispatcherPriority.Background);
                                    timer.Tick += timer_Tick;
                                    timer.Interval = TimeSpan.FromMilliseconds(RefreshPollingTime);
                                    timer.Start();
                                }

                                eventStarted.Set();
                                System.Windows.Threading.Dispatcher.Run();
                            }
                            catch (ThreadAbortException)
                            {
                                EnsureReleaseLicense();
                                bThreadTerminated = true;
                                dispatcher.Closing -= dispatcher_Closing;
                            }
                            finally
                            {
                                EnsureReleaseLicense();
                                bThreadTerminated = true;
                                dispatcher.Closing -= dispatcher_Closing;
                            }
                        });

                        thread.SetApartmentState(ApartmentState.STA);
                        thread.IsBackground = true;
                        thread.Start();

                        eventStarted.WaitOne();
                    }
                }
                catch
                {
                    EnsureReleaseLicense();
                    throw;
                }
            }
        }

        public void ReleaseLicense()
        {
            EnsureReleaseLicense();
        }

        bool licenseReleased;
        void EnsureReleaseLicense()
        {
            lock (lockObject)
            {
                if (licenseAcquired)
                {
                    licenseAcquired = false;
                    activeSessions.Release();
                    logLicense.InfoFormat(Properties.Resources.LicenseClientReleased, Interlocked.Decrement(ref currentSessions));
                }
            }
        }

        void dispatcher_Closing(object sender, EventArgs e)
        {
            dispatcher.Dispatcher.BeginInvokeShutdown(DispatcherPriority.Background);
        }

        static bool HasValueProvider(AutomationPeer autoElement, Point pt, bool bAny = false)
        {
            if (autoElement == null || autoElement.GetChildren() == null)
                return false;

            foreach (AutomationPeer child in autoElement.GetChildren())
            {
                var rect = child.GetBoundingRectangle();
                if (!child.IsEnabled() || rect.Width == 0 || rect.Height == 0 || !bAny && !rect.Contains(pt))
                    continue;

                var valueProv = child.GetPattern(PatternInterface.Value) as IValueProvider;
                if (valueProv != null && !valueProv.IsReadOnly)
                    return true;

                if (HasValueProvider(child, pt, bAny))
                    return true;
            }

            return false;
        }

        static bool HasValueProvider(UIElement element, Point pt, bool bAny = false)
        {
            if (element == null)
                return false;

            try
            {
                var autoElement = UIElementAutomationPeer.CreatePeerForElement(element) as UIElementAutomationPeer;
                if (autoElement == null && element is ContentControl && (element as ContentControl).Content is UIElement)
                {
                    element = (element as ContentControl).Content as UIElement;
                    autoElement = UIElementAutomationPeer.CreatePeerForElement(element) as UIElementAutomationPeer;
                }

                if (autoElement == null)
                    return false;

                var invokeSelectedItem = autoElement.GetPattern(PatternInterface.SelectionItem) as ISelectionItemProvider;
                if (invokeSelectedItem != null)
                    invokeSelectedItem.Select();

                if (autoElement != null)
                {
                    var valueProv = autoElement.GetPattern(PatternInterface.Value) as IValueProvider;
                    if (valueProv != null && !valueProv.IsReadOnly)
                        return true;
                }

                try
                {
                    if (HasValueProvider(autoElement, pt, bAny))
                        return true;
                }
                catch(Exception ex)
                { }
            }
            catch (Exception ex)
            { }

            return false;
        }

        static String GetValueProvider(AutomationPeer autoElement, Point pt, bool bIsDown, ref bool bStopPropagation, ref IExpandCollapseProvider expProv, bool bRecursing = false)
        {
            if (autoElement == null || autoElement.GetChildren() == null)
                return null;

            foreach (var child in autoElement.GetChildren())
            {
                var rect = child.GetBoundingRectangle();
                if (rect.Width == 0 || rect.Height == 0 || !child.IsEnabled() || !rect.Contains(pt))
                {
#if DEBUGTRACE
                    System.Diagnostics.Debug.WriteLine(String.Format("Control not clickable {0}, isEnabled = {1}, rect = {2}, pt = {3}",
                        child.GetType(), child.IsEnabled(), rect, pt));
#endif
                    continue;
                }

                var uipeer = child as UIElementAutomationPeer;
                if (uipeer != null && (!uipeer.Owner.IsVisible || !uipeer.Owner.IsHitTestVisible))
                    continue;

                var invokeSelectedItem = child.GetPattern(PatternInterface.SelectionItem) as ISelectionItemProvider;
                if (invokeSelectedItem != null)
                    invokeSelectedItem.Select();
 
                var valueProv = child.GetPattern(PatternInterface.Value) as IValueProvider;
                if (valueProv != null && !valueProv.IsReadOnly)
                    return valueProv.Value;

                if (!bIsDown)
                {
                    var invokeProv = child.GetPattern(PatternInterface.Invoke) as IInvokeProvider;
                    if (invokeProv != null)
                    {
                        invokeProv.Invoke();
                    }

                    var toggleProv = child.GetPattern(PatternInterface.Toggle) as IToggleProvider;
                    if (toggleProv != null)
                    {
                        toggleProv.Toggle();
                    }

                    var expandProv = child.GetPattern(PatternInterface.ExpandCollapse) as IExpandCollapseProvider;
                    if (expandProv != null)
                    {
                        uipeer = autoElement as UIElementAutomationPeer;
                        if (uipeer == null || !(uipeer.Owner is ComboBox))
                            expProv = expandProv;
                    }

                    var scrollProv = child.GetPattern(PatternInterface.Scroll) as IScrollProvider;
                    if (scrollProv != null)
                    {
                        var rcTest = rect;
                        rcTest.X += rcTest.Width - scrollSize;
                        if (rcTest.Contains(pt))
                        {
                            rcTest = rect;
                            rcTest.Height = scrollSize;
                            if (rcTest.Contains(pt))
                                scrollProv.Scroll(ScrollAmount.NoAmount, ScrollAmount.SmallDecrement);
                            else
                            {
                                rcTest = rect;
                                rcTest.Y += rcTest.Height - scrollSize;
                                if (rcTest.Contains(pt))
                                    scrollProv.Scroll(ScrollAmount.NoAmount, ScrollAmount.SmallIncrement);
                                else
                                {
                                    rcTest = rect;
                                    rcTest.Y += rcTest.Height / 2;
                                    if (rcTest.Contains(pt))
                                        scrollProv.Scroll(ScrollAmount.NoAmount, ScrollAmount.LargeIncrement);
                                    else
                                        scrollProv.Scroll(ScrollAmount.NoAmount, ScrollAmount.LargeDecrement);
                                }
                            }
                        }

                        rcTest = rect;
                        rcTest.Y += rcTest.Height - scrollSize;
                        if (rcTest.Contains(pt))
                        {
                            rcTest = rect;
                            rcTest.Width = scrollSize;
                            if (rcTest.Contains(pt))
                                scrollProv.Scroll(ScrollAmount.SmallDecrement, ScrollAmount.NoAmount);
                            else
                            {
                                rcTest = rect;
                                rcTest.X += rcTest.Width - scrollSize;
                                if (rcTest.Contains(pt))
                                    scrollProv.Scroll(ScrollAmount.SmallIncrement, ScrollAmount.NoAmount);
                                else
                                {
                                    rcTest = rect;
                                    rcTest.X += rcTest.Width / 2;
                                    if (rcTest.Contains(pt))
                                        scrollProv.Scroll(ScrollAmount.LargeIncrement, ScrollAmount.NoAmount);
                                    else
                                        scrollProv.Scroll(ScrollAmount.LargeDecrement, ScrollAmount.NoAmount);
                                }
                            }
                        }
                    }
                }

                try
                {
                    var ret = GetValueProvider(child, pt, bIsDown, ref bStopPropagation, ref expProv, true);
                    if (ret != null)
                        return ret;
                }
                catch(Exception ex)
                { }
            }

            if (!bRecursing && expProv != null)
            {
                bStopPropagation = true;
                if (expProv.ExpandCollapseState != ExpandCollapseState.Expanded)
                    expProv.Expand();
                else
                    expProv.Collapse();
            }
            return null;
        }

        static String GetValueProvider(HitTestResult res, Point pt, bool bIsDown, ref bool bStopPropagation)
        {
            try
            {
                var element = res.VisualHit as UIElement;
                var autoElement = UIElementAutomationPeer.CreatePeerForElement(element) as UIElementAutomationPeer;
                if (autoElement == null && element is ContentControl && (element as ContentControl).Content is UIElement)
                {
                    element = (element as ContentControl).Content as UIElement;
                    autoElement = UIElementAutomationPeer.CreatePeerForElement(element) as UIElementAutomationPeer;
                }
                if (autoElement == null || !element.IsHitTestVisible)
                    return null;
                element.UpdateLayout();
                var rect = autoElement.GetBoundingRectangle();
                if (rect.Width == 0 && (element as FrameworkElement).ActualWidth == 0 || rect.Height == 0 && (element as FrameworkElement).ActualHeight == 0 || !autoElement.IsEnabled())
                    return null;

                // var winPoint = autoElement.GetClickablePoint();

                var invokeSelectedItem = autoElement.GetPattern(PatternInterface.SelectionItem) as ISelectionItemProvider;
                if (invokeSelectedItem != null)
                    invokeSelectedItem.Select();

                if (autoElement.Owner is ComboBox)
                {
                    var combo = autoElement.Owner as ComboBox;
                    combo.IsEditable = true;
                }

                var valueProv = autoElement.GetPattern(PatternInterface.Value) as IValueProvider;
                if (valueProv != null)
                {
                    if (autoElement.Owner is ComboBox)
                    {
                        var combo = autoElement.Owner as ComboBox;
                        var ret = new StringBuilder();
                        foreach(var item in combo.Items)
                        {
                            if (item is ComboBoxItem)
                            {
                                var txt = (item as ComboBoxItem).Content.ToString();
                                if (ret.Length != 0)
                                    ret.Append(ListSerializationHelper.tagSelectionSeparator);
                                ret.Append(txt);
                            }
                            else if (item is String)
                            {
                                var txt = item as String;
                                if (ret.Length != 0)
                                    ret.Append(ListSerializationHelper.tagSelectionSeparator);
                                ret.Append(txt);
                            }
                        }
                        return String.Format("{0}{1}", ListSerializationHelper.tagListSelection, ret.ToString());
                    }
                    if (!valueProv.IsReadOnly)
                        return valueProv.Value;
                }

                IExpandCollapseProvider expProv = null;
                return GetValueProvider(autoElement, pt, bIsDown, ref bStopPropagation, ref expProv);
            }
            catch (Exception ex)
            {
            }

            return null;
        }

        static void ResetValueProviderSelection(HitTestResult res, Point pt)
        {
            try
            {
                var element = res.VisualHit as UIElement;
                var autoElement = UIElementAutomationPeer.CreatePeerForElement(element) as UIElementAutomationPeer;
                if (autoElement == null && element is ContentControl && (element as ContentControl).Content is UIElement)
                {
                    element = (element as ContentControl).Content as UIElement;
                    autoElement = UIElementAutomationPeer.CreatePeerForElement(element) as UIElementAutomationPeer;
                }
                if (autoElement == null)
                    return;

                ResetValueProviderSelection(autoElement, pt);
            }
            catch (Exception ex)
            {
            }
        }

        static void ResetValueProviderSelection(AutomationPeer autoElement, Point pt)
        {
            if (autoElement == null || autoElement.GetChildren() == null)
                return;

            var list = autoElement.GetChildren();
            foreach (var child in list)
            {
                var rect = child.GetBoundingRectangle();
                if (!child.IsEnabled())
                {
#if DEBUGTRACE
                    System.Diagnostics.Debug.WriteLine(String.Format("Control not clickable {0}, isEnabled = {1}, rect = {2}, pt = {3}",
                        child.GetType(), child.IsEnabled(), rect, pt));
#endif
                    continue;
                }

                var invokeSelectedItem = child.GetPattern(PatternInterface.SelectionItem) as ISelectionItemProvider;
                if (invokeSelectedItem != null && !invokeSelectedItem.IsSelected)
                {
                    invokeSelectedItem.Select();
                    break;
                }

                try
                {
                    ResetValueProviderSelection(child, pt);
                }
                catch (Exception)
                {
                }
            }

            foreach (var child in list)
            {
                var rect = child.GetBoundingRectangle();
                if (!child.IsEnabled() || rect.Width == 0 || rect.Height == 0 || !rect.Contains(pt))
                {
#if DEBUGTRACE
                    System.Diagnostics.Debug.WriteLine(String.Format("Control not clickable {0}, isEnabled = {1}, rect = {2}, pt = {3}",
                        child.GetType(), child.IsEnabled(), rect, pt));
#endif
                    continue;
                }

                var invokeSelectedItem = child.GetPattern(PatternInterface.SelectionItem) as ISelectionItemProvider;
                if (invokeSelectedItem != null)
                {
                    invokeSelectedItem.Select();
                    break;
                }

                try
                {
                    ResetValueProviderSelection(child, pt);
                }
                catch (Exception)
                {
                }
            }
        }

        static String SetValueProvider(AutomationPeer autoElement, String value, Point pt)
        {
            if (autoElement == null || autoElement.GetChildren() == null)
                return null;

            foreach (var child in autoElement.GetChildren())
            {
                var rect = child.GetBoundingRectangle();
                if (!child.IsEnabled() || rect.Width == 0 || rect.Height == 0 || !rect.Contains(pt))
                    continue;

                var uipeer = child as UIElementAutomationPeer;
                if (uipeer != null && !uipeer.Owner.IsVisible)
                    continue;

                var valueProv = child.GetPattern(PatternInterface.Value) as IValueProvider;
                if (valueProv != null/* && !valueProv.IsReadOnly*/)
                {
                    valueProv.SetValue(value);
                    if (!string.IsNullOrEmpty(value) && string.IsNullOrEmpty(valueProv.Value))
                    {
                        DateTime dValue;
                        if (DateTime.TryParse(value, System.Threading.Thread.CurrentThread.CurrentCulture, DateTimeStyles.None, out dValue))
                            valueProv.SetValue(dValue.ToString(CultureInfo.InvariantCulture));
                    }
                    return String.Empty;
                }

                var ret = SetValueProvider(child, value, pt);
                if (ret == String.Empty)
                    return ret;
            }

            return null;
        }

        static String SetValueProvider(HitTestResult res, String value, Point pt)
        {
            try
            {
                var element = res.VisualHit as UIElement;
                var autoElement = UIElementAutomationPeer.CreatePeerForElement(element) as UIElementAutomationPeer;
                if (autoElement == null && element is ContentControl && (element as ContentControl).Content is UIElement)
                {
                    element = (element as ContentControl).Content as UIElement;
                    autoElement = UIElementAutomationPeer.CreatePeerForElement(element) as UIElementAutomationPeer;
                }
                if (autoElement == null)
                    return null;
                // var winPoint = autoElement.GetClickablePoint();

                var valueProv = autoElement.GetPattern(PatternInterface.Value) as IValueProvider;
                if (valueProv != null/* && !valueProv.IsReadOnly*/)
                {
                    valueProv.SetValue(value);
                    if (!string.IsNullOrEmpty(value) && string.IsNullOrEmpty(valueProv.Value))
                    {
                        DateTime dValue;
                        if (DateTime.TryParse(value, System.Threading.Thread.CurrentThread.CurrentCulture, DateTimeStyles.None, out dValue))
                            valueProv.SetValue(dValue.ToString(CultureInfo.InvariantCulture));
                    }
                    return String.Empty;
                }

                return SetValueProvider(autoElement, value, pt);
            }
            catch (Exception ex)
            {
                return ex.Message;
            }

            return null;
        }

        const double scrollSize = 30;
        bool SimulateEvent(Uri uri, HitTestResult res, MouseButton action, bool bIsDown, Rect rect, Point pnt)
        {
            if (!bIsDown)
            {
                try
                {
                    var element = res.VisualHit as UIElement;
                    var autoElement = UIElementAutomationPeer.CreatePeerForElement(element) as UIElementAutomationPeer;
                    if (autoElement == null && element is ContentControl && (element as ContentControl).Content is UIElement)
                    {
                        element = (element as ContentControl).Content as UIElement;
                        autoElement = UIElementAutomationPeer.CreatePeerForElement(element) as UIElementAutomationPeer;
                    }
                    if (autoElement != null)
                    {
                        // var winPoint = autoElement.GetClickablePoint();

                        var invokeSelectedItem = autoElement.GetPattern(PatternInterface.SelectionItem) as ISelectionItemProvider;
                        if (invokeSelectedItem != null)
                            invokeSelectedItem.Select();

                        var invokeProv = autoElement.GetPattern(PatternInterface.Invoke) as IInvokeProvider;
                        if (invokeProv != null)
                        {
                            invokeProv.Invoke();
                            if (!(element is ICommandSource))
                                CheckExecuteCommand(uri, res, bIsDown);
                            return true;
                        }

                        var toggleProv = autoElement.GetPattern(PatternInterface.Toggle) as IToggleProvider;
                        if (toggleProv != null)
                        {
                            toggleProv.Toggle();
                            CheckExecuteCommand(uri, res, bIsDown);
                            return true;
                        }

                        var expandProv = autoElement.GetPattern(PatternInterface.ExpandCollapse) as IExpandCollapseProvider;
                        if (expandProv != null && !(autoElement.Owner is ComboBox))
                        {
                            if (expandProv.ExpandCollapseState != ExpandCollapseState.Expanded)
                                expandProv.Expand();
                            else
                                expandProv.Collapse();
                            CheckExecuteCommand(uri, res, bIsDown);
                            return true;
                        }

                        var scrollProv = autoElement.GetPattern(PatternInterface.Scroll) as IScrollProvider;
                        if (scrollProv != null)
                        {
                            var rcTest = rect;
                            rcTest.X += rcTest.Width - scrollSize;
                            if (rcTest.Contains(pnt))
                            {
                                rcTest = rect;
                                rcTest.Height = scrollSize;
                                if (rcTest.Contains(pnt))
                                    scrollProv.Scroll(ScrollAmount.NoAmount, ScrollAmount.SmallDecrement);
                                else
                                {
                                    rcTest = rect;
                                    rcTest.Y += rcTest.Height - scrollSize;
                                    if (rcTest.Contains(pnt))
                                        scrollProv.Scroll(ScrollAmount.NoAmount, ScrollAmount.SmallIncrement);
                                    else
                                    {
                                        rcTest = rect;
                                        rcTest.Y += rcTest.Height / 2;
                                        if (rcTest.Contains(pnt))
                                            scrollProv.Scroll(ScrollAmount.NoAmount, ScrollAmount.LargeIncrement);
                                        else
                                            scrollProv.Scroll(ScrollAmount.NoAmount, ScrollAmount.LargeDecrement);
                                    }
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                }
            }

            var args = new MouseButtonEventArgs(Mouse.PrimaryDevice, 0, action) { RoutedEvent = bIsDown ? Mouse.MouseDownEvent : Mouse.MouseUpEvent };
            (res.VisualHit as UIElement).RaiseEvent(args);
            if (CheckExecuteCommand(uri, res, bIsDown))
                args.Handled = true;

            return args.Handled;
        }

        private bool CheckExecuteCommand(Uri uri, HitTestResult res, bool bIsDown)
        {
            if (/*!args.Handled && */!bIsDown/* && !(res.VisualHit is ICommandSource)*/)
            {
                var uie = res.VisualHit as UIElement;
                if (mapElementCommandToName.ContainsKey(uie) && mapElementCommandToDoc.ContainsKey(uie))
                {
                    // uie.Scale(-0.2, 100, false, true, new SineEase(), isRelative: true);
                    mapElementCommandToDoc[uie].RemoteExecuteCommand(mapElementCommandToName[uie]);
                    return true;
                }

                if (mapUriToEmbeddeds.ContainsKey(uri))
                {
                    var listEmbeeded = mapUriToEmbeddeds[uri];
                    if (listEmbeeded.Count > 0)
                    {
                        foreach(var embedded in listEmbeeded)
                        {
                            if (embedded.CheckRemoteCommands(uie))
                                return true;
                        }
                    }
                }
            }

            return false;
        }

        bool CanSimulateEvent(UIElement element, out UIElement finalElement, bool recursive = false, UIElement parentElement = null)
        {
            finalElement = element;
            if (element == null || !(element as FrameworkElement).IsHitTestVisible)
                return false;

            try
            {
                var autoElement = UIElementAutomationPeer.CreatePeerForElement(element) as UIElementAutomationPeer;
                if (autoElement == null && element is ContentControl && (element as ContentControl).Content is UIElement)
                {
                    element = (element as ContentControl).Content as UIElement;
                    autoElement = UIElementAutomationPeer.CreatePeerForElement(element) as UIElementAutomationPeer;
                }
                if (autoElement != null)
                {
                    element.UpdateLayout();
                    var rect = autoElement.GetBoundingRectangle();
                    if (rect.Width == 0 && (element as FrameworkElement).ActualWidth == 0 || 
                        rect.Height == 0 && (element as FrameworkElement).ActualHeight == 0 || !autoElement.IsEnabled())
                        return false;

                    if (element is ComboBox && element.IsHitTestVisible)
                        return true;
                    if (element is ListBox && element.IsHitTestVisible)
                        return true;

                    var invokeProv = autoElement.GetPattern(PatternInterface.Invoke) as IInvokeProvider;
                    if (invokeProv != null)
                        return true;

                    var toggleProv = autoElement.GetPattern(PatternInterface.Toggle) as IToggleProvider;
                    if (toggleProv != null)
                        return true;

                    var expandProv = autoElement.GetPattern(PatternInterface.ExpandCollapse) as IExpandCollapseProvider;
                    if (expandProv != null)
                        return true;

                    var valueProv = autoElement.GetPattern(PatternInterface.Value) as IValueProvider;
                    if (valueProv != null && !valueProv.IsReadOnly)
                        return true;

                    var rangevalueProv = autoElement.GetPattern(PatternInterface.Scroll) as IScrollProvider;
                    if (rangevalueProv != null)
                        return true;

                    var textProv = autoElement.GetPattern(PatternInterface.Text) as ITextProvider;
                    if (textProv != null)
                        return true;
                }
                else
                {
                    if (parentElement is IRemotelyCommandable && element.IsHitTestVisible)
                        return (parentElement as IRemotelyCommandable).CheckRemoteCommands(element, false);
                    if (element.IsHitTestVisible && mapElementCommandToName.ContainsKey(element))
                        return true;
                }
            }
            catch (Exception ex)
            { }

            if (recursive)
            {
                var elements = element.GetChildrenOfType<FrameworkElement>().ToList();
                var list = (from c in elements
                            where c.IsHitTestVisible
                            select c).ToList();

                foreach (var uie in elements)
                {
                    if (CanSimulateEvent(uie, out finalElement, false, element))
                        return true;
                }
            }

            return false;
        }

        PixelFormat GetCurrentImageFormat()
        {
            if (LowResolution)
                return PixelFormats.Gray2;
            return PixelFormats.Default;
        }

        int currentSkip = 0;
        void timer_Tick(object sender, EventArgs e)
        {
            lock (lockObject)
            {
                if (bDisposed)
                    return;

                if (DisableStaticOptimization)
                {
                    foreach (var guid in mapGuidToElementUpdateOnly.Keys)
                    {
                        var ret = FrameworkElementToBase64(mapGuidToElementUpdateOnly[guid], null, GetCurrentImageFormat(), mapGuidToElementPos[guid].Size);
                        //if (listUpdatingImages.Contains(guid))
                        //    listUpdatingImages.Remove(guid);
                        if (/*!String.IsNullOrEmpty(ret) && */(!mapGuidToBase64Element.ContainsKey(guid) || ret != mapGuidToBase64Element[guid]))
                        {
#if DEBUGTRACE
                            System.Diagnostics.Debug.WriteLine(String.Format("Object {0} Changed",
                                                fe.Name));
#endif
                            // tempMap.Add(guid, ret);
                            if (mapGuidToBase64Element.ContainsKey(guid))
                                mapGuidToBase64Element.Remove(guid);
                            mapGuidToBase64Element.Add(guid, ret);
                            listChanged.Add(guid);
                        }
                    }
                }

                    // var tempMap = new Dictionary<Guid, String>();
                foreach (var guid in mapGuidToElement.Keys.Skip(currentSkip).Take(RefreshPollingTimeCount))
                {
                    if (listChanged.Contains(guid) || listChangedPending.Contains(guid)/* ||
                        listUpdatingImages.Contains(guid)*/)
                        continue;
                    if (!mapGuidToParentCanvas.ContainsKey(guid))
                        continue;

                    // var point = new Point(Canvas.GetLeft(mapGuidToElement[guid]),
                    //                       Canvas.GetTop(mapGuidToElement[guid]));
                    Rect rect = Rect.Empty;
                    var fe = mapGuidToElement[guid] as FrameworkElement;
                    if (fe.IsVisible)
                    {
	                    // var rect = new Rect(point.X, point.Y, fe.ActualWidth, fe.ActualHeight);
	                    // var rect = VisualTreeHelper.GetDescendantBounds(fe);
	
	                    UIElement controlViewport = mapGuidToParentCanvas[guid];
	                    if (mapGuidToParentCanvas[guid].Parent is Viewbox)
	                        controlViewport = (UIElement)mapGuidToParentCanvas[guid].Parent;
	                    rect = DependencyObjectExtensions.CalculateBoundRect(new List<UIElement>() { fe }, controlViewport);
                    }

                    bool bChanged = rect != mapGuidToElementPos[guid];
                    if (bChanged)
                    {
#if DEBUGTRACE
                        System.Diagnostics.Debug.WriteLine(String.Format("Object {0} Changed its position from {1} to {2}",
                                            fe.Name, mapGuidToElementPos[guid], rect));
#endif

                        mapGuidToElementPos.Remove(guid);
                        mapGuidToElementPos.Add(guid, rect);
                        if (!listStatusChanged.Contains(guid))
                            listStatusChanged.Add(guid);
                    }
                    // else
                    if (!DisableStaticOptimization)
                    {
                        //if (!listUpdatingImages.Contains(guid))
                        //    listUpdatingImages.Add(guid);
                        var ret = FrameworkElementToBase64(mapGuidToElement[guid], mapGuidToParentCanvas[guid], GetCurrentImageFormat());
                        //if (listUpdatingImages.Contains(guid))
                        //    listUpdatingImages.Remove(guid);
                        if (/*!String.IsNullOrEmpty(ret) && */(!mapGuidToBase64Element.ContainsKey(guid) || ret != mapGuidToBase64Element[guid]))
                        {
#if DEBUGTRACE
                            System.Diagnostics.Debug.WriteLine(String.Format("Object {0} Changed",
                                                fe.Name));
#endif
                            // tempMap.Add(guid, ret);
                            if (mapGuidToBase64Element.ContainsKey(guid))
                                mapGuidToBase64Element.Remove(guid);
                            mapGuidToBase64Element.Add(guid, ret);
                            listChanged.Add(guid);
                        }
                    }
                }

                //foreach (var el in listChanged)
                //{
                //    mapGuidToBase64Element.Remove(el);
                //    if (tempMap.ContainsKey(el))
                //        mapGuidToBase64Element.Add(el, tempMap[el]);
                //}

                if (listChanged.Count > 0)
                    OnPendingChanges();
                if (listStatusChanged.Count > 0)
                    OnPendingStatusChanges();
            }

                currentSkip += RefreshPollingTimeCount;
                if (currentSkip >= mapGuidToElement.Count)
                {
                    currentSkip = 0;

                    // Flush the dispatcher queue
                    gridContainer.Dispatcher.Invoke(DispatcherPriority.SystemIdle, new Action(() => { }));

                    //if (ServerType == ServerType.HTML5)
                    {
                        GC.Collect();
                        //GC.WaitForPendingFinalizers();
                        //GC.Collect();
                    }

                    System.Windows.Input.CommandManager.InvalidateRequerySuggested();
                }
        }

        String FrameworkElementToBase64(UIElement c, UIElement parent, PixelFormat format)
        {
            return FrameworkElementToBase64(c, parent, format, Size.Empty);
        }

        String FrameworkElementToBase64(UIElement c, UIElement parent, PixelFormat format, Size renderSize)
        {
            if (c.RenderSize.Width == 0 || c.RenderSize.Height == 0)
                return String.Empty;

            // lock (lockStaticObject)
            lockSemaphoreRenderingPipeLine.WaitOne();
            try
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    // RenderOptions.SetBitmapScalingMode(c, BitmapScalingMode.LowQuality);
                    try
                    {
                        Rect renderRect = Rect.Empty;
                        Point ptoffset = new Point(0, 0);
                        if (parent == null)
                        {
                            renderRect = VisualTreeHelper.GetDescendantBounds(c);
                            if (renderSize != Size.Empty)
                            {
                                renderRect.Width = renderSize.Width;
                                renderRect.Height = renderSize.Height;
                            }
                        }
                        else
                        {
                            renderRect = DependencyObjectExtensions.CalculateBoundRect(new List<UIElement>() { c },
                                                                        parent);
                            var rect = VisualTreeHelper.GetDescendantBounds(c);
                            if (c is Shape)
                            {
                                var shape = c as Shape;
                                rect.Inflate(-(shape.StrokeThickness / 2), -(shape.StrokeThickness / 2));
                            }

                            var x = (renderRect.Width - rect.Width) / 2;
                            var y = (renderRect.Height - rect.Height) / 2;
                            ptoffset = new Point(x, y);
                        }

                        // Temporarily add a PresentationSource if none exists
                        // using (var temporaryPresentationSource = new HwndSource(new HwndSourceParameters()))
                        {
                            //var rtb = new RenderTargetBitmap((int)renderRect.Width,
                            //                                (int)renderRect.Height, 96, 96,
                            //                                PixelFormats.Pbgra32);
                            try
                            {
                                // var ptoffset = new Point(-renderRect.X, -renderRect.Y);
                                (c as FrameworkElement).ApplyTemplate();
                                c.ClipToBounds = false;
                                c.Measure(c.RenderSize); //Important
                                c.Arrange(new Rect(ptoffset, c.RenderSize)); //Important

                                var rtb = new RenderTargetBitmap((int)Math.Ceiling(renderRect.Width),
                                                                (int)Math.Ceiling(renderRect.Height), 
                                                                96, 96, PixelFormats.Pbgra32);

                                //(c as FrameworkElement).ApplyTemplate();
                                // c.UpdateLayout();
                                rtb.Render(c);
                                rtb.Freeze();

                                BitmapSource bmpSource = rtb;
                                if (format != PixelFormats.Pbgra32 && format != PixelFormats.Default)
                                {
                                    var newFormatedBitmapSource = new FormatConvertedBitmap();

                                    // BitmapSource objects like FormatConvertedBitmap can only have their properties 
                                    // changed within a BeginInit/EndInit block.
                                    newFormatedBitmapSource.BeginInit();

                                    // Use the BitmapSource object defined above as the source for this new  
                                    // BitmapSource (chain the BitmapSource objects together).
                                    newFormatedBitmapSource.Source = rtb;

                                    // Set the DestinationFormat to the palletized pixel format of Indexed1.
                                    newFormatedBitmapSource.DestinationFormat = PixelFormats.BlackWhite;
                                    newFormatedBitmapSource.EndInit();
                                    bmpSource = newFormatedBitmapSource;
                                }

                                var encoder = new PngBitmapEncoder();
                                encoder.Frames.Add(BitmapFrame.Create(bmpSource));
                                encoder.Save(ms);

#if DEBUG
                                //RenderTargetBitmap rtbdisk = new RenderTargetBitmap((int)renderRect.Width,
                                //                                    (int)renderRect.Height, 96, 96, PixelFormats.Pbgra32);
                                //c.Measure(c.RenderSize); //Important
                                //c.Arrange(new Rect(ptoffset, c.RenderSize)); //Important
                                //rtbdisk.Render(c);
                                //var encoderdisk = new PngBitmapEncoder();
                                //encoderdisk.Frames.Add(BitmapFrame.Create(rtbdisk));

                                ////var encoderdisk = new PngBitmapEncoder();
                                ////encoderdisk.Frames.Add(BitmapFrame.Create(bmpSource));
                                //using (Stream fsx = File.Create(@"c:\temp\lastimage.png"))
                                //{
                                //    encoderdisk.Save(fsx);
                                //}
#endif

                                byte[] imageBytes = ms.ToArray();
                                c.Arrange(new Rect(new Point(Canvas.GetLeft(c), Canvas.GetTop(c)), c.RenderSize)); //Important
                                rtb.Clear();
                                rtb = null;

                                return Convert.ToBase64String(imageBytes);
                            }
                            catch (Exception ex)
                            {
                                return String.Empty;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        return String.Empty;
                    }
                }
            }
            finally
            {
                lockSemaphoreRenderingPipeLine.Release();
            }
        }

        public String SendDataValueProvider(Uri uri, Point pt, String value)
        {
            var ret = String.Empty;
            lastTimeUsed = DateTime.UtcNow;
            EnsureThread();
            if (thread == null || bDisposed)
                return ret;
            dispatcher.Dispatcher.InvokeIfRequired(() =>
            {
                lock (lockObject)
                {
                    if (mapUriToCanvas.ContainsKey(uri))
                    {
                        var canvas = mapUriToCanvas[uri];
                        // var pnt = canvas.TranslatePoint(pt, canvas);
                        UIElement controlViewport = canvas;
                        if (canvas.Parent is Viewbox)
                            controlViewport = (UIElement)canvas.Parent;

                        var pnt = controlViewport == canvas ? canvas.PointToScreen(pt) : ((canvas.Parent as Viewbox).Parent as Visual).PointToScreen(pt);

                        for (int i = canvas.Children.Count - 1; i >= 0; --i)
                        {
                            UIElement uie = canvas.Children[i];
                            var original = uie;

                            //Rect itemRect = VisualTreeHelper.GetDescendantBounds(uie);
                            //Rect itemBounds = uie.TransformToAncestor(canvas).TransformBounds(itemRect);

                            if (!mapElementToGuid.ContainsKey(uie) || !mapGuidToElementPos.ContainsKey(mapElementToGuid[uie]))
                                continue;

                            Rect itemBounds = mapGuidToElementPos[mapElementToGuid[uie]];
                            if (itemBounds.Contains(pt))
                            {
                                var content = uie;
                                if (uie is ContentControl && !(uie is UserControl) && (uie as ContentControl).Content is FrameworkElement)
                                    content = (uie as ContentControl).Content as FrameworkElement;
                                if (!content.IsHitTestVisible)
                                    continue;

                                var elements = uie.GetVisualChildrenOfType<FrameworkElement>().ToList();
                                UIElement finalElement;
                                var list = (from c in elements
                                            where c.IsEnabled && DependencyObjectExtensions.CalculateBoundRect(new List<UIElement>() { c },
                                                                            controlViewport).Contains(pt) && CanSimulateEvent(c, out finalElement, false, uie)
                                            select c).ToList();

                                if (list.Count == 1)
                                    uie = list[0];
                                else if (list.Count > 1)
                                {
                                    var index = 0;
                                    list.ForEach(el =>
                                    {
                                        var currentIndex = elements.IndexOf(el);
                                        if (el.IsEnabled && el.IsVisible && currentIndex > index && (el.TemplatedParent == null || !list.Contains(el.TemplatedParent)))
                                            index = currentIndex;
                                    });
                                    uie = elements[index];
                                }

                                if (original is ComboBox)
                                    uie = original;
                                if (!uie.IsEnabled || !uie.IsVisible || !content.IsEnabled || !content.IsVisible)
                                    break;

                                var rect = DependencyObjectExtensions.CalculateBoundRect(new List<UIElement>() { uie },
                                                                            controlViewport);
                                var res = new PointHitTestResult(uie, pt);
                                ret = SetValueProvider(res, value, pnt);
                                if (ret != null)
                                {
                                    ResetValueProviderSelection(res, pnt);

                                    if (!DisableStaticOptimization)
                                    {
                                        var guid = mapElementToGuid[canvas.Children[i]];
                                        if (mapGuidToBase64Element.ContainsKey(guid))
                                            mapGuidToBase64Element.Remove(guid);
                                        if (!listChanged.Contains(guid) && !listChangedPending.Contains(guid))
                                        {
                                            listChanged.Add(guid);
                                            OnPendingChanges();
                                        }
                                    }
                                    break;
                                }
                                else if (uie != original)
                                {
                                    rect = DependencyObjectExtensions.CalculateBoundRect(new List<UIElement>() { original },
                                                                                controlViewport);
                                    res = new PointHitTestResult(original, pt);
                                    ret = SetValueProvider(res, value, pnt);
                                    if (ret != null)
                                    {
                                        ResetValueProviderSelection(res, pnt);

                                        if (!DisableStaticOptimization)
                                        {
                                            var guid = mapElementToGuid[canvas.Children[i]];
                                            if (mapGuidToBase64Element.ContainsKey(guid))
                                                mapGuidToBase64Element.Remove(guid);
                                            if (!listChanged.Contains(guid) && !listChangedPending.Contains(guid))
                                            {
                                                listChanged.Add(guid);
                                                OnPendingChanges();
                                            }
                                        }
                                        break;
                                    }
                                }

                                if (ret == null)
                                {
                                    var listvalue = (from c in elements
                                                     where c.IsEnabled && DependencyObjectExtensions.CalculateBoundRect(new List<UIElement>() { c },
                                                                                controlViewport).Contains(pt) && HasValueProvider(c, pnt)
                                                     select c).ToList();

                                    if (listvalue.Count > 0)
                                    {
                                        uie = listvalue[0];

                                        rect = DependencyObjectExtensions.CalculateBoundRect(new List<UIElement>() { uie }, controlViewport);
                                        res = new PointHitTestResult(uie, pt);
                                        ret = SetValueProvider(res, value, pnt);
                                        if (ret != null)
                                        {
                                            ResetValueProviderSelection(res, pnt);

                                            if (!DisableStaticOptimization)
                                            {
                                                var guid = mapElementToGuid[canvas.Children[i]];
                                                if (mapGuidToBase64Element.ContainsKey(guid))
                                                    mapGuidToBase64Element.Remove(guid);
                                                if (!listChanged.Contains(guid) && !listChangedPending.Contains(guid))
                                                {
                                                    listChanged.Add(guid);
                                                    OnPendingChanges();
                                                }
                                            }
                                        }
                                    }
                                }
                                break;
                            }
                        }
                    }
                }
            });

            if (ret == null)
                ret = String.Empty;
            return ret;
        }

        public Status SimulateEvent(Uri uri, Point pt, MouseButton action, bool bIsDown)
        {
            Status ret = null;
            lastTimeUsed = DateTime.UtcNow;
            EnsureThread();
            if (thread == null || uri == null || bDisposed)
                return ret;
            dispatcher.Dispatcher.InvokeIfRequired(() =>
            {
                lock (lockObject)
                {
                    if (mapUriToCanvas.ContainsKey(uri))
                    {
                        var canvas = mapUriToCanvas[uri];
                        // var pnt = canvas.TranslatePoint(pt, canvas);
                        UIElement controlViewport = canvas;
                        if (canvas.Parent is Viewbox)
                            controlViewport = (UIElement)canvas.Parent;

                        var pnt = controlViewport == canvas ? canvas.PointToScreen(pt) : ((canvas.Parent as Viewbox).Parent as Visual).PointToScreen(pt);

                        for (int i = canvas.Children.Count - 1; i >= 0; --i)
                        {
                            UIElement uie = canvas.Children[i];
                            var original = uie;

                            //Rect itemRect = VisualTreeHelper.GetDescendantBounds(uie);
                            //Rect itemBounds = uie.TransformToAncestor(canvas).TransformBounds(itemRect);

                            if (uie == null || !mapElementToGuid.ContainsKey(uie) || !mapGuidToElementPos.ContainsKey(mapElementToGuid[uie]))
                                continue;

                            Rect itemBounds = mapGuidToElementPos[mapElementToGuid[uie]];
                            if (itemBounds.Contains(pt))
                            {
                                var content = uie;
                                if (uie is ContentControl && !(uie is UserControl) && (uie as ContentControl).Content is FrameworkElement)
                                    content = (uie as ContentControl).Content as FrameworkElement;
                                if (!content.IsHitTestVisible)
                                    continue;

                                var elements = uie.GetVisualChildrenOfType<FrameworkElement>().ToList();
                                UIElement finalElement;
                                var list = (from c in elements
                                            where DependencyObjectExtensions.CalculateBoundRect(new List<UIElement>() { c },
                                                                            controlViewport).Contains(pt) && CanSimulateEvent(c, out finalElement, false, uie)
                                            select c).ToList();

                                if (list.Count == 1)
                                    uie = list[0];
                                else if (list.Count > 1)
                                {
                                    var originalCommandable = original as IRemotelyCommandable;
                                    var index = 0;
                                    foreach (var el in list)
                                    {
                                        var currentIndex = elements.IndexOf(el);
                                        if (el.IsEnabled && el.IsVisible && currentIndex > index && (el.TemplatedParent == null || !list.Contains(el.TemplatedParent)))
                                        {
                                            index = currentIndex;
                                            if (originalCommandable != null && originalCommandable.CheckRemoteCommands(el, false))
                                                break;
                                        }
                                    }
                                    uie = elements[index];
                                }

                                if (original != null && mapElementCommandToName.ContainsKey(original))
                                    uie = original;
                                else if (original is ComboBox)
                                    uie = original;
                                if (!uie.IsEnabled || !uie.IsVisible || !content.IsEnabled || !content.IsVisible)
                                    break;

                                var rect = DependencyObjectExtensions.CalculateBoundRect(new List<UIElement>() { uie },
                                                                            controlViewport);

                                var res = new PointHitTestResult(uie, pt);
                                bool bStopPropagation = false;
                                var v = GetValueProvider(res, pnt, bIsDown, ref bStopPropagation);
                                if (v != null)
                                {
                                    List<String> sel = null;
                                    if (v.StartsWith(ListSerializationHelper.tagListSelection))
                                    {
                                        v = v.Replace(ListSerializationHelper.tagListSelection, "");
                                        sel = v.Split(ListSerializationHelper.tagSelectionSeparator).ToList();
                                    }
                                    ret = new Status() { hasValueProvider = true, X = (int)pt.X, Y = (int)pt.Y, value = v, selection = sel };

                                    if (uie != null && mapElementToGuid.ContainsKey(uie) && mapGuidToElementStatus.ContainsKey(mapElementToGuid[uie]))
                                    {
                                        var sts = mapGuidToElementStatus[mapElementToGuid[uie]];
                                        sts.hasValueProvider = ret.hasValueProvider;
                                        sts.X = ret.X;
                                        sts.Y = ret.Y;
                                        sts.value = ret.value;
                                        sts.selection = ret.selection;
                                    }
                                }
                                if (bStopPropagation || SimulateEvent(uri, res, action, bIsDown, rect, pt))
                                {
                                    //var guid = mapElementToGuid[canvas.Children[i]];
                                    //if (mapGuidToBase64Element.ContainsKey(guid))
                                    //    mapGuidToBase64Element.Remove(guid);
                                    //if (!listChanged.Contains(guid) && !listChangedPending.Contains(guid))
                                    //{
                                    //    listChanged.Add(guid);
                                    //    OnPendingChanges();
                                    //}
                                    break;
                                }
                                else if (uie != original)
                                {
                                    rect = DependencyObjectExtensions.CalculateBoundRect(new List<UIElement>() { original },
                                                                                controlViewport);
                                    res = new PointHitTestResult(original, pt);
                                    bStopPropagation = false;
                                    v = GetValueProvider(res, pnt, bIsDown, ref bStopPropagation);
                                    if (v != null)
                                    {
                                        List<String> sel = null;
                                        if (v.StartsWith(ListSerializationHelper.tagListSelection))
                                        {
                                            v = v.Replace(ListSerializationHelper.tagListSelection, "");
                                            sel = v.Split(ListSerializationHelper.tagSelectionSeparator).ToList();
                                        }
                                        ret = new Status() { hasValueProvider = true, X = (int)pt.X, Y = (int)pt.Y, value = v, selection = sel };

                                        if (original != null && mapElementToGuid.ContainsKey(original) && mapGuidToElementStatus.ContainsKey(mapElementToGuid[original]))
                                        {
                                            var sts = mapGuidToElementStatus[mapElementToGuid[original]];
                                            sts.hasValueProvider = ret.hasValueProvider;
                                            sts.X = ret.X;
                                            sts.Y = ret.Y;
                                            sts.value = ret.value;
                                            sts.selection = ret.selection;
                                        }
                                    }
                                    if (bStopPropagation || SimulateEvent(uri, res, action, bIsDown, rect, pt))
                                    {
                                        //var guid = mapElementToGuid[canvas.Children[i]];
                                        //if (mapGuidToBase64Element.ContainsKey(guid))
                                        //    mapGuidToBase64Element.Remove(guid);
                                        //if (!listChanged.Contains(guid) && !listChangedPending.Contains(guid))
                                        //{
                                        //    listChanged.Add(guid);
                                        //    OnPendingChanges();
                                        //}
                                        break;
                                    }
                                }

                                if (ret == null)
                                {
                                    var listvalue = (from c in elements
                                                     where c.IsEnabled && DependencyObjectExtensions.CalculateBoundRect(new List<UIElement>() { c },
                                                                         controlViewport).Contains(pt) && HasValueProvider(c, pnt)
                                                     select c).ToList();

                                    if (listvalue.Count > 0)
                                    {
                                        uie = listvalue[0];

                                        rect = DependencyObjectExtensions.CalculateBoundRect(new List<UIElement>() { uie }, controlViewport);
                                        res = new PointHitTestResult(uie, pt);
                                        bStopPropagation = false;
                                        v = GetValueProvider(res, pnt, bIsDown, ref bStopPropagation);
                                        if (v != null)
                                        {
                                            List<String> sel = null;
                                            if (v.StartsWith(ListSerializationHelper.tagListSelection))
                                            {
                                                v = v.Replace(ListSerializationHelper.tagListSelection, "");
                                                sel = v.Split(ListSerializationHelper.tagSelectionSeparator).ToList();
                                            }
                                            ret = new Status() { hasValueProvider = true, X = (int)pt.X, Y = (int)pt.Y, value = v, selection = sel };

                                            if (uie != null && mapElementToGuid.ContainsKey(uie) && mapGuidToElementStatus.ContainsKey(mapElementToGuid[uie]))
                                            {
                                                var sts = mapGuidToElementStatus[mapElementToGuid[uie]];
                                                sts.hasValueProvider = ret.hasValueProvider;
                                                sts.X = ret.X;
                                                sts.Y = ret.Y;
                                                sts.value = ret.value;
                                                sts.selection = ret.selection;
                                            }
                                        }
                                    }
                                }
                                break;
                            }
                        }
                    }
                }
            });

            return ret;
        }

        public bool IsValid()
        {
            return thread != null;
        }

        public bool IsResizable(Uri uri)
        {
            lock (lockObject)
            {
                if (mapUriToDocuments.ContainsKey(uri))
                    return IsResizable(mapUriToDocuments[uri]);
            }

            return false;
        }

        bool IsResizable(ScreenDocument document)
        {
            return ServerType == ServerType.AppServer || (ServerType == ServerType.HTML5 && document.FitInWindow);
        }

        double scaleX = 1.0;
        double scaleY = 1.0;
        /*
        Rect ScaleRect(Rect rect)
        {
            return new Rect(rect.Left * scaleX, rect.Top * scaleY, rect.Width * scaleX, rect.Height * scaleY);
        }

        Point ScalePoint(Point pt)
        {
            return new Point(pt.X * scaleX, pt.Y * scaleY);
        }
        */

        void UpdateCanvasScale(Canvas canvas, ScreenDocument doc, List<string> listZoomVisibilityEntities,
                                    int width, int height)
        {
            canvas.RenderTransform = null;
            if (ServerType == ServerType.HTML5 && canvas.Width > 0 && canvas.Height > 0 &&
                width > 0 && height > 0)
            {
                var t = new ScaleTransform();
                t.ScaleX = width / canvas.Width;
                t.ScaleY = height / canvas.Height;
                //if (doc.FitInWindow)
                //    canvas.RenderTransform = t;
                //else
                //    canvas.RenderTransform = null;

                UpdateZoomVisibility(canvas, doc, listZoomVisibilityEntities, t.ScaleX, t.ScaleY);
            }

            scaleX = scaleY = 1;

            if (!IsResizable(doc))
                return;

            double viewboxWidth = width;
            double viewboxHeight = height;
            if (canvas.Width > 0 && canvas.Height > 0 &&
                width > 0 && height > 0)
            {
                if (doc.KeepAspectRatio && height < width) //factorX must be limited by factorY in order to keep aspect ratio
                    scaleX = height / canvas.Height;
                else
                    scaleX = width / canvas.Width;
                if (doc.KeepAspectRatio && width < height) //factorY must be limited by factorX in order to keep aspect ratio
                    scaleY = width / canvas.Width;
                else
                    scaleY = height / canvas.Height;

                if (doc.KeepAspectRatio)
                {
                    var originalFactor = canvas.Width / canvas.Height;
                    var currentFactor = width / height;
                    if (currentFactor > originalFactor)
                        viewboxWidth = height * originalFactor;
                    else
                        viewboxHeight = width / originalFactor;
                }

            }

            var viewbox = canvas.Parent as Viewbox;
            viewbox.Width = viewboxWidth;
            viewbox.Height = viewboxHeight;
            viewbox.UpdateLayout();
        }

        public void OnResize(Uri uri, int width, int height)
        {
            lastTimeUsed = DateTime.UtcNow;
            var ret = String.Empty;
            EnsureThread();
            if (thread == null || bDisposed)
                return;

            dispatcher.Dispatcher.InvokeIfRequired(() =>
            {
                lock (lockObject)
                {
                    List<String> listZoomVisibilityEntities = null;
                    if (mapZoomVisibilityEntities.ContainsKey(uri))
                        listZoomVisibilityEntities = mapZoomVisibilityEntities[uri];

                    if (mapUriToCanvas.ContainsKey(uri))
                    {
                        var canvas = mapUriToCanvas[uri];
                        var doc = mapUriToDocuments[uri];

                        UpdateCanvasScale(canvas, doc, listZoomVisibilityEntities, width, height);

                        if (DisableStaticOptimization)
                        {
                            var id = doc.IdDocument;
                            var rc = new Rect(0, 0, canvas.Width * scaleX, canvas.Height * scaleY);
                            mapGuidToElementPos.Remove(id);
                            mapGuidToElementPos.Add(id, rc);
                        }

                        foreach (var guid in mapGuidToElement.Keys)
                        {
                            if (!mapGuidToParentCanvas.ContainsKey(guid))
                                continue;

                            var fe = mapGuidToElement[guid] as FrameworkElement;

                            var controlViewport = (UIElement)(IsResizable(doc) ? mapGuidToParentCanvas[guid].Parent : mapGuidToParentCanvas[guid]);
                            var rect = DependencyObjectExtensions.CalculateBoundRect(new List<UIElement>() { fe }, controlViewport);

                            mapGuidToElementPos.Remove(guid);
                            mapGuidToElementPos.Add(guid, rect);
                            if (!listStatusChanged.Contains(guid) && !listStatusChangedPending.Contains(guid))
                                listStatusChanged.Add(guid);

                            if (!DisableStaticOptimization)
                            {
                                if (mapGuidToBase64Element.ContainsKey(guid))
                                    mapGuidToBase64Element.Remove(guid);
                                if (!listChanged.Contains(guid) && !listChangedPending.Contains(guid))
                                    listChanged.Add(guid);
                            }
                        }

                        if (listChanged.Count > 0)
                            OnPendingChanges();
                        if (listStatusChanged.Count > 0)
                            OnPendingStatusChanges();

                        if (mapUriToBackground.ContainsKey(uri))
                            mapUriToBackground.Remove(uri);
                    }
                }
            });
        }

        public String GetBackground(Uri uri)
        {
            lastTimeUsed = DateTime.UtcNow;
            var ret = String.Empty;
            EnsureThread();
            if (thread == null || bDisposed)
                return Properties.Resources.NoValidSessionBase64;

            lock (lockObject)
            {
                if (mapUriToBackground.ContainsKey(uri))
                    return mapUriToBackground[uri];
            }

            // Flushing the Dispatcher queue down to DispatcherPriority.Render
            dispatcher.Dispatcher.Invoke(DispatcherPriority.Loaded, new Action(() => { }));

            dispatcher.Dispatcher.InvokeIfRequired(() =>
            {
                lock (lockObject)
                {
                    if (mapUriToBackground.ContainsKey(uri))
                        ret = mapUriToBackground[uri];
                    else if (mapUriToCanvas.ContainsKey(uri))
                    {
                        var canvas = mapUriToCanvas[uri];

                        Brush background = canvas.Background;
                        if (mapUriToCanvasBackground.ContainsKey(uri))
                            background = mapUriToCanvasBackground[uri];

                        var rect = new Canvas
                        {
                            Background = background,
                            Width = canvas.Width * scaleX,
                            Height = canvas.Height * scaleY
                        };
                        // ResourceDictionaryExtensions.AddCommonResources(rect, commonFolder);
                        gridContainer.Children.Add(rect);
                        rect.CacheMode = new BitmapCache() { EnableClearType = true };

                        var size = new Size(canvas.Width * scaleX, canvas.Height * scaleY);
                        rect.Measure(size);
                        rect.Arrange(new Rect(0, 0, size.Width, size.Height));

                        if (mapUriToBackgroundChildren.ContainsKey(uri))
                        {
                            mapUriToBackgroundChildren[uri].ForEach(c =>
                            {
                                rect.Children.Add(c);
                            });

                            //mapUriToBackgroundChildren[uri].Clear();
                            //mapUriToBackgroundChildren.Remove(uri);
                        }

                        ret = FrameworkElementToBase64(rect, null, GetCurrentImageFormat(), size);
                        mapUriToBackground.Add(uri, ret);

                        gridContainer.Children.Remove(rect);
                        rect.Dispose();
                    }
                }
            });

            return ret;
        }

        // bool bEnableLayoutUpdates = false;
        public void EnableUpdates(bool bEnable)
        {
            // bEnableLayoutUpdates = bEnable;
            if (timer != null)
                timer.IsEnabled = bEnable == true;
        }

        public void SetZoomVisibilityItems(Uri uri, double zoomLevel)
        {
            List<String> listZoomVisibilityEntities = null;
            lock (lockObject)
            {
                if (!mapZoomVisibilityEntities.ContainsKey(uri))
                    return;
                listZoomVisibilityEntities = mapZoomVisibilityEntities[uri];
                if (listZoomVisibilityEntities.Count == 0)
                    return;
            }

            lastTimeUsed = DateTime.UtcNow;
            EnsureThread();
            if (thread == null || bDisposed)
                return;
            dispatcher.Dispatcher.InvokeIfRequired(() =>
            {
                lock (lockObject)
                {
                    var doc = mapUriToDocuments[uri];
                    var canvas = mapUriToCanvas[uri];
                    UpdateZoomVisibility(canvas, doc, listZoomVisibilityEntities, zoomLevel, zoomLevel);
                }
            });
        }

        private void UpdateZoomVisibility(Canvas canvas, ScreenDocument doc, List<string> listZoomVisibilityEntities,
                                    double zoomLevelX, double zoomLevelY)
        {
            if (listZoomVisibilityEntities == null || listZoomVisibilityEntities.Count == 0)
                return;

            listZoomVisibilityEntities.ForEach(name =>
            {
                var child = doc.FindInnerControl(canvas, name);
                if (child != null)
                {
                    if (zoomLevelX > doc.MapScreenEntities[name].ZoomLevelVisibilityX &&
                        zoomLevelY > doc.MapScreenEntities[name].ZoomLevelVisibilityY)
                    {
                        if (!doc.MapScreenEntities[name].IsInvisibleForSecurity())
                        {
                            if (child.Visibility == Visibility.Collapsed)
                            {
                                child.Visibility = Visibility.Visible;
                            }
                        }
                    }
                    else
                    {
                        if (child.Visibility == Visibility.Visible)
                        {
                            child.Visibility = Visibility.Collapsed;
                        }
                    }
                }
            });
        }

        public IList<Guid> OpenUri(Uri uri, ref Size sizeScreen, int nWidth = 0, int nHeight = 0,
                                    FileSystemProviderBase fileSystemProvider = null, IDocument parent = null,
                                    String historiansettings = null,
                                    String eventsettings = null,
                                    String serverentitysettings = null,
                                    String schedulersettings = null, String parameter = null,
                                    int clientTimezoneOffset = 0)
        {
            lastTimeUsed = DateTime.UtcNow;
            var ret = new List<Guid>();
            EnsureThread();
            if (thread == null || bDisposed)
                return ret;
            var retSize = new Size();

            dispatcher.Dispatcher.InvokeIfRequired(() =>
            {
                lock (lockStaticObject)
                {
                    var listAlarm = new List<FrameworkElement>();
                    var listRecipe = new List<FrameworkElement>();
                    var listConnectionString = new List<FrameworkElement>();
                    var listEventConnectionString = new List<FrameworkElement>();
                    var listScheduler = new List<FrameworkElement>();
                    if (mapUriToCanvas.ContainsKey(uri))
                    {
                        var canvas = mapUriToCanvas[uri];
                        foreach (UIElement uie in canvas.Children)
                        {
                            ret.Add(mapElementToGuid[uie]);
                        }
                    }
                    else
                    {
                        var path = uri.GetPathString();
                        var Document = ScreenDocument.FromFile(path, parent);
                        if (Document != null)
                        {
                            Document.Parent = parent;
                            Document.InRuntime = true;
                            Document.SetIsBlindServer();

                            Document.SetParameterFile(parameter);

                            LoadData(GetUniqueTitle(Document), Document);

                            if (listOpenedUri.Contains(path))
                                listOpenedUri.Remove(path);
                            listOpenedUri.Add(path);

                            var canvas = Document.GetCurrentXamlDocument();

                            if (!IsResizable(Document))
                            {
                                if (!gridContainer.Children.Contains(canvas))
                                    gridContainer.Children.Add(canvas);
                            }
                            else {
                                Viewbox viewbox = (from UIElement c in gridContainer.Children where (c as Viewbox)?.Child == canvas select (Viewbox)c).FirstOrDefault();
                                if (viewbox == null)
                                {
                                    viewbox = new Viewbox() { Stretch = Stretch.Fill };
                                    viewbox.Child = canvas;
                                    gridContainer.Children.Add(viewbox);
                                }
                            }

                            // Remove all not visible on client elements
                            var list = (from c in Document.MapScreenEntities/*.AsParallel()*/
                                        where !c.Value.VisibleOnClient
                                        select c.Key).ToList();

                            Parallel.ForEach(Document.MapScreenEntities.Values, c => c.SetIsBlindServer());

                            foreach (var name in list)
                            {
                                var element = Document.FindInnerControl(canvas, name) as FrameworkElement;
                                if (element == null)
                                    continue;
                                if (canvas.Children.Contains(element))
                                    canvas.Children.Remove(element);
                            }

                            var userservice = Document.GetService(typeof(IUFUserEditorManager)) as IUFUserEditorManager;
                            if (userservice == null || userservice.GetEnableUserManager(Document))
                            {
                                var listAccess = (from c in Document.MapScreenEntities/*.AsParallel()*/
                                                  where c.Value.HasAccessControl || c.Value.HasAccessLevelControl
                                                  select c.Key).ToList();

                                foreach (var name in listAccess)
                                {
                                    var element = Document.FindInnerControl(canvas, name) as FrameworkElement;
                                    if (element == null)
                                        continue;
                                    if (!Document.MapScreenEntities[name].HasReadableAccess(AccessMask) ||
                                        !Document.MapScreenEntities[name].HasAccessLevel(Role, AccessLevel))
                                    {
                                        if (canvas.Children.Contains(element))
                                            canvas.Children.Remove(element);
                                        else
                                        {
                                            var pc = element.FindParent<Canvas>();
                                            if (pc != null && pc.Children.Contains(element))
                                                pc.Children.Remove(element);
                                            else
                                            {
                                                pc = element.FindAncestor<Canvas>();
                                                if (pc != null && pc.Children.Contains(element))
                                                    pc.Children.Remove(element);
                                                else
                                                    element.Visibility = Visibility.Collapsed;
                                            }
                                        }
                                    }
                                    else if (!Document.MapScreenEntities[name].HasWritableAccess(AccessMask))
                                        element.IsEnabled = false;
                                }
                            }

                            //var listAlarm = new List<FrameworkElement>();
                            //var listConnectionString = new List<FrameworkElement>();
                            //var listEventConnectionString = new List<FrameworkElement>();
                            //var listScheduler = new List<FrameworkElement>();
                            if (!String.IsNullOrEmpty(historiansettings))
                                historiansettings = RealTimeConnectionManagerViewModel.ReplaceServerRenamedOnDataSource(historiansettings, Document.SessionString);
                            if (!String.IsNullOrEmpty(eventsettings))
                                eventsettings = RealTimeConnectionManagerViewModel.ReplaceServerRenamedOnDataSource(eventsettings, Document.SessionString);
                            ScreenDocument.SetRunningOnServer(canvas, true);
                            ScreenDocument.SetClientTimezoneOffset(canvas, -clientTimezoneOffset);
                            ScreenDocument.SetScreenDocument(canvas, Document);
                            ScreenDocument.SetAccessRole(canvas, Role);
                            ScreenDocument.SetAccessLevel(canvas, AccessLevel);
                            ScreenDocument.SetAccessMask(canvas, AccessMask);

                            Document.GetTagList += Document_GetTagList;
                            Document.GetPrototypeList += Document_GetPrototypeList;
                            Document.GetTagEntityReference += Document_GetTagEntityReference;

                            Document.LoadResources(canvas);
                            Document.SetImagesBaseUri(canvas, true);
                            Document.RefreshEntityStyleBinding(canvas);

                            bool bLoadingSymbols = false;
                            Document.RepositoryItemsLoading += (ob, ev) =>
                            {
                                bLoadingSymbols = true;
                            };
                            Document.RepositoryItemsLoaded += (ob, ev) =>
                            {
                                bLoadingSymbols = false;
                                Document.OnScreenLoaded();
                            };

                            Document.RepositoryItemLoaded += (ob, ev) =>
                            {
                                FrameworkElement fe = ob as FrameworkElement;
                                if (fe != null)
                                {
                                    if (fe is ContentControl && (fe as ContentControl).Content is FrameworkElement && !(fe is UserControl))
                                    {
                                        if (!DisableStaticOptimization)
                                        {
                                            lock (lockObject)
                                            {
                                                if (mapElementToGuid.ContainsKey(fe))
                                                {
                                                    var g = mapElementToGuid[fe];
                                                    if (!listChanged.Contains(g) &&
                                                        !listChangedPending.Contains(g))
                                                        listChanged.Add(g);
                                                }
                                            }
                                        }
                                    }

                                    var localListAlarm = new List<FrameworkElement>();
                                    var localListRecipe = new List<FrameworkElement>();
                                    var localListConnectionString = new List<FrameworkElement>();
                                    var localListEventConnectionString = new List<FrameworkElement>();
                                    var localListScheduler = new List<FrameworkElement>();
                                    ScreenDocument.CheckPreBinding(localListAlarm,
                                        localListConnectionString,
                                        localListEventConnectionString,
                                        localListScheduler, fe, localListRecipe);
                                    if (localListConnectionString.Count > 0)
                                    {
                                        Document.PreBindConnectionStringSource(localListConnectionString, historiansettings);
                                    }
                                    if (localListEventConnectionString.Count > 0)
                                    {
                                        Document.PreBindConnectionStringSource(localListEventConnectionString, eventsettings);
                                    }
                                    if (localListAlarm.Count > 0)
                                    {
                                        Document.PreBindAlarmSource(localListAlarm, serverentitysettings);
                                    }
                                    if (localListScheduler.Count > 0)
                                    {
                                        Document.PreBindSchedulerSource(localListScheduler, schedulersettings);
                                    }
                                    if (localListRecipe.Count > 0)
                                    {
                                        Document.PreBindRecipeSource(localListRecipe, ClientSessionName);
                                    }
                                }
                            };

                            foreach (FrameworkElement uie in canvas.Children)
                                ScreenDocument.CheckPreBinding(listAlarm, listConnectionString, listEventConnectionString, listScheduler, uie, listRecipe);
                            if (listConnectionString.Count > 0)
                            {
                                Document.PreBindConnectionStringSource(listConnectionString, historiansettings);
                            }
                            if (listEventConnectionString.Count > 0)
                            {
                                Document.PreBindConnectionStringSource(listEventConnectionString, eventsettings);
                            }
                            if (listAlarm.Count > 0)
                            {
                                Document.PreBindAlarmSource(listAlarm, serverentitysettings);
                            }
                            if (listScheduler.Count > 0)
                            {
                                Document.PreBindSchedulerSource(listScheduler, schedulersettings);
                            }
                            if (listRecipe.Count > 0)
                            {
                                Document.PreBindRecipeSource(listRecipe, ClientSessionName);
                            }

                            var sessionName = ClientSessionName;
                            if (listAlarm.Count > 0)
                                sessionName = String.Format("{0}-{1}", Guid.NewGuid(), ClientSessionName);
                            Document.PrepareExecution(canvas, sessionName, Document.MapScreenEntities.Keys.ToList());
                            Document.UpdateRepositoryItems(gridContainer, canvas, true);
                            Document.ExecuteScriptCode();
                            if (mapUriToDocuments.ContainsKey(uri))
                                mapUriToDocuments.Remove(uri);
                            mapUriToDocuments.Add(uri, Document);
                            if (!bLoadingSymbols)
                                Document.OnScreenLoaded();

                            if (mapUriToCanvas.ContainsKey(uri))
                                mapUriToCanvas.Remove(uri);
                            mapUriToCanvas.Add(uri, canvas);
                            if (canvas.Background != null)
                            {
                                if (mapUriToCanvasBackground.ContainsKey(uri))
                                    mapUriToCanvasBackground.Remove(uri);
                                mapUriToCanvasBackground.Add(uri, canvas.Background);
                                canvas.Background = null;
                            }
                            if (mapUriToEmbeddeds.ContainsKey(uri))
                                mapUriToEmbeddeds.Remove(uri);
                            var embedViews = canvas.GetChildrenOfType<ScreenManager.SpecialObjects.EmbeddedTabScreen>();
                            mapUriToEmbeddeds.Add(uri, embedViews.ToList());

                            //double xRatio = 1.0;
                            //double yRatio = 1.0;
                            //Size size = new Size(nWidth, nHeight);
                            //if (nWidth == 0 || nHeight == 0)
                            //    size = new Size(canvas.Width, canvas.Height);
                            //else
                            //{
                            //    xRatio = canvas.Width / (double)nWidth;
                            //    yRatio = canvas.Height / (double)nHeight;

                            //    foreach (FrameworkElement element in canvas.Children)
                            //    {
                            //        var top = Canvas.GetTop(element) / yRatio;
                            //        var left = Canvas.GetLeft(element) / xRatio;
                            //        var width = element.Width / xRatio;
                            //        var height = element.Height / yRatio;
                            //        Canvas.SetTop(element, top);
                            //        Canvas.SetLeft(element, left);
                            //        element.Width = width;
                            //        element.Height = height;
                            //    }
                            //}

                            if (!Double.IsNaN(Document.Width) && Document.Width > 0)
                                retSize.Width = Document.Width;
                            if (!Double.IsNaN(Document.Height) && Document.Height > 0)
                                retSize.Height = Document.Height;

                            if (Double.IsNaN(canvas.Width) || retSize.Width > 0)
                                canvas.Width = retSize.Width;
                            if (Double.IsNaN(canvas.Height) || retSize.Height > 0)
                                canvas.Height = retSize.Height;

                            var listBackground = new List<UIElement>();
                            //canvas.Width = size.Width;
                            //canvas.Height = size.Height;
                            canvas.Measure(retSize);
                            var canvasRect = new Rect(0, 0, retSize.Width, retSize.Height);
                            canvas.Arrange(canvasRect);

                            //var args = new RoutedEventArgs() 
                            //{ 
                            //    RoutedEvent = FrameworkElement.LoadedEvent 
                            //};

                            List<UIElement> listControlCulture = new List<UIElement>();
                            var stringeditorManager = Document.GetService(typeof(IStringEditorManager)) as IStringEditorManager;
                            if (stringeditorManager != null)
                            {
                                if (String.IsNullOrEmpty(Culture))
                                    Culture = stringeditorManager.GetActiveCulture(Document);
                                if (!String.IsNullOrEmpty(Culture))
                                {
                                    var map = stringeditorManager.GetListStringForCulture(Document, Culture);
                                    if (map != null && map.Count > 0)
                                    {
                                        var embedViewers = embedViews.Cast<UserControl>().ToList();
                                        listControlCulture = Document.GetDynControls(canvas, map, embedViewers);
                                        dispatcher.Dispatcher.BeginInvoke(DispatcherPriority.Loaded, new Action(() =>
                                        {
                                            Document.ChangeLanguage(Culture, canvas, map, false, embedViewers);
                                            stringeditorManager.SetActiveCulture(Document, Culture);
                                        }));
                                    }
                                }
                            }

                            var unitConverterEditorComponent = Document.GetService(typeof(IUnitConverterEditorManager)) as IUnitConverterEditorManager;
                            if (unitConverterEditorComponent != null)
                            {
                                if (String.IsNullOrEmpty(Converter))
                                    Converter = unitConverterEditorComponent.GetActiveConverter(Document);
                                if (!String.IsNullOrEmpty(Converter))
                                    Document.ChangeUnitConverter(Converter, unitConverterEditorComponent);
                            }

                            try
                            {
                                if (String.IsNullOrEmpty(User))
                                    RealTimeConnectionManagerViewModel.SetUserIdentity(ClientSessionName, null, new StringCollection());
                                else
                                    RealTimeConnectionManagerViewModel.SetUserIdentity(ClientSessionName, new UserIdentity(User, Password), new StringCollection());
                            }
                            catch (Exception ex)
                            {
                                // ScreenComponent.UIInterface.ShowError(String.Format(Properties.Resources.FailedToActivateUserOnServer, ex.Message));
                                log.Warn(String.Format(Properties.Resources.FailedToActivateUserOnServer, ex.Message));
                            }

                            var listZoomVisibilityEntities = (from c in Document.MapScreenEntities// .AsParallel()
                                                              where c.Value.HasZoomVisibility
                                                              select c.Key).ToList();
                            if (mapZoomVisibilityEntities.ContainsKey(uri))
                                mapZoomVisibilityEntities.Remove(uri);
                            mapZoomVisibilityEntities.Add(uri, listZoomVisibilityEntities);

                            UpdateCanvasScale(canvas, Document, listZoomVisibilityEntities, nWidth, nHeight);

                            if (DisableStaticOptimization)
                            {
                                var id = Document.IdDocument;
                                if (mapGuidToElement.ContainsKey(id))
                                    mapGuidToElement.Remove(id);
                                mapGuidToElement.Add(id, canvas);
                                if (mapElementToGuid.ContainsKey(canvas))
                                    mapElementToGuid.Remove(canvas);
                                mapElementToGuid.Add(canvas, id);
                                ret.Add(id);
                                if (mapGuidToElementUpdateOnly.ContainsKey(id))
                                    mapGuidToElementUpdateOnly.Remove(id);
                                mapGuidToElementUpdateOnly.Add(id, (UIElement)(IsResizable(Document) ? canvas.Parent : canvas));
                                if (mapGuidToElementStatus.ContainsKey(id))
                                    mapGuidToElementStatus.Remove(id);
                                mapGuidToElementStatus.Add(id, new Status() { hasImage = true, connected = true });

                                var rc = new Rect(0, 0, canvas.Width * scaleX, canvas.Height * scaleY);
                                if (mapGuidToElementPos.ContainsKey(id))
                                    mapGuidToElementPos.Remove(id);
                                mapGuidToElementPos.Add(id, rc);
                            }

                            if (DisableStaticOptimization && canvas.CacheMode == null)
                                canvas.CacheMode = new BitmapCache() { EnableClearType = true };

                            var listMissingElement = new List<UIElement>();
                            foreach (FrameworkElement uie in canvas.Children)
                            {
                                //uie.RaiseEvent(args);
                                //var elements = uie.GetChildrenOfType<FrameworkElement>().ToList();
                                //elements.ForEach(el => el.RaiseEvent(args));
                                if (!Document.MapScreenEntities.ContainsKey(uie.Name))
                                {
                                    listMissingElement.Add(uie);
                                    continue;
                                }

                                if (DisableStaticOptimization || 
                                        listControlCulture.Contains(uie) ||
                                        Document.IsDynamicEntity(uie) ||
                                        listAlarm.Contains(uie) ||
                                        listConnectionString.Contains(uie) ||
                                        listEventConnectionString.Contains(uie) ||
                                        listScheduler.Contains(uie))
                                {
                                    var guid = Document.MapScreenEntities[uie.Name].ID;
                                    Document.MapScreenEntities[uie.Name].ForceDynamicOnClient = true;

                                    if (mapGuidToElement.ContainsKey(guid))
                                        mapGuidToElement.Remove(guid);
                                    mapGuidToElement.Add(guid, uie);
                                    if (mapElementToGuid.ContainsKey(uie))
                                        mapElementToGuid.Remove(uie);
                                    mapElementToGuid.Add(uie, guid);

                                    var fe = uie as FrameworkElement;
                                    var controlViewport = (UIElement)(IsResizable(Document) ? canvas.Parent : canvas);
                                    var rect = DependencyObjectExtensions.CalculateBoundRect(new List<UIElement>() { fe }, controlViewport);

                                    if (mapGuidToElementPos.ContainsKey(guid))
                                        mapGuidToElementPos.Remove(guid);
                                    mapGuidToElementPos.Add(guid, rect);
                                    if (mapGuidToParentCanvas.ContainsKey(guid))
                                        mapGuidToParentCanvas.Remove(guid);
                                    mapGuidToParentCanvas.Add(guid, canvas);
                                    ret.Add(guid);

//                                    var layoutUpdatedChangedInvoker = new DelayedSingleActionInvoker(() =>
//                                    {
//                                        lock (lockObject)
//                                        {
//                                            if (!listUpdatingImages.Contains(guid)/* && !listChanged.Contains(guid) &&
//                                                !listChangedPending.Contains(guid)*/)
//                                            {
//                                                if (mapGuidToBase64Element.ContainsKey(guid))
//                                                {
//                                                    if (!listUpdatingImages.Contains(guid))
//                                                        listUpdatingImages.Add(guid);
//                                                    var image = FrameworkElementToBase64(mapGuidToElement[guid], mapGuidToParentCanvas[guid], GetCurrentImageFormat());
//                                                    if (listUpdatingImages.Contains(guid))
//                                                        listUpdatingImages.Remove(guid);
//                                                    if (image != mapGuidToBase64Element[guid])
//                                                    {
//                                                        mapGuidToBase64Element.Remove(guid);
//                                                        mapGuidToBase64Element.Add(guid, image);
//#if DEBUGTRACE
//                                                        System.Diagnostics.Debug.WriteLine(String.Format("LayoutUpdating {0} - {1}", uie.Name, guid));
//#endif
//                                                        if (!listChanged.Contains(guid) &&
//                                                            !listChangedPending.Contains(guid))
//                                                        {
//                                                            listChanged.Add(guid);
//                                                            OnPendingChanges();
//                                                        }
//                                                    }
//                                                }
//                                                else
//                                                {
//                                                    if (mapGuidToBase64Element.ContainsKey(guid))
//                                                        mapGuidToBase64Element.Remove(guid);
//#if DEBUGTRACE
//                                                    System.Diagnostics.Debug.WriteLine(String.Format("LayoutUpdating {0} - {1}", uie.Name, guid));
//#endif
//                                                    if (!listChanged.Contains(guid) &&
//                                                        !listChangedPending.Contains(guid))
//                                                    {
//                                                        listChanged.Add(guid);
//                                                        OnPendingChanges();
//                                                    }
//                                                }
//                                            }

//                                            rect = DependencyObjectExtensions.CalculateBoundRect(new List<UIElement>() { uie },
//                                                                                            mapGuidToParentCanvas[guid]);

//                                            bool bChanged = rect != mapGuidToElementPos[guid];
//                                            if (bChanged)
//                                            {
//#if DEBUGTRACE
//                                                System.Diagnostics.Debug.WriteLine(String.Format("Object {0} Changed its position from {1} to {2}",
//                                                                    uie.Name, mapGuidToElementPos[guid], rect));
//#endif

//                                                mapGuidToElementPos.Remove(guid);
//                                                mapGuidToElementPos.Add(guid, rect);
//                                                if (!listStatusChanged.Contains(guid) &&
//                                                    !listStatusChangedPending.Contains(guid))
//                                                {
//                                                    listStatusChanged.Add(guid);
//                                                    OnPendingStatusChanges();
//                                                }
//                                            }

//                                        }
//                                    }, TimeSpan.FromMilliseconds(RefreshPollingTime), DispatcherPriority.Normal, false);

//                                    uie.LayoutUpdated += (o, e) =>
//                                    {
//#if DEBUGTRACE
//                                        System.Diagnostics.Debug.WriteLine(String.Format("LAYOUTUPDATED for {0} - {1}",
//                                                            uie.Name, guid));
//#endif
//                                        lock (lockObject)
//                                        {
//                                            if (bEnableLayoutUpdates && !listUpdatingImages.Contains(guid))
//                                            {
//                                                if (!listChanged.Contains(guid) &&
//                                                    !listChangedPending.Contains(guid))
//                                                    layoutUpdatedChangedInvoker.BeginInvoke();
//                                                //else
//                                                //{
//                                                //    if (mapGuidToBase64Element.ContainsKey(guid))
//                                                //        mapGuidToBase64Element.Remove(guid);
//                                                //}
//                                            }
//                                        }
//                                    };
                                    //var elements = uie.GetChildrenOfType<FrameworkElement>().ToList();
                                    //elements.ForEach(el =>
                                    //    {
                                    //        el.LayoutUpdated += (o, e) =>
                                    //        {
                                    //            layoutUpdatedChangedInvoker.BeginInvoke();
                                    //        };
                                    //    });

                                    if (!DisableStaticOptimization && uie.CacheMode == null)
                                        uie.CacheMode = new BitmapCache() { EnableClearType = true };
                                }
                                else
                                {
                                    listBackground.Add(uie);
                                }
                            }

                            listBackground.ForEach(element =>
                            {
                                canvas.Children.Remove(element);
                            });
                            listMissingElement.ForEach(element =>
                            {
                                canvas.Children.Remove(element);
                            });
                            if (!mapUriToBackgroundChildren.ContainsKey(uri))
                                mapUriToBackgroundChildren.Add(uri, listBackground);
                        }
                    }

                    if (mapUriToDocuments.ContainsKey(uri))
                    {
                        var doc = mapUriToDocuments[uri];
                        var cnv = mapUriToCanvas[uri];
                        Brush background = cnv.Background;
                        if (mapUriToCanvasBackground.ContainsKey(uri))
                            background = mapUriToCanvasBackground[uri];
                        doc.Background = background;

                        var commandOnNotCommandable = (from c in doc.MapScreenEntities/*.AsParallel()*/
                                                       where c.Value.HasCommands &&
                                                             c.Value.Element != null/* && 
                                                             !(c.Value.Element is ICommandSource)*/
                                                       select c.Key).ToList();
                        commandOnNotCommandable.ForEach(key =>
                        {
                            var uie = doc.MapScreenEntities[key].Element;
                            if (mapElementCommandToName.ContainsKey(uie))
                                mapElementCommandToName.Remove(uie);
                            mapElementCommandToName.Add(uie, key);
                            if (mapElementCommandToDoc.ContainsKey(uie))
                                mapElementCommandToDoc.Remove(uie);
                            mapElementCommandToDoc.Add(uie, doc);
                            if (uie is ContentControl && !(uie is UserControl))
                            {
                                var cc = uie as ContentControl;
                                if (cc.Content != null && cc.Content is UIElement)
                                {
                                    uie = cc.Content as UIElement;
                                    if (mapElementCommandToName.ContainsKey(uie))
                                        mapElementCommandToName.Remove(uie);
                                    mapElementCommandToName.Add(uie, key);
                                    if (mapElementCommandToDoc.ContainsKey(uie))
                                        mapElementCommandToDoc.Remove(uie);
                                    mapElementCommandToDoc.Add(uie, doc);
                                }
                            }
                        });

                        var listDynamic = (from c in doc.MapScreenEntities/*.AsParallel()*/
                                            // where c.Value.IsDynamicEntity() &&
                                           where (doc.IsDynamicEntity(c.Key) || commandOnNotCommandable.Contains(c.Key)) &&
                                           mapGuidToElement.ContainsKey(c.Value.ID) 
                                           //&& !listConnectionString.Contains(mapGuidToElement[c.Value.ID])
                                           select c.Key).ToList();
                        listDynamic.ForEach(key =>
                        {
                            var child = doc.FindInnerControl(cnv, key) as FrameworkElement;
                            if (child != null)
                            {
                                if (!cnv.Children.Contains(child))
                                {
                                    var controlViewport = (UIElement)(IsResizable(doc) ? cnv.Parent : cnv);
                                    var rect = DependencyObjectExtensions.CalculateBoundRect(new List<UIElement>() { child }, controlViewport);
                                    var guid = doc.MapScreenEntities[key].ID;
                                    if (mapGuidToElement.ContainsKey(guid))
                                        mapGuidToElement.Remove(guid);
                                    mapGuidToElement.Add(guid, child);
                                    if (mapElementToGuid.ContainsKey(child))
                                        mapElementToGuid.Remove(child);
                                    mapElementToGuid.Add(child, guid);
                                    if (mapGuidToElementPos.ContainsKey(guid))
                                        mapGuidToElementPos.Remove(guid);
                                    mapGuidToElementPos.Add(guid, rect);
                                    if (mapGuidToParentCanvas.ContainsKey(guid))
                                        mapGuidToParentCanvas.Remove(guid);
                                    mapGuidToParentCanvas.Add(guid, cnv);
                                    ret.Add(guid);

//                                    var layoutUpdatedChangedInvoker = new DelayedSingleActionInvoker(() =>
//                                    {
//                                        lock (lockObject)
//                                        {
//                                            if (!listUpdatingImages.Contains(guid)/* && !listChanged.Contains(guid) &&
//                                                !listChangedPending.Contains(guid)*/)
//                                            {
//                                                if (mapGuidToBase64Element.ContainsKey(guid))
//                                                {
//                                                    if (!listUpdatingImages.Contains(guid))
//                                                        listUpdatingImages.Add(guid);
//                                                    var image = FrameworkElementToBase64(mapGuidToElement[guid], mapGuidToParentCanvas[guid], GetCurrentImageFormat());
//                                                    if (listUpdatingImages.Contains(guid))
//                                                        listUpdatingImages.Remove(guid);
//                                                    if (image != mapGuidToBase64Element[guid])
//                                                    {
//                                                        mapGuidToBase64Element.Remove(guid);
//                                                        mapGuidToBase64Element.Add(guid, image);
//#if DEBUGTRACE
//                                                        System.Diagnostics.Debug.WriteLine(String.Format("LayoutUpdating {0} - {1}", key, guid));
//#endif
//                                                        if (!listChanged.Contains(guid) &&
//                                                            !listChangedPending.Contains(guid))
//                                                        {
//                                                            listChanged.Add(guid);
//                                                            OnPendingChanges();
//                                                        }
//                                                    }
//                                                }
//                                                else
//                                                {
//                                                    if (mapGuidToBase64Element.ContainsKey(guid))
//                                                        mapGuidToBase64Element.Remove(guid);
//#if DEBUGTRACE
//                                                    System.Diagnostics.Debug.WriteLine(String.Format("LayoutUpdating {0} - {1}", key, guid));
//#endif
//                                                    if (!listChanged.Contains(guid) &&
//                                                        !listChangedPending.Contains(guid))
//                                                    {
//                                                        listChanged.Add(guid);
//                                                        OnPendingChanges();
//                                                    }
//                                                }
//                                            }

//                                            rect = DependencyObjectExtensions.CalculateBoundRect(new List<UIElement>() { child },
//                                                                                            mapGuidToParentCanvas[guid]);

//                                            bool bChanged = rect != mapGuidToElementPos[guid];
//                                            if (bChanged)
//                                            {
//#if DEBUGTRACE
//                                                System.Diagnostics.Debug.WriteLine(String.Format("Object {0} Changed its position from {1} to {2}",
//                                                                    child.Name, mapGuidToElementPos[guid], rect));
//#endif

//                                                mapGuidToElementPos.Remove(guid);
//                                                mapGuidToElementPos.Add(guid, rect);
//                                                if (!listStatusChanged.Contains(guid) &&
//                                                    !listStatusChangedPending.Contains(guid))
//                                                {
//                                                    listStatusChanged.Add(guid);
//                                                    OnPendingStatusChanges();
//                                                }
//                                            }

//                                        }
//                                    }, TimeSpan.FromMilliseconds(RefreshPollingTime), DispatcherPriority.Normal, false);

//                                    child.LayoutUpdated += (o, e) =>
//                                    {
//#if DEBUGTRACE
//                                        System.Diagnostics.Debug.WriteLine(String.Format("LAYOUTUPDATED for {0} - {1}",
//                                                            child.Name, guid));
//#endif

//                                        lock (lockObject)
//                                        {
//                                            if (bEnableLayoutUpdates && !listUpdatingImages.Contains(guid))
//                                            {
//                                                if (!listChanged.Contains(guid) &&
//                                                    !listChangedPending.Contains(guid))
//                                                    layoutUpdatedChangedInvoker.BeginInvoke();
//                                                //else
//                                                //{
//                                                //    if (mapGuidToBase64Element.ContainsKey(guid))
//                                                //        mapGuidToBase64Element.Remove(guid);
//                                                //}
//                                            }
//                                        }
//                                    };
                                    //var elements = child.GetChildrenOfType<FrameworkElement>().ToList();
                                    //elements.ForEach(el =>
                                    //{
                                    //    el.LayoutUpdated += (o, e) =>
                                    //    {
                                    //        layoutUpdatedChangedInvoker.BeginInvoke();
                                    //    };
                                    //});
                                }

                                var g = mapElementToGuid[child];
                                if (mapGuidToElementStatus.ContainsKey(g))
                                    mapGuidToElementStatus.Remove(g);
                                mapGuidToElementStatus.Add(g, new Status() { hasImage = !DisableStaticOptimization });
                                //if (doc.MapScreenEntities[key].OpcuaEntityReference != null)
                                //    mapGuidToElementStatus[g].LastMessage = doc.MapScreenEntities[key].OpcuaEntityReference.LastMessage;
                                //else
                                {
                                    mapGuidToElementStatus[g].LastMessage = doc.MapScreenEntities[key].LastErrorMessage;
                                    doc.MapScreenEntities[key].PropertyChanged += ScreenSink_PropertyChanged;
                                }
                                // if (doc.MapScreenEntities[key].HasCommands)
                                if (doc.HasCommandOrInnerHasCommand(key) || commandOnNotCommandable.Contains(key) || child is ScreenManager.SpecialObjects.EmbeddedTabScreen)
                                {
                                    mapGuidToElementStatus[g].simulateEvent = true;
                                    mapGuidToElementStatus[g].connected = true;
                                    mapGuidToElementStatus[g].writable = child.IsHitTestVisible;
                                }
                                else
                                {
                                    UIElement finalElement;
                                    mapGuidToElementStatus[g].simulateEvent = CanSimulateEvent(child, out finalElement, true) || HasValueProvider(child, new Point(0, 0), true);
                                }

                                    var datacontextChild = child;
                                    if (doc.MapScreenEntities[key].SourceSymbolLinked && datacontextChild is ContentControl)
                                    {
                                        datacontextChild = (datacontextChild as ContentControl).Content as FrameworkElement;
                                        if (datacontextChild == null)
                                            datacontextChild = child;

                                        //if (datacontextChild.CacheMode == null)
                                        //    datacontextChild.CacheMode = new BitmapCache() { EnableClearType = true };
                                    }

                                    if (mapDataContextElementToGuid.ContainsKey(datacontextChild))
                                        mapDataContextElementToGuid.Remove(datacontextChild);
                                    mapDataContextElementToGuid.Add(datacontextChild, g);
                                    datacontextChild.DataContextChanged += datacontextChild_DataContextChanged;

                                    if (datacontextChild.DataContext != null)
                                    {
                                        datacontextChild_DataContextChanged(datacontextChild, new DependencyPropertyChangedEventArgs());
                                        //var data = datacontextChild.DataContext;
                                        //datacontextChild.DataContext = null;
                                        //datacontextChild.DataContext = data;
                                    }

                                    if (doc.MapScreenEntities[key].OpcuaEntityReference == null ||
                                        doc.MapScreenEntities[key].OpcuaEntityReference.TypeDefinitionNodeId != null ||
                                        !doc.MapScreenEntities[key].OpcuaEntityReference.IsValid)
                                        mapGuidToElementStatus[g].connected = true;
                                // }

                                lock (lockObject)
                                {
                                    if (!listStatusChanged.Contains(g) &&
                                        !listStatusChangedPending.Contains(g))
                                        listStatusChanged.Add(g);
                                }
                            }
                        });
                    }
                    else
                        ret = null;

                    // bEnableLayoutUpdates = true;
                }
            });

            if (listStatusChanged.Count > 0)
                OnPendingStatusChanges();

            sizeScreen = retSize;

            return ret;
        }

        void Document_GetTagEntityReference(object sender, GetTagEntityReference ev)
        {
            var Document = sender as ScreenDocument;
            if (Document == null)
                return;
            var service = Document.GetService(typeof(IUFUAEditorManager)) as IUFUAEditorManager;
            if (service == null)
                return;
            var erString = service.GetTagEntityReference(Document, ev.Name, ev.Instance, inExecution: true);
            if (!String.IsNullOrEmpty(erString))
                ev.entityReference = erString.FromXml<OPCUAEntityReference>();
        }

        void Document_GetPrototypeList(object sender, GetPrototypeListEventArgs ev)
        {
            var Document = sender as ScreenDocument;
            if (Document == null)
                return;
            var service = Document.GetService(typeof(IUFUAEditorManager)) as IUFUAEditorManager;
            if (service == null)
                return;
            ev.mapDefinitions = service.GetFlatListPrototypes(Document);
            ev.mapPrototypes = service.GetFlatListPrototypeInstances(Document);
        }

        void Document_GetTagList(object sender, GetTagListEventArgs ev)
        {
            var Document = sender as ScreenDocument;
            if (Document == null)
                return;
            var service = Document.GetService(typeof(IUFUAEditorManager)) as IUFUAEditorManager;
            if (service == null)
                return;
            ev.list = service.GetFlatListTags(Document);
        }

        void datacontextChild_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var datacontextChild = sender as FrameworkElement;
            var monitoredItem = datacontextChild.DataContext as MonitoredItemViewModel;
            if (!mapDataContextElementToGuid.ContainsKey(datacontextChild) ||
                (monitoredItem != null && monitoredItem.IsReadOnly))
                return;
            datacontextChild.DataContextChanged -= datacontextChild_DataContextChanged;
            var g = mapDataContextElementToGuid[datacontextChild];
            mapDataContextElementToGuid.Remove(datacontextChild);
            lock (lockObject)
            {
                if (mapGuidToDataContextItem.ContainsKey(g))
                    mapGuidToDataContextItem.Remove(g);

                if (monitoredItem != null && mapGuidToElementStatus.ContainsKey(g))
                {
                    mapGuidToDataContextItem.Add(g, monitoredItem);

                    var listAlarm = new List<FrameworkElement>();
                    var listRecipe = new List<FrameworkElement>();
                    var listConnectionString = new List<FrameworkElement>();
                    var listEventConnectionString = new List<FrameworkElement>();
                    var listScheduler = new List<FrameworkElement>();
                    ScreenDocument.CheckPreBinding(listAlarm, listConnectionString, listEventConnectionString, listScheduler, datacontextChild, listRecipe);
                    //datacontextChild.GetChildrenOfType<FrameworkElement>().ToList().ForEach(element =>
                    //{
                    //    ScreenDocument.CheckPreBinding(listAlarm, listConnectionString, listEventConnectionString, listScheduler, element);
                    //});

                    if (monitoredItem.NodeIdModel != null && monitoredItem.NodeIdModel.IsScalar && listAlarm.Count == 0 && listConnectionString.Count == 0 &&
                        listEventConnectionString.Count == 0 && listScheduler.Count == 0)
                    {
                        if (monitoredItem.NodeIdModel.IsBoolean)
                            mapGuidToElementStatus[g].dataType = 0;
                        else if (monitoredItem.NodeIdModel.IsString)
                            mapGuidToElementStatus[g].dataType = 2;
                        else
                            mapGuidToElementStatus[g].dataType = 1;
                    }
                    else
                        mapGuidToElementStatus[g].dataType = -1;

                    if (datacontextChild is UserControl)
                    {
                        var autoElement = UIElementAutomationPeer.CreatePeerForElement(datacontextChild) as UIElementAutomationPeer;
                        if (autoElement != null &&
                            autoElement.GetPattern(PatternInterface.Value) is IValueProvider &&
                            (autoElement.GetAutomationControlType() == AutomationControlType.Custom))
                        {
                            mapGuidToElementStatus[g].dataType = -1;
                        }
                    }

                    if (monitoredItem.Range != null)
                    {
                        mapGuidToElementStatus[g].minValue = monitoredItem.Range.Low;
                        mapGuidToElementStatus[g].maxValue = monitoredItem.Range.High;
                    }

                    mapGuidToElementStatus[g].writable = HasValueProvider(datacontextChild, new Point(0, 0), false) && datacontextChild.IsHitTestVisible &&
                        monitoredItem.NodeIdModel != null && monitoredItem.NodeIdModel.IsUserWritable;

                    mapGuidToElementStatus[g].connected = true;
                    if (monitoredItem.NodeIdModel != null && !monitoredItem.GetSubscriptionViewModelParent().GetSessionViewModelParent().Connected)
                        mapGuidToElementStatus[g].LastMessage = monitoredItem.GetSubscriptionViewModelParent().GetSessionViewModelParent().LastMessage;
                    else
                        mapGuidToElementStatus[g].LastMessage = String.Empty;

                    if (!listStatusChanged.Contains(g) &&
                        !listStatusChangedPending.Contains(g))
                    {
                        listStatusChanged.Add(g);
                    }

                    //monitoredItem.GetSubscriptionViewModelParent().GetSessionViewModelParent().PropertyChanged += (ob, ev) =>
                    //    {
                    //        if (ev.PropertyName == "LastMessage")
                    //        {
                    //            lock (lockObject)
                    //            {
                    //                if (mapGuidToElementStatus.ContainsKey(g))
                    //                {
                    //                    if (!monitoredItem.GetSubscriptionViewModelParent().GetSessionViewModelParent().Connected)
                    //                        mapGuidToElementStatus[g].LastMessage = monitoredItem.GetSubscriptionViewModelParent().GetSessionViewModelParent().LastMessage;
                    //                    else
                    //                        mapGuidToElementStatus[g].LastMessage = String.Empty;

                    //                    if (!listStatusChanged.Contains(g))
                    //                        listStatusChanged.Add(g);
                    //                }
                    //            }
                    //        }
                    //    };
                }
            }

            if (listStatusChanged.Count > 0)
                OnPendingStatusChanges();
        }

        void ScreenSink_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "LastErrorMessage")
            {
                var entity = (sender as ScreenEntity);
                if (entity == null)
                    return;
                var g = entity.ID;
                mapGuidToElementStatus[g].LastMessage = (sender as ScreenEntity).LastErrorMessage;

                lock (lockObject)
                {
                    if (!listStatusChanged.Contains(g) &&
                        !listStatusChangedPending.Contains(g))
                    {
                        listStatusChanged.Add(g);
                    }
                }

                if (listStatusChanged.Count > 0)
                    OnPendingStatusChanges();
            }
        }

        public Status GetDataValueAndRange(Guid id)
        {
            lastTimeUsed = DateTime.UtcNow;
            lock (lockObject)
            {
                if (mapGuidToDataContextItem.ContainsKey(id) && mapGuidToElement.ContainsKey(id))
                {
                    if (mapGuidToElement[id].Visibility == Visibility.Visible)
                    {
                        mapGuidToElementStatus[id].minValue = mapGuidToDataContextItem[id].Range.Low;
                        mapGuidToElementStatus[id].maxValue = mapGuidToDataContextItem[id].Range.High;
                        mapGuidToElementStatus[id].value = mapGuidToDataContextItem[id].Value;
                        return mapGuidToElementStatus[id];
                    }
                }
            }

            return null;
        }

        public bool SetDataValue(Guid id, String value)
        {
            lastTimeUsed = DateTime.UtcNow;
            if (thread == null)
                return false;
            lock (lockObject)
            {
                if (mapGuidToDataContextItem.ContainsKey(id))
                {
                    mapGuidToDataContextItem[id].Value = value;
                    return true;
                }
            }
            return false;
        }

        public static Size GetScreenSize(Uri uri, IDocument parent, FileSystemProviderBase fileSystemProvider = null)
        {
            var ret = new Size();
            var doc = ScreenDocument.FromFile(uri.GetPathString(), parent);
            if (doc != null)
            {
                ret.Width = doc.Width;
                ret.Height = doc.Height;
                doc.Dispose();
            }
            return ret;
        }

        public void CloseUri(Uri uri)
        {
            lastTimeUsed = DateTime.UtcNow;
            EnsureThread();
            if (thread == null || bDisposed || uri == null)
                return;
            dispatcher.Dispatcher.InvokeIfRequired(() =>
            {
                Canvas canvas = null;
                ScreenDocument Document = null;
                lock (lockObject)
                {
                    if (mapUriToBackground.ContainsKey(uri))
                        mapUriToBackground.Remove(uri);
                    if (mapUriToBackgroundChildren.ContainsKey(uri))
                    {
                        mapUriToBackgroundChildren[uri].Clear();
                        mapUriToBackgroundChildren.Remove(uri);
                    }
                    if (mapUriToCanvas.ContainsKey(uri))
                    {
                        canvas = mapUriToCanvas[uri];
                        mapUriToCanvas.Remove(uri);
                        mapUriToCanvasBackground.Remove(uri);
                        mapUriToEmbeddeds.Remove(uri);

                        if (gridContainer.Children.Contains(canvas))
                            gridContainer.Children.Remove(canvas);

                        foreach (UIElement uie in canvas.Children)
                        {
                            var guid = mapElementToGuid[uie];

                            RemoveElementFromMaps(uie);
                            
                            mapGuidToElement.Remove(guid);
                            mapGuidToBase64Element.Remove(guid);
                            mapGuidToElementPos.Remove(guid);
                            if (mapGuidToParentCanvas.ContainsKey(guid))
                                mapGuidToParentCanvas.Remove(guid);
                            if (mapGuidToDataContextItem.ContainsKey(guid))
                                mapGuidToDataContextItem.Remove(guid);
                            if (mapGuidToElementStatus.ContainsKey(guid))
                                mapGuidToElementStatus.Remove(guid);
                            if (listChanged.Contains(guid))
                                listChanged.Remove(guid);
                            if (listChangedPending.Contains(guid))
                                listChangedPending.Remove(guid);
                            if (listStatusChanged.Contains(guid))
                                listStatusChanged.Remove(guid);
                            if (listStatusChangedPending.Contains(guid))
                                listStatusChangedPending.Remove(guid);

                            var found = (from c in mapDataContextElementToGuid where c.Value == guid select c.Key).ToList();
                            if (found.Count > 0 && mapDataContextElementToGuid.ContainsKey(found[0]))
                            {
                                mapDataContextElementToGuid.Remove(found[0]);
                                found[0].DataContextChanged -= datacontextChild_DataContextChanged;
                            }
                        }

                        if (mapUriToDocuments.ContainsKey(uri))
                        {
                            Document = mapUriToDocuments[uri];

                            foreach (var key in Document.MapScreenEntities.Keys)
                                Document.MapScreenEntities[key].PropertyChanged -= ScreenSink_PropertyChanged;

                            var listDynamic = (from c in Document.MapScreenEntities/*.AsParallel()*/
                                               where c.Value.IsDynamicEntity()
                                               select c.Key).ToList();
                            listDynamic.ForEach(key =>
                            {
                                var child = Document.FindInnerControl(canvas, key) as FrameworkElement;
                                if (child != null)
                                {
                                    if (!canvas.Children.Contains(child))
                                    {
                                        var guid = Document.MapScreenEntities[key].ID;

                                        RemoveElementFromMaps(child);
                                                                                
                                        mapGuidToElement.Remove(guid);
                                        mapGuidToBase64Element.Remove(guid);
                                        mapGuidToElementPos.Remove(guid);
                                        if (mapGuidToParentCanvas.ContainsKey(guid))
                                            mapGuidToParentCanvas.Remove(guid);
                                        if (mapGuidToDataContextItem.ContainsKey(guid))
                                            mapGuidToDataContextItem.Remove(guid);
                                        if (mapGuidToElementStatus.ContainsKey(guid))
                                            mapGuidToElementStatus.Remove(guid);
                                        if (listChanged.Contains(guid))
                                            listChanged.Remove(guid);
                                        if (listChangedPending.Contains(guid))
                                            listChangedPending.Remove(guid);
                                        if (listStatusChanged.Contains(guid))
                                            listStatusChanged.Remove(guid);
                                        if (listStatusChangedPending.Contains(guid))
                                            listStatusChangedPending.Remove(guid);

                                        var found = (from c in mapDataContextElementToGuid where c.Value == guid select c.Key).ToList();
                                        if (found.Count > 0 && mapDataContextElementToGuid.ContainsKey(found[0]))
                                        {
                                            mapDataContextElementToGuid.Remove(found[0]);
                                            found[0].DataContextChanged -= datacontextChild_DataContextChanged;
                                        }
                                    }
                                }
                            });

                            if (mapGuidToElementUpdateOnly.ContainsKey(Document.IdDocument))
                                mapGuidToElementUpdateOnly.Remove(Document.IdDocument);
                            if (mapGuidToElement.ContainsKey(Document.IdDocument))
                                mapGuidToElement.Remove(Document.IdDocument);
                            if (mapElementToGuid.ContainsKey(canvas))
                                mapElementToGuid.Remove(canvas);
                            if (mapGuidToElementStatus.ContainsKey(Document.IdDocument))
                                mapGuidToElementStatus.Remove(Document.IdDocument);

                            if (mapGuidToElementPos.ContainsKey(Document.IdDocument))
                                mapGuidToElementPos.Remove(Document.IdDocument);
                            if (mapGuidToParentCanvas.ContainsKey(Document.IdDocument))
                                mapGuidToParentCanvas.Remove(Document.IdDocument);

                            mapUriToDocuments.Remove(uri);
                            SaveData(GetUniqueTitle(Document), Document);
                            Document.OnScreenUnloaded();
                        }

                        if (mapZoomVisibilityEntities.ContainsKey(uri))
                            mapZoomVisibilityEntities.Remove(uri);
                    }
                }

                if (Document != null)
                {
                    Document.GetTagList -= Document_GetTagList;
                    Document.GetPrototypeList -= Document_GetPrototypeList;
                    Document.GetTagEntityReference -= Document_GetTagEntityReference;

                    if (!Document.TerminateScriptCode())
                    {
                        AddDocumentToBeDisposed(Document, canvas);
                        return;
                    }

                    Document.TerminateExecution(canvas);
                    Document.UnloadResources(canvas);
                    Document.Dispose();
                }

                if (canvas != null)
                    canvas.Dispose();
            });
        }

        DispatcherTimer delayTerminate;
        void AddDocumentToBeDisposed(ScreenDocument doc, Canvas canvas)
        {
            if (!mapDocumentsToBeDisposed.ContainsKey(doc))
                mapDocumentsToBeDisposed.Add(doc, canvas);
            if (delayTerminate == null)
            {
                delayTerminate = new DispatcherTimer();
                delayTerminate.Interval = TimeSpan.FromSeconds(1);
                delayTerminate.Tick += (o, e) =>
                {
                    delayTerminate.Stop();

                    CleanDocumentToBeDisposed();

                    if (mapDocumentsToBeDisposed.Count > 0)
                        delayTerminate.Start();
                };
            }

            delayTerminate.Start();
        }

        void CleanDocumentToBeDisposed()
        {
            mapDocumentsToBeDisposed.Keys.ToList().ForEach(doc =>
            {
                if (doc.TerminateScriptCode())
                {
                    doc.TerminateExecution(mapDocumentsToBeDisposed[doc]);
                    doc.UnloadResources(mapDocumentsToBeDisposed[doc]);
                    doc.Dispose();
                    mapDocumentsToBeDisposed[doc].Dispose();
                    mapDocumentsToBeDisposed.Remove(doc);
                }
            });
        }

        void RemoveElementFromMaps(UIElement uie)
        {
            if (mapElementToGuid.ContainsKey(uie))
                mapElementToGuid.Remove(uie);
            if (mapElementCommandToName.ContainsKey(uie))
                mapElementCommandToName.Remove(uie);
            if (mapElementCommandToDoc.ContainsKey(uie))
                mapElementCommandToDoc.Remove(uie);
            if (uie is ContentControl && !(uie is UserControl))
            {
                var cc = uie as ContentControl;
                if (cc.Content != null && cc.Content is UIElement)
                {
                    var element = cc.Content as UIElement;
                    if (mapElementCommandToName.ContainsKey(element))
                        mapElementCommandToName.Remove(element);
                    if (mapElementCommandToDoc.ContainsKey(element))
                        mapElementCommandToDoc.Remove(element);
                }
            }

            if (mapOriginalBound.ContainsKey(uie))
                mapOriginalBound.Remove(uie);

            var fe = uie as FrameworkElement;
            if (uie is ContentControl && (uie as ContentControl).Content is FrameworkElement && !(uie is UserControl))
                fe = (uie as ContentControl).Content as FrameworkElement;
        }

        public Rect GetStartingPoint(Guid guid)
        {
            lastTimeUsed = DateTime.UtcNow;
            lock (lockObject)
            {
                if (mapGuidToElementPos.ContainsKey(guid))
                    return mapGuidToElementPos[guid];
                return Rect.Empty;
            }
        }

        static readonly Status defStatus = new Status
        {
            connected = true,
            writable = false
        };

        public Status GetStatus(Guid guid)
        {
            lastTimeUsed = DateTime.UtcNow;
            lock (lockObject)
            {
                if (listStatusChangedPending.Contains(guid))
                    listStatusChangedPending.Remove(guid);

                if (mapGuidToElementStatus.ContainsKey(guid))
                    return mapGuidToElementStatus[guid];
                return defStatus;
            }
        }


        public String GetImageBase64(Guid guid)
        {
            lastTimeUsed = DateTime.UtcNow;
            var ret = String.Empty;
            lock (lockObject)
            {
#if DEBUGTRACE
                System.Diagnostics.Debug.WriteLine(String.Format("Getting Image for {0}", guid));
#endif
                if (mapGuidToBase64Element.ContainsKey(guid))
                {
                    if (listChangedPending.Contains(guid))
                        listChangedPending.Remove(guid);
                    return mapGuidToBase64Element[guid];
                }
            }

            EnsureThread();
            if (thread == null || bDisposed)
                return String.Empty;
            dispatcher.Dispatcher.InvokeIfRequired(() =>
            {
                lock (lockObject)
                {
#if DEBUGTRACE
                    System.Diagnostics.Debug.WriteLine(String.Format("Getting Image Generation for {0}", guid));
#endif

                    if (mapGuidToBase64Element.ContainsKey(guid))
                        ret = mapGuidToBase64Element[guid];
                    else if (mapGuidToElement.ContainsKey(guid))
                    {
                        //if (!listUpdatingImages.Contains(guid))
                        //    listUpdatingImages.Add(guid);
                        Canvas parent = null;
                        if (mapGuidToParentCanvas.ContainsKey(guid))
                            parent = mapGuidToParentCanvas[guid];
                        ret = FrameworkElementToBase64(mapGuidToElement[guid], parent, GetCurrentImageFormat());
                        //if (listUpdatingImages.Contains(guid))
                        //    listUpdatingImages.Remove(guid);
                        //if (String.IsNullOrEmpty(ret))
                        //{
                        //    //if (!listChanged.Contains(guid) &&
                        //    //    !listChangedPending.Contains(guid))
                        //    //{
                        //    //    listChanged.Add(guid);
                        //    //    OnPendingChanges();
                        //    //}
                        //}
                        //else
                        if (mapGuidToBase64Element.ContainsKey(guid))
                            mapGuidToBase64Element.Remove(guid);
                            mapGuidToBase64Element.Add(guid, ret);
                    }

                    if (listChangedPending.Contains(guid))
                        listChangedPending.Remove(guid);
                }
            });

            return ret;
        }

        public IList<Guid> ListChanges()
        {
            lastTimeUsed = DateTime.UtcNow;
            lock (lockObject)
            {
#if DEBUGTRACE
                System.Diagnostics.Debug.WriteLine(String.Format("Getting list of changes {0}", listChanged.Count));
#endif

                if (listChanged.Count == 0)
                    return null;

                var ret = new List<Guid>();
                ret.AddRange(listChanged);
                listChanged.ForEach(guid =>
                {
                    if (!listChangedPending.Contains(guid))
                        listChangedPending.Add(guid);
                });
                listChanged.Clear();
                bPendingChanges = false;
                return ret;
            }
        }

        public IList<Guid> ListStatusChanges()
        {
            lastTimeUsed = DateTime.UtcNow;
            lock (lockObject)
            {
#if DEBUGTRACE
                System.Diagnostics.Debug.WriteLine(String.Format("Getting list of status changes {0}", listStatusChanged.Count));
#endif

                if (listStatusChanged.Count == 0)
                    return null;

                var ret = new List<Guid>();
                ret.AddRange(listStatusChanged);
                listStatusChanged.Clear();
                bPendingStatusChanges = false;
                return ret;
            }
        }

        public IDictionary<String, String>[] GetCurrentListVoiceCommands(Uri uri, String locale)
        {
            lastTimeUsed = DateTime.UtcNow;
            if (thread == null)
                return null;
            lock (lockObject)
            {
                var pathString = uri.GetPathString();
                int foundSep = pathString.IndexOf(parameterSeparator);
                if (foundSep >= 0)
                {
                    pathString = pathString.Substring(0, foundSep);
                    uri = new Uri(pathString);
                }
                if (mapUriToDocuments.ContainsKey(uri))
                {
                    var Document = mapUriToDocuments[uri];

                    var list = (from c in Document.MapScreenEntities.Keys.AsParallel()
                                where !String.IsNullOrEmpty(Document.MapScreenEntities[c].SpeechCommand) &&
                                      Document.MapScreenEntities[c].HasCommands
                                select Document.MapScreenEntities[c].SpeechCommand).ToList();
                    var distinctList = list.Distinct();
                    var duplicates = list.GroupBy(x => x).Where(x => x.Count() > 1).Select(x => x.Key).Distinct().ToDictionary(x => x);
                    var map = distinctList.ToDictionary(x => x);
                    var mapTranslated = new Dictionary<string, string>(map);

                    if (!String.IsNullOrEmpty(locale))
                    {
                        var stringeditorManager = Document.GetService(typeof(IStringEditorManager)) as IStringEditorManager;
                        if (stringeditorManager != null)
                        {
                            var mapculture = stringeditorManager.GetListStringForCulture(Document, locale);
                            if (mapculture != null)
                            {
                                var keys = map.Keys;
                                foreach(var key in keys)
                                {
                                    if (mapculture.ContainsKey(key))
                                    {
                                        mapTranslated.Remove(key);
                                        mapTranslated.Add(key, mapculture[key]);
                                    }
                                }
                            }
                        }
                    }

                    return new Dictionary<string, string>[] { mapTranslated, duplicates };
                }
            }

            return null;
        }

        public void ExecuteVoiceCommands(Uri uri, String command)
        {
            lastTimeUsed = DateTime.UtcNow;
            EnsureThread();
            if (thread == null || bDisposed)
                return;

            dispatcher.Dispatcher.InvokeIfRequired(() =>
            {
                lock (lockObject)
                {
                    var pathString = uri.GetPathString();
                    int foundSep = pathString.IndexOf(parameterSeparator);
                    if (foundSep >= 0)
                    {
                        pathString = pathString.Substring(0, foundSep);
                        uri = new Uri(pathString);
                    }
                    if (mapUriToDocuments.ContainsKey(uri))
                    {
                        var Document = mapUriToDocuments[uri];

                        var foundKeys = (from c in Document.MapScreenEntities.Keys.AsParallel()
                                            where Document.MapScreenEntities[c].SpeechCommand == command &&
                                                Document.MapScreenEntities[c].HasCommands
                                            select c).ToList();
                        if (foundKeys.Count > 0)
                        {
                            Document.RemoteExecuteCommand(foundKeys[0]);
                        }
                    }
                }
            });
        }

        static readonly string parameterSeparator = "$";

        public IDictionary<String, bool> ListPendingCommands(Uri uri)
        {
            lastTimeUsed = DateTime.UtcNow;
            var ret = new Dictionary<String, bool>();
            if (thread == null)
                return ret;
            lock (lockObject)
            {
                var pathString = uri.GetPathString();
                int foundSep = pathString.IndexOf(parameterSeparator);
                if (foundSep >= 0)
                {
                    pathString = pathString.Substring(0, foundSep);
                    uri = new Uri(pathString);
                }
                if (mapUriToDocuments.ContainsKey(uri))
                {
                    var Document = mapUriToDocuments[uri];

                    var listCommandEmbedded = new List<CommandManager.RemoteExecute>();
                    if (mapUriToEmbeddeds.ContainsKey(uri))
                    {
                        var listEmbeeded = mapUriToEmbeddeds[uri];
                        if (listEmbeeded.Count > 0)
                        {
                            if (!bDisposed)
                            {
                                listEmbeeded.ForEach(embedded =>
                                    {
                                        var l = embedded.GetPendingCommands();
                                        if (l != null)
                                            listCommandEmbedded.AddRange(l);
                                    });
                            }
                        }
                    }

                    var list = Document.GetPendingCommands();
                    if (list == null)
                        list = listCommandEmbedded;
                    else
                        list.AddRange(listCommandEmbedded);
                    if (list != null)
                    {
                        foreach (var el in list)
                        {
                            if (el.uri != null)
                            {
                                if (el is CommandManager.ChangeCultureRemoteExecute)
                                {
                                    ScreenDocument doc = null;
                                    Canvas canvas = null;
                                    List<ScreenManager.SpecialObjects.EmbeddedTabScreen> embeddeds = new List<ScreenManager.SpecialObjects.EmbeddedTabScreen>();
                                    var found = (from c in mapUriToDocuments where c.Value.Parent == el.Parent.Parent select c).ToList();
                                    if (found.Count > 0)
                                    {
                                        doc = found[0].Value;
                                        if (mapUriToCanvas.ContainsKey(found[0].Key))
                                            canvas = mapUriToCanvas[found[0].Key];
                                        if (mapUriToEmbeddeds.ContainsKey(found[0].Key))
                                            embeddeds = mapUriToEmbeddeds[found[0].Key];
                                    }
                                    if (doc == null || canvas == null)
                                        continue;

                                    EnsureThread();
                                    if (thread == null || bDisposed)
                                        continue;
                                    dispatcher.Dispatcher.BeginInvokeIfRequired(() =>
                                    {
                                        var stringeditorManager = doc.GetService(typeof(IStringEditorManager)) as IStringEditorManager;
                                        if (stringeditorManager == null)
                                            return;
                                        Culture = el.uri.OriginalString;
                                        if (!String.IsNullOrEmpty(Culture))
                                        {
                                            var map = stringeditorManager.GetListStringForCulture(doc, Culture);
                                            if (map != null && map.Count > 0)
                                            {
                                                doc.ChangeLanguage(Culture, canvas, map, false, embeddeds.Cast<UserControl>().ToList());

                                                if (stringeditorManager != null)
                                                {
                                                    stringeditorManager.SetActiveCulture(doc, Culture);
                                                }
                                            }
                                        }
                                    });
                                }
                                else if (el is CommandManager.ChangeConverterRemoteExecute)
                                {
                                    ScreenDocument doc = null;
                                    Canvas canvas = null;
                                    var found = (from c in mapUriToDocuments where c.Value.Parent == el.Parent.Parent select c).ToList();
                                    if (found.Count > 0)
                                    {
                                        doc = found[0].Value;
                                        if (mapUriToCanvas.ContainsKey(found[0].Key))
                                            canvas = mapUriToCanvas[found[0].Key];
                                    }
                                    if (doc == null || canvas == null)
                                        continue;

                                    EnsureThread();
                                    if (thread == null || bDisposed)
                                        continue;
                                    dispatcher.Dispatcher.BeginInvokeIfRequired(() =>
                                    {
                                        var unitConverterEditorComponent = doc.GetService(typeof(IUnitConverterEditorManager)) as IUnitConverterEditorManager;
                                        if (unitConverterEditorComponent == null)
                                            return;
                                        Converter = el.uri.OriginalString;
                                        doc.ChangeUnitConverter(Converter, unitConverterEditorComponent);
                                    });
                                }
                                else if (el is CommandManager.OpenScreenCommandRemoteExecute)
                                {
                                    var os = el as CommandManager.OpenScreenCommandRemoteExecute;
                                    if (el.executionMode != ExecutionMode.Stop)
                                    {
                                        var path = el.uri.ToString();
                                        if (os.PreserveCurrentParameterFile)
                                        {
                                            ScreenDocument doc = null;
                                            var found = (from c in mapUriToDocuments where c.Value.Parent == el.Parent.Parent select c).ToList();
                                            if (found.Count > 0)
                                            {
                                                doc = (el.Parent as ScreenDocument) ?? found[0].Value;
                                            }
                                            if (doc != null && !String.IsNullOrEmpty(doc.ParameterFile))
                                                path = String.Format("{0}{1}{2}", path, parameterSeparator, doc.ParameterFile);
                                        }
                                        else if (os.ParameterFile != null && !String.IsNullOrEmpty(os.ParameterFile.GetPathString()))
                                            path = String.Format("{0}{1}{2}", path, parameterSeparator, os.ParameterFile.GetPathString());
                                        if (ret.ContainsKey(path))
                                            ret.Remove(path);
                                        if (!ret.ContainsKey(path))
                                            ret.Add(path, el.executionMode == ExecutionMode.Synchro);
                                    }
                                    else if (listOpenedUri.Count > 1)
                                    {
                                        if (ret.ContainsKey(listOpenedUri[listOpenedUri.Count - 2]))
                                            ret.Remove(listOpenedUri[listOpenedUri.Count - 2]);
                                        ret.Add(listOpenedUri[listOpenedUri.Count - 2], el.executionMode == ExecutionMode.Synchro);
                                        listOpenedUri.RemoveAt(listOpenedUri.Count - 1);
                                        listOpenedUri.RemoveAt(listOpenedUri.Count - 1);
                                        if (!ret.ContainsKey(String.Empty))
                                            ret.Add(String.Empty, false);
                                    }
                                }
                            }
                            else if (el is CommandManager.OpenScreenCommandRemoteExecute)
                            {
                                if (el.executionMode == ExecutionMode.Stop)
                                {
                                    if (listOpenedUri.Count > 1)
                                    {
                                        if (ret.ContainsKey(listOpenedUri[listOpenedUri.Count - 2]))
                                            ret.Remove(listOpenedUri[listOpenedUri.Count - 2]);
                                        ret.Add(listOpenedUri[listOpenedUri.Count - 2], el.executionMode == ExecutionMode.Synchro);
                                        listOpenedUri.RemoveAt(listOpenedUri.Count - 1);
                                        listOpenedUri.RemoveAt(listOpenedUri.Count - 1);
                                    }
                                    if (!ret.ContainsKey(String.Empty))
                                        ret.Add(String.Empty, false);
                                }
                            }
                            else if (el.ex != null)
                            {
                                var key = String.Format("{0}{1}", parameterSeparator, el.ex.Message);
                                if (!ret.ContainsKey(key))
                                    ret.Add(String.Format("{0}{1}", parameterSeparator, el.ex.Message), false);
                                log.Warn(el.ex.Message);
                            }
                        }
                    }

                }
            }
            return ret;
        }

        #endregion

        #region Properties
        private int _RefreshPollingTime = 200;
        public int RefreshPollingTime
        {
            get { return _RefreshPollingTime; }
            set
            {
                _RefreshPollingTime = value;
            }
        }

        private int _RefreshPollingTimeCount = 10;
        public int RefreshPollingTimeCount
        {
            get { return _RefreshPollingTimeCount; }
            set
            {
                _RefreshPollingTimeCount = value;
            }
        }

        private int _SessionTimeout = 60;
        public int SessionTimeout
        {
            get { return _SessionTimeout; }
            set
            {
                _SessionTimeout = value;
            }
        }

        private int _ConcurrentRenderingPipeline = 5;
        public int ConcurrentRenderingPipeline
        {
            get { return _ConcurrentRenderingPipeline; }
            set
            {
                _ConcurrentRenderingPipeline = value;
                if (lockSemaphoreRenderingPipeLine == null)
                    lockSemaphoreRenderingPipeLine = new Semaphore(_ConcurrentRenderingPipeline, _ConcurrentRenderingPipeline);
            }
        }
        

        private bool _LowResolution = false;
        public bool LowResolution
        {
            get { return _LowResolution; }
            set
            {
                _LowResolution = value;
            }
        }

        private string _ClientSessionName = "ScreenSinkHTML5";
        public string ClientSessionName
        {
            get { return _ClientSessionName; }
            set
            {
                _ClientSessionName = value;
            }
        }

        private string _User;
        public string User
        {
            get { return _User; }
            set
            {
                _User = value;
            }
        }

        private string _Password;
        public string Password
        {
            get { return _Password; }
            set
            {
                _Password = value;
            }
        }

        public string Role
        {
            get { return _Role; }
            set
            {
                _Role = value;
            }
        }

        public string Culture
        {
            get { return _Culture; }
            set
            {
                _Culture = value;
            }
        }

        public string Converter
        {
            get { return _Converter; }
            set
            {
                _Converter = value;
            }
        }

        public int AccessMask
        {
            get { return _AccessMask; }
            set
            {
                _AccessMask = value;
            }
        }

        public int AccessLevel
        {
            get { return _AccessLevel; }
            set
            {
                _AccessLevel = value;
            }
        }

        public bool UseLicense
        {
            get { return !bDisposed && !isPopup; }
        }

        public bool DemoMode
        {
            get { return !activeSessions.FoundLicense; }
        }
        #endregion

        #region Events
        bool bEnableEvents;
        public void EnableEvents(bool bEnable = true)
        {
            if (bEnableEvents == bEnable)
                return;
            bEnableEvents = bEnable;
            if (listChanged.Count > 0)
                OnPendingChanges();
            if (listStatusChanged.Count > 0)
                OnPendingStatusChanges();
        }

        public event EventHandler PendingChanges;
        bool bPendingChanges;
        void OnPendingChanges()
        {
            if (!bEnableEvents || bPendingChanges)
                return;

#if DEBUGTRACE
            System.Diagnostics.Debug.WriteLine("Sending PendigChange");
#endif

            bPendingChanges = true;
            var t = PendingChanges;
            if (t != null)
                t(this, EventArgs.Empty);
        }

        public event EventHandler PendingStatusChanges;
        bool bPendingStatusChanges;
        void OnPendingStatusChanges()
        {
            if (!bEnableEvents || bPendingStatusChanges)
                return;

#if DEBUGTRACE
            
            ("Sending PendigStatusChanges");
#endif

            bPendingStatusChanges = true;
            var t = PendingStatusChanges;
            if (t != null)
                t(this, EventArgs.Empty);
        }
        #endregion

        #region IDisposable
        bool bDisposed;
        public void Dispose()
        {
            lock (mapIdSinks)
            {
                if (mapIdSinks.ContainsKey(ClientId))
                    mapIdSinks.Remove(ClientId);
            }

            lock (lockObject)
            {
                if (bDisposed)
                    return;
                bDisposed = true;

                if (timer != null)
                {
                    timer.Stop();
                    timer.Tick -= timer_Tick;
                    timer = null;
                }

                EnsureReleaseLicense();

                if (thread == null || !thread.IsAlive)
                    return;
            }

            dispatcher.Dispatcher.InvokeIfRequired(() =>
            {
                foreach (var uri in mapUriToCanvas.Keys)
                {
                    if (!mapUriToDocuments.ContainsKey(uri))
                        continue;
                    var canvas = mapUriToCanvas[uri];
                    var Document = mapUriToDocuments[uri];

                    var stringmanager = Document.GetService(typeof(IStringEditorManager)) as IStringEditorManager;
                    if (stringmanager != null)
                        stringmanager.ClearActiveCulture(Document);

                    SaveData(GetUniqueTitle(Document), Document);

                    foreach (var key in Document.MapScreenEntities.Keys)
                        Document.MapScreenEntities[key].PropertyChanged -= ScreenSink_PropertyChanged;

                    if (gridContainer.Children.Contains(canvas))
                        gridContainer.Children.Remove(canvas);

                    Document.GetTagList -= Document_GetTagList;
                    Document.GetPrototypeList -= Document_GetPrototypeList;
                    Document.GetTagEntityReference -= Document_GetTagEntityReference;

                    if (!Document.TerminateScriptCode())
                    {
                        AddDocumentToBeDisposed(Document, canvas);
                    }
                    else
                    {
                        Document.TerminateExecution(canvas);
                        Document.UnloadResources(canvas);

                        Document.Dispose();
                        canvas.Dispose();
                    }
                }

                foreach (var element in mapDataContextElementToGuid.Keys)
                {
                    element.DataContextChanged -= datacontextChild_DataContextChanged;
                }

                try
                {
                    RealTimeConnectionManagerViewModel.SetUserIdentity(ClientSessionName, null, new StringCollection());
                }
                catch (Exception ex)
                {
                    log.Warn(String.Format(Properties.Resources.FailedToActivateUserOnServer, ex.Message));
                }

                mapDataContextElementToGuid.Clear();
                mapOriginalBound.Clear();

                mapUriToEmbeddeds.Clear();
                mapUriToCanvas.Clear();
                mapUriToCanvasBackground.Clear();
                mapUriToDocuments.Clear();
                mapGuidToElementUpdateOnly.Clear();
                mapZoomVisibilityEntities.Clear();
                dispatcher.Close();
            });

            //thread.Join();
            while (!bThreadTerminated)
                Thread.Sleep(500);

            thread = null;

            mapUriToEmbeddeds.Clear();
            mapUriToCanvas.Clear();
            mapUriToCanvasBackground.Clear();
            mapUriToBackground.Clear();
            mapUriToBackgroundChildren.Clear();
            mapGuidToElement.Clear();
            mapElementToGuid.Clear();
            mapElementCommandToName.Clear();
            mapElementCommandToDoc.Clear();
            mapGuidToBase64Element.Clear();
            mapGuidToElementPos.Clear();
            mapGuidToDataContextItem.Clear();
            mapGuidToParentCanvas.Clear();
            mapGuidToElementStatus.Clear();
            listChanged.Clear();
            listChangedPending.Clear();
            listStatusChanged.Clear();
            listStatusChangedPending.Clear(); 

#if DEBUGTRACE
            System.GC.Collect();
#endif
        }
        #endregion

        #region Storage

        static IsolatedStorageFile GetStorage()
        {
            try
            {
                return IsolatedStorageFile.GetStore(IsolatedStorageScope.User | IsolatedStorageScope.Assembly, null, null);
            }
            catch
            { }

            return null;
        }

        static String GetUniqueTitle(IDocument doc)
        {
            if (doc.Parent == null)
                return doc.Title;
            return String.Format("{0}_{1}", doc.Parent.Title, doc.Title);
        }

        static String GetStoreDataFileName(String title)
        {
            return String.Format("{0}.{1}.Data.dat", title, System.IO.Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().Location));
        }

        void SaveData(String title, ScreenDocument Document)
        {
            try
            {
                var isoStorage = GetStorage();
                if (null == isoStorage || string.IsNullOrEmpty(title) || Document == null)
                    return;

                using (var stream = new IsolatedStorageFileStream(GetStoreDataFileName(title), FileMode.Create, isoStorage))
                {
                    XmlWriterSettings settings = new XmlWriterSettings
                    {
                        Indent = true,
                        OmitXmlDeclaration = false,
                        Encoding = Encoding.UTF8
                    };

                    using (XmlWriter writer = XmlWriter.Create(stream, settings))
                    {
                        try
                        {
                            var serializer = new DataContractSerializer(typeof(IDictionary<String, Object>));
                            serializer.WriteObject(writer, Document.dataContextExpando as IDictionary<String, Object>);
                        }
                        catch (Exception ex)
                        {
                            writer.Close();
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }

        IDictionary<String, Object> LoadData(String title, ScreenDocument Document)
        {
            if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl) || Document == null)
                return null;

            try
            {
                var isoStorage = GetStorage();
                if (null != isoStorage && !String.IsNullOrEmpty(title))
                {
                    using (var stream = new IsolatedStorageFileStream(GetStoreDataFileName(title), FileMode.OpenOrCreate, isoStorage))
                    {
                        XmlReaderSettings settings = new XmlReaderSettings
                        {
                            ConformanceLevel = ConformanceLevel.Document,
                            CloseInput = true
                        };

                        using (XmlReader reader = XmlReader.Create(stream, settings))
                        {
                            var serializer = new DataContractSerializer(typeof(IDictionary<String, Object>));
                            var map = serializer.ReadObject(reader) as IDictionary<String, Object>;
                            var dataMap = Document.dataContextExpando as IDictionary<String, Object>;
                            foreach (var entry in map)
                            {
                                dataMap[entry.Key] = entry.Value;
                                if (!Document.mapDataContextExpando.ContainsKey(entry.Key))
                                {
                                    var mi = new MonitoredItemViewModel(true);
                                    mi.DataValue = new DataValue(new Variant(entry.Value), StatusCodes.BadWaitingForInitialData);
                                    Document.mapDataContextExpando.Add(entry.Key, mi);
                                }
                            }
                            return map;
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }

            return null;
        }
        #endregion
    }
}
