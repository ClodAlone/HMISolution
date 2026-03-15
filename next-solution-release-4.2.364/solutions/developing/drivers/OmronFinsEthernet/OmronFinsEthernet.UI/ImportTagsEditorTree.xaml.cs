using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Reflection;
using UFUAModel;
using Utilities;
using DriverCodeBase.UI;
using DriverCodeBase.UI.Controls;


namespace OmronFinsEthernet.UI
{
    public partial class ImportTagsEditorTree : UserControl, IDisposable
    {
        ImportDataModelOmronFinsEthernet importDataModel;
        bool alreadyLoaded = false;
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
                    colWidth = 250
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
                foreach (var elemList in list)
                {
                    ImportData elem = elemList as ImportData;
                    if (elem != null)
                    {
                        bool isStructure = elem.Children.Count > 0;
                        ImportData single = elem;
                        OmronFinsEthernetDynTagSettings sp = new OmronFinsEthernetDynTagSettings();
                        if (!sp.ParseAddress(single.Address))
                            continue;
                        sp.StationName = stationName;
                        single.DynAddress = sp.ToString();
                        ImportTag tagtoimport = new ImportTag()
                        {
                            Name = single.Name,
                            DataType = (DataType)((ImportDataOmronFinsEthernet)single).TagTypeInt,
                            DynSettings = single.DynAddress,
                            Description = single.Description,
                            Folder = importfolder,
                            ModelType = UFUAModel.ModelType.Variable,
                            ArrayDimension = single.ArrayDimension
                        };

                        taglist.Add(tagtoimport);

                    }
                }
                DataContext = new ImportObject() { PrototypesToImport = protolist, TagsToImport = taglist };
            }
            else
                DataContext = this;
        }

        private ImportDataModel LoadFile(string file)
        {
            string stationName = baseImportTree.ReadStationName();
            importDataModel = new ImportDataModelOmronFinsEthernet(readStationName);           

            file = file.ToLower();
            //if (file.Contains(".csv"))
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
                            row = line.Split('\t');
                            parsedData.Add(row);
                        }
                    }

                    bool refresh = false;
                    OmronFinsEthernetDynTagSettings p = new OmronFinsEthernetDynTagSettings();
                    string dynamicaddress = String.Empty;
                    int MovType = 0;
                    uint varsize = 0;
                    string strSymb = String.Empty;
                    string strAddr = String.Empty;
                    string strDesc = String.Empty;
                    string szType = String.Empty;
                    OmronAddress variableAddressObj = new OmronAddress();

                    foreach (var s in parsedData)
                    {
                        if (s.Length < 3)
                        {
                            continue;
                        }
                        else
                        {
                            strSymb = s[0];
                            szType = s[1];
                            strAddr = s[2];
                        }

                        if (s.Length > 3)
                        {
                            strDesc = s[3];
                        }

                        DataConversionTypes ConversionType = DataConversionTypes.None;
                        uint stringOrArraySize = 0;
                        MovType = GetMoviconTypeId(szType, ref varsize, ref ConversionType, out stringOrArraySize);
                        if (MovType == -1)
                        {
                            continue;
                        }
                        else if (MovType == (int)UFUAModel.DataType.String)
                        {
                            if (stringOrArraySize > 0)
                            {
                                string stringSize = String.Format(":{0}", stringOrArraySize);
                                strAddr += stringSize;
                            }
                        }

                        variableAddressObj.Set(strAddr);
                        if (!variableAddressObj.IsValid)
                        {
                            String szAux = "CIO" + strAddr;
                            strAddr = szAux;
                            variableAddressObj.Set(strAddr);
                            if (!variableAddressObj.IsValid)
                                continue;
                        }

                        dynamicaddress = String.Empty;
                        p.StationName = stationName;
                        p.DataConversionType = ConversionType;
                        if (!p.ParseAddress(variableAddressObj.Get()))
                        {
                            continue;
                        }

                        dynamicaddress = p.ToString();

                        if (strSymb == String.Empty)
                        {
                            strSymb = strAddr;
                            strSymb.Replace(':', '_');
                            strSymb.Replace('.', '_');
                        }

                        refresh = true;
                        var IVar = importDataModel.addImportData();
                        IVar.Name = strSymb;
                        IVar.Address = variableAddressObj.Get();
                        IVar.DynAddress = dynamicaddress;
                        ((ImportDataOmronFinsEthernet)IVar).TagTypeInt = MovType;
                        IVar.Select = false;
                        IVar.Description = strDesc;
                        IVar.szType = szType.Trim();
                        if (MovType != (int)UFUAModel.DataType.String)
                        {
                            IVar.ArrayDimension = stringOrArraySize;
                        }
                        else
                        {
                            IVar.ArrayDimension = 0;
                        }
                        AddTreeItem(IVar);

                        // Add to the tree also the elements of an array
                        if (IVar.ArrayDimension > 0)
                        {
                            string szElementName = String.Empty;
                            int indexOfOpenBracket = IVar.szType.IndexOf("[");
                            string elementType = String.Empty;
                            if (indexOfOpenBracket > 0)
                            {
                                elementType = IVar.szType.Substring(0, indexOfOpenBracket);
                            }
                            else
                            {
                                elementType = IVar.szType;
                            }
                            uint addressIncrement = varsize / 2;
                            if (addressIncrement < 1)
                            {
                                addressIncrement = 1;
                            }
                            uint i = 0;
                            for (i = 0; i < IVar.ArrayDimension; i++)
                            {
                                ImportData f = importDataModel.addImportData();
                                szElementName = String.Format("{0}[{1}]", IVar.Name, i);
                                f.Name = szElementName;
                                OmronAddress elementAddressObject = new OmronAddress();
                                elementAddressObject.Set(variableAddressObj.Get());
                                if (((ImportDataOmronFinsEthernet)IVar).TagTypeInt != (int)UFUAModel.DataType.Boolean)
                                {
                                    elementAddressObject.Address = new ushortUnion((ushort)(variableAddressObj.Address.USHORT + i * addressIncrement));
                                }
                                else
                                {
                                    elementAddressObject.BitNumber = (sbyte)((variableAddressObj.BitNumber + i) % 16);
                                    elementAddressObject.Address = new ushortUnion((ushort)(variableAddressObj.Address.USHORT + (variableAddressObj.BitNumber + i) / 16));
                                    elementAddressObject.DataFormat = DataFormats.Bit;
                                }
                                f.Address = elementAddressObject.Get();
                                if (!p.ParseAddress(f.Address))
                                {
                                    continue;
                                }
                                f.DynAddress = p.ToString();
                                ((ImportDataOmronFinsEthernet)f).TagTypeInt = MovType;
                                f.Select = false;
                                f.Description = String.Empty;
                                f.szType = elementType;
                                f.ArrayDimension = 0;
                                AddTreeItem(f, IVar);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    string message;
                    message = String.Format(Properties.Resources.ImportExceptionFileLoading, file, ex.Message);
                    MessageBox.Show(message, Properties.Resources.ImportMsgBoxTitle);
                    //return;
                    importDataModel = null;
                }
            }

            if (importDataModel == null || importDataModel.Children.Count == 0)
            {
                MessageBox.Show(DriverCodeBase.UI.Properties.Resources.ImportErrorEmptyFile,
                                Properties.Resources.ImportMsgBoxTitle);
            }

            return importDataModel;
        }

         public int GetMoviconTypeId(string Type, ref uint VarSize, ref DataConversionTypes ConversionType, out uint stringOrArraySize)
         {
             int nType = -1;
             VarSize = 0;
             stringOrArraySize = 0;
             ConversionType = DataConversionTypes.None;

             Type.Trim();
             if (Type.Length == 0)
                 return (nType);
             Type = Type.ToUpper();

             string typeString = String.Empty;
             uint numOfElements = 0;
             int indexOpenSquareBracket = Type.IndexOf("[");
             int indexCloseSquareBracket = Type.IndexOf("]");
             if((indexOpenSquareBracket > 0) && (indexCloseSquareBracket > (indexOpenSquareBracket + 1)))
             {
                string numOfElementsString = Type.Substring(indexOpenSquareBracket + 1, indexCloseSquareBracket - indexOpenSquareBracket - 1);
                if (uint.TryParse(numOfElementsString, out numOfElements) == false)
                {
                    numOfElements = 0;
                }
                typeString = Type.Substring(0, indexOpenSquareBracket);
             }
             else
             {
                typeString = Type;
             }

             if (typeString == "CHANNEL")
             {
                 nType = (int)DataType.UInt16;
                 VarSize = 2;
             }
             else if (typeString == "BOOL")
             {
                 nType = (int)UFUAModel.DataType.Boolean;
                 VarSize = 1;
             }
             else if (typeString == "NUMBER")
             {
                 nType = (int)UFUAModel.DataType.UInt16;
                 VarSize = 2;
             }
             else if (typeString == "WORD")
             {
                 nType = (int)UFUAModel.DataType.UInt16;
                 VarSize = 2;
             }
             else if (typeString == "DWORD")
             {
                 nType = (int)UFUAModel.DataType.UInt32;
                 VarSize = 4;
             }
             else if (typeString == "INT")
             {
                 nType = (int)UFUAModel.DataType.Int16;
                 VarSize = 2;
             }
             else if (typeString == "DINT")
             {
                 nType = (int)UFUAModel.DataType.Int32;
                 VarSize = 4;
             }
             else if (typeString == "REAL")
             {
                 nType = (int)UFUAModel.DataType.Float;
                 VarSize = 4;
             }
             else if (typeString == "LREAL")
             {
                 nType = (int)UFUAModel.DataType.Double;
                 VarSize = 8;
             }
             else if (typeString == "UDINT")
             {
                 nType = (int)UFUAModel.DataType.UInt32;
                 VarSize = 4;
             }
             else if (typeString == "UDINT_BCD")
             {
                 nType = (int)UFUAModel.DataType.UInt32;
                 VarSize = 4;
                 ConversionType = DataConversionTypes.BCD32Bits;
             }
             else if (typeString == "UINT")
             {
                 nType = (int)UFUAModel.DataType.UInt16;
                 VarSize = 2;
             }
             else if (typeString == "UINT_BCD")
             {
                 nType = (int)UFUAModel.DataType.UInt16;
                 VarSize = 2;
                 ConversionType = DataConversionTypes.BCD16Bits;
             }
             else if(typeString == "STRING")
             {
                nType = (int)UFUAModel.DataType.String;
                VarSize = 2;
             }

             if(nType != -1)
             {
                if (numOfElements > 0)
                {
                    stringOrArraySize = numOfElements;
                }
             }

             return (nType);
         }

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

    public class ImportDataOmronFinsEthernet : ImportData, IDisposable
    {
        public ImportDataOmronFinsEthernet(ImportDataModel inDataModel) :
            base(inDataModel)
        {
            dataModelOmronFinsEthernet = inDataModel as ImportDataModelOmronFinsEthernet;
        }
        private ImportDataModelOmronFinsEthernet dataModelOmronFinsEthernet { get; set; }
      
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
                    var sl = el as ImportDataOmronFinsEthernet;
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
    public class ImportDataModelOmronFinsEthernet : ImportDataModel, IDisposable
    {
        public ImportDataModelOmronFinsEthernet(GetStationName inGetStationName) :
            base(inGetStationName)
        {
        }
        public override ImportData addImportData()
        {
            ImportDataOmronFinsEthernet importData = new ImportDataOmronFinsEthernet(this);
            return importData;
        }

        #region IDisposable Members
        public void Dispose()
        {
            foreach (ImportData el in Children)
            {
                ImportDataOmronFinsEthernet els7 = el as ImportDataOmronFinsEthernet;
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
