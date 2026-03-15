using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Utilities;
using System.Reflection;
using UFUAModel;
using System.Collections.ObjectModel;
using DriverCodeBaseEx.UI;
using DriverCodeBaseEx.UI.Controls;
using System.Linq;

namespace MelsecQEth.UI
{        
    /// <summary>
    /// Interaction logic for ImportTagsEditorTree.xaml
    /// </summary>
    public partial class ImportTagsEditorTree : UserControl, IDisposable
    {       
        ImportDataModelMelsecQ importDataModel;
        bool alreadyLoaded = false;
        BaseImportTree baseImportTree;

        public GetStationName readStationName;

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
                    colWidth = 200
                });
                columns.Add(new BaseImportTree.GridColData()
                {
                    colName = "Type",
                    bindingName = "TagType",
                    colWidth = 200
                });
                columns.Add(new BaseImportTree.GridColData()
                {
                    colName = "Note",
                    bindingName = "TagDescription",
                    colWidth = 400
                });

                baseImportTree = new BaseImportTree(DriverName, lista[0],
                    (lista[1].ToLower().IndexOf("true") != -1), columns);
                baseImportTree.SetVisibleButtons();
                baseImportTree.SetFileFilter("csv files|*.csv");
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
                Dictionary<string, ImportPrototype> protoMap = new Dictionary<string, ImportPrototype>();
                ObservableCollection<object> notToBeImported = new ObservableCollection<object>();
                foreach (var single in list)
                {
                    if (single != null && (notToBeImported.IndexOf(single) < 0))
                    {                     
                        bool isArray = (single.ArrayDimension > 0) ? true : false;

                        MelsecQEthDynTagSettings sp = new MelsecQEthDynTagSettings();

                        if (!sp.ParseAddress(((ImportDataMelsecQ)single).AddressType, single.Address))
                            continue;
                        sp.StationName = stationName;                        
                        sp.ArrayDimension = single.ArrayDimension;
                        switch (((ImportDataMelsecQ)single).TagTypeInt)
                        {
                            case (int)DataType.String:
                                sp.VarType = UFUAModel.DataType.String;
                                sp.StringLength = (ushort)MelsecQEthFileImportParser.GetStringLength(single.szType);
                                sp.UnicodeString = single.szType.Contains("WSTRING");
                                break;
                            case MelsecQEthFileImportParser.ImportDataTypeStruct:
                                ImportPrototype proto = addPrototype(protoMap, single);
                                if (proto == null)
                                    continue;                                
                                break;
                            case MelsecQEthFileImportParser.ImportUnknownDataType:
                                continue;
                                break;
                        }
                        single.DynAddress = sp.ToString();

                        ImportTag tagtoimport = new ImportTag()
                        {
                            Name = single.Name,
                            DataType = (DataType)((ImportDataMelsecQ)single).TagTypeInt,
                            DynSettings = single.DynAddress,
                            Folder = importfolder,
                            ModelType = UFUAModel.ModelType.Variable,
                            Description = single.Description,
                            ArrayDimension = single.ArrayDimension
                        };
                        if (((ImportDataMelsecQ)single).TagTypeInt == MelsecQEthFileImportParser.ImportDataTypeStruct)
                        {                            
                            tagtoimport.ModelType = UFUAModel.ModelType.ObjectType;
                            tagtoimport.PrototypeModel = ((ImportDataMelsecQ)single).Prototype;
                        }

                        taglist.Add(tagtoimport);

                        if (isArray)
                        {
                            foreach (ImportData el in single.Children)
                            {
                                if (el != null)
                                    notToBeImported.Add(el);
                            }
                        }
                    }
                }
                DataContext = new ImportObject() { PrototypesToImport = protoMap.Values.ToList(), TagsToImport = taglist };
            }
            else
                DataContext = this;
        }

        private ImportDataModel LoadFile(string file)
        {
            if (importDataModel != null)
            {
                importDataModel.Dispose();
                importDataModel = null;
            }

            MelsecQEthStationSettings station = (MelsecQEthStationSettings)baseImportTree.GetStationSettings(baseImportTree.ReadStationName());

            MelsecQEthFileImportParser importParser = new MelsecQEthFileImportParser();

            bool import = importParser.Import(station, readStationName, file);
            if (import)
            {
                importDataModel = importParser.GetImportedVariables();
            }
            else
            {
                MessageBox.Show(importParser.LastError, Properties.Resources.ImportMsgBoxTitle);
                importDataModel = null;
            }
            
            return importDataModel;

        }

        private bool IsAllPrototypeMembersValid(ImportData elem)
        {
            bool result = true;
            foreach (var t in elem.Children)
            {
                if (((ImportDataMelsecQ)t).TagTypeInt == MelsecQEthFileImportParser.ImportDataTypeStruct)
                {
                    if (!IsAllPrototypeMembersValid(t))
                    {
                        result = false; 
                        break; 
                    }
                }
                else
                {
                    if (((ImportDataMelsecQ)t).TagTypeInt == MelsecQEthFileImportParser.ImportUnknownDataType)
                    {
                        result = false;
                        break;
                    }
                }                
            }
        
            return result;
        }

        private ImportPrototype addPrototype(Dictionary<string, ImportPrototype> protoMap, ImportData elem)
        {
            ImportPrototype proto = null;

            if (!IsAllPrototypeMembersValid(elem))
                return proto;

            string protoname = ((ImportDataMelsecQ)elem).Prototype;
            if (!protoMap.ContainsKey(protoname))
            {
                if (elem.Children.Count != 0)
                {
                    proto = new ImportPrototype();
                    proto.Name = UFUAModel.Helpers.NameValidator.EnsureValidName(protoname);
                    proto.Elements = new List<ImportTag>();

                    foreach (var t in elem.Children)
                    {
                        ImportTag a = new ImportTag();

                        if (((ImportDataMelsecQ)t).TagTypeInt == MelsecQEthFileImportParser.ImportDataTypeStruct)
                        {
                            ImportPrototype protoElem = addPrototype(protoMap, t);
                            if (protoElem == null)
                                continue;

                            a.ModelType = UFUAModel.ModelType.ObjectType;
                            a.PrototypeModel = protoElem.Name;
                        }
                        else
                        {
                            a.ModelType = UFUAModel.ModelType.Variable;
                        }
                        a.ArrayDimension = t.ArrayDimension;
                        a.Name = t.Name;
                        a.DataType = (DataType)((ImportDataMelsecQ)t).TagTypeInt;
                        a.Description = t.Description;
                        proto.Elements.Add(a);
                    }
                    if (!protoMap.ContainsKey(protoname))
                    {
                        protoMap.Add(protoname, proto);
                    }
                }
            }
            else
            {
                proto = protoMap[protoname];
            }
            return proto;
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
    
    public class ImportDataMelsecQ : ImportData, IDisposable
    {
        public ImportDataMelsecQ(ImportDataModel inDataModel) :
            base(inDataModel)
        {
            dataModelMelsecQ = inDataModel as ImportDataModelMelsecQ;
            _AddressType = MelsecQEthProtocol.AddressTypes.DataArea;
            _Prototype = string.Empty;
        }
        private ImportDataModelMelsecQ dataModelMelsecQ { get; set; }

        private int _TagTypeInt;
        public int TagTypeInt
        {
            get { return _TagTypeInt; }
            set { _TagTypeInt = value; }
        }

        public override DataType IconTagType
        {
            get
            {
                if (_TagTypeInt == MelsecQEthFileImportParser.ImportUnknownDataType || _TagTypeInt == MelsecQEthFileImportParser.ImportDataTypeStruct)
                    return DataType.Boolean;
                else
                    return (DataType)_TagTypeInt;
            }
        }

        private MelsecQEthProtocol.AddressTypes _AddressType;
        public MelsecQEthProtocol.AddressTypes AddressType
        {
            get { return _AddressType; }
            set { _AddressType = value; }
        }        

        private string _Prototype;

        public string Prototype
        {
            get { return _Prototype; }
            set { _Prototype = value; }
        }

        #region IDisposable Members
        public void Dispose()
        {
            if (Children != null && Children.Count > 0)
            {
                foreach (ImportData el in Children)
                {
                    var sl = el as ImportDataMelsecQ;
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

    public class ImportDataModelMelsecQ : ImportDataModel, IDisposable
    {
        public ImportDataModelMelsecQ(GetStationName inGetStationName) :
            base(inGetStationName)
        {
        }
        public override ImportData addImportData()
        {
            ImportDataMelsecQ importData = new ImportDataMelsecQ(this);
            return importData;
        }

        #region IDisposable Members
        public void Dispose()
        {
            foreach (ImportData el in Children)
            {
                ImportDataMelsecQ els7 = el as ImportDataMelsecQ;
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
