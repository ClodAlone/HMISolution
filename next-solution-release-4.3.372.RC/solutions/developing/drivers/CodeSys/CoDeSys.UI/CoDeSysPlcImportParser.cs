using System;
using System.Collections.Generic;
using DriverCodeBase.UI;

namespace CoDeSys.UI
{
    public partial class CoDeSysPlcImportParser : CoDeSysImportBase, IDisposable
    {
        #region Properties
        #endregion

        #region Constructors
        public CoDeSysPlcImportParser() : base()
        {
        }
        #endregion

        #region Methods

        //public ImportDataModel Import(GetStationName readStationName, string station, string deviceHostName, string pLCAddress)
        public ImportDataModel Import(GetStationName readStationName, string station, string deviceHostName, string pLCAddress, ulong nTypeConnection, ulong port, string username, string passwordPLC, string GatewayPassword)
        {
            _ImportDataModel = null;
            CoDeSysProtocol.PLCHandlerErrors Ret;

            base.LastError = string.Empty;
                                
            //test codesys library and codesys wrapper presence; it one of these is not present, stop driver
            CoDeSysPLCHandlerWrapper PLCHandler = new CoDeSysPLCHandlerWrapper();
            if (!PLCHandler.IsWrapperInstalled())
            {
                base.LastError = Properties.Resources.ErrorCoDeSysWrapperNotInstalled;
                return _ImportDataModel;
            }

            if (!PLCHandler.IsCoDeSysInstalled())
            {
                base.LastError = Properties.Resources.ErrorCoDeSysNotInstalled;
                return _ImportDataModel;
            }

            // Init CoDeSys wrapper library
            PLCHandler.Init();

            string Error = string.Empty;
            // Get handle of station for CoDeSys Wrapper
            Ret = PLCHandler.Create();
            if (Ret != CoDeSysProtocol.PLCHandlerErrors.RESULT_OK)
            {
                base.LastError = string.Format(Properties.Resources.ErrorCoDeSysWrapperCannotCreateStation, Ret);
                return _ImportDataModel;
            }

            //Ret = PLCHandler.Connect(deviceHostName, pLCAddress);
            Ret = PLCHandler.AllConnections(deviceHostName, nTypeConnection, pLCAddress, port, username, passwordPLC, GatewayPassword, null);
            if (Ret == CoDeSysProtocol.PLCHandlerErrors.RESULT_OK)
            {
                if (PLCHandler.GetState() == CoDeSysProtocol.PLCHandlerState.STATE_RUNNING)
                {
                    List<CoDeSysPLCHandlerWrapper.PlcSymbolDesc> VarList;
                    Ret = GetPlcData(PLCHandler, out VarList);
                    if (Ret == CoDeSysProtocol.PLCHandlerErrors.RESULT_OK)
                    {
                        PreParsePlcData(VarList);

                        _ImportDataModel = ParsePlcData(readStationName);
                    }
                    else
                    {
                        base.LastError = string.Format(Properties.Resources.ErrorDuringRetrivingVarListFromPlc, Ret);
                    }
                }
                else
                {
                    base.LastError = string.Format(Properties.Resources.ErrorConnectionToPlc, Properties.Resources.ErrorPlcIsNotInRunning);
                }
            }
            else
            {
                base.LastError = string.Format(Properties.Resources.ErrorConnectionToPlc, Ret);
            }
            

            PLCHandler.Disconnect();

            PLCHandler.Release();

            PLCHandler.Dispose();

            return _ImportDataModel;
        }

        /// <summary>
        /// Remove from list of vars unecessary array elements
        ///     Simple array : remove all array elements
        ///     Array of struct : from 1st element get struct, then remove all array elements
        /// </summary>
        /// <param name="rows"></param>
        private void RemoveUnecessaryArrayElements(ref List<ImportPlcSymbolDesc> rows)
        {
            int ArrayDeclarationIndex = 0;
            int RowIndex = 0;
            ImportPlcSymbolDesc ArrayDeclaration;

            while (RowIndex<rows.Count)
            {
                ImportUDTType Category = GetArrayType(rows[RowIndex]);
                switch (Category)
                {
                    case ImportUDTType.Array:   // in a simpe array remove all array elements --> leave only array declararion
                        ArrayDeclarationIndex = RowIndex;
                        ArrayDeclaration = rows[RowIndex];

                        RowIndex++;
                        while (RowIndex < rows.Count)
                        {
                            if (rows[RowIndex].pszName.IndexOf(ArrayDeclaration.pszName + "[") == 0) {
                                rows.RemoveAt(RowIndex);
                                RowIndex--;
                            }
                            //else
                            //{
                            //    RowIndex--;
                            //    break;
                            //}
                            RowIndex++;
                        }
                        RowIndex = ArrayDeclarationIndex;
                        break;

                    case ImportUDTType.ArrayOfStruct:
                        ArrayDeclarationIndex = RowIndex;
                        //  in a array of struct keep declaration and first array element only --> used later to built struct                        
                        ArrayDeclaration = rows[RowIndex];

                        RowIndex++;
                        if ((RowIndex)< rows.Count)
                        {
                            List<ImportPlcSymbolDesc> MembersOfStruct = new List<ImportPlcSymbolDesc>();
                            ImportPlcSymbolDesc FirstElementOfArray = rows[RowIndex];
                            while (RowIndex < rows.Count)
                            {
                                if (rows[RowIndex].pszName.IndexOf(ArrayDeclaration.pszName + "[") == 0)
                                {
                                    // remove all array element except 1st --> used later to built struct
                                    if (!rows[RowIndex].pszName.Contains(FirstElementOfArray.pszName))
                                    {                                        
                                        rows.RemoveAt(RowIndex);
                                        RowIndex--;
                                    }
                                }
                                //else
                                //{
                                //    RowIndex--;
                                //    break;
                                //}
                                RowIndex++;
                            }
                        }
                        RowIndex = ArrayDeclarationIndex;
                        break;
                }

                RowIndex++;
            }
        }
                
        /// <summary>
        /// Replace default Data type used by codesys to export struct ("DATA") with a new one calculated by the program
        /// </summary>
        /// <param name="varDeclaration"></param>
        /// <param name="oldVarType"></param>
        /// <param name="newVarType"></param>
        /// <returns></returns>
        private string ReplaceStandardStructDataType(string varDeclaration, string newVarType)
        {
            string Result = varDeclaration;

            // find data type declaration
            int Index = varDeclaration.LastIndexOf("DATA");
            if (Index>=0)
            {
                Result = varDeclaration.Substring(0, Index) + newVarType;
            }

            return Result;
        }


        //private bool IsMemberPartOfArrayOfStructDeclaration(int currentIndex, List<ImportPlcSymbolDesc> rows) {
        //    bool Result = false;
        //    ImportPlcSymbolDesc row = rows[currentIndex];

        //    if ((currentIndex - 1) >= 0) {
        //        ImportUDTType Category;
        //        if (IsElementAnUndefinedStruct(rows[currentIndex - 1], out Category))
        //            Result = (Category == ImportUDTType.ArrayOfStruct);
        //    }

        //    return Result;
        //}


        private int StructMemberNestedLevel(string szName)
        {
            return szName.Split('.').Length - 1;
        }

        private string StructMemberLastNameLevel(string szName)
        {
            var Splitted = szName.Split('.');
            return Splitted[Splitted.Length - 1];
        }

        /// <summary>
        /// With a list of variable (member of struct) and create a struct (nested struct are supported)
        /// </summary>
        /// <param name="typeName"></param>
        /// <param name="rows"></param>
        /// <param name="rowIndex"></param>
        private void ParsePlcDataTypeStructTPY(string typeName, ref List<ImportPlcSymbolDesc> rows, ref int rowIndex, ref int level)
        {
            List<CDSImportedVariable> ListOfElements = new List<CDSImportedVariable>();

            while (rowIndex < rows.Count)
            {
                ImportPlcSymbolDesc row = rows[rowIndex];
                //bool IgnoreVar = false;

                // members of "current" struct is terminated --> return to declaration of previous struct
                if (StructMemberNestedLevel(rows[rowIndex].pszName) < level)
                {
                    level = StructMemberNestedLevel(rows[rowIndex].pszName);
                    break;
                }

                ImportUDTType Category;
                // if element is a struct
                if (IsElementAnUndefinedStruct(row, out Category))
                {
                    // struct element after array of struct in only a struct definition --> not required to create a struct 
                    //IgnoreVar = IsMemberPartOfArrayOfStructDeclaration(rowIndex, rows);
                    //if (!IgnoreVar)
                    //{
                        string NewTypeName = ParseStructName(row.pszName + "_STRUCT");
                        row.pszType = ReplaceStandardStructDataType(row.pszType, NewTypeName);
                        level = StructMemberNestedLevel(rows[rowIndex].pszName) + 1;
                                            
                        switch(Category)
                        {
                            case ImportUDTType.Struct:
                                rowIndex += 1;
                                break;
                            case ImportUDTType.ArrayOfStruct:
                                // next element is a struct declaration --> go directly to 1st struct's member
                                rowIndex += 2;
                                break;
                        }

                        ParsePlcDataTypeStructTPY(NewTypeName, ref rows, ref rowIndex, ref level);

                        rowIndex--;
                    //}
                }

                //if (!IgnoreVar) {
                    row.pszName = StructMemberLastNameLevel(row.pszName);
                    CDSImportedVariable NewVar = ParseVariablesDefinition(row);
                    if (NewVar != null)
                        ListOfElements.Add(NewVar);
                //}
                rowIndex++;
            }

            if (ListOfElements != null && ListOfElements.Count > 0)
            {
                if (!MapSTRUCT.ContainsKey(typeName))
                    MapSTRUCT.Add(typeName, ListOfElements);
            }
        }

        /// <summary>
        /// Remove memmbers of struct from list of vars and create prototype/struct
        /// </summary>
        /// <param name="rows"></param>
        private void RemoveAndCreateStruct(ref List<ImportPlcSymbolDesc> rows)
        {
            int RowIndex = 0;            

            RowIndex = 0;
            while (RowIndex < rows.Count)
            {
                ImportUDTType Category;
                if (IsElementAnUndefinedStruct(rows[RowIndex], out Category)) {                     
                    List<ImportPlcSymbolDesc> MembersOfStruct = new List<ImportPlcSymbolDesc>();

                    ImportPlcSymbolDesc StructDeclaration = rows[RowIndex];

                    MembersOfStruct.Add(StructDeclaration);

                    RowIndex++;
                    while (RowIndex < rows.Count)
                    {
                        if (rows[RowIndex].pszName == StructDeclaration.pszName || rows[RowIndex].pszName.Contains(StructDeclaration.pszName + ".") || rows[RowIndex].pszName.Contains(StructDeclaration.pszName + "["))
                        {
                            ImportPlcSymbolDesc Member = (ImportPlcSymbolDesc)rows[RowIndex].CastedClone();
                            if (Member.pszName.Contains("["))
                            {
                                string ArrayIndex = Member.pszName.Substring(Member.pszName.IndexOf("["), Member.pszName.LastIndexOf("]") - Member.pszName.IndexOf("[") + 1);
                                Member.pszName = Member.pszName.Replace(ArrayIndex, string.Empty);
                            }

                            MembersOfStruct.Add(Member);

                            rows.RemoveAt(RowIndex);
                            RowIndex--;
                        }
                        else
                        {
                            RowIndex--;
                            break;
                        }
                        RowIndex++;
                    }

                    if (MembersOfStruct.Count > 0)
                    {
                        int Index = 0;
                        int Level = StructMemberNestedLevel(MembersOfStruct[Index].pszName);
                        string NewStructName = ParseStructName(StructDeclaration.pszName + "_STRUCT");

                        // ignore next element --> struct declaration
                        Index++;
                        
                        ParsePlcDataTypeStructTPY(NewStructName, ref MembersOfStruct, ref Index, ref Level);
                        // change default data type of struct (DATA) with new one calculated by program                        
                        StructDeclaration.pszType = ReplaceStandardStructDataType(StructDeclaration.pszType, NewStructName);
                    }
                }
                
                RowIndex++;
            }
        }

        private void PreParsePlcData(List<CoDeSysPLCHandlerWrapper.PlcSymbolDesc> importedVars)
        {
            List<ImportPlcSymbolDesc> rows = new List<ImportPlcSymbolDesc>();

            //foreach (var var in importedVars)
            //    System.Diagnostics.Debug.WriteLine(string.Format("{0}-{1}", var.pszName, var.pszType));

            foreach (var var in importedVars)
                rows.Add(new ImportPlcSymbolDesc(var));

            //foreach (ImportPlcSymbolDesc var in rows)
            //    System.Diagnostics.Debug.WriteLine(string.Format("{0}-{1}", var.pszName, var.pszType));

            // remove array elements inside list of vars
            RemoveUnecessaryArrayElements(ref rows);

            ////System.Diagnostics.Debug.WriteLine("-----------------------------------------------------------------");
            //foreach (ImportPlcSymbolDesc var in rows)
            //    System.Diagnostics.Debug.WriteLine(string.Format("{0}-{1}", var.pszName, var.pszType));

            // remove var used to declare member of struct and create struct
            RemoveAndCreateStruct(ref rows);

            //System.Diagnostics.Debug.WriteLine("-----------------------------------------------------------------");
            //foreach (ImportPlcSymbolDesc var in rows)
            //    System.Diagnostics.Debug.WriteLine(string.Format("{0}-{1}", var.pszName, var.pszType));

            // cycle remain vars to create final list --> remove undefined datatype
            for (int RowIndex = 0; RowIndex < rows.Count; RowIndex++)
            {
                List<CDSImportedVariable> ArrayOfStructInStruct = new List<CDSImportedVariable>();
                CDSImportedVariable NewVar = ParseVariablesDefinition(rows[RowIndex], ref ArrayOfStructInStruct);
                if (NewVar != null)
                {
                    ParsedVars.Add(NewVar);
                    if (ArrayOfStructInStruct.Count>0)
                        ParsedVars.AddRange(ArrayOfStructInStruct);
                }
            }
        }

        private CoDeSysProtocol.PLCHandlerErrors GetPlcData(CoDeSysPLCHandlerWrapper plcHandler,out List<CoDeSysPLCHandlerWrapper.PlcSymbolDesc> varList)
        {                                    
            //retrive list of all vars from device
            CoDeSysProtocol.PLCHandlerErrors nRet = plcHandler.GetVarListFromPLC(out varList);
            if (nRet != CoDeSysProtocol.PLCHandlerErrors.RESULT_OK)                
                varList = null;

            return nRet;
        }
        
        #region IDisposable Members

        public void Dispose()
        {
            
        }
        #endregion
    }
    #endregion
}
