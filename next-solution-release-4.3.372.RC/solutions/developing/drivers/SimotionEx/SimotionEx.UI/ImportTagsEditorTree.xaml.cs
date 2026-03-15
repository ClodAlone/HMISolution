using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Utilities;
using System.Reflection;
using UFUAModel;
using System.Collections.ObjectModel;
using Accon.AGLink;
using SimotionImportParser;
using DriverCodeBaseEx;
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
        bool protect;
        Guid protectionCode;
        bool alreadyLoaded = false;        
        Dictionary<string, List<string>> m_mapUDT;
        public GetStationName readStationName;
        BaseImportTree baseImportTree;
        Dictionary<string, ImportVariable> m_MapImportVariableStruct;
        Dictionary<string, List<String>> m_mapUDTBaseS7P;
        Dictionary<long, SimImportParser.ImportVariable> m_MapImportVariable;
        Dictionary<string, string> m_MapValueDepthStructToNumStruct;
        public SimImportParser pareserTia;
        SimImportParser.ImportSourceManagement importSource = SimImportParser.ImportSourceManagement.None;
        string variableImportFile = string.Empty;

        public ImportTagsEditorTree()
        {
            InitializeComponent();
            Loaded += (o, e) =>
            {
                btnLoadProgram.IsEnabled = false;
                CmbProgram.IsEnabled = false;
                SetProgramVisibility(Visibility.Hidden);
                
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
                    bindingName = "szTypeView",
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
                baseImportTree.SetVisibleButtons();
                baseImportTree.SetFileFilter(Properties.Resources.SYMBOLIC_FILE_FILTER);
                baseImportTree.LoadImportFile = LoadFile;
                baseImportTree.CreateItemControl = CreateImportDataTreeItemControl;

                MainStack.Children.Add(baseImportTree);

                if (alreadyLoaded)
                {
                    importDataModel = null;
                    return;
                }

                alreadyLoaded = true;

                m_MapImportVariableStruct = new Dictionary<string, ImportVariable>();
                m_mapUDTBaseS7P = new Dictionary<string, List<string>>();
                m_MapImportVariable = new Dictionary<long, SimImportParser.ImportVariable>();
                m_mapUDT = new Dictionary<string, List<string>>();

                conn = lista[0];// DataContext as string;
                protect = (lista[1].ToLower().IndexOf("true") != -1);
                protectionCode = new Guid(lista[2]);

                readStationName = baseImportTree.funcGetStationName();

                DataContext = this;
            };
        }

        private ImportDataTreeItemControl CreateImportDataTreeItemControl(object tag)
        {
            return ((ImportDataTreeItemControl)new ImportDataTreeItemControlSimotion(tag));
        }

        void SetProgramVisibility(Visibility vis)
        {
            btnLoadProgram.Visibility =
                TxtProgram.Visibility = CmbProgram.Visibility = vis;
        }

        public void ImportSelectedTags()
        {            
            string importfolder = baseImportTree.ReadFolderName();
            List<ImportData> list = baseImportTree.GetSelectedTags();
            string stationName = baseImportTree.ReadStationName();
            int behaviorExistingTags = baseImportTree.GetBehaviorForExistingTags();
            int behaviorDynamicLink = baseImportTree.GetBehaviorForDynamicLink();

            if (list.Count > 0)
            {
                if (!UpdateSymbolFile(stationName))
                    return;

                using (ImportDataProgressBar progressBar = new ImportDataProgressBar())
                {
                    // Set the number of tags that will be imported
                    progressBar.StartImport(list.Count);
                    try
                    {
                        List<ImportTag> taglist = new List<ImportTag>();
                        Dictionary<string, ImportPrototype> protoMap = new Dictionary<string, ImportPrototype>();
                        ObservableCollection<object> notToBeImported = new ObservableCollection<object>();
                        foreach (var elem in list)
                        {
                            progressBar.ImportNewTag(elem.Name);
                            progressBar.ThrowIfCancellationRequested();

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
                                    switch (single.Name)
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

                                    //Individual members of a DTL structure can only be read.
                                    if (elementOfDTL)
                                    {
                                        sp.TagLinkType = (int)DriverCodeBaseEx.Enumerators.LinkType.Input;
                                    }


                                    single.DynAddress = sp.ToString();

                                    ImportPrototype proto = null;
                                    if (isStructure)
                                    {
                                        proto = addPrototype(protoMap, single, progressBar);
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
                                        ArrayDimension = single.ArrayDimension,
                                        BehaviorForExistingTags = (BehaviorExistingTagsValues)behaviorExistingTags,
                                        BehaviorForDynamicLink = (BehaviorDynamicLinkValues)behaviorDynamicLink,
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
                    catch (OperationCanceledException ex)
                    {
                        //UFUAServerDocument.logGeneral.Info(Properties.Resources.TagsImportCanceled);
                    }
                }
            }
        }

        private static ImportPrototype addPrototype(Dictionary<string, ImportPrototype> protoMap, ImportData elem, ImportDataProgressBar progressBar)
        {
            progressBar.ThrowIfCancellationRequested();
            progressBar.UpdateImportingTag(elem.Name);

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

                            if (el.ImportType == SimImportParser.ImportTypes.Struct)
                            {
                                ImportPrototype protoElem = addPrototype(protoMap, el, progressBar);
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

        private ImportDataModel LoadFile(string file)
        {
            btnLoadProgram.IsEnabled = false;
            CmbProgram.IsEnabled = false;
            SetProgramVisibility(Visibility.Hidden);

            file = file.ToLower();
            if (file.Contains(".sti"))
            {
                variableImportFile = file;
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
                importSource = SimImportParser.ImportSourceManagement.Project;
            }
        }


        /// <summary>
        /// Create a .tia file with symbolic info (for AGLink driver) and text files with list of datablock
        /// </summary>
        /// <param name="stationName"></param>
        /// <returns></returns>
        private bool UpdateSymbolFile(string stationName)
        {
            bool result = false;

            try
            {
                // Set the library license code
                AGL4.Activate("00BEF5-E1F5-260260");
            }
            catch
            {
                MessageBox.Show(Properties.Resources.AGLink40NotFound,
                                DriverCodeBaseEx.UI.Properties.Resources.ImportMsgBoxTitle);
                return result;
            }

            //Loading channel and station for connections; set also project's driver folder path
            SimotionStationSettings stationSetting = (SimotionStationSettings)baseImportTree.GetStationSettings(stationName);
            
            string stFile = SimotionUISymbolicFileManagement.GetSymbolicFileName(conn, stationSetting, SimotionUISymbolicFileManagement.FilePath.FileWithFullPath);

            if (stFile.ToLower() != variableImportFile.ToLower())
            {
                bool proceed = false;
                if (string.IsNullOrEmpty(stFile))
                    proceed = true;
                else
                    if (MessageBox.Show(string.Format(Properties.Resources.ImportRequestToUpdateSymbolicFileName, variableImportFile, stFile), Properties.Resources.ImportSymbolicFile, MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                        proceed = true;

                if (proceed)
                {
                    using (ConfigurationEditor configurationEditor = new ConfigurationEditor())
                    {
                        configurationEditor.Init<SimotionDriverSettings>(conn, SimotionProtocol.GetDriverName(), protect, protectionCode);
                        {
                            result = true;
                            SimotionImportFile import = new SimotionImportFile(conn, configurationEditor.UfW);
                            string file = import.ImportRequest(variableImportFile, stationSetting);
                            if (!string.IsNullOrEmpty(file))
                                SimotionUISymbolicFileManagement.UpdatedSymbolicFileName(configurationEditor.UfW, stationSetting, file);
                            // save to storage
                            configurationEditor.Save(out string dummy, protect, protectionCode);
                        }
                    }
                }
                //else
                //{
                //    // symbolic file used to import is different from file defined into station --> user refused to import from file then all import failed
                //}
            }
            else
            {   // file used to import is the same defined into the station --> OK
                result = true;
            }

            return result;
        }

        private void createDataModel()
        {
            try
            {
                if (importDataModel != null)
                    importDataModel.Dispose();
                importDataModel = new ImportDataModelSimotion(readStationName);
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
            IVar.szTypeView = IVar.szType;
            if (!string.IsNullOrEmpty(m_MapImportVariable[Key].ArrayDimensionDesc))
                IVar.szTypeView = m_MapImportVariable[Key].ArrayDimensionDesc;                
            IVar.Size = m_MapImportVariable[Key].nSize;
            IVar.ElemType = m_MapImportVariable[Key].nElemType;
            IVar.Address = m_MapImportVariable[Key].szAddress;
            IVar.Description = m_MapImportVariable[Key].szDescription;
            IVar.ImportType = m_MapImportVariable[Key].ImportType;
            IVar.ArrayDimension = (uint)m_MapImportVariable[Key].ArrayDimension;
            IVar.S7DataFormat = m_MapImportVariable[Key].S7DataFormat;
            //IVar.StringLength = m_MapImportVariable[Key].StringLength;

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

            Dictionary<string, SimImportParser.stPrototype> mapUDTBaseS7 = pareserTia.GetMapUDTBaseS7P();
            foreach (var item in m_mapUDTBaseS7P) {
                SimImportParser.stPrototype Proto = mapUDTBaseS7[item.Key];
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
            importDataModel = new ImportDataModelSimotion(readStationName);

            if (pareserTia != null)
            {
                pareserTia.Dispose();
                pareserTia = null;
            }

            pareserTia = new SimImportParser(SimImportParser.ImportSourceManagement.Project, file);
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
                CmbProgram.SelectedIndex = 0;
                return;
            }
            else
            {
                using (new WaitCursor())
                {                    
                    pareserTia.ParsingSelectProgram(0);
                    LoadTags();
                    importSource = SimImportParser.ImportSourceManagement.Project;
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

        private SimImportParser.ImportTypes _ImportType;
        public SimImportParser.ImportTypes ImportType
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

        //private uint _StringLength;
        //public uint StringLength
        //{
        //    get { return _StringLength; }
        //    set { _StringLength = value; }
        //}

        private string _szTypeView;
        public string szTypeView
        {
            get { return _szTypeView; }
            set { _szTypeView = value; }
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
        public ImportDataModelSimotion(GetStationName inGetStationName):
            base(inGetStationName)
        {            
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
