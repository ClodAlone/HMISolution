using System;
using System.Collections.Generic;
using DriverCodeBaseEx.UI;
using System.IO;


namespace GESRTP2.UI
{
    public partial class GESRTP2FileImportParser : GESRTP2FileImportBase
    {                
        #region Methods
        public bool Import(GESRTP2Protocol.PlcTypes plcType, GetStationName readStationName, string file)
        {
            bool result = GetVariableFromFile(plcType, file, out List<FileVariable> varList);
            if (result)
                result = ParseFileVariable(plcType, varList, readStationName);

            return result;            
        }        

        public ImportDataModelGESRTP2 GetImportedVariables()
        {
            return _ImportDataModel;
        }

        private bool ParseFileVariable(GESRTP2Protocol.PlcTypes plcType, List<FileVariable> varList, GetStationName readStationName)
        {
            _ImportDataModel = new ImportDataModelGESRTP2(readStationName);

            List<ImportedVariable> varListParsed = new List<ImportedVariable>();
            List<ImportedVariable> arrayOfStructInStruct = new List<ImportedVariable>();
            
            foreach (FileVariable var in varList)
                AddPublishProperty(plcType, var.Name, var.Publish);

            // remove array elements inside list of vars
            RemoveUnecessaryArrayElements(plcType, ref varList);

            // remove var used to declare member of struct and create struct
            RemoveAndCreateStruct(plcType, ref varList);
                    
            // cycle remain vars to create final list --> remove undefined datatype
            for (int rowIndex = 0; rowIndex < varList.Count; rowIndex++)
            {
                arrayOfStructInStruct = new List<ImportedVariable>();
                ImportedVariable newVar = ParseVariablesDefinition(plcType, varList[rowIndex], ref arrayOfStructInStruct);
                if (newVar != null)
                {
                    varListParsed.Add(newVar);
                    if (arrayOfStructInStruct.Count > 0)
                        varListParsed.AddRange(arrayOfStructInStruct);
                }
            }

            // remove array of struct from struct's members --> not supported from Movicon prototype (remove empty struct)
            RemoveArrayOfStructInSubStruct(varListParsed);

            // add var into ImportData and into TreeView
            for (int rowIndex = 0; rowIndex < varListParsed.Count; rowIndex++)                    
                AddVariable(varListParsed[rowIndex]);                    

            return (String.IsNullOrEmpty(LastError) && _ImportDataModel != null);
        }

        /// <summary>
        /// Remove from list of vars unecessary array elements
        ///     Simple array : remove all array elements
        ///     Array of struct : from 1st element get struct, then remove all array elements
        /// </summary>
        /// <param name="rows"></param>
        private void RemoveUnecessaryArrayElements(GESRTP2Protocol.PlcTypes plcType, ref List<FileVariable> rows)
        {
            int ArrayDeclarationIndex = 0;
            int RowIndex = 0;
            FileVariable ArrayDeclaration;

            while (RowIndex<rows.Count)
            {
                ImportTypes Category = GetArrayType(rows[RowIndex]);
                switch (Category)
                {
                    case ImportTypes.Array:   // in a simpe array remove all array elements --> leave only array declararion
                        ArrayDeclarationIndex = RowIndex;
                        ArrayDeclaration = rows[RowIndex];

                        RowIndex++;
                        while (RowIndex < rows.Count)
                        {
                            if (rows[RowIndex].Name.IndexOf(ArrayDeclaration.Name + "[") == 0)
                            {
                                if (rows[RowIndex].IsDataArea())
                                    ArrayDeclaration.AddDataAreaArrayElementAddress(ArrayDeclaration.Name, rows[RowIndex].Name, rows[RowIndex].ADDR, rows[RowIndex].ADDDataArea);
                                rows.RemoveAt(RowIndex);
                                RowIndex--;
                            }                                
                            RowIndex++;
                        }
                        RowIndex = ArrayDeclarationIndex;
                        break;

                    case ImportTypes.ArrayOfStruct:
                        ArrayDeclarationIndex = RowIndex;
                        //  in a array of struct keep declaration and first array element only --> used later to built struct                        
                        ArrayDeclaration = rows[RowIndex];

                        RowIndex++;
                        if (RowIndex < rows.Count)
                        {
                            List<FileVariable> MembersOfStruct = new List<FileVariable>();
                            FileVariable FirstElementOfArray = rows[RowIndex];
                            while (RowIndex < rows.Count)
                            {
                                if (rows[RowIndex].Name.IndexOf(ArrayDeclaration.Name + "[") == 0)
                                {
                                    //AddStructMemberPublish(rows[RowIndex].Name, rows[RowIndex].Publish);
                                    // remove all array element except 1st --> used later to built struct
                                    if (!rows[RowIndex].Name.Contains(FirstElementOfArray.Name))
                                    {                                            
                                        rows.RemoveAt(RowIndex);
                                        RowIndex--;
                                    }
                                }                                   
                                RowIndex++;
                            }
                        }
                        RowIndex = ArrayDeclarationIndex;
                        break;
                }
                RowIndex++;
            }
        }         

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
        private void ParseVariableDataTypeStruct(GESRTP2Protocol.PlcTypes plcType, string typeName, ref List<FileVariable> rows, ref int rowIndex, ref int level)
        {
            List<ImportedVariable> ListOfElements = new List<ImportedVariable>();

            while (rowIndex < rows.Count)
            {
                if (rows[rowIndex].IsSymbolic())
                {
                    FileVariable row = rows[rowIndex];

                    // members of "current" struct is terminated --> return to declaration of previous struct
                    if (StructMemberNestedLevel(rows[rowIndex].Name) < level)
                    {
                        level = StructMemberNestedLevel(rows[rowIndex].Name);
                        break;
                    }

                    // if element is a struct
                    if (IsStructOrArrayOfStructType(row, out ImportTypes Category))
                    {
                        // struct element after array of struct in only a struct definition --> not required to create a struct 
                        string NewTypeName = row.sType;
                        //row.sType = ReplaceStandardStructDataType(row.sType, NewTypeName);
                        level = StructMemberNestedLevel(rows[rowIndex].Name) + 1;

                        switch (Category)
                        {
                            case ImportTypes.Struct:
                                rowIndex += 1;
                                break;
                            case ImportTypes.ArrayOfStruct:
                                // next element is a struct declaration --> go directly to 1st struct's member
                                rowIndex += 2;
                                break;
                        }

                        ParseVariableDataTypeStruct(plcType, NewTypeName, ref rows, ref rowIndex, ref level);

                        rowIndex--;
                    }

                    row.Name = StructMemberLastNameLevel(row.Name);
                    ImportedVariable NewVar = ParseVariablesDefinition(plcType, row);
                    if (NewVar != null)
                        ListOfElements.Add(NewVar);
                }
                rowIndex++;
            }

            if (ListOfElements != null && ListOfElements.Count > 0)
            {
                if (!_MapSTRUCT.ContainsKey(typeName))
                    _MapSTRUCT.Add(typeName, ListOfElements);
            }
        }

        /// <summary>
        /// Remove memmbers of struct from list of vars and create prototype/struct
        /// </summary>
        /// <param name="rows"></param>
        private void RemoveAndCreateStruct(GESRTP2Protocol.PlcTypes plcType, ref List<FileVariable> rows)
        {
            int RowIndex = 0;
            while (RowIndex < rows.Count)
            {
                if (rows[RowIndex].IsSymbolic())
                {
                    if (IsStructOrArrayOfStructType(rows[RowIndex], out ImportTypes Category))
                    {
                        List<FileVariable> MembersOfStruct = new List<FileVariable>();

                        FileVariable StructDeclaration = rows[RowIndex];

                        MembersOfStruct.Add(StructDeclaration);

                        RowIndex++;
                        while (RowIndex < rows.Count)
                        {
                            if (rows[RowIndex].Name == StructDeclaration.Name || rows[RowIndex].Name.Contains(StructDeclaration.Name + ".") || rows[RowIndex].Name.Contains(StructDeclaration.Name + "["))
                            {
                                //AddStructMemberPublish(rows[RowIndex].Name, rows[RowIndex].Publish);
                                FileVariable Member = (FileVariable)rows[RowIndex].Clone();
                                if (Member.Name.Contains("["))
                                {
                                    string ArrayIndex = Member.Name.Substring(Member.Name.IndexOf("["), Member.Name.LastIndexOf("]") - Member.Name.IndexOf("[") + 1);
                                    Member.Name = Member.Name.Replace(ArrayIndex, string.Empty);
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
                            int Level = StructMemberNestedLevel(MembersOfStruct[Index].Name);
                            
                            // ignore next element --> struct declaration
                            Index++;
                            ParseVariableDataTypeStruct(plcType, StructDeclaration.sType, ref MembersOfStruct, ref Index, ref Level);                            
                        }
                    }
                }
                RowIndex++;
            }
        }

       
        private bool GetVariableFromFile(GESRTP2Protocol.PlcTypes plcType, string file, out List<FileVariable> varList)
        {
            varList = null;

            try
            {
                string[] lines = File.ReadAllLines(file);
                varList = new List<FileVariable>();
                
                foreach (string line in lines)
                {
                    FileVariable row = new FileVariable();
                    if (row.ParseFile(line))
                        varList.Add(row);                    
                }
            }
            catch (Exception ex)
            {                
                LastError = string.Format(Properties.Resources.ImportErrorUnexpectedError, ex.Message);
                varList = null;
            }
                        
            return string.IsNullOrEmpty(LastError);
        }
    }
    #endregion
}
