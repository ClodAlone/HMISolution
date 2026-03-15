using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using UFInterfaces;
using UFUAEditor.Controls;
using UFUAEditor.Document;
using UIMsgBoxAlertService.ComponentService;
using Utilities;
using WPFUtilities.ImportExportHelpers;
using log4net;
using DataLoggerModel;
using DocumentManager.ComponentService;
using UFUAEditor.ComponentService;
using System.ComponentModel;
using System.Windows.Controls;
using UFUAModel;
using Opc.Ua;
using Opc.Ua.Export;
using System.IO;

namespace UFUAEditor.Helpers
{
    internal class ItemOptions<T>
    {
        public string EntryKey { get; set; }
        public Dictionary<string, string> Entry { get; set; }
        public ImportExportHelper<T> IeObjectHelper { get; set; }
        public Dictionary<string, ulong> MapStartCounter { get; set; }
        public List<string> ListNames { get; set; }
        public Dictionary<string,T> ExistingObjects { get; set; }
        public Dictionary<string, string> RenamedMap { get; set; }
        public object Root { get; set; }
        public string PrototypeName { get; set; }

        public ItemOptions(ItemOptions<T> options)
        {
            this.EntryKey = options.EntryKey;
            this.Entry = options.Entry;
            this.IeObjectHelper = options.IeObjectHelper;
            this.MapStartCounter = options.MapStartCounter;
            this.ListNames = options.ListNames;
            this.ExistingObjects = options.ExistingObjects;
            this.RenamedMap = options.RenamedMap;
            this.Root = options.Root;
            this.PrototypeName = options.PrototypeName;
        }
        public ItemOptions()
        {

        }
    }
    internal class ImportExportOptions : ImportExportBaseOptions
    {
        public CustomDialogResults dialogRetValue { get; set; }
        public Dictionary<string, string> renamedObjectMap { get; set; }
        public Dictionary<string, string> renamedObject2Map { get; set; }
        public List<string> protoError { get; set; }
        public List<string> errorMessages { get; set; }
        public List<Action<CacheTags>> pendingActions { get; set; }
        public readonly UFUAServerDocument serverDocument;

        public ImportExportOptions(UFUAServerDocument document)
            : base(document as IDocument)
        {
            this.serverDocument = document;
            errorMessages = new List<string>();
            pendingActions = new List<Action<CacheTags>>();
        }

        public ImportExportOptions(ImportExportOptions options)
            : base(options)
        {
            this.serverDocument = options.serverDocument as UFUAServerDocument;
            dialogRetValue = options.dialogRetValue;
            renamedObject2Map = options.renamedObject2Map;
            renamedObjectMap = options.renamedObjectMap;
            protoError = options.protoError;
            errorMessages = options.errorMessages;
            pendingActions = options.pendingActions;
        }
    }

    internal class DocumentImportExportHelper<T>
    {
        #region declarations
        readonly UFUAServerDocument document;
        readonly IWorkspace workspace;
        readonly Type objectType;
        readonly string title;
        readonly string helpLink;
        readonly string fileExtension;
        readonly bool showFilter = true;
        readonly bool showSeparator = true;
        static readonly ILog logGeneral = LogManager.GetLogger(Properties.Resources.ImportExportLog);
        #endregion

        #region ctor
        public DocumentImportExportHelper(UFUAServerDocument document)
        {
            if (document == null)
                return;

            this.document = document;
            this.workspace = document.GetService(typeof(IWorkspace)) as IWorkspace;
            this.objectType = typeof(T);
            fileExtension = Properties.Settings.Default.ImportExportFileExtension;
            if (objectType == typeof(UFUAModel.ConfigurationBase))
            {
                title = Properties.UICommandResource.ImportExportAddressSpaceName;
                helpLink = Properties.Settings.Default.ImportExportTagHelpLink;
                fileExtension = Properties.Settings.Default.ImportExportOPCUAFileExtension;
                showFilter = false;
                showSeparator = false;
            }
            else if (objectType == typeof(UFUAModel.UFUATag))
            {
                title = Properties.Resources.ImportTags;
                helpLink = Properties.Settings.Default.ImportExportTagHelpLink;
            }
            else if (objectType == typeof(UFUAModel.UFUATagPrototype))
            {
                title = Properties.Resources.ImportPrototypes;
                helpLink = Properties.Settings.Default.ImportExportTagPrototypeHelpLink;
            }
            else if (objectType == typeof(UFUAModel.UFUAAlarmDefinition))
            {
                title = Properties.Resources.ImportAlarms;
                helpLink = Properties.Settings.Default.ImportExportAlarmHelpLink;
            }
            else if (objectType == typeof(UFUAModel.UFUAHistorianSettings))
            {
                title = Properties.Resources.ImportHistorianSettings;
                helpLink = Properties.Settings.Default.ImportExportHistorianHelpLink;
            }
            else if (objectType == typeof(DataLoggerModel.DataLoggerSettings))
            {
                title = Properties.Resources.ImportDataloggerSettings;
                helpLink = Properties.Settings.Default.ImportExportDataloggerHelpLink;
            }
            else if (objectType == typeof(UFUAModel.UFUAEngineeringUnit))
            {
                title = Properties.Resources.ImportEUnit;
                helpLink = Properties.Settings.Default.ImportExportEUnitHelpLink;
            }
            else
            {
                title = string.Empty;
                helpLink = Properties.Settings.Default.ImportExportHelpLink;
            }
        }
        #endregion

        #region method
        internal ImportExportResult ImportExport()
        {
            if (document == null)
                return new ImportExportResult(ResultType.Failed);

            ImportExportResult result;
            var control = new ImportEditor(document);
            control.ClearValue(UserControl.HeightProperty);
            control.ClearValue(UserControl.WidthProperty);
            control.ShowFilter = showFilter;
            control.ShowSeparator = showSeparator;
            control.FileExtension = fileExtension;

            List<string> lista = new List<string>();
            control.DataContext = lista;

            GeneralDialogContent Dialog = new GeneralDialogContent(control)
            {
                Title = title,
                Owner = Application.Current.Windows.Count > 0 ? Application.Current.Windows[0] : Application.Current.MainWindow,
                HelpLink = helpLink
            };
            Dialog.Closing += (o, e) =>
            {
                if(Dialog.DialogResult != null && (bool)Dialog.DialogResult && string.IsNullOrEmpty(control.FilePath))
                {
                    var uiMsgBox = document?.GetService(typeof(IUIMsgBoxAlertService)) as IUIMsgBoxAlertService;
                    if (uiMsgBox != null)
                        uiMsgBox.ShowWarning(Properties.Resources.FilePathEmptyWarning);
                    e.Cancel = true;
                }
            };

            try
            {
                if (workspace != null)
                    workspace.ResetBusy();

                if ((bool)Dialog.ShowDialog())
                {
                    ImportExportOptions options = new ImportExportOptions(document)
                    {
                        Separator = ImportExportHelper<T>.Separators[control.Separator].ToString(),
                        FilePath = control.FilePath,
                        MatchCase = control.MatchCase,
                        Filter = control.Filter,
                        EnumAction = control.EnumAction,
                        ObjectType = typeof(T)
                    };


                    ImportExport<T> importExport = new ImportExport<T>(options);
                    result = importExport.CompleteAction();
                    if (options.errorMessages.Count > 0)
                        options.errorMessages.ForEach(e => logGeneral.Error(e));
                    result.WasImporting = options.EnumAction == ActionType.Import;
                    return result;
                }

            }
            finally
            {
                if (workspace != null)
                    workspace.RestoreBusy();
            }

            return new ImportExportResult(ResultType.Aborted);
        }
        #endregion
    }

    internal class ImportExport<T>
    {
        #region declarations
        readonly IWorkspace workspace;
        static readonly ILog logGeneral = LogManager.GetLogger(Properties.Resources.ImportExportLog);
        Dictionary<string, string> renamedProtoMap;
        Dictionary<string, string> renamedTagMap;
        List<IXPSimpleObject> addedObjects;
        List<IXPSimpleObject> changedObjects;
        List<IXPSimpleObject> removedObjects;
        ImportExportOptions options;
        #endregion

        #region ctor
        public ImportExport(ImportExportOptions options)
        {
            this.options = new ImportExportOptions(options);
            if (this.options.serverDocument != null)
                this.workspace = this.options.serverDocument.GetService(typeof(IWorkspace)) as IWorkspace;
            renamedProtoMap = options.renamedObjectMap ?? new Dictionary<string, string>();
            renamedTagMap = options.renamedObject2Map ?? new Dictionary<string, string>();
        }
        #endregion

        #region methods
        internal ImportExportResult CompleteAction()
        {
            if (options.serverDocument == null || string.IsNullOrEmpty(options.FilePath))
                return new ImportExportResult(ResultType.Failed); 

            ImportExportResult result = null;
            if (workspace != null)
                workspace.IsBusy = true;

            try
            {
                ImportExportHelper<T> ieHelper = new ImportExportHelper<T>(options);

                if (options.EnumAction == ActionType.Export)
                {
                    result = new ImportExportResult(ResultType.Failed);
                    ieHelper.CollectError += (o, e) =>
                    {
                        CollectErrors(e.Message);
                    };
                    ieHelper.GetCollectionToExport += (o, e) =>
                    {
                        List<T> objectlist = null;
                        if (options.ObjectType == typeof(UFUAModel.UFUATagPrototype))
                        {
                            if (!string.IsNullOrEmpty(options.Filter))
                                objectlist = (from t in options.serverDocument.GetPrototypes()
                                              where System.Text.RegularExpressions.Regex.IsMatch(t.Name, options.Filter, !options.MatchCase ? System.Text.RegularExpressions.RegexOptions.IgnoreCase : System.Text.RegularExpressions.RegexOptions.None)
                                              select t).ToList() as List<T>;
                            else
                                objectlist = options.serverDocument.GetPrototypes().ToList() as List<T>;
                        }
                        else if (options.ObjectType == typeof(UFUAModel.UFUATag))
                        {
                            if (options.SilentMode)
                            {
                                if(options.ParentObjectType == typeof(UFUAModel.UFUATag))
                                {
                                    if (!string.IsNullOrEmpty(options.Filter))
                                        objectlist = (from t in options.serverDocument.GetFullTagMemberImportExportCollection(false)
                                                      where System.Text.RegularExpressions.Regex.IsMatch(t.Name, options.Filter, !options.MatchCase ? System.Text.RegularExpressions.RegexOptions.IgnoreCase : System.Text.RegularExpressions.RegexOptions.None)
                                                      select t).ToList() as List<T>;
                                    else
                                        objectlist = options.serverDocument.GetFullTagMemberImportExportCollection(false).ToList() as List<T>;
                                }
                                else if (options.ParentObjectType == typeof(UFUAModel.UFUATagPrototype))
                                {
                                    if (!string.IsNullOrEmpty(options.Filter))
                                        objectlist = (from t in options.serverDocument.GetFullTagMemberImportExportCollection()
                                                      where System.Text.RegularExpressions.Regex.IsMatch(t.Name, options.Filter, !options.MatchCase ? System.Text.RegularExpressions.RegexOptions.IgnoreCase : System.Text.RegularExpressions.RegexOptions.None)
                                                      select t).ToList() as List<T>;
                                    else
                                        objectlist = options.serverDocument.GetFullTagMemberImportExportCollection().ToList() as List<T>;
                                }
                            }
                            else
                            {
                                if (!string.IsNullOrEmpty(options.Filter))
                                    objectlist = (from t in options.serverDocument.GetFlatTagCollection()
                                                  where System.Text.RegularExpressions.Regex.IsMatch(t.Name, options.Filter, !options.MatchCase ? System.Text.RegularExpressions.RegexOptions.IgnoreCase : System.Text.RegularExpressions.RegexOptions.None)
                                                  select t).ToList() as List<T>;
                                else
                                    objectlist = options.serverDocument.GetFlatTagCollection().ToList() as List<T>;
                            }
                            objectlist?.ForEach(obj => (obj as UFUAModel.UFUATag).AfterConstruction());
                        }
                        else if (options.ObjectType == typeof(UFUAModel.UFUAEngineeringUnit))
                        {
                            if (!string.IsNullOrEmpty(options.Filter))
                                objectlist = (from t in options.serverDocument.GetEngineeringUnits()
                                              where System.Text.RegularExpressions.Regex.IsMatch(t.Name, options.Filter, !options.MatchCase ? System.Text.RegularExpressions.RegexOptions.IgnoreCase : System.Text.RegularExpressions.RegexOptions.None)
                                              select t).ToList() as List<T>;
                            else
                                objectlist = options.serverDocument.GetEngineeringUnits().ToList() as List<T>;
                        }
                        else if (options.ObjectType == typeof(UFUAModel.UFUAAlarmDefinition))
                        {
                            if (!string.IsNullOrEmpty(options.Filter))
                                objectlist = (from t in options.serverDocument.GetAlarmDefinitions()
                                              where System.Text.RegularExpressions.Regex.IsMatch(t.Name, options.Filter, !options.MatchCase ? System.Text.RegularExpressions.RegexOptions.IgnoreCase : System.Text.RegularExpressions.RegexOptions.None)
                                              select t).ToList() as List<T>;
                            else
                                objectlist = options.serverDocument.GetAlarmDefinitions().ToList() as List<T>;
                        }
                        else if (options.ObjectType == typeof(UFUAModel.UFUAHistorianSettings))
                        {
                            if (!string.IsNullOrEmpty(options.Filter))
                                objectlist = (from t in options.serverDocument.GetHistoricalSettings()
                                              where System.Text.RegularExpressions.Regex.IsMatch(t.Name, options.Filter, !options.MatchCase ? System.Text.RegularExpressions.RegexOptions.IgnoreCase : System.Text.RegularExpressions.RegexOptions.None)
                                              select t).ToList() as List<T>;
                            else
                                objectlist = options.serverDocument.GetHistoricalSettings().ToList() as List<T>;
                        }
                        else if (options.ObjectType == typeof(DataLoggerModel.DataLoggerSettings))
                        {
                            if (!string.IsNullOrEmpty(options.Filter))
                                objectlist = (from t in options.serverDocument.GetDataLoggerSettings()
                                              where System.Text.RegularExpressions.Regex.IsMatch(t.Name, options.Filter, !options.MatchCase ? System.Text.RegularExpressions.RegexOptions.IgnoreCase : System.Text.RegularExpressions.RegexOptions.None)
                                              select t).ToList() as List<T>;
                            else
                                objectlist = options.serverDocument.GetDataLoggerSettings().ToList() as List<T>;
                        }
                        else if (options.ObjectType == typeof(DataLoggerModel.DataLoggerColumn))
                        {
                            objectlist = options.serverDocument.GetDataLoggerColumnSettings(allSettings: true).ToList() as List<T>;
                        }
                        e.Objectlist = objectlist;

                    };
                    var bRet = ieHelper.ExportToCsv(options.SilentMode);
                    if (options.ObjectType == typeof(UFUAModel.ConfigurationBase))
                    {
                        result = ServerImportExportNodeSet(options);
                    }
                    else if (bRet)
                    {
                        if (options.ObjectType == typeof(UFUAModel.UFUATag) && !options.SilentMode)
                        {
                            ImportExportOptions newOptions = new ImportExportOptions(options.serverDocument)
                            {
                                Separator = options.Separator,
                                FilePath = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(options.FilePath), $"{System.IO.Path.GetFileNameWithoutExtension(options.FilePath)}_{Properties.Settings.Default.ImportExportSubPrototypeMemberFileName}{System.IO.Path.GetExtension(options.FilePath).ToLower()}"),
                                MatchCase = false,
                                Filter = null,
                                EnumAction = options.EnumAction,
                                SilentMode = true,
                                ParentObjectType = typeof(UFUAModel.UFUATag),
                                ObjectType = typeof(UFUAModel.UFUATag)
                            };
                            ImportExport<UFUAModel.UFUATag> importExport = new ImportExport<UFUAModel.UFUATag>(newOptions);
                            result = importExport.CompleteAction();
                        }
                        else if (options.ObjectType == typeof(UFUAModel.UFUATagPrototype))
                        {
                            ImportExportOptions newOptions = new ImportExportOptions(options.serverDocument)
                            {
                                Separator = options.Separator,
                                FilePath = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(options.FilePath), $"{System.IO.Path.GetFileNameWithoutExtension(options.FilePath)}_{Properties.Settings.Default.ImportExportPrototypeMemberFileName}{System.IO.Path.GetExtension(options.FilePath).ToLower()}"),
                                MatchCase = false,
                                Filter = null,
                                EnumAction = options.EnumAction,
                                SilentMode = true,
                                ParentObjectType = typeof(UFUAModel.UFUATagPrototype),
                                ObjectType = typeof(UFUAModel.UFUATag)
                            };
                            ImportExport<UFUAModel.UFUATag> importExport = new ImportExport<UFUAModel.UFUATag>(newOptions);
                            result = importExport.CompleteAction();
                        }
                        else if (options.ObjectType == typeof(DataLoggerModel.DataLoggerSettings))
                        {
                            ImportExportOptions newOptions = new ImportExportOptions(options.serverDocument)
                            {
                                Separator = options.Separator,
                                FilePath = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(options.FilePath), $"{System.IO.Path.GetFileNameWithoutExtension(options.FilePath)}_{Properties.Settings.Default.ImportExportColumnListSettingsFileName}{System.IO.Path.GetExtension(options.FilePath).ToLower()}"),
                                MatchCase = false,
                                Filter = null,
                                EnumAction = options.EnumAction,
                                SilentMode = true,
                                ParentObjectType = typeof(DataLoggerModel.DataLoggerSettings),
                                ObjectType = typeof(DataLoggerModel.DataLoggerColumn)
                            };
                            ImportExport<DataLoggerModel.DataLoggerColumn> importExport = new ImportExport<DataLoggerModel.DataLoggerColumn>(newOptions);
                            result = importExport.CompleteAction();
                        }
                    }
                }
                else
                {
                    workspace?.UpdateProgressState(0,
                                           0,
                                          $"{Properties.Resources.WorkInProgress}",
                                          TaskbarItemProgressState.Indeterminate);

                    Dictionary<string,T> existingObjects = null;
                    List<string> listNames = null;
                    object root = null;
                    string protoPath = string.Empty;
                    string subProtoPath = string.Empty;
                    string subProtoName = string.Empty;
                    bool bInit = false;
                    var mapStartCounter = new Dictionary<string, ulong>();
                    var tagOwner = string.Empty;
                    if (addedObjects == null)
                        addedObjects = new List<IXPSimpleObject>();
                    addedObjects.Clear();
                    if (removedObjects == null)
                        removedObjects = new List<IXPSimpleObject>();
                    removedObjects.Clear();
                    if (changedObjects == null)
                        changedObjects = new List<IXPSimpleObject>();
                    changedObjects.Clear();
                    UnitOfWork uow = (options.document as UFUAServerDocument).GetSession();
                    ieHelper.AddPath += (o, e) =>
                    {
                        if (options.ObjectType == typeof(UFUAModel.UFUATag) &&
                            (options.ParentObjectType == null ||
                             options.ParentObjectType == typeof(UFUAModel.UFUATagPrototype) ||
                             options.ParentObjectType == typeof(UFUAModel.UFUATag)))
                        {
                            string locProto = e.Proto;
                            string locSubProto = e.SubProto;
                            if (renamedProtoMap.ContainsKey(locProto))
                                locProto = renamedProtoMap[locProto];
                            var prototype = options.serverDocument.GetPrototype(locProto, locSubProto);
                            e.Root = options.serverDocument.AddNewFolder(e.Path.Replace("\\", "/"), uow, prototype, addedObjects);
                        }
                        else if (options.ObjectType == typeof(UFUAModel.UFUAAlarmDefinition))
                        {
                            if (!string.IsNullOrEmpty(e.Path))
                                e.Root = options.serverDocument.AddNewAlarmSource(e.Path.Replace("\\", "/"), uow, addedObjects);
                        }
                        else if (options.ObjectType == typeof(DataLoggerModel.DataLoggerColumn))
                        {
                            if (!string.IsNullOrEmpty(e.Path))
                            {
                                string dlrName = e.Path;
                                if (renamedProtoMap.ContainsKey(dlrName))
                                    dlrName = renamedProtoMap[dlrName];
                                e.Root = options.serverDocument.AddNewDatalogger(dlrName.Replace("\\", "/"), uow);
                            }
                        }
                    };
                    List<string> subPrototypeCreated = null;
                    List<string> subPrototypeError = null;
                    List<string> prototypeError = null;
                    ieHelper.CollectError += (o, e) =>
                    {
                        CollectErrors(e.Message);
                    };
                    ieHelper.UpdateProgress += (o, e) =>
                    {
                        if (workspace != null)
                        {
                            if (e.Counter == -1)
                            {
                                workspace?.IncrementProgressState();
                            }
                            else
                            {
                                workspace?.UpdateProgressState(0,
                                           e.Counter,
                                           $"{Properties.Resources.WorkInProgress}",
                                           TaskbarItemProgressState.Normal);
                            }
                        }
                        
                    };

                    ieHelper.AddNewObject += (o, e) =>
                    {
                        string protoName = e.ProtoName;
                        string locProto = e.Proto;
                        var locTagOwner = e.SubProto;

                        if (locTagOwner.StartsWith("/") && locTagOwner.Length > 1)
                            locTagOwner = locTagOwner.Substring(1);

                        bool bForceOp = false;
                        string name = null;
                        if (options.ParentObjectType == typeof(UFUAModel.UFUATag))
                        {
                            if (subPrototypeCreated == null)
                                subPrototypeCreated = new List<string>();
                            if (subPrototypeError == null)
                                subPrototypeError = new List<string>();
                            if (!string.IsNullOrEmpty(locTagOwner) && !subPrototypeCreated.Contains(locTagOwner))
                            {
                                if (subPrototypeError.Contains(locTagOwner))
                                    return;
                                string _locTagOwner = locTagOwner;
                                if (renamedTagMap.ContainsKey(locTagOwner))
                                    locTagOwner = renamedTagMap[locTagOwner];
                                var tagFound = options.serverDocument.GetUFUATag(locTagOwner);
                                UFUAModel.UFUATagPrototype proto = null;
                                if (tagFound != null)
                                {
                                    proto = options.serverDocument.CreateSubPrototype(tagFound);
                                    if (!string.IsNullOrEmpty(locProto) && proto == null)
                                    {
                                        if (!subPrototypeError.Contains(locTagOwner))
                                            subPrototypeError.Add(locTagOwner);

                                        CollectErrors(string.Format(Properties.Resources.ImportSubPrototypeError, locTagOwner, locProto));
                                        return;
                                    }
                                    bForceOp = true;
                                    subPrototypeCreated.Add(_locTagOwner);
                                }
                                else
                                {
                                    if (!subPrototypeError.Contains(locTagOwner))
                                        subPrototypeError.Add(locTagOwner);
                                    if (!options.protoError.Contains(locProto))
                                        CollectErrors(string.Format(Properties.Resources.ImportSubPrototypeError, locTagOwner, locProto));
                                    return;
                                }
                            }
                        }
                        else if (options.ParentObjectType == null && options.ObjectType == typeof(UFUAModel.UFUATag))
                        {
                            name = ieHelper.GetObjectFullName(e.RawData);
                            if (prototypeError == null)
                                prototypeError = new List<string>();
                            if (!string.IsNullOrEmpty(protoName))
                            {
                                if (prototypeError.Contains(protoName) || options.serverDocument.GetPrototype(protoName) == null)
                                {
                                    if (!prototypeError.Contains(protoName))
                                        prototypeError.Add(protoName);
                                    CollectErrors(string.Format(Properties.Resources.ImportSubPrototypeError, name, protoName));
                                    return;
                                }
                            }
                        }

                        if (e.Root != root || !bInit || locProto != protoPath || bForceOp)
                        {
                            if (renamedProtoMap.ContainsKey(locProto))
                                locProto = renamedProtoMap[locProto];
                            mapStartCounter = new Dictionary<string, ulong>();

                            GetExistingObjectCollection(ieHelper,out existingObjects, out listNames, e.Root, options, locProto, locTagOwner);
                        }
                        bInit = true;
                        root = e.Root;
                        protoPath = locProto;
                        tagOwner = locTagOwner;

                        if (options.ParentObjectType == typeof(UFUAModel.UFUATag))
                        {
                            if (listNames == null && listNames.Count() <= 0)
                                return;
                        }

                        Helpers.ItemOptions<T> objectOptions = new Helpers.ItemOptions<T>()
                        {
                            EntryKey = e.EntryKey,
                            Entry = e.RawData,
                            IeObjectHelper = ieHelper,
                            MapStartCounter = mapStartCounter,
                            ListNames = listNames,
                            ExistingObjects = existingObjects,
                            RenamedMap = renamedProtoMap,
                            Root = e.Root,
                            PrototypeName = locProto
                        };

                        e.ImportedObject = AddImportedObject(objectOptions);
                        e.ExitRequest = options.dialogRetValue == CustomDialogResults.Cancel;
                        if (e.ImportedObject != null)
                        {
                            if (options.ParentObjectType == null && options.ObjectType == typeof(UFUAModel.UFUATag))
                            {
                                var _name = (e.ImportedObject as UFUAModel.UFUATag).GetFullName();
                                if (_name != name && !renamedTagMap.ContainsKey(name))
                                    renamedTagMap.Add(name, _name);
                            }
                        }
                    };

                    ieHelper.UpdateAggregated += (o, e) =>
                    {
                        UpdateAggregated(e.RawData, e.ImportedObject, renamedProtoMap);

                        if (options.ParentObjectType == null && options.ObjectType == typeof(UFUAModel.UFUATag))
                        {
                            var tagfound = (e.ImportedObject as UFUAModel.UFUATag);
                            if (tagfound != null)
                            {
                                var _name = tagfound.GetFullName();
                                if (!string.IsNullOrEmpty(tagfound.PrototypeName) && !prototypeError.Contains(tagfound.PrototypeName))
                                    options.serverDocument.CreateSubPrototype(tagfound);
                            }
                        }
                    };

                    if (options.ObjectType == typeof(UFUAModel.ConfigurationBase))
                    {
                        result = ServerImportExportNodeSet(options);
                    }
                    else
                    {
                        var bRet = ieHelper.ImportFromCsv(options.SilentMode);
                        if (options.pendingActions.Count > 0)
                        {
                            CacheTags mapTags = options.serverDocument.InitCacheTags();
                            options.pendingActions.ForEach(a =>
                            {
                                a(mapTags);
                            });
                            options.pendingActions.Clear();
                            mapTags.BackslashTags.Clear();
                            mapTags.UnderscoreTags.Clear();
                        }

                        if (bRet && options.ObjectType == typeof(UFUAModel.UFUATag) && !options.SilentMode)
                        {
                            ImportExportOptions newOptions = new ImportExportOptions(options.serverDocument)
                            {
                                Separator = options.Separator,
                                FilePath = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(options.FilePath), $"{System.IO.Path.GetFileNameWithoutExtension(options.FilePath)}_{Properties.Settings.Default.ImportExportSubPrototypeMemberFileName}{System.IO.Path.GetExtension(options.FilePath).ToLower()}"),
                                MatchCase = false,
                                Filter = null,
                                EnumAction = options.EnumAction,
                                renamedObjectMap = renamedProtoMap,
                                renamedObject2Map = renamedTagMap,
                                protoError = prototypeError,
                                dialogRetValue = CustomDialogResults.NoAll,
                                SilentMode = true,
                                ParentObjectType = typeof(UFUAModel.UFUATag),
                                ObjectType = typeof(UFUAModel.UFUATag)
                            };
                            ImportExport<UFUAModel.UFUATag> importExport = new ImportExport<UFUAModel.UFUATag>(newOptions);
                            result = importExport.CompleteAction();
                        }
                        else if (bRet && options.ObjectType == typeof(UFUAModel.UFUATagPrototype))
                        {
                            ImportExportOptions newOptions = new ImportExportOptions(options.serverDocument)
                            {
                                Separator = options.Separator,
                                FilePath = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(options.FilePath), $"{System.IO.Path.GetFileNameWithoutExtension(options.FilePath)}_{Properties.Settings.Default.ImportExportPrototypeMemberFileName}{System.IO.Path.GetExtension(options.FilePath).ToLower()}"),
                                MatchCase = false,
                                Filter = null,
                                EnumAction = options.EnumAction,
                                renamedObjectMap = renamedProtoMap,
                                renamedObject2Map = renamedTagMap,
                                dialogRetValue = CustomDialogResults.NoAll,
                                SilentMode = true,
                                ParentObjectType = typeof(UFUAModel.UFUATagPrototype),
                                ObjectType = typeof(UFUAModel.UFUATag)
                            };
                            ImportExport<UFUAModel.UFUATag> importExport = new ImportExport<UFUAModel.UFUATag>(newOptions);
                            result = importExport.CompleteAction();
                        }
                        else if (bRet && options.ObjectType == typeof(DataLoggerModel.DataLoggerSettings))
                        {
                            ImportExportOptions newOptions = new ImportExportOptions(options.serverDocument)
                            {
                                Separator = options.Separator,
                                FilePath = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(options.FilePath), $"{System.IO.Path.GetFileNameWithoutExtension(options.FilePath)}_{Properties.Settings.Default.ImportExportColumnListSettingsFileName}{System.IO.Path.GetExtension(options.FilePath).ToLower()}"),
                                MatchCase = false,
                                Filter = null,
                                EnumAction = options.EnumAction,
                                renamedObjectMap = renamedProtoMap,
                                renamedObject2Map = renamedTagMap,
                                dialogRetValue = CustomDialogResults.NoAll,
                                SilentMode = true,
                                ParentObjectType = typeof(DataLoggerModel.DataLoggerSettings),
                                ObjectType = typeof(DataLoggerModel.DataLoggerColumn)
                            };
                            ImportExport<DataLoggerModel.DataLoggerColumn> importExport = new ImportExport<DataLoggerModel.DataLoggerColumn>(newOptions);
                            result = importExport.CompleteAction();
                        }
                        else
                            result = new ImportExportResult(ResultType.Successfully);
                    }
                }

            }
            catch (Exception ex)
            {
                string error = string.Empty;
                if (options.EnumAction == ActionType.Import)
                    error = String.Format(Properties.Resources.ImportError, options.FilePath, ex.Message);
                else
                    error = $"{Properties.Resources.ExportError}: {ex.Message}";

                CollectErrors(error);

                result = new ImportExportResult(ResultType.Failed);
            }
            finally
            {
                if (workspace != null)
                {
                    workspace.ResetProgressState();
                    workspace.IsBusy = false;
                }
            }

            if (options.EnumAction == ActionType.Import)
            {
                if(result != null)
                {
                    if (result.RemovedObjects != null)
                    {
                        if (removedObjects == null)
                            removedObjects = new List<IXPSimpleObject>(result.RemovedObjects);
                        else
                            removedObjects.AddRange(result.AddedObjects);
                    }
                    if (result.AddedObjects != null)
                    {
                        if (addedObjects == null)
                            addedObjects = new List<IXPSimpleObject>(result.AddedObjects);
                        else
                            addedObjects.AddRange(result.AddedObjects);
                    }
                    if (result.ChangedObjects != null)
                    {
                        if (changedObjects == null)
                            changedObjects = new List<IXPSimpleObject>(result.ChangedObjects);
                        else
                            changedObjects.AddRange(result.ChangedObjects);
                    }
                }
                if (options.errorMessages.Count > 0)
                    return new ImportExportResult(ResultType.Successfully) { WasError = true, RemovedObjects = removedObjects, AddedObjects = addedObjects, ChangedObjects = changedObjects };
                else
                    return new ImportExportResult(ResultType.Successfully) { RemovedObjects = removedObjects, AddedObjects = addedObjects, ChangedObjects = changedObjects };
            }

            if (options.errorMessages.Count > 0)
                return new ImportExportResult(ResultType.Successfully) { WasError = true };

            return new ImportExportResult(ResultType.Successfully);
        }

        private void CollectErrors(string error)
        {
            if (!string.IsNullOrEmpty(error) && !options.errorMessages.Contains(error))
                options.errorMessages.Add(error);
        }

        private ImportExportResult ServerImportExportNodeSet(ImportExportOptions options)
        {
            if (options == null || String.IsNullOrEmpty(options.FilePath) || options.document == null)
                return new ImportExportResult(ResultType.Failed);

            string file = options.FilePath;
            ImportExportResult ret = null;

            if (options.EnumAction == ActionType.Export)
            {
                if (workspace != null)
                {
                    workspace.IsBusy = true;
                    workspace.UpdateProgressState(0,
                                              0,
                                              $"{Properties.UICommandResource.ImportExportAddressSpaceDescription}: {Properties.Resources.WorkInProgress}",
                                              UFInterfaces.TaskbarItemProgressState.Indeterminate);
                }
                try
                {
                    using (var nodeset = new UANodeSet(options.document as UFUAServerDocument))
                    {
                        ret = nodeset.Export();
                        using (var ostrm = File.Open($"{file}", FileMode.Create, FileAccess.ReadWrite))
                        {
                            nodeset.Write(ostrm);
                        };
                    }
                }
                finally
                {
                    if (workspace != null)
                    {
                        workspace.ResetProgressState();
                        workspace.IsBusy = false;
                    }

                    if (ret != null && ret.WasError)
                    {
                        var uiMsgBox = options?.document?.GetService(typeof(IUIMsgBoxAlertService)) as IUIMsgBoxAlertService;
                        if (uiMsgBox != null)
                        {
                            uiMsgBox.ShowError(string.Format(Properties.Resources.ExportErrorOn, file, Properties.Resources.OpenSysLogWarning));
                        }
                    }
                }

                return new ImportExportResult(ResultType.Successfully);
            }
            else
            {
                if (workspace != null)
                {
                    workspace.IsBusy = true;
                    workspace.UpdateProgressState(0,
                                              0,
                                              $"{Properties.UICommandResource.ImportExportAddressSpaceDescription}: {Properties.Resources.WorkInProgress}",
                                              UFInterfaces.TaskbarItemProgressState.Indeterminate);
                }

                try
                {
                    using (var ostrm = File.Open($"{file}", FileMode.Open, FileAccess.ReadWrite))
                    {
                        using (var nodeset = UANodeSet.Read(ostrm))
                        {
                            var doc = options.document as UFUAServerDocument;
                            ret = nodeset.Import(doc);
                        }
                    };
                }
                finally
                {
                    if (workspace != null)
                    {
                        workspace.ResetProgressState();
                        workspace.IsBusy = false;
                    }

                    if (ret != null && ret.WasError)
                    {
                        var uiMsgBox = options.document.GetService(typeof(IUIMsgBoxAlertService)) as IUIMsgBoxAlertService;
                        if (uiMsgBox != null)
                        {
                            uiMsgBox.ShowError(string.Format(Properties.Resources.ImportError, file, Properties.Resources.OpenSysLogWarning));
                        }
                    }
                }
                return ret;
            }
        }
        private void UpdateAggregated(Dictionary<string, string> rawData, T importedObject, Dictionary<string, string> renamedMap = null)
        {
            if (rawData == null || rawData.Count == 0)
                return;
            if (options.ObjectType == typeof(UFUAModel.UFUATag))
            {
                UFUAModel.UFUATag tag = importedObject as UFUAModel.UFUATag;
                string key = "AlarmList";
                string subKey = string.Empty;
                string path = string.Empty;
                string subPath = string.Empty;
                if (rawData.ContainsKey(key))
                {
                    path = rawData[key];
                    string result = string.Empty;
                    if (!string.IsNullOrEmpty(path))
                    {
                        result = AssignAlarms(tag, path);
                        CollectErrors(result);
                    }
                }

                key = "HistorianSettings";
                if (rawData.ContainsKey(key))
                {
                    path = rawData[key];
                    if (!string.IsNullOrEmpty(path))
                    {
                        if (options.serverDocument.HistoricalSettingsNameExists(path))
                            tag.HistorianSettings = path;
                        else
                        {
                            CollectErrors(string.Format(Properties.Resources.ImportTagHistorianNotExists, path, tag.Name));
                            tag.HistorianSettings = string.Empty;
                        }
                    }
                }

                key = "Views";
                if (rawData.ContainsKey(key))
                {
                    path = rawData[key];
                    if (!string.IsNullOrEmpty(path))
                    {
                        path = path.Replace(", ", "|");
                        options.serverDocument.AssignViews(tag, path);
                    }
                }

                if (options.ParentObjectType == typeof(UFUAModel.UFUATagPrototype))
                {
                    key = "PrototypeReferenceName";
                    if (rawData.ContainsKey(key))
                    {
                        path = rawData[key];
                        if (tag.UFUAFolder == null)
                        {
                            if (renamedMap.ContainsKey(path))
                                path = renamedMap[path];
                            var prototype = options.serverDocument.GetPrototype(path);
                            (importedObject as UFUAModel.UFUATag).UFUATagPrototype = prototype;
                        }
                    }
                }
            }
            else if (options.ObjectType == typeof(DataLoggerModel.DataLoggerSettings))
            {
                AssignDataloggerTags(importedObject as DataLoggerModel.DataLoggerSettings, rawData);
            }
            else if (options.ObjectType == typeof(DataLoggerModel.DataLoggerColumn))
            {
                AssignColumnTag(importedObject as DataLoggerModel.DataLoggerColumn, rawData.Values.FirstOrDefault());
            }


            return;
        }

        IUFUAEditorManager editor;
        UFUAModel.TagEntityReference GetTagReference(string path, CacheTags mapTags)
        {
            if (mapTags != null)
            {
                string tagName = options.serverDocument.GetRelativePath(path);

                var split = tagName.Split(':');
                var tagString = tagName;
                String member = null;
                string instance = null;
                string name = split[0];
                if (split.Length > 1)
                {
                    name = split[1];
                    instance = split[0];
                    tagString = instance;
                    member = name;
                }

                UFUAModel.UFUATag tagfound = null;
                if (mapTags.BackslashTags.ContainsKey(tagString))
                    tagfound = mapTags.BackslashTags[tagString];
                else if (tagString.Contains('\\'))
                {
                    var parts = tagString.Split('\\');
                    string instanceCheck = null;
                    List<UFUAModel.UFUATag> list = new List<UFUAModel.UFUATag>();
                    for (int i = 0; i < parts.Count(); i++)
                    {
                        instanceCheck = String.Join("\\", parts.SubArray(0,parts.Count() - i));

                        if (mapTags.BackslashTags.ContainsKey(instanceCheck))
                        {
                            tagfound = mapTags.BackslashTags[instanceCheck];
                            instance = instanceCheck;
                            name = tagString.Replace(String.Format("{0}\\", instanceCheck), "");

                            if(tagfound != null)
                                break;
                        }
                    }
                }
                else if (tagString.Contains('_'))
                {
                    if (mapTags.UnderscoreTags.ContainsKey(tagString))
                        tagfound = mapTags.UnderscoreTags[tagString];
                }

                if (tagfound != null)
                {
                    List<UFUAModel.UFUATag> list = new List<UFUAModel.UFUATag>();
                    if (!String.IsNullOrEmpty(instance))
                        tagfound = options.serverDocument.GetInstanceTag(tagfound, name, instance, list);
                    if (tagfound != null)
                    {
                        var entityReference = options.serverDocument.GetTagOPCUAEntityReference(tagfound, list, false);
                        if (entityReference == null)
                            return null;
                        Guid tagGuid = Guid.Empty;
                        if (entityReference.ResolvedNodeId.IdType == Opc.Ua.IdType.Guid)
                            tagGuid = (Guid)entityReference.ResolvedNodeId.Identifier;
                        else if (entityReference.ResolvedNodeId.IdType == Opc.Ua.IdType.String)
                        {
                            var identifier = entityReference.ResolvedNodeId.Identifier.ToString();
                            var index = identifier.LastIndexOf('?');
                            if (index != -1)
                            {
                                identifier = identifier.Substring(index + 1);
                                identifier = identifier.Replace('/', '\\').Split('\\').LastOrDefault(); 
                            }
                            Guid.TryParse(identifier, out tagGuid);
                        }

                        if (tagGuid == Guid.Empty)
                            return null;
                        return new UFUAModel.TagEntityReference(tagGuid, entityReference.RelativePath, entityReference.ResolvedNodeId, entityReference.HumanReadable);
                    }
                }
            }
            return null;
        }
        UFUAModel.TagEntityReference GetTagReference(string path)
        {
            var split = path.Split(':');
            string member = null;
            var name = split[0];
            if (split.Length > 1)
                member = split[1];
            return UFUAEditorManagerComponent.ufuaEditorManagerComponent.GetObjectTagEntityReference(options.serverDocument, name, member);
        }
        private void AssignColumnTag(DataLoggerColumn dataLoggerColumn, string path)
        {
            if (string.IsNullOrEmpty(path))
                return;
            Action<CacheTags> action = new Action<CacheTags>((o) =>
            {   
                dataLoggerColumn.ColumnTag = GetTagReference(path, o);
            });
            options.pendingActions.Add(action);
        }

        private void AssignDataloggerTags(DataLoggerSettings dataLoggerSettings, Dictionary<string, string> values)
        {
            if (values == null || values.Count == 0)
                return;

            foreach (string key in values.Keys)
            {
                if (key == "EnableRecordingTagName")
                {
                    var path = values[key];
                    if (!string.IsNullOrEmpty(path))
                    {
                        Action<CacheTags> action = new Action<CacheTags>((o) =>
                        {
                            dataLoggerSettings.EnableRecordingTag = GetTagReference(path, o);
                        });
                        options.pendingActions.Add(action);
                    }
                }
                if (key == "RecordingTagName")
                {
                    var path = values[key];
                    if (!string.IsNullOrEmpty(path))
                    {
                        Action<CacheTags> action = new Action<CacheTags>((o) =>
                        {
                            dataLoggerSettings.RecordingTag = GetTagReference(path, o);
                        });
                        options.pendingActions.Add(action);
                    }
                }
                if (key == "ResettingTagName")
                {
                    var path = values[key];
                    if (!string.IsNullOrEmpty(path))
                    {
                        Action<CacheTags> action = new Action<CacheTags>((o) =>
                        {
                            dataLoggerSettings.ResettingTag = GetTagReference(path, o);
                        });
                        options.pendingActions.Add(action);
                    }
                }
            }
        }

        private string AssignAlarms(UFUAModel.UFUATag tag, string alarms)
        {
            Dictionary<string, UFUAModel.UFUAAlarmThreshold> thresholdList = new Dictionary<string, UFUAAlarmThreshold>();
            tag.UFUAAlarmThresholds?.ToList().ForEach(alarm =>
            {
                thresholdList.Add(GetAlarmPath(alarm), alarm);
            });
            UnitOfWork uow = (options.document as UFUAServerDocument).GetSession();
            List<UFUAModel.UFUAAlarmThreshold> addedThresholds = new List<UFUAModel.UFUAAlarmThreshold>();

            if (string.IsNullOrEmpty(alarms))
                return null;
            StringBuilder result = new StringBuilder();
            var alarmList = alarms.Split('|').ToList();
            alarmList.ForEach(p =>
            {
                try
                {
                    if (!string.IsNullOrEmpty(p))
                    {
                        var composite = p.Split('@');
                        var alarm = composite?.FirstOrDefault().Replace('#', '/');
                        var path = composite?.FirstOrDefault()?.Split('#')?.FirstOrDefault()?.Split('/')?.ToList();
                        if (path.Count() < 3)
                            throw new Exception(string.Format(Properties.Resources.InvalidAlarmDefinitionDuringTagImporting, tag.GetFullName(), alarm));

                        var alrmText = composite?.FirstOrDefault()?.Split('#')?.LastOrDefault();
                        var optionList = composite?.LastOrDefault();
                        var alarmname = path.LastOrDefault();
                        path.RemoveAt(path.Count() - 1);
                        var sourcename = path.Count() >= 1 ? path.LastOrDefault() : string.Empty;

                        UFUAModel.UFUAAlarmThreshold alarmthreshold = null;
                        var key = $"{alarm} - {optionList}";
                        if (thresholdList.ContainsKey(key))
                        {
                            alarmthreshold = thresholdList[key];
                        }
                        else
                        {
                            UFUAModel.UFUAArea parent = null;
                            if (!string.IsNullOrEmpty(sourcename))
                            {
                                for (int i = 0; i < path.Count() - 1; i++)
                                {
                                    UFUAModel.UFUAArea alarmarea;
                                    if (i == 0)
                                    {
                                        alarmarea = (from area in new XPQuery<UFUAModel.UFUAArea>(uow, true).AsParallel()
                                                     where area.UFUAAreaAss == null && area.Name == path[i]
                                                     select area).FirstOrDefault();
                                        parent = alarmarea ?? throw new Exception(string.Format(Properties.Resources.InvalidAlarmDefinitionDuringTagImporting, tag.GetFullName(), alarm));
                                    }
                                    else
                                    {
                                        alarmarea = (from area in new XPQuery<UFUAModel.UFUAArea>(uow, true).AsParallel()
                                                     where area.UFUAAreaAss == parent && area.Name == path[i]
                                                     select area).FirstOrDefault();
                                        parent = alarmarea ?? throw new Exception(string.Format(Properties.Resources.InvalidAlarmDefinitionDuringTagImporting, tag.GetFullName(), alarm));
                                    }
                                }

                                UFUAModel.UFUAAlarmSource alarmsource;
                                alarmsource = (from area in new XPQuery<UFUAModel.UFUAAlarmSource>(uow, true).AsParallel()
                                               where area.UFUAArea == parent && area.Name == sourcename
                                               select area).FirstOrDefault();
                                UFUAModel.UFUAAlarmDefinition alarmdef;
                                if (alarmsource != null)
                                {
                                    alarmdef = (from alarmdefinition in new XPQuery<UFUAModel.UFUAAlarmDefinition>(uow, true).AsParallel()
                                                where alarmdefinition.Name == alarmname && alarmdefinition.UFUAAlarmDefinitions == alarmsource
                                                select alarmdefinition).FirstOrDefault();

                                    if (alarmdef == null)
                                        throw new Exception(string.Format(Properties.Resources.InvalidAlarmDefinitionDuringTagImporting, tag.GetFullName(), alarm));

                                    alarmthreshold = new UFUAModel.UFUAAlarmThreshold(alarmdef, uow);
                                }
                                else
                                    throw new Exception(string.Format(Properties.Resources.InvalidAlarmDefinitionDuringTagImporting, tag.GetFullName(), alarm));

                            }
                            else
                                throw new Exception(string.Format(Properties.Resources.InvalidAlarmDefinitionDuringTagImporting, tag.GetFullName(), alarm));

                        }

                        if (alarmthreshold != null)
                        {
                            addedThresholds.Add(alarmthreshold);
                            alarmthreshold.AlarmText = alrmText;
                            var thrOptions = optionList?.Split('#');
                            var optionCount = thrOptions.Count();
                            if (optionCount > 0)
                                alarmthreshold.Expression = thrOptions[0];
                            if (optionCount > 1)
                            {
                                bool enabled;
                                if (bool.TryParse(thrOptions[1], out enabled))
                                    alarmthreshold.AddTagDescription = enabled;
                            }
                            if (optionCount > 2)
                            {
                                bool enabled;
                                if (bool.TryParse(thrOptions[2], out enabled))
                                    alarmthreshold.Enabled = enabled;
                            }
                            if (optionCount > 3 && !string.IsNullOrEmpty(thrOptions[3]))
                            {
                                string tagPath = thrOptions[3];
                                if (tagPath.StartsWith("Tags/"))
                                    tagPath = tagPath.Replace("Tags/", "");
                                Action<CacheTags> action = new Action<CacheTags>((o) =>
                                {
                                    alarmthreshold.EnableTag = GetTagReference(tagPath, o);
                                });
                                options.pendingActions.Add(action);
                            }
                            if (optionCount > 4)
                            {
                                UFUAModel.ThreeStateType enableQualityGood;
                                if (Enum.TryParse(thrOptions[4], out enableQualityGood))
                                    alarmthreshold.EnableQualityGood = enableQualityGood;
                            }
                            if (optionCount > 5 && !string.IsNullOrEmpty(thrOptions[5]))
                            {
                                string tagPath = thrOptions[5];
                                if (tagPath.StartsWith("Tags/"))
                                    tagPath = tagPath.Replace("Tags/", "");
                                Action<CacheTags> action = new Action<CacheTags>((o) =>
                                {
                                    alarmthreshold.ActivationLowValueTag = GetTagReference(tagPath, o);
                                });
                                options.pendingActions.Add(action);
                            }
                            if (optionCount > 6 && !string.IsNullOrEmpty(thrOptions[6]))
                            {
                                string tagPath = thrOptions[6];
                                if (tagPath.StartsWith("Tags/"))
                                    tagPath = tagPath.Replace("Tags/", "");
                                Action<CacheTags> action = new Action<CacheTags>((o) =>
                                {
                                    alarmthreshold.ActivationValueTag = GetTagReference(tagPath, o);
                                });
                                options.pendingActions.Add(action);
                            }
                            if (optionCount > 7)
                            {
                                UFUAModel.ThreeStateType beep;
                                if (Enum.TryParse(thrOptions[7], out beep))
                                    alarmthreshold.Beep = beep;
                            }
                            if (optionCount > 8)
                            {
                                int oid;
                                if (int.TryParse(thrOptions[8], out oid))
                                    alarmthreshold.Oid = oid;
                            }
                            if (optionCount > 9 && !string.IsNullOrEmpty(thrOptions[9]))
                            {
                                string tagPath = thrOptions[9];
                                if (tagPath.StartsWith("Tags/"))
                                    tagPath = tagPath.Replace("Tags/", "");
                                Action<CacheTags> action = new Action<CacheTags>((o) =>
                                {
                                    alarmthreshold.HighHighLimitTag = GetTagReference(tagPath, o);
                                });
                                options.pendingActions.Add(action);
                            }
                            if (optionCount > 10 && !string.IsNullOrEmpty(thrOptions[10]))
                            {
                                string tagPath = thrOptions[10];
                                if (tagPath.StartsWith("Tags/"))
                                    tagPath = tagPath.Replace("Tags/", "");
                                Action<CacheTags> action = new Action<CacheTags>((o) =>
                                {
                                    alarmthreshold.HighLimitTag = GetTagReference(tagPath, o);
                                });
                                options.pendingActions.Add(action);
                            }
                            if (optionCount > 11 && !string.IsNullOrEmpty(thrOptions[11]))
                            {
                                string tagPath = thrOptions[11];
                                if (tagPath.StartsWith("Tags/"))
                                    tagPath = tagPath.Replace("Tags/", "");
                                Action<CacheTags> action = new Action<CacheTags>((o) =>
                                {
                                    alarmthreshold.LowLimitTag = GetTagReference(tagPath, o);
                                });
                                options.pendingActions.Add(action);
                            }
                            if (optionCount > 12 && !string.IsNullOrEmpty(thrOptions[12]))
                            {
                                string tagPath = thrOptions[12];
                                if (tagPath.StartsWith("Tags/"))
                                    tagPath = tagPath.Replace("Tags/", "");
                                Action<CacheTags> action = new Action<CacheTags>((o) =>
                                {
                                    alarmthreshold.LowLowLimitTag = GetTagReference(tagPath, o);
                                });
                                options.pendingActions.Add(action);
                            }
                            if (optionCount > 13)
                            {
                                string tagPath = thrOptions[13];
                                if (tagPath.StartsWith("Tags/"))
                                    tagPath = tagPath.Replace("Tags/", "");
                                Action<CacheTags> action = new Action<CacheTags>((o) =>
                                {
                                    alarmthreshold.SeverityTag = GetTagReference(tagPath, o);
                                });
                                options.pendingActions.Add(action);
                            }
                            if (optionCount > 14)
                                alarmthreshold.SeverityExpression = thrOptions[14];
                            if (optionCount > 15)
                            {
                                while (alarmthreshold.AliasTags.Count > 0)
                                    alarmthreshold.AliasTags[0].Delete();

                                for (int i = 15; i < optionCount; i++)
                                {
                                    if (optionCount > i + 1)
                                    {
                                        string tagPath = thrOptions[i];
                                        int oid = int.Parse(thrOptions[i + 1]);
                                        if (tagPath.StartsWith("Tags/"))
                                            tagPath = tagPath.Replace("Tags/", "");
                                        {
                                            Action<CacheTags> action = new Action<CacheTags>((o) =>
                                            {
                                                alarmthreshold.AliasTags.Add(new UFUAModel.XPTagEntityReference(uow) { TagEntity = GetTagReference(tagPath), Oid = oid });
                                            });
                                            options.pendingActions.Add(action);

                                        }
                                        i++;
                                    }
                                }
                            }
                            
                            
                            if (!tag.UFUAAlarmThresholds.Contains(alarmthreshold))
                                tag.UFUAAlarmThresholds.Add(alarmthreshold);
                        }
                        else
                            throw new Exception(string.Format(Properties.Resources.InvalidAlarmDefinitionDuringTagImporting, tag.GetFullName(), alarm));
                    }
                }
                catch (Exception ex)
                {
                    if (result.Length > 0)
                        result.Append(Environment.NewLine);
                    result.Append(ex.Message);
                }
            });

            thresholdList.Clear();

            for (int i = 0; i < tag.UFUAAlarmThresholds?.Count; i++)
            {
                var alarm = tag.UFUAAlarmThresholds[i];
                if (!addedThresholds.Contains(alarm))
                {
                    tag.UFUAAlarmThresholds.Remove(alarm);
                    alarm.Delete();
                    if (alarm is IXPSimpleObject && !removedObjects.Contains(alarm))
                        removedObjects.Add(alarm);
                }
            }

            tag.NotifyPropertyChanged("UFUAAlarmThresholds");
            return result.ToString();
        }

        private string GetAlarmPath(UFUAAlarmThreshold alarm)
        {
            if (alarm.UFUAAlarmDefinitionRef != null)
                return string.Format("{0}/{1}/{2} - {3}", alarm.UFUAAlarmDefinitionRef.SourcePath, alarm.UFUAAlarmDefinitionRef.Name, alarm.AlarmText, alarm.AlarmOptions);
            else
                return alarm.CompleteName;
        }

        internal T AddImportedObject(ItemOptions<T> objectOptions)
        {
            if (objectOptions.Entry == null || string.IsNullOrEmpty(objectOptions.EntryKey))
                return default(T);
            if (objectOptions.ExistingObjects != null && objectOptions.ExistingObjects.ContainsKey(objectOptions.EntryKey))
            {
                var existingObject = objectOptions.ExistingObjects[objectOptions.EntryKey];
                if (existingObject != null)
                {
                    if (options.dialogRetValue != CustomDialogResults.YesAll &&
                        options.dialogRetValue != CustomDialogResults.NoAll &&
                        options.dialogRetValue != CustomDialogResults.Cancel)
                    {
                        string objectName = objectOptions.IeObjectHelper.GetObjectFullName(existingObject);
                        var uiMsgBox = options?.document?.GetService(typeof(IUIMsgBoxAlertService)) as IUIMsgBoxAlertService;
                        if (uiMsgBox != null)
                            options.dialogRetValue = uiMsgBox.ShowYesNoAllCancel(string.Format(Properties.Resources.ImportTagNameExist, objectName), CustomDialogIcons.Question);
                    }

                    if (options.dialogRetValue == CustomDialogResults.Cancel)
                    {
                        (options.document as UFUAServerDocument).GetSession().DropChanges();
                        return default(T);
                    }
                    else if (options.dialogRetValue == CustomDialogResults.No || options.dialogRetValue == CustomDialogResults.NoAll)
                    {
                        if (existingObject is IXPSimpleObject)
                            changedObjects.Add(existingObject as IXPSimpleObject);
                        return existingObject;
                    }
                }
            }

            var obj = (T)CreateNewObject(objectOptions);
            if (obj != null)
            {
                if (obj is IXPSimpleObject)
                    addedObjects.Add(obj as IXPSimpleObject);
                if (objectOptions.ExistingObjects == null)
                    objectOptions.ExistingObjects = new Dictionary<string,T>();
                var key = objectOptions.IeObjectHelper.GetObjectKey(obj);
                if (!objectOptions.ExistingObjects.ContainsKey(key))
                    objectOptions.ExistingObjects.Add(key, obj);
            }
            return obj;
        }

        private void GetExistingObjectCollection(ImportExportHelper<T> ieHelper,out Dictionary<string,T> existingObjects,out List<string> listNames, object root, ImportExportOptions options, string protoPath, string tagOwner)
        {
            List<T> _existingObjects = new List<T>();
            existingObjects = new Dictionary<string, T>();
            listNames = new List<string>();
            if (typeof(T) == typeof(UFUAModel.UFUATagPrototype))
            {
                _existingObjects = options.serverDocument.GetPrototypes() as List<T>;
                listNames = options.serverDocument.GetPrototypesNames() as List<string>;
            }
            else if (typeof(T) == typeof(UFUAModel.UFUATag))
            {
                if (options.SilentMode)
                {
                    _existingObjects = options.serverDocument.GetTagMemberCollection(root as UFUAModel.UFUAFolder, protoPath, tagOwner).ToList() as List<T>;
                    listNames = (from UFUAModel.UFUATag o in _existingObjects select o.Name).ToList();
                }
                else
                {
                    _existingObjects = options.serverDocument.GetTagCollection(root as UFUAModel.UFUAFolder) as List<T>;
                    listNames = (from UFUAModel.UFUATag o in _existingObjects select o.Name).ToList();
                }
            }
            else if (options.ObjectType == typeof(UFUAModel.UFUAEngineeringUnit))
            {
                _existingObjects = options.serverDocument.GetEngineeringUnits() as List<T>;
                listNames = options.serverDocument.GetEngineeringUnitNames() as List<string>;
            }
            else if (options.ObjectType == typeof(UFUAModel.UFUAAlarmDefinition))
            {
                if(root != null)
                {
                    _existingObjects = options.serverDocument.GetAlarmDefinitions(root as UFUAModel.UFUAAlarmSource) as List<T>;
                    listNames = (from UFUAModel.UFUAAlarmDefinition o in _existingObjects select o.Name).ToList();
                }
            }
            else if (options.ObjectType == typeof(UFUAModel.UFUAHistorianSettings))
            {
                _existingObjects = options.serverDocument.GetHistoricalSettings() as List<T>;
                listNames = options.serverDocument.GetHistoricalSettingsNameList(false) as List<string>;
            }
            else if (options.ObjectType == typeof(DataLoggerModel.DataLoggerSettings))
            {
                _existingObjects = options.serverDocument.GetDataLoggerSettings() as List<T>;
                listNames = options.serverDocument.GetDataLoggerSettingsNames() as List<string>;
            }
            else if (options.ObjectType == typeof(DataLoggerModel.DataLoggerColumn))
            {
                _existingObjects = options.serverDocument.GetDataLoggerColumnSettings(root as DataLoggerModel.DataLoggerSettings) as List<T>;
                listNames = options.serverDocument.GetDataLoggerColumnNameList(root as DataLoggerModel.DataLoggerSettings) as List<string>;
            }
            foreach(T o in _existingObjects)
            {
                string key = ieHelper.GetObjectKey(o);
                if (!existingObjects.ContainsKey(key))
                    existingObjects.Add(key, o);
            };
        }

        object CreateNewObject(ItemOptions<T> objectOptions)
        {
            string fullname = string.Empty;
            string name = string.Empty;
            string tagName = string.Empty;

            if (options.ObjectType == typeof(UFUAModel.UFUATag))
            {
                UFUAModel.UFUATag newObject = options.serverDocument.AddNewTag(objectOptions.Root as UFUAModel.UFUAFolder, !string.IsNullOrEmpty(objectOptions.PrototypeName));
                //ensure Name
                name = objectOptions.IeObjectHelper.GetObjectName(objectOptions.Entry, (newObject as UFUAModel.UFUATag).Name);
                fullname = objectOptions.IeObjectHelper.GetObjectFullName(objectOptions.Entry);
                var _name = UFUAModel.Helpers.NameValidator.EnsureValidName(name);
                tagName = options.serverDocument.NewImportedTagName((objectOptions.Root as UFUAModel.UFUAFolder), _name, objectOptions.MapStartCounter, objectOptions.ListNames, "{0}_{1}");
                newObject.Name = tagName;

                options.serverDocument.EnsureValidNodeId(newObject);
                if (tagName != name)
                    logGeneral.Info(string.Format(Properties.Resources.ImportedTagNameChanged, fullname, $"{newObject.FolderPath}\\{tagName}"));
                return newObject;
            }
            else if (options.ObjectType == typeof(UFUAModel.UFUATagPrototype))
            {
                UFUAModel.UFUATagPrototype newObject = options.serverDocument.AddNewPrototype();
                //ensure Name
                name = objectOptions.IeObjectHelper.GetObjectName(objectOptions.Entry, (newObject as UFUAModel.UFUATagPrototype).Name);
                fullname = name;
                var _name = UFUAModel.Helpers.NameValidator.EnsureValidName(name);
                tagName = options.serverDocument.NewImportedPrototypeName(_name, objectOptions.MapStartCounter, objectOptions.ListNames, "{0}_{1}");
                newObject.Name = tagName;
                options.serverDocument.EnsureValidNodeId(newObject);
                renamedProtoMap.Add(name, tagName);
                if (tagName != name)
                    logGeneral.Info(string.Format(Properties.Resources.ImportedPrototypeNameChanged, fullname, tagName));
                return newObject;
            }
            else if (options.ObjectType == typeof(UFUAModel.UFUAEngineeringUnit))
            {
                UFUAModel.UFUAEngineeringUnit newObject = options.serverDocument.AddNewEngineeringUnits();
                //ensure Name
                name = objectOptions.IeObjectHelper.GetObjectName(objectOptions.Entry, newObject.Name);
                var _name = UFUAModel.Helpers.NameValidator.EnsureValidName(name);
                tagName = options.serverDocument.NewEngineeringUnitsName(_name, objectOptions.MapStartCounter, objectOptions.ListNames);
                newObject.Name = tagName;
                if (tagName != name)
                    logGeneral.Info(string.Format(Properties.Resources.ImportedEUnitNameChanged, name, tagName));
                return newObject;
            }
            else if (options.ObjectType == typeof(UFUAModel.UFUAAlarmDefinition))
            {
                UFUAModel.UFUAAlarmDefinition newObject = options.serverDocument.AddNewAlarmPrototype(objectOptions.Root as UFUAModel.UFUAAlarmSource);
                //ensure Name
                name = objectOptions.IeObjectHelper.GetObjectName(objectOptions.Entry, (newObject as UFUAModel.UFUAAlarmDefinition).Name);
                fullname = objectOptions.IeObjectHelper.GetObjectFullName(objectOptions.Entry);
                var _name = UFUAModel.Helpers.NameValidator.EnsureValidName(name);
                tagName = options.serverDocument.NewAlarmPrototypeName((objectOptions.Root as UFUAModel.UFUAAlarmSource), _name, objectOptions.MapStartCounter, objectOptions.ListNames);
                newObject.Name = tagName;
                if (tagName != name)
                    logGeneral.Info(string.Format(Properties.Resources.ImportedAlarmDefinitionNameChanged, fullname, $"{newObject.FolderPath}\\{tagName}"));
                return newObject;
            }
            else if (options.ObjectType == typeof(UFUAModel.UFUAHistorianSettings))
            {
                UFUAModel.UFUAHistorianSettings newObject = options.serverDocument.AddNewHistoricalSettings();
                //ensure Name
                name = objectOptions.IeObjectHelper.GetObjectName(objectOptions.Entry, (newObject as UFUAModel.UFUAHistorianSettings).Name);
                fullname = objectOptions.IeObjectHelper.GetObjectFullName(objectOptions.Entry);
                var _name = UFUAModel.Helpers.NameValidator.EnsureValidName(name);
                tagName = options.serverDocument.NewHistoricalSettingsName(_name, objectOptions.MapStartCounter, objectOptions.ListNames);
                newObject.Name = tagName;
                if (tagName != name)
                    logGeneral.Info(string.Format(Properties.Resources.ImportedHistorianSettingNameChanged, fullname, $"{tagName}"));
                return newObject;
            }
            else if (options.ObjectType == typeof(DataLoggerModel.DataLoggerSettings))
            {
                DataLoggerModel.DataLoggerSettings newObject = options.serverDocument.AddNewDataLoggerSettings();
                //ensure Name
                name = objectOptions.IeObjectHelper.GetObjectName(objectOptions.Entry, (newObject as DataLoggerModel.DataLoggerSettings).Name);
                fullname = objectOptions.IeObjectHelper.GetObjectFullName(objectOptions.Entry);
                var _name = DataLoggerModel.Helpers.DBNameValidator.EnsureValidName(name);
                tagName = options.serverDocument.NewDataLoggerSettingsName(_name, objectOptions.MapStartCounter, objectOptions.ListNames);
                newObject.Name = tagName;
                renamedProtoMap.Add(name, tagName);
                if (tagName != name)
                    logGeneral.Info(string.Format(Properties.Resources.ImportedDataloggerSettingNameChanged, fullname, $"{tagName}"));
                return newObject;
            }
            else if (options.ObjectType == typeof(DataLoggerModel.DataLoggerColumn))
            {
                DataLoggerModel.DataLoggerColumn newObject = options.serverDocument.AddNewDataLoggerColumn(objectOptions.Root as DataLoggerModel.DataLoggerSettings);
                //ensure Name
                name = objectOptions.IeObjectHelper.GetObjectName(objectOptions.Entry, (newObject as DataLoggerModel.DataLoggerColumn).Name);
                fullname = objectOptions.IeObjectHelper.GetObjectFullName(objectOptions.Entry);
                var _name = DataLoggerModel.Helpers.DBNameValidator.EnsureValidName(name);
                tagName = options.serverDocument.NewDataLoggerColumnName(objectOptions.Root as DataLoggerModel.DataLoggerSettings, _name, objectOptions.MapStartCounter, objectOptions.ListNames);
                newObject.ColumnName = tagName;
                if (tagName != name)
                    logGeneral.Info(string.Format(Properties.Resources.ImportedDataloggerColumnNameChanged, fullname, $"{tagName}"));
                return newObject;
            }



            return default(T);
        }


        #endregion
    }

    public static class Extensions
    {
        public static T[] SubArray<T>(this T[] array, int offset, int length)
        {
            T[] result = new T[length];
            Array.Copy(array, offset, result, 0, length);
            return result;
        }
    }
}
