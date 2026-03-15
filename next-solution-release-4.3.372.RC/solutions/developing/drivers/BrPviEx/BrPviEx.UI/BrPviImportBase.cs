using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UFUAModel;
using DriverCodeBaseEx.UI;
using DriverCodeBaseEx.UI.Controls;


namespace BrPvi.UI
{
    public class BrPviImportBase
    {
        #region Properties
        public enum ImportUDTType : byte
        {
            Unknown,
            Struct,
            Enum,
            Pointer,
            Alias,
            Array,
            ArrayOfStruct,
            IgnoreVar   // exclude var from import
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

        // Dictionary of variable definitions
        // Key = PVI object (CPU = global variables or task = local variables) complete name, Value = Definitions of variables 
        public Dictionary<string, string[]> mapPviVariables;
        // Dictionary for type definitions
        // Key = PVI variable object complete name, Value = Type Definition
        public Dictionary<string, string> mapPviVarTypes;

        public ImportDataModelBrPVI importDataModel = null;
        public int m_lGlobalID = 0;

        private string _LastError;
        public string LastError { set { _LastError = value; }  get { return _LastError; } }
                
        // string use to parse declaration var in Automation Studio to match data type declaration
        //match data type string with only continuos letters,numeber and <_>
        public const string MATCH_DATATYPE_VAR_DECLARATION = "[a-zA-Z][a-zA-Z0-9_]*";
        #endregion

        #region Constructors
        public BrPviImportBase()
        {
            _LastError = null;
        }
        #endregion

        #region Methods
        private string PlcTypeToMoviconType(string dataType)
        {
            switch (dataType)
            {
                case "i8":
                    return DataType.SByte.ToString();
                case "i16":
                    return DataType.Int16.ToString();
                case "i32":
                    return DataType.Int32.ToString();
                case "i64":
                    return DataType.Int64.ToString();
                case "u8":
                    return DataType.Byte.ToString();
                case "u16":
                    return DataType.UInt16.ToString();
                case "u32":
                    return DataType.UInt32.ToString();
                case "u64":
                    return DataType.UInt64.ToString();
                case "f32":
                    return DataType.Float.ToString();
                case "f64":
                    return DataType.Double.ToString();
                case "boolean":
                    return DataType.Boolean.ToString();
                case "string":
                    return DataType.String.ToString();
                case "wstring":
                    return DataType.String.ToString();
                case "data":
                    return DataType.Byte.ToString();
            }

            return null;
        }

        Dictionary<string, int> SupportedPlcVariableTypes = new Dictionary<string, int>();

        public void FillSupportedPlcTypeDictionary()
        {
            SupportedPlcVariableTypes["i8"] = (int)DataType.SByte;
            SupportedPlcVariableTypes["i16"] = (int)DataType.Int16;
            SupportedPlcVariableTypes["i32"] = (int)DataType.Int32;
            SupportedPlcVariableTypes["i64"] = (int)DataType.Int64;
            SupportedPlcVariableTypes["u8"] = (int)DataType.Byte;
            SupportedPlcVariableTypes["u16"] = (int)DataType.UInt16;
            SupportedPlcVariableTypes["u32"] = (int)DataType.UInt32;
            SupportedPlcVariableTypes["u64"] = (int)DataType.UInt64;
            SupportedPlcVariableTypes["f32"] = (int)DataType.Float;
            SupportedPlcVariableTypes["f64"] = (int)DataType.Double;
            SupportedPlcVariableTypes["boolean"] = (int)DataType.Boolean;
            SupportedPlcVariableTypes["string"] = (int)DataType.String;
            SupportedPlcVariableTypes["wstring"] = (int)DataType.String;
            SupportedPlcVariableTypes["struct"] = -1;
            //SupportedPlcVariableTypes["time"] = (int)DataType.UInt32;
            //SupportedPlcVariableTypes["dt"] = (int)DataType.UInt32;
            //SupportedPlcVariableTypes["date"] = (int)DataType.UInt32;
            //SupportedPlcVariableTypes["tod"] = (int)DataType.UInt32;
            SupportedPlcVariableTypes["data"] = (int)DataType.Byte;
        }

        public virtual ImportDataModel Import(GetStationName readStationName, string station)
        {
            return importDataModel;
        }
        
        public virtual bool ImportFailed()
        {
            return (!string.IsNullOrEmpty(_LastError) || importDataModel==null);
        }

        public string GetMoviconTypeFromPlcType(string address)
        {
            string Result = null;
            var Matches = Regex.Matches(address, MATCH_DATATYPE_VAR_DECLARATION);
            if (Matches.Count > 0)
            {
                Result = PlcTypeToMoviconType(Matches[Matches.Count - 1].ToString());
                if (!string.IsNullOrEmpty(Result))
                {
                    Result = string.Format("{0}{1}", address.Substring(0, Matches[Matches.Count - 1].Index), Result);
                }
            }

            if (string.IsNullOrEmpty(Result))
                Result = address;

            return Result;
        }

        public bool IsSupportedPlcType(string varType)
        {
            // Fill the dictionary of supported types
            if (SupportedPlcVariableTypes.Count == 0)
            {
                FillSupportedPlcTypeDictionary();
            }
            if (SupportedPlcVariableTypes.Keys.Contains(varType))
            {
                return (true);
            }
            else
            {
                return (false);
            }
        }

        public bool IsPlcStructureType(string varType)
        {
            if (String.Compare(varType.ToLower(), "struct") == 0)
            {
                return (true);
            }
            else
            {
                return (false);
            }
        }

        public int GetMoviconType(string varType)
        {
            int moviconType = 0;
            if (!SupportedPlcVariableTypes.TryGetValue(varType, out moviconType))
            {
                return (-1);
            }
            return (moviconType);
        }

        public void AddTreeItem(ImportData tag, ImportData parent = null)
        {
            tag.Parent = parent;
            ((ImportDataBrPvi)tag).strId = tag.Id.ToString("X6");
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
            ((ImportDataBrPvi)tag).strLevel = tag.TreeLevel.ToString("X2");
        }

         public void AddPviArray(string szNamePrefix,
                                 string szAddressPrefix,
                                 string szTaskName,
                                 string szArrayName,
                                 string szElemType,
                                 int moviconType,
                                 uint nElemSize,
                                 DriverCodeBaseEx.Enumerators.LinkType jobType,
                                 uint elementsNumber,
                                 uint nDimCount,
                                 uint[] startIndexes,
                                 uint[] indexLimits,
                                 int parentId = -1,
                                 ImportData inRootItem = null)
        {
            if (nDimCount == 0)
            {
                return;
            }

            string szAddress = szAddressPrefix + szArrayName;
            // Add the array variable to the tree
            ImportData v = importDataModel.addImportData();
            ((ImportDataBrPvi)v).PreName = szNamePrefix;
            v.Name = szArrayName;
            ((ImportDataBrPvi)v).Size = elementsNumber * nElemSize;
            //v.szType = "ARRAY OF " + szElemType;
            v.szType = "ARRAY ";
            for (int Index = 0; Index < startIndexes.Length; Index++)
                v.szType += string.Format("[{0}..{1}]", startIndexes[Index], indexLimits[Index] - 1);
            v.szType += " OF " + szElemType;
            ((ImportDataBrPvi)v).szTypeView = GetMoviconTypeFromPlcType(v.szType);
            ((ImportDataBrPvi)v).Task = szTaskName;
            ((ImportDataBrPvi)v).ElemType = szElemType;
            ((ImportDataBrPvi)v).Type = (DataType)moviconType;
            v.Address = szAddress;
            v.Description = String.Empty;
            v.parentId = parentId;
            v.Id = m_lGlobalID++;
            v.ArrayDimension = elementsNumber;
            ((ImportDataBrPvi)v).ImportDataType = ImportTypes.Array;
            ((ImportDataBrPvi)v).JobType = jobType;
            AddTreeItem(v, inRootItem);

            // Add the array elements to the tree
            string varNamePrefix = String.Format("{0}[", szArrayName);
            string addrPrefix = String.Format("{0}[", szAddress);
            AddArrayElements(szNamePrefix,
                             szTaskName,
                             varNamePrefix,
                             addrPrefix,
                             v.Id,
                             nDimCount,
                             0,
                             szElemType,
                             moviconType,
                             nElemSize,
                             jobType,
                             startIndexes,
                             indexLimits,
                             v);
        }

        void AddArrayElements(string szNamePrefix,
                              string szTaskName,
                              string varNamePrefix,
                              string addressPrefix,
                              int parentId,
                              uint nDimCount,
                              uint currentDim,
                              string szElemType,
                              int moviconType,
                              uint nElemSize,
                              DriverCodeBaseEx.Enumerators.LinkType jobType,
                              uint[] startIndexes,
                              uint[] indexLimits,
                              ImportData inRootItem)
        {
            for (uint i = startIndexes[currentDim]; i < indexLimits[currentDim]; i++)
            {
                uint RealArrayIndex = i - startIndexes[currentDim];
                //in multidimension array, from 2nd dimension, use start index (standard array, always start from 0)
                if (currentDim > 0)
                    RealArrayIndex = i;
                string szAddress = String.Format("{0}{1}", addressPrefix, RealArrayIndex);
                string varName = String.Format("{0}{1}", varNamePrefix, RealArrayIndex);
                if (currentDim == (nDimCount - 1))
                {
                    varName += "]";
                    szAddress += "]";
                    AddPviStandardVar(szNamePrefix,
                                      varName,
                                      szAddress,
                                      szTaskName,
                                      String.Empty,
                                      szElemType,
                                      moviconType,
                                      nElemSize,
                                      jobType,
                                      parentId,
                                      inRootItem);
                }
                else
                {
                    varName += ",";
                    szAddress += ",";
                    AddArrayElements(szNamePrefix,
                                     szTaskName,
                                     varName,
                                     szAddress,
                                     parentId,
                                     nDimCount,
                                     currentDim + 1,
                                     szElemType,
                                     moviconType,
                                     nElemSize,
                                     jobType,
                                     startIndexes,
                                     indexLimits,
                                     inRootItem);
                }
            }
        }

        public void AddPviStandardVar(string szNamePrefix,
                                       string szVarName,
                                       string szAddress,
                                       string taskName,
                                       string description,
                                       string szVarType,
                                       int moviconType,
                                       uint nVarSize,
                                       DriverCodeBaseEx.Enumerators.LinkType jobType,
                                       int parentId = -1,
                                       ImportData inRootItem = null)
        {
            ImportData v = importDataModel.addImportData();
            ((ImportDataBrPvi)v).PreName = szNamePrefix;
            v.Name = szVarName;
            ((ImportDataBrPvi)v).Size = nVarSize;
            v.szType = szVarType;
            ((ImportDataBrPvi)v).szTypeView = GetMoviconTypeFromPlcType(v.szType);
            ((ImportDataBrPvi)v).Task = taskName;
            ((ImportDataBrPvi)v).ElemType = szVarType;
            ((ImportDataBrPvi)v).Type = (DataType)moviconType;
            v.Address = szAddress;
            v.Description = description;
            v.parentId = parentId;
            v.Id = m_lGlobalID++;
            v.ArrayDimension = 0;
            ((ImportDataBrPvi)v).ImportDataType = ImportTypes.Standard;
            ((ImportDataBrPvi)v).JobType = jobType;
            AddTreeItem(v, inRootItem);
        }
    }
    #endregion
}
