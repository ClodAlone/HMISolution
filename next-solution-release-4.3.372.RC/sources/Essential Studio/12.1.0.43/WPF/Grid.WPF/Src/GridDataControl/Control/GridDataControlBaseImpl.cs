#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
namespace Syncfusion.Windows.Controls.Grid
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Windows.Input;
    using System.Windows.Media;
    using Syncfusion.Windows.Controls.Scroll;
    using Syncfusion.Windows.Controls.Cells;
    using System.ComponentModel;
    using System.Windows;
    using System.Windows.Controls;
    using Syncfusion.Windows.Data;
    using System.Diagnostics;
    //using Syncfusion.Windows.Controls.Grid.Automation.Peers;
#if !SILVERLIGHT && SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    /// <summary>
    /// Derived control from <see cref="GridControlBase"/> that has customizations done for
    /// DataBound scenarios.
    /// </summary>

    [StyleTypedProperty(Property = "RowStyle", StyleTargetType = typeof(GridDataRowControl))]
    [StyleTypedProperty(Property = "AlternateRowStyle", StyleTargetType = typeof(GridDataRowControl))]
    [StyleTypedProperty(Property = "HeaderStyle", StyleTargetType = typeof(GridDataHeaderCellControl))]
    public class GridDataControlBaseImpl : GridControlBase, IDisposable
    {
        #region Style containers
        private VisualContainer frameworkElementsFrame;
        private GridDataRowControl evenRow;
        private GridDataRowControl oddRow;
        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="GridDataControlBaseImpl"/> class.
        /// </summary>
        public GridDataControlBaseImpl()
            : base()
        {
#if !SILVERLIGHT
            var rowResizeController = this.MouseControllerDispatcher.Find("ResizeRowsMouseController") as GridResizeRowsMouseController;
            this.MouseControllerDispatcher.Remove(rowResizeController);
            this.MouseControllerDispatcher.Add(new GridDataNestedResizeRowsMouseController(this));

            var selectCellMouseController = this.MouseControllerDispatcher.Find("SelectCellsMouseController") as GridSelectCellsMouseController;
            this.MouseControllerDispatcher.Remove(selectCellMouseController);
            selectCellMouseController.Dispose();
            this.MouseControllerDispatcher.Add(new GridDataNestedSelectCellsMouseController(this));
#endif
            this.InitSelectsCellsMouseController();
        }

        #region EnableBlendStyling Code


        #region HeaderStyle (DependencyProperty)

        /// <summary>
        /// Gets / sets the header style.
        /// </summary>
        public Style HeaderStyle
        {
            get { return (Style)GetValue(HeaderStyleProperty); }
            set { SetValue(HeaderStyleProperty, value); }
        }

        public static readonly DependencyProperty HeaderStyleProperty = DependencyProperty.Register("HeaderStyle", typeof(Style), typeof(GridDataControlBaseImpl), new PropertyMetadata(null));

        #endregion
#if !SILVERLIGHT

        #region ColumnOptionsPaneStyle

        public Style ColumnOptionPaneStyle
        {
            get { return (Style)GetValue(ColumnOptionPaneStyleProperty); }
            set { SetValue(ColumnOptionPaneStyleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ColumnOptionPaneStyle.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ColumnOptionPaneStyleProperty =
            DependencyProperty.Register("ColumnOptionPaneStyle", typeof(Style), typeof(GridDataControlBaseImpl), new PropertyMetadata(null));

        #endregion


#endif

        #region RowStyle (DependencyProperty)

        /// <summary>
        /// Gets / sets the RowStyle.
        /// </summary>
        public Style RowStyle
        {
            get { return (Style)GetValue(RowStyleProperty); }
            set { SetValue(RowStyleProperty, value); }
        }

        public static readonly DependencyProperty RowStyleProperty = DependencyProperty.Register("RowStyle", typeof(Style), typeof(GridDataControlBaseImpl), new PropertyMetadata(null, OnRowStyleChanged));

        private static void OnRowStyleChanged(DependencyObject dpo, DependencyPropertyChangedEventArgs args)
        {
            var grid = dpo as GridDataControlBaseImpl;
            if (grid.EnableBlendStyling)
            {
                grid.evenRow.Style = (Style)args.NewValue;
                grid.InvalidateCells();
            }
        }

        #endregion

        #region AlternateRowStyle (DependencyProperty)

        /// <summary>
        /// Gets / sets the alternate row style.
        /// </summary>
        public Style AlternateRowStyle
        {
            get { return (Style)GetValue(AlternateRowStyleProperty); }
            set { SetValue(AlternateRowStyleProperty, value); }
        }

        public static readonly DependencyProperty AlternateRowStyleProperty = DependencyProperty.Register("AlternateRowStyle", typeof(Style), typeof(GridDataControlBaseImpl), new PropertyMetadata(null, OnAlternateRowStyleChanged));

        private static void OnAlternateRowStyleChanged(DependencyObject dpo, DependencyPropertyChangedEventArgs args)
        {
            var grid = dpo as GridDataControlBaseImpl;
            if (grid.EnableBlendStyling)
            {
                grid.oddRow.Style = (Style)args.NewValue;
                grid.InvalidateCells();
            }
        }

        #endregion

        #region EnableBlendStyling (DependencyProperty)

        /// <summary>
        /// Gets / sets enable blend styling.
        /// </summary>
        public bool EnableBlendStyling
        {
            get { return (bool)GetValue(EnableBlendStylingProperty); }
            set { SetValue(EnableBlendStylingProperty, value); }
        }

        public static readonly DependencyProperty EnableBlendStylingProperty = DependencyProperty.Register("EnableBlendStyling", typeof(bool), typeof(GridDataControlBaseImpl), new PropertyMetadata(false, OnEnableBlendStylingChanged));

        private static void OnEnableBlendStylingChanged(DependencyObject dpo, DependencyPropertyChangedEventArgs args)
        {
            var grid = dpo as GridDataControlBaseImpl;
            var value = (bool)args.NewValue;
            if (value)
            {
                grid.InitializeProxyContainers();
            }
            else
            {
                grid.ClearStyleFrame();
            }
        }

        #endregion

        private void InitializeProxyContainers()
        {
            this.frameworkElementsFrame = new VisualContainer();
            this.Children.Add(this.frameworkElementsFrame);
            this.evenRow = new GridDataRowControl();
            this.frameworkElementsFrame.Children.Add(evenRow);
            this.oddRow = new GridDataRowControl();
            this.frameworkElementsFrame.Children.Add(oddRow);
        }

        /*       private void InitializeProperties(GridRow row)
               {
       #if !SILVERLIGHT
                   var backgroundDp = DependencyPropertyDescriptor.FromProperty(GridRow.BackgroundProperty, typeof(GridRow));
                   backgroundDp.AddValueChanged(row, OnBackgroundChanged);

                   var foregroundDp = DependencyPropertyDescriptor.FromProperty(GridRow.ForegroundProperty, typeof(GridRow));
                   foregroundDp.AddValueChanged(row, OnForegroundChanged);
       #endif
               }

               private void UnwireEvents(GridRow row)
               {

                   // clear events
                   var backgroundDp = DependencyPropertyDescriptor.FromProperty(GridRow.BackgroundProperty, typeof(GridRow));
                   backgroundDp.RemoveValueChanged(row, OnBackgroundChanged);

                   var foregroundDp = DependencyPropertyDescriptor.FromProperty(GridRow.ForegroundProperty, typeof(GridRow));
                   foregroundDp.RemoveValueChanged(row, OnForegroundChanged);
              }       

               private void OnBackgroundChanged(object sender, EventArgs args)
               {
                   this.InvalidateCells();
               }

               private void OnForegroundChanged(object sender, EventArgs args)
               {
                   this.InvalidateCells();
               }*/

        private void ClearStyleFrame()
        {
            this.frameworkElementsFrame.Children.Clear();
        }

        internal VisualContainer FrameworkElementsFrame
        {
            get { return this.frameworkElementsFrame; }
        }

        #endregion

        protected override void OnQueryBaseStyles(GridQueryBaseStylesEventArgs e)
        {
            base.OnQueryBaseStyles(e);

            if (this.EnableBlendStyling)
            {
                var colIdx = this.TableModel.TableProperties.ShowRowHeader ? 1 : 0;
                colIdx = this.TableModel.Table.HasNestedTables ? colIdx + 1 : colIdx;
                colIdx = this.TableModel.Table.HasDetailsView ? colIdx + 1 : colIdx;

                if ((e.Cell.ColumnIndex >= colIdx) && e.Cell.RowIndex >= this.TableModel.ResolveStartIndexBasedOnPosition())
                {
                    var rowIndex = -1;
                    var canContinue = false;
                    if (this.TableModel.Table.HasGroups)
                    {
                        rowIndex = this.TableModel.ResolveIndexToGroupPosition(e.Cell.RowIndex);
                        var displayEl = rowIndex > -1 && rowIndex < this.TableModel.Table.GroupModel.DisplayElements.Count ? this.TableModel.Table.GroupModel.DisplayElements[rowIndex] : null;
                        if (displayEl != null)
                        {
                            canContinue = displayEl is RecordEntry && !(displayEl is SummaryRecordEntry);
                        }
                    }
                    else
                    {
                        rowIndex = this.TableModel.ResolveIndexToRecordPosition(e.Cell.RowIndex);
                        if (this.TableModel.View != null && this.TableModel.View.Records != null)
                        {
                            canContinue = rowIndex > -1 && rowIndex < this.TableModel.View.Records.Count ? true : false;
                        }
                    }

                    if (!(e.Cell.ColumnIndex < this.TableModel.ResolveDefaultColumnOffset()))
                    {
                        var colIndex = this.TableModel.ResolvePositionToVisibleColumnIndex(e.Cell.ColumnIndex);
                        canContinue = colIndex < this.TableModel.TableProperties.VisibleColumns.Count ? true : false;
                    }
                    else
                    {
                        canContinue = false;
                    }

                    if (canContinue)
                    {
                        var gridStyle = new GridStyleInfo();
                        // Changed to fix the borders not appearing on enabling the blend style issue fix
                        var hasEvenOdd = this.RowStyle != null || this.AlternateRowStyle != null;
                        if (hasEvenOdd)
                        {
                            this.evenRow.InitializeGridStyle(gridStyle);
                            e.BaseStyles.Add(gridStyle);
                        }

                        if (this.AlternateRowStyle != null && (e.Cell.RowIndex != this.TableModel.ResolveAddNewPositionInGrid()))
                        {
                            if (e.Cell.RowIndex % 2 == 0)
                            {
                                this.evenRow.InitializeGridStyle(gridStyle);
                            }
                            else
                            {
                                this.oddRow.InitializeGridStyle(gridStyle);
                            }
                        }
                    }
                }
            }
        }

        protected override void OnUnloaded(RoutedEventArgs e)
        {
            base.OnUnloaded(e);
        }
        private void InitSelectsCellsMouseController()
        {
            var selectController = this.MouseControllerDispatcher.Find("SelectCellsMouseController") as GridSelectCellsMouseController;
            if (selectController != null)
            {
                selectController.AdjustedRangeFunc = (type, r, c, selectedRange) =>
                {
                    int headercolumn = this.TableModel.HeaderColumns;

                    var style = this.Model[r, c] as GridDataStyleInfo;
                    var tableStyleIdentity = style.CellIdentity;
                    switch (type)
                    {
                        case GridSelectCellsMouseController.SelectionType.IsCells:
                            if (tableStyleIdentity.TableCellType != GridDataTableCellType.RecordCell && tableStyleIdentity.TableCellType != GridDataTableCellType.AddNewRecordCell && tableStyleIdentity.TableCellType != GridDataTableCellType.UnboundColumnCell && tableStyleIdentity.TableCellType != GridDataTableCellType.RowHeaderCell && tableStyleIdentity.TableCellType != GridDataTableCellType.UnboundRecordCell && tableStyleIdentity.TableCellType != GridDataTableCellType.FilterBarCell && tableStyleIdentity.TableCellType != GridDataTableCellType.DropDownFilterCell)
                            {
                                selectedRange = GridRangeInfo.Empty;
                            }

                            break;
                        case GridSelectCellsMouseController.SelectionType.IsRow:
                            if (((headercolumn <= c) &&
                                (tableStyleIdentity.TableCellType == GridDataTableCellType.RecordCell)
                                || tableStyleIdentity.TableCellType == GridDataTableCellType.AddNewRecordCell
                                || tableStyleIdentity.TableCellType == GridDataTableCellType.UnboundColumnCell
                                || tableStyleIdentity.TableCellType == GridDataTableCellType.UnboundRecordCell
                                || tableStyleIdentity.TableCellType == GridDataTableCellType.FilterBarCell
                                || tableStyleIdentity.TableCellType == GridDataTableCellType.DropDownFilterCell
                                || tableStyleIdentity.TableCellType == GridDataTableCellType.RecordPlusMinusCell))
                            {
                                var adjustValue = this.TableModel.Table.HasNestedTables ? 1 : 0;
                                var colOffset = this.TableModel.ResolveDefaultColumnOffset();
                                                                
                                selectedRange = selectedRange.ExpandRange(0, colOffset, this.Model.RowCount, this.Model.ColumnCount - adjustValue);

                            }
                            else if (tableStyleIdentity.TableCellType == GridDataTableCellType.GroupCaptionPlusMinusCell
                                || tableStyleIdentity.TableCellType == GridDataTableCellType.GroupCaptionCell
                                || tableStyleIdentity.TableCellType == GridDataTableCellType.GroupCaptionSummaryEmptyCell
                                || tableStyleIdentity.TableCellType == GridDataTableCellType.GroupCaptionSummaryTitleCell
                                || tableStyleIdentity.TableCellType == GridDataTableCellType.GroupCaptionSummaryRecordCell
                                || tableStyleIdentity.TableCellType == GridDataTableCellType.GroupCaptionSummaryCoveredCell
                                || tableStyleIdentity.TableCellType == GridDataTableCellType.SummaryCoveredCell
                                || tableStyleIdentity.TableCellType == GridDataTableCellType.SummaryEmptyCell
                                || tableStyleIdentity.TableCellType == GridDataTableCellType.SummaryRecordCell
                                || tableStyleIdentity.TableCellType == GridDataTableCellType.SummaryTitleCell
                                )
                            {
                                var adjustValue = this.TableModel.Table.HasNestedTables ? 1 : 0;
                                // var colOffset = this.TableModel.ResolveDefaultColumnOffset();
                                if (tableStyleIdentity.Group != null)
                                    selectedRange = selectedRange.ExpandRange(0, 0, this.Model.RowCount, this.Model.ColumnCount - adjustValue);
                            }
                            else if (tableStyleIdentity.TableCellType != GridDataTableCellType.RowHeaderCell && (tableStyleIdentity.TableCellType != GridDataTableCellType.RecordCell) || headercolumn <= c)
                            {
                                selectedRange = GridRangeInfo.Empty;
                            }

                            break;
                    }
                    return selectedRange;
                };
            }
        }

        internal override void RaisePrepareRenderCell(GridPrepareRenderCellEventArgs e)
        {
            base.RaisePrepareRenderCell(e);
#if !SILVERLIGHT
            var style = e.Style as GridRenderStyleInfo;//this.CurrentCell.Grid.Model[e.Cell.RowIndex, e.Cell.ColumnIndex];
            var currentStyle = style.ModelStyle;
#else
            var currentStyle = this.CurrentCell.Grid.Model[e.Cell.RowIndex, e.Cell.ColumnIndex]; ;
#endif
            if (currentStyle != null)
            {
#if !SILVERLIGHT
                if (this.TableModel.TableProperties.ShowHoveringBackground && !Model.SelectedRanges.AnyRangeContains(GridRangeInfo.Cell(e.Cell.RowIndex, e.Cell.ColumnIndex)))
                {
                    if (((currentStyle as GridDataStyleInfo).CellIdentity.TableCellType == GridDataTableCellType.RecordCell || (currentStyle as GridDataStyleInfo).CellIdentity.TableCellType == GridDataTableCellType.GroupCaptionCell))
                    {
                        //RowColumnIndex rci = new RowColumnIndex();
                        //rci.RowIndex = e.Cell.RowIndex;
                        //rci.ColumnIndex = e.Cell.ColumnIndex;
                        //Brush Background = e.Style.Background;
                        if (rowSpanUnderMouse != null)
                        {
                            if (rowSpanUnderMouse.Contains(style.CellRowColumnIndex))
                            {
                                if ((currentStyle as GridDataStyleInfo).CellIdentity.TableCellType == GridDataTableCellType.RecordCell)
                                {
                                    e.Style.Background = this.TableModel.GetHoveringRecordCellBackground();
                                    e.Style.Foreground = this.TableModel.GetHoveringRecordCellForeground();
                                }
                                if ((currentStyle as GridDataStyleInfo).CellIdentity.TableCellType == GridDataTableCellType.GroupCaptionCell)
                                {
                                    e.Style.Background = this.TableModel.GetHoveringGroupCaptionCellBackground();
                                }
                            }
                        }
                        //if (oldrowSpanUnderMouse != null)
                        //{
                        //    if (oldrowSpanUnderMouse.Contains(rci))
                        //    {
                        //        e.Style.Background = Background;
                        //    }
                        //}
                    }
                }
#endif
                if (!this.TableModel.TableProperties.IsLegacyStyleEnabled)
                {
                    var tableStyleIdentity = (currentStyle as GridDataStyleInfo).CellIdentity;                    

                    if (Model.Options != null && (Model.Options.DrawSelectionOptions & (GridDrawSelectionOptions.ReplaceBackground | GridDrawSelectionOptions.ReplaceTextColor)) != 0)
                    {
                        if (e.Cell.RowIndex >= Model.HeaderRows && e.Cell.ColumnIndex >= Model.HeaderColumns)
                        {
                            if (tableStyleIdentity.TableCellType == GridDataTableCellType.GroupCaptionPlusMinusCell
                        || tableStyleIdentity.TableCellType == GridDataTableCellType.RecordPlusMinusCell                        
                        ||tableStyleIdentity.TableCellType==GridDataTableCellType.EmptyCell
                        || tableStyleIdentity.TableCellType == GridDataTableCellType.NestedTableEmptyCell)                        
                            return;
                        
                            if (Model.SelectedRanges.AnyRangeContains(GridRangeInfo.Cell(e.Cell.RowIndex, e.Cell.ColumnIndex)))
                            {                                
                                if ((Model.Options.DrawSelectionOptions & GridDrawSelectionOptions.ReplaceBackground) != 0)
                                {
                                    
                                    if (tableStyleIdentity.TableCellType == GridDataTableCellType.GroupCaptionCell                                        
                                        || tableStyleIdentity.TableCellType == GridDataTableCellType.GroupCaptionSummaryEmptyCell
                                        || tableStyleIdentity.TableCellType == GridDataTableCellType.GroupCaptionSummaryTitleCell
                                        || tableStyleIdentity.TableCellType == GridDataTableCellType.GroupCaptionSummaryRecordCell
                                        || tableStyleIdentity.TableCellType == GridDataTableCellType.GroupCaptionSummaryCoveredCell
                                        || tableStyleIdentity.TableCellType == GridDataTableCellType.SummaryCoveredCell
                                        || tableStyleIdentity.TableCellType == GridDataTableCellType.SummaryEmptyCell
                                        || tableStyleIdentity.TableCellType == GridDataTableCellType.SummaryRecordCell
                                        || tableStyleIdentity.TableCellType == GridDataTableCellType.SummaryTitleCell)
                                    {
                                        e.Style.Background = this.TableModel.GetGroupCaptionSelectionBackground();
                                    }
                                    else if (CurrentCell.RowIndex == e.Style.RowIndex && CurrentCell.ColumnIndex == e.Style.ColumnIndex)
                                    {
                                        if (tableStyleIdentity.TableCellType == GridDataTableCellType.FilterBarCell || tableStyleIdentity.TableCellType == GridDataTableCellType.DropDownFilterCell)
                                        { }
                                        else if (this.Model.Options.ShowCurrentCell)
                                        {
#if SILVERLIGHT
                                            if (this.CurrentCell.IsEditing)
                                            {
                                                e.Style.Background = Brushes.White;
                                            }
                                            else
#else
                                            if (this.CurrentCell.IsEditing && !this.TableModel.TableProperties.EnableVisualStyleForEditors)
                                                e.Style.Background = Brushes.White;
                                            else
#endif
                                                e.Style.Background = this.TableModel.GetCurrentCellSelectionBackground();
                                        }
                                    }
                                }
                                if ((Model.Options.DrawSelectionOptions & GridDrawSelectionOptions.ReplaceTextColor) != 0)
                                {
                                    if (tableStyleIdentity.TableCellType == GridDataTableCellType.GroupCaptionPlusMinusCell
                                        || tableStyleIdentity.TableCellType == GridDataTableCellType.GroupCaptionCell
                                        || tableStyleIdentity.TableCellType == GridDataTableCellType.GroupCaptionSummaryEmptyCell
                                        || tableStyleIdentity.TableCellType == GridDataTableCellType.GroupCaptionSummaryTitleCell
                                        || tableStyleIdentity.TableCellType == GridDataTableCellType.GroupCaptionSummaryRecordCell
                                        || tableStyleIdentity.TableCellType == GridDataTableCellType.GroupCaptionSummaryCoveredCell
                                        || tableStyleIdentity.TableCellType == GridDataTableCellType.SummaryCoveredCell
                                        || tableStyleIdentity.TableCellType == GridDataTableCellType.SummaryEmptyCell
                                        || tableStyleIdentity.TableCellType == GridDataTableCellType.SummaryRecordCell
                                        || tableStyleIdentity.TableCellType == GridDataTableCellType.SummaryTitleCell)
                                    {
                                        e.Style.Foreground = this.TableModel.GetGroupCaptionSelectionForeground();
                                    }
                                    else if (CurrentCell.RowIndex == e.Style.RowIndex && CurrentCell.ColumnIndex == e.Style.ColumnIndex &&
                                        (currentStyle as GridDataStyleInfo).CellIdentity.TableCellType != GridDataTableCellType.FilterBarCell
                                        && Model.Options.ShowCurrentCell)
                                    {
#if SILVERLIGHT
                                        if (this.CurrentCell.IsEditing)
                                        {
                                            e.Style.Foreground = Brushes.Black;
                                        }
                                        else
#else
                                        if (this.CurrentCell.IsEditing && !this.TableModel.TableProperties.EnableVisualStyleForEditors)
                                            e.Style.Foreground = Brushes.Black;
                                        else
#endif
                                            e.Style.Foreground = this.TableModel.GetCurrentCellSelectionForeground();
                                        
                                    }
                                }
                            }
                        }
                    }
                }

#if SILVERLIGHT
                if ((currentStyle as GridDataStyleInfo).CellIdentity.TableCellType == GridDataTableCellType.RowHeaderCell)
                {
                    e.Style.Background = (this.Model as GridDataTableModel).GetRowHeaderBackground(); 
                }
                if ((currentStyle as GridDataStyleInfo).CellIdentity.TableCellType == GridDataTableCellType.ColumnHeaderIndentCell ||
                    (currentStyle as GridDataStyleInfo).CellIdentity.TableCellType == GridDataTableCellType.TopLeftHeaderCell)
                {
                    e.Style.Background = (this.Model as GridDataTableModel).GetHeaderBackground();
                }
#endif
            }
        }
        
//We have already set the currentcellbackgroung from RaisePrepareRenderCell() method, so no need this code.
//#if SILVERLIGHT
//        internal override Brush GetCurrentCellBackground()
//        {
//            var currentStyle = this.Model[this.CurrentCell.RowIndex, this.CurrentCell.ColumnIndex];
//            if (!this.TableModel.TableProperties.IsLegacyStyleEnabled)
//            {
//                if ((Model.Options.DrawSelectionOptions & (GridDrawSelectionOptions.ReplaceBackground | GridDrawSelectionOptions.ReplaceTextColor | GridDrawSelectionOptions.AlphaBlend)) != 0)
//                {
//                    if (this.CurrentCell.RowIndex >= Model.HeaderRows && this.CurrentCell.ColumnIndex >= Model.HeaderColumns)
//                    {
//                        if (Model.SelectedRanges.AnyRangeContains(GridRangeInfo.Cell(this.CurrentCell.RowIndex, this.CurrentCell.ColumnIndex)))
//                        {
//                            if ((Model.Options.DrawSelectionOptions & GridDrawSelectionOptions.ReplaceBackground) != 0)
//                            {
//                                if (((currentStyle as GridDataStyleInfo).CellIdentity.TableCellType == GridDataTableCellType.GroupCaptionCell ||
//                                    (currentStyle as GridDataStyleInfo).CellIdentity.TableCellType == GridDataTableCellType.GroupCaptionPlusMinusCell))
//                                    return this.TableModel.GetGroupCaptionSelectionBackground();
//                                else if (CurrentCell.RowIndex == this.CurrentCell.RowIndex && CurrentCell.ColumnIndex == this.CurrentCell.ColumnIndex)
//                                    return this.TableModel.GetCurrentCellSelectionBackground();
//                            }
//                        }
//                    }
//                }
//            }
//            return base.GetCurrentCellBackground();
//        }
//#endif

#if !SILVERLIGHT

        protected override bool CanDrawHorizontalLineFirst()
        {
            if (Model != null)
            {
                if((Model as GridDataTableModel).TableProperties.IsLegacyStyleEnabled)
                {
                    return false;
                }

                return true;
            }

            return false;
        }
#endif

        protected internal override bool ShouldGridTryToHandlePreviewKeyDown(System.Windows.Input.KeyEventArgs e)
        {
            //var currentCell = this.CurrentCell;
            //// if the current cell is not a nested grid then proceed else return false
            if (e.Key != Key.Delete)
            {
                return base.ShouldGridTryToHandlePreviewKeyDown(e);
            }

            return false;
        }

        public override bool MoveCurrentCellWithArrowKey(System.Windows.Input.KeyEventArgs e)
        {

            // This totally breaking the tab navigations for Add new Row and Grid when the Allow Edit is Set to False.             
            //if (!this.TableModel.TableProperties.AllowEdit && e.Key == Key.Tab )
            //{
            //    return false;
            //}

            // var range = this.CurrentCell.RangeInfo; Unused local variable
            var isInAddNewRow = this.TableModel.CurrencyManager.IsInAddNewRow;
            var isInFilterBarRow = this.TableModel.ResolveFilterBarPositionInGrid() == this.CurrentCell.RowIndex;
            if (isInFilterBarRow)
            {
                return base.MoveCurrentCellWithArrowKey(e);
            }

            //if (isInAddNewRow && this.TableModel.View.Records.Count > 0)
            //{
            //    RowColumnIndex rc = CurrentCell.CellRowColumnIndex;
            //    GridRangeInfo gridCells = this.NavigateWithArrowKeysCellsRange;
            //    bool isLastCol = (rc.ColumnIndex == gridCells.Right);
            //    bool isLastRow = (rc.RowIndex == gridCells.Bottom);
            //    if (isLastCol && isLastRow)
            //    {
            //        this.TableModel.CurrencyManager.isLastRowCol = true;
            //        this.TableModel.CurrencyManager.EndEdit();
            //        this.TableModel.CurrencyManager.isLastRowCol = false;
            //        return base.MoveCurrentCellWithArrowKey(e);
            //    }
            //}

            //// we are handling the Key.Enter && Key.Escape in the CurrentRecordManager

            if ((e.Key == System.Windows.Input.Key.Up ||
                         e.Key == System.Windows.Input.Key.Down ||
                         e.Key == System.Windows.Input.Key.Left ||
                         e.Key == System.Windows.Input.Key.Right) && !isInAddNewRow && this.TableModel.CurrencyManager.UpdateMode!= UpdateMode.RowCachedMode)
            {
                if (this.TableModel != null
                    && this.TableModel.CurrencyManager != null
                    && this.TableModel.CurrencyManager.IsEditing)
                {
                    if (this.TableModel is GridDataChildTableModel)
                    {
                        if (this.TableModel.Grid != null && object.Equals(this.TableModel, this.TableModel.Grid.Model))
                        {
                            this.TableModel.CurrencyManager.ConfirmChanges();
                        }
                    }
                    else
                    {
                        if (this.TableModel.CurrencyManager.CurrentCell.IsValid && this.TableModel.CurrencyManager.CurrentCell.IsModified)
                            this.TableModel.CurrencyManager.ConfirmChanges();
                    }
                }
            }

            if ((!isInAddNewRow && e.Key != Key.Enter)
                || (isInAddNewRow && e.Key != Key.Enter && e.Key != Key.Escape  ) ||(e.Key == Key.Tab && !isInAddNewRow) ||(isInAddNewRow && e.Key == Key.Tab && this.TableModel.ColumnCount != this.CurrentCell.ColumnIndex + 1))
            {
                var k = base.MoveCurrentCellWithArrowKey(e);
                if (CurrentCell.Renderer is GridDataCellNestedGridRenderer)
                {
#if !SILVERLIGHT
                    var ChildModel = CurrentCell.Renderer.CurrentCellUIElement as GridDataCellNestedGridEditor;
                    if (ChildModel != null)
                        ChildModel.IsInShiftTab = this.IsInShiftTab;
#endif
                    if (CurrentCell.Renderer.IsFocusable)
                        CurrentCell.Renderer.IsFocused = true;
                }
                if (this.NavigateWithArrowKeysCellsRange.Contains(this.CurrentCell.RangeInfo)
#if !SILVERLIGHT
 && e.KeyboardDevice.Modifiers == ModifierKeys.Shift
#endif
 && (e.Key == Key.Up || e.Key == Key.Down)
                    && this.TableModel.Options.ListBoxSelectionMode == GridSelectionMode.MultiExtended)
                {
                    this.InvalidateCell(GridRangeInfo.Col(this.CurrentCell.RangeInfo.Left));
                }

                return k;
            }
            //else if (isInAddNewRow && e.Key != Key.Enter && e.Key != Key.Escape)
            //{
            //    var k = base.MoveCurrentCellWithArrowKey(e);
            //    if (this.NavigateWithArrowKeysCellsRange.Contains(this.CurrentCell.RangeInfo))
            //    {
            //        this.InvalidateCell(GridRangeInfo.Col(this.CurrentCell.RangeInfo.Left));
            //    }

            //    return k;
            //}

            return e.Handled;
        }

        /// <summary>
        /// The range of cells in the grid that can be navigated to using arrow keys.
        /// This range can but does not need to include header and footer rows and columns.
        /// The default scenario is that header and footer rows and columns are excluded
        /// <para/>
        /// Cells outside the range can still be clicked on and be made the current cell but they
        /// will be skipped when the user navigate with arrow keys.
        /// </summary>
        /// <value>The grid cells range.</value>
        /// <remarks>
        /// When the user clicks on a cell it will be made the current cell through
        /// a <see cref="GridCurrentCell.MoveTo"/> call when the cell is enabled.
        /// When the user navigates with arrow keys the next current cell is determined
        /// by looping in the direction and skipping disabled cells with the <see cref="GridCurrentCell.Move"/>
        /// method. The Move method checks this range to determine when to stop searching
        /// in a given direction.
        /// </remarks>
        public override GridRangeInfo NavigateWithArrowKeysCellsRange
        {
            get
            {
                //adjust the first column CurrentCell movement while Key Navigation
                int leftColIdx = 0;                
                if (this.Model is GridDataTableModel)
                {
                    leftColIdx += (this.Model as GridDataTableModel).Table.HasNestedTables && (this.Model as GridDataTableModel).TableProperties.ShowRecordPlusMinus ? 1 : 0;
                    leftColIdx += (this.Model as GridDataTableModel).Table.HasGroups ? (this.Model as GridDataTableModel).View.GroupDescriptions.Count : 0;
                    leftColIdx += (this.Model as GridDataTableModel).Table.HasDetailsView ? 1 : 0;
                }
                return GridRangeInfo.Cells(
                    //this.TableModel.TableProperties.StackedHeaderRows.Count + 
                    Model.HeaderRows,
                    Model.HeaderColumns + leftColIdx,
                    RowHeights.LineCount - (RowHeights.FooterLineCount + 1),
                    ColumnWidths.LineCount - (ColumnWidths.FooterLineCount + 1)
                );
            }
        }

        /// <summary>
        /// Gets the table model.
        /// </summary>
        /// <value>The table model.</value>
        public GridDataTableModel TableModel
        {
            get
            {
                return this.Model as GridDataTableModel;
            }
        }

        private GridDataStyleManager styleManager;

        internal GridDataStyleManager StyleManager
        {
            get
            {
                return styleManager;
            }
            set
            {
                bool canInvalidate = value != styleManager;

                styleManager = value;

                if (canInvalidate)
                    InvalidateCells();
            }
        }

        protected override bool ShouldRenderCurrentCellBorder()
        {
            var currentCell = this.CurrentCell;
            if (currentCell.Renderer != null && currentCell.Renderer.HasCurrentCellState)
            {
                var style = currentCell.Renderer.CurrentStyle.ModelStyle as GridDataStyleInfo;
                if (style != null && style.CellIdentity != null && (style.CellIdentity.TableCellType == GridDataTableCellType.RecordCell || style.CellIdentity.TableCellType == GridDataTableCellType.AddNewRecordCell ||
                    style.CellIdentity.TableCellType == GridDataTableCellType.FilterBarCell||style.CellIdentity.TableCellType== GridDataTableCellType.DropDownFilterCell || 
                    style.CellIdentity.TableCellType == GridDataTableCellType.UnboundColumnCell))
                {
                    return base.ShouldRenderCurrentCellBorder();
                }

                return false;
            }

            return base.ShouldRenderCurrentCellBorder();
        }

        public override GridRangeInfo ExpandSelectedCellsRange(GridRangeInfo selectedCells)
        {
            GridDataTableModel model = this.Model as GridDataTableModel;
            if (model != null)
            {
                if (model.Table.HasNestedTables)
                {
                    return selectedCells.ExpandRange(0, 0, Model.RowCount, model.ColumnCount - 1);
                }
            }
            //this.PlusHandler();
            return selectedCells.ExpandRange(0, 0, Model.RowCount, Model.ColumnCount);
        }

        protected override void OnRaiseQueryAllowDragColumn(GridQueryDragColumnHeaderEventArgs args)
        {
            var colIdx = this.TableModel.ResolvePositionToVisibleColumnIndex(args.Column);
            GridDataVisibleColumn visibleCol = colIdx > -1 && colIdx < this.TableModel.TableProperties.VisibleColumns.Count ? this.TableModel.TableProperties.VisibleColumns[colIdx] : null;
            if (visibleCol != null)
            {
                args.AllowDrag = visibleCol.AllowDrag;
            }
            base.OnRaiseQueryAllowDragColumn(args);

            if (args.Handled)
            {
                return;
            }

            if (args.Reason == GridQueryDragColumnHeaderReason.HitTest || args.Reason == GridQueryDragColumnHeaderReason.MouseUp)
            {
                var colIndex = this.TableModel.ResolvePositionToVisibleColumnIndex(args.Column);
                if (colIndex < 0)
                {
                    args.AllowDrag = false;
                }
            }
            else if (args.Reason == GridQueryDragColumnHeaderReason.MouseMove)
            {
                var colIndex = this.TableModel.ResolvePositionToVisibleColumnIndex(args.InsertBeforeColumn);
                if (colIndex < 0)
                {
                    args.AllowDrag = false;
                }
            }
        }

        protected override void OnResizingRows(GridResizingRowsEventArgs args)
        {
            base.OnResizingRows(args);

            if (!args.AllowResize || args.Handled)
                return;

            for (int TopRow = args.Rows.Top; (TopRow <= args.Rows.Bottom && args.Reason != GridResizeCellsReason.HitTest && !args.Rows.IsEmpty); TopRow++)
            {                
                    var colIdx = args.Rows.Left;
                    if (args.AllowResize && TopRow > 0)
                    {
                        if (args.Height < 5 && TopRow == 1)
                            args.Height = 5;
                        if (args.Reason == GridResizeCellsReason.MouseMove)
                        {
                            var Column = this.TableModel.TableProperties.VisibleColumns[colIdx];
                            bool PrevVal = Column.IsInSuspend;
                            Column.IsInSuspend = true;
                            Column.IsHidden = false;
                            Column.IsInSuspend = PrevVal;
                        }
                        else if (args.Reason == GridResizeCellsReason.MouseUp)
                            this.TableModel.RowHeights[TopRow] = args.Height;

                        else if (args.Reason == GridResizeCellsReason.DoubleClick)
                        {
                            this.TableModel.TableProperties.SuspendEvents();
                            this.TableModel.RowHeights[TopRow] = this.TableModel.TableProperties.DefaultHeaderRowHeight;
                            this.TableModel.TableProperties.ResumeEvents();
                            args.AllowResize = false;
                        }
                    }
                    else
                        args.AllowResize = false;                        
            }
        }

        protected override void OnResizingColumns(GridResizingColumnsEventArgs args)
        {
            base.OnResizingColumns(args);
            if (!args.AllowResize)
            {

                return;
            }
            if (args.Handled)
            {
                return;
            }
            // handling args.Reason == HitTest, actually enables resizing for all cells in that column, so do not handle it here
            if (args.Reason != GridResizeCellsReason.HitTest && !args.Columns.IsEmpty)
            {
                for (int i = args.Columns.Left; i <= args.Columns.Right; i++)
                {
                    var colIdx = this.TableModel.ResolvePositionToVisibleColumnIndex(i);
                    // var isAutoOnLoad = this.TableModel.ColumnAutoSizer.IsAutoOnLoad; Unused local variable
                    GridDataVisibleColumn visibleCol = colIdx > -1 && colIdx < this.TableModel.TableProperties.VisibleColumns.Count ? this.TableModel.TableProperties.VisibleColumns[colIdx] : null;
                    int hiddenCount = this.TableModel.HiddenColRanges.Count(v => v.Left < args.Columns.Left);
                    var hasGroup = this.TableModel.Table.HasGroups && !(this.TableModel.Table.HasDetailsView || this.TableModel.Table.HasNestedTables);
                    var x = hasGroup ? hiddenCount + 1 : hiddenCount;
                    if (visibleCol != null)
                    {
                        args.AllowResize = visibleCol.AllowResize && !visibleCol.AutoFit;
                        if (args.AllowResize)
                        {
                            if (colIdx == x && args.Width < 5)
                            {
                                args.Width = 5;
                            }
                            if (this.TableModel.Table.HasNestedTables && this.TableModel.ChildTableModelCollection.Count > 0)
                            {
                                var childTotalExt = this.TableModel.ChildTableModelCollection[0].ColumnWidths.TotalExtent;
                                foreach (var collection in this.TableModel.ChildTableModelCollection)
                                {
                                    if (collection.ColumnWidths.TotalExtent > childTotalExt)
                                        childTotalExt = collection.ColumnWidths.TotalExtent;
                                }
                                var parentTotalExt = this.Model.ColumnWidths.TotalExtent;
                                var extraWidth = 0.0;
                                if (this.Model.ActiveGridView != null)
                                {
                                    for (int column = 0; column < this.Model.ActiveGridView.NavigateWithArrowKeysCellsRange.Left; column++)
                                        extraWidth += this.Model.ColumnWidths[column];
                                }
                                if (this.TableModel.TableProperties.AllowNestedGridPadding)
                                    extraWidth += 8;
                                var deltaWidth = childTotalExt - (parentTotalExt - (extraWidth + this.ScrollColumns.GetLineSize(i)));
                                if (args.Width < deltaWidth)
                                {
                                    args.Width = deltaWidth;
                                }
                            }
                            if (visibleCol.Width.UnitType == GridControlLengthUnitType.None || visibleCol.Width.UnitType == GridControlLengthUnitType.Star)
                            {
                                if (args.Width <= visibleCol.MinimumWidth)
                                {
                                    args.Width = visibleCol.MinimumWidth;

                                }
                                if ((args.Width >= visibleCol.MaximumWidth) && (visibleCol.MaximumWidth != 0))
                                {
                                    args.Width = visibleCol.MaximumWidth;
                                }
                               if (args.Width == 0)
                                    visibleCol.IsHidden = true;
                            }
                            if (args.Reason == GridResizeCellsReason.MouseMove
#if !SILVERLIGHT
 && args.InHiddenColResize
#endif
)
                            {
                                bool prevValue = visibleCol.IsInSuspend;

                                visibleCol.IsInSuspend = true;
                                visibleCol.IsHidden = false;

                                visibleCol.IsInSuspend = prevValue;
                            }
                            else if (args.Reason == GridResizeCellsReason.MouseUp)
                            {
                                //if (visibleCol.Width.IsNone || isAutoOnLoad)
                                {
                                    visibleCol.Width = new GridDataControlLength(args.Width);
                                }

                                //Commented to allow resize for all type of widths.
                                //if (!isAutoOnLoad)
                                //{
                                //    if (visibleCol.Width.UnitType == GridControlLengthUnitType.Star)
                                //    {
                                //        this.TableModel.ColumnAutoSizer.SetResizedColumnWidth(visibleCol, args.Width);
                                //    }
                                //    else
                                //    {
                                //        this.TableModel.TableProperties.SuspendEvents();
                                //        this.TableModel.ColumnAutoSizer.SetWidthAndStar(visibleCol);
                                //        this.TableModel.TableProperties.ResumeEvents();
                                //        args.Width = visibleCol.ActualWidth;
                                //    }
                                //}

#if SILVERLIGHT
                                args.Handled = true;
                                args.AllowResize = false;
#endif
                            }
                            else if (args.Reason == GridResizeCellsReason.DoubleClick)
                            {
                                this.TableModel.TableProperties.SuspendEvents();
                                var maxLength = this.Model.RowCount;
                                if (this.Model.Options.MaxLength > 0 && this.Model.Options.MaxLength <= this.Model.RowCount)
                                {
                                    maxLength = this.Model.Options.MaxLength;
                                }
                                var range = GridRangeInfo.Cells(0, i, maxLength, i);
                                this.TableModel.ResizeDataColumnsToFit(range, GridResizeToFitOptions.None);
                                this.TableModel.TableProperties.ResumeEvents();
                                args.AllowResize = false;
                            }
                        }
                    }
                    else
                    {
                        //Disable resizing for other columns(Expander Column, Row Header)
                        args.AllowResize = false;
                    }
                }
            }
        }

#if !SILVERLIGHT
        protected override void OnCurrentCellActivating(GridCurrentCellActivatingEventArgs e)
        {
            base.OnCurrentCellActivating(e);
            // workaround to get the Templated cells work fine with the CurrentCell architecture, CurrentCellMove is getting called after the Activating here, SD3565
            var colIdx = this.TableModel.ResolvePositionToVisibleColumnIndex(e.CellRowColumnIndex.ColumnIndex);
            if (colIdx > -1 && colIdx < this.TableModel.TableProperties.VisibleColumns.Count - 1)
            {
                var column = this.TableModel.TableProperties.VisibleColumns[colIdx];
                if (column.CellEditItemTemplate != null && column.CellItemTemplate != null && e.ActivateOptions.IsActivateTriggeredByMouseDownIntoUIElement)
                {
                    e.ActivateOptions.IsActivateTriggeredByMouseDownIntoUIElement = false;
                }
            }
        }
#else
        protected override void OnCurrentCellActivated()
        {
            base.OnCurrentCellActivated();
            // get the ScrollControlChildFrame for the header cell
            var headerRowIndex = this.TableModel.TableProperties.StackedHeaderRows.Count;
            var headerColIndex = this.CurrentCell.ColumnIndex;
            var cellElements = this.ArrangedCellUIElements.GetCellUIElements(headerRowIndex, headerColIndex);
            if (cellElements != null)
            {
                var el = cellElements.UIElements.Count > 0 ? cellElements.UIElements[0] : null;
                if (el != null)
                {
                    var header = el as GridDataHeaderCellControl;
                    if (header != null)
                    {
                        header.CloseFilterDropDown();
                        var parent = header.FindParentElementOfType<ScrollControlChildFrame>();
                        var headerItems = parent.FindElementsOfType<GridDataHeaderCellControl>().Where(h => h != null && h.IsDropDownOpen).ToList();
                        for (int i = 0; i < headerItems.Count; i++)
                        {
                            headerItems[i].CloseFilterDropDown();
                        }
                    }
                }
            }
        }
#endif

        //protected override System.Windows.Automation.Peers.AutomationPeer OnCreateAutomationPeer()
        //{
        //    return new GridDataControlAutomationPeer(this);
        //}
#if !SILVERLIGHT
        public override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
#else 
        public void Dispose()
        {
#endif
            if(this.MouseControllerDispatcher != null)
            {
                this.MouseControllerDispatcher.Dispose();
            }
        }
    }



    public class GridDataRowControl : Control
    {
        public GridDataRowControl()
        {
            IsTabStop = false;
#if !SILVERLIGHT
            KeyboardNavigation.SetIsTabStop(this, false);
#endif
        }

        public virtual void InitializeGridStyle(GridStyleInfo gridStyle)
        {
            // background
            gridStyle.Background = this.Background;

            // foreground
            gridStyle.Foreground = this.Foreground;

            //Font setttings
            gridStyle.Font.FontFamily = this.FontFamily;
            gridStyle.Font.FontSize = this.FontSize;
            gridStyle.Font.FontStretch = this.FontStretch;
            gridStyle.Font.FontStyle = this.FontStyle;
            gridStyle.Font.FontWeight = this.FontWeight;

            //Flow direction 
#if !SILVERLIGHT
            gridStyle.FlowDirection = this.FlowDirection;
#endif
            gridStyle.Padding = new Windows.Controls.Cells.CellMarginsInfo(this.Padding);
            gridStyle.Tag = this.Tag;

#if !SILVERLIGHT
            gridStyle.ToolTip = this.ToolTip;
#elif SILVERLIGHT
            gridStyle.ToolTip = System.Windows.Controls.ToolTipService.ToolTipProperty;
#endif
            //Alignments
            gridStyle.VerticalAlignment = this.VerticalAlignment;
            gridStyle.HorizontalAlignment = this.HorizontalAlignment;

            //BorderBrush

            gridStyle.Borders.Top = new Pen(this.BorderBrush, this.BorderThickness.Top);
            gridStyle.Borders.Left = new Pen(this.BorderBrush, this.BorderThickness.Left);
            gridStyle.Borders.Bottom = new Pen(this.BorderBrush, this.BorderThickness.Bottom);
            gridStyle.Borders.Right = new Pen(this.BorderBrush, this.BorderThickness.Right);

        }
    }

}
