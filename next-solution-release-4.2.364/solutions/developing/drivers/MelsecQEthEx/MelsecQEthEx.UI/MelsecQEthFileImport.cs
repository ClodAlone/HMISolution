using System;
using System.Collections.Generic;
using System.IO;
using DriverCodeBaseEx.UI;
using UFUAModel;


namespace MelsecQEth.UI
{
    internal class MelsecQEthFileImportParser
    {
        private enum MelsecQEthImportVarType : int
        {
            ImportVarType_Invalid,
            ImportVarType_VAR_GLOBAL,
            ImportVarType_VAR_LOCAL
        }

        private enum MelsecQEthFileType : int
        {
            Type_ASCII = 1,
            Type_UNICODE_16 = 2,
            Error = -1
        }

        private const string LabelAccessfromExternalDeviceEnabled = "1";
        public const int ImportDataTypeStruct = 9999;
        public const int ImportUnknownDataType = -1;
        public const string DATATYPEPROTOTYPE_TIMER = "MQ_Timer";
        public const string DATATYPEPROTOTYPE_LTIMER = "MQ_LTimer";
        public const string DATATYPEPROTOTYPE_COUNTER = "MQ_Counter";
        public const string DATATYPEPROTOTYPE_LCOUNTER = "MQ_LCounter";
        public const string DATATYPEPROTOTYPE_RETENTIVETIMER = "MQ_RetentiveTimer";
        public const string DATATYPEPROTOTYPE_LRETENTIVETIMER = "MQ_LRetentiveTimer";
                

        public class ImportMovType
        {
            public string Name { get; set; }
            public int Type { get; set; }
            public uint VarSize { get; set; }
            public string Prototype;
            public string Note;

            public ImportMovType()
            {
                Name = string.Empty;
                Type = ImportUnknownDataType;
                VarSize = 0;
                Prototype = string.Empty;
                Note = string.Empty;
            }

            public bool IsPrototype()
            {
                return Type == ImportDataTypeStruct;
            }

            public bool IsUnknownType()
            {
                return Type == -1;
            }

            public bool HasNote()
            {
                return !string.IsNullOrEmpty(Note);
            }
        }

        protected ImportDataModelMelsecQ _ImportDataModel;
        protected int _lGlobalID;

        //protected Dictionary<string, List<ImportedVariable>> _MapSTRUCT;

        #region Properties
        private string _LastError;
        public string LastError { set { _LastError = value; } get { return _LastError; } }
        #endregion

        #region Constructors
        public MelsecQEthFileImportParser()
        {
            _ImportDataModel = null;
            _LastError = null;
            _lGlobalID = 0;
            //_MapSTRUCT = new Dictionary<string, List<ImportedVariable>>();
        }
        #endregion

        public bool Import(MelsecQEthStationSettings station, GetStationName readStationName, string file)
        {
            return ImportVariableFromFile(station, readStationName, file);
        }

        public ImportDataModelMelsecQ GetImportedVariables()
        {
            return _ImportDataModel;
        }

        private MelsecQEthImportVarType IsVarDeclaration(string rowField)
        {
            MelsecQEthImportVarType importVarType = MelsecQEthImportVarType.ImportVarType_Invalid;
            string varDeclaration = rowField.ToUpper();
            if (String.Equals(varDeclaration, "VAR_GLOBAL"))
            {
                importVarType = MelsecQEthImportVarType.ImportVarType_VAR_GLOBAL;
            }

            else if (String.Equals(varDeclaration, "VAR_INPUT") ||
                     String.Equals(varDeclaration, "VAR_OUTPUT") ||
                     String.Equals(varDeclaration, "VAR_IN_OUT") ||
                     String.Equals(varDeclaration, "VAR"))
            {
                importVarType = MelsecQEthImportVarType.ImportVarType_VAR_LOCAL;
            }

            return importVarType;
        }

        private bool IsPouName(string rowField)
        {
            bool retValue = false;
            string pouName = rowField.ToUpper();
            if (String.Equals(pouName, "POUNAME") ||
               String.Equals(pouName, "POENAME"))
            {
                retValue = true;
            }
            return retValue;
        }

        private bool IsArrayDeclaration(string rowField, ref int arrayFirstIndex, ref int arrayDim, ref ImportMovType arrayElementMoviconType, ref uint arrayElementSize, ref int arrayNrDim, ref string ElementType, bool bPlcSupportLabel, string filePath)
        {
            arrayNrDim = 0;
            arrayFirstIndex = 1;
            arrayDim = 1;
            arrayElementMoviconType.Type = MelsecQEthFileImportParser.ImportUnknownDataType;
            arrayElementSize = 0;
            ElementType = String.Empty;

            try
            {
                // Array declaration?
                string typeField = rowField.ToUpper();
                if (!typeField.StartsWith("ARRAY ["))
                {
                    arrayFirstIndex = 0;
                    arrayDim = 0;
                    return false;
                }

                // Parse the array dimension and type
                int firstIndex = typeField.IndexOf('[');
                if (firstIndex < 0)
                {
                    return false;
                }
                int closeBracketIndex = -1;
                do
                {
                    int doublePointIndex = typeField.IndexOf("..", firstIndex);
                    if (doublePointIndex < 0)
                    {
                        return false;
                    }
                    string firstArrayIndex = typeField.Substring(
                                          firstIndex + 1,
                                          doublePointIndex - firstIndex - 1);
                    firstArrayIndex.Trim();
                    arrayFirstIndex = Convert.ToInt32(firstArrayIndex);

                    int lastIndex = typeField.IndexOf(",", doublePointIndex);
                    if (lastIndex < 0)
                    {
                        closeBracketIndex = typeField.IndexOf(']');
                        if (closeBracketIndex < 0)
                        {
                            return false;
                        }
                        lastIndex = closeBracketIndex;
                    }

                    string lastArrayIndex = typeField.Substring(
                                        doublePointIndex + 2,
                                        lastIndex - doublePointIndex - 2);
                    lastArrayIndex.Trim();
                    int arrayLastIndex = Convert.ToInt32(lastArrayIndex);
                    if (arrayLastIndex <= arrayFirstIndex)
                    {
                        return false;
                    }
                    arrayNrDim++;
                    // Set the array dimension
                    arrayDim *= (arrayLastIndex - arrayFirstIndex + 1);
                    firstIndex = lastIndex;
                } while (closeBracketIndex < 0);

                int ofIndex = typeField.IndexOf(" OF ");
                if ((ofIndex <= closeBracketIndex) ||
                    (typeField.Length <= (ofIndex + 4)))
                {
                    return false;
                }
                // Parse the array type
                string elemType = typeField.Substring(ofIndex + 4);
                ElementType = elemType.Trim();
                arrayElementMoviconType = GetMoviconTypeId(ElementType, bPlcSupportLabel, filePath);
                if (arrayElementMoviconType.IsUnknownType())
                {
                    ElementType = String.Empty;
                    return false;
                }

                // Set the array element size
                arrayElementSize = arrayElementMoviconType.VarSize;
            }
            catch (Exception e)
            {
            }

            return true;
        }

        private bool ImportVariableFromFile(MelsecQEthStationSettings station, GetStationName readStationName, string file)
        {
            _ImportDataModel = new ImportDataModelMelsecQ(readStationName);
            file = file.ToLower();
            if (file.Contains(".csv"))
            {
                try
                {                    
                    bool bPlcSupportLabel = MelsecQEthProtocol.PlcSupportLabelAddress((MelsecQEthProtocol.PlcTypes)station.PlcType);
                    uint maxFrameSize = MelsecQEthProtocol.GetMaxJobSize((MelsecQEthProtocol.PlcTypes)station.PlcType);
                    string filePath = System.IO.Path.GetDirectoryName(file);
                    int iReurnCheckTypeFileEncoding = CheckTypeFileEncoding(file);
                    if (iReurnCheckTypeFileEncoding == (int)MelsecQEthFileType.Error)
                    {
                        return false;
                    }
                    char splitChar = ';';
                    if (iReurnCheckTypeFileEncoding == (int)MelsecQEthFileType.Type_UNICODE_16)
                    {
                        splitChar = '"';
                    }
                    //Check if the file was created by Gx Works 3
                    bool TheFileIsGxWorks3 = false;
                    int PositionMemoryArea = -1;
                    using (System.IO.StreamReader readFile1 = new System.IO.StreamReader(file))
                    {
                        string line;
                        string[] row;
                        while ((line = readFile1.ReadLine()) != null)
                        {
                            row = line.Split('\t');
                            if (row.Length > 3)
                            {
                                if (row[4].Contains("Initial Value"))
                                {
                                    if (row[5].Contains("Assign (Device/Label)"))
                                    {
                                        PositionMemoryArea = 5;
                                        TheFileIsGxWorks3 = true;
                                        splitChar = '\t';
                                        break;
                                    }
                                }
                            }
                        }
                    }
                    List<string[]> parsedData = new List<string[]>();
                    using (System.IO.StreamReader readFile = new System.IO.StreamReader(file))
                    {
                        string line;
                        string[] row;
                        if (TheFileIsGxWorks3)
                        {
                            while ((line = readFile.ReadLine()) != null)
                            {
                                line = line.Replace("\"", "");
                                row = line.Split(splitChar);
                                parsedData.Add(row);
                            }
                        }
                        else
                        {
                            while ((line = readFile.ReadLine()) != null)
                            {
                                row = line.Split(splitChar);
                                parsedData.Add(row);
                            }

                        }
                    }

                    if (iReurnCheckTypeFileEncoding == (int)MelsecQEthFileType.Type_UNICODE_16)
                        FileImportedOfTypeUTF_16_LE(parsedData, station.Name, maxFrameSize, bPlcSupportLabel, filePath, PositionMemoryArea, TheFileIsGxWorks3);
                    else
                        FileImportedOfTypeASCII(parsedData, station.Name, maxFrameSize, bPlcSupportLabel, filePath);
                }
                catch (Exception ex)
                {
                    _LastError = MelsecQEth.UI.Properties.Resources.ErrorReadFile + ex.ToString();
                }

                if (_ImportDataModel == null || _ImportDataModel.Children.Count == 0)
                    _LastError = DriverCodeBaseEx.UI.Properties.Resources.ImportErrorEmptyFile;
            }

            return (string.IsNullOrEmpty(LastError));
        }

        //Added version 2.2.30.0 (FOGBUGZ 15497)
        private void FileImportedOfTypeASCII(List<string[]> parsedData, string stationName, uint maxFrameSize, bool bPlcSupportLabel, string filePath)
        {
            try
            {
                MelsecQEthDynTagSettings p = new MelsecQEthDynTagSettings();
                string dynamicaddress = String.Empty;
                ImportMovType MovType = new ImportMovType();
                string pouName = String.Empty;
                string varName = String.Empty;
                int arrayStart = 0;
                int arrayDim = 0;
                int arrayNrDim = 0;
                uint arrayElementSize = 0;
                string arrayElementType = String.Empty;
                int arrayIndex = 0;
                ImportData arrayParent = null;
                string arrayAddress = String.Empty;
                int arrayId = -1;
                bool arrayElement = false;
                MelsecQAddress arrayAddressObj = new MelsecQAddress();
                MelsecQAddress variableAddressObj = new MelsecQAddress();
                string varType = String.Empty;
                foreach (var s in parsedData)
                {
                    if (s.Length == 0)
                    {
                        continue;
                    }

                    // Variable declaration?
                    MelsecQEthImportVarType importVarType = IsVarDeclaration(s[0].Trim());
                    if (importVarType == MelsecQEthImportVarType.ImportVarType_Invalid)
                    {
                        // POU declaration?
                        if ((s.Length > 1) && IsPouName(s[0].Trim()))
                        {
                            pouName = s[1].Trim();
                        }
                        continue;
                    }

                    if (s.Length < 7)
                    {
                        continue;
                    }

                    // Set the variable name
                    varName = s[1].Trim();
                    if (varName == String.Empty)
                    {
                        continue;
                    }
                    string auxString = varName;
                    varName = auxString.Replace('.', '_');

                    // Array?
                    if (arrayDim == 0)
                    {
                        arrayElement = false;
                        if (!IsArrayDeclaration(s[4].Trim(), ref arrayStart, ref arrayDim, ref MovType, ref arrayElementSize, ref arrayNrDim, ref arrayElementType, bPlcSupportLabel, filePath))
                        {
                            if (arrayDim < 0)
                            {
                                arrayDim = 0;
                                continue;
                            }
                        }
                        else
                        {
                            // Get the address of the first element of
                            // the array and check it
                            arrayAddress = s[3].Trim();
                            if (arrayAddress == String.Empty)
                            {
                                arrayDim = 0;
                                continue;
                            }
                            arrayAddressObj.Set(MelsecQEthProtocol.AddressTypes.DataArea, arrayAddress);
                            if (!arrayAddressObj.IsValid)
                            {
                                arrayDim = 0;
                                continue;
                            }

                            arrayIndex = 0;
                            dynamicaddress = String.Empty;
                            p.StationName = stationName;
                            if (!p.ParseAddress(MelsecQEthProtocol.AddressTypes.DataArea, arrayAddressObj.Address))
                            {
                                arrayDim = 0;
                                continue;
                            }
                            dynamicaddress = p.ToString();

                            ImportData impData = _ImportDataModel.addImportData();
                            impData.Name = varName;
                            ((ImportDataMelsecQ)impData).AddressType = variableAddressObj.AddressType;
                            impData.Address = arrayAddressObj.Address;
                            impData.DynAddress = dynamicaddress;
                            ((ImportDataMelsecQ)impData).TagTypeInt = MovType.Type;
                            impData.Description = MovType.Note;
                            impData.szType = s[4].Trim();
                            impData.ArrayDimension = (uint)arrayDim;
                            impData.parentId = -1;
                            impData.Id = _lGlobalID++;
                            arrayId = impData.Id;
                            arrayParent = impData;

                            AddTreeItem(impData);

                            continue;
                        }
                    }

                    // Special case: element of a previously declared array
                    if (arrayDim > 0)
                    {
                        variableAddressObj.Set(MelsecQEthProtocol.AddressTypes.DataArea, arrayAddressObj.Address);
                        int devAddress = arrayAddressObj.StartAddress;
                        if ((MovType.Type != (int)UFUAModel.DataType.UInt32) &&
                            (MovType.Type != (int)UFUAModel.DataType.Int32) &&
                            (MovType.Type != (int)UFUAModel.DataType.Float))
                        {
                            devAddress += arrayIndex;
                        }
                        else
                        {
                            devAddress += 2 * arrayIndex;
                        }
                        if (!variableAddressObj.TryToSet(MelsecQEthProtocol.AddressTypes.DataArea, devAddress))
                        {
                            continue;
                        }
                        arrayElement = true;
                        arrayIndex++;
                        if (arrayIndex >= arrayDim)
                        {
                            arrayDim = 0;
                            arrayIndex++;
                        }
                    }

                    // Variable
                    else
                    {
                        arrayElement = false;
                        variableAddressObj.Set(MelsecQEthProtocol.AddressTypes.DataArea, s[3].Trim());
                        if (!variableAddressObj.IsValid)
                        {
                            continue;
                        }
                    }

                    // Local variable? --> Add the POU name to the variable name
                    if (importVarType == MelsecQEthImportVarType.ImportVarType_VAR_LOCAL)
                    {
                        auxString = pouName + "_" + varName;
                        varName = auxString;
                    }

                    dynamicaddress = String.Empty;
                    p.StationName = stationName;
                    if (!p.ParseAddress(MelsecQEthProtocol.AddressTypes.DataArea, variableAddressObj.Address))
                    {
                        continue;
                    }
                    dynamicaddress = p.ToString();
                    MovType = GetMoviconTypeId(s[4].Trim(), bPlcSupportLabel, filePath);
                    if (!MovType.IsUnknownType())
                    {
                        ImportData impData = _ImportDataModel.addImportData();
                        impData.Name = varName;
                        impData.Address = variableAddressObj.Address;
                        impData.DynAddress = dynamicaddress;
                        ((ImportDataMelsecQ)impData).TagTypeInt = MovType.Type;
                        impData.Description = MovType.Note;
                        impData.szType = s[4].Trim();
                        impData.ArrayDimension = 0;
                        impData.Id = _lGlobalID++;
                        if (!arrayElement)
                        {
                            impData.parentId = -1;
                            AddTreeItem(impData);
                        }
                        else
                        {
                            impData.parentId = arrayId;
                            AddTreeItem(impData, arrayParent);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        //Added version 2.2.30.0 (FOGBUGZ 15497)
        private void FileImportedOfTypeUTF_16_LE(List<string[]> parsedData, string stationName, uint maxFrameSize, bool bPlcSupportLabel, string filePath, int PositionMemoryArea, bool TheFileIsGxWorks3)
        {
            try
            {
                MelsecQEthDynTagSettings p = new MelsecQEthDynTagSettings();
                string dynamicaddress = String.Empty;
                ImportMovType MovType = new ImportMovType();
                string pouName = String.Empty;
                string varName = String.Empty;
                int arrayStart = 0;
                int arrayDim = 0;
                int arrayNrDim = 0;
                uint arrayElementSize = 0;
                string arrayElementType = String.Empty;
                string ElementType = String.Empty;
                int arrayIndex = 0;
                ImportData arrayParent = null;
                string arrayAddress = String.Empty;
                int arrayId = -1;
                bool arrayElement = false;
                MelsecQAddress arrayAddressObj = new MelsecQAddress();
                MelsecQAddress partialAddressObj = new MelsecQAddress();
                MelsecQAddress variableAddressObj = new MelsecQAddress();
                string varType = String.Empty;
                int TheStringToBeDataType = -1;
                int TheStringToBeVarType = -1;
                int TheStringToBeName = -1;
                int TheStringToBeMemoryArea = -1;
                int TheStringToBeDescripion = -1;
                int TheAccessfromExternalDevice = -1;
                int TheStringToBeLabelAddress = 1;
                if (TheFileIsGxWorks3)
                {
                    TheStringToBeVarType = 0;
                    TheStringToBeName = 1;
                    TheStringToBeDataType = 2;
                    TheStringToBeMemoryArea = PositionMemoryArea;
                    TheStringToBeDescripion = 7;
                    TheAccessfromExternalDevice = 27;
                }
                else
                {
                    TheStringToBeVarType = 1;
                    TheStringToBeName = 3;
                    TheStringToBeDataType = 5;
                    TheStringToBeMemoryArea = 9;
                    TheStringToBeDescripion = 13;
                }
                foreach (var s in parsedData)
                {
                    if (s.Length < 6)
                    {
                        continue;
                    }

                    // Variable declaration?
                    MelsecQEthImportVarType importVarType = MelsecQEthImportVarType.ImportVarType_Invalid;

                    importVarType = IsVarDeclaration(s[TheStringToBeVarType].Trim());
                    if (importVarType == MelsecQEthImportVarType.ImportVarType_Invalid)
                    {
                        // POU declaration?

                        if ((s.Length > 1) && IsPouName(s[1].Trim()))
                        {
                            pouName = s[3].Trim();
                        }
                        continue;
                    }

                    if (s.Length < 7)
                    {
                        continue;
                    }

                    if (TheFileIsGxWorks3)
                    {
                        if (s.Length < 28)
                        {
                            continue;
                        }
                    }

                    // Set the variable name                    
                    varName = s[TheStringToBeName].Trim();
                    if (varName == String.Empty)
                    {
                        continue;
                    }
                    string auxString = varName;
                    varName = auxString.Replace('.', '_');

                    // Array?
                    if (arrayDim == 0)
                    {
                        arrayElement = false;

                        if (!IsArrayDeclaration(s[TheStringToBeDataType].Trim(), ref arrayStart, ref arrayDim, ref MovType, ref arrayElementSize, ref arrayNrDim, ref arrayElementType, bPlcSupportLabel, filePath))
                        {
                            if (arrayDim < 0)
                            {
                                arrayDim = 0;
                                continue;
                            }
                        }
                        else
                        {
                            // Get the address of the first element of
                            // the array and check it
                            if (TheFileIsGxWorks3)
                            {
                                arrayAddress = s[TheStringToBeMemoryArea].Trim();
                            }
                            else
                            {
                                arrayAddress = s[9].Trim();
                            }

                            //if (arrayAddress == String.Empty)
                            //{
                            //    arrayDim = 0;
                            //    continue;
                            //}
                            arrayAddressObj.Set(MelsecQEthProtocol.AddressTypes.DataArea, arrayAddress);
                            if (!arrayAddressObj.IsValid)
                            {
                                // is AccessfromExternalDevice enabled ?
                                if (s[TheAccessfromExternalDevice] == LabelAccessfromExternalDeviceEnabled)
                                {
                                    arrayAddressObj.Set(MelsecQEthProtocol.AddressTypes.Label, s[TheStringToBeLabelAddress]);
                                    if (!arrayAddressObj.IsValid)
                                    {
                                        arrayDim = 0;
                                        continue;
                                    }
                                }
                            }

                            arrayIndex = 0;
                            dynamicaddress = String.Empty;
                            p.StationName = stationName;
                            if (!p.ParseAddress(arrayAddressObj.AddressType, arrayAddressObj.Address))
                            {
                                arrayDim = 0;
                                continue;
                            }
                            dynamicaddress = p.ToString();

                            string varArrayName;
                            int partialArrayDim = 0;
                            int partialArray = 0;
                            if (arrayAddressObj.AddressType == MelsecQEthProtocol.AddressTypes.DataArea)
                            {
                                if (arrayElementSize < 2)
                                    arrayElementSize = 2;
                                partialArrayDim = (int)(MovType.Type != (int)UFUAModel.DataType.Boolean ? maxFrameSize / arrayElementSize : maxFrameSize * 8);
                                if (partialArrayDim > arrayDim)
                                    partialArrayDim = arrayDim;
                                partialArray = (int)((arrayDim + partialArrayDim - 1) / partialArrayDim);
                            }
                            else
                            {
                                // create only 1 array
                                partialArray = 1;
                                partialArrayDim = arrayDim;
                            }

                            int elementIndex = 0;
                            for (int partialIndex = 0; partialIndex < partialArray; partialIndex++)
                            {
                                string devAPartialddress = null;
                                if (arrayAddressObj.AddressType == MelsecQEthProtocol.AddressTypes.DataArea)
                                {
                                    partialAddressObj.Set(arrayAddressObj.AddressType, arrayAddressObj.Address);
                                    devAPartialddress = (arrayAddressObj.StartAddress + elementIndex).ToString();
                                }
                                else
                                {
                                    devAPartialddress = arrayAddressObj.Address;
                                    partialAddressObj.Set(MelsecQEthProtocol.AddressTypes.Label, devAPartialddress);
                                }

                                if (!partialAddressObj.TryToSet(arrayAddressObj.AddressType, devAPartialddress))
                                {
                                    arrayDim = 0;
                                    continue;
                                }

                                if (partialArray > 1)
                                    varArrayName = varName + "_" + (partialIndex + 1).ToString() + "_of_" + partialArray.ToString();
                                else
                                    varArrayName = varName;

                                p.StationName = stationName;
                                if (!p.ParseAddress(arrayAddressObj.AddressType, partialAddressObj.Address))
                                {
                                    arrayDim = 0;
                                    continue;
                                }

                                // array declaration
                                dynamicaddress = p.ToString();
                                ImportData impData = _ImportDataModel.addImportData();
                                impData.Name = varArrayName;
                                ((ImportDataMelsecQ)impData).AddressType = arrayAddressObj.AddressType;
                                impData.Address = partialAddressObj.GetNewAddress(partialAddressObj.AddressType, devAPartialddress);
                                impData.DynAddress = dynamicaddress;
                                ((ImportDataMelsecQ)impData).TagTypeInt = MovType.Type;
                                if (MovType.IsPrototype())
                                {
                                    ((ImportDataMelsecQ)impData).Prototype = MovType.Prototype;
                                    if (!IsInternalPrototype(MovType.Prototype))
                                        // use method only to "initially" validate prototype
                                        ExternalPrototypeExist(MovType, filePath);
                                }
                                impData.szType = s[TheStringToBeDataType].Trim();
                                impData.Description = s[TheStringToBeDescripion].Trim();
                                if (MovType.HasNote())
                                    impData.Description = MovType.Note;
                                impData.ArrayDimension = (uint)(elementIndex + partialArrayDim < arrayDim ? partialArrayDim : arrayDim - elementIndex);
                                impData.parentId = -1;
                                impData.Id = _lGlobalID++;
                                arrayId = impData.Id;
                                arrayParent = impData;
                                if (partialAddressObj.AddressType == MelsecQEthProtocol.AddressTypes.Label)
                                {
                                    AddTreeItem(impData);
                                    
                                    // protocol don't support multidimensional boolean array
                                    if (MovType.Type == (int)UFUAModel.DataType.Boolean && arrayNrDim > 1)
                                    {                                        
                                        impData.Description = string.Format(Properties.Resources.ErrorUnsupportedDataType, GetPrototypeFile(filePath, MovType.Name));
                                        ((ImportDataMelsecQ)impData).TagTypeInt = MelsecQEthFileImportParser.ImportUnknownDataType;
                                        arrayDim = 0;
                                        continue;
                                    }
                                }
                                else
                                {
                                    if (MovType.Type != (int)UFUAModel.DataType.String)
                                        AddTreeItem(impData);
                                }
                                // array elements
                                for (int index = 0; index < partialArrayDim && elementIndex < arrayDim; index++)
                                {
                                    string devAddress = null;
                                    if (arrayAddressObj.AddressType == MelsecQEthProtocol.AddressTypes.DataArea)
                                    {
                                        devAddress = arrayAddressObj.StartAddress.ToString();
                                        variableAddressObj.Set(MelsecQEthProtocol.AddressTypes.DataArea, arrayAddressObj.Address);
                                        MovType = GetMoviconTypeId(s[TheStringToBeDataType].Trim(), bPlcSupportLabel, filePath);

                                        if ((MovType.Type == (int)UFUAModel.DataType.UInt64) ||
                                            (MovType.Type == (int)UFUAModel.DataType.Int64) ||
                                            (MovType.Type == (int)UFUAModel.DataType.Double))
                                        {
                                            devAddress = (Convert.ToInt32(devAddress) + 4 * arrayIndex).ToString();
                                        }
                                        else if (MovType.Type == (int)UFUAModel.DataType.String)
                                        {
                                            if (s[TheStringToBeDataType].Contains("WSTRING"))
                                            {
                                                devAddress = (Convert.ToInt32(devAddress) + (((int)MovType.VarSize + 1) * arrayIndex)).ToString();
                                            }
                                            else
                                            {
                                                devAddress = (Convert.ToInt32(devAddress) + (((int)MovType.VarSize / 2 + 1) * arrayIndex)).ToString();
                                            }
                                        }
                                        else if ((MovType.Type != (int)UFUAModel.DataType.UInt32) &&
                                                 (MovType.Type != (int)UFUAModel.DataType.Int32) &&
                                                 (MovType.Type != (int)UFUAModel.DataType.Float))
                                        {
                                            devAddress = (Convert.ToInt32(devAddress) + arrayIndex).ToString();
                                        }
                                        else
                                        {
                                            devAddress = (Convert.ToInt32(devAddress) + (2 * arrayIndex)).ToString();
                                        }
                                    }
                                    else
                                    {
                                        devAddress = string.Format("{0}[{1}]", arrayAddressObj.Address, (index + arrayStart)).ToString();
                                        variableAddressObj.Set(MelsecQEthProtocol.AddressTypes.Label, devAddress);
                                    }

                                    if (!variableAddressObj.TryToSet(variableAddressObj.AddressType, devAddress))
                                    {
                                        continue;
                                    }
                                    arrayElement = true;
                                    arrayIndex++;

                                    dynamicaddress = String.Empty;
                                    p.StationName = stationName;
                                    if (!p.ParseAddress(variableAddressObj.AddressType, variableAddressObj.Address))
                                    {
                                        continue;
                                    }
                                    dynamicaddress = p.ToString();
                                    if (!MovType.IsUnknownType())
                                    {
                                        ImportData impDataElement = _ImportDataModel.addImportData();
                                        String tmpNameVar = string.Format("{0}_{1}", varName, (elementIndex + arrayStart));
                                        impDataElement.Name = tmpNameVar;
                                        ((ImportDataMelsecQ)impDataElement).AddressType = variableAddressObj.AddressType;
                                        impDataElement.Address = variableAddressObj.GetNewAddress(variableAddressObj.AddressType, devAddress);
                                        impDataElement.DynAddress = dynamicaddress;                                        
                                        if (MovType.IsPrototype())
                                        {
                                            ((ImportDataMelsecQ)impDataElement).Prototype = MovType.Prototype;                                            
                                            if (IsInternalPrototype(MovType.Prototype))
                                                AddInternalPrototype(impDataElement, MovType);
                                            else
                                                AddExternalPrototype(impDataElement, stationName, MovType, filePath);
                                        }
                                        ((ImportDataMelsecQ)impDataElement).TagTypeInt = MovType.Type;
                                        impDataElement.Description = MovType.Note;
                                        impDataElement.szType = arrayElementType;
                                        impDataElement.ArrayDimension = 0;
                                        impDataElement.Id = _lGlobalID++;
                                        if (variableAddressObj.AddressType == MelsecQEthProtocol.AddressTypes.Label)
                                        {
                                            impDataElement.parentId = arrayId;
                                            AddTreeItem(impDataElement, arrayParent);
                                        }
                                        else
                                        {
                                            if (MovType.Type == (int)UFUAModel.DataType.String)
                                            {
                                                impDataElement.parentId = -1;
                                                AddTreeItem(impDataElement);
                                            }
                                            else
                                            {
                                                impDataElement.parentId = arrayId;
                                                AddTreeItem(impDataElement, arrayParent);
                                            }
                                        }
                                        elementIndex++;
                                    }
                                }
                            }
                            arrayDim = 0;
                            continue;
                        }
                    }

                    arrayElement = false;
                    variableAddressObj.Set(MelsecQEthProtocol.AddressTypes.DataArea, s[TheStringToBeMemoryArea].Trim());
                    if (!variableAddressObj.IsValid)
                    {
                        if (bPlcSupportLabel)
                        {
                            // is AccessfromExternalDevice enabled ?
                            if (s[TheAccessfromExternalDevice] == LabelAccessfromExternalDeviceEnabled)
                            {
                                // check if is Label address
                                variableAddressObj.Set(MelsecQEthProtocol.AddressTypes.Label, s[TheStringToBeLabelAddress].Trim());
                                if (!variableAddressObj.IsValid)
                                {
                                    continue;
                                }
                            }
                        }
                        else
                        {
                            continue;
                        }
                    }

                    // Local variable? --> Add the POU name to the variable name
                    if (importVarType == MelsecQEthImportVarType.ImportVarType_VAR_LOCAL)
                    {
                        auxString = pouName + "_" + varName;
                        varName = auxString;
                    }

                    dynamicaddress = String.Empty;
                    p.StationName = stationName;
                    if (!p.ParseAddress(variableAddressObj.AddressType, variableAddressObj.Address))
                    {
                        continue;
                    }
                    dynamicaddress = p.ToString();
                    MovType = GetMoviconTypeId(s[TheStringToBeDataType].Trim(), bPlcSupportLabel, filePath);
                    if (!MovType.IsUnknownType())
                    {
                        ImportData impData = _ImportDataModel.addImportData();
                        impData.Name = varName;
                        ((ImportDataMelsecQ)impData).AddressType = variableAddressObj.AddressType;
                        impData.Address = variableAddressObj.Address;
                        impData.DynAddress = dynamicaddress;                        
                        if (MovType.IsPrototype())
                        {
                            ((ImportDataMelsecQ)impData).Prototype = MovType.Prototype;
                            if (IsInternalPrototype(MovType.Prototype))
                                AddInternalPrototype(impData, MovType);
                            else
                                AddExternalPrototype(impData, stationName, MovType, filePath);
                        }
                        ((ImportDataMelsecQ)impData).TagTypeInt = MovType.Type;                        
                        impData.szType = s[TheStringToBeDataType].Trim();
                        impData.Description = s[TheStringToBeDescripion].Trim();
                        if (MovType.HasNote())
                            impData.Description = MovType.Note;
                        impData.ArrayDimension = 0;
                        impData.Id = _lGlobalID++;

                        impData.parentId = -1;
                        AddTreeItem(impData);
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private bool ImportPrototypeFromFile(ImportData parent, string stationName, string file)
        {
            try
            {
                string filePath = Path.GetDirectoryName(file);
                int iReurnCheckTypeFileEncoding = CheckTypeFileEncoding(file);
                if (iReurnCheckTypeFileEncoding == (int)MelsecQEthFileType.Error)                
                    return false;

                char splitChar = '\t';
                List<string[]> parsedData = new List<string[]>();
                using (StreamReader readFile = new StreamReader(file))
                {
                    string line;
                    string[] row;
                    
                    while ((line = readFile.ReadLine()) != null)
                    {
                        line = line.Replace("\"", "");
                        row = line.Split(splitChar);
                        parsedData.Add(row);
                    }
                }

                FileImportedOfTypeUTF_16_LE_Prototype(parent, parsedData, stationName, filePath);                
            }
            catch (Exception ex)
            {
                _LastError = MelsecQEth.UI.Properties.Resources.ErrorReadFile + ex.ToString();
            }

            return true;
        }

        /// <summary>
        /// Import structure/prototype from file (only for file generated by GX Works and for label address)
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="parsedData"></param>
        /// <param name="stationName"></param>
        /// <param name="filePath"></param>
        private void FileImportedOfTypeUTF_16_LE_Prototype(ImportData parent, List<string[]> parsedData, string stationName, string filePath)
        {
            try
            {
                MelsecQEthDynTagSettings p = new MelsecQEthDynTagSettings();
                string dynamicaddress = String.Empty;
                ImportMovType MovType = new ImportMovType();
                string varName = String.Empty;
                int arrayStart = 0;
                int arrayDim = 0;
                int arrayNrDim = 0;
                uint arrayElementSize = 0;
                string arrayElementType = String.Empty;
                int arrayIndex = 0;
                ImportData arrayParent = null;
                string arrayAddress = String.Empty;
                int arrayId = -1;
                bool arrayElement = false;
                MelsecQAddress arrayAddressObj = new MelsecQAddress();
                MelsecQAddress partialAddressObj = new MelsecQAddress();
                MelsecQAddress variableAddressObj = new MelsecQAddress();
                string varType = String.Empty;
                int TheStringToClass = 0;
                int TheStringToBeName = 1;
                int TheStringToBeDataType = 2;                
                bool bPlcSupportLabel = true;
                
                foreach (var s in parsedData)
                {
                    if (s.Length < 3)
                        continue;

                    if (!string.IsNullOrEmpty(s[TheStringToClass]))
                        continue;

                    if (string.IsNullOrEmpty(s[TheStringToBeName].Trim()) || string.IsNullOrEmpty(s[TheStringToBeDataType].Trim()))
                        continue;

                    // Set the variable name                    
                    varName = s[TheStringToBeName].Trim();
                    varType = s[TheStringToBeDataType].Trim();
                    
                    // Array?
                    if (arrayDim == 0)
                    {
                        arrayElement = false;

                        if (!IsArrayDeclaration(s[TheStringToBeDataType].Trim(), ref arrayStart, ref arrayDim, ref MovType, ref arrayElementSize, ref arrayNrDim, ref arrayElementType, bPlcSupportLabel, filePath))
                        {
                            if (arrayDim < 0)
                            {
                                arrayDim = 0;
                                continue;
                            }
                        }
                        else
                        {
                            arrayAddress = varName;
                            
                            arrayAddressObj.Set(MelsecQEthProtocol.AddressTypes.Label, arrayAddress);

                            
                            arrayIndex = 0;
                            dynamicaddress = String.Empty;
                            p.StationName = stationName;
                            if (!p.ParseAddress(arrayAddressObj.AddressType, arrayAddressObj.Address))
                            {
                                arrayDim = 0;
                                continue;
                            }
                            dynamicaddress = p.ToString();

                            string varArrayName;
                            int partialArrayDim = 0;
                            int partialArray = 0;
                            

                            // create only 1 array
                            partialArray = 1;
                            partialArrayDim = arrayDim;
                            
                            int elementIndex = 0;
                            for (int partialIndex = 0; partialIndex < partialArray; partialIndex++)
                            {
                                string devAPartialddress = null;
                                devAPartialddress = arrayAddressObj.Address;
                                partialAddressObj.Set(MelsecQEthProtocol.AddressTypes.Label, devAPartialddress);
                                

                                if (!partialAddressObj.TryToSet(arrayAddressObj.AddressType, devAPartialddress))
                                {
                                    arrayDim = 0;
                                    continue;
                                }

                                if (partialArray > 1)
                                    varArrayName = varName + "_" + (partialIndex + 1).ToString() + "_of_" + partialArray.ToString();
                                else
                                    varArrayName = varName;

                                p.StationName = stationName;
                                if (!p.ParseAddress(arrayAddressObj.AddressType, partialAddressObj.Address))
                                {
                                    arrayDim = 0;
                                    continue;
                                }

                                // array declaration
                                dynamicaddress = p.ToString();
                                ImportData impData = _ImportDataModel.addImportData();
                                impData.Name = varArrayName;
                                ((ImportDataMelsecQ)impData).AddressType = arrayAddressObj.AddressType;
                                impData.Address = partialAddressObj.GetNewAddress(partialAddressObj.AddressType, devAPartialddress);
                                impData.DynAddress = dynamicaddress;
                                ((ImportDataMelsecQ)impData).TagTypeInt = MovType.Type;
                                if (MovType.IsPrototype())
                                {
                                    ((ImportDataMelsecQ)impData).Prototype = MovType.Prototype;                                    
                                    if (!IsInternalPrototype(MovType.Prototype))
                                        // use method only to "initially" validate prototype
                                        ExternalPrototypeExist(MovType, filePath);
                                }
                                impData.szType = s[TheStringToBeDataType].Trim();
                                impData.Description = string.Empty; // s[TheStringToBeDescripion].Trim();
                                if (MovType.HasNote())
                                    impData.Description = MovType.Note;
                                impData.ArrayDimension = (uint)(elementIndex + partialArrayDim < arrayDim ? partialArrayDim : arrayDim - elementIndex);
                                impData.parentId = -1;
                                impData.Id = _lGlobalID++;
                                arrayId = impData.Id;
                                arrayParent = impData;
                                
                                AddTreeItem(impData, parent);
                                                                
                                // array elements
                                for (int index = 0; index < partialArrayDim && elementIndex < arrayDim; index++)
                                {
                                    string devAddress = null;
                                    if (arrayAddressObj.AddressType == MelsecQEthProtocol.AddressTypes.DataArea)
                                    {
                                        devAddress = arrayAddressObj.StartAddress.ToString();
                                        variableAddressObj.Set(MelsecQEthProtocol.AddressTypes.DataArea, arrayAddressObj.Address);
                                        MovType = GetMoviconTypeId(s[TheStringToBeDataType].Trim(), bPlcSupportLabel, filePath);

                                        if ((MovType.Type == (int)UFUAModel.DataType.UInt64) ||
                                            (MovType.Type == (int)UFUAModel.DataType.Int64) ||
                                            (MovType.Type == (int)UFUAModel.DataType.Double))
                                        {
                                            devAddress = (Convert.ToInt32(devAddress) + 4 * arrayIndex).ToString();
                                        }
                                        else if (MovType.Type == (int)UFUAModel.DataType.String)
                                        {
                                            if (s[TheStringToBeDataType].Contains("WSTRING"))
                                            {
                                                devAddress = (Convert.ToInt32(devAddress) + (((int)MovType.VarSize + 1) * arrayIndex)).ToString();
                                            }
                                            else
                                            {
                                                devAddress = (Convert.ToInt32(devAddress) + (((int)MovType.VarSize / 2 + 1) * arrayIndex)).ToString();
                                            }
                                        }
                                        else if ((MovType.Type != (int)UFUAModel.DataType.UInt32) &&
                                                 (MovType.Type != (int)UFUAModel.DataType.Int32) &&
                                                 (MovType.Type != (int)UFUAModel.DataType.Float))
                                        {
                                            devAddress = (Convert.ToInt32(devAddress) + arrayIndex).ToString();
                                        }
                                        else
                                        {
                                            devAddress = (Convert.ToInt32(devAddress) + (2 * arrayIndex)).ToString();
                                        }
                                    }
                                    else
                                    {
                                        devAddress = string.Format("{0}[{1}]", arrayAddressObj.Address, (index + arrayStart)).ToString();
                                        variableAddressObj.Set(MelsecQEthProtocol.AddressTypes.Label, devAddress);
                                    }

                                    if (!variableAddressObj.TryToSet(variableAddressObj.AddressType, devAddress))
                                    {
                                        continue;
                                    }
                                    arrayElement = true;
                                    arrayIndex++;

                                    dynamicaddress = String.Empty;
                                    p.StationName = stationName;
                                    if (!p.ParseAddress(variableAddressObj.AddressType, variableAddressObj.Address))
                                    {
                                        continue;
                                    }
                                    dynamicaddress = p.ToString();
                                    if (!MovType.IsUnknownType())
                                    {
                                        ImportData impDataElement = _ImportDataModel.addImportData();
                                        String tmpNameVar = string.Format("{0}_{1}", varName, (elementIndex + arrayStart));
                                        impDataElement.Name = tmpNameVar;
                                        ((ImportDataMelsecQ)impDataElement).AddressType = variableAddressObj.AddressType;
                                        impDataElement.Address = variableAddressObj.GetNewAddress(variableAddressObj.AddressType, devAddress);
                                        impDataElement.DynAddress = dynamicaddress;
                                        if (MovType.IsPrototype())
                                        {
                                            ((ImportDataMelsecQ)impDataElement).Prototype = MovType.Prototype;
                                            if (IsInternalPrototype(MovType.Prototype))
                                                AddInternalPrototype(impDataElement, MovType);
                                            else
                                                AddExternalPrototype(impDataElement, stationName, MovType, filePath);
                                        }
                                        ((ImportDataMelsecQ)impDataElement).TagTypeInt = MovType.Type;
                                        impDataElement.Description = MovType.Note;
                                        impDataElement.szType = arrayElementType;
                                        impDataElement.ArrayDimension = 0;
                                        impDataElement.Id = _lGlobalID++;
                                        if (variableAddressObj.AddressType == MelsecQEthProtocol.AddressTypes.Label)
                                        {
                                            impDataElement.parentId = arrayId;
                                            AddTreeItem(impDataElement, arrayParent);
                                        }                                        
                                        elementIndex++;
                                    }
                                }
                            }
                            arrayDim = 0;
                            continue;
                        }
                    }

                    arrayElement = false;
                    variableAddressObj.Set(MelsecQEthProtocol.AddressTypes.Label, string.Format("{0}.{1}", parent.Name, s[TheStringToBeName].Trim()));
                    if (!variableAddressObj.IsValid)
                        continue;

                    dynamicaddress = String.Empty;
                    p.StationName = stationName;
                    if (!p.ParseAddress(variableAddressObj.AddressType, variableAddressObj.Address))
                        continue;

                    dynamicaddress = p.ToString();
                    MovType = GetMoviconTypeId(s[TheStringToBeDataType].Trim(), bPlcSupportLabel, filePath);
                    if (!MovType.IsUnknownType())
                    {
                        ImportData impData = _ImportDataModel.addImportData();
                        impData.Name = varName;
                        ((ImportDataMelsecQ)impData).AddressType = variableAddressObj.AddressType;
                        impData.Address = variableAddressObj.Address;
                        impData.DynAddress = dynamicaddress;
                        if (MovType.IsPrototype())
                        {
                            ((ImportDataMelsecQ)impData).Prototype = MovType.Prototype;
                            if (IsInternalPrototype(MovType.Prototype))
                                AddInternalPrototype(impData, MovType);
                            else
                                AddExternalPrototype(impData, stationName, MovType, filePath);                            
                        }
                        ((ImportDataMelsecQ)impData).TagTypeInt = MovType.Type;
                        impData.szType = s[TheStringToBeDataType].Trim();
                        impData.Description = string.Empty;// s[TheStringToBeDescripion].Trim();
                        if (MovType.HasNote())
                            impData.Description = MovType.Note;
                        impData.ArrayDimension = 0;
                        impData.Id = _lGlobalID++;                                                
                        AddTreeItem(impData, parent);
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        //Added version 2.2.30.0 (FOGBUGZ 15497)
        private int CheckTypeFileEncoding(String file)
        {
            int tmpByte = 0;
            try
            {
                using (System.IO.StreamReader readFile = new System.IO.StreamReader(file))
                {
                    tmpByte = readFile.Read();
                    if (tmpByte > 0)
                    {
                        if ((readFile.CurrentEncoding.EncodingName == System.Text.Encoding.UTF8.EncodingName) ||
                            (readFile.CurrentEncoding.EncodingName == System.Text.Encoding.ASCII.EncodingName))
                        {
                            tmpByte = (int)MelsecQEthFileType.Type_ASCII;
                        }
                        else if (readFile.CurrentEncoding.EncodingName == System.Text.Encoding.Unicode.EncodingName)
                        {
                            tmpByte = (int)MelsecQEthFileType.Type_UNICODE_16;
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {
                throw;
            }
            return (tmpByte);
        }

        private ImportMovType GetMoviconTypeId(string Type, bool bPlcSupportLabel, string filePath)
        {
            ImportMovType result = new ImportMovType();

            Type = Type.Trim().ToUpper();

            result.Name = Type;
            if (Type.Length == 0)
                return result;

            if (Type.Contains("BIT"))
            {
                result.Type = (int)UFUAModel.DataType.Boolean;
                result.VarSize = 1;
            }
            else if (Type.Contains("BOOL"))
            {
                result.Type = (int)UFUAModel.DataType.Boolean;
                result.VarSize = 1;
            }
            else if (Type.Contains("BYTE"))
            {
                result.Type = (int)UFUAModel.DataType.Byte;
                result.VarSize = 1;
            }
            //Modified version 2.2.30.0 (FOGBUGZ 15497)
            //Contains Returns a value indicating whether a specified substring occurs within this string, 
            //so the string "DWORD" must be checked before the string "WORD".
            else if (Type.Contains("DWORD"))
            {
                result.Type = (int)UFUAModel.DataType.UInt32;
                result.VarSize = 4;
            }
            else if (Type.Contains("WORD"))
            {
                result.Type = (int)UFUAModel.DataType.UInt16;
                result.VarSize = 2;
            }
            //Modified version 2.2.30.0 (FOGBUGZ 15497)
            //Contains Returns a value indicating whether a specified substring occurs within this string, 
            //so the string "DINT" mast be checked before the string "INT". 
            else if (Type.Contains("DINT"))
            {
                result.Type = (int)UFUAModel.DataType.Int32;
                result.VarSize = 4;
            }
            else if (Type.Contains("INT"))
            {
                result.Type = (int)UFUAModel.DataType.Int16;
                result.VarSize = 2;
            }
            else if (Type.Contains("LREAL"))
            {
                result.Type = (int)UFUAModel.DataType.Double;
                result.VarSize = 8;
            }
            else if (Type.Contains("REAL"))
            {
                result.Type = (int)UFUAModel.DataType.Float;
                result.VarSize = 4;
            }
            else if (Type.Contains("CHAR"))
            {
                result.Type = (int)UFUAModel.DataType.SByte;
                result.VarSize = 1;
            }
            else if (Type.Contains("WSTRING"))
            {
                result.Type = (int)UFUAModel.DataType.String;
                result.VarSize = GetStringLength(Type);
                if (result.VarSize == 0)
                    result.Type = MelsecQEthFileImportParser.ImportUnknownDataType;
            }
            else if (Type.Contains("STRING"))
            {
                result.Type = (int)UFUAModel.DataType.String;
                result.VarSize = GetStringLength(Type);
                if (result.VarSize == 0)
                    result.Type = MelsecQEthFileImportParser.ImportUnknownDataType;
            }
            else if (bPlcSupportLabel)
            {
                switch (Type)
                {
                    case "TIME":
                        result.Type = (int)UFUAModel.DataType.Int32;
                        result.VarSize = 4;
                        break;
                    case "LTIMER":
                        result.Type = (int)ImportDataTypeStruct;
                        result.Prototype = DATATYPEPROTOTYPE_LTIMER;
                        break;
                    case "TIMER":
                        result.Type = (int)ImportDataTypeStruct;
                        result.Prototype = DATATYPEPROTOTYPE_TIMER;
                        break;
                    case "LCOUNTER":
                        result.Type = (int)ImportDataTypeStruct;
                        result.Prototype = DATATYPEPROTOTYPE_LCOUNTER;
                        break;
                    case "COUNTER":
                        result.Type = (int)ImportDataTypeStruct;
                        result.Prototype = DATATYPEPROTOTYPE_COUNTER;
                        break;
                    case "LRETENTIVETIMER":
                        result.Type = (int)ImportDataTypeStruct;
                        result.Prototype = DATATYPEPROTOTYPE_LRETENTIVETIMER;
                        break;
                    case "RETENTIVETIMER":
                        result.Type = (int)ImportDataTypeStruct;
                        result.Prototype = DATATYPEPROTOTYPE_RETENTIVETIMER;                        
                        break;
                    default:
                        // consider as a protoype (unsupported data type will be checked later)
                        result.Type = (int)ImportDataTypeStruct;
                        result.Prototype = Type;                        
                        break;
                }
            }
            return result;
        }

        string GetPrototypeFile(string filePath, string prototypeName)
        {
            return Path.Combine(filePath, string.Format("{0}.csv", prototypeName));
        }

        bool PrototypeFileExist(string filePath, string prototypeName)
        {
            bool result = false;
            try
            {
                result = File.Exists(GetPrototypeFile(filePath, prototypeName));
            }
            catch (Exception ex) { }

            return result;
        }

        public static uint GetStringLength(string Type)
        {
            var start = Type.LastIndexOf("[") + 1;
            string match2 = Type.Substring(start, Type.LastIndexOf("]") - start);
            uint n;
            if (!uint.TryParse(match2, out n))
            {
                return (0);
            }
            return (n);
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
                _ImportDataModel.Children.Add(tag);
            }
            else
            {
                tag.TreeLevel = parent.TreeLevel - 1;
                string strLevel = tag.TreeLevel.ToString("X2");
                tag.parentId = parent.Id;
                parent.Children.Add(tag);
            }
        }

        private bool IsInternalPrototype(string prototype)
        {
            return (prototype == DATATYPEPROTOTYPE_TIMER
                    || prototype == DATATYPEPROTOTYPE_LTIMER
                    || prototype == DATATYPEPROTOTYPE_COUNTER
                    || prototype == DATATYPEPROTOTYPE_LCOUNTER
                    || prototype == DATATYPEPROTOTYPE_RETENTIVETIMER
                    || prototype == DATATYPEPROTOTYPE_LRETENTIVETIMER);
        }

        private ImportData CreateInternalPrototypeMember(ImportData single, MelsecQEthProtocol.AddressTypes addressType, string name, int dataType, string szType, uint arrayDimension = 0, string description = null)
        {
            ImportData impData = _ImportDataModel.addImportData();
            impData.Name = name;

            MelsecQEthDynTagSettings sp = new MelsecQEthDynTagSettings();
            if (!sp.ParseAddress(addressType, string.Format("{0}.{1}", single.Address, name)))
                return null;
            sp.ArrayDimension = arrayDimension;

            ((ImportDataMelsecQ)impData).AddressType = addressType;
            ((ImportDataMelsecQ)impData).Address = string.Format("{0}.{1}", single.Address, name);
            ((ImportDataMelsecQ)impData).TagTypeInt = dataType;
            impData.Description = String.Empty;
            ((ImportDataMelsecQ)impData).DynAddress = sp.ToString();
            impData.szType = szType;
            impData.ArrayDimension = arrayDimension;
            impData.parentId = -1;
            impData.Id = _lGlobalID++;

            return impData;
        }

        private void AddInternalPrototype(ImportData single, ImportMovType movType)
        {
            switch (movType.Prototype)
            {
                case DATATYPEPROTOTYPE_TIMER:
                    single.Children.Add(CreateInternalPrototypeMember(single, ((ImportDataMelsecQ)single).AddressType, "S", (int)UFUAModel.DataType.Boolean, "BOOL"));
                    single.Children.Add(CreateInternalPrototypeMember(single, ((ImportDataMelsecQ)single).AddressType, "C", (int)UFUAModel.DataType.Boolean, "BOOL"));
                    single.Children.Add(CreateInternalPrototypeMember(single, ((ImportDataMelsecQ)single).AddressType, "N", (int)UFUAModel.DataType.UInt16, "WORD"));
                    break;
                case DATATYPEPROTOTYPE_LTIMER:
                    single.Children.Add(CreateInternalPrototypeMember(single, ((ImportDataMelsecQ)single).AddressType, "S", (int)UFUAModel.DataType.Boolean, "BOOL"));
                    single.Children.Add(CreateInternalPrototypeMember(single, ((ImportDataMelsecQ)single).AddressType, "C", (int)UFUAModel.DataType.Boolean, "BOOL"));
                    single.Children.Add(CreateInternalPrototypeMember(single, ((ImportDataMelsecQ)single).AddressType, "N", (int)UFUAModel.DataType.UInt32, "DWORD"));
                    break;
                case DATATYPEPROTOTYPE_COUNTER:
                    single.Children.Add(CreateInternalPrototypeMember(single, ((ImportDataMelsecQ)single).AddressType, "S", (int)UFUAModel.DataType.Boolean, "BOOL"));
                    single.Children.Add(CreateInternalPrototypeMember(single, ((ImportDataMelsecQ)single).AddressType, "C", (int)UFUAModel.DataType.Boolean, "BOOL"));
                    single.Children.Add(CreateInternalPrototypeMember(single, ((ImportDataMelsecQ)single).AddressType, "N", (int)UFUAModel.DataType.UInt16, "WORD"));
                    break;
                case DATATYPEPROTOTYPE_LCOUNTER:
                    single.Children.Add(CreateInternalPrototypeMember(single, ((ImportDataMelsecQ)single).AddressType, "S", (int)UFUAModel.DataType.Boolean, "BOOL"));
                    single.Children.Add(CreateInternalPrototypeMember(single, ((ImportDataMelsecQ)single).AddressType, "C", (int)UFUAModel.DataType.Boolean, "BOOL"));
                    single.Children.Add(CreateInternalPrototypeMember(single, ((ImportDataMelsecQ)single).AddressType, "N", (int)UFUAModel.DataType.UInt32, "DWORD"));
                    break;
                case DATATYPEPROTOTYPE_RETENTIVETIMER:
                    single.Children.Add(CreateInternalPrototypeMember(single, ((ImportDataMelsecQ)single).AddressType, "S", (int)UFUAModel.DataType.Boolean, "BOOL"));
                    single.Children.Add(CreateInternalPrototypeMember(single, ((ImportDataMelsecQ)single).AddressType, "C", (int)UFUAModel.DataType.Boolean, "BOOL"));
                    single.Children.Add(CreateInternalPrototypeMember(single, ((ImportDataMelsecQ)single).AddressType, "N", (int)UFUAModel.DataType.UInt16, "WORD"));
                    break;
                case DATATYPEPROTOTYPE_LRETENTIVETIMER:
                    single.Children.Add(CreateInternalPrototypeMember(single, ((ImportDataMelsecQ)single).AddressType, "S", (int)UFUAModel.DataType.Boolean, "BOOL"));
                    single.Children.Add(CreateInternalPrototypeMember(single, ((ImportDataMelsecQ)single).AddressType, "C", (int)UFUAModel.DataType.Boolean, "BOOL"));
                    single.Children.Add(CreateInternalPrototypeMember(single, ((ImportDataMelsecQ)single).AddressType, "N", (int)UFUAModel.DataType.UInt32, "DWORD"));
                    break;
            }
        }

        private ImportPrototype addPrototype(Dictionary<string, ImportPrototype> protoMap, ImportData elem)
        {
            ImportPrototype proto = null;
            string protoname = ((ImportDataMelsecQ)elem).Prototype;
            if (!protoMap.ContainsKey(protoname))
            {
                if (elem.Children.Count != 0)
                {
                    proto = new ImportPrototype();
                    proto.Name = UFUAModel.Helpers.NameValidator.EnsureValidName(protoname);
                    proto.Elements = new List<ImportTag>();

                    foreach (var t in elem.Children)
                    {
                        ImportTag a = new ImportTag();

                        if (((ImportDataMelsecQ)t).TagTypeInt == ImportDataTypeStruct)
                        {
                            ImportPrototype protoElem = addPrototype(protoMap, t);
                            if (protoElem == null)
                                continue;

                            a.ModelType = UFUAModel.ModelType.ObjectType;
                            a.PrototypeModel = protoElem.Name;
                        }
                        else
                        {
                            a.ModelType = UFUAModel.ModelType.Variable;
                        }
                        a.ArrayDimension = t.ArrayDimension;
                        a.Name = t.Name;
                        a.DataType = (DataType)((ImportDataMelsecQ)t).TagTypeInt;
                        a.Description = t.Description;
                        proto.Elements.Add(a);
                    }
                    if (!protoMap.ContainsKey(protoname))
                    {
                        protoMap.Add(protoname, proto);
                    }
                }
            }
            else
                proto = protoMap[protoname];
            return proto;
        }

        private void AddExternalPrototype(ImportData parent, string stationName, ImportMovType movType, string filePath)
        {
            // check if struct file exist
            if (!ExternalPrototypeExist(movType, filePath))
                return;

            ImportPrototypeFromFile(parent, stationName, GetPrototypeFile(filePath, movType.Prototype));
        }

        private bool ExternalPrototypeExist(ImportMovType movType, string filePath)
        {
            if (!PrototypeFileExist(filePath, movType.Prototype))
            {
                movType.Type = MelsecQEthFileImportParser.ImportUnknownDataType;
                movType.Note = string.Format(Properties.Resources.ErrorUnsupportedDataType, GetPrototypeFile(filePath, movType.Prototype));                
            }

            return (movType.Type != MelsecQEthFileImportParser.ImportUnknownDataType);
        }
    }
}