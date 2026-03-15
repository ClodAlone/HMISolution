using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Utilities;
using System.Reflection;
using UFUAModel;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Accon.AGLink;
using Accon.Symbolik;
using DriverCodeBaseEx.UI;
using DriverCodeBaseEx.UI.Controls;
using S7ImportParser;

namespace S7TCP.UI
{
    /// <summary>
    /// Interaction logic for ImportTagsEditorTree.xaml
    /// </summary>
    public enum ImportTypes
    {
        Unknown,
        Standard,
        Array,
        Struct,
    }

    public delegate bool GetAddDBnumber();
    public struct ImportVariable
    {
        public String szName;
        public String szDescription;
        public DataType nType;
        public String szType;
        public String szAddress;
        public uint nSize;
        public uint ArrayDimension;
        public long lID;
        public long lParent;
        public DataType nElemType;
        public String szPreName;
        public uint valMinAbsOp;
        public uint valMaxAbsOp;
        public UInt64 structKeyCode;
        public bool structFull;
        public ImportTypes ImportType;
        public Dictionary<uint, long> m_MapArrayVariable;
    };

    public partial class ImportTagsEditorTree : UserControl, IDisposable
    {
        static String[] FormatTxt =
        {
            "VOID",                   // #define  AGLSYM_FORMAT_VOID         0
        	"BOOL",                   // #define  AGLSYM_FORMAT_BOOL         1
        	"BYTE",                   // #define  AGLSYM_FORMAT_BYTE         2
        	"CHAR",                   // #define  AGLSYM_FORMAT_CHAR         3
        	"WORD",                   // #define  AGLSYM_FORMAT_WORD         4
        	"INT",                    // #define  AGLSYM_FORMAT_INT          5
        	"DWORD",                  // #define  AGLSYM_FORMAT_DWORD        6
        	"DINT",                   // #define  AGLSYM_FORMAT_DINT         7
        	"REAL",                   // #define  AGLSYM_FORMAT_REAL         8
        	"",//	"DATE",                   // #define  AGLSYM_FORMAT_DATE         9
        	"",//	"TIME OF DAY",            // #define  AGLSYM_FORMAT_TIMEOFDAY   10
        	"",//	"TIME",                   // #define  AGLSYM_FORMAT_TIME        11
        	"S5TIME",                 // #define  AGLSYM_FORMAT_S5TIME      12
        	"",                    //                                    13
        	"",//	"DATE_AND_TIME",          // #define  AGLDBSYM_FORMAT_DATEANDTIME 14
        	"",                    //                                    15
        	"",                    //                                    16
        	"",                    //                                    17
        	"",                    //                                    18
        	"STRING",                 // #define  AGLDBSYM_FORMAT_STRING    19
        	"",//	"POINTER",                // #define  AGLDBSYM_FORMAT_POINTER   20
        	"",                    //                                    21
        	"",//	"ANY",                    // #define  AGLDBSYM_FORMAT_ANY       22
        	"",//	"BLOCK_FB",               // #define  AGLSYM_FORMAT_BLOCKFB     23
        	"",//	"BLOCK_FC",               // #define  AGLSYM_FORMAT_BLOCKFC     24
        	"",//	"BLOCK_DB",               // #define  AGLSYM_FORMAT_BLOCKDB     25
        	"",//	"BLOCK_SDB",              // #define  AGLSYM_FORMAT_BLOCKSDB    26
        	"",                    //                                    27
        	"COUNTER",                // #define  AGLSYM_FORMAT_COUNTER     28
        	"TIMER",                   // #define  AGLSYM_FORMAT_TIMER       29
        };

        static uint[] FormatKeyCode =
        {
            3,                   // #define  AGLSYM_FORMAT_VOID         0
        	5,                   // #define  AGLSYM_FORMAT_BOOL         1
        	7,                   // #define  AGLSYM_FORMAT_BYTE         2
        	11,                   // #define  AGLSYM_FORMAT_CHAR         3
        	13,                   // #define  AGLSYM_FORMAT_WORD         4
        	17,                    // #define  AGLSYM_FORMAT_INT          5
        	19,                  // #define  AGLSYM_FORMAT_DWORD        6
        	23,                   // #define  AGLSYM_FORMAT_DINT         7
        	29,                   // #define  AGLSYM_FORMAT_REAL         8
        	31,                   // #define  AGLSYM_FORMAT_DATE         9
        	37,            // #define  AGLSYM_FORMAT_TIMEOFDAY   10
        	41,                   // #define  AGLSYM_FORMAT_TIME        11
        	43,                 // #define  AGLSYM_FORMAT_S5TIME      12
        	1,                    //                                    13
        	1,//	"DATE_AND_TIME",          // #define  AGLDBSYM_FORMAT_DATEANDTIME 14
        	1,                    //                                    15
        	1,                    //                                    16
        	1,                    //                                    17
        	1,                    //                                    18
        	47,                 // #define  AGLDBSYM_FORMAT_STRING    19
        	1,//	"POINTER",                // #define  AGLDBSYM_FORMAT_POINTER   20
        	1,                    //                                    21
        	1,//	"ANY",                    // #define  AGLDBSYM_FORMAT_ANY       22
        	1,//	"BLOCK_FB",               // #define  AGLSYM_FORMAT_BLOCKFB     23
        	1,//	"BLOCK_FC",               // #define  AGLSYM_FORMAT_BLOCKFC     24
        	1,//	"BLOCK_DB",               // #define  AGLSYM_FORMAT_BLOCKDB     25
        	1,//	"BLOCK_SDB",              // #define  AGLSYM_FORMAT_BLOCKSDB    26
        	1,                    //                                    27
        	53,                // #define  AGLSYM_FORMAT_COUNTER     28
        	59,                // #define  AGLSYM_FORMAT_TIMER       29
        };

        bool alreadyLoaded = false;
        ImportDataModelS7Tcp importDataModel;
        int m_lGlobalID = 0;
        Dictionary<string, List<string>> m_mapUDT;
        public GetStationName readStationName;
        public GetAddDBnumber readAddDBnumber;

        int pPrjHandle;
        Dictionary<string, ImportVariable> m_MapImportVariableStruct;
        Dictionary<string, ImportVariable> m_MapImportVariableArray;
        Dictionary<string, List<string>> m_mapUDTBaseS7P;
        Dictionary<long, ImportVariable> m_MapImportVariable;

        bool isS7P;
        bool isTIAPortal;        
        Dictionary<string, string> m_MapValueDepthStructToNumStruct;
        BaseImportTree baseImportTree;

        private S7TIAImportParser pareserTia;
        string conn;
        string invalidNamePrefix;

        private uint MAX_DATA_BYTES = 212; //--> moved into S7Protocol() CTor() 

        public ImportTagsEditorTree()
        {
            InitializeComponent();
            Loaded += (o, e) =>
            {
                SetProgramVisibility(Visibility.Hidden);
                AddDBnumber.Visibility = System.Windows.Visibility.Hidden;
                CmbProgram.Text = "";
                pPrjHandle = -1;

                isS7P = false;
                isTIAPortal = false;

                if (alreadyLoaded)
                {                                        
                    importDataModel = null;
                    AddDBnumber.IsChecked = true;                 
                    return;
                }
                alreadyLoaded = true;

                List<string> lista = DataContext as List<string>;
                if (lista == null || lista.Count < 2)
                    return;

                conn = lista[0];// DataContext as string;

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
                baseImportTree.SetVisibleButtons(bGetPLCTags: true);
                baseImportTree.SetFileFilter("TIA Files(*.ap11 *.ap12 *.ap13 *.ap14 *.ap15 *.ap15_1 *.ap16 *.ap17 *.ap18)|*.ap11;*.ap12;*.ap13;*.ap14;*.ap15;*.ap15_1;*.ap16;*.ap17;*.ap18|s7p files|*.s7p|awl files|*.awl|sdf files|*.sdf");
                baseImportTree.LoadImportFile = LoadFile;
                baseImportTree.GetPlcTags = Read_Plc_Info;
                MainStack.Children.Add(baseImportTree);

                AddDBnumber.IsChecked = true;
                readAddDBnumber = () => AddDBnumber.IsChecked == true;
                DependencyPropertyDescriptor descriptor =
                   DependencyPropertyDescriptor.FromProperty(CheckBox.IsCheckedProperty, typeof(CheckBox));
                descriptor.AddValueChanged(AddDBnumber, AddDBnumber_Click);

                readStationName = baseImportTree.funcGetStationName();
                
                DataContext = this;

                m_MapImportVariableStruct = new Dictionary<string, ImportVariable>();
                m_MapImportVariableArray = new Dictionary<string, ImportVariable>();
                m_mapUDTBaseS7P = new Dictionary<string, List<string>>();
                m_MapImportVariable = new Dictionary<long, ImportVariable>();

                m_mapUDT = new Dictionary<string, List<string>>();

                //conn = lista[0];// DataContext as string;
                invalidNamePrefix = Properties.Settings.Default.INVALID_NAME_PREFIX;
            };
        }

        private void AddDBnumber_Click(object sender, EventArgs e)
        {
            baseImportTree.UpdateDataWithInterfaceParameters();
        }

        private static void BuildStringLengths(ImportData elem, string stringParentInfo, ref string stringLengthInfo)
        {
            if (elem.Children.Count != 0)
            {
                foreach (ImportData el in elem.Children)
                {
                    if (el == null)
                    {
                        continue;
                    }
                    if (el.Children.Count > 0)
                    {                        
                        BuildStringLengths(el, string.Format("{0}{1}.", stringParentInfo, el.Name), ref stringLengthInfo);
                        continue;
                    }
                    if (((ImportDataS7Tcp)el).Type != DataType.String)
                    {
                        continue;
                    }

                    int colonIndex = el.Address.IndexOf(':');
                    if (colonIndex < 1)
                    {
                        continue;
                    }
                    string szStringSize = el.Address.Substring(colonIndex + 1);
                    uint stringSize = 0;
                    if (uint.TryParse(szStringSize, out stringSize) != true)
                    {
                        continue;
                    }
                    if (stringSize == 0)
                    {
                        continue;
                    }

                    // Valid length --> Add it to stringLengthInfo 

                    // Build the complete name of the structure element
                    //string elementName = String.Format("{0}.{1}", elem.Name, el.Name);
                    string elementName = String.Format("{0}{1}", stringParentInfo, el.Name);

                    // Add the length of the string to stringLengthInfo
                    if (!String.IsNullOrWhiteSpace(stringLengthInfo))
                    {
                        stringLengthInfo += ";";
                    }
                    stringLengthInfo += elementName + ":" + stringSize;
                }
            }
        }

        // Used to check if all the strings of a structure have the same length
        void SetStructStringLengthsShortForm(ref string stringLengths)
        {
            string[] arrayOfStrings = stringLengths.Split(';');
            Int64 commonLength = 0;
            foreach (string elem in arrayOfStrings)
            {
                int index = elem.IndexOf(":");
                if (index > 0)
                {
                    string lengthString = elem.Substring(index + 1);
                    if (!String.IsNullOrWhiteSpace(lengthString))
                    {
                        Int64 stringSize = 0;
                        if (Int64.TryParse(lengthString, out stringSize))
                        {
                            if (stringSize > 0)
                            {
                                if (commonLength == 0)
                                {
                                    commonLength = stringSize;
                                }
                                else
                                {
                                    if (commonLength != stringSize)
                                    {
                                        commonLength = 0;
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
            }

            if (commonLength == 0)
            {
                return;
            }

            // All the strings have the same length, so set the short form of the info: ":<common string length>"
            stringLengths = String.Format(":{0}", commonLength);
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
                        Dictionary<string, ImportPrototype> protoMap = new Dictionary<string, ImportPrototype>();
                        ObservableCollection<object> notToBeImported = new ObservableCollection<object>();
                        foreach (var elemList in list)
                        {
                            progressBar.ImportNewTag(elemList.Name);
                            progressBar.ThrowIfCancellationRequested();

                            ImportData elem = elemList as ImportData;
                            if (elem != null && (notToBeImported.IndexOf(elem) < 0))
                            {                                
                                bool isStructure = elem.Children.Count > 0;
                                ImportData single = elem;

                                // FOGBUGZ 11412 and 11413
                                bool isArray = false;
                                if (single.ArrayDimension > 0)
                                {
                                    isStructure = false;
                                    isArray = true;
                                }

                                if (single != null)
                                {

                                    S7TCPDynTagSettings sp = new S7TCPDynTagSettings();

                                    if (!sp.ParseAddress(single.Address))
                                        continue;

                                    if (((ImportDataS7Tcp)single).Type == DataType.String)
                                        sp.LenStringEnable = true;

                                    sp.StationName = stationName;

                                    // FOGBUGZ 11412 and 11413
                                    sp.ArrayDimension = single.ArrayDimension;

                                    single.DynAddress = sp.ToString();

                                    ImportPrototype proto = null;
                                    if (isStructure)
                                    {
                                        string stringParentInfo = String.Empty;
                                        string stringLengthInfo = String.Empty;
                                        BuildStringLengths(single, stringParentInfo, ref stringLengthInfo);
                                        if (!String.IsNullOrWhiteSpace(stringLengthInfo))
                                        {
                                            // If all the strings of the structure have the same length, set the short form for this info
                                            SetStructStringLengthsShortForm(ref stringLengthInfo);

                                            sp.StructStringFieldLengths = stringLengthInfo;
                                        }

                                        proto = addPrototype(protoMap, single, invalidNamePrefix, progressBar);
                                        if (proto == null)
                                            continue;
                                    }
                                    single.DynAddress = sp.ToString();

                                    // FOGBUGZ 11412 and 11413
                                    string importTagName = single.TreeName.Trim(']');
                                    //importTagName = UFUAModel.Helpers.NameValidator.EnsureValidName(importTagName);

                                    ImportTag tagtoimport = new ImportTag()
                                    {
                                        // FOGBUGZ 11412 and 11413
                                        //Name = single.Name,
                                        Name = importTagName,

                                        DataType = ((ImportDataS7Tcp)single).Type,
                                        DynSettings = single.DynAddress,
                                        Folder = importfolder,
                                        ModelType = (isStructure ? UFUAModel.ModelType.ObjectType : UFUAModel.ModelType.Variable),
                                        Description = single.Description

                                        // FOGBUGZ 11412 and 11413
                                        ,
                                        ArrayDimension = single.ArrayDimension
                                        ,
                                        BehaviorForExistingTags = (BehaviorExistingTagsValues)behaviorExistingTags,
                                        BehaviorForDynamicLink = (BehaviorDynamicLinkValues)behaviorDynamicLink,
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
                        }
                        DataContext = new ImportObject() { PrototypesToImport = protoMap.Values.ToList(), TagsToImport = taglist, SearchForCompatiblePrototypes = true };
                    }
                    catch (OperationCanceledException ex)
                    {
                        //UFUAServerDocument.logGeneral.Info(Properties.Resources.TagsImportCanceled);
                    }
                }
            }            
        }

        private static ImportPrototype addPrototype(Dictionary<string, ImportPrototype> protoMap, ImportData elem, string invNamePrefix, ImportDataProgressBar progressBar)
        {
            progressBar.ThrowIfCancellationRequested();
            progressBar.UpdateImportingTag(elem.Name);

            ImportPrototype proto = null;
            string protoname;
            if (elem.szType.ToUpper() == "STRUCT")
                protoname = string.Format("prototype_{0}", elem.Name.Trim(']'));
            else
                protoname = elem.szType;
            if (!protoMap.ContainsKey(protoname))
            {
                if (elem.Children.Count != 0)
                {
                    proto = new ImportPrototype();
                    proto.Name = UFUAModel.Helpers.NameValidator.EnsureValidName(protoname);
                    proto.Elements = new List<ImportTag>();
                    int invNameIndex = 0;

                    foreach (var t in elem.Children)
                    {
                        ImportData el = t as ImportData;
                        if (el != null)
                        {
                            ImportTag a = new ImportTag();

                            if (((ImportDataS7Tcp)el).ImportType == ImportTypes.Struct)
                            {
                                ImportPrototype protoElem = addPrototype(protoMap, el, invNamePrefix, progressBar);
                                if (protoElem == null)
                                    continue;

                                a.ModelType = UFUAModel.ModelType.ObjectType;
                                a.PrototypeModel = protoElem.Name;
                            }
                            else
                            {
                                a.ModelType = UFUAModel.ModelType.Variable;
                            }
                            a.ArrayDimension = el.ArrayDimension;
                            string checkedElName = el.Name;
                            string invNamePrefixWithUnivoqueIndex = invNamePrefix + String.Format("{0}_", invNameIndex.ToString("D5"));
                            invNameIndex++;
                            a.Name = UFUAModel.Helpers.NameValidator.EnsureValidNameWithPrefix(checkedElName, invNamePrefixWithUnivoqueIndex);
                            a.DataType = ((ImportDataS7Tcp)el).Type;
                            a.Description = el.Description;
                            proto.Elements.Add(a);
                        }
                    }
                    protoMap.Add(protoname, proto);
                }
            }
            else
                proto = protoMap[protoname];
            return proto;
        }

        private ImportDataModel Read_Plc_Info()
        {
            string stationName = baseImportTree.ReadStationName();
            if (stationName.Length == 0)
            {
                MessageBox.Show(DriverCodeBaseEx.UI.Properties.Resources.ImportEnterStationName,
                                DriverCodeBaseEx.UI.Properties.Resources.ImportMsgBoxTitle);
                return null;
            }

            return DisplayDirectImport(stationName);
        }

        private ImportDataModel LoadFile(string file)
        {
            CmbProgram.ItemsSource = null;
            isS7P = false;
            isTIAPortal = false;

            //textTitle.Text = string.Format("{0} {1}", Properties.Resources.Import_Device_Variables, file);
            //textTitle.Refresh();

            SetProgramVisibility(Visibility.Hidden);

            file = file.ToLower();
            string fileExt = System.IO.Path.GetExtension(file);

            switch (fileExt) {
                case ".s7p":
                    isS7P = true;
                    DisplayImportFileS7p(file);
                    break;
                case ".ap11":
                case ".ap12":
                case ".ap13":
                case ".ap14":
                case ".ap15":
                case ".ap15_1":
                case ".ap16":
                case ".ap17":
                case ".ap18":
                    isTIAPortal = true;                
                    DisplayImportFileTIAPortal(file);
                    break;
                case ".awl":            
                    DisplayImportFileAwl(file);
                    break;
                case ".sdf":
                    DisplayImportFileSdf(file);
                    break;
            }

            if (fileExt != ".s7p" &&
                fileExt != ".ap11" &&
                fileExt != ".ap12" &&
                fileExt != ".ap13" &&
                fileExt != ".ap14" &&
                fileExt != ".ap15" &&
                fileExt != ".ap15_1" &&
                fileExt != ".ap16" &&
                fileExt != ".ap17" &&
                fileExt != ".ap18"
               )
            {
                //ImportTree.Model = importDataModel;

                //if (listViewSortCol != null)
                //{
                //    AdornerLayer.GetAdornerLayer(listViewSortCol).Remove(listViewSortAdorner);
                //    ImportTree.Items.SortDescriptions.Clear();
                //}
                //importDataModel = null;
            }            
            if (importDataModel == null )
            {
                MessageBox.Show(DriverCodeBaseEx.UI.Properties.Resources.ImportErrorEmptyFile,
                                Properties.Resources.ImportMsgBoxTitle);
            }
            if(CmbProgram.ItemsSource != null)//Controllo che sia stato inizializzato la combo selezione programmi del progetto
            {
                if (((Dictionary<int, string>)CmbProgram.ItemsSource).Count() == 0)
                {
                    MessageBox.Show(DriverCodeBaseEx.UI.Properties.Resources.ImportErrorEmptyFile,
                                    Properties.Resources.ImportMsgBoxTitle);

                    importDataModel = null;
                }                
            }

            return importDataModel;
        }

        private ImportDataModel DisplayDirectImport(string stationName)
        {
            IntPtr rootSchemaNodeHandle = IntPtr.Zero;

            S7TCPStation station = new S7TCPStation(new S7TCPDriver(conn), (S7TCPStationSettings)baseImportTree.GetStationSettings(stationName));
            S7TCPChannel channel = new S7TCPChannel(new S7TCPDriver(conn), (S7TCPChannelSettings)baseImportTree.GetChannelSettingsFromStation(stationName));            

            try
            {
                // Set the library license code
                AGL4.Activate("00BEF5-E1F5-260260");
            }
            catch
            {
                MessageBox.Show(Properties.Resources.AGLink40NotFound,
                                Properties.Resources.ImportMsgBoxTitle);
                return null;
            }

            S7TcpDirectImportFromPlc connToPlc = new S7TcpDirectImportFromPlc(new S7TCPDriver(), channel, station);

            if (!connToPlc.InitOffLine())
            {
                MessageBox.Show(string.Format(Properties.Resources.ErrorConnectToDeviceOrFunctionNotSupported, channel.TcpChannelHostName),
                                      DriverCodeBaseEx.UI.Properties.Resources.ImportMsgBoxTitle);
                return null;
            }

            // Disable driver to create/modify ecc .tia and .list files during import
            if (!connToPlc.DeviceOpen())
            {
                MessageBox.Show(string.Format(Properties.Resources.ErrorConnectToDeviceOrFunctionNotSupported, channel.TcpChannelHostName),
                                     DriverCodeBaseEx.UI.Properties.Resources.ImportMsgBoxTitle);
                baseImportTree.SetButtonEnable(eImportButtons.eiBtnUpdateSymbol, false);
                return null;
            }
            
            int nRet = AGL4.AGL40_SUCCESS;            
            rootSchemaNodeHandle = new IntPtr();
            using (new WaitCursor())
            {
                //string titleLabel;
                //titleLabel = Properties.Resources.Import_Device_Variables + channel.TcpChannelHostName;
                try
                {
                    nRet = AGL4.Symbolic_LoadAGLinkSymbolsFromPLC(connToPlc.connNr, ref rootSchemaNodeHandle);
                    if (nRet != AGL4.AGL40_SUCCESS)
                    {
                        MessageBox.Show(Properties.Resources.ErrorLoadSymbolsFromPLC_ErrorCode + nRet.ToString(),
                                       DriverCodeBaseEx.UI.Properties.Resources.ImportMsgBoxTitle);
                    }
                }
                catch
                {
                    MessageBox.Show(string.Format(Properties.Resources.ErrorLoadSymbolsFromPLC_ErrorCode, nRet.ToString()),
                                    DriverCodeBaseEx.UI.Properties.Resources.ImportMsgBoxTitle);
                    nRet = -1;
                }
                connToPlc.DeviceClose();
                connToPlc.DisConnection();

                if (nRet != AGL4.AGL40_SUCCESS || rootSchemaNodeHandle == IntPtr.Zero)
                {
                    return null;
                }

                if (importDataModel != null)
                    importDataModel.Dispose();
                importDataModel = new ImportDataModelS7Tcp(readStationName, readAddDBnumber);

                if (pareserTia != null)
                {
                    pareserTia.Dispose();
                    pareserTia = null;
                }
                pareserTia = new S7TIAImportParser(S7TIAImportParser.ImportSourceManagement.Plc, null, S7TIAImportParser.ProtocolType.S7Tcp);
                CmbProgram.Text = "";
                pareserTia.ParsingHandlerPointer(rootSchemaNodeHandle);
                LoadTags();
                pareserTia.Dispose();
                pareserTia = null;

                return importDataModel;
            }
        }

        private void DisplayImportFileS7p(string file)
        {            
            if (importDataModel != null)
                importDataModel.Dispose();
            importDataModel = new ImportDataModelS7Tcp(readStationName, readAddDBnumber);
            try
            {
                // Set the library license code
                AGL4.Activate("00BEF5-E1F5-260260");
            }
            catch
            {
                MessageBox.Show(Properties.Resources.AGLink40NotFound,
                                Properties.Resources.ImportMsgBoxTitle);
                return;
            }

            pPrjHandle = -1;
            CmbProgram.Text = "";

            int ret = AGL4Sym.OpenProject(file, ref pPrjHandle);
            if (ret == AGL4Sym.AGLSYM_SUCCESS)
            {
                string progName = "";
                List<string> Programs = new List<string>();
                ret = AGL4Sym.FindFirstProgram(pPrjHandle, ref progName);
                if (ret == AGL4Sym.AGLSYM_SUCCESS)
                {
                    Programs.Add(progName);
                    while (AGL4Sym.FindNextProgram(pPrjHandle, ref progName) >= 0)
                        Programs.Add(progName);
                    AGL4Sym.FindCloseProgram(pPrjHandle);
                    if (Programs.Count() > 0)
                    {
                        Dictionary<int, string> programList = new Dictionary<int, string>();
                        foreach (string program in Programs)
                            programList.Add(programList.Count, program);
                        CmbProgram.ItemsSource = programList;
                        if (programList.Count > 0)
                            CmbProgram.SelectedIndex = 0;
                        SetProgramVisibility(Visibility.Visible);
                        AddDBnumber.IsChecked = true;
                        AddDBnumber.Visibility = System.Windows.Visibility.Hidden;
                    }
                }
            }
        }

        private void LoadProgram_Click(object sender, RoutedEventArgs e)
        {
            if (CmbProgram.Text.Length == 0)
                MessageBox.Show(Properties.Resources.ImportEnterProgramName,
                                Properties.Resources.ImportMsgBoxTitle);
            else if (isS7P)
                LoadProgram_S7p(sender, e);
            else if (isTIAPortal)
                LoadProgram_TIAPortal(sender, e);
        }
        private void LoadProgram_S7p(object sender, RoutedEventArgs e)
        {             
            int ret = AGL4Sym.SelectProgram(pPrjHandle, CmbProgram.Text);
            if (ret == AGL4Sym.AGLSYM_SUCCESS)
            {
                s7MemoryClear();

                String strAbsOpd = "";
                String strSymbol = "";
                String strComment = "";
                Int32 intFormat = 0;
                String PreName = "";

                uint nDBNumber = 0;
                uint nCurrentAddress = 0;
                uint nBitNumber = 0;
                bool bIncrementAddressIfNotBit = false;


                ret = AGL4Sym.FindFirstSymbol(pPrjHandle, ref strAbsOpd, ref strSymbol, ref strComment, ref intFormat);
                if (ret == AGL4Sym.AGLSYM_SUCCESS)
                {
                    while (ret == AGL4Sym.AGLSYM_SUCCESS)
                    {
                        AddVariableS7P(PreName, strSymbol, intFormat, strAbsOpd, strComment, 0, ref nDBNumber,
                            ref nCurrentAddress, ref nBitNumber, ref bIncrementAddressIfNotBit);
                        ret = AGL4Sym.FindNextSymbol(pPrjHandle, ref strAbsOpd, ref strSymbol, ref strComment, ref intFormat);
                    }
                    ret = AGL4Sym.FindCloseSymbol(pPrjHandle);
                }

                int DBCount = 0;
                int retDb = AGL4Sym.ReadPrjDBCount(pPrjHandle, ref DBCount);
                if (retDb == AGL4Sym.AGLSYM_SUCCESS && DBCount > 0)
                {
                    UInt16[] DBList = new UInt16[DBCount + 1];

                    retDb = AGL4Sym.ReadPrjDBList(pPrjHandle, ref DBList, DBCount + 1);

                    if (retDb == AGL4Sym.AGLSYM_SUCCESS)
                    {
                        for (int i = 0; i < DBCount; i++)
                        {

                            AGL4Sym.DATA_DBSYM40 DBSymbol = new AGL4Sym.DATA_DBSYM40();
                            retDb = AGL4Sym.FindFirstDbSymbolEx(pPrjHandle, DBList[i], ref DBSymbol, "");

                            while (retDb == AGL4Sym.AGLSYM_SUCCESS)
                            {
                                int indexSymbol = 0;
                                PreName = "";
                                while (indexSymbol < AGL4Sym.AGLSYM_SYMB_LEN && indexSymbol < DBSymbol.Symbol.Length && DBSymbol.Symbol[indexSymbol] != '.')
                                {
                                    if (DBSymbol.Symbol[indexSymbol] != '"' && ((DBSymbol.Symbol[indexSymbol] != ' ') || (DBSymbol.Symbol[indexSymbol + 1] != ' ')))
                                        PreName = PreName + DBSymbol.Symbol[indexSymbol];
                                    indexSymbol++;
                                }
                                if (indexSymbol < AGL4Sym.AGLSYM_SYMB_LEN && DBSymbol.Symbol[indexSymbol] == '.')
                                {
                                    AddVariableS7P(PreName, DBSymbol.Symbol.Substring(indexSymbol + 1),
                                        DBSymbol.Format, DBSymbol.AbsOpd, DBSymbol.Comment,
                                        DBSymbol.Size, ref nDBNumber, ref nCurrentAddress,
                                        ref nBitNumber, ref bIncrementAddressIfNotBit);

                                    retDb = AGL4Sym.FindNextDbSymbolEx(pPrjHandle, ref DBSymbol);
                                }
                            }
                            ret = AGL4Sym.FindCloseDbSymbol(pPrjHandle);
                        }
                    }
                }
                if (ret == AGL4Sym.AGLSYM_SUCCESS || retDb == AGL4Sym.AGLSYM_SUCCESS)
                {
                    AddVariableArrayRootS7P();

                    createMapUDTBaseS7P();
                    AddVariableRootS7P(m_MapImportVariableStruct);

                    createDataModel();

                }
                AGL4Sym.CloseProject(pPrjHandle);
                SetProgramVisibility(Visibility.Hidden);
            }
        }

        void s7MemoryClear()
        {
            foreach (var item in m_MapImportVariableStruct)
                RemoveLogicalChild(item.Value);
            foreach (var item in m_MapImportVariableArray)
                RemoveLogicalChild(item.Value);
            foreach (var item in m_mapUDTBaseS7P)
                RemoveLogicalChild(item.Value);
            foreach (var item in m_MapImportVariable)
                RemoveLogicalChild(item.Value);

            m_MapImportVariableStruct.Clear();
            m_MapImportVariableArray.Clear();
            m_mapUDTBaseS7P.Clear();
            m_MapImportVariable.Clear();

        }     

        private void createDataModel()
        {
            try
            {                
                if (importDataModel != null)
                    importDataModel.Dispose();
                importDataModel = new ImportDataModelS7Tcp(readStationName, readAddDBnumber);
                Dictionary<long, ImportData> importedVariable = new Dictionary<long, ImportData>();

                foreach (var item in m_MapImportVariable)
                {
                    if (!importedVariable.ContainsKey(item.Key))
                    {
                        createDataModelElement(importedVariable, item.Key);
                    }
                }
                //ImportTree.Model = importDataModel;

                //if (listViewSortCol != null)
                //{
                //    AdornerLayer.GetAdornerLayer(listViewSortCol).Remove(listViewSortAdorner);
                //    ImportTree.Items.SortDescriptions.Clear();
                //}
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(String.Format("Exception in createDataModel: {0}", e.Message));
                importDataModel = null;
            }

            baseImportTree.SetImportDataModel(importDataModel);
        }

        private void createDataModelElement(Dictionary<long, ImportData> importedVariable, long Key)
        {
            if (!m_MapImportVariable.Keys.Contains(Key))
            {
                System.Diagnostics.Debug.WriteLine(String.Format("createDataModelElement Key {0} not found in m_MapImportVariable", Key));
                return;
            }
            ImportData IVar = importDataModel.addImportData();
            IVar.Id = (int)m_MapImportVariable[Key].lID;
            IVar.parentId = (int)m_MapImportVariable[Key].lParent;
            ((ImportDataS7Tcp)IVar).PreName = m_MapImportVariable[Key].szPreName;
            IVar.Name = m_MapImportVariable[Key].szName;
            ((ImportDataS7Tcp)IVar).Type = m_MapImportVariable[Key].nType;
            IVar.szType = m_MapImportVariable[Key].szType;
            if (m_MapImportVariable[Key].szType == "STRING")
                ((ImportDataS7Tcp)IVar).Size = m_MapImportVariable[Key].nSize + 2;
            else
                ((ImportDataS7Tcp)IVar).Size = m_MapImportVariable[Key].nSize;

            ((ImportDataS7Tcp)IVar).ElemType = m_MapImportVariable[Key].nElemType;
            IVar.Address = m_MapImportVariable[Key].szAddress;
            IVar.Description = m_MapImportVariable[Key].szDescription;
            ((ImportDataS7Tcp)IVar).ImportType = m_MapImportVariable[Key].ImportType;
            IVar.ArrayDimension = (uint)m_MapImportVariable[Key].ArrayDimension;

            if (IVar.parentId < 0)
                AddTreeItem(IVar);
            else
            {
                if (!importedVariable.ContainsKey(IVar.parentId))
                    createDataModelElement(importedVariable, IVar.parentId);
                if (importedVariable.ContainsKey(IVar.parentId))
                    AddTreeItem(IVar, importedVariable[IVar.parentId]);
            }
            importedVariable[Key] = IVar;
        }

        void AddVariableRootS7P(Dictionary<string, ImportVariable> m_MapVariableRoot)
        {
            ImportVariable IVar;
            foreach (var item in m_MapVariableRoot)
            {
                IVar = item.Value;
                IVar.nSize += ((IVar.valMaxAbsOp - IVar.valMinAbsOp) / 10);
                m_MapImportVariable[IVar.lID] = IVar;
            }
        }
        void createMapUDTBaseS7P()
        {
            Dictionary <UInt64, string > mapUdtBase = new Dictionary<ulong, string>();
            string UDTBase;
            uint indexMUdt = 1;

            Normalize_mapUDTBaseS7P();

            Dictionary<string, ImportVariable> m_MapImportVariableStructChange = new Dictionary<string, ImportVariable>();
            foreach (var item in m_MapImportVariableStruct)
            {
                ImportVariable IVar = item.Value;
                if(!mapUdtBase.ContainsKey(IVar.structKeyCode))
                {
                    UDTBase = string.Format("UDT_{0}", indexMUdt++);
                    mapUdtBase.Add(IVar.structKeyCode, UDTBase);
                    m_mapUDT[UDTBase] = m_mapUDTBaseS7P[item.Key];
                }
                else
                {
                    UDTBase = mapUdtBase[IVar.structKeyCode];
                    bool UDTBaseOK = testUdtElemS7P(UDTBase, item.Key);
                    if (!UDTBaseOK)
                    {
                        while ((!string.IsNullOrEmpty(UDTBase)) && !UDTBaseOK)
                        {
                            IVar.structKeyCode++;
                            if (mapUdtBase.ContainsKey(IVar.structKeyCode))
                            {
                                UDTBase = mapUdtBase[IVar.structKeyCode];
                                UDTBaseOK = testUdtElemS7P(UDTBase, item.Key);
                            }
                            else UDTBase = "";
                        }
                        if ((!string.IsNullOrEmpty(UDTBase)) && UDTBaseOK)
                        {
                            m_MapImportVariableStructChange[item.Key] = IVar;
                        }
                        if (UDTBase == "")
                        {
                            UDTBase = string.Format("UDT_{0}", indexMUdt++);
                            mapUdtBase.Add(IVar.structKeyCode, UDTBase);
                            m_mapUDT[UDTBase] = m_mapUDTBaseS7P[item.Key];
                            m_MapImportVariableStructChange[item.Key] = IVar;
                        }
                    }
                    else
                    {
                        m_mapUDTBaseS7P[item.Key] = new List<string>();
                    }

                }
            }

            foreach (var item in m_MapImportVariableStructChange)
                m_MapImportVariableStruct[item.Key] = item.Value;

            m_MapImportVariableStructChange.Clear();
            foreach (var item in m_MapImportVariableStruct)
            {
                ImportVariable IVar = item.Value;
                IVar.szType = mapUdtBase[IVar.structKeyCode];
                m_MapImportVariableStructChange[item.Key] = IVar;

            }
            m_MapImportVariableStruct = m_MapImportVariableStructChange;
            Clean_mapUDT();
        }
        bool testUdtElemS7P(string UDTBase, string Struct)
        {
            List<string> StringArray = m_mapUDTBaseS7P[Struct];
            List<string> StringArrayTest = m_mapUDT[UDTBase];
            bool testOk = true;

            int sizeArray = StringArray.Count();
            int indexArray = sizeArray;
            int indexArrayTest = StringArrayTest.Count();

            if ((indexArray != indexArrayTest))
            {
                testOk = false;
            }
            while (testOk && indexArray > 0)
            {
                while (testOk && indexArrayTest > 0)
                {
                    if (StringArray[indexArray % sizeArray] == StringArrayTest[((indexArrayTest + indexArray) % sizeArray)])
                    {
                        indexArrayTest = 0;
                    }
                    else
                    {
                        indexArrayTest--;
                        if (indexArrayTest == 0)
                        {
                            testOk = false;
                        }
                    }
                }
                indexArrayTest = sizeArray;
                indexArray--;
            }
            return testOk;
        }
        void Clean_mapUDT()
        {
            int indexArray;
            Dictionary<string, List<string>> m_mapUDTtmp = new Dictionary<string, List<string>>();

            foreach (var item in m_mapUDT)
            {
                if (item.Value.Count()>0)
                {
                    m_mapUDTtmp[item.Key] = new List<string>();
                    indexArray = 0;
                    while (indexArray < item.Value.Count())
                    {
                        m_mapUDTtmp[item.Key].Add(item.Value[indexArray].Substring(0,item.Value[indexArray].IndexOf(';') + 1));
                        indexArray++;
                    }
                }
            }
            m_mapUDT = m_mapUDTtmp;
            m_mapUDTtmp.Clear();
        }

        void Normalize_mapUDTBaseS7P()
        {
            if (isS7P)
            {
                uint memberAddress;
                int nAux;
                string member;
                string szAux;
                ImportVariable IVar;
                Dictionary<string, List<string>> m_mapUDTBaseS7PTmp = new Dictionary<string, List<string>>();

                foreach (var item in m_mapUDTBaseS7P)
                {
                    m_mapUDTBaseS7PTmp[item.Key] = new List<string>();
                    IVar = m_MapImportVariableStruct[item.Key];
                    for (int indexArray = 0; indexArray < item.Value.Count(); indexArray++)
                    {
                        szAux = item.Value[indexArray];
                        nAux = szAux.IndexOf(';') + 1;
                        member = szAux.Substring(nAux);
                        memberAddress = uint.Parse(member);
                        member = szAux.Substring(0, nAux);
                        memberAddress = memberAddress - IVar.valMinAbsOp;
                        m_mapUDTBaseS7PTmp[item.Key].Add(string.Format("{0}{1}", member, memberAddress));
                    }
                }
                m_mapUDTBaseS7P = m_mapUDTBaseS7PTmp;
            }

            if (isTIAPortal)
            {
                uint memberAddress;
                int nAux;
                string member;
                string szAux;
                ImportVariable IVar;

                Dictionary<string, S7TIAImportParser.stPrototype> mapUDTBaseS7 = pareserTia.GetMapUDTBaseS7P();
                foreach (var item in m_mapUDTBaseS7P)
                {
                    S7TIAImportParser.stPrototype Proto = mapUDTBaseS7[item.Key];
                    IVar = m_MapImportVariableStruct[item.Key];
                    foreach (string Element in Proto.Values)
                    {
                        szAux = Element;
                        nAux = szAux.IndexOf(';') + 1;
                        member = szAux.Substring(nAux);
                        memberAddress = uint.Parse(member);
                        member = szAux.Substring(0, nAux);
                        memberAddress = memberAddress - IVar.valMinAbsOp;
                        m_mapUDTBaseS7P[item.Key].Add(string.Format("{0}{1}", member, memberAddress));
                    }
                }
            }
        }
        void AddVariableArrayRootS7P()
        {

            uint nAggregationLimit = MAX_DATA_BYTES;

            foreach (var item in m_MapImportVariableArray)
            {
                ImportVariable IVarArray = item.Value;
                IVarArray.nSize += (IVarArray.valMaxAbsOp - IVarArray.valMinAbsOp);

                uint nElemSize = SizeMoviconType(IVarArray.nElemType);
                uint nArrayLimit = (nAggregationLimit / nElemSize) * nElemSize;

                if (IVarArray.nSize <= nArrayLimit)
                {
                    IVarArray.ArrayDimension = (uint)IVarArray.m_MapArrayVariable.Count();
                    IVarArray.nType = IVarArray.nElemType;
                    IVarArray.m_MapArrayVariable = new Dictionary<uint, long>();
                    m_MapImportVariable[IVarArray.lID] = IVarArray;

                }
                else
                {
                    uint nArrayDim = IVarArray.nSize / nElemSize;
                    uint nArrayBlockSize = nArrayLimit / nElemSize;
                    uint nArrayBlocks = (nArrayDim + nArrayBlockSize - 1) / nArrayBlockSize;
                    uint nArrayBlock = 1;
                    uint nArrayStart = 0;
                    uint nArrayEnd;
                    long elementlID;
                    ImportVariable IVarBlock = new ImportVariable();
                    ImportVariable IVarElement = new ImportVariable();
                    string szAux;

                    IVarBlock.szDescription = "";
                    IVarBlock.lParent = -1;
                    IVarBlock.nType = IVarArray.nElemType;
                    IVarBlock.nElemType = IVarArray.nElemType;
                    IVarBlock.szType = IVarArray.szType;
                    IVarBlock.nSize = nArrayBlockSize * nElemSize;
                    IVarBlock.ArrayDimension = nArrayBlockSize ;

                    while (nArrayBlock <= nArrayBlocks)
                    {
                        szAux = string.Format("_{0}_of_{1}", nArrayBlock, nArrayBlocks);


                        IVarBlock.szName = IVarArray.szName + szAux;
                        IVarBlock.lID = m_lGlobalID++;

                        elementlID = IVarArray.m_MapArrayVariable[IVarArray.valMinAbsOp + nElemSize * nArrayStart];
                        IVarElement = m_MapImportVariable[elementlID];

                        IVarBlock.szAddress = IVarElement.szAddress;

                        nArrayEnd = nArrayStart + nArrayBlockSize;
                        if (nArrayEnd > nArrayDim)
                        {
                            nArrayEnd = nArrayDim;
                            IVarBlock.nSize = (nArrayEnd - nArrayStart) * nElemSize;
                            IVarBlock.ArrayDimension = nArrayEnd - nArrayStart;
                        }

                        m_MapImportVariable[IVarBlock.lID] = IVarBlock;

                        while (nArrayStart < nArrayEnd)
                        {
                            elementlID = IVarArray.m_MapArrayVariable[IVarArray.valMinAbsOp + nElemSize * nArrayStart];
                            IVarElement = m_MapImportVariable[elementlID];

                            m_MapImportVariable.Remove(elementlID);
                            IVarElement.lParent = IVarBlock.lID;
                            m_MapImportVariable[elementlID] = IVarElement;
                            nArrayStart++;
                        }
                        nArrayBlock++;
                    }
                    IVarArray.m_MapArrayVariable.Clear();
                }
            }
            m_MapImportVariableArray.Clear();
        }
        uint SizeMoviconType(DataType nElemType)
        {
            switch(nElemType)
            {
                case DataType.Boolean:
                    return 1;
                case DataType.SByte:
                    return 1;
                case DataType.Byte:
                    return 1;
                case DataType.Int16:
                    return 2;
                case DataType.UInt16:
                    return 2;
                case DataType.Int32:
                    return 4;
                case DataType.UInt32:
                    return 4;
                case DataType.Int64:
                    return 8;
                case DataType.UInt64:
                    return 8;
                case DataType.Float:
                    return 4;
                case DataType.Double:
                    return 8;
                case DataType.String:
                    return 1;
            }
            return 0;
        }
        void AddVariableS7P(String PreName, String Symbol, int Format,
            String inAbsOpd, String Comment, UInt16 Size,
            ref uint nDBNumber, ref uint nCurrentAddress,
            ref uint nBitNumber, ref bool bIncrementAddressIfNotBit)

        {
            if (Format >= 0)
            {
                String szType = FormatTxt[Format];
                if (szType.CompareTo("") != 0)
                {
                    //Format the address 
                    string AbsOpd = inAbsOpd.Replace(" ", "");
               

                    if (!AddVariableStructS7P(PreName, ref Symbol,ref szType,
                        Format, AbsOpd, Comment, Size, ref nDBNumber, ref nCurrentAddress,
                        ref nBitNumber, ref bIncrementAddressIfNotBit))
                    {

                        if (!AddVariableArrayS7P(PreName, ref Symbol, ref szType,
                            Format, AbsOpd, Comment, Size, ref nDBNumber, ref nCurrentAddress,
                            ref nBitNumber, ref bIncrementAddressIfNotBit))
                        {
                            AddVariableSimpleS7P(PreName, Symbol, ref szType,
                                AbsOpd, Comment, Size);
                        }
                    }
                }
            }
        }
        void AddVariableSimpleS7P(String PreName, String Symbol, ref String szType,
            string AbsOpd, string Comment, UInt16 Size)
        {
            ImportVariable IVar = new ImportVariable();
            int nConversion = 0;
            uint nVarSize = 0;

            IVar.szType = szType;

            IVar.szPreName = PreName;
            IVar.szName = Symbol.Trim('\"');
            if (!GetMoviconTypeId(szType, ref nConversion, ref nVarSize, ref IVar.nType))
                return;
            if (Size != 0)
            {
                nVarSize = (uint)(Size + 7) / 8;
            }
            IVar.nSize = nVarSize;
            IVar.ArrayDimension = 0;
            IVar.nElemType = DataType.Boolean ;
            IVar.szDescription = Comment;
            IVar.lParent = -1;
            IVar.lID = m_lGlobalID++;

            if (szType != "STRING")
            {
                if (szType != "S5TIME")
                {
                    IVar.szAddress = AbsOpd;
                }
                else
                {
                    IVar.szAddress = AbsOpd + ",T";
                }

            }
            else
            {
                if (getDbNumber(AbsOpd) < 0)
                    return;

                if (IVar.nSize > MAX_DATA_BYTES)
                    IVar.nSize = MAX_DATA_BYTES;

                IVar.szAddress = string.Format("DB{0}.DBB{1}:{2}", getDbNumber(AbsOpd), valLowAbsOpS7P(AbsOpd) / 10 + 2, IVar.nSize - 2);
            }

            m_MapImportVariable[IVar.lID] = IVar;
        }


        bool AddVariableArrayS7P(String PreName, ref String Symbol, ref String szType,
            int Format, string AbsOpd, string Comment, UInt16 Size,
            ref uint nDBNumber, ref uint nCurrentAddress,
            ref uint nBitNumber, ref bool bIncrementAddressIfNotBit)
        {
            bool Out = false;

            // added in version 10.1.0.16 (FOGBUGZ 9604)
            if ("STRING" == szType ||
                ("S5TIME")== szType)
            {
                return Out;
            }


            ImportVariable IVar = new ImportVariable();
            string szArray = Symbol.Trim('\"');
            int rightP = szArray.IndexOf('[');
            int i;

            if (rightP > 0 && szArray.IndexOf(']') == szArray.Length - 1)
            {
                if ("BOOL" == szType)
                {
                    string szStruct = szArray.Substring(0, szArray.Length - 1);
                    szStruct = szStruct.Remove(rightP, 1).Insert(rightP, ".");
                    while ((i = szStruct.IndexOf(',', rightP + 1)) >= 0)
                    {
                        szStruct = szStruct.Remove(i, 1).Insert(i, "_");
                        rightP = i;
                    }
                    Symbol = szStruct;
                    Out = AddVariableStructS7P(PreName, ref Symbol, ref szType, Format,
                        AbsOpd, Comment, Size, ref nDBNumber, ref nCurrentAddress,
                        ref nBitNumber, ref bIncrementAddressIfNotBit);

                }
                else
                {
                    int nConversion = 0;
                    uint nVarSize = 0;

                    if (!GetMoviconTypeId(szType, ref nConversion, ref nVarSize, ref IVar.nType))
                        return false;

                    if (Size != 0)
                    {
                        nVarSize = (uint)((Size + 7) / 8);
                    }

                    szArray = szArray.Remove(rightP, 1).Insert(rightP, "_");
                    while ((i = szArray.IndexOf(',', rightP + 1)) >= 0)
                    {
                        rightP = i;
                        szArray = szArray.Remove(i, 1).Insert(i, "_");
                    }
                    szArray = szArray.Substring(0, rightP);
                    szArray = PreName + "_" + szArray;

                    IVar.szAddress = AbsOpd;
                    IVar.valMinAbsOp = valLowAbsOpS7P(AbsOpd) / 10;

                    ImportVariable IVarArray;
                    if (!m_MapImportVariableArray.ContainsKey(szArray))
                    {
                        IVarArray = new ImportVariable();
                        IVarArray.m_MapArrayVariable = new Dictionary<uint, long>();
                        IVarArray.nSize = nVarSize;
                        IVarArray.ArrayDimension = 0;
                        IVarArray.szName = szArray;
                        IVarArray.szAddress = IVar.szAddress;
                        IVarArray.szDescription = "";
                        IVarArray.valMinAbsOp = IVarArray.valMaxAbsOp = IVar.valMinAbsOp;
                        IVarArray.nElemType = IVar.nType;
                        IVarArray.nType = DataType.Boolean;
                        IVar.ImportType = ImportTypes.Array;
                        IVarArray.szType = "ARRAY OF " + szType;
                        IVarArray.lParent = -1;
                        IVarArray.lID = m_lGlobalID++;
                        m_MapImportVariableArray[szArray] = IVarArray;

                    }
                    else
                    {
                        IVarArray = m_MapImportVariableArray[szArray];
                        if (IVar.valMinAbsOp < IVarArray.valMinAbsOp)
                        {
                            IVarArray.valMinAbsOp = IVar.valMinAbsOp;
                            IVarArray.szAddress = IVar.szAddress;
                        }
                        if (IVar.valMinAbsOp > IVarArray.valMaxAbsOp)
                        {
                            IVarArray.valMaxAbsOp = IVar.valMinAbsOp;
                        }
                        m_MapImportVariableArray[szArray] = IVarArray;
                    }

                    IVar.lID = m_lGlobalID++;
                    IVarArray.m_MapArrayVariable.Add(IVar.valMinAbsOp, IVar.lID);


                    IVar.nSize = nVarSize;
                    IVar.ArrayDimension = 0;
                    IVar.szDescription = Comment;
                    IVar.lParent = IVarArray.lID;
                    IVar.szPreName = PreName;
                    IVar.szName = Symbol.Trim('\"');
                    IVar.nElemType = DataType.Boolean;
                    IVar.szType = szType;
                    m_MapImportVariable[IVar.lID] = IVar;

                    Out = true;

                }
            }
            return Out;
        }

        bool AddVariableStructS7P(String PreName, ref String Symbol, ref String szType,
            int Format, string AbsOpd, string Comment, UInt16 Size,
            ref uint nDBNumber, ref uint nCurrentAddress,
            ref uint nBitNumber, ref bool bIncrementAddressIfNotBit, long lParent = -1)

        {
            bool bOut = false;

            if ("S5TIME" == szType)
            {
                return bOut;
            }

            if (getDbNumber(AbsOpd) < 0)
            {
                return bOut;
            }

            int nConversion = 0;
            uint nVarSize = 0;
            DataType nType = DataType.Boolean;

            ImportVariable IVar = new ImportVariable();
            IVar.szName = Symbol.Trim('\"');

            int leftP = IVar.szName.IndexOf('.');

            if (leftP > 0)
            {
                ImportVariable IVarStruct;
                String preElement = IVar.szName.Substring(0, leftP);
                String subElement = IVar.szName.Substring(leftP + 1);
                String Struct = PreName + "_" + preElement;
                String element = IVar.szName.Substring(0,leftP) + "_" + subElement;


                if ((element.IndexOf('[') > 0) || (szType == "STRING"))
                {
                    return false;
                }
                else if (GetMoviconTypeId(szType, ref nConversion, ref nVarSize, ref nType))
                {
                    IVar.szPreName = PreName;
                    IVar.nElemType = DataType.Boolean;
                    IVar.nType = nType;
                    if (Size != 0)
                    {
                        nVarSize = (uint)((Size + 7) / 8);
                    }
                    IVar.nSize = nVarSize;
                    IVar.ArrayDimension = 0;

                    IVar.szAddress = AbsOpd;
                    IVar.szDescription = Comment;
                    IVar.valMinAbsOp = valLowAbsOpS7P(AbsOpd);
                    IVar.valMaxAbsOp = valLowAbsOpS7P(AbsOpd);


                    Opc.Ua.BuiltInType nVarType = GetTypeId(szType);
                    string szAddress = AbsOpd;
                    uint lastnCurrentAddress = 0;
                    uint lastnBitNumber = 0;
                    if (!m_MapImportVariableStruct.ContainsKey(Struct))
                    {
                        IVarStruct = new ImportVariable();
                        IVarStruct.structFull = false;
                        IVarStruct.valMinAbsOp = IVar.valMinAbsOp;
                        IVarStruct.valMaxAbsOp = IVar.valMaxAbsOp;
                        IVarStruct.szPreName = IVar.szPreName;
                        IVarStruct.nElemType = IVar.nElemType;
                        IVarStruct.nType = IVar.nType;
                        IVarStruct.nSize = IVar.nSize;
                        IVarStruct.ArrayDimension = IVar.ArrayDimension;

                        IVarStruct.szAddress = IVar.szAddress = AbsOpd;
                        IVarStruct.szDescription = IVar.szDescription;

                        nBitNumber = IVarStruct.valMinAbsOp % 10;
                        nCurrentAddress = IVarStruct.valMinAbsOp / 10;
                        nDBNumber = (uint)getDbNumber(AbsOpd);
                        bIncrementAddressIfNotBit = false;
                        GetAddress(nVarType, nDBNumber, ref nCurrentAddress, ref nBitNumber,
                            ref bIncrementAddressIfNotBit, 0, ref szType);
                        if(lParent == (long)(-1))
                            IVarStruct.structFull = false;
                        else
                            IVarStruct.structFull = m_MapImportVariable[lParent].structFull;
                        IVarStruct.szName = preElement;
                        IVarStruct.ImportType = ImportTypes.Struct;
                        IVarStruct.szType = Struct;
                        IVarStruct.lParent = lParent;
                        IVarStruct.lID = m_lGlobalID++;
                        if (element.Split('.').Length == 1)
                        {
                            IVarStruct.structKeyCode = FormatKeyCode[Format];
                        }
                        else
                        {
                            //Structures that do not contain simple variables, to avoid duplicating the structure key, It subtract the maximum value.
                            IVarStruct.structKeyCode = ulong.MaxValue - (ulong)IVarStruct.lID;
                        }
                        m_MapImportVariableStruct[Struct] = IVarStruct;


                        m_mapUDTBaseS7P[Struct] = new List<string>();

                        GetMoviconTypeId(szType, ref nConversion, ref nVarSize, ref IVarStruct.nType);

                        IVarStruct.nElemType = DataType.Boolean;
                        IVarStruct.szDescription = Comment;
                        m_MapImportVariable[IVarStruct.lID] = IVarStruct;

                        bOut = true;
                    }
                    else
                    {
                        lastnCurrentAddress = nCurrentAddress;
                        lastnBitNumber = nBitNumber;
                        szAddress = GetAddress(nVarType, nDBNumber, ref nCurrentAddress, ref nBitNumber,
                            ref bIncrementAddressIfNotBit, 0, ref szType);
                        IVarStruct = m_MapImportVariableStruct[Struct];

                    }

                    if (element.IndexOf('.') > 0)
                    {
                        Symbol = element;
                        nCurrentAddress = lastnCurrentAddress;
                        nBitNumber = lastnBitNumber ;
                        bOut = AddVariableStructS7P(PreName, ref Symbol, ref szType, Format,
                            AbsOpd, Comment, Size, ref nDBNumber, ref nCurrentAddress,
                            ref nBitNumber, ref bIncrementAddressIfNotBit, IVarStruct.lID);
                    }
                    else
                    {
                        IVar.szName = subElement;
                        if (szAddress != AbsOpd)
                            IVarStruct.structFull = true;

                        IVar.structFull = IVarStruct.structFull;

                        if (!IVarStruct.structFull)
                        {
                            if (IVar.valMinAbsOp < IVarStruct.valMinAbsOp)
                            {
                                IVarStruct.valMinAbsOp = IVar.valMinAbsOp;
                                IVarStruct.szAddress = IVar.szAddress;
                            }
                            if (IVar.valMaxAbsOp > IVarStruct.valMaxAbsOp)
                            {
                                IVarStruct.valMaxAbsOp = IVar.valMaxAbsOp;
                                IVarStruct.nSize = IVar.nSize;
                                IVarStruct.ArrayDimension = 0;

                            }
                            IVarStruct.structKeyCode = m_MapImportVariableStruct[Struct].structKeyCode * FormatKeyCode[Format];
                            m_MapImportVariableStruct[Struct] = IVarStruct;

                            if (!m_mapUDTBaseS7P.ContainsKey(Struct))
                                m_mapUDTBaseS7P[Struct] = new List<string>();
                            m_mapUDTBaseS7P[Struct].Add(string.Format("{0} : {1} ;{2}", subElement, szType, IVar.valMinAbsOp));
                            IVar.lParent = IVarStruct.lID;
                            IVar.nElemType = IVar.nType;
                            IVar.szType = szType;
                            IVar.szDescription = Comment;
                            IVar.lID = m_lGlobalID++;
                            m_MapImportVariable[IVar.lID] = IVar;

                            bOut = true;
                        }
                    }
                }
            }

            return bOut;
        }

        int getDbNumber(string AbsOpd)
        {
            if(AbsOpd == "")
                return -1;
            if (AbsOpd[0] != 'D' && AbsOpd[0] != 'B')
                return -1;
            int indexAbsOpd = 2;
            int rt = 0;

            while (indexAbsOpd < AGL4Sym.AGLSYM_ABSOP_LEN && AbsOpd[indexAbsOpd] != '.')
            {
                indexAbsOpd++;
            }

            if (AbsOpd[indexAbsOpd] == '.')
            {
                int mul = 1;
                indexAbsOpd--;
                while (indexAbsOpd > 0 && (AbsOpd[indexAbsOpd] >= '0' && AbsOpd[indexAbsOpd] <= '9'))
                {
                    rt = rt + (AbsOpd[indexAbsOpd] - '0') * mul;
                    mul *= 10;
                    indexAbsOpd--;
                }
            }

            return rt;
        }

        uint valLowAbsOpS7P(string AbsOpd)
        {
            if (AbsOpd == "")
                return 0;
            int indexAbsOpd = AbsOpd.Length - 1;
            uint rt = 0;
            bool isBOOL = false;
            int mul = 1;

            while (indexAbsOpd > 0 && (AbsOpd[indexAbsOpd] == '.' || (AbsOpd[indexAbsOpd] >= '0' && AbsOpd[indexAbsOpd] <= '9')))
            {
                if (AbsOpd[indexAbsOpd] == '.')
                {
                    isBOOL = true;
                }
                else
                {
                    rt = (uint)(rt + (AbsOpd[indexAbsOpd] - '0') * mul);
                    mul *= 10;
                }
                indexAbsOpd--;
            }
            if (!isBOOL)
            {
                rt *= 10;
            }
            return rt;
        }

        private void LoadProgram_TIAPortal(object sender, RoutedEventArgs e)
        {
            using (new WaitCursor())
            {
                pareserTia.ParsingSelectProgram(CmbProgram.SelectedIndex);

                //var idm = LoadTags();
                LoadTags();

                pareserTia.Dispose();
                pareserTia = null;
                //baseImportTree.SetImportDataModel(idm);
                //importSource = S7TIAImportParser.ImportSourceManagement.Project;
            }
        }
        void AddVariableArrayRootTiaPortal()
        {

            foreach (var item in m_MapImportVariableArray)
            {
                ImportVariable IVarArray = item.Value;
                m_MapImportVariable[IVarArray.lID] = IVarArray;
            }
            m_MapImportVariableArray.Clear();
        }

        void createMapUDTBaseTiaPortal()
        {
            ImportVariable IVar;

            Normalize_mapUDTBaseS7P();


            Dictionary<string, ImportVariable> m_MapImportVariableStructTmp = new Dictionary<string, ImportVariable>();
            foreach (var item in m_MapImportVariableStruct)
            {
                IVar = m_MapImportVariableStruct[item.Key];
                if (!m_MapImportVariableStructTmp.ContainsKey(IVar.szType))
                    m_MapImportVariableStructTmp[IVar.szType] = IVar;
            }
            m_MapImportVariableStruct = m_MapImportVariableStructTmp;
            foreach (var item in m_MapValueDepthStructToNumStruct)
                RemoveLogicalChild(item.Value);
            m_MapValueDepthStructToNumStruct.Clear();
            Clean_mapUDT();
        }               

        private void DisplayImportFileAwl(string file)
        {
            System.IO.StreamReader readFile = new System.IO.StreamReader(file);
            try
            {
                if (importDataModel != null)
                    importDataModel.Dispose();
                importDataModel = new ImportDataModelS7Tcp(readStationName, readAddDBnumber);

                ParseDataTypeAWL(ref readFile);

                PrepareDataTypeMapsAWL();
                ParseVariableAWL(ref readFile);
            }
            catch (Exception e)
            {
            }
            finally
            {
                if (readFile != null)
                {
                    readFile.Close();
                    AddDBnumber.Visibility = System.Windows.Visibility.Visible;
                }
            }
        }

        /// <summary>
        /// Struct name is the sum of all struct level
        /// </summary>
        /// <param name="structTree"></param>
        /// <param name="structName"></param>
        private string AWLGetStructureName(List<string> structTree)
        {
            string StructName = string.Empty;

            foreach (string SubStructName in structTree)
                StructName += string.Format("_{0}", SubStructName);

            if (StructName.Length > 0 && StructName.Substring(0, 1) == "_")
                StructName = StructName.Substring(1, StructName.Length - 1);

            return StructName;
        }

        private void ParseVariableAWL(ref System.IO.StreamReader readFile)
        {
            long ulPos;

            string szDBName = string.Empty;
            string line;
            string szApp;
            string szGlobalName = string.Empty;
            uint nDBNumber = 0;
            uint nGlobalLevel = 0;
            uint nCurrentAddress = 0;
            uint nBitNumber = 0;
            bool bInitialStructFound = false;
            bool bDataDeclarationEndFound = false;
            bool bIncrementAddressIfNotBit = false;
            bool bFirstVarOfStruct = false;

            List<string> slLevelName = new List<string>();
            List<bool> blIsArray = new List<bool>();
            List<bool> blIsStruct = new List<bool>();
            List<uint> lnArrayDim = new List<uint>();
            List<uint> lnArrayIndex = new List<uint>();
            List<long> llInitPosition = new List<long>();

            List<string> StructTree = new List<string>();

            while ((line = readFile.ReadLine()) != null)
            {
                int nOf = line.IndexOf(" OF ");
                int nSlash = line.IndexOf("//");

                if ((nOf > 0) && (nSlash >= (nOf + 4)))
                {
                    bool bCommentBreaksLine = false;
                    if (nSlash == (nOf + 4))
                    {
                        bCommentBreaksLine = true;
                    }
                    else
                    {
                        string szBl = line.Substring(nOf + 4, nSlash - nOf - 4);
                        szBl = szBl.Trim();
                        if (szBl.Length == 0)
                        {
                            bCommentBreaksLine = true;
                        }
                    }
                    if (bCommentBreaksLine)
                    {
                        //comment breaks the line!!!
                        szApp = readFile.ReadLine();
                        string szTmp = line.Substring(nSlash);
                        line = line.Substring(0, nSlash);
                        line += szApp + szTmp;
                    }
                }

                ulPos = readFile.BaseStream.Position;

                // Skip empty lines
                if (line.Length == 0)
                {
                    continue;
                }

                // Check if the line is the beginning of a User Data Type definition
                // and, in this case, store the UDT definition in the corresponding map
                /*if (AWLUDTGet(ref readFile, line))
                {
                    continue;
                }*/

                // Search for the number of the Data Block
                if (szDBName.Length == 0)
                {
                    AWLGetDBNumber(line, ref szDBName, ref nDBNumber);
                    continue;
                }

                // Search for the initial "STRUCT" string
                if (!bInitialStructFound)
                {
                    bInitialStructFound = AWLIsTheInitialStructLine(line);
                    nGlobalLevel = 0;
                    StructTree.Clear();

                    // Check if the whole data block is a UDT 
                    if (!bInitialStructFound)
                    {
                        if (AWLUDTFound(line.Replace("  ", string.Empty)))
                            bDataDeclarationEndFound = true;
                    }

                    continue;
                }
                // Check if the line is an "end of structure" declaration
                if (AWLIsEndOfStruct(line))
                {
                    if (nGlobalLevel == 0)
                    {
                        bDataDeclarationEndFound = true;
                        nBitNumber = 0;
                        if ((nCurrentAddress % 2) > 0)
                            nCurrentAddress++;

                        continue;
                    }

                    if (nGlobalLevel > 0)
                    {
                        nGlobalLevel--;
                        //remove previous struct level
                        if (StructTree.Count > 0)
                            StructTree.RemoveAt(StructTree.Count - 1);
                    }

                    if (bIncrementAddressIfNotBit)
                    {
                        bIncrementAddressIfNotBit = false;
                        nCurrentAddress++;
                    }

                    nBitNumber = 0;
                    if ((nCurrentAddress % 2) > 0)
                        nCurrentAddress++;
                    continue;
                }

                // Get the name and the type of the variable
                string szVarName = string.Empty;
                string szVarType = string.Empty;
                string szDescription = string.Empty;

                bool bIsStruct = false;
                bool bIsArray = false;
                uint nArrayDim = 0;
                uint nStringLen = 0;


                AWLGetVarNameType(line, ref szVarName, ref szVarType, ref bIsStruct,
                    ref bIsArray, ref nArrayDim, ref szDescription, ref nStringLen);

                string szAddress;

                //////////Struct/////////////////////////////////////////////////////////
                if (bIsStruct)
                {
                    //Struct or array of struct
                    nGlobalLevel++;
                    StructTree.Add(szVarName);
                    //define struct name as a sum of of struct sublevel
                    szVarName = AWLGetStructureName(StructTree);

                    //To make the structures word aligned
                    bFirstVarOfStruct = true;

                    if (bIsArray)
                    {
                    }
                    else
                    {
                        AWLGetStructure(ref readFile, szVarName, szDescription, ref nCurrentAddress,
                            ref nDBNumber, ref nBitNumber, ref bIncrementAddressIfNotBit, ref nGlobalLevel, ref StructTree);

                    }
                    continue;
                }


                if (szVarName.Length == 0 || szVarType.Length == 0)
                    continue;

                Opc.Ua.BuiltInType nVarType = GetTypeId(szVarType);

                if (bFirstVarOfStruct)
                {
                    if (bIncrementAddressIfNotBit)
                    {
                        bIncrementAddressIfNotBit = false;
                        nCurrentAddress++;
                    }

                    nBitNumber = 0;
                    if ((nCurrentAddress % 2) > 0)
                        nCurrentAddress++;
                    bFirstVarOfStruct = false;
                }


                //////////////////////////////////////////////////////////////////////////
                //////////////////Array of simple type//////////////////////////////
                if (bIsArray && nVarType >= 0)
                {
                    szAddress = GetAddress(nVarType, nDBNumber, ref nCurrentAddress, ref nBitNumber,
                        ref bIncrementAddressIfNotBit, nStringLen, ref szVarType, false);
                    if (nVarType == Opc.Ua.BuiltInType.String)
                        nCurrentAddress -= 2;
                    AddArray(nDBNumber, ref nCurrentAddress, ref nBitNumber, ref bIncrementAddressIfNotBit,
                        "", szVarName, szAddress, szVarType, szDescription, nStringLen, nArrayDim, ImportTypes.Array);

                }
                //////////////////////////////////////////////////////////////////////////
                /////////////////////Array of UDT/////////////////////////////////////////
                //else if (bIsArray && nVarType < 0)
                //{
                //    for (uint i = 0; i < nArrayDim; i++)
                //    {
                //        string szVarTempName;
                //        szVarTempName = string.Format(szVarComplName + "_{0}", i);

                //        AWLUDTAddToList(szVarTempName, szVarType, szDescription,
                //            szGlobalName, nDBNumber, ref nCurrentAddress, ref nBitNumber, ref bIncrementAddressIfNotBit);

                //    }
                //}
                ////////////////////////////UDT/////////////////////////////////////////
                else if (!bIsArray && nVarType == Opc.Ua.BuiltInType.Null)
                {
                    AddStruct(nDBNumber, ref nCurrentAddress, ref nBitNumber, ref bIncrementAddressIfNotBit,
                        "", szVarName, szDescription, szVarType);
                    continue;

                    //AWLIncrementCurAddForNotSuppType(ref nCurrentAddress, ref szVarType,
                    //        bIsArray, nArrayDim, ref nBitNumber,
                    //        ref bFirstVarOfStruct,
                    //        ref bIncrementAddressIfNotBit);
                    //continue;
                }
                ///////////////////////////////variable///////////////////////////////////
                else if (!bIsArray)
                {
                    szAddress = GetAddress(nVarType, nDBNumber, ref nCurrentAddress, ref nBitNumber,
                        ref bIncrementAddressIfNotBit, nStringLen, ref szVarType);

                    if (szAddress.Length == 0)
                        continue;

                    AddStandardVar(nDBNumber, "", szVarName, szAddress, szVarType, szDescription, nStringLen);

                }

            }
        }


        private void AddArray(uint nDBNumber, ref uint nCurrentAddress, ref uint nBitNumber,
            ref bool bIncrementAddressIfNotBit, String szNamePrefix, string szArrayName,
            string szArrayAddress, string szVarType, string szDescription,
            uint nStringLen, uint nArrayDim, ImportTypes elementType,
            int lParentID = -1, ImportData inRootItem = null)
        {
            Opc.Ua.BuiltInType nVarType = GetTypeId(szVarType);

            if (nVarType == Opc.Ua.BuiltInType.String || elementType != ImportTypes.Standard)
            {
                string szTempAddress = string.Empty;
                InsertArrayMembers(nVarType, nDBNumber, ref nCurrentAddress, ref nBitNumber,
                    ref bIncrementAddressIfNotBit, nArrayDim, nStringLen, ref szVarType,
                    szArrayName, szDescription, ref szTempAddress,
                    lParentID, inRootItem);
            }
            else
                InsertSimpleTypeArray(nVarType, nDBNumber, ref nCurrentAddress, ref nBitNumber,
                ref bIncrementAddressIfNotBit, nArrayDim, 0, ref szVarType, szArrayName, szDescription,
                lParentID, inRootItem);
        }

        private void AddStandardVar(uint nDBNumber, String szNamePrefix, string szVarName,
            string szAddress, string szVarType, string szDescription, uint nStringLen,
            int lParentID = -1, ImportData inRootItem = null)
        {
            // Add the variable to the dialog list
            DataType nType = DataType.Boolean;
            int nConversion = 0;
            uint nVarSize = 0;
            if (GetMoviconTypeId(szVarType, ref nConversion, ref nVarSize, ref nType))
            {
                var IVar = importDataModel.addImportData();
                IVar.parentId = lParentID;
                if (nDBNumber > 0)
                    ((ImportDataS7Tcp)IVar).PreName = string.Format("DB{0}", nDBNumber);
                IVar.Name = szNamePrefix + szVarName;
                ((ImportDataS7Tcp)IVar).Type = nType;
                IVar.szType = szVarType;
                if (szVarType == "STRING")
                    nVarSize = nStringLen + 2;
                ((ImportDataS7Tcp)IVar).Size = nVarSize;
                ((ImportDataS7Tcp)IVar).ElemType = DataType.Boolean;
                IVar.Address = szAddress;
                IVar.Description = szDescription;
                IVar.Id = m_lGlobalID++;
                IVar.ArrayDimension = 0;
                ((ImportDataS7Tcp)IVar).ImportType = ImportTypes.Standard;

                AddTreeItem(IVar, inRootItem);
            }
        }

        private void ParseDataTypeAWL(ref System.IO.StreamReader readFile)
        {
            string line;

            m_mapUDT.Clear();

            while ((line = readFile.ReadLine()) != null)
            {
                if (AWLUDTGet(ref readFile, line))
                {
                    continue;
                }
            }
            readFile.BaseStream.Seek(0, System.IO.SeekOrigin.Begin);
        }
        void PrepareDataTypeMapsAWL()
        {
            Dictionary<string, List<string>> mapOk = new Dictionary<string, List<string>>();
            Dictionary<string, List<string>> mapTemp = new Dictionary<string, List<string>>();
            bool bOk = true;

            foreach (var itemStruct in m_mapUDT)
            {
                foreach (string itemStructValue in itemStruct.Value)
                    bOk &= (GetStructMemberTypeFromStringAWL(itemStructValue) != ImportTypes.Unknown);
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
                    bool bType = (GetStructMemberTypeFromStringAWL(itemTempValue) != ImportTypes.Unknown);
                    bOk = mapOk.ContainsKey(itemTemp.Key);
                    if (bOk)
                        foreach (string itemOkValue in mapOk[itemTemp.Key])
                            ValDef.Add(itemOkValue);
                    else if (bType)
                        ValDef.Add(itemTempValue);
                }
                mapOk.Add(itemTemp.Key, ValDef);

            }
            m_mapUDT = mapOk;
        }
        ImportTypes GetStructMemberTypeFromStringAWL(String szFieldLine)
        {
            // Get the name and the type of the variable
            string szVarName = string.Empty;
            string szVarType = string.Empty;
            string szDescription = string.Empty;

            bool bIsStruct = false;
            bool bIsArray = false;
            uint nArrayDim = 0;
            uint nStringLen = 0;

            AWLGetVarNameType(szFieldLine, ref szVarName, ref szVarType, ref bIsStruct,
                ref bIsArray, ref nArrayDim, ref szDescription, ref nStringLen);

            uint nFieldSize = 0;
            String nMovType = "";

            // Field type?
            ImportTypes nFieldType = ImportTypes.Unknown;
            nFieldType = GetVarTypeAndSizeAWL(szVarType, ref nFieldSize, ref nMovType);

            return nFieldType;
        }
        ImportTypes GetVarTypeAndSizeAWL(string inSzType, ref uint nVarSize, ref String nType)
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
            if (szType == "BOOL")
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
            else if (szType == "WORD"
                || szType == "COUNTER"
                || szType == "S5TIME")
            {
                nType = "UInt16";
                nVarSize = 2;
                exit = ImportTypes.Standard;
            }
            else if (szType == "DWORD"
                || szType == "TIMER")
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
            else if (szType == "CHAR")
            {
                nType = "SByte";
                nVarSize = 1;
                exit = ImportTypes.Standard;
            }
            else if (szType == "STRING")
            {
                nType = "String";
                nVarSize = 256;
                exit = ImportTypes.Standard;
            }
            else if (m_mapUDT.ContainsKey(inSzType))
            {
                if (m_mapUDT[inSzType].Count != 0)
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
            uint nBitNumber = 0;
            bool bIncrementAddressIfNotBit = false;

            if (m_mapUDT.ContainsKey(szType))
            {
                if (m_mapUDT[szType].Count != 0)
                {
                    string nMovType;
                    string szFieldName;
                    string szFieldType;
                    string szFieldDescription;
                    bool bIsStruct;
                    bool bIsArray;
                    uint nArrayDim;
                    uint nStringLen;


                    foreach (string itemStructValue in m_mapUDT[szType])
                    {
                        nMovType = string.Empty;
                        szFieldName = string.Empty;
                        szFieldType = string.Empty;
                        szFieldDescription = string.Empty;
                        bIsStruct = false;
                        bIsArray = false;
                        nArrayDim = 0;
                        nStringLen = 0;

                        AWLGetVarNameType(itemStructValue, ref szFieldName, ref szFieldType, ref bIsStruct,
                        ref bIsArray, ref nArrayDim, ref szFieldDescription, ref nStringLen);

                        GetAddress(GetTypeId(szFieldType), 0, ref outVal, ref nBitNumber,
                            ref bIncrementAddressIfNotBit, nStringLen, ref szFieldType);

                    }
                }
            }
            if (bIncrementAddressIfNotBit)
                outVal++;
            return (outVal);
        }
        ImportTypes GetStructMemberTypeFromStringAWL(String szFieldLine, ref uint nFieldSize, ref String nMovType)
        {
            // Field type?
            ImportTypes nFieldType = ImportTypes.Unknown;

            // Get the name and the type of the variable
            string szVarName = string.Empty;
            string szVarType = string.Empty;
            string szDescription = string.Empty;

            bool bIsStruct = false;
            bool bIsArray = false;
            uint nArrayDim = 0;
            uint nStringLen = 0;

            AWLGetVarNameType(szFieldLine, ref szVarName, ref szVarType, ref bIsStruct,
                ref bIsArray, ref nArrayDim, ref szDescription, ref nStringLen);

            nFieldType = GetVarTypeAndSizeAWL(szVarType, ref nFieldSize, ref nMovType);

            return nFieldType;
        }

        bool AddStruct(uint nDBNumber, ref uint nCurrentAddress, ref uint nBitNumber, ref bool bIncrementAddressIfNotBit,
            String szNamePrefix, String szVarName, String description,
            String szVarType, int parentId = -1, ImportData inRootItem = null)
        {
            if (!AWLUDTFound(szVarType))
                return false;

            String szStructNamePrefix = szNamePrefix + szVarName + ".";

            int lParentID = parentId;

            uint nFieldSize = 0;

            if (inRootItem == null)
                lParentID = -1;

            ImportData rootItem = importDataModel.addImportData();
            rootItem.parentId = lParentID;
            lParentID = m_lGlobalID++;
            rootItem.Id = lParentID;
            ((ImportDataS7Tcp)rootItem).PreName = string.Format("DB{0}", nDBNumber);
            rootItem.Name = szVarName;
            rootItem.szType = szVarType;
            ((ImportDataS7Tcp)rootItem).Type = DataType.Boolean;
            rootItem.Description = description;
            ((ImportDataS7Tcp)rootItem).Size = nFieldSize;
            rootItem.ArrayDimension = 0;

            ImportTypes nFieldType;
            ImportTypes elementType;
            string nMovType;
            string szFieldName;
            string szFieldType;
            string szFieldDescription;
            bool bIsStruct;
            bool bIsArray;
            uint nArrayDim;
            uint nStringLen;

            bool isFirst = true;

            foreach (string itemStructValue in m_mapUDT[szVarType])
            {
                nMovType = string.Empty;
                szFieldName = string.Empty;
                szFieldType = string.Empty;
                szFieldDescription = string.Empty;
                bIsStruct = false;
                bIsArray = false;
                nArrayDim = 0;
                nStringLen = 0;

                AWLGetVarNameType(itemStructValue, ref szFieldName, ref szFieldType, ref bIsStruct,
                ref bIsArray, ref nArrayDim, ref szFieldDescription, ref nStringLen);

                if (szFieldName.Length == 0 || szFieldType.Length == 0)
                {
                    continue;
                }


                elementType = GetVarTypeAndSizeAWL(szFieldType, ref nFieldSize, ref nMovType);


                if (nArrayDim != 0)
                {
                    nFieldType = ImportTypes.Array;
                }
                else
                {
                    nFieldType = elementType;
                }

                String elemDescription = (!string.IsNullOrEmpty(description)) ? description + ", " + szFieldDescription : "";

                Opc.Ua.BuiltInType nVarType = GetTypeId(szFieldType);
                string szAddress = GetAddress(nVarType, nDBNumber, ref nCurrentAddress, ref nBitNumber,
                    ref bIncrementAddressIfNotBit, nStringLen, ref szFieldType);


                // Add the field to the dialog list
                switch (nFieldType)
                {
                    // Variable of standard type
                    case ImportTypes.Standard:
                        if (nMovType != "String")
                        {
                            AddStructRoot(inRootItem, rootItem, ref isFirst, szAddress, szVarType);
                            AddStandardVar(nDBNumber, "", szFieldName,
                                szAddress, szFieldType, elemDescription, 0, lParentID, rootItem);
                        }
                        else
                        {
                            AddStandardVar(nDBNumber, szStructNamePrefix, szFieldName,
                                szAddress, szFieldType, elemDescription, nFieldSize);
                        }
                        break;
                    // Array
                    case ImportTypes.Array:

                        if (elementType == ImportTypes.Standard)
                        {
                            AddStructRoot(inRootItem, rootItem, ref isFirst, szAddress, szVarType);
                            AddArray(nDBNumber, ref nCurrentAddress, ref nBitNumber, ref bIncrementAddressIfNotBit,
                                "", szFieldName, szAddress, szFieldType, elemDescription, nFieldSize,
                                nArrayDim, elementType, lParentID, rootItem);

                        }
                        else
                        {
                            AddArray(nDBNumber, ref nCurrentAddress, ref nBitNumber, ref bIncrementAddressIfNotBit,
                                szStructNamePrefix, szFieldName, szAddress, szFieldType,
                                 elemDescription, nFieldSize, nArrayDim, elementType);
                        }
                        break;
                    // Structure 
                    case ImportTypes.Struct:
                        if (AddStruct(nDBNumber, ref nCurrentAddress, ref nBitNumber, ref bIncrementAddressIfNotBit,
                            szStructNamePrefix, szFieldName, elemDescription, szFieldType, lParentID, rootItem))
                            AddStructRoot(inRootItem, rootItem, ref isFirst, szAddress, szVarType);


                        break;
                }
            }

            if (bIncrementAddressIfNotBit)
            {
                nCurrentAddress++;
                bIncrementAddressIfNotBit = false;
            }
            nCurrentAddress += (nCurrentAddress % 2);

            return !isFirst;
        }

        private void AddStructRoot(ImportData inRootItem, ImportData rootItem, ref bool isFirst, string szAddress, string szVarType)
        {
            if (isFirst)
            {
                isFirst = false;
                rootItem.Address = szAddress;
                ((ImportDataS7Tcp)rootItem).ImportType = ImportTypes.Struct;
                rootItem.szType = szVarType;
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

        bool AWLGetStructure(ref System.IO.StreamReader awlFile, string structName, string description,
            ref uint nCurrentAddress, ref uint nDBNumber, ref uint nBitNumber, ref bool bIncrementAddressIfNotBit,
            ref uint nGlobalLevel, ref List<string> structTree)
        {
            if (structName.Length == 0)
                return false;

            var structure = importDataModel.addImportData();
            uint nVarSize = 0;
            if (nDBNumber > 0)
                ((ImportDataS7Tcp)structure).PreName = string.Format("DB{0}", nDBNumber);//https://support/Products/default.asp?7797
            structure.Name = structName;
            ((ImportDataS7Tcp)structure).Type = DataType.Boolean;

            structure.szType = "STRUCT";

            ((ImportDataS7Tcp)structure).Size = nVarSize;
            ((ImportDataS7Tcp)structure).ElemType = DataType.Boolean;
            structure.Address = string.Empty;
            structure.Description = description;
            structure.Id = m_lGlobalID++;
            ((ImportDataS7Tcp)structure).ImportType = ImportTypes.Struct;

            // FOGBUGZ 11412 and 11413
            structure.ArrayDimension = 0;

            bool firstelement = true;


            string szFileRecord;
            while (((szFileRecord = awlFile.ReadLine()) != null))
            {
                if (!AWLIsEndOfStruct(szFileRecord))
                {
                    szFileRecord = szFileRecord.Trim();
                    if (szFileRecord.Length != 0)
                    {
                        //add member
                        string szVarName = string.Empty;
                        string szVarType = string.Empty;
                        string szDescription = string.Empty;

                        bool bIsStruct = false;
                        bool bIsArray = false;
                        uint nArrayDim = 0;
                        uint nStringLen = 0;
                        AWLGetVarNameType(szFileRecord, ref szVarName, ref szVarType, ref bIsStruct,
                        ref bIsArray, ref nArrayDim, ref szDescription, ref nStringLen);

                        string szAddress;
                        bool bFirstVarOfStruct = false;
                        if (bIsStruct)
                        {
                            //Struct or array of struct
                            //To make the structures word aligned
                            bFirstVarOfStruct = true;

                            nGlobalLevel++;
                            //add current struct name 
                            structTree.Add(szVarName);
                            //define struct name as a sum of of struct sublevel
                            szVarName = AWLGetStructureName(structTree);

                            if (bIsArray)
                            {
                            }
                            else
                            {
                                AWLGetStructure(ref awlFile, szVarName, szDescription, ref nCurrentAddress,
                                    ref nDBNumber, ref nBitNumber, ref bIncrementAddressIfNotBit, ref nGlobalLevel, ref structTree);

                            }
                            continue;
                        }
                        if (szVarName.Length == 0 || szVarType.Length == 0)
                            continue;

                        Opc.Ua.BuiltInType nVarType = GetTypeId(szVarType);

                        if (bIsArray && nVarType >= 0)
                        {
                            // FOGBUGZ 11412 and 11413
                            //if (nVarType == Opc.Ua.BuiltInType.Boolean || nVarType == Opc.Ua.BuiltInType.String)
                            if (nVarType == Opc.Ua.BuiltInType.String)

                            {
                                string szTempAddress = string.Empty;
                                InsertArrayMembers(nVarType, nDBNumber, ref nCurrentAddress, ref nBitNumber,
                                    ref bIncrementAddressIfNotBit, nArrayDim, nStringLen, ref szVarType, szVarName, szDescription, ref szTempAddress);
                            }
                            else
                                InsertSimpleTypeArray(nVarType, nDBNumber, ref nCurrentAddress, ref nBitNumber,
                                ref bIncrementAddressIfNotBit, nArrayDim, nStringLen, ref szVarType, szVarName, szDescription);

                        }
                        else if (!bIsArray)
                        {
                            if (bFirstVarOfStruct)
                            {
                                if (bIncrementAddressIfNotBit)
                                {
                                    bIncrementAddressIfNotBit = false;
                                    nCurrentAddress++;
                                }

                                nBitNumber = 0;
                                if ((nCurrentAddress % 2) > 0)
                                    nCurrentAddress++;
                                bFirstVarOfStruct = false;
                            }

                            szAddress = GetAddress(nVarType, nDBNumber, ref nCurrentAddress, ref nBitNumber,
                                ref bIncrementAddressIfNotBit, nStringLen, ref szVarType);

                            if (szAddress.Length == 0)
                                continue;

                            // Add the variable to the dialog list
                            DataType nType = DataType.Boolean;
                            int nConv = 0;
                            uint nSize = 0;
                            if (GetMoviconTypeId(szVarType, ref nConv, ref nSize, ref nType))
                            {
                                var IVar = importDataModel.addImportData();
                                if (nDBNumber > 0)
                                    ((ImportDataS7Tcp)IVar).PreName = string.Format("DB{0}", nDBNumber);//https://support/Products/default.asp?7797
                                ((ImportDataS7Tcp)IVar).Type = nType;

                                IVar.szType = szVarType;
                                ((ImportDataS7Tcp)IVar).Size = nVarSize;
                                ((ImportDataS7Tcp)IVar).ElemType = DataType.Boolean;
                                IVar.Address = szAddress;
                                IVar.Description = szDescription;
                                IVar.Id = m_lGlobalID++;
                                ((ImportDataS7Tcp)IVar).ImportType = ImportTypes.Standard;

                                // FOGBUGZ 11412 and 11413
                                IVar.ArrayDimension = 0;

                                if (szVarType == "STRING")
                                {
                                    IVar.Name = structName + "." + szVarName;
                                    nVarSize = nStringLen + 2;
                                    AddTreeItem(IVar);

                                }
                                else
                                {
                                    IVar.Name = szVarName;
                                    if (firstelement)
                                    {
                                        firstelement = false;
                                        structure.Address = szAddress;
                                        AddTreeItem(structure);
                                    }
                                    AddTreeItem(IVar, structure);
                                }
                            }

                        }
                    }
                }
                else
                {
                    if (nGlobalLevel == 0)
                    {
                        nBitNumber = 0;
                        if ((nCurrentAddress % 2) > 0)
                            nCurrentAddress++;

                    }
                    if (nGlobalLevel > 0)
                    {
                        nGlobalLevel--;
                        if (structTree.Count > 0)
                            structTree.RemoveAt(structTree.Count - 1);
                    }
                    if (bIncrementAddressIfNotBit)
                    {
                        bIncrementAddressIfNotBit = false;
                        nCurrentAddress++;
                    }

                    nBitNumber = 0;
                    if ((nCurrentAddress % 2) > 0)
                        nCurrentAddress++;
                    break;
                }
            }


            return true;
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

        void AWLIncrementCurAddForNotSuppType(ref uint nCurrentAddress,
                                                      ref string szVarType,
                                                      bool bIsArray,
                                                      uint nArrayDim,
                                                      ref uint nBitNumber,
                                                      ref bool bFirstVarOfStruct,
                                                      ref bool bIncrementAddressIfNotBit)
        {
            szVarType = szVarType.Trim().ToUpper();
            if (szVarType.Length == 0)
            {
                return;
            }

            uint nVarDim = 1; // Default
            if (szVarType == "TIME")
            {
                nVarDim = 4;
            }
            else if (szVarType == "DATE")
            {
                nVarDim = 2;
            }
            else if (szVarType == "TIME_OF_DAY")
            {
                nVarDim = 4;
            }
            else if (szVarType == "DATE_AND_TIME")
            {
                nVarDim = 8;
            }
            else if (szVarType == "STRING")
            {
                nVarDim = 2 + nArrayDim;
            }

            if (!bIsArray)
            {
                //variable

                //Added in version 10.0.0.17
                if (bFirstVarOfStruct)
                {
                    if (bIncrementAddressIfNotBit)
                    {
                        bIncrementAddressIfNotBit = false;
                        nCurrentAddress++;
                    }

                    nBitNumber = 0;
                    if ((nCurrentAddress % 2) > 0)
                    {
                        nCurrentAddress++;
                    }
                    bFirstVarOfStruct = false;
                }
            }
            else
            {
                nVarDim *= nArrayDim;
            }

            if (bIncrementAddressIfNotBit)
            {
                nCurrentAddress++;
                bIncrementAddressIfNotBit = false;
            }

            //May be unaligned even if the previous was a byte var
            if ((nCurrentAddress % 2) > 0)
            {
                nCurrentAddress++;
            }

            nCurrentAddress += nVarDim;

            nBitNumber = 0;

            if (bIsArray)
            {
                if ((nCurrentAddress % 2) > 0)
                {
                    nCurrentAddress++;
                }
            }

            return;
        }

        //string GetAddress(Opc.Ua.BuiltInType nVarType, uint nDBNumber, ref uint nCurrentAddress, ref uint nBitNumber,
        //                        ref bool bIncrementAddressIfNotBit, uint nStringLen, ref string szVarType)
        string GetAddress(Opc.Ua.BuiltInType nVarType, uint nDBNumber, ref uint nCurrentAddress, ref uint nBitNumber,
                                ref bool bIncrementAddressIfNotBit, uint nStringLen, ref string szVarType, bool incrementCurrentAddress = true)
        {

            string szAddress = string.Empty;

            switch (nVarType)
            {
                case Opc.Ua.BuiltInType.Boolean:
                    szAddress = string.Format("DB{0}.DBX{1}.{2}", nDBNumber, nCurrentAddress, nBitNumber);
                    if (incrementCurrentAddress)
                    {
                        nBitNumber++;
                        if (nBitNumber > 7)
                        {
                            nBitNumber = 0;
                            nCurrentAddress++;
                            bIncrementAddressIfNotBit = false;
                        }
                        else
                        {
                            bIncrementAddressIfNotBit = true;
                        }
                    }

                    break;
                case Opc.Ua.BuiltInType.Byte:
                case Opc.Ua.BuiltInType.SByte:
                    if (bIncrementAddressIfNotBit)
                    {
                        nCurrentAddress++;
                        bIncrementAddressIfNotBit = false;
                    }
                    szAddress = string.Format("DB{0}.DBB{1}", nDBNumber, nCurrentAddress);
                    if (incrementCurrentAddress)
                    {
                        nCurrentAddress++;
                    }
                    nBitNumber = 0;
                    break;
                case Opc.Ua.BuiltInType.UInt16:
                case Opc.Ua.BuiltInType.Int16:
                    if (bIncrementAddressIfNotBit)
                    {
                        nCurrentAddress++;
                        bIncrementAddressIfNotBit = false;
                    }
                    if ((nCurrentAddress % 2) > 0)
                    {
                        nCurrentAddress++;
                    }
                    szAddress = string.Format("DB{0}.DBW{1}", nDBNumber,
                    nCurrentAddress);
                    if (incrementCurrentAddress)
                    {
                        nCurrentAddress += 2;
                    }
                    nBitNumber = 0;
                    break;
                case Opc.Ua.BuiltInType.UInt32:
                case Opc.Ua.BuiltInType.Int32:
                case Opc.Ua.BuiltInType.Float:
                    if (bIncrementAddressIfNotBit)
                    {
                        nCurrentAddress++;
                        bIncrementAddressIfNotBit = false;
                    }
                    if ((nCurrentAddress % 2) > 0)
                    {
                        nCurrentAddress++;
                    }
                    szAddress = string.Format("DB{0}.DBD{1}", nDBNumber,
                    nCurrentAddress);
                    if (incrementCurrentAddress)
                    {
                        nCurrentAddress += 4;
                    }
                    nBitNumber = 0;
                    break;
                case Opc.Ua.BuiltInType.String:
                    if (bIncrementAddressIfNotBit)
                    {
                        nCurrentAddress++;
                        bIncrementAddressIfNotBit = false;
                    }
                    //Added in version 10.0.0.20
                    //May be unaligned even if the previous was a byte var
                    if ((nCurrentAddress % 2) > 0)
                    {
                        nCurrentAddress++;
                    }

                    nCurrentAddress += 2;

                    szAddress = string.Format("DB{0}.DBB{1}:{2}", nDBNumber,
                    nCurrentAddress, nStringLen);

                    if (incrementCurrentAddress)
                    {
                        nCurrentAddress += nStringLen;
                    }

                    nBitNumber = 0;
                    szVarType = "STRING";
                    break;
                default:
                    if (AWLUDTFound(szVarType))
                    {
                        if (bIncrementAddressIfNotBit)
                        {
                            nCurrentAddress++;
                            bIncrementAddressIfNotBit = false;
                        }
                        nCurrentAddress += (nCurrentAddress % 2);

                        szAddress = string.Format("DB{0}.DBD{1}", nDBNumber,
                        nCurrentAddress);
                        if (incrementCurrentAddress)
                        {
                            nCurrentAddress += StructSize(szVarType);
                        }
                    }
                    break;

            }
            return szAddress;
        }


        bool AWLUDTGet(ref System.IO.StreamReader fileAWL, string szLine)
        {
            // Check if the line is the beginning of a UDT declaration
            int nIndex = szLine.IndexOf("TYPE");
            if (nIndex < 0)
            {
                return (false);
            }
            if (szLine.Length <= (4 + nIndex))
            {
                return (false);
            }

            // Get the UDT name
            string szUDTName = szLine.Substring(5 + nIndex);
            szUDTName = szUDTName.Trim();
            RemoveBlanks(ref szUDTName);
            if (szUDTName.Length == 0)
            {
                return (false);
            }

            // Save the current position in the file
            // (just in case something goes wrong)
            long ulFirstFilePos = fileAWL.BaseStream.Position;

            // Store the UDT definition in a String Array
            List<string> pStringArray = new List<string>();

            string szFileRecord;
            bool bEndOfUDTFound = false;
            bool bInitialStructFound = false;
            bool bDefinitionComplete = false;
            while (!bEndOfUDTFound && ((szFileRecord = fileAWL.ReadLine()) != null))
            {
                if (!AWLUDTIsEnd(szFileRecord))
                {
                    if (!bInitialStructFound)
                    {
                        bInitialStructFound = AWLIsTheInitialStructLine(szFileRecord);
                        continue;

                    }
                    else
                    {
                        if (!bDefinitionComplete)
                        {
                            bDefinitionComplete = AWLIsEndOfStruct(szFileRecord);
                            if (!bDefinitionComplete)
                            {
                                szFileRecord = szFileRecord.Trim();
                                if (szFileRecord.Length != 0)
                                {
                                    pStringArray.Add(szFileRecord);
                                }
                            }
                        }
                    }
                }
                else
                {
                    bEndOfUDTFound = true;
                }
            }

            // Check if the UDT definition has been correctly read
            if (!bEndOfUDTFound || !bDefinitionComplete || pStringArray.Count == 0)
            {
                pStringArray.Clear();
                fileAWL.BaseStream.Seek(ulFirstFilePos, System.IO.SeekOrigin.Begin);
                return (false);
            }

            // Add the UDT definition to the corresponding map
            m_mapUDT.Add(szUDTName, pStringArray);

            return (true);
        }

        bool AWLUDTIsEnd(string szLine)
        {
            // Search for the string "END_TYPE"
            int nIndex = szLine.IndexOf("END_TYPE");
            if (nIndex >= 0)
            {
                return true;
            }

            return (false);
        }

        void AWLGetDBNumber(string szLine, ref string szDBName,
                                   ref uint nDBNumber)
        {
            // Init the output parameters
            szDBName = string.Empty;
            nDBNumber = 0;

            // Skip empty lines
            if (szLine.Length == 0)
            {
                return;
            }

            // Search for the string "DATA_BLOCK"
            int nIndex = szLine.IndexOf("DATA_BLOCK");

            // String "DATA_BLOCK" not found?
            if (nIndex < 0)
            {
                return;
            }

            // Name of the data block not present?
            if ((nIndex + 10) > (szLine.Length - 1))
            {
                return;
            }

            string szAux = szLine.Substring(nIndex + 10);

            // Search for the string "DB"
            nIndex = szAux.IndexOf("DB");

            if ((nIndex >= 0) && ((nIndex + 2) <= (szAux.Length - 1)))
            {
                string szAux2 = szAux.Substring(nIndex + 2);
                szAux2 = szAux2.Trim();
                if (szAux2.Length != 0)
                {
                    // Get the Data Block number
                    int nDBNum = Convert.ToInt16(szAux2);

                    if (nDBNum > 0)
                    {
                        // Set the output parameters: Data Block name and number
                        szDBName = string.Format("DB{0}", nDBNum);
                        nDBNumber = (uint)nDBNum;
                    }
                }
            }
        }

        bool AWLIsTheInitialStructLine(string szLine)
        {
            // Search for the string "STRUCT"
            return (szLine.ToUpper().IndexOf("STRUCT") >= 0);
        }

        bool AWLUDTFound(string szType)
        {
            string szVarType = szType.Trim();
            if (szVarType.Length == 0)
                return (false);

            if (m_mapUDT.ContainsKey(szVarType))
            {
                if (m_mapUDT[szVarType].Count == 0)
                    return (false);
                return (true);
            }

            return (false);
        }

        bool AWLIsEndOfStruct(string szLine)
        {
            // Search for the string "END_STRUCT"
            return (szLine.ToUpper().IndexOf("END_STRUCT") >= 0);
        }

        void RemoveStructName(ref string szStructName, ref string szCurStructName)
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

        void AWLGetVarNameType(string szLine, ref string szVarName, ref string szVarType,
            ref bool bIsStruct, ref bool bIsArray, ref uint nArrayDim, ref string szDesc, ref uint nStringLen)
        {
            // Init the output parameters
            szVarName = string.Empty;
            szVarType = string.Empty;
            bIsStruct = false;
            bIsArray = false;
            nArrayDim = 0;

            int nCom = szLine.IndexOf("//");
            int nNewLine = szLine.IndexOf("\n");
            if (nCom > 0 && nNewLine > nCom)
            {
                szDesc = szLine.Substring(nCom + 2, nNewLine - nCom - 2);
                szLine = szLine.Substring(0, nCom) + szLine.Substring(nNewLine + 1);
            }
            else if (nCom > 0)
            {
                szDesc = szLine.Substring(nCom + 2);
                szLine = szLine.Substring(0, nCom);
            }
            else
            {
                szDesc = string.Empty;
            }

            // Search for the character ':'
            int nIndex = szLine.IndexOf(":");
            if ((nIndex > 0) && (nIndex < (szLine.Length - 1)))
            {
                // Get the variable name
                szVarName = szLine.Substring(0, nIndex);
                RemoveBlanks(ref szVarName);

                string szAux = szLine.Substring(nIndex + 1);

                //Decide the type of item
                bIsArray = (szAux.ToUpper().IndexOf("ARRAY") >= 0);
                bIsStruct = (szAux.ToUpper().IndexOf("STRUCT") >= 0);

                if (bIsStruct && bIsArray)
                {
                    //Array of structures
                    szVarType = string.Empty;

                    // Get the ARRAY elements number and type
                    nIndex = szAux.IndexOf("[");
                    if ((nIndex >= 0) && (nIndex < (szAux.Length - 1)))
                    {
                        string szAux2 = szAux.Substring(nIndex + 1);
                        nIndex = szAux2.IndexOf("..");
                        if ((nIndex > 0) && (nIndex < (szAux.Length - 2)))
                        {
                            szAux = szAux2.Substring(0, nIndex).Trim();
                            uint nLowLimit = 0;
                            if (szAux.Length != 0)
                            {
                                nLowLimit = Convert.ToUInt16(szAux);
                                szAux = szAux2.Substring(nIndex + 2);
                                nIndex = szAux.IndexOf("]");
                                if (nIndex > 0)
                                {
                                    szAux2 = szAux.Substring(0, nIndex).Trim();
                                    uint nHighLimit = nLowLimit;
                                    if (szAux2.Length != 0)
                                    {
                                        nHighLimit = Convert.ToUInt16(szAux2);
                                        if (nHighLimit > nLowLimit)
                                        {
                                            nIndex = szAux.IndexOf(" OF ");
                                            if ((nIndex > 0) &&
                                                (nIndex < (szAux.Length - 4)))
                                            {
                                                szAux2 = szAux.Substring(nIndex + 4).Trim();
                                                szAux = szAux2.Substring(0, nIndex).Trim();
                                                if (szAux.Length != 0)
                                                {
                                                    // Set the type string
                                                    szVarType = szAux;
                                                    if (szVarType.ToUpper() == "STRING")
                                                    {//look for string size
                                                        szAux2 = szAux2.Trim();
                                                        int nIndex1 = szAux2.IndexOf("[");
                                                        int nIndex2 = szAux2.IndexOf("]");
                                                        if (nIndex1 != -1 && nIndex1 != -1)
                                                        {
                                                            szAux = szAux2.Substring(nIndex1 + 1, nIndex2 - nIndex1 - 1);
                                                            if (szAux.Length != 0)
                                                                nStringLen = Convert.ToUInt16(szAux);
                                                        }
                                                    }
                                                    // Set the array size
                                                    nArrayDim = nHighLimit - nLowLimit + 1;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }

                }
                else if (bIsArray)
                {
                    //Array of variables
                    // Reset the type string
                    szVarType = string.Empty;

                    // Get the ARRAY elements number and type
                    nIndex = szAux.IndexOf("[");
                    if ((nIndex >= 0) && (nIndex < (szAux.Length - 1)))
                    {
                        string szAux2 = szAux.Substring(nIndex + 1);
                        nIndex = szAux2.IndexOf("..");
                        if ((nIndex > 0) && (nIndex < (szAux.Length - 2)))
                        {
                            szAux = szAux2.Substring(0, nIndex).Trim();
                            uint nLowLimit = 0;
                            if (szAux.Length != 0)
                            {
                                nLowLimit = Convert.ToUInt16(szAux);
                                szAux = szAux2.Substring(nIndex + 2);
                                nIndex = szAux.IndexOf("]");
                                if (nIndex > 0)
                                {
                                    szAux2 = szAux.Substring(0, nIndex).Trim();
                                    uint nHighLimit = nLowLimit;
                                    if (szAux2.Length != 0)
                                    {
                                        nHighLimit = Convert.ToUInt16(szAux2);
                                        if (nHighLimit > nLowLimit)
                                        {
                                            nIndex = szAux.IndexOf(" OF ");
                                            if ((nIndex > 0) &&
                                                (nIndex < (szAux.Length - 4)))
                                            {
                                                szAux2 = szAux.Substring(nIndex + 4).Trim();
                                                nIndex = szAux2.IndexOf(";"); //https://support.progea.com/Products/default.asp?7539
                                                if (nIndex > 0)
                                                {
                                                    int nIndex2p = szAux2.IndexOf(":=");
                                                    if ((nIndex2p > 0) && (nIndex2p < nIndex))
                                                    {
                                                        nIndex = nIndex2p;
                                                    }

                                                    szAux = szAux2.Substring(0, nIndex).Trim();
                                                    if (szAux.Length != 0)
                                                    {
                                                        //////////////////////////////////////////////////////////////////////////
                                                        //https://support.progea.com/Products/default.asp?7575
                                                        if (szAux.ToUpper().IndexOf("STRING") == 0)
                                                        {
                                                            // Set the type string
                                                            szVarType = "STRING";
                                                            //////////////////////////////////////////////////////////////////////////
                                                            //look for string size
                                                            szAux2 = szAux2.Trim();
                                                            int nIndex1 = szAux2.IndexOf("[");
                                                            int nIndex2 = szAux2.IndexOf("]");
                                                            if (nIndex1 != -1 && nIndex1 != -1)
                                                            {
                                                                szAux = szAux2.Substring(nIndex1 + 1, nIndex2 - nIndex1 - 1);
                                                                if (szAux.Length != 0)
                                                                    nStringLen = Convert.ToUInt16(szAux);
                                                            }
                                                        }
                                                        //////////////////////////////////////////////////////////////////////////
                                                        //https://support.progea.com/Products/default.asp?7575
                                                        else //array of UDT?
                                                            szVarType = szAux;
                                                        //////////////////////////////////////////////////////////////////////////

                                                        // Set the array size
                                                        nArrayDim = nHighLimit - nLowLimit + 1;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                else if (bIsStruct)
                {
                    //Structure definition
                    szVarType = szAux;
                    RemoveBlanks(ref szVarType);
                }
                else
                {
                    //Variable
                    nArrayDim = 0;
                    nIndex = szAux.IndexOf("[");
                    int nEndArray = szAux.IndexOf("]");
                    if (nIndex > 0 && nEndArray > nIndex)
                    {
                        //got array dimension
                        string szDim = szAux.Substring(nIndex + 1, nEndArray - nIndex - 1);
                        RemoveBlanks(ref szDim);
                        nArrayDim = Convert.ToUInt16(szDim);
                    }
                    else
                    {
                        nIndex = szAux.IndexOf(";");
                    }
                    //get the variable type
                    if (nIndex > 0)
                    {
                        szVarType = szAux.Substring(0, nIndex);

                        szAux = szVarType;
                        nIndex = szAux.IndexOf(":");
                        if (nIndex > 0)
                        {
                            szVarType = szAux.Substring(0, nIndex);
                        }

                        szVarType = szVarType.Trim();
                        if (szVarType.ToUpper() == "STRING")
                        {
                            nStringLen = nArrayDim;
                            if (nStringLen == 0)
                            {
                                nStringLen = S7Protocol.DEFAULTSTRINGLENGTH_TIAPORTAL; // Default string size
                            }
                            nArrayDim = 0;
                        }
                    }
                }
                // Check if the type is ARRAY
                if (szVarType.ToUpper() == "ARRAY")
                {
                    bIsArray = true;
                }

                // Check if the type is STRUCT
                else if (szVarType.ToUpper() == "STRUCT")
                {
                    bIsStruct = true;
                }
            }
            RemoveBlanks(ref szVarType);
        }

        void RemoveBlanks(ref string str)
        {
            str = str.Replace(" ", string.Empty);
        }

        Opc.Ua.BuiltInType GetTypeId(string szType)
        {
            Opc.Ua.BuiltInType nType = Opc.Ua.BuiltInType.Null;
            szType = szType.Trim();
            if (szType.Length == 0)
            {
                return (nType);
            }

            szType = szType.ToUpper();
            if (szType == "BOOL")
            {
                nType = Opc.Ua.BuiltInType.Boolean;
            }
            else if (szType == "BYTE")
            {
                nType = Opc.Ua.BuiltInType.Byte;
            }
            else if (szType == "WORD"
                || szType == "COUNTER")
            {
                nType = Opc.Ua.BuiltInType.UInt16;
            }
            else if (szType == "S5TIME")
            {
                nType = Opc.Ua.BuiltInType.UInt16;
            }
            else if (szType == "DWORD"
                || szType == "TIMER")
            {
                nType = Opc.Ua.BuiltInType.UInt32;
            }
            else if (szType == "INT")
            {
                nType = Opc.Ua.BuiltInType.Int16;
            }
            else if (szType == "DINT")
            {
                nType = Opc.Ua.BuiltInType.Int32;
            }
            else if (szType == "REAL")
            {
                nType = Opc.Ua.BuiltInType.Float;
            }
            else if (szType == "CHAR")
            {
                nType = Opc.Ua.BuiltInType.SByte;
            }
            else if (szType == "STRING")
            {
                nType = Opc.Ua.BuiltInType.String;
            }

            return (nType);
        }

        void InsertArrayMembers(Opc.Ua.BuiltInType nVarType, uint nDBNumber, ref uint nCurrentAddress, ref uint nBitNumber,
            ref bool bIncrementAddressIfNotBit, uint nArrayDim, uint nStringLen, ref string szVarType,
            string szVarName, string szDescription, ref string szFirstVarAddress,
            int lParentID = -1, ImportData inRootItem = null)
        {

            // Case "array after a bool variable": the address must be
            // increased and the bit number must be reset
            nBitNumber = 0;
            if (bIncrementAddressIfNotBit)
            {
                nCurrentAddress++;
                bIncrementAddressIfNotBit = false;
            }

            if ((nCurrentAddress % 2) > 0)
                nCurrentAddress++;

            string szFieldLine;
            uint i = 0;
            int nConversion = 0;
            uint nVarSize = 0;

            DataType nElemType = DataType.Boolean;
            if (!GetMoviconTypeId(szVarType, ref nConversion, ref nVarSize, ref nElemType))
                return;
            for (i = 0; i < nArrayDim; i++)
            {
                var f = importDataModel.addImportData();
                szFieldLine = string.Format("{0}[{1}]", szVarName, i);
                if (nDBNumber > 0)
                    ((ImportDataS7Tcp)f).PreName = string.Format("DB{0}", nDBNumber);//https://support/Products/default.asp?7797
                f.Name = szFieldLine;

                if (szVarType == "STRING")
                    nVarSize = nStringLen + 2;

                ((ImportDataS7Tcp)f).Size = nVarSize;
                f.szType = szVarType;
                ((ImportDataS7Tcp)f).Type = nElemType;
                f.Address = GetAddress(nVarType, nDBNumber, ref nCurrentAddress, ref nBitNumber,
                    ref bIncrementAddressIfNotBit, nStringLen, ref szVarType);
                if (i == 0)
                    szFirstVarAddress = f.Address;

                f.Description = szDescription;
                f.Id = m_lGlobalID++;

                // FOGBUGZ 11412 and 11413
                f.ArrayDimension = 0;

                f.parentId = lParentID;
                ((ImportDataS7Tcp)f).ImportType = ImportTypes.Standard;

                AddTreeItem(f, inRootItem);

            }
            //Last iteration? --> Set to 0 the bit number and
            // increment the byte address
            if (nVarType == Opc.Ua.BuiltInType.Boolean && nBitNumber != 0)
            {
                nBitNumber = 0;
                nCurrentAddress++;
                bIncrementAddressIfNotBit = false;
            }

            if ((nCurrentAddress % 2) > 0)
                nCurrentAddress++;
        }



        void InsertSimpleTypeArray(Opc.Ua.BuiltInType nVarType,
            uint nDBNumber, ref uint nCurrentAddress, ref uint nBitNumber, ref bool bIncrementAddressIfNotBit,
            uint nArrayDim, uint nStringLen, ref string szVarType, string szVarComplName, string szDescription,
            int lParentID = -1, ImportData inRootItem = null)
        {
            string szAddress;

            // Case "array after a bool variable": the address must be
            // increased and the bit number must be reset
            nBitNumber = 0;
            if (bIncrementAddressIfNotBit)
            {
                nCurrentAddress++;
                bIncrementAddressIfNotBit = false;
            }

            if ((nCurrentAddress % 2) > 0)
                nCurrentAddress++;

            // Added in version 10.1.0.11, FOGBUGZ 8283: errors importing arrays
            uint nCurrentAddressLastValue = nCurrentAddress;

            szAddress = GetAddress(nVarType, nDBNumber, ref nCurrentAddress, ref nBitNumber,
                 ref bIncrementAddressIfNotBit, nStringLen, ref szVarType);
            nBitNumber--;

            // Added in version 10.1.0.11, FOGBUGZ 8283: errors importing arrays
            nCurrentAddress = nCurrentAddressLastValue;

            DataType nType = DataType.Boolean;
            int nConversion = 0;
            uint nVarSize = 0;
            if (!GetMoviconTypeId(szVarType, ref nConversion, ref nVarSize, ref nType))
                return;
            var IVar = importDataModel.addImportData();
            if (szVarType.ToUpper() == "STRING")
                nVarSize = nStringLen + 2;

            if (nDBNumber > 0)
                ((ImportDataS7Tcp)IVar).PreName = string.Format("DB{0}", nDBNumber);//https://support/Products/default.asp?7797
            IVar.Name = szVarComplName;
            ((ImportDataS7Tcp)IVar).ElemType = nType;

            // FOGBUGZ 11412 and 11413
            //IVar.Type = DataType.Boolean;
            ((ImportDataS7Tcp)IVar).Type = nType;

            ((ImportDataS7Tcp)IVar).Size = nArrayDim * nVarSize;
            IVar.szType = "ARRAY OF " + szVarType;
            IVar.Address = szAddress;
            IVar.Description = szDescription;
            IVar.Id = m_lGlobalID++;

            // FOGBUGZ 11412 and 11413
            IVar.ArrayDimension = nArrayDim;

            IVar.parentId = lParentID;
            ((ImportDataS7Tcp)IVar).ImportType = ImportTypes.Array;

            AddTreeItem(IVar, inRootItem);

            string szFieldLine;
            uint i = 0;
            for (i = 0; i < nArrayDim; i++)
            {
                var f = importDataModel.addImportData();
                szFieldLine = string.Format("{0}[{1}]", szVarComplName, i);

                if (nDBNumber > 0)
                    ((ImportDataS7Tcp)f).PreName = string.Format("DB{0}", nDBNumber);//https://support/Products/default.asp?7797
                f.Name = szFieldLine;
                ((ImportDataS7Tcp)f).Size = nVarSize;
                f.szType = szVarType;
                ((ImportDataS7Tcp)f).Type = ((ImportDataS7Tcp)IVar).ElemType;
                f.Address = GetAddress(nVarType, nDBNumber, ref nCurrentAddress, ref nBitNumber,
                    ref bIncrementAddressIfNotBit, nStringLen, ref szVarType);

                f.Description = szDescription;
                f.Id = m_lGlobalID++;

                // FOGBUGZ 11412 and 11413
                f.ArrayDimension = 0;

                f.parentId = IVar.Id;
                ((ImportDataS7Tcp)f).ImportType = ImportTypes.Standard;

                AddTreeItem(f, IVar);

            }
            //Last iteration? --> Set to 0 the bit number and
            // increment the byte address
            if (nVarType == Opc.Ua.BuiltInType.Boolean && nBitNumber != 0)
            {
                nBitNumber = 0;
                nCurrentAddress++;
                bIncrementAddressIfNotBit = false;
            }
            if ((nCurrentAddress % 2) > 0)
                nCurrentAddress++;

        }


        private void DisplayImportFileSdf(string file)
        {
            try
            {
                if (importDataModel != null)
                    importDataModel.Dispose();
                importDataModel = new ImportDataModelS7Tcp(readStationName, readAddDBnumber);

                System.IO.StreamReader readFile = new System.IO.StreamReader(file);

                string line;

                while ((line = readFile.ReadLine()) != null)
                {
                    var IVar = importDataModel.addImportData();
                    int nStart = line.IndexOf('"', 0);
                    if (nStart == -1)
                        return;

                    int nEnd = line.IndexOf('"', nStart + 1);
                    if (nEnd == -1)
                        return;

                    string strSymb = line.Substring(nStart + 1, nEnd - nStart - 1);
                    RemoveBlanks(ref strSymb);


                    nStart = line.IndexOf('"', nEnd + 1);
                    if (nStart == -1)
                        return;

                    nEnd = line.IndexOf('"', nStart + 1);
                    if (nEnd == -1)
                        return;

                    string strAddr = line.Substring(nStart + 1, nEnd - nStart - 1);
                    if (strAddr.Length == 0)
                        continue;

                    //if address is invalid or unsupported, we skip the line
                    RemoveBlanks(ref strAddr);


                    Step7Area area = Step7Area.aInvalid;
                    int dbnumber = -1;
                    Step7Format format = Step7Format.frmInvalid;
                    Step7WordTrans trans = Step7WordTrans.wtC;
                    int offset = 0;
                    int bit = 0;
                    int length = 0;
                    bool s7_200 = false;
                    if (!S7TCPDynTagSettings.StaticParseAddress(strAddr, ref area, ref dbnumber,
                        ref format, ref trans, ref offset, ref bit, ref length, ref s7_200))
                        continue;

                    //
                    // Get the variable type.
                    //

                    string strType = string.Empty;
                    nStart = line.IndexOf('"', nEnd + 1);
                    if (nStart != -1)
                    {
                        nEnd = line.IndexOf('"', nStart + 1);
                        if (nEnd != -1)
                        {
                            strType = line.Substring(nStart + 1, nEnd - nStart - 1);
                            RemoveBlanks(ref strType);
                        }
                    }
                    //Added in version 10.0.0.8
                    string strDesc = string.Empty;
                    nStart = line.IndexOf('"', nEnd + 1);
                    if (nStart != -1)
                    {
                        nEnd = line.IndexOf('"', nStart + 1);
                        if (nEnd != -1)
                        {
                            strDesc = line.Substring(nStart + 1, nEnd - nStart - 1).Trim();

                        }
                    }

                    if (strSymb.Length == 0)
                    {
                        strSymb = strAddr;
                        strSymb.Replace(':', '_');
                        strSymb.Replace('.', '_');
                    }

                    string strDlg = strSymb + " - ";
                    strDlg += strAddr + " - ";
                    strDlg += strType;
                    //Added in version 10.0.0.9
                    if (strDesc.Length != 0)
                    {
                        strDlg += " - " + strDesc;
                    }

                    DataType nType = DataType.Boolean;
                    int nConversion = 0;
                    uint nVarSize = uint.MaxValue;
                    if (!GetMoviconTypeId(strType, ref nConversion, ref nVarSize, ref nType))
                        continue;

                    if (dbnumber != -1)
                        ((ImportDataS7Tcp)IVar).PreName = string.Format("DB{0}", dbnumber);//https://support/Products/default.asp?7797
                    IVar.Name = strSymb;
                    ((ImportDataS7Tcp)IVar).Type = nType;
                    ((ImportDataS7Tcp)IVar).Size = nVarSize;
                    IVar.szType = strType;
                    ((ImportDataS7Tcp)IVar).ElemType = DataType.Boolean;
                    IVar.Address = strAddr;
                    IVar.Description = strDesc;
                    IVar.Id = m_lGlobalID++;

                    // FOGBUGZ 11412 and 11413
                    IVar.ArrayDimension = 0;
                    ((ImportDataS7Tcp)IVar).ImportType = ImportTypes.Standard;

                    AddTreeItem(IVar);
                }
                readFile.Close();
                AddDBnumber.Visibility = System.Windows.Visibility.Visible;
            }
            catch (Exception e)
            {
            }
        }

        public bool GetMoviconTypeId(string szType, ref int nConversion, ref uint nVarSize, ref DataType nType)
        {
            bool exit = false;
            szType = szType.Trim();
            szType = szType.ToUpper();
            nConversion = 0;
            nVarSize = 0;
            nType = DataType.Boolean;
            if (szType.Length == 0)
            {
                return (exit);
            }
            if (szType == "BOOL")
            {
                nType = DataType.Boolean;
                nVarSize = 1;
                exit = true;
            }
            else if (szType == "BYTE")
            {
                nType = DataType.Byte;
                nVarSize = 1;
                exit = true;
            }
            else if (szType == "WORD"
                || szType == "COUNTER")
            {
                nType = DataType.UInt16;
                nVarSize = 2;
                exit = true;
            }
            else if (szType == "S5TIME")
            {
                nType = DataType.UInt32;
                nConversion = 1;
                nVarSize = 4;
                exit = true;
            }
            else if (szType == "DWORD"
                || szType == "TIMER")
            {
                nType = DataType.UInt32;
                nVarSize = 4;
                exit = true;
            }
            else if (szType == "INT")
            {
                nType = DataType.Int16;
                nVarSize = 2;
                exit = true;
            }
            else if (szType == "DINT")
            {
                nType = DataType.Int32;
                nVarSize = 4;
                exit = true;
            }
            else if (szType == "REAL")
            {
                nType = DataType.Float;
                nVarSize = 4;
                exit = true;
            }
            else if (szType == "CHAR")
            {
                nType = DataType.SByte;
                nVarSize = 1;
                exit = true;
            }
            else if (szType == "STRING")
            {
                nType = DataType.String;
                exit = true;
            }

            return (exit);
        }

        #region S7TimportParser 


        void SetProgramVisibility(Visibility vis)
        {
            btnLoadProgram.Visibility = vis;
            TxtProgram.Visibility = vis;
            CmbProgram.Visibility = vis;
        }

        private void DisplayImportFileTIAPortal(string file)
        {
            if (importDataModel != null)
                importDataModel.Dispose();
            importDataModel = new ImportDataModelS7Tcp(readStationName, readAddDBnumber);

            if (pareserTia != null)
            {
                pareserTia.Dispose();
                pareserTia = null;
            }
            pareserTia = new S7TIAImportParser(S7TIAImportParser.ImportSourceManagement.Project, file , S7TIAImportParser.ProtocolType.S7Tcp);
            btnLoadProgram.IsEnabled = false; CmbProgram.IsEnabled = false;
            SetProgramVisibility(Visibility.Hidden);

            Dictionary<int, string> mapProgramPLC = pareserTia.GetListOfProgramPLC();
            // file not imported
            if (mapProgramPLC == null || mapProgramPLC.Count == 0)
            {
                //baseImportTree.SetButtonEnable(eImportButtons.eiBtnUpdateSymbol, false);
                return;
            }

            if (mapProgramPLC.Count > 1)
            {
                //clear imported elements
                m_MapImportVariable.Clear();

                CmbProgram.ItemsSource = mapProgramPLC;
                btnLoadProgram.IsEnabled = true; CmbProgram.IsEnabled = true;
                SetProgramVisibility(Visibility.Visible);
                AddDBnumber.IsChecked = true;
                AddDBnumber.Visibility = System.Windows.Visibility.Hidden;
                CmbProgram.SelectedIndex = 0;                
                return;
            }
            else
            {
                using (new WaitCursor())
                {
                    pareserTia.ParsingSelectProgram(0);
                    //ret = LoadTags();
                    LoadTags();

                    pareserTia.Dispose();
                    pareserTia = null;
                    //importSourceS7ImportParser = S7TIAImportParser.ImportSourceManagement.Project;
                }
            }            
        }

        Dictionary<long, ImportVariable> S7ImportParsertToS7ImportAdapter(Dictionary<long, S7TIAImportParser.ImportVariable> mapImportVariable)
        {
            if (mapImportVariable == null)
                return null;

            Dictionary<long, ImportVariable> result = new Dictionary<long, ImportVariable>();

            foreach (var element in mapImportVariable) {

                ImportVariable dest = new ImportVariable()
                {
                    szName = element.Value.szName,
                    szDescription = element.Value.szDescription,
                    nType = element.Value.nType,
                    szType = element.Value.szType,
                    szAddress = element.Value.szAddress,
                    nSize = element.Value.nSize,
                    ArrayDimension = element.Value.ArrayDimension,
                    lID = element.Value.lID,
                    lParent = element.Value.lParent,
                    nElemType = element.Value.nElemType,
                    szPreName = element.Value.szPreName,
                    valMinAbsOp = element.Value.valMinAbsOp,
                    valMaxAbsOp = element.Value.valMaxAbsOp,
                    ImportType = (ImportTypes)element.Value.ImportType,
                    //public UInt64 structKeyCode;
                    //public bool structFull;
                    //public ImportTypes ImportType;
                    //public Dictionary<uint, long> m_MapArrayVariable;
                };
                result[element.Key] = dest;
            }
            
            return result;
        }

        void LoadTags()
        {
            //ImportDataModel ret = null;
            m_MapImportVariable = S7ImportParsertToS7ImportAdapter(pareserTia.GetMapImportVariable());

            m_mapUDTBaseS7P = new Dictionary<string, List<string>>(); // pareserTia.GetMapUDTBaseS7P();
            baseImportTree.SetButtonEnable(eImportButtons.eiBtnLoadFromFile, true);
            btnLoadProgram.IsEnabled = false; CmbProgram.IsEnabled = false;
            SetProgramVisibility(Visibility.Hidden);
            m_mapUDTBaseS7P = new Dictionary<string, List<string>>();
            m_MapValueDepthStructToNumStruct = new Dictionary<string, string>();
            m_mapUDT = new Dictionary<string, List<string>>();
            createMapUDTBaseTiaPortal();
            createDataModel();
            //baseImportTree.SetButtonEnable(eImportButtons.eiBtnUpdateSymbol, true);
            //remove file with block list --> driver rebuilt automatically on first connect
            //DeleteFileListaDataBlocksAndTables(CmbStation.Text);
            //return ret;
        }

        //private ImportDataModel createDataModelS7ImportParser()
        //{
        //    try
        //    {
        //        if (importDataModel != null)
        //            importDataModel.Dispose();
        //        importDataModel = new ImportDataModelS7Tcp(readStationName, readAddDBnumber);
        //        Dictionary<long, ImportData> importedVariable = new Dictionary<long, ImportData>();

        //        foreach (var item in m_MapImportVariable)
        //        {
        //            if (!importedVariable.ContainsKey(item.Key))
        //            {
        //                try
        //                {
        //                    createDataModelElement(importedVariable, item.Key);
        //                }
        //                catch (Exception e)
        //                {
        //                }
        //            }
        //        }
        //        return importDataModel;
        //    }
        //    catch (Exception e)
        //    {
        //        return null;
        //    }
        //}
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
                    baseImportTree.LoadImportFile -= LoadFile;

                    baseImportTree.Dispose();
                    baseImportTree = null;
                }

                if (pareserTia != null)
                {
                    pareserTia.Dispose();
                    pareserTia = null;
                }
            }
        }
        #endregion
    }

    public class ImportDataS7Tcp : ImportData, IDisposable
    {
        public ImportDataS7Tcp(ImportDataModel inDataModel)
            : base(inDataModel)
        {
            dataModelS7Tcp = inDataModel as ImportDataModelS7Tcp;
        }
        private ImportDataModelS7Tcp dataModelS7Tcp { get; set; }

        private string _PreName;
        public string PreName
        {
            get { return _PreName; }
            set { _PreName = value; }
        }

        private DataType _Type;
        public DataType Type
        {
            get { return _Type; }
            set { _Type = value; }
        }

        private DataType _ElemType;
        public DataType ElemType
        {
            get { return _ElemType; }
            set { _ElemType = value; }
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
                string outName = _Name;
                if (Parent == null)
                {
                    if (dataModelS7Tcp.getAddDBnumber() && (!string.IsNullOrEmpty(PreName)) && PreName != null)
                        outName = PreName + "_" + outName;
                    string StationName = dataModelS7Tcp.getStationName();
                    if (StationName != "_")
                        outName = StationName + outName;
                }

                return outName; 
            }
            set { _Name = value; }
        }

        //private DataType _Type;
        //public DataType Type
        //{
        //    get { return _Type; }
        //    set { _Type = value; }
        //}

        //private int _TagType;
        //public int TagType
        //{
        //    get { return _TagType; }
        //    set { _TagType = value; }
        //}

        //private uint _TreeLevel;
        //public uint TreeLevel
        //{
        //    get { return _TreeLevel; }
        //    set { _TreeLevel = value; }
        //}

        //public string TreeName
        //{
        //    get { return getTreeName(); }
        //}

        //private string getTreeName()
        //{
        //    string treeName = Name;
        //    if(Parent != null)
        //    {
        //        treeName = Parent.getTreeName() + "." + treeName;
        //    }
        //    return treeName;
        //}

        public override DataType IconTagType
        {
            get { return _Type; }
        }

        #region IDisposable Members
        public void Dispose()
        {
            if (Children == null)
            {
                return;
            }

            if (Children.Count() > 0)
            {
                foreach (ImportData el in Children)
                {
                    var sls7 = el as ImportDataS7Tcp;
                    if (sls7 != null)
                    {
                        sls7.Dispose();
                    }
                }
                Children.Clear();
            }
        }
        #endregion
    }

    public class ImportDataModelS7Tcp : ImportDataModel, IDisposable
    {
        public GetAddDBnumber getAddDBnumber { get; private set; }

        public ImportDataModelS7Tcp(GetStationName inGetStationName, GetAddDBnumber inGetAddDBnumber) :
            base(inGetStationName)
        {
            getAddDBnumber = inGetAddDBnumber;
        }

        public override ImportData addImportData()
        {
            ImportDataS7Tcp importData = new ImportDataS7Tcp(this);
            return importData;
        }
        #region IDisposable Members

        public void Dispose()
        {
            foreach (ImportData el in Children)
            {
                ImportDataS7Tcp els7 = el as ImportDataS7Tcp;
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
