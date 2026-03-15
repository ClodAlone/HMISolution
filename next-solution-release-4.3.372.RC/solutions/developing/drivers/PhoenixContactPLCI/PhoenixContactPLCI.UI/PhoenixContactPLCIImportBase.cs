using System;
using System.Collections.Generic;
using System.Linq;
using UFUAModel;
using System.IO;
using PhoenixContact.PlciDotNet;
using PhoenixContact.PlciDotNet.Internal;
using DriverCodeBase.UI;

namespace PhoenixContactPLCI.UI
{
    public class PhoenixContactPLCIImportBase : IDisposable
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

        protected class ParamRif
        {
            public int OffsetOfList { get; set; }
            public int NumberOfElement { get; set; }
            public string structType { get; set; }

        }

        protected class ImportedVariable : ICloneable, IComparable<ImportedVariable>
        {
            public string VariableName { get; set; }
            public string TaskName { get; set; }
            public string VarType { get; set; }
            public ImportUDTType Category { get; set; }
            public string VarTypeOfStruct { get; set; }
            public uint VarSize { get; set; }
            public uint ElementsNumber { get; set; }
            public uint ArrayDimCount { get; set; }
            public uint[] ArrayStartIndexes { get; set; }
            public uint[] ArrayIndexLimits { get; set; }
            public string Description { get; set; }

            public ImportedVariable()
            {
                VariableName = string.Empty;
                TaskName = string.Empty;
                VarType = string.Empty;
                Category = ImportUDTType.Unknown;
                VarTypeOfStruct = string.Empty;
                VarSize = 0;
                ElementsNumber = 0;
                ArrayDimCount = 0;
                ArrayStartIndexes = null;
                ArrayIndexLimits = null;
                Description = string.Empty;
            }

            #region ICloneable Members
            public object Clone()
            {
                return MemberwiseClone();
            }

            public ImportedVariable CastedClone()
            {
                return (ImportedVariable)MemberwiseClone();
            }
            #endregion

            #region IComparable
            public int CompareTo(ImportedVariable other)
            {
                return String.Compare(this.VariableName , other.VariableName);
            }
            #endregion
        }

        protected ImportDataModelPhoenixContactPlci _ImportDataModel;
        protected int _lGlobalID;

        protected List<ImportedVariable> ParsedVars;
        protected Dictionary<string, List<ImportedVariable>> MapSTRUCT;
        protected Dictionary<string, string[]> MapUDT;
        protected Dictionary<string, List<ImportedVariable>> MapUDTI;
        protected Dictionary<string, ParamRif> MapStructWitRifLisVar;

        protected MetaDataParser myMetaDataParser;

        private string _LastError;
        public string LastError { set { _LastError = value; } get { return _LastError; } }
                
        #endregion

        #region Constructors
        public PhoenixContactPLCIImportBase()
        {
            Init();
        }

        protected void Init()
        {
            _ImportDataModel = null;
            _lGlobalID = 0;
            ParsedVars = new List<ImportedVariable>();
            MapSTRUCT = new Dictionary<string, List<ImportedVariable>>();
            MapUDT = new Dictionary<string, string[]>();
            MapUDTI = new Dictionary<string, List<ImportedVariable>>();
            MapStructWitRifLisVar = new Dictionary<string, ParamRif>();
            myMetaDataParser = null;
        }

        #endregion

        #region Methods


        public ImportDataModelPhoenixContactPlci ImportFromBin(GetStationName readStationName, string binFile)
        {
            Init();
            // retrive vars definition, data type definition, ecc from symbolic file
            if (!ParseBinFile(binFile, out string errorMessage))
            {
                LastError = errorMessage;
                return _ImportDataModel;
            }

            // base method to fill list of imported var for user
            _ImportDataModel = ParsePlcData(readStationName);

            return _ImportDataModel;
        }

        private bool ParseMetaData(MetaElement myMetaElement, ref List<ImportedVariable> listImportVariables)
        {
            MetaElement refItem = null;

            if (listImportVariables == null)
            {
                return (false);
            }

            uint fieldAmount = myMetaElement.GetFieldAmount();

            bool bFirst = true;
            for (ushort i = 0; i < fieldAmount; i++)
            {
                if (bFirst == true)
                {
                    refItem = myMetaElement.GetChild();
                    System.Diagnostics.Debug.WriteLine(string.Format("{0}\t\t\tTypeName {1}  | Datatype {2:d} ArrayType {3} |Size {4} | IsArray = {5} | IsOPC = {6} | LB = {7} | UB = {8}",
                                                                    refItem.GetInstancePath(), refItem.GetTypeName(), refItem.GetDataType(), refItem.GetArrayTypeName(), refItem.GetDataSize(), Convert.ToInt16(refItem.IsArray()), Convert.ToInt16(refItem.IsOPC()), refItem.GetLowerBound(), refItem.GetUpperBound()));

                    if ((Convert.ToInt16(refItem.IsOPC()) == 1) || CheckAndInsertVarInAStructure(refItem.GetInstancePath()))
                    {
                        MetaElementType dataType = refItem.GetDataType();
                        if ((dataType >= MetaElementType.TypeBool) && (dataType < MetaElementType.TypePtr) && !string.IsNullOrEmpty(PhoenixContactPLCIProtocol.GetVarTypeByNumber((uint)refItem.GetDataType())))
                        {
                            AddVariableStandard(refItem, ref listImportVariables);
                        }
                        else if (dataType >= MetaElementType.TypeArray)
                        {
                            AddVariableArray(refItem, ref listImportVariables);
                        }
                        else if (dataType >= MetaElementType.TypeStruct)
                        {
                            AddVariableStructure(refItem, ref listImportVariables);
                        }
                    }
                    ParseMetaData(refItem, ref listImportVariables);
                    bFirst = false;
                }
                else
                {
                    refItem = refItem.GetNextElement();
                    System.Diagnostics.Debug.WriteLine(string.Format("{0}\t\t\tTypeName {1}  | Datatype {2:d} ArrayType {3} |Size {4} | IsArray = {5} | IsOPC = {6} | LB = {7} | UB = {8}",
                                                                    refItem.GetInstancePath(), refItem.GetTypeName(), refItem.GetDataType(), refItem.GetArrayTypeName(), refItem.GetDataSize(), Convert.ToInt16(refItem.IsArray()), Convert.ToInt16(refItem.IsOPC()), refItem.GetLowerBound(), refItem.GetUpperBound()));
                    if ((Convert.ToInt16(refItem.IsOPC()) == 1) || CheckAndInsertVarInAStructure(refItem.GetInstancePath()))
                    {
                        MetaElementType dataType = refItem.GetDataType();
                        if ((dataType >= MetaElementType.TypeBool) && (dataType < MetaElementType.TypePtr) && !string.IsNullOrEmpty(PhoenixContactPLCIProtocol.GetVarTypeByNumber((uint)refItem.GetDataType())))
                        {
                            AddVariableStandard(refItem, ref listImportVariables);
                        }
                        else if (dataType >= MetaElementType.TypeArray)
                        {
                            AddVariableArray(refItem, ref listImportVariables);
                        }
                        else if (dataType >= MetaElementType.TypeStruct)
                        {
                            AddVariableStructure(refItem, ref listImportVariables);
                        }
                    }
                    if (refItem.GetChild() != null)
                    {
                        ParseMetaData(refItem, ref listImportVariables);
                    }
                }
            }
            return (true);
        }

        private void AddVariableStandard(MetaElement refItem, ref List<ImportedVariable> listImportVariables)
        {

            string pathname = refItem.GetInstancePath();
            //Remove the sequence of characters "[{0}]" that are inserted in the path to distinguish that the path is an array type data.
            int found = pathname.IndexOf("[");
            if (found != -1)
            {
                pathname = pathname.Replace("[{0}]", "");
            }
            ImportedVariable var = new ImportedVariable();
            var.VariableName = pathname;
            var.VarType = PhoenixContactPLCIProtocol.GetVarTypeByNumber((uint)refItem.GetDataType());
            var.VarTypeOfStruct = string.Empty;
            var.Category = ImportUDTType.Unknown;
            var.TaskName = string.Empty;
            var.Description = string.Empty;
            var.VarTypeOfStruct = string.Empty;
            var.VarSize = refItem.GetDataSize();
            if (refItem.IsString())
            {
                //String variables cannot have zero length if they are not imported
                if ((var.VarSize < 7))
                {
                    return;
                }
                var.VarSize -= 6;
            }
            var.ElementsNumber = 0;
            var.ArrayDimCount = 0;
            var.ArrayStartIndexes = null;
            var.ArrayIndexLimits = null;
            bool bIsAddUdt = AddUDTVariable(var.VariableName, var, ref listImportVariables);
            if (!bIsAddUdt)
            {
                listImportVariables.Add(var);
            }

        }

        private void AddVariableArray(MetaElement refItem, ref List<ImportedVariable> listImportVariables)
        {
            string pathname = refItem.GetInstancePath();
            //Remove the sequence of characters "[{0}]" that are inserted in the path to distinguish that the path is an array type data.
            int found = pathname.IndexOf("[");
            if (found != -1)
            {
                pathname = pathname.Replace("[{0}]", "");
            }

            ImportedVariable var = new ImportedVariable();
            var.VariableName = pathname;
            var.VarType = refItem.GetArrayTypeName();
            var.VarTypeOfStruct = string.Empty;
            var.Category = ImportUDTType.Array;
            var.TaskName = string.Empty;
            var.Description = string.Empty;
            var.VarTypeOfStruct = string.Empty;
            var.VarSize = refItem.GetDataSize();

            if (refItem.IsString())
            {
                //The string array prototype must always be followed by the number of bytes in length, if it is not imported.
                string resultString = System.Text.RegularExpressions.Regex.Match(var.VarType, @"\d+").Value;
                uint value = 0;
                bool canConvert = uint.TryParse(resultString, out value);
                if (!canConvert)
                {
                    return;
                }
                var.VarSize = value;
                var.VarType = "STRING";
            }
            var.ElementsNumber = refItem.GetDataSize();
            uint dimensionArray = 1;
            var.ArrayDimCount = dimensionArray;
            uint[] limitLower = new uint[dimensionArray];
            uint[] limitUpper = new uint[dimensionArray];
            limitLower[0] = refItem.GetLowerBound();
            limitUpper[0] = refItem.GetUpperBound();
            var.ArrayStartIndexes = limitLower;
            var.ArrayIndexLimits = limitUpper;
            listImportVariables.Add(var);

            //Some methods of the library do not pretend, see the one to discriminate if it is a structure, 
            //so I thought it appropriate to use the following condition
            if (PhoenixContactPLCIProtocol.GetVarType(refItem.GetArrayTypeName()) == PhoenixContactPLCIProtocol.VarType.UNKNOWN)
            {
                ParamRif pr = new ParamRif();
                pr.OffsetOfList = listImportVariables.Count;
                listImportVariables[pr.OffsetOfList - 1].VarTypeOfStruct = refItem.GetTypeName();
                pr.NumberOfElement = 0;
                pr.structType = var.VarTypeOfStruct;
                MapStructWitRifLisVar[var.VariableName] = pr;
                if (!MapUDTI.ContainsKey(var.VarTypeOfStruct))
                {
                    List<ImportedVariable> ListVar = new List<ImportedVariable>();
                    MapUDTI[var.VarTypeOfStruct] = ListVar;
                }
            }
        }

        private void AddVariableStructure(MetaElement refItem, ref List<ImportedVariable> listImportVariables)
        {
            string pathname = refItem.GetInstancePath();
            //Remove the sequence of characters "[{0}]" that are inserted in the path to distinguish that the path is an array type data.
            int found = pathname.IndexOf("[");
            if (found != -1)
            {
                pathname = pathname.Replace("[{0}]", "");
            }

            ImportedVariable var = new ImportedVariable();
            var.VariableName = pathname;
            var.VarType = PhoenixContactPLCIProtocol.GetVarTypeByNumber((uint)refItem.GetDataType());
            var.VarTypeOfStruct = refItem.GetTypeName();
            var.Category = ImportUDTType.Struct;
            var.TaskName = string.Empty;
            var.Description = string.Empty;
            var.VarSize = refItem.GetDataSize();
            var.ElementsNumber = 1;
            var.ArrayDimCount = 0;
            var.ArrayStartIndexes = null;
            var.ArrayIndexLimits = null;
            var.VarTypeOfStruct = refItem.GetTypeName();
            bool bIsAddUdt = AddUDTVariable(var.VariableName, var, ref listImportVariables);
            if (!bIsAddUdt)
            {
                listImportVariables.Add(var);
            }
            ParamRif pr = new ParamRif();
            pr.OffsetOfList = listImportVariables.Count;
            pr.NumberOfElement = 0;
            pr.structType = var.VarTypeOfStruct;
            MapStructWitRifLisVar[pathname] = pr;
            if (!MapUDTI.ContainsKey(var.VarTypeOfStruct))
            {
                List<ImportedVariable> ListVar = new List<ImportedVariable>();
                MapUDTI[var.VarTypeOfStruct] = ListVar;
            }
        }

        private bool CheckAndInsertVarInAStructure(string name)
        {
            bool bRet = false;
            string strWithoutMember = string.Empty;
            //Remove the sequence of characters "[{0}]" that are inserted in the path to distinguish that the path is an array type data.
            int lastOfChar = name.LastIndexOf("[");
            if (lastOfChar != -1)
            {
                name = name.Replace("[{0}]", "");
            }

            //I remove the member name from the full address path
            lastOfChar = name.LastIndexOf(".");
            strWithoutMember = name.Substring(0, lastOfChar);


            foreach (var item in MapStructWitRifLisVar)
            {
                if (strWithoutMember.Equals(item.Key))
                {
                    MapStructWitRifLisVar[item.Key].NumberOfElement += 1;
                    bRet = true;
                }
            }
            return (bRet);
        }

        private bool AddUDTVariable(string name, ImportedVariable var, ref List<ImportedVariable> listImportVariables)
        {
            bool bRet = false;

            //Remove the member name from the full address path
            int lastOfChar = name.LastIndexOf(".");
            string strWithoutMember = name.Substring(0, lastOfChar);

            //I flow at the map of the structures, with reference to the Importvarlist , for extract the type of UDT
            foreach (var item in MapStructWitRifLisVar)
            {
                if (strWithoutMember.Equals(item.Key))
                {
                    MapStructWitRifLisVar[item.Key].NumberOfElement += 1;

                    //I extract the name of the structure from the address without the name of the member
                    string nameStruct = MapStructWitRifLisVar[item.Key].structType;

                    if (MapUDTI.ContainsKey(nameStruct))
                    {
                        //I divide the name of the member from the address to insert it in the list of udt
                        int LastIndexOfChar = var.VariableName.LastIndexOf(".");
                        var.VariableName = var.VariableName.Substring(LastIndexOfChar + 1);
                        bool bAddVarible = true;

                        foreach (var varLis in MapUDTI[nameStruct])
                        {
                            if (varLis.VariableName.Equals(var.VariableName))
                            {
                                bAddVarible = false;
                                break;
                            }
                        }
                        if (bAddVarible)
                        {
                            System.Diagnostics.Debug.WriteLine("--- InserVariable structure name {0} - Variable name {1}", nameStruct, var.VariableName);
                            MapUDTI[nameStruct].Add(var);
                        }
                    }
                    bRet = true;
                }
            }
            return (bRet);
        }

        protected bool ParseBinFile(string binFile, out string error)
        {
            error = string.Empty;

            //reading from the file
            byte[] byteData = null;
            try
            {
                byteData = File.ReadAllBytes(binFile);
            }
            catch (IOException e)
            {
                return false;
            }

            List<ImportedVariable> listImportVariables = new List<ImportedVariable>();
            //MapStructWitRifLisVar = new Dictionary<string, ParamRif>();

            myMetaDataParser = new MetaDataParser();
            myMetaDataParser.Init(byteData);

            MetaElement myMetaElement = myMetaDataParser.GetMetaRoot();
            myMetaElement = myMetaElement.GetChild();

            //parse global variables;
            if (!ParseMetaData(myMetaElement, ref listImportVariables))
            {
                error = Properties.Resources.ErrorImportParsingGlobalVariables;
                return false;
            }

            //parse instance/local variables
            myMetaElement = myMetaDataParser.GetMetaRoot();
            myMetaElement = myMetaElement.GetChild();
            myMetaElement = myMetaElement.GetNextElement();
            if (!ParseMetaData(myMetaElement, ref listImportVariables))
            {
                error = Properties.Resources.ErrorImportParsingInstanceVariables;
                return false;
            }

            ParsedVars.AddRange(listImportVariables);
            MapSTRUCT = MapUDTI;
            
            return true;
        }

        /// <summary>
        /// Test if selected file is a valid boot image file 
        /// </summary>
        /// <returns></returns>
        public bool IsBootImageFile(string file)
        {
            bool isValid = false;

            try
            {
                //reading from the file
                byte[] byteData = null;
                try
                {
                    byteData = File.ReadAllBytes(file);
                }
                catch (IOException e)
                {
                    return false;
                }

                myMetaDataParser = new MetaDataParser();
                myMetaDataParser.Init(byteData);

                using (MetaElement myMetaElement = myMetaDataParser.GetMetaRoot())
                {
                    isValid = true;
                }
            }
            catch (Exception ex)
            {
                isValid = false;
            }
            finally
            {
                if (myMetaDataParser != null)
                    myMetaDataParser.Dispose();
            }

            return (isValid);
        }

        public virtual bool ImportFailed()
        {
            return (!string.IsNullOrEmpty(_LastError) || _ImportDataModel == null);
        }

        protected void RemoveArrayOfStructInSubStruct()
        {
            foreach (var MembersOfStruct in MapSTRUCT.Values)
                MembersOfStruct.RemoveAll(a => a.Category == ImportUDTType.ArrayOfStruct);

            // try to remove struct with no members
            foreach (var tag in MapSTRUCT.Where(a => a.Value.Count == 0).ToList().AsParallel())
            {
                MapSTRUCT.Remove(tag.Key);
                // remove alla vars with this data type
                ParsedVars.RemoveAll(a => a.Category == ImportUDTType.ArrayOfStruct && a.VarTypeOfStruct == tag.Key);
            }
        }
                
        protected ImportDataModelPhoenixContactPlci ParsePlcData(GetStationName readStationName)
        {
            _ImportDataModel = new ImportDataModelPhoenixContactPlci(readStationName);

            // remove array of struct from struct's members --> not supported from Movicon prototype (remove empty struct)
            RemoveArrayOfStructInSubStruct();

            // add var into ImportData and into TreeView
            foreach (var Var in ParsedVars)
                AddPlcVariable(Var);

            return _ImportDataModel;
        }

        private int GetMoviconType(string varType)
        {
            PhoenixContactPLCIProtocol.VarType V = PhoenixContactPLCIProtocol.GetVarType(varType);
            if (V == PhoenixContactPLCIProtocol.VarType.UNKNOWN)
                return -1;
             else 
                return (int)PhoenixContactPLCIProtocol.GetDataType(V);
        }

        private void AddTreeItem(ImportData tag, ImportData parent = null)
        {
            tag.Parent = parent;
            //((ImportDataPhoenixContactPlci)tag).strId = tag.Id.ToString("X6");
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
            //((ImportDataPhoenixContactPlci)tag).strLevel = tag.TreeLevel.ToString("X2");
        }

        protected void AddArray(string szNamePrefix,
                                string szAddressPrefix,
                                string szTaskName,
                                string szArrayName,
                                string szElemType,
                                int moviconType,
                                uint nElemSize,
                                DriverCodeBase.Enumerators.LinkType jobType,
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
            ImportData v = _ImportDataModel.addImportData();
            ((ImportDataPhoenixContactPlci)v).PreName = szNamePrefix;
            v.Name = szArrayName;            
            if (szElemType.Equals("STRING"))
            {
                ((ImportDataPhoenixContactPlci)v).Size = nElemSize;
            }
            else
            {
                ((ImportDataPhoenixContactPlci)v).Size = elementsNumber * nElemSize;
            }
            v.szType = "ARRAY ";
            for (int Index = 0; Index < startIndexes.Length; Index++)
                v.szType += string.Format("[{0}..{1}]", startIndexes[Index], indexLimits[Index]);
            v.szType += " OF " + szElemType;
            //v.szTypeView = GetMoviconTypeFromPlcType(v.szType);
            ((ImportDataPhoenixContactPlci)v).Task = szTaskName;
            ((ImportDataPhoenixContactPlci)v).ElemType = szElemType.ToString();
            ((ImportDataPhoenixContactPlci)v).Type = (DataType)moviconType;
            v.Address = szAddress;
            v.Description = String.Empty;
            v.parentId = parentId;
            v.Id = _lGlobalID++;
            v.ArrayDimension = elementsNumber;
            ((ImportDataPhoenixContactPlci)v).ImportDataType = ImportTypes.Array;
            ((ImportDataPhoenixContactPlci)v).JobType = jobType;
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

        protected void AddArrayElements(string szNamePrefix,
                              string szTaskName,
                              string varNamePrefix,
                              string addressPrefix,
                              int parentId,
                              uint nDimCount,
                              uint currentDim,
                              string szElemType,
                              int moviconType,
                              uint nElemSize,
                              DriverCodeBase.Enumerators.LinkType jobType,
                              uint[] startIndexes,
                              uint[] indexLimits,
                              ImportData inRootItem)
        {
            for (uint i = startIndexes[currentDim]; i <= indexLimits[currentDim]; i++)
            {
                //uint RealArrayIndex = i - startIndexes[currentDim];
                ////in multi dimension array, from 2nd dimension, use start index (standard array, always start from 0)
                //if (currentDim > 0)
                //  RealArrayIndex = i;
                string szAddress = String.Format("{0}{1}", addressPrefix, i);
                string varName = String.Format("{0}{1}", varNamePrefix, i);
                if (currentDim == (nDimCount - 1))
                {
                    varName += "]";
                    szAddress += "]";
                    AddStandardVar(szNamePrefix,
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

        private void AddPlcVariable(ImportedVariable var)
        {
            int moviconType;

            DriverCodeBase.Enumerators.LinkType jobType = DriverCodeBase.Enumerators.LinkType.InputOutput;

            // Set the first part of the variable name
            string namePrefix = String.Empty;
            if (!String.IsNullOrWhiteSpace(var.TaskName))
            {
                namePrefix = var.TaskName + "_";
            }

            // Structure?
            if (PhoenixContactPLCIProtocol.IsPlcStructureType(var.VarType))
            {
                // Add the structure variable and its elements to the tree
                AddPviStructureVariable(namePrefix,
                                        var.TaskName,
                                        var.VariableName,
                                        var.VarTypeOfStruct,
                                        var.VarSize,
                                        var, //string.Empty,
                                        jobType,
                                        var.ElementsNumber,
                                        -1,
                                        null);
                return;
            }

            // Array
            if (var.ElementsNumber > 1)
            {
                // Add the array variable and its elements to the tree
                AddArrayVariable(namePrefix,
                                    String.Empty,
                                    var.TaskName,
                                    var.VariableName,
                                    var.VarType,
                                    var,
                                    jobType,
                                    var.ElementsNumber,
                                    var.VarSize,
                                    -1,
                                    null);
                return;
            }

            // Standard variable
            // Get the corresponding MOVICON type
            moviconType = GetMoviconType(var.VarType);
            // Invalid Type?
            if (moviconType == -1)
            {
                return;
            }

            // Add the variable to the tree
            AddStandardVar(namePrefix,
                              var.VariableName,
                              var.VariableName,
                              var.TaskName,
                              String.Empty,
                              var.VarType,
                              moviconType,
                              var.VarSize,
                              jobType,
                              -1,
                              null);
            }

            private void AddPviStructureVariable(string namePrefix,
                                     string taskName,
                                     string variableName,
                                     string variableType,
                                     uint nVarSize,
                                     ImportedVariable typeDefinition, // string typeDefinition,
                                     DriverCodeBase.Enumerators.LinkType jobType,
                                     uint elementsNumber,
                                     int parentId = -1,
                                     ImportData inRootItem = null)
            {

            //// Parse the definitions of the structure elements
            //int searchIndex = typeDefinition.IndexOf('{');
            //if (searchIndex < 0)
            //{
            //    return;
            //}

            //string auxString = typeDefinition.Substring(searchIndex);
            //char[] splitParameters = new char[1];
            //splitParameters[0] = '{';
            //string[] structureElements = typeDefinition.Split(splitParameters);
            //if (structureElements.Count() < 1)
            //{
            //    return;
            //}

            // If needed, add the task name to the type name
            string completeTypeName = String.Empty;
            if (!String.IsNullOrWhiteSpace(taskName))
            {
                completeTypeName = taskName + "_" + variableType;
            }
            else
            {
                completeTypeName = variableType.ToString();
            }

            // Simple structure?
            if (elementsNumber == 1)
            {
                List<ImportedVariable> structureElements = GetStructElements(typeDefinition.VarTypeOfStruct.ToString());
                if (structureElements == null || structureElements.Count == 0)
                    return;

                // Build the structure variable to be added to the tree
                ImportData v = _ImportDataModel.addImportData();
                ((ImportDataPhoenixContactPlci)v).PreName = namePrefix;
                ((ImportDataPhoenixContactPlci)v).Name = variableName;
                ((ImportDataPhoenixContactPlci)v).Size = nVarSize;
                v.szType = completeTypeName;
                //v.szTypeView = GetMoviconTypeFromPlcType(v.szType);
                ((ImportDataPhoenixContactPlci)v).Task = taskName;
                ((ImportDataPhoenixContactPlci)v).ElemType = completeTypeName;
                v.Address = variableName;
                v.Description = String.Empty;
                v.parentId = parentId;
                v.Id = _lGlobalID++;
                v.ArrayDimension = 0;
                ((ImportDataPhoenixContactPlci)v).ImportDataType = ImportTypes.StructOrEnum;
                ((ImportDataPhoenixContactPlci)v).JobType = jobType;

                // Add the structure elements to the tree
                string addressPrefix = variableName;
                AddStructureElements(namePrefix,
                                        taskName,
                                        addressPrefix,
                                        structureElements,
                                        jobType,
                                        v.Id,
                                        inRootItem,
                                        v);
            }
            // Array of Structures
            else
            {
                // Add the array elements to the tree
                AddStructureArray(namePrefix,
                                     variableName,
                                     taskName,
                                     completeTypeName,
                                     new List<ImportedVariable>() { typeDefinition },
                                     jobType,
                                     elementsNumber,
                                     parentId,
                                     inRootItem);
            }
        }

        private void AddStructureElements(string namePrefix,
                                     string taskName,
                                     string addressPrefix,
                                     List<ImportedVariable> structureElements, //string[] structureElements,
                                     DriverCodeBase.Enumerators.LinkType jobType,
                                     int parentId,
                                     ImportData inParentRootItem,
                                     ImportData inRootItem)
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
                if (PhoenixContactPLCIProtocol.IsPlcStructureType(varType))
                {
                    varType = structElement.VarTypeOfStruct;

                    string completeTypeName = String.Empty;
                    if (!String.IsNullOrWhiteSpace(taskName))
                    {
                        completeTypeName = taskName + "_" + varType;
                    }
                    else
                    {
                        completeTypeName = varType.ToString();
                    }

                    // Array of structures?
                    if (elementsNumber > 1)
                    {
                        List<ImportedVariable> substructureDefinition = new List<ImportedVariable>();
                        substructureDefinition.Add(structElement);
                        substructureDefinition.AddRange(GetStructElements(structElement.VarTypeOfStruct.ToString()));
                        // no elements
                        if (substructureDefinition.Count == 1)
                            return;
                        AddStructureArray(varPreName,
                                             elementName,
                                             taskName,
                                             completeTypeName,
                                             substructureDefinition,
                                             jobType,
                                             elementsNumber,
                                             -1,
                                             null);
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
                        ((ImportDataPhoenixContactPlci)v).PreName = varPreName;
                        v.Name = elementName;
                        ((ImportDataPhoenixContactPlci)v).Size = variableLength;
                        v.szType = completeTypeName;
                        //v.szTypeView = GetMoviconTypeFromPlcType(v.szType);
                        ((ImportDataPhoenixContactPlci)v).Task = taskName;
                        ((ImportDataPhoenixContactPlci)v).ElemType = completeTypeName;
                        v.Address = elementAddress;
                        v.Description = String.Empty;
                        v.parentId = parentId;
                        v.Id = _lGlobalID++;
                        v.ArrayDimension = 0;
                        ((ImportDataPhoenixContactPlci)v).ImportDataType = ImportTypes.StructOrEnum;
                        ((ImportDataPhoenixContactPlci)v).JobType = jobType;

                        // Add the substructure elements to the tree
                        AddStructureElements(namePrefix,
                                                taskName,
                                                elementAddress,
                                                substructureElements,
                                                jobType,
                                                v.Id,
                                                inRootItem,
                                                v);
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
                        AddArrayVariable(varPreName,
                                            addrPrefix,
                                            taskName,
                                            elementName,
                                            varType,
                                            structElement,
                                            jobType,
                                            elementsNumber,
                                            variableLength,
                                            parentId,
                                            inRootItem);
                    }
                    else // Standard variable
                    {
                        // Add the standard variable to the tree
                        AddStandardVar(varPreName,
                                          elementName,
                                          elementAddress,
                                          taskName,
                                          String.Empty, // No description
                                          varType,
                                          moviconType,
                                          variableLength,
                                          jobType,
                                          parentId,
                                          inRootItem);
                    }
                }
            }
        }


        private void AddArrayVariable(string namePrefix,
                                 string addressPrefix,
                                 string taskName,
                                 string variableName,
                                 string elementType,
                                 ImportedVariable typeDefinition, //string typeDefinition,
                                 DriverCodeBase.Enumerators.LinkType jobType,
                                 uint elementsNumber,
                                 uint elementSize,
                                 int parentId = -1,
                                 ImportData inRootItem = null)
        {
            // Get the MOVICON type corresponding to the element type
            int moviconType = GetMoviconType(elementType);
            // Invalid Type?
            if (moviconType == -1)
            {
                return;
            }

            // Add the variable to the tree
            AddArray(namePrefix,
                        addressPrefix,
                        taskName,
                        variableName,
                        elementType,
                        moviconType,
                        elementSize,
                        jobType,
                        elementsNumber,
                        typeDefinition.ArrayDimCount,
                        typeDefinition.ArrayStartIndexes,
                        typeDefinition.ArrayIndexLimits,
                        parentId,
                        inRootItem);
        }

        private void AddStructureArray(string namePrefix,
                                  string variableName,
                                  string taskName,
                                  string completeTypeName,
                                  List<ImportedVariable> structureElements, //string[] structureElements,
                                  DriverCodeBase.Enumerators.LinkType jobType,
                                  uint elementsNumber,
                                  int parentId,
                                  ImportData inRootItem)
        {

            string varNamePrefix = String.Format("{0}[", variableName);
            string addrPrefix = String.Format("{0}[", namePrefix + variableName);

            AddStructureArrayElements(namePrefix,
                                         varNamePrefix,
                                         addrPrefix,
                                         taskName,
                                         completeTypeName,
                                         structureElements,
                                         jobType,
                                         elementsNumber,
                                         structureElements[0].ArrayDimCount,
                                         0,
                                         structureElements[0].ArrayStartIndexes,
                                         structureElements[0].ArrayIndexLimits,
                                         parentId,
                                         inRootItem);//,
                                                     //pviObject);
        }

        private void AddStructureArrayElements(string namePrefix,
                                  string varNamePrefix,
                                  string addrPrefix,
                                  string taskName,
                                  string completeTypeName,
                                  List<ImportedVariable> structureElements, //string[] structureElements,
                                  DriverCodeBase.Enumerators.LinkType jobType,
                                  uint elementsNumber,
                                  uint arrayDimCount,
                                  uint currentDim,
                                  uint[] arrayStartIndexes,
                                  uint[] arrayIndexLimits,
                                  int parentId,
                                  ImportData inRootItem) //,
                                                         //BrPviASImportParser.ImportedVariable pviObject)
        {
            for (uint i = arrayStartIndexes[currentDim]; i <= arrayIndexLimits[currentDim]; i++)
            {                
                string szAddress = String.Format("{0}{1}", addrPrefix, i);
                string varName = String.Format("{0}{1}", varNamePrefix, i);
                if (currentDim == (arrayDimCount - 1))
                {
                    varName += "]";
                    szAddress += "]";
                    AddStructureArraySingleElement(namePrefix,
                                                      varName,
                                                      szAddress,
                                                      taskName,
                                                      completeTypeName,
                                                      structureElements,
                                                      jobType,
                                                      parentId,
                                                      inRootItem);
                }
                else
                {
                    varName += ",";
                    szAddress += ",";
                    AddStructureArrayElements(namePrefix,
                                                 varName,
                                                 szAddress,
                                                 taskName,
                                                 completeTypeName,
                                                 structureElements,
                                                 jobType,
                                                 elementsNumber,
                                                 arrayDimCount,
                                                 currentDim + 1,
                                                 arrayStartIndexes,
                                                 arrayIndexLimits,
                                                 parentId,
                                                 inRootItem);
                }
            }
        }

        private void AddStructureArraySingleElement(string namePrefix,
                                               string varName,
                                               string szAddress,
                                               string taskName,
                                               string completeTypeName,
                                               List<ImportedVariable> structureElements, //string[] structureElements,
                                               DriverCodeBase.Enumerators.LinkType jobType,
                                               int parentId,
                                               ImportData inRootItem)
        {

            uint variableLength = structureElements[0].VarSize;

            // Build the structure variable to be added to the tree
            ImportData v = _ImportDataModel.addImportData();
            ((ImportDataPhoenixContactPlci)v).PreName = namePrefix;
            v.Name = varName;
            ((ImportDataPhoenixContactPlci)v).Size = variableLength;
            ((ImportDataPhoenixContactPlci)v).szType = completeTypeName;
            //v.szTypeView = GetMoviconTypeFromPlcType(v.szType);
            ((ImportDataPhoenixContactPlci)v).Task = taskName;
            ((ImportDataPhoenixContactPlci)v).ElemType = completeTypeName;
            v.Address = szAddress;
            v.Description = String.Empty;
            v.parentId = parentId;
            v.Id = _lGlobalID++;
            v.ArrayDimension = 0;
            ((ImportDataPhoenixContactPlci)v).ImportDataType = ImportTypes.StructOrEnum;
            ((ImportDataPhoenixContactPlci)v).JobType = jobType;

            // Add the structure elements to the tree
            if (structureElements.Count() < 1) // davide 2)
            {
                return;
            }

            List<ImportedVariable> elementDefinitions = GetStructElements(structureElements[0].VarTypeOfStruct.ToString());
            if (elementDefinitions == null || elementDefinitions.Count == 0)
                return;

            string addressPrefix = szAddress;
            AddStructureElements(namePrefix,
                                    taskName,
                                    addressPrefix,
                                    elementDefinitions,
                                    jobType,
                                    v.Id,
                                    inRootItem,
                                    v);
        }

        private void AddStandardVar(string szNamePrefix,
                                       string szVarName,
                                       string szAddress,
                                       string taskName,
                                       string description,
                                       string szVarType,
                                       int moviconType,
                                       uint nVarSize,
                                       DriverCodeBase.Enumerators.LinkType jobType,
                                       int parentId = -1,
                                       ImportData inRootItem = null)
        {
            ImportData v = _ImportDataModel.addImportData();
            ((ImportDataPhoenixContactPlci)v).PreName = szNamePrefix;
            v.Name = szVarName;
            ((ImportDataPhoenixContactPlci)v).Size = nVarSize;
            v.szType = szVarType.ToString();
            //v.szTypeView = GetMoviconTypeFromPlcType(v.szType);

            ((ImportDataPhoenixContactPlci)v).Task = taskName;
            ((ImportDataPhoenixContactPlci)v).ElemType = szVarType.ToString();
            ((ImportDataPhoenixContactPlci)v).Type = (DataType)moviconType;            
            v.Address = szAddress;
            v.Description = description;
            v.parentId = parentId;
            v.Id = _lGlobalID++;
            v.ArrayDimension = 0;
            ((ImportDataPhoenixContactPlci)v).ImportDataType = ImportTypes.Standard;
            ((ImportDataPhoenixContactPlci)v).JobType = jobType;
            //With case resolution 28498 it is no longer necessary to split strings from string structures and arrays
            AddTreeItem(v, inRootItem);            
        }

        private List<ImportedVariable> GetStructElements(string structName)
        {
            if (MapSTRUCT.ContainsKey(structName))
                return MapSTRUCT[structName];
            else
                return new List<ImportedVariable>();
        }


        public virtual void Dispose()
        {
            if (myMetaDataParser != null)
            {
                myMetaDataParser.Dispose();
                myMetaDataParser = null;
            }
            if (_ImportDataModel != null)
                _ImportDataModel = null;
        }
    }
    #endregion
}
