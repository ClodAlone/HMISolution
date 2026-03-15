using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UFInterfaces.CoreHostComponents;
using UFInterfaces;
using System.Windows.Threading;
using System.Collections;
using System.Windows;
using OPCUAClientStatus.ComponentService;
using Tracing.ComponentService;
using System.Windows.Media.Imaging;
using System.Reflection;
using System.Windows.Media;
using System.Windows.Input;
using System.Windows.Controls;
using Utilities;
using StringManager.ComponentService;

namespace OPCUAClientStatus.ComponentService
{
    public class OPCUABrowserComponent : ComponentBase<IOPCUAClientStatus>, IOPCUAClientStatus, IDisposable
    {
        #region Declaration
        OPCUAClientStatusUI OPCUAClientStatusUI;
        Object lockObject = new Object();
        IWorkspace workspace;
        ISimpleLogging simpleLogging;
        IStringEditorManager stringManager;
        bool bLoaded;
        bool bEnableIdleCode;
        #endregion

        #region IOPCUAClientStatus Members

        private void GetComponentInterfaces()
        {
            if (workspace == null)
                workspace = GetService(typeof(IWorkspace)) as IWorkspace;

            if (simpleLogging == null)
                simpleLogging = GetService(typeof(ISimpleLogging)) as ISimpleLogging;
        }

        public UserControl GetClientStatusControl()
        {
            if (stringManager == null)
                stringManager = GetService(typeof(IStringEditorManager)) as IStringEditorManager;
            var ret = new OPCUAClientStatusUI(null, simpleLogging, stringManager);
            ret.EnableUIMng(true);
            return ret;
        }
        #endregion

        private static BitmapImage GetControlImage()
        {
            var bm = SharedResources.Helpers.ResourceManager.GetCommonImage("OPCUAClientStatus", "OPCCEditor");
            return bm;
        }

        ContentControl emptyControl;
        private void CreateDockedControl()
        {
            lock (lockObject)
            {
                if (OPCUAClientStatusUI != null || workspace == null)
                    return;

                if (emptyControl == null)
                {
                    emptyControl = new ContentControl();

                    workspace.AutoHideAnimationStop += workspace_AutoHideAnimationStop;
                    workspace.AutoHideAnimationStart += workspace_AutoHideAnimationStart;
                    workspace.DockStateChanged += workspace_DockStateChanged;
                    workspace.ActiveWindowChanged += workspace_ActiveWindowChanged;

                    if (workspace.HasContentRendered())
                        bEnableIdleCode = true;

                    BitmapImage bm = GetControlImage();

                    workspace.AddDockingChildren(emptyControl, Properties.Resources.OPCUAClientStatus_Title, UFInterfaces.DockState.AutoHidden, UFInterfaces.DockSide.Left, false, itemID: nameof(OPCUABrowserComponent));
                    workspace.SetDockedElementIcon(emptyControl, new ImageBrush(bm));
                    lastDockSide = workspace.GetElementDockSide(emptyControl);
                }
                else
                {
                    using (var cursor = new WaitCursor())
                    {
                        OPCUAClientStatusUI = new OPCUAClientStatusUI(workspace, simpleLogging, stringManager);

                        workspace.SetDesiredHeightAndWidthInDockedMode(emptyControl, OPCUAClientStatusUI.Height, OPCUAClientStatusUI.Width);
                        OPCUAClientStatusUI.ClearValue(FrameworkElement.WidthProperty);
                        OPCUAClientStatusUI.ClearValue(FrameworkElement.HeightProperty);

                        emptyControl.Content = OPCUAClientStatusUI;

                        OPCUAClientStatusUI.Loaded += (o, e) =>
                        {
                            if (bEnableIdleCode)
                            {
                                if (!bLoaded && OPCUAClientStatusUI != null && OPCUAClientStatusUI.IsVisible)
                                {
                                    bLoaded = true;
                                }
                            }
                        };
                        OPCUAClientStatusUI.Unloaded += (o, e) =>
                        {
                            if (bEnableIdleCode)
                                bLoaded = false;
                        };
                    }
                }
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
                }
            }
            if (e.OldValue == emptyControl)
            {
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

        void workspace_DockStateChanged(FrameworkElement sender, UFInterfaces.DockStateEventArgs e)
        {
            if (sender == emptyControl && e.OldState == UFInterfaces.DockState.AutoHidden)
            {
                if (emptyControl != null)
                    emptyControl.Visibility = Visibility.Visible;
                CreateDockedControl();
                if (OPCUAClientStatusUI != null)
                    OPCUAClientStatusUI.EnableUIMng();
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
                CreateDockedControl();

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
                            CreateDockedControl();
                            if (OPCUAClientStatusUI != null)
                                OPCUAClientStatusUI.EnableUIMng();
                        }
                    }
                }
                else
                {
                    CreateDockedControl();
                    if (OPCUAClientStatusUI != null)
                        OPCUAClientStatusUI.EnableUIMng();
                }
            }
        }

        #region IUFInterfaceBase Members

        public void Initialize()
        {
            GetComponentInterfaces();
            CreateDockedControl();
        }

        #endregion

        #region IDisposable Members

        void IDisposable.Dispose()
        {
            if (workspace != null)
            {
                workspace.AutoHideAnimationStop -= workspace_AutoHideAnimationStop;
                workspace.AutoHideAnimationStart -= workspace_AutoHideAnimationStart;
                workspace.DockStateChanged -= workspace_DockStateChanged;
                workspace.ActiveWindowChanged -= workspace_ActiveWindowChanged;
            }

            OPCUAClientStatusUI = null;
            lockObject = null;
        }

        #endregion
    }
}
