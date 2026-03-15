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
using UFInterfaces.Scriptable;
using System.ComponentModel;
using WinWrap.Basic;
using System.Windows.Controls;
using System.Linq;
using System.Windows.Interop;
using DocumentManager.ComponentService;
using ScriptManager.ComponentService;
using Opc.Ua;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.Drawing;
using DevExpress.Xpf.Core;
using IWorkspace = UFInterfaces.IWorkspace;
using Utilities.WPF;
using System.Windows.Input;
using log4net;

namespace ScriptExplorer.ComponentService
{
    public class ScriptExplorerComponent : ComponentBase<IScriptExplorer>, IScriptExplorer, IDisposable
    {
        #region Declaration

        Object lockObject = new Object();
        IWorkspace workspace;
        ISimpleLogging simpleLogging;
        IUIMsgBoxAlertService uiInterface;
        ScriptExplorerUI scriptExplorerUI;
        DispatcherOperation IdleExecutionPending;
        bool bLoaded;
        bool bEnableIdleCode;
        bool bPendingIdleCode;

        Dictionary<BasicIdeCtl, IScriptable> mapCurrentControls = new Dictionary<BasicIdeCtl,IScriptable>();
        Dictionary<BasicIdeCtl, IList> mapCurrentReferences = new Dictionary<BasicIdeCtl, IList>();
        List<BasicIdeCtl> listControlsToAttach = new List<BasicIdeCtl>();
        List<BasicIdeCtl> listReadyControls = new List<BasicIdeCtl>();

        static readonly ILog sysLog = LogManager.GetLogger(Properties.Resources.ScriptExplorer_Title);
        #endregion

        #region IUFInterfaceBase Members

        void IUFInterfaceBase.Initialize()
        {
            GetComponentInterfaces();
        }
        #endregion

        void workspace_WorkspaceLoading(object sender, EventArgs e)
        {
            CreateScriptExplorer();
        }

        private static BitmapImage GetControlImage()
        {
            var bm = SharedResources.Helpers.ResourceManager.GetCommonImage("Script", "SCMEditorSmall");
            return bm;
        }

        private void GetComponentInterfaces()
        {
            if (workspace == null)
                workspace = GetService(typeof(IWorkspace)) as IWorkspace;
            if (workspace == null)
                throw new NotImplementedException("Expecting the missing IWorkpsace Interface");

            workspace.WorkspaceLoading += workspace_WorkspaceLoading;

            if (simpleLogging == null)
                simpleLogging = GetService(typeof(ISimpleLogging)) as ISimpleLogging;

            if (uiInterface == null)
                uiInterface = GetService(typeof(IUIMsgBoxAlertService)) as IUIMsgBoxAlertService;
        }

        ContentControl emptyControl;
        public void CreateScriptExplorer()
        {
            lock (lockObject)
            {
                if (scriptExplorerUI != null)
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

                    workspace.AddDockingChildren(emptyControl, Properties.Resources.ScriptExplorer_Title, UFInterfaces.DockState.AutoHidden, UFInterfaces.DockSide.Bottom, false, false, itemID: nameof(ScriptExplorerComponent));
                    workspace.SetDockedElementIcon(emptyControl, new ImageBrush(bm));
                    lastDockSide = workspace.GetElementDockSide(emptyControl);
                }
                else
                {
                    using (var cursor = new WaitCursor())
                    {
                        scriptExplorerUI = new ScriptExplorerUI();

                        scriptExplorerUI.Loaded += (o, e) =>
                            {
                                if (bEnableIdleCode)
                                {
                                    if (!bLoaded && scriptExplorerUI != null && scriptExplorerUI.IsVisible)
                                    {
                                        bLoaded = true;
                                        if (workspace.GetElementDockState(emptyControl) == UFInterfaces.DockState.Document)
                                            PromoteCodeToIdle(false);
                                    }
                                }
                            };
                        scriptExplorerUI.Unloaded += (o, e) =>
                            {
                                if (bEnableIdleCode)
                                    bLoaded = false;
                            };

                        scriptExplorerUI.LostFocus += (o, e) =>
                            {
                                if (bEnableIdleCode)
                                {
                                    if (scriptExplorerUI != null && scriptExplorerUI.IsVisible)
                                    {
                                        PromoteCodeToIdle(true);
                                    }
                                }
                            };

                        /*
                        scriptExplorerUI.TabControl.SelectionChanged += (o, e) =>
                            {
                                foreach(var item in e.AddedItems)
                                {
                                    var tabitem = item as DXTabItem;
                                    if (tabitem == null)
                                        continue;
                                    var grid = tabitem.Content as Grid;
                                    if (grid == null)
                                        continue;

                                    scriptExplorerUI.Dispatcher.BeginInvokeAsynchronouslyInInput(() =>
                                    {
                                        basicIdeCtl_OpenSheet(grid.Children[0], null);
                                    });
                                }
                            };
                        */

                        scriptExplorerUI.TabControl.SelectionChanged += (o, e) =>
                        {
                            if (e.NewSelectedItem is DXTabItem)
                            {
                                var tabitem = e.NewSelectedItem as DXTabItem;
                                if (!(tabitem.Content is Grid))
                                {
                                    var fe = tabitem.Content as FrameworkElement;
                                    if (fe != null && fe.Tag is IScriptable)
                                    {
                                        var scriptable = fe.Tag as IScriptable;
                                        var basicIdeCtl = CreateScriptControl(scriptable, tabitem.Header as String);
                                        var grid = new Grid();
                                        grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(0, GridUnitType.Auto) });
                                        grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(50, GridUnitType.Star) });
                                        var progress = new ProgressBar()
                                        {
                                            IsIndeterminate = true,
                                            ToolTip = Properties.Resources.LoadingReferences,
                                            HorizontalAlignment = HorizontalAlignment.Stretch,
                                            VerticalAlignment = VerticalAlignment.Top,
                                            Height = 5
                                        };
                                        Grid.SetRow(basicIdeCtl, 1);
                                        grid.Children.Add(basicIdeCtl);
                                        grid.Children.Add(progress);
                                        basicIdeCtl.MessageHook += basicIdeCtl_MessageHook;
                                        listControlsToAttach.Add(basicIdeCtl);

                                        //basicIdeCtl.Loaded += (o, e) =>
                                        //{
                                        //    basicIdeCtl_OpenSheet(basicIdeCtl, null);
                                        //};
                                        tabitem.Content = grid;
                                    }
                                }
                            }
                        };

                        workspace.SetDesiredHeightAndWidthInDockedMode(emptyControl, scriptExplorerUI.Height, scriptExplorerUI.Width);
                        scriptExplorerUI.ClearValue(FrameworkElement.WidthProperty);
                        scriptExplorerUI.ClearValue(FrameworkElement.HeightProperty);

                        emptyControl.Content = scriptExplorerUI;
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
                workspace.ContextDocumentChanging -= workspace_ContextDocumentChanging;

                if (emptyControl != null)
                    Window.GetWindow(emptyControl).PreviewMouseDown -= ScriptExplorerUI_PreviewMouseDown;

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
                if (emptyControl != null)
                    Window.GetWindow(emptyControl).PreviewMouseDown += ScriptExplorerUI_PreviewMouseDown;
                workspace.ActiveWindowChanging += workspace_ActiveWindowChanging;
                workspace.ContextDocumentChanging += workspace_ContextDocumentChanging;
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

        private void ScriptExplorerUI_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (emptyControl == null)
                return;
            
            var layoutItem = DevExpress.Xpf.Docking.DockLayoutManager.GetLayoutItem(emptyControl);
            if (layoutItem == null || LayoutHelper.IsChildElement(layoutItem, e.OriginalSource as DependencyObject))
                return;

            layoutItem.IsActive = false;
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
                CreateScriptExplorer();

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

        internal bool IsWebHMIProject()
        {
            IDocument selectedDocument = workspace.ContextDocument as IDocument;
            bool isWebHMIProject = selectedDocument != null && selectedDocument.ProjectType == ProjectType.WebHMI.ToString();
            return isWebHMIProject;
        }

        bool bContextObjectChanged;
        void workspace_ContextContentChanged(object sender, EventArgs e)
        {
            if (scriptExplorerUI != null)
                PromoteCodeToIdle(false);
            bContextObjectChanged = true;
        }

        void workspace_ContextContentChanging(object sender, CancelEventArgs e)
        {
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

        List<IScriptable> currentlistScriptable = new List<IScriptable>();

        private void IdleExecution()
        {
            CreateScriptExplorer();
            if (!bContextObjectChanged)
                return;
            bContextObjectChanged = false;

            using (new WaitCursor())
            {
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

                var listScriptable = new List<IScriptable>();
                var mapTitle = new Dictionary<IScriptable, String>();
                bool isWebHMIProject = IsWebHMIProject();
                if (!isWebHMIProject)
                {
                    list.ForEach(obj =>
                    {
                        try
                        {
                            if (obj is IScriptable && (obj as IScriptable).CanEdit)
                            {
                                listScriptable.Add(obj as IScriptable);

                                mapTitle[obj as IScriptable] = (obj as IScriptable).Name;
                            }
                            else
                            {
                                var listFriends = new ArrayList(workspace.GetFriendObjects(obj, null));
                                if (listFriends != null)
                                {
                                    foreach (var v in listFriends)
                                    {
                                        if (v is IScriptable && (v as IScriptable).CanEdit)
                                        {
                                            listScriptable.Add(v as IScriptable);

                                            mapTitle[v as IScriptable] = (v as IScriptable).Name;
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

                    if (listScriptable.Count == currentlistScriptable.Count)
                    {
                        bool bChanged = false;
                        for (int i = 0; i < listScriptable.Count; ++i)
                        {
                            if (listScriptable[i] != currentlistScriptable[i])
                            {
                                bChanged = true;
                                break;
                            }
                        }
                        if (!bChanged)
                            return;
                    }
                }

                currentlistScriptable.Clear();
                currentlistScriptable.AddRange(listScriptable);

                // CheckChanges(mapCurrentControls);
                scriptExplorerUI.TabControl.Visibility = Visibility.Collapsed;
                scriptExplorerUI.TabControl.Items.Clear();
                DestroyAttachedControls();

                scriptExplorerUI.TabControl.BeginInit();
                listScriptable.ForEach(scriptable => 
                    {
                        var tabitem = new DXTabItem 
                        { 
                            Header = mapTitle[scriptable], 
                            Content = new FrameworkElement() { Tag = scriptable }, 
                            IsSelected = false 
                        };
                        tabitem.InitItemTemplate();
                        scriptExplorerUI.TabControl.Items.Add(tabitem);
                    });
                scriptExplorerUI.TabControl.EndInit();

                if (scriptExplorerUI.TabControl.Items.Count > 0)
                {
                    scriptExplorerUI.TabControl.SelectedIndex = 0;
                    scriptExplorerUI.TabControl.Visibility = Visibility.Visible;
                    scriptExplorerUI.TextNoItems.Visibility = Visibility.Collapsed;
                    scriptExplorerUI.TextNotSupported.Visibility = Visibility.Collapsed;
                }
                else
                {
                    if(isWebHMIProject)
                    {
                        scriptExplorerUI.TextNoItems.Visibility = Visibility.Collapsed;
                        scriptExplorerUI.TextNotSupported.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        scriptExplorerUI.TextNotSupported.Visibility = Visibility.Collapsed;
                        scriptExplorerUI.TextNoItems.Visibility = Visibility.Visible;
                    }

                }
            }
        }

        bool bSettingCode;
        BasicIdeCtl CreateScriptControl(IScriptable scriptable, String title)
        {
            var basicIdeCtl = new BasicIdeCtl();
#if DEBUG
            if (System.IO.File.Exists(@"C:\Program Files (x86)\Polar Engineering\WinWrap Basic\Certificates\Application-a67e0d79.htm"))
#endif
            basicIdeCtl.Secret = new Guid(Properties.Settings.Default.SecretKey);

            basicIdeCtl.FileMenuVisible = false;
            basicIdeCtl.FileTools = false;
            basicIdeCtl.FullPopupMenu = true;
            basicIdeCtl.FileDesc = title;
            basicIdeCtl.Caption = title;
            basicIdeCtl.LargeIcon = null;
            basicIdeCtl.SmallIcon = null;
            basicIdeCtl.TaskbarIconMode = WinWrap.Basic.TaskbarIconModeConstants.IconNoneSysmenuNone;
            // basicIdeCtl.AttachToWindow(Window.GetWindow(scriptExplorerUI), ManageConstants.All);
            LoadFont(basicIdeCtl);
            basicIdeCtl.FontChanged += PropagateFont;
            basicIdeCtl.Loaded += (o, e) =>
                {
                    basicIdeCtl.AttachToWindow(null, ManageConstants.OnCaptionChange);
                };

            basicIdeCtl.SortSheets = false;
            basicIdeCtl.ActiveSheetChange += (i, j) =>
            {
                basicIdeCtl.Locked = basicIdeCtl.ActiveSheet > 1;
            };

            basicIdeCtl.CloseSheet_ += (o, e) =>
            {
                if (mapCurrentControls.ContainsKey(basicIdeCtl) && 
                    !bSettingCode && e.Sheet == 1)
                    e.Cancel = 1;
            };

            basicIdeCtl.OverrideModalWindowOwner += (o, e) =>
            {
                e.OwnerHandle = new WindowInteropHelper(Window.GetWindow(scriptExplorerUI)).Handle;
            };

            mapCurrentControls.Add(basicIdeCtl, scriptable);

            basicIdeCtl.OpenSheet += basicIdeCtl_OpenSheet;
            return basicIdeCtl;
        }

        public void LoadFont(BasicIdeCtl ctl, Font newFont = null)
        {
            Font fontToLoad = newFont;
            if (fontToLoad == null)
                fontToLoad = FontPropertiesHelper.GetProperty("ScriptEditor");
            if (fontToLoad != null)
                ctl.Font = fontToLoad;
        }

        void PropagateFont(object sender, EventArgs e)
        {
            if (sender is BasicIdeCtl)
            {
                Font newFont = (sender as BasicIdeCtl).Font;
                FontPropertiesHelper.SetProperty(newFont, "ScriptEditor");
                mapCurrentControls.Keys.Where(ide => !ide.Font.Equals(newFont)).ToList().ForEach(x => LoadFont(x, newFont));
            }
        }

        IntPtr basicIdeCtl_MessageHook(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            //Debug.WriteLine(String.Format("Message {0} received", msg));

            if (msg == 0x0007 /* WM_SETFOCUS */ ||
                msg == 0x0021 /* WM_MOUSEACTIVATE */)
            {
                if (workspace != null && workspace.ActiveWindow != emptyControl)
                {
                    workspace.ActiveWindow = emptyControl;
                    scriptExplorerUI.Focusable = true;
                    scriptExplorerUI.Focus();
                }
            }

            return IntPtr.Zero;
        }
        static readonly String baseCode = "'#Language \"WWB.NET\"";

        void AddPendingReferences()
        {
            if (mapCurrentReferences.Count == 0)
                return;
            var first = mapCurrentReferences.First();
            var referencelist = first.Value;
            if (referencelist == null || referencelist.Count == 0)
            {
                mapCurrentReferences.Remove(first.Key);
                ClearProgressBar(first.Key);
            }
            else
            {
                var reference = referencelist[0];
                referencelist.RemoveAt(0);
                var referenceGetTypeAssembly = reference.GetType().Assembly;
                var referenceName = mapCurrentControls[first.Key].GetReferenceName(reference);
                if (referenceName.StartsWith("%"))
                    first.Key.AddExtension(referenceName, reference);
                else
                {
                    first.Key.AddExtension("#", referenceGetTypeAssembly);
                    first.Key.AddExtensionObjectWithEvents(referenceName, reference);
                }
            }

            Dispatcher.CurrentDispatcher.BeginInvokeAsynchronouslyInBackground(AddPendingReferences);
        }

        void ClearProgressBar(BasicIdeCtl control)
        {
            if (scriptExplorerUI == null)
                return;

            foreach (DXTabItem item in scriptExplorerUI.TabControl.Items)
            {
                if (item.Content is Panel)
                {
                    var panel = item.Content as Panel;
                    if (panel.Children.Contains(control))
                    {
                        if (panel.Children[panel.Children.Count - 1] is ProgressBar)
                            panel.Children.RemoveAt(panel.Children.Count - 1);
                        break;
                    }
                }
            }
        }

        void basicIdeCtl_OpenSheet(object sender, WinWrap.Basic.Classic.OpenSheetEventArgs ev)
        {
            if (!(sender is BasicIdeCtl))
                return;
            BasicIdeCtl basicIdeCtl = sender as BasicIdeCtl;
            if (!mapCurrentControls.ContainsKey(basicIdeCtl))
                return;
            basicIdeCtl.OpenSheet -= basicIdeCtl_OpenSheet;
            if (!listReadyControls.Contains(basicIdeCtl))
                listReadyControls.Add(basicIdeCtl);
            else
                return;

            Dispatcher.CurrentDispatcher.BeginInvoke(
            (Action)delegate
            {
                if (mapCurrentControls.ContainsKey(basicIdeCtl))
                {
                    IScriptable scriptable = mapCurrentControls[basicIdeCtl];
                    basicIdeCtl.AttachToWindow(null, ManageConstants.Disconnecting);

                    if (scriptable.CanReadMacro)
                    {
                        basicIdeCtl.ReadMacro += (o, e) =>
                        {
                            if (scriptable.CanReadMacro && e.FileName.StartsWith("*") && workspace.ContextDocument is IDocument)
                            {
                                var doc = workspace.ContextDocument as IDocument;
                                var filename = e.FileName.Replace("*", "");
                                var scriptManager = doc.GetService(typeof(IScriptManager)) as IScriptManager;
                                if (scriptManager != null)
                                {
                                    e.Code = scriptManager.GetScriptCode(doc, filename);
                                    e.Changed = true;
                                    e.Cancel = false;
                                };
                            }
                        };
                    }

                    //Assembly wpfCoreAssembly = typeof(CommandBinding).Assembly;
                    //basicIdeCtl.AddExtension("#", wpfCoreAssembly);
                    //Assembly wpfFrameworkAssembly = typeof(Window).Assembly;
                    //basicIdeCtl.AddExtension("#", wpfFrameworkAssembly);
                    var opcua = typeof(DataValue).Assembly;
                    basicIdeCtl.AddExtension("#", opcua);

                    basicIdeCtl.AddExtension("$Feature ExtensionCache False", null);
                    basicIdeCtl.AddExtension("$Feature DotNetAutoCleanup False", null);
                    if (listControlsToAttach.Contains(basicIdeCtl))
                        basicIdeCtl.AddExtension("$Feature HighlightSymbol False", null);

                    var quickList = scriptable.GetQuickReferenceList();
                    if (quickList != null)
                    {
                        foreach(var reference in quickList)
                        {
                            var referenceGetTypeAssembly = reference.GetType().Assembly;
                            var referenceName = scriptable.GetReferenceName(reference);
                            if (referenceName.StartsWith("%"))
                                basicIdeCtl.AddExtension(referenceName, reference);
                            else
                            {
                                basicIdeCtl.AddExtension("#", referenceGetTypeAssembly);
                                basicIdeCtl.AddExtensionObjectWithEvents(referenceName, reference);
                            }
                        }
                    }

                    var task1 = Task.Factory.StartNew(delegate
                    {
                        var ret = scriptable.GetReferenceList();
                        if (quickList != null)
                        {
                            foreach (var reference in quickList)
                                ret.Remove(reference);
                        }
                        return ret;
                    });
                    var task2 = task1.ContinueWith(ret =>
                    {
                        if (mapCurrentControls.ContainsKey(basicIdeCtl))
                        {
                            if (mapCurrentReferences.ContainsKey(basicIdeCtl))
                                mapCurrentReferences.Remove(basicIdeCtl);
                            if (ret.Result != null)
                            {
                                mapCurrentReferences.Add(basicIdeCtl, ret.Result);
                                Dispatcher.CurrentDispatcher.BeginInvokeAsynchronouslyInBackground(AddPendingReferences);
                            }
                            else
                                ClearProgressBar(basicIdeCtl);
                        }
                    }, TaskScheduler.FromCurrentSynchronizationContext());

                    //foreach (var reference in scriptable.GetReferenceList())
                    //{
                    //    var referenceGetTypeAssembly = reference.GetType().Assembly;
                    //    var referenceName = scriptable.GetReferenceName(reference);
                    //    if (referenceName.StartsWith("%"))
                    //        basicIdeCtl.AddExtension(referenceName, reference);
                    //    else
                    //    {
                    //        basicIdeCtl.AddExtension("#", referenceGetTypeAssembly);
                    //        basicIdeCtl.AddExtensionObjectWithEvents(referenceName, reference);
                    //    }
                    //}
                    bSettingCode = true;
                    try
                    {
                        basicIdeCtl.EventMode = true;
                        if (String.IsNullOrEmpty(scriptable.Code))
                            basicIdeCtl.Code = baseCode;
                        else
                        {
                            basicIdeCtl.Code = scriptable.Code;
                            basicIdeCtl.BreakPoints = scriptable.Breakpoints;
                        }
                        basicIdeCtl.Changed = false;
                    }
                    finally
                    {
                        bSettingCode = false;
                    }
                    //basicIdeCtl.AddExtension("$Feature WWB.COM False", null); Tom cat vegna un azzideint
                    //basicIdeCtl.AddExtension("$Feature WWB.NET True", null);
                }
            }, DispatcherPriority.Normal);
        }

        void workspace_ActiveWindowChanging(object sender, CancelEventArgs e)
        {
            e.Cancel = !CheckChanges(mapCurrentControls);
            if (e.Cancel)
            {
                workspace.FlashDockedElement(emptyControl);
                var dockState = workspace.GetElementDockState(emptyControl);
                if (dockState == UFInterfaces.DockState.AutoHidden)
                    workspace.SetElementDockState(emptyControl, UFInterfaces.DockState.Dock);
            }
            else
            {
                //CheckChanges(mapCurrentControls);
                workspace.EndEdit();
            }
        }

        void workspace_ContextDocumentChanging(object sender, CancelEventArgs e)
        {
            e.Cancel = !CheckChanges(mapCurrentControls);
            if (e.Cancel)
            {
                workspace.FlashDockedElement(emptyControl);
                var dockState = workspace.GetElementDockState(emptyControl);
                if (dockState == UFInterfaces.DockState.AutoHidden)
                    workspace.SetElementDockState(emptyControl, UFInterfaces.DockState.Dock);
            }
        }

        bool CheckChanges(Dictionary<BasicIdeCtl, IScriptable> map)
        {
            bool bRet = true;
            foreach (var control in map.Keys)
            {
                if (!listReadyControls.Contains(control))
                    continue;

                if (control.SheetCount > 1)
                    control.ActiveSheet = 1;
                if (!control.Changed)
                {
                    if (map[control].Breakpoints == null)
                    {
                        if (control.BreakPoints.Length == 0)
                            continue;
                    }
                    if (map[control].Breakpoints != null)
                    {
                        var arraysAreEqual = Enumerable.SequenceEqual(map[control].Breakpoints, control.BreakPoints);
                        if (arraysAreEqual)
                            continue;
                    }
                }

                try
                {
                    map[control].Code = control.Code;
                    map[control].Breakpoints = control.BreakPoints;

                    using (var meinfo = control.Query("GetSymbolInfo Me Public"))
                    {
                        var prototypes = new List<String>();
                        for (int i = 1; ; ++i)
                        {
                            var prototype = (String)meinfo["Member" + i];
                            if (prototype == null/* || prototype.Contains('(') || prototype.Contains(')')*/)
                                break;

                            prototype = prototype.Replace('(', ' ').Replace(')', ' ').Trim();
                            prototypes.Add(prototype);
                        }

                        if (prototypes.Count == 0)
                            prototypes = null;
                        map[control].SetListProcedures(prototypes);
                    }

                    control.Changed = false;
                }
                catch (Exception ex)
                {
                    if (uiInterface != null)
                    {
                        emptyControl.Dispatcher.BeginInvokeAsynchronouslyInInput(() =>
                        {
                            uiInterface.ShowError(String.Format(Properties.Resources.ErrorOccuredSettingCode, ex.Message));
                        });
                        bRet = false;
                    }
                }
            }

            return bRet;
        }

        void DestroyAttachedControls()
        {
            var controls = (from c in mapCurrentControls.Keys
                            where listControlsToAttach.Contains(c)
                            select c).ToList();

            foreach (var control in controls)
            {
                if (mapCurrentControls.ContainsKey(control))
                    mapCurrentControls.Remove(control);
                if (mapCurrentReferences.ContainsKey(control))
                    mapCurrentReferences.Remove(control);
            }

            DestroyScriptControls(controls);
        }

        void DestroyScriptControls(IList<BasicIdeCtl> controls)
        {
            foreach (var control in controls)
            {
                try
                {
                    while (control.Shutdown() < 0)
                        WaitForPriority.DoEvents();

                    control.Disconnect();
                    control.Dispose();
                }
                catch (Exception ex)
                {
                    sysLog.Error(Properties.Resources.ErrorOccurredOnDestroy, ex);
                }

                if (listControlsToAttach.Contains(control))
                    listControlsToAttach.Remove(control);
                if (listReadyControls.Contains(control))
                    listReadyControls.Remove(control);
            }
        }

        #region IDisposable Members

        void IDisposable.Dispose()
        {
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

            var controls = mapCurrentControls.Keys.ToList();
            mapCurrentControls.Clear();
            mapCurrentReferences.Clear();
            if (scriptExplorerUI != null)
            {
                scriptExplorerUI.TabControl.Items.Clear();
                scriptExplorerUI = null;
            }
            DestroyScriptControls(controls);
        }

        #endregion

        #region IScriptExplorer
        public IDictionary<IScriptable, Grid> CreateEditControls(IList<IScriptable> list)
        {
            var ret = new Dictionary<IScriptable, Grid>();
            foreach (var scriptable in list)
            {
                var editor = CreateScriptControl(scriptable, scriptable.Name);
                var grid = new Grid();
                grid.Children.Add(editor);
                ret.Add(scriptable, grid);
            }

            return ret;
        }

        public void CheckForChanges(IList<IScriptable> list)
        {
            var check = new Dictionary<BasicIdeCtl, IScriptable>();
            foreach (var scriptable in list)
            {
                var found = (from c in mapCurrentControls where c.Value == scriptable && 
                             !listControlsToAttach.Contains(c.Key)
                             select c.Key).ToList();
                if (found.Count > 0)
                    check.Add(found[0], scriptable);
            }

            CheckChanges(check);
        }

        public void DestroyEditControls(IList<IScriptable> list)
        {
            var controls = new List<BasicIdeCtl>();
            foreach (var scriptable in list)
            {
                var found = (from c in mapCurrentControls where c.Value == scriptable && 
                                                        !listControlsToAttach.Contains(c.Key)
                             select c.Key).ToList();
                if (found.Count > 0)
                    controls.Add(found[0]);
            }

            foreach (var control in controls)
            {
                if (mapCurrentControls.ContainsKey(control))
                    mapCurrentControls.Remove(control);
                if (mapCurrentReferences.ContainsKey(control))
                    mapCurrentReferences.Remove(control);
            }

            DestroyScriptControls(controls);
        }

        #endregion
    }
}
