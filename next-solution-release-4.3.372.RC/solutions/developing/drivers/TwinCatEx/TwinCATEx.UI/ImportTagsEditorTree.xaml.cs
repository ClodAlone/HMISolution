using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Reflection;
using System.Xml.Linq;
using Utilities;
using UFUAModel;
using System.Collections.ObjectModel;
using TwinCAT.Ads;
using log4net;
using DriverCodeBaseEx.UI;
using DriverCodeBaseEx.UI.Controls;

namespace TwinCAT.UI
{
    public enum ImportUDTType : byte
    {
        Unknown,
        Struct,
        Enum,
        Pointer,
        Alias,
        Array
    }

    public enum ImportTypes : byte
    {
        Unknown,
        Standard,
        Array,
        StructOrEnum
    }

    public enum ImportVarType : byte
    {
        Normal,
        Constant,
        Persistent,
        Retain
    }

    /// <summary>
    /// Interaction logic for ImportTagsEditorTree.xaml
    /// </summary>
    public partial class ImportTagsEditorTree : UserControl, IDisposable
    {
        private class TwincatDataTypeInfo
        {
            public string Name { get; set; }
            public int nType { get; set; }
            public uint VarSize { get; set; }

            public TwincatDataTypeInfo(string name, int ntype, uint varsize)
            {
                Name = name;
                nType = ntype;
                VarSize = varsize;
            }
        }

        public class TwincatPrototypeMembersInfo
        {
            public class MemberInfo
            {
                public int nType { get; set; }
                public uint VarSize { get; set; }
                public bool Invalid { get; set; }
                public string SubElementName { get; set; }
                public string SubElementType { get; set; }
                public DataType SubMoviconType { get; set; }
                public int ArrayDimension { get; set; }
                public ImportTypes SubTypeCategory { get; set; }
            }

            public string[] RawMembers { get; set; }
            private MemberInfo[] _Members = null;
            public MemberInfo[] Members { get { return _Members; } set { _Members = value; } }
            public TwincatPrototypeMembersInfo()
            {
                RawMembers = new string[0];
                Members = null;
            }

            public TwincatPrototypeMembersInfo(string[] rawMembers)
            {
                RawMembers = rawMembers;
                Members = null;
            }

            public bool NoMembers()
            {
                return (RawMembers == null || RawMembers.Length == 0);
            }

            public string[] GetRawMembers()
            {
                if (RawMembers == null)
                    return new string[0];
                else
                    return RawMembers;
            }

            public string GetRawMember(int index)
            {
                if (RawMembers == null)
                    return string.Empty;
                else
                    return RawMembers[index];
            }

            public MemberInfo[] GetMembers()
            {
                if (_Members == null)
                    return new MemberInfo[0];
                else
                    return _Members;
            }

            public bool IsMembersParsed()
            {
                return (Members != null);
            }

            public void AddMember(TwincatPrototypeMembersInfo.MemberInfo member)
            {
                if (Members == null)
                    _Members = new MemberInfo[0];
                Array.Resize(ref _Members, Members.Length + 1);
                _Members[Members.Length - 1] = member;
            }
        }

        ImportDataModelTwinCAT importDataModel;
        string conn;
        bool alreadyLoaded = false;
        List<string> stationList;
        Dictionary<string, byte> channelList;
        Dictionary<string, byte> TwinCATVersionList;
        byte TwinCATVersion;
        Dictionary<string, TwincatPrototypeMembersInfo> mapSTRUCT;
        Dictionary<string, TwincatPrototypeMembersInfo> mapFunctionBlock;
        Dictionary<string, TwincatPrototypeMembersInfo> mapUDT;
        Dictionary<string, int> mapConstants;
        Dictionary<string, TwincatPrototypeMembersInfo> mapOK;
        Dictionary<string, TwincatPrototypeMembersInfo> mapTemp;
        bool bErrorsOccurredDuringImport = false;
        int m_lGlobalID = 0;
        public GetStationName readStationName;
        BaseImportTree baseImportTree;

        static string ImportEnumAlias = "___Movicon_Import_Enum_Alias";
        static string ImportStandardTypeAlias = "___Movicon_Import_StandardType_Alias";

        Dictionary<string, TwincatDataTypeInfo> mapSupportedTwincatDataType = new Dictionary<string, TwincatDataTypeInfo>();

        public ImportTagsEditorTree()
        {
            InitializeComponent();
            Loaded += (o, e) =>
            {
                if (alreadyLoaded)
                {
                    importDataModel = null;
                    return;
                }

                alreadyLoaded = true;
                List<string> lista = DataContext as List<string>;
                if (lista == null || lista.Count < 2)
                    return;

                conn = lista[0];

                Assembly a = Assembly.GetAssembly(this.GetType());
                string DriverName = a.GetName().Name;
                DriverName = DriverName.Replace(".UI", "");

                // Add column to the Import Grid
                List<BaseImportTree.GridColData> columns = new List<BaseImportTree.GridColData>();
                columns.Add(new BaseImportTree.GridColData()
                {
                    colName = "Name",
                    bindingName = "TagName",
                    colWidth = 300
                });
                columns.Add(new BaseImportTree.GridColData()
                {
                    colName = "Address",
                    bindingName = "TagAddress",
                    colWidth = 250
                });
                columns.Add(new BaseImportTree.GridColData()
                {
                    colName = "Type",
                    bindingName = "TagType",
                    colWidth = 200
                });

                baseImportTree = new BaseImportTree(DriverName, lista[0],
                   (lista[1].ToLower().IndexOf("true") != -1), columns);
                baseImportTree.SetVisibleButtons(bGetPLCTags: true);
                baseImportTree.SetFileFilter("tpy files|*.tpy");
                baseImportTree.LoadImportFile = LoadFile;
                baseImportTree.GetPlcTags = Read_Plc_Info;
                baseImportTree.CreateItemControl = CreateImportDataTreeItemControl;
                baseImportTree.AddTrueChildren = SubstituteDummyChildWithTrueChildren;
                MainStack.Children.Add(baseImportTree);

                readStationName = baseImportTree.funcGetStationName();

                DataContext = this;

                stationList = new List<string>();
                channelList = new Dictionary<string, byte>();
                TwinCATVersionList = new Dictionary<string, byte>();
                TwinCATVersion = (byte)TwinCAT.TwinCATVersions.Version2x;
                mapSTRUCT = new Dictionary<string, TwincatPrototypeMembersInfo>();
                mapFunctionBlock = new Dictionary<string, TwincatPrototypeMembersInfo>();
                mapUDT = new Dictionary<string, TwincatPrototypeMembersInfo>();
                mapConstants = new Dictionary<string, int>();
                mapOK = new Dictionary<string, TwincatPrototypeMembersInfo>();
                mapTemp = new Dictionary<string, TwincatPrototypeMembersInfo>();
                bErrorsOccurredDuringImport = false;

                FillStationList();

                DefineSupportedTwincatDataType();
            };
        }

        private ImportDataTreeItemControl CreateImportDataTreeItemControl(object tag)
        {
            return ((ImportDataTreeItemControl)new ImportDataTreeItemControlTwinCAT(tag));
        }

        private void FillStationList()
        {
            string channelName;
            byte TwinCATVer;

            stationList.Clear();

            foreach (var settings in baseImportTree.GetStationSettingsList().Values)
            {
                stationList.Add(settings.Name);
                channelName = settings.Channel;
                TwinCATVer = (byte)TwinCAT.TwinCATVersions.Version2x;

                if (String.IsNullOrEmpty(channelName) || !channelList.TryGetValue(channelName, out TwinCATVer))
                {
                    TwinCATVer = (byte)TwinCAT.TwinCATVersions.Version2x;
                }

                TwinCATVersionList.Add(settings.Name, TwinCATVer);
            }
            stationList.Sort();
        }

        public void ImportSelectedTags()
        {
            string importfolder = baseImportTree.ReadFolderName();
            List<ImportData> list = new List<ImportData>();
            list = baseImportTree.GetSelectedTags();
            string stationName = baseImportTree.ReadStationName();
            int behaviorExistingTags = baseImportTree.GetBehaviorForExistingTags();
            int behaviorDynamicLink = baseImportTree.GetBehaviorForDynamicLink();

            if (list.Count > 0)
            {
                using (ImportDataProgressBar progressBar = new ImportDataProgressBar())
                {
                    // Set the number of tags that will be imported
                    progressBar.StartImport(list.Count);
                    try
                    {
                        List<ImportTag> taglist = new List<ImportTag>();
                        Dictionary<string, ImportPrototype> protoMap = new Dictionary<string, ImportPrototype>();
                        Dictionary<object, bool> notToBeImported = new Dictionary<object, bool>();

                        foreach (var elemList in list)
                        {
                            progressBar.ImportNewTag(elemList.Name);
                            progressBar.ThrowIfCancellationRequested();

                            ImportDataTwinCAT elemTwinCAT = elemList as ImportDataTwinCAT;
                            if (elemTwinCAT != null && (!notToBeImported.ContainsKey(elemTwinCAT)))
                            {                                
                                List<ImportDataTwinCAT> expandedItemList = new List<ImportDataTwinCAT>();
                                Dictionary<string, ImportPrototype> prototypesToBeCreated = new Dictionary<string, ImportPrototype>();
                                Dictionary<string, bool> parentTypes = new Dictionary<string, bool>();
                                ExpandItemToBeImported(elemTwinCAT, ref expandedItemList, ref prototypesToBeCreated, ref parentTypes, progressBar, null);

                                foreach (ImportDataTwinCAT elem in expandedItemList)
                                {
                                    if (elem != null)
                                    {
                                        if (elem.ImportDataType == ImportTypes.Standard)
                                        {
                                            TwinCATDynTagSettings ds = new TwinCATDynTagSettings();
                                            if (!ds.ParseAddress(elem.Address))
                                            {
                                                continue;
                                            }
                                            ds.Address = elem.Address;
                                            ds.StationName = stationName;
                                            ds.ArrayDimension = 0;
                                            ds.Length = 0;
                                            // Special case String: set string length (DL)
                                            if (elem.Type == DataType.String)
                                            {
                                                ds.Length = elem.Size;
                                            }
                                            // Special case Constant: set task type = 0 = Input 
                                            if (VarIsConstant(elem.Description))
                                            {
                                                ds.TagLinkType = (int)DriverCodeBaseEx.Enumerators.LinkType.Input;
                                            }
                                            elem.DynAddress = ds.ToString();
                                            string importTagName = ((ImportDataTwinCAT)elem).ImportTagName.Trim(']');
                                            ImportTag tagtoimport = new ImportTag()
                                            {
                                                Name = importTagName.Trim(),
                                                DataType = elem.Type,
                                                DynSettings = elem.DynAddress,
                                                Folder = importfolder,
                                                ModelType = UFUAModel.ModelType.Variable,
                                                Description = elem.Description,
                                                ArrayDimension = elem.ArrayDimension,
                                                BehaviorForExistingTags = (BehaviorExistingTagsValues)behaviorExistingTags,
                                                BehaviorForDynamicLink = (BehaviorDynamicLinkValues)behaviorDynamicLink,
                                            };
                                            taglist.Add(tagtoimport);
                                        }
                                        else if (elem.ImportDataType == ImportTypes.Array)
                                        {
                                            TwinCATDynTagSettings ds = new TwinCATDynTagSettings();
                                            if (!ds.ParseAddress(elem.Address))
                                            {
                                                continue;
                                            }
                                            ds.Address = elem.Address;
                                            ds.StationName = stationName;
                                            ds.ArrayDimension = elem.ArrayDimension;
                                            ds.Length = 0;
                                            elem.DynAddress = ds.ToString();
                                            string importTagName = ((ImportDataTwinCAT)elem).ImportTagName.Trim(']');
                                            ImportTag tagtoimport = new ImportTag()
                                            {
                                                Name = importTagName.Trim(),
                                                DataType = elem.Type,
                                                DynSettings = elem.DynAddress,
                                                Folder = importfolder,
                                                ModelType = UFUAModel.ModelType.Variable,
                                                Description = elem.Description,
                                                ArrayDimension = elem.ArrayDimension,
                                                BehaviorForExistingTags = (BehaviorExistingTagsValues)behaviorExistingTags,
                                                BehaviorForDynamicLink = (BehaviorDynamicLinkValues)behaviorDynamicLink,
                                            };
                                            taglist.Add(tagtoimport);
                                            foreach (ImportData el in elem.Children)
                                            {
                                                if (el != null)
                                                {
                                                    notToBeImported[el] = true;
                                                }
                                            }
                                        }
                                        else if (elem.ImportDataType == ImportTypes.StructOrEnum)
                                        {
                                            TwinCATDynTagSettings ds = new TwinCATDynTagSettings();
                                            if (!ds.ParseAddress(elem.Address))
                                            {
                                                continue;
                                            }
                                            ds.Address = elem.Address;
                                            ds.StationName = stationName;
                                            ds.ArrayDimension = elem.ArrayDimension;
                                            ds.Length = 0;
                                            elem.DynAddress = ds.ToString();
                                            string importTagName = ((ImportDataTwinCAT)elem).ImportTagName.Trim(']');
                                            ImportTag tagtoimport = new ImportTag()
                                            {
                                                Name = importTagName.Trim(),
                                                DataType = elem.Type,
                                                DynSettings = elem.DynAddress,
                                                Folder = importfolder,
                                                ModelType = UFUAModel.ModelType.ObjectType,
                                                PrototypeModel = elem.ElemType,
                                                Description = elem.Description,
                                                ArrayDimension = elem.ArrayDimension,
                                                BehaviorForExistingTags = (BehaviorExistingTagsValues)behaviorExistingTags,
                                                BehaviorForDynamicLink = (BehaviorDynamicLinkValues)behaviorDynamicLink,
                                            };
                                            taglist.Add(tagtoimport);
                                        }
                                    }
                                }
                                foreach (string protoName in prototypesToBeCreated.Keys)
                                {
                                    if (!protoMap.ContainsKey(protoName))
                                    {
                                        ImportPrototype importPrototype = new ImportPrototype();
                                        if (prototypesToBeCreated.TryGetValue(protoName, out importPrototype) == true)
                                        {
                                            protoMap.Add(protoName, importPrototype);
                                        }
                                    }
                                }
                            }
                        }

                        DataContext = new ImportObject() { PrototypesToImport = protoMap.Values.ToList(), TagsToImport = taglist };
                    }
                    catch (OperationCanceledException ex)
                    {
                        //UFUAServerDocument.logGeneral.Info(Properties.Resources.TagsImportCanceled);
                    }
                }
            }            
        }

        bool IsStringType(string szType)
        {
            bool isString = false;
            uint varSize = 0;
            string varType = String.Copy(szType);
            if (GetVarTypeAndSize(ref varType, ref varSize) != -1)
            {
                if (varType == "STRING")
                {
                    isString = true;
                }
            }

            return (isString);
        }

        private void ExpandItemToBeImported(ImportDataTwinCAT itemToBeExpanded, ref List<ImportDataTwinCAT> expandedItem, ref Dictionary<string, ImportPrototype> protoMap, ref Dictionary<string, bool> parentTypes, ImportDataProgressBar progressBar, ImportPrototype parentPrototype = null)
        {
            progressBar.ThrowIfCancellationRequested();
            progressBar.UpdateImportingTag(itemToBeExpanded.Name);

            if (itemToBeExpanded == null)
            {
                return;
            }

            // Get the category and the information of the element type
            uint nVarSize = 0;
            uint nArraySize0 = 0;
            uint nArraySize1 = 0;
            uint nArraySize2 = 0;
            uint nArrayStart0 = 0;
            uint nArrayStart1 = 0;
            uint nArrayStart2 = 0;
            int moviconType = (int)DataType.Boolean;
            string elementType = String.Empty;
            if (String.IsNullOrWhiteSpace(itemToBeExpanded.ElemType) && !String.IsNullOrWhiteSpace(itemToBeExpanded.szType))
            {
                itemToBeExpanded.ElemType = itemToBeExpanded.szType;
            }
            elementType = String.Copy(itemToBeExpanded.ElemType);
            string elementName = String.Copy(itemToBeExpanded.Name);
            ImportTypes typeCategory = (ImportTypes)GetVarTypeAndSizeFileExp(ref elementType, ref nVarSize, ref nArraySize0, ref nArraySize1, ref nArraySize2, ref nArrayStart0, ref nArrayStart1, ref nArrayStart2, ref moviconType);
            if (typeCategory == ImportTypes.Unknown)
            {
                return;
            }

            ImportTypes arrayElementCategory = typeCategory;

            // Eventually, force the type category to array
            if ((typeCategory != ImportTypes.Array) && (itemToBeExpanded.ArrayDimension > 0))
            {
                typeCategory = ImportTypes.Array;
                if (nArraySize0 == 0)
                {
                    nArraySize0 = itemToBeExpanded.ArrayDimension;
                }
            }

            switch (typeCategory)
            {
                // Standard item
                case ImportTypes.Standard:
                    // Not an element of a parent structure
                    if ((parentPrototype == null))
                    {
                        expandedItem.Add(itemToBeExpanded);
                    }

                    // element of a parent structure --> Add it to the parent prototype 
                    else
                    {
                        if (!IsStringType(elementType))
                        {
                            if (parentPrototype.Elements == null)
                            {
                                parentPrototype.Elements = new List<ImportTag>();
                            }
                            ImportTag importProtoElement = new ImportTag();
                            importProtoElement.Name = itemToBeExpanded.Name;
                            importProtoElement.ModelType = ModelType.Variable;
                            importProtoElement.ArrayDimension = 0;
                            importProtoElement.DataType = (DataType)moviconType;
                            parentPrototype.Elements.Add(importProtoElement);
                        }
                        // Special case: Strings must be extracted from the structure
                        else
                        {
                            ImportDataTwinCAT stringToBeExtracted = (ImportDataTwinCAT)importDataModel.addImportData();
                            stringToBeExtracted.PreName = itemToBeExpanded.PreName;
                            stringToBeExtracted.Name = elementName;
                            stringToBeExtracted.szType = elementType;
                            stringToBeExtracted.ElemType = elementType;
                            stringToBeExtracted.Type = DataType.String;
                            stringToBeExtracted.Size = nVarSize;
                            stringToBeExtracted.ImportDataType = ImportTypes.Standard;
                            stringToBeExtracted.Address = itemToBeExpanded.Address + elementName;
                            expandedItem.Add(stringToBeExtracted);
                        }
                    }
                    break;

                // Array
                case ImportTypes.Array:
                    {
                        string elemType = string.Copy(elementType);
                        uint elemSize = 0;
                        int movType = GetVarTypeAndSize(ref elemType, ref elemSize);
                        // Element of Standard type?
                        if (movType >= 0)
                        {
                            if (nArraySize0 == 0)
                            {
                                return;
                            }

                            if (!IsStringType(elementType))
                            {
                                // One dimensional array --> Import the variable
                                if (nArraySize1 == 0)
                                {
                                    // If the array is an element of a parent structure,
                                    // add the array name to the address
                                    itemToBeExpanded.ArrayDimension = nArraySize0;
                                    itemToBeExpanded.Type = (DataType)movType;
                                    if (parentPrototype == null)
                                    {
                                        expandedItem.Add(itemToBeExpanded);
                                    }
                                    else
                                    {
                                        //itemToBeExpanded.Address += elementName;
                                        if (parentPrototype.Elements == null)
                                        {
                                            parentPrototype.Elements = new List<ImportTag>();
                                        }
                                        ImportTag importProtoElement = new ImportTag();
                                        importProtoElement.Name = itemToBeExpanded.Name;
                                        importProtoElement.ModelType = ModelType.Variable;
                                        importProtoElement.ArrayDimension = nArraySize0;
                                        importProtoElement.DataType = (DataType)movType;
                                        parentPrototype.Elements.Add(importProtoElement);
                                    }
                                }

                                // Two dimensional array --> Split the array into unidimensional arrays to be imported
                                else if (nArraySize2 == 0)
                                {
                                    for (uint nIdx = nArrayStart0; nIdx < nArrayStart0 + nArraySize0; nIdx++)
                                    {
                                        string szName = String.Empty;
                                        if (parentPrototype == null)
                                        {
                                            szName = String.Format("[{0},{1}", nIdx, nArrayStart1);
                                        }
                                        else
                                        {
                                            szName = String.Format("{0}[{1},{2}", elementName, nIdx, nArrayStart1);
                                        }

                                        ImportDataTwinCAT arrayToBeExtracted = (ImportDataTwinCAT)importDataModel.addImportData();
                                        arrayToBeExtracted.PreName = itemToBeExpanded.PreName;
                                        arrayToBeExtracted.Name = szName;
                                        arrayToBeExtracted.szType = elementType;
                                        arrayToBeExtracted.ElemType = elementType;
                                        arrayToBeExtracted.Type = (DataType)movType;
                                        arrayToBeExtracted.ArrayDimension = nArraySize1;
                                        arrayToBeExtracted.ImportDataType = ImportTypes.Array;
                                        arrayToBeExtracted.Address = itemToBeExpanded.Address + szName + "]";
                                        expandedItem.Add(arrayToBeExtracted);
                                    }
                                }

                                // Three dimensional array --> Split the array into unidimensional arrays to be imported
                                else
                                {
                                    for (uint nIdx2 = nArrayStart0; nIdx2 < nArrayStart0 + nArraySize0; nIdx2++)
                                    {
                                        for (uint nIdx = nArrayStart1; nIdx < nArrayStart1 + nArraySize1; nIdx++)
                                        {
                                            string szName = String.Empty;
                                            if (parentPrototype == null)
                                            {
                                                szName = String.Format("[{0},{1},{2}", nIdx2, nIdx, nArrayStart2);
                                            }
                                            else
                                            {
                                                szName = String.Format("{0}[{1},{2},{3}", elementName, nIdx2, nIdx, nArrayStart2);
                                            }

                                            ImportDataTwinCAT arrayToBeExtracted = (ImportDataTwinCAT)importDataModel.addImportData();
                                            arrayToBeExtracted.PreName = itemToBeExpanded.PreName;
                                            arrayToBeExtracted.Name = szName;
                                            arrayToBeExtracted.szType = elementType;
                                            arrayToBeExtracted.ElemType = elementType;
                                            arrayToBeExtracted.Type = (DataType)movType;
                                            arrayToBeExtracted.ArrayDimension = nArraySize2;
                                            arrayToBeExtracted.ImportDataType = ImportTypes.Array;
                                            arrayToBeExtracted.Address = itemToBeExpanded.Address + szName + "]";
                                            expandedItem.Add(arrayToBeExtracted);
                                        }
                                    }
                                }
                            }

                            // Special case: strings must be extracted from the array (it doesn't matter if the array is member of a parent structure)
                            else
                            {
                                // Extract each string from the array 
                                if (nArraySize0 == 0)
                                {
                                    return;
                                }

                                // One dimensional array
                                if (nArraySize1 == 0)
                                {
                                    for (uint nIdx = nArrayStart0; nIdx < nArrayStart0 + nArraySize0; nIdx++)
                                    {
                                        string szName = String.Empty;
                                        if (parentPrototype == null)
                                        {
                                            szName = String.Format("[{0}", nIdx);
                                        }
                                        else
                                        {
                                            szName = String.Format("{0}[{1}", elementName, nIdx);
                                        }
                                        ImportDataTwinCAT stringToBeExtracted = (ImportDataTwinCAT)importDataModel.addImportData();
                                        stringToBeExtracted.PreName = itemToBeExpanded.Name;
                                        stringToBeExtracted.Name = szName;
                                        stringToBeExtracted.szType = elementType;
                                        stringToBeExtracted.ElemType = elementType;
                                        stringToBeExtracted.Type = DataType.String;
                                        stringToBeExtracted.ArrayDimension = 0;
                                        stringToBeExtracted.ImportDataType = ImportTypes.Standard;
                                        stringToBeExtracted.Size = nVarSize;
                                        stringToBeExtracted.Address = itemToBeExpanded.Address + szName + "]";
                                        expandedItem.Add(stringToBeExtracted);
                                    }
                                }

                                // Two dimensional array
                                else if (nArraySize2 == 0)
                                {
                                    for (uint nIdx2 = nArrayStart0; nIdx2 < nArrayStart0 + nArraySize0; nIdx2++)
                                    {
                                        for (uint nIdx = nArrayStart1; nIdx < nArrayStart1 + nArraySize1; nIdx++)
                                        {
                                            string szName = String.Empty;
                                            if (parentPrototype == null)
                                            {
                                                szName = String.Format("[{0},{1}", nIdx2, nIdx);
                                            }
                                            else
                                            {
                                                szName = String.Format("{0}[{1},{2}", elementName, nIdx2, nIdx);
                                            }
                                            ImportDataTwinCAT stringToBeExtracted = (ImportDataTwinCAT)importDataModel.addImportData();
                                            stringToBeExtracted.PreName = itemToBeExpanded.Name;
                                            stringToBeExtracted.Name = szName;
                                            stringToBeExtracted.szType = elementType;
                                            stringToBeExtracted.ElemType = elementType;
                                            stringToBeExtracted.Type = DataType.String;
                                            stringToBeExtracted.ArrayDimension = 0;
                                            stringToBeExtracted.ImportDataType = ImportTypes.Standard;
                                            stringToBeExtracted.Size = nVarSize;
                                            stringToBeExtracted.Address = itemToBeExpanded.Address + szName + "]";
                                            expandedItem.Add(stringToBeExtracted);
                                        }
                                    }
                                }

                                // Three dimensional array
                                else
                                {
                                    for (uint nIdx2 = nArrayStart0; nIdx2 < nArrayStart0 + nArraySize0; nIdx2++)
                                    {
                                        for (uint nIdx = nArrayStart1; nIdx < nArrayStart1 + nArraySize1; nIdx++)
                                        {
                                            for (uint nIdx3 = nArrayStart2; nIdx3 < nArrayStart2 + nArraySize2; nIdx3++)
                                            {
                                                string szName = String.Empty;
                                                if (parentPrototype == null)
                                                {
                                                    szName = String.Format("[{0},{1},{2}", nIdx2, nIdx, nIdx3);
                                                }
                                                else
                                                {
                                                    szName = String.Format("{0}[{1},{2},{3}", elementName, nIdx2, nIdx, nIdx3);
                                                }
                                                ImportDataTwinCAT stringToBeExtracted = (ImportDataTwinCAT)importDataModel.addImportData();
                                                stringToBeExtracted.PreName = itemToBeExpanded.Name;
                                                stringToBeExtracted.Name = szName;
                                                stringToBeExtracted.szType = elementType;
                                                stringToBeExtracted.ElemType = elementType;
                                                stringToBeExtracted.Type = DataType.String;
                                                stringToBeExtracted.ArrayDimension = 0;
                                                stringToBeExtracted.ImportDataType = ImportTypes.Standard;
                                                stringToBeExtracted.Size = nVarSize;
                                                stringToBeExtracted.Address = itemToBeExpanded.Address + szName + "]";
                                                expandedItem.Add(stringToBeExtracted);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        // Structure: each structure must be extracted from the array
                        else
                        {
                            // Extract each structure from the array 
                            if (nArraySize0 == 0)
                            {
                                return;
                            }

                            // One dimensional array
                            if (nArraySize1 == 0)
                            {
                                for (uint nIdx = nArrayStart0; nIdx < nArrayStart0 + nArraySize0; nIdx++)
                                {
                                    string szName = String.Empty;
                                    if (parentPrototype == null)
                                    {
                                        szName = String.Format("[{0}", nIdx);
                                    }
                                    else
                                    {
                                        szName = String.Format("{0}[{1}", elementName, nIdx);
                                    }
                                    ImportDataTwinCAT structToBeExtracted = (ImportDataTwinCAT)importDataModel.addImportData();
                                    structToBeExtracted.PreName = itemToBeExpanded.PreName;
                                    structToBeExtracted.Name = szName;
                                    structToBeExtracted.szType = elementType;
                                    structToBeExtracted.ElemType = elementType;
                                    structToBeExtracted.Type = (DataType)moviconType;
                                    structToBeExtracted.ArrayDimension = 0;
                                    structToBeExtracted.ImportDataType = ImportTypes.StructOrEnum;
                                    structToBeExtracted.Address = itemToBeExpanded.Address + szName + "]";
                                    // Recursive call for extracting the structure from the parent array
                                    ExpandItemToBeImported(structToBeExtracted, ref expandedItem, ref protoMap, ref parentTypes, progressBar, null);
                                }
                            }

                            // Two dimensional array
                            else if (nArraySize2 == 0)
                            {
                                for (uint nIdx2 = nArrayStart0; nIdx2 < nArrayStart0 + nArraySize0; nIdx2++)
                                {
                                    for (uint nIdx = nArrayStart1; nIdx < nArrayStart1 + nArraySize1; nIdx++)
                                    {
                                        string szName = String.Empty;
                                        if (parentPrototype == null)
                                        {
                                            szName = String.Format("[{0},{1}", nIdx2, nIdx);
                                        }
                                        else
                                        {
                                            szName = String.Format("{0}[{1},{2}", elementName, nIdx2, nIdx);
                                        }
                                        ImportDataTwinCAT structToBeExtracted = (ImportDataTwinCAT)importDataModel.addImportData();
                                        structToBeExtracted.PreName = itemToBeExpanded.PreName;
                                        structToBeExtracted.Name = szName;
                                        structToBeExtracted.szType = elementType;
                                        structToBeExtracted.ElemType = elementType;
                                        structToBeExtracted.Type = (DataType)moviconType;
                                        structToBeExtracted.ArrayDimension = 0;
                                        structToBeExtracted.ImportDataType = ImportTypes.StructOrEnum;
                                        structToBeExtracted.Address = itemToBeExpanded.Address + szName + "]";
                                        // Recursive call for extracting the structure from the parent array
                                        ExpandItemToBeImported(structToBeExtracted, ref expandedItem, ref protoMap, ref parentTypes, progressBar, null);
                                    }
                                }
                            }

                            // Three dimensional array
                            else
                            {
                                for (uint nIdx2 = nArrayStart0; nIdx2 < nArrayStart0 + nArraySize0; nIdx2++)
                                {
                                    for (uint nIdx = nArrayStart1; nIdx < nArrayStart1 + nArraySize1; nIdx++)
                                    {
                                        for (uint nIdx3 = nArrayStart2; nIdx3 < nArrayStart2 + nArraySize2; nIdx3++)
                                        {
                                            string szName = String.Empty;
                                            if (parentPrototype == null)
                                            {
                                                szName = String.Format("[{0},{1},{2}", nIdx2, nIdx, nIdx3);
                                            }
                                            else
                                            {
                                                szName = String.Format("{0}[{1},{2},{3}", elementName, nIdx2, nIdx, nIdx3);
                                            }
                                            ImportDataTwinCAT structToBeExtracted = (ImportDataTwinCAT)importDataModel.addImportData();
                                            structToBeExtracted.PreName = itemToBeExpanded.PreName;
                                            structToBeExtracted.Name = szName;
                                            structToBeExtracted.szType = elementType;
                                            structToBeExtracted.ElemType = elementType;
                                            structToBeExtracted.Type = (DataType)moviconType;
                                            structToBeExtracted.ArrayDimension = 0;
                                            structToBeExtracted.ImportDataType = ImportTypes.StructOrEnum;
                                            structToBeExtracted.Address = itemToBeExpanded.Address + szName + "]";
                                            // Recursive call for extracting the structure from the parent array
                                            ExpandItemToBeImported(structToBeExtracted, ref expandedItem, ref protoMap, ref parentTypes, progressBar, null);
                                        }
                                    }
                                }
                            }

                        }
                    }
                    break;

                // Structure
                case ImportTypes.StructOrEnum:
                    {
                        string elemType = String.Empty;
                        if (String.IsNullOrWhiteSpace(itemToBeExpanded.ElemType) && !String.IsNullOrWhiteSpace(itemToBeExpanded.szType))
                        {
                            itemToBeExpanded.ElemType = itemToBeExpanded.szType;
                        }
                        elemType = String.Copy(itemToBeExpanded.ElemType);

                        TwincatPrototypeMembersInfo prototypeDefinition = GetPrototypeDefinition(elemType, parentTypes/*, itemToBeExpanded*/);
                        if (prototypeDefinition == null)
                        {
                            // If the item is not a structure, something went wrong: do nothing.
                            return;
                        }
                        ImportPrototype proto = new ImportPrototype();
                        proto.Name = elemType;
                        proto.Elements = new List<ImportTag>();
                        if (!parentTypes.ContainsKey(elementType))
                        {
                            parentTypes[elementType] = true;
                        }

                        foreach (var protoElement in prototypeDefinition.GetMembers())
                        {
                            if (protoElement.Invalid)
                                continue;
                            // Avoid recursions in the type definition
                            if (parentTypes.ContainsKey(protoElement.SubElementType))
                                continue;
                            ImportDataTwinCAT elementOfStruct = (ImportDataTwinCAT)importDataModel.addImportData();
                            elementOfStruct.PreName = itemToBeExpanded.PreName + itemToBeExpanded.Name + ".";
                            elementOfStruct.Name = protoElement.SubElementName;
                            elementOfStruct.szType = protoElement.SubElementType;
                            elementOfStruct.ElemType = protoElement.SubElementType;
                            elementOfStruct.Type = protoElement.SubMoviconType;
                            elementOfStruct.ArrayDimension = 0;
                            elementOfStruct.ImportDataType = protoElement.SubTypeCategory;
                            elementOfStruct.Address = itemToBeExpanded.Address + ".";
                            if (protoElement.SubTypeCategory == ImportTypes.StructOrEnum)
                            {
                                elementOfStruct.Address += protoElement.SubElementName;
                            }
                            // Recursive call for an element of a structure
                            ExpandItemToBeImported(elementOfStruct, ref expandedItem, ref protoMap, ref parentTypes, progressBar, proto);
                        }

                        if (parentTypes.ContainsKey(elementType))
                        {
                            parentTypes.Remove(elementType);
                        }

                        if (proto.Elements.Count > 0)
                        {
                            if (!protoMap.ContainsKey(proto.Name))
                            {
                                protoMap.Add(proto.Name, proto);
                            }
                            if (parentPrototype == null)
                            {
                                if (itemToBeExpanded.ImportDataType == ImportTypes.Unknown)
                                {
                                    itemToBeExpanded.ImportDataType = ImportTypes.StructOrEnum;
                                }
                                expandedItem.Add(itemToBeExpanded);
                            }
                            // Member of a parent struct
                            else
                            {
                                ImportTag importProtoElement = new ImportTag();
                                importProtoElement.Name = itemToBeExpanded.Name;
                                importProtoElement.ModelType = ModelType.ObjectType;
                                importProtoElement.ArrayDimension = 0;
                                importProtoElement.PrototypeModel = proto.Name;
                                parentPrototype.Elements.Add(importProtoElement);
                            }
                        }
                    }
                    break;
            }
        }

        bool GetStructureElementNameAndType(string elementDefinition, ref string elementName, ref string elementType)
        {
            // Spit the element definiton
            string[] elementDefinitionMembers = elementDefinition.Split(':');
            if (elementDefinitionMembers.Length < 2)
            {
                return (false);
            }

            // Get the name of the field
            string auxName = elementDefinitionMembers[0].Trim();
            if (String.IsNullOrWhiteSpace(auxName) == true)
            {
                return (false);
            }
            // Get the type of the field
            string auxType = elementDefinitionMembers[1].Trim();
            if (String.IsNullOrWhiteSpace(auxType) == true)
            {
                return (false);
            }

            elementName = auxName;
            elementType = auxType;

            return (true);
        }

        private void ParsePrototype(TwincatPrototypeMembersInfo prototypeDefinition, Dictionary<string, bool> parentTypes/*, ImportDataTwinCAT itemToBeExpanded*/)
        {
            foreach (var protoElement in prototypeDefinition.GetRawMembers())
            {
                TwincatPrototypeMembersInfo.MemberInfo elementOfStruct = new TwincatPrototypeMembersInfo.MemberInfo();

                //progressBar.ThrowIfCancellationRequested();

                // Get the name and the type of the element
                string subElementName = string.Empty;
                string subElementType = string.Empty;
                if (GetStructureElementNameAndType(protoElement, ref subElementName, ref subElementType) == false)
                {
                    elementOfStruct.Invalid = true;
                    prototypeDefinition.AddMember(elementOfStruct);
                    continue;
                }
                elementOfStruct.SubElementType = subElementType;
                // Avoid recursions in the type definition                
                if (parentTypes.ContainsKey(subElementType))
                {
                    prototypeDefinition.AddMember(elementOfStruct);
                    continue;
                }

                // Get the category and the information of the element type
                uint subVarSize = 0;
                uint subArraySize0 = 0;
                uint subArraySize1 = 0;
                uint subArraySize2 = 0;
                uint subArrayStart0 = 0;
                uint subArrayStart1 = 0;
                uint subArrayStart2 = 0;
                int subMoviconType = (int)DataType.Boolean;
                string subType = string.Copy(subElementType);
                ImportTypes subTypeCategory = (ImportTypes)GetVarTypeAndSizeFileExp(ref subType, ref subVarSize, ref subArraySize0, ref subArraySize1, ref subArraySize2, ref subArrayStart0, ref subArrayStart1, ref subArrayStart2, ref subMoviconType);
                //ImportDataTwinCAT elementOfStruct = (ImportDataTwinCAT)importDataModel.addImportData();
                //elementOfStruct.PreName = itemToBeExpanded.PreName + itemToBeExpanded.Name + ".";
                elementOfStruct.SubElementName = subElementName;
                elementOfStruct.SubElementType = subElementType;
                elementOfStruct.SubMoviconType = (DataType)subMoviconType;
                elementOfStruct.ArrayDimension = 0;
                elementOfStruct.SubTypeCategory = subTypeCategory;
                //elementOfStruct.Address = itemToBeExpanded.Address + ".";
                //if (subTypeCategory == ImportTypes.StructOrEnum)
                //{
                //    elementOfStruct.Address += subElementName;
                //}
                prototypeDefinition.AddMember(elementOfStruct);
            }
        }

        private TwincatPrototypeMembersInfo GetPrototypeDefinition(string prototypeName, Dictionary<string, bool> parentTypes/*, ImportDataTwinCAT itemToBeExpanded*/)
        {
            TwincatPrototypeMembersInfo pUDTDefinition = null;
            if (mapUDT.ContainsKey(prototypeName) && !mapUDT[prototypeName].NoMembers())
            {
                pUDTDefinition = mapUDT[prototypeName];
            }
            else if (mapSTRUCT.ContainsKey(prototypeName) && !mapSTRUCT[prototypeName].NoMembers())
                pUDTDefinition = mapSTRUCT[prototypeName];
            else if (mapFunctionBlock.ContainsKey(prototypeName) && !mapFunctionBlock[prototypeName].NoMembers())
                pUDTDefinition = mapFunctionBlock[prototypeName];
            if (pUDTDefinition != null)
            {
                if (!pUDTDefinition.IsMembersParsed())
                    ParsePrototype(pUDTDefinition, parentTypes/*, itemToBeExpanded*/);
            }

            return (pUDTDefinition);
        }

        private static ImportPrototype addPrototype(Dictionary<string, ImportPrototype> protoMap, ImportData elem, ImportTagsEditorTree parent)
        {
            ImportPrototype proto = null;
            if (((ImportDataTwinCAT)elem).ElemType == null)
            {
                return (proto);
            }
            if (!protoMap.ContainsKey(((ImportDataTwinCAT)elem).ElemType))
            {
                if (elem.Children.Count != 0)
                {
                    proto = new ImportPrototype();
                    proto.Name = ((ImportDataTwinCAT)elem).ElemType;
                    proto.Elements = new List<ImportTag>();

                    foreach (var t in elem.Children)
                    {
                        ImportData el = t as ImportData;
                        if (el != null)
                        {
                            ImportTag a = new ImportTag();
                            if ((el.szType == "STRUCT") || parent.mapUDT.ContainsKey(el.szType))
                            {
                                ImportPrototype protoElem = addPrototype(protoMap, el, parent);
                                if (protoElem == null)
                                    continue;

                                a.ModelType = UFUAModel.ModelType.ObjectType;
                                a.PrototypeModel = protoElem.Name;
                            }
                            else
                            {
                                a.ModelType = ModelType.Variable;
                            }
                            a.ArrayDimension = el.ArrayDimension;
                            a.Name = el.Name;
                            a.DataType = ((ImportDataTwinCAT)el).Type;
                            a.Description = el.Description;
                            proto.Elements.Add(a);
                        }
                    }
                    protoMap.Add(((ImportDataTwinCAT)elem).ElemType, proto);
                }
            }
            else
                proto = protoMap[((ImportDataTwinCAT)elem).ElemType];
            return proto;
        }

        //private bool SearchMatchingPrototype(ImportPrototype candProto, ImportData currItem)
        //{
        //    return false;
        //    if (candProto.Elements.Count != currItem.Children.Count())
        //    {
        //        return false;
        //    }

        //    bool matchingPrototype = false;
        //    foreach (var m in candProto.Elements)
        //    {
        //        bool fieldFound = false;
        //        foreach (ImportData el in currItem.Children)
        //        {
        //            if (el != null)
        //            {
        //                fieldFound = m.Name == el.Name && m.DataType == ((ImportDataTwinCAT)el).Type;
        //            }
        //            else
        //            {
        //                return false;
        //            }
        //            if (fieldFound == true)
        //            {
        //                break;
        //            }
        //        }
        //        if (fieldFound == false)
        //        {
        //            matchingPrototype = false;
        //            break;
        //        }
        //        else
        //        {
        //            matchingPrototype = true;
        //        }
        //    }

        //    return matchingPrototype;
        //}

        static readonly ILog log = log4net.LogManager.GetLogger(Properties.Resources.AreaFileLog);
        private void PrintingOfImportErrors()
        {
            if (bErrorsOccurredDuringImport)
            {
                MessageBox.Show(Properties.Resources.ImportError);
            }
        }

        private ImportDataModel LoadFile(string file)
        {
            return (OneLevelLoadFile(file));
        }

        private ImportDataModel OneLevelLoadFile(string file)
        {
            string stationName = baseImportTree.ReadStationName();

            TwinCATVersion = (byte)TwinCAT.TwinCATVersions.Version2x;
            if (!TwinCATVersionList.TryGetValue(stationName, out TwinCATVersion))
            {
                TwinCATVersion = (byte)TwinCAT.TwinCATVersions.Version2x;
            }

            string fileTestTpy = file.ToLower();
            if (fileTestTpy.Contains(".tpy"))
            {
                //stationList.Clear();
                channelList.Clear();
                //TwinCATVersionList.Clear();
                mapSTRUCT.Clear();
                mapFunctionBlock.Clear();
                mapUDT.Clear();
                mapConstants.Clear();
                mapOK.Clear();
                mapTemp.Clear();
                bErrorsOccurredDuringImport = false;

                try
                {
                    using (new WaitCursor())
                    {
                        if (importDataModel != null)
                            importDataModel.Dispose();
                        importDataModel = new ImportDataModelTwinCAT(readStationName);

                        // Load the file
                        XDocument tpyDoc = XDocument.Load(file);
                        if (tpyDoc != null)
                        {
                            // Create the root element of the tree

                            ParseDataTypesTPY(ref tpyDoc);

                            PrepareDataTypeMapsTPY();

                            ParseSymbolsTPY(ref tpyDoc);
                        }

                        // Any variable parsed?
                    }
                }
                catch (Exception ex)
                {
                    //throw;
                    MessageBox.Show(DriverCodeBaseEx.UI.Properties.Resources.ImportErrorEmptyFile,
                                    Properties.Resources.ImportMsgBoxTitle);
                    return null;
                }
                if (importDataModel == null || importDataModel.Children.Count == 0)
                {
                    MessageBox.Show(DriverCodeBaseEx.UI.Properties.Resources.ImportErrorEmptyFile,
                                    Properties.Resources.ImportMsgBoxTitle);
                }
                //ImportTree.Model = importDataModel;

                //if (listViewSortCol != null)
                //{
                //    AdornerLayer.GetAdornerLayer(listViewSortCol).Remove(listViewSortAdorner);
                //    ImportTree.Items.SortDescriptions.Clear();
                //}

                PrintingOfImportErrors();
            }

            return importDataModel;
        }

        private void ParseDataTypesTPY(ref XDocument tpyDoc)
        {
            IEnumerable<XElement> dataTypes = from plcProjectInfoElement
                                              in tpyDoc.Element("PlcProjectInfo").Elements()
                                              where plcProjectInfoElement.Name == "DataTypes"
                                              select plcProjectInfoElement;

            foreach (XElement dataTypeInfo in dataTypes)
            {
                IEnumerable<XElement> dataTypeInfos = dataTypeInfo.Elements();
                foreach (XElement dataType in dataTypeInfos)
                {
                    ParseSingleDataTypeTPY(dataType);
                }
            }
        }

        private void PrepareDataTypeMapsTPY()
        {
            bool itemOK = true;
            foreach (KeyValuePair<string, TwincatPrototypeMembersInfo> kvp in mapSTRUCT)
            {
                if (kvp.Value.GetRawMembers().Length != 0)
                {
                    itemOK = true;
                    foreach (string structField in kvp.Value.GetRawMembers())
                    {
                        if (GetStructMemberTypeFromString(structField) == (int)ImportTypes.Unknown)
                            itemOK = false;
                    }
                    if (itemOK == true)
                    {
                        mapOK[kvp.Key] = kvp.Value;
                    }
                    else
                    {
                        mapTemp[kvp.Key] = kvp.Value;
                    }
                }
            }

            foreach (KeyValuePair<string, TwincatPrototypeMembersInfo> kvp in mapTemp)
            {
                // Count the number of strings that form this type definition
                int stringCount = 0;
                int i = 0;
                for (i = 0; i < kvp.Value.GetRawMembers().Length; i++)
                {
                    string item = kvp.Value.GetRawMember(i);
                    bool bType = true;
                    if (GetStructMemberTypeFromString(item) ==
                       (int)ImportTypes.Unknown)
                    {
                        bType = false;
                    }
                    bool bOK = mapOK.ContainsKey(item);
                    if ((bType == true) || (bOK == true))
                    {
                        if (bOK == true)
                        {
                            stringCount += mapOK[item].GetRawMembers().Length;
                        }
                        else
                        {
                            stringCount++;
                        }
                    }
                }

                string[] valDef;
                if (stringCount > 0)
                {
                    valDef = new string[stringCount];
                    int curStringIndex = 0;
                    for (i = 0; i < kvp.Value.GetRawMembers().Length; i++)
                    {
                        string item = kvp.Value.GetRawMember(i);
                        bool bType = true;
                        if (GetStructMemberTypeFromString(item) ==
                           (int)ImportTypes.Unknown)
                        {
                            bType = false;
                        }
                        bool bOK = mapOK.ContainsKey(item);
                        if ((bType == true) || (bOK == true))
                        {
                            if (bOK == true)
                            {
                                for (int j = 0; j < mapOK[item].GetRawMembers().Length; j++)
                                {
                                    valDef[curStringIndex++] = mapOK[item].GetRawMember(j);
                                }
                            }
                            else
                            {
                                valDef[curStringIndex++] = item;
                            }
                        }
                    }
                }
                else
                {
                    valDef = new string[1];
                    valDef[0] = String.Empty;
                }
                mapOK[kvp.Key] = new TwincatPrototypeMembersInfo(valDef);
            }

            mapSTRUCT.Clear();
            foreach (KeyValuePair<string, TwincatPrototypeMembersInfo> kvp in mapOK)
            {
                mapSTRUCT[kvp.Key] = kvp.Value;
            }
        }

        private void ParseSymbolsTPY(ref XDocument tpyDoc)
        {
            IEnumerable<XElement> symbols = from plcProjectInfoElement
                                              in tpyDoc.Element("PlcProjectInfo").Elements()
                                            where plcProjectInfoElement.Name == "Symbols"
                                            select plcProjectInfoElement;

            foreach (XElement symbolInfo in symbols)
            {
                IEnumerable<XElement> symbolInfos = symbolInfo.Elements();
                foreach (XElement symbol in symbolInfos)
                {
                    ParseSingleSymbolTPY(symbol);
                }
            }
        }

        private void ParseSingleSymbolTPY(XElement symbol)
        {
            XElement symbolName = symbol.Element("Name");
            if (symbolName == null)
            {
                return;
            }
            string symbolNameValue = symbolName.Value;
            if (String.IsNullOrEmpty(symbolNameValue))
            {
                return;
            }
            int pointPos = symbolNameValue.IndexOf('.');
            if (pointPos < 0)
            {
                return;
            }

            // Check the variables' type 
            int nVarType = (int)ImportVarType.Normal;
            string szDummy = (string)symbolName.Attribute("Constant");
            if ((szDummy) != null && (szDummy != String.Empty))
            {
                nVarType = (int)ImportVarType.Constant;
            }
            else
            {
                szDummy = String.Empty;
                szDummy = (string)symbolName.Attribute("Persistent");
                if ((szDummy) != null && (szDummy != String.Empty))
                {
                    nVarType = (int)ImportVarType.Persistent;
                }
                else
                {
                    szDummy = String.Empty;
                    szDummy = (string)symbolName.Attribute("szRetain");
                    if ((szDummy) != null && (szDummy != String.Empty))
                    {
                        nVarType = (int)ImportVarType.Retain;
                    }
                }
            }

            XElement symbolType = symbol.Element("Type");
            if (symbolType == null)
            {
                return;
            }
            string symbolTypeValue = symbolType.Value;
            if (symbolTypeValue == String.Empty)
            {
                return;
            }

            // Check Pointer type 
            string szPointer = (string)symbolType.Attribute("Pointer");
            if (szPointer != null)
            {
                UInt32 pointerValue = 0;
                try
                {
                    pointerValue = Convert.ToUInt32(szPointer, 16);
                }
                catch (FormatException)
                {
                    return;
                }
                catch (OverflowException)
                {
                    return;
                }
                catch (ArgumentException)
                {
                    return;
                }
                if (pointerValue != 0)
                    return;
            }

            byte category = (byte)ImportUDTType.Unknown;
            string szDecoration = (string)symbolType.Attribute("Decoration");
            if (szDecoration != null)
            {
                UInt32 decorationValue = 0;
                try
                {
                    decorationValue = Convert.ToUInt32(szDecoration, 16);
                }
                catch (FormatException)
                {
                    return;
                }
                catch (OverflowException)
                {
                    return;
                }
                catch (ArgumentException)
                {
                    return;
                }
                category = GetUdtTypeTPY(decorationValue);
            }

            int nVariableType = (int)ImportTypes.Unknown;
            uint nVarSize = 0;
            uint nArraySize0 = 0;
            uint nArraySize1 = 0;
            uint nArraySize2 = 0;
            uint nArrayStart0 = 0;
            uint nArrayStart1 = 0;
            uint nArrayStart2 = 0;
            int nMovType = (int)DataType.Boolean;
            switch (category)
            {
                case (byte)ImportUDTType.Unknown:
                case (byte)ImportUDTType.Alias:
                    if (GetVarTypeAndSize(ref symbolTypeValue, ref nVarSize) >= 0)
                    {
                        nVariableType = (int)ImportTypes.Standard;
                    }
                    break;

                case (byte)ImportUDTType.Enum:
                case (byte)ImportUDTType.Struct:
                    nVariableType = (int)ImportTypes.StructOrEnum;
                    break;

                case (byte)ImportUDTType.Array:
                    // Check the variable type and get the variable size
                    GetVarTypeAndSizeFileExp(ref symbolTypeValue, ref nVarSize,
                        ref nArraySize0, ref nArraySize1,
                        ref nArraySize2, ref nArrayStart0,
                        ref nArrayStart1,
                        ref nArrayStart2, ref nMovType);
                    nVariableType = (int)ImportTypes.Array;
                    break;
            }

            if (nVariableType == (int)ImportTypes.Unknown)
            {
                return;
            }

            string szAddressPrefix = symbolNameValue.Substring(0, pointPos);
            string szSingleVarName = symbolNameValue.Substring(pointPos + 1);
            string szNamePrefix = String.Empty;
            string description = null;
            XElement symbolComment = symbol.Element("Comment");
            if (symbolComment != null)
            {
                description = symbolComment.Value;
            }
            if (description == null)
            {
                description = String.Empty;
            }
            if (pointPos > 0)
            {
                szNamePrefix = szAddressPrefix + '_';
            }

            szAddressPrefix += '.';

            switch (nVariableType)
            {
                // Variable of standard type
                case (int)ImportTypes.Standard:
                    AddStandardVarList(szNamePrefix,
                                       szSingleVarName,
                                       szAddressPrefix,
                                       description,
                                       symbolTypeValue,
                                       nVarSize);
                    break;

                // Array
                case (int)ImportTypes.Array:
                    OneLevelAddArrayList(ref szNamePrefix,
                                         ref szSingleVarName,
                                         ref szAddressPrefix,
                                         ref description,
                                         ref symbolTypeValue,
                                         ref nVarSize,
                                         nArraySize0,
                                         nArraySize1,
                                         nArraySize2,
                                         nArrayStart0,
                                         nArrayStart1,
                                         nArrayStart2);
                    break;

                // Structure or enumeration
                case (int)ImportTypes.StructOrEnum:
                    OneLevelAddStructOrEnumList(ref szNamePrefix,
                                                ref szSingleVarName,
                                                ref szAddressPrefix,
                                                ref description,
                                                ref symbolTypeValue);
                    break;
            }

        }

        private void AddStandardVarList(string szNamePrefix,
                                        string szVarName,
                                        string szAddressPrefix,
                                        string description,
                                        string szVarType,
                                        uint nVarSize,
                                        int parentId = -1,
                                        ImportData inRootItem = null)
        {
            string szVarNameList = szVarName;
            char[] charsToTrim = { '\n', '\r', '\t', ' ', ',' };
            szVarNameList = szVarNameList.Trim(charsToTrim);
            string szSingleVarName = String.Empty;
            string szAux = String.Empty;
            bool bAllVariablesAdded = false;
            int nLastProcessedChar = 0;
            int nCommaIndex = 0;
            while (bAllVariablesAdded == false)
            {
                szAux = String.Empty;
                szAux = szVarNameList.Substring(nLastProcessedChar);
                nCommaIndex = szAux.IndexOf(',');
                if (nCommaIndex >= 0)
                {
                    if (szAux.Length > nCommaIndex)
                    {
                        nLastProcessedChar += nCommaIndex + 1;
                    }
                    else
                    {
                        nLastProcessedChar += szAux.Length;
                    }
                    szSingleVarName = String.Empty;
                    szSingleVarName = szAux.Substring(0, nCommaIndex);
                }
                else
                {
                    bAllVariablesAdded = true;
                    szSingleVarName = String.Empty;
                    szSingleVarName = szAux;
                    nLastProcessedChar += szAux.Length;
                }
                szSingleVarName = szSingleVarName.Trim(charsToTrim);
                if (szSingleVarName == String.Empty)
                {
                    bAllVariablesAdded = true;
                    continue;
                }

                AddStandardVar(szNamePrefix, szSingleVarName, szAddressPrefix + szVarName,
                    description, szVarType, nVarSize, -1, -1, parentId, inRootItem);
            }
        }

        private void AddArrayList(ref string szNamePrefix,
                                  ref string szArrayName,
                                  ref string szAddressPrefix,
                                  ref string szDescrPrefix,
                                  ref string szElemType,
                                  ref uint nElemSize,
                                  uint nArrayDim0,
                                  uint nArrayDim1,
                                  uint nArrayDim2,
                                  uint nArrayStart0,
                                  uint nArrayStart1,
                                  uint nArrayStart2,
                                  int parentId = -1,
                                  ImportData inRootItem = null)
        {
            if (nArrayDim0 == 0)
            {
                return;
            }

            string szSingleArrayName;
            string szArrayNameList = szArrayName;
            char[] charsToTrim = { '\n', '\r', '\t', ' ', ',' };
            szArrayNameList = szArrayNameList.Trim(charsToTrim);
            string szAux;
            bool bAllArraysAdded = false;
            int nLastProcessedChar = 0;
            int nCommaIndex = 0;
            while (!bAllArraysAdded)
            {
                szAux = String.Empty;
                szAux = szArrayNameList.Substring(nLastProcessedChar);
                nCommaIndex = szAux.IndexOf(',');
                if (nCommaIndex >= 0)
                {
                    if (szAux.Length > nCommaIndex)
                    {
                        nLastProcessedChar += nCommaIndex + 1;
                    }
                    else
                    {
                        nLastProcessedChar += szAux.Length;
                    }
                    szSingleArrayName = String.Empty;
                    szSingleArrayName = szAux.Substring(0, nCommaIndex);
                }
                else
                {
                    bAllArraysAdded = true;
                    szSingleArrayName = String.Empty;
                    szSingleArrayName = szAux;
                    nLastProcessedChar += szAux.Length;
                }
                szSingleArrayName = szSingleArrayName.Trim(charsToTrim);
                if (szSingleArrayName == String.Empty)
                {
                    bAllArraysAdded = true;
                    continue;
                }

                AddArray(szNamePrefix, szSingleArrayName, szAddressPrefix,
                    szDescrPrefix, ref szElemType, ref nElemSize, nArrayDim0, nArrayDim1,
                    nArrayDim2, nArrayStart0, nArrayStart1, nArrayStart2, parentId, inRootItem);
            }
        }

        private void OneLevelAddArrayList(ref string szNamePrefix,
                                          ref string szArrayName,
                                          ref string szAddressPrefix,
                                          ref string szDescrPrefix,
                                          ref string szElemType,
                                          ref uint nElemSize,
                                          uint nArrayDim0,
                                          uint nArrayDim1,
                                          uint nArrayDim2,
                                          uint nArrayStart0,
                                          uint nArrayStart1,
                                          uint nArrayStart2,
                                          int parentId = -1,
                                          ImportData inRootItem = null)
        {
            if (nArrayDim0 == 0)
            {
                return;
            }

            string szSingleArrayName;
            string szArrayNameList = szArrayName;
            char[] charsToTrim = { '\n', '\r', '\t', ' ', ',' };
            szArrayNameList = szArrayNameList.Trim(charsToTrim);
            string szAux;
            bool bAllArraysAdded = false;
            int nLastProcessedChar = 0;
            int nCommaIndex = 0;
            while (!bAllArraysAdded)
            {
                szAux = String.Empty;
                szAux = szArrayNameList.Substring(nLastProcessedChar);
                nCommaIndex = szAux.IndexOf(',');
                if (nCommaIndex >= 0)
                {
                    if (szAux.Length > nCommaIndex)
                    {
                        nLastProcessedChar += nCommaIndex + 1;
                    }
                    else
                    {
                        nLastProcessedChar += szAux.Length;
                    }
                    szSingleArrayName = String.Empty;
                    szSingleArrayName = szAux.Substring(0, nCommaIndex);
                }
                else
                {
                    bAllArraysAdded = true;
                    szSingleArrayName = String.Empty;
                    szSingleArrayName = szAux;
                    nLastProcessedChar += szAux.Length;
                }
                szSingleArrayName = szSingleArrayName.Trim(charsToTrim);
                if (szSingleArrayName == String.Empty)
                {
                    bAllArraysAdded = true;
                    continue;
                }

                OneLevelAddArray(szNamePrefix, szSingleArrayName, szAddressPrefix,
                                 szDescrPrefix, ref szElemType, ref nElemSize, nArrayDim0, nArrayDim1,
                                 nArrayDim2, nArrayStart0, nArrayStart1, nArrayStart2, parentId, inRootItem);
            }
        }

        private void OneLevelAddStructOrEnumList(ref string szNamePrefix,
                                                 ref string szVarName,
                                                 ref string szAddressPrefix,
                                                 ref string description,
                                                 ref string szVarType,
                                                 int parentId = -1,
                                                 ImportData inRootItem = null)
        {
            if ((!mapUDT.ContainsKey(szVarType) || mapUDT[szVarType].NoMembers())
                &&
                (!mapSTRUCT.ContainsKey(szVarType) || mapSTRUCT[szVarType].NoMembers())
                &&
                (!mapFunctionBlock.ContainsKey(szVarType) || mapFunctionBlock[szVarType].NoMembers())
                )
            {
                return;
            }

            string szSingleVarName;
            string szVarNameList = szVarName;
            char[] charsToTrim = { '\n', '\r', '\t', ' ', ',' };
            szVarNameList = szVarNameList.Trim(charsToTrim);
            string szAux;
            bool bAllVarAdded = false;
            int nLastProcessedChar = 0;
            int nCommaIndex = 0;
            while (!bAllVarAdded)
            {
                szAux = String.Empty;
                szAux = szVarNameList.Substring(nLastProcessedChar);
                nCommaIndex = szAux.IndexOf(',');
                if (nCommaIndex >= 0)
                {
                    if (szAux.Length > nCommaIndex)
                    {
                        nLastProcessedChar += nCommaIndex + 1;
                    }
                    else
                    {
                        nLastProcessedChar += szAux.Length;
                    }
                    szSingleVarName = String.Empty;
                    szSingleVarName = szAux.Substring(0, nCommaIndex);
                }
                else
                {
                    bAllVarAdded = true;
                    szSingleVarName = String.Empty;
                    szSingleVarName = szAux;
                    nLastProcessedChar += szAux.Length;
                }
                szSingleVarName = szSingleVarName.Trim(charsToTrim);
                if (szSingleVarName == String.Empty)
                {
                    bAllVarAdded = true;
                    continue;
                }

                OneLevelAddStructOrEnum(szNamePrefix, szSingleVarName, szAddressPrefix, szSingleVarName,
                                        description, szVarType, parentId, inRootItem);
            }
        }

        private void AddArray(string szNamePrefix,
                              string szArrayName,
                              string szAddressPrefix,
                              string szDescrPrefix,
                              ref string szElemType,
                              ref uint nElemSize,
                              uint nArrayDim0,
                              uint nArrayDim1,
                              uint nArrayDim2,
                              uint nArrayStart0,
                              uint nArrayStart1,
                              uint nArrayStart2,
                              int parentId = -1,
                              ImportData inRootItem = null)
        {
            if (nArrayDim0 == 0)
            {
                return;
            }

            string szName;
            string szName1;
            if (nArrayDim1 == 0)
            {
                AddArray1Dim(szNamePrefix, szArrayName, szAddressPrefix, szDescrPrefix,
                             ref szElemType, ref nElemSize, nArrayDim0, nArrayStart0, 0, parentId, inRootItem);
            }
            else if (nArrayDim2 == 0)
            {
                for (uint nIdx = nArrayStart0; nIdx < nArrayStart0 + nArrayDim0; nIdx++)
                {
                    szName = String.Format("{0}[{1}", szArrayName, nIdx);
                    AddArray1Dim(szNamePrefix, szName, szAddressPrefix, szDescrPrefix,
                                 ref szElemType, ref nElemSize, nArrayDim1, nArrayStart1, 1, parentId, inRootItem);
                }
            }
            else
            {
                for (uint nIdx2 = nArrayStart0; nIdx2 < nArrayStart0 + nArrayDim0; nIdx2++)
                {
                    for (uint nIdx = nArrayStart1; nIdx < nArrayDim1 + nArrayStart1;
                        nIdx++)
                    {
                        szName1 = String.Format("{0}[{1},{2}", szArrayName, nIdx2, nIdx);
                        AddArray1Dim(szNamePrefix, szName1, szAddressPrefix,
                                     szDescrPrefix, ref szElemType, ref nElemSize, nArrayDim2,
                                     nArrayStart2, 2, parentId, inRootItem);
                    }
                }
            }
        }

        private void OneLevelAddArray(string szNamePrefix,
                                      string szArrayName,
                                      string szAddressPrefix,
                                      string szDescrPrefix,
                                      ref string szElemType,
                                      ref uint nElemSize,
                                      uint nArrayDim0,
                                      uint nArrayDim1,
                                      uint nArrayDim2,
                                      uint nArrayStart0,
                                      uint nArrayStart1,
                                      uint nArrayStart2,
                                      int parentId = -1,
                                      ImportData inRootItem = null)
        {
            if (nArrayDim0 == 0)
            {
                return;
            }

            string szName;
            string szName1;
            if (nArrayDim1 == 0)
            {
                OneLevelAddArray1Dim(szNamePrefix, szArrayName, szAddressPrefix, szDescrPrefix,
                                     ref szElemType, ref nElemSize, nArrayDim0, nArrayStart0, 0, parentId, inRootItem);
            }
            else if (nArrayDim2 == 0)
            {
                for (uint nIdx = nArrayStart0; nIdx < nArrayStart0 + nArrayDim0; nIdx++)
                {
                    szName = String.Format("{0}[{1}", szArrayName, nIdx);
                    OneLevelAddArray1Dim(szNamePrefix, szName, szAddressPrefix, szDescrPrefix,
                                         ref szElemType, ref nElemSize, nArrayDim1, nArrayStart1, 1, parentId, inRootItem);
                }
            }
            else
            {
                for (uint nIdx2 = nArrayStart0; nIdx2 < nArrayStart0 + nArrayDim0; nIdx2++)
                {
                    for (uint nIdx = nArrayStart1; nIdx < nArrayDim1 + nArrayStart1;
                        nIdx++)
                    {
                        szName1 = String.Format("{0}[{1},{2}", szArrayName, nIdx2, nIdx);
                        OneLevelAddArray1Dim(szNamePrefix, szName1, szAddressPrefix,
                                             szDescrPrefix, ref szElemType, ref nElemSize, nArrayDim2,
                                             nArrayStart2, 2, parentId, inRootItem);
                    }
                }
            }
        }

        private void AddStandardVar(string szNamePrefix,
                                    string szVarName,
                                    string szAddress,
                                    string description,
                                    string szVarType,
                                    uint nVarSize,
                                    int nElemType,
                                    int nMoviconType,
                                    int parentId = -1,
                                    ImportData inRootItem = null)
        {
            ImportData v = importDataModel.addImportData();
            ((ImportDataTwinCAT)v).PreName = szNamePrefix;
            v.Name = szVarName;
            ((ImportDataTwinCAT)v).Size = nVarSize;
            v.szType = szVarType;
            uint nSize = 0;
            uint nArraySize0 = 0;
            uint nArraySize1 = 0;
            uint nArraySize2 = 0;
            uint nArrayStart0 = 0;
            uint nArrayStart1 = 0;
            uint nArrayStart2 = 0;
            int nMovType = (int)DataType.Boolean;

            int nGetType = GetVarTypeAndSizeFileExp(ref szVarType,
                                                    ref nSize,
                                                    ref nArraySize0,
                                                    ref nArraySize1,
                                                    ref nArraySize2,
                                                    ref nArrayStart0,
                                                    ref nArrayStart1,
                                                    ref nArrayStart2,
                                                    ref nMovType);

            ((ImportDataTwinCAT)v).Type = (DataType)(nMoviconType != -1 ? nMoviconType : nMovType);
            ((ImportDataTwinCAT)v).ElemType = szVarType;
            v.Address = szAddress;
            v.Description = description;
            v.parentId = parentId;
            v.Id = m_lGlobalID++;
            v.ArrayDimension = 0;
            ((ImportDataTwinCAT)v).ImportDataType = ImportTypes.Standard;
            AddTreeItem(v, inRootItem);
        }

        private void AddArray1Dim(string szNamePrefix,
                                  string szArrayName,
                                  string szAddressPrefix,
                                  string szDescrPrefix,
                                  ref string szElemType,
                                  ref uint nElemSize,
                                  uint nArrayDim,
                                  uint nArrayStart,
                                  uint nDimIndex,
                                  int parentId = -1,
                                  ImportData inRootItem = null)

        {
            if (nArrayDim == 0)
            {
                return;
            }

            // Element type?
            uint nElementSize = 0;
            int nElementType = GetVarTypeAndSize(ref szElemType, ref nElementSize);
            if (nElementType >= 0)
            {
                if ((nElementType == (int)DataType.String) && (nElemSize > 0))
                {
                    nElementSize = nElemSize;
                }

                //TreeViewItemAdv arrayNode;
                ImportData v = importDataModel.addImportData();
                ((ImportDataTwinCAT)v).PreName = szNamePrefix;
                string szAux;
                v.Name = szArrayName;
                if (nDimIndex > 0)
                {
                    v.Name += "]";
                }

                ((ImportDataTwinCAT)v).Size = nArrayDim * nElementSize;
                v.szType = "ARRAY OF " + szElemType;
                ((ImportDataTwinCAT)v).Type = (DataType)nElementType;
                ((ImportDataTwinCAT)v).ElemType = szElemType;
                ((ImportDataTwinCAT)v).ImportDataType = ImportTypes.Array;
                v.ArrayDimension = nArrayDim;

                szAux = String.Empty;
                string szAddress = szAddressPrefix + szArrayName;
                if (nDimIndex > 0)
                {
                    szAux = String.Format(",{0}]", nArrayStart);
                }
                szAddress += szAux;
                v.Address = szAddress;

                string szDescription = szDescrPrefix;
                v.Description = szDescription;

                ImportData RootArray;
                if (nElementType != (int)DataType.String)
                {
                    v.parentId = parentId;
                    v.Id = m_lGlobalID++;

                    // move multidimensional array to root of tree
                    if (nDimIndex > 1)
                        inRootItem = null;

                    AddTreeItem(v, inRootItem);
                    RootArray = v;
                }
                else
                {
                    // move multidimensional array to root of tree
                    if (nDimIndex > 1)
                        inRootItem = null;

                    v.Id = parentId;
                    RootArray = inRootItem;
                }

                string szElementName;
                string szFieldLine;
                uint i = 0;
                for (i = 0; i < nArrayDim; i++)
                {
                    ImportData f = importDataModel.addImportData();

                    szFieldLine = String.Empty;
                    szAux = String.Empty;
                    szElementName = String.Empty;
                    szFieldLine = szArrayName;
                    szElementName = szArrayName;

                    if (nDimIndex == 0)
                    {
                        szAux = String.Format("[{0}]", i + nArrayStart);
                    }
                    else
                    {
                        szAux = String.Format(",{0}]", i + nArrayStart);
                    }
                    szElementName += szAux;
                    szFieldLine += szAux;

                    ((ImportDataTwinCAT)f).PreName = szNamePrefix;
                    ((ImportDataTwinCAT)f).Name = szElementName;
                    ((ImportDataTwinCAT)f).Size = nElementSize;
                    f.szType = szElemType;
                    ((ImportDataTwinCAT)f).Type = (DataType)nElementType;
                    f.Address = szAddressPrefix + szFieldLine;
                    f.Description = szDescrPrefix;
                    f.parentId = v.Id;
                    f.Id = m_lGlobalID++;
                    f.ArrayDimension = 0;
                    ((ImportDataTwinCAT)f).ImportDataType = ImportTypes.Standard;
                    AddTreeItem(f, RootArray);
                }
            }
            else
            {
                string szFieldLine;
                string szAux;
                uint i = 0;
                for (i = 0; i < nArrayDim; i++)
                {
                    szFieldLine = String.Empty;
                    szAux = String.Empty;
                    szFieldLine = szArrayName;
                    if (nDimIndex == 0)
                    {
                        szAux = String.Format("[{0}]", i + nArrayStart);
                    }
                    else
                    {
                        szAux = String.Format(",{0}]", i + nArrayStart);
                    }
                    szFieldLine += szAux;
                    AddStructOrEnum(szNamePrefix, szFieldLine, szAddressPrefix, szFieldLine,
                                    szDescrPrefix, szElemType);
                }
            }
        }

        private void OneLevelAddArray1Dim(string szNamePrefix,
                                          string szArrayName,
                                          string szAddressPrefix,
                                          string szDescrPrefix,
                                          ref string szElemType,
                                          ref uint nElemSize,
                                          uint nArrayDim,
                                          uint nArrayStart,
                                          uint nDimIndex,
                                          int parentId = -1,
                                          ImportData inRootItem = null)

        {
            if (nArrayDim == 0)
            {
                return;
            }

            // Element type?
            uint nElementSize = 0;
            int nElementType = GetVarTypeAndSize(ref szElemType, ref nElementSize);
            if (nElementType >= 0)
            {
                if ((nElementType == (int)DataType.String) && (nElemSize > 0))
                {
                    nElementSize = nElemSize;
                }

                ImportDataTwinCAT v = (ImportDataTwinCAT)importDataModel.addImportData();
                v.PreName = szNamePrefix;
                string szAux;
                v.Name = szArrayName;
                if (nDimIndex > 0)
                {
                    v.Name += "]";
                }

                v.Size = nArrayDim * nElementSize;
                v.szType = "ARRAY OF " + szElemType;
                v.Type = (DataType)nElementType;
                v.ElemType = szElemType;
                v.ImportDataType = ImportTypes.Array;
                v.ArrayDimension = nArrayDim;

                szAux = String.Empty;
                string szAddress = szAddressPrefix + szArrayName;
                if (nDimIndex > 0)
                {
                    szAux = String.Format(",{0}]", nArrayStart);
                }
                szAddress += szAux;
                v.Address = szAddress;

                string szDescription = szDescrPrefix;
                v.Description = szDescription;

                ImportData RootArray;
                v.parentId = parentId;
                v.Id = m_lGlobalID++;
                v.ImportDataType = ImportTypes.Array;
                AddTreeItem(v, inRootItem);
                RootArray = v;

                string szElementName;
                string szFieldLine;
                uint i = 0;
                for (i = 0; i < nArrayDim; i++)
                {
                    ImportDataTwinCAT f = (ImportDataTwinCAT)importDataModel.addImportData();

                    szFieldLine = String.Empty;
                    szAux = String.Empty;
                    szElementName = String.Empty;
                    szFieldLine = szArrayName;
                    szElementName = szArrayName;

                    if (nDimIndex == 0)
                    {
                        szAux = String.Format("[{0}]", i + nArrayStart);
                    }
                    else
                    {
                        szAux = String.Format(",{0}]", i + nArrayStart);
                    }
                    szElementName += szAux;
                    szFieldLine += szAux;

                    f.PreName = szNamePrefix;
                    f.Name = szElementName;
                    f.Size = nElementSize;
                    f.szType = szElemType;
                    f.Type = (DataType)nElementType;
                    f.Address = szAddressPrefix + szFieldLine;
                    f.Description = szDescrPrefix;
                    f.parentId = v.Id;
                    f.Id = m_lGlobalID++;
                    f.ArrayDimension = 0;
                    f.ImportDataType = ImportTypes.Standard;
                    AddTreeItem(f, RootArray);
                }
            }
            else
            {
                string szFieldLine;
                string szAux;
                uint i = 0;
                for (i = 0; i < nArrayDim; i++)
                {
                    szFieldLine = String.Empty;
                    szAux = String.Empty;
                    szFieldLine = szArrayName;
                    if (nDimIndex == 0)
                    {
                        szAux = String.Format("[{0}]", i + nArrayStart);
                    }
                    else
                    {
                        szAux = String.Format(",{0}]", i + nArrayStart);
                    }
                    szFieldLine += szAux;
                    OneLevelAddStructOrEnum(szNamePrefix, szFieldLine, szAddressPrefix, szFieldLine,
                                            szDescrPrefix, szElemType, parentId, inRootItem);
                }
            }
        }

        private bool AddStructOrEnum(string szNamePrefix,
                                     string szVarName,
                                     string szAddressPrefix,
                                     string szAddress,
                                     string description,
                                     string szVarType,
                                     int parentId = -1,
                                     ImportData inRootItem = null)
        {
            TwincatPrototypeMembersInfo pUDTDefinition = null;
            bool bUDT = false;
            bool bSTRUCT = false;
            bool bFB = false;
            if (!mapUDT.ContainsKey(szVarType) || mapUDT[szVarType].NoMembers())
            {
                if (!mapSTRUCT.ContainsKey(szVarType) || mapSTRUCT[szVarType].NoMembers())
                {
                    if (!mapFunctionBlock.ContainsKey(szVarType) || mapFunctionBlock[szVarType].NoMembers())
                    {
                        return false;
                    }
                    else
                    {
                        pUDTDefinition = mapFunctionBlock[szVarType];
                        bFB = true;
                    }
                }
                else
                {
                    pUDTDefinition = mapSTRUCT[szVarType];
                    bSTRUCT = true;
                }
            }
            else
            {
                pUDTDefinition = mapUDT[szVarType];
                bUDT = true;
            }

            string szStructNamePrefix = szNamePrefix + szVarName + ".";
            string szStructAddressPrefix = szAddressPrefix + szVarName + ".";
            string szFbNamePrefix = szNamePrefix + szVarName + "_";
            string szParentAddress = szAddressPrefix + szVarName;
            bool bAddStructTemplateToMap = false;
            string[] pTemplateArray = new string[1];
            int templateArraySize = 0;
            int nTotSize = 0;
            string szFieldName = string.Empty;
            string szFieldLine = string.Empty;
            string szFieldType = string.Empty;
            int nIndex = 0;
            uint nFieldSize = 0;
            uint nArraySize0 = 0;
            uint nArraySize1 = 0;
            uint nArraySize2 = 0;
            uint nArrayStart0 = 0;
            uint nArrayStart1 = 0;
            uint nArrayStart2 = 0;
            int nFieldType = 0;
            int nStringArrayDim = pUDTDefinition.GetRawMembers().Length;
            int i = 0;

            int lParentID = parentId;
            if (inRootItem == null)
                lParentID = -1;

            bool isFirst = true;
            ImportData RootStruct = inRootItem;
            ImportData rootItem = importDataModel.addImportData();
            rootItem.parentId = lParentID;
            ((ImportDataTwinCAT)rootItem).PreName = szNamePrefix;
            rootItem.Name = szVarName;
            rootItem.szType = szVarType;
            rootItem.parentId = parentId;
            rootItem.Id = m_lGlobalID++;
            ((ImportDataTwinCAT)rootItem).Type = DataType.Boolean;
            rootItem.Address = szParentAddress;
            rootItem.Description = description;
            ((ImportDataTwinCAT)rootItem).Size = (uint)nTotSize;
            rootItem.ArrayDimension = 0;

            AddStructRoot(szVarType, inRootItem, pTemplateArray, "", ref isFirst, rootItem, ref RootStruct);
            for (i = 0; i < nStringArrayDim; i++)
            {
                ImportData f = importDataModel.addImportData();
                szFieldLine = String.Empty;
                szFieldLine = pUDTDefinition.GetRawMember(i);
                nIndex = szFieldLine.IndexOf(':');
                if (nIndex <= 0)
                {
                    continue;
                }
                szFieldName = String.Empty;
                szFieldName = szFieldLine.Substring(0, nIndex);
                szFieldName = szFieldName.Trim();
                if (szFieldName == String.Empty)
                {
                    continue;
                }
                if (szFieldLine.Length < (nIndex + 2))
                {
                    continue;
                }
                szFieldType = String.Empty;
                szFieldType = szFieldLine.Substring(nIndex + 1);
                szFieldType = szFieldType.Trim();
                if (szFieldType == String.Empty)
                {
                    continue;
                }


                nFieldSize = 0;
                nArraySize0 = 0;
                nArraySize1 = 0;
                nArraySize2 = 0;
                nArrayStart0 = 0;
                nArrayStart1 = 0;
                nArrayStart2 = 0;
                int nMovType = (int)DataType.Boolean;

                // Field type?
                nFieldType = GetVarTypeAndSizeFileExp(ref szFieldType,
                                                      ref nFieldSize,
                                                      ref nArraySize0,
                                                      ref nArraySize1,
                                                      ref nArraySize2,
                                                      ref nArrayStart0,
                                                      ref nArrayStart1,
                                                      ref nArrayStart2,
                                                      ref nMovType);

                String elemDescription = description;
                if (elemDescription == null)
                {
                    elemDescription = String.Empty;
                }

                // Add the field to the dialog list
                switch (nFieldType)
                {
                    // Variable of standard type
                    case (int)ImportTypes.Standard:
                        if (String.Compare(szFieldName, ImportEnumAlias, true) == 0)
                        {
                            AddStandardVar("",
                                            szParentAddress,
                                            szParentAddress,
                                            elemDescription,
                                            szFieldType,
                                            nFieldSize,
                                            -1, -1
                                            );
                        }
                        else if (nMovType == (int)DataType.String)
                        {
                            AddStandardVar("",
                                            szStructNamePrefix + szFieldName,
                                            szParentAddress + "." + szFieldName,
                                            elemDescription,
                                            szFieldType,
                                            nFieldSize,
                                            -1, -1
                                            );
                        }
                        else
                        {
                            string szNameTmp;
                            if (bFB && szFieldName[0] == '.')
                            {
                                szNameTmp = szFieldName.Substring(1);
                            }
                            else
                            {
                                szNameTmp = szFieldName;
                            }
                            ImportData lRootItem = inRootItem;
                            bAddStructTemplateToMap = true;
                            lParentID = rootItem.parentId;
                            lRootItem = RootStruct;
                            if (String.Compare(szFieldName,
                                    "___Movicon_Import_Alias", true) == 0)
                            {
                                AddStandardVar(szStructNamePrefix,
                                                szNameTmp,
                                                szParentAddress + "." + szNameTmp,
                                                elemDescription,
                                                szFieldType,
                                                nFieldSize,
                                                -1, -1,
                                                lParentID,
                                                lRootItem
                                                );
                            }
                            else
                            {
                                ((ImportDataTwinCAT)f).PreName = szStructNamePrefix;
                                f.Name = szNameTmp;
                                ((ImportDataTwinCAT)f).Size = nFieldSize;
                                f.szType = szFieldType;
                                ((ImportDataTwinCAT)f).Type = (DataType)nMovType;
                                f.Address = szParentAddress + "." + szNameTmp;
                                f.Description = elemDescription;
                                f.parentId = lParentID;
                                f.Id = m_lGlobalID++;
                                f.ArrayDimension = 0;
                                AddTreeItem(f, lRootItem);
                            }

                        }
                        break;

                    // Array
                    case (int)ImportTypes.Array:

                        uint nElementSize = 0;
                        int nElementType = GetVarTypeAndSize(ref szFieldType, ref nElementSize);

                        if (String.Compare(szFieldName,
                            "___Movicon_Import_Array", true) == 0)
                        {
                            AddArray(szNamePrefix,
                                     szVarName,
                                     szAddressPrefix,
                                     elemDescription,
                                     ref szFieldType,
                                     ref nFieldSize,
                                     nArraySize0,
                                     nArraySize1,
                                     nArraySize2,
                                     nArrayStart0,
                                     nArrayStart1,
                                     nArrayStart2);
                        }
                        else
                        {
                            ImportData lRootItem = null;
                            string namePrefix = szFbNamePrefix;
                            if (nElementType >= 0 && nElementType != (int)UFUAModel.DataType.String)
                            {
                                namePrefix = szStructNamePrefix;
                                lParentID = parentId;
                                lRootItem = inRootItem;
                                szStructNamePrefix = szVarName + ".";
                                bAddStructTemplateToMap = true;
                                lParentID = rootItem.parentId;
                                lRootItem = RootStruct;
                            }
                            AddArray(namePrefix,
                                     szFieldName,
                                     szStructAddressPrefix,
                                     elemDescription,
                                     ref szFieldType,
                                     ref nFieldSize,
                                     nArraySize0,
                                     nArraySize1,
                                     nArraySize2,
                                     nArrayStart0,
                                     nArrayStart1,
                                     nArrayStart2,
                                     lParentID,
                                     lRootItem);
                        }
                        break;

                    // Structure or enumeration
                    case (int)ImportTypes.StructOrEnum:
                        if (AddStructOrEnum(szStructNamePrefix,
                                        szFieldName,
                                        szStructAddressPrefix,
                                        szFieldName,
                                        elemDescription,
                                        szFieldType, lParentID, rootItem))
                            bAddStructTemplateToMap = true;
                        break;
                }
            }

            // If needed add the template to the structure template map
            if (!bAddStructTemplateToMap)
            {
                if (inRootItem != null)
                    inRootItem.Children.Remove(rootItem);
                else
                    importDataModel.Children.Remove(rootItem);
            }
            return bAddStructTemplateToMap;

        }

        private bool OneLevelAddStructOrEnum(string szNamePrefix,
                                             string szVarName,
                                             string szAddressPrefix,
                                             string szAddress,
                                             string description,
                                             string szVarType,
                                             int parentId = -1,
                                             ImportData inRootItem = null)
        {
            string szParentAddress = szAddressPrefix + szVarName;
            bool bAddStructTemplateToMap = false;
            int nTotSize = 0;

            int lParentID = parentId;
            if (inRootItem == null)
                lParentID = -1;

            ImportDataTwinCAT rootItem = (ImportDataTwinCAT)importDataModel.addImportData();
            rootItem.parentId = lParentID;
            rootItem.PreName = szNamePrefix;
            rootItem.Name = szVarName;
            rootItem.parentId = parentId;
            rootItem.Id = m_lGlobalID++;
            rootItem.Address = szParentAddress;
            rootItem.Description = description;
            rootItem.ArrayDimension = 0;
            string trueTypeName = String.Empty;
            int movType = (int)UFUAModel.DataType.Boolean;
            uint dataSize = 0;

            if (!TypeIsEnumOrStandardTypeAlias(szVarType, ref trueTypeName, ref movType, ref dataSize))
            {
                rootItem.szType = szVarType;
                rootItem.Size = (uint)nTotSize;
                rootItem.Type = DataType.Boolean;
                rootItem.ImportDataType = ImportTypes.StructOrEnum;
                OneLevelAddStructRoot(szVarType, inRootItem, rootItem);
            }
            else
            {
                rootItem.szType = trueTypeName;
                ((ImportDataTwinCAT)rootItem).Size = (uint)dataSize;
                ((ImportDataTwinCAT)rootItem).Type = (DataType)movType;
                ((ImportDataTwinCAT)rootItem).ImportDataType = ImportTypes.Standard;
                AddTreeItem(rootItem, inRootItem);
            }

            return bAddStructTemplateToMap;
        }

        private bool AddStructRoot(string szVarType, ImportData inRootItem, string[] pTemplateArray, string szFieldName, ref bool isFirst, ImportData rootItem, ref ImportData RootStruct)
        {
            if (szFieldName != "___Movicon_Import_Alias")
            {
                if (isFirst)
                {
                    isFirst = false;
                    AddTreeItem(rootItem, inRootItem);
                    RootStruct = rootItem;
                    rootItem.Id = m_lGlobalID++;
                    //FOGBUGZ 18268
                    //rootItem.ElemType = UFUAModel.Helpers.NameValidator.EnsureValidName(szVarType);
                    ((ImportDataTwinCAT)rootItem).ElemType = szVarType;
                    rootItem.szType = "STRUCT";
                }
                return true;
            }
            return false;
        }

        private bool OneLevelAddStructRoot(string szVarType, ImportData inRootItem, ImportData rootItem)
        {
            AddTreeItem(rootItem, inRootItem);
            rootItem.Id = m_lGlobalID++;
            ((ImportDataTwinCAT)rootItem).ElemType = szVarType;
            rootItem.szType = "STRUCT";

            // Add a dummy child
            rootItem.Children.Add(BaseImportTree.DummySubItem);
            return true;
        }

        bool GetUDTDefinition(ref string[] pUDTDefinition, string szVarType)
        {
            bool bUDT = false;
            bool bSTRUCT = false;
            bool bFB = false;
            if (!mapUDT.ContainsKey(szVarType) || mapUDT[szVarType].NoMembers())
            {
                if (!mapSTRUCT.ContainsKey(szVarType) || mapSTRUCT[szVarType].NoMembers())
                {
                    if (!mapFunctionBlock.ContainsKey(szVarType) || mapFunctionBlock[szVarType].NoMembers())
                    {
                        return false;
                    }
                    else
                    {
                        pUDTDefinition = mapFunctionBlock[szVarType].GetRawMembers();
                        bFB = true;
                    }
                }
                else
                {
                    pUDTDefinition = mapSTRUCT[szVarType].GetRawMembers();
                    bSTRUCT = true;
                }
            }
            else
            {
                pUDTDefinition = mapUDT[szVarType].GetRawMembers();
                bUDT = true;
            }

            return (bUDT || bSTRUCT || bFB);
        }

        private void SubstituteDummyChildWithTrueChildren(ObservableCollection<ImportData> children, ImportData root)
        {
            if (!BaseImportTree.IsDummyChild(children))
            {
                return;
            }
            children.Clear();
            ImportDataTwinCAT twincatRoot = root as ImportDataTwinCAT;
            string[] udtDefinition = new string[0];
            if (!GetUDTDefinition(ref udtDefinition, twincatRoot.ElemType))
            {
                return;
            }
            if (udtDefinition.Count() == 0)
            {
                return;
            }
            foreach (var field in udtDefinition)
            {
                // Each field de finition has the following definition:
                // <Field Name> : <Field Type>
                int index = field.IndexOf(':');
                if (index <= 1)
                {
                    continue;
                }
                string fieldName = field.Substring(0, index);
                fieldName = fieldName.Trim();
                if (String.IsNullOrWhiteSpace(fieldName) || (field.Length < (index + 2)))
                {
                    continue;
                }
                string fieldTypeName = field.Substring(index + 1);
                if (String.IsNullOrWhiteSpace(fieldTypeName))
                {
                    continue;
                }
                fieldTypeName = fieldTypeName.Trim();
                uint fieldSize = 0;
                uint arraySize0 = 0;
                uint arraySize1 = 0;
                uint arraySize2 = 0;
                uint arrayStart0 = 0;
                uint arrayStart1 = 0;
                uint arrayStart2 = 0;
                int fieldType = 0;
                int movType = (int)DataType.Boolean;
                // Field type?
                fieldType = GetVarTypeAndSizeFileExp(ref fieldTypeName,
                                                     ref fieldSize,
                                                     ref arraySize0,
                                                     ref arraySize1,
                                                     ref arraySize2,
                                                     ref arrayStart0,
                                                     ref arrayStart1,
                                                     ref arrayStart2,
                                                     ref movType);
                switch (fieldType)
                {
                    // Variable of standard type
                    case (int)ImportTypes.Standard:
                        AddStandardVar(twincatRoot.ImportTagName + ".",
                                       fieldName,
                                       twincatRoot.Address + "." + fieldName,
                                       String.Empty,
                                       fieldTypeName,
                                       fieldSize,
                                       -1,
                                       movType,
                                       root.Id,
                                       root);
                        break;
                    case (int)ImportTypes.Array:
                        OneLevelAddArray(twincatRoot.ImportTagName + ".",
                                         fieldName,
                                         twincatRoot.Address + ".",
                                         String.Empty,
                                         ref fieldTypeName,
                                         ref fieldSize,
                                         arraySize0,
                                         arraySize1,
                                         arraySize2,
                                         arrayStart0,
                                         arrayStart1,
                                         arrayStart2,
                                         root.Id,
                                         root);
                        break;
                    case (int)ImportTypes.StructOrEnum:
                        OneLevelAddStructOrEnum(twincatRoot.ImportTagName + ".",
                                                fieldName,
                                                twincatRoot.Address + ".",
                                                fieldName,
                                                String.Empty,
                                                fieldTypeName,
                                                root.Id,
                                                root);
                        break;
                }
            }
        }

        private int GetStructMemberTypeFromString(string structField)
        {
            int Index = structField.IndexOf(':');
            if (Index <= 0)
            {
                return (int)ImportTypes.Unknown;
            }

            string FieldName = structField.Substring(0, Index);
            FieldName = FieldName.Trim();
            if (FieldName == String.Empty)
            {
                return (int)ImportTypes.Unknown;
            }

            if (structField.Length < (Index + 2))
            {
                return (int)ImportTypes.Unknown;
            }
            string FieldType = structField.Substring(Index + 1);
            FieldType = FieldType.Trim();
            if (FieldType == String.Empty)
            {
                return (int)ImportTypes.Unknown;
            }

            UInt32 FieldSize = 0;
            UInt32 ArraySize0 = 0;
            UInt32 ArraySize1 = 0;
            UInt32 ArraySize2 = 0;
            UInt32 ArrayStart0 = 0;
            UInt32 ArrayStart1 = 0;
            UInt32 ArrayStart2 = 0;
            int MovType = 0;
            // Field type?
            int numFieldType = GetVarTypeAndSizeFileExp(ref FieldType, ref FieldSize,
                ref ArraySize0, ref ArraySize1, ref ArraySize2, ref ArrayStart0,
                ref ArrayStart1, ref ArrayStart2, ref MovType);

            return numFieldType;
        }

        private int GetVarTypeAndSizeFileExp(ref string FieldType, ref UInt32 StandardSize,
                ref UInt32 ArraySize0, ref UInt32 ArraySize1, ref UInt32 ArraySize2, ref UInt32 ArrayStart0,
                ref UInt32 ArrayStart1, ref UInt32 ArrayStart2, ref int MovType)
        {
            int returnValue = (int)ImportTypes.Unknown;
            MovType = (int)DataType.Boolean;
            StandardSize = 0;
            ArraySize0 = 0;
            ArraySize1 = 0;
            ArraySize2 = 0;
            ArrayStart0 = 0;
            ArrayStart1 = 0;
            ArrayStart2 = 0;
            FieldType = FieldType.Trim();
            if (FieldType == String.Empty)
            {
                return returnValue;
            }

            // Standard type?
            MovType = GetVarTypeAndSize(ref FieldType, ref StandardSize);
            if (MovType >= 0)
            {
                return (int)ImportTypes.Standard;
            }

            // Array?
            int searchIndex1 = FieldType.IndexOf("ARRAY");
            if (searchIndex1 >= 0)
            {
                if (FieldType.Length < (searchIndex1 + 6))
                {
                    return returnValue;
                }

                string szAux = FieldType.Substring(searchIndex1 + 5);
                szAux = szAux.Trim();
                searchIndex1 = szAux.IndexOf('[');
                if (searchIndex1 < 0)
                {
                    return returnValue;
                }

                int searchIndex2 = szAux.IndexOf("..");
                if (searchIndex2 < searchIndex1)
                {
                    return returnValue;
                }
                string szAux2 = szAux.Substring(searchIndex1 + 1,
                                    searchIndex2 - searchIndex1 - 1);
                int nStart0 = 0;
                if (Int32.TryParse(szAux2, out nStart0) == false)
                {
                    if (mapConstants.ContainsKey(szAux2) == false)
                    {
                        return returnValue;
                    }
                    nStart0 = mapConstants[szAux2];
                }
                if (nStart0 < 0)
                {
                    return returnValue;
                }

                int nStart1 = 0;
                int nStart2 = 0;
                int nEnd0 = 0;
                int nEnd1 = 0;
                int nEnd2 = 0;
                int searchIndex3 = szAux.IndexOf(',');

                // Unidimensional array?
                if (searchIndex3 < searchIndex2)
                {
                    searchIndex3 = szAux.IndexOf(']');
                    if (searchIndex3 < searchIndex2)
                    {
                        return returnValue;
                    }
                    szAux2 = String.Empty;
                    szAux2 = szAux.Substring(searchIndex2 + 2,
                                        searchIndex3 - searchIndex2 - 2);
                    if (Int32.TryParse(szAux2, out nEnd0) == false)
                    {
                        if (mapConstants.ContainsKey(szAux2) == false)
                        {
                            return returnValue;
                        }
                        nEnd0 = mapConstants[szAux2];
                    }
                    if (nEnd0 < nStart0)
                    {
                        return returnValue;
                    }

                    searchIndex1 = szAux.IndexOf("OF");
                    if (searchIndex1 < searchIndex3)
                    {
                        return returnValue;
                    }
                    if (szAux.Length < (searchIndex1 + 3))
                    {
                        return returnValue;
                    }
                    szAux2 = String.Empty;
                    szAux2 = szAux.Substring(searchIndex1 + 2);
                    szAux2 = szAux2.Trim();
                    if (GetVarTypeAndSize(ref szAux2, ref StandardSize) < 0)
                    {
                        if (!mapUDT.ContainsKey(szAux2) || mapUDT[szAux2].NoMembers())
                        {
                            if (!mapSTRUCT.ContainsKey(szAux2) || mapSTRUCT[szAux2].NoMembers())
                            {
                                if (!mapFunctionBlock.ContainsKey(szAux2) || mapFunctionBlock[szAux2].NoMembers())
                                {
                                    return returnValue;
                                }
                            }
                        }
                    }

                    ArrayStart0 = (UInt32)nStart0;
                    ArraySize0 = (UInt32)(nEnd0 - nStart0 + 1);
                    FieldType = String.Empty;
                    FieldType = szAux2;
                    returnValue = (int)ImportTypes.Array;
                }

                // Multidimensional array
                else
                {
                    szAux2 = String.Empty;
                    szAux2 = szAux.Substring(searchIndex2 + 2,
                        searchIndex3 - searchIndex2 - 2);
                    if (Int32.TryParse(szAux2, out nEnd0) == false)
                    {
                        if (mapConstants.ContainsKey(szAux2) == false)
                        {
                            return returnValue;
                        }
                        nEnd0 = mapConstants[szAux2];
                    }
                    if (nEnd0 <= nStart0)
                    {
                        return returnValue;
                    }
                    if (szAux.Length < (searchIndex3 + 11))
                    {
                        return returnValue;
                    }

                    szAux2 = String.Empty;
                    szAux2 = szAux.Substring(searchIndex3 + 1);
                    szAux2 = szAux2.Trim();
                    szAux = String.Empty;
                    szAux = szAux2;
                    searchIndex2 = szAux.IndexOf("..");
                    if (searchIndex2 < 1)
                    {
                        return (returnValue);
                    }
                    szAux2 = String.Empty;
                    szAux2 = szAux.Substring(0, searchIndex2);
                    if (Int32.TryParse(szAux2, out nStart1) == false)
                    {
                        if (mapConstants.ContainsKey(szAux2) == false)
                        {
                            return returnValue;
                        }
                        nStart1 = mapConstants[szAux2];
                    }
                    if (nStart1 < 0)
                    {
                        return returnValue;
                    }
                    searchIndex3 = szAux.IndexOf(",");

                    // Bi-dimensional array?
                    if (searchIndex3 < searchIndex2)
                    {
                        searchIndex3 = szAux.IndexOf(']');
                        if (searchIndex3 < searchIndex2)
                        {
                            return returnValue;
                        }
                        szAux2 = String.Empty;
                        szAux2 = szAux.Substring(searchIndex2 + 2,
                            searchIndex3 - searchIndex2 - 2);
                        if (Int32.TryParse(szAux2, out nEnd1) == false)
                        {
                            if (mapConstants.ContainsKey(szAux2) == false)
                            {
                                return returnValue;
                            }
                            nEnd1 = mapConstants[szAux2];
                        }
                        if (nEnd1 < nStart1)
                        {
                            return returnValue;
                        }

                        searchIndex1 = szAux.IndexOf("OF");
                        if (searchIndex1 < searchIndex3)
                        {
                            return returnValue;
                        }
                        if (szAux.Length < (searchIndex1 + 3))
                        {
                            return returnValue;
                        }
                        szAux2 = String.Empty;
                        szAux2 = szAux.Substring(searchIndex1 + 2);
                        szAux2 = szAux2.Trim();
                        if (GetVarTypeAndSize(ref szAux2, ref StandardSize) < 0)
                        {
                            if (!mapUDT.ContainsKey(szAux2) || mapUDT[szAux2].NoMembers())
                            {
                                if (!mapSTRUCT.ContainsKey(szAux2) || mapSTRUCT[szAux2].NoMembers())
                                {
                                    if (!mapFunctionBlock.ContainsKey(szAux2) || mapFunctionBlock[szAux2].NoMembers())
                                    {
                                        return returnValue;
                                    }
                                }
                            }
                        }

                        ArrayStart0 = (UInt32)nStart0;
                        ArraySize0 = (UInt32)(nEnd0 - nStart0 + 1);
                        ArrayStart1 = (UInt32)nStart1;
                        ArraySize1 = (UInt32)(nEnd1 - nStart1 + 1);
                        FieldType = String.Empty;
                        FieldType = szAux2;
                        returnValue = (int)ImportTypes.Array;
                    }

                    // 3-dimensional array
                    else
                    {
                        szAux2 = String.Empty;
                        szAux2 = szAux.Substring(searchIndex2 + 2,
                            searchIndex3 - searchIndex2 - 2);
                        if (Int32.TryParse(szAux2, out nEnd1) == false)
                        {
                            if (mapConstants.ContainsKey(szAux2) == false)
                            {
                                return returnValue;
                            }
                            nEnd1 = mapConstants[szAux2];
                        }
                        if (nEnd1 <= nStart1)
                        {
                            return returnValue;
                        }
                        if (szAux.Length < (searchIndex3 + 11))
                        {
                            return returnValue;
                        }
                        szAux2 = String.Empty;
                        szAux2 = szAux.Substring(searchIndex3 + 1);
                        szAux2.Trim();
                        szAux = String.Empty;
                        szAux = szAux2;
                        searchIndex2 = szAux.IndexOf("..");
                        if (searchIndex2 < 1)
                        {
                            return returnValue;
                        }
                        szAux2 = String.Empty;
                        szAux2 = szAux.Substring(0, searchIndex2);
                        if (Int32.TryParse(szAux2, out nStart2) == false)
                        {
                            if (mapConstants.ContainsKey(szAux2) == false)
                            {
                                return returnValue;
                            }
                            nStart2 = mapConstants[szAux2];
                        }
                        if (nStart2 < 0)
                        {
                            return returnValue;
                        }
                        searchIndex3 = szAux.IndexOf(']');
                        if (searchIndex3 < searchIndex2)
                        {
                            return returnValue;
                        }
                        szAux2 = String.Empty;
                        szAux2 = szAux.Substring(searchIndex2 + 2,
                                searchIndex3 - searchIndex2 - 2);
                        if (Int32.TryParse(szAux2, out nEnd2) == false)
                        {
                            if (mapConstants.ContainsKey(szAux2) == false)
                            {
                                return returnValue;
                            }
                            nEnd2 = mapConstants[szAux2];
                        }
                        if (nEnd2 < nStart2)
                        {
                            return returnValue;
                        }

                        searchIndex1 = szAux.IndexOf("OF");
                        if (searchIndex1 < searchIndex3)
                        {
                            return returnValue;
                        }
                        if (szAux.Length < (searchIndex1 + 3))
                        {
                            return returnValue;
                        }
                        szAux2 = String.Empty;
                        szAux2 = szAux.Substring(searchIndex1 + 2);
                        szAux2 = szAux2.Trim();
                        if (GetVarTypeAndSize(ref szAux2, ref StandardSize) < 0)
                        {
                            if (!mapUDT.ContainsKey(szAux2) || mapUDT[szAux2].NoMembers())
                            {
                                if (!mapSTRUCT.ContainsKey(szAux2) || mapSTRUCT[szAux2].NoMembers())
                                {
                                    if (!mapFunctionBlock.ContainsKey(szAux2) || mapFunctionBlock[szAux2].NoMembers())
                                    {
                                        return returnValue;
                                    }
                                }
                            }
                        }

                        ArrayStart0 = (UInt32)nStart0;
                        ArraySize0 = (UInt32)(nEnd0 - nStart0 + 1);
                        ArrayStart1 = (UInt32)nStart1;
                        ArraySize1 = (UInt32)(nEnd1 - nStart1 + 1);
                        ArrayStart2 = (UInt32)nStart2;
                        ArraySize2 = (UInt32)(nEnd2 - nStart2 + 1);
                        FieldType = String.Empty;
                        FieldType = szAux2;
                        returnValue = (int)ImportTypes.Array;
                    }
                }

                return returnValue;
            }

            if (mapUDT.ContainsKey(FieldType) == true)
            {
                string[] udtDefinition = mapUDT[FieldType].GetRawMembers();
                if (udtDefinition.Length > 0)
                {
                    //if(UdtIsEnum(udtDefinition) == false)
                    string stdType = String.Empty;
                    if (TypeIsEnumOrStandardTypeAlias(FieldType, ref stdType, ref MovType, ref StandardSize) == false)
                    {
                        returnValue = (int)ImportTypes.StructOrEnum;
                    }
                    else
                    {
                        //string stdType = "INT";
                        //MovType = GetVarTypeAndSize(ref stdType, ref StandardSize);
                        if (MovType >= 0)
                        {
                            returnValue = (int)ImportTypes.Standard;
                        }
                    }
                }

                return returnValue;
            }

            if (mapSTRUCT.ContainsKey(FieldType) && !mapSTRUCT[FieldType].NoMembers())
            {
                returnValue = (int)ImportTypes.StructOrEnum;
                return returnValue;
            }

            if (mapFunctionBlock.ContainsKey(FieldType) && !mapFunctionBlock[FieldType].NoMembers())
            {
                returnValue = (int)ImportTypes.StructOrEnum;
                return returnValue;
            }

            return returnValue;
        }

        private bool TypeIsEnumOrStandardTypeAlias(string typeName, ref string trueTypeName, ref int movType, ref uint dataSize)
        {
            bool returnValue = false;
            trueTypeName = String.Empty;
            movType = (int)UFUAModel.DataType.Boolean;
            dataSize = 1;
            //if(!TypeIsEnum(typeName, ref trueTypeName, ref movType, ref dataSize))
            returnValue = TypeIsEnum(typeName, ref trueTypeName, ref movType, ref dataSize);
            if (returnValue == false)
            {
                trueTypeName = String.Empty;
                movType = (int)UFUAModel.DataType.Boolean;
                dataSize = 1;
                returnValue = TypeIsStandardAlias(typeName, ref trueTypeName, ref movType, ref dataSize);
            }
            return (returnValue);
        }

        private bool TypeIsStandardAlias(string typeName, ref string trueTypeName, ref int movType, ref uint dataSize)
        {
            bool returnValue = false;
            trueTypeName = String.Empty;
            movType = (int)UFUAModel.DataType.Boolean;
            dataSize = 1;
            if (mapUDT.ContainsKey(typeName) == true)
            {
                string[] udtDefinition = mapUDT[typeName].GetRawMembers();
                if ((udtDefinition != null) && (udtDefinition.Count() > 0))
                {
                    if (UdtIsStandardAlias(udtDefinition) == true)
                    {
                        string fieldLine = udtDefinition[0];
                        int index = fieldLine.IndexOf(':');
                        if ((index > 0) && ((index + 2) <= fieldLine.Length))
                        {
                            string enumEquivalentType = fieldLine.Substring(index + 1);
                            enumEquivalentType.Trim();
                            if (String.IsNullOrWhiteSpace(enumEquivalentType))
                            {
                                trueTypeName = "INT";
                            }
                            trueTypeName = enumEquivalentType.ToUpper();
                        }
                        else
                        {
                            trueTypeName = "INT";
                        }

                        movType = GetVarTypeAndSize(ref trueTypeName, ref dataSize);
                        if (movType < 0)
                        {
                            trueTypeName = "INT";
                            movType = (int)UFUAModel.DataType.Int16;
                            dataSize = 2;
                        }

                        returnValue = true;
                    }
                }
            }
            return (returnValue);
        }

        private bool TypeIsEnum(string typeName, ref string trueTypeName, ref int movType, ref uint dataSize)
        {
            bool returnValue = false;
            trueTypeName = String.Empty;
            movType = (int)UFUAModel.DataType.Boolean;
            dataSize = 1;
            if (mapUDT.ContainsKey(typeName) == true)
            {
                string[] udtDefinition = mapUDT[typeName].GetRawMembers();
                if ((udtDefinition != null) && (udtDefinition.Count() > 0))
                {
                    if (UdtIsEnum(udtDefinition) == true)
                    {
                        string fieldLine = udtDefinition[0];
                        int index = fieldLine.IndexOf(':');
                        if ((index > 0) && ((index + 2) <= fieldLine.Length))
                        {
                            string enumEquivalentType = fieldLine.Substring(index + 1);
                            enumEquivalentType.Trim();
                            if (String.IsNullOrWhiteSpace(enumEquivalentType))
                            {
                                trueTypeName = "INT";
                            }
                            trueTypeName = enumEquivalentType.ToUpper();
                        }
                        else
                        {
                            trueTypeName = "INT";
                        }

                        movType = GetVarTypeAndSize(ref trueTypeName, ref dataSize);
                        if (movType < 0)
                        {
                            trueTypeName = "INT";
                            movType = (int)UFUAModel.DataType.Int16;
                            dataSize = 2;
                        }

                        returnValue = true;
                    }
                }
            }
            return (returnValue);
        }

        private bool UdtIsStandardAlias(string[] udtDefinition)
        {
            if (udtDefinition.Length != 1)
            {
                return false;
            }

            string fieldLine = udtDefinition[0];
            int index = fieldLine.IndexOf(':');
            if (index <= 0)
            {
                return false;
            }

            string fieldName = fieldLine.Substring(0, index);
            fieldName = fieldName.Trim();
            if (fieldName == String.Empty)
            {
                return (false);
            }

            if (fieldLine.Length < (index + 2))
            {
                return (false);
            }

            string fieldType = fieldLine.Substring(index + 1);
            fieldType = fieldType.Trim();
            if (fieldType == String.Empty)
            {
                return (false);
            }
            if (String.Compare(fieldName, ImportStandardTypeAlias, true) != 0)
            {
                return (false);
            }

            return true;
        }

        private bool UdtIsEnum(string[] udtDefinition)
        {
            if (udtDefinition.Length != 1)
            {
                return false;
            }

            string fieldLine = udtDefinition[0];
            int index = fieldLine.IndexOf(':');
            if (index <= 0)
            {
                return false;
            }

            string fieldName = fieldLine.Substring(0, index);
            fieldName = fieldName.Trim();
            if (fieldName == String.Empty)
            {
                return (false);
            }

            if (fieldLine.Length < (index + 2))
            {
                return (false);
            }

            string fieldType = fieldLine.Substring(index + 1);
            fieldType = fieldType.Trim();
            if (fieldType == String.Empty)
            {
                return (false);
            }
            if (String.Compare(fieldName, ImportEnumAlias, true) != 0)
            {
                return (false);
            }

            return true;
        }

        private void ParseSingleDataTypeTPY(XElement dataType)
        {
            XElement dataTypeName = dataType.Element("Name");
            XElement dataTypeType = dataType.Element("Type");
            if (dataTypeName == null)
            {
                return;
            }
            string typeName = dataTypeName.Value;
            if (String.IsNullOrEmpty(typeName))
            {
                return;
            }

            // Check Pointer type 
            if (dataTypeType != null)
            {
                string szPointer = (string)dataTypeType.Attribute("Pointer");
                if (szPointer != null)
                {
                    UInt32 pointerValue = 0;
                    try
                    {
                        pointerValue = Convert.ToUInt32(szPointer, 16);
                    }
                    catch (FormatException)
                    {
                        return;
                    }
                    catch (OverflowException)
                    {
                        return;
                    }
                    catch (ArgumentException)
                    {
                        return;
                    }
                    if (pointerValue != 0)
                        return;
                }
            }

            string typeDecoration = (string)dataTypeName.Attribute("Decoration");
            if (typeDecoration == null)
            {
                return;
            }

            UInt64 decorationValue64 = Convert.ToUInt64(typeDecoration, 16);
            if (decorationValue64 > 0xFFFFFFFF)
            {
                return;
            }
            UInt32 decorationValue = (UInt32)decorationValue64;
            byte category = GetUdtTypeTPY(decorationValue);
            switch (category)
            {
                case (byte)ImportUDTType.Struct:
                    ParseDataTypeStructTPY(dataType, typeName);
                    break;

                case (byte)ImportUDTType.Array:
                    ParseDataTypeArrayTPY(dataType, typeName);
                    break;

                case (byte)ImportUDTType.Enum:
                    ParseDataTypeEnumTPY(dataType, typeName);
                    break;
            }
        }

        private byte GetUdtTypeTPY(UInt32 decorationValue)
        {
            byte udtType = (byte)ImportUDTType.Unknown;
            UInt32 auxValue = decorationValue >> 24;
            switch (auxValue)
            {
                case 0:
                    udtType = (byte)ImportUDTType.Alias;
                    break;

                case 0x10:
                case 0x11:
                    udtType = (byte)ImportUDTType.Struct;
                    break;

                case 0x20:
                    udtType = (byte)ImportUDTType.Array;
                    break;

                case 0x30:
                    udtType = (byte)ImportUDTType.Enum;
                    break;
            }
            return (udtType);
        }

        private void ParseDataTypeStructTPY(XElement dataType, string typeName)
        {
            IEnumerable<XElement> subTypes = from subType
                                             in dataType.Elements()
                                             where subType.Name == "Type"
                                             select subType;
            if (subTypes.Count() > 0)
            {
                return;
            }

            IEnumerable<XElement> subItems = from subElement
                                             in dataType.Elements()
                                             where subElement.Name == "SubItem"
                                             select subElement;
            if (subItems.Count() <= 0)
            {
                return;
            }

            List<string> listOfStrings = new List<string>();

            foreach (XElement subItemInfo in subItems)
            {
                string DataTypeStructSubItem = String.Empty;
                ParseDataTypeStructSubItemTPY(subItemInfo, ref DataTypeStructSubItem);
                if (DataTypeStructSubItem != String.Empty)
                {
                    listOfStrings.Add(DataTypeStructSubItem);
                }
            }
            if (listOfStrings.Count == 0)
            {
                return;
            }

            XElement dataTypeFbInfo = dataType.Element("FbInfo");
            if (dataTypeFbInfo == null)
            {
                mapSTRUCT[typeName] = new TwincatPrototypeMembersInfo(listOfStrings.ToArray());
            }
            else
            {
                mapFunctionBlock[typeName] = new TwincatPrototypeMembersInfo(listOfStrings.ToArray());
            }
        }

        private void ParseDataTypeArrayTPY(XElement dataType, string typeName)
        {
            XElement dataTypeArrayInfo = dataType.Element("ArrayInfo");
            if (dataTypeArrayInfo == null)
            {
                return;
            }

            if (mapUDT.ContainsKey(typeName))
            {
                return;
            }

            string[] arrayOfStrings = new string[1];
            arrayOfStrings[0] = "___Movicon_Import_Array : " + typeName;
            mapUDT[typeName] = new TwincatPrototypeMembersInfo(arrayOfStrings);
        }

        private void ParseDataTypeEnumTPY(XElement dataType, string typeName)
        {
            XElement dataTypeInfo = dataType.Element("Type");
            if (dataTypeInfo == null)
            {
                return;
            }

            if (mapUDT.ContainsKey(typeName))
            {
                return;
            }

            string dataTypeInfoName = dataTypeInfo.Value;

            string[] arrayOfStrings = new string[1];
            arrayOfStrings[0] = ImportEnumAlias + " : " + dataTypeInfoName;
            mapUDT[typeName] = new TwincatPrototypeMembersInfo(arrayOfStrings);

            uint VarSize = 0;
            int VarType = GetVarTypeAndSize(ref dataTypeInfoName, ref VarSize);
            if ((VarType <= (int)UFUAModel.DataType.UInt32) && (VarType >= (int)UFUAModel.DataType.SByte))
            {

                IEnumerable<XElement> enumInfos = from enumElement
                                                 in dataType.Elements()
                                                  where enumElement.Name == "EnumInfo"
                                                  select enumElement;
                if (enumInfos.Count() <= 0)
                {
                    return;
                }

                foreach (XElement enumInfo in enumInfos)
                {
                    ParseDataTypeEnumConstantTPY(enumInfo);
                }
            }
        }

        private void ParseDataTypeStructSubItemTPY(XElement subItem, ref string subItemString)
        {
            subItemString = String.Empty;

            XElement subItemName = subItem.Element("Name");
            if (subItemName == null)
            {
                return;
            }
            string itemName = subItemName.Value;
            if (String.IsNullOrEmpty(itemName))
            {
                return;
            }

            XElement subItemType = subItem.Element("Type");
            if (subItemType == null)
            {
                return;
            }
            // Check Pointer type 
            string szPointer = (string)subItemType.Attribute("Pointer");
            if (szPointer != null)
            {
                UInt32 pointerValue = 0;
                try
                {
                    pointerValue = Convert.ToUInt32(szPointer, 16);
                }
                catch (FormatException)
                {
                    return;
                }
                catch (OverflowException)
                {
                    return;
                }
                catch (ArgumentException)
                {
                    return;
                }
                if (pointerValue != 0)
                    return;
            }
            string itemType = subItemType.Value;
            if (String.IsNullOrEmpty(itemType))
            {
                return;
            }

            subItemString = itemName + " : " + itemType;
        }

        private int GetVarTypeAndSize(ITcAdsSymbol adsSymbolInfo, ref string Type, ref uint VarSize)
        {
            VarSize = 0;
            Type = String.Empty;
            int nType = -1;
            switch (adsSymbolInfo.Datatype)
            {
                case AdsDatatypeId.ADST_BIT:
                    nType = (int)UFUAModel.DataType.Boolean;
                    Type = "BOOL";
                    VarSize = 1;
                    break;
                case AdsDatatypeId.ADST_INT8:
                    nType = (int)UFUAModel.DataType.SByte;
                    Type = "SINT";
                    VarSize = 1;
                    break;
                case AdsDatatypeId.ADST_UINT8:
                    nType = (int)UFUAModel.DataType.Byte;
                    Type = "BYTE";
                    VarSize = 1;
                    break;
                case AdsDatatypeId.ADST_INT16:
                    nType = (int)UFUAModel.DataType.Int16;
                    Type = "INT";
                    VarSize = 2;
                    break;
                case AdsDatatypeId.ADST_UINT16:
                    nType = (int)UFUAModel.DataType.UInt16;
                    Type = "UINT";
                    VarSize = 2;
                    break;
                case AdsDatatypeId.ADST_INT32:
                    nType = (int)UFUAModel.DataType.Int32;
                    Type = "DINT";
                    VarSize = 4;
                    break;
                case AdsDatatypeId.ADST_UINT32:
                    nType = (int)UFUAModel.DataType.UInt32;
                    Type = "UDINT";
                    VarSize = 4;
                    break;
                case AdsDatatypeId.ADST_INT64:
                    nType = (int)UFUAModel.DataType.Int64;
                    Type = "LINT";
                    VarSize = 8;
                    break;
                case AdsDatatypeId.ADST_UINT64:
                    nType = (int)UFUAModel.DataType.UInt64;
                    Type = "ULINT";
                    VarSize = 8;
                    break;
                case AdsDatatypeId.ADST_REAL32:
                    nType = (int)UFUAModel.DataType.Float;
                    Type = "REAL";
                    VarSize = 4;
                    break;
                case AdsDatatypeId.ADST_REAL64:
                    nType = (int)UFUAModel.DataType.Double;
                    Type = "LREAL";
                    VarSize = 8;
                    break;
                case AdsDatatypeId.ADST_STRING:
                    nType = (int)UFUAModel.DataType.String;
                    Type = "STRING";
                    VarSize = (uint)adsSymbolInfo.Size;
                    break;
            }
            return (nType);
        }

        /// <summary>
        /// Define the list of supported Twincat data type in Movicon
        /// </summary>
        private void DefineSupportedTwincatDataType()
        {
            mapSupportedTwincatDataType["BOOL"] = new TwincatDataTypeInfo("BOOL", (int)UFUAModel.DataType.Boolean, 1);
            mapSupportedTwincatDataType["BIT"] = new TwincatDataTypeInfo("BIT", (int)UFUAModel.DataType.Boolean, 1);
            mapSupportedTwincatDataType["BYTE"] = new TwincatDataTypeInfo("BYTE", (int)UFUAModel.DataType.Byte, 1);
            mapSupportedTwincatDataType["WORD"] = new TwincatDataTypeInfo("WORD", (int)UFUAModel.DataType.UInt16, 2);
            mapSupportedTwincatDataType["DWORD"] = new TwincatDataTypeInfo("DWORD", (int)UFUAModel.DataType.UInt32, 4);
            mapSupportedTwincatDataType["SINT"] = new TwincatDataTypeInfo("SINT", (int)UFUAModel.DataType.SByte, 1);
            mapSupportedTwincatDataType["USINT"] = new TwincatDataTypeInfo("USINT", (int)UFUAModel.DataType.Byte, 1);
            mapSupportedTwincatDataType["INT"] = new TwincatDataTypeInfo("INT", (int)UFUAModel.DataType.Int16, 2);
            mapSupportedTwincatDataType["UINT"] = new TwincatDataTypeInfo("UINT", (int)UFUAModel.DataType.UInt16, 2);
            mapSupportedTwincatDataType["INT16"] = new TwincatDataTypeInfo("INT16", (int)UFUAModel.DataType.Int16, 2);
            mapSupportedTwincatDataType["INT16"] = new TwincatDataTypeInfo("INT16", (int)UFUAModel.DataType.UInt16, 2);
            mapSupportedTwincatDataType["DINT"] = new TwincatDataTypeInfo("DINT", (int)UFUAModel.DataType.Int32, 4);
            mapSupportedTwincatDataType["UDINT"] = new TwincatDataTypeInfo("UDINT", (int)UFUAModel.DataType.UInt32, 4);
            mapSupportedTwincatDataType["REAL"] = new TwincatDataTypeInfo("REAL", (int)UFUAModel.DataType.Float, 4);
            mapSupportedTwincatDataType["LREAL"] = new TwincatDataTypeInfo("LREAL", (int)UFUAModel.DataType.Double, 8);
            mapSupportedTwincatDataType["LINT"] = new TwincatDataTypeInfo("LINT", (int)UFUAModel.DataType.Int64, 8);
            mapSupportedTwincatDataType["ULINT"] = new TwincatDataTypeInfo("ULINT", (int)UFUAModel.DataType.UInt64, 8);
            //mapSupportedTwincatDataType["STRING"] = new TwincatDataTypeInfo("STRING", (int)UFUAModel.DataType.String, 0);
            mapSupportedTwincatDataType["TIME"] = new TwincatDataTypeInfo("TIME", (int)UFUAModel.DataType.UInt32, 4);
            mapSupportedTwincatDataType["TIME_OF_DAY"] = new TwincatDataTypeInfo("TIME_OF_DAY", (int)UFUAModel.DataType.UInt32, 4);
            mapSupportedTwincatDataType["TOD"] = new TwincatDataTypeInfo("TOD", (int)UFUAModel.DataType.UInt32, 4);
            mapSupportedTwincatDataType["DATE"] = new TwincatDataTypeInfo("DATE", (int)UFUAModel.DataType.UInt32, 4);
            mapSupportedTwincatDataType["DT"] = new TwincatDataTypeInfo("DT", (int)UFUAModel.DataType.UInt32, 4);
            mapSupportedTwincatDataType["DATE_AND_TIME"] = new TwincatDataTypeInfo("DATE_AND_TIME", (int)UFUAModel.DataType.UInt32, 4);
            mapSupportedTwincatDataType["OTCID"] = new TwincatDataTypeInfo("OTCID", (int)UFUAModel.DataType.UInt32, 4);
        }


        private int GetVarTypeAndSize(ref string Type, ref uint VarSize)
        {
            int nType = -1;
            VarSize = 0;
            Type = Type.Trim();

            if (Type.Length > 0)
            {
                // test data type "support" using "orginal" definition 
                if (mapSupportedTwincatDataType.TryGetValue(Type, out TwincatDataTypeInfo t))
                {
                    nType = t.nType;
                    VarSize = t.VarSize;
                } // test data type "support" using Upcase definition 
                else if (mapSupportedTwincatDataType.TryGetValue(Type.ToUpper(), out TwincatDataTypeInfo t1))
                {
                    nType = t1.nType;
                    VarSize = t1.VarSize;
                }
                else if (Type.ToUpper().IndexOf("STRING") == 0)
                {
                    nType = (int)UFUAModel.DataType.String;
                    VarSize = 81;
                    int nIndex = Type.IndexOf('(');
                    int nIndex2 = Type.IndexOf(')');
                    string szSize = String.Empty;
                    if ((nIndex > 0) && (nIndex2 > (nIndex + 1)))
                    {
                        szSize = Type.Substring(nIndex + 1, nIndex2 - nIndex - 1);
                        szSize = szSize.Trim();
                        if (szSize != String.Empty)
                        {
                            int stringSize = 0;
                            if (Int32.TryParse(szSize, out stringSize) == true)
                            {
                                if (stringSize > 0)
                                {
                                    VarSize = (uint)stringSize + 1;
                                    Type = "STRING";
                                }
                            }
                        }
                    }
                    else
                    {
                        nIndex = Type.IndexOf('[');
                        nIndex2 = Type.IndexOf(']');
                        if ((nIndex > 0) && (nIndex2 > (nIndex + 1)))
                        {
                            szSize = Type.Substring(nIndex + 1, nIndex2 - nIndex - 1);
                            szSize = szSize.Trim();
                            if (szSize != String.Empty)
                            {
                                int stringSize = 0;
                                if (Int32.TryParse(szSize, out stringSize) == true)
                                {
                                    if (stringSize > 0)
                                    {
                                        VarSize = (uint)stringSize + 1;
                                        Type = "STRING";
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return (nType);
        }

        void ParseDataTypeEnumConstantTPY(XElement enumInfo)
        {
            XElement enumInfoText = enumInfo.Element("Text");
            if (enumInfoText == null)
            {
                return;
            }
            string enumInfoTextValue = enumInfoText.Value;
            if (String.IsNullOrEmpty(enumInfoTextValue))
            {
                return;
            }
            XElement enumInfoEnum = enumInfo.Element("Enum");
            if (enumInfoEnum == null)
            {
                return;
            }
            string enumInfoEnumString = enumInfoEnum.Value;
            if (String.IsNullOrEmpty(enumInfoEnumString))
            {
                return;
            }
            Int32 enumInfoEnumValue = 0;
            if (Int32.TryParse(enumInfoEnumString, out enumInfoEnumValue) == false)
            {
                return;
            }
            if (mapConstants.ContainsKey(enumInfoTextValue) == false)
            {
                mapConstants.Add(enumInfoTextValue, enumInfoEnumValue);
            }
        }

        private bool VarIsConstant(string varDescr)
        {
            if (varDescr.IndexOf("Constant ", StringComparison.OrdinalIgnoreCase) > 0)
            {
                return true;
            }
            return false;
        }

        internal void AddTreeItem(ImportData tag, ImportData parent = null)
        {
            tag.Parent = parent;
            ((ImportDataTwinCAT)tag).strId = tag.Id.ToString("X6");
            if (parent == null)
            {
                tag.TreeLevel = 0xFF;
                tag.parentId = -1;
                importDataModel.Children.Add(tag);
            }
            else
            {
                tag.TreeLevel = parent.TreeLevel - 1;
                tag.parentId = parent.Id;
                parent.Children.Add(tag);
            }
            ((ImportDataTwinCAT)tag).strLevel = tag.TreeLevel.ToString("X2");
        }

        #region DirectImport


        /// <summary>
        /// Import of tags directly from the PLC
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private ImportDataModel Read_Plc_Info()
        {
            bErrorsOccurredDuringImport = false;

            DisplayDirectImport();

            return importDataModel;
        }

        private void DisplayDirectImport()
        {
            OneLevelDisplayDirectImport();
        }

        private void OneLevelDisplayDirectImport()
        {
            //Loading channel and station for connections; set also project's driver folder path
            string stationName = baseImportTree.ReadStationName();
            TwinCATStation station = new TwinCATStation(new TwinCATDriver(conn), (TwinCATStationSettings)baseImportTree.GetStationSettings(stationName));
            TwinCATChannel channel = new TwinCATChannel(new TwinCATDriver(conn), (TwinCATChannelSettings)baseImportTree.GetChannelSettingsFromStation(stationName));

            if (!channel.TestChannelComm())
            {
                MessageBox.Show(Properties.Resources.ErrorConnectToDeviceIP + channel.TwinCATAdsAmsNetId,
                                     Properties.Resources.ImportMsgBoxTitle);

                importDataModel = null;
                return;
            }

            using (new WaitCursor())
            {
                List<TcAdsSymbolInfo> ListTcAdsSymbol = new List<TcAdsSymbolInfo>();
                if (!channel.AdsSyncReadReq(ref ListTcAdsSymbol, out string error))
                {
                    MessageBox.Show(error, Properties.Resources.ImportMsgBoxTitle);
                    channel.DeviceClose();
                    importDataModel = null;
                    return;
                }
                channel.DeviceClose();

                if (ListTcAdsSymbol.Count == 0)
                {
                    importDataModel = null;
                    return;
                }

                if (importDataModel != null)
                {
                    importDataModel.Dispose();
                }
                importDataModel = new ImportDataModelTwinCAT(readStationName);

                // Prepare maps for the UDT types
                mapUDT.Clear();
                mapSTRUCT.Clear();
                mapFunctionBlock.Clear();
                mapConstants.Clear();
                mapOK.Clear();
                mapTemp.Clear();
                foreach (TcAdsSymbolInfo vs in ListTcAdsSymbol)
                {
                    AddSymbolTypeToMapForDirectImport(vs);
                }
                mapOK = mapUDT;

                // Parse symbols info
                m_lGlobalID = 0;
                foreach (TcAdsSymbolInfo vs in ListTcAdsSymbol)
                {
                    string Type = vs.Type;
                    uint nVarSize = 0;
                    int nType = GetVarTypeAndSize(ref Type, ref nVarSize);
                    if (nType != -1)
                    {
                        AddSimpleVariablePLCImport(vs, nVarSize, Type, nType);
                    }
                    else
                    {
                        OneLevelAddStructureOrArray(vs, nVarSize, Type);
                    }
                }
            }

            if (importDataModel == null || importDataModel.Children.Count == 0)
            {
                MessageBox.Show(DriverCodeBaseEx.UI.Properties.Resources.ImportErrorEmptyFile,
                                Properties.Resources.ImportMsgBoxTitle);
                importDataModel = null;
            }

            PrintingOfImportErrors();
        }

        private void AddSymbolTypeToMapForDirectImport(TcAdsSymbolInfo vs)
        {
            // Skip pointers
            if ((vs.IsPointer == true) || (vs.IsTcComInterfacePointer))
            {
                return;
            }

            // Skip standard types, but set a dummy type for enumeration and alias types
            // If vs.SubSymbolCount == 0, the type of the symbol could be a standard one, an enumeration or an alias for a standard type 
            if (vs.SubSymbolCount < 1)
            {
                // Check data type
                string Type = vs.Type;
                //if(String.IsNullOrEmpty(Type) || (Type == "ITComObjectServer") || (Type == "OTCID"))
                if (String.IsNullOrEmpty(Type))
                {
                    return;
                }
                uint nVarSize = 0;
                int nType = GetVarTypeAndSize(ref Type, ref nVarSize);

                // Standard type
                if (nType != -1)
                {
                    return;
                }

                // Enumeration or alias for a standard type?
                string correspondingType = String.Empty;
                uint correspondingVarSize = (uint)vs.Size;

                nType = GetVarTypeAndSize(vs, ref correspondingType, ref correspondingVarSize);

                // nType = GetEnumTypeCorrespondingToSize(ref correspondingType, correspondingVarSize);
                if ((nType != -1) && !mapUDT.Keys.Contains(Type))
                {
                    string[] arrayOfStrings = new string[1];
                    arrayOfStrings[0] = ImportStandardTypeAlias + " : " + correspondingType;
                    mapUDT[Type] = new TwincatPrototypeMembersInfo(arrayOfStrings);
                }
                return;
            }

            // Check if the symbol is an array of structures
            if ((vs.IsArray) || vs.Type.Contains("ARRAY"))
            {
                if (vs.SubSymbolCount < 1)
                {
                    return;
                }
                TcAdsSymbolInfo itemNext = vs.SubSymbols[0];

                // check that the object is not null, because it could lead to an exception
                if (itemNext == null)
                {
                    log.ErrorFormat(Properties.Resources.ImportErrorNullSubItem, vs.Name, 0);
                    bErrorsOccurredDuringImport = true;
                    return;
                }

                string TypeItem = itemNext.Type;
                uint nVarSizeItem = 0;
                int nType = GetVarTypeAndSize(ref TypeItem, ref nVarSizeItem);

                AddSymbolTypeToMapForDirectImport(itemNext);

                return;
            }

            string StructureName = vs.Type;

            for (int indexElem = 0; indexElem < vs.SubSymbolCount; indexElem++)
            {
                TcAdsSymbolInfo itemNext = vs.SubSymbols[indexElem];

                //check that the object is not null, because it could lead to an exception
                if (itemNext == null)
                {
                    log.ErrorFormat(Properties.Resources.ImportErrorNullSubItem, vs.Name, indexElem);
                    bErrorsOccurredDuringImport = true;
                    continue;
                }

                string TypeItem = itemNext.Type;
                uint nVarSizeItem = 0;
                // Make s copy of the type string, because in case of strings GetVarTypeAndSize remove the size from the type definition  
                string itemType = String.Copy(TypeItem);
                int nType = GetVarTypeAndSize(ref itemType, ref nVarSizeItem);

                //type of data that has not been imported because it is not known
                switch (TypeItem)
                {
                    case "ITComObjectServer":
                        //case "OTCID":
                        continue;
                }

                List<string> listOfStrings = new List<string>();
                if (!mapUDT.ContainsKey(StructureName))
                {
                    listOfStrings.Add(itemNext.ShortName + ":" + TypeItem);
                    mapUDT[StructureName] = new TwincatPrototypeMembersInfo(listOfStrings.ToArray());
                }
                else
                {
                    TwincatPrototypeMembersInfo arr = null;
                    if (!mapUDT.TryGetValue(StructureName, out arr))
                    {
                        continue;
                    }
                    listOfStrings.AddRange(arr.GetRawMembers());
                    //check that the member type is not already stored
                    if (!listOfStrings.Contains(itemNext.ShortName + ":" + TypeItem))
                    {
                        listOfStrings.Add(itemNext.ShortName + ":" + TypeItem);
                        mapUDT[StructureName] = new TwincatPrototypeMembersInfo(listOfStrings.ToArray());
                    }
                }
                if (nType == -1)
                {
                    AddSymbolTypeToMapForDirectImport(itemNext);
                }
            }
        }

        private void AddSimpleVariablePLCImport(TcAdsSymbolInfo vs, uint nVarSize, string Type, int nType, int parentId = -1)
        {
            ImportDataTwinCAT Item = (ImportDataTwinCAT)importDataModel.addImportData();
            Item.Name = vs.Name;
            Item.Size = nVarSize;
            Item.szType = Type;
            Item.parentId = parentId;
            Item.Address = vs.Name;
            Item.ElemType = Type;
            Item.Description = vs.Comment;
            Item.Id = m_lGlobalID++;
            Item.ArrayDimension = 0;
            Item.Type = (DataType)nType;
            Item.ImportDataType = ImportTypes.Standard;

            AddTreeItem(Item);
        }

        private void AddSructureOrArray(TcAdsSymbolInfo vs, uint nVarSize, string Type, ref ImportData id, int parentId = -1)
        {
            if ((vs.IsArray) || Type.Contains("ARRAY"))
            {

                string szNamePrefix = string.Empty;
                string szSingleVarName = vs.Name;
                string szAddressPrefix = string.Empty;
                string description = vs.Comment;
                uint nArraySize0 = 0;
                uint nArraySize1 = 0;
                uint nArraySize2 = 0;
                uint nArrayStart0 = 0;
                uint nArrayStart1 = 0;
                uint nArrayStart2 = 0;
                int nMovType = -1;
                int nRet = GetVarTypeAndSizeFileExp(ref Type, ref nVarSize,
                                           ref nArraySize0, ref nArraySize1,
                                           ref nArraySize2, ref nArrayStart0,
                                           ref nArrayStart1,
                                           ref nArrayStart2, ref nMovType);

                if (nRet == (int)ImportTypes.Unknown)
                {
                    try
                    {
                        nVarSize = (uint)vs.Size;
                        AddStructureVariablePLCImport(vs, nVarSize, Type, ref id, m_lGlobalID);
                    }
                    catch (Exception ex)
                    {
                        log.ErrorFormat(Properties.Resources.ImportException, ex.Message);
                        bErrorsOccurredDuringImport = true;
                    }
                }
                else
                {
                    AddArrayList(ref szNamePrefix,
                                ref szSingleVarName,
                                ref szAddressPrefix,
                                ref description,
                                ref Type,
                                ref nVarSize,
                                nArraySize0,
                                nArraySize1,
                                nArraySize2,
                                nArrayStart0,
                                nArrayStart1,
                                nArrayStart2);
                }
            }
            else if ((vs.SubSymbolCount > 0) && (!string.IsNullOrWhiteSpace(vs.Type)))
            {
                try
                {
                    nVarSize = (uint)vs.Size;
                    AddStructureVariablePLCImport(vs, nVarSize, Type, ref id, m_lGlobalID);
                }
                catch (Exception ex)
                {
                    log.ErrorFormat(Properties.Resources.ImportException, ex.Message);
                    bErrorsOccurredDuringImport = true;
                }
            }
        }

        private void OneLevelAddStructureOrArray(TcAdsSymbolInfo vs, uint nVarSize, string Type)
        {
            // Array
            if ((vs.IsArray) || Type.Contains("ARRAY"))
            {

                string szNamePrefix = string.Empty;
                string szSingleVarName = vs.Name;
                string szAddressPrefix = string.Empty;
                string description = vs.Comment;
                uint nArraySize0 = 0;
                uint nArraySize1 = 0;
                uint nArraySize2 = 0;
                uint nArrayStart0 = 0;
                uint nArrayStart1 = 0;
                uint nArrayStart2 = 0;
                int nMovType = -1;
                int nRet = GetVarTypeAndSizeFileExp(ref Type, ref nVarSize,
                                           ref nArraySize0, ref nArraySize1,
                                           ref nArraySize2, ref nArrayStart0,
                                           ref nArrayStart1,
                                           ref nArrayStart2, ref nMovType);

                // Array of structures
                if (nRet == (int)ImportTypes.Unknown)
                {
                    try
                    {
                        nVarSize = (uint)vs.Size;
                        OneLevelAddRootStructureVariablePLCImport(vs, nVarSize, Type, m_lGlobalID);
                    }
                    catch (Exception ex)
                    {
                        log.ErrorFormat(Properties.Resources.ImportException, ex.Message);
                        bErrorsOccurredDuringImport = true;
                    }
                }

                // Array of standard type elements
                else
                {
                    OneLevelAddArrayList(ref szNamePrefix,
                                         ref szSingleVarName,
                                         ref szAddressPrefix,
                                         ref description,
                                         ref Type,
                                         ref nVarSize,
                                         nArraySize0,
                                         nArraySize1,
                                         nArraySize2,
                                         nArrayStart0,
                                         nArrayStart1,
                                         nArrayStart2);
                }
            }

            // Structure
            else if ((vs.SubSymbolCount > 0) && (!string.IsNullOrWhiteSpace(vs.Type)))
            {
                try
                {
                    nVarSize = (uint)vs.Size;
                    OneLevelAddRootStructureVariablePLCImport(vs, nVarSize, Type, m_lGlobalID);
                }
                catch (Exception ex)
                {
                    log.ErrorFormat(Properties.Resources.ImportException, ex.Message);
                    bErrorsOccurredDuringImport = true;
                }
            }

            // Enumeration?
            else if (!string.IsNullOrWhiteSpace(vs.Type) && (vs.Type != "ITComObjectServer") && (vs.IsPointer == false) && (vs.IsTcComInterfacePointer == false))
            {
                string correspondingType = String.Empty;
                uint correspondingVarSize = (uint)vs.Size;
                //int typeCode = GetEnumTypeCorrespondingToSize(ref correspondingType, correspondingVarSize);
                int typeCode = GetVarTypeAndSize(vs, ref correspondingType, ref correspondingVarSize);
                if (typeCode != -1)
                {
                    AddSimpleVariablePLCImport(vs, correspondingVarSize, correspondingType, typeCode);
                }
            }

        }

        private void AddStructureVariablePLCImport(TcAdsSymbolInfo vs, uint nVarSize, string Type, ref ImportData SubItemSt, int parentId = -1)
        {
            string StrucutreName = vs.Type;
            ImportData itemBase = null;
            if (SubItemSt == null)
            {
                itemBase = importDataModel.addImportData();
                itemBase.Name = vs.Name;
                ((ImportDataTwinCAT)itemBase).Size = nVarSize;
                itemBase.szType = Type;
                itemBase.parentId = parentId;
                itemBase.Address = vs.Name;
                ((ImportDataTwinCAT)itemBase).ElemType = Type;
                itemBase.Description = vs.Comment;
                itemBase.Id = m_lGlobalID++;
                itemBase.ArrayDimension = 0;
            }
            else
            {
                itemBase = SubItemSt;
            }


            //Scroll through the items the itemBase
            for (int indexElem = 0; indexElem < vs.SubSymbolCount; indexElem++)
            {
                TcAdsSymbolInfo itemNext = vs.SubSymbols[indexElem];

                //check that the object is not null, because it could lead to an exception
                if (itemNext == null)
                {
                    log.ErrorFormat(Properties.Resources.ImportErrorNullSubItem, vs.Name, indexElem);
                    bErrorsOccurredDuringImport = true;
                    continue;
                }

                string TypeItem = itemNext.Type;
                uint nVarSizeItem = 0;
                int nType = GetVarTypeAndSize(ref TypeItem, ref nVarSizeItem);

                //type of data that has not been imported because it is not known
                switch (TypeItem)
                {
                    case "ITComObjectServer":
                    case "OTCID":
                        continue;
                        break;
                }

                if (nType != -1)
                {
                    List<string> listOfStrings = new List<string>();
                    //String types cannot be inserted into a prototype, because they need length.
                    if ((nType != (int)UFUAModel.DataType.String))
                    {
                        if (!mapUDT.ContainsKey(StrucutreName))
                        {
                            listOfStrings.Add(itemNext.ShortName + ":" + TypeItem);
                            mapUDT[StrucutreName] = new TwincatPrototypeMembersInfo(listOfStrings.ToArray());
                        }
                        else
                        {
                            TwincatPrototypeMembersInfo arr = null;
                            if (!mapUDT.TryGetValue(StrucutreName, out arr))
                            {
                                continue;
                            }
                            listOfStrings.AddRange(arr.GetRawMembers());
                            //check that the member type is not already stored
                            if (!listOfStrings.Contains(itemNext.ShortName + ":" + TypeItem))
                            {
                                listOfStrings.Add(itemNext.ShortName + ":" + TypeItem);
                                mapUDT[StrucutreName] = new TwincatPrototypeMembersInfo(listOfStrings.ToArray());
                            }
                        }
                    }

                    ImportData SubItem = importDataModel.addImportData();
                    SubItem.Address = itemNext.Name;
                    SubItem.Description = vs.Comment;
                    SubItem.Id = m_lGlobalID++;
                    SubItem.ArrayDimension = 0;
                    ((ImportDataTwinCAT)SubItem).Type = (DataType)nType;

                    //String types cannot be inserted into a prototype, because they need length.
                    if (nType == (int)UFUAModel.DataType.String)
                    {
                        SubItem.Name = itemNext.Name;
                        SubItem.parentId = -1;
                        ((ImportDataTwinCAT)SubItem).Size = nVarSizeItem;
                        SubItem.szType = TypeItem;
                        ((ImportDataTwinCAT)SubItem).ElemType = TypeItem;
                        AddTreeItem(SubItem);
                    }
                    else
                    {
                        SubItem.Name = itemNext.ShortName;
                        SubItem.parentId = parentId;
                        ((ImportDataTwinCAT)SubItem).Size = nVarSize;
                        SubItem.szType = TypeItem;
                        ((ImportDataTwinCAT)SubItem).ElemType = TypeItem;
                        AddTreeItem(SubItem, itemBase);
                    }
                }
                else
                {
                    ImportData SubItem = importDataModel.addImportData();
                    SubItem.Address = itemNext.Name;
                    SubItem.Description = vs.Comment;
                    SubItem.Id = m_lGlobalID++;
                    SubItem.ArrayDimension = 0;
                    ((ImportDataTwinCAT)SubItem).Type = (DataType)nType;
                    SubItem.Name = itemNext.ShortName;
                    SubItem.parentId = parentId;
                    ((ImportDataTwinCAT)SubItem).Size = nVarSize;
                    SubItem.szType = TypeItem;
                    ((ImportDataTwinCAT)SubItem).ElemType = TypeItem;
                    if (vs.IsArray || TypeItem.Contains("ARRAY"))
                    {
                        SubItem.Name = itemNext.Name;
                        AddSructureOrArray(itemNext, (uint)itemNext.Size, TypeItem, ref SubItem, -1);
                        if (!TypeItem.Contains("ARRAY"))
                        {
                            AddTreeItem(SubItem);
                        }
                    }
                    else
                    {
                        AddSructureOrArray(itemNext, (uint)itemNext.Size, TypeItem, ref SubItem, parentId);
                        AddTreeItem(SubItem, itemBase);
                    }
                }
            }
            if ((SubItemSt == null) && (!vs.IsArray))
            {
                AddTreeItem(itemBase);
            }
        }

        private void OneLevelAddRootStructureVariablePLCImport(TcAdsSymbolInfo vs, uint nVarSize, string Type, int parentId = -1)
        {
            // Add the structure
            ImportDataTwinCAT itemBase = (ImportDataTwinCAT)importDataModel.addImportData();
            itemBase.Name = vs.Name;
            itemBase.Size = nVarSize;
            itemBase.szType = "STRUCT";
            itemBase.parentId = parentId;
            itemBase.Address = vs.Name;
            itemBase.ElemType = Type;
            itemBase.Description = vs.Comment;
            itemBase.Id = m_lGlobalID++;
            itemBase.ArrayDimension = 0;
            itemBase.ImportDataType = ImportTypes.StructOrEnum;
            AddTreeItem(itemBase);

            // Add a dummy child
            itemBase.Children.Add(BaseImportTree.DummySubItem);
        }

        #endregion

        #region IDisposable Members
        public void Dispose()
        {
            using (new WaitCursor())
            {
                if (importDataModel != null)
                {
                    importDataModel.Dispose();
                    importDataModel = null;
                }

                if (baseImportTree != null)
                {
                    baseImportTree.LoadImportFile -= LoadFile;
                    baseImportTree.AddTrueChildren -= SubstituteDummyChildWithTrueChildren;

                    baseImportTree.Dispose();
                    baseImportTree = null;
                }
            }
        }
        #endregion
    }

    public class ImportDataTwinCAT : ImportData, IDisposable
    {
        public ImportDataTwinCAT(ImportDataModel inDataModel) :
            base(inDataModel)
        {
            dataModelTwinCAT = inDataModel as ImportDataModelTwinCAT;
        }
        private ImportDataModelTwinCAT dataModelTwinCAT { get; set; }


        private string _PreName;
        public string PreName
        {
            get { return _PreName; }
            set { _PreName = value; }
        }
        private string _Name;
        public override string Name
        {
            get { return _Name; }
            set { _Name = value; }
        }

        private string _ElemType;
        public string ElemType
        {
            get { return _ElemType; }
            set { _ElemType = value; }
        }

        private DataType _Type;
        public DataType Type
        {
            get { return _Type; }
            set { _Type = value; }
        }

        private ImportTypes _ImportDataType;
        public ImportTypes ImportDataType
        {
            get { return _ImportDataType; }
            set { _ImportDataType = value; }
        }

        private uint _Size;
        public uint Size
        {
            get { return _Size; }
            set { _Size = value; }
        }

        private string _strLevel;
        public string strLevel
        {
            get { return _strLevel; }
            set { _strLevel = value; }
        }

        private string _strId;
        public string strId
        {
            get { return _strId; }
            set { _strId = value; }
        }

        public override string TreeName
        {
            get
            {
                return TreeLevel == 0xff ?
                       ImportTagName : Name;
            }
        }
        public string ImportTagName
        {
            get
            {
                return dataModelTwinCAT.getStationName() != "_" ?
                       dataModelTwinCAT.getStationName() + _PreName + _Name : _PreName + _Name;
            }
        }

        public override DataType IconTagType
        {
            get { return _Type; }
        }
        #region IDisposable Members
        public void Dispose()
        {
            if (Children != null && Children.Count > 0)
            {
                foreach (ImportData el in Children)
                {
                    var sl = el as ImportDataTwinCAT;
                    if (sl != null)
                    {
                        sl.Dispose();
                    }
                }
                Children.Clear();
            }
        }
        #endregion
    };

    public class ImportDataModelTwinCAT : ImportDataModel, IDisposable
    {
        public ImportDataModelTwinCAT(GetStationName inGetStationName) :
            base(inGetStationName)
        {
        }
        public override ImportData addImportData()
        {
            ImportDataTwinCAT importData = new ImportDataTwinCAT(this);
            return importData;
        }

        #region IDisposable Members
        public void Dispose()
        {
            foreach (ImportData el in Children)
            {
                ImportDataTwinCAT els7 = el as ImportDataTwinCAT;
                if (els7 != null)
                {
                    els7.Dispose();
                }
            }
            Children.Clear();
            Children = null;
        }
        #endregion
    }
}
