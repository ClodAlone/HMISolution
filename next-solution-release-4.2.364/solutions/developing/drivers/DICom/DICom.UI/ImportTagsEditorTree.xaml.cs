using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Utilities;
using System.Reflection;
using UFUAModel;
using DriverCodeBase.UI;
using DriverCodeBase.UI.Controls;

namespace DICom.UI
{
    /// <summary>
    /// Interaction logic for ImportTagsEditor.xaml
    /// </summary>    
    public partial class ImportTagsEditorTree : UserControl, IDisposable
    {
        ImportDataModelDICom importDataModel;
        BaseImportTree baseImportTree;

        bool alreadyLoaded = false;
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
                    colName = "Description",
                    bindingName = "TagDescription",
                    colWidth = 210
                });

                baseImportTree = new BaseImportTree(DriverName, lista[0],
                    (lista[1].ToLower().IndexOf("true") != -1), columns);
                baseImportTree.SetVisibleButtons();
                baseImportTree.SetFileFilter("csv files|*.csv");
                baseImportTree.LoadImportFile = LoadFile;
                MainStack.Children.Add(baseImportTree);

                baseImportTree.AddStationName.IsChecked = true;
                readStationName = baseImportTree.funcGetStationName();

                DataContext = this;
            };

        }
        public void ImportSelectedTags()
        {
            string importfolder = baseImportTree.ReadFolderName();            
            List<ImportData> list = baseImportTree.GetSelectedTags();
            string stationName = baseImportTree.ReadStationName();

            if (list.Count > 0)
            {
                List<ImportTag> taglist = new List<ImportTag>();
                List<ImportPrototype> protolist = new List<ImportPrototype>();
                foreach (var elemList in list)
                {
                    ImportData elem = elemList as ImportData;
                    if (elem != null)
                    {
                        //bool isStructure = elem.Children.Count > 0;
                        ImportData single = elem;
                        DIComDynTagSettings sp = new DIComDynTagSettings();
                        sp.StationName = stationName;
                        sp.VarName = ((ImportDataDICom)elem).VarName;
                        single.DynAddress = sp.ToString();

                        ImportTag tagtoimport = new ImportTag()
                        {
                            Name = single.Name,                            
                            DataType = single.TagType,
                            DynSettings = single.DynAddress,
                            Folder = importfolder,
                            ModelType = UFUAModel.ModelType.Variable,
                            Description = single.Description,
                        };

                        taglist.Add(tagtoimport);

                    }
                }
                DataContext = new ImportObject() { PrototypesToImport = protolist, TagsToImport = taglist };
            }
            else
                DataContext = this;
        }

        private ImportDataModel LoadFile(string file)
        {
            string stationName = baseImportTree.ReadStationName();
            importDataModel = new ImportDataModelDICom(readStationName);
         
            file = file.ToLower();
            if (file.Contains(".csv"))
            {
                using (new WaitCursor())
                {
                    ImportFromFile parse = new ImportFromFile();
                    if (parse.ParseCsv(file, out string errorMessage))
                    {                        
                        foreach (var var in parse.ParsedVars)
                        {
                            int movType = DIComProtocol.GetMoviconTypeId(var.VarType);
                            if (movType != -1)
                            {
                                var IVar = importDataModel.addImportData();
                                IVar.Name = UFUAModel.Helpers.NameValidator.EnsureValidName(parse.GetTagName(var));
                                ((ImportDataDICom)IVar).VarName = var.VarName;
                                IVar.Address = var.VarName;
                                IVar.TagType = (UFUAModel.DataType)movType;
                                IVar.szType = ((UFUAModel.DataType)movType).ToString();
                                IVar.Description = var.Note;                                
                                AddTreeItem(IVar);
                            }
                        }
                    } 
                    else
                    {
                        MessageBox.Show(String.Format(Properties.Resources.ErrorImportFromFile, file, errorMessage), Properties.Resources.ImportMsgBoxTitle);
                    }
                }
                if (importDataModel == null || importDataModel.Children.Count == 0)
                {
                    MessageBox.Show(DriverCodeBase.UI.Properties.Resources.ImportErrorEmptyFile,Properties.Resources.ImportMsgBoxTitle);
                }
            }

            return importDataModel;
        }        
  
        internal void AddTreeItem(ImportData tag, ImportData parent = null)
        {
            tag.Parent = parent;
            if (parent == null)
            {
                tag.TreeLevel = 0xFF;
                tag.parentId = -1;                                
                importDataModel.Children.Add(tag);
            }
            else
            {
                tag.TreeLevel = parent.TreeLevel - 1;
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
            }
        }
        #endregion
    }

    public class ImportDataDICom : ImportData, IDisposable
    {
        public ImportDataDICom(ImportDataModel inDataModel) :
            base(inDataModel)
        {
            dataModelDICom = inDataModel as ImportDataModelDICom;
        }
        private ImportDataModelDICom dataModelDICom { get; set; }

        private string _VarName;
        public string VarName
        {
            get { return _VarName; }
            set { _VarName = value; }
        }

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

        #region IDisposable Members
        public void Dispose()
        {
            if (Children != null && Children.Count > 0)
            {
                foreach (ImportData el in Children)
                {
                    var sl = el as ImportDataDICom;
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

    public class ImportDataModelDICom : ImportDataModel, IDisposable
    {
        public ImportDataModelDICom(GetStationName inGetStationName) :
            base(inGetStationName)
        {
        }
        public override ImportData addImportData()
        {
            ImportDataDICom importData = new ImportDataDICom(this);
            return importData;
        }

        #region IDisposable Members
        public void Dispose()
        {
            foreach (ImportData el in Children)
            {
                ImportDataDICom els7 = el as ImportDataDICom;
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
