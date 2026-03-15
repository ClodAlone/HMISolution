////////////////////////////////////////////////////////////////////////////////////////////////////
// file:	ImportTagsEditorTree.xaml.cs
//
// summary:	Implements the import tags editor tree.xaml class
////////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Utilities;
//using Utilities.WPF;
//using DevExpress.Xpo;
using System.Reflection;
//using DevExpress.Xpo.DB;
//using Ookii.Dialogs.Wpf;
using UFUAModel;
//using Aga.Controls.Tree;
//using System.Collections.ObjectModel;
//using System.Windows.Documents;
//using System.Windows.Media;
//using System.ComponentModel;
using DriverCodeBase.UI;
using DriverCodeBase.UI.Controls;

namespace DriverSerialExample.UI
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>   Gets station name. </summary>
    ///
    /// <returns>   A string. </returns>
    ////////////////////////////////////////////////////////////////////////////////////////////////////
    //public delegate string GetStationName();

    /// <summary>   Interaction logic for ImportTagsEditor.xaml. </summary>
    public partial class ImportTagsEditorTree : UserControl, IDisposable
    {

        /// <summary>   The import data model. </summary>
        //ImportDataModel importDataModel;
        ImportDataModelDriverSerialExample importDataModel;
        BaseImportTree baseImportTree;

        ///// <summary>   The connection. </summary>
        //string conn;
        ///// <summary>   The idl. </summary>
        //IDataLayer idl = null;
        /// <summary>   true if already loaded. </summary>
        bool alreadyLoaded = false;
        ///// <summary>   List of stations. </summary>
        //Dictionary<int, string> stationList;
        ///// <summary>   true to protect. </summary>
        //bool protect;
        ///// <summary>   The file base. </summary>
        //string fileBase;
        ///// <summary>   The in memory. </summary>
        //InMemoryDataStore InMemory = null;
        /// <summary>   Name of the read station. </summary>
        public GetStationName readStationName;

        /// <summary>   Default constructor. </summary>
        public ImportTagsEditorTree()
        {
            InitializeComponent();

            //executed at loaded
            Loaded += (o, e) =>
            {
                if (alreadyLoaded)
                {
                    //if (listViewSortCol != null)
                    //{
                    //    AdornerLayer.GetAdornerLayer(listViewSortCol).Remove(listViewSortAdorner);
                    //    ImportTree.Items.SortDescriptions.Clear();
                    //}
                    //importDataModel = null;
                    //ImportTree.Model = importDataModel;
                    //CmbStation.Text = "";
                    importDataModel = null;
                    return;
                }

                alreadyLoaded = true;
                //stationList = new Dictionary<int, string>();

                List<string> lista = DataContext as List<string>;
                if (lista == null || lista.Count < 2)
                    return;

                //conn = lista[0];
                //protect = (lista[1].ToLower().IndexOf("true") != -1);

                Assembly a = Assembly.GetAssembly(this.GetType());
                string DriverName = a.GetName().Name;
                DriverName = DriverName.Replace(".UI", "");
                //idl = DriverCodeBase.CommunicationDriver.GetDriverDataLayer(conn, DriverName);
                //fileBase = DriverCodeBase.CommunicationDriver.GetFileBase(conn, DriverName);

                //if (fileBase.Length > 0)
                //{
                //    InMemory = DriverCodeBase.CommunicationDriver.GetDataStore(fileBase);
                //    idl = new SimpleDataLayer(InMemory);
                //}

                //using (var ufw = new UnitOfWork(idl))
                //{

                //    var stationssettings = (from drvsettings in new XPQuery<DriverCodeBase.DriverSettings>(ufw).AsParallel()
                //                            select drvsettings.StationSettings).ToList();

                //    foreach (var stationsettings in stationssettings)
                //    {
                //        foreach (var settings in stationsettings)
                //        {
                //            stationList.Add(stationList.Count, settings.Name);
                //        }
                //    }
                //    CmbStation.ItemsSource = stationList;
                //    CmbStation.IsEditable = true;
                //}

                //readStationName = () => AddStationName.IsChecked == true ? CmbStation.Text + "_" : "";
                //DependencyPropertyDescriptor descriptor =
                //   DependencyPropertyDescriptor.FromProperty(CheckBox.IsCheckedProperty, typeof(CheckBox));
                //descriptor.AddValueChanged(AddStationName, ImportTree_Changed);
                //descriptor =
                //  DependencyPropertyDescriptor.FromProperty(ComboBox.TextProperty, typeof(ComboBox));
                //descriptor.AddValueChanged(CmbStation, ImportTree_Changed);

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

        private ImportDataModel LoadFile(string file)
        {
            string stationName = baseImportTree.ReadStationName();

            importDataModel = new ImportDataModelDriverSerialExample(readStationName);

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
                        //bool refresh = false;
                        DriverSerialExampleDynTagSettings p = new DriverSerialExampleDynTagSettings();
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
                                        //refresh = true;
                                        var IVar = importDataModel.addImportData();
                                        IVar.Name = s[0];
                                        IVar.Address = s[1].Trim();
                                        IVar.DynAddress = dynamicaddress;
                                        ((ImportDataDriverSerialExample)IVar).TagTypeInt = MovType;
                                        IVar.Select = false;
                                        IVar.szType = s[2];
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

        //////////////////////////////////////////////////////////////////////////////////////////////////////
        ///// <summary>   Event handler. Called by ImportTree for changed events. </summary>
        /////
        ///// <param name="sender">               . </param>
        ///// <param name="e" type="EventArgs">   Event information. </param>
        //////////////////////////////////////////////////////////////////////////////////////////////////////
        //private void ImportTree_Changed(object sender, EventArgs e)
        //{
        //    ImportTree.Items.Refresh();
        //}

        ///// <summary>   The list view sort col. </summary>
        //private GridViewColumnHeader listViewSortCol = null;
        ///// <summary>   The list view sort adorner. </summary>
        //private SortAdorner listViewSortAdorner = null;

        //////////////////////////////////////////////////////////////////////////////////////////////////////
        ///// <summary>   Sort row by column header. </summary>
        /////
        ///// <param name="sender">   . </param>
        ///// <param name="e">        . </param>
        //////////////////////////////////////////////////////////////////////////////////////////////////////
        //private void lvUsersColumnHeader_Click(object sender, RoutedEventArgs e)
        //{
        //    GridViewColumnHeader column = (sender as GridViewColumnHeader);
        //    string sortBy = "Tag.Sort" + column.Tag.ToString();
        //    if (listViewSortCol != null)
        //    {
        //        AdornerLayer.GetAdornerLayer(listViewSortCol).Remove(listViewSortAdorner);
        //        ImportTree.Items.SortDescriptions.Clear();
        //    }

        //    ListSortDirection newDir = ListSortDirection.Ascending;
        //    if (listViewSortCol == column && listViewSortAdorner.Direction == newDir)
        //    {
        //        newDir = ListSortDirection.Descending;
        //        sortBy = sortBy + "R";
        //    }

        //    listViewSortCol = column;
        //    listViewSortAdorner = new SortAdorner(listViewSortCol, newDir);
        //    AdornerLayer.GetAdornerLayer(listViewSortCol).Add(listViewSortAdorner);
        //    ImportTree.Items.SortDescriptions.Add(new SortDescription(sortBy, newDir));
        //}

        //////////////////////////////////////////////////////////////////////////////////////////////////////
        ///// <summary>   Select all tags read. </summary>
        /////
        ///// <param name="sender">   . </param>
        ///// <param name="e">        . </param>
        //////////////////////////////////////////////////////////////////////////////////////////////////////
        //private void SelAll_Click(object sender, RoutedEventArgs e)
        //{
        //    ImportTree.SelectAll();
        //}

        //////////////////////////////////////////////////////////////////////////////////////////////////////
        ///// <summary>   Deselect all tags read. </summary>
        /////
        ///// <param name="sender">   . </param>
        ///// <param name="e">        . </param>
        //////////////////////////////////////////////////////////////////////////////////////////////////////
        //private void Clear_Click(object sender, RoutedEventArgs e)
        //{
        //    ImportTree.UnselectAll();
        //}

        /// <summary>   Imports all selected tags. </summary>
        public void ImportSelectedTags()
        {
            string importfolder = baseImportTree.ReadFolderName();
            List<ImportData> list = new List<ImportData>();
            list = baseImportTree.GetSelectedTags();
            string stationName = baseImportTree.ReadStationName();
            int behaviorExistingTags = baseImportTree.GetBehaviorForExistingTags();
            int behaviorDynamicLink = baseImportTree.GetBehaviorForDynamicLink();

            //if (CmbStation.Text.Length == 0)
            //    return;
            //if (ImportTree.SelectedItems.Count > 0)
            //{
            //    string importfolder = CmbStation.Text;
            //    List<ImportTag> taglist = new List<ImportTag>();
            //    List<ImportPrototype> protolist = new List<ImportPrototype>();
            //    foreach (TreeNode tvi in ImportTree.SelectedItems)
            //    {
            //        ImportData elem = tvi.Tag as ImportData;
            //        if (elem != null)
            //        {
            //            bool isStructure = elem.Children.Count > 0;
            //            ImportData single = elem;
            //            DriverSerialExampleDynTagSettings sp = new DriverSerialExampleDynTagSettings();
            //                if (!sp.ParseAddress(single.Address))
            //                    continue;
            //                sp.StationName = CmbStation.Text;
            //                single.DynAddress = sp.ToString();

            //                ImportTag tagtoimport = new ImportTag()
            //                {
            //                    Name = single.Name,
            //                    DataType = (DataType)single.TagType,
            //                    DynSettings = single.DynAddress,
            //                    Folder = importfolder,
            //                    ModelType = UFUAModel.ModelType.Variable,
            //                };

            //                taglist.Add(tagtoimport);

            //        }
            //    }
            //    DataContext = new ImportObject() { PrototypesToImport = protolist, TagsToImport = taglist };
            //}
            //else
            //    DataContext = null;
            if (list.Count > 0)
            {
                List<ImportTag> taglist = new List<ImportTag>();
                List<ImportPrototype> protolist = new List<ImportPrototype>();
                foreach (var elem in list)
                {
                    ImportData single = elem as ImportData;
                    if (single != null)
                    {
                        bool isStructure = single.Children.Count > 0;
                        DriverSerialExampleDynTagSettings sp = new DriverSerialExampleDynTagSettings();
                        if (!sp.ParseAddress(single.Address))
                            continue;
                        sp.StationName = stationName;
                        single.DynAddress = sp.ToString();

                        ImportTag tagtoimport = new ImportTag()
                        {
                            Name = single.Name,
                            DataType = (DataType)((ImportDataDriverSerialExample)single).TagTypeInt,
                            DynSettings = single.DynAddress,
                            Folder = importfolder,
                            ModelType = UFUAModel.ModelType.Variable,
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

        //////////////////////////////////////////////////////////////////////////////////////////////////////
        ///// <summary>   Start importing the selected tag. </summary>
        /////
        ///// <param name="sender">   . </param>
        ///// <param name="e">        . </param>
        //////////////////////////////////////////////////////////////////////////////////////////////////////
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

        //////////////////////////////////////////////////////////////////////////////////////////////////////
        ///// <summary>   Reads the tags from an input file. </summary>
        /////
        ///// <exception cref="Exception">    Thrown when an exception error condition occurs. </exception>
        /////
        ///// <param name="sender">   . </param>
        ///// <param name="e">        . </param>
        //////////////////////////////////////////////////////////////////////////////////////////////////////
        //private void LoadFile_Click(object sender, RoutedEventArgs e)
        //{
        //    importDataModel = new ImportDataModel(readStationName);

        //    string file = String.Empty;
        //    if (Environment.UserInteractive)
        //    {
        //        VistaOpenFileDialog dialog = new VistaOpenFileDialog();
        //        dialog.Filter = "csv files|*.csv";
        //        if (dialog.ShowDialog() != true)
        //            file = String.Empty;
        //        file = dialog.FileName;
        //    }
        //    if (String.IsNullOrEmpty(file))
        //        return;

        //    file = file.ToLower();
        //    if (file.Contains(".csv"))
        //    {
        //        try
        //        {
        //            using (new WaitCursor())
        //            {
        //                List<string[]> parsedData = new List<string[]>();
        //                using (System.IO.StreamReader readFile = new System.IO.StreamReader(file))
        //                {
        //                    string line;
        //                    string[] row;

        //                    while ((line = readFile.ReadLine()) != null)
        //                    {
        //                        row = line.Split(',');
        //                        parsedData.Add(row);
        //                    }
        //                }
        //                DriverSerialExampleDynTagSettings p = new DriverSerialExampleDynTagSettings();
        //                string dynamicaddress;
        //                int MovType;
        //                uint varsize;
        //                foreach (var s in parsedData)
        //                {
        //                    MovType = -1;
        //                    dynamicaddress = string.Empty;
        //                    varsize = 0;
        //                    if ((s.Length > 1) && !s[0].StartsWith("//") && p.ParseAddress(s[1].Trim()))
        //                    {
        //                        p.StationName = CmbStation.Text;
        //                        dynamicaddress = p.ToString();

        //                        if (s.Length > 2)
        //                        {
        //                            MovType = GetMoviconTypeId(s[2], ref varsize);
        //                            if (MovType != -1)
        //                            {
        //                                var IVar = importDataModel.addImportData();
        //                                IVar.Name = s[0];
        //                                IVar.Address = s[1].Trim();
        //                                IVar.DynAddress = dynamicaddress;
        //                                IVar.TagType = MovType;
        //                                IVar.Select = false;
        //                                IVar.szType = s[2];
        //                                AddTreeItem(IVar);
        //                            }
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            throw;
        //        }
        //    }

        //    ImportTree.Model = importDataModel;

        //    if (listViewSortCol != null)
        //    {
        //        AdornerLayer.GetAdornerLayer(listViewSortCol).Remove(listViewSortAdorner);
        //        ImportTree.Items.SortDescriptions.Clear();
        //    }
        //}

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets movicon type identifier. </summary>
        ///
        /// <param name="Type" type="string">       The type. </param>
        /// <param name="VarSize" type="ref uint">  [in,out] Size of the variable. </param>
        ///
        /// <returns>   The movicon type identifier. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
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

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Add item to importDataModel. </summary>
        ///
        /// <param name="tag">      . </param>
        /// <param name="parent">   (Optional) </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        internal void AddTreeItem(ImportData tag, ImportData parent = null)
        {
            tag.Parent = parent;
            if (parent == null)
            {
                tag.TreeLevel = 0xFF;
                string strLevel = tag.TreeLevel.ToString("X2");
                tag.parentId = -1;
                //tag.SortName = tag.Name;
                //tag.SortAddress = tag.Address;
                //tag.SortType = tag.szType;
                //tag.SortNameR = tag.Name + strLevel;
                //tag.SortAddressR = tag.Address + strLevel;
                //tag.SortTypeR = tag.szType + tag.SortNameR;
                importDataModel.Children.Add(tag);
            }
            else
            {
                tag.TreeLevel = parent.TreeLevel - 1;
                string strLevel = tag.TreeLevel.ToString("X2");
                tag.parentId = parent.Id;
                //tag.SortName = parent.SortName + tag.Name;
                //tag.SortAddress = parent.SortAddress + tag.Address;
                //tag.SortType = parent.SortType + tag.szType;
                //tag.SortNameR = parent.SortNameR.Substring(0, parent.SortNameR.Length - 2) + strLevel + tag.Name + strLevel;
                //tag.SortAddressR = parent.SortAddressR.Substring(0, parent.SortAddressR.Length - 2) + strLevel + tag.Address + strLevel;
                //tag.SortTypeR = parent.SortTypeR.Substring(0, parent.SortTypeR.Length - 2) + strLevel + tag.szType + tag.SortNameR;
                parent.Children.Add(tag);
            }
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

            //if (idl != null)
            //    idl.Dispose();
            using (new WaitCursor())
            {
                if (importDataModel != null)
                {
                    //importDataModel.Dispose();
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

    /// <summary>   Sort Adorner. </summary>
    //public class SortAdorner : Adorner
    //{
    //    /// <summary>   The ascending geometry. </summary>
    //    private static Geometry ascGeometry =
    //            Geometry.Parse("M 0 4 L 3.5 0 L 7 4 Z");

    //    /// <summary>   The descending geometry. </summary>
    //    private static Geometry descGeometry =
    //            Geometry.Parse("M 0 0 L 3.5 4 L 7 0 Z");

    //    ////////////////////////////////////////////////////////////////////////////////////////////////////
    //    /// <summary>   Gets or sets the direction. </summary>
    //    ///
    //    /// <value> The direction. </value>
    //    ////////////////////////////////////////////////////////////////////////////////////////////////////
    //    public ListSortDirection Direction { get; private set; }

    //    ////////////////////////////////////////////////////////////////////////////////////////////////////
    //    /// <summary>   Constructor. </summary>
    //    ///
    //    /// <param name="element" type="UIElement">     The element. </param>
    //    /// <param name="dir" type="ListSortDirection"> The dir. </param>
    //    ////////////////////////////////////////////////////////////////////////////////////////////////////
    //    public SortAdorner(UIElement element, ListSortDirection dir)
    //        : base(element)
    //    {
    //        this.Direction = dir;
    //    }

    //    ////////////////////////////////////////////////////////////////////////////////////////////////////
    //    /// <summary>
    //    /// When overridden in a derived class, participates in rendering operations that are directed by
    //    /// the layout system. The rendering instructions for this element are not used directly when
    //    /// this method is invoked, and are instead preserved for later asynchronous use by layout and
    //    /// drawing.
    //    /// </summary>
    //    ///
    //    /// <param name="drawingContext">   The drawing instructions for a specific element. This context
    //    ///                                 is provided to the layout system. </param>
    //    ////////////////////////////////////////////////////////////////////////////////////////////////////
    //    protected override void OnRender(DrawingContext drawingContext)
    //    {
    //        base.OnRender(drawingContext);

    //        if (AdornedElement.RenderSize.Width < 20)
    //            return;

    //        TranslateTransform transform = new TranslateTransform
    //                (
    //                        AdornedElement.RenderSize.Width - 15,
    //                        (AdornedElement.RenderSize.Height - 5) / 2
    //                );
    //        drawingContext.PushTransform(transform);

    //        Geometry geometry = ascGeometry;
    //        if (this.Direction == ListSortDirection.Descending)
    //            geometry = descGeometry;
    //        drawingContext.DrawGeometry(Brushes.Black, null, geometry);

    //        drawingContext.Pop();
    //    }
    //}

    ///// <summary>   import data.   </summary>
    //public class ImportData
    //{
    //    /// <summary>   The children. </summary>
    //    private readonly ObservableCollection<ImportData> _children = new ObservableCollection<ImportData>();
    //    ////////////////////////////////////////////////////////////////////////////////////////////////////
    //    /// <summary>   Gets the children. </summary>
    //    ///
    //    /// <value> The children. </value>
    //    ////////////////////////////////////////////////////////////////////////////////////////////////////
    //    public ObservableCollection<ImportData> Children
    //    {
    //        get { return _children; }
    //    }

    //    ////////////////////////////////////////////////////////////////////////////////////////////////////
    //    /// <summary>   Returns a string that represents the current object. </summary>
    //    ///
    //    /// <returns>   A string that represents the current object. </returns>
    //    ////////////////////////////////////////////////////////////////////////////////////////////////////
    //    public override string ToString()
    //    {
    //        return Name;
    //    }

    //    ////////////////////////////////////////////////////////////////////////////////////////////////////
    //    /// <summary>   Constructor. </summary>
    //    ///
    //    /// <param name="inDataModel" type="ImportDataModel">   The in data model. </param>
    //    ////////////////////////////////////////////////////////////////////////////////////////////////////
    //    public ImportData(ImportDataModel inDataModel)
    //    {
    //        _Id = 0;
    //        dataModel = inDataModel;
    //    }
    //    ////////////////////////////////////////////////////////////////////////////////////////////////////
    //    /// <summary>   Gets or sets the data model. </summary>
    //    ///
    //    /// <value> The data model. </value>
    //    ////////////////////////////////////////////////////////////////////////////////////////////////////
    //    private ImportDataModel dataModel { get; set; }

    //    /// <summary>   The identifier. </summary>
    //    private int _Id;
    //    ////////////////////////////////////////////////////////////////////////////////////////////////////
    //    /// <summary>   Gets or sets the identifier. </summary>
    //    ///
    //    /// <value> The identifier. </value>
    //    ////////////////////////////////////////////////////////////////////////////////////////////////////
    //    public int Id
    //    {
    //        get { return _Id; }
    //        set { _Id = value; }
    //    }
    //    /// <summary>   Identifier for the parent. </summary>
    //    private int _parentId;
    //    ////////////////////////////////////////////////////////////////////////////////////////////////////
    //    /// <summary>   Gets or sets the identifier of the parent. </summary>
    //    ///
    //    /// <value> The identifier of the parent. </value>
    //    ////////////////////////////////////////////////////////////////////////////////////////////////////
    //    public int parentId
    //    {
    //        get { return _parentId; }
    //        set { _parentId = value; }
    //    }
    //    /// <summary>   The name. </summary>
    //    private string _Name;
    //    ////////////////////////////////////////////////////////////////////////////////////////////////////
    //    /// <summary>   Gets or sets the name. </summary>
    //    ///
    //    /// <value> The name. </value>
    //    ////////////////////////////////////////////////////////////////////////////////////////////////////
    //    public string Name
    //    {
    //        get
    //        {
    //            return dataModel.getStationName() != "_" ?
    //                   dataModel.getStationName() + _Name : _Name;
    //        }
    //        set { _Name = value; }
    //    }
    //    /// <summary>   The address. </summary>
    //    private string _Address;
    //    ////////////////////////////////////////////////////////////////////////////////////////////////////
    //    /// <summary>   Gets or sets the address. </summary>
    //    ///
    //    /// <value> The address. </value>
    //    ////////////////////////////////////////////////////////////////////////////////////////////////////
    //    public string Address
    //    {
    //        get { return _Address; }
    //        set { _Address = value; }
    //    }
    //    /// <summary>   true to select, false to deselect. </summary>
    //    private bool _Select;
    //    ////////////////////////////////////////////////////////////////////////////////////////////////////
    //    /// <summary>   Gets or sets a value indicating whether the select. </summary>
    //    ///
    //    /// <value> true if select, false if not. </value>
    //    ////////////////////////////////////////////////////////////////////////////////////////////////////
    //    public bool Select
    //    {
    //        get { return _Select; }
    //        set { _Select = value; }
    //    }
    //    /// <summary>   The dynamic address. </summary>
    //    private string _DynAddress;
    //    ////////////////////////////////////////////////////////////////////////////////////////////////////
    //    /// <summary>   Gets or sets the dynamic address. </summary>
    //    ///
    //    /// <value> The dynamic address. </value>
    //    ////////////////////////////////////////////////////////////////////////////////////////////////////
    //    public string DynAddress
    //    {
    //        get { return _DynAddress; }
    //        set { _DynAddress = value; }
    //    }
    //    /// <summary>   Type of the tag. </summary>
    //    private int _TagType;
    //    ////////////////////////////////////////////////////////////////////////////////////////////////////
    //    /// <summary>   Gets or sets the type of the tag. </summary>
    //    ///
    //    /// <value> The type of the tag. </value>
    //    ////////////////////////////////////////////////////////////////////////////////////////////////////
    //    public int TagType
    //    {
    //        get { return _TagType; }
    //        set { _TagType = value; }
    //    }
    //    /// <summary>   The type. </summary>
    //    private string _szType;
    //    ////////////////////////////////////////////////////////////////////////////////////////////////////
    //    /// <summary>   Gets or sets the type. </summary>
    //    ///
    //    /// <value> The size type. </value>
    //    ////////////////////////////////////////////////////////////////////////////////////////////////////
    //    public string szType
    //    {
    //        get { return _szType; }
    //        set { _szType = value; }
    //    }
    //    /// <summary>   The parent. </summary>
    //    private ImportData _Parent;
    //    ////////////////////////////////////////////////////////////////////////////////////////////////////
    //    /// <summary>   Gets or sets the parent. </summary>
    //    ///
    //    /// <value> The parent. </value>
    //    ////////////////////////////////////////////////////////////////////////////////////////////////////
    //    public ImportData Parent
    //    {
    //        get { return _Parent; }
    //        set { _Parent = value; }
    //    }
    //    /// <summary>   The tree level. </summary>
    //    private uint _TreeLevel;
    //    ////////////////////////////////////////////////////////////////////////////////////////////////////
    //    /// <summary>   Gets or sets the tree level. </summary>
    //    ///
    //    /// <value> The tree level. </value>
    //    ////////////////////////////////////////////////////////////////////////////////////////////////////
    //    public uint TreeLevel
    //    {
    //        get { return _TreeLevel; }
    //        set { _TreeLevel = value; }
    //    }

    //    /// <summary>   Type of the sort. </summary>
    //    private string _SortType;
    //    ////////////////////////////////////////////////////////////////////////////////////////////////////
    //    /// <summary>   Gets or sets the type of the sort. </summary>
    //    ///
    //    /// <value> The type of the sort. </value>
    //    ////////////////////////////////////////////////////////////////////////////////////////////////////
    //    public string SortType
    //    {
    //        get { return _SortType; }
    //        set { _SortType = value; }
    //    }
    //    /// <summary>   The sort address. </summary>
    //    private string _SortAddress;
    //    ////////////////////////////////////////////////////////////////////////////////////////////////////
    //    /// <summary>   Gets or sets the sort address. </summary>
    //    ///
    //    /// <value> The sort address. </value>
    //    ////////////////////////////////////////////////////////////////////////////////////////////////////
    //    public string SortAddress
    //    {
    //        get { return _SortAddress; }
    //        set { _SortAddress = value; }
    //    }
    //    /// <summary>   Name of the sort. </summary>
    //    private string _SortName;
    //    ////////////////////////////////////////////////////////////////////////////////////////////////////
    //    /// <summary>   Gets or sets the name of the sort. </summary>
    //    ///
    //    /// <value> The name of the sort. </value>
    //    ////////////////////////////////////////////////////////////////////////////////////////////////////
    //    public string SortName
    //    {
    //        get { return _SortName; }
    //        set { _SortName = value; }
    //    }
    //    /// <summary>   The sort type r. </summary>
    //    private string _SortTypeR;
    //    ////////////////////////////////////////////////////////////////////////////////////////////////////
    //    /// <summary>   Gets or sets the sort type r. </summary>
    //    ///
    //    /// <value> The sort type r. </value>
    //    ////////////////////////////////////////////////////////////////////////////////////////////////////
    //    public string SortTypeR
    //    {
    //        get { return _SortTypeR; }
    //        set { _SortTypeR = value; }
    //    }
    //    /// <summary>   The sort address r. </summary>
    //    private string _SortAddressR;
    //    ////////////////////////////////////////////////////////////////////////////////////////////////////
    //    /// <summary>   Gets or sets the sort address r. </summary>
    //    ///
    //    /// <value> The sort address r. </value>
    //    ////////////////////////////////////////////////////////////////////////////////////////////////////
    //    public string SortAddressR
    //    {
    //        get { return _SortAddressR; }
    //        set { _SortAddressR = value; }
    //    }
    //    /// <summary>   The sort name r. </summary>
    //    private string _SortNameR;
    //    ////////////////////////////////////////////////////////////////////////////////////////////////////
    //    /// <summary>   Gets or sets the sort name r. </summary>
    //    ///
    //    /// <value> The sort name r. </value>
    //    ////////////////////////////////////////////////////////////////////////////////////////////////////
    //    public string SortNameR
    //    {
    //        get { return _SortNameR; }
    //        set { _SortNameR = value; }
    //    }
       
    //    ////////////////////////////////////////////////////////////////////////////////////////////////////
    //    /// <summary>   Gets the name of the tree. </summary>
    //    ///
    //    /// <value> The name of the tree. </value>
    //    ////////////////////////////////////////////////////////////////////////////////////////////////////
    //    public string TreeName
    //    {
    //        get { return getTreeName(); }
    //    }

    //    ////////////////////////////////////////////////////////////////////////////////////////////////////
    //    /// <summary>   Gets tree name. </summary>
    //    ///
    //    /// <returns>   The tree name. </returns>
    //    ////////////////////////////////////////////////////////////////////////////////////////////////////
    //    private string getTreeName()
    //    {
    //        string treeName = Name;
    //        if(Parent != null)
    //        {
    //            treeName = Parent.getTreeName() + "." + treeName;
    //        }
    //        return treeName;
    //    }
    //};

    ///// <summary>   Import Data Model. </summary>
    //public class ImportDataModel : ITreeModel
    //{
    //    ////////////////////////////////////////////////////////////////////////////////////////////////////
    //    /// <summary>   Gets or sets the children. </summary>
    //    ///
    //    /// <value> The children. </value>
    //    ////////////////////////////////////////////////////////////////////////////////////////////////////
    //    public ObservableCollection<ImportData> Children { get; private set; }
    //    ////////////////////////////////////////////////////////////////////////////////////////////////////
    //    /// <summary>   Gets or sets the name of the get station. </summary>
    //    ///
    //    /// <value> The name of the get station. </value>
    //    ////////////////////////////////////////////////////////////////////////////////////////////////////
    //    public GetStationName getStationName { get; private set; }

    //    ////////////////////////////////////////////////////////////////////////////////////////////////////
    //    /// <summary>   Constructor. </summary>
    //    ///
    //    /// <param name="inGetStationName" type="GetStationName">   Name of the in get station. </param>
    //    ////////////////////////////////////////////////////////////////////////////////////////////////////
    //    public ImportDataModel(GetStationName inGetStationName)
    //    {
    //        Children = new ObservableCollection<ImportData>();
    //        getStationName = inGetStationName;
    //    }

    //    ////////////////////////////////////////////////////////////////////////////////////////////////////
    //    /// <summary>   Gets the children of this item. </summary>
    //    ///
    //    /// <param name="parent" type="object"> The parent. </param>
    //    ///
    //    /// <returns>   The children. </returns>
    //    ////////////////////////////////////////////////////////////////////////////////////////////////////
    //    public System.Collections.IEnumerable GetChildren(object parent)
    //    {
    //        if (parent == null)
    //            return Children;
    //        return (parent as ImportData).Children;
    //    }

    //    ////////////////////////////////////////////////////////////////////////////////////////////////////
    //    /// <summary>   Query if 'parent' has children. </summary>
    //    ///
    //    /// <param name="parent" type="object"> The parent. </param>
    //    ///
    //    /// <returns>   true if children, false if not. </returns>
    //    ////////////////////////////////////////////////////////////////////////////////////////////////////
    //    public bool HasChildren(object parent)
    //    {
    //        return (parent as ImportData).Children.Count > 0;
    //    }

    //    ////////////////////////////////////////////////////////////////////////////////////////////////////
    //    /// <summary>   Adds import data. </summary>
    //    ///
    //    /// <returns>   An ImportData. </returns>
    //    ////////////////////////////////////////////////////////////////////////////////////////////////////
    //    public ImportData addImportData()
    //    {
    //        ImportData importData = new ImportData(this);
    //        return importData;
    //    }
    //}

    public class ImportDataDriverSerialExample : ImportData, IDisposable
    {
        public ImportDataDriverSerialExample(ImportDataModel inDataModel) :
            base(inDataModel)
        {
            dataModelDriverSerialExample = inDataModel as ImportDataModelDriverSerialExample;
        }
        private ImportDataModelDriverSerialExample dataModelDriverSerialExample { get; set; }

        //    private int _Id;
        //    public int Id
        //    {
        //        get { return _Id; }
        //        set { _Id = value; }
        //    }
        //    private int _parentId;
        //    public int parentId
        //    {
        //        get { return _parentId; }
        //        set { _parentId = value; }
        //    }
        //    private string _Name;
        //    public string Name
        //    {
        //        get
        //        {
        //            return dataModel.getStationName() != "_" ?
        //                   dataModel.getStationName() + _Name : _Name;
        //        }
        //        set { _Name = value; }
        //    }
        //    private string _Address;
        //    public string Address
        //    {
        //        get { return _Address; }
        //        set { _Address = value; }
        //    }
        //    private bool _Select;
        //    public bool Select
        //    {
        //        get { return _Select; }
        //        set { _Select = value; }
        //    }
        //    private string _DynAddress;
        //    public string DynAddress
        //    {
        //        get { return _DynAddress; }
        //        set { _DynAddress = value; }
        //    }
        private int _TagTypeInt;
        public int TagTypeInt
        {
            get { return _TagTypeInt; }
            set { _TagTypeInt = value; }
        }
        //    private string _szType;
        //    public string szType
        //    {
        //        get { return _szType; }
        //        set { _szType = value; }
        //    }
        //    private ImportData _Parent;
        //    public ImportData Parent
        //    {
        //        get { return _Parent; }
        //        set { _Parent = value; }
        //    }
        //    private uint _TreeLevel;
        //    public uint TreeLevel
        //    {
        //        get { return _TreeLevel; }
        //        set { _TreeLevel = value; }
        //    }

        //    private string _SortType;
        //    public string SortType
        //    {
        //        get { return _SortType; }
        //        set { _SortType = value; }
        //    }
        //    private string _SortAddress;
        //    public string SortAddress
        //    {
        //        get { return _SortAddress; }
        //        set { _SortAddress = value; }
        //    }
        //    private string _SortName;
        //    public string SortName
        //    {
        //        get { return _SortName; }
        //        set { _SortName = value; }
        //    }
        //    private string _SortTypeR;
        //    public string SortTypeR
        //    {
        //        get { return _SortTypeR; }
        //        set { _SortTypeR = value; }
        //    }
        //    private string _SortAddressR;
        //    public string SortAddressR
        //    {
        //        get { return _SortAddressR; }
        //        set { _SortAddressR = value; }
        //    }
        //    private string _SortNameR;
        //    public string SortNameR
        //    {
        //        get { return _SortNameR; }
        //        set { _SortNameR = value; }
        //    }

        //    public string TreeName
        //    {
        //        get { return getTreeName(); }
        //    }

        //    private string getTreeName()
        //    {
        //        string treeName = Name;
        //        if(Parent != null)
        //        {
        //            treeName = Parent.getTreeName() + "." + treeName;
        //        }
        //        return treeName;
        //    }

        public override DataType IconTagType
        {
            get
            {
                if (_TagTypeInt == -1)
                    return DataType.Boolean;
                else
                    return (DataType)_TagTypeInt;
            }
        }

        #region IDisposable Members
        public void Dispose()
        {
            if (Children != null && Children.Count > 0)
            {
                foreach (ImportData el in Children)
                {
                    var sl = el as ImportDataDriverSerialExample;
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

    public class ImportDataModelDriverSerialExample : ImportDataModel, IDisposable
    {
        public ImportDataModelDriverSerialExample(GetStationName inGetStationName) :
            base(inGetStationName)
        {
        }
        public override ImportData addImportData()
        {
            ImportDataDriverSerialExample importData = new ImportDataDriverSerialExample(this);
            return importData;
        }

        #region IDisposable Members
        public void Dispose()
        {
            foreach (ImportData el in Children)
            {
                ImportDataDriverSerialExample els7 = el as ImportDataDriverSerialExample;
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
