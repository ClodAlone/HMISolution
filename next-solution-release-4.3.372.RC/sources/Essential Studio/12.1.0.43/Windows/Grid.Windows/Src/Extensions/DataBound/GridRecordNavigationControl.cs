//-------------------------------------------------------------------------------------------------
// <copyright file="GridRecordNavigationControl.cs" company="syncfusion">
//  Copyright (c) Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
// </copyright>
//-------------------------------------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

using Syncfusion.Diagnostics;
using Syncfusion.Windows.Forms;

namespace Syncfusion.Windows.Forms.Grid
{
    /// <summary>
    ///    Provides a <see cref="RecordNavigationControl"/> with support for a <see cref="GridControlBase"/>.
    /// </summary>
    [ToolboxItem(true)]
    [System.Drawing.ToolboxBitmap(typeof(Syncfusion.Windows.Forms.Grid.GridRecordNavigationControl), "ToolboxIcons.GridRecordNavigationControl.bmp")]
    [Description("Represents a control with splitter frame and NavigationBar on the bottom left scrollbar")]
    public class GridRecordNavigationControl : RecordNavigationControl, ISplitterPaneFactory
    {
        private GridControlBase _gridControl;
        private bool inUpdate = false;
        ////private int savedRecord = -1;
        private bool mouse = false;

        /// <override/>
        /// <summary>Gets or sets the site of the control.</summary>
        public override System.ComponentModel.ISite Site
        {
            get
            {
                return base.Site;
            }

            set
            {
                base.Site = value;
            }
        }

        /// <overload>
        /// Initializes a <see cref="GridRecordNavigationControl"/>.
        /// </overload>
        /// <summary>
        /// Initializes a <see cref="GridRecordNavigationControl"/>.
        /// </summary>
        public GridRecordNavigationControl()
            : this(null)
        {
            try
            {
                AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(AssemblyInfo.AssemblyResolver);
                new Syncfusion.Core.Licensing.LicensedComponent(typeof(GridRecordNavigationControl));
            }
            finally
            {
                AppDomain.CurrentDomain.AssemblyResolve -= new ResolveEventHandler(AssemblyInfo.AssemblyResolver);
            }
        }

        /// <summary>
        /// Initializes a <see cref="GridRecordNavigationControl"/> and associates it with a grid.
        /// </summary>
        /// <param name="gridControl">The grid to be displayed in this record navigation control.</param>
        public GridRecordNavigationControl(GridControlBase gridControl)
        {
            if (gridControl != null)
            {
                this._gridControl = gridControl;
                Controls.Add(gridControl);
                WireGrid(gridControl);
                this.ThemesEnabled = gridControl.ThemesEnabled;
            }

            this.ShowToolTips = true;
            this.SplitBars = Syncfusion.Windows.Forms.DynamicSplitBars.Both;
            this.SplitterPaneFactory = this;
            this.NavigationBar.ButtonBarMouseDown += new MouseEventHandler(BarMouseDown);
            this.NavigationBar.MouseUp += new MouseEventHandler(BarMouseUp);
        }

        /// <override/>
        protected override void Dispose(bool disposing)
        {
#if DEBUG
            if (Switches.Development.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(disposing);
            }
#else
            ;
#endif
            if (disposing)
            {
                UnwireGrid(_gridControl);
                foreach (Control ctrl in this.Controls)
                {
                    ctrl.Dispose();
                }
            }

            base.Dispose(disposing);
        }

        /// <override/>
        protected override void OnControlAdded(ControlEventArgs e)
        {
#if DEBUG
            if (Switches.Development.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(e.Control);
            }
#else
            ;
#endif
            base.OnControlAdded(e);
            if (_gridControl == null && e.Control is GridControlBase)
            {
                _gridControl = (GridControlBase)e.Control;
                _gridControl.FillSplitterPane = true;
                Controls.Add(_gridControl);
                WireGrid(_gridControl);
                GridDataBoundGrid dataGrid = e.Control as GridDataBoundGrid;
                if (dataGrid != null)
                {
                    this.NavigationBar.MaxRecord = dataGrid.Binder.RecordCount;
                    this.NavigationBar.MinRecord = 1;
                    this.NavigationBar.AllowAddNew = dataGrid.Binder.AllowAddNew;
                    this.NavigationBar.Enabled = NavigationBar.MaxRecord > 0 || this.NavigationBar.AllowAddNew;
                }
                else
                {
                    this.NavigationBar.MaxRecord = this.RowIndexToRecord(_gridControl, _gridControl.Model.RowCount);
                }
            }
        }

        /// <summary>
        /// Implements hooks with the specified <see cref="GridControlBase"/> for current cell movement.
        /// </summary>
        /// <param name="gridControl">The grid control to listen to.</param>
        public void WireGrid(GridControlBase gridControl)
        {
#if DEBUG
            if (Switches.Development.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(gridControl);
            }
#else
            ;
#endif
            if (gridControl != null)
            {
                GridDataBoundGrid dataGrid = gridControl as GridDataBoundGrid;
                if (dataGrid != null)
                {
                    dataGrid.RowEnter += new GridRowEventHandler(DataGridEnterRow);
                }

                gridControl.CurrentCellActivated += new EventHandler(GridCurrentCellActivated);

                gridControl.CurrentCellMoving += new GridCurrentCellMovingEventHandler(GridCurrentCellMoving);
                gridControl.ScrollInfoChanged += new EventHandler(GridScrollInfoChanged);
                gridControl.ThemeChanged += new EventHandler(Theme_Changed);
                gridControl.VisibleChanged += new EventHandler(GridVisibleChanged);
                gridControl.Disposed += new EventHandler(gridControl_Disposed);
                this.NavigationBar.RnbData = gridControl as IRecordNavigationBarData;
            }
        }

        /// <summary>
        /// Reset hooks with the specified <see cref="GridControlBase"/>.
        /// </summary>
        /// <param name="gridControl">The grid control to listen to.</param>
        public void UnwireGrid(GridControlBase gridControl)
        {
#if DEBUG
            if (Switches.Development.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(gridControl);
            }
#else
            ;
#endif
            if (gridControl != null)
            {
                GridDataBoundGrid dataGrid = gridControl as GridDataBoundGrid;
                if (dataGrid != null)
                {
                    dataGrid.RowEnter -= new GridRowEventHandler(DataGridEnterRow);
                }

                gridControl.CurrentCellActivated -= new EventHandler(GridCurrentCellActivated);
                gridControl.ScrollInfoChanged -= new EventHandler(GridScrollInfoChanged);
                gridControl.ThemeChanged -= new EventHandler(Theme_Changed);
                gridControl.VisibleChanged += new EventHandler(GridVisibleChanged);
                gridControl.Disposed -= new EventHandler(gridControl_Disposed);
                this.NavigationBar.RnbData = null;
                this.NavigationBar.ButtonBarMouseDown -= new MouseEventHandler(BarMouseDown);
                this.NavigationBar.MouseUp -= new MouseEventHandler(BarMouseUp);
            }
        }

        private void Theme_Changed(object sender, EventArgs e)
        {
            this.ThemesEnabled = _gridControl != null && _gridControl.ThemesEnabled;
        }

        private void GridVisibleChanged(object sender, EventArgs e)
        {
#if DEBUG
            if (Switches.Development.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(sender);
            }
#else
            ;
#endif
            GridControlBase gridControl = sender as GridControlBase;
            if (gridControl != null && gridControl.Visible)
            {
                GridDataBoundGrid dataGrid = sender as GridDataBoundGrid;
                if (dataGrid != null && dataGrid.Binder.List != null)
                {
                    this.NavigationBar.CurrentRecord = RowIndexToRecord(dataGrid, dataGrid.Binder.CurrentRowIndex);
                }
                else
                {
                    if (gridControl != null)
                    {
                        if (gridControl.CurrentCell.HasCurrentCell)
                        {
                            this.NavigationBar.CurrentRecord = RowIndexToRecord(gridControl, gridControl.CurrentCell.RowIndex);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Creates a new control for the specified splitter pane.
        /// </summary>       
        /// <param name="row">The row in a <see cref="SplitterControl"/>.</param>
        /// <param name="column">The column in a <see cref="SplitterControl"/>.</param>
        /// <param name="mainControl">The control in the first splitter pane.</param>
        /// <param name="parent">A parent control. Can be a <see cref="SplitterControl"/>.</param>
        /// <returns>A new instance of <see cref="GridControlBase"/>.</returns>
        public Control CreateNewControl(int row, int column, Control mainControl, Control parent)
        {
#if DEBUG
            if (Switches.Development.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(row, column, mainControl, parent);
            }
#else
            ;
#endif
            Control newControl = null;
            ICreateNewWindow createNewWindow = mainControl as ICreateNewWindow;
            if (createNewWindow != null)
            {
                newControl = createNewWindow.CreateNewControl(parent, row, column);
            }

            if (newControl is GridControlBase)
            {
                WireGrid((GridControlBase)newControl);
            }

            return newControl;
        }

        /// <summary>
        /// Hides or disposes the control for the specified splitter pane.
        /// </summary>      
        /// <param name="row">The row in a <see cref="SplitterControl"/>.</param>
        /// <param name="column">The column in a <see cref="SplitterControl"/>.</param>
        /// <param name="control">The control in the splitter pane that should be hidden.</param>
        /// <param name="parent">A parent control. Can be a <see cref="SplitterControl"/>.</param>
        public void DisposeControl(int row, int column, Control control, Control parent)
        {
#if DEBUG
            if (Switches.Development.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(row, column, control, parent);
            }
#else
            ;
#endif
            if (control is GridControlBase)
            {
                GridControlBase grid = (GridControlBase)control;
                UnwireGrid(grid);
            }
        }

        void GridScrollInfoChanged(object sender, EventArgs e)
        {
#if DEBUG
            if (Switches.Development.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(sender);
            }
#else
            ;
#endif
            GridDataBoundGrid dataGrid = sender as GridDataBoundGrid;
            if (dataGrid != null)
            {
                this.NavigationBar.RnbData = dataGrid as IRecordNavigationBarData;
                this.NavigationBar.MaxRecord = dataGrid.Binder.RecordCount;
                this.NavigationBar.MinRecord = 1;
                this.NavigationBar.AllowAddNew = dataGrid.Binder.AllowAddNew;
                this.NavigationBar.Enabled = NavigationBar.MaxRecord > 0 || this.NavigationBar.AllowAddNew;
            }
            else
            {
                GridControlBase gridControl = sender as GridControlBase;
                if (gridControl != null)
                {
                    this.NavigationBar.MaxRecord = this.RowIndexToRecord(gridControl, gridControl.Model.RowCount);
                    int rowIndex, colIndex;
                    if (gridControl.CurrentCell.GetCurrentCell(out rowIndex, out colIndex))
                    {
                        int record = RowIndexToRecord(gridControl, rowIndex);
                        if (record > 0)
                        {
                            this.NavigationBar.CurrentRecord = record;
                            ////this.savedRecord = rowIndex;
                        }
                    }
                }
            }
        }

        void GridCurrentCellActivated(object sender, EventArgs e)
        {
#if DEBUG
            if (Switches.Development.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(sender);
            }
#else
            ;
#endif
            this.NavigationBar.Enabled = NavigationBar.MaxRecord >= NavigationBar.MinRecord || NavigationBar.AllowAddNew;

            if (inUpdate)
            {
                return;
            }

            try
            {
                inUpdate = true;
                int rowIndex, colIndex;
                GridControlBase gridControl = this.ActivePane as GridControlBase;
                if (gridControl != null && gridControl.CurrentCell.GetCurrentCell(out rowIndex, out colIndex))
                {
                    int record = RowIndexToRecord(gridControl, rowIndex);
                    if (record > 0)
                    {
                        this.NavigationBar.CurrentRecord = record;
                        ////this.savedRecord = rowIndex;
                    }
                }
            }
            finally
            {
                inUpdate = false;
            }
        }

        void DataGridEnterRow(object sender, GridRowEventArgs e)
        {
#if DEBUG
            if (Switches.Development.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(sender, e);
            }
#else
            ;
#endif
            if (inUpdate)
            {
                return;
            }

            GridDataBoundGrid dataGrid = sender as GridDataBoundGrid;
            if (dataGrid == null)
            {
                return;
            }

            try
            {
                inUpdate = true;
                int record = RowIndexToRecord(dataGrid, e.RowIndex);
                if (record > 0)
                {
                    this.NavigationBar.SetCurrentRecord(record, true);
                    ////this.savedRecord = record;
                }
            }
            finally
            {
                inUpdate = false;
                this.NavigationBar.PerformLayout();
            }
        }

        /// <summary>
        /// Calculates the one-based record displayed at a specific row. 
        /// </summary>
        /// <param name="grid">A reference to the grid.</param>
        /// <param name="rowIndex">The row index.</param>
        /// <remarks>
        /// When the grid has more than one hierarchy level, the record
        /// in the root level will be returned.
        /// </remarks>
        /// <returns>
        /// The one-based record (as displayed in the record navigation bar) for the given row index.
        /// </returns>
        public int RowIndexToRecord(GridControlBase grid, int rowIndex)
        {
            GridDataBoundGrid dataGrid = grid as GridDataBoundGrid;
            if (dataGrid != null)
            {
                return dataGrid.Binder.RowIndexToListManagerPosition(rowIndex) + 1;
            }
            else
            {
                return rowIndex - grid.Model.Rows.HeaderCount;
            }
        }

        /// <summary>
        /// Calculates the first row in the grid for a one-based record index. 
        /// </summary>
        /// <param name="grid">A reference to the grid.</param>
        /// <param name="record">The one-based record. When the grid has more than one hierarchy level, only the record
        /// for the root level can be specified.</param>
        /// <returns>Row index.</returns>
        /// <remarks>
        /// The first row index for the specified record.
        /// </remarks>
        public int RecordToRowIndex(GridControlBase grid, int record)
        {
            GridDataBoundGrid dataGrid = grid as GridDataBoundGrid;
            if (dataGrid != null)
            {
                return dataGrid.Binder.ListManagerPositionToRowIndex(record - 1);
            }
            else
            {
                return record + grid.Model.Rows.HeaderCount;
            }
        }

        void GridCurrentCellMoving(object sender, GridCurrentCellMovingEventArgs e)
        {
#if DEBUG
            if (Switches.Development.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(sender, e);
            }
#else
            ;
#endif
            if (inUpdate && Control.MouseButtons != MouseButtons.None)
            {
                e.Options |= GridSetCurrentCellOptions.NoSetFocus;
                mouse = true;
            }
        }

        void BarMouseDown(object sender, MouseEventArgs e)
        {
#if DEBUG
            if (Switches.Development.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo();
            }
#endif
            mouse = false;
        }

        void BarMouseUp(object sender, MouseEventArgs e)
        {
#if DEBUG
            if (Switches.Development.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo();
            }
#endif
            if (mouse)
            {
                GridControlBase gridControl = this.ActivePane as GridControlBase;
                if ((gridControl.Model.Options.ActivateCurrentCellBehavior & GridCellActivateAction.SetCurrent) != 0)
                {
                    gridControl.CurrentCell.BeginEdit();
                }

                mouse = false;
            }
        }

        /// <override/>
        protected override void OnCurrentRecordChanging(CurrentRecordEventArgs e)
        {
#if DEBUG
            if (Switches.Development.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(e);
            }
#else
            ;
#endif
            base.OnCurrentRecordChanging(e);

            if (inUpdate)
            {
                return;
            }

            GridControlBase gridControl = this.ActivePane as GridControlBase;
            inUpdate = true;
            try
            {
                int newRowIndex = this.RecordToRowIndex(gridControl, e.Record);

                if (gridControl != null && CurrentRecord != e.Record)
                {
                    ////Trace.WriteLine("NavigationBarCurrentRecordChanged " + e.Record.ToString());

                    int rowIndex = gridControl.CurrentCell.RowIndex;
                    if (rowIndex <= 0 || Math.Abs(rowIndex - newRowIndex) > 10)
                    {
                        gridControl.CurrentCell.MoveTo(newRowIndex, Math.Max(gridControl.Model.Cols.HeaderCount + 1, gridControl.CurrentCell.ColIndex), GridSetCurrentCellOptions.ScrollInView);
                    }
                    else if (rowIndex < newRowIndex)
                    {
                        gridControl.CurrentCell.MoveDown(newRowIndex - rowIndex);
                    }
                    else if(rowIndex != newRowIndex)
                    {
                        gridControl.CurrentCell.MoveUp(rowIndex - newRowIndex);
                    }
                }
            }
            catch (Exception ex)
            {
                TraceUtil.TraceExceptionCatched(ex);
                if (!ExceptionManager.RaiseExceptionCatched(this, ex))
                {
                    throw;
                }

                e.Cancel = true;
            }
            finally
            {
                int r = this.RowIndexToRecord(gridControl, gridControl.CurrentCell.RowIndex);
                if (r > 0)
                {
                    e.Record = r;
                }

                inUpdate = false;
            }
        }

        /// <summary>
        /// Gets the <see cref="GridControlBase"/> that is displayed in the record navigation pane.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public GridControlBase GridControl
        {
            get
            {
                return _gridControl;
            }
        }

        private void gridControl_Disposed(object sender, EventArgs e)
        {
            this._gridControl = null;
        }

        #region Overrides
        /// <summary>
        /// Gets or sets a value indicating whether the control can accept data that the user drags onto it.
        /// </summary>
        [Description("Specifies whether the control can accept data that the user drags onto it"), Category("Record Navigation")]
        public override bool AllowDrop
        {
            get
            {
                return base.AllowDrop;
            }
            set
            {
                base.AllowDrop = value;
            }
        }

        /// <summary>
        /// Gets or sets the color of arrows in enabled buttons.
        /// </summary>
        [Description("Specifies the color of arrows in enabled buttons."), Category("Appearance")]
        public override Color EnabledArrowColor
        {
            get
            {
                return base.EnabledArrowColor;
            }
            set
            {
                base.EnabledArrowColor = value;
            }
        }

        /// <summary>
        /// Gets or sets the color of arrows in disabled buttons.
        /// </summary>
        [Description("Specifies the color of arrows in disabled buttons."), Category("Appearance")]
        public override Color DisabledArrowColor
        {
            get
            {
                return base.DisabledArrowColor;
            }
            set
            {
                base.DisabledArrowColor = value;
            }
        }

        /// <summary>
        /// Gets or sets the button look for the arrow buttons.
        /// </summary>
        [Description("Indicates if arrow buttons should be drawn flat or raised."), Category("Appearance")]
        public override ButtonLook ButtonLook
        {
            get
            {
                return base.ButtonLook;
            }
            set
            {
                base.ButtonLook = value;
            }
        }

        /// <summary>
        /// Gets or sets the Office like scrollbars.
        /// </summary>
        [Description("Specifies the Office like scrollbars"), Category("Appearance")]
        public override OfficeScrollBars GridOfficeScrollBars
        {
            get
            {
                return base.GridOfficeScrollBars;
            }
            set
            {
                base.GridOfficeScrollBars = value;
            }
        }

        /// <summary>
        /// Gets or sets the horizontal splitter position in percentages of the splitter control's width.
        /// </summary>
        [Description("Specifies the horizontal splitter position in percentages of the splitter control's width"), Category("Layout")]
        public override int HSplitPos
        {
            get
            {
                return base.HSplitPos;
            }
            set
            {
                base.HSplitPos = value;
            }
        }

        /// <summary>
        /// Gets or sets the label to be displayed before the record field textbox.
        /// </summary>
        [Description("Specifies the label to be displayed before the record field textbox."), Category("Appearance")]
        public override string Label
        {
            get
            {
                return base.Label;
            }
            set
            {
                base.Label = value;
            }
        }

        /// <summary>
        /// Gets or sets an optional maximum label (e.g. "of 1000").
        /// </summary>
        [DefaultValue("")]
        [Description("Specifies an optional maximum label (e.g. of 1000)."), Category("Record Navigation")]
        public override string MaxLabel
        {
            get
            {
                return base.MaxLabel;
            }
            set
            {
                base.MaxLabel = value;
            }
        }

        /// <summary>
        ///  Gets or sets the maximum record position.
        /// </summary>
        [Description("Specifies the maximum record position."), Category("Record Navigation")]
        public override int MaxRecord
        {
            get
            {
                return base.MaxRecord;
            }
            set
            {
                base.MaxRecord = value;
            }
        }

        /// <summary>
        ///  Gets or sets the minimum record position.
        /// </summary>
        [Description("Specifies the minimum record position."), Category("Record Navigation")]
        public override int MinRecord
        {
            get
            {
                return base.MinRecord;
            }
            set
            {
                base.MinRecord = value;
            }
        }

        /// <summary>
        /// Gets or sets the width of the navigation bar.
        /// </summary>
        [DefaultValue(216)]
        [Description("Specifies the width of the navigation bar."), Category("Layout")]
        public override int NavigationBarWidth
        {
            get
            {
                return base.NavigationBarWidth;
            }
            set
            {
                base.NavigationBarWidth = value;
            }
        }

        /// <summary>
        /// Gets / sets the shortest interval for firing scroll event.
        /// </summary>
        [DefaultValue(20),
        Description("Specifies the shortest interval for firing scroll event."), Category("Record Navigation")]
        public override int MinRepeatClickDelay
        {
            get
            {
                return base.MinRepeatClickDelay;
            }
            set
            {
                base.MinRepeatClickDelay = value;
            }
        }

        /// <summary>
        /// Gets or sets the arrow button that should be shown in an arrow bar.
        /// </summary>
        [DefaultValue(DisplayArrowButtons.All),
        Description("Gets or sets the arrow button that should be shown in an arrow bar."), Category("Layout")]
        public override DisplayArrowButtons NavigationButtons
        {
            get
            {
                return base.NavigationButtons;
            }
            set
            {
                base.NavigationButtons = value;
            }
        }

        /// <summary>
        /// Indicates whether adding new records is enabled.
        /// </summary>
        [Description("Specifies whether adding new records is enabled."), Category("Record Navigation")]
        public override bool AllowAddNew
        {
            get
            {
                return base.AllowAddNew;
            }
            set
            {
                base.AllowAddNew = value;
            }
        }

        /// <summary>
        /// Gets or sets the backcolor of the navigation bar.
        /// </summary>
        [Description("Specifies the backcolor of the navigation bar."), Category("Appearance")]
        public override Color NavigationBarBackColor
        {
            get
            {
                return base.NavigationBarBackColor;
            }
            set
            {
                base.NavigationBarBackColor = value;
            }
        }

        /// <summary>
        /// Toggles between standard and Office2007 scrollbars.
        /// </summary>
        [Description("Toggle between standard and Office2007 scrollbars."),
        DefaultValue(false), Category("Appearance")]
        public override bool Office2007ScrollBars
        {
            get
            {
                return base.Office2007ScrollBars;
            }
            set
            {
                base.Office2007ScrollBars = value;
            }
        }

        /// <summary>
        /// Gets or sets the style of Office2007 scroll bars
        /// </summary>
        [Browsable(true),
        Description("Office 2007 style scrollbars"),
        DefaultValue(Office2007ColorScheme.Blue), Category("Appearance")]
        public override Office2007ColorScheme Office2007ScrollBarsColorScheme
        {
            get
            {
                return base.Office2007ScrollBarsColorScheme;
            }
            set
            {
                base.Office2007ScrollBarsColorScheme = value;
            }
        }

        /// <summary>
        /// Gets or sets the style of Office2007 scroll bars.
        /// </summary>
        [Browsable(true),
        Description("Office 2007 style scrollbars."),
        DefaultValue(Office2010ColorScheme.Blue), Category("Appearance")]
        public override Office2010ColorScheme Office2010ScrollBarsColorScheme
        {
            get
            {
                return base.Office2010ScrollBarsColorScheme;
            }
            set
            {
                base.Office2010ScrollBarsColorScheme = value;
            }
        }

        /// <summary>
        /// Gets / sets the number of Milliseconds to wait before repeatedly firing scroll event.
        /// </summary>
        [DefaultValue(200),
        Description("Milliseconds to wait before repeatedly firing scroll event."), Category("Record Navigation")]
        public override int RepeatClickDelay
        {
            get
            {
                return base.RepeatClickDelay;
            }
            set
            {
                base.RepeatClickDelay = value;
            }
        }

        /// <summary>
        /// Toggles visibility of the Horizontal scrollbar.
        /// </summary>
        [Description("Toggles visibility of the Horizontal scrollbar.")]
        [DefaultValue(true), Category("Record Navigation")]
        public override bool ShowHorizontalScrollBar
        {
            get
            {
                return base.ShowHorizontalScrollBar;
            }
            set
            {
                base.ShowHorizontalScrollBar = value;
            }
        }

        /// <summary>
        /// Indicates whether ToolTips are being shown for tabs that have ToolTips set on
        /// them.
        /// </summary>
        [DefaultValue(false),
        Description("Indicates whether ToolTips are being shown for tabs that have ToolTips set on them."),
        Category("Record Navigation")]
        public override bool ShowToolTips
        {
            get
            {
                return base.ShowToolTips;
            }
            set
            {
                base.ShowToolTips = value;
            }
        }

        /// <summary>
        /// Toggles visibility of the vertical scrollbar.
        /// </summary>
        [Description("Toggles visibility of the vertical scrollbar.")]
        [DefaultValue(true), Category("Record Navigation")]
        public override bool ShowVerticalScrollBar
        {
            get
            {
                return base.ShowVerticalScrollBar;
            }
            set
            {
                base.ShowVerticalScrollBar = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating what split behavior is supported. Rows, Columns or Both.
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint),
        Description("Indicates the splitter behavior that should be supported."), Category("Layout")]
        public override DynamicSplitBars SplitBars
        {
            get
            {
                return base.SplitBars;
            }
            set
            {
                base.SplitBars = value;
            }
        }

        /// <summary>
        /// Gets or sets the vertical splitter position in percentages of the splitter control's height.
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [DefaultValue(100)]
        [Description("The horizontal splitter position in percentages of the height."), Category("Layout")]
        public override int VSplitPos
        {
            get
            {
                return base.VSplitPos;
            }
            set
            {
                base.VSplitPos = value;
            }
        }

        /// <summary>
        /// Gets / sets whether the control should be drawn using Windows XP Themes if available.
        /// </summary>
        [Description("Specifies whether the control should be drawn using Windows XP Themes if available"), Category("Appearance")]
        public override bool ThemesEnabled
        {
            get
            {
                return base.ThemesEnabled;
            }
            set
            {
                base.ThemesEnabled = value;
            }
        }

        #endregion
    }
}
