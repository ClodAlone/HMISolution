//-------------------------------------------------------------------------------------------------
// <copyright file="GridGroupDropArea.cs" company="syncfusion">
// Copyright (c) Syncfusion Inc. 2001 - 2014. All rights reserved.
//
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
// </copyright>
//-------------------------------------------------------------------------------------------------

using System;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using Syncfusion.Collections;
using Syncfusion.Diagnostics;
using Syncfusion.Drawing;
using Syncfusion.Grouping;
using Syncfusion.Windows.Forms;
using Syncfusion.Windows.Forms.Grid;
using System.Collections.Generic;

namespace Syncfusion.Windows.Forms.Grid.Grouping
{
    /// <summary>
    /// The GridGroupDropArea class implements a control that is bound to a <see cref="GridTableDescriptor"/>.
    /// It lets the user drag and drop column headers into its bounds and change the <see cref="TableDescriptor.GroupedColumns"/>
    /// at runtime.
    /// </summary>
    public class GridGroupDropArea : GridControlBase
    {
        static int tableCounter = 0;
        int tableId = 0;
        private GridTableControl gridTableControl;
        GridVisualStyles visualStyle;
        bool penChanged = false;
        private TreeLinePlacement treeLinePlacement = TreeLinePlacement.Bottom;
        /*
                /// <overload>
                /// Initializes the control
                /// </overload>
                /// <summary>
                /// Initializes the control
                /// </summary>
                public GridGroupDropArea()
                    : this(new GridGroupDropAreaModel())
                {
                    tableId = ++tableCounter;
                    if (Engine.VerboseEnsureObjectLifeTime)
                        TraceUtil.TraceCurrentMethodInfo(tableId);
                }
        */

        /// <override/>
        protected override void Dispose(bool disposing)
        {
            if (Engine.VerboseEnsureObjectLifeTime)
            {
                TraceUtil.TraceCurrentMethodInfo(Model != null ? Model.ToString() : "Model = null", tableId);
            }

            if (disposing)
            {
                UnwireModel();
                this.Model.Dispose();
            }

            base.Dispose(disposing);
        }

        Color treeLineColor; 
         /// <summary>
        /// Gets or sets the color of the TreeLines
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Color TreeLineColor
        {
            get
            {
                return treeLineColor;
            } 
            set
            {
                if (treeLineColor != value)
                {
                    if (this.GroupingControl != null)
                    {
                        for (int i = 0; i < this.GroupingControl.GroupDropPanel.Controls.Count; i++)
                        {
                            GridGroupDropArea groupDropArea = (GridGroupDropArea)this.GroupingControl.GroupDropPanel.Controls[i];
                            groupDropArea.treeLineColor = value;
                        }
                    }
                    else
                        treeLineColor = value;
                    penChanged = !penChanged;
                }
               
            }
        }

        /// <summary>
        /// Gets or Sets the TreeLinePlacement in the GroupDropArea
        /// </summary>
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Syncfusion.Documentation.DocumentationExclude()]
        [System.Xml.Serialization.XmlIgnore]
        public TreeLinePlacement TreeLinePlacement
        {
            get
            {
                return this.treeLinePlacement;
            }
            set
            {
                if (this.treeLinePlacement != value)
                {
                    if (this.GroupingControl != null)
                    {
                        for (int i = 0; i < this.GroupingControl.GroupDropPanel.Controls.Count; i++)
                        {
                            GridGroupDropArea groupDropArea = (GridGroupDropArea)this.GroupingControl.GroupDropPanel.Controls[i];
                            groupDropArea.treeLinePlacement = value;
                        }
                    }
                    else
                        treeLinePlacement = value;
                }
            }
        }

        private bool allowRemove = false;
        /// <summary>
        /// Gets / sets whether the <see cref="GridGroupDropArea"/> should support removal of Groups dynamically.
        /// </summary>
        [Category("Grouping Control")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [DefaultValue(false)]
        public bool AllowRemove
        {
            get
            {
                return allowRemove;
            }
            set
            {
                if (allowRemove != value)
                {
                    if (this.GroupingControl != null && this.GroupingControl.GroupDropPanel.Controls.Count>1)
                    {
                        for (int i = 0; i < this.GroupingControl.GroupDropPanel.Controls.Count; i++)
                        {
                            GridGroupDropArea groupDropArea = (GridGroupDropArea)this.GroupingControl.GroupDropPanel.Controls[i];
                            groupDropArea.allowRemove = value;
                        }
                    }
                    else
                        allowRemove = value;
                }
                if (allowRemove)
                    this.Model.GridTableModel.UpdateColumnWidths(true);
            }
        }

        private bool dynamicResizing = false;
        /// <summary>
        /// Gets or sets the value to resize the GroupDropArea dynamically
        /// Not applicable if GridGroupingControl has multiple GroupDropAreas
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool DynamicResizing
        {
            get
            {
                return dynamicResizing;
            }
            set
            {

                dynamicResizing = value;
            }
        }

        /// <summary>
        /// Returns the <see cref="GridGroupingControl"/> that hosts this control.
        /// </summary>
        [ReadOnly(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public GridGroupingControl GroupingControl
        {
            get
            {
                Control c = Parent;
                while (c != null)
                {
                    if (c is GridGroupingControl)
                    {
                        return (GridGroupingControl)c;
                    }

                    c = c.Parent;
                }

                return null;
            }
        }

        bool inWndProc = false;

        /// <internalonly/>
        [Syncfusion.Documentation.DocumentationExclude]
        protected override void WndProc(ref Message msg)
        {
            bool oldInWndProc = inWndProc;
            inWndProc = true;
            base.WndProc(ref msg);
            inWndProc = oldInWndProc;
            ////            if (!inWndProc && !ParentDesignMode)
            ////            {
            ////                GridTableControl tableControl = Model.ActiveGridView as GridTableControl;
            ////                if (tableControl != null)
            ////                    tableControl.SynchronizeGridWithEngine();
            ////            }
        }

        internal bool ParentDesignMode
        {
            get
            {
#if EMUDESIGN
                return true;
#endif

                Control parent = Parent;
                while (parent != null)
                {
                    ISite site = Parent.Site;
                    if (site != null)
                    {
                        return site.DesignMode;
                    }

                    if (parent is GridGroupingControl)
                    {
                        return ((GridGroupingControl)parent).InDesigner;
                    }

                    parent = parent.Parent;
                }

                return DesignMode;
            }
        }

        /// <internalonly/>
        [Syncfusion.Documentation.DocumentationExclude]
        protected override void OnBeforePaint(EventArgs e)
        {
            if (!this.ParentDesignMode)
            {
                GridTableControl tableControl = Model.ActiveGridView as GridTableControl;
                if (tableControl != null)
                {
                    tableControl.SynchronizeGridWithEngine();
                }
            }

            base.OnBeforePaint(e);
        }

        /// <internalonly/>
        /// <summary>Used internally.</summary>
        [Syncfusion.Documentation.DocumentationExclude]
        public override void Invalidate()
        {
            Model.ResetVolatileData();
            base.Invalidate();
        }

        /// <summary>
        /// Initializes the control
        /// </summary>
        /// <param name="tableControl">The table control.</param>
        /// <param name="model">The group drop area model.</param>
        public GridGroupDropArea(GridTableControl tableControl, GridGroupDropAreaModel model)
            : base(model)
        {
            this.VScrollBehavior = GridScrollbarMode.Disabled;
            this.HScrollBehavior = GridScrollbarMode.Disabled;
            this.ThemesEnabled = true;
            this.WantKeys = false;
            SetStyle(ControlStyles.Selectable, false);
            Model.ActiveGridView = this;
            this.MouseControllerDispatcher.Add(new GroupDropAreaDragHeaderMouseController(this));
            if (tableControl != null)
            {
                gridTableControl = tableControl;
                tableControl.GroupDropArea = this;
            }

            Model.TableStyle.WrapText = false;
            Model.TableStyle.Trimming = StringTrimming.EllipsisWord;
            tableId = ++tableCounter;
            if (Engine.VerboseEnsureObjectLifeTime)
            {
                TraceUtil.TraceCurrentMethodInfo(tableId);
            }
        }

        /// <summary>
        /// Used internally.
        /// </summary>
        /// <param name="point">The point value.</param>
        /// <returns>returns the GridColumnDescriptor</returns>
        /// <internalonly/>
        [Syncfusion.Documentation.DocumentationExclude]
        public GridColumnDescriptor GetHeaderColumnDescriptorAt(Point point)
        {
            return Model.GetHeaderColumnDescriptorAt(PointToRangeInfo(point));
        }

        /// <internalonly/>
        /// <summary>Used internally.</summary>
        [Syncfusion.Documentation.DocumentationExclude]
        [System.Xml.Serialization.XmlIgnore]
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new GridScrollbarMode VScrollBehavior
        {
            get
            { 
                return base.VScrollBehavior; 
            }
           
            set
            { 
                base.VScrollBehavior = value; 
            }
        }

        /// <internalonly/>
        /// <summary>Used internally.</summary>
        [Syncfusion.Documentation.DocumentationExclude]
        [System.Xml.Serialization.XmlIgnore]
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new GridScrollbarMode HScrollBehavior
        {
            get
            { 
                return base.HScrollBehavior; 
            }
            
            set
            { 
                base.HScrollBehavior = value; 
            }
        }

        bool first = true;

        /// <internalonly/>
        /// <summary>Used internally.</summary>
        [Syncfusion.Documentation.DocumentationExclude]
        public new GridGroupDropAreaModel Model
        {
            get
            {
                return (GridGroupDropAreaModel)base.Model;
            }

            set
            {
                base.Model = value;
            }
        }

        /// <internalonly/>
        [Syncfusion.Documentation.DocumentationExclude]
        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (first)
            {
                ////this.MouseControllerDispatcher.Add(new GroupDropAreaDragHeaderMouseController(this));
                first = false;
            }

            base.OnMouseDown(e);
        }

        /// <internalonly/>
        [Syncfusion.Documentation.DocumentationExclude]
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
        }

        /// <internalonly/>
        [Syncfusion.Documentation.DocumentationExclude]
        protected override void OnMouseEnter(EventArgs e)
        {
            ViewLayout.Reset();
            base.OnMouseEnter(e);
        }

        /// <internalonly/>
        [Syncfusion.Documentation.DocumentationExclude]
        protected override void WireModel()
        {
            base.WireModel();
        }

        /// <internalonly/>
        [Syncfusion.Documentation.DocumentationExclude]
        protected override void UnwireModel()
        {
            base.UnwireModel();
        }

        /// <internalonly/>
        [Syncfusion.Documentation.DocumentationExclude]
        protected override void InitLayout()
        {
            Initialize();
            base.InitLayout();
        }

        /// <internalonly/>
        [Syncfusion.Documentation.DocumentationExclude]
        protected override void OnPaint(PaintEventArgs e)
        {
            this.Model.ActiveGridView = this;
            if (Model.changed)
            {
                Model.BeginUpdate(BeginUpdateOptions.None);
                Model.ResizeColumns();
                Model.EndUpdate(false);
            }

            base.OnPaint(e);
        }

        /// <summary>
        /// To apply color to paint the Treelines
        /// </summary>
        public void GetTreeLineBrush(out Brush brush, out Pen pen)
        {
            if (this.GroupingControl != null)
            {
                visualStyle = this.GroupingControl.GridVisualStyles;
                if (TreeLineColor != Color.Empty && !penChanged)
                {
                    pen = new Pen(TreeLineColor);
                    brush = new SolidBrush(TreeLineColor);
                }
                else
                {
                    if (this.GroupingControl.TableModel.EnableLegacyStyle)
                    {
                        switch (visualStyle)
                        {
                            case GridVisualStyles.Office2007Blue:
                            case GridVisualStyles.Office2010Blue:
                            case GridVisualStyles.Office2003:
                                pen = new Pen(Color.MidnightBlue);
                                brush = new SolidBrush(Color.MidnightBlue);
                                break;
                            case GridVisualStyles.Office2010Black:
                                pen = new Pen(Color.WhiteSmoke);
                                brush = new SolidBrush(Color.WhiteSmoke);
                                break;
                            case GridVisualStyles.Office2007Black:
                            case GridVisualStyles.Office2007Silver:
                            case GridVisualStyles.Office2010Silver:
                                pen = new Pen(Color.Black);
                                brush = new SolidBrush(Color.Black);
                                break;
                            case GridVisualStyles.Metro:
                                pen = new Pen(Color.White);
                                brush = new SolidBrush(Color.White);
                                break;
                            default:
                                pen = new Pen(Color.Black);
                                brush = new SolidBrush(Color.Black);
                                break;
                        }
                    }
                    else
                    {
                        switch (visualStyle)
                        {
                            case GridVisualStyles.Office2007Black:
                                pen = new Pen(Color.FromArgb(229, 229, 229));
                                brush = new SolidBrush(Color.FromArgb(229, 229, 229));
                                break;
                            case GridVisualStyles.Office2010Black:
                                pen = new Pen(Color.WhiteSmoke);
                                brush = new SolidBrush(Color.WhiteSmoke);
                                break;
                            case GridVisualStyles.Office2007Blue:
                            case GridVisualStyles.Office2010Blue:
                            case GridVisualStyles.Office2003:
                            case GridVisualStyles.Office2007Silver:
                            case GridVisualStyles.Office2010Silver:
                                pen = new Pen(Color.Black);
                                brush = new SolidBrush(Color.Black);
                                break;
                            case GridVisualStyles.Metro:
                                pen = new Pen(Color.White);
                                brush = new SolidBrush(Color.White);
                                break;
                            default:
                                pen = new Pen(Color.Black);
                                brush = new SolidBrush(Color.Black);
                                break;
                        }
                    }
                }
            }
            else
            {
                pen = new Pen(Color.Black);
                brush = new SolidBrush(Color.Black);
            }
        }


        /// <summary>
        /// Handler to draw the TreeLines
        /// </summary>
        protected override void OnCellDrawn(GridDrawCellEventArgs e)
        {
            if (this.GroupingControl != null && this.GroupingControl.HierarchicalGroupDropArea)
            {
                if (this.Model.Table.TableDescriptor.GroupedColumns.Count > 1)
                {
                    Pen pen1;
                    Brush brush;
                    if (!penChanged)
                        GetTreeLineBrush(out brush, out pen1);
                    else
                    {
                        pen1 = new Pen(treeLineColor);
                        brush = new SolidBrush(treeLineColor);
                    }
                    if (TreeLinePlacement == TreeLinePlacement.Bottom)
                    {
                        if (e.RowIndex >= 2 && e.RowIndex <= this.Model.RowCount)
                        {
                            if (e.ColIndex % 2 == 0 && e.ColIndex >= 4 && e.RowIndex == ((e.ColIndex / 2) + 2))
                            {
                                Rectangle cellrect = e.Renderer.GetCellClientRectangle(e.RowIndex, e.ColIndex, e.Style, true);
                                int p0 = cellrect.X + (cellrect.Width / 2);
                                int p1 = cellrect.Y + (cellrect.Height / 2);
                                int p2 = p1 + cellrect.Width;
                                Point pt0 = new Point(p0, cellrect.Y - 1);
                                Point pt1 = new Point(pt0.X, (cellrect.Y + cellrect.Height / 2));
                                Point pt2 = new Point(pt1.X + (cellrect.Width / 2) + 1, pt1.Y);// 1 added to hide borders
                                e.Graphics.DrawLine(pen1, pt0, pt1);
                                e.Graphics.DrawLine(pen1, pt1, pt2);
                                brush.Dispose();
                                pen1.Dispose();
                            }
                            if (e.ColIndex % 2 == 1 && e.ColIndex >= 4 && e.RowIndex == ((e.ColIndex / 2) + 2))
                            {
                                Rectangle cellrect = e.Renderer.GetCellClientRectangle(e.RowIndex, e.ColIndex, e.Style, true);
                                int Xp0 = cellrect.X - 1;// +(cellrect.Height / 2);// 1 added to hide borders
                                int Yp0 = cellrect.Y + (cellrect.Height / 2);
                                int Xp1 = Xp0 + cellrect.Width;
                                int Yp1 = Yp0;
                                Point pt0 = new Point(Xp0, Yp0);
                                Point pt1 = new Point(Xp1 + 1, Yp1);
                                e.Graphics.DrawLine(pen1, pt0, pt1);
                                brush.Dispose();
                                pen1.Dispose();
                            }
                        }
                    }
                    else
                    {
                        if (e.RowIndex >= 2 && e.RowIndex <= this.Model.RowCount && e.ColIndex <= (this.Model.Table.TableDescriptor.GroupedColumns.Count * 2) + 2)
                        {
                            if (e.ColIndex % 2 == 1 && e.ColIndex >= 4 && e.RowIndex == (e.ColIndex / 2))
                            {
                                Rectangle cellrect = e.Renderer.GetCellClientRectangle(e.RowIndex, e.ColIndex, e.Style, true);
                                int Xp0 = cellrect.X - 1;// 1 subtracted to hide borders
                                int Yp0 = cellrect.Y + (cellrect.Height / 2);
                                int Xp1 = Xp0 + cellrect.Width + 1;
                                int Yp1 = Yp0;
                                Point pt0 = new Point(Xp0, Yp0);
                                Point pt1 = new Point(Xp1, Yp1);
                                e.Graphics.DrawLine(pen1, pt0, pt1);
                                brush.Dispose();
                                pen1.Dispose();
                            }
                            if (e.ColIndex % 2 == 0 && e.ColIndex > 4 && e.RowIndex == ((e.ColIndex / 2) - 1))
                            {
                                Rectangle cellrect = e.Renderer.GetCellClientRectangle(e.RowIndex, e.ColIndex, e.Style, true);
                                int Xp0 = cellrect.X - 1;
                                int Yp0 = cellrect.Y + (cellrect.Height / 2);
                                int Xp1 = Xp0 + (cellrect.Width / 2) + 1;
                                int Yp1 = Yp0;
                                int Xp2 = Xp1;
                                int Yp2 = Yp1 + (cellrect.Height / 2) + 1;
                                Point pt0 = new Point(Xp0, Yp0);
                                Point pt1 = new Point(Xp1, Yp1);
                                Point pt2 = new Point(Xp2, Yp2);
                                e.Graphics.DrawLine(pen1, pt0, pt1);
                                e.Graphics.DrawLine(pen1, pt1, pt2);
                                brush.Dispose();
                                pen1.Dispose();
                            }
                        }
                    }
                }
            }
            base.OnCellDrawn(e);
        }

        /// <internalonly/>
        [Syncfusion.Documentation.DocumentationExclude]
        protected override void OnCellClick(GridCellClickEventArgs e)
        {
            base.OnCellClick(e);
            if (!e.Cancel)
            {
                GridStyleInfo style = this.GetViewStyleInfo(e.RowIndex, e.ColIndex);
                GridRangeInfo range = this.Model.CoveredRanges.FindRange(e.RowIndex, e.ColIndex);

                if (string.IsNullOrEmpty(style.Description) && range.Contains(GridRangeInfo.Cell(e.RowIndex, e.ColIndex)))
                {
                    GridStyleInfo coveredcellstyle = this.GetViewStyleInfo(e.RowIndex - 1, e.ColIndex);
                    style.Description = coveredcellstyle.Description;
                }

                Rectangle cellBounds = this.GetCellRenderer(e.RowIndex, e.ColIndex).GetCellBoundsCore(e.RowIndex, e.ColIndex);
                Rectangle innerBounds;
                Rectangle[] buttonsBounds = new Rectangle[1];
                int buttonWidth = 8;
                innerBounds = cellBounds;
                Rectangle buttonArea = Rectangle.FromLTRB((((innerBounds.Right + 2 - (buttonWidth * 2)))), (innerBounds.Top - 1), (innerBounds.Right), innerBounds.Bottom);
                buttonsBounds[0] = GridUtil.CenterInRect(buttonArea, new Size(buttonWidth, 20));
                if ((style.CellType == "CustomColumnHeaderCell" || style.CellType == GridCellTypeName.Static) && buttonsBounds[0].Contains(e.MouseEventArgs.X, e.MouseEventArgs.Y)&& 
                    this.GroupingControl!=null && this.GroupingControl.GridGroupDropArea.AllowRemove)
                {
                    this.Model.Table.TableDescriptor.GroupedColumns.Remove(style.Description.ToString());
                }
                else
                {
                    GridTable table = Model.Table;
                    GridColumnDescriptor cd = table.TableDescriptor.Columns.FindByMappingName(style.Description);
                    SortColumnDescriptor sd = table.TableDescriptor.GroupedColumns[cd == null ? style.Description : cd.MappingName];
                    if (sd != null)
                    {
                        GridTableControl tableControl = (GridTableControl)table.TableModel.ActiveGridView;
                        if (tableControl == null || tableControl.ControlEndEdit(true))
                        {
                            if (sd.SortDirection == ListSortDirection.Ascending)
                            {
                                sd.SortDirection = ListSortDirection.Descending;
                            }
                            else
                            {
                                sd.SortDirection = ListSortDirection.Ascending;
                            }
                        }
                    }
                }
            }

            Model.Table.EnsureInitialized(this, true);
        }

        ////        protected override void OnDoubleClick(EventArgs e)
        ////        {
        ////            if (Model.HasTable)
        ////            {
        ////                GridTable table = Model.Table;
        ////                table.InvalidateCounterTopDown(true);
        ////            }
        ////            base.OnDoubleClick (e);
        ////            Model.Table.EnsureInitialized(this, true);
        ////        }
        ////

        /// <internalonly/>
        [Syncfusion.Documentation.DocumentationExclude]
        protected override void Refresh(bool fromModel)
        {
            ViewLayout.Reset();
            base.Refresh(fromModel);
            ////Model.Table.EnsureInitialized(this, true);
        }

        /// <override/>
        /// <summary>Specifies the font used to display text in the grid.</summary>
        [Description(@"The font used to display text in the grid."),
        AmbientValue(null),
        Category(@"Appearance")]
        [RefreshProperties(RefreshProperties.All)]
        public override Font Font
        {
            get
            {
                return IsDisposing || IsDisposed ? base.Font : Model.TableStyle.GdipFont;
            }

            set
            {
                if (value != null)
                {
                    GridFontInfo font = Model.TableStyle.Font;
                    font.Facename = value.FontFamily.Name;
                    font.FontStyle = value.Style;
                    font.Unit = value.Unit;
                    font.Size = value.Size;
                }
            }
        }

        private bool ShouldSerializeFont()
        {
            return Model.TableStyle.HasFont;
        }

        /// <summary>
        /// Gets or sets the "Drag a column header here to group by that column." text that
        /// is displayed when no columns were added to GroupDropArea.
        /// </summary>
        public string DragColumnHeaderText
        {
            get
            {
                return Model.dragColumnHeaderHereText;
            }

            set
            {
                Model.dragColumnHeaderHereText = value;
            }
        }
    }

    /// <internalonly/>
    /// <summary>Used internally.</summary>
    [Syncfusion.Documentation.DocumentationExclude]
    public class GridGroupDropAreaModel : GridModel
    {
        GridTableModel gridTableModel;
        static int tableCounter = 0;
        int tableId = 0;
        internal string dragColumnHeaderHereText = Localization.SR.GetString(Localization.SR.DragColumnHeaderHereText);

        /// <summary>
        /// Used internally.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> containing the name of the <see cref="T:System.ComponentModel.Component"/>, if any, or null if the <see cref="T:System.ComponentModel.Component"/> is unnamed.
        /// </returns>
        /// <internalonly/>
        public override string ToString()
        {
            string isdisposed = IsDisposed ? ", Disposed" : string.Empty;
            string tnam = table != null ? table.ToString() : "Table = null";
            return GetType().Name + "(" + tnam + ", " + tableId.ToString() + isdisposed + ")";
        }

        /// <internalonly/>
        protected override void Dispose(bool disposing)
        {
            if (Engine.VerboseEnsureObjectLifeTime)
            {
                TraceUtil.TraceCurrentMethodInfo(table != null ? table.ToString() : "Table = null", tableId);
            }

            gridTableModel = null;
            UnwireTable();
            table = null;
            base.Dispose(disposing);
        }

        /// <internalonly/>
        /// <summary>Used internally.</summary>
        public GridGroupDropAreaModel(GridTableModel gridTableModel)
        {
            tableId = ++tableCounter;
            if (Engine.VerboseEnsureObjectLifeTime)
            {
                TraceUtil.TraceCurrentMethodInfo(tableId);
            }

            this.gridTableModel = gridTableModel;

            QueryCellModel += new GridQueryCellModelEventHandler(ModelQueryCellModel);

            CommandStack.Enabled = false;
            Rows.DefaultSize = 5;
            Cols.DefaultSize = 65;

            Options.VerticalScrollTips = false;
            Options.HorizontalScrollTips = false;
            Options.VerticalThumbTrack = false;
            Options.HorizontalThumbTrack = false;

            Options.ExcelLikeCurrentCell = false;
            Options.ShowCurrentCellBorderBehavior = GridShowCurrentCellBorder.WhenGridActive;
            Options.ExcelLikeSelectionFrame = false;
            Options.FloatCellsMode = GridFloatCellsMode.None;

            Options.NumberedRowHeaders = false;
            Options.NumberedColHeaders = false;
            Options.ResizeColsBehavior = GridResizeCellsBehavior.None;
            Options.ResizeRowsBehavior = GridResizeCellsBehavior.None;
            Options.AllowDragSelectedCols = false;
            Options.AllowDragSelectedRows = false;
            Options.AllowSelection = GridSelectionFlags.None;

            Options.ControllerOptions = GridControllerOptions.ClickCells;

            CellModels.Add("CustomColumnHeaderCell", new GroupedHeaderCellModel(this));
            CellModels.Add("ColumnHeaderCell", new GridTableColumnHeaderCellModel(this));

            RowHeights[0] = 0;
            ColWidths[0] = 0;
            IGraphicsProvider graphics = this.GetGraphicsProvider();
            float ratio = graphics.Graphics.DpiY / 96;
            globalWidth = RowHeights[2] = (int)((float)18 * ratio) + 2; ////.Table.DefaultColumnHeaderRowHeight;
            Color clrBack, headerBorderTop,headerBorderLeft;
            Options.GridVisualStylesDrawing.GetGroupDropAreaColors(out clrBack, out headerBorderTop, out headerBorderLeft);
            GridStyleInfo standard = BaseStylesMap["Standard"].StyleInfo;
            standard.Borders.All = GridBorder.Empty;
            standard.BackColor =  clrBack;//Color.FromArgb(94, 171, 222);
            standard.CellType = "Static";
            standard.Enabled = false;
            standard.Themed = true;
            this.Properties.BackgroundColor = SystemColors.ControlDark;

            RowCount = 2;
            ColCount = 100;
        }
        internal int globalWidth;
        internal static void ModelQueryCellModel(object sender, GridQueryCellModelEventArgs e)
        {
            if (e.CellModel == null)
            {
                IGridCellModelFactory pGridCellModelFactory = GridFactoryProvider.CellModelFactory;

                if (pGridCellModelFactory != null)
                {
                    e.CellModel = pGridCellModelFactory.CreateCellModel(e.CellType, ((IGridModelSource)sender).Model);
                }
            }
        }

        /// <internalonly/>
        /// <summary>Used internally.</summary>
        [ReadOnly(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public GridTableModel GridTableModel
        {
            get
            {
                return gridTableModel;
            }
        }

        GridTable table = null;

        /// <internalonly/>
        /// <summary>Used internally.</summary>
        public GridTable Table
        {
            get
            {
                if (table == null)
                {
                    if (GridTableModel != null)
                    {
                        table = GridTableModel.Table;
                    }

                    if (table != null)
                    {
                        WireTable();
                    }
                }

                return table;
            }

            set
            {
                if (table != value)
                {
                    UnwireTable();
                    table = value;
                    WireTable();
                }
            }
        }

        /// <internalonly/>
        /// <summary>Used internally.</summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasTable
        {
            get
            {
                return table != null;
            }
        }

        void WireTable()
        {
            if (table != null)
            {
                table.TableDescriptor.ItemPropertiesChanged += new EventHandler(TableDescriptor_ItemPropertiesChanged);
                table.TableDescriptor.Columns.Changed += new ListPropertyChangedEventHandler(Columns_Changed);
                table.TableDescriptor.GroupedColumns.Changed += new ListPropertyChangedEventHandler(GroupedColumns_Changed);
                table.TableDescriptor.TableSourceListChanged += new TableEventHandler(TableDescriptor_TableSourceListChanged);

                table.TableDescriptor.TableOptions.Changed += new Syncfusion.Styles.StyleChangedEventHandler(TableOptions_Changed);
                if (table.TableDescriptor.Engine != null)
                {
                    table.TableDescriptor.Engine.TableOptions.Changed += new Syncfusion.Styles.StyleChangedEventHandler(TableOptions_Changed);
                }

                this.ColWidths.ResizeToFit(GridRangeInfo.Cells(2, 1, 2, ColCount), GridResizeToFitOptions.None);
            }
        }

        void UnwireTable()
        {
            if (table != null)
            {
                table.TableDescriptor.ItemPropertiesChanged -= new EventHandler(TableDescriptor_ItemPropertiesChanged);
                table.TableDescriptor.GroupedColumns.Changed -= new ListPropertyChangedEventHandler(GroupedColumns_Changed);
                table.TableDescriptor.Columns.Changed -= new ListPropertyChangedEventHandler(Columns_Changed);
                table.TableDescriptor.TableSourceListChanged -= new TableEventHandler(TableDescriptor_TableSourceListChanged);

                table.TableDescriptor.TableOptions.Changed -= new Syncfusion.Styles.StyleChangedEventHandler(TableOptions_Changed);
                if (table.TableDescriptor.Engine != null)
                {
                    table.TableDescriptor.Engine.TableOptions.Changed -= new Syncfusion.Styles.StyleChangedEventHandler(TableOptions_Changed);
                }
            }
        }
        /// <summary>
        /// To apply Office scrollbars to GridGroupDropArea
        /// </summary>
        /// <param name="theme">theme to apply</param>
        private void ApplyScrollBars(string theme)
        {
            if (GridTableModel.GroupingControl != null)
            {
                switch (theme)
                {
                    case "Office2007Blue":
                        this.GridTableModel.GroupingControl.GridGroupDropArea.GridOfficeScrollBars = OfficeScrollBars.Office2007;
                        this.GridTableModel.GroupingControl.GridGroupDropArea.Office2007ScrollBarsColorScheme = Office2007ColorScheme.Blue;
                        break;
                    case "Office2007Black":
                        this.GridTableModel.GroupingControl.GridGroupDropArea.GridOfficeScrollBars = OfficeScrollBars.Office2007;
                        this.GridTableModel.GroupingControl.GridGroupDropArea.Office2007ScrollBarsColorScheme = Office2007ColorScheme.Black;
                        break;
                    case "Office2007Silver":
                        this.GridTableModel.GroupingControl.GridGroupDropArea.GridOfficeScrollBars = OfficeScrollBars.Office2007;
                        this.GridTableModel.GroupingControl.GridGroupDropArea.Office2007ScrollBarsColorScheme = Office2007ColorScheme.Silver;
                        break;
                    case "Office2010Blue":
                        this.GridTableModel.GroupingControl.GridGroupDropArea.GridOfficeScrollBars = OfficeScrollBars.Office2010;
                        this.GridTableModel.GroupingControl.GridGroupDropArea.Office2010ScrollBarsColorScheme = Office2010ColorScheme.Blue;
                        break;
                    case "Office2010Black":
                        this.GridTableModel.GroupingControl.GridGroupDropArea.GridOfficeScrollBars = OfficeScrollBars.Office2010;
                        this.GridTableModel.GroupingControl.GridGroupDropArea.Office2010ScrollBarsColorScheme = Office2010ColorScheme.Black;
                        break;
                    case "Office2010Silver":
                        this.GridTableModel.GroupingControl.GridGroupDropArea.GridOfficeScrollBars = OfficeScrollBars.Office2010;
                        this.GridTableModel.GroupingControl.GridGroupDropArea.Office2010ScrollBarsColorScheme = Office2010ColorScheme.Silver;
                        break;
                    case "Metro":
                        this.GridTableModel.GroupingControl.GridGroupDropArea.GridOfficeScrollBars = OfficeScrollBars.Metro;
                        break;
                    default:
                        this.GridTableModel.GroupingControl.GridGroupDropArea.GridOfficeScrollBars = OfficeScrollBars.None;
                        break;
                }
            }
        }
        private void TableOptions_Changed(object sender, Syncfusion.Styles.StyleChangedEventArgs e)
        {
            //// Update GroupDropArea properties - VisualStyles
            Options.GridVisualStyles = gridTableModel.Options.GridVisualStyles;
            Options.GridVisualStylesDrawing = gridTableModel.Options.GridVisualStylesDrawing;
            ApplyScrollBars(Options.GridVisualStyles.ToString());

            ////Update BackColor & Background Color

            Color clrBack = Color.Empty;
            Color headerBorderTop = Color.Empty;
            Color headerBorderLeft = Color.Empty;

            GridStyleInfo standard = BaseStylesMap["Standard"].StyleInfo;
            if (Options.GridVisualStylesDrawing.GetGroupDropAreaColors(out clrBack, out headerBorderTop, out headerBorderLeft))
            {
               standard.BackColor = clrBack;
                this.Properties.BackgroundColor = clrBack;
                if (this.gridTableModel.GroupingControl != null)
                {
                    this.gridTableModel.GroupingControl.GroupDropPanel.BackColor = clrBack;
                    this.gridTableModel.GroupingControl.Splitter.BackColor = clrBack;
                }
            }

        }

        bool IsHeaderColIndex(int colIndex)
        {
            return colIndex >= 4 && colIndex % 2 == 0 && (colIndex / 2 - 2) < Math.Max(1, Table.TableDescriptor.GroupedColumns.Count);
        }

        /// <summary>
        /// Used internally.
        /// </summary>
        /// <param name="colIndex">Index of the col.</param>
        /// <returns>returns the field</returns>
        /// <internalonly/>
        public int ColIndexToField(int colIndex)
        {
            int ci = Math.Max(4, colIndex);
            int num = Math.Max(0, Math.Min(Table.TableDescriptor.GroupedColumns.Count, (ci / 2) - 2));
            return num;
        }
        public int HighestGroupedColumn(SortColumnDescriptorCollection groupedcolumns)
        {
            int num = 0;
            int temp=0;
            foreach (SortColumnDescriptor gp in groupedcolumns)
            {
                if(gp.Name.Length>temp)
                {
                    temp = gp.Name.Length;
                    num = groupedcolumns.IndexOf(gp);
                }
            }
            
            return num;
        }
        /// <summary>
        /// Used internally.
        /// </summary>
        /// <param name="fieldNum">The field num.</param>
        /// <returns>returns the column index</returns>
        /// <internalonly/>
        public int FieldToColIndex(int fieldNum)
        {
            return (fieldNum + 2) * 2;
        }

        /// <summary>
        /// Used internally.
        /// </summary>
        /// <param name="rowIndex">Index of the row.</param>
        /// <param name="colIndex">Index of the col.</param>
        /// <returns>returns the GridColumnDescriptor</returns>
        /// <internalonly/>
        public GridColumnDescriptor GetHeaderColumnDescriptorAt(int rowIndex, int colIndex)
        {
            if (gridTableModel.HierarchicalGroupDropArea ? (!IsHeaderColIndex(colIndex)) : (rowIndex != 2 || !IsHeaderColIndex(colIndex)) && !(gridTableModel.GroupDropAlign == GridGroupDropAreaAlignment.Left || gridTableModel.GroupDropAlign == GridGroupDropAreaAlignment.Right))
            {
                return null;
            }
            int num;
            if (gridTableModel.GroupDropAlign == GridGroupDropAreaAlignment.Left || gridTableModel.GroupDropAlign == GridGroupDropAreaAlignment.Right)
            {
                num = ColIndexToField(rowIndex);
            }
            else
            {
                num = ColIndexToField(colIndex);
            }
            if (num >= Table.TableDescriptor.GroupedColumns.Count)
            {
                return null;
            }

            return Table.TableDescriptor.Columns.FindByMappingName(Table.TableDescriptor.GroupedColumns[num].Name);
        }

        /// <summary>
        /// Gets the header column descriptor at.
        /// </summary>
        /// <param name="cell">The GridRangeInfo cell.</param>
        /// <returns>returns the GridColumnDescriptor</returns>
        /// <internalonly/>
        public GridColumnDescriptor GetHeaderColumnDescriptorAt(GridRangeInfo cell)
        {
            return GetHeaderColumnDescriptorAt(cell.Top, cell.Left);
        }

        /// <summary>
        /// Used internally.
        /// </summary>
        /// <param name="cd">The GridColumnDescriptor.</param>
        /// <returns>returns the column index</returns>
        /// <internalonly/>
        public int GetColIndexOf(GridColumnDescriptor cd)
        {
            if (cd == null)
            {
                return 0;
            }

            int fieldNum = Table.TableDescriptor.GroupedColumns.IndexOf(cd.MappingName);
            if (fieldNum == -1)
            {
                return 0;
            }

            return FieldToColIndex(fieldNum);
        }

        /// <summary>
        /// Used internally.
        /// </summary>
        /// <param name="cd">The GridColumnDescriptor.</param>
        /// <returns>returns the GridRangeInfo</returns>
        /// <internalonly/>
        public GridRangeInfo GetRangeInfoOf(GridColumnDescriptor cd)
        {
            int colIndex = GetColIndexOf(cd);
            if (colIndex == 0)
            {
                return GridRangeInfo.Empty;
            }
            else
            {
                return GridRangeInfo.Cell(2, colIndex);
            }
        }

        /// <internalonly/>
        protected override void OnQueryCoveredRange(GridQueryCoveredRangeEventArgs e)
        {
            if (Model != null &&gridTableModel.HierarchicalGroupDropArea&& HasTable && this.GridTableModel.Table.TableDescriptor.GroupedColumns.Count > 1)
            {
                if ((e.ColIndex >= 4 && e.ColIndex % 2 == 0))
                {
                    if (e.RowIndex >= e.ColIndex / 2 && e.RowIndex <= (e.ColIndex / 2) + 1)
                        e.Range = GridRangeInfo.Cells((e.ColIndex / 2), e.ColIndex, (e.ColIndex / 2) + 1, e.ColIndex);
                }
                if (e.ColIndex == 2)
                {
                    if (e.RowIndex >= e.ColIndex && e.RowIndex <= (e.ColIndex ) + 1)
                        e.Range = GridRangeInfo.Cells((e.ColIndex ), e.ColIndex, (e.ColIndex ) + 1, e.ColIndex);
                }
                e.Handled = true;
            }
            base.OnQueryCoveredRange(e);
        }

        private Dictionary<int, int> HeaderCells = new Dictionary<int, int>();

        /// <internalonly/>
        protected /*internal*/ override void OnQueryCellInfo(GridQueryCellInfoEventArgs e)
        {
            if (HasTable)
            {
                GridTable table = Table;
                e.Style.VerticalAlignment = GridVerticalAlignment.Middle;
                #region [HierarchialGroupDropArea]
                if (gridTableModel.HierarchicalGroupDropArea)
                {
                    if (table != null)
                    {
                        Color clrBack = Color.Empty;
                        Color headerBorderTop = Color.Empty;
                        Color headerBorderLeft = Color.Empty;
                        Color metroTextColor = Color.FromArgb(127, 127, 127);
                        if (Options.GridVisualStyles == GridVisualStyles.Office2010Black )
                        {
                            e.Style.TextColor = Color.White;
                        }
                        else if (Options.GridVisualStyles == GridVisualStyles.Metro)
                            e.Style.TextColor = metroTextColor;
                        if (e.ColIndex == 2 && e.RowIndex == 2)
                        {
                            e.Style.Text = table.TableDescriptor.Name;
                            e.Style.Font.Bold = true;
                            if (Options.GridVisualStylesDrawing.GetGroupDropAreaColors(out clrBack, out headerBorderTop, out headerBorderLeft)
                                && Options.GridVisualStyles != GridVisualStyles.SystemTheme)
                            {
                                if (Options.GridVisualStyles == GridVisualStyles.Metro)
                                {
                                    e.Style.TextColor = Color.White;
                                }
                                e.Style.BackColor = clrBack;
                            }
                            else
                            {
                                e.Style.BackColor = SystemColors.ControlDark;
                            }

                            e.Style.HorizontalAlignment = GridHorizontalAlignment.Left;
                            e.Style.CellType = "Static";
                            e.Style.Enabled = false;
                        }
                        else if (IsHeaderColIndex(e.ColIndex) && e.RowIndex >= 2 && e.RowIndex == e.ColIndex/2)
                        {
                            int num = ColIndexToField(e.ColIndex);
                            if (num < table.TableDescriptor.GroupedColumns.Count)
                            {
                                SortColumnDescriptor sd = table.TableDescriptor.GroupedColumns[num];
                                GridColumnDescriptor cd = table.TableDescriptor.Columns.FindByMappingName(sd.Name);

                                if (!HeaderCells.ContainsKey(e.RowIndex) && !HeaderCells.ContainsValue(e.ColIndex))
                                {
                                    HeaderCells.Add(e.RowIndex , e.ColIndex);
                                }
                                int val;
                                if (HeaderCells.TryGetValue(e.RowIndex, out val))
                                {
                                    if (val == e.ColIndex)
                                    {
                                        if (gridTableModel.GroupingControl != null)
                                        {
                                            if (gridTableModel.GroupingControl.GridGroupDropArea.AllowRemove)
                                                e.Style.CellType = "CustomColumnHeaderCell";
                                            else
                                                e.Style.CellType = "ColumnHeaderCell";
                                        }
                                        else
                                        {
                                            GridGroupDropArea groupDropArea = (GridGroupDropArea)this.ActiveGridView;
                                            if (groupDropArea.AllowRemove)
                                                e.Style.CellType = "CustomColumnHeaderCell";
                                            else
                                                e.Style.CellType = "ColumnHeaderCell";
                                        }
                                        e.Style.CellValue = cd != null ? cd.HeaderText : sd.Name;
                                        e.Style.Description = sd.Name;
                                        e.Style.Tag = sd.SortDirection;
                                        if (Options.GridVisualStylesDrawing.GetGroupDropAreaColors(out clrBack, out headerBorderTop, out headerBorderLeft)
                                            && Options.GridVisualStyles != GridVisualStyles.SystemTheme)
                                        {
                                            if (Options.GridVisualStyles == GridVisualStyles.Metro)
                                            {
                                                e.Style.TextColor = Color.White;
                                            }
                                            e.Style.BackColor = clrBack;
                                            e.Style.Borders.Top = new GridBorder(GridBorderStyle.Solid, headerBorderTop, GridBorderWeight.Thin);
                                            e.Style.Borders.Left = new GridBorder(GridBorderStyle.Solid, headerBorderLeft, GridBorderWeight.Thin);
                                        }
                                     
                                        else
                                        {
                                            e.Style.BackColor = SystemColors.Control;
                                        }
                                    }
                                }
                            }
                            else if (table.TableDescriptor.GroupedColumns.Count == 0 && num == 0 && e.RowIndex == 2)
                            {
                                if (e.RowIndex == 2)
                                    e.Style.Text = dragColumnHeaderHereText;
                                if (Options.GridVisualStylesDrawing.GetGroupDropAreaColors(out clrBack, out headerBorderTop, out headerBorderLeft)
                                    && Options.GridVisualStyles != GridVisualStyles.SystemTheme)
                                {
                                    if (Options.GridVisualStyles == GridVisualStyles.Metro)
                                    {
                                        e.Style.TextColor = Color.White;
                                    }
                                    e.Style.BackColor = clrBack;
                                }
                                else
                                {
                                    e.Style.BackColor = SystemColors.Control;
                                    e.Style.Borders.All = new GridBorder(GridBorderStyle.Dotted);
                                }

                                e.Style.CellType = "Static";
                            }
                        }
                        e.Handled = true;
                    }

                }
                #endregion
                #region[left and right]
                else if (gridTableModel.GroupDropAlign == GridGroupDropAreaAlignment.Left || gridTableModel.GroupDropAlign == GridGroupDropAreaAlignment.Right)
                {
                   
                    if (e.ColIndex == 2 && table != null)
                    {
                        Color clrBack = Color.Empty;
                        Color headerBorderTop = Color.Empty;
                        Color headerBorderLeft = Color.Empty;
                        Color metroTextColor = Color.FromArgb(127, 127, 127);
                        if (Options.GridVisualStyles == GridVisualStyles.Office2010Black)
                        {
                            e.Style.TextColor = Color.White;
                        }
                        else if (Options.GridVisualStyles == GridVisualStyles.Metro)
                            e.Style.TextColor = metroTextColor;
                        e.Style.CellValue = null;
                        e.Style.CellType = "Static";
                        e.Style.Borders.All = new GridBorder(GridBorderStyle.None, Color.Empty);
                        if (e.RowIndex == 2)
                        {
                            e.Style.Text = table.TableDescriptor.Name;
                            e.Style.Font.Bold = true;
                            if (Options.GridVisualStylesDrawing.GetGroupDropAreaColors(out clrBack, out headerBorderTop, out headerBorderLeft)
                               && Options.GridVisualStyles != GridVisualStyles.SystemTheme)
                            {
                                if (Options.GridVisualStyles == GridVisualStyles.Metro)
                                    e.Style.TextColor = Color.White;
                                e.Style.BackColor = clrBack;
                            }

                            e.Style.HorizontalAlignment = GridHorizontalAlignment.Left;
                            e.Style.CellType = "Static";
                        }
                        else if (IsHeaderColIndex(e.RowIndex))
                        {
                            int num = ColIndexToField(e.RowIndex);
                            if (num < table.TableDescriptor.GroupedColumns.Count)
                            {
                                SortColumnDescriptor sd = table.TableDescriptor.GroupedColumns[num];
                                GridColumnDescriptor cd = table.TableDescriptor.Columns.FindByMappingName(sd.Name);

                                if (gridTableModel.GroupingControl != null)
                                {
                                    if (gridTableModel.GroupingControl.GridGroupDropArea.AllowRemove)
                                        e.Style.CellType = "CustomColumnHeaderCell";
                                    else
                                        e.Style.CellType = "ColumnHeaderCell";
                                }
                                else
                                {
                                    GridGroupDropArea groupDropArea = (GridGroupDropArea)this.ActiveGridView;
                                    if (groupDropArea.AllowRemove)
                                        e.Style.CellType = "CustomColumnHeaderCell";
                                    else
                                        e.Style.CellType = "ColumnHeaderCell";
                                }
                                e.Style.CellValue = cd != null ? cd.HeaderText : sd.Name;
                                e.Style.Description = sd.Name;
                                e.Style.Tag = sd.SortDirection;
                                if (Options.GridVisualStylesDrawing.GetGroupDropAreaColors(out clrBack, out headerBorderTop, out headerBorderLeft)
                                    && Options.GridVisualStyles != GridVisualStyles.SystemTheme)
                                {
                                    e.Style.BackColor = clrBack;
                                    e.Style.Borders.Top = new GridBorder(GridBorderStyle.Solid, headerBorderTop, GridBorderWeight.Thin);
                                    e.Style.Borders.Left = new GridBorder(GridBorderStyle.Solid, headerBorderLeft, GridBorderWeight.Thin);
                                }
                                else
                                {
                                    e.Style.BackColor = SystemColors.Control;
                                }
                                
                            }
                            else if (table.TableDescriptor.GroupedColumns.Count == 0 && num == 0)
                            {
                                if (Options.GridVisualStylesDrawing.GetGroupDropAreaColors(out clrBack, out headerBorderTop, out headerBorderLeft)
                                    && Options.GridVisualStyles != GridVisualStyles.SystemTheme)
                                {
                                    e.Style.BackColor = clrBack;
                                }
                                else
                                {
                                    e.Style.BackColor = SystemColors.Control;
                                    e.Style.Borders.All = new GridBorder(GridBorderStyle.Dotted);
                                }

                                e.Style.CellType = "Static";
                            }
                            e.Handled = true;
                        }
                    }
                }
                #endregion
                #region [Default]
                else
                {
                    if (e.RowIndex == 2 && table != null)
                    {
                        Color clrBack = Color.Empty;
                        Color headerBorderTop = Color.Empty;
                        Color headerBorderLeft = Color.Empty;
                        Color metroTextColor = Color.FromArgb(127, 127, 127);
                        if (Options.GridVisualStyles == GridVisualStyles.Office2010Black)
                        {
                            e.Style.TextColor = Color.White;
                        }
                        else if (Options.GridVisualStyles == GridVisualStyles.Metro)
                            e.Style.TextColor = metroTextColor;

                        if (e.ColIndex == 2)
                        {
                            e.Style.Text = table.TableDescriptor.Name;
                            e.Style.Font.Bold = true;
                            if (Options.GridVisualStylesDrawing.GetGroupDropAreaColors(out clrBack, out headerBorderTop, out headerBorderLeft)
                                && Options.GridVisualStyles != GridVisualStyles.SystemTheme)
                            {
                                if (Options.GridVisualStyles == GridVisualStyles.Metro)
                                {
                                    e.Style.TextColor = Color.White;
                                }
                                e.Style.BackColor = clrBack;
                            }
                            else
                            {
                                e.Style.BackColor = SystemColors.ControlDark;
                            }

                            e.Style.HorizontalAlignment = GridHorizontalAlignment.Left;
                            e.Style.CellType = "Static";
                        }
                        else if (IsHeaderColIndex(e.ColIndex))
                        {
                            int num = ColIndexToField(e.ColIndex);
                            if (num < table.TableDescriptor.GroupedColumns.Count)
                            {
                                SortColumnDescriptor sd = table.TableDescriptor.GroupedColumns[num];
                                GridColumnDescriptor cd = table.TableDescriptor.Columns.FindByMappingName(sd.Name);

                                if (gridTableModel.GroupingControl != null)
                                {
                                    if (gridTableModel.GroupingControl.GridGroupDropArea.AllowRemove)
                                        e.Style.CellType = "CustomColumnHeaderCell";
                                    else
                                        e.Style.CellType = "ColumnHeaderCell";
                                }
                                else
                                {
                                    GridGroupDropArea groupDropArea = (GridGroupDropArea)this.ActiveGridView;
                                    if (groupDropArea.AllowRemove)
                                        e.Style.CellType = "CustomColumnHeaderCell";
                                    else
                                        e.Style.CellType = "ColumnHeaderCell";
                                }
                                e.Style.CellValue = cd != null ? cd.HeaderText : sd.Name;
                                e.Style.Description = sd.Name;
                                e.Style.Tag = sd.SortDirection;
                                if (Options.GridVisualStylesDrawing.GetGroupDropAreaColors(out clrBack, out headerBorderTop, out headerBorderLeft)
                                    && Options.GridVisualStyles != GridVisualStyles.SystemTheme)
                                {
                                    if (Options.GridVisualStyles == GridVisualStyles.Metro)
                                    {
                                        e.Style.TextColor = Color.White;
                                    }
                                    e.Style.BackColor = clrBack;
                                    e.Style.Borders.Top = new GridBorder(GridBorderStyle.Solid, headerBorderTop, GridBorderWeight.Thin);
                                    e.Style.Borders.Left = new GridBorder(GridBorderStyle.Solid, headerBorderLeft, GridBorderWeight.Thin);
                                }
                                else
                                {
                                    e.Style.BackColor = SystemColors.Control;
                                }
                            }
                            else if (table.TableDescriptor.GroupedColumns.Count == 0 && num == 0)
                            {
                                e.Style.Text = dragColumnHeaderHereText;
                                e.Style.HorizontalAlignment = GridHorizontalAlignment.Left;
                                if (Options.GridVisualStylesDrawing.GetGroupDropAreaColors(out clrBack, out headerBorderTop, out headerBorderLeft)
                                    && Options.GridVisualStyles != GridVisualStyles.SystemTheme)
                                {
                                    if (Options.GridVisualStyles == GridVisualStyles.Metro)
                                    {
                                        e.Style.TextColor = Color.White;
                                    }
                                    e.Style.BackColor = clrBack;
                                }
                                else
                                {
                                    e.Style.BackColor = SystemColors.Control;
                                    e.Style.Borders.All = new GridBorder(GridBorderStyle.Dotted);
                                }

                                e.Style.CellType = "Static";
                            }
                        }
                        e.Handled = true;
                    }
                }
                #endregion
            }
            ////base.OnQueryCellInfo (e);
        }
        /// <internalonly/>
        protected /*internal*/ override void OnQueryColWidth(GridRowColSizeEventArgs e)
        {
            ////TraceUtil.TraceCurrentMethodInfo(e);
            if (HasTable)
            {
                GridTable table = Table;
                GridGroupingControl groupingGrid = this.Table.Engine.ParentControl as GridGroupingControl;
                ////if (changed)
                ////    ResizeColumns();
                if (table != null && IsHeaderColIndex(e.Index))
                {
                    int num = ColIndexToField(e.Index);
                    if (num > table.TableDescriptor.GroupedColumns.Count + 1)
                    {
                        e.Size = 200;
                        e.Handled = true;
                    }
                    
                }
                else if (e.Index > 0 && e.Index != 2)
                {
                    e.Size = 15;
                    e.Handled = true;
                }
                if (table.TableDescriptor.GroupedColumns.Count > 0 && e.Index > (table.TableDescriptor.GroupedColumns.Count * 2) + 4)
                {
                    e.Size = 0;
                    e.Handled = true;
                }
                if (table != null && IsHeaderColIndex(e.Index) && table.TableDescriptor.GroupedColumns.Count > 0)
                {
                    int idx = ColIndexToField(e.Index);
                    int groupedIndex = table.TableDescriptor.Columns.IndexOf(table.TableDescriptor.GroupedColumns[idx].Name);

                    int groupRowIndex = GridTableModel.HierarchicalGroupDropArea ? idx + 2 : 2;
                    GridStyleInfo style = this[groupRowIndex, e.Index];
                    int rmButtonWidth;
                    if (style.CellType == "CustomColumnHeaderCell")
                        rmButtonWidth = 10;
                    else
                        rmButtonWidth = 0;
                    if (table.TableDescriptor.Columns[table.TableDescriptor.GroupedColumns[idx].Name]!= null)
                    {
                        Size txtSize = TextRenderer.MeasureText(style.Text, style.GdipFont);
                        if(table.TableDescriptor.Columns[table.TableDescriptor.GroupedColumns[idx].Name].isImageApplied)
                            e.Size = txtSize.Width + 40 + rmButtonWidth;
                        else
                            e.Size = txtSize.Width + 20 + rmButtonWidth;
                        if (groupingGrid.EnableTouchMode)
                            e.Size += 5;
                        e.Handled = true;
                    }
                }
                if (this.GridTableModel.GroupDropAlign == GridGroupDropAreaAlignment.Left || this.GridTableModel.GroupDropAlign == GridGroupDropAreaAlignment.Right)
                {
                    if (e.Index == 1)
                        e.Size = 5;
                    if (table != null && e.Index == 2 && table.TableDescriptor.GroupedColumns.Count > 0)
                    {
                        int idx = HighestGroupedColumn(table.TableDescriptor.GroupedColumns);
                        //int idx = ColIndexToField(e.Index);
                        int groupedIndex = table.TableDescriptor.Columns.IndexOf(table.TableDescriptor.GroupedColumns[idx].Name);

                        int groupRowIndex = GridTableModel.HierarchicalGroupDropArea ? idx + 2 : 2;
                        GridStyleInfo style = this[groupRowIndex, e.Index];
                        int rmButtonWidth;
                        if (style.CellType == "CustomColumnHeaderCell")
                            rmButtonWidth = 10;
                        else
                            rmButtonWidth = 0;
                        if (table.TableDescriptor.Columns[table.TableDescriptor.GroupedColumns[idx].Name] != null)
                        {
                            Size txtSize = TextRenderer.MeasureText(style.Text, style.GdipFont);
                            if (table.TableDescriptor.Columns[table.TableDescriptor.GroupedColumns[idx].Name].isImageApplied)
                                e.Size = txtSize.Width + 40 + rmButtonWidth + 8;
                            else
                                e.Size = txtSize.Width + 20 + rmButtonWidth + 8;
                            e.Handled = true;
                        }
                    }
                }
            }

            base.OnQueryColWidth(e);
        }

        /// <internalonly/>
        /// <summary>Used internally.</summary>
        public void ResizeColumns()
        {
            changed = false;
            //GroupDropArea title column
            this.ColWidths.ResizeToFit(GridRangeInfo.Cells(2, 1, 2, 2), GridResizeToFitOptions.None);
            //GroupDropArea button column 
            //this.ColWidths.ResizeToFit(GridRangeInfo.Cells(3, 3, 2, ColCount), GridResizeToFitOptions.None);
            
            // Will take care of button column too
            this.ColWidths.ResizeToFit(GridRangeInfo.Cells(2, 4, this.RowCount, ColCount), GridResizeToFitOptions.None);

            if (Table != null && string.IsNullOrEmpty(Table.TableDescriptor.Name))
            {
                this.HideCols[2] = true;
                this.HideCols[3] = true;
            }
        }

        /// <internalonly/>
        protected /*internal*/ override void OnQueryRowHeight(GridRowColSizeEventArgs e)
        {
            if (this.Model != null && Table!=null)
            {
                GridGroupingControl groupingGrid = this.Table.Engine.ParentControl as GridGroupingControl;
                if (gridTableModel.HierarchicalGroupDropArea)
                {
                    if (e.Index >= 2 && Table.TableDescriptor.GroupedColumns.Count > 1)
                    {
                        if (groupingGrid.EnableTouchMode)
                        {
                            e.Size = 15;
                        }
                        else
                        {
                            e.Size = 10;
                        }
                        e.Handled = true;
                    }
                    else if (e.Index >= 2)
                    {
                        if (groupingGrid.EnableTouchMode)
                        {
                            e.Size = globalWidth + 7;
                        }
                        else
                        {
                            e.Size = globalWidth;
                        }
                        e.Handled = true;
                    }
                }
                else
                {
                    if (e.Index >= 2)
                    {
                        if (groupingGrid.EnableTouchMode)
                        {
                            e.Size = globalWidth + 7;
                        }
                        else
                        {
                            e.Size = globalWidth;
                        }
                        e.Handled = true;
                    }
                }
            }

            base.OnQueryRowHeight(e);
        }

        internal bool changed = true;
        private void GroupedColumns_Changed(object sender, ListPropertyChangedEventArgs e)
        {
            changed = true;
            if (this.ActiveGridView != null && this.ActiveGridView.IsHandleCreated)
            {
                this.ActiveGridView.BeginInvoke(new MethodInvoker(ActiveGridView.Refresh));
            }

            if (this.table.TableModel.HierarchicalGroupDropArea)
            {
                if ((e.Action == ListPropertyChangedType.Add || e.Action == ListPropertyChangedType.Insert) && Table.TableDescriptor.GroupedColumns.Count > 1)
                {
                    this.RowCount = Table.TableDescriptor.GroupedColumns.Count + 2;
                }
                if (e.Action == ListPropertyChangedType.Remove && Table.TableDescriptor.GroupedColumns.Count >= 1)
                {
                    this.RowCount = Table.TableDescriptor.GroupedColumns.Count > 1 ? Table.TableDescriptor.GroupedColumns.Count + 2 : 2;
                }
                if (e.Action == ListPropertyChangedType.Refresh && Table.TableDescriptor.GroupedColumns.Count == 0)
                    this.RowCount = 2;
            }
            /*this.ActiveGridView.ViewLayout.Reset();
            this.ActiveGridView.Model.ResetVolatileData();
            this.ActiveGridView.Invalidate();*/
        }

        private void Columns_Changed(object sender, ListPropertyChangedEventArgs e)
        {
            changed = true;
            if (this.ActiveGridView != null && this.ActiveGridView.IsHandleCreated)
            {
                this.ActiveGridView.BeginInvoke(new MethodInvoker(ActiveGridView.Refresh));
            }
        }

        private void TableDescriptor_ItemPropertiesChanged(object sender, EventArgs e)
        {
            changed = true;
            if (this.ActiveGridView != null && this.ActiveGridView.IsHandleCreated)
            {
                this.ActiveGridView.BeginInvoke(new MethodInvoker(ActiveGridView.Refresh));
            }

            Timer t = new Timer();
            t.Interval = 10;
            t.Tick += new EventHandler(t_Tick);
            t.Start();
        }

        bool inStop = false;

        private void t_Tick(object sender, EventArgs e)
        {
            try
            {
                //// Incident 35141 - The call to t.Stop causes issue with 
                Syncfusion.Windows.Forms.Tools.Design.ThreadHooks.IgnoreWndProcNcodeZero = true;

                Timer t = (Timer)sender;
                t.Tick -= new EventHandler(t_Tick);
                if (inStop)
                {
                    return;
                }

                inStop = true;
                t.Stop();
                t.Dispose();
                if (!this.IsDisposed && !this.IsDisposing)
                {
                    this.ResizeColumns();
                }
            }
            finally
            {
                inStop = false;
                Syncfusion.Windows.Forms.Tools.Design.ThreadHooks.IgnoreWndProcNcodeZero = false;
            }
        }

        private void TableDescriptor_TableSourceListChanged(object sender, TableEventArgs e)
        {
            Timer t = new Timer();
            t.Interval = 10;
            t.Tick += new EventHandler(t_Tick);
            t.Start();
        }
    }
}
