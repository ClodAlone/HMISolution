using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Reflection;
using UFUAModel;
using System.Collections.ObjectModel;
using DriverCodeBase.UI;
using DriverCodeBase.UI.Controls;
using Utilities;

namespace PPI.UI
{
    /// <summary>
    /// Interaction logic for ImportTagsEditorTree.xaml
    /// </summary>
    public enum ImportTypes
    {
        Unknown,
        Standard,
        Array,
        Struct,
    }
        
    public partial class ImportTagsEditorTree : UserControl, IDisposable
    {
        bool alreadyLoaded = false;
        ImportDataModelPPI importDataModel;
        int m_lGlobalID = 0;
        Dictionary<string, List<string>> m_mapUDT;
        public GetStationName readStationName;
        BaseImportTree baseImportTree;        

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
                baseImportTree.SetVisibleButtons();
                baseImportTree.SetFileFilter("sdf files|*.sdf");
                baseImportTree.LoadImportFile = LoadFile;
                MainStack.Children.Add(baseImportTree);

                readStationName = baseImportTree.funcGetStationName();

                DataContext = this;

                m_mapUDT = new Dictionary<string, List<string>>();
            };
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
                List<ImportTag> taglist = new List<ImportTag>();
                Dictionary<string, ImportPrototype> protoMap = new Dictionary<string, ImportPrototype>();
                ObservableCollection<object> notToBeImported = new ObservableCollection<object>();
                foreach (var elemList in list)
                {
                     ImportData elem = elemList as ImportData;
                     if (elem != null && (notToBeImported.IndexOf(elem) < 0))
                     {
                         bool isStructure = elem.Children.Count > 0;
                         ImportData single = elem;

                         // FOGBUGZ 11412 and 11413
                         bool isArray = false;
                         if (single.ArrayDimension > 0)
                         {
                             isStructure = false;
                             isArray = true;
                         }

                         if (single != null)
                         {

                             PPIDynTagSettings sp = new PPIDynTagSettings();

                             if (!sp.ParseAddress(single.Address))
                                 continue;

                             sp.StationName = stationName;

                             // FOGBUGZ 11412 and 11413
                             sp.ArrayDimension = single.ArrayDimension;

                             single.DynAddress = sp.ToString();

                             ImportPrototype proto = null;
                             if (isStructure)
                             {
                                 proto = addPrototype(protoMap, single);
                                 if (proto == null)
                                     continue;
                             }

                             // FOGBUGZ 11412 and 11413
                             string importTagName = single.TreeName.Trim(']');
                             //importTagName = UFUAModel.Helpers.NameValidator.EnsureValidName(importTagName);

                             ImportTag tagtoimport = new ImportTag()
                             {
                                 // FOGBUGZ 11412 and 11413
                                 //Name = single.Name,
                                 Name = importTagName,

                                 DataType = ((ImportDataPPI)single).Type,
                                 DynSettings = single.DynAddress,
                                 Folder = importfolder,
                                 ModelType = (isStructure ? UFUAModel.ModelType.ObjectType : UFUAModel.ModelType.Variable),
                                 Description = single.Description

                                 // FOGBUGZ 11412 and 11413
                                 ,
                                 ArrayDimension = single.ArrayDimension
                                 ,
                                 BehaviorForExistingTags = (BehaviorExistingTagsValues)behaviorExistingTags,
                                 BehaviorForDynamicLink = (BehaviorDynamicLinkValues)behaviorDynamicLink,
                             };
                             if (isStructure)
                             {
                                 tagtoimport.PrototypeModel = proto.Name;
                                 tagtoimport.ModelType = UFUAModel.ModelType.ObjectType;
                             }

                             taglist.Add(tagtoimport);

                             // FOGBUGZ 11412 and 11413
                             if (isArray)
                             {
                                 foreach (ImportData el in elem.Children)
                                 {
                                     if (el != null)
                                     {
                                         notToBeImported.Add(el);
                                     }
                                 }
                             }
                         }
                     }
                 }

                 DataContext = new ImportObject() { PrototypesToImport = protoMap.Values.ToList(), TagsToImport = taglist };
             }
             else
                 DataContext = this;
         }

         private static ImportPrototype addPrototype(Dictionary<string, ImportPrototype> protoMap, ImportData elem)
         {
             ImportPrototype proto = null;
             string protoname;
             if (elem.szType == "STRUCT")
                 protoname = string.Format("prototype_{0}", elem.Name.Trim(']'));
             else
                 protoname = elem.szType;
             if (!protoMap.ContainsKey(protoname))
             {
                 if (elem.Children.Count != 0)
                 {
                     proto = new ImportPrototype();
                     proto.Name = protoname;
                     proto.Elements = new List<ImportTag>();

                     foreach (var t in elem.Children)
                     {
                         ImportData el = t as ImportData;
                         if (el != null)
                         {
                             ImportTag a = new ImportTag();

                             if (((ImportDataPPI)el).ImportType == ImportTypes.Struct)
                             {
                                 ImportPrototype protoElem = addPrototype(protoMap, el);
                                 if (protoElem == null)
                                     continue;

                                 a.ModelType = UFUAModel.ModelType.ObjectType;
                                 a.PrototypeModel = protoElem.Name;
                             }
                             else
                             {
                                 a.ModelType = UFUAModel.ModelType.Variable;
                             }
                             a.ArrayDimension = el.ArrayDimension;
                             a.Name = el.Name;
                             a.DataType = ((ImportDataPPI)el).Type;
                             a.Description = el.Description;
                             proto.Elements.Add(a);
                         }
                     }
                     protoMap.Add(protoname, proto);
                 }
             }
             else
                 proto = protoMap[protoname];
             return proto;
         }

         private ImportDataModel LoadFile(string file)
         {             
             importDataModel = new ImportDataModelPPI(readStationName);
             
             file = file.ToLower();
             if (file.Contains(".awl"))
             {
                 DisplayImportFileAwl(file);
             }
             else if (file.Contains(".sdf"))
             {
                 DisplayImportFileSdf(file);
             }

            return importDataModel;        
         }


         private void DisplayImportFileAwl(string file)
         {
             System.IO.StreamReader readFile = new System.IO.StreamReader(file);
             try
             {
                 ParseDataTypeAWL(ref readFile);

                 PrepareDataTypeMapsAWL();
                 ParseVariableAWL(ref readFile);
             }
             catch (Exception e)
             {
             }
             finally
             {
                 if (readFile != null)
                 {
                     readFile.Close();
                 }
             }
         }

         private void ParseVariableAWL(ref System.IO.StreamReader readFile)
         {
             long ulPos;

             string szDBName = string.Empty;
             string line;
             string szApp;
             string szGlobalName = string.Empty;
             uint nDBNumber = 0;
             uint nGlobalLevel = 0;
             uint nCurrentAddress = 0;
             uint nBitNumber = 0;
             bool bInitialStructFound = false;
             bool bDataDeclarationEndFound = false;
             bool bIncrementAddressIfNotBit = false;
             bool bFirstVarOfStruct = false;

             List<string> slLevelName = new List<string>();
             List<bool> blIsArray = new List<bool>();
             List<bool> blIsStruct = new List<bool>();
             List<uint> lnArrayDim = new List<uint>();
             List<uint> lnArrayIndex = new List<uint>();
             List<long> llInitPosition = new List<long>();

             while ((line = readFile.ReadLine()) != null)
             {
                 int nOf = line.IndexOf(" OF ");
                 int nSlash = line.IndexOf("//");

                 if ((nOf > 0) && (nSlash >= (nOf + 4)))
                 {
                     bool bCommentBreaksLine = false;
                     if (nSlash == (nOf + 4))
                     {
                         bCommentBreaksLine = true;
                     }
                     else
                     {
                         string szBl = line.Substring(nOf + 4, nSlash - nOf - 4);
                         szBl = szBl.Trim();
                         if (szBl.Length == 0)
                         {
                             bCommentBreaksLine = true;
                         }
                     }
                     if (bCommentBreaksLine)
                     {
                         //comment breaks the line!!!
                         szApp = readFile.ReadLine();
                         string szTmp = line.Substring(nSlash);
                         line = line.Substring(0, nSlash);
                         line += szApp + szTmp;
                     }
                 }

                 ulPos = readFile.BaseStream.Position;

                 // Skip empty lines
                 if (line.Length == 0)
                 {
                     continue;
                 }

                 // Check if the line is the beginning of a User Data Type definition
                 // and, in this case, store the UDT definition in the corresponding map
                 /*if (AWLUDTGet(ref readFile, line))
                 {
                     continue;
                 }*/

                 // Search for the number of the Data Block
                 if (szDBName.Length == 0)
                 {
                     AWLGetDBNumber(line, ref szDBName, ref nDBNumber);
                     continue;
                 }

                 // Search for the initial "STRUCT" string
                 if (!bInitialStructFound)
                 {
                     bInitialStructFound = AWLIsTheInitialStructLine(line);
                     nGlobalLevel = 0;

                     // Check if the whole data block is a UDT 
                     if (!bInitialStructFound)
                     {
                         if (AWLUDTFound(line.Replace("  ", string.Empty)))
                             bDataDeclarationEndFound = true;
                     }

                     continue;
                 }
                 // Check if the line is an "end of structure" declaration
                 if (AWLIsEndOfStruct(line))
                 {
                     if (nGlobalLevel == 0)
                     {
                         bDataDeclarationEndFound = true;
                         nBitNumber = 0;
                         if ((nCurrentAddress % 2) > 0)
                             nCurrentAddress++;

                         continue;
                     }

                     nGlobalLevel--;

                     if (bIncrementAddressIfNotBit)
                     {
                         bIncrementAddressIfNotBit = false;
                         nCurrentAddress++;
                     }

                     nBitNumber = 0;
                     if ((nCurrentAddress % 2) > 0)
                         nCurrentAddress++;
                     continue;
                 }

                 // Get the name and the type of the variable
                 string szVarName = string.Empty;
                 string szVarType = string.Empty;
                 string szDescription = string.Empty;

                 bool bIsStruct = false;
                 bool bIsArray = false;
                 uint nArrayDim = 0;
                 uint nStringLen = 0;


                 AWLGetVarNameType(line, ref szVarName, ref szVarType, ref bIsStruct,
                     ref bIsArray, ref nArrayDim, ref szDescription, ref nStringLen);

                 string szAddress;

                 //////////Struct/////////////////////////////////////////////////////////
                 if (bIsStruct)
                 {
                     //Struct or array of struct
                     nGlobalLevel++;

                     //To make the structures word aligned
                     bFirstVarOfStruct = true;

                     if (bIsArray)
                     {
                     }
                     else
                     {
                         AWLGetStructure(ref readFile, szVarName, szDescription, ref nCurrentAddress,
                             ref nDBNumber, ref nBitNumber, ref bIncrementAddressIfNotBit, ref nGlobalLevel);

                     }
                     continue;
                 }


                 if (szVarName.Length == 0 || szVarType.Length == 0)
                     continue;

                 Opc.Ua.BuiltInType nVarType = GetTypeId(szVarType);

                 if (bFirstVarOfStruct)
                 {
                     if (bIncrementAddressIfNotBit)
                     {
                         bIncrementAddressIfNotBit = false;
                         nCurrentAddress++;
                     }

                     nBitNumber = 0;
                     if ((nCurrentAddress % 2) > 0)
                         nCurrentAddress++;
                     bFirstVarOfStruct = false;
                 }


                 //////////////////////////////////////////////////////////////////////////
                 //////////////////Array of simple type//////////////////////////////
                 if (bIsArray && nVarType >= 0)
                 {
                     szAddress = GetAddress(nVarType, nDBNumber, ref nCurrentAddress, ref nBitNumber,
                         ref bIncrementAddressIfNotBit, nStringLen, ref szVarType, false);
                     if (nVarType == Opc.Ua.BuiltInType.String)
                         nCurrentAddress -= 2;
                     AddArray(nDBNumber, ref nCurrentAddress, ref nBitNumber, ref bIncrementAddressIfNotBit,
                         "", szVarName, szAddress, szVarType, szDescription, nStringLen, nArrayDim, ImportTypes.Array);

                 }
                 //////////////////////////////////////////////////////////////////////////
                 /////////////////////Array of UDT/////////////////////////////////////////
                 //else if (bIsArray && nVarType < 0)
                 //{
                 //    for (uint i = 0; i < nArrayDim; i++)
                 //    {
                 //        string szVarTempName;
                 //        szVarTempName = string.Format(szVarComplName + "_{0}", i);

                 //        AWLUDTAddToList(szVarTempName, szVarType, szDescription,
                 //            szGlobalName, nDBNumber, ref nCurrentAddress, ref nBitNumber, ref bIncrementAddressIfNotBit);

                 //    }
                 //}
                 ////////////////////////////UDT/////////////////////////////////////////
                 else if (!bIsArray && nVarType == Opc.Ua.BuiltInType.Null)
                 {
                     AddStruct(nDBNumber, ref nCurrentAddress, ref nBitNumber, ref bIncrementAddressIfNotBit,
                         "", szVarName, szDescription, szVarType);
                     continue;
                     
                     //AWLIncrementCurAddForNotSuppType(ref nCurrentAddress, ref szVarType,
                     //        bIsArray, nArrayDim, ref nBitNumber,
                     //        ref bFirstVarOfStruct,
                     //        ref bIncrementAddressIfNotBit);
                     //continue;
                 }
                 ///////////////////////////////variable///////////////////////////////////
                 else if (!bIsArray)
                 {
                     szAddress = GetAddress(nVarType, nDBNumber, ref nCurrentAddress, ref nBitNumber,
                         ref bIncrementAddressIfNotBit, nStringLen, ref szVarType);

                     if (szAddress.Length == 0)
                         continue;

                     AddStandardVar(nDBNumber, "", szVarName, szAddress, szVarType, szDescription, nStringLen);

                 }

             }
         }


         private void AddArray(uint nDBNumber, ref uint nCurrentAddress, ref uint nBitNumber,
             ref bool bIncrementAddressIfNotBit, String szNamePrefix, string szArrayName,
             string szArrayAddress, string szVarType,string szDescription, 
             uint nStringLen, uint nArrayDim, ImportTypes elementType,
             int lParentID = -1, ImportData inRootItem = null)
         {
             Opc.Ua.BuiltInType nVarType = GetTypeId(szVarType);

             if (nVarType == Opc.Ua.BuiltInType.String || elementType != ImportTypes.Standard)
             {
                 string szTempAddress = string.Empty;
                 InsertArrayMembers(nVarType, nDBNumber, ref nCurrentAddress, ref nBitNumber,
                     ref bIncrementAddressIfNotBit, nArrayDim, nStringLen, ref szVarType, 
                     szArrayName, szDescription, ref szTempAddress,
                     lParentID,inRootItem);
             }
             else
                 InsertSimpleTypeArray(nVarType, nDBNumber, ref nCurrentAddress, ref nBitNumber,
                 ref bIncrementAddressIfNotBit, nArrayDim, 0, ref szVarType, szArrayName, szDescription,
                 lParentID,inRootItem);
         }

         private void AddStandardVar(uint nDBNumber, String szNamePrefix, string szVarName,
             string szAddress,string szVarType, string szDescription, uint nStringLen,
             int lParentID = -1, ImportData inRootItem = null)
         {
             // Add the variable to the dialog list
             DataType nType = DataType.Boolean;
             int nConversion = 0;
             uint nVarSize = 0;
             if (GetMoviconTypeId(szVarType, ref nConversion, ref nVarSize, ref nType))
             {
                 var IVar = importDataModel.addImportData();
                 IVar.parentId = lParentID;
                 if (nDBNumber > 0)
                    ((ImportDataPPI)IVar).PreName = string.Format("DB{0}", nDBNumber);
                 IVar.Name = szNamePrefix + szVarName;
                ((ImportDataPPI)IVar).Type = nType;
                 IVar.szType = szVarType;
                 if (szVarType == "STRING")
                     nVarSize = nStringLen + 2;
                ((ImportDataPPI)IVar).Size = nVarSize;
                ((ImportDataPPI)IVar).ElemType = DataType.Boolean;
                 IVar.Address = szAddress;
                 IVar.Description = szDescription;
                 IVar.Id = m_lGlobalID++;
                 IVar.ArrayDimension = 0;
                ((ImportDataPPI)IVar).ImportType = ImportTypes.Standard;

                 AddTreeItem(IVar, inRootItem);
             }
         }

        private void ParseDataTypeAWL(ref System.IO.StreamReader readFile)
         {
             string line;

             m_mapUDT.Clear();

             while ((line = readFile.ReadLine()) != null)
             {
                 if (AWLUDTGet(ref readFile, line))
                 {
                     continue;
                 }
             }
             readFile.BaseStream.Seek(0, System.IO.SeekOrigin.Begin);
         }
         void PrepareDataTypeMapsAWL()
         {
             Dictionary<string, List<string>> mapOk = new Dictionary<string, List<string>>();
             Dictionary<string, List<string>> mapTemp = new Dictionary<string, List<string>>();
             bool bOk = true;

             foreach (var itemStruct in m_mapUDT)
             {
                 foreach (string itemStructValue in itemStruct.Value)
                     bOk &= (GetStructMemberTypeFromStringAWL(itemStructValue) != ImportTypes.Unknown);
                 if (bOk)
                     mapOk.Add(itemStruct.Key, itemStruct.Value);
                 else
                     mapTemp.Add(itemStruct.Key, itemStruct.Value);
             }
             foreach (var itemTemp in mapTemp)
             {
                 List<string> ValDef = new List<string>();
                 foreach (string itemTempValue in itemTemp.Value)
                 {
                     bool bType = (GetStructMemberTypeFromStringAWL(itemTempValue) != ImportTypes.Unknown);
                     bOk = mapOk.ContainsKey(itemTemp.Key);
                     if (bOk)
                         foreach (string itemOkValue in mapOk[itemTemp.Key])
                             ValDef.Add(itemOkValue);
                     else if (bType)
                         ValDef.Add(itemTempValue);
                 }
                 mapOk.Add(itemTemp.Key, ValDef);

             }
             m_mapUDT = mapOk;
         }
         ImportTypes GetStructMemberTypeFromStringAWL(String szFieldLine)
         {
             // Get the name and the type of the variable
             string szVarName = string.Empty;
             string szVarType = string.Empty;
             string szDescription = string.Empty;

             bool bIsStruct = false;
             bool bIsArray = false;
             uint nArrayDim = 0;
             uint nStringLen = 0;

             AWLGetVarNameType(szFieldLine, ref szVarName, ref szVarType, ref bIsStruct,
                 ref bIsArray, ref nArrayDim, ref szDescription, ref nStringLen);
             
             uint nFieldSize = 0;
             String nMovType = "";

             // Field type?
             ImportTypes nFieldType = ImportTypes.Unknown;
             nFieldType = GetVarTypeAndSizeAWL(szVarType, ref nFieldSize,ref nMovType);

             return nFieldType;
         }
         ImportTypes GetVarTypeAndSizeAWL(string inSzType, ref uint nVarSize, ref String nType)
         {
             ImportTypes exit = ImportTypes.Unknown;
             inSzType = inSzType.Trim();
             string szType = inSzType;
             szType = szType.ToUpper();
             nVarSize = 0;
             nType = "";
             if (szType.Length == 0)
             {
                 return (exit);
             }
             if (szType == "BOOL") 
             {
                 nType = "Boolean";
                 nVarSize = 1;
                 exit = ImportTypes.Standard;
             }
             else if (szType == "BYTE")
             {
                 nType = "Byte";
                 nVarSize = 1;
                 exit = ImportTypes.Standard;
             }
             else if (szType == "WORD"
                 || szType == "COUNTER" 
                 || szType == "S5TIME")
             {
                 nType = "UInt16";
                 nVarSize = 2;
                 exit = ImportTypes.Standard;
             }
             else if (szType == "DWORD"
                 || szType == "TIMER")
             {
                 nType = "UInt32";
                 nVarSize = 4;
                 exit = ImportTypes.Standard;
             }
             else if (szType == "INT")
             {
                 nType = "Int16";
                 nVarSize = 2;
                 exit = ImportTypes.Standard;
             }
             else if (szType == "DINT" )
             {
                 nType = "Int32";
                 nVarSize = 4;
                 exit = ImportTypes.Standard;
             }
             else if (szType == "REAL")
             {
                 nType = "Float";
                 nVarSize = 4;
                 exit = ImportTypes.Standard;
             }
            else if (szType == "CHAR")
            {
                nType = "SByte";
                nVarSize = 1;
                exit = ImportTypes.Standard;
            }
             else if (szType == "STRING")
             {
                 nType = "String";
                 nVarSize = 256;
                 exit = ImportTypes.Standard;
             }
             else if (m_mapUDT.ContainsKey(inSzType))
             {
                 if (m_mapUDT[inSzType].Count != 0)
                 {
                     nType = inSzType;
                     nVarSize = StructSize(inSzType);
                     exit = ImportTypes.Struct;
                 }
             }

             return (exit);
         }
         uint StructSize(string szType)
         {
             uint outVal = 0;
             uint nBitNumber = 0;
             bool bIncrementAddressIfNotBit = false;

             if (m_mapUDT.ContainsKey(szType))
             {
                 if (m_mapUDT[szType].Count != 0)
                 {
                     string nMovType;
                     string szFieldName;
                     string szFieldType;
                     string szFieldDescription;
                     bool bIsStruct;
                     bool bIsArray;
                     uint nArrayDim;
                     uint nStringLen;


                     foreach (string itemStructValue in m_mapUDT[szType])
                     {
                         nMovType = string.Empty;
                         szFieldName = string.Empty;
                         szFieldType = string.Empty;
                         szFieldDescription = string.Empty;
                         bIsStruct = false;
                         bIsArray = false;
                         nArrayDim = 0;
                         nStringLen = 0;

                         AWLGetVarNameType(itemStructValue, ref szFieldName, ref szFieldType, ref bIsStruct,
                         ref bIsArray, ref nArrayDim, ref szFieldDescription, ref nStringLen);

                         GetAddress(GetTypeId(szFieldType), 0, ref outVal, ref nBitNumber,
                             ref bIncrementAddressIfNotBit, nStringLen, ref szFieldType);

                     }
                 }
             }
             if (bIncrementAddressIfNotBit)
                 outVal++;
             return (outVal);
         }
         ImportTypes GetStructMemberTypeFromStringAWL(String szFieldLine, ref uint nFieldSize, ref String nMovType)
         {
             // Field type?
             ImportTypes nFieldType = ImportTypes.Unknown;

             // Get the name and the type of the variable
             string szVarName = string.Empty;
             string szVarType = string.Empty;
             string szDescription = string.Empty;

             bool bIsStruct = false;
             bool bIsArray = false;
             uint nArrayDim = 0;
             uint nStringLen = 0;

             AWLGetVarNameType(szFieldLine, ref szVarName, ref szVarType, ref bIsStruct,
                 ref bIsArray, ref nArrayDim, ref szDescription, ref nStringLen);

             nFieldType = GetVarTypeAndSizeAWL(szVarType, ref nFieldSize, ref nMovType);

             return nFieldType;
         }

         bool AddStruct(uint nDBNumber, ref uint nCurrentAddress, ref uint nBitNumber, ref bool bIncrementAddressIfNotBit,
             String szNamePrefix, String szVarName, String description, 
             String szVarType, int parentId = -1, ImportData inRootItem = null)
         {
             if (!AWLUDTFound(szVarType))
                 return false;

             String szStructNamePrefix = szNamePrefix + szVarName + ".";

             int lParentID = parentId;

             uint nFieldSize = 0;

             if (inRootItem == null)
                 lParentID = -1;

             ImportData rootItem = importDataModel.addImportData();
             rootItem.parentId = lParentID;
             lParentID = m_lGlobalID++;
             rootItem.Id = lParentID;
             ((ImportDataPPI)rootItem).PreName = string.Format("DB{0}", nDBNumber);
             rootItem.Name = szVarName;
             rootItem.szType = szVarType;
            ((ImportDataPPI)rootItem).Type = DataType.Boolean;
             rootItem.Description = description;
            ((ImportDataPPI)rootItem).Size = nFieldSize;
             rootItem.ArrayDimension = 0;

             ImportTypes nFieldType;
             ImportTypes elementType;
             string nMovType;
             string szFieldName;
             string szFieldType;
             string szFieldDescription;
             bool bIsStruct;
             bool bIsArray;
             uint nArrayDim;
             uint nStringLen;

             bool isFirst = true;

             foreach (string itemStructValue in m_mapUDT[szVarType])
             {
                 nMovType = string.Empty;
                 szFieldName = string.Empty;
                 szFieldType = string.Empty;
                 szFieldDescription = string.Empty;
                 bIsStruct = false;
                 bIsArray = false;
                 nArrayDim = 0;
                 nStringLen = 0;
                 
                 AWLGetVarNameType(itemStructValue, ref szFieldName, ref szFieldType, ref bIsStruct,
                 ref bIsArray, ref nArrayDim, ref szFieldDescription, ref nStringLen);

                 if (szFieldName.Length == 0 || szFieldType.Length == 0)
                 {
                     continue;
                 }


                 elementType = GetVarTypeAndSizeAWL(szFieldType, ref nFieldSize, ref nMovType);


                 if (nArrayDim != 0)
                 {
                     nFieldType = ImportTypes.Array;
                 }
                 else
                 {
                     nFieldType = elementType;
                 }

                 String elemDescription = (!string.IsNullOrEmpty(description)) ? description + ", " + szFieldDescription : "";

                 Opc.Ua.BuiltInType nVarType = GetTypeId(szFieldType);
                 string szAddress = GetAddress(nVarType, nDBNumber, ref nCurrentAddress, ref nBitNumber,
                     ref bIncrementAddressIfNotBit, nStringLen, ref szFieldType);
                 

                 // Add the field to the dialog list
                 switch (nFieldType)
                 {
                     // Variable of standard type
                     case ImportTypes.Standard:
                         if (nMovType != "String")
                         {
                             AddStructRoot(inRootItem, rootItem, ref isFirst, szAddress, szVarType);
                             AddStandardVar(nDBNumber, "", szFieldName, 
                                 szAddress, szFieldType, elemDescription, 0, lParentID, rootItem);
                         }
                         else
                         {
                             AddStandardVar(nDBNumber, szStructNamePrefix, szFieldName, 
                                 szAddress, szFieldType, elemDescription, nFieldSize);
                         }
                         break;
                     // Array
                     case ImportTypes.Array:

                         if (elementType == ImportTypes.Standard)
                         {
                             AddStructRoot(inRootItem, rootItem, ref isFirst, szAddress, szVarType);
                             AddArray(nDBNumber, ref nCurrentAddress, ref nBitNumber, ref bIncrementAddressIfNotBit,
                                 "", szFieldName, szAddress, szFieldType, elemDescription, nFieldSize, 
                                 nArrayDim, elementType, lParentID, rootItem);

                         }
                         else
                         {
                             AddArray(nDBNumber, ref nCurrentAddress, ref nBitNumber, ref bIncrementAddressIfNotBit,
                                 szStructNamePrefix, szFieldName, szAddress, szFieldType,
                                  elemDescription, nFieldSize, nArrayDim, elementType);
                         }
                         break;
                     // Structure 
                     case ImportTypes.Struct:
                         if (AddStruct(nDBNumber, ref nCurrentAddress, ref nBitNumber, ref bIncrementAddressIfNotBit,
                             szStructNamePrefix, szFieldName, elemDescription, szFieldType, lParentID, rootItem))
                             AddStructRoot(inRootItem, rootItem, ref isFirst, szAddress, szVarType);


                         break;
                 }
             }

             if (bIncrementAddressIfNotBit)
             {
                 nCurrentAddress++;
                 bIncrementAddressIfNotBit = false;
             }
             nCurrentAddress += (nCurrentAddress % 2);

             return !isFirst;
         }

         private void AddStructRoot(ImportData inRootItem, ImportData rootItem, ref bool isFirst, string szAddress, string szVarType)
         {
             if (isFirst)
             {
                 isFirst = false;
                 rootItem.Address = szAddress;
                 ((ImportDataPPI)rootItem).ImportType = ImportTypes.Struct;
                 rootItem.szType = szVarType;
                 if (rootItem.parentId < 0)
                 {
                     AddTreeItem(rootItem);
                 }
                 else
                 {
                     AddTreeItem(rootItem, inRootItem);
                 }
             }
         }

         bool AWLGetStructure(ref System.IO.StreamReader awlFile, string structName, string description,
             ref uint nCurrentAddress, ref uint nDBNumber, ref uint nBitNumber, ref bool bIncrementAddressIfNotBit,
             ref uint nGlobalLevel)
         {
             if (structName.Length == 0)
                 return false;

             var structure = importDataModel.addImportData();
             uint nVarSize = 0;
             if (nDBNumber > 0)
                 ((ImportDataPPI)structure).PreName = string.Format("DB{0}", nDBNumber);//https://support/Products/default.asp?7797
             structure.Name = structName;
            ((ImportDataPPI)structure).Type = DataType.Boolean;

             structure.szType = "STRUCT";

            ((ImportDataPPI)structure).Size = nVarSize;
            ((ImportDataPPI)structure).ElemType = DataType.Boolean;
             structure.Address = string.Empty;
             structure.Description = description;
             structure.Id = m_lGlobalID++;
            ((ImportDataPPI)structure).ImportType = ImportTypes.Struct;

             // FOGBUGZ 11412 and 11413
             structure.ArrayDimension = 0;

             bool firstelement = true;


             string szFileRecord;
             while (((szFileRecord = awlFile.ReadLine()) != null))
             {
                 if (!AWLIsEndOfStruct(szFileRecord))
                 {
                     szFileRecord = szFileRecord.Trim();
                     if (szFileRecord.Length != 0)
                     {
                         //add member
                         string szVarName = string.Empty;
                         string szVarType = string.Empty;
                         string szDescription = string.Empty;

                         bool bIsStruct = false;
                         bool bIsArray = false;
                         uint nArrayDim = 0;
                         uint nStringLen = 0;
                         AWLGetVarNameType(szFileRecord, ref szVarName, ref szVarType, ref bIsStruct,
                         ref bIsArray, ref nArrayDim, ref szDescription, ref nStringLen);

                         string szAddress;
                         bool bFirstVarOfStruct = false;
                         if (bIsStruct)
                         {
                             //Struct or array of struct
                             //To make the structures word aligned
                             bFirstVarOfStruct = true;

                             if (bIsArray)
                             {
                             }
                             else
                             {
                                 AWLGetStructure(ref awlFile, szVarName, szDescription, ref nCurrentAddress,
                                     ref nDBNumber, ref nBitNumber, ref bIncrementAddressIfNotBit, ref nGlobalLevel);

                             }
                             continue;
                         }
                         if (szVarName.Length == 0 || szVarType.Length == 0)
                             continue;

                         Opc.Ua.BuiltInType nVarType = GetTypeId(szVarType);

                         if (bIsArray && nVarType >= 0)
                         {
                             // FOGBUGZ 11412 and 11413
                             //if (nVarType == Opc.Ua.BuiltInType.Boolean || nVarType == Opc.Ua.BuiltInType.String)
                             if (nVarType == Opc.Ua.BuiltInType.String)

                             {
                                 string szTempAddress = string.Empty;
                                 InsertArrayMembers(nVarType, nDBNumber, ref nCurrentAddress, ref nBitNumber,
                                     ref bIncrementAddressIfNotBit, nArrayDim, nStringLen, ref szVarType, szVarName, szDescription, ref szTempAddress);
                             }
                             else
                                 InsertSimpleTypeArray(nVarType, nDBNumber, ref nCurrentAddress, ref nBitNumber,
                                 ref bIncrementAddressIfNotBit, nArrayDim, nStringLen, ref szVarType, szVarName, szDescription);

                         }
                         else if (!bIsArray)
                         {
                             if (bFirstVarOfStruct)
                             {
                                 if (bIncrementAddressIfNotBit)
                                 {
                                     bIncrementAddressIfNotBit = false;
                                     nCurrentAddress++;
                                 }

                                 nBitNumber = 0;
                                 if ((nCurrentAddress % 2) > 0)
                                     nCurrentAddress++;
                                 bFirstVarOfStruct = false;
                             }

                             szAddress = GetAddress(nVarType, nDBNumber, ref nCurrentAddress, ref nBitNumber,
                                 ref bIncrementAddressIfNotBit, nStringLen, ref szVarType);

                             if (szAddress.Length == 0)
                                 continue;

                             // Add the variable to the dialog list
                             DataType nType = DataType.Boolean;
                             int nConv = 0;
                             uint nSize = 0;
                             if (GetMoviconTypeId(szVarType, ref nConv, ref nSize, ref nType))
                             {
                                 var IVar = importDataModel.addImportData();
                                 if (nDBNumber > 0)
                                    ((ImportDataPPI)IVar).PreName = string.Format("DB{0}", nDBNumber);//https://support/Products/default.asp?7797
                                 IVar.Name = szVarName;
                                ((ImportDataPPI)IVar).Type = nType;

                                 IVar.szType = szVarType;
                                 if (szVarType == "STRING")
                                     nVarSize = nStringLen + 2;
                                ((ImportDataPPI)IVar).Size = nVarSize;
                                ((ImportDataPPI)IVar).ElemType = DataType.Boolean;
                                 IVar.Address = szAddress;
                                 IVar.Description = szDescription;
                                 IVar.Id = m_lGlobalID++;
                                ((ImportDataPPI)IVar).ImportType = ImportTypes.Standard;

                                 // FOGBUGZ 11412 and 11413
                                 IVar.ArrayDimension = 0;

                                 if (firstelement)
                                 {
                                     firstelement = false;
                                     structure.Address = szAddress;
                                     AddTreeItem(structure);
                                 }
                                 AddTreeItem(IVar, structure);
                             }

                         }
                     }
                 }
                 else
                 {
                     if (nGlobalLevel == 0)
                     {
                         nBitNumber = 0;
                         if ((nCurrentAddress % 2) > 0)
                             nCurrentAddress++;

                     }
                     nGlobalLevel--;
                     if (bIncrementAddressIfNotBit)
                     {
                         bIncrementAddressIfNotBit = false;
                         nCurrentAddress++;
                     }

                     nBitNumber = 0;
                     if ((nCurrentAddress % 2) > 0)
                         nCurrentAddress++;
                     break;
                 }
             }


             return true;
         }
         
//**********************************************************************
         internal void AddTreeItem(ImportData tag, ImportData parent = null)
         {
             tag.Parent = parent;
             if (parent == null)
             {
                 tag.TreeLevel = 0xFF;
                 string strLevel = tag.TreeLevel.ToString("X2");
                 tag.parentId = -1;                 
                 importDataModel.Children.Add(tag);
             }
             else
             {
                 tag.TreeLevel = parent.TreeLevel - 1;
                 string strLevel = tag.TreeLevel.ToString("X2");
                 tag.parentId = parent.Id;                 
                 parent.Children.Add(tag);
             }
         }

         void AWLIncrementCurAddForNotSuppType(ref uint nCurrentAddress,
                                                       ref string szVarType,
                                                       bool bIsArray,
                                                       uint nArrayDim,
                                                       ref uint nBitNumber,
                                                       ref bool bFirstVarOfStruct,
                                                       ref bool bIncrementAddressIfNotBit)
         {
             szVarType = szVarType.Trim().ToUpper();
             if (szVarType.Length == 0)
             {
                 return;
             }

             uint nVarDim = 1; // Default
             if (szVarType == "TIME")
             {
                 nVarDim = 4;
             }
             else if (szVarType == "DATE")
             {
                 nVarDim = 2;
             }
             else if (szVarType == "TIME_OF_DAY")
             {
                 nVarDim = 4;
             }
             else if (szVarType == "DATE_AND_TIME")
             {
                 nVarDim = 8;
             }
             else if (szVarType == "STRING")
             {
                 nVarDim = 2 + nArrayDim;
             }

             if (!bIsArray)
             {
                 //variable

                 //Added in version 10.0.0.17
                 if (bFirstVarOfStruct)
                 {
                     if (bIncrementAddressIfNotBit)
                     {
                         bIncrementAddressIfNotBit = false;
                         nCurrentAddress++;
                     }

                     nBitNumber = 0;
                     if ((nCurrentAddress % 2) > 0)
                     {
                         nCurrentAddress++;
                     }
                     bFirstVarOfStruct = false;
                 }
             }
             else
             {
                 nVarDim *= nArrayDim;
             }

             if (bIncrementAddressIfNotBit)
             {
                 nCurrentAddress++;
                 bIncrementAddressIfNotBit = false;
             }

             //May be unaligned even if the previous was a byte var
             if ((nCurrentAddress % 2) > 0)
             {
                 nCurrentAddress++;
             }

             nCurrentAddress += nVarDim;

             nBitNumber = 0;

             if (bIsArray)
             {
                 if ((nCurrentAddress % 2) > 0)
                 {
                     nCurrentAddress++;
                 }
             }

             return;
         }

         //string GetAddress(Opc.Ua.BuiltInType nVarType, uint nDBNumber, ref uint nCurrentAddress, ref uint nBitNumber,
         //                        ref bool bIncrementAddressIfNotBit, uint nStringLen, ref string szVarType)
         string GetAddress(Opc.Ua.BuiltInType nVarType, uint nDBNumber, ref uint nCurrentAddress, ref uint nBitNumber,
                                 ref bool bIncrementAddressIfNotBit, uint nStringLen, ref string szVarType, bool incrementCurrentAddress=true)
         {

             string szAddress = string.Empty;

             switch (nVarType)
             {
                 case Opc.Ua.BuiltInType.Boolean:
                     szAddress = string.Format("DB{0}.DBX{1}.{2}", nDBNumber, nCurrentAddress, nBitNumber);
                     if (incrementCurrentAddress)
                     {
                         nBitNumber++;
                         if (nBitNumber > 7)
                         {
                             nBitNumber = 0;
                             nCurrentAddress++;
                             bIncrementAddressIfNotBit = false;
                         }
                         else
                         {
                             bIncrementAddressIfNotBit = true;
                         }
                     }

                     break;
                 case Opc.Ua.BuiltInType.Byte:
                 case Opc.Ua.BuiltInType.SByte:
                     if (bIncrementAddressIfNotBit)
                     {
                         nCurrentAddress++;
                         bIncrementAddressIfNotBit = false;
                     }
                     szAddress = string.Format("DB{0}.DBB{1}", nDBNumber, nCurrentAddress);
                     if (incrementCurrentAddress)
                     {
                         nCurrentAddress++;
                     }
                     nBitNumber = 0;
                     break;
                 case Opc.Ua.BuiltInType.UInt16:
                 case Opc.Ua.BuiltInType.Int16:
                     if (bIncrementAddressIfNotBit)
                     {
                         nCurrentAddress++;
                         bIncrementAddressIfNotBit = false;
                     }
                     if ((nCurrentAddress % 2) > 0)
                     {
                         nCurrentAddress++;
                     }
                     szAddress = string.Format("DB{0}.DBW{1}", nDBNumber,
                     nCurrentAddress);
                     if (incrementCurrentAddress)
                     {
                         nCurrentAddress += 2;
                     }
                     nBitNumber = 0;
                     break;
                 case Opc.Ua.BuiltInType.UInt32:
                 case Opc.Ua.BuiltInType.Int32:
                 case Opc.Ua.BuiltInType.Float:
                     if (bIncrementAddressIfNotBit)
                     {
                         nCurrentAddress++;
                         bIncrementAddressIfNotBit = false;
                     }
                     if ((nCurrentAddress % 2) > 0)
                     {
                         nCurrentAddress++;
                     }
                     szAddress = string.Format("DB{0}.DBD{1}", nDBNumber,
                     nCurrentAddress);
                     if (incrementCurrentAddress)
                     {
                         nCurrentAddress += 4;
                     }
                     nBitNumber = 0;
                     break;
                 case Opc.Ua.BuiltInType.String:
                     if (bIncrementAddressIfNotBit)
                     {
                         nCurrentAddress++;
                         bIncrementAddressIfNotBit = false;
                     }
                     //Added in version 10.0.0.20
                     //May be unaligned even if the previous was a byte var
                     if ((nCurrentAddress % 2) > 0)
                     {
                         nCurrentAddress++;
                     }

                     nCurrentAddress += 2;

                     szAddress = string.Format("DB{0}.DBB{1}:{2}", nDBNumber,
                     nCurrentAddress, nStringLen);

                     if (incrementCurrentAddress)
                     {
                         nCurrentAddress += nStringLen;
                     }

                     nBitNumber = 0;
                     szVarType = "STRING";
                     break;
                 default:
                     if (AWLUDTFound(szVarType))
                     {
                         if (bIncrementAddressIfNotBit)
                         {
                             nCurrentAddress++;
                             bIncrementAddressIfNotBit = false;
                         }
                         nCurrentAddress += (nCurrentAddress % 2);

                         szAddress = string.Format("DB{0}.DBD{1}", nDBNumber,
                         nCurrentAddress);
                         if (incrementCurrentAddress)
                         {
                             nCurrentAddress += StructSize(szVarType);
                         }
                     }
                     break;

             }
             return szAddress;
         }


         bool AWLUDTGet(ref System.IO.StreamReader fileAWL, string szLine)
         {
             // Check if the line is the beginning of a UDT declaration
             int nIndex = szLine.IndexOf("TYPE");
             if (nIndex < 0)
             {
                 return (false);
             }
             if (szLine.Length <= (4 + nIndex))
             {
                 return (false);
             }

             // Get the UDT name
             string szUDTName = szLine.Substring(5 + nIndex);
             szUDTName = szUDTName.Trim();
             RemoveBlanks(ref szUDTName);
             if (szUDTName.Length == 0)
             {
                 return (false);
             }

             // Save the current position in the file
             // (just in case something goes wrong)
             long ulFirstFilePos = fileAWL.BaseStream.Position;

             // Store the UDT definition in a String Array
             List<string> pStringArray = new List<string>();

             string szFileRecord;
             bool bEndOfUDTFound = false;
             bool bInitialStructFound = false;
             bool bDefinitionComplete = false;
             while (!bEndOfUDTFound && ((szFileRecord = fileAWL.ReadLine()) != null))
             {
                 if (!AWLUDTIsEnd(szFileRecord))
                 {
                     if (!bInitialStructFound)
                     {
                         bInitialStructFound = AWLIsTheInitialStructLine(szFileRecord);
                         continue;

                     }
                     else
                     {
                         if (!bDefinitionComplete)
                         {
                             bDefinitionComplete = AWLIsEndOfStruct(szFileRecord);
                             if (!bDefinitionComplete)
                             {
                                 szFileRecord = szFileRecord.Trim();
                                 if (szFileRecord.Length != 0)
                                 {
                                     pStringArray.Add(szFileRecord);
                                 }
                             }
                         }
                     }
                 }
                 else
                 {
                     bEndOfUDTFound = true;
                 }
             }

             // Check if the UDT definition has been correctly read
             if (!bEndOfUDTFound || !bDefinitionComplete || pStringArray.Count == 0)
             {
                 pStringArray.Clear();
                 fileAWL.BaseStream.Seek(ulFirstFilePos, System.IO.SeekOrigin.Begin);
                 return (false);
             }

             // Add the UDT definition to the corresponding map
             m_mapUDT.Add(szUDTName, pStringArray);

             return (true);
         }

         bool AWLUDTIsEnd(string szLine)
         {
             // Search for the string "END_TYPE"
             int nIndex = szLine.IndexOf("END_TYPE");
             if (nIndex >= 0)
             {
                 return true;
             }

             return (false);
         }

         void AWLGetDBNumber(string szLine, ref string szDBName,
                                    ref uint nDBNumber)
         {
             // Init the output parameters
             szDBName = string.Empty;
             nDBNumber = 0;

             // Skip empty lines
             if (szLine.Length == 0)
             {
                 return;
             }

             // Search for the string "DATA_BLOCK"
             int nIndex = szLine.IndexOf("DATA_BLOCK");

             // String "DATA_BLOCK" not found?
             if (nIndex < 0)
             {
                 return;
             }

             // Name of the data block not present?
             if ((nIndex + 10) > (szLine.Length - 1))
             {
                 return;
             }

             string szAux = szLine.Substring(nIndex + 10);

             // Search for the string "DB"
             nIndex = szAux.IndexOf("DB");

             if ((nIndex >= 0) && ((nIndex + 2) <= (szAux.Length - 1)))
             {
                 string szAux2 = szAux.Substring(nIndex + 2);
                 szAux2 = szAux2.Trim();
                 if (szAux2.Length != 0)
                 {
                     // Get the Data Block number
                     int nDBNum = Convert.ToInt16(szAux2);

                     if (nDBNum > 0)
                     {
                         // Set the output parameters: Data Block name and number
                         szDBName = string.Format("DB{0}", nDBNum);
                         nDBNumber = (uint)nDBNum;
                     }
                 }
             }
         }

         bool AWLIsTheInitialStructLine(string szLine)
         {
             // Search for the string "STRUCT"
             if (szLine.IndexOf("STRUCT") >= 0)
                 return true;

             return false;
         }

         bool AWLUDTFound(string szType)
         {
             string szVarType = szType.Trim();
             if (szVarType.Length == 0)
                 return (false);

             if (m_mapUDT.ContainsKey(szVarType))
             {
                 if (m_mapUDT[szVarType].Count == 0)
                     return (false);
                 return (true);
             }

             return (false);
         }

         bool AWLIsEndOfStruct(string szLine)
         {
             // Search for the string "END_STRUCT"
             if (szLine.IndexOf("END_STRUCT") >= 0)
             {
                 return true;
             }

             return false;
         }

         void RemoveStructName(ref string szStructName, ref string szCurStructName)
         {
             if (szStructName.Length == 0)
             {
                 szCurStructName = string.Empty;
                 return;
             }

             if (szCurStructName.Length == 0)
             {
                 szStructName = string.Empty;
                 return;
             }

             int nIndex = szStructName.IndexOf(szCurStructName);
             if (nIndex <= 0)
             {
                 szCurStructName = string.Empty;
                 szStructName = string.Empty;
                 return;
             }

             string szAux = szStructName.Substring(0, nIndex - 1);
             szStructName = szAux;
             szCurStructName = string.Empty;
         }

         void AWLGetVarNameType(string szLine, ref string szVarName, ref string szVarType,
             ref bool bIsStruct, ref bool bIsArray, ref uint nArrayDim, ref string szDesc, ref uint nStringLen)
         {
             // Init the output parameters
             szVarName = string.Empty;
             szVarType = string.Empty;
             bIsStruct = false;
             bIsArray = false;
             nArrayDim = 0;

             int nCom = szLine.IndexOf("//");
             int nNewLine = szLine.IndexOf("\n");
             if (nCom > 0 && nNewLine > nCom)
             {
                 szDesc = szLine.Substring(nCom + 2, nNewLine - nCom - 2);
                 szLine = szLine.Substring(0, nCom) + szLine.Substring(nNewLine + 1);
             }
             else if (nCom > 0)
             {
                 szDesc = szLine.Substring(nCom + 2);
                 szLine = szLine.Substring(0, nCom);
             }
             else
             {
                 szDesc = string.Empty;
             }

             // Search for the character ':'
             int nIndex = szLine.IndexOf(":");
             if ((nIndex > 0) && (nIndex < (szLine.Length - 1)))
             {
                 // Get the variable name
                 szVarName = szLine.Substring(0, nIndex);
                 RemoveBlanks(ref szVarName);

                 string szAux = szLine.Substring(nIndex + 1);

                 //Decide the type of item
                 bIsArray = (szAux.IndexOf("ARRAY") >= 0);
                 bIsStruct = (szAux.IndexOf("STRUCT") >= 0);

                 if (bIsStruct && bIsArray)
                 {
                     //Array of structures
                     szVarType = string.Empty;

                     // Get the ARRAY elements number and type
                     nIndex = szAux.IndexOf("[");
                     if ((nIndex >= 0) && (nIndex < (szAux.Length - 1)))
                     {
                         string szAux2 = szAux.Substring(nIndex + 1);
                         nIndex = szAux2.IndexOf("..");
                         if ((nIndex > 0) && (nIndex < (szAux.Length - 2)))
                         {
                             szAux = szAux2.Substring(0, nIndex).Trim();
                             uint nLowLimit = 0;
                             if (szAux.Length != 0)
                             {
                                 nLowLimit = Convert.ToUInt16(szAux);
                                 szAux = szAux2.Substring(nIndex + 2);
                                 nIndex = szAux.IndexOf("]");
                                 if (nIndex > 0)
                                 {
                                     szAux2 = szAux.Substring(0, nIndex).Trim();
                                     uint nHighLimit = nLowLimit;
                                     if (szAux2.Length != 0)
                                     {
                                         nHighLimit = Convert.ToUInt16(szAux2);
                                         if (nHighLimit > nLowLimit)
                                         {
                                             nIndex = szAux.IndexOf(" OF ");
                                             if ((nIndex > 0) &&
                                                 (nIndex < (szAux.Length - 4)))
                                             {
                                                 szAux2 = szAux.Substring(nIndex + 4).Trim();
                                                 szAux = szAux2.Substring(0, nIndex).Trim();
                                                 if (szAux.Length != 0)
                                                 {
                                                     // Set the type string
                                                     szVarType = szAux;
                                                     if (szVarType == "STRING")
                                                     {//look for string size
                                                         szAux2 = szAux2.Trim();
                                                         int nIndex1 = szAux2.IndexOf("[");
                                                         int nIndex2 = szAux2.IndexOf("]");
                                                         if (nIndex1 != -1 && nIndex1 != -1)
                                                         {
                                                             szAux = szAux2.Substring(nIndex1 + 1, nIndex2 - nIndex1 - 1);
                                                             if (szAux.Length != 0)
                                                                 nStringLen = Convert.ToUInt16(szAux);
                                                         }
                                                     }
                                                     // Set the array size
                                                     nArrayDim = nHighLimit - nLowLimit + 1;
                                                 }
                                             }
                                         }
                                     }
                                 }
                             }
                         }
                     }

                 }
                 else if (bIsArray)
                 {
                     //Array of variables
                     // Reset the type string
                     szVarType = string.Empty;

                     // Get the ARRAY elements number and type
                     nIndex = szAux.IndexOf("[");
                     if ((nIndex >= 0) && (nIndex < (szAux.Length - 1)))
                     {
                         string szAux2 = szAux.Substring(nIndex + 1);
                         nIndex = szAux2.IndexOf("..");
                         if ((nIndex > 0) && (nIndex < (szAux.Length - 2)))
                         {
                             szAux = szAux2.Substring(0, nIndex).Trim();
                             uint nLowLimit = 0;
                             if (szAux.Length != 0)
                             {
                                 nLowLimit = Convert.ToUInt16(szAux);
                                 szAux = szAux2.Substring(nIndex + 2);
                                 nIndex = szAux.IndexOf("]");
                                 if (nIndex > 0)
                                 {
                                     szAux2 = szAux.Substring(0, nIndex).Trim();
                                     uint nHighLimit = nLowLimit;
                                     if (szAux2.Length != 0)
                                     {
                                         nHighLimit = Convert.ToUInt16(szAux2);
                                         if (nHighLimit > nLowLimit)
                                         {
                                             nIndex = szAux.IndexOf(" OF ");
                                             if ((nIndex > 0) &&
                                                 (nIndex < (szAux.Length - 4)))
                                             {
                                                 szAux2 = szAux.Substring(nIndex + 4).Trim();
                                                 nIndex = szAux2.IndexOf(";"); //https://support.progea.com/Products/default.asp?7539
                                                 if (nIndex > 0)
                                                 {
                                                     int nIndex2p = szAux2.IndexOf(":=");
                                                     if ((nIndex2p > 0) && (nIndex2p < nIndex))
                                                     {
                                                         nIndex = nIndex2p;
                                                     }

                                                     szAux = szAux2.Substring(0, nIndex).Trim();
                                                     if (szAux.Length != 0)
                                                     {
                                                         //////////////////////////////////////////////////////////////////////////
                                                         //https://support.progea.com/Products/default.asp?7575
                                                         if (szAux.IndexOf("STRING") == 0)
                                                         {
                                                             // Set the type string
                                                             szVarType = "STRING";
                                                             //////////////////////////////////////////////////////////////////////////
                                                             //look for string size
                                                             szAux2 = szAux2.Trim();
                                                             int nIndex1 = szAux2.IndexOf("[");
                                                             int nIndex2 = szAux2.IndexOf("]");
                                                             if (nIndex1 != -1 && nIndex1 != -1)
                                                             {
                                                                 szAux = szAux2.Substring(nIndex1 + 1, nIndex2 - nIndex1 - 1);
                                                                 if (szAux.Length != 0)
                                                                     nStringLen = Convert.ToUInt16(szAux);
                                                             }
                                                         }
                                                         //////////////////////////////////////////////////////////////////////////
                                                         //https://support.progea.com/Products/default.asp?7575
                                                         else //array of UDT?
                                                             szVarType = szAux;
                                                         //////////////////////////////////////////////////////////////////////////

                                                         // Set the array size
                                                         nArrayDim = nHighLimit - nLowLimit + 1;
                                                     }
                                                 }
                                             }
                                         }
                                     }
                                 }
                             }
                         }
                     }
                 }
                 else if (bIsStruct)
                 {
                     //Structure definition
                     szVarType = szAux;
                     RemoveBlanks(ref szVarType);
                 }
                 else
                 {
                     //Variable
                     nArrayDim = 0;
                     nIndex = szAux.IndexOf("[");
                     int nEndArray = szAux.IndexOf("]");
                     if (nIndex > 0 && nEndArray > nIndex)
                     {
                         //got array dimension
                         string szDim = szAux.Substring(nIndex + 1, nEndArray - nIndex - 1);
                         RemoveBlanks(ref szDim);
                         nArrayDim = Convert.ToUInt16(szDim);
                     }
                     else
                     {
                         nIndex = szAux.IndexOf(";");
                     }
                     //get the variable type
                     if (nIndex > 0)
                     {
                         szVarType = szAux.Substring(0, nIndex);

                         szAux = szVarType;
                         nIndex = szAux.IndexOf(":");
                         if (nIndex > 0)
                         {
                             szVarType = szAux.Substring(0, nIndex);
                         }

                         szVarType = szVarType.Trim();
                         if (szVarType == "STRING")
                         {
                             nStringLen = nArrayDim;
                             nArrayDim = 0;
                         }
                     }
                 }
                 // Check if the type is ARRAY
                 if (szVarType == "ARRAY")
                 {
                     bIsArray = true;
                 }

                 // Check if the type is STRUCT
                 else if (szVarType == "STRUCT")
                 {
                     bIsStruct = true;
                 }
             }
             RemoveBlanks(ref szVarType);
         }

         void RemoveBlanks(ref string str)
         {
             str = str.Replace(" ", string.Empty);
         }

         Opc.Ua.BuiltInType GetTypeId(string szType)
         {
             Opc.Ua.BuiltInType nType = Opc.Ua.BuiltInType.Null;
             szType = szType.Trim();
             if (szType.Length == 0)
             {
                 return (nType);
             }

             szType = szType.ToUpper();
             if (szType == "BOOL")
             {
                 nType = Opc.Ua.BuiltInType.Boolean;
             }
             else if (szType == "BYTE")
             {
                 nType = Opc.Ua.BuiltInType.Byte;
             }
             else if (szType == "WORD"
                 || szType == "COUNTER")
             {
                 nType = Opc.Ua.BuiltInType.UInt16;
             }
             else if (szType == "S5TIME")
             {
                 nType = Opc.Ua.BuiltInType.UInt16;
             }
             else if (szType == "DWORD"
                 || szType == "TIMER")
             {
                 nType = Opc.Ua.BuiltInType.UInt32;
             }
             else if (szType == "INT")
             {
                 nType = Opc.Ua.BuiltInType.Int16;
             }
             else if (szType == "DINT")
             {
                 nType = Opc.Ua.BuiltInType.Int32;
             }
             else if (szType == "REAL")
             {
                 nType = Opc.Ua.BuiltInType.Float;
             }
             else if (szType == "CHAR")
             {
                 nType = Opc.Ua.BuiltInType.SByte;
             }
             else if (szType == "STRING")
             {
                 nType = Opc.Ua.BuiltInType.String;
             }

             return (nType);
         }

         void InsertArrayMembers(Opc.Ua.BuiltInType nVarType, uint nDBNumber, ref uint nCurrentAddress, ref uint nBitNumber,
             ref bool bIncrementAddressIfNotBit, uint nArrayDim, uint nStringLen, ref string szVarType,
             string szVarName, string szDescription, ref string szFirstVarAddress,
             int lParentID = -1, ImportData inRootItem = null)
         {

             // Case "array after a BOOL variable": the address must be
             // increased and the bit number must be reset
             nBitNumber = 0;
             if (bIncrementAddressIfNotBit)
             {
                 nCurrentAddress++;
                 bIncrementAddressIfNotBit = false;
             }

             if ((nCurrentAddress % 2) > 0)
                 nCurrentAddress++;

             string szFieldLine;
             uint i = 0;
             int nConversion = 0;
             uint nVarSize = 0;

             DataType nElemType = DataType.Boolean;
             if (!GetMoviconTypeId(szVarType, ref nConversion, ref nVarSize, ref nElemType))
                 return;
             for (i = 0; i < nArrayDim; i++)
             {
                 var f = importDataModel.addImportData();
                 szFieldLine = string.Format("{0}[{1}]", szVarName, i);
                 if (nDBNumber > 0)
                    ((ImportDataPPI)f).PreName = string.Format("DB{0}", nDBNumber);//https://support/Products/default.asp?7797
                 f.Name = szFieldLine;

                 if (szVarType == "STRING")
                     nVarSize = nStringLen + 2;

                ((ImportDataPPI)f).Size = nVarSize;
                 f.szType = szVarType;
                ((ImportDataPPI)f).Type = nElemType;
                 f.Address = GetAddress(nVarType, nDBNumber, ref nCurrentAddress, ref nBitNumber,
                     ref bIncrementAddressIfNotBit, nStringLen, ref szVarType);
                 if (i == 0)
                     szFirstVarAddress = f.Address;

                 f.Description = szDescription;
                 f.Id = m_lGlobalID++;

                 // FOGBUGZ 11412 and 11413
                 f.ArrayDimension = 0;

                 f.parentId = lParentID;
                 ((ImportDataPPI)f).ImportType = ImportTypes.Standard;

                 AddTreeItem(f,inRootItem);

             }
             //Last iteration? --> Set to 0 the bit number and
             // increment the byte address
             if (nVarType == Opc.Ua.BuiltInType.Boolean && nBitNumber != 0)
             {
                 nBitNumber = 0;
                 nCurrentAddress++;
                 bIncrementAddressIfNotBit = false;
             }

             if ((nCurrentAddress % 2) > 0)
                 nCurrentAddress++;
         }



         void InsertSimpleTypeArray(Opc.Ua.BuiltInType nVarType, 
             uint nDBNumber, ref uint nCurrentAddress, ref uint nBitNumber,ref bool bIncrementAddressIfNotBit,
             uint nArrayDim, uint nStringLen, ref string szVarType, string szVarComplName, string szDescription,
             int lParentID = -1, ImportData inRootItem = null)
         {
             string szAddress;

             // Case "array after a BOOL variable": the address must be
             // increased and the bit number must be reset
             nBitNumber = 0;
             if (bIncrementAddressIfNotBit)
             {
                 nCurrentAddress++;
                 bIncrementAddressIfNotBit = false;
             }

             if ((nCurrentAddress % 2) > 0)
                 nCurrentAddress++;

             // Added in version 10.1.0.11, FOGBUGZ 8283: errors importing arrays
             uint nCurrentAddressLastValue = nCurrentAddress;

             szAddress = GetAddress(nVarType, nDBNumber, ref nCurrentAddress, ref nBitNumber,
                  ref bIncrementAddressIfNotBit, nStringLen, ref szVarType);
             nBitNumber--;

             // Added in version 10.1.0.11, FOGBUGZ 8283: errors importing arrays
             nCurrentAddress = nCurrentAddressLastValue;

             DataType nType = DataType.Boolean;
             int nConversion = 0;
             uint nVarSize = 0;
             if (!GetMoviconTypeId(szVarType, ref nConversion, ref nVarSize, ref nType))
                 return;
             var IVar = importDataModel.addImportData();
             if (szVarType == "STRING")
                 nVarSize = nStringLen + 2;

             if (nDBNumber > 0)
                ((ImportDataPPI)IVar).PreName = string.Format("DB{0}", nDBNumber);//https://support/Products/default.asp?7797
             IVar.Name = szVarComplName;
             ((ImportDataPPI)IVar).ElemType = nType;

             // FOGBUGZ 11412 and 11413
             //IVar.Type = DataType.Boolean;
             ((ImportDataPPI)IVar).Type = nType;

             ((ImportDataPPI)IVar).Size = nArrayDim * nVarSize;
             IVar.szType = "ARRAY OF " + szVarType;
             IVar.Address = szAddress;
             IVar.Description = szDescription;
             IVar.Id = m_lGlobalID++;

             // FOGBUGZ 11412 and 11413
             IVar.ArrayDimension = nArrayDim;

             IVar.parentId = lParentID;
             ((ImportDataPPI)IVar).ImportType = ImportTypes.Array;

             AddTreeItem(IVar, inRootItem);

             string szFieldLine;
             uint i = 0;
             for (i = 0; i < nArrayDim; i++)
             {
                 var f = importDataModel.addImportData();
                 szFieldLine = string.Format("{0}[{1}]", szVarComplName, i);

                 if (nDBNumber > 0)
                     ((ImportDataPPI)f).PreName = string.Format("DB{0}", nDBNumber);//https://support/Products/default.asp?7797
                 f.Name = szFieldLine;
                 ((ImportDataPPI)f).Size = nVarSize;
                 f.szType = szVarType;
                 ((ImportDataPPI)f).Type = ((ImportDataPPI)IVar).ElemType;
                 f.Address = GetAddress(nVarType, nDBNumber, ref nCurrentAddress, ref nBitNumber,
                     ref bIncrementAddressIfNotBit, nStringLen, ref szVarType);

                 f.Description = szDescription;
                 f.Id = m_lGlobalID++;

                 // FOGBUGZ 11412 and 11413
                 f.ArrayDimension = 0;

                 f.parentId = IVar.Id;
                 ((ImportDataPPI)f).ImportType = ImportTypes.Standard;

                 AddTreeItem(f, IVar);

             }
             //Last iteration? --> Set to 0 the bit number and
             // increment the byte address
             if (nVarType == Opc.Ua.BuiltInType.Boolean && nBitNumber != 0)
             {
                 nBitNumber = 0;
                 nCurrentAddress++;
                 bIncrementAddressIfNotBit = false;
             }
             if ((nCurrentAddress % 2) > 0)
                 nCurrentAddress++;

         }


         private void DisplayImportFileSdf(string file)
         {
             try
             {
                 System.IO.StreamReader readFile = new System.IO.StreamReader(file);

                 string line;

                 while ((line = readFile.ReadLine()) != null)
                 {
                     var IVar = importDataModel.addImportData();
                     int nStart = line.IndexOf('"', 0);
                     if (nStart == -1)
                         return;

                     int nEnd = line.IndexOf('"', nStart + 1);
                     if (nEnd == -1)
                         return;

                     string strSymb = line.Substring(nStart + 1, nEnd - nStart - 1);
                     RemoveBlanks(ref strSymb);


                     nStart = line.IndexOf('"', nEnd + 1);
                     if (nStart == -1)
                         return;

                     nEnd = line.IndexOf('"', nStart + 1);
                     if (nEnd == -1)
                         return;

                     string strAddr = line.Substring(nStart + 1, nEnd - nStart - 1);
                     if (strAddr.Length == 0)
                         continue;

                     //if address is invalid or unsupported, we skip the line
                     RemoveBlanks(ref strAddr);


                     Step7Area area = Step7Area.aInvalid;
                     int dbnumber = -1;
                     Step7Format format = Step7Format.frmInvalid;
                     Step7WordTrans trans = Step7WordTrans.wtC;
                     int offset = 0;
                     int bit = 0;
                     int length = 0;
                     if (!PPIDynTagSettings.StaticParseAddress(strAddr, ref area, ref dbnumber,
                         ref format, ref trans, ref offset, ref bit, ref length))
                         continue;

                     //
                     // Get the variable type.
                     //

                     string strType = string.Empty;
                     nStart = line.IndexOf('"', nEnd + 1);
                     if (nStart != -1)
                     {
                         nEnd = line.IndexOf('"', nStart + 1);
                         if (nEnd != -1)
                         {
                             strType = line.Substring(nStart + 1, nEnd - nStart - 1);
                             RemoveBlanks(ref strType);
                         }
                     }
                     //Added in version 10.0.0.8
                     string strDesc = string.Empty;
                     nStart = line.IndexOf('"', nEnd + 1);
                     if (nStart != -1)
                     {
                         nEnd = line.IndexOf('"', nStart + 1);
                         if (nEnd != -1)
                         {
                             strDesc = line.Substring(nStart + 1, nEnd - nStart - 1).Trim();

                         }
                     }

                     if (strSymb.Length == 0)
                     {
                         strSymb = strAddr;
                         strSymb.Replace(':', '_');
                         strSymb.Replace('.', '_');
                     }

                     string strDlg = strSymb + " - ";
                     strDlg += strAddr + " - ";
                     strDlg += strType;
                     //Added in version 10.0.0.9
                     if (strDesc.Length != 0)
                     {
                         strDlg += " - " + strDesc;
                     }

                     DataType nType = DataType.Boolean;
                     int nConversion = 0;
                     uint nVarSize = uint.MaxValue;
                     if (!GetMoviconTypeId(strType, ref nConversion, ref nVarSize, ref nType))
                         continue;

                     if (dbnumber != -1)
                         ((ImportDataPPI)IVar).PreName = string.Format("DB{0}", dbnumber);//https://support/Products/default.asp?7797
                     IVar.Name = strSymb;
                     ((ImportDataPPI)IVar).Type = nType;
                     ((ImportDataPPI)IVar).Size = nVarSize;
                     IVar.szType = strType;
                     ((ImportDataPPI)IVar).ElemType = DataType.Boolean;
                     IVar.Address = strAddr;
                     IVar.Description = strDesc;
                     IVar.Id = m_lGlobalID++;

                     // FOGBUGZ 11412 and 11413
                     IVar.ArrayDimension = 0;
                     ((ImportDataPPI)IVar).ImportType = ImportTypes.Standard;

                     AddTreeItem(IVar);
                 }
                 readFile.Close();
             }
             catch (Exception e)
             {
             }
         }

         public bool GetMoviconTypeId(string szType, ref int nConversion, ref uint nVarSize, ref DataType nType)
         {
             bool exit = false;
             szType = szType.Trim();
             szType = szType.ToUpper();
             nConversion = 0;
             nVarSize = 0;
             nType = DataType.Boolean;
             if (szType.Length == 0)
             {
                 return (exit);
             }
             if (szType == "BOOL")
             {
                 nType = DataType.Boolean;
                 nVarSize = 1;
                 exit = true;
             }
             else if (szType == "BYTE")
             {
                 nType = DataType.Byte;
                 nVarSize = 1;
                 exit = true;
             }
             else if (szType == "WORD"
                 || szType == "COUNTER"
                 || szType == "TIMER")
             {
                 nType = DataType.UInt16;
                 nVarSize = 2;
                 exit = true;
             }
             else if (szType == "S5TIME")
             {
                 nType = DataType.UInt32;
                 nConversion = 1;
                 nVarSize = 4;
                 exit = true;
             }
             else if (szType == "DWORD")
             {
                 nType = DataType.UInt32;
                 nVarSize = 4;
                 exit = true;
             }
             else if (szType == "INT")
             {
                 nType = DataType.Int16;
                 nVarSize = 2;
                 exit = true;
             }
             else if (szType == "DINT")
             {
                 nType = DataType.Int32;
                 nVarSize = 4;
                 exit = true;
             }
             else if (szType == "REAL")
             {
                 nType = DataType.Float;
                 nVarSize = 4;
                 exit = true;
             }
             else if (szType == "CHAR")
             {
                 nType = DataType.SByte;
                 nVarSize = 1;
                 exit = true;
             }
             else if (szType == "STRING")
             {
                 nType = DataType.String;
                 exit = true;
             }

             return (exit);
         }
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

                    baseImportTree.Dispose();
                    baseImportTree = null;
                }
            }
         }
         #endregion
    }

    public class ImportDataPPI : ImportData, IDisposable
    {
        public ImportDataPPI(ImportDataModel inDataModel)
            : base(inDataModel)
        {
            dataModelPPI = inDataModel as ImportDataModelPPI;
        }
        private ImportDataModelPPI dataModelPPI { get; set; }
       
        private string _PreName;
        public string PreName
        {
            get { return _PreName; }
            set { _PreName = value; }
        }
       
        private DataType _ElemType;
        public DataType ElemType
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
        private ImportTypes _ImportType;
        public ImportTypes ImportType
        {
            get { return _ImportType; }
            set { _ImportType = value; }
        }
        
        private uint _Size;
        public uint Size
        {
            get { return _Size; }
            set { _Size = value; }
        }
        private string _Name;
        public override string Name
        {
            get 
            {
                string outName = _Name;
                if (Parent == null)
                {
                    if (!string.IsNullOrEmpty(PreName) && PreName != null)
                        outName = PreName + "_" + outName;
                    string StationName = dataModelPPI.getStationName();
                    if (StationName != "_")
                        outName = StationName + outName;
                }

                return outName; 
            }
            set { _Name = value; }
        }
                
        //public string TreeName
        //{
        //    get { return getTreeName(); }
        //}

        //private string getTreeName()
        //{
        //    string treeName = Name;
        //    if(Parent != null)
        //    {
        //        treeName = Parent.getTreeName() + "." + treeName;
        //    }
        //    return treeName;
        //}

        public override DataType IconTagType
        {
            get { return _Type; }
        }

        #region IDisposable Members
        public void Dispose()
        {
            if (Children == null)
            {
                return;
            }

            if (Children.Count() > 0)
            {
                foreach (ImportData el in Children)
                {
                    var sls7 = el as ImportDataPPI;
                    if (sls7 != null)
                    {
                        sls7.Dispose();
                    }
                }
                Children.Clear();
            }
        }
        #endregion
    }

    public class ImportDataModelPPI : ImportDataModel, IDisposable
    {
        public ImportDataModelPPI(GetStationName inGetStationName) :
            base(inGetStationName)
        {
        }

        public override ImportData addImportData()
        {
            ImportDataPPI importData = new ImportDataPPI(this);
            return importData;
        }
        #region IDisposable Members

        public void Dispose()
        {
            foreach (ImportData el in Children)
            {
                ImportDataPPI els7 = el as ImportDataPPI;
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
