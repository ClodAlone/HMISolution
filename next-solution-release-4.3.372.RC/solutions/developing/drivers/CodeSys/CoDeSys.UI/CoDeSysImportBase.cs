using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using UFUAModel;
using DriverCodeBase.UI;

namespace CoDeSys.UI
{
    public class CoDeSysImportBase
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

        public class ImportPlcSymbolDesc : ICloneable
        {
            public string pszName;
            public uint ulTypeId;   //public ulong ulOffset;
            public string pszType;
            public ushort usRefId;
            public uint ulOffset;  //public ulong ulOffset;
            public uint ulSize;
            public byte[] szAccess;
            public sbyte bySwapSize;

            private void Default()
            {
                pszName = string.Empty;
                ulTypeId = 0;
                pszType = string.Empty;
                usRefId = 0;
                ulOffset = 0;
                ulSize = 0;
                szAccess = new byte[2];
                bySwapSize = 0;
            }

            public ImportPlcSymbolDesc()
            {
                Default();
            }

            public ImportPlcSymbolDesc(CoDeSysPLCHandlerWrapper.PlcSymbolDesc value)
            {
                Default();
                pszName = value.pszName;
                ulTypeId = value.ulTypeId;
                pszType = value.pszType;
                usRefId = value.usRefId;
                ulOffset = value.ulOffset;
                ulSize = value.ulSize;
                szAccess = new byte[2];
                bySwapSize = value.bySwapSize;
            }
            
            #region ICloneable Members
            public object Clone()
            {
                return MemberwiseClone();
            }

            public ImportPlcSymbolDesc CastedClone()
            {
                return (ImportPlcSymbolDesc)MemberwiseClone();
            }
            #endregion
        }

        public class CDSImportedVariable : ICloneable, IComparable<CDSImportedVariable>
        {
            public string VariableName { get; set; }
            // TaskName not used --> only for function signature compatibility
            public string TaskName { get; set; }
            //public string Description { get; set; }
            public string VarType { get; set; }
            public ImportUDTType Category { get; set; }
            public string VarTypeOfStruct { get; set; }
            public uint VarSize { get; set; }
            public uint ElementsNumber { get; set; }
            public uint ArrayDimCount { get; set; }
            public uint[] ArrayStartIndexes { get; set; }
            public uint[] ArrayIndexLimits { get; set; }

            public CDSImportedVariable()
            {
                VariableName = string.Empty;
                TaskName = string.Empty;
                //Description = string.Empty;
                VarType = string.Empty;
                Category = ImportUDTType.Unknown;
                VarTypeOfStruct = string.Empty;
                VarSize = 0;
                ElementsNumber = 0;
                ArrayDimCount = 0;
                ArrayStartIndexes = null;
                ArrayIndexLimits = null;
            }

            #region ICloneable Members
            public object Clone()
            {
                return MemberwiseClone();
            }

            public CDSImportedVariable CastedClone()
            {
                return (CDSImportedVariable)MemberwiseClone();
            }
            #endregion

            #region IComparable
            public int CompareTo(CDSImportedVariable other)
            {
                return String.Compare(this.VariableName , other.VariableName);
            }
            #endregion
        }

        protected ImportDataModelCoDeSys _ImportDataModel;
        protected int _lGlobalID;

        public List<CDSImportedVariable> ParsedVars;
        public Dictionary<string, List<CDSImportedVariable>> MapSTRUCT;
        public Dictionary<string, string[]> MapUDT;
        public Dictionary<string, int> MapConstants;

        private string _LastError;
        public string LastError { set { _LastError = value; } get { return _LastError; } }
                
        #endregion

        #region Constructors
        public CoDeSysImportBase()
        {
            _ImportDataModel = null;
            _lGlobalID = 0;
            ParsedVars = new List<CDSImportedVariable>();
            MapSTRUCT = new Dictionary<string, List<CDSImportedVariable>>();
            MapUDT = new Dictionary<string, string[]>();
            MapConstants = new Dictionary<string, int>();            
        }

        #endregion

        #region Methods

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
                
        protected ImportDataModelCoDeSys ParsePlcData(GetStationName readStationName)
        {
            _ImportDataModel = new ImportDataModelCoDeSys(readStationName);

            // remove array of struct from struct's members --> not supported from Movicon prototype (remove empty struct)
            RemoveArrayOfStructInSubStruct();

            // add var into ImportData and into TreeView
            foreach (var Var in ParsedVars)
                AddPlcVariable(Var);

            return _ImportDataModel;
        }

        protected int GetMoviconType(string varType)
        {
            CoDeSysProtocol.VarType V = CoDeSysProtocol.GetVarType(varType);
            if (V == CoDeSysProtocol.VarType.VAR_TYPE_E_UNKNOWN)
                return -1;
             else 
                return (int)CoDeSysProtocol.GetDataType(V);
        }

        protected void AddTreeItem(ImportData tag, ImportData parent = null)
        {
            tag.Parent = parent;
            ((ImportDataCoDeSys)tag).strId = tag.Id.ToString("X6");
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
            //((ImportDataCoDeSys)tag).strLevel = tag.TreeLevel.ToString("X2");
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
            ((ImportDataCoDeSys)v).PreName = szNamePrefix;
            v.Name = szArrayName;
            ((ImportDataCoDeSys)v).Size = elementsNumber * nElemSize;
            v.szType = "ARRAY ";
            for (int Index = 0; Index < startIndexes.Length; Index++)
                v.szType += string.Format("[{0}..{1}]", startIndexes[Index], indexLimits[Index]-1);
            v.szType += " OF " + szElemType;
            //v.szTypeView = GetMoviconTypeFromPlcType(v.szType);
            ((ImportDataCoDeSys)v).Task = szTaskName;
            ((ImportDataCoDeSys)v).ElemType = szElemType.ToString();
            ((ImportDataCoDeSys)v).Type = (DataType)moviconType;
            v.Address = szAddress;
            v.Description = String.Empty;
            v.parentId = parentId;
            v.Id = _lGlobalID++;
            v.ArrayDimension = elementsNumber;
            ((ImportDataCoDeSys)v).ImportDataType = ImportTypes.Array;
            ((ImportDataCoDeSys)v).JobType = jobType;
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
            for (uint i = startIndexes[currentDim]; i < indexLimits[currentDim]; i++)
            {
                //uint RealArrayIndex = i - startIndexes[currentDim];
                ////in multidimension array, from 2nd dimension, use start index (standard array, always start from 0)
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

        protected void AddPlcVariable(CoDeSysSFImportParser.CDSImportedVariable var)
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
            if (CoDeSysProtocol.IsPlcStructureType(var.VarType))
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

            protected void AddPviStructureVariable(string namePrefix,
                                     string taskName,
                                     string variableName,
                                     string variableType,
                                     uint nVarSize,
                                     CoDeSysSFImportParser.CDSImportedVariable typeDefinition, // string typeDefinition,
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
            if ((elementsNumber == 1) && (typeDefinition.Category != ImportUDTType.ArrayOfStruct))
            {
                List<CoDeSysSFImportParser.CDSImportedVariable> structureElements = GetStructElements(typeDefinition.VarTypeOfStruct.ToString());
                if (structureElements == null || structureElements.Count == 0)
                    return;

                // Build the structure variable to be added to the tree
                ImportData v = _ImportDataModel.addImportData();
                ((ImportDataCoDeSys)v).PreName = namePrefix;
                v.Name = variableName;
                ((ImportDataCoDeSys)v).Size = nVarSize;
                v.szType = completeTypeName;
                //v.szTypeView = GetMoviconTypeFromPlcType(v.szType);
                ((ImportDataCoDeSys)v).Task = taskName;
                ((ImportDataCoDeSys)v).ElemType = completeTypeName;
                v.Address = variableName;
                v.Description = String.Empty;
                v.parentId = parentId;
                v.Id = _lGlobalID++;
                v.ArrayDimension = 0;
                ((ImportDataCoDeSys)v).ImportDataType = ImportTypes.StructOrEnum;
                ((ImportDataCoDeSys)v).JobType = jobType;

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
            else if (elementsNumber >= 1)
            {
                // Add the array elements to the tree
                AddStructureArray(namePrefix,
                                     variableName,
                                     taskName,
                                     completeTypeName,
                                     new List<CoDeSysSFImportParser.CDSImportedVariable>() { typeDefinition },
                                     jobType,
                                     elementsNumber,
                                     parentId,
                                     inRootItem);
            }
        }

        protected void AddStructureElements(string namePrefix,
                                     string taskName,
                                     string addressPrefix,
                                     List<CoDeSysSFImportParser.CDSImportedVariable> structureElements, //string[] structureElements,
                                     DriverCodeBase.Enumerators.LinkType jobType,
                                     int parentId,
                                     ImportData inParentRootItem,
                                     ImportData inRootItem)
        {
            bool parentStructHasBeenAddedToTheTree = false;

            for (int i = 0; i < structureElements.Count(); i++)
            {
                CoDeSysSFImportParser.CDSImportedVariable structElement = structureElements[i];

                string auxString = String.Format(".{0}", structElement.VariableName);
                string elementName = structElement.VariableName;
                string elementAddress = addressPrefix + auxString;
                string varPreName = namePrefix + addressPrefix + ".";

                string varType = structElement.VarType;


                uint variableLength = structElement.VarSize;

                uint elementsNumber = structElement.ElementsNumber;

                // Structure?
                if (CoDeSysProtocol.IsPlcStructureType(varType))
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
                        List<CoDeSysSFImportParser.CDSImportedVariable> substructureDefinition = new List<CoDeSysSFImportParser.CDSImportedVariable>();
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

                        List<CoDeSysSFImportParser.CDSImportedVariable> substructureElements = GetStructElements(structElement.VarTypeOfStruct.ToString());
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
                        ((ImportDataCoDeSys)v).PreName = varPreName;
                        v.Name = elementName;
                        ((ImportDataCoDeSys)v).Size = variableLength;
                        v.szType = completeTypeName;
                        //v.szTypeView = GetMoviconTypeFromPlcType(v.szType);
                        ((ImportDataCoDeSys)v).Task = taskName;
                        ((ImportDataCoDeSys)v).ElemType = completeTypeName;
                        v.Address = elementAddress;
                        v.Description = String.Empty;
                        v.parentId = parentId;
                        v.Id = _lGlobalID++;
                        v.ArrayDimension = 0;
                        ((ImportDataCoDeSys)v).ImportDataType = ImportTypes.StructOrEnum;
                        ((ImportDataCoDeSys)v).JobType = jobType;

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


        protected void AddArrayVariable(string namePrefix,
                                 string addressPrefix,
                                 string taskName,
                                 string variableName,
                                 string elementType,
                                 CoDeSysSFImportParser.CDSImportedVariable typeDefinition, //string typeDefinition,
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

        protected void AddStructureArray(string namePrefix,
                                  string variableName,
                                  string taskName,
                                  string completeTypeName,
                                  List<CoDeSysSFImportParser.CDSImportedVariable> structureElements, //string[] structureElements,
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

        protected void AddStructureArrayElements(string namePrefix,
                                  string varNamePrefix,
                                  string addrPrefix,
                                  string taskName,
                                  string completeTypeName,
                                  List<CoDeSysSFImportParser.CDSImportedVariable> structureElements, //string[] structureElements,
                                  DriverCodeBase.Enumerators.LinkType jobType,
                                  uint elementsNumber,
                                  uint arrayDimCount,
                                  uint currentDim,
                                  uint[] arrayStartIndexes,
                                  uint[] arrayIndexLimits,
                                  int parentId,
                                  ImportData inRootItem) //,
                                                         //BrPviASImportParser.CDSImportedVariable pviObject)
        {
            for (uint i = arrayStartIndexes[currentDim]; i < arrayIndexLimits[currentDim]; i++)
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

        protected void AddStructureArraySingleElement(string namePrefix,
                                               string varName,
                                               string szAddress,
                                               string taskName,
                                               string completeTypeName,
                                               List<CoDeSysSFImportParser.CDSImportedVariable> structureElements, //string[] structureElements,
                                               DriverCodeBase.Enumerators.LinkType jobType,
                                               int parentId,
                                               ImportData inRootItem)
        {

            uint variableLength = structureElements[0].VarSize;

            // Build the structure variable to be added to the tree
            ImportData v = _ImportDataModel.addImportData();
            ((ImportDataCoDeSys)v).PreName = namePrefix;
            v.Name = varName;
            ((ImportDataCoDeSys)v).Size = variableLength;
            v.szType = completeTypeName;
            //v.szTypeView = GetMoviconTypeFromPlcType(v.szType);
            ((ImportDataCoDeSys)v).Task = taskName;
            ((ImportDataCoDeSys)v).ElemType = completeTypeName;
            v.Address = szAddress;
            v.Description = String.Empty;
            v.parentId = parentId;
            v.Id = _lGlobalID++;
            v.ArrayDimension = 0;
            ((ImportDataCoDeSys)v).ImportDataType = ImportTypes.StructOrEnum;
            ((ImportDataCoDeSys)v).JobType = jobType;

            // Add the structure elements to the tree
            if (structureElements.Count() < 1) // davide 2)
            {
                return;
            }

            List<CoDeSysSFImportParser.CDSImportedVariable> elementDefinitions = GetStructElements(structureElements[0].VarTypeOfStruct.ToString());
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

        protected void AddStandardVar(string szNamePrefix,
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
            ((ImportDataCoDeSys)v).PreName = szNamePrefix;
            v.Name = szVarName;
            ((ImportDataCoDeSys)v).Size = nVarSize;
            v.szType = szVarType.ToString();
            //v.szTypeView = GetMoviconTypeFromPlcType(v.szType);
            ((ImportDataCoDeSys)v).Task = taskName;
            ((ImportDataCoDeSys)v).ElemType = szVarType.ToString();
            ((ImportDataCoDeSys)v).Type = (DataType)moviconType;            
            v.Address = szAddress;
            v.Description = description;
            v.parentId = parentId;
            v.Id = _lGlobalID++;
            v.ArrayDimension = 0;
            ((ImportDataCoDeSys)v).ImportDataType = ImportTypes.Standard;
            ((ImportDataCoDeSys)v).JobType = jobType;
            AddTreeItem(v, inRootItem);
        }

        public List<CDSImportedVariable> GetStructElements(string structName)
        {
            if (MapSTRUCT.ContainsKey(structName))
                return MapSTRUCT[structName];
            else
                return new List<CDSImportedVariable>();
        }

        protected ImportUDTType GetArrayType(ImportPlcSymbolDesc startElement)
        {
            string VarType;
            string VarTypeOfStruct;
            ImportUDTType Category = ImportUDTType.Unknown;
            string VarDeclaration;

            VarDeclaration = startElement.pszType;
            GetUdtTypeTPY(startElement.pszName, ref VarDeclaration, out Category, out VarType, out VarTypeOfStruct);

            return Category;
        }

        protected bool IsElementAnUndefinedStruct(ImportPlcSymbolDesc element, out ImportUDTType category)
        {                                   
            string VarType;
            string VarTypeOfStruct;
            bool UndefinedStruct = false;

            category = ImportUDTType.Unknown;

            string VarDeclaration = element.pszType;
            GetUdtTypeTPY(element.pszName, ref VarDeclaration, out category, out VarType, out VarTypeOfStruct);

            if (category == ImportUDTType.Struct || category == ImportUDTType.ArrayOfStruct)
            {
                UndefinedStruct = (VarType == CoDeSysProtocol.GENERIC_STRUCT || VarTypeOfStruct == CoDeSysProtocol.GENERIC_STRUCT);
            }

            return UndefinedStruct;
        }

        
        /// <summary>
        /// Remove T_ prefix from datatype (add by default in all data type of symbolic file)
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        protected string RemoveTUnderscore(string text)
        {
            if (text.Substring(0, 2) == "T_")
                return text.Substring(2, text.Length - 2);
            else
                return text;
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

        private string GetStructNameFromVarName(string varType, string varName)
        {
            if (varType.ToLower() =="data" || varType.ToLower() == CoDeSysProtocol.GENERIC_STRUCT)
                return ParseStructName(string.Format("{0}_STRUCT",varName));
            else
                return varType;
        }

        protected void GetUdtTypeTPY(string variableName, ref string varDeclaration, out ImportUDTType udtType, out string varType, out string varTypeOfStruct)
        {
            string FirstVarType = null;
            
            udtType = ImportUDTType.Unknown;
            varType = string.Empty;
            varTypeOfStruct = string.Empty;

            //split var declaration by space
            var Matches = varDeclaration.Split(' ');
            // simple or structure data type
            if (Matches.Length == 1)
            {
                FirstVarType = Matches[0];
                varType = FirstVarType; // GetStructNameFromVarName(FirstVarType,variableName); //FirstVarType;
                // all structure are defined with type data (no name of structure are defined)
                if (FirstVarType.ToLower() == "data")
                {
                    FirstVarType = CoDeSysProtocol.GENERIC_STRUCT;
                    varType = CoDeSysProtocol.GENERIC_STRUCT;
                }
            }
            else // array of 
            {
                FirstVarType = Matches[0];
                if (FirstVarType.ToLower().IndexOf("array", 0) >= 0)
                    FirstVarType = FirstVarType.Trim().Substring(0,"array".Length);
                varType = Matches[Matches.Length - 1].ToString(); //GetStructNameFromVarName(Matches[Matches.Length - 1].ToString(), variableName); 
                //// all structure are defined with type data (no name of structure are defined)
                if (varType.ToLower() == "data")
                    varType = CoDeSysProtocol.GENERIC_STRUCT;
            }

            switch (FirstVarType.ToLower())
            {
                case CoDeSysProtocol.GENERIC_STRUCT:
                    udtType = ImportUDTType.Struct;
                    break;
                case "array":
                    udtType = ImportUDTType.Array;
                    //array of structur is managed later as Struct
                    if (!CoDeSysProtocol.IsStandardDataType(varType))
                    {
                        udtType = ImportUDTType.ArrayOfStruct;
                        varTypeOfStruct = varType;
                        //varType = CoDeSysProtocol.GENERIC_STRUCT;
                    }
                    break;
                case "":    //enum are converted as Int32
                    udtType = ImportUDTType.Enum;
                    varType = "enum";
                    break;
                default:
                    //array of structur is managed later as Struct
                    if (CoDeSysProtocol.IsStandardDataType(varType))
                    {
                        udtType = ImportUDTType.Alias;
                    }
                    else
                    {
                        if (IsEnumDataType(varType))
                        {
                            udtType = ImportUDTType.Enum;
                        }
                        else
                        {
                            //varType = CoDeSysProtocol.GENERIC_STRUCT;
                            udtType = ImportUDTType.Struct;
                            varTypeOfStruct = varType;
                            //varType = CoDeSysProtocol.GENERIC_STRUCT;
                        }
                    }
                    break;
            }
            //varType = ConvertVarTypeFileFormatToDirectImportTypeFormat(varType);
        }

        private bool IsEnumDataType(string varType)
        {
            return (MapUDT.ContainsKey(varType));
        }
               

        private void TraverseStructAndGetArrayOfStruct(string sourcePath, string structName, ref List<CDSImportedVariable> estractedArrayOfStructIsStruct)
        {
            if (!MapSTRUCT.ContainsKey(structName))
                return;

            // get members of struct
            List<CDSImportedVariable> MembersOfStruct = MapSTRUCT[structName];
            foreach(CDSImportedVariable Member in MembersOfStruct)
            {
                if (Member.Category == ImportUDTType.Struct || Member.Category== ImportUDTType.ArrayOfStruct)
                {
                    string ChildPath = string.Format("{0}.{1}", sourcePath, Member.VariableName);
                    if (Member.Category == ImportUDTType.ArrayOfStruct)
                    {
                        CDSImportedVariable ArrayOfStruct = Member.CastedClone();
                        ArrayOfStruct.VariableName = ChildPath;                        
                        estractedArrayOfStructIsStruct.Add(ArrayOfStruct);
                    }
                    TraverseStructAndGetArrayOfStruct(ChildPath, Member.VariableName, ref estractedArrayOfStructIsStruct);
                }
            }
        }


        protected CDSImportedVariable ParseVariablesDefinition(ImportPlcSymbolDesc row)
        {
            List<CDSImportedVariable> NestedArrayOfStruct = null;
            return ParseVariablesDefinition(row, ref NestedArrayOfStruct);
        }

        protected CDSImportedVariable ParseVariablesDefinition(ImportPlcSymbolDesc row, ref List<CDSImportedVariable> estractedArrayOfStructIsStruct)
        {
            CDSImportedVariable NewElement = null;

            string VariableName = row.pszName;
            string VarDeclaration = row.pszType;
            uint VarSize = 0;
            string VarType;
            string VarTypeOfStruct;
            ImportUDTType Category = ImportUDTType.Unknown;

            //get data type
            GetUdtTypeTPY(VariableName, ref VarDeclaration, out Category, out VarType, out VarTypeOfStruct);

            NewElement = new CDSImportedVariable();
            NewElement.VariableName = VariableName;
            //NewElement.TaskName = TaskName;
            //NewElement.Description = Comment;
            NewElement.VarType = VarType;
            NewElement.Category = Category;

            switch (Category)
            {
                case ImportUDTType.IgnoreVar:
                    NewElement = null;
                    break;

                case ImportUDTType.Alias:
                    if (CoDeSysProtocol.GetVarTypeAndSize(ref VarDeclaration, out VarSize) >= 0)
                    {
                        //NewElement.VarDeclaration = VarDeclaration;
                        NewElement.VarSize = VarSize;
                        NewElement.ElementsNumber = 1;
                    }
                    else
                    {
                        NewElement = null;
                    }
                    break;

                case ImportUDTType.Enum:    // inside PLC this type is managed as i32
                    //NewElement.VarDeclaration = VarDeclaration;
                    NewElement.VarSize = 0;
                    NewElement.ElementsNumber = 1;
                    // for area of struct store datatype
                    break;

                // Structure or data type not supported (DATE, DATE_AND_TIME --> not supported and so discard it)
                case ImportUDTType.Struct:
                    //NewElement.VarDeclaration = VarDeclaration;
                    NewElement.VarSize = VarSize;
                    NewElement.ElementsNumber = 1;
                    // for area of struct store datatype
                    NewElement.VarTypeOfStruct = VarType;

                    //don't add variable without mapped structure data type
                    if (!MapSTRUCT.ContainsKey(NewElement.VarTypeOfStruct)) 
                        NewElement = null;
                    else
                        if (estractedArrayOfStructIsStruct != null)
                            TraverseStructAndGetArrayOfStruct(VariableName, NewElement.VarTypeOfStruct, ref estractedArrayOfStructIsStruct);

                    break;

                case ImportUDTType.Array:
                    {
                        uint ArrayDimCount;
                        uint ElementsNumber;
                        uint[] ArrayStartIndexes;
                        uint[] ArrayIndexLimits;

                        // Check the variable type and get the variable size
                        GetVarTypeAndSize(ref VarDeclaration, ref VarSize, out ArrayDimCount, out ElementsNumber, out ArrayStartIndexes, out ArrayIndexLimits);

                        //NewElement.VarDeclaration = VarDeclaration;                                    
                        NewElement.VarSize = VarSize;
                        NewElement.ElementsNumber = ElementsNumber;
                        NewElement.ArrayDimCount = ArrayDimCount;
                        NewElement.ArrayStartIndexes = ArrayStartIndexes;
                        NewElement.ArrayIndexLimits = ArrayIndexLimits;
                    }
                    break;

                case ImportUDTType.ArrayOfStruct:
                    {
                        uint ArrayDimCount;
                        uint ElementsNumber;
                        uint[] ArrayStartIndexes;
                        uint[] ArrayIndexLimits;

                        // Check the variable type and get the variable size
                        GetVarTypeAndSize(ref VarDeclaration, ref VarSize, out ArrayDimCount, out ElementsNumber, out ArrayStartIndexes, out ArrayIndexLimits);

                        //NewElement.VarDeclaration = VarDeclaration;                                    
                        NewElement.VarSize = VarSize;
                        NewElement.ElementsNumber = ElementsNumber;
                        NewElement.ArrayDimCount = ArrayDimCount;
                        NewElement.ArrayStartIndexes = ArrayStartIndexes;
                        NewElement.ArrayIndexLimits = ArrayIndexLimits;
                        NewElement.VarTypeOfStruct = VarTypeOfStruct;

                        //don't add variable without mapped structure data type
                        if (!MapSTRUCT.ContainsKey(NewElement.VarTypeOfStruct))
                            NewElement = null;
                        else if (estractedArrayOfStructIsStruct != null)
                        {
                            if (ArrayDimCount == 1)
                            {
                                for (uint i = ArrayStartIndexes[0]; i < ArrayIndexLimits[0]; i++)
                                {
                                    // Complete the name with the array index
                                    string varCompleteName = VariableName + String.Format("[{0}]", i);

                                    TraverseStructAndGetArrayOfStruct(varCompleteName, NewElement.VarTypeOfStruct, ref estractedArrayOfStructIsStruct);
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

                                        TraverseStructAndGetArrayOfStruct(varCompleteName, NewElement.VarTypeOfStruct, ref estractedArrayOfStructIsStruct);
                                    }
                                }
                            }
                            else if (ArrayDimCount == 3)
                            {
                                for (uint i = ArrayStartIndexes[0]; i < ArrayIndexLimits[0]; i++)
                                {
                                    for (uint j = ArrayStartIndexes[1]; j < ArrayIndexLimits[1]; j++)
                                    {
                                        for (uint k = ArrayStartIndexes[2]; k < ArrayIndexLimits[2]; k++)
                                        {
                                            // Complete the name with the array index
                                            string varCompleteName = VariableName + String.Format("[{0},{1},{2}]", i, j, k);

                                            TraverseStructAndGetArrayOfStruct(varCompleteName, NewElement.VarTypeOfStruct, ref estractedArrayOfStructIsStruct);
                                        }
                                    }
                                }
                            }
                        }
                    }
                    break;
            }

            return NewElement;
        }

        protected int GetVarTypeAndSize(ref string FieldType, ref UInt32 StandardSize, out uint arrayDimCount, out uint elementsNumber, out UInt32[] arrayStartIndexes, out UInt32[] arrayIndexLimits)
        {
            int returnValue = (int)ImportTypes.Unknown;
            int MovType = (int)DataType.Boolean;
            StandardSize = 0;
            arrayDimCount = 0;
            elementsNumber = 0;
            arrayStartIndexes = null;
            arrayIndexLimits = null;
            FieldType = FieldType.Trim();
            if (FieldType == String.Empty)
            {
                return returnValue;
            }

            // Array?
            int searchIndex1 = FieldType.ToLower().IndexOf("array");
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
                    if (MapConstants.ContainsKey(szAux2) == false)
                    {
                        return returnValue;
                    }
                    nStart0 = MapConstants[szAux2];
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
                        if (MapConstants.ContainsKey(szAux2) == false)
                        {
                            return returnValue;
                        }
                        nEnd0 = MapConstants[szAux2];
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

                    // dont' check here if struct is mapped
                    //if (CoDeSysProtocol.GetVarTypeAndSize(ref szAux2, out StandardSize) < 0)
                    //{
                    //    if ((MapUDT.ContainsKey(szAux2) == false) || ((MapUDT[szAux2].Length) <= 0))
                    //    {
                    //        if ((MapSTRUCT.ContainsKey(szAux2) == false) || ((MapSTRUCT[szAux2].Count) <= 0))
                    //        {
                    //            //if ((_mapFunctionBlock.ContainsKey(szAux2) == false) || ((_mapFunctionBlock[szAux2].Length) <= 0))
                    //            //{
                    //            return returnValue;
                    //            //}
                    //        }
                    //    }
                    //}

                    arrayDimCount = 1;
                    Array.Resize(ref arrayStartIndexes, 1);
                    Array.Resize(ref arrayIndexLimits, 1);
                    arrayStartIndexes[0] = (UInt32)nStart0;
                    //arrayIndexLimits[0] = (UInt32)(nEnd0 - nStart0 + 1);
                    arrayIndexLimits[0] = (UInt32)(nEnd0 + 1);
                    //elementsNumber = arrayIndexLimits[0];
                    elementsNumber = (uint)(nEnd0 - nStart0 + 1);
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
                        if (MapConstants.ContainsKey(szAux2) == false)
                        {
                            return returnValue;
                        }
                        nEnd0 = MapConstants[szAux2];
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
                        if (MapConstants.ContainsKey(szAux2) == false)
                        {
                            return returnValue;
                        }
                        nStart1 = MapConstants[szAux2];
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
                            if (MapConstants.ContainsKey(szAux2) == false)
                            {
                                return returnValue;
                            }
                            nEnd1 = MapConstants[szAux2];
                        }
                        if (nEnd1 <= nStart1)
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

                        // dont' check here if struct is mapped
                        //if (CoDeSysProtocol.GetVarTypeAndSize(ref szAux2, out StandardSize) < 0)
                        //{
                        //    if ((MapUDT.ContainsKey(szAux2) == false) || ((MapUDT[szAux2].Length) <= 0))
                        //    {
                        //        if ((MapSTRUCT.ContainsKey(szAux2) == false) || ((MapSTRUCT[szAux2].Count) <= 0))
                        //        {
                        //            //if ((_mapFunctionBlock.ContainsKey(szAux2) == false) || ((_mapFunctionBlock[szAux2].Length) <= 0))
                        //            //{
                        //            return returnValue;
                        //            //}
                        //        }
                        //    }
                        //}

                        arrayDimCount = 2;
                        Array.Resize(ref arrayStartIndexes, 2);
                        Array.Resize(ref arrayIndexLimits, 2);
                        arrayStartIndexes[0] = (UInt32)nStart0;
                        //arrayIndexLimits[0] = (UInt32)(nEnd0 - nStart0 + 1);
                        arrayIndexLimits[0] = (UInt32)(nEnd0 + 1);
                        arrayStartIndexes[1] = (UInt32)nStart1;
                        //arrayIndexLimits[1] = (UInt32)(nEnd1 - nStart1 + 1);
                        arrayIndexLimits[1] = (UInt32)(nEnd1 + 1);
                        elementsNumber = (uint)((nEnd0 - nStart0 + 1) * (nEnd1 - nStart1 + 1));
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
                            if (MapConstants.ContainsKey(szAux2) == false)
                            {
                                return returnValue;
                            }
                            nEnd1 = MapConstants[szAux2];
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
                            if (MapConstants.ContainsKey(szAux2) == false)
                            {
                                return returnValue;
                            }
                            nStart2 = MapConstants[szAux2];
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
                            if (MapConstants.ContainsKey(szAux2) == false)
                            {
                                return returnValue;
                            }
                            nEnd2 = MapConstants[szAux2];
                        }
                        if (nEnd2 <= nStart2)
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
                        if (CoDeSysProtocol.GetVarTypeAndSize(ref szAux2, out StandardSize) < 0)
                        {
                            if ((MapUDT.ContainsKey(szAux2) == false) || ((MapUDT[szAux2].Length) <= 0))
                            {
                                if ((MapSTRUCT.ContainsKey(szAux2) == false) || ((MapSTRUCT[szAux2].Count) <= 0))
                                {
                                    //if ((_mapFunctionBlock.ContainsKey(szAux2) == false) || ((_mapFunctionBlock[szAux2].Length) <= 0))
                                    //{
                                    return returnValue;
                                    //}
                                }
                            }
                        }

                        arrayDimCount = 3;
                        Array.Resize(ref arrayStartIndexes, 3);
                        Array.Resize(ref arrayIndexLimits, 3);
                        arrayStartIndexes[0] = (UInt32)nStart0;
                        //arrayIndexLimits[0] = (UInt32)(nEnd0 - nStart0 + 1);
                        arrayIndexLimits[0] = (UInt32)(nEnd0 + 1);
                        arrayStartIndexes[1] = (UInt32)nStart1;
                        //arrayIndexLimits[1] = (UInt32)(nEnd1 - nStart1 + 1);
                        arrayIndexLimits[1] = (UInt32)(nEnd1 + 1);
                        arrayStartIndexes[2] = (UInt32)nStart2;
                        //arrayIndexLimits[2] = (UInt32)(nEnd2 - nStart2 + 1);
                        arrayIndexLimits[2] = (UInt32)(nEnd2 + 1);
                        elementsNumber = (uint)((nEnd0 - nStart0 + 1) * (nEnd1 - nStart1 + 1) * (nEnd2 - nStart2 + 1));
                        FieldType = String.Empty;
                        FieldType = szAux2;
                        returnValue = (int)ImportTypes.Array;
                    }
                }

                return returnValue;
            }

            if (MapUDT.ContainsKey(FieldType) == true)
            {
                string[] udtDefinition = MapUDT[FieldType];
                if (udtDefinition.Length > 0)
                {
                    if (UdtIsEnum(udtDefinition) == false)
                    {
                        returnValue = (int)ImportTypes.StructOrEnum;
                    }
                    else
                    {
                        string stdType = "INT";
                        MovType = CoDeSysProtocol.GetVarTypeAndSize(ref stdType, out StandardSize);
                        if (MovType >= 0)
                        {
                            returnValue = (int)ImportTypes.Standard;
                        }
                    }
                }

                return returnValue;
            }

            if (MapSTRUCT.ContainsKey(FieldType) == true)
            {
                if ((MapSTRUCT[FieldType].Count) > 0)
                {
                    returnValue = (int)ImportTypes.StructOrEnum;
                }
                return returnValue;
            }

            //if (_mapFunctionBlock.ContainsKey(FieldType) == true)
            //{
            //    if ((_mapFunctionBlock[FieldType].Length) <= 0)
            //    {
            //        returnValue = (int)ImportTypes.StructOrEnum;
            //    }
            //    return returnValue;
            //}

            // Standard type?
            MovType = CoDeSysProtocol.GetVarTypeAndSize(ref FieldType, out StandardSize);
            if (MovType >= 0)
            {
                returnValue = (int)ImportTypes.Standard;
            }

            return returnValue;
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
            if (String.Compare(fieldName, "___Movicon_Import_Enum_Alias", true) != 0)
            {
                return (false);
            }

            return true;
        }

    }
    #endregion
}
