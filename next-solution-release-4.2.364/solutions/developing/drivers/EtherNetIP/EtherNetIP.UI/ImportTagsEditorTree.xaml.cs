using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Utilities;
using System.Reflection;
using UFUAModel;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using System.IO;
using System.Data;
using ExcelDataReader;
using log4net;
using DriverCodeBase.UI;
using DriverCodeBase.UI.Controls;


namespace EtherNetIP.UI
{
    public enum ImportTypes
    {
        Unknown,
        Standard,
        Array,
        Struct,
        ArrayOfStructures,
    }
    public enum XlsFieldIndex
    {
        Name ,
        DataType,
        Dimension,
        StringSize,
    }

    public enum CsvOldPlcFieldIndex : int
    {
        Address = 0,
        NotUsed,
        Symbol,        
    }

    /// <summary>
    /// Interaction logic for ImportTagsEditorTree.xaml
    /// </summary>
    public partial class ImportTagsEditorTree : UserControl//, IDisposable
    {
        ImportDataModelEthernetIP importDataModel;
        string conn;
        bool alreadyLoaded = false;        
        int m_lGlobalID = 0;
        Dictionary<string, List<string>> m_mapStruct;
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
                    colWidth = 150
                });

                conn = lista[0];
                baseImportTree = new BaseImportTree(DriverName, lista[0], (lista[1].ToLower().IndexOf("true") != -1), columns);                
                // supported file type and import from PLC will be update in CmbStationChanged
                baseImportTree.SetFileFilter("*.csv");
                baseImportTree.SetVisibleButtons(bGetPLCTags: true);
                baseImportTree.SetButtonEnable(eImportButtons.eiBtnLoadFromDevice, false);
                baseImportTree.LoadImportFile = LoadFile;
                baseImportTree.GetPlcTags = Read_Plc_Info;
                baseImportTree.StationChanged += CmbStationChanged;
                baseImportTree.CreateItemControl = CreateImportDataTreeItemControl;

                MainStack.Children.Add(baseImportTree);

                readStationName = baseImportTree.funcGetStationName();

                DataContext = this;

                m_mapStruct = new Dictionary<string, List<string>>();

                // force to update some internal parameters
                CmbStationChanged();
            };
        }
        

        private void CmbStationChanged(object sender, RoutedEventArgs e)
        {
            CmbStationChanged();
        }

        private void CmbStationChanged()
        {
            if (!baseImportTree.HasStation(baseImportTree.ReadStationName()))
                return;

            EtherNetIPStation station = new EtherNetIPStation(new EtherNetIPDriver(conn), (EtherNetIPStationSettings)baseImportTree.GetStationSettings(baseImportTree.ReadStationName()));
            if (station.PlcType == PlcTypes.ControlLogix_CompactLogix)
                baseImportTree.SetButtonEnable(eImportButtons.eiBtnLoadFromDevice, true);
            else
                baseImportTree.SetButtonEnable(eImportButtons.eiBtnLoadFromDevice, false);

            // set support file type from selected station --> plc type
            string filter = string.Empty;
            if (station.PlcType == PlcTypes.Micro800_series)
                filter = "xls files|*.xls;*.xlsx|csv files|*.csv";
            else
                filter = "L5K files|*.L5K";

            // enable support of csv files for old plc series
            if (EtherNetIpProtocol.IsOldPlcModel(station.PlcType) && !filter.Contains(".csv"))
                filter += "|csv files|*.csv";

            baseImportTree.SetFileFilter(filter);            
        }    
      
        public void ImportSelectedTags()
        {
            List<ImportData> list = new List<ImportData>();
            list = baseImportTree.GetSelectedTags();
            string importfolder = baseImportTree.ReadFolderName();
            string stationName = baseImportTree.ReadStationName();

            if (list.Count > 0)
            {
                //string importfolder;
                //if (string.IsNullOrEmpty(ImportFolderText.Text))
                //{
                //    importfolder = CmbStation.Text;
                //}
                //else
                //{
                //    importfolder = ImportFolderText.Text;
                //}
                List<ImportTag> taglist = new List<ImportTag>();
                Dictionary<string, ImportPrototype> protoMap = new Dictionary<string, ImportPrototype>();
                ObservableCollection<object> notToBeImported = new ObservableCollection<object>();

                foreach (var elem in list)
                {
                    if (elem != null && (notToBeImported.IndexOf(elem) < 0))
                    {
                        ImportDataEthernetIP single = elem as ImportDataEthernetIP;

                        bool isArray = false;
                        bool isStructure = false;

                        switch (single.ImportType)
                        {
                            case ImportTypes.Struct:
                                isStructure = true;
                                break;
                            case ImportTypes.Array:
                                isArray = true;
                                break;
                        }

                        EtherNetIPDynTagSettings sp = new EtherNetIPDynTagSettings();
                        sp.AddressType = single.AddressMode;
                        if (!isStructure)
                        {
                            //sp.TagFormat = (TagFormats)Enum.Parse(typeof(TagFormats), single.ElemType);
                            TagFormats outTagFormat;
                            if (!GetTagFormat(single.ElemType, out outTagFormat))
                            {
                                log.ErrorFormat(Properties.Resources.ErrorVariableDataTypeUnsupported, single.Address, single.ElemType);
                                continue;
                            }
                            sp.TagFormat = outTagFormat;


                            if (sp.TagFormat == TagFormats.BOOL && isArray == true)
                                sp.TagFormat = TagFormats.ARRAYOF32BITS;
                        }
                        else
                            sp.TagFormat = TagFormats.STRUCTURE;

                        if (sp.TagFormat == TagFormats.STRING)
                        {
                            uint stringSize = isArray ? single.Size / single.ArrayDimension : single.Size;
                            EtherNetIPStation station = new EtherNetIPStation(new EtherNetIPDriver(conn), (EtherNetIPStationSettings)baseImportTree.GetStationSettings(stationName));
                            if (station.PlcType == PlcTypes.Micro800_series)
                                stringSize += 2;

                            if (station.PlcType == PlcTypes.SLC500_MicroLogix)
                            {
                                stringSize = 0;
                            }
                            else if (stringSize > 2)
                            {
                                single.Address += (":" + (stringSize).ToString());
                            }
                            else
                            {
                                continue;
                            }

                        }

                        if (!sp.ParseAddress(single.Address))
                            continue;

                        sp.StationName = stationName;

                        // FOGBUGZ 11412 and 11413
                        sp.ArrayDimension = single.ArrayDimension;

                        single.DynAddress = sp.ToString();

                        ImportPrototype proto = null;
                        if (isStructure)
                        {

                            proto = addPrototype(protoMap, single);
                            if (proto == null)
                                continue;
                        }

                        // FOGBUGZ 11412 and 11413
                        string importTagName = (single.PreName + single.Name).Trim(']');
                        //importTagName = UFUAModel.Helpers.NameValidator.EnsureValidName(importTagName);

                        ImportTag tagtoimport = new ImportTag()
                        {
                            // FOGBUGZ 11412 and 11413
                            //Name = single.Name,
                            Name = importTagName,

                            DataType = single.Type,
                            DynSettings = single.DynAddress,
                            Folder = importfolder,
                            ModelType = (isStructure ? UFUAModel.ModelType.ObjectType : UFUAModel.ModelType.Variable),
                            Description = single.Description,
                            ArrayDimension = single.ArrayDimension
                        };
                        if (isStructure)
                        {
                            tagtoimport.PrototypeModel = proto.Name;
                            tagtoimport.ModelType = UFUAModel.ModelType.ObjectType;
                        }

                        taglist.Add(tagtoimport);

                        // FOGBUGZ 11412 and 11413
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

                DataContext = new ImportObject() { PrototypesToImport = protoMap.Values.ToList(), TagsToImport = taglist };
            }
            else
                DataContext = this;
        }

        static readonly ILog log = log4net.LogManager.GetLogger(Properties.Resources.AreaFileLog);
        bool GetTagFormat(string inTagFormat, out TagFormats outTagFormat)
        {
            bool bRet = true;
            outTagFormat = TagFormats.BOOL;
            string inTagFormatUpper;
            inTagFormatUpper = inTagFormat.ToUpper();
            switch (inTagFormatUpper)
            {
                case "BOOL":
                case "BIT":
                    outTagFormat = TagFormats.BOOL;
                    break;

                case "SINT":
                    outTagFormat = TagFormats.SINT;
                    break;

                case "INT":
                    outTagFormat = TagFormats.INT;
                    break;

                case "DINT":
                    outTagFormat = TagFormats.DINT;
                    break;

                case "REAL":
                    outTagFormat = TagFormats.REAL;
                    break;

                case "TIMER":
                    outTagFormat = TagFormats.TIMER;
                    break;

                case "COUNTER":
                    outTagFormat = TagFormats.COUNTER;
                    break;

                case "ARRAYOF32BITS":
                    outTagFormat = TagFormats.ARRAYOF32BITS;
                    break;

                case "STRING":
                    outTagFormat = TagFormats.STRING;
                    break;

                case "STRUCTURE":
                    outTagFormat = TagFormats.STRUCTURE;
                    break;

                case "LINT":
                    outTagFormat = TagFormats.LINT;
                    break;

                default:
                    bRet = false;
                    break;

            };
            return (bRet);
        }

        private static ImportPrototype addPrototype(Dictionary<string, ImportPrototype> protoMap, ImportData elem)
        {
            ImportPrototype proto = null;
            if (!protoMap.ContainsKey(((ImportDataEthernetIP)elem).ElemType))
            {
                if (elem.Children.Count != 0)
                {
                    proto = new ImportPrototype();
                    proto.Name = ((ImportDataEthernetIP)elem).ElemType;
                    proto.Elements = new List<ImportTag>();

                    foreach (var t in elem.Children)
                    {
                        ImportData el = t as ImportData;
                        if (el != null)
                        {
                            ImportTag a = new ImportTag();

                            if (el.szType == "STRUCT")
                            {
                                ImportPrototype protoElem = addPrototype(protoMap, el);
                                if (protoElem == null)
                                    continue;

                                a.ModelType = UFUAModel.ModelType.ObjectType;
                                a.PrototypeModel = protoElem.Name;
                            }
                            else
                            {
                                a.ModelType = ModelType.Variable;
                            }
                            a.ArrayDimension = el.ArrayDimension;
                            a.Name = el.Name;
                            a.DataType = ((ImportDataEthernetIP)el).Type;
                            a.Description = el.Description;
                            proto.Elements.Add(a);
                        }
                    }
                    protoMap.Add(((ImportDataEthernetIP)elem).ElemType, proto);
                }
            }
            else
                proto = protoMap[((ImportDataEthernetIP)elem).ElemType];
            return proto;
        }

        private ImportDataModel LoadFile(string file)
        {
            EtherNetIPStationSettings stationSett = (EtherNetIPStationSettings)baseImportTree.GetStationSettings(baseImportTree.ReadStationName());           

            bool bErrorFileSelected = false;
            file = file.ToLower();
            switch (stationSett.PlcType)
            {
                case PlcTypes.ControlLogix_CompactLogix:
                    if(!file.Contains(".l5k"))
                    {
                        bErrorFileSelected = true;
                    }
                    break;
                case PlcTypes.Micro800_series:
                    if(!file.Contains(".xls") &&
                       !file.Contains(".csv"))
                    {
                        bErrorFileSelected = true;
                    }
                    break;
                case PlcTypes.PLC5:
                case PlcTypes.SLC500_MicroLogix:
                    if(!file.Contains(".csv"))
                    {
                        bErrorFileSelected = true;
                    }
                    break;
                default:
                    bErrorFileSelected = true;
                    break;

            }
            if(bErrorFileSelected)
            {
                MessageBox.Show(string.Format(Properties.Resources.ErrorSelectedFile, stationSett.Name),
                                Properties.Resources.ImportMsgBoxTitle);
                return(null);
            }

            importDataModel = new ImportDataModelEthernetIP(readStationName);
            using (new WaitCursor())
            {   
                if (file.Contains(".l5k"))
                {
                    DisplayImportFileL5K(file);
                    if (importDataModel == null || importDataModel.Children.Count == 0)
                    {
                        MessageBox.Show(DriverCodeBase.UI.Properties.Resources.ImportErrorEmptyFile,
                                        Properties.Resources.ImportMsgBoxTitle);
                    }

                }
                else if (file.Contains(".xls"))
                {
                    DisplayImportFileXLS(file);
                    if (importDataModel == null || importDataModel.Children.Count == 0)
                    {
                        MessageBox.Show(DriverCodeBase.UI.Properties.Resources.ImportErrorEmptyFile,
                                        Properties.Resources.ImportMsgBoxTitle);
                    }

                }
                else if (file.Contains(".csv"))
                {
                    if (EtherNetIpProtocol.IsOldPlcModel(stationSett.PlcType))
                        DisplayImportFileCSVOldPlc(file);
                    else
                        DisplayImportFileCSV(file);

                    if (importDataModel == null || importDataModel.Children.Count == 0)
                    {
                        MessageBox.Show(DriverCodeBase.UI.Properties.Resources.ImportErrorEmptyFile,
                                        Properties.Resources.ImportMsgBoxTitle);
                    }

                }

                return importDataModel;
            }
        }

        void DisplayMicro8xxImportFile(List<string[]> parsedData)
        {
            DataType MovType;
            uint varsize = 0;
            UInt32 ArraySize = 0;
            String ArrayParent = "";
            long lParentID = -1;
            var Parent = importDataModel.addImportData();

            foreach (string[] s in parsedData)
            {
                MovType = GetXLSMoviconTypeId(s, ref varsize);
                if ((isArrayXls(s[(int)XlsFieldIndex.Dimension]) && MovType == DataType.String) ||
                    MovType == unchecked((DataType)(-1)))
                {
                    continue;
                }


                var IVar = importDataModel.addImportData();
                IVar.Name = s[(int)XlsFieldIndex.Name];
                IVar.Address = s[(int)XlsFieldIndex.Name];
                ((ImportDataEthernetIP)IVar).Size = varsize;
                IVar.Id = m_lGlobalID++;
                ((ImportDataEthernetIP)IVar).ElemType = s[(int)XlsFieldIndex.DataType];
                ((ImportDataEthernetIP)IVar).Type = MovType;
                if (lParentID == -1)
                {
                    if (isArrayXls(s[(int)XlsFieldIndex.Dimension]))
                    {
                        ((ImportDataEthernetIP)IVar).ImportType = ImportTypes.Array;
                        Parent = IVar;
                        lParentID = IVar.Id;
                        ArrayParent = IVar.Name + "[";
                        ArraySize = 0;
                    }
                    AddTreeItem(IVar);
                }
                else
                {
                    if (IVar.Name.IndexOf(ArrayParent) == 0)
                    {
                        IVar.TagType = Parent.TagType;
                        AddTreeItem(IVar, Parent);
                        ArraySize++;
                    }
                    else
                    {
                        Parent.ArrayDimension = ArraySize;
                        ((ImportDataEthernetIP)Parent).Size = ((ImportDataEthernetIP)Parent).Size * ArraySize;
                        ArraySize = 0;
                        if (isArrayXls(s[(int)XlsFieldIndex.Dimension]))
                        {
                            ((ImportDataEthernetIP)IVar).ImportType = ImportTypes.Array;
                            Parent = IVar;
                            lParentID = IVar.Id;
                            ArrayParent = IVar.Name + "[";
                        }
                        else
                        {
                            lParentID = -1;
                            ArrayParent = "";
                        }
                        AddTreeItem(IVar);
                    }
                }

            }
            if (lParentID != -1)
            {
                Parent.ArrayDimension = ArraySize;
                ((ImportDataEthernetIP)Parent).Size = ((ImportDataEthernetIP)Parent).Size * ArraySize;
            }
        }

        private void DisplayImportFileCSV(string fileCSV)
        {
            List<string[]> parsedData = new List<string[]>();
            try
            {
                using (System.IO.StreamReader readFile = new System.IO.StreamReader(fileCSV))
                {
                    string line;
                    string[] row;
                    int colNum = (int)XlsFieldIndex.StringSize + 1;
                    while ((line = readFile.ReadLine()) != null)
                    {
                        if (!string.IsNullOrWhiteSpace(line))
                        {
                            string[] partialRow = new string[colNum];
                            for (int j = 0; j < colNum; j++)
                            {
                                partialRow[j] = String.Empty;
                            }
                            row = line.Split(';');
                            int colMax = colNum;
                            if (colMax > row.Length)
                            {
                                colMax = row.Length;
                            }
                            for (int j = 0; j < colMax; j++)
                            {
                                if (!String.IsNullOrWhiteSpace(row[j]))
                                {
                                    partialRow[j] = row[j];
                                }
                            }
                            parsedData.Add(partialRow);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                if (Environment.UserInteractive)
                    MessageBox.Show(ex.Message);
                return;
            }

            DisplayMicro8xxImportFile(parsedData);
        }

        private void GetOldPlcCSVMoviconTypeId(string type, out DataType movType, out TagFormats dataFormat, out string address, out uint varSize) //, out DriverCodeBase.Enumerators.LinkType jobType)
        {
            dataFormat = unchecked((TagFormats)(-1));
            movType = unchecked((DataType)(-1));
            varSize = 0;

            address = type;

            if (string.IsNullOrEmpty(type))
                return;

            var TypeInfo = type.ToUpper().Split(':');
            //The numeric part is removed from the Type Info string
            string stType = Regex.Replace(TypeInfo[0], @"\d", "");
            switch (stType)
            {
                case "I": // FileTypes.Input
                    if (address.Contains("I:"))
                        address = address.Replace("I:", string.Format("I{0}:", (int)FileTypes.Input));
                    if (type.Contains('/'))
                    {
                        movType = DataType.Boolean;
                        dataFormat = TagFormats.BOOL;
                        varSize = 1;
                    }
                    else
                    {
                        movType = DataType.UInt16;
                        dataFormat = TagFormats.INT;
                        varSize = 2;
                    }
                    break;
                case "O": // FileTypes.Output                
                    if (address.Contains("O:"))
                        address = address.Replace("O:", string.Format("O{0}:", (int)FileTypes.Output));
                    if (type.Contains('/'))
                    {
                        movType = DataType.Boolean;
                        dataFormat = TagFormats.BOOL;
                        varSize = 1;
                    }
                    else
                    {
                        movType = DataType.UInt16;
                        dataFormat = TagFormats.INT;
                        varSize = 2;
                    }
                    break;
                case "S": // FileTypes.Status
                    if (address.Contains("S:"))
                        address = address.Replace("S:", string.Format("S{0}:", (int)FileTypes.Status));
                    if (type.Contains('/'))
                    {
                        movType = DataType.Boolean;
                        dataFormat = TagFormats.BOOL;
                        varSize = 1;
                    }
                    else
                    {
                        movType = DataType.UInt16;
                        dataFormat = TagFormats.INT;
                        varSize = 2;
                    }
                    break;
                case "B": // FileTypes.Status
                    if (type.Contains('/'))
                    {
                        movType = DataType.Boolean;
                        dataFormat = TagFormats.BOOL;
                        varSize = 1;
                    }
                    else
                    {
                        movType = DataType.UInt16;
                        dataFormat = TagFormats.INT;
                        varSize = 2;
                    }
                    break;
                case "T": // FileTypes.Timer
                    if (address.Contains(".ACC"))
                        address = address.Replace(".ACC", "/ACC");
                    if (address.Contains(".PRE"))
                        address = address.Replace(".PRE", "/PRE");
                    movType = DataType.UInt16;
                    dataFormat = TagFormats.INT;
                    varSize = 2;
                    break;
                case "C": // FileTypes.Counter
                    if (address.Contains(".ACC"))
                        address = address.Replace(".ACC", "/ACC");
                    if (address.Contains(".PRE"))
                        address = address.Replace(".PRE", "/PRE");
                    movType = DataType.UInt16;
                    dataFormat = TagFormats.INT;
                    varSize = 2;
                    break;
                case "R": // FileTypes.Control
                case "N": // FileTypes.Integer                    
                    if (type.Contains('/'))
                    {
                        dataFormat = TagFormats.BOOL;
                        movType = DataType.Boolean;
                        varSize = 1;
                    }
                    else
                    {
                        dataFormat = TagFormats.INT;
                        movType = DataType.UInt16;
                        varSize = 2;
                    }
                    varSize = 2;
                    break;
                case "F": // FileTypes.Float
                    movType = DataType.Float;
                    dataFormat = TagFormats.REAL;
                    varSize = 4;
                    break;
                case "ST": // FileTypes.String
                    movType = DataType.String;
                    dataFormat = TagFormats.STRING;
                    varSize = 84;
                    break;
                case "L": // FileTypes.LINT
                    movType = DataType.Int32;
                    dataFormat = TagFormats.LINT;
                    varSize = 4;
                    break;
            }
        }

        void DisplayOldPlcImportFile(List<string[]> parsedData)
        {
            foreach (string[] s in parsedData)
            {
                uint VarSize;
                TagFormats DataFormat;
                DataType MovType;
                string Address;

                // parse address and retrive info to built dynamic settings and tag variable
                GetOldPlcCSVMoviconTypeId(s[(int)CsvOldPlcFieldIndex.Address], out MovType, out DataFormat, out Address, out VarSize);
                if (MovType != unchecked((DataType)(-1))) {
                    var IVar = importDataModel.addImportData();
                    // if symbol (name) is not present, use instead parsed address
                    if (!string.IsNullOrEmpty(s[(int)CsvOldPlcFieldIndex.Symbol]))
                        IVar.Name = s[(int)CsvOldPlcFieldIndex.Symbol];
                    else
                        IVar.Name = UFUAModel.Helpers.NameValidator.EnsureValidName(s[(int)CsvOldPlcFieldIndex.Address]);
                    IVar.Address = Address;
                    ((ImportDataEthernetIP)IVar).Size = VarSize;
                    IVar.Id = m_lGlobalID++;
                    ((ImportDataEthernetIP)IVar).ElemType = DataFormat.ToString();
                    ((ImportDataEthernetIP)IVar).Type = MovType;
                    ((ImportDataEthernetIP)IVar).AddressMode = AddressTypes.DataFile;
                    AddTreeItem(IVar);
                }
            }
        }

        /// <summary>
        /// Import from CSV file for PlcTypes.SLC500_MicroLogix,PLC5 PLC series
        /// </summary>
        /// <param name="fileCSV"></param>
        private void DisplayImportFileCSVOldPlc(string fileCSV)
        {
            List<string[]> parsedData = new List<string[]>();
            try
            {
                using (System.IO.StreamReader readFile = new System.IO.StreamReader(fileCSV))
                {
                    string line;
                    int colNum = Enum.GetNames(typeof(CsvOldPlcFieldIndex)).Length;
                    while ((line = readFile.ReadLine()) != null)
                    {
                        if (!string.IsNullOrWhiteSpace(line))
                        {
                            // split line by <,> or ""
                            var regex = new Regex("(?<=^|,)(\"(?:[^\"]|\"\")*\"|[^,]*)");
                            MatchCollection CsvColumns = regex.Matches(line);

                            string[] partialRow = new string[colNum];
                            for (int j = 0; j < colNum; j++)
                            {
                                if (CsvColumns.Count > (j + 1))
                                    partialRow[j] = CsvColumns[j].Value.Trim();
                                else
                                    partialRow[j] = String.Empty;
                            }

                            parsedData.Add(partialRow);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                if (Environment.UserInteractive)
                    MessageBox.Show(ex.Message);
                return;
            }

            DisplayOldPlcImportFile(parsedData);
        }

        private void DisplayImportFileXLS(string fileXLS)
        {
            List<string[]> parsedData = new List<string[]>();
            ReadExcel readExcel = new ReadExcel();
            try
            {
                if (!ReadExcel.ReadExcelFile(fileXLS, ref parsedData))
                {
                    return;
                }
            }
            catch (Exception ex)
            {
                if (Environment.UserInteractive)
                    MessageBox.Show(ex.Message);
                return;
            }

            DisplayMicro8xxImportFile(parsedData);
        }

        private ImportDataTreeItemControl CreateImportDataTreeItemControl(object tag)
        {
            return ((ImportDataTreeItemControl)new ImportDataTreeItemControlEthernetIP(tag));
        }
        //bool isArrayXls(String Dimension)
        //{
        //    Dimension.Trim();
        //    if (Dimension.IsEmpty())
        //    {
        //        return (false);
        //    }
        //    int nSearchIndex1 = Dimension.Find(_T("["));
        //    int nSearchIndex2 = Dimension.Find(_T("]"));

        //    return (nSearchIndex1 >= 0 && nSearchIndex2 > nSearchIndex1);

        //}
        //int GetVarTypeAndSizeXLS(ref String szType, ref uint nVarSize )
        //{
        //    int nType = -1;
        //    nVarSize = 0;
        //    szType.Trim();
        //    if (szType.IsEmpty())
        //    {
        //        return (nType);
        //    }
        //    if (!szType.CompareNoCase(_T("BOOL")))
        //    {
        //        nType = _DRV_VAR_TYPE_BIT;
        //        nVarSize = 1;
        //    }
        //    else if (!szType.CompareNoCase(_T("SINT")))
        //    {
        //        nType = _DRV_VAR_TYPE_SIGNBYTE;
        //        nVarSize = 1;
        //    }
        //    else if (!szType.CompareNoCase(_T("INT")))
        //    {
        //        nType = _DRV_VAR_TYPE_SIGNWORD;
        //        nVarSize = 2;
        //    }
        //    else if (!szType.CompareNoCase(_T("DINT")))
        //    {
        //        nType = _DRV_VAR_TYPE_SIGNDWORD;
        //        nVarSize = 4;
        //    }
        //    else if (!szType.CompareNoCase(_T("REAL")))
        //    {
        //        nType = _DRV_VAR_TYPE_FLOAT;
        //        nVarSize = 4;
        //    }
        //    else if (!szType.CompareNoCase(_T("STRING")))
        //    {
        //        nType = _DRV_VAR_TYPE_STRING;
        //        nVarSize = 82;
        //    }


        //    return (nType);
        //}
        bool isArrayXls(String Dimension)
        {
            Dimension.Trim();
            if (Dimension.Length == 0)
            {
                return (false);
            }
            int nSearchIndex1 = Dimension.IndexOf('[');
            int nSearchIndex2 = Dimension.IndexOf(']');

            return (nSearchIndex1 >= 0 && nSearchIndex2 > nSearchIndex1);

        }

        public DataType GetXLSMoviconTypeId(string[] s, ref uint VarSize)
        {
            DataType nType = unchecked((DataType)(-1));
            VarSize = 0;
            string Type = s[(int)XlsFieldIndex.DataType];
            Type.Trim();
            if (Type.Length == 0)
                return (nType);
            Type = Type.ToUpper();
            switch (Type)
            {
                case "BOOL":
                    nType = DataType.Boolean;
                    VarSize = 1;
                    break;
                case "SINT":
                    nType = DataType.SByte;
                    VarSize = 1;
                    break;
                case "DINT":
                    nType = DataType.Int32;
                    VarSize = 2;
                    break;
                case "INT":
                    nType = DataType.Int16;
                    VarSize = 4;
                    break;
                case "REAL":
                    nType = DataType.Float;
                    VarSize = 4;
                    break;
                case "STRING":
                    nType = DataType.String;
                    if (!uint.TryParse(s[(int)XlsFieldIndex.StringSize], out VarSize))
                        VarSize = EtherNetIpProtocol.STRING_MAX_LENGHT;
                    break;
                case "LINT":
                    nType = DataType.Int64;
                    VarSize = 8;
                    break;
            }
            return (nType);

        }

        private void DisplayImportFileL5K(string fileL5K)
        {
            System.IO.StreamReader readFile = new System.IO.StreamReader(fileL5K);

            try
            {

                string line;

                ParseDataTypeL5K(ref readFile);

                PrepareDataTypeMapsL5K();

                //while ((line = L5KReadLine(ref readFile)) != null)
                while ((line = ReadLineL5K(ref readFile)) != null)
                {
                    if (!ParseModuleVariableL5K(ref readFile, line))
                        ParseGlobalVariableL5K(ref readFile, line);
                }

            }
            catch (Exception e)
            {
            }
            finally
            {
                if (readFile != null)
                {
                    readFile.Close();
                }
            }
        }

        void ParseDataTypeL5K(ref System.IO.StreamReader readFile)
        {
            string line;
            string Key = "";
            string Argument = "";
            string StructName;

            m_mapStruct.Clear();

            InserDataTypeDefinitionL5K("TIMER", "DINT PRE;DINT ACC;BIT EN;BIT TT;BIT DN;END_DATATYPE");
            InserDataTypeDefinitionL5K("COUNTER", "DINT PRE;DINT ACC;BIT CU;BIT CD;BIT DN;BIT OV;BIT UN;END_DATATYPE");
            InserDataTypeDefinitionL5K("AXIS_CIP_DRIVE", "REAL ActualVelocity;REAL CurrentFeedback;END_DATATYPE");

            while ((line = ReadLineL5K(ref readFile)) != null)
            {
                // Skip empty lines
                if (line.Length == 0)
                {
                    continue;
                }

                String TypeDefinitionBlock;
                Key = GetKeyL5K(line, ref Argument);
                switch (Key)
                {
                    case "DATATYPE":
                        if (!GetStructNameL5K(Argument, out StructName))
                            continue;
                        if (!GetDataTypeDefinitionL5K(ref readFile, out TypeDefinitionBlock))
                            continue;
                        break;
                    case "ADD_ON_INSTRUCTION_DEFINITION":
                        if (!GetStructNameL5K(Argument, out StructName))
                            continue;
                        if (!GetAddOnInstructionDefinitionL5K(ref readFile, out TypeDefinitionBlock))
                            continue;
                        break;
                    default:
                        continue;

                }
                InserDataTypeDefinitionL5K(StructName, TypeDefinitionBlock);
            }

            readFile.BaseStream.Seek(0, System.IO.SeekOrigin.Begin);
        }

        void PrepareDataTypeMapsL5K()
        {
            Dictionary<string, List<string>> mapOk = new Dictionary<string, List<string>>();
            Dictionary<string, List<string>> mapTemp = new Dictionary<string, List<string>>();
            bool bOk = true;

            foreach (var itemStruct in m_mapStruct)
            {
                foreach (string itemStructValue in itemStruct.Value)
                    bOk &= (GetStructMemberTypeFromStringL5K(itemStructValue) != ImportTypes.Unknown);
                if (bOk)
                    mapOk.Add(itemStruct.Key, itemStruct.Value);
                else
                    mapTemp.Add(itemStruct.Key, itemStruct.Value);
            }
            foreach (var itemTemp in mapTemp)
            {
                List<string> ValDef = new List<string>();
                foreach (string itemTempValue in itemTemp.Value)
                {
                    bool bType = (GetStructMemberTypeFromStringL5K(itemTempValue) != ImportTypes.Unknown);
                    bOk = mapOk.ContainsKey(itemTemp.Key);
                    if (bOk)
                        foreach (string itemOkValue in mapOk[itemTemp.Key])
                            ValDef.Add(itemOkValue);
                    else if (bType)
                        ValDef.Add(itemTempValue);
                }
                mapOk.Add(itemTemp.Key, ValDef);

            }
            m_mapStruct = mapOk;
        }

        bool ParseModuleVariableL5K(ref System.IO.StreamReader readFile,
                                            String szLine)
        {
            // Check if the line is the beginning of a program module
            String szProgramName = "";
            if (GetKeyL5K(szLine, ref szProgramName) != "PROGRAM")
            {
                return false;
            }

            szProgramName = szProgramName.Trim();

            int nIndex = szProgramName.IndexOf(" ");
            if (nIndex < 0)
            {
                return false;
            }

            // Save the current position in the file
            // (just in case something goes wrong)
            long ulFirstFilePos = readFile.BaseStream.Position;

            szProgramName = szProgramName.Substring(0, nIndex);
            szProgramName.Trim();

            // Read the whole block of the module definition
            String szVarBlock = "";
            bool bDefinitionComplete = false;
            bool bFindInitTag = false;
            bool bFindEndTag = false;

            String szFileLine;
            String szAux = "";
            while (!bDefinitionComplete && (szFileLine = ReadLineL5K(ref readFile)) != null)
            {
                if (GetKeyL5K(szFileLine, ref szAux) == "END_PROGRAM")
                {
                    bDefinitionComplete = true;
                }
                else
                {
                    if (!bFindInitTag)
                    {
                        if (GetKeyL5K(szFileLine, ref szAux) == "TAG")
                        {
                            bFindInitTag = true;
                        }
                    }
                    else
                    {
                        if (!bFindEndTag)
                        {
                            if (GetKeyL5K(szFileLine, ref szAux) == "END_TAG")
                            {
                                bFindEndTag = true;
                            }
                            else
                            {
                                szVarBlock += szFileLine;
                            }
                        }
                    }
                }
            }
            // Check if the block has been correctly read
            if (!bDefinitionComplete)
            {
                readFile.BaseStream.Seek(ulFirstFilePos, System.IO.SeekOrigin.Begin);
                return false;
            }
            if (bFindEndTag)
            {
                // Initializations

                String szAddressPrefix = string.Format("Program:{0}.", szProgramName);
                String szNamePrefix = string.Format("{0}_", szProgramName);
                String szDescrPrefix = string.Format("Module {0} ", szProgramName);

                // Add variable declarations to the list of variables that can be imported
                ParseVariableBlockL5K(ref szVarBlock, ref szAddressPrefix, ref szNamePrefix,
                    ref szDescrPrefix);
            }

            return true;
        }

        bool ParseGlobalVariableL5K(ref System.IO.StreamReader readFile,
                                            String szLine)
        {
            // Check if the line is the beginning of a block of global variable
            // declarations
            String szAux = "";
            if (GetKeyL5K(szLine, ref szAux) != "TAG")
            {
                return false;
            }

            // Save the current position in the file
            // (just in case something goes wrong)
            long ulFirstFilePos = readFile.BaseStream.Position;

            // Read the whole block of variable definition
            String szVarBlock = "";
            bool bDefinitionComplete = false;



            while (!bDefinitionComplete && (szAux = ReadLineL5K(ref readFile)) != null)
            {
                szVarBlock += szAux;

                if (GetKeyL5K(szAux, ref szAux) == "END_TAG")
                {
                    bDefinitionComplete = true;
                }
            }

            // Check if the block has been correctly read
            if (!bDefinitionComplete)
            {
                readFile.BaseStream.Seek(ulFirstFilePos, System.IO.SeekOrigin.Begin);
                return false;
            }


            // Initializations

            String szAddressPrefix = "";
            String szNamePrefix = "";
            String szDescrPrefix = "Global Variable ";


            // Add variable declarations to the list of variables that can be imported
            ParseVariableBlockL5K(ref szVarBlock, ref szAddressPrefix, ref szNamePrefix,
                ref szDescrPrefix);

            return true;
        }


        void ParseVariableBlockL5K(ref String szBlock,
                                            ref String szAddressPrefix,
                                            ref String szNamePrefix,
                                            ref String szDescrPrefix)
        {
            bool bParsingCompleted = false;
            int nIndex = 0;
            int nIndex2 = 0;
            String szAux;
            String szAux2;
            String szVarName;
            String szVarType;
            String description;
            int nIndexBlock = 0;

            while (!bParsingCompleted)
            {
                // Parse the variable name, address and type, and add an item to
                // the variable list
                if (nIndexBlock >= szBlock.Length)
                {
                    bParsingCompleted = true;
                    continue;
                }

                nIndex = szBlock.IndexOf(';', nIndexBlock);
                if (nIndex < 1)
                {
                    bParsingCompleted = true;
                    continue;
                }

                szAux = szBlock.Substring(nIndexBlock, nIndex - nIndexBlock).Trim(new char[] { '\r', '\n', '\t', ' ' });

                nIndexBlock = nIndex + 1;

                if (szAux.Length == 0)
                {
                    continue;
                }

                //if the variable has the External Access attribute is set to None, then the tag cannot be accessed from outside the controller.
                if (szAux.IndexOf("ExternalAccess := None", StringComparison.InvariantCultureIgnoreCase) != -1)
                {
                    continue;
                }

                description = GetDescriptionL5K(szAux).Trim(new char[] { '\"' });
                RemoveArgumentsL5K(ref szAux);

                // Search for the character ':'
                nIndex2 = szAux.IndexOf(':');
                if (nIndex2 < 1)
                {
                    continue;
                }

                // Get the variable name
                szVarName = szAux.Substring(0, nIndex2).Trim(new char[] { '\r', '\n', '\t', ' ' });
                if (szVarName.Length == 0)
                {
                    continue;
                }

                // Get the variable type
                if (nIndex2 == (szAux.Length - 1))
                {
                    continue;
                }

                szAux2 = szAux.Substring(nIndex2 + 1).Trim(new char[] { '\r', '\n', '\t', ' ' });
                if (szAux2.Length == 0)
                {
                    continue;
                }

                // Variable initialization?
                nIndex2 = szAux2.IndexOf(":=");
                if (nIndex2 < 0)
                {
                    nIndex2 = szAux2.Length;
                }

                szVarType = szAux2.Substring(0, nIndex2).Trim(new char[] { '\r', '\n', '\t', ' ' });
                if (szVarType.Length == 0)
                {
                    continue;
                }

                // Check the variable type and get the variable size
                uint nVarSize = 0;
                uint nArraySize0 = 0;
                uint nArraySize1 = 0;
                uint nArraySize2 = 0;
                string nMovType = "";
                ImportTypes nVariableType = ImportTypes.Unknown;
                ImportTypes arrayType = ImportTypes.Unknown;

                if (GetArrayDimL5K(ref szVarType, ref szAux, ref nArraySize0, ref nArraySize1, ref nArraySize2))
                {
                    nVariableType = ImportTypes.Array;
                    szVarType = szAux;
                    arrayType = GetVarTypeAndSizeL5K(szVarType, ref nVarSize, ref nMovType);
                }
                else
                {
                    nVariableType = GetVarTypeAndSizeL5K(szVarType, ref nVarSize, ref nMovType);
                }

                // Add the variable to the dialog list
                //////////////////////////////////////////////////////////////////////////

                String szAddress = szAddressPrefix + szVarName;
                szVarName = szNamePrefix + szVarName;
                switch (nVariableType)
                {
                    // Variable of standard type
                    case ImportTypes.Standard:
                        AddStandardVar("", szVarName, "", szAddress,
                                description, szVarType, nMovType, nVarSize);
                        break;

                    // Structure or enumeration
                    case ImportTypes.Struct:
                        AddStruct("", szVarName, "", szAddress, description, szVarType);
                        break;

                    // Array
                    case ImportTypes.Array:
                        AddArray("", szVarName, "", szAddress,
                            description, szVarType, nVarSize, nArraySize0,
                            nArraySize1, nArraySize2, arrayType, nMovType);
                        break;
                }
            }
        }

        void AddStandardVar(String szNamePrefix, String szVarName,
                                            String szAddressPrefix, String szAddress,
                                            String description, String szVarType, String nMovType,
                                            uint nVarSize, int parentId = -1, ImportData inRootItem = null)
        {


            var IVar = importDataModel.addImportData();
            ((ImportDataEthernetIP)IVar).PreName = szNamePrefix;
            IVar.Name = szVarName;
            ((ImportDataEthernetIP)IVar).ElemType = szVarType;
            ((ImportDataEthernetIP)IVar).Type = (DataType)Enum.Parse(typeof(DataType), nMovType);
            IVar.szType = "";
            ((ImportDataEthernetIP)IVar).Size = nVarSize;
            IVar.Address = szAddressPrefix + szAddress;
            IVar.Description = description;
            IVar.parentId = parentId;
            IVar.Id = m_lGlobalID++;
            IVar.ArrayDimension = 0;
            ((ImportDataEthernetIP)IVar).ImportType = ImportTypes.Standard;

            AddTreeItem(IVar, inRootItem);

        }



        bool AddStruct(String szNamePrefix, String szVarName, String szAddressPrefix, String szAddress,
            String description, String szVarType, int parentId = -1, ImportData inRootItem = null)
        {
            if (!StructFoundL5K(szVarType))
                return false;
            if (m_mapStruct[szVarType].Count == 0)
                return false;


            String szStructNamePrefix = szNamePrefix + szVarName + ".";
            String szStructAddressPrefix = szAddressPrefix + szAddress + ".";

            int lParentID = parentId;

            uint nFieldSize = 0;

            if (inRootItem == null)
                lParentID = -1;

            ImportData rootItem = importDataModel.addImportData();
            rootItem.parentId = lParentID;
            lParentID = m_lGlobalID++;
            rootItem.Id = lParentID;
            ((ImportDataEthernetIP)rootItem).PreName = szNamePrefix;
            rootItem.Name = szVarName;
            rootItem.szType = "STRUCT";
            ((ImportDataEthernetIP)rootItem).ElemType = szVarType;
            ((ImportDataEthernetIP)rootItem).Type = DataType.Boolean;
            rootItem.Address = szAddressPrefix + szAddress;
            rootItem.Description = description;
            ((ImportDataEthernetIP)rootItem).Size = nFieldSize;
            rootItem.ArrayDimension = 0;

            String szAux = "";
            String szFieldName;
            String szFieldType;
            String szFieldDescription;
            string nMovType = "";
            int nIndex = 0;
            int nIndex2 = 0;
            uint nArraySize0 = 0;
            uint nArraySize1 = 0;
            uint nArraySize2 = 0;
            ImportTypes nFieldType = ImportTypes.Unknown;
            ImportTypes elementType = ImportTypes.Unknown;

            bool isFirst = true;

            foreach (string itemStructValue in m_mapStruct[szVarType])
            {

                nIndex = itemStructValue.IndexOf(' ');
                if (nIndex <= 0)
                {
                    continue;
                }
                if (itemStructValue.Length < (nIndex + 2))
                {
                    continue;
                }
                nIndex2 = itemStructValue.IndexOf("\"");
                szFieldType = itemStructValue.Substring(0, nIndex).Trim();
                if (szFieldType.Length == 0)
                {
                    continue;
                }
                if (nIndex2 < (nIndex + 2))
                {
                    szFieldName = itemStructValue.Substring(nIndex + 1).Trim();
                    szFieldDescription = "";
                }
                else
                {
                    szFieldName = itemStructValue.Substring(nIndex + 1, nIndex2 - nIndex - 1).Trim();
                    szFieldDescription = itemStructValue.Substring(nIndex2).Trim(new char[] { '\"' });
                }
                if (szFieldName.Length == 0)
                {
                    continue;
                }


                elementType = GetVarTypeAndSizeL5K(szFieldType, ref nFieldSize, ref nMovType);

                nArraySize0 = 0;
                nArraySize1 = 0;
                nArraySize2 = 0;

                if (GetArrayDimL5K(ref szFieldName, ref szAux, ref nArraySize0, ref nArraySize1, ref nArraySize2))
                {
                    nFieldType = ImportTypes.Array;
                    szFieldName = szAux;

                }
                else
                {
                    nFieldType = elementType;
                }

                String elemDescription = (!string.IsNullOrEmpty(description)) ? description + ", " + szFieldDescription : "";

                // Add the field to the dialog list
                switch (nFieldType)
                {
                    // Variable of standard type
                    case ImportTypes.Standard:
                        //if (nMovType != "String")
                        //{
                            AddStructRoot(inRootItem, rootItem, ref isFirst, szAddress, szVarType);
                            AddStandardVar(szStructNamePrefix, szFieldName, "", szStructAddressPrefix + szFieldName,
                                elemDescription, szFieldType, nMovType,
                                nFieldSize, rootItem.Id, rootItem);
                        //}
                        //else
                        //{
                        //    AddStandardVar("", szStructNamePrefix + szFieldName, "", szStructAddressPrefix + szFieldName,
                        //        elemDescription, szFieldType, nMovType, nFieldSize);

                        //}
                        break;
                    // Array
                    case ImportTypes.Array:
                        if (elementType == ImportTypes.Standard)
                        {
                            AddStructRoot(inRootItem, rootItem, ref isFirst, szAddress, szVarType);
                            AddArray(szStructNamePrefix, szFieldName, szStructAddressPrefix, szFieldName,
                                    elemDescription, szFieldType, nFieldSize,
                                    nArraySize0, nArraySize1, nArraySize2, elementType, nMovType, lParentID, rootItem);

                        }
                        else
                        {
                            AddArray("", szStructNamePrefix + szFieldName, "", szStructAddressPrefix + szFieldName,
                                    elemDescription, szFieldType, nFieldSize, nArraySize0, nArraySize1, nArraySize2, elementType, nMovType);
                        }
                        break;
                    // Structure 
                    case ImportTypes.Struct:
                        AddStructRoot(inRootItem, rootItem, ref isFirst, szAddress, szVarType);
                        AddStruct(szStructNamePrefix, szFieldName, szStructAddressPrefix, szFieldName, elemDescription,
                                        szFieldType, lParentID, rootItem);

                        break;
                }
            }


            return true;
        }

        private void AddStructRoot(ImportData inRootItem, ImportData rootItem, ref bool isFirst, string szAddress, string szVarType)
        {
            if (isFirst)
            {
                isFirst = false;
                rootItem.Address = szAddress;
                ((ImportDataEthernetIP)rootItem).ImportType = ImportTypes.Struct;
                rootItem.szType = "STRUCT";
                if (rootItem.parentId < 0)
                {
                    AddTreeItem(rootItem);
                }
                else
                {
                    AddTreeItem(rootItem, inRootItem);
                }
            }
        }

        void AddArray(String szNamePrefix, String szArrayName, String szAddressPrefix,
                        String szArrayAddress, String description, String szElemType, uint nElemSize,
                        uint nArrayDim0, uint nArrayDim1, uint nArrayDim2, ImportTypes arrayType,
                        String nMovType, int parentId = -1, ImportData inRootItem = null)
        {

            if (nArrayDim0 == 0)
            {
                return;
            }

            String szName, szName1;
            String szAddress, szAddress1;
            if (nArrayDim1 == 0)
            {
                AddArray1Dim(ref szNamePrefix, ref szArrayName, ref szAddressPrefix, ref szArrayAddress, ref description,
                    ref szElemType, nElemSize, nArrayDim0, 0, arrayType, nMovType, parentId, inRootItem);
            }
            else if (nArrayDim2 == 0)
            {
                for (uint nIdx = 0; nIdx < nArrayDim0; nIdx++)
                {
                    szName = string.Format("{0}[{1}", szArrayName, nIdx);
                    szAddress = string.Format("{0}[{1}", szArrayAddress, nIdx);
                    AddArray1Dim(ref szNamePrefix, ref szName, ref szAddressPrefix, ref szAddress, ref description,
                        ref szElemType, nElemSize, nArrayDim1, 1, arrayType, nMovType, parentId, inRootItem);
                }
            }
            else
            {
                for (uint nIdx2 = 0; nIdx2 < nArrayDim0; nIdx2++)
                {
                    for (uint nIdx = 0; nIdx < nArrayDim1;
                        nIdx++)
                    {
                        szName1 = string.Format("{0}[{1},{2}", szArrayName, nIdx2, nIdx);
                        szAddress1 = string.Format("{0}[{1},{2}", szArrayAddress, nIdx2, nIdx);
                        AddArray1Dim(ref szNamePrefix, ref szName1, ref szAddressPrefix, ref szAddress1,
                            ref description, ref szElemType, nElemSize, nArrayDim2,
                                2, arrayType, nMovType, parentId, inRootItem);
                    }
                }
            }
        }

        void AddArray1Dim(ref String szNamePrefix, ref String szArrayName,
                            ref String szAddressPrefix, ref String szAddress, ref String description,
                            ref String szElemType, uint nElemSize, uint nArrayDim,
                            uint nDimIndex, ImportTypes arrayType, String nMovType, int parentId = -1, ImportData inRootItem = null)
        {
            if (nElemSize * nArrayDim > EtherNetIpProtocol.MAX_SEGMENT_DATA_SIZE && arrayType == ImportTypes.Standard)
            {
                uint nArrayBlockSize = EtherNetIpProtocol.MAX_SEGMENT_DATA_SIZE / nElemSize;
                uint nArrayBlocks = (nArrayDim + nArrayBlockSize - 1) / nArrayBlockSize;
                uint nArrayBlock = 1;
                uint nArrayStart = 0;
                while (nArrayStart < nArrayDim)
                {
                    if (nArrayStart + nArrayBlockSize > nArrayDim)
                        nArrayBlockSize = nArrayDim - nArrayStart;
                    AddArray1DimDisplace(ref szNamePrefix, ref szArrayName,
                        ref szAddressPrefix, ref szAddress, ref description,
                        ref szElemType, nElemSize, nArrayBlockSize,
                        nDimIndex, nArrayStart, nArrayBlock, nArrayBlocks, arrayType, nMovType, parentId, inRootItem);
                    nArrayBlock++;
                    nArrayStart += nArrayBlockSize;
                }

                return;
            }
            else
            {
                AddArray1DimDisplace(ref szNamePrefix, ref szArrayName,
                    ref szAddressPrefix, ref szAddress, ref description,
                    ref szElemType, nElemSize, nArrayDim,
                    nDimIndex, 0, 1, 1, arrayType, nMovType, parentId, inRootItem);
            }
        }

        void AddArray1DimDisplace(ref String szNamePrefix, ref String szArrayName,
                            ref String szAddressPrefix, ref String szAddress, ref String description,
                            ref String szElemType, uint nElemSize, uint nArrayDim,
                            uint nDimIndex, uint nArrayStart, uint nArrayBlock, uint nArrayBlocks, ImportTypes arrayType,
                            String nMovType, int parentId = -1, ImportData inRootItem = null)
        {

            if (nArrayDim == 0)
            {
                return;
            }

            uint nElementSize = 0;


            if ((nMovType == "String") && (nElemSize > 0))
            {
                nElementSize = nElemSize;
            }

            ImportData IVar = null;
            int IVarId = -1;

            String szAux;


            if (arrayType == ImportTypes.Standard && nMovType != "String")
            {
                IVar = importDataModel.addImportData();

                String szName = szArrayName;
                if (nDimIndex > 0)
                {
                    szName += "]";
                }
                if (nArrayBlocks > 1)
                {
                    szAux = string.Format("_{0}_of_{1}", nArrayBlock, nArrayBlocks);
                    szName += szAux;
                }
                ((ImportDataEthernetIP)IVar).PreName = szNamePrefix;
                IVar.Name = szName;

                ((ImportDataEthernetIP)IVar).ElemType = szElemType;
                ((ImportDataEthernetIP)IVar).Type = (DataType)Enum.Parse(typeof(DataType), nMovType);
                IVar.szType = "ARRAY";
                ((ImportDataEthernetIP)IVar).ImportType = ImportTypes.Array;

                IVar.ArrayDimension = nArrayDim;
                ((ImportDataEthernetIP)IVar).Size = nArrayDim * nElementSize;

                if (nDimIndex == 0)
                    szAux = string.Format("[{0}]", nArrayStart);
                else
                    szAux = string.Format(",{0}]", nArrayStart);

                IVar.Address = szAddressPrefix + szAddress + szAux;
                IVar.Description = description;
                IVar.parentId = parentId;
                IVarId = IVar.Id = m_lGlobalID++;
                AddTreeItem(IVar, inRootItem);

            }

            String szFieldLine;
            uint i = 0;
            for (i = 0; i < nArrayDim; i++)
            {
                szFieldLine = szArrayName;
                if (nDimIndex == 0)
                {
                    szAux = string.Format("[{0}]", i + nArrayStart);
                }
                else
                {
                    szAux = string.Format(",{0}]", i + nArrayStart);
                }
                szFieldLine += szAux;

                if (arrayType == ImportTypes.Standard && nMovType != "String")
                {
                    AddStandardVar(szNamePrefix, szFieldLine, szAddressPrefix, szAddress + szAux,
                        description, szElemType, nMovType, nElementSize, IVar.Id, IVar);

                }
                else if (arrayType == ImportTypes.Struct)
                {
                    AddStruct(szNamePrefix, szFieldLine, szAddressPrefix, szAddress + szAux,
                        description, szElemType, IVarId, IVar);
                }
                else if (arrayType != ImportTypes.Unknown)
                {
                    if (szNamePrefix == "")
                        AddStandardVar("", szFieldLine, szAddressPrefix, szAddress + szAux,
                            description, szElemType, nMovType, nElementSize);
                    else
                        AddStandardVar("", szNamePrefix + "." + szFieldLine, szAddressPrefix, szAddress + szAux,
                            description, szElemType, nMovType, nElementSize);

                }
            }
            return;
        }



        ImportTypes GetVarTypeAndSizeL5K(string inSzType, ref uint nVarSize, ref String nType)
        {
            ImportTypes exit = ImportTypes.Unknown;
            inSzType = inSzType.Trim();
            string szType = inSzType;
            szType = szType.ToUpper();
            nVarSize = 0;
            nType = "";
            if (szType.Length == 0)
            {
                return (exit);
            }
            if (szType == "BOOL" ||
                szType == "BIT")
            {
                nType = "Boolean";
                nVarSize = 1;
                exit = ImportTypes.Standard;
            }
            else if (szType == "BYTE")
            {
                nType = "Byte";
                nVarSize = 1;
                exit = ImportTypes.Standard;
            }
            else if (szType == "SINT")
            {
                nType = "SByte";
                nVarSize = 1;
                exit = ImportTypes.Standard;
            }
            else if (szType == "WORD"
                || szType == "UINT")
            {
                nType = "UInt16";
                nVarSize = 2;
                exit = ImportTypes.Standard;
            }
            else if (szType == "DWORD"
                || szType == "UDINT")
            {
                nType = "UInt32";
                nVarSize = 4;
                exit = ImportTypes.Standard;
            }
            else if (szType == "INT")
            {
                nType = "Int16";
                nVarSize = 2;
                exit = ImportTypes.Standard;
            }
            else if (szType == "DINT")
            {
                nType = "Int32";
                nVarSize = 4;
                exit = ImportTypes.Standard;
            }
            else if (szType == "REAL")
            {
                nType = "Float";
                nVarSize = 4;
                exit = ImportTypes.Standard;
            }
            else if (szType == "LREAL")
            {
                nType = "Double";
                nVarSize = 8;
                exit = ImportTypes.Standard;
            }
            else if (szType == "STRING")
            {
                nType = "String";
                nVarSize = EtherNetIpProtocol.STRING_MAX_LENGHT;
                exit = ImportTypes.Standard;
            }
            else if (szType == "LINT")
            {
                nType = "Int64";
                nVarSize = 8;
                exit = ImportTypes.Standard;
            }
            else if (m_mapStruct.ContainsKey(inSzType))
            {
                if (m_mapStruct[inSzType].Count != 0)
                {
                    nType = inSzType;
                    nVarSize = StructSize(inSzType);
                    exit = ImportTypes.Struct;
                }
            }

            return (exit);
        }

        uint StructSize(string szType)
        {
            uint outVal = 0;
            if (m_mapStruct.ContainsKey(szType))
            {
                if (m_mapStruct[szType].Count != 0)
                {
                    uint nFieldSize = 0;
                    String nMovType = "";
                    foreach (string itemStructValue in m_mapStruct[szType])
                    {
                        if (GetStructMemberTypeFromStringL5K(itemStructValue, ref nFieldSize, ref nMovType) != ImportTypes.Unknown)
                            outVal += nFieldSize;
                    }
                }
            }
            return (outVal);
        }

        ImportTypes GetStructMemberTypeFromStringL5K(String szFieldLine)
        {
            uint nFieldSize = 0;
            String nMovType = "";
            return GetStructMemberTypeFromStringL5K(szFieldLine, ref nFieldSize, ref nMovType);
        }

        ImportTypes GetStructMemberTypeFromStringL5K(String szFieldLine, ref uint nFieldSize, ref String nMovType)
        {
            // Field type?
            ImportTypes nFieldType = ImportTypes.Unknown;

            int nSearchIndex = szFieldLine.IndexOf(" ");
            if (nSearchIndex < 0)
            {
                return (nFieldType);
            }

            nFieldType = GetVarTypeAndSizeL5K(szFieldLine.Substring(0, nSearchIndex), ref nFieldSize,
                ref nMovType);

            return nFieldType;
        }

        void InserDataTypeDefinitionL5K(string StructName, string TypeDefinitionBlock)
        {
            if (!m_mapStruct.ContainsKey(StructName))
            {
                List<string> StringArray = new List<string>();
                if (ParseDataTypeDefinitionBlockL5K(TypeDefinitionBlock, StringArray))
                    m_mapStruct.Add(StructName, StringArray);
            }
        }

        bool ParseDataTypeDefinitionBlockL5K(String TypeDefinitionBlock, List<String> StringArray)
        {
            bool bParsingCompleted = false;
            int nIndex;
            int nIndexBlock = 0;

            String szAux;
            while (!bParsingCompleted)
            {
                if (nIndexBlock >= TypeDefinitionBlock.Length)
                {
                    bParsingCompleted = true;
                    continue;
                }

                nIndex = TypeDefinitionBlock.IndexOf(';', nIndexBlock);
                if (nIndex < 1)
                {
                    bParsingCompleted = true;
                    continue;
                }

                szAux = TypeDefinitionBlock.Substring(nIndexBlock, nIndex - nIndexBlock).Trim(new char[] { '\r', '\n', '\t', ' ', ';' });

                nIndexBlock = nIndex + 1;

                if (string.IsNullOrWhiteSpace(szAux))
                {
                    continue;
                }

                int nIndex2;
                String szAux2 = "";
                String description = "";
                description = GetDescriptionL5K(szAux);
                GetArgumentsL5K(szAux, ref szAux2);
                nIndex2 = szAux2.IndexOf("Hidden := 1");
                if (nIndex2 < 0)
                {
                    RemoveArgumentsL5K(ref szAux);
                    nIndex2 = szAux.IndexOf(' ');
                    if (!string.IsNullOrWhiteSpace(szAux) && nIndex2 > 0)
                    {
                        nIndex2 = szAux.IndexOf(' ', nIndex2 + 1);
                        if (nIndex2 > 0)
                        {
                            szAux2 = szAux.Substring(0, nIndex2).Trim(new char[] { '\r', '\n', '\t', ' ' });
                            szAux = szAux2;
                        }
                        if (description.Length == 0)
                            szAux += "\"\"";
                        else
                            szAux += description;

                        StringArray.Add(szAux);
                    }
                }
            }

            // Check if the TYPE definition has been correctly parsed
            if (StringArray.Count() == 0)
            {
                return false;
            }
            return true;

        }

        bool GetStructNameL5K(String Argument, out String StructName)
        {
            StructName = RemoveArgumentsL5K(Argument).Trim(new char[] { '\r', '\n', '\t', ' ' });
            if (StructName.Length != 0)
                if (!m_mapStruct.ContainsKey(StructName))
                    return true;

            return false;

        }

        string ReadLineL5K(ref System.IO.StreamReader readFile)
        {
            string outString = readFile.ReadLine();
            string aux;
            if (!string.IsNullOrEmpty(outString))
            {
                while (outString.EndsWith(","))
                {
                    aux = readFile.ReadLine();
                    if (string.IsNullOrEmpty(aux))
                        break;
                    outString += aux;
                }
            }
            return outString;

        }

        String GetKeyL5K(String Line)
        {
            String LineOut = "";
            return GetKeyL5K(Line, ref LineOut);
        }

        String GetKeyL5K(String Line, ref String LineOut)
        {
            if (String.IsNullOrWhiteSpace(Line))
            {
                return "";
            }

            String Aux = Line.Trim(new char[] { '\r', '\n', '\t', ' ' });
            LineOut = String.Empty;

            int spaceIndex = Aux.IndexOf(" ");
            if (spaceIndex < 0)
            {
                return Aux;
            }
            else
            {
                LineOut = Aux.Substring(spaceIndex);
                return Aux.Substring(0, spaceIndex);
            }
        }

        String RemoveArgumentsL5K(String Line)
        {
            String aux = Line;
            RemoveArgumentsL5K(ref aux);
            return aux;
        }

        void RemoveArgumentsL5K(ref String Line)
        {
            if (String.IsNullOrWhiteSpace(Line))
            {
                return;
            }

            String szAuxLeft;
            String szAuxRight;
            int CommentInitIndex = Line.IndexOf("(");
            if (CommentInitIndex < 0 || CommentInitIndex >= Line.Length)
                return;
            int CommentEndIndex = Line.LastIndexOf(")");

            if (CommentEndIndex < Line.Length)
            {
                szAuxLeft = Line.Substring(0, CommentInitIndex);
                szAuxRight = Line.Substring(CommentEndIndex + 1);
                Line = szAuxLeft + szAuxRight;
            }
        }

        String GetDescriptionL5K(String Line)
        {
            int nIndex;
            int nIndex2;
            String aux = "";
            GetArgumentsL5K(Line, ref aux);
            nIndex = aux.IndexOf("Description");
            if (nIndex < 0)
                return "";
            nIndex = aux.IndexOf(":=", nIndex);
            if (nIndex < 0)
                return "";
            nIndex = aux.IndexOf("\"", nIndex);
            if (nIndex < 0)
                return "";
            nIndex2 = aux.IndexOf("\"", nIndex + 1);
            if (nIndex2 < 0)
                return "";
            return aux.Substring(nIndex, nIndex2 - nIndex + 1);
        }

        bool GetArgumentsL5K(String Line, ref String Arguments)
        {
            if (String.IsNullOrWhiteSpace(Line))
                return false;

            int CommentInitIndex = Line.IndexOf("(");
            if (CommentInitIndex < 0 || CommentInitIndex >= Line.Length)
                return false;
            int LenArguents = Line.LastIndexOf(")");

            if (LenArguents <= 0)
                return false;

            String aux = Line.Substring(CommentInitIndex + 1, LenArguents - (CommentInitIndex + 1));

            Arguments = aux;

            return true;
        }

        bool StructFoundL5K(string szType)
        {
            string szVarType = szType.Trim(new char[] { '\r', '\n', '\t', ' ' });
            if (szVarType.Length == 0)
                return (false);

            if (m_mapStruct.ContainsKey(szVarType))
            {
                if (m_mapStruct[szVarType].Count == 0)
                    return (false);
                return (true);
            }

            return (false);
        }

        void RemoveStructNameL5K(ref string szStructName, ref string szCurStructName)
        {
            if (szStructName.Length == 0)
            {
                szCurStructName = string.Empty;
                return;
            }

            if (szCurStructName.Length == 0)
            {
                szStructName = string.Empty;
                return;
            }

            int nIndex = szStructName.IndexOf(szCurStructName);
            if (nIndex <= 0)
            {
                szCurStructName = string.Empty;
                szStructName = string.Empty;
                return;
            }

            string szAux = szStructName.Substring(0, nIndex - 1);
            szStructName = szAux;
            szCurStructName = string.Empty;
        }

        bool GetArrayDimL5K(ref String szLine, ref String szLineOut, ref uint nArraySize1, ref uint nArraySize2, ref uint nArraySize3)
        {
            uint[] nArraySize = new uint[3];
            nArraySize[0] = 0;
            nArraySize[1] = 0;
            nArraySize[2] = 0;
            if (!string.IsNullOrWhiteSpace(szLine))
            {
                int Index1 = szLine.IndexOf("[");
                int Index2 = szLine.IndexOf("]");
                if ((Index1 >= 0) && (Index2 > Index1))
                {
                    szLineOut = szLine.Substring(0, Index1).Trim(new char[] { '\r', '\n', '\t', ' ' });

                    String szAux = szLine.Substring(Index1 + 1, Index2 - Index1 - 1).Trim(new char[] { '\r', '\n', '\t', ' ' });
                    String szAux2;

                    for (int i = 0; i < 3; i++)
                    {
                        if (!string.IsNullOrWhiteSpace(szAux))
                        {
                            Index1 = szAux.IndexOf(",");
                            if (Index1 < 0)
                            {
                                szAux2 = szAux;
                                szAux = "";
                            }
                            else
                            {
                                String szAux3 = szAux;
                                szAux = szAux3.Substring(Index1 + 1);
                                szAux2 = szAux3.Substring(0, Index1);
                            }
                            try
                            {
                                uint number = UInt32.Parse(szAux2);
                                nArraySize[i] = number;
                            }
                            catch (FormatException)
                            {
                            }
                        }
                    }
                }
            }
            if (nArraySize[0] + nArraySize[1] + nArraySize[2] > 0)
            {
                nArraySize1 = nArraySize[0];
                nArraySize2 = nArraySize[1];
                nArraySize3 = nArraySize[2];

                return true;
            }
            return false;

        }

        bool ParseDataTypeNameL5K(ref String szUDTName)
        {
            String szAux = RemoveArgumentsL5K(szUDTName).Trim(new char[] { '\r', '\n', '\t', ' ' });

            if (string.IsNullOrWhiteSpace(szAux))
                return false;

            szUDTName = szAux;
            return true;

        }

        bool GetDataTypeDefinitionL5K(ref System.IO.StreamReader fileL5K,
                                                        out String TypeDefinitionBlock)
        {
            // Read the whole type definition
            bool bDefinitionComplete = false;
            TypeDefinitionBlock = "";

            String szFileLine;
            while (!bDefinitionComplete && ((szFileLine = ReadLineL5K(ref fileL5K)) != null))
            {
                if (GetKeyL5K(szFileLine) == ("END_DATATYPE"))
                    bDefinitionComplete = true;
                else
                {
                    //if the variable has the Hidden attribute is set to 1, then the tag cannot be accessed from outside the controller.
                    if (szFileLine.IndexOf("Hidden := 1", StringComparison.InvariantCultureIgnoreCase) != -1)
                    {
                        continue;
                    }
                    TypeDefinitionBlock += szFileLine.Trim(new char[] { '\r', '\n', '\t', ' ' });
                }

            }

            return (bDefinitionComplete);
        }

        bool GetAddOnInstructionDefinitionL5K(ref System.IO.StreamReader fileL5K,
                                                            out String TypeDefinitionBlock)
        {
            // Read the whole type definition
            bool bDefinitionComplete = false;
            bool bDefinitionInit = false;
            TypeDefinitionBlock = "";

            String szFileLine;
            while (!bDefinitionComplete && ((szFileLine = ReadLineL5K(ref fileL5K)) != null))
            {
                if (GetKeyL5K(szFileLine) == ("END_ADD_ON_INSTRUCTION_DEFINITION"))
                    bDefinitionComplete = true;
                else if (!bDefinitionInit)
                {
                    if (GetKeyL5K(szFileLine) == ("PARAMETERS"))
                    {
                        bDefinitionInit = true;
                    }
                    else if (GetKeyL5K(szFileLine) == ("LOCAL_TAGS"))
                    {
                        bDefinitionInit = true;
                    }
                }
                else if (GetKeyL5K(szFileLine) == ("END_PARAMETERS"))
                    bDefinitionInit = false;
                else if (GetKeyL5K(szFileLine) == ("END_LOCAL_TAGS"))
                { 
                    bDefinitionInit = false;
                }
                else
                {
                    if (szFileLine.IndexOf("Hidden := 1", StringComparison.InvariantCultureIgnoreCase) != -1)
                    {
                        continue;
                    }
                    //if the variable has the External Access attribute is set to None, then the tag cannot be accessed from outside the controller.                   
                    if (szFileLine.IndexOf("ExternalAccess := None", StringComparison.InvariantCultureIgnoreCase) != -1)
                    {
                        continue;
                    }
                    if (szFileLine.IndexOf("Usage := InOut", StringComparison.InvariantCultureIgnoreCase) != -1)
                    {
                        continue;
                    }
                    

                    String Aux = szFileLine.Trim(new char[] { '\r', '\n', '\t', ' ' });
                    String Name = GetKeyL5K(Aux, ref Aux);
                    Aux = Aux.Trim(new char[] { '\r', '\n', '\t', ':', ' ' });
                    String Type = GetKeyL5K(Aux, ref Aux);
                    TypeDefinitionBlock += (Type + " " + Name + Aux);
                }

            }

            return (bDefinitionComplete);
        }

        bool IsEndOfStructL5K(string szLine)
        {
            if (szLine.IndexOf("END_DATATYPE") >= 0)
            {
                return true;
            }

            return false;
        }

        //**********************************************************************
        internal void AddTreeItem(ImportData tag, ImportData parent = null)
        {
            string strId = tag.Id.ToString("X6");
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

        #region Import Direct Method
        private ImportDataModel Read_Plc_Info()
        {            
            importDataModel = new ImportDataModelEthernetIP(readStationName);

            DisplayDirectImport();

            return importDataModel;
        }

        private void DisplayDirectImport()
        {
            using (new WaitCursor())
            {
                EtherNetIPStation station = new EtherNetIPStation(new EtherNetIPDriver(conn), (EtherNetIPStationSettings)baseImportTree.GetStationSettings(baseImportTree.ReadStationName()));
                EtherNetIPChannel channel = new EtherNetIPChannel(new EtherNetIPDriver(conn), (EtherNetIPChannelSettings)baseImportTree.GetChannelSettingsFromStation(baseImportTree.ReadStationName()));
                if (channel.EtherNetIPDeviceOpen())
                {
                    byte[] requestBuffer = new byte[EtherNetIpProtocol.TCP_MAX_SEGMENT_SIZE];
                    ushortUnion requestBufferPointer = new ushortUnion(0);
                    if (EtherNetIpProtocol.Logix5550ForwardOpen(station, ref requestBuffer, ref requestBufferPointer, channel))
                    {
                        if (!station.BuildInfoMaps)
                        {
                            if (station.ConnectionIDSet)
                            {
                                station.Logix5550NonBlockPhAddEmptyInfoMaps();
                                if (EtherNetIpProtocol.Logix5000V21BuildInstancesMaps(station, ref requestBuffer, ref requestBufferPointer, channel))
                                {
                                    station.BuildInfoMaps = true;
                                }
                            }
                        }

                        if (station.BuildInfoMaps)
                        {

                            foreach (KeyValuePair<string, PlcTagInstanceInfo> entry in station.m_mapPlcTagInstanceInfo)
                            {

                                // Check the variable type and get the variable size
                                uint nVarSize = 0;
                                uint nArraySize0 = entry.Value.m_nDim0;
                                uint nArraySize1 = entry.Value.m_nDim1;
                                uint nArraySize2 = entry.Value.m_nDim2;

                                string nMovType = "";
                                string szVarType = "";

                                string szVarName;
                                string szAddress = entry.Key;
                                if (szAddress.Contains("Program:"))
                                    szVarName = szAddress.Substring(8);
                                else
                                    szVarName = szAddress;

                                ImportTypes nVariableType = GetVarTypeAndSize(entry.Value.m_nType, ref nVarSize, ref szVarType, ref nMovType, ref nArraySize0);

                                // Add the variable to the dialog list
                                //////////////////////////////////////////////////////////////////////////

                                if (nVariableType != ImportTypes.Unknown)
                                {
                                    AddVariable(szVarName, szVarType, nVarSize, nArraySize0, nArraySize1, nArraySize2, nMovType, nVariableType, ImportTypes.Standard, szAddress, entry.Value.m_nType, station);
                                }
                            }
                        }
                        EtherNetIpProtocol.Logix5550ForwardClose(station, ref requestBuffer, ref requestBufferPointer, channel);
                    }

                    station.Logix5550NonBlockPhAddEmptyInfoMaps();
                    EtherNetIpProtocol.UnRegisterSession(ref requestBuffer, ref requestBufferPointer, channel);
                    channel.DeviceClose();

                    if (importDataModel == null || importDataModel.Children.Count == 0)
                    {
                        MessageBox.Show(Properties.Resources.ErrorImportConnectToDevice, Properties.Resources.ImportMsgBoxTitle);
                        importDataModel = null;
                    }
                }
                else
                {
                    MessageBox.Show(Properties.Resources.ErrorImportConnectToDevice, Properties.Resources.ImportMsgBoxTitle);
                    importDataModel = null;
                }

                station.Dispose();
                channel.Dispose();
            } 
        }

        private void AddVariable(string szVarName, string szVarType, uint nVarSize, uint nArraySize0, uint nArraySize1, uint nArraySize2, string nMovType, ImportTypes nVariableType, ImportTypes arrayType, string szAddress, ushort templateIntance, EtherNetIPStation station)
        {
            switch (nVariableType)
            {
                // Variable of standard type
                case ImportTypes.Standard:
                    AddStandardVar("", szVarName, "", szAddress,
                            "", szVarType, nMovType, nVarSize);
                    break;

                // Structure or enumeration
                case ImportTypes.Struct:
                    AddStruct("", szVarName, "", szAddress, (ushort)(templateIntance & 0xfff), station);
                    break;

                // Array
                case ImportTypes.Array:
                    AddArray("", szVarName, "", szAddress,
                        "", szVarType, nVarSize, nArraySize0,
                        nArraySize1, nArraySize2, arrayType, nMovType, (ushort)(templateIntance & 0xfff), station);
                    break;
                case ImportTypes.ArrayOfStructures:
                    AddArray("", szVarName, "", szAddress,
                        "", szVarType, nVarSize, nArraySize0,
                        nArraySize1, nArraySize2, ImportTypes.Struct, nMovType, (ushort)(templateIntance & 0xfff), station);
                    break;
            }
        }

        ImportTypes TagType(ushort nVarType, ref uint nVarSize, ref String szVarType, ref String nType)
        {
            ImportTypes exit = ImportTypes.Standard;
            nType = "";
            if ((nVarType & 0x8000) == 0x8000)
            {
                // String
                if (nVarType == 0x8FCE)
                {
                    nType = "String";
                    szVarType = "STRING";
                    nVarSize = EtherNetIpProtocol.STRING_MAX_LENGHT;
                }
                //Array of string
                else if ((nVarType == 0xAFCE) || (nVarType == 0xCFCE) || (nVarType == 0xEFCE))
                {
                    exit = ImportTypes.Array;
                    szVarType = "STRING";
                    nType = "String";
                    nVarSize = EtherNetIpProtocol.STRING_MAX_LENGHT;
                }
                // Array of structure?
                else if ((nVarType & 0x6000) != 0x0)
                {
                    exit = ImportTypes.ArrayOfStructures;
                }

                // Structure
                else
                {
                    exit = ImportTypes.Struct;
                }
            }

            // Array with elements of atomic type?
            else if ((nVarType & 0x6000) != 0x0)
            {
                exit = ImportTypes.Array;
            }

            // Atomic tag
            return exit;

        }
        ImportTypes GetVarTypeAndSize(ushort nVarType, ref uint nVarSize, ref String szVarType, ref String nType, ref uint nArraySize0)
        {
            ImportTypes exit = TagType(nVarType, ref nVarSize, ref szVarType, ref nType);
            if (exit == ImportTypes.Struct || exit == ImportTypes.ArrayOfStructures || nType == "String")
            {
                return (exit);
            }
            ushort nTypeProt = (ushort)(nVarType & 0x00FF);

            if (nTypeProt == 0xC1)
            {
                szVarType = "BOOL";
                nType = "Boolean";
                nVarSize = 1;
            }
            else if (nTypeProt == 0xC2)
            {
                szVarType = "SINT";
                nType = "SByte";
                nVarSize = 1;
            }
            else if (nTypeProt == 0xC3)
            {
                szVarType = "INT";
                nType = "Int16";
                nVarSize = 2;
            }
            else if (nTypeProt == 0xC4)
            {
                szVarType = "DINT";
                nType = "Int32";
                nVarSize = 4;
            }
            else if (nTypeProt == 0xD3)
            {
                szVarType = "ARRAYOF32BITS";
                nType = "Boolean";
                nVarSize = 4;
                exit = ImportTypes.Array;
                nArraySize0 = 32;
            }
            else if (nTypeProt == 0xCA)
            {
                szVarType = "REAL";
                nType = "Float";
                nVarSize = 4;
            }
            else if (nTypeProt == 0xC5)
            {
                szVarType = "LINT";
                nType = "Int64";
                nVarSize = 8;
            } else
            {
                // unsupported datatype
                exit = ImportTypes.Unknown;
            }

            return (exit);
        }

        string GetVarType(uint nTypeProt)
        {
            string szType = string.Empty;
            if (nTypeProt == 0)
            {
                return (szType);
            }
            if (nTypeProt == 0xC1)
            {
                szType = ("BOOL");
            }
            else if (nTypeProt == 0xC2)
            {
                szType = ("SINT");
            }
            else if (nTypeProt == 0xC3)/*INT*/
            {
                szType = ("INT");
            }
            else if (nTypeProt == 0xC4)/*DINT*/
            {
                szType = ("DINT");
            }
            else if (nTypeProt == 0xD3) /*DWORD*/
            {
                szType = ("BOOLEAN ARRAY");
            }
            else if (nTypeProt == 0xCA) /*REAL*/
            {
                szType = ("REAL");
            }
            else if (nTypeProt == 0xC5) /*LINT*/
            {
                szType = ("LINT");
            }

            return (szType);
        }

        bool AddStruct(String szNamePrefix, String szVarName, String szAddressPrefix, String szAddress,
            ushort templateIntance, EtherNetIPStation station, int parentId = -1, ImportData inRootItem = null)
        {
            if (!station.m_mapPlcTemplateInfo.ContainsKey(templateIntance))
                return false;
            if (station.m_mapPlcTemplateInfo[templateIntance].m_mapFieldInfo.Count == 0)
                return false;

            string szVarType = station.m_mapPlcTemplateInfo[templateIntance].m_mapFieldInfo.First().Value.m_szTemplateName;

            String szStructNamePrefix = szNamePrefix + szVarName + ".";
            String szStructAddressPrefix = szAddressPrefix + szAddress + ".";

            int lParentID = parentId;

            uint nFieldSize = 0;

            if (inRootItem == null)
                lParentID = -1;

            ImportData rootItem = importDataModel.addImportData();
            rootItem.parentId = lParentID;
            lParentID = m_lGlobalID++;
            rootItem.Id = lParentID;
            ((ImportDataEthernetIP)rootItem).PreName = szNamePrefix;
            rootItem.Name = szVarName;
            rootItem.szType = "STRUCT";
            ((ImportDataEthernetIP)rootItem).ElemType = szVarType;
            ((ImportDataEthernetIP)rootItem).Type = DataType.Boolean;
            rootItem.Address = szAddressPrefix + szAddress;
            rootItem.Description = "";
            ((ImportDataEthernetIP)rootItem).Size = nFieldSize;
            rootItem.ArrayDimension = 0;

            String szFieldName;
            String szFieldType;
            string nMovType = "";
            uint nArraySize0 = 0;
            ImportTypes nFieldType = ImportTypes.Unknown;

            bool isFirst = true;

            foreach (KeyValuePair<string, PlcTemplateFieldInfo> entry in station.m_mapPlcTemplateInfo[templateIntance].m_mapFieldInfo)
            //foreach (string itemStructValue in m_mapStruct[szVarType])
            {
                if (entry.Value.m_szName.Contains("ZZZZZZZZZZ"))
                    continue;

                nFieldSize = 0;
                szFieldType = "";
                nArraySize0 = entry.Value.m_nDim0;
                nFieldType = GetVarTypeAndSize(entry.Value.m_nType, ref nFieldSize, ref szFieldType, ref nMovType, ref nArraySize0);
                szFieldName = entry.Value.m_szName;

                // Add the field to the dialog list
                switch (nFieldType)
                {
                    // Variable of standard type
                    case ImportTypes.Standard:
                        //if (nMovType != "String")
                        //{
                            AddStructRoot(inRootItem, rootItem, ref isFirst, szAddress, szVarType);
                            AddStandardVar(szStructNamePrefix, szFieldName, "", szStructAddressPrefix + szFieldName,
                                "", szFieldType, nMovType,
                                nFieldSize, rootItem.Id, rootItem);
                        //}
                        //else
                        //{
                        //    AddStandardVar("", szStructNamePrefix + szFieldName, "", szStructAddressPrefix + szFieldName,
                        //        "", szFieldType, nMovType, nFieldSize);

                        //}
                        break;
                    // Array
                    case ImportTypes.Array:
                        if (nMovType != "String")
                        {
                            AddStructRoot(inRootItem, rootItem, ref isFirst, szAddress, szVarType);
                            AddArray(szStructNamePrefix, szFieldName, szStructAddressPrefix, szFieldName,
                                "", szFieldType, nFieldSize, nArraySize0, 0, 0, ImportTypes.Standard, nMovType,
                                (ushort)(entry.Value.m_nTemplateInstance & 0xfff), station, lParentID, rootItem);
                        }
                        else
                        {
                            AddArray("", szStructNamePrefix + szFieldName, "", szStructAddressPrefix + szFieldName,
                                    "", szFieldType, nFieldSize, nArraySize0, 0, 0, ImportTypes.Standard, nMovType,
                                    (ushort)(entry.Value.m_nTemplateInstance & 0xfff), station);
                        }
                        break;
                    case ImportTypes.ArrayOfStructures:
                        AddArray("", szStructNamePrefix + szFieldName, "", szStructAddressPrefix + szFieldName,
                                "", szFieldType, nFieldSize, nArraySize0, 0, 0, ImportTypes.Struct, nMovType,
                                (ushort)(entry.Value.m_nTemplateInstance & 0xfff), station);
                        break;
                    // Structure 
                    case ImportTypes.Struct:
                        AddStructRoot(inRootItem, rootItem, ref isFirst, szAddress, szVarType);
                        AddStruct(szStructNamePrefix, szFieldName, szStructAddressPrefix, szFieldName,
                            (ushort)(entry.Value.m_nTemplateInstance & 0xfff), station, lParentID, rootItem);

                        break;
                }
            }


            return true;
        }

        void AddArray(String szNamePrefix, String szArrayName, String szAddressPrefix,
                String szArrayAddress, String description, String szElemType, uint nElemSize,
                uint nArrayDim0, uint nArrayDim1, uint nArrayDim2, ImportTypes arrayType,
                String nMovType, ushort templateIntance, EtherNetIPStation station, int parentId = -1, ImportData inRootItem = null)
        {

            if (nArrayDim0 == 0)
            {
                return;
            }

            String szName, szName1;
            String szAddress, szAddress1;
            if (nArrayDim1 == 0)
            {
                AddArray1Dim(ref szNamePrefix, ref szArrayName, ref szAddressPrefix, ref szArrayAddress, ref description,
                    ref szElemType, nElemSize, nArrayDim0, 0, arrayType, nMovType, templateIntance, station, parentId, inRootItem);
            }
            else if (nArrayDim2 == 0)
            {
                for (uint nIdx = 0; nIdx < nArrayDim0; nIdx++)
                {
                    szName = string.Format("{0}[{1}", szArrayName, nIdx);
                    szAddress = string.Format("{0}[{1}", szArrayAddress, nIdx);
                    AddArray1Dim(ref szNamePrefix, ref szName, ref szAddressPrefix, ref szAddress, ref description,
                        ref szElemType, nElemSize, nArrayDim1, 1, arrayType, nMovType, templateIntance, station, parentId, inRootItem);
                }
            }
            else
            {
                for (uint nIdx2 = 0; nIdx2 < nArrayDim0; nIdx2++)
                {
                    for (uint nIdx = 0; nIdx < nArrayDim1;
                        nIdx++)
                    {
                        szName1 = string.Format("{0}[{1},{2}", szArrayName, nIdx2, nIdx);
                        szAddress1 = string.Format("{0}[{1},{2}", szArrayAddress, nIdx2, nIdx);
                        AddArray1Dim(ref szNamePrefix, ref szName1, ref szAddressPrefix, ref szAddress1,
                            ref description, ref szElemType, nElemSize, nArrayDim2,
                                2, arrayType, nMovType, templateIntance, station, parentId, inRootItem);
                    }
                }
            }
        }

        void AddArray1Dim(ref String szNamePrefix, ref String szArrayName,
                            ref String szAddressPrefix, ref String szAddress, ref String description,
                            ref String szElemType, uint nElemSize, uint nArrayDim,
                            uint nDimIndex, ImportTypes arrayType, String nMovType, ushort templateIntance, EtherNetIPStation station, int parentId = -1, ImportData inRootItem = null)
        {
            if (nElemSize * nArrayDim > EtherNetIpProtocol.MAX_SEGMENT_DATA_SIZE && arrayType == ImportTypes.Standard)
            {
                uint nArrayBlockSize = EtherNetIpProtocol.MAX_SEGMENT_DATA_SIZE / nElemSize;
                uint nArrayBlocks = (nArrayDim + nArrayBlockSize - 1) / nArrayBlockSize;
                uint nArrayBlock = 1;
                uint nArrayStart = 0;
                while (nArrayStart < nArrayDim)
                {
                    if (nArrayStart + nArrayBlockSize > nArrayDim)
                        nArrayBlockSize = nArrayDim - nArrayStart;
                    AddArray1DimDisplace(ref szNamePrefix, ref szArrayName,
                        ref szAddressPrefix, ref szAddress, ref description,
                        ref szElemType, nElemSize, nArrayBlockSize,
                        nDimIndex, nArrayStart, nArrayBlock, nArrayBlocks, arrayType, nMovType, templateIntance, station, parentId, inRootItem);
                    nArrayBlock++;
                    nArrayStart += nArrayBlockSize;
                }

                return;
            }
            else
            {
                AddArray1DimDisplace(ref szNamePrefix, ref szArrayName,
                    ref szAddressPrefix, ref szAddress, ref description,
                    ref szElemType, nElemSize, nArrayDim,
                    nDimIndex, 0, 1, 1, arrayType, nMovType, templateIntance, station, parentId, inRootItem);
            }
        }

        void AddArray1DimDisplace(ref String szNamePrefix, ref String szArrayName,
                            ref String szAddressPrefix, ref String szAddress, ref String description,
                            ref String szElemType, uint nElemSize, uint nArrayDim,
                            uint nDimIndex, uint nArrayStart, uint nArrayBlock, uint nArrayBlocks, ImportTypes arrayType,
                            String nMovType, ushort templateIntance, EtherNetIPStation station, int parentId = -1, ImportData inRootItem = null)
        {

            if (nArrayDim == 0)
            {
                return;
            }

            uint nElementSize = 0;


            if ((nMovType == "String") && (nElemSize > 0))
            {
                nElementSize = nElemSize;
            }

            ImportData IVar = null;
            int IVarId = -1;

            String szAux;


            if (arrayType == ImportTypes.Standard && nMovType != "String")
            {
                IVar = importDataModel.addImportData();

                String szName = szArrayName;
                if (nDimIndex > 0)
                {
                    szName += "]";
                }
                if (nArrayBlocks > 1)
                {
                    szAux = string.Format("_{0}_of_{1}", nArrayBlock, nArrayBlocks);
                    szName += szAux;
                }
                ((ImportDataEthernetIP)IVar).PreName = szNamePrefix;
                IVar.Name = szName;

                ((ImportDataEthernetIP)IVar).ElemType = szElemType;
                ((ImportDataEthernetIP)IVar).Type = (DataType)Enum.Parse(typeof(DataType), nMovType);
                IVar.szType = "ARRAY";
                ((ImportDataEthernetIP)IVar).ImportType = ImportTypes.Array;

                IVar.ArrayDimension = nArrayDim;
                ((ImportDataEthernetIP)IVar).Size = nArrayDim * nElementSize;

                if (nDimIndex == 0)
                    szAux = string.Format("[{0}]", nArrayStart);
                else
                    szAux = string.Format(",{0}]", nArrayStart);

                IVar.Address = szAddressPrefix + szAddress + szAux;
                IVar.Description = description;
                IVar.parentId = parentId;
                IVarId = IVar.Id = m_lGlobalID++;
                AddTreeItem(IVar, inRootItem);

            }

            String szFieldLine;
            uint i = 0;
            for (i = 0; i < nArrayDim; i++)
            {
                szFieldLine = szArrayName;
                if (nDimIndex == 0)
                {
                    szAux = string.Format("[{0}]", i + nArrayStart);
                }
                else
                {
                    szAux = string.Format(",{0}]", i + nArrayStart);
                }
                szFieldLine += szAux;

                if (arrayType == ImportTypes.Standard && nMovType != "String")
                {
                    AddStandardVar(szNamePrefix, szFieldLine, szAddressPrefix, szAddress + szAux,
                        description, szElemType, nMovType, nElementSize, IVar.Id, IVar);

                }
                else if (arrayType == ImportTypes.Struct)
                {
                    AddStruct(szNamePrefix, szFieldLine, szAddressPrefix, szAddress + szAux, templateIntance, station);
                }
                else if (arrayType != ImportTypes.Unknown)
                {
                    if (szNamePrefix == "")
                        AddStandardVar("", szFieldLine, szAddressPrefix, szAddress + szAux,
                            description, szElemType, nMovType, nElementSize);
                    else
                        AddStandardVar("", szNamePrefix + "." + szFieldLine, szAddressPrefix, szAddress + szAux,
                            description, szElemType, nMovType, nElementSize);

                }
            }
            return;
        }

        #endregion

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
                    baseImportTree.GetPlcTags -= Read_Plc_Info;
                    baseImportTree.LoadImportFile -= LoadFile;
                    baseImportTree.StationChanged -= CmbStationChanged;

                    baseImportTree.Dispose();
                    baseImportTree = null;
                }
            }
        }
        #endregion
    }

    public class ImportDataEthernetIP : ImportData, IDisposable
    {
        public ImportDataEthernetIP(ImportDataModel inDataModel)
            : base(inDataModel)
        {
            _AddressMode = AddressTypes.TagName;
            dataModelEthernetIP = inDataModel as ImportDataModelEthernetIP;
        }
        private ImportDataModelEthernetIP dataModelEthernetIP { get; set; }
        
        private string _PreName;
        public string PreName
        {
            get { return _PreName; }
            set { _PreName = value; }
        }
        private string _ElemType;
        public string ElemType
        {
            get { return _ElemType; }
            set { _ElemType = value; }
        }
        private DataType _Type;
        public DataType Type
        {
            get { return _Type; }
            set { _Type = value; }
        }
        private ImportTypes _ImportType;
        public ImportTypes ImportType
        {
            get { return _ImportType; }
            set { _ImportType = value; }
        }
        
        private uint _Size;
        public uint Size
        {
            get { return _Size; }
            set { _Size = value; }
        }
        private string _Name;
        public override string Name
        {
            get
            {
                return dataModelEthernetIP.getStationName() != "_" ?
                       dataModelEthernetIP.getStationName() + _Name : _Name;
            }
            set { _Name = value; }
        }
        
        private AddressTypes _AddressMode;
        public AddressTypes AddressMode
        {
            get { return _AddressMode; }
            set { _AddressMode = value; }
        }

        public override DataType IconTagType
        {
            get { return _Type; }
        }

        public void Dispose()
        {
            foreach (ImportData el in Children)
            {
                ImportDataEthernetIP els7 = el as ImportDataEthernetIP;
                if (els7 != null)
                {
                    els7.Dispose();
                }
            }
            Children.Clear();
            //Children = null;
        }
    }

    public class ImportDataModelEthernetIP : ImportDataModel, IDisposable
    {
        public ImportDataModelEthernetIP(GetStationName inGetStationName) :
            base(inGetStationName)
        {
        }

        public override ImportData addImportData()
        {
            ImportDataEthernetIP importData = new ImportDataEthernetIP(this);
            return importData;
        }

        public void Dispose()
        {
            foreach (ImportData el in Children)
            {
                ImportDataEthernetIP els7 = el as ImportDataEthernetIP;
                if (els7 != null)
                {
                    //els7.Dispose();
                }
            }
            Children.Clear();
            Children = null;
        }
    }

    public class ReadExcel
    {
        public static bool ReadExcelFile(String p_strFilePath, ref List<string[]> ListXlsRecord)
        {
            DataSet ds;
            var extension = Path.GetExtension(p_strFilePath).ToLower();
            FileStream stream = File.Open(p_strFilePath, FileMode.Open, FileAccess.Read);
            IExcelDataReader excelReader = null;

            if (extension == ".xls")
            {
                //1. Reading from a binary Excel file ('97-2003 format; *.xls)
                excelReader = ExcelReaderFactory.CreateBinaryReader(stream);
            }
            else if (extension == ".xlsx")
            {
                //2. Reading from a OpenXml Excel file (2007 format; *.xlsx)
                excelReader = ExcelReaderFactory.CreateOpenXmlReader(stream);
            }
            //else if (extension == ".csv")
            //{
            //    excelReader = ExcelReaderFactory.CreateCsvReader(stream);
            //}
            else
            {
                return false;
            }

            //3. DataSet - The result of each spreadsheet will be created in the result.Tables
            ds = excelReader.AsDataSet(new ExcelDataSetConfiguration()
            {
                UseColumnDataType = false,
                ConfigureDataTable = (tableReader) => new ExcelDataTableConfiguration()

            });

            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                ListXlsRecord.Add(dr.ItemArray.Select(c => c.ToString()).ToArray());
            }

            return ListXlsRecord.Count() > 0;
        }
    }    
}
