//-------------------------------------------------------------------------------------------------
// <copyright file="GridListControl.cs" company="syncfusion">
//  Copyright (c) Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
// </copyright>
//-------------------------------------------------------------------------------------------------

using System;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing.Design;
using System.Design;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using System.Text;
using System.Security;
using System.Security.Permissions;

using Syncfusion.Collections;
using Syncfusion.Diagnostics;
using Syncfusion.Drawing;
using Syncfusion.Windows.Forms;

//// TODO: Sort, Sorted
//// TODO: ForeColor, BackColor
//// TODO: ToString
//// TODO: SelectedIndices, SelectedItems
//// TODO: ShowKeyboardCues / ShowFocusCues?
//// TODO: Tab Key, other dialog keys

namespace Syncfusion.Windows.Forms.Grid
{
    /// <summary>
    /// Implements a <see cref="ListControl"/> that shows data in the <see cref="GridControl"/>
    /// child window. Supports multiple columns, databinding, and selection of items similar to a list box.
    /// </summary>
    /// <remarks>
    /// Because <see cref="GridListControl"/> is derived from <see cref="ListControl"/>, you can easily replace an existing list box (which is also derived from ListControl)
    /// with this <see cref="GridListControl"/>.
    /// <para/>
    /// You can access the underlying grid control with the <see cref="Grid"/> property. The listcontrol
    /// supports several selection modes similar to a list box.
    /// </remarks>
    [Designer(typeof(GridListControlDesigner))]
    [ToolboxItem(true)]
    [System.Drawing.ToolboxBitmap(typeof(Syncfusion.Windows.Forms.Grid.GridListControl), "ToolboxIcons.GridListControl.bmp")]
    [Description("A list control based grid.")]
    public class GridListControl : ListControl,
        IThemedControl, ISupportInitialize
    {
        BorderStyle borderStyle = BorderStyle.FixedSingle;
        internal GridListControlChild grid;
        bool multiColumn = false;
        bool inSetData = false;
        bool resizeToFit = false;
        SelectionMode selectionMode = SelectionMode.One;
        int selectedIndex = -1;
        int updateCount = 0;
        ImageList imageList;
        PropertyDescriptor imageDescriptor = null;
        bool showColumnHeader = true;
        bool scrollAlwaysVisible = false;

        bool fillLastColumn = false;
        bool uiCues = false;
        bool allowResize = true;
        bool autoResize = true;
        bool isDisposing = false;
        ArrayList columns = new ArrayList();

        /// <summary>
        /// Occurs when the <see cref="MultiColumn"/> property is changed.
        /// </summary>
        [Category("Behavior")]
        [Description("Occurs when the MultiColumn property is changed.")]
        public event EventHandler MultiColumnChanged;

        /// <summary>
        ///   <para> Specifies that no matches are found during a search.</para>
        /// </summary>
        public const int NoMatches = -1 /*0xFFFFFFFF*/;

        /// <summary>
        ///   <para>Specifies the default item height for an owner-drawn <see cref="GridListControl" />.</para>
        /// </summary>
        public const int DefaultItemHeight = 13 /*0x000D*/;

        /// <summary>
        /// Initializes a new <see cref="GridListControl"/>.
        /// </summary>
        public GridListControl()
        {
            try
            {
                AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(AssemblyInfo.AssemblyResolver);
                new Syncfusion.Core.Licensing.LicensedComponent(typeof(GridListControl));
            }
            finally
            {
                AppDomain.CurrentDomain.AssemblyResolve -= new ResolveEventHandler(AssemblyInfo.AssemblyResolver);
            }

            grid = this.CreateGridChild();
            SetStyle(ControlStyles.ResizeRedraw, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | Syncfusion.Windows.Forms.WhidbeyCompatibleControlStyles.DoubleBuffer, true);
            SetStyle(ControlStyles.UserMouse, true);

            grid.BeginInit();
            grid.SuspendChangeEvents();
            grid.ControllerOptions = GridControllerOptions.ResizeCells | GridControllerOptions.SelectCells;
            grid.Initialize();

            grid.KeyDown += new KeyEventHandler(GridKeyDown);
            grid.CausesValidationChanged += new EventHandler(GridCausesValidationChanged);

            grid.QueryRowCount += new GridRowColCountEventHandler(GridQueryRowCount);
            grid.QueryColWidth += new GridRowColSizeEventHandler(GridQueryColWidth);
            grid.QueryCellInfo += new GridQueryCellInfoEventHandler(GridQueryCellInfo);
            grid.SaveCellInfo += new GridSaveCellInfoEventHandler(grid_SaveCellInfo);
            grid.QueryColCount += new GridRowColCountEventHandler(GridQueryColCount);
            grid.PrepareViewStyleInfo += new GridPrepareViewStyleInfoEventHandler(GridPrepareViewStyleInfo);

            grid.CurrentCellMoving += new GridCurrentCellMovingEventHandler(GridCurrentCellMoving);
            grid.CurrentCellActivating += new GridCurrentCellActivatingEventHandler(GridCurrentCellActivating);
            grid.CurrentCellActivated += new EventHandler(GridCurrentCellActivated);
            grid.CurrentCellDeactivated += new GridCurrentCellDeactivatedEventHandler(GridCurrentCellDeactivated);
            grid.ResizingColumns += new GridResizingColumnsEventHandler(GridResizingColumns);

            grid.SelectionChanged += new GridSelectionChangedEventHandler(GridSelectionChanged);
            grid.GotFocus += new EventHandler(GridGotFocus);
            grid.LostFocus += new EventHandler(GridLostFocus);

            grid.Paint += new PaintEventHandler(GridPaint);
            grid.MouseWheel += new MouseEventHandler(GridMouseWheel);
            grid.MouseUp += new MouseEventHandler(GridMouseUp);
            grid.MouseMove += new MouseEventHandler(GridMouseMove);
            grid.MouseHover += new EventHandler(GridMouseHover);
            grid.MouseLeave += new EventHandler(GridMouseLeave);
            grid.MouseEnter += new EventHandler(GridMouseEnter);
            grid.GridControlMouseMove += new CancelMouseEventHandler(GridBeforeMouseMove);
            grid.MouseDown += new MouseEventHandler(GridMouseDown);
            grid.KeyUp += new KeyEventHandler(GridKeyUp);
            grid.KeyPress += new KeyPressEventHandler(GridKeyPress);
            grid.HelpRequested += new HelpEventHandler(GridHelpRequested);
            grid.DragDrop += new DragEventHandler(GridDragDrop);
            grid.DragLeave += new EventHandler(GridDragLeave);
            grid.DragOver += new DragEventHandler(GridDragOver);
            grid.DragEnter += new DragEventHandler(GridDragEnter);
            grid.DoubleClick += new EventHandler(GridDoubleClick);
            grid.Click += new EventHandler(GridClick);
            grid.ContextMenuChanged += new EventHandler(GridContextMenuChanged);

            grid.InsideScrollMargins = Size.Empty;
            grid.CommandStack.Enabled = false;
            grid.Rows.DefaultSize = 17;
            grid.Cols.DefaultSize = 65;
            grid.RowHeights[0] = 25;
            grid.ColWidths[0] = 0;
            grid.ExcelLikeCurrentCell = false;
            grid.ExcelLikeSelectionFrame = false;
            grid.AllowDragSelectedCols = false;
            grid.AllowDragSelectedRows = false;
            grid.AllowColumnResizeUsingCellBoundaries = true;
            grid.CommandStack.Enabled = false;
            grid.FloatCellsMode = GridFloatCellsMode.None;
            grid.NumberedColHeaders = false;
            grid.NumberedRowHeaders = false;
            grid.Dock = DockStyle.Fill;
            grid.BackColor = SystemColors.Window;
            grid.ListBoxSelectionMode = System.Windows.Forms.SelectionMode.One;
            grid.Model.Options.MulitExtendedArrowKeySelect = false;
            ResetBaseStyles();

            grid.Properties.BackgroundColor = SystemColors.Window;

            grid.ShowCurrentCellBorderBehavior = GridShowCurrentCellBorder.HideAlways;
            grid.AllowSelection = GridSelectionFlags.Row | GridSelectionFlags.AlphaBlend;
            grid.ResizeRowsBehavior = GridResizeCellsBehavior.None;
            grid.ResizeColsBehavior = GridResizeCellsBehavior.InsideGrid | GridResizeCellsBehavior.ResizeSingle;
            grid.TabIndex = 1;
            grid.WantKeys = true;
            this.BorderStyle = BorderStyle.Fixed3D;
            grid.VerticalThumbTrack = true;
            grid.HorizontalThumbTrack = true;
            grid.WantTabKey = false;
            grid.CurrentCell.ActivateOnGotFocus = true;

            SuspendLayout();
            Controls.Add(grid);
            grid.ResumeChangeEvents();
            ResumeLayout(false);
            grid.EndInit();
        }
        #region For Touch

        bool isScaling = false;
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
                for (int i = 0; i < this.Grid.RowCount; i++)
                {
                    if (this.Grid.Model.RowHeights[i] != 0)
                        this.Grid.Model.RowHeights[i] += 5;
                }
                for (int i = 0; i < this.Grid.ColCount; i++)
                {
                    if (this.Grid.Model.ColWidths[i] != 0)
                    this.grid.Model.ColWidths[i] += 15;
                }
            }
            else
            {
                for (int i = 0; i < this.Grid.RowCount; i++)
                {
                    if (this.Grid.Model.RowHeights[i] != 0)
                        this.Grid.Model.RowHeights[i] -= 5;
                }
                for (int i = 0; i < this.grid.ColCount; i++)
                {
                    if (this.Grid.Model.ColWidths[i] != 0)
                        this.Grid.Model.ColWidths[i] -= 15;
                }
            }
            this.ResumeLayout();
            this.Invalidate();
            this.EndUpdate();
        }
        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
        }
        #endregion
        void grid_SaveCellInfo(object sender, GridSaveCellInfoEventArgs e)
        {
            e.Handled = true;
        }

        /// <summary>
        /// Creates the grid child.
        /// </summary>
        /// <returns>returns GridListControlChild</returns>
        /// <internalonly/>
        [Syncfusion.Documentation.DocumentationExclude]
        protected virtual GridListControlChild CreateGridChild()
        {
            return new GridListControlChild(this);
        }

        void ResetBaseStyles()
        {
            grid.BaseStylesMap.RegisterStandardStyles();
            GridStyleInfo standard = grid.BaseStylesMap["Standard"].StyleInfo;
            standard.CellType = "Static";
            standard.Trimming = StringTrimming.EllipsisCharacter;
            grid.BaseStylesMap["Header"].StyleInfo.Enabled = true;
            grid.BaseStylesMap["Column Header"].StyleInfo.Enabled = false;
            grid.BaseStylesMap["Row Header"].StyleInfo.Enabled = true;
            grid.BaseStylesMap.Modified = false;
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
        /// Gets the underlying <see cref="GridControl"/> wrapped by this ListControl class.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public GridControl Grid
        {
            get
            {
                return grid;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to toggle support for Windows 2000 and Windows XP transparency. Set this to True
        /// if you want the grid to draw transparent over a background bitmap.
        /// </summary>
        [Browsable(true)]////, DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [DefaultValue(false)]
        [Category("Appearance")]
        [Description("Gets or sets a value indicating whether to toggle support for Windows 2000 and Windows XP transparency. Set this to True if you want the grid to draw transparent over a background bitmap.")]
        public bool SupportsTransparentBackColor
        {
            get
            {
                return GetStyle(ControlStyles.SupportsTransparentBackColor);
            }

            set
            {
                if (this.SupportsTransparentBackColor != value)
                {
                    SetStyle(ControlStyles.SupportsTransparentBackColor, value);
                    foreach (Control c in this.Controls)
                    {
                        MakeTransparent(c, this.SupportsTransparentBackColor);
                    }

                    this.OnSupportsTransparentBackColorChanged(EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Occurs when the <see cref="SupportsTransparentBackColor"/> has changed.
        /// </summary>
        [Category("Appearance")]
        [Description("Occurs when the SupportsTransparentBackColor property has changed.")]
        public event EventHandler SupportsTransparentBackColorChanged;

        /// <override/>
        void _ControlAdded(ControlEventArgs e)
        {
            MakeTransparent(e.Control, this.SupportsTransparentBackColor);
        }

        private void MakeTransparent(Control control, bool value)
        {
            System.Reflection.MethodInfo mInfo = typeof(Control).GetMethod("SetStyle", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.InvokeMethod | System.Reflection.BindingFlags.NonPublic);
            if (mInfo != null)
            {
                mInfo.Invoke(control, new object[] { ControlStyles.SupportsTransparentBackColor, value });
            }
        }

        /// <summary>
        /// Raises the <see cref="SupportsTransparentBackColorChanged"/> event.
        /// </summary>
        /// <param name="e">An <see cref="EventArgs" /> that contains the event data.</param>
        protected virtual void OnSupportsTransparentBackColorChanged(EventArgs e)
        {
            if (SupportsTransparentBackColorChanged != null)
            {
                SupportsTransparentBackColorChanged(this, e);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the user can resize columns.
        /// </summary>
        [DefaultValue(true)]
        [Description("Indicates if the user can resize columns.")]
        [Category("Grid")]
        public bool AllowResizeColumns
        {
            get
            {
                return allowResize;
            }

            set
            {
                if (value != allowResize)
                {
                    allowResize = value;
                    if (allowResize)
                    {
                        grid.ResizeColsBehavior = GridResizeCellsBehavior.InsideGrid | GridResizeCellsBehavior.ResizeSingle;
                    }
                    else
                    {
                        grid.ResizeColsBehavior = GridResizeCellsBehavior.None;
                    }
                }
            }
        }

        void GridResizingColumns(object sender, GridResizingColumnsEventArgs e)
        {
#if DEBUG
            if (Switches.Development.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo();
            }
#endif
            if (e.Reason == GridResizeCellsReason.HitTest)
            {
                Point p = e.Point;
                int row, col;
                grid.PointToRowCol(p, out row, out col);
                if (row > grid.Rows.HeaderCount && !grid.AllowColumnResizeUsingCellBoundaries)
                {
                    e.Cancel = true;
                }
            }
            else if (e.Reason == GridResizeCellsReason.DoubleClick)
            {
                GridRangeInfo columns = grid.Selections.Ranges.GetRangesContaining(e.Columns).ActiveRange.UnionRange(e.Columns);
                grid.Model.ColWidths.ResizeToFit(grid.ViewLayout.VisibleCellsRange.IntersectRange(columns), GridResizeToFitOptions.IncludeHeaders);
                e.Cancel = true;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether columns should be automatically sized
        /// to fit contents of their cells.
        /// </summary>
        [Description("Indicates if columns should be automatically sized to fit contents of their cells.")]
        [DefaultValue(true)]
        [Category("Grid")]
        public bool AutoSizeColumns
        {
            get
            {
                return autoResize;
            }

            set
            {
                if (value != autoResize)
                {
                    autoResize = value;
                    int n = this.FillLastColumn ? 1 : 0;
                    if (autoResize && grid.ColCount > n)
                    {
                        if (autoResize && this.Visible)
                        {
                            grid.ColWidths.ResizeToFit(GridRangeInfo.Cells(0, 1, grid.RowCount, grid.ColCount - n), GridResizeToFitOptions.NoShrinkSize | GridResizeToFitOptions.IncludeHeaders);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether column headers should be displayed.
        /// </summary>
        [DefaultValue(true)]
        [Description("Indicates if column headers should be displayed.")]
        [Category("Grid")]
        public bool ShowColumnHeader
        {
            get
            {
                return showColumnHeader;
            }

            set
            {
                if (showColumnHeader != value)
                {
                    showColumnHeader = value;
                    if (showColumnHeader)
                    {
                        Grid.HideRows[0] = false;
                    }
                    else
                    {
                        Grid.HideRows[0] = true;
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the last column should be sized in a way
        /// that the whole client area of the grid is used.
        /// </summary>
        [DefaultValue(false)]
        [Description("Indicates if the last column should be sized in a way that the whole client area of the grid is used.")]
        [Category("Grid")]
        public bool FillLastColumn
        {
            get
            {
                return fillLastColumn;
            }

            set
            {
                if (fillLastColumn != value)
                {
                    fillLastColumn = value;
                    Grid.SmoothControlResize = !value;
                    PerformLayout();
                    Refresh();
                }
            }
        }

        /// <override/>
        /// <summary>Gets or sets the text associated with this control.</summary>
        public override string Text
        {
            get
            {
                if (Items != null && this.selectedIndex >= 0 && this.selectedIndex < Items.Count)
                {
                    return GetItemText(Items[selectedIndex]);
                }

                return base.Text;
            }

            set
            {
                if (Items != null && value != Text)
                {
                    if (value.Length > 0)
                    {
                        FindItem(value, true, -1, false);
                    }
                    else
                    {
                        this.SelectedIndex = -1;
                    }
                }
            }
        }

        internal bool NeedsResizeToFit
        {
            get
            {
                return resizeToFit;
            }
        }

        /// <override/>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                isDisposing = true;
                this.DataSource = null;

                if (gridListControlSource != null)
                {
                    gridListControlSource.ItemPropertiesChanged -= new EventHandler(gridListControlSource_ItemPropertiesChanged);
                }

                if (this._themedDrawing != null)
                {
                    this._themedDrawing.Dispose();
                    this._themedDrawing = null;
                }
            }

            base.Dispose(disposing);
        }

        /// <override/>
        [SecurityPermission(SecurityAction.LinkDemand, UnmanagedCode = true)]
        protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {
                case 522: /*0x20a WM_MOUSEWHEEL*/
                    this.WmMouseWheel(ref m);
                    return;
                case NativeMethods.WM_NCPAINT:
                    this.WmNcPaint(ref m);
                    break;
            }

            base.WndProc(ref m);
        }

        private ThemedWindowDrawing _themedDrawing = null;

        private ThemedWindowDrawing themedDrawing
        {
            get
            {
                if (XPThemes.IsThemedOS && XPThemes.IsAppThemed)
                {
                    if (_themedDrawing == null)
                    {
                        _themedDrawing = new ThemedWindowDrawing();
                    }
                }

                return this._themedDrawing;
            }
        }

        bool forceNonThemedBorder = false;

        /// <summary>
        /// Gets or sets a value indicating whether to disable themed border drawing and instead draw a solid black border
        /// when setting BorderStyle.FixedSingle.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Category("Grid")]
        public bool ForceNonThemedBorder
        {
            get
            {
                return forceNonThemedBorder;
            }

            set
            {
                forceNonThemedBorder = value;
            }
        }
        
        private void WmNcPaint(ref Message msg)
        {
            bool themed = XPThemes.IsThemedOS && ((IThemedControl)this).ThemesEnabled;
            if (themed && themedDrawing != null && !forceNonThemedBorder)
            {
                themedDrawing.DrawThemedBorderColor(this, ref msg);
            }

            base.WndProc(ref msg);
        }

        private void WmMouseWheel(ref Message m)
        {
            if (this.grid != null && !this.ContainsFocus)
            {
                Point p = new Point((short)(int)m.LParam, (int)m.LParam >> 16/*0x10*/);
                p = grid.PointToClient(p);

                int wdelta = (int)m.WParam.ToInt64();
                wdelta = wdelta >> 16/*0x10*/;
                grid.RaiseMouseWheel(new MouseEventArgs(MouseButtons.None, 0, p.X, p.Y, wdelta));
            }
            else
            {
                base.WndProc(ref m);
            }
        }

        /// <summary>
        /// Sets the <see cref="ListControl.DataSource"/>, <see cref="ListControl.DisplayMember"/>,
        /// and <see cref="ListControl.ValueMember"/> properties at run-time.
        /// </summary>
        /// <param name="bindingContext">The <see cref="BindingContext"/> of the parent form.</param>
        /// <param name="dataSource">The data source, typed as <see cref="System.Object"/>, for the <see cref="GridListControl"/> control. </param>
        /// <param name="displayMember">A string that specifies the property of the datasource whose contents you want to display.</param>
        /// <param name="valueMember">A string that specifies the property of the datasource from which to draw the value.</param>
        public void SetDataBinding(BindingContext bindingContext, object dataSource, string displayMember, string valueMember)
        {
            if (this.DataManager == null ||
                bindingContext != BindingContext
                || dataSource != this.DataSource || displayMember != this.DisplayMember || valueMember != this.ValueMember)
            {
                inSetData = true;
                SelectedIndex = -1;
                DataSource = null;
                DisplayMember = string.Empty;
                ValueMember = string.Empty;
                DisplayMember = displayMember;
                ValueMember = valueMember;
                DataSource = dataSource;
                BindingContext = bindingContext;
                OnBindingContextChanged(EventArgs.Empty);
                inSetData = false;
                resizeToFit = true;
                SetItemsCore(null);
            }
        }

        /// <summary>
        /// Gets or sets a list of images that can be referenced with ImageIndex properties.
        /// </summary>
        [Description("A list of images that can be referenced with ImageIndex properties.")]
        [Category("Grid")]
        public ImageList ImageList
        {
            get
            {
                return imageList;
            }

            set
            {
                imageList = value;
            }
        }

        bool ShouldSerializeImageList()
        {
            return imageList != null;
        }

        void GridQueryColCount(object sender, GridRowColCountEventArgs e)
        {
            if (columnsDirty && DataManager != null)
            {
                SetItemsCore(DataManager.List);
            }

            if (this.ListManager == null)
            {
                e.Count = 255;
            }
            else if (multiColumn)
            {
                if (this.gridListControlSource != null)
                {
                    e.Count = this.gridListControlSource.GetVisibleColumnCount();
                }
                else
                {
                    e.Count = columns.Count;
                }
            }
            else
            { 
                e.Count = 1; 
            }

            e.Handled = true;
        }

        ////#region ISupportInitialize
        bool inInit;

        /// <summary>
        /// Implements the <see cref="ISupportInitialize.BeginInit"/> of the <see cref="ISupportInitialize"/> interface.
        /// </summary>
        public void BeginInit()
        {
#if DEBUG
            if (Switches.BeginEndUpdate.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo();
            }
#endif
            if (inInit)
            {
                throw new System.Exception("BeginInit called twice.");
            }

            inInit = true;

            BeginUpdate();
        }

        /// <summary>
        /// Implements the <see cref="ISupportInitialize.EndInit"/> of the <see cref="ISupportInitialize"/> interface.
        /// </summary>
        public void EndInit()
        {
#if DEBUG
            if (Switches.BeginEndUpdate.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo();
            }
#endif
            inInit = false;

            EndUpdate();
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
        ////#endregion

        /// <summary>
        /// Provides access to style information for a specified column in the list control.
        /// </summary>
        /// <param name="colNum">The zero-based column index.</param>
        /// <returns>The style information for the specified column.</returns>
        /// <remarks>
        /// For GridListControl you need to use this method to access style information
        /// because the column styles indexer of the underlying grid control is ignored
        /// when style information is collected for cells in the list control.
        /// </remarks>
        public GridStyleInfo GetColumnStyle(int colNum)
        {
            return (GridStyleInfo)columns[colNum];
        }

        bool columnsDirty = false;

        internal void InitializeColumns(CurrencyManager listManager)
        {
            if (inSetData || inInit)
            {
                return;
            }

            columnsDirty = false;
            grid.BeginUpdate();
            try
            {
                this.columns = new ArrayList();

                if (listManager == null)
                {
                    return;
                }

                ////this.gridColumns.remove_CollectionChanged(new CollectionChangeEventHandler(this, OnColumnCollectionChanged));
                PropertyDescriptorCollection pdc = listManager.GetItemProperties();
                int count = pdc.Count;
                int total = 0;
                imageDescriptor = null;
                for (int index = 0; index < count; index++)
                {
                    PropertyDescriptor pd = pdc[index];
                    if (pd.IsBrowsable)
                    {
                        if (this.PropertyDescriptorIsARelation(pd))
                        {
                            ////this.relationsList.Add(pd.Name);
                        }
                        else if (pd.Name == "ImageIndex")
                        {
                            this.imageDescriptor = pd;
                        }
                        else if (multiColumn || (total == 0 && GridUtil.IsEmpty(this.DisplayMember)) || pd.Name == this.DisplayMember)
                        {
                            total++;
                            this.CreateGridColumn(pd, total);
                        }
                    }
                }

                grid.ViewLayout.Reset();
                resizeToFit = true;
            }
            finally
            {
                grid.EndUpdate();
                grid.ResetVolatileData();
            }
        }

        /// <summary>
        /// Creates a column for a given <see cref="PropertyDescriptor"/> from the datasource.
        /// </summary>
        /// <param name="pd">The <see cref="PropertyDescriptor"/> with column information.</param>
        /// <param name="column">The column index in the grid.</param>
        public virtual void CreateGridColumn(PropertyDescriptor pd, int column)
        {
            GridStyleInfo style = new GridStyleInfo();
            columns.Add(style);

            Type type;

            type = pd.PropertyType;
            style.CellValueType = type;
            if (type.Equals(typeof(Boolean)))
            {
                style.CellType = "GridCheckBoxCell";
            }
            else if (type.Equals(typeof(String)))
            { 
            }
            else if (type.Equals(typeof(DateTime)))
            {
                style.Format = "d";
            }
            else if (type.Equals(typeof(Int16)) || type.Equals(typeof(Int32)) || type.Equals(typeof(Int64)) || type.Equals(typeof(UInt16)) || type.Equals(typeof(UInt32)) || type.Equals(typeof(UInt64)) || type.Equals(typeof(Decimal)) || type.Equals(typeof(Double)) || type.Equals(typeof(Single)) || type.Equals(typeof(Byte)) || type.Equals(typeof(SByte)))
            {
                style.Format = "G";
                style.HorizontalAlignment = GridHorizontalAlignment.Right;
            }
            else if (type == typeof(Decimal))
            {
                style.HorizontalAlignment = GridHorizontalAlignment.Right;
            }

            ////grid[0, column].Text = pd.Name;
            style.Tag = pd;

            ////grid.ColWidths[column] = grid[0, column].CellModel.CalculatePreferredCellSize(grid.CreateGridGraphics(), 0, column, grid[0, column], GridQueryBounds.Width).Width;
        }

        /// <override/>
        protected override void OnLayout(LayoutEventArgs e)
        {
#if DEBUG
            if (Switches.GridListControl.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(Name, e.AffectedProperty);
            }
#else
            ;
#endif
            base.OnLayout(e);
            Grid.ViewLayout.Reset();
            Invalidate();
        }

        ////        ///// <override/>
        ////        protected override void OnVisibleChanged(EventArgs e)
        ////        {
        ////            TraceUtil.TraceCurrentMethodInfoIf(Switches.GridListControl.TraceVerbose, Name);
        ////            base.OnVisibleChanged(e);
        ////            Grid.ViewLayout.Reset();
        ////            Invalidate();
        ////        }
        ////
        ////        ///// <override/>
        ////        protected override void OnSizeChanged(EventArgs e)
        ////        {
        ////            base.OnSizeChanged(e);
        ////        }

        /// <summary>
        /// Finds text in the list box.
        /// </summary>
        /// <param name="prefix">The text (or prefix) to find.</param>
        /// <param name="selectItem">True if you want to select the text in the list box.</param>
        /// <param name="start">The first index to start searching.</param>
        /// <param name="ignoreCase">True if case can be ignored; False if case sensitive.</param>
        /// <returns>The index of the entry that starts with the text; -1 if
        /// no entry could be found.</returns>
        public virtual int FindItem(string prefix, bool selectItem, int start, bool ignoreCase)
        {
            string itemText;
            int index = FindItemInternal(Items, prefix, start, ignoreCase, out itemText);
            if (selectItem && index != this.SelectedIndex)
            {
                this.SelectedIndex = index;
            }

            return index;
        }

        internal int FindItemInternal(IList items, string prefix, int start, bool ignoreCase, out string itemText)
        {
            return FindItemInternal(items, prefix, start, ignoreCase, false, out itemText);
        }

        internal int FindItemInternal(IList items, string prefix, int start, bool ignoreCase, bool exact, out string itemText)
        {
            if (ignoreCase)
            {
                prefix = prefix.ToUpper(CultureInfo.CurrentCulture);
            }

            int count = items.Count;
            for (int i = 0; i < count; i++)
            {
                int index = (i + start + 1) % count;
                ////cb itemText = this.GetItemText(Items[index]);
                itemText = this.GetItemText(items[index]);

                //// itemText will be the same as the item's text.
                //// compareText will be the text against which the prefix will be compared.
                string compareText = itemText;
                if (ignoreCase)
                {
                    compareText = compareText.ToUpper(CultureInfo.CurrentCulture);
                }

                if (exact)
                {
                    if (prefix == compareText)
                    {
                        return index;
                    }
                }
                else if (compareText.Length >= prefix.Length)
                {
                    compareText = compareText.Substring(0, prefix.Length);

                    if (prefix == compareText)
                    {
                        return index;
                    }
                }
            }

            itemText = string.Empty;
            return -1;
        }

        /// <summary>
        /// Handles the <see cref="GridControl.SelectionChanged"/> event of the <see cref="Grid"/> and sets <see cref="SelectedIndex"/> property.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        protected virtual void GridSelectionChanged(object sender, GridSelectionChangedEventArgs e)
        {
            if (e.Range.Top > 0 && grid.CurrentCell.HasCurrentCell && grid.CurrentCell.RowIndex > 0)
            {
                this.SelectedIndex = grid.CurrentCell.RowIndex - 1;
            }
        }

        /// <summary>
        /// Handles the <see cref="GridControl.QueryColWidth"/> event of the <see cref="Grid"/> and returns the width of individual columns.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        protected virtual void GridQueryColWidth(object sender, GridRowColSizeEventArgs e)
        {
            if (gridListControlSource != null)
            {
                if (this.fillLastColumn && e.Index == Grid.ColCount)
                {
                    int width = Grid.ColCount <= 0 ? 0 : Grid.ColWidths.GetTotal(0, Grid.ColCount - 1);
                    e.Size = Grid.ClientRectangle.Width - width;
                    e.Handled = true;
                }
                else if (e.Index > 0)
                {
                    if (this.DataSource is IGridListControlSource)
                    {
                        e.Size = gridListControlSource.GetWidth(e.Index - 1);
                        e.Handled = true;
                    }
                }
                else
                {
                    e.Size = 0;
                    e.Handled = true;
                }
            }
            else
            {
                if (this.fillLastColumn && e.Index == Grid.ColCount)
                {
                    int width = Grid.ColCount <= 0 ? 0 : Grid.ColWidths.GetTotal(0, Grid.ColCount - 1);
                    e.Size = Grid.ClientRectangle.Width - width;
                    e.Handled = true;
                }
            }
        }

        /// <summary>
        /// Handles the <see cref="Control.KeyDown"/> event of the <see cref="Grid"/> and moves the current selection when an arrow key is pressed.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        protected virtual void GridKeyDown(object sender, KeyEventArgs e)
        {
#if DEBUG
            if (Switches.GridListControl.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(Name, e.KeyCode);
            }
#else
            ;
#endif
            base.OnKeyDown(e);

            if (e.Handled)
            {
                return;
            }

            Keys modifierKeys = Control.ModifierKeys;
            Keys keyCode = e.KeyCode & Keys.KeyCode;
            bool controlKeyDown = (e.Modifiers & Keys.Control) != Keys.None;
            bool shiftKeyDown = (e.Modifiers & Keys.Shift) != Keys.None;
            bool extendSelection = (modifierKeys & Keys.Shift) != Keys.None;
            bool menuKeyDown = (e.Modifiers & Keys.Menu) != Keys.None;

            switch (keyCode)
            {
                case Keys.Space:
                    if (selectionMode == SelectionMode.MultiExtended
                        || selectionMode == SelectionMode.MultiSimple)
                    {
                        GridRangeInfo rowRange = GridRangeInfo.Row(grid.CurrentCell.RowIndex);
                        grid.Selections.SelectRange(rowRange, !grid.Selections.Ranges.AnyRangeContains(rowRange));
                        e.Handled = true;
                    }

                    break;

                case Keys.Home:
                    grid.CurrentCell.Move(GridDirectionType.Top, 1, extendSelection);
                    e.Handled = true;
                    break;
                case Keys.End:
                    grid.CurrentCell.Move(GridDirectionType.Bottom, 1, extendSelection);
                    e.Handled = true;
                    break;
                case Keys.PageDown:
                    grid.CurrentCell.Move(GridDirectionType.PageDown, 1, extendSelection);
                    e.Handled = true;
                    break;
                case Keys.PageUp:
                    grid.CurrentCell.Move(GridDirectionType.PageUp, 1, extendSelection);
                    e.Handled = true;
                    break;
            }
        }

        /// <override/>
        protected override void OnKeyDown(KeyEventArgs e)
        {
            GridKeyDown(this, e);
        }

        private bool PropertyDescriptorIsARelation(PropertyDescriptor prop)
        {
            if (typeof(IList).IsAssignableFrom(prop.PropertyType))
            {
                return !typeof(Array).IsAssignableFrom(prop.PropertyType);
            }

            return false;
        }

        void GridGotFocus(object sender, EventArgs e)
        {
#if DEBUG
            if (Switches.GridListControl.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo();
            }
#endif
            ////            Trace.WriteLine("GridGotFocus");
        }

        void GridLostFocus(object sender, EventArgs e)
        {
            ////            Trace.WriteLine("GridLostFocus");
            if (SelectedIndex != -1)
            {
                RefreshItem(SelectedIndex);
            }
        }

        void GridChangeUICues(object sender, UICuesEventArgs e)
        {
#if DEBUG
            if (Switches.GridListControl.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo();
            }
#endif
            Grid.Invalidate();
            this.uiCues = e.ShowFocus || e.ShowKeyboard;
            base.OnChangeUICues(e);
        }

        /// <override/>
        protected override void OnChangeUICues(UICuesEventArgs e)
        {
#if DEBUG
            if (Switches.GridListControl.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo();
            }
#endif
            Grid.Invalidate();
            base.OnChangeUICues(e);
        }

        /// <summary>
        ///   <para>Gets or sets the border style of the control.</para>
        /// </summary>
        [SRCategory(@"Appearance"),
        DefaultValue(BorderStyle.None),
        Description(@"The border style of the control.")]
        public BorderStyle BorderStyle
        {
            get
            {
                return this.borderStyle;
            }

            set
            {
                if (this.borderStyle != value)
                {
                    if (!Enum.IsDefined(typeof(System.Windows.Forms.BorderStyle), value))
                    {
                        throw new InvalidEnumArgumentException("value", (int)value, typeof(BorderStyle));
                    }

                    this.borderStyle = value;
                    UpdateStyles();
                }
            }
        }

        /// <override/>
        protected override CreateParams CreateParams
        {
            [SecurityPermission(SecurityAction.LinkDemand, UnmanagedCode = true)]
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle = cp.ExStyle | 0x80/*WS_EX_TOOLWINDOW*/;

                switch (this.borderStyle)
                {
                    case BorderStyle.Fixed3D:
                        cp.ExStyle |= 0x200; // WS_EX_DLGFRAME
                        break;
                    case BorderStyle.FixedSingle:
                        cp.Style |= 0x800000; // WS_BORDER
                        break;
                }

                return cp;
            }
        }

        /// <summary>
        ///   <para> Gets or sets a value indicating whether the vertical scroll bar is shown at all times.</para>
        /// </summary>
        [Description("Indicates if the list box should always have a scroll bar present, regardless of how many items are in it."),
        Localizable(true),
        Category("Behavior"),
        DefaultValue(false)]
        public bool ScrollAlwaysVisible
        {
            get
            {
                return this.scrollAlwaysVisible;
            } // end of method get_ScrollAlwaysVisible

            set
            {
                if (this.scrollAlwaysVisible != value)
                {
                    this.scrollAlwaysVisible = value;
                    if (this.scrollAlwaysVisible)
                    {
                        Grid.VScrollBehavior = GridScrollbarMode.Enabled | GridScrollbarMode.AutoScroll;
                    }
                    else
                    {
                        Grid.VScrollBehavior = GridScrollbarMode.Automatic;
                    }

                    Grid.UpdateScrollBars();
                }
            } // end of method set_ScrollAlwaysVisible
        }

        /// <summary>
        ///   <para>Gets or sets the currently selected item in the <see cref="GridListControl"/>
        /// .</para>
        /// </summary>
        [Bindable(true),
        Browsable(false),
        Description("The currently selected item in the list box, or NULL."),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public object SelectedItem
        {
            get
            {
                if (Grid.Selections.Count > 0 && Items != null)
                {
                    int index = Grid.Selections.Ranges[0].Top - 1;
                    if (index >= 0 && index < Items.Count)
                    { 
                        return Items[index]; 
                    }
                }

                return null;
            } // end of method get_SelectedItem

            set
            {
                int index = -1;
                if (Items != null && value != null)
                {
                    index = Items.IndexOf(value);
                }

                if (index != SelectedIndex)
                {
                    SelectedIndex = index;
                }
            } // end of method set_SelectedItem
        }

        private void CheckIndex(int index)
        {
            if (index < -1 || index >= this.Items.Count)
            {
                throw new ArgumentOutOfRangeException(SR.GetString("IndexOutOfRange", index));
            }
        }

        private void CheckNoDataSource()
        {
            if (base.DataSource != null)
            {
                throw new ArgumentException(SR.GetString("DataSourceLocksItems"));
            }
        }

        /// <summary>
        ///   <para>Unselects all items in the <see cref="GridListControl" />.</para>
        /// </summary>
        public void ClearSelected()
        {
            if (grid.Selections.Count > 0)
            {
                grid.Selections.Clear();
                this.OnSelectedIndexChanged(EventArgs.Empty);
            }
        }

        /// <overload>
        ///   <para>Finds the first item in the <see cref="GridListControl" />
        /// that starts with the specified string.</para>
        /// </overload>
        /// <summary>
        ///   <para>Finds the first item in the <see cref="GridListControl" />
        /// that starts with the specified string.</para>
        /// </summary>
        /// <param name="s">The text to search for. </param>
        /// <returns>
        ///   <para>The zero-based index of the first item found; returns
        /// <see langword="ListBox.NoMatches" /> if no match is found.</para>
        /// </returns>
        public int FindString(string s)
        {
            return this.FindString(s, 0);
        }

        /// <summary>
        ///   <para>Finds the first item in the <see cref="GridListControl" /> that starts with the specified string.
        /// The search starts at a specific starting index.</para>
        /// </summary>
        /// <param name="s">The text to search for. </param>
        /// <param name="startIndex">The zero-based index of the item before the first item to be searched. Set to negative one (-1) to search from the beginning of the control. </param>
        /// <returns>
        ///   <para>The zero-based index of the first item found; returns
        /// <see langword="ListBox.NoMatches" />
        /// if no match is found.</para>
        /// </returns>
        public int FindString(string s, int startIndex)
        {
            this.CheckIndex(startIndex);
            string itemText;
            return this.FindItemInternal(Items, s, startIndex, true, out itemText);
        }

        /// <overload>
        ///   <para>Finds the first item in the <see cref="GridListControl" />
        /// that exactly matches the specified string.</para>
        /// </overload>
        /// <summary>
        ///   <para>Finds the first item in the <see cref="GridListControl" />
        /// that exactly matches the specified string.</para>
        /// </summary>
        /// <param name="s">The text to search for. </param>
        /// <returns>
        ///   <para>The zero-based index of the first item found; returns
        /// <see langword="ListBox.NoMatches" />
        /// if no match is found.</para>
        /// </returns>
        public int FindStringExact(string s)
        {
            return this.FindStringExact(s, -1);
        }

        /// <summary>
        ///   <para>Finds the first item in the <see cref="GridListControl" /> that exactly matches the specified string.
        /// The search starts at a specific starting
        /// index.</para>
        /// </summary>
        /// <param name="s">The text to search for. </param>
        /// <param name="startIndex">The zero-based index of the item before the first item to be searched. Set to negative one (-1) to search from the beginning of the control. </param>
        /// <returns>
        ///   <para>The zero-based index of the first item found;
        /// returns <see langword="ListBox.NoMatches" />
        /// if no match is found.</para>
        /// </returns>
        public int FindStringExact(string s, int startIndex)
        {
            this.CheckIndex(startIndex);
            string itemText;
            return this.FindItemInternal(Items, s, startIndex, false, true, out itemText);
        }

        /// <summary>
        ///   <para>Returns the height of an item in the <see cref="GridListControl" />
        /// .</para>
        /// </summary>
        /// <param name="index">The zero-based index of the item to return the height for. </param>
        /// <returns>
        ///   <para>The height, in pixels, of the specified item.</para>
        /// </returns>
        public int GetItemHeight(int index)
        {
            this.CheckIndex(index);
            return Grid.RowHeights[index + 1];
        }

        /// <summary>
        ///   <para>Returns the bounding rectangle for an item in the
        /// <see cref="GridListControl" />
        /// .</para>
        /// </summary>
        /// <param name="index">The zero-based index of an item whose bounding rectangle you want to return. </param>
        /// <returns>
        ///   <para>A <see cref="System.Drawing.Rectangle" /> that represents the bounding rectangle for the specified item.</para>
        /// </returns>
        public Rectangle GetItemRectangle(int index)
        {
            this.CheckIndex(index);
            return Grid.RangeInfoToRectangle(GridRangeInfo.Row(index + 1));
        }

        /// <summary>
        ///   <para>Returns a value indicating whether the specified item is selected.</para>
        /// </summary>
        /// <param name="index">The zero-based index of the item that determines whether it is selected. </param>
        /// <returns>
        ///   <para>
        ///     <see langword="True" /> If the
        /// specified item is currently selected in the <see cref="GridListControl" />; otherwise, <see langword="False" /> .</para>
        /// </returns>
        public bool GetSelected(int index)
        {
            this.CheckIndex(index);
            return Grid.Selections.Ranges.AnyRangeContains(GridRangeInfo.Row(index + 1));
        }

        /// <overload>
        ///   <para>Returns the zero-based index of the item
        /// at the specified coordinates.</para>
        /// </overload>
        /// <summary>
        ///   <para>Returns the zero-based index of the item
        /// at the specified coordinates.</para>
        /// </summary>
        /// <param name="p">A <see cref="System.Drawing.Point" /> object containing the coordinates used to obtain the item index. </param>
        /// <returns>
        ///   <para>The zero-based index of the item found at the specified coordinates; returns
        /// <see langword="ListBox.NoMatches" /> if no match is found.</para>
        /// </returns>
        public int IndexFromPoint(Point p)
        {
            int row, col;
            grid.PointToRowCol(p, out row, out col, -1);
            if (row != -1)
            {
                return row - 1;
            }

            return -1;
        }

        /// <summary>
        ///   <para>Returns the zero-based index of the item at the specified coordinates.</para>
        /// </summary>
        /// <param name="x">The x coordinate of the location to search. </param>
        /// <param name="y">The y coordinate of the location to search. </param>
        /// <returns>
        ///   <para>The zero-based index of the item found at the specified
        /// coordinates; returns <see langword="ListBox.NoMatches" />
        /// if no match is
        /// found.</para>
        /// </returns>
        public int IndexFromPoint(int x, int y)
        {
            return IndexFromPoint(new Point(x, y));
        }

        // ListControl overrides

        /// <override/>
        protected override void SetItemCore(int index, object value)
        {
            grid.InvalidateRange(GridRangeInfo.Row(index + 1));
        }

        /// <summary>
        /// Suspends updating the list control.
        /// </summary>
        public void BeginUpdate()
        {
            updateCount++;
        }

        /// <summary>
        /// Resumes updating the list control after a <see cref="BeginUpdate()"/>, calls PerformLayout in grid.
        /// </summary>
        public void EndUpdate()
        {
            if (--updateCount == 0)
            {
                this.SetItemsCore(null);
                PerformLayout();
            }
        }

        /// <summary>
        /// Gets a value indicating whether <see cref="BeginUpdate()"/> was called.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public virtual bool Updating
        {
            get
            {
                return updateCount > 0;
            }
        }

        /// <override/>
        protected override void SetItemsCore(IList items)
        {
            // && items != null)
            if (!inInitLayout && !inSetData && !Updating && DataManager != null)
            {
                ////inResizeToFit = true;
                grid.BeginUpdate();
                this.InitializeColumns(DataManager);
                int n = this.FillLastColumn ? 1 : 0;
                if (autoResize && grid.ColCount > n)
                {
                    grid.ColWidths.ResizeToFit(GridRangeInfo.Cells(0, 1, Math.Min(40, grid.RowCount), grid.ColCount - n), GridResizeToFitOptions.NoShrinkSize | GridResizeToFitOptions.IncludeHeaders);
                }

                grid.HideCols[0] = true;
                grid.RowHeights.ResizeToFit(GridRangeInfo.Row(0), GridResizeToFitOptions.NoShrinkSize | GridResizeToFitOptions.IncludeHeaders);
                Graphics g = CreateGraphics();
                ItemHeight = grid.CalculatePreferredCellSize(g, 1, 1, GridQueryBounds.Height).Height;
                g.Dispose();
                grid.EndUpdate(false);
                grid.Invalidate();
                ////inResizeToFit = false;
            }
        }

        /// <override/>
        /// <summary>Specifies the index of the selected item.</summary>
        [Category("Behavior")]
        [Description("Specifies the index of the selected item.")]
        public override int SelectedIndex
        {
            set
            {
                if (value != selectedIndex)
                {
                    SelectedIndexInternal = value;
                    if (SelectedIndexInternal != selectedIndex)
                    {
                        selectedIndex = SelectedIndexInternal;
                        OnSelectedIndexChanged(EventArgs.Empty);
                    }
                }
            }

            get
            {
                return selectedIndex;
            }
        }
        
        /// <summary>
        ///   <para>When overridden in a derived class, resynchronizes the data of
        /// the object at the specified index with the contents of the datasource.</para>
        /// </summary>
        /// <param name="index">
        ///   <para>The zero-based index of the item whose data to refresh.</para>
        /// </param>
        protected override void RefreshItem(int index)
        {
            Grid.ResetVolatileData();
            grid.InvalidateRange(GridRangeInfo.Row(index + 1));
        } // end of method RefreshItem

        bool inInitLayout = false;

        /// <override/>
        protected override void InitLayout()
        {
            inInitLayout = true;
            base.InitLayout();
            Grid.ViewLayout.Reset();
            Invalidate();
            inInitLayout = false;
            ////OnBindingContextChanged(EventArgs.Empty);
        }

        /// <override/>
        protected override /*Control*/ void OnBindingContextChanged(EventArgs e)
        {
            ////            if (!inInitLayout)
#if DEBUG
            if (Switches.GridListControl.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(Name);
            }
#else
            ;
#endif

            Grid.ResetVolatileData();
            ////Grid.UpdateScrollBars();
            base.OnBindingContextChanged(e);
            ////            Grid.ViewLayout.Reset();
            ////            Invalidate();
        }

        IGridListControlSource gridListControlSource = null;

        /// <summary>
        ///  Overridden. See the <see cref="System.Windows.Forms.ListControl.DataSourceChanged" /> event.
        /// </summary>
        /// <param name="e">A <see cref="System.EventArgs" /> that contains the event data.</param>
        protected override void OnDataSourceChanged(EventArgs e)
        {
#if DEBUG
            if (Switches.GridListControl.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(Name, DataSource);
            }
#else
            ;
#endif

            if (gridListControlSource != null)
            {
                gridListControlSource.ItemPropertiesChanged -= new EventHandler(gridListControlSource_ItemPropertiesChanged);
            }

            gridListControlSource = DataSource as IGridListControlSource;
            if (gridListControlSource != null)
            {
                gridListControlSource.ItemPropertiesChanged += new EventHandler(gridListControlSource_ItemPropertiesChanged);
            }

            Grid.ResetVolatileData();
            columnsDirty = true;
            base.OnDataSourceChanged(e);

            // Fixes defect 1348 - GridListControl become empty if you set MultiColumn = true before setting the datasource in VS 2005
            if (!isDisposing && IsHandleCreated)
            {
                BeginInvoke(new EventHandler(delayRefresh), new object[] { this, null });
            }
        }

        void delayRefresh(object sender, EventArgs e)
        {
            Refresh();
        }

        /// <summary>
        ///  Overridden. See the <see cref="System.Windows.Forms.ListControl.DisplayMemberChanged" /> event.
        /// </summary>
        /// <param name="e">A <see cref="System.EventArgs" /> that contains the event data.</param>
        protected override void OnDisplayMemberChanged(EventArgs e)
        {
#if DEBUG
            if (Switches.GridListControl.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(Name, this.DisplayMember);
            }
#else
            ;
#endif
            Grid.ResetVolatileData();
            ////Grid.UpdateScrollBars();
            base.OnDisplayMemberChanged(e);
        }

        /// <override/>
        protected override void OnSelectedIndexChanged(EventArgs e)
        {
#if DEBUG
            if (Switches.GridListControl.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(Name, this.SelectedIndex);
            }
#else
            ;
#endif

            base.OnSelectedIndexChanged(e);

            if (DataManager != null && DataManager.Position != this.SelectedIndex
                && this.SelectedIndex != -1)
            {
                DataManager.Position = this.SelectedIndex;
            }

            this.grid._AccessibilityNotifyClients(AccessibleEvents.Focus, SelectedIndex);
            this.grid._AccessibilityNotifyClients(AccessibleEvents.Selection, SelectedIndex);
        }

        /// <summary>
        ///   Overriden. See the <see cref="System.Windows.Forms.ListControl.SelectedValueChanged" /> event.
        /// </summary>
        /// <param name="e">A <see cref="System.EventArgs" /> that contains the event data.</param>
        protected override void OnSelectedValueChanged(EventArgs e)
        {
#if DEBUG
            if (Switches.GridListControl.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(Name, this.SelectedValue);
            }
#else
            ;
#endif

            base.OnSelectedValueChanged(e);
        } // end of method OnSelectedValueChanged

        /// <summary>
        ///   Overridden. See the <see cref="System.Windows.Forms.ListControl.ValueMemberChanged" /> event.
        /// </summary>
        /// <param name="e">A <see cref="System.EventArgs" /> that contains the event data.</param>
        protected override void OnValueMemberChanged(EventArgs e)
        {
#if DEBUG
            if (Switches.GridListControl.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(Name, this.ValueMember);
            }
#else
            ;
#endif
            Grid.ResetVolatileData();
            ////Grid.UpdateScrollBars();
            base.OnValueMemberChanged(e);
        }

        /// <summary>
        /// Refreshes the child view using the DataRelation to get to
        /// the child rows of the selected row.
        /// </summary>
        private void RefreshRows(CurrencyManager bmb)
        {
            //// Assuming it is bound to a DataTable / DataView.
            grid.CurrentCell.Deactivate(true);
            grid.ResetVolatileData();
            ////inResizeToFit = true;
            if (autoResize)
            {
                grid.ColWidths.ResizeToFit(GridRangeInfo.Rows(grid.TopRowIndex, grid.ViewLayout.LastVisibleRow), GridResizeToFitOptions.IncludeHeaders);
            }

            ////inResizeToFit = false;
            Grid.ViewLayout.Reset();
            grid.UpdateScrollBars();
            grid.Refresh();
        }

        void GridQueryRowCount(object sender, GridRowColCountEventArgs e)
        {
            if (ListManager != null)
            {
                e.Count = ListManager.Count;
                e.Handled = true;
            }
        }

        int imageColumn = 1;

        /// <summary>
        /// Gets or sets the column that should display any optional images from the image list.
        /// </summary>
        [DefaultValue(1)]
        [Description("The column that should display any optional images from the image list.")]
        [Category("Grid")]
        public int ImageColumn
        {
            get
            {
                return imageColumn;
            }
            
            set
            {
                imageColumn = value;
            }
        }
        
        void GridQueryCellInfo(object sender, GridQueryCellInfoEventArgs e)
        {
            if (columnsDirty)
            {
                this.InitializeColumns(this.DataManager);
            }

            int colNum = e.ColIndex - 1;
            if (ListManager != null && columns != null && colNum >= 0 && colNum < columns.Count)
            {
                GridStyleInfo colStyle = this.GetColumnStyle(colNum);
                PropertyDescriptor pd = colStyle.Tag as PropertyDescriptor;

                if (e.RowIndex < 0)
                {
                    // Column style.
                    e.Style.ModifyStyle(colStyle, Syncfusion.Styles.StyleModifyType.Override);
                    e.Handled = true;
                }
                else if (e.RowIndex == 0)
                {
                    // Column header.
                    e.Style.CellValue = pd.Name;
                    if (gridListControlSource != null)
                    {
                        GridStyleInfo style = gridListControlSource.GetStyle(e.RowIndex - 1, e.ColIndex);
                        e.Style.ModifyStyle(style, Syncfusion.Styles.StyleModifyType.Override);
                        e.Style.Identity.InnerIdentity = style.Identity;
                    }

                    e.Handled = true;
                }
                else if (e.RowIndex > 0 && e.RowIndex <= ListManager.Count)
                {
                    try
                    {
                        if (e.ColIndex == imageColumn && imageList != null && imageDescriptor != null)
                        {
                            e.Style.ImageIndex = Convert.ToInt32(imageDescriptor.GetValue(ListManager.List[e.RowIndex - 1]));
                            e.Style.ImageList = imageList;
                        }
                    }
                    catch (System.Exception ex)
                    {
                        TraceUtil.TraceExceptionCatched(ex);
                        if (!ExceptionManager.RaiseExceptionCatched(this, ex))
                        {
                            throw;
                        }
                    }

                    e.Style.CellValue = pd.GetValue(ListManager.List[e.RowIndex - 1]);
                    if (gridListControlSource != null)
                    {
                        GridStyleInfo style = gridListControlSource.GetStyle(e.RowIndex - 1, e.ColIndex);
                        e.Style.ModifyStyle(style, Syncfusion.Styles.StyleModifyType.Override);
                        e.Style.Identity.InnerIdentity = style.Identity;
                    }

                    e.Handled = true;
                }
            }
        }

        void GridPrepareViewStyleInfo(object sender, GridPrepareViewStyleInfoEventArgs e)
        {
            if (ListManager != null && e.RowIndex > 0)
            {
                if (e.ColIndex > 0)
                {
                    e.Style.TextMargins.Left = 2;
                    e.Style.TextMargins.Right = 2;
                    e.Style.TextMargins.Top = 2;

                    if (grid.HasControlFocus && grid.CurrentCell.HasCurrentCellAt(e.RowIndex))
                    {
                        ////e.Style.Interior = new BrushInfo(SystemColors.Highlight);
                        GridBorder thin = new GridBorder(GridBorderStyle.Dotted, SystemColors.Highlight);
                        GridBorder medium = new GridBorder(GridBorderStyle.Dotted, SystemColors.Highlight, GridBorderWeight.Thick);
                        if (grid.DefaultGridBorderStyle == GridBorderStyle.None)
                        {
                            e.Style.TextMargins.Top = 1;
                            e.Style.Borders.Top = thin;
                        }
                        else
                        {
                            e.Style.TextMargins.Top = 1;
                            e.Style.Borders.Top = thin;
                        }

                        e.Style.Borders.Bottom = medium;
                        if (e.ColIndex == 1)
                        {
                            e.Style.TextMargins.Left = 0;
                            e.Style.Borders.Left = medium;
                        }

                        if (e.ColIndex == grid.ColCount)
                        {
                            e.Style.Borders.Right = medium;
                            e.Style.TextMargins.Right = 0;
                        }
                    }
                }
            }
            else if (e.RowIndex == 0 && e.Style.Description.Length > 0)
            {
                // Alternative header can be specified with a description field.
                e.Style.CellValue = e.Style.Description;
            }
        }

        // UpdateData method...

        /// <summary>
        /// Returns the row count for the specified datasource.
        /// </summary>
        /// <param name="dataSource">The datasource, typed as <see cref="System.Object"/>.</param>
        /// <returns>The number of records in the datasource.</returns>
        public virtual int GetRowCount(object dataSource)
        {
            if (ListManager != null)
            {
                return ListManager.Count;
            }

            return 0;
        }

        /// <summary>
        /// Handles the <see cref="GridControlBase.CurrentCellMoving"/> event of the <see cref="Grid"/>.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        protected virtual void GridCurrentCellMoving(object sender, GridCurrentCellMovingEventArgs e)
        {
            e.ColIndex = 0;
            e.Options = GridSetCurrentCellOptions.BeginEndUpdate | GridSetCurrentCellOptions.NoSetFocus | GridSetCurrentCellOptions.NoSyncCurrentCell;
            if (!this.disableScrollInView)
            {
                e.Options |= GridSetCurrentCellOptions.ScrollInView;
            }

            if (this.selectionMode != System.Windows.Forms.SelectionMode.One)
            {
                e.Options |= GridSetCurrentCellOptions.NoSelectRange;
            }
        }

        /// <summary>
        /// Handles the <see cref="GridControlBase.CurrentCellActivating"/> event of the <see cref="Grid"/>.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        protected virtual void GridCurrentCellActivating(object sender, GridCurrentCellActivatingEventArgs e)
        {
            e.ColIndex = 0;
        }

        /// <summary>
        /// Handles the <see cref="GridControlBase.CurrentCellDeactivated"/> event of the <see cref="Grid"/>.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        protected virtual void GridCurrentCellDeactivated(object sender, GridCurrentCellDeactivatedEventArgs e)
        {
            // Check if Deactivate is called stand-alone or called from MoveTo and row is moving.
            if (!grid.CurrentCell.IsInMoveTo || e.RowIndex != grid.CurrentCell.MoveToRowIndex)
            {
                grid.RefreshRange(GridRangeInfo.Row(e.RowIndex), GridRangeOptions.MergeAllSpannedCells);
            }
        }

        /// <summary>
        /// Handles the <see cref="GridControlBase.CurrentCellActivated"/> event of the <see cref="Grid"/>.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        protected virtual void GridCurrentCellActivated(object sender, EventArgs e)
        {
            // Check if Activate is called stand-alone or called from MoveTo and row is moving.
            SelectedIndex = grid.CurrentCell.RowIndex - 1;
            if (!grid.CurrentCell.IsInMoveTo || grid.CurrentCell.RowIndex != grid.CurrentCell.MoveFromRowIndex || !grid.CurrentCell.MoveFromActiveState)
            {
                grid.RefreshRange(GridRangeInfo.Row(grid.CurrentCell.RowIndex), GridRangeOptions.MergeAllSpannedCells);
            }
        }

        /// <summary>
        /// Gets the <see cref="CurrencyManager"/> for the datasource in this list control.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        CurrencyManager ListManager
        {
            get
            {
                return this.DataManager;
            }
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        private int SelectedIndexInternal
        {
            get
            {
                return grid.CurrentCell.HasCurrentCell && grid.CurrentCell.RowIndex > 0
                    ? grid.CurrentCell.RowIndex - 1
                    : -1;
            }

            set
            {
#if DEBUG
                if (Switches.GridListControl.TraceVerbose)
                {
                    TraceUtil.TraceCurrentMethodInfo();
                }
#endif
                if (value != SelectedIndexInternal)
                {
                    if (value != -1)
                    {
                        grid.ForceCurrentCellMoveTo = true;
                        if (grid.CurrentCell.MoveTo(value + 1, 0)
                            && this.selectionMode == System.Windows.Forms.SelectionMode.One && value != -1)
                        {
                            grid.Selections.ChangeSelection(grid.Selections.Ranges.ActiveRange, GridRangeInfo.Row(value + 1));
                        }

                        grid.CurrentCell.ActivateOnGotFocus = true;
                    }
                    else
                    {
                        grid.CurrentCell.Deactivate(false);
                        grid.Selections.Clear();
                        grid.CurrentCell.ActivateOnGotFocus = false;
                    }
                }
            }
        }

        // List box like methods.

        /// <summary>
        ///   <para> Gets or sets the method in which items are selected in
        /// the <see cref="GridListControl" />
        /// .</para>
        /// </summary>
        [Category("Behavior"),
        DefaultValue(SelectionMode.One),
        Description("Indicates if the list box is to be single-select, multi-select, or unselectable.")]
        public virtual SelectionMode SelectionMode
        {
            set
            {
                if (!Enum.IsDefined(typeof(System.Windows.Forms.SelectionMode), value))
                {
                    throw new InvalidEnumArgumentException("value", (int)value, typeof(System.Windows.Forms.SelectionMode));
                }

                if (this.selectionMode != value)
                {
                    this.selectionMode = value;
                    grid.ListBoxSelectionMode = this.selectionMode;
                }
            }

            get
            {
                return this.selectionMode;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the grid should display only single or multiple columns.
        /// </summary>
        [Description("Indicates if the grid should display only single or multiple columns.")]
        [Category("Appearance")]
        public bool MultiColumn
        {
            get
            {
                return multiColumn;
            }

            set
            {
                if (multiColumn != value)
                {
                    multiColumn = value;
                    this.InitializeColumns(DataManager);
                    PerformLayout();
                    grid.Invalidate();
                    OnMultiColumnChanged(EventArgs.Empty);
                }
            }
        }
        
        /// <summary>
        /// Raises the <see cref="GridListControl.MultiColumnChanged"/> event.
        /// </summary>
        /// <param name="e">A <see cref="System.EventArgs" /> that contains the event data.</param>
        protected virtual void OnMultiColumnChanged(EventArgs e)
        {
#if DEBUG
            if (Switches.GridListControlEvents.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(Name, this.MultiColumn);
            }
#else
            ;
#endif
            if (MultiColumnChanged != null)
            {
                MultiColumnChanged(this, e);
            }
        }

        /// <summary>
        /// Gets or sets the first visible row index in the list control.
        /// </summary>
        [Description("The first visible row index in the list control.")]
        [Category("Grid")]
        public int TopIndex
        {
            get
            {
                return grid.TopRowIndex;
            }

            set
            {
                grid.TopRowIndex = value;
            }
        }

        /// <summary>
        ///   <para> Selects or clears the selection for the specified
        /// item in a <see cref="GridListControl" />
        /// .</para>
        /// </summary>
        /// <param name="index">The zero-based index of the item in a <see cref="GridListControl" /> to select or clear the selection. </param>
        /// <param name="value">
        /// <see langword="True" /> to select the specified item; otherwise, <see langword="False" /> . </param>
        public void SetSelected(int index, bool value)
        {
            if (value)
            {
                this.SelectedIndex = index;
                grid.Selections.Add(GridRangeInfo.Row(index + 1));
            }
            else
            {
                if (this.SelectedIndex == index)
                {
                    grid.CurrentCell.Deactivate(true);
                }

                grid.Selections.Remove(GridRangeInfo.Row(index + 1));
            }
        }

        /// <summary>
        /// Gets or sets the default row item height in the list control.
        /// </summary>
        [Description("The default row item height in the list control.")]
        [Category("Grid")]
        public int ItemHeight
        {
            get
            {
                return grid.Rows.DefaultSize;
            }

            set
            {
                grid.Rows.DefaultSize = value;
            }
        }

        /// <summary>
        /// Gets a list with items that are displayed in the list control.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public IList Items
        {
            get
            {
                return DataManager != null ? DataManager.List : null;
            }
        }

        /// <summary>
        /// Gets or sets the VisualStyles (skins) like Office2010, Office2007, Office2003
        /// </summary>
        [Browsable(true),
        DefaultValue(GridVisualStyles.SystemTheme),
        Category(@"Appearance")]
        [Description("Specifies look and feel skins for the Grid")]
        public GridVisualStyles GridVisualStyles
        {
            get
            {
                return grid.GridVisualStyles;
            }

            set
            {
                grid.GridVisualStyles = value;
            }
        }

        /// <summary>
        /// [Deprecated] Gets or sets the VisualStyles (skins) like Office2010, Office2007, Office2003
        /// </summary>
        [Browsable(true),
        DefaultValue(ColorStyles.SystemTheme),
        Category(@"Appearance")]
        [Description("[Deprecated] Specifies look and feel skins for the Grid")]
        public ColorStyles ColorStyles
        {
            get
            {
                return grid.ColorStyles;
            }

            set
            {
                grid.ColorStyles = value;
            }
        }

        /// <summary>
        /// Gets or sets the VisualStylesDrawing object
        /// </summary>
        [Browsable(false)]
        [Description("Gets or sets the VisualStylesDrawing object"),
        Category(@"Appearance")]
        public IVisualStylesDrawing GridVisualStylesDrawing
        {
            get
            {
                return grid.GridVisualStylesDrawing;
            }

            set
            {
                grid.GridVisualStylesDrawing = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether XP Themes (visual styles) should be used for this control when
        /// available.
        /// </summary>
        [DefaultValue(false),
        Category(@"Appearance")]
        [Description("Specifies whether XP Themes (visual styles) should be used for this control when available.")]
        public bool ThemesEnabled
        {
            get 
            { 
                return grid.ThemesEnabled; 
            }

            set
            {
                if (grid.ThemesEnabled != value)
                {
                    grid.ThemesEnabled = value;
                    this.OnThemeChanged(EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Enable or Disable the Legacy styles in the Table Model
        /// Value should be false to apply ColorStyles
        /// </summary>
        [Description("Allow Legacy Styles to Enable or Disable")]
        [Browsable(true), DefaultValue(true)]
        [Category("Appearance")]
        public bool ApplyVisualStyles
        {
            get
            {
                return Grid.Model.EnableLegacyStyle;
            }
            set
            {
                if (Grid.Model.EnableLegacyStyle != value)
                {
                    Grid.Model.EnableLegacyStyle = value;
                }
            }
        }
        /// <summary>
        /// Raises the <see cref="ThemeChanged"/> event.
        /// </summary>
        /// <param name="e">An EventArgs that contains the event data.</param>
        /// <remarks>
        /// <para>The OnThemeChanged method also allows derived classes to handle the event
        /// without attaching a delegate. This is the preferred technique for
        /// handling the event in a derived class.</para>
        /// <para>Notes to Inheritors:  When overriding OnThemeChanged in a derived
        /// class, be sure to call the base class's OnThemeChanged method so that
        /// registered delegates receive the event.</para>
        /// </remarks>
        protected virtual void OnThemeChanged(EventArgs e)
        {
#if DEBUG
            if (Switches.GridListControlEvents.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(Name, this.ThemesEnabled);
            }
#else
            ;
#endif
            if (this.ThemeChanged != null)
            {
                try
                {
                    this.ThemeChanged(this, e);
                }
                catch (Exception ex)
                {
                    TraceUtil.TraceExceptionCatched(ex);
                    if (!ExceptionManager.RaiseExceptionCatched(this, ex))
                    {
                        throw;
                    }
                }
            }
        }

        /// <summary>
        /// This event will be fired when the ThemesEnabled property is changed.
        /// </summary>
        [Category("Appearance")]
        [Description("This event will be fired when the ThemesEnabled property is changed.")]
        public event EventHandler ThemeChanged;

        /// <override/>
        protected override void SetBoundsCore(int x, int y, int width, int height, System.Windows.Forms.BoundsSpecified specified)
        {
            Grid.ViewLayout.Reset();
            Invalidate();
            base.SetBoundsCore(x, y, width, height, specified);
        }

        /// <override/>
        protected override void OnBackColorChanged(EventArgs e)
        {
            grid.ResetBackColor();
#if DEBUG
            if (Switches.GridListControl.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(Name, this.BackColor);
            }
#else
            ;
#endif
            base.OnBackColorChanged(e);
        }

        /// <override/>
        protected override void OnFontChanged(EventArgs e)
        {
            grid.TableStyle.Font.Size = this.Font.SizeInPoints;
            grid.TableStyle.Font.FontStyle = this.Font.Style;
#if DEBUG
            if (Switches.GridListControl.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(Name, this.Font);
            }
#else
            ;
#endif
            base.OnFontChanged(e);
        }

        /// <override/>
        /// <summary>Specifies the foreground color used to display text and graphics in the control.</summary>
        [Description(@"The foreground color used to display text and graphics in the control."),
        Category(@"Appearance")]
        public override Color ForeColor
        {
            get
            {
                if (grid == null)
                {
                    return base.ForeColor;
                }

                return grid.TableStyle.TextColor;
            }

            set
            {
                grid.TableStyle.TextColor = value;
                Refresh();
            }
        }

        void GridPaint(object sender, PaintEventArgs e)
        {
            OnPaint(e);
        } // end of method

        void GridMouseWheel(object sender, MouseEventArgs e)
        {
            OnMouseWheel(e);
        } // end of method OnMouseWheel

        void GridMouseUp(object sender, MouseEventArgs e)
        {
            OnMouseUp(e);
        } // end of method OnMouseUp

        bool disableScrollInView = false;

        void GridMouseMove(object sender, MouseEventArgs e)
        {
            // DisableScrollInView prevents the grid from scrolling while
            // position is selected by hovering the mouse over the list control.
            disableScrollInView = Control.MouseButtons == MouseButtons.None;
            try
            {
                OnMouseMove(e);
            }
            finally
            {
                disableScrollInView = false;
            }
        } // end of method OnMouseMove

        void GridBeforeMouseMove(object sender, CancelMouseEventArgs e)
        {
            // DisableScrollInView prevents the grid from scrolling while
            // position is selected by hovering the mouse over the list control.
            disableScrollInView = Control.MouseButtons == MouseButtons.None;
        } // end of method OnMouseMove

        void GridMouseHover(object sender, EventArgs e)
        {
            OnMouseHover(e);
        } // end of method OnMouseHover

        void GridMouseLeave(object sender, EventArgs e)
        {
            OnMouseLeave(e);
        } // end of method

        void GridMouseEnter(object sender, EventArgs e)
        {
            OnMouseEnter(e);
        } // end of method

        void GridMouseDown(object sender, MouseEventArgs e)
        {
            OnMouseDown(e);
        } // end of method

        void GridKeyUp(object sender, KeyEventArgs e)
        {
            OnKeyUp(e);
        } // end of method

        void GridKeyPress(object sender, KeyPressEventArgs e)
        {
            OnKeyPress(e);
        } // end of method

        void GridHelpRequested(object sender, HelpEventArgs hevent)
        {
            OnHelpRequested(hevent);
        } // end of method

        void GridGiveFeedback(object sender, GiveFeedbackEventArgs gfbevent)
        {
            OnGiveFeedback(gfbevent);
        } // end of method

        void GridDragDrop(object sender, DragEventArgs drgevent)
        {
            OnDragDrop(drgevent);
        } // end of method

        void GridDragLeave(object sender, EventArgs e)
        {
            OnDragLeave(e);
        } // end of method

        void GridDragOver(object sender, DragEventArgs drgevent)
        {
            OnDragOver(drgevent);
        } // end of method

        void GridDragEnter(object sender, DragEventArgs drgevent)
        {
            OnDragEnter(drgevent);
        } // end of method

        void GridDoubleClick(object sender, EventArgs e)
        {
            OnDoubleClick(e);
        } // end of method

        void GridClick(object sender, EventArgs e)
        {
            OnClick(e);
        } // end of method

        void GridContextMenuChanged(object sender, EventArgs e)
        {
            OnContextMenuChanged(e);
        } // end of method

        // Focus
        bool isActiveControl = false;
        bool isValidating = false;
        bool isDeactivatedCalled = false;
        bool hasControlFocus = false;
        bool isValidated = false;
        ////        bool isMousePressed = false;
        ////
        ////        void WmMouseDown(ref Message msg)
        ////        {
        ////            this.isMousePressed = true;
        ////            base.WndProc(ref msg);
        ////        }
        ////
        ////        void WmMouseUp(ref Message msg)
        ////        {
        ////            this.isMousePressed = false;
        ////            base.WndProc(ref msg);
        ////        }

        /// <override/>
        protected override void OnCausesValidationChanged(EventArgs e)
        {
            if (grid != null)
            {
                grid.CausesValidation = this.CausesValidation;
            }
        }

        void GridCausesValidationChanged(object sender, EventArgs e)
        {
            this.CausesValidation = grid.CausesValidation;
        }

        ////        protected override void OnStyleChanged(EventArgs e)
        ////        {
        ////            if (!GetStyle(ControlStyles.Selectable))
        ////                grid.WantKeys = false;
        ////        }
        ////

        ////        ///// <summary>
        ////        ///// Indicates if the the control is handling a <see cref="Control.MouseDown"/> event.
        ////        ///// </summary>
        ////        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        ////        public bool IsMousePressed
        ////        {
        ////            get
        ////            {
        ////                return isMousePressed;
        ////            }
        ////        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="OnValidating"/> method has been called. <see cref="OnLeave"/> and <see cref="OnEnter"/> reset this flag.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsValidating
        {
            get
            {
                return isValidating;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="OnValidated"/> method has been called. <see cref="OnLeave"/> and <see cref="OnEnter"/> reset this flag.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsValidated
        {
            get
            {
                return isValidated;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="OnEnter"/> has been called. <see cref="OnLeave"/> resets this flag.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsActiveControl
        {
            get
            {
                return isActiveControl;
            }
        }

        /// <summary>
        /// Gets a value indicating whether <see cref="OnDeactivated"/> has been called. <see cref="OnEnter"/> resets this flag.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsDeactivated
        {
            get
            {
                return isDeactivatedCalled;
            }
        }

        /// <summary>
        /// Gets a value indicating whether <see cref="OnControlGotFocus"/> has been called. <see cref="OnControlLostFocus"/> resets this flag.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasControlFocus
        {
            get
            {
                return hasControlFocus;
            }
        }

        /// <override/>
        protected override void OnEnter(EventArgs e)
        {
            isActiveControl = true;
            isValidating = false;
            isValidated = false;
#if DEBUG
            if (Switches.GridListControlEvents.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(this);
            }
#else
            ;
#endif
            base.OnEnter(e);
        }

        /// <override/>
        protected override void OnLeave(EventArgs e)
        {
            isActiveControl = false;
            isValidating = false;
#if DEBUG
            if (Switches.GridListControlEvents.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(this);
            }
#else
            ;
#endif
            base.OnLeave(e);

            if ((!this.CausesValidation || this.IsValidated) && !hasControlFocus)
            {
                OnDeactivated(e);
            }
        }

        /// <override/>
        protected override void OnValidating(CancelEventArgs e)
        {
            isValidating = true;
#if DEBUG
            if (Switches.GridListControlEvents.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(e.Cancel, this);
            }
#else
            ;
#endif

            base.OnValidating(e);
        }

        /// <override/>
        protected override void OnValidated(EventArgs e)
        {
            isValidating = false;
            isValidated = true;
#if DEBUG
            if (Switches.GridListControlEvents.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(this);
            }
#else
            ;
#endif
            base.OnValidated(e);

            if (!isActiveControl && !hasControlFocus)
            {
                OnDeactivated(e);
            }
        }

        /// <override/>
        protected override void OnLostFocus(EventArgs e)
        {
            RaiseControlLostFocus();
        }

        /// <override/>
        protected override void OnGotFocus(EventArgs e)
        {
            grid.Focus();
            RaiseControlGotFocus();
        }

        /// <summary>
        /// Occurs when both <see cref="OnControlLostFocus"/> and <see cref="OnLeave"/> occur.
        /// </summary>
        [Category("Focus")]
        [Description("Occurs when both OnControlLostFocus and OnLeave occur.")]
        public event EventHandler Deactivated;

        /// <summary>
        /// Raises the <see cref="Deactivated"/> event.
        /// </summary>
        /// <param name="e">An <see cref="EventArgs" /> that contains the event data.</param>
        protected virtual void OnDeactivated(EventArgs e)
        {
            isDeactivatedCalled = true;
#if DEBUG
            if (Switches.GridListControlEvents.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(this);
            }
#else
            ;
#endif
            if (Deactivated != null)
            {
                Deactivated(this, e);
            }

            if (grid != null)
            {
                grid.Invalidate();
            }
        }

        void ChildGotFocus(object sender, EventArgs e)
        {
            RaiseControlGotFocus();
        }

        void ChildLostFocus(object sender, EventArgs e)
        {
            RaiseControlLostFocus();
        }

        /// <summary>
        /// Determines if this control contains focus. Override this method if you
        /// want to show drop-down windows and indicate the control has not lost focus when
        /// the drop-down is shown.
        /// </summary>
        /// <returns>True if the control or any child control has focus; False otherwise.</returns>
        public virtual bool QueryFocusInside()
        {
            return ContainsFocus;
        }

        /// <summary>
        /// Raises the <see cref="Control.GotFocus"/> event. This method is called when the control
        /// or any child control has focus and did not have focus before.
        /// </summary>
        /// <remarks>
        /// Inheriting classes should override this method instead of overriding <see cref="Control.OnGotFocus"/>
        /// because <see cref="OnControlGotFocus"/> is also called when child controls get focus and it
        /// is not called when focus is moved within child controls of this control.
        /// </remarks>
        protected virtual void OnControlGotFocus()
        {
#if DEBUG
            if (Switches.GridListControlEvents.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(this);
            }
#else
            ;
#endif
            base.OnGotFocus(EventArgs.Empty);

            if (grid != null)
            {
                grid.Invalidate();
            }
        }

        /// <summary>
        /// Raises the <see cref="Control.LostFocus"/> event. This method is called when the control
        /// or any child control loses focus and the new focused control is not a child of this control.
        /// </summary>
        /// <remarks>
        /// Inheriting classes should override this method instead of overriding <see cref="Control.OnLostFocus"/>
        /// because <see cref="OnControlLostFocus"/> is also called when child controls lose focus and it
        /// is not called when focus is moved within child controls of this control.
        /// </remarks>
        protected virtual void OnControlLostFocus()
        {
#if DEBUG
            if (Switches.GridListControlEvents.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(this);
            }
#else
            ;
#endif

            base.OnLostFocus(EventArgs.Empty);

            if (!isActiveControl && !isValidating && !(CausesValidation && !this.isValidated))
            {
                OnDeactivated(EventArgs.Empty);
            }
            else
            {
                grid.CancelUpdate();

                if (isValidating)
                {
                    OnValidatingLostFocus();
                }
            }
        }

        /// <summary>
        /// This method is called if the control's <see cref="OnControlLostFocus"/> notification occurs
        /// while handling a <see cref="Control.Validating"/> event. This typically occurs if a
        /// message box is displayed from a <see cref="Control.Validating"/> event handler.
        /// </summary>
        protected virtual void OnValidatingLostFocus()
        {
            // Sometimes when users display a message box while the control is validated,
            // the message box is shown behind the application window and the user has
            // the impression the application is locked up.
            //
            // Pressing the <ALT> key will make the message box appear correctly in front of
            // the window.
            //
            // The following line emulates pressing the <ALT> key.
            //            SendKeys.Send("%");
        }

        void RaiseControlGotFocus()
        {
            if (!this.hasControlFocus)
            {
                hasControlFocus = true;
                OnControlGotFocus();
            }
        }

        void RaiseControlLostFocus()
        {
            if (!hasControlFocus)
            {
#if DEBUG
                if (Switches.GridListControlEvents.TraceVerbose)
                {
                    TraceUtil.TraceCurrentMethodInfo("-Duplicate call-", isValidating, this);
                }
#else
                ;
#endif
            }
            else if (!QueryFocusInside())
            {
                hasControlFocus = false;
                OnControlLostFocus();
            }
        }

        /// <override/>
        protected override void OnControlRemoved(System.Windows.Forms.ControlEventArgs e)
        {
#if DEBUG
            if (Switches.GridListControlEvents.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(e.Control, this);
            }
#else
            ;
#endif
            base.OnControlRemoved(e);

            e.Control.GotFocus -= new EventHandler(ChildGotFocus);
            e.Control.LostFocus -= new EventHandler(ChildLostFocus);
        }

        /// <override/>
        protected override void OnControlAdded(System.Windows.Forms.ControlEventArgs e)
        {
#if DEBUG
            if (Switches.GridListControlEvents.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(e.Control, this);
            }
#else
            ;
#endif
            base.OnControlAdded(e);

            e.Control.GotFocus += new EventHandler(ChildGotFocus);
            e.Control.LostFocus += new EventHandler(ChildLostFocus);
            _ControlAdded(e);
        }

        /// <override/>
        /// <summary>Specifies the Backcolor used to display text in the control.</summary>
        [Description(@"The Backcolor used to display text in the control."),
        AmbientValue(null),
        Category(@"Appearance"),
        RefreshProperties(RefreshProperties.All)]
        public override Color BackColor
        {
            get
            {
                return base.BackColor;
            }

            set
            {
                if (value.A != 255)
                {
                    this.SupportsTransparentBackColor = true;
                }

                base.BackColor = value;
            }
        }

        /// <summary>
        /// Resets the <see cref="BackColor"/> to its default value.
        /// </summary>
        public override void ResetBackColor()
        {
            BackColor = SystemColors.Window;
        }

        bool ShouldSerializeBackColor()
        {
            return BackColor != SystemColors.Window;
        }

        //
        //        /// <copyfrom cref="GridModelOptions.ExcelLikeSelectionFrame"/><summary>See <see cref="GridModelOptions.ExcelLikeSelectionFrame"/> in the GridModel class for information.</summary>
        //        [
        //        Browsable(true),
        //        DefaultValue(false),
        //        RefreshProperties(RefreshProperties.Repaint),
        //        ]
        //        [Description("Specifies whether the active selection should be outlined with a selection frame.")]
        //        public bool ExcelLikeSelectionFrame
        //        {
        //            get
        //            {
        //                return Grid.Model.Options.ExcelLikeSelectionFrame;
        //            }
        //            set
        //            {
        //                Grid.Model.Options.ExcelLikeSelectionFrame = value;
        //            }
        //        }

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
                if (!GetStyle(ControlStyles.SupportsTransparentBackColor) && value != null)
                {
                    SupportsTransparentBackColor = true;
                }

                Grid.BackgroundImage = value;
            }

            get
            {
                return Grid.BackgroundImage;
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
                    return Grid.IsSplitterPaneClosing ? base.Font : this.Parent.Font;
                }
                return Grid.IsSplitterPaneClosing ? base.Font : TableStyle.GdipFont;
            }

            set
            {
                if (value != null)
                {
                    hasFont = true;
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
        [Category("Appearance")]
        public GridStyleInfo TableStyle
        {
            get
            {
                return Grid.Model.TableStyle;
            }

            set
            {
                Grid.Model.TableStyle = value;
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

        /// <summary>
        /// Gets or sets the backcolor for header cells.
        /// </summary>
        [Browsable(true)]
        [Description("The backcolor for header cells.")]
        [RefreshProperties(RefreshProperties.All)]
        [Category("Appearance")]
        public Color HeaderBackColor
        {
            get
            {
                return grid.BaseStylesMap["Column Header"].StyleInfo.BackColor;
            }

            set
            {
                grid.BaseStylesMap["Column Header"].StyleInfo.BackColor = value;
                Refresh();
            }
        }

        bool ShouldSerializeHeaderBackColor()
        {
            return grid.BaseStylesMap["Column Header"].StyleInfo.HasInterior;
        }

        /// <summary>
        /// Resets the <see cref="HeaderBackColor"/> property.
        /// </summary>
        public void ResetHeaderBackColor()
        {
            grid.BaseStylesMap["Column Header"].StyleInfo.ResetInterior();
        }

        /// <summary>
        /// Gets or sets the text color for header cells.
        /// </summary>
        [Browsable(true)]
        [Description("The text color for header cells.")]
        [RefreshProperties(RefreshProperties.All)]
        [Category("Appearance")]
        public Color HeaderTextColor
        {
            get
            {
                return grid.BaseStylesMap["Column Header"].StyleInfo.TextColor;
            }

            set
            {
                grid.BaseStylesMap["Column Header"].StyleInfo.TextColor = value;
                Refresh();
            }
        }

        bool ShouldSerializeHeaderTextColor()
        {
            return grid.BaseStylesMap["Column Header"].StyleInfo.HasTextColor;
        }

        /// <summary>
        /// Resets the <see cref="HeaderTextColor"/> property.
        /// </summary>
        public void ResetHeaderTextColor()
        {
            grid.BaseStylesMap["Column Header"].StyleInfo.ResetTextColor();
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
                return Grid.Model.Options.MinResizeRowSize;
            }

            set
            {
                Grid.Model.Options.MinResizeRowSize = value;
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
                return Grid.Model.Options.MinResizeColSize;
            }

            set
            {
                Grid.Model.Options.MinResizeColSize = value;
            }
        }

        ////        ///// <copyfrom cref="GridModelOptions.SmoothControlResize"/><summary>See <see cref="GridModelOptions.SmoothControlResize"/> in the GridModel class for information.</summary>
        ////        [
        ////        Browsable(true),
        ////        DefaultValue(true),
        ////        ]
        ////        [Description("Defines whether a grid should be completely refreshed when the user resizes the window or if only newly visible rows or columns should be redrawn.")]
        ////        public bool SmoothControlResize
        ////        {
        ////            get
        ////            {
        ////                return Grid.Model.Options.SmoothControlResize;
        ////            }
        ////            set
        ////            {
        ////                Model.Options.SmoothControlResize = value;
        ////            }
        ////        }

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
                return Grid.Model.Properties;
            }

            set
            {
                Grid.Model.Properties = value;
                Refresh();
            }
        }

        bool ShouldSerializeProperties()
        {
            return Grid.Model.Properties.Modified;
        }

        /// <summary>
        /// Resets the <see cref="Properties"/> object to its default state.
        /// </summary>
        public void ResetProperties()
        {
            Grid.Model.Properties = new GridProperties();
            Grid.Model.Properties.BackgroundColor = SystemColors.Window;
            Grid.Model.Properties.ResetModified();
        }

        /// <copyfrom cref="GridModelOptions.ResizeRowsBehavior"/><summary>See <see cref="GridModelOptions.ResizeRowsBehavior"/> in the GridModel class for information.</summary>
        [Browsable(true),
        Editor(typeof(Syncfusion.Windows.Forms.Design.EnumFlagsEditor), typeof(UITypeEditor)),
        DefaultValue(GridResizeCellsBehavior.None)]
        [Description("Defines behavior for resizing rows.")]
        [Category("Behavior")]
        public GridResizeCellsBehavior ResizeRowsBehavior
        {
            get
            {
                return Grid.Model.Options.ResizeRowsBehavior;
            }

            set
            {
                Grid.Model.Options.ResizeRowsBehavior = value;
            }
        }

        ////        ///// <copyfrom cref="GridModelOptions.ResizeColsBehavior"/><summary>See <see cref="GridModelOptions.ResizeColsBehavior"/> in the GridModel class for information.</summary>
        ////        [
        ////        Browsable(true),
        ////        Editor(typeof(Syncfusion.Windows.Forms.Design.EnumFlagsEditor), typeof(UITypeEditor)),
        ////        DefaultValue(GridResizeCellsBehavior.InsideGrid|GridResizeCellsBehavior.ResizeSingle)
        ////        ]
        ////        [Description("Defines behavior for resizing columns.")]
        ////        public GridResizeCellsBehavior ResizeColsBehavior
        ////        {
        ////            get
        ////            {
        ////                return Grid.Model.Options.ResizeColsBehavior;
        ////            }
        ////            set
        ////            {
        ////                Grid.Model.Options.ResizeColsBehavior = value;
        ////            }
        ////        }

        /// <copyfrom cref="GridModelOptions.TransparentBackground"/><summary>See <see cref="GridModelOptions.TransparentBackground"/> in the GridModel class for information.</summary>
        [Browsable(true),
        DefaultValue(false),
        RefreshProperties(RefreshProperties.Repaint)]
        [Description("Defines whether the grid should erase and fill background of cells or only draw cell text.")]
        [Category("Appearance")]
        public bool TransparentBackground
        {
            get
            {
                return Grid.Model.Options.TransparentBackground;
            }

            set
            {
                Grid.Model.Options.TransparentBackground = value;
                Refresh();
            }
        }

        /// <copyfrom cref="GridModelOptions.AlphaBlendSelectionColor"/><summary>See <see cref="GridModelOptions.AlphaBlendSelectionColor"/> in the GridModel class for information.</summary>
        [Browsable(true)]
        [Description("Specifies the color for alphablended cell selections.")]
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Grid")]
        public Color AlphaBlendSelectionColor
        {
            get
            {
                return Grid.Model.Options.AlphaBlendSelectionColor;
            }

            set
            {
                Grid.Model.Options.AlphaBlendSelectionColor = value;
            }
        }

        /// <summary>
        /// Resets the <see cref="AlphaBlendSelectionColor"/> property.
        /// </summary>
        public void ResetAlphaBlendSelectionColor()
        {
            AlphaBlendSelectionColor = SystemColors.Highlight;
        }

        /// <summary>
        /// Specifies whether or not to serialize the AlphaBlendSelectionColor in the designer.
        /// </summary>
        /// <returns>True to serialize; False otherwise.</returns>
        public bool ShouldSerializeAlphaBlendSelectionColor()
        {
            return Grid.Model.Options.AlphaBlendSelectionColor != Color.FromArgb(64, SystemColors.Highlight)
                && (Grid.Model.Options.AllowSelection & GridSelectionFlags.AlphaBlend) != 0;
        }

        ////        ///// <copyfrom cref="GridModelOptions.ControllerOptionsChanged"/><summary>See <see cref="GridModelOptions.ControllerOptionsChanged"/> in the GridModel class for information.</summary>
        ////        [
        ////        Editor(typeof(Syncfusion.Windows.Forms.Design.EnumFlagsEditor), typeof(UITypeEditor)),
        ////        DefaultValue(GridControllerOptions.All)
        ////        ]
        ////        [Description("Specifies which mouse controllers should be enabled for the grid.")]
        ////        [RefreshProperties(RefreshProperties.All)]
        ////        public GridControllerOptions ControllerOptions
        ////        {
        ////            get
        ////            {
        ////                return Grid.Model.Options.ControllerOptions;
        ////            }
        ////            set
        ////            {
        ////                Grid.Model.Options.ControllerOptions = value;
        ////            }
        ////        }
        ////

        /// <summary>
        /// Returns the value for the ValueMember of the specified item.
        /// </summary>
        /// <param name="item">The row item.</param>
        /// <returns>The value of the ValueMember.</returns>
        public object GetItemValue(object item)
        {
            return ListUtil.GetItemValue(this.DataSource, this.ValueMember, item);
        }

        /// <summary>
        /// Gets or sets a value indicating whether the control should enable its Accessibility support.
        /// </summary>
        [Browsable(true),
        Category("Behavior"),
        Description("Specifies if the control should enable its Accessibility support."),
        DefaultValue(false)]
        public bool AccessibilityEnabled
        {
            get
            {
                return grid.AccessibilityEnabled;
            }

            set
            {
                grid.AccessibilityEnabled = value;
            }
        }

        BindingContext bindingContext;

        /// <override/>
        /// <summary>Gets or sets the <see cref="BindingContext"/> for the control.</summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override BindingContext BindingContext
        {
            get
            {
                if (bindingContext == null)
                {
                    bindingContext = new BindingContext();
                }

                return bindingContext;
            }

            set
            {
                if (bindingContext != value)
                {
                    bindingContext = value;
                    OnBindingContextChanged(EventArgs.Empty);
                }
            }
        }

        private void gridListControlSource_ItemPropertiesChanged(object sender, EventArgs e)
        {
            Grid.ResetVolatileData();
            columnsDirty = true;
        }
    }

    /// <internalonly/>
    /// <summary>Used internally.</summary>
    [ToolboxItem(false)]
    [Syncfusion.Documentation.DocumentationExclude]
    public class GridListControlModel : GridModel
    {
        internal GridListControl listControl;

        /// <summary>
        /// Default Constructor.
        /// </summary>
        public GridListControlModel()
            : base()
        {
        }

        /// <summary>
        /// Used internally.
        /// </summary>
        /// <param name="name">The name of the column.</param>
        /// <returns>
        /// The column index; or -1 if column could not be resolved.
        /// </returns>
        /// <internalonly/>
        public override int NameToColIndex(string name)
        {
            for (int n = 0; n < listControl.Grid.ColCount; n++)
            {
                PropertyDescriptor pd = listControl.GetColumnStyle(n).Tag as PropertyDescriptor;
                if (pd != null && pd.Name == name)
                {
                    return n + 1;
                }
            }

            return base.NameToColIndex(name);
        }

        /// <internalonly/>
        /// <summary>Gets Listcontrol. Used internally.</summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public GridListControl ListControl
        {
            get
            {
                return listControl;
            }
        }
    }

    /// <internalonly/>
    /// <summary>Used internally.</summary>
    [ToolboxItem(false)]
    [Syncfusion.Documentation.DocumentationExclude]
    public class GridListControlChild : GridControl
    {
        GridListControl listControl;
        GridListControlModel listControlModel;

        /// <internalonly/>
        /// <summary>Used internally.</summary>
        public GridListControlChild(GridListControl listControl)
            : this(listControl, new GridListControlModel())
        {
        }

        /// <internalonly/>
        /// <summary>Used internally.</summary>
        public GridListControlChild(GridListControl listControl, GridListControlModel model)
            : base(model)
        {
            this.Name = "gridControl";
            this.listControl = listControl;
            listControlModel = (GridListControlModel)Model;
            listControlModel.listControl = listControl;
        }

        /// <summary>
        /// Creates the list control item accessibility instance.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns>returns GridListControlItemAccessibleObject</returns>
        /// <internalonly/>
        protected internal virtual GridListControlItemAccessibleObject CreateListControlItemAccessibilityInstance(int index)
        {
            ////TraceUtil.TraceCurrentMethodInfo(index);
            return new GridListControlItemAccessibleObject(this, index);
        }

        /// <internalonly/>
        /// <summary>Gets ListControl. Used internally.</summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public GridListControl ListControl
        {
            get
            {
                return listControl;
            }
        }

        GridListControlItemAccessibleObjectsIndexer itemAccessibleObjects = null;

        /// <internalonly/>
        /// <summary>Gets Item AccessibleObjects. Used internally.</summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public GridListControlItemAccessibleObjectsIndexer ItemAccessibleObjects
        {
            get
            {
                if (itemAccessibleObjects == null)
                {
                    itemAccessibleObjects = new GridListControlItemAccessibleObjectsIndexer(this);
                }

                return itemAccessibleObjects;
            }
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
            if (this.AccessibilityEnabled)
            {
                // Overridden to return the custom AccessibleObject
                // for the entire grid.
                return new GridListControlChildAccessibleObject(this);
            }

            return base.CreateAccessibilityInstance();
        }

        internal void _AccessibilityNotifyClients(AccessibleEvents accEvent, int childID)
        {
            AccessibilityNotifyClients(accEvent, childID);
        }
    }

    // Making this public because the GridListControlChild was made public

    /// <internalonly/>
    /// <summary>Used internally.</summary>
    [Documentation.DocumentationExclude()]
    public class GridListControlChildAccessibleObject : Control.ControlAccessibleObject
    {
        GridListControl listControl;
        GridListControlChild gridChild;

        internal GridListControl ListControl
        {
            get
            {
                return listControl;
            }
        }

        /// <internalonly/>
        /// <summary>Used internally.</summary>
        public GridListControlChildAccessibleObject(GridListControlChild owner)
            : base(owner)
        {
            this.gridChild = owner;
            this.listControl = gridChild.ListControl;
        }

        // Gets the role for the grid. This is used by accessibility programs.

        /// <internalonly/>
        /// <summary>Used internally.</summary>
        public override AccessibleRole Role
        {
            get
            {
                return AccessibleRole.List;
            }
        }

        /// <internalonly/>
        /// <summary>Used internally.</summary>
        public override /*AccessibleObject*/ string Name
        {
            get
            {
                return this.listControl.AccessibleName;
            } // end of method get_Name
        }

        /// <internalonly/>
        /// <summary>Used internally.</summary>
        public override /*AccessibleObject*/ Rectangle Bounds
        {
            get
            {
                return this.gridChild.RectangleToScreen(this.gridChild.ClientRectangle);
            } // end of method get_Bounds
        }

        /// <internalonly/>
        /// <summary>Used internally.</summary>
        public override string Description
        {
            get
            {
                return this.listControl.AccessibleDescription;
            }
        }

        /// <internalonly/>
        /// <summary>Used internally.</summary>
        public override string Help
        {
            get
            {
                return string.Empty;
            }
        }

        /// <internalonly/>
        /// <summary>Used internally.</summary>
        public override AccessibleObject Parent
        {
            get
            {
                return listControl.AccessibilityObject;
            }
        }

        // Gets the state for the grid. This is used by accessibility programs.

        /// <internalonly/>
        /// <summary>Used internally.</summary>
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

        /// <summary>
        /// Used internally.
        /// </summary>
        /// <returns>
        /// The number of children belonging to an accessible object.
        /// </returns>
        /// <internalonly/>
        public override int GetChildCount()
        {
            return listControl.Items.Count;
        }

        // Gets the Accessibility object of the cell idetified by index.

        /// <summary>
        /// Used internally.
        /// </summary>
        /// <param name="index">The zero-based index of the accessible child.</param>
        /// <returns>
        /// An <see cref="T:System.Windows.Forms.AccessibleObject"/> that represents the accessible child corresponding to the specified index.
        /// </returns>
        /// <internalonly/>
        public override AccessibleObject GetChild(int index)
        {
            if (index < listControl.Items.Count)
            {
                return gridChild.ItemAccessibleObjects[index];
            }

            return null;
        }

        /// <internalonly/>
        /// <summary>Used internally.</summary>
        public override string Value
        {
            get
            {
                return listControl.Text;
            }

            set
            {
                listControl.Text = value;
            }
        }
        
        // Helper function that is used by the GridListControlItemAccessibleObject's accessibility object
        // to navigate between sibiling controls. Specifically, this function is used in
        // the GridListControlItemAccessibleObject.Navigate function.
        internal AccessibleObject NavigateFromChild(GridListControlItemAccessibleObject child, AccessibleNavigation navdir)
        {
            int index = child.Index;

            switch (navdir)
            {
                case AccessibleNavigation.FirstChild:
                    index = 0;
                    break;

                case AccessibleNavigation.LastChild:
                    index = listControl.Items.Count - 1;
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
                    if (index < listControl.Items.Count)
                    {
                        index++;
                    }

                    break;
            }

            return this.GetChild(index);
        }

        // Helper function that is used by the grid's accessibility object
        // to select a specific grid control. Specifically, this function is used
        // in the grid.gridAccessibleObject.Select function.
        internal void SelectChild(GridListControlItemAccessibleObject child, AccessibleSelection selection)
        {
            int index = child.Index;

            ////To simulate a click AccessibleSelection.TakeFocus|AccessibleSelection.TakeSelection.
            ////To select a target item by simulating CTRL + click AccessibleSelection.TakeFocus|AccessibleSelection.AddSelection.
            ////To cancel selection of a target item by simulating CTRL + click AccessibleSelection.TakeFocus|AccessibleSelection.RemoveSelection.
            ////To simulate SHIFT + click AccessibleSelection.TakeFocus|AccessibleSelection.ExtendSelection.

            ////To select a range of objects and put focus on the last object Specify AccessibleSelection.TakeFocus on the starting object to set the selection anchor. Then call Select again and specify AccessibleSelection.TakeFocus|AccessibleSelection.ExtendSelection on the last object.
            ////To deselect all objects Specify AccessibleSelection.TakeSelection on any object. This flag deselects all selected objects except the one just selected. Then call Select again and specify AccessibleSelection.RemoveSelection on the same object.

            //// Determine which selection action should occur, based on the
            //// AccessibleSelection value.
            if ((selection & AccessibleSelection.TakeFocus) != 0)
            {
                if ((selection & AccessibleSelection.TakeSelection) != 0)
                {
                    this.listControl.SelectedIndex = index;
                    this.listControl.SetSelected(index, true);
                }

                if ((selection & AccessibleSelection.AddSelection) != 0)
                {
                    this.listControl.SetSelected(index, true);
                }

                if ((selection & AccessibleSelection.RemoveSelection) != 0)
                {
                    this.listControl.SetSelected(index, false);
                }

                if ((selection & AccessibleSelection.ExtendSelection) != 0)
                {
                    int index1 = this.listControl.SelectedIndex;
                    if (index1 < index)
                    {
                        for (int i = index1; i <= index; i++)
                        {
                            this.listControl.SetSelected(i, true);
                        }
                    }
                    else
                    {
                        for (int i = index; i <= index1; i++)
                        {
                            this.listControl.SetSelected(i, true);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Used internally.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Windows.Forms.AccessibleObject"/> that specifies the currently focused child. This method returns the calling object if the object itself is focused. Returns null if no object has focus.
        /// </returns>
        /// <internalonly/>
        public override AccessibleObject GetFocused()
        {
            if (this.gridChild.Focused)
            {
                return GetSelected();
            }
            else
            {
                return base.GetFocused();
            }
        }

        /// <summary>
        /// Used internally.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Windows.Forms.AccessibleObject"/> that represents the currently selected child. This method returns the calling object if the object itself is selected. Returns null if is no child is currently selected and the object itself does not have focus.
        /// </returns>
        /// <internalonly/>
        public override AccessibleObject GetSelected()
        {
            if (listControl.SelectedIndex != -1)
            {
                return GetChild(listControl.SelectedIndex + base.GetChildCount());
            }

            return base.GetSelected();
        }

        /// <summary>
        /// Used internally.
        /// </summary>
        /// <param name="x">The horizontal screen coordinate.</param>
        /// <param name="y">The vertical screen coordinate.</param>
        /// <returns>
        /// An <see cref="T:System.Windows.Forms.AccessibleObject"/> that represents the child object at the given screen coordinates. This method returns the calling object if the object itself is at the location specified. Returns null if no object is at the tested location.
        /// </returns>
        /// <internalonly/>
        public override AccessibleObject HitTest(int x, int y)
        {
            Point point = gridChild.PointToClient(new Point(x, y));
            GridRangeInfo range = gridChild.PointToRangeInfo(point);
            if (!range.IsEmpty && range.Top > 0)
            {
                return gridChild.ItemAccessibleObjects[range.Top];
            }

            return base.HitTest(x, y);
        }

        /// <summary>
        /// Used internally.
        /// </summary>
        /// <param name="navdir">One of the <see cref="T:System.Windows.Forms.AccessibleNavigation"/> values.</param>
        /// <returns>
        /// An <see cref="T:System.Windows.Forms.AccessibleObject"/> that represents one of the <see cref="T:System.Windows.Forms.AccessibleNavigation"/> values.
        /// </returns>
        /// <internalonly/>
        public override AccessibleObject Navigate(AccessibleNavigation navdir)
        {
            GridListControlItemAccessibleObject accObj = GetSelected() as GridListControlItemAccessibleObject;
            if (accObj != null)
            {
                return this.NavigateFromChild(accObj, navdir);
            }

            return base.Navigate(navdir);
        }
    }

    // Making this public because the GridListControlChild was made public.

    /// <internalonly/>
    /// <summary>Used internally.</summary>
    [Documentation.DocumentationExclude()]
    public class GridListControlItemAccessibleObject : AccessibleObject
    {
        GridListControl listControl;
        GridListControlChild gridChild;
        int index;

        /// <internalonly/>
        /// <summary>Used internally.</summary>
        public GridListControlItemAccessibleObject(GridListControlChild grid, int index)
        {
            this.gridChild = grid;
            this.listControl = grid.ListControl;
            this.index = index;
        }

        /// <internalonly/>
        /// <summary>Gets Index.Used internally.</summary>
        public int Index
        {
            get
            {
                return index;
            }
        }

        internal GridListControlChildAccessibleObject GridAccessibilityObject
        {
            get
            {
                return gridChild.AccessibilityObject as GridListControlChildAccessibleObject;
            }
        }

        /// <internalonly/>
        /// <summary>Used internally.</summary>
        public override /*AccessibleObject*/ void Select(AccessibleSelection flags)
        {
            if ((flags & AccessibleSelection.TakeFocus) != 0)
            {
                if (!gridChild.Focused)
                {
                    gridChild.Focus();
                }
            }

            if ((flags & AccessibleSelection.TakeSelection) != 0)
            {
                GridAccessibilityObject.SelectChild(this, flags);
            }
        } // end of method Select

        /// <summary>
        /// Navigate to the next or previous grid entry.
        /// </summary>
        /// <param name="navdir">The entry to navigate to.</param>
        /// <returns>The child object.</returns>
        public override /*AccessibleObject*/ AccessibleObject Navigate(AccessibleNavigation navdir)
        {
            return GridAccessibilityObject.NavigateFromChild(this, navdir);
        } // end of method Navigate
        
        /// <internalonly/>
        /// <summary>Used internally.</summary>
        public override /*AccessibleObject*/ void DoDefaultAction()
        {
            this.Select((AccessibleSelection.TakeSelection | AccessibleSelection.TakeFocus));
        } // end of method DoDefaultAction

        /// <summary>
        /// Returns the currently focused child, if any.
        /// Returns this if the object itself is focused.
        /// </summary>
        /// <returns>The currently focused child.</returns>
        public override /*AccessibleObject*/ AccessibleObject GetFocused()
        {
            return this.GridAccessibilityObject.GetFocused();
        } // end of method GetFocused
        
        /// <internalonly/>
        /// <summary>Used internally.</summary>
        public override /*AccessibleObject*/ AccessibleStates State
        {
            get
            {
                AccessibleStates accessibleStates = AccessibleStates.Selectable | AccessibleStates.Focusable;
                if (listControl.GetSelected(index))
                {
                    accessibleStates |= AccessibleStates.Selected;
                }

                if (listControl.SelectedIndex == index)
                {
                    accessibleStates |= AccessibleStates.Focused;
                }

                if (index + 1 < gridChild.TopRowIndex || index + 1 > gridChild.ViewLayout.LastVisibleRow)
                {
                    accessibleStates |= AccessibleStates.Offscreen | AccessibleStates.Invisible;
                }

                if (listControl.SelectionMode != SelectionMode.One)
                {
                    accessibleStates |= AccessibleStates.MultiSelectable;
                }

                return accessibleStates;
            } // end of method get_State
        }

        /// <internalonly/>
        /// <summary>Used internally.</summary>
        public override /*AccessibleObject*/ AccessibleRole Role
        {
            get
            {
                return AccessibleRole.ListItem;
            } // end of method get_Role
        }

        /// <internalonly/>
        /// <summary>Used internally.</summary>
        public override /*AccessibleObject*/ AccessibleObject Parent
        {
            get
            {
                return gridChild.AccessibilityObject;
            } // end of method get_Parent
        }

        /// <internalonly/>
        /// <summary>Used internally.</summary>
        public override /*AccessibleObject*/ string Name
        {
            get
            {
                return listControl.Items[index].ToString();
            } // end of method get_Name
        }

        /// <internalonly/>
        /// <summary>Used internally.</summary>
        public override /*AccessibleObject*/ string DefaultAction
        {
            get
            {
                return "Click";
            } // end of method get_DefaultAction
        }

        /// <internalonly/>
        /// <summary>Used internally.</summary>
        public override /*AccessibleObject*/ Rectangle Bounds
        {
            get
            {
                return this.gridChild.RectangleToScreen(gridChild.RangeInfoToRectangle(GridRangeInfo.Row(index + 1)));
            } // end of method get_Bounds
        }

        /// <internalonly/>
        /// <summary>Used internally.</summary> 
        public override string Description
        {
            get
            {
                return listControl.GetItemText(listControl.Items[Index]);
            }
        }
    }

    // Making this public because the GridListControlChild was made public.

    /// <internalonly/>
    /// <summary>Used internally.</summary>
    [Documentation.DocumentationExclude()]
    public class GridListControlItemAccessibleObjectsIndexer
    {
        ArrayList data;
        GridListControl listControl;
        GridListControlChild gridChild;

        internal GridListControlItemAccessibleObjectsIndexer(GridListControlChild grid)
        {
            this.gridChild = grid;
            this.listControl = gridChild.ListControl;
            this.data = new ArrayList();
        }

        internal GridListControlItemAccessibleObject GetItem(int index)
        {
            // Returns NULL if listControlItem is not found.
            return index < data.Count ? data[index] as GridListControlItemAccessibleObject : null;
        }

        internal void SetItem(int index, GridListControlItemAccessibleObject accObj)
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

        /// <internalonly/>
        /// <summary>Used internally.</summary>
        [Browsable(false)]
        public GridListControlItemAccessibleObject /*IGridData*/ this[int index]
        {
            get
            {
                GridListControlItemAccessibleObject accObj = GetItem(index);
                if (accObj == null)
                {
                    accObj = gridChild.CreateListControlItemAccessibilityInstance(index);
                    // Save weak reference to object.
                    SetItem(index, accObj);
                }

                return accObj;
            }
        }
    }
    
    /// <internalonly/>
    [Documentation.DocumentationExclude()]
    public interface IGridListControlSource
    {
        /// <summary>
        /// Gets the width.
        /// </summary>
        /// <param name="columnIndex">Index of the column.</param>
        /// <returns>returns width</returns>
        /// <internalonly/>
        int GetWidth(int columnIndex);

        /// <summary>
        /// Gets the style.
        /// </summary>
        /// <param name="recordNum">The record num.</param>
        /// <param name="columnIndex">Index of the column.</param>
        /// <returns>returns grid style info</returns>
        /// <internalonly/>
        GridStyleInfo GetStyle(int recordNum, int columnIndex);

        /// <internalonly/>
        /// <summary>
        /// Used internally.
        /// </summary>
        void SetWidth(int columnIndex, int width);

        /// <internalonly/>
        /// <summary>
        /// Used internally.
        /// </summary>
        event EventHandler ItemPropertiesChanged;

        /// <summary>
        /// Gets the visible column count.
        /// </summary>
        /// <returns>returns visible column count</returns>
        /// <internalonly/>
        int GetVisibleColumnCount();
    }
    
    /// <internalonly/>
    [Syncfusion.Documentation.DocumentationExclude()]
    internal class GridListControlDesigner : ControlDesigner
    {
#if SyncfusionFramework2_0

        DesignerActionListCollection actionLists;
        // Properties
        public override DesignerActionListCollection ActionLists
        {
            get
            {
                if (this.actionLists == null)
                {
                    actionLists = new DesignerActionListCollection();
                    actionLists.Add(new GridListControlChooseDataSourceActionList(Component));
                    actionLists.Add(new DesignerActionSupportList(Component, null));
                }

                return this.actionLists;
            }
        }

#endif
    }

#if SyncfusionFramework2_0
    internal class GridListControlChooseDataSourceActionList : DesignerActionList
    {
        // Methods
        public GridListControlChooseDataSourceActionList(IComponent component)
            : base(component)
        {
        }

        public override DesignerActionItemCollection GetSortedActionItems()
        {
            DesignerActionItemCollection collection1 = new DesignerActionItemCollection();
            collection1.Add(new DesignerActionHeaderItem("Data", "Data"));
            DesignerActionPropertyItem item1 = new DesignerActionPropertyItem("DataSource", "Choose DataSource", "Data");
            item1.RelatedComponent = Component;
            collection1.Add(item1);
            return collection1;
        }

        // Properties
        [AttributeProvider(typeof(IListSource))]
        public object DataSource
        {
            get
            {
                return Grid.DataSource;
            }

            set
            {
                IDesignerHost host1 = Component.Site.GetService(typeof(IDesignerHost)) as IDesignerHost;
                PropertyDescriptor descriptor1 = TypeDescriptor.GetProperties(Grid)["DataSource"];
                IComponentChangeService service1 = Component.Site.GetService(typeof(IComponentChangeService)) as IComponentChangeService;
                object[] objArray1 = new object[1] { Grid.Name };
                DesignerTransaction transaction1 = host1.CreateTransaction("GridListControlChooseDataSourceTransactionString " + objArray1.ToString());
                try
                {
                    service1.OnComponentChanging(Component, descriptor1);
                    Grid.DataSource = value;
                    service1.OnComponentChanged(Component, descriptor1, null, null);
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

        ////Properties
        GridListControl Grid
        {
            get { return Component as GridListControl; }
        }
    }
#endif
}
