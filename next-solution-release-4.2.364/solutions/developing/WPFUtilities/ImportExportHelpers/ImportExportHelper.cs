using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using log4net;
using UIMsgBoxAlertService.ComponentService;
using UFUAEditor.ComponentService;
using System.IO;
using DevExpress.Xpo;
using DevExpress.Xpo.Metadata;
using System.Threading;
using DocumentManager.ComponentService;
using System.Text.RegularExpressions;
using UFInterfaces;

namespace WPFUtilities.ImportExportHelpers
{
    public enum ImportType
    {
        Base,
        Medium,
        Advanced
    }

    public enum ActionType
    {
        Export,
        Import
    }

    public enum SepType
    {
        SemiColon,
        Colon,
        Tab
    }

    public enum ResultType
    {
        Successfully,
        Aborted,
        Failed
    }

    public class ImportExportResult
    {
        public ResultType Result;
        public bool WasError;
        public bool WasImporting;
        public List<IXPSimpleObject> RemovedObjects;
        public List<IXPSimpleObject> AddedObjects;
        public List<IXPSimpleObject> ChangedObjects;

        public ImportExportResult(ResultType resultType)
        {
            switch (resultType)
            {
                case ResultType.Successfully:
                    Result = ResultType.Successfully;
                    WasError = false;
                    break;
                case ResultType.Aborted:
                    Result = ResultType.Aborted;
                    WasError = false;
                    break;
                case ResultType.Failed:
                    Result = ResultType.Failed;
                    break;
                default:
                    break;
            }
        }
    }

    public class ImportExportBaseOptions
    {
        public ActionType EnumAction { get; set; }
        public bool SilentMode { get; set; }
        public string Separator { get; set; }
        public string FilePath { get; set; }
        public bool MatchCase { get; set; }
        public string Filter { get; set; }
        public Type ParentObjectType { get; set; }
        public Type ObjectType { get; set; }
        public readonly IDocument document;
        public ImportExportBaseOptions(IDocument document)
        {
            this.document = document;
        }
        public ImportExportBaseOptions(ImportExportBaseOptions options)
        {
            this.EnumAction = options.EnumAction;
            this.SilentMode = options.SilentMode;
            this.Separator = options.Separator;
            this.FilePath = options.FilePath;
            this.MatchCase = options.MatchCase;
            this.Filter = options.Filter;
            this.document = options.document;
            this.ObjectType  = options.ObjectType;
            this.ParentObjectType = options.ParentObjectType;
        }
    }

    public class ImportExportHelper<T>
    {
        #region declarations
        string importTagProtoNameInfo;
        static readonly ILog log = LogManager.GetLogger(Properties.Resources.ImportEditorTitle);
        readonly ImportExportBaseOptions options;
        readonly IUIMsgBoxAlertService uIMsgBoxAlertService;
        PropertyInfo[] writableProperties;
        PropertyInfo[] properties;
        PropertyInfo extRefKey;
        PropertyInfo extRefValue;
        Dictionary<string, string> propToExportNameMap;
        List<String> propNameList;
        List<string> keyList;
        List<string> aggregatedProperties;
        List<string> externalPropertyReferences;
        bool bAssociate;
        bool bUsePrototype;
        bool bUseSubPrototype;
        bool bUpdateAggregatedProp;
        bool bUpdateExternalRef;
        string assName;
        string externalRefInfo;
        string externalRefValue;
        string assProtoName;
        string assSubProtTagOwnerPath;
        string innerChar;
        string innerNLine;
        string innerKeySeparator;
        IDocument document;
        IWorkspace workspace;

        public static readonly Dictionary<SepType, char> Separators = new Dictionary<SepType, char>()
        {
            {SepType.SemiColon, ';'},
            {SepType.Colon, ','},
            {SepType.Tab, '\t'}
        };

        #endregion

        #region ctor
        public ImportExportHelper(ImportExportBaseOptions options)
        {
            this.options = new ImportExportBaseOptions(options);

            this.uIMsgBoxAlertService = this.options.document.GetService(typeof(IUIMsgBoxAlertService)) as IUIMsgBoxAlertService;

            InitProMaps();

            Utilities.Exportable attribute = (Utilities.Exportable)options.ObjectType.GetCustomAttributes(typeof(Utilities.Exportable), true).FirstOrDefault() as Utilities.Exportable;
            keyList = attribute?.RequiredKeys.ToList();
            aggregatedProperties = attribute?.AggregatedProperties?.ToList();
            externalPropertyReferences = attribute?.UseExternalRef?.ToList();
            assName = attribute?.ImportFolderInfo;
            externalRefInfo = attribute?.ExternalRefInfo;
            externalRefValue = attribute?.ExternalRefValue;
            assProtoName = attribute?.ImportFolderPrototype;
            assSubProtTagOwnerPath = attribute?.ImportTagOwnerInfo;
            importTagProtoNameInfo = attribute?.ImportTagProtoNameInfo;
            bAssociate = !string.IsNullOrEmpty(assName);
            bUsePrototype = !string.IsNullOrEmpty(assProtoName);
            bUseSubPrototype = !string.IsNullOrEmpty(assSubProtTagOwnerPath);
            bUpdateAggregatedProp = aggregatedProperties != null && aggregatedProperties.Count > 0;
            bUpdateExternalRef = externalPropertyReferences != null && externalPropertyReferences.Count > 0;
            if (!string.IsNullOrEmpty(externalRefInfo))
                extRefKey = (from p in properties where p.Name == externalRefInfo select p).FirstOrDefault();
            if (!string.IsNullOrEmpty(externalRefValue))
                extRefValue = (from p in properties where p.Name == externalRefValue select p).FirstOrDefault();

            innerChar = $"&#{Encoding.ASCII.GetBytes(options.Separator).FirstOrDefault()}";
            innerNLine = $"&#{Encoding.ASCII.GetBytes(Environment.NewLine).FirstOrDefault()}";
            innerKeySeparator = $"|";
            document = options.document;
            workspace = document.GetService(typeof(IWorkspace)) as IWorkspace;
        }

        private void InitProMaps()
        {
            properties = GetProperties();
            if(propToExportNameMap == null)
                propToExportNameMap = new Dictionary<string, string>();
            properties.ToList().ForEach(p =>
            {
            var propname = p.Name;
            if (Attribute.IsDefined(p, typeof(Utilities.Exportable)))
                propname = (p.GetCustomAttributes(typeof(Utilities.Exportable), true).FirstOrDefault() as Utilities.Exportable).ExportPropertyName ?? propname;
                propToExportNameMap.Add(p.Name, propname);
            });
            propNameList = propToExportNameMap.Values.ToList();
            writableProperties = (from p in properties where p.CanWrite && !Attribute.IsDefined(p, typeof(System.ComponentModel.ReadOnlyAttribute)) select p).ToArray();
        }
        #endregion

        #region static members
        static object GetSafeValue(string v, Type t, PropertyInfo p)
        {
            if (t == typeof(TimeSpan))
            {
                TimeSpan ret;
                if (TimeSpan.TryParse(v, out ret))
                    return ret;
                else
                    return null;
            }
            else if (t.IsEnum)
            {
                if (Enum.IsDefined(t, v))
                    return Enum.Parse(t, v);
                else
                {
                    if (string.IsNullOrEmpty(v))
                    {
                        return null;
                    }
                    else
                    {
                        return Activator.CreateInstance(t);
                    }
                }
            }
            else
                return v == null ? null : Convert.ChangeType(v, t);

        }
        #endregion

        #region members
        public bool ExportToCsv(bool bOverride)
        {
            var file = options.FilePath.ToLower();
            if (string.IsNullOrEmpty(file))
                return false;
            bool oldBusyState = false;
            if (System.IO.File.Exists(file) && !bOverride && uIMsgBoxAlertService != null)
            {
                if (workspace != null)
                {
                    oldBusyState = workspace.IsBusy;
                    workspace.IsBusy = false;
                }

                var res = uIMsgBoxAlertService.ShowYesNoCancel(Properties.Resources.ExportFileWarning, CustomDialogIcons.Question);
                if (res == CustomDialogResults.Cancel)
                    return false;
                else if (res == CustomDialogResults.No)
                {
                    int i = 1;
                    while (System.IO.File.Exists(System.IO.Path.Combine(System.IO.Path.GetDirectoryName(file), string.Format("{0}{1}{2}", System.IO.Path.GetFileNameWithoutExtension(file), i.ToString(), System.IO.Path.GetExtension(file).ToLower()))))
                    {
                        i++;
                    }
                    file = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(file), string.Format("{0}{1}{2}", System.IO.Path.GetFileNameWithoutExtension(file), i.ToString(), System.IO.Path.GetExtension(file).ToLower()));
                }

                if (workspace != null)
                {
                    workspace.IsBusy = oldBusyState;
                }
            }

            try
            {
                string ret = null;
                ImportExportEventArgs<T> m = new ImportExportEventArgs<T>();
                Dictionary<string, string> externalReferences = new Dictionary<string, string>();
                OnGetCollectionToExport(m);
                if(m.Objectlist == null)
                    return false;

                string dummyFields = String.Join("",GetDummyFields(options.Separator).ToList());
                ret = $"{ options.ObjectType}{dummyFields}{Environment.NewLine}";
                ret = $"{ret}{String.Join($"{Environment.NewLine}", ToCsv(m.Objectlist, options.Separator, externalReferences).ToList())}";

                try
                {
                    File.WriteAllText(file, ret, Encoding.Unicode);
                    if(externalReferences.Count > 0)
                    {
                        externalRefExtensionBasefolder = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(file), $"{System.IO.Path.GetFileNameWithoutExtension(file)}{Properties.Settings.Default.ImportExportExternalRefExtension}");
                        Directory.CreateDirectory(externalRefExtensionBasefolder);
                        externalReferences.Keys.ToList().ForEach(k =>
                        {
                            try
                            {
                                var extfile = System.IO.Path.Combine(externalRefExtensionBasefolder, $"{k}{Properties.Settings.Default.ImportExportExternalRefExtension}");
                                File.WriteAllText(extfile, externalReferences[k], Encoding.Unicode);
                            }
                            catch (Exception ex)
                            {
                                OnCollectError(new ImportExportEventArgs<T>(ex.Message));
                            }
                        });
                    }
                    ShowMessage(string.Format(string.Format(Properties.Resources.ExportActionSuccessfully, file)), true);
                    return true;
                }
                catch (Exception ex)
                {
                    OnCollectError(new ImportExportEventArgs<T>(ex.Message));
                    return false;
                }
            }
            catch (Exception ex)
            {
                OnCollectError(new ImportExportEventArgs<T>(ex.Message));
                return false;
            }
        }
        string externalRefExtensionBasefolder = string.Empty;
        public bool ImportFromCsv(bool bOverride)
        {
            var file = options.FilePath.ToLower();
            if (string.IsNullOrEmpty(file) || !System.IO.File.Exists(file))
            {
                if (bOverride)
                {
                    log.Warn($"{Properties.Resources.ImportExpportFileNotFound}: {file}");
                    return false;
                }

                ShowMessage(Properties.Resources.ImportExpportFileNotFound);
                return false;
            }

            string[] lineSeparator = new string[] { Environment.NewLine };
            try
            {
                var content = File.ReadAllText(file);
                var objectlist = content.Split(lineSeparator, StringSplitOptions.None).ToList();
                if ((bOverride && objectlist.Count < 2) || (!bOverride && objectlist.Count < 2))
                {
                    ShowMessage(string.Format(Properties.Resources.ImportError, file, Properties.Resources.ImportExpportFileEmpty));
                    return false;
                }
                var sep = options.Separator.ToCharArray();
                var header = objectlist[0].Split(sep)[0];
                objectlist.RemoveRange(0, 1);


                if (header != $"{options.ObjectType}")
                {
                    ShowMessage(string.Format(string.Format(Properties.Resources.ImportError, file, Properties.Resources.ImportActionTypeError)));
                    return false;
                }

                if (string.IsNullOrEmpty(objectlist[0]))
                {
                    ShowMessage(string.Format(string.Format(Properties.Resources.ImportError, file, Properties.Resources.ImportExpportFileEmpty)));
                    return false;
                }

                var headers = objectlist[0].Split(sep).ToList();
                objectlist.RemoveRange(0, 1);
                Dictionary<string, string> externalReferences = new Dictionary<string, string>();
                if (bUpdateExternalRef)
                {
                    //var basefolder = System.IO.Path.GetDirectoryName(file);
                    externalRefExtensionBasefolder = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(file), $"{System.IO.Path.GetFileNameWithoutExtension(file)}{Properties.Settings.Default.ImportExportExternalRefExtension}");
                    if(Directory.Exists(externalRefExtensionBasefolder))
                    {
                        var files = Directory.GetFiles(externalRefExtensionBasefolder, $"*{Properties.Settings.Default.ImportExportExternalRefExtension}");
                        files.ToList().ForEach(f =>
                        {
                            try
                            {
                                externalReferences.Add(Path.GetFileNameWithoutExtension(f), File.ReadAllText(f));
                            }
                            catch (Exception ex)
                            {
                                OnCollectError(new ImportExportEventArgs<T>(ex.Message));
                            }
                        });
                    }
                }
                var rawData = FromCsv(headers, objectlist, externalReferences).ToList();
                rawData.RemoveAll((data) => data.Count == 0);
                if (rawData != null && rawData.Count > 0)
                {
                    bool res;
                    try
                    {
                        bool bExit = false;
                        bool bAggregate = bAssociate && (rawData[0] as Dictionary<string, string>).Keys.Contains(assName);
                        bool bUseProto = bUsePrototype && (rawData[0] as Dictionary<string, string>).Keys.Contains(assProtoName);
                        bool bUseSubProto = bUseSubPrototype && (rawData[0] as Dictionary<string, string>).Keys.Contains(assSubProtTagOwnerPath);
                        UpdateProgressEventArgs p = new UpdateProgressEventArgs(rawData.Count);
                        OnUpdateProgress(p);
                        while (rawData.Count > 0 && !bExit)
                        {
                            object root = null;
                            if (bAggregate)
                            {
                                List<Dictionary<string, string>> aggregated;
                                string path = string.Empty;
                                string protoAssigned = string.Empty;
                                string subProtoAssigned = string.Empty;
                                
                                path = rawData[0][assName];

                                if (bUseProto)
                                {
                                    protoAssigned = rawData[0][assProtoName];
                                    if(bUseSubProto)
                                    {
                                        subProtoAssigned = rawData[0][assSubProtTagOwnerPath];

                                        aggregated = (from Dictionary<string, string> data in rawData.AsParallel()
                                                      where data.Count > 0 && data[assName] == path && data[assProtoName] == protoAssigned
                                                      && data[assSubProtTagOwnerPath] == subProtoAssigned
                                                      select data).ToList();
                                    }
                                    else 
                                        aggregated = (from Dictionary<string, string> data in rawData.AsParallel()
                                                      where data.Count > 0 && data[assName] == path && data[assProtoName] == protoAssigned
                                                      select data).ToList();
                                }
                                else
                                    aggregated = (from Dictionary<string, string> data in rawData.AsParallel()
                                    where data.Count > 0 && data[assName] == path
                                    select data).ToList();

                                if (aggregated.Count > 0)
                                {
                                    if (bUseProto)
                                    {
                                        if(bUseSubProto)
                                            rawData.RemoveAll((data) => data[assName] == path && data[assProtoName] == protoAssigned
                                            && data[assSubProtTagOwnerPath] == subProtoAssigned);
                                        else
                                            rawData.RemoveAll((data) => data[assName] == path && data[assProtoName] == protoAssigned);
                                    }
                                    else
                                        rawData.RemoveAll((data) => data[assName] == path);

                                    if (!string.IsNullOrEmpty(path))
                                    {
                                        ImportExportEventArgs<T> m = new ImportExportEventArgs<T>(path: path, root: root, proto: protoAssigned, subProto: subProtoAssigned);
                                        OnAddPath(m);
                                        root = m.Root;
                                    }

                                    bExit = AddNewObjects(aggregated,root, protoAssigned, subProtoAssigned);
                                    if (bExit)
                                        break;
                                }
                            }
                            else
                            {
                                var entry = rawData[0] as Dictionary<string, string>;
                                string protoPath = string.Empty;
                                string subProtoPath = string.Empty;
                                if (bUseProto)
                                {
                                    protoPath = entry[assProtoName];
                                    if (bUseSubProto)
                                        subProtoPath = entry[assSubProtTagOwnerPath];
                                }

                                rawData.RemoveAt(0);

                                bExit = AddEntryNewObject(entry, root, protoPath, subProtoPath);
                                if (bExit)
                                    break;
                            }
                        }
                        res = true && !bExit;
                    }
                    catch (Exception ex)
                    {
                        OnCollectError(new ImportExportEventArgs<T>(ex.Message));
                        res = false;
                    }

                    return res;
                }

                return false;
            }
            catch (Exception ex)
            {
                OnCollectError(new ImportExportEventArgs<T>(ex.Message));
                return false;
            }
        }
        void ShowMessage(string error, bool isInfo = false)
        {
            bool oldBusyState = false;
            if (workspace != null)
            {
                oldBusyState = workspace.IsBusy;
                workspace.IsBusy = false;
            }

            if (uIMsgBoxAlertService != null)
            {
                if(isInfo)
                    uIMsgBoxAlertService.ShowInformation(error);
                else
                    uIMsgBoxAlertService.ShowError(error);
            }

            if (workspace != null)
            {
                workspace.IsBusy = oldBusyState;
            }
        }
        private bool AddNewObjects(List<Dictionary<string, string>> aggregated, object root, string protoPath, string subProtoPath)
        {
            bool bExit = false;
            foreach (var entry in aggregated)
            {
                bExit = AddEntryNewObject(entry, root, protoPath, subProtoPath);
                if (bExit)
                    break;
            }
            return bExit;
        }

        private bool AddEntryNewObject(Dictionary<string, string> entry, object root, string protoPath, string subProtoPath)
        {
            if (entry == null)
                return false;

            T importedObject = default(T);
            ImportExportEventArgs<T> m = new ImportExportEventArgs<T>(
                entryKey: GetEntryKey(entry),
                entry: entry,
                obj: root,
                proto: protoPath,
                subProto: subProtoPath,
                protoNameInfo: !string.IsNullOrEmpty(importTagProtoNameInfo) && entry.ContainsKey(importTagProtoNameInfo) ? entry[importTagProtoNameInfo] : null);

            OnAddNewObject(m);
            importedObject = m.ImportedObject;
            if (importedObject != null)
            {
                SetPropValues(importedObject, entry);
                if(bUpdateAggregatedProp)
                    SetAggregatedProp(importedObject, entry);
            }
            else if(m.Message != null)
            {
                if (uIMsgBoxAlertService != null)
                    uIMsgBoxAlertService.ShowError($"{m.Message}");
            }
            UpdateProgressEventArgs n = new UpdateProgressEventArgs(-1);
            OnUpdateProgress(n);
            return m.ExitRequest;
        }

        private void SetAggregatedProp(T importedObject, Dictionary<string, string> entry)
        {
            Dictionary<string, string> rawData = new Dictionary<string, string>();
            foreach (string a in aggregatedProperties)
            {
                if(propToExportNameMap.ContainsKey(a) && 
                    entry.ContainsKey(propToExportNameMap[a]))
                {
                    rawData.Add(a, entry[propToExportNameMap[a]]);
                }
            }
            ImportExportEventArgs<T> m = new ImportExportEventArgs<T>(
                entry: rawData,
                obj: importedObject);

            OnUpdateAggregated(m);
            if (m.Message != null)
            {
                if (uIMsgBoxAlertService != null)
                    uIMsgBoxAlertService.ShowError($"{m.Message}");
            }
        }

        private IEnumerable<string> GetPropValues(T customObject, Dictionary<string, string> externalReferences)
        {
            bool bInitExtKeys = false;
            string extRefKeyValue = null;
            string extRefPropValue = null;
            foreach (PropertyInfo p in properties)
            {
                var prop = p.GetValue(customObject, null);
                if (prop == null)
                    yield return "";
                else
                {
                    string value = prop.ToString();
                    if (bUpdateExternalRef && externalPropertyReferences.Contains(p.Name) && !string.IsNullOrWhiteSpace(value))
                    {
                        //string value = prop.ToString();
                        if(!bInitExtKeys)
                        {
                            if (extRefKey != null)
                                extRefKeyValue = extRefKey.GetValue(customObject, null)?.ToString();
                            if (extRefValue != null)
                                extRefPropValue = extRefValue.GetValue(customObject, null)?.ToString();
                            bInitExtKeys = true;
                        }
                        //var guid = (from e in externalReferences.Keys where externalReferences[e] == value select e).FirstOrDefault();
                        //if(guid == null)
                        //{
                        string guid = !string.IsNullOrEmpty(extRefKeyValue) ? !string.IsNullOrEmpty(extRefPropValue) ? $"{extRefKeyValue}_{extRefPropValue}" : $"{extRefKeyValue}_{Guid.NewGuid().ToString()}" : Guid.NewGuid().ToString();
                            externalReferences.Add(guid, value);
                        //}
                        yield return guid;
                    }
                    else
                        yield return EnsureSpecialCharacters(value);
                }
                    
            }
        }

        private string EnsureSpecialCharacters(string value, bool revertDirection = false)
        {
            var res = value;
            if (string.IsNullOrEmpty(res))
                return res;
            if (revertDirection)
            {
                if(res.Contains(innerChar))
                    res = res.Replace(innerChar, options.Separator);
                if (res.Contains(innerNLine))
                    res = res.Replace(innerNLine, Environment.NewLine);
            }
            else
            {
                if (res.Contains(options.Separator))
                    res = res.Replace(options.Separator, innerChar);
                if (res.Contains(Environment.NewLine))
                    res = res.Replace(Environment.NewLine, innerNLine);
            }

            return res;
        }

        private Dictionary<string, string> GetPropValuesMap(T customObject, bool useKeylist = false)
        {
            var dictionary = new Dictionary<string, string>();
            PropertyInfo[] _properties;
            if (useKeylist)
                _properties = (from p in properties where keyList.Contains(p.Name) select p).ToArray();
            else
                _properties = properties;

            foreach (PropertyInfo p in _properties)
                {
                    string ret = string.Empty;
                    var prop = p.GetValue(customObject, null);
                    var name = propToExportNameMap[p.Name];

                    if (prop == null)
                        dictionary.Add(name, "");
                    else
                        dictionary.Add(name, prop.ToString());
                }
            return dictionary;
        }

        Dictionary<string, string> GetRawPropValues(List<string> headers, string rawValue, Dictionary<string, string> externalReferences)
        {
            if (string.IsNullOrWhiteSpace(rawValue) || headers == null)
                return new Dictionary<string, string>();
            List<string> rawValues = rawValue.Split(options.Separator.ToCharArray()).ToList();

            if (rawValues.Count != headers.Count)
                return new Dictionary<string, string>();

            var dictionary = new Dictionary<string, string>();
            foreach (string h in headers)
            {
                if (propNameList.Contains(h))
                {
                    if (bUpdateExternalRef && externalPropertyReferences.Contains(h))
                    {
                        string keyValue = rawValues[headers.IndexOf(h)];
                        if (externalReferences.ContainsKey(keyValue))
                        {
                            keyValue = externalReferences[keyValue];
                            dictionary.Add(h, keyValue);
                        }
                        else if(!string.IsNullOrEmpty(keyValue))
                        {
                            OnCollectError(new ImportExportEventArgs<T>(string.Format(Properties.Resources.ImportPropertiesError, h, keyValue, string.Format(Properties.Resources.FileNotFound, $"{externalRefExtensionBasefolder}\\{keyValue}"))));
                        }
                    }
                    else
                        dictionary.Add(h, EnsureSpecialCharacters(rawValues[headers.IndexOf(h)], true));
                }
            }
            return dictionary;
        }
        private IEnumerable<string> GetDummyFields(string separator)
        {
            yield return String.Join(separator,new string[propNameList.Count - 1]);
        }
        private IEnumerable<string> ToCsv(IEnumerable<T> objectlist, string separator, Dictionary<string, string> externalReferences)
        {
            yield return String.Join(separator, propNameList);
            foreach (var o in objectlist)
            {
                var propResult = GetPropValues(o, externalReferences);
                yield return string.Join(separator, propResult);
            }
        }

        private PropertyInfo[] GetProperties(bool onlyWritable = false)
        {
            return (from prop in options.ObjectType.GetProperties().Where(p => ((onlyWritable && p.CanWrite) | !onlyWritable) && p.GetIndexParameters().Length == 0 &&
                                         Attribute.IsDefined(p, typeof(Utilities.Exportable)))
                    select prop).ToArray();
        }

        private void SetPropValues(T customObject, Dictionary<string, string> data)
        {
            var objectPropName = GetObjectName(data);

            if (data == null || customObject == null)
                return;
            foreach (PropertyInfo p in writableProperties)
            {
                string propname = propToExportNameMap[p.Name];
                if (p.Name == keyList.LastOrDefault())
                    continue;

                try
                {
                    if (data.ContainsKey(propname))
                    {
                        Type t = VFS.ReflectionUtils.StripNullableType(p.PropertyType);
                        object safeValue = GetSafeValue(data[propname], t, p);
                        p.SetValue(customObject, safeValue);
                    }
                }
                catch (Exception ex)
                {
                    OnCollectError(new ImportExportEventArgs<T>(string.Format(Properties.Resources.ImportPropertiesError,
                        propname, objectPropName, ex.Message)));
                }
            }
        }
        
        public string GetObjectName(Dictionary<string, string> entry, string name = null)
        {
            var ret = name;
            if (keyList == null || keyList.Count == 0)
                return ret;
            var key = keyList.LastOrDefault()?.ToString();
            if (string.IsNullOrEmpty(key))
                return ret;
            ret = GetKeyValue(entry, key);
            return ret;
        }

        public string GetObjectFullName(Dictionary<string, string> entry)
        {
            var ret = string.Empty;
            if (keyList == null || keyList.Count == 0)
                return ret;
            foreach (string key in keyList)
            {
                var _v = GetKeyValue(entry, key);
                if (!string.IsNullOrEmpty(_v))
                {
                    _v = _v.Replace("\\", "/");
                    if (!string.IsNullOrEmpty(ret))
                    {
                        ret = $"{ret}/{_v}";
                    }
                    else
                    {
                        ret = _v;
                    }
                }
            }
            return ret;
        }

        private string GetKeyValue(Dictionary<string, string> entry,string  key)
        {
            if (string.IsNullOrEmpty(key))
                return null;
            if (propToExportNameMap.ContainsKey(key) && entry.ContainsKey(propToExportNameMap[key])
                && entry[propToExportNameMap[key]].Length > 0)
                return entry[propToExportNameMap[key]];
            else if (entry.ContainsKey(key))
                return entry[key];
            else
                return null;
        }

        public string GetTagOwner(Dictionary<string, string> entry)
        {
            var ret = string.Empty;
            if (string.IsNullOrEmpty(assSubProtTagOwnerPath))
                return ret;
            string key = assSubProtTagOwnerPath;
            if (propToExportNameMap.ContainsKey(key) && entry.ContainsKey(propToExportNameMap[key]))
                ret = $"{entry[propToExportNameMap[key]]}";
            else if (entry.ContainsKey(key))
                ret = $"{entry[key]}";
            return ret;
        }

        public string GetObjectName(T o)
        {
            var ret = string.Empty;
            var propertyMap = GetPropValuesMap(o,true);
            if (keyList == null || keyList.Count == 0)
                return ret;
            var key = keyList.LastOrDefault().ToString();
            if (propertyMap.ContainsKey(key))
                ret = $"{propertyMap[key]}";
            return ret;
        }

        public string GetObjectFullName(T o)
        {
            var propertyMap = GetPropValuesMap(o,true);
            if (keyList == null || keyList.Count == 0)
                return string.Empty;
            string ret = string.Empty;
            foreach (string key in keyList)
            {
                if (propertyMap.ContainsKey(key) && !string.IsNullOrEmpty(propertyMap[key]))
                    ret = $"{ret}\\{propertyMap[key]}";
            }
            return ret;
        }

        IEnumerable<dynamic> FromCsv(List<string> headers, List<string> objectlist, Dictionary<string, string> externalReferences)
        {
            foreach (var o in objectlist)
            {
                var dictionary = GetRawPropValues(headers, o, externalReferences);
                yield return dictionary;
            }
        }

        public string GetEntryKey(Dictionary<string, string> _entry)
        {
            if (keyList == null || keyList.Count == 0)
                return null;
            string ret = string.Empty;
            foreach(string key in keyList) 
            {
                if (!_entry.ContainsKey(key))
                    ret = $"{ret}{innerKeySeparator}";
                else
                    ret = $"{ret}{innerKeySeparator}{_entry[key]}";
            };

            return ret;
        }

        public string GetObjectKey(T o)
        {
            var propertyMap = GetPropValuesMap(o, true);
            return GetEntryKey(propertyMap);
        }

        #endregion

        #region events
        public event EventHandler<ImportExportEventArgs<T>> AddPath;
        private void OnAddPath(ImportExportEventArgs<T> e)
        {
            AddPath?.Invoke(null, e);
        }

        public event EventHandler<ImportExportEventArgs<T>> GetCollectionToExport;
        private void OnGetCollectionToExport(ImportExportEventArgs<T> e)
        {
            GetCollectionToExport?.Invoke(null, e);
        }

        public event EventHandler<ImportExportEventArgs<T>> AddNewObject;
        private void OnAddNewObject(ImportExportEventArgs<T> e)
        {
            AddNewObject?.Invoke(null, e);
        }

        public event EventHandler<ImportExportEventArgs<T>> UpdateAggregated;
        private void OnUpdateAggregated(ImportExportEventArgs<T> e)
        {
            UpdateAggregated?.Invoke(null, e);
        }

        public event EventHandler<UpdateProgressEventArgs> UpdateProgress;
        private void OnUpdateProgress(UpdateProgressEventArgs e)
        {
            UpdateProgress?.Invoke(null, e);
        }

        public event EventHandler<ImportExportEventArgs<T>> CollectError;
        private void OnCollectError(ImportExportEventArgs<T> e)
        {
            CollectError?.Invoke(null, e);
        }
        #endregion
    }
    public sealed class ImportExportEventArgs<T> : EventArgs
    {
        /// <summary>
        /// Constructs a new instance of MSZEventArgs.
        /// </summary>
        public ImportExportEventArgs()
        {
            _ObjectType = typeof(T);
        }

        public ImportExportEventArgs(string path, object root, string proto, string subProto)
        {
            _ObjectType = typeof(T);
            _Path = path;
            _Root = root;
            _Proto = proto;
            _SubProto = subProto;
        }

        public ImportExportEventArgs(string entryKey, Dictionary<string,string> entry, object obj, string proto, string subProto,string protoNameInfo)
        {
            _EntryKey = entryKey;
            _ObjectType = typeof(T);
            _RawData = entry;
            _Root = obj;
            _Proto = proto;
            _SubProto = subProto;
            _ProtoName = protoNameInfo;
        }

        public ImportExportEventArgs(Dictionary<string, string> entry, T obj)
        {
            _ObjectType = typeof(T);
            _RawData = entry;
            _ImportedObject = obj;
        }

        public ImportExportEventArgs(string message)
        {
            _ObjectType = typeof(T);
            _Message = message;
        }

        List<T> _Objectlist;
        public List<T> Objectlist
        {
            get { return _Objectlist; }
            set { _Objectlist = value; }
        }

        private readonly Type _ObjectType;
        public Type ObjectType
        {
            get { return _ObjectType; }
        }

        private readonly string _Path;
        public string Path
        {
            get { return _Path; }
        }

        private readonly string _SubProto;
        public string SubProto
        {
            get { return _SubProto; }
        }

        private readonly string _Proto;
        public string Proto
        {
            get { return _Proto; }
        }

        private readonly string _ProtoName;
        public string ProtoName
        {
            get { return _ProtoName; }
        }

        private object _Root;
        public object Root
        {
            get { return _Root; }
            set { _Root = value; }
        }

        private T _ImportedObject;
        public T ImportedObject
        {
            get { return _ImportedObject; }
            set { _ImportedObject = value; }
        }

        private string _Message;
        public string Message
        {
            get { return _Message; }
            set { _Message = value; }
        }

        private bool _ExitRequest;
        public bool ExitRequest
        {
            get { return _ExitRequest; }
            set { _ExitRequest = value; }
        }

        private CustomDialogResults _Response;
        public CustomDialogResults Response
        {
            get { return _Response; }
            set { _Response = value; }
        }

        private readonly Dictionary<string, string> _RawData;
        public Dictionary<string, string> RawData
        {
            get { return _RawData; }
        }
        private readonly string _EntryKey;
        public string EntryKey
        {
            get { return _EntryKey; }
        }
    }
    public sealed class UpdateProgressEventArgs : EventArgs
    {
        /// <summary>
        /// Constructs a new instance of MSZEventArgs.
        /// </summary>
        public UpdateProgressEventArgs(int counter)
        {
            _Counter = counter;
        }
        private int _Counter;
        public int Counter
        {
            get { return _Counter; }
            set { _Counter = value; }
        }
    }

}
