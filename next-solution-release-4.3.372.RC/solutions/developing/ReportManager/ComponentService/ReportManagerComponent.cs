using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
#if !NET_STANDARD
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Controls;
using PropertyControl.ComponentService;
using ReportSettings.PropertyDataTemplate;
using Toolbox.ComponentService;
using Tracing.ComponentService;
using UIMsgBoxAlertService.ComponentService;
using VFS;
#endif
using DocumentManager.ComponentService;
using ReportSettings.Documents;
using UFInterfaces;
using UFInterfaces.CoreHostComponents;
using Utilities;
using System.Collections.ObjectModel;
using UFUAEditor.ComponentService;
using DataReader;
using UriResolver.ComponentService;
using log4net;
using UFProjectManager.ComponentService;
using UFInterfaces.Editors;
using System.Threading.Tasks;
using DocumentManager.ComponentService.Helpers;

namespace ReportManager.ComponentService
{
    public class ReportManagerComponent : ComponentBase<IReportManager>, IReportManager, IDocumentManager, IDisposable
#if !NET_STANDARD
        , ICrossReference
#endif
    {
        #region Declaration

        readonly Object lockObject = new Object();

        readonly Dictionary<String, ReportDocument> mapActiveDocuments = new Dictionary<String, ReportDocument>(StringComparer.OrdinalIgnoreCase);
        readonly Dictionary<ReportDocument, String> mapActiveDocumentUris = new Dictionary<ReportDocument, String>();
        readonly Dictionary<String, String> mapActiveDocumentTitles = new Dictionary<String, String>(StringComparer.OrdinalIgnoreCase);

        readonly Dictionary<ReportDocument, ReportExecuter> mapRunningReports = new Dictionary<ReportDocument, ReportExecuter>();
        readonly Dictionary<IDocument, List<ReportExecuter>> mapRunningReportsPerParent = new Dictionary<IDocument, List<ReportExecuter>>();

#if !NET_STANDARD
        public static ReportManagerComponent reportManagerComponent { get; protected set; }
        //internal RibbonEditor ribbonEditorRibbonTab { get; private set; }
        MenuControl menuControl;

        private static readonly ILog logLicense = LogManager.GetLogger(Properties.Resources.LicenseManager);
        private static readonly ILog logGeneral = LogManager.GetLogger(Properties.Resources.ReportExecuter);
#else
        private static readonly ILog logLicense = LogManager.GetLogger(System.Reflection.Assembly.GetEntryAssembly(), Properties.Resources.LicenseManager);
        private static readonly ILog logGeneral = LogManager.GetLogger(System.Reflection.Assembly.GetEntryAssembly(), Properties.Resources.ReportExecuter);
#endif

        #endregion Declaration

        #region IUFInterfaceBase Members

        void IUFInterfaceBase.Initialize()
        {
#if !NET_STANDARD
            if (reportManagerComponent == null)
                reportManagerComponent = this;

            GetComponentInterfaces();
#endif
        }

        #endregion IUFInterfaceBase Members

#if !NET_STANDARD
        private static BitmapImage GetControlImage(String image, bool bShared = false)
        {
            var bm = SharedResources.Helpers.ResourceManager.GetCommonImage(reportManagerComponent.TypeLabel, image, bShared);
            return bm;
        }

        private void GetComponentInterfaces()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            var ext = Properties.Settings.Default.DefaultFileExt.ToLower().Replace(".", "");
            ApplicationPropertiesHelper.SetProperty(ext, Path.GetFileNameWithoutExtension(assembly.Location));

            if (workspace == null)
                workspace = GetService(typeof(IWorkspace)) as IWorkspace;
            if (workspace != null)
            {
                workspace.Closed += workspace_Closed;
                workspace.Closing += workspace_Closing;
                workspace.CloseButtonClick += workspace_CloseButtonClick;
                workspace.ActiveWindowChanged += workspace_ActiveWindowChanged;
            }

            if (PropertyControl != null)
            {
                var dt = new DataTemplate();
                var factory = new FrameworkElementFactory(typeof(DataReaderPropertyEditor));
                dt.DataType = typeof(DataReaderReference);
                dt.VisualTree = factory;

                PropertyControl.AddPropertyEditor(typeof(DataReaderReference), dt);
                /*
                var dt = new DataTemplate();
                var factory = new FrameworkElementFactory(typeof(ReportConnectionStringReferencePropertyEditor));
                dt.DataType = typeof(ConnectionStringReference);
                dt.VisualTree = factory;

                PropertyControl.AddPropertyEditor(typeof(ConnectionStringReference), dt);
                */
                PropertyControl.AcceptChanges += PropertyControl_AcceptChanges;
            }
        }

        void PropertyControl_AcceptChanges(object sender, EventArgs e)
        {
            using (var cursor = new WaitCursor())
            {
                var list = (from c in mapActiveDocuments.Values// .AsParallel()
                            where c.NeedsSave == true && c.ActiveView == null
                            select c).ToList();
                list.ForEach(doc => doc.SaveToFile());
            }
        }

        void workspace_CloseButtonClick(object sender, UFInterfaces.CloseButtonEventArgs e)
        {
            if (!(e.TargetItem is ReportEditorUI))
                return;

            var view = e.TargetItem as ReportEditorUI;
            if (!CloseReport(view))
                e.Cancel.Cancel = true;
        }

        private bool CanClose(ReportEditorUI view, bool bSave = true, bool bDispose = true)
        {
            String uri;
            if (mapActiveDocumentUris.TryGetValue(view.Document, out uri))
            {
                if (bSave && view.Document.NeedsSave)
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
                        view.SaveReportLayout();
                        if (!view.Document.SaveCurrentDocument())
                            return false;
                    }
                }
            }

            return true;
        }
        private bool CloseReport(ReportEditorUI view, bool bSave = true, bool bDispose = true)
        {
            if (view == null)
                return true;

            String uri;
            if (mapActiveDocumentUris.TryGetValue(view.Document, out uri))
            {
                if (bSave && view.Document.NeedsSave)
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
                        view.SaveReportLayout();
                        view.Document.SaveCurrentDocument();
                    }
                }

                if (bDispose)
                {
                    mapActiveDocumentUris.Remove(view.Document);
                    mapActiveDocuments.Remove(uri);
                    mapActiveDocumentTitles.Remove(uri);
                }
            }

            if (bDispose)
            {
                if (workspace.ContextDocument == view.Document)
                    workspace.ContextDocument = null;

                view.Document.PropertyChanged -= Document_PropertyChanged;

                //List<RibbonTab> list = new List<RibbonTab>();
                //if (ribbonDebug != null)
                //    list.Add(ribbonDebug);

                //workspace.RemoveRibbonsTabItem(list, view.CommandBindings, TypeTitle, Colors.Red);
                workspace.RemoveDockingChildren(view);
                if (view.Document is IDisposable)
                    (view.Document as IDisposable).Dispose();
                view.Dispose();
            }

            return true;
        }

        void workspace_Closed(object sender, EventArgs e)
        {
            if (mapActiveDocuments.Count == 0)
                return;

            ReportDocument[] array = new ReportDocument[mapActiveDocuments.Values.Count];
            mapActiveDocuments.Values.CopyTo(array, 0);
            foreach (var doc in array)
            {
                ReportEditorUI view = doc.ActiveView as ReportEditorUI;
                if (view != null)
                    CloseReport(view, false);
            }
        }

        void workspace_Closing(object sender, CancelEventArgs e)
        {
            if (mapActiveDocuments.Count == 0)
                return;

            ReportDocument[] array = new ReportDocument[mapActiveDocuments.Values.Count];
            mapActiveDocuments.Values.CopyTo(array, 0);
            foreach (var doc in array)
            {
                ReportEditorUI view = doc.ActiveView as ReportEditorUI;
                if (view != null && !CanClose(view, true, false))
                {
                    e.Cancel = true;
                    break;
                }
            }
        }

        void workspace_ActiveWindowChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var viewOld = from entry in mapActiveDocuments/*.AsParallel()*/ where entry.Value.ActiveView == e.OldValue select entry.Value.ActiveView;
            var viewNew = from entry in mapActiveDocuments/*.AsParallel()*/ where entry.Value.ActiveView == e.NewValue select entry.Value.ActiveView;

            if (e.OldValue != null && e.OldValue is ReportEditorUI && viewOld != null)
            {
                var view = e.OldValue as ReportEditorUI;
                //List<RibbonTab> list = new List<RibbonTab>();
                //if (ribbonEditorRibbonTab != null)
                //    list.Add(ribbonEditorRibbonTab);
                //if (ribbonEditorRibbonTab != null)
                //    ribbonEditorRibbonTab.IsEnabled = false;

                //workspace.RemoveRibbonsTabItem(list, view.CommandBindings, TypeTitle, Colors.Red);
                //if (workspace.ContextDocument == view.Document)
                //    workspace.ContextDocument = null;

                var dockstate = workspace.GetElementDockState(view);
                if (dockstate == UFInterfaces.DockState.Document && !view.HasBeenLoaded)
                {
                    view.Visibility = Visibility.Collapsed;
                }

                //if (ribbonEditorRibbonTab != null)
                //{
                //    ribbonEditorRibbonTab.DataContext = null;
                //    ribbonEditorRibbonTab.fontNameBox.SelectionChanged -= fontNameBox_SelectionChanged;
                //    ribbonEditorRibbonTab.fontSizeBox.SelectionChanged -= fontSizeBox_SelectionChanged;
                //    ribbonEditorRibbonTab.borderWidhtBox.SelectionChanged -= borderWidhtBox_SelectionChanged;
                //}
            }
            //if (e.NewValue != null && e.NewValue is ReportEditorUI && viewNew != null)
            //{
            //    var view = e.NewValue as ReportEditorUI;
            //    //List<RibbonTab> list = new List<RibbonTab>();
            //    if (ribbonEditorRibbonTab == null)
            //    {
            //        ribbonEditorRibbonTab = new RibbonEditor();
            //        FontStyleHelper.InitializeFontComboBox(ribbonEditorRibbonTab.fontNameBox);
            //        FontStyleHelper.InitializeFontSizeComboBox(ribbonEditorRibbonTab.fontSizeBox);
            //        InitializeBorderWidthComboBox(ribbonEditorRibbonTab.borderWidhtBox);
            //    }
            //    ribbonEditorRibbonTab.fontNameBox.SelectionChanged += fontNameBox_SelectionChanged;
            //    ribbonEditorRibbonTab.fontSizeBox.SelectionChanged += fontSizeBox_SelectionChanged;
            //    ribbonEditorRibbonTab.borderWidhtBox.SelectionChanged += borderWidhtBox_SelectionChanged;

            //    //list.Add(ribbonEditorRibbonTab);
                
            //    // allow ribbon command binding with report editor UI
            //    ribbonEditorRibbonTab.DataContext = e.NewValue;

            //    //workspace.AddRibbonsTabItem(list, -1, view.CommandBindings, TypeTitle, Colors.Red);
            //    workspace.ContextDocument = view.Document;
            //    workspace.ContextObject = view.Document;

            //    var dockstate = workspace.GetElementDockState(view);
            //    if (dockstate == UFInterfaces.DockState.Document && view.HasBeenLoaded)
            //    {
            //        view.Visibility = Visibility.Visible;
            //    }
            //}
        }

        void fontNameBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var view = workspace.ActiveWindow as ReportEditorUI;
            if (view == null)
                return;
            //if (ribbonEditorRibbonTab.fontNameBox.SelectedValue != null && ribbonEditorRibbonTab.fontNameBox.SelectedValue is ComboBoxItem)
            //    view.OnFontNameSelectionChanged((ribbonEditorRibbonTab.fontNameBox.SelectedValue as ComboBoxItem).Content as FontFamily);
        }

        void fontSizeBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var view = workspace.ActiveWindow as ReportEditorUI;
            if (view == null)
                return;
            //if (ribbonEditorRibbonTab.fontSizeBox.SelectedValue != null && ribbonEditorRibbonTab.fontSizeBox.SelectedValue is ComboBoxItem)
            //    view.OnFontSizeSelectionChanged(Convert.ToInt16((ribbonEditorRibbonTab.fontSizeBox.SelectedValue as ComboBoxItem).Content.ToString()));
        }

        void borderWidhtBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var view = workspace.ActiveWindow as ReportEditorUI;
            if (view == null)
                return;
            //if (ribbonEditorRibbonTab.borderWidhtBox.SelectedValue != null && ribbonEditorRibbonTab.borderWidhtBox.SelectedValue is ComboBoxItem)
            //    view.OnBorderWidthSelectionChanged(Convert.ToInt16((ribbonEditorRibbonTab.borderWidhtBox.SelectedValue as ComboBoxItem).Content.ToString()));
        }

        private static void InitializeBorderWidthComboBox(ComboBox r_combo)
        {
            int[] sizes = new int[10] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
            for (int i = 0, cnt = sizes.Length; i < cnt; ++i)
            {
                ComboBoxItem item = new ComboBoxItem { Content = sizes[i] };
                r_combo.Items.Add(item);
            }
        }
#endif
        String GetDocumentTitle(Uri uri)
        {
            return Path.GetFileNameWithoutExtension(uri.GetPathString());
        }

        String GetDocumentTitle(String uri)
        {
            return Path.GetFileNameWithoutExtension(uri);
        }

#if !NET_STANDARD
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
                foreach (KeyValuePair<String, ReportDocument> keyvaluepair in mapActiveDocuments)
                {
                    if (keyvaluepair.Value != sender)
                        continue;

                    workspace.SetChangedDocumentTitle(keyvaluepair.Value.ActiveView, keyvaluepair.Value.NeedsSave);
                    break;
                }
            }
        }

        static void CreateDefaultDocument(Uri uri, IDocument parent, bool encryptFile = false)
        {
            using (var newProject = new ReportDocument { FullPath = uri.GetPathString(), Parent = parent })
            {
                newProject.SaveToFile(forceEncryption: encryptFile);
            }
        }

        public ReportEditorUI GetActiveView(Uri uri)
        {
            ReportDocument doc = null;
            if (mapActiveDocuments.TryGetValue(uri.GetPathString(), out doc))
                return doc.ActiveView as ReportEditorUI;
            return null;
        }
#endif

        #region IDocumentManager Members
#if !NET_STANDARD
        public void Edit(Uri uri, IDocument parent)
        {
            using (new WaitCursor())
            {
                if (!Properties.Settings.Default.UseEditorWPF)
                {
                    ReportDocument doc = null;
                    if (!mapActiveDocuments.TryGetValue(uri.GetPathString(), out doc))
                    {
                        doc = ReportDocument.FromFile(uri.GetPathString(), parent);
                        if (doc == null)
                            return;

                        doc.Parent = parent;
                        mapActiveDocuments.Add(uri.GetPathString(), doc);
                        mapActiveDocumentUris.Add(doc, uri.GetPathString());
                    }

                    var helper = new ReportDesignerToolHelper(doc, this);
                    helper.OpenReportDesignerTool();
                }
                else
                {
                    lock (lockObject)
                    {
                        ReportDocument doc = null;
                        if (mapActiveDocuments.TryGetValue(uri.GetPathString(), out doc) && doc.ActiveView != null)
                        {
                            if (doc.Parent == parent)
                                workspace.ActivateDockedElement(doc.ActiveView);
                            else
                            {
                                if (!CloseReport(doc.ActiveView as ReportEditorUI))
                                    return;
                                doc = null;
                            }
                        }

                        if (doc == null)
                            doc = ReportDocument.FromFile(uri.GetPathString(), parent);

                        if (doc != null && doc.ActiveView == null)
                        {
                            doc.Parent = parent;
                            ReportEditorUI reportEditor = new ReportEditorUI(this, doc);
                            doc.ActiveView = reportEditor;

                            workspace.SetDesiredHeightAndWidthInDockedMode(reportEditor, reportEditor.Height, reportEditor.Width);
                            reportEditor.ClearValue(FrameworkElement.WidthProperty);
                            reportEditor.ClearValue(FrameworkElement.HeightProperty);

                            BitmapImage bm = TypeIcon;

                            if (!mapActiveDocuments.ContainsKey(uri.GetPathString()))
                                mapActiveDocuments.Add(uri.GetPathString(), doc);

                            if (!mapActiveDocumentUris.ContainsKey(doc))
                                mapActiveDocumentUris.Add(doc, uri.GetPathString());

                            workspace.AddDockingChildren(reportEditor, String.Format("{0} ({1})", CreateDocumentTitle(uri, parent), parent.Title),
                                UFInterfaces.DockState.Document, UFInterfaces.DockSide.Left);
                            workspace.SetDockedElementIcon(reportEditor, new ImageBrush(bm));

                            workspace.ActivateDockedElement(reportEditor);

                            reportEditor.Document.PropertyChanged += Document_PropertyChanged;
                        }
                    }
                }
            }
        }

        public void Copy(Uri uri, String newPath, bool bCopy, IDocument parent, bool bUploading)
        {
            using (new WaitCursor())
            {
                lock (lockObject)
                {
                    ReportDocument doc = null;
                    if (mapActiveDocuments.TryGetValue(uri.GetPathString(), out doc))
                    {
                        if (!bCopy)
                            CloseReport(doc.ActiveView as ReportEditorUI);
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
                                    var view = doc.ActiveView as ReportEditorUI;
                                    if (view == null)
                                        return;
                                    view.SaveReportLayout();
                                    doc.SaveCurrentDocument();
                                }
                            }
                        }
                    }

                    ReportDocument.CopyFile(uri.GetPathString(), newPath, bCopy, parent);
                }
            }
        }

        public void Rename(Uri uri, String oldName, String newName, IDocument parent)
        {
            using (new WaitCursor())
            {
                lock (lockObject)
                {
                    ReportDocument doc = null;
                    bool bReopen = false;
                    mapActiveDocuments.TryGetValue(uri.GetPathString(), out doc);
                    if (doc != null && doc.ActiveView is ReportEditorUI)
                    {
                        bReopen = true;
                        if (!CloseReport(doc.ActiveView as ReportEditorUI))
                            return;
                    }
                    else if (doc != null)
                    {
                        doc.PropertyChanged -= Document_PropertyChanged;
                        if (doc is IDisposable)
                            (doc as IDisposable).Dispose();
                    }

                    if (mapActiveDocuments.ContainsKey(uri.GetPathString()))
                        mapActiveDocuments.Remove(uri.GetPathString());
                    if (doc != null && mapActiveDocumentUris.ContainsKey(doc))
                        mapActiveDocumentUris.Remove(doc);
                    if (mapActiveDocumentTitles.ContainsKey(uri.GetPathString()))
                        mapActiveDocumentTitles.Remove(uri.GetPathString());

                    var newname = ReportDocument.RenameFile(uri.GetPathString(), oldName, newName, parent.fileSystemProviderBase);

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
                    ReportDocument doc = null;
                    if (mapActiveDocuments.TryGetValue(uri.GetPathString(), out doc))
                    {
                        if (doc.ActiveView is ReportEditorUI)
                            CloseReport(doc.ActiveView as ReportEditorUI, false);
                        else
                        {
                            doc.PropertyChanged -= Document_PropertyChanged;
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

                    ReportDocument.RemoveFile(uri.GetPathString(), parent.fileSystemProviderBase);
                }
            }
        }
#endif
        public void Execute(Uri uri, IDocument parent, ExecutionMode mode, Object Context)
        {
            ReportDocument doc = null;
            ReportExecuter executer = null;
            String title = null;
            ReportService.DefaultSourceType sourceType = ReportService.DefaultSourceType.Undefined;
            var rootParent = DocumentHelper.GetRootParent(parent, traverse: false);
            var map = Context as IDictionary<String, Object>;
            if (map != null && map.ContainsKey("ReportAlarmDoc"))
            {
#if !DEBUG
                var state = MSZ.MSZView.GetModules("KXS7y64HI+TAGrwcJgBbSp8rwADghe5thN77IIPXmuqG+ab5543ZRBCCsCBJbBNx/IoqCaj13T3tqZw/KR6gYQ=="/* STA */);
                if (state == false)
                {
                    logLicense.Warn(Properties.Resources.NoStatisticsLicense);
                    return;
                }
#endif

                title = Properties.Resources.StatisticReport;
                doc = map["ReportAlarmDoc"] as ReportDocument;
                sourceType = ReportService.DefaultSourceType.EventLog;

                if (doc == null)
                {
                    logGeneral.ErrorFormat(Properties.Resources.MissingParameterExecutingReportCommand, title);
                    return;
                }

                lock (lockObject)
                {
                    if (mapRunningReportsPerParent.ContainsKey(rootParent))
                    {
                        var executers = (from c in mapRunningReportsPerParent[rootParent]
                                            where c.reportDocument.Title == doc.Title
                                            select c).ToList();
                        if (executers.Count > 0)
                            executer = executers[0];
                    }
                }

                if (executer != null)
                {
                    doc.Dispose();
                    doc = executer.reportDocument;
                }
                else
                    doc.Parent = parent;
            }
            else 
            {
#if !DEBUG
                var state = MSZ.MSZView.GetModules("KXS7y64HI+TAGrwcJgBbSp8rwADghe5thN77IIPXmuqG+ab5543ZRBCCsCBJbBNxcPspvRaFavRuuz2KyDjJ8w=="/* REP */);
                if (state == false)
                {
                    logLicense.Warn(Properties.Resources.NoRecipeLicense);
                    return;
                }
#endif

                uri = parent.MakeAbosoluteUri(uri);
                parent = parent.UpdateParentFromUri(uri);
                lock (lockObject)
                {
                    if (mapRunningReportsPerParent.ContainsKey(rootParent))
                    {
                        var executers = (from c in mapRunningReportsPerParent[rootParent]
                                         where c.reportDocument.FullPath == uri.GetPathString()
                                         select c).ToList();
                        if (executers.Count > 0)
                            executer = executers[0];
                    }
                }

                if (executer != null)
                    doc = executer.reportDocument;
                if (doc == null)
                {
                    if (!ReportDocument.ExistFile(uri.GetPathString()
#if !NET_STANDARD
                    , parent.fileSystemProviderBase
#endif
                        ))
                    {
#if !NET_STANDARD
                        if (UIInterface != null)
                        {
                            UIInterface.ShowError(String.Format(Properties.Resources.DocNotFound,
                                GetDocumentTitle(uri)));
                        }
#else
                        logGeneral.ErrorFormat(Properties.Resources.DocNotFound, GetDocumentTitle(uri));
#endif
                        return;
                    }

                    doc = ReportDocument.FromFile(uri.GetPathString(), parent);
                    doc.Parent = parent;
                }
            }

            if (doc == null)
            {
#if !NET_STANDARD
                UIInterface.ShowError(String.Format(Properties.Resources.CouldNotLocateUri, uri.GetPathString()));
#else
                logGeneral.ErrorFormat(Properties.Resources.CouldNotLocateUri, uri.GetPathString());
#endif
                return;
            }

            if (executer == null)
            {
                lock (lockObject)
                {
                    if (!mapRunningReports.TryGetValue(doc, out executer))
                    {
                        executer = new ReportExecuter(this, doc, sourceType);
                        mapRunningReports.Add(doc, executer);
                    }

                    if (!mapRunningReportsPerParent.ContainsKey(rootParent))
                        mapRunningReportsPerParent.Add(rootParent, new List<ReportExecuter>());
                    if (!mapRunningReportsPerParent[rootParent].Contains(executer))
                        mapRunningReportsPerParent[rootParent].Add(executer);
                }
            }

            executer.Start(parent, mode, Context, title);
        }

        public void PreTerminate(Uri uri, IDocument parent)
        { }

        public void Terminate(Uri uri, IDocument parent)
        {
            var rootParent = DocumentHelper.GetRootParent(parent, traverse: false);

            List<ReportExecuter> recipeExecuters = null;
            lock (lockObject)
            {
                if (mapRunningReportsPerParent.ContainsKey(rootParent))
                {
                    recipeExecuters = new List<ReportExecuter>(mapRunningReportsPerParent[rootParent]);
                    mapRunningReportsPerParent.Remove(rootParent);
                }
            }

            if (recipeExecuters != null)
            {
                recipeExecuters.ForEach(executer =>
                {
                    var doc = executer.reportDocument;
                    executer.Dispose();
                    doc.Dispose();
                    lock (lockObject)
                        mapRunningReports.Remove(doc);
                });
            }
        }

#if !NET_STANDARD
        public void SaveAllChild(IDocument parent)
        {
            var list = (from c in mapActiveDocuments.Values// .AsParallel()
                        where c.Parent == parent && c.ActiveView != null 
                        select c).ToList();

            list.ForEach(document =>
            {
                (document.ActiveView as ReportEditorUI).SaveReportLayout();
                document.SaveToFile();
            });
        }

        public bool CloseAllChild(IDocument parent, bool bParentClosing = false)
        {
            var list = (from c in mapActiveDocuments.Values// .AsParallel()
                        where c.Parent == parent && c.ActiveView != null 
                        select c).ToList();

            foreach (var document in list)
            {
                if (!CloseReport(document.ActiveView as ReportEditorUI))
                    return false;
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

        public bool SaveDocument(IDocument parent, Uri uri, bool encryptFile = false)
        {
            if (mapActiveDocuments.ContainsKey(uri.GetPathString()))
                return mapActiveDocuments[uri.GetPathString()].SaveToFile(forceEncryption: encryptFile);
            var doc = ReportDocument.FromFile(uri.GetPathString(), parent);
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

        public ObservableCollection<IDocumentManager> GetChildDocumentManagers()
        {
            return null;
        }

#if !NET_STANDARD
        public ObservableCollection<IDocumentManager> GetChildDocumentManagers(Uri uri, IDocument parent)
        {
            ReportDocument doc = null;
            if (!mapActiveDocuments.TryGetValue(uri.GetPathString(), out doc))
            {
                doc = ReportDocument.FromFile(uri.GetPathString(), parent);
                if (doc == null)
                    return null;
                doc.Parent = parent;
                mapActiveDocuments.Add(uri.GetPathString(), doc);
                mapActiveDocumentUris.Add(doc, uri.GetPathString());
            }

            return null;
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
                return Properties.Settings.Default.TypeLabel;
            }
        }

#if !NET_STANDARD
        public BitmapImage TypeIcon
        {
            get
            {
                return GetControlImage("RMEditorSmall");
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
                return GetControlImage("RMEditor");
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
                Assembly assembly = Assembly.GetExecutingAssembly();
                return Path.GetFileNameWithoutExtension(assembly.Location);
            }
        }

        public String FileType
        {
            get
            {
                return Properties.Settings.Default.DefaultFileExt;
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

        public bool RegisterFileType
        {
            get { return true; }
        }

#if !NET_STANDARD
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

#if !NET_STANDARD
        public bool IsResourceExpandable(IDocument document = null)
        {
            return false;
        }
#endif

        public bool IsStartupControllerAware
        {
            get
            {
                return false;
            }
        }

#if !NET_STANDARD
        public Uri CreateNewDocument(Uri relative, IDocument parent, bool encryptFile = false)
        {
            String path, newReportName;
            int i = 0;
            Uri url = null;
            if (parent != null && parent.fileSystemProviderBase != null)
            {
                do
                {
                    newReportName = String.Format("{0}{1}", Properties.Settings.Default.DefaultReportName, ++i);
                    path = String.Format("{0}{1}{2}", relative.OriginalString, newReportName, Properties.Settings.Default.DefaultFileExt);
                } while (parent.fileSystemProviderBase.Exists(new FileManagerFile(parent.fileSystemProviderBase, path)));

                url = new Uri(path, UriKind.RelativeOrAbsolute);
                CreateDefaultDocument(url, parent, encryptFile);
                return url;
            }

            if (relative.IsAbsoluteUri && Path.HasExtension(relative.OriginalString))
            {
                var ret = Path.ChangeExtension(relative.OriginalString, Properties.Settings.Default.DefaultFileExt);
                var uri = new Uri(ret, UriKind.RelativeOrAbsolute);
                CreateDefaultDocument(uri, parent, encryptFile);
                return uri;
            }

            do
            {
                newReportName = String.Format("{0}{1}", Properties.Settings.Default.DefaultReportName, ++i);
                path = String.Format("{0}{1}{2}", relative.OriginalString, newReportName, Properties.Settings.Default.DefaultFileExt);
            } while (File.Exists(path));

            if (path.StartsWith("\\"))
                url = new Uri(path);
            else
                url = new Uri(String.Format("{0}://{1}", TypeScheme, path), UriKind.RelativeOrAbsolute);
            CreateDefaultDocument(url, parent, encryptFile);
            return url;
        }
#endif

        public Type DocumentType
        {
            get
            {
                return typeof(ReportDocument);
            }
        }
        
#if !NET_STANDARD
        public IToolbar GetToolbar()
        {
            if (menuControl == null)
                menuControl = new MenuControl();
            return menuControl;
        }

        public IList<System.Windows.Input.ICommand> GetAlwaysAvailableCommand()
        {
            return null;
        }
#endif
        #endregion IDocumentManager Members

        #region Properties
#if !NET_STANDARD
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
#endif

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
        
        #endregion Properties
        
        #region IDisposable Members

        void IDisposable.Dispose()
        {
            mapActiveDocuments.Clear();
            mapActiveDocumentUris.Clear();

            lock (lockObject)
            {
                foreach (ReportExecuter recipe in mapRunningReports.Values)
                    recipe.Dispose();
                mapRunningReports.Clear();
                mapRunningReportsPerParent.Clear();
            }

#if !NET_STANDARD
            if (workspace != null)
            {
                workspace.Closed -= workspace_Closed;
                workspace.Closing -= workspace_Closing;
                workspace.CloseButtonClick -= workspace_CloseButtonClick;
                workspace.ActiveWindowChanged -= workspace_ActiveWindowChanged;
            }

            if (PropertyControl != null)
                PropertyControl.AcceptChanges -= PropertyControl_AcceptChanges;
#endif
        }
#if !NET_STANDARD
        #region ICrossReference
        public List<CrossReferenceResultModel> GetCRObjects(CrossReferenceModel model)
        {
            var result = new List<UFInterfaces.Editors.CrossReferenceResultModel>();
            if (model.ResourceList == null || model.QuitEvent.IsCancellationRequested)
                return result;
            bool getConnections = model.CRManagement.CrossReferenceTypeList.Contains(CrossReferenceType.Connections);
            if (!getConnections)
                return result;

            IUFUAEditorManager uFUAEditorManager = GetService(typeof(IUFUAEditorManager)) as IUFUAEditorManager;
            DataReaderEditor.Converters.DataReaderModelConverter dataReaderModelConverter = null;
            string histDefConnectionString = null;
            var p = DocumentManager.ComponentService.Helpers.DocumentHelper.GetRootParent(model.Parent, traverse: false);
            string pTitle = p.Title;
            var path = model.Parent.fileSystemProviderBase != null ? string.Empty : p.rootBase;
            List<String> list = model.ResourceList as List<String>;

            string docType = DocManagerType.ReportManager.ToString();
            Parallel.ForEach(list, (resource, loopstate) =>
            {
                if (model.QuitEvent.IsCancellationRequested)
                    loopstate.Break();
                var uri = new Uri($"{path}\\{resource}", UriKind.RelativeOrAbsolute);
                var pathString = uri.GetPathString();
                using (ReportDocument doc = GetDocument(model.Parent, uri))
                {
                    if (doc != null)
                    {
                        string connectionString = null;
                        if (doc.ReaderItemSources != null && !string.IsNullOrEmpty(doc.ReaderItemSources.Connection))
                        {
                            if (dataReaderModelConverter == null)
                                dataReaderModelConverter = new DataReaderEditor.Converters.DataReaderModelConverter();
                            connectionString = dataReaderModelConverter.Convert(doc.ReaderItemSources, typeof(String), null, System.Globalization.CultureInfo.CurrentCulture)?.ToString();
                        }
                        else
                        {
                            if (string.IsNullOrEmpty(histDefConnectionString))
                                histDefConnectionString = uFUAEditorManager?.GetHistorianDefaultConnection(doc);
                            connectionString = histDefConnectionString;
                        }

                        if (doc.ReaderItemSources != null)
                            lock (result)
                                result.Add(new UFInterfaces.Editors.CrossReferenceResultModel()
                                {
                                    RelativePath = XpoHelpers.XpoHelper.NormalizeConnectionString(connectionString, doc.rootBase), //$"{TypeTitle}\\{doc.Title}",
                                    Name = Properties.Resources.RefrencedBy, //$"{doc.Title} - {Properties.Resources.DefConnectionString}",
                                    AppName = $"{TypeTitle}",
                                    CReferenceType = CrossReferenceType.Connections,
                                    Description = doc.Title, //doc.ReaderItemSources.Connection,
                                    Settings = string.Format("{0}|{1}|{2}", docType, doc.rootBase, Properties.Resources.RefrencedBy),
                                    ContainerDoc = TypeScheme,
                                    IconType = docType
                                });
                    }
                }
            });

            return result;
        }

        public ReportDocument GetDocument(IDocument parent, Uri uri, bool bCreate = true)
        {
            var p = DocumentManager.ComponentService.Helpers.DocumentHelper.GetRootParent(parent, traverse: false);
            ReportDocument doc = null;
            doc = ReportDocument.FromFile(uri.GetPathString(), parent, create:bCreate);
            if (doc != null)
                doc.Parent = p;
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
                return false;
            }
        }

        public void RenameCRObjects(CrossReferenceModel model)
        {
           
        }

        public void EditCRObject(IDocument parent, string settings)
        {
           
        }
        #endregion
#endif
        #endregion IDisposable Members
    }
}