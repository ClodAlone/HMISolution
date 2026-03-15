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
using System.Xml.Linq;
using System.IO;
using DriverCodeBaseEx.UI;
using DriverCodeBaseEx.UI.Controls;
using DriverCodeBaseEx;
using System.Collections;

namespace MTConnect.UI
{
    /// <summary>
    /// Interaction logic for ImportTagsEditorTree.xaml
    /// </summary>
    public partial class ImportTagsEditorTree : UserControl, IDisposable
    {
        ImportDataModelMTConnect importDataModel;
        bool alreadyLoaded = false;
        string conn;
        public GetStationName readStationName;
        BaseImportTree baseImportTree;

        //member parse steem xml
        string nameDevice = null;
        Dictionary<string, List<List<XAttribute>>> Devices;



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

                Assembly a = Assembly.GetAssembly(this.GetType());
                string DriverName = a.GetName().Name;
                DriverName = DriverName.Replace(".UI", "");

                // Add column to the Import Grid
                List<BaseImportTree.GridColData> columns = new List<BaseImportTree.GridColData>();
                columns.Add(new BaseImportTree.GridColData()
                {
                    colName = "Name",
                    bindingName = "TagName",
                    colWidth = 200
                });
                columns.Add(new BaseImportTree.GridColData()
                {
                    colName = "Path",
                    bindingName = "TagDynPath",
                    colWidth = 400
                });
                columns.Add(new BaseImportTree.GridColData()
                {
                    colName = "Address",
                    bindingName = "TagAddress",
                    colWidth = 100
                });
                columns.Add(new BaseImportTree.GridColData()
                {
                    colName = "Type",
                    bindingName = "TagType",
                    colWidth = 200
                });
                columns.Add(new BaseImportTree.GridColData()
                {
                    colName = "Category",
                    bindingName = "TagDynCategory",
                    colWidth = 200
                });

                baseImportTree = new BaseImportTree(DriverName, lista[0], 
                    (lista[1].ToLower().IndexOf("true") != -1), columns);

                baseImportTree.SetVisibleButtons(bGetPLCTags: true);
                baseImportTree.SetFileFilter("MTConnect Export File(*.XML)|*.XML");
                baseImportTree.LoadImportFile += LoadFile;
                baseImportTree.GetPlcTags += Read_Plc_Info;
                //baseImportTree.ImpUpdate += UpdateSymbolFile;
                baseImportTree.StationChanged += BaseImportTree_StationChanged;
                baseImportTree.CreateItemControl = CreateImportDataTreeItemControl;

                MainStack.Children.Add(baseImportTree);
                Devices = new Dictionary<string, List<List<XAttribute>>>();

                conn = lista[0];// DataContext as string;
                readStationName = baseImportTree.funcGetStationName();

                DataContext = this;
            };
        }
        private ImportDataTreeItemControl CreateImportDataTreeItemControl(object tag)
        {
            return ((ImportDataTreeItemControl)new ImportDataTreeItemControlMTConnect(tag));
        }
        private void BaseImportTree_StationChanged(object sender, RoutedEventArgs e)
        {
            string stationName = baseImportTree.ReadStationName();
            if (baseImportTree.HasStation(stationName))
            {
                MTConnectStation station = new MTConnectStation(new MTConnectDriver(conn), (MTConnectStationSettings)baseImportTree.GetStationSettings(stationName));
                baseImportTree.SetButtonEnable(eImportButtons.eiBtnLoadFromDevice, true);
            }
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
                                ImportDataMTConnect single = (ImportDataMTConnect)elem;
                                bool isArray = false;

                                if (single.ArrayDimension > 0)
                                {
                                    isStructure = false;
                                    isArray = true;
                                }

                                MTConnectDynTagSettings sp = new MTConnectDynTagSettings();
                                sp.TagName = single.Address;
                                sp.StationName = stationname;
                                sp.ArrayDimension = single.ArrayDimension;
                                sp.PathParameter = single.DynAddress;
                                MTConnectProtocol.GetCategory(single.Description);
                                sp.VarType = single.TagType;
                                sp.TagLinkType = (int)DriverCodeBaseEx.Enumerators.LinkType.Input; 


                                ImportPrototype proto = null;
                                string protoname = single.szType;
                                if (isStructure)
                                {
                                    proto = addPrototype(protoMap, single, progressBar);
                                    if (proto == null)
                                        continue;
                                }

                                if (single.Description.ToUpper().Equals("CONDITION"))
                                {
                                    single.TagType = DataType.String;
                                }

                                string importTagName = single.ImportTagName.Trim(']');
                                ImportTag tagtoimport = new ImportTag()
                                {
                                    Name = importTagName,
                                    DataType = single.TagType,
                                    DynSettings = sp.ToString(),
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

        MTConnectProtocol.FormatDataType_E GetFormatMTConnect(DataType dt)
        {
            MTConnectProtocol.FormatDataType_E formType = MTConnectProtocol.FormatDataType_E.String;
           
            switch (dt)
            {
                case DataType.Double:
                    formType = MTConnectProtocol.FormatDataType_E.Double;
                    break;
                case DataType.Int32:
                    formType = MTConnectProtocol.FormatDataType_E.Signed_DWord;
                    break;
                case DataType.UInt32:
                    formType = MTConnectProtocol.FormatDataType_E.DWord;
                    break;
                default:
                    formType = MTConnectProtocol.FormatDataType_E.String;
                    break;
            }
            return formType;
        }

        private static ImportPrototype addPrototype(Dictionary<string, ImportPrototype> protoMap, ImportDataMTConnect elem, ImportDataProgressBar progressBar)
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
                        ImportDataMTConnect el = t as ImportDataMTConnect;
                        if (el != null)
                        {
                            ImportTag a = new ImportTag();
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
        
        private ImportDataModel LoadFile(string file)
        {
            ImportDataModel idm = null;
            file = file.ToLower();
            if (file.Contains(".xml"))
            {
                using (new WaitCursor())
                {
                    XElement currentDoc = XElement.Load(file);//loads your xml
                    idm = LoadVariableToTheGrid(currentDoc);
                }
            }

            return idm;
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

            using (new WaitCursor())
            {
                return DisplayDirectImport(stationName);
            }
        }

        private ImportDataModel DisplayDirectImport(string stationName)
        {

            MTConnectStation station = new MTConnectStation(new MTConnectDriver(conn), (MTConnectStationSettings)baseImportTree.GetStationSettings(stationName));
            MTConnectChannel channel = new MTConnectChannel(new MTConnectDriver(conn), (MTConnectChannelSettings)baseImportTree.GetChannelSettingsFromStation(stationName));

            XElement currentDoc = channel.GetCommandProbe(channel.ServerAddress, channel.ServerPort);
            if (currentDoc == null)
            {
                //MessageBox.Show(Properties.Resources.ErrorConnectToDeviceIP + channel.TcpChannelHostName,
                //                      DriverCodeBaseEx.UI.Properties.Resources.ImportMsgBoxTitle);
                return null;
            }

            //The stream of the machine variable request is saved in an xml file,
            //saving this file could be useful in case the connection was not present,
            //then it could be useful in case of error during the import.
            bool Result = channel.SaveSymbolicFile(stationName, currentDoc, String.Empty);           

            station.Dispose();
            channel.Dispose();

            return LoadVariableToTheGrid(currentDoc);
        }
        private ImportDataModel LoadVariableToTheGrid(XElement currentDoc)
        {
            if(currentDoc == null)
            {
                return (null);
            }
            bool Result = false;
            if (importDataModel != null)
                importDataModel.Dispose();
            importDataModel = new ImportDataModelMTConnect(readStationName);

            //Parse streeming xml
            ParseProbeResponse(currentDoc);
            string stationName = baseImportTree.ReadStationName();
            MTConnectStation station = new MTConnectStation(new MTConnectDriver(conn), (MTConnectStationSettings)baseImportTree.GetStationSettings(stationName));
            string deviceName = station.DeviceId;
            station.Dispose();

            //zero elemets exstract from the parse, so return null
            if (Devices == null ||  Devices.Count == 0 || !Devices.ContainsKey(deviceName))
            {
                return null;
            }

            //extraction all element attribute into a list
            List<List<XAttribute>> Listelements = Devices[deviceName];


            string dynamicaddress;
            int MovType;
            uint varsize;
            foreach (var listAttribute in Listelements)
            {
                MovType = -1;
                dynamicaddress = string.Empty;
                varsize = 0;
                var IVar = importDataModel.addImportData();
                foreach (XAttribute att in listAttribute)
                {
                    if (att.Name.Equals("name"))
                    {
                        IVar.Name = att.Value;
                    }
                    switch (att.Name.LocalName.ToLower())
                    {
                        case "name":
                            IVar.Name = att.Value;
                            break;

                        case "id":
                            IVar.Address = att.Value;
                            break;
                        case "category":
                            IVar.Description = att.Value;
                            break;
                        case "type":
                            IVar.szType = att.Value;
                            IVar.TagType = (DataType)GetMoviconTypeId(att.Value, ref varsize);
                            break;
                        case "path":
                            IVar.DynAddress = att.Value;
                            break;

                    }
                }
                //IVar.DynAddress = String.Format("{0}:{1}",IVar.DynAddress,IVar.Address);
                if(String.IsNullOrWhiteSpace(IVar.Name))
                {
                    IVar.Name = String.Format("{0}", IVar.Address);
                }
                else
                {
                    IVar.Name = String.Format("{0}_{1}", IVar.Name, IVar.Address);
                }

                IVar.Select = false;
                AddTreeItem(IVar);
            }

            Devices.Clear();
            return importDataModel;
        }
        #region Parse stream xml
        private void ParseProbeResponse(XElement currentDoc)
        {
            foreach (var urlElement in currentDoc.Elements())
            {
                //Console.WriteLine(urlElement.Attributes().Count());
                ParseElemet(urlElement, false, String.Empty);
            }
        }

        private void ParseElemet(XElement child, bool bStartMemPath, string path)
        {
            Console.WriteLine(child.Elements().Count());
            if (child.Elements().Count() == 0)
            {

                if (child.Attributes().Count() != 0)
                {
                    LoadAttribite(child, ref bStartMemPath, ref path);
                }
            }
            else
            {
                LoadAttribite(child, ref bStartMemPath, ref path);
                foreach (XElement urlElement in child.Elements())
                {
                    ParseElemet(urlElement, bStartMemPath, path);
                }
            }
        }
        private void LoadAttribite(XElement child, ref bool bStartMemPath, ref string path)
        {
            try
            {
                bool newdevice = false;
                //save 
                string marker = marker = (child as XElement).Name.LocalName;

                //Create the path in base the deeply of tree
                if (bStartMemPath)
                {
                    if(String.IsNullOrWhiteSpace(path))
                        path = String.Format("//{0}", marker);
                    else
                        path = String.Format("{0}/{1}", path, marker);
                }

                //check marker if the name is the "Device"
                if (marker.Equals("Device"))
                {
                    bStartMemPath = true;
                    newdevice = true;
                }
                //extraction all element attribute into a list
                IEnumerable<XAttribute> attList =
                                from at in child.Attributes()
                                select at;
                if (attList.Count() != 0 && bStartMemPath)
                {

                    List<XAttribute> DataItemAttribute = new List<XAttribute>();
                    foreach (XAttribute att in attList)
                    {
                        //Console.Write(String.Format("{0}\t", att));
                        //Added the attributes list at the Device                         
                        if (newdevice && att.Name.LocalName.Equals("name"))
                        {
                            //remember the name of device
                            nameDevice = att.Value.ToString();
                            Devices.Add(att.Value.ToString(), new List<List<XAttribute>>());
                        }
                        else
                        {
                            if (!newdevice && (child as XElement).Name.LocalName.Equals("DataItem"))
                            {
                                DataItemAttribute.Add(att);
                            }
                        }
                    }
                    //Added the path element and added the element list at device
                    if (marker.Equals("DataItem"))
                    {
                        XAttribute elpercorso = new XAttribute("path", path);
                        DataItemAttribute.Add(elpercorso);
                        (Devices[nameDevice]).Add(DataItemAttribute);
                    }
                }
                //Console.WriteLine(String.Format("Marker={0} Patt={1}", marker, path));
            }
            catch (System.Exception ex)
            {

            }
        }
        #endregion Parse stream xml

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

        public int GetMoviconTypeId(string Type, ref uint VarSize)
        {
            int nType = -1;
            VarSize = 0;
            Type.Trim();

            if (Type.Length == 0)
                return (nType);

            Type = Type.ToUpper();

            if (Type.Equals("ACCELERATION"))
            {
                nType = (int)DataType.Double;
                VarSize = 8;
            }
            else if (Type.Equals("AMPERAGE"))
            {
                nType = (int)DataType.Double;
                VarSize = 8;
            }
            else if (Type.Equals("ANGLE"))
            {
                nType = (int)DataType.Double;
                VarSize = 8;
            }
            else if (Type.Equals("ANGULAR_ACCELERATION"))
            {
                nType = (int)DataType.Double;
                VarSize = 8;
            }
            else if (Type.Equals("ANGULAR_VELOCITY"))
            {
                nType = (int)DataType.Double;
                VarSize = 8;
            }
            else if (Type.Equals("AXIS_FEEDRATE"))
            {
                nType = (int)DataType.Double;
                VarSize = 8;
            }
            else if (Type.Equals("DISPLACEMENT"))
            {
                nType = (int)DataType.Double;
                VarSize = 8;
            }
            else if (Type.Equals("FREQUENCY"))
            {
                nType = (int)DataType.Double;
                VarSize = 8;
            }
            else if (Type.Equals("LOAD"))
            {
                nType = (int)DataType.Double;
                VarSize = 8;
            }
            else if (Type.Equals("PATH_FEEDRATE"))
            {
                nType = (int)DataType.Double;
                VarSize = 8;
            }
            else if (Type.Equals("POSITION"))
            {
                nType = (int)DataType.Double;
                VarSize = 8;
            }
            else if (Type.Equals("PRESSURE"))
            {
                nType = (int)DataType.Double;
                VarSize = 8;
            }
            else if (Type.Equals("SPINDLE_SPEED"))
            {
                nType = (int)DataType.Double;
                VarSize = 8;
            }
            else if (Type.Equals("TEMPERATURE"))
            {
                nType = (int)DataType.Double;
                VarSize = 8;
            }
            else if (Type.Equals("TORQUE"))
            {
                nType = (int)DataType.Double;
                VarSize = 8;
            }
            else if (Type.Equals("VELOCITY"))
            {
                nType = (int)DataType.Double;
                VarSize = 8;
            }
            else if (Type.Equals("VOLTAGE"))
            {
                nType = (int)DataType.Double;
                VarSize = 8;
            }
            else if (Type.Equals("WATTAGE"))
            {
                nType = (int)DataType.Double;
                VarSize = 8;
            }
            else if (Type.Equals("LINE"))
            {
                nType = (int)DataType.UInt32;
                VarSize = 4;
            }
            else if (Type.Equals("PART_COUNT"))
            {
                nType = (int)DataType.Int32;
                VarSize = 4;
            }
            else //All known data types and are not, imported into Movicon as strings
            {
                nType = (int)DataType.String;
                VarSize = 20;
            }

            return (nType);

        }


        #region IDisposable Members
        public void Dispose()
        {
            using (new WaitCursor())
            {
                if (baseImportTree != null)
                {
                    baseImportTree.LoadImportFile -= LoadFile;
                    baseImportTree.GetPlcTags -= Read_Plc_Info;

                    baseImportTree.Dispose();
                    baseImportTree = null;
                }
            }
        }
        #endregion

        ///// <summary>   Import data. </summary>
        public class ImportDataModelMTConnect : ImportDataModel
        {
            //    ////////////////////////////////////////////////////////////////////////////////////////////////////
            //    /// <summary>   Constructor. </summary>
            //    ///
            //    /// <param name="inDataModel" type="ImportDataModel">   The in data model. </param>
            //    ////////////////////////////////////////////////////////////////////////////////////////////////////
            public ImportDataModelMTConnect(GetStationName inGetStationName)
                : base(inGetStationName)
            {
            }

            public override ImportData addImportData()
            {
                ImportDataMTConnect importData = new ImportDataMTConnect(this);
                return importData;
            }

            #region IDisposable Members
            public void Dispose()
            {
                foreach (ImportData el in Children)
                {
                    ImportDataMTConnect els7 = el as ImportDataMTConnect;
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
        ///// <summary>   Import data. </summary>
    public class ImportDataMTConnect : ImportData
    {
        //    ////////////////////////////////////////////////////////////////////////////////////////////////////
        //    /// <summary>   Constructor. </summary>
        //    ///
        //    /// <param name="inDataModel" type="ImportDataModel">   The in data model. </param>
        //    ////////////////////////////////////////////////////////////////////////////////////////////////////
        public ImportDataMTConnect(ImportDataModel inDataModel)
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

        //private MTConnectImportBase.ImportTypes _ImportDataType;
        //public MTConnectImportBase.ImportTypes ImportDataType
        //{
        //    get { return _ImportDataType; }
        //    set { _ImportDataType = value; }
        //}

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
                    var sl = el as ImportDataMTConnect;
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
    public class ImportDataModelSimotion : ImportDataModel, IDisposable
    {
        public ImportDataModelSimotion(GetStationName inGetStationName) :
            base(inGetStationName)
        {
        }

        public override ImportData addImportData()
        {
            ImportDataMTConnect importData = new ImportDataMTConnect(this);
            return importData;
        }
        #region IDisposable Members

        public void Dispose()
        {
            foreach (ImportData el in Children)
            {
                ImportDataMTConnect els7 = el as ImportDataMTConnect;
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

