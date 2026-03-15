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
using Tracing.ComponentService;
using UIMsgBoxAlertService.ComponentService;
using Utilities;
using System.Collections.Generic;
using UFInterfaces.Animatable;
using System.ComponentModel;
using System.Windows.Controls;
using DevExpress.Xpf.Core;
using IWorkspace = UFInterfaces.IWorkspace;
using System.Windows.Input;
using Utilities.WPF;
using DocumentManager.ComponentService;

namespace AnimationExplorer.ComponentService
{
    public class AnimationExplorerComponent : ComponentBase<IAnimationExplorer>, IAnimationExplorer, IDisposable
    {
        #region Declaration

        Object lockObject = new Object();
        IWorkspace workspace;
        ISimpleLogging simpleLogging;
        IUIMsgBoxAlertService uiInterface;
        AnimationExplorerUI animationExplorerUI;
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
                    CreateAnimationExplorer();
                });
        }
        #endregion

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            animationExplorerUI = null;

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
        }

        private static BitmapImage GetControlImage()
        {
            var bm = SharedResources.Helpers.ResourceManager.GetCommonImage("AnimationExplorer", "AEEditor");
            return bm;
        }

        private void GetComponentInterfaces()
        {
            if (workspace == null)
                workspace = GetService(typeof(IWorkspace)) as IWorkspace;
            if (workspace == null)
                throw new NotImplementedException("Expecting the missing IWorkpsace Interface");

            if (simpleLogging == null)
                simpleLogging = GetService(typeof(ISimpleLogging)) as ISimpleLogging;

            if (uiInterface == null)
                uiInterface = GetService(typeof(IUIMsgBoxAlertService)) as IUIMsgBoxAlertService;
        }

        ContentControl emptyControl;
        public void CreateAnimationExplorer()
        {
            lock (lockObject)
            {
                if (animationExplorerUI != null)
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

                    // workspace.SetDesiredSideMode(animationExplorerUI, "SmartTagsControl");
                    workspace.AddDockingChildren(emptyControl, Properties.Resources.AnimationExplorer_Title, UFInterfaces.DockState.AutoHidden, UFInterfaces.DockSide.Bottom, false, itemID: nameof(AnimationExplorerComponent));
                    workspace.SetDockedElementIcon(emptyControl, new ImageBrush(bm));
                    lastDockSide = workspace.GetElementDockSide(emptyControl);
                }
                else
                {
                    using (var cursor = new WaitCursor())
                    {
                        animationExplorerUI = new AnimationExplorerUI();

                        animationExplorerUI.Loaded += (o, e) =>
                        {
                            if (bEnableIdleCode)
                            {
                                if (!bLoaded && animationExplorerUI != null && animationExplorerUI.IsVisible)
                                {
                                    bLoaded = true;
                                    if (workspace.GetElementDockState(emptyControl) == UFInterfaces.DockState.Document)
                                        PromoteCodeToIdle(true);
                                }
                            }
                        };
                        animationExplorerUI.Unloaded += (o, e) =>
                        {
                            if (bEnableIdleCode)
                                bLoaded = false;
                        };

                        animationExplorerUI.TabControl.SelectionChanged += (o, e) =>
                        {
                            if (e.NewSelectedItem is DXTabItem)
                            {
                                var tabitem = e.NewSelectedItem as DXTabItem;
                                if (!(tabitem.Content is AnimationEditorUI))
                                {
                                    var fe = tabitem.Content as FrameworkElement;
                                    if (fe != null && fe.Tag is IAnimatable)
                                    {
                                        var animatable = fe.Tag as IAnimatable;
                                        var isWebHMIProject = IsWebHMIProject();
                                        var contained = (animatable as IEntityReference)?.ContainedObject;
                                        bool isSupported = !isWebHMIProject ||
                                                           !(animatable is IEntityReference && contained != null &&
                                                            (contained is UserControl) ||
                                                            (contained is ContentControl && !((contained as ContentControl).Content is Viewbox)) ||
                                                            (contained is Control && !(contained is ContentControl)));
                                        FrameworkElement editor = null;
                                        if (isSupported)
                                        {
                                            editor = new AnimationEditorUI(false, IsWebHMIProject());
                                            (editor as AnimationEditorUI).SetIsActive(workspace.ActiveWindow == emptyControl);
                                            editor.ClearValue(FrameworkElement.WidthProperty);
                                            editor.ClearValue(FrameworkElement.HeightProperty);

                                            editor.DataContext = animatable;
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

                        workspace.SetDesiredHeightAndWidthInDockedMode(emptyControl, animationExplorerUI.Height, animationExplorerUI.Width);
                        animationExplorerUI.ClearValue(FrameworkElement.WidthProperty);
                        animationExplorerUI.ClearValue(FrameworkElement.HeightProperty);

                        emptyControl.Content = animationExplorerUI;
                    }
                }
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

                SetIsActive(true);
            }
            if (e.OldValue == emptyControl)
            {
                workspace.ActiveWindowChanging -= workspace_ActiveWindowChanging;

                SetIsActive(false);

                lastDockSide = workspace.GetElementDockSide(emptyControl);
                if (lastDockSide == UFInterfaces.DockSide.Tabbed && !workspace.GetElementIsSelectedTab(emptyControl) &&
                    workspace.GetElementDockState(emptyControl) != DockState.Float)
                {
                    if (emptyControl != null)
                        emptyControl.Visibility = Visibility.Collapsed;
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

        bool CheckErrorBindings(bool showerror = true)
        {

            bool bRet = true;
            if (animationExplorerUI == null)
                return false;

            foreach (var control in animationExplorerUI.TabControl.Items)
            {
                DXTabItem ti = control as DXTabItem;
                if (ti == null)
                    continue;
                AnimationEditorUI ae = ti.Content as AnimationEditorUI;
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
            if (animationExplorerUI != null)
                focusedElement = FocusManager.GetFocusedElement(Window.GetWindow(animationExplorerUI));

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

            if (!e.Cancel && animationExplorerUI != null && focusedElement != null &&
                focusedElement != FocusManager.GetFocusedElement(Window.GetWindow(animationExplorerUI)))
            {
                focusedElement.Focusable = true;
                focusedElement.Focus();
                FocusManager.SetFocusedElement(Window.GetWindow(animationExplorerUI), focusedElement);
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

        bool bAutoHideVisible = false;
        bool bWasAutoHideVisible = false;
        void workspace_AutoHideAnimationStart(object sender, RoutedEventArgs e)
        {
            if (e.OriginalSource == emptyControl)
            {
                CreateAnimationExplorer();

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
                    CheckChanges();

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

        internal bool IsWebHMIProject()
        {
            IDocument selectedDocument = workspace.ContextDocument as IDocument;
            bool isWebHMIProject = selectedDocument != null && selectedDocument.ProjectType == ProjectType.WebHMI.ToString();
            return isWebHMIProject;
        }

        bool bContextObjectChanged;
        void workspace_ContextContentChanged(object sender, EventArgs e)
        {
            if (animationExplorerUI != null)
                PromoteCodeToIdle(false);
            bContextObjectChanged = true;
        }

        void workspace_ContextContentChanging(object sender, CancelEventArgs e)
        {
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
                CreateAnimationExplorer();
                if (!bContextObjectChanged)
                    return;
                bContextObjectChanged = false;

                // CheckChanges();

                animationExplorerUI.TabControl.Visibility = Visibility.Collapsed;
                for (int i = animationExplorerUI.TabControl.Items.Count - 1; i >= 0; i--)
                 {
                    var content = (animationExplorerUI.TabControl.Items[i] as DXTabItem).Content as AnimationEditorUI;
                    content?.Dispose();
                    if (animationExplorerUI.TabControl.Items[i] is IDisposable)
                        (animationExplorerUI.TabControl.Items[i] as IDisposable).Dispose();
                 }
                animationExplorerUI.TabControl.Items.Clear();
                List<Object> list = new List<Object>();
                if (workspace.ContextObject != null)
                {
                    list.Add(workspace.ContextObject is IEntityReference && (workspace.ContextObject as IEntityReference).ContainedObject != null ? (workspace.ContextObject as IEntityReference).ContainedObject : workspace.ContextObject);
                }
                else if (workspace.ContextObjects != null)
                {
                    foreach (Object o in workspace.ContextObjects)
                    {
                        list.Add(o is IEntityReference && (o as IEntityReference).ContainedObject != null ? (o as IEntityReference).ContainedObject : o);
                    }
                }

                var listAnimatable = new List<IAnimatable>();
                var mapTitle = new Dictionary<IAnimatable, String>();
                list.ForEach(obj =>
                {
                    try
                    {
                        if (obj is IAnimatable)
                        {
                            listAnimatable.Add(obj as IAnimatable);

                            mapTitle[obj as IAnimatable] = (obj as IAnimatable).Name;
                        }
                        else
                        {
                            var listFriends = workspace.GetFriendObjects(obj, null);
                            if (listFriends != null)
                            {
                                foreach (var v in listFriends)
                                {
                                    if (v is IAnimatable)
                                    {
                                        listAnimatable.Add(v as IAnimatable);

                                        mapTitle[v as IAnimatable] = (v as IAnimatable).Name;
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

                animationExplorerUI.TabControl.BeginInit();
                bool isWebHMIProject = IsWebHMIProject();
                listAnimatable.ForEach(animatable =>
                    {
                        DXTabItem tabitem = new DXTabItem
                        {
                            Header = mapTitle[animatable],
                            Content = new FrameworkElement() { Tag = animatable },
                            IsSelected = false
                        };
                        tabitem.InitItemTemplate();
                        animationExplorerUI.TabControl.Items.Add(tabitem);
                    });
                animationExplorerUI.TabControl.EndInit();

                if (animationExplorerUI.TabControl.Items.Count > 0)
                {
                    animationExplorerUI.TabControl.SelectedIndex = 0;
                    animationExplorerUI.TabControl.Visibility = Visibility.Visible;
                    animationExplorerUI.TextNoItems.Visibility = Visibility.Collapsed;
                }
                else
                    animationExplorerUI.TextNoItems.Visibility = Visibility.Visible;
            }
        }

        void SetIsActive(bool bSet)
        {
            if (animationExplorerUI == null)
                return;

            foreach (var control in animationExplorerUI.TabControl.Items)
            {
                DXTabItem ti = control as DXTabItem;
                if (ti == null)
                    continue;
                AnimationEditorUI ae = ti.Content as AnimationEditorUI;
                if (ae != null)
                    ae.SetIsActive(bSet);
            }
        }

        void CheckChanges()
        {
            if (animationExplorerUI == null)
                return;

            foreach (var control in animationExplorerUI.TabControl.Items)
            {
                DXTabItem ti = control as DXTabItem;
                if (ti == null)
                    continue;
                AnimationEditorUI ae = ti.Content as AnimationEditorUI;
                if (ae != null)
                    ae.PropagateChanges();
            }
        }


        #region IAnimationExplorer Members

        public UserControl control
        {
            get 
            { 
                AnimationEditorUI popupControl = new AnimationEditorUI();
                return popupControl;
            }
        }

        DispatcherOperation pendingActivation;
        public void Activate()
        {
            if (emptyControl == null || pendingActivation != null)
                return;

            pendingActivation = Dispatcher.CurrentDispatcher.BeginInvokeAsynchronouslyInApplicationIdle(() =>
            {
                workspace.FlashDockedElement(emptyControl);
                pendingActivation = null;
            });
        }

        #endregion

        void IDisposable.Dispose()
        {
            if (animationExplorerUI != null)
            {
                foreach (DXTabItem item in animationExplorerUI.TabControl.Items)
                {
                    if (item.Content is IDisposable)
                        (item.Content as IDisposable).Dispose();
                }
                animationExplorerUI.TabControl.Items.Clear();
                //animationExplorerUI.TabControl.Dispose();
            }
        }
    }
}
