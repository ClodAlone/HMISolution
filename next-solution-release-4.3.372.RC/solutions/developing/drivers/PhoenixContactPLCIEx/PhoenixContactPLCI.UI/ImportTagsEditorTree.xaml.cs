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


namespace PhoenixContactPLCI.UI
{
    /// <summary>
    /// Interaction logic for ImportTagsEditorTree.xaml
    /// </summary>
    public partial class ImportTagsEditorTree : UserControl, IDisposable
    {        
        bool alreadyLoaded = false;
        string conn;
        public GetStationName readStationName;
        BaseImportTree baseImportTree;

        public ImportTagsEditorTree()
        {
            InitializeComponent();
            Loaded += (o, e) =>
            {
                if (alreadyLoaded)
                {
                    return;
                }
                alreadyLoaded = true;

                List<string> lista = DataContext as List<string>;
                if (lista == null || lista.Count < 2)
                    return;

                conn = lista[0];

                Assembly a = Assembly.GetAssembly(this.GetType());
                string DriverName = a.GetName().Name;
                DriverName = DriverName.Replace(".UI", "");

                // Add column to the Import Grid
                List<BaseImportTree.GridColData> columns = new List<BaseImportTree.GridColData>();
                columns.Add(new BaseImportTree.GridColData()
                {
                    colName = "Name",
                    bindingName = "TagName",
                    colWidth = 350
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

                baseImportTree = new BaseImportTree(DriverName, lista[0],(lista[1].ToLower().IndexOf("true") != -1), columns);
                baseImportTree.SetVisibleButtons(bGetPLCTags: true);
                baseImportTree.SetFileFilter("Phoenix Contact PLCI Project File(*.MWT)|*.MWT|Phoenix Contact PLCI Boot image File(*.BIN)|*.BIN");
                baseImportTree.LoadImportFile = LoadFromFile;
                baseImportTree.GetPlcTags = LoadFromDevice;
                baseImportTree.CreateItemControl = CreateImportDataTreeItemControl;

                MainStack.Children.Add(baseImportTree);

                readStationName = baseImportTree.funcGetStationName();

                DataContext = this;
            };
        }

        private ImportDataModel DisplayImportPlc()
        {
            PhoenixContactPLCIImportFromPlc importer = new PhoenixContactPLCIImportFromPlc();
            PhoenixContactPLCIStationSettings st = (PhoenixContactPLCIStationSettings)baseImportTree.GetStationSettings(baseImportTree.ReadStationName());
            PhoenixContactPLCIChannelSettings ch = (PhoenixContactPLCIChannelSettings)baseImportTree.GetChannelSettingsFromStation(baseImportTree.ReadStationName());
            ImportDataModel idm = null;

            ////import source
            //textTitle.Text = string.Format(Properties.Resources.ImportDeviceVariables, ch.ServerAddress);
            //textTitle.Refresh();
            
            using (new WaitCursor())
            {
                idm = importer.Import(readStationName, conn, ch, st);
            }

            if (importer.ImportFailed())
            {
                MessageBox.Show(string.Format(Properties.Resources.ErrorImportFailed, importer.LastError), Properties.Resources.ImportMsgBoxTitle);
                baseImportTree.SetButtonEnable(eImportButtons.eiBtnImport, false);
                idm = null;
            }
            importer.Dispose();

            return idm;
        }

        private ImportDataModel LoadFromDevice()
        {
            //Check if application is started to 32 bit
            if (!PhoenixContactPLCIDriver.IsRunningEnviroment32Bit())
            {
                MessageBox.Show(Properties.Resources.ErrorDriverSupportOnly32bitEnvironment,
                                Properties.Resources.ImportMsgBoxTitle, MessageBoxButton.OK, MessageBoxImage.Warning);
                return null;
            }
            
            return DisplayImportPlc();
        }

        public void ImportSelectedTags()
        {
            string importfolder = baseImportTree.ReadFolderName();
            List<ImportData> list = new List<ImportData>();            
            list = baseImportTree.GetSelectedTags();
            string stationname = baseImportTree.ReadStationName();
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
                                ImportDataPhoenixContactPlci single = (ImportDataPhoenixContactPlci)elem;
                                bool isArray = false;

                                if (single.ArrayDimension > 0)
                                {
                                    isStructure = false;
                                    isArray = true;
                                }

                                PhoenixContactPLCIDynTagSettings sp = new PhoenixContactPLCIDynTagSettings();
                                sp.Address = single.Address;
                                sp.StationName = stationname;
                                sp.ArrayDimension = single.ArrayDimension;

                                sp.DataFormat = PhoenixContactPLCIProtocol.VarType.UNKNOWN;
                                if (isStructure)
                                {
                                    sp.DataFormat = PhoenixContactPLCIProtocol.VarType.STRUCT;
                                }
                                else
                                {
                                    sp.DataFormat = PhoenixContactPLCIProtocol.GetVarType(single.ElemType);
                                    if (sp.DataFormat == PhoenixContactPLCIProtocol.VarType.STRING)
                                    {
                                        //A default value is loaded
                                        sp.StringLength = (uint)Properties.Settings.Default.DEFAULT_STRING_LENGTH;
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
                                string importTagName = single.ImportTagName.Trim(']');
                                ImportTag tagtoimport = new ImportTag()
                                {
                                    Name = importTagName,
                                    DataType = single.Type,
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
                        DataContext = new ImportObject() { PrototypesToImport = protoMap.Values.ToList(), TagsToImport = taglist, SearchForCompatiblePrototypes = true };
                    }
                    catch (OperationCanceledException ex)
                    {
                        //UFUAServerDocument.logGeneral.Info(Properties.Resources.TagsImportCanceled);
                    }
                }
            }
        }

        private static ImportPrototype addPrototype(Dictionary<string, ImportPrototype> protoMap, ImportDataPhoenixContactPlci elem, ImportDataProgressBar progressBar)
        {
            progressBar.ThrowIfCancellationRequested();
            progressBar.UpdateImportingTag(elem.Name);

            ImportPrototype proto = null;
            if (!protoMap.ContainsKey(elem.ElemType))
            {
                if (elem.Children.Count != 0)
                {
                    proto = new ImportPrototype();
                    proto.Name = elem.ElemType;
                    proto.Elements = new List<ImportTag>();

                    foreach (var t in elem.Children)
                    {
                        ImportDataPhoenixContactPlci el = t as ImportDataPhoenixContactPlci;
                        if (el != null)
                        {
                            ImportTag a = new ImportTag();

                            //if (el.szType == PhoenixContactPLCIProtocol.GENERIC_STRUCT)
                            if (el.ImportDataType == PhoenixContactPLCIImportBase.ImportTypes.StructOrEnum)
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
                            a.DataType = el.Type;
                            a.Description = el.Description;
                            proto.Elements.Add(a);
                        }
                    }
                    protoMap.Add(elem.ElemType, proto);
                }
            }
            else
                proto = protoMap[elem.ElemType];
            return proto;
        }

        private ImportDataModel LoadFromFile(string file)
        {            
            //Check if application is started to 32 bit
            if (!PhoenixContactPLCIDriver.IsRunningEnviroment32Bit())
            {
                MessageBox.Show(Properties.Resources.ErrorDriverSupportOnly32bitEnvironment,
                                Properties.Resources.ImportMsgBoxTitle, MessageBoxButton.OK, MessageBoxImage.Warning);
                return null;
            }

            ImportDataModel idm = null;
            file = file.ToLower();
            if (file.Contains(".bin"))
            {
                idm = DisplayImportBinFile(file);
            }
            if (file.Contains(".mwt"))
            {
                idm = DisplayImportMwtFile(file);
            }

            return idm;
        }

        private ImportDataModel DisplayImportMwtFile(string file)
        {
            PhoenixContactPLCIImportFromFile importer = new PhoenixContactPLCIImportFromFile();
            if (!importer.IsMwtProject(file))
            {
                MessageBox.Show(Properties.Resources.ErrorDriverTheProjectDoesNotContainTheFile, Properties.Resources.ImportMsgBoxTitle);
                importer.Dispose();
                return null;
            }
            
            ImportDataModel importDataModel = DisplayImportBinFile(importer.GetBinFileFromMwtProject(file));
            
            importer.Dispose();

            return importDataModel;
        }

        private ImportDataModel DisplayImportBinFile(string file)
        {
            PhoenixContactPLCIImportFromFile importer = new PhoenixContactPLCIImportFromFile();
            if (!importer.IsBootImageFile(file))
            {
                MessageBox.Show(Properties.Resources.ErrorImportFileFormatInvalid, Properties.Resources.ImportMsgBoxTitle);
                importer.Dispose();
                return null;
            }

            //// show file and Symbol file  version into title bar
            //textTitle.Text = string.Format(Properties.Resources.ImportDeviceVariables, file);
            //textTitle.Refresh();

            ImportDataModel idm = null;
            using (new WaitCursor())
            {                
                idm = importer.ImportFromBin(readStationName, file);
            }

            if (importer.ImportFailed())
            {
                MessageBox.Show(string.Format(Properties.Resources.ErrorImportFailed, importer.LastError), Properties.Resources.ImportMsgBoxTitle);
                baseImportTree.SetButtonEnable(eImportButtons.eiBtnImport, false);
                idm = null;
            }            
            importer.Dispose();

            return idm;
        }

        private ImportDataTreeItemControl CreateImportDataTreeItemControl(object tag)
        {
            return ((ImportDataTreeItemControl)new ImportDataTreeItemControlPhoenixContactPLCI(tag));
        }

        #region IDisposable Members
        public void Dispose()
        {
            using (new WaitCursor())
            {
                if (baseImportTree != null)
                {
                    baseImportTree.GetPlcTags -= LoadFromDevice;
                    baseImportTree.LoadImportFile -= LoadFromFile;

                    baseImportTree.Dispose();
                    baseImportTree = null;
                }
            }
        }
        #endregion
    }

    ///// <summary>   Import data. </summary>
    public class ImportDataModelPhoenixContactPlci : ImportDataModel
    {
        //    ////////////////////////////////////////////////////////////////////////////////////////////////////
        //    /// <summary>   Constructor. </summary>
        //    ///
        //    /// <param name="inDataModel" type="ImportDataModel">   The in data model. </param>
        //    ////////////////////////////////////////////////////////////////////////////////////////////////////
        public ImportDataModelPhoenixContactPlci(GetStationName inGetStationName)
            : base(inGetStationName)
        {
        }

        public override ImportData addImportData()
        {
            ImportDataPhoenixContactPlci importData = new ImportDataPhoenixContactPlci(this);
            return importData;
        }

        #region IDisposable Members
        public void Dispose()
        {
            foreach (ImportData el in Children)
            {
                ImportDataPhoenixContactPlci els7 = el as ImportDataPhoenixContactPlci;
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

    ///// <summary>   Import data. </summary>
    public class ImportDataPhoenixContactPlci : ImportData
    {
        //    ////////////////////////////////////////////////////////////////////////////////////////////////////
        //    /// <summary>   Constructor. </summary>
        //    ///
        //    /// <param name="inDataModel" type="ImportDataModel">   The in data model. </param>
        //    ////////////////////////////////////////////////////////////////////////////////////////////////////
        public ImportDataPhoenixContactPlci(ImportDataModel inDataModel)
            : base(inDataModel)
        {
        }

        private string _PreName;
        public string PreName
        {
            get { return _PreName; }
            set { _PreName = value; }
        }

        //private string _strId;
        //public string strId
        //{
        //    get { return _strId; }
        //    set { _strId = value; }
        //}

        private string _Task;
        public string Task
        {
            get { return _Task; }
            set { _Task = value; }
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

        private PhoenixContactPLCIImportBase.ImportTypes _ImportDataType;
        public PhoenixContactPLCIImportBase.ImportTypes ImportDataType
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

        private DriverCodeBaseEx.Enumerators.LinkType _JobType;
        public DriverCodeBaseEx.Enumerators.LinkType JobType
        {
            get { return _JobType; }
            set { _JobType = value; }
        }
        
        public string ImportTagName
        {
            get
            {
                return dataModel.getStationName() != "_" ?
                       dataModel.getStationName() + _PreName + Name : PreName + Name;
            }
        }

        public override DataType IconTagType
        {
            get { return _Type; }
        }

        public string TagTreeName
        {
            get
            {
                return TreeLevel == 0xff ?
                       ImportTagName : Name;
            }
        }

        #region IDisposable Members
        public void Dispose()
        {
            if (Children != null && Children.Count > 0)
            {
                foreach (ImportData el in Children)
                {
                    var sl = el as ImportDataPhoenixContactPlci;
                    if (sl != null)
                    {
                        sl.Dispose();
                    }
                }
                Children.Clear();
            }
        }
        #endregion
    };
}
