#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Syncfusion.XlsIO;
using System.IO;
using Syncfusion.Windows.Controls.Grid;
using Syncfusion.Windows.Controls.Grid.Converter;
using Syncfusion.Windows.Tools.Controls;
using Syncfusion.Windows.Controls;
using Syncfusion.Windows.Controls.Spreadsheet.Commands;
using Syncfusion.Windows.Controls.Spreadsheet.Resources;
using Microsoft.Win32;
using Syncfusion.Windows.Shared;
using Syncfusion.Compression.Zip;
using System.Xml.Linq;
using System.Windows.Controls.Primitives;
using Syncfusion.Windows.Controls.Scroll;

namespace Syncfusion.Windows.Controls.Spreadsheet
{
    public partial class SpreadsheetControl
    {
        #region ImportExtensions

        public ImportFromExcelCommand ImportFromExcelCommand
        {
            get { return (ImportFromExcelCommand)GetValue(ImportFromExcelCommandProperty); }
            set { SetValue(ImportFromExcelCommandProperty, value); }
        }

        public static readonly DependencyProperty ImportFromExcelCommandProperty =
            DependencyProperty.Register("ImportFromExcelCommand", typeof(ImportFromExcelCommand), typeof(SpreadsheetControl), new PropertyMetadata(null));

        public bool ImportFromExcel(string filename)
        {
            ExcelProperties.SetExcelEngine(filename);
            ExcelProperties.VisibleSheetCount = 0;
            ExcelProperties.FileName = filename;
            if (this.IsLoaded)
                LoadTabItems();
            else
                loadTabItemsOnLoad = true;
            return true;
        }

        internal void ImportExcel(Stream stream)
        {
            DisposeOldInstances();
            
#if !SILVERLIGHT
            ExcelProperties.SetExcelEngine(stream, string.Empty);
            ExcelProperties.spreadControl = this;
#else
            ExcelProperties.SetExcelEngine(stream, string.Empty, this);
#endif
            ExcelProperties.VisibleSheetCount = 0;
            if (this.IsLoaded)
                LoadTabItems();
            else
                loadTabItemsOnLoad = true;
        }

        public bool ImportFromExcel(Stream stream)
        {
            DisposeOldInstances();
#if !SILVERLIGHT
            ExcelProperties.SetExcelEngine(stream, string.Empty);
            ExcelProperties.spreadControl = this;
#else
            ExcelProperties.SetExcelEngine(stream, string.Empty, this);
#endif
            ExcelProperties.VisibleSheetCount = 0;
            ExcelProperties.FileName = string.Empty;
            if (this.IsLoaded)
                LoadTabItems();
            else
                loadTabItemsOnLoad = true;
            return true;
        }

        public bool ImportFromExcel(Stream stream,ExcelOpenType openType)
        {
            if (this.ExcelProperties != null && this.ExcelProperties.WorkBook != null)
            {
                if (this.ExcelProperties.Application.Workbooks.Count > 0)
                    this.ExcelProperties.Application.Workbooks.Close();
                this.ExcelProperties.WorkBook = this.ExcelProperties.Application.Workbooks.Open(stream, openType);

                if (this.ExcelProperties.WorkBook.Version != ExcelVersion.Excel97to2003)
                {
                    this.ExcelProperties.ChangedXMLCellList = new XElement("ChangedCells");
                    this.ExcelProperties.ChangedCellList = CurrentSpreadsheetGrid.ReadXMLFile(stream);
                }

                if (this.ExcelProperties.ChangedCellList == null)
                    this.ExcelProperties.ChangedCellList = new BinaryList();
                else
                    CurrentSpreadsheetGrid.CreateXElement();

                ExcelProperties.FileName = string.Empty;
                ExcelProperties.spreadControl = this;
                if (this.IsLoaded)
                    LoadTabItems();
                else
                    loadTabItemsOnLoad = true;
                return true;
            }
            else return false;
        }

        public bool ImportFromExcel(Stream stream,ExcelVersion version)
        {
            if (this.ExcelProperties != null && this.ExcelProperties.WorkBook != null)
            {
                if (this.ExcelProperties.Application.Workbooks.Count > 0)
                    this.ExcelProperties.Application.Workbooks.Close();
                this.ExcelProperties.WorkBook = this.ExcelProperties.Application.Workbooks.Open(stream, version);
                ExcelProperties.FileName = string.Empty;
                if (this.IsLoaded)
                    LoadTabItems();
                else
                    loadTabItemsOnLoad = true;
                return true;
            }
            else return false;
        }

        public bool ImportFromExcel(string filename, ExcelOpenType openType)
        {
            if (this.ExcelProperties != null && this.ExcelProperties.WorkBook != null)
            {
                if (this.ExcelProperties.Application.Workbooks.Count > 0)
                    this.ExcelProperties.Application.Workbooks.Close();
                this.ExcelProperties.WorkBook = this.ExcelProperties.Application.Workbooks.Open(filename, openType);
                ExcelProperties.FileName = filename;
                return true;
                if (this.IsLoaded)
                    LoadTabItems();
                else
                    loadTabItemsOnLoad = true;
            }
            else return false;
        }

        public bool ImportFromExcel(string filename, ExcelVersion version)
        {
            if (this.ExcelProperties != null && this.ExcelProperties.WorkBook != null)
            {
                if (this.ExcelProperties.Application.Workbooks.Count > 0)
                    this.ExcelProperties.Application.Workbooks.Close();
                this.ExcelProperties.WorkBook = this.ExcelProperties.Application.Workbooks.Open(filename, version);
                ExcelProperties.FileName = filename;
                if (this.IsLoaded)
                    LoadTabItems();
                else
                    loadTabItemsOnLoad = true;
                return true;
            }
            else return false;
        }

        public bool ImportFromExcel(string filename,ExcelOpenType openType, ExcelVersion version)
        {
            if (this.ExcelProperties != null && this.ExcelProperties.WorkBook != null)
            {
                if (this.ExcelProperties.Application.Workbooks.Count > 0)
                    this.ExcelProperties.Application.Workbooks.Close();
                this.ExcelProperties.WorkBook = this.ExcelProperties.Application.Workbooks.Open(filename, openType, version);
                ExcelProperties.FileName = filename;
                if (this.IsLoaded)
                    LoadTabItems();
                else
                    loadTabItemsOnLoad = true;
                return true;
            }
            else return false;
        }


        public bool ImportFromExcel(Stream stream, ExcelOpenType openType, ExcelVersion version)
        {
            if (this.ExcelProperties != null && this.ExcelProperties.WorkBook != null)
            {
                if (this.ExcelProperties.Application.Workbooks.Count > 0)
                    this.ExcelProperties.Application.Workbooks.Close();
                this.ExcelProperties.WorkBook = this.ExcelProperties.Application.Workbooks.Open(stream, openType, version);
                ExcelProperties.FileName = string.Empty;
                if (this.IsLoaded)
                    LoadTabItems();
                else
                    loadTabItemsOnLoad = true;
                return true;
            }
            else return false;
        }

        /// <summary>
        /// To import the workbook to the Spreadsheet control
        /// </summary>
        /// <param name="workbook">Workbook</param>
        public bool ImportFromExcel(IWorkbook workbook)
        {
            if (this.ExcelProperties != null && this.ExcelProperties.WorkBook != null)
            {
                DisposeOldInstances();
                this.ExcelProperties.WorkBook = workbook.Clone();
                ExcelProperties.FileName = string.Empty;
                //To load the work book at run time
                if (this.IsLoaded)
                    LoadTabItems();
                else
                    loadTabItemsOnLoad = true;
                return true;
            }
            else return false;
        }

        #endregion

        #region LoadExcel

        internal Dictionary<string, SpreadsheetGrid> GridCollection;
        internal SpreadsheetGridModel[] ModelCollection;
        bool onLoadTabItems = false;
        private void LoadTabItems()
        {
            loadTabItemsOnLoad = false;
#if SILVERLIGHT
            if (!System.ComponentModel.DesignerProperties.IsInDesignTool)
            {
#endif
            if (ExcelProperties.WorkBook != null && ExcelProperties.WorkBook.TabSheets.Count > 0 && _excelTabControlAdv != null)
            {
                GridProperties.UnWireGridEvents(this.CurrentSpreadsheetGrid);
                onLoadTabItems = true;
                UnregisterSheetFromFormulaEngine();
                _excelTabControlAdv.Items.Clear();
                ExcelProperties.VisibleSheetCount = 0;
                GridCollection.Clear();
                
                ModelCollection = ExcelGridModelImportExtensions.ImportFromExcelToVirtualGrid(ExcelProperties.WorkBook);
                for (int i = 0; i < ExcelProperties.WorkBook.TabSheets.Count; i++)
                {
                    if (ExcelProperties.ExtendRowAndColumn)
                    {
                        if (ModelCollection[i].RowCount < ExcelProperties.DefaultRowCount)
                            ModelCollection[i].RowCount = ExcelProperties.DefaultRowCount;
                        if (ModelCollection[i].ColumnCount < ExcelProperties.DefaultColumnCount)
                            ModelCollection[i].ColumnCount = ExcelProperties.DefaultColumnCount;
                    }
#if !SILVERLIGHT
                    TabItemExt tab = new TabItemExt
                    {
                        Header = ExcelProperties.WorkBook.TabSheets[i].Name,
                        HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch,
                        VerticalAlignment = System.Windows.VerticalAlignment.Stretch,
                        HorizontalContentAlignment = System.Windows.HorizontalAlignment.Stretch,
                        VerticalContentAlignment = System.Windows.VerticalAlignment.Stretch
                    };
#else
                    TabItemAdv tab = new TabItemAdv
                                           {
                                               Header = this.ExcelProperties.WorkBook.TabSheets[i].Name,
                                               HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch,
                                               VerticalAlignment = System.Windows.VerticalAlignment.Stretch,
                                               HorizontalContentAlignment = System.Windows.HorizontalAlignment.Stretch,
                                               VerticalContentAlignment = System.Windows.VerticalAlignment.Stretch
                                           };
#endif
                    SpreadsheetControl.SetSheetName(tab, this.ExcelProperties.WorkBook.TabSheets[i].Name);
                    SpreadsheetGrid grid = new SpreadsheetGrid();
#if !SILVERLIGHT
                    ScrollViewer sv = new ScrollViewer
#else
                    ScrollableContentViewer sv = new ScrollableContentViewer
#endif
                                          {
                                              CanContentScroll = true,
                                              HorizontalScrollBarVisibility = ScrollBarVisibility.Auto,
                                              VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
                                              Content = grid
                                          };
                    //ContextMenu
#if !SILVERLIGHT

                    sv.Loaded += new RoutedEventHandler(sv_Loaded);

                    if (this.TabStyleManager != null && this.TabStyleManager.ShowTabItemContextMenu)
                    {
                        if (this.TabStyleManager.IsCustomTabItemContextMenuEnabled)
                            tab.ContextMenu = this.TabStyleManager.TabItemContextMenu;
                        else
                            tab.ContextMenu = GetDefaultContextMenu(ExcelProperties.WorkBook.TabSheets[i].Name, ExcelProperties.WorkBook.TabSheets[i].IsPasswordProtected);
                        tab.ContextMenuOpening += new ContextMenuEventHandler(tab_ContextMenuOpening);
                    }
#else
                    sv.Loaded += new RoutedEventHandler(sv_Loaded);

                    if (this.TabStyleManager != null && this.TabStyleManager.ShowTabItemContextMenu)
                    {
                        if (this.TabStyleManager.IsCustomTabItemContextMenuEnabled)
                            ContextMenuAdvService.SetContextMenuAdv(tab, this.TabStyleManager.TabItemContextMenu);
                        else
                            ContextMenuAdvService.SetContextMenuAdv(tab, GetDefaultContextMenu(ExcelProperties.WorkBook.TabSheets[i].Name, ExcelProperties.WorkBook.TabSheets[i].IsPasswordProtected));
                    }
#endif
                    
                    //Grid settings
                    grid.SheetName = ExcelProperties.WorkBook.TabSheets[i].Name;
                    grid.ExcelProperties = ExcelProperties;
                    ModelCollection[i].SperadsheetGrid = grid;
                    grid.Model = ModelCollection[i];

                    if (i < ExcelProperties.WorkBook.Worksheets.Count)
                    {
                        grid.MouseControllerDispatcher.Add(new GridExcelMarkerMouseController(grid, ExcelProperties.WorkBook.Worksheets[i]));
                        grid.Model.GridCopyPaste = new SpreadsheetGridCopyPaste(grid, ExcelProperties.WorkBook.Worksheets[i]);
                        grid.ShowGridLines = ExcelProperties.WorkBook.Worksheets[i].IsGridLinesVisible;
                        grid.IsRowHeaderVisible = ExcelProperties.WorkBook.Worksheets[i].IsRowColumnHeadersVisible;
                        grid.IsColumnHeadersVisible = ExcelProperties.WorkBook.Worksheets[i].IsRowColumnHeadersVisible;
                    }
                    InitializeSpreadsheetGrid(grid);
                    string tabsheetName = SpreadsheetControl.GetSheetName(tab);
                    GridCollection.Add(tabsheetName, grid);
                    tab.Content = sv;

                    _excelTabControlAdv.Items.Add(tab);
                    if (ExcelProperties.WorkBook.TabSheets[i].Visibility != WorksheetVisibility.Visible)
#if !SILVERLIGHT
                        ((TabItemExt)_excelTabControlAdv.Items[i]).Visibility = Visibility.Collapsed;
#else
                        ((TabItemAdv)_excelTabControlAdv.Items[i]).Visibility = Visibility.Collapsed;
#endif
                    else if (!ExcelProperties.ExtendRowAndColumn && ExcelProperties.WorkBook.Worksheets[i].UsedRange.LastRow == 0 &&
                        ExcelProperties.WorkBook.Worksheets[i].UsedRange.LastColumn == 0)
                    {
#if !SILVERLIGHT
                        ((TabItemExt)_excelTabControlAdv.Items[i]).Visibility = Visibility.Collapsed;
#else
                        ((TabItemAdv)_excelTabControlAdv.Items[i]).Visibility = Visibility.Collapsed;
#endif
                    }
                    else
                        ExcelProperties.VisibleSheetCount += 1;

                }
                onLoadTabItems = false;
                string name;
                if (ExcelProperties.WorkBook.ActiveSheet != null)
                    name = ExcelProperties.WorkBook.ActiveSheet.Name;
                else
                    name = ExcelProperties.WorkBook.Charts[0].Name;
                this.SetCurrentGridChanged(name);

                //Raise WorkbookLoaded Event
                RaiseWorkBookLoaded();
            }
#if SILVERLIGHT
            }
#endif
        }

#if !SILVERLIGHT
        /// <summary>
        /// Handel the tab context menu when it open inside the grid control
        /// </summary>
        /// <param name="sender">Tab Item</param>
        /// <param name="e">ContextMenuEventArgs</param>
        void tab_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            if (this.GridProperties.SpreadsheetGrid != null)
            {
                var point = System.Windows.Input.Mouse.GetPosition(this.GridProperties.SpreadsheetGrid);
                var rowcol = this.GridProperties.SpreadsheetGrid.PointToCellRowColumnIndexOutsideCells(point, false);
                if (!rowcol.IsEmpty)
                    e.Handled = true;
            }
        }
#endif
        /// <summary>
        /// This will optimize the Loading time of the tab
        /// </summary>
        /// <param name="sender">GridModel</param>
        /// <param name="e">GridQueryDependentCellValueEventArgs</param>
        void FormulaEngine_QueryDependentCellValue(object sender, GridQueryDependentCellValueEventArgs e)
        {
            SpreadsheetGridModel gridModel = sender as SpreadsheetGridModel;
            var result = ModelCollection.Where(x => x == gridModel).FirstOrDefault();
            if (this.ExcelProperties.WorkBook != null && result != null)
            {
                IWorksheet sheet = this.ExcelProperties.WorkBook.Worksheets[result.SheetName];
                if (sheet != null)
                {
                    var range = sheet.Range[e.RowIndex, e.ColumnIndex];
                    if (!range.HasFormula)
                    {
                        string cellvalue = string.Empty;
                        if (range.HasNumber)
                        {
                            cellvalue = range.Number.ToString();
                        }
                        else
                        {
                            cellvalue = range.DisplayText.TrimStart();
                            cellvalue = cellvalue.TrimEnd();
                            if (string.IsNullOrEmpty(cellvalue) || string.IsNullOrEmpty(cellvalue))
                            {
                                cellvalue = "";
                            }
                            else
                            {
                                if (!range.NumberFormat.Equals("General"))
                                {
                                    var format = GetFormat(range.NumberFormat);
                                    var zeroFormatedText = 0.ToString(format);
                                    if (cellvalue.Trim() == zeroFormatedText.Trim() || cellvalue == string.Empty)
                                        cellvalue = "0";
                                }
                            }
                        }
                        e.CellValue = cellvalue;
                    }
                    else
                    {
                        if (CurrentSpreadsheetGrid != null && gridModel != CurrentSpreadsheetGrid.Model)
                        {
                            GridStyleInfo style = new GridStyleInfo();
                            string text = range.Value;
                            style.Text = text.Replace("'", "");
                            gridModel.FormulaEngine.FormulaContextCell = GridRangeInfo.GetAlphaLabel(e.ColumnIndex) + e.RowIndex.ToString();
                            style.FormulaTag = new GridFormulaTag(gridModel.FormulaEngine.Parse(style.Text), null, e.RowIndex, e.ColumnIndex);
                            style.Description = "IsFormulaOnlyImported";
                            gridModel.Data[e.RowIndex, e.ColumnIndex] = style.Store;
                        }
                        e.Cancel = true;
                    }
                }
            }
            else
                e.Cancel = true;
        }

        //To get the Zero Format
        private static string GetFormat(string format)
        {
            if (!format.Equals("General"))
            {
                format = format.Replace("\\", "");
                format = format.Replace("_(", "");
                format = format.Replace("_)", "");
                format = format.Replace("*", "");
                format = format.Replace("?", "");
                format = format.Replace("@", "");
                int i = 3;
                string cellformat = string.Empty;
                var temp = format.Split(";".ToArray(), StringSplitOptions.RemoveEmptyEntries);
                if (temp.Count() > i)
                {
                    foreach (var item in temp)
                    {
                        if (i == 0)
                            break;
                        i--;
                        cellformat += (item + ';');
                    }
                    format = cellformat;
                }
                return format;
            }
            return string.Empty;
        }

#if !SILVERLIGHT
        internal ContextMenu GetDefaultContextMenu(string sheetName, bool IsPasswordProtected)
        {
            ContextMenu contextMenu = new ContextMenu();
#if ClientProfile
            ResourceDictionary resources = new ResourceDictionary()
            {
                Source = new Uri("/Syncfusion.Spreadsheet.WPF.ClientProfile;component/SpreadsheetRibbon/Themes/Generic.xaml", UriKind.RelativeOrAbsolute)
            };
#else
            ResourceDictionary resources = new ResourceDictionary()
            {
                Source = new Uri("/Syncfusion.Spreadsheet.Wpf;component/SpreadsheetRibbon/Themes/Generic.xaml", UriKind.RelativeOrAbsolute)
            };
#endif

            Image DeleteSheetIcon = resources["DeleteSheetContextMenuIcon"] as Image;
            Image InsertSheetIcon = resources["InserSheetContextMenuIcon"] as Image;
            Image ProtectsheetIcon = resources["ProtectsheetContextMenuIcon"] as Image;

            MenuItem insertMenuItem = new MenuItem() { Header = SpreadsheetResourceWrapper.Insert };
            insertMenuItem.CommandParameter = sheetName;
            insertMenuItem.Command = this.InsertSheetCommand;
            insertMenuItem.Icon = InsertSheetIcon;

            MenuItem deleteMenuItem = new MenuItem() { Header = SpreadsheetResourceWrapper.Delete };
            deleteMenuItem.CommandParameter = sheetName;
            deleteMenuItem.Command = this.DeleteSheetCommand;
            deleteMenuItem.Icon = DeleteSheetIcon;

            MenuItem protectMenuItem = new MenuItem();
            if (IsPasswordProtected)
                protectMenuItem.Header = string.Format("{0}...", SpreadsheetResourceWrapper.UnProtectSheet);
            else
                protectMenuItem.Header = string.Format("{0}...", SpreadsheetResourceWrapper.ProtectSheet);
            protectMenuItem.Icon = ProtectsheetIcon;
            protectMenuItem.CommandParameter = sheetName;
            protectMenuItem.Command = this.ProtectSheetCommand;

            MenuItem hideMenuItem = new MenuItem() { Header = SpreadsheetResourceWrapper.Hide };
            hideMenuItem.CommandParameter = sheetName;
            hideMenuItem.Command = this.HideSheetCommand;

            MenuItem unhideMenuItem = new MenuItem() { Header = SpreadsheetResourceWrapper.Unhide };
            unhideMenuItem.Command = this.UnHideSheetCommand;
            
            contextMenu.Items.Add(insertMenuItem);
            contextMenu.Items.Add(deleteMenuItem);
            contextMenu.Items.Add(protectMenuItem);
            contextMenu.Items.Add(new Separator());
            contextMenu.Items.Add(hideMenuItem);
            contextMenu.Items.Add(unhideMenuItem);
            return contextMenu;
        }
#else
        internal ContextMenuAdv GetDefaultContextMenu(string sheetName, bool IsPasswordProtected)
        {
            ContextMenuAdv contextMenu = new ContextMenuAdv();

            System.Windows.ResourceDictionary resources = new System.Windows.ResourceDictionary()
            {
                Source = new Uri("/Syncfusion.Spreadsheet.Silverlight;component/SpreadsheetRibbon/Themes/Generic.xaml", UriKind.RelativeOrAbsolute)
            };

            ImageSource DeleteSheetIconSource = resources["DeleteSheetContextMenuIcon"] as ImageSource;
            ImageSource InsertSheetIconSource = resources["InserSheetContextMenuIcon"] as ImageSource;
            ImageSource ProtectsheetIconSource = resources["ProtectsheetContextMenuIcon"] as ImageSource;

            Image DeleteSheetIcon = new Image() { Source = DeleteSheetIconSource };
            Image InsertSheetIcon = new Image() { Source = InsertSheetIconSource };
            Image ProtectsheetIcon = new Image() { Source = ProtectsheetIconSource };

            ContextMenuItemAdv insertMenuItem = new ContextMenuItemAdv() { Header = SpreadsheetResourceWrapper.Insert };
            insertMenuItem.CommandParameter = sheetName;
            insertMenuItem.Command = this.InsertSheetCommand;
            insertMenuItem.Icon = InsertSheetIcon;

            ContextMenuItemAdv deleteMenuItem = new ContextMenuItemAdv() { Header = SpreadsheetResourceWrapper.Delete };
            deleteMenuItem.CommandParameter = sheetName;
            deleteMenuItem.Command = this.DeleteSheetCommand;
            deleteMenuItem.Icon = DeleteSheetIcon;

            ContextMenuItemAdv protectMenuItem = new ContextMenuItemAdv();
            if (IsPasswordProtected)
                protectMenuItem.Header = string.Format("{0}...", SpreadsheetResourceWrapper.UnProtectSheet);
            else
                protectMenuItem.Header = string.Format("{0}...", SpreadsheetResourceWrapper.ProtectSheet);
            protectMenuItem.Icon = ProtectsheetIcon;
            protectMenuItem.CommandParameter = sheetName;
            protectMenuItem.Command = this.ProtectSheetCommand;

            ContextMenuItemAdv hideMenuItem = new ContextMenuItemAdv() { Header = SpreadsheetResourceWrapper.Hide };
            hideMenuItem.CommandParameter = sheetName;
            hideMenuItem.Command = this.HideSheetCommand;

            ContextMenuItemAdv unhideMenuItem = new ContextMenuItemAdv() { Header = SpreadsheetResourceWrapper.Unhide };
            unhideMenuItem.Command = this.UnHideSheetCommand;

            contextMenu.Items.Add(insertMenuItem);
            contextMenu.Items.Add(deleteMenuItem);
            contextMenu.Items.Add(protectMenuItem);
            contextMenu.Items.Add(new SeparatorAdv());
            contextMenu.Items.Add(hideMenuItem);
            contextMenu.Items.Add(unhideMenuItem);
            return contextMenu;
        }
#endif
        public void UnregisterSheetFromFormulaEngine()
        {
            foreach (var item in GridCollection)
            {
                GridFormulaEngine.UnregisterGridAsSheet(item.Key, item.Value.Model);
            }
        }
        #endregion

        #region ExportExtensions

        public void Export()
        {
            //SaveFileDialog sfd = new SaveFileDialog
            //                         {
            //    //DefaultExt = fileExtension,
            //    FilterIndex = 1,
            //    //Filter = "Excel 97 - 2003 Files(*.xls)|*.xls|Excel 2007 - 2010 Files(*.xlsx)|*.xlsx"
            //};
            //if (sfd.ShowDialog() == true)
            //{
            //    using (Stream stream = sfd.OpenFile())
            //    {
            //        ExcelProperties.WorkBook.SaveAs(stream);
            //    }
            //}
        }

#if !SILVERLIGHT
        private bool SaveFlag;
#endif
        public void SaveAs()
        {
            SaveFileDialog sfd = new SaveFileDialog
            {
                FilterIndex = 1,
                Filter = "Excel 97 to 2003 Files(*.xls)|*.xls|Excel 2007 to 2010 Files(*.xlsx)|*.xlsx"
            };

            if (sfd.ShowDialog() == true)
            {
#if !SILVERLIGHT
                SaveFlag = true;
#endif
                using (Stream stream = sfd.OpenFile())
                {
                    if (sfd.FilterIndex == 1)
                        this.ExcelProperties.WorkBook.Version = ExcelVersion.Excel97to2003;
                    else
                        this.ExcelProperties.WorkBook.Version = ExcelVersion.Excel2010;
                    this.ExcelProperties.WorkBook.SaveAs(stream);
                    
                    if (this.ExcelProperties.WorkBook.Version != ExcelVersion.Excel97to2003)
                    {
                        Stream xdoc = this.GridProperties.ActiveSpreadsheetGrid.CreateXMLFile();
                        ZipArchive archive = new ZipArchive();
                        archive.Open(stream, true);
                        if (archive["Syncfusion"] == null)
                            archive.AddItem("Syncfusion", null, false, FileAttributes.Directory);
                        if (archive["Syncfusion/Custom.xml"] != null)
                            archive.UpdateItem("Syncfusion/Custom.xml", xdoc, false);
                        else
                            archive.AddItem("Syncfusion/Custom.xml", xdoc, true, FileAttributes.Normal);
                        archive.Save(stream, false);
                        archive.Close();
                    }
                }
            }
            else
            {
#if !SILVERLIGHT
                SaveFlag = false;
#endif
                NewWorkbook();
            }
        }

        public ExportToExcelCommand ExportToExcelCommand
        {
            get { return (ExportToExcelCommand)GetValue(ExportToExcelCommandProperty); }
            set { SetValue(ExportToExcelCommandProperty, value); }
        }

        public static readonly DependencyProperty ExportToExcelCommandProperty =
            DependencyProperty.Register("ExportToExcelCommand", typeof(ExportToExcelCommand), typeof(SpreadsheetControl), new PropertyMetadata(null));

        #endregion

        #region EnableRenderOptimization

#if SILVERLIGHT
        public bool EnableRenderOptimization
        {
            get { return (bool)GetValue(EnableRenderOptimizationProperty); }
            set { SetValue(EnableRenderOptimizationProperty, value); }
        }

        public static readonly DependencyProperty EnableRenderOptimizationProperty = 
            DependencyProperty.Register("EnableRenderOptimization", typeof(bool), typeof(SpreadsheetControl), new PropertyMetadata(OnEnableRenderOptimizationChanged));

        private static void OnEnableRenderOptimizationChanged(DependencyObject dpo, DependencyPropertyChangedEventArgs args)
        {
            SpreadsheetControl ctrl = dpo as SpreadsheetControl;
            if (ctrl != null)
                ctrl.OnEnableRenderOptimizationChanged((bool)args.NewValue);
        }

        private void OnEnableRenderOptimizationChanged(bool NewValue)
        {
            if (GridCollection != null)
            {
                foreach (var item in GridCollection)
                {
                    item.Value.EnableRenderOptimization = NewValue;
                }
            }
        }
#endif
        #endregion

        #region Commands

        public ProtectSheetCommand ProtectSheetCommand
        {
            get { return (ProtectSheetCommand)GetValue(ProtectSheetCommandProperty); }
            set { SetValue(ProtectSheetCommandProperty, value); }
        }

        public static readonly DependencyProperty ProtectSheetCommandProperty =
            DependencyProperty.Register("ProtectSheetCommand", typeof(ProtectSheetCommand), typeof(SpreadsheetControl), new PropertyMetadata(null));

        public DeleteSheetCommand DeleteSheetCommand
        {
            get { return (DeleteSheetCommand)GetValue(DeleteSheetCommandProperty); }
            set { SetValue(DeleteSheetCommandProperty, value); }
        }

        public static readonly DependencyProperty DeleteSheetCommandProperty =
            DependencyProperty.Register("DeleteSheetCommand", typeof(DeleteSheetCommand), typeof(SpreadsheetControl), new PropertyMetadata(null));

        public UnHideSheetCommand UnHideSheetCommand
        {
            get { return (UnHideSheetCommand)GetValue(UnHideSheetCommandProperty); }
            set { SetValue(UnHideSheetCommandProperty, value); }
        }

        public static readonly DependencyProperty UnHideSheetCommandProperty =
            DependencyProperty.Register("UnHideSheetCommand", typeof(UnHideSheetCommand), typeof(SpreadsheetControl), new PropertyMetadata(null));

        public HideSheetCommand HideSheetCommand
        {
            get { return (HideSheetCommand)GetValue(HideSheetCommandProperty); }
            set { SetValue(HideSheetCommandProperty, value); }
        }

        public static readonly DependencyProperty HideSheetCommandProperty =
            DependencyProperty.Register("HideSheetCommand", typeof(HideSheetCommand), typeof(SpreadsheetControl), new PropertyMetadata(null));

        public RowColumnHeadersVisiblityCommand RowColumnHeadersVisiblityCommand
        {
            get { return (RowColumnHeadersVisiblityCommand)GetValue(RowColumnHeadersVisiblityCommandProperty); }
            set { SetValue(RowColumnHeadersVisiblityCommandProperty, value); }
        }
        
        public static readonly DependencyProperty RowColumnHeadersVisiblityCommandProperty =
            DependencyProperty.Register("RowColumnHeadersVisiblityCommand", typeof(RowColumnHeadersVisiblityCommand), typeof(SpreadsheetControl), new PropertyMetadata(null));

        public InsertPictureCommand InsertPictureCommand
        {
            get { return (InsertPictureCommand)GetValue(InsertPictureCommandProperty); }
            set { SetValue(InsertPictureCommandProperty, value); }
        }

        public static readonly DependencyProperty InsertPictureCommandProperty =
            DependencyProperty.Register("InsertPictureCommand", typeof(InsertPictureCommand), typeof(SpreadsheetControl), new PropertyMetadata(null));

       

        public ShowGridLinesCommand ShowGridLinesCommand
        {
            get { return (ShowGridLinesCommand)GetValue(ShowGridLinesCommandProperty); }
            set { SetValue(ShowGridLinesCommandProperty, value); }
        }

        public static readonly DependencyProperty ShowGridLinesCommandProperty =
            DependencyProperty.Register("ShowGridLinesCommand", typeof(ShowGridLinesCommand), typeof(SpreadsheetControl), new PropertyMetadata(null));


        public PasteCommand PasteCommand
        {
            get { return (PasteCommand)GetValue(PasteCommandProperty); }
            set { SetValue(PasteCommandProperty, value); }
        }

        public static readonly DependencyProperty PasteCommandProperty =
            DependencyProperty.Register("PasteCommand", typeof(PasteCommand), typeof(SpreadsheetControl), new PropertyMetadata(null));

        public CutCommand CutCommand
        {
            get { return (CutCommand)GetValue(CutCommandProperty); }
            set { SetValue(CutCommandProperty, value); }
        }

        public static readonly DependencyProperty CutCommandProperty =
            DependencyProperty.Register("CutCommand", typeof(CutCommand), typeof(SpreadsheetControl), new PropertyMetadata(null));

      
        public CopyCommand CopyCommand
        {
            get { return (CopyCommand)GetValue(CopyCommandProperty); }
            set { SetValue(CopyCommandProperty, value); }
        }

        public static readonly DependencyProperty CopyCommandProperty =
            DependencyProperty.Register("CopyCommand", typeof(CopyCommand), typeof(SpreadsheetControl), new PropertyMetadata(null));

        public InsertCommentCommand InsertCommentCommand
        {
            get { return (InsertCommentCommand)GetValue(InsertCommentCommandProperty); }
            set { SetValue(InsertCommentCommandProperty, value); }
        }

        public static readonly DependencyProperty InsertCommentCommandProperty =
            DependencyProperty.Register("InsertCommentCommand", typeof(InsertCommentCommand), typeof(SpreadsheetControl), new PropertyMetadata(null));

        public HyperlinkCommand HyperlinkCommand
        {
            get { return (HyperlinkCommand)GetValue(HyperlinkCommandProperty); }
            set { SetValue(HyperlinkCommandProperty, value); }
        }

        public static readonly DependencyProperty HyperlinkCommandProperty =
            DependencyProperty.Register("HyperlinkCommand", typeof(HyperlinkCommand), typeof(SpreadsheetControl), new PropertyMetadata(null));

        public static readonly DependencyProperty BoldCommandProperty =
            DependencyProperty.Register("BoldCommand", typeof(BoldCommand), typeof(SpreadsheetControl), new PropertyMetadata(null));

        public BoldCommand BoldCommand
        {
            get { return (BoldCommand)GetValue(BoldCommandProperty); }
            set { SetValue(BoldCommandProperty, value); }
        }

        public static readonly DependencyProperty ItalicCommandProperty =
            DependencyProperty.Register("ItalicCommand", typeof(ItalicCommand), typeof(SpreadsheetControl), new PropertyMetadata(null));

        public ItalicCommand ItalicCommand
        {
            get { return (ItalicCommand)GetValue(ItalicCommandProperty); }
            set { SetValue(ItalicCommandProperty, value); }
        }

        public static readonly DependencyProperty UnderlineCommandProperty =
            DependencyProperty.Register("UnderlineCommand", typeof(UnderlineCommand), typeof(SpreadsheetControl), new PropertyMetadata(null));

        public UnderlineCommand UnderlineCommand
        {
            get { return (UnderlineCommand)GetValue(UnderlineCommandProperty); }
            set { SetValue(UnderlineCommandProperty, value); }
        }

        public FontFamilyCommand FontFamilyCommand
        {
            get { return (FontFamilyCommand)GetValue(ChangeFontFamilyCommandProperty); }
            set { SetValue(ChangeFontFamilyCommandProperty, value); }
        }

        public static readonly DependencyProperty ChangeFontFamilyCommandProperty =
            DependencyProperty.Register("FontFamilyCommand", typeof(FontFamilyCommand), typeof(SpreadsheetControl), new PropertyMetadata(null));


        public static readonly DependencyProperty FontSizeCommandProperty =
            DependencyProperty.Register("FontSizeCommand", typeof(FontSizeCommand), typeof(SpreadsheetControl), new PropertyMetadata(null));

        public FontSizeCommand FontSizeCommand
        {
            get { return (FontSizeCommand)GetValue(FontSizeCommandProperty); }
            set { SetValue(FontSizeCommandProperty, value); }
        }

        public static readonly DependencyProperty FillColorCommandProperty =
            DependencyProperty.Register("FillColorCommand", typeof(FillColorCommand), typeof(SpreadsheetControl), new PropertyMetadata(null));

        public FillColorCommand FillColorCommand
        {
            get { return (FillColorCommand)GetValue(FillColorCommandProperty); }
            set { SetValue(FillColorCommandProperty, value); }
        }

        public static readonly DependencyProperty FontColorCommandProperty =
            DependencyProperty.Register("FontColorCommand", typeof(FontColorCommand), typeof(SpreadsheetControl), new PropertyMetadata(null));

        public FontColorCommand FontColorCommand
        {
            get { return (FontColorCommand)GetValue(FontColorCommandProperty); }
            set { SetValue(FontColorCommandProperty, value); }
        }

        public static readonly DependencyProperty VerticalAlignmentCommandProperty =
            DependencyProperty.Register("VerticalAlignmentCommand", typeof(VerticalAlignmentCommand), typeof(SpreadsheetControl), new PropertyMetadata(null));

        public VerticalAlignmentCommand VerticalAlignmentCommand
        {
            get { return (VerticalAlignmentCommand)GetValue(VerticalAlignmentCommandProperty); }
            set { SetValue(VerticalAlignmentCommandProperty, value); }
        }

        public static readonly DependencyProperty HorizontalAlignmentCommandProperty =
            DependencyProperty.Register("HorizontalAlignmentCommand", typeof(HorizontalAlignmentCommand), typeof(SpreadsheetControl), new PropertyMetadata(null));

        public HorizontalAlignmentCommand HorizontalAlignmentCommand
        {
            get { return (HorizontalAlignmentCommand)GetValue(HorizontalAlignmentCommandProperty); }
            set { SetValue(HorizontalAlignmentCommandProperty, value); }
        }

        public ConditionalFormatCommand ConditionalFormatCommand
        {
            get { return (ConditionalFormatCommand)GetValue(ConditionalFormatCommandProperty); }
            set { SetValue(ConditionalFormatCommandProperty, value); }
        }

        public static readonly DependencyProperty ConditionalFormatCommandProperty =
            DependencyProperty.Register("ConditionalFormatCommand", typeof(ConditionalFormatCommand), typeof(SpreadsheetControl), new PropertyMetadata(null));


      
        public FormatAsTableCommand FormatAsTableCommand
        {
            get { return (FormatAsTableCommand)GetValue(FormatAsTableCommandProperty); }
            set { SetValue(FormatAsTableCommandProperty, value); }
        }

        public static readonly DependencyProperty FormatAsTableCommandProperty =
            DependencyProperty.Register("FormatAsTableCommand", typeof(FormatAsTableCommand), typeof(SpreadsheetControl), new PropertyMetadata(null));

        public CellStyleCommand CellStyleCommand
        {
            get { return (CellStyleCommand)GetValue(CellStyleCommandProperty); }
            set { SetValue(CellStyleCommandProperty, value); }
        }

        public static readonly DependencyProperty CellStyleCommandProperty =
            DependencyProperty.Register("CellStyleCommand", typeof(CellStyleCommand), typeof(SpreadsheetControl), new PropertyMetadata(null));

        public InsertRowCommand InsertRowCommand
        {
            get { return (InsertRowCommand)GetValue(InsertRowCommandProperty); }
            set { SetValue(InsertRowCommandProperty, value); }
        }

        public static readonly DependencyProperty InsertRowCommandProperty =
            DependencyProperty.Register("InsertRowCommand", typeof(InsertRowCommand), typeof(SpreadsheetControl), new PropertyMetadata(null));

        public DeleteRowCommand DeleteRowCommand
        {
            get { return (DeleteRowCommand)GetValue(DeleteRowCommandProperty); }
            set { SetValue(DeleteRowCommandProperty, value); }
        }

        public static readonly DependencyProperty DeleteRowCommandProperty =
            DependencyProperty.Register("DeleteRowCommand", typeof(DeleteRowCommand), typeof(SpreadsheetControl), new PropertyMetadata(null));

        public InsertColumnCommand InsertColumnCommand
        {
            get { return (InsertColumnCommand)GetValue(InsertColumnCommandProperty); }
            set { SetValue(InsertColumnCommandProperty, value); }
        }

        public static readonly DependencyProperty InsertColumnCommandProperty =
            DependencyProperty.Register("InsertColumnCommand", typeof(InsertColumnCommand), typeof(SpreadsheetControl), new PropertyMetadata(null));

        public DeleteColumnCommand DeleteColumnCommand
        {
            get { return (DeleteColumnCommand)GetValue(DeleteColumnCommandProperty); }
            set { SetValue(DeleteColumnCommandProperty, value); }
        }

        public static readonly DependencyProperty DeleteColumnCommandProperty =
            DependencyProperty.Register("DeleteColumnCommand", typeof(DeleteColumnCommand), typeof(SpreadsheetControl), new PropertyMetadata(null));

        public MergeCommand MergeCommand
        {
            get { return (MergeCommand)GetValue(MergeCommandProperty); }
            set { SetValue(MergeCommandProperty, value); }
        }

        public static readonly DependencyProperty MergeCommandProperty =
            DependencyProperty.Register("MergeCommand", typeof(MergeCommand), typeof(SpreadsheetControl), new PropertyMetadata(null));

        public RowHeightCommand RowHeightCommand
        {
            get { return (RowHeightCommand)GetValue(RowHeightCommandProperty); }
            set { SetValue(RowHeightCommandProperty, value); }
        }

         public static readonly DependencyProperty RowHeightCommandProperty =
            DependencyProperty.Register("RowHeightCommand", typeof(RowHeightCommand), typeof(SpreadsheetControl), new PropertyMetadata(null));


        public BorderCommand BorderCommand
        {
            get { return (BorderCommand)GetValue(BorderCommandProperty); }
            set { SetValue(BorderCommandProperty, value); }
        }

        public static readonly DependencyProperty BorderCommandProperty =
            DependencyProperty.Register("BorderCommand", typeof(BorderCommand), typeof(SpreadsheetControl), new PropertyMetadata(null));

        public ColumnWidthCommand ColumnWidthCommand
        {
            get { return (ColumnWidthCommand)GetValue(ColumnWidthCommandProperty); }
            set { SetValue(ColumnWidthCommandProperty, value); }
        }

        public static readonly DependencyProperty ColumnWidthCommandProperty =
            DependencyProperty.Register("ColumnWidthCommand", typeof(ColumnWidthCommand), typeof(SpreadsheetControl), new PropertyMetadata(null));

        public AutoFitCommand AutoFitCommand
        {
            get { return (AutoFitCommand)GetValue(AutoFitCommandProperty); }
            set { SetValue(AutoFitCommandProperty, value); }
        }

        public static readonly DependencyProperty AutoFitCommandProperty =
            DependencyProperty.Register("AutoFitCommand", typeof(AutoFitCommand), typeof(SpreadsheetControl), new PropertyMetadata(null));


        public HideRowCommand HideRowCommand
        {
            get { return (HideRowCommand)GetValue(HideRowCommandProperty); }
            set { SetValue(HideRowCommandProperty, value); }
        }

        public static readonly DependencyProperty HideRowCommandProperty =
            DependencyProperty.Register("HideRowCommand", typeof(HideRowCommand), typeof(SpreadsheetControl), new PropertyMetadata(null));

        public HideColumnCommand HideColumnCommand
        {
            get { return (HideColumnCommand)GetValue(HideColumnCommandProperty); }
            set { SetValue(HideColumnCommandProperty, value); }
        }

        public static readonly DependencyProperty HideColumnCommandProperty =
            DependencyProperty.Register("HideColumnCommand", typeof(HideColumnCommand), typeof(SpreadsheetControl), new PropertyMetadata(null));

      
        public IncreaseDecimalCommand IncreaseDecimalCommand
        {
            get { return (IncreaseDecimalCommand)GetValue(IncreaseDecimalCommandProperty); }
            set { SetValue(IncreaseDecimalCommandProperty, value); }
        }

        public static readonly DependencyProperty IncreaseDecimalCommandProperty =
            DependencyProperty.Register("IncreaseDecimalCommand", typeof(IncreaseDecimalCommand), typeof(SpreadsheetControl), new PropertyMetadata(null));

        public NumberFormatCommand NumberFormatCommand
        {
            get { return (NumberFormatCommand)GetValue(NumberFormatCommandProperty); }
            set { SetValue(NumberFormatCommandProperty, value); }
        }

        public static readonly DependencyProperty NumberFormatCommandProperty =
            DependencyProperty.Register("NumberFormatCommand", typeof(NumberFormatCommand), typeof(SpreadsheetControl), new PropertyMetadata(null));

        public HideCurrentSheetCommand HideCurrentSheetCommand
        {
            get { return (HideCurrentSheetCommand)GetValue(HideCurrentSheetCommandProperty); }
            set { SetValue(HideCurrentSheetCommandProperty, value); }
        }

        public static readonly DependencyProperty HideCurrentSheetCommandProperty =
            DependencyProperty.Register("HideCurrentSheetCommand", typeof(HideCurrentSheetCommand), typeof(SpreadsheetControl), new PropertyMetadata(null));
        public InsertSheetCommand InsertSheetCommand
        {
            get { return (InsertSheetCommand) GetValue(InsertSheetCommandProperty); }
            set { SetValue(InsertSheetCommandProperty, value); }
        }

        public static readonly DependencyProperty InsertSheetCommandProperty =
            DependencyProperty.Register("InsertSheetCommand", typeof (InsertSheetCommand), typeof (SpreadsheetControl), new PropertyMetadata(null));

        public DeleteCurrentSheetCommand DeleteCurrentSheetCommand
        {
            get { return (DeleteCurrentSheetCommand) GetValue(DeleteCurrentSheetCommandProperty); }
            set { SetValue(DeleteCurrentSheetCommandProperty, value); }
        }

        public static readonly DependencyProperty DeleteCurrentSheetCommandProperty =
            DependencyProperty.Register("DeleteCurrentSheetCommand", typeof(DeleteCurrentSheetCommand), typeof(SpreadsheetControl), new PropertyMetadata(null));

        public DataValidationCommand DataValidationCommand
        {
            get { return (DataValidationCommand) GetValue(DataValidationCommandProperty); }
            set { SetValue(DataValidationCommandProperty, value); }
        }
        
        public static readonly DependencyProperty DataValidationCommandProperty =
            DependencyProperty.Register("DataValidationCommand", typeof (DataValidationCommand), typeof (SpreadsheetControl), new PropertyMetadata(null));

        public FreezePaneCommand FreezePaneCommand
        {
            get { return (FreezePaneCommand) GetValue(FreezePaneCommandProperty); }
            set { SetValue(FreezePaneCommandProperty, value); }
        }
        
        public static readonly DependencyProperty FreezePaneCommandProperty =
            DependencyProperty.Register("FreezePaneCommand", typeof (FreezePaneCommand), typeof (SpreadsheetControl), new PropertyMetadata(null));



        public IncreaseIndentCommand IncreaseIndentCommand
        {
            get { return (IncreaseIndentCommand) GetValue(IncreaseIndentCommandProperty); }
            set { SetValue(IncreaseIndentCommandProperty, value); }
        }

        public static readonly DependencyProperty IncreaseIndentCommandProperty =
            DependencyProperty.Register("IncreaseIndentCommand", typeof (IncreaseIndentCommand), typeof (SpreadsheetControl), new PropertyMetadata(null));

        public ProtectWorkbookCommand ProtectWorkbookCommand
        {
            get { return (ProtectWorkbookCommand)GetValue(ProtectWorkbookCommandProperty); }
            set { SetValue(ProtectWorkbookCommandProperty, value); }
        }

        public static readonly DependencyProperty ProtectWorkbookCommandProperty =
            DependencyProperty.Register("ProtectWorkbookCommand", typeof(ProtectWorkbookCommand), typeof(SpreadsheetControl), new PropertyMetadata(null));

        public GroupOptionsCommand GroupOptionsCommand
        {
            get { return (GroupOptionsCommand)GetValue(GroupOptionsCommandProperty); }
            set { SetValue(GroupOptionsCommandProperty, value); }
        }

        public static readonly DependencyProperty GroupOptionsCommandProperty =
            DependencyProperty.Register("GroupOptionsCommand", typeof(GroupOptionsCommand), typeof(SpreadsheetControl), new PropertyMetadata(null));


        public GroupCommand GroupCommand
        {
            get { return (GroupCommand)GetValue(GroupCommandProperty); }
            set { SetValue(GroupCommandProperty, value); }
        }

        public static readonly DependencyProperty GroupCommandProperty =
            DependencyProperty.Register("GroupCommand", typeof(GroupCommand), typeof(SpreadsheetControl), new PropertyMetadata(null));

        public UngroupCommand UngroupCommand
        {
            get { return (UngroupCommand)GetValue(UngroupCommandProperty); }
            set { SetValue(UngroupCommandProperty, value); }
        }

        public static readonly DependencyProperty UngroupCommandProperty =
            DependencyProperty.Register("UngroupCommand", typeof(UngroupCommand), typeof(SpreadsheetControl), new PropertyMetadata(null));

        public NewCommand NewCommand
        {
            get { return (NewCommand)GetValue(NewCommandProperty); }
            set { SetValue(NewCommandProperty, value); }
        }

        public static readonly DependencyProperty NewCommandProperty =
            DependencyProperty.Register("CloseCommand", typeof(NewCommand), typeof(SpreadsheetControl), new PropertyMetadata(null));

       
        public EncryptCommand EncryptCommand
        {
            get { return (EncryptCommand)GetValue(EncryptCommandProperty); }
            set { SetValue(EncryptCommandProperty, value); }
        }

        public static readonly DependencyProperty EncryptCommandProperty =
            DependencyProperty.Register("EncryptCommand", typeof(EncryptCommand), typeof(SpreadsheetControl), new PropertyMetadata(null));

        public DeleteCommentCommand DeleteCommentCommand
        {
            get{ return (DeleteCommentCommand)GetValue(DeleteCommentCommandProperty); }
            set { SetValue(DeleteCommentCommandProperty, value); }
        }

        public static readonly DependencyProperty DeleteCommentCommandProperty = 
            DependencyProperty.Register("DeleteCommentCommand", typeof(DeleteCommentCommand), typeof(SpreadsheetControl), new PropertyMetadata(null));
        
        public ProtectCurrentSheetCommand ProtectCurrentSheetCommand
        {
            get { return (ProtectCurrentSheetCommand)GetValue(ProtectCurrentSheetCommandProperty); }
            set { SetValue(ProtectCurrentSheetCommandProperty, value); }
        }

        public static readonly DependencyProperty ProtectCurrentSheetCommandProperty =
            DependencyProperty.Register("ProtectCurrentSheetCommand", typeof(ProtectCurrentSheetCommand), typeof(SpreadsheetControl), new PropertyMetadata(null));


        /// <summary>
        /// Gets or sets the wrap text command.
        /// </summary>
        /// <value>The wrap text command.</value>
        public WrapTextCommand WrapTextCommand
        {
            get { return (WrapTextCommand)GetValue(WrapTextCommandProperty); }
            set { SetValue(WrapTextCommandProperty, value); }
        }

        public static readonly DependencyProperty WrapTextCommandProperty =
            DependencyProperty.Register("WrapTextCommand", typeof(WrapTextCommand), typeof(SpreadsheetControl), new PropertyMetadata(null));

#if !SILVERLIGHT

        public SaveCommand SaveCommand
        {
            get { return (SaveCommand)GetValue(SaveCommandProperty); }
            set { SetValue(SaveCommandProperty, value); }
        }

        public static readonly DependencyProperty SaveCommandProperty =
            DependencyProperty.Register("SaveCommand", typeof(SaveCommand), typeof(SpreadsheetControl), new PropertyMetadata(null));

        public ExitCommand ExitCommand
        {
            get { return (ExitCommand)GetValue(ExitCommandProperty); }
            set { SetValue(ExitCommandProperty, value); }
        }

        public static readonly DependencyProperty ExitCommandProperty =
            DependencyProperty.Register("ExitCommand", typeof(ExitCommand), typeof(SpreadsheetControl), new PropertyMetadata(null));
#endif

        #endregion

        #region Public Methods

        public void New()
        {
            DisposeOldInstances();
            CreateBlankDefaultWorkbook(3);
            LoadTabItems();
        }

        private void DisposeOldInstances()
        {
            GridProperties.CurrentExcelGridModel = null;
            if (ExcelProperties.WorkBook != null)
            {
                ExcelProperties.Application.Workbooks.Close();
                ExcelProperties.WorkBook.Close();
            }

#if !SILVERLIGHT
            UnWireWorkSheetEvent();
#endif
            if (_excelTabControlAdv != null)
            {
                foreach (var tabItem in _excelTabControlAdv.Items)
                {
#if !SILVERLIGHT
                    ((TabItemExt)tabItem).Content = null;
#else
                    ((TabItemAdv) tabItem).Content = null;
#endif
                }
                _excelTabControlAdv.Items.Clear();
            }

            foreach (var grid in GridCollection)
            {
                this.FormulaRangeSelection.UnHookEvents(grid.Value);
#if !SILVERLIGHT
				grid.Value.Model.Dispose();
                grid.Value.Dispose(true);
#else
                grid.Value.Dispose();
#endif
            }
#if !SILVERLIGHT
            UnregisterSheetFromFormulaEngine();
#endif
            GridCollection.Clear();
            CurrentSpreadsheetGrid = null;
            ModelCollection = null;
            this.GridProperties.SpreadsheetGrid = null;

            if (this.GridProperties.CurrentCell != null)
            {
                this.GridProperties.CurrentCell.Dispose();
                this.GridProperties.CurrentCell = null;
            }
            if (this.GridProperties.CurrentCellStyle != null)
            {
#if !SILVERLIGHT
            this.GridProperties.CurrentCellStyle.Dispose(true);
#endif
                this.GridProperties.CurrentCellStyle = null;
            }
        }

        internal void NewWorkbook()
        {
#if SILVERLIGHT
            var result = MessageBox.Show(SpreadsheetResourceWrapper.ValidationMessage_DiscardChanges, SpreadsheetResourceWrapper.Warning, MessageBoxButton.OKCancel);
            if (result == MessageBoxResult.OK)
            {
                ExcelProperties.WorkBook.Close();
                UnregisterSheetFromFormulaEngine();
                GridCollection.Clear();
                _excelTabControlAdv.Items.Clear();
                ExcelProperties.WorkBook = ExcelProperties.Application.Workbooks.Create(3);
                LoadTabItems();
            }
#else
            var result = MessageBox.Show(SpreadsheetResourceWrapper.ValidationMessage_DiscardChanges, SpreadsheetResourceWrapper.Warning, MessageBoxButton.YesNoCancel);
            if (result == MessageBoxResult.Yes || result == MessageBoxResult.No)
            {
                if (result == MessageBoxResult.No)
                {
                    SaveAs();
                    if(!SaveFlag)
                        return;
                }
                DisposeOldInstances();
                CreateBlankDefaultWorkbook(3);
                LoadTabItems();
            }
#endif
        }

        public void New(int count)
        {
            DisposeOldInstances();
            CreateBlankDefaultWorkbook(count);
            LoadTabItems();
        }

        public void ProtectSheet(string sheetName, string password)
        {
            IWorksheet worksheet =
             this.ExcelProperties.WorkBook.Worksheets[sheetName];
            if (worksheet != null)
            {
                worksheet.Protect(password);
                this.ExcelProperties.IsPasswordProtected = true;
                this.GridProperties.IsDisableMode = true;
                this.GridProperties.CurrentExcelGridModel.TableStyle.ReadOnly = true;
                RefreshTabItemContentMenu(sheetName);
            }
        }

        public void UnProtectSheet(string sheetName, string password)
        {
            IWorksheet worksheet =
               this.ExcelProperties.WorkBook.Worksheets[sheetName];
            if (worksheet != null && worksheet.IsPasswordProtected)
            {
                try
                {
                    worksheet.Unprotect(password);
                    this.ExcelProperties.IsPasswordProtected = false;
                    if (!this.GridProperties.IsFocusedOnGraphicCells)
                        this.GridProperties.IsDisableMode = false;
                    this.GridProperties.CurrentExcelGridModel.TableStyle.ReadOnly = false;
                    RefreshTabItemContentMenu(sheetName);
                }
                catch (Exception)
                {
                    MessageBox.Show(SpreadsheetResourceWrapper.ValidationMessage_InCorrectPassword, SpreadsheetResourceWrapper.MessageBoxCaption, MessageBoxButton.OK);
                }
            }
        }

        public void ProtectWorkbook(bool IsProtectWindow, bool IsProtectContent, string password)
        {
            if (this.ExcelProperties.WorkBook != null && !this.ExcelProperties.WorkBook.IsWindowProtection)
                this.ExcelProperties.WorkBook.Protect(IsProtectWindow, IsProtectContent, password);
        }

        public void ProtectWorkbook(bool IsProtectWindow, bool IsProtectContent)
        {
            if (this.ExcelProperties.WorkBook != null && !this.ExcelProperties.WorkBook.IsWindowProtection)
                this.ExcelProperties.WorkBook.Protect(IsProtectWindow, IsProtectContent);
        }

        public void UnProtectWorkbook()
        {
            this.ExcelProperties.WorkBook.Unprotect();
        }

        public void UnProtectWorkbook(string password)
        {
            this.ExcelProperties.WorkBook.Unprotect(password);
        }

        public void AddSheet(string name)
        {
            AddSheet(name, ExcelProperties.WorkBook.Worksheets.Count);
        }

        public void AddSheet(string name, int insertAt)
        {
            var addingEventArgs = new WorkSheetAddingEventArgs() { SheetName = name, Cancel = false };
            if (WorkSheetAdding != null)
            {
                WorkSheetAdding(this, addingEventArgs);
                name = string.IsNullOrEmpty(addingEventArgs.SheetName) ? name : addingEventArgs.SheetName;
            }

            if (!addingEventArgs.Cancel)
            {
                ExcelProperties.WorkBook.Worksheets.Create(name);
                IWorksheet worksheet = ExcelProperties.WorkBook.Worksheets[name];
#if !SILVERLIGHT
                //Wire Cell Value Changed Event
                worksheet.CellValueChanged += new XlsIO.Implementation.RangeImpl.CellValueChangedEventHandler(OnCellValueChanged);
#endif
                if (worksheet == null) return;

                string lastCell = GridRangeInfo.GetAlphaLabel(ExcelProperties.DefaultColumnCount) + ExcelProperties.DefaultRowCount;
                string defaultRange = "A1:" + lastCell;
                if (ExcelProperties.DefaultRowHeight != 15)
                    worksheet.Range[defaultRange].RowHeight = ExcelProperties.DefaultRowHeight;
                worksheet.Range[defaultRange].ColumnWidth = ExcelProperties.DefaultColumnWidth;

                if (_excelTabControlAdv != null)
                {
                    SpreadsheetGridModel model = new SpreadsheetGridModel();
                    GridFormulaEngine.RegisterGridAsSheet(ExcelProperties.WorkBook.Worksheets[name].Name, model, ExcelGridModelImportExtensions.SheetFamilyId);
                    model.ImportFromExcelToVirtualGrid(ExcelProperties.WorkBook, ExcelProperties.WorkBook.Worksheets[name], null);
                    model.FormulaEngine.SupportBlanksInSheetNames = true;
                    model.FormulaEngine.UseNoAmpersandQuotes = true;
                    model.Options.AllowExcelLikeResizing = true;
                    model.TableStyle.CellType = "FormulaCell";
#if !SILVERLIGHT
                    TabItemExt tab = new TabItemExt
                    {
                        Header = name
                    };
#else
                TabItemAdv tab = new TabItemAdv
                                       {
                                           Header = name
                                       };
#endif
                    SpreadsheetControl.SetSheetName(tab, name);

#if !SILVERLIGHT
                    if (this.TabStyleManager != null && this.TabStyleManager.ShowTabItemContextMenu)
                    {
                        if (this.TabStyleManager.IsCustomTabItemContextMenuEnabled)
                            tab.ContextMenu = this.TabStyleManager.TabItemContextMenu;
                        else
                            tab.ContextMenu = GetDefaultContextMenu(name, ExcelProperties.WorkBook.Worksheets[name].IsPasswordProtected);
                        tab.ContextMenuOpening += new ContextMenuEventHandler(tab_ContextMenuOpening);
                    }
#else
                if (this.TabStyleManager != null && this.TabStyleManager.ShowTabItemContextMenu)
                {
                    if (this.TabStyleManager.IsCustomTabItemContextMenuEnabled)
                        ContextMenuAdvService.SetContextMenuAdv(tab, this.TabStyleManager.TabItemContextMenu);
                    else
                        ContextMenuAdvService.SetContextMenuAdv(tab, GetDefaultContextMenu(name, ExcelProperties.WorkBook.Worksheets[name].IsPasswordProtected));
                }
#endif

                    SpreadsheetGrid grid = new SpreadsheetGrid();
#if !SILVERLIGHT
                    ScrollViewer sv = new ScrollViewer
#else
                ScrollableContentViewer sv = new ScrollableContentViewer
#endif
                                              {
                                                  CanContentScroll = true,
                                                  HorizontalScrollBarVisibility = ScrollBarVisibility.Auto,
                                                  VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
                                                  Content = grid
                                              };

                    sv.Loaded+=new RoutedEventHandler(sv_Loaded);

                    grid.SheetName = name;
                    grid.ExcelProperties = ExcelProperties;
                    model.SperadsheetGrid = grid;
                    grid.Model = model;
                    grid.MouseControllerDispatcher.Add(new GridExcelMarkerMouseController(grid, worksheet));
                    grid.Model.GridCopyPaste = new SpreadsheetGridCopyPaste(grid, worksheet);
                    grid.IsRowHeaderVisible = worksheet.IsRowColumnHeadersVisible;
                    grid.IsColumnHeadersVisible = worksheet.IsRowColumnHeadersVisible;
                    InitializeSpreadsheetGrid(grid);
                    GridCollection.Add(SpreadsheetControl.GetSheetName(tab), grid);
                    tab.Content = sv;
                    _excelTabControlAdv.Items.Insert(insertAt, tab);
                    TabSelectedIndex = insertAt;
                    ExcelProperties.VisibleSheetCount += 1;
                }

                if (WorkSheetAdded != null)
                    WorkSheetAdded(this, new WorkSheetAddedEventArgs() { ExcelProperties = this.ExcelProperties, SheetName = name });
            }
        }

        public void RemoveSheet(string name)
        {
            if (ExcelProperties.VisibleSheetCount > 1 && ExcelProperties.WorkBook.Worksheets.Count >1)
            {
                ExcelProperties.WorkBook.Worksheets.Remove(name);
                if (_excelTabControlAdv != null)
                {
#if !SILVERLIGHT
                    TabItemExt tab = _excelTabControlAdv.Items.Cast<TabItemExt>().FirstOrDefault(item => SpreadsheetControl.GetSheetName(item) == name);
                    if (tab != null) tab.ContextMenuOpening -= new ContextMenuEventHandler(tab_ContextMenuOpening);
#else
                    TabItemAdv tab = _excelTabControlAdv.Items.Cast<TabItemAdv>().FirstOrDefault(item => SpreadsheetControl.GetSheetName(item) == name);
#endif
                    SpreadsheetGrid grid = GridCollection[name];
                    this.GridProperties.UnWireGridEvents(grid);
                    this.FormulaRangeSelection.UnHookEvents(grid);
                    GridFormulaEngine.UnregisterGridAsSheet(name, grid.Model);
                    GridCollection.Remove(name);
                    if (tab != null) _excelTabControlAdv.Items.Remove(tab);
                    ExcelProperties.VisibleSheetCount -= 1;
                    SpreadsheetGridCopyPaste.SourceRange = null;
#if !SILVERLIGHT
                    Clipboard.SetText(string.Empty);
#endif
                }
            }
            else
                MessageBox.Show(SpreadsheetResourceWrapper.ValidationMessage_DeleteSheet, SpreadsheetResourceWrapper.MessageBoxCaption, MessageBoxButton.OK);
            
        }

        public void HideSheet(string name)
        {
            if (ExcelProperties.VisibleSheetCount <= 1)
            {
                MessageBox.Show(SpreadsheetResourceWrapper.ValidationMessage_HideSheet, SpreadsheetResourceWrapper.MessageBoxCaption, MessageBoxButton.OK);
                return;
            }
            var worksheet = ExcelProperties.WorkBook.Worksheets[name];
            worksheet.Visibility = WorksheetVisibility.Hidden;
        
#if !SILVERLIGHT
            TabItemExt tab = _excelTabControlAdv.Items.Cast<TabItemExt>().FirstOrDefault(item => SpreadsheetControl.GetSheetName(item) == name);
#else
            TabItemAdv tab = _excelTabControlAdv.Items.Cast<TabItemAdv>().FirstOrDefault(item => SpreadsheetControl.GetSheetName(item) == name);
#endif
            tab.Visibility = Visibility.Collapsed;
            ExcelProperties.VisibleSheetCount -= 1;
            if (worksheet.Index == TabSelectedIndex)
            {
                bool isTabIndexset = false;
                if (ExcelProperties.WorkBook.Worksheets.Count > TabSelectedIndex + 1)
                {
                    for (int i = (int)TabSelectedIndex + 1; i < ExcelProperties.WorkBook.Worksheets.Count; i++)
                    {
                        if (ExcelProperties.WorkBook.Worksheets[i].Visibility == WorksheetVisibility.Visible)
                        {
                            TabSelectedIndex = i;
                            isTabIndexset = true;
                            break;
                        }
                    }
                }
                if (!isTabIndexset && TabSelectedIndex - 1 >= 0)
                {
                    for (int j = (int)TabSelectedIndex - 1; j >= 0; j--)
                    {
                        if (ExcelProperties.WorkBook.Worksheets[j].Visibility == WorksheetVisibility.Visible)
                        {
                            TabSelectedIndex = j;
                            isTabIndexset = true;
                            break;
                        }
                    }
                }
            }
            //int count = _excelTabControlAdv.Items.Count;
            //if (TabSelectedIndex + 1 < _excelTabControlAdv.Items.Count)
            //    TabSelectedIndex += 1;
            //else if (TabSelectedIndex - 1 >= 0)
            //    TabSelectedIndex -= 1;

            this.UnHideSheetCommand.ExecuteChanged();
        }

        public void UnHideSheet(string name)
        {
            var worksheet = ExcelProperties.WorkBook.Worksheets[name];
            worksheet.Visibility = WorksheetVisibility.Visible;
#if !SILVERLIGHT
            TabItemExt tab = _excelTabControlAdv.Items.Cast<TabItemExt>().FirstOrDefault(item => SpreadsheetControl.GetSheetName(item) == name);
#else
            TabItemAdv tab = _excelTabControlAdv.Items.Cast<TabItemAdv>().FirstOrDefault(item => SpreadsheetControl.GetSheetName(item) == name);
#endif
            tab.Visibility = Visibility.Visible;
            ExcelProperties.VisibleSheetCount += 1;
             int index = _excelTabControlAdv.Items.IndexOf(tab);
            TabSelectedIndex = index;
        }

        public void EncryptWorkBook(string password)
        {
            if (ExcelProperties != null && ExcelProperties.WorkBook != null)
                ExcelProperties.WorkBook.PasswordToOpen = password;
        }

        #endregion

        #region Internal Methods

        internal void RaiseWorkBookLoaded()
        {
            if (WorkBookLoaded != null)
            {
                List<SpreadsheetGrid> ListofGrid = new List<SpreadsheetGrid>();
                foreach (var item in GridCollection)
                    ListofGrid.Add(item.Value);
                WorkbookLoadedEventArgs e = new WorkbookLoadedEventArgs(ExcelProperties.WorkBook, ListofGrid);
                WorkBookLoaded(this, e);
            }
        }

        internal void RefreshTabItemContentMenu(string sheetName)
        {
            foreach (var item in this._excelTabControlAdv.Items)
            {
#if !SILVERLIGHT
                var tabItem = (TabItemExt)item;
#else
                var tabItem = (TabItemAdv)item;
#endif

                string tabsheetName = GetSheetName(tabItem);
                if (tabItem != null && sheetName != null && sheetName.ToString().Equals(tabsheetName) && !TabStyleManager.IsCustomTabItemContextMenuEnabled && TabStyleManager.ShowTabItemContextMenu)
                {
#if !SILVERLIGHT
                    tabItem.ContextMenu = this.GetDefaultContextMenu(sheetName, this.ExcelProperties.WorkBook.Worksheets[sheetName].IsPasswordProtected);
#else
                    ContextMenuAdvService.SetContextMenuAdv(tabItem, GetDefaultContextMenu(ExcelProperties.WorkBook.Worksheets[sheetName].Name, ExcelProperties.WorkBook.Worksheets[sheetName].IsPasswordProtected));
#endif
                    break;
                }
            }
        }

        private void InitializeSpreadsheetGrid(SpreadsheetGrid grid)
        {
            grid.Model.Options.ExcelLikeFreezePane = true;
            grid.Model.Options.ExcelLikeCurrentCell = true;
            grid.Model.Options.ExcelLikeSelectionFrame = true;
            grid.Model.Options.AllowExcelLikeResizing = true;
            grid.Model.Options.EnterKeyBehaviour = EnterKeyBehaviour.MouseDown;
            grid.Model.Options.FloatCellMode = GridFloatCellsMode.OnDemandCalculation;
            grid.Model.Options.ActivateCurrentCellBehavior = GridCellActivateAction.DblClickOnCell;
            grid.IsStandAloneExcelGrid = false;
            GridCommentService.SetShowComment(grid, true);
            GridTooltipService.SetShowTooltips(grid, true);
#if !SILVERLIGHT
            grid.Model.TableStyle.FloatCellMode = GridFloatCellsMode.OnDemandCalculation;
            //grid.Model.TableStyle.EnableFloatingCell = true;
#else
            grid.Model.TableStyle.FloatCellsMode = GridFloatCellsMode.OnDemandCalculation;
            grid.Model.TableStyle.TextTrimming = TextTrimming.None;
            //grid.Model.TableStyle.EnableFloatCell = true;
            grid.Model.TableStyle.AllowRowResize = true;
            grid.Model.EnableMultiline = true;
#endif
            grid.Model.FormulaEngine.QueryDependentCellValue += new GridQueryDependentCellValueEventHandler(FormulaEngine_QueryDependentCellValue);
        }
        #endregion

        #region Scroll Viewer

#if SILVERLIGHT
        void sv_Loaded(object sender, RoutedEventArgs e)
        {
            ScrollableContentViewer sv = (sender as ScrollableContentViewer);
            if (sv != null)
            {
                ScrollBar HorizontalScrollBar = GetChildObject<ScrollBar>(sv, "HorizontalScrollBar");
                if (HorizontalScrollBar != null)
                {
                    // Thumb thumb = (Thumb)((FrameworkElement)VisualTreeHelper.GetChild(HorizontalScrollBar, 0)).FindName("HorizontalThumb");

                    GetThumbAndHookEvent(HorizontalScrollBar, "HorizontalThumb");
                }
                ScrollBar VerticalScrollBar = GetChildObject<ScrollBar>(sv, "VerticalScrollBar");
                if (VerticalScrollBar != null)
                {
                    GetThumbAndHookEvent(VerticalScrollBar, "VerticalThumb");
                }
            }
        }

#else
        void sv_Loaded(object sender, RoutedEventArgs e)
        {
            ScrollViewer sv = (sender as ScrollViewer);

            if (sv != null)
            {
                ScrollBar HorizontalScrollBar = GetChildObject<ScrollBar>(sv, "PART_HorizontalScrollBar");
                if (HorizontalScrollBar != null)
                {
                    GetThumbAndHookEvent(HorizontalScrollBar, "PART_Track");
                }
                ScrollBar VerticalScrollBar = GetChildObject<ScrollBar>(sv, "PART_VerticalScrollBar");
                if (VerticalScrollBar != null)
                {
                    GetThumbAndHookEvent(VerticalScrollBar, "PART_Track");
                }
            }
        }
#endif


#if SILVERLIGHT
        /// <summary>
        /// Unhook the scrollbar event
        /// </summary>
        private void UnhookScrollbarEvent(ScrollableContentViewer sv)
        {
            if (sv != null)
            {
                ScrollBar HorizontalScrollBar = GetChildObject<ScrollBar>(sv, "HorizontalScrollBar");
                if (HorizontalScrollBar != null)
                {
                    GetThumbAndUnhookEvent(HorizontalScrollBar, "HorizontalThumb");
                }
                ScrollBar VerticalScrollBar = GetChildObject<ScrollBar>(sv, "VerticalScrollBar");
                if (VerticalScrollBar != null)
                {
                    GetThumbAndUnhookEvent(VerticalScrollBar, "VerticalThumb");
                }
                sv.Loaded -= new RoutedEventHandler(sv_Loaded);
            }
        }
#else
        /// <summary>
        /// Unhook the scrollbar event
        /// </summary>
        private void UnhookScrollbarEvent(ScrollViewer sv)
        {
        if (sv != null)
            {
                ScrollBar HorizontalScrollBar = GetChildObject<ScrollBar>(sv, "PART_HorizontalScrollBar");
                if (HorizontalScrollBar != null)
                {
                    GetThumbAndHookEvent(HorizontalScrollBar, "PART_Track"); //  HorizontalThumb
                }
                ScrollBar VerticalScrollBar = GetChildObject<ScrollBar>(sv, "PART_VerticalScrollBar");
                if (VerticalScrollBar != null)
                {
                    GetThumbAndHookEvent(VerticalScrollBar, "PART_Track"); //  VerticalThumb
                }
            }
        }
#endif

#if SILVERLIGHT
        /// <summary>
        /// Get the scrollbar thumb and hook event
        /// </summary>
        private void GetThumbAndHookEvent(ScrollBar sb, string thumbName)
        {
            if (sb != null && sb.Tag == null)
            {
                Thumb thumb = GetChildObject<Thumb>(sb, thumbName);
                if (thumb != null)
                {
                    thumb.DragStarted += new DragStartedEventHandler(thumb_DragStarted);
                    thumb.DragCompleted += new DragCompletedEventHandler(thumb_DragCompleted);
                    sb.Tag = true;
                }
            }
        }
#else
        /// <summary>
        /// Get the scrollbar thumb and hook event
        /// </summary>
        private void GetThumbAndHookEvent(ScrollBar sb, string thumbName)
        {
            if (sb != null && sb.Tag == null)
            {
                Track track = GetChildObject<Track>(sb, thumbName);
                if (track != null)
                {
                    track.Thumb.DragStarted += new DragStartedEventHandler(thumb_DragStarted);
                    track.Thumb.DragCompleted += new DragCompletedEventHandler(thumb_DragCompleted);
                    sb.Tag = true;
                }
            }
        }
#endif

        /// <summary>
        /// Get the scrollbar thumb and unhook event
        /// </summary>
        private void GetThumbAndUnhookEvent(ScrollBar sb, string thumbName)
        {
            if (sb != null)
            {
                Thumb thumb = GetChildObject<Thumb>(sb, thumbName);
                if (thumb != null)
                {
                    thumb.DragStarted -= new DragStartedEventHandler(thumb_DragStarted);
                    thumb.DragCompleted -= new DragCompletedEventHandler(thumb_DragCompleted);
                }
            }
        }

        void thumb_DragCompleted(object sender, DragCompletedEventArgs e)
        {
            if (this.CurrentSpreadsheetGrid.ExcelProperties.spreadControl.OptimizeFormulaCalculation)
            {
                VisibleLinesCollection visibleRows = CurrentSpreadsheetGrid.ScrollRows.GetVisibleLines();
                VisibleLinesCollection visibleColumns = CurrentSpreadsheetGrid.ScrollColumns.GetVisibleLines();
                int top = visibleRows[visibleRows.FirstBodyVisibleIndex].LineIndex;
                int left = visibleColumns[visibleColumns.FirstBodyVisibleIndex].LineIndex;
                int bottom = visibleRows[visibleRows.LastBodyVisibleIndex].LineIndex;
                int right = visibleColumns[visibleColumns.LastBodyVisibleIndex].LineIndex;
                GridRangeInfo visibleRange = GridRangeInfo.Cells(top, left, bottom, right);
                CurrentSpreadsheetGrid.ResumeFormulaCalculation(visibleRange);
            }
        }

        void thumb_DragStarted(object sender, DragStartedEventArgs e)
        {
            if (this.CurrentSpreadsheetGrid.ExcelProperties.spreadControl.OptimizeFormulaCalculation)
                CurrentSpreadsheetGrid.SuspendFormulaCalculation();
        }

        /// <summary>
        /// Generic method to get the child object
        /// </summary>
        public static T GetChildObject<T>(DependencyObject obj, string name) where T : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
            {
                object c = VisualTreeHelper.GetChild(obj, i);
                if (c.GetType().FullName == typeof(T).FullName && (String.IsNullOrEmpty(name) || ((FrameworkElement)c).Name == name))
                {
                    return (T)c;
                }
                if (c is DependencyObject)
                {
                    object gc = GetChildObject<T>(c as DependencyObject, name);
                    if (gc != null)
                        return (T)gc;
                }
            }
            return null;
        }


        #endregion
    }

    #region Helper Class

    public static class GridExcelHelper
    {
        public static GridRangeInfo ConvertExcelRangeToGridRange(this IRange range)
        {
            return GridRangeInfo.Cells(range.Row, range.Column, range.LastRow, range.LastColumn);
        }

        public static GridRangeInfo ConvertExcelRangeToGridRange(string range)
        {
            //if (range.Contains(":"))
            //{
                string[] args = range.Split(new char[] { ':' });
                int argCount = args.GetLength(0);
                if (argCount == 2)
                {
                    int top = RowIndex(args[0]);
                    int left = ColumnIndex(args[0]);
                    int bottom = RowIndex(args[1]);
                    int right = ColumnIndex(args[1]);
                    return GridRangeInfo.Cells(top, left, bottom, right);
                }
                else if (argCount == 1)
                {
                    int top = RowIndex(args[0]);
                    int left = ColumnIndex(args[0]);
                    int bottom = RowIndex(args[0]);
                    int right = ColumnIndex(args[0]);
                    return GridRangeInfo.Cells(top, left, bottom, right);
                }
            //}
            return GridRangeInfo.Empty;
        }

        public static string ConvertGridRangeToExcelRange(this GridRangeInfo gridRangeInfo)
        {
            if (!gridRangeInfo.IsEmpty)
            {
                string firstcell = string.Empty;
                string lastcell = string.Empty;
                string range = string.Empty;
                if (gridRangeInfo.IsCells)
                {
                    firstcell = GridRangeInfo.GetAlphaLabel(gridRangeInfo.Left) + gridRangeInfo.Top;
                    lastcell = GridRangeInfo.GetAlphaLabel(gridRangeInfo.Right) + gridRangeInfo.Bottom;
                }
                if (!string.IsNullOrEmpty(firstcell) && !string.IsNullOrEmpty(lastcell))
                    range = firstcell + ":" + lastcell;
                return range;
            }
            return string.Empty;
        }

        public static string ConvertGridRangeToExcelRange(this GridRangeInfo gridRangeInfo,SpreadsheetGridModel gridModel)
        {
            if(!gridRangeInfo.IsEmpty)
            {
                string firstcell = string.Empty;
                string lastcell = string.Empty;
                string range = string.Empty;
                if (gridRangeInfo.IsCells)
                {

                    firstcell = GridRangeInfo.GetAlphaLabel(gridRangeInfo.Left == 0 ? gridRangeInfo.Left + 1 : gridRangeInfo.Left) + (gridRangeInfo.Top == 0 ? gridRangeInfo.Top + 1 : gridRangeInfo.Top);
                    lastcell = GridRangeInfo.GetAlphaLabel(gridRangeInfo.Right) + (gridRangeInfo.Bottom != 0 ? gridRangeInfo.Bottom.ToString() : string.Empty);
                }
                else if (gridRangeInfo.IsCols)
                {
                    firstcell = GridRangeInfo.GetAlphaLabel(gridRangeInfo.Left) + 1;
                    lastcell = GridRangeInfo.GetAlphaLabel(gridRangeInfo.Right) + gridModel.RowCount;
                }
                else if (gridRangeInfo.IsRows)
                {
                    firstcell = GridRangeInfo.GetAlphaLabel(1) + gridRangeInfo.Top;
                    lastcell = GridRangeInfo.GetAlphaLabel(gridModel.ColumnCount) + gridRangeInfo.Bottom;
                }
                else if (gridRangeInfo.IsTable)
                {
                    firstcell = GridRangeInfo.GetAlphaLabel(1) + 1;
                    lastcell = GridRangeInfo.GetAlphaLabel(gridModel.ColumnCount) + gridModel.RowCount;
                }
                if (!string.IsNullOrEmpty(firstcell) && !string.IsNullOrEmpty(lastcell))
                    range = firstcell + ":" + lastcell;
                return range;
            }
            return string.Empty;
        }

        /// <summary>
        /// Method to auto fit the column width for the given IRange.
        /// </summary>
        /// <param name="spreadsheetControl"></param>
        /// <param name="range"></param>
        public static void ResizeColumnToFit(this SpreadsheetControl spreadsheetControl, IRange range)
        {
            var sheet = spreadsheetControl.ExcelProperties.WorkBook.ActiveSheet;
            for (int i = 1; i <= range.LastColumn; i++)
            {
                sheet.AutofitColumn(i);
                var width = sheet.GetColumnWidthInPixels(i);
                if (width != 0)
                {
                    spreadsheetControl.GridProperties.CurrentExcelGridModel.ColumnWidths[i] = sheet.GetColumnWidthInPixels(i);
                }
                else
                {
                    spreadsheetControl.GridProperties.CurrentExcelGridModel.ColumnWidths.SetHidden(i, i, true);
                }
            }

        }

        internal static int RowIndex(string s)
        {
            int i = 0;
            while (i < s.Length && char.IsLetter(s[i]))
                i++;
            if (i < s.Length)
            {
                int row;
                if (int.TryParse(s.Substring(i), out row))
                {
                    return row;
                }
            }
            return -1;
        }

        internal static int ColumnIndex(string s)
        {
            int i = 0;
            int k = 0;
            s = s.ToUpper();
            while (i < s.Length && char.IsLetter(s[i]))
            {
                k = k * 26 + s[i] - 'A' + 1;
                i++;
            }
            return k;
        }
    }

    #endregion

    #region Enum

    public enum CellFormat
    {
        RowHeight,
        ColumnWidth,
    }

    public enum Freeze
    {
        FreezePanes = 0,
        FreezeTopRow = 1,
        FreezeFirstColumn = 2
    }

    #endregion

}