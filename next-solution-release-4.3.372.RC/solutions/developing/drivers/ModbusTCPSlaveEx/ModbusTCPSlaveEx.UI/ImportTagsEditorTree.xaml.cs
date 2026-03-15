using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Utilities;
using System.Reflection;
using UFUAModel;
using DriverCodeBaseEx.UI;
using DriverCodeBaseEx.UI.Controls;

namespace ModbusTCPSlave.UI
{
    /// <summary>
    /// Interaction logic for ImportTagsEditor.xaml
    /// </summary>    
    public partial class ImportTagsEditorTree : UserControl, IDisposable
    {
        ImportDataModelModbusTCPSlave importDataModel;
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
                        List<ImportPrototype> protolist = new List<ImportPrototype>();
                        foreach (var elemList in list)
                        {
                            progressBar.ImportNewTag(elemList.Name);
                            progressBar.ThrowIfCancellationRequested();

                            ImportData elem = elemList as ImportData;
                            if (elem != null)
                            {
                                bool isStructure = elem.Children.Count > 0;
                                ImportData single = elem;
                                ModbusTCPSlaveDynTagSettings sp = new ModbusTCPSlaveDynTagSettings();
                                if (!sp.ParseAddress(single.Address))
                                    continue;
                                sp.StationName = stationName;
                                single.DynAddress = sp.ToString();

                                ImportTag tagtoimport = new ImportTag()
                                {
                                    Name = single.Name,
                                    DataType = (DataType)((ImportDataModbusTCPSlave)single).TagTypeInt,
                                    DynSettings = single.DynAddress,
                                    Folder = importfolder,
                                    ModelType = UFUAModel.ModelType.Variable,
                                    Description = single.Description,
                                    BehaviorForExistingTags = (BehaviorExistingTagsValues)behaviorExistingTags,
                                    BehaviorForDynamicLink = (BehaviorDynamicLinkValues)behaviorDynamicLink,
                                };

                                taglist.Add(tagtoimport);

                            }
                        }
                        DataContext = new ImportObject() { PrototypesToImport = protolist, TagsToImport = taglist };
                    }
                    catch (OperationCanceledException ex)
                    {
                        //UFUAServerDocument.logGeneral.Info(Properties.Resources.TagsImportCanceled);
                    }
                }
            }
        }
       
        private ImportDataModel LoadFile(string file)
        {
            string stationName = baseImportTree.ReadStationName();
            importDataModel = new ImportDataModelModbusTCPSlave(readStationName);
         
            file = file.ToLower();
            if (file.Contains(".csv"))
            {
                try
                {
                    using (new WaitCursor())
                    {
                        List<string[]> parsedData = new List<string[]>();
                        using (System.IO.StreamReader readFile = new System.IO.StreamReader(file))
                        {
                            string line;
                            string[] row;

                            while ((line = readFile.ReadLine()) != null)
                            {
                                row = line.Split(',');
                                parsedData.Add(row);
                            }
                        }
                        bool refresh = false;
                        ModbusTCPSlaveDynTagSettings p = new ModbusTCPSlaveDynTagSettings();
                        string dynamicaddress;
                        int MovType;
                        uint varsize;
                        foreach (var s in parsedData)
                        {
                            MovType = -1;
                            dynamicaddress = string.Empty;
                            varsize = 0;
                            if ((s.Length > 1) && !s[0].StartsWith("//") && p.ParseAddress(s[1].Trim()))
                            {
                                p.StationName = stationName;
                                dynamicaddress = p.ToString();

                                if (s.Length > 2)
                                {
                                    MovType = GetMoviconTypeId(s[2], ref varsize);
                                    if (MovType != -1)
                                    {
                                        refresh = true;
                                        var IVar = importDataModel.addImportData();
                                        IVar.Name = s[0];
                                        IVar.Address = s[1].Trim();
                                        IVar.DynAddress = dynamicaddress;
                                        ((ImportDataModbusTCPSlave)IVar).TagTypeInt = MovType;
                                        IVar.Select = false;
                                        IVar.szType = s[2];
                                        if (s.Length > 3)
                                            IVar.Description = s[3];
                                        else
                                            IVar.Description = "";
                                        AddTreeItem(IVar);
                                    }
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    string message;
                    message = String.Format(Properties.Resources.ImportExceptionFileLoading, file, ex.Message);
                    MessageBox.Show(message, Properties.Resources.ImportMsgBoxTitle);
                    importDataModel = null;
                }
            }

            if (importDataModel == null || importDataModel.Children.Count == 0)
            {
                MessageBox.Show(DriverCodeBaseEx.UI.Properties.Resources.ImportErrorEmptyFile,
                                Properties.Resources.ImportMsgBoxTitle);
            }

            return importDataModel;
        }

        public int GetMoviconTypeId(string Type, ref uint VarSize)
        {
            int nType = -1;
            VarSize = 0;
            Type.Trim();
            if (Type.Length == 0)
                return (nType);
            Type = Type.ToUpper();
            if (Type.Contains("BIT"))
            {
                nType = (int)DataType.Boolean;
                VarSize = 1;
            }
            else if (Type.Contains("BOOL"))
            {
                nType = (int)DataType.Boolean;
                VarSize = 1;
            }
            else if (Type.Contains("BYTE"))
            {
                nType = (int)DataType.Byte;
                VarSize = 1;
            }
            else if (Type.Contains("WORD"))
            {
                nType = (int)DataType.UInt16;
                VarSize = 2;
            }
            else if (Type.Contains("DWORD"))
            {
                nType = (int)DataType.UInt32;
                VarSize = 4;
            }
            else if (Type.Contains("INT"))
            {
                nType = (int)DataType.Int16;
                VarSize = 2;
            }
            else if (Type.Contains("DINT"))
            {
                nType = (int)DataType.Int32;
                VarSize = 4;
            }
            else if (Type.Contains("REAL"))
            {
                nType = (int)DataType.Float;
                VarSize = 4;
            }
            else if (Type.Contains("CHAR"))
            {
                nType = (int)DataType.SByte;
                VarSize = 1;
            }
            return (nType);

        }
  
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

    public class ImportDataModbusTCPSlave : ImportData, IDisposable
    {
        public ImportDataModbusTCPSlave(ImportDataModel inDataModel) :
            base(inDataModel)
        {
            dataModelModbusTCPSlave = inDataModel as ImportDataModelModbusTCPSlave;
        }
        private ImportDataModelModbusTCPSlave dataModelModbusTCPSlave { get; set; }

        private int _TagTypeInt;
        public int TagTypeInt
        {
            get { return _TagTypeInt; }
            set { _TagTypeInt = value; }
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
                    var sl = el as ImportDataModbusTCPSlave;
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

    public class ImportDataModelModbusTCPSlave : ImportDataModel, IDisposable
    {
        public ImportDataModelModbusTCPSlave(GetStationName inGetStationName) :
            base(inGetStationName)
        {
        }
        public override ImportData addImportData()
        {
            ImportDataModbusTCPSlave importData = new ImportDataModbusTCPSlave(this);
            return importData;
        }

        #region IDisposable Members
        public void Dispose()
        {
            foreach (ImportData el in Children)
            {
                ImportDataModbusTCPSlave els7 = el as ImportDataModbusTCPSlave;
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
