using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using Utilities;
using Utilities.WPF;
using System.Reflection;
using UFUAModel;
using System.Collections.ObjectModel;
using System.ComponentModel;
using DriverCodeBase.UI;
using DriverCodeBase.UI.Controls;

namespace MelsecQEth.UI
{
    public enum MelsecQEthImportVarType : int
    {
        ImportVarType_Invalid,
        ImportVarType_VAR_GLOBAL,
        ImportVarType_VAR_LOCAL
    }

    public enum MelsecQEthFileType : int
    {
        Type_ASCII = 1,
        Type_UNICODE_16 = 2,
        Error = -1
    }
    
    /// <summary>
    /// Interaction logic for ImportTagsEditorTree.xaml
    /// </summary>
    public partial class ImportTagsEditorTree : UserControl, IDisposable
    {
        ImportDataModelMelsecQ importDataModel;
        bool alreadyLoaded = false;
        BaseImportTree baseImportTree;

        int m_lGlobalID = 0;
        public GetStationName readStationName;

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
                    colWidth = 200
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
                baseImportTree.SetFileFilter("csv files|*.csv");
                baseImportTree.LoadImportFile = LoadFile;
                MainStack.Children.Add(baseImportTree);

                readStationName = baseImportTree.funcGetStationName();

                DataContext = this;
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
                List<ImportPrototype> protolist = new List<ImportPrototype>();
                ObservableCollection<object> notToBeImported = new ObservableCollection<object>();
                foreach (var elem in list)
                {
                    ImportData single = elem as ImportData;
                    if (single != null && (notToBeImported.IndexOf(single) < 0))
                    {
                     
                        bool isArray = (single.ArrayDimension > 0) ? true : false;

                        MelsecQEthDynTagSettings sp = new MelsecQEthDynTagSettings();
                        if (!sp.ParseAddress(single.Address))
                            continue;
                        sp.StationName = stationName;
                        single.DynAddress = sp.ToString();

                        sp.ArrayDimension = single.ArrayDimension;

                        string importTagName = single.Name;
                        //importTagName = UFUAModel.Helpers.NameValidator.EnsureValidName(importTagName);

                        ImportTag tagtoimport = new ImportTag()
                        {
                            Name = importTagName,
                            DataType = (DataType)((ImportDataMelsecQ)single).TagTypeInt,
                            DynSettings = single.DynAddress,
                            Folder = importfolder,
                            ModelType = UFUAModel.ModelType.Variable,
                            Description = single.Description,

                            ArrayDimension = single.ArrayDimension,
                            BehaviorForExistingTags = (BehaviorExistingTagsValues)behaviorExistingTags,
                            BehaviorForDynamicLink = (BehaviorDynamicLinkValues)behaviorDynamicLink,
                        };

                        if(tagtoimport.DataType == DataType.String)
                        {
                           
                            tagtoimport.DynSettings += "|DL=" + GetStringLength( elem.szType);
                            //I
                            if (elem.szType.Contains("WSTRING"))
                            {
                                tagtoimport.DynSettings += "|UN=True";
                            }
                            else
                            {
                                tagtoimport.DynSettings += "|UN=False";
                            }
                        }

                        taglist.Add(tagtoimport);

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
                DataContext = new ImportObject() { PrototypesToImport = protolist, TagsToImport = taglist };
            }
            else
                DataContext = this;
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

        private bool IsArrayDeclaration(string rowField,
                                       ref int arrayDim,
                                       ref int arrayElementMoviconType,
                                       ref uint arrayElementSize,
                                       ref string ElementType)
        {
            arrayDim = 1;
            arrayElementMoviconType = -1;
            arrayElementSize = 0;
            ElementType = String.Empty;

            try
            {
                // Array declaration?
                string typeField = rowField.ToUpper();
                if (!typeField.StartsWith("ARRAY ["))
                {
                    arrayDim = 0;
                    return false;
                }

                // Parse the array dimension and type
                int firstIndex = typeField.IndexOf('[');
                if (firstIndex < 0)
                {
                    return false;
                }
                int closeBracketIndex =-1;
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
                    int arrayFirstIndex = Convert.ToInt32(firstArrayIndex);

                    int lastIndex = typeField.IndexOf(",", doublePointIndex);
                    if(lastIndex < 0)
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
                    // Set the array dimension
                    arrayDim *= (arrayLastIndex - arrayFirstIndex + 1);
                    firstIndex = lastIndex;
                } while (closeBracketIndex<0);

                int ofIndex = typeField.IndexOf(" OF ");
                if ( (ofIndex <= closeBracketIndex) ||
                    (typeField.Length <= (ofIndex + 4)))
                {
                    return false;
                }
                // Parse the array type
                string elemType = typeField.Substring(ofIndex + 4);
                ElementType = elemType.Trim();
                uint VarSize = 0;
                arrayElementMoviconType = GetMoviconTypeId(ElementType,
                                                           ref VarSize);
                if (arrayElementMoviconType == -1)
                {
                    ElementType = String.Empty;
                    return false;
                }

                // Set the array element size
                arrayElementSize = VarSize;
            }
            catch (Exception e)
            {
            }

            return true;
        }

        private ImportDataModel LoadFile(string file)
        {
            importDataModel = new ImportDataModelMelsecQ(readStationName);

            file = file.ToLower();
            if (file.Contains(".csv"))
            {
                //Modified version 2.2.30.0 (FOGBUGZ 15497)
                //try
                //{
                //    using (new WaitCursor())
                //    {
                //        List<string[]> parsedData = new List<string[]>();
                //        using (System.IO.StreamReader readFile = new System.IO.StreamReader(file))
                //        {
                //            string line;
                //            string[] row;

                //            while ((line = readFile.ReadLine()) != null)
                //            {
                //                row = line.Split(';');
                //                parsedData.Add(row);
                //            }
                //        }

                //        MelsecQEthDynTagSettings p = new MelsecQEthDynTagSettings();
                //        string dynamicaddress = String.Empty;
                //        int MovType = 0;
                //        uint varsize = 0;
                //        string pouName = String.Empty;
                //        string varName = String.Empty;
                //        int arrayDim = 0;
                //        uint arrayElementSize = 0;
                //        string arrayElementType = String.Empty;
                //        int arrayIndex = 0;
                //        ImportData arrayParent = null;
                //        string arrayAddress = String.Empty;
                //        int arrayId = -1;
                //        bool arrayElement = false;
                //        MelsecQAddress arrayAddressObj = new MelsecQAddress();
                //        MelsecQAddress variableAddressObj = new MelsecQAddress();
                //        string varType = String.Empty;
                //        foreach (var s in parsedData)
                //        {
                //            if (s.Length == 0)
                //            {
                //                continue;
                //            }

                //            // Variable declaration?
                //            MelsecQEthImportVarType importVarType = IsVarDeclaration(s[0].Trim());
                //            if (importVarType == MelsecQEthImportVarType.ImportVarType_Invalid)
                //            {
                //                // POU declaration?
                //                if ((s.Length > 1) && IsPouName(s[0].Trim()))
                //                {
                //                    pouName = s[1].Trim();
                //                }
                //                continue;
                //            }

                //            if (s.Length < 7)
                //            {
                //                continue;
                //            }

                //            // Set the variable name
                //            varName = s[1].Trim();
                //            if (varName == String.Empty)
                //            {
                //                continue;
                //            }
                //            string auxString = varName;
                //            varName = auxString.Replace('.', '_');

                //            // Array?
                //            if (arrayDim == 0)
                //            {
                //                arrayElement = false;
                //                if (!IsArrayDeclaration(s[4].Trim(),
                //                   ref arrayDim, ref MovType,
                //                   ref arrayElementSize, ref arrayElementType))
                //                {
                //                    if (arrayDim < 0)
                //                    {
                //                        arrayDim = 0;
                //                        continue;
                //                    }
                //                }
                //                else
                //                {
                //                    // Get the address of the first element of
                //                    // the array and check it
                //                    arrayAddress = s[3].Trim();
                //                    if (arrayAddress == String.Empty)
                //                    {
                //                        arrayDim = 0;
                //                        continue;
                //                    }
                //                    arrayAddressObj.Set(arrayAddress);
                //                    if (!arrayAddressObj.IsValid)
                //                    {
                //                        arrayDim = 0;
                //                        continue;
                //                    }

                //                    arrayIndex = 0;
                //                    dynamicaddress = String.Empty;
                //                    p.StationName = CmbStation.Text;
                //                    if (!p.ParseAddress(arrayAddressObj.Address))
                //                    {
                //                        arrayDim = 0;
                //                        continue;
                //                    }
                //                    dynamicaddress = p.ToString();

                //                    ImportData impData = importDataModel.addImportData();
                //                    impData.Name = varName;
                //                    impData.Address = arrayAddressObj.Address;
                //                    impData.DynAddress = dynamicaddress;
                //                    impData.TagType = MovType;
                //                    impData.Select = false;
                //                    impData.szType = s[4].Trim();
                //                    impData.ArrayDimension = (uint)arrayDim;
                //                    impData.parentId = -1;
                //                    impData.Id = m_lGlobalID++;
                //                    arrayId = impData.Id;
                //                    arrayParent = impData;

                //                    AddTreeItem(impData);

                //                    continue;
                //                }
                //            }

                //            // Special case: element of a previously declared array
                //            if (arrayDim > 0)
                //            {
                //                variableAddressObj.Set(arrayAddressObj.Address);
                //                int devAddress = arrayAddressObj.StartAddress;
                //                if ((MovType != (int)UFUAModel.DataType.UInt32) &&
                //                    (MovType != (int)UFUAModel.DataType.Int32) &&
                //                    (MovType != (int)UFUAModel.DataType.Float))
                //                {
                //                    devAddress += arrayIndex;
                //                }
                //                else
                //                {
                //                    devAddress += 2 * arrayIndex;
                //                }
                //                if (!variableAddressObj.Set(devAddress))
                //                {
                //                    continue;
                //                }
                //                arrayElement = true;
                //                arrayIndex++;
                //                if (arrayIndex >= arrayDim)
                //                {
                //                    arrayDim = 0;
                //                    arrayIndex++;
                //                }
                //            }

                //            // Variable
                //            else
                //            {
                //                arrayElement = false;
                //                variableAddressObj.Set(s[3].Trim());
                //                if (!variableAddressObj.IsValid)
                //                {
                //                    continue;
                //                }
                //            }

                //            // Local variable? --> Add the POU name to the variable name
                //            if (importVarType == MelsecQEthImportVarType.ImportVarType_VAR_LOCAL)
                //            {
                //                auxString = pouName + "_" + varName;
                //                varName = auxString;
                //            }

                //            dynamicaddress = String.Empty;
                //            p.StationName = CmbStation.Text;
                //            if (!p.ParseAddress(variableAddressObj.Address))
                //            {
                //                continue;
                //            }
                //            dynamicaddress = p.ToString();
                //            MovType = GetMoviconTypeId(s[4].Trim(), ref varsize);
                //            if (MovType != -1)
                //            {
                //                ImportData impData = importDataModel.addImportData();
                //                impData.Name = varName;
                //                impData.Address = variableAddressObj.Address;
                //                impData.DynAddress = dynamicaddress;
                //                impData.TagType = MovType;
                //                impData.Select = false;
                //                impData.szType = s[4].Trim();
                //                impData.ArrayDimension = 0;
                //                impData.Id = m_lGlobalID++;
                //                if (!arrayElement)
                //                {
                //                    impData.parentId = -1;
                //                    AddTreeItem(impData);
                //                }
                //                else
                //                {
                //                    impData.parentId = arrayId;
                //                    AddTreeItem(impData, arrayParent);
                //                }
                //            }
                //        }
                //    }

                //    ImportTree.Model = importDataModel;
                //    if (listViewSortCol != null)
                //    {
                //        AdornerLayer.GetAdornerLayer(listViewSortCol).Remove(listViewSortAdorner);
                //        ImportTree.Items.SortDescriptions.Clear();
                //    }
                //}
                //catch (Exception ex)
                //{
                //    throw;
                //}

                try
                {
                    using (new WaitCursor())
                    {

                        int iReurnCheckTypeFileEncoding = CheckTypeFileEncoding(file);
                        if (iReurnCheckTypeFileEncoding == (int)MelsecQEthFileType.Error)
                        {
                            return null;
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
                                    if (row[4].Contains("Initial Value") )
                                    {
                                        if(row[5].Contains("Assign (Device/Label)"))
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
                        {                            
                            FileImportedOfTypeUTF_16_LE(parsedData, PositionMemoryArea, TheFileIsGxWorks3);                           
                        }
                        else  
                        {
                            FileImportedOfTypeASCII(parsedData);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(MelsecQEth.UI.Properties.Resources.ErrorReadFile + ex.ToString());
                    importDataModel = null;
                }

                if (importDataModel == null || importDataModel.Children.Count == 0)
                {
                    MessageBox.Show(DriverCodeBase.UI.Properties.Resources.ImportErrorEmptyFile,
                                    Properties.Resources.ImportMsgBoxTitle);
                }
            }

            return importDataModel;

        }

        //Added version 2.2.30.0 (FOGBUGZ 15497)
        private void FileImportedOfTypeASCII(List<string[]> parsedData)
        {
            string stationName = baseImportTree.ReadStationName();

            try
            {                                    
                MelsecQEthDynTagSettings p = new MelsecQEthDynTagSettings();
                string dynamicaddress = String.Empty;
                int MovType = 0;
                uint varsize = 0;
                string pouName = String.Empty;
                string varName = String.Empty;
                int arrayDim = 0;
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
                        if (!IsArrayDeclaration(s[4].Trim(),
                            ref arrayDim, ref MovType,
                            ref arrayElementSize, ref arrayElementType))
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
                            arrayAddressObj.Set(arrayAddress);
                            if (!arrayAddressObj.IsValid)
                            {
                                arrayDim = 0;
                                continue;
                            }

                            arrayIndex = 0;
                            dynamicaddress = String.Empty;
                            p.StationName = stationName;
                            if (!p.ParseAddress(arrayAddressObj.Address))
                            {
                                arrayDim = 0;
                                continue;
                            }
                            dynamicaddress = p.ToString();

                            ImportData impData = importDataModel.addImportData();
                            impData.Name = varName;
                            impData.Address = arrayAddressObj.Address;
                            impData.DynAddress = dynamicaddress;
                            ((ImportDataMelsecQ)impData).TagTypeInt = MovType;
                            impData.Select = false;
                            impData.szType = s[4].Trim();
                            impData.ArrayDimension = (uint)arrayDim;
                            impData.parentId = -1;
                            impData.Id = m_lGlobalID++;
                            arrayId = impData.Id;
                            arrayParent = impData;

                            AddTreeItem(impData);

                            continue;
                        }
                    }
                           
                    // Special case: element of a previously declared array
                    if (arrayDim > 0)
                    {
                        variableAddressObj.Set(arrayAddressObj.Address);
                        int devAddress = arrayAddressObj.StartAddress;
                        if ((MovType != (int)UFUAModel.DataType.UInt32) &&
                            (MovType != (int)UFUAModel.DataType.Int32) &&
                            (MovType != (int)UFUAModel.DataType.Float))
                        {
                            devAddress += arrayIndex;
                        }
                        else
                        {
                            devAddress += 2 * arrayIndex;
                        }
                        if (!variableAddressObj.Set(devAddress))
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
                        variableAddressObj.Set(s[3].Trim());
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
                    if (!p.ParseAddress(variableAddressObj.Address))
                    {
                        continue;
                    }
                    dynamicaddress = p.ToString();
                    MovType = GetMoviconTypeId(s[4].Trim(), ref varsize);
                    if (MovType != -1)
                    {
                        ImportData impData = importDataModel.addImportData();
                        impData.Name = varName;
                        impData.Address = variableAddressObj.Address;
                        impData.DynAddress = dynamicaddress;
                        ((ImportDataMelsecQ)impData).TagTypeInt = MovType;
                        impData.Select = false;
                        impData.szType = s[4].Trim();
                        impData.ArrayDimension = 0;
                        impData.Id = m_lGlobalID++;
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
        private void FileImportedOfTypeUTF_16_LE(List<string[]> parsedData, int PositionMemoryArea, bool TheFileIsGxWorks3)
        {
            string stationName = baseImportTree.ReadStationName();

            try
            {
                MelsecQEthDynTagSettings p = new MelsecQEthDynTagSettings();
                string dynamicaddress = String.Empty;
                int MovType = 0;
                uint varsize = 0;
                string pouName = String.Empty;
                string varName = String.Empty;
                int arrayDim = 0;
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
                if (TheFileIsGxWorks3)
                {
                    TheStringToBeVarType = 0;
                    TheStringToBeName = 1;
                    TheStringToBeDataType = 2;
                    TheStringToBeMemoryArea = PositionMemoryArea;
                    TheStringToBeDescripion = 7;
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
                       
                        if (!IsArrayDeclaration(s[TheStringToBeDataType].Trim(),
                            ref arrayDim, ref MovType,
                            ref arrayElementSize, ref arrayElementType))
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

                            if (arrayAddress == String.Empty)
                            {
                                arrayDim = 0;
                                continue;
                            }
                            arrayAddressObj.Set(arrayAddress);
                            if (!arrayAddressObj.IsValid)
                            {
                                arrayDim = 0;
                                continue;
                            }

                            arrayIndex = 0;
                            dynamicaddress = String.Empty;
                            p.StationName = stationName;
                            if (!p.ParseAddress(arrayAddressObj.Address))
                            {
                                arrayDim = 0;
                                continue;
                            }
                            dynamicaddress = p.ToString();

                            string varArrayName;
                            if (arrayElementSize < 2)
                                arrayElementSize = 2;
                            int partialArrayDim = (int)(MovType != (int)UFUAModel.DataType.Boolean ? MelsecQEthCommJob.MAXBYTE_SIZE / arrayElementSize : MelsecQEthCommJob.MAXBYTE_SIZE * 8);
                            if (partialArrayDim > arrayDim)
                                partialArrayDim = arrayDim;
                            int partialArray = (int)((arrayDim + partialArrayDim - 1) / partialArrayDim);
                            int elementIndex = 0;
                            for (uint partialIndex = 0; partialIndex < partialArray; partialIndex++)
                            {
                                partialAddressObj.Set(arrayAddressObj.Address);
                                int devAPartialddress = (int)(arrayAddressObj.StartAddress + elementIndex);

                                if (!partialAddressObj.Set(devAPartialddress))
                                {
                                    arrayDim = 0;
                                    continue;
                                }

                                if (partialArray > 1)
                                {
                                    varArrayName = varName + "_" + (partialIndex + 1).ToString() + "_of_" + partialArray.ToString();
                                }
                                else
                                    varArrayName = varName;


                                p.StationName = stationName;
                                if (!p.ParseAddress(partialAddressObj.Address))
                                {
                                    arrayDim = 0;
                                    continue;
                                }
                                dynamicaddress = p.ToString();
                                ImportData impData = importDataModel.addImportData();
                                impData.Name = varArrayName;
                                impData.Address = partialAddressObj.GetNewAddress(devAPartialddress);
                                impData.DynAddress = dynamicaddress;
                                ((ImportDataMelsecQ)impData).TagTypeInt = MovType;
                                impData.Select = false;
                                impData.szType = s[TheStringToBeDataType].Trim();
                                impData.Description = s[TheStringToBeDescripion].Trim();
                                impData.ArrayDimension = (uint)(elementIndex + partialArrayDim < arrayDim ? partialArrayDim : arrayDim - elementIndex);
                                impData.parentId = -1;
                                impData.Id = m_lGlobalID++;
                                arrayId = impData.Id;
                                arrayParent = impData;
                                if(MovType != (int)UFUAModel.DataType.String)
                                {
                                    AddTreeItem(impData);
                                }
                                


                                for (uint index = 0; index < partialArrayDim && elementIndex < arrayDim; index++)
                                {
                                    variableAddressObj.Set(arrayAddressObj.Address);
                                    int devAddress = arrayAddressObj.StartAddress;
                                    MovType = GetMoviconTypeId(s[TheStringToBeDataType].Trim(), ref varsize);

                                    if ((MovType == (int)UFUAModel.DataType.UInt64) ||
                                        (MovType == (int)UFUAModel.DataType.Int64) ||
                                        (MovType == (int)UFUAModel.DataType.Double))
                                    {
                                        devAddress += 4 * arrayIndex;
                                    }
                                    else if (MovType == (int)UFUAModel.DataType.String)
                                    {
                                        if(s[TheStringToBeDataType].Contains("WSTRING"))
                                        {
                                            devAddress += (((int)varsize + 1) * arrayIndex);
                                        }
                                        else
                                        {
                                            devAddress += (((int)varsize / 2 + 1) * arrayIndex);
                                        }
                                    }
                                    else if ((MovType != (int)UFUAModel.DataType.UInt32) &&
                                             (MovType != (int)UFUAModel.DataType.Int32) &&
                                             (MovType != (int)UFUAModel.DataType.Float))
                                    {
                                        devAddress += arrayIndex;
                                    }
                                    else
                                    {
                                        devAddress += 2 * arrayIndex;
                                    }

                                    if (!variableAddressObj.Set(devAddress))
                                    {
                                        continue;
                                    }
                                    arrayElement = true;
                                    arrayIndex++;

                                    dynamicaddress = String.Empty;
                                    p.StationName = stationName;
                                    if (!p.ParseAddress(variableAddressObj.Address))
                                    {
                                        continue;
                                    }
                                    dynamicaddress = p.ToString();
                                    //MovType = GetMoviconTypeId(s[5].Trim(), ref varsize);
                                    if (MovType != -1)
                                    {
                                        ImportData impDataElement = importDataModel.addImportData();
                                        String tmpNameVar = varName + "_" + elementIndex.ToString();
                                        impDataElement.Name = tmpNameVar;
                                        impDataElement.Address = variableAddressObj.GetNewAddress(devAddress);
                                        impDataElement.DynAddress = dynamicaddress;
                                        ((ImportDataMelsecQ)impDataElement).TagTypeInt = MovType;
                                        impDataElement.Select = false;
                                        impDataElement.szType = arrayElementType;
                                        impDataElement.ArrayDimension = 0;
                                        impDataElement.Id = m_lGlobalID++;
                                        if (MovType != (int)UFUAModel.DataType.String)
                                        {
                                            impDataElement.parentId = arrayId;
                                            AddTreeItem(impDataElement, arrayParent);
                                        }
                                        else
                                        {
                                            impDataElement.parentId = -1;
                                            AddTreeItem(impDataElement);
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
                    variableAddressObj.Set(s[TheStringToBeMemoryArea].Trim());               
                    
                    if (!variableAddressObj.IsValid)
                    {                       
                        continue;          
                    }
                   

                    // Local variable? --> Add the POU name to the variable name
                    if (importVarType == MelsecQEthImportVarType.ImportVarType_VAR_LOCAL)
                    {
                        auxString = pouName + "_" + varName;
                        varName = auxString;
                    }

                    dynamicaddress = String.Empty;
                    p.StationName = stationName;
                    if (!p.ParseAddress(variableAddressObj.Address))
                    {
                        continue;
                    }
                    dynamicaddress = p.ToString();
                    MovType = GetMoviconTypeId(s[TheStringToBeDataType].Trim(), ref varsize); //TODO
                    if (MovType != -1)
                    {
                        ImportData impData = importDataModel.addImportData();
                        impData.Name = varName;
                        impData.Address = variableAddressObj.Address;
                        impData.DynAddress = dynamicaddress;
                        ((ImportDataMelsecQ)impData).TagTypeInt = MovType;
                        impData.Select = false;
                        impData.szType =  s[TheStringToBeDataType].Trim() ;
                        impData.Description = s[TheStringToBeDescripion].Trim();
                        impData.ArrayDimension = 0;
                        impData.Id = m_lGlobalID++;

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
                        if ((readFile.CurrentEncoding.EncodingName == System.Text.Encoding.UTF8.EncodingName )||
                            (readFile.CurrentEncoding.EncodingName == System.Text.Encoding.ASCII.EncodingName ))
                        {
                            tmpByte = (int) MelsecQEthFileType.Type_ASCII;
                        } 
                        else if (readFile.CurrentEncoding.EncodingName == System.Text.Encoding.Unicode.EncodingName)
                        {
                            tmpByte = (int) MelsecQEthFileType.Type_UNICODE_16;
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

        public int GetMoviconTypeId(string Type, ref uint VarSize)
        {
            int nType = -1;
            VarSize = 0;
            Type.Trim();
            if (Type.Length == 0)
                return (nType);
            Type = Type.ToUpper();
            if (Type.Contains("BIT"))
            {
                nType = (int)UFUAModel.DataType.Boolean;
                VarSize = 1;
            }
            else if (Type.Contains("BOOL"))
            {
                nType = (int)UFUAModel.DataType.Boolean;
                VarSize = 1;
            }
            else if (Type.Contains("BYTE"))
            {
                nType = (int)UFUAModel.DataType.Byte;
                VarSize = 1;
            }
            //Modified version 2.2.30.0 (FOGBUGZ 15497)
            //Contains Returns a value indicating whether a specified substring occurs within this string, 
            //so the string "DWORD" must be checked before the string "WORD".
            else if (Type.Contains("DWORD")) 
            {
                nType = (int)UFUAModel.DataType.UInt32;
                VarSize = 4;
            }
            else if (Type.Contains("WORD"))
            {
                nType = (int)UFUAModel.DataType.UInt16;
                VarSize = 2;
            }
            //Modified version 2.2.30.0 (FOGBUGZ 15497)
            //Contains Returns a value indicating whether a specified substring occurs within this string, 
            //so the string "DINT" mast be checked before the string "INT". 
            else if (Type.Contains("DINT"))
            {
                nType = (int)UFUAModel.DataType.Int32;
                VarSize = 4;
            }
            else if (Type.Contains("INT"))
            {
                nType = (int)UFUAModel.DataType.Int16;
                VarSize = 2;
            }
            else if (Type.Contains("LREAL"))
            {
                nType = (int)UFUAModel.DataType.Double;
                VarSize = 8;
            }
            else if (Type.Contains("REAL"))
            {
                nType = (int)UFUAModel.DataType.Float;
                VarSize = 4;
            }
            else if (Type.Contains("CHAR"))
            {
                nType = (int)UFUAModel.DataType.SByte;
                VarSize = 1;
            }
            else if (Type.Contains("WSTRING"))
            {
                nType = (int)UFUAModel.DataType.String;
                VarSize = GetStringLength(Type);
                if (VarSize == 0)
                {
                    nType = -1;
                }
            }
            else if (Type.Contains("STRING"))
            {
                nType = (int)UFUAModel.DataType.String;
                VarSize = GetStringLength(Type);
                if(VarSize == 0)
                {
                    nType = -1;
                }
            }
            return (nType);

        }

        uint GetStringLength(string Type)
        {
            var start = Type.LastIndexOf("[") +1 ;
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
    
    public class ImportDataMelsecQ : ImportData, IDisposable
    {
        public ImportDataMelsecQ(ImportDataModel inDataModel) :
            base(inDataModel)
        {
            dataModelMelsecQ = inDataModel as ImportDataModelMelsecQ;
        }
        private ImportDataModelMelsecQ dataModelMelsecQ { get; set; }

        private int _TagTypeInt;
        public int TagTypeInt
        {
            get { return _TagTypeInt; }
            set { _TagTypeInt = value; }
        }

        //public string TreeName
        //{
        //    get { return getTreeName(); }
        //}

        //private string getTreeName()
        //{
        //    string treeName = Name;
        //    if (Parent != null)
        //    {
        //        treeName = Parent.getTreeName() + "." + treeName;
        //    }
        //    return treeName;
        //}

        public override DataType IconTagType
        {
            get
            {
                if (_TagTypeInt == -1)
                    return DataType.Boolean;
                else
                    return (DataType)_TagTypeInt;
            }
        }

        #region IDisposable Members
        public void Dispose()
        {
            if (Children != null && Children.Count > 0)
            {
                foreach (ImportData el in Children)
                {
                    var sl = el as ImportDataMelsecQ;
                    if (sl != null)
                    {
                        sl.Dispose();
                    }
                }
                Children.Clear();
            }
        }
        #endregion
    }

    public class ImportDataModelMelsecQ : ImportDataModel, IDisposable
    {
        public ImportDataModelMelsecQ(GetStationName inGetStationName) :
            base(inGetStationName)
        {
        }
        public override ImportData addImportData()
        {
            ImportDataMelsecQ importData = new ImportDataMelsecQ(this);
            return importData;
        }

        #region IDisposable Members
        public void Dispose()
        {
            foreach (ImportData el in Children)
            {
                ImportDataMelsecQ els7 = el as ImportDataMelsecQ;
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
