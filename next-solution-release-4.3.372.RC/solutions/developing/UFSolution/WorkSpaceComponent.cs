using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using Syncfusion.Windows.Tools.Controls;
using UFInterfaces;
using UFInterfaces.CoreHostComponents;
using DocumentManager.ComponentService;
using Utilities;
using Utilities.WPF;
using SmartTagsControl.ComponentService;

namespace UFSolution
{
    class WorkSpaceComponent : ComponentBase<IWorkspace>, /*IWorkspace,*/ IEditableObject, IDisposable
    {
        #region Declaration

        readonly UFMainWindow mainWindow;
        Object lockObject = new Object();
        Object PromoteSelectingObject;
        IList PromoteSelectingObjects;
        Object ContextSelectedObject;
        IList ContextSelectedObjects;
        DispatcherOperation IdleExecutionPending;
        DispatcherOperation IdleExecutionDocumentPending;
        IDocument PromoteSelectingDocument;
        IDocument ContextSelectedDocument;
        Object lastActive;
        #endregion Declaration

        public WorkSpaceComponent(UFMainWindow wnd)
        {
            mainWindow = wnd;
        }

        #region IWorkspace Members

        public bool IsBusy
        {
            get
            {
                if (bShuttingDown)
                    return false;
                bool bRet = false;
                mainWindow.Dispatcher.InvokeIfRequired(
                        () =>
                        {
                            bRet = mainWindow.IsBusyVisible();
                        });
                return bRet;
            }
            set
            {
                if (bShuttingDown)
                    return;
                mainWindow.Dispatcher.InvokeIfRequired(
                        () =>
                        {
                            mainWindow.SetBusy(value);
                        });
            }
        }

        public String BusyContent
        {
            get
            {
                String ret = String.Empty;
                if (bShuttingDown)
                    return ret;
                mainWindow.Dispatcher.InvokeIfRequired(
                        () =>
                        {
                            ret = mainWindow.GetBusyContent();
                        });
                return ret;
            }
            set
            {
                if (bShuttingDown)
                    return;
                mainWindow.Dispatcher.InvokeIfRequired(
                        () =>
                        {
                            if (mainWindow.GetBusyContent() != value)
                            {
                                mainWindow.SetBusyContent(value);
                            }
                        });
            }
        }


        public void AddDockingChildren(FrameworkElement el,
                                       String header,
                                       UFInterfaces.DockState state,
                                       UFInterfaces.DockSide side, bool bCanClose = true, bool bCanFloat = true)
        {
            if (bShuttingDown)
                return;

            mainWindow.Dispatcher.InvokeIfRequired(
            () =>
            {
                if (!mainWindow.dockingManager.Children.Contains(el))
                {
                    using (var cursor = new WaitCursor())
                    {
                        if (String.IsNullOrEmpty(el.Name))
                        {
                            try
                            {
                                el.Name = DependencyObjectExtensions.AdaptName(header);
                            }
                            catch { }
                        }

                        bool bFound = false;
                        var originalName = el.Name;
                        int i = 0;
                        while (true)
                        {
                            foreach (FrameworkElement item in mainWindow.dockingManager.Children)
                            {
                                if (item.Name == el.Name)
                                {
                                    bFound = true;
                                    el.Name = String.Format("{0}{1}", originalName, ++i);
                                    break;
                                }
                            }

                            if (!bFound)
                                break;
                            bFound = false;
                        }

                        DockingManager.SetState(el, (Syncfusion.Windows.Tools.Controls.DockState)state);
                        DockingManager.SetHeader(el, header);
                        DockingManager.SetSideInDockedMode(el, (Syncfusion.Windows.Tools.Controls.DockSide)side);
                        DockingManager.SetCanClose(el, bCanClose);
                        DockingManager.SetCanFloat(el, bCanFloat);
                        DockingManager.SetAnimateOnNewItemAdded(el, false);

                        mainWindow.AddDockingChild(el);

                        if (state == UFInterfaces.DockState.Document)
                        {
                            mainWindow.Dispatcher.BeginInvokeAsynchronouslyInApplicationIdle(
                            () =>
                            {
                                if (mainWindow.dockingManager.Children.Contains(el))
                                {
                                    ActiveWindow = el;
                                    el.Focusable = true;
                                    el.Focus();
                                    FocusManager.SetFocusedElement(el, el);
                                }
                            });
                        }
                        // (mainWindow.dockingManager.DocContainer as DocumentContainer).CreateHorizontalTabGroup(el);
                    }
                }
            });
        }

        public void RemoveDockingChildren(FrameworkElement el)
        {
            if (bShuttingDown)
                return;

            mainWindow.Dispatcher.InvokeIfRequired(
            () =>
            {
                using (var cursor = new WaitCursor())
                {
                    if (mainWindow.dockingManager.Children.Contains(el))
                    {
                        //mainWindow.dockingManager.UpdateLayout();
                        //mainWindow.dockingManager.BeginInit();
                        mainWindow.dockingManager.Children.Remove(el);
                        //mainWindow.dockingManager.EndInit();
                    }

                    mainWindow.SaveCurrentWorkspace(el);

                    if (el is IDisposable)
                    {
                        //var disposables = (from c in el.GetChildrenOfType<FrameworkElement>() where c is IDisposable select c).ToList();

                        //disposables.ForEach(c =>
                        //{
                        //    (c as IDisposable).Dispose();
                        //});

                        IDisposable dispose = el as IDisposable;
                        dispose.Dispose();

                        DependencyObjectExtensions.CleanChildrenOfTypeCache();
                    }
                }
            });
        }

        public UFInterfaces.DockState GetElementDockState(FrameworkElement el)
        {
            if (bShuttingDown)
                return UFInterfaces.DockState.Dock;
            return (UFInterfaces.DockState)DockingManager.GetState(el);
        }

        public void SetElementDockState(FrameworkElement el, UFInterfaces.DockState dockState)
        {
            if (bShuttingDown)
                return;
            DockingManager.SetState(el, (Syncfusion.Windows.Tools.Controls.DockState)dockState);
        }

        public UFInterfaces.DockSide GetElementDockSide(FrameworkElement el)
        {
            if (bShuttingDown)
                return UFInterfaces.DockSide.None;
            try
            {
                return (UFInterfaces.DockSide)DockingManager.GetSide(el, DockingManager.GetState(el));
            }
            catch (Exception ex)
            {
                return UFInterfaces.DockSide.None;
            }
        }

        FrameworkElement dockedElement;
        public void FlashDockedElement(FrameworkElement element)
        {
            if (bShuttingDown)
                return;
            mainWindow.Dispatcher.InvokeIfRequired(
            () =>
            {
                var state = (UFInterfaces.DockState)DockingManager.GetState(element);

                bool bForceSendDockingBogus = false;
                if (state == UFInterfaces.DockState.AutoHidden)
                {
                    dockedElement = element;
                    DockingManager.SetState(element, Syncfusion.Windows.Tools.Controls.DockState.Dock);
                    mainWindow.dockingManager.ActiveWindowChanged += DockingManager_ActiveWindowChanged;
                    bForceSendDockingBogus = true;
                    // DockingManager.AutoHideTab(element);
                }
                else if (state == UFInterfaces.DockState.Hidden)
                    mainWindow.dockingManager.RestoreElement(element);

                DockingManager.SelectTab(element);

                //if (state == DockState.AutoHidden)
                //    DockingManager.SetState(element, DockState.Dock);

                // mainWindow.dockingManager.ActiveWindow = element;
                mainWindow.dockingManager.ActivateWindow(element.Name);

                element.Focusable = true;
                element.Focus();
                FocusManager.SetFocusedElement(element, element);

                mainWindow.ForceAnimationStopEventDockingBogus(element);
                //if (bForceSendDockingBogus)
                //    mainWindow.ForceAnimationStopEventDockingBogus(element);

                mainWindow.Refresh();
            });
        }

        private void DockingManager_ActiveWindowChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {   
            if (mainWindow.dockingManager.ActiveWindow != dockedElement)
            {
                mainWindow.dockingManager.ActiveWindowChanged -= DockingManager_ActiveWindowChanged;
                DockingManager.SetState(dockedElement, Syncfusion.Windows.Tools.Controls.DockState.AutoHidden);
            }
        }

        public void SetDesiredHeightAndWidthInDockedMode(FrameworkElement el, double Height, double Width)
        {
            if (bShuttingDown)
                return;
            if (!Double.IsNaN(Height))
            {
                DockingManager.SetDesiredHeightInDockedMode(el, Height);
                DockingManager.SetDesiredHeightInFloatingMode(el, Height);
            }
            if (!Double.IsNaN(Width))
            {
                DockingManager.SetDesiredWidthInDockedMode(el, Width);
                DockingManager.SetDesiredWidthInFloatingMode(el, Width);
            }
        }

        public void SetDesiredSideMode(FrameworkElement el, String target)
        {
            if (bShuttingDown)
                return;
            var name = DependencyObjectExtensions.AdaptName(target);
            var parentEl = GetElementFromName(name);
            if (parentEl != null)
            {
                var dockstate = (UFInterfaces.DockState)DockingManager.GetState(parentEl);
                if (dockstate != UFInterfaces.DockState.Dock)
                {
                    DockingManager.SetState(parentEl, Syncfusion.Windows.Tools.Controls.DockState.Dock);
                }

                DockingManager.SetTargetNameInDockedMode(el, name);
            }
        }

        public void SetDockedElementIcon(FrameworkElement el, Brush b)
        {
            if (bShuttingDown)
                return;
            DockingManager.SetIcon(el, b);
        }

        readonly static String SuffixChangedDocument = "*";

        public void SetChangedDocumentTitle(FrameworkElement el, bool bSet)
        {
            if (bShuttingDown)
                return;
            mainWindow.Dispatcher.InvokeIfRequired(
            () =>
            {
                String title = DockingManager.GetHeader(el) as String;
                if (title == null)
                    return;
                if (bSet && !title.Contains(SuffixChangedDocument))
                {
                    title += SuffixChangedDocument;
                    DockingManager.SetHeader(el, title);
                }
                else if (!bSet && title.Contains(SuffixChangedDocument))
                {
                    title = title.Trim(SuffixChangedDocument.ToCharArray());
                    DockingManager.SetHeader(el, title);
                }
            });
        }

        public void SetChangedDocumentTitle(FrameworkElement el, String title, bool bSet)
        {
            if (bShuttingDown)
                return;
            mainWindow.Dispatcher.InvokeIfRequired(
            () =>
            {
                if (bSet)
                {
                    title += SuffixChangedDocument;
                    DockingManager.SetHeader(el, title);
                }
                else
                {
                    title = title.Trim(SuffixChangedDocument.ToCharArray());
                    DockingManager.SetHeader(el, title);
                }
            });
        }

        FrameworkElement GetElementFromName(String name)
        {
            if (bShuttingDown)
                return null;

            FrameworkElement element = null;
            foreach (FrameworkElement child in mainWindow.dockingManager.Children)
            {
                if (child.Name != name)
                    continue;

                element = child;
                break;
            }
            return element;
        }

        FrameworkElement GetElement(String header)
        {
            if (bShuttingDown)
                return null;

            FrameworkElement element = null;
            foreach (FrameworkElement child in mainWindow.dockingManager.Children)
            {
                String title = DockingManager.GetHeader(child) as String;

                if (title == null || title.Trim(SuffixChangedDocument.ToCharArray()) != header)
                    continue;

                element = child;
                break;
            }
            return element;
        }

        public void FlashDockedElement(String header)
        {
            if (bShuttingDown)
                return;

            mainWindow.Dispatcher.BeginInvoke(
            (Action)(() =>
            {
                //if (!mainWindow.IsActive)
                //    return;

                FrameworkElement element = GetElement(header);
                if (element != null)
                {
                    var state = (UFInterfaces.DockState)DockingManager.GetState(element);

                    if (state == UFInterfaces.DockState.AutoHidden)
                        DockingManager.AutoHideTab(element);
                    else if (state == UFInterfaces.DockState.Hidden)
                        mainWindow.dockingManager.RestoreElement(element);

                    DockingManager.SelectTab(element);

                    if (state == UFInterfaces.DockState.AutoHidden)
                        DockingManager.SetState(element, Syncfusion.Windows.Tools.Controls.DockState.Dock);

                    // mainWindow.dockingManager.ActiveWindow = element;
                    mainWindow.dockingManager.ActivateWindow(element.Name);
                    element.Focusable = true;
                    element.Focus();
                    FocusManager.SetFocusedElement(element, element);
                }
            }), DispatcherPriority.Background);
        }

        public void HideAllAutoHideWindows()
        {
            if (bShuttingDown)
                return;
            (from c in mainWindow.dockingManager.Children.OfType<FrameworkElement>()
                        where (UFInterfaces.DockState)DockingManager.GetState(c) == UFInterfaces.DockState.AutoHidden
                        select c).ToList().ForEach(child =>
                     {
                         DockingManager.AutoHideTab(child);
                     });
            //foreach (FrameworkElement child in mainWindow.dockingManager.Children)
            //{
            //    if (DockingManager.GetState(child) == DockState.AutoHidden)
            //        DockingManager.AutoHideTab(child);
            //}
        }

        public void ActivateDockedElement(String header)
        {
            if (bShuttingDown)
                return;
            mainWindow.Dispatcher.BeginInvokeAsynchronouslyInBackground(
            () =>
            {
                HideAllAutoHideWindows();
            });

            FlashDockedElement(header);
        }

        public void ActivateDockedElement(FrameworkElement el)
        {
            if (bShuttingDown)
                return;
            mainWindow.Dispatcher.BeginInvokeAsynchronouslyInBackground(
            () =>
            {
                HideAllAutoHideWindows();
            });

            FlashDockedElement(el);
        }

        public void ActivatePreviousActiveElement()
        {
            if (bShuttingDown)
                return;
            var element = lastActive as FrameworkElement;
            if (element == null)
                return;
            ActivateDockedElement(element);
        }

        public bool HasContentRendered()
        {
            if (bShuttingDown)
                return false;
            return mainWindow.HasContentRendered();
        }

        public void ShowTaskBarTooltip(String Title, String Content, int nTimeout)
        {
            if (bShuttingDown)
                return;
            mainWindow.Dispatcher.InvokeIfRequired(
            () =>
            {
                mainWindow.notifyIcon.BalloonTipText = Content;
                mainWindow.notifyIcon.BalloonTipTitle = Title;
                if (!String.IsNullOrEmpty(mainWindow.notifyIcon.BalloonTipText))
                    mainWindow.notifyIcon.ShowBalloonTip(nTimeout);
            });
        }


        public event EventHandler EasyModeChanged;
        virtual public void OnEasyModeChanged(Object sender)
        {
            EventHandler temp = EasyModeChanged;
            if (temp != null)
                temp(sender, EventArgs.Empty);
        }

        public bool IsInEasyMode
        {
            get
            {
                return mainWindow.IsInEasyMode;
            }
        }

        public event EventHandler WorkspaceLoading;

        virtual public void OnWorkspaceLoading(Object sender)
        {
            EventHandler temp = WorkspaceLoading;
            if (temp != null)
                temp(sender, EventArgs.Empty);
        }

        public event EventHandler WorkspaceLoaded;

        virtual public void OnWorkspaceLoaded(Object sender)
        {
            EventHandler temp = WorkspaceLoaded;
            if (temp != null)
                temp(sender, EventArgs.Empty);
        }

        public event EventHandler ContentRendered;

        virtual public void OnContentRendered(Object sender)
        {
            EventHandler temp = ContentRendered;
            if (temp != null)
                temp(sender, EventArgs.Empty);
        }

        public event EventHandler<CancelEventArgs> Closing;

        virtual public void OnClosing(Object sender, CancelEventArgs e)
        {
            EventHandler<CancelEventArgs> temp = Closing;
            if (temp != null)
                temp(sender, e);
        }

        public event EventHandler Closed;

        virtual public void OnClosed(Object sender)
        {
            EventHandler temp = Closed;
            if (temp != null)
                temp(sender, EventArgs.Empty);
        }

        public event RoutedEventHandler WindowActivated;

        virtual public void OnWindowActivated(Object sender, RoutedEventArgs e)
        {
            RoutedEventHandler temp = WindowActivated;
            if (temp != null)
                temp(sender, e);
        }

        public event RoutedEventHandler WindowDeactivated;

        virtual public void OnWindowDeactivated(Object sender, RoutedEventArgs e)
        {
            RoutedEventHandler temp = WindowDeactivated;
            if (temp != null)
                temp(sender, e);
        }

        public event RoutedEventHandler WindowDragEnd;

        virtual public void OnWindowDragEnd(Object sender, RoutedEventArgs e)
        {
            RoutedEventHandler temp = WindowDragEnd;
            if (temp != null)
                temp(sender, e);
        }

        public event RoutedEventHandler WindowDragStart;

        virtual public void OnWindowDragStart(Object sender, RoutedEventArgs e)
        {
            RoutedEventHandler temp = WindowDragStart;
            if (temp != null)
                temp(sender, e);
        }

        public event RoutedEventHandler WindowVisibilityChanged;

        virtual public void OnWindowVisibilityChanged(Object sender, RoutedEventArgs e)
        {
            RoutedEventHandler temp = WindowVisibilityChanged;
            if (temp != null)
                temp(sender, e);
        }

        public event RoutedEventHandler BeforeContextMenuOpen;

        virtual public void OnBeforeContextMenuOpen(Object sender, RoutedEventArgs e)
        {
            RoutedEventHandler temp = BeforeContextMenuOpen;
            if (temp != null)
                temp(sender, e);
        }

        public event RoutedEventHandler AutoHideAnimationStart;

        virtual public void OnAutoHideAnimationStart(Object sender, RoutedEventArgs e)
        {
            RoutedEventHandler temp = AutoHideAnimationStart;
            if (temp != null)
                temp(sender, e);
        }

        public event RoutedEventHandler AutoHideAnimationStop;

        virtual public void OnAutoHideAnimationStop(Object sender, RoutedEventArgs e)
        {
            RoutedEventHandler temp = AutoHideAnimationStop;
            if (temp != null)
                temp(sender, e);
        }

        public event EventHandler<CancelEventArgs> ActiveWindowChanging;

        virtual public void OnActiveWindowChanging(DependencyObject d, ActiveWindowChangingEventArgs e)
        {
            CancelEventArgs cea = new CancelEventArgs();
            EventHandler<CancelEventArgs> temp = ActiveWindowChanging;
            if (temp != null)
                temp(d, cea);
            e.Cancel = cea.Cancel;
        }

        public event PropertyChangedCallback ActiveWindowChanged;

        virtual public void OnActiveWindowChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue is FrameworkElement)
            {
                var state = GetElementDockState(e.OldValue as FrameworkElement);
                if (state == UFInterfaces.DockState.Document)
                    lastActive = e.OldValue;
            }
            PropertyChangedCallback temp = ActiveWindowChanged;
            if (temp != null)
                temp(d, e);
        }

        public event Syncfusion.Windows.Tools.Controls.OnCloseTabsEventHandler CloseAllTabs;

        virtual public void OnCloseAllTabs(object sender, CloseTabEventArgs e)
        {
            Syncfusion.Windows.Tools.Controls.OnCloseTabsEventHandler temp = CloseAllTabs;
            if (temp != null)
                temp(sender, e);
            lastActive = null;
        }

        public event Syncfusion.Windows.Tools.Controls.CloseButtonEventHandler CloseButtonClick;

        virtual public void OnCloseButtonClick(object sender, Syncfusion.Windows.Tools.Controls.CloseButtonEventArgs e)
        {
            Syncfusion.Windows.Tools.Controls.CloseButtonEventHandler temp = CloseButtonClick;
            if (temp != null)
                temp(sender, e);

            if (e.TargetItem == lastActive)
                lastActive = null;
        }

        public event Syncfusion.Windows.Tools.Controls.OnCloseTabsEventHandler CloseOtherTabs;

        virtual public void OnCloseOtherTabs(object sender, CloseTabEventArgs e)
        {
            Syncfusion.Windows.Tools.Controls.OnCloseTabsEventHandler temp = CloseOtherTabs;
            if (temp != null)
                temp(sender, e);

            if (e.TargetTabItem == lastActive)
                lastActive = null;
        }

        public event Syncfusion.Windows.Tools.Controls.DockStateHandler DockStateChanged;

        virtual public void OnDockStateChanged(FrameworkElement sender, Syncfusion.Windows.Tools.Controls.DockStateEventArgs e)
        {
            Syncfusion.Windows.Tools.Controls.DockStateHandler temp = DockStateChanged;
            if (temp != null)
                temp(sender, e);
        }

        public event Syncfusion.Windows.Tools.Controls.ElementHiddenEventHandler ElementHidden;

        virtual public void OnElementHidden(object sender)
        {
            Syncfusion.Windows.Tools.Controls.ElementHiddenEventHandler temp = ElementHidden;
            if (temp != null)
                temp(sender);
        }

        public event Syncfusion.Windows.Tools.Controls.ElementShownEventHandler ElementShown;

        virtual public void OnElementShown(object sender)
        {
            Syncfusion.Windows.Tools.Controls.ElementShownEventHandler temp = ElementShown;
            if (temp != null)
                temp(sender);
        }

        public event SelectionChangedEventHandler RibbonSelectionChanged;

        virtual public void OnRibbonSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectionChangedEventHandler temp = RibbonSelectionChanged;
            if (temp != null)
                temp(sender, e);
        }

        public void AddMenuItem(MenuItem item, int nPos, CommandBindingCollection commandBinding)
        {
            if (bShuttingDown)
                return;
            mainWindow.Dispatcher.InvokeIfRequired(
            () =>
            {
                if (mainWindow.MainMenu.Items.Contains(item))
                    return;

                if (commandBinding != null)
                    mainWindow.CommandBindings.AddRange(commandBinding);

                if (mainWindow.MainMenu.Items.Count < 1)
                    mainWindow.MainMenu.Items.Add(item);
                else
                {
                    nPos = nPos < 0 || nPos > mainWindow.MainMenu.Items.Count ? mainWindow.MainMenu.Items.Count : nPos;
                    mainWindow.MainMenu.Items.Insert(nPos, item);
                }
            });
        }

        public void RemoveMenuItem(MenuItem item, CommandBindingCollection commandBinding)
        {
            if (bShuttingDown)
                return;
            mainWindow.Dispatcher.InvokeIfRequired(
            () =>
            {
                if (mainWindow.MainMenu.Items.Contains(item))
                    mainWindow.MainMenu.Items.Remove(item);

                if (commandBinding != null)
                {
                    foreach (CommandBinding cb in commandBinding)
                    {
                        if (mainWindow.CommandBindings.Contains(cb))
                            mainWindow.CommandBindings.Remove(cb);
                    }
                }
            });
        }

        class RibbonChangingRequest
        {
            public List<Syncfusion.Windows.Tools.Controls.RibbonTab> list;
            public String contextTabGroup;
            public Color backColor;
            public int nPos;
            public CommandBindingCollection commandBinding;

            public sealed override bool Equals(object obj)
            {
                if (object.ReferenceEquals(this, obj))
                    return true;

                RibbonChangingRequest control = obj as RibbonChangingRequest;
                if (control.list != null && list == null)
                    return false;

                if (control.list != null && list != null)
                {
                    foreach (var v in control.list)
                    {
                        if (!list.Contains(v))
                            return false;
                    }
                }

                return true;
            }
        }

        List<RibbonChangingRequest> AddPendingRibbonRequests = new List<RibbonChangingRequest>();
        List<RibbonChangingRequest> RemovePendingRibbonRequests = new List<RibbonChangingRequest>();
        DispatcherOperation IdleExecutionPendingRibbons;
        Dictionary<Syncfusion.Windows.Tools.Controls.RibbonBar, List<Syncfusion.Windows.Tools.Controls.RibbonButton>> addedRibbonBarQAMA = new Dictionary<Syncfusion.Windows.Tools.Controls.RibbonBar, List<Syncfusion.Windows.Tools.Controls.RibbonButton>>();
        Dictionary<Syncfusion.Windows.Tools.Controls.RibbonBar, List<Syncfusion.Windows.Tools.Controls.RibbonButton>> RemovedRibbonBarQAMA = new Dictionary<Syncfusion.Windows.Tools.Controls.RibbonBar, List<Syncfusion.Windows.Tools.Controls.RibbonButton>>();

        bool bShuttingDown;
        internal void Shutdown()
        {
            bShuttingDown = true;
        }

        void IdleExecutionRibbonPending()
        {
            if (bShuttingDown)
                return;

            //mainWindow.Ribbons.UpdateLayout();
            //mainWindow.Ribbons.BeginInit();

            var cleanlist = new List<RibbonChangingRequest>();
            AddPendingRibbonRequests.ForEach(check =>
                {
                    if (RemovePendingRibbonRequests.Contains(check))
                    {
                        cleanlist.Add(check);
                    }

                    if (check.commandBinding != null)
                    {
//#if DEBUG
//                        var start = check.commandBinding.Count;
//#endif
                        foreach (CommandBinding cb in check.commandBinding)
                        {
                            if (mainWindow.CommandBindings.Contains(cb))
                                mainWindow.CommandBindings.Remove(cb);
                            mainWindow.CommandBindings.Insert(0, cb);

                            RoutedUICommand rc = cb.Command as RoutedUICommand;
                            if (rc == null)
                                continue;

                            RibbonCommandManager.Unregister(cb.Command);
                            if (!String.IsNullOrEmpty(rc.Text))
                                RibbonCommandManager.Register(cb.Command, new RibbonCommandProvider(rc.Text.Replace("_", "")));
                        }
//#if DEBUG
//                        var diff = check.commandBinding.Count - start;
//                        System.Diagnostics.Debug.WriteLine(String.Format("mainWindow.CommandBindings : New = {0}, Total = {1}", diff, check.commandBinding.Count));
//#endif
                    }
                });

            RemovePendingRibbonRequests.ForEach(r =>
            {
                if (!cleanlist.Contains(r))
                {
                    r.list.ForEach(item =>
                    {
                        foreach (var bar in item.Items)
                        {
                            if (bar is Syncfusion.Windows.Tools.Controls.RibbonBar)
                            {
                                Syncfusion.Windows.Tools.Controls.RibbonBar ribbonBar = bar as Syncfusion.Windows.Tools.Controls.RibbonBar;
                                if (!addedRibbonBarQAMA.ContainsKey(ribbonBar))
                                    continue;

                                addedRibbonBarQAMA[ribbonBar].ForEach(button =>
                                {
                                    try
                                    {
                                        if (mainWindow.Ribbons.QuickAccessToolBar.Items.Contains(button))
                                            mainWindow.Ribbons.QuickAccessToolBar.Items.Remove(button);
                                        else if (mainWindow.Ribbons.ApplicationMenu.Items.Contains(button))
                                            mainWindow.Ribbons.ApplicationMenu.Items.Remove(button);
                                        //else if (mainWindow.RibbonBackStage.Items.Contains(button))
                                        //    mainWindow.RibbonBackStage.Items.Remove(button);
                                    }
                                    catch { }
                                });
                                if (!RemovedRibbonBarQAMA.ContainsKey(ribbonBar))
                                    RemovedRibbonBarQAMA.Add(ribbonBar, addedRibbonBarQAMA[ribbonBar]);
                                addedRibbonBarQAMA.Remove(ribbonBar);
                            }
                        }

                        if (!String.IsNullOrEmpty(r.contextTabGroup))
                        {
                            var group = (from c in mainWindow.Ribbons.ContextTabGroups where c.Label == r.contextTabGroup select c).ToList();
                            if (group.Count >= 1 && mainWindow.Ribbons.Items.Contains(item))
                            {
                                group[0].IsGroupVisible = false;
                                //mainWindow.Ribbons.SelectedIndex = 0;
                                //mainWindow.Ribbons.Items.Remove(item);
                                //group[0].RibbonTabs.Remove(item);
                                //if (group[0].RibbonTabs.Count == 0)
                                //{
                                //    mainWindow.Ribbons.ContextTabGroups.Remove(group[0]);
                                //}
                            }
                        }
                        else if (mainWindow.Ribbons.Items.Contains(item))
                        {
                            mainWindow.Ribbons.SelectedIndex = 0;
                            mainWindow.Ribbons.Items.Remove(item);
                        }
                    });
                }

                if (r.commandBinding != null)
                {
//#if DEBUG
//                    var start = r.commandBinding.Count;
//#endif
                    foreach (CommandBinding cb in r.commandBinding)
                    {
                        if (mainWindow.CommandBindings.Contains(cb))
                            mainWindow.CommandBindings.Remove(cb);

                        RibbonCommandManager.Unregister(cb.Command);
                    }
//#if DEBUG
//                    var diff = start - r.commandBinding.Count;
//                    System.Diagnostics.Debug.WriteLine(String.Format("mainWindow.CommandBindings : New = {0}, Total = {1}", diff, r.commandBinding.Count));
//#endif
                }
            });

            RemovePendingRibbonRequests.Clear();

            // mainWindow.Ribbons.EndInit();

            var grouptoActivate = (from c in mainWindow.Ribbons.ContextTabGroups where c.IsGroupVisible == true select c).ToList();
            if (grouptoActivate.Count > 0)
            {
                grouptoActivate[grouptoActivate.Count - 1].Activate();
            }

            mainWindow.Ribbons.UpdateLayout();
            mainWindow.Ribbons.BeginInit();

            var listToActivate = new List<Syncfusion.Windows.Tools.Controls.RibbonTab>();
            var listToChecked = new List<Syncfusion.Windows.Tools.Controls.RibbonTab>();
            AddPendingRibbonRequests.ForEach(r =>
                {
                    if (!cleanlist.Contains(r))
                    {
                        r.list.ForEach(item =>
                        {
                            if (item.IsChecked && !listToChecked.Contains(item))
                                listToChecked.Add(item);
                        });

                        r.list.ForEach(item =>
                           {
                               var listQA = new List<Syncfusion.Windows.Tools.Controls.RibbonButton>();
                               var mapQA = new Dictionary<Syncfusion.Windows.Tools.Controls.RibbonButton, Syncfusion.Windows.Tools.Controls.RibbonBar>();
                               var listMA = new List<Syncfusion.Windows.Tools.Controls.RibbonButton>();
                               var mapMA = new Dictionary<Syncfusion.Windows.Tools.Controls.RibbonButton, Syncfusion.Windows.Tools.Controls.RibbonBar>();
                               var listBS = new List<Syncfusion.Windows.Tools.Controls.RibbonButton>();
                               var mapBS = new Dictionary<Syncfusion.Windows.Tools.Controls.RibbonButton, Syncfusion.Windows.Tools.Controls.RibbonBar>();

                               ExtractRibbonItems(item, listQA, mapQA, listMA, mapMA, listBS, mapBS);

                               listQA.ForEach(button =>
                                   {
                                       try
                                       {
                                           mapQA[button].Items.Remove(button);
                                           if (!mainWindow.Ribbons.QuickAccessToolBar.Items.Contains(button))
                                               mainWindow.Ribbons.QuickAccessToolBar.Items.Add(button);
                                       }
                                       catch { }
                                   });

                               listMA.ForEach(button =>
                               {
                                   try
                                   {
                                       mapMA[button].Items.Remove(button);
                                       if (!mainWindow.Ribbons.ApplicationMenu.Items.Contains(button))
                                           mainWindow.Ribbons.ApplicationMenu.Items.Add(button);
                                   }
                                   catch { }
                               });

                               listBS.ForEach(button =>
                               {
                                   try
                                   {
                                       mapBS[button].Items.Remove(button);
                                       //mainWindow.RibbonBackStage.Items.Add(button);
                                   }
                                   catch { }
                               });

                               if (!String.IsNullOrEmpty(r.contextTabGroup))
                               {
                                   ContextTabGroup contextTabGroup;
                                   var group = (from c in mainWindow.Ribbons.ContextTabGroups where c.Label == r.contextTabGroup select c).ToList();
                                   if (group.Count == 0)
                                   {
                                       contextTabGroup = new ContextTabGroup()
                                                             {
                                                                 Label = r.contextTabGroup,
                                                                 BackColor = r.backColor
                                                             };
                                       mainWindow.Ribbons.ContextTabGroups.Add(contextTabGroup);
                                   }
                                   else
                                       contextTabGroup = group[0];

                                   if (!mainWindow.Ribbons.Items.Contains(item) &&
                                       !contextTabGroup.RibbonTabs.Contains(item))
                                   {
                                       if (contextTabGroup.RibbonTabs.Count < 1)
                                       {
                                           contextTabGroup.RibbonTabs.Add(item);
                                       }
                                       else
                                       {
                                           r.nPos = r.nPos < 0 || r.nPos > contextTabGroup.RibbonTabs.Count ? contextTabGroup.RibbonTabs.Count : r.nPos;
                                           try
                                           {
                                               contextTabGroup.RibbonTabs.Insert(r.nPos, item);
                                           }
                                           catch
                                           {
                                               try
                                               {
                                                   contextTabGroup.RibbonTabs.Add(item);
                                               }
                                               catch
                                               { }
                                           }
                                       }

                                       if (!listToActivate.Contains(item))
                                           listToActivate.Add(item);
                                   }

                                    contextTabGroup.IsGroupVisible = true;
                                    contextTabGroup.Activate();
                               }
                               else if (!mainWindow.Ribbons.Items.Contains(item))
                               {
                                   if (mainWindow.Ribbons.Items.Count < 1)
                                   {
                                       mainWindow.Ribbons.Items.Add(item);
                                       //int nSelectedIndex = mainWindow.Ribbons.SelectedIndex;
                                       //mainWindow.Ribbons.SelectedIndex = mainWindow.Ribbons.Items.Count - 1;
                                       //mainWindow.Ribbons.SelectedIndex = nSelectedIndex;
                                   }
                                   else
                                   {
                                       r.nPos = r.nPos < 0 || r.nPos > mainWindow.Ribbons.Items.Count ? mainWindow.Ribbons.Items.Count : r.nPos;
                                       try
                                       {
                                           mainWindow.Ribbons.Items.Insert(r.nPos, item);
                                       }
                                       catch
                                       {
                                           try
                                           {
                                               mainWindow.Ribbons.Items.Add(item);
                                           }
                                           catch
                                           { }
                                       }
                                       // mainWindow.Ribbons.SelectedIndex = r.nPos;
                                   }

                                   if (!listToActivate.Contains(item))
                                       listToActivate.Add(item);
                               }
                               //else if (mainWindow.Ribbons.Items.Contains(item))
                               //{
                               //    mainWindow.Ribbons.SelectedIndex = mainWindow.Ribbons.Items.IndexOf(item);
                               //}

                               //mainWindow.Ribbons.SelectedItem = null;
                               //WaitForPriority.Wait(DispatcherPriority.ApplicationIdle, mainWindow);
                           });
                    }
                });

            AddPendingRibbonRequests.Clear();

            mainWindow.Ribbons.EndInit();

            grouptoActivate = (from c in mainWindow.Ribbons.ContextTabGroups where c.IsGroupVisible == true select c).ToList();
            if (grouptoActivate.Count > 0)
            {
                grouptoActivate[grouptoActivate.Count - 1].Activate();
            }

            if (listToChecked.Count > 0)
            {
                var list = new List<Syncfusion.Windows.Tools.Controls.RibbonTab>(listToChecked.Count);
                list.AddRange(listToChecked);
                listToChecked.Clear();
                mainWindow.Dispatcher.BeginInvoke(DispatcherPriority.Background, (Action)(() =>
                {
                    foreach (Syncfusion.Windows.Tools.Controls.RibbonTab item in mainWindow.Ribbons.Items)
                        item.IsChecked = false;
                    list.ForEach((item) => item.IsChecked = true);
                }));
            }

            if (listToActivate.Count > 0)
            {
                var activate = listToActivate[0];
                mainWindow.Dispatcher.BeginInvoke(DispatcherPriority.Background, (Action)(() =>
                {
                    mainWindow.Ribbons.SelectedItem = activate;
                }));
                // WaitForPriority.Wait(DispatcherPriority.Background, mainWindow);
                listToActivate.Clear();
            }

            mainWindow.Dispatcher.BeginInvoke(DispatcherPriority.Background, (Action)(() =>
            {
                if (mainWindow.Ribbons.RibbonState == Syncfusion.Windows.Tools.RibbonState.Adorner)
                {
                    mainWindow.Ribbons.RibbonState = Syncfusion.Windows.Tools.RibbonState.Hide;
                }
            }));

            // Dirty the commands registered with CommandManager,
            // such as our Save command, so that they are queried
            // to see if they can execute now.
            CommandManager.InvalidateRequerySuggested();
        }

        private void ExtractRibbonItems(ItemsControl item,
                                        List<Syncfusion.Windows.Tools.Controls.RibbonButton> listQA, Dictionary<Syncfusion.Windows.Tools.Controls.RibbonButton, Syncfusion.Windows.Tools.Controls.RibbonBar> mapQA,
                                        List<Syncfusion.Windows.Tools.Controls.RibbonButton> listMA, Dictionary<Syncfusion.Windows.Tools.Controls.RibbonButton, Syncfusion.Windows.Tools.Controls.RibbonBar> mapMA,
                                        List<Syncfusion.Windows.Tools.Controls.RibbonButton> listBS, Dictionary<Syncfusion.Windows.Tools.Controls.RibbonButton, Syncfusion.Windows.Tools.Controls.RibbonBar> mapBS)
        {
            ResourceDictionary computerdictionary = new ResourceDictionary() { Source = new Uri(@"/Syncfusion.VectorImages.WPF;component/Icons/Computer.xaml", UriKind.RelativeOrAbsolute) };

            foreach (var bar in item.Items)
            {
                if (bar is Syncfusion.Windows.Tools.Controls.RibbonBar)
                {
                    Syncfusion.Windows.Tools.Controls.RibbonBar ribbonBar = bar as Syncfusion.Windows.Tools.Controls.RibbonBar;
                    if (ribbonBar.CollapseImage == null)
                        ribbonBar.CollapseImage = (DrawingImage)computerdictionary["FolderIcon"];

                    if (RemovedRibbonBarQAMA.ContainsKey(ribbonBar))
                    {
                        RemovedRibbonBarQAMA[ribbonBar].ForEach(button =>
                        {
                            String tag = button.Tag as String;
                            tag = tag.ToLower();
                            if (tag.Contains("quickaccess"))
                            {
                                listQA.Add(button);
                                mapQA.Add(button, ribbonBar);
                                if (!addedRibbonBarQAMA.ContainsKey(ribbonBar))
                                    addedRibbonBarQAMA.Add(ribbonBar, new List<Syncfusion.Windows.Tools.Controls.RibbonButton>());
                                addedRibbonBarQAMA[ribbonBar].Add(button);
                            }
                            else if (tag.Contains("applicationmenu"))
                            {
                                listMA.Add(button);
                                mapMA.Add(button, ribbonBar);
                                if (!addedRibbonBarQAMA.ContainsKey(ribbonBar))
                                    addedRibbonBarQAMA.Add(ribbonBar, new List<Syncfusion.Windows.Tools.Controls.RibbonButton>());
                                addedRibbonBarQAMA[ribbonBar].Add(button);
                            }
                            else if (tag.Contains("backstage"))
                            {
                                listBS.Add(button);
                                mapBS.Add(button, ribbonBar);
                                if (!addedRibbonBarQAMA.ContainsKey(ribbonBar))
                                    addedRibbonBarQAMA.Add(ribbonBar, new List<Syncfusion.Windows.Tools.Controls.RibbonButton>());
                                addedRibbonBarQAMA[ribbonBar].Add(button);
                            }
                        });
                    }

                    foreach (var button in ribbonBar.Items)
                    {
                        if (button is ItemsControl)
                            ExtractRibbonItems(button as ItemsControl, listQA, mapQA, listMA, mapMA, listBS, mapBS);
                        else if (button is Syncfusion.Windows.Tools.Controls.RibbonButton)
                        {
                            Syncfusion.Windows.Tools.Controls.RibbonButton ribbonButton = button as Syncfusion.Windows.Tools.Controls.RibbonButton;
                            if (ribbonButton.Tag is String)
                            {
                                String tag = ribbonButton.Tag as String;
                                tag = tag.ToLower();
                                if (tag.Contains("quickaccess"))
                                {
                                    listQA.Add(ribbonButton);
                                    mapQA.Add(ribbonButton, ribbonBar);
                                    if (!addedRibbonBarQAMA.ContainsKey(ribbonBar))
                                        addedRibbonBarQAMA.Add(ribbonBar, new List<Syncfusion.Windows.Tools.Controls.RibbonButton>());
                                    addedRibbonBarQAMA[ribbonBar].Add(ribbonButton);
                                }
                                else if (tag.Contains("applicationmenu"))
                                {
                                    listMA.Add(ribbonButton);
                                    mapMA.Add(ribbonButton, ribbonBar);
                                    if (!addedRibbonBarQAMA.ContainsKey(ribbonBar))
                                        addedRibbonBarQAMA.Add(ribbonBar, new List<Syncfusion.Windows.Tools.Controls.RibbonButton>());
                                    addedRibbonBarQAMA[ribbonBar].Add(ribbonButton);
                                }
                                else if (tag.Contains("backstage"))
                                {
                                    listBS.Add(ribbonButton);
                                    mapBS.Add(ribbonButton, ribbonBar);
                                    if (!addedRibbonBarQAMA.ContainsKey(ribbonBar))
                                        addedRibbonBarQAMA.Add(ribbonBar, new List<Syncfusion.Windows.Tools.Controls.RibbonButton>());
                                    addedRibbonBarQAMA[ribbonBar].Add(ribbonButton);
                                }
                            }
                        }
                    }
                }
            }
        }

        void PromoteIdleRibbonPending()
        {
            if (bShuttingDown)
                return;
            // IdleExecutionRibbonPending();
            if (IdleExecutionPendingRibbons == null)
            {
                Action action = () => IdleExecutionRibbonPending();
                IdleExecutionPendingRibbons = mainWindow.Dispatcher.BeginInvoke(action, DispatcherPriority.ApplicationIdle);
                IdleExecutionPendingRibbons.Completed += (sender, e) =>
                {
                    IdleExecutionPendingRibbons = null;
                };
            }
        }

        public void AddRibbonTabItem(Syncfusion.Windows.Tools.Controls.RibbonTab item, int n, CommandBindingCollection c,
                                     String ctg, Color bc)
        {
            if (bShuttingDown)
                return;
            mainWindow.Dispatcher.InvokeIfRequired(
            () =>
            {
                AddPendingRibbonRequests.Add(new RibbonChangingRequest()
                {
                    contextTabGroup = ctg,
                    backColor = bc,
                    list = new List<Syncfusion.Windows.Tools.Controls.RibbonTab>()
                    {
                        item
                    },
                    nPos = n,
                    commandBinding = c
                });

                PromoteIdleRibbonPending();
            });
        }

        public void AddRibbonsTabItem(List<Syncfusion.Windows.Tools.Controls.RibbonTab> l, int n, CommandBindingCollection c,
                                      String ctg, Color bc)
        {
            if (bShuttingDown)
                return;
            mainWindow.Dispatcher.InvokeIfRequired(
            () =>
            {
                AddPendingRibbonRequests.Add(new RibbonChangingRequest()
                {
                    contextTabGroup = ctg,
                    backColor = bc,
                    list = l,
                    nPos = n,
                    commandBinding = c
                });

                PromoteIdleRibbonPending();
            });
        }

        public void RemoveRibbonTabItem(Syncfusion.Windows.Tools.Controls.RibbonTab item, CommandBindingCollection c, String ctg, Color bc)
        {
            if (bShuttingDown)
                return;
            mainWindow.Dispatcher.InvokeIfRequired(
            () =>
            {
                RemovePendingRibbonRequests.Add(new RibbonChangingRequest()
                {
                    contextTabGroup = ctg,
                    backColor = bc,
                    list = new List<Syncfusion.Windows.Tools.Controls.RibbonTab>()
                    {
                        item
                    },
                    commandBinding = c
                });

                PromoteIdleRibbonPending();
            });
        }

        public void RemoveRibbonsTabItem(List<Syncfusion.Windows.Tools.Controls.RibbonTab> l, CommandBindingCollection c, String ctg, Color bc)
        {
            if (bShuttingDown)
                return;
            mainWindow.Dispatcher.InvokeIfRequired(
            () =>
            {
                RemovePendingRibbonRequests.Add(new RibbonChangingRequest()
                {
                    contextTabGroup = ctg,
                    backColor = bc,
                    list = l,
                    commandBinding = c
                });

                PromoteIdleRibbonPending();
            });
        }

        public void SetMainMenuVisibility(Visibility visibility)
        {
            if (bShuttingDown)
                return;
            mainWindow.Dispatcher.InvokeIfRequired(
            () =>
            {
                mainWindow.MainMenu.Visibility = visibility;
            });
        }

        public void SetRibbonVisibility(Visibility visibility)
        {
            if (bShuttingDown)
                return;
            mainWindow.Dispatcher.InvokeIfRequired(
            () =>
            {
                mainWindow.Ribbons.Visibility = visibility;
            });
        }

        public void RegisterComponent(IComponent component)
        {
            if (bShuttingDown)
                return;
            mainWindow.Dispatcher.InvokeIfRequired(
            () =>
            {
                mainWindow.AddAndInitializeComponent(component);
            }, DispatcherPriority.Send);
        }

        public event EventHandler<CancelEventArgs> ContextContentChanging;

        virtual public void OnContextContentChanging(object sender, CancelEventArgs e)
        {
            EventHandler<CancelEventArgs> temp = ContextContentChanging;
            if (temp != null)
                temp(sender, e);
        }

        public event EventHandler ContextContentChanged;

        virtual public void OnContextContentChanged(object sender)
        {
            EventHandler temp = ContextContentChanged;
            if (temp != null)
                temp(sender, EventArgs.Empty);
        }

        public event EventHandler RefreshCurrentContents;

        virtual public void OnRefreshCurrentContents(object sender)
        {
            EventHandler temp = RefreshCurrentContents;
            if (temp != null)
                temp(sender, EventArgs.Empty);
        }

        

        public event EventHandler<CancelEventArgs> ContextDocumentChanging;

        virtual public void OnContextDocumentChanging(object sender, CancelEventArgs e)
        {
            EventHandler<CancelEventArgs> temp = ContextDocumentChanging;
            if (temp != null)
                temp(sender, e);
        }

        public event EventHandler ContextDocumentChanged;

        virtual public void OnContextDocumentChanged(object sender)
        {
            EventHandler temp = ContextDocumentChanged;
            if (temp != null)
                temp(sender, EventArgs.Empty);
        }


        public FrameworkElement ActiveWindow
        {
            get
            {
                return mainWindow.dockingManager.ActiveWindow;
            }
            set
            {
                mainWindow.Dispatcher.InvokeIfRequired(
                () =>
                {
                    mainWindow.dockingManager.ActiveWindow = value;
                }, DispatcherPriority.Background);
            }
        }

        public object ContextObject
        {
            get
            {
                return ContextSelectedObject;
            }
            set
            {
                PromoteSelectionObject(value);
            }
        }

        public IDocument ContextDocument
        {
            get
            {
                return ContextSelectedDocument;
            }
            set
            {
                PromoteSelectionDocument(value);
            }
        }

        public IList ContextObjects
        {
            get
            {
                return ContextSelectedObjects;
            }
            set
            {
                PromoteSelectionObject(value);
            }
        }

        public void UpdateContextNow()
        {
            PromoteCodeToIdle(true);
        }

        public void ForceRefreshCurrentContents()
        {
            OnRefreshCurrentContents(this);
        }

        public IList GetFriendObjects(Object f, Type t)
        {
            GetFriendObjectsEventArgs e = new GetFriendObjectsEventArgs()
            {
                friend = f,
                expectedType = t
            };

            OnPromptFriendObjects(f, e);
            return e.friendList;
        }

        public event EventHandler<GetFriendObjectsEventArgs> PromptFriendObjects;

        virtual public void OnPromptFriendObjects(object sender, GetFriendObjectsEventArgs e)
        {
            EventHandler<GetFriendObjectsEventArgs> temp = PromptFriendObjects;
            if (temp != null)
                temp(sender, e);
        }

        public Object GetDocumentEditorObject(Object f)
        {
            GetDocumentEditorObjectEventArgs e = new GetDocumentEditorObjectEventArgs()
            {
                obj = f
            };

            OnPromptDocumentEditorObject(f, e);
            return e.documentEditor;
        }

        public event EventHandler<GetDocumentEditorObjectEventArgs> PromptDocumentEditorObject;

        virtual public void OnPromptDocumentEditorObject(object sender, GetDocumentEditorObjectEventArgs e)
        {
            EventHandler<GetDocumentEditorObjectEventArgs> temp = PromptDocumentEditorObject;
            if (temp != null)
                temp(sender, e);
        }

        public UserControl GetSmartTagsEditorObject(Object f)
        {
            GetSmartTagsEditorObjectEventArgs e = new GetSmartTagsEditorObjectEventArgs()
            {
                obj = f
            };

            OnPromptSmartTagsEditorObject(f, e);
            return e.smartTagsEditor;
        }

        public event EventHandler<GetSmartTagsEditorObjectEventArgs> PromptSmartTagsEditorObject;

        virtual public void OnPromptSmartTagsEditorObject(object sender, GetSmartTagsEditorObjectEventArgs e)
        {
            EventHandler<GetSmartTagsEditorObjectEventArgs> temp = PromptSmartTagsEditorObject;
            if (temp != null)
                temp(sender, e);
        }

        public virtual void StatusText(String text)
        {
            mainWindow.Dispatcher.InvokeIfRequired(
            () =>
            {
                mainWindow.StatusText = text;
            }, DispatcherPriority.Send);
        }

        #endregion IWorkspace Members

        #region IUFInterfaceBase Members

        public void Initialize()
        {
        }

        #endregion IUFInterfaceBase Members

        private void PromoteSelectionObject(Object ob)
        {
            lock (lockObject)
            {
                PromoteSelectingObject = ob;
                PromoteSelectingObjects = null;
                PromoteCodeToIdle(false);
            }
        }

        private void PromoteSelectionDocument(IDocument ob)
        {
            lock (lockObject)
            {
                PromoteSelectingDocument = ob;
                PromoteDocumentCodeToIdle(false);
            }
        }

        private void PromoteSelectionObject(IList obs)
        {
            lock (lockObject)
            {
                PromoteSelectingObject = null;
                PromoteSelectingObjects = obs;
                PromoteCodeToIdle(false);
            }
        }

        private void PromoteCodeToIdle(bool bSynchro)
        {
            if (bSynchro && IdleExecutionPending != null)
            {
                IdleExecutionPending.Abort();
                IdleExecutionPending = null;
            }
            if (IdleExecutionPending == null)
            {
                Action action = () => IdleExecution();
                Dispatcher disp = mainWindow != null ? mainWindow.Dispatcher : Dispatcher.CurrentDispatcher;
                if (bSynchro)
                    disp.Invoke(action, DispatcherPriority.Send);
                else
                {
                    IdleExecutionPending = disp.BeginInvoke(action, DispatcherPriority.Background);
                    IdleExecutionPending.Completed += (sender, e) => IdleExecutionPending = null;
                }
            }
        }

        private void PromoteDocumentCodeToIdle(bool bSynchro)
        {
            if (bSynchro && IdleExecutionDocumentPending != null)
            {
                IdleExecutionDocumentPending.Abort();
                IdleExecutionDocumentPending = null;
            }
            if (IdleExecutionDocumentPending == null)
            {
                Action action = () => IdleDocumentExecution();
                Dispatcher disp = mainWindow != null ? mainWindow.Dispatcher : Dispatcher.CurrentDispatcher;
                if (bSynchro)
                    disp.Invoke(action, DispatcherPriority.Send);
                else
                {
                    IdleExecutionDocumentPending = disp.BeginInvoke(action, DispatcherPriority.Background);
                    IdleExecutionDocumentPending.Completed += (sender, e) => IdleExecutionDocumentPending = null;
                }
            }
        }

        public IEntityReference GetEntityReferenceContextObject(Object obj)
        {
            lock (lockObject)
            {
                if (mapEntities.ContainsKey(obj))
                    return mapEntities[obj];
            }

            return null;
        }

        readonly Dictionary<Object, IEntityReference> mapEntities = new Dictionary<Object, IEntityReference>();
        private void IdleExecution()
        {
            if (bShuttingDown)
                return;
            lock (lockObject)
            {
                if (ContextSelectedObject != null)
                {
                    if (PromoteSelectingObject == ContextSelectedObject)
                        return;
                }
                else if (PromoteSelectingObjects != null)
                {
                    if (ContextSelectedObjects != null && PromoteSelectingObjects.Count == ContextSelectedObjects.Count)
                    {
                        bool bChanged = false;
                        for(int i = 0; i < PromoteSelectingObjects.Count; ++i)
                        {
                            if (PromoteSelectingObjects[i] != ContextSelectedObjects[i])
                            {
                                bChanged = true;
                                break;
                            }
                        }
                        if (!bChanged)
                            return;
                    }
                }

                CancelEventArgs cea = new CancelEventArgs();
                OnContextContentChanging(this, cea);
                if (cea.Cancel)
                    return;

                mapEntities.Clear();
                if (PromoteSelectingObject != null)
                {
                    // ContextSelectedObject = PromoteSelectingObject is IEntityReference && (PromoteSelectingObject as IEntityReference).ContainedObject != null ? (PromoteSelectingObject as IEntityReference).ContainedObject : PromoteSelectingObject;
                    ContextSelectedObject = PromoteSelectingObject;
                    if (PromoteSelectingObject is IEntityReference)
                        mapEntities.Add(ContextSelectedObject, PromoteSelectingObject as IEntityReference);
                    ContextSelectedObjects = null;
                }
                else if (PromoteSelectingObjects != null)
                {
                    List<Object> list = new List<Object>();
                    foreach (Object o in PromoteSelectingObjects)
                    {
                        // var obj = o is IEntityReference && (o as IEntityReference).ContainedObject != null ? (o as IEntityReference).ContainedObject : o;
                        var obj = o;
                        list.Add(obj);
                        if (obj is IEntityReference)
                        {
                            if (mapEntities.ContainsKey(obj))
                                mapEntities.Remove(obj);
                            mapEntities.Add(obj, o as IEntityReference);
                        }
                    }
                    ContextSelectedObject = null;
                    ContextSelectedObjects = list;
                }
                else
                {
                    ContextSelectedObject = null;
                    ContextSelectedObjects = null;
                }

                OnContextContentChanged(this);

                PromoteSelectingObject = null;
                PromoteSelectingObjects = null;
            }
        }

        private void IdleDocumentExecution()
        {
            if (bShuttingDown)
                return;
            lock (lockObject)
            {
                if (PromoteSelectingDocument != ContextSelectedDocument)
                {
                    CancelEventArgs cea = new CancelEventArgs();
                    OnContextDocumentChanging(this, cea);
                    if (cea.Cancel)
                        return;

                    ContextSelectedDocument = PromoteSelectingDocument;
                    OnContextDocumentChanged(this);
                }

                PromoteSelectingDocument = null;
            }
        }

        #region IEditableObject Members

        private List<IEditableObject> GetListEditors()
        {
            List<Object> list = new List<Object>();
            if (ContextSelectedObject != null)
            {
                list.Add(ContextSelectedObject);
                if (ContextSelectedObject is IEntityReference)
                {
                    var entity = ContextSelectedObject as IEntityReference;
                    if (entity.ContainedObject != null)
                        list.Add(entity.ContainedObject);
                }
            }
            else if (ContextSelectedObjects != null)
            {
                foreach (var v in ContextSelectedObjects)
                {
                    list.Add(v);
                    if (v is IEntityReference)
                    {
                        var entity = v as IEntityReference;
                        if (entity.ContainedObject != null)
                            list.Add(entity.ContainedObject);
                    }
                }
            }

            List<IEditableObject> listEditors = new List<IEditableObject>();
            list.ForEach(o =>
                {
                    Object editor = GetDocumentEditorObject(o);
                    if (editor is IEditableObject && !listEditors.Contains(editor as IEditableObject))
                        listEditors.Add(editor as IEditableObject);
                });
            return listEditors;
        }

        public void BeginEdit()
        {
            List<IEditableObject> listEditors = GetListEditors();
            if (listEditors.Count == 0)
                throw new NotSupportedException("No Editors available for the Edit interface");
            listEditors.ForEach(editable =>
                {
                    editable.BeginEdit();
                });
        }

        public void CancelEdit()
        {
            List<IEditableObject> listEditors = GetListEditors();
            listEditors.ForEach(editable =>
            {
                editable.CancelEdit();
            });
        }

        public void EndEdit()
        {
            List<IEditableObject> listEditors = GetListEditors();
            listEditors.ForEach(editable =>
            {
                editable.EndEdit();
            });
        }

        #endregion IEditableObject Members

        #region IDisposable Members

        void IDisposable.Dispose()
        {
            PromoteSelectingObject = null;
            PromoteSelectingObjects = null;

            if (IdleExecutionPending != null)
            {
                IdleExecutionPending.Abort();
                IdleExecutionPending = null;
            }
            if (IdleExecutionDocumentPending != null)
            {
                IdleExecutionDocumentPending.Abort();
                IdleExecutionDocumentPending = null;
            }
            
            lockObject = null;
        }

        #endregion IDisposable Members
    }
}