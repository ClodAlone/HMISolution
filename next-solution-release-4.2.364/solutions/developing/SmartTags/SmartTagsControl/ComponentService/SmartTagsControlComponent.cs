using System;
using UFInterfaces.CoreHostComponents;
using UFInterfaces;
using System.Windows.Threading;
using System.Collections;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Reflection;
using System.IO;
using System.Windows.Media;
using System.Collections.Generic;
using System.ComponentModel;
using Utilities;
using System.Windows.Controls;
using DocumentManager.ComponentService;
using SmartTagsControl.ViewModel;
using DevExpress.Xpf.NavBar;

namespace SmartTagsControl.ComponentService
{
    public class SmartTagsControlComponent : ComponentBase<ISmartTagsControl>, ISmartTagsControl, IDisposable
    {
        #region Declaration
        Object lockObject = new Object();
        SmartTagsControlUI smartTagsControlUI;
        SmartTagsViewModel smartTagsViewModel;
        Object PromoteSelectingObject;
        IList PromoteSelectingObjects;
        DispatcherOperation IdleExecutionPending;
        bool bLoaded;
        bool bEnableIdleCode;
        bool bPendingIdleCode;

        public static IWorkspace workspace { get; protected set; }
        public static bool workspaceAvailable { get { return workspace != null; } }

        public static SmartTagsControlComponent smartTagsControlComponent { get; protected set; }

        public static void QueryInterfaces()
        {
            if (workspace == null)
                workspace = smartTagsControlComponent.GetService(typeof(IWorkspace)) as IWorkspace;
        }
        #endregion

        #region IUFInterfaceBase Members

        void IUFInterfaceBase.Initialize()
        {
            if (smartTagsControlComponent == null)
                smartTagsControlComponent = this;

            QueryInterfaces();

            if (!workspaceAvailable)
                throw new NotImplementedException("Expecting the missing IWorkpsace Interface");

            workspace.WorkspaceLoading += workspace_WorkspaceLoading;
        }

        #endregion

        void workspace_WorkspaceLoading(object sender, EventArgs e)
        {
            CreateSmartTagsControl();
        }

        private static BitmapImage GetControlImage()
        {
            var bm = SharedResources.Helpers.ResourceManager.GetCommonImage("SmartTags", "STEditor");
            return bm;
        }

        ContentControl emptyControl;
        public void CreateSmartTagsControl()
        {
            lock (lockObject)
            {
                if (smartTagsControlUI != null)
                    return;

                if (emptyControl == null)
                {
                    emptyControl = new ContentControl();

                    if (workspace.HasContentRendered())
                        bEnableIdleCode = true;

                    BitmapImage bm = GetControlImage();

                    workspace.AddDockingChildren(emptyControl, Properties.Resources.SmartTags_Title, UFInterfaces.DockState.AutoHidden, UFInterfaces.DockSide.Right, false, itemID: nameof(SmartTagsControlComponent));
                    lastDockSide = workspace.GetElementDockSide(emptyControl);
                    workspace.SetDockedElementIcon(emptyControl, new ImageBrush(bm));

                    workspace.AutoHideAnimationStop += workspace_AutoHideAnimationStop;
                    workspace.AutoHideAnimationStart += workspace_AutoHideAnimationStart;
                    workspace.ContextContentChanging += workspace_ContextContentChanging;
                    workspace.ContextContentChanged += workspace_ContextContentChanged;
                    workspace.ActiveWindowChanged += workspace_ActiveWindowChanged;
                    workspace.DockStateChanged += workspace_DockStateChanged;
                    workspace.ContentRendered += workspace_ContentRendered;
                }
                else
                {
                    using (var CursorHelper = new WaitCursor())
                    {
                        smartTagsViewModel = new SmartTagsViewModel();
                        smartTagsControlUI = new SmartTagsControlUI()
                        {
                            DataContext = smartTagsViewModel
                        };

                        smartTagsControlUI.Loaded += (o, e) =>
                        {
                            if (bEnableIdleCode)
                            {
                                if (!bLoaded && smartTagsControlUI != null && smartTagsControlUI.IsVisible)
                                {
                                    bLoaded = true;
                                    if (workspace.GetElementDockState(emptyControl) == UFInterfaces.DockState.Document)
                                        PromoteCodeToIdle(true);
                                }
                            }
                        };
                        smartTagsControlUI.Unloaded += (o, e) =>
                        {
                            if (bEnableIdleCode)
                                bLoaded = false;
                        };

                        workspace.SetDesiredHeightAndWidthInDockedMode(emptyControl, smartTagsControlUI.Height, smartTagsControlUI.Width);
                        smartTagsControlUI.ClearValue(FrameworkElement.WidthProperty);
                        smartTagsControlUI.ClearValue(FrameworkElement.HeightProperty);

                        emptyControl.Content = smartTagsControlUI;
                    }
                }
            }
        }

        void workspace_ContentRendered(object sender, EventArgs e)
        {
            bEnableIdleCode = true;
            if (bPendingIdleCode)
            {
                bPendingIdleCode = false;
                PromoteCodeToIdle(false);
            }
        }

        void workspace_DockStateChanged(FrameworkElement sender, UFInterfaces.DockStateEventArgs e)
        {
            if (sender == emptyControl && e.OldState == UFInterfaces.DockState.AutoHidden)
            {
                if (emptyControl != null)
                    emptyControl.Visibility = Visibility.Visible;
                PromoteCodeToIdle(true);
            }
            else if (sender == emptyControl && (e.NewState == UFInterfaces.DockState.Hidden || e.NewState == UFInterfaces.DockState.AutoHidden))
            {
                if (emptyControl != null)
                {
                    emptyControl.Visibility = Visibility.Collapsed;
                    bAutoHideVisible = false;
                }
            }
        }

        UFInterfaces.DockSide lastDockSide;
        bool bActionWasOnButton;
        void workspace_ActiveWindowChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue == emptyControl)
            {
                smartTagsControlUI.btnCancel.ItemClick -= btnCancel_Click;
                smartTagsControlUI.btnOK.ItemClick -= btnOK_Click;
                smartTagsControlUI.btnCancel.IsEnabled = false;
                smartTagsControlUI.btnOK.IsEnabled = false;
                if (!bActionWasOnButton)
                {
                    smartTagsViewModel.EndEdit();
                    workspace.EndEdit();
                    OnAcceptChanges();
                }

                lastDockSide = workspace.GetElementDockSide(emptyControl);
                if (lastDockSide == UFInterfaces.DockSide.Tabbed && !workspace.GetElementIsSelectedTab(emptyControl) &&
                    workspace.GetElementDockState(emptyControl) != DockState.Float)
                {
                    if (emptyControl != null)
                        emptyControl.Visibility = Visibility.Collapsed;
                }
            }
            if (e.NewValue == emptyControl)
            {
                if (lastDockSide == UFInterfaces.DockSide.Tabbed && workspace.GetElementIsSelectedTab(emptyControl))
                {
                    if (emptyControl != null)
                        emptyControl.Visibility = Visibility.Visible;
                    PromoteCodeToIdle(true);
                }

                bActionWasOnButton = false;

                if (smartTagsControlUI != null)
                {
                    smartTagsViewModel.BeginEdit();
                    workspace.BeginEdit();
                    OnPrepareChanges();

                    smartTagsControlUI.btnCancel.IsEnabled = true;
                    smartTagsControlUI.btnOK.IsEnabled = true;

                    smartTagsControlUI.btnCancel.ItemClick += btnCancel_Click;
                    smartTagsControlUI.btnOK.ItemClick += btnOK_Click;
                }
            }

            if (emptyControl != null && (e.OldValue == emptyControl || e.NewValue == emptyControl))
            {
                var dockstate = workspace.GetElementDockState(emptyControl);
                if (dockstate == UFInterfaces.DockState.Document)
                {
                    if (e.NewValue == emptyControl && bLoaded)
                        emptyControl.Visibility = Visibility.Visible;
                    //else if (e.OldValue == emptyControl)
                    //    emptyControl.Visibility = Visibility.Collapsed;
                }
            }
        }

        void btnOK_Click(object sender, RoutedEventArgs e)
        {
            bActionWasOnButton = true;
            smartTagsViewModel.EndEdit();
            workspace.EndEdit();
            OnAcceptChanges();
            workspace.ActivatePreviousActiveElement();
        }

        void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            if (workspace.ContextDocument == null)
                return;

            bActionWasOnButton = true;
            smartTagsViewModel.CancelEdit();
            workspace.CancelEdit();
            OnCancelChanges();
            workspace.ActivatePreviousActiveElement();
        }

        bool bContextObjectChanged;
        void workspace_ContextContentChanged(object sender, EventArgs e)
        {
            if (workspace.ContextObject != null)
            {
                SelectedObjects = null;
                SelectedObject = workspace.ContextObject;
            }
            else
            {
                SelectedObject = null;
                SelectedObjects = workspace.ContextObjects;
            }
            bContextObjectChanged = true;
        }

        void workspace_ContextContentChanging(object sender, CancelEventArgs e)
        {
            if (smartTagsControlUI != null)
                OnAcceptChanges();
        }

        bool bAutoHideVisible = false;
        bool bWasAutoHideVisible = false;
        void workspace_AutoHideAnimationStart(object sender, RoutedEventArgs e)
        {
            if (e.OriginalSource == emptyControl)
            {
                CreateSmartTagsControl();

                bAutoHideVisible = true;
                if (idleOperation != null)
                {
                    idleOperation.Abort();
                    idleOperation = null;
                }
                // if (bAutoHideVisible)
                {
                    if (emptyControl != null)
                        emptyControl.Visibility = Visibility.Visible;
                }
            }
        }

        DispatcherOperation idleOperation;
        void workspace_AutoHideAnimationStop(object sender, RoutedEventArgs e)
        {
            if (e.OriginalSource == emptyControl)
            {
                if (emptyControl != null/* && workspace.ActiveWindow != emptyControl*/)
                {
                    //var dockstate = workspace.GetElementDockState(emptyControl);
                    //if (dockstate == DockState.AutoHidden)
                    {
                        if (bWasAutoHideVisible)
                        {
                            bWasAutoHideVisible = false;
                            if (idleOperation == null)
                            {
                                idleOperation = emptyControl.Dispatcher.BeginInvokeAsynchronouslyInApplicationIdle(() =>
                                {
                                    if (!Utilities.WPF.DockingHelper.GetLayoutItemVisible(emptyControl))
                                        emptyControl.Visibility = Visibility.Collapsed;
                                    else
                                        bAutoHideVisible = bWasAutoHideVisible = true;
                                });

                                idleOperation.Completed += (o, ev) =>
                                {
                                    idleOperation = null;
                                };
                            }
                            bAutoHideVisible = false;
                        }
                        else
                        {
                            bWasAutoHideVisible = true;
                            PromoteCodeToIdle(false);
                        }
                    }
                }
                else
                    PromoteCodeToIdle(true);
            }
        }

        private void PromoteSelectionObject(Object ob)
        {
            lock (lockObject)
            {
                PromoteSelectingObject = ob;
                PromoteSelectingObjects = null;
                PromoteCodeToIdle(false);
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
            if (!bEnableIdleCode)
            {
                bPendingIdleCode = true;
                return;
            }

            var dockstate = workspace.GetElementDockState(emptyControl);
            bool bVisible = emptyControl.Visibility == Visibility.Visible &&
                (dockstate == UFInterfaces.DockState.Document && bLoaded || dockstate != UFInterfaces.DockState.Document && dockstate != UFInterfaces.DockState.AutoHidden ||
                 bAutoHideVisible && dockstate == UFInterfaces.DockState.AutoHidden);

            if (IdleExecutionPending == null && (bSynchro || bVisible))
            {
                Action action = () => IdleExecution();
                Dispatcher disp = emptyControl != null ? emptyControl.Dispatcher : Dispatcher.CurrentDispatcher;
                IdleExecutionPending = disp.BeginInvoke(action, bSynchro ? DispatcherPriority.Send : DispatcherPriority.Background);
                IdleExecutionPending.Completed += (sender, e) => IdleExecutionPending = null;
            }
        }

        void UpdateCurrentSelection()
        {
            if (PromoteSelectingObject != null)
            {
                if (PromoteSelectingObject is IEntityReference &&
                    (PromoteSelectingObject as IEntityReference).ContainedObject != null &&
                    selectedObject != (PromoteSelectingObject as IEntityReference).ContainedObject)
                {
                    selectedObject = (PromoteSelectingObject as IEntityReference).ContainedObject;
                    selectedObjects = null;
                }
                else if (selectedObject != PromoteSelectingObject)
                {
                    selectedObject = PromoteSelectingObject;
                    selectedObjects = null;
                }
            }
            else if (PromoteSelectingObjects != null)
            {
                var list = new List<Object>();
                foreach (Object o in PromoteSelectingObjects)
                {
                    if (o is IEntityReference &&
                        (o as IEntityReference).ContainedObject != null)
                        list.Add((o as IEntityReference).ContainedObject);
                    else
                        list.Add(o);
                }

                bool bChanged = true;
                if (selectedObjects != null)
                {
                    bChanged = false;
                    foreach (Object o in list)
                    {
                        if (!selectedObjects.Contains(o))
                        {
                            bChanged = true;
                            break;
                        }
                    }
                }

                if (bChanged)
                {
                    if (list.Count == 0)
                    {
                        selectedObject = null;
                        selectedObjects = null;
                    }
                    else if (list.Count == 1)
                    {
                        selectedObject = list[0];
                        selectedObjects = null;
                    }
                    else
                    {
                        selectedObjects = list;
                        selectedObject = null;
                    }
                }
            }
            else
            {
                selectedObject = null;
                selectedObjects = null;
            }

            // retrive ISmartTagsUI control interfaces for editing the selected objects
            if (SmartTagsControlComponent.workspaceAvailable)
            {
                List<UserControl> editabileObjects = new List<UserControl>();
                if (SelectedObject != null)
                {
                    var control = SmartTagsControlComponent.workspace.GetSmartTagsEditorObject(SelectedObject);
                    if (control == null)
                        control = GetSmartTagsEditorObject(SelectedObject);
                    if (control != null && !editabileObjects.Contains(control))
                    {
                        control.DataContext = SelectedObject;
                        editabileObjects.Add(control);
                    }
                }
                else if (SelectedObjects != null && SelectedObjects.Count > 0)
                {
                    foreach (var obj in SelectedObjects)
                    {
                        var control = SmartTagsControlComponent.workspace.GetSmartTagsEditorObject(obj);
                        if (control == null)
                            control = GetSmartTagsEditorObject(obj);
                        if (control != null && !editabileObjects.Contains(control))
                        {
                            control.DataContext = obj;
                            editabileObjects.Add(control);
                        }
                    }
                }

                smartTagsViewModel.EditableObjects = editabileObjects;

                smartTagsControlUI.tabNavControl.Groups.Clear();
                foreach (UserControl control in smartTagsViewModel.EditableObjects)
                {
                    var newTabItem = new NavBarGroup()
                    {
                        Header = control.Name,
                        DisplaySource = DisplaySource.Content,
                        GroupScrollMode = ScrollMode.None,
                        Content = control
                    };

                    smartTagsControlUI.tabNavControl.Groups.Add(newTabItem);
                }

                smartTagsViewModel.BeginEdit();
            }
        }

        UserControl GetSmartTagsEditorObject(Object obj)
        {
            var type = obj.GetType();

            string rootPath = string.Format("{0}\\SmartTags", System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location));
            if (Directory.Exists(rootPath))
            {
                var list = FindAndLoadDLL.LoadDLLs<UserControl>(rootPath, String.Format("{0}.dll", type.Name), false);
                if (list.Count > 0)
                    return list[0];
            }

            return null;
        }

        private void IdleExecution()
        {
            using (new WaitCursor())
            {
                CreateSmartTagsControl();
                if (!bContextObjectChanged)
                    return;
                bContextObjectChanged = false;

                OnSelecting();

                UpdateCurrentSelection();

                OnSelected();
            }
        }

        #region ISmartTagsControl Members

        public event EventHandler PrepareChanges;
        virtual public void OnPrepareChanges()
        {
            EventHandler temp = PrepareChanges;
            if (temp != null)
                temp(this, EventArgs.Empty);
        }

        public event EventHandler Selecting;
        virtual public void OnSelecting()
        {
            EventHandler temp = Selecting;
            if (temp != null)
                temp(this, EventArgs.Empty);
        }

        public event EventHandler Selected;
        virtual public void OnSelected()
        {
            EventHandler temp = Selected;
            if (temp != null)
                temp(this, EventArgs.Empty);
        }

        public event EventHandler AcceptChanges;
        virtual public void OnAcceptChanges()
        {
            EventHandler temp = AcceptChanges;
            if (temp != null)
                temp(this, EventArgs.Empty);
        }

        public event EventHandler CancelChanges;
        virtual public void OnCancelChanges()
        {
            EventHandler temp = CancelChanges;
            if (temp != null)
                temp(this, EventArgs.Empty);
        }

        object selectedObject;
        public object SelectedObject
        {
            get
            {
                return PromoteSelectingObject != null ? PromoteSelectingObject : selectedObject;
            }
            set
            {
                PromoteSelectionObject(value);
            }
        }

        IList selectedObjects;
        public IList SelectedObjects
        {
            get
            {
                return PromoteSelectingObjects != null ? PromoteSelectingObjects : selectedObjects;
            }
            set
            {
                PromoteSelectionObject(value);
            }
        }

        #endregion

        #region IDisposable Members

        void IDisposable.Dispose()
        {
            PromoteSelectingObject = null;
            PromoteSelectingObjects = null;
            smartTagsControlUI = null;

            if (workspace != null)
            {
                workspace.AutoHideAnimationStop -= workspace_AutoHideAnimationStop;
                workspace.AutoHideAnimationStart -= workspace_AutoHideAnimationStart;
                workspace.WorkspaceLoaded -= workspace_WorkspaceLoading;
                workspace.ContextContentChanging -= workspace_ContextContentChanging;
                workspace.ContextContentChanged -= workspace_ContextContentChanged;
                workspace.DockStateChanged -= workspace_DockStateChanged;
                workspace.ActiveWindowChanged -= workspace_ActiveWindowChanged;
                workspace.ContentRendered -= workspace_ContentRendered;
            }

            if (IdleExecutionPending != null)
            {
                IdleExecutionPending.Abort();
                IdleExecutionPending = null;
            }

            lockObject = null;
        }

        #endregion
    }
}