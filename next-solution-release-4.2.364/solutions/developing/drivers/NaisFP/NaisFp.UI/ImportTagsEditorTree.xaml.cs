using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Reflection;
using UFUAModel;
using System.Text.RegularExpressions;
using System.Collections.ObjectModel;
using Utilities;
using DriverCodeBase.UI;
using DriverCodeBase.UI.Controls;

namespace NaisFp.UI
{
    /// <summary>
    /// Interaction logic for ImportTagsEditorTree.xaml
    /// </summary>
    public partial class ImportTagsEditorTree : UserControl, IDisposable
    {
        ImportDataModelNaisFP importDataModel;
        bool alreadyLoaded = false;        
        BaseImportTree baseImportTree;
        public GetStationName readStationName;
        int m_lGlobalID = 0;

        #region static const

        public const int GlobField = 0;
        public const int NameField = 1;
        public const int AddrIECField = 2;
        public const int AddrField = 3;
        public const int TypeField = 4;
        public const int InitField = 5;
        public const int DescField = 6;
        public const int TotalCsvFields = 7;

        #endregion

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
                    colWidth = 150
                });

                baseImportTree = new BaseImportTree(DriverName, lista[0],
                   (lista[1].ToLower().IndexOf("true") != -1), columns);
                baseImportTree.SetVisibleButtons();
                baseImportTree.SetFileFilter("csv files|*.csv|All files|*.*");
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

            if (list.Count > 0)
            {
                List<ImportTag> taglist = new List<ImportTag>();
                List<ImportPrototype> protolist = new List<ImportPrototype>();
                ObservableCollection<object> notToBeImported = new ObservableCollection<object>();
                foreach (var elemList in list)
                {
                    ImportData elem = elemList as ImportData;
                    if (elem != null && (notToBeImported.IndexOf(elem) < 0))
                    {
                        ImportDataNaisFP single = elem as ImportDataNaisFP;

                        bool isArray = (single.ArrayDimension > 0) ? true: false;

                        NaisFpDynTagSettings sp = new NaisFpDynTagSettings();
                        if (!sp.ParseAddress(single.Address))
                            continue;
                        sp.StationName = stationName;
                        single.DynAddress = sp.ToString();

                        sp.ArrayDimension = single.ArrayDimension;

                        string importTagName = single.Name;
                        //importTagName = UFUAModel.Helpers.NameValidator.EnsureValidName(importTagName);

                        ImportTag tagtoimport = new ImportTag()
                        {
                            Name = single.Name,
                            DataType = (DataType)single.TagTypeInt,
                            DynSettings = single.DynAddress,
                            Description = single.Description,
                            Folder = importfolder,
                            ModelType = UFUAModel.ModelType.Variable,

                            ArrayDimension = single.ArrayDimension
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
            else
                DataContext = this;
        }

        private bool IsArrayDeclaration(string TypeField,
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

                // Parse the array dimension and type
                Regex TagNameParser = new Regex(@"^\s*ARRAY\s*\[\s*(?<InitIndex>\d+)\s*..\s*(?<EndIndex>\d+)\s*\]\s*OF\s+(?<ElementType>\w+)?\s*$");
                Match TagNameMatch = TagNameParser.Match(TypeField);
                if (!TagNameMatch.Success)
                {
                    arrayDim = 0;
                    return false;
                }

                // Test the array dimension
                int InitIndex = Convert.ToInt32(TagNameMatch.Groups["InitIndex"].Value);
                int EndIndex = Convert.ToInt32(TagNameMatch.Groups["EndIndex"].Value);
                if (InitIndex > EndIndex)
                {
                    return false;
                }

                // Test the array type
                uint VarSize = 0;
                ElementType = TagNameMatch.Groups["ElementType"].Value;
                arrayElementMoviconType = GetMoviconTypeId(ElementType,
                                                           ref VarSize);
                if (arrayElementMoviconType == -1)
                {
                    ElementType = String.Empty;
                    return false;
                }

                // Set the array dimension
                arrayDim = EndIndex - InitIndex + 1;
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
            string stationName = baseImportTree.ReadStationName();

            importDataModel = new ImportDataModelNaisFP(readStationName);

            file = file.ToLower();
            if (file.Contains(".csv"))
            {
                try
                {
                    List<string[]> parsedData = new List<string[]>();
                    using (System.IO.StreamReader readFile = new System.IO.StreamReader(file))
                    {
                        string line;
                        string[] row;

                        while ((line = readFile.ReadLine()) != null)
                        {
                            if (!string.IsNullOrWhiteSpace(line))
                            {
                                row = line.Split(';');
                                if (row.Length >= TotalCsvFields)
                                {
                                    row[GlobField] = row[GlobField].ToUpper();
                                    row[TypeField] = row[TypeField].ToUpper();
                                    parsedData.Add(row);
                                }
                            }
                        }
                    }

                    NaisFpDynTagSettings p = new NaisFpDynTagSettings();
                    string dynamicaddress = String.Empty;
                    int MovType = 0;
                    uint varsize = 0;
                    int arrayDim = 0;
                    uint arrayElementSize = 0;
                    string arrayElementType = String.Empty;
                    int arrayIndex = 0;
                    string arrayAddress = String.Empty;
                    int arrayId = -1;
                    ImportData arrayParent = null;
                    bool arrayElement = false;
                    string varName = String.Empty;
                    string varDescr = String.Empty;
 
                    NaisFpAddress arrayAddressObj = new NaisFpAddress();
                    NaisFpAddress variableAddressObj = new NaisFpAddress();


                    foreach (var s in parsedData)
                    {

                        if (s.Length < TotalCsvFields)
                        {
                            continue;
                        }

                        if ((s[GlobField] != "VAR_GLOBAL" && s[GlobField] != "VAR_GLOBAL_RETAIN") ||
                            s[NameField].Length == 0 ||
                            s[AddrField].Length == 0 ||
                            s[TypeField].Length == 0 ||
                            s[TypeField].Contains("STRING") ||
                            s[TypeField].Contains("TIME") )
                        {
                            continue;
                        }

                        // Set the variable name
                        varName = s[NameField].Trim().Replace('.', '_');
                        if (varName == String.Empty)
                        {
                            continue;
                        }

                        Regex TagNameParser = new Regex(@"^\""(?<Description>.+?)?\""$");
                        Match TagNameMatch = TagNameParser.Match(s[DescField]);
                        if (TagNameMatch.Success)
                        {
                            varDescr = TagNameMatch.Groups["Description"].Value;
                        }

                        // Array?
                        if (arrayDim == 0)
                        {
                            arrayElement = false;
                            if (!IsArrayDeclaration(s[TypeField].Trim(),
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
                                arrayAddress = s[AddrField].Trim();
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
                                p.StationName = stationName; // CmbStation.Text;
                                if (!p.ParseAddress(arrayAddressObj.Get()))
                                {
                                    arrayDim = 0;
                                    continue;
                                }
                                dynamicaddress = p.ToString();

                                ImportData impData = importDataModel.addImportData();
                                impData.Name = varName;
                                impData.Address = arrayAddressObj.Get();
                                impData.DynAddress = dynamicaddress;
                                ((ImportDataNaisFP)impData).TagTypeInt = MovType;
                                impData.Select = false;
                                impData.szType = s[TypeField].Trim();
                                impData.ArrayDimension = (uint)arrayDim;
                                impData.parentId = -1;
                                impData.Id = m_lGlobalID++;
                                arrayId = impData.Id;
                                arrayParent = impData;
                                impData.Description = varDescr;

                                AddTreeItem(impData);
                                continue;
                            }
                        }

                        // Special case: element of a previously declared array
                        if (arrayDim > 0)
                        {
                            variableAddressObj.Set(arrayAddressObj.Get());
                            if (!variableAddressObj.SetElemArrayAddress(MovType, arrayIndex))
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
                            variableAddressObj.Set(s[AddrField].Trim());
                            if (!variableAddressObj.IsValid)
                            {
                                continue;
                            }
                        }

                        dynamicaddress = String.Empty;
                        p.StationName = stationName;
                        if (!p.ParseAddress(variableAddressObj.Get()))
                        {
                            continue;
                        }

                        
                        dynamicaddress = p.ToString();
                        MovType = GetMoviconTypeId(s[TypeField].Trim(), ref varsize);
                        if (MovType != -1)
                        {
                            ImportData impData = importDataModel.addImportData();
                            impData.Name = varName;
                            impData.Address = variableAddressObj.Get();
                            impData.DynAddress = dynamicaddress;
                            ((ImportDataNaisFP)impData).TagTypeInt = MovType;
                            impData.Select = false;
                            impData.szType = s[TypeField].Trim();
                            impData.ArrayDimension = 0;
                            impData.Id = m_lGlobalID++;
                            impData.Description = varDescr;
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

                    //ImportTree.Model = importDataModel;

                    //if (listViewSortCol != null)
                    //{
                    //    AdornerLayer.GetAdornerLayer(listViewSortCol).Remove(listViewSortAdorner);
                    //    ImportTree.Items.SortDescriptions.Clear();
                    //}

                }
                catch (Exception ex)
                {
                    //ImportTree.Items.Clear();
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

        void SetNewAddr(ref string strAddr, int i)
        {
            NaisFpAddress na = new NaisFpAddress();
            na.Set(strAddr);
            if (na.isBit())
            {
                uint Word = (uint)(na.StartAddress.USHORT + (na.BitNumber + i) / 0x10);
                uint Bit = (uint)((na.BitNumber + i) % 0x10);
                strAddr = string.Format("{0}{1:D3}{2:X1}",na.MemoryArea.ToString(), Word, Bit);

            }
            else
            {
                strAddr = string.Format("{0}{1:D}", na.MemoryArea.ToString(), na.StartAddress.USHORT + i * (na.Size() / 2));
            }

        }

         public int GetMoviconTypeId(string Type, ref uint VarSize)
         {
             int nType = -1;
             VarSize = 0;

             Type.Trim();
             if (Type.Length == 0)
                 return (nType);
             Type = Type.ToUpper();

             if (Type.Contains("BOOL"))
             {
                 nType = (int)UFUAModel.DataType.Boolean;
                 VarSize = 1;
             }
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
             //else if (Type.Contains("TIME"))
             //{
             //    nType = (int)UFUAModel.DataType.UInt16;
             //    VarSize = 2;
             //}
             //else if (Type.Contains("STRING"))
             //{
             //    nType = (int)UFUAModel.DataType.UInt16;
             //    VarSize = 2;
             //}
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

    public class ImportDataNaisFP : ImportData, IDisposable
    {
        public ImportDataNaisFP(ImportDataModel inDataModel) :
            base(inDataModel)
        {
            dataModelNaisFP = inDataModel as ImportDataModelNaisFP;
        }
        private ImportDataModelNaisFP dataModelNaisFP { get; set; }

       
        //private string _Name;
        //public string Name
        //{
        //    get
        //    {
        //        return dataModel.getStationName() != "_" ?
        //               dataModel.getStationName() + _Name : _Name;
        //    }
        //    set { _Name = value; }
        //}
       
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
                    var sl = el as ImportDataNaisFP;
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

    public class ImportDataModelNaisFP : ImportDataModel, IDisposable
    {
        public ImportDataModelNaisFP(GetStationName inGetStationName) :
            base(inGetStationName)
        {
        }
        public override ImportData addImportData()
        {
            ImportDataNaisFP importData = new ImportDataNaisFP(this);
            return importData;
        }

        #region IDisposable Members
        public void Dispose()
        {
            foreach (ImportData el in Children)
            {
                ImportDataNaisFP els7 = el as ImportDataNaisFP;
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
