using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using DocumentManager.ComponentService;
using PropertyControl.ComponentService;
using ScriptManager.Document;
using Toolbox.ComponentService;
using Tracing.ComponentService;
using UFInterfaces;
using UFInterfaces.CoreHostComponents;
using UIMsgBoxAlertService.ComponentService;
using Utilities;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using VFS;
using UFUAEditor.ComponentService;
using log4net;
using WinWrap.Basic;
using System.Runtime.Serialization;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using DocumentManager.ComponentService.Helpers;

namespace ScriptManager.ComponentService
{
    public class ScriptManagerComponent : ComponentBase<IScriptManager>, IScriptManager, IDocumentManager, IDisposable, ICrossReference
    {
        #region Declaration

        readonly Object lockObject = new Object();

        readonly Dictionary<String, ScriptDocument> mapActiveDocuments = new Dictionary<String, ScriptDocument>(StringComparer.OrdinalIgnoreCase);
        readonly Dictionary<ScriptDocument, String> mapActiveDocumentUris = new Dictionary<ScriptDocument, String>();
        readonly Dictionary<String, String> mapActiveDocumentTitles = new Dictionary<String, String>(StringComparer.OrdinalIgnoreCase);

        readonly Dictionary<ScriptDocument, ScriptExecuter> mapRunningScripts = new Dictionary<ScriptDocument, ScriptExecuter>();
        readonly Dictionary<IDocument, List<ScriptExecuter>> mapRunningScriptsPerParent = new Dictionary<IDocument, List<ScriptExecuter>>();
        readonly List<IDocument> listTerminatingDocument = new List<IDocument>();


        private static readonly ILog logLicense = LogManager.GetLogger(Properties.Resources.LicenseManager);
        public static ScriptManagerComponent scriptManagerComponent { get; protected set; }
        MenuControl menuControl;
        #endregion Declaration

        #region IUFInterfaceBase Members

        void IUFInterfaceBase.Initialize()
        {
            if (scriptManagerComponent == null)
                scriptManagerComponent = this;
            GetComponentInterfaces();
        }

        #endregion IUFInterfaceBase Members

        private static BitmapImage GetControlImage(String image, bool bShared = false)
        {
            var bm = SharedResources.Helpers.ResourceManager.GetCommonImage(scriptManagerComponent.TypeLabel, image, bShared);
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
                PropertyControl.AcceptChanges += PropertyControl_AcceptChanges;
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
            if (!(e.TargetItem is ScriptEditorUI))
                return;

            ScriptEditorUI view = e.TargetItem as ScriptEditorUI;
            if (!CloseScript(view))
                e.Cancel.Cancel = true;
        }

        private bool CanClose(ScriptEditorUI view, bool bSave = true)
        {
            String uri;
            view.UpdateBreakPointState();
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
                        if (!view.Document.SaveCurrentDocument())
                            return false;
                    }
                }
            }

            return true;
        }

        private bool CloseScript(ScriptEditorUI view, bool bSave = true, bool bDispose = true)
        {
            if (view == null || view.Document == null)
                return true;

            var doc = view.Document;

            String uri;
            view.UpdateBreakPointState();
            if (mapActiveDocumentUris.TryGetValue(doc, out uri))
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
                        doc.SaveCurrentDocument();
                }

                if (bDispose)
                {
                    mapActiveDocumentUris.Remove(doc);
                    mapActiveDocuments.Remove(uri);
                    mapActiveDocumentTitles.Remove(uri);
                }
            }

            if (bDispose)
            {
                if (workspace.ContextDocument == doc)
                    workspace.ContextDocument = null;
                if (workspace.ContextObject == doc)
                    workspace.ContextObject = null;

                doc.PropertyChanged -= Document_PropertyChanged;

                bClosingScript = true;
                workspace.RemoveDockingChildren(view);
                bClosingScript = false;
                // view.Dispose();
                if (doc is IDisposable)
                    (doc as IDisposable).Dispose();
                view.Dispose();
            }

            return true;
        }

        void workspace_Closed(object sender, EventArgs e)
        {
            if (mapActiveDocuments.Count == 0)
                return;

            ScriptDocument[] array = new ScriptDocument[mapActiveDocuments.Values.Count];
            mapActiveDocuments.Values.CopyTo(array, 0);
            foreach (var doc in array)
            {
                ScriptEditorUI view = doc.ActiveView as ScriptEditorUI;
                CloseScript(view, false);
            }
        }

        void workspace_Closing(object sender, CancelEventArgs e)
        {
            if (mapActiveDocuments.Count == 0)
                return;

            ScriptDocument[] array = new ScriptDocument[mapActiveDocuments.Values.Count];
            mapActiveDocuments.Values.CopyTo(array, 0);
            foreach (var doc in array)
            {
                ScriptEditorUI view = doc.ActiveView as ScriptEditorUI;
                if (view != null && !CanClose(view))
                {
                    e.Cancel = true;
                    break;
                }
            }
        }

        FrameworkElement elementAutoHiddenToRestore;
        bool bClosingScript;
        void workspace_ActiveWindowChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var viewOld = from entry in mapActiveDocuments/*.AsParallel()*/ where entry.Value.ActiveView == e.OldValue select entry.Value.ActiveView;
            var viewNew = from entry in mapActiveDocuments/*.AsParallel()*/ where entry.Value.ActiveView == e.NewValue select entry.Value.ActiveView;

            if (e.OldValue != null && e.OldValue is ScriptEditorUI && viewOld != null)
            {
                var view = e.OldValue as ScriptEditorUI;
                //if (workspace.ContextDocument == view.Document)
                //    workspace.ContextDocument = null;

                var dockstate = workspace.GetElementDockState(view);
                if (dockstate == UFInterfaces.DockState.Document && !view.IsLoaded)
                {
                    view.Visibility = Visibility.Collapsed;
                }

                if (!bClosingScript && e.NewValue != null && e.NewValue is FrameworkElement)
                {
                    var newActive = e.NewValue as FrameworkElement;
                    var dockstateActive = workspace.GetElementDockState(newActive);
                    if (dockstateActive == UFInterfaces.DockState.AutoHidden)
                    {
                        workspace.SetElementDockState(newActive, UFInterfaces.DockState.Dock);
                        elementAutoHiddenToRestore = newActive;
                    }
                }
            }
            if (e.NewValue != null && e.NewValue is ScriptEditorUI && viewNew != null)
            {
                var view = e.NewValue as ScriptEditorUI;
                workspace.ContextDocument = view.Document;
                workspace.ContextObject = view.Document;

                var dockstate = workspace.GetElementDockState(view);
                if (dockstate == UFInterfaces.DockState.Document && view.IsLoaded)
                {
                    view.Visibility = Visibility.Visible;
                }

                if (elementAutoHiddenToRestore != null)
                {
                    workspace.SetElementDockState(elementAutoHiddenToRestore, UFInterfaces.DockState.AutoHidden);
                    elementAutoHiddenToRestore = null;
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
                foreach (KeyValuePair<String, ScriptDocument> keyvaluepair in mapActiveDocuments)
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
            using (var newProject = new ScriptDocument
            {
                FullPath = uri.GetPathString(), Parent = parent })
            {
                var str = Properties.Settings.Default.ScriptDefaultText;
                str = str.Replace("'newline'", Environment.NewLine);
                newProject.Code = str;
                newProject.SaveToFile(forceEncryption: encryptFile);
            }
        }
        #region ICrossReference
        public List<UFInterfaces.Editors.CrossReferenceResultModel> GetCRObjects(UFInterfaces.Editors.CrossReferenceModel model)
        {
            var result = new List<UFInterfaces.Editors.CrossReferenceResultModel>();
            if (model.ResourceList == null || model.QuitEvent.IsCancellationRequested)
                return result;
            bool getTags = model.CRManagement.CrossReferenceTypeList.Contains(CrossReferenceType.Tags);
            if (!getTags)
                return result;
            var p = DocumentHelper.GetRootParent(model.Parent, traverse: false);
            string path = model.Parent.fileSystemProviderBase != null ? string.Empty : p.rootBase;
            List<String> list = model.ResourceList as List<String>;
            string docType = DocManagerType.ScriptManager.ToString();
            for (int i = 0; i < list.Count(); i++)
            {
                var resource = list[i];
                if (model.QuitEvent.IsCancellationRequested)
                    return result;
                var uri = new Uri(string.Format("{0}\\{1}", path, resource), UriKind.RelativeOrAbsolute);
                using (ScriptDocument doc = CreateDocument(model.Parent, uri))
                {
                    if (doc != null)
                    {
                        var _folder = resource.Replace('/', '\\').Remove(resource.LastIndexOf(".Script"));

                        if (doc.CurrentStatusTag != null)
                        {
                            lock (result)
                            {
                                result.Add(new UFInterfaces.Editors.CrossReferenceResultModel()
                                {
                                    RelativePath = doc.CurrentStatusTag.RelativePath,
                                    Name = doc.CurrentStatusTag.Name,
                                    AppName = doc.CurrentStatusTag.AppName,
                                    ReferencedNodeId = doc.CurrentStatusTag.ResolvedNodeId?.Identifier.ToString(),
                                    EndpointUrl = doc.CurrentStatusTag.EndpointUrl,
                                    CReferenceType = CrossReferenceType.Tags,
                                    Description = _folder,
                                    Settings = string.Format("{0}|{1}", DocManagerType.ScriptManager, doc.FilePath),
                                    ContainerDoc = TypeScheme,
                                    IconType = docType
                                });
                            }
                        }

                        if (doc.CycleTimeTag != null)
                        {
                            lock (result)
                            {
                                result.Add(new UFInterfaces.Editors.CrossReferenceResultModel()
                                {
                                    RelativePath = doc.CycleTimeTag.RelativePath,
                                    Name = doc.CycleTimeTag.Name,
                                    AppName = doc.CycleTimeTag.AppName,
                                    ReferencedNodeId = doc.CycleTimeTag.ResolvedNodeId?.Identifier.ToString(),
                                    EndpointUrl = doc.CycleTimeTag.EndpointUrl,
                                    CReferenceType = CrossReferenceType.Tags,
                                    Description = _folder,
                                    Settings = string.Format("{0}|{1}", DocManagerType.ScriptManager, doc.FilePath),
                                    ContainerDoc = TypeScheme,
                                    IconType = docType
                                });
                            }
                        }
                        var collectionrefList = doc.GetListUsedVariablesFromPersistence(model, true);
                        foreach (var collectionref in collectionrefList)
                        {
                            if (model.QuitEvent.IsCancellationRequested)
                                return result;
                            OPCUAViewModel.OPCUAEntityReference refdetails = collectionref as OPCUAViewModel.OPCUAEntityReference;
                            if (refdetails != null && !string.IsNullOrEmpty(refdetails.RelativePath))
                            {
                                var x = refdetails;
                                lock (result)
                                {
                                    result.Add(new UFInterfaces.Editors.CrossReferenceResultModel()
                                    {
                                        RelativePath = refdetails.RelativePath,
                                        Name = refdetails.Name,
                                        AppName = refdetails.AppName,
                                        ReferencedNodeId = refdetails.ResolvedNodeId?.Identifier.ToString(),
                                        EndpointUrl = refdetails.EndpointUrl,
                                        CReferenceType = CrossReferenceType.Tags,
                                        Description = _folder,
                                        Settings = string.Format("{0}|{1}", DocManagerType.ScriptManager, doc.FilePath),
                                        ContainerDoc = TypeScheme,
                                        IconType = docType
                                    });
                                }
                            }
                        }
                    }
                }
            }

            return result;
        }
        public void EditCRObject(IDocument parent, string settings)
        {
            var p = DocumentHelper.GetRootParent(parent, traverse: false);
            string[] path = settings.Split('|');
            if (path.Length >= 2)
            {
                var uri = new Uri(path[1], UriKind.RelativeOrAbsolute);
                Edit(uri, p);
            }
        }
        #endregion
        private ScriptDocument CreateDocument(IDocument parent, Uri uri)
        {
            var p = DocumentHelper.GetRootParent(parent, traverse: false);
            ScriptDocument doc = null;
            doc = ScriptDocument.FromFile(uri.GetPathString(), parent);
            if (doc != null)
                doc.Parent = p;
            return doc;
        }

        private ScriptDocument GetOrCreateDocumentFromUri(IDocument parent, Uri uri)
        {
            var p = DocumentHelper.GetRootParent(parent, traverse: false);
            ScriptDocument doc = null;
            if (!mapActiveDocuments.TryGetValue(uri.GetPathString(), out doc))
            {
                doc = ScriptDocument.FromFile(uri.GetPathString(), parent);
                if (doc != null)
                {
                    doc.Parent = p;
                    mapActiveDocuments.Add(uri.GetPathString(), doc);
                    mapActiveDocumentUris.Add(doc, uri.GetPathString());
                }
            }
            return doc;
        }

        #region IDocumentManager Members

        public void Edit(Uri uri, IDocument parent)
        {
            using (new WaitCursor())
            {
                lock (lockObject)
                {
                    ScriptDocument doc = null;
                    if (mapActiveDocuments.TryGetValue(uri.GetPathString(), out doc) && doc.ActiveView != null)
                    {
                        if (doc.Parent == parent)
                            workspace.ActivateDockedElement(doc.ActiveView);
                        else
                        {
                            if (!CloseScript(doc.ActiveView as ScriptEditorUI))
                                return;
                            doc = null;
                        }
                    }
                    
                    if (doc == null)
                    {
                        doc = ScriptDocument.FromFile(uri.GetPathString(), parent);
                        if (doc == null)
                            return;
                        mapActiveDocuments.Add(uri.GetPathString(), doc);
                        mapActiveDocumentUris.Add(doc, uri.GetPathString());
                    }

                    if (doc != null && doc.ActiveView == null)
                    { 
                        ScriptEditorUI scriptEditor = new ScriptEditorUI(this, doc);
                        doc.Parent = parent;
                        doc.ActiveView = scriptEditor;

                        BasicIdeCtl basicIdeCtl = (doc.ActiveView as ScriptEditorUI).basicIdeCtl;

                        workspace.SetDesiredHeightAndWidthInDockedMode(scriptEditor, scriptEditor.Height, scriptEditor.Width);
                        scriptEditor.ClearValue(FrameworkElement.WidthProperty);
                        scriptEditor.ClearValue(FrameworkElement.HeightProperty);

                        BitmapImage bm = TypeIcon;

                        workspace.AddDockingChildren(scriptEditor, String.Format("{0} ({1})", CreateDocumentTitle(uri, parent), parent.Title),
                            UFInterfaces.DockState.Document, UFInterfaces.DockSide.Left);
                        workspace.SetDockedElementIcon(scriptEditor, new ImageBrush(bm));

                        workspace.ActivateDockedElement(scriptEditor);

                        scriptEditor.Document.PropertyChanged += Document_PropertyChanged;
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
                    ScriptDocument doc = null;
                    if (mapActiveDocuments.TryGetValue(uri.GetPathString(), out doc))
                    {
                        if (!bCopy)
                            CloseScript(doc.ActiveView as ScriptEditorUI);
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
                                    doc.SaveCurrentDocument();
                                }
                            }
                        }
                    }

                    ScriptDocument.CopyFile(uri.GetPathString(), newPath, bCopy, parent);
                }
            }
        }

        public void Rename(Uri uri, String oldName, String newName, IDocument parent)
        {
            using (new WaitCursor())
            {
                lock (lockObject)
                {
                    ScriptDocument doc = null;
                    bool bReopen = false;
                    mapActiveDocuments.TryGetValue(uri.GetPathString(), out doc);
                    if (doc != null && doc.ActiveView is ScriptEditorUI)
                    {
                        bReopen = true;
                        if (!CloseScript(doc.ActiveView as ScriptEditorUI))
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

                    var newname = ScriptDocument.RenameFile(uri.GetPathString(), oldName, newName, parent.fileSystemProviderBase);

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
                    ScriptDocument doc = null;
                    if (mapActiveDocuments.TryGetValue(uri.GetPathString(), out doc))
                    {
                        if (doc.ActiveView is ScriptEditorUI)
                            CloseScript(doc.ActiveView as ScriptEditorUI, false);
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

                    ScriptDocument.RemoveFile(uri.GetPathString(), parent.fileSystemProviderBase);
                }
            }
        }

        public void Execute(Uri uri, IDocument parent, ExecutionMode mode, Object Context)
        {
#if !DEBUG
            var enableVB = MSZ.MSZView.GetModules("KXS7y64HI+TAGrwcJgBbSp8rwADghe5thN77IIPXmuqG+ab5543ZRBCCsCBJbBNxN+rOYP4u2+aLWnhBhIXJ4A=="/* VB */);
            if (enableVB == false)
            {
                logLicense.Warn(Properties.Resources.NoVBLicense);
                return;
            }
#endif
            ScriptExecuter executer = null;
            ScriptDocument doc = null;
            var docParent = DocumentHelper.GetRootParent(parent, traverse: false);

            lock (lockObject)
            {
                if (listTerminatingDocument.Contains(docParent))
                    return;

                System.Diagnostics.Debug.WriteLine(String.Format("Executer Script {0}, {1}, mode {2}", uri, docParent.Title, mode));

                uri = docParent.MakeAbosoluteUri(uri);
                docParent = docParent.UpdateParentFromUri(uri);

                ScriptExecuter currentExecuter = null;
                if (mapRunningScriptsPerParent.ContainsKey(docParent))
                {
                    var executers = (from c in mapRunningScriptsPerParent[docParent] 
                                     where c.Document.FullPath == uri.GetPathString() 
                                     select c).ToList();
                    if (executers.Count > 0)
                    {
                        currentExecuter = executers[0];
                        System.Diagnostics.Debug.WriteLine(String.Format("Executer Found Script {0}, {1}, mode {2}", uri, docParent.Title, mode));
                    }
                }

                if (currentExecuter != null)
                    doc = currentExecuter.Document;
                else // if (!mapActiveDocuments.TryGetValue(uri.GetPathString(), out doc))
                {
                    if (!ScriptDocument.ExistFile(uri.GetPathString(), docParent.fileSystemProviderBase))
                    {
                        if (UIInterface != null)
                        {
                            UIInterface.ShowError(String.Format(Properties.Resources.DocNotFound,
                                GetDocumentTitle(uri)));
                        }
                        return;
                    }

                    doc = ScriptDocument.FromFile(uri.GetPathString(), docParent);
                    if (doc == null)
                        return;
                    doc.Parent = docParent;
                    // mapActiveDocuments.Add(uri.GetPathString(), doc);
                }

                if (mode == ExecutionMode.Stop)
                {
                    mapRunningScripts.TryGetValue(doc, out executer);
                }
                else
                {
                    if (!mapRunningScripts.TryGetValue(doc, out executer))
                    {
                        executer = new ScriptExecuter(doc, this);
                        System.Diagnostics.Debug.WriteLine(String.Format("Executer Created Script {0}, {1}, mode {2}", uri, docParent.Title, mode));

                        mapRunningScripts.Add(doc, executer);
                    }

                    if (!mapRunningScriptsPerParent.ContainsKey(docParent))
                        mapRunningScriptsPerParent.Add(docParent, new List<ScriptExecuter>());
                    if (!mapRunningScriptsPerParent[docParent].Contains(executer))
                        mapRunningScriptsPerParent[docParent].Add(executer);
                }
            }

            if (executer != null)
            {
                if (mode != ExecutionMode.Stop && mode != ExecutionMode.Synchro)
                {
                    if (mode != executer.LastExecutionMode)
                        executer.Stop();
                    executer.Start(docParent, mode, Context);
                }
                if (mode == ExecutionMode.Synchro)
                {
                    executer.Stop();
                    executer.Start(docParent, mode, Context);
                }
                if (mode == ExecutionMode.Stop)
                {
                    executer.Stop();
                    /*
                    lock (lockObject)
                    {
                        mapRunningScripts.Remove(doc);
                        doc.Dispose();
                        if (mapRunningScriptsPerParent.ContainsKey(Parent))
                        {
                            mapRunningScriptsPerParent[Parent].Remove(executer);
                        }
                    }
                    */
                }
            }
        }

        public void PreTerminate(Uri uri, IDocument parent)
        {
            var docParent = DocumentHelper.GetRootParent(parent, traverse: false);
            lock (lockObject)
            {
                if (!listTerminatingDocument.Contains(docParent))
                    listTerminatingDocument.Add(docParent);
            }
        }

        public void Terminate(Uri uri, IDocument parent)
        {
            var docParent = DocumentHelper.GetRootParent(parent, traverse: false);
            if (mapRunningScriptsPerParent.ContainsKey(docParent))
            {
                mapRunningScriptsPerParent[docParent].ForEach(script => 
                    {
                        script.Stop(true);
                        var docs = (from c in mapRunningScripts where c.Value == script select c.Key).ToList();
                        if (docs.Count > 0)
                        {
                            mapRunningScripts.Remove(docs[0]);
                            docs[0].Dispose();
                        }
                    });
                mapRunningScriptsPerParent.Remove(docParent);
            }

            listTerminatingDocument.Remove(docParent);
        }

        public bool SaveDocument(IDocument parent, Uri uri, bool encryptFile = false)
        {
            if (mapActiveDocuments.ContainsKey(uri.GetPathString()))
                return mapActiveDocuments[uri.GetPathString()].SaveToFile(forceEncryption: encryptFile);
            var doc = ScriptDocument.FromFile(uri.GetPathString(), parent);
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

        public void SaveAllChild(IDocument parent)
        {
            var list = (from c in mapActiveDocuments.Values// .AsParallel()
                        where c.Parent == parent && c.ActiveView != null 
                        select c).ToList();

            list.ForEach(document =>
            {
                document.SaveCurrentDocument();
            });
        }

        public bool CloseAllChild(IDocument parent, bool bParentClosing = false)
        {
            var list = (from c in mapActiveDocuments.Values// .AsParallel()
                        where c.Parent == parent && c.ActiveView != null 
                        select c).ToList();

            foreach (var document in list)
            {
                if (!CloseScript(document.ActiveView as ScriptEditorUI))
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

        public ObservableCollection<IDocumentManager> GetChildDocumentManagers()
        {
            return null;
        }

        public ObservableCollection<IDocumentManager> GetChildDocumentManagers(Uri uri, IDocument parent)
        {
            ScriptDocument doc = null;
            if (!mapActiveDocuments.TryGetValue(uri.GetPathString(), out doc))
            {
                doc = ScriptDocument.FromFile(uri.GetPathString(), parent);
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
            var rootParent = DocumentHelper.GetRootParent(parent, traverse: false);
            var dependencies = new List<String>();
            if (UFUAEditor != null)
                dependencies.Add(UFUAEditor.GetServiceName(parent));

            return new Service.ServiceControl(rootParent.Title, dependencies.ToArray(), rootParent.FilePath);
        }

        public IDictionary<string, string> GetOptionsLicenseRequired(IDocument parent)
        {
            return null;
        }

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

        public BitmapImage TypeIcon
        {
            get
            {
                return GetControlImage("SCMEditorSmall");
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
                return GetControlImage("SCMEditor");
            }
        }

        public System.Windows.Controls.Primitives.Popup TypeContextMenu
        {
            get
            {
                return null;
            }
        }

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

        public bool IsResourceExpandable(IDocument document = null)
        {
            return false;
        }
        public bool IsStartupControllerAware
        {
            get
            {
                return false;
            }
        }

        public Uri CreateNewDocument(Uri relative, IDocument parent, bool encryptFile = false)
        {
            String path, newScriptName;
            int i = 0;
            Uri url = null;
            if (parent != null && parent.fileSystemProviderBase != null)
            {
                do
                {
                    newScriptName = String.Format("{0}{1}", Properties.Settings.Default.DefaultScriptName, ++i);
                    path = String.Format("{0}{1}{2}", relative.OriginalString, newScriptName, Properties.Settings.Default.DefaultFileExt);
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
                newScriptName = String.Format("{0}{1}", Properties.Settings.Default.DefaultScriptName, ++i);
                path = String.Format("{0}{1}{2}", relative.OriginalString, newScriptName, Properties.Settings.Default.DefaultFileExt);
            } while (File.Exists(path));

            if (path.StartsWith("\\"))
                url = new Uri(path);
            else
                url = new Uri(String.Format("{0}://{1}", TypeScheme, path), UriKind.RelativeOrAbsolute);
            CreateDefaultDocument(url, parent, encryptFile);
            return url;
        }

        public Type DocumentType
        {
            get
            {
                return typeof(ScriptDocument);
            }
        }

        #endregion IDocumentManager Members

        #region IScriptManager
        public String GetScriptCode(IDocument parent, String script)
        {
            if (parent == null)
                return null;
            var path = String.Format("{0}\\{1}\\{2}{3}", parent.rootBase, Properties.Settings.Default.TypeLabel, script, Properties.Settings.Default.DefaultFileExt);
            var doc = ScriptDocument.FromFile(path, parent);
            if (doc == null)
                return null;
            var ret = doc.Code;
            doc.Dispose();
            return ret;
        }
        #endregion

        #region Properties

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

        #endregion Properties

        #region IDisposable Members

        void IDisposable.Dispose()
        {
            mapActiveDocuments.Clear();
            mapActiveDocumentUris.Clear();
            if (workspace != null)
            {
                workspace.Closed -= workspace_Closed;
                workspace.Closing -= workspace_Closing;
                workspace.CloseButtonClick -= workspace_CloseButtonClick;
                workspace.ActiveWindowChanged -= workspace_ActiveWindowChanged;
            }

            if (PropertyControl != null)
                PropertyControl.AcceptChanges -= PropertyControl_AcceptChanges;
        }

        #endregion IDisposable Members

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

        public void RenameCRObjects(UFInterfaces.Editors.CrossReferenceModel model)
        {
            if (model.ResourceList == null || model.QuitEvent.IsCancellationRequested)
                return;

            var p = DocumentHelper.GetRootParent(model.Parent, traverse: false);
            foreach (var resource in model.ResourceList)
            {
                if (model.QuitEvent.IsCancellationRequested)
                    return;

                var uri = new Uri(string.Format("{0}\\{1}", model.Parent.fileSystemProviderBase != null ? string.Empty : p.rootBase, resource), UriKind.RelativeOrAbsolute);
                using (ScriptDocument doc = CreateDocument(model.Parent, uri))
                {
                    if (doc != null)
                        doc.RenameReferences(model);
                }
            }
        }

        public IToolbar GetToolbar()
        {
            if (menuControl == null)
                menuControl = new MenuControl(this, workspace);
            return menuControl;
        }

        public IList<System.Windows.Input.ICommand> GetAlwaysAvailableCommand()
        {
            return null;
        }
    }
}