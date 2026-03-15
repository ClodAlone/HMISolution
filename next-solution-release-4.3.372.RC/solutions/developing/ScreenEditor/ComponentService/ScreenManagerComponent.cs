using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using UFUserEditor.ComponentService;
#if !NET_STANDARD
using UFShortcutEditor.ComponentService;
using UIMsgBoxAlertService.ComponentService;
using Utilities.WPF;
using Converters;
#endif
#if !WINDOWS_UWP
#if !NET_STANDARD
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using AnimationExplorer.ComponentService;
using CommandExplorer.ComponentService;
using PropertyControl.ComponentService;
using SymbolGallery.ComponentService;
using Toolbox.ComponentService;
using Tracing.ComponentService;
using VFS;
using OPCUAClientStatus.ComponentService;
using MSSchedulerSettings.ComponentService;
using UFMenuEditor.ComponentService;
using ScriptExplorer.ComponentService;
using UFCrossReferenceEditor.ComponentService;
using WPFUtilities.PropertyDataTemplate;
#endif
using System.Threading;
using UFUAEditor.ComponentService;
using System.Configuration;
#else
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Markup;
#endif
using DocumentManager.ComponentService;
using UFInterfaces;
using UFInterfaces.CoreHostComponents;
using Utilities;
using UFInterfaces.AuthenticationCredentialsProvider;
using ScreenSettings;
using System.Collections.ObjectModel;
using UFProjectManager.ComponentService;
using StringManager.ComponentService;
using WPFUtilities;
using System.Text;
using ScreenSettings.Entities;
using System.Windows.Input;
using System.Threading.Tasks;
using log4net;
using DocumentManager.ComponentService.Helpers;
#if !WINDOWS_UWP && !NET_STANDARD
using System.Windows.Media.Media3D;
using OPCUAViewModel.PropertyDataTemplate;
#endif

namespace ScreenManager.ComponentService
{
    public class ScreenManagerComponent : ComponentBase<IScreenManager>, IScreenManager, IDocumentManager, IDisposable
#if !WINDOWS_UWP && !NET_STANDARD
        , ICrossReference
#endif
    {
#region Declaration

        readonly Object lockObject = new Object();

        readonly Dictionary<String, ScreenDocument> mapActiveDocuments = new Dictionary<String, ScreenDocument>(StringComparer.OrdinalIgnoreCase);
        readonly Dictionary<ScreenDocument, String> mapActiveDocumentUris = new Dictionary<ScreenDocument, String>();
        readonly Dictionary<String, String> mapActiveDocumentTitles = new Dictionary<String, String>(StringComparer.OrdinalIgnoreCase);


#if !NET_STANDARD
        bool bLoaded;
        bool bToolbarInitialized;
        internal MenuControl menuControl;
        List<CommandBinding> globalCbs;

        readonly Dictionary<String, String> mapItems = new Dictionary<String, String>();
        string lastTriedDocumentFileName;

        internal StopWatcher performanceWatcher;
#endif

#if !WINDOWS_UWP
       public static ScreenManagerComponent screenManagerComponent { get; protected set; }

#if !NET_STANDARD
        static readonly ILog logGeneral = LogManager.GetLogger(Properties.Resources.GeneralLog);
        private static readonly ILog logDeploy = LogManager.GetLogger(Properties.Resources.SvgGenerator);
#else
        static readonly ILog logGeneral = LogManager.GetLogger(System.Reflection.Assembly.GetEntryAssembly(), Properties.Resources.GeneralLog);
#endif
#endif
        #endregion

        #region IUFInterfaceBase Members

        void IUFInterfaceBase.Initialize()
        {
#if !WINDOWS_UWP && !NET_STANDARD
            if (screenManagerComponent == null)
                screenManagerComponent = this;

            GetComponentInterfaces();
#endif
        }

        #endregion

#if !WINDOWS_UWP && !NET_STANDARD

        public static void EnableSoftwareRendering()
        {
            RenderOptions.ProcessRenderMode = System.Windows.Interop.RenderMode.SoftwareOnly;
        }

        public static BitmapImage GetBitmapImage(String image, bool bShared = false)
        {
            var bm = SharedResources.Helpers.ResourceManager.GetCommonImage(Properties.Settings.Default.TypeLabel, image, bShared);
            return bm;
        }

        private void GetComponentInterfaces()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            var ext = Properties.Settings.Default.DefaultFileExt.ToLower().Replace(".", "");
            ApplicationPropertiesHelper.SetProperty(ext, Path.GetFileNameWithoutExtension(assembly.Location));

            LoadRequiredAssemblies();

            if (workspace == null)
                workspace = GetService(typeof(IWorkspace)) as IWorkspace;
            if (workspace != null)
            {
                workspace.Closing += workspace_Closing;
                workspace.Closed += workspace_Closed;
                workspace.CloseButtonClick += workspace_CloseButtonClick;
                workspace.ActiveWindowChanged += workspace_ActiveWindowChanged;
                workspace.PromptFriendObjects += workspace_PromptFriendObjects;
                workspace.PromptDocumentEditorObject += workspace_PromptDocumentEditorObject;
                workspace.RibbonSelectionChanged += workspace_RibbonSelectionChanged;
            }

            if (PropertyControl != null)
            {
                var dt = new DataTemplate();
                var factory = new FrameworkElementFactory(typeof(CommandManager.PropertyDataTemplate.MenuUriPropertyEditor));
                dt.VisualTree = factory;
                PropertyControl.AddPropertyEditor("MenuName", typeof(Uri), typeof(ScreenSettings.Entities.ScreenEntity), dt);

                dt = new DataTemplate();
                factory = new FrameworkElementFactory(typeof(WPFUtilities.PropertyDataTemplate.TextPropertyEditor));
                factory.SetValue(WPFUtilities.PropertyDataTemplate.TextPropertyEditor.WorkspaceProperty, workspace);
                dt.DataType = typeof(String);
                dt.VisualTree = factory;
                PropertyControl.AddPropertyEditor("ListItemSources", typeof(String), typeof(ScreenSettings.Entities.ScreenEntity), dt);

                dt = new DataTemplate();
                factory = new FrameworkElementFactory(typeof(DataReaderEditor.PropertyDataTemplate.DataSourcePropertyEditor));
                factory.SetValue(DataReaderEditor.PropertyDataTemplate.DataSourcePropertyEditor.WorkspaceProperty, workspace);
                dt.DataType = typeof(DataReader.DataReaderModel);
                dt.VisualTree = factory;
                PropertyControl.AddPropertyEditor("ReaderItemSources", typeof(DataReader.DataReaderModel), typeof(ScreenSettings.Entities.ScreenEntity), dt);

                dt = new DataTemplate();
                factory = new FrameworkElementFactory(typeof(ExpressionPropertyEditor));
                factory.SetValue(ExpressionPropertyEditor.WorkspaceProperty, workspace);
                dt.VisualTree = factory;
                PropertyControl.AddPropertyEditor("Expression", typeof(String), typeof(ScreenSettings.Entities.ScreenEntity), dt);
                PropertyControl.AddPropertyEditor("ReverseExpression", typeof(String), typeof(ScreenSettings.Entities.ScreenEntity), dt);

                dt = new DataTemplate();
                factory = new FrameworkElementFactory(typeof(WPFUtilities.PropertyDataTemplate.IdUnitConverterPropertyEditor));
                factory.SetValue(IdUnitConverterPropertyEditor.WorkspaceProperty, workspace);
                dt.DataType = typeof(String);
                dt.VisualTree = factory;
                PropertyControl.AddPropertyEditor("IdUnitConverter", typeof(String), typeof(ScreenSettings.Entities.ScreenEntity), dt);

                dt = new DataTemplate();
                factory = new FrameworkElementFactory(typeof(WPFUtilities.PropertyDataTemplate.BitMaskPropertyEditor));
                dt.DataType = typeof(int);
                dt.VisualTree = factory;
                PropertyControl.AddPropertyEditor("ReadableAccessMask", typeof(int), typeof(ScreenSettings.Entities.ScreenEntity), dt);
                PropertyControl.AddPropertyEditor("WritableAccessMask", typeof(int), typeof(ScreenSettings.Entities.ScreenEntity), dt);

                dt = new DataTemplate();
                factory = new FrameworkElementFactory(typeof(PropertyDataTemplate.CultureFontListPropertyEditor));
                dt.DataType = typeof(Dictionary<string,FontSettings>);
                dt.VisualTree = factory;
                PropertyControl.AddPropertyEditor("FontSettingList", typeof(Dictionary<string, FontSettings>), typeof(ScreenSettings.Entities.ScreenEntity), dt);


                dt = new DataTemplate();
                factory = new FrameworkElementFactory(typeof(WPFUtilities.PropertyDataTemplate.BitMaskLevelPropertyEditor));
                dt.DataType = typeof(int);
                dt.VisualTree = factory;
                PropertyControl.AddPropertyEditor("VisibilityLevel", typeof(int), typeof(ScreenSettings.Entities.ScreenEntity), dt);

                dt = new DataTemplate();
                factory = new FrameworkElementFactory(typeof(NumericUpDownPropertyEditor));
                factory.SetValue(NumericUpDownPropertyEditor.MinValueProperty, 0d);
                factory.SetValue(NumericUpDownPropertyEditor.MaxValueProperty, double.MaxValue);
                factory.SetValue(NumericUpDownPropertyEditor.SpinValueProperty, 0.5);
                factory.SetValue(NumericUpDownPropertyEditor.MaskUseAsDisplayFormatProperty, false);
                dt.DataType = typeof(double);
                dt.VisualTree = factory;
                PropertyControl.AddPropertyEditor("Width", typeof(double), typeof(FrameworkElement), typeof(ScreenDocument), dt, true);
                PropertyControl.AddPropertyEditor("Height", typeof(double), typeof(FrameworkElement), typeof(ScreenDocument), dt, true);
                PropertyControl.AddPropertyEditor("Canvas.Left", typeof(double), typeof(FrameworkElement), typeof(ScreenDocument), dt, true);
                PropertyControl.AddPropertyEditor("Canvas.Top", typeof(double), typeof(FrameworkElement), typeof(ScreenDocument), dt, true);

                dt = new DataTemplate();
                factory = new FrameworkElementFactory(typeof(PropertyDataTemplate.BrushPropertyEditor));
                factory.SetValue(PropertyDataTemplate.BrushPropertyEditor.EditOptionProperty, ScreenManager.PropertyDataTemplate.EditOptionsEnum.Background);
                dt.DataType = typeof(Brush);
                dt.VisualTree = factory;
                PropertyControl.AddPropertyEditor("Background", typeof(Brush), typeof(FrameworkElement), typeof(ScreenDocument), dt, true);
                PropertyControl.AddPropertyEditor("Fill", typeof(Brush), typeof(FrameworkElement), typeof(ScreenDocument), dt, true);

                dt = new DataTemplate();
                factory = new FrameworkElementFactory(typeof(PropertyDataTemplate.BrushPropertyEditor));
                factory.SetValue(PropertyDataTemplate.BrushPropertyEditor.EditOptionProperty, ScreenManager.PropertyDataTemplate.EditOptionsEnum.Foreground);
                dt.DataType = typeof(Brush);
                dt.VisualTree = factory;
                PropertyControl.AddPropertyEditor("Foreground", typeof(Brush), typeof(FrameworkElement), typeof(ScreenDocument), dt, true);

                dt = new DataTemplate();
                factory = new FrameworkElementFactory(typeof(PropertyDataTemplate.BrushPropertyEditor));
                factory.SetValue(PropertyDataTemplate.BrushPropertyEditor.EditOptionProperty, ScreenManager.PropertyDataTemplate.EditOptionsEnum.BorderBrush);
                dt.DataType = typeof(Brush);
                dt.VisualTree = factory;
                PropertyControl.AddPropertyEditor("BorderBrush", typeof(Brush), typeof(FrameworkElement), typeof(ScreenDocument), dt, true);
                PropertyControl.AddPropertyEditor("Stroke", typeof(Brush), typeof(FrameworkElement), typeof(ScreenDocument), dt, true);

                dt = new DataTemplate();
                factory = new FrameworkElementFactory(typeof(PropertyDataTemplate.AnimationExplorerPropertyEditor));
                dt.DataType = typeof(bool);
                dt.VisualTree = factory;
                PropertyControl.AddPropertyEditor("Animations", typeof(bool), typeof(ScreenEntity), typeof(ScreenDocument), dt);

                dt = new DataTemplate();
                factory = new FrameworkElementFactory(typeof(PropertyDataTemplate.CommandExplorerPropertyEditor));
                dt.DataType = typeof(bool);
                dt.VisualTree = factory;
                PropertyControl.AddPropertyEditor("Commands", typeof(bool), typeof(ScreenEntity), typeof(ScreenDocument), dt);

                dt = new DataTemplate();
                factory = new FrameworkElementFactory(typeof(PropertyDataTemplate.GeoScadaExplorerPropertyEditor));
                dt.DataType = typeof(bool);
                dt.VisualTree = factory;
                PropertyControl.AddPropertyEditor("GeoScada", typeof(bool), typeof(ScreenDocument), dt);

                Dispatcher.CurrentDispatcher.BeginInvokeAsynchronouslyInBackground(() =>
                {
                    if (UserEditor != null)
                    {
                        Type accRoleType = UserEditor.GetAccessRoleEditorType();
                        dt = new DataTemplate();
                        factory = new FrameworkElementFactory(accRoleType);
                        dt.DataType = typeof(string);
                        dt.VisualTree = factory;
                        PropertyControl.AddPropertyEditor("AccessRole", typeof(string), typeof(ScreenSettings.Entities.ScreenEntity), dt);
                    }
                });
                //PropertyControl.AcceptChanges += PropertyControl_AcceptChanges;
            }
        }

        //void PropertyControl_AcceptChanges(object sender, EventArgs e)
        //{
        //    using (var cursor = new WaitCursor())
        //    {
        //        var list = (from c in mapActiveDocuments.Values// .AsParallel()
        //                    where c.NeedsSave == true && c.ActiveView == null
        //                    select c).ToList();
        //        list.ForEach(doc =>
        //        {
        //            if (doc.NeedsSave)
        //            {
        //                var res = UIInterface.ShowYesNo(String.Format(Properties.Resources.SaveDoc,
        //                    doc.Title), CustomDialogIcons.Question);
        //                if (res == CustomDialogResults.Yes)
        //                {
        //                    if (doc.SaveToFile())
        //                        doc.NeedsSave = false;
        //                }
        //            }
        //        });
        //    }
        //}

        void workspace_RibbonSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //if (e.AddedItems.Contains(ribbonInsert))
            //{
            //    bInsertRibbonIsActive = true;
            //    if (IdleExecutionPendingRibbons == null)
            //    {
            //        Action action = () => IdleExecutionRibbonPending();
            //        IdleExecutionPendingRibbons = Dispatcher.CurrentDispatcher.BeginInvoke(action, DispatcherPriority.Background);
            //        IdleExecutionPendingRibbons.Completed += (s, ev) =>
            //        {
            //            IdleExecutionPendingRibbons = null;
            //        };
            //    }
            //}
            //if (e.RemovedItems.Contains(ribbonInsert))
            //    bInsertRibbonIsActive = false;
        }

        void LoadFromXml(string filepath)
        {
            //mapItems.Clear();

            try
            {
                var keyexpandolist = Utilities.XmlHelper.GetExpandoAttributeFromXml(File.ReadAllText(filepath), "resources", true);
                if (keyexpandolist.Count() != 0)
                {
                    keyexpandolist.ToList().ForEach(e =>
                    {
                        var regkeydictionary = e as IDictionary<string, object>;
                        regkeydictionary.ToList().ForEach(r =>
                        {
                            mapItems.Add(r.Key.ToString(), r.Value.ToString());
                        });
                    });
                }
            }
            catch (Exception e)
            {

            }

        }

        bool bInsertRibbonIsActive;
        DispatcherOperation IdleExecutionPendingRibbons;
        readonly IEnumerable<string> tables = new string[1] { "Toolbox" };
        void IdleExecutionRibbonPending()
        {
            if (!bLoaded)
            {
                bLoaded = true;

                mapItems.Clear();

                string mainversion = Utilities.AssemblyInfo.FileFormatMainVersion;
                string startingPath = String.Format("{0}.{1}\\Cultures\\", Utilities.ApplicationPropertiesHelper.GetProperty<String>("CommonFolder"), mainversion);
                string fileToOpen = string.Empty;

                foreach (string key in tables)
                {
                    fileToOpen = string.Format("{0}{1}\\StringTable_{2}.xml", startingPath, System.Threading.Thread.CurrentThread.CurrentUICulture.Name, key);

                    if (!File.Exists(fileToOpen))
                        fileToOpen = string.Format("{0}StringTable_{1}.xml", startingPath, key);
                    if (File.Exists(fileToOpen))
                    {
                        LoadFromXml(fileToOpen);
                    }
                }
            }
        }

        void workspace_PromptDocumentEditorObject(object sender, GetDocumentEditorObjectEventArgs e)
        {
            if (sender is ScreenDocument)
            {
                e.documentEditor = (sender as ScreenDocument).ActiveView;
                return;
            }

            if (!(sender is FrameworkElement))
                return;
            FrameworkElement element = sender as FrameworkElement;
            foreach (var Doc in mapActiveDocumentUris.Keys)
            {
                if (Doc.ActiveView == null || !LayoutHelper.IsChildElement(Doc.ActiveView, element))
                    continue;

                e.documentEditor = Doc.ActiveView;
                break;
            }
        }

        void workspace_PromptFriendObjects(object sender, GetFriendObjectsEventArgs e)
        {
            if (!(sender is FrameworkElement))
                return;
            FrameworkElement element = sender as FrameworkElement;

            foreach (var Doc in mapActiveDocumentUris.Keys)
            {
                if (!LayoutHelper.IsChildElement(Doc.ActiveView, element))
                    continue;

                // Screen.RegisterName(Screen.MainSurface, element);

                Doc.GetFriendObjects(element, e);
                break;
            }
        }

        void workspace_CloseButtonClick(object sender, UFInterfaces.CloseButtonEventArgs e)
        {
            if (!(e.TargetItem is ScreenEditorView))
                return;

            ScreenEditorView view = e.TargetItem as ScreenEditorView;
            if (!CloseScreen(view))
                e.Cancel.Cancel = true;
        }

        bool CanClose(ScreenEditorView view, bool bSave = true)
        {
            if (view == null)
                return true;
            var doc = view.Document;
            if (doc == null)
                return true;

            String uri;
            if (mapActiveDocumentUris.TryGetValue(doc, out uri))
            {
                if (bSave)
                {
                    if (doc.NeedsSave)
                    {
                        if (UIInterface != null)
                        {
                            var res = UIInterface.ShowYesNoCancel(String.Format(Properties.Resources.SaveDoc,
                                GetDocumentTitle(uri)), CustomDialogIcons.Question);
                            if (res == CustomDialogResults.Cancel || res == CustomDialogResults.None)
                                return false;
                            bSave = res == CustomDialogResults.Yes;
                        }

                        if (bSave)
                        {
                            if (!view.SaveCurrentDocument())
                                return false;
                        }
                    }

                    if (!view.CanClose())
                        return false;
                }
            }

            return true;
        }

        private bool CloseScreen(ScreenEditorView view, bool bSave = true)
        {
            if (view == null)
                return true;

            String uri;
            var doc = view.Document;
            if (doc != null && mapActiveDocumentUris.TryGetValue(doc, out uri))
            {
                if (bSave && doc.NeedsSave)
                {
                    if (UIInterface != null)
                    {
                        var res = UIInterface.ShowYesNoCancel(String.Format(Properties.Resources.SaveDoc,
                            GetDocumentTitle(uri)), CustomDialogIcons.Question);
                        if (res == CustomDialogResults.Cancel || res == CustomDialogResults.None)
                            return false;
                        bSave = res == CustomDialogResults.Yes;
                    }

                    if (bSave)
                    {
                        view.SaveCurrentDocument();
                    }
                }

                view.SaveImage();
                mapActiveDocumentUris.Remove(doc);
                mapActiveDocuments.Remove(uri);
                mapActiveDocumentTitles.Remove(uri);
                var tb = menuControl.GetToolbar();
                if (tb != null)
                    tb.Editor = null;
            }
            if (doc != null)
            {
                doc.PropertyChanged -= Document_PropertyChanged;
                IDocument parent = DocumentHelper.GetRootParent(doc, false);
                if (parent is INotifyPropertyChanged)
                    (parent as INotifyPropertyChanged).PropertyChanged -= Parent_PropertyChanged;
            }

            view.OnDeactivate();

            if (doc is IDisposable)
                (doc as IDisposable).Dispose();
            view.Dispose();
            workspace.RemoveDockingChildren(view);

            return true;
        }

        void workspace_Closed(object sender, EventArgs e)
        {
            if (mapActiveDocuments.Count == 0)
                return;

            ScreenDocument[] array = new ScreenDocument[mapActiveDocuments.Values.Count];
            mapActiveDocuments.Values.CopyTo(array, 0);
            foreach (var doc in array)
            {
                ScreenEditorView view = doc.ActiveView as ScreenEditorView;
                if (view != null)
                    CloseScreen(view, false);
            }
        }

        void workspace_Closing(object sender, CancelEventArgs e)
        {
            if (mapActiveDocuments.Count == 0)
                return;

            ScreenDocument[] array = new ScreenDocument[mapActiveDocuments.Values.Count];
            mapActiveDocuments.Values.CopyTo(array, 0);
            foreach (var doc in array)
            {
                ScreenEditorView view = doc.ActiveView as ScreenEditorView;
                if (view != null && !CanClose(view))
                {
                    e.Cancel = true;
                    break;
                }
            }
        }

        void GetGlobalCbs(ScreenEditorView view)
        {
            var globalCmds = (from DevExpress.Xpf.Bars.BarItem bi in menuControl.menuGeneralItems.GetChildrenOfType<DevExpress.Xpf.Bars.BarItem>() where bi.Command != null select bi.Command).ToList();
            globalCbs = (from CommandBinding cb in view.CommandBindings where globalCmds.Contains(cb.Command) select cb).ToList();
        }

        void workspace_ActiveWindowChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var viewOld = from entry in mapActiveDocuments/*.AsParallel()*/ where entry.Value.ActiveView == e.OldValue select entry.Value.ActiveView;
            var viewNew = from entry in mapActiveDocuments/*.AsParallel()*/ where entry.Value.ActiveView == e.NewValue select entry.Value.ActiveView;

            if (e.OldValue != null && e.OldValue is ScreenEditorView && viewOld != null)
            {
                var view = e.OldValue as ScreenEditorView;
                
                if (globalCbs == null)
                    GetGlobalCbs(view);
                Workspace.RemoveBarManagerCommands(new CommandBindingCollection(globalCbs));
                
                view.OnDeactivate();

                var dockstate = workspace.GetElementDockState(view);
                if (dockstate == UFInterfaces.DockState.Document && !view.IsLoaded)
                {
                    view.Visibility = Visibility.Collapsed;
                }
            }

            if (e.NewValue != null && e.NewValue is ScreenEditorView && viewNew != null)
            {
                var view = e.NewValue as ScreenEditorView;
                var tb = menuControl.GetToolbar();
                if (tb != null)
                    tb.Editor = view;
                menuControl.DataContext = workspace.ContextDocument = view.Document;

                if (globalCbs == null)
                    GetGlobalCbs(view);
                Workspace.AddBarManagerGlobalCommands(new CommandBindingCollection(globalCbs));

                view.OnActivate(true);

                var dockstate = workspace.GetElementDockState(view);
                if (dockstate == UFInterfaces.DockState.Document && view.IsLoaded)
                {
                    view.Visibility = Visibility.Visible;
                }
            }
        }

        bool bChangingFont;
        private void fontSizeBox_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            bChangingFont = true;
        }

        bool bSelectingFont;
        void ScreenManagerComponent_SelectionChanged(object sender, EventArgs e)
        {
            bSelectingFont = true;
            bSelectingFont = false;
        }

        //private static void InitializeFontComboBox(ComboBox r_Combo)
        //{
        //    var list = (from c in Fonts.SystemFontFamilies 
        //                orderby c.Source
        //                select c).ToList();
        //    foreach (FontFamily fontFamily in list)
        //    {
        //        ComboBoxItem item = new ComboBoxItem { Content = fontFamily };
        //        item.FontFamily = fontFamily;
        //        r_Combo.Items.Add(item);
        //    }
        //}

        //private static void InitializeFontSizeComboBox(ComboBox r_combo)
        //{
        //    int[] sizes = new int[27] { 8, 9, 10, 11, 12, 14, 16, 18, 20, 22, 24, 28, 36, 48, 72, 82, 92, 102, 112, 122, 132, 142, 160, 180, 200, 220, 240 };
        //    for (int i = 0, cnt = sizes.Length; i < cnt; ++i)
        //    {
        //        ComboBoxItem item = new ComboBoxItem { Content = sizes[i] };
        //        r_combo.Items.Add(item);
        //    }
        //}
#endif

#if !WINDOWS_UWP
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
#endif
        public void LoadRequiredAssemblies()
        {
            if (!String.IsNullOrEmpty(Properties.Settings.Default.RequiredAssemblies))
            {
                var array = Properties.Settings.Default.RequiredAssemblies.Split(';');
                foreach (var assembly in array)
                {
                    try
                    {
                        Assembly.Load(assembly);
                    }
                    catch (Exception ex)
                    {
#if !WINDOWS_UWP
                        logGeneral.Debug(ex.Message);
#endif
                    }
                }
            }
        }

        String GetDocumentTitle(Uri uri)
        {
            return Path.GetFileNameWithoutExtension(uri.GetPathString());
        }

        String GetDocumentTitle(String uri)
        {
            return Path.GetFileNameWithoutExtension(uri);
        }

#if !WINDOWS_UWP && !NET_STANDARD
        String CreateDocumentTitle(Uri uri, IDocument parent)
        {
            lock (lockObject)
            {
                var relative = parent.MakeRelativeUri(new Uri(uri.GetPathString(), UriKind.RelativeOrAbsolute));
                String name = Path.GetFileNameWithoutExtension(relative.GetPathString());
                String folder = Path.GetDirectoryName(relative.GetPathString());
                folder = folder.Replace(String.Format("{0}\\", TypeLabel), "");
                String ret = null;
                if (String.IsNullOrEmpty(folder) || folder == TypeLabel)
                    ret = name;
                else
                    ret = String.Format("{0}\\{1}", folder, name);
                //String sourcefmt = ret;
                //int i = 1;
                //while (mapActiveDocumentTitles.ContainsValue(ret))
                //    ret = String.Format("{0}{1}", sourcefmt, i++);

                mapActiveDocumentTitles.Add(uri.GetPathString(), ret);

                return ret;
            }
        }

        void Document_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (String.Compare(e.PropertyName, "NeedsSave", false) == 0)
            {
                foreach (KeyValuePair<String, ScreenDocument> keyvaluepair in mapActiveDocuments)
                {
                    if (keyvaluepair.Value != sender)
                        continue;

                    workspace.SetChangedDocumentTitle(keyvaluepair.Value.ActiveView, keyvaluepair.Value.NeedsSave);
                    break;
                }
            }
            else if (String.Compare(e.PropertyName, "LayoutChanged", false) == 0)
            {
                foreach (KeyValuePair<String, ScreenDocument> keyvaluepair in mapActiveDocuments)
                {
                    if (keyvaluepair.Value != sender)
                        continue;

                    workspace.SetChangedDocumentTitle(keyvaluepair.Value.ActiveView, keyvaluepair.Value.LayoutChanged);
                    break;
                }
            }
        }

        public static Uri CreateDocument(Uri uri, IDocument parent, String model)
        {
            String name = Path.GetFileNameWithoutExtension(uri.GetPathString());
            String sourcefilename = model;
            String fileName = String.Format("{0}\\{1}{2}", Path.GetDirectoryName(uri.GetPathString()), name, Properties.Settings.Default.DefaultFileExt);
            if (parent != null && parent.fileSystemProviderBase != null)
            {
                //parent.fileSystemProviderBase.UploadFile(null, fileName,
                //    System.Text.Encoding.Unicode.GetBytes(newScreenType.xamlCode));
                var sourcesettingsFileName = ScreenDocument.GetSettingsFileName(sourcefilename);
                var destsettingsFileName = ScreenDocument.GetSettingsFileName(fileName);
                if (File.Exists(sourcesettingsFileName))
                {
                    parent.fileSystemProviderBase.UploadFile(null, destsettingsFileName, File.ReadAllBytes(sourcesettingsFileName));
                }

                if (File.Exists(sourcefilename))
                {
                    var text = File.ReadAllText(sourcefilename);
                    var data = System.Text.Encoding.Unicode.GetBytes(text);
                    parent.fileSystemProviderBase.UploadFile(null, fileName, data);
                }
            }
            else
                ScreenDocument.CopyFile(sourcefilename, fileName, true, parent, false);

            return new Uri(fileName, UriKind.RelativeOrAbsolute);
        }

        static Uri CreateNamedDocument(Uri uri, IDocument parent, String model)
        {
            //String startingPath = ApplicationPropertiesHelper.GetProperty<String>("CommonFolder");
            //string mainversion = Utilities.AssemblyInfo.FileFormatMainVersion;
            //var ResourcePath = String.Format("{0}.{1}\\NewScreenTypes", startingPath, mainversion);
            //if (!Directory.Exists(ResourcePath))
            if(!File.Exists(model))
                return null;

            //var ResourceFileName = String.Format("{0}.{1}\\NewScreenTypes\\{2}{3}", startingPath, mainversion, model, ".xaml");

            String fileName = /*String.Format("{0}{1}", */uri.GetPathString()/*, Properties.Settings.Default.DefaultFileExt)*/;
            if (parent != null && parent.fileSystemProviderBase != null)
            {
                //parent.fileSystemProviderBase.UploadFile(null, fileName,
                //    System.Text.Encoding.Unicode.GetBytes(newScreenType.xamlCode));
                var sourcesettingsFileName = ScreenDocument.GetSettingsFileName(model/*ResourceFileName*/);
                var destsettingsFileName = ScreenDocument.GetSettingsFileName(fileName);
                if (File.Exists(sourcesettingsFileName))
                {
                    parent.fileSystemProviderBase.UploadFile(null, destsettingsFileName, File.ReadAllBytes(sourcesettingsFileName));
                }

                if (File.Exists(model/*ResourceFileName*/))
                {
                    var text = File.ReadAllText(model/*ResourceFileName*/);
                    var data = System.Text.Encoding.Unicode.GetBytes(text);
                    parent.fileSystemProviderBase.UploadFile(null, fileName, data);
                }
            }
            else
                ScreenDocument.CopyFile(model/*ResourceFileName*/, fileName, true, parent, false);
            // File.WriteAllText(fileName, newScreenType.xamlCode);

            return new Uri(fileName, UriKind.RelativeOrAbsolute);
        }
        Uri CreateDefaultDocument(Uri uri, IDocument parent, bool encryptFile)
        {
            IDocument p = DocumentHelper.GetRootParent(parent, false);
            var storeFileName = String.Format("{0}{1}.{2}", parent.GetSpecialFolder(SpecialFolders.Documents).GetPathString(), p.Title, Properties.Settings.Default.NewScreenSettingsExtension);
            lastTriedDocumentFileName = null;

            String filePath = String.Empty;
            while (true)
            {
                var newScreenType = new NewScreen(storeFileName)
                {
                    fileName = lastTriedDocumentFileName ?? Path.GetFileNameWithoutExtension(uri.GetPathString()),
                    xamlCode = ScreenDocument.DefaultXaml
                };

                var Dialog = new GeneralDialogContent(newScreenType) { Owner = Application.Current.MainWindow };
                if (parent != null && parent.ActiveView != null)
                    Dialog.Owner = parent.ActiveView.FindParent<Window>();
                Dialog.Title = Properties.Resources.NewScreen;
                Dialog.HelpLink = "NewEmptyScreen";
                if (Dialog.ShowDialog() != true)
                    return null;
                
                string destfilename = newScreenType.ScreenName;
                string xamlCode = (newScreenType as NewScreen).xamlCode;
                string xamlSettingCode = (newScreenType as NewScreen).xamlSettingCode;

                if (string.IsNullOrEmpty(destfilename) || string.IsNullOrEmpty(xamlCode) || string.IsNullOrEmpty(xamlSettingCode))
                    return null;

                string sourcefileName = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
                sourcefileName = Path.ChangeExtension(sourcefileName, "xaml");
                var fileTempsettings = ScreenDocument.GetSettingsFileName(sourcefileName);
                try
                {
                    File.WriteAllText(sourcefileName, xamlCode);
                    File.WriteAllText(fileTempsettings, xamlSettingCode);

                    filePath = String.Format("{0}\\{1}{2}", Path.GetDirectoryName(uri.GetPathString()), destfilename, Properties.Settings.Default.DefaultFileExt);
                    if (parent != null && parent.fileSystemProviderBase != null)
                    {
                        if (parent.fileSystemProviderBase.Exists(new FileManagerFile(parent.fileSystemProviderBase, filePath)))
                        {
                            if (UIInterface != null)
                            {
                                UIInterface.ShowError(Properties.Resources.NameAlreadyInUse);
                            }
                        }
                        else
                        {
                            var sourcesettingsFileName = ScreenDocument.GetSettingsFileName(sourcefileName);
                            var destsettingsFileName = ScreenDocument.GetSettingsFileName(filePath);
                            if (File.Exists(sourcesettingsFileName))
                            {
                                parent.fileSystemProviderBase.UploadFile(null, destsettingsFileName, File.ReadAllBytes(sourcesettingsFileName));
                            }

                            if (File.Exists(sourcefileName))
                            {
                                var text = File.ReadAllText(sourcefileName);
                                var data = System.Text.Encoding.Unicode.GetBytes(text);
                                parent.fileSystemProviderBase.UploadFile(null, filePath, data);
                            }
                            break;
                        }
                    }
                    else
                    {
                        if (File.Exists(filePath))
                        {
                            if (UIInterface != null)
                            {
                                UIInterface.ShowError(Properties.Resources.NameAlreadyInUse);
                            }
                        }
                        else
                        {
                            ScreenDocument.CopyFile(sourcefileName, filePath, true, parent, false);
                            if (encryptFile)
                            {
                                SaveDocument(parent, new Uri(filePath, UriKind.RelativeOrAbsolute), encryptFile);
                            }
                            break;
                        }
                    }
                    // File.WriteAllText(fileName, newScreenType.xamlCode);
                }
                catch
                {
                    lastTriedDocumentFileName = destfilename;
                    throw;
                }
                finally
                {
                    if (File.Exists(sourcefileName))
                        File.Delete(sourcefileName);
                }
            }
            return new Uri(filePath, UriKind.RelativeOrAbsolute);
        }
        Uri CreateDefaultTemplatedDocument(Uri uri, IDocument parent, bool encryptFile)
        {
            var newScreenType = (UserControl)new NewScreenType()
            {
                fileName = Path.GetFileNameWithoutExtension(uri.GetPathString()),
                xamlCode = ScreenDocument.DefaultXaml
            };

            String filePath = String.Empty;
            while (true)
            {
                var Dialog = new GeneralDialogContent(newScreenType) { Owner = Application.Current.MainWindow };
                if (parent != null && parent.ActiveView != null)
                    Dialog.Owner = parent.ActiveView.FindParent<Window>();
                Dialog.Title = Properties.Resources.NewScreen;
                Dialog.HelpLink = "NewScreen";
                if (Dialog.ShowDialog() != true)
                {
                    return null;
                }
                string destfilename = (newScreenType as NewScreenType).fileName;
                string sourcefileName = (newScreenType as NewScreenType).sourcefileName;

                if (string.IsNullOrEmpty(destfilename) || string.IsNullOrEmpty(sourcefileName))
                    return null;
                filePath = String.Format("{0}\\{1}{2}", Path.GetDirectoryName(uri.GetPathString()), destfilename, Properties.Settings.Default.DefaultFileExt);
                if (parent != null && parent.fileSystemProviderBase != null)
                {
                    if (parent.fileSystemProviderBase.Exists(new FileManagerFile(parent.fileSystemProviderBase, filePath)))
                    {
                        if (UIInterface != null)
                        {
                            UIInterface.ShowError(Properties.Resources.NameAlreadyInUse);
                        }
                    }
                    else
                    {
                        //parent.fileSystemProviderBase.UploadFile(null, fileName,
                        //    System.Text.Encoding.Unicode.GetBytes(newScreenType.xamlCode));
                        var sourcesettingsFileName = ScreenDocument.GetSettingsFileName(sourcefileName);
                        var destsettingsFileName = ScreenDocument.GetSettingsFileName(filePath);
                        if (File.Exists(sourcesettingsFileName))
                        {
                            parent.fileSystemProviderBase.UploadFile(null, destsettingsFileName, File.ReadAllBytes(sourcesettingsFileName));
                        }

                        if (File.Exists(sourcefileName))
                        {
                            var text = File.ReadAllText(sourcefileName);
                            var data = System.Text.Encoding.Unicode.GetBytes(text);
                            parent.fileSystemProviderBase.UploadFile(null, filePath, data);
                        }
                        break;
                    }
                }
                else
                {
                    if (File.Exists(filePath))
                    {
                        if (UIInterface != null)
                        {
                            UIInterface.ShowError(Properties.Resources.NameAlreadyInUse);
                        }
                    }
                    else
                    {
                        ScreenDocument.CopyFile(sourcefileName, filePath, true, parent, false);
                        if (encryptFile)
                        {
                            SaveDocument(parent, new Uri(filePath, UriKind.RelativeOrAbsolute), encryptFile);
                        }
                        break;
                    }
                }
                // File.WriteAllText(fileName, newScreenType.xamlCode);
            }
            return new Uri(filePath, UriKind.RelativeOrAbsolute);
        }
        public Uri GetFirstAvailableDoc()
        {
            if (mapActiveDocumentUris.Count > 0)
                return new Uri(mapActiveDocumentUris.Values.First(), UriKind.RelativeOrAbsolute);
            return null;
        }

        public ScreenDocument OpenDocument(Uri uri, IDocument parent)
        {
            return ScreenDocument.FromFile(uri.GetPathString(), parent);
        }

        public ScreenEditorView GetActiveView(Uri uri)
        {
            ScreenDocument doc = null;
            if (mapActiveDocuments.TryGetValue(uri.GetPathString(), out doc))
                return doc.ActiveView as ScreenEditorView;
            return null;
        }
#endif

#region IDocumentManager Members

#if !WINDOWS_UWP && !NET_STANDARD
        public void Edit(Uri uri, IDocument parent)
        {
            using (new WaitCursor())
            {
                lock (lockObject)
                {
                    ScreenDocument doc = null;
                    if (mapActiveDocuments.TryGetValue(uri.GetPathString(), out doc) && doc.ActiveView != null)
                    {
                        if (doc.Parent == parent)
                            workspace.ActivateDockedElement(doc.ActiveView);
                        else
                        {
                            if (!CloseScreen(doc.ActiveView as ScreenEditorView))
                                return;
                            doc = null;
                        }
                    }

                    if (doc == null)
                        doc = ScreenDocument.FromFile(uri.GetPathString(), parent);

                    if (doc != null && doc.ActiveView == null)
                    {
                        SymbolGallery.GetHashCode();
                        AnimationExplorer.GetHashCode();
                        CommandExplorer.GetHashCode();

                        ScreenEditorView screenEditor = new ScreenEditorView(this, doc);
                        doc.Parent = parent;
                        doc.ActiveView = screenEditor;

                        var tb = menuControl.GetToolbar();
                        if (tb != null)
                            tb.Editor = screenEditor;

                        menuControl.DataContext = doc;

                        workspace.SetDesiredHeightAndWidthInDockedMode(screenEditor, screenEditor.Height, screenEditor.Width);
                        screenEditor.ClearValue(FrameworkElement.WidthProperty);
                        screenEditor.ClearValue(FrameworkElement.HeightProperty);

                        BitmapImage bm = GetBitmapImage("SMEditorSmall");

                        if (!mapActiveDocuments.ContainsKey(uri.GetPathString()))
                            mapActiveDocuments.Add(uri.GetPathString(), doc);

                        if (!mapActiveDocumentUris.ContainsKey(doc))
                            mapActiveDocumentUris.Add(doc, uri.GetPathString());

                        workspace.AddDockingChildren(screenEditor,
                            String.Format("{0} ({1})", CreateDocumentTitle(uri, parent), parent.Title),
                            UFInterfaces.DockState.Document, UFInterfaces.DockSide.Left);
                        workspace.SetDockedElementIcon(screenEditor, new ImageBrush(bm));

                        workspace.ActivateDockedElement(screenEditor);

                        screenEditor.Document.PropertyChanged += Document_PropertyChanged;
                        if (parent is INotifyPropertyChanged)
                            (parent as INotifyPropertyChanged).PropertyChanged += Parent_PropertyChanged;
                    }
                }
            }
        }

        private void Parent_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (String.Compare(e.PropertyName, "ProjectType", false) == 0)
            {
                foreach (KeyValuePair<String, ScreenDocument> keyvaluepair in mapActiveDocuments)
                {
                    ScreenEditorView screenEditor = keyvaluepair.Value.ActiveView as ScreenEditorView;
                    screenEditor?.ProjectTypeChanged();
                }
            }
        }

        public void Copy(Uri uri, String newPath, bool bCopy, IDocument parent, bool bUploading)
        {
            using (new WaitCursor())
            {
                lock (lockObject)
                {
                    ScreenDocument doc = null;
                    if (mapActiveDocuments.TryGetValue(uri.GetPathString(), out doc))
                    {
                        if (!bCopy)
                            CloseScreen(doc.ActiveView as ScreenEditorView);
                        else if (doc.NeedsSave)
                        {
                            if (UIInterface != null)
                            {
                                var res = UIInterface.ShowYesNoCancel(String.Format(Properties.Resources.SaveDoc,
                                    GetDocumentTitle(uri)), CustomDialogIcons.Question);
                                if (res == CustomDialogResults.Cancel || res == CustomDialogResults.None)
                                    return;
                                if (res == CustomDialogResults.Yes)
                                {
                                    var view = doc.ActiveView as ScreenEditorView;
                                    if (view == null)
                                        return;
                                    view.SaveCurrentDocument();
                                }
                            }
                        }
                    }

                    ScreenDocument.CopyFile(uri.GetPathString(), newPath, bCopy, parent, bUploading);
                }
            }
        }

        public void Rename(Uri uri, String oldName, String newName, IDocument parent)
        {
            using (new WaitCursor())
            {
                lock (lockObject)
                {
                    ScreenDocument doc = null;
                    bool bReopen = false;
                    mapActiveDocuments.TryGetValue(uri.GetPathString(), out doc);
                    if (doc != null && doc.ActiveView is ScreenEditorView)
                    {
                        bReopen = true;
                        if (!CloseScreen(doc.ActiveView as ScreenEditorView))
                            return;
                    }
                    else if (doc != null)
                    {
                        doc.PropertyChanged -= Document_PropertyChanged;
                        var root = DocumentHelper.GetRootParent(doc, false);
                        if (root is INotifyPropertyChanged)
                            (root as INotifyPropertyChanged).PropertyChanged -= Parent_PropertyChanged;
                        if (doc is IDisposable)
                            (doc as IDisposable).Dispose();
                    }

                    if (mapActiveDocuments.ContainsKey(uri.GetPathString()))
                        mapActiveDocuments.Remove(uri.GetPathString());
                    if (doc != null && mapActiveDocumentUris.ContainsKey(doc))
                        mapActiveDocumentUris.Remove(doc);
                    if (mapActiveDocumentTitles.ContainsKey(uri.GetPathString()))
                        mapActiveDocumentTitles.Remove(uri.GetPathString());

                    var newname = ScreenDocument.RenameFile(uri.GetPathString(), oldName, newName, parent.fileSystemProviderBase);

                    if (bReopen)
                    {
                        Edit(new Uri(newname, UriKind.RelativeOrAbsolute), parent);
                    }
                }
            }
        }

        public void Delete(Uri uri, IDocument parent)
        {
            //if (UIInterface != null)
            //{
            //    if (UIInterface.ShowOkCancel(String.Format(Properties.Resources.ConfirmRemove,
            //        GetDocumentTitle(uri)), CustomDialogIcons.Exclamation) == CustomDialogResults.Cancel)
            //        return;
            //}

            using (new WaitCursor())
            {
                lock (lockObject)
                {
                    ScreenDocument doc = null;
                    if (mapActiveDocuments.TryGetValue(uri.GetPathString(), out doc))
                    {
                        if (doc.ActiveView is ScreenEditorView)
                            CloseScreen(doc.ActiveView as ScreenEditorView, false);
                        else
                        {
                            doc.PropertyChanged -= Document_PropertyChanged;
                            var root = DocumentHelper.GetRootParent(doc, false);
                            if (root is INotifyPropertyChanged)
                                (root as INotifyPropertyChanged).PropertyChanged -= Parent_PropertyChanged;
                            if (doc is IDisposable)
                                (doc as IDisposable).Dispose();
                        }
                    }

                    if (mapActiveDocuments.ContainsKey(uri.GetPathString()))
                        mapActiveDocuments.Remove(uri.GetPathString());
                    if (doc != null && mapActiveDocumentUris.ContainsKey(doc))
                        mapActiveDocumentUris.Remove(doc);
                    if (mapActiveDocumentTitles.ContainsKey(uri.GetPathString()))
                        mapActiveDocumentTitles.Remove(uri.GetPathString());

                    ScreenDocument.RemoveFile(uri.GetPathString(), parent, parent.fileSystemProviderBase);
                    if(parent.fileSystemProviderBase == null)
                    {
                        ScreenDocument.RemoveAllCompiledScreenFiles(uri.GetPathString());
                    }
                }
            }
        }

        public bool SaveDocument(IDocument parent, Uri uri, bool encryptFile)
        {
            if (mapActiveDocuments.ContainsKey(uri.GetPathString()))
                return mapActiveDocuments[uri.GetPathString()].SaveToFile(forceEncryption: encryptFile);
            var doc = ScreenDocument.FromFile(uri.GetPathString(), parent);
            if (doc != null)
            {
                doc.Parent = parent;
                using (doc)
                {
                    return doc.SaveToFile(forceEncryption: encryptFile);
                }
            }
            return false;
        }

        public void CleanCoreFiles(String projectPath)
        { }
#endif
        public IDocument GetDocument(Uri uri)
        {
            if (mapActiveDocuments.ContainsKey(uri.GetPathString()))
                return mapActiveDocuments[uri.GetPathString()];

            return null;
        }

        public IDocument GetChildDocument(Uri uri)
        {
            return null;
        }

#if !NET_STANDARD
#if !WINDOWS_UWP
        readonly Dictionary<Uri, List<Window>> mapDialogsOpen = new Dictionary<Uri, List<Window>>();
        readonly Dictionary<IDocument, ScreenTransitionViewer> mapMainWindows = new Dictionary<IDocument, ScreenTransitionViewer>();
        readonly Dictionary<IDocument, List<Window>> mapFrameWindows = new Dictionary<IDocument, List<Window>>();
        readonly Dictionary<IDocument, List<Window>> mapModalWindows = new Dictionary<IDocument, List<Window>>();
        readonly Dictionary<IDocument, String> mapMainTheme = new Dictionary<IDocument, String>();
#else
        readonly Dictionary<Uri, List<ContentDialog>> mapDialogsOpen = new Dictionary<Uri, List<ContentDialog>>();

        async void ShowDialog(ContentDialog wnd)
        {
            var res = await wnd.ShowAsync();
        }
#endif
        readonly List<Tuple<Uri, Window>> listPrevious = new List<Tuple<Uri, Window>>();
        readonly List<Uri> listNexts = new List<Uri>();
        readonly Dictionary<Uri, Object> mapUriContext = new Dictionary<Uri, Object>();
        bool bExecutingPrevious;

        static Tuple<Uri, Window> FindTupleByUri(List<Tuple<Uri, Window>> t, Uri uri)
        {
            return (from item in t where item.Item1 == uri select item).FirstOrDefault();
        }

        internal void UpdatePreviousList(Uri uri)
        {
            if (FindTupleByUri(listPrevious, uri) == null)
                listPrevious.Add(new Tuple<Uri, Window>(uri, null));
        }

        internal void UpdatePreviousListRemove(Uri uri)
        {
            var tuple = FindTupleByUri(listPrevious, uri);
            if (tuple != null)
            {
                listPrevious.Remove(tuple);
                if (listNexts.Contains(tuple.Item1))
                    listNexts.Remove(tuple.Item1);
                listNexts.Add(tuple.Item1);
            }
        }

        public bool OpenMap(IDocument parent, ExecutionMode mode, double X, double Y, 
            bool IsRelative, IEntityReference Entity, Rect openingArea, bool enableZoomingScrolling,
            bool showMiniMap, bool showNextButton, Point3D openingParameters)
        {
            UserControl activeView = null;
            var rootParent = parent.Parent;
            if (rootParent == null)
                rootParent = parent;

            var p = rootParent;
            var memParent = p;
            while (p != null)
            {
                p = p.Parent;
                if (p == null)
                    break;
                memParent = p;
            }
            rootParent = memParent;

            var docParent = parent.Parent;
            if (docParent == null || parent.IsRoot)
                docParent = parent;

            p = docParent;
            memParent = p;
            while (p != null)
            {
                p = p.Parent;
                if (p == null || p.IsRoot)
                    break;
                memParent = p;
            }
            docParent = memParent;

            if (mapMainWindows.ContainsKey(rootParent))
            {
                if (!mapMainWindows[rootParent].Dispatcher.CheckAccess())

                {
                    mapMainWindows[rootParent].Dispatcher.Invoke(
                        (Action)(() => 
                            OpenMap(
                                parent, 
                                mode, 
                                X, 
                                Y, 
                                IsRelative, 
                                Entity, 
                                openingArea, 
                                enableZoomingScrolling,
                                showMiniMap, 
                                showNextButton,
                                openingParameters)));
                    return true;
                }
            }
            if (rootParent == parent)
            {
                if (mapMainWindows.ContainsKey(parent))
                    activeView = mapMainWindows[parent];
                else
                    activeView = parent.ActiveView;
            }
            else
            {
                if (parent.ActiveView != null && parent.ActiveView.Dispatcher.CheckAccess())
                    activeView = parent.ActiveView;
                else if (mapMainWindows.ContainsKey(rootParent))
                    activeView = mapMainWindows[rootParent];
            }

            var screenController = rootParent as IScreenController;
            if (screenController == null)
                return false;
            if (mode == ExecutionMode.Synchro)
            {
                UIElement target = null;
                if (Entity != null)
                    target = Entity.ContainedObject as UIElement;

                if (IsRelative && target != null && !Double.IsNaN(X) && !Double.IsNaN(Y))
                {
                    var parentViewer = target.FindParent<ScreenViewer>();
                    if (parentViewer != null)
                    {
                        var ptScreen = parentViewer.PointToScreen(new Point(X * parentViewer.ZoomLevelX, Y * parentViewer.ZoomLevelY));
                        X = ptScreen.X;
                        Y = ptScreen.Y;
                    }
                }

                var viewer = new GeoViewer(this, screenController,
                    activeView as ScreenTransitionViewer, rootParent, bIsModal:true);
                viewer.Dispatcher.BeginInvokeAsynchronouslyInBackground(() =>
                {
                    viewer.ZoomTo(openingArea, openingParameters);
                    viewer.MapScrollingZooming(enableZoomingScrolling);
                    viewer.SetOptions(showMiniMap, showNextButton);
                });

                var wnd = new DevExpress.Xpf.Core.DXWindow
                {
                    Owner = (activeView == null ? null : activeView.FindParent<Window>() ?? Application.Current.MainWindow),
                    //WindowStyle = WindowStyle.ToolWindow,
                    //WindowState = WindowState.Normal,
                    //SizeToContent = SizeToContent.WidthAndHeight,
                    ////Topmost = true,
                    //ShowInTaskbar = false,
                    Content = viewer,
                    BorderEffect = DevExpress.Xpf.Core.BorderEffect.None,
                    WindowStyle = WindowStyle.None
                };

                if (!Double.IsNaN(X) && !Double.IsNaN(Y))
                {
                    wnd.Left = X;
                    wnd.Top = Y;
                }

                if (!Double.IsNaN(X) && !Double.IsNaN(Y))
                {
                    wnd.Left = X;
                    wnd.Top = Y;
                }

                if (Properties.Settings.Default.Aliased)
                    RenderOptions.SetEdgeMode(wnd, EdgeMode.Aliased);

                ResourceDictionaryExtensions.AddCommonResources(wnd);

                var themeName = String.Empty;
                if (mapMainTheme.ContainsKey(rootParent))
                    themeName = mapMainTheme[rootParent];
                ThemeHelper.SetTheme(wnd, themeName);
                wnd.ShowDialog();
                wnd.Close();

                viewer.Dispose();
            }
            else if (activeView is ScreenTransitionViewer)
            {
                var containerView = activeView as ScreenTransitionViewer;
                containerView.OpenMap(rootParent, openingArea, enableZoomingScrolling,
                        showMiniMap, showNextButton, openingParameters);
            }

            return true;
        }
#endif

        public void Execute(Uri uri, IDocument parent, ExecutionMode mode, Object Context)
        {
#if !NET_STANDARD
            UserControl activeView = null;
            var rootParent = parent.Parent;
            if (rootParent == null)
                rootParent = parent;

            var p = rootParent;
            var memParent = p;
            while (p != null)
            {
                p = p.Parent;
                if (p == null)
                    break;
                memParent = p;
            }
            rootParent = memParent;

            var docParent = parent.Parent;
            if (docParent == null || parent.IsRoot)
                docParent = parent;

            p = docParent;
            memParent = p;
            while (p != null)
            {
                p = p.Parent;
                if (p == null || p.IsRoot)
                    break;
                memParent = p;
            }
            docParent = memParent;

#if !WINDOWS_UWP
            if (mapMainWindows.ContainsKey(rootParent))
            {
                if (!mapMainWindows[rootParent].Dispatcher.CheckAccess())

                {
                    mapMainWindows[rootParent].Dispatcher.BeginInvoke((Action)(() => Execute(uri, parent, mode, Context)));
                    return;
                }
            }
            if (rootParent == parent)
            {
                if (mapMainWindows.ContainsKey(parent))
                    activeView = mapMainWindows[parent];
                else
                    activeView = parent.ActiveView;
            }
            else
            {
                if (parent.ActiveView != null && parent.ActiveView.Dispatcher.CheckAccess())
                    activeView = parent.ActiveView;
                else if (mapMainWindows.ContainsKey(rootParent))
                    activeView = mapMainWindows[rootParent];
            }

            if (Properties.Settings.Default.EnablePerformancesLog && activeView is ScreenTransitionViewer)
            {
                var text = String.Format(Properties.Resources.ScreenLoadingTraceMessage, parent.MakeRelativeUri(uri).GetPathString(), "{0}");
                performanceWatcher = new StopWatcher(text);
            }
#else
            RunOnUIThread.RunIfRequired(() =>
            {
#endif
                if (uri != null && parent != null)
                {
                    uri = parent.MakeAbosoluteUri(uri);
                    docParent = docParent.UpdateParentFromUri(uri);
                }
                if (uri != null && !ScreenDocument.ExistFile(uri.GetPathString()
#if !WINDOWS_UWP
                , parent.fileSystemProviderBase
#endif
                ))
                {
                    if (UIInterface != null)
                    {
                        UIInterface.ShowError(String.Format(Properties.Resources.DocNotFound,
                            GetDocumentTitle(uri)));
                    }
                    return;
                }

                String parameterFile = null;
                int nRequestedMonitor = -1;
                if (Context is IDictionary<String, Object>)
                {
                    var map = Context as IDictionary<String, Object>;
                    if (map.ContainsKey("ParameterFile"))
                    { 
                        parameterFile = Convert.ToString(map["ParameterFile"]);
                        if (parameterFile == "*")
                        {
                            parameterFile = null;
                            if (map.ContainsKey("Entity"))
                            {
                                IEntityReference entity = map["Entity"] as IEntityReference;
                                if (entity != null)
                                {
                                    var target = entity.ContainedObject as UIElement;
                                    if (target != null)
                                    {
                                        var document = ScreenSettings.ScreenDocument.GetScreenDocument(target) as ScreenDocument;
                                        if (document != null)
                                            parameterFile = document.ParameterFile;
                                    }
                                }
                            }
                        }
                    }
#if !WINDOWS_UWP
                if (map.ContainsKey("Monitor"))
                    nRequestedMonitor = Convert.ToInt32(map["Monitor"]);

                if (map.ContainsKey("Call3DControl") && map.ContainsKey("ViewName"))
                {
                    var call3DControl = Convert.ToString(map["Call3DControl"]);
                    var viewName = Convert.ToString(map["ViewName"]);

                    if (map.ContainsKey("Entity"))
                    {
                        IEntityReference entity = map["Entity"] as IEntityReference;
                        if (entity != null)
                        {
                            var target = entity.ContainedObject as UIElement;
                            if (target != null)
                            {
                                var view = target.FindParent<ScreenViewer>();
                                if (view != null)
                                {
                                    view.Activate3DCameraView(call3DControl, viewName);
                                    return;
                                }
                            }
                        }
                    }

                    if (activeView is ScreenViewer)
                    {
                        var view = activeView as ScreenViewer;
                        view.Activate3DCameraView(call3DControl, viewName);
                    }
                    else
                    {
                        if (mapMainWindows.ContainsKey(rootParent))
                        {
                            var view = mapMainWindows[rootParent].GetSelectedScreenViewer();
                            if (view != null)
                                view.Activate3DCameraView(call3DControl, viewName);
                        }
                    }

                    foreach(var list in mapDialogsOpen.Values)
                    {
                        foreach(var wnd in list)
                        {
                            var screenviewer = wnd.Content as ScreenViewer;
                            if (screenviewer != null)
                                screenviewer.Activate3DCameraView(call3DControl, viewName);
                        }
                    }
                    return;
                }
#endif
                }

                var bOpenAsFrame = (mode == ExecutionMode.Normal || mode == ExecutionMode.Shared) && nRequestedMonitor > 0 && nRequestedMonitor <= System.Windows.Forms.SystemInformation.MonitorCount && activeView is ScreenTransitionViewer && uri != null;
                Window frameWindow = null;

                if (mode == ExecutionMode.Stop)
                {
                    if (uri != null && mapDialogsOpen.ContainsKey(uri) && mapDialogsOpen[uri].Count > 0)
                    {
#if !WINDOWS_UWP
                        var wnd = mapDialogsOpen[uri].Last();
                        wnd.Hide();
                        wnd.Dispatcher.BeginInvokeAsynchronously(() => wnd.Close());
#else
                        mapDialogsOpen[uri].Last().Hide();
#endif
                    }
                    else
                    {
                        if (activeView == null)
                        {
#if !WINDOWS_UWP
                        if (mapMainWindows.ContainsKey(rootParent))
#endif
                            {
                                if (listPrevious.Count > 1)
                                {
                                    var nextUri = listPrevious.Last().Item1;
                                    listPrevious.Remove(listPrevious.Last());
                                    if (listNexts.Contains(nextUri))
                                        listNexts.Remove(nextUri);
                                    listNexts.Add(nextUri);
                                    var prevUri = (from pr in listPrevious where pr.Item2 == null select pr).LastOrDefault()?.Item1 ?? listPrevious.Last().Item1;
                                    Object ctx = null;
                                    if (mapUriContext.ContainsKey(prevUri))
                                        ctx = mapUriContext[prevUri];

                                    bExecutingPrevious = true;
                                    try
                                    {
                                        Execute(prevUri, parent, ExecutionMode.Normal, ctx);
                                    }
                                    finally
                                    {
                                        bExecutingPrevious = false;
                                    }
                                }
#if !WINDOWS_UWP
                            else if (uri == null)
                                mapMainWindows[rootParent].ActivateHome();
                            else
                                mapMainWindows[rootParent].ActivatePrevious();
#endif
                            }
                            return;
                        }

                        UIElement target = null;
                        var map = Context as IDictionary<String, Object>;
                        if (map != null && map.ContainsKey("Entity"))
                        {
                            IEntityReference entity = map["Entity"] as IEntityReference;
                            if (entity != null)
                            {
                                target = entity.ContainedObject as UIElement;
                            }
                        }
                        if (target != null)
                        {
                            var childWnd = target.FindParent<Window>();
                            if (childWnd != null)
                            {
                                var found = (from c in mapDialogsOpen.Values where c.Contains(childWnd) select childWnd).ToList();
                                if (found.Count > 0)
                                {
#if !WINDOWS_UWP
                                    childWnd.Hide();
                                    childWnd.Dispatcher.BeginInvokeAsynchronously(() => childWnd.Close());
#else
                                    childWnd.Hide();
#endif
                                    return;
                                }
                            }
                        }
#if !WINDOWS_UWP
                        var wnd = activeView.FindParent<Window>();
#else
                        var wnd = activeView.FindParent<ContentDialog>();
#endif
                        if (wnd != null)
                        {
                            var found = (from c in mapDialogsOpen.Values where c.Contains(wnd) select wnd).ToList();
                            if (found.Count > 0)
                            {
#if !WINDOWS_UWP
                                wnd.Hide();
                                wnd.Dispatcher.BeginInvokeAsynchronously(() => wnd.Close());
#else
                                wnd.Hide();
#endif
                                return;
                            }

                            if (map != null && map.ContainsKey("Entity"))
                            {
                                IEntityReference entity = map["Entity"] as IEntityReference;
                                if (entity != null)
                                {
                                    target = entity.ContainedObject as UIElement;
                                }
                            }
                            if (target != null)
                            {
#if !WINDOWS_UWP
                                var childWnd = target.FindParent<Window>();
#else
                                var childWnd = target.FindParent<ContentDialog>();
#endif
                                if (childWnd != null && childWnd != wnd)
                                {
#if !WINDOWS_UWP
                                    childWnd.Close();
#else
                                    childWnd.Hide();
#endif
                                    return;
                                }
                            }
                        }

#if !WINDOWS_UWP
                    //if (activeView is ScreensTileViewer)
                    //{
                    //    var containerView = activeView as ScreensTileViewer;
                    //    if (!containerView.CloseAllPopup(target))
                    //    {
                    //        if (uri == null)
                    //            containerView.CloseActivate();
                    //        else if (containerView.ListScreens.Contains(uri))
                    //            containerView.ListScreens.Remove(uri);
                    //    }
                    //}
                    //else 
                    if (activeView is ScreenTransitionViewer)
                    {
                        var containerView = activeView as ScreenTransitionViewer;
                        if (!containerView.CloseAllPopup(target))
                        {
                            if (uri == null)
                                containerView.ActivateHome();
                            else if(containerView.ListScreens.Contains(uri.GetPathString()))
                            {
                                if (listPrevious.Count > 1)
                                {
                                    var nextUri = listPrevious.Last().Item1;
                                    listPrevious.Remove(listPrevious.Last());
                                    if (listNexts.Contains(nextUri))
                                        listNexts.Remove(nextUri);
                                    listNexts.Add(nextUri);
                                    var prevUri = (from pr in listPrevious where pr.Item2 == wnd select pr).LastOrDefault()?.Item1 ?? listPrevious.Last().Item1;
                                    Object ctx = null;
                                    if (mapUriContext.ContainsKey(prevUri))
                                        ctx = mapUriContext[prevUri];

                                    bExecutingPrevious = true;
                                    try
                                    {
                                        Execute(prevUri, parent, ExecutionMode.Normal, ctx);
                                    }
                                    finally
                                    {
                                        bExecutingPrevious = false;
                                    }
                                }
                                else
                                    containerView.ActivatePrevious();
                            }
                        }
                    }
                    else if (mapMainWindows.ContainsKey(rootParent))
                    {
                        if (uri == null)
                            mapMainWindows[rootParent].ActivateHome();
                        else
                            mapMainWindows[rootParent].ActivatePrevious();
                    }
#endif
                    }
                }
#if !WINDOWS_UWP
            //else if (mode == ExecutionMode.Shared)
            //{
            //    if (uri == null)
            //        return;

            //    if (activeView is ScreensTileViewer)
            //    {
            //        var containerView = activeView as ScreensTileViewer;
            //        containerView.OpenOrActivate(uri, parameterFile, parent: docParent);
            //    }
            //    else if (activeView is ScreenTransitionViewer)
            //    {
            //        var containerView = activeView as ScreenTransitionViewer;
            //        containerView.OpenOrActivate(uri, true, parameterFile, parent: docParent);
            //    }
            //    else if (mapMainWindows.ContainsKey(rootParent))
            //    {
            //        mapMainWindows[rootParent].OpenOrActivate(uri, true, parameterFile, nRequestedMonitor: nRequestedMonitor, parent: docParent);
            //    }
            //}
#endif      
                else if (mode == ExecutionMode.Synchro || bOpenAsFrame)
                {
                    if (uri == null)
                        return;

                    bool bSynchroFrame = false;
                    UIElement target = null;
                    var map = Context as IDictionary<String, Object>;
                    if (map.ContainsKey("Entity"))
                    {
                        IEntityReference entity = map["Entity"] as IEntityReference;
                        if (entity != null)
                            target = entity.ContainedObject as UIElement;
                    }
                    if (Context is IDictionary<String, Object>)
                    {
                        bSynchroFrame = map.ContainsKey("SynchroFrame") && (Convert.ToBoolean(map["SynchroFrame"])) == true;
#if !WINDOWS_UWP
                    bool bSynchroPopup = map.ContainsKey("SynchroPopup") && (Convert.ToBoolean(map["SynchroPopup"])) == true;
                    if (bSynchroPopup)
                    {
                        double dX = Double.NaN;
                        if (map.ContainsKey("X"))
                            dX = Convert.ToDouble(map["X"]);
                        double dY = Double.NaN;
                        if (map.ContainsKey("Y"))
                            dY = Convert.ToDouble(map["Y"]);
                        double dWidth = Double.NaN;
                        if (map.ContainsKey("Width"))
                            dWidth = Convert.ToDouble(map["Width"]);
                        double dHeight = Double.NaN;
                        if (map.ContainsKey("Height"))
                            dHeight = Convert.ToDouble(map["Height"]);
                        bool bOffsetRelativeToScreen = false;
                        if (map.ContainsKey("OffsetRelativeToScreen"))
                            bOffsetRelativeToScreen = Convert.ToBoolean(map["OffsetRelativeToScreen"]);

                        if (target == null)
                            target = System.Windows.Input.Keyboard.FocusedElement as UIElement;

                        if (bOffsetRelativeToScreen)
                        {
                            var parentScreen = target.FindParent<ScreenViewer>();
                            if (parentScreen != null) { 
                                var parentViewerBounds = parentScreen.PointToScreen(new Point(0, 0));
                                dX += parentViewerBounds.X;
                                dY += parentViewerBounds.Y;
                            }
                        }

                        //if (activeView is ScreensTileViewer)
                        //{
                        //    var containerView = activeView as ScreensTileViewer;
                        //    if (target == null)
                        //        target = containerView;
                        //    containerView.OpenPopup(uri, target, parameterFile, dX, dY, dWidth, dHeight);
                        //}
                        //else 
                        if (activeView is ScreenTransitionViewer)
                        {
                            var containerView = activeView as ScreenTransitionViewer;
                            if (target == null)
                                target = containerView;
                            containerView.OpenPopup(uri, target, parameterFile, dX, dY, dWidth, dHeight, bOffsetRelativeToScreen);
                        }
                        else if (mapMainWindows.ContainsKey(rootParent))
                        {
                            if (target == null)
                                target = mapMainWindows[rootParent];
                            mapMainWindows[rootParent].OpenPopup(uri, target, parameterFile, dX, dY, dWidth, dHeight, bOffsetRelativeToScreen);
                        }
                        return;
                    }
#endif
                    }

                    bool bFoundToActivate = false;
                    if (mapDialogsOpen.ContainsKey(uri) && mapDialogsOpen[uri].Count > 0)
                    {
                        var found = (from c in mapDialogsOpen[uri]
                                     where c.Content is ScreenViewer && (c.Content as ScreenViewer).GetParameterFile() == parameterFile
                                     select c).ToList();
                        if (found.Count > 0)
                        {
#if !WINDOWS_UWP
                    found[0].Activate();
#endif
                            bFoundToActivate = true;
                        }
                    }

                    if (!bFoundToActivate)
                    {
                        bool bIsRelative = map.ContainsKey("IsRelative") && (Convert.ToBoolean(map["IsRelative"])) == true;
                        double dX = Double.NaN;
                        if (map.ContainsKey("X"))
                            dX = Convert.ToDouble(map["X"]);
                        double dY = Double.NaN;
                        if (map.ContainsKey("Y"))
                            dY = Convert.ToDouble(map["Y"]);
                        double dWidth = Double.NaN;
                        if (map.ContainsKey("Width"))
                            dWidth = Convert.ToDouble(map["Width"]);
                        double dHeight = Double.NaN;
                        if (map.ContainsKey("Height"))
                            dHeight = Convert.ToDouble(map["Height"]);
#if !WINDOWS_UWP
                    bool bUsesRelativeCoords = false;
                    bool bIsCenterOwner = false;
                    bool isPromptPadRequest = map.ContainsKey("IsPromptPadRequest") && (Convert.ToBoolean(map["IsPromptPadRequest"])) == true;
                    if (isPromptPadRequest)
                    {
                         if (target == null)
                            target = System.Windows.Input.Keyboard.FocusedElement as UIElement;

                        bIsCenterOwner = Double.IsNaN(dX) && Double.IsNaN(dY);
                    }

                    if(target != null)
                    {
                        var parentViewer = target.FindParent<ScreenViewer>();
                        if (parentViewer != null)
                        {
                            if (nRequestedMonitor == -1)
                            {
                                if (!String.IsNullOrEmpty(Properties.Settings.Default.MultiMonitorMap))
                                    nRequestedMonitor = parentViewer.UntranslatedMonitorNumber;
                                else
                                    nRequestedMonitor = parentViewer.MonitorNumber;
                            }

                            if (!Double.IsNaN(dX) && !Double.IsNaN(dY))
                            {
                                if (bIsRelative)
                                {
                                    bUsesRelativeCoords = true;
                                    if (parentViewer != null)
                                    {
                                        var ptScreen = parentViewer.PointToScreen(new Point(dX * parentViewer.ZoomLevelX, dY * parentViewer.ZoomLevelY));
                                        dX = ptScreen.X;
                                        dY = ptScreen.Y;
                                    }
                                }
                                else if (isPromptPadRequest)
                                {
                                    var ptScreen = parentViewer.PointToScreen(new Point(0, 0));
                                    var screen = System.Windows.Forms.Screen.FromPoint(new System.Drawing.Point((int)ptScreen.X, (int)ptScreen.Y));
                                    var workingArea = screen.WorkingArea;

                                    dY = workingArea.Top + dY;
                                    dX = workingArea.Left + dX;
                                }
                            }
                            else if (isPromptPadRequest)
                            {
                                var ptScreen = parentViewer.PointToScreen(new Point(0, 0));
                                var screen = System.Windows.Forms.Screen.FromPoint(new System.Drawing.Point((int)ptScreen.X, (int)ptScreen.Y));
                                var workingArea = screen.WorkingArea;

                                dY = workingArea.Top;
                                dX = workingArea.Left;
                            }
                        }
                    }

#endif
                    int autoCloseSec = 0;
                        if (map.ContainsKey("AutoCloseSecs"))
                            autoCloseSec = Convert.ToInt32(map["AutoCloseSecs"]);

                        var viewer = new ScreenViewer(this, uri, docParent, activeView,
                            isModal: !bOpenAsFrame, parameterFile: parameterFile, nMonitor: nRequestedMonitor,
                            customleft: dX, customtop: dY, autocloseSeconds: autoCloseSec, customWidth: dWidth, customHeight: dHeight, bIsrelative: bUsesRelativeCoords, bIscenterOwner: bIsCenterOwner);
#if !WINDOWS_UWP
                    if (Properties.Settings.Default.UseThemedWindow)
                        frameWindow = new DevExpress.Xpf.Core.ThemedWindow() { Width = viewer.Document.Width, Height = viewer.Document.Height }; 
                    else
                        frameWindow = new DevExpress.Xpf.Core.DXWindow() { BorderEffect = DevExpress.Xpf.Core.BorderEffect.Default };
                    frameWindow.Content = viewer;
                    frameWindow.AllowsTransparency = viewer.Document != null && viewer.Document.WindowOpacity < 1;
                    frameWindow.WindowStyle = bOpenAsFrame ? WindowStyle.None : WindowStyle.ToolWindow;

                    if (!bOpenAsFrame)
                    {
                        frameWindow.Owner = (activeView == null ? null : activeView.FindParent<Window>() ?? Application.Current.MainWindow);
                        //WindowStyle = WindowStyle.ToolWindow,
                        //WindowState = WindowState.Normal,
                        //SizeToContent = SizeToContent.WidthAndHeight,
                        ////Topmost = true,
                        frameWindow.ShowInTaskbar = viewer.Document != null && viewer.Document.ShowInTaskbar;
                    }

                    if (Properties.Settings.Default.Aliased)
                        RenderOptions.SetEdgeMode(frameWindow, EdgeMode.Aliased);
#else
                        var wnd = new ContentDialog()
                        {
                            Content = viewer,
                            FullSizeDesired = true
                        };
#endif

#if !WINDOWS_UWP
                    ResourceDictionaryExtensions.AddCommonResources(frameWindow);
#endif
                        LoadDefaultStyle(frameWindow, rootParent);

#if !WINDOWS_UWP
                    var themeName = String.Empty;
                    if (mapMainTheme.ContainsKey(rootParent))
                        themeName = mapMainTheme[rootParent];
                    ThemeHelper.SetTheme(frameWindow, themeName);
                    // SkinStorage.SetEnableOptimization(wnd, true);
#endif
                        if (!mapDialogsOpen.ContainsKey(uri))
#if !WINDOWS_UWP
                        mapDialogsOpen.Add(uri, new List<Window>());
#else
                            mapDialogsOpen.Add(uri, new List<ContentDialog>());
#endif
                        mapDialogsOpen[uri].Add(frameWindow);
#if !WINDOWS_UWP
                    if (bSynchroFrame || bOpenAsFrame)
                    {
                        if (!mapFrameWindows.ContainsKey(rootParent))
                            mapFrameWindows.Add(rootParent, new List<Window>());

                        frameWindow.Show();
                        if (bOpenAsFrame)
                            viewer.UpdateWindowPosition(frameWindow);
                        frameWindow.Closed += (o, e) =>
                            {
                                viewer.Dispose();
                                if (mapDialogsOpen.ContainsKey(uri))
                                    mapDialogsOpen[uri].Remove(frameWindow);
                                (from item in listPrevious where item.Item2 == frameWindow select item).ToList().ForEach(t => listPrevious.Remove(t));
                                if (mapMainWindows.ContainsKey(rootParent) && mapFrameWindows[rootParent].Contains(frameWindow))
                                    mapFrameWindows[rootParent].Remove(frameWindow);
                                if (frameWindow.Owner != null)
                                {
                                    frameWindow.Owner.Focusable = true;
                                    frameWindow.Owner.Focus();
                                }
                            };
                    }
                    else
#endif
                        {
#if !WINDOWS_UWP
                        if (!mapModalWindows.ContainsKey(rootParent))
                            mapModalWindows.Add(rootParent, new List<Window>());
                        if (mapMainWindows.ContainsKey(rootParent) && !mapModalWindows[rootParent].Contains(frameWindow))
                            mapModalWindows[rootParent].Add(frameWindow);
#endif
                        frameWindow.Closed += (o, e) =>
                        {
#if WINDOWS_UWP
                            viewer.Dispose();
#endif
                            if (mapDialogsOpen.ContainsKey(uri))
                                mapDialogsOpen[uri].Remove(frameWindow);
                            (from item in listPrevious where item.Item2 == frameWindow select item).ToList().ForEach(t => listPrevious.Remove(t));
#if !WINDOWS_UWP
                            if (mapMainWindows.ContainsKey(rootParent) && mapModalWindows[rootParent].Contains(frameWindow))
                                mapModalWindows[rootParent].Remove(frameWindow);
                            if (mapModalWindows[rootParent].Count > 0)
                            {
                                mapModalWindows[rootParent].Last().Focusable = true;
                                mapModalWindows[rootParent].Last().Focus();
                            }
                            else
#endif
                            if (frameWindow.Owner != null)
                            {
                                frameWindow.Owner.Focusable = true;
                                frameWindow.Owner.Focus();
                            }
                        };
#if !WINDOWS_UWP
                        var result = frameWindow.ShowDialog();
                        frameWindow.Close();
                        viewer.Dispose();
#else
                        ShowDialog(wnd);
#endif
                    }
                    }
                }
                else
                {
#if !WINDOWS_UWP
                    //if (activeView is ScreensTileViewer)
                    //{
                    //    var containerView = activeView as ScreensTileViewer;
                    //    if (uri != null)
                    //    {
                    //        containerView.OpenOrActivateNormal(uri, parameterFile, nRequestedMonitor: nRequestedMonitor, parent: docParent);
                    //    }
                    //}
                    //else 
                    if (activeView is ScreenTransitionViewer)
                    {
                        var containerView = activeView as ScreenTransitionViewer;
                        if (uri != null)
                        {
                            if (!bExecutingPrevious)
                            {
                                var tuple = FindTupleByUri(listPrevious, uri);
                                if (tuple != null)
                                    listPrevious.Remove(tuple);
                                listPrevious.Add(new Tuple<Uri, Window>(uri, null));
                                if (Context != null)
                                {
                                    if (mapUriContext.ContainsKey(uri))
                                        mapUriContext.Remove(uri);
                                    mapUriContext.Add(uri, Context);
                                    var map = Context as IDictionary<String, Object>;
                                    if (map != null && map.ContainsKey("Entity"))
                                        map.Remove("Entity");
                                }
                                else if (mapUriContext.ContainsKey(uri))
                                    mapUriContext.Remove(uri);
                            }

                            containerView.OpenOrActivate(uri, parameterFile: parameterFile, nRequestedMonitor: nRequestedMonitor, parent: docParent);
                        }
                        else
                        {
                            if (listPrevious.Count > 1)
                            {
                                var nextUri = listPrevious.Last().Item1;
                                listPrevious.Remove(listPrevious.Last());
                                if (listNexts.Contains(nextUri))
                                    listNexts.Remove(nextUri);
                                listNexts.Add(nextUri);
                                var prevUri = listPrevious.Last().Item1;
                                Object ctx = null;
                                if (mapUriContext.ContainsKey(prevUri))
                                    ctx = mapUriContext[prevUri];

                                bExecutingPrevious = true;
                                try
                                {
                                    Execute(prevUri, parent, ExecutionMode.Normal, ctx);
                                }
                                finally
                                {
                                    bExecutingPrevious = false;
                                }
                            }
                            else
                                containerView.ActivatePrevious();
                        }
                    }
                    else
#endif
                    {
                        if (!bExecutingPrevious && uri != null)
                        {
                            var tuple = FindTupleByUri(listPrevious, uri);
                            if (tuple != null)
                                listPrevious.Remove(tuple);
                            listPrevious.Add(new Tuple<Uri, Window>(uri, frameWindow));
                            if (Context != null)
                            {
                                if (mapUriContext.ContainsKey(uri))
                                    mapUriContext.Remove(uri);
                                mapUriContext.Add(uri, Context);
                                var map = Context as IDictionary<String, Object>;
                                if (map != null && map.ContainsKey("Entity"))
                                    map.Remove("Entity");
                            }
                            else if (mapUriContext.ContainsKey(uri))
                                mapUriContext.Remove(uri);
                        }
#if !WINDOWS_UWP
                        if (mapMainWindows.ContainsKey(rootParent))
                        {
                            if (uri != null)
                            {
                                mapMainWindows[rootParent].OpenOrActivate(uri, parameterFile: parameterFile, nRequestedMonitor: nRequestedMonitor, parent: docParent);
                            }
                            else
                                mapMainWindows[rootParent].ActivatePrevious();
                            return;
                        }

                        var controller = Context as IScreenController;
                        if (controller == null && uri == null)
                            return;

                        var setting = ConfigurationManager.AppSettings["ImproveWpfRenderingPerformance"];
                        if (setting == Boolean.TrueString)
                        {
                            Thread.CurrentThread.Priority = ThreadPriority.Highest;
                        }

                        var screenViewer = new ScreenTransitionViewer(this, parent, controller);
                        screenViewer.PrevGesture += (o, e) =>
                        {
                            if (listPrevious.Count > 1)
                            {
                                var nextUri = listPrevious.Last().Item1;
                                listPrevious.Remove(listPrevious.Last());
                                if (listNexts.Contains(nextUri))
                                    listNexts.Remove(nextUri);
                                listNexts.Add(nextUri);
                                var prevUri = listPrevious.Count > 0 ? listPrevious.Last().Item1 : screenViewer.ActivateOnLoad;
                                Object ctx = null;
                                if (mapUriContext.ContainsKey(prevUri))
                                    ctx = mapUriContext[prevUri];

                                bExecutingPrevious = true;
                                try
                                {
                                    Execute(prevUri, parent, ExecutionMode.Normal, ctx);
                                }
                                finally
                                {
                                    bExecutingPrevious = false;
                                }
                            }
                            else
                                screenViewer.ActivatePrevious();
                        };
                        screenViewer.NextGesture += (o, e) =>
                        {
                            if (listNexts.Count > 0)
                            {
                                var prevUri = listNexts.Last();
                                listNexts.Remove(prevUri);
                                var tuple = FindTupleByUri(listPrevious, screenViewer.ActivateOnLoad);
                                if (tuple != null)
                                    listPrevious.Remove(tuple);
                                listPrevious.Add(new Tuple<Uri, Window>(screenViewer.ActivateOnLoad, ((ScreenTransitionViewer)o).FindParent<Window>()));
                                Object ctx = null;
                                if (mapUriContext.ContainsKey(prevUri))
                                    ctx = mapUriContext[prevUri];

                                Execute(prevUri, parent, ExecutionMode.Normal, ctx);
                            }
                            else
                                screenViewer.ActivateNext();
                        };

                        if (controller == null && uri != null)
                            screenViewer.ListScreens.Add(uri.GetPathString());

                        var top = controller.GetTopScreen();
                        var left = controller.GetLeftScreen();
                        var right = controller.GetRightScreen();
                        var bottom = controller.GetBottomScreen();
                        screenViewer.SetLayoutUris(top, left, right, bottom, controller.GetLayoutScreenOrder());

                        screenViewer.SetGadgetsUris(controller.GetGadgetScreens());
                        screenViewer.SetAutoLoadsUris(controller.GetAutoLoadScreens());

                        top = controller.GetAppBarTopScreen();
                        bottom = controller.GetAppBarBottomScreen();
                        left = controller.GetAppBarLeftScreen();
                        right = controller.GetAppBarRightScreen();
                        screenViewer.SetAppBarsUris(top, bottom, left, right);

                        listPrevious.Clear();
                        listNexts.Clear();
                        mapUriContext.Clear();

                        Window wnd = null;
                    /*
                        switch(controller.GetTouchType())
                        {
                            case TouchType.None:
                            */

                                var speechWindow = new SpeechWindow()
                                {
                                    EnableSpeechRecognition = controller.GetHasSpeechEnabled(),
                                    SpeechConfidenceLevel = controller.GetSpeechConfidenceLevel(),
                                    SpeechCulture = controller.GetSpeechCulture(),
                                    DefaultSpeechCommands = controller.GetDefaultSpeechCommands(),
                                    Visibility = Visibility.Hidden,
                                    WindowStyle = WindowStyle.None
                                };
                                wnd = speechWindow;
                                speechWindow.content.Content = screenViewer;

                                speechWindow.Home += (o1, e1) =>
                                {
                                    screenViewer.ActivateHome();
                                };
                                speechWindow.MoveBack += (o1, e1) =>
                                {
                                    if (listPrevious.Count > 0)
                                    {
                                        var prevUri = listPrevious.Last().Item1;
                                        Object ctx = null;
                                        if (mapUriContext.ContainsKey(prevUri))
                                            ctx = mapUriContext[prevUri];
                                        listPrevious.Remove(listPrevious.Last());
                                        if (listNexts.Contains(prevUri))
                                            listNexts.Remove(prevUri);
                                        listNexts.Add(prevUri);

                                        Execute(prevUri, parent, mode, ctx);
                                    }
                                    else
                                        screenViewer.ActivatePrev();
                                };
                                speechWindow.MoveForward += (o1, e1) =>
                                {
                                    if (listNexts.Count > 0)
                                    {
                                        var prevUri = listNexts.Last();
                                        Object ctx = null;
                                        if (mapUriContext.ContainsKey(prevUri))
                                            ctx = mapUriContext[prevUri];
                                        listNexts.Remove(prevUri);
                                        var tuple = FindTupleByUri(listPrevious, uri);
                                        if (tuple != null)
                                            listPrevious.Remove(tuple);
                                        listPrevious.Add(new Tuple<Uri, Window>(uri, (SpeechWindow)o1));

                                        Execute(prevUri, parent, mode, ctx);
                                    }
                                    else
                                        screenViewer.ActivateNext();
                                };
                                speechWindow.ZoomIn += (o1, e1) =>
                                {
                                    screenViewer.ZoomIn();
                                };
                                speechWindow.ZoomOut += (o1, e1) =>
                                {
                                    screenViewer.ZoomOut();
                                };
                                speechWindow.Reset += (o1, e1) =>
                                {
                                    screenViewer.Reset();
                                };
                                speechWindow.SpeechCommand += (o1, e1) =>
                                {
                                    screenViewer.ExecuteCommandName(e1.Command, speechWindow.SpeechCulture);
                                };
                                speechWindow.SwipeUp += (o1, e1) =>
                                {
                                    screenViewer.OpenTopBar();
                                };
                                speechWindow.SwipeDown += (o1, e1) =>
                                {
                                    screenViewer.OpenBottomBar();
                                };
                                speechWindow.SwipeLeft += (o1, e1) =>
                                {
                                    screenViewer.OpenLeftBar();
                                };
                                speechWindow.SwipeRight += (o1, e1) =>
                                {
                                    screenViewer.OpenRightBar();
                                };

                                screenViewer.ActiveContentChanged += (o1, e1) =>
                                {
                                    screenViewer.Dispatcher.BeginInvokeAsynchronouslyInBackground(() =>
                                    {
                                        var defCommands = controller.GetDefaultSpeechCommands();
                                        speechWindow.DefaultSpeechCommands = defCommands;

                                        String culture = null;
                                        if (StringEditor != null)
                                        {
                                            culture = StringEditor.GetActiveCulture(rootParent, false);
                                            var mapculture = StringEditor.GetListStringForCulture(rootParent, culture);
                                            if (mapculture != null)
                                            {
                                                if (defCommands != null)
                                                {
                                                    var listCommandTranslated = new List<String>();
                                                    defCommands.ForEach(s =>
                                                    {
                                                        var news = s;
                                                        if (mapculture.ContainsKey(s))
                                                            news = mapculture[s];
                                                        if (!listCommandTranslated.Contains(news))
                                                            listCommandTranslated.Add(news);
                                                    });

                                                    speechWindow.DefaultSpeechCommands = listCommandTranslated;
                                                }
                                            }
                                        }
                                        var list = screenViewer.GetActiveListCommands(culture);
                                        speechWindow.LoadDynamicGrammar(list, culture);
                                    });
                                };
                    /*
                                break;
                            case TouchType.Surface:
                                wnd = new SurfaceWindow() { Visibility = Visibility.Hidden, WindowStyle = WindowStyle.None }; 
                                wnd.Content = screenViewer;                                
                                break;
                            case TouchType.Kinect:
                                try
                                {
                                    var kwnd = new KinectControls.KinectWindow() 
                                    { 
                                        EnableSpeechRecognition = controller.GetHasSpeechEnabled(), 
                                        SpeechConfidenceLevel = controller.GetSpeechConfidenceLevel(),
                                        Visibility = Visibility.Hidden,
                                        WindowStyle = WindowStyle.None
                                    };

                                    kwnd.ReadyForContent += (o, e) =>
                                        {
                                            kwnd.SetContent(screenViewer);

                                            kwnd.Home += (o1, e1) =>
                                                {
                                                    screenViewer.ActivateHome();
                                                };
                                            kwnd.MoveBack += (o1, e1) =>
                                                {
                                                    if (listPrevious.Count > 0)
                                                    {
                                                        var prevUri = listPrevious.Last();
                                                        Object ctx = null;
                                                        if (mapUriContext.ContainsKey(prevUri))
                                                            ctx = mapUriContext[prevUri];
                                                        listPrevious.Remove(prevUri);
                                                        if (listNexts.Contains(prevUri))
                                                            listNexts.Remove(prevUri);
                                                        listNexts.Add(prevUri);

                                                        Execute(prevUri, parent, mode, ctx);
                                                    }
                                                    else
                                                        screenViewer.ActivatePrev();
                                                };
                                            kwnd.MoveForward += (o1, e1) =>
                                                {
                                                    if (listNexts.Count > 0)
                                                    {
                                                        var prevUri = listNexts.Last();
                                                        Object ctx = null;
                                                        if (mapUriContext.ContainsKey(prevUri))
                                                            ctx = mapUriContext[prevUri];
                                                        listNexts.Remove(prevUri);
                                                        if (listPrevious.Contains(prevUri))
                                                            listPrevious.Remove(prevUri);
                                                        listPrevious.Add(prevUri);

                                                        Execute(prevUri, parent, mode, ctx);
                                                    }
                                                    else
                                                        screenViewer.ActivateNext();
                                                };
                                            kwnd.ZoomIn += (o1, e1) =>
                                            {
                                                screenViewer.ZoomIn();
                                            };
                                            kwnd.ZoomOut += (o1, e1) =>
                                            {
                                                screenViewer.ZoomOut();
                                            };
                                            kwnd.Reset += (o1, e1) =>
                                            {
                                                screenViewer.Reset();
                                            };
                                            kwnd.SpeechCommand += (o1, e1) =>
                                            {
                                                screenViewer.ExecuteCommandName(e1.Command);
                                            };

                                            screenViewer.ActiveContentChanged += (o1, e1) =>
                                                {
                                                    screenViewer.Dispatcher.BeginInvokeAsynchronouslyInBackground(() =>
                                                        {
                                                            var list = screenViewer.GetActiveListCommands();
                                                            kwnd.LoadDynamicGrammar(list);
                                                        });
                                                };
                                        };
                                    wnd = kwnd;
                                }
                                catch (Exception ex)
                                {
                                    UIInterface.ShowError(String.Format(Properties.Resources.KinectNotFound,
                                        ex.Message));
                                
                                    wnd = new Window();
                                    wnd.Content = screenViewer;                                                                
                                }
                                break;
                        }
                        */

                        if (Properties.Settings.Default.Aliased)
                            RenderOptions.SetEdgeMode(wnd, EdgeMode.Aliased);

                        SysVariables.SysVariables.GetSysVariables().UpdateSysVariable(SysVariables.SysNames.ActiveScreen, String.Empty);
                        System.Windows.Input.Mouse.AddPreviewMouseMoveHandler(wnd, OnMouseMoved);

                    // LayoutHelper.HasTouchInput() ? new SurfaceWindow() : new SurfaceWindow();
                        wnd.ShowActivated = true;
                        wnd.WindowStyle = WindowStyle.None;
                        wnd.ShowInTaskbar = true;

                        System.Drawing.Rectangle workingArea = System.Windows.Forms.Screen.AllScreens[0].WorkingArea;
                        wnd.Left = workingArea.Left;
                        wnd.Top = workingArea.Top;
                        wnd.Width = workingArea.Width;
                        wnd.Height = workingArea.Height;
                        wnd.SizeToContent = SizeToContent.Manual;
                        wnd.WindowState = WindowState.Maximized;

                        if (Application.Current.Dispatcher.CheckAccess() && Application.Current.Windows.Count > 0)
                            wnd.Owner = Application.Current.Windows[0];
                        wnd.Title = String.Format(Properties.Resources.RuntimeTitle, parent.Title);

                        mapMainWindows.Add(rootParent, screenViewer);
                        ResourceDictionaryExtensions.AddCommonResources(wnd);
                        LoadDefaultStyle(wnd, rootParent);

                        var theme = controller.GetTheme();
                        var themeName = theme.ToString();
                        if (mapMainTheme.ContainsKey(rootParent))
                            mapMainTheme.Remove(rootParent);
                        mapMainTheme.Add(rootParent, themeName);
                        ThemeHelper.SetTheme(wnd, themeName);

                        screenViewer.Refresh();
                        wnd.Show();
                        wnd.Activate();
                        screenViewer.SetBusy(true);
                        wnd.Refresh();
                        screenViewer.Refresh();
                        screenViewer.SetBusy(false);

                        bool bClosing = false;
                        // wnd.Closing += (ob, eve) =>
                        wnd.Dispatcher.ShutdownStarted += (ob, eve) =>
                            {
                                System.Windows.Input.Mouse.RemovePreviewMouseMoveHandler(wnd, OnMouseMoved);
                                if (mousemoveTimer != null)
                                {
                                    mousemoveTimer.Stop();
                                    mousemoveTimer = null;
                                }

                                wnd.ShowInTaskbar = true;
                                if (mapFrameWindows.ContainsKey(rootParent))
                                {
                                    while (mapFrameWindows[rootParent].Count > 0)
                                        mapFrameWindows[rootParent].First().Close();
                                    mapFrameWindows.Remove(rootParent);
                                }
                                mapDialogsOpen.Clear();
                                listPrevious.Clear();
                                screenViewer.Dispose();
                            };
                        wnd.Dispatcher.ShutdownFinished += (ob, eve) =>
                            {
                                mapMainWindows.Remove(rootParent);

                                SystemSecurity.EnableCTRLALTDEL();
                                SystemSecurity.ShowStartMenu();

                                bClosing = true;
                                if (AuthenticationCredentialsProvider != null && !AuthenticationCredentialsProvider.IsFaulted(rootParent.Title))
                                    AuthenticationCredentialsProvider.Logout(rootParent.Title, false);
                            };
                        // if (Application.Current.Dispatcher.CheckAccess() && Application.Current.Windows.Count == 1)
                        wnd.Closed += (sender1, e1) =>
                            {
                                wnd.Dispatcher.InvokeShutdown();
                                wnd.Hide();
                            };

                        if (AuthenticationCredentialsProvider != null && UserEditor != null)
                        {
                            if (UserEditor.GetEnableUserManager(rootParent))
                            {
                                var systemRole = UserEditor.GetDesktopSystemRole(rootParent);
                                if (!String.IsNullOrEmpty(systemRole))
                                {
                                    SystemSecurity.KillCtrlAltDelete();
                                    SystemSecurity.KillStartMenu();
                                    AuthenticationCredentialsProvider.UserOnline += (ob, ev) =>
                                        {
                                            if (!String.IsNullOrEmpty(ev.User))
                                            {
                                                if (AuthenticationCredentialsProvider.IsFaulted(rootParent.Title) &&
                                                    !AuthenticationCredentialsProvider.IsCurrentUserInRole(rootParent.Title, systemRole))
                                                {
                                                    if (!bClosing)
                                                    {
                                                        SystemSecurity.KillCtrlAltDelete();
                                                        SystemSecurity.KillStartMenu();
                                                    }
                                                }
                                                else
                                                {
                                                    SystemSecurity.EnableCTRLALTDEL();
                                                    SystemSecurity.ShowStartMenu();
                                                }
                                            }
                                            else
                                            {
                                                if (!bClosing)
                                                {
                                                    SystemSecurity.KillCtrlAltDelete();
                                                    SystemSecurity.KillStartMenu();
                                                }
                                            }
                                        };

                                    wnd.Closing += (ob, eve) =>
                                    {
                                        if (!AuthenticationCredentialsProvider.IsFaulted(rootParent.Title))
                                        {
                                            if (!AuthenticationCredentialsProvider.IsCurrentUserInRole(rootParent.Title, systemRole))
                                            {
                                                AuthenticationCredentialsProvider.ValidateUsingCredentialsProvider(rootParent, rootParent.Title, systemRole, owner: wnd);
                                                eve.Cancel = true;
                                            }
                                            else
                                            {
                                                bClosing = true;
                                                AuthenticationCredentialsProvider.Logout(rootParent.Title);
                                            }
                                        }
                                    };

                                    /*
                                    Application.Current.SessionEnding += (ob, eve) =>
                                    {
                                        if (!AuthenticationCredentialsProvider.IsFaulted(rootParent.Title) &&
                                            !AuthenticationCredentialsProvider.IsCurrentUserInRole(rootParent.Title, systemRole))
                                        {
                                            AuthenticationCredentialsProvider.ValidateUsingCredentialsProvider(rootParent.Title, systemRole);
                                            eve.Cancel = true;
                                        }
                                    };
                                    */
                                }
                                else
                                {
                                    wnd.Closing += (ob, eve) =>
                                    {
                                        bClosing = true;
                                        if (!AuthenticationCredentialsProvider.IsFaulted(rootParent.Title))
                                            AuthenticationCredentialsProvider.Logout(rootParent.Title, false);
                                    };
                                }
                            }
                        }
#else
                        var controller = Context as IScreenController;
                        if (controller == null && uri == null)
                            return;
                        if (controller != null && uri == null)
                        {
                            uri = controller.GetStartupScreen();
                            uri = parent.MakeAbosoluteUri(uri);
                        }
                        var transitionControl = Window.Current.Content as Controls.TransitionControl;
                        if (transitionControl == null)
                        {
                            transitionControl = new Controls.TransitionControl();
                            Window.Current.Content = transitionControl;
                        }
                        var current = transitionControl.GetContent() as ScreenViewer;
                        if (current != null && current.CurrentUri == uri)
                            return;
                        var viewer = new ScreenViewer(this, uri, parent, activeView,
                            parameterFile: parameterFile);
                        transitionControl.SetContent(viewer);
                        if (current != null)
                            current.Dispose();
#endif
                    }
                }
#if WINDOWS_UWP
            });
#endif
#else
            throw new PlatformNotSupportedException("Only supported on Windows operating system.");
#endif
        }

#if !NET_STANDARD
        DispatcherTimer mousemoveTimer;
        void OnMouseMoved(object o, MouseEventArgs args)
        {
            SysVariables.SysVariables.GetSysVariables().UpdateSysVariable(SysVariables.SysNames.MouseMove, true);
            if (mousemoveTimer == null)
            {
                mousemoveTimer = new DispatcherTimer();
                mousemoveTimer.Tick += (ob, e) =>
                {
                    mousemoveTimer.Stop();
                    SysVariables.SysVariables.GetSysVariables().UpdateSysVariable(SysVariables.SysNames.MouseMove, false);
                };
                mousemoveTimer.Interval = TimeSpan.FromMilliseconds(100);
            }
            mousemoveTimer.Stop();
            mousemoveTimer.Start();
        }
#endif

#if !WINDOWS_UWP && !NET_STANDARD
        public void SaveAllChild(IDocument parent)
        {
            var list = (from c in mapActiveDocuments.Values// .AsParallel() 
                        where c.Parent == parent && c.ActiveView != null 
                        select c).ToList();

            list.ForEach(document =>
            {
                (document.ActiveView as ScreenEditorView).SaveCurrentDocument();
            });

            list = (from c in mapActiveDocuments.Values// .AsParallel()
                        where c.NeedsSave == true && c.ActiveView == null
                        select c).ToList();
            list.ForEach(doc =>
            {
                var res = UIInterface.ShowYesNo(String.Format(Properties.Resources.SaveDoc,
                    doc.Title), CustomDialogIcons.Question);
                if (res == CustomDialogResults.Yes)
                {
                    if (doc.SaveToFile())
                        doc.NeedsSave = false;
                }
            });
        }

        public bool CloseAllChild(IDocument parent, bool bParentClosing = false)
        {
            var list = (from c in mapActiveDocuments.Values// .AsParallel()
                        where c.Parent == parent && c.ActiveView != null 
                        select c).ToList();

            foreach (var document in list)
            {
                if (!CloseScreen(document.ActiveView as ScreenEditorView))
                    return false;
            }

            list = (from c in mapActiveDocuments.Values// .AsParallel()
                    where c.Parent == parent && c.NeedsSave == true && c.ActiveView == null
                    select c).ToList();
            foreach(var doc in list)
            {
                var res = UIInterface.ShowYesNoCancel(String.Format(Properties.Resources.SaveDoc,
                    doc.Title), CustomDialogIcons.Question);
                if (res == CustomDialogResults.Cancel)
                    return false;
                if (res == CustomDialogResults.Yes)
                {
                    if (doc.SaveToFile())
                        doc.NeedsSave = false;
                }
            }

            if (bParentClosing)
                CleanOnClose(parent);

            return true;
        }

        void CleanOnClose(IDocument parent)
        {
            var listToClean = (from c in mapActiveDocuments// .AsParallel()
                               where c.Value.Parent == parent && c.Value.ActiveView == null
                               select c).ToList();
            listToClean.ForEach(pair =>
            {
                mapActiveDocuments.Remove(pair.Key);
                pair.Value.Dispose();

                if (mapActiveDocumentUris.ContainsKey(pair.Value))
                    mapActiveDocumentUris.Remove(pair.Value);
                if (mapActiveDocumentTitles.ContainsKey(pair.Key))
                    mapActiveDocumentTitles.Remove(pair.Key);
            });
        }

        public bool IsAnyChildNeedsSave(IDocument parent)
        {
            var list = (from c in mapActiveDocuments.Values// .AsParallel()
                        where c.Parent == parent && c.ActiveView != null 
                        select c).ToList();
            foreach (var document in list)
            {
                if (document.NeedsSave)
                    return true;
            }

            return false;
        }
#endif
        public ObservableCollection<IDocumentManager> GetChildDocumentManagers()
        {
            return null;
        }

        public void PreTerminate(Uri uri, IDocument parent)
        { }

        public void Terminate(Uri uri, IDocument parent)
        {
        }

#if !WINDOWS_UWP && !NET_STANDARD
        public ObservableCollection<IDocumentManager> GetChildDocumentManagers(Uri uri, IDocument parent)
        {
            ScreenDocument doc = null;
            if (!mapActiveDocuments.TryGetValue(uri.GetPathString(), out doc))
            {
                doc = ScreenDocument.FromFile(uri.GetPathString(), parent);
                if (doc == null)
                    return null;
                doc.Parent = parent;
                mapActiveDocuments.Add(uri.GetPathString(), doc);
                mapActiveDocumentUris.Add(doc, uri.GetPathString());
            }

            var list = new ObservableCollection<IDocumentManager>();
            // list.Add(new TreeDocumentManagers.TreeEntitiesDocManager(this, doc));
            list.Add(new TreeDocumentManagers.TreeElementsDocManager(this, doc));
            return list;
        }

        public UFInterfaces.Service.IServiceControl GetServiceControl(IDocument parent)
        {
            return null;
        }

        public IDictionary<string, string> GetOptionsLicenseRequired(IDocument parent)
        {
            return null;
        }
#endif
        public String TypeTitle
        {
            get
            {
                return Properties.Resources.TypeTitle;
            }
        }

        public String TypeLabel
        {
            get
            {
#if !WINDOWS_UWP
                return Properties.Settings.Default.TypeLabel;
#else
                return "Screen";
#endif
            }
        }

#if !WINDOWS_UWP && !NET_STANDARD
        public BitmapImage TypeIcon
        {
            get
            {
                return GetBitmapImage("SMEditorSmall");
            }
        }

        public BitmapImage TypeIconOpen
        {
            get
            {
                return TypeIcon;
            }
        }

        public BitmapImage TypeIconLarge
        {
            get
            {
                return GetBitmapImage("SMEditor");
            }
        }

        public System.Windows.Controls.Primitives.Popup TypeContextMenu
        {
            get
            {
                return null;
            }
        }
#endif
        public String TypeScheme
        {
            get
            {
#if !WINDOWS_UWP
                Assembly assembly = Assembly.GetExecutingAssembly();
                return Path.GetFileNameWithoutExtension(assembly.Location);
#else
                Assembly assembly = this.GetType().GetTypeInfo().Assembly;
                return Path.GetFileNameWithoutExtension(assembly.GetName().Name);
#endif
            }
        }

        public String FileType
        {
            get
            {
#if !WINDOWS_UWP
                return Properties.Settings.Default.DefaultFileExt;
#else
                return ".xaml";
#endif
            }
        }

        public string FileName
        {
            get { return String.Empty; }
        }

        public String[] SaveAsFileExtensions
        {
            get
            {
                if (!String.IsNullOrWhiteSpace(Properties.Settings.Default.SaveAsFileExtensions))
                    return Properties.Settings.Default.SaveAsFileExtensions.ToLower().Split(';');
                return null;
            }
        }

#if !WINDOWS_UWP
        public bool RegisterFileType
        {
            get { return false; }
        }
#endif
        public bool isMultipleResource
        {
            get
            {
                return true;
            }
        }

        public bool isServiceResource
        {
            get { return false; }
        }

#if !WINDOWS_UWP && !NET_STANDARD
        public bool CanBeDragged
        {
            get
            {
                return false;
            }
        }
        public Object DragContent
        {
            get
            {
                return null;
            }
        }

        public Object BrowsableContent
        {
            get
            {
                return null;
            }
        }

        public bool IsResourceExpandable(IDocument document = null)
        {
            return true;
        }
#endif
        public bool IsStartupControllerAware
        {
            get
            {
                return true;
            }
        }

#if !WINDOWS_UWP && !NET_STANDARD
        public Uri CreateNewDocument(Uri relative, IDocument parent, String name, String model)
        {
            String path;
            Uri url;
            if (parent != null && parent.fileSystemProviderBase != null)
            {
                
                path = String.Format("{0}{1}{2}", relative.OriginalString, name, Properties.Settings.Default.DefaultFileExt);
                if (parent.fileSystemProviderBase.Exists(new FileManagerFile(parent.fileSystemProviderBase, path)))
                    return null;

                url = new Uri(path, UriKind.RelativeOrAbsolute);
                CreateNamedDocument(url, parent, model);
                return url;
            }

            if (relative.IsAbsoluteUri && Path.HasExtension(relative.OriginalString))
            {
                var ret = Path.ChangeExtension(relative.OriginalString, Properties.Settings.Default.DefaultFileExt);
                if (File.Exists(ret))
                    return null;
                var uri = new Uri(ret, UriKind.RelativeOrAbsolute);
                return CreateNamedDocument(uri, parent, model);
            }

            path = String.Format("{0}{1}{2}", relative.OriginalString, name, Properties.Settings.Default.DefaultFileExt);
            if (File.Exists(path))
                return null;

            url = new Uri(String.Format("{0}://{1}", TypeScheme, path), UriKind.RelativeOrAbsolute);
            return CreateNamedDocument(url, parent, model);
        }

        public Uri CreateNewDocument(Uri relative, IDocument parent, bool encryptFile = false)
        {
            return CreateNewDocument(relative, parent, false, encryptFile);
        }
        public Uri CreateNewTemplatedDocument(Uri relative, IDocument parent, bool encryptFile = false)
        {
            return CreateNewDocument(relative, parent, true, encryptFile);
        }
        public Uri CreateNewDocument(Uri relative, IDocument parent, bool useTemplates, bool encryptFile = false)
        {
                String path, newScreenName;
            int i = 0;
            Uri url;

            if (parent != null && parent.fileSystemProviderBase != null)
            {
                do
                {
                    newScreenName = String.Format("{0}{1}", Properties.Settings.Default.DefaultScreenName, ++i);
                    path = String.Format("{0}{1}{2}", relative.OriginalString, newScreenName, Properties.Settings.Default.DefaultFileExt);
                } while (parent.fileSystemProviderBase.Exists(new FileManagerFile(parent.fileSystemProviderBase, path)));

                url = new Uri(path, UriKind.RelativeOrAbsolute);
                if(!useTemplates)
                    CreateDefaultDocument(url, parent, encryptFile);
                else
                    CreateDefaultTemplatedDocument(url, parent, encryptFile);

                return url;
            }

            if (relative.IsAbsoluteUri && Path.HasExtension(relative.OriginalString))
            {
                var ret = Path.ChangeExtension(relative.OriginalString, Properties.Settings.Default.DefaultFileExt);
                var uri = new Uri(ret, UriKind.RelativeOrAbsolute);
                if (!useTemplates)
                    return CreateDefaultDocument(uri, parent, encryptFile);
                else
                    return CreateDefaultTemplatedDocument(uri, parent, encryptFile);
            }
            
            
            do
            {
                newScreenName = String.Format("{0}{1}", Properties.Settings.Default.DefaultScreenName, ++i);
                path = String.Format("{0}{1}{2}", relative.OriginalString, newScreenName, Properties.Settings.Default.DefaultFileExt);
            } while (File.Exists(path));
            var t = parent.MakeRelativeUri(new Uri(path)).ToString();
            if (path.StartsWith("\\"))
                url = new Uri(path); 
            else
                url = new Uri(String.Format("{0}://{1}", TypeScheme, path), UriKind.RelativeOrAbsolute);
            if (!useTemplates)
                return CreateDefaultDocument(url, parent, encryptFile);
            else
                return CreateDefaultTemplatedDocument(url, parent, encryptFile);
        }
#endif
        public Type DocumentType
        {
            get
            {
                return typeof(ScreenDocument);
            }
        }

#endregion

#region Properties

#if !WINDOWS_UWP && !NET_STANDARD
        IWorkspace workspace;
        public IWorkspace Workspace
        {
            get
            {
                return workspace;
            }
        }

        ISimpleLogging simpleLogging;
        public ISimpleLogging SimpleLogging
        {
            get
            {
                if (simpleLogging == null)
                    simpleLogging = GetService(typeof(ISimpleLogging)) as ISimpleLogging;
                return simpleLogging;
            }
        }

        ISymbolGallery symbolGallery;
        public ISymbolGallery SymbolGallery
        {
            get
            {
                if (symbolGallery == null)
                    symbolGallery = GetService(typeof(ISymbolGallery)) as ISymbolGallery;
                return symbolGallery;
            }
        }

        IAnimationExplorer animationExplorer;
        public IAnimationExplorer AnimationExplorer
        {
            get
            {
                if (animationExplorer == null)
                    animationExplorer = GetService(typeof(IAnimationExplorer)) as IAnimationExplorer;
                return animationExplorer;
            }
        }

        ICommandExplorer commandExplorer;
        public ICommandExplorer CommandExplorer
        {
            get
            {
                if (commandExplorer == null)
                    commandExplorer = GetService(typeof(ICommandExplorer)) as ICommandExplorer;
                return commandExplorer;
            }
        }

        IToolbox toolBox;
        public IToolbox ToolBox
        {
            get
            {
                if (toolBox == null)
                    toolBox = GetService(typeof(IToolbox)) as IToolbox;
                return toolBox;
            }
        }

        IPropertyControl propertyControl;
        public IPropertyControl PropertyControl
        {
            get
            {
                if (propertyControl == null)
                    propertyControl = GetService(typeof(IPropertyControl)) as IPropertyControl;
                return propertyControl;
            }
        }

        IScriptExplorer scriptExplorer;
        public IScriptExplorer ScriptExplorer
        {
            get
            {
                if (scriptExplorer == null)
                    scriptExplorer = GetService(typeof(IScriptExplorer)) as IScriptExplorer;
                return scriptExplorer;
            }
        }
#endif
        IUFProjectManager projectManager;
        public IUFProjectManager ProjectManager
        {
            get
            {
                if (projectManager == null)
                    projectManager = GetService(typeof(IUFProjectManager)) as IUFProjectManager;
                return projectManager;
            }
        }

        IStringEditorManager stringEditor;
        public IStringEditorManager StringEditor
        {
            get
            {
                if (stringEditor == null)
                    stringEditor = GetService(typeof(IStringEditorManager)) as IStringEditorManager;
                return stringEditor;
            }
        }

#if !NET_STANDARD
        IShortcutEditorManager shortcutEditor;
        public IShortcutEditorManager ShortcutEditor
        {
            get
            {
                if (shortcutEditor == null)
                    shortcutEditor = GetService(typeof(IShortcutEditorManager)) as IShortcutEditorManager;
                return shortcutEditor;
            }
        }
#endif

#if !WINDOWS_UWP && !NET_STANDARD
        IMenuEditorManager menuEditor;
        public IMenuEditorManager MenuEditor
        {
            get
            {
                if (menuEditor == null)
                    menuEditor = GetService(typeof(IMenuEditorManager)) as IMenuEditorManager;
                return menuEditor;
            }
        }
#endif

#if !NET_STANDARD
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

        IAuthenticationCredentialsProvider authenticationCredentialsProvider;
        public IAuthenticationCredentialsProvider AuthenticationCredentialsProvider
        {
            get
            {
                if (authenticationCredentialsProvider == null)
                    authenticationCredentialsProvider = GetService(typeof(IAuthenticationCredentialsProvider)) as IAuthenticationCredentialsProvider;
                return authenticationCredentialsProvider;
            }
        }
#endif

        IUFUserEditorManager userEditor;
        public IUFUserEditorManager UserEditor
        {
            get
            {
                if (userEditor == null)
                    userEditor = GetService(typeof(IUFUserEditorManager)) as IUFUserEditorManager;
                return userEditor;
            }
        }

#if !WINDOWS_UWP && !NET_STANDARD
        IUFUAEditorManager ufuaEditor;
        public IUFUAEditorManager UFUAEditor
        {
            get
            {
                if (ufuaEditor == null)
                    ufuaEditor = GetService(typeof(IUFUAEditorManager)) as IUFUAEditorManager;
                return ufuaEditor;
            }
        }

        IOPCUAClientStatus opcuaClientStatus;
        public IOPCUAClientStatus OPCUAClientStatus
        {
            get
            {
                if (opcuaClientStatus == null)
                    opcuaClientStatus = GetService(typeof(IOPCUAClientStatus)) as IOPCUAClientStatus;
                return opcuaClientStatus;
            }
        }

        ICrossReferenceEditorManager crossReferenceEditor;
        public ICrossReferenceEditorManager CrossReferenceEditor
        {
            get
            {
                if (crossReferenceEditor == null)
                    crossReferenceEditor = GetService(typeof(ICrossReferenceEditorManager)) as ICrossReferenceEditorManager;
                return crossReferenceEditor;
            }
        }

        ISchedulerEditorManager schedEditor;
        public ISchedulerEditorManager SchedulerEditor
        {
            get
            {
                if (schedEditor == null)
                    schedEditor = GetService(typeof(ISchedulerEditorManager)) as ISchedulerEditorManager;
                return schedEditor;
            }
        }

#endif
        #endregion

        #region IDisposable Members

        void IDisposable.Dispose()
        {
            mapActiveDocuments.Clear();
            mapActiveDocumentUris.Clear();
#if !WINDOWS_UWP && !NET_STANDARD
            if (workspace != null)
            {
                workspace.Closing -= workspace_Closing;
                workspace.Closed -= workspace_Closed;
                workspace.CloseButtonClick -= workspace_CloseButtonClick;
                workspace.ActiveWindowChanged -= workspace_ActiveWindowChanged;
                workspace.PromptFriendObjects -= workspace_PromptFriendObjects;
                workspace.PromptDocumentEditorObject -= workspace_PromptDocumentEditorObject;
                workspace.RibbonSelectionChanged -= workspace_RibbonSelectionChanged;
            }

            //if (PropertyControl != null)
            //    PropertyControl.AcceptChanges -= PropertyControl_AcceptChanges;
#endif
        }

        #endregion

#if !WINDOWS_UWP && !NET_STANDARD
        private ScreenDocument GetOrCreateDocument(IDocument parent)
        {
            var uri = new Uri(parent.FilePath, UriKind.RelativeOrAbsolute);

            ScreenDocument doc = null;
            if (!mapActiveDocuments.TryGetValue(uri.GetPathString(), out doc))
            {
                doc = ScreenDocument.FromFile(uri.GetPathString(), parent);
                if (doc == null)
                {
                    return null;
                }
                doc.Parent = DocumentHelper.GetRootParent(parent, traverse: false);
                mapActiveDocuments.Add(uri.GetPathString(), doc);
                mapActiveDocumentUris.Add(doc, uri.GetPathString());
            }
            return doc;
        }
#endif
#region IScreenManager Members
#if !WINDOWS_UWP && !NET_STANDARD
        public UserControl GetNewScreenTemplateControl()
        {
            return GetNewScreenTemplateControl(String.Empty);
        }

        public UserControl GetNewScreenTemplateControl(Uri uri)
        {
            return GetNewScreenTemplateControl(Path.GetFileNameWithoutExtension(uri.GetPathString()));
        }

        UserControl GetNewScreenTemplateControl(String filename)
        {
            return new NewScreenType(true)
            {
                fileName = filename,
                xamlCode = ScreenDocument.DefaultXaml
            };
        }
#endif
#endregion

#if !WINDOWS_UWP && !NET_STANDARD

#region ICrossReference
        IEnumerable<String> cRTagList;
        IDictionary<String, String> cRMapPrototypes;
        IDictionary<String, IList<String>> cRMapDefinitions;
        public List<UFInterfaces.Editors.CrossReferenceResultModel> GetCRObjects(UFInterfaces.Editors.CrossReferenceModel model)
        {
            var result = new List<UFInterfaces.Editors.CrossReferenceResultModel>();
            if (model.ResourceList == null || model.QuitEvent.IsCancellationRequested)
                return result;
            bool getTexts = model.CRManagement.CrossReferenceTypeList.Contains(CrossReferenceType.Strings);
            bool getTags = model.CRManagement.CrossReferenceTypeList.Contains(CrossReferenceType.Tags);
            bool getConnections = model.CRManagement.CrossReferenceTypeList.Contains(CrossReferenceType.Connections);
            bool getScreens = model.CRManagement.CrossReferenceTypeList.Contains(CrossReferenceType.Resources);
            if (!getTexts && !getTags && !getConnections && !getScreens)
                return result;

            var p = DocumentHelper.GetRootParent(model.Parent, true);
            string pTitle = p.Title;
            bool updateStrings = false;
            var stringeditorManager = p.GetService(typeof(IStringEditorManager)) as IStringEditorManager;
            updateStrings = stringeditorManager != null;

            if (getTags)
            {
                cRTagList = UFUAEditor.GetFlatListTags(p);
                cRMapPrototypes = UFUAEditor.GetFlatListPrototypeInstances(p);
                cRMapDefinitions = UFUAEditor.GetFlatListPrototypes(p);
            }
            var path = model.Parent.fileSystemProviderBase != null ? string.Empty : p.rootBase;
            List<Object> quickDispatcherList = new List<object>();
            object lockobject = new object();
            bool bInit = false;
            List<String> list = model.ResourceList as List<String>;
            string docType = DocManagerType.ScreenManager.ToString();
            Dictionary<Uri, CRMapsHeler> postonedScreenDictionary = new Dictionary<Uri, CRMapsHeler>();
            //Parallel.ForEach(list, (resource, loopstate) =>
            for (int i = 0; i < list.Count(); i++)
            {
                var resource = list[i];
                if (model.QuitEvent.IsCancellationRequested)
                    return result;
                    //loopstate.Break();
                var uri = new Uri($"{path}\\{resource}", UriKind.RelativeOrAbsolute);
                var pathString = uri.GetPathString();
                using (ScreenDocument doc = CreateDocument(model.Parent, uri))
                {
                    if (doc != null)
                    {
                        string docTitle = doc.Title;

                        doc.GetTagEntityReference += (o, ev) =>
                        {
                            var erString = UFUAEditor.GetTagEntityReference(doc, ev.Name, ev.Instance, useCachedUow: true);
                            if (!String.IsNullOrEmpty(erString))
                                ev.entityReference = erString.FromXml<OPCUAViewModel.OPCUAEntityReference>();
                        };

                        var cRMapsHeler = new CRMapsHeler() { UpdateStrings = updateStrings };
                        {
                            //lock (lockobject)
                            {
                                if (bInit)
                                    cRMapsHeler.DocDispatcherReferenceList = quickDispatcherList;
                                else
                                {
                                    quickDispatcherList = doc.GetQuickVariableObjectDispatcherList(cRTagList?.ToList(), cRMapPrototypes, cRMapDefinitions);
                                    bInit = true;
                                    cRMapsHeler.DocDispatcherReferenceList = quickDispatcherList;
                                }
                            }

                            doc.GetDynamicMapDetailsForElementAndChilds(cRMapsHeler, model);

                            //foreach (var reference in cRMapsHeler.ScreenLinks.Keys)
                            Parallel.ForEach(cRMapsHeler.ScreenLinks.Keys, reference =>
                            {
                                if (model.QuitEvent.IsCancellationRequested)
                                    return;
                                foreach (var collectionref in cRMapsHeler.ScreenLinks[reference])
                                {
                                    if (model.QuitEvent.IsCancellationRequested)
                                        return;
                                    String _reference = reference;
                                    IDictionary<String, String> refscreendetails = collectionref as IDictionary<String, String>;
                                    if (refscreendetails.Count > 0 && !string.IsNullOrEmpty(refscreendetails.First().Value))
                                    {
                                        var name = path.Split('/').LastOrDefault();
                                        lock (result)
                                        {
                                            result.Add(new UFInterfaces.Editors.CrossReferenceResultModel()
                                            {
                                                RelativePath = refscreendetails.First().Value,
                                                Name = name,
                                                AppName = p.Title,
                                                CReferenceType = CrossReferenceType.Resources,
                                                Description = string.Format("{0}\\{1} ({2})", doc.Title, _reference, refscreendetails.First().Key),
                                                Settings = string.Format("{0}|{1}", _reference, pathString),
                                                ContainerDoc = TypeScheme,
                                                IconType = docType
                                            });
                                        }
                                    }
                                }
                            });

                            //foreach (var reference in cRMapsHeler.TagInScreenEntities.Keys)
                            Parallel.ForEach(cRMapsHeler.TagInScreenEntities.Keys, reference =>
                            {
                                if (model.QuitEvent.IsCancellationRequested)
                                    return;
                                foreach (var collectionref in cRMapsHeler.TagInScreenEntities[reference])
                                {
                                    if (model.QuitEvent.IsCancellationRequested)
                                        return;
                                    String _reference = reference;
                                    IDictionary<String, OPCUAViewModel.OPCUAEntityReference> refdetails = collectionref as IDictionary<String, OPCUAViewModel.OPCUAEntityReference>;
                                    if (refdetails.Count > 0 && !string.IsNullOrEmpty(refdetails.First().Value?.RelativePath))
                                        lock (result)
                                        {
                                            var details = refdetails.First().Value;
                                            result.Add(new UFInterfaces.Editors.CrossReferenceResultModel()
                                            {
                                                RelativePath = details.RelativePath,
                                                Name = details.Name,
                                                AppName = details.AppName,
                                                ReferencedNodeId = details.ResolvedNodeId?.Identifier.ToString(),
                                                EndpointUrl = details.EndpointUrl,
                                                CReferenceType = CrossReferenceType.Tags,
                                                Description = string.Format("{0}{1} ({2})", doc.Title, "\\" + _reference, refdetails.First().Key),
                                                Settings = string.Format("{0}|{1}", _reference, pathString),
                                                ContainerDoc = TypeScheme,
                                                IconType = docType
                                            });
                                        }
                                }
                            });

                            //foreach (var reference in cRMapsHeler.TagInScreen.Keys)
                            Parallel.ForEach(cRMapsHeler.TagInScreen.Keys, reference =>
                            {
                                if (model.QuitEvent.IsCancellationRequested)
                                    return;
                                foreach (var collectionref in cRMapsHeler.TagInScreen[reference])
                                {
                                    if (model.QuitEvent.IsCancellationRequested)
                                        return;
                                    String _reference = reference;
                                    IDictionary<String, OPCUAViewModel.OPCUAEntityReference> refdetails = collectionref as IDictionary<String, OPCUAViewModel.OPCUAEntityReference>;
                                    if (refdetails.Count > 0 && !string.IsNullOrEmpty(refdetails.First().Value?.RelativePath))
                                    lock (result)
                                    {
                                        var details = refdetails.First().Value;
                                        result.Add(new UFInterfaces.Editors.CrossReferenceResultModel()
                                        {
                                            RelativePath = details.RelativePath,
                                            Name = details.Name,
                                            AppName = details.AppName,
                                            ReferencedNodeId = details.ResolvedNodeId?.Identifier.ToString(),
                                            EndpointUrl = details.EndpointUrl,
                                            CReferenceType = CrossReferenceType.Tags,
                                            Description = string.Format("{0}{1} ({2})", doc.Title, string.Empty, refdetails.First().Key),
                                            Settings = string.Format("{0}|{1}", _reference, pathString),
                                            ContainerDoc = TypeScheme,
                                            IconType = docType
                                        });
                                    }
                                }
                            });
                            if (cRMapsHeler.ErrorMessages.Count > 0)
                                model.AddMessages(cRMapsHeler.ErrorMessages);
                            cRMapsHeler.ErrorMessages.Clear();

                            //foreach (var reference in cRMapsHeler.ConnectionString.Keys)
                            Parallel.ForEach(cRMapsHeler.ConnectionString.Keys, reference =>
                            {
                                if (model.QuitEvent.IsCancellationRequested)
                                    return;
                                foreach (var collectionref in cRMapsHeler.ConnectionString[reference])
                                {
                                    if (model.QuitEvent.IsCancellationRequested)
                                        return;
                                    String _reference = reference;
                                    IDictionary<String, String> refscreendetails = collectionref as IDictionary<String, String>;
                                    if (refscreendetails.Count > 0 && !string.IsNullOrEmpty(refscreendetails.First().Value))
                                    {
                                        lock (result)
                                        {
                                            if (refscreendetails.First().Value != "Undefined")
                                            {
                                                string settings = string.Format("{0}|{1}|{2}", reference, pathString, $"{reference}");
                                                string container = TypeScheme;
                                                bool bUseDefault = false;
                                                if (refscreendetails.First().Key.Contains("@UseDefault"))
                                                {
                                                    bUseDefault = true;
                                                    settings = string.Format("{0}|{1}|{2}", DocManagerType.HistorianConn.ToString(), p.rootBase, reference);
                                                    container = (UFUAEditor as IDocumentManager).TypeScheme;
                                                }
                                                result.Add(new UFInterfaces.Editors.CrossReferenceResultModel()
                                                {
                                                    RelativePath = refscreendetails.First().Value, //$"{Properties.Resources.EntitiesConnectionString}\\{doc.Title}\\{reference}",
                                                    Name = Properties.Resources.RefrencedBy, //$"{reference}",
                                                    AppName = p.Title,
                                                    CReferenceType = CrossReferenceType.Connections,
                                                    Description = $"{doc.Title}\\{_reference}", //string.Format("{0}", refscreendetails.First().Value),
                                                    Settings = settings,
                                                    ContainerDoc = container,
                                                    IconType = docType
                                                });
                                            }
                                        }
                                    }
                                }
                            });
                            if (cRMapsHeler.ErrorMessages.Count > 0)
                                model.AddMessages(cRMapsHeler.ErrorMessages);
                            cRMapsHeler.ErrorMessages.Clear();

                            //foreach (var reference in cRMapsHeler.StringIDs.Keys)
                            Parallel.ForEach(cRMapsHeler.StringIDs.Keys, reference =>
                            {
                                if (model.QuitEvent.IsCancellationRequested)
                                    return;
                                foreach (var collectionref in cRMapsHeler.StringIDs[reference])
                                {
                                    if (model.QuitEvent.IsCancellationRequested)
                                        return;
                                    IDictionary<String, String> refscreendetails = collectionref as IDictionary<String, String>;
                                    string textId = refscreendetails.First().Value;
                                    string referenceType = refscreendetails.First().Key;
                                    if (refscreendetails.Count > 0 && !string.IsNullOrEmpty(textId))
                                    {
                                        lock (result)
                                        {
                                            result.Add(new UFInterfaces.Editors.CrossReferenceResultModel()
                                            {
                                                RelativePath = $"{textId}",
                                                Name = $"{textId}",
                                                AppName = p.Title,
                                                CReferenceType = CrossReferenceType.Strings,
                                                Description = string.Format("{0}\\{1} ({2})", doc.Title, reference, referenceType),
                                                Settings = string.Format("{0}|{1}|{2}", reference, pathString, $"{reference}"),
                                                ContainerDoc = TypeScheme,
                                                IconType = docType
                                            });
                                        }
                                    }
                                }
                            });
                            if (cRMapsHeler.ErrorMessages.Count > 0)
                                model.AddMessages(cRMapsHeler.ErrorMessages);
                            cRMapsHeler.ErrorMessages.Clear();

                            result.Add(new UFInterfaces.Editors.CrossReferenceResultModel()
                            {
                                RelativePath = $"{doc.Title}",
                                Name = $"{doc.Title}",
                                AppName = p.Title,
                                CReferenceType = CrossReferenceType.Strings,
                                Description = string.Format("{0} ({1})", doc.Title, Properties.Resources.NewScreenName),
                                Settings = string.Format("{0}|{1}|{2}", null, pathString, null),
                                ContainerDoc = TypeScheme,
                                IconType = docType
                            });
                        }
                    }

                }
            }//);

            return result;
        }

        public void EditCRObject(IDocument parent, string settings)
        {
            var p = DocumentHelper.GetRootParent(parent, true);
            string[] path = settings.Split('|');
            if(path.Length >= 2)
            {
                var uri = new Uri(path[1], UriKind.RelativeOrAbsolute);
                Edit(uri, p);
                Dispatcher.CurrentDispatcher.BeginInvokeIfRequired(() =>
                                    {
                                       GetActiveView(uri).SelectElement(path[0]);
                                    });
            }
        }
#endregion

        private ScreenDocument CreateDocument(IDocument parent, Uri uri)
        {
            var p = DocumentHelper.GetRootParent(parent, true);
            ScreenDocument doc = null;
            doc = ScreenDocument.FromFile(uri.GetPathString(), parent);
            if (doc != null)
                doc.Parent = p;
            return doc;
        }
        private ScreenDocument GetOrCreateDocumentFromUri(IDocument parent, Uri uri)
        {
            var p = DocumentHelper.GetRootParent(parent, true);
            ScreenDocument doc = null;
            if (!mapActiveDocuments.TryGetValue(uri.GetPathString(), out doc))
            {
                doc = ScreenDocument.FromFile(uri.GetPathString(), parent);
                if (doc != null)
                {
                    doc.Parent = p;
                    mapActiveDocuments.Add(uri.GetPathString(), doc);
                    mapActiveDocumentUris.Add(doc, uri.GetPathString());
                }
            }
            return doc;
        }

        public bool NeedToCloseCRDocuments()
        {
            return mapActiveDocuments.Count > 0 && (from d in mapActiveDocuments.Values where d.ActiveView != null select d).FirstOrDefault() != null;
        }

        public bool NeedSingleThreadedApartment
        {
            get
            {
                return true;
            }
        }

        public void RenameCRObjects(UFInterfaces.Editors.CrossReferenceModel model)
        {
            if (model.ResourceList == null || model.QuitEvent.IsCancellationRequested)
                return;

            var p = DocumentHelper.GetRootParent(model.Parent, true);
            Dictionary<string, string> nodeIdMap = new Dictionary<string, string>();
            foreach (var resource in model.ResourceList)
            {
                if (model.QuitEvent.IsCancellationRequested)
                    return;

                var uri = new Uri(string.Format("{0}\\{1}", model.Parent.fileSystemProviderBase != null ? string.Empty : p.rootBase, resource), UriKind.RelativeOrAbsolute);

                var entityList = new List<UIElement>();

                using (ScreenDocument doc = CreateDocument(model.Parent, uri))
                {
                    if(doc != null)
                    {
                        Canvas activeLayer = null;
                        foreach (var name in doc.MapScreenEntities.Keys)
                        {
                            if (model.QuitEvent.IsCancellationRequested)
                                return;
                            
                            var entity = doc.MapScreenEntities[name];
                            if (entity.Entity == null )
                            {
                                UIElement uie = null;
                                if(!string.IsNullOrEmpty(entity.ProblematicXaml))
                                {
                                    try
                                    {
                                        uie = entity.ProblematicXaml.ReadUIElement();
                                    }
                                    catch(Exception ex)
                                    {
                                        model.ErrorMessages.Add($"{doc.Title}: {ex.Message}");
                                    }
                                }
                                else
                                {
                                    if(activeLayer == null)
                                        activeLayer = doc.GetCurrentXamlDocument();
                                    uie = doc.FindInnerControl(activeLayer, name);
                                }
                                if (uie != null)
                                {
                                    entity.Entity = uie;
                                    entity.Document = doc;
                                    entityList.Add(uie);
                                }
                            }
                            doc.UpdateProblematicXamlWriterProperties((entity.Entity as FrameworkElement), name);
                        }
                        if(activeLayer == null)
                            activeLayer = doc.GetCurrentXamlDocument();
                        doc.UpdateRepositoryItems(activeLayer, activeLayer, true);
                        doc.RenameReferences(nodeIdMap, model);
                        activeLayer.Dispose();
                        
                        //Clean the maps in order to load the updated documents
                        ScreenDocument key = null;
                        if (mapActiveDocuments.ContainsKey(uri.GetPathString()))
                        {
                            key = mapActiveDocuments[uri.GetPathString()];
                            mapActiveDocuments.Remove(uri.GetPathString());
                        }
                        if (key != null && mapActiveDocumentUris.ContainsKey(key))
                            mapActiveDocumentUris.Remove(key);

                        if (mapActiveDocumentTitles.ContainsKey(uri.GetPathString()))
                            mapActiveDocumentTitles.Remove(uri.GetPathString());
                    }
                }

                foreach (var entity in entityList.OfType<IDisposable>())
                    entity.Dispose();
                entityList.Clear();
            }
        }
  
       
#endif

#region Style
#if !NET_STANDARD
        void LoadDefaultStyle(FrameworkElement fe, IDocument parent)
        {
            String fileStyle = String.Format("{0}{1}", parent.GetSpecialFolder(SpecialFolders.Documents), "default.style");
            var uri = new Uri(fileStyle, UriKind.RelativeOrAbsolute);
            fileStyle = uri.GetPathString();

#if !WINDOWS_UWP
            bool bTempFile = false;
            if (parent.fileSystemProviderBase != null)
            {
                if (parent.fileSystemProviderBase.Exists(new FileManagerFile(parent.fileSystemProviderBase, fileStyle)))
                {
                    var data = parent.fileSystemProviderBase.ReadFile(new FileManagerFile(parent.fileSystemProviderBase, fileStyle));
                    var tempFile = new TemporaryFile();
                    try
                    {
                        File.WriteAllBytes(tempFile.FilePath, data);
                    }
                    catch (Exception ex)
                    {
                        if (Environment.UserInteractive)
                            MessageBox.Show(ex.Message);

                        return;
                    }

                    fileStyle = tempFile.FilePath;
                    bTempFile = true;
                }
            }
#endif
            if (!File.Exists(fileStyle))
                return;

            try
            {
#if !WINDOWS_UWP
                using (var stream = File.OpenRead(fileStyle))
                {
                    var resdict = (ResourceDictionary)XamlReader.Load(stream);
                    //var resdict = new ResourceDictionary
                    //{
                    //    Source = new Uri(fileStyle, UriKind.RelativeOrAbsolute)
                    //};
                    fe.Resources.MergedDictionaries.Add(resdict);
                }
#else
                var resdict = (ResourceDictionary)XamlReader.Load(fileStyle);
                fe.Resources.MergedDictionaries.Add(resdict);
#endif
            }
            catch (Exception ex)
            {
#if !WINDOWS_UWP
                if (Environment.UserInteractive && UIInterface != null)
                    UIInterface.ShowError(ex.Message);
#else
                if (UIInterface != null)
                    UIInterface.ShowError(ex.Message);
#endif
            }

#if !WINDOWS_UWP
            if (bTempFile)
            {
                try
                {
                    File.Delete(fileStyle);
                }
                catch { }
            }
#endif
        }

        public void UpdateStringId(UIElement element, bool bAlreadyUntranslated, bool bClearTexts, bool bClearTooltips)
        {
            (Workspace.ContextDocument.ActiveView as ScreenEditorView)?.UpdateStringIdAndTranslate(element, bAlreadyUntranslated, bClearTexts, bClearTooltips);
        }
        public bool SaveToSvg(UFInterfaces.Editors.SvgModel model, Color tileColor)
        {
            var p = DocumentHelper.GetRootParent(model.Parent, false);
            if (model.ResourceList == null)
                return false;
            var path = model.Parent.fileSystemProviderBase != null ? p.rootBaseDB : p.rootBase;
            var svgPath = model.ProjectFolder;

            if (String.IsNullOrEmpty(svgPath))
            {
                if (Workspace != null)
                {
                    //Workspace.IsBusy = false;
                    Workspace.ResetProgressState();
                    WaitForPriority.DoEvents();
                }
                return false;
            }
            if (!System.IO.Directory.Exists(svgPath))
                System.IO.Directory.CreateDirectory(svgPath);

            var title = System.IO.Path.GetFileNameWithoutExtension(p.FilePath);
            if (XpoHelpers.XpoHelper.IsDataSource(p.FilePath))
                title = XpoHelpers.XpoHelper.GetDataSourceTitle(p.FilePath, onlytitle: true);
            //var startupInfo = new Amib.Threading.STPStartInfo()
            //{
            //    ThreadPoolName = "ScreenManagerThreadPool",
            //    ThreadPriority = System.Threading.ThreadPriority.BelowNormal,
            //    MaxWorkerThreads = SysInfo.GetNumberOfLogicalProcessors()* 4,
            //    AreThreadsBackground = false,
            //    ApartmentState = ApartmentState.STA
            //};
            List<String> list = model.ResourceList as List<String>;
            //Amib.Threading.SmartThreadPool smartThreadPool = new Amib.Threading.SmartThreadPool(startupInfo);

            //if(workspace != null)
            //    workspace.IsBusy = true;

            Workspace?.UpdateProgressState(0,
                        model.ResourceList.Count(),
                        $"{Properties.Resources.ExportingToSvg}",
                        TaskbarItemProgressState.Normal);
            WaitForPriority.DoEvents();

            try
            {
                if (p.fileSystemProviderBase != null)
                {
                    if (string.IsNullOrEmpty(title))
                    {
                        if (Workspace != null)
                        {
                            //Workspace.IsBusy = false;
                            Workspace.ResetProgressState();
                            WaitForPriority.DoEvents();
                        }
                        return false;
                    }

                    FileManagerFile project = new FileManagerFile(p.fileSystemProviderBase, p.rootBaseDB);
                    if (project != null)
                    {
                        CopyFile(p.fileSystemProviderBase, project, svgPath, $"{title}{ProjectManager.GetDefaultFileExt()}");
                        model.ExportedProjectFilePath = System.IO.Path.Combine(svgPath, $"{title}{ProjectManager.GetDefaultFileExt()}");
                    }

                    FileManagerFolder fileManager = new FileManagerFolder(p.fileSystemProviderBase, p.GetSpecialFolder(SpecialFolders.Images).ToString());
                    if (p.fileSystemProviderBase.Exists(fileManager))
                    {
                        CopyFiles(p.fileSystemProviderBase,fileManager, $"{svgPath}\\{p.GetSpecialFolder(SpecialFolders.Images).ToString()}");
                    }
                    fileManager = new FileManagerFolder(p.fileSystemProviderBase, p.GetSpecialFolder(SpecialFolders.Documents).ToString());
                    if (p.fileSystemProviderBase.Exists(fileManager))
                    {
                        CopyFiles(p.fileSystemProviderBase, fileManager, $"{svgPath}\\{p.GetSpecialFolder(SpecialFolders.Documents).ToString()}");
                    }
                    fileManager = new FileManagerFolder(p.fileSystemProviderBase, p.rootBaseDB);
                    if (p.fileSystemProviderBase.Exists(fileManager))
                    {
                        CopyFiles(p.fileSystemProviderBase, fileManager, $"{svgPath}\\{title}");
                    }
                }

#if DEBUG
                using (var mainwatch = new StopWatcher("SVG complete saving took : {0}"))
#endif
                {
                    Dictionary<string, string> eumap = null;
                    bool errorMessages = false;
                    List<SVGHelper.screenobject> screenobjects = new List<SVGHelper.screenobject>();
                    var screenController = p as IScreenController;
                    for (int i = 0; i < model.ResourceList.Count(); i++)
                    {
                        var resource = list[i];
                        var uri = p.fileSystemProviderBase != null ? new Uri($"{resource}", UriKind.RelativeOrAbsolute) : new Uri($"{path}\\{resource}", UriKind.RelativeOrAbsolute);
                        if (screenController != null)
                        {
                            Color color = screenController.GetIdentityColor(uri);
                            screenobjects.Add(new SVGHelper.screenobject() { Path = p.MakeRelativeUri(uri).GetPathString(), Color = SVGHelper.ColorConverterExtensions.ToRgbString(color) });
                        }
                        //smartThreadPool.QueueWorkItem(() =>
                        //{
                        if (p.fileSystemProviderBase != null && resource.StartsWith(p.rootBaseDB))
                                resource = resource.Replace($"{p.rootBaseDB}\\", $"{title}\\");
                            var pathString = $"{svgPath}\\{resource}";
                            var absolutePath = System.IO.Path.GetDirectoryName(pathString) + "\\";
                            var absolutePath2 = $"{svgPath}\\{SpecialFolders.Images}\\";
                            
                            using (ScreenDocument doc = CreateDocument(model.Parent, uri))
                            {
                                try
                                {
                                    var cssPrefix = Properties.Settings.Default.SVGStylePrefix + i;
                                    if (UFUAEditor != null)
                                    {
                                        if(eumap == null)
                                            eumap = UFUAEditor.GetTagsEngineeringUnit(doc);
                                        doc.GetTagEntityReference += (o, ev) =>
                                        {
                                            var erString = UFUAEditor.GetTagEntityReference(doc, ev.Name, ev.Instance, useCachedUow: true);
                                            if (!String.IsNullOrEmpty(erString))
                                                ev.entityReference = erString.FromXml<OPCUAViewModel.OPCUAEntityReference>();
                                        };
                                    }

                                    using (SVGHelper.DocSVGHelper docSVGHelper = new SVGHelper.DocSVGHelper(doc, WebHMIDesignHelper.WebHMIHelper.VisibleHMIScreenControls, eumap, cssPrefix))
                                    {
                                        var ret = docSVGHelper.Save(pathString);
                                        errorMessages = errorMessages || ret;
                                    }
                                }
                                catch (Exception ex)
                                {
                                    errorMessages = true;
                                    logDeploy.Error(string.Format(Properties.Resources.ErrorExportingDocument, doc.Title), ex);
                                }
                            }
                            workspace?.IncrementProgressState();
                            if (model.QuitEvent.IsCancellationRequested)
                                break;
                            WaitForPriority.DoEvents();
                        //});
                    }

                    if(screenobjects.Count > 0)
                    {
                        try
                        {
                            string documents = $"{p.GetSpecialFolder(SpecialFolders.Documents).GetPathString()}";
                            string tilePath = p.fileSystemProviderBase != null ? $"{svgPath}\\{documents}" : $"{documents}";
                            SVGHelper.tile tile = new SVGHelper.tile() 
                            { 
                                Color = SVGHelper.ColorConverterExtensions.ToRgbString(tileColor),
                                ScreenObjects = screenobjects
                            };
                            var ret = SVGHelper.TileSVGHelper.Save($"{tilePath}", p, tile);
                            errorMessages = errorMessages || ret;
                        }
                        catch (Exception ex)
                        {
                            errorMessages = true;
                            logDeploy.Error(string.Format(Properties.Resources.ErrorExportingDocument, p.Title), ex);
                        }
                    }
                    //smartThreadPool.WaitForIdle();
                    //smartThreadPool.Dispose();

                    if (Workspace != null)
                    {
                        //Workspace.IsBusy = false;
                        Workspace.ResetProgressState();
                        WaitForPriority.DoEvents();
                    }

                    if (model.QuitEvent.IsCancellationRequested)
                    { 
                        return false;
                    }
                    else if (errorMessages)
                    {
                        model.IsInError = true;
                        return false;
                    }
                };
            }
            catch
            {
                //if (workspace != null)
                //    workspace.IsBusy = false;
                return false;
            }
            return true;
        }

        private void CopyFiles(FileSystemProviderBase fileSystemProviderBase, FileManagerFolder fildeManager, string destination)
        {
            if (fileSystemProviderBase == null || fildeManager == null)
                return;
            var fileManagerFiles = fildeManager.GetFiles().ToList();
            fileManagerFiles.ForEach(f =>
            {
                CopyFile(fileSystemProviderBase, f, destination);
            });
            var fileManagerFolders = fildeManager.GetFolders().ToList();
            fileManagerFolders.ForEach(f =>
            {
                CopyFiles(fileSystemProviderBase, f, $"{destination}\\{f.Name}");
            });
        }

        private void CopyFile(FileSystemProviderBase fileSystemProviderBase, FileManagerFile f, string destination, string fileName = null)
        {
            try
            {
                string name = !string.IsNullOrEmpty(fileName) ? fileName : f.Name;
                var destFile = $"{destination}\\{name}";
                var data = fileSystemProviderBase.ReadFile(f);
                Directory.CreateDirectory(Path.GetDirectoryName(destFile));
                File.WriteAllBytes(destFile, data);
            }
            catch (Exception ex)
            {
            }
        }

        public IToolbar GetToolbar()
        {
            if (menuControl == null)
                menuControl = new MenuControl(this, workspace);
            if (!menuControl.IsToolbarShown && workspace.IsWorkspaceLoaded && !bToolbarInitialized)
            {
                bToolbarInitialized = true;
                menuControl.Show();
            }

            return menuControl;
        }

        public IList<System.Windows.Input.ICommand> GetAlwaysAvailableCommand()
        {
            return null;
        }
#endif
        #endregion
    }
}