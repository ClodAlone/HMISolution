using System;
using UFInterfaces.CoreHostComponents;
using UFInterfaces;
using System.Windows.Threading;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Reflection;
using System.IO;
using System.Windows.Media;
using System.Collections.Generic;
using System.ComponentModel;
using CommandExplorer.ComponentService;
using CommandExplorer;
using Utilities;
using System.Windows.Input;
using UFInterfaces.Commandable;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using DevExpress.Xpf.Core;
using IWorkspace = UFInterfaces.IWorkspace;
using UIMsgBoxAlertService.ComponentService;
using Utilities.WPF;
using DocumentManager.ComponentService;

namespace CommandExplorer.ComponentService
{
    public class CommandExplorerComponent : ComponentBase<ICommandExplorer>, ICommandExplorer, IDisposable
    {
        #region Declaration
        Object lockObject = new Object();
        IWorkspace workspace;
        IUIMsgBoxAlertService uiInterface;
        CommandExplorerUI commandExplorerUI;
        DispatcherOperation IdleExecutionPending;
        bool bLoaded;
        bool bEnableIdleCode;
        bool bPendingIdleCode;
        #endregion

        #region IUFInterfaceBase Members

        void IUFInterfaceBase.Initialize()
        {
            Dispatcher.CurrentDispatcher.InvokeIfRequired(() =>
                {
                    GetComponentInterfaces();
                    CreateCommandExplorer();
                });
        }
        #endregion

        private static BitmapImage GetControlImage()
        {
            var bm = SharedResources.Helpers.ResourceManager.GetCommonImage("CommandExplorer", "CEEditor");
            return bm;
        }

        private void GetComponentInterfaces()
        {
            if (workspace == null)
                workspace = GetService(typeof(IWorkspace)) as IWorkspace;
            if (workspace == null)
                throw new NotImplementedException("Expecting the missing IWorkpsace Interface");
            if (uiInterface == null)
                uiInterface = GetService(typeof(IUIMsgBoxAlertService)) as IUIMsgBoxAlertService;
        }

        ContentControl emptyControl;
        public void CreateCommandExplorer()
        {
            lock (lockObject)
            {
                if (commandExplorerUI != null)
                    return;

                if (emptyControl == null)
                {
                    emptyControl = new ContentControl();

                    workspace.AutoHideAnimationStop += workspace_AutoHideAnimationStop;
                    workspace.AutoHideAnimationStart += workspace_AutoHideAnimationStart;
                    workspace.ContextContentChanging += workspace_ContextContentChanging;
                    workspace.ContextContentChanged += workspace_ContextContentChanged;
                    workspace.DockStateChanged += workspace_DockStateChanged;
                    workspace.ActiveWindowChanged += workspace_ActiveWindowChanged;
                    workspace.ContentRendered += workspace_ContentRendered;
                    workspace.RefreshCurrentContents += workspace_RefreshCurrentContents;

                    if (workspace.HasContentRendered())
                        bEnableIdleCode = true;

                    BitmapImage bm = GetControlImage();

                    // workspace.SetDesiredSideMode(commandExplorerUI, "SmartTagsControl");
                    workspace.AddDockingChildren(emptyControl, Properties.Resources.CommandExplorer_Title, UFInterfaces.DockState.AutoHidden, UFInterfaces.DockSide.Bottom, false, itemID: nameof(CommandExplorerComponent));
                    workspace.SetDockedElementIcon(emptyControl, new ImageBrush(bm));
                    lastDockSide = workspace.GetElementDockSide(emptyControl);
                }
                else
                {
                    using (var cursor = new WaitCursor())
                    {
                        commandExplorerUI = new CommandExplorerUI();

                        commandExplorerUI.Loaded += (o, e) =>
                        {
                            if (bEnableIdleCode)
                            {
                                if (!bLoaded && commandExplorerUI != null && commandExplorerUI.IsVisible)
                                {
                                    bLoaded = true;
                                    if (workspace.GetElementDockState(emptyControl) == UFInterfaces.DockState.Document)
                                        PromoteCodeToIdle(true);
                                }
                            }
                        };
                        commandExplorerUI.Unloaded += (o, e) =>
                        {
                            if (bEnableIdleCode)
                                bLoaded = false;
                        };

                        commandExplorerUI.TabControl.SelectionChanged += (o, e) =>
                        {
                            if (e.NewSelectedItem is DXTabItem)
                            {
                                var tabitem = e.NewSelectedItem as DXTabItem;
                                if (!(tabitem.Content is CommandEditorUI))
                                {
                                    var fe = tabitem.Content as FrameworkElement;
                                    if (fe != null && fe.Tag is ICommandable)
                                    {
                                        var commandable = fe.Tag as ICommandable;
                                        var isWebHMIProject = IsWebHMIProject();
                                        bool isSupported = !isWebHMIProject || commandable.WebHMISupported;
                                        FrameworkElement editor = null;
                                        if (isSupported)
                                        {
                                            editor = new CommandEditorUI(false, isWebHMIProject);
                                            editor.ClearValue(FrameworkElement.WidthProperty);
                                            editor.ClearValue(FrameworkElement.HeightProperty);

                                            editor.DataContext = commandable;
                                        }
                                        else
                                        {
                                            editor = new TextBlock() { HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center, Text = Properties.Resources.WebHMINotSupported };
                                        }

                                        tabitem.Content = editor;
                                    }
                                }
                            }
                        };

                        workspace.SetDesiredHeightAndWidthInDockedMode(emptyControl, commandExplorerUI.Height, commandExplorerUI.Width);
                        commandExplorerUI.ClearValue(FrameworkElement.WidthProperty);
                        commandExplorerUI.ClearValue(FrameworkElement.HeightProperty);

                        emptyControl.Content = commandExplorerUI;
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

        UFInterfaces.DockSide lastDockSide;
        void workspace_ActiveWindowChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue == emptyControl)
            {
                workspace.ActiveWindowChanging -= workspace_ActiveWindowChanging;

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

                try
                {
                    workspace.BeginEdit();
                }
                catch
                {

                }

                workspace.ActiveWindowChanging += workspace_ActiveWindowChanging;
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

        bool CheckErrorBindings(bool showerror = true)
        {

            bool bRet = true;
            if (commandExplorerUI == null)
                return false;

            foreach (var control in commandExplorerUI.TabControl.Items)
            {
                DXTabItem ti = control as DXTabItem;
                if (ti == null)
                    continue;
                CommandEditorUI ae = ti.Content as CommandEditorUI;
                if (ae != null)
                {
                    ae.ForcePropertyControlUIFocus();
                    bRet &= ae.CheckErrorBindings();
                    if (!bRet)
                    {
                        if (showerror && uiInterface != null)
                            uiInterface.ShowWarning(Properties.Resources.InvalidValues);
                        ae.ForcePropertyControlUIFocus();
                        break;
                    }

                }
            }
            return !bRet;
        }
        void workspace_ActiveWindowChanging(object sender, CancelEventArgs e)
        {
            IInputElement focusedElement = null;
            if (commandExplorerUI != null)
                focusedElement = FocusManager.GetFocusedElement(Window.GetWindow(commandExplorerUI));

            e.Cancel = CheckErrorBindings();
            if (e.Cancel)
            {
                workspace.FlashDockedElement(emptyControl);
                var dockState = workspace.GetElementDockState(emptyControl);
                if (dockState == UFInterfaces.DockState.AutoHidden)
                    workspace.SetElementDockState(emptyControl, UFInterfaces.DockState.Dock);
            }
            else
            {
                CheckChanges();
                workspace.EndEdit();
            }

            if (!e.Cancel && commandExplorerUI != null && focusedElement != null &&
                focusedElement != FocusManager.GetFocusedElement(Window.GetWindow(commandExplorerUI)))
            {
                focusedElement.Focusable = true;
                focusedElement.Focus();
                FocusManager.SetFocusedElement(Window.GetWindow(commandExplorerUI), focusedElement);
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

        internal bool IsWebHMIProject()
        {
            IDocument selectedDocument = workspace.ContextDocument as IDocument;
            bool isWebHMIProject = selectedDocument != null && selectedDocument.ProjectType == ProjectType.WebHMI.ToString();
            return isWebHMIProject;
        }

        bool bContextObjectChanged;
        void workspace_ContextContentChanged(object sender, EventArgs e)
        {
            if (emptyControl != null)
                PromoteCodeToIdle(false);
            bContextObjectChanged = true;
        }

        void workspace_ContextContentChanging(object sender, CancelEventArgs e)
        {
        }

        bool bAutoHideVisible = false;
        bool bWasAutoHideVisible = false;
        void workspace_AutoHideAnimationStart(object sender, RoutedEventArgs e)
        {
            if (e.OriginalSource == emptyControl)
            {
                CreateCommandExplorer();

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

        void workspace_RefreshCurrentContents(object sender, EventArgs e)
        {
            if (emptyControl == null || emptyControl.Visibility != Visibility.Visible)
                return;

            var dockstate = workspace.GetElementDockState(emptyControl);
            bool bVisible = emptyControl.Visibility == Visibility.Visible &&
                (dockstate == UFInterfaces.DockState.Document && bLoaded || dockstate != UFInterfaces.DockState.Document && dockstate != UFInterfaces.DockState.AutoHidden ||
                 bAutoHideVisible && dockstate == UFInterfaces.DockState.AutoHidden);
            if (!bVisible)
                return;

            bContextObjectChanged = true;
            IdleExecution();
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

        private void IdleExecution()
        {
            using (new WaitCursor())
            {
                CreateCommandExplorer();
                if (!bContextObjectChanged)
                    return;
                bContextObjectChanged = false;

                // CheckChanges();

                commandExplorerUI.TabControl.Visibility = Visibility.Collapsed;

                for (int i = commandExplorerUI.TabControl.Items.Count - 1; i >= 0; i--)
                {
                    var content = (commandExplorerUI.TabControl.Items[i] as DXTabItem).Content as CommandEditorUI;
                    content?.Dispose();
                    if (commandExplorerUI.TabControl.Items[i] is IDisposable)
                        (commandExplorerUI.TabControl.Items[i] as IDisposable).Dispose();
                }
                
                commandExplorerUI.TabControl.Items.Clear();
                
                List<Object> list = new List<Object>();
                if (workspace.ContextObject != null)
                {
                    if (workspace.ContextObject is ICommandable)
                        list.Add(workspace.ContextObject);
                    else
                        list.Add(workspace.ContextObject is IEntityReference && (workspace.ContextObject as IEntityReference).ContainedObject != null ? (workspace.ContextObject as IEntityReference).ContainedObject : workspace.ContextObject);
                }
                else if (workspace.ContextObjects != null)
                {
                    foreach (Object o in workspace.ContextObjects)
                    {
                        if (o is ICommandable)
                            list.Add(o);
                        else
                            list.Add(o is IEntityReference && (o as IEntityReference).ContainedObject != null ? (o as IEntityReference).ContainedObject : o);
                    }
                }

                var listCommandable = new List<ICommandable>();
                var mapTitle = new Dictionary<ICommandable, String>();
                list.ForEach(obj =>
                {
                    var source = obj;
                    if (obj is ContentControl && (obj as ContentControl).Content is UIElement &&
                        !(obj is UserControl))
                    {
                        obj = (obj as ContentControl).Content;
                    }

                    try
                    {
                        if (!(obj is ICommandSource) || !(obj is ToggleButton))
                        {
                            if (obj is ICommandable)
                            {
                                listCommandable.Add(obj as ICommandable);

                                mapTitle[obj as ICommandable] = (obj as ICommandable).Name;
                            }
                            else
                            {
                                var listFriends = workspace.GetFriendObjects(source, null);
                                if (listFriends != null)
                                {
                                    foreach (var v in listFriends)
                                    {
                                        if (v is ICommandable && obj is ICommandSource && !(obj is ToggleButton))
                                        {
                                            listCommandable.Add(v as ICommandable);

                                            mapTitle[v as ICommandable] = (v as ICommandable).Name;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Trace.TraceError(ex.ToString());
                    }
                });

                commandExplorerUI.TabControl.BeginInit();
                listCommandable.ForEach(animatable =>
                {
                    DXTabItem tabitem = new DXTabItem
                    {
                        Header = mapTitle[animatable],
                        Content = new FrameworkElement() { Tag = animatable },
                        IsSelected = false
                    };
                    tabitem.InitItemTemplate();
                    commandExplorerUI.TabControl.Items.Add(tabitem);
                });
                commandExplorerUI.TabControl.EndInit();

                if (commandExplorerUI.TabControl.Items.Count > 0)
                {
                    commandExplorerUI.TabControl.SelectedIndex = 0;
                    commandExplorerUI.TabControl.Visibility = Visibility.Visible;
                    commandExplorerUI.TextNoItems.Visibility = Visibility.Collapsed;
                }
                else
                    commandExplorerUI.TextNoItems.Visibility = Visibility.Visible;
            }
        }

        void ForceCommandExplorerUIFocus()
        {
            if (commandExplorerUI != null)
            {
                commandExplorerUI.Focusable = true;
                commandExplorerUI.Focus();
                FocusManager.SetFocusedElement(Window.GetWindow(commandExplorerUI), commandExplorerUI);
            }
        }

        void CheckChanges()
        {
            if (commandExplorerUI == null)
                return;

            foreach (var control in commandExplorerUI.TabControl.Items)
            {
                DXTabItem ti = control as DXTabItem;
                if (ti == null)
                    continue;
                CommandEditorUI ae = ti.Content as CommandEditorUI;
                if (ae != null)
                    ae.PropagateChanges();
            }
        }


        #region ICommandExplorer Members

        public UserControl control
        {
            get
            {
                CommandEditorUI popupControl = new CommandEditorUI();
                return popupControl;
            }
        }

        public void PropagateChanges(UserControl commandlist)
        {
            var currControl = commandlist as CommandEditorUI;
            if (currControl != null)
                currControl.PropagateChanges();
            
        }
        public void SetSync(UserControl commandlist, bool sync)
        { 
            var currControl = commandlist as CommandEditorUI;
            if (currControl != null)
                currControl.Synchronous = sync;

        }

        DispatcherOperation pendingActivation;
        public void Activate(int indexTab = -1)
        {
            if (emptyControl == null || pendingActivation != null)
                return;

            pendingActivation = Dispatcher.CurrentDispatcher.BeginInvokeAsynchronouslyInApplicationIdle(() =>
            {
                workspace.FlashDockedElement(emptyControl);
                if (commandExplorerUI != null && indexTab != -1)
                {
                    workspace.ForceRefreshCurrentContents();
                    if (commandExplorerUI.TabControl.Items.Count > indexTab)
                        commandExplorerUI.TabControl.SelectedIndex = indexTab;
                }
                pendingActivation = null;
            });
        }

        #endregion

        #region IDisposable Members

        void IDisposable.Dispose()
        {
            commandExplorerUI = null;

            if (workspace != null)
            {
                workspace.AutoHideAnimationStop -= workspace_AutoHideAnimationStop;
                workspace.AutoHideAnimationStart -= workspace_AutoHideAnimationStart;
                workspace.ContextContentChanging -= workspace_ContextContentChanging;
                workspace.ContextContentChanged -= workspace_ContextContentChanged;
                workspace.DockStateChanged -= workspace_DockStateChanged;
                workspace.ActiveWindowChanged -= workspace_ActiveWindowChanged;
                workspace.ContentRendered -= workspace_ContentRendered;
                workspace.RefreshCurrentContents -= workspace_RefreshCurrentContents;
            }

            if (IdleExecutionPending != null)
            {
                IdleExecutionPending.Abort();
                IdleExecutionPending = null;
            }

            lockObject = null;

            if (commandExplorerUI != null)
            {
                foreach (DXTabItem item in commandExplorerUI.TabControl.Items)
                {
                    if (item.Content is IDisposable)
                        (item.Content as IDisposable).Dispose();
                }
                commandExplorerUI.TabControl.Items.Clear();
                //commandExplorerUI.TabControl.Dispose();
            }
        }

        #endregion
    }
}
