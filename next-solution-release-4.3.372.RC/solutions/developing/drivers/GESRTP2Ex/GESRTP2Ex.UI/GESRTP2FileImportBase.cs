using System;
using System.Collections.Generic;
using System.Linq;
using UFUAModel;
using DriverCodeBaseEx.UI;
using System.Text.RegularExpressions;

namespace GESRTP2.UI
{
    public class GESRTP2FileImportBase
    {
        protected class ImportedVariable : ICloneable
        {
            private Dictionary<string, Dictionary<string, FileVariable.DataAreaAddress>> dataAreaArrayElementsAddress;

            public string VariableName { get; set; }
            public string VarType { get; set; }
            public ImportTypes Category { get; set; }
            public string VarTypeOfStruct { get; set; }
            public uint VarSize { get; set; }
            public uint ElementsNumber { get; set; }
            public uint ArrayDimCount { get; set; }
            public uint[] ArrayStartIndexes { get; set; }
            public uint[] ArrayIndexLimits { get; set; }
            public string Publish { get; set; }
            public AreaTypes AreaType { set; get; }
            public string Address { set; get; }
            public ushort AddressDataArea { set; get; }
            public string Descrition { set; get; }

            public ImportedVariable()
            {
                dataAreaArrayElementsAddress = new Dictionary<string, Dictionary<string, FileVariable.DataAreaAddress>>();

                VariableName = string.Empty;
                VarType = string.Empty;
                Category = ImportTypes.Unknown;
                VarTypeOfStruct = string.Empty;
                VarSize = 0;
                ElementsNumber = 0;
                ArrayDimCount = 0;
                ArrayStartIndexes = null;
                ArrayIndexLimits = null;                
                Publish = string.Empty;                
                AreaType = AreaTypes.Symbolic;
                Address = string.Empty;
                AddressDataArea = 0;
                Descrition = string.Empty;
            }

            public void AddDataAreaAllArrayElementsAddress(Dictionary<String, Dictionary<string, FileVariable.DataAreaAddress>> arr)
            {
                dataAreaArrayElementsAddress = new Dictionary<string, Dictionary<string, FileVariable.DataAreaAddress>>(arr);
            }

            public FileVariable.DataAreaAddress GetDataAreaAddressArrayElement(string varArrayName, string elementName)
            {
                FileVariable.DataAreaAddress address = null;
                if (dataAreaArrayElementsAddress.ContainsKey(varArrayName))
                {
                    if (dataAreaArrayElementsAddress[varArrayName].ContainsKey(elementName))
                        address = dataAreaArrayElementsAddress[varArrayName][elementName];
                }

                return address;
            }

            #region ICloneable Members
            public object Clone()
            {
                return MemberwiseClone();
            }
            #endregion
        }

        public class FileVariable
        {
            public class DataAreaAddress
            {
                public string Address { get; set; }
                public ushort AddressDataArea { get; set; }

                private int _CurrentArrayIndex;

                public DataAreaAddress(string address, ushort addressDataArea)
                {
                    Address = address;
                    AddressDataArea = addressDataArea;
                    _CurrentArrayIndex = 1;
                }

                public int GetNextArrayElementIndex()
                {                    
                    return ++_CurrentArrayIndex;
                }

                public int GetCurrentArrayElementIndex()
                {
                    return _CurrentArrayIndex;
                }
            }

            Dictionary<string, Dictionary<string, DataAreaAddress>> dataAreaArrayElementsAddress = new Dictionary<string, Dictionary<string, DataAreaAddress>>();

            public string RawData { set; get; }
            public string Name { set; get; }
            public string sType { set; get; }            
            public string Descr { set; get; }
            public string DataTypeID { set; get; }
            public string Retentive { set; get; }
            public string Force2 { set; get; }
            public string DisplayFormat { set; get; }
            private uint _ArrayDimension1;
            public uint ArrayDimension1 { set { _ArrayDimension1 = value; } get { return _ArrayDimension1; } }
            private uint _ArrayDimension2;
            public uint ArrayDimension2 { set { _ArrayDimension2 = value; } get { return _ArrayDimension2; } }
            public string SHOW_EXT { set; get; }
            public string Publish { get { return SHOW_EXT; } }
            public string MarkAsUsed { set; get; }
            private uint _MaxLength;
            public uint MaxLength { set { _MaxLength = value; } get { return _MaxLength; } }
            public string STORED_VAL { set; get; }
            public string DataSource { set; get; }
            public string DataSourceClsid { set; get; }
            public string ADDR { set; get; }
            public string ADDR_OFFSET { set; get; }
            public string IOAddressAlias { set; get; }
            public string Input_VTL { set; get; }
            public string Output_VTL { set; get; }
            public string extra_properties { set; get; }
            public AreaTypes AreaType { set; get; }

            private UInt16 _ADDDataArea;
            public UInt16 ADDDataArea { set { _ADDDataArea = value; } get { return _ADDDataArea; } }

            private bool Parse(string line)
            {
                RawData = line;

                // parsing cvs file with quotes (that contain ,)
                Regex CSVParser = new Regex(",(?=(?:[^\"]*\"[^\"]*\")*(?![^\"]*\"))");
                List<String> fields = CSVParser.Split(line).ToList();
                if (fields.Count < 21)
                    return false;

                //PT_ID,PT_TYPE,DESC,DataTypeID,RETENTIVE,Force2,DisplayFormat,LENGTH,ArrayDimension2,SHOW_EXT,MarkAsUsed,MaxLength,STORED_VAL,DataSource,DataSourceClsid,ADDR,ADDR_OFFSET,IOAddressAlias,Input_VTL,Output_VTL,extra_properties                
                Name = fields[0].Trim('"');   // PT_ID 
                sType = fields[1];
                // filter variable declaration array index variable[0] etc
                if (sType.IndexOf("]")>0)
                {
                    Name += string.Format(",{0}", fields[1]);
                    fields.RemoveAt(1);

                    sType = fields[1];
                }

                // discard null name/data type
                if (string.IsNullOrWhiteSpace(Name) || string.IsNullOrWhiteSpace(sType))
                    return false;

                Descr = fields[2].Trim();
                DataTypeID = fields[3].Trim();
                Retentive = fields[4].Trim();
                Force2 = fields[5].Trim();
                DisplayFormat = fields[6].Trim();
                if (!UInt32.TryParse(fields[7].Trim(), out _ArrayDimension1))
                    _ArrayDimension1 = 0;
                if (!UInt32.TryParse(fields[8].Trim(), out _ArrayDimension2))
                    _ArrayDimension2 = 0;
                SHOW_EXT = fields[9].Trim();
                MarkAsUsed = fields[10].Trim();
                if (!UInt32.TryParse(fields[11].Trim(), out _MaxLength))
                    _MaxLength = 32;
                STORED_VAL = fields[12].Trim('"');
                DataSource = fields[13].Trim();
                DataSourceClsid = fields[14].Trim();
                ADDR = fields[15].Trim();
                ADDR_OFFSET = fields[16].Trim();
                IOAddressAlias = fields[17].Trim();
                Input_VTL = fields[18].Trim();
                Output_VTL = fields[19].Trim();
                extra_properties = fields[20].Trim();

                return true;
            }

            //public bool ParseSNF(string line)
            //{                
            //    if (!Parse(line))
            //        return false;

            //    if (IsSymbolic())
            //    {
            //        AreaType = AreaTypes.Symbolic;

            //        // enum data type not supported
            //        if (DisplayFormat.ToLower() == "enumtext")
            //            return false;
            //    }
            //    else
            //    {
            //        if (!ParseDataAreaFields())
            //            return false;
            //    }

            //    return true;
            //}

            private bool ParseDataAreaFields()
            {                                       
                // not a valid data area address
                if (!(ADDR.Length == 7 || ADDR.Length == 8))
                    return false;

                // valid address example %R00001
                if (ADDR.Substring(0, 1) != "%")
                    return false;

                string address = ADDR.TrimStart('%'); 

                String sAreaType = String.Empty;
                foreach (char c in address)
                {
                    if (Char.IsLetter(c))
                        sAreaType += c;
                }

                int type = GetMoviconType(sType);
                if (type == -1)
                    return false;

                AreaType = GetAreaType(sAreaType, (DataType)type);
                if (AreaType == unchecked((AreaTypes)(-1)))
                    return false;

                address = address.TrimStart(sAreaType.ToCharArray());
                if (!UInt16.TryParse(address, out _ADDDataArea))
                    return false;

                return true;
            }

            public bool ParseFile(string line)
            {
                if (!Parse(line))
                    return false;

                if (IsSymbolic())
                {
                    AreaType = AreaTypes.Symbolic;

                    // enum data type not supported
                    if (DisplayFormat.ToLower() == "enumtext")
                        return false;
                }
                else
                {
                    if (!ParseDataAreaFields())
                        return false;
                }

                return true;
            }

            //public bool ParseCSV(string line)
            //{                
            //    if (!Parse(line))
            //        return false;

            //    return ParseDataAreaFields();
            //}

            public bool IsSymbolic()
            {
                return (ADDR.ToLower() == "<Symbolic>".ToLower());
            }

            public bool IsDataArea()
            {
                return !IsSymbolic();
            }

            public bool IsArray()
            {
                return (ArrayDimension1 != 0 || ArrayDimension2 != 0);
            }

            public bool IsPublished()
            {
                // publish property of PLC variable
                return Publish.ToLower().Contains("external");
            }

            public void AddDataAreaArrayElementAddress(string varArrayName, string elemetName ,string address, ushort addressDataArea)
            {
                if (!dataAreaArrayElementsAddress.ContainsKey(varArrayName))
                    dataAreaArrayElementsAddress[varArrayName] = new Dictionary<string, DataAreaAddress>();
                    
                dataAreaArrayElementsAddress[varArrayName][elemetName] = new DataAreaAddress(address, addressDataArea);
            }

            public Dictionary<String, Dictionary<string, DataAreaAddress>> GetDataAreaAllArrayElementsAddress()
            {
                return dataAreaArrayElementsAddress;
            }

            #region ICloneable Members
            public object Clone()
            {
                return MemberwiseClone();
            }
            #endregion
        }        

        private static AreaTypes GetAreaType(String strAreaType, UFUAModel.DataType MovType)
        {
            switch (strAreaType)
            {
                case "R":
                    return AreaTypes.RegisterWords_R;
                case "AI":
                    return AreaTypes.AnalogInputWords_AI;
                case "AQ":
                    return AreaTypes.AnalogOutputWords_AQ;
                case "I":
                    if (MovType == UFUAModel.DataType.Boolean)
                        return AreaTypes.DiscreteInputBits_I;
                    else
                        return AreaTypes.DiscreteInputBytes_I;
                case "Q":
                    if (MovType == UFUAModel.DataType.Boolean)
                        return AreaTypes.DiscreteOutputBits_Q;
                    else
                        return AreaTypes.DiscreteOutputBytes_Q;
                case "T":
                    if (MovType == UFUAModel.DataType.Boolean)
                        return AreaTypes.DiscreteTemporaryBits_T;
                    else
                        return AreaTypes.DiscreteTemporaryBytes_T;
                case "SA":
                    if (MovType == UFUAModel.DataType.Boolean)
                        return AreaTypes.DiscreteBits_SA;
                    else
                        return AreaTypes.DiscreteBytes_SA;
                case "SB":
                    if (MovType == UFUAModel.DataType.Boolean)
                        return AreaTypes.DiscreteBits_SB;
                    else
                        return AreaTypes.DiscreteBytes_SB;
                case "SC":
                    if (MovType == UFUAModel.DataType.Boolean)
                        return AreaTypes.DiscreteBits_SC;
                    else
                        return AreaTypes.DiscreteBytes_SC;
                case "S":
                    if (MovType == UFUAModel.DataType.Boolean)
                        return AreaTypes.DiscreteBits_S_readOnly;
                    else
                        return AreaTypes.DiscreteBytes_S_readOnly;
                case "G":
                    if (MovType == UFUAModel.DataType.Boolean)
                        return AreaTypes.GeniusGlobalDataBits_G;
                    else
                        return AreaTypes.GeniusGlobalDataBytes_G;
                case "M":
                    if (MovType == UFUAModel.DataType.Boolean)
                        return AreaTypes.DiscreteInternalBits_M;
                    else
                        return AreaTypes.DiscreteInternalBytes_M;
                case "W":
                    return AreaTypes.WordMemory_W;
                default:
                    return unchecked((AreaTypes)(-1));
            }
        }

        public enum VarType : uint
        {
            VAR_TYPE_BOOL = 0,
            VAR_TYPE_BYTE,
            VAR_TYPE_WORD,
            VAR_TYPE_DWORD,
            VAR_TYPE_INT,
            VAR_TYPE_LREAL,
            VAR_TYPE_REAL,
            VAR_TYPE_SINT,
            VAR_TYPE_UDINT,
            VAR_TYPE_UINT,
            VAR_TYPE_USINT,            
            VAR_TYPE_DINT,
            VAR_TYPE_STRING,
            VAR_TYPE_ARRAY,
            VAR_TYPE_STRUCT,
            VAR_TYPE_E_UNKNOWN
        }

        private static VarType GetVarType(string stringType)
        {
            VarType ResulType = VarType.VAR_TYPE_E_UNKNOWN;

            switch (stringType.ToUpper().Trim())
            {
                case "BOOL":
                    ResulType = VarType.VAR_TYPE_BOOL;
                    break;
                case "BYTE":
                    ResulType = VarType.VAR_TYPE_BYTE;
                    break;
                case "DINT":
                    ResulType = VarType.VAR_TYPE_DINT;
                    break;
                case "DWORD":
                    ResulType = VarType.VAR_TYPE_DWORD;
                    break;
                case "INT":
                    ResulType = VarType.VAR_TYPE_INT;
                    break;
                case "LREAL":
                    ResulType = VarType.VAR_TYPE_LREAL;
                    break;
                case "REAL":
                    ResulType = VarType.VAR_TYPE_REAL;
                    break;
                case "SINT":
                    ResulType = VarType.VAR_TYPE_SINT;
                    break;
                case "STRING":
                    ResulType = VarType.VAR_TYPE_STRING;
                    break;
                case "UDINT":
                    ResulType = VarType.VAR_TYPE_UDINT;
                    break;
                case "UINT":
                    ResulType = VarType.VAR_TYPE_UINT;
                    break;
                case "USINT":
                    ResulType = VarType.VAR_TYPE_USINT;
                    break;
                case "WORD":
                    ResulType = VarType.VAR_TYPE_WORD;
                    break;                                                
                default:
                    ResulType = VarType.VAR_TYPE_STRUCT;
                    break;
            }

            return ResulType;
        }

        protected static bool IsStandardDataType(string dataType)
        {
            return (GetVarType(dataType) != VarType.VAR_TYPE_E_UNKNOWN && GetVarType(dataType) != VarType.VAR_TYPE_STRUCT);
        }

        protected static bool IsPlcStructureType(string varType)
        {
            return (GetVarType(varType) == VarType.VAR_TYPE_E_UNKNOWN || GetVarType(varType) == VarType.VAR_TYPE_STRUCT);
        }

        protected static int GetMoviconType(string varType)
        {
            VarType V = GetVarType(varType);
            if (V == VarType.VAR_TYPE_E_UNKNOWN)
                return -1;
            else
                return (int)GetDataType(V);
        }

        protected static UFUAModel.DataType GetDataType(VarType dataFormat)
        {
            switch (dataFormat)
            {
                case VarType.VAR_TYPE_BOOL:
                    return UFUAModel.DataType.Boolean;
                case VarType.VAR_TYPE_BYTE:
                    return UFUAModel.DataType.Byte;
                case VarType.VAR_TYPE_DINT:
                    return UFUAModel.DataType.Int32;
                case VarType.VAR_TYPE_DWORD:
                    return UFUAModel.DataType.UInt32;
                case VarType.VAR_TYPE_INT:
                    return UFUAModel.DataType.Int16;
                case VarType.VAR_TYPE_LREAL:
                    return UFUAModel.DataType.Double;
                case VarType.VAR_TYPE_REAL:
                    return UFUAModel.DataType.Float;
                case VarType.VAR_TYPE_STRING:
                    return UFUAModel.DataType.String;
                case VarType.VAR_TYPE_UDINT:
                    return UFUAModel.DataType.UInt32;
                case VarType.VAR_TYPE_UINT:
                    return UFUAModel.DataType.UInt16;
                case VarType.VAR_TYPE_USINT:
                    return UFUAModel.DataType.Byte;
                case VarType.VAR_TYPE_WORD:
                    return UFUAModel.DataType.UInt16;
                default:
                    return 0;
            }
        }

        protected static uint GetByteSizeOfVarType(VarType varType, uint defaultSize)
        {
            switch (varType)
            {
                case VarType.VAR_TYPE_BOOL:
                    return 1;
                case VarType.VAR_TYPE_BYTE:
                    return 1;
                case VarType.VAR_TYPE_DINT:
                    return 4;
                case VarType.VAR_TYPE_DWORD:
                    return 4;
                case VarType.VAR_TYPE_INT:
                    return 2;
                case VarType.VAR_TYPE_LREAL:
                    return 8;
                case VarType.VAR_TYPE_REAL:
                    return 4;
                case VarType.VAR_TYPE_STRING:
                    if (defaultSize == 0)
                        return 32;
                    else
                        return defaultSize;
                case VarType.VAR_TYPE_UDINT:
                    return 4;
                case VarType.VAR_TYPE_UINT:
                    return 2;
                case VarType.VAR_TYPE_USINT:
                    return 1;
                case VarType.VAR_TYPE_WORD:
                    return 2;                
            }
            return 0;
        }

        protected static int GetVarTypeAndSize(FileVariable row, out uint varSize)
        {
            VarType v = GetVarType(row.sType);
            if (v == VarType.VAR_TYPE_E_UNKNOWN)
            {
                varSize = 0;
                return -1;
            }
            else
            {
                varSize = GetByteSizeOfVarType(v, row.MaxLength);
                return (int)GetDataType(v);
            }
        }

        public enum ImportTypes : byte
        {
            Unknown,
            Struct, // StructOrEnum
            ArrayOfStruct,
            Standard,
            Alias,
            Array            
        }

        protected ImportDataModelGESRTP2 _ImportDataModel;
        protected int _lGlobalID;
        
        protected Dictionary<string, List<ImportedVariable>> _MapSTRUCT;
        protected Dictionary<string, string> _MapPublish;

        #region Properties
        private string _LastError;
        public string LastError { set { _LastError = value; } get { return _LastError; } }
                
        #endregion

        #region Constructors
        public GESRTP2FileImportBase()
        {
            _ImportDataModel = null;
            _LastError = null;
            _lGlobalID = 0;            
            _MapSTRUCT = new Dictionary<string, List<ImportedVariable>>();
            _MapPublish = new Dictionary<string, string>();
        }
        #endregion

        #region Methods

        public void AddPublishProperty(GESRTP2Protocol.PlcTypes plcType, string memberFullPath, string publish)
        {
            _MapPublish[memberFullPath] = publish;
        }

        /// <summary>
        /// Get the publish/show_text value (retrieve)
        /// </summary>
        /// <param name="memberFullPath"></param>
        /// <returns></returns>
        public string GetPublishProperty(string memberFullPath)
        {
            if (_MapPublish.ContainsKey(memberFullPath))
                return _MapPublish[memberFullPath];
            else
                return string.Empty;
        }        

        protected void RemoveArrayOfStructInSubStruct(List<ImportedVariable> parsedVars)
        {
            foreach (var MembersOfStruct in _MapSTRUCT.Values)
                MembersOfStruct.RemoveAll(a => a.Category == ImportTypes.ArrayOfStruct);

            // try to remove struct with no members
            foreach (var tag in _MapSTRUCT.Where(a => a.Value.Count == 0).ToList().AsParallel())
            {
                _MapSTRUCT.Remove(tag.Key);
                // remove all vars with this data type
                parsedVars.RemoveAll(a => a.Category == ImportTypes.ArrayOfStruct && a.VarTypeOfStruct == tag.Key);
            }
        }        

        protected void AddTreeItem(ImportData tag, ImportData parent = null)
        {
            tag.Parent = parent;
            if (parent == null)
            {
                tag.TreeLevel = 0xFF;
                tag.parentId = -1;
                _ImportDataModel.Children.Add(tag);
            }
            else
            {
                tag.TreeLevel = parent.TreeLevel - 1;
                tag.parentId = parent.Id;
                parent.Children.Add(tag);
            }
        }

        protected void AddArray(string szNamePrefix, string szAddressPrefix, string szArrayName, string descrition, string szElemType, int moviconType, uint nElemSize, DriverCodeBaseEx.Enumerators.LinkType jobType, AreaTypes areaType, uint elementsNumber, uint elementSize, ImportedVariable typeDefinition,int parentId = -1, ImportData inRootItem = null)
        {
            if (typeDefinition.ArrayDimCount == 0)
                return;

            string szAddress = szAddressPrefix + szArrayName;
            // Add the array variable to the tree
            ImportData v = _ImportDataModel.addImportData();
            ((ImportDataGESRTP2)v).Name = szNamePrefix;
            v.Name = szArrayName;
            v.Description = descrition;
            v.szType = "ARRAY ";
            for (int Index = 0; Index < typeDefinition.ArrayStartIndexes.Length; Index++)
                v.szType += string.Format("[{0}..{1}]", typeDefinition.ArrayStartIndexes[Index], typeDefinition.ArrayIndexLimits[Index]-1);
            v.szType += " OF " + szElemType;
            //v.szTypeView = GetMoviconTypeFromPlcType(v.szType);
            ((ImportDataGESRTP2)v).ElemType = szElemType.ToString();
            ((ImportDataGESRTP2)v).TagType = (DataType)moviconType;
            ((ImportDataGESRTP2)v).Size = elementsNumber * nElemSize;
            if (((ImportDataGESRTP2)v).TagType == DataType.String)
            {
                ((ImportDataGESRTP2)v).StringLength = elementSize;
                v.szType = string.Format("{0}({1})", v.szType, ((ImportDataGESRTP2)v).StringLength);
            }
            if (areaType == AreaTypes.Symbolic)
            {
                v.Address = szAddress;
            }
            else
            {
                v.Address = typeDefinition.Address;
                ((ImportDataGESRTP2)v).AddressDataArea = typeDefinition.AddressDataArea;
            }          
            v.parentId = parentId;
            v.Id = _lGlobalID++;
            v.ArrayDimension = elementsNumber;
            ((ImportDataGESRTP2)v).ImportDataType = ImportTypes.Array;
            ((ImportDataGESRTP2)v).JobType = jobType;
            ((ImportDataGESRTP2)v).AreaType = areaType;
            ((ImportDataGESRTP2)v).Publish = GetPublishProperty(v.Address);
            AddTreeItem(v, inRootItem);

            // Add the array elements to the tree
            string varNamePrefix = String.Format("{0}[", szArrayName);
            string addrPrefix = String.Format("{0}[", szAddress);            
            AddArrayElements(szNamePrefix, varNamePrefix, addrPrefix, v.Id, 0, szElemType, moviconType, nElemSize, jobType, areaType, typeDefinition, v);
        }

        protected void AddArrayElements(string szNamePrefix, string varNamePrefix, string addressPrefix, int parentId, uint currentDim, string szElemType, int moviconType, uint nElemSize, DriverCodeBaseEx.Enumerators.LinkType jobType, AreaTypes areaType, ImportedVariable typeDefinition, ImportData inRootItem)
        {            
            for (uint i = typeDefinition.ArrayStartIndexes[currentDim]; i < typeDefinition.ArrayIndexLimits[currentDim]; i++)
            {
                string szAddress = string.Empty;
                UInt16 nAddressDataArea = 0;                
                string varName = String.Format("{0}{1}", varNamePrefix, i);
                szAddress = String.Format("{0}{1}", addressPrefix, i);
                if (currentDim == (typeDefinition.ArrayDimCount - 1))
                {
                    
                    varName += "]";
                    szAddress += "]";

                    FileVariable.DataAreaAddress addr = typeDefinition.GetDataAreaAddressArrayElement(typeDefinition.VariableName, szAddress);
                    if (addr != null)
                    {
                        szAddress = addr.Address;
                        nAddressDataArea = addr.AddressDataArea;
                    }                    

                    AddStandardVar(szNamePrefix, varName, szAddress, nAddressDataArea, String.Empty, szElemType, moviconType, nElemSize, jobType, areaType, parentId, inRootItem);
                }
                else
                {                    
                    varName += ",";
                    szAddress += ",";
                    AddArrayElements(szNamePrefix, varName, szAddress, parentId, currentDim + 1, szElemType, moviconType, nElemSize, jobType, areaType, typeDefinition, inRootItem);
                }
            }
        }

        protected void AddVariable(ImportedVariable var)
        {
            DriverCodeBaseEx.Enumerators.LinkType jobType = DriverCodeBaseEx.Enumerators.LinkType.InputOutput;

            // Structure?
            if (IsPlcStructureType(var.VarType))
            {
                // Add the structure variable and its elements to the tree
                AddPviStructureVariable(string.Empty, var.VariableName, var.Descrition, var.VarTypeOfStruct, var.VarSize, var, jobType, var.AreaType, var.ElementsNumber, -1, null);
            }
            else if (var.ElementsNumber > 1) // Array
            {
                // Add the array variable and its elements to the tree
                AddArrayVariable(string.Empty, String.Empty, var.VariableName, var.Descrition, var.VarType, var, jobType, var.AreaType, var.ElementsNumber, var.VarSize, -1, null);

            }
            else
            {

                // Standard variable
                // Get the corresponding MOVICON type
                int moviconType = GetMoviconType(var.VarType);
                // Invalid Type?
                if (moviconType != -1)
                    AddStandardVar(string.Empty, var.VariableName, var.Address, var.AddressDataArea, var.Descrition, var.VarType, moviconType, var.VarSize, jobType, var.AreaType, -1, null);
            }
        }

        protected void AddPviStructureVariable(string namePrefix, string variableName, string description, string variableType, uint nVarSize, ImportedVariable typeDefinition,  DriverCodeBaseEx.Enumerators.LinkType jobType, AreaTypes areaType, uint elementsNumber, int parentId = -1, ImportData inRootItem = null)
        {
            // If needed, add the task name to the type name
            string completeTypeName = variableType.ToString();

            // Simple structure?
            if ((elementsNumber == 1) && (typeDefinition.Category != ImportTypes.ArrayOfStruct))
            {
                List<ImportedVariable> structureElements = GetStructElements(typeDefinition.VarTypeOfStruct.ToString());
                if (structureElements == null || structureElements.Count == 0)
                    return;

                // Build the structure variable to be added to the tree
                ImportData v = _ImportDataModel.addImportData();
                ((ImportDataGESRTP2)v).PreName = namePrefix;
                v.Name = variableName;
                v.Description = description;
                ((ImportDataGESRTP2)v).Size = nVarSize;
                v.szType = completeTypeName;
                ((ImportDataGESRTP2)v).ElemType = completeTypeName;
                v.Address = variableName;                
                v.parentId = parentId;
                v.Id = _lGlobalID++;
                ((ImportDataGESRTP2)v).ImportDataType = ImportTypes.Struct;
                ((ImportDataGESRTP2)v).JobType = jobType;
                ((ImportDataGESRTP2)v).AreaType = areaType;

                // Add the structure elements to the tree                
                AddStructureElements(namePrefix, variableName, structureElements, jobType,areaType, v.Id, inRootItem, v);
            }
            // Array of Structures
            else if (elementsNumber >= 1)
            {
                // Add the array elements to the tree
                AddStructureArray(namePrefix,variableName, completeTypeName,new List<ImportedVariable>() { typeDefinition }, jobType, areaType, elementsNumber, parentId, inRootItem);
            }
        }

        protected void AddStructureElements(string namePrefix, string addressPrefix, List<ImportedVariable> structureElements, DriverCodeBaseEx.Enumerators.LinkType jobType, AreaTypes areaType, int parentId, ImportData inParentRootItem, ImportData inRootItem)
        {
            bool parentStructHasBeenAddedToTheTree = false;

            for (int i = 0; i < structureElements.Count(); i++)
            {
                ImportedVariable structElement = structureElements[i];

                string auxString = String.Format(".{0}", structElement.VariableName);
                string elementName = structElement.VariableName;
                string elementAddress = addressPrefix + auxString;
                string varPreName = namePrefix + addressPrefix + ".";
                string varType = structElement.VarType;
                uint variableLength = structElement.VarSize;
                uint elementsNumber = structElement.ElementsNumber;

                // Structure?
                if (IsPlcStructureType(varType))
                {
                    varType = structElement.VarTypeOfStruct;

                    string completeTypeName = varType.ToString();                    

                    // Array of structures?
                    if (elementsNumber > 1)
                    {
                        List<ImportedVariable> substructureDefinition = new List<ImportedVariable>();
                        substructureDefinition.Add(structElement);
                        substructureDefinition.AddRange(GetStructElements(structElement.VarTypeOfStruct.ToString()));
                        // no elements
                        if (substructureDefinition.Count == 1)
                            return;
                        AddStructureArray(varPreName, elementName, completeTypeName, substructureDefinition, jobType, areaType, elementsNumber, - 1, null);
                    }
                    else  // singol structure element
                    {

                        List<ImportedVariable> substructureElements = GetStructElements(structElement.VarTypeOfStruct.ToString());
                        if (substructureElements == null || substructureElements.Count == 0)
                            return;

                        // Add the parent structure variable to the tree
                        if ((inRootItem != null) && !parentStructHasBeenAddedToTheTree)
                        {
                            AddTreeItem(inRootItem, inParentRootItem);
                            parentStructHasBeenAddedToTheTree = true;
                        }

                        // Build the substructure variable to be added to the tree
                        ImportData v = _ImportDataModel.addImportData();
                        ((ImportDataGESRTP2)v).PreName = varPreName;
                        v.Name = elementName;
                        v.Description = String.Empty;
                        ((ImportDataGESRTP2)v).Size = variableLength;
                        v.szType = completeTypeName;
                        //v.szTypeView = GetMoviconTypeFromPlcType(v.szType);
                        ((ImportDataGESRTP2)v).ElemType = completeTypeName;
                        v.Address = elementAddress;                        
                        v.parentId = parentId;
                        v.Id = _lGlobalID++;
                        ((ImportDataGESRTP2)v).ImportDataType = ImportTypes.Struct;
                        ((ImportDataGESRTP2)v).JobType = jobType;
                        ((ImportDataGESRTP2)v).AreaType = areaType;
                        
                        // Add the substructure elements to the tree
                        AddStructureElements(namePrefix, elementAddress, substructureElements, jobType, areaType, v.Id, inRootItem, v);
                    }
                }
                else
                {

                    // Get the corresponding MOVICON type
                    int moviconType = GetMoviconType(varType);
                    // Invalid Type?
                    if (moviconType == -1)
                    {
                        i++;
                        continue;
                    }

                    // Add the parent structure variable to the tree
                    if ((inRootItem != null) && !parentStructHasBeenAddedToTheTree)
                    {
                        AddTreeItem(inRootItem, inParentRootItem);
                        parentStructHasBeenAddedToTheTree = true;
                    }

                    // Array of non complex elements ?
                    if (elementsNumber > 1)
                    {
                        string addrPrefix = addressPrefix + ".";
                        // Add the array variable and its elements to the tree
                        AddArrayVariable(varPreName, addrPrefix, elementName, string.Empty, varType, structElement, jobType, areaType, elementsNumber, variableLength, parentId, inRootItem);
                    }
                    else // Standard variable
                    {
                        // Add the standard variable to the tree
                        AddStandardVar(varPreName, elementName, elementAddress, 0, String.Empty, varType, moviconType, variableLength, jobType, areaType, parentId, inRootItem);
                    }
                }
            }
        }


        protected void AddArrayVariable(string namePrefix, string addressPrefix, string variableName, string descrition, string elementType, ImportedVariable typeDefinition, DriverCodeBaseEx.Enumerators.LinkType jobType, AreaTypes areaType, uint elementsNumber, uint elementSize, int parentId = -1, ImportData inRootItem = null)
        {
            // Get the MOVICON type corresponding to the element type
            int moviconType = GetMoviconType(elementType);
            // Invalid Type?
            if (moviconType != -1)
                // Add the variable to the tree
                AddArray(namePrefix, addressPrefix, variableName, descrition, elementType, moviconType, elementSize, jobType, areaType, elementsNumber, elementSize, typeDefinition, parentId, inRootItem);
        }

        protected void AddStructureArray(string namePrefix, string variableName, string completeTypeName, List<ImportedVariable> structureElements, DriverCodeBaseEx.Enumerators.LinkType jobType, AreaTypes areaType, uint elementsNumber, int parentId, ImportData inRootItem)
        {
            string varNamePrefix = String.Format("{0}[", variableName);
            string addrPrefix = String.Format("{0}[", namePrefix + variableName);

            AddStructureArrayElements(namePrefix, varNamePrefix, addrPrefix, completeTypeName, structureElements, jobType, areaType, elementsNumber, structureElements[0].ArrayDimCount, 0, structureElements[0].ArrayStartIndexes, structureElements[0].ArrayIndexLimits, parentId, inRootItem);
        }

        protected void AddStructureArrayElements(string namePrefix, string varNamePrefix, string addrPrefix, string completeTypeName, List<ImportedVariable> structureElements, DriverCodeBaseEx.Enumerators.LinkType jobType, AreaTypes areaType, uint elementsNumber, uint arrayDimCount, uint currentDim, uint[] arrayStartIndexes, uint[] arrayIndexLimits, int parentId, ImportData inRootItem)
        {
            for (uint i = arrayStartIndexes[currentDim]; i < arrayIndexLimits[currentDim]; i++)
            {                
                string szAddress = String.Format("{0}{1}", addrPrefix, i);
                string varName = String.Format("{0}{1}", varNamePrefix, i);
                if (currentDim == (arrayDimCount - 1))
                {
                    varName += "]";
                    szAddress += "]";
                    AddStructureArraySingleElement(namePrefix, varName, szAddress, completeTypeName, structureElements, jobType, areaType, parentId, inRootItem);
                }
                else
                {
                    varName += ",";
                    szAddress += ",";
                    AddStructureArrayElements(namePrefix, varName, szAddress, completeTypeName, structureElements, jobType, areaType, elementsNumber, arrayDimCount, currentDim + 1, arrayStartIndexes, arrayIndexLimits, parentId, inRootItem);
                }
            }
        }

        protected void AddStructureArraySingleElement(string namePrefix, string varName, string szAddress, string completeTypeName, List<ImportedVariable> structureElements, DriverCodeBaseEx.Enumerators.LinkType jobType, AreaTypes areaType, int parentId, ImportData inRootItem)
        {
            uint variableLength = structureElements[0].VarSize;

            // Build the structure variable to be added to the tree
            ImportData v = _ImportDataModel.addImportData();
            ((ImportDataGESRTP2)v).PreName = namePrefix;
            v.Name = varName;
            v.Description = String.Empty;
            ((ImportDataGESRTP2)v).Size = variableLength;
            v.szType = completeTypeName;
            ((ImportDataGESRTP2)v).ElemType = completeTypeName;
            v.Address = szAddress;            
            v.parentId = parentId;
            v.Id = _lGlobalID++;
            ((ImportDataGESRTP2)v).ImportDataType = ImportTypes.Struct;
            ((ImportDataGESRTP2)v).JobType = jobType;
            ((ImportDataGESRTP2)v).AreaType = areaType;

            // Add the structure elements to the tree
            if (structureElements.Count() < 1) // davide 2)
            {
                return;
            }

            List<ImportedVariable> elementDefinitions = GetStructElements(structureElements[0].VarTypeOfStruct.ToString());
            if (elementDefinitions == null || elementDefinitions.Count == 0)
                return;

            string addressPrefix = szAddress;
            AddStructureElements(namePrefix, addressPrefix, elementDefinitions, jobType, areaType, v.Id, inRootItem, v);
        }

        protected void AddStandardVar(string szNamePrefix, string szVarName, string szAddress, ushort nAddressDataArea, string description, string szVarType, int moviconType, uint nVarSize, DriverCodeBaseEx.Enumerators.LinkType jobType, AreaTypes areaType, int parentId = -1, ImportData inRootItem = null)
        {
            ImportData v = _ImportDataModel.addImportData();
            ((ImportDataGESRTP2)v).PreName = szNamePrefix;
            v.Name = szVarName;
            v.Description = description;
            v.szType = szVarType.ToString();
            ((ImportDataGESRTP2)v).ElemType = szVarType.ToString();
            ((ImportDataGESRTP2)v).TagType = (DataType)moviconType;
            ((ImportDataGESRTP2)v).Size = nVarSize;
            if (((ImportDataGESRTP2)v).TagType == DataType.String)
            {
                ((ImportDataGESRTP2)v).StringLength = nVarSize;
                v.szType = string.Format("{0}({1})", v.szType, ((ImportDataGESRTP2)v).StringLength);
            }
            v.Address = szAddress;
            ((ImportDataGESRTP2)v).AddressDataArea = nAddressDataArea;            
            v.parentId = parentId;
            v.Id = _lGlobalID++;
            ((ImportDataGESRTP2)v).ImportDataType = ImportTypes.Standard;
            ((ImportDataGESRTP2)v).JobType = jobType;
            ((ImportDataGESRTP2)v).AreaType = areaType;
            ((ImportDataGESRTP2)v).Publish = GetPublishProperty(v.Address);
            AddTreeItem(v, inRootItem);
        }

        protected List<ImportedVariable> GetStructElements(string structName)
        {
            if (_MapSTRUCT.ContainsKey(structName))
                return _MapSTRUCT[structName];
            else
                return new List<ImportedVariable>();
        }

        protected ImportTypes GetArrayType(FileVariable startElement)
        {
            GetUdtTypeTPY(startElement, out ImportTypes Category, out string VarType, out string VarTypeOfStruct);

            return Category;
        }

        protected bool IsStructOrArrayOfStructType(FileVariable element, out ImportTypes category)
        {                                   
            GetUdtTypeTPY(element, out category, out string VarType, out string VarTypeOfStruct);

            return (category == ImportTypes.Struct || category == ImportTypes.ArrayOfStruct);
        }

        /// <summary>
        /// Replace unvalid prototype char inside string with "_"
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        protected string ParseStructName(string name)
        {
            return UFUAModel.Helpers.NameValidator.EnsureValidName(name);
        }

        protected void GetUdtTypeTPY(FileVariable element, out ImportTypes udtType, out string varType, out string varTypeOfStruct)
        {
            udtType = ImportTypes.Unknown;
            varTypeOfStruct = string.Empty;
            varType = element.sType;
            
            VarType vt = GetVarType(varType);
            if (vt == VarType.VAR_TYPE_STRUCT)
                varTypeOfStruct = varType;                
            
            switch (vt)
            {
                case VarType.VAR_TYPE_STRUCT:
                    if (element.IsArray())
                        udtType = ImportTypes.ArrayOfStruct;
                    else
                        udtType = ImportTypes.Struct;
                    break;
                default:
                    if (IsStandardDataType(varType))
                    {
                        if (element.IsArray())
                            udtType = ImportTypes.Array;
                        else
                            udtType = ImportTypes.Alias;
                    }                    
                    break;
            }
        }

        private void TraverseStructAndGetArrayOfStruct(string sourcePath, string structName, ref List<ImportedVariable> estractedArrayOfStructIsStruct)
        {
            if (!_MapSTRUCT.ContainsKey(structName))
                return;

            // get members of struct
            List<ImportedVariable> MembersOfStruct = _MapSTRUCT[structName];
            foreach(ImportedVariable Member in MembersOfStruct)
            {
                if (Member.Category == ImportTypes.Struct || Member.Category== ImportTypes.ArrayOfStruct)
                {
                    string ChildPath = string.Format("{0}.{1}", sourcePath, Member.VariableName);
                    if (Member.Category == ImportTypes.ArrayOfStruct)
                    {
                        ImportedVariable ArrayOfStruct = (ImportedVariable)Member.Clone();
                        ArrayOfStruct.VariableName = ChildPath;                        
                        estractedArrayOfStructIsStruct.Add(ArrayOfStruct);
                    }
                    TraverseStructAndGetArrayOfStruct(ChildPath, Member.VariableName, ref estractedArrayOfStructIsStruct);
                }
            }
        }


        protected ImportedVariable ParseVariablesDefinition(GESRTP2Protocol.PlcTypes plcType, FileVariable row)
        {
            List<ImportedVariable> NestedArrayOfStruct = null;
            return ParseVariablesDefinition(plcType, row, ref NestedArrayOfStruct);
        }

        protected ImportedVariable ParseVariablesDefinition(GESRTP2Protocol.PlcTypes plcType, FileVariable row, ref List<ImportedVariable> estractedArrayOfStructIsStruct)
        {            
            string VariableName = row.Name;
            string VarDeclaration = row.sType;
            uint VarSize = 0;

            if (row.IsSymbolic() && plcType == GESRTP2Protocol.PlcTypes.Series90_30)
                return null;

            ImportedVariable newElement = new ImportedVariable();
            if (row.IsSymbolic())
                newElement.AreaType = AreaTypes.Symbolic;
            else
                newElement.AreaType = row.AreaType;

            //get data type
            GetUdtTypeTPY(row, out ImportTypes Category, out string VarType, out string VarTypeOfStruct);

            newElement.VariableName = VariableName;
            newElement.Address = VariableName;
            newElement.Descrition = row.Descr;
            newElement.VarType = VarType;
            newElement.Category = Category;

            switch (Category)
            {
                case ImportTypes.Alias:
                    if (GetVarTypeAndSize(row, out VarSize) >= 0)
                    {                        
                        //NewElement.VarDeclaration = VarDeclaration;
                        newElement.VarSize = VarSize;
                        newElement.ElementsNumber = 1;
                        newElement.Publish = row.Publish;
                        if (row.IsDataArea())
                        {
                            newElement.Address = row.ADDR;
                            newElement.AddressDataArea = row.ADDDataArea;
                        }
                    }
                    else
                    {
                        newElement = null;
                    }
                    break;
                
                // Structure or data type not supported (DATE, DATE_AND_TIME --> not supported and so discard it)
                case ImportTypes.Struct:
                    //NewElement.VarDeclaration = VarDeclaration;
                    //NewElement.VarSize = 0; // VarSize;
                    newElement.ElementsNumber = 1;
                    // for area of struct store datatype
                    newElement.VarTypeOfStruct = VarType;

                    //don't add variable without mapped structure data type
                    if (!_MapSTRUCT.ContainsKey(newElement.VarTypeOfStruct))
                        newElement = null;
                    else
                        if (estractedArrayOfStructIsStruct != null)
                        TraverseStructAndGetArrayOfStruct(VariableName, newElement.VarTypeOfStruct, ref estractedArrayOfStructIsStruct);

                    break;

                case ImportTypes.Array:
                    {
                        // Check the variable type and get the variable size
                        GetVarTypeAndSize(row, out VarSize, out uint ArrayDimCount, out uint ElementsNumber, out uint[] ArrayStartIndexes, out uint[] ArrayIndexLimits);

                        //NewElement.VarDeclaration = VarDeclaration;                                    
                        newElement.VarSize = VarSize;
                        newElement.ElementsNumber = ElementsNumber;
                        newElement.ArrayDimCount = ArrayDimCount;
                        newElement.ArrayStartIndexes = ArrayStartIndexes;
                        newElement.ArrayIndexLimits = ArrayIndexLimits;

                        newElement.Publish = row.Publish;

                        if (row.IsDataArea())
                        {
                            newElement.Address = row.ADDR;
                            newElement.AddressDataArea = row.ADDDataArea;
                            newElement.AddDataAreaAllArrayElementsAddress(row.GetDataAreaAllArrayElementsAddress());
                        }
                    }
                    break;

                case ImportTypes.ArrayOfStruct:
                    {                            
                        // Check the variable type and get the variable size
                        GetVarTypeAndSize(row, out VarSize, out uint ArrayDimCount, out uint ElementsNumber, out uint[] ArrayStartIndexes, out uint[] ArrayIndexLimits);

                        newElement.VarSize = VarSize;
                        newElement.ElementsNumber = ElementsNumber;
                        newElement.ArrayDimCount = ArrayDimCount;
                        newElement.ArrayStartIndexes = ArrayStartIndexes;
                        newElement.ArrayIndexLimits = ArrayIndexLimits;
                        newElement.VarTypeOfStruct = VarTypeOfStruct;

                        //don't add variable without mapped structure data type
                        if (!_MapSTRUCT.ContainsKey(newElement.VarTypeOfStruct))
                            newElement = null;
                        else if (estractedArrayOfStructIsStruct != null)
                        {
                            if (ArrayDimCount == 1)
                            {
                                for (uint i = ArrayStartIndexes[0]; i < ArrayIndexLimits[0]; i++)
                                {
                                    // Complete the name with the array index
                                    string varCompleteName = VariableName + String.Format("[{0}]", i);

                                    TraverseStructAndGetArrayOfStruct(varCompleteName, newElement.VarTypeOfStruct, ref estractedArrayOfStructIsStruct);
                                }
                            }
                            else if (ArrayDimCount == 2)
                            {
                                for (uint i = ArrayStartIndexes[0]; i < ArrayIndexLimits[0]; i++)
                                {
                                    for (uint j = ArrayStartIndexes[1]; j < ArrayIndexLimits[1]; j++)
                                    {
                                        // Complete the name with the array index
                                        string varCompleteName = VariableName + String.Format("[{0},{1}]", i, j);

                                        TraverseStructAndGetArrayOfStruct(varCompleteName, newElement.VarTypeOfStruct, ref estractedArrayOfStructIsStruct);
                                    }
                                }
                            }
                        }
                    }
                    break;                    
            }
            

            return newElement;
        }

        private int GetVarTypeAndSize(FileVariable row, out UInt32 standardSize, out uint arrayDimCount, out uint elementsNumber, out UInt32[] arrayStartIndexes, out UInt32[] arrayIndexLimits)
        {
            int returnValue = (int)ImportTypes.Unknown;
            int movType = (int)DataType.Boolean;
            standardSize = 0;
            arrayDimCount = 0;
            elementsNumber = 0;
            arrayStartIndexes = null;
            arrayIndexLimits = null;

            if (row.ArrayDimension1 > 0)
            {
                standardSize = row.ArrayDimension1;
                elementsNumber = row.ArrayDimension1;
                arrayDimCount = 1;
                Array.Resize(ref arrayStartIndexes, 1);
                arrayStartIndexes[0] = 0;
                Array.Resize(ref arrayIndexLimits, 1);
                arrayIndexLimits[0] = row.ArrayDimension1;

                returnValue = (int)ImportTypes.Array;

                if (row.ArrayDimension2 > 0)
                {
                    elementsNumber = row.ArrayDimension1 * row.ArrayDimension2;
                    arrayDimCount = 2;
                    Array.Resize(ref arrayStartIndexes, 2);
                    arrayStartIndexes[0] = 0;
                    arrayStartIndexes[1] = 0;
                    Array.Resize(ref arrayIndexLimits, 2);
                    arrayIndexLimits[1] = row.ArrayDimension2;

                    returnValue = (int)ImportTypes.Array;
                }                
            }

            if (IsStandardDataType(row.sType))
            {
                // Standard type?
                movType = GetVarTypeAndSize(row, out standardSize);
                if (movType >= 0)
                    returnValue = (int)ImportTypes.Standard;
            }
            else
            {                
                if (_MapSTRUCT.ContainsKey(row.sType) == true)
                {
                    if (_MapSTRUCT[row.sType].Count > 0)
                        returnValue = (int)ImportTypes.Struct;                    
                }
            }

            return returnValue;
        }        
        #endregion
    }
}
