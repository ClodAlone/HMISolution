using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Utilities;
using System.Reflection;
using UFUAModel;
using DriverCodeBase.UI;
using DriverCodeBase.UI.Controls;

namespace ModbusTCP.UI
{
    /// <summary>
    /// Interaction logic for ImportTagsEditor.xaml
    /// </summary>
    public partial class ImportTagsEditorTree : UserControl, IDisposable
    {

        BaseImportTree baseImportTree;

        ImportDataModel importDataModel;

        bool alreadyLoaded = false;

        public GetStationName readStationName;

        public ImportTagsEditorTree()
        {
            InitializeComponent();

            Loaded += (o, e) =>
            {
                if (alreadyLoaded)
                    return;

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
                    colWidth = 100
                });
                columns.Add(new BaseImportTree.GridColData()
                {
                    colName = "Type",
                    bindingName = "TagType",
                    colWidth = 210
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

                readStationName = baseImportTree.funcGetStationName();

                DataContext = this;
            };

        }

        public void ImportSelectedTags()
        {
            string importfolder = string.Empty;
            List<ImportData> list = new List<ImportData>();
            importfolder = baseImportTree.ReadFolderName();
            list = baseImportTree.GetSelectedTags();
            int behaviorExistingTags = baseImportTree.GetBehaviorForExistingTags();
            int behaviorDynamicLink = baseImportTree.GetBehaviorForDynamicLink();

            if (list.Count > 0)
            {
                
                List<ImportTag> taglist = new List<ImportTag>();
                List<ImportPrototype> protolist = new List<ImportPrototype>();
                foreach (var elem in list)
                {
                    if (elem != null)
                    {

                        bool isStructure = elem.Children.Count > 0;
                        ImportData single = elem;
                            ModbusTCPDynTagSettings sp = new ModbusTCPDynTagSettings();
                            if (!sp.ParseAddress(single.Address))
                                continue;
                            sp.StationName = baseImportTree.ReadStationName();
                            single.DynAddress = sp.ToString();

                            ImportTag tagtoimport = new ImportTag()
                            {
                                Name = single.Name,
                                DataType = (DataType)single.TagType,
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
            else
                DataContext = this;
        }

        private ImportDataModel LoadFile(string file)
        {
            importDataModel = new ImportDataModel(readStationName);

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
                        ModbusTCPDynTagSettings p = new ModbusTCPDynTagSettings();
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
                                p.StationName = baseImportTree.ReadStationName();
                                dynamicaddress = p.ToString();

                                if (s.Length > 2)
                                {
                                    MovType = GetMoviconTypeId(s[2], ref varsize);
                                    if (MovType != -1)
                                    {
                                        var IVar = importDataModel.addImportData();
                                        IVar.Name = s[0];
                                        IVar.Address = s[1].Trim();
                                        IVar.DynAddress = dynamicaddress;
                                        IVar.TagType = (DataType)MovType;
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
                    return null;
                }
            }

            if (importDataModel == null || importDataModel.Children.Count == 0)
            {
                MessageBox.Show(DriverCodeBase.UI.Properties.Resources.ImportErrorEmptyFile,
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
            if (Type.Equals("BIT"))
            {
                nType = (int)DataType.Boolean;
                VarSize = 1;
            }
            else if (Type.Equals("BOOL"))
            {
                nType = (int)DataType.Boolean;
                VarSize = 1;
            }
            else if (Type.Equals("BYTE"))
            {
                nType = (int)DataType.Byte;
                VarSize = 1;
            }
            else if (Type.Equals("WORD"))
            {
                nType = (int)DataType.UInt16;
                VarSize = 2;
            }
            else if (Type.Equals("DWORD"))
            {
                nType = (int)DataType.UInt32;
                VarSize = 4;
            }
            else if (Type.Equals("INT"))
            {
                nType = (int)DataType.Int16;
                VarSize = 2;
            }
            else if (Type.Equals("DINT"))
            {
                nType = (int)DataType.Int32;
                VarSize = 4;
            }
            else if (Type.Equals("REAL"))
            {
                nType = (int)DataType.Float;
                VarSize = 4;
            }
            else if (Type.Equals("CHAR"))
            {
                nType = (int)DataType.SByte;
                VarSize = 1;
            }
            else if (Type.Equals("DOUBLE"))
            {
                nType = (int)DataType.Double;
                VarSize = 8;
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
            
        }

        #endregion
    }
}
