//-------------------------------------------------------------------------------------------------
// <copyright file="GridDataBoundGrid.cs" company="syncfusion">
//  Copyright (c) Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
// </copyright>
//-------------------------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using System.Drawing.Design;
using System.Runtime.Serialization;
using System.Security;
using System.Security.Permissions;

using Syncfusion.ComponentModel;
using Syncfusion.Drawing;
using Syncfusion.Diagnostics;
using Syncfusion.Styles;
using System.Collections.Generic;
using Syncfusion.Windows.Forms;
#if SyncfusionFramework4_0
using System.Windows.Automation.Provider;
#elif SyncfusionFramework3_5
using System.Windows.Automation.Provider;
#endif

namespace Syncfusion.Windows.Forms.Grid
{
    /// <summary>
    /// Defines if databound grid should sort a column if user clicks on it.
    /// </summary>
    public enum GridSortBehavior
    {
        /// <summary>
        /// No sorting when user clicks once.
        /// </summary>
        None,

        /// <summary>
        /// Sort column when user clicks once.
        /// </summary>
        SingleClick,

        /// <summary>
        /// Sort column when user double-clicks.
        /// </summary>
        DoubleClick
    }

    /// <summary>
    /// Provides event data for the <see cref="GridDataBoundGrid.RowEnter"/>, <see cref="GridDataBoundGrid.RowEditing"/>,
    /// <see cref="GridDataBoundGrid.RowLeave"/> and <see cref="GridDataBoundGrid.RowSaved"/> event.
    /// </summary>
    public class GridRowEventArgs : SyncfusionCancelEventArgs
    {
        int rowIndex;
        bool isAddNew;

        /// <summary>
        /// Initializes a new <see cref="GridRowEventArgs"/> object.
        /// </summary>
        /// <param name="rowIndex">The row index.</param>
        public GridRowEventArgs(int rowIndex)
        {
            this.rowIndex = rowIndex;
        }

        /// <summary>
        /// Initializes a new <see cref="GridRowEventArgs"/> object.
        /// </summary>
        /// <param name="rowIndex">The row index.</param>
        /// <param name="isAddNew">Indicates if AddNew was called for <see cref="GridDataBoundGrid.RowEditing"/> and <see cref="GridDataBoundGrid.RowSaved"/> events.</param>
        public GridRowEventArgs(int rowIndex, bool isAddNew)
        {
            this.rowIndex = rowIndex;
            this.isAddNew = isAddNew;
        }

        /// <summary>
        /// Gets or sets the row index.
        /// </summary>
        [TraceProperty(true)]
        public int RowIndex
        {
            get
            {
                return rowIndex;
            }

            set
            {
                rowIndex = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether AddNew was called for <see cref="GridDataBoundGrid.RowEditing"/> and <see cref="GridDataBoundGrid.RowSaved"/> events.
        /// </summary>
        [TraceProperty(true)]
        public bool IsAddNew
        {
            get
            {
                return isAddNew;
            }

            set
            {
                isAddNew = value;
            }
        }
    }

    /// <summary>
    /// A method that represents a handler for the <see cref="GridDataBoundGrid.RowEnter"/>,
    /// <see cref="GridDataBoundGrid.RowLeave"/>, or <see cref="GridDataBoundGrid.RowSaved"/> event.
    /// </summary>
    public delegate void GridRowEventHandler(object sender, GridRowEventArgs e);

    /// <summary>
    /// Provides event data for the <see cref="GridDataBoundGrid.RowsDeleting"/> event.
    /// </summary>
    public class GridRowRangeEventArgs : SyncfusionCancelEventArgs
    {
        int from;
        int to;

        /// <summary>
        /// Initializes a new <see cref="GridRowEventArgs"/> object.
        /// </summary>
        /// <param name="from">The first index.</param>
        /// <param name="last">The last index.</param>
        public GridRowRangeEventArgs(int from, int last)
        {
            this.from = from;
            this.to = last;
        }

        /// <summary>
        /// Gets the first index.
        /// </summary>
        [TraceProperty(true)]
        public int From
        {
            get
            {
                return from;
            }
        }

        /// <summary>
        /// Gets the row index.
        /// </summary>
        [TraceProperty(true)]
        public int To
        {
            get
            {
                return to;
            }
        }
    }

    /// <summary>
    /// A method that represents a handler for the <see cref="GridDataBoundGrid.RowsDeleting"/> event.
    /// </summary>
    public delegate void GridRowRangeEventHandler(object sender, GridRowRangeEventArgs e);

    /// <summary>
    /// Provides event data for the <see cref="GridDataBoundGrid.ValidateFailed"/> event.
    /// </summary>
    public class GridValidateFailedEventArgs : SyncfusionHandledEventArgs
    {
        /// <summary>
        /// Initializes a new <see cref="GridValidateFailedEventArgs"/> object.
        /// </summary>
        public GridValidateFailedEventArgs()
        {
        }
    }

    /// <summary>
    /// A method that represents a handler for the <see cref="GridDataBoundGrid.ValidateFailed"/> event.
    /// </summary>
    public delegate void GridValidateFailedEventHandler(object sender, GridValidateFailedEventArgs e);

    /// <summary>
    /// Provides support for displaying ADO.NET data and other data sources in a grid. Data will be
    /// loaded from the given data source and changes will be written back to the data source.
    /// </summary>
    /// <remarks>
    /// To display a table in the <see cref="GridDataBoundGrid"/> at run-time,
    /// set the <see cref="DataSource"/> and <see cref="DataMember"/> properties to a
    /// valid data source. The following data sources are valid:
    /// DataTable
    /// DataView
    /// DataSet
    /// A single dimension array
    /// Any component that implements the IListSource interface
    /// Any component that implements the IList interface.
    /// </remarks>
    [ToolboxItem(true)]
    [System.Drawing.ToolboxBitmap(typeof(Syncfusion.Windows.Forms.Grid.GridDataBoundGrid), "ToolboxIcons.GridDataBoundGrid.bmp")]
    [Designer(typeof(GridDataBoundGridControlDesigner))]
#if SyncfusionFramework2_0
    [ComplexBindingProperties("DataSource", "DataMember"),
    Description("Displays ADO.NET data and other data sources in a grid."),
    DefaultEvent("CellClick"),
    Docking(DockingBehavior.Ask)]
#endif
    public class GridDataBoundGrid : GridControlBaseImp, ISupportInitialize, IVisualStyle
    {
        internal GridModelDataBinder binder;
        bool firstShown = true;
        bool isDesign = false;
        bool isWired = true;
        private ColorStyles colorStyles = ColorStyles.SystemTheme;
        private bool isMetroSettingsApplied = false;
        /// <override/>
        protected override void OnPaint(PaintEventArgs pe)
        {
            base.OnPaint(pe);
            if (isDesign && DataSource == null)
            {
                SetControlDisplayInfo(pe.Graphics);
            }
        }

        private void SetControlDisplayInfo(Graphics g)
        {
            Rectangle rect = this.ClientRectangle;
            StringFormat sf = new StringFormat();
            sf.Alignment = StringAlignment.Center;
            sf.LineAlignment = StringAlignment.Center;
            g.DrawString("Set the DataSource property to preview...", this.Font, new SolidBrush(SystemColors.ControlDark), rect, sf);
        }
        /// <summary>
        /// Gets or sets the <see cref="GridBorderStyle"/> value to be used as default for cell borders.
        /// </summary>
        [Browsable(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description("Specifies the border style to be used as default for cell borders.")]
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Grid")]
        [DefaultValue(GridBorderStyle.Dotted)]
        public GridBorderStyle DefaultGridBorderStyle
        {
            get
            {
                return Model.Options.DefaultGridBorderStyle;
            }

            set
            {
                Model.Options.DefaultGridBorderStyle = value;
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether the grid should be display column headers.
        /// </summary>
        [Description("Specifies if column headers should be displayed or hidden.")]
        [Browsable(true), DefaultValue(true)]
        [RefreshProperties(RefreshProperties.Repaint)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Category("Grid")]
        public bool ShowColumnHeaders
        {
            get
            {
                return Properties.ColHeaders;
            }

            set
            {
                Properties.ColHeaders = value;
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether horizontal lines should be displayed.
        /// </summary>
        [Description("Specifies if horizontal lines should be displayed.")]
        [Browsable(true), DefaultValue(true)]
        [RefreshProperties(RefreshProperties.Repaint)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Category("Grid")]
        public bool DisplayHorizontalLines
        {
            get
            {
                return Properties.DisplayHorzLines;
            }

            set
            {
                Properties.DisplayHorzLines = value;
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether vertical lines should be displayed.
        /// </summary>
        [Description("Specifies if vertical lines should be displayed.")]
        [Browsable(true), DefaultValue(true)]
        [RefreshProperties(RefreshProperties.Repaint)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Category("Grid")]
        public bool DisplayVerticalLines
        {
            get
            {
                return Properties.DisplayVertLines;
            }

            set
            {
                Properties.DisplayVertLines = value;
            }
        }
        /// <summary>
        /// Enable or Disable the Legacy styles in the Table Model
        /// Value should be false to apply ColorStyles
        /// </summary>
        [Description("Allow Legacy Styles to Enable or Disable")]
        [Browsable(true), DefaultValue(true)]
        [Category("Grid")]
        public bool ApplyVisualStyles
        {
            get
            {
                return Model.EnableLegacyStyle;
            }
            set
            {
                if (Model.EnableLegacyStyle != value)
                {
                    Model.EnableLegacyStyle = value;
                }
            }
        }
        /// <summary>
        /// To specify the browse only state of the Grid
        /// </summary>
        [Description("To specify the browseonly state of the Grid.")]
        [Browsable(true), DefaultValue(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Category("Grid")]
        public bool BrowseOnly
        {
            get
            {
                return Model.BrowseOnly;
            }
            set
            {
                Model.BrowseOnly = value;
            }
        }
        /// <summary>
        /// Gets or sets the color of grid lines.
        /// </summary>
        [Description("The color of grid lines.")]
        [RefreshProperties(RefreshProperties.Repaint)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Category("Grid")]
        public Color GridLineColor
        {
            get
            {
                return Properties.GridLineColor;
            }

            set
            {
                Properties.GridLineColor = value;
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether column headers should be printed when printing the grid.
        /// </summary>
        [Description("Specifies if column headers should be printed when printing the grid.")]
        [Browsable(true), DefaultValue(true), Category("Grid")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool PrintColumnHeader
        {
            get
            {
                return Properties.PrintColHeader;
            }

            set
            {
                Properties.PrintColHeader = value;
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether the grid should draw horizontal lines when printing.
        /// </summary>
        [Description("Specifies if the grid should draw horizontal lines when printing.")]
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool PrintHorizontalLines
        {
            get
            {
                return Properties.PrintHorzLines;
            }

            set
            {
                Properties.PrintHorzLines = value;
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether row headers should be printed when printing the grid.
        /// </summary>
        [Description("Specifies if row headers should be printed when printing the grid.")]
        [Browsable(true), DefaultValue(true), Category("Grid")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool PrintRowHeader
        {
            get
            {
                return Properties.PrintRowHeader;
            }

            set
            {
                Properties.PrintRowHeader = value;
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether the grid should draw vertical lines when printing.
        /// </summary>
        [Description("Specifies if the grid should draw vertical lines when printing.")]
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool PrintVerticalLines
        {
            get
            {
                return Properties.PrintVertLines;
            }

            set
            {
                Properties.PrintVertLines = value;
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether row headers should be displayed or hidden. (Might be better to use HideCols[0] = false) instead.
        /// </summary>
        [Description("Specifies if row headers should be displayed or hidden.")]
        [Browsable(true), DefaultValue(true), Category("Grid")]
        [RefreshProperties(RefreshProperties.Repaint)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool ShowRowHeaders
        {
            get
            {
                return Properties.RowHeaders;
            }

            set
            {
                Properties.RowHeaders = value;
            }
        }
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

        /// <summary>
        /// Occurs before the record at the specified row is being expanded to show details
        /// of a child relation.
        /// Check <see cref="GridModelDataBinder.GetRecordStateAtRowIndex"/>
        /// of the <see cref="Binder"/> object for information about the affected record.
        /// </summary>
        [Description("Occurs before the record at the specified row is being expanded to show details of a child relation."), Category("Behavior")]
        public event GridRowEventHandler RowExpanding;

        /// <summary>
        /// Occurs after the record at the specified row has been expanded to show details
        /// of a child relation.
        /// Check <see cref="GridModelDataBinder.GetRecordStateAtRowIndex"/>
        /// of the <see cref="Binder"/> object for information about the affected record.
        /// </summary>
        [Description("Occurs after the record at the specified row has been expanded to show details of a child relation."), Category("Behavior")]
        public event GridRowEventHandler RowExpanded;

        /// <summary>
        /// Occurs before the record at the specified row is expanded to show details
        /// of a child relation.
        /// Check <see cref="GridModelDataBinder.GetRecordStateAtRowIndex"/>
        /// of the <see cref="Binder"/> object for information about the affected record.
        /// </summary>
        [Description("Occurs before the record at the specified row is expanded to show details of a child relation."), Category("Behavior")]
        public event GridRowEventHandler RowCollapsing;

        /// <summary>
        /// Occurs after the record at the specified row has been expanded to show details
        /// of a child relation.
        /// Check <see cref="GridModelDataBinder.GetRecordStateAtRowIndex"/>
        /// of the <see cref="Binder"/> object for information about the affected record.
        /// </summary>
        [Description("Occurs after the record at the specified row has been expanded to show details of a child relation."), Category("Behavior")]
        public event GridRowEventHandler RowCollapsed;

        /// <summary>
        /// Occurs before the grid deletes a number of records from the datasource.
        /// </summary>
        [Description("Occurs before the grid deletes a number of records from the datasource."), Category("Behavior")]
        public event GridRowRangeEventHandler RowsDeleting;

        /// <summary>
        /// Occurs after the grid deleted a number of records from the datasource.
        /// </summary>
        [Description("Occurs after the grid deleted a number of records from the datasource."), Category("Behavior")]
        public event GridRowRangeEventHandler RowsDeleted;

        /// <summary>
        /// Occurs before the current cell is activated in a new row.
        /// Check <see cref="GridModelDataBinder.GetRecordStateAtRowIndex"/>
        /// of the <see cref="Binder"/> object for information about the affected record.
        /// </summary>
        [Description("Occurs before the current cell is activated in a new row."), Category("Behavior")]
        public event GridRowEventHandler RowEnter;

        /// <summary>
        /// Occurs after the current cell's changes have been saved and before it is deactivated and the changes in the current row are saved to the underlying data table.
        /// Check <see cref="GridModelDataBinder.GetRecordStateAtRowIndex"/>
        /// of the <see cref="Binder"/> object for information about the affected record.
        /// </summary>
        [Description("Occurs after the current cell's changes have been saved and before it is deactivated and the changes in the current row are saved to the underlying data table. "), Category("Behavior")]
        public event GridRowEventHandler RowLeave;

        /// <summary>
        /// Occurs after the changes in the current row have been saved.
        /// Check <see cref="GridModelDataBinder.GetRecordStateAtRowIndex"/>
        /// of the <see cref="Binder"/> object for information about the affected record.
        /// </summary>
        [Description("Occurs after the changes in the current row have been saved."), Category("Behavior")]
        public event GridRowEventHandler RowSaved;

        /// <summary>
        /// Occurs before the current row is edited.
        /// Check <see cref="GridModelDataBinder.GetRecordStateAtRowIndex"/>
        /// of the <see cref="Binder"/> object for information about the affected record.
        /// </summary>
        [Description("Occurs before the current row is edited."), Category("Behavior")]
        public event GridRowEventHandler RowEditing;

        /// <overload>
        /// Initializes a new instance of <see cref="GridDataBoundGrid"/>.
        /// </overload>
        /// <summary>
        /// Initializes a new instance of <see cref="GridDataBoundGrid"/>.
        /// </summary>
        public GridDataBoundGrid()
            : this(new GridDataBoundGridModel())
        {
        }

        #region For Touch

        bool _touchMode = false;

        /// <summary>
        /// gets or sets the touchmode
        /// </summary>
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        public virtual bool EnableTouchMode
        {
            get
            {
                return _touchMode;
            }
            set
            {
                if (_touchMode != value)
                {
                    _touchMode = value;
                    if (_touchMode)
                    {
                        ApplyScaleToControl(1.5F);
                    }
                    else
                    {
                        ApplyScaleToControl(1);
                    }

                }
            }
        }
        private bool ShouldSerializeTouchMode()
        {
            return EnableTouchMode != false;
        }

        /// <summary></summary>
        private void ResetTouchMode()
        {
            EnableTouchMode = false;
        }
        /// <summary>
        /// applies the scaling
        /// </summary>
        /// <param name="scaleFactor"></param>
        public void ApplyScaleToControl(float sf)
        {
            this.BeginUpdate();
            this.SuspendLayout();
            if (sf == 1.5)
            {
                for (int i = 0; i < this.Model.RowCount; i++)
                {
                    if (this.Model.RowHeights[i] != 0)
                        this.Model.RowHeights[i] += 5;
                }
                for (int i = 0; i < this.Model.ColCount; i++)
                {
                    if (this.Model.ColWidths[i] != 0)
                        this.Model.ColWidths[i] += 15;
                }
            }
            else
            {
                for (int i = 0; i < this.Model.RowCount; i++)
                {
                    if (this.Model.RowHeights[i] != 0)
                        this.Model.RowHeights[i] -= 5;
                }
                for (int i = 0; i < this.Model.ColCount; i++)
                {
                    if (this.Model.ColWidths[i] != 0)
                        this.Model.ColWidths[i] -= 15;
                }
            }
            this.ResumeLayout();
            this.Invalidate();
            this.EndUpdate();
        }
        protected override void OnFontChanged(EventArgs e)
        {
            if (binder == null && isWired)
                EnsureBinder();
            base.OnFontChanged(e);
        }
        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
        }
        #endregion

        /// <summary>
        /// Initializes a new instance of <see cref="GridDataBoundGrid"/> and attaches it
        /// to a <see cref="GridDataBoundGridModel"/>.
        /// </summary>
        /// <param name="model">Grid model.</param>
        public GridDataBoundGrid(GridDataBoundGridModel model)
            : base(model)
        {
            try
            {
                AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(AssemblyInfo.AssemblyResolver);
                new Syncfusion.Core.Licensing.LicensedComponent(typeof(GridDataBoundGrid));
            }
            finally
            {
                AppDomain.CurrentDomain.AssemblyResolve -= new ResolveEventHandler(AssemblyInfo.AssemblyResolver);
            }
            sizedColumns = new Dictionary<string, int>();
            if (model.ActiveGridView == null)
            {
                model.ActiveGridView = this;
            }

            ////this.SetStyle(Syncfusion.Windows.Forms.WhidbeyCompatibleControlStyles.DoubleBuffer, false);
        }

        /// <summary>
        /// Occurs when the FieldChooser dialog is being displayed. 
        /// </summary>
        [Description("Occurs before the FieldChooser dialog is displayed"), Category("Field Chooser")]
        public event FieldChooserShowingEventHandler FieldChooserShowing;
        /// <summary>
        /// Occurs immediately after the FieldChooser dialog is displayed. 
        /// </summary>
        [Description("Occurs after the FieldChooser dialog is displayed"), Category("Field Chooser")]
        public event FieldChooserShownEventHandler FieldChooserShown;
        /// <summary>
        /// Occurs immediately before the FieldChooser dialog is Closed. 
        /// </summary>
        [Description("Occurs when the FieldChooser dialog is being closed"), Category("Field Chooser")]
        public event FieldChooserClosingEventHandler FieldChooserClosing;
        /// <summary>
        /// Occurs immediately after the FieldChooser dialog is Closed. 
        /// </summary>
        [Description("Occurs after the FieldChooser dialog is closed"), Category("Field Chooser")]
        public event FieldChooserClosedEventHandler FieldChooserClosed;
        
        /// <override/>
        /// <summary>
        /// Creates a new databound grid.
        /// </summary>
        /// <param name="parent">A parent control.</param>
        /// <param name="row">Row index.</param>
        /// <param name="column">Column index.</param>
        /// <returns>The new grid.</returns>
        public override Control CreateNewControl(Control parent, int row, int column)
        {
            GridDataBoundGrid grid1 = new GridDataBoundGrid(DataBoundGridModel);
            WireNewControl(grid1);
            return grid1;
        }

        /// <summary>
        /// Helper method to CreateNewControl to initialize new grid based on current grid settings.
        /// </summary>
        /// <param name="grid">Grid created with CreateNewControl.</param>
        /// <remarks>You should call this method from the overridden CreateNewControl method in your derived
        /// GridDataBoundGrid. Overriding CreateNewControl is required if you want your derived grid to be
        /// contained in a TabBarSplitterControl or a GridRecordNavigationControl.
        /// </remarks>
        /// <example>
        /// <code lang="C#">
        /// public override Control CreateNewControl(Control parent, int row, int column)
        /// {
        /// MyGridDataBoundGrid grid = new MyGridDataBoundGrid((GridDataBoundGridModel) this.Model);
        /// this.WireNewControl(grid);
        /// return grid;
        /// }
        /// </code>
        /// </example>
        protected void WireNewControl(GridDataBoundGrid grid)
        {
            grid.binder = binder;
            grid.WireBinder();
            grid.BackColor = BackColor;
            grid.ForeColor = ForeColor;
            grid.BorderStyle = BorderStyle;
            grid.ShowTreeLines = ShowTreeLines; ////wires treeline support
        }

        /// <summary>
        /// Gets a reference to the <see cref="GridModelDataBinder"/> that manages the underlying
        /// data source.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public GridModelDataBinder Binder
        {
            get
            {
                return binder;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the grid should listen to IBindindList.ListChanged events or
        /// if it should only handle currency manager events.
        /// </summary>
        /// <remarks>
        /// This property is set to false by default to ensure backward compatibility with earlier versions. But
        /// if you drop a GridDataBoundGrid onto a form in the designer, the property will be set True as default (using
        /// UseListChangedEvent gives better performance that are listening to currency manager events.)
        /// </remarks>
        [DefaultValue(false)]
        [Category("Grid")]
        [Description("Gets or sets a value indicating whether the grid should listen to IBindindList.ListChanged events or if it should only handle currency manager events.")]
        public bool UseListChangedEvent
        {
            get
            {
                EnsureBinder();
                return binder.OptimizeListChangedEvent;
            }

            set
            {
                EnsureBinder();
                binder.OptimizeListChangedEvent = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the grid should call Control.Update after each
        /// IBindindList.ListChanged event. This property only has any effect
        /// if <see cref="UseListChangedEvent"/> has been set to True.
        /// </summary>
        /// <remarks>
        /// Set this property to False if you want to manually call Update. Manually
        /// calling Update can have better performance because you can then batch
        /// several datatable modifications into one operation and then only call update
        /// when changes are done.
        /// </remarks>
        [DefaultValue(true)]
        [Category("Grid")]
        [Description("Gets or sets a value indicating whether the grid should call Control.Update after each IBindindList.ListChanged event. This property only has any effect if UseListChangedEvent has been set to True.")]
        public bool ForceUpdateAfterListChangedEvent
        {
            get
            {
                EnsureBinder();
                return binder.ForceUpdateAfterListChangedEvent;
            }

            set
            {
                EnsureBinder();
                binder.ForceUpdateAfterListChangedEvent = value;
            }
        }

        #region TreeCell support

        private const int TREECOLUMN = 2;
        private bool treeLines = false;

        /// <summary>
        /// Gets or sets a value indicating whether to enable or disable tree-like expand / collapse for row header cells in a hierarchical grid.
        /// </summary>
        [DefaultValue(false)]
        [Description("Enables or disables tree-like expand / collapse for row header cells in a hierarchical grid.")]
        [Category("Grid")]
        public bool ShowTreeLines
        {
            get
            {
                return treeLines;
            }

            set
            {
                if (treeLines != value)
                {
                    this.BeginUpdate();
                    treeLines = value;
                    if (treeLines)
                    {
                        if (!this.Model.CellModels.ContainsKey("DataBoundTreeCell"))
                        {
                            this.Model.CellModels.Add("DataBoundTreeCell", new GridDataBoundTreeCellModel(this.Model));
                        }

                        WireTreeLineEvents();
                        //// this.Model.Cols.Hidden[TREECOLUMN - 1] = true; //dt 32946
                    }
                    else
                    {
                        ////this.Model.Cols.Hidden[TREECOLUMN - 1] = false; ////dt 32946
                        UnwireTreeLineEvents();
                    }

                    this.EndUpdate();
                    this.RefreshRange(GridRangeInfo.Cols(TREECOLUMN - 1, TREECOLUMN));
                }
            }
        }

        private bool indentLevels = false;

        /// <summary>
        /// Gets or sets a value indicating whether to show hierarchical levels indented without treelines.
        /// </summary>
        [DefaultValue(false)]
        [Description("Show hierarchical levels indented without treelines.")]
        [Category("Grid")]
        public bool IndentHierarchies
        {
            get
            {
                return indentLevels;
            }

            set
            {
                if (indentLevels != value)
                {
                    this.BeginUpdate();
                    indentLevels = value;
                    if (indentLevels && !this.ShowTreeLines)
                    {
                        this.ShowTreeLines = true;
                    }
                    else if (!indentLevels && this.ShowTreeLines)
                    {
                        this.ShowTreeLines = false;
                    }

                    this.EndUpdate();
                    this.RefreshRange(GridRangeInfo.Cols(TREECOLUMN - 1, TREECOLUMN));
                }
            }
        }

        private void WireTreeLineEvents()
        {
            this.CellButtonClicked += new GridCellButtonClickedEventHandler(treeCellButton_Clicked);
            this.Model.QueryCellInfo += new GridQueryCellInfoEventHandler(gridModel_QueryCellInfo);
            this.SplitterPaneClosing += new EventHandler(grid_SplitterPaneClosing);
            this.Model.QueryColWidth += new GridRowColSizeEventHandler(Model_QueryColWidth);
        }
        
        private void UnwireTreeLineEvents()
        {
            this.Model.QueryCellInfo -= new GridQueryCellInfoEventHandler(gridModel_QueryCellInfo);
            this.CellButtonClicked -= new GridCellButtonClickedEventHandler(treeCellButton_Clicked);
            this.SplitterPaneClosing -= new EventHandler(grid_SplitterPaneClosing);
            this.Model.QueryColWidth -= new GridRowColSizeEventHandler(Model_QueryColWidth);
        }

        private void grid_SplitterPaneClosing(object sender, EventArgs e)
        {
            UnwireTreeLineEvents();
        }

        ////added to avoid using Hidden columns that caused a problem with 4.4 and higher
        ////see  dt 32946
        void Model_QueryColWidth(object sender, GridRowColSizeEventArgs e)
        {
            if (e.Index == TREECOLUMN - 1)
            {
                e.Size = 0;
                e.Handled = true;
            }
        }

        private void treeCellButton_Clicked(object sender, GridCellButtonClickedEventArgs e)
        {
            if (e.ColIndex == TREECOLUMN && this.Binder.HierarchyLevelCount > 0)
            {
                if (this.IsExpandedAtRowIndex(e.RowIndex))
                {
                    this.CollapseAtRowIndex(e.RowIndex);
                }
                else
                {
                    this.ExpandAtRowIndex(e.RowIndex);
                }
            }
        }

        private void gridModel_QueryCellInfo(object sender, GridQueryCellInfoEventArgs e)
        {
            if (this.Binder.HierarchyLevelCount > 0
                && e.RowIndex > this.Model.Rows.HeaderCount && e.ColIndex == TREECOLUMN
                && e.RowIndex <= this.Model.RowCount - (this.Binder.AllowAddNew ? 1 : 0))
            {
                e.Style.Borders.Bottom = new GridBorder(GridBorderStyle.None);
                e.Style.CellType = "DataBoundTreeCell";
            }
        }

        #endregion

        /// <summary>
        /// Gets the <see cref="GridDataBoundGridModel"/> that manages data to be displayed in the grid.
        /// </summary>
        /// <remarks>
        /// You can replace the <see cref="GridDataBoundGridModel"/> at run-time. The <see cref="GridControlBase"/>
        /// will release and establish links to the previous model and establish new relationships
        /// with the new model and then redraw itself.
        /// </remarks>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public GridDataBoundGridModel DataBoundGridModel
        {
            get
            {
                return (GridDataBoundGridModel)Model;
            }
        }

        #region ISupportInitialize
        bool inInit;

        /// <summary>
        /// Implements <see cref="ISupportInitialize.BeginInit"/> of the <see cref="ISupportInitialize"/> interface.
        /// </summary>
        public void BeginInit()
        {
#if DEBUG
            if (Switches.BeginEndUpdate.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(this.PaneDesc);
            }
#else
            ;
#endif
             if (inInit)
            {
                throw new System.Exception("BeginInit called twice.");
            }

            inInit = true;
        }

        /// <summary>
        /// Implements <see cref="ISupportInitialize.EndInit"/> of the <see cref="ISupportInitialize"/> interface.
        /// </summary>
        public void EndInit()
        {
#if DEBUG
            if (Switches.BeginEndUpdate.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(this.PaneDesc);
            }
#else
            ;
#endif
            inInit = false;

            EnsureBinder();

            this.InitSplitterControl();

            forceDataSource = true;
            if (binder.DataSource == null && this._dataSource != null)
            {
                if (GridListUtil.GetItemProperties(_dataSource).Count > 0)
                {
                    this.DataSource = _dataSource;
                    this._dataSource = null;
                }
            }

            if (this.DesignMode && (this.allowResizeToFit && binder.List != null))
            {
                Model.ColWidths.ResizeToFit(GridRangeInfo.Rows(1, ViewLayout.LastVisibleRow), GridResizeToFitOptions.IncludeHeaders);
            }

            CurrentCell.MoveTo(Model.Rows.HeaderCount + 1, Model.Cols.HeaderCount + 1);
        }

        bool allowResizeToFit = true;

        /// <summary>
        /// Gets or sets a value indicating whether the grid should automatically resize columns to fit cell contents
        /// on first display of the data.
        /// </summary>
        [DefaultValue(true)]
        [Category("Grid")]
        [Description("Gets or sets a value indicating whether the grid should automatically resize columns to fit cell contents on first display of the data.")]
        public bool AllowResizeToFit
        {
            get
            {
                return allowResizeToFit;
            }

            set
            {
                allowResizeToFit = value;
            }
        }
        
        internal bool isFilterBarWired = false;
        /// <summary>
        /// Determine is the filterbar is wired to grid.
        /// </summary>
        [DefaultValue(false)]
        [Category("Grid")]
        [Description("Gets or sets a value indicating whether the grid is wired with the filter bar")]
        [Browsable(false),DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsFilterBarWired
        {
            get
            {
                return isFilterBarWired;
            }
        }
        
        /// <summary>
        /// Gets a value indicating whether <see cref="GridModel.BeginInit"/> was called.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool Initializing
        {
            get
            {
                return this.inInit;
            }
        }
        #endregion

        /// <summary>
        /// Raises the <see cref="RowEnter"/> event.
        /// </summary>
        /// <param name="e">A <see cref="CancelEventArgs" /> that contains the event data.</param>
        protected virtual void OnRowEnter(GridRowEventArgs e)
        {
#if DEBUG
            if (Switches.GridDataBoundGridEvents.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(PaneDesc, e);
            }
#else
            ;
#endif
            if (RowEnter != null)
            {
                RowEnter(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="RowLeave"/> event.
        /// </summary>
        /// <param name="e">A <see cref="CancelEventArgs" /> that contains the event data.</param>
        protected virtual void OnRowLeave(GridRowEventArgs e)
        {
#if DEBUG
            if (Switches.GridDataBoundGridEvents.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(PaneDesc, e);
            }
#else
            ;
#endif
            if (RowLeave != null)
            {
                RowLeave(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="RowSaved"/> event.
        /// </summary>
        /// <param name="e">A <see cref="EventArgs" /> that contains the event data.</param>
        protected virtual void OnRowSaved(GridRowEventArgs e)
        {
#if DEBUG
            if (Switches.GridDataBoundGridEvents.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(PaneDesc, e);
            }
#else
            ;
#endif
            if (RowSaved != null)
            {
                RowSaved(this, e);
            }
        }
        
        /// <summary>
        /// Raises the <see cref="RowEditing"/> event.
        /// </summary>
        /// <param name="e">A <see cref="CancelEventArgs" /> that contains the event data.</param>
        protected virtual void OnRowEditing(GridRowEventArgs e)
        {
#if DEBUG
            if (Switches.GridDataBoundGridEvents.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(PaneDesc, e);
            }
#else
            ;
#endif
            if (RowEditing != null)
            {
                RowEditing(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="RowExpanding"/> event.
        /// </summary>
        /// <param name="e">A <see cref="GridRowEventArgs" /> that contains the event data.</param>
        protected virtual void OnRowExpanding(GridRowEventArgs e)
        {
            if (RowExpanding != null)
            {
                RowExpanding(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="RowExpanded"/> event.
        /// </summary>
        /// <param name="e">A <see cref="GridRowEventArgs" /> that contains the event data.</param>
        protected virtual void OnRowExpanded(GridRowEventArgs e)
        {
            if (RowExpanded != null)
            {
                RowExpanded(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="RowCollapsing"/> event.
        /// </summary>
        /// <param name="e">A <see cref="GridRowEventArgs" /> that contains the event data.</param>
        protected virtual void OnRowCollapsing(GridRowEventArgs e)
        {
            if (RowCollapsing != null)
            {
                RowCollapsing(this, e);
            }
        }
        
        /// <summary>
        /// Raises the <see cref="RowCollapsed"/> event.
        /// </summary>
        /// <param name="e">A <see cref="GridRowEventArgs" /> that contains the event data.</param>
        protected virtual void OnRowCollapsed(GridRowEventArgs e)
        {
            if (RowCollapsed != null)
            {
                RowCollapsed(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="RowsDeleting"/> event.
        /// </summary>
        /// <param name="e">A <see cref="GridRowRangeEventArgs" /> that contains the event data.</param>
        protected virtual void OnRowsDeleting(GridRowRangeEventArgs e)
        {
            if (RowsDeleting != null)
            {
                RowsDeleting(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="RowsDeleted"/> event.
        /// </summary>
        /// <param name="e">A <see cref="GridRowRangeEventArgs" /> that contains the event data.</param>
        protected virtual void OnRowsDeleted(GridRowRangeEventArgs e)
        {
            if (RowsDeleted != null)
            {
                RowsDeleted(this, e);
            }
        }
        
        /// <summary>
        /// Raises the FieldChooserShowing Event.
        /// </summary>
        /// <param name="e"> The event data. </param>
        public  void OnFieldChooserShowing(FieldChooserShowingEventArgs e)
        {
            if (this.FieldChooserShowing != null)
            {
                this.FieldChooserShowing(this, e);
            }
        }
        /// <summary>
        /// Raises the FieldChooserShown Event.
        /// </summary>
        /// <param name="e"> The event data. </param>
        public virtual void OnFieldChooserShown(FieldChooserShownEventArgs e)
        {
            if (this.FieldChooserShown != null)
            {
                this.FieldChooserShown(this, e);
            }
        }
        /// <summary>
        /// Raises the FieldChooserClosing Event.
        /// </summary>
        /// <param name="e"> The event data. </param>
        public virtual void OnFieldChooserClosing(FieldChooserClosingEventArgs e)
        {
            if (this.FieldChooserClosing != null)
            {
                this.FieldChooserClosing(this, e);
            }
        }
        /// <summary>
        /// Raises the FieldChooserClosed Event.
        /// </summary>
        /// <param name="e"> The event data. </param>
        public virtual void OnFieldChooserClosed(FieldChooserClosedEventArgs e)
        {
            if (this.FieldChooserClosed != null)
            {
                this.FieldChooserClosed(this, e);
            }
        }
        /// <summary>
        ///   <para>Gets or sets the specific list in a <see cref="GridDataBoundGrid.DataSource" /> for which the <see cref="GridDataBoundGrid"/>
        /// control
        /// displays a gridModel.</para>
        /// </summary>
        [Category("Data"),
        DefaultValue(null),
        Editor("System.Windows.Forms.Design.DataMemberListEditor, System.Design", typeof(System.Drawing.Design.UITypeEditor)),
        Description("Indicates a sub-list of the DataSource to show in the DataGrid.")]
        [RefreshProperties(RefreshProperties.All)]
        public virtual string DataMember
        {
            get
            {
                if (binder == null)
                {
                    return string.Empty;
                }

                return binder.DataMember;
            }

            set
            {
                EnsureBinder();
                binder.DataMember = value;
            }
        }

        /// <summary>
        ///   <para>Gets or sets the datasource that the gridModel is displaying data for.</para>
        /// </summary>
        [Description("Indicates the source of data for the GridDataBoundGrid."),
        DefaultValue(null),
        Category("Data"),
        RefreshProperties(RefreshProperties.Repaint),
        TypeConverter("System.Windows.Forms.Design.DataSourceConverter, System.Design")]
#if SyncfusionFramework2_0
        [AttributeProvider(typeof(IListSource))]
#endif
        public virtual object DataSource
        {
            get
            {
                if (binder == null || _dataSource != null)
                {
                    return _dataSource;
                }

                return binder.DataSource;
            }

            set
            {
                EnsureBinder();
                // Delay setting the DataSource until grid is initialized and visible.
                if (!forceDataSource && !DesignMode && (inInit || this.firstShown))
                {
                    this._dataSource = value;
                }
                else
                {
                    if (BindingContext == null)                   
                    { 
                        BindingContext = new BindingContext(); 
                    }

                    binder.DataSource = value;
                }
            }
        }

        bool initCC = false;
        object _dataSource;
        bool forceDataSource = false;

        void EnsureBinder()
        {
            if (binder == null)
            {
                binder = CreateBinder();
                binder.BindingContext = this.BindingContext;
                binder.Site = this.Site;
                initCC = Model.DataProvider == null;
                Model.DataProvider = binder;
                WireBinder();
            }
        }

        /// <summary>
        /// Creates an instance of the GridModelDataBinder object.
        /// </summary>
        /// <returns>The GridModelDataBinder object that provides plumbing to the datasource for this grid.</returns>
        protected virtual GridModelDataBinder CreateBinder()
        {
            return new GridModelDataBinder(Model);
        }

        void WireBinder()
        {
            binder.CurrentPositionChanged += new EventHandler(BinderCurrentPositionChanged);
            binder.EditModeChanged += new EventHandler(BinderEditModeChanged);
            binder.ItemChanged += new ItemChangedEventHandler(BinderItemChanged);
            binder.RowChanged += new EventHandler(BinderRowChanged);
            binder.viewCount++;
            isWired = true;
        }

        /// <override/>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                ////MessageBox.Show("Dispose DataGrid " + this.isDesign.ToString());
                this.UnwireBinder();
                if (this.ShowTreeLines)
                {
                    this.ShowTreeLines = false;
                }

                if (binder != null)
                {
                    if (this.isDesign || binder.viewCount == 0)
                    {
                        this.binder.Dispose();
                    }
                }
                if (this.Model != null && this.FindParentForm() != null && this.FindParentForm().Disposing)
                {
                    this.Model.Dispose();
                }
            }

            base.Dispose(disposing);
        }

        void UnwireBinder()
        {
            if (binder != null)
            {
                binder.viewCount--;

                binder.CurrentPositionChanged -= new EventHandler(BinderCurrentPositionChanged);
                binder.EditModeChanged -= new EventHandler(BinderEditModeChanged);
                binder.ItemChanged -= new ItemChangedEventHandler(BinderItemChanged);
                binder.RowChanged -= new EventHandler(BinderRowChanged);

                binder.RecordsRemoved -= new EventHandler(BinderDataSourceChanged);
                binder.DataSourceChanged -= new EventHandler(BinderDataSourceChanged);
                binder.GridBoundColumnsChanged -= new EventHandler(BinderGridBoundColumnsChanged);
                isWired = false;
            }
        }

        void BinderItemChanged(object sender, ItemChangedEventArgs e)
        {
#if DEBUG
            if (Switches.GridDataBoundGrid.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(PaneDesc, e.Index, Parent.GetType().Name);
            }
#else
            ;
#endif
            if (!CurrentCell.IsInMoveTo && e.Index != -1)
            {
                Model.ResetVolatileData();
                this.RefreshRange(GridRangeInfo.Row(binder.PositionToRowIndex(e.Index)));
            }
        }

        void BinderRowChanged(object sender, EventArgs e)
        {
#if DEBUG
            if (Switches.GridDataBoundGrid.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(PaneDesc, binder.CurrentRowIndex, CurrentCell.ColIndex);
            }
#else
            ;
#endif

            if (!CurrentCell.IsInMoveTo && !CurrentCell.IsInActivate && !CurrentCell.IsInDeactivate)
            {
                BeginUpdate(BeginUpdateOptions.InvalidateAndScroll);
                Model.ResetVolatileData();
                if (CurrentCell.HasCurrentCell)
                {
                    CurrentCell.Deactivate(true);
                    CurrentCell.Activate(binder.CurrentRowIndex, CurrentCell.ColIndex, GridSetCurrentCellOptions.NoSetFocus);
                }
                else
                {
                    CurrentCell.SetPositionNoActivate(binder.CurrentRowIndex, CurrentCell.ColIndex);
                }

                EndUpdate(true);
            }
            else
            {
                this.RefreshRange(GridRangeInfo.Row(binder.CurrentRowIndex));
                CurrentCell.Refresh();
            }
        }

        bool inBinderCurrentPositionChanged = false;

        void BinderCurrentPositionChanged(object sender, EventArgs e)
        {
            if (inBinderCurrentPositionChanged)
            {
                return;
            }

            inBinderCurrentPositionChanged = true;

            try
            {
#if DEBUG
                if (Switches.GridDataBoundGrid.TraceVerbose)
                {
                    TraceUtil.TraceCurrentMethodInfo(PaneDesc, binder.SavedPosition, binder.CurrentPosition);
                }
#else
                ;
#endif

                if (binder.SavedPosition == -1
                    || binder.CurrentPosition == -1
                    || binder.RecordDiffersAtPosition(binder.SavedPosition, binder.CurrentPosition))
                {
                    if (binder.SavedPosition != -1)
                    {
                        this.RefreshRange(GridRangeInfo.Row(binder.SavedRowIndex), GridRangeOptions.MergeCoveredCells);
                    }

                    if (binder.CurrentPosition != -1)
                    {
                        this.InvalidateRange(GridRangeInfo.Row(binder.CurrentRowIndex), GridRangeOptions.MergeCoveredCells);
                        if (!CurrentCell.IsInActiveOrDeactivate && !CurrentCell.IsInMoveTo && !CurrentCell.IsInActivated)
                        {
                            CurrentCell.MoveTo(binder.CurrentRowIndex, CurrentCell.ColIndex, GridSetCurrentCellOptions.NoSyncCurrentCell);
                        }
                    }
                }
            }
            finally
            {
                inBinderCurrentPositionChanged = false;
            }
        }

        void BinderEditModeChanged(object sender, EventArgs e)
        {
#if DEBUG
            if (Switches.GridDataBoundGrid.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(this.PaneDesc);
            }
#else
            ;
#endif
            GridRangeInfo range = GridRangeInfo.Row(binder.CurrentRowIndex);
            Rectangle rect = RangeInfoToRectangle(range, GridRangeOptions.MergeCoveredCells);
            if (binder.IsAppendRow)
            {
                rect.Height = GridBounds.Bottom - rect.Top;
            }

            ViewLayout.Reset();
            this.Invalidate(rect);
            ////UpdateScrollBars();
        }

        bool initialized = false;

        /// <override/>
        protected override void OnBindingContextChanged(EventArgs e)
        {
#if DEBUG
            if (Switches.GridDataBoundGrid.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(this.PaneDesc);
            }
#else
            ;
#endif
            if (binder != null)
            {
                binder.BindingContext = this.BindingContext;
            }

            base.OnBindingContextChanged(e);
        }

        /// <override/>
        protected override void OnSplitterPaneClosing(EventArgs e)
        {
            if (binder != null)
            {
                this.UnwireBinder();
            }

            base.OnSplitterPaneClosing(e);
            this.binder = null;
        }

        /// <override/>
        protected override void InitLayout()
        {
#if DEBUG
            if (Switches.GridDataBoundGrid.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(this.PaneDesc);
            }
#else
            ;
#endif
            EnsureBinder();
            if (binder.BindingContext == null || binder.List == null)
            {
                binder.BindingContext = this.BindingContext;
                binder.Set_ListManager(binder.DataSource, binder.DataMember, true);
            }

            binder.RecordsRemoved += new EventHandler(BinderDataSourceChanged);
            binder.DataSourceChanged += new EventHandler(BinderDataSourceChanged);
            binder.GridBoundColumnsChanged += new EventHandler(BinderGridBoundColumnsChanged);
            this.RegisterDataObjectConsumer(new GridDataBoundGridTextDataObjectConsumer(this));

            base.InitLayout();

            initialized = true;
            isDesign = this.DesignMode;

            if (inInit)
            {
                return;
            }

            bool nr = false;
            forceDataSource = true;
            if (binder.DataSource == null && this._dataSource != null)
            {
                nr = true;
                this.DataSource = _dataSource;
            }

            this._dataSource = null;

            if (!initCC)
            {
                CurrentCell.SetPositionNoActivate(binder.CurrentRowIndex, Model.Cols.HeaderCount + 1);
            }
            else
            {
                if (!nr)
                {
                    this.ResizeVisibleRowsToFit();
                }

                CurrentCell.MoveTo(Model.Rows.HeaderCount + 1, Model.Cols.HeaderCount + 1);
            }
        }

        /// <override/>
        protected override void OnEnter(EventArgs e)
        {
            base.OnEnter(e);

            if (binder != null && binder.CurrentPosition <= 0)
            {
                binder.SetCurrentPosition(0, false);
            }

            CurrentCell.Refresh();
        }

        /// <override/>
        protected override void OnValidating(CancelEventArgs e)
        {
            base.OnValidating(e);

            if (!e.Cancel)
            {
                // Fixes issues with new records and related tables in MasterDetails scenario
                CurrentCell.EndEdit();
                if (!CurrentCell.IsEditing)
                {
                    CurrentCell.ConfirmChanges();
                }

                e.Cancel |= CurrentCell.IsModified;

                if (binder != null && binder.listManager != null)
                {
                    binder.listManager.Refresh();
                }
            }
        }

        /// <override/>
        protected override void OnLeave(EventArgs e)
        {
            base.OnLeave(e);
        }

        void BinderDataSourceChanged(object sender, EventArgs e)
        {
#if DEBUG
            if (Switches.GridDataBoundGrid.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(PaneDesc, initialized, initCC);
            }
#else
            ;
#endif
            if (initialized && !inInit)
            {
                CurrentCell.Deactivate(true);
                if (binder.CurrentPosition >= 0)
                {
                    CurrentCell.Activate(binder.CurrentRowIndex, Math.Max(Model.Cols.HeaderCount + 1, CurrentCell.ColIndex));
                    // TODO: could add here a reason, e.g. "BinderDataSourceChanged"
                    OnRowEnter(new GridRowEventArgs(binder.CurrentRowIndex));
                }
                else
                {
                    CurrentCell.MoveTo(Model.Rows.HeaderCount + 1, Model.Cols.HeaderCount + 1);
                }

                Refresh();

                if (DesignMode)
                {
                    this.ResizeVisibleRowsToFit();
                }
            }
        }

        void BinderGridBoundColumnsChanged(object sender, EventArgs e)
        {
#if DEBUG
            if (Switches.GridDataBoundGrid.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(PaneDesc, initialized, initCC);
            }
#else
            ;
#endif
            if (initialized)
            {
                BeginUpdate(BeginUpdateOptions.None);
                Model.ResetVolatileData();
                ViewLayout.Reset();
                this.ResizeVisibleRowsToFit();
                Refresh();
                EndUpdate(true);
            }
        }

        /// <summary>
        /// Gets or sets the columns to be displayed in the GridDataBoundGrid.
        /// </summary>
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Content)]
        [LocalizableAttribute(true)]
        [Description("Manages the columns to be displayed in the GridDataBoundGrid."),
        Category("Data"),
        RefreshProperties(RefreshProperties.All)]
        public virtual GridBoundColumnsCollection GridBoundColumns
        {
            get
            {
                EnsureBinder();
                return binder.GridBoundColumns;
            }

            set
            {
                EnsureBinder();
                binder.GridBoundColumns = value;
                Refresh();
            }
        }

        void ResetGridBoundColumns()
        {
            if (binder != null)
            {
                if (Site != null)
                {
                    IDesignerHost designerHost = (IDesignerHost)Site.GetService(typeof(IDesignerHost));

                    DesignerTransaction designerTransaction = null;
                    if (designerHost != null)
                    {
                        designerTransaction = designerHost.CreateTransaction("Reset GridBoundColumns");
                    }

                    this.GridBoundColumns = binder.CreateBoundColumnsCollection();

                    if (designerTransaction != null)
                    {
                        designerTransaction.Commit();
                    }
                    ////binder.ResetGridBoundColumns();
                }
                else
                {
                    this.GridBoundColumns = binder.CreateBoundColumnsCollection();
                }
            }
        }

        bool ShouldSerializeGridBoundColumns()
        {
            return binder != null && binder.GridBoundColumns.Count > 0;
        }

        bool leaveRow = false;
        bool rejected = false;

        /// <override/>
        protected override void OnCurrentCellRejectedChanges(EventArgs e)
        {
#if DEBUG
            if (Switches.GridDataBoundGrid.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(this.PaneDesc);
            }
#else
            ;
#endif
            base.OnCurrentCellRejectedChanges(e);
            if (IsCurrentCellUnboundCell())
            {
                return;
            }

            CancelUpdate();
            Model.ResetVolatileData();
            binder.ResetField(binder.ColIndexToField(CurrentCell.ColIndex));
            if (!binder.IsAnyDirtyField())
            {
                binder.CancelEdit();
            }

            CurrentCell.Refresh();
            ////Model.ResetActiveText(CurrentCell.RowIndex, CurrentCell.ColIndex);
            ////CurrentCell.Renderer.Initialize(CurrentCell.RowIndex, CurrentCell.ColIndex);
        }

        /// <summary>
        /// Allows you to adjust the current cell position before the <see cref="GridControlBase.CurrentCellMoving"/> event
        /// handler proceeds.
        /// </summary>
        /// <param name="e">The <see cref="GridCurrentCellMovingEventArgs"/> with event data.</param>
        /// <remarks>
        /// The default implementation will check if the current cell is about to be moved onto a row header. <para/>
        /// If this is the case, the column index will be changed so that the current cell moves onto the first
        /// column in the row.
        /// </remarks>
        protected virtual void AdjustRowHeader(GridCurrentCellMovingEventArgs e)
        {
            int colIndex = e.ColIndex;
            if (colIndex <= 0)
            {
                colIndex = Model.Cols.HeaderCount + 1;
            }

            while (!this.GetViewStyleInfo(e.RowIndex, colIndex).Enabled)
            {
                if (colIndex < Model.ColCount)
                {
                    colIndex++;
                }
                else
                {
                    return;
                }
            }

            e.ColIndex = colIndex;
        }

        /// <summary>
        /// Allows you to adjust the current cell position before the <see cref="GridControlBase.CurrentCellMoving"/> event
        /// handler proceeds.
        /// </summary>
        /// <param name="e">The <see cref="GridCurrentCellMovingEventArgs"/> with event data.</param>
        /// <remarks>
        /// The default implementation will check if the current cell is about to be moved onto a column header. <para/>
        /// If this is the case, the row index will be changed so that the current cell moves onto the first
        /// row in the column.
        /// </remarks>
        protected virtual void AdjustColHeader(GridCurrentCellMovingEventArgs e)
        {
            int rowIndex = e.RowIndex;

            while (!this.GetViewStyleInfo(rowIndex, e.ColIndex).Enabled)
            {
                if (rowIndex < Model.RowCount)
                {
                    rowIndex++;
                }
                else
                {
                    return;
                }
            }

            e.RowIndex = rowIndex;
        }

        /// <override/>
        protected override void OnCurrentCellMoving(GridCurrentCellMovingEventArgs e)
        {
#if DEBUG
            if (Switches.GridDataBoundGrid.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(this.PaneDesc);
            }
#else
            ;
#endif
            base.OnCurrentCellMoving(e);

            if (!e.Cancel)
            {
                AdjustRowHeader(e);
                AdjustColHeader(e);

                if (IsCurrentCellUnboundCell(e.RowIndex, e.ColIndex) || e.Cancel)
                {
                    return;
                }

                leaveRow = binder.RecordDiffersAtRowIndex(e.RowIndex, binder.CurrentRowIndex);
                if (leaveRow)
                {
                    e.Options &= GridSetCurrentCellOptions.NoSetFocus | GridSetCurrentCellOptions.NoSelectRange; // All but NoSetFocus is cleared
                    BeginUpdate(BeginUpdateOptions.InvalidateAndScroll | BeginUpdateOptions.SynchronizeScrollBars);
                }
            }
        }

        internal bool isDeactivated = false;
        string controlText;

        /// <override/>
        protected override void OnCurrentCellDeactivating(CancelEventArgs e)
        {
#if DEBUG
            if (Switches.GridDataBoundGrid.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(this.PaneDesc);
            }
#else
            ;
#endif
            base.OnCurrentCellDeactivating(e);

            if (e.Cancel
                || (CurrentCell.IsInMoveTo && IsCurrentCellUnboundCell(CurrentCell.MoveToRowIndex, CurrentCell.MoveToColIndex))
                || (!CurrentCell.IsInMoveTo && IsCurrentCellUnboundCell(CurrentCell.RowIndex, CurrentCell.ColIndex)))
            {
                return;
            }

            isDeactivated = false;
            rejected = false;

            if (CurrentCell.IsInMoveTo && binder.RecordDiffersAtRowIndex(CurrentCell.RowIndex, CurrentCell.MoveToRowIndex))
            {
                binder.ResetError();
                controlText = CurrentCell.Renderer.ControlText;
            }

            if (!CurrentCell.IsModified)
            {
                if (CurrentCell.IsInMoveTo && binder.RecordDiffersAtRowIndex(binder.CurrentRowIndex, CurrentCell.MoveToRowIndex))
                {
                    e.Cancel = !EndEdit();
                }
            }
            // otherwise, wait for AcceptedChanges
        }

        bool EndEdit()
        {
            GridRowEventArgs e = new GridRowEventArgs(binder.CurrentRowIndex, binder.IsAddNew);
            try
            {
                OnRowLeave(e);
                if (!e.Cancel)
                {
                    if (binder.IsEditing)
                    {
                        if (binder.IsAddNew || binder.IsAppendRow)
                        {
                            Invalidate(this.ViewLayout.RectangleBottomOfRow(binder.CurrentRowIndex - 1, GridCellSizeKind.VisibleSize));
                        }

                        binder.EndEdit();
                        InvalidateRange(GridRangeInfo.Row(binder.CurrentRowIndex), GridRangeOptions.MergeCoveredCells);
                        this.OnRowSaved(e);
                    }
                    else
                    {
                        InvalidateRange(GridRangeInfo.Row(binder.CurrentRowIndex), GridRangeOptions.MergeCoveredCells);
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

                if (CurrentCell.IsInMoveTo)
                {
                    throw;
                }
                else
                {
                    CurrentCell.ErrorMessage = ex.Message;
                    CurrentCell.Exception = ex;
                    if (!this.IsInLeaveOrValidate && !this.IsValidating)
                    {
                        CancelUpdate();
                        this.RaiseValidateFailed(CurrentCell.IsInDeactivate ? CurrentCell.ColIndex : -1);
                    }

                    e.Cancel = true;
                }
            }

            return !e.Cancel;
        }

        bool BeginEdit()
        {
            if (binder.IsEditing)
            {
                return true;
            }

            GridRowEventArgs e = new GridRowEventArgs(binder.CurrentRowIndex, binder.IsAppendRow);
            int rowIndex = binder.CurrentRowIndex;
            this.BeginUpdate(BeginUpdateOptions.InvalidateAndScroll);
            this.OnRowEditing(e);
            binder.BeginEdit();
            if (binder.IsAddNew || binder.IsAppendRow)
            {
                Invalidate(this.ViewLayout.RectangleBottomOfRow(rowIndex, GridCellSizeKind.VisibleSize));
            }

            this.EndUpdate(true);
            return !e.Cancel;
        }

        /// <override/>
        protected override void OnCurrentCellAcceptedChanges(CancelEventArgs e)
        {
#if DEBUG
            if (Switches.GridDataBoundGrid.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(this.PaneDesc);
            }
#else
            ;
#endif
            base.OnCurrentCellAcceptedChanges(e);

            if (e.Cancel
                || (CurrentCell.IsInMoveTo && IsCurrentCellUnboundCell(CurrentCell.MoveToRowIndex, CurrentCell.MoveToColIndex))
                || (!CurrentCell.IsInMoveTo && IsCurrentCellUnboundCell(CurrentCell.RowIndex, CurrentCell.ColIndex)))
            {
                return;
            }

            if (CurrentCell.IsInMoveTo && binder.RecordDiffersAtRowIndex(binder.CurrentRowIndex, CurrentCell.MoveToRowIndex)
                && !this.IsCurrentCellUnboundCell(CurrentCell.MoveToRowIndex, CurrentCell.MoveToColIndex))
            {
                e.Cancel = !EndEdit();
            }

            isDeactivated = true;
        }

        /// <override/>
        protected override void OnCurrentCellConfirmChangesFailed(EventArgs e)
        {
#if DEBUG
            if (Switches.GridDataBoundGrid.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(this.PaneDesc);
            }
#else
            ;
#endif
            base.OnCurrentCellConfirmChangesFailed(e);

            if ((CurrentCell.IsInMoveTo && IsCurrentCellUnboundCell(CurrentCell.MoveToRowIndex, CurrentCell.MoveToColIndex))
                || (!CurrentCell.IsInMoveTo && IsCurrentCellUnboundCell(CurrentCell.RowIndex, CurrentCell.ColIndex)))
            {
                return;
            }

            if (!CurrentCell.IsInDeactivate && !this.IsInLeaveOrValidate)
            {
                this.RaiseValidateFailed(CurrentCell.ColIndex);
            }
        }

        GridRangeInfo markedHeaderRange = GridRangeInfo.Empty;
        GridRangeInfo deactivatedMarkedHeader = GridRangeInfo.Empty;

        /// <override/>
        protected override void OnCurrentCellDeactivated(GridCurrentCellDeactivatedEventArgs e)
        {
#if DEBUG
            if (Switches.GridDataBoundGrid.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(PaneDesc, e);
            }
#else
            ;
#endif

            if (!CurrentCell.IsInMoveTo && markedHeaderRange.IsCells)
            {
                this.InvalidateRange(markedHeaderRange, GridRangeOptions.MergeCoveredCells);
                markedHeaderRange = GridRangeInfo.Empty;
            }

            base.OnCurrentCellDeactivated(e);
            if ((CurrentCell.IsInMoveTo && IsCurrentCellUnboundCell(CurrentCell.MoveToRowIndex, CurrentCell.MoveToColIndex))
                || (!CurrentCell.IsInMoveTo && IsCurrentCellUnboundCell(e.RowIndex, e.ColIndex)))
            {
                return;
            }

            isDeactivated = true;
        }

        /// <override/>
        protected override void OnCurrentCellMoveFailed(GridCurrentCellMoveFailedEventArgs e)
        {
#if DEBUG
            if (Switches.GridDataBoundGrid.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(PaneDesc, e);
            }
#else
            ;
#endif
            base.OnCurrentCellMoveFailed(e);

            if (IsCurrentCellUnboundCell(CurrentCell.MoveToRowIndex, CurrentCell.MoveToColIndex))
            {
                return;
            }

            this.RaiseValidateFailed(CurrentCell.ColIndex);
        }

        /// <summary>
        /// Occurs when validation of the current record or current cell fails.
        /// </summary>
        /// <remarks>
        /// You can display a message box or correct cell values in this event.
        /// </remarks>
        [Category("Focus")]
        [Description("Occurs when validation of the current record or current cell fails")]
        public event GridValidateFailedEventHandler ValidateFailed;

        /// <summary>
        /// Raises the <see cref="ValidateFailed"/> event.
        /// </summary>
        /// <param name="e">A <see cref="EventArgs" /> that contains the event data.</param>
        /// <para/>
        /// <example>
        /// <code lang="C#">
        ///         private void gridDataBoundGrid3_ValidateFailed(object sender, GridValidateFailedEventArgs e)
        ///         {
        ///             TraceUtil.TraceCurrentMethodInfo(e);
        ///             GridCurrentCell cc = this.gridDataBoundGrid3.CurrentCell;
        ///             cc.Grid.CancelUpdate();
        ///             MessageBox.Show("My Error:" + cc.ErrorMessage);
        ///             cc.RejectChanges();
        ///             cc.ResetError();
        ///         }
        /// </code>
        /// </example>
        protected virtual void OnValidateFailed(GridValidateFailedEventArgs e)
        {
#if DEBUG
            if (Switches.GridDataBoundGridEvents.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(PaneDesc, e);
            }
#else
            ;
#endif
            if (ValidateFailed != null)
            {
                ValidateFailed(this, e);
            }
        }

        void RaiseValidateFailed(int colIndex)
        {
            Model.Selections.Clear(true);
            CancelUpdate();
            GridValidateFailedEventArgs e = new GridValidateFailedEventArgs();
            OnValidateFailed(e);
            GridCurrentCell cc = CurrentCell;
            if (!e.Handled && cc.ErrorMessage.Length > 0 && !this.IsMousePressed)
            {
                if (!this.IsInLeaveOrValidate && ContainsFocus)
                {
                    cc.Refresh();
                }

                cc.DisplayWarningText(cc.ErrorMessage);
                cc.ResetError();
            }

            if (cc.IsActive && ContainsFocus)
            {
                if (!CurrentCell.HasCurrentCellAt(binder.CurrentRowIndex, cc.ColIndex))
                {
                    CurrentCell.Deactivate(true);
                    CurrentCell.Activate(binder.CurrentRowIndex, cc.ColIndex);
                }
                else
                {
                    cc.Refresh();
                }

                InvalidateRange(GridRangeInfo.Row(cc.RowIndex), GridRangeOptions.MergeCoveredCells);
            }
        }

        /// <override/>
        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
        }

        /// <override/>
        protected override void OnCurrentCellDeactivateFailed(EventArgs e)
        {
#if DEBUG
            if (Switches.GridDataBoundGrid.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(this.PaneDesc);
            }
#else
            ;
#endif
            base.OnCurrentCellDeactivateFailed(e);

            if ((CurrentCell.IsInMoveTo && IsCurrentCellUnboundCell(CurrentCell.MoveToRowIndex, CurrentCell.MoveToColIndex))
                || (!CurrentCell.IsInMoveTo && IsCurrentCellUnboundCell(CurrentCell.RowIndex, CurrentCell.ColIndex)))
            {
                return;
            }

            if (!CurrentCell.IsInMoveTo || binder.RecordDiffersAtRowIndex(CurrentCell.MoveFromRowIndex, CurrentCell.MoveToRowIndex))
            {
                // In case of MouseDown this will be handled in GridControlBase.OnMouseDown.
                if (binder.HasError && !this.IsMousePressed)  
                {
                    CurrentCell.ErrorMessage = binder.ErrorMessage;
                    CurrentCell.Exception = binder.Exception;
                }
            }

            if (CurrentCell.IsActive && !binder.RecordDiffersAtRowIndex(CurrentCell.RowIndex, binder.CurrentRowIndex))
            {
                binder.CurrentPosition = binder.RowIndexToPosition(CurrentCell.RowIndex);
            }
            else
            {
                CurrentCell.Activate(binder.CurrentRowIndex, CurrentCell.ColIndex);
                CurrentCell.BeginEdit();
            }

            if (!rejected && CurrentCell.HasCurrentCell)
            {
                CurrentCell.IsModified = true;
            }

            InvalidateRange(GridRangeInfo.Row(CurrentCell.RowIndex), GridRangeOptions.MergeCoveredCells);

            if (leaveRow)
            {
                EndUpdate(true);
            }
        }

        /// <override/>
        protected override void OnCurrentCellActivating(GridCurrentCellActivatingEventArgs e)
        {
#if DEBUG
            if (Switches.GridDataBoundGrid.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(PaneDesc, e);
            }
#else
            ;
#endif
            if (e.Cancel
                || (CurrentCell.IsInMoveTo && IsCurrentCellUnboundCell(CurrentCell.MoveToRowIndex, CurrentCell.MoveToColIndex))
                || (!CurrentCell.IsInMoveTo && IsCurrentCellUnboundCell(e.RowIndex, e.ColIndex)))
            {
                base.OnCurrentCellActivating(e);
                return;
            }

            if (!this.isDeactivated && CurrentCell.IsInMoveTo && (CurrentCell.MoveFromActiveState || binder.HasError))
            {
                e.RowIndex = CurrentCell.MoveFromRowIndex;
                e.ColIndex = CurrentCell.MoveFromColIndex;
            }

            if (!CurrentCell.IsInMoveTo || !CurrentCell.MoveFromActiveState || leaveRow || binder.RecordDiffersAtRowIndex(CurrentCell.MoveFromRowIndex, CurrentCell.MoveToRowIndex))
            {
                if (binder.IsEditing)
                {
                    bool b = true;
                    try
                    {
                        b = EndEdit();
                    }
                    catch (Exception ex)
                    {
                        TraceUtil.TraceExceptionCatched(ex);
                        if (!ExceptionManager.RaiseExceptionCatched(this, ex))
                        {
                            throw;
                        }

                        b = false;
                    }

                    if (!b)
                    {
                        this.RaiseValidateFailed(CurrentCell.ColIndex);
                        e.RowIndex = binder.CurrentRowIndex;
                        binder.CancelEdit();
                        e.Cancel = true;
                        base.OnCurrentCellActivating(e);
                        return;
                    }
                }

                // TODO: could add here a reason, e.g. "CurrentCellMoving",
                // but for now the user can also simple check CurrentCell.IsMoving
                // when CurrentCell.IsMoving is true the event can be canceled before
                // the current cell is moved. Otherwise the event is fired only
                // to notify that the data source has changed and can not be canceled
                // (see BinderDataSourceChanged)
                GridRowEventArgs ce = new GridRowEventArgs(e.RowIndex);
                this.OnRowEnter(ce);
                if (ce.Cancel)
                {
                    e.Cancel = true;
                    return;
                }

                e.RowIndex = ce.RowIndex;

                base.OnCurrentCellActivating(e);

                binder.SetCurrentPosition(binder.RowIndexToPosition(e.RowIndex), false);
            }
            else
            {
                base.OnCurrentCellActivating(e);
            }
        }

        /// <override/>
        protected override void OnCurrentCellActivated(EventArgs e)
        {
#if DEBUG
            if (Switches.GridDataBoundGrid.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(this.PaneDesc);
            }
#else
            ;
#endif
            base.OnCurrentCellActivated(e);

            try
            {
                if ((CurrentCell.IsInMoveTo && IsCurrentCellUnboundCell(CurrentCell.MoveToRowIndex, CurrentCell.MoveToColIndex))
                    || (!CurrentCell.IsInMoveTo && IsCurrentCellUnboundCell(CurrentCell.RowIndex, CurrentCell.ColIndex)))
                {
                    RefreshRange(GridRangeInfo.Row(CurrentCell.RowIndex));
                    binder.RaiseCurrentPositionChanged();
                    return;
                }

                isDeactivated = false;
                if (!this.inBinderCurrentPositionChanged &&
                    (!CurrentCell.IsInMoveTo || leaveRow || binder.RecordDiffersAtRowIndex(CurrentCell.MoveFromRowIndex, CurrentCell.MoveToRowIndex)))
                {
                    RefreshRange(GridRangeInfo.Row(CurrentCell.RowIndex));
                    binder.RaiseCurrentPositionChanged();
                    if (!binder.InRowChanged && !IsMousePressed)
                    {
                        CurrentCell.ScrollInView(GridScrollCurrentCellReason.KeyPress);
                    }
                }

                binder.ResetError();

                if (CurrentCell.HasCurrentCell && binder.IsFieldDirty(binder.ColIndexToField(CurrentCell.ColIndex)))
                {
                    CurrentCell.IsModified = true;
                }

                if (leaveRow)
                {
                    EndUpdate(true);
                    leaveRow = false;
                }
            }
            finally
            {
                GridCurrentCell cc = this.CurrentCell;
                if (!this.IsCurrentCellUnboundCell()
                    && binder.currentRecordState != null)
                {
                    binder.SyncCurrentRecordState(binder.RowIndexToPosition(CurrentCell.RowIndex));
                    GridHierarchyLevel ghl = binder.levels[binder.currentRecordState.level] as GridHierarchyLevel;
                    if (ghl.headerTopRow != -1)
                    {
                        int rowIndex = ghl.headerTopRow + binder.currentRecordState.row;
                        GridRangeInfo newMarkedHeader;
                        if (this.HighlightCurrentColumnHeader)
                        {
                            newMarkedHeader = GridRangeInfo.Cell(rowIndex, cc.ColIndex);
                        }
                        else
                        {
                            newMarkedHeader = GridRangeInfo.Empty;
                        }

                        if (!newMarkedHeader.Equals(markedHeaderRange))
                        {
                            Update();
                            this.InvalidateRange(markedHeaderRange, GridRangeOptions.MergeCoveredCells);
                            this.InvalidateRange(newMarkedHeader, GridRangeOptions.MergeCoveredCells);
                            markedHeaderRange = newMarkedHeader;
                        }
                    }
                }
            }
        }

        /// <override/>
        protected override void OnCurrentCellStartEditing(CancelEventArgs e)
        {
#if DEBUG
            if (Switches.GridDataBoundGrid.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(this.PaneDesc);
            }
#else
            ;
#endif
            base.OnCurrentCellStartEditing(e);

            if (binder.IsFieldDirty(binder.ColIndexToField(CurrentCell.ColIndex)))
            {
                CurrentCell.IsModified = true;
            }
        }

        /// <override/>
        protected override void OnCurrentCellControlGotFocus(ControlEventArgs e)
        {
            if (CurrentCell.HasCurrentCell && !CurrentCell.IsInAcceptedChanges && CurrentCell.IsEditing && binder.IsFieldDirty(binder.ColIndexToField(CurrentCell.ColIndex)))
            {
                CurrentCell.IsModified = true;
            }
#if DEBUG

            if (Switches.GridDataBoundGrid.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(this.PaneDesc);
            }
#else
            ;
#endif
            base.OnCurrentCellControlGotFocus(e);
        }

        /// <override/>
        protected override void OnControlGotFocus()
        {
            if (CurrentCell.HasCurrentCell && !CurrentCell.IsInAcceptedChanges && CurrentCell.IsEditing && binder.IsFieldDirty(binder.ColIndexToField(CurrentCell.ColIndex)))
            {
                CurrentCell.IsModified = true;
            }
#if DEBUG
            if (Switches.GridDataBoundGrid.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(this.PaneDesc);
            }
#else
            ;
#endif
            base.OnControlGotFocus();
        }

        /// <override/>
        /// <summary>
        /// Determines if the current cell is shown at the specified row.
        /// </summary>
        /// <param name="rowIndex">Row index.</param>
        /// <returns>True if it is shown at the given row.</returns>
        public override bool IsShowCurrentRow(int rowIndex)
        {
            return CurrentCell.HasCurrentCell && !binder.RecordDiffersAtRowIndex(binder.CurrentRowIndex, rowIndex);
        }

        internal bool IsCurrentCellUnboundCell()
        {
            return CurrentCell.HasCurrentCell
                && IsCurrentCellUnboundCell(CurrentCell.RowIndex, CurrentCell.ColIndex);
        }

        internal bool IsCurrentCellUnboundCell(int rowIndex, int colIndex)
        {
            if (rowIndex <= Model.Rows.HeaderCount
                || colIndex <= Model.Cols.HeaderCount)
            {
                return true;
            }

            int fieldNum = binder.ColIndexToField(colIndex);
            if (fieldNum == -1)
            {
                return true;
            }

            int position = binder.RowIndexToPosition(rowIndex);
            GridBoundRecordState state = binder.GetRecordStateAtPosition(position);
            GridHierarchyLevel ghl = binder.levels[state.level] as GridHierarchyLevel;
            fieldNum = ghl.RowFieldToField(state.row, fieldNum);
            GridBoundColumnsCollection columns = ghl.InternalColumns;
            return fieldNum >= columns.Count || columns[fieldNum] == null || columns[fieldNum].PropertyDescriptor == null;
        }

        /// <override/>
        protected override void OnCurrentCellChanging(CancelEventArgs e)
        {
#if DEBUG
            if (Switches.GridDataBoundGrid.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(this.PaneDesc);
            }
#else
            ;
#endif
            base.OnCurrentCellChanging(e);

            if (IsCurrentCellUnboundCell() || e.Cancel)
            {
                return;
            }

            if (!binder.AllowEdit || Model.ReadOnly || Model[CurrentCell.RowIndex, CurrentCell.ColIndex].ReadOnly)
            {
                e.Cancel = true;
            }
            else
            {
                e.Cancel = !BeginEdit();
            }
        }

        /// <override/>
        protected override void OnCurrentCellChanged(EventArgs e)
        {
            base.OnCurrentCellChanged(e);

            if (IsCurrentCellUnboundCell())
            {
                return;
            }

            if (CurrentCell.IsModified && !binder.IsEditing)
            {
                BeginEdit();
            }
        }

        /// <summary>
        /// Gets or sets the sort behavior if databound grid should sort a column if user clicks on it.
        /// </summary>
        [Description("Gets / sets the sort behavior if databound grid should sort a column if user clicks on it.")]
        [Category("Grid")]
        public GridSortBehavior SortBehavior
        {
            get
            {
                return binder != null ? binder.sortBehavior : GridSortBehavior.None;
            }

            set
            {
                EnsureBinder();
                binder.sortBehavior = value;
            }
        }

        /// <override/>
        protected override void OnCellDoubleClick(GridCellClickEventArgs e)
        {
#if DEBUG
            if (Switches.GridDataBoundGrid.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(PaneDesc, e);
            }
#else
            ;
#endif
            base.OnCellDoubleClick(e);

            if (!e.Cancel &&
                (e.RowIndex <= Model.Rows.HeaderCount
                || e.ColIndex <= Model.Cols.HeaderCount))
            {
                if (binder.sortBehavior == GridSortBehavior.DoubleClick)
                {
                    if (e.ColIndex > Model.Cols.HeaderCount)
                    {
                        if (e.RowIndex == 0)
                        {
                            SortColumn(e.ColIndex);
                            e.Cancel = true;
                            ////return;
                        }
                        else if (e.RowIndex < this.binder.RootHierarchyLevel.RowCountPerRecord)
                        {
                            int fieldNum = binder.ColIndexToField(e.ColIndex);
                            int internalFieldNum = binder.RootHierarchyLevel.RowFieldToField(e.RowIndex, fieldNum);
                            SortField(internalFieldNum);
                            e.Cancel = true;
                            ////return;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Sorts the grid by the specified column.
        /// </summary>
        /// <param name="colIndex">The column to use as a key for sorting the data in the grid.</param>
        /// <remarks>
        /// If you sort the same column again, the sort order will be reversed.
        /// </remarks>
        public virtual void SortColumn(int colIndex)
        {
            SortField(binder.ColIndexToField(colIndex));
        }

        void SortField(int fieldNum)
        {
            bool hasCC = CurrentCell.HasCurrentCell;
            int colIndex = CurrentCell.ColIndex;
            BeginUpdate();
            try
            {
                if (CommitChanges())
                {
                    CollapseAll();
                    binder.Sort(fieldNum);
                    Selections.Clear();
                    Refresh();
                    if (hasCC)
                    {
                        CurrentCell.MoveTo(binder.CurrentRowIndex, colIndex);
                    }
                }
            }
            finally
            {
                EndUpdate();
            }
        }

        /// <summary>
        /// Validates the current object and saves any pending changed in the current record.
        /// </summary>
        /// <returns>True if object is valid and changes could be saved; False otherwise.</returns>
        public bool CommitChanges()
        {
            CancelEventArgs e = new CancelEventArgs();
            OnValidating(e);
            OnValidated(EventArgs.Empty);
            return !e.Cancel;
        }

        /// <override/>
        protected override void OnCellClick(GridCellClickEventArgs e)
        {
#if DEBUG
            if (Switches.GridDataBoundGrid.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(PaneDesc, e);
            }
#else
            ;
#endif
            base.OnCellClick(e);

            if (!e.Cancel && e.RowIndex <= Model.Rows.HeaderCount)
            {
                if (binder.sortBehavior == GridSortBehavior.SingleClick)
                {
                    if (e.ColIndex > Model.Cols.HeaderCount)
                    {
                        if (e.RowIndex == 0)
                        {
                            SortColumn(e.ColIndex);
                        }
                        else if (e.RowIndex < this.binder.RootHierarchyLevel.RowCountPerRecord)
                        {
                            int fieldNum = binder.ColIndexToField(e.ColIndex);
                            int internalFieldNum = binder.RootHierarchyLevel.RowFieldToField(e.RowIndex, fieldNum);
                            SortField(internalFieldNum);
                        }
                    }
                }

                return;
            }
        }

        /// <override/>
        protected override void OnCellButtonClicked(GridCellButtonClickedEventArgs e)
        {
#if DEBUG
            if (Switches.GridDataBoundGrid.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(PaneDesc, e);
            }
#else
            ;
#endif
            base.OnCellButtonClicked(e);

            // Set current position and / or repaint row when user clicks the check box in the row header.
            int colIndex = e.ColIndex;
            int rowIndex = e.RowIndex;
            if (!e.Cancel && colIndex == 1 && binder.levels.Count > 1)
            {
                if (IsExpandedAtRowIndex(rowIndex))
                {
                    this.MoveToRow(rowIndex, colIndex + 1);
                    this.CollapseAtRowIndex(rowIndex);
                }
                else
                {
                    this.MoveToRow(rowIndex, colIndex + 1);
                    this.ExpandAtRowIndex(rowIndex);
                }

                return;
            }
        }

        void MoveToRow(int rowIndex, int colIndex)
        {
            GetNextCurrentCellPosition(GridDirectionType.Right, ref rowIndex, ref colIndex);
            CurrentCell.MoveTo(rowIndex, colIndex);
            this.InvalidateRange(GridRangeInfo.Row(rowIndex), GridRangeOptions.MergeCoveredCells);
        }

        /// <summary>
        /// Checks if relation is expanded at the specified row in the grid.
        /// </summary>
        /// <param name="rowIndex">The absolute row index.</param>
        /// <returns>True if it is expanded at the specified row.</returns>
        public bool IsExpandedAtRowIndex(int rowIndex)
        {
            int position = binder.RowIndexToPosition(rowIndex);
            return binder.IsExpanded(position);
        }

        /// <summary>
        /// Expands the relation at the specified row in the grid. Before the records are
        /// collapsed, a cancelable <see cref="RowCollapsing"/> event is raised. After the operation
        /// has completed a <see cref="RowCollapsed"/> event is raised.
        /// </summary>
        /// <param name="rowIndex">The absolute row index.</param>
        public void CollapseAtRowIndex(int rowIndex)
        {
            int position = binder.RowIndexToPosition(rowIndex);
            GridRowEventArgs e = new GridRowEventArgs(rowIndex);
            if (binder.GetRecordStateAtPosition(position).hasChildList)
            {
                this.OnRowCollapsing(e);
                if (!e.Cancel)
                {
                    binder.CollapseRecord(position);
                    this.OnRowCollapsed(e);
                }
            }
        }

        /// <summary>
        /// Expands the relation at the specified row in the grid. Before the records are
        /// expanded a cancelable <see cref="RowExpanding"/> event is raised. After the operation
        /// is complete, a <see cref="RowExpanded"/> event is raised.
        /// </summary>
        /// <param name="rowIndex">The absolute row index.</param>
        public void ExpandAtRowIndex(int rowIndex)
        {
            int position = binder.RowIndexToPosition(rowIndex);
            GridRowEventArgs e = new GridRowEventArgs(rowIndex);
            if (binder.GetRecordStateAtPosition(position).hasChildList)
            {
                this.OnRowExpanding(e);
                if (!e.Cancel)
                {
                    binder.ExpandRecord(position);
                    this.OnRowExpanded(e);
                }
            }
        }
        
        /// <summary>
        /// Expands all nodes in the grid.
        /// </summary>
        /// <remarks>
        /// Expanding all nodes can be a lengthy process. The grid will give feedback
        /// through a <see cref="OperationFeedback"/> object about the progress
        /// of the operation and gives the user the option to abort.
        /// </remarks>
        public void ExpandAll()
        {
            GridModel gridModel = Model;
            gridModel.BeginUpdate(BeginUpdateOptions.InvalidateAndScroll);

            binder.CancelEdit();
            if (gridModel.ActiveGridView != null)
            {
                gridModel.ActiveGridView.CurrentCell.Deactivate(true);
            }

            OperationFeedback op = new OperationFeedback(gridModel);
            op.Description = "Expand all records";
            op.AllowCancel = true;
            op.AllowRollback = false;

            try
            {
                int recordCount = binder.RecordCount * binder.RootHierarchyLevel.rowCount;
                int record = 0;
                int levelOneRecord = 0;
                float rootProgress = 0;

                int position = 0;
                while (position < recordCount + binder.expRecordCount)
                {
                    GridBoundRecordState state = binder.GetRecordStateAtPosition(position);
                    if (state.row > 0)
                    {
                        position++;
                        continue;
                    }

                    ExpandAtRowIndex(Binder.PositionToRowIndex(position));
                    position++;

                    // Feedback and option to abort long operation
                    if (op.ShouldShowFeedback)
                    {
                        if (state.level == 0)
                        {
                            rootProgress = record * 100f / recordCount;
                            op.PercentComplete = Math.Min(100, (int)rootProgress);
                            record++;
                            levelOneRecord = 0;
                        }
                        else if (state.level == 1 && recordCount < 25)
                        {
                            int levelOneRecordCount = state.table != null ? state.table.Count : 0;
                            // let's get more detailed only if root level is not giving enough feedback
                            float f2 = levelOneRecord * 100f / (levelOneRecordCount * recordCount);
                            op.PercentComplete = Math.Min(100, (int)(rootProgress + f2));
                            levelOneRecord++;
                        }
                    }

                    if (op.ShouldCancel)
                    {
                        return;
                    }
                }
            }
            finally
            {
                op.Close();
                gridModel.EndUpdate(true);
            }
        }

        /// <summary>
        /// Collapses all nodes in the grid.
        /// </summary>
        /// <remarks>
        /// Collapsing all nodes can be a lengthy process. The grid will give feedback
        /// through a <see cref="OperationFeedback"/> object about the progress
        /// of the operation and gives the user the option to abort.
        /// </remarks>
        public void CollapseAll()
        {
            GridModel gridModel = Model;
            gridModel.BeginUpdate(BeginUpdateOptions.InvalidateAndScroll);

            if (gridModel.ActiveGridView != null)
            {
                gridModel.ActiveGridView.CurrentCell.Deactivate(true);
            }

            OperationFeedback op = new OperationFeedback(gridModel);
            op.Description = "Collapse all records";
            op.AllowCancel = true;
            op.AllowRollback = false;

            try
            {
                int count = binder.RecordCount * binder.RootHierarchyLevel.rowCount;
                for (int n = 0; n < count; n++)
                {
                    CollapseAtRowIndex(Binder.PositionToRowIndex(n));
                    // Feedback and option to abort long operation
                    if (op.ShouldShowFeedback)
                    {
                        op.PercentComplete = Math.Min(100, n * 100 / count);
                    }

                    if (op.ShouldCancel)
                    {
                        return;
                    }
                }
            }
            finally
            {
                op.Close();
                gridModel.EndUpdate(true);
            }
        }
        
        /// <override/>
        /// <summary>
        /// User pressed key down.
        /// </summary>
        /// <param name="e">Event args.</param>
        public override void OnCurrentCellKeyDown(KeyEventArgs e)
        {
#if DEBUG
            if (Switches.KeyboardEvents.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(PaneDesc, e.Handled, e.KeyCode, Control.ModifierKeys);
            }
#else
            ;
#endif

            ////if (!IsCurrentCellUnboundCell())
            HandleBoundCurrentCellKeyDown(e);
            base.OnCurrentCellKeyDown(e);
        }

        /// <override/>
        protected override void OnKeyDown(KeyEventArgs e)
        {
#if DEBUG
            if (Switches.GridDataBoundGrid.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(PaneDesc, e.Handled, e.KeyCode, e.Modifiers);
            }
#else
            ;
#endif
            ////if (!IsCurrentCellUnboundCell())
            HandleBoundCurrentCellKeyDown(e);
            base.OnKeyDown(e);
        }
        
        /// <internalonly/>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected virtual void HandleBoundCurrentCellKeyDown(KeyEventArgs e)
        {
#if DEBUG
            if (Switches.GridDataBoundGrid.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(PaneDesc, e.Handled, e.KeyCode, e.Modifiers);
            }
#else
            ;
#endif
            if (!Capture && e.KeyCode == Keys.Escape && WantEscapeKey)
            {
                CancelUpdate();
                BeginUpdate(BeginUpdateOptions.InvalidateAndScroll);
                try
                {
                    CancelCurrent();
                    Model.Selections.Clear(true);
                    Model.ResetVolatileData();
                    InvalidateRange(GridRangeInfo.Row(CurrentCell.RowIndex), GridRangeOptions.MergeCoveredCells);
                    e.Handled = true;
                }
                finally
                {
                    EndUpdate();
                    Focus();
                    FixCurrentCellGotFocus();
                }

                return;
            }
            else if (e.KeyCode == Keys.Delete)
            {
                GridRangeInfo range = Selections.Ranges.ActiveRange;
                if (range.IsRows || range.IsTable)
                {
                    if (Model.Options.ListBoxSelectionMode != SelectionMode.None
                        && CurrentCell.IsEditing
                        && (Model.Options.ActivateCurrentCellBehavior & GridCellActivateAction.SetCurrent) != 0)
                    {
                        return;
                    }

                    if (range.IsRows)
                    {
                        DeleteRecordsAtRowIndex(range.Top, range.Bottom);
                    }
                    else if (range.IsTable)
                    {
                        DeleteRecordsAtRowIndex(Binder.ListManagerPositionToRowIndex(0), Binder.ListManagerPositionToRowIndex(Binder.RecordCount - 1));
                    }

                    e.Handled = true;
                }
                else
                {
                    if (ShouldDeleteKeyClearCells())
                    {
                        bool restoreDirectSaveCellInfo = binder.DirectSaveCellInfo;

                        try
                        {
                            bool multiRow = Model.SelectedRanges.Count > 1 || Model.SelectedRanges.ActiveRange.Height > 1;
                            if (multiRow)
                            {
                                Model.ConfirmChanges();
                                binder.EndEdit();
                                binder.DirectSaveCellInfo = multiRow;
                            }

                            Model.ClearCells(Model.SelectedRanges, false);
                        }
                        finally
                        {
                            binder.DirectSaveCellInfo = restoreDirectSaveCellInfo;
                        }

                        e.Handled = true;
                    }
                }
            }
        }

        /// <summary>
        /// Shoulds the delete key clear cells.
        /// </summary>
        /// <returns>returns boolean value</returns>
        /// <internalonly/>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected virtual bool ShouldDeleteKeyClearCells()
        {
            if (!Model.Options.ExcelLikeCurrentCell)
            {
                return Model.SelectedRanges.Count > 0;
            }
            else
            {
                return
                    Model.SelectedRanges.Count > 1 ||
                    (Model.SelectedRanges.Count == 1 && !CurrentCell.RangeInfo.Contains(Model.SelectedRanges[0]));
            }
        }

        /// <summary>
        /// Removes the records at the specified rows from the datasource. Before the rows are deleted,
        /// a cancelable <see cref="GridDataBoundGrid.RowsDeleting"/> event is raised. After the operation
        /// is complete, a <see cref="GridDataBoundGrid.RowsDeleted"/> event is raised.
        /// </summary>
        /// <param name="top">The first row to delete.</param>
        /// <param name="bottom">The last row to delete.</param>
        public void DeleteRecordsAtRowIndex(int top, int bottom)
        {
            if (!binder.AllowRemove || Model.ReadOnly || !binder.EnableRemove)
            {
                return;
            }

            GridRowRangeEventArgs e = new GridRowRangeEventArgs(top, bottom);
            this.OnRowsDeleting(e);
            if (!e.Cancel)
            {
                int pos = binder.CurrentPosition;
                Selections.Clear(true);
                BeginUpdate(BeginUpdateOptions.InvalidateAndScroll);
                try
                {
                    bool done = false;
                    CancelCurrent();
                    int first = binder.PositionToListManagerPosition(binder.RowIndexToPosition(top));
                    int last = Math.Min(binder.listManager != null ? binder.listManager.Count - 1 : binder.List.Count - 1, binder.PositionToListManagerPosition(binder.RowIndexToPosition(bottom)));

                    if (last < 0)
                    {
                        return;
                    }

                    if (binder.IsEditing && first <= binder.CurrentPosition && last >= binder.CurrentPosition)
                    {
                        if (binder.IsAddNew)
                        {
                            done = last == first;
                            last--;
                        }

                        binder.CancelEdit();
                    }

                    if (pos >= first && pos <= last)
                    {
                        if (first > 0 && binder.BindToCurrencyManager)
                        {
                            binder.listManager.Position = first - 1;
                        }
                    }

                    if (!done)
                    {
                        binder.RemoveRecords(first, last);
                    }

                    Selections.Clear();
                    CurrentCell.Reactivate();
                }
                finally
                {
                    EndUpdate();
                    FixCurrentCellGotFocus();
                }

                this.OnRowsDeleted(e);
            }
        }

        void CancelCurrent()
        {
            if (CurrentCell.HasCurrentCell)
            {
                if (!CurrentCell.IsModified)
                {
                    binder.CancelEdit();
                }
                else
                {
                    CurrentCell.RejectChanges();
                }

                CurrentCell.CancelEdit();
                if (!Updating)
                {
                    FixCurrentCellGotFocus();
                }
            }
        }

        // Support for RecordNavigationBar
        //        int IRecordNavigationBarData.MinRecord
        //        {
        //            get { return 1; }
        //        }
        //
        //        int IRecordNavigationBarData.MaxRecord
        //        {
        //            get { return (int) binder.RowIndexToPosition(Model.RowCount); }
        //        }
        //
        //        bool IRecordNavigationBarData.AllowAddNew
        //        {
        //            get { return binder.SupportsAddNew; }
        //        }

        /// <override/>
        protected override void OnGridValidating(CancelEventArgs e)
        {
#if DEBUG
            if (Switches.GridDataBoundGrid.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(binder.IsEditing, PaneDesc);
            }
#else
            ;
#endif
            try
            {
                if (e.Cancel || this.IsSplitterPaneClosing)
                {
                    return;
                }
#if DEBUG
                if (Switches.GridDataBoundGrid.TraceVerbose)
                {
                    TraceUtil.TraceCurrentMethodInfo(PaneDesc);
                }
#else
                ;
#endif

                if (!e.Cancel && binder.IsEditing)
                {
                    bool modified = CurrentCell.IsModified;
                    try
                    {
                        bool validateError = false;
                        Model.BeginUpdate(BeginUpdateOptions.InvalidateAndScroll);
                        e.Cancel = validateError = !CurrentCell.ConfirmChanges() || !EndEdit();
                        Model.EndUpdate(!validateError);
                        if (validateError)
                        {
                            this.RaiseValidateFailed(CurrentCell.ColIndex);
                            if (CurrentCell.HasCurrentCell)
                            {
                                CurrentCell.IsModified = modified;
                            }

                            e.Cancel = true;
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
                        EndUpdate(true);
                        CurrentCell.ErrorMessage = ex.Message;
                        CurrentCell.Exception = ex;
                        this.RaiseValidateFailed(CurrentCell.ColIndex);
                        return;
                    }
                    finally
                    {
                    }
                }
            }
            finally
            {
            }
        }

        /// <override/>
        protected override void OnResizingColumns(GridResizingColumnsEventArgs e)
        {
#if DEBUG
            if (Switches.GridDataBoundGrid.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(this.PaneDesc);
            }
#else
            ;
#endif
            if (AllowProportionalColumnSizing)
            {
                if (e.Reason == Syncfusion.Windows.Forms.Grid.GridResizeCellsReason.MouseUp)
                {
                    string name = this.binder.InternalColumns[e.Columns.Left - 1].MappingName;
                    if (!sizedColumns.ContainsKey(name))
                    {
                        if (comparingClientSize(name, e.Width, this.binder.InternalColumns[name].Width))
                            e.Cancel = true;
                    }
                    else
                    {
                        if (comparingClientSize(name, e.Width, this.binder.InternalColumns[name].Width))
                            e.Cancel = true;
                    }

                }
                else if (e.Reason == Syncfusion.Windows.Forms.Grid.GridResizeCellsReason.DoubleClick)
                {
                    this.Model.ColWidths[e.Columns.Left] = 5; //set it to some minimun and then size up to optimal size
                    this.Model.ColWidths.ResizeToFit(GridRangeInfo.Col(e.Columns.Left));
                    int size = this.Model.ColWidths[e.Columns.Left];
                    e.Cancel = true; //we handled it...
                    string name = this.binder.InternalColumns[e.Columns.Left - 1].MappingName;
                    if (!sizedColumns.ContainsKey(name))
                    {
                        if (comparingClientSize(name, size, this.binder.InternalColumns[name].Width))
                            e.Cancel = true;
                    }
                    else
                    {
                        if (comparingClientSize(name, size, this.binder.InternalColumns[name].Width))
                            e.Cancel = true;
                    }
                }
            }
            else
            {
                if (e.Reason == GridResizeCellsReason.DoubleClick)
                {
                    if (!this.UnHideColsOnDblClick && (this.Model.HideCols[e.Columns.Left + 1] || this.Model.ColWidths[e.Columns.Left + 1] == 0))
                    {
                        e.Cancel = true;
                    }

                    else
                    {
                        this.Model.ColWidths.ResizeToFit(GridRangeInfo.Col(e.Columns.Left));
                        e.Cancel=true;
                    }
                }
            }
            base.OnResizingColumns(e);
        }
        /// <override/>
        protected override void OnClientSizeChanged(System.EventArgs e)
        {
            if (this.AllowProportionalColumnSizing)
                this.SetColumnWidthsforGDBG();
            base.OnClientSizeChanged(e);
        }
        /// <summary>
        /// compare the grid client size to change the column width with new column width allow resizing to fit.
        /// </summary>
        /// <param name="colName">Column name </param>
        /// <param name="newColWidth"> new column with</param>
        /// <param name="oldColWidth">column width before change the size of column</param>
        /// <returns>returns true value.</returns>
        private bool comparingClientSize(string colName, int newColWidth, int oldColWidth)
        {
            GridBoundColumn gbcolumn = this.binder.InternalColumns[colName];
            int colIndex = this.binder.InternalColumns.IndexOf(gbcolumn);
            int clientWidth = this.ClientSize.Width;
            int vColCount = this.binder.InternalColumns.Count;
            int frozenCount = this.InternalGetFrozenCols();
            int frozen = this.Model.ColWidths.GetTotal(0, frozenCount);
            int resizedRightColWidth = 0;
            string name = string.Empty;
            int rightColWidth = 0;
            int resizedLeftColWidth = 0;
            int leftColWidth = 0;
            int rightColCount = 0;
            int rightResizedColCount = 0;
            int leftColCount = 0;
            for (int i = colIndex + 1; i < vColCount; i++)
            {
                name = this.binder.InternalColumns[i].MappingName;
                if (sizedColumns.ContainsKey(name))
                {
                    resizedRightColWidth += sizedColumns[name];
                    rightResizedColCount++;
                }
                else
                    rightColWidth += this.binder.InternalColumns[name].Width;
                rightColCount++;
            }
            for (int i = 0; i < colIndex; i++)
            {
                name = this.binder.InternalColumns[i].MappingName;
                if (sizedColumns.ContainsKey(name))
                {
                    resizedLeftColWidth += sizedColumns[name];
                }
                else
                    leftColWidth += this.binder.InternalColumns[name].Width;
                leftColCount++;
            }

            int tot = clientWidth - leftColWidth - resizedLeftColWidth - newColWidth - frozen - resizedRightColWidth;
            if (tot > 0 && rightColCount > 0)
            {
                if (sizedColumns.ContainsKey(colName))
                {
                    sizedColumns.Remove(colName);
                }
                sizedColumns.Add(colName, newColWidth);

                int dx = tot / (rightColCount - rightResizedColCount);

                for (int i = colIndex; i < vColCount; i++)
                {
                    name = this.binder.InternalColumns[i].MappingName;
                    if (!sizedColumns.ContainsKey(name))
                        this.binder.InternalColumns[name].Width = dx;
                    else
                        this.binder.InternalColumns[name].Width = sizedColumns[name];
                }
                return false;
            }
            else
                return true;
        }
        /// <summary>
        /// set the column width when AllowProportionalColumnSizing  is enable
        /// </summary>
        private void SetColumnWidthsforGDBG()
        {
            int width = this.ClientSize.Width;
            int count = this.binder.InternalColumns.Count;
            if (count > 0 && count > sizedColumns.Count)
            {
                int frozenCount = this.InternalGetFrozenCols();
                int frozen = this.Model.ColWidths.GetTotal(0, frozenCount);
                int fixedSize = 0;
                foreach (string name1 in sizedColumns.Keys)
                {
                    fixedSize += sizedColumns[name1];
                }
                int dx = (width - frozen - fixedSize) / (count - sizedColumns.Count);
                string name = "";
                int addedWidth = 0;
                for (int i = 0; i < count - 1; ++i)
                {
                    name = this.binder.InternalColumns[i].MappingName;
                    if (!sizedColumns.ContainsKey(name))
                    {
                        //this.binder.GridBoundColumns[name].Width = dx;
                        this.binder.InternalColumns[name].Width = dx;
                        addedWidth += dx;
                    }
                }
                name = this.binder.InternalColumns[count - 1].MappingName;
                this.binder.InternalColumns[name].Width = width - frozen - fixedSize - addedWidth; //add all the roundoff pixels to the last column
            }
        }
        /// <override/>
        protected override void OnVisibleChanged(System.EventArgs e)
        {
#if DEBUG
            if (Switches.GridDataBoundGrid.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(PaneDesc, initCC, firstShown);
            }
#else
            ;
#endif
            if (Visible && !Disposing && initCC && firstShown)
            {
                Model.ResetVolatileData();
                forceDataSource = true;
                if (binder.DataSource == null && this._dataSource != null)
                {
                    this.DataSource = _dataSource;
                }

                this._dataSource = null;
                ResizeVisibleRowsToFit();
            }

            if (Visible)
            {
                firstShown = false;
            }
            if (AllowProportionalColumnSizing)
            {
                SetColumnWidthsforGDBG();
            }
            base.OnVisibleChanged(e);
        }

        bool resizeOnUpdatingChanged = false;

        void ResizeVisibleRowsToFit()
        {
            ResizeVisibleRowsToFit(false);
        }

        void ResizeVisibleRowsToFit(bool force)
        {
            if (this.allowResizeToFit)
            {
                if (Updating)
                {
                    resizeOnUpdatingChanged = true;
                }
                else
                {
                    Model.ColWidths.ResizeToFit(GridRangeInfo.Rows(Model.Rows.HeaderCount + 1, ViewLayout.LastVisibleRow), GridResizeToFitOptions.IncludeHeaders);
                }
            }
        }

        /// <override/>
        protected override void OnUpdatingChanged(EventArgs e)
        {
            base.OnUpdatingChanged(e);
            if (resizeOnUpdatingChanged && allowResizeToFit)
            {
                resizeOnUpdatingChanged = false;
                Model.ColWidths.ResizeToFit(GridRangeInfo.Rows(Model.Rows.HeaderCount + 1, ViewLayout.LastVisibleRow), GridResizeToFitOptions.IncludeHeaders);
            }

            resizeOnUpdatingChanged = false;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the column header for the current cell should be highlighted.
        /// </summary>
        /// <remarks>
        /// Highlighting the column header will help users identify quickly in which level they are in a hierarchy.
        /// <para/>
        /// If you want to customize the look of the highlighted header you should handle the <see cref="GridControlBase.PrepareViewStyleInfo"/>
        /// event and check <see cref="IsMarkedHeader"/> and set <see cref="CancelEventArgs.Cancel"/> to True.
        /// </remarks>
        [DefaultValue(false)]
        [Description("Specifies if the column header for the current cell should be highlighted.")]
        [RefreshProperties(RefreshProperties.All)]
        [Category("Grid")]
        public bool HighlightCurrentColumnHeader
        {
            get
            {
                EnsureBinder();
                return binder.outlineHeaderField;
            }

            set
            {
                EnsureBinder();
                binder.outlineHeaderField = value;
                this.markedHeaderRange = GridRangeInfo.Empty;
            }
        }

        /// <summary>
        /// Determines if the column header at the specified row and column index should be outlined.
        /// </summary>
        /// <param name="rowIndex">The row index.</param>
        /// <param name="colIndex">The column index.</param>
        /// <returns>True if header should be outlined; False otherwise.</returns>
        public bool IsMarkedHeader(int rowIndex, int colIndex)
        {
            return this.markedHeaderRange.Contains(GridRangeInfo.Cell(rowIndex, colIndex));
        }

        /// <override/>
        protected override void OnPrepareViewStyleInfo(GridPrepareViewStyleInfoEventArgs e)
        {
            base.OnPrepareViewStyleInfo(e);

            if (!e.Cancel && !CurrentCell.StaticDrawing && !PrintingMode && IsMarkedHeader(e.RowIndex, e.ColIndex))
            {
                ////e.Style.CellAppearance = GridCellAppearance.Flat;
                e.Style.BackColor = SystemColors.Highlight;
                e.Style.TextColor = SystemColors.HighlightText;
                ////e.Style.Borders.All = new GridBorder(GridBorderStyle.Dashed);
            }
        }

        /// <copyfrom cref="GridModelOptions.ControllerOptionsChanged"/><summary>See <see cref="GridModelOptions.ControllerOptionsChanged"/> in the GridModel class for information.</summary>
        [Editor(typeof(Syncfusion.Windows.Forms.Design.EnumFlagsEditor), typeof(UITypeEditor)),
        DefaultValue(GridControllerOptions.All)]
        [Description("Specifies which mouse controllers should be enabled for the grid.")]
        [RefreshProperties(RefreshProperties.All)]
        [Category("Grid")]
        public GridControllerOptions ControllerOptions
        {
            get
            {
                return Model.Options.ControllerOptions;
            }

            set
            {
                EnsureBinder();
                Model.Options.ControllerOptions = value;
            }
        }

        /// <copyfrom cref="GridModel.BaseStylesMap"/><summary>See <see cref="GridModel.BaseStylesMap"/> in the GridModel class for information.</summary>
        [Browsable(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Description("The collection of base styles used in this grid.")]
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Grid")]
        public GridBaseStylesMap BaseStylesMap
        {
            get
            {
                return Model.BaseStylesMap;
            }

            set
            {
                EnsureBinder();
                Model.BaseStylesMap = value;
            }
        }

        bool ShouldSerializeBaseStylesMap()
        {
            return BaseStylesMap.Modified;
        }

        /// <summary>
        /// Resets the <see cref="BaseStylesMap"/> property.
        /// </summary>
        public void ResetBaseStylesMap()
        {
            if (!BaseStylesMap.InCollectionEditor)
            {
                Binder.ResetBaseStyles();
                Refresh();
            }
        }

        /// <copyfrom cref="GridModelOptions.ActivateCurrentCellBehavior"/><summary>See <see cref="GridModelOptions.ActivateCurrentCellBehavior"/> in the GridModel class for information.</summary>
        [Browsable(true),
        DefaultValue(GridCellActivateAction.ClickOnCell)]
        [Description("Specifies current cell activation behavior when moving the current cell or clicking inside a cell.")]
        [Category("Grid")]
        public GridCellActivateAction ActivateCurrentCellBehavior
        {
            get
            {
                return Model.Options.ActivateCurrentCellBehavior;
            }

            set
            {
                EnsureBinder();
                Model.Options.ActivateCurrentCellBehavior = value;
            }
        }

        /// <copyfrom cref="GridModelOptions.AllowDragSelectedCols"/><summary>See <see cref="GridModelOptions.AllowDragSelectedCols"/> in the GridModel class for information.</summary>
        [Browsable(true),
        DefaultValue(false)]
        [Description("Allow the user to drag selected columns by clicking on the column header.")]
        [Category("Grid")]
        public bool AllowDragSelectedCols
        {
            get
            {
                return Model.Options.AllowDragSelectedCols;
            }

            set
            {
                EnsureBinder();
                Model.Options.AllowDragSelectedCols = value;
            }
        }

        /// <copyfrom cref="GridModelOptions.AlphaBlendSelectionColor"/><summary>See <see cref="GridModelOptions.AlphaBlendSelectionColor"/> in the GridModel class for information.</summary>
        [Browsable(true)]
        [Description("Specifies the color for alpha blended cell selections.")]
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Grid")]
        public Color AlphaBlendSelectionColor
        {
            get
            {
                return Model.Options.AlphaBlendSelectionColor;
            }

            set
            {
                EnsureBinder();
                Model.Options.AlphaBlendSelectionColor = value;
            }
        }

        /// <summary>
        /// Resets the <see cref="AlphaBlendSelectionColor"/> property.
        /// </summary>
        public void ResetAlphaBlendSelectionColor()
        {
            AlphaBlendSelectionColor = SystemColors.Highlight;
        }

        bool ShouldSerializeAlphaBlendSelectionColor()
        {
            return Model.Options.AlphaBlendSelectionColor != Color.FromArgb(64, SystemColors.Highlight)
                && (Model.Options.AllowSelection & GridSelectionFlags.AlphaBlend) != 0;
        }

        /// <copyfrom cref="GridModelOptions.AllowSelection"/><summary>See <see cref="GridModelOptions.AllowSelection"/> in the GridModel class for information.</summary>
        [Browsable(true),
        Editor(typeof(Syncfusion.Windows.Forms.Design.EnumFlagsEditor), typeof(UITypeEditor)),
        RefreshProperties(RefreshProperties.Repaint),
        DefaultValue(GridSelectionFlags.Any)]
        [Description("Defines selection behavior of the grid.")]
        [Category("Grid")]
        public GridSelectionFlags AllowSelection
        {
            get
            {
                return Model.Options.AllowSelection;
            }

            set
            {
                EnsureBinder();
                Model.Options.AllowSelection = value;
            }
        }
        /// <summary>
        /// Get or Set of Skin Manager Interface
        /// </summary>
        private string style;
        string IVisualStyle.VisualTheme
        {
            get
            {
                return style;
            }
            set
            {
                style = value;
                this.Model.EnableLegacyStyle = false;
                switch (style)
                {
                    case "Office2007Blue":
                        GridVisualStyles = GridVisualStyles.Office2007Blue;
                        break;
                    case "Office2007Black":
                        GridVisualStyles = GridVisualStyles.Office2007Black;
                        break;
                    case "Office2007Silver":
                        GridVisualStyles = GridVisualStyles.Office2007Silver;
                        break;
                    case "Office2010Blue":
                        GridVisualStyles = GridVisualStyles.Office2010Blue;
                        break;
                    case "Office2010Black":
                        GridVisualStyles = GridVisualStyles.Office2010Black;
                        break;
                    case "Office2010Silver":
                        GridVisualStyles = GridVisualStyles.Office2010Silver;
                        break;
                }
            }
        }
        /// <summary>
        /// [Deprecated] Gets or sets the VisualStyles (skins) like Office2010, Office2007, Office2003
        /// </summary>
        [Browsable(true),
        DefaultValue(ColorStyles.SystemTheme)]
        [Description("[Deprecated] Specifies look and feel skins for the Grid")]
        [Category("Grid")]
        public ColorStyles ColorStyles
        {
            get
            {
                return colorStyles;
            }

            set
            {
                colorStyles = value;
                switch (value)
                {
                    case ColorStyles.Office2003:
                        this.GridVisualStyles = GridVisualStyles.Office2003;
                        break;
                    case ColorStyles.Office2007Blue:
                        this.GridVisualStyles = GridVisualStyles.Office2007Blue;
                        break;
                    case ColorStyles.Office2007Black:
                        this.GridVisualStyles = GridVisualStyles.Office2007Black;
                        break;
                    case ColorStyles.Office2007Silver:
                        this.GridVisualStyles = GridVisualStyles.Office2007Silver;
                        break;
                    case ColorStyles.Office2010Blue:
                        this.GridVisualStyles = GridVisualStyles.Office2010Blue;
                        break;
                    case ColorStyles.Office2010Black:
                        this.GridVisualStyles = GridVisualStyles.Office2010Black;
                        break;
                    case ColorStyles.Office2010Silver:
                        this.GridVisualStyles = GridVisualStyles.Office2010Silver;
                        break;
                }
            }
        }

        /// <summary>
        /// Gets or sets the VisualStyles (skins) like Office2010, Office2007, Office2003.
        /// </summary>
        [Browsable(true),
        DefaultValue(GridVisualStyles.SystemTheme)]
        [Description("Specifies look and feel skins for the Grid")]
        [Category("Grid")]
        public GridVisualStyles GridVisualStyles
        {
            get
            {
                return Model.Options.GridVisualStyles;
            }

            set
            {
                EnsureBinder();
                GridVisualStyles visualStyles = Model.Options.GridVisualStyles;
                Model.Options.GridVisualStyles = value;
                if (value == GridVisualStyles.Metro)
                {
                    if (!this.PersistAppearanceSettings)
                    {
                        this.DefaultRowHeight = 20;
                        GridStyleInfo.Default.BackColor = Color.White;
                        GridStyleInfo.Default.TextColor = Color.FromArgb(138, 138, 138);
                        GridStyleInfo.Default.Font.Facename = "Segoe UI";
                        GridStyleInfo.Default.Font.Size = 9F;
                        this.BaseStylesMap.ColumnHeader.StyleInfo.Font.Bold = true;
                        Model.Options.DefaultGridBorderStyle = GridBorderStyle.Solid;
                        Model.Properties.GridLineColor = Color.FromArgb(255, 234, 234, 234);
                        GridStyleInfo.Default.Borders.Bottom = new GridBorder(Model.Options.DefaultGridBorderStyle, Model.Properties.GridLineColor, GridBorderWeight.ExtraThin);
                        GridStyleInfo.Default.Borders.Right = new GridBorder(Model.Options.DefaultGridBorderStyle, Model.Properties.GridLineColor, GridBorderWeight.ExtraThin);
                        isMetroSettingsApplied = true;
                    }
                    this.GridOfficeScrollBars = OfficeScrollBars.Metro;
                }
                if (!this.Model.EnableLegacyStyle && value != GridVisualStyles.Metro)
                {
                    switch (value)
                    {
                        case GridVisualStyles.Office2007Blue:
                            this.GridOfficeScrollBars = OfficeScrollBars.Office2007;
                            this.Office2007ScrollBarsColorScheme = Office2007ColorScheme.Blue;
                            GridStyleInfo.Default.BackColor = Color.White;
                            GridStyleInfo.Default.TextColor = Color.DarkSlateGray;
                            GridStyleInfo.Default.Borders.Bottom = new GridBorder(GridBorderStyle.Solid, Color.FromArgb(234, 234, 234), GridBorderWeight.ExtraThin);
                            GridStyleInfo.Default.Borders.Right = new GridBorder(GridBorderStyle.Solid, Color.FromArgb(234, 234, 234), GridBorderWeight.ExtraThin);
                            break;

                        case GridVisualStyles.Office2007Black:
                            this.GridOfficeScrollBars = OfficeScrollBars.Office2007;
                            this.Office2007ScrollBarsColorScheme = Office2007ColorScheme.Black;
                            GridStyleInfo.Default.TextColor = SystemColors.InactiveCaptionText;
                            GridStyleInfo.Default.BackColor = Color.White;
                            GridStyleInfo.Default.Borders.Bottom = new GridBorder(GridBorderStyle.Solid, Color.FromArgb(205, 200, 177), GridBorderWeight.ExtraThin);
                            GridStyleInfo.Default.Borders.Right = new GridBorder(GridBorderStyle.Solid, Color.FromArgb(205, 200, 177), GridBorderWeight.ExtraThin);
                            break;

                        case GridVisualStyles.Office2007Silver:
                            this.GridOfficeScrollBars = OfficeScrollBars.Office2007;
                            this.Office2007ScrollBarsColorScheme = Office2007ColorScheme.Silver;
                            GridStyleInfo.Default.BackColor = Color.FromArgb(252, 252, 252);
                            GridStyleInfo.Default.TextColor = Color.FromArgb(51, 51, 51);
                            GridStyleInfo.Default.Borders.Bottom = new GridBorder(GridBorderStyle.Solid, Color.FromArgb(234, 234, 234), GridBorderWeight.ExtraThin);
                            GridStyleInfo.Default.Borders.Right = new GridBorder(GridBorderStyle.Solid, Color.FromArgb(234, 234, 234), GridBorderWeight.ExtraThin);
                            break;

                        case GridVisualStyles.Office2010Blue:
                            this.GridOfficeScrollBars = OfficeScrollBars.Office2010;
                            this.Office2010ScrollBarsColorScheme = Office2010ColorScheme.Blue;
                            GridStyleInfo.Default.BackColor = Color.White;
                            GridStyleInfo.Default.TextColor = Color.DarkSlateGray;
                            GridStyleInfo.Default.Borders.Bottom = new GridBorder(GridBorderStyle.Solid, Color.FromArgb(234, 234, 234), GridBorderWeight.ExtraThin);
                            GridStyleInfo.Default.Borders.Right = new GridBorder(GridBorderStyle.Solid, Color.FromArgb(234, 234, 234), GridBorderWeight.ExtraThin);
                            break;

                        case GridVisualStyles.Office2010Black:
                            this.GridOfficeScrollBars = OfficeScrollBars.Office2010;
                            this.Office2010ScrollBarsColorScheme = Office2010ColorScheme.Black;
                            this.BaseStylesMap.ColumnHeader.StyleInfo.TextColor = Color.White;
                            GridStyleInfo.Default.TextColor = SystemColors.InactiveCaptionText;
                            GridStyleInfo.Default.BackColor = Color.White;
                            GridStyleInfo.Default.Borders.Bottom = new GridBorder(GridBorderStyle.Solid, Color.FromArgb(205, 200, 177), GridBorderWeight.ExtraThin);
                            GridStyleInfo.Default.Borders.Right = new GridBorder(GridBorderStyle.Solid, Color.FromArgb(205, 200, 177), GridBorderWeight.ExtraThin);
                            break;

                        case GridVisualStyles.Office2010Silver:
                            this.GridOfficeScrollBars = OfficeScrollBars.Office2010;
                            this.Office2010ScrollBarsColorScheme = Office2010ColorScheme.Silver;
                            GridStyleInfo.Default.BackColor = Color.FromArgb(252, 252, 252);
                            GridStyleInfo.Default.TextColor = Color.FromArgb(51, 51, 51);
                            GridStyleInfo.Default.Borders.Bottom = new GridBorder(GridBorderStyle.Solid, Color.FromArgb(234, 234, 234), GridBorderWeight.ExtraThin);
                            GridStyleInfo.Default.Borders.Right = new GridBorder(GridBorderStyle.Solid, Color.FromArgb(234, 234, 234), GridBorderWeight.ExtraThin);
                            break;
                        default:
                            this.GridOfficeScrollBars = OfficeScrollBars.None;
                            GridStyleInfo.Default.BackColor = Color.White;
                            GridStyleInfo.Default.TextColor = Color.Black;
                            GridStyleInfo.Default.Borders.Bottom = new GridBorder(GridBorderStyle.Standard, SystemColors.Control);
                            GridStyleInfo.Default.Borders.Right = new GridBorder(GridBorderStyle.Standard, SystemColors.Control);
                            break;
                    }
                    if (isMetroSettingsApplied)
                    {
                        GridStyleInfo.Default.Font.Size = 8.25f;
                        Model.Options.DefaultGridBorderStyle = GridBorderStyle.Dotted;
                        Model.Properties.GridLineColor = SystemColors.GrayText;
                        GridStyleInfo.Default.Borders.Bottom = new GridBorder(Model.Options.DefaultGridBorderStyle, Model.Properties.GridLineColor, GridBorderWeight.ExtraThin);
                        GridStyleInfo.Default.Borders.Right = new GridBorder(Model.Options.DefaultGridBorderStyle, Model.Properties.GridLineColor, GridBorderWeight.ExtraThin);
                        this.BaseStylesMap.ColumnHeader.StyleInfo.HorizontalAlignment = GridHorizontalAlignment.Center;
                        isMetroSettingsApplied = false;
                    }
                }
                else if (isMetroSettingsApplied && value != GridVisualStyles.Metro)
                {
                    GridStyleInfo.Default.Font.Size = 8.25f;
                    GridStyleInfo.Default.TextColor = (value == GridVisualStyles.Office2010Black) ? Color.White : SystemColors.WindowText;
                    this.DefaultRowHeight = 17;
                    isMetroSettingsApplied = false;
                }
                if (this.DpiAware)
                {
                    this.DefaultRowHeight = base.RowHeightOnScaling();
                    this.Model.RowHeights[0] = this.DefaultRowHeight + 8;//Header padding value is increased by 8 regardless of normal rows.
                }
            }
        }        

        /// <summary>
        /// set the colors of metro theme for Grid
        /// </summary>
        /// <param name="metroColors">Collection of metro color</param>
        public void SetMetroStyle(GridMetroColors metroColors)
        {
            this.Model.Options.SetMetroStyles(metroColors);
            this.GridVisualStyles = GridVisualStyles.Metro;
        }
        
        /// <summary>
        /// Gets or sets the VisualStylesDrawing object
        /// </summary>
        [Browsable(false)]
        [Description("Gets or sets the VisualStylesDrawing object")]
        public IVisualStylesDrawing GridVisualStylesDrawing
        {
            get
            {
                return Model.Options.GridVisualStylesDrawing;
            }

            set
            {
                EnsureBinder();
                Model.Options.GridVisualStylesDrawing = value;
            }
        }

        /// <summary>
        ///   <para> Gets or sets the method in which items are selected in
        /// the <see cref="GridListControl" />
        /// .</para>
        /// </summary>
        [DefaultValue(SelectionMode.None),
        Description(@"Indicates if the list box is to be single-select, multi-select, or unselectable."),
        RefreshProperties(RefreshProperties.Repaint)]
        [Category("Grid")]
        public virtual SelectionMode ListBoxSelectionMode
        {
            get
            {
                return Model.Options.ListBoxSelectionMode;
            }

            set
            {
                EnsureBinder();
                Model.Options.ListBoxSelectionMode = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to control the kind of textbox control that is created for TextBox cells. 
        /// In general the original text box behaves better than the richtext box with Hebrew and arabic languages.
        /// By default the grid uses the RichTextBox control for cell editing, but if you set
        /// UseRightToLeftCompatibleTextBox to true then the grid will do editing with original TextBox controls
        /// instead.
        /// </summary>
        [Browsable(true),
        DefaultValue(false)]
        [Description("Controls the kind of textbox control that is created for TextBox cells. In general the original text box behaves better than the default richtext box with Hebrew and arabic languages")]
        [Category("Grid")]
        public bool UseRightToLeftCompatibleTextBox
        {
            get
            {
                return Model.Options.UseRightToLeftCompatibleTextBox;
            }

            set
            {
                EnsureBinder();
                Model.Options.UseRightToLeftCompatibleTextBox = value;
            }
        }
        private bool drawIndividualSpannedCellBorders = false;

        Dictionary<string, int> sizedColumns = null;

        /// <summary>
        /// Gets or sets a value indicating whether Individual borders need to be drawn for spanned cells when they get focus.
        /// Ensure UseRightToLeftCompatibleTextBox is set to False to ensure the borders are shown properly such that the actual textbox is not edited directly.
        /// </summary>
        [
        Browsable(false),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
        ]
        public bool DrawIndividualSpannedCellBorders
        {
            get
            {
                return this.drawIndividualSpannedCellBorders;
            }
            set
            {
                if (this.drawIndividualSpannedCellBorders != value)
                {
                    this.drawIndividualSpannedCellBorders = value;
                    this.Invalidate();
                }
            }
        }
        
        /// <copyfrom cref="GridModelOptions.ExcelLikeCurrentCell"/><summary>See <see cref="GridModelOptions.ExcelLikeCurrentCell"/> in the GridModel class for information.</summary>
        [Browsable(true),
        DefaultValue(false),
        RefreshProperties(RefreshProperties.Repaint)]
        [Description("Defines Excel-like current cell behavior. When the user moves the current cell out of a selected range, the range will be cleared.")]
        [Category("Grid")]
        public bool ExcelLikeCurrentCell
        {
            get
            {
                return Model.Options.ExcelLikeCurrentCell;
            }

            set
            {
                EnsureBinder();
                Model.Options.ExcelLikeCurrentCell = value;
            }
        }

        /// <copyfrom cref="GridModelOptions.ExcelLikeSelectionFrame"/><summary>See <see cref="GridModelOptions.ExcelLikeSelectionFrame"/> in the GridModel class for information.</summary>
        [Browsable(true),
        DefaultValue(false),
        RefreshProperties(RefreshProperties.Repaint)]
        [Description("Specifies whether the active selection should be outlined with a selection frame.")]
        [Category("Grid")]
        public bool ExcelLikeSelectionFrame
        {
            get
            {
                return Model.Options.ExcelLikeSelectionFrame;
            }

            set
            {
                EnsureBinder();
                Model.Options.ExcelLikeSelectionFrame = value;
            }
        }

        /// <override/>
        /// <summary>Specifies the background image used for the control.</summary>
        [Localizable(true),
        DefaultValue(null),
        Description(@"The background image used for the control."),
        Category(@"Appearance"),
        RefreshProperties(RefreshProperties.Repaint)]
        public override Image BackgroundImage
        {
            set
            {
                EnsureBinder();
                if (!GetStyle(ControlStyles.SupportsTransparentBackColor) && value != null)
                {
                    Model.Options.TransparentBackground = true;
                }

                base.BackgroundImage = value;
            }

            get
            {
                return base.BackgroundImage;
            }
        }

        private bool hasFont = false;
        /// <override/>
        /// <summary>Specifies the font used to display text in the control.</summary>
        [Description(@"The font used to display text in the control."),
        AmbientValue(null),
        Category(@"Appearance"),
        RefreshProperties(RefreshProperties.All)]
        public override Font Font
        {
            get
            {
                if (this.Parent != null && this.Parent.Font.Size != TableStyle.GdipFont.Size && !hasFont)
                {
                    return this.IsSplitterPaneClosing ? base.Font : this.Parent.Font;
                }
                return this.IsSplitterPaneClosing ? base.Font : TableStyle.GdipFont;
            }

            set
            {
                if (value != null)
                {
                    hasFont = true;
                    EnsureBinder();
                    GridFontInfo font = TableStyle.Font;
                    font.Facename = value.FontFamily.Name;
                    font.FontStyle = value.Style;
                    font.Size = value.SizeInPoints;
                }
            }
        }

        private bool ShouldSerializeFont()
        {
            return TableStyle.HasFont;
        }

        /// <copyfrom cref="GridModel.TableStyle"/>
        /// <summary>Gets or sets the table style.</summary>
        [Browsable(true)]
        [Description("The table style. Individual cells will inherit attributes from the table style.")]
        [RefreshProperties(RefreshProperties.All)]
        [Category("Grid")]
        public GridStyleInfo TableStyle
        {
            get
            {
                return Model.TableStyle;
            }

            set
            {
                EnsureBinder();
                Model.TableStyle = value;
            }
        }

        bool ShouldSerializeTableStyle()
        {
            return !TableStyle.IsEmpty;
        }

        /// <summary>
        /// Resets the <see cref="TableStyle"/> property.
        /// </summary>
        public void ResetTableStyle()
        {
            TableStyle = new GridStyleInfo();
            Refresh();
        }

        /// <copyfrom cref="GridModelOptions.MinResizeRowSize"/><summary>See <see cref="GridModelOptions.MinResizeRowSize"/> in the GridModel class for information.</summary>
        [Browsable(true),
        DefaultValue(0)]
        [Description("Defines the minimum row height when the user resizes a row with the mouse.")]
        [Category("Grid")]
        public int MinResizeRowSize
        {
            get
            {
                return Model.Options.MinResizeRowSize;
            }

            set
            {
                EnsureBinder();
                Model.Options.MinResizeRowSize = value;
            }
        }

        /// <copyfrom cref="GridModelOptions.MinResizeColSize"/><summary>See <see cref="GridModelOptions.MinResizeColSize"/> in the GridModel class for information.</summary>
        [Browsable(true),
        DefaultValue(0)]
        [Description("Defines the minimum column width when the user resizes a column with the mouse.")]
        [Category("Grid")]
        public int MinResizeColSize
        {
            get
            {
                return Model.Options.MinResizeColSize;
            }

            set
            {
                EnsureBinder();
                Model.Options.MinResizeColSize = value;
            }
        }

        /// <copyfrom cref="GridModelOptions.SmoothControlResize"/><summary>See <see cref="GridModelOptions.SmoothControlResize"/> in the GridModel class for information.</summary>
        [Browsable(true),
        DefaultValue(true)]
        [Description("Defines whether a grid should be completely refreshed when the user resizes the window or if only newly visible rows or columns should be redrawn.")]
        [Category("Grid")]
        public bool SmoothControlResize
        {
            get
            {
                return Model.Options.SmoothControlResize;
            }

            set
            {
                EnsureBinder();
                Model.Options.SmoothControlResize = value;
            }
        }
        
        /// <copyfrom cref="GridModel.Properties"/><summary>See <see cref="GridModel.Properties"/> in the GridModel class for information.</summary>
        [Browsable(true),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Description("Manages more options for the grid. Printing related. Also manages colors for grid background, grid lines, and more.")]
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Grid")]
        public GridProperties Properties
        {
            get
            {
                return Model.Properties;
            }

            set
            {
                EnsureBinder();
                Model.Properties = value;
            }
        }

        bool ShouldSerializeProperties()
        {
            return Model.Properties.Modified;
        }

        /// <summary>
        /// Resets the <see cref="Properties"/> object to its default state.
        /// </summary>
        public void ResetProperties()
        {
            Model.Properties = new GridProperties();
        }

        /// <copyfrom cref="GridModelOptions.ResizeRowsBehavior"/><summary>See <see cref="GridModelOptions.ResizeRowsBehavior"/> in the GridModel class for information.</summary>
        [Browsable(true),
        Editor(typeof(Syncfusion.Windows.Forms.Design.EnumFlagsEditor), typeof(UITypeEditor)),
        DefaultValue(GridResizeCellsBehavior.ResizeSingle | GridResizeCellsBehavior.OutlineHeaders | GridResizeCellsBehavior.OutlineBounds)]
        [Description("Defines behavior for resizing rows.")]
        [Category("Grid")]
        public GridResizeCellsBehavior ResizeRowsBehavior
        {
            get
            {
                return Model.Options.ResizeRowsBehavior;
            }

            set
            {
                EnsureBinder();
                Model.Options.ResizeRowsBehavior = value;
            }
        }

        /// <copyfrom cref="GridModelOptions.ResizeColsBehavior"/><summary>See <see cref="GridModelOptions.ResizeColsBehavior"/> in the GridModel class for information.</summary>
        [Browsable(true),
        Editor(typeof(Syncfusion.Windows.Forms.Design.EnumFlagsEditor), typeof(UITypeEditor)),
        DefaultValue(GridResizeCellsBehavior.ResizeSingle | GridResizeCellsBehavior.OutlineHeaders | GridResizeCellsBehavior.OutlineBounds)]
        [Description("Defines behavior for resizing columns.")]
        [Category("Grid")]
        public GridResizeCellsBehavior ResizeColsBehavior
        {
            get
            {
                return Model.Options.ResizeColsBehavior;
            }

            set
            {
                EnsureBinder();
                Model.Options.ResizeColsBehavior = value;
            }
        }

        /// <copyfrom cref="GridModelOptions.ShowCurrentCellBorderBehavior"/><summary>See <see cref="GridModelOptions.ShowCurrentCellBorderBehavior"/> in the GridModel class for information.</summary>
        [Browsable(true),
        DefaultValue(GridShowCurrentCellBorder.WhenGridActive),
        RefreshProperties(RefreshProperties.Repaint)]
        [Description("Defines when to show current cell frame or border.")]
        [Category("Grid")]
        public GridShowCurrentCellBorder ShowCurrentCellBorderBehavior
        {
            get
            {
                return Model.Options.ShowCurrentCellBorderBehavior;
            }

            set
            {
                EnsureBinder();
                Model.Options.ShowCurrentCellBorderBehavior = value;
            }
        }

        /// <copyfrom cref="GridModelOptions.TransparentBackground"/><summary>See <see cref="GridModelOptions.TransparentBackground"/> in the GridModel class for information.</summary>
        [Browsable(true),
        DefaultValue(false),
        RefreshProperties(RefreshProperties.Repaint)]
        [Description("Defines whether grid should erase and fill background of cells or only draw cell text.")]
        [Category("Grid")]
        public bool TransparentBackground
        {
            get
            {
                return Model.Options.TransparentBackground;
            }

            set
            {
                EnsureBinder();
                Model.Options.TransparentBackground = value;
            }
        }

        /// <summary>
        /// get / set the bool value of UseComplexBinding
        /// </summary>
        [DefaultValue(false), Category("Grid"), Browsable(false)]
        public bool UseComplexBinding
        {
            get
            {
                return this.binder.UseComplexBinding;
            }
            set
            {
                this.binder.UseComplexBinding = value;
            }
        }

        /// <copyfrom cref="GridModel.this[int,int]"/><summary>See <see cref="GridModel.this[int,int]"/> in the GridModel class for information.</summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public GridStyleInfo this[int rowIndex, int colIndex]
        {
            get
            {
                return Model[rowIndex, colIndex];
            }

            set
            {
                Model[rowIndex, colIndex] = value;
            }
        }

        /// <summary>
        /// Returns the column index for a column that matches a given name.
        /// </summary>
        /// <param name="name">The name of the field to be matched.</param>
        /// <returns>The column index in the grid; -1 if not found.</returns>
        public int NameToColIndex(string name)
        {
            int fieldNum = binder.NameToField(name);
            if (fieldNum != -1)
            {
                return binder.FieldToColIndex(fieldNum);
            }

            return -1;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the grid supports removing records if the underlying datasource
        /// allows it. See <see cref="GridModelDataBinder.AllowRemove"/>.
        /// </summary>
        [DefaultValue(true)]
        [Browsable(true),
        RefreshProperties(RefreshProperties.Repaint)]
        [Description("A value indicating if the grid supports removing records if the underlying datasource allows it.")]
        [Category("Grid")]
        public bool EnableRemove
        {
            get
            {
                if (binder == null)
                {
                    return false;
                }

                return binder.EnableRemove;
            }

            set
            {
                EnsureBinder();
                binder.EnableRemove = value;
                Refresh();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the grid supports editing records if the underlying datasource
        /// allows editing records. See <see cref="GridModelDataBinder.AllowEdit"/>.
        /// </summary>
        [DefaultValue(true)]
        [Browsable(true),
        RefreshProperties(RefreshProperties.Repaint)]
        [Description("A value indicating if the grid supports editing records if the underlying datasource allows it.")]
        [Category("Grid")]
        public bool EnableEdit
        {
            get
            {
                if (binder == null)
                {
                    return false;
                }

                return binder.EnableEdit;
            }

            set
            {
                EnsureBinder();
                binder.EnableEdit = value;
                Refresh();
            }
        }
        
        /// <summary>
        /// Gets or sets a value indicating whether the grid supports adding new records if the underlying datasource
        /// allows adding new records. See <see cref="GridModelDataBinder.AllowAddNew"/>.
        /// </summary>
        [DefaultValue(true)]
        [Browsable(true),
        RefreshProperties(RefreshProperties.Repaint)]
        [Description("A value indicating if the grid supports adding new records if the underlying datasource allows it.")]
        [Category("Grid")]
        public bool EnableAddNew
        {
            get
            {
                if (binder == null)
                {
                    return false;
                }

                return binder.EnableAddNew;
            }

            set
            {
                EnsureBinder();
                binder.EnableAddNew = value;
                if (DesignMode)
                {
                    Model.ResetVolatileData();
                    if (binder.SupportsAddNew && value)
                    {
                        CurrentCell.MoveTo(Model.Rows.HeaderCount + 1, Model.Cols.HeaderCount + 1);
                    }
                }

                Refresh();
            }
        }

        internal GridDataBoundGridRecordAccessibleObject CreateRecordAccessibilityInstance(int index)
        {
            ////TraceUtil.TraceCurrentMethodInfo(index);
            return new GridDataBoundGridRecordAccessibleObject(this, index);
        }

        GridDataBoundGridRecordAccessibleObjectsIndexer recordAccessibleObjects = null;

        internal GridDataBoundGridRecordAccessibleObjectsIndexer RecordAccessibleObjects
        {
            get
            {
                if (recordAccessibleObjects == null)
                {
                    recordAccessibleObjects = new GridDataBoundGridRecordAccessibleObjectsIndexer(this);
                }

                return recordAccessibleObjects;
            }
        }

        internal GridDataBoundGridFieldHeaderAccessibleObject CreateFieldHeaderAccessibilityInstance(int index)
        {
            ////TraceUtil.TraceCurrentMethodInfo(index);
            return new GridDataBoundGridFieldHeaderAccessibleObject(this, index);
        }

        GridDataBoundGridFieldHeaderAccessibleObjectsIndexer colHeaderAccessibleObjects = null;

        internal GridDataBoundGridFieldHeaderAccessibleObjectsIndexer FieldHeaderAccessibleObjects
        {
            get
            {
                if (colHeaderAccessibleObjects == null)
                {
                    colHeaderAccessibleObjects = new GridDataBoundGridFieldHeaderAccessibleObjectsIndexer(this);
                }

                return colHeaderAccessibleObjects;
            }
        }
#if SyncfusionFramework4_0
        /// <summary>
        /// Assigns the new UIAProvider for Accessibility.
        /// </summary>
        [Description("Assigns the new UIAProvider for Accessibility.")]
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public GridUIAProvider Provider
        {
            get
            {
                return new GridUIAProvider(this);
            }
        }
#elif SyncfusionFramework3_5
        /// <summary>
        /// Assigns the new UIAProvider for Accessibility.
        /// </summary>
        [Description("Assigns the new UIAProvider for Accessibility.")]
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public GridUIAProvider Provider
        {
            get
            {
                return new GridUIAProvider(this);
            }
        }

#endif
        /// <override/>
        protected override void WndProc(ref Message msg)
        {
#if SyncfusionFramework4_0
            if (msg.Msg == 0x003D /*Wmsg_GETOBJECT*/ && this.AccessibilityEnabled)
            {
                msg.Result = AutomationInteropProvider.ReturnRawElementProvider(Handle, msg.WParam, msg.LParam, this.Provider);
                return;
            }
#elif SyncfusionFramework3_5
             if (msg.Msg == 0x003D /*Wmsg_GETOBJECT*/ && this.AccessibilityEnabled)
            {
                msg.Result = AutomationInteropProvider.ReturnRawElementProvider(Handle, msg.WParam, msg.LParam, this.Provider);
                return;
            }

#endif
            base.WndProc(ref msg);
        }

        /// <summary>
        /// Creates a new accessibility object for the control.
        /// </summary>
        /// <returns>
        /// A new <see cref="T:System.Windows.Forms.AccessibleObject"/> for the control.
        /// </returns>
        /// <override/>
        protected override AccessibleObject CreateAccessibilityInstance()
        {
            if (AccessibilityEnabled)
            {
                // Overridden to return the custom AccessibleObject
                // for the entire grid.
                return new GridDataBoundGridAccessibleObject(this);
            }

            return base.CreateAccessibilityInstance();
        }
    }

    /// <summary>
    /// Provides data for FieldChooserShowing event which occurs before the FieldChooser dialog 
    /// Showing in column header.
    /// </summary>
    /// 
    public class FieldChooserShowingEventArgs : SyncfusionCancelEventArgs
    {
        private string caption;
        private object fieldList;
        /// <summary>
        /// Constructor for FieldChooserShowingEventArgs
        /// </summary>
        /// <param name="caption"> Caption text of FieldChooser dialog. </param>
        /// <param name="fieldList"> Control of TreeViewAdv or GridControl. </param>
        public FieldChooserShowingEventArgs(string caption, object fieldList)
        {
            this.fieldList = fieldList;
            this.caption = caption;
        }
        /// <summary>
        /// Gets or Sets the value for FieldChooser dialog caption.
        /// </summary>
        public string Caption
        {
            get
            {
                return caption;
            }
            set
            {
                if (caption != value)
                    caption = value;
            }
        }
        /// <summary>
        /// Gets or Sets the value for TreeViewAdv or GridControl.
        /// </summary>
        public object FieldList
        {
            get
            {
                return fieldList;
            }
            set
            {
                if (fieldList != value)
                    fieldList = value;
            }
        }
    }
    /// <summary>
    /// Handles the FieldChooserShowing event.
    /// </summary>
    /// <param name="sender"> The object instance.</param>
    /// <param name="e"> The event data. </param>
    public delegate void FieldChooserShowingEventHandler(object sender, FieldChooserShowingEventArgs e);
    /// <summary>
    /// Provides data for FieldChooserShown event which occurs after the FieldChooser dialog 
    /// Shown in Column Header
    /// </summary>
    public class FieldChooserShownEventArgs : SyncfusionEventArgs
    {
        private object fieldList;
        private string caption;
        /// <summary>
        /// Constructor for FieldChooserShownEventArgs.
        /// </summary>
        /// <param name="caption"> Caption text of FieldChooser dialog. </param>
        /// <param name="fieldList"> Control of TreeviewAdv or GridControl. </param>
        public FieldChooserShownEventArgs(string caption, object fieldList)
        {
            this.caption = caption;
            this.fieldList = fieldList;
        }
        /// <summary>
        /// Gets the caption text of FieldChooser dialog.
        /// </summary>
        public string Caption
        {
            get
            {
                return caption;
            }
        }
        /// <summary>
        /// Gets the Value of TreeviewAdv or GridControl.
        /// </summary>
        public object FieldList
        {
            get
            {
                return fieldList;
            }
        }
    }
    /// <summary>
    /// Handles the FieldChooserShown event.
    /// </summary>
    /// <param name="sender"> The object instance. </param>
    /// <param name="e"> The event data. </param>
    public delegate void FieldChooserShownEventHandler(object sender, FieldChooserShownEventArgs e);
    /// <summary>
    /// Provides data for FieldChooserClosing event which occurs before the FieldChooser dialog 
    /// Closing in column header.
    /// </summary>
    public class FieldChooserClosingEventArgs : SyncfusionCancelEventArgs
    {
        private object fieldList;
        private string caption;
        /// <summary>
        /// Constructor for FieldChooserClosingEventArgs.
        /// </summary>
        /// <param name="caption"> Caption text of FieldChooser dialog. </param>
        /// <param name="fieldList"> Control of TreeviewAdv or GridControl. </param>
        public FieldChooserClosingEventArgs(string caption, object fieldList)
        {
            this.caption = caption;
            this.fieldList = fieldList;
        }
        /// <summary>
        /// Gets or Sets the value for FieldChooser dialog Caption.
        /// </summary>
        public string Caption
        {
            get
            {
                return caption;
            }
            set
            {
                if (caption != value)
                    caption = value;
            }
        }
        /// <summary>
        /// Gets or Sets Value for TreeViewAdv or GridControl.
        /// </summary>
        public object FieldList
        {
            get
            {
                return fieldList;
            }
            set
            {
                if (fieldList != value)
                    fieldList = value;
            }
        }
    }
    ///<summary>
    /// Handles the FieldChooserClosing event.
    /// </summary>
    /// <param name="sender"> The object instance. </param>
    /// <param name="e"> The event data. </param>
    public delegate void FieldChooserClosingEventHandler(object sender, FieldChooserClosingEventArgs e);
    /// <summary>
    /// Provides data for FieldChooserClosed event which occurs after the FieldChooser dialog 
    /// Closed in Column Header.
    /// </summary>
    public class FieldChooserClosedEventArgs : SyncfusionEventArgs
    {
        private object fieldList;
        private string caption;
        /// <summary>
        /// Constructor for FieldChooserClosedEventArgs.
        /// </summary>
        /// <param name="caption"> Caption text of FieldChooser dialog. </param>
        /// <param name="fieldList"> object of the fieldchooser. </param>
        public FieldChooserClosedEventArgs(string caption, object fieldList)
        {
            this.caption = caption;
            this.fieldList = fieldList;
        }
        /// <summary>
        /// Gets the value for Caption of FieldChooser dialog 
        /// </summary>
        public string Caption
        {
            get
            {
                return caption;
            }
        }
        /// <summary>
        /// Gets the value for TreeviewAdv or GridControl.
        /// </summary>
        public object FieldList
        {
            get
            {
                return fieldList;
            }
        }
    }
    /// <summary>
    /// Handles the FieldChooserClosed event.
    /// </summary>
    /// <param name="sender"> The object instance. </param>
    /// <param name="e"> The event data. </param>
    public delegate void FieldChooserClosedEventHandler(object sender, FieldChooserClosedEventArgs e);
    /// <summary>
    /// This is the <see cref="GridDataBoundGridModel"/> class that holds all data information about a <see cref="GridDataBoundGrid"/>.
    /// </summary>
    [Serializable]
    public class GridDataBoundGridModel : GridModel
    {
        /// <overload>
        /// Initializes a new instance of <see cref="GridDataBoundGridModel"/>.
        /// </overload>
        /// <summary>
        /// Initializes a new instance of <see cref="GridDataBoundGridModel"/>.
        /// </summary>
        public GridDataBoundGridModel()
            : this(typeof(GridModel))
        {
        }

        /// <summary>
        /// Initializes a new instance of <see cref="GridDataBoundGridModel"/>.
        /// </summary>
        /// <param name="type">Type information.</param>
        public GridDataBoundGridModel(Type type)
            : base(type)
        {
        }

        /// <summary>
        /// Initializes a new <see cref="GridDataBoundGridModel"/> from a serialization stream.
        /// </summary>
        /// <param name="info">An object that holds all the data needed to serialize or deserialize this instance.</param>
        /// <param name="context">Describes the source and destination of the serialized stream specified by info. </param>
        protected GridDataBoundGridModel(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        /// <override/>
        [SecurityPermissionAttribute(SecurityAction.Demand, SerializationFormatter = true)]
        protected override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
        }

        /// <summary>
        /// Gets or sets reference to the <see cref="GridModelDataBinder"/> that manages the underlying
        /// datasource.
        /// </summary>
        [Browsable(false)]
        public GridModelDataBinder Binder
        {
            get
            {
                return (GridModelDataBinder)DataProvider;
            }

            set
            {
                if (this.Binder != value)
                {
                    this.DataProvider = value;
                    OnBinderChanged(EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Occurs when the <see cref="Binder"/> has changed.
        /// </summary>
        public event EventHandler BinderChanged;

        /// <summary>
        /// Raises the <see cref="BinderChanged"/> event.
        /// </summary>
        /// <param name="e">A <see cref="EventArgs" /> that contains the event data.</param>
        protected virtual void OnBinderChanged(EventArgs e)
        {
            if (BinderChanged != null)
            {
                BinderChanged(this, e);
            }
        }

        /// <override/>
        protected /*internal*/ override void OnColsMoving(GridRangeMovingEventArgs e)
        {
            BeginUpdate(BeginUpdateOptions.InvalidateAndScroll);
            try
            {
                base.OnColsMoving(e);
                if (e.Cancel)
                {
                    return;
                }

                Binder.ModelColsMoving(this, e);
            }
            finally
            {
                EndUpdate();
            }
        }

        /// <override/>
        protected /*internal*/ override void OnClipboardPaste(GridCutPasteEventArgs e)
        {
            base.OnClipboardPaste(e);
            if (e.Handled || this.ActiveGridView == null)
            {
                return;
            }

            e.Handled = true;
            e.Result = false;

            if (this.ReadOnly || !this.Binder.EnableEdit)
            {
                return;
            }

            //// Verify that only one range or current cell is selected.
            GridRangeInfoList rangeList;
            if (!this.Selections.GetSelectedRanges(out rangeList, true))
            {
                return;
            }

            ////            if (rangeList.Count > 1)
            ////            {
            ////                // only one selected range is allowed
            ////                if (this.ActiveGridView != null)
            ////                    MessageBox.Show(FindFormHelper.FindForm(this.ActiveGridView), SR.GetString("Grid_IDM_PASTENOTMULTI"));
            ////                return;
            ////            }

            IDataObject iData = Clipboard.GetDataObject();
            int clipboardFlags = e.ClipboardFlags;
            e.Result = Binder.DataBoundPaste(iData, clipboardFlags, rangeList, e);
        }

        /// <override/>
        protected /*internal*/ override void OnClipboardCanPaste(GridCutPasteEventArgs e)
        {
            base.OnClipboardCanPaste(e);
            if (e.Handled)
            {
                return;
            }

            if (this.ReadOnly || !this.Binder.EnableEdit)
            {
                e.Result = false;
            }

            e.IgnoreCurrentCell = this.SelectedRanges.Count > 0;
        }

        bool inClipboardCut = false;

        /// <override/>
        protected /*internal*/ override void OnClipboardCut(GridCutPasteEventArgs e)
        {
            if (inClipboardCut)
            {
                e.IgnoreCurrentCell = this.SelectedRanges.Count > 0;
                return;
            }

            base.OnClipboardCut(e);
            if (e.Handled)
            {
                return;
            }

            bool restoreDirectSaveCellInfo = Binder.DirectSaveCellInfo;
            try
            {
                inClipboardCut = true;
                Binder.DirectSaveCellInfo = true;
                e.Handled = true;

                if (this.ReadOnly || !this.Binder.EnableEdit)
                {
                    return;
                }

                // this will trigger a recursive call to OnClipboardCut
                e.Result = this.CutPaste.Cut();
            }
            finally
            {
                Binder.DirectSaveCellInfo = restoreDirectSaveCellInfo;
                inClipboardCut = false;
            }
        }

        /// <override/>
        protected /*internal*/ override void OnClipboardCanCut(GridCutPasteEventArgs e)
        {
            base.OnClipboardCanCut(e);
            if (e.Handled)
            {
                return;
            }

            if (this.ReadOnly || !this.Binder.EnableEdit)
            {
                e.Result = false;
            }

            e.IgnoreCurrentCell = this.SelectedRanges.Count > 0;
        }

        /// <override/>
        protected /*internal*/ override void OnClipboardCopy(GridCutPasteEventArgs e)
        {
            base.OnClipboardCopy(e);
            if (e.Handled)
            {
                return;
            }

            e.IgnoreCurrentCell = this.SelectedRanges.Count > 0;
        }

        /// <override/>
        protected /*internal*/ override void OnClipboardCanCopy(GridCutPasteEventArgs e)
        {
            base.OnClipboardCanCopy(e);
            if (e.Handled)
            {
                return;
            }

            e.IgnoreCurrentCell = this.SelectedRanges.Count > 0;
        }

        bool inClearingCells = false;

        /// <override/>
        protected /*internal*/ override void OnClearingCells(GridClearingCellsEventArgs e)
        {
            if (inClearingCells)
            {
                e.ClearStyle = false;
                return;
            }

            base.OnClearingCells(e);
            if (e.Handled)
            {
                return;
            }

            bool restoreDirectSaveCellInfo = Binder.DirectSaveCellInfo;

            try
            {
                inClearingCells = true;
                Binder.DirectSaveCellInfo = true;
                e.Handled = true;

                if (this.ReadOnly || !this.Binder.EnableEdit)
                {
                    return;
                }

                if (this.ActiveGridView != null)
                {
                    if (e.RangeList.Count == 1 && e.RangeList[0].Equals(this.ActiveGridView.CurrentCell.RangeInfo))
                    {
                        this.Binder.BeginEdit();
                        Binder.DirectSaveCellInfo = false;
                    }
                    else if (e.RangeList.AnyRangeContains(this.ActiveGridView.CurrentCell.RangeInfo))
                    {
                        this.Binder.BeginEdit();
                    }
                }

                // this will trigger a recursive call to OnClearingCells
                e.Result = ClearCells(e.RangeList, e.ClearStyle);
            }
            finally
            {
                Binder.DirectSaveCellInfo = restoreDirectSaveCellInfo;
                inClearingCells = false;
            }
        }

        /// <override/>
        protected /*internal*/ override void OnQueryOleDataSourceData(GridQueryOleDataSourceDataEventArgs e)
        {
            base.OnQueryOleDataSourceData(e);
            if (e.Handled)
            {
                return;
            }

            e.DragDropFlags &= ~GridDragDropFlags.Styles;
            e.IgnoreCurrentCell = this.SelectedRanges.Count > 0;
        }
    }

    internal class GridDataBoundGridAccessibleObject : Control.ControlAccessibleObject
    {
        GridDataBoundGrid dataBoundGrid;

        public GridDataBoundGridAccessibleObject(GridDataBoundGrid owner)
            : base(owner)
        {
            this.dataBoundGrid = owner;
            this.dataBoundGrid.CurrentCellActivated += new EventHandler(dataBoundGrid_CurrentCellActivated);
        }

        // Gets the role for the grid. This is used by accessibility programs.
        public override AccessibleRole Role
        {
            get
            {
                return AccessibleRole.Table;
            }
        }

        public override /*AccessibleObject*/ string Name
        {
            get
            {
                return this.dataBoundGrid.AccessibleName;
            } // end of method get_Name
        }

        Rectangle cachedBounds = Rectangle.Empty;

        void getBounds()
        {
            cachedBounds = this.dataBoundGrid.RectangleToScreen(this.dataBoundGrid.ClientRectangle);
        }

        public override /*AccessibleObject*/ Rectangle Bounds
        {
            get
            {
                if (!this.dataBoundGrid.IsHandleCreated)
                {
                    return Rectangle.Empty;
                }

                if (this.dataBoundGrid.InvokeRequired)
                {
                    dataBoundGrid.Invoke(new MethodInvoker(getBounds));
                }
                else
                {
                    getBounds();
                }

                return cachedBounds;
            } // end of method get_Bounds
        }

        public override string Description
        {
            get
            {
                return this.dataBoundGrid.AccessibleDescription;
            }
        }

        public override string Help
        {
            get
            {
                return string.Empty;
            }
        }

        public override AccessibleObject Parent
        {
            get
            {
                return dataBoundGrid.AccessibilityObject;
            }
        }
        
        // Gets the state for the grid. This is used by accessibility programs.
        public override AccessibleStates State
        {
            get
            {
                AccessibleStates state = AccessibleStates.None;

                return state;
            }
        }

        // The grid objects are "child" controls in terms of accessibility so
        // return the number of ChartLengend objects.
        public override int GetChildCount()
        {
            return dataBoundGrid.binder.RecordCount + dataBoundGrid.binder.InternalColumns.Count;
        }

        // Gets the Accessibility object of the cell idetified by index.
        public override AccessibleObject GetChild(int index)
        {
            if (index < dataBoundGrid.binder.InternalColumns.Count)
            {
                return dataBoundGrid.FieldHeaderAccessibleObjects[index];
            }

            index -= dataBoundGrid.binder.InternalColumns.Count;
            if (index < dataBoundGrid.binder.RecordCount)
            {
                return dataBoundGrid.RecordAccessibleObjects[index];
            }

            return null;
        }

        public override string Value
        {
            get
            {
                return dataBoundGrid.Text;
            }

            set
            {
                dataBoundGrid.Text = value;
            }
        }

        // Helper function that is used by the GridDataBoundGridRecordAccessibleObject's accessibility object
        // to navigate between sibiling controls. Specifically, this function is used in
        // the GridDataBoundGridRecordAccessibleObject.Navigate function.
        internal AccessibleObject NavigateFromChild(AccessibleObject child, AccessibleNavigation navdir)
        {
            int index = -1;
            GridDataBoundGridRecordAccessibleObject rowAcc = child as GridDataBoundGridRecordAccessibleObject;
            if (rowAcc != null)
            {
                index = rowAcc.Index + dataBoundGrid.binder.InternalColumns.Count;
            }
            else
            {
                GridDataBoundGridFieldHeaderAccessibleObject colHeaderAcc = child as GridDataBoundGridFieldHeaderAccessibleObject;
                index = colHeaderAcc.Index;
            }

            switch (navdir)
            {
                case AccessibleNavigation.FirstChild:
                    index = 0;
                    break;

                case AccessibleNavigation.LastChild:
                    index = GetChildCount() - 1;
                    break;

                case AccessibleNavigation.Left:
                case AccessibleNavigation.Previous:
                case AccessibleNavigation.Up:
                    if (index > 0)
                    {
                        index--;
                    }

                    break;

                case AccessibleNavigation.Right:
                case AccessibleNavigation.Next:
                case AccessibleNavigation.Down:
                    if (index < GetChildCount())
                    {
                        index++;
                    }

                    break;
            }

            return this.GetChild(index);
        }

        //// Helper function that is used by the grid's accessibility object
        //// to select a specific grid control. Specifically, this function is used
        //// in the grid.gridAccessibleObject.Select function.
        internal void SelectChild(GridDataBoundGridRecordAccessibleObject child, AccessibleSelection selection)
        {
            int index = child.Index;

            ////To simulate a click AccessibleSelection.TakeFocus|AccessibleSelection.TakeSelection.
            ////To select a target item by simulating CTRL + click AccessibleSelection.TakeFocus|AccessibleSelection.AddSelection.
            ////To cancel selection of a target item by simulating CTRL + click AccessibleSelection.TakeFocus|AccessibleSelection.RemoveSelection.
            ////To simulate SHIFT + click AccessibleSelection.TakeFocus|AccessibleSelection.ExtendSelection.

            ////To select a range of objects and put focus on the last object Specify AccessibleSelection.TakeFocus on the starting object to set the selection anchor. Then call Select again and specify AccessibleSelection.TakeFocus|AccessibleSelection.ExtendSelection on the last object.
            ////To deselect all objects Specify AccessibleSelection.TakeSelection on any object. This flag deselects all selected objects except the one just selected. Then call Select again and specify AccessibleSelection.RemoveSelection on the same object.

            int rowIndex = dataBoundGrid.binder.ListManagerPositionToRowIndex(index);

            //// Determine which selection action should occur, based on the
            //// AccessibleSelection value.
            if ((selection & AccessibleSelection.TakeFocus) != 0)
            {
                if ((selection & AccessibleSelection.TakeSelection) != 0)
                {
                    this.dataBoundGrid.CurrentCell.MoveTo(rowIndex, this.dataBoundGrid.CurrentCell.ColIndex, GridSetCurrentCellOptions.ScrollInView);
                }

                if ((selection & AccessibleSelection.AddSelection) != 0)
                {
                    this.dataBoundGrid.Selections.Add(GridRangeInfo.Row(rowIndex));
                }

                if ((selection & AccessibleSelection.RemoveSelection) != 0)
                {
                    this.dataBoundGrid.Selections.Remove(GridRangeInfo.Row(rowIndex));
                }

                if ((selection & AccessibleSelection.ExtendSelection) != 0)
                {
                    int index1 = this.dataBoundGrid.CurrentCell.RowIndex;
                    this.dataBoundGrid.Selections.Add(GridRangeInfo.Rows(Math.Min(rowIndex, index1), Math.Max(rowIndex, index1)));
                }
            }
        }

        public override AccessibleObject GetFocused()
        {
            if (this.dataBoundGrid.Focused)
            {
                return GetSelected();
            }
            else
            {
                return base.GetFocused();
            }
        }

        public override AccessibleObject GetSelected()
        {
            if (dataBoundGrid.binder.CurrentPosition != -1)
            {
                return GetChild(dataBoundGrid.binder.CurrentPosition + dataBoundGrid.binder.InternalColumns.Count);
            }

            return base.GetSelected();
        }

        public override AccessibleObject HitTest(int x, int y)
        {
            Point point = dataBoundGrid.PointToClient(new Point(x, y));
            GridRangeInfo range = dataBoundGrid.PointToRangeInfo(point);
            if (!range.IsEmpty && range.Top > 0)
            {
                int record = dataBoundGrid.binder.RowIndexToListManagerPosition(range.Top);
                if (record >= 0)
                {
                    return dataBoundGrid.RecordAccessibleObjects[record];
                }
            }

            return base.HitTest(x, y);
        }

        public override AccessibleObject Navigate(AccessibleNavigation navdir)
        {
            GridDataBoundGridRecordAccessibleObject accObj = GetSelected() as GridDataBoundGridRecordAccessibleObject;
            if (accObj != null)
            {
                return this.NavigateFromChild(accObj, navdir);
            }

            return base.Navigate(navdir);
        }

        private void dataBoundGrid_CurrentCellActivated(object sender, EventArgs e)
        {
            int record = dataBoundGrid.Binder.RowIndexToListManagerPosition(dataBoundGrid.Binder.CurrentRowIndex);
            if (record >= 0)
            {
                int childId = record + this.dataBoundGrid.binder.InternalColumns.Count;
                NotifyClients(AccessibleEvents.Focus, childId);
                NotifyClients(AccessibleEvents.Selection, childId);
            }
        }
    }

    internal class GridDataBoundGridRecordAccessibleObject : AccessibleObject
    {
        GridDataBoundGrid dataBoundGrid;
        int index;

        public GridDataBoundGridRecordAccessibleObject(GridDataBoundGrid grid, int index)
        {
            this.dataBoundGrid = grid;
            this.index = index;
        }

        public int Index
        {
            get
            {
                return index;
            }
        }

        internal GridDataBoundGridAccessibleObject GridAccessibilityObject
        {
            get
            {
                return dataBoundGrid.AccessibilityObject as GridDataBoundGridAccessibleObject;
            }
        }

        public override /*AccessibleObject*/ void Select(AccessibleSelection flags)
        {
            if ((flags & AccessibleSelection.TakeFocus) != 0)
            {
                if (!dataBoundGrid.Focused)
                {
                    dataBoundGrid.Focus();
                }
            }

            if ((flags & AccessibleSelection.TakeSelection) != 0)
            {
                GridAccessibilityObject.SelectChild(this, flags);
            }
        } //// end of method Select

        /// <summary>
        /// Navigate to the next or previous grid entry.
        /// </summary>
        /// <param name="navdir">One of the <see cref="T:System.Windows.Forms.AccessibleNavigation"/> values.</param>
        /// <returns>
        /// An <see cref="T:System.Windows.Forms.AccessibleObject"/> that represents one of the <see cref="T:System.Windows.Forms.AccessibleNavigation"/> values.
        /// </returns>
        /// <exception cref="T:System.Runtime.InteropServices.COMException">
        /// The navigation attempt fails.
        /// </exception>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode"/>
        /// </PermissionSet>
        public override /*AccessibleObject*/ AccessibleObject Navigate(AccessibleNavigation navdir)
        {
            return GridAccessibilityObject.NavigateFromChild(this, navdir);
        } // end of method Navigate
        
        public override /*AccessibleObject*/ void DoDefaultAction()
        {
            this.Select((AccessibleSelection.TakeSelection | AccessibleSelection.TakeFocus));
        } // end of method DoDefaultAction

        /// <summary>
        /// Returns the currently focused child, if any.
        /// Returns this if the object itself is focused.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Windows.Forms.AccessibleObject"/> that specifies the currently focused child. This method returns the calling object if the object itself is focused. Returns null if no object has focus.
        /// </returns>
        /// <exception cref="T:System.Runtime.InteropServices.COMException">
        /// The control cannot be retrieved.
        /// </exception>
        public override /*AccessibleObject*/ AccessibleObject GetFocused()
        {
            return this.GridAccessibilityObject.GetFocused();
        } // end of method GetFocused

         public override /*AccessibleObject*/ AccessibleStates State
        {
            get
            {
                AccessibleStates accessibleStates = AccessibleStates.Selectable | AccessibleStates.Focusable;
                
                int rowIndex = dataBoundGrid.Binder.ListManagerPositionToRowIndex(Index);
                if (dataBoundGrid.Model.SelectedRanges.Contains(GridRangeInfo.Row(rowIndex)))
                {
                    accessibleStates |= AccessibleStates.Selected;
                }

                if (dataBoundGrid.Binder.CurrentPosition == index)
                {
                    accessibleStates |= AccessibleStates.Focused;
                }

                if (rowIndex < dataBoundGrid.TopRowIndex || rowIndex > dataBoundGrid.ViewLayout.LastVisibleRow)
                {
                    accessibleStates |= AccessibleStates.Offscreen | AccessibleStates.Invisible;
                }

                accessibleStates |= AccessibleStates.MultiSelectable;

                return accessibleStates;
            } // end of method get_State
        }

        public override /*AccessibleObject*/ AccessibleRole Role
        {
            get
            {
                return AccessibleRole.Row;
            } // end of method get_Role
        }

        public override /*AccessibleObject*/ AccessibleObject Parent
        {
            get
            {
                return dataBoundGrid.AccessibilityObject;
            } // end of method get_Parent
        }

        public override /*AccessibleObject*/ string Name
        {
            get
            {
                return "Record " + (Index + 1).ToString();
            } // end of method get_Name
        }

        public override /*AccessibleObject*/ string DefaultAction
        {
            get
            {
                return "Click";
            } // end of method get_DefaultAction
        }

        public override /*AccessibleObject*/ Rectangle Bounds
        {
            get
            {
                int rowIndex = dataBoundGrid.Binder.ListManagerPositionToRowIndex(Index);
                Rectangle r = dataBoundGrid.RangeInfoToRectangle(GridRangeInfo.Row(rowIndex), GridRangeOptions.MergeCoveredCells);
                r.Intersect(dataBoundGrid.ClientRectangle);
                return this.dataBoundGrid.RectangleToScreen(r);
            } // end of method get_Bounds
        }

        public override string Description
        {
            get
            {
                int rowIndex = dataBoundGrid.Binder.ListManagerPositionToRowIndex(Index);
                return "Record " + (Index + 1).ToString();
            }
        }
    }

    internal class GridDataBoundGridFieldHeaderAccessibleObject : AccessibleObject
    {
        GridDataBoundGrid dataBoundGrid;
        int index;

        public GridDataBoundGridFieldHeaderAccessibleObject(GridDataBoundGrid grid, int index)
        {
            this.dataBoundGrid = grid;
            this.index = index;
        }

        public int Index
        {
            get
            {
                return index;
            }
        }

        internal GridDataBoundGridAccessibleObject GridAccessibilityObject
        {
            get
            {
                return dataBoundGrid.AccessibilityObject as GridDataBoundGridAccessibleObject;
            }
        }

        public override /*AccessibleObject*/ void Select(AccessibleSelection flags)
        {
            if ((flags & AccessibleSelection.TakeFocus) != 0)
            {
                if (!dataBoundGrid.Focused)
                {
                    dataBoundGrid.Focus();
                }

                int rowNum;
                int fieldNum = dataBoundGrid.Binder.RootHierarchyLevel.FieldToRowField(index, out rowNum);
                int colIndex = dataBoundGrid.Binder.FieldToColIndex(fieldNum);
                dataBoundGrid.CurrentCell.MoveTo(dataBoundGrid.Binder.CurrentRowIndex + rowNum, colIndex, GridSetCurrentCellOptions.ScrollInView);
            }
        } // end of method Select

        /// <summary>
        /// Navigate to the next or previous grid entry.
        /// </summary>
        /// <param name="navdir">One of the <see cref="T:System.Windows.Forms.AccessibleNavigation"/> values.</param>
        /// <returns>
        /// An <see cref="T:System.Windows.Forms.AccessibleObject"/> that represents one of the <see cref="T:System.Windows.Forms.AccessibleNavigation"/> values.
        /// </returns>
        /// <exception cref="T:System.Runtime.InteropServices.COMException">
        /// The navigation attempt fails.
        /// </exception>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode"/>
        /// </PermissionSet>
        public override /*AccessibleObject*/ AccessibleObject Navigate(AccessibleNavigation navdir)
        {
            return GridAccessibilityObject.NavigateFromChild(this, navdir);
        } // end of method Navigate
        
        /// <summary>
        /// Performs the default action associated with this accessible object.
        /// </summary>
        /// <exception cref="T:System.Runtime.InteropServices.COMException">
        /// The default action for the control cannot be performed.
        /// </exception>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode"/>
        /// </PermissionSet>
        public override /*AccessibleObject*/ void DoDefaultAction()
        {
            this.Select((AccessibleSelection.TakeSelection | AccessibleSelection.TakeFocus));
        } // end of method DoDefaultAction

        /// <summary>
        /// Returns the currently focused child, if any.
        /// Returns this if the object itself is focused.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Windows.Forms.AccessibleObject"/> that specifies the currently focused child. This method returns the calling object if the object itself is focused. Returns null if no object has focus.
        /// </returns>
        /// <exception cref="T:System.Runtime.InteropServices.COMException">
        /// The control cannot be retrieved.
        /// </exception>
        public override /*AccessibleObject*/ AccessibleObject GetFocused()
        {
            return this.GridAccessibilityObject.GetFocused();
        } // end of method GetFocused

        public override /*AccessibleObject*/ AccessibleStates State
        {
            get
            {
                AccessibleStates accessibleStates = AccessibleStates.Selectable;
                int rowNum;

                if (GridUtil.IsEmpty(dataBoundGrid.Binder.RootHierarchyLevel.InternalColumns[index].MappingName))
                {
                    accessibleStates |= AccessibleStates.Invisible;
                }
                else
                {
                    int fieldNum = dataBoundGrid.Binder.RootHierarchyLevel.FieldToRowField(index, out rowNum);
                    int colIndex = dataBoundGrid.Binder.FieldToColIndex(fieldNum);

                    if (colIndex < dataBoundGrid.LeftColIndex || colIndex > dataBoundGrid.ViewLayout.LastVisibleCol)
                    {
                        accessibleStates |= AccessibleStates.Offscreen | AccessibleStates.Invisible;
                    }
                }

                return accessibleStates;
            } // end of method get_State
        }

        public override /*AccessibleObject*/ AccessibleRole Role
        {
            get
            {
                return AccessibleRole.ColumnHeader;
            } // end of method get_Role
        }

        public override /*AccessibleObject*/ AccessibleObject Parent
        {
            get
            {
                return dataBoundGrid.AccessibilityObject;
            } // end of method get_Parent
        }

        public override /*AccessibleObject*/ string Name
        {
            get
            {
                return this.dataBoundGrid.binder.InternalColumns[index].MappingName;
            } // end of method get_Name
        }

        public override /*AccessibleObject*/ string DefaultAction
        {
            get
            {
                return "Select";
            } // end of method get_DefaultAction
        }

        public override /*AccessibleObject*/ Rectangle Bounds
        {
            get
            {
                int rowNum;
                int fieldNum = dataBoundGrid.Binder.RootHierarchyLevel.FieldToRowField(index, out rowNum);
                int colIndex = dataBoundGrid.Binder.FieldToColIndex(fieldNum);
                if (colIndex <= 0)
                {
                    return Rectangle.Empty;
                }

                return this.dataBoundGrid.RectangleToScreen(dataBoundGrid.RangeInfoToRectangle(GridRangeInfo.Cell(rowNum, colIndex), GridRangeOptions.MergeCoveredCells));
            } // end of method get_Bounds
        }

        public override string Description
        {
            get
            {
                return this.dataBoundGrid.binder.InternalColumns[index].HeaderText;
            }
        }
    }

    internal class GridDataBoundGridRecordAccessibleObjectsIndexer
    {
        ArrayList data;
        GridDataBoundGrid dataBoundGrid;

        internal GridDataBoundGridRecordAccessibleObjectsIndexer(GridDataBoundGrid grid)
        {
            this.dataBoundGrid = grid;
            this.data = new ArrayList();
        }

        internal GridDataBoundGridRecordAccessibleObject GetItem(int index)
        {
            // Returns NULL if DataBoundGridRow is not found.
            return index >= 0 && index < data.Count ? data[index] as GridDataBoundGridRecordAccessibleObject : null;
        }

        internal void SetItem(int index, GridDataBoundGridRecordAccessibleObject accObj)
        {
            if (index >= data.Count)
            {
                object[] newItems = new object[index - data.Count + 1];
                data.AddRange(newItems);
            }

            data[index] = accObj;
        }

        internal void ResetItem(int index)
        {
            if (index < data.Count)
            {
                data[index] = null;
            }
        }

        [Browsable(false)]
        public GridDataBoundGridRecordAccessibleObject /*IGridData*/ this[int index]
        {
            get
            {
                GridDataBoundGridRecordAccessibleObject accObj = GetItem(index);
                if (accObj == null)
                {
                    accObj = dataBoundGrid.CreateRecordAccessibilityInstance(index);
                    SetItem(index, accObj);
                }

                return accObj;
            }
        }
    }

    internal class GridDataBoundGridFieldHeaderAccessibleObjectsIndexer
    {
        ArrayList data;
        GridDataBoundGrid dataBoundGrid;

        internal GridDataBoundGridFieldHeaderAccessibleObjectsIndexer(GridDataBoundGrid grid)
        {
            this.dataBoundGrid = grid;
            this.data = new ArrayList();
        }

        internal GridDataBoundGridFieldHeaderAccessibleObject GetItem(int index)
        {
            // Returns NULL if DataBoundGridRow is not found.
            return index < data.Count ? data[index] as GridDataBoundGridFieldHeaderAccessibleObject : null;
        }

        internal void SetItem(int index, GridDataBoundGridFieldHeaderAccessibleObject accObj)
        {
            if (index >= data.Count)
            {
                object[] newItems = new object[index - data.Count + 1];
                data.AddRange(newItems);
            }

            data[index] = accObj;
        }

        internal void ResetItem(int index)
        {
            if (index < data.Count)
            {
                data[index] = null;
            }
        }

        [Browsable(false)]
        public GridDataBoundGridFieldHeaderAccessibleObject /*IGridData*/ this[int index]
        {
            get
            {
                GridDataBoundGridFieldHeaderAccessibleObject accObj = GetItem(index);
                if (accObj == null)
                {
                    accObj = dataBoundGrid.CreateFieldHeaderAccessibilityInstance(index);
                    SetItem(index, accObj);
                }

                return accObj;
            }
        }
    }
    
    /// <internalonly/>
    [Syncfusion.Documentation.DocumentationExclude()]
    internal class GridDataBoundGridControlDesigner : ControlDesigner
    {
#if !SyncfusionFramework2_0
        public override void OnSetComponentDefaults()
        {
            GridDataBoundGrid grid = this.Control as GridDataBoundGrid;
            if (grid != null)
            {
                grid.UseListChangedEvent = true;
                grid.OptimizeInsertRemoveCells = true;
                grid.UseRightToLeftCompatibleTextBox = true;
            }
            base.OnSetComponentDefaults ();
        }
#else
        public override void InitializeNewComponent(IDictionary defaultValues)
        {
            GridDataBoundGrid grid = this.Control as GridDataBoundGrid;
            if (grid != null)
            {
                grid.UseListChangedEvent = true;
                grid.OptimizeInsertRemoveCells = true;
                grid.UseRightToLeftCompatibleTextBox = true;
            }

            base.InitializeNewComponent(defaultValues);
        }

        DesignerActionListCollection actionLists;

        private void BuildActionLists()
        {
            this.actionLists = new DesignerActionListCollection();
            this.actionLists.Add(new GridDataBoundGridChooseDataSourceActionList(this));
            DesignerVerb[] verbs = new DesignerVerb[Verbs.Count];
            Verbs.CopyTo(verbs, 0);
            ////this.actionLists.Add(new DesignerActionVerbList(verbs));
            this.actionLists.Add(new DesignerActionSupportList(Component, verbs));
            this.actionLists[0].AutoShow = true;
        }

        //// Properties
        public override DesignerActionListCollection ActionLists
        {
            get
            {
                if (this.actionLists == null)
                {
                    this.BuildActionLists();
                }

                return this.actionLists;
            }
        }

        internal object DataSource
        {
            get
            {
                GridDataBoundGrid grid = this.Control as GridDataBoundGrid;
                if (grid != null)
                {
                    return grid.DataSource;
                }

                return null;
            }

            set
            {
                GridDataBoundGrid grid = this.Control as GridDataBoundGrid;
                if (grid != null)
                {
                    grid.DataSource = value;
                }
            }
        }

        internal string DataMember
        {
            get
            {
                GridDataBoundGrid grid = this.Control as GridDataBoundGrid;
                if (grid != null)
                {
                    return grid.DataMember;
                }

                return null;
            }

            set
            {
                GridDataBoundGrid grid = this.Control as GridDataBoundGrid;
                if (grid != null)
                {
                    grid.DataMember = value;
                }
            }
        }
#endif
    }
    
#if SyncfusionFramework2_0
    //// Nested Types
    [ComplexBindingProperties("DataSource", "DataMember")]
    internal class GridDataBoundGridChooseDataSourceActionList : DesignerActionList
    {
        // Methods
        public GridDataBoundGridChooseDataSourceActionList(GridDataBoundGridControlDesigner owner)
            : base(owner.Component)
        {
            this.owner = owner;
        }

        public override DesignerActionItemCollection GetSortedActionItems()
        {
            DesignerActionItemCollection collection1 = new DesignerActionItemCollection();
            collection1.Add(new DesignerActionHeaderItem("Data", "Data"));
            DesignerActionPropertyItem item1 = new DesignerActionPropertyItem("DataSource", "Choose DataSource", "Data");
            item1.RelatedComponent = this.owner.Component;
            collection1.Add(item1);

            item1 = new DesignerActionPropertyItem("DataMember", "Choose DataMember", "Data");
            item1.RelatedComponent = this.owner.Component;
            collection1.Add(item1);

            return collection1;
        }

        // Properties
        [AttributeProvider(typeof(IListSource))]
        public object DataSource
        {
            get
            {
                return this.owner.DataSource;
            }

            set
            {
                GridDataBoundGrid view1 = (GridDataBoundGrid)this.owner.Component;
                IDesignerHost host1 = this.owner.Component.Site.GetService(typeof(IDesignerHost)) as IDesignerHost;
                PropertyDescriptor descriptor1 = TypeDescriptor.GetProperties(view1)["DataSource"];
                IComponentChangeService service1 = this.owner.Component.Site.GetService(typeof(IComponentChangeService)) as IComponentChangeService;
                object[] objArray1 = new object[1] { view1.Name };
                DesignerTransaction transaction1 = host1.CreateTransaction("GridDataBoundGridChooseDataSourceTransactionString " + objArray1.ToString());
                try
                {
                    service1.OnComponentChanging(this.owner.Component, descriptor1);
                    this.owner.DataSource = value;
                    service1.OnComponentChanged(this.owner.Component, descriptor1, null, null);
                    transaction1.Commit();
                    transaction1 = null;
                }
                finally
                {
                    if (transaction1 != null)
                    {
                        transaction1.Cancel();
                    }
                }
            }
        }

        [DefaultValue(""),
        Editor("System.Windows.Forms.Design.DataMemberListEditor, System.Design", "System.Drawing.Design.UITypeEditor, System.Drawing")]
        public string DataMember
        {
            get
            {
                return this.owner.DataMember;
            }

            set
            {
                GridDataBoundGrid view1 = (GridDataBoundGrid)this.owner.Component;
                IDesignerHost host1 = this.owner.Component.Site.GetService(typeof(IDesignerHost)) as IDesignerHost;
                PropertyDescriptor descriptor1 = TypeDescriptor.GetProperties(view1)["DataMember"];
                IComponentChangeService service1 = this.owner.Component.Site.GetService(typeof(IComponentChangeService)) as IComponentChangeService;
                object[] objArray1 = new object[1] { view1.Name };
                DesignerTransaction transaction1 = host1.CreateTransaction("GridDataBoundGridChooseDataMemberTransactionString " + objArray1.ToString());
                try
                {
                    service1.OnComponentChanging(this.owner.Component, descriptor1);
                    this.owner.DataMember = value;
                    service1.OnComponentChanged(this.owner.Component, descriptor1, null, null);
                    transaction1.Commit();
                    transaction1 = null;
                }
                finally
                {
                    if (transaction1 != null)
                    {
                        transaction1.Cancel();
                    }
                }
            }
        }

        // Fields
        private GridDataBoundGridControlDesigner owner;
    }

    ////internal class DesignerActionVerbList : DesignerActionList
    ////{
    ////    //// Methods
    ////    public DesignerActionVerbList(DesignerVerb[] verbs)
    ////        : base(null)
    ////    {
    ////        this._verbs = verbs;
    ////    }

    ////    public override DesignerActionItemCollection GetSortedActionItems()
    ////    {
    ////        DesignerActionItemCollection coll = new DesignerActionItemCollection();
    ////        for (int i = 0; i < this._verbs.Length; i++)
    ////        {
    ////            if ((this._verbs[i].Visible && this._verbs[i].Enabled) && this._verbs[i].Supported)
    ////            {
    ////                coll.Add(new DesignerActionVerbItem(this._verbs[i]));
    ////            }
    ////        }
    ////        return coll;
    ////    }

    ////    //// Properties
    ////    public override bool AutoShow
    ////    {
    ////        get
    ////        {
    ////            return false;
    ////        }
    ////    }

    ////    //// Fields
    ////    private DesignerVerb[] _verbs;
    ////}

    internal class DesignerActionVerbItem : DesignerActionMethodItem
    {
        //// Methods
        public DesignerActionVerbItem(DesignerVerb verb)
            : base(null, string.Empty, string.Empty)
        {
            if (verb == null)
            {
                throw new ArgumentNullException();
            }

            this._targetVerb = verb;
        }

        public override void Invoke()
        {
            this._targetVerb.Invoke();
        }
        
        //// Properties
        public override string Category
        {
            get
            {
                return "Grid";
            }
        }

        public override string Description
        {
            get
            {
                return this._targetVerb.Description;
            }
        }

        public override string DisplayName
        {
            get
            {
                return this._targetVerb.Text;
            }
        }

        public override bool IncludeAsDesignerVerb
        {
            get
            {
                return false;
            }
        }

        public override string MemberName
        {
            get
            {
                return null;
            }
        }

        //// Fields
        private DesignerVerb _targetVerb;
    }

    internal class DesignerActionSupportList : DesignerActionList
    {
        public DesignerActionSupportList(IComponent component, DesignerVerb[] verbs)
            : base(component)
        {
            this._verbs = verbs;
            ////this.AutoShow = true;
        }

        /// <summary>
        /// Returns the collection of <see cref="T:System.ComponentModel.Design.DesignerActionItem"/> objects contained in the list.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.ComponentModel.Design.DesignerActionItem"/> array that contains the items in this list.
        /// </returns>
        /// override
        public override DesignerActionItemCollection GetSortedActionItems()
        {
            DesignerActionItemCollection actionItems = new DesignerActionItemCollection();

            actionItems.Add(new DesignerActionHeaderItem("Look and feel", "Grid"));
            if (_verbs != null)
            {
                for (int i = 0; i < this._verbs.Length; i++)
                {
                    if ((this._verbs[i].Visible && this._verbs[i].Enabled) && this._verbs[i].Supported)
                    {
                        actionItems.Add(new DesignerActionVerbItem(this._verbs[i]));
                    }
                }
            }

            ////actionItems.Add(
            ////    new DesignerActionPropertyItem(
            ////    "ThemesEnabled",
            ////    "ThemesEnabled", "Grid"));

            actionItems.Add(
              new DesignerActionPropertyItem(
                "GridVisualStyles",
                "Skins",
                "Grid"));
            
            actionItems.Add(new DesignerActionHeaderItem("Documentation and Support Resource", "Assistance"));

            actionItems.Add(
                new DesignerActionMethodItem(
                this,
                "OnViewOnLineDocumentation",
                "Online Documentation", 
                "Assistance"));

            actionItems.Add(
              new DesignerActionMethodItem(
                this,
                "OnGoToForums",
                "Forums Support", 
                "Assistance"));

            actionItems.Add(
              new DesignerActionMethodItem(
                this,
                "OnGoToDirectTrac",
                "Direct-Trac Support", 
                "Assistance"));

            actionItems.Add(
                new DesignerActionPropertyItem(
                    "Search",
                    "Search: ", 
                    "Assistance"));

            actionItems.Add(
                new DesignerActionPropertyItem(
                    "KeyWord",
                    string.Empty, 
                    "Assistance"));
            
            actionItems.Add(
                new DesignerActionMethodItem(
                    this,
                    "OnSearch",
                    "Search...", 
                    "Assistance"));

            return actionItems;
        }
        
        #region ActionItem Members and Helpers

        protected void OnSearch()
        {
            switch (this.search)
            {
                case SearchOptions.Forums:
                    OnView(string.Format("http://www.syncfusion.com/support/forums/grid-windows/search/{0}", keyword));
                    break;
                case SearchOptions.KnowledgeBase:
                    OnView(string.Format("http://www.syncfusion.com/support/kb/tag/{0}", keyword));
                    break;
                case SearchOptions.Syncfusion:
                    OnView(string.Format("http://www.syncfusion.com/search/{0}", keyword));
                    break;
            }
        }

        private void OnViewOnLineDocumentation()
        {
            OnView("http://help.syncfusion.com/resources/User%20Interface/Windows%20Forms/Grid");
        }


        private void OnGoToForums()
        {
            OnView("http://www.syncfusion.com/support/forums");
        }

        private void OnGoToDirectTrac()
        {
            OnView("http://www.syncfusion.com/Account/Logon?ReturnUrl=%2fsupport%2fdirecttrac");
        }

        private void OnView(string link)
        {
            Cursor current = Cursor.Current;
            try
            {
                Cursor.Current = Cursors.WaitCursor;
                Process.Start(link);
            }
            finally
            {
                Cursor.Current = current;
            }
        }

        public string KeyWord
        {
            get
            {
                return keyword;
            }

            set
            {
                keyword = value;
            }
        }

       

        public GridVisualStyles GridVisualStyles
        {
            get
            {
                return Grid.Model.Options.GridVisualStyles;
            }

            set
            {
                IDesignerHost host = Component.Site.GetService(typeof(IDesignerHost)) as IDesignerHost;
                PropertyDescriptor pd = TypeDescriptor.GetProperties(Grid.Model.Options)["GridVisualStyles"];
                IComponentChangeService service = Component.Site.GetService(typeof(IComponentChangeService)) as IComponentChangeService;
                object[] objArray = new object[1] { Grid.Name };
                DesignerTransaction transaction = host.CreateTransaction("GridVisualStylesTransactionString " + objArray.ToString());
                try
                {
                    service.OnComponentChanging(Grid.Model.Options, pd);
                    Grid.Model.Options.GridVisualStyles = value;
                    service.OnComponentChanged(Grid.Model.Options, pd, null, null);
                    transaction.Commit();
                    transaction = null;
                }
                finally
                {
                    if (transaction != null)
                    {
                        transaction.Cancel();
                    }
                }
            }
        }

        public SearchOptions Search
        {
            get { return this.search; }
            set { this.search = value; }
        }

        private GridControlBase Grid
        {
            get
            {
                if (Component is GridControlBase)
                {
                    return (GridControlBase)this.Component;
                }
                else
                {
                    if (Component is GridListControl)
                    {
                        return (GridControlBase)(Component as GridListControl).Grid;
                    }
                }

                return null;
            }
        }

        #endregion ////ActionItem Members and Helpers

        //// Fields
        private DesignerVerb[] _verbs;
        string keyword = string.Empty;
        SearchOptions search = SearchOptions.KnowledgeBase;
    }

    internal enum SearchOptions
    {
        /// <summary>
        /// Represents Syncfusion
        /// </summary>
        Syncfusion,

        /// <summary>
        /// Represents Forums
        /// </summary>
        Forums,

        /// <summary>
        /// Represents Knowledge Base
        /// </summary>
        KnowledgeBase
    }
#endif
}
