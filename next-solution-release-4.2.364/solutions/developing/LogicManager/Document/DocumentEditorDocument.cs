using System;
using System.Collections.Generic;
using System.Linq;
using DocumentManager.ComponentService;
#if !WINDOWS_UWP
#if !NET_STANDARD
using System.Windows.Controls;
using VFS;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using WPFUtilities.Extensions;
using System.Windows.Media;
using UFInterfaces.PropertyControl;
using UFUAEditor.ComponentService;
#endif
using log4net;
#else
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
#endif
using System.ComponentModel;
using ViewModelLib;
using System.IO;
using LogicManager.ComponentService;
using System.Windows;
using System.Xml;
using Utilities;
using System.Runtime.Serialization;
using UFInterfaces.Constants;
using System.Threading;
using OPCUAViewModel;
using Northwoods.GoXam.Model;
using System.Xml.Linq;
using Northwoods.GoXam;
using System.Diagnostics;
using UFInterfaces;
using Opc.Ua;
using LogicCore;
#if !NET_STANDARD
using UIMsgBoxAlertService.ComponentService;
#endif
using DocumentManager.ComponentService.Helpers;

namespace LogicManager.Document
{
    [Flags]
    public enum LogicStatus : int
    {
        None = 0,
        Error = 1,
        ReadingTags = 2,
        WritingTags = 4,
        WaitingForGoodTags = 8,
        Cycling = 16,
        Starting = 32,
        Stopping = 64,
        Running = 128
    }

    [DataContract(Name = "LogicDocument", Namespace = UFInterfaces.Constants.Namespaces.UriProgea)]
    public class DocumentEditorDocument : ViewModelBase,
#if !WINDOWS_UWP && !NET_STANDARD
        ICloneable, INotifyPropertyVisibilityChanged, 
#endif
        IDocument, IEntityReference
    {
#region Declarations

#if !WINDOWS_UWP && !NET_STANDARD
        static string TempFilenamePreface = "default";
        static int TempFilenameCount = 0;
#endif
        GraphLinksModel<GateData, String, String, WireData> model = new GraphLinksModel<GateData, String, String, WireData>();

        static readonly String Gate = "Gate";
        static readonly String Wire = "Wire";
        static readonly String LogicCircuit = "LogicCircuit";

#endregion

#region Constructors

        public DocumentEditorDocument()
        {
        }

#endregion

#region Persistance

        [DataMember]
        String sCode;
        [DataMember]
        string sessionName;
        [DataMember]
        int removeDisabledItemAfterSecs = 30;
        [DataMember]
        int maxCleanCount = 2;
        [DataMember]
        bool useAlwaysSecureConnections = false;
        [DataMember]
        int slowSamplingInterval = 5000;
        [DataMember]
        bool disableWhenNotUsed = true;
        [DataMember]
        int publishingInterval = 1000;
        [DataMember]
        int fastSamplingInterval = 500;
        [DataMember]
        int cycleClock = 250;
#if !WINDOWS_UWP
        [DataMember]
        ThreadPriority threadPriority = ThreadPriority.Normal;
#endif
        [DataMember]
        OPCUAEntityReference cycleTimeTag;
        [DataMember]
        OPCUAEntityReference currentStatusTag;
        [DataMember]
        Guid id;

#endregion

#region Methods
        bool IsBelongFromParent(IDocument parent)
        {
            return id == Guid.Empty || id == parent.Id;
        }

        internal void UpdateSessionSettings()
        {
            if (String.IsNullOrEmpty(SessionName))
                return;

            String[] serverUriArray = null;
#if !WINDOWS_UWP && !NET_STANDARD
            var ufuaEditor = GetService(typeof(IUFUAEditorManager)) as IUFUAEditorManager;
            if (ufuaEditor != null)
                serverUriArray = ufuaEditor.GetServerUriArray(this);
#endif
            RealTimeConnectionManagerViewModel.AddSessionSettings(SessionString, new SessionSettings()
            {
                ParentTitle = Parent.Title,
                RemoveDisabledItemAfterSecs = this.RemoveDisabledItemAfterSecs,
                MaxCleanCount = this.MaxCleanCount,
                UseAlwaysSecureConnections = this.UseAlwaysSecureConnections,
                SlowSamplingInterval = this.SlowSamplingInterval,
                DisableWhenNotUsed = this.DisableWhenNotUsed,
                PublishingInterval = this.PublishingInterval,
                ServerArray = serverUriArray,
                FastSamplingInterval = this.FastSamplingInterval
            });
        }

        [OnDeserializing]
        private void PreInitialize(StreamingContext context)
        {
            removeDisabledItemAfterSecs = 30;
            maxCleanCount = 2;
            useAlwaysSecureConnections = false;
            slowSamplingInterval = 5000;
            disableWhenNotUsed = true;
            publishingInterval = 250;
            fastSamplingInterval = 500;
#if !WINDOWS_UWP
            threadPriority = ThreadPriority.Normal;
#endif
        }

#if !WINDOWS_UWP && !NET_STANDARD
        public bool SaveCurrentDocument()
        {
            if (!NeedsSave)
                return true;

            return SaveToFile();
        }

        private bool WriteProjectDataStream(Stream ostrm)
        {
            var settings = new XmlWriterSettings
            {
                Encoding = System.Text.Encoding.UTF8,
                Indent = true,
                CloseOutput = true
            };

            using (var writer = XmlDictionaryWriter.Create(ostrm, settings))
            {
                bool bRet = false;
                try
                {
                    var serializer = new DataContractSerializer(typeof(DocumentEditorDocument));
                    serializer.WriteObject(writer, this);
                    NeedsSave = false;
                    bRet = true;
                }
                finally
                {
                    writer.Close();
                }

                return bRet;
            }
        }

        internal bool SaveToFile(bool forceEncryption = false)
        {
            try
            {
                XElement root = model.Save<GateData, WireData>(LogicCircuit, Gate, Wire);
                Code = root.ToString();

                if (fileSystemProviderBase != null)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        if (!WriteProjectDataStream(memoryStream))
                            return false;

                        fileSystemProviderBase.UploadFile(null, FullPath, memoryStream.ToArray());
                        return true;
                    }
                }

                Directory.CreateDirectory(Path.GetDirectoryName(FullPath));
                String settingsFileName = FullPath;
                if (forceEncryption || Protected)
                {
                    id = Id;
                    using (var memoryStream = new MemoryStream())
                    {
                        if (!WriteProjectDataStream(memoryStream))
                            return false;

                        var str = Convert.ToBase64String(memoryStream.ToArray());
                        var toWrite = WPFUtilities.CryptString.CryptString.EncryptString(str);
                        File.WriteAllText(settingsFileName, toWrite);
                    }
                }
                else
                {
                    id = Guid.Empty;
                    using (var ostrm = File.Open(settingsFileName, FileMode.Create, FileAccess.ReadWrite))
                    {
                        return WriteProjectDataStream(ostrm);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(String.Format(Properties.Resources.ErrorSavingDocument, ex.Message),
                                        Title, MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            return true;
        }

        public static void CopyFile(String fullPath, String newPath, bool bCopy,
            IDocument parent, UFInterfaces.IWorkspace work = null, IDocumentManager manager = null)
        {
            string ret = string.Empty;
            using (var sourceDoc = FromFile(fullPath, parent))
            {
                if (sourceDoc == null)
                    return;

                // TODO: code for copying all document content
            }

            if (!bCopy)
                RemoveFile(fullPath, parent);
        }
#endif
        static DocumentEditorDocument ReadFromStream(Stream stream)
        {
            try
            {
                var formatter = new DataContractSerializer(typeof(DocumentEditorDocument));
                var document = formatter.ReadObject(stream) as DocumentEditorDocument;
                return document;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

#if !WINDOWS_UWP && !NET_STANDARD
        internal static bool ExistFile(String fullPath, FileSystemProviderBase fileSystemProvider = null)
        {
            if (fileSystemProvider != null)
                return fileSystemProvider.Exists(new FileManagerFile(fileSystemProvider, fullPath));

            return File.Exists(fullPath);
        }
#endif

        public static DocumentEditorDocument FromFile(String fullPath, IDocument parent)
        {
            try
            {
                DocumentEditorDocument document = null;

#if !WINDOWS_UWP && !NET_STANDARD
                FileSystemProviderBase fileSystemProvider = parent.fileSystemProviderBase;
                if (fileSystemProvider != null)
                {
                    if (fileSystemProvider.Exists(new FileManagerFile(fileSystemProvider, fullPath)))
                    {
                        var data = fileSystemProvider.ReadFile(new FileManagerFile(fileSystemProvider, fullPath));
                        using (var memoryStream = new MemoryStream(data))
                        {
                            document = ReadFromStream(memoryStream);
                            if (document == null)
                            {
                                var uiMsgBox = parent.GetService(typeof(IUIMsgBoxAlertService)) as IUIMsgBoxAlertService;
                                if (uiMsgBox != null)
                                    uiMsgBox.ShowError(String.Format(Properties.Resources.ErrorReadingDocument, fullPath));
                            }
                            else
                                document.FullPath = fullPath;
                        }
                    }

                    if (document == null)
                    {
                        document = new DocumentEditorDocument()
                        {
                            FullPath = fullPath
                        };
                    }

                    document.ReadModel();
                    return document;
                }
#endif
                if (File.Exists(fullPath))
                {
#if !WINDOWS_UWP && !NET_STANDARD
                    if (parent.Protected || !Utilities.IO.FileSystem.IsXmlFile(fullPath))
                    {
                        var data = WPFUtilities.CryptString.CryptString.DecryptString(File.ReadAllText(fullPath));
                        using (var reader = new MemoryStream(Convert.FromBase64String(data)))
                        {
                            document = ReadFromStream(reader);
                            if (document == null)
                            {
                                var uiMsgBox = parent.GetService(typeof(IUIMsgBoxAlertService)) as IUIMsgBoxAlertService;
                                if (uiMsgBox != null)
                                    uiMsgBox.ShowError(String.Format(Properties.Resources.ErrorReadingDocument, fullPath));
                            }
                            else if (!document.IsBelongFromParent(parent))
                            {
                                document.Dispose();
                                var uiMsgBox = parent.GetService(typeof(IUIMsgBoxAlertService)) as IUIMsgBoxAlertService;
                                if (uiMsgBox != null)
                                    uiMsgBox.ShowError(String.Format(Properties.Resources.ErrorValidatingDocument, fullPath));
                                return null;
                            }
                            else
                                document.FullPath = fullPath;
                        }
                    }
                    else
#endif
                    {
                        using (var fileStream = new FileStream(fullPath, FileMode.Open, FileAccess.Read))
                        {
                            document = ReadFromStream(fileStream);
                            if (document == null)
                            {
#if !NET_STANDARD
#if !WINDOWS_UWP
                                var uiMsgBox = parent.GetService(typeof(IUIMsgBoxAlertService)) as IUIMsgBoxAlertService;
                                if (uiMsgBox != null)
                                    uiMsgBox.ShowError(String.Format(Properties.Resources.ErrorReadingDocument, fullPath));
#else
                                showMsgBox(String.Format(Properties.Resources.ErrorReadingDocument, fullPath), fullPath);
#endif
#endif
                            }
                            else
                                document.FullPath = fullPath;
                        }
                    }
                }

                if (document == null)
                {
                    document = new DocumentEditorDocument()
                    {
                        FullPath = fullPath
                    };
                }
                document.ReadModel();
                return document;
            }
            catch
            {
#if !NET_STANDARD
                var uiMsgBox = parent.GetService(typeof(IUIMsgBoxAlertService)) as IUIMsgBoxAlertService;
                if (uiMsgBox != null)
                    uiMsgBox.ShowError(String.Format(Properties.Resources.ErrorReadingDocument, fullPath));
#endif
                return null;
            }
        }

#if WINDOWS_UWP
        private static async void showMsgBox(string message, string heading)
        {
            var msgDialog = new MessageDialog(message, heading);
            await msgDialog.ShowAsync();
        }
#endif

#if !WINDOWS_UWP && !NET_STANDARD
        internal static void RemoveFile(string fullPath, IDocument parent)
        {
            FileSystemProviderBase fileSystemProvider = parent.fileSystemProviderBase;
            if (fileSystemProvider != null)
            {
                var fileManagerFile = new FileManagerFile(fileSystemProvider, fullPath);
                if (fileSystemProvider.Exists(fileManagerFile))
                {
                    fileSystemProvider.DeleteFile(fileManagerFile);
                }
            }
            else
            {
                if (File.Exists(fullPath))
                    File.Delete(fullPath);
            }
        }

        internal static String RenameFile(String fullPath, String oldName, String newName, FileSystemProviderBase fileSystemProvider = null)
        {
            var folder = Path.GetDirectoryName(fullPath);
            var newPathName = String.Format("{0}\\{1}{2}", folder, newName, Path.GetExtension(fullPath));

            if (fileSystemProvider != null)
            {
                var fileManagerFile = new FileManagerFile(fileSystemProvider, fullPath);
                if (fileSystemProvider.Exists(fileManagerFile))
                {
                    fileSystemProvider.RenameFile(fileManagerFile, String.Format("{0}{1}", newName, fileManagerFile.Extension));
                }
            }
            else
            {
                if (File.Exists(fullPath))
                    File.Move(fullPath, newPathName);
            }

            return newPathName;
        }

        internal static void CopyFile(String fullPath, String newPath, bool bCopy, IDocument parent)
        {
            FileSystemProviderBase fileSystemProvider = parent.fileSystemProviderBase;
            FileSystemProviderBase targetVFS = null;
            bool bDisposeTargetVFS = true;
            bool bTargetDataSource = XpoHelpers.XpoHelper.IsDataSource(newPath);
            if (bTargetDataSource)
            {
                targetVFS = new DataSourceFileSystemProvider("")
                {
                    ConnectionString = newPath
                };
            }
            else
            {
                if (!Path.IsPathRooted(newPath))
                {
                    targetVFS = fileSystemProvider;
                    bDisposeTargetVFS = false;
                }
            }

            try
            {
                if (fileSystemProvider != null)
                {
                    var fileManagerFile = new FileManagerFile(fileSystemProvider, fullPath);
                    if (fileSystemProvider.Exists(fileManagerFile))
                    {
                        var data = fileSystemProvider.ReadFile(fileManagerFile);
                        if (targetVFS != null)
                        {
                            if (bDisposeTargetVFS)
                                targetVFS.UploadFile(null, fullPath, data);
                            else
                                targetVFS.UploadFile(null, newPath, data);
                        }
                        else
                            File.WriteAllBytes(newPath, data);

                        if (!bCopy)
                            fileSystemProvider.DeleteFile(fileManagerFile);
                    }
                }
                else
                {
                    if (File.Exists(fullPath))
                    {
                        if (targetVFS != null)
                        {
                            targetVFS.UploadFile(null, fullPath.Replace(parent.rootBase, parent.rootBaseDB),
                                File.ReadAllBytes(fullPath));
                        }
                        else
                            File.Copy(fullPath, newPath, true);
                    }

                    if (!bCopy)
                        RemoveFile(fullPath, parent);
                }
            }
            finally
            {
                if (bDisposeTargetVFS && targetVFS != null && targetVFS is DataSourceFileSystemProvider)
                    (targetVFS as DataSourceFileSystemProvider).Dispose();
            }
        }
#endif
        void ReadModel()
        {
            model = new GraphLinksModel<GateData, String, String, WireData>();
            // create a simple model of GateData and WireData (defined below)
            // tell the model how to choose a data template for the node
#if !WINDOWS_UWP && !NET_STANDARD
            model.NodeCategoryPath = GateData.NodeCategoryPath;
#endif
            // initialize it from data in an XML file that is an embedded resource
            if (!String.IsNullOrEmpty(Code))
                model.Load<GateData, WireData>(XElement.Parse(Code), AllocateItemData, AllocatePipeData);
#if !WINDOWS_UWP && !NET_STANDARD
            model.Modifiable = true;  // let the user modify the graph
            model.HasUndoManager = true;  // support undo/redo

            // model.IsModified = false;
            model.Changed += (o, e) =>
            {
                NeedsSave = true;
            };
#endif
        }

        private GateData AllocateItemData(XElement xe)
        {
            if (xe.Name.LocalName != Gate)
                return null;

            var type = XHelper.Read("Key", xe, "");
            var types = type.Split(' ');
            var ret = GateData.CreateFrom(types[0]);
#if !WINDOWS_UWP
            if (ret == null)
            {
#if !NET_STANDARD
                var syslog = LogManager.GetLogger(Title);
#else
                var syslog = LogManager.GetLogger(System.Reflection.Assembly.GetEntryAssembly(), Title);
#endif
                syslog.Error(String.Format(Properties.Resources.CannotLoadElement, type));
            }
#endif
                return ret;
        }
        private WireData AllocatePipeData(XElement xe)
        {
            if (xe.Name.LocalName != Wire)
                return null;
            return new WireData();
        }
#if !WINDOWS_UWP && !NET_STANDARD
        internal List<OPCUAViewModel.OPCUAEntityReference> GetReferenceList(bool bAll = true)
        {
            var list = new List<OPCUAViewModel.OPCUAEntityReference>();

            if (CycleTimeTag != null && CycleTimeTag.IsValid)
                list.Add(CycleTimeTag);
            if (CurrentStatusTag != null && CurrentStatusTag.IsValid)
                list.Add(CurrentStatusTag);
            if (model != null && bAll)
                foreach (GateData data in model.NodesSource)
                {
                    GateData _data = data;
                    data.GetTagList().ForEach(tag =>
                    {
                        list.Add(tag);
                    });
                }
            return list;
        }
        internal List<GateData> GetModelReferenceList()
        {
            var list = new List<GateData>();

            if (Model != null)
                list.AddRange((from GateData data in Model.NodesSource
                 where (data.GetTagList().Count > 0)
                 select data).ToList());

            return list;
        }
        internal List<UFInterfaces.Editors.CrossReferenceResultModel> GetCReferenceList(UFInterfaces.Editors.CrossReferenceModel crmodel)
        {
            var list = new List<UFInterfaces.Editors.CrossReferenceResultModel>();
            bool getTags = crmodel.CRManagement.CrossReferenceTypeList.Contains(CrossReferenceType.Tags);
            if (!getTags)
                return list;
            string docType = DocManagerType.LogicManager.ToString();
            if (CycleTimeTag != null && CycleTimeTag.IsValid)
                list.Add(new UFInterfaces.Editors.CrossReferenceResultModel()
                {
                    RelativePath = CycleTimeTag.RelativePath,
                    Name = CycleTimeTag.Name,
                    AppName = CycleTimeTag.AppName,
                    ReferencedNodeId = CycleTimeTag.ResolvedNodeId?.Identifier.ToString(),
                    EndpointUrl = CycleTimeTag.EndpointUrl,
                    CReferenceType = CrossReferenceType.Tags,
                    Description = string.Format("{0} ({1})", Title, Properties.Resources.CycleTimeTag),
                    Settings = string.Format("{0}|{1}", docType, FullPath),
                    ContainerDoc = DocumentEditorManagerComponent.documentEditorManagerComponent.TypeScheme,
                    IconType = docType
                });

            if (CurrentStatusTag != null && CurrentStatusTag.IsValid)
                    list.Add(new UFInterfaces.Editors.CrossReferenceResultModel()
                    {
                        RelativePath = CurrentStatusTag.RelativePath,
                        Name = CurrentStatusTag.Name,
                        AppName = CurrentStatusTag.AppName,
                        ReferencedNodeId = CurrentStatusTag.ResolvedNodeId?.Identifier.ToString(),
                        EndpointUrl = CurrentStatusTag.EndpointUrl,
                        CReferenceType = CrossReferenceType.Tags,
                        Description = string.Format("{0} ({1})", Title, Properties.Resources.CurrentStatusTag),
                        Settings = string.Format("{0}|{1}", docType, FullPath),
                        ContainerDoc = DocumentEditorManagerComponent.documentEditorManagerComponent.TypeScheme,
                        IconType = docType
                    });

            if (model != null)
            {
                foreach (GateData data in model.NodesSource)
                {
                    if (crmodel.QuitEvent.IsCancellationRequested)
                        return list;

                    GateData _data = data;
                    data.GetTagList().ForEach(tag =>
                    {
                        if (crmodel.QuitEvent.IsCancellationRequested)
                            return;

                        list.Add(new UFInterfaces.Editors.CrossReferenceResultModel()
                        {
                            RelativePath = tag.RelativePath,
                            Name = tag.Name,
                            AppName = tag.AppName,
                            ReferencedNodeId = tag.ResolvedNodeId?.Identifier.ToString(),
                            EndpointUrl = tag.EndpointUrl,
                            CReferenceType = CrossReferenceType.Tags,
                            Description = string.Format("{0}\\{1} - ({2} {3})", Title, data.Text, data.Name, Properties.Resources.LogicItem),
                            Settings = string.Format("{0}#{1}|{2}", _data.Key, _data.Text, FullPath),
                            ContainerDoc = DocumentEditorManagerComponent.documentEditorManagerComponent.TypeScheme,
                            IconType = docType
                        });
                    });
                }
            }

            return list;
        }
        internal void RenameReferences(UFInterfaces.Editors.CrossReferenceModel model)
        {
            IUFUAEditorManager UfuaEditorService = GetService(typeof(IUFUAEditorManager)) as IUFUAEditorManager;
            if (UfuaEditorService == null)
                return;
            string defaultlocalendpoint = UfuaEditorService.GetDefaultLocalEndpoint(this, true);
            var endpointslist = UfuaEditorService.GetEndpoints(this);
            string aplicationName = UfuaEditorService.GetAplicationName(this, true);
            using (var cursor = new WaitCursor())
            {
                var list = GetReferenceList();
                var taglist = GetReferenceList(false);
                var modellist = GetModelReferenceList();

                if (DocumentEditorManagerComponent.documentEditorManagerComponent.UFUAEditor != null)
                {
                    var nodelist = (from c in list
                                    where c.ResolvedNodeId != null
                                    select c.ResolvedNodeId.ToString()).Distinct().ToList();
                    var mapNodes = DocumentEditorManagerComponent.documentEditorManagerComponent.UFUAEditor.GetListNodeNames(this, nodelist);
                    if (mapNodes == null)
                        return;
                    bool bDirty = false;
                    (from c in taglist
                     where c.ResolvedNodeId != null
                     select c).ToList().ForEach(item =>
                     {
                         if (model.QuitEvent.IsCancellationRequested)
                             return;
                         string node = item.ResolvedNodeId.ToString();
                         if (mapNodes.ContainsKey(node))
                         {
                             bool changeEndpoint = !endpointslist.Contains(item.EndpointUrl);
                             if (changeEndpoint)
                             {
                                 item.EndpointUrl = item.EndpointUrl.Replace(item.AppName, aplicationName);
                                 if (!endpointslist.Contains(item.EndpointUrl))
                                     item.EndpointUrl = defaultlocalendpoint;
                             }
                             item.AppName = aplicationName;
                             var shortname = mapNodes[node];
                             var newName = String.Format("{0} ({1})", shortname, item.AppName);
                             if (item.HumanReadable != newName || changeEndpoint)
                             {
                                 item.HumanReadable = newName;
                                 item.ReadablePath = CrossReferenceHelper.Helper.GetNewPath(shortname, item.ReadablePath);
                                 item.RelativePath = CrossReferenceHelper.Helper.GetNewPath(shortname, item.RelativePath);
                                 bDirty = true;
                             }
                         }
                     });

                    (from c in modellist
                     select c).ToList().ForEach(data =>
                     {
                         if (model.QuitEvent.IsCancellationRequested)
                             return;

                         data.Terminate();
                         var _list = data.GetTagList();
                         _list.ForEach(item =>
                         {
                             if (model.QuitEvent.IsCancellationRequested)
                                 return;
                             if (item != null && item.ResolvedNodeId != null)
                             {
                                 string node = item.ResolvedNodeId.ToString();
                                 if (mapNodes.ContainsKey(node))
                                 {
                                     bool changeEndpoint = !endpointslist.Contains(item.EndpointUrl);
                                     if (changeEndpoint)
                                     {
                                         item.EndpointUrl = item.EndpointUrl.Replace(item.AppName, aplicationName);
                                         if (!endpointslist.Contains(item.EndpointUrl))
                                             item.EndpointUrl = defaultlocalendpoint;
                                         item.AppName = aplicationName;
                                     }
                                     var shortname = mapNodes[node];
                                     var newName = String.Format("{0} ({1})", shortname, item.AppName);
                                     if (item.HumanReadable != newName || changeEndpoint)
                                     {
                                         if (data.Text == item.HumanReadable)
                                             data.Text = newName;
                                         item.HumanReadable = newName;
                                         item.ReadablePath = CrossReferenceHelper.Helper.GetNewPath(shortname, item.ReadablePath);
                                         item.RelativePath = CrossReferenceHelper.Helper.GetNewPath(shortname, item.RelativePath);

                                         bDirty = true;
                                     }
                                 }
                             }
                         });
                         data.UpdateTagList(_list);
                     });

                    if (bDirty)
                        try
                        {
                            SaveToFile();
                        }
                        catch (Exception ex)
                        {
                        }
                }
            }
        }
#endif
        void SubscribeAllVariables()
        {
            if (model == null || bIsInDesignMode)
                return;

            if (CycleTimeTag != null
#if !WINDOWS_UWP && !NET_STANDARD
                && CycleTimeTag.IsValid
#endif
                )
            {
                var tagxml = CycleTimeTag.ToXml();
                if (!String.IsNullOrEmpty(tagxml))
                    _runtimeCycleTimeTag = tagxml.FromXml<OPCUAEntityReference>();

                if (_runtimeCycleTimeTag != null)
                {
                    _runtimeCycleTimeTag.Resolve(SessionString, parent);
                    _runtimeCycleTimeTag.SetInUse(this, true);
                }
            }
            if (CurrentStatusTag != null
#if !WINDOWS_UWP && !NET_STANDARD
                && CurrentStatusTag.IsValid
#endif
                )
            {
                var tagxml = CurrentStatusTag.ToXml();
                if (!String.IsNullOrEmpty(tagxml))
                    _runtimeCurrentStatusTag = tagxml.FromXml<OPCUAEntityReference>();

                if (_runtimeCurrentStatusTag != null)
                {
                    _runtimeCurrentStatusTag.Resolve(SessionString, parent);
                    _runtimeCurrentStatusTag.SetInUse(this, true);
                }
            }

            foreach(GateData data in model.NodesSource)
            {
                data.GetTagList().ForEach(tag =>
                {
                    tag.Resolve(SessionString, parent);
                    tag.SetInUse(this, true);
                });
            }
        }

        void UnsubscribeAllVariables()
        {
            if (bIsInDesignMode)
                return;

            foreach (GateData data in model.NodesSource)
            {
                data.GetTagList().ForEach(tag =>
                {
                    tag.SetInUse(this, false);
                });
            }

            lock (lockObject)
            {
                if (_runtimeCycleTimeTag != null
#if !WINDOWS_UWP && !NET_STANDARD
                && _runtimeCycleTimeTag.IsValid
#endif
                )
                {
                    _runtimeCycleTimeTag.SetInUse(this, false);
                    _runtimeCycleTimeTag = null;
                }
                if (cycleTimeObserver != null)
                {
                    cycleTimeObserver.Dispose();
                    cycleTimeObserver = null;
                }

                if (_runtimeCurrentStatusTag != null
#if !WINDOWS_UWP && !NET_STANDARD
                && _runtimeCurrentStatusTag.IsValid
#endif
                )
                {
                    _runtimeCurrentStatusTag.SetInUse(this, false);
                    _runtimeCurrentStatusTag = null;
                }
                if (currentStatusObserver != null)
                {
                    currentStatusObserver.Dispose();
                    currentStatusObserver = null;
                }
            }
        }

#if !NET_STANDARD
#if !WINDOWS_UWP
        System.Windows.Threading.
#endif
        DispatcherTimer uiTimer;
#endif
        Timer timer;
        bool bInsideTimer;
        bool bIsInDesignMode;
        bool bStarted;

        public event EventHandler CycleExecuting;
        virtual public void OnCycleExecuting()
        {
            EventHandler temp = CycleExecuting;
            if (temp != null)
                temp(this, EventArgs.Empty);
        }

        public event EventHandler CycleExecuted;
        virtual public void OnCycleExecuted()
        {
            EventHandler temp = CycleExecuted;
            if (temp != null)
                temp(this, EventArgs.Empty);
        }

        internal void Start(bool bUsingUITimer = false, bool bDesign = false)
        {
            lock (lockObject)
            {
                if (bStarted)
                    return;
                bStarted = true;
                UpdateSessionSettings();

                ChangeStatus(LogicStatus.Starting, LogicStatus.None);

                PrepareLogicCycle();
                if (Model == null || listStartingNodes == null || listStartingNodes.Count == 0 ||
                    listEndingNodes == null || listEndingNodes.Count == 0)
                {
                    ChangeStatus(LogicStatus.Error, LogicStatus.Starting, Properties.Resources.NoInputOrOutputNodes);
                    return;
                }

                InExecution = true;
                ChangeStatus(LogicStatus.Running, LogicStatus.None);
                bIsInDesignMode = bDesign;
                SubscribeAllVariables();

#if !NET_STANDARD
                if (bUsingUITimer)
                {
                    uiTimer = new
#if !WINDOWS_UWP
                        System.Windows.Threading.
#endif
                        DispatcherTimer();
                    uiTimer.Tick += (o, e) =>
                        {
                            lock (lockObject)
                            {
                                if (bInsideTimer || _IsInStoppingMode || !InExecution)
                                    return;
                                bInsideTimer = true;
                            }
                            bool bCycleExecuted = false;
                            try
                            {
                                OnCycleExecuting();

                                bCycleExecuted = LogicCycle();
                            }
                            finally
                            {
                                bInsideTimer = false;
                            }

                            if (bCycleExecuted)
                                OnCycleExecuted();
                        };
                    uiTimer.Interval = TimeSpan.FromMilliseconds(CycleClock);
                    uiTimer.Start();
                }
                else
#endif
                {
                    timer = new Timer(timerCallback, null, CycleClock, CycleClock);
                }

                ChangeStatus(LogicStatus.None, LogicStatus.Starting);
            }
        }

        void timerCallback(Object state)
        {
            if (bObjectDisposed)
                return;
            lock (lockObject)
            {
                if (bInsideTimer || _IsInStoppingMode || !InExecution)
                    return;
                bInsideTimer = true;
                if (timer != null)
                {
                    timer.Dispose();
                    timer = null;
                }
            }

            bool bCycleExecuted = false;
            try
            {
                OnCycleExecuting();

                bCycleExecuted = LogicCycle();
            }
            finally
            {
                bInsideTimer = false;
            }

            if (bCycleExecuted)
                OnCycleExecuted();

            lock (lockObject)
            {
                if (timer == null)
                    timer = new Timer(timerCallback, null, CycleClock, CycleClock);
            }
        }

        internal void Stop()
        {
            lock (lockObject)
            {
                if (!bStarted)
                    return;

                ChangeStatus(LogicStatus.Stopping, LogicStatus.None);
                _IsInStoppingMode = true;
#if !NET_STANDARD
                if (uiTimer != null)
                {
                    uiTimer.Stop();
                    uiTimer = null;
                }
#endif
                if (timer != null)
                {
                    timer.Dispose();
                    timer = null;
                }
            }

            while (bInsideTimer)
#if !WINDOWS_UWP
                Thread.Sleep(100);
#else
                System.Threading.Tasks.Task.Delay(100).Wait();
#endif
            lock(lockObject)
            { 
                ChangeStatus(LogicStatus.None, LogicStatus.Stopping | LogicStatus.Running);
                UnsubscribeAllVariables();
                InExecution = false;
                _IsInStoppingMode = false;
                bStarted = false;

                foreach (GateData d in model.NodesSource)
                    d.Terminate();
            }
        }

        List<GateData> listStartingNodes;
        List<GateData> listEndingNodes;
        void PrepareLogicCycle()
        {
            if (listStartingNodes == null)
                listStartingNodes = new List<GateData>();
            else
                listStartingNodes.Clear();

            if (listEndingNodes == null)
                listEndingNodes = new List<GateData>();
            else
                listEndingNodes.Clear();

            foreach (GateData d in model.NodesSource)
            {
                if (d.GateType == GateTypes.Input)
                    listStartingNodes.Add(d);
                else if (d.GateType == GateTypes.Output)
                    listEndingNodes.Add(d);

                d.Init(this);
            }
        }

        bool ReadAllInputImage()
        {
            if (bIsInDesignMode)
                return true;

            bool bRet = true;
            listStartingNodes.ForEach(gateData =>
                {
                    bRet &= gateData.ReadData();
                });

            listEndingNodes.ForEach(gateData =>
            {
                bRet &= gateData.ReadData();
            });

            return bRet;
        }

        bool WriteAllOutputImage()
        {
            if (bIsInDesignMode)
                return true;

            bool bRet = true;
            listEndingNodes.ForEach(gateData =>
            {
                try
                {
                    gateData.WriteData();
                }
                catch (Exception ex)
                {
#if !WINDOWS_UWP
#if !NET_STANDARD
                    var syslog = LogManager.GetLogger(Title);
#else
                    var syslog = LogManager.GetLogger(System.Reflection.Assembly.GetEntryAssembly(), Title);
#endif
                    syslog.Error(ex.Message, ex);
#endif
                    bRet = false;
                }
            });
            return bRet;
        }

        PropertyObserver<OPCUAEntityReference> cycleTimeObserver;
        void ChangeCycleTime(TimeSpan time)
        {
            lock (lockObject)
            {
                CycleTime = time;

                if (!bIsInDesignMode && _runtimeCycleTimeTag != null)
                {
                    if (_runtimeCycleTimeTag.MonitoredItemViewModel == null)
                    {
                        if (cycleTimeObserver == null)
                        {
                            cycleTimeObserver = new PropertyObserver<OPCUAEntityReference>(_runtimeCycleTimeTag);
                            cycleTimeObserver.RegisterHandler(n => n.MonitoredItemViewModel, n =>
                            {
                                if (n.MonitoredItemViewModel != null)
                                {
                                    lock (lockObject)
                                    {
                                        if (cycleTimeObserver != null)
                                        {
                                            cycleTimeObserver.Dispose();
                                            cycleTimeObserver = null;
                                        }

                                        ChangeCycleTime(CycleTime);
                                    }
                                }
                            });
                        }
                    }
                    else
                    {
                        try
                        {
                            _runtimeCycleTimeTag.MonitoredItemViewModel.WriteValue(CycleTime.TotalMilliseconds);
                        }
                        catch (Exception ex)
                        {
#if !WINDOWS_UWP
#if !NET_STANDARD
                            var syslog = LogManager.GetLogger(Title);
#else
                            var syslog = LogManager.GetLogger(System.Reflection.Assembly.GetEntryAssembly(), Title);
#endif
                            syslog.Error(String.Format(Properties.Resources.WriteVariableException, _runtimeCycleTimeTag.HumanReadable), ex);
#endif
                        }
                    }
                }
            }
        }

        PropertyObserver<OPCUAEntityReference> currentStatusObserver;
        void ChangeStatus(LogicStatus add, LogicStatus remove, String error = null)
        {
            lock (lockObject)
            {
                CurrentStatus |= add;
                CurrentStatus &= ~(remove);
                if (String.IsNullOrEmpty(error) ||
                    (CurrentStatus & LogicStatus.Error) != 0)
                    currentError = error;

                if (!bIsInDesignMode && _runtimeCurrentStatusTag != null)
                {
                    if (_runtimeCurrentStatusTag.MonitoredItemViewModel == null)
                    {
                        if (currentStatusObserver == null)
                        {
                            currentStatusObserver = new PropertyObserver<OPCUAEntityReference>(_runtimeCurrentStatusTag);
                            currentStatusObserver.RegisterHandler(n => n.MonitoredItemViewModel, n =>
                            {
                                if (n.MonitoredItemViewModel != null)
                                {
                                    lock (lockObject)
                                    {
                                        if (currentStatusObserver != null)
                                        {
                                            currentStatusObserver.Dispose();
                                            currentStatusObserver = null;
                                        }

                                        ChangeStatus(CurrentStatus, LogicStatus.None, CurrentError);
                                    }
                                }
                            });
                        }
                    }
                    else
                    {
                        try
                        {
                            _runtimeCurrentStatusTag.MonitoredItemViewModel.WriteValue((int)CurrentStatus);
                        }
                        catch (Exception ex)
                        {
#if !WINDOWS_UWP
#if !NET_STANDARD
                            var syslog = LogManager.GetLogger(Title);
#else
                            var syslog = LogManager.GetLogger(System.Reflection.Assembly.GetEntryAssembly(), Title);
#endif
                            syslog.Error(String.Format(Properties.Resources.WriteVariableException, _runtimeCurrentStatusTag.HumanReadable), ex);
#endif
                        }
                    }
                }
            }
        }

        bool LogicCycle()
        {
            // Finds all starter nodes; one must start at these nodes and navigate through the graph
            var list = new List<GateData>();
            list.AddRange(listStartingNodes);

            var watcher = Stopwatch.StartNew();

            ChangeStatus(LogicStatus.ReadingTags, LogicStatus.None);
            if (!ReadAllInputImage())
            {
                ChangeStatus(LogicStatus.WaitingForGoodTags, LogicStatus.ReadingTags);
                return false;
            }
            ChangeStatus(LogicStatus.Cycling, LogicStatus.ReadingTags | LogicStatus.WaitingForGoodTags);
            // Updates the current collection of nodes until that collection is empty
            // (this occurs when all Nodes have been updated and we are at the end of the Diagram);
            // stops updating if InvalidOperationException is thrown
#if !WINDOWS_UWP
            var oldPriority = Thread.CurrentThread.Priority;
            if (!bIsInDesignMode)
                Thread.CurrentThread.Priority = ThreadPriority;               
#endif
            try
            {
                // UpdateCollection will modify the collection to hold the nodes to execute next
                while (list.Count > 0) { UpdateCollection(list); }
            }
            catch (InvalidOperationException)
            {
            }
#if !WINDOWS_UWP
            Thread.CurrentThread.Priority = oldPriority;     
#endif
            // Once all Parts have been updated,
            // set their Tag properties to null so they can be updated again
            foreach (GateData d in model.NodesSource)
            {
                d.Visited = false;
            }

            bool bRet = true;
            ChangeStatus(LogicStatus.WritingTags, LogicStatus.Cycling);
            if (!WriteAllOutputImage())
            {
                ChangeStatus(LogicStatus.Error, LogicStatus.WritingTags, Properties.Resources.ErrorWritingOutputTag);
                bRet = false;
            }

            ChangeStatus(LogicStatus.None, LogicStatus.WritingTags | LogicStatus.Error);
            watcher.Stop();
            ChangeCycleTime(watcher.Elapsed);
            return bRet;
        }

        // When updating, we will need to update the links leading out of every given node
        // and update the collection so that it only contains those nodes
        // that must likewise be updated next time this method is called.
        private void UpdateCollection(List<GateData> nodes)
        {
            List<GateData> newnodes = new List<GateData>();
            foreach (GateData data in nodes)
            {
                // One of the starter nodes might have had its value changed by the user.
                // So, update the values of the links leading out of it to reflect that change.
                if (data != null && data.GateType == GateTypes.Input)
                {
                    foreach (WireData d in model.GetToLinksForNode(data))
                    {
                        if (d != null) d.Value = data.Value;
                    }
                }

                foreach (GateData outdata in model.GetToNodesForNode(data))
                {
                    // ignore nodes already "visited"
                    // if (!outdata.Visited)
                    {
                        outdata.Visited = true;  // declare "visited"
                        newnodes.Add(outdata);
                        // Checks that the node has the correct number of inputs
                        var linksInto = model.GetFromLinksForNode(outdata).ToArray();
                        if (outdata.GateType == GateTypes.Generic || outdata.GateType == GateTypes.Output)
                        {
                            try
                            {
                                outdata.Value = outdata.Execute(linksInto);
                            }
                            catch(Exception ex)
                            {
                                ChangeStatus(LogicStatus.Error, LogicStatus.None, ex.Message);
#if !NET_STANDARD
                                var syslog = LogManager.GetLogger(Title);
#else
                                var syslog = LogManager.GetLogger(System.Reflection.Assembly.GetEntryAssembly(), Title);
#endif
                                syslog.Error(String.Format(Properties.Resources.ExecutionError, outdata.Name), ex);
                            }

                            // Once the value of a Node has been updated,
                            // set the values of the links leading out of it
                            foreach (WireData d in model.GetToLinksForNode(outdata))
                            {
                                if (d != null) d.Value = outdata.Value;
                            }
                        }
                        else
                        {
                            // If the Node has the incorrect number of inputs, stop updating
                            throw new InvalidOperationException();
                        }
                    }
                }
            }
            // modify the collection to reflect new nodes that need to be executed
            nodes.Clear();
            nodes.AddRange(newnodes);
        }

#endregion

#region Properties

#if !WINDOWS_UWP
        [Browsable(false)]
#endif
        internal GraphLinksModel<GateData, String, String, WireData> Model
        {
            get
            {
                return model;
            }
        }

#if !WINDOWS_UWP
        [Browsable(false)]
#endif
        public string SessionString
        {
            get
            {
                if (!String.IsNullOrEmpty(sessionName))
                {
                    return sessionName;
                }

                var docParent = Parent;
                if (Parent != null)
                    docParent = DocumentHelper.GetRootParent(Parent, traverse: false);
                return docParent != null ? docParent.Title : Title;
            }
        }

        internal bool _IsInStoppingMode;
#if !WINDOWS_UWP
        [Browsable(false)]
#endif
        public bool IsInStoppingMode
        {
            get { return _IsInStoppingMode; }
        }

        public int CycleClock
        {
            get { return cycleClock; }
#if !WINDOWS_UWP && !NET_STANDARD
            set
            {
                if (value == cycleClock)
                    return;
                cycleClock = value;
                OnPropertyChanged("CycleClock");
                NeedsSave = true;
            }
#endif
        }

        OPCUAEntityReference _runtimeCycleTimeTag;
        public OPCUAEntityReference CycleTimeTag
        {
            get
            {
                return cycleTimeTag;
            }
#if !WINDOWS_UWP && !NET_STANDARD
            set
            {
                if (cycleTimeTag == value)
                    return;
                cycleTimeTag = value;
                OnPropertyChanged("CycleTimeTag");
                NeedsSave = true;
            }
#endif
        }

        OPCUAEntityReference _runtimeCurrentStatusTag;
        public OPCUAEntityReference CurrentStatusTag
        {
            get
            {
                return currentStatusTag;
            }
#if !WINDOWS_UWP && !NET_STANDARD
            set
            {
                if (currentStatusTag == value)
                    return;
                currentStatusTag = value;
                OnPropertyChanged("CurrentStatusTag");
                NeedsSave = true;
            }
#endif
        }

#if !WINDOWS_UWP
        public ThreadPriority ThreadPriority
        {
            get
            {
                return threadPriority;
            }
#if !NET_STANDARD
            set
            {
                if (threadPriority == value)
                    return;
                threadPriority = value;
                OnPropertyChanged("ThreadPriority");
                NeedsSave = true;
            }
#endif
        }
#endif
        public string SessionName
        {
            get { return sessionName; }
#if !WINDOWS_UWP && !NET_STANDARD
            set
            {
                if (value == sessionName)
                    return;
                sessionName = value;
                NeedsSave = true;
                UpdateSessionSettings();
                OnPropertyChanged("SessionName");
                OnPropertyVisiblityChanged("SessionName");
            }
#endif
        }

        public int RemoveDisabledItemAfterSecs
        {
            get { return removeDisabledItemAfterSecs; }
#if !WINDOWS_UWP && !NET_STANDARD
            set
            {
                if (removeDisabledItemAfterSecs == value)
                    return;
                removeDisabledItemAfterSecs = value;
                OnPropertyChanged("RemoveDisabledItemAfterSecs");
                NeedsSave = true;

                UpdateSessionSettings();
            }
#endif
        }

        public int MaxCleanCount
        {
            get { return maxCleanCount; }
#if !WINDOWS_UWP && !NET_STANDARD
            set
            {
                if (maxCleanCount == value)
                    return;
                maxCleanCount = value;
                OnPropertyChanged("MaxCleanCount");
                NeedsSave = true;

                UpdateSessionSettings();
            }
#endif
        }

        public bool UseAlwaysSecureConnections
        {
            get { return useAlwaysSecureConnections; }
#if !WINDOWS_UWP && !NET_STANDARD
            set
            {
                if (useAlwaysSecureConnections == value)
                    return;
                useAlwaysSecureConnections = value;
                OnPropertyChanged("UseAlwaysSecureConnections");
                NeedsSave = true;

                UpdateSessionSettings();
            }
#endif
        }

        public int FastSamplingInterval
        {
            get
            {
                return fastSamplingInterval;
            }
#if !WINDOWS_UWP && !NET_STANDARD
            set
            {
                if (fastSamplingInterval == value)
                    return;
                fastSamplingInterval = value;
                OnPropertyChanged("FastSamplingInterval");
                NeedsSave = true;

                UpdateSessionSettings();
            }
#endif
        }

        public int SlowSamplingInterval
        {
            get
            {
                return slowSamplingInterval;
            }
#if !WINDOWS_UWP && !NET_STANDARD
            set
            {
                if (slowSamplingInterval == value)
                    return;
                slowSamplingInterval = value;
                OnPropertyChanged("SlowSamplingInterval");
                NeedsSave = true;

                UpdateSessionSettings();
            }
#endif
        }

        public bool DisableWhenNotUsed
        {
            get { return disableWhenNotUsed; }
#if !WINDOWS_UWP && !NET_STANDARD
            set
            {
                if (disableWhenNotUsed == value)
                    return;
                disableWhenNotUsed = value;
                OnPropertyChanged("DisableWhenNotUsed");
                NeedsSave = true;

                UpdateSessionSettings();
            }
#endif
        }

        public int PublishingInterval
        {
            get
            {
                return publishingInterval;
            }
#if !WINDOWS_UWP && !NET_STANDARD
            set
            {
                if (publishingInterval == value)
                    return;
                publishingInterval = value;
                OnPropertyChanged("PublishingInterval");
                NeedsSave = true;

                UpdateSessionSettings();
            }
#endif
        }

#if !WINDOWS_UWP
        [Browsable(false)]
#endif
        public String Code
        {
            get
            {
                return sCode;
            }
#if !WINDOWS_UWP && !NET_STANDARD
            internal set
            {
                if (Code == value)
                    return;
                sCode = value;
                OnPropertyChanged("Code");
                NeedsSave = true;
            }
#endif
        }

#if !WINDOWS_UWP && !NET_STANDARD
        private bool _DisableNeedsSave = false;
        [Browsable(false)]
        public bool DisableNeedsSave
        {
            get { return _DisableNeedsSave; }
            set
            {
                if (_DisableNeedsSave == value)
                    return;

                _DisableNeedsSave = value;
            }
        }

        private bool _NeedsSave = false;
        [Browsable(false)]
        public bool NeedsSave
        {
            get { return _NeedsSave; }
            set
            {
                if (DisableNeedsSave || _NeedsSave == value)
                    return;

                _NeedsSave = value;
                OnPropertyChanged("NeedsSave");
            }
        }
#endif
#if !WINDOWS_UWP
        [ReadOnly(true)]
#endif
        public string FullPath
        {
            get
            {
#if !WINDOWS_UWP && !NET_STANDARD
                if (String.IsNullOrEmpty(Filename))
                {
                    return Path.Combine(Folder, TemporaryFilename);
                }
                else
#endif
                {
                    return Path.Combine(Folder, Filename);
                }
            }
            internal set
            {
                Folder = Path.GetDirectoryName(value);
                Filename = Path.GetFileName(value);
            }
        }

        private string _Folder = "";
#if !WINDOWS_UWP
        [Browsable(false)]
#endif
        public string Folder
        {
            get
            {
                return _Folder;
            }
            internal set
            {
                if (_Folder != value)
                {
                    _Folder = value;
                    OnPropertyChanged("Folder");
                    OnPropertyChanged("FullPath");
                }
            }
        }

        private string _Filename;
#if !WINDOWS_UWP
        [Browsable(false)]
#endif
        public string Filename
        {
            get
            {
#if !WINDOWS_UWP && !NET_STANDARD
                if (String.IsNullOrEmpty(_Filename))
                {
                    return TemporaryFilename;
                }
                else
#endif
                {
                    return _Filename;
                }
            }
            internal set
            {
                if (_Filename != value)
                {
                    _Filename = value;
                    OnPropertyChanged("Filename");
                    OnPropertyChanged("FullPath");
                }
            }
        }

#if !WINDOWS_UWP && !NET_STANDARD
        string _TemporaryFilename = "";
        [Browsable(false)]
        public string TemporaryFilename
        {
            get
            {
                if (string.IsNullOrEmpty(_TemporaryFilename))
                {
                    string temp = "";
                    /*
                    if (TempFilenameCount == 0)
                    {
                        temp = TempFilenamePreface + ".xaml";
                    }
                    else
                    {
                        temp = TempFilenamePreface + TempFilenameCount + ".xaml";
                    }
                    */
                    _TemporaryFilename = temp;
                    TempFilenameCount++;
                }
                return _TemporaryFilename;
            }
        }
#endif
#if !WINDOWS_UWP
        [Browsable(false)]
#endif
        public bool InExecution { get; private set; }

        TimeSpan cycleTime;
#if !WINDOWS_UWP
        [Browsable(false)]
#endif
        public TimeSpan CycleTime
        {
            get
            {
                return cycleTime;
            }

            internal set
            {
                if (cycleTime == value)
                    return;
                cycleTime = value;
                OnPropertyChanged("CycleTime");
            }
        }

        LogicStatus currentStatus;
#if !WINDOWS_UWP
        [Browsable(false)]
#endif
        public LogicStatus CurrentStatus
        {
            get
            {
                return currentStatus;
            }

            internal set
            {
                if (currentStatus == value)
                    return;
                currentStatus = value;
                OnPropertyChanged("CurrentStatus");
            }
        }

        String currentError;
#if !WINDOWS_UWP
        [Browsable(false)]
#endif
        public String CurrentError
        {
            get
            {
                return currentError;
            }

            internal set
            {
                if (currentError == value)
                    return;
                currentError = value;
                OnPropertyChanged("CurrentError");
            }
        }

#if !WINDOWS_UWP && !NET_STANDARD
        internal GeneralDialogContent CurrentRuntimeView { get; set; }
#endif
#endregion

#region ICloneable Members

#if !WINDOWS_UWP && !NET_STANDARD
        DocumentEditorDocument(DocumentEditorDocument template)
        {
            if (template == null)
                return;

            throw new NotImplementedException();
        }

        public object Clone()
        {
            return new DocumentEditorDocument(this);
        }
#endif
#endregion

#region IDocument

        public event EventHandler Disposing;
        virtual public void OnDisposing(Object sender)
        {
            EventHandler temp = Disposing;
            if (temp != null)
                temp(sender, EventArgs.Empty);
        }

#if !NET_STANDARD
        UserControl activeView;
#if !WINDOWS_UWP
        [Browsable(false)]
#endif
        public UserControl ActiveView
        {
            get
            {
                return activeView;
            }
            set
            {
                activeView = value;
            }
        }

#if !WINDOWS_UWP
        [Browsable(false)]
#endif
        public UserControl View
        {
            get
            {
                return activeView;
            }
        }
#endif

        IDocument parent;
#if !WINDOWS_UWP
        [Browsable(false)]
#endif
        public IDocument Parent
        {
            get
            {
                return parent;
            }
            set
            {
                parent = value;
            }
        }

#if !WINDOWS_UWP
        [Browsable(false)]
#endif
        public IList<IDocument> Childs
        {
            get
            {
                return null;
            }
        }

#if !WINDOWS_UWP
        [Browsable(false)]
#endif
        public String ProjectType
        {
            get
            {
                if (Parent != null)
                    return Parent.ProjectType;
                return String.Empty;
            }
        }

#if !WINDOWS_UWP
        [Browsable(false)]
#endif
        public String Theme
        {
            get
            {
                if (Parent != null)
                    return Parent.Theme;
                return String.Empty;
            }
        }

        public Uri GetSpecialFolder(SpecialFolders specialFolder)
        {
            if (Parent != null)
                return Parent.GetSpecialFolder(specialFolder);
            return null;
        }

        public Uri MakeAbosoluteUri(Uri relative)
        {
            if (relative == null)
                return null;

            if (Parent != null)
                return Parent.MakeAbosoluteUri(relative);
            if (relative.IsAbsoluteUri)
                return relative;

            // return new Uri(new Uri(System.IO.Path.GetDirectoryName(FullPath) + "\\"), relative);
            return null;
        }

        public IDocument UpdateParentFromUri(Uri relative)
        {
            if (Parent != null)
                return Parent.UpdateParentFromUri(relative);
            return this;
        }

        public Uri MakeRelativeUri(Uri absolute)
        {
            if (absolute == null)
                return null;

            if (Parent != null)
                return Parent.MakeRelativeUri(absolute);
            if (!absolute.IsAbsoluteUri)
                return absolute;

            // return new Uri(Path.GetDirectoryName(FullPath) + "\\").MakeRelativeUri(absolute);
            return null;
        }

#if !WINDOWS_UWP && !NET_STANDARD
        [Browsable(false)]
        public FileSystemProviderBase fileSystemProviderBase
        {
            get
            {
                if (Parent != null)
                    return Parent.fileSystemProviderBase;
                return new PhysicalFileSystemProvider("");
            }
        }
#endif
#if !WINDOWS_UWP
        [Browsable(false)]
#endif
        public String rootBase
        {
            get
            {
                if (Parent != null)
                    return Parent.rootBase;
                return Path.GetDirectoryName(FullPath);
            }
        }

#if !WINDOWS_UWP
        [Browsable(false)]
#endif
        public String rootBaseDB
        {
            get
            {
                if (Parent != null)
                    return Parent.rootBaseDB;
                return String.Empty;
            }
        }

#if !WINDOWS_UWP
        [Browsable(false)]
#endif
        public bool Protected
        {
            get
            {
                if (Parent != null)
                    return Parent.Protected;
                return false;
            }
        }

#if !WINDOWS_UWP
        [Browsable(false)]
#endif
        [EditorBrowsable(EditorBrowsableState.Never)]
        public Guid Id
        {
            get
            {
                if (Parent != null)
                    return Parent.Id;
                return Guid.Empty;
            }
        }

        public String Title
        {
            get
            {
                return Path.GetFileNameWithoutExtension(FullPath);
            }
        }

#if !WINDOWS_UWP
        [Browsable(false)]
#endif
        public String FilePath
        {
            get
            {
                return FullPath;
            }
        }

#if !WINDOWS_UWP
        [Browsable(false)]
#endif
        public bool IsEmpty
        {
            get
            {
                return false;
            }
        }

#if !WINDOWS_UWP
        [Browsable(false)]
#endif
        public bool IsRoot
        {
            get
            {
                return false;
            }
        }

        public Object GetService(Type type)
        {
            if (Parent != null)
                return Parent.GetService(type);
            return null;
        }

#endregion

#region IEntityReference Members

#if !WINDOWS_UWP
        [Browsable(false)]
#endif
        [EditorBrowsable(EditorBrowsableState.Never)]
        public ImageSource CollapsedImageSource
        {
            get
            {
                return null;
            }
        }

#if !WINDOWS_UWP
        [Browsable(false)]
#endif
        [EditorBrowsable(EditorBrowsableState.Never)]
        public ImageSource ExpandedImageSource
        {
            get
            {
                return null;
            }
        }

#if !WINDOWS_UWP
        [Browsable(false)]
#endif
        [EditorBrowsable(EditorBrowsableState.Never)]
        public
#if !WINDOWS_UWP && !NET_STANDARD
            System.Windows.Controls.
#endif
            ContextMenu contextMenu
        {
            get
            {
                return null;
            }
        }

#if !WINDOWS_UWP
        [Browsable(false)]
#endif
        [EditorBrowsable(EditorBrowsableState.Never)]
        public object Tooltip
        {
            get
            {
                return null;
            }
        }

#if !WINDOWS_UWP
        [Browsable(false)]
#endif
        [EditorBrowsable(EditorBrowsableState.Never)]
        public object ContainedObject
        {
            get
            {
                return null;
            }
        }

#if !WINDOWS_UWP
        [Browsable(false)]
#endif
        [EditorBrowsable(EditorBrowsableState.Never)]
        public object EntityParent
        {
            get
            {
                return null;
            }
        }

#if !WINDOWS_UWP
        [Browsable(false)]
#endif
        [EditorBrowsable(EditorBrowsableState.Never)]
        public string TypeDefinitionString
        {
            get
            {
                return null;
            }
        }

#endregion

#region INotifyPropertyVisibilityChanged Members

#if !WINDOWS_UWP && !NET_STANDARD
        /// <summary>
        /// Gets the visibility state for the property with the given name.
        /// </summary>
        /// <param name="propertyName">The property name that you want konw the current visibility state.</param>
        /// <returns></returns>
        bool INotifyPropertyVisibilityChanged.this[string propertyName]
        {
            get
            {
                if (propertyName == "RemoveDisabledItemAfterSecs" ||
                    propertyName == "MaxCleanCount" ||
                    propertyName == "UseAlwaysSecureConnections" ||
                    propertyName == "SlowSamplingInterval" ||
                    propertyName == "DisableWhenNotUsed" ||
                    propertyName == "PublishingInterval" ||
                    propertyName == "FastSamplingInterval")
                {
                    return !String.IsNullOrEmpty(SessionName);
                }

                return true;
            }
        }

        /// <summary>
        /// Raised when a property visibility state on this object has a new value.
        /// </summary>
        public event PropertyChangedEventHandler PropertyVisiblityChanged;

        /// <summary>
        /// Raises this object's PropertyVisiblityChanged event.
        /// </summary>
        /// <param name="propertyName">The property that has changed his value and has triggered the change of visibility.</param>
        void OnPropertyVisiblityChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyVisiblityChanged;
            if (handler != null)
            {
                var e = new PropertyChangedEventArgs(propertyName);
                handler(this, e);
            }
        }
#endif
#endregion

#region IDisposable

        protected bool bObjectDisposed;
        protected override void OnDispose()
        {
            if (bObjectDisposed)
                return;
            bObjectDisposed = true;

#if !WINDOWS_UWP && !NET_STANDARD
            if (CurrentRuntimeView != null)
            {
                CurrentRuntimeView.Close();
                CurrentRuntimeView = null;
            }
#endif
            Stop();
            OnDisposing(this);

			base.OnDispose();
        }

#endregion
    }
}
