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
using System.Windows.Controls;
using System.Windows.Markup;
using System.Xml;
using Tracing.ComponentService;
using UriResolver.ComponentService;
using SymbolGallery.ComponentService;
using SymbolGallery;
using Utilities;
using DocumentManager.ComponentService;
using UIMsgBoxAlertService.ComponentService;
using HelpProvider.ComponentService;

namespace UFProjectManager.ComponentService
{
    public class SymbolGalleryComponent : ComponentBase<ISymbolGallery>, ISymbolGallery, IDisposable
    {
        #region Declaration
        Object lockObject = new Object();
        IWorkspace workspace;
        public static SymbolGalleryComponent symbolGalleryComponent { get; protected set; }
        ISimpleLogging simpleLogging;
        IUriRisolver uriResolver;
        SymbolGalleryUI symbolGalleryUI;
        bool bLoaded;
        bool bEnableIdleCode;
        bool bPendingIdleCode;
        bool bLibraryLoaded;
        #endregion
        #region IUFInterfaceBase Members

        void IUFInterfaceBase.Initialize()
        {
            Dispatcher.CurrentDispatcher.InvokeIfRequired(() =>
                {
                    if (symbolGalleryComponent == null)
                        symbolGalleryComponent = this;

                    GetComponentInterfaces();
                    CreateSymbolGallery();
                });
        }
        #endregion
        #region Properties
        IUIMsgBoxAlertService uIMsgBoxAlertService;
        public IUIMsgBoxAlertService UIMsgBoxAlertService
        {
            get
            {
                if (uIMsgBoxAlertService == null)
                    uIMsgBoxAlertService = GetService(typeof(IUIMsgBoxAlertService)) as IUIMsgBoxAlertService;
                return uIMsgBoxAlertService;
            }
        }

        IHelpProvider helpProvider;
        public IHelpProvider HelpProvider
        {
            get
            {
                if (helpProvider == null)
                    helpProvider = GetService(typeof(IHelpProvider)) as IHelpProvider;
                return helpProvider;
            }
        }
        #endregion
        private static BitmapImage GetControlImage()
        {
            var bm = SharedResources.Helpers.ResourceManager.GetCommonImage("SymbolGallery", "SGEditor");
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

            if (uriResolver == null)
                uriResolver = GetService(typeof(IUriRisolver)) as IUriRisolver;
        }

        ContentControl emptyControl;
        public void CreateSymbolGallery()
        {
            lock (lockObject)
            {
                if (symbolGalleryUI != null)
                    return;

                if (emptyControl == null)
                {
                    emptyControl = new ContentControl();

                    workspace.AutoHideAnimationStart += workspace_AutoHideAnimationStart;
                    workspace.AutoHideAnimationStop += workspace_AutoHideAnimationStop;
                    workspace.DockStateChanged += workspace_DockStateChanged;
                    workspace.ContextContentChanged += workspace_ContextContentChanged;
                    workspace.ContextDocumentChanged += workspace_ContextDocumentChanged;
                    workspace.ContentRendered += workspace_ContentRendered;
                    workspace.ActiveWindowChanged += workspace_ActiveWindowChanged;

                    if (workspace.HasContentRendered())
                        bEnableIdleCode = true;

                    BitmapImage bm = GetControlImage();

                    workspace.AddDockingChildren(emptyControl, SymbolGallery.Properties.Resources.Title, UFInterfaces.DockState.AutoHidden, UFInterfaces.DockSide.Right, false, itemID: nameof(SymbolGalleryComponent));
                    workspace.SetDockedElementIcon(emptyControl, new ImageBrush(bm));
                    lastDockSide = workspace.GetElementDockSide(emptyControl);
                }
                else
                {
                    using (var cursor = new WaitCursor())
                    {
                        symbolGalleryUI = new SymbolGalleryUI(workspace);

                        workspace.SetDesiredHeightAndWidthInDockedMode(emptyControl, symbolGalleryUI.Height, symbolGalleryUI.Width);
                        symbolGalleryUI.ClearValue(FrameworkElement.WidthProperty);
                        symbolGalleryUI.ClearValue(FrameworkElement.HeightProperty);

                        symbolGalleryUI.AddCurrentProject(workspace.ContextDocument);

                        symbolGalleryUI.Loaded += (o, e) =>
                        {
                            if (bEnableIdleCode)
                            {
                                if (!bLoaded && symbolGalleryUI != null && emptyControl.IsVisible)
                                {
                                    bLoaded = true;
                                    var state = workspace.GetElementDockState(emptyControl);
                                    if (state == UFInterfaces.DockState.Document || state == UFInterfaces.DockState.Dock)
                                        LoadLibraries();
                                }
                            }
                            else
                                bPendingIdleCode = true;
                        };
                        symbolGalleryUI.Unloaded += (o, e) =>
                        {
                            if (bEnableIdleCode)
                                bLoaded = false;
                        };

                        emptyControl.Content = symbolGalleryUI;
                    }
                }
            }
        }

        IDocument lastDocumentActive;
        void workspace_ContextDocumentChanged(object sender, EventArgs e)
        {
            if (symbolGalleryUI == null)
                return;

            var dockstate = workspace.GetElementDockState(emptyControl);
            bool bVisible = emptyControl.Visibility == Visibility.Visible &&
                (dockstate == UFInterfaces.DockState.Document || dockstate != UFInterfaces.DockState.Document && dockstate != UFInterfaces.DockState.AutoHidden ||
                 bAutoHideVisible && dockstate == UFInterfaces.DockState.AutoHidden);

            lastDocumentActive = workspace.ContextDocument;
            if (bVisible && workspace.ContextDocument != null)
            {
                symbolGalleryUI.AddCurrentProject(lastDocumentActive);
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
                    CreateSymbolGallery();
                    LoadLibraries();
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
                CreateSymbolGallery();
                LoadLibraries();
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
                CreateSymbolGallery();

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
                            CreateSymbolGallery();
                            LoadLibraries();
                        }
                    }
                }
                else
                {
                    CreateSymbolGallery();
                    LoadLibraries();
                }
            }
        }

        void workspace_ContentRendered(object sender, EventArgs e)
        {
            bEnableIdleCode = true;
            if (bPendingIdleCode)
            {
                bPendingIdleCode = false;
                LoadLibraries(true);
            }
        }

        private void LoadLibraries(bool bForce = false)
        {
            if (!bForce && !bEnableIdleCode)
            {
                bPendingIdleCode = true;
                return;
            }

            var dockstate = workspace.GetElementDockState(emptyControl);
            bool bVisible = emptyControl.Visibility == Visibility.Visible &&
                (dockstate == UFInterfaces.DockState.Document && bLoaded || dockstate != UFInterfaces.DockState.Document && dockstate != UFInterfaces.DockState.AutoHidden ||
                 bAutoHideVisible && dockstate == UFInterfaces.DockState.AutoHidden);
            if ((bForce || bVisible))
            {
                if (!bLibraryLoaded)
                {
                    if (symbolGalleryUI != null)
                        symbolGalleryUI.LoadSymbolGalleries();

                    bLibraryLoaded = true;
                }

                if(workspace.ContextDocument != null)
                {
                    symbolGalleryUI.AddCurrentProject(workspace.ContextDocument);
                }
            }
        }

        void workspace_ContextContentChanged(object sender, EventArgs e)
        {
            if (symbolGalleryUI == null)
                return;

            var dockstate = workspace.GetElementDockState(emptyControl);
            bool bVisible = emptyControl.IsVisible &&
                (dockstate == UFInterfaces.DockState.Document  || dockstate != UFInterfaces.DockState.Document && dockstate != UFInterfaces.DockState.AutoHidden ||
                 bAutoHideVisible && dockstate == UFInterfaces.DockState.AutoHidden);

            if (bVisible && workspace.ContextObject != null)
            {
                var SelectObject = workspace.ContextObject;
                var entityref = workspace.GetEntityReferenceContextObject(SelectObject);
                if (entityref != null)
                    symbolGalleryUI.SelectFromTypeDefinition(entityref.TypeDefinitionString);
                else if (SelectObject is IEntityReference &&
                    (SelectObject as IEntityReference).TypeDefinitionString != null)
                {
                    symbolGalleryUI.SelectFromTypeDefinition((SelectObject as IEntityReference).TypeDefinitionString);
                }
            }
        }

        #region IDisposable Members

        void IDisposable.Dispose()
        {
            if (symbolGalleryUI != null)
            {
                symbolGalleryUI.Dispose();
                symbolGalleryUI = null;
            }

            if (workspace != null)
            {
                workspace.AutoHideAnimationStop -= workspace_AutoHideAnimationStop;
                workspace.AutoHideAnimationStart -= workspace_AutoHideAnimationStart;
                workspace.DockStateChanged -= workspace_DockStateChanged;
                workspace.ContextContentChanged -= workspace_ContextContentChanged;
                workspace.ContextDocumentChanged -= workspace_ContextDocumentChanged;
                workspace.ContentRendered -= workspace_ContentRendered;
                workspace.ActiveWindowChanged -= workspace_ActiveWindowChanged;
            }

            lockObject = null;
        }

        #endregion

        #region ISymbolInterface
        public bool AddSymbolToLibrary(String folder, String name, String xaml, String settings)
        {
            CreateSymbolGallery();

            LoadLibraries(true);

            workspace.FlashDockedElement(emptyControl);
            return symbolGalleryUI.AddSymbolToLibrary(folder, name, xaml, settings);
        }

        public bool UpdateSymbolToLibrary(String xaml, String settings, String provider, String path, String relativePath)
        {
            CreateSymbolGallery();

            workspace.FlashDockedElement(emptyControl);
            return symbolGalleryUI.UpdateSymbolToLibrary(xaml, settings, provider, path, relativePath);
        }

        public String GetCurrentDropSettings(UserControl control = null)
        {
            CreateSymbolGallery();

            if (control == null || !(control is SymbolGalleryUI))
                return symbolGalleryUI.GetCurrentDropSettings();     
            else
                return (control as SymbolGalleryUI).GetCurrentDropSettings();
        }

        const String settingsExt = ".settings";
        public String GetCurrentDropSettings(String hash)
        {
            try
            {
                String fileSettings = hash + settingsExt;
                if (File.Exists(fileSettings))
                    return File.ReadAllText(fileSettings);
            }
            catch (Exception ex)
            {

            }

            return null;
        }
        public String GetCurrentSourceSymbolProvider(UserControl control = null)
        {
            CreateSymbolGallery();

            if (control == null || !(control is SymbolGalleryUI))
                return symbolGalleryUI.GetCurrentSourceSymbolProvider();
            else
                return (control as SymbolGalleryUI).GetCurrentSourceSymbolProvider();
        }

        public String GetCurrentSourceSymbolPath(UserControl control = null, string currentSourceSymbolPath = null)
        {
            CreateSymbolGallery();
            string path = null;
            if (control == null || !(control is SymbolGalleryUI))
                return symbolGalleryUI.GetCurrentSourceSymbolPath(currentSourceSymbolPath);
            else
                return (control as SymbolGalleryUI).GetCurrentSourceSymbolPath(currentSourceSymbolPath);
        }

        public String GetCurrentSourceSymbolCode(UserControl control = null)
        {
            CreateSymbolGallery();

            if (control == null || !(control is SymbolGalleryUI))
                return symbolGalleryUI.GetCurrentSourceSymbolCode();
            else
                return (control as SymbolGalleryUI).GetCurrentSourceSymbolCode();
        }

        public String GetSymbolElement(String sourceSymbolProvider, String sourceSymbolPath, String relativePath = null)
        {
            return CarouselListControl.GetSymbolElement(sourceSymbolProvider, sourceSymbolPath, relativePath);
        }
        public String GetSymbolSettings(String sourceSymbolProvider, String sourceSymbolPath, String relativePath = null)
        {
            return CarouselListControl.GetSymbolSettings(sourceSymbolProvider, sourceSymbolPath, relativePath);
        }
        public String GetSymbolCode(String sourceSymbolProvider, String sourceSymbolPath, String relativePath = null)
        {
            return CarouselListControl.GetSymbolCode(sourceSymbolProvider, sourceSymbolPath, relativePath);
        }

        public UserControl GetStyleLibraryControl(String type, String current)
        {
            var ret = new SymbolGalleryUI(workspace, type);
            ret.LoadSymbolGalleries();
            return ret;
        }

        #endregion
    }
}
