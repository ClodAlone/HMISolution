#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using Syncfusion.Windows.Controls.Grid;
using Syncfusion.Windows.Controls.Spreadsheet.Commands;
using Syncfusion.Windows.Controls.Spreadsheet.Resources;
using Syncfusion.Windows.Tools.Controls;
using Syncfusion.XlsIO;
#if !SILVERLIGHT
using Syncfusion.Windows.Shared;
using Microsoft.Win32;
using System.Data;
using Syncfusion.Windows.Controls.Cells;
using System.Security.Permissions;
using System.Xml.Linq;
using Syncfusion.XlsIO.Implementation;
#else
using Syncfusion.Windows.Controls.Theming;
using System.Xml.Linq;
using Syncfusion.Windows.Styles;
#endif


namespace Syncfusion.Windows.Controls.Spreadsheet
{
#if SILVERLIGHT
    //[Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Default,
    //  Type = typeof(SpreadsheetControl), XamlResource = "/Syncfusion.Spreadsheet.Silverlight;component/Themes/Generic.xaml")]
    //[Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2010Blue,
    //  Type = typeof(SpreadsheetControl), XamlResource = "/Syncfusion.Spreadsheet.Silverlight;component/Themes/Generic.xaml")]
    //[Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2010Black,
    //  Type = typeof(SpreadsheetControl), XamlResource = "/Syncfusion.Spreadsheet.Silverlight;component/Themes/Office2010Black.xaml")]
    //[Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2010Black,
    //  Type = typeof(SpreadsheetControl), XamlResource = "/Syncfusion.Spreadsheet.Silverlight;component/Themes/Office2010Silver.xaml")]
#else
    //[SkinType(SkinVisualStyle = Skin.Office2010Blue,
    //  Type = typeof(TileViewControl), XamlResource = "/Syncfusion.Spreadsheet.Wpf;component/Themes/Office2010Blue.xaml")]
    //[SkinType(SkinVisualStyle = Skin.Office2007Black,
    //Type = typeof(TileViewControl), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/ButtonControls/Button/Themes/Office2007BlackStyle.xaml")]
    //[SkinType(SkinVisualStyle = Skin.Office2007Silver,
    //Type = typeof(TileViewControl), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/ButtonControls/Button/Themes/Office2007SilverStyle.xaml")]
#endif

    public partial class SpreadsheetControl : System.Windows.Controls.Control, ISkinStylePropagator, IDisposable
    {
        #region Events
        public event WorkSheetAddingEventHandler WorkSheetAdding;
        public event WorkSheetAddedEventHandler WorkSheetAdded;
        public event WorkbookLoadedEventHandler WorkBookLoaded;
        #endregion

        #region Constructor
        private static readonly DependencyProperty SheetNameProperty;

        static SpreadsheetControl()
        {
            SheetNameProperty = DependencyProperty.RegisterAttached("SheetName", typeof(string), typeof(SpreadsheetControl), new PropertyMetadata(null));
        }

        internal static void SetSheetName(UIElement element, string value)
        {
            element.SetValue(SheetNameProperty, value);
        }

        internal static string GetSheetName(UIElement element)
        {
            return (string)element.GetValue(SheetNameProperty);
        }
#if SILVERLIGHT
        internal bool IsLoaded = false;
#else
        /// <summary>
        /// Checks whether security permission can be granted. Read-only.
        /// </summary>
        internal static bool IsSecurityGranted
        {
            get
            {
                SecurityPermission perm = new SecurityPermission(PermissionState.Unrestricted);
                bool bResult = false;
                try
                {
                    perm.Demand();
                    bResult = true;
                }
                catch (Exception) { }
                return bResult;
            }
        }

        /// <summary>
        /// Checks whether license is valid.
        /// </summary>
        internal static void ValidateLicense()
        {
            try
            {
                AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(AssemblyInfo.AssemblyResolver);
#if AllowUnsafeCode
                new Syncfusion.Core.Licensing.LicensedComponent(typeof(GridDataControl));
#endif
            }
            finally
            {
                AppDomain.CurrentDomain.AssemblyResolve -= new ResolveEventHandler(AssemblyInfo.AssemblyResolver);
            }
        }
#endif
        bool loadTabItemsOnLoad = false;
        public SpreadsheetControl()
        {
#if !SILVERLIGHT
            if (SpreadsheetControl.IsSecurityGranted)
            {
                SpreadsheetControl.ValidateLicense();
            }
#endif
            DefaultStyleKey = typeof(SpreadsheetControl);
            ExcelProperties = new ExcelProperties();
            GridProperties = new GridProperties();
            TabStyleManager = new TabStyleManager();
            ExportToExcelCommand = new ExportToExcelCommand(this);
            ImportFromExcelCommand = new ImportFromExcelCommand(this);
            ProtectWorkbookCommand = new ProtectWorkbookCommand(this);
            GroupOptionsCommand = new GroupOptionsCommand(this);
            GroupCommand = new GroupCommand(this);
            UngroupCommand = new UngroupCommand(this);
            DeleteCommentCommand = new DeleteCommentCommand(this);
            RowColumnHeadersVisiblityCommand = new RowColumnHeadersVisiblityCommand(this);
            HideSheetCommand = new HideSheetCommand(this);
            UnHideSheetCommand = new UnHideSheetCommand(this);
            DeleteSheetCommand = new DeleteSheetCommand(this);
            ProtectSheetCommand = new ProtectSheetCommand(this);
#if SILVERLIGHT
#else
            this.CommandBindings.Add(new System.Windows.Input.CommandBinding(ApplicationCommands.Copy, CommandExtensions.CommandExtensions.ExecuteCopyCommand, CommandExtensions.CommandExtensions.CanExecuteCopyCommand));
            this.CommandBindings.Add(new System.Windows.Input.CommandBinding(ApplicationCommands.Cut, CommandExtensions.CommandExtensions.ExecuteCutCommand, CommandExtensions.CommandExtensions.CanExecuteCutCommand));
            this.CommandBindings.Add(new System.Windows.Input.CommandBinding(ApplicationCommands.Paste, CommandExtensions.CommandExtensions.ExecutePasteCommand, CommandExtensions.CommandExtensions.CanExecutePasteCommand));
            SaveCommand = new SaveCommand(this);
            ExitCommand = new ExitCommand(this);
#endif
            BoldCommand = new BoldCommand(this);
            ItalicCommand = new ItalicCommand(this);
            UnderlineCommand = new UnderlineCommand(this);
            FontSizeCommand = new FontSizeCommand(this);
            FontColorCommand = new FontColorCommand(this);
            HorizontalAlignmentCommand = new HorizontalAlignmentCommand(this);
            VerticalAlignmentCommand = new VerticalAlignmentCommand(this);
            CellStyleCommand = new CellStyleCommand(this);
            InsertRowCommand =new InsertRowCommand(this);
            DeleteRowCommand = new DeleteRowCommand(this);
            InsertColumnCommand = new InsertColumnCommand(this);
            DeleteColumnCommand = new DeleteColumnCommand(this);
            MergeCommand = new MergeCommand(this);
            BorderCommand = new BorderCommand(this);
            ColumnWidthCommand = new ColumnWidthCommand(this);
            AutoFitCommand = new AutoFitCommand(this);
            HideRowCommand = new HideRowCommand(this);
            HideColumnCommand = new HideColumnCommand(this);
            InsertSheetCommand = new InsertSheetCommand(this);
            DeleteCurrentSheetCommand = new DeleteCurrentSheetCommand(this);
            FreezePaneCommand = new FreezePaneCommand(this);
            RowHeightCommand = new RowHeightCommand(this);
            ConditionalFormatCommand = new ConditionalFormatCommand(this);
            DataValidationCommand = new DataValidationCommand(this);
            FormatAsTableCommand = new FormatAsTableCommand(this);
            HyperlinkCommand = new HyperlinkCommand(this);
            InsertCommentCommand = new InsertCommentCommand(this);
            HideCurrentSheetCommand = new HideCurrentSheetCommand(this);
            CopyCommand = new CopyCommand(this);
            CutCommand = new CutCommand(this);
            PasteCommand = new PasteCommand(this);
            ShowGridLinesCommand = new ShowGridLinesCommand(this);
            NumberFormatCommand = new NumberFormatCommand(this);
            IncreaseIndentCommand = new IncreaseIndentCommand(this);
            IncreaseDecimalCommand = new IncreaseDecimalCommand(this);
            FillColorCommand = new FillColorCommand(this);
            FontFamilyCommand = new FontFamilyCommand(this);
            InsertPictureCommand = new InsertPictureCommand(this);
            ProtectCurrentSheetCommand = new ProtectCurrentSheetCommand(this);
            EncryptCommand = new EncryptCommand(this);
            NewCommand = new NewCommand(this);
            WrapTextCommand = new WrapTextCommand(this);
            GridCollection = new Dictionary<string, SpreadsheetGrid>();
            GridProperties.PropertyChanged+=new System.ComponentModel.PropertyChangedEventHandler(GridPropertiesPropertyChanged);
            ExcelProperties.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(ExcelProperties_PropertyChanged);
            CreateBlankDefaultWorkbook(3);
#if SILVERLIGHT
            this.Loaded += (s, e) =>
                {
                    IsLoaded = true;
                    if (IsFormulaBarVisibilityChanged)
                        RefreshFormulaBarRowDefinition();
                    OnStyleChanged(VisualStyle);
                };
            this.Unloaded += (s, e) =>
                {
                    IsLoaded = false;
                };
#else
            this.Loaded += (s, e) =>
                {
                    if (loadTabItemsOnLoad)
                        this.LoadTabItems();
                    UpdatedVisualStyle();
                    this.RefreshFormulaBarRowDefinition();
                };
#endif
        }

        void ExcelProperties_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals("IsWorkBookProtected"))
            {
                this.InsertSheetCommand.ExecuteChanged();
                this.DeleteCurrentSheetCommand.ExecuteChanged();
                this.HideCurrentSheetCommand.ExecuteChanged();
                this.DeleteSheetCommand.ExecuteChanged();
            }
            else if (e.PropertyName == "WorkBook")
            {
#if !SILVERLIGHT
                WireWorkSheetEvent();
#endif
                SpreadsheetGridCopyPaste.SourceRange = null;
            }
        }

#if !SILVERLIGHT
        private void WireWorkSheetEvent()
        {
            if (ExcelProperties.WorkBook != null && ExcelProperties.WorkBook.Worksheets.Count > 0)
            {
                foreach (IWorksheet sheet in ExcelProperties.WorkBook.Worksheets)
                {
                    sheet.CellValueChanged += new XlsIO.Implementation.RangeImpl.CellValueChangedEventHandler(OnCellValueChanged);
                }
            }
        }

        private void UnWireWorkSheetEvent()
        {
            if (ExcelProperties.WorkBook != null && ExcelProperties.WorkBook.Worksheets.Count > 0)
            {
                foreach (IWorksheet sheet in ExcelProperties.WorkBook.Worksheets)
                {
                    sheet.CellValueChanged -= new XlsIO.Implementation.RangeImpl.CellValueChangedEventHandler(OnCellValueChanged);
                }
            }
        }

        void OnCellValueChanged(object sender, XlsIO.Implementation.CellValueChangedEventArgs e)
        {
            string sheetName = this.GridProperties.CurrentSheetName;
            var grid = GridCollection[sheetName];
            if (grid != null && grid.CommittedCellRowColumnIndex != null && !grid.CommittedCellRowColumnIndex.IsEmpty && grid.CommittedCellRowColumnIndex.RowIndex == e.Range.Row && grid.CommittedCellRowColumnIndex.ColumnIndex == e.Range.Column)
                return;
            else
            {
                GridRangeInfo range = GridExcelHelper.ConvertExcelRangeToGridRange(e.Range);
                // Invalidate Cell when the Customer directly changes the value in the worksheet.
                GridProperties.CurrentExcelGridModel.InvalidateCell(range);
                if(GridProperties.CurrentExcelGridModel.Data.ContainsKey(new RowColumnIndex(range.Top,range.Left)))
                {
                    GridProperties.CurrentExcelGridModel.Data.Remove(new RowColumnIndex(range.Top, range.Left));
                }
            }
        }
#endif

        #endregion

        #region PrivateMembers
#if SILVERLIGHT
        TabControlAdv _excelTabControlAdv;
#else
        TabControlExt _excelTabControlAdv;
#endif
        #endregion

        #region Properties

        public void SetRowColumnHeaderVisibility(string sheetName, Visibility visibility)
        {
            if (CurrentSpreadsheetGrid != null)
            {
                IWorksheet worksheet = this.ExcelProperties.WorkBook.Worksheets[sheetName];
                if (sheetName == this.GridProperties.CurrentSheetName)
                {
                    worksheet.IsRowColumnHeadersVisible = visibility == System.Windows.Visibility.Visible ? true : false;
                    this.GridProperties.CurrentExcelGridModel.RowHeights.SetHidden(0, 0, visibility == System.Windows.Visibility.Visible ? false : true);
                    this.GridProperties.CurrentExcelGridModel.ColumnWidths.SetHidden(0, 0, visibility == System.Windows.Visibility.Visible ? false : true);
                    this.GridProperties.SpreadsheetGrid.IsRowHeaderVisible = visibility == System.Windows.Visibility.Visible ? true : false;
                    this.GridProperties.SpreadsheetGrid.IsColumnHeadersVisible = visibility == System.Windows.Visibility.Visible ? true : false;
                    this.GridProperties.CurrentExcelGridModel.InvalidateCell(GridRangeInfo.Col(0));
                    this.GridProperties.CurrentExcelGridModel.InvalidateCell(GridRangeInfo.Row(0));
                    this.GridProperties.IsRowColumnHeadersVisible = visibility == System.Windows.Visibility.Visible ? true : false;
                }
                else
                {
                    worksheet.IsRowColumnHeadersVisible = visibility == System.Windows.Visibility.Visible ? true : false;
                }
            }
        }

            

        public Visibility FormulaBarVisibility
        {
            get { return (Visibility)GetValue(FormulaBarVisibilityProperty); }
            set { SetValue(FormulaBarVisibilityProperty, value); }
        }

        public static readonly DependencyProperty FormulaBarVisibilityProperty =
            DependencyProperty.Register("FormulaBarVisibility", typeof(Visibility), typeof(SpreadsheetControl), new PropertyMetadata(Visibility.Collapsed,OnFormulaBarVisibilityChanged));

        private static void OnFormulaBarVisibilityChanged(DependencyObject obj,DependencyPropertyChangedEventArgs args)
        {
            SpreadsheetControl ctrl = obj as SpreadsheetControl;
            if (ctrl != null)
                ctrl.OnFormulaBarVisibilityChanged(args);
        }

#if SILVERLIGHT
        bool IsFormulaBarVisibilityChanged = false;
#endif
        protected virtual void OnFormulaBarVisibilityChanged(DependencyPropertyChangedEventArgs args)
        {
#if SILVERLIGHT
            IsFormulaBarVisibilityChanged = true;
#endif
            if (this.IsLoaded && this.MainGrid != null && this.MainGrid.RowDefinitions.Count > 0)
            {
                this.RefreshFormulaBarRowDefinition();
            }
        }

        private void RefreshFormulaBarRowDefinition()
        {
            if (this.IsLoaded && this.MainGrid != null && this.MainGrid.RowDefinitions.Count > 0)
            {
                this.MainGrid.RowDefinitions[0].Height = new GridLength(this.FormulaBarVisibility == System.Windows.Visibility.Visible ? 23.0 : 0.0);
#if SILVERLIGHT
                IsFormulaBarVisibilityChanged = false;
#endif
            }
        }


        //Grouping
        int outlineRowCount = 0;
        internal int OutlineRowCount
        {
            get
            {
                return outlineRowCount;
            }
            set
            {
                outlineRowCount = value;
                this.RefreshRowGroupPanel();
            }
        }

        int outlineColumnCount = 0;
        internal int OutlineColumnCount
        {
            get
            {
                return outlineColumnCount;
            }
            set
            {
                outlineColumnCount = value;
                this.RefreshColumnGroupPanel();
            }
        }
        
        public void RefreshRowGroupPanel()
        {
            if (this.IsLoaded && this.Part_Grid != null)
            {                                   
                this.Part_Grid.ColumnDefinitions[0].Width =this.OutlineRowCount>0 ? new GridLength((this.OutlineRowCount + 1) * 20):new GridLength(0);
                this.RowGroupPanel.InvalidateMeasure();
#if !SILVERLIGHT
                this.RowGroupPanel.InvalidateVisual();
#endif                                             
            }
        }

        public void RefreshColumnGroupPanel()
        {
            if (this.IsLoaded && this.Part_Grid != null)
            {
                this.Part_Grid.RowDefinitions[0].Height = this.OutlineColumnCount > 0 ? new GridLength((this.OutlineColumnCount + 1) * 20) : new GridLength(0);
                this.ColumnGroupPanel.InvalidateMeasure();
#if !SILVERLIGHT
                this.ColumnGroupPanel.InvalidateVisual();
#endif                                
            }
        }

        public GridProperties GridProperties
        {
            get { return (GridProperties)GetValue(GridPropertiesProperty); }
            set { SetValue(GridPropertiesProperty, value); }
        }

#if SILVERLIGHT
        public static readonly DependencyProperty GridPropertiesProperty =
            DependencyProperty.Register("GridProperties", typeof(GridProperties), typeof(SpreadsheetControl), new PropertyMetadata(new GridProperties()));
#else
        public static readonly DependencyProperty GridPropertiesProperty =
            DependencyProperty.Register("GridProperties", typeof(GridProperties), typeof(SpreadsheetControl));
#endif

        public ExcelProperties ExcelProperties
        {
            get { return (ExcelProperties)GetValue(ExcelPropertiesProperty); }
            internal set { SetValue(ExcelPropertiesProperty, value); }
        }

#if SILVERLIGHT
        public static readonly DependencyProperty ExcelPropertiesProperty =
            DependencyProperty.Register("ExcelProperties", typeof(ExcelProperties), typeof(SpreadsheetControl), new PropertyMetadata(new ExcelProperties()));
#else
        public static readonly DependencyProperty ExcelPropertiesProperty =
            DependencyProperty.Register("ExcelProperties", typeof(ExcelProperties), typeof(SpreadsheetControl));
#endif
        public int? TabSelectedIndex
        {
            get { return (int)GetValue(TabSelectedIndexProperty); }
            set { SetValue(TabSelectedIndexProperty, value); }
        }

        internal static readonly DependencyProperty TabSelectedIndexProperty =
            DependencyProperty.Register("TabSelectedIndex", typeof(int), typeof(SpreadsheetControl), new PropertyMetadata(OnSelectedIndexChanged));

        static void OnSelectedIndexChanged(DependencyObject dp,DependencyPropertyChangedEventArgs args)
        {
            //SpreadsheetControl editorControl = (SpreadsheetControl) dp;
            //string sheetname = string.Empty;
            //ExcelTabItem tabext = editorControl._excelTabControlAdv.SelectedItem != null ? editorControl._excelTabControlAdv.SelectedItem as ExcelTabItem : null;
            //if (tabext != null)
            //    sheetname = tabext.SheetName;

            //if (!string.IsNullOrEmpty(sheetname))
            //    editorControl.SetCurrentGridChanged(sheetname);
        }

        internal object TabSelectedItem
        {
            get { return GetValue(TabSelectedItemProperty); }
            set { SetValue(TabSelectedItemProperty, value); }
        }

        public static readonly DependencyProperty TabSelectedItemProperty =
            DependencyProperty.Register("TabSelectedItem", typeof (object), typeof (SpreadsheetControl), new PropertyMetadata(OnTabSelectedItemChanged));

        static void OnTabSelectedItemChanged(DependencyObject dp,DependencyPropertyChangedEventArgs args)
        {
            SpreadsheetControl editorControl = (SpreadsheetControl)dp;
            if (!editorControl.onLoadTabItems)
            {
                string sheetname = string.Empty;

#if !SILVERLIGHT
                TabItemExt tabext = args.NewValue as TabItemExt;
#else
            TabItemAdv tabext = args.NewValue as TabItemAdv;
#endif
                if (tabext != null)
                    sheetname = GetSheetName(tabext);

                if (!string.IsNullOrEmpty(sheetname))
                    editorControl.SetCurrentGridChanged(sheetname);
            }
        }

        protected virtual void SetCurrentGridChanged(string sheetName)
        {
            foreach (var item in this._excelTabControlAdv.Items)
            {
#if !SILVERLIGHT
                var tabItem = (TabItemExt)item;
#else 
                var tabItem = (TabItemAdv)item;
#endif

                string tabsheetName = GetSheetName(tabItem);
                if (tabItem != null && sheetName != null && sheetName.ToString().Equals(tabsheetName))
                    tabItem.IsSelected = true;
            }
            GridProperties.SetGridProperties(GridCollection[sheetName]);
        }

        public bool OptimizeFormulaCalculation
        {
            get { return (bool)GetValue(OptimizeFormulaCalculationProperty); }
            set { SetValue(OptimizeFormulaCalculationProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ImportDisplayText.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty OptimizeFormulaCalculationProperty =
            DependencyProperty.Register("OptimizeFormulaCalculation", typeof(bool), typeof(SpreadsheetControl), new PropertyMetadata(true,OnOptimizeFormulaCalculationChanged));

        static void OnOptimizeFormulaCalculationChanged(DependencyObject dp,DependencyPropertyChangedEventArgs args)
        {
            SpreadsheetControl editorControl = (SpreadsheetControl)dp;

            if (editorControl != null && editorControl.ModelCollection != null)
            {
                foreach (SpreadsheetGridModel gridModel in editorControl.ModelCollection)
                {
                    gridModel.FormulaEngine.ForceParsingOfLibraryFunctionArguments = (bool)args.NewValue;
                }
            }
        }

        /// <summary>
        /// Supports the Formula Range selection in Spreadsheet
        /// </summary>
        internal SpreadsheetFormulaRangeSelection FormulaRangeSelection
        {
            get { return (SpreadsheetFormulaRangeSelection)GetValue(FormulaRangeSelectionProperty); }
            set { SetValue(FormulaRangeSelectionProperty, value); }
        }
        public static readonly DependencyProperty FormulaRangeSelectionProperty =
            DependencyProperty.Register("FormulaRangeSelection", typeof(SpreadsheetFormulaRangeSelection), typeof(SpreadsheetControl), new PropertyMetadata(new SpreadsheetFormulaRangeSelection()));

        /// <summary>
        /// Enables or Disables the formula range selection in Spreadsheet
        /// </summary>
        public bool EnableFormulaRangeSelection
        {
            get { return (bool)GetValue(EnableFormulaRangeSelectionProperty); }
            set { SetValue(EnableFormulaRangeSelectionProperty, value); }
        }
        public static readonly DependencyProperty EnableFormulaRangeSelectionProperty =
            DependencyProperty.Register("EnableFormulaRangeSelection", typeof(bool), typeof(SpreadsheetControl), new PropertyMetadata(true));

        #endregion

        #region OnApplyTemplate
        System.Windows.Controls.Grid MainGrid;
        System.Windows.Controls.Grid Part_Grid;
        SpreadsheetGroupPanel RowGroupPanel;
        SpreadsheetGroupPanel ColumnGroupPanel;
        GridSplitter _Splitter;
        public override void OnApplyTemplate()
        {
            this.UnWireEvents();
            base.OnApplyTemplate();
#if SILVERLIGHT
            _excelTabControlAdv = GetTemplateChild("ExcelTabControlAdv") as TabControlAdv;
            EndEditButton = GetTemplateChild("EndEditButton") as RibbonButton;
            CancelEditButton = GetTemplateChild("CancelEditButton") as RibbonButton;
#else 
            _excelTabControlAdv = GetTemplateChild("ExcelTabControlAdv") as TabControlExt;
            _excelTabControlAdv.DefaultContextMenuItemVisibility = Visibility.Collapsed;
            EndEditButton = GetTemplateChild("EndEditButton") as ButtonAdv;
            CancelEditButton = GetTemplateChild("CancelEditButton") as ButtonAdv;
#endif
            NameBox = GetTemplateChild("NameBox") as TextBox;
            FormulaBar = GetTemplateChild("FormulaBar") as TextBox;
            _Splitter = GetTemplateChild("Splitter") as GridSplitter;
            MainGrid = GetTemplateChild("MainGrid") as System.Windows.Controls.Grid;
            Part_Grid = GetTemplateChild("Part_Grid") as System.Windows.Controls.Grid;
            RowGroupPanel = GetTemplateChild("RowGroupPanel") as SpreadsheetGroupPanel;
            ColumnGroupPanel = GetTemplateChild("ColumnGroupPanel") as SpreadsheetGroupPanel;
            
            RowGroupPanel.Control = this;
            ColumnGroupPanel.Control = this;

#if SILVERLIGHT
            FormulaBarGrid = GetTemplateChild("FormulaBarGrid") as System.Windows.Controls.Grid;
            UpdatedVisualStyle();
            if(MainGrid!= null)
                this.RefreshFormulaBarRowDefinition();
#endif
            if (ExcelProperties.WorkBook == null && _excelTabControlAdv != null)
            {
                CreateBlankDefaultWorkbook(3);
            }

            this.FormulaRangeSelection = new SpreadsheetFormulaRangeSelection(this, _excelTabControlAdv, FormulaBar);
            LoadTabItems();
            this.WireEvents();
        }
        #endregion

        #region OnGridPropertiesChanged
        SpreadsheetGrid CurrentSpreadsheetGrid = null;
        void GridPropertiesPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals("SpreadsheetGrid"))
            {
                CurrentSpreadsheetGrid = this.GridProperties.SpreadsheetGrid;
            }
            if (e.PropertyName.Equals("CurrentExcelGridModel"))
            {
                UpdateGridVisualStyle();
            }
            else if(CurrentSpreadsheetGrid != null && GridProperties.CurrentExcelGridModel != null)
            {

                this.HorizontalAlignmentCommand.ExecuteChanged();
                //IsEditing
                this.BorderCommand.ExecuteChanged();
                this.CellStyleCommand.ExecuteChanged();
                this.ColumnWidthCommand.ExecuteChanged();
                this.ConditionalFormatCommand.ExecuteChanged();
                this.DataValidationCommand.ExecuteChanged();
                this.DeleteColumnCommand.ExecuteChanged();
                this.DeleteCommentCommand.ExecuteChanged();
                this.DeleteRowCommand.ExecuteChanged();
                this.DeleteCurrentSheetCommand.ExecuteChanged();
                this.IncreaseIndentCommand.ExecuteChanged();
                this.FillColorCommand.ExecuteChanged();
                this.FormatAsTableCommand.ExecuteChanged();
                this.VerticalAlignmentCommand.ExecuteChanged();
                this.MergeCommand.ExecuteChanged();
                this.HyperlinkCommand.ExecuteChanged();
                this.ShowGridLinesCommand.ExecuteChanged();
                this.InsertPictureCommand.ExecuteChanged();
                this.DeleteCommentCommand.ExecuteChanged();
                this.InsertCommentCommand.ExecuteChanged();
                this.ProtectCurrentSheetCommand.ExecuteChanged();
                this.ProtectWorkbookCommand.ExecuteChanged();
                this.WrapTextCommand.ExecuteChanged();
                this.RowHeightCommand.ExecuteChanged();
                this.InsertRowCommand.ExecuteChanged();
                this.InsertColumnCommand.ExecuteChanged();
                this.InsertColumnCommand.ExecuteChanged();
                this.InsertRowCommand.ExecuteChanged();
                this.HideColumnCommand.ExecuteChanged();
                this.HideRowCommand.ExecuteChanged();
                this.RowHeightCommand.ExecuteChanged();
                this.GroupCommand.ExecuteChanged();
                this.UngroupCommand.ExecuteChanged();
            }          

            if (e.PropertyName.Equals("SelectedRange"))
            {
                IWorksheet worksheet = ExcelProperties.WorkBook.Worksheets[GridProperties.CurrentSheetName];
                if (worksheet != null)
                {
                    string range = GridProperties.SelectedRange.ActiveRange.ConvertGridRangeToExcelRange(GridProperties.CurrentExcelGridModel);
                    if (!string.IsNullOrEmpty(range))
                        ExcelProperties.CurrentExcelRangeStyle = worksheet.Range[range];
                }
            }
          
        }

        #endregion

        #region StyleManager
        public TabStyleManager TabStyleManager
        {
            get { return (TabStyleManager)GetValue(TabStyleManagerProperty); }
            set { SetValue(TabStyleManagerProperty, value); }
        }

#if SILVERLIGHT
        public static readonly DependencyProperty TabStyleManagerProperty =
            DependencyProperty.Register("TabStyleManager", typeof(TabStyleManager), typeof(SpreadsheetControl), new PropertyMetadata(new TabStyleManager()));
#else
        public static readonly DependencyProperty TabStyleManagerProperty =
            DependencyProperty.Register("TabStyleManager", typeof(TabStyleManager), typeof(SpreadsheetControl),new PropertyMetadata(null));
#endif
        #endregion

        #region Events
#if SILVERLIGHT
        internal bool RaiseCurrentCellValidating(CurrentCellValidateEventArgs e)
        {
            OnCurrentCellValidating(e);
            return e.Cancel;
        }

        protected virtual void OnCurrentCellValidating(CurrentCellValidateEventArgs e)
        {
            if (CurrentCellValidating != null)
                CurrentCellValidating(this, e);
        }

        public event CurrentCellValidateEventHandler CurrentCellValidating;
#else
        internal bool RaiseCurrentCellValidating(CurrentCellValidatingEventArgs e)
        {
            OnCurrentCellValidating(e);
            return e.Cancel;
        }

        protected virtual void OnCurrentCellValidating(CurrentCellValidatingEventArgs e)
        {
            if (CurrentCellValidating != null)
                CurrentCellValidating(this, e);
        }

        public event CurrentCellValidatingEventHandler CurrentCellValidating;
#endif

        public event CellRequestNavigateEventHandler CellRequestNavigate;

        protected virtual void OnCellRequestNavigate(CellRequestNavigateEventArgs e)
        {
            if (CellRequestNavigate != null)
                CellRequestNavigate(this, e);
        }

        internal bool RaiseCellRequestNavigate(CellRequestNavigateEventArgs e)
        {
            OnCellRequestNavigate(e);
            if (e.Handled) return true;

            string name = e.Name;
#if !SILVERLIGHT
            TabItemExt moveto = null;
#else
            TabItemAdv moveto = null;
#endif
            bool isSuccess = false;
#if !SILVERLIGHT
            foreach (TabItemExt item in _excelTabControlAdv.Items)
#else
            foreach (TabItemAdv item in _excelTabControlAdv.Items)
#endif
            {
                if (SpreadsheetControl.GetSheetName(item) == name)
                {
                    moveto = item;
                    isSuccess = true;
                    break;
                }
            }
            if (isSuccess)
            {
                if (moveto.Visibility == System.Windows.Visibility.Visible)
                {
                    TabSelectedItem = moveto;
                    SpreadsheetGrid gridctrl = GridCollection[name];
                    if (gridctrl != null && e.RowIndex > 0 && e.ColumnIndex > 0)
                    {
                        gridctrl.CurrentCell.MoveTo(e.RowIndex, e.ColumnIndex);
                        gridctrl.CurrentCell.ScrollInView();
                    }
                }
            }
            else if (string.IsNullOrEmpty(e.Name) && e.Uri == null)
            {
                MessageBox.Show(SpreadsheetResourceWrapper.ValidationMessage_SheetNotFound, SpreadsheetResourceWrapper.ValidationMessage_Error, MessageBoxButton.OK);
                this.Focus();
            }
            return isSuccess;
        }


        #endregion

        #region FormulaBar

        TextBox NameBox = null;
        internal TextBox FormulaBar = null;
        BindingExpression NameBoxTextBinding = null;
#if !SILVERLIGHT
        ButtonAdv EndEditButton = null;
        ButtonAdv CancelEditButton = null;
#else
        System.Windows.Controls.Grid FormulaBarGrid;
        RibbonButton EndEditButton = null;
        RibbonButton CancelEditButton = null;
        BindingExpression FormulaBarTextBinding = null;
        bool IsFormulaBarEditing;
#endif

        private void WireEvents()
        {
            if (NameBox != null)
#if !SILVERLIGHT
                NameBox.PreviewKeyDown += new KeyEventHandler(NameBoxPreviewKeyDown);
#else
                NameBox.KeyDown += new KeyEventHandler(NameBoxPreviewKeyDown);
#endif
            if (FormulaBar != null)
            {
#if !SILVERLIGHT
                FormulaBar.PreviewKeyDown += new KeyEventHandler(FormulaBarPreviewKeyDown);
#else
                FormulaBar.KeyDown += new KeyEventHandler(FormulaBarPreviewKeyDown);
                FormulaBar.TextChanged += new TextChangedEventHandler(FormulaBarTextChanged);
#endif
                FormulaBar.GotFocus += new RoutedEventHandler(FormulaBarGotFocus);
                FormulaBar.LostFocus += new RoutedEventHandler(FormulaBarLostFocus);
            }
            if (EndEditButton != null)
                this.EndEditButton.Click += new RoutedEventHandler(EndEditButton_Click);
            if (this.CancelEditButton != null)
                this.CancelEditButton.Click += new RoutedEventHandler(CancelEditButton_Click);
        }

        private void UnWireEvents()
        {
            if (NameBox != null)
#if !SILVERLIGHT
                NameBox.PreviewKeyDown -= new KeyEventHandler(NameBoxPreviewKeyDown);
#else
                NameBox.KeyDown -= new KeyEventHandler(NameBoxPreviewKeyDown);
#endif
            if (FormulaBar != null)
            {
#if !SILVERLIGHT
                FormulaBar.PreviewKeyDown -= new KeyEventHandler(FormulaBarPreviewKeyDown);
#else
                FormulaBar.KeyDown -= new KeyEventHandler(FormulaBarPreviewKeyDown);
                FormulaBar.TextChanged -= new TextChangedEventHandler(FormulaBarTextChanged);
#endif
                FormulaBar.GotFocus -= new RoutedEventHandler(FormulaBarGotFocus);
                FormulaBar.LostFocus -= new RoutedEventHandler(FormulaBarLostFocus);
            }
            if (EndEditButton != null)
                this.EndEditButton.Click -= new RoutedEventHandler(EndEditButton_Click);
            if (this.CancelEditButton != null)
                this.CancelEditButton.Click -= new RoutedEventHandler(CancelEditButton_Click);

            if (this.CurrentSpreadsheetGrid != null)
                UnhookScrollbarEvent(this.CurrentSpreadsheetGrid.ScrollOwner);
            
        }

        private void EndEditCurrentCell()
        {
            if (CurrentSpreadsheetGrid != null && !this.GridProperties.IsFocusedOnGraphicCells)
            {
                if (this.FormulaRangeSelection.IsInFormulaEditing)
                    this.FormulaRangeSelection.editingGrid.CurrentCell.EndEdit();

                if (CurrentSpreadsheetGrid.CurrentCell != null)
                    CurrentSpreadsheetGrid.CurrentCell.EndEdit();
            }
            this.GridProperties.IsEditing = false;
            this.RefreshFormulaBar();
        }

        private void CancelEditCurrentCell()
        {
            if (CurrentSpreadsheetGrid != null && !this.GridProperties.IsFocusedOnGraphicCells)
            {
                if (this.FormulaRangeSelection.IsInFormulaEditing)
                    this.FormulaRangeSelection.editingGrid.CurrentCell.CancelEdit();

                if (CurrentSpreadsheetGrid.CurrentCell != null)
                    CurrentSpreadsheetGrid.CurrentCell.CancelEdit();
                this.RefreshFormulaBar();
            }
            this.GridProperties.IsEditing = false;
        }

        internal void RefreshFormulaBar()
        {
            if (CurrentSpreadsheetGrid != null && FormulaBar != null && this.GridProperties != null)
            {
                this.GridProperties.RefreshCurrentStyle();
            }
        }

        void NameBoxPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (NameBoxTextBinding == null)
                    NameBoxTextBinding = NameBox.GetBindingExpression(TextBox.TextProperty);
                if (NameBoxTextBinding != null)
                    NameBoxTextBinding.UpdateSource();
                this.Focus();
            }
        }

#if SILVERLIGHT
        void FormulaBarTextChanged(object sender, TextChangedEventArgs e)
        {
            if (!this.CurrentSpreadsheetGrid.CurrentCell.IsEditing && IsFormulaBarEditing)
            {
                if (FormulaBarTextBinding == null)
                    FormulaBarTextBinding = FormulaBar.GetBindingExpression(TextBox.TextProperty);
                if (FormulaBarTextBinding != null && GridProperties.IsEditing)
                    FormulaBarTextBinding.UpdateSource();
            }
        }
#endif
        void FormulaBarPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
#if !SILVERLIGHT
                if (this.GridProperties.IsFocusedOnGraphicCells)
                {
                    int i = this.GridProperties.CurrentExcelGridModel.GraphicModel.SelectedGraphicCells.Count - 1;
                    var span = this.GridProperties.CurrentExcelGridModel.GraphicModel.SelectedGraphicCells[i];
                    this.GridProperties.CurrentExcelGridModel.GraphicModel[span.CellSpanIndex].GraphicCellControl.Focus();
                    e.Handled = true;
                }
#endif
                EndEditCurrentCell();
            }
            else if (e.Key == Key.Escape)
            {
#if !SILVERLIGHT
                if (this.GridProperties.IsFocusedOnGraphicCells)
                {
                    int i = this.GridProperties.CurrentExcelGridModel.GraphicModel.SelectedGraphicCells.Count - 1;
                    var span = this.GridProperties.CurrentExcelGridModel.GraphicModel.SelectedGraphicCells[i];
                    this.GridProperties.CurrentExcelGridModel.GraphicModel[span.CellSpanIndex].GraphicCellControl.Focus();
                    e.Handled = true;
                }
#endif
                CancelEditCurrentCell();
            }
            else if (this.GridProperties.IsFocusedOnGraphicCells)
            {
                MessageBox.Show("Reference is not valid");
                e.Handled = true;
            }
        }

        void FormulaBarGotFocus(object sender, RoutedEventArgs e)
        {
            GridProperties.IsEditing = true;
            GridProperties.CurrentExcelGridModel.SuspendCurrentCellFormulaParsing();
#if SILVERLIGHT
            IsFormulaBarEditing = true;
#endif
        }

        void FormulaBarLostFocus(object sender, RoutedEventArgs e)
        {
            GridProperties.CurrentExcelGridModel.ResumeCurrentCellFormulaParsing();
#if SILVERLIGHT
            IsFormulaBarEditing = false;
#endif
        }
        
        void CancelEditButton_Click(object sender, RoutedEventArgs e)
        {
            this.CancelEditCurrentCell();
        }

        void EndEditButton_Click(object sender, RoutedEventArgs e)
        {
            this.EndEditCurrentCell();
        }
#if FormulaBar
        void FormulaBar_KeyDown(object sender, KeyEventArgs e)
        {
            
        }

        void spreadsheetGrid_CurrentCellMoved(object sender, GridCurrentCellMovedEventArgs args)
        {
            inCurrentCellMoved = true;
            RefreshFormulaBar();
            inCurrentCellMoved = false;
        }

        void spreadsheetGrid_CurrentCellChanged(object sender, ComponentModel.SyncfusionRoutedEventArgs args)
        {
            if (inTextChanged || inCurrentCellMoved)
                return;
            inCurrentCellChanged = true;
            RefreshFormulaBar();
            inCurrentCellChanged = false;
        }

        private void RefreshFormulaBar()
        {
            if (CurrentSpreadsheetGrid != null && FormulaBar != null)
            {
                if (CurrentSpreadsheetGrid.CurrentCell != null)
                    FormulaBar.Text = CurrentSpreadsheetGrid.Model[CurrentSpreadsheetGrid.CurrentCell.CellRowColumnIndex.RowIndex, CurrentSpreadsheetGrid.CurrentCell.CellRowColumnIndex.ColumnIndex].Text;
            }
        }

        void formulaCell_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (inCurrentCellChanged || inCurrentCellMoved)
                return;

            inTextChanged = true;

            if (CurrentSpreadsheetGrid != null)
            {
                //GridCurrentCell cc = CurrentSpreadsheetGrid.CurrentCell;
                //cc.Renderer.SetControlText(FormulaBar.Text, false);
                //cc.EndEdit();
                //cc.Renderer.RaiseBeginEdit();
                //cc.Renderer.RaiseStartEditing();
                //cc.Renderer.RefreshContent();
                //cc.Renderer.ResetControlText();
                //cc.Renderer.ResetControlValue();
                //cc.UnloadCurrentCellUIElement();
                CurrentSpreadsheetGrid.InvalidateVisual();
            }
            inTextChanged = false;
        }

        void formulaCell_PreviewKeyDown(object sender, KeyEventArgs e)
        {
#if !SILVERLIGHT
            if (e.Key == System.Windows.Input.Key.Return)
#else
            if (e.Key == System.Windows.Input.Key.Enter)
#endif
            {
                e.Handled = true;
                CurrentSpreadsheetGrid.CurrentCell.MoveDown();
            }
        }

        void formulaCell_LostFocus(object sender, RoutedEventArgs e)
        {
            this.EndEditCurrentCell();
        }

        void formulaCell_GotFocus(object sender, RoutedEventArgs e)
        {
            this.BeginEditCurrentCell();
        }
#endif
        #endregion

        #region DataTableImport
#if !SILVERLIGHT
        /// <summary>
        /// Imports from data table.
        /// </summary>
        /// <param name="table">The table.</param>
        public void ImportFromDataTable(DataTable table)
        {
            this.ExcelProperties.WorkBook.Worksheets[0].ImportDataTable(table,true,1,1);
            this.CurrentSpreadsheetGrid.InvalidateCells();
            this.CurrentSpreadsheetGrid.InvalidateVisual();
        }

        /// <summary>
        /// Imports from data table.
        /// </summary>
        /// <param name="table">The table.</param>
        /// <param name="isFieldNameShow">if set to <c>true</c> [is field name show].</param>
        /// <param name="startRow">The start row.</param>
        /// <param name="startCol">The start col.</param>
        public void ImportFromDataTable(DataTable table, bool isFieldNameShow, int startRow, int startCol)
        {
            this.ExcelProperties.WorkBook.Worksheets[0].ImportDataTable(table, isFieldNameShow, startRow, startCol);
            this.CurrentSpreadsheetGrid.InvalidateCells();
            this.CurrentSpreadsheetGrid.InvalidateVisual();
        }
        /// <summary>
        /// Imports from data table.
        /// </summary>
        /// <param name="table">The table.</param>
        /// <param name="isFieldNameShow">if set to <c>true</c> [is field name show].</param>
        /// <param name="startRow">The start row.</param>
        /// <param name="startCol">The start col.</param>
        /// <param name="preserveTypes">if set to <c>true</c> [preserve types].</param>
        public void ImportFromDataTable(DataTable table, bool isFieldNameShow, int startRow, int startCol, bool preserveTypes)
        {
            this.ExcelProperties.WorkBook.Worksheets[0].ImportDataTable(table, isFieldNameShow, startRow, startCol, preserveTypes);
            this.CurrentSpreadsheetGrid.InvalidateCells();
            this.CurrentSpreadsheetGrid.InvalidateVisual();
        }

        /// <summary>
        /// Imports from data table.
        /// </summary>
        /// <param name="table">The table.</param>
        /// <param name="isFieldNameShow">if set to <c>true</c> [is field name show].</param>
        /// <param name="startRow">The start row.</param>
        /// <param name="startCol">The start col.</param>
        /// <param name="maxRow">The max row.</param>
        /// <param name="maxCol">The max col.</param>
        public void ImportFromDataTable(DataTable table, bool isFieldNameShow, int startRow, int startCol, int maxRow, int maxCol)
        {
            this.ExcelProperties.WorkBook.Worksheets[0].ImportDataTable(table, isFieldNameShow, startRow, startCol, maxRow, maxCol);
            this.CurrentSpreadsheetGrid.InvalidateCells();
            this.CurrentSpreadsheetGrid.InvalidateVisual();
        }

        /// <summary>
        /// Imports from data table.
        /// </summary>
        /// <param name="table">The table.</param>
        /// <param name="isFieldNameShow">if set to <c>true</c> [is field name show].</param>
        /// <param name="startRow">The start row.</param>
        /// <param name="startCol">The start col.</param>
        /// <param name="maxRow">The max row.</param>
        /// <param name="maxCol">The max col.</param>
        /// <param name="preserveTypes">if set to <c>true</c> [preserve types].</param>
        public void ImportFromDataTable(DataTable table, bool isFieldNameShow, int startRow, int startCol, int maxRow, int maxCol, bool preserveTypes)
        {
            this.ExcelProperties.WorkBook.Worksheets[0].ImportDataTable(table, isFieldNameShow, startRow, startCol, maxRow, maxCol, preserveTypes);
            this.CurrentSpreadsheetGrid.InvalidateCells();
            this.CurrentSpreadsheetGrid.InvalidateVisual();
        }



#endif
    
        #endregion

        #region PrivateMethod
        /// <summary>
        /// Create blank Workbook with default setting
        /// </summary>
        private void CreateBlankDefaultWorkbook(int Count)
        {
            IWorkbook workbook = ExcelProperties.Application.Workbooks.Create(Count);
            ExcelProperties.WorkBook = workbook;
            ExcelProperties.spreadControl = this;
            this.ExcelProperties.ChangedXMLCellList = new XElement("ChangedCells");
            if (this.ExcelProperties.ChangedCellList == null || CurrentSpreadsheetGrid == null)
                this.ExcelProperties.ChangedCellList = new BinaryList();
            else 
                CurrentSpreadsheetGrid.CreateXElement();
            
            string lastCell = GridRangeInfo.GetAlphaLabel(ExcelProperties.DefaultColumnCount) + ExcelProperties.DefaultRowCount;
            string defaultRange = "A1:" + lastCell;
            foreach (IWorksheet worksheets in ExcelProperties.WorkBook.Worksheets)
            {
                if (ExcelProperties.DefaultRowHeight != 15)
                    worksheets.Range[defaultRange].RowHeight = ExcelProperties.DefaultRowHeight;
                worksheets.Range[defaultRange].ColumnWidth = ExcelProperties.DefaultColumnWidth;
                worksheets.Range[defaultRange].CellStyle.Font.FontName = "Tahoma";
                worksheets.Range[defaultRange].CellStyle.Font.Size = 10.0;
            }
        }
        #endregion

        #region VisualStyle

#if !SILVERLIGHT
        internal string VisualStyle
        {
            get { return (string)GetValue(VisualStyleProperty); }
            set { SetValue(VisualStyleProperty, value); }
        }

        internal static readonly DependencyProperty VisualStyleProperty =
            DependencyProperty.Register("VisualStyle", typeof(string), typeof(SpreadsheetControl), new UIPropertyMetadata("Office2010Blue"));

        public void OnStyleChanged(string visualStyle)
        {
            this.VisualStyle = visualStyle;
            UpdatedVisualStyle();
        }  
        
#else
        public void OnStyleChanged(Theming.VisualStyle visualStyle)
        {
            this.VisualStyle = visualStyle;
        }

        public Syncfusion.Windows.Controls.Theming.VisualStyle VisualStyle
        {
            get { return (Syncfusion.Windows.Controls.Theming.VisualStyle)GetValue(VisualStyleProperty); }
            set { SetValue(VisualStyleProperty, value); }
        }

        public static readonly DependencyProperty VisualStyleProperty =
            DependencyProperty.Register("VisualStyle", typeof(Syncfusion.Windows.Controls.Theming.VisualStyle), typeof(SpreadsheetControl), new PropertyMetadata(Syncfusion.Windows.Controls.Theming.VisualStyle.Office2010Blue,OnVisualStylePropertyChanged));
#endif
        private static void OnVisualStylePropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            SpreadsheetControl control = obj as SpreadsheetControl;
            if (control != null)
                control.UpdatedVisualStyle();
        }

        private void UpdatedVisualStyle()
        {
            if (_excelTabControlAdv == null)
                return;
            if (this.VisualStyle.ToString() == "Office2010Blue")
                this.TabStyleManager.TabVisualStyle = TabVisualStyle.ExcelBlue;
            else if (this.VisualStyle.ToString() == "Office2010Silver")
                TabStyleManager.TabVisualStyle = TabVisualStyle.ExcelSilver;
            else if (this.VisualStyle.ToString() == "Office2010Black")
                TabStyleManager.TabVisualStyle = TabVisualStyle.ExcelBlack;
            else
                TabStyleManager.TabVisualStyle = TabVisualStyle.None;
#if !SILVERLIGHT
            SkinStorage.SetVisualStyle(_excelTabControlAdv, VisualStyle);
#else
            if (!System.ComponentModel.DesignerProperties.IsInDesignTool)
                if (FormulaBarGrid != null)
                    SkinManager.SetVisualStyle(FormulaBarGrid, VisualStyle);
            //    SkinManager.SetVisualStyle(_excelTabControlAdv, VisualStyle);
#endif
            UpdateGridVisualStyle();

            if (this.VisualStyle.ToString() == "Office2010Blue" || this.VisualStyle.ToString() == "Office2007Blue")
            {
                this.Background = new SolidColorBrush(Color.FromArgb(255, 218, 231, 245));
                this.BorderBrush = new SolidColorBrush(Color.FromArgb(255, 169, 186, 212));
            }
            else if (this.VisualStyle.ToString() == "Office2010Silver" || this.VisualStyle.ToString() == "Office2007Silver")
            {
                this.Background = new SolidColorBrush(Color.FromArgb(255, 223, 227, 232));
                this.BorderBrush = new SolidColorBrush(Color.FromArgb(255, 189, 194, 199));
            }
            else if (this.VisualStyle.ToString() == "Office2010Black" || this.VisualStyle.ToString() == "Office2010Black")
            {
                this.Background = new SolidColorBrush(Color.FromArgb(255, 106, 106, 106));
                this.BorderBrush = new SolidColorBrush(Color.FromArgb(255, 73, 73, 73));
            }
        }

        private void UpdateGridVisualStyle()
        {
            if (this.GridProperties != null && this.GridProperties.CurrentExcelGridModel != null)
            {
                if (this.VisualStyle.ToString() == "Office2010Blue")
                {
                    var Office2010BlueStyle = new Office2010BlueSpreadsheetGridVisualStyle();
                    this.GridProperties.CurrentExcelGridModel.SpreadsheetGridVisualStyle = Office2010BlueStyle;
                    this.GridProperties.CurrentExcelGridModel.HeaderStyle.Background = Office2010BlueStyle.HeaderBackgroundBrush;
                    this.GridProperties.CurrentExcelGridModel.HeaderStyle.Foreground = Office2010BlueStyle.HeaderForegroundBrush;
                    this.GridProperties.CurrentExcelGridModel.HeaderStyle.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
                    this.GridProperties.CurrentExcelGridModel.HeaderStyle.VerticalAlignment = System.Windows.VerticalAlignment.Center;
                }
                else if (this.VisualStyle.ToString() == "Office2010Silver")
                {
                    var Office2010SilverStyle = new Office2010SilverSpreadsheetGridVisualStyle();
                    this.GridProperties.CurrentExcelGridModel.SpreadsheetGridVisualStyle = Office2010SilverStyle;
                    this.GridProperties.CurrentExcelGridModel.HeaderStyle.Background = Office2010SilverStyle.HeaderBackgroundBrush;
                    this.GridProperties.CurrentExcelGridModel.HeaderStyle.Foreground = Office2010SilverStyle.HeaderForegroundBrush;
                    this.GridProperties.CurrentExcelGridModel.HeaderStyle.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
                    this.GridProperties.CurrentExcelGridModel.HeaderStyle.VerticalAlignment = System.Windows.VerticalAlignment.Center;
                }
                else if (this.VisualStyle.ToString() == "Office2010Black")
                {
                    var Office2010Black = new Office2010BlackSpreadsheetGridVisualStyle();
                    this.GridProperties.CurrentExcelGridModel.SpreadsheetGridVisualStyle = Office2010Black;
                    this.GridProperties.CurrentExcelGridModel.HeaderStyle.Background = Office2010Black.HeaderBackgroundBrush;
                    this.GridProperties.CurrentExcelGridModel.HeaderStyle.Foreground = Office2010Black.HeaderForegroundBrush;
                    this.GridProperties.CurrentExcelGridModel.HeaderStyle.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
                    this.GridProperties.CurrentExcelGridModel.HeaderStyle.VerticalAlignment = System.Windows.VerticalAlignment.Center;
                }
                this.GridProperties.CurrentExcelGridModel.InvalidateCell(GridRangeInfo.Col(0));
                this.GridProperties.CurrentExcelGridModel.InvalidateCell(GridRangeInfo.Row(0));
            }
        }

        #endregion

        #region Dispose Method

        public void Dispose()
        {
            foreach (var item in this.GridCollection)
            {
                this.GridProperties.UnWireGridEvents(item.Value);
                this.FormulaRangeSelection.UnHookEvents(item.Value);
#if SILVERLIGHT
                item.Value.Dispose();
#else
                item.Value.Dispose(true);
#endif
            }
#if !SILVERLIGHT
            UnWireWorkSheetEvent();
#endif
            GridProperties.PropertyChanged -= new System.ComponentModel.PropertyChangedEventHandler(GridPropertiesPropertyChanged);
            ExcelProperties.PropertyChanged -= new System.ComponentModel.PropertyChangedEventHandler(ExcelProperties_PropertyChanged);
            ExcelProperties.WorkBook.Close();
            ExcelProperties.ExcelEngine.Dispose();
            this.FormulaRangeSelection.Dispose();
#if SILVERLIGHT
            foreach (TabItemAdv tabitem in _excelTabControlAdv.Items)
            {
                tabitem.Content = null;
            }
#else
            foreach (TabItemExt tabitem in _excelTabControlAdv.Items)
            {
#if !SILVERLIGHT
                tabitem.ContextMenuOpening -= new ContextMenuEventHandler(tab_ContextMenuOpening);
#endif
                tabitem.Content = null;
            }
#endif
            _excelTabControlAdv.Items.Clear();
        }
        #endregion
    }
}
