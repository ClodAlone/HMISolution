using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Utilities;
using Utilities.WPF;
using System.Reflection;
using UFUAModel;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Accon.AGLink;
using S7ImportParser;
using DriverCodeBaseEx.UI;
using DriverCodeBaseEx.UI.Controls;

namespace Simotion.UI
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
        public AGL4.SystemType S7DataFormat;
        public uint StringLength;
    };

    public partial class ImportTagsEditorTree : UserControl, IDisposable
    {
        ImportDataModelSimotion importDataModel;
        string conn;
        bool alreadyLoaded = false;        
        Dictionary<string, List<string>> m_mapUDT;
        public GetStationName readStationName;
        public GetAddDBnumber readAddDBnumber;
        BaseImportTree baseImportTree;
        Dictionary<string, ImportVariable> m_MapImportVariableStruct;
        Dictionary<string, List<String>> m_mapUDTBaseS7P;
        Dictionary<long, SimotionImportParser.ImportVariable> m_MapImportVariable;
        Dictionary<string, string> m_MapValueDepthStructToNumStruct;
        public SimotionImportParser pareserTia;        
        SimotionImportParser.ImportSourceManagement importSource = SimotionImportParser.ImportSourceManagement.None;

        public ImportTagsEditorTree()
        {
            InitializeComponent();
            Loaded += (o, e) =>
            {
                btnLoadProgram.IsEnabled = false;
                CmbProgram.IsEnabled = false;
                SetProgramVisibility(Visibility.Hidden);
                AddDBnumber.Visibility = Visibility.Hidden;
                
                CmbProgram.Text = "";

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

                columns.Add(new BaseImportTree.GridColData()
                {
                    colName = "Description",
                    bindingName = "TagDescription",
                    colWidth = 300
                });

                baseImportTree = new BaseImportTree(DriverName, lista[0],
                    (lista[1].ToLower().IndexOf("true") != -1), columns);
                baseImportTree.SetVisibleButtons(bGetPLCTags:true, bUpdateSymbol:true);
                baseImportTree.SetButtonEnable(eImportButtons.eiBtnUpdateSymbol,false);
                baseImportTree.SetFileFilter("TIA Files(*.ap11 *.ap12 *.ap13 *.ap14 *.ap15 *.ap15_1 *.ap16 *.ap17)|*.ap11;*.ap12;*.ap13;*.ap14;*.ap15;*.ap15_1;*.ap16;*.ap17");
                baseImportTree.LoadImportFile = LoadFile;
                baseImportTree.GetPlcTags = Read_Plc_Info;
                baseImportTree.ImpUpdate += UpdateSymbolFile;
                baseImportTree.StationChanged += BaseImportTree_StationChanged;

                MainStack.Children.Add(baseImportTree);

                if (alreadyLoaded)
                {
                    importDataModel = null;
                    AddDBnumber.IsChecked = true;
                    // refresh interface from selected station
                    BaseImportTree_StationChanged(null, new RoutedEventArgs());
                    return;
                }

                alreadyLoaded = true;                                                

                m_MapImportVariableStruct = new Dictionary<string, ImportVariable>() ;
                m_mapUDTBaseS7P = new Dictionary<string, List<string>>();
                m_MapImportVariable = new Dictionary<long, SimotionImportParser.ImportVariable>();
                m_mapUDT = new Dictionary<string, List<string>>();

                conn = lista[0];// DataContext as string;
                //protect = (lista[1].ToLower().IndexOf("true") != -1);

                AddDBnumber.IsChecked = true;
                readAddDBnumber = () => AddDBnumber.IsChecked == true;

                readStationName = baseImportTree.funcGetStationName();                

                DependencyPropertyDescriptor descriptor =
                   DependencyPropertyDescriptor.FromProperty(CheckBox.IsCheckedProperty, typeof(CheckBox));
                descriptor.AddValueChanged(AddDBnumber, AddDBnumber_Click);

                // refresh import from File/PLC button's state
                BaseImportTree_StationChanged(null, new RoutedEventArgs());

                DataContext = this;
            };
        }

        private void AddDBnumber_Click(object sender, EventArgs e)
        {
            baseImportTree.UpdateDataWithInterfaceParameters();
        }

        private void BaseImportTree_StationChanged(object sender, RoutedEventArgs e)
        {                        
            string stationName = baseImportTree.ReadStationName();
            if (baseImportTree.HasStation(stationName))
            {
                SimotionStation station = new SimotionStation(new SimotionDriver(conn), (SimotionStationSettings)baseImportTree.GetStationSettings(stationName));
                baseImportTree.SetButtonEnable(eImportButtons.eiBtnLoadFromDevice,
                    (station.ImportSource == SimotionImportParser.ImportSourceManagement.ProjectAndPlc));
            } 
        }

        void SetProgramVisibility(Visibility vis)
        {
            btnLoadProgram.Visibility =
                TxtProgram.Visibility = CmbProgram.Visibility = vis;
        }

        public void ImportSelectedTags()
        {
            string importfolder = string.Empty;
            List<ImportData> list = new List<ImportData>();
            importfolder = baseImportTree.ReadFolderName();
            list = baseImportTree.GetSelectedTags();
            string stationName = string.Empty;
            stationName = baseImportTree.ReadStationName();

            if (list.Count > 0)
            {
                if (!UpdateSymbolAndTableListFiles(stationName))
                    return;
                
                List<ImportTag> taglist = new List<ImportTag>();
                Dictionary<string, ImportPrototype> protoMap = new Dictionary<string, ImportPrototype>();
                ObservableCollection<object> notToBeImported = new ObservableCollection<object>();
                foreach (var elem in list)
                {
                    if (elem != null && (notToBeImported.IndexOf(elem) < 0))
                    {
                        bool isStructure = elem.Children.Count > 0;
                        ImportDataSimotion single = elem as ImportDataSimotion;

                        bool isArray = false;
                        if (single.ArrayDimension > 0)
                        {
                            isStructure = false;
                            isArray = true;
                        }

                        //Individual members of a DTL structure can only be read.
                        if (single != null)
                        {
                            bool elementOfDTL = false;
                            switch(single.Name)
                            {
                                case "YEAR":
                                case "MONTH":
                                case "DAY":
                                case "WEEKDAY":
                                case "HOUR":
                                case "MINUTE":
                                case "SECOND":
                                case "NANOSECOND":
                                    int nPosDTL = single.Address.IndexOf("." + single.Name);
                                    string szAddress = single.Address.Substring(0, nPosDTL);
                                    nPosDTL = szAddress.IndexOf("DTL");
                                    if ((nPosDTL + 3) == szAddress.Length)
                                        elementOfDTL = true;
                                    break;
                                default:
                                    break;
                            }

                            SimotionDynTagSettings sp = new SimotionDynTagSettings();

                            if (!sp.ParseAddress(single.Address))
                                continue;

                            sp.StationName = stationName;
                            sp.ArrayDimension = single.ArrayDimension;
                            sp.S7DataFormat = (S7DataFormats)single.S7DataFormat;
                            sp.StringLength = single.StringLength;
                            //Individual members of a DTL structure can only be read.
                            if (elementOfDTL)
                            {
                                sp.TagLinkType = (int) DriverCodeBaseEx.Enumerators.LinkType.Input;
                            }        
                            

                            single.DynAddress = sp.ToString();

                            ImportPrototype proto = null;
                            if (isStructure)
                            {
                                proto = addPrototype(protoMap, single);
                                if (proto == null)
                                    continue;
                            }

                            string importTagName = single.TreeName.Trim(']');

                            ImportTag tagtoimport = new ImportTag()
                            {
                                Name = importTagName,

                                DataType = single.TagType,
                                DynSettings = single.DynAddress,
                                Folder = importfolder,
                                ModelType = (isStructure ? UFUAModel.ModelType.ObjectType : UFUAModel.ModelType.Variable),
                                Description = single.Description,                                
                                ArrayDimension = single.ArrayDimension
                            };
                            if (isStructure)
                            {
                                //It is important to filter the name of the prototype, because if the string contains special characters, 
                                //it is not loaded in the property in the property variables window
                                tagtoimport.PrototypeModel = UFUAModel.Helpers.NameValidator.EnsureValidName(proto.Name);
                                tagtoimport.ModelType = UFUAModel.ModelType.ObjectType;
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
                }
                DataContext = new ImportObject() { PrototypesToImport = protoMap.Values.ToList(), TagsToImport = taglist, SearchForCompatiblePrototypes = true };
            }
            else
                DataContext = this;
        }

        private static ImportPrototype addPrototype(Dictionary<string, ImportPrototype> protoMap, ImportData elem)
        {
            ImportPrototype proto = null;
            ImportDataSimotion s7elem = elem as ImportDataSimotion;
            if (s7elem == null)
                return null;
            string protoname;
            if (s7elem.szType == "STRUCT")
                protoname = string.Format("prototype_{0}", s7elem.Name.Trim(']'));
            else
                protoname = s7elem.szType;
            if (!protoMap.ContainsKey(protoname))
            {
                if (s7elem.Children.Count != 0)
                {
                    proto = new ImportPrototype();
                    proto.Name = UFUAModel.Helpers.NameValidator.EnsureValidName(protoname);
                    proto.Elements = new List<ImportTag>();

                    foreach (var t in s7elem.Children)
                    {
                        ImportDataSimotion el = t as ImportDataSimotion;
                        if (el != null)
                        {
                            ImportTag a = new ImportTag();

                            if (el.ImportType == SimotionImportParser.ImportTypes.Struct)
                            {
                                ImportPrototype protoElem = addPrototype(protoMap, el);
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
                            a.Name = el.Name;
                            a.DataType = el.TagType;
                            a.Description = el.Description;
                            proto.Elements.Add(a);
                        }
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
        
        private void DeleteListaDataBlocksAndTables(string stationName)
        {            
            //Loading channel and station for connections; set also project's driver folder path
            SimotionStation station = new SimotionStation(new SimotionDriver(conn), (SimotionStationSettings)baseImportTree.GetStationSettings(stationName));
            SimotionChannel channel = new SimotionChannel(new SimotionDriver(conn), (SimotionChannelSettings)baseImportTree.GetChannelSettingsFromStation(stationName));

            // init is required for retrive file path from movicon
            channel.InitOffLine(station);

            channel.DeleteListaDataBlocksAndTables(station.Name);

            station.Dispose();
            channel.Dispose();
        }

        /// <summary>
        /// Create a file with a list of all tags imported from Project on with "pattern" <real plc address>:<project plc address>
        /// </summary>
        /// <param name="stationName"></param>
        private void CreateListTagsPlcAndProject(string stationName)
        {
            //Loading channel and station for connections; set also project's driver folder path
            SimotionStation station = new SimotionStation(new SimotionDriver(conn), (SimotionStationSettings)baseImportTree.GetStationSettings(stationName));
            SimotionChannel channel = new SimotionChannel(new SimotionDriver(conn), (SimotionChannelSettings)baseImportTree.GetChannelSettingsFromStation(stationName));

            channel.InitOffLine(station);          

            List<SimotionImportProjectTag> ProjectTags = new List<SimotionImportProjectTag>();
            foreach (var tag in m_MapImportVariable.Values)
                ProjectTags.Add(new SimotionImportProjectTag(tag.szAddress, tag.szAddressImport));

            channel.SaveImportProjectTags(station.Name, ProjectTags);

            station.Dispose();
            channel.Dispose();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Save File Symbol. </summary>
        ///
        /// <returns>   true if it succeeds, false if it fails. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        private bool SaveFileSymbol(string stationName, SimotionImportParser.ImportSourceManagement importSource)
        {
            bool Result = false;

            try
            {
                // Set the library license code
                AGL4.Activate("00BEF5-E1F5-260260");
            }
            catch
            {
                MessageBox.Show(Properties.Resources.AGLink40NotFound,
                                DriverCodeBaseEx.UI.Properties.Resources.ImportMsgBoxTitle);
                return false;
            }

            //Loading channel and station for connections; set also project's driver folder path
            SimotionStation station = new SimotionStation(new SimotionDriver(conn), (SimotionStationSettings)baseImportTree.GetStationSettings(stationName));
            SimotionChannel channel = new SimotionChannel(new SimotionDriver(conn), (SimotionChannelSettings)baseImportTree.GetChannelSettingsFromStation(stationName));

            // init is required for retrive file path from movicon
            channel.InitOffLine(station);

            //get handle of source use to import data
            IntPtr schemaNodeHandle = pareserTia.GetHandler();
            if (schemaNodeHandle == IntPtr.Zero)
                return Result;

            Result = channel.SaveSymbolicFile(stationName, importSource, schemaNodeHandle);

            station.Dispose();
            channel.Dispose();

            return Result;
        }


        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Save File with list of block </summary>
        ///
        /// <returns>   true if it succeeds, false if it fails. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        private bool SaveListaDataBlocksAndTables(string stationName)
        {
            bool Result = false;
            List<string> listDataBlocksAndTables = new List<string>();

            try
            {
                // Set the library license code
                AGL4.Activate("00BEF5-E1F5-260260");
            }
            catch
            {
                MessageBox.Show(Properties.Resources.AGLink40NotFound,
                                DriverCodeBaseEx.UI.Properties.Resources.ImportMsgBoxTitle);
                return false;
            }

            IntPtr schemaNodeHandle = pareserTia.GetHandler();
            if (schemaNodeHandle == IntPtr.Zero)            
                return Result;

            //Loading channel and station for connections; set also project's driver folder path
            SimotionStation station = new SimotionStation(new SimotionDriver(conn), (SimotionStationSettings)baseImportTree.GetStationSettings(stationName));
            SimotionChannel channel = new SimotionChannel(new SimotionDriver(conn), (SimotionChannelSettings)baseImportTree.GetChannelSettingsFromStation(stationName));

            // init is required for retrive file path from movicon
            channel.InitOffLine(station);

            //Check there is the file, with the list datablocks and Tables
            Result = channel.SaveListDataBlockAndTableBase(stationName, schemaNodeHandle);

            station.Dispose();
            channel.Dispose();

            return Result;
        }

        private ImportDataModel LoadFile(string file)
        {
            btnLoadProgram.IsEnabled = false; CmbProgram.IsEnabled = false;
            SetProgramVisibility(Visibility.Hidden);

            file = file.ToLower();
            if (file.Contains(".ap11") ||
                file.Contains(".ap12") ||
                file.Contains(".ap13") ||
                file.Contains(".ap14") ||
                file.Contains(".ap15") ||
                file.Contains(".ap15_1")||
                file.Contains(".ap16")||
                file.Contains(".ap17"))
            {
                DisplayImportFileTIAPortal(file);
            }
            return importDataModel;
        }

        private void LoadProgram_Click(object sender, RoutedEventArgs e)
        {
            if (CmbProgram.Text.Length == 0)
                MessageBox.Show(Properties.Resources.ImportEnterProgramName,
                                DriverCodeBaseEx.UI.Properties.Resources.ImportMsgBoxTitle);
            
            using (new WaitCursor())
            {
                pareserTia.ParsingSelectProgram(CmbProgram.SelectedIndex);

                LoadTags();
                importSource = SimotionImportParser.ImportSourceManagement.Project;
            }
        }


        /// <summary>
        /// Create a .tia file with symbolic info (for AGLink driver) and text files with list of datablock
        /// </summary>
        /// <param name="stationName"></param>
        /// <returns></returns>
        private bool UpdateSymbolAndTableListFiles(string stationName)
        {
            if (!SaveFileSymbol(stationName, importSource))
                return false;

            switch (importSource)
            {
                case SimotionImportParser.ImportSourceManagement.Plc:
                    if (!SaveListaDataBlocksAndTables(stationName))
                        return false;
                    break;
                case SimotionImportParser.ImportSourceManagement.Project:
                    DeleteListaDataBlocksAndTables(stationName);
                    CreateListTagsPlcAndProject(stationName);
                    break;                
            }

            return true;
        }

        private void UpdateSymbolFile(object sender, RoutedEventArgs e)
        {
            string stationName = baseImportTree.ReadStationName();
            if (!UpdateSymbolAndTableListFiles(stationName))
                return;

            var wnd = this.FindParent<Window>();
            if (wnd != null)
            {
                wnd.Close();
            }
        }

        private void createDataModel()
        {
            try
            {
                if (importDataModel != null)
                    importDataModel.Dispose();
                importDataModel = new ImportDataModelSimotion(readStationName, readAddDBnumber);
                Dictionary<long, ImportData> importedVariable = new Dictionary<long, ImportData>();

                foreach (var item in m_MapImportVariable)
                {
                    if (!importedVariable.ContainsKey(item.Key))
                    {
                        try
                        {
                            createDataModelElement(importedVariable, item.Key);
                        }
                        catch (Exception e)
                        {
                        }
                    }
                }
            }
            catch (Exception e)
            {
                importDataModel = null;
            }
            baseImportTree.SetImportDataModel(importDataModel);
        }

        private void createDataModelElement(Dictionary<long, ImportData> importedVariable, long Key)
        {
            ImportDataSimotion IVar = importDataModel.addImportData() as ImportDataSimotion;
            IVar.Id = (int)m_MapImportVariable[Key].lID;
            IVar.parentId = (int)m_MapImportVariable[Key].lParent;
            IVar.PreName = m_MapImportVariable[Key].szPreName;
            IVar.Name = m_MapImportVariable[Key].szName;
            IVar.TagType = m_MapImportVariable[Key].nType;
            IVar.szType = m_MapImportVariable[Key].szType;
            IVar.Size = m_MapImportVariable[Key].nSize;
            IVar.ElemType = m_MapImportVariable[Key].nElemType;
            IVar.Address = m_MapImportVariable[Key].szAddress;
            IVar.Description = m_MapImportVariable[Key].szDescription;
            IVar.ImportType = m_MapImportVariable[Key].ImportType;
            IVar.ArrayDimension = (uint)m_MapImportVariable[Key].ArrayDimension;
            IVar.S7DataFormat = m_MapImportVariable[Key].S7DataFormat;
            IVar.StringLength = m_MapImportVariable[Key].StringLength;

            if (IVar.parentId < 0)
                AddTreeItem(IVar);
            else
            {
                if (!importedVariable.ContainsKey(IVar.parentId))
                    createDataModelElement(importedVariable, IVar.parentId);
                AddTreeItem(IVar, importedVariable[IVar.parentId]);
            }
            importedVariable[Key] = IVar;
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
            uint memberAddress;
            int nAux;
            string member;
            string szAux;
            ImportVariable IVar;

            Dictionary<string, SimotionImportParser.stPrototype> mapUDTBaseS7 = pareserTia.GetMapUDTBaseS7P();
            foreach (var item in m_mapUDTBaseS7P) {
                SimotionImportParser.stPrototype Proto = mapUDTBaseS7[item.Key];
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

        //////////////////////////////////////////////////////////////////////////
        // TIA Portal Import
        //////////////////////////////////////////////////////////////////////////
        private void DisplayImportFileTIAPortal(string file)
        {
            if (importDataModel != null)
                importDataModel.Dispose();
            importDataModel = new ImportDataModelSimotion(readStationName, readAddDBnumber);

            if (pareserTia != null)
            {
                pareserTia.Dispose();
                pareserTia = null;
            }
            pareserTia = new SimotionImportParser(SimotionImportParser.ImportSourceManagement.Project, file);
            btnLoadProgram.IsEnabled = false; CmbProgram.IsEnabled = false;
            SetProgramVisibility(Visibility.Hidden);

            Dictionary<int, string> mapProgramPLC = pareserTia.GetListOfProgramPLC();
            // file not imported
            if (mapProgramPLC == null || mapProgramPLC.Count == 0)
            {
                baseImportTree.SetButtonEnable(eImportButtons.eiBtnUpdateSymbol, false);
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
                    LoadTags();
                    importSource = SimotionImportParser.ImportSourceManagement.Project;
                }
            }
        }

        void LoadTags()
        {
            //ImportDataModel ret = null;
            m_MapImportVariable = pareserTia.GetMapImportVariable();

            m_mapUDTBaseS7P = new Dictionary<string, List<string>>(); // pareserTia.GetMapUDTBaseS7P();
            baseImportTree.SetButtonEnable(eImportButtons.eiBtnLoadFromFile, true);
            btnLoadProgram.IsEnabled = false; CmbProgram.IsEnabled = false;
            SetProgramVisibility(Visibility.Hidden);
            m_mapUDTBaseS7P = new Dictionary<string, List<string>>();
            m_MapValueDepthStructToNumStruct = new Dictionary<string, string>();
            m_mapUDT = new Dictionary<string, List<string>>();
            createMapUDTBaseTiaPortal();
            createDataModel();
            baseImportTree.SetButtonEnable(eImportButtons.eiBtnUpdateSymbol, true);
            //remove file with block list --> driver rebuilt automatically on first connect
            //DeleteFileListaDataBlocksAndTables(CmbStation.Text);
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
            m_MapValueDepthStructToNumStruct.Clear();
            Clean_mapUDT();
        }

        //**********************************************************************
        internal void AddTreeItem(ImportDataSimotion tag, ImportData parent = null)
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

        private ImportDataModel DisplayDirectImport(string stationName)
        {
            IntPtr rootSchemaNodeHandle = IntPtr.Zero;

            SimotionStation station = new SimotionStation(new SimotionDriver(conn), (SimotionStationSettings)baseImportTree.GetStationSettings(stationName));
            SimotionChannel channel = new SimotionChannel(new SimotionDriver(conn), (SimotionChannelSettings)baseImportTree.GetChannelSettingsFromStation(stationName));            

            try
            {
                // Set the library license code
                AGL4.Activate("00BEF5-E1F5-260260");
            }
            catch
            {
                MessageBox.Show(Properties.Resources.AGLink40NotFound,
                                DriverCodeBaseEx.UI.Properties.Resources.ImportMsgBoxTitle);
                return null;
            }

            if (!channel.InitOffLine(station))
            {
                MessageBox.Show(Properties.Resources.ErrorConnectToDeviceIP + channel.TcpChannelHostName,
                                      DriverCodeBaseEx.UI.Properties.Resources.ImportMsgBoxTitle);
                return null;
            }

            // Disable driver to create/modify ecc .tia and .list files during import
            if (!channel.DeviceOpen(false))
            {
                MessageBox.Show(Properties.Resources.ErrorConnectToDeviceIP + channel.TcpChannelHostName,
                                     DriverCodeBaseEx.UI.Properties.Resources.ImportMsgBoxTitle);

                baseImportTree.SetButtonEnable(eImportButtons.eiBtnUpdateSymbol, false);
                return null;
            }
            
            int nRet = AGL4.AGL40_SUCCESS;
            rootSchemaNodeHandle = IntPtr.Zero;
            using (new WaitCursor())
            { 
                //string titleLabel;
                //titleLabel = Properties.Resources.Import_Device_Variables + channel.TcpChannelHostName;
                try
                { 
                    nRet = AGL4.Symbolic_LoadAGLinkSymbolsFromPLC(channel.plcConnection.connNr, ref rootSchemaNodeHandle);
                    if (nRet != AGL4.AGL40_SUCCESS)
                    {
                        MessageBox.Show(Properties.Resources.ErrorLoadSmbolsFromPLC_ErrorCode + nRet.ToString(),
                                       DriverCodeBaseEx.UI.Properties.Resources.ImportMsgBoxTitle);
                    }
                }
                catch
                {
                    MessageBox.Show(Properties.Resources.ErrorLoadSmbolsFromPLC_ErrorCode ,
                                    DriverCodeBaseEx.UI.Properties.Resources.ImportMsgBoxTitle);
                    nRet = -1;
                }

                channel.DeviceClose();
                channel.DisConnection();

                if (nRet != AGL4.AGL40_SUCCESS || rootSchemaNodeHandle == IntPtr.Zero)
                {
                    return null;
                }
                
                if (pareserTia != null)
                {
                    pareserTia.Dispose();
                    pareserTia = null;
                }
                pareserTia = new SimotionImportParser(SimotionImportParser.ImportSourceManagement.Plc, null);               
                CmbProgram.Text = "";
                pareserTia.ParsingHandlerPointer(rootSchemaNodeHandle);
                LoadTags();
                importSource = SimotionImportParser.ImportSourceManagement.Plc;
                
                return importDataModel;
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
                    baseImportTree.GetPlcTags -= Read_Plc_Info;
                    baseImportTree.LoadImportFile -= LoadFile;
                    baseImportTree.ImpUpdate -= UpdateSymbolFile;
                    baseImportTree.StationChanged -= BaseImportTree_StationChanged;

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

    public class ImportDataSimotion : ImportData, IDisposable
    {

        public ImportDataSimotion(ImportDataModel inDataModel)
            : base(inDataModel)
        {
            dataModelS7TTIA = inDataModel as ImportDataModelSimotion;
        }
        private ImportDataModelSimotion dataModelS7TTIA { get;  set; }

        private string _PreName;
        public string PreName
        {
            get { return _PreName; }
            set { _PreName = value; }
        }

        private DataType _ElemType;
        public DataType ElemType
        {
            get { return _ElemType; }
            set { _ElemType = value; }
        }

        private SimotionImportParser.ImportTypes _ImportType;
        public SimotionImportParser.ImportTypes ImportType
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
                    if (dataModelS7TTIA.getAddDBnumber() && (!string.IsNullOrEmpty(PreName)) )
                        outName = PreName + "_" + outName;
                    string StationName = dataModelS7TTIA.getStationName();
                    if (StationName != "_")
                        outName = StationName + outName;
                }

                return outName; 
            }
            set { _Name = value; }
        }
        
        private AGL4.SystemType _S7DataFormat;
        public AGL4.SystemType S7DataFormat
        {
            get { return _S7DataFormat; }
            set { _S7DataFormat = value; }
        }

        private uint _StringLength;
        public uint StringLength
        {
            get { return _StringLength; }
            set { _StringLength = value; }
        }

        #region IDisposable Members
        public void Dispose()
        {
            if(Children == null)
            {
                return;
            }

            if(Children.Count() > 0)
            {            
                foreach (ImportData el in Children)
                {
                    var sls7 = el as ImportDataSimotion;
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

    public class ImportDataModelSimotion : ImportDataModel, IDisposable
    {
        public GetAddDBnumber getAddDBnumber { get; private set; }

        public ImportDataModelSimotion(GetStationName inGetStationName, GetAddDBnumber inGetAddDBnumber):
            base(inGetStationName)
        {
            getAddDBnumber = inGetAddDBnumber;
        }

        public override ImportData addImportData()
        {
            ImportDataSimotion importData = new ImportDataSimotion(this);
            return importData; 
        }
        #region IDisposable Members

        public void Dispose()
        {            
            foreach (ImportData el in Children)
            {
                ImportDataSimotion els7 = el as ImportDataSimotion;
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
