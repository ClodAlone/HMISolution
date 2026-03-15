#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Windows;
using System.Windows.Media;
using Syncfusion.Windows.Controls.Grid;
using Syncfusion.Windows.Tools.Controls;
using Syncfusion.XlsIO;
using Syncfusion.Windows.Shared;
using Syncfusion.Windows.Controls.Spreadsheet.Resources;
using System.Drawing;
using System.Windows.Controls;
using Syncfusion.Windows.Controls.Scroll;
using Syncfusion.Windows.GridCommon;
using Syncfusion.Windows.Controls.Cells;
using System.Collections.Generic;
using System.Windows.Shapes;
using System.Windows.Input;
using Syncfusion.Windows.Styles;
using System.Xml.Linq;
using System.IO;
using Syncfusion.Compression.Zip;


namespace Syncfusion.Windows.Controls.Spreadsheet
{
    /// <summary>
    /// 
    /// </summary>
    public class SpreadsheetGrid : GridControl
    {
        #region PrivateMembers

        internal bool IsStandAloneExcelGrid = true;
        /// <summary>
        /// When we save the cell value in the worksheet it will trigger the CellValueChanged, to avoid the invalidate cell 
        /// in the CellValueChanged event handler, this CommittedRowColumn Index was maintained
        /// </summary>
        internal RowColumnIndex CommittedCellRowColumnIndex { get; set; }

        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="SpreadsheetGrid"/> class.
        /// </summary>
        public SpreadsheetGrid()
        {
#if SILVERLIGHT
         
            
            Model.TableStyle.EnableFloatCell = true;
            Model.TableStyle.FloatCellsMode = GridFloatCellsMode.OnDemandCalculation;
#else
            this.FocusVisualStyle = null;
            Model.TableStyle.TextWrapping = TextWrapping.NoWrap;
            //Model.TableStyle.EnableFloatingCell = true;
            Model.TableStyle.FloatCellMode = GridFloatCellsMode.OnDemandCalculation;
            Model.Options.WrapCell = false;
#endif
            Model.Options.EnterKeyBehaviour = EnterKeyBehaviour.MouseDown;
            //this.MouseControllerDispatcher.Add(new GridExcelMarkerMouseController(this));
            ShowGridLines = true;
            
        }
        #endregion

        #region DependencyProperties

        internal ExcelProperties ExcelProperties
        {
            get { return (ExcelProperties)GetValue(ExcelPropertiesProperty); }
            set { SetValue(ExcelPropertiesProperty, value); }
        }

        public static readonly DependencyProperty ExcelPropertiesProperty =
            DependencyProperty.Register("ExcelProperties", typeof(ExcelProperties), typeof(SpreadsheetGrid), new PropertyMetadata(OnExcelPropertiesChanged));

        private static void OnExcelPropertiesChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            SpreadsheetGrid excelGrid = obj as SpreadsheetGrid;
            if (excelGrid != null)
            {
                
            }
        }

        internal string SheetName
        {
            get { return (string)GetValue(SheetNameProperty); }
            set { SetValue(SheetNameProperty, value); }
        }

        public static readonly DependencyProperty SheetNameProperty =
            DependencyProperty.Register("SheetName", typeof(string), typeof(SpreadsheetGrid), new PropertyMetadata(string.Empty));

        
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
        }

        public bool IsColumnHeadersVisible
        {
            get { return (bool)GetValue(IsColumnHeadersVisibleProperty); }
            set { SetValue(IsColumnHeadersVisibleProperty, value); }
        }

        public static readonly DependencyProperty IsColumnHeadersVisibleProperty =
            DependencyProperty.Register("IsColumnHeadersVisible", typeof(bool), typeof(SpreadsheetGrid), new PropertyMetadata(true));

        public bool IsRowHeaderVisible
        {
            get { return (bool)GetValue(IsRowHeaderVisibleProperty); }
            set { SetValue(IsRowHeaderVisibleProperty, value); }
        }

        public static readonly DependencyProperty IsRowHeaderVisibleProperty =
            DependencyProperty.Register("IsRowHeaderVisible", typeof(bool), typeof(SpreadsheetGrid), new PropertyMetadata(true));

        #endregion

        #region Importing
        
        protected override void OnQueryCellInfo(GridQueryCellInfoEventArgs e)
        {
            if (!IsStandAloneExcelGrid)
            {
                if (e.Cell.RowIndex > 0 && e.Cell.ColumnIndex > 0) // e.Style.IsChanged &&
                {
                    IWorksheet changedSheet = ExcelProperties.WorkBook.Worksheets[SheetName];
                    if (changedSheet != null)
                    {
                        IRange range = changedSheet.Range[e.Cell.RowIndex, e.Cell.ColumnIndex];
                        if(!(this.Model as SpreadsheetGridModel).IsInInsert)
                            ImportExportHelper.SetAlignment(range, e.Style);
                    }
                }
            }
            SetExcelLikeUI(e);
            base.OnQueryCellInfo(e);
        }

        //Set the Row and column Header
        private void SetExcelLikeUI(GridQueryCellInfoEventArgs e)
        {
            if (e.Style.RowIndex == 0 && this.IsColumnHeadersVisible)
                e.Style.CellValue = GridRangeInfo.GetAlphaLabel(e.Cell.ColumnIndex);
            else if (e.Style.ColumnIndex == 0 && this.IsRowHeaderVisible)
                e.Style.CellValue = e.Style.RowIndex;
        }

        private System.Windows.Media.Brush _headerBackgroundBrush;

        public System.Windows.Media.Brush HeaderBackgroundBrush
        {
            get
            {
                if (_headerBackgroundBrush == null)
                    return new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 218, 231, 245));
                return _headerBackgroundBrush;
            }
            set
            {
                _headerBackgroundBrush = value;
            }
        }

        #endregion

        #region ModelCreated

        protected override GridModel OnModelCreated()
        {
            SpreadsheetGridModel excelGridModel = new SpreadsheetGridModel
                                                {
                                                    SperadsheetGrid = this
                                                };
            return excelGridModel;
        }

        //internal virtual SpreadsheetGridStyleInfo GetSpreadsheetGridRenderStyleInfo(RowColumnIndex Cell)
        //{
        //    return (SpreadsheetGridStyleInfo)this.Model[Cell.RowIndex, Cell.ColumnIndex];
        //}

        #endregion

        #region Overrides

        #region CurrentCellValidating
#if SILVERLIGHT

        protected override void OnCurrentCellValidating(CurrentCellValidateEventArgs e)
#else
        protected override void OnCurrentCellValidating(Syncfusion.Windows.Controls.Grid.CurrentCellValidatingEventArgs e)
#endif
        {
            base.OnCurrentCellValidating(e);
#if SILVERLIGHT

            var _excelEditorControl = VisualUtil.FindVisualParent<SpreadsheetControl>(this);
#else
            var _excelEditorControl = VisualUtils.FindVisualParent<SpreadsheetControl>(this);
#endif
            if (_excelEditorControl != null)
            {
                _excelEditorControl.RaiseCurrentCellValidating(e);
                if (!e.Handled)
                {
                    if (e.Style.HasIntegerEdit)
                    {
                        e.Cancel = OnValidateIntegerEdit(e);
                    }
                    else if (e.Style.HasDoubleEdit)
                    {
                        e.Cancel = OnValidateDoubleEdit(e);
                    }
                    else if (e.Style.HasMaskEdit)
                    {
                        e.Cancel = OnValidateMaskEdit(e);
                    }
                    
                    else if (e.Style.CellType == "ComboBox")
                    {
                        e.Cancel = OnValidateComboBoxCell(e);
                    }
                    else
                    {
                        try
                        {
                            if (e.NewValue != null && e.NewValue.ToString().Length > 0 && e.NewValue.ToString()[0] == '=')
                            {
                                //Before parse the formula we need to set the FormulaContextCell
                                this.Model.FormulaEngine.FormulaContextCell = GridRangeInfo.GetAlphaLabel(e.Style.ColumnIndex) + e.Style.RowIndex.ToString();
                                string val = this.Model.FormulaEngine.Parse(e.NewValue.ToString());
                            }
                        }
                        catch (Exception ex)
                        {
                            if (ex.Message == "mismatched parentheses")
                            {
                                e.Cancel = OnValidateFormulaCell(e);
                            }
                        }
                    }
                }
            }
        }

#if SILVERLIGHT
        private bool OnValidateComboBoxCell(CurrentCellValidateEventArgs e)
#else
        private bool OnValidateComboBoxCell(CurrentCellValidatingEventArgs e)
#endif
        {

            List<string> listitem = e.Style.ItemsSource as List<string>;
            if (e.NewValue != null)
            {
                if (!listitem.Contains(e.NewValue.ToString()))
                    return ShowValidationMessage(e);
            }
            return false;

        }

#if SILVERLIGHT
        private bool OnValidateFormulaCell(CurrentCellValidateEventArgs e)
#else
        private bool OnValidateFormulaCell(CurrentCellValidatingEventArgs e)
#endif
        {
            e.Handled = true;
            MessageBoxResult result = MessageBox.Show("Your formula is missing paranthesis --).");
            if (result == MessageBoxResult.OK)
                return true;
            e.NewValue = e.OldValue;
            return false;
        }
#if SILVERLIGHT
        private bool OnValidateMaskEdit(CurrentCellValidateEventArgs e)
#else
        private bool OnValidateMaskEdit(CurrentCellValidatingEventArgs e)
#endif 
        {
            try
            {
                var maskEdit = e.Style.MaskEdit;
                
                if (maskEdit.HasMask)
                {
                    if (maskEdit.MinLength == maskEdit.MaxLength)
                    {
                        //Not Equal to
                        if (maskEdit.MinLength == e.NewValue.ToString().Length)
                        {
                            return ShowValidationMessage(e);
                        }
                    }
                    else
                    {
                        //Not between
                        if (maskEdit.MinLength < e.NewValue.ToString().Length && maskEdit.MaxLength > e.NewValue.ToString().Length)
                        {
                            return ShowValidationMessage(e);
                        }
                    }
                    return false;
                }
                else if (maskEdit.HasMinLength && maskEdit.HasMaxLength && maskEdit.MinLength == maskEdit.MaxLength)
                {
                    //Equal to
                    if (maskEdit.MinLength != e.NewValue.ToString().Length)
                    {
                        return ShowValidationMessage(e);
                    }
                    return false;
                }
                if (maskEdit.HasMinLength)
                {
                    if (maskEdit.MinLength > e.NewValue.ToString().Length)
                    {
                        return ShowValidationMessage(e);
                    }
                }
                if (maskEdit.HasMaxLength)
                {
                    if (maskEdit.MaxLength < e.NewValue.ToString().Length)
                    {
                        return ShowValidationMessage(e);
                    }
                }
            }
            catch (Exception)
            {
                return ShowValidationMessage(e);
            }
            return false;
        }

#if SILVERLIGHT
        private bool OnValidateDoubleEdit(CurrentCellValidateEventArgs e)
#else
        private bool OnValidateDoubleEdit(CurrentCellValidatingEventArgs e)
#endif 
        {
            try
            {
                //if have the value as empty, then we can’t convert it into int or double
                if (e.NewValue != null && string.IsNullOrEmpty(e.NewValue.ToString()))
                    return false;
                var doubleEdit = e.Style.DoubleEdit;
                if (doubleEdit.IsScrollingOnCircle)
                {
                    if (doubleEdit.MinValue == doubleEdit.MaxValue)
                    {
                        //Not Equal to
                        if (doubleEdit.MinValue == Convert.ToDouble(e.NewValue.ToString()))
                        {
                            return ShowValidationMessage(e);
                        }
                    }
                    else
                    {
                        //Not between
                        if ((doubleEdit.MinValue < Convert.ToDouble(e.NewValue.ToString())) && doubleEdit.MaxValue > Convert.ToDouble(e.NewValue.ToString()))
                        {
                            return ShowValidationMessage(e);
                        }
                    }
                    return false;
                }
                else if (doubleEdit.HasMinValue && doubleEdit.HasMaxValue && doubleEdit.MinValue == doubleEdit.MaxValue)
                {
                    //Equal to
                    if (doubleEdit.MinValue != Convert.ToDouble(e.NewValue.ToString()))
                    {
                        return ShowValidationMessage(e);
                    }
                    return false;
                }
                if (doubleEdit.HasMinValue)
                {
                    if (doubleEdit.MinValue > Convert.ToDouble(e.NewValue.ToString()))
                    {
                        return ShowValidationMessage(e);
                    }
                }
                if (doubleEdit.HasMaxValue)
                {
                    if (doubleEdit.MaxValue < Convert.ToDouble(e.NewValue.ToString()))
                    {
                        return ShowValidationMessage(e);
                    }
                }
            }
            catch (Exception)
            {
                return ShowValidationMessage(e);
            }
            return false;            
        }

#if SILVERLIGHT
        private bool OnValidateIntegerEdit(CurrentCellValidateEventArgs e)
#else
        private bool OnValidateIntegerEdit(CurrentCellValidatingEventArgs e)
#endif 
        {
            try
            {
                //if have the value as empty, then we can’t convert it into int or double
                if (e.NewValue != null && string.IsNullOrEmpty(e.NewValue.ToString()))
                    return false;
                var integeredit = e.Style.IntegerEdit;
                
                if (integeredit.IsScrollingOnCircle)
                {
                    if (integeredit.MinValue == integeredit.MaxValue)
                    {
                        //Not Equal to
                        if (integeredit.MinValue == Convert.ToInt64(e.NewValue.ToString()))
                        {
                            return ShowValidationMessage(e);
                        }
                    }
                    else
                    {
                        //Not between
                        if ((integeredit.MinValue < Convert.ToInt64(e.NewValue.ToString())) && integeredit.MaxValue > Convert.ToInt64(e.NewValue.ToString()))
                        {
                            return ShowValidationMessage(e);
                        }
                    }
                    return false;
                }
                else if (integeredit.HasMinValue && integeredit.HasMaxValue && integeredit.MinValue == integeredit.MaxValue)
                {
                    //Equal to
                    if (integeredit.MinValue != Convert.ToInt64(e.NewValue.ToString()))
                    {
                        return ShowValidationMessage(e);
                    }
                    return false;
                }
                if (integeredit.HasMinValue)
                {
                    if (integeredit.MinValue > Convert.ToInt64(e.NewValue.ToString()))
                    {
                        return ShowValidationMessage(e);
                    }
                }
                if (integeredit.HasMaxValue)
                {
                    if (integeredit.MaxValue < Convert.ToInt64(e.NewValue.ToString()))
                    {
                        return ShowValidationMessage(e);
                    }
                }
            }
            catch (Exception)
            {
                return ShowValidationMessage(e);
            }
            return false;
        }

#if SILVERLIGHT
        private bool ShowValidationMessage(CurrentCellValidateEventArgs e)
#else
        private bool ShowValidationMessage(CurrentCellValidatingEventArgs e)
#endif
        {
            e.Handled = true;
            MessageBoxResult result;
            if (!string.IsNullOrEmpty(e.Style.ErrorAlertText) && e.Style.ErrorAlertTitle != null)
                result = MessageBox.Show(e.Style.ErrorAlertText, e.Style.ErrorAlertTitle, MessageBoxButton.OKCancel);
            else if (!string.IsNullOrEmpty(e.Style.ErrorAlertText))
                result = MessageBox.Show(e.Style.ErrorAlertText, SpreadsheetResourceWrapper.ValidationFailed, MessageBoxButton.OKCancel);
            else if (e.Style.ErrorAlertTitle != null)
                result = MessageBox.Show(SpreadsheetResourceWrapper.ValidationMessage_ValueIsNoValid, e.Style.ErrorAlertTitle, MessageBoxButton.OKCancel);
            else
                result = MessageBox.Show(SpreadsheetResourceWrapper.ValidationMessage_ValueIsNoValid, SpreadsheetResourceWrapper.ValidationFailed, MessageBoxButton.OKCancel);
            if (result == MessageBoxResult.OK)
                return true;
            else
            {
                if (e.OldValue != null && !string.IsNullOrEmpty(e.OldValue.ToString()))
                    e.NewValue = e.OldValue;
                else
                    e.NewValue = null;
                return false;
            }
        }
        #endregion

        #region OnKeyDown

#if !SILVERLIGHT

        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            base.OnPreviewKeyDown(e);
            if (!e.Handled)
            {
                if (e.Key == Key.Delete && CurrentCell != null && CurrentCell.HasCurrentCell && !CurrentCell.IsEditing)
                {
                    if (this.Model.CommandStack.Enabled)
                        this.Model.CommandStack.BeginTrans("Delete selected range");
                    foreach (GridRangeInfo SelectedRange in this.Model.SelectedRanges)
                    {
                        //string address = SelectedRange.ConvertGridRangeToExcelRange();
                        //IRange SelectedWorkbookRange = ExcelProperties.WorkBook.Worksheets[SheetName].Range[address];
                        //SelectedWorkbookRange.Clear(ExcelClearOptions.ClearContent);
                        GridRangeInfo ExpandedRange = ExpandSelectedCellsRange(SelectedRange);
                        for (int row = ExpandedRange.Top; row <= ExpandedRange.Bottom; row++)
                        {
                            for (int col = ExpandedRange.Left; col <= ExpandedRange.Right; col++)
                            {
                                var style = this.Model[row, col];
                                if (!style.ReadOnly && style.Enabled)
                                    style.ResetCellValue();
                                
                            }
                        }
                        if (CurrentCell.Renderer != null && CurrentCell.Renderer.CurrentStyle.CellType != "DateTimeEdit")
                        {
                            if (!CurrentCell.Renderer.CurrentStyle.ReadOnly && CurrentCell.Renderer.CurrentStyle.Enabled)
                            {
                                CurrentCell.Renderer.ControlText = string.Empty;
                                CurrentCell.Renderer.ControlValue = null;
                            }
                        }

                        if (CurrentCell.Renderer != null && CurrentCell.Renderer is GridCellDateTimeEditCellRenderer)
                        {
                            CurrentCell.Deactivate();
                            CurrentCell.Activate(CurrentCell.RowIndex, CurrentCell.ColumnIndex);
                        }

                        InvalidateCell(SelectedRange);
                    }
                    if (this.Model.CommandStack.InTransaction)
                        this.Model.CommandStack.CommitTrans();
                }
            }
        }
#else
       
        protected override void OnKeyDown(System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Delete && CurrentCell != null && CurrentCell.HasCurrentCell && !CurrentCell.IsEditing)
            {
                foreach (GridRangeInfo SelectedRange in this.Model.SelectedRanges)
                {
                    //string address = SelectedRange.ConvertGridRangeToExcelRange();
                    //IRange SelectedWorkbookRange = ExcelProperties.WorkBook.Worksheets[SheetName].Range[address];
                    //SelectedWorkbookRange.Clear(ExcelClearOptions.ClearContent);
                    //InvalidateCell(SelectedRange);
                    GridRangeInfo SRange = ExpandSelectedCellsRange(SelectedRange);
                    for (int row = SRange.Top; row <= SRange.Bottom; row++)
                    {
                        for (int col = SRange.Left; col <= SRange.Right; col++)
                        {
                            var style = this.Model[row, col];
                            style.ResetCellValue();
                        }
                    }
                    InvalidateCell(SelectedRange);
                }
                InvalidateVisual(true);
                e.Handled = true;
            }
            base.OnKeyDown(e);
        }
#endif
        #endregion 
        
        public override void SetRowHeight(int rowIndex, double size)
        {
            this.Model.CommandStack.SuspendUndo = false;
            this.Model.CommandStack.BeginTrans("RowHeight");
            if (this.Model.IsRows(this.Model.SelectedRanges) && this.Model.SelectedRanges.AnyRangeContains(GridRangeInfo.Row(rowIndex)))
            {
                foreach (GridRangeInfo rangeToResize in this.Model.SelectedRanges)
                {
                    if (size > 0)
                    {
                        int top = rangeToResize.Top;
                        if (this.Model.CommandStack.ShouldGenerateUndoInfo)
                        {
                            for (int row = top; row <= rangeToResize.Bottom; row++)
                            {
                                this.Model.CommandStack.Push(new SpreadsheetSetRowSizeCommand(this.Model as SpreadsheetGridModel, rangeToResize.Top, rangeToResize.Bottom, RowHeights[row]));
                            }
                        }
                        this.Model.CommandStack.SuspendUndo = true;
                        this.Model.RowHeights.SetRange(rangeToResize.Top,rangeToResize.Bottom, size);
                        this.Model.CommandStack.SuspendUndo = false;
                        if (top <= 0) top++;
                        this.ExcelProperties.WorkBook.ActiveSheet.SetRowHeightInPixels(top, (rangeToResize.Bottom +1) - top, size);
                    }
                    else
                    {
                        if (this.Model.CommandStack.ShouldGenerateUndoInfo)
                            this.Model.CommandStack.Push(new SpreadsheetSetRowHideCommand(this.Model as SpreadsheetGridModel, rangeToResize.Top, rangeToResize.Bottom, false));
                        this.Model.CommandStack.SuspendUndo = true;
                        this.Model.RowHeights.SetHidden(rangeToResize.Top, rangeToResize.Bottom, true);
                        this.Model.CommandStack.SuspendUndo = false;
                    }
                }
                InvalidateVisual();
            }
            else
            {
                if (!RowHeights.SupportsNestedLines || RowHeights.GetNestedLines(rowIndex) == null)
                {
                    if (size > 0)
                    {
                        if (this.Model.CommandStack.ShouldGenerateUndoInfo)
                            this.Model.CommandStack.Push(new SpreadsheetSetRowSizeCommand(this.Model as SpreadsheetGridModel, rowIndex, rowIndex, RowHeights[rowIndex]));
                        this.Model.CommandStack.SuspendUndo = true;
                        this.Model.RowHeights.SetRange(rowIndex, rowIndex, size);
                        this.Model.CommandStack.SuspendUndo = false;
                        if (rowIndex > 0)
                            this.ExcelProperties.WorkBook.ActiveSheet.SetRowHeightInPixels(rowIndex, size);
                    }
                    else
                    {
                        if (this.Model.CommandStack.ShouldGenerateUndoInfo)
                            this.Model.CommandStack.Push(new SpreadsheetSetRowHideCommand(this.Model as SpreadsheetGridModel, rowIndex, rowIndex, false));
                        this.Model.CommandStack.SuspendUndo = true;
                        this.Model.RowHeights.SetHidden(rowIndex, rowIndex, true);
                        this.Model.CommandStack.SuspendUndo = false;
                    }
                }
                InvalidateVisual();
            }
            this.Model.CommandStack.CommitTrans();
        }

        public override void SetColumnWidth(int columnIndex, double size)
        {
            this.Model.CommandStack.SuspendUndo = false;
            this.Model.CommandStack.BeginTrans("ColumnWidth");
            if (this.Model.IsCols(this.Model.SelectedRanges) && this.Model.SelectedRanges.AnyRangeContains(GridRangeInfo.Col(columnIndex)))
            {
                foreach (GridRangeInfo rangeToResize in this.Model.SelectedRanges)
                {
                    this.ArrangedCellUIElements.Invalidate(new CellSpanInfoBase(0, rangeToResize.Left,this.RowHeights.LineCount - 1,rangeToResize.Right));
                    if (size > 0)
                    {
                        int left = rangeToResize.Left;
                        if (this.Model.CommandStack.ShouldGenerateUndoInfo)
                        {
                            for (int col = left; col <= rangeToResize.Right; col++)
                            {
                                this.Model.CommandStack.Push(new SpreadsheetSetColumnSizeCommand(this.Model as SpreadsheetGridModel, col, col,ColumnWidths[col]));
                            }
                        }
                        this.Model.CommandStack.SuspendUndo = true;
                        this.Model.ColumnWidths.SetRange(rangeToResize.Left, rangeToResize.Right, size);
                        this.Model.CommandStack.SuspendUndo = false;
                        if (left <= 0) left++;
                        this.ExcelProperties.WorkBook.ActiveSheet.SetColumnWidthInPixels(left, (rangeToResize.Right + 1) - left, (int)size);
                    }
                    else
                    {
                        if (this.Model.CommandStack.ShouldGenerateUndoInfo)
                            this.Model.CommandStack.Push(new SpreadsheetSetColumnHideCommand(this.Model as SpreadsheetGridModel, rangeToResize.Left, rangeToResize.Right, false));
                        this.Model.CommandStack.SuspendUndo = true;
                        this.Model.ColumnWidths.SetHidden(rangeToResize.Left, rangeToResize.Right, true);
                        this.Model.CommandStack.SuspendUndo = false;
                    }
                }
            }
            else
            {
                if (columnIndex < this.Model.ColumnCount)
                    this.ArrangedCellUIElements.Invalidate(new CellSpanInfoBase(0, columnIndex,this.RowHeights.LineCount - 1,columnIndex + 1));

                if (size > 0)
                {
                    if (this.Model.CommandStack.ShouldGenerateUndoInfo)
                        this.Model.CommandStack.Push(new SpreadsheetSetColumnSizeCommand(this.Model as SpreadsheetGridModel, columnIndex, columnIndex, ColumnWidths[columnIndex]));
                    this.Model.CommandStack.SuspendUndo = true;
                    this.Model.ColumnWidths.SetRange(columnIndex, columnIndex, size);
                    this.Model.CommandStack.SuspendUndo = false;
                    if (columnIndex > 0)
                        this.ExcelProperties.WorkBook.ActiveSheet.SetColumnWidthInPixels(columnIndex, (int)size);
                }
                else
                {
                    if (this.Model.CommandStack.ShouldGenerateUndoInfo)
                        this.Model.CommandStack.Push(new SpreadsheetSetColumnHideCommand(this.Model as SpreadsheetGridModel, columnIndex, columnIndex, false));
                    this.Model.CommandStack.SuspendUndo = true;
                    this.Model.ColumnWidths.SetHidden(columnIndex, columnIndex, true);
                    this.Model.CommandStack.SuspendUndo = false;
                }
            }
            InvalidateVisual();
            this.Model.CommandStack.CommitTrans();
        }

        protected override void OnResizingColumns(GridResizingColumnsEventArgs args)
        {
            base.OnResizingColumns(args);
            if (args.Reason == GridResizeCellsReason.DoubleClick)
            {
                double columnWidth;
               if(this.Model.IsCols(this.Model.SelectedRanges))
               {
                   if (this.Model.CommandStack.ShouldGenerateUndoInfo && !this.Model.CommandStack.SuspendUndo)
                       this.Model.CommandStack.BeginTrans("ResizeColumns");

                   foreach (GridRangeInfo rangeToResize in this.Model.SelectedRanges)
                    {
                        for (int i = rangeToResize.Left; i <= rangeToResize.Right; i++)
                        {                            
                            if (!this.IsBlankColumn(i))
                            {
                                if (this.Model.CommandStack.ShouldGenerateUndoInfo && !this.Model.CommandStack.SuspendUndo)
                                {
                                    columnWidth = this.Model.ColumnWidths[i];
                                    SpreadsheetSetColumnSizeCommand spreadsheetSetColumnSizeCommand = new SpreadsheetSetColumnSizeCommand(this.Model as SpreadsheetGridModel, i, i, columnWidth);
                                    this.Model.CommandStack.Push(spreadsheetSetColumnSizeCommand);                                    
                                }

                                this.Model.CommandStack.SuspendUndo = true;
                                this.Model.ResizeColumnsToFit(GridRangeInfo.Col(i), GridResizeToFitOptions.IncludeHeaders);
                                this.Model.CommandStack.SuspendUndo = false;
                            }
                        }
                    }

                   if (this.Model.CommandStack.ShouldGenerateUndoInfo && !this.Model.CommandStack.SuspendUndo)
                       this.Model.CommandStack.CommitTrans();
               }
               else if(!this.IsBlankColumn(args.Columns.Left))
               {
                   if (this.Model.CommandStack.ShouldGenerateUndoInfo && !this.Model.CommandStack.SuspendUndo)
                   {
                       columnWidth = this.Model.ColumnWidths[args.Columns.Left];
                       SpreadsheetSetColumnSizeCommand spreadsheetSetColumnSizeCommand = new SpreadsheetSetColumnSizeCommand(this.Model as SpreadsheetGridModel, args.Columns.Left, args.Columns.Left, columnWidth);
                       this.Model.CommandStack.Push(spreadsheetSetColumnSizeCommand);
                   }
                   this.Model.CommandStack.SuspendUndo = true;
                   this.Model.ResizeColumnsToFit(GridRangeInfo.Col(args.Columns.Left), GridResizeToFitOptions.IncludeHeaders);
                   this.Model.CommandStack.SuspendUndo = false;
               }

               args.AllowResize = false;
            }
            else if (args.Reason == GridResizeCellsReason.MouseMove)
                this.Model.CommandStack.SuspendUndo = true;
            else if (args.Reason == GridResizeCellsReason.MouseUp)
                this.Model.CommandStack.SuspendUndo = false;
        }

        protected override void OnResizingRows(GridResizingRowsEventArgs args)
        {
            base.OnResizingRows(args);
            if (args.Reason == GridResizeCellsReason.MouseMove)
                this.Model.CommandStack.SuspendUndo = true;
            else if(args.Reason == GridResizeCellsReason.MouseUp)
                this.Model.CommandStack.SuspendUndo = false;
        }
                
        protected override void OnCommittedCellInfo(GridCommitCellInfoEventArgs e)
        {
            if (e.Sip != null && e.Sip.PropertyName == "CellValue")
            {
                CommittedCellRowColumnIndex = e.Style.CellRowColumnIndex;
                if (e.Style.Description != null && e.Style.Description == "IsImported")
                {
                    IWorksheet changedSheet = ExcelProperties.WorkBook.Worksheets[SheetName];
                    if (changedSheet != null)
                    {
                        IRange range = changedSheet.Range[e.Cell.RowIndex, e.Cell.ColumnIndex];
                        ImportExportHelper.ExportCellToExcel(Model, ExcelProperties.WorkBook, range, e);

                        if (e.Style.CellValue == null && e.Style.CellType == "DateTimeEdit")
                            e.Style.CellType = "FormulaCell";

                        GridSheetFamilyItem family = GridFormulaEngine.GetSheetFamilyItem(this.Model);
                        var token = family.GetTokenForGridModel(this.Model);
                        string sheet = token != null ? token.ToString() : string.Empty;
                        string s = sheet + GridRangeInfo.GetAlphaLabel(e.Cell.ColumnIndex) + e.Cell.RowIndex.ToString();
                        if (!(this.ExcelProperties.ChangedCellList.Contains(s)))
                        {
                            XElement xelement = new XElement("CellAddress", s);
                            this.ExcelProperties.ChangedXMLCellList.Add(xelement);
                            this.ExcelProperties.ChangedCellList.AddIfUnique(s);
                        }
                    }
                }
            }
            base.OnCommittedCellInfo(e);
        }

        public event DependencyPropertyChangedEventHandler ShowGridLinesChanged;
        protected override void OnShowGridLinesChanged(DependencyPropertyChangedEventArgs args)
        {
            if (ShowGridLinesChanged != null)
                ShowGridLinesChanged(this, args);
            base.OnShowGridLinesChanged(args);
        }

        protected override void OnLoaded(RoutedEventArgs e)
        {
            base.OnLoaded(e);
            //SetRowHeaderWidth(GridResizeToFitOptions.IncludeHeaders);
            ScrollRows.Changed += ScrollRowsChanged;
        }

        void ScrollRowsChanged(object sender, EventArgs e)
        {
            SetRowHeaderWidth(GridResizeToFitOptions.IncludeHeaders|GridResizeToFitOptions.NoShrinkSize);
        }

        protected override void OnCurrentCellMoving(GridCurrentCellMovingEventArgs e)
        {
            if (e.CellRowColumnIndex != null)
            {
                // To set the current cell as next to the header cell while click the row/column header
                e.CellRowColumnIndex = new Cells.RowColumnIndex(Math.Max(e.CellRowColumnIndex.RowIndex, 1), Math.Max(e.CellRowColumnIndex.ColumnIndex, 1));
            }
            base.OnCurrentCellMoving(e);
        }

#if SILVERLIGHT
        /// <summary>
        /// OnArrangeCell gets
        /// the <see cref="ICellRenderer"/> for a cell and calls its <see cref="ICellRenderer.Arrange"/>
        /// method. The method
        /// also adjust the <see cref="CellArgs.CellRect"/> and subtracts the border margins
        /// from the rectangle.
        /// </summary>
        /// <param name="aca">The cell layout information.</param>
        protected override void OnArrangeCell(Cells.ArrangeCellArgs aca)
        {
            base.OnArrangeCell(aca);
        }

        /// <summary>
        /// Renders the cell borders.
        /// </summary>
        protected override void RenderCellBorders()
        {
            base.RenderCellBorders();
        }
#endif
        internal bool TextMarginChanged = false;
        internal CellMarginsInfo tempTextmargin = null;

        protected override void OnCurrentCellStartEditing(ComponentModel.SyncfusionCancelRoutedEventArgs e)
        {
            SpreadsheetGridCopyPaste.SourceRange = null;
            
#if !SILVERLIGHT
            Clipboard.SetText(string.Empty);
#endif
            base.OnCurrentCellStartEditing(e);
         
        }

        /// <summary>
        /// Gets the width of the visible columns.
        /// </summary>
        /// <param name="Col">The col.</param>
        /// <returns></returns>
        internal double getVisibleColumnsWidth(int Col)
        {
            //calculate visible columns width
            double visibleColumnsWidth = 0.0;
            for (int i = Col; i < this.ScrollColumns.LastBodyVisibleLineIndex; i++)
            {
                visibleColumnsWidth += this.ColumnWidths[Col];
            }
            return visibleColumnsWidth;
        }


        protected override void OnCurrentCellEditingComplete(ComponentModel.SyncfusionRoutedEventArgs e)
        {
            base.OnCurrentCellEditingComplete(e);
            CommittedCellRowColumnIndex = RowColumnIndex.Empty;
            // To set the current cell rowheights of currentcell
            string outText = string.Empty;
            SpreadsheetGridModel SpreadModel = this.Model as SpreadsheetGridModel;
#if !SILVERLIGHT
            //When Editing Complete then remove CenterAcrossSelection
            if (TextMarginChanged)
            {
                if (tempTextmargin != null)
                {
                    SpreadModel[CurrentCell.RowIndex, CurrentCell.ColumnIndex].TextMargins = tempTextmargin;
                }
                TextMarginChanged = false;
            }
#endif
            var currentCellStyle = this.RenderStyles.GetRenderStyleInfo(CurrentCell.RowIndex, CurrentCell.ColumnIndex);
            if (currentCellStyle.TextWrapping == TextWrapping.Wrap)
            {
                this.Model.CommandStack.SuspendUndo = true;
                this.Model.ResizeRowsToFit(GridRangeInfo.Row(CurrentCell.RowIndex), GridResizeToFitOptions.None);
                if (this.Model.RowHeights[CurrentCell.RowIndex] < this.ExcelProperties.WorkBook.ActiveSheet.GetRowHeightInPixels(CurrentCell.RowIndex))
                    this.Model.RowHeights[CurrentCell.RowIndex] = this.ExcelProperties.WorkBook.ActiveSheet.GetRowHeightInPixels(CurrentCell.RowIndex);
                this.Model.CommandStack.SuspendUndo = false;
            }
            this.ExcelProperties.spreadControl.RefreshFormulaBar();
        }

        /// <summary>
        /// Prepare Rendrer scell raised to set HighlightSelectionForeGround
        /// </summary>
        /// <param name="e">The <see cref="Syncfusion.Windows.Controls.Grid.GridPrepareRenderCellEventArgs"/> instance containing the event data.</param>
        protected override void OnPrepareRenderCell(GridPrepareRenderCellEventArgs e)
        {
            SpreadsheetGridModel SpreadModel = this.Model as SpreadsheetGridModel;
#if SILVERLIGHT
            var Spreadctrl = VisualUtil.FindVisualParent<SpreadsheetControl>(this);
#else
            var Spreadctrl = VisualUtils.FindVisualParent<SpreadsheetControl>(this);
#endif
            if (e.Cell.RowIndex == 0 && e.Cell.ColumnIndex == 0)
            {
                base.OnPrepareRenderCell(e);
                return;
            }
           //Row header highlight background
            if (e.Cell.RowIndex == 0 && SpreadModel.SelectedRanges.AnyRangeIntersects(GridRangeInfo.Col(e.Cell.ColumnIndex)) && Spreadctrl!=null)
            {
                //check wheather the user defined the property HighlightBackground or not
                if (Spreadctrl.GridProperties.HighlightSelectionHeaderBackground == null)
                    e.Style.Background = SpreadModel.SpreadsheetGridVisualStyle.SelectionHeaderBackgroundBrush;
                else
                    e.Style.Background = Spreadctrl.GridProperties.HighlightSelectionHeaderBackground;

                GridRangeInfoList range = SpreadModel.SelectedRanges.GetRangesIntersecting(GridRangeInfo.Col(e.Cell.ColumnIndex));
                if (range.Count > 0 && (range[0].RangeType == GridRangeInfoType.Cols || range[0].RangeType == GridRangeInfoType.Table))
                {
                    e.Style.Background = SpreadModel.SpreadsheetGridVisualStyle.RowColumnSelectionHeaderBackgroundBrush;
                }
            }
            else if (e.Cell.RowIndex == 0 && SpreadModel.ActiveGridView.CurrentCell.ColumnIndex == e.Cell.ColumnIndex)
            {
                e.Style.Background = SpreadModel.SpreadsheetGridVisualStyle.HeaderBackgroundBrush;
            }
            //Column header highlight background
            if (e.Cell.ColumnIndex == 0 && SpreadModel.SelectedRanges.AnyRangeIntersects(GridRangeInfo.Row(e.Cell.RowIndex)) && Spreadctrl != null)
            {
                //check wheather the user defined the property HighlightBackground or not
                if (Spreadctrl.GridProperties.HighlightSelectionHeaderBackground == null)
                    e.Style.Background = SpreadModel.SpreadsheetGridVisualStyle.SelectionHeaderBackgroundBrush;
                else
                    e.Style.Background = Spreadctrl.GridProperties.HighlightSelectionHeaderBackground;

                GridRangeInfoList range = SpreadModel.SelectedRanges.GetRangesIntersecting(GridRangeInfo.Col(e.Cell.RowIndex));
                if (range.Count > 0 && (range[0].RangeType == GridRangeInfoType.Rows || range[0].RangeType == GridRangeInfoType.Table))
                {
                    e.Style.Background = SpreadModel.SpreadsheetGridVisualStyle.RowColumnSelectionHeaderBackgroundBrush;
                }
            }
            else if (e.Cell.ColumnIndex == 0 && SpreadModel.ActiveGridView.CurrentCell.RowIndex == e.Cell.RowIndex)
            {
                e.Style.Background = SpreadModel.SpreadsheetGridVisualStyle.HeaderBackgroundBrush;
            }
            base.OnPrepareRenderCell(e);
        }

        /// <summary>
        /// Convert Strings to color.
        /// </summary>
        /// <param name="hexaColor">Color of the hexa.</param>
        /// <returns></returns>
        public static System.Windows.Media.Color StringToColor(string hexaColor)
        {
            var color = System.Windows.Media.Color.FromArgb(Convert.ToByte(hexaColor.Substring(1, 2), 16), Convert.ToByte(hexaColor.Substring(3, 2), 16), Convert.ToByte(hexaColor.Substring(5, 2), 16), Convert.ToByte(hexaColor.Substring(7, 2), 16));
            return color;
        }

        /// <summary>
        /// Gets the length of the text.
        /// </summary>
        /// <param name="row">The row.</param>
        /// <param name="col">The col.</param>
        /// <returns></returns>
        public double getTextLength(int row,int col)
        {
            
          //calculating Text Length
#if !SILVERLIGHT
            System.Drawing.Graphics graphics = System.Drawing.Graphics.FromImage(new Bitmap(1, 1));  
            SizeF textSize = graphics.MeasureString(this.Model[CurrentCell.RowIndex, col].Text, new Font(this.Model[CurrentCell.RowIndex, col].Font.FontFamily.ToString(),(float)this.Model[CurrentCell.RowIndex, col].Font.FontSize));
            double textlength = textSize.Width;
#else
             TextBlock txtMeasure = new TextBlock();
             txtMeasure.FontSize = this.Model[CurrentCell.RowIndex, col].Font.FontSize;
             txtMeasure.Text = this.Model[CurrentCell.RowIndex, col].Text;
             txtMeasure.FontFamily = this.Model[CurrentCell.RowIndex, col].Font.FontFamily;
             double textlength = txtMeasure.ActualWidth;
#endif
             return textlength;
        }
        #endregion

        #region Internal Method
        internal virtual void CellRequestNavigate(CellRequestNavigateEventArgs e)
        {
#if SILVERLIGHT
            var _excelEditorControl = VisualUtil.FindVisualParent<SpreadsheetControl>(this);
#else
            var _excelEditorControl = VisualUtils.FindVisualParent<SpreadsheetControl>(this);
#endif
            if (_excelEditorControl != null)
                _excelEditorControl.RaiseCellRequestNavigate(e);
        }

        internal void CreateXElement()
        {
            foreach (var item in this.ExcelProperties.ChangedCellList)
            {
                XElement xelement = new XElement("CellAddress", item);
                this.ExcelProperties.ChangedXMLCellList.Add(xelement);
            }
        }

        internal Stream CreateXMLFile()
        {
            XDocument myXml = new XDocument(new XDeclaration("1.0", "utf-8", "yes"), this.ExcelProperties.ChangedXMLCellList);
            MemoryStream ms = new MemoryStream();
#if SyncfusionFramework4_0
            myXml.Save(ms);
#endif
            return ms;
        }

        public BinaryList ReadXMLFile(Stream mStream)
        {
            ZipArchive archive = new ZipArchive();
            archive.Open(mStream, true);
            ZipArchiveItem item = archive["Syncfusion/Custom.xml"];
            if (item != null)
            {
                BinaryList blist = new BinaryList();

                XDocument xdoc = null;
#if SyncfusionFramework4_0
                XDocument.Load(item.DataStream);
#endif
                foreach (XElement currentElement in xdoc.Root.Elements())
                {
                    if (currentElement.Name.LocalName == "CellAddress")
                    {
                        string CellAddress = currentElement.Value;
                        blist.Add(CellAddress);
                    }
                }

                return blist;
            }
            return null;
        }

        internal string SheetToken(string s)
        {
            int i = 0;
            string s1 = "";
            if (i < s.Length && s[i] == '!')
            {
                i++;
                while (i < s.Length && s[i] != '!')
                    i++;
                s1 = s.Substring(0, i + 1);
            }

            if (i < s.Length)
                return s1;

            return string.Empty;
        }


        /// <summary>
        /// Determines whether the entire column is blank or not.
        /// </summary>
        /// <param name="columnIndex">ColumnIndex to check </param>
        /// <returns>true if all cells in the column are blank, otherwise returns false.</returns>
        internal bool IsBlankColumn(int columnIndex)
        {
            int startRow=0;
            int lastRow = this.Model.RowCount - 1;

            if (this.ExcelProperties.WorkBook.ActiveSheet.IsRowColumnHeadersVisible)
                startRow = 1;

            for (int i = startRow; i <= lastRow; i++)
            {
                if (!string.IsNullOrEmpty(this.Model[i,columnIndex].CellValue.ToString()))
                    return false;
            }

            return true;
        }

        #endregion

        #region Private Methods
        private void SetRowHeaderWidth(GridResizeToFitOptions options)
        {
            int lastVisibleRowIndex = ScrollRows.LastBodyVisibleLineIndex;
            Model.ResizeColumnsToFit(GridRangeInfo.Cell(lastVisibleRowIndex, 0), options);
        }
        #endregion
    }
}
