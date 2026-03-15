using Opc.Ua;
using Opc.Ua.Helpers;
using OPCUAViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
#if !NET_STANDARD
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using UIMsgBoxAlertService.ComponentService;
#endif
using UFInterfaces;
using DocumentManager.ComponentService;
using TempVariablesManager.Document;
using Utilities;
using log4net;
using System.Globalization;
using DevExpress.Xpo;
using System.ComponentModel;
using TempVariablesModel;
using DocumentManager.ComponentService.Helpers;
using UFProjectManager.ComponentService;


namespace TempVariablesManager
{
    public class TempVariables : DataSinkInterface, IDisposable, IUFInterfaceBase
    {
#region Declarations
        IUFProjectManager iUFProjectManager;
        IDocument rootParent;
        Dictionary<String, MonitoredItemViewModel> mapTempVariables = new Dictionary<String, MonitoredItemViewModel>();
        Dictionary<String, Variable> mapTempLocalVariables = new Dictionary<String, Variable>();
        Dictionary<IDocument, TempVariablesPersistence> mapActiveDocuments = new Dictionary<IDocument, TempVariablesPersistence>();
        Dictionary<IDocument, TempVariables> mapRunningVariables = new Dictionary<IDocument, TempVariables>();
        List<String> errorLoadingDocumentUris = new List<String>();

        Object lockObject = new Object();
        TempVariablesPersistence currentDocument;

        bool bNeedToReload;
        bool isLoaded;
#if !NET_STANDARD
        internal static readonly ILog log = LogManager.GetLogger(Properties.Resources.TempVarLog);
#else
        internal static readonly ILog log = LogManager.GetLogger(System.Reflection.Assembly.GetEntryAssembly(), Properties.Resources.TempVarLog);
#endif

        static readonly public String dataSynkName = "TemporaryVariables";
#endregion
        
#region Ctor
        public TempVariables()
        { }
#endregion

#region Singleton
        static Object singletonLocker = new Object();
        static TempVariables singletonInstance;
        static public TempVariables GetTempVariables()
        {
            lock (singletonLocker)
            {
                if (singletonInstance != null)
                    return singletonInstance;
                singletonInstance = new TempVariables();
                OPCUAEntityReference.RegisterDataSinkInterface(dataSynkName, singletonInstance);
                return singletonInstance;
            }
        }
#endregion

#region Methods
#if !NET_STANDARD
        public static BitmapImage GetBitmapImage(String image, bool bShared = false)
        {
            var bm = SharedResources.Helpers.ResourceManager.GetCommonImage("TempVariables", image, bShared);
            return bm;
        }
        
        public BitmapImage GetTagBitmapImage(object dataType)
        {
            if(dataType is UFUAModel.DataType)
            {
                BitmapImage bmpImage = null;
                switch ((UFUAModel.DataType)dataType)
                {
                    case UFUAModel.DataType.Boolean:
                        bmpImage = TempVariables.GetBitmapImage("TVEditorVariableBooleanSmall");
                        break;
                    case UFUAModel.DataType.SByte:
                        bmpImage = TempVariables.GetBitmapImage("TVEditorVariableSByteSmall");
                        break;
                    case UFUAModel.DataType.Byte:
                        bmpImage = TempVariables.GetBitmapImage("TVEditorVariableByteSmall");
                        break;
                    case UFUAModel.DataType.Int16:
                        bmpImage = TempVariables.GetBitmapImage("TVEditorVariableInt16Small");
                        break;
                    case UFUAModel.DataType.UInt16:
                        bmpImage = TempVariables.GetBitmapImage("TVEditorVariableUInt16Small");
                        break;
                    case UFUAModel.DataType.Int32:
                        bmpImage = TempVariables.GetBitmapImage("TVEditorVariableInt32Small");
                        break;
                    case UFUAModel.DataType.UInt32:
                        bmpImage = TempVariables.GetBitmapImage("TVEditorVariableUInt32Small");
                        break;
                    case UFUAModel.DataType.Int64:
                        bmpImage = TempVariables.GetBitmapImage("TVEditorVariableInt64Small");
                        break;
                    case UFUAModel.DataType.UInt64:
                        bmpImage = TempVariables.GetBitmapImage("TVEditorVariableUInt64Small");
                        break;
                    case UFUAModel.DataType.Float:
                        bmpImage = TempVariables.GetBitmapImage("TVEditorVariableFloatSmall");
                        break;
                    case UFUAModel.DataType.Double:
                        bmpImage = TempVariables.GetBitmapImage("TVEditorVariableDoubleSmall");
                        break;
                    case UFUAModel.DataType.String:
                        bmpImage = TempVariables.GetBitmapImage("TVEditorVariableStringSmall");
                        break;
                    default:
                        bmpImage = TempVariables.GetBitmapImage("TVEditorSmall");
                        break;
                }
                return bmpImage;
            }
            return null;
        }
#endif

        Type GetVarType(UFUAModel.DataType type)
        {
            switch (type)
            {
                case UFUAModel.DataType.Boolean:
                    return typeof(bool);
                case UFUAModel.DataType.SByte:
                    return typeof(SByte);
                case UFUAModel.DataType.Byte:
                    return typeof(Byte);
                case UFUAModel.DataType.Int16:
                    return typeof(Int16);
                case UFUAModel.DataType.Int32:
                    return typeof(Int32);
                case UFUAModel.DataType.Int64:
                    return typeof(Int64);
                case UFUAModel.DataType.UInt16:
                    return typeof(UInt16);
                case UFUAModel.DataType.UInt32:
                    return typeof(UInt32);
                case UFUAModel.DataType.UInt64:
                    return typeof(UInt64);
                case UFUAModel.DataType.Float:
                    return typeof(float);
                case UFUAModel.DataType.Double:
                    return typeof(double);
                case UFUAModel.DataType.String:
                    return typeof(string);
                default:
                    return typeof(string);
            }

        }
        static Array GetArray(Type type, object value, uint arraydimension)
        {
            Array retarray = Array.CreateInstance(type, arraydimension);
            if (value != null && value is Array)
            {
                for (int i = 0; i <= (value as Array).GetUpperBound(0); i++)
                {
                    if (i < arraydimension)
                        retarray.SetValue(Convert.ChangeType((value as Array).GetValue(i), type, CultureInfo.InvariantCulture), i);
                }

            }
            else
                for (int i = 0; i < arraydimension; i++)
                {
                    retarray.SetValue(Convert.ChangeType(value, type, CultureInfo.InvariantCulture), i);
                }

            return retarray;
        }

        static Array GetArray(Type type, object[] value, uint arraydimension)
        {
            var retarray = Array.CreateInstance(type,arraydimension);

            if(value!= null)
            {
                for (int i = 0; i < value.Count(); i++)
                {
                    if (i < arraydimension)
                        retarray.SetValue(Convert.ChangeType(value[i], type, CultureInfo.InvariantCulture), i);
                }
            }

            return retarray;
        }

        void UpdateDataType(string relativepath)
        {
            if (mapTempVariables.ContainsKey(relativepath) && mapTempLocalVariables.ContainsKey(relativepath))
            {
                try
                {
                    uint arraydimension = (uint)mapTempLocalVariables[relativepath].ArrayDimension;
                    var _value = mapTempVariables[relativepath].DataValue.Value;
                    Type _type = GetVarType(mapTempLocalVariables[relativepath].DataType);
                    string _initialvalue = mapTempLocalVariables[relativepath].InitialValue;

                    DataValue _dataValue = mapTempVariables[relativepath].DataValue;
                    BuiltInType builtinType = MonitoredItemViewModel.GetBuiltInType(_type.Name);

                    if (_value == null)
                    {
                        if (!string.IsNullOrEmpty(_initialvalue))
                        {
                            try
                            {
                                _value = ChangeTypeHelper.ChangeType(_initialvalue, builtinType, arraydimension);
                            }
                            catch (Exception)
                            {
                                if (arraydimension > 0)
                                {
                                    if (_type == typeof(string))
                                        _value = GetArray(_type, string.Empty, arraydimension);
                                    else
                                        _value = GetArray(_type, 0, arraydimension);
                                }
                                else
                                {
                                    if (_type == typeof(string))
                                        _value = string.Empty;
                                    else
                                        _value = Convert.ChangeType(0, _type, CultureInfo.InvariantCulture);
                                }
                            }
                        }
                        else
                        {
                            if (arraydimension > 0)
                            {
                                if (_type == typeof(string))
                                    _value = GetArray(_type, string.Empty, arraydimension);
                                else
                                    _value = GetArray(_type, 0, arraydimension);
                            }
                            else
                            {
                                if (_type == typeof(string))
                                    _value = string.Empty;
                                else
                                    _value = 0;
                            }
                        }
                            
                    }
                    try
                    {
                        _value = ChangeTypeHelper.ChangeType(_value, builtinType, arraydimension); // prechange type base on currenthread localization first
                        if (arraydimension == 0)
                            _dataValue = new DataValue(new Variant(Opc.Ua.TypeInfo.Cast(_value, builtinType)), StatusCodes.Good, DateTime.UtcNow);
                        else if (_value is Array)
                        {
                            _dataValue = new DataValue(new Variant(Opc.Ua.TypeInfo.CastArray((_value as Array), builtinType, builtinType, ChangeTypeHelper.CastArrayElement)), StatusCodes.Good, DateTime.UtcNow);
                        }
                    }
                    catch (Exception ex)
                    {
#if !NET_STANDARD
                        log.Error(string.Format(Properties.Resources.DifferentDataTypeError, relativepath), null);
                        if (iUFProjectManager != null)
                            iUFProjectManager.AddLogEntity(rootParent, Properties.Resources.TempVarLog,
                              DateTime.UtcNow, string.Format(Properties.Resources.DifferentDataTypeError, relativepath),
                              System.Diagnostics.EventLogEntryType.Error);
#endif
                    }

                    UpdateTempVariable(relativepath, _dataValue);
                }
                catch (Exception)
                {

#if !NET_STANDARD
                    log.Error(string.Format(Properties.Resources.DifferentDataTypeError, relativepath), null);
                    if (iUFProjectManager != null)
                        iUFProjectManager.AddLogEntity(rootParent, Properties.Resources.TempVarLog,
                          DateTime.UtcNow, string.Format(Properties.Resources.DifferentDataTypeError, relativepath),
                          System.Diagnostics.EventLogEntryType.Error);
#endif
                }
            }
        }

        public bool AddTempVariable(Variable x)
        {
            lock (lockObject)
            {
                Opc.Ua.NamespaceTable n = new Opc.Ua.NamespaceTable();
                UInt16 ns = (UInt16)(n.Count + 2 - 1);
                string relativepath = x.GetRelative(); // x.GetRelativeName();
                if (!mapTempVariables.ContainsKey(relativepath))
                    mapTempVariables.Add(relativepath, new MonitoredItemViewModel());
                mapTempLocalVariables[relativepath] = x;

                try
                {
                    UpdateDataType(relativepath);
                }
                catch (Exception ex)
                {
#if !NET_STANDARD
                    log.Error(string.Format(Properties.Resources.DifferentDataTypeError, x.Name), ex);
                    if (iUFProjectManager != null)
                        iUFProjectManager.AddLogEntity(rootParent, Properties.Resources.TempVarLog,
                          DateTime.UtcNow, $"{string.Format(Properties.Resources.DifferentDataTypeError, x.Name)}: {ex.Message}",
                          System.Diagnostics.EventLogEntryType.Error);
#endif
                }

                return true;
            }
        }

        void RemoveTempVariable(Variable x)
        {
            lock (lockObject)
            {
                string varname = x.GetRelativeName();
                if (mapTempVariables.ContainsKey(varname))
                    mapTempVariables.Remove(varname);
                if (mapTempLocalVariables.ContainsKey(varname))
                    mapTempLocalVariables.Remove(varname);
                x.Delete();
            }
        }

        void UpdateTempVariable(String name, DataValue datavalue)
        {
            MonitoredItemViewModel model = null;
            lock (lockObject)
            {
                if (mapTempVariables.ContainsKey(name))
                    model = mapTempVariables[name];
            }

            if (model != null)
                model.DataValue = datavalue;
        }

#if !NET_STANDARD
        public void Copy(Uri uri, string newPath, bool bCopy, IDocument parent)
        {
            using (new WaitCursor())
            {
                lock (lockObject)
                {
                    TempVariablesPersistence.CopyFile(uri.GetPathString(), newPath, bCopy, parent, null);
                }
            }
        }

        public bool Save(IDocument parent, bool encryptFile = false)
        {
            var doc = GetOrCreateDocument(parent);
            if (doc == null)
                return false;
            return doc.SaveToFile(bForceSave: true, forceEncryption: encryptFile);
        }

        public Folder FindFolderByNodeId(IDocument parent, Guid guid)
        {
            var doc = GetOrCreateDocument(parent);
            if (doc == null)
                return null;
            return doc.FindFolderByNodeId(guid);
        }

        public IList<Variable> GetTagCollection(IDocument parent, Folder root = null)
        {
            var doc = GetOrCreateDocument(parent);
            if (doc == null)
                return null;
            return doc.GetTagCollection(root);
        }

        public IList<Folder> GetFolderCollection(IDocument parent, Folder root = null)
        {
            var doc = GetOrCreateDocument(parent);
            if (doc == null)
                return null;
            return doc.GetFolderCollection(root);
        }
#endif

        TempVariablesPersistence GetOrCreateDocument(IDocument parent, bool bRefresh = false)
        {
            if (parent == null)
                return null;

            lock (lockObject)
            {
                rootParent = DocumentHelper.GetRootParent(parent, traverse: false);
                var uri = rootParent.rootBase;
                Dictionary<IDocument, TempVariablesPersistence> map = mapActiveDocuments;
                var list = (from c in map/*.AsParallel()*/
                            where c.Key == rootParent//  && c.Value.Parent == p && c.Value.ActiveView == null
                            select c.Value).ToList();
                TempVariablesPersistence doc = null;
                iUFProjectManager = parent.GetService(typeof(IUFProjectManager)) as IUFProjectManager;
                if (list.Count == 0 || bRefresh)
                {
                    try
                    {
                        doc = TempVariablesPersistence.FromFile(uri, parent, bThrowExceptions: true);
                        errorLoadingDocumentUris.Remove(uri);
                    }
                    catch (Exception ex)
                    {
                        if (!errorLoadingDocumentUris.Contains(uri))
                        {
                            errorLoadingDocumentUris.Add(uri);
#if !NET_STANDARD
                            var uiMsgBox = parent.GetService(typeof(IUIMsgBoxAlertService)) as IUIMsgBoxAlertService;
                            if (uiMsgBox != null)
                                uiMsgBox.ShowError(ex.Message);
                            log.ErrorFormat(ex.Message);
                            if (iUFProjectManager != null)
                                iUFProjectManager.AddLogEntity(rootParent, Properties.Resources.TempVarLog,
                                  DateTime.UtcNow, ex.Message,
                                  System.Diagnostics.EventLogEntryType.Error);
#endif
                        }
                    }
                    if (doc == null)
                    {
                        return null;
                    }
                    doc.Parent = rootParent;
                    if (map.ContainsKey(rootParent))
                    {
                        map[rootParent].Dispose();
                        map.Remove(rootParent);
                    }
                    map.Add(rootParent, doc);
                }
                else
                    doc = list[0]; 
                return doc;
            }
        }

        public TempVariablesPersistence GetDocument(IDocument parent)
        {
            var doc = GetOrCreateDocument(parent);
            return doc;
        }

#if !NET_STANDARD
        void CurrentDocument_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var doc = (TempVariablesPersistence)sender;
            if (e.PropertyName == "NeedsSave")
            {
                if (doc.NeedsSave)
                    bNeedToReload = true;
            }
            if (e.PropertyName == "NeedToReloadAddressSpace")
            {
                if (doc.NeedToReloadAddressSpace)
                    bNeedToReload = true;
            }
        }
#endif
#endregion

#region DataSinkInterface
        public string DataSynkName
        {
            get
            {
                return dataSynkName;
            }
        }

        public string HumanReadableName
        {
            get
            {
                return Properties.Resources.AddressSpaceHeader;
            }
        }

        public string TypeScheme
        {
            get
            {
                return Properties.Settings.Default.TypeScheme;
            }
        }

        public bool IsProjectTypeAware(String projectType)
        {
            return true;
        }

        public void Start()
        {
            lock (lockObject)
            {
                if (currentDocument == null)
                    return;

                var rootParent = DocumentHelper.GetRootParent(currentDocument, traverse: false);
                if (mapRunningVariables.ContainsKey(rootParent))
                {
#if !NET_STANDARD
                    if (UIInterface != null)
                        UIInterface.ShowWarning(Properties.Resources.CannotBeStartedTwice);
                    log.Error(Properties.Resources.CannotBeStartedTwice);
                    if (iUFProjectManager != null)
                        iUFProjectManager.AddLogEntity(rootParent, Properties.Resources.TempVarLog,
                          DateTime.UtcNow, Properties.Resources.CannotBeStartedTwice,
                          System.Diagnostics.EventLogEntryType.Error);
#endif
                    return;
                }

                mapRunningVariables.Add(rootParent, new TempVariables());
                mapRunningVariables[rootParent].SetDocumentParent(rootParent);
                mapRunningVariables[rootParent].UnloadMap();
                mapRunningVariables[rootParent].LoadMap();
            }
        }

        public void Stop()
        {
            lock (lockObject)
            {
                mapRunningVariables.Values.ToList().ForEach((data) => data.Dispose());
                mapRunningVariables.Clear();
            }
        }

        public void UpdateVariable(String name, DataValue value)
        {
            UpdateTempVariable(name, value);
        }

        public MonitoredItemViewModel GetVariable(String name, IDocument parent)
        {
            lock (lockObject)
            {
                if (parent == null)
                    return null;

                TempVariables currentIntance = singletonInstance;
                var rootParent = DocumentHelper.GetRootParent(parent, traverse: false);
                if (mapRunningVariables.ContainsKey(rootParent))
                    currentIntance = mapRunningVariables[rootParent];
                else
                {
                    currentIntance.SetDocumentParent(parent);
                    if (!currentIntance.isLoaded || currentIntance.bNeedToReload)
                    {
                        currentIntance.UnloadMap();
                        currentIntance.LoadMap();
                    }
                }

                try
                {
                    currentIntance.UpdateDataType(name);
                }
                catch (Exception ex)
                {
#if !NET_STANDARD
                    log.Error(string.Format(Properties.Resources.DifferentDataTypeError, name), ex);
                    if (iUFProjectManager != null)
                        iUFProjectManager.AddLogEntity(rootParent, Properties.Resources.TempVarLog,
                          DateTime.UtcNow, $"{string.Format(Properties.Resources.DifferentDataTypeError, name)}: {ex.Message}",
                          System.Diagnostics.EventLogEntryType.Error);
#endif
                }

                if (currentIntance.mapTempVariables.ContainsKey(name))
                    return currentIntance.mapTempVariables[name];
                return null;
            }
        }

        public List<MonitoredItemViewModel> GetRunningVariables()
        {
            lock (lockObject)
            {
                var ret = new List<MonitoredItemViewModel>();
                mapRunningVariables.Values.ToList().ForEach((data) => ret.AddRange(data.mapTempVariables.Values.ToList()));
                return ret;
            }
        }

        public List<String> GetVariables(IDocument parent = null)
        {
            if (parent != null)
            {
                var rootParent = DocumentHelper.GetRootParent(parent, traverse: false);
                using (var tempVariables = new TempVariables())
                {
                    tempVariables.SetDocumentParent(parent);
                    tempVariables.LoadMap();
                    return tempVariables.mapTempVariables.Keys.ToList();
                }
            }
            else
            {
                lock (lockObject)
                {
                    {
                        if (bNeedToReload)
                        {
                            bNeedToReload = false;

                            UnloadMap();
                            LoadMap();
                        }

                        return mapTempVariables.Keys.ToList();
                    }
                }
            }
        }

        public OPCUAEntityReference GetReference(String name)
        {
            if (mapTempLocalVariables.ContainsKey(name))
                return GetReference(mapTempLocalVariables[name]);
            return new OPCUAEntityReference(null, DataSynkName, null, name, null, name, null);
        }

        public OPCUAEntityReference GetReference(Variable tagFound)
        {
            Opc.Ua.NamespaceTable n = new Opc.Ua.NamespaceTable();
            UInt16 ns = (UInt16)(n.Count + 2 - 1);
            string relativepath = tagFound.GetRelativePath(ns);
            string relativename = tagFound.GetRelative();
            //OPCUAEntityReference entityreference = new OPCUAEntityReference(null, DataSynkName, null,
            //                    relativepath, null, string.Format("{0} ({1})", tagFound.Name, DataSynkName), null);
            return new OPCUAEntityReference(null, DataSynkName, null, relativename, null, string.Format("{0} ({1})", tagFound.Name, DataSynkName), null, relativename);
        }
#if !WINDOWS_UWP && !NET_STANDARD
        public bool RemoveVariable(String name)
        {
            if (bNeedToReload)
            {
                bNeedToReload = false;

                UnloadMap();
                LoadMap();
            }
            if (mapTempLocalVariables.ContainsKey(name))
            {
                mapTempLocalVariables[name].Delete();
                mapTempLocalVariables.Remove(name);
            }
            if (mapTempVariables.ContainsKey(name))
                mapTempVariables.Remove(name);
            return true;
        }
#endif
#if !NET_STANDARD
        public UserControl Editor(bool bPopup = true)
        {
            if (currentDocument != null)
                return new Controls.ControlEditor(this, bPopup);
            return null;
        }
#endif

        public void SetDocumentParent(DocumentManager.ComponentService.IDocument parent)
        {
            var doc = GetOrCreateDocument(parent);
            if (doc == null)
                return;

            if (currentDocument != doc)
            {
#if !NET_STANDARD
                if (currentDocument != null)
                {
                    //currentDocument.SaveToFile();
                    currentDocument.PropertyChanged -= CurrentDocument_PropertyChanged;
                }
#endif

                currentDocument = doc;
#if !NET_STANDARD
                if (currentDocument != null)
                {
                    currentDocument.PropertyChanged += CurrentDocument_PropertyChanged;
                    bNeedToReload = true;
                }
#endif
            }
        }

#if !NET_STANDARD
        public bool NeedsSave(IDocument parent)
        {
            var doc = GetOrCreateDocument(parent);
            if (doc == null)
                return false;
            return doc.NeedsSave;
        }

        public bool CheckSource(IDocument parent, object source)
        {
            var doc = GetOrCreateDocument(parent);
            if (doc == null)
                return false;
            try
            {
                return (source as IXPSimpleObject).Session == doc.UowContext;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool CheckVariable(string name)
        {
            if (!isLoaded)
                LoadMap();
            lock (lockObject)
                return !mapTempVariables.ContainsKey(name);
        }
#endif

        public void DisposingDocumentParent(DocumentManager.ComponentService.IDocument parent)
        {
            lock (lockObject)
            {
                var rootParent = DocumentHelper.GetRootParent(parent, traverse: false);
                var uri = rootParent.rootBase;
                if (mapRunningVariables.ContainsKey(rootParent))
                {
                    mapRunningVariables[rootParent].Dispose();
                    mapRunningVariables.Remove(rootParent);
                }
                if (mapActiveDocuments.ContainsKey(rootParent))
                {
                    if (mapActiveDocuments[rootParent] == currentDocument)
                    {
                        //currentDocument.SaveToFile();
#if !NET_STANDARD
                        currentDocument.PropertyChanged -= CurrentDocument_PropertyChanged;
#endif
                        currentDocument.Dispose();
                        currentDocument = null;
                    }
                    mapActiveDocuments.Remove(rootParent);
                }
	            if (errorLoadingDocumentUris.Contains(uri))
	                errorLoadingDocumentUris.Remove(uri);
            }
        }

        void LoadMap()
        {
            if (isLoaded || currentDocument == null)
                return;
            isLoaded = true;

            currentDocument.GetFlatTagCollection().ToList().ForEach(x =>
            {
                AddTempVariable(x); //.GetRelativeName());
            });
        }

        void UnloadMap()
        {
            lock (lockObject)
            {
                isLoaded = false;
                mapTempVariables.Clear();
                mapTempLocalVariables.Clear();
            }
        }

#endregion

#region Properties
        public TempVariablesPersistence CurrentDocument
        {
            get
            {
                return currentDocument;
            }
        }

#if !NET_STANDARD
        IUIMsgBoxAlertService uiInterface;
        public IUIMsgBoxAlertService UIInterface
        {
            get
            {
                if (uiInterface == null && currentDocument != null)
                    uiInterface = currentDocument.GetService(typeof(IUIMsgBoxAlertService)) as IUIMsgBoxAlertService;
                return uiInterface;
            }
        }
#endif
#endregion

#region IDisposable
        bool bDisposed;
        public void Dispose()
        {
            if (bDisposed)
                return;
            bDisposed = true;

            Stop();
            UnloadMap();

            if (currentDocument != null)
            {
#if !NET_STANDARD
                currentDocument.PropertyChanged -= CurrentDocument_PropertyChanged;
#endif
                currentDocument.Dispose();
            }
        }
#endregion

#region IUFInterfaceBase
        public void Initialize()
        {
            GetTempVariables();
        }
        #endregion
    }
}
