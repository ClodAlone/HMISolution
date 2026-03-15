using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Reflection;
using Utilities;
using UFUAModel;
using System.Collections.ObjectModel;
using DriverCodeBase.UI;
using DriverCodeBase.UI.Controls;

namespace CoDeSys.UI
{

    /// <summary>
    /// Interaction logic for ImportTagsEditorTree.xaml
    /// </summary>
    public partial class ImportTagsEditorTree : UserControl, IDisposable
    {
        ImportDataModel importDataModel;
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
                    bindingName = "TagName", // TreeName
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

                baseImportTree = new BaseImportTree(DriverName, lista[0], (lista[1].ToLower().IndexOf("true") != -1), columns);
                baseImportTree.SetVisibleButtons(bGetPLCTags: true);
                baseImportTree.SetFileFilter("xml files|*.xml");
                baseImportTree.LoadImportFile = LoadFile;
                baseImportTree.GetPlcTags = GetPlcTags;
                baseImportTree.CreateItemControl = CreateImportDataTreeItemControl;

                MainStack.Children.Add(baseImportTree);

                readStationName = baseImportTree.funcGetStationName();

                DataContext = this;
            };
        }

        private ImportDataTreeItemControl CreateImportDataTreeItemControl(object tag)
        {
            return ((ImportDataTreeItemControl)new ImportDataTreeItemControlCoDeSys(tag));
        }

        private void DisplayImportPlc()
        {
            string stationName = baseImportTree.ReadStationName();

            CoDeSysPlcImportParser CDSImporter = new CoDeSysPlcImportParser();
            CoDeSysStationSettings st = (CoDeSysStationSettings)baseImportTree.GetStationSettings(stationName);
            CoDeSysChannelSettings ch = (CoDeSysChannelSettings)baseImportTree.GetChannelSettingsFromStation(stationName);

            ////import source
            //textTitle.Text = string.Format("{0} {1}", Properties.Resources.Import_Device_Variables, ch.DeviceName);
            //textTitle.Refresh();

            //DisableAllButtons();
            //ImportTree.Model = null;
            //ImportTree.Refresh();
            if ((ch.ConnectionType == CoDeSysChannelSettings.CONNECTION.DIRECT) &&
                (string.IsNullOrWhiteSpace(ch.DeviceAddress)))
            {
                MessageBox.Show(string.Format(Properties.Resources.ErrorConnectionParameter, ch.Name), Properties.Resources.ImportMsgBoxTitle);
                //EnableAllButtonsExceptTags();
                importDataModel = null;
                return;
            }
            else if ((ch.ConnectionType == CoDeSysChannelSettings.CONNECTION.GATEWAY) &&
                    (string.IsNullOrWhiteSpace(ch.DeviceAddress) || string.IsNullOrWhiteSpace(ch.DeviceName)))
            {
                MessageBox.Show(string.Format(Properties.Resources.ErrorConnectionParameter, ch.Name), Properties.Resources.ImportMsgBoxTitle);
                importDataModel = null;
                return ;
            }

            using (new WaitCursor())
            {
                importDataModel = CDSImporter.Import(readStationName, stationName, ch.DeviceName, ch.DeviceAddress, (ulong)ch.ConnectionType, ch.PlcPort , ch.UserPLC, ch.PasswordPLC, ch.PasswordGateWay);
            }

            if (CDSImporter.ImportFailed())
            {
                MessageBox.Show(string.Format(Properties.Resources.ImportErrorFromPlc, CDSImporter.LastError), Properties.Resources.ImportMsgBoxTitle);
                //EnableAllButtonsExceptTags();
                importDataModel = null;
            }            

            CDSImporter.Dispose();
        }

        private ImportDataModel GetPlcTags()
        {
            //Check if application is started to 32 bit
            if (!Environment.Is64BitProcess)
            {
                MessageBox.Show(CoDeSys.Properties.Resources.Error32bitEnvironmentUnsupported,
                                Properties.Resources.ImportMsgBoxTitle);
                return null;
            }
        
            DisplayImportPlc();

            return importDataModel;
        }

        public void ImportSelectedTags()
        {
            List<ImportData> list = new List<ImportData>();
            string importfolder = baseImportTree.ReadFolderName();
            list = baseImportTree.GetSelectedTags();
            string stationName = baseImportTree.ReadStationName();

            if (list.Count > 0)
            {                
                List<ImportTag> taglist = new List<ImportTag>();
                Dictionary<string, ImportPrototype> protoMap = new Dictionary<string, ImportPrototype>();
                ObservableCollection<object> notToBeImported = new ObservableCollection<object>();
                foreach (var elemList in list)
                {
                    ImportData elem = elemList as ImportData;
                    if (elem != null && (notToBeImported.IndexOf(elem) < 0))
                    {
                        bool isStructure = elem.Children.Count() > 0;
                        ImportDataCoDeSys single = elem as ImportDataCoDeSys;
                        bool isArray = false;

                        if (single.ArrayDimension > 0)
                        {
                            isStructure = false;
                            isArray = true;
                        }

                        CoDeSysDynTagSettings sp = new CoDeSysDynTagSettings();
                        sp.Address = single.Address;
                        sp.StationName = stationName;
                        sp.ArrayDimension = single.ArrayDimension;

                        sp.CoDeSysVarType = CoDeSysProtocol.VarType.VAR_TYPE_E_UNKNOWN;
                        //if (isStructure) //CoDeSysProtocol.IsStandardDataType(single.OriginalType))
                        //{
                        //    sp.CoDeSysVarType = CoDeSysProtocol.VarType.VAR_TYPE_STRUCT;
                        //} else { 
                        //    sp.CoDeSysVarType = CoDeSysProtocol.GetVarType(single.ElemType);
                        //    if (CoDeSysProtocol.IsStringType(sp.CoDeSysVarType))
                        //        sp.AddStringLengthToAddress(CoDeSysProtocol.StringConverterCalculateSizeFromItem(sp.CoDeSysVarType, single.Size));
                        //}

                        // Set the job type
                        sp.TagLinkType = (int)single.JobType;

                        single.DynAddress = sp.ToString();

                        ImportPrototype proto = null;
                        string protoname = single.szType;
                        if (isStructure)
                        {
                            proto = addPrototype(protoMap, single);
                            if (proto == null)
                                continue;
                        }
                        //FOGBUGZ 18268
                        //string importTagName = UFUAModel.Helpers.NameValidator.EnsureValidName(single.ImportTagName.Trim(']'));
                        string importTagName = ((ImportDataCoDeSys)single).ImportTagName.Trim(']');
                        ImportTag tagtoimport = new ImportTag()
                        {
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
                DataContext = new ImportObject() { PrototypesToImport = protoMap.Values.ToList(), TagsToImport = taglist , SearchForCompatiblePrototypes = true };
            }
            else
            {
                DataContext = this;
            }
        }

        private static ImportPrototype addPrototype(Dictionary<string, ImportPrototype> protoMap, ImportData elem)
        {
            ImportPrototype proto = null;
            if (!protoMap.ContainsKey(((ImportDataCoDeSys)elem).ElemType))
            {
                if (elem.Children.Count != 0)
                {
                    proto = new ImportPrototype();
                    proto.Name = ((ImportDataCoDeSys)elem).ElemType;
                    proto.Elements = new List<ImportTag>();

                    foreach (var t in elem.Children)
                    {
                        ImportData el = t as ImportData;
                        if (el != null)
                        {
                            ImportTag a = new ImportTag();

                            //if (el.szType == CoDeSysProtocol.GENERIC_STRUCT)
                            if (((ImportDataCoDeSys)el).ImportDataType == CoDeSysImportBase.ImportTypes.StructOrEnum)
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
                            a.DataType = ((ImportDataCoDeSys)el).Type;
                            a.Description = el.Description;
                            proto.Elements.Add(a);
                        }
                    }
                    protoMap.Add(((ImportDataCoDeSys)elem).ElemType, proto);
                }
            }
            else
                proto = protoMap[((ImportDataCoDeSys)elem).ElemType];
            return proto;
        }

        
        // TODO
        private ImportDataModel LoadFile(string file)
        {                        
            file = file.ToLower();
            if (file.Contains(".xml"))
            {               
                DisplayImportSymbolFile(file);
            }

            return importDataModel;
        }

        private void DisplayImportSymbolFile(string file)
        {            
            // Import from file class
            CoDeSysSFImportParser CDSImporter = new CoDeSysSFImportParser();

            CDSImporter.SymbolFile = file;
            if (!CDSImporter.IsSymbolFile())
            {
                MessageBox.Show(Properties.Resources.ImportFileFormatInvalid, Properties.Resources.ImportMsgBoxTitle);
                importDataModel = null;
                return;
            }

            // show file and Symboli file  version into title bar
            //textTitle.Text = string.Format(Properties.Resources.Import_Device_Variables_And_Version, file, CDSImporter.SymbolFileVersion);
            //textTitle.Refresh();

            //DisableAllButtons();
            //ImportTree.Model = null;
            //ImportTree.Refresh();

            using (new WaitCursor())
            {
                importDataModel = CDSImporter.Import(readStationName, baseImportTree.ReadStationName());
            }

            if (CDSImporter.ImportFailed())
            {
                MessageBox.Show(string.Format(Properties.Resources.ImportErrorFileParsingFailed, CDSImporter.LastError), Properties.Resources.ImportMsgBoxTitle);
                //EnableAllButtonsExceptTags();
                importDataModel = null;
            }
            //else
            //{
            //    ImportTree.Model = importDataModel;
            //    if (listViewSortCol != null)
            //    {
            //        AdornerLayer.GetAdornerLayer(listViewSortCol).Remove(listViewSortAdorner);
            //        ImportTree.Items.SortDescriptions.Clear();
            //    }
            //    EnableAllButtons();
            //}
            CDSImporter.Dispose();
        }
       
        #region IDisposable Members

        public void Dispose()
        {
            using (new WaitCursor())
            {
                if (importDataModel != null)
                {
                    //importDataModel.Dispose();
                    importDataModel = null;
                }

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

    public class ImportDataCoDeSys : ImportData, IDisposable
    {
        public ImportDataCoDeSys(ImportDataModel inDataModel)
            : base(inDataModel)
        {
            dataModelCoDeSys = inDataModel as ImportDataModelCoDeSys;
        }
        private ImportDataModelCoDeSys dataModelCoDeSys { get; set; }

        private string _strId;
        public string strId
        {
            get { return _strId; }
            set { _strId = value; }
        }

        private string _PreName;
        public string PreName
        {
            get { return _PreName; }
            set { _PreName = value; }
        }
        
        private string _Name;
        public override string Name
        {
            get { return _Name;  }
            set { _Name = value; }
        }        
        
        //private int _TagType;
        //public int TagType
        //{
        //    get { return _TagType; }
        //    set { _TagType = value; }
        //}
        
        private string _ElemType;
        public string ElemType
        {
            get { return _ElemType; }
            set { _ElemType = value; }
        }        
        
        private CoDeSysImportBase.ImportTypes _ImportDataType;
        public CoDeSysImportBase.ImportTypes ImportDataType
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

        private string _Task;
        public string Task
        {
            get { return _Task; }
            set { _Task = value; }
        }

        private DriverCodeBase.Enumerators.LinkType _JobType;
        public DriverCodeBase.Enumerators.LinkType JobType
        {
            get { return _JobType; }
            set { _JobType = value; }
        }

        private DataType _Type;
        public DataType Type
        {
            get { return _Type; }
            set { _Type = value; }
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
                return dataModelCoDeSys.getStationName() != "_" ?
                       dataModelCoDeSys.getStationName() + _PreName + _Name : _PreName + _Name;
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
                ImportDataCoDeSys els7 = el as ImportDataCoDeSys;
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
    public class ImportDataModelCoDeSys : ImportDataModel, IDisposable
    {
        public ImportDataModelCoDeSys(GetStationName inGetStationName) :
            base(inGetStationName)
        {
        }
        public override ImportData addImportData()
        {
            ImportDataCoDeSys importData = new ImportDataCoDeSys(this);
            return importData;
        }

        #region IDisposable Members
        public void Dispose()
        {
            foreach (ImportData el in Children)
            {
                ImportDataCoDeSys els7 = el as ImportDataCoDeSys;
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
