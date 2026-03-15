#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Syncfusion.Windows.Controls.Grid;
using System.ComponentModel;
using Syncfusion.XlsIO;
using System.Collections;
using System.Linq;
using System.Globalization;
using System.Text.RegularExpressions;
using Syncfusion.XlsIO.Implementation;

namespace Syncfusion.Windows.Controls.Spreadsheet
{
    public class GridProperties : DependencyObject, INotifyPropertyChanged
    {
        public GridProperties()
        {
            
        }

        internal void SetGridProperties(SpreadsheetGrid grid)
        {
            this.UnWireGridEvents(SpreadsheetGrid);
            this.SpreadsheetGrid = grid;
            CurrentExcelGridModel = grid.Model as SpreadsheetGridModel;

            this.CurrentExcelGridModel.CommandStack.SuspendUndo = true;

            CurrentSheetName = grid.SheetName;
            ShowGridLines = grid.ShowGridLines;
            IWorksheet worksheet = this.SpreadsheetGrid.ExcelProperties.WorkBook.Worksheets[this.SpreadsheetGrid.SheetName];
            if (worksheet != null)
            {
                worksheet.Activate();
                this.IsRowColumnHeadersVisible = worksheet.IsRowColumnHeadersVisible;
            }
            if (grid.CurrentCell != null && !grid.CurrentCell.HasCurrentCell)
            {
                grid.CurrentCell.MoveTo(1, 1);
                grid.CurrentCell.ScrollInView();
                SelectedRange = grid.Model.SelectedRanges;
            }
            CurrentCell = grid.CurrentCell;
            if (CurrentCell != null)
            {
                CurrentCellStyle = HasCurrentExcelGrid ? CurrentExcelGridModel[CurrentCell.RowIndex, CurrentCell.ColumnIndex] : null;
                IsEditing = CurrentCell.IsEditing;
            }

            if (worksheet!=null && worksheet.IsPasswordProtected)
                this.SpreadsheetGrid.ExcelProperties.IsPasswordProtected = true;
            else
                this.SpreadsheetGrid.ExcelProperties.IsPasswordProtected = false;

            //Grouping                        
            this.SpreadsheetGrid.ExcelProperties.spreadControl.OutlineRowCount = (this.SpreadsheetGrid.ExcelProperties.WorkBook.ActiveSheet as WorksheetImpl).RowsOutlineLevel;
            this.SpreadsheetGrid.ExcelProperties.spreadControl.OutlineColumnCount = (this.SpreadsheetGrid.ExcelProperties.WorkBook.ActiveSheet as WorksheetImpl).ColumnsOutlineLevel;

#if !SILVERLIGHT
            if (this.CurrentExcelGridModel.GraphicModel.SelectedGraphicCells.Count > 0)
                this.IsFocusedOnGraphicCells = true;
            else
                this.IsFocusedOnGraphicCells = false;
#endif
            if (this.CurrentExcelGridModel.FrozenColumns > 1 || this.CurrentExcelGridModel.FooterRows > 1)
                IsFrozen = true;
            else
                IsFrozen = false;
            this.WireGridEvents(SpreadsheetGrid);

            this.CurrentExcelGridModel.CommandStack.SuspendUndo = false;
        }
        
        internal void RefreshCurrentStyle()
        {
            if (CurrentCell != null)
            {
                CurrentCellStyle = HasCurrentExcelGrid ? CurrentExcelGridModel[CurrentCell.RowIndex, CurrentCell.ColumnIndex] : null;
            }
        }
        bool _isfrozen;
        public bool IsFrozen
        {
            get
            {
                return _isfrozen;
            }
            set
            {
                _isfrozen = value;              
                OnPropertyChanged("IsFrozen");
            }
        }

        bool _isfocusedOnGraphicCells = false;
#if !SILVERLIGHT
        public bool IsFocusedOnGraphicCells
#else
        internal bool IsFocusedOnGraphicCells
#endif
        {
            get
            {
                return _isfocusedOnGraphicCells;
            }
            set
            {
                _isfocusedOnGraphicCells = value;
                if (_isfocusedOnGraphicCells)
                {
                    IsDisableMode = true;
                }
                else if (!this._CurrentGridModel.ExcelProperties.IsPasswordProtected
                        && !this.CurrentExcelGridModel.ExcelProperties.spreadControl.FormulaRangeSelection.IsInFormulaEditing)
                    IsDisableMode = false;

                OnPropertyChanged("IsFocusedOnGraphicCells");
            }
        }

        #region WireAndUnWireEvents
        internal void WireGridEvents(SpreadsheetGrid grid)
        {
            if (grid == null)
                return;

            grid.CurrentCellActivated += new ComponentModel.GridRoutedEventHandler(grid_CurrentCellActivated);
            grid.SelectionChanged += new GridSelectionChangedEventHandler(grid_SelectionChanged);
            grid.CurrentCellStartEditing += new ComponentModel.GridCancelRoutedEventHandler(grid_CurrentCellStartEditing);
            grid.CurrentCellEditingComplete += new ComponentModel.GridRoutedEventHandler(grid_CurrentCellEditingComplete);
            grid.CurrentCellChanged += new ComponentModel.GridRoutedEventHandler(grid_CurrentCellChanged);
#if SILVERLIGHT
            grid.ShowGridLinesChanged += new DependencyPropertyChangedEventHandler(grid_ShowGridLinesChanged);
#endif
        }

        internal void UnWireGridEvents(SpreadsheetGrid grid)
        {
            if (grid == null)
                return;

            grid.CurrentCellActivated -= new ComponentModel.GridRoutedEventHandler(grid_CurrentCellActivated);
            grid.SelectionChanged -= new GridSelectionChangedEventHandler(grid_SelectionChanged);
            grid.CurrentCellStartEditing -= new ComponentModel.GridCancelRoutedEventHandler(grid_CurrentCellStartEditing);
            grid.CurrentCellEditingComplete -= new ComponentModel.GridRoutedEventHandler(grid_CurrentCellEditingComplete);
            grid.CurrentCellChanged -= new ComponentModel.GridRoutedEventHandler(grid_CurrentCellChanged);
#if SILVERLIGHT
            grid.ShowGridLinesChanged -= new DependencyPropertyChangedEventHandler(grid_ShowGridLinesChanged);
#endif
        }

        #endregion

        #region GridEvents
        void grid_SelectionChanged(object sender, GridSelectionChangedEventArgs e)
        {
            SelectedRange = _spreadsheetGrid.Model.SelectedRanges;
            if (e.Reason == GridSelectionReason.MouseUp || e.Reason== GridSelectionReason.ArrowKey)
            {
                SelectedNamedRangeInfo = e.Range;
                nameboxtext = GetCellName(SelectedNamedRangeInfo);
                CellName = nameboxtext;
            }
            else if (e.Reason == GridSelectionReason.SelectRange)
            {
                nameboxtext = GetCellName(e.Range);
                if (!CellName.Equals(nameboxtext))
                {
                    nameboxtext = GetCellName(SelectedNamedRangeInfo);
                    CellName = nameboxtext;
                }
            }
        }

        void grid_CurrentCellActivated(object sender, ComponentModel.SyncfusionRoutedEventArgs args)
        {
            this.ActiveSpreadsheetGrid.Model.CommandStack.SuspendUndo = true;
            CurrentCell = _spreadsheetGrid.CurrentCell;
            IsEditing = this.SpreadsheetGrid.CurrentCell.IsEditing;
            CurrentCellStyle = HasCurrentExcelGrid ? this.SpreadsheetGrid.RenderStyles.GetRenderStyleInfo(CurrentCell.CellRowColumnIndex) : null;
            HorizontalAlignment = CurrentCellStyle.HorizontalAlignment;
            VerticalAlignment = CurrentCellStyle.VerticalAlignment;
            this.ActiveSpreadsheetGrid.Model.CommandStack.SuspendUndo = false;
        }

        void grid_ShowGridLinesChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            ShowGridLines = (bool)e.NewValue;
        }

        #endregion

        #region GridCurrentStates
        /// <summary>
        ///    Returns an SpreadsheetGrid that represents the active Grid
        ///    in the specified window or workbook. Read-only.
        /// </summary>
        public SpreadsheetGrid ActiveSpreadsheetGrid
        {
            get
            {
                return _spreadsheetGrid;
            }
        }

        private SpreadsheetGrid _spreadsheetGrid;
        internal SpreadsheetGrid SpreadsheetGrid
        {
            get { return _spreadsheetGrid; }
            set
            {
                _spreadsheetGrid = value;
                OnPropertyChanged("SpreadsheetGrid");
            }
        }

        private SpreadsheetGridModel _CurrentGridModel;
        public SpreadsheetGridModel CurrentExcelGridModel
        {
            get
            {
                return _CurrentGridModel;
            }
            set
            {
                _CurrentGridModel = value;
                OnPropertyChanged("CurrentExcelGridModel");
            }
        }

        public bool HasCurrentExcelGrid
        {
            get { return _spreadsheetGrid!=null && CurrentExcelGridModel != null; }
        }

        private string _currentSheetName;
        public string CurrentSheetName
        {
            get { return _currentSheetName; }
            set
            {
                _currentSheetName = value;
                OnPropertyChanged("CurrentSheetName");
            }
        }

        private GridCurrentCell _currentCell;
        public GridCurrentCell CurrentCell
        {
            get
            {
                return _currentCell;
            }
            set
            {
                _currentCell = value;
                OnPropertyChanged("CurrentCell");
            }
        }

        public bool HasCurrentCell
        {
            get
            {
                return CurrentCell != null && CurrentCell.HasCurrentCell;
            }
        }
     
        private GridStyleInfo _currentCellStyle;
        public GridStyleInfo CurrentCellStyle
        {
            get
            {
                return _currentCellStyle;
            }
            set
            {
                _currentCellStyle = value;

                if (_currentCellStyle != null 
#if !SILVERLIGHT
                    && CurrentExcelGridModel.GraphicModel.SelectedGraphicCells.Count <= 0)
#else
                    )
#endif
                {
                    SelectedNamedRangeInfo = GridRangeInfo.Cell(_currentCellStyle.RowIndex, _currentCellStyle.ColumnIndex);
                    nameboxtext = GetCellName(SelectedNamedRangeInfo);
                    CellName = nameboxtext;
                }
                else
                {
                    nameboxtext = string.Empty;
                    CellName = nameboxtext;
                }
                OnPropertyChanged("CurrentCellStyle");
                OnPropertyChanged("FormattedText"); // To update the formula bar
            }
        }

        private GridRangeInfo SelectedNamedRangeInfo;
        internal string nameboxtext;
        private string cellName = string.Empty;
        public string CellName
        {
            get { return cellName; }
            set
            {
                if (nameboxtext.Equals(cellName) && !nameboxtext.Equals(value))
                {
                    if (!AddNamedRange(value, SelectedNamedRangeInfo))
                    {
                        cellName = nameboxtext;
                        OnPropertyChanged("CellName");
                        return;
                    }
                }
                cellName = value;
                OnPropertyChanged("CellName");
            }
        }

        private string formattedText;
        public string FormattedText
        {
            get
            {

                if (IsFocusedOnGraphicCells)
                    return string.Empty;
                if (IsEditing)
                    return formattedText;
                else if (CurrentCellStyle != null)
                {
                    if (CurrentCellStyle.Text.StartsWith("="))
                        return CurrentCellStyle.Text;
                    return CurrentCellStyle.FormattedText;
                }
                else
                    return string.Empty;
            }
            set
            {
                if (IsFocusedOnGraphicCells)
                    formattedText = string.Empty;
                else
                {
                    var currentStyle = SpreadsheetGrid.Model[CurrentCell.RowIndex, CurrentCell.ColumnIndex];
                    if (currentStyle.CellType == "FormulaCell")
                        CurrentCell.Renderer.ControlText = value;
                    else
                        currentStyle.CellValue = value;

                formattedText = value;
                if (formattedText != null && formattedText.StartsWith("="))
                {
                    if (currentStyle.HasFormulaTag)
                        currentStyle.FormulaTag = null;
                }
#if SILVERLIGHT
                SpreadsheetGrid.InvalidateCell(CurrentCell.CellRowColumnIndex);
#endif
                }
            }
        }

        private bool isDisableMode = false;
        public bool IsDisableMode
        {
            get
            {
                return isDisableMode;
            }
            set
            {
                isDisableMode = value;
                OnPropertyChanged("IsDisableMode");
            }
        }

        void grid_CurrentCellChanged(object sender, ComponentModel.SyncfusionRoutedEventArgs args)
        {
            if (SpreadsheetGrid.CurrentCell.Renderer != null && SpreadsheetGrid.CurrentCell.Renderer.CurrentCellUIElement != null)
            {
                object text = null;
                if (SpreadsheetGrid.CurrentCell.Renderer.CurrentCellUIElement is TextBox)
                    text = SpreadsheetGrid.CurrentCell.Renderer.CurrentCellUIElement.GetValue(TextBox.TextProperty);
                else if (SpreadsheetGrid.CurrentCell.Renderer.CurrentCellUIElement is ComboBox)
                {
                    text = SpreadsheetGrid.CurrentCell.Renderer.CurrentCellUIElement.GetValue(ComboBox.SelectedValueProperty);
                }
                if (text != null)
                {
                    formattedText = text.ToString();
                    OnPropertyChanged("FormattedText");
                }
            }
            else if (SpreadsheetGrid.CurrentCell.Renderer != null)
            {
                formattedText = SpreadsheetGrid.CurrentCell.Renderer.ControlText;
                OnPropertyChanged("FormattedText");
            }
        }


        private GridRangeInfoList _selectedRange;
        public GridRangeInfoList SelectedRange
        {
            get
            {
                return _selectedRange;
            }
            set
            {
                _selectedRange = value;
                OnPropertyChanged("SelectedRange");
            }
        }

        public bool HasSelectedRange
        {
            get
            {
                if (CurrentExcelGridModel != null)
                {
                    if (CurrentExcelGridModel.SelectedRanges.Count > 0)
                    {
                        var range = CurrentExcelGridModel.SelectedRanges[0];
                        if(range.RangeType== GridRangeInfoType.Table || range.RangeType==GridRangeInfoType.Rows|| range.RangeType==GridRangeInfoType.Cols)
                        {
                            return true;
                        }
                        if (!range.IsEmpty && (range.Height > 1) || (range.Width > 1))
                        {
                            return true;
                        }
                    }
                }
                return false;
            }
        }

        private bool showGridLines = false;
        public bool ShowGridLines
        {
            get { return showGridLines; }
            set
            {
                if (showGridLines != value)
                {
                    showGridLines = value;
                    SetGridLinesVisibility(showGridLines);
                    OnPropertyChanged("ShowGridLines");
                }
            }
        }

        private bool isRowColumnHeadersVisible = false; 
        public bool IsRowColumnHeadersVisible
        {
            get { return isRowColumnHeadersVisible; }
            set
            {
                if (isRowColumnHeadersVisible != value)
                {
                    isRowColumnHeadersVisible = value;
                    SetRowColumnHeadersVisibility(isRowColumnHeadersVisible);
                    OnPropertyChanged("IsRowColumnHeadersVisible");
                }
            }
        }

        #endregion

        #region HighlightHeaderBackgroundProperty

        /// <summary>
        /// Gets or sets the high light background.
        /// </summary>
        /// <value>The high light background.</value>
        public Brush HighlightSelectionHeaderBackground
        {
            get { return (Brush)this.GetValue(HighlightSelectionHeaderBackgroundProperty); }
            set { this.SetValue(HighlightSelectionHeaderBackgroundProperty, value); }
        }


        /// <summary>
        /// HighLightBackground property
        /// </summary>
        public static readonly DependencyProperty HighlightSelectionHeaderBackgroundProperty = DependencyProperty.Register(
         "HighlightSelectionHeaderBackground", typeof(Brush), typeof(GridProperties), new PropertyMetadata(null));

        #endregion

        #region IsEditing

        private bool isEditing = false;
        public bool IsEditing
        {
            get { return isEditing; }
            set
            {
                if (isEditing != value)
                {
                    isEditing = value;
                    //if (CurrentCell != null)
                    //{
                    //    SpreadsheetGridStyleInfo style = SpreadsheetGrid.GetSpreadsheetGridRenderStyleInfo(CurrentCell.CellRowColumnIndex);
                    //    if (style != null) style.SuspendFormattedTextUpdate = isEditing;
                    //}
                    
                    OnPropertyChanged("IsEditing");
                }
            }
        }

        void grid_CurrentCellEditingComplete(object sender, ComponentModel.SyncfusionRoutedEventArgs args)
        {
            IsEditing = false;
        }

        void grid_CurrentCellStartEditing(object sender, ComponentModel.SyncfusionCancelRoutedEventArgs args)
        {
            IsEditing = true;
        }

        #endregion

        #region Font

        private HorizontalAlignment _HorizontalAlignment;
        public HorizontalAlignment HorizontalAlignment
        {
            get
            {
                return _HorizontalAlignment;
            }
            set
            {
                _HorizontalAlignment = value;
                OnPropertyChanged("HorizontalAlignment");
            }
        }

        private VerticalAlignment _VerticalAlignment = VerticalAlignment.Bottom;
        public VerticalAlignment VerticalAlignment
        {
            get
            {
                //if (CurrentCellStyle != null)
                //    _VerticalAlignment = CurrentCellStyle.VerticalAlignment;
                return _VerticalAlignment;
            }
            set
            {
                //if (CurrentCellStyle != null)
                //    CurrentCellStyle.VerticalAlignment = value;
                _VerticalAlignment = value;
                OnPropertyChanged("VerticalAlignment");
            }
        }
        #endregion

        #region PuplicMethods
        public void RefreshGridProperties(string PropertyName)
        {
            this.OnPropertyChanged(PropertyName);
        }
        #endregion

        #region Private Methods

        private void SetRowColumnHeadersVisibility(bool IsVisible)
        {
            if (this.SpreadsheetGrid != null && this.SpreadsheetGrid.ExcelProperties.WorkBook != null)
            {
                this.CurrentExcelGridModel.CommandStack.SuspendUndo = true;
                if (!IsVisible)
                {
                    this.CurrentExcelGridModel.RowHeights.SetHidden(0, 0, true);
                    this.CurrentExcelGridModel.ColumnWidths.SetHidden(0, 0, true);
                }
                else
                {
                    this.CurrentExcelGridModel.RowHeights.SetHidden(0, 0, false);
                    this.CurrentExcelGridModel.ColumnWidths.SetHidden(0, 0, false);
                }
                IWorksheet worksheet = this.SpreadsheetGrid.ExcelProperties.WorkBook.Worksheets[this.SpreadsheetGrid.SheetName];
                if (IsVisible != worksheet.IsRowColumnHeadersVisible)
                    worksheet.IsRowColumnHeadersVisible = IsVisible;

                this.CurrentExcelGridModel.CommandStack.SuspendUndo = false;
            }
        }

        private void SetGridLinesVisibility(bool IsVisible)
        {
            if (this.SpreadsheetGrid != null && this.SpreadsheetGrid.ExcelProperties.WorkBook != null)
            {
                GridStyleInfo[] cellsInfo=new GridStyleInfo[]{};

                if(this.ActiveSpreadsheetGrid.Model.CommandStack.ShouldGenerateUndoInfo)
                    this.ActiveSpreadsheetGrid.Model.CommandStack.Push(new SpreadsheetGridLinesCommand(this.ActiveSpreadsheetGrid.Model as SpreadsheetGridModel, GridRangeInfo.Empty, cellsInfo, Styles.StyleModifyType.Copy,this.ActiveSpreadsheetGrid.ShowGridLines));

                this.ActiveSpreadsheetGrid.ShowGridLines = IsVisible;
                this.ActiveSpreadsheetGrid.InvalidateCells();
                IWorksheet worksheet = this.SpreadsheetGrid.ExcelProperties.WorkBook.Worksheets[this.SpreadsheetGrid.SheetName];
                if (worksheet!=null && IsVisible != worksheet.IsGridLinesVisible)
                    worksheet.IsGridLinesVisible = IsVisible;
            }
        }

        /// <summary>
        /// Return the cell name based on the  GridRangeInfo
        /// </summary>
        /// <param name="Range">GridRangeInfo</param>
        /// <returns>Cell Name</returns>
        private string GetCellName(GridRangeInfo Range)
        {
            if (!Range.IsEmpty)
            {
                string value = GridRangeInfoToGlobalWorkbookRange(Range);
                if (!string.IsNullOrEmpty(value) && SpreadsheetGrid.Model.FormulaEngine.NamedRanges.ContainsValue(value))
                {
                    string key = FindKey(value, SpreadsheetGrid.Model.FormulaEngine.NamedRanges);
                    if (!string.IsNullOrEmpty(key))
                        return key;
                }

                if (Range.RangeType == GridRangeInfoType.Cols)
                    return GridRangeInfo.GetAlphaLabel(Range.Left) + 1;
                else if (Range.RangeType == GridRangeInfoType.Rows)
                    return GridRangeInfo.GetAlphaLabel(Range.Left + 1) + Range.Top;
                else if (Range.RangeType == GridRangeInfoType.Table)
                    return "A1";
                else
                    return GridRangeInfo.GetAlphaLabel(Range.Left == 0 ? Range.Left + 1 : Range.Left) + (Range.Top == 0 ? Range.Top + 1 : Range.Top);
            }
            return string.Empty;
        }

        /// <summary>
        /// Search the value in the Hashtable, if the value found then it will return the particular key
        /// </summary>
        /// <param name="Value">Hashtable value</param>
        /// <param name="HT">Hashtable</param>
        /// <returns>Hashtable key </returns>
        private string FindKey(string Value, Hashtable HT)
        {
            string Key = "";
            IDictionaryEnumerator e = HT.GetEnumerator();
            while (e.MoveNext()) 
            { 
                if (e.Value.ToString().Equals(Value))
                { 
                    Key = e.Key.ToString();
                }
            }
            return Key;
        }

        /// <summary>
        /// Convert the GridRangeInfo To workbook range (Address Global = "'Sheet1'!$A$1")
        /// </summary>
        /// <param name="Range">GridRangeInfo</param>
        /// <returns>string - Global Address</returns>
        private string GridRangeInfoToGlobalWorkbookRange(GridRangeInfo Range)
        {
            string value = string.Empty;
            if (Range.RangeType == GridRangeInfoType.Cells)
            {
                if (Range.Left == Range.Right && Range.Top == Range.Bottom) //"'Sheet1'!$A$1"
                    value = SpreadsheetGrid.SheetName + "!$" + GridRangeInfo.GetAlphaLabel(Range.Left) + "$" + Range.Top;
                else //"'Sheet1'!$A$1:$E$1"
                    value = SpreadsheetGrid.SheetName + "!$" + GridRangeInfo.GetAlphaLabel(Range.Left) + "$" + Range.Top + ":$" + GridRangeInfo.GetAlphaLabel(Range.Right) + "$" + Range.Bottom;
            }
            else if (Range.RangeType == GridRangeInfoType.Cols) //"'Sheet1'!$A:$A"
            {
                value = SpreadsheetGrid.SheetName + "!$" + GridRangeInfo.GetAlphaLabel(Range.Left) + ":$" + GridRangeInfo.GetAlphaLabel(Range.Right);
            }
            else if (Range.RangeType == GridRangeInfoType.Rows) //"'Sheet1'!$1:$1"
            {
                value = SpreadsheetGrid.SheetName + "!$" + Range.Top + ":$" + Range.Bottom;
            }
            return value;
        }

        /// <summary>
        /// Add the named range to the GridFormulaEngine and Workbook
        /// </summary>
        /// <param name="Name">NamedRange Name</param>
        /// <param name="Range">GridRangeInfo</param>
        private bool AddNamedRange(string Name,GridRangeInfo Range)
        {
            Regex AlphaPattern = new Regex(@"^[a-zA-Z0-9_.]*$");
            if (AlphaPattern.IsMatch(Name))
            {
                //To avoid the named range with the name A1, C21 or AB11
                int row = RowIndex(Name);
                int col = ColIndex(Name);
                if (row != int.MinValue && col != int.MinValue && row <= SpreadsheetGrid.Model.RowCount && col <= SpreadsheetGrid.Model.ColumnCount)
                {
                    return false;
                }
                string value = GridRangeInfoToGlobalWorkbookRange(Range);
                if (!string.IsNullOrEmpty(value) && !string.IsNullOrEmpty(Name))
                {
                    try
                    {
                        if (!SpreadsheetGrid.Model.FormulaEngine.NamedRanges.ContainsKey(Name) && !SpreadsheetGrid.Model.FormulaEngine.NamedRanges.ContainsValue(value))
                        {
                            SpreadsheetGrid.Model.FormulaEngine.AddNamedRange(Name, value);
                            IWorksheet sheet = SpreadsheetGrid.ExcelProperties.WorkBook.Worksheets[SpreadsheetGrid.SheetName];
                            IName lname1 = sheet.Names.Add(Name);
                            lname1.RefersToRange = sheet.Range[value];
                            return true;
                        }
                    }
                    catch (Exception)
                    {
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Returns the row index from a cell reference.
        /// </summary>
        /// <param name="s">String holding a cell reference such as C21 or AB11.</param>
        /// <returns>An integer with the corresponding row number.</returns>
        private int RowIndex(string s)
        {
            int row;
            int i = 0;
            s = s.Replace("$", string.Empty);
            while (i < s.Length && char.IsLetter(s[i]))
                i++;
            if (i < s.Length)
            {
                if (int.TryParse(s.Substring(i), out row))
                    return row;
            }
            return int.MinValue;
        }

        /// <summary>
        /// Returns a column index from a cell reference.
        /// </summary>
        /// <param name="s">String holding a cell reference such as C21 or AB11.</param>
        /// <returns>An integer with the corresponding column number.</returns>
        private int ColIndex(string s)
        {
            int i = 0;
            int k = 0;
            //To remove the $ symbol in the cells address
            s = s.Replace("$", string.Empty);
            s = s.ToUpper(CultureInfo.InvariantCulture);

            while (i < s.Length && char.IsLetter(s[i]))
            {
                k = (int)(k * 26 + (int)s[i] - (int)('A') + 1);
                i++;
            }

            if (k == 0)
                return int.MinValue;

            return k;
        }

        #endregion

        #region INotifyPropertyChanged Members
        private void OnPropertyChanged(string PropertyName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(PropertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

    }
}
