////////////////////////////////////////////////////////////////////////////////////////////////////
// file:	ImportTagsEditorTree.xaml.cs
//
// summary:	Implements the import tags editor tree.xaml class
////////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Utilities;
using System.Reflection;
using UFUAModel;
using DriverCodeBaseEx.UI;
using DriverCodeBaseEx.UI.Controls;
using System.Linq;

namespace GESRTP2.UI
{
    /// <summary>   Interaction logic for ImportTagsEditor.xaml. </summary>
    public partial class ImportTagsEditorTree : UserControl, IDisposable
    {        
        /// <summary>   The import data model. </summary>
        ImportDataModelGESRTP2 importDataModel;
        string conn;
        /// <summary>   true if already loaded. </summary>
        bool alreadyLoaded = false;
        /// <summary>   Name of the read station. </summary>
        public GetStationName readStationName;
        BaseImportTree baseImportTree;

        /// <summary>   Default constructor. </summary>
        public ImportTagsEditorTree()
        {
            InitializeComponent();

            //executed at loaded
            Loaded += (o, e) =>
            {
                if (alreadyLoaded)
                {
                    if (importDataModel != null)
                    {
                        importDataModel.Dispose();
                        importDataModel = null;
                    }
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
                columns.Add(new BaseImportTree.GridColData()
                {
                    colName = "Publish",
                    bindingName = "TagPublish",
                    colWidth = 200
                });
                columns.Add(new BaseImportTree.GridColData()
                {
                    colName = "Description",
                    bindingName = "TagDescription",
                    colWidth = 210
                });

                conn = lista[0];
                baseImportTree = new BaseImportTree(DriverName, lista[0],
                    (lista[1].ToLower().IndexOf("true") != -1), columns);
                baseImportTree.SetVisibleButtons();
                baseImportTree.SetFileFilter(".csv files, .snf files|*.csv;*.snf");
                baseImportTree.LoadImportFile = LoadFile;
                baseImportTree.CreateItemControl = CreateImportDataTreeItemControl;
                MainStack.Children.Add(baseImportTree);

                readStationName = baseImportTree.funcGetStationName();

                DataContext = this;
            };
        }

        private ImportDataTreeItemControl CreateImportDataTreeItemControl(object tag)
        {
            return ((ImportDataTreeItemControl)new ImportDataTreeItemControlGESRTP2(tag));
        }

        /// <summary>   Imports all selected tags. </summary>
        public void ImportSelectedTags()
        {
            string importfolder = baseImportTree.ReadFolderName();
            List<ImportData> list = baseImportTree.GetSelectedTags();
            string stationName = baseImportTree.ReadStationName();
            int behaviorExistingTags = baseImportTree.GetBehaviorForExistingTags();
            int behaviorDynamicLink = baseImportTree.GetBehaviorForDynamicLink();

            if (list.Count > 0)
            {
                // Set the number of tags that will be imported
                using (ImportDataProgressBar progressBar = new ImportDataProgressBar())
                {
                    // Set the number of tags that will be imported
                    progressBar.StartImport(list.Count);
                    try
                    {
                        List<ImportTag> taglist = new List<ImportTag>();
                        Dictionary<string, ImportPrototype> protoMap = new Dictionary<string, ImportPrototype>();

                        foreach (var elem in list)
                        {
                            ImportDataGESRTP2 single = elem as ImportDataGESRTP2;
                            if (single != null)
                            {
                                progressBar.ImportNewTag(single.Name);
                                progressBar.ThrowIfCancellationRequested();

                                if (single.AreaType == AreaTypes.Symbolic)
                                {
                                    bool isStructure = single.Children.Count() > 0;
                                    if (single.ArrayDimension > 0)
                                        isStructure = false;

                                    GESRTP2DynTagSettings sp = new GESRTP2DynTagSettings();
                                    sp.VarType = single.TagType;
                                    sp.AreaType = single.AreaType;
                                    sp.SymbolicAddress = single.Address;
                                    sp.StationName = stationName;
                                    sp.ArrayDimension = single.ArrayDimension;
                                    //if (single.TagType == DataType.String)
                                    //    sp.StringLength = single.StringLength;

                                    // Set the job type
                                    sp.TagLinkType = (int)single.JobType;

                                    single.DynAddress = sp.ToString();

                                    ImportPrototype proto = null;
                                    string protoname = single.szType;
                                    if (isStructure)
                                    {
                                        proto = AddPrototypeSymbolic(protoMap, single, progressBar);
                                        if (proto == null)
                                            continue;
                                    }

                                    ImportTag tagtoimport = new ImportTag()
                                    {
                                        Name = ((ImportDataGESRTP2)single).ImportTagName,
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
                                        tagtoimport.PrototypeModel = proto.Name;
                                        tagtoimport.ModelType = UFUAModel.ModelType.ObjectType;
                                    }

                                    taglist.Add(tagtoimport);
                                }
                                else
                                {
                                    GESRTP2DynTagSettings sp = new GESRTP2DynTagSettings();
                                    sp.StationName = stationName;
                                    sp.AreaType = single.AreaType;
                                    if (single.TagType == DataType.String)
                                        sp.StringLength = single.StringLength;
                                    if (GESRTP2Protocol.DataType(single.AreaType) == Opc.Ua.BuiltInType.Byte)
                                        sp.StartAddress = (ushort)((((ImportDataGESRTP2)single).AddressDataArea - 1) / 8 + 1);
                                    else
                                        sp.StartAddress = ((ImportDataGESRTP2)single).AddressDataArea;

                                    sp.TagLinkType = (int)GESRTP2Protocol.AreaLinkType(single.AreaType);
                                    sp.VarType = single.TagType;

                                    ImportTag tagtoimport = new ImportTag()
                                    {
                                        Name = single.Name,
                                        DataType = single.TagType,
                                        DynSettings = sp.ToString(),
                                        Folder = importfolder,
                                        ModelType = UFUAModel.ModelType.Variable,
                                        Description = single.Description,
                                        ArrayDimension = single.ArrayDimension,
                                        BehaviorForExistingTags = (BehaviorExistingTagsValues)behaviorExistingTags,
                                        BehaviorForDynamicLink = (BehaviorDynamicLinkValues)behaviorDynamicLink,
                                    };

                                    taglist.Add(tagtoimport);
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

        private static ImportPrototype AddPrototypeSymbolic(Dictionary<string, ImportPrototype> protoMap, ImportData elem, ImportDataProgressBar progressBar)
        {
            progressBar.ThrowIfCancellationRequested();
            progressBar.UpdateImportingTag(elem.Name);

            ImportPrototype proto = null;
            if (!protoMap.ContainsKey(((ImportDataGESRTP2)elem).ElemType))
            {
                if (elem.Children.Count != 0)
                {
                    proto = new ImportPrototype();
                    proto.Name = ((ImportDataGESRTP2)elem).ElemType;
                    proto.Elements = new List<ImportTag>();

                    foreach (var t in elem.Children)
                    {
                        ImportData el = t as ImportData;
                        if (el != null)
                        {                            
                            ImportTag a = new ImportTag();
                            if (((ImportDataGESRTP2)el).ImportDataType == GESRTP2FileImportBase.ImportTypes.Struct)
                            {
                                ImportPrototype protoElem = AddPrototypeSymbolic(protoMap, el, progressBar);
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
                            a.DataType = el.TagType;
                            a.Description = el.Description;
                            proto.Elements.Add(a);
                        }
                    }
                    protoMap.Add(((ImportDataGESRTP2)elem).ElemType, proto);
                }
            }
            else
            {
                proto = protoMap[((ImportDataGESRTP2)elem).ElemType];
            }
            return proto;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Reads the tags from an input file. </summary>
        /////
        /// <exception cref="Exception">    Thrown when an exception error condition occurs. </exception>
        ///
        /// <param name="sender">   . </param>
        /// <param name="e">        . </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        private ImportDataModel LoadFile(string file)
        {
            if (importDataModel != null)
            {
                importDataModel.Dispose();
                importDataModel = null;
            }

            using (GESRTP2Station station = new GESRTP2Station(new GESRTP2Driver(conn), (GESRTP2StationSettings)baseImportTree.GetStationSettings(baseImportTree.ReadStationName())))
            {
                GESRTP2FileImportParser importParser = new GESRTP2FileImportParser();

                bool import = importParser.Import(station.PlcType, readStationName, file);
                if (import)
                {
                    importDataModel = importParser.GetImportedVariables();
                }
                else
                {
                    MessageBox.Show(importParser.LastError, Properties.Resources.ImportMsgBoxTitle);
                    importDataModel = null;
                }
            }

            return importDataModel;
        }
        #region IDisposable Members

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged
        /// resources.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
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

    /// <summary>   Import data. </summary>
    public class ImportDataGESRTP2 : ImportData, IDisposable
    {        
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Constructor. </summary>
        ///
        /// <param name="inDataModel" type="ImportDataModel">   The in data model. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public ImportDataGESRTP2(ImportDataModel inDataModel) : 
            base(inDataModel)
        {        
            dataModelGESRTP2 = inDataModel as ImportDataModelGESRTP2;

            JobType = DriverCodeBaseEx.Enumerators.LinkType.InputOutput;
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets or sets the data model. </summary>
        ///
        /// <value> The data model. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        private ImportDataModelGESRTP2 dataModelGESRTP2 { get; set; }

        private string _Name;
        public override string Name
        {
            get { return _Name; }
            set { _Name = value; }
        }

        //N.B.: In Movicon 3.4 property Address was defined string type, so we defined a new AddressNum (ushort) and update import
        private ushort _AddressDataArea;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets or sets the address. </summary>
        ///
        /// <value> The address. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public ushort AddressDataArea
        {
            get { return _AddressDataArea; }
            set { _AddressDataArea = value; }
        }
       
        private AreaTypes _AreaType;
        public AreaTypes AreaType
        {
            get { return _AreaType; }
            set { _AreaType = value; }
        }

        private uint _StringLength;
        public uint StringLength
        {
            get { return _StringLength; }
            set { _StringLength = value; }
        }

        private string _PreName;
        public string PreName
        {
            get { return _PreName; }
            set { _PreName = value; }
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

        private GESRTP2FileImportBase.ImportTypes _ImportDataType;
        public GESRTP2FileImportBase.ImportTypes ImportDataType
        {
            get { return _ImportDataType; }
            set { _ImportDataType = value; }
        }

        public string ImportTagName
        {
            get
            {
                return dataModelGESRTP2.getStationName() != "_" ?
                       dataModelGESRTP2.getStationName() + _PreName + _Name : _PreName + _Name;
            }
        }

        private string _ElemType;
        public string ElemType
        {
            get { return _ElemType; }
            set { _ElemType = value; }
        }

        private string _Publish;
        public string Publish
        {
            get { return _Publish; }
            set { _Publish = value; }
        }

        #region IDisposable Members
        public void Dispose()
        {
            if (Children != null && Children.Count > 0)
            {
                foreach (ImportData el in Children)
                {
                    var sl = el as ImportDataGESRTP2;
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

    public class ImportDataModelGESRTP2 : ImportDataModel, IDisposable
    {
        public ImportDataModelGESRTP2(GetStationName inGetStationName) :
            base(inGetStationName)
        {
        }

        public override ImportData addImportData()
        {
            ImportDataGESRTP2 importData = new ImportDataGESRTP2(this);
            return importData;
        }

        #region IDisposable Members

        public void Dispose()
        {
            foreach (ImportData el in Children)
            {
                ImportDataGESRTP2 els7 = el as ImportDataGESRTP2;
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
