using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using UFInterfaces;
using UFInterfaces.CoreHostComponents;
using DocumentManager.ComponentService;
using Utilities;
using Utilities.WPF;
using UFSolutionNext;
using DevExpress.Xpf.Docking;
using UIMsgBoxAlertService.ComponentService;
using System.Linq;
using DevExpress.Xpf.Bars;
using Utilities.Commands;
using System.Reflection;
using System.IO;
using System.Xml.Linq;

namespace UFSolution
{
    class WorkSpaceComponent : ComponentBase<IWorkspace>, IWorkspace, IEditableObject, IDisposable
    {
        #region Declaration

        readonly UFMainWindow mainWindow;
        LogViewerContainer emptyLogViewerControl;
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
        readonly static String SuffixChangedDocument = "*";
        public List<string> ActiveToolbarsSchemes
        {
            get;
            private set;
        } = new List<string>();
        Dictionary<String, List<Bar>> addedBars = new Dictionary<String, List<Bar>>();
        Dictionary<Bar, String> addedBarsReverse = new Dictionary<Bar, String>();
        int addedBarManagers = 0;
        static List<string> HiddenPanels = null;
        const string hiddenPanelsFileName = "HiddenWorkspacePanels";
        const string hiddenComponentsFileName = "HiddenProjectComponents";
        static List<string> hiddenComponents = null;
        static Dictionary<IToolbar, bool> easyModeHiddenToolbars = null;
        DispatcherOperation dpActiveWindow;
        FrameworkElement lastActiveWindow;
        bool bIsWorkspaceLoaded;
        bool bDisposed;
        public bool IsWorkspaceLoaded
        {
            get
            {
                return bIsWorkspaceLoaded;
            }
            set
            {
                bIsWorkspaceLoaded = value;
                if (value)
                {
                    mainWindow.ToggleComponentsVisibility(HiddenPanels);
                    if (IsInEasyMode)
                        OnEasyModeChanged(mainWindow);
                }
            }
        }
        #endregion Declaration

        public WorkSpaceComponent(UFMainWindow wnd)
        {
            mainWindow = wnd;
            CreateDockedControls();
        }

        #region IWorkspace Members

        #region Public Properties
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
                            bRet = mainWindow.IsBusy;
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
                            mainWindow.IsBusy = value;
                        });
            }
        }

        public void ResetBusy() 
        {
            if (bShuttingDown)
                return;
            mainWindow.Dispatcher.InvokeIfRequired(
                    () =>
                    {
                        mainWindow.ResetBusy();
                    });
        }

        public void RestoreBusy()
        {
            if (bShuttingDown)
                return;
            mainWindow.Dispatcher.InvokeIfRequired(
                    () =>
                    {
                        mainWindow.RestoreBusy();
                    });
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
                            ret = mainWindow.BusyContent;
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
                            mainWindow.BusyContent = value;
                        });
            }
        }
        #endregion

        private void CreateDockedControls()
        {
            if (emptyLogViewerControl != null)
                return;

            emptyLogViewerControl = new LogViewerContainer(this);

            AddDockingChildren(emptyLogViewerControl, UFSolutionNext.Properties.Resources.LogTitle, UFInterfaces.DockState.AutoHidden, UFInterfaces.DockSide.Bottom, false, itemID: nameof(Log4NetViewer.Log4NetViewer));
            SetDockedElementIcon(emptyLogViewerControl, new ImageBrush(GetBitmapImage("WStrace")));
        }

        public static System.Windows.Media.Imaging.BitmapImage GetBitmapImage(String image, bool bShared = false)
        {
            var bm = SharedResources.Helpers.ResourceManager.GetCommonImage("WorkSpace", image, bShared);
            return bm;
        }

        public void ShowSystemLog()
        {
            if (emptyLogViewerControl == null)
                return;

            ActivateDockedElement(emptyLogViewerControl);
            emptyLogViewerControl.CreateLogViewer();

            mainWindow.Dispatcher.BeginInvoke(
            (Action)(() =>
            {
                FrameworkElement element = emptyLogViewerControl.LogViewercontrol;
                var state = DockingHelper.GetState(emptyLogViewerControl);
                var panel = DockLayoutManager.GetLayoutItem(emptyLogViewerControl);

                if (panel != null)
                    mainWindow.dockingManager.ActivateDockItem(panel);

                mainWindow.Refresh();

                element.Focusable = true;
                element.Focus();
                using (var cursor = new WaitCursor())
                {
                    System.Threading.Thread.Sleep(1000);
                    emptyLogViewerControl.LogViewercontrol.Refresh();
                }
                FocusManager.SetFocusedElement(element, element);
            }), DispatcherPriority.Background);
        }

        void ToggleEasyModeConfig()
        {
            if (HiddenPanels == null)
                LoadHiddenPanels();

            if (IsWorkspaceLoaded)
                mainWindow.ToggleComponentsVisibility(HiddenPanels);
        }

        internal void EasyModeToggleToolbars()
        {
            if (easyModeHiddenToolbars == null && IsInEasyMode)
            {
                easyModeHiddenToolbars = new Dictionary<IToolbar, bool>();
                UriRisolver.GetListInstalledDocumentManagers().ForEach(im =>
                {
                    var tb = im.GetToolbar();
                    if (tb != null && IsComponentHidden(im.TypeScheme))
                        easyModeHiddenToolbars.Add(tb, tb.IsVisible());
                });
            }
            if (easyModeHiddenToolbars == null)
                return;

            foreach (var tb in easyModeHiddenToolbars.Keys.ToList())
            {
                if (IsInEasyMode)
                {
                    easyModeHiddenToolbars[tb] = tb.IsVisible();
                    if (tb.HideMenuItem())
                        tb.Hide();
                }
                else
                {
                    tb.ShowMenuItem();
                    if (easyModeHiddenToolbars[tb])
                        tb.Show();
                }
            }
        }

        static void LoadHiddenPanels()
        {
            HiddenPanels = new List<string>();
            string filepath = string.Format("{0}\\{1}.xml", Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location), hiddenPanelsFileName);
            if (File.Exists(filepath))
            {
                try
                {
                    var xml = XDocument.Load(@filepath);
                    foreach (XElement node in xml.Root.Descendants("Name").ToList())
                    {
                        var componentName = node.Value as String;
                        if (!String.IsNullOrEmpty(componentName))
                            HiddenPanels.Add(componentName.Trim());
                    }
                }
                catch (Exception ex) { }
            }
        }

        public event EventHandler<ProgressStateChangedEventArgs> ProgressStateChanged;

        virtual public void OnProgressStateChanged(object sender, ProgressStateChangedEventArgs args)
        {
            ProgressStateChanged?.Invoke(sender, args);
        }

        public void UpdateProgressState(int value, int maxvalue, string description, TaskbarItemProgressState state)
        {
            if (bShuttingDown)
                return;
            mainWindow.Dispatcher.BeginInvokeIfRequired(
                    () =>
                    {
                        mainWindow.UpdateProgressState(value, maxvalue, description, state);
                    });
        }

        public void ResetProgressState()
        {
            if (bShuttingDown)
                return;
            mainWindow.Dispatcher.BeginInvokeIfRequired(
                    () =>
                    {
                        mainWindow.ResetProgressState();
                    });
        }
        public void IncrementProgressState(int step = 1)
        {
            if (bShuttingDown)
                return;
            mainWindow.Dispatcher.BeginInvokeIfRequired(
                    () =>
                    {
                        mainWindow.IncrementProgressState(step);
                    });
        }
        public bool IsDockedChildren(FrameworkElement el)
        {
            if (bShuttingDown)
                return false;
            bool ret = false;
            mainWindow.Dispatcher.InvokeIfRequired(
            () =>
            {
                ret = mainWindow.dockingManager.IsControlDocked(el);
            });

            return ret;
        }

        public void RemoveBarManagerItem(FrameworkElement barManager, CommandBindingCollection commandBindings, string componentTypeScheme = "")
        {
            if (bShuttingDown || !(barManager is DevExpress.Xpf.Bars.BarManager))
                return;

            mainWindow.Dispatcher.InvokeIfRequired(
            () =>
            {
                if (!mainWindow.menuGrid.Children.Contains(barManager))
                    return;

                mainWindow.menuGrid.Children.Remove(barManager);

                DevExpress.Xpf.Bars.BarManager bmanager = (barManager as DevExpress.Xpf.Bars.BarManager);
                if (bmanager != null)
                {
                    //addedBarManagers--;
                    if (componentTypeScheme != "" && ActiveToolbarsSchemes.Contains(componentTypeScheme))
                        ActiveToolbarsSchemes.Remove(componentTypeScheme);
                    foreach (var bar in bmanager.Bars)
                    {
                        if (addedBarsReverse.Keys.Contains(bar))
                        {
                            addedBars[addedBarsReverse[bar]].Remove(bar);
                            if (addedBars[addedBarsReverse[bar]].Count == 0)
                            {
                                var mergingTargetBar = (from Bar b in mainWindow.mainBarManager.Bars where b.Name == addedBarsReverse[bar] select b).FirstOrDefault();
                                if (mergingTargetBar != null)
                                    mainWindow.mainBarManager.Bars.Remove(mergingTargetBar); //addedBarsReverse[bar]
                                addedBars.Remove(addedBarsReverse[bar]);
                            }
                            addedBarsReverse.Remove(bar);
                        }
                    }
                }

                if (commandBindings != null)
                {
                    foreach (CommandBinding cb in commandBindings)
                        if (mainWindow.CommandBindings.Contains(cb))
                            mainWindow.CommandBindings.Remove(cb);
                }
            });
        }

        public void AddBarManagerItem(FrameworkElement barManager, CommandBindingCollection globalCommandBindings, string componentTitle = "", string componentTypeScheme = "")
        {
            if (bShuttingDown || !(barManager is BarManager))
                return;
            mainWindow.Dispatcher.InvokeIfRequired(
            () =>
            {
                if (mainWindow.menuGrid.Children.Contains(barManager))
                    return;

                mainWindow.menuGrid.Children.Insert(0, barManager);
                BarManager bmanager = (barManager as BarManager);

                if (bmanager != null)
                {
                    addedBarManagers++;
                    if (componentTypeScheme != "" && !ActiveToolbarsSchemes.Contains(componentTypeScheme))
                        ActiveToolbarsSchemes.Add(componentTypeScheme);
                    foreach (var bar in bmanager.Bars)
                    {
                        if (bar.IsMainMenu) 
                        {
                            //Menu: Workaround for randomly duplicated DevExpress menu items
                            //if (bar.Name == "startupMenu") 
                            //{
                            //    foreach (var link in bar.ItemLinks)
                            //    {
                            //        var resname = link.Item.Content?.ToString();
                            //        if (!String.IsNullOrEmpty(resname) && !String.IsNullOrEmpty(UFSolutionNext.Properties.Resources.ResourceManager.GetString(resname)))
                            //            link.Item.Content = UFSolutionNext.Properties.Resources.ResourceManager.GetString(resname);
                            //    }
                            //}
                            mainWindow.mainMenu.UnMerge(bar);
                            mainWindow.mainMenu.Merge(bar);
                        }
                        else
                        {
                            //Toolbars: automatically create target bar if merging target (MergingProperties.Name) is not found
                            var mergingTargetName = MergingProperties.GetName(bar);
                            var mergingTargetBar = (from Bar b in mainWindow.mainBarManager.Bars where b.Name == mergingTargetName select b).FirstOrDefault();
                            if (mergingTargetBar == null)
                            {   
                                var newTargetBar = new Bar() {
                                    Name = mergingTargetName,
                                    Caption = String.Format("{0} {1}", componentTitle, !String.IsNullOrEmpty(bar.Caption) ? bar.Caption : ""),
                                    IsMultiLine = false,
                                    AllowCustomizationMenu = false,
                                    AllowDrop = true,
                                    AllowHide = DevExpress.Utils.DefaultBoolean.True,
                                    AllowCollapse = true,
                                    AllowQuickCustomization = DevExpress.Utils.DefaultBoolean.False,
                                    DockInfo = new BarDockInfo() { Row = addedBarManagers + 1 },
                                    IsCollapsed = !mainWindow.bMenusLoaded,
                                    ShowDragWidget = mainWindow.bMenusLoaded,
                                    HideWhenEmpty = true
                                };
                                mainWindow.mainBarManager.Bars.Add(newTargetBar);
                                newTargetBar.Merge(bar);
                                addedBars.Add(mergingTargetName, new List<Bar>() { bar });
                                addedBarsReverse.Add(bar, mergingTargetName);
                            }
                            else if (addedBars.Keys.Contains(mergingTargetName))
                            {
                                addedBars[mergingTargetName].Add(bar);
                                addedBarsReverse.Add(bar, mergingTargetName);
                            }
                        }
                    }
                }
                AddBarManagerGlobalCommands(globalCommandBindings);
            });
        }

        public void AddBarManagerGlobalCommands(CommandBindingCollection globalCommandBindings)
        {
            if (bShuttingDown || globalCommandBindings == null)
                return;

            mainWindow.Dispatcher.InvokeIfRequired(
            () =>
            {
                foreach (CommandBinding cb in globalCommandBindings)
                {
                    if (mainWindow.CommandBindings.Contains(cb))
                        mainWindow.CommandBindings.Remove(cb);

                    if (!mainWindow.bLoading)
                    {
                        MergeGlobalCommands(cb);
                    }

                    mainWindow.CommandBindings.Insert(0, cb);
                }
            });
        }

        void MergeGlobalCommands(CommandBinding newCb)
        {
            var oldBinding = (from CommandBinding c in mainWindow.CommandBindings where (c.Command as RoutedCommand)?.Name == (newCb.Command as RoutedCommand)?.Name select c).FirstOrDefault();
            mainWindow.Dispatcher.BeginInvokeAsynchronously(() =>
            {
                List<BarItem> menuitems;
                if (oldBinding != null)
                {
                    menuitems = (from BarItem bl in mainWindow.mainBarManager.GetChildrenOfType<BarItem>() where (bl.Command == oldBinding.Command || bl.Command == newCb.Command) select bl).ToList();
                    
                    mainWindow.CommandBindings.Remove(oldBinding);
                }
                else
                    menuitems = (from BarItem bl in mainWindow.mainBarManager.GetChildrenOfType<BarItem>() where bl.Content as string == (newCb.Command as RoutedUICommand).Text && bl.Command != null select bl).ToList();
                if (menuitems != null)
                    foreach (var menuitem in menuitems)
                        menuitem.IsVisible = menuitem.Command == newCb.Command;
            });
        }

        public void RemoveBarManagerCommands(CommandBindingCollection commandBindings)
        {
            if (bShuttingDown || commandBindings == null)
                return;

            mainWindow.Dispatcher.InvokeIfRequired(
            () =>
            {
                foreach (CommandBinding cb in commandBindings)
                    if (mainWindow.CommandBindings.Contains(cb))
                        mainWindow.CommandBindings.Remove(cb);
            });
        }

        public void AddDockingChildren(FrameworkElement el,
                                       String header,
                                       DockState state, DockSide side, 
                                       bool bCanClose = true, bool bCanFloat = true, 
                                       DockSide layoutGroupDockSide = DockSide.Left, String itemID = null)
        {
            if (bShuttingDown)
                return;

            mainWindow.Dispatcher.InvokeIfRequired(
            () =>
            {
                if (!mainWindow.dockingManager.IsControlDocked(el))
                {
                    using (var cursor = new WaitCursor())
                    {
                        if (String.IsNullOrEmpty(el.Name))
                        {
                            try
                            {
                                el.Name = DependencyObjectExtensions.AdaptName(itemID ?? header);
                            }
                            catch { }
                        }

                        bool bFound = false;
                        var originalName = el.Name;
                        int i = 0;
                        while (true)
                        {
                            foreach (FrameworkElement item in mainWindow.dockingManager.GetAllDockedControls())
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

                        mainWindow.AddDockedControl(el, header, side, state, bCanClose, bCanFloat, layoutGroupDockSide);

                        if (state == DockState.Document)
                        {
                            lastActiveWindow = el;
                            if (dpActiveWindow == null || dpActiveWindow.Status == DispatcherOperationStatus.Aborted || dpActiveWindow.Status == DispatcherOperationStatus.Completed)
                            {
                                dpActiveWindow = mainWindow.Dispatcher.BeginInvokeAsynchronouslyInApplicationIdle(
                                () =>
                                {
                                    if (bDisposed)
                                        return;

                                    if (mainWindow.dockingManager.IsControlDocked(lastActiveWindow))
                                    {
                                        ActiveWindow = lastActiveWindow;
                                        lastActiveWindow.Focusable = true;
                                        lastActiveWindow.Focus();
                                        FocusManager.SetFocusedElement(lastActiveWindow, lastActiveWindow);
                                    }
                                });
                            }
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
                    if (mainWindow.dockingManager.IsControlDocked(el))
                    {
                        mainWindow.RemoveDockedControl(el);
                    }

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

        UriResolver.ComponentService.IUriRisolver uriRisolver;
        public UriResolver.ComponentService.IUriRisolver UriRisolver
        {
            get
            {
                if (uriRisolver == null)
                    uriRisolver = GetService(typeof(UriResolver.ComponentService.IUriRisolver)) as UriResolver.ComponentService.IUriRisolver;
                return uriRisolver;
            }
        }

        IUIMsgBoxAlertService uiInterface;
        public IUIMsgBoxAlertService UIInterface
        {
            get
            {
                if (uiInterface == null)
                    uiInterface = GetService(typeof(IUIMsgBoxAlertService)) as IUIMsgBoxAlertService;
                return uiInterface;
            }
        }

        public DockState GetElementDockState(FrameworkElement el)
        {
            if (bShuttingDown)
                return DockState.Dock;
            return DockingHelper.GetState(el);
        }

        public void SetElementDockState(FrameworkElement el, DockState dockState)
        {
            if (bShuttingDown)
                return;
            mainWindow.SetState(el, dockState);
        }

        public DockSide GetElementDockSide(FrameworkElement el)
        {
            if (bShuttingDown)
                return DockSide.None;
            try
            {
                return mainWindow.GetSide(el);
            }
            catch (Exception ex)
            {
                return DockSide.None;
            }
        }

        public bool GetElementIsSelectedTab(FrameworkElement el)
        {
            if (el == null)
                return false;

            var layoutEl = DockLayoutManager.GetLayoutItem(el);
            if (layoutEl == null || !layoutEl.IsTabPage)
                return false;

            var tabGroup = layoutEl.FindParent<TabbedGroup>();
            if (tabGroup == null)
                return false;

            return tabGroup.SelectedItem == layoutEl;
        }

        public void FlashDockedElement(FrameworkElement element)
        {
            if (bShuttingDown)
                return;
            mainWindow.Dispatcher.InvokeIfRequired(
            () =>
            {
                var panel = DockLayoutManager.GetLayoutItem(element);
                mainWindow.dockingManager.ActivateDockItem(panel);

                mainWindow.Refresh();

                element.Focusable = true;
                element.Focus();
                FocusManager.SetFocusedElement(element, element);
            });
        }

        public void SetDesiredHeightAndWidthInDockedMode(FrameworkElement el, double Height, double Width)
        {
            if (bShuttingDown)
                return;

            mainWindow.SetDesiredDockSize(el, new Size(Width, Height));
        }

        public void SetDesiredSideMode(FrameworkElement el, String target)
        {
            if (bShuttingDown)
                return;
            var name = DependencyObjectExtensions.AdaptName(target);
            var parentEl = GetElementFromName(name);
            if (parentEl != null)
            {
                var dockstate = DockingHelper.GetState(parentEl);
                if (dockstate != DockState.Dock)
                {
                    mainWindow.SetState(parentEl, DockState.Dock);
                }

                //DockingManager.SetTargetNameInDockedMode(el, name);
            }
        }

        public void SetChangedDocumentTitle(FrameworkElement el, bool bSet)
        {
            if (bShuttingDown)
                return;
            mainWindow.Dispatcher.InvokeIfRequired(
            () =>
            {
                String title = DockingHelper.GetHeader(el) as String;
                if (title == null)
                    return;
                if (bSet && !title.Contains(SuffixChangedDocument))
                {
                    title += SuffixChangedDocument;
                    DockingHelper.SetHeader(el, title);
                }
                else if (!bSet && title.Contains(SuffixChangedDocument))
                {
                    title = title.Trim(SuffixChangedDocument.ToCharArray());
                    DockingHelper.SetHeader(el, title);
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
                    DockingHelper.SetHeader(el, title);
                }
                else
                {
                    title = title.Trim(SuffixChangedDocument.ToCharArray());
                    DockingHelper.SetHeader(el, title);
                }
            });
        }

        FrameworkElement GetElementFromName(String name)
        {
            if (bShuttingDown)
                return null;

            return mainWindow.dockingManager.GetElementFromName(name);
        }

        FrameworkElement GetElement(String header)
        {
            if (bShuttingDown)
                return null;

            return mainWindow.dockingManager.GetElementFromHeader(header, SuffixChangedDocument);
        }

        public void FlashDockedElement(String header)
        {
            if (bShuttingDown)
                return;

            mainWindow.Dispatcher.BeginInvoke(
            (Action)(() =>
            {
                FrameworkElement element = GetElement(header);
                if (element != null)
                {
                    var state = DockingHelper.GetState(element);
                    var panel = DockLayoutManager.GetLayoutItem(element);

                    if (panel != null)
                        mainWindow.dockingManager.ActivateDockItem(panel);

                    mainWindow.Refresh();

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

            mainWindow.dockingManager.HideAllAutoHideWindows();
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
                //TODO (Notifications)
                //mainWindow.notifyIcon.BalloonTipText = Content;
                //mainWindow.notifyIcon.BalloonTipTitle = Title;
                //if (!String.IsNullOrEmpty(mainWindow.notifyIcon.BalloonTipText))
                //    mainWindow.notifyIcon.ShowBalloonTip(nTimeout);
            });
        }

        public event EventHandler EasyModeChanged;
        virtual public void OnEasyModeChanged(Object sender)
        {
            ToggleEasyModeConfig();
            EasyModeChanged?.Invoke(sender, EventArgs.Empty);
        }

        public bool IsInEasyMode
        {
            get
            {
                return mainWindow.IsInEasyMode;
            }
        }

        public string ThemeKey
        {
            get
            {
                return mainWindow.ThemeKey;
            }
        }

        public event EventHandler WorkspaceLoading;

        virtual public void OnWorkspaceLoading(Object sender)
        {
            WorkspaceLoading?.Invoke(sender, EventArgs.Empty);
        }

        public event EventHandler WorkspaceLoaded;

        virtual public void OnWorkspaceLoaded(Object sender)
        {
            WorkspaceLoaded?.Invoke(sender, EventArgs.Empty);
            IsWorkspaceLoaded = true;
        }

        public event EventHandler ContentRendered;

        virtual public void OnContentRendered(Object sender)
        {
            ContentRendered?.Invoke(sender, EventArgs.Empty);
        }

        public event EventHandler<CancelEventArgs> Closing;

        virtual public void OnClosing(Object sender, CancelEventArgs e)
        {
            Closing?.Invoke(sender, e);
        }

        public event EventHandler Closed;

        virtual public void OnClosed(Object sender)
        {
            Closed?.Invoke(sender, EventArgs.Empty);
        }

        public event RoutedEventHandler WindowActivated;

        //virtual public void OnWindowActivated(Object sender, RoutedEventArgs e)
        //{
        //    WindowActivated?.Invoke(sender, e);
        //}

        public event RoutedEventHandler WindowDeactivated;

        //virtual public void OnWindowDeactivated(Object sender, RoutedEventArgs e)
        //{
        //    WindowDeactivated?.Invoke(sender, e);
        //}

        public event RoutedEventHandler WindowDragEnd;

        //virtual public void OnWindowDragEnd(Object sender, RoutedEventArgs e)
        //{
        //    WindowDragEnd?.Invoke(sender, e);
        //}

        public event RoutedEventHandler WindowDragStart;

        //virtual public void OnWindowDragStart(Object sender, RoutedEventArgs e)
        //{
        //    WindowDragStart?.Invoke(sender, e);
        //}

        public event RoutedEventHandler WindowVisibilityChanged;

        virtual public void OnWindowVisibilityChanged(Object sender, RoutedEventArgs e)
        {
            WindowVisibilityChanged?.Invoke(sender, e);
        }

        public event RoutedEventHandler BeforeContextMenuOpen;

        //virtual public void OnBeforeContextMenuOpen(Object sender, RoutedEventArgs e)
        //{
        //    BeforeContextMenuOpen?.Invoke(sender, e);
        //}

        public event RoutedEventHandler AutoHideAnimationStart;

        virtual public void OnAutoHideAnimationStart(Object sender, RoutedEventArgs e)
        {
            AutoHideAnimationStart?.Invoke(sender, e);
        }

        public event RoutedEventHandler AutoHideAnimationStop;

        virtual public void OnAutoHideAnimationStop(Object sender, RoutedEventArgs e)
        {
            AutoHideAnimationStop?.Invoke(sender, e);
        }

        public event EventHandler<CancelEventArgs> ActiveWindowChanging;

        virtual public void OnActiveWindowChanging(DependencyObject d, DevExpress.Xpf.Docking.Base.ItemCancelEventArgs e)
        {
            CancelEventArgs cea = new CancelEventArgs();
            ActiveWindowChanging?.Invoke(d, cea);
            e.Cancel = cea.Cancel;
        }

        public event PropertyChangedCallback ActiveWindowChanged;

        virtual public void OnActiveWindowChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue is FrameworkElement)
            {
                var state = GetElementDockState(e.OldValue as FrameworkElement);
                if (state == DockState.Document)
                    lastActive = e.OldValue;
            }
            ActiveWindowChanged?.Invoke(d, e);
        }

        public event OnCloseTabsEventHandler CloseAllTabs;

        //virtual public void OnCloseAllTabs(object sender, CloseTabEventArgs e)
        //{
        //    CloseAllTabs?.Invoke(sender, e);
        //    lastActive = null;
        //}

        public event CloseButtonEventHandler CloseButtonClick;

        virtual public void OnCloseButtonClick(object sender, CloseButtonEventArgs e)
        {
            CloseButtonClick?.Invoke(sender, e);

            if (e.TargetItem == lastActive)
                lastActive = null;
        }

        public event OnCloseTabsEventHandler CloseOtherTabs;

        //virtual public void OnCloseOtherTabs(object sender, CloseTabEventArgs e)
        //{
        //    CloseOtherTabs?.Invoke(sender, e);

        //    if (e.TargetTabItem == lastActive)
        //        lastActive = null;
        //}

        public event DockStateHandler DockStateChanged;

        virtual public void OnDockStateChanged(FrameworkElement sender, DockStateEventArgs e)
        {
            DockStateChanged?.Invoke(sender, e);
        }

        public event EventHandler<CancelEventArgs> DockItemRestoring;

        virtual public void OnDockItemRestoring(Object sender, CancelEventArgs e)
        {
            DockItemRestoring?.Invoke(sender, e);
        }

        public event EventHandler DockItemRestored;

        virtual public void OnDockItemRestored(Object sender)
        {
            DockItemRestored?.Invoke(sender, EventArgs.Empty);
        }

        public event ElementHiddenEventHandler ElementHidden;

        //virtual public void OnElementHidden(object sender)
        //{
        //    ElementHidden?.Invoke(sender);
        //}

        public event ElementShownEventHandler ElementShown;

        //virtual public void OnElementShown(object sender)
        //{
        //    ElementShown?.Invoke(sender);
        //}

        public event SelectionChangedEventHandler RibbonSelectionChanged;

        //virtual public void OnRibbonSelectionChanged(object sender, SelectionChangedEventArgs e)
        //{
        //    RibbonSelectionChanged?.Invoke(sender, e);
        //}

        public void AddMenuItem(MenuItem item, int nPos, CommandBindingCollection commandBinding)
        {

        }

        public void RemoveMenuItem(MenuItem item, CommandBindingCollection commandBinding)
        {

        }

        public void SetDockedElementIcon(FrameworkElement el, Brush b)
        {
            if (bShuttingDown || !(b is ImageBrush))
                return;

            DockingHelper.SetIcon(el, b as ImageBrush);
        }

        bool bShuttingDown;
        internal void Shutdown()
        {
            bShuttingDown = true;
        }

        void IdleExecutionRibbonPending()
        {
            
        }

        private void ExtractRibbonItems(ItemsControl item,
                                        List<RibbonButton> listQA, Dictionary<RibbonButton, RibbonBar> mapQA,
                                        List<RibbonButton> listMA, Dictionary<RibbonButton, RibbonBar> mapMA,
                                        List<RibbonButton> listBS, Dictionary<RibbonButton, RibbonBar> mapBS)
        {
            
        }

        void PromoteIdleRibbonPending()
        {
           
        }

        public void AddRibbonTabItem(RibbonTab item, int n, CommandBindingCollection c,
                                     String ctg, Color bc)
        {
            
        }

        public void AddRibbonsTabItem(List<RibbonTab> l, int n, CommandBindingCollection c,
                                      String ctg, Color bc)
        {
            
        }

        public void RemoveRibbonTabItem(RibbonTab item, CommandBindingCollection c, String ctg, Color bc)
        {
            
        }

        public void RemoveRibbonsTabItem(List<RibbonTab> l, CommandBindingCollection c, String ctg, Color bc)
        {
            
        }

        public void SetMainMenuVisibility(Visibility visibility)
        {
            
        }

        public void SetRibbonVisibility(Visibility visibility)
        {
            
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
            ContextContentChanging?.Invoke(sender, e);
        }

        public event EventHandler ContextContentChanged;

        virtual public void OnContextContentChanged(object sender)
        {
            ContextContentChanged?.Invoke(sender, EventArgs.Empty);
        }

        public event EventHandler RefreshCurrentContents;

        virtual public void OnRefreshCurrentContents(object sender)
        {
            RefreshCurrentContents?.Invoke(sender, EventArgs.Empty);
        }

        

        public event EventHandler<CancelEventArgs> ContextDocumentChanging;

        virtual public void OnContextDocumentChanging(object sender, CancelEventArgs e)
        {
            ContextDocumentChanging?.Invoke(sender, e);
        }

        public event EventHandler ContextDocumentChanged;

        virtual public void OnContextDocumentChanged(object sender)
        {
            ContextDocumentChanged?.Invoke(sender, EventArgs.Empty);
        }


        public FrameworkElement ActiveWindow
        {
            get
            {
                return mainWindow.dockingManager.GetActiveWindow();
            }
            set
            {
                mainWindow.Dispatcher.InvokeIfRequired(
                () =>
                {
                    mainWindow.dockingManager.SetActiveWindow(value);
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
            if (IdleExecutionPending != null)
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
            PromptDocumentEditorObject?.Invoke(sender, e);
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
            PromptSmartTagsEditorObject?.Invoke(sender, e);
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

        public bool LayoutLoadRequest(string path)
        {
            if (bShuttingDown)
                return false;

            bool bRet = false;
            mainWindow.Dispatcher.InvokeIfRequired(() =>
            {
                bRet = mainWindow.LoadDockMenuStates(path);
            });
            return bRet;
        }

        public bool LayoutSaveRequest(string path, UserControl layoutContent = null, VFS.FileSystemProviderBase fileSystemProvider = null)
        {
            if (bShuttingDown)
                return false;

            bool bRet = false;
            mainWindow.Dispatcher.InvokeIfRequired(() =>
            {
                if (layoutContent == null)
                    bRet = mainWindow.SaveDockMenuStates(path, fileSystemProvider);
                else
                    bRet = mainWindow.SaveDockStates(path, DockLayoutManager.GetLayoutItem(layoutContent), fileSystemProvider);
            });
            return bRet;
        }

        public void ReloadDockState(FrameworkElement el, String layoutFilePath, VFS.FileSystemProviderBase fileSystemProvider = null)
        {
            if (bShuttingDown)
                return;

            mainWindow.Dispatcher.InvokeIfRequired(() => {
                mainWindow.ReloadDockState(el, layoutFilePath, fileSystemProvider);
            });
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
            if (bDisposed)
                return;
            bDisposed = true;

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
            emptyLogViewerControl.Dispose();
            lockObject = null;
        }

        public bool IsComponentHidden(string componentName)
        {
            if (!IsInEasyMode || componentName == null)
                return false;

            if (hiddenComponents == null)
                LoadHiddenComponents();

            return hiddenComponents == null ? false : hiddenComponents.Contains(componentName);
        }

        void LoadHiddenComponents()
        {
            hiddenComponents = new List<string>();
            string filepath = string.Format("{0}\\{1}.xml", Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location), hiddenComponentsFileName);
            if (File.Exists(filepath))
            {
                try
                {
                    var xml = XDocument.Load(@filepath);
                    foreach (XElement node in xml.Root.Descendants("Name").ToList())
                    {
                        var componentName = (node.Value as String).Trim();
                        if (!String.IsNullOrEmpty(componentName))
                        {
                            //var doc = (from im in UriRisolver.GetListInstalledDocumentManagers() where im.TypeScheme == componentName select im).FirstOrDefault();
                            //if (doc == null || doc.isHidable)
                            hiddenComponents.Add(componentName);
                        }
                    }
                }
                catch (Exception ex) { }
            }
        }
        #endregion IDisposable Members
    }
}