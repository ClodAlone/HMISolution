using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using DocumentManager.ComponentService;
using UFInterfaces;
using UFInterfaces.CoreHostComponents;
using UIMsgBoxAlertService.ComponentService;
using UriResolver.ComponentService;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using UFInterfaces.AuthenticationCredentialsProvider;
using System.Windows.Threading;
using Tracing.ComponentService;
using Utilities;

namespace Plugin.ComponentService
{
    public class PluginComponent : ComponentBase<IPlugin>, IPlugin
    {
        #region Declaration

        Object lockObject = new Object();
        PluginUI pluginUI;
        DispatcherOperation IdleExecutionPending;
        IWorkspace workspace;
        ISimpleLogging simpleLogging;
        IUIMsgBoxAlertService uiInterface;
        bool bLoaded;
        bool bEnableIdleCode;
        bool bPendingIdleCode;

        #endregion Declaration

        #region IUFInterfaceBase Members

        void IUFInterfaceBase.Initialize()
        {
            Dispatcher.CurrentDispatcher.InvokeIfRequired(() =>
            {
                GetComponentInterfaces();
                CreatePluginUI();
            });
        }
        #endregion

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            pluginUI = null;

            if (workspace != null)
            {
                workspace.AutoHideAnimationStop -= workspace_AutoHideAnimationStop;
                workspace.AutoHideAnimationStart -= workspace_AutoHideAnimationStart;
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

        private static BitmapImage GetControlImage()
        {
            BitmapImage bm = new BitmapImage();
            bm.BeginInit();
            Assembly assembly = Assembly.GetExecutingAssembly();

            String str = String.Format("pack://application:,,,/{0};component/Images/Plugin.png",
                Path.GetFileNameWithoutExtension(assembly.Location));
            bm.UriSource = new Uri(str);
            bm.EndInit();
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

        public void CreatePluginUI()
        {
            lock (lockObject)
            {
                if (pluginUI != null)
                    return;

                workspace.AutoHideAnimationStop += workspace_AutoHideAnimationStop;
                workspace.AutoHideAnimationStart += workspace_AutoHideAnimationStart;
                workspace.ContextContentChanging += workspace_ContextContentChanging;
                workspace.ContextContentChanged += workspace_ContextContentChanged;
                workspace.DockStateChanged += workspace_DockStateChanged;
                workspace.ActiveWindowChanged += workspace_ActiveWindowChanged;
                workspace.ContentRendered += workspace_ContentRendered;

                if (workspace.HasContentRendered())
                    bEnableIdleCode = true;

                pluginUI = new PluginUI();

                pluginUI.Loaded += (o, e) =>
                {
                    if (bEnableIdleCode)
                    {
                        if (!bLoaded && pluginUI != null && pluginUI.IsVisible)
                        {
                            bLoaded = true;
                            if (workspace.GetElementDockState(pluginUI) == UFInterfaces.DockState.Document)
                                PromoteCodeToIdle(true);
                        }
                    }
                };
                pluginUI.Unloaded += (o, e) =>
                {
                    if (bEnableIdleCode)
                        bLoaded = false;
                };

                workspace.SetDesiredHeightAndWidthInDockedMode(pluginUI, pluginUI.Height, pluginUI.Width);
                pluginUI.ClearValue(FrameworkElement.WidthProperty);
                pluginUI.ClearValue(FrameworkElement.HeightProperty);

                BitmapImage bm = GetControlImage();

                workspace.AddDockingChildren(pluginUI, Properties.Resources.Plugin_Title, UFInterfaces.DockState.Dock, UFInterfaces.DockSide.Right, false, itemID: nameof(PluginUI));
                workspace.SetDockedElementIcon(pluginUI, new ImageBrush(bm));
                lastDockSide = workspace.GetElementDockSide(pluginUI);
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
            if (e.NewValue == pluginUI)
            {
                if (lastDockSide == UFInterfaces.DockSide.Tabbed && workspace.GetElementIsSelectedTab(pluginUI))
                {
                    if (pluginUI != null)
                        pluginUI.Visibility = Visibility.Visible;
                    PromoteCodeToIdle(true);
                }

                workspace.BeginEdit();
            }
            if (e.OldValue == pluginUI)
            {
                CheckChanges();
                workspace.EndEdit();

                lastDockSide = workspace.GetElementDockSide(pluginUI);
                if (lastDockSide == UFInterfaces.DockSide.Tabbed && !workspace.GetElementIsSelectedTab(pluginUI))
                {
                    if (pluginUI != null)
                        pluginUI.Visibility = Visibility.Collapsed;
                }
            }

            if (pluginUI != null && (e.OldValue == pluginUI || e.NewValue == pluginUI))
            {
                var dockstate = workspace.GetElementDockState(pluginUI);
                if (dockstate == UFInterfaces.DockState.Document)
                {
                    if (e.NewValue == pluginUI && bLoaded)
                        pluginUI.Visibility = Visibility.Visible;
                    else if (e.OldValue == pluginUI)
                        pluginUI.Visibility = Visibility.Collapsed;
                }
            }
        }

        void workspace_DockStateChanged(FrameworkElement sender, UFInterfaces.DockStateEventArgs e)
        {
            if (sender == pluginUI && e.OldState == UFInterfaces.DockState.AutoHidden)
            {
                if (pluginUI != null)
                    pluginUI.Visibility = Visibility.Visible;
                PromoteCodeToIdle(true);
            }
            else if (sender == pluginUI && (e.NewState == UFInterfaces.DockState.Hidden || e.NewState == UFInterfaces.DockState.AutoHidden))
            {
                if (pluginUI != null)
                {
                    pluginUI.Visibility = Visibility.Collapsed;
                    bAutoHideVisible = false;
                }
            }
        }

        bool bAutoHideVisible = false;
        bool bWasAutoHideVisible = false;
        void workspace_AutoHideAnimationStart(object sender, RoutedEventArgs e)
        {
            if (e.OriginalSource == pluginUI)
            {
                bAutoHideVisible = true;
                if (idleOperation != null)
                {
                    idleOperation.Abort();
                    idleOperation = null;
                }
                // if (bAutoHideVisible)
                {
                    if (pluginUI != null)
                        pluginUI.Visibility = Visibility.Visible;
                }
            }
        }

        DispatcherOperation idleOperation;
        void workspace_AutoHideAnimationStop(object sender, RoutedEventArgs e)
        {
            if (e.OriginalSource == pluginUI)
            {
                if (pluginUI != null/* && workspace.ActiveWindow != pluginUI*/)
                {
                    CheckChanges();

                    //var dockstate = workspace.GetElementDockState(pluginUI);
                    //if (dockstate == DockState.AutoHidden)
                    {
                        if (bWasAutoHideVisible)
                        {
                            bWasAutoHideVisible = false;
                            if (idleOperation == null)
                            {
                                idleOperation = pluginUI.Dispatcher.BeginInvokeAsynchronouslyInApplicationIdle(() =>
                                {
                                    if (!Utilities.WPF.DockingHelper.GetLayoutItemVisible(pluginUI))
                                        pluginUI.Visibility = Visibility.Collapsed;
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
                       }
                    }
                }
                else
                    PromoteCodeToIdle(true);
            }
        }

        bool bContextObjectChanged;
        void workspace_ContextContentChanged(object sender, EventArgs e)
        {
            if (pluginUI != null)
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

            var dockstate = workspace.GetElementDockState(pluginUI);
            bool bVisible = pluginUI.Visibility == Visibility.Visible &&
                (dockstate == UFInterfaces.DockState.Document && bLoaded || dockstate != UFInterfaces.DockState.Document && dockstate != UFInterfaces.DockState.AutoHidden ||
                 bAutoHideVisible && dockstate == UFInterfaces.DockState.AutoHidden);

            if (IdleExecutionPending == null && (bSynchro || bVisible))
            {
                Action action = () => IdleExecution();
                Dispatcher disp = pluginUI != null ? pluginUI.Dispatcher : Dispatcher.CurrentDispatcher;
                IdleExecutionPending = disp.BeginInvoke(action, bSynchro ? DispatcherPriority.Send : DispatcherPriority.ApplicationIdle);
                IdleExecutionPending.Completed += (sender, e) => IdleExecutionPending = null;
            }
        }

        private void IdleExecution()
        {
            using (new WaitCursor())
            {
                CreatePluginUI();
                if (!bContextObjectChanged)
                    return;
                bContextObjectChanged = false;

                CheckChanges();

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

                //TODO : code here what to do with current context object selection
            }
        }

        void CheckChanges()
        {
        }
    }
}