using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Reflection;
using Utilities;
using UFUAModel;
using System.Collections.ObjectModel;
using DriverCodeBaseEx.UI;
using DriverCodeBaseEx.UI.Controls;

namespace BrPvi.UI
{
    /// <summary>
    /// Interaction logic for ImportTagsEditorTree.xaml
    /// </summary>
    public partial class ImportTagsEditorTree : UserControl, IDisposable
    {
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
                    //importDataModel = null;                    
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
                    colName = "Task",
                    bindingName = "TagTask",
                    colWidth = 200
                });
                columns.Add(new BaseImportTree.GridColData()
                {
                    colName = "Type",
                    bindingName = "TagType",
                    colWidth = 150
                });                

                baseImportTree = new BaseImportTree(DriverName, lista[0], (lista[1].ToLower().IndexOf("true") != -1), columns);
                baseImportTree.SetVisibleButtons(bGetPLCTags: true);
                baseImportTree.SetFileFilter("apj files| *.apj");                
                baseImportTree.LoadImportFile = LoadFile;
                baseImportTree.GetPlcTags = GetPlcTags;
                baseImportTree.CreateItemControl = CreateImportDataTreeItemControl;

                MainStack.Children.Add(baseImportTree);                
                
                readStationName = baseImportTree.funcGetStationName();

                DataContext = this;
            };
        }  


        private ImportDataModel DisplayImportPlc(string stationName)
        {
            ImportDataModel idm = null;
            BrPviPlcImportParser BrImporter = new BrPviPlcImportParser();

            //import source
            //textTitle.Text = string.Format("{0} {1}", Properties.Resources.Import_Device_Variables, dictionaryOfStationSettings[CmbStation.Text].BrPviDestinationStationIPAddress);
            //textTitle.Refresh();

            using (new WaitCursor())
            {
                BrImporter.DictionaryOfChannelSettings = baseImportTree.GeChannelSettingsList().ToDictionary(item => item.Key, item => (BrPviChannelSettings)item.Value);
                BrImporter.DictionaryOfStationSettings = baseImportTree.GetStationSettingsList().ToDictionary(item => item.Key, item => (BrPviStationSettings)item.Value);
                idm = BrImporter.Import(readStationName, baseImportTree.ReadStationName());
            }

            if (BrImporter.ImportFailed())
            {
                BrPviUtils.MessageBox_Show(this, string.Format(Properties.Resources.ImportErrorFromPlc, BrImporter.LastError), Properties.Resources.ImportMsgBoxTitle);
                idm = null;
            }

            BrImporter.Dispose();

            return idm;
        }

        private ImportDataModel GetPlcTags()
        {
            // Check if the station has been selected
            string stationName = baseImportTree.ReadStationName();
            if (stationName.Length == 0)
            {
                BrPviUtils.MessageBox_Show(this, Properties.Resources.ImportEnterStationName, Properties.Resources.ImportMsgBoxTitle);
                return null;
            }

            return DisplayImportPlc(stationName);
        }

        private ImportDataTreeItemControl CreateImportDataTreeItemControl(object tag)
        {
            return ((ImportDataTreeItemControl)new ImportDataTreeItemControlBrPvi(tag));
        }

        public void ImportSelectedTags()
        {            
            List<ImportData> list = new List<ImportData>();
            string importfolder = baseImportTree.ReadFolderName();
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
                        foreach (var elem in list)
                        {
                            progressBar.ImportNewTag(elem.Name);
                            progressBar.ThrowIfCancellationRequested();

                            if (elem != null && (notToBeImported.IndexOf(elem) < 0))
                            {
                                bool isStructure = elem.Children.Count() > 0;
                                ImportDataBrPvi single = elem as ImportDataBrPvi;

                                bool isArray = false;
                                if (single.ArrayDimension > 0)
                                {
                                    isStructure = false;
                                    isArray = true;
                                }

                                BrPviDynTagSettings sp = new BrPviDynTagSettings();
                                sp.BrPviVariableName = single.Address;
                                sp.BrPviTaskName = ((ImportDataBrPvi)single).Task;
                                sp.StationName = stationName;
                                sp.ArrayDimension = single.ArrayDimension;
                                sp.BrPviArrayLength = 0;
                                sp.BrPviStringType = (uint)BrPviDynTagSettings.EnStringType.String;
                                // Special case String: set string length (DL)
                                if (single.Type == DataType.String)
                                {
                                    if (sp.ArrayDimension == 0)
                                    {
                                        sp.BrPviArrayLength = single.Size;
                                    }
                                    else
                                    {
                                        sp.BrPviArrayLength = single.Size / sp.ArrayDimension;
                                    }
                                    if (single.ElemType.Equals("wstring"))
                                    {
                                        sp.BrPviStringType = (uint)BrPviDynTagSettings.EnStringType.WString;
                                    }
                                }
                                // Set the job type
                                sp.TagLinkType = (int)single.JobType;

                                single.DynAddress = sp.ToString();

                                ImportPrototype proto = null;
                                string protoname = single.szType;
                                if (isStructure)
                                {
                                    proto = addPrototype(protoMap, single, progressBar);
                                    if (proto == null)
                                        continue;
                                }
                                //FOGBUGZ 18268
                                //string importTagName = UFUAModel.Helpers.NameValidator.EnsureValidName(single.ImportTagName.Trim(']'));
                                string importTagName = ((ImportDataBrPvi)single).ImportTagName.Trim(']');
                                ImportTag tagtoimport = new ImportTag()
                                {
                                    Name = importTagName,
                                    DataType = ((ImportDataBrPvi)single).Type,
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
                                    tagtoimport.PrototypeModel = proto.Name;
                                    //FOGBUGZ 18268
                                    //tagtoimport.PrototypeModel = UFUAModel.Helpers.NameValidator.EnsureValidName(tagtoimport.PrototypeModel);
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
                        DataContext = new ImportObject() { PrototypesToImport = protoMap.Values.ToList(), TagsToImport = taglist };
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
            if (!protoMap.ContainsKey(((ImportDataBrPvi)elem).ElemType))
            {
                if (elem.Children.Count != 0)
                {
                    proto = new ImportPrototype();
                    proto.Name = ((ImportDataBrPvi)elem).ElemType;
                    proto.Elements = new List<ImportTag>();

                    foreach (var t in elem.Children)
                    {
                        ImportData el = t as ImportData;
                        if (el != null)
                        {
                            ImportTag a = new ImportTag();

                            //if (el.szType == "STRUCT")
                            if (((ImportDataBrPvi)el).ImportDataType == BrPviImportBase.ImportTypes.StructOrEnum)
                            {
                                ImportPrototype protoElem = addPrototype(protoMap, el, progressBar);
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
                            a.DataType = ((ImportDataBrPvi)el).Type;
                            a.Description = el.Description;
                            proto.Elements.Add(a);
                        }
                    }
                    protoMap.Add(((ImportDataBrPvi)elem).ElemType, proto);
                }
            }
            else
                proto = protoMap[((ImportDataBrPvi)elem).ElemType];
            return proto;
        }

        private bool SearchMatchingPrototype(ImportPrototype candProto, ImportData currItem)
        {
            return false;
            //if (candProto.Elements.Count != currItem.Children.Count())
            //{
            //    return false;
            //}

            //bool matchingPrototype = false;
            //foreach (var m in candProto.Elements)
            //{
            //    bool fieldFound = false;
            //    foreach (ImportData el in currItem.Children)
            //    {
            //        if (el != null)
            //        {
            //            fieldFound = m.Name == el.Name && m.DataType == el.Type;
            //        }
            //        else
            //        {
            //            return false;
            //        }
            //        if (fieldFound == true)
            //        {
            //            break;
            //        }
            //    }
            //    if (fieldFound == false)
            //    {
            //        matchingPrototype = false;
            //        break;
            //    }
            //    else
            //    {
            //        matchingPrototype = true;
            //    }
            //}

            //return matchingPrototype;
        }

        //private void Import_Click(object sender, RoutedEventArgs e)
        //{
        //    if (CmbStation.Text.Length == 0)
        //    {
        //        MessageBox.Show(Properties.Resources.ImportEnterStationName,
        //                        Properties.Resources.ImportMsgBoxTitle);
        //        return;
        //    }

        //    var wnd = this.FindParent<Window>();
        //    if (wnd != null)
        //    {
        //        wnd.DialogResult = true;
        //        wnd.Close();
        //    }
        //}

        private ImportDataModel LoadFile(string file)
        {            
            file = file.ToLower();
            if (file.Contains(".apj"))
            {
                return DisplayImportFileAS(file);
            }

            return null;
        }
       
        private ImportDataModel DisplayImportFileAS(string file)
        {
            ImportDataModel ret = null;

            // Import from file class
            BrPviASImportParser BrImporter = new BrPviASImportParser();

            BrImporter.ProjectFile = file;
            if (!BrImporter.IsASProject())
            {
                BrPviUtils.MessageBox_Show(this, Properties.Resources.ImportFileFormatInvalid, Properties.Resources.ImportMsgBoxTitle);
                return ret;
            }

            // show file and AS version into title bar
            //textTitle.Text = string.Format(Properties.Resources.Import_Device_Variables_And_Version, file, BrImporter.ProjectVersion);
            //textTitle.Refresh();

            using (new WaitCursor())
            {
                ret = BrImporter.Import(readStationName, baseImportTree.ReadStationName());
            }

            if (BrImporter.ImportFailed())
            {
                BrPviUtils.MessageBox_Show(this, string.Format(Properties.Resources.ImportErrorFileParsingFailed, BrImporter.LastError), Properties.Resources.ImportMsgBoxTitle);
                ret = null;
            }           
            BrImporter.Dispose();

            return ret;
        }
       
        #region IDisposable Members

        public void Dispose()
        {
            using(new WaitCursor())
            {
                //if (importDataModel != null)
                //{
                //    importDataModel.Dispose();
                //    importDataModel = null;
                //}

                if (baseImportTree != null)
                {
                    baseImportTree.GetPlcTags -= GetPlcTags;
                    baseImportTree.LoadImportFile -= LoadFile;

                    baseImportTree.Dispose();
                    baseImportTree = null;
                }                
            }
        }

        #endregion
    }

    public class ImportDataBrPvi : ImportData, IDisposable
    {
        public ImportDataBrPvi(ImportDataModel inDataModel)
            : base(inDataModel)
        {
            dataModelBrPvi = inDataModel as ImportDataModelBrPVI;
        }
        private ImportDataModelBrPVI dataModelBrPvi { get; set; }

        //private int _Id;
        //public int Id
        //{
        //    get { return _Id; }
        //    set { _Id = value; }
        //}
        private string _PreName;
        public string PreName
        {
            get { return _PreName; }
            set { _PreName = value; }
        }
        //private string _Name;
        //public string Name
        //{
        //    get { return _Name;  }
        //    set { _Name = value; }
        //}
        //private string _Address;
        //public string Address
        //{
        //    get { return _Address; }
        //    set { _Address = value; }
        //}        
        //private bool _Select;
        //public bool Select
        //{
        //    get { return _Select; }
        //    set { _Select = value; }
        //}
        //private string _DynAddress;
        //public string DynAddress
        //{
        //    get { return _DynAddress; }
        //    set { _DynAddress = value; }
        //}
        //private int _TagType;
        //public int TagType
        //{
        //    get { return _TagType; }
        //    set { _TagType = value; }
        //}
        //private string _szType;
        //public string szType
        //{
        //    get { return _szType; }
        //    set { _szType = value; }
        //}

        //used to show tag data type with Movicon DataType and not PLC Data Type
        private string _szTypeView;
        public string szTypeView
        {
            get { return _szTypeView; }
            set { _szTypeView = value; }
        }
        //private string _Description;
        //public string Description
        //{
        //    get { return _Description; }
        //    set { _Description = value; }
        //}
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
        private BrPviImportBase.ImportTypes _ImportDataType;
        public BrPviImportBase.ImportTypes ImportDataType
        {
            get { return _ImportDataType; }
            set { _ImportDataType = value; }
        }

        private uint _Size;
        public uint Size
        {
            get { return _Size; }
            set { _Size = value; }
        }
        //private int _parentId;
        //public int parentId
        //{
        //    get { return _parentId; }
        //    set { _parentId = value; }
        //}

        //private uint _ArrayDimension;
        //public uint ArrayDimension
        //{
        //    get { return _ArrayDimension; }
        //    set { _ArrayDimension = value; }
        //}

        //private ImportData _Parent;
        //public ImportData Parent
        //{
        //    get { return _Parent; }
        //    set { _Parent = value; }
        //}
        //private uint _TreeLevel;
        //public uint TreeLevel
        //{
        //    get { return _TreeLevel; }
        //    set { _TreeLevel = value; }
        //}

        private string _strLevel;
        public string strLevel
        {
            get { return _strLevel; }
            set { _strLevel = value; }
        }

        private string _strId;
        public string strId
        {
            get { return _strId; }
            set { _strId = value; }
        }

        private string _Task;
        public string Task
        {
            get { return _Task; }
            set { _Task = value; }
        }

        private DriverCodeBaseEx.Enumerators.LinkType _JobType;
        public DriverCodeBaseEx.Enumerators.LinkType JobType
        {
            get { return _JobType; }
            set { _JobType = value; }
        }
        public override string TreeName
        {
            get
            {
                return TreeLevel == 0xff ?
                       ImportTagName : Name;
            }
        }

        public string ImportTagName
        {
            get
            {
                return dataModelBrPvi.getStationName() != "_" ?
                       dataModelBrPvi.getStationName() + _PreName + Name: _PreName + Name;
            }
        }

        public override DataType IconTagType
        {
            get { return _Type; }
        }
        #region IDisposable Members

        public void Dispose()
        {
            foreach (ImportData el in Children)
            {
                ImportDataBrPvi els7 = el as ImportDataBrPvi;
                if (els7 != null)
                {
                    els7.Dispose();
                }
            }
            Children.Clear();
            //Children = null;
        }

        #endregion

    };


    public class ImportDataModelBrPVI : ImportDataModel, IDisposable
    {
        public ImportDataModelBrPVI(GetStationName inGetStationName) :
            base(inGetStationName)
        {
        }

        public override ImportData addImportData()
        {
            ImportDataBrPvi importData = new ImportDataBrPvi(this);
            return importData;
        }
        #region IDisposable Members

        public void Dispose()
        {
            foreach (ImportData el in Children)
            {
                ImportDataBrPvi els7 = el as ImportDataBrPvi;
                if (els7 != null)
                {
                    els7.Dispose();
                }
            }
            Children.Clear();
            Children = null;
        }
    }

    #endregion

        //public class ImportDataModel : ITreeModel, IDisposable
        //{
        //    public ObservableCollection<ImportData> Children { get; private set; }
        //    public GetStationName getStationName { get; private set; }

        //    public ImportDataModel(GetStationName inGetStationName)
        //    {
        //        Children = new ObservableCollection<ImportData>();
        //        getStationName = inGetStationName;
        //    }

        //    public System.Collections.IEnumerable GetChildren(object parent)
        //    {
        //        if (parent == null)
        //            return Children;
        //        return (parent as ImportData).Children;
        //    }

        //    public bool HasChildren(object parent)
        //    {
        //        return (parent as ImportData).Children.Count > 0;
        //    }

        //    public ImportData addImportData()
        //    {
        //        ImportData importData = new ImportData(this);
        //        return importData;
        //    }
        //    #region IDisposable Members

        //    public void Dispose()
        //    {
        //        foreach (ImportData el in Children)
        //        {
        //            if (el != null)
        //            {
        //                el.Dispose();
        //            }
        //        }
        //        Children.Clear();
        //        Children = null;
        //    }

        //    #endregion
        //}
    }
