using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Utilities;
using System.Reflection;
using UFUAModel;
using System.Collections.ObjectModel;
using DriverCodeBaseEx.UI;
using DriverCodeBaseEx.UI.Controls;


namespace MelsecFX.UI
{
    public enum MelsecFXImportVarType : int
    {
        ImportVarType_Invalid,
        ImportVarType_VAR_GLOBAL,
        ImportVarType_VAR_LOCAL
    }

    /// <summary>
    /// Interaction logic for ImportTagsEditorTree.xaml
    /// </summary>
    public partial class ImportTagsEditorTree : UserControl, IDisposable
    {
        ImportDataModelMelsecFX importDataModel;
        bool alreadyLoaded = false;
        int m_lGlobalID = 0;
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
                using (ImportDataProgressBar progressBar = new ImportDataProgressBar())
                {
                    // Set the number of tags that will be imported
                    progressBar.StartImport(list.Count);
                    try
                    {
                        List<ImportTag> taglist = new List<ImportTag>();
                        List<ImportPrototype> protolist = new List<ImportPrototype>();
                        ObservableCollection<object> notToBeImported = new ObservableCollection<object>();
                        foreach (var elem in list)
                        {
                            progressBar.ImportNewTag(elem.Name);
                            progressBar.ThrowIfCancellationRequested();

                            ImportDataMelsecFX single = elem as ImportDataMelsecFX;
                            if (single != null && (notToBeImported.IndexOf(single) < 0))
                            {                        
                                bool isArray = (single.ArrayDimension > 0) ? true : false;

                                MelsecFXDynTagSettings sp = new MelsecFXDynTagSettings();
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
                                    DataType = (DataType)((ImportDataMelsecFX)single).TagTypeInt,
                                    DynSettings = single.DynAddress,
                                    Folder = importfolder,
                                    ModelType = UFUAModel.ModelType.Variable,

                                    ArrayDimension = single.ArrayDimension,
                                    BehaviorForExistingTags = (BehaviorExistingTagsValues)behaviorExistingTags,
                                    BehaviorForDynamicLink = (BehaviorDynamicLinkValues)behaviorDynamicLink,
                                };

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
                    catch (OperationCanceledException ex)
                    {
                        //UFUAServerDocument.logGeneral.Info(Properties.Resources.TagsImportCanceled);
                    }
                }
            }
        }

        private MelsecFXImportVarType IsVarDeclaration(string rowField)
        {
            MelsecFXImportVarType importVarType = MelsecFXImportVarType.ImportVarType_Invalid;
            string varDeclaration = rowField.ToUpper();
            if (String.Equals(varDeclaration, "VAR_GLOBAL"))
            {
                importVarType = MelsecFXImportVarType.ImportVarType_VAR_GLOBAL;
            }

            else if (String.Equals(varDeclaration, "VAR_INPUT") ||
                     String.Equals(varDeclaration, "VAR_OUTPUT") ||
                     String.Equals(varDeclaration, "VAR_IN_OUT") ||
                     String.Equals(varDeclaration, "VAR"))
            {
                importVarType = MelsecFXImportVarType.ImportVarType_VAR_LOCAL;
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
            arrayDim = -1;
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
                int openBracketIndex = typeField.IndexOf('[');
                int doublePointIndex = typeField.IndexOf("..");
                int closeBracketIndex = typeField.IndexOf(']');
                int ofIndex = typeField.IndexOf(" OF ");
                if ((openBracketIndex < 0) ||
                    (doublePointIndex <= openBracketIndex) ||
                    (closeBracketIndex <= doublePointIndex) ||
                    (ofIndex <= closeBracketIndex) ||
                    (typeField.Length <= (ofIndex + 4)))
                {
                    return false;
                }
                string firstIndex = typeField.Substring(
                                      openBracketIndex + 1,
                                      doublePointIndex - openBracketIndex - 1);
                firstIndex.Trim();
                int arrayFirstIndex = Convert.ToInt32(firstIndex);
                string lastIndex = typeField.Substring(
                                     doublePointIndex + 2,
                                     closeBracketIndex - doublePointIndex - 2);
                lastIndex.Trim();
                int arrayLastIndex = Convert.ToInt32(lastIndex);
                if (arrayLastIndex <= arrayFirstIndex)
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

                // Set the array dimension
                arrayDim = arrayLastIndex - arrayFirstIndex + 1;
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
            string stationaNane = baseImportTree.ReadStationName();

            importDataModel = new ImportDataModelMelsecFX(readStationName);

            file = file.ToLower();
            if (file.Contains(".csv"))
            {
                try
                {
                    using (new WaitCursor())
                    {
                        List<string[]> parsedData = new List<string[]>();
                        using (System.IO.StreamReader readFile = new System.IO.StreamReader(file))
                        {
                            string line;
                            string[] row;

                            while ((line = readFile.ReadLine()) != null)
                            {
                                row = line.Split(';');
                                parsedData.Add(row);
                            }
                        }

                        MelsecFXDynTagSettings p = new MelsecFXDynTagSettings();
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
                        MelsecFXAddress arrayAddressObj = new MelsecFXAddress();
                        MelsecFXAddress variableAddressObj = new MelsecFXAddress();
                        string varType = String.Empty;
                        foreach (var s in parsedData)
                        {
                            if (s.Length == 0)
                            {
                                continue;
                            }

                            // Variable declaration?
                            MelsecFXImportVarType importVarType = IsVarDeclaration(s[0].Trim());
                            if (importVarType == MelsecFXImportVarType.ImportVarType_Invalid)
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
                                    p.StationName = stationaNane; // CmbStation.Text;
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
                                    ((ImportDataMelsecFX)impData).TagTypeInt = MovType;
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
                            if (importVarType == MelsecFXImportVarType.ImportVarType_VAR_LOCAL)
                            {
                                auxString = pouName + "_" + varName;
                                varName = auxString;
                            }

                            dynamicaddress = String.Empty;
                            p.StationName = stationaNane; // CmbStation.Text;
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
                                ((ImportDataMelsecFX)impData).TagTypeInt = MovType;
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

                    //ImportTree.Model = importDataModel;
                    //if (listViewSortCol != null)
                    //{
                    //    AdornerLayer.GetAdornerLayer(listViewSortCol).Remove(listViewSortAdorner);
                    //    ImportTree.Items.SortDescriptions.Clear();
                    //}
                }
                catch (Exception ex)
                {
                    string message;
                    message = String.Format(Properties.Resources.ImportExceptionFileLoading, file, ex.Message);
                    MessageBox.Show(message, Properties.Resources.ImportMsgBoxTitle);
                    //return;
                    importDataModel = null;
                }
                if (importDataModel == null || importDataModel.Children.Count == 0)
                {
                    MessageBox.Show(DriverCodeBaseEx.UI.Properties.Resources.ImportErrorEmptyFile,
                                    Properties.Resources.ImportMsgBoxTitle);
                }
            }

            return importDataModel;
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
            else if (Type.Contains("WORD"))
            {
                nType = (int)UFUAModel.DataType.UInt16;
                VarSize = 2;
            }
            else if (Type.Contains("DWORD"))
            {
                nType = (int)UFUAModel.DataType.UInt32;
                VarSize = 4;
            }
            else if (Type.Contains("INT"))
            {
                nType = (int)UFUAModel.DataType.Int16;
                VarSize = 2;
            }
            else if (Type.Contains("DINT"))
            {
                nType = (int)UFUAModel.DataType.Int32;
                VarSize = 4;
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
            return (nType);

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

    public class ImportDataMelsecFX : ImportData, IDisposable
    {
        public ImportDataMelsecFX(ImportDataModel inDataModel) :
            base(inDataModel)
        {
            dataModelMelsecFx = inDataModel as ImportDataModelMelsecFX;
        }
        private ImportDataModelMelsecFX dataModelMelsecFx { get; set; }
        
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
                    var sl = el as ImportDataMelsecFX;
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

    public class ImportDataModelMelsecFX : ImportDataModel, IDisposable
    {
        public ImportDataModelMelsecFX(GetStationName inGetStationName) :
            base(inGetStationName)
        {
        }
        public override ImportData addImportData()
        {
            ImportDataMelsecFX importData = new ImportDataMelsecFX(this);
            return importData;
        }

        #region IDisposable Members
        public void Dispose()
        {
            foreach (ImportData el in Children)
            {
                ImportDataMelsecFX els7 = el as ImportDataMelsecFX;
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
